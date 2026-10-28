using PluginSupport;
using System.Drawing.Drawing2D;

namespace LogicModel
{
    public class AlarmDetector : Func, IFuncPresenter
    {
        public AlarmDetector()
        {
            FuncName = "<>";
            FuncDesc = "Блок сигнализации и выделения уставок";
            Inputs = [new PinInput { Kind = ValueKind.Analog }];
            Outputs = [
                new PinOutput { Name = "HH", Kind = ValueKind.Digital },
                new PinOutput { Name = "H", Kind = ValueKind.Digital },
                new PinOutput { Name = "L", Kind = ValueKind.Digital },
                new PinOutput { Name = "LL", Kind = ValueKind.Digital },
            ];
            CalculateHeight();
        }

        public bool IsPresent => true;
        public string[] FuncPresenter => 
            ["Обеспечивает вырабатывание световой и звуковой сигнализации на станции оператора при достижении любого из пороговых значений." +
            " Соответствующий логический выход также принимает значение \"1\" при достижении порогового значения."];

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                AllowedFuncProperties.PinInverted;

        public override AlarmDetector DeepClone()
        {
            return new AlarmDetector()
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
            //if (Inputs.Length > 0)
            //{
            //    var choose = Inputs[0].Value > 0;
            //    var input0 = Inputs[1].Value > 0;
            //    var input1 = Inputs[2].Value > 0;
            //    var result = choose ? input1 : input0;
            //    if (Outputs.Length > 0 && Outputs[0].Value > 0 != result)
            //    {
            //        Outputs[0].Value = result ? 1 : 0;
            //        Transfer();
            //    }
            //}
        }

        public override GraphicsPath[] GetTextPaths()
        {
            List<GraphicsPath> paths = [.. base.GetTextPaths()];
            using var fontFunc = new Font("Segoe UI", 12f);
            using var sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            // значения входов или выходов
            var targets = GetTargets();
            foreach (var info in targets)
            {
                var p = new GraphicsPath();
                var t = info.Target;
                if (info.IsOutput) 
                {
                    sf.Alignment = StringAlignment.Far;
                    t.Offset(-t.Width * 1.2f, 0f); 
                }
                else 
                {
                    sf.Alignment = StringAlignment.Near;
                    t.Offset(t.Width * 1.2f, 0f); 
                }
                
                p.AddString(info.IsOutput ? Outputs[info.PinIndex].Name ?? "" : Inputs[info.PinIndex].Name ?? "",
                    fontFunc.FontFamily, (int)FontStyle.Bold, 10f, new PointF(info.IsOutput ? t.Right : t.Left, t.Top + t.Height / 2), sf);
                paths.Add(p);
            }
            return [.. paths];
        }
    }
}
