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
            Outputs = [new PinOutput { Kind = ValueKind.AI }];
            CalculateHeight();
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowBorder |
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        public override AnalogInputTag DeepClone()
        {
            return new AnalogInputTag()
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
