using PluginSupport;

namespace LogicModel
{
    public class DigitalOutput : DigitalInput
    {
        public DigitalOutput()
        {
            FuncName = "#/D";
            FuncDesc = "Дискретный вывод сигнала";
            Inputs = [new FuncInput { }];
            Outputs = [new FuncOutput { }];
            //AllowedFuncProperties = AllowedFuncProperties.All ^
            //    (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);
            CalculateHeight();
        }
    }
}
