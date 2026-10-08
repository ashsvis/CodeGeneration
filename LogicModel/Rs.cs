using PluginSupport;
using System.Drawing.Drawing2D;

namespace LogicModel
{
    public class Rs : Func
    {
        public Rs()
        {
            FuncName = "RS";
            FuncDesc = "RS-триггер";
            Inputs = [new PinInput { Name = "S" }, new PinInput { Name = "R" }];
            Outputs = [new PinOutput { Name = "Q" }];
            CalculateHeight();
        }

        public override Rs DeepClone()
        {
            return new Rs()
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

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted);

        public override GraphicsPath[] GetTextPaths()
        {
            List<GraphicsPath> paths = [..base.GetTextPaths()];
            using var fontFunc = new Font("Segoe UI", 12f);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            // значения входов или выходов
            var targets = GetTargets();
            foreach (var info in targets)
            {
                var p = new GraphicsPath();
                var t = info.Target;
                if (info.IsOutput) t.Offset(-t.Width * 1.2f, 0f); else t.Offset(t.Width * 1.2f, 0f);
                p.AddString(info.IsOutput ? Outputs[info.PinIndex].Name ?? "" : Inputs[info.PinIndex].Name ?? "", 
                    fontFunc.FontFamily, (int)FontStyle.Bold, 10f, t, sf);
                paths.Add(p);
            }
            return [.. paths];
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var Set = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                var Reset = (Inputs[1].Value > 0) ^ Inputs[1].IsInverted;
                var Q = Outputs[0].Value;
                if (Reset)
                {
                    if (Q == 1)
                    {
                        var result = false;
                        result ^= Outputs[0].IsInverted;
                        Outputs[0].Value = result ? 1 : 0;
                        base.Calculate();
                    }
                }
                else if (Set)
                {
                    if (Q == 0)
                    {
                        var result = true;
                        result ^= Outputs[0].IsInverted;
                        Outputs[0].Value = result ? 1 : 0;
                        base.Calculate();
                    }
                }
            }
        }
    }
}
