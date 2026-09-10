namespace CodeGenerator
{
    public partial class MainForm : Form
    {
        private DrawPanel drawPanel;

        public MainForm()
        {
            InitializeComponent();
            drawPanel = new DrawPanel { Dock = DockStyle.Fill, };
            drawPanel.OnDraw += DrawPanel_OnDraw;
            drawPanel.OnPanOrZoom += DrawPanel_OnPanOrZoom;

            Controls.Add(drawPanel);
        }

        private void DrawPanel_OnPanOrZoom(object? sender, DrawPanel.PanOrZoomEventArgs e)
        {

        }

        private void DrawPanel_OnDraw(object? sender, DrawPanel.DrawEventArgs e)
        {
            var rect = new Rectangle(100, 100, 150, 150);
            e.Graphics?.DrawEllipse(Pens.White, rect);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
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

        private void tsmiExit_Click(object sender, EventArgs e)
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
            Properties.Settings.Default.Save();
        }
    }
}
