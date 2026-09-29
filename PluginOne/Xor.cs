using PluginSupport;

namespace PluginOne
{
    public class Xor : Cell, ILink
    {
        public Xor()
        {
            FuncName = "=1";
            FuncDesc = "Èñêëþ÷àþùåå ÈËÈ";
            InvertInputs = [false, false];
            InvertOutputs = [false];
            Inputs = [false, false];
            Outputs = [false];
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result ^ (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0 && Outputs[0] != result)
                {
                    Outputs[0] = result ^ InvertOutputs[0];
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(Outputs[0]));
                }
            }
        }

        public override void Linking(ILink link, int index)
        {
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesForFirst;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesForSecond;
                    break;
            }
        }

        public void MakeChangesForFirst(object? sender, OutputChangedEventArgs e)
        {
            Inputs[0] = e.NewValue;
            Calculate();
        }

        public void MakeChangesForSecond(object? sender, OutputChangedEventArgs e)
        {
            Inputs[1] = e.NewValue;
            Calculate();
        }
    }
}
