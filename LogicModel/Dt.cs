namespace LogicModel
{
    public class Dt : Rs
    {
        public Dt()
        {
            FuncName = "T";
            FuncDesc = "D-ענטדדונ";
            Inputs = [new PinInput { Name = "D" }, new PinInput { Name = "C" }];
            Outputs = [new PinOutput { Name = "Q" }];
            CalculateHeight();
        }

        public override Dt DeepClone()
        {
            return new Dt()
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
                var Data = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                var Strob = (Inputs[1].Value > 0) ^ Inputs[1].IsInverted;
                if (Strob)
                {
                    var result = Data;
                    result ^= Outputs[0].IsInverted;
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }
    }
}
