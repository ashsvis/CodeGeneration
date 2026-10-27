using PluginSupport;

namespace LogicModel
{
    public partial class PluginLogic
    {
        static partial void AddTreeNodeCompareItems(TreeNodeCollection items)
        {
            items.Add(new TreeNode("Элемент \"Сравнение\"") { Tag = typeof(Eq) });
            items.Add(new TreeNode("Элемент \"Не равно\"") { Tag = typeof(Ne) });
            items.Add(new TreeNode("Элемент \"Больше\"") { Tag = typeof(Gt) });
            items.Add(new TreeNode("Элемент \"Больше или равно\"") { Tag = typeof(Ge) });
            items.Add(new TreeNode("Элемент \"Меньше\"") { Tag = typeof(Lt) });
            items.Add(new TreeNode("Элемент \"Меньше или равно\"") { Tag = typeof(Le) });
        }
    }

    public class Eq : Func, IFuncPresenter
    {
        public Eq()
        {
            FuncName = "==";
            FuncDesc = "Элемент \"Сравнение\"";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => true;

        public override Eq DeepClone()
        {
            return new Eq()
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
                var result = Math.Abs(Inputs[0].Value - Inputs[1].Value) < double.Epsilon;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }
    }

    public class Ne : Func, IFuncPresenter
    {
        public Ne()
        {
            FuncName = "!=";
            FuncDesc = "Элемент \"Не равно\"";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override Ne DeepClone()
        {
            return new Ne()
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
                var result = !(Math.Abs(Inputs[0].Value - Inputs[1].Value) < double.Epsilon);
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }
    }

    public class Gt : Func, IFuncPresenter
    {
        public Gt()
        {
            FuncName = ">";
            FuncDesc = "Элемент \"Больше\"";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override Gt DeepClone()
        {
            return new Gt()
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
                var result = Inputs[0].Value > Inputs[1].Value;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }
    }

    public class Ge : Func, IFuncPresenter
    {
        public Ge()
        {
            FuncName = ">=";
            FuncDesc = "Элемент \"Больше или равно\"";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override Ge DeepClone()
        {
            return new Ge()
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
                var result = Inputs[0].Value >= Inputs[1].Value;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }
    }

    public class Lt : Func, IFuncPresenter
    {
        public Lt()
        {
            FuncName = "<";
            FuncDesc = "Элемент \"Меньше\"";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override Lt DeepClone()
        {
            return new Lt()
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
                var result = Inputs[0].Value < Inputs[1].Value;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }
    }

    public class Le : Func, IFuncPresenter
    {
        public Le()
        {
            FuncName = "<=";
            FuncDesc = "Элемент \"Меньше или равно\"";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override Le DeepClone()
        {
            return new Le()
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
                var result = Inputs[0].Value <= Inputs[1].Value;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }
    }

}