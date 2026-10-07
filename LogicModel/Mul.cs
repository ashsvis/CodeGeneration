using PluginSupport;

namespace LogicModel
{
    public class Mul : Add
    {
        public Mul()
        {
            FuncName = "MUL";
            FuncDesc = "Умножение";
            Inputs = [new FuncInput { Kind = ValueKind.Analog }, new FuncInput { Kind = ValueKind.Analog }];
            Outputs = [new FuncOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value * Inputs[1].Value;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    UpdateLinked(result);
                }
            }
        }
    }
}
