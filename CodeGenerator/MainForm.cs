using PluginSupport;
using System.Drawing.Drawing2D;

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

            /*
            //сканируем плагины в папке Plugins
            pm.ScanPlugins(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"));

            //перебираем плагины, создаем пункт меню для каждого
            foreach (var plugin in pm.Plugins)
            {
                plugin.ConnectEvents(drawPanel);
                var item = tsmiPlugins.DropDownItems.Add(plugin.Name);
                //item.Click += delegate { plugin.Run(this); }; // при клике на меню, запускаем плагин на выполнение
            }
            */

            tvLibrary.Nodes.Clear();
            tvLibrary.Nodes.Add(new TreeNode("Круг") { Tag = typeof(Circle) });
            tvLibrary.Nodes.Add(new TreeNode("Прямоугольник") { Tag = typeof(Rect) });
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
                if (shape.ContainsPoint(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location))))
                {
                    var other = shapes.Where(x => x.Selected).Contains(shape);
                    if (!other && shapes.Count(x => x.Selected) > 1 && !ctrl)
                        shapes.ForEach(shape => shape.Selected = false);
                    shape.Selected = true;
                    shape.Hover = true;
                    shapeFound = true;
                    if (e.Button == MouseButtons.Right)
                        contextMenu.Items.AddRange(shape.GetContextMenuItems(shapes.Count(x => x.Selected) > 1));
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
            foreach (var shape in shapes)
                shape.Hover = false;
            foreach (var shape in shapes.Select(x => x).Reverse())
            {
                if (shape.ContainsPoint(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location))))
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
                    shape.Draw(e.Graphics);
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
            tvLibrary.SelectedNode = tvLibrary.Nodes[0];
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

    public class Circle: Shape
    {
        public float Radius { get; set; } = 50f;

        public override GraphicsPath GetGraphicsPath()
        {
            var rect = new RectangleF(Location.X - Radius, Location.Y - Radius, Radius * 2f, Radius * 2f);
            var path = new GraphicsPath();
            path.AddEllipse(rect);
            return path;
        }

        public override ToolStripItem[] GetContextMenuItems(bool several)
        {
            List<ToolStripItem> items = [];
            var item = new ToolStripMenuItem() { Text = "Круг", Enabled = false };
            if (!several)
                items.Add(item);
            var baseItems = base.GetContextMenuItems(several);
            if (!several && baseItems.Length > 0)
                items.Add((ToolStripItem)new ToolStripSeparator());
            items.AddRange(baseItems);
            return [.. items];
        }
    }

    public class Rect : Shape
    {
        public float Width { get; set; } = 100f;
        public float Height { get; set; } = 80f;

        public override GraphicsPath GetGraphicsPath()
        {
            var rect = new RectangleF(Location.X - Width / 2f, Location.Y - Height / 2f, Width, Height);
            var path = new GraphicsPath();
            path.AddRectangle(rect);
            return path;
        }

        public override ToolStripItem[] GetContextMenuItems(bool several)
        {
            List<ToolStripItem> items = [];
            var item = new ToolStripMenuItem() { Text = "Прямоугольник", Enabled = false };
            if (!several)
                items.Add(item);
            var baseItems = base.GetContextMenuItems(several);
            if (!several && baseItems.Length > 0)
                items.Add((ToolStripItem)new ToolStripSeparator());
            items.AddRange(baseItems);
            return [.. items];
        }

        //public override void Draw(Graphics? g)
        //{
        //    if (g == null) return;
        //    base.Draw(g);
        //    var rect = new RectangleF(Location.X - Width / 2f, Location.Y - Height / 2f, Width, Height);
        //    using var sf = new StringFormat();
        //    sf.Alignment = StringAlignment.Center;
        //    sf.LineAlignment = StringAlignment.Center;
        //    g.DrawString(Location.ToString(), SystemFonts.DefaultFont, SystemBrushes.ControlText, rect, sf);
        //}
    }
}
