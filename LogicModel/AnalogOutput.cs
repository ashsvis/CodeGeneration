using PluginSupport;

namespace LogicModel
{
    public class AnalogOutput : DigitalInput
    {
        public AnalogOutput()
        {
            FuncName = "#/A";
            FuncDesc = "Аналоговый вывод сигнала";
            Inputs = [new FuncInput { }];
            Outputs = [new FuncOutput { }];
            CalculateHeight();
        }
    }
}
