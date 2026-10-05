using PluginSupport;
using System.Drawing.Drawing2D;
using System.IO;

namespace PluginOne
{
    public class DigitalInput : Func, ILink
    {
        public DigitalInput()
        {
            FuncName = "D/#";
            FuncDesc = "¬вод дискретного сигнала";
            Inputs = [new() { }];
            Outputs = [new() { }];
            AllowedPinInverted = false;
            AllowedShowFuncName = false;
            AllowedShowLabelNumber = false;
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

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
    }
}
