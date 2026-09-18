using PluginSupport;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace PluginOne
{
    public class Cell : Shape
    {
        public float Width { get; set; } = 50f;
        public float Height { get; set; } = 50f;
        protected bool[] Inputs = [];
        protected bool[] Outputs = [];
        public string? FuncName { get; set; }
        public string? FuncDesc { get; set; }

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var step = Height / 2f;
            var maxPins = Math.Max(Inputs.Length, Outputs.Length);
            var CalcHeight = maxPins > 1 ? step * (maxPins + 1) : Height;
            var rect = new RectangleF(Location.X, Location.Y, Width, CalcHeight);
            List<GraphicsPath> paths = [];
            // вывод бокса
            var path = new GraphicsPath();
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

        /// <summary>
        /// Добавление пунктов в контекстное меню
        /// </summary>
        /// <param name="point">Точка нажатия на элементе</param>
        /// <param name="several">Признак выбора нескольких элементов</param>
        /// <returns></returns>
        public override ToolStripItem[] GetContextMenuItems(PointF point, bool several)
        {
            List<ToolStripItem> items = [];
            var targets = GetTargets();
            foreach (var target in targets)
            {
                if (target.Item3.Contains(point))
                {
                    var io = target.Item1 ? "выход" : "выход";
                    var item = new ToolStripMenuItem() { Text = $"Инвертировать {io} {target.Item2 + 1}" };
                    items.Add(item);
                    return [.. items];
                }
            }
            return [.. items];
        }

        public override GraphicsPath[] GetTextPaths()
        {
            List<GraphicsPath> paths = [];
            // вывод обозначения логической функции
            var path = new GraphicsPath();
            var fname = FuncName ?? "";
            using var font = new Font("Arial", 18f);
            var sz = TextRenderer.MeasureText(fname, font);
            var trect = new RectangleF(Location, new SizeF(Width, sz.Height));
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            path.AddString(fname, font.FontFamily, 0, font.Size, trect, sf);
            path.CloseFigure();
            paths.Add(path);
            return [.. paths];
        }

        public override Tuple<bool, int, RectangleF>[] GetTargets()
        {
            List<Tuple<bool, int, RectangleF>> items = [];
            var step = Height / 2f;
            var maxPins = Math.Max(Inputs.Length, Outputs.Length);
            var CalcHeight = maxPins > 1 ? step * (maxPins + 1) : Height;
            var rect = new RectangleF(Location.X, Location.Y, Width, CalcHeight);
            var sizeTarget = Width / 4f;
            // вывод таргетов входов
            var hi = Inputs.Length < maxPins ? (CalcHeight - (step * Inputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < Inputs.Length; i++)
            {
                var r = new RectangleF(rect.Left - sizeTarget, rect.Top + hi - sizeTarget / 2f, sizeTarget, sizeTarget);
                items.Add(new Tuple<bool, int, RectangleF>(false, i, r));
                hi += step;
            }
            // вывод таргетов выходов
            var ho = Outputs.Length < maxPins ? (CalcHeight - (step * Outputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < Outputs.Length; i++)
            {
                var r = new RectangleF(rect.Right, rect.Top + ho - sizeTarget / 2f, sizeTarget, sizeTarget);
                items.Add(new Tuple<bool, int, RectangleF>(true, i, r));
                ho += step;
            }
            return [.. items];
        }
    }
}
