using CodeGenerator.Commands;
using PluginSupport;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Xml.Linq;

namespace CodeGenerator
{
    public partial class MainForm : Form, IHost, IUndoRedoSupport
    {
        private bool modelChanged = false;

        private readonly string dragFormat;
        private readonly DrawPanel drawPanel;
        private readonly PluginManager pm = new();

        private static List<Page> Pages => ContextAssistant.ContextPages;
        private static List<Shape> Shapes => ContextAssistant.ContextShapes;
        private static List<PluginSupport.Link> Links => ContextAssistant.ContextLinks;

        private readonly string caption = string.Empty;
        private string fileName = string.Empty;
        private readonly UndoRedoManager UndoRedoManager = new();


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
                    ModelAssistant.AddType(key, dict[key]);
            }
            const int totalPages = 5;
            const int totalPagesInRow = 10;
            ContextAssistant.Context = ContextKind.Model;
            for (var i = 1; i <= totalPages; i++)
            {
                var row = (i - 1) / totalPagesInRow;
                var column = (i - 1) - row * totalPagesInRow;
                Pages.Add(new Page(i, 420, 297, row, column) { PagesCount = totalPages });
            }
            ContextAssistant.Context = ContextKind.Field;
            for (var i = 1; i <= 2; i++)
            {
                var row = (i - 1) / totalPagesInRow;
                var column = (i - 1) - row * totalPagesInRow;
                Pages.Add(new Page(i, 420, 297, row, column) { PagesCount = 2 });
            }
            ContextAssistant.Context = ContextKind.Model;
            UndoRedoManager.OnStateChaned += UndoRedoManager_OnStateChanged;
        }

        private void UndoRedoManager_OnStateChanged(object? sender, EventArgs e)
        {
            drawPanel.Invalidate();
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
        private Point startPoint = Point.Empty;
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

        private XElement? xElementsOnCurrentPageBeforeDrag;

        private void DrawPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            startPoint = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
            var page = Pages.FirstOrDefault(x => x.Bounds.Contains(startPoint));
            if (page != null && page != currentPage || page == null)
            {
                currentPage = page;
                ModelAssistant.UnselectAll();
            }
            if (e.Button == MouseButtons.Left && e.Clicks > 1 && !runEmulate)
            {
                foreach (var shape in ModelAssistant.ShapesFrom(page))
                {
                    if (shape.ContainsPoint(startPoint, 5f / (float)drawPanel.Zoom))
                    {
                        shape.Click(runEmulate, drawPanel.PointToScreen(e.Location));
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
                        UndoRedoManager.Execute(new PasteShapesAndLinksCommand(ContextAssistant.Context, currentPage, 
                            ModelAssistant.CopyPageToXml(page), copycuted, startPoint, SubscribeShapeEvents, UnsubscribeShapeEvents));
                        modelChanged = true;
                    }
                }
                return;
            }

            if (e.Button == MouseButtons.Right && !runEmulate)
                contextMenu.Items.Clear();
            dragShapes = false;
            dragShapesPreview = false;
            var shapeFound = false;
            var ctrl = ModifierKeys.HasFlag(Keys.Control);
            if (!ctrl && Shapes.Count(x => x.Selected) == 1)
                ModelAssistant.UnselectAll();
            firstShape = null;
            firstLinkPoint = null;
            var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
            foreach (var shape in ModelAssistant.ShapesFrom(page))
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
                            contextMenu.Items.AddRange(shape.GetContextMenuItems(point, Shapes.Count(x => x.Selected) > 1));
                            contextMenu.Show(drawPanel, e.Location);
                        }
                        break;
                    }
                    else if (shape.IsInputTargetsPoint(point, out int inputIndex))
                    {
                        var other = ModelAssistant.ShapesFrom(page).Where(x => x.Selected).Contains(shape);
                        if (!other && Shapes.Count(x => x.Selected) > 1 && !ctrl)
                            Shapes.ForEach(shape => shape.Selected = false);
                        shape.Selected = !runEmulate;
                        shape.Hover = !runEmulate;
                        shapeFound = true;
                        if (e.Button == MouseButtons.Left)
                        {
                            if (runEmulate)
                            {
                                if (!shape.IsLinked(inputIndex))
                                {
                                    if (shape.GetInputValueKind(inputIndex) == ValueKind.Digital)
                                        shape.SetInputValue(inputIndex, shape.GetInputValue(inputIndex) > 0 ? 0 : 1);
                                    else if (shape.GetInputValueKind(inputIndex) == ValueKind.Analog)
                                        shape.SetInputValue(inputIndex, shape.GetInputValue(inputIndex) + 1);
                                }
                            }
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (runEmulate)
                            {
                                if (!shape.IsLinked(inputIndex))
                                {
                                    if (shape.GetInputValueKind(inputIndex) == ValueKind.Digital)
                                        shape.SetInputValue(inputIndex, shape.GetInputValue(inputIndex) > 0 ? 0 : 1);
                                    else if (shape.GetInputValueKind(inputIndex) == ValueKind.Analog)
                                        shape.SetInputValue(inputIndex, shape.GetInputValue(inputIndex) - 1);
                                }
                            }
                            else
                            {
                                contextMenu.Items.AddRange(shape.GetContextMenuItems(point, Shapes.Count(x => x.Selected) > 1));
                                contextMenu.Show(drawPanel, e.Location);
                            }
                        }
                        break;
                    }
                }
                else if (shape.ContainsPoint(point, 5f / (float)drawPanel.Zoom))
                {
                    var other = ModelAssistant.ShapesFrom(page).Where(x => x.Selected).Contains(shape);
                    if (!other && ModelAssistant.ShapesFrom(page).Count(x => x.Selected) > 1 && !ctrl)
                        ModelAssistant.UnselectAllOnPage(page);
                    shape.Selected = !runEmulate;
                    shape.Hover = !runEmulate;
                    shapeFound = true;
                    dragShapesPreview = !runEmulate;
                    dragCopiedShapes = ModifierKeys.HasFlag(Keys.Control);
                    if (e.Button == MouseButtons.Right && !runEmulate)
                    {
                        contextMenu.Items.AddRange(shape.GetContextMenuItems(point, Shapes.Count(x => x.Selected) > 1));
                        contextMenu.Show(drawPanel, e.Location);
                    }
                    else if (runEmulate)
                    {
                        shape.Click(runEmulate);
                        dragShapesPreview = false;
                        dragCopiedShapes = false;
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

        private void DrawPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            // передача положения текущего положения курсора
            currentPoint = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));

            Cursor = Cursors.Default;
            var page = Pages.FirstOrDefault(x => x.Bounds.Contains((Point)currentPoint));
            if (page != null && page != currentPage || page == null)
                currentPage = page;
            if (currentPoint is Point point)
                HighLight(page, point);
            if (leftPressed)
            {
                if (!dragShapes && dragShapesPreview)
                {
                    dragShapesPreview = false;

                    pasted = ModelAssistant.PasteFromXml(ModelAssistant.CopySelectedToXml(), currentPoint);
                    movePasted = true;
                    drawPanel.Invalidate();
                    dragCopiedShapes = false;

                    dragShapes = !runEmulate;
                    if (dragShapes)
                    {
                        //firstPoint = ModelAssistant.MovePointToGrid(Shapes.Where(x => x.Selected).First().Location);
                        //xElementsOnCurrentPageBeforeDrag = ModelAssistant.CopyPageToXml(page);
                    }
                }
                if (dragShapes)
                {
                    // начинаем перемещение фигур
                    var ePoint = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                    var dx = ePoint.X - startPoint.X;
                    var dy = ePoint.Y - startPoint.Y;
                    if (dragCopiedShapes)
                    {

                        //var copyed = ModelAssistant.CopySelectedToXml();
                        //var loc = new Point(
                        //    Shapes.Where(x => x.Selected).Select(x => x.Location.X).Min(),
                        //    Shapes.Where(x => x.Selected).Select(x => x.Location.Y).Min());
                        //ModelAssistant.UnselectAll();
                        //ModelAssistant.PasteFromXml(copyed, loc, SubscribeShapeEvents);
                        //dragCopiedShapes = false;
                        //modelChanged = true;
                    }
                    // перемещаем только выбранные фигуры
                    //List<Link> list = [];
                    //for (var n = 0; n < Shapes.Count; n++)
                    //{
                    //    var shape = Shapes[n];
                    //    if (shape.Selected)
                    //    {
                    //        shape.Location = Point.Add(shape.Location, new Size(dx, dy));
                    //        var linksFromSource = Links.Where(x => x.Source == shape);
                    //        if (shape.GetOutputPinPoint(0) is Point spoint)
                    //        {
                    //            foreach (var link in linksFromSource)
                    //            {
                    //                if (!list.Contains(link))
                    //                {
                    //                    link.StartPoint = spoint;
                    //                    list.Add(link);
                    //                }
                    //            }
                    //        }
                    //        for (var i = 0; i < shape.CountInputs(); i++)
                    //        {
                    //            var link = Links.FirstOrDefault(x => x.Target == shape && x.TargetPinIndex == i);
                    //            if (link != null && shape.GetInputPinPoint(i) is Point tpoint)
                    //                link.EndPoint = tpoint;
                    //        }
                    //    }
                    //}
                    //foreach (var link in Links.Where(x => x.Selected))
                    //{
                    //    var pts = link.GetPoints();
                    //    for (var i = 0; i < pts.Length; i++)
                    //    {
                    //        pts[i] = Point.Add(pts[i], new Size(dx, dy));
                    //    }
                    //    link.SetPoints(pts);
                    //}
                    modelChanged = !runEmulate;
                    startPoint = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                }
                else if (linkBuilding)
                {
                    // начинаем тянуть ссылку от выхода функции ко входу функции
                    currentPoint = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                }
                else if (frameBuilding)
                {
                    //ничего не выбрано, рисуем рамку выбора
                    currentPoint = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                    if (startPoint is Point first && currentPoint is Point current)
                    {
                        var minX = Math.Min(first.X, current.X);
                        var minY = Math.Min(first.Y, current.Y);
                        var maxX = Math.Max(first.X, current.X);
                        var maxY = Math.Max(first.Y, current.Y);
                        ribbonRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                        var mode = current.X > first.X;
                        foreach (var shape in ModelAssistant.ShapesFrom(page))
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

        private void DrawPanel_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                var page = Pages.FirstOrDefault(x => x.Bounds.Contains(point));
                if (leftPressed)
                {
                    leftPressed = false;
                    if (dragShapes)
                    {
                        Cursor = Cursors.WaitCursor;
                        Cursor.Show();
                        dragShapesPreview = false;
                        dragShapes = false;

                        (GraphicsPath[] paths, Rectangle r) = pasted;
                        var loc = drawPanel.GetLocation(MousePosition);
                        loc = ModelAssistant.MovePointToGrid(Point.Add(loc, new Size(-r.Left, -r.Top)));
                        var dx = loc.X;
                        var dy = loc.Y;

                        UndoRedoManager.Execute(new MoveShapesAndLinksCommand(ContextAssistant.Context, page, dx, dy,
                            ModelAssistant.ShapesFrom(page).Where(x => x.Selected)));

                        movePasted = false;
                        modelChanged = !runEmulate;
                        drawPanel.Refresh();
                        Cursor = Cursors.Default;
                        Cursor.Hide();
                    }
                    else if (linkBuilding)
                    {
                        // построение связи данных между фигурами
                        linkBuilding = false;
                        foreach (var shape in ModelAssistant.ShapesFrom(page))
                        {
                            if (firstShape is Shape linked &&
                                shape.IsInputTargetsPoint(point, out int index) &&
                                !shape.IsLinked(index) &&
                                firstShape.GetOutputValueKind(0) == shape.GetInputValueKind(index))
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
                                    UndoRedoManager.Execute(new BuildLinkCommand(ContextAssistant.Context, page, firstShape, 0, shape, index));
                                    modelChanged = !runEmulate;
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
                        ModelAssistant.ShapesFrom(page).ForEach(shape => shape.Selected = false);

                        var mode = (currentPoint ?? Point.Empty).X > startPoint.X;
                        foreach (var shape in ModelAssistant.ShapesFrom(page))
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

        private void SelectLinksForBothSelectedShapes(Page? page)
        {
            var selected = ModelAssistant.ShapesFrom(page).Where(x => x.Selected).ToList();
            foreach (var link in ModelAssistant.LinksFrom(page))
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

        private void HighLight(Page? page, Point point)
        {
            ModelAssistant.ShapesFrom(page).ForEach(shape =>
            {
                shape.Hover = false;
                shape.CanInputLink = false;
                shape.CanOutputLink = false;
            });
            ModelAssistant.LinksFrom(page).ForEach(link => link.Hover = false);
            var list = ModelAssistant.ShapesFrom(page);
            foreach (var shape in list)
            {
                if (firstShape == null && shape.IsOutputTargetsPoint(point) || firstShape == shape)
                {
                    if (!runEmulate)
                        Cursor = Cursors.Cross;
                    shape.CanOutputLink = !runEmulate;
                    break;
                }
                else
                if (shape.IsInputTargetsPoint(point, out int index))
                {
                    Cursor = shape.IsLinked(index) ? Cursors.Arrow : linkBuilding ? Cursors.Cross : Cursors.Hand;
                    if (linkBuilding && !shape.IsLinked(index) && firstShape != null)
                        shape.CanInputLink = !runEmulate &&
                            firstShape.Page == shape.Page &&
                            firstShape.GetOutputValueKind(0) == shape.GetInputValueKind(index);
                    else
                        shape.CanInputLink = false;
                    shape.Hover = !runEmulate && !linkBuilding;
                    break;
                }
                else
                if (shape.ContainsPoint(point, 5f / (float)drawPanel.Zoom))
                {
                    shape.Hover = !runEmulate;
                    if (runEmulate && shape is IAcceptClick accept && accept.CanAcceptClick)
                        Cursor = Cursors.Hand;
                    break;
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
                    currentPoint = ModelAssistant.MovePointToGrid(Point.Ceiling(drawPanel.GetLocation(new Point(e.X, e.Y))));
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

                    UnselectAll();
                    var point = ModelAssistant.MovePointToGrid(drawPanel.GetLocation(new Point(e.X, e.Y)));
                    UndoRedoManager.Execute(new AddShapeCommand(ContextAssistant.Context, draged.Shape, point, SubscribeShapeEvents, UnsubscribeShapeEvents));
                    if (draged.Shape is IAcceptClick clickedShape)
                        draged.Shape.Click(runEmulate, PointToScreen(new Point(e.X, e.Y)));
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
                    items.AddRange(Shapes.Where(x => x.GetType() == type));
            }
            e.Results = [.. items];
        }

        private void Shape_OnShowProperties(object? sender, EventArgs e)
        {
            //pgProperties.SelectedObject = sender;
            //tcUtilites.SelectedTab = tpProperties;
        }

        private void Shape_OnMakeCopy(object? sender, EventArgs e)
        {
            if (runEmulate) return;
            if (sender is Shape source)
            {
                Shapes.ForEach(shape => shape.Selected = false);
                var shape = source.DeepClone();
                shape.NewGuid();

                var point = Point.Add(shape.Location, new Size(12, 12));
                UndoRedoManager.Execute(new AddShapeCommand(ContextAssistant.Context, shape, point, SubscribeShapeEvents, UnsubscribeShapeEvents));

                ModelAssistant.SortAndAssignToPages();
                shape.Selected = true;
                modelChanged = true;
                drawPanel.Invalidate();
            }
        }

        private void Shape_OnDeleteLink(object sender, DeleteLinkFromTargetEventArgs e)
        {
            if (runEmulate) return;
            var linksForDelete = Links.Where(x => x.Source == e.Source &&
                x.Target == e.Target && x.TargetPinIndex == e.TargetPinIndex).ToList();
            List<int> pageIndexes = [];
            foreach (var link in linksForDelete)
            {
                // ищем источник и цель
                var source = Shapes.FirstOrDefault(x => x == link.Source && link.SourcePinIndex == 0);
                var target = Shapes.FirstOrDefault(x => x == link.Target && link.TargetPinIndex == e.TargetPinIndex);
                // если найдены оба, то отписывается
                if (source != null && target != null)
                    link.UnlinkToLocation(source, target);
                if (source is IPageTargetLink plink)
                    plink.RemoveTargetFromOutput(target, link.TargetPinIndex);
                // удаляем визуальную ссылку
                Links.Remove(link);
                pageIndexes.Add(link.Page);
                modelChanged = true;
            }
            foreach (var index in pageIndexes.Distinct())
            {
                var page = Pages.FirstOrDefault(x => x.Number == index);
                ModelAssistant.UpdateLinksOnPage(page);
            }
            drawPanel.Invalidate();
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
            foreach (var page in Pages)
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
                    var loc =  drawPanel.GetLocation(MousePosition);
                    loc = ModelAssistant.MovePointToGrid(Point.Add(loc, new Size(-(int)r.Left, -(int)r.Top)));
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
                var mode = (currentPoint ?? Point.Empty).X > startPoint.X;
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
            foreach (var shape in Shapes.Where(x => page != null && x.Page == page.Number || x.Page == 0))
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
            foreach (var link in Links.Where(x => page != null && x.Page == page.Number || x.Page == 0))
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
                foreach (var link in Links.Where(x => page != null && x.Page == page.Number || x.Page == 0))
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
                    if (Shapes.Any(x => x.Selected) &&
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

            ModelAssistant.RemoveSelected(UnsubscribeShapeEvents);

            modelChanged = true;
            drawPanel.Invalidate();
            Cursor = Cursors.Default;
            Cursor.Hide();
        }

        private void Shape_OnDelete(object? sender, EventArgs e)
        {
            if (sender is Shape shape)
            {
                UnselectAll();
                shape.Selected = true;
                RemoveSelected();
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

        private static void UnselectAll()
        {
            ModelAssistant.UnselectAll();
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
                    Cursor = Cursors.WaitCursor;
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
                finally
                {
                    Cursor = Cursors.Default;
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
                    Cursor = Cursors.WaitCursor;
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
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void LoadXml(string filename)
        {
            var xdoc = XDocument.Load(filename);
            var root = xdoc.Element("Document");
            if (root == null) return;
            //var name = root.Attribute("Name")?.Value;

            var context = ContextAssistant.Context;

            ContextAssistant.Context = ContextKind.Model;
            var xmodel = root.Element("Model");
            ReadContextFromXml(xmodel);

            ContextAssistant.Context = ContextKind.Field;
            var xfield = root.Element("Field");
            ReadContextFromXml(xfield);

            ContextAssistant.Context = context;

            var env = ContextAssistant.ContextEnvironment;
            drawPanel.RestoreWheelData(env.Matrix[0], env.Matrix[1], env.Matrix[2], env.Matrix[3], env.Matrix[4], env.Matrix[5], env.Origin, env.Zoom);
            drawPanel.Invalidate();
        }

        private void ReadContextFromXml(XElement? xcontext)
        {
            Shapes.Clear();
            Links.Clear();
            if (xcontext == null) return;
            var xenvironment = xcontext.Element("Environment");
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

                    ContextAssistant.ContextEnvironment = new(m11, m12, m21, m22, dx, dy,
                        new Point(ParseHelper.ParseInteger(xoX, 0), ParseHelper.ParseInteger(xoY, 0)),
                        zoom);
                }
            }
            int n = 0;
            foreach (var xpage in xcontext.Elements("Page"))
            {
                var xnumber = xpage.Attribute("Number")?.Value;
                if (!string.IsNullOrWhiteSpace(xnumber))
                {
                    var number = ParseHelper.ParseInteger(xnumber, 0);
                    var page = Pages.FirstOrDefault(x => x.Number == number);
                    if (page != null)
                    {
                        var xwidth = xpage.Attribute("Width")?.Value;
                        if (!string.IsNullOrWhiteSpace(xwidth))
                            page.Width = ParseHelper.ParseInteger(xwidth, 420);
                        var xheight = xpage.Attribute("Height")?.Value;
                        if (!string.IsNullOrWhiteSpace(xheight))
                            page.Height = ParseHelper.ParseInteger(xheight, 297);
                    }
                }
                n = ModelAssistant.ReadElements(n, xpage, SubscribeShapeEvents);
            }
            var xothers = xcontext.Element("OutsidePages");
            if (xothers != null)
            {
                n = ModelAssistant.ReadElements(n, xothers, SubscribeShapeEvents);
            }
            ModelAssistant.SortAndAssignToPages();
        }

        public void SaveXml(string filename)
        {
            var root = new XElement("Document");
            root.Add(new XAttribute("Name", System.IO.Path.GetFileNameWithoutExtension(filename)));
            var doc = new XDocument(new XComment("Данные чертёжного документа"), root);

            UpdateContextEnvironment();
            var currentContext = ContextAssistant.Context;
            try
            {
                ContextAssistant.Context = ContextKind.Model;
                var xmodel = new XElement("Model");
                SaveContextToXml(root, xmodel, ContextAssistant.ContextEnvironment);
                ContextAssistant.Context = ContextKind.Field;
                var xfield = new XElement("Field");
                SaveContextToXml(root, xfield, ContextAssistant.ContextEnvironment);

                doc.Save(filename);
                modelChanged = false;
            }
            finally
            {
                ContextAssistant.Context = currentContext;
            }
        }

        private static void SaveContextToXml(XElement root, XElement xcontext, KeepEnvironment environment)
        {
            root.Add(xcontext);
            var xenvironment = new XElement("Environment");
            xcontext.Add(xenvironment);

            var env = environment;
            float[] el = env.Matrix;
            var fp = CultureInfo.GetCultureInfo("en-US");
            xenvironment.Add(new XElement("Matrix", string.Join(", ", el.Select(x => x.ToString(fp)))));
            var xorigin = new XElement("Origin");
            xorigin.Add(new XAttribute("X", env.Origin.X));
            xorigin.Add(new XAttribute("Y", env.Origin.Y));
            xenvironment.Add(xorigin);
            xenvironment.Add(new XElement("Zoom", env.Zoom.ToString(fp)));

            foreach (var page in Pages)
            {
                var xpage = page.WriteContent();
                xcontext.Add(xpage);
                foreach (var shape in ModelAssistant.ShapesFrom(page))
                {
                    var xshape = shape.WriteContent();
                    xpage.Add(xshape);
                }
                foreach (var link in ModelAssistant.LinksFrom(page))
                {
                    var xlink = link.WriteContentExtended();
                    xpage.Add(xlink);
                }
            }
            var xothers = new XElement("OutsidePages");
            xcontext.Add(xothers);
            foreach (var shape in ModelAssistant.ShapesFrom(null))
            {
                var xshape = shape.WriteContent();
                xothers.Add(xshape);
            }
            foreach (var link in ModelAssistant.LinksFrom(null))
            {
                var xlink = link.WriteContentExtended();
                xothers.Add(xlink);
            }
        }

        private void TsmiCreate_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            var context = ContextAssistant.Context;

            ContextAssistant.Context = ContextKind.Model;
            Links.Clear();
            // для всех удаляемых элементов
            foreach (var shape in Shapes) UnsubscribeShapeEvents(shape);
            Shapes.Clear();
            ContextAssistant.ContextEnvironment.Reset();

            ContextAssistant.Context = ContextKind.Field;
            Links.Clear();
            // для всех удаляемых элементов
            foreach (var shape in Shapes) UnsubscribeShapeEvents(shape);
            Shapes.Clear();
            ContextAssistant.ContextEnvironment.Reset();

            ContextAssistant.Context = context;
            modelChanged = false;
            fileName = string.Empty;
            Text = string.IsNullOrEmpty(fileName) ? caption : $"{caption} - {fileName}";
            drawPanel.Reset();
            drawPanel.Invalidate();
            copycuted = null;
            pasted.Item2 = Rectangle.Empty;
            TraceAssistant.InitField(Shapes, Links);
            UndoRedoManager.Clear();
        }

        private void TimerInterface_Tick(object sender, EventArgs e)
        {
            tsmiSave.Enabled = tsbSave.Enabled = modelChanged;
            tsslStatus.Text = $"Смещение базовой точки: {drawPanel.Origin}, зум: {drawPanel.Zoom}";
            tsmiCut.Enabled = tsmiCopy.Enabled = tsbCut.Enabled = tsbCopy.Enabled = Shapes.Any(x => x.Selected);
            tsmiPaste.Enabled = tsbPaste.Enabled = copycuted != null;
            tsbRun.Checked = tsmiRun.Checked = runEmulate;
            tsmiModel.Checked = ContextAssistant.Context == ContextKind.Model;
            tsmiField.Checked = ContextAssistant.Context == ContextKind.Field;
            tsmiUndo.Enabled = tsbUndo.Enabled = CanUndo();
            tsmiRedo.Enabled = tsbRedo.Enabled = CanRedo();
        }

        private void TsmiCopy_Click(object sender, EventArgs e)
        {
            if (runEmulate) return;
            copycuted = ModelAssistant.CopySelectedToXml();
        }

        private void TsmiCut_Click(object sender, EventArgs e)
        {
            if (runEmulate) return;
            copycuted = ModelAssistant.CopySelectedToXml();
            RemoveSelected();
        }

        private void TsmiPaste_Click(object sender, EventArgs e)
        {
            if (runEmulate) return;
            if (copycuted != null && !movePasted)
            {
                ModelAssistant.UnselectAll();
                pasted = ModelAssistant.PasteFromXml(copycuted);
                movePasted = true;
                drawPanel.Invalidate();
            }
        }

        private void TsmiSelectAll_Click(object sender, EventArgs e)
        {
            if (runEmulate) return;
            UnselectAll();
            ModelAssistant.SelectAllOnPage(currentPage);
            drawPanel.Invalidate();
        }

        private int pageToPrint;
        private int pagesToPrint;

        private void TsmiPreview_Click(object sender, EventArgs e)
        {
            pageToPrint = 1;
            pagesToPrint = Pages.Count;
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
            if (Pages.FirstOrDefault(x => x.Number == pageToPrint) is Page page)
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
                foreach (var shape in Shapes.Where(x => x.Page == number))
                    shape.Draw(e.Graphics, Pens.Black, Brushes.White);
                foreach (var link in Links.Where(x => x.Page == number))
                    link.DrawLines(e.Graphics, Pens.Black);
                foreach (var link in Links.Where(x => x.Page == number))
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
                pagesToPrint = Pages.Count;
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
            Dictionary<string, double> values = [];
            foreach (var shapes in ContextAssistant.AllShapes.Values)
            {
                foreach (var shape in shapes)
                {
                    shape.Calculate();
                    if (shape is INamedEmulationSource named && !string.IsNullOrEmpty(named.TagName))
                    {
                        if (values.ContainsKey(named.TagName))
                            values[named.TagName] = named.Value;
                        else
                            values.Add(named.TagName, named.Value);
                    }
                }
                foreach (var shape in shapes)
                {
                    if (shape is ICycle cycle)
                        cycle.SwitchOffSubscibers();
                }
            }
            foreach (var shapes in ContextAssistant.AllShapes.Values)
            {
                foreach (var shape in shapes)
                {
                    if (shape is INamedEmulationTarget named && !string.IsNullOrEmpty(named.TagName))
                    {
                        if (values.TryGetValue(named.TagName, out double value))
                            named.Value = value;
                    }
                }
            }

            runEmulate = true;
            drawPanel.Invalidate();
        }

        private void EmulateBackground_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            runEmulate = false;
            drawPanel.Invalidate();
        }

        private void UpdateContextEnvironment()
        {
            if (drawPanel.Transformation != null)
            {
                float[] el = drawPanel.Transformation.Elements;
                ContextAssistant.ContextEnvironment = new KeepEnvironment(el[0], el[1], el[2], el[3], el[4], el[5], drawPanel.Origin, drawPanel.Zoom);
            }
        }

        private void TsmiModel_Click(object sender, EventArgs e)
        {
            if (ContextAssistant.Context != ContextKind.Model)
            {
                UpdateContextEnvironment();
                ContextAssistant.Context = ContextKind.Model;
                var env = ContextAssistant.ContextEnvironment;
                drawPanel.RestoreWheelData(env.Matrix[0], env.Matrix[1], env.Matrix[2], env.Matrix[3], env.Matrix[4], env.Matrix[5], env.Origin, env.Zoom);
                drawPanel.Invalidate();
            }
        }

        private void TsmiField_Click(object sender, EventArgs e)
        {
            if (ContextAssistant.Context != ContextKind.Field)
            {
                UpdateContextEnvironment();
                ContextAssistant.Context = ContextKind.Field;
                var env = ContextAssistant.ContextEnvironment;
                drawPanel.RestoreWheelData(env.Matrix[0], env.Matrix[1], env.Matrix[2], env.Matrix[3], env.Matrix[4], env.Matrix[5], env.Origin, env.Zoom);
                drawPanel.Invalidate();
            }
        }

        public void Undo()
        {
            UndoRedoManager.Undo();
        }

        public void Redo()
        {
            UndoRedoManager.Redo();
        }

        public bool CanUndo()
        {
            return !runEmulate && UndoRedoManager.UndoPossible();
        }

        public bool CanRedo()
        {
            return !runEmulate && UndoRedoManager.RedoPossible();
        }

        private void TsmiUndo_Click(object sender, EventArgs e)
        {
            if (CanUndo()) UndoRedoManager.Undo();
        }

        private void TsmiRedo_Click(object sender, EventArgs e)
        {
            if (CanRedo()) UndoRedoManager.Redo();
        }
    }
}
