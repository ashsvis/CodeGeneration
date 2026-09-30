using PluginSupport;
using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class Rs : Cell, ILink
    {
        public Rs()
        {
            FuncName = "RS";
            FuncDesc = "RS-триггер";
            Inputs = [new() { Name = "S" }, new() { Name = "R" }];
            Outputs = [new() { Name = "Q" }];
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override GraphicsPath[] GetTextPaths()
        {
            List<GraphicsPath> paths = [..base.GetTextPaths()];
            using var fontFunc = new Font("Segoe UI", 12f);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            // значения входов или выходов
            var targets = GetTargets();
            foreach (var target in targets)
            {
                var p = new GraphicsPath();
                var r = target.Item3;
                if (target.Item1) r.Offset(-r.Width * 1.2f, 0f); else r.Offset(r.Width * 1.2f, 0f);
                p.AddString(target.Item1 ? Outputs[target.Item2].Name ?? "" : Inputs[target.Item2].Name ?? "", 
                    fontFunc.FontFamily, (int)FontStyle.Bold, 10f, r, sf);
                paths.Add(p);
            }
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
                var Set = Inputs[0].Value ^ Inputs[0].IsInverted;
                var Reset = Inputs[1].Value ^ Inputs[1].IsInverted;
                var Q = Outputs[0].Value;
                if (Reset)
                {
                    if (Q)
                    {
                        var result = false;
                        result ^= Outputs[0].IsInverted;
                        Outputs[0].Value = result;
                        UpdateLinked(result);
                    }
                }
                else if (Set)
                {
                    if (!Q)
                    {
                        var result = true;
                        result ^= Outputs[0].IsInverted;
                        Outputs[0].Value = result;
                        UpdateLinked(result);
                    }
                }
            }
        }

        public override void Linking(ILink? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesForSet;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesForReset;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
        }

        public override void Unlinking(ILink? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesForSet;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesForReset;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
        }

        public void MakeChangesForSet(object? sender, OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesForReset(object? sender, OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }
    }
}
