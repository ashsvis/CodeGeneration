using PluginSupport;

namespace PluginOne
{
    public class Rtrig : Not, ICycle
    {
        private bool lastState;

        public Rtrig()
        {
            FuncName = "Rtrig";
            FuncDesc = "Детектор фронта";
            Inputs = [new() { }];
            Outputs = [new() { }];
            AllowedPinInverted = false;
            CalculateHeight();
        }

        public override void Calculate()
        {
            var result = Inputs[0].Value;
            if (result == false)
                lastState = false;
            if (Outputs[0].Value != result && lastState == false)
            {
                if (result == true && Outputs[0].Value == false)
                {
                    Outputs[0].Value = result;
                    if (lastState == false)
                    {
                        lastState = true;
                        UpdateLinked(true);
                    }
                }
                Outputs[0].Value = false;
            }
        }

        public void SwitchOffSubscibers()
        {
            UpdateLinked(false);
        }
    }
}
