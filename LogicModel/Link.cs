using PluginSupport;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace LogicModel
{
    public class Link : PluginSupport.Link
    {
        public override ILocation? Source { get; set; }
        public override int SourceIndex { get; set; }
        public override ILocation? Target { get; set; }
        public override int TargetIndex { get; set; }
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

        public override bool NoDataToWrite()
        {
            return false;
        }

        public override XElement WriteContent()
        {
            var xlink = new XElement($"{this.GetType().FullName}");
            if (Source is ILocation souloc)
                xlink.Add(new XAttribute("Source", souloc.Index));
            if (Target is ILocation tarloc)
            {
                xlink.Add(new XAttribute("Target", tarloc.Index));
                xlink.Add(new XAttribute("Pin", TargetPinIndex));
            }
            xlink.Add(new XAttribute("Start", $"{StartPoint.X},{StartPoint.Y}"));
            xlink.Add(new XAttribute("End", $"{EndPoint.X},{EndPoint.Y}"));
            xlink.Add(new XAttribute("Points", string.Join(" ", linkPoints.Select(p => $"{p.X},{p.Y}"))));
            return xlink;
        }

        public override void ReadContent(XElement xlink)
        {
            if (xlink == null || xlink.Name != $"{this.GetType().FullName}") return;
            var sSource = xlink.Attribute("Source")?.Value;
            if (!string.IsNullOrWhiteSpace(sSource))
                SourceIndex = ParseHelper.ParseInteger(sSource, 0);
            var sTarget = xlink.Attribute("Target")?.Value;
            if (!string.IsNullOrWhiteSpace(sTarget))
                TargetIndex = ParseHelper.ParseInteger(sTarget, 0);
            var sPin = xlink.Attribute("Pin")?.Value;
            if (!string.IsNullOrWhiteSpace(sPin))
                TargetPinIndex = ParseHelper.ParseInteger(sPin, 0);
            var sStart = xlink.Attribute("Start")?.Value;
            if (!string.IsNullOrWhiteSpace(sStart))
            {
                var vals = sStart.Split(',');
                if (vals.Length == 2)
                    StartPoint = new Point(ParseHelper.ParseInteger(vals[0], 0), ParseHelper.ParseInteger(vals[1], 0));
            }
            var sEnd = xlink.Attribute("End")?.Value;
            if (!string.IsNullOrWhiteSpace(sEnd))
            {
                var vals = sEnd.Split(',');
                if (vals.Length == 2)
                    EndPoint = new Point(ParseHelper.ParseInteger(vals[0], 0), ParseHelper.ParseInteger(vals[1], 0));
            }
            var sPoints = xlink.Attribute("Points")?.Value;
            if (!string.IsNullOrWhiteSpace(sPoints))
            {
                var pvals = sPoints.Split(' ');
                List<Point> points = [];
                foreach (var p in pvals)
                {
                    var vals = p.Split(',');
                    if (vals.Length == 2)
                    {
                        var point = new Point(ParseHelper.ParseInteger(vals[0], 0), ParseHelper.ParseInteger(vals[1], 0));
                        points.Add(point);
                    }
                }
                SetPoints([.. points]);
            }
        }

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
                    var rect = new Rectangle(pt.X - 2, pt.Y - 2, 4, 4);
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
