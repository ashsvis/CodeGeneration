using PluginSupport;

namespace CodeGenerator
{
    public partial class MainForm : Form, IHost
    {
        private readonly DrawPanel drawPanel;
        private readonly PluginManager pm = new();

        private readonly List<Shape> shapes = [];

        public MainForm()
        {
            InitializeComponent();

            drawPanel = new DrawPanel 
            { 
                Dock = DockStyle.Fill,
                AllowDrop = true,
            };
            drawPanel.DragEnter += DrawPanel_DragEnter;
            drawPanel.DragOver += DrawPanel_DragOver;
            drawPanel.DragDrop += DrawPanel_DragDrop;
            drawPanel.QueryContinueDrag += DrawPanel_QueryContinueDrag;
            drawPanel.MouseDown += DrawPanel_MouseDown;
            drawPanel.MouseMove += DrawPanel_MouseMove;
            drawPanel.MouseUp += DrawPanel_MouseUp;

            drawPanel.OnDraw += DrawPanel_OnDraw;
            drawPanel.OnPanOrZoom += DrawPanel_OnPanOrZoom;
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

        private PointF firstPoint = PointF.Empty;
        private bool leftPressed = false;

        private void DrawPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            firstPoint = e.Location;
            leftPressed = e.Button == MouseButtons.Left;
            if (e.Button == MouseButtons.Right)
                contextMenu.Items.Clear();
            var shapeFound = false;
            var ctrl = ModifierKeys.HasFlag(Keys.Control);
            shapes.ForEach(shape => shape.Hover = false);
            if (!ctrl && shapes.Count(x => x.Selected) == 1)
                shapes.ForEach(shape => shape.Selected = false);
            foreach (var shape in shapes.Select(x => x).Reverse())
            {
                var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                if (shape.TargetsPoint(point))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        shape.Click(point, (a, b, c) => 
                        {
                            var ret = drawPanel.DoDragDrop(new object[] { shape }, DragDropEffects.Link);
                            if (ret == DragDropEffects.None)
                            {
                                Cursor = Cursors.Default;
                            }
                        });

                    }
                    else if (e.Button == MouseButtons.Right)
                        contextMenu.Items.AddRange(shape.GetContextMenuItems(point, shapes.Count(x => x.Selected) > 1));
                    break;
                }
                else if (shape.ContainsPoint(point))
                {
                    var other = shapes.Where(x => x.Selected).Contains(shape);
                    if (!other && shapes.Count(x => x.Selected) > 1 && !ctrl)
                        shapes.ForEach(shape => shape.Selected = false);
                    shape.Selected = true;
                    shape.Hover = true;
                    shapeFound = true;
                    if (e.Button == MouseButtons.Right)
                        contextMenu.Items.AddRange(shape.GetContextMenuItems(point, shapes.Count(x => x.Selected) > 1));
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
            tsslStatus.Text = $"Смещение базовой точки: {drawPanel.Origin}, текущая точка: {e.Location}, зум: {drawPanel.GetLocation(drawPanel.PointToScreen(e.Location))}";
            Cursor = Cursors.Default;
            foreach (var shape in shapes)
                shape.Hover = false;
            foreach (var shape in shapes.Select(x => x).Reverse())
            {
                var point = drawPanel.GetLocation(drawPanel.PointToScreen(e.Location));
                if (shape.TargetsPoint(point))
                    Cursor = Cursors.Hand;
                else if (shape.ContainsPoint(point))
                {
                    shape.Hover = true;
                    break;
                }
            }
            if (leftPressed)
            {
                var dx = e.X - firstPoint.X;
                var dy = e.Y - firstPoint.Y;
                // перемещаем только выбранные фигуры
                foreach (var shape in shapes)
                {
                    if (shape is ILocation loc && loc.Selected)
                        loc.Location = PointF.Add(loc.Location, new SizeF(dx / (float)drawPanel.Zoom, dy / (float)drawPanel.Zoom));
                }
                firstPoint = e.Location;
            }
            drawPanel.Invalidate();
        }

        private void DrawPanel_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (leftPressed)
                {
                    leftPressed = false;
                    drawPanel.Invalidate();
                }
            }
        }

        private void DrawPanel_QueryContinueDrag(object? sender, QueryContinueDragEventArgs e)
        {
            e.Action = e.EscapePressed ? DragAction.Cancel : DragAction.Continue;
        }

        private void DrawPanel_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.GetDataPresent(typeof(object[])))
                    e.Effect = DragDropEffects.Copy;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void DrawPanel_DragOver(object? sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            if (e.Data.GetData(typeof(object[])) != null)
            {
                if (e.AllowedEffect == DragDropEffects.Copy)
                    e.Effect = DragDropEffects.Copy;
                else if (e.AllowedEffect == DragDropEffects.Link)
                {
                    var point = drawPanel.GetLocation(new Point(e.X, e.Y));
                    e.Effect = DragDropEffects.None;
                    foreach (var shape in shapes)
                    {
                        if (shape.TargetsPoint(point))
                        {
                            e.Effect = DragDropEffects.Link;
                            break;
                        }
                    }
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
                if (e.Data.GetData(typeof(object[])) is object[] items)
                {
                    shapes.ForEach(shape => shape.Selected = false);
                    foreach (var shape in items.Cast<Shape>())
                    {
                        shape.Location = drawPanel.GetLocation(new Point(e.X, e.Y));
                        shape.Selected = true;
                        shapes.Add(shape);
                        drawPanel.Invalidate();
                    }
                }
            }
            else if (e.Effect == DragDropEffects.Link)
            {
                if (e.Data.GetData(typeof(object[])) is object[] items)
                {
                    var point = drawPanel.GetLocation(new Point(e.X, e.Y));
                    e.Effect = DragDropEffects.None;
                }
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
                            var ret = tvLibrary.DoDragDrop(new object[] { module }, DragDropEffects.Copy);
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

    }
}
