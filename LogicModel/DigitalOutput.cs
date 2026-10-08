using PluginSupport;

namespace LogicModel
{
    public class DigitalOutput : DigitalInput
    {
        public DigitalOutput()
        {
            FuncName = "#/D";
            FuncDesc = "Дискретный вывод сигнала";
            Inputs = [new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.DO }];
            CalculateHeight();
        }
    }
}
