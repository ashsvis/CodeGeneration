using PluginSupport;

namespace LogicModel
{
    public partial class PluginLogic
    {
        static partial void AddTreeNodeItems(List<TreeNode> items)
        {
            TreeNode nodesGroup;
            nodesGroup = new TreeNode("Дизъюнкция") { Tag = typeof(Or2) };
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("OR2") { Tag = typeof(Or2) });
            nodesGroup.Nodes.Add(new TreeNode("OR3") { Tag = typeof(Or3) });
            nodesGroup.Nodes.Add(new TreeNode("OR4") { Tag = typeof(Or4) });
            nodesGroup.Nodes.Add(new TreeNode("OR5") { Tag = typeof(Or5) });
            nodesGroup.Nodes.Add(new TreeNode("OR6") { Tag = typeof(Or6) });
            nodesGroup.Nodes.Add(new TreeNode("OR7") { Tag = typeof(Or7) });
            nodesGroup.Nodes.Add(new TreeNode("OR8") { Tag = typeof(Or8) });
            nodesGroup = new TreeNode("Конъюнкция") { Tag = typeof(And2) };
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

    public class Or2 : Func
    {
        public Or2()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override Or2 DeepClone()
        {
            return new Or2()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class Or3 : Func
    {
        public Or3()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new PinInput { }, new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override Or3 DeepClone()
        {
            return new Or3()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class Or4 : Func
    {
        public Or4()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override Or4 DeepClone()
        {
            return new Or4()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class Or5 : Func
    {
        public Or5()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override Or5 DeepClone()
        {
            return new Or5()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class Or6 : Func
    {
        public Or6()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override Or6 DeepClone()
        {
            return new Or6()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class Or7 : Func
    {
        public Or7()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override Or7 DeepClone()
        {
            return new Or7()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class Or8 : Func
    {
        public Or8()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override Or8 DeepClone()
        {
            return new Or8()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class And2 : Func
    {
        public And2()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override And2 DeepClone()
        {
            return new And2()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class And3 : Func
    {
        public And3()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new PinInput { }, new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override And3 DeepClone()
        {
            return new And3()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class And4 : Func
    {
        public And4()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override And4 DeepClone()
        {
            return new And4()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class And5 : Func
    {
        public And5()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override And5 DeepClone()
        {
            return new And5()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class And6 : Func
    {
        public And6()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override And6 DeepClone()
        {
            return new And6()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class And7 : Func
    {
        public And7()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override And7 DeepClone()
        {
            return new And7()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

    public class And8 : Func
    {
        public And8()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }, new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
        }

        public override And8 DeepClone()
        {
            return new And8()
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
                var result = (Inputs[0].Value > 0) ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && ((Inputs[i].Value > 0) ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && (Outputs[0].Value > 0) != result)
                {
                    Outputs[0].Value = result ? 1 : 0;
                    base.Calculate();
                }
            }
        }

   }

}