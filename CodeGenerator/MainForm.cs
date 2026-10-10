using PluginSupport;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
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
        private bool dragShapesPreview = false;
        private bool dragShapes = false;
        private bool dragCopiedShapes = false;
        private bool frameBuilding = false;
        private Rectangle ribbonRect = Rectangle.Empty;
        private bool addShapes = false;
        private Shape? addShape;

        private bool movePasted = false;
        private XElement? copycuted;
        private (GraphicsPath[], Rectangle) pasted;

        private void DrawPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            firstPoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
            leftPressed = e.Button == MouseButtons.Left;

            if (movePasted)
            {
                movePasted = false;
                if (e.Button == MouseButtons.Left)
                {
                    if (copycuted != null)
                    {
                        PasteFromXml(copycuted, shapes, links, firstPoint);
                        modelChanged = true;
                    }
                }
                return;
            }

            if (e.Button == MouseButtons.Right)
                contextMenu.Items.Clear();
            dragShapes = false;
            dragShapesPreview = false;
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
            pgProperties.SelectedObject = null;
            foreach (var shape in shapes.Select(x => x).Reverse())
            {
                var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                if (shape.IsPointInTargets(point))
                {
                    if (shape.IsOuputTargetsPoint(point, out int outputIndex))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            pgProperties.SelectedObject = shape.GetPinPoints().FirstOrDefault(x => x.IsOutput && x.PinIndex == outputIndex);
                            tcUtilites.SelectedTab = tpProperties;
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
                            pgProperties.SelectedObject = shape.GetPinPoints().FirstOrDefault(x => !x.IsOutput && x.PinIndex == inputIndex);
                            tcUtilites.SelectedTab = tpProperties;

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
                    dragShapesPreview = true;
                    dragCopiedShapes = ModifierKeys.HasFlag(Keys.Control);
                    if (e.Button == MouseButtons.Right)
                    {
                        contextMenu.Items.AddRange(shape.GetContextMenuItems(point, shapes.Count(x => x.Selected) > 1));
                        contextMenu.Show(drawPanel, e.Location);
                    }
                    else if (e.Button == MouseButtons.Left)
                    {
                        pgProperties.SelectedObject = shape;
                        tcUtilites.SelectedTab = tpProperties;
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
                currentPoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
                if (!dragShapes && dragShapesPreview &&
                    (Math.Abs(firstPoint.X - ((Point)currentPoint).X) > 3 || Math.Abs(firstPoint.Y - ((Point)currentPoint).Y) > 3))
                    dragShapes = true;
                if (dragShapes)
                {
                    var ctrl = ModifierKeys.HasFlag(Keys.Control);
                    // начинаем перемещение фигур
                    var ePoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
                    var dx = ePoint.X - firstPoint.X;
                    var dy = ePoint.Y - firstPoint.Y;
                    if (dragCopiedShapes)
                    {
                        var copyed = CopySelectedToXml();
                        var loc = shapes.Where(x => x.Selected).Select(x => x.Location).OrderBy(p => p.X).ThenBy(p => p.Y).First();
                        var size = new Size(loc.X - ePoint.X, loc.Y - ePoint.Y);
                        shapes.ForEach(x => x.Selected = false);
                        links.ForEach(x => x.Selected = false);
                        PasteFromXml(copyed, shapes, links, Point.Add(ePoint, size));
                        dragCopiedShapes = false;
                        modelChanged = true;
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
                        dragShapesPreview = false;
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

                                        link.LinkToLocation(firstShape, 0, link.StartPoint, shape, index, link.EndPoint, points);
                                        links.Add(link);
                                        // настройка фигуры для установления связи
                                        shape.SetInputValue(index, firstShape.GetOutputValue(0));

                                        // сохранение настроек для визуальной связи
                                        link.Source = firstShape;
                                        link.SourcePinIndex = 0;
                                        link.Target = shape;
                                        link.TargetPinIndex = index;
                                        // сохранение настроек связи для источника
                                        firstShape.SetOutputTarget(link.SourcePinIndex, link.Target, link.TargetPinIndex, link.Id);
                                        // сохранение настроек связи для цели
                                        shape.SetInputSource(link.TargetPinIndex, link.Source, link.SourcePinIndex, link.Id);
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
                shape.NewGuid();
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
            StampAssistant.DrawPageBorder(e.Graphics, new Point(0, 0), 420, 297, true);
            StampAssistant.DrawPageBorder(e.Graphics, new Point(420, 0), 420, 297);
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

            // рисуем перетягиваемую из библиотеки фигуру
            if (addShapes && addShape is Shape added && currentPoint is Point _)
            {
                added.Location = drawPanel.GetLocation(MousePosition);
                using var defaultpen = new Pen(added.Foreground);
                using var brush = new SolidBrush(added.Background);
                addShape.Draw(e.Graphics, defaultpen, brush);
            }

            // рисуем перетягиваемые после Paste фигуры
            if (pasted.Item2 != Rectangle.Empty && movePasted)
            {
                (GraphicsPath[] paths, Rectangle r) = pasted;
                foreach (GraphicsPath path in paths)
                {
                    var loc = drawPanel.GetLocation(MousePosition);
                    loc = Point.Add(loc, new Size(-(int)r.Left, -(int)r.Top));
                    e.Graphics?.TranslateTransform(loc.X, loc.Y);
                    e.Graphics?.DrawPath(Pens.Teal, path);
                    e.Graphics?.TranslateTransform(-loc.X, -loc.Y);
                }
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

            // рисуем резиновую рамку выделения объектов
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
                        RemoveSelected();
                    }
                    break;
            }
        }

        private void RemoveSelected()
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
                        var source = shapes.FirstOrDefault(x => x.Id == link.SourceId);
                        var target = shapes.FirstOrDefault(x => x.Id == link.TargetId);
                        if (source != null && target != null)
                        {
                            link.LinkToLocation(source, link.SourcePinIndex, link.StartPoint, target, link.TargetPinIndex, link.EndPoint, [.. link.GetPoints()]);
                            links.Add(link);
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

        public XElement CopySelectedToXml()
        {
            var xmodel = new XElement("Copy");
            foreach (var shape in shapes.Where(x => x.Selected))
            {
                var xshape = shape.WriteContent();
                xmodel.Add(xshape);
            }
            foreach (var link in links.Where(x => x.Selected))
            {
                var xlink = link.WriteContent();
                xmodel.Add(xlink);
            }
            return xmodel;
        }

        private (GraphicsPath[], Rectangle) PasteFromXml(XElement xmodel)
        {
            if (xmodel == null) return ([], Rectangle.Empty);
            List<Shape> pastedShapes = [];
            List<Link> pastedLinks = [];
            var rect = Rectangle.Empty;
            int n = 0;
            foreach (var xelement in xmodel.Descendants())
            {
                if (types.ContainsKey($"{xelement.Name}"))
                {
                    var type = types[$"{xelement.Name}"];
                    var obj = Activator.CreateInstance(type);
                    if (obj is Shape shape)
                    {
                        shape.Selected = true;
                        shape.Index = n++;
                        shape.ReadContent(xelement);
                        shape.OnDelete += Shape_OnDelete;
                        shape.OnDeleteLink += Shape_OnDeleteLink;
                        shape.OnMakeCopy += Shape_OnMakeCopy;
                        pastedShapes.Add(shape);
                        rect = rect.IsEmpty ? shape.Bounds : Rectangle.Union(rect, shape.Bounds);
                    }
                    else if (obj is Link link)
                    {
                        link.ReadContent(xelement);
                        var source = pastedShapes.FirstOrDefault(x => x.Id == link.SourceId);
                        var target = pastedShapes.FirstOrDefault(x => x.Id == link.TargetId);
                        if (source != null && target != null)
                        {
                            link.LinkToLocation(source, link.SourcePinIndex, link.StartPoint, target, link.TargetPinIndex, link.EndPoint, [.. link.GetPoints()]);
                            pastedLinks.Add(link);
                        }
                    }
                }
            }
            foreach (var link in pastedLinks)
            {
                link.Selected = true;
                var source = pastedShapes.FirstOrDefault(x => x.Id == link.SourceId);
                if (source != null)
                {
                    source.NewGuid();
                    link.SourceId = source.Id;
                }
                var target = pastedShapes.FirstOrDefault(x => x.Id == link.TargetId);
                if (target != null)
                {
                    target.NewGuid();
                    link.TargetId = target.Id;
                }
            }

            List<GraphicsPath> list = [];
            foreach (var shape in pastedShapes)
                list.AddRange(shape.GetGraphicsPaths());
            foreach (var link in pastedLinks)
                list.AddRange(link.GetLinesPaths());
            return ([.. list], rect);
        }

        private void PasteFromXml(XElement xmodel, List<Shape> shapes, List<Link> links, Point point)
        {
            if (xmodel == null) return;
            List<Shape> pastedShapes = [];
            List<Link> pastedLinks = [];
            int n = 0;
            foreach (var xelement in xmodel.Descendants())
            {
                if (types.ContainsKey($"{xelement.Name}"))
                {
                    var type = types[$"{xelement.Name}"];
                    var obj = Activator.CreateInstance(type);
                    if (obj is Shape shape)
                    {
                        shape.Selected = true;
                        shape.Index = n++;
                        shape.ReadContent(xelement);
                        shape.OnDelete += Shape_OnDelete;
                        shape.OnDeleteLink += Shape_OnDeleteLink;
                        shape.OnMakeCopy += Shape_OnMakeCopy;
                        pastedShapes.Add(shape);
                    }
                    else if (obj is Link link)
                    {
                        link.ReadContent(xelement);
                        var source = pastedShapes.FirstOrDefault(x => x.Id == link.SourceId);
                        var target = pastedShapes.FirstOrDefault(x => x.Id == link.TargetId);
                        if (source != null && target != null)
                        {
                            link.LinkToLocation(source, link.SourcePinIndex, link.StartPoint, target, link.TargetPinIndex, link.EndPoint, [.. link.GetPoints()]);
                            pastedLinks.Add(link);
                        }
                    }
                }
            }
            foreach (var link in pastedLinks)
            {
                link.Selected = true;
                var source = pastedShapes.FirstOrDefault(x => x.Id == link.SourceId);
                if (source != null)
                {
                    source.NewGuid();
                    link.SourceId = source.Id;
                }
                var target = pastedShapes.FirstOrDefault(x => x.Id == link.TargetId);
                if (target != null)
                {
                    target.NewGuid();
                    link.TargetId = target.Id;
                }
            }
            shapes.AddRange(pastedShapes);
            links.AddRange(pastedLinks);

            var p = pastedShapes.Select(x => x.Location).OrderBy(x => x.X).ThenBy(x => x.Y).First();
            foreach (var shape in pastedShapes)
            {
                shape.Location = Point.Add(shape.Location, new Size(point.X - p.X, point.Y - p.Y));
            }
            foreach (var link in pastedLinks)
            {
                link.StartPoint = Point.Add(link.StartPoint, new Size(point.X - p.X, point.Y - p.Y));
                link.EndPoint = Point.Add(link.EndPoint, new Size(point.X - p.X, point.Y - p.Y));
                var points = link.GetPoints();
                for (var i = 0; i < points.Length; i++)
                    points[i] = Point.Add(points[i], new Size(point.X - p.X, point.Y - p.Y));
                link.SetPoints(points);
            }

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
                copycuted = null;
                pasted.Item2 = Rectangle.Empty;
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
            tsmiCut.Enabled = tsmiCopy.Enabled = tsbCut.Enabled = tsbCopy.Enabled = shapes.Any(x => x.Selected);
            tsmiPaste.Enabled = tsbPaste.Enabled = copycuted != null;
        }

        private void TsmiCopy_Click(object sender, EventArgs e)
        {
            copycuted = CopySelectedToXml();
        }

        private void TsmiCut_Click(object sender, EventArgs e)
        {
            copycuted = CopySelectedToXml();
            RemoveSelected();
        }

        private void TsmiPaste_Click(object sender, EventArgs e)
        {
            if (copycuted != null && !movePasted)
            {
                shapes.ForEach(x => x.Selected = false);
                links.ForEach(x => x.Selected = false);
                pasted = PasteFromXml(copycuted);
                movePasted = true;
            }
        }

        private void TsmiSelectAll_Click(object sender, EventArgs e)
        {
            shapes.ForEach(x => x.Selected = true);
            links.ForEach(x => x.Selected = true);
            drawPanel.Invalidate();
        }
    }
}
