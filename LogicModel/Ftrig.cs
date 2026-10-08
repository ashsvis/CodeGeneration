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
            Inputs = [new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override Ftrig DeepClone()
        {
            return new Ftrig()
            {
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
                    if (lastState == false)
                    {
                        lastState = true;
                    }
                }
                Outputs[0].Value = 0;
                base.Calculate();
            }
        }
    }
}
