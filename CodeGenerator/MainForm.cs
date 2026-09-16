using PluginSupport;

namespace CodeGenerator
{
    public partial class MainForm : Form, IHost
    {
        private readonly DrawPanel drawPanel;
        private readonly PluginManager pm = new();

        public MainForm()
        {
            InitializeComponent();

            drawPanel = new DrawPanel { Dock = DockStyle.Fill, };
            drawPanel.OnDraw += DrawPanel_OnDraw;
            drawPanel.OnPanOrZoom += DrawPanel_OnPanOrZoom;

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
            panCenter.Controls.Add(drawPanel);
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
            var rect = new Rect() { Location = new PointF(80f, 110f), Width = 100f, Height = 80f };
            rect.Draw(e.Graphics);
            var circle = new Circle() { Location = new PointF(100f, 100f), Radius = 50f, Background = Color.FromArgb(200, SystemColors.Window) };
            circle.Draw(e.Graphics);
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
        public float Radius { get; set; }

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
        public float Width { get; set; }
        public float Height { get; set; }

        public override void Draw(Graphics? g)
        {
            // пример круга
            var rect = new RectangleF(Location.X - Width / 2f, Location.Y - Height - 2f, Width, Height);
            using var brush = new SolidBrush(Background);
            g?.FillRectangle(brush, rect);
            using var pen = new Pen(Foreground, 1);
            g?.DrawRectangle(pen, rect);
        }
    }
}
