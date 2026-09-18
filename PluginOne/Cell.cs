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
            var CalcHeight = maxPins > 1 ? step * (maxPins + 1) : Height;
            var rect = new RectangleF(Location.X, Location.Y, Width, CalcHeight);
            List<GraphicsPath> paths = [];
            var path = new GraphicsPath();
            // бокс
            path.AddRectangle(rect);
            // вывод входов
            var hi = Inputs.Length < maxPins ? (CalcHeight - (step * Inputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < Inputs.Length; i++)
            {
                path.AddLine(rect.Left, rect.Top + hi, rect.Left - Width / 4f, rect.Top + hi);
                path.CloseFigure();
                hi += step;
            }
            // вывод выходов
            var ho = Outputs.Length < maxPins ? (CalcHeight - (step * Outputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < Outputs.Length; i++)
            {    
                path.AddLine(rect.Right, rect.Top + ho, rect.Right + Width / 4f, rect.Top + ho);
                path.CloseFigure();
                ho += step;
            }
            paths.Add(path);
            var sizeInvertRing = Width / 6f;
            // вывод инверсий входов
            hi = Inputs.Length < maxPins ? (CalcHeight - (step * Inputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < Inputs.Length; i++)
            {
                if (Inputs[i])
                {
                    path = new GraphicsPath();
                    var r = new RectangleF(rect.Left - sizeInvertRing / 2f, rect.Top + hi - sizeInvertRing / 2f, sizeInvertRing, sizeInvertRing);
                    path.AddEllipse(r);
                    paths.Add(path);
                }
                hi += step;
            }
            // вывод инверсий выходов
            ho = Outputs.Length < maxPins ? (CalcHeight - (step * Outputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < Outputs.Length; i++)
            {
                if (Outputs[i])
                {
                    path = new GraphicsPath();
                    var r = new RectangleF(rect.Right - sizeInvertRing / 2f, rect.Top + ho - sizeInvertRing / 2f, sizeInvertRing, sizeInvertRing);
                    path.AddEllipse(r);
                    paths.Add(path);
                }
                ho += step;
            }
            return [..paths];
        }
    }
}
