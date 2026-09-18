namespace PluginOne
{
    public class Xor : Cell
    {
        public Xor()
        {
            FuncName = "=1";
            FuncDesc = "Èñêëş÷àşùåå ÈËÈ";
            InvertInputs = [false, false];
            InvertOutputs = [false];
            Inputs = [false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result ^ (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }
}
