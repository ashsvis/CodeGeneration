using PluginSupport;

namespace LogicModel
{
    public class Sub : Add
    {
        public Sub()
        {
            FuncName = "SUB";
            FuncDesc = "Вычитание";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }

        public override Sub DeepClone()
        {
            return new Sub()
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
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value - Inputs[1].Value;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                }
            }
        }
    }
}
