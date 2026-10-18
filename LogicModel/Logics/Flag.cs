using PluginSupport;

namespace LogicModel
{
    public class Flag : Func, IAcceptClick
    {
        public bool Value { get; set; }

        public Flag()
        {
            FuncName = "FL";
            FuncDesc = "Флаг";
            Inputs = [new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public override Flag DeepClone()
        {
            return new Flag()
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

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted);

        public bool CanAcceptClick => true;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    Transfer();
                }
            }
        }

        public override void Click(bool runEmulate)
        {
            if (runEmulate)
                Click(ChangeEmulateValue);
            else
                Click(ChangeValue);
        }

        private void ChangeEmulateValue(MouseButtons mouseButtons, params object[] args)
        {
            var result = Outputs[0].Value;
            Outputs[0].Value = result > 0 ? 0 : 1;
            Transfer();
        }

        private void ChangeValue(MouseButtons mouseButtons, params object[] args)
        {
            Value = !Value;
        }
    }
}