namespace PluginSupport
{
    public class DrawEventArgs : EventArgs
    {
        public Graphics? Graphics { get; set; }
        public double Zoom { get; set; }
        public PointF ViewPort { get; set; }
    }
}
