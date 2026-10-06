using PluginSupport;
using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class DigitalInputTag : Func, ILinked
    {
        public DigitalInputTag()
        {
            FuncDesc = "Тег дискретного входа";
            FuncName = "TAG_DI";
            Inputs = [];
            Outputs = [new() { }];
            Background = SystemColors.Control;
            Width = 48 * 3;
            CalculateHeight();
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowBorder |
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        public override Shape DeepClone()
        {
            return new DigitalInputTag()
            {
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
            };
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var paths = base.GetGraphicsPaths().ToList();
            var rect = Bounds;
            var path = new GraphicsPath();
            path.AddArc(new Rectangle(rect.X, rect.Y, rect.Height, rect.Height), 90f, 180f);
            path.AddArc(new Rectangle(rect.X + rect.Width - rect.Height, rect.Y, rect.Height, rect.Height), 270f, 180f);
            path.CloseFigure();
            paths.Add(path);
            return [.. paths];
        }

        public override GraphicsPath[] GetTextPaths()
        {
            var paths = base.GetTextPaths().ToList();
            var rect = Bounds;
            rect.Inflate(rect.Height / 4, 0);
            var fname = FuncName ?? "";
            using var fontFunc = new Font("Segoe UI", 12f);
            var sz = TextRenderer.MeasureText(fname, fontFunc);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            var path = new GraphicsPath();
            path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Bold, fontFunc.Size, rect, sf);
            paths.Add(path);
            return [.. paths];
        }

        public virtual void UpdateLinked(bool result)
        {
            OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            link.OnOutputChange += MakeChanges;
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
        }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            link.OnOutputChange -= MakeChanges;
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
        }

        public void MakeChanges(object? sender, OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }
    }
}
