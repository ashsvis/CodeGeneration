using PluginSupport;
using System.Drawing.Drawing2D;

namespace LogicModel
{
    public class Ftrig : Not, ICycle, IFuncPresenter
    {
        private bool lastState;

        public Ftrig()
        {
            FuncName = "Ftrig";
            FuncDesc = "Признак срабатывания по спаду";
            Inputs = [new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public new bool IsPresent => true;
        public new string[] FuncPresenter => ["Указывает, что данный логический вход должен срабатывать" +
            " в течение одного такта контроллера при переходе из состояния логического \"1\" в \"0\""];

        public override Ftrig DeepClone()
        {
            return new Ftrig()
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
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowFuncName);

        public override void Calculate()
        {
            var result = Inputs[0].Value > 0 ? 0 : 1;
            if (result == 0)
                lastState = false;
            if (Outputs[0].Value != result && lastState == false)
            {
                if (result == 1 && Outputs[0].Value == 0)
                {
                    Outputs[0].Value = result;
                    Transfer();
                    if (lastState == false)
                        lastState = true;
                }
            }
        }

        public void SwitchOffSubscibers()
        {
            Outputs[0].Value = 0;
            Transfer();
        }

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var paths = base.GetGraphicsPaths().ToList();
            var rect = Bounds;
            rect.Height /= 3;
            rect.Height *= 2;
            rect.Inflate(-12, -6);
            var path = new GraphicsPath();
            path.AddLines(new Point(rect.Left, rect.Top), new Point(rect.Left + rect.Width / 2, rect.Top),
                new Point(rect.Left + rect.Width / 2, rect.Bottom), new Point(rect.Right, rect.Bottom));
            paths.Add(path);
            return [.. paths];
        }
    }
}
