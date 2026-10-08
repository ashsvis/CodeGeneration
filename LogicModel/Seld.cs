using PluginSupport;

namespace LogicModel
{
    public class Seld : Func
    {
        public Seld()
        {
            FuncName = "SEL";
            FuncDesc = "Выбор сигнала";
            Inputs = [new PinInput { Name = "0/1" }, new PinInput { Name = "0" }, new PinInput { Name = "1" }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override Seld DeepClone()
        {
            return new Seld()
            {
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
            };
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted);

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var choose = Inputs[0].Value > 0;
                var input0 = Inputs[1].Value > 0;
                var input1 = Inputs[2].Value > 0;
                var result = choose ? input1 : input0;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                }
            }
        }
    }
}
