using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public abstract class Link
    {
        public abstract PointF StartPoint { get; set; }
        public abstract PointF EndPoint { get; set; }
        public abstract GraphicsPath[] GetGraphicsPaths();

        public virtual void Draw(Graphics? g, Pen pen)
        {
            foreach (var p in GetGraphicsPaths())
            {
                using var path = p;
                g?.DrawPath(pen, path);
            }
        }
    }
}
