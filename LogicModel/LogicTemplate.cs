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

    public class Or2 : Func, ILinked
    {
        public Or2()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }
   }

    public class Or3 : Func, ILinked
    {
        public Or3()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange += MakeChangesFor3;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange -= MakeChangesFor3;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }

        public void MakeChangesFor3(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[2].Value = e.NewValue;
        }
   }

    public class Or4 : Func, ILinked
    {
        public Or4()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange += MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange += MakeChangesFor4;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange -= MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange -= MakeChangesFor4;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }

        public void MakeChangesFor3(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[2].Value = e.NewValue;
        }

        public void MakeChangesFor4(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[3].Value = e.NewValue;
        }
   }

    public class Or5 : Func, ILinked
    {
        public Or5()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange += MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange += MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange += MakeChangesFor5;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange -= MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange -= MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange -= MakeChangesFor5;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }

        public void MakeChangesFor3(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[2].Value = e.NewValue;
        }

        public void MakeChangesFor4(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[3].Value = e.NewValue;
        }

        public void MakeChangesFor5(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[4].Value = e.NewValue;
        }
   }

    public class Or6 : Func, ILinked
    {
        public Or6()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange += MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange += MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange += MakeChangesFor5;
                    break;
                case 5:
                    link.OnOutputChange += MakeChangesFor6;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange -= MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange -= MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange -= MakeChangesFor5;
                    break;
                case 5:
                    link.OnOutputChange -= MakeChangesFor6;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }

        public void MakeChangesFor3(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[2].Value = e.NewValue;
        }

        public void MakeChangesFor4(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[3].Value = e.NewValue;
        }

        public void MakeChangesFor5(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[4].Value = e.NewValue;
        }

        public void MakeChangesFor6(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[5].Value = e.NewValue;
        }
   }

    public class Or7 : Func, ILinked
    {
        public Or7()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange += MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange += MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange += MakeChangesFor5;
                    break;
                case 5:
                    link.OnOutputChange += MakeChangesFor6;
                    break;
                case 6:
                    link.OnOutputChange += MakeChangesFor7;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange -= MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange -= MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange -= MakeChangesFor5;
                    break;
                case 5:
                    link.OnOutputChange -= MakeChangesFor6;
                    break;
                case 6:
                    link.OnOutputChange -= MakeChangesFor7;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }

        public void MakeChangesFor3(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[2].Value = e.NewValue;
        }

        public void MakeChangesFor4(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[3].Value = e.NewValue;
        }

        public void MakeChangesFor5(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[4].Value = e.NewValue;
        }

        public void MakeChangesFor6(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[5].Value = e.NewValue;
        }

        public void MakeChangesFor7(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[6].Value = e.NewValue;
        }
   }

    public class Or8 : Func, ILinked
    {
        public Or8()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange += MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange += MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange += MakeChangesFor5;
                    break;
                case 5:
                    link.OnOutputChange += MakeChangesFor6;
                    break;
                case 6:
                    link.OnOutputChange += MakeChangesFor7;
                    break;
                case 7:
                    link.OnOutputChange += MakeChangesFor8;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange -= MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange -= MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange -= MakeChangesFor5;
                    break;
                case 5:
                    link.OnOutputChange -= MakeChangesFor6;
                    break;
                case 6:
                    link.OnOutputChange -= MakeChangesFor7;
                    break;
                case 7:
                    link.OnOutputChange -= MakeChangesFor8;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }

        public void MakeChangesFor3(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[2].Value = e.NewValue;
        }

        public void MakeChangesFor4(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[3].Value = e.NewValue;
        }

        public void MakeChangesFor5(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[4].Value = e.NewValue;
        }

        public void MakeChangesFor6(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[5].Value = e.NewValue;
        }

        public void MakeChangesFor7(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[6].Value = e.NewValue;
        }

        public void MakeChangesFor8(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[7].Value = e.NewValue;
        }
   }

    public class And2 : Func, ILinked
    {
        public And2()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }
   }

    public class And3 : Func, ILinked
    {
        public And3()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange += MakeChangesFor3;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange -= MakeChangesFor3;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }

        public void MakeChangesFor3(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[2].Value = e.NewValue;
        }
   }

    public class And4 : Func, ILinked
    {
        public And4()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange += MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange += MakeChangesFor4;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange -= MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange -= MakeChangesFor4;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }

        public void MakeChangesFor3(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[2].Value = e.NewValue;
        }

        public void MakeChangesFor4(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[3].Value = e.NewValue;
        }
   }

    public class And5 : Func, ILinked
    {
        public And5()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange += MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange += MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange += MakeChangesFor5;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange -= MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange -= MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange -= MakeChangesFor5;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }

        public void MakeChangesFor3(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[2].Value = e.NewValue;
        }

        public void MakeChangesFor4(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[3].Value = e.NewValue;
        }

        public void MakeChangesFor5(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[4].Value = e.NewValue;
        }
   }

    public class And6 : Func, ILinked
    {
        public And6()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange += MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange += MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange += MakeChangesFor5;
                    break;
                case 5:
                    link.OnOutputChange += MakeChangesFor6;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange -= MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange -= MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange -= MakeChangesFor5;
                    break;
                case 5:
                    link.OnOutputChange -= MakeChangesFor6;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }

        public void MakeChangesFor3(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[2].Value = e.NewValue;
        }

        public void MakeChangesFor4(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[3].Value = e.NewValue;
        }

        public void MakeChangesFor5(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[4].Value = e.NewValue;
        }

        public void MakeChangesFor6(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[5].Value = e.NewValue;
        }
   }

    public class And7 : Func, ILinked
    {
        public And7()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange += MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange += MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange += MakeChangesFor5;
                    break;
                case 5:
                    link.OnOutputChange += MakeChangesFor6;
                    break;
                case 6:
                    link.OnOutputChange += MakeChangesFor7;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange -= MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange -= MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange -= MakeChangesFor5;
                    break;
                case 5:
                    link.OnOutputChange -= MakeChangesFor6;
                    break;
                case 6:
                    link.OnOutputChange -= MakeChangesFor7;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }

        public void MakeChangesFor3(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[2].Value = e.NewValue;
        }

        public void MakeChangesFor4(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[3].Value = e.NewValue;
        }

        public void MakeChangesFor5(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[4].Value = e.NewValue;
        }

        public void MakeChangesFor6(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[5].Value = e.NewValue;
        }

        public void MakeChangesFor7(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[6].Value = e.NewValue;
        }
   }

    public class And8 : Func, ILinked
    {
        public And8()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            Inputs = [new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }, new FuncDigitalInput { }];
            Outputs = [new FuncDigitalOutput { }];
            CalculateHeight();
        }

        public event OutputChangedEventHandler? OnOutputChange;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value ^ Inputs[0].IsInverted;
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i].Value ^ Inputs[i].IsInverted);
                result ^= Outputs[0].IsInverted;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange += MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange += MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange += MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange += MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange += MakeChangesFor5;
                    break;
                case 5:
                    link.OnOutputChange += MakeChangesFor6;
                    break;
                case 6:
                    link.OnOutputChange += MakeChangesFor7;
                    break;
                case 7:
                    link.OnOutputChange += MakeChangesFor8;
                    break;
            }
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
       }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            switch (index)
            {
                case 0:
                    link.OnOutputChange -= MakeChangesFor1;
                    break;
                case 1:
                    link.OnOutputChange -= MakeChangesFor2;
                    break;
                case 2:
                    link.OnOutputChange -= MakeChangesFor3;
                    break;
                case 3:
                    link.OnOutputChange -= MakeChangesFor4;
                    break;
                case 4:
                    link.OnOutputChange -= MakeChangesFor5;
                    break;
                case 5:
                    link.OnOutputChange -= MakeChangesFor6;
                    break;
                case 6:
                    link.OnOutputChange -= MakeChangesFor7;
                    break;
                case 7:
                    link.OnOutputChange -= MakeChangesFor8;
                    break;
            }
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
       }

        public void MakeChangesFor1(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
        }

        public void MakeChangesFor2(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[1].Value = e.NewValue;
        }

        public void MakeChangesFor3(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[2].Value = e.NewValue;
        }

        public void MakeChangesFor4(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[3].Value = e.NewValue;
        }

        public void MakeChangesFor5(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[4].Value = e.NewValue;
        }

        public void MakeChangesFor6(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[5].Value = e.NewValue;
        }

        public void MakeChangesFor7(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[6].Value = e.NewValue;
        }

        public void MakeChangesFor8(object? sender,  OutputChangedEventArgs e)
        {
            Inputs[7].Value = e.NewValue;
        }
   }

}