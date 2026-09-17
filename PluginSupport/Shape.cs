using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public abstract class Shape : ILocation
    {
        public PointF Location { get; set; }
        public Color Foreground { get; set; } = Color.FromArgb(200, 200, 200);
        public Color Background { get; set; } = Color.FromArgb(50, 50, 50);

        public bool Selected { get; set; }
        public bool Hover { get; set; }

        public abstract GraphicsPath[] GetGraphicsPath();

        public virtual void Draw(Graphics? g, Pen? pen = null, Brush? brush = null)
        {
            using var Brush = new SolidBrush(Background);
            using var Pen = new Pen(Foreground, 1);
            foreach (var p in GetGraphicsPath())
            {
                using var path = p;
                g?.FillPath(brush ?? Brush, path);
                g?.DrawPath(pen ?? Pen, path);
            }
        }

        public bool ContainsPoint(PointF point)
        {
            using var pen = new Pen(Foreground, 1);
            foreach (var p in GetGraphicsPath())
            {
                using var path = p;
                if (path.IsOutlineVisible(point, pen) || path.IsVisible(point))
                    return true;
            }
            return false;
        }

        public virtual ToolStripItem[] GetContextMenuItems(bool several)
        {
            List<ToolStripItem> items = [];
            var item = new ToolStripMenuItem() { Text = "Поднять наверх" };
            items.Add(item);
            item = new ToolStripMenuItem() { Text = "Поднять выше" };
            items.Add(item);
            item = new ToolStripMenuItem() { Text = "Опустить ниже" };
            items.Add(item);
            item = new ToolStripMenuItem() { Text = "Опустить вниз" };
            items.Add(item);
            items.Add((ToolStripItem)new ToolStripSeparator());
            item = new ToolStripMenuItem() { Text = "Удалить" };
            items.Add(item);
            return [.. items];
        }
    }
}
