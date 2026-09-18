namespace PluginOne
{
    public class Not : Cell
    {
        public Not() 
        {
            FuncName = "1";
            FuncDesc = "Инверсия";
            InvertInputs = [false];
            InvertOutputs = [true];
            Inputs = [false];
            Outputs = [true];
        }
    }
}
