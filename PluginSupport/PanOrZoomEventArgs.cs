namespace PluginSupport
{
    public class PanOrZoomEventArgs : EventArgs
    {
        public double Zoom { get; set; }
        public PointF Origin { get; set; }
    }
}
