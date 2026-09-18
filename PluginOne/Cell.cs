using PluginSupport;
using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class Cell : Shape
    {
        public float Width { get; set; } = 50f;
        public float Height { get; set; } = 50f;
        protected bool[] InvertInputs = [];
        protected bool[] InvertOutputs = [];
        protected bool[] Inputs = [];
        protected bool[] Outputs = [];
        public string? FuncName { get; set; }
        public string? FuncDesc { get; set; }

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var step = Height / 2f;
            var maxPins = Math.Max(InvertInputs.Length, InvertOutputs.Length);
            var CalcHeight = maxPins > 1 ? step * (maxPins + 1) : Height;
            var rect = new RectangleF(Location.X, Location.Y, Width, CalcHeight);
            List<GraphicsPath> paths = [];
            // вывод бокса
            var path = new GraphicsPath();
            path.AddRectangle(rect);
            // вывод входов
            var hi = InvertInputs.Length < maxPins ? (CalcHeight - (step * InvertInputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < InvertInputs.Length; i++)
            {
                path.AddLine(rect.Left, rect.Top + hi, rect.Left - Width / 4f, rect.Top + hi);
                path.CloseFigure();
                hi += step;
            }
            // вывод выходов
            var ho = InvertOutputs.Length < maxPins ? (CalcHeight - (step * InvertOutputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < InvertOutputs.Length; i++)
            {    
                path.AddLine(rect.Right, rect.Top + ho, rect.Right + Width / 4f, rect.Top + ho);
                path.CloseFigure();
                ho += step;
            }
            paths.Add(path);
            var sizeInvertRing = Width / 6f;
            // вывод инверсий входов
            hi = InvertInputs.Length < maxPins ? (CalcHeight - (step * InvertInputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < InvertInputs.Length; i++)
            {
                if (InvertInputs[i])
                {
                    path = new GraphicsPath();
                    var r = new RectangleF(rect.Left - sizeInvertRing / 2f, rect.Top + hi - sizeInvertRing / 2f, sizeInvertRing, sizeInvertRing);
                    path.AddEllipse(r);
                    paths.Add(path);
                }
                hi += step;
            }
            // вывод инверсий выходов
            ho = InvertOutputs.Length < maxPins ? (CalcHeight - (step * InvertOutputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < InvertOutputs.Length; i++)
            {
                if (InvertOutputs[i])
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
                    item.Click += (s, e) => 
                    {
                        if (target.Item1)
                            InvertOutputs[target.Item2] = !InvertOutputs[target.Item2];
                        else
                            InvertInputs[target.Item2] = !InvertInputs[target.Item2];
                        Calculate(Inputs, InvertInputs);
                    };
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
            using var font = new Font("Segoe UI", 12f);
            var sz = TextRenderer.MeasureText(fname, font);
            var trect = new RectangleF(Location, new SizeF(Width, sz.Height));
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            path.AddString(fname, font.FontFamily, 0, font.Size, trect, sf);
            paths.Add(path);
            // значения входов или выходов
            var targets = GetTargets();
            foreach (var target in targets)
            {
                var p = new GraphicsPath();
                var value = target.Item1 ? Outputs[target.Item2] ^ InvertOutputs[target.Item2] : Inputs[target.Item2];
                var r = target.Item3;
                r.Offset(0f, -r.Height / 2f);
                p.AddString(value.ToString()[..1], font.FontFamily, 0, 10f, r, sf);
                paths.Add(p);
            }
            return [.. paths];
        }

        /// <summary>
        /// Получение массива целей
        /// </summary>
        /// <returns>Кортеж bool - (true: выход, false: вход; индекс входа или выхода; прямоугольник цели)</returns>
        public override Tuple<bool, int, RectangleF>[] GetTargets()
        {
            List<Tuple<bool, int, RectangleF>> items = [];
            var step = Height / 2f;
            var maxPins = Math.Max(InvertInputs.Length, InvertOutputs.Length);
            var CalcHeight = maxPins > 1 ? step * (maxPins + 1) : Height;
            var rect = new RectangleF(Location.X, Location.Y, Width, CalcHeight);
            var sizeTarget = Width / 4f;
            // вывод целей входов
            var hi = InvertInputs.Length < maxPins ? (CalcHeight - (step * InvertInputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < InvertInputs.Length; i++)
            {
                var r = new RectangleF(rect.Left - sizeTarget, rect.Top + hi - sizeTarget / 2f, sizeTarget, sizeTarget);
                items.Add(new Tuple<bool, int, RectangleF>(false, i, r));
                hi += step;
            }
            // вывод целей выходов
            var ho = InvertOutputs.Length < maxPins ? (CalcHeight - (step * InvertOutputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < InvertOutputs.Length; i++)
            {
                var r = new RectangleF(rect.Right, rect.Top + ho - sizeTarget / 2f, sizeTarget, sizeTarget);
                items.Add(new Tuple<bool, int, RectangleF>(true, i, r));
                ho += step;
            }
            return [.. items];
        }

        public virtual void Calculate(bool[] values, bool[] inverts) { }

        /// <summary>
        /// Обработка клика по цели
        /// </summary>
        /// <param name="point">Точка нажатия</param>
        public override void Click(PointF point, Action<bool, int, RectangleF>? action = null)
        {
            var targets = GetTargets();
            foreach (var target in targets)
            {
                if (target.Item3.Contains(point))
                {
                    var pin = target.Item2;
                    if (target.Item1)
                    {
                        // выходы
                        //Outputs[target.Item2] = !Outputs[target.Item2];
                        action?.Invoke(true, target.Item2, target.Item3);
                    }
                    else
                    {
                        // входы
                        Inputs[target.Item2] = !Inputs[target.Item2];
                        Calculate(Inputs, InvertInputs);
                    }
                }
            }
        }
    }
}
