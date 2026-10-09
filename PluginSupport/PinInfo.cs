namespace PluginSupport
{
    public class PinInfo(Shape? owner, bool isOutput, int pinIndex, Point? pinPoint)
    {
        public Shape? Owner { get; set; } = owner;
        public bool IsOutput { get; set; } = isOutput;
        public int PinIndex { get; set; } = pinIndex;
        public Point? PinPoint { get; set; } = pinPoint;
        public Guid LinkId { get; set; } = Guid.Empty;
    }
}
