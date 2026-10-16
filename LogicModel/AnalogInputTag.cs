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
                Inputs = [],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
            };
        }

        public override void Click(bool runEmulate, int clicks)
        {
            if (runEmulate && clicks == 1)
                Click(ChangeEmulateValue);
        }

        private void ChangeEmulateValue(MouseButtons mouseButtons, params object[] args)
        {
            var result = Outputs[0].Value;
            if (mouseButtons == MouseButtons.Left)
                Outputs[0].Value = result + 1;
            else if (mouseButtons == MouseButtons.Right)
                Outputs[0].Value = result - 1;
            Transfer();
        }
    }
}
