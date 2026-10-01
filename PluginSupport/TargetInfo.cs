namespace PluginSupport
{
    public class TargetInfo(bool isOutput, int pinIndex, RectangleF target)
    {
        public bool IsOutput { get; set; } = isOutput;
        public int PinIndex { get; set; } = pinIndex;
        public RectangleF Target { get; set; } = target;
        public PointF PinPoint { get; set; }
    }
}
