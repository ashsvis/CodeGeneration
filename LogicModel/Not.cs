using PluginSupport;

namespace LogicModel
{
    public class Not : Func, ILinked
    {
        public Not() 
        {
            FuncName = "1";
            FuncDesc = "Инверсия";
            Inputs = [new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { IsInverted = true }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public virtual void UpdateLinked(bool result)
        {
            OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted ^ Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
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
