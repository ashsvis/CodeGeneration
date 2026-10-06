using PluginSupport;

namespace LogicModel
{
    public class DigitalOutputTag : DigitalInputTag
    {
        public DigitalOutputTag()
        {
            FuncDesc = "Тег дискретного выхода";
            FuncName = "TAG_DO";
            Inputs = [new() { }];
            Outputs = [];
            CalculateHeight();
        }
    }
}
