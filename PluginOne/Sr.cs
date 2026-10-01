namespace PluginOne
{
    public class Sr : Rs
    {
        public Sr()
        {
            FuncName = "SR";
            FuncDesc = "SR-ענטדדונ";
            Inputs = [new() { Name = "S" }, new() { Name = "R" }];
            Outputs = [new() { Name = "Q" }];
            CalculateHeight();
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var Set = Inputs[0].Value ^ Inputs[0].IsInverted;
                var Reset = Inputs[1].Value ^ Inputs[1].IsInverted;
                var Q = Outputs[0].Value;
                if (Set)
                {
                    if (!Q)
                    {
                        var result = true;
                        result ^= Outputs[0].IsInverted;
                        Outputs[0].Value = result;
                        UpdateLinked(result);
                    }
                }
                else if (Reset)
                {
                    if (Q)
                    {
                        var result = false;
                        result ^= Outputs[0].IsInverted;
                        Outputs[0].Value = result;
                        UpdateLinked(result);
                    }
                }
            }
        }
    }
}
