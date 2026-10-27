namespace PluginSupport
{
    public class InvertPinByIndexEventArgs(bool isOutput, int pinIndex, bool state) : EventArgs
    {
        public readonly int PinIndex = pinIndex;
        public bool State = state;
        public readonly bool IsOutput = isOutput;
        
    }

    public delegate void InvertPinByIndexEventHandler(object sender, InvertPinByIndexEventArgs e);
}
