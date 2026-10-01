using PluginSupport;
using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class FuncLink : PluginSupport.Link
    {
        public override ILocation? Source { get; set; }
        public override ILocation? Target { get; set; }
        public override PointF StartPoint { get; set; }
        public override PointF EndPoint { get; set; }


        private SizeF StartShift { get; set; }
        private SizeF EndShift { get; set; }

        private PointF[] linkPoints = [];
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

        public override void LinkToLocation(ILocation? source, PointF startPoint, ILocation? target, PointF endPoint, List<PointF> points)
        {
            if (source == null || target == null) return;
            Source = source;
            Target = target;

            StartShift = new SizeF(startPoint.X - source.Location.X, startPoint.Y - source.Location.Y);
            EndShift = new SizeF(endPoint.X - target.Location.X, endPoint.Y - target.Location.Y);

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
            StartPoint = PointF.Add(e.NewValue, StartShift);
            mustRebuild = true;
        }

        private void MakeChangesForLast(object sender, LocationChangedEventArgs e)
        {
            EndPoint = PointF.Add(e.NewValue, EndShift);
            mustRebuild = true;
        }

        public override PointF[] GetPoints()
        {
            return linkPoints;
        }

        public override void SetPoints(PointF[] points)
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
