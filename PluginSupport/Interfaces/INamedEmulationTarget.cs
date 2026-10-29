namespace PluginSupport
{
    public interface INamedEmulationTarget
    {
        string? TagName { get; }
        double Value { set; }
    }
}
