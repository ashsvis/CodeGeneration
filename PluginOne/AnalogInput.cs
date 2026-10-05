namespace PluginOne
{
    public class AnalogInput : DigitalInput
    {
        public AnalogInput()
        {
            FuncName = "A/#";
            FuncDesc = "Аналоговый ввод сигнала";
            Inputs = [new() { }];
            Outputs = [new() { }];
            AllowedPinInverted = false;
            CalculateHeight();
        }
    }
}
