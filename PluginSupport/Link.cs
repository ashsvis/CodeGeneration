using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public abstract class Link
    {
        public abstract ILocation? Source { get; set; }
        public abstract ILocation? Target { get; set; }
        public abstract PointF StartPoint { get; set; }
        public abstract PointF EndPoint { get; set; }

        public abstract GraphicsPath[] GetGraphicsPaths();

        public abstract void LinkToLocation(ILocation? source, PointF startPoint, ILocation? target, PointF endPoint, List<PointF> points);
        public abstract void UnlinkToLocation(ILocation? source, ILocation? target);

        public virtual void Draw(Graphics? g, Pen pen)
        {
            foreach (var p in GetGraphicsPaths())
            {
                using var path = p;
                g?.DrawPath(pen, path);
            }
        }
    }
}
