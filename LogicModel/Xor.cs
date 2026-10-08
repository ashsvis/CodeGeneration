using PluginSupport;

namespace LogicModel
{
    public class Xor : Func
    {
        public Xor()
        {
            FuncName = "=1";
            FuncDesc = "Èñêëþ÷àþùåå ÈËÈ";
            Inputs = [new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override Xor DeepClone()
        {
            return new Xor()
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
            if (Inputs.Length > 0)
            {
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result ^= ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }
    }
}
