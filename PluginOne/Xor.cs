using PluginSupport;

namespace PluginOne
{
    public class Xor : Cell, ILink
    {
        public Xor()
        {
            FuncName = "=1";
            FuncDesc = "Èñêëþ÷àþùåå ÈËÈ";
            Inputs = [new() { }, new() { }];
            Outputs = [new() { }];
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result ^ (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
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
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesForSecond(object? sender, OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }
    }
}
