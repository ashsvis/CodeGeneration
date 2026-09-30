using PluginSupport;
using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class CellLink : Link
    {
        public override PointF StartPoint { get; set; }
        public override PointF EndPoint { get; set; }

        public override GraphicsPath[] GetGraphicsPaths()
        {
            List<GraphicsPath> paths = [];
            var path = new GraphicsPath();
            path.AddLine(StartPoint, EndPoint);
            paths.Add(path);
            return [.. paths];
        }
    }
}
