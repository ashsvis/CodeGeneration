using PluginSupport;

namespace LogicModel
{
    public class Sela : Seld
    {
        public Sela()
        {
            FuncName = "SEL";
            FuncDesc = "Выбор сигнала";
            Inputs = [new PinInput { Name = "0/1" }, 
                new PinInput { Name = "0", Kind = ValueKind.Analog }, 
                new PinInput { Name = "1", Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }

        public override Sela DeepClone()
        {
            return new Sela()
            {
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
            };
        }

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
