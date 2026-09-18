using PluginSupport;
using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class Circle: Shape
    {
        public float Radius { get; set; } = 50f;

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var rect = new RectangleF(Location.X - Radius, Location.Y - Radius, Radius * 2f, Radius * 2f);
            var path = new GraphicsPath();
            path.AddEllipse(rect);
            return [path];
        }

        public override ToolStripItem[] GetContextMenuItems(PointF point, bool several)
        {
            List<ToolStripItem> items = [];
            var item = new ToolStripMenuItem() { Text = "Круг", Enabled = false };
            if (!several)
                items.Add(item);
            var baseItems = base.GetContextMenuItems(point, several);
            if (!several && baseItems.Length > 0)
                items.Add(new ToolStripSeparator());
            items.AddRange(baseItems);
            return [.. items];
        }
    }
}
