using PluginSupport;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace PluginOne
{
    public class Func : Shape
    {
        public const int BaseWidth = 48;
        public const int BaseHeight = 48;
        public int Width { get; set; } = BaseWidth;
        public int Height { get; set; } = BaseHeight;
        private int CalcHeight { get; set; }
        protected FuncInput[] Inputs = [];
        protected FuncOutput[] Outputs = [];
        public string? FuncName { get; set; }
        public string? FuncDesc { get; set; }

        public override Shape DeepClone()
        {
            return new Func()
            {
                Location = Location,
                Width = Width,
                Height = Height,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [..Inputs.Select(x => x.DeepClone())],
                Outputs = [..Outputs.Select(x => x.DeepClone())],
                AllowedFuncProperties = AllowedFuncProperties,
            };
        }

        public override XElement WriteContent()
        {
            var xfunc = new XElement($"{this.GetType().FullName}");
            if (!string.IsNullOrEmpty(FuncName))
                xfunc.Add(new XAttribute("Name", FuncName));
            if (!string.IsNullOrEmpty(FuncDesc))
                xfunc.Add(new XAttribute("Description", FuncDesc));
            xfunc.Add(new XAttribute("Location", Location));
            xfunc.Add(new XAttribute("Width", Width));
            xfunc.Add(new XAttribute("Height", Height));            
            xfunc.Add(new XAttribute("Allowed", AllowedFuncProperties));
            foreach (var input in Inputs)
                xfunc.Add(input.WriteContent());
            foreach (var output in Outputs)
                xfunc.Add(output.WriteContent());
            return xfunc;
        }

        public override void ReadContent(XElement xfunc)
        {
            if (xfunc == null || xfunc.Name != $"{this.GetType().FullName}") return;
            FuncName = xfunc.Attribute("Name")?.Value;
            FuncDesc = xfunc.Attribute("Description")?.Value;
            var slocation = xfunc.Attribute("Location")?.Value;
            var swidth = xfunc.Attribute("Width")?.Value;
            var sheight = xfunc.Attribute("Height")?.Value;
            var sallowed = xfunc.Attribute("Allowed")?.Value;
            if (!string.IsNullOrWhiteSpace(slocation) && 
                !string.IsNullOrWhiteSpace(swidth) &&
                !string.IsNullOrWhiteSpace(sheight) &&
                !string.IsNullOrWhiteSpace(sallowed))
            {
                Location = ParseHelper.ParsePoint(slocation, Point.Empty);
                Width = ParseHelper.ParseInteger(swidth, 0);
                Height = ParseHelper.ParseInteger(sheight, 0);
                AllowedFuncProperties = (AllowedFuncProperties)ParseHelper.ParseInteger(sheight, 0xfffffff);
            }
        }

        public AllowedFuncProperties AllowedFuncProperties { get; set; } = AllowedFuncProperties.All;

        protected override void CalculateHeight()
        {
            var step = Height / 2;
            var maxPins = Math.Max(Inputs.Length, Outputs.Length);
            CalcHeight = maxPins > 1 ? step * (maxPins + 1) : Height;
        }

        public override Rectangle Bounds => new(Location.X, Location.Y, Width, CalcHeight);

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var step = Height / 2;
            var maxPins = Math.Max(Inputs.Length, Outputs.Length);
            if (CalcHeight == 0f)
                CalcHeight = maxPins > 1 ? step * (maxPins + 1) : BaseHeight;
            var rect = new Rectangle(Location.X, Location.Y, Width, CalcHeight);
            List<GraphicsPath> paths = [];
            var path = new GraphicsPath();
            if (AllowedFuncProperties.HasFlag(AllowedFuncProperties.ShowBorder))
            {
                // вывод бокса
                path.AddRectangle(rect);
            }
            int hi, ho;
            if (AllowedFuncProperties.HasFlag(AllowedFuncProperties.ShowPins))
            {
                // вывод входов
                hi = Inputs.Length < maxPins ? (CalcHeight - (step * Inputs.Length + 1) + step) / 2 : step;
                for (int i = 0; i < Inputs.Length; i++)
                {
                    path.AddLine(rect.Left, rect.Top + hi, rect.Left - BaseWidth / 4f, rect.Top + hi);
                    path.CloseFigure();
                    hi += step;
                }
                // вывод выходов
                if (Outputs.Length == 1)
                {
                    path.AddLine(rect.Right, rect.Top + rect.Height / 2, rect.Right + BaseWidth / 4, rect.Top + rect.Height / 2);
                    path.CloseFigure();
                }
                else
                {
                    ho = Outputs.Length < maxPins ? (CalcHeight - (step * Outputs.Length + 1) + step) / 2 : step;
                    for (int i = 0; i < Outputs.Length; i++)
                    {
                        path.AddLine(rect.Right, rect.Top + ho, rect.Right + BaseWidth / 4, rect.Top + ho);
                        path.CloseFigure();
                        ho += step;
                    }
                }
            }
            paths.Add(path);
            var sizeInvertRing = Width / 8;
            // вывод инверсий входов
            hi = Inputs.Length < maxPins ? (CalcHeight - (step * Inputs.Length + 1) + step) / 2 : step;
            for (int i = 0; i < Inputs.Length; i++)
            {
                if (Inputs[i].IsInverted)
                {
                    path = new GraphicsPath();
                    var r = new Rectangle(rect.Left - sizeInvertRing / 2, rect.Top + hi - sizeInvertRing / 2, sizeInvertRing, sizeInvertRing);
                    path.AddEllipse(r);
                    paths.Add(path);
                }
                hi += step;
            }
            // вывод инверсий выходов
            ho = Outputs.Length < maxPins ? (CalcHeight - (step * Outputs.Length + 1) + step) / 2 : step;
            for (int i = 0; i < Outputs.Length; i++)
            {
                if (Outputs[i].IsInverted)
                {
                    path = new GraphicsPath();
                    var r = new Rectangle(rect.Right - sizeInvertRing / 2, rect.Top + ho - sizeInvertRing / 2, sizeInvertRing, sizeInvertRing);
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
        public override ToolStripItem[] GetContextMenuItems(Point point, bool several)
        {
            List<ToolStripItem> items = [];
            var targets = GetTargets();
            foreach (var target in targets)
            {
                if (target.Target.Contains(point))
                {
                    if (AllowedFuncProperties.HasFlag(AllowedFuncProperties.PinInverted))
                    {
                        var item = new ToolStripMenuItem("Инвертировать");
                        item.Click += (s, e) =>
                        {
                            if (target.IsOutput)
                                Outputs[target.PinIndex].IsInverted = !Outputs[target.PinIndex].IsInverted;
                            else
                                Inputs[target.PinIndex].IsInverted = !Inputs[target.PinIndex].IsInverted;
                        };
                        items.Add(item);
                    }
                    // если это вход и он связан, то
                    if (!target.IsOutput && Inputs[target.PinIndex].IsLinked)
                    {
                        var item = new ToolStripMenuItem("Удалить связь");
                        item.Click += (s, e) =>
                        {
                            var link = Inputs[target.PinIndex].Link;
                            UnlinkInput(link, target.PinIndex);
                            DeleteLinkFromTarget(link, (ILink)this, target.PinIndex);
                        };
                        items.Add(item);
                    }
                    return [.. items];
                }
            }
            items.AddRange(base.GetContextMenuItems(point, several));
            return [.. items];
        }

        public override GraphicsPath[] GetTextPaths()
        {
            List<GraphicsPath> paths = [];
            // вывод обозначения логической функции
            var path = new GraphicsPath();
            if (AllowedFuncProperties.HasFlag(AllowedFuncProperties.ShowFuncName))
            {
                var fname = FuncName ?? "";
                using var fontFunc = new Font("Segoe UI", 12f);
                var sz = TextRenderer.MeasureText(fname, fontFunc);
                var trect = new Rectangle(Location, new Size(Width, sz.Height));
                using var sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Bold, fontFunc.Size, trect, sf);
                paths.Add(path);
            }
            if (AllowedFuncProperties.HasFlag(AllowedFuncProperties.ShowLabelNumber))
            {
                // вывод номера по порядку выполнения
                path = new GraphicsPath();
                var fnumber = $"L{Index + 1}";
                using var fontNumber = new Font("Segoe UI", 12f);
                var sz = TextRenderer.MeasureText(fnumber, fontNumber);
                var trect = new Rectangle(Location, new Size(Width, CalcHeight));
                using var sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Far;
                path.AddString(fnumber, fontNumber.FontFamily, (int)FontStyle.Bold, fontNumber.Size, trect, sf);
                paths.Add(path);
            }
            if (AllowedFuncProperties.HasFlag(AllowedFuncProperties.ShowPins))
            {
                // значения входов или выходов
                var targets = GetTargets();
                foreach (var target in targets)
                {
                    var p = new GraphicsPath();
                    var value = target.IsOutput ? Outputs[target.PinIndex].Value : Inputs[target.PinIndex].Value;
                    var t = target.Target;
                    t.Offset(0f, -t.Height / 4);
                    using var fontFunc = new Font("Segoe UI", 12f);
                    using var sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    p.AddString($"{(value ? 'T' : 'F')}", fontFunc.FontFamily, (int)FontStyle.Bold, 10f, t, sf);
                    paths.Add(p);
                }
            }
            return [.. paths];
        }

        /// <summary>
        /// Получение массива целей
        /// </summary>
        /// <returns>Кортеж bool - (true: выход, false: вход; индекс входа или выхода; прямоугольник цели)</returns>
        public override TargetInfo[] GetTargets()
        {
            List<TargetInfo> items = [];
            var step = Height / 2;
            var maxPins = Math.Max(Inputs.Length, Outputs.Length);
            var CalcHeight = maxPins > 1 ? step * (maxPins + 1) : Height;
            var rect = new Rectangle(Location.X, Location.Y, Width, CalcHeight);
            var sizeTarget = BaseWidth / 4;
            // вывод целей входов
            var hi = Inputs.Length < maxPins ? (CalcHeight - (step * Inputs.Length + 1) + step) / 2 : step;
            for (int i = 0; i < Inputs.Length; i++)
            {
                var r = new Rectangle(rect.Left - sizeTarget, rect.Top + hi - sizeTarget, sizeTarget, sizeTarget * 2);
                r.Inflate(0, -1);
                items.Add(new TargetInfo(false, i, r));
                hi += step;
            }
            // вывод целей выходов
            if (Outputs.Length == 1)
            {
                var r = new Rectangle(rect.Right, rect.Top + rect.Height / 2 - sizeTarget, sizeTarget, sizeTarget * 2);
                r.Inflate(0, -1);
                items.Add(new TargetInfo(true, 0, r));
            }
            else
            {
                var ho = Outputs.Length < maxPins ? (CalcHeight - (step * Outputs.Length + 1) + step) / 2 : step;
                for (int i = 0; i < Outputs.Length; i++)
                {
                    var r = new Rectangle(rect.Right, rect.Top + ho - sizeTarget, sizeTarget, sizeTarget * 2);
                    r.Inflate(0, -1);
                    items.Add(new TargetInfo(true, i, r));
                    ho += step;
                }
            }
            return [.. items];
        }

        /// <summary>
        /// Получение массива целей
        /// </summary>
        /// <returns>Кортеж bool - (true: выход, false: вход; индекс входа или выхода; точка пина)</returns>
        public override PinInfo[] GetPinPoints()
        {
            List<PinInfo> items = [];
            var step = Height / 2;
            var maxPins = Math.Max(Inputs.Length, Outputs.Length);
            var CalcHeight = maxPins > 1 ? step * (maxPins + 1) : Height;
            var rect = new Rectangle(Location.X, Location.Y, Width, CalcHeight);
            var sizeTarget = BaseWidth / 4;
            // вывод целей входов
            var hi = Inputs.Length < maxPins ? (CalcHeight - (step * Inputs.Length + 1) + step) / 2 : step;
            for (int i = 0; i < Inputs.Length; i++)
            {
                var p = new Point(rect.Left - sizeTarget, rect.Top + hi);
                items.Add(new PinInfo(false, i, p));
                hi += step;
            }
            // вывод целей выходов
            if (Outputs.Length == 1)
            {
                var p = new Point(rect.Right + sizeTarget, rect.Top + rect.Height / 2);
                items.Add(new PinInfo(true, 0, p));
            }
            else
            {
                var ho = Outputs.Length < maxPins ? (CalcHeight - (step * Outputs.Length + 1) + step) / 2 : step;
                for (int i = 0; i < Outputs.Length; i++)
                {
                    var p = new Point(rect.Right + sizeTarget, rect.Top + ho);
                    items.Add(new PinInfo(true, i, p));
                    ho += step;
                }
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
        public override void Click(Point point, Action<TargetInfo>? action = null)
        {
            var targets = GetTargets();
            foreach (TargetInfo info in targets)
            {
                if (info.Target.Contains(point))
                {
                    var pin = info.PinPoint;
                    if (info.IsOutput)
                    {
                        // выходы
                        action?.Invoke(info);
                    }
                    else
                    {
                        // входы
                        if (!Inputs[info.PinIndex].IsLinked)
                        {
                            Inputs[info.PinIndex].Value = !Inputs[info.PinIndex].Value;
                        }
                    }
                }
            }
        }

        public override Point? GetInputPinPoint(int index)
        {
            if (index >= 0 && index < Inputs.Length)
            {
                var item = GetPinPoints().FirstOrDefault(x => !x.IsOutput && x.PinIndex == index);
                if (item != null)
                    return item.PinPoint;
            }
            return null;
        }

        public override Point? GetOutputPinPoint(int index = 0)
        {
            if (index >= 0 && index < Outputs.Length)
            {
                var item = GetPinPoints().FirstOrDefault(x => x.IsOutput && x.PinIndex == index);
                if (item != null)
                    return item.PinPoint;
            }
            return null;
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

        public override int CountInputs()
        {
            return Inputs.Length;
        }

        public override int CountOutputs()
        {
            return Outputs.Length;
        }

        public override Type GetLinkTypeToCreate()
        {
            return typeof(FuncLink);
        }
    }
}
