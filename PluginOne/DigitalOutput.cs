namespace PluginOne
{
    public class DigitalOutput : DigitalInput
    {
        public DigitalOutput()
        {
            FuncName = "#/D";
            FuncDesc = "Вывод дискретного сигнала";
            Inputs = [new() { }];
            Outputs = [new() { }];
            AllowedPinInverted = false;
            CalculateHeight();
        }
    }
}
