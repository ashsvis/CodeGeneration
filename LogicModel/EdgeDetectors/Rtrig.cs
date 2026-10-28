using PluginSupport;

namespace LogicModel
{
    public class Rtrig : Not, ICycle, IFuncPresenter
    {
        private bool lastState;

        public Rtrig()
        {
            FuncName = "Rtrig";
            FuncDesc = "ѕризнак срабатывани€ по фронту";
            Inputs = [new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public new bool IsPresent => true;
        public new string[] FuncPresenter => ["”казывает, что данный логический вход должен срабатывать в течение одного такта контроллера при переходе из состо€ни€ логического \"0\" в \"1\""];

        public override Rtrig DeepClone()
        {
            return new Rtrig()
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

        public override void Calculate()
        {
            var result = Inputs[0].Value;
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
    }
}
