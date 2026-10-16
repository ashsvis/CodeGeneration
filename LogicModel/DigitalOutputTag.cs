using PluginSupport;

namespace LogicModel
{
    public class DigitalOutputTag : DigitalInputTag, IAcceptClick
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

        public new bool CanAcceptClick => false;

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
                Outputs = [],
                TagName = TagName,
            };
        }

        public override void Click(bool runEmulate)
        {
            if (!runEmulate)
                Click(ChangeText);
        }

        private void ChangeText(MouseButtons mouseButtons, params object[] args)
        {
            var dlg = new InputValueForm { StrValue = TagName ?? "" };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TagName = dlg.StrValue;
            }
        }
    }
}
