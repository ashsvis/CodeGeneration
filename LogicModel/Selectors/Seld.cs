using PluginSupport;
using System.Drawing.Drawing2D;

namespace LogicModel
{
    public class Seld : Func, IFuncPresenter
    {
        public Seld()
        {
            FuncName = "SEL";
            FuncDesc = "Дискретный мультиплексор";
            Inputs = [new PinInput { Name = "0/1", Kind = ValueKind.Digital }, 
                new PinInput { Name = "0", Kind = ValueKind.Digital }, 
                new PinInput { Name = "1", Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => true;

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                AllowedFuncProperties.PinInverted;

        public override Seld DeepClone()
        {
            return new Seld()
            {
                id = Id,
                Index = Index,
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
                var input0 = Inputs[1].Value > 0;
                var input1 = Inputs[2].Value > 0;
                var result = choose ? input1 : input0;
                if (Outputs.Length > 0 && Outputs[0].Value > 0 != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
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
