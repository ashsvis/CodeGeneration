using PluginSupport;

namespace LogicModel
{
    public class AnalogInput : DigitalInput, IFuncPresenter
    {
        public AnalogInput()
        {
            FuncName = "A/#";
            FuncDesc = "Ввод аналогового сигнала";
            Inputs = [new PinInput { Kind = ValueKind.AI }];
            Outputs = [new PinOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }

        public new bool IsPresent => true;
        public new string[] FuncPresenter => ["Преобразует аналоговый сигнал в цифровую переменную контроллера"];

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        public override AnalogInput DeepClone()
        {
            return new AnalogInput()
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
