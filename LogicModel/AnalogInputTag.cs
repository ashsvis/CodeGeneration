using PluginSupport;

namespace LogicModel
{
    public class AnalogInputTag : DigitalInputTag
    {
        public AnalogInputTag()
        {
            FuncDesc = "Тег аналогового входа";
            FuncName = "TAG_AI";
            Inputs = [];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }
    }
}
