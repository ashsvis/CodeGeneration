using PluginSupport;

namespace LogicModel
{
    public class Dt : Rs, IFuncPresenter
    {
        private bool strob = false;

        public Dt()
        {
            FuncName = "T";
            FuncDesc = "D-триггер";
            Inputs = [new PinInput { Name = "D", Kind = ValueKind.Digital }, new PinInput { Name = "C", Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Name = "Q", Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public new bool IsPresent => false;
        public new string[] FuncPresenter => 
            ["D - вход данных", "C - запись по фронту 0 -> 1", "Значение D фиксируется строго в момент фронта такта"];

        public override Dt DeepClone()
        {
            return new Dt()
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
                var Data = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                var Strob = (Inputs[1].Value > 0) ^ Inputs[1].IsInverted;
                if (Strob && !strob)
                {
                    strob = true;
                    var result = Data;
                    result ^= Outputs[0].IsInverted;
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
                else if (!Strob && strob)
                    strob = false;
            }
        }
    }
}
