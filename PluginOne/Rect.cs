using PluginSupport;
using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class Rect : Shape
    {
        public float Width { get; set; } = 100f;
        public float Height { get; set; } = 80f;

        public override GraphicsPath[] GetGraphicsPath()
        {
            var rect = new RectangleF(Location.X - Width / 2f, Location.Y - Height / 2f, Width, Height);
            var path = new GraphicsPath();
            path.AddRectangle(rect);
            return [path];
        }

        public override ToolStripItem[] GetContextMenuItems(bool several)
        {
            List<ToolStripItem> items = [];
            var item = new ToolStripMenuItem() { Text = "Прямоугольник", Enabled = false };
            if (!several)
                items.Add(item);
            var baseItems = base.GetContextMenuItems(several);
            if (!several && baseItems.Length > 0)
                items.Add(new ToolStripSeparator());
            items.AddRange(baseItems);
            return [.. items];
        }
    }
}
