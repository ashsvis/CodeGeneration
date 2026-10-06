namespace LogicModel
{
    public class Sr : Rs
    {
        public Sr()
        {
            FuncName = "SR";
            FuncDesc = "SR-ענטדדונ";
            Inputs = [new FuncInput { Name = "S" }, new FuncInput { Name = "R" }];
            Outputs = [new FuncOutput { Name = "Q" }];
            CalculateHeight();
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
                        UpdateLinked(result ? 1 : 0);
                    }
                }
                else if (Reset)
                {
                    if (Q == 1)
                    {
                        var result = false;
                        result ^= Outputs[0].IsInverted;
                        Outputs[0].Value = result ? 1 : 0;
                        UpdateLinked(result ? 1 : 0);
                    }
                }
            }
        }
    }
}
