using PluginSupport;

namespace LogicModel
{
    public partial class PluginLogic
    {
        static partial void AddTreeNodeArithmeticItems(TreeNodeCollection items)
        {
            items.Add(new TreeNode("Сложение") { Tag = typeof(Add) });
            items.Add(new TreeNode("Вычитание") { Tag = typeof(Sub) });
            items.Add(new TreeNode("Умножение") { Tag = typeof(Mul) });
            items.Add(new TreeNode("Деление") { Tag = typeof(Div) });
        }
    }

    public class Add : Func
    {
        public Add()
        {
            FuncName = "ADD";
            FuncDesc = "Сложение";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public override Add DeepClone()
        {
            return new Add()
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
                var result = Inputs[0].Value + Inputs[1].Value;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    Transfer();
                }
            }
        }
    }

    public class Sub : Func
    {
        public Sub()
        {
            FuncName = "SUB";
            FuncDesc = "Вычитание";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public override Sub DeepClone()
        {
            return new Sub()
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
                var result = Inputs[0].Value - Inputs[1].Value;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    Transfer();
                }
            }
        }
    }

    public class Mul : Func
    {
        public Mul()
        {
            FuncName = "MUL";
            FuncDesc = "Умножение";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public override Mul DeepClone()
        {
            return new Mul()
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
                var result = Inputs[0].Value * Inputs[1].Value;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    Transfer();
                }
            }
        }
    }

    public class Div : Func
    {
        public Div()
        {
            FuncName = "DIV";
            FuncDesc = "Деление";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public override Div DeepClone()
        {
            return new Div()
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
                var result = Inputs[0].Value / Inputs[1].Value;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    Transfer();
                }
            }
        }
    }

}