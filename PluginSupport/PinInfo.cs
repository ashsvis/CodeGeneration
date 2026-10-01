namespace PluginSupport
{
    public class PinInfo(bool isOutput, int pinIndex, PointF pinPoint)
    {
        public bool IsOutput { get; set; } = isOutput;
        public int PinIndex { get; set; } = pinIndex;
        public PointF PinPoint { get; set; } = pinPoint;
    }
}
