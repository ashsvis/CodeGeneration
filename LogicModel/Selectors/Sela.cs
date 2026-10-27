using PluginSupport;
using System.Drawing.Drawing2D;

namespace LogicModel
{
    public class Sela : Seld, IFuncPresenter
    {
        public Sela()
        {
            FuncName = "SEL";
            FuncDesc = "Аналоговый мультиплексор";
            Inputs = [new PinInput { Name = "A/B", Kind = ValueKind.Digital }, 
                new PinInput { Name = "A", Kind = ValueKind.Analog }, 
                new PinInput { Name = "B", Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }

        public new bool IsPresent => false;

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted);

        public override Sela DeepClone()
        {
            return new Sela()
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

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var choose = Inputs[0].Value > 0;
                var input0 = Inputs[1].Value;
                var input1 = Inputs[2].Value;
                var result = choose ? input1 : input0;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    Transfer();
                }
            }
        }

        public override GraphicsPath[] GetTextPaths()
        {
            List<GraphicsPath> paths = [.. base.GetTextPaths()];
            using var fontFunc = new Font("Segoe UI", 12f);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            // значения входов или выходов
            var targets = GetTargets();
            foreach (var info in targets)
            {
                var p = new GraphicsPath();
                var t = info.Target;
                if (info.IsOutput) t.Offset(-t.Width * 1.2f, 0f); else t.Offset(t.Width * 1.2f, 0f);
                p.AddString(info.IsOutput ? Outputs[info.PinIndex].Name ?? "" : Inputs[info.PinIndex].Name ?? "",
                    fontFunc.FontFamily, (int)FontStyle.Bold, 10f, new PointF(t.Left, t.Top + t.Height / 2), sf);
                paths.Add(p);
            }
            return [.. paths];
        }
    }
}
