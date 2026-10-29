namespace PluginSupport
{
    public class PropertyChangedEventArgs(string? propName, object? oldValue, object? newValue) : EventArgs
    {
        public string? PropName { get; } = propName;
        public object? OldValue { get; } = oldValue;
        public object? NewValue { get; } = newValue;
    }

    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
}
