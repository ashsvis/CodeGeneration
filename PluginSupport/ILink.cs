using System.Xml.Linq;

namespace PluginSupport
{
    public interface ILinked
    {
         event OutputChangedEventHandler? OnOutputChange;
    }

    public class OutputChangedEventArgs(bool newValue) : EventArgs
    {
        public bool NewValue { get; set; } = newValue;

    }

    public delegate void OutputChangedEventHandler(object sender, OutputChangedEventArgs e); 
}
