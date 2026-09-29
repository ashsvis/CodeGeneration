using PluginSupport;

namespace PluginOne
{
    public class Not : Cell, ILink
    {
        public Not() 
        {
            FuncName = "1";
            FuncDesc = "Инверсия";
            InvertInputs = [false];
            InvertOutputs = [true];
            Inputs = [false];
            Outputs = [false];
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                if (Outputs.Length > 0 && Outputs[0] != result)
                {
                    Outputs[0] = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(Outputs[0] ^ InvertOutputs[0]));
                }
            }
        }

        public override void Linking(ILink link, int index)
        {
            link.OnOutputChange += MakeChanges;
        }

        public void MakeChanges(object? sender, OutputChangedEventArgs e)
        {
            Inputs[0] = e.NewValue;
        }
    }
}
