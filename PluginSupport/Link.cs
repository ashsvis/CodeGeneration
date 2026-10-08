using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace PluginSupport
{
    public abstract class Link : IPersistent<Link>, IDeepCloneable<Link>
    {
        private Guid id = Guid.Empty;
        public Guid Id
        {
            get
            {
                if (id == Guid.Empty) id = Guid.NewGuid();
                return id;
            }
        }

        public abstract Shape? Source { get; set; }
        public abstract Guid SourceId { get; set; }
        public abstract int SourcePinIndex { get; set; }
        public abstract int SourceIndex { get; set; }
        public abstract Shape? Target { get; set; }
        public abstract Guid TargetId { get; set; }
        public abstract int TargetIndex { get; set; }
        public abstract int TargetPinIndex { get; set; }
        public abstract Point StartPoint { get; set; }
        public abstract Point EndPoint { get; set; }
        public abstract int Length { get; }
        public abstract Point[] GetPoints();
        public abstract void SetPoints(Point[] points);
        public abstract Rectangle Bounds { get; }
        public Color Foreground { get; set; } = Color.FromArgb(200, 200, 200);
        public bool Selected { get; set; }
        public bool Hover { get; set; }
        public bool IsShort { get; set; }

        public abstract Link DeepClone();

        public abstract GraphicsPath[] GetLinesPaths();
        public abstract GraphicsPath[] GetDotsPaths();

        public abstract void LinkToLocation(Shape source, Point startPoint, 
            Shape target, int targetPinIndex, Point endPoint, List<Point> points);
        public abstract void UnlinkToLocation(Shape source, Shape target);

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

        public abstract XElement WriteContent();
        public abstract void ReadContent(XElement element);
        public abstract bool NoDataToWrite();
    }
}
