using PluginSupport;
using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class DescriptionBox : Func, ILink
    {
        public DescriptionBox()
        {
            FuncDesc = "Описание параметра";
            FuncName = "TAG_DESCRITION";
            Width = 48 * 5;
            Height = 48 * 2;
            Inputs = [new() { }];
            Outputs = [new() { }];
            Background = SystemColors.Control;
            AllowedFuncProperties = AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowPins |
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);
            CalculateHeight();
        }

        public override Shape DeepClone()
        {
            return new DescriptionBox()
            {
                Location = Location,
                Width = Width,
                Height = Height,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
                AllowedFuncProperties = AllowedFuncProperties,
            };
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override GraphicsPath[] GetTextPaths()
        {
            var paths = base.GetTextPaths().ToList();
            var rect = Bounds;
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

        public override void LinkInput(ILink? link, int index)
        {
            if (link == null) return;
            link.OnOutputChange += MakeChanges;
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
        }

        public override void UnlinkInput(ILink? link, int index)
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

        public override PinInfo[] GetPinPoints()
        {
            var infos = base.GetPinPoints();
            var sizeTarget = BaseWidth / 4;
            foreach (var pin in infos)
            {
                if (pin.IsOutput)
                    pin.PinPoint = Point.Subtract(pin.PinPoint, new Size(sizeTarget, 0));
                else
                    pin.PinPoint = Point.Add(pin.PinPoint, new Size(sizeTarget, 0));
            }
            return infos;
        }
    }
}
