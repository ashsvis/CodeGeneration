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

        public abstract GraphicsPath GetGraphicsPath();

        public virtual void Draw(Graphics? g)
        {
            using var path = GetGraphicsPath();
            using var brush = new SolidBrush(Background);
            g?.FillPath(brush, path);
            using var pen = new Pen(Foreground, 1);
            g?.DrawPath(pen, path);
        }

        public virtual void Draw(Graphics? g, Pen pen, Brush brush)
        {
            using var path = GetGraphicsPath();
            g?.FillPath(brush, path);
            g?.DrawPath(pen, path);
        }

        public bool ContainsPoint(PointF point)
        {
            using var pen = new Pen(Foreground, 1);
            using var path = GetGraphicsPath();
            return path.IsOutlineVisible(point, pen) || path.IsVisible(point);
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
