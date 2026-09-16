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
                        figure.Location = PrepareMousePosition(drawPanel.PointToClient(new Point(e.X, e.Y)));
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
            foreach (var figure in figures)
            {
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

        private void tvLibrary_MouseDown(object sender, MouseEventArgs e)
        {
            var node = tvLibrary.GetNodeAt(e.X, e.Y);
            if (node != null && e.Button == MouseButtons.Left)
            {
                tvLibrary.SelectedNode = null;
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

        private PointF PrepareMousePosition(PointF point)
        {
            PointF[] arr = [point];
            Matrix matrix = new();

            var zoom = (float)drawPanel.Zoom;
            var origin = drawPanel.Origin;

            matrix.Translate(origin.X, origin.Y);
            matrix.Scale(1 / zoom, 1 / zoom);
            matrix.TransformPoints(arr);
            matrix.Dispose();
            return new PointF(arr[0].X, arr[0].Y);
        }

    }

    public abstract class Figure
    {
        public PointF Location { get; set; }
        public Color Foreground { get; set; } = SystemColors.ControlText;
        public Color Background { get; set; } = SystemColors.Window;
        public abstract void Draw(Graphics? g);
    }

    public class Circle: Figure
    {
        public float Radius { get; set; } = 50f;

        public override void Draw(Graphics? g)
        {
            // пример круга
            var rect = new RectangleF(Location.X - Radius, Location.Y - Radius, Radius * 2f, Radius * 2f);
            using var brush = new SolidBrush(Background);
            g?.FillEllipse(brush, rect);
            using var pen = new Pen(Foreground, 1);
            g?.DrawEllipse(pen, rect);
        }
    }

    public class Rect : Figure
    {
        public float Width { get; set; } = 100f;
        public float Height { get; set; } = 80f;

        public override void Draw(Graphics? g)
        {
            // пример круга
            var rect = new RectangleF(Location.X - Width / 2f, Location.Y - Height / 2f, Width, Height);
            using var brush = new SolidBrush(Background);
            g?.FillRectangle(brush, rect);
            using var pen = new Pen(Foreground, 1);
            g?.DrawRectangle(pen, rect);
        }
    }
}
