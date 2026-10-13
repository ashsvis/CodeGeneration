using PluginSupport;
using System.Drawing.Drawing2D;

namespace LogicModel
{
    public class DigitalInputPageLink : Func
    {
        public DigitalInputPageLink()
        {
            FuncName = "-->D";
            FuncDesc = "¬ход€ща€ дискретна€ ссылка на страницу";
            Inputs = [];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public override DigitalInputPageLink DeepClone()
        {
            return new DigitalInputPageLink()
            {
                id = Id,
                Index = this.Index,
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
            };
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowBorder | AllowedFuncProperties.ShowPins |
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var paths = base.GetGraphicsPaths().ToList();
            var rect = Bounds;
            var path = new GraphicsPath();
            var pinPoint = GetOutputPinPoint(0) ?? Point.Empty;
            path.AddLine(new Point(pinPoint.X - rect.Width / 4, pinPoint.Y), new Point(pinPoint.X, pinPoint.Y - rect.Height / 8));
            path.CloseFigure();
            path.AddLine(new Point(pinPoint.X - rect.Width / 4, pinPoint.Y), new Point(pinPoint.X, pinPoint.Y + rect.Height / 8));
            path.CloseFigure();
            path.AddLine(new Point(pinPoint.X - rect.Width /4, pinPoint.Y), new Point(pinPoint.X, pinPoint.Y));
            path.CloseFigure();
            paths.Add(path);
            return [.. paths];
        }

        public override GraphicsPath[] GetTextPaths()
        {
            var paths = base.GetTextPaths().ToList();
            using var fontFunc = new Font("Segoe UI", 14f);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Center;
            var path = new GraphicsPath();
            var fname = "—сылка # с листа #";
            var pinPoint = GetOutputPinPoint(0) ?? Point.Empty;
            path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Bold,
                fontFunc.Size, Point.Add(pinPoint, new Size(-24, 0)), sf);
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
    }
}
