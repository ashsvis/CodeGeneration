namespace PluginSupport
{
    public interface IPlugin
    {
        string Name { get; }
        void Run(IHost host);
        void ConnectEvents(DrawPanel panel);
    }

}
