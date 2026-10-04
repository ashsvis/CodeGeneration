using PluginSupport;
using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class FuncLink : PluginSupport.Link
    {
        public override ILocation? Source { get; set; }
        public override ILocation? Target { get; set; }
        public override int TargetPinIndex { get; set; }
        public override Point StartPoint { get; set; }
        public override Point EndPoint { get; set; }
        public override int Length => Math.Abs(EndPoint.X - StartPoint.X) + Math.Abs(EndPoint.Y - StartPoint.Y);
        
        public override Rectangle Bounds
        {
            get
            {
                var minX = linkPoints.Min(x => x.X);
                var maxX = linkPoints.Max(x => x.X);
                var minY = linkPoints.Min(y => y.Y);
                var maxY = linkPoints.Max(y => y.Y);
                return new Rectangle(minX, minY, maxX - minX, maxY - minY);
            }
        }

        private Size StartShift { get; set; }
        private Size EndShift { get; set; }

        private Point[] linkPoints = [];
        private bool mustRebuild = false;

        public bool MustRebuild => mustRebuild;


        public override GraphicsPath[] GetLinesPaths()
        {
            List<GraphicsPath> paths = [];
            var path = new GraphicsPath();
            if (linkPoints.Length > 1)
            {
                if (!IsShort)
                {
                    var fmp = new Point(StartPoint.X, linkPoints.First().Y);
                    path.AddLine(StartPoint, fmp);
                    path.AddLine(fmp, linkPoints.First());
                }
                path.AddLines(linkPoints);
                var lmp = new Point(linkPoints.Last().X, EndPoint.Y);
                path.AddLine(linkPoints.Last(), lmp);
                path.AddLine(lmp, EndPoint);
                paths.Add(path);
            }
            else
            {
                path.AddLine(StartPoint, EndPoint);
                paths.Add(path);
            }
            return [.. paths];
        }

        public override GraphicsPath[] GetDotsPaths()
        {
            List<GraphicsPath> paths = [];
            var path = new GraphicsPath();
            if (linkPoints.Length > 1)
            {
                var pt = linkPoints[0];
                if (pt != StartPoint)
                {
                    var rect = new Rectangle(pt.X - 3, pt.Y - 3, 6, 6);
                    path.AddEllipse(rect);
                    paths.Add(path);
                }
            }
            return [.. paths];
        }

        public override void LinkToLocation(ILocation? source, Point startPoint, 
            ILocation? target, int targetPinIndex, Point endPoint, List<Point> points)
        {
            if (source == null || target == null) return;
            Source = source;
            Target = target;
            TargetPinIndex = targetPinIndex;

            StartShift = new Size(startPoint.X - source.Location.X, startPoint.Y - source.Location.Y);
            EndShift = new Size(endPoint.X - target.Location.X, endPoint.Y - target.Location.Y);

            SetPoints([.. points]);

            source.OnLocationChange += MakeChangesForFirst;
            target.OnLocationChange += MakeChangesForLast;
        }

        public override void UnlinkToLocation(ILocation? source, ILocation? target)
        {
            if (source == null || target == null) return;
            source.OnLocationChange -= MakeChangesForFirst;
            target.OnLocationChange -= MakeChangesForLast;
            Source = null;
            Target = null;
        }

        private void MakeChangesForFirst(object sender, LocationChangedEventArgs e)
        {
            StartPoint = Point.Add(e.NewValue, StartShift);
            mustRebuild = true;
        }

        private void MakeChangesForLast(object sender, LocationChangedEventArgs e)
        {
            EndPoint = Point.Add(e.NewValue, EndShift);
            mustRebuild = true;
        }

        public override Point[] GetPoints()
        {
            return linkPoints;
        }

        public override void SetPoints(Point[] points)
        {
            linkPoints = points;
            IsShort = linkPoints.Length > 0 && linkPoints[0] != StartPoint;
        }

        public override void Rebuild()
        {
            if (mustRebuild)
            {
                mustRebuild = false;
                RebuildLinkFromTarget(this);
            }
        }
    }
}
