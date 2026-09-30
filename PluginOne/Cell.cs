using PluginSupport;
using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class Cell : Shape
    {
        public float Width { get; set; } = 48f;
        public float Height { get; set; } = 48f;
        protected float CalcHeight { get; set; }
        protected CellInput[] Inputs = [];
        protected CellOutput[] Outputs = [];
        public string? FuncName { get; set; }
        public string? FuncDesc { get; set; }

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var step = Height / 2f;
            var maxPins = Math.Max(Inputs.Length, Outputs.Length);
            CalcHeight = maxPins > 1 ? step * (maxPins + 1) : Height;
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
                if (Inputs[i].IsInverted)
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
                if (Outputs[i].IsInverted)
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
                            Outputs[target.Item2].IsInverted = !Outputs[target.Item2].IsInverted;
                        else
                            Inputs[target.Item2].IsInverted = !Inputs[target.Item2].IsInverted;
                    };
                    items.Add(item);
                    // если это вход и он связан, то
                    if (!target.Item1 && Inputs[target.Item2].IsLinked)
                    {
                        item = new ToolStripMenuItem() { Text = $"Удалить связь по входу {target.Item2 + 1}" };
                        item.Click += (s, e) =>
                        {
                            UnlinkInput(Inputs[target.Item2].Link, target.Item2);
                        };
                        items.Add(item);
                    }
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
            using var fontFunc = new Font("Segoe UI", 12f);
            var sz = TextRenderer.MeasureText(fname, fontFunc);
            var trect = new RectangleF(Location, new SizeF(Width, sz.Height));
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Bold, fontFunc.Size, trect, sf);
            paths.Add(path);
            // вывод номера по порядку выполнения
            path = new GraphicsPath();
            var fnumber = $"L{Index + 1}";
            using var fontNumber = new Font("Segoe UI", 12f);
            sz = TextRenderer.MeasureText(fnumber, fontNumber);
            trect = new RectangleF(Location, new SizeF(Width, CalcHeight));
            using var sf1 = new StringFormat();
            sf1.Alignment = StringAlignment.Center;
            sf1.LineAlignment = StringAlignment.Far;
            path.AddString(fnumber, fontNumber.FontFamily, (int)FontStyle.Bold, fontFunc.Size, trect, sf1);
            paths.Add(path);
            // значения входов или выходов
            var targets = GetTargets();
            foreach (var target in targets)
            {
                var p = new GraphicsPath();
                var value = target.Item1 ? Outputs[target.Item2].Value : Inputs[target.Item2].Value;
                var r = target.Item3;
                r.Offset(0f, -r.Height / 2f);
                p.AddString($"{(value ? 'T' : 'F')}", fontFunc.FontFamily, (int)FontStyle.Bold, 10f, r, sf);
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
            var maxPins = Math.Max(Inputs.Length, Outputs.Length);
            var CalcHeight = maxPins > 1 ? step * (maxPins + 1) : Height;
            var rect = new RectangleF(Location.X, Location.Y, Width, CalcHeight);
            var sizeTarget = Width / 4f;
            // вывод целей входов
            var hi = Inputs.Length < maxPins ? (CalcHeight - (step * Inputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < Inputs.Length; i++)
            {
                var r = new RectangleF(rect.Left - sizeTarget, rect.Top + hi - sizeTarget / 2f, sizeTarget, sizeTarget);
                items.Add(new Tuple<bool, int, RectangleF>(false, i, r));
                hi += step;
            }
            // вывод целей выходов
            var ho = Outputs.Length < maxPins ? (CalcHeight - (step * Outputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < Outputs.Length; i++)
            {
                var r = new RectangleF(rect.Right, rect.Top + ho - sizeTarget / 2f, sizeTarget, sizeTarget);
                items.Add(new Tuple<bool, int, RectangleF>(true, i, r));
                ho += step;
            }
            return [.. items];
        }

        /// <summary>
        /// Получение массива целей
        /// </summary>
        /// <returns>Кортеж bool - (true: выход, false: вход; индекс входа или выхода; точка пина)</returns>
        public override Tuple<bool, int, PointF>[] GetPinPoints()
        {
            List<Tuple<bool, int, PointF>> items = [];
            var step = Height / 2f;
            var maxPins = Math.Max(Inputs.Length, Outputs.Length);
            var CalcHeight = maxPins > 1 ? step * (maxPins + 1) : Height;
            var rect = new RectangleF(Location.X, Location.Y, Width, CalcHeight);
            var sizeTarget = Width / 4f;
            // вывод целей входов
            var hi = Inputs.Length < maxPins ? (CalcHeight - (step * Inputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < Inputs.Length; i++)
            {
                var p = new PointF(rect.Left - sizeTarget, rect.Top + hi - sizeTarget / 2f);
                items.Add(new Tuple<bool, int, PointF>(false, i, p));
                hi += step;
            }
            // вывод целей выходов
            var ho = Outputs.Length < maxPins ? (CalcHeight - (step * Outputs.Length + 1) + step) / 2f : step;
            for (int i = 0; i < Outputs.Length; i++)
            {
                var p = new PointF(rect.Right, rect.Top + ho - sizeTarget / 2f);
                items.Add(new Tuple<bool, int, PointF>(true, i, p));
                ho += step;
            }
            return [.. items];
        }

        public override void Calculate() { }
        public override void LinkInput(ILink? link, int index) { }
        public override void UnlinkInput(ILink? link, int index) { }

        public override void UnlinkAllInputs() 
        { 
            for (var i = 0; i < Inputs.Length; i++)
                UnlinkInput(Inputs[i].Link, i);
        }

        public override void UnlinkOutputFor(ILink? link)
        {
            foreach (var input in Inputs.Where(x => x.Link == link))
            {
                for (var i = 0; i < Inputs.Length; i++)
                {
                    if (Inputs[i] == input)
                        UnlinkInput(Inputs[i].Link, i);
                }
            }
        }

        public override bool IsLinked(int index) 
        { 
            if (index >= 0 && index < Inputs.Length)
                return Inputs[index].IsLinked;
            return false; 
        }

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
                        action?.Invoke(true, target.Item2, target.Item3);
                    }
                    else
                    {
                        // входы
                        if (!Inputs[target.Item2].IsLinked)
                        {
                            Inputs[target.Item2].Value = !Inputs[target.Item2].Value;
                        }
                    }
                }
            }
        }

        public override object? GetOutputValue(int index = 0)
        {
            if (index >=0 && index < Outputs.Length)
                return Outputs[index].Value;
            return null;
        }

        public override void SetInputValue(int index, object? value)
        {
            if (index >= 0 && index < Inputs.Length)
            {
                Inputs[index].Value = Convert.ToBoolean(value);
            }
        }
    }
}
