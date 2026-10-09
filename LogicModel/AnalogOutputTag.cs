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

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowBorder |
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        public override AnalogOutputTag DeepClone()
        {
            return new AnalogOutputTag()
            {
                id = Id,
                Index = this.Index,
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
            };
        }
    }
}
