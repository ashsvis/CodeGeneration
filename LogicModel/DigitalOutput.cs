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

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        public override DigitalOutput DeepClone()
        {
            return new DigitalOutput()
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
