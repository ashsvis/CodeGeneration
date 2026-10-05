using PluginSupport;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Reflection;

namespace CodeGenerator
{
    public partial class MainForm : Form, IHost
    {
        private const int step = 12;

        private readonly string dragFormat;
        private readonly DrawPanel drawPanel;
        private readonly PluginManager pm = new();

        private readonly List<Shape> shapes = [];
        private readonly List<PluginSupport.Link> links = [];
        private Cell[,]? field;

        public MainForm()
        {
            InitializeComponent();
            dragFormat = $"{typeof(DragedInfo).FullName}";
            drawPanel = new DrawPanel
            {
                Dock = DockStyle.Fill,
                AllowDrop = true,
            };
            drawPanel.DragOver += DrawPanel_DragOver;
            drawPanel.DragDrop += DrawPanel_DragDrop;
            drawPanel.QueryContinueDrag += DrawPanel_QueryContinueDrag;
            drawPanel.MouseDown += DrawPanel_MouseDown;
            drawPanel.MouseMove += DrawPanel_MouseMove;
            drawPanel.MouseUp += DrawPanel_MouseUp;

            drawPanel.OnDraw += DrawPanel_OnDraw;
            drawPanel.OnPanOrZoom += DrawPanel_OnPanOrZoom;

            //drawPanel.MouseEnter += (o, e) => Cursor.Hide();
            drawPanel.MouseLeave += (o, e) =>
            {
                currentPoint = null;
                drawPanel.Invalidate();
                Cursor.Show();
            };

            panCenter.Controls.Add(drawPanel);

            tvLibrary.Nodes.Clear();
            //сканируем плагины в папке Plugins
            pm.ScanPlugins(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"));
            //перебираем плагины
            foreach (var plugin in pm.Plugins)
            {
                var rootNode = new TreeNode(plugin.Name);
                rootNode.Expand();
                tvLibrary.Nodes.Add(rootNode);
                rootNode.Nodes.AddRange(plugin.TreeNodeItems());
            }
        }

        /// <summary>
        /// Построение поля для трассировки связей
        /// </summary>
        private void InitField()
        {
            if (shapes.Count > 0)
            {
                Rectangle rect = Rectangle.Ceiling(shapes.First().Bounds);
                foreach (var shape in shapes.Skip(1))
                    rect = Rectangle.Union(rect, Rectangle.Ceiling(shape.Bounds));
                rect.Inflate(step * 5, step * 5);
                field = new Cell[rect.Width / step + 1, rect.Height / step + 1];
                // инициализация поля трассировки связей
                for (int i = 0; i < field.GetLength(0); i++)
                {
                    for (int j = 0; j < field.GetLength(1); j++)
                    {
                        field[i, j] = new Cell() { Node = new Point(i * step + rect.Left, j * step + rect.Top) };
                    }
                }
                // указание занятых ячеек поля, на которых размещены фигуры и связи
                foreach (var shape in shapes)
                {
                    var bounds = shape.Bounds;
                    var dx = (bounds.X - rect.X) / step;
                    var dy = (bounds.Y - rect.Y) / step;
                    for (int i = -1; i < bounds.Width / step + 2; i++)
                    {
                        for (var j = 0; j < bounds.Height / step + 1; j++)
                        {
                            field[i + dx, j + dy].Empty = ThroughPassage.None;
                        }
                    }
                    // занятие точек входов
                    for (var n = 0; n < shape.CountInputs(); n++)
                    {
                        var pt = shape.GetInputPinPoint(n);
                        if (pt is Point point)
                        {
                            var px = (point.X - rect.X) / step;
                            var py = (point.Y - rect.Y) / step;
                            field[px, py].Empty = ThroughPassage.None;
                        }
                    }
                    // занятие точек выходов
                    for (var n = 0; n < shape.CountOutputs(); n++)
                    {
                        var pt = shape.GetOutputPinPoint(n);
                        if (pt is Point point)
                        {
                            var px = (point.X - rect.X) / step;
                            var py = (point.Y - rect.Y) / step;
                            field[px, py].Empty = ThroughPassage.None;
                        }
                    }
                }
                // занятие точек существующих связей
                foreach (var link in links)
                {
                    var points = link.GetPoints();
                    for (var i = 0; i < points.Length; i++)
                    {
                        var point = points[i];
                        var px = (point.X - rect.X) / step;
                        var py = (point.Y - rect.Y) / step;
                        if (px >= 0 && px < field.GetLength(0) &&
                            py >= 0 && py < field.GetLength(1))
                        {
                            field[px, py].Empty = ThroughPassage.None;
                            if (i > 0 && i < points.Length - 1)
                            {
                                var ptPrev = points[i - 1];
                                var ptNext = points[i + 1];
                                if (ptPrev.X == ptNext.X && point.X == ptPrev.X)
                                {
                                    field[px, py].Empty = ThroughPassage.Vertical;
                                }
                                else if (ptPrev.Y == ptNext.Y && point.Y == ptPrev.Y)
                                {
                                    field[px, py].Empty = ThroughPassage.Horizontal;
                                }
                            }
                        }
                    }
                }
            }
            else
                field = null;
        }

        private List<Point> BuildWaveInField(PluginSupport.Link link)
        {
            List<Point> points = [];
            if (field == null) return [];
            // очистка старых точек линии связи
            link.SetPoints([]);
            InitField();
            Rectangle rect = Rectangle.Ceiling(shapes.First().Bounds);
            foreach (var shape in shapes.Skip(1))
                rect = Rectangle.Union(rect, Rectangle.Ceiling(shape.Bounds));
            rect.Inflate(step * 5, step * 5);
            // установка начальной точки волны
            var n = 1;
            var start = link.StartPoint;
            var px = (start.X - rect.X) / step;
            var py = (start.Y - rect.Y) / step;
            if (px >= 0 && px < field.GetLength(0) &&
                py >= 0 && py < field.GetLength(1))
            {
                field[px, py].Wave = n;
                field[px, py].Empty = ThroughPassage.None;
            }
            // поиск других связей из этого же выхода
            foreach (var other in links.Where(x => x != link && x.StartPoint == link.StartPoint))
            {
                foreach (var pt in other.GetPoints())
                {
                    // установка начальной точки волны во всех точках связи
                    var ox = (pt.X - rect.X) / step;
                    var oy = (pt.Y - rect.Y) / step;
                    if (ox >= 0 && ox < field.GetLength(0) &&
                        oy >= 0 && oy < field.GetLength(1))
                    {
                        field[ox, oy].Wave = n;
                        field[ox, oy].Empty = ThroughPassage.None;
                    }
                }
            }
            // установка конечной точки волны
            var end = link.EndPoint;
            var gx = (end.X - rect.X) / step;
            var gy = (end.Y - rect.Y) / step;
            if (gx >= 0 && gx < field.GetLength(0) &&
                gy >= 0 && gy < field.GetLength(1))
            {
                field[gx, gy].Empty = ThroughPassage.Both;
            }           
            // заполнение свободных ячеек номером волны
            var found = false;
            while (!found)
            {
                var changed = false;
                for (int i = 0; i < field.GetLength(0); i++)
                {
                    for (int j = 0; j < field.GetLength(1); j++)
                    {
                        if (field[i, j].Wave == n)
                        {
                            if (i - 1 >= 0 && 
                                (field[i - 1, j].Empty == ThroughPassage.Both ||
                                 field[i - 1, j].Empty == ThroughPassage.Vertical))
                            {
                                field[i - 1, j].Wave = n + 1;
                                field[i - 1, j].Empty = ThroughPassage.None;
                                changed = true;
                                if (!found)
                                    found = gx == i - 1 && gy == j;
                            }
                            if (j - 1 >= 0 && 
                                (field[i, j - 1].Empty == ThroughPassage.Both ||
                                 field[i, j - 1].Empty == ThroughPassage.Horizontal))
                            {
                                field[i, j - 1].Wave = n + 1;
                                field[i, j - 1].Empty = ThroughPassage.None;
                                changed = true;
                                if (!found)
                                    found = gx == i && gy == j - 1;
                            }
                            if (i + 1 < field.GetLength(0) && 
                                (field[i + 1, j].Empty == ThroughPassage.Both ||
                                 field[i + 1, j].Empty == ThroughPassage.Vertical))
                            {
                                field[i + 1, j].Wave = n + 1;
                                field[i + 1, j].Empty = ThroughPassage.None;
                                changed = true;
                                if (!found)
                                    found = gx == i + 1 && gy == j;
                            }
                            if (j + 1 < field.GetLength(1) && 
                                (field[i, j + 1].Empty == ThroughPassage.Both ||
                                 field[i, j + 1].Empty == ThroughPassage.Horizontal))
                            {
                                field[i, j + 1].Wave = n + 1;
                                field[i, j + 1].Empty = ThroughPassage.None;
                                changed = true;
                                if (!found)
                                    found = gx == i && gy == j + 1;
                            }
                            if (found) break;
                        }
                        if (found) break;
                    }
                }
                if (!changed) break;
                n++;
            }
            if (found)
            {
                var i = gx;
                var j = gy;
                var wave = field[i, j].Wave;
                points.Add(field[i, j].Node);
                while (wave > 0)
                {
                    if (i - 1 >= 0 && field[i - 1, j].Wave == wave)
                    {
                        i--;
                        points.Add(field[i, j].Node);
                        goto nextWave;
                    }
                    if (j - 1 >= 0 && field[i, j - 1].Wave == wave)
                    {
                        j--;
                        points.Add(field[i, j].Node);
                        goto nextWave;
                    }
                    if (j + 1 < field.GetLength(1) && field[i, j + 1].Wave == wave)
                    {
                        j++;
                        points.Add(field[i, j].Node);
                        goto nextWave;
                    }
                    if (i + 1 < field.GetLength(0) && field[i + 1, j].Wave == wave)
                    {
                        i++;
                        points.Add(field[i, j].Node);
                        goto nextWave;
                    }
                nextWave:
                    wave--;
                }
            }
            points.Reverse();
            return points;
        }

        private Point? firstLinkPoint = null;
        private Point? currentPoint = null;
        private Point firstPoint = Point.Empty;
        private Shape? firstShape = null;
        private bool linkBuilding = false;
        private bool leftPressed = false;
        private bool dragShapes = false;
        private bool frameBuilding = false;
        private Rectangle ribbonRect = Rectangle.Empty;

        private void DrawPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            firstPoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
            leftPressed = e.Button == MouseButtons.Left;
            if (e.Button == MouseButtons.Right)
                contextMenu.Items.Clear();
            dragShapes = false;
            var shapeFound = false;
            var ctrl = ModifierKeys.HasFlag(Keys.Control);
            shapes.ForEach(shape => shape.Hover = false);
            links.ForEach(link =>
            {
                link.Hover = false;
                link.Selected = false;
            });
            if (!ctrl && shapes.Count(x => x.Selected) == 1)
                shapes.ForEach(shape => shape.Selected = false);
            firstShape = null;
            firstLinkPoint = null;
            foreach (var shape in shapes.Select(x => x).Reverse())
            {
                var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                if (shape.IsPointInTargets(point))
                {
                    if (shape.IsOuputTargetsPoint(point, out int outputIndex))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            shape.Click(point, (targetInfo) =>
                            {
                                firstShape = shape;
                                linkBuilding = true;
                                firstLinkPoint = shape.GetOutputPinPoint(outputIndex);
                            });
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            contextMenu.Items.AddRange(shape.GetContextMenuItems(point, shapes.Count(x => x.Selected) > 1));
                            contextMenu.Show(drawPanel, e.Location);
                        }
                        break;
                    }
                    else if (shape.IsInputTargetsPoint(point, out int inputIndex))
                    {
                        var other = shapes.Where(x => x.Selected).Contains(shape);
                        if (!other && shapes.Count(x => x.Selected) > 1 && !ctrl)
                            shapes.ForEach(shape => shape.Selected = false);
                        shape.Selected = true;
                        shape.Hover = true;
                        shapeFound = true;
                        if (e.Button == MouseButtons.Left)
                        {
                            shape.Click(point, (targetInfo) => {});
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            contextMenu.Items.AddRange(shape.GetContextMenuItems(point, shapes.Count(x => x.Selected) > 1));
                            contextMenu.Show(drawPanel, e.Location);
                        }
                        break;
                    }
                }
                else if (shape.ContainsPoint(point, 5f / (float)drawPanel.Zoom))
                {
                    var other = shapes.Where(x => x.Selected).Contains(shape);
                    if (!other && shapes.Count(x => x.Selected) > 1 && !ctrl)
                        shapes.ForEach(shape => shape.Selected = false);
                    shape.Selected = true;
                    shape.Hover = true;
                    shapeFound = true;
                    dragShapes = true;
                    if (e.Button == MouseButtons.Right)
                    {
                        contextMenu.Items.AddRange(shape.GetContextMenuItems(point, shapes.Count(x => x.Selected) > 1));
                        contextMenu.Show(drawPanel, e.Location);
                    }
                    break;
                }
            }
            if (shapeFound)
            {
                if (e.Button == MouseButtons.Left)
                {
                    SelectLinksForBothSelectedShapes();
                }
            }
            else
            {
                shapes.ForEach(shape =>
                {
                    shape.Selected = false;
                    shape.Hover = false;
                });
                links.ForEach(link =>
                {
                    link.Hover = false;
                    link.Selected = false;
                });
                //ничего не выбрано, рисуем рамку выбора
                frameBuilding = true;
            }
            drawPanel.Invalidate();
        }

        private void SelectLinksForBothSelectedShapes()
        {
            links.ForEach(link =>
            {
                link.Hover = false;
                link.Selected = false;
            });
            var selected = shapes.OfType<ILocation>().Where(x => x.Selected).ToList();
            foreach (var link in links)
            {
                var sourceShape = selected.FirstOrDefault(x => x is ILocation source && source == link.Source);
                var targetShape = selected.FirstOrDefault(x => x is ILocation target && target == link.Target);
                if (sourceShape != null && targetShape != null)
                {
                    link.Selected = true;
                    link.Hover = true;
                }
            }
        }

        private void DrawPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            // передача положения текущего положения курсора
            currentPoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
            tsslStatus.Text = $"Смещение базовой точки: {drawPanel.Origin}, текущая точка: {e.Location}, зум: {drawPanel.GetLocation(drawPanel.PointToScreen(e.Location))}";
            Cursor = Cursors.Default;
            foreach (var shape in shapes)
            {
                shape.Hover = false;
                shape.CanInputLink = false;
                shape.CanOutputLink = false;
            }
            links.ForEach(link => link.Hover = false);
            foreach (var shape in shapes.Select(x => x).Reverse())
            {
                var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                if (firstShape == null && shape.IsOutputTargetsPoint(point) || firstShape == shape)
                {
                    Cursor = Cursors.Cross;
                    shape.CanOutputLink = true;
                    drawPanel.Invalidate();
                }
                else if (shape.IsInputTargetsPoint(point, out int index))
                {
                    Cursor = shape.IsLinked(index) ? Cursors.Arrow : linkBuilding ? Cursors.Cross : Cursors.Hand;
                    shape.CanInputLink = !shape.IsLinked(index) && linkBuilding;
                    shape.Hover = !linkBuilding;
                    drawPanel.Invalidate();
                }
                else if (shape.ContainsPoint(point, 5f / (float)drawPanel.Zoom))
                {
                    shape.Hover = true;
                    drawPanel.Invalidate();
                }
            }
            if (leftPressed)
            {
                if (dragShapes)
                {
                    // начинам перемещение фигур
                    var ePoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
                    var dx = ePoint.X - firstPoint.X;
                    var dy = ePoint.Y - firstPoint.Y;
                    currentPoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
                    // перемещаем только выбранные фигуры
                    foreach (var shape in shapes)
                    {
                        if (shape is ILocation item && item.Selected)
                            item.Location = Point.Add(item.Location, new Size(dx, dy));
                    }
                    foreach (var link in links)
                    {
                        if (link.Selected)
                        {
                            var pts = link.GetPoints();
                            for (var i = 0; i < pts.Length; i++)
                                pts[i] = Point.Add(pts[i], new Size(dx, dy));
                            link.SetPoints(pts);
                        }
                    }
                    firstPoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
                }
                else if (linkBuilding)
                {
                    // начинаем тянуть ссылку от выхода функции ко входу функции
                    currentPoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
                }
                else if (frameBuilding)
                {
                    //ничего не выбрано, рисуем рамку выбора
                    currentPoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
                    if (firstPoint is Point first && currentPoint is Point current)
                    {
                        var minX = Math.Min(first.X, current.X);
                        var minY = Math.Min(first.Y, current.Y);
                        var maxX = Math.Max(first.X, current.X);
                        var maxY = Math.Max(first.Y, current.Y);
                        ribbonRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                        var mode = current.X > first.X;
                        foreach (var shape in shapes)
                        {
                            if (mode && ribbonRect.Contains(shape.Bounds) ||
                                !mode && ribbonRect.IntersectsWith(shape.Bounds))
                            {
                                shape.Hover = true;
                            }
                        }
                    }
                }
            }
            drawPanel.Invalidate();
        }

        private const int Step = 12; // Размер клетки

        private static Point MovePointToGrid(Point point)
        {
            return new Point(point.X / Step * Step, point.Y / Step * Step);
        }

        private void DrawPanel_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (leftPressed)
                {
                    leftPressed = false;
                    if (dragShapes)
                    {
                        dragShapes = false;
                        foreach (var shape in shapes)
                        {
                            if (shape is ILocation item && item.Selected)
                                item.Location = MovePointToGrid(item.Location);
                        }
                        SortIndexByLocation();
                        foreach (var link in links)
                            link.Rebuild();
                        InitField();
                        drawPanel.Invalidate();
                    }
                    else if (linkBuilding)
                    {
                        // построение связи данных между фигурами
                        linkBuilding = false;
                        foreach (var shape in shapes.Select(x => x).Reverse())
                        {
                            var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                            if (firstShape is ILink link &&
                                shape.IsInputTargetsPoint(point, out int index) &&
                                !shape.IsLinked(index))
                            {
                                // создание представления связи
                                var type = shape.GetLinkTypeToCreate();
                                var cellLink = (PluginSupport.Link?)Activator.CreateInstance(type);
                                if (cellLink != null && firstShape is ILocation source && shape is ILocation target)
                                {
                                    var startPoint = firstShape.GetOutputPinPoint(0);
                                    var endPoint = shape.GetInputPinPoint(index);
                                    if (startPoint != null && endPoint != null)
                                    {
                                        cellLink.StartPoint = (Point)startPoint;
                                        cellLink.EndPoint = (Point)endPoint;
                                        // построение волны и точек визуальной связи
                                        var points = BuildWaveInField(cellLink);

                                        cellLink.LinkToLocation(source, cellLink.StartPoint, target, index, cellLink.EndPoint, points);
                                        links.Add(cellLink);
                                        // настройка фигуры для установления связи
                                        shape.SetInputValue(index, firstShape.GetOutputValue(0));
                                        shape.LinkInput(link, index);
                                        cellLink.OnRebuildLink += CellLink_OnRebuildLink;
                                    }
                                }
                                break;
                            }
                        }
                        firstShape = null;
                        InitField();
                        drawPanel.Invalidate();
                    }
                    else if (frameBuilding)
                    {
                        frameBuilding = false;
                        shapes.ForEach(shape => shape.Selected = false);

                        var mode = (currentPoint ?? Point.Empty).X > firstPoint.X;
                        foreach(var shape in shapes)
                        {
                            if (mode && ribbonRect.Contains(shape.Bounds) ||
                                !mode && ribbonRect.IntersectsWith(shape.Bounds))
                            {
                                shape.Selected = true;
                            }
                        }
                        SelectLinksForBothSelectedShapes();
                        ribbonRect = Rectangle.Empty;
                        drawPanel.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Событие при перестройке точек линии связи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CellLink_OnRebuildLink(object sender, RebuildLinkFromTargetEventArgs e)
        {
            UpdateOtherLinks(e.Link);
        }

        private void DrawPanel_QueryContinueDrag(object? sender, QueryContinueDragEventArgs e)
        {
            e.Action = e.EscapePressed ? DragAction.Cancel : DragAction.Continue;
        }

        private void DrawPanel_DragOver(object? sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            if (e.Data.GetData(dragFormat) != null)
            {
                if (e.AllowedEffect == DragDropEffects.Copy)
                {
                    currentPoint = MovePointToGrid(Point.Ceiling(drawPanel.GetLocation(new Point(e.X, e.Y))));
                    drawPanel.Invalidate();
                    e.Effect = DragDropEffects.Copy;
                }
                else
                    e.Effect = DragDropEffects.None;
                drawPanel.Invalidate();
            }
        }

        private void DrawPanel_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            if (e.Effect == DragDropEffects.Copy)
            {
                if (e.Data.GetData(dragFormat) is DragedInfo draged)
                {
                    shapes.ForEach(shape => shape.Selected = false);
                    var shape = draged.Shape;
                    currentPoint = MovePointToGrid(drawPanel.GetLocation(new Point(e.X, e.Y)));
                    shape.Location = (Point)currentPoint;
                    shape.Selected = true;
                    shape.OnDeleteLink += Shape_OnDeleteLink;
                    shapes.Add(shape);
                    SortIndexByLocation();
                    InitField();
                    drawPanel.Invalidate();
                }
            }
        }

        private void Shape_OnDeleteLink(object sender, DeleteLinkFromTargetEventArgs e)
        {
            var linksForDelete = links.Where(x => x.Source == e.Source && x.Target == e.Target && x.TargetPinIndex == e.TargetPinIndex).ToList();
            foreach (var link in linksForDelete)
            {
                // ищем источник и цель
                var source = shapes.FirstOrDefault(x => x == link.Source);
                var target = shapes.FirstOrDefault(x => x == link.Target);
                // если найдены оба, то отписывается
                if (source != null && target != null)
                    link.UnlinkToLocation(source, target);
                // удаляем визуальную ссылку
                links.Remove(link);
                UpdateOtherLinks(link);
            }
            InitField();
            drawPanel.Invalidate();
        }

        private void UpdateOtherLinks(Link link)
        {
            // для всех визуальных связей, начало выходит из одной точки с перестраиваемой связью
            // очищаем массив внутренних точек
            foreach (var item in links.Where(x => x.StartPoint == link.StartPoint))
                item.SetPoints([]);
            // для всех визуальных связей, начало выходит из одной точки с перестраиваемой связью
            // перестраиваем массив внутренних точек
            foreach (var item in links.Where(x => x.StartPoint == link.StartPoint).OrderBy(x => x.Length))
                item.SetPoints([.. BuildWaveInField(item)]);
        }

        private void SortIndexByLocation()
        {
            var n = 0;
            foreach (var item in shapes.OrderBy(x => x.Location.X).ThenBy(x => x.Location.Y))
            {
                item.Index = n;
                n++;
            }
            shapes.Sort(new ShapesComparer());
        }

        class ShapesComparer : IComparer<Shape>
        {
            public int Compare(Shape? x, Shape? y)
            {
                return x == null || y == null ? 0 : x.Index > y.Index ? 1 : x.Index < y.Index ? -1 : 0;
            }
        }

        public void AddControlToMainForm(Control control)
        {
            panCenter.Controls.Clear();
            panCenter.Controls.Add(control);
        }

        /// <summary>
        /// Обработка перемещений и зуммирования видового экрана,
        /// получаем точку origin и коэффициент зуммирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawPanel_OnPanOrZoom(object? sender, PanOrZoomEventArgs e)
        {
            tsslStatus.Text = $"Смещение базовой точки: {e.Origin}, зум: {e.Zoom}";
        }

        private void DrawPanel_OnDraw(object? sender, DrawEventArgs e)
        {
            DrawPageBorder(e.Graphics, new Point(0, 0), 420, 297, true);
            DrawPageBorder(e.Graphics, new Point(420, 0), 420, 297);
            // рисуем фигуры из списка
            using var hoverpen = new Pen(Color.White);
            using var selectpen = new Pen(Color.Teal);
            using var selecthoverpen = new Pen(Color.CadetBlue);
            using var hoverbrush = new SolidBrush(Color.Teal);
            using var selectbrush = new SolidBrush(Color.Teal);
            using var selecthoverbrush = new SolidBrush(Color.CadetBlue);
            foreach (var shape in shapes)
            {
                using var brush = new SolidBrush(shape.Background);
                if (shape.Hover && shape.Selected)
                    shape.Draw(e.Graphics, selecthoverpen, brush);
                else if (shape.Hover)
                    shape.Draw(e.Graphics, hoverpen, brush);
                else if (shape.CanOutputLink || shape.CanInputLink)
                {
                    //using var hoverbrush = new SolidBrush(Color.Teal);
                    shape.Draw(e.Graphics, hoverpen, hoverbrush);
                }
                else if (shape.Selected)
                    shape.Draw(e.Graphics, selectpen, brush);
                else
                {
                    using var defaultpen = new Pen(shape.Foreground);
                    shape.Draw(e.Graphics, defaultpen, brush);
                }
            }
            // рисуем связи фигур из списка
            foreach (var link in links)
            {
                if (link.Hover && link.Selected)
                    link.DrawLines(e.Graphics, selecthoverpen);
                else if (link.Hover)
                    link.DrawLines(e.Graphics, hoverpen);
                else if (link.Selected)
                    link.DrawLines(e.Graphics, selectpen);
                else
                {
                    using var pen = new Pen(link.Foreground);
                    link.DrawLines(e.Graphics, pen);
                }
            }
            if (!dragShapes)
            {
                // рисуем присоединения связей фигур из списка
                foreach (var link in links)
                {
                    if (link.Hover && link.Selected)
                        link.DrawDots(e.Graphics, selecthoverbrush);
                    else if (link.Hover)
                        link.DrawDots(e.Graphics, hoverbrush);
                    else if (link.Selected)
                        link.DrawDots(e.Graphics, selectbrush);
                    else
                    {
                        using var brush = new SolidBrush(link.Foreground);
                        link.DrawDots(e.Graphics, brush);
                    }
                }
            }
            // рисование курсора при свободном движении указателя мыши
            if (MouseButtons.HasFlag(MouseButtons.None) && currentPoint is Point point)
            {
                var cursize = (int)(50f / drawPanel.Zoom);
                using var cursorpen = new Pen(SystemColors.ControlDarkDark, 0f);
                e.Graphics?.DrawLine(cursorpen, Point.Add(point, new Size(-cursize, 0)), Point.Add(point, new Size(cursize, 0)));
                e.Graphics?.DrawLine(cursorpen, Point.Add(point, new Size(0, -cursize)), Point.Add(point, new Size(0, cursize)));
            }

            // рисуем резиновую связь в момент построения связи
            if (linkBuilding && firstLinkPoint is Point source && currentPoint is Point target)
            {
                using var linkpen = new Pen(Color.Teal);
                linkpen.StartCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                linkpen.EndCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                e.Graphics?.DrawLine(linkpen, source, target);
            }

            if (frameBuilding && ribbonRect is Rectangle rect)
            {
                var mode = (currentPoint ?? Point.Empty).X > firstPoint.X;
                using var pen = new Pen(mode ? Color.Blue : Color.Lime, 0);
                try
                {
                    pen.DashPattern = [(int)(4 / drawPanel.Zoom), (int)(4 / drawPanel.Zoom)];
                }
                catch
                {
                    pen.DashPattern = [4, 4];
                }
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                e.Graphics?.DrawRectangle(pen, rect);
            }

            //if (field != null)
            //{
            //    // рисуем поле трассировки связей
            //    for (int i = 0; i < field.GetLength(0); i++)
            //    {
            //        for (int j = 0; j < field.GetLength(1); j++)
            //        {
            //            field[i, j].Draw(e.Graphics);
            //        }
            //    }
            //}
        }

        private static void DrawPageBorder(Graphics? graphics, Point origin, int width, int height, bool bigStamp = false)
        {
            int kf = 4;
            var borderPen = new Pen(Color.FromArgb(127, Color.Gray), 0)
            {
                DashStyle = DashStyle.Dash
            };
            var borderRect = new Rectangle(origin.X * kf - 20 * kf, origin.Y * kf - 5 * kf, width * kf, height * kf);
            graphics?.DrawRectangle(borderPen, borderRect);
            var outRect = new Rectangle(borderRect.X + 20 * kf, borderRect.Y + 5 * kf,
                            borderRect.Width - 25 * kf, borderRect.Height - 10 * kf);
            graphics?.DrawRectangle(borderPen, outRect);
            if (bigStamp)
                DrawBigStamp(graphics, borderPen, outRect, kf);
            else
                DrawSmallStamp(graphics, borderPen, outRect, kf);
            DrawSideStamp(graphics, borderPen, outRect, kf);
        }

        private static void DrawBigStamp(Graphics? graphics, Pen borderPen, Rectangle outRect, int kf)
        {
            var stamptRect = new Rectangle(outRect.Right - 185 * kf, outRect.Bottom - 55 * kf, 185 * kf, 55 * kf);
            graphics?.DrawLine(borderPen, stamptRect.Location, new PointF(stamptRect.Right, stamptRect.Top));
            //if (Module.Size != PaperSize.A4 || Module.Orientation != PaperOrientation.Portrait)
            graphics?.DrawLine(borderPen, stamptRect.Location, new PointF(stamptRect.Left, stamptRect.Bottom));

            int[] steps = [10 * kf, 10 * kf, 10 * kf, 10 * kf, 15 * kf, 10 * kf];
            string[] names = ["Изм.", "Кол.уч", "Лист", "№ док.", "Подпись", "Дата"];
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            using var stampfont = new Font("Arial", 2f * kf);
            using var brush = new SolidBrush(Color.FromArgb(127, Color.Gray));
            var x = 0 * kf;
            for (var i = 0; i < steps.Length; i++)
            {
                x += steps[i];
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left + x, stamptRect.Top), new Point(stamptRect.Left + x, stamptRect.Top + 25 * kf));
                graphics?.DrawString(names[i], stampfont, brush,
                    new Point(stamptRect.Left + x - (steps[i] / 2), stamptRect.Top + 23 * kf), sf);
            }
            var y = 0 * kf;
            for (var i = 0; i < 10; i++)
            {
                y += 5 * kf;
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left, stamptRect.Top + y),
                    new Point(stamptRect.Left + 65 * kf, stamptRect.Top + y));
            }
            steps = [20 * kf, 20 * kf, 15 * kf, 10 * kf];
            names = ["Разраб.", "Пров.", "Т.контр.", "", "Н.контр.", "Утв."];
            x = 0;
            for (var i = 0; i < steps.Length; i++)
            {
                x += steps[i];
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left + x, stamptRect.Top + 25 * kf), new Point(stamptRect.Left + x, stamptRect.Bottom));
            }
            sf.Alignment = StringAlignment.Near;
            y = stamptRect.Top + 23 * kf;
            for (var i = 0; i < names.Length; i++)
            {
                y += 5 * kf;
                graphics?.DrawString(names[i], stampfont, brush, new Point(stamptRect.Left + 1 * kf, y), sf);
            }
            y = stamptRect.Top + 10 * kf;
            for (var i = 0; i < 3; i++)
            {
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left + 65 * kf, y), new PointF(stamptRect.Right, y));
                y += 15 * kf;
            }
            steps = [15 * kf, 15 * kf, 20 * kf];
            names = ["Стадия", "Лист", "Листов"];
            sf.Alignment = StringAlignment.Center;
            x = stamptRect.Right - 50 * kf;
            graphics?.DrawLine(borderPen, new Point(x, stamptRect.Top + 25 * kf), new Point(x, stamptRect.Bottom));
            graphics?.DrawLine(borderPen, new Point(x, stamptRect.Top + 30 * kf), new Point(stamptRect.Right, stamptRect.Top + 30 * kf));
            for (var i = 0; i < steps.Length; i++)
            {
                x += steps[i];
                graphics?.DrawString(names[i], stampfont, brush,
                    new Point(x - (steps[i] / 2), stamptRect.Top + 28 * kf), sf);
                if (i == steps.Length - 1) continue;
                graphics?.DrawLine(borderPen, new Point(x, stamptRect.Top + 25 * kf), new Point(x, stamptRect.Top + 40 * kf));
            }
            using var bigfont = new Font(stampfont.Name, 3f * kf);
            var pageNumRect = new Rectangle(stamptRect.Right - 35 * kf, stamptRect.Bottom - 25 * kf, 15 * kf, 10 * kf);
            graphics?.DrawString("1", bigfont, brush,
                new Point(pageNumRect.X + pageNumRect.Width / 2, pageNumRect.Y + pageNumRect.Height / 2), sf);
            var pagesCountRect = new Rectangle(stamptRect.Right - 20 * kf, stamptRect.Bottom - 25 * kf, 20 * kf, 10 * kf);
            graphics?.DrawString("3", bigfont, brush,
                new Point(pagesCountRect.X + pagesCountRect.Width / 2, pagesCountRect.Y + pagesCountRect.Height / 2), sf);
            graphics?.DrawString("Формат A3", stampfont, brush,
                new Point(outRect.Right - 25 * kf, outRect.Bottom + 2 * kf), sf);
        }

        private static void DrawSmallStamp(Graphics? graphics, Pen borderPen, Rectangle outRect, int kf)
        {
            var stamptRect = new Rectangle(outRect.Right - 185 * kf, outRect.Bottom - 15 * kf, 185 * kf, 15 * kf);
            graphics?.DrawLine(borderPen, stamptRect.Location, new Point(stamptRect.Right, stamptRect.Top));
            graphics?.DrawLine(borderPen, stamptRect.Location, new Point(stamptRect.Left, stamptRect.Bottom));
            int[] steps = [7 * kf, 10 * kf, 23 * kf, 15 * kf, 10 * kf, 110 * kf];
            string[] names = ["Изм.", "Лист", "№ докум.", "Подп.", "Дата", ""];
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var stampfont = new Font("Arial", 2f * kf);
            var brush = new SolidBrush(Color.FromArgb(127, Color.Gray));
            var x = 0 * kf;
            for (var i = 0; i < steps.Length; i++)
            {
                x += steps[i];
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left + x, stamptRect.Top), new Point(stamptRect.Left + x, stamptRect.Bottom));
                graphics?.DrawString(names[i], stampfont, brush,
                    new Point(stamptRect.Left + x - (steps[i] / 2), stamptRect.Bottom - 2 * kf), sf);
            }
            var y = 0 * kf;
            for (var i = 0; i < 3; i++)
            {
                y += 5 * kf;
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left, stamptRect.Top + y),
                    new Point(stamptRect.Left + 65 * kf, stamptRect.Top + y));
            }
            var pageRect = new Rectangle(outRect.Right - 10 * kf, outRect.Bottom - 15 * kf, 10 * kf, 7 * kf);
            using var bigfont = new Font(stampfont.Name, 3f * kf);
            var pageNumRect = new Rectangle(outRect.Right - 10 * kf, outRect.Bottom - 8 * kf, 10 * kf, 8 * kf);
            graphics?.DrawLine(borderPen, pageNumRect.Location, new PointF(pageNumRect.Right, pageNumRect.Top));

            graphics?.DrawString("Лист", stampfont, brush,
                new Point(pageRect.X + pageRect.Width / 2, pageRect.Y + pageRect.Height / 2), sf);
            if (true)
            {
                graphics?.DrawString("2", bigfont, brush,
                    new Point(pageNumRect.X + pageNumRect.Width / 2, pageNumRect.Y + pageNumRect.Height / 2), sf);
            }
            graphics?.DrawString("Формат A3", stampfont, brush,
                new Point(outRect.Right - 25 * kf, outRect.Bottom + 2 * kf), sf);
        }

        private static void DrawSideStamp(Graphics? graphics, Pen borderPen, Rectangle outRect, int kf)
        {
            int[] steps = [25 * kf, 35 * kf, 25 * kf];
            string[] names = ["Инв.№ подл.", "Подп. и дата", "Взам. инв. №"];
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            using var stampfont = new Font("Arial", 3f * kf);
            using var brush = new SolidBrush(Color.FromArgb(127, Color.Gray));
            var y = outRect.Bottom;
            for (var i = 0; i < steps.Length; i++)
            {
                graphics?.DrawLine(borderPen, new PointF(outRect.Left - 12 * kf, y), new Point(outRect.Left, y));
                y -= steps[i];
                var p = new Point(outRect.Left - 9 * kf, y + steps[i] / 2);
                DrawVerticalText(graphics, p, names[i], stampfont, brush, sf);
            }
            graphics?.DrawLine(borderPen, new Point(outRect.Left - 12 * kf, outRect.Bottom),
                new Point(outRect.Left - 12 * kf, outRect.Bottom - 85 * kf));
            graphics?.DrawLine(borderPen, new PointF(outRect.Left - 7 * kf, outRect.Bottom),
                new Point(outRect.Left - 7 * kf, outRect.Bottom - 85 * kf));
            DrawVerticalText(graphics, new Point(outRect.Left - 17 * kf, y - 1 * kf), "Согласовано", stampfont, brush,
                new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
            steps = [20 * kf, 20 * kf, 15 * kf];
            graphics?.DrawLine(borderPen, new Point(outRect.Left - 20 * kf, y), new PointF(outRect.Left, y));
            for (var i = 0; i < steps.Length; i++)
            {
                y -= steps[i];
                graphics?.DrawLine(borderPen, new Point(outRect.Left - 15 * kf, y), new Point(outRect.Left, y));
            }
            y -= 10 * kf;
            graphics?.DrawLine(borderPen, new Point(outRect.Left - 20 * kf, y), new Point(outRect.Left, y));
            var x = outRect.Left - 15 * kf;
            for (var i = 0; i < 3; i++)
            {
                graphics?.DrawLine(borderPen, new Point(x, outRect.Bottom - 85 * kf),
                    new Point(x, outRect.Bottom - 150 * kf));
                x += 5 * kf;
            }
        }

        private static void DrawVerticalText(Graphics? graphics, Point p, string text, Font stampfont, SolidBrush brush, StringFormat sf)
        {
            using var path = new GraphicsPath();
            path.AddString(text, stampfont.FontFamily, 0, stampfont.Size, p, sf);
            using var rotateMatrix = new Matrix();
            rotateMatrix.RotateAt(-90f, p);
            path.Transform(rotateMatrix);
            graphics?.FillPath(brush, path);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            panLeft.Width = Properties.Settings.Default.leftpan;
            panRight.Width = Properties.Settings.Default.rightpan;
            drawPanel.RestoreWheelData(
                Properties.Settings.Default.m11,
                Properties.Settings.Default.m12,
                Properties.Settings.Default.m21,
                Properties.Settings.Default.m22,
                Properties.Settings.Default.dx,
                Properties.Settings.Default.dy,
                Properties.Settings.Default.origin,
                Properties.Settings.Default.zoom);
            tsslStatus.Text = $"Смещение базовой точки: {drawPanel.Origin}, зум: {drawPanel.Zoom}";
        }

        private void TsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (drawPanel.Transformation == null) return;
            var el = drawPanel.Transformation.Elements;
            Properties.Settings.Default.m11 = el[0];
            Properties.Settings.Default.m12 = el[1];
            Properties.Settings.Default.m21 = el[2];
            Properties.Settings.Default.m22 = el[3];
            Properties.Settings.Default.dx = el[4];
            Properties.Settings.Default.dy = el[5];
            Properties.Settings.Default.origin = drawPanel.Origin;
            Properties.Settings.Default.zoom = drawPanel.Zoom;
            Properties.Settings.Default.leftpan = panLeft.Width;
            Properties.Settings.Default.rightpan = panRight.Width;
            Properties.Settings.Default.Save();
        }

        private void TvLibrary_MouseDown(object sender, MouseEventArgs e)
        {
            var node = tvLibrary.GetNodeAt(e.X, e.Y);
            tvLibrary.SelectedNode = null;
            if (node != null && e.Button == MouseButtons.Left)
            {
                if (node.Tag is Type type)
                {
                    try
                    {
                        var module = (Shape?)Activator.CreateInstance(type);
                        if (module != null)
                        {
                            tvLibrary.SelectedNode = node;
                            var ret = tvLibrary.DoDragDrop(new DragedInfo { Shape = module }, DragDropEffects.Copy);
                            if (ret == DragDropEffects.None)
                            {
                                Cursor = Cursors.Default;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Вставка элемента", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void TimerCalculate_Tick(object sender, EventArgs e)
        {
            foreach (var shape in shapes)
                shape.Calculate();
            foreach (var shape in shapes)
            {
                if (shape is ICycle cycle)
                    cycle.SwitchOffSubscibers();
            }
            drawPanel.Invalidate();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    break;
                case Keys.Delete:
                    if (shapes.Any(x => x.Selected) &&
                        MessageBox.Show("Удалить выбранны(й,е) объект(ы)?", "Удаление объектов",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        timerCalculate.Enabled = false;
                        try
                        {
                            // сбор элементов для удаления на основании их выбранности
                            var shapesForDelete = shapes.Where(x => x.Selected).ToList();
                            // сбор связей, которые используются удаляемыми элементами
                            var linksForDelete = links.Where(x => shapesForDelete.Any(y => y == x.Source || y == x.Target)).ToList();
                            foreach (var link in linksForDelete)
                            {
                                // ищем источник и цель
                                var source = shapes.FirstOrDefault(x => x == link.Source);
                                var target = shapes.FirstOrDefault(x => x == link.Target);
                                // если найдены оба, то отписывается
                                if (source != null && target != null)
                                    link.UnlinkToLocation(source, target);
                                link.OnRebuildLink -= CellLink_OnRebuildLink;
                                // удаляем визуальную ссылку
                                links.Remove(link);
                                UpdateOtherLinks(link);
                            }
                            // для всех удаляемых элементов
                            foreach (var shape in shapesForDelete)
                            {
                                // удаляем подписки для всех входов эелемента
                                shape.UnlinkAllInputs();
                                // ищем элементы, у которых были связаны выходы
                                foreach (var item in shapes)
                                {
                                    if (shape is ILink link)
                                        item.UnlinkOutputFor(link);
                                }
                                shape.OnDeleteLink -= Shape_OnDeleteLink;
                            }
                            foreach (var shape in shapesForDelete)
                                shapes.Remove(shape);
                            SortIndexByLocation();
                            InitField();
                            drawPanel.Invalidate();
                        }
                        finally
                        {
                            timerCalculate.Enabled = true;
                        }
                    }
                    break;
            }
        }
    }
}
