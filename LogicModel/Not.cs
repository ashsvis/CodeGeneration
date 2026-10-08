using PluginSupport;
using System;

namespace LogicModel
{
    public class Not : Func
    {
        public Not() 
        {
            FuncName = "1";
            FuncDesc = "Инверсия";
            Inputs = [new PinInput { }];
            Outputs = [new PinOutput { IsInverted = true }];
            CalculateHeight();
        }

        public override Not DeepClone()
        {
            return new Not()
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

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted ^ Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }
    }
}
