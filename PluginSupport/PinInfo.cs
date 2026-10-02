namespace PluginSupport
{
    public class PinInfo(bool isOutput, int pinIndex, Point pinPoint)
    {
        public bool IsOutput { get; set; } = isOutput;
        public int PinIndex { get; set; } = pinIndex;
        public Point PinPoint { get; set; } = pinPoint;
    }
}
