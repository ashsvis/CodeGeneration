using PluginSupport;

namespace LogicModel
{
    public class AnalogInputTag : DigitalInputTag, IAcceptClick
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

        public new bool CanAcceptClick => true;

        public override AnalogInputTag DeepClone()
        {
            return new AnalogInputTag()
            {
                id = Id,
                Index = Index,
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
                TagName = TagName,
            };
        }

        public override void Click(bool runEmulate)
        {
            if (runEmulate)
                Click(ChangeEmulateValue);
            else
                Click(ChangeText);
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

        private void ChangeText(MouseButtons mouseButtons, params object[] args)
        {
            var dlg = new InputValueForm { StrValue = TagName ?? "" };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TagName = dlg.StrValue;
            }
        }
       
        public override void DrawWhenDebug(Graphics? g, Brush defaultBrush)
        {
            // stub
        }
    }
}
