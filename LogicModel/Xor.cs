using PluginSupport;

namespace LogicModel
{
    public class Xor : Func, ILinked
    {
        public Xor()
        {
            FuncName = "=1";
            FuncDesc = "Èñêëþ÷àþùåå ÈËÈ";
            Inputs = [new FuncInput { }, new FuncInput { }];
            Outputs = [new FuncOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

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
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result ? 1 : 0));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesForFirst;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesForSecond;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
        }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesForFirst;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesForSecond;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
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
