using PluginSupport;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace LogicModel
{
    public class DigitalOutputPageLink : Func, IPageLink
    {
        public DigitalOutputPageLink()
        {
            FuncName = "D-->";
            FuncDesc = "Исходящая дискретная ссылка со страницы";
            Inputs = [new PinInput { Kind = ValueKind.Digital }];
            Outputs = [];
            CalculateHeight();
        }

        [ReadOnly(true)]
        public int Number { get; set; }

        public override DigitalOutputPageLink DeepClone()
        {
            return new DigitalOutputPageLink()
            {
                id = Id,
                Index = this.Index,
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [],
            };
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowBorder | AllowedFuncProperties.ShowPins |
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        public ValueKind ValueKind => ValueKind.Digital;

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var paths = base.GetGraphicsPaths().ToList();
            var rect = Bounds;
            var path = new GraphicsPath();
            var pinPoint = GetInputPinPoint(0) ?? Point.Empty;
            path.AddLine(new Point(pinPoint.X, pinPoint.Y - rect.Height / 8), new Point(pinPoint.X + rect.Width / 4, pinPoint.Y));
            path.CloseFigure();
            path.AddLine(new Point(pinPoint.X, pinPoint.Y + rect.Height / 8), new Point(pinPoint.X + rect.Width / 4, pinPoint.Y));
            path.CloseFigure();
            path.AddLine(new Point(pinPoint.X, pinPoint.Y), new Point(pinPoint.X + rect.Width / 4, pinPoint.Y));
            path.CloseFigure();
            paths.Add(path);
            return [.. paths];
        }

        public override GraphicsPath[] GetTextPaths()
        {
            var paths = base.GetTextPaths().ToList();
            var rect = Bounds;
            using var fontFunc = new Font("Segoe UI", 14f);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            var path = new GraphicsPath();
            var fname = $"Ссылка {Number} на лист(ы) #";
            var sz = TextRenderer.MeasureText(fname, fontFunc);
            rect.Width = sz.Width;
            path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Regular, 
                fontFunc.Size, new Point(rect.Left + 12, rect.Top + rect.Height / 2), sf);
            paths.Add(path);
            return [.. paths];
        }

        public override bool ContainsPoint(Point point, float width)
        {
            using var pen = new Pen(Foreground, width);
            foreach (var p in GetTextPaths())
            {
                using var path = p;
                if (path.IsOutlineVisible(point, pen) || path.IsVisible(point))
                    return true;
            }
            return false;
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value > 0;
                //if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                //{
                //    Outputs[0].Value = result ? 1 : 0;
                //    base.Calculate();
                //}
            }
        }

        public void SetLinkNumber(int number)
        {
            Number = number;
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
            var item = new ToolStripMenuItem("Установить связь...");
            item.Click += (s, e) =>
            {
            };
            items.Add(item);
            items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem() { Text = "Удалить" };
            item.Click += (s, e) => Delete();
            items.Add(item);
            return [.. items];
        }
    }
}
