namespace PluginSupport
{
    public class DeleteLinkFromTargetEventArgs(Shape? source, Shape? target, int targetPinIndex) : EventArgs
    {
        public Shape? Source { get; set; } = source;
        public Shape? Target { get; set; } = target;
        public int TargetPinIndex { get; set; } = targetPinIndex;
    }

    public delegate void DeleteLinkFromTargetEventHandler(object sender, DeleteLinkFromTargetEventArgs e);
}
