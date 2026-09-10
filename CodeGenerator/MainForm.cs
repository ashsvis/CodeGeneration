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
                Properties.Settings.Default.tk0, 
                Properties.Settings.Default.tk1,
                Properties.Settings.Default.tk2,
                Properties.Settings.Default.tk3,
                Properties.Settings.Default.tk4,
                Properties.Settings.Default.tk5);
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (drawPanel.Transformation == null) return;
            var el = drawPanel.Transformation.Elements;
            Properties.Settings.Default.tk0 = el[0];
            Properties.Settings.Default.tk1 = el[1];
            Properties.Settings.Default.tk2 = el[2];
            Properties.Settings.Default.tk3 = el[3];
            Properties.Settings.Default.tk4 = el[4];
            Properties.Settings.Default.tk5 = el[5];
            Properties.Settings.Default.Save();
        }
    }
}
