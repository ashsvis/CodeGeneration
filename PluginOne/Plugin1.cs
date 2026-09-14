using PluginSupport;

namespace PluginOne
{
    public class Plugin1 : IPlugin
    {
        public string Name => "Plugin 1";

        private void OnDraw(object? sender, DrawEventArgs e)
        {
            // пример круга
            var rect = new Rectangle(110, 110, 150, 150);
            e.Graphics?.DrawEllipse(Pens.Yellow, rect);
        }

        public void Run(IHost host)
        {
            var control = new UserControl1() { Dock = DockStyle.Fill };
            host.AddControlToMainForm(control);
        }

        public void ConnectEvents(DrawPanel panel)
        {
            panel.OnDraw += OnDraw;
        }
    }
}
