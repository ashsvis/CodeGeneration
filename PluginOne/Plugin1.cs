using PluginSupport;

namespace PluginOne
{
    public class Plugin1 : IPlugin
    {
        public string Name => "Plugin 1";

        public void Run(IHost host)
        {
            var control = new UserControl1() { Dock = DockStyle.Fill };
            host.AddControlToMainForm(control);
        }
    }
}
