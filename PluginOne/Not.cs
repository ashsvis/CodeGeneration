using PluginSupport;

namespace PluginOne
{
    public class Not : Cell, ILink
    {
        public Not() 
        {
            FuncName = "1";
            FuncDesc = "Инверсия";
            Inputs = [new() { }];
            Outputs = [new() { IsInverted = true }];
        }

        public event OutputChangedEventHandler? OnOutputChange;

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

        public override void Linking(ILink link, int index)
        {
            link.OnOutputChange += MakeChanges;
        }

        public void MakeChanges(object? sender, OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }
    }
}
