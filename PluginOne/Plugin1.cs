using PluginSupport;

namespace PluginOne
{
    public class Plugin1 : IPlugin
    {
        public string Name => "Plugin 1";

        public void Run(IHost host)
        {
            var control = new UserControl1();
            host.AddControlToMainForm(control);
        }
    }
}
