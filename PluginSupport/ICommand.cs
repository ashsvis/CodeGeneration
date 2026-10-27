namespace PluginSupport
{
    public interface ICommand
    {
        string Name { get; }
        void Execute();
        void UnExecute();
    }
}
