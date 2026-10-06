using PluginSupport;

namespace LogicModel
{
    public class Ftrig : Rtrig, ICycle
    {
        private bool lastState;

        public Ftrig()
        {
            FuncName = "Ftrig";
            FuncDesc = "Детектор спада";
            Inputs = [new FuncInput { }];
            Outputs = [new FuncOutput { }];
            CalculateHeight();
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
                    if (lastState == false)
                    {
                        lastState = true;
                        UpdateLinked(1);
                    }
                }
                Outputs[0].Value = 0;
            }
        }
    }
}
