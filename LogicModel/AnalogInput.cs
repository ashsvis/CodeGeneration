using PluginSupport;

namespace LogicModel
{
    public class AnalogInput : DigitalInput
    {
        public AnalogInput()
        {
            FuncName = "A/#";
            FuncDesc = "Аналоговый ввод сигнала";
            Inputs = [new FuncInput { }];
            Outputs = [new FuncOutput { }];
            CalculateHeight();
        }
    }
}
