namespace PluginSupport
{
    public interface INamedEmulationSource
    {
        string? TagName { get; }
        double Value { get; }
    }
}
