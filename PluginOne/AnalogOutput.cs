namespace PluginOne
{
    public class AnalogOutput : DigitalInput
    {
        public AnalogOutput()
        {
            FuncName = "#/A";
            FuncDesc = "Аналоговый вывод сигнала";
            Inputs = [new() { }];
            Outputs = [new() { }];
            AllowedPinInverted = false;
            CalculateHeight();
        }
    }
}
