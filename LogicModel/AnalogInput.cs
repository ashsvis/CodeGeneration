using PluginSupport;

namespace LogicModel
{
    public class AnalogInput : DigitalInput
    {
        public AnalogInput()
        {
            FuncName = "A/#";
            FuncDesc = "Аналоговый ввод сигнала";
            Inputs = [new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }
    }
}
