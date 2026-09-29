namespace PluginSupport
{
    public interface ILink
    {
        event OutputChangedEventHandler? OnOutputChange;
    }

    public class OutputChangedEventArgs(bool newValue) : EventArgs
    {
        public bool NewValue { get; set; } = newValue;

    }

    public delegate void OutputChangedEventHandler(object sender, OutputChangedEventArgs e); 
}
