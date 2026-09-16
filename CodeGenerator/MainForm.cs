using PluginSupport;
using System.Drawing.Drawing2D;

namespace CodeGenerator
{
    public partial class MainForm : Form, IHost
    {
        private readonly DrawPanel drawPanel;
        private readonly PluginManager pm = new();

        private readonly List<Figure> figures = [];

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
        private Figure? pressedFigure = null;

        private void DrawPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                firstPoint = e.Location;
                leftPressed = true;
                pressedFigure = null;
                // выбор или невыбор фигур мышью путём нажатия на фигуру
                var modify = false;
                foreach (var figure in figures.Select(x => x).Reverse())
                {
                    if (figure.ContainsPoint(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location))))
                    {
                        if (!ModifierKeys.HasFlag(Keys.Control))
                        {
                            pressedFigure = figure;
                            if (!figure.Selected)
                            {
                                figure.Selected = true;
                                modify = true;
                            }
                        }
                        else if (ModifierKeys.HasFlag(Keys.Control) && figure.Selected)
                        {
                            figure.Selected = false;
                            modify = true;
                        }
                        drawPanel.Invalidate();
                        break;
                    }
                }
                // если выбор был сделан, то выходим
                if (modify) return;
            }
        }

        private void DrawPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            tsslStatus.Text = $"Смещение базовой точки: {drawPanel.Origin}, текущая точка: {e.Location}, зум: {drawPanel.GetLocation(drawPanel.PointToScreen(e.Location))}";
            foreach (var figure in figures)
                figure.Hover = false;
            foreach (var figure in figures.Select(x => x).Reverse())
            {
                if (figure.ContainsPoint(drawPanel.GetLocation(drawPanel.PointToScreen(e.Location))))
                {
                    figure.Hover = true;
                    break;
                }
            }
            if (leftPressed)
            {
                var dx = e.X - firstPoint.X;
                var dy = e.Y - firstPoint.Y;

                if (pressedFigure != null)
                {
                    pressedFigure.Location = PointF.Add(pressedFigure.Location, new SizeF(dx, dy));
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
                    foreach (var figure in items.Cast<Figure>())
                    {
                        figure.Location = drawPanel.GetLocation(new Point(e.X, e.Y));
                        figures.Add(figure);
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
            foreach (var figure in figures)
            {
                if (figure.Hover && figure.Selected)
                {
                    using var brush = new SolidBrush(figure.Background);
                    figure.Draw(e.Graphics, selecthoverpen, brush);
                }
                else if (figure.Hover)
                {
                    using var brush = new SolidBrush(figure.Background);
                    figure.Draw(e.Graphics, hoverpen, brush);
                }
                else if (figure.Selected)
                {
                    using var brush = new SolidBrush(figure.Background);
                    figure.Draw(e.Graphics, selectpen, brush);
                }
                else
                    figure.Draw(e.Graphics);
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
                        var module = (Figure?)Activator.CreateInstance(type);
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

    public abstract class Figure
    {
        public PointF Location { get; set; }
        public Color Foreground { get; set; } = Color.FromArgb(200, 200, 200);
        public Color Background { get; set; } = Color.FromArgb(50, 50, 50);

        public bool Selected { get; set; }
        public bool Hover { get; set; }

        public abstract GraphicsPath GetGraphicsPath();

        public virtual void Draw(Graphics? g)
        {
            using var path = GetGraphicsPath();
            using var brush = new SolidBrush(Background);
            g?.FillPath(brush, path);
            using var pen = new Pen(Foreground, 1);
            g?.DrawPath(pen, path);
        }

        public virtual void Draw(Graphics? g, Pen pen, Brush brush)
        {
            using var path = GetGraphicsPath();
            g?.FillPath(brush, path);
            g?.DrawPath(pen, path);
        }

        public bool ContainsPoint(PointF point)
        {
            using var pen = new Pen(Foreground, 1);
            using var path = GetGraphicsPath();
            return path.IsOutlineVisible(point, pen) || path.IsVisible(point);
        }
    }

    public class Circle: Figure
    {
        public float Radius { get; set; } = 50f;

        public override GraphicsPath GetGraphicsPath()
        {
            var rect = new RectangleF(Location.X - Radius, Location.Y - Radius, Radius * 2f, Radius * 2f);
            var path = new GraphicsPath();
            path.AddEllipse(rect);
            return path;
        }

        //public override void Draw(Graphics? g)
        //{
        //    if (g == null) return;
        //    base.Draw(g);
        //    var rect = new RectangleF(Location.X - Radius, Location.Y - Radius, Radius * 2f, Radius * 2f);
        //    using var sf = new StringFormat();
        //    sf.Alignment = StringAlignment.Center;
        //    sf.LineAlignment = StringAlignment.Center;
        //    g.DrawString(Location.ToString(), SystemFonts.DefaultFont, SystemBrushes.ControlText, rect, sf);
        //}
    }

    public class Rect : Figure
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
