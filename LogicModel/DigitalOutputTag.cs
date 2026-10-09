using PluginSupport;

namespace LogicModel
{
    public class DigitalOutputTag : DigitalInputTag
    {
        public DigitalOutputTag()
        {
            FuncDesc = "Тег дискретного выхода";
            FuncName = "TAG_DO";
            Inputs = [new PinInput { Kind = ValueKind.DO }];
            Outputs = [];
            CalculateHeight();
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowBorder |
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        public override DigitalOutputTag DeepClone()
        {
            return new DigitalOutputTag()
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
