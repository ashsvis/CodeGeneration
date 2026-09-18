
namespace PluginOne
{
    public partial class PluginLogic
    {
        static partial void AddTreeNodeItems(List<TreeNode> items)
        {
            TreeNode nodesGroup;
            nodesGroup = new TreeNode("OR");
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("OR2") { Tag = typeof(Or2) });
            nodesGroup.Nodes.Add(new TreeNode("OR3") { Tag = typeof(Or3) });
            nodesGroup.Nodes.Add(new TreeNode("OR4") { Tag = typeof(Or4) });
            nodesGroup.Nodes.Add(new TreeNode("OR5") { Tag = typeof(Or5) });
            nodesGroup.Nodes.Add(new TreeNode("OR6") { Tag = typeof(Or6) });
            nodesGroup.Nodes.Add(new TreeNode("OR7") { Tag = typeof(Or7) });
            nodesGroup.Nodes.Add(new TreeNode("OR8") { Tag = typeof(Or8) });
            nodesGroup = new TreeNode("AND");
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("AND2") { Tag = typeof(And2) });
            nodesGroup.Nodes.Add(new TreeNode("AND3") { Tag = typeof(And3) });
            nodesGroup.Nodes.Add(new TreeNode("AND4") { Tag = typeof(And4) });
            nodesGroup.Nodes.Add(new TreeNode("AND5") { Tag = typeof(And5) });
            nodesGroup.Nodes.Add(new TreeNode("AND6") { Tag = typeof(And6) });
            nodesGroup.Nodes.Add(new TreeNode("AND7") { Tag = typeof(And7) });
            nodesGroup.Nodes.Add(new TreeNode("AND8") { Tag = typeof(And8) });
            nodesGroup = new TreeNode("XOR");
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("XOR2") { Tag = typeof(Xor2) });
            nodesGroup.Nodes.Add(new TreeNode("XOR3") { Tag = typeof(Xor3) });
            nodesGroup.Nodes.Add(new TreeNode("XOR4") { Tag = typeof(Xor4) });
            nodesGroup.Nodes.Add(new TreeNode("XOR5") { Tag = typeof(Xor5) });
            nodesGroup.Nodes.Add(new TreeNode("XOR6") { Tag = typeof(Xor6) });
            nodesGroup.Nodes.Add(new TreeNode("XOR7") { Tag = typeof(Xor7) });
            nodesGroup.Nodes.Add(new TreeNode("XOR8") { Tag = typeof(Xor8) });
        }
    }

    public class Or2 : Cell
    {
        public Or2()
        {
            FuncName = "1";
            Inputs = [false, false];
            Outputs = [false];
        }
    }

    public class Or3 : Cell
    {
        public Or3()
        {
            FuncName = "1";
            Inputs = [false, false, false];
            Outputs = [false];
        }
    }

    public class Or4 : Cell
    {
        public Or4()
        {
            FuncName = "1";
            Inputs = [false, false, false, false];
            Outputs = [false];
        }
    }

    public class Or5 : Cell
    {
        public Or5()
        {
            FuncName = "1";
            Inputs = [false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class Or6 : Cell
    {
        public Or6()
        {
            FuncName = "1";
            Inputs = [false, false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class Or7 : Cell
    {
        public Or7()
        {
            FuncName = "1";
            Inputs = [false, false, false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class Or8 : Cell
    {
        public Or8()
        {
            FuncName = "1";
            Inputs = [false, false, false, false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class And2 : Cell
    {
        public And2()
        {
            FuncName = "&";
            Inputs = [false, false];
            Outputs = [false];
        }
    }

    public class And3 : Cell
    {
        public And3()
        {
            FuncName = "&";
            Inputs = [false, false, false];
            Outputs = [false];
        }
    }

    public class And4 : Cell
    {
        public And4()
        {
            FuncName = "&";
            Inputs = [false, false, false, false];
            Outputs = [false];
        }
    }

    public class And5 : Cell
    {
        public And5()
        {
            FuncName = "&";
            Inputs = [false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class And6 : Cell
    {
        public And6()
        {
            FuncName = "&";
            Inputs = [false, false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class And7 : Cell
    {
        public And7()
        {
            FuncName = "&";
            Inputs = [false, false, false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class And8 : Cell
    {
        public And8()
        {
            FuncName = "&";
            Inputs = [false, false, false, false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class Xor2 : Cell
    {
        public Xor2()
        {
            FuncName = "=1";
            Inputs = [false, false];
            Outputs = [false];
        }
    }

    public class Xor3 : Cell
    {
        public Xor3()
        {
            FuncName = "=1";
            Inputs = [false, false, false];
            Outputs = [false];
        }
    }

    public class Xor4 : Cell
    {
        public Xor4()
        {
            FuncName = "=1";
            Inputs = [false, false, false, false];
            Outputs = [false];
        }
    }

    public class Xor5 : Cell
    {
        public Xor5()
        {
            FuncName = "=1";
            Inputs = [false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class Xor6 : Cell
    {
        public Xor6()
        {
            FuncName = "=1";
            Inputs = [false, false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class Xor7 : Cell
    {
        public Xor7()
        {
            FuncName = "=1";
            Inputs = [false, false, false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class Xor8 : Cell
    {
        public Xor8()
        {
            FuncName = "=1";
            Inputs = [false, false, false, false, false, false, false, false];
            Outputs = [false];
        }
    }

}