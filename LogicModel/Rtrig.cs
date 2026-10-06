using PluginSupport;

namespace LogicModel
{
    public class Rtrig : Not, ICycle
    {
        private bool lastState;

        public Rtrig()
        {
            FuncName = "Rtrig";
            FuncDesc = "Детектор фронта";
            Inputs = [new FuncInput { }];
            Outputs = [new FuncOutput { }];
            CalculateHeight();
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
                    if (lastState == false)
                    {
                        lastState = true;
                        UpdateLinked(1);
                    }
                }
                Outputs[0].Value = 0;
            }
        }

        public void SwitchOffSubscibers()
        {
            UpdateLinked(0);
        }
    }
}
