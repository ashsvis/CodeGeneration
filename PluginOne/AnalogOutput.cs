using PluginSupport;

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
            AllowedFuncProperties = AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);
            CalculateHeight();
        }
    }
}
