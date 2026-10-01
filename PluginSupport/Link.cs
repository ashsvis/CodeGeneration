using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public abstract class Link
    {
        public abstract ILocation? Source { get; set; }
        public abstract ILocation? Target { get; set; }
        public abstract PointF StartPoint { get; set; }
        public abstract PointF EndPoint { get; set; }
        public abstract PointF[] GetPoints();
        public abstract void SetPoints(PointF[] points);
        public abstract void Update();

        public abstract GraphicsPath[] GetGraphicsPaths();

        public abstract void LinkToLocation(ILocation? source, PointF startPoint, ILocation? target, PointF endPoint, List<PointF> points);
        public abstract void UnlinkToLocation(ILocation? source, ILocation? target);

        public event RebuildLinkFromTargetEventHandler? OnRebuildLink;

        public virtual void Draw(Graphics? g, Pen pen)
        {
            foreach (var p in GetGraphicsPaths())
            {
                using var path = p;
                g?.DrawPath(pen, path);
            }
        }

        public void RebuildLinkFromTarget(Link link)
        {
            OnRebuildLink?.Invoke(this, new RebuildLinkFromTargetEventArgs(link));
        }
    }

    public class RebuildLinkFromTargetEventArgs(Link link) : EventArgs
    {
        public Link Link { get; set; } = link;
    }

    public delegate void RebuildLinkFromTargetEventHandler(object sender, RebuildLinkFromTargetEventArgs e);
}
