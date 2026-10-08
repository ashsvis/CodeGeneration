using PluginSupport;

namespace LogicModel
{
    public class AnalogInput : DigitalInput
    {
        public AnalogInput()
        {
            FuncName = "A/#";
            FuncDesc = "Аналоговый ввод сигнала";
            Inputs = [new PinInput { Kind = ValueKind.AI }];
            Outputs = [new PinOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }
    }
}
