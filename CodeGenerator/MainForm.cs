using PluginSupport;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

namespace CodeGenerator
{
    public partial class MainForm : Form, IHost
    {
        private bool modelChanged = false;

        private readonly string dragFormat;
        private readonly DrawPanel drawPanel;
        private readonly PluginManager pm = new();

        private readonly List<Shape> shapes = [];
        private readonly List<PluginSupport.Link> links = [];

        private readonly Dictionary<string, Type> types = [];
        private readonly string caption = string.Empty;
        private string fileName = string.Empty;

        public MainForm()
        {
            InitializeComponent();
            caption = Text;
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

            drawPanel.MouseEnter += (o, e) => Cursor.Hide();
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
                var dict = plugin.GetTypes();
                foreach (var key in dict.Keys)
                    types.TryAdd(key, dict[key]);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            panLeft.Width = Properties.Settings.Default.leftpan;
            panRight.Width = Properties.Settings.Default.rightpan;
        }

        private Point? firstLinkPoint = null;
        private Point? currentPoint = null;
        private Point firstPoint = Point.Empty;
        private Shape? firstShape = null;
        private bool linkBuilding = false;
        private bool leftPressed = false;
        private bool dragShapes = false;
        private bool dragCopiedShapes = false;
        private bool frameBuilding = false;
        private Rectangle ribbonRect = Rectangle.Empty;
        private bool addShapes = false;
        private Shape? addShape;

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
                            shape.Click(point, (targetInfo) => { });
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
                    dragCopiedShapes = ModifierKeys.HasFlag(Keys.Control);
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
            var selected = shapes.Where(x => x.Selected).ToList();
            foreach (var link in links)
            {
                var sourceShape = selected.FirstOrDefault(x => x == link.Source);
                var targetShape = selected.FirstOrDefault(x => x == link.Target);
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
                    if (linkBuilding && !shape.IsLinked(index) && firstShape != null)
                    {
                        shape.CanInputLink = firstShape.GetOutputValueKind(0) == shape.GetInputValueKnd(index);
                    }
                    else
                        shape.CanInputLink = false;
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
                    var ctrl = ModifierKeys.HasFlag(Keys.Control);
                    // начинаем перемещение фигур
                    var ePoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
                    var dx = ePoint.X - firstPoint.X;
                    var dy = ePoint.Y - firstPoint.Y;
                    currentPoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
                    if (dragCopiedShapes)
                    {
                        List<Shape> copiedShapes = [];
                        List<Link> copiedLinks = [];
                        foreach (var shape in shapes)
                        {
                            if (shape.Selected)
                                copiedShapes.Add(shape.DeepClone());
                        }
                        foreach (var link in links)
                        {
                            if (link.Selected)
                                copiedLinks.Add(link.DeepClone());
                        }
                        shapes.ForEach(shape => shape.Selected = false);
                        links.ForEach(link => link.Selected = false);
                        foreach (var link in copiedLinks)
                        {
                            link.Selected = true;
                            if (copiedShapes.FirstOrDefault(x => x.Id == link.SourceId) is Shape source &&
                                copiedShapes.FirstOrDefault(x => x.Id == link.TargetId) is Shape target)
                            {
                                link.LinkToLocation(source, link.StartPoint, target, link.TargetPinIndex, link.EndPoint, [.. link.GetPoints()]);
                                links.Add(link);
                                // сохранение настроек для визуальной связи
                                link.Source = source;
                                link.SourceId = source.Id;
                                link.SourcePinIndex = link.SourcePinIndex;
                                link.Target = target;
                                link.TargetId = target.Id;
                                link.TargetPinIndex = link.TargetPinIndex;
                                // сохранение настроек связи для источника
                                source.SetOutputTarget(link.SourcePinIndex, link.Target, link.TargetPinIndex);
                                // сохранение настроек связи для цели
                                target.SetInputSource(link.TargetPinIndex, link.Source, link.SourcePinIndex);
                            }
                        }
                        foreach (var shape in copiedShapes)
                        {
                            shape.NewGuid();
                            shape.Selected = true;
                            shape.OnDelete += Shape_OnDelete;
                            shape.OnDeleteLink += Shape_OnDeleteLink;
                            shape.OnMakeCopy += Shape_OnMakeCopy;
                            shape.Index = shapes.Count;
                            shapes.Add(shape);
                        }
                       dragCopiedShapes = false;
                    }
                    // перемещаем только выбранные фигуры
                    List<Link> list = [];
                    foreach (var shape in shapes)
                    {
                        if (shape.Selected)
                        {
                            shape.Location = Point.Add(shape.Location, new Size(dx, dy));
                            var linksFromSource = links.Where(x => x.Source == shape);
                            if (shape.GetOutputPinPoint(0) is Point spoint)
                            {
                                foreach (var link in linksFromSource)
                                {
                                    link.StartPoint = spoint;
                                    if (!list.Contains(link)) list.Add(link);
                                }
                            }
                            for (var i = 0; i < shape.CountInputs(); i++)
                            {
                                var link = links.FirstOrDefault(x => x.Target == shape && x.TargetPinIndex == i);
                                if (link != null && shape.GetInputPinPoint(i) is Point tpoint)
                                    link.EndPoint = tpoint;
                            }
                        }
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
                    modelChanged = true;
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

        private static readonly int Step = TraceAssistant.CellSize; // Размер клетки

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
                        List<Shape> shapesList = [];
                        foreach (var shape in shapes.Where(x => x.Selected))
                        {
                            if (!shapesList.Contains(shape)) shapesList.Add(shape);
                            foreach (var link in links.Where(x => x.Target == shape))
                            {
                                if (link.Source != null && !shapesList.Contains(link.Source))
                                    shapesList.Add(link.Source);
                            }
                        }
                        List<Link> linksList = [];
                        foreach (var shape in shapesList)
                        {
                            shape.Location = MovePointToGrid(shape.Location);
                            var linksFromSource = links.Where(x => x.Source == shape);
                            if (shape.GetOutputPinPoint(0) is Point spoint)
                            {
                                foreach (var link in linksFromSource)
                                {
                                    link.StartPoint = spoint;
                                    if (!linksList.Contains(link)) linksList.Add(link);
                                }
                            }
                            for (var i = 0; i < shape.CountInputs(); i++)
                            {
                                var link = links.FirstOrDefault(x => x.Target == shape && x.TargetPinIndex == i);
                                if (link != null && shape.GetInputPinPoint(i) is Point tpoint)
                                {
                                    link.EndPoint = tpoint;
                                    if (!linksList.Contains(link)) linksList.Add(link);
                                }
                            }
                        }
                        linksList.ForEach(x => x.SetPoints([]));
                        foreach (var link in linksList.OrderBy(x => x.Length))
                        {
                            var points = TraceAssistant.BuildWaveInField(shapes, links, link);
                            link.SetPoints([.. points]);
                        }

                        SortIndexByLocation();
                        modelChanged = true;
                        drawPanel.Invalidate();
                    }
                    else if (linkBuilding)
                    {
                        // построение связи данных между фигурами
                        linkBuilding = false;
                        foreach (var shape in shapes.Select(x => x).Reverse())
                        {
                            var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                            if (firstShape is Shape linked &&
                                shape.IsInputTargetsPoint(point, out int index) &&
                                !shape.IsLinked(index) &&
                                firstShape.GetOutputValueKind(0) == shape.GetInputValueKnd(index))
                            {
                                // создание представления связи
                                var type = shape.GetLinkTypeToCreate();
                                var link = (PluginSupport.Link?)Activator.CreateInstance(type);
                                if (link != null)
                                {
                                    var startPoint = firstShape.GetOutputPinPoint(0);
                                    var endPoint = shape.GetInputPinPoint(index);
                                    if (startPoint != null && endPoint != null)
                                    {
                                        link.StartPoint = (Point)startPoint;
                                        link.EndPoint = (Point)endPoint;
                                        // построение волны и точек визуальной связи
                                        var points = TraceAssistant.BuildWaveInField(shapes, links, link);

                                        link.LinkToLocation(firstShape, link.StartPoint, shape, index, link.EndPoint, points);
                                        links.Add(link);
                                        // настройка фигуры для установления связи
                                        shape.SetInputValue(index, firstShape.GetOutputValue(0));

                                        // сохранение настроек для визуальной связи
                                        link.Source = firstShape;
                                        link.SourcePinIndex = 0;
                                        link.Target = shape;
                                        link.TargetPinIndex = index;
                                        // сохранение настроек связи для источника
                                        firstShape.SetOutputTarget(link.SourcePinIndex, link.Target, link.TargetPinIndex);
                                        // сохранение настроек связи для цели
                                        shape.SetInputSource(link.TargetPinIndex, link.Source, link.SourcePinIndex);
                                        modelChanged = true;
                                    }
                                }
                                break;
                            }
                        }
                        firstShape = null;
                        drawPanel.Invalidate();
                    }
                    else if (frameBuilding)
                    {
                        frameBuilding = false;
                        shapes.ForEach(shape => shape.Selected = false);

                        var mode = (currentPoint ?? Point.Empty).X > firstPoint.X;
                        foreach (var shape in shapes)
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
                    addShapes = false;
                    addShape = null;
                    shapes.ForEach(shape => shape.Selected = false);
                    var shape = draged.Shape;
                    currentPoint = MovePointToGrid(drawPanel.GetLocation(new Point(e.X, e.Y)));
                    shape.Location = (Point)currentPoint;
                    shape.Selected = true;
                    shape.OnDelete += Shape_OnDelete;
                    shape.OnDeleteLink += Shape_OnDeleteLink;
                    shape.OnMakeCopy += Shape_OnMakeCopy;
                    shapes.Add(shape);
                    SortIndexByLocation();
                    modelChanged = true;
                    drawPanel.Invalidate();
                }
            }
        }

        private void Shape_OnMakeCopy(object? sender, EventArgs e)
        {
            if (sender is Shape source)
            {
                shapes.ForEach(shape => shape.Selected = false);
                var shape = source.DeepClone();
                shape.Location = Point.Add(shape.Location, new Size(12, 12));
                shape.Selected = true;
                shape.OnDelete += Shape_OnDelete;
                shape.OnDeleteLink += Shape_OnDeleteLink;
                shape.OnMakeCopy += Shape_OnMakeCopy;
                shapes.Add(shape);
                SortIndexByLocation();
                modelChanged = true;
                drawPanel.Invalidate();
            }
        }

        private void Shape_OnDeleteLink(object sender, DeleteLinkFromTargetEventArgs e)
        {
            var linksForDelete = links.Where(x => x.Source == e.Source && 
                x.Target == e.Target && x.TargetPinIndex == e.TargetPinIndex).ToList();
            foreach (var link in linksForDelete)
            {
                // ищем источник и цель
                var source = shapes.FirstOrDefault(x => x == link.Source && link.SourcePinIndex == 0);
                var target = shapes.FirstOrDefault(x => x == link.Target && link.TargetPinIndex == e.TargetPinIndex);
                // если найдены оба, то отписывается
                if (source != null && target != null)
                    link.UnlinkToLocation(source, target);
                // удаляем визуальную ссылку
                links.Remove(link);
                UpdateOtherLinks(link);
                modelChanged = true;
            }
            drawPanel.Invalidate();
        }

        private void UpdateOtherLinks(PluginSupport.Link link)
        {
            // для всех визуальных связей, начало выходит из одной точки с перестраиваемой связью
            // очищаем массив внутренних точек
            foreach (var item in links.Where(x => x.StartPoint == link.StartPoint))
                item.SetPoints([]);
            // для всех визуальных связей, начало выходит из одной точки с перестраиваемой связью
            // перестраиваем массив внутренних точек
            foreach (var item in links.Where(x => x.StartPoint == link.StartPoint).OrderBy(x => x.Length))
                item.SetPoints([.. TraceAssistant.BuildWaveInField(shapes, links, item)]);
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
            //tsslStatus.Text = $"Смещение базовой точки: {e.Origin}, зум: {e.Zoom}";
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
            if (addShapes && addShape is Shape added && currentPoint is Point addpoint)
            {
                // рисуем перетягиваемую из библиотеки фигуру
                added.Location = drawPanel.GetLocation(MousePosition);
                using var defaultpen = new Pen(added.Foreground);
                using var brush = new SolidBrush(added.Background);
                addShape.Draw(e.Graphics, defaultpen, brush);
            }

            // рисование курсора при свободном движении указателя мыши
            if (MouseButtons.HasFlag(MouseButtons.None) && currentPoint is Point point && !addShapes)
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

            // рисуем поле трассировки связей
            //TraceAssistant.DrawField(e.Graphics);
        }

        private static void DrawPageBorder(Graphics? graphics, Point origin, int width, int height, bool bigStamp = false)
        {
            int kf = 5;
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

        private void TsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (drawPanel.Transformation == null) return;
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
                        addShape = (Shape?)Activator.CreateInstance(type);
                        if (addShape != null)
                        {
                            tvLibrary.SelectedNode = node;
                            addShapes = true;
                            var ret = tvLibrary.DoDragDrop(new DragedInfo { Shape = addShape }, DragDropEffects.Copy);
                            if (ret == DragDropEffects.None)
                            {
                                addShapes = false;
                                addShape = null;
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

                                // удаляем визуальную ссылку
                                links.Remove(link);
                                UpdateOtherLinks(link);
                            }
                            // для всех удаляемых элементов
                            foreach (var shape in shapesForDelete)
                            {
                                shape.OnDeleteLink -= Shape_OnDeleteLink;
                                shape.OnDelete -= Shape_OnDelete;
                                shape.OnMakeCopy -= Shape_OnMakeCopy;
                            }
                            foreach (var shape in shapesForDelete)
                                shapes.Remove(shape);
                            SortIndexByLocation();
                            modelChanged = true;
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

        private void Shape_OnDelete(object? sender, EventArgs e)
        {
            if (sender is Shape shapeForDelete)
            {
                timerCalculate.Enabled = false;
                try
                {
                    // сбор связей, которые используются удаляемыми элементами
                    var linksForDelete = links.Where(x => shapeForDelete == x.Source || shapeForDelete == x.Target).ToList();
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
                    shapeForDelete.OnDeleteLink -= Shape_OnDeleteLink;
                    shapeForDelete.OnDelete -= Shape_OnDelete;
                    shapeForDelete.OnMakeCopy -= Shape_OnMakeCopy;
                    shapes.Remove(shapeForDelete);
                    SortIndexByLocation();
                    modelChanged = true;
                    drawPanel.Invalidate();
                }
                finally
                {
                    timerCalculate.Enabled = true;
                }
            }
        }

        private void TsmiSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fileName))
                SaveWithDialog();
            else
            {
                try
                {
                    SaveXml(fileName);
                    UnselectAll();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Сохранение модели", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void UnselectAll()
        {
            shapes.ForEach(shape => { shape.Selected = false; shape.Hover = false; });
            links.ForEach(link => { link.Selected = false; link.Hover = false; });
        }

        private void TsmiSaveAs_Click(object sender, EventArgs e)
        {
            SaveWithDialog();
        }

        private void SaveWithDialog()
        {
            var dlg = new SaveFileDialog()
            {
                Title = "Сохранение модели",
                FileName = "",
                DefaultExt = "xml",
                Filter = "Файл модели (*.xml)|*.xml"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    fileName = dlg.FileName;
                    SaveXml(fileName);
                    UnselectAll();
                    Text = string.IsNullOrEmpty(fileName) ? caption : $"{caption} - {fileName}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Сохранение модели", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TsmiOpenFile_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                Title = "Загрузка ранее сохранённой модели",
                FileName = "",
                DefaultExt = "xml",
                Filter = "Файл модели (*.xml)|*.xml",
                Multiselect = false,
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ClearAll();
                    fileName = dlg.FileName;
                    LoadXml(fileName);
                    modelChanged = false;
                    Text = string.IsNullOrEmpty(fileName) ? caption : $"{caption} - {fileName}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Загрузка модели", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadXml(string filename)
        {
            var xdoc = XDocument.Load(filename);
            var root = xdoc.Element("Document");
            if (root == null) return;
            var name = root.Attribute("Name")?.Value;

            var xenvironment = root.Element("Environment");
            var fp = CultureInfo.GetCultureInfo("en-US");
            var xmatrix = xenvironment?.Element("Matrix")?.Value;
            var xorigin = xenvironment?.Element("Origin");
            var xzoom = xenvironment?.Element("Zoom")?.Value;
            if (xmatrix != null && xorigin != null && xzoom != null)
            {
                var xoX = xorigin.Attribute("X")?.Value;
                var xoY = xorigin.Attribute("Y")?.Value;
                var svals = xmatrix.Split(", ");
                if (xoX != null && xoY != null && svals.Length == 6)
                {
                    var m11 = ParseHelper.ParseSingle(svals[0], fp, 1);
                    var m12 = ParseHelper.ParseSingle(svals[1], fp, 0);
                    var m21 = ParseHelper.ParseSingle(svals[2], fp, 0);
                    var m22 = ParseHelper.ParseSingle(svals[3], fp, 1);
                    var dx = ParseHelper.ParseSingle(svals[4], fp, 0);
                    var dy = ParseHelper.ParseSingle(svals[5], fp, 0);
                    var zoom = ParseHelper.ParseDouble(xzoom, fp, 1);

                    drawPanel.RestoreWheelData(m11, m12, m21, m22, dx, dy,
                        new Point(ParseHelper.ParseInteger(xoX, 0), ParseHelper.ParseInteger(xoY, 0)),
                        zoom);
                }
            }

            var xmodel = root.Element("Model");
            if (xmodel == null) return;
            shapes.Clear();
            int n = 0;
            foreach (var xelement in xmodel.Descendants())
            {
                if (types.ContainsKey($"{xelement.Name}"))
                {
                    var type = types[$"{xelement.Name}"];
                    var obj = Activator.CreateInstance(type);
                    if (obj is Shape shape)
                    {
                        shape.Index = n++;
                        shape.ReadContent(xelement);
                        shape.OnDelete += Shape_OnDelete;
                        shape.OnDeleteLink += Shape_OnDeleteLink;
                        shape.OnMakeCopy += Shape_OnMakeCopy;
                        shapes.Add(shape);
                    }
                    else if (obj is Link link)
                    {
                        link.ReadContent(xelement);
                        if (link.SourceIndex >= 0 && link.SourceIndex < shapes.Count &&
                            link.TargetIndex >= 0 && link.TargetIndex < shapes.Count)
                        {
                            if (shapes[link.SourceIndex] is Shape source &&
                                shapes[link.TargetIndex] is Shape target)
                            {
                                link.LinkToLocation(source, link.StartPoint, target, link.TargetPinIndex, link.EndPoint, [.. link.GetPoints()]);
                                links.Add(link);
                                // сохранение настроек для визуальной связи
                                link.Source = source;
                                link.SourcePinIndex = link.SourcePinIndex;
                                link.Target = target;
                                link.TargetPinIndex = link.TargetPinIndex;
                                // сохранение настроек связи для источника
                                source.SetOutputTarget(link.SourcePinIndex, link.Target, link.TargetPinIndex);
                                // сохранение настроек связи для цели
                                target.SetInputSource(link.TargetPinIndex, link.Source, link.SourcePinIndex);
                            }
                        }
                    }
                }
            }
            drawPanel.Invalidate();
        }

        public void SaveXml(string filename)
        {
            var root = new XElement("Document");
            root.Add(new XAttribute("Name", System.IO.Path.GetFileNameWithoutExtension(filename)));
            var doc = new XDocument(new XComment("Данные чертёжного документа"), root);
            if (drawPanel.Transformation != null)
            {
                var xenvironment = new XElement("Environment");
                root.Add(xenvironment);
                float[] el = drawPanel.Transformation.Elements;
                var fp = CultureInfo.GetCultureInfo("en-US");
                xenvironment.Add(new XElement("Matrix", string.Join(", ", el.Select(x => x.ToString(fp)))));
                var xorigin = new XElement("Origin");
                xorigin.Add(new XAttribute("X", drawPanel.Origin.X));
                xorigin.Add(new XAttribute("Y", drawPanel.Origin.Y));
                xenvironment.Add(xorigin);
                xenvironment.Add(new XElement("Zoom", drawPanel.Zoom.ToString(fp)));
            }
            var xmodel = new XElement("Model");
            root.Add(xmodel);
            foreach (var shape in shapes)
            {
                var xshape = shape.WriteContent();
                xmodel.Add(xshape);
            }
            foreach (var link in links)
            {
                var xlink = link.WriteContent();
                xmodel.Add(xlink);
            }
            doc.Save(filename);
            modelChanged = false;
        }

        private void TsmiCreate_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            timerCalculate.Enabled = false;
            try
            {
                // сбор элементов для удаления на основании их выбранности
                var shapesForDelete = shapes.ToList();
                // сбор связей, которые используются удаляемыми элементами
                var linksForDelete = links.Where(x => shapesForDelete.Any(y => y == x.Source || y == x.Target)).ToList();
                foreach (var link in linksForDelete)
                {
                    // ищем источник и цель
                    var source = shapes.FirstOrDefault(x => x == link.Source);
                    var target = shapes.FirstOrDefault(x => x == link.Target);
                    // удаляем визуальную ссылку
                    links.Remove(link);
                    UpdateOtherLinks(link);
                }
                // для всех удаляемых элементов
                foreach (var shape in shapesForDelete)
                {
                    shape.OnDeleteLink -= Shape_OnDeleteLink;
                    shape.OnDelete -= Shape_OnDelete;
                    shape.OnMakeCopy -= Shape_OnMakeCopy;
                }
                foreach (var shape in shapesForDelete)
                    shapes.Remove(shape);
                SortIndexByLocation();
                modelChanged = false;
                fileName = string.Empty;
                Text = string.IsNullOrEmpty(fileName) ? caption : $"{caption} - {fileName}";
                drawPanel.Reset();
                drawPanel.Invalidate();
            }
            finally
            {
                timerCalculate.Enabled = true;
            }
        }

        private void TimerInterface_Tick(object sender, EventArgs e)
        {
            tsmiSave.Enabled = tsbSave.Enabled = modelChanged;
            tsslStatus.Text = $"Смещение базовой точки: {drawPanel.Origin}, зум: {drawPanel.Zoom}";
        }
    }
}
