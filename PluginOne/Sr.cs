using System.Drawing.Drawing2D;

namespace PluginOne
{
    public class Sr : Cell
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

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var Set = Inputs[0] ^ InvertInputs[0];
                var Reset = Inputs[1] ^ InvertInputs[1];
                var Q = Outputs[0];
                if (Set)
                    Outputs[0] = true;
                else if (Reset)
                    Outputs[0] = false;
                else
                    Outputs[0] = Q;
            }
        }
    }
}
