using PluginSupport;

namespace LogicModel
{
    public class DigitalOutput : DigitalInput
    {
        public DigitalOutput()
        {
            FuncName = "#/D";
            FuncDesc = "Дискретный вывод сигнала";
            Inputs = [new FuncInput { Kind = ValueKind.Digital }];
            Outputs = [new FuncOutput { Kind = ValueKind.DO }];
            CalculateHeight();
        }
    }
}
