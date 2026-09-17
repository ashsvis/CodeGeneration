namespace PluginSupport
{
    public interface ILocation
    {
        PointF Location { get; set; }
        bool Selected { get; set; }
    }
}
