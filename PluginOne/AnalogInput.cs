using PluginSupport;

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
            AllowedFuncProperties = AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);
            CalculateHeight();
        }
    }
}
