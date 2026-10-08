using PluginSupport;

namespace LogicModel
{
    public class AnalogOutput : DigitalInput
    {
        public AnalogOutput()
        {
            FuncName = "#/A";
            FuncDesc = "Аналоговый вывод сигнала";
            Inputs = [new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.AO }];
            CalculateHeight();
        }
    }
}
