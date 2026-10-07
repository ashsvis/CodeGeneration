namespace PluginSupport
{
    public interface ILocation
    {
        Point Location { get; set; }
        bool Selected { get; set; }
        int Index { get; set; }
        void LinkInput(ILinked? link, int index);

        event LocationChangedEventHandler? OnLocationChange;
    }

    public class LocationChangedEventArgs(Point newValue) : EventArgs
    {
        public Point NewValue { get; set; } = newValue;

    }

    public delegate void LocationChangedEventHandler(object sender, LocationChangedEventArgs e);
}
