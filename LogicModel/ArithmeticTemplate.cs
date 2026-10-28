using PluginSupport;

namespace LogicModel
{
    public partial class PluginLogic
    {
        static partial void AddTreeNodeArithmeticItems(TreeNodeCollection items)
        {
            items.Add(new TreeNode("Блок сложения") { Tag = typeof(Add) });
            items.Add(new TreeNode("Блок вычитания") { Tag = typeof(Sub) });
            items.Add(new TreeNode("Блок умножения") { Tag = typeof(Mul) });
            items.Add(new TreeNode("Блок деления") { Tag = typeof(Div) });
        }
    }

    public class Add : Func, IFuncPresenter
    {
        public Add()
        {
            FuncName = "ADD";
            FuncDesc = "Блок сложения";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }

        public bool IsPresent => true;
        public string[] FuncPresenter => [""];

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

    public class Sub : Func, IFuncPresenter
    {
        public Sub()
        {
            FuncName = "SUB";
            FuncDesc = "Блок вычитания";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }

        public bool IsPresent => true;
        public string[] FuncPresenter => [""];

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

    public class Mul : Func, IFuncPresenter
    {
        public Mul()
        {
            FuncName = "MUL";
            FuncDesc = "Блок умножения";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }

        public bool IsPresent => true;
        public string[] FuncPresenter => [""];

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

    public class Div : Func, IFuncPresenter
    {
        public Div()
        {
            FuncName = "DIV";
            FuncDesc = "Блок деления";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }

        public bool IsPresent => true;
        public string[] FuncPresenter => [""];

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