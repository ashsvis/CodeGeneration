
namespace PluginOne
{
    public partial class PluginLogic
    {
        static partial void AddTreeNodeItems(List<TreeNode> items)
        {
            TreeNode nodesGroup;
            nodesGroup = new TreeNode("Дизъюнкция");
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("OR2") { Tag = typeof(Or2) });
            nodesGroup.Nodes.Add(new TreeNode("OR3") { Tag = typeof(Or3) });
            nodesGroup.Nodes.Add(new TreeNode("OR4") { Tag = typeof(Or4) });
            nodesGroup.Nodes.Add(new TreeNode("OR5") { Tag = typeof(Or5) });
            nodesGroup.Nodes.Add(new TreeNode("OR6") { Tag = typeof(Or6) });
            nodesGroup.Nodes.Add(new TreeNode("OR7") { Tag = typeof(Or7) });
            nodesGroup.Nodes.Add(new TreeNode("OR8") { Tag = typeof(Or8) });
            nodesGroup = new TreeNode("Конъюнкция");
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

    public class Or2 : Cell
    {
        public Or2()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            InvertInputs = [false, false];
            InvertOutputs = [false];
            Inputs = [false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class Or3 : Cell
    {
        public Or3()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            InvertInputs = [false, false, false];
            InvertOutputs = [false];
            Inputs = [false, false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class Or4 : Cell
    {
        public Or4()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            InvertInputs = [false, false, false, false];
            InvertOutputs = [false];
            Inputs = [false, false, false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class Or5 : Cell
    {
        public Or5()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            InvertInputs = [false, false, false, false, false];
            InvertOutputs = [false];
            Inputs = [false, false, false, false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class Or6 : Cell
    {
        public Or6()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            InvertInputs = [false, false, false, false, false, false];
            InvertOutputs = [false];
            Inputs = [false, false, false, false, false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class Or7 : Cell
    {
        public Or7()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            InvertInputs = [false, false, false, false, false, false, false];
            InvertOutputs = [false];
            Inputs = [false, false, false, false, false, false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class Or8 : Cell
    {
        public Or8()
        {
            FuncName = "1";
            FuncDesc = "Дизъюнкция";
            InvertInputs = [false, false, false, false, false, false, false, false];
            InvertOutputs = [false];
            Inputs = [false, false, false, false, false, false, false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result || (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class And2 : Cell
    {
        public And2()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            InvertInputs = [false, false];
            InvertOutputs = [false];
            Inputs = [false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class And3 : Cell
    {
        public And3()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            InvertInputs = [false, false, false];
            InvertOutputs = [false];
            Inputs = [false, false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class And4 : Cell
    {
        public And4()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            InvertInputs = [false, false, false, false];
            InvertOutputs = [false];
            Inputs = [false, false, false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class And5 : Cell
    {
        public And5()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            InvertInputs = [false, false, false, false, false];
            InvertOutputs = [false];
            Inputs = [false, false, false, false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class And6 : Cell
    {
        public And6()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            InvertInputs = [false, false, false, false, false, false];
            InvertOutputs = [false];
            Inputs = [false, false, false, false, false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class And7 : Cell
    {
        public And7()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            InvertInputs = [false, false, false, false, false, false, false];
            InvertOutputs = [false];
            Inputs = [false, false, false, false, false, false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

    public class And8 : Cell
    {
        public And8()
        {
            FuncName = "&";
            FuncDesc = "Конъюнкция";
            InvertInputs = [false, false, false, false, false, false, false, false];
            InvertOutputs = [false];
            Inputs = [false, false, false, false, false, false, false, false];
            Outputs = [false];
        }

        public override void Calculate(bool[] values, bool[] inverts)
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0] ^ InvertInputs[0];
                for (int i = 1; i < Inputs.Length; i++)
                    result = result && (Inputs[i] ^ InvertInputs[i]);
                if (Outputs.Length > 0)
                    Outputs[0] = result;
            }
        }
    }

}