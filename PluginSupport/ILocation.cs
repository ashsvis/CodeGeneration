namespace PluginSupport
{
    public interface ILocation
    {
        PointF Location { get; set; }
        bool Selected { get; set; }
        event LocationChangedEventHandler? OnLocationChange;
    }

    public class LocationChangedEventArgs(PointF newValue) : EventArgs
    {
        public PointF NewValue { get; set; } = newValue;

    }

    public delegate void LocationChangedEventHandler(object sender, LocationChangedEventArgs e);
}
