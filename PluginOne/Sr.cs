using PluginSupport;
using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class Sr : Cell, ILink
    {
        public Sr()
        {
            FuncName = "SR";
            FuncDesc = "SR-триггер";
            InvertInputs = [false, false];
            InvertOutputs = [false];
            Inputs = [false, false];
            InputNames = ["S", "R"];
            Outputs = [false];
            OutputNames = ["Q"];
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override GraphicsPath[] GetTextPaths()
        {
            List<GraphicsPath> paths = [.. base.GetTextPaths()];
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
                if (target.Item1) r.Offset(-r.Width, 0f); else r.Offset(r.Width, 0f);
                p.AddString(target.Item1 ? OutputNames[target.Item2] : InputNames[target.Item2],
                    fontFunc.FontFamily, (int)FontStyle.Bold, 10f, r, sf);
                paths.Add(p);
            }
            return [.. paths];
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var Set = Inputs[0] ^ InvertInputs[0];
                var Reset = Inputs[1] ^ InvertInputs[1];
                var Q = Outputs[0];
                if (Set)
                {
                    if (Outputs[0])
                    {
                        var result = true ^ InvertOutputs[0];
                        result ^= InvertOutputs[0];
                        Outputs[0] = result;
                        OnOutputChange?.Invoke(this, new OutputChangedEventArgs(Outputs[0]));
                    }
                }
                else if (Reset)
                {
                    if (!Outputs[0])
                    {
                        var result = false ^ InvertOutputs[0];
                        result ^= InvertOutputs[0];
                        Outputs[0] = result;
                        OnOutputChange?.Invoke(this, new OutputChangedEventArgs(Outputs[0]));
                    }
                }
                else
                {
                    if (Outputs[0] != Q)
                    {
                        var result = Q ^ InvertOutputs[0];
                        result ^= InvertOutputs[0];
                        Outputs[0] = result;
                        OnOutputChange?.Invoke(this, new OutputChangedEventArgs(Outputs[0]));
                    }
                }
            }
        }

        public override void Linking(ILink link, int index)
        {
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesForSet;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesForReset;
                    break;
            }
        }

        public void MakeChangesForSet(object? sender, OutputChangedEventArgs e)
        {
            Inputs[0] = e.NewValue;
        }

        public void MakeChangesForReset(object? sender, OutputChangedEventArgs e)
        {
            Inputs[1] = e.NewValue;
        }
    }
}
