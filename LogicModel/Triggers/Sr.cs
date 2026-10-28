using PluginSupport;

namespace LogicModel
{
    public class Sr : Rs, IFuncPresenter
    {
        public Sr()
        {
            FuncName = "SR";
            FuncDesc = "SR-триггер";
            Inputs = [new PinInput { Name = "S", Kind = ValueKind.Digital }, new PinInput { Name = "R", Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Name = "Q", Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public new bool IsPresent => true;
        public new string[] FuncPresenter => ["S - установка", "R - сброс", "Приоритетом обладает вход триггера S"];

        public override Sr DeepClone()
        {
            return new Sr()
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

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var Set = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                var Reset = (Inputs[1].Value > 0) ^ Inputs[1].IsInverted;
                var Q = Outputs[0].Value;
                if (Set)
                {
                    if (Q == 0)
                    {
                        var result = true;
                        result ^= Outputs[0].IsInverted;
                        Outputs[0].Value = result ? 1 : 0;
                        Transfer();
                    }
                }
                else if (Reset)
                {
                    if (Q == 1)
                    {
                        var result = false;
                        result ^= Outputs[0].IsInverted;
                        Outputs[0].Value = result ? 1 : 0;
                        Transfer();
                    }
                }
            }
        }
    }
}
