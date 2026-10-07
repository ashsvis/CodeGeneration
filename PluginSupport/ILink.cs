namespace PluginSupport
{
    public interface ILinked
    {
        event OutputChangedEventHandler? OnOutputChange;
    }

    public class OutputChangedEventArgs(double newValue) : EventArgs
    {
        public double NewValue { get; set; } = newValue;

    }

    public delegate void OutputChangedEventHandler(object sender, OutputChangedEventArgs e); 
}
