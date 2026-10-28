using PluginSupport;

namespace LogicModel
{
    public class Ftrig : Rtrig, ICycle, IFuncPresenter
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
        public new string[] FuncPresenter => ["Указывает, что данный логический вход должен срабатывать в течение одного такта контроллера при переходе из состояния логического \"1\" в \"0\""];

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
    }
}
