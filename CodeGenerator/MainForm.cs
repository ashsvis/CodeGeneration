using PluginSupport;

namespace CodeGenerator
{
    public partial class MainForm : Form, IHost
    {
        private readonly string dragFormat;
        private readonly DrawPanel drawPanel;
        private readonly PluginManager pm = new();

        private readonly List<Shape> shapes = [];
        private readonly List<Link> links = [];

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
            //drawPanel.MouseLeave += (o, e) =>
            //{
            //    currentPoint = null;
            //    drawPanel.Invalidate();
            //    Cursor.Show();
            //};

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

        private Point? firstCurrentPoint = null;
        private PointF? firstLinkPoint = null;
        private Point? currentPoint = null;
        private PointF firstPoint = PointF.Empty;
        private Shape? firstShape = null;
        private bool linkBuilding = false;
        private bool leftPressed = false;
        private bool dragShapes = false;

        private void DrawPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            firstPoint = e.Location;
            firstCurrentPoint = MovePointToGrid(Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location))));
            leftPressed = e.Button == MouseButtons.Left;
            if (e.Button == MouseButtons.Right)
                contextMenu.Items.Clear();
            dragShapes = false;
            var shapeFound = false;
            var ctrl = ModifierKeys.HasFlag(Keys.Control);
            shapes.ForEach(shape => shape.Hover = false);
            if (!ctrl && shapes.Count(x => x.Selected) == 1)
                shapes.ForEach(shape => shape.Selected = false);
            firstShape = null;
            firstLinkPoint = null;
            foreach (var shape in shapes.Select(x => x).Reverse())
            {
                var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                if (shape.IsPointInTargets(point))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        shape.Click(point, (isOutput, pinIndex, pinRect) =>
                        {
                            firstShape = shape;
                            linkBuilding = true;
                            firstLinkPoint = new PointF(pinRect.Right, pinRect.Location.Y + pinRect.Height / 2f);
                        });
                    }
                    else if (e.Button == MouseButtons.Right)
                        contextMenu.Items.AddRange(shape.GetContextMenuItems(point, shapes.Count(x => x.Selected) > 1));
                    break;
                }
                else if (shape.ContainsPoint(point, 5f / (float)drawPanel.Zoom))
                {
                    var other = shapes.Where(x => x.Selected).Contains(shape);
                    if (!other && shapes.Count(x => x.Selected) > 1 && !ctrl)
                        shapes.ForEach(shape => shape.Selected = false);
                    shape.Selected = true;
                    shape.Hover = true;
                    shapeFound = true;
                    if (e.Button == MouseButtons.Right)
                        contextMenu.Items.AddRange(shape.GetContextMenuItems(point, shapes.Count(x => x.Selected) > 1));
                    else
                        dragShapes = true;
                    break;
                }
            }
            if (!shapeFound)
            {
                shapes.ForEach(shape =>
                {
                    shape.Selected = false;
                    shape.Hover = false;
                });
            }
            drawPanel.Invalidate();
            if (e.Button == MouseButtons.Right)
                contextMenu.Show(drawPanel, e.Location);
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
                    var dx = e.X - firstPoint.X;
                    var dy = e.Y - firstPoint.Y;
                    // перемещаем только выбранные фигуры
                    foreach (var shape in shapes)
                    {
                        if (shape is ILocation item && item.Selected)
                            item.Location = PointF.Add(item.Location, new SizeF(dx / (float)drawPanel.Zoom, dy / (float)drawPanel.Zoom));
                    }
                    firstPoint = e.Location;
                }
                else if (linkBuilding)
                {
                    currentPoint = Point.Ceiling(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location)));
                }
            }
            drawPanel.Invalidate();
        }

        private const int Step = 12; // Размер клетки

        private static Point MovePointToGrid(Point point)
        {
            return new Point((int)Math.Round((decimal)point.X / Step) * Step, (int)Math.Round((decimal)point.Y / Step) * Step);
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
                                item.Location = MovePointToGrid(Point.Ceiling(item.Location));
                        }
                        SortIndexByLocation();
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
                                shape.SetInputValue(index, firstShape.GetOutputValue(0));
                                shape.LinkInput(link, index);
                                break;
                            }
                        }
                        firstShape = null;
                    }
                    drawPanel.Invalidate();
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
                    shapes.ForEach(shape => shape.Selected = false);
                    var shape = draged.Shape;
                    currentPoint = MovePointToGrid(Point.Ceiling(drawPanel.GetLocation(new Point(e.X, e.Y))));
                    shape.Location = (Point)currentPoint;
                    shape.Selected = true;
                    shapes.Add(shape);
                    SortIndexByLocation();
                    drawPanel.Invalidate();
                }
            }
        }

        private void SortIndexByLocation()
        {
            var n = 0;
            foreach (var item in shapes.OrderBy(x => x.Location.X).ThenBy(x => x.Location.Y))
            {
                item.Index = n;
                n++;
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
            // рисуем связи фигур из списка
            foreach (var link in links)
            {
                link.Draw(e.Graphics, Pens.White);
            }
            // рисуем фигуры из списка
            using var hoverpen = new Pen(Color.FromArgb(255, 255, 255), 1f);
            using var selectpen = new Pen(Color.DarkMagenta, 1f);
            using var selecthoverpen = new Pen(Color.Magenta, 1f);
            foreach (var shape in shapes)
            {
                if (shape.Hover && shape.Selected)
                {
                    using var brush = new SolidBrush(shape.Background);
                    shape.Draw(e.Graphics, selecthoverpen, brush);
                }
                else if (shape.Hover)
                {
                    using var brush = new SolidBrush(shape.Background);
                    shape.Draw(e.Graphics, hoverpen, brush);
                }
                else if (shape.CanOutputLink || shape.CanInputLink)
                {
                    using var brush = new SolidBrush(Color.Teal);
                    shape.Draw(e.Graphics, hoverpen, brush);
                }
                else if (shape.Selected)
                {
                    using var brush = new SolidBrush(shape.Background);
                    shape.Draw(e.Graphics, selectpen, brush);
                }
                else
                {
                    using var brush = new SolidBrush(shape.Background);
                    using var pen = new Pen(shape.Foreground, 1);
                    shape.Draw(e.Graphics, pen, brush);
                }
            }
            // рисование курсора при свободном движении указателя мыши
            //if (MouseButtons.HasFlag(MouseButtons.None) && currentPoint is Point point)
            //{
            //    var cursize = (float)(50f / drawPanel.Zoom);
            //    using var cursorpen = new Pen(Color.FromArgb(255, 255, 255), 0f);
            //    e.Graphics?.DrawLine(cursorpen, PointF.Add(point, new SizeF(-cursize, 0)), PointF.Add(point, new SizeF(cursize, 0)));
            //    e.Graphics?.DrawLine(cursorpen, PointF.Add(point, new SizeF(0, -cursize)), PointF.Add(point, new SizeF(0, cursize)));
            //}

            // рисуем резиновую связь в момент построения связи
            if (linkBuilding && firstLinkPoint is PointF point1 && currentPoint is Point point2)
            {
                using var linkpen = new Pen(Color.Teal, 1f);
                linkpen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                linkpen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                e.Graphics?.DrawLine(linkpen, point1, point2);
            }
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
                            var forDelete = shapes.Where(x => x.Selected).ToList();
                            foreach (var shape in forDelete)
                            {
                                shape.UnlinkAllInputs();
                                foreach (var item in shapes)
                                {
                                    if (shape is ILink link)
                                        item.UnlinkOutputFor(link);
                                }
                            }
                            foreach (var shape in forDelete)
                                shapes.Remove(shape);
                            SortIndexByLocation();
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

    public class DragedInfo
    {
        public required Shape Shape { get; set; }
        public HitInfo HitInfo { get; set; }
    }

    public struct HitInfo
    {
        public ShapeHits Hits;
        public uint PinIndex;
        public int LinkIndex;
        public PointF PinPoint;
    }

    public enum ShapeHits
    {
        None,
        Body,
        Caption,
        OrderNum,
        InputLink,
        OutputLink,
        Descriptor,
        LeftEdge
    }
}
