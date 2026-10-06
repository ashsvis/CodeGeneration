using PluginSupport;

namespace LogicModel
{
    public class AnalogOutput : DigitalInput
    {
        public AnalogOutput()
        {
            FuncName = "#/A";
            FuncDesc = "Аналоговый вывод сигнала";
            Inputs = [new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }
    }
}
