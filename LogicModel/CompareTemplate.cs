using PluginSupport;

namespace LogicModel
{
    public partial class PluginLogic
    {
        static partial void AddTreeNodeCompareItems(TreeNodeCollection items)
        {
            items.Add(new TreeNode("Равно") { Tag = typeof(Eq) });
            items.Add(new TreeNode("Не равно") { Tag = typeof(Ne) });
            items.Add(new TreeNode("Больше") { Tag = typeof(Gt) });
            items.Add(new TreeNode("Больше или равно") { Tag = typeof(Ge) });
            items.Add(new TreeNode("Меньше") { Tag = typeof(Lt) });
            items.Add(new TreeNode("Меньше или равно") { Tag = typeof(Le) });
        }
    }

    public class Eq : Func
    {
        public Eq()
        {
            FuncName = "EQ";
            FuncDesc = "Равно";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

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

    public class Ne : Func
    {
        public Ne()
        {
            FuncName = "NE";
            FuncDesc = "Не равно";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

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

    public class Gt : Func
    {
        public Gt()
        {
            FuncName = "GT";
            FuncDesc = "Больше";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

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

    public class Ge : Func
    {
        public Ge()
        {
            FuncName = "GE";
            FuncDesc = "Больше или равно";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

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

    public class Lt : Func
    {
        public Lt()
        {
            FuncName = "LT";
            FuncDesc = "Меньше";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

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

    public class Le : Func
    {
        public Le()
        {
            FuncName = "LE";
            FuncDesc = "Меньше или равно";
            Inputs = [new PinInput { Kind = ValueKind.Analog }, new PinInput { Kind = ValueKind.Analog }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

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