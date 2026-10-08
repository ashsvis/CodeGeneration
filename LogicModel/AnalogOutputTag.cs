using PluginSupport;

namespace LogicModel
{
    public class AnalogOutputTag : DigitalInputTag
    {
        public AnalogOutputTag()
        {
            FuncDesc = "Тег аналогового выхода";
            FuncName = "TAG_AO";
            Inputs = [new PinInput { Kind = ValueKind.AO }];
            Outputs = [];
            CalculateHeight();
        }
    }
}
