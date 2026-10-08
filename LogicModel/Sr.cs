namespace LogicModel
{
    public class Sr : Rs
    {
        public Sr()
        {
            FuncName = "SR";
            FuncDesc = "SR-ענטדדונ";
            Inputs = [new PinInput { Name = "S" }, new PinInput { Name = "R" }];
            Outputs = [new PinOutput { Name = "Q" }];
            CalculateHeight();
        }

        public override Sr DeepClone()
        {
            return new Sr()
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
                var Set = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                var Reset = (Inputs[1].Value > 0) ^ Inputs[1].IsInverted;
                var Q = Outputs[0].Value;
                if (Set)
                {
                    if (Q == 0)
                    {
                        var result = true;
                        result ^= Outputs[0].IsInverted;
                        Outputs[0].Value = result ? 1 : 0;
                    }
                }
                else if (Reset)
                {
                    if (Q == 1)
                    {
                        var result = false;
                        result ^= Outputs[0].IsInverted;
                        Outputs[0].Value = result ? 1 : 0;
                    }
                }
            }
        }
    }
}
