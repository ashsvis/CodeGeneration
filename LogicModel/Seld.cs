using PluginSupport;

namespace LogicModel
{
    public class Seld : Func, ILinked
    {
        public Seld()
        {
            FuncName = "SEL";
            FuncDesc = "Выбор сигнала";
            Inputs = [new FuncInput { Name = "0/1" }, new FuncInput { Name = "0" }, new FuncInput { Name = "1" }];
            Outputs = [new FuncOutput { }];
            CalculateHeight();
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted);

        public event OutputChangedEventHandler? OnOutputChange;

        public virtual void UpdateLinked(double result)
        {
            OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var choose = Inputs[0].Value > 0;
                var input0 = Inputs[1].Value > 0;
                var input1 = Inputs[2].Value > 0;
                var result = choose ? input1 : input0;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    UpdateLinked(result ? 1 : 0);
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
