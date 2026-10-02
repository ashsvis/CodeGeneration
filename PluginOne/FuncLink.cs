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


        private Size StartShift { get; set; }
        private Size EndShift { get; set; }

        private Point[] linkPoints = [];
        private bool mustRebuild = false;

        public bool MustRebuild => mustRebuild;


        public override GraphicsPath[] GetGraphicsPaths()
        {
            List<GraphicsPath> paths = [];
            var path = new GraphicsPath();
            if (linkPoints.Length > 1)
                path.AddLines(linkPoints);
            else
                path.AddLine(StartPoint, EndPoint);
            paths.Add(path);
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

            linkPoints = [.. points];

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
        }

        public override void Update()
        {
            if (mustRebuild)
            {
                mustRebuild = false;
                RebuildLinkFromTarget(this);
            }
        }
    }
}
