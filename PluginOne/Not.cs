namespace PluginOne
{
    public class Not : Cell
    {
        public Not() 
        {
            FuncName = "1";
            FuncDesc = "Инверсия";
            Inputs = [false];
            Outputs = [true];
        }
    }
}
