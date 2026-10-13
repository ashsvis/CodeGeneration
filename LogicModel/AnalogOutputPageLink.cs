using PluginSupport;
using System;
using System.Drawing.Drawing2D;

namespace LogicModel
{
    public class AnalogOutputPageLink : Func
    {
        public AnalogOutputPageLink()
        {
            FuncName = "A-->";
            FuncDesc = "»сход€ща€ аналогова€ ссылка со страницы";
            Inputs = [new PinInput { Kind = ValueKind.Analog }];
            Outputs = [];
            CalculateHeight();
        }

        public override AnalogOutputPageLink DeepClone()
        {
            return new AnalogOutputPageLink()
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
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowBorder | AllowedFuncProperties.ShowPins|
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

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
            var fname = "—сылка # на лист(ы) #";
            var sz = TextRenderer.MeasureText(fname, fontFunc);
            rect.Width = sz.Width;
            path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Bold,
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
    }
}
