using PluginSupport;
using System.Drawing.Drawing2D;

namespace LogicModel
{
    public class DigitalInput : Func
    {
        public DigitalInput()
        {
            FuncName = "D/#";
            FuncDesc = "Дискретный ввод сигнала";
            Inputs = [new PinInput { Kind = ValueKind.DI }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        public override DigitalInput DeepClone()
        {
            return new DigitalInput()
            {
                id = Id,
                Index = this.Index,
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
            };
        }

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var paths = base.GetGraphicsPaths().ToList();
            var rect = Bounds;
            var path = new GraphicsPath();
            path.AddLine(new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y));
            paths.Add(path);
            return [..paths];
        }

        public override GraphicsPath[] GetTextPaths()
        {
            var paths = base.GetTextPaths().ToList();
            var rect = Bounds;
            rect.Width /= 2;
            rect.Height /= 2;
            using var fontFunc = new Font("Segoe UI", 12f);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            var path = new GraphicsPath();
            var fname = (FuncName ?? "").Split('/').First();
            var sz = TextRenderer.MeasureText(fname, fontFunc);
            path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Bold, fontFunc.Size, rect, sf);
            paths.Add(path);
            path = new GraphicsPath();
            rect.Offset(rect.Width, rect.Height);
            fname = (FuncName ?? "").Split('/').Last();
            sz = TextRenderer.MeasureText(fname, fontFunc);
            path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Bold, fontFunc.Size, rect, sf);
            paths.Add(path);
            return [..paths];
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    base.Calculate();
                }
            }
        }
    }
}
