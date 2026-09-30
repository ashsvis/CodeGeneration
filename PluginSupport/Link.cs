using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public abstract class Link
    {
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
