using PluginSupport;

namespace PluginOne
{
    public class AnalogInputTag : DigitalInputTag
    {
        public AnalogInputTag()
        {
            FuncDesc = "Тег аналогового входа";
            FuncName = "TAG_AI";
            Width = 48 * 3;
            Inputs = [];
            Outputs = [new() { }];
            AllowedFuncProperties = AllowedFuncProperties.All ^ 
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowBorder | 
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);
            CalculateHeight();
        }
    }
}
