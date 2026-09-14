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

            //сканируем плагины в папке Plugins
            pm.ScanPlugins(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"));

            //перебираем плагины, создаем пункт меню для каждого
            foreach (var plugin in pm.Plugins)
            {
                plugin.ConnectEvents(drawPanel);
                var item = tsmiPlugins.DropDownItems.Add(plugin.Name);
                //item.Click += delegate { plugin.Run(this); }; // при клике на меню, запускаем плагин на выполнение
            }

            panCenter.Controls.Add(drawPanel);
        }

        public void AddControlToMainForm(Control control)
        {
            panCenter.Controls.Clear();
            panCenter.Controls.Add(control);
        }

        private void DrawPanel_OnPanOrZoom(object? sender, PanOrZoomEventArgs e)
        {

        }

        private void DrawPanel_OnDraw(object? sender, DrawEventArgs e)
        {
            // пример круга
            var rect = new Rectangle(100, 100, 150, 150);
            e.Graphics?.DrawEllipse(Pens.White, rect);
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
}
