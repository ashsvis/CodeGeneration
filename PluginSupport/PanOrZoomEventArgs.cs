namespace PluginSupport
{
    public class PanOrZoomEventArgs : EventArgs
    {
        public double Zoom { get; set; }
        public PointF ViewPort { get; set; }
    }
}
