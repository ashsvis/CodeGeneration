using PluginSupport;

namespace LogicModel
{
    public class AnalogOutputTag : DigitalInputTag, IAcceptClick
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
        public new bool CanAcceptClick => false;

        public override AnalogOutputTag DeepClone()
        {
            return new AnalogOutputTag()
            {
                id = Id,
                Index = Index,
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
