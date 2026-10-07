using PluginSupport;

namespace LogicModel
{
    public class Not : Func, ILinked
    {
        public Not() 
        {
            FuncName = "1";
            FuncDesc = "Инверсия";
            Inputs = [new FuncInput { }];
            Outputs = [new FuncOutput { IsInverted = true }];
            CalculateHeight();
        }

        public override Not DeepClone()
        {
            return new Not()
            {
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
            };
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted ^ Outputs[0].IsInverted;
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
            link.OnOutputChange += MakeChanges;
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
        }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            link.OnOutputChange -= MakeChanges;
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
        }

        public void MakeChanges(object? sender, OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }
    }
}
