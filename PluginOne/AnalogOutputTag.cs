using PluginSupport;

namespace PluginOne
{
    public class AnalogOutputTag : DigitalInputTag
    {
        public AnalogOutputTag()
        {
            FuncDesc = "Тег аналогового выхода";
            FuncName = "TAG_AO";
            Width = 48 * 3;
            Inputs = [new() { }];
            Outputs = [];
            AllowedFuncProperties = AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowBorder |
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);
            CalculateHeight();
        }
    }
}
