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
        public abstract int Length { get; }
        public abstract Point[] GetPoints();
        public abstract void SetPoints(Point[] points);
        public abstract void Rebuild();
        public abstract Rectangle Bounds { get; }
        public Color Foreground { get; set; } = Color.FromArgb(200, 200, 200);
        public bool Selected { get; set; }
        public bool Hover { get; set; }
        public bool IsShort { get; set; }

        public abstract GraphicsPath[] GetLinesPaths();
        public abstract GraphicsPath[] GetDotsPaths();

        public abstract void LinkToLocation(ILocation? source, Point startPoint, 
            ILocation? target, int targetPinIndex, Point endPoint, List<Point> points);
        public abstract void UnlinkToLocation(ILocation? source, ILocation? target);

        public event RebuildLinkFromTargetEventHandler? OnRebuildLink;

        public virtual void DrawLines(Graphics? g, Pen pen)
        {
            foreach (var p in GetLinesPaths())
            {
                using var path = p;
                g?.DrawPath(pen, path);
            }
        }

        public virtual void DrawDots(Graphics? g, Brush brush)
        {
            foreach (var p in GetDotsPaths())
            {
                using var path = p;
                g?.FillPath(brush, path);
            }
        }

        public void RebuildLinkFromTarget(Link link)
        {
            OnRebuildLink?.Invoke(this, new RebuildLinkFromTargetEventArgs(link));
        }
    }

    public class RebuildLinkFromTargetEventArgs(PluginSupport.Link link) : EventArgs
    {
        public PluginSupport.Link Link { get; set; } = link;
    }

    public delegate void RebuildLinkFromTargetEventHandler(object sender, RebuildLinkFromTargetEventArgs e);
}
