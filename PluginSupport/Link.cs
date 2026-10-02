using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public abstract class Link
    {
        public abstract ILocation? Source { get; set; }
        public abstract ILocation? Target { get; set; }
        public abstract int TargetPinIndex { get; set; }
        public abstract Point StartPoint { get; set; }
        public abstract Point EndPoint { get; set; }
        public abstract Point[] GetPoints();
        public abstract void SetPoints(Point[] points);
        public abstract void Rebuild();

        public abstract GraphicsPath[] GetGraphicsPaths();

        public abstract void LinkToLocation(ILocation? source, Point startPoint, 
            ILocation? target, int targetPinIndex, Point endPoint, List<Point> points);
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
