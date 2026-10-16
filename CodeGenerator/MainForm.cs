using PluginSupport;
using System.ComponentModel;
using System.Drawing;
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

        private readonly List<Page> pages = [];

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
            const int totalPages = 5;
            const int totalPagesInRow = 10;
            for (var i = 1; i <= totalPages; i++)
            {
                var row = (i - 1) / totalPagesInRow;
                var column = (i - 1) - row * totalPagesInRow;
                pages.Add(new Page(i, 420, 297, row, column) { PagesCount = totalPages });
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

        private Page? currentPage;

        private bool movePasted = false;
        private XElement? copycuted;
        private (GraphicsPath[], Rectangle) pasted;

        private bool runEmulate = false;

        private void DrawPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            firstPoint = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
            var page = pages.FirstOrDefault(x => x.Bounds.Contains(firstPoint));
            if (page != null && page != currentPage || page == null)
            {
                currentPage = page;
                shapes.ForEach(shape => { shape.Selected = false; shape.Hover = false; });
                links.ForEach(link => { link.Selected = false; link.Hover = false; });
            }
            if (e.Button == MouseButtons.Left && e.Clicks > 1 && !runEmulate)
            {
                foreach (var shape in ModelAssistant.ShapesFrom(page, shapes))
                {
                    if (shape.ContainsPoint(firstPoint, 5f / (float)drawPanel.Zoom))
                    {
                        shape.Click(runEmulate);
                        break;
                    }
                }

                return;
            }
            ribbonRect = Rectangle.Empty;
            frameBuilding = false;
            leftPressed = e.Button == MouseButtons.Left;

            if (movePasted)
            {
                movePasted = false;
                if (e.Button == MouseButtons.Left)
                {
                    if (copycuted != null)
                    {
                        PasteFromXml(copycuted, shapes, links, firstPoint);
                        SortIndexByLocation();
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
            ModelAssistant.ShapesFrom(page, shapes).ForEach(shape => shape.Hover = false);
            ModelAssistant.LinksFrom(page, links).ForEach(link =>
            {
                link.Hover = false;
                link.Selected = false;
            });
            if (!ctrl && shapes.Count(x => x.Selected) == 1)
                shapes.ForEach(shape => shape.Selected = false);
            firstShape = null;
            firstLinkPoint = null;
            var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
            foreach (var shape in ModelAssistant.ShapesFrom(page, shapes))
            {
                if (shape.IsPointInTargets(point))
                {
                    if (shape.IsOuputTargetsPoint(point, out int outputIndex))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            firstShape = shape;
                            linkBuilding = !runEmulate;
                            firstLinkPoint = shape.GetOutputPinPoint(outputIndex);
                        }
                        else if (e.Button == MouseButtons.Right && !runEmulate)
                        {
                            contextMenu.Items.AddRange(shape.GetContextMenuItems(point, shapes.Count(x => x.Selected) > 1));
                            contextMenu.Show(drawPanel, e.Location);
                        }
                        break;
                    }
                    else if (shape.IsInputTargetsPoint(point, out int inputIndex))
                    {
                        var other = ModelAssistant.ShapesFrom(page, shapes).Where(x => x.Selected).Contains(shape);
                        if (!other && shapes.Count(x => x.Selected) > 1 && !ctrl)
                            shapes.ForEach(shape => shape.Selected = false);
                        shape.Selected = !runEmulate;
                        shape.Hover = !runEmulate;
                        shapeFound = true;
                        if (e.Button == MouseButtons.Left)
                        {
                            if (runEmulate)
                            {
                                if (!shape.IsLinked(inputIndex))
                                    shape.SetInputValue(inputIndex, shape.GetInputValue(inputIndex) > 0 ? 0 : 1);
                            }
                        }
                        else if (e.Button == MouseButtons.Right && !runEmulate)
                        {
                            contextMenu.Items.AddRange(shape.GetContextMenuItems(point, shapes.Count(x => x.Selected) > 1));
                            contextMenu.Show(drawPanel, e.Location);
                        }
                        break;
                    }
                }
                else if (shape.ContainsPoint(point, 5f / (float)drawPanel.Zoom))
                {
                    var other = ModelAssistant.ShapesFrom(page, shapes).Where(x => x.Selected).Contains(shape);
                    if (!other && ModelAssistant.ShapesFrom(page, shapes).Count(x => x.Selected) > 1 && !ctrl)
                        ModelAssistant.ShapesFrom(page, shapes).ForEach(shape => shape.Selected = false);
                    shape.Selected = !runEmulate;
                    shape.Hover = !runEmulate;
                    shapeFound = true;
                    dragShapesPreview = !runEmulate;
                    dragCopiedShapes = ModifierKeys.HasFlag(Keys.Control);
                    if (e.Button == MouseButtons.Left && runEmulate)
                    {
                        shape.Click(runEmulate);
                        dragShapesPreview = false;
                        dragCopiedShapes = false;
                    }
                    else if (e.Button == MouseButtons.Right)
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
                    SelectLinksForBothSelectedShapes(page);
            }
            else
            {
                UnselectAll();
                //ничего не выбрано, рисуем рамку выбора
                frameBuilding = !runEmulate;
            }
            drawPanel.Invalidate();
        }

        private void SelectLinksForBothSelectedShapes(Page? page)
        {
            ModelAssistant.LinksFrom(page, links).ForEach(link =>
            {
                link.Hover = false;
                link.Selected = false;
            });
            var selected = ModelAssistant.ShapesFrom(page, shapes).Where(x => x.Selected).ToList();
            foreach (var link in ModelAssistant.LinksFrom(page, links))
            {
                var sourceShape = selected.FirstOrDefault(x => x == link.Source);
                var targetShape = selected.FirstOrDefault(x => x == link.Target);
                if (sourceShape != null && targetShape != null)
                {
                    link.Selected = !runEmulate;
                    link.Hover = !runEmulate;
                }
            }
        }

        private void DrawPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            // передача положения текущего положения курсора
            currentPoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));

            Cursor = Cursors.Default;
            var page = pages.FirstOrDefault(x => x.Bounds.Contains((Point)currentPoint));
            if (page != null && page != currentPage || page == null)
                currentPage = page;
            ModelAssistant.ShapesFrom(page, shapes).ForEach(shape =>
            {
                shape.Hover = false;
                shape.CanInputLink = false;
                shape.CanOutputLink = false;
            });
            ModelAssistant.LinksFrom(page, links).ForEach(link => link.Hover = false);
            var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
            foreach (var shape in ModelAssistant.ShapesFrom(page, shapes))
            {
                if (firstShape == null && shape.IsOutputTargetsPoint(point) || firstShape == shape)
                {
                    if (!runEmulate)
                        Cursor = Cursors.Cross;
                    shape.CanOutputLink = !runEmulate;
                    break;
                }
                else if (shape.IsInputTargetsPoint(point, out int index))
                {
                    Cursor = shape.IsLinked(index) ? Cursors.Arrow : linkBuilding ? Cursors.Cross : Cursors.Hand;
                    if (linkBuilding && !shape.IsLinked(index) && firstShape != null)
                        shape.CanInputLink = !runEmulate && firstShape.Page == shape.Page && firstShape.GetOutputValueKind(0) == shape.GetInputValueKnd(index);
                    else
                        shape.CanInputLink = false;
                    shape.Hover = !runEmulate && !linkBuilding;
                    break;
                }
                else if (shape.ContainsPoint(point, 5f / (float)drawPanel.Zoom))
                {
                    shape.Hover = !runEmulate;
                    if (runEmulate && ModifierKeys.HasFlag(Keys.Alt))
                    {
                        Cursor = Cursors.Hand;
                    }
                    break;
                }
            }

            if (leftPressed)
            {
                currentPoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
                if (!dragShapes && dragShapesPreview &&
                    (Math.Abs(firstPoint.X - ((Point)currentPoint).X) > 3 || Math.Abs(firstPoint.Y - ((Point)currentPoint).Y) > 3))
                    dragShapes = !runEmulate;
                if (dragShapes)
                {
                    // начинаем перемещение фигур
                    var ePoint = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                    var dx = ePoint.X - firstPoint.X;
                    var dy = ePoint.Y - firstPoint.Y;
                    if (dragCopiedShapes)
                    {
                        var copyed = CopySelectedToXml();
                        var loc = new Point(
                            shapes.Where(x => x.Selected).Select(x => x.Location.X).Min(),
                            shapes.Where(x => x.Selected).Select(x => x.Location.Y).Min());
                        shapes.ForEach(x => x.Selected = false);
                        links.ForEach(x => x.Selected = false);
                        PasteFromXml(copyed, shapes, links, loc);
                        SortIndexByLocation();
                        dragCopiedShapes = false;
                        modelChanged = true;
                    }
                    // перемещаем только выбранные фигуры
                    List<Link> list = [];
                    for (var n = 0; n < shapes.Count; n++)
                    {
                        var shape = shapes[n];
                        if (shape.Selected)
                        {
                            shape.Location = Point.Add(shape.Location, new Size(dx, dy));
                            var linksFromSource = links.Where(x => x.Source == shape);
                            if (shape.GetOutputPinPoint(0) is Point spoint)
                            {
                                foreach (var link in linksFromSource)
                                {
                                    if (!list.Contains(link))
                                    {
                                        link.StartPoint = spoint;
                                        list.Add(link);
                                    }
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
                    foreach (var link in links.Where(x => x.Selected))
                    {
                        var pts = link.GetPoints();
                        for (var i = 0; i < pts.Length; i++)
                        {
                            pts[i] = Point.Add(pts[i], new Size(dx, dy));
                        }
                        link.SetPoints(pts);
                    }
                    modelChanged = !runEmulate;
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
                        foreach (var shape in ModelAssistant.ShapesFrom(page, shapes))
                        {
                            if (mode && ribbonRect.Contains(shape.Bounds) ||
                                !mode && ribbonRect.IntersectsWith(shape.Bounds))
                            {
                                shape.Hover = !runEmulate;
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
                var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                var page = pages.FirstOrDefault(x => x.Bounds.Contains(point));
                if (leftPressed)
                {
                    leftPressed = false;
                    if (dragShapes)
                    {
                        Cursor = Cursors.WaitCursor;
                        Cursor.Show();
                        dragShapesPreview = false;
                        dragShapes = false;
                        foreach (var shape in ModelAssistant.ShapesFrom(page, shapes))
                            shape.Location = MovePointToGrid(shape.Location);
                        foreach (var link in ModelAssistant.LinksFrom(page, links))
                        {
                            var source = ModelAssistant.ShapesFrom(page, shapes).FirstOrDefault(x => x.Id == link.SourceId);
                            if (source != null && source.GetOutputPinPoint(link.SourcePinIndex) is Point sourcePinPoint)
                                link.StartPoint = sourcePinPoint;
                            var target = ModelAssistant.ShapesFrom(page, shapes).FirstOrDefault(x => x.Id == link.TargetId);
                            if (target != null && target.GetInputPinPoint(link.TargetPinIndex) is Point targetPinPoint)
                                link.EndPoint = targetPinPoint;
                        }
                        UpdateLinksOnPage(page);

                        SortIndexByLocation();
                        modelChanged = !runEmulate;
                        drawPanel.Refresh();
                        Cursor = Cursors.Default;
                        Cursor.Hide();
                    }
                    else if (linkBuilding)
                    {
                        // построение связи данных между фигурами
                        linkBuilding = false;
                        foreach (var shape in ModelAssistant.ShapesFrom(page, shapes))
                        {
                            if (firstShape is Shape linked &&
                                shape.IsInputTargetsPoint(point, out int index) &&
                                !shape.IsLinked(index) &&
                                firstShape.GetOutputValueKind(0) == shape.GetInputValueKnd(index))
                            {
                                if (linked.Page == 0 || shape.Page == 0)
                                {
                                    MessageBox.Show("Для создания связи" +
                                        "\nразмещайте соединяемые фигуры" +
                                        "\nв пределах одного листа.",
                                        "Создание связи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else if (linked.Page == shape.Page)
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
                                            var points = TraceAssistant.BuildWaveInField(page, shapes, links, link);

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
                                            SortIndexByLocation();
                                            modelChanged = !runEmulate;
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Для создания межстраничной связи" +
                                        "\nиспользуйте межстраничные ссылки" +
                                        "\nиз библиотеки компонентов.",
                                        "Создание связи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                break;
                            }
                        }
                        firstShape = null;
                        drawPanel.Refresh();
                    }
                    else if (frameBuilding)
                    {
                        frameBuilding = false;
                        ModelAssistant.ShapesFrom(page, shapes).ForEach(shape => shape.Selected = false);

                        var mode = (currentPoint ?? Point.Empty).X > firstPoint.X;
                        foreach (var shape in ModelAssistant.ShapesFrom(page, shapes))
                        {
                            if (mode && ribbonRect.Contains(shape.Bounds) ||
                                !mode && ribbonRect.IntersectsWith(shape.Bounds))
                            {
                                shape.Selected = !runEmulate;
                            }
                        }
                        SelectLinksForBothSelectedShapes(page);
                        ribbonRect = Rectangle.Empty;
                        drawPanel.Refresh();
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
                    SubscribeShapeEvents(shape);
                    shapes.Add(shape);
                    SortIndexByLocation();
                    modelChanged = true;
                    drawPanel.Invalidate();
                }
            }
        }

        private void SubscribeShapeEvents(Shape shape)
        {
            shape.OnShowProperties += Shape_OnShowProperties;
            shape.OnDelete += Shape_OnDelete;
            shape.OnDeleteLink += Shape_OnDeleteLink;
            shape.OnMakeCopy += Shape_OnMakeCopy;
            shape.OnQuery += Shape_OnQuery;
            shape.OnChange += Shape_OnChange;
            shape.OnClick += Shape_OnClick;
        }

        private void UnsubscribeShapeEvents(Shape shape)
        {
            shape.OnShowProperties -= Shape_OnShowProperties;
            shape.OnDelete -= Shape_OnDelete;
            shape.OnDeleteLink -= Shape_OnDeleteLink;
            shape.OnMakeCopy -= Shape_OnMakeCopy;
            shape.OnQuery -= Shape_OnQuery;
            shape.OnChange -= Shape_OnChange;
            shape.OnClick -= Shape_OnClick;
        }

        private void Shape_OnClick(object sender, ClickEventArgs e)
        {
            e.Action?.Invoke(MouseButtons, e.Args);
        }

        private void Shape_OnChange(object? sender, EventArgs e)
        {
            modelChanged = true;
            drawPanel.Invalidate();
        }

        private void Shape_OnQuery(object sender, QueryEventArgs e)
        {
            List<object> items = [];
            foreach (var arg in e.Args)
            {
                if (arg is Type type)
                    items.AddRange(shapes.Where(x => x.GetType() == type));
            }
            e.Results = [.. items];
        }

        private void Shape_OnShowProperties(object? sender, EventArgs e)
        {
            pgProperties.SelectedObject = sender;
            tcUtilites.SelectedTab = tpProperties;
        }

        private void Shape_OnMakeCopy(object? sender, EventArgs e)
        {
            if (runEmulate) return;
            if (sender is Shape source)
            {
                shapes.ForEach(shape => shape.Selected = false);
                var shape = source.DeepClone();
                shape.NewGuid();
                shape.Location = Point.Add(shape.Location, new Size(12, 12));
                SubscribeShapeEvents(shape);
                shapes.Add(shape);
                SortIndexByLocation();
                shape.Selected = true;
                modelChanged = true;
                drawPanel.Invalidate();
            }
        }

        private void Shape_OnDeleteLink(object sender, DeleteLinkFromTargetEventArgs e)
        {
            if (runEmulate) return;
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
                if (source is IPageTargetLink plink)
                {
                    plink.RemoveTargetFromOutput(target, link.TargetPinIndex);
                }
                // удаляем визуальную ссылку
                links.Remove(link);
                UpdateOtherLinks(link);
                modelChanged = true;
            }
            drawPanel.Invalidate();
        }

        private void UpdateOtherLinks(PluginSupport.Link link)
        {
            if (link.Page == 0) return;
            // для всех визуальных связей, начало выходит из одной точки с перестраиваемой связью
            // очищаем массив внутренних точек
            foreach (var item in links.Where(x => x.StartPoint == link.StartPoint))
                item.SetPoints([]);
            // для всех визуальных связей, начало выходит из одной точки с перестраиваемой связью
            // перестраиваем массив внутренних точек
            foreach (var item in links.Where(x => x.StartPoint == link.StartPoint).OrderByDescending(x => x.Length))
                item.SetPoints([.. TraceAssistant.BuildWaveInField(null, shapes, links, item)]);
        }

        private void UpdateLinksOnPage(Page? page)
        {
            foreach (var item in ModelAssistant.LinksFrom(page, links))
                item.SetPoints([]);
            foreach (var item in ModelAssistant.LinksFrom(page, links))
                item.SetPoints([.. TraceAssistant.BuildWaveInField(page, shapes, links, item)]);
        }

        private void SortIndexByLocation()
        {
            ModelAssistant.ClearCache();

            // сброс номеров страниц
            shapes.ForEach(shape => shape.Page = 0);
            links.ForEach(link => link.Page = 0);

            // назначение номеров страниц на основании попадания в область листа
            foreach (var page in pages)
            {
                foreach (var shape in shapes.Where(x => x.Page == 0))
                {
                    var p = pages.FirstOrDefault(x => x.Bounds.Contains(shape.Bounds));
                    if (p != null) shape.Page = p.Number;
                }
                foreach (var link in links.Where(x => x.Page == 0))
                {
                    var p = pages.FirstOrDefault(x => x.Bounds.Contains(link.Bounds));
                    if (p != null) link.Page = p.Number;
                }
            }

            // назначение порядкового индекса для фигур
            var n = 0;
            foreach (var shape in shapes.OrderBy(x => x.Page).ThenBy(x => x.Location.X).ThenBy(x => x.Location.Y))
            {
                shape.Index = n;    // назначение порядкового индекса
                n++;
            }
            shapes.Sort(new ShapesComparer());

            // назначение порядкового индекса для исходящих внеших ссылок
            n = 1;
            foreach (var shape in shapes.OrderBy(x => x.Index))
            {
                if (shape is IPageSourceLink link)
                {
                    link.SetLinkNumber(n++);
                }
            }
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
            foreach (var page in pages)
            {
                DrawInPage(e.Graphics, page);
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
            if (MouseButtons.HasFlag(MouseButtons.None) && !addShapes && !runEmulate)
            {
                if (currentPoint is Point point)
                {
                    var cursize = (int)(50f / drawPanel.Zoom);
                    using var cursorpen = new Pen(SystemColors.ControlDarkDark, 0f);
                    e.Graphics?.DrawLine(cursorpen, Point.Add(point, new Size(-cursize, 0)), Point.Add(point, new Size(cursize, 0)));
                    e.Graphics?.DrawLine(cursorpen, Point.Add(point, new Size(0, -cursize)), Point.Add(point, new Size(0, cursize)));
                }
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
            if (frameBuilding && !ribbonRect.IsEmpty)
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
                e.Graphics?.DrawRectangle(pen, ribbonRect);
            }

            // рисуем поле трассировки связей
            //TraceAssistant.DrawField(e.Graphics);
        }

        private void DrawInPage(Graphics? graphics, Page? page)
        {
            page?.Draw(graphics, Color.FromArgb(127, Color.Gray));
            var hoverpen = new Pen(Color.White);
            var selectpen = new Pen(Color.Teal);
            var selecthoverpen = new Pen(Color.CadetBlue);
            var hoverbrush = new SolidBrush(Color.Teal);
            var selectbrush = new SolidBrush(Color.Teal);
            var selecthoverbrush = new SolidBrush(Color.CadetBlue);
            foreach (var shape in shapes.Where(x => page != null && x.Page == page.Number || x.Page == 0))
            {
                Pen pen;
                Brush brush;
                var defaultpen = new Pen(shape.Foreground);
                var backgroundbrush = new SolidBrush(shape.Background);
                if (shape.Hover && shape.Selected)
                {
                    pen = selecthoverpen;
                    brush = backgroundbrush;
                }
                else if (shape.Hover)
                {
                    pen = hoverpen;
                    brush = backgroundbrush;
                }
                else if (shape.CanOutputLink || shape.CanInputLink)
                {
                    pen = hoverpen;
                    brush = hoverbrush;
                }
                else if (shape.Selected)
                {
                    pen = selectpen;
                    brush = backgroundbrush;
                }
                else
                {
                    pen = defaultpen;
                    brush = backgroundbrush;
                }
                shape.Draw(graphics, pen, brush);

                if (runEmulate)
                {
                    // вывод текущих значений входов, выходов и номеров меток фигур
                    using var defaultbrush = new SolidBrush(pen.Color);
                    shape.DrawWhenDebug(graphics, defaultbrush);
                }
            }
            // рисуем связи фигур из списка
            foreach (var link in links.Where(x => page != null && x.Page == page.Number || x.Page == 0))
            {
                Pen pen;
                using var defaultpen = new Pen(link.Foreground);
                if (runEmulate)
                {
                    if (link.Source != null && link.Source.GetOutputValueKind() == ValueKind.Digital)
                    {
                        if (link.Source.GetOutputValue() > 0)
                            pen = Pens.Lime;
                        else
                            pen = Pens.Red;
                    }
                    else
                        pen = defaultpen;
                }
                else if (link.Hover && link.Selected)
                    pen = selecthoverpen;
                else if (link.Hover)
                    pen = hoverpen;
                else if (link.Selected)
                    pen = selectpen;
                else
                    pen = defaultpen;
                link.DrawLines(graphics, pen);
            }
            if (!dragShapes)
            {
                // рисуем присоединения связей фигур из списка
                foreach (var link in links.Where(x => page != null && x.Page == page.Number || x.Page == 0))
                {
                    Brush brush;
                    using var defaultbrush = new SolidBrush(link.Foreground);
                    if (runEmulate)
                    {
                        if (link.Source != null && link.Source.GetOutputValueKind() == ValueKind.Digital)
                        {
                            if (link.Source.GetOutputValue() > 0)
                                brush = Brushes.Lime;
                            else
                                brush = Brushes.Red;
                        }
                        else
                            brush = defaultbrush;
                    }
                    else if (link.Hover && link.Selected)
                        brush = selecthoverbrush;
                    else if (link.Hover)
                        brush = hoverbrush;
                    else if (link.Selected)
                        brush = selectbrush;
                    else
                        brush = defaultbrush;
                    link.DrawDots(graphics, brush);
                }
            }
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
                            addShapes = !runEmulate;
                            if (addShapes)
                            {
                                var ret = tvLibrary.DoDragDrop(new DragedInfo { Shape = addShape }, DragDropEffects.Copy);
                                if (ret == DragDropEffects.None)
                                {
                                    addShapes = false;
                                    addShape = null;
                                    Cursor = Cursors.Default;
                                }
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
            Cursor = Cursors.WaitCursor;
            Cursor.Show();
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
                UnsubscribeShapeEvents(shape);
            foreach (var shape in shapesForDelete)
            {
                shape.Delete();
                shapes.Remove(shape);
            }
            SortIndexByLocation();
            modelChanged = true;
            drawPanel.Invalidate();
            Cursor = Cursors.Default;
            Cursor.Hide();
        }

        private void Shape_OnDelete(object? sender, EventArgs e)
        {
            if (sender is Shape shapeForDelete)
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
                        SubscribeShapeEvents(shape);
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
            SortIndexByLocation();

            // подключение внешних ссылок
            foreach (var shape in shapes)
            {
                if (shape is IPageSourceLink plink)
                {
                    foreach (var id in plink.GetIdsForConnect())
                    {
                        var target = shapes.FirstOrDefault(shape => shape.Id == id);
                        if (target == null) continue;
                        plink.AddTarget(target);
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
                        SubscribeShapeEvents(shape);
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
                        SubscribeShapeEvents(shape);
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
            List<Shape> newIdShapes = [];
            foreach (var link in pastedLinks)
            {
                link.Selected = true;
                var source = pastedShapes.FirstOrDefault(x => x.Id == link.SourceId);
                if (source != null)
                {
                    source.NewGuid();
                    newIdShapes.Add(source);
                    link.SourceId = source.Id;
                }
                var target = pastedShapes.FirstOrDefault(x => x.Id == link.TargetId);
                if (target != null)
                {
                    target.NewGuid();
                    newIdShapes.Add(target);
                    link.TargetId = target.Id;
                }
            }
            shapes.AddRange(pastedShapes);
            links.AddRange(pastedLinks);

            var p = new Point(
                pastedShapes.Where(x => x.Selected).Select(x => x.Location.X).Min(),
                pastedShapes.Where(x => x.Selected).Select(x => x.Location.Y).Min());
            foreach (var shape in pastedShapes)
            {
                if (!newIdShapes.Contains(shape))
                    shape.NewGuid();
                shape.Location = Point.Add(shape.Location, new Size(point.X - p.X, point.Y - p.Y));
                shape.Location = MovePointToGrid(shape.Location);
            }
            foreach (var link in pastedLinks)
            {
                link.StartPoint = Point.Add(link.StartPoint, new Size(point.X - p.X, point.Y - p.Y));
                link.EndPoint = Point.Add(link.EndPoint, new Size(point.X - p.X, point.Y - p.Y));
                var points = link.GetPoints();
                for (var i = 0; i < points.Length; i++)
                    points[i] = Point.Add(points[i], new Size(point.X - p.X, point.Y - p.Y));
                link.SetPoints(points);

                link.StartPoint = MovePointToGrid(link.StartPoint);
                link.EndPoint = MovePointToGrid(link.EndPoint);
                points = link.GetPoints();
                for (var i = 0; i < points.Length; i++)
                    points[i] = MovePointToGrid(points[i]);
                link.SetPoints(points);
            }
            if (pastedShapes.Count == 1)
                pgProperties.SelectedObject = pastedShapes[0];
        }

        private void TsmiCreate_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            links.Clear();
            // для всех удаляемых элементов
            foreach (var shape in shapes)
                UnsubscribeShapeEvents(shape);
            shapes.Clear();
            modelChanged = false;
            fileName = string.Empty;
            Text = string.IsNullOrEmpty(fileName) ? caption : $"{caption} - {fileName}";
            drawPanel.Reset();
            drawPanel.Invalidate();
            copycuted = null;
            pasted.Item2 = Rectangle.Empty;
            TraceAssistant.InitField(shapes, links);
        }

        private void TimerInterface_Tick(object sender, EventArgs e)
        {
            tsmiSave.Enabled = tsbSave.Enabled = modelChanged;
            tsslStatus.Text = $"Смещение базовой точки: {drawPanel.Origin}, зум: {drawPanel.Zoom}";
            tsmiCut.Enabled = tsmiCopy.Enabled = tsbCut.Enabled = tsbCopy.Enabled = shapes.Any(x => x.Selected);
            tsmiPaste.Enabled = tsbPaste.Enabled = copycuted != null;
            tsbRun.Checked = tsmiRun.Checked = runEmulate;
        }

        private void TsmiCopy_Click(object sender, EventArgs e)
        {
            if (runEmulate) return;
            copycuted = CopySelectedToXml();
        }

        private void TsmiCut_Click(object sender, EventArgs e)
        {
            if (runEmulate) return;
            copycuted = CopySelectedToXml();
            RemoveSelected();
        }

        private void TsmiPaste_Click(object sender, EventArgs e)
        {
            if (runEmulate) return;
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
            if (runEmulate) return;
            UnselectAll();
            ModelAssistant.ShapesFrom(currentPage, shapes).ForEach(x => x.Selected = true);
            ModelAssistant.LinksFrom(currentPage, links).ForEach(x => x.Selected = true);
            drawPanel.Invalidate();
        }

        private int pageToPrint;
        private int pagesToPrint;

        private void TsmiPreview_Click(object sender, EventArgs e)
        {
            pageToPrint = 1;
            pagesToPrint = pages.Count;
            printPreviewDialog1.ShowDialog(this);
        }

        private void PrintPreviewDialog1_Shown(object sender, EventArgs e)
        {
            var screen = Screen.FromRectangle(this.Bounds);
            if (/*Screen.PrimaryScreen?*/screen.WorkingArea is Rectangle rect)
            {
                printPreviewDialog1.SetDesktopLocation(rect.Left, rect.Top);
                printPreviewDialog1.SetDesktopBounds(rect.Left, rect.Top, rect.Width, rect.Height);
            }
        }

        private void PrintDocument1_QueryPageSettings(object sender, System.Drawing.Printing.QueryPageSettingsEventArgs e)
        {
            e.PageSettings.PrinterSettings.PrinterName = "Microsoft Print To PDF";
            e.PageSettings.Landscape = true;
            e.PageSettings.PaperSize = e.PageSettings.PrinterSettings.PaperSizes[5]; // A3 - 5, A4 - 6
        }

        private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (pages.FirstOrDefault(x => x.Number == pageToPrint) is Page page)
            {
                var number = page.Number;
                var width = e.PageSettings.PrintableArea.Width;
                var height = e.PageSettings.PrintableArea.Height;
                var kfY = 5.0 * page.Height / width;
                var dx = 82 - page.Column * height;
                var dy = 20 - page.Row * width;
                e.Graphics?.TranslateTransform(dx, dy);
                e.Graphics?.ScaleTransform((float)(1.0 / kfY), (float)(1.0 / kfY));
                page?.Draw(e.Graphics, Color.Black);
                foreach (var shape in shapes.Where(x => x.Page == number))
                    shape.Draw(e.Graphics, Pens.Black, Brushes.White);
                foreach (var link in links.Where(x => x.Page == number))
                    link.DrawLines(e.Graphics, Pens.Black);
                foreach (var link in links.Where(x => x.Page == number))
                    link.DrawDots(e.Graphics, Brushes.Black);
                e.Graphics?.ScaleTransform((float)kfY, (float)kfY);
                e.Graphics?.TranslateTransform(-dx, -dy);
            }
            e.HasMorePages = pagesToPrint > 1;
            pageToPrint++;
            pagesToPrint--;
        }

        private void TsmiPrint_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                pageToPrint = 1;
                pagesToPrint = pages.Count;
                printDocument1.Print();
            }
        }

        private void TsbRun_Click(object sender, EventArgs e)
        {
            UnselectAll();
            if (!runEmulate && !emulateBackground.IsBusy)
            {
                if (!emulateBackground.IsBusy)
                {
                    emulateBackground.RunWorkerAsync();
                    Cursor.Show();
                }
            }
            else
            {
                emulateBackground.CancelAsync();
                Cursor.Hide();
            }
            drawPanel.Invalidate();
        }

        private void EmulateBackground_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var ms = DateTime.Now.Millisecond / 100;
            while (!worker.CancellationPending)
            {
                var now = DateTime.Now.Millisecond / 100;
                if (now != ms)
                {
                    ms = now;
                    worker.ReportProgress(ms);
                }
            }

        }

        private void EmulateBackground_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            foreach (var shape in shapes)
                shape.Calculate();
            foreach (var shape in shapes)
            {
                if (shape is ICycle cycle)
                    cycle.SwitchOffSubscibers();
            }
            runEmulate = true;
            drawPanel.Invalidate();
        }

        private void EmulateBackground_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            runEmulate = false;
            drawPanel.Invalidate();
        }
    }
}
