using PluginSupport;

namespace LogicModel
{
    public class AnalogInput : DigitalInput
    {
        public AnalogInput()
        {
            FuncName = "A/#";
            FuncDesc = "Аналоговый ввод сигнала";
            Inputs = [new FuncInput { Kind = ValueKind.AI }];
            Outputs = [new FuncOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }
    }
}
