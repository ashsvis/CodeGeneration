namespace PluginOne
{
    public class Rs : Cell
    {
        public Rs()
        {
            FuncName = "RS";
            FuncDesc = "RS-ענטדדונ";
            InvertInputs = [false, false];
            InvertOutputs = [false];
            Inputs = [false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var Set = Inputs[0] ^ InvertInputs[0];
                var Reset = Inputs[1] ^ InvertInputs[1];
                var Q = Outputs[0];
                if (Reset)
                    Outputs[0] = false;
                else if (Set)
                    Outputs[0] = true;
                else
                    Outputs[0] = Q;
            }
        }
    }
}
