using PluginSupport;

namespace LogicModel
{
    public partial class PluginLogic
    {
        static partial void AddTreeNodeLogicItems(List<TreeNode> items)
        {
            TreeNode nodesGroup;
            nodesGroup = new TreeNode("Элемент \"ИЛИ\"") { Tag = typeof(Or2) };
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("OR2") { Tag = typeof(Or2) });
            nodesGroup.Nodes.Add(new TreeNode("OR3") { Tag = typeof(Or3) });
            nodesGroup.Nodes.Add(new TreeNode("OR4") { Tag = typeof(Or4) });
            nodesGroup.Nodes.Add(new TreeNode("OR5") { Tag = typeof(Or5) });
            nodesGroup.Nodes.Add(new TreeNode("OR6") { Tag = typeof(Or6) });
            nodesGroup.Nodes.Add(new TreeNode("OR7") { Tag = typeof(Or7) });
            nodesGroup.Nodes.Add(new TreeNode("OR8") { Tag = typeof(Or8) });
            nodesGroup = new TreeNode("Элемент \"И\"") { Tag = typeof(And2) };
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("AND2") { Tag = typeof(And2) });
            nodesGroup.Nodes.Add(new TreeNode("AND3") { Tag = typeof(And3) });
            nodesGroup.Nodes.Add(new TreeNode("AND4") { Tag = typeof(And4) });
            nodesGroup.Nodes.Add(new TreeNode("AND5") { Tag = typeof(And5) });
            nodesGroup.Nodes.Add(new TreeNode("AND6") { Tag = typeof(And6) });
            nodesGroup.Nodes.Add(new TreeNode("AND7") { Tag = typeof(And7) });
            nodesGroup.Nodes.Add(new TreeNode("AND8") { Tag = typeof(And8) });
        }
    }

    public class Or2 : Func, IFuncPresenter
    {
        public Or2()
        {
            FuncName = "1";
            FuncDesc = "Элемент \"ИЛИ\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => true;

        public override Or2 DeepClone()
        {
            return new Or2()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class Or3 : Func, IFuncPresenter
    {
        public Or3()
        {
            FuncName = "1";
            FuncDesc = "Элемент \"ИЛИ\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override Or3 DeepClone()
        {
            return new Or3()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class Or4 : Func, IFuncPresenter
    {
        public Or4()
        {
            FuncName = "1";
            FuncDesc = "Элемент \"ИЛИ\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override Or4 DeepClone()
        {
            return new Or4()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class Or5 : Func, IFuncPresenter
    {
        public Or5()
        {
            FuncName = "1";
            FuncDesc = "Элемент \"ИЛИ\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override Or5 DeepClone()
        {
            return new Or5()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class Or6 : Func, IFuncPresenter
    {
        public Or6()
        {
            FuncName = "1";
            FuncDesc = "Элемент \"ИЛИ\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override Or6 DeepClone()
        {
            return new Or6()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class Or7 : Func, IFuncPresenter
    {
        public Or7()
        {
            FuncName = "1";
            FuncDesc = "Элемент \"ИЛИ\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override Or7 DeepClone()
        {
            return new Or7()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class Or8 : Func, IFuncPresenter
    {
        public Or8()
        {
            FuncName = "1";
            FuncDesc = "Элемент \"ИЛИ\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override Or8 DeepClone()
        {
            return new Or8()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class And2 : Func, IFuncPresenter
    {
        public And2()
        {
            FuncName = "&";
            FuncDesc = "Элемент \"И\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => true;

        public override And2 DeepClone()
        {
            return new And2()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class And3 : Func, IFuncPresenter
    {
        public And3()
        {
            FuncName = "&";
            FuncDesc = "Элемент \"И\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override And3 DeepClone()
        {
            return new And3()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class And4 : Func, IFuncPresenter
    {
        public And4()
        {
            FuncName = "&";
            FuncDesc = "Элемент \"И\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override And4 DeepClone()
        {
            return new And4()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class And5 : Func, IFuncPresenter
    {
        public And5()
        {
            FuncName = "&";
            FuncDesc = "Элемент \"И\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override And5 DeepClone()
        {
            return new And5()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class And6 : Func, IFuncPresenter
    {
        public And6()
        {
            FuncName = "&";
            FuncDesc = "Элемент \"И\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override And6 DeepClone()
        {
            return new And6()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class And7 : Func, IFuncPresenter
    {
        public And7()
        {
            FuncName = "&";
            FuncDesc = "Элемент \"И\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override And7 DeepClone()
        {
            return new And7()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

    public class And8 : Func, IFuncPresenter
    {
        public And8()
        {
            FuncName = "&";
            FuncDesc = "Элемент \"И\"";
            Inputs = [new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }, new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => false;

        public override And8 DeepClone()
        {
            return new And8()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    Transfer();
                }
            }
        }

   }

}