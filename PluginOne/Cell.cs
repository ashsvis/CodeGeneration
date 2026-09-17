using PluginSupport;
using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class Cell : Shape
    {
        public float Width { get; set; } = 50f;
        public float Height { get; set; } = 50f;
        protected bool[] Inputs = [];
        protected bool[] Outputs = [];

        public override GraphicsPath[] GetGraphicsPath()
        {
            var step = Height / 2f;
            var maxPins = Math.Max(Inputs.Length, Outputs.Length);
            var CalcHeight = maxPins > 1 ? Height * maxPins - Height / 2f : Height;
            var rect = new RectangleF(Location.X - Width / 2f, Location.Y - CalcHeight / 2f, Width, CalcHeight);
            List<GraphicsPath> paths = [];
            var path = new GraphicsPath();
            // бокс
            path.AddRectangle(rect);
            // вывод входа
            path.AddLine(rect.Left, rect.Top + rect.Height / 2f, rect.Left - Width / 4f, rect.Top + rect.Height / 2f);
            path.CloseFigure();
            // вывод выхода
            path.AddLine(rect.Right, rect.Top + rect.Height / 2f, rect.Right + Width / 4f, rect.Top + rect.Height / 2f);
            path.CloseFigure();
            paths.Add(path);
            if (Inputs.Length == 1 && Inputs[0])
            {
                path = new GraphicsPath();
                // инверсия входа
                var h = Width / 6f;
                var r = new RectangleF(rect.Left - h / 2f, rect.Top - h / 2f + rect.Height / 2f, h, h);
                path.AddEllipse(r);
                paths.Add(path);
            }
            if (Outputs.Length == 1 && Outputs[0])
            {
                path = new GraphicsPath();
                // инверсия выхода
                var h = Width / 6f;
                var r = new RectangleF(rect.Right - h / 2f, rect.Top - h / 2f + rect.Height / 2f, h, h);
                path.AddEllipse(r);
                paths.Add(path);
            }
            return [..paths];
        }
    }
}
