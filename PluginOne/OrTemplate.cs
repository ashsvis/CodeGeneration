
namespace PluginOne
{
    public partial class PluginLogic
    {
        static partial void AddTreeNodeItems(List<TreeNode> items)
        {
            items.Add(new TreeNode("OR2") { Tag = typeof(Or2) });
            items.Add(new TreeNode("OR3") { Tag = typeof(Or3) });
            items.Add(new TreeNode("OR4") { Tag = typeof(Or4) });
            items.Add(new TreeNode("OR5") { Tag = typeof(Or5) });
            items.Add(new TreeNode("OR6") { Tag = typeof(Or6) });
            items.Add(new TreeNode("OR7") { Tag = typeof(Or7) });
            items.Add(new TreeNode("OR8") { Tag = typeof(Or8) });
        }
    }

    public class Or2 : Cell
    {
        public Or2()
        {
            Inputs = [false, false];
            Outputs = [false];
        }
    }

    public class Or3 : Cell
    {
        public Or3()
        {
            Inputs = [false, false, false];
            Outputs = [false];
        }
    }

    public class Or4 : Cell
    {
        public Or4()
        {
            Inputs = [false, false, false, false];
            Outputs = [false];
        }
    }

    public class Or5 : Cell
    {
        public Or5()
        {
            Inputs = [false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class Or6 : Cell
    {
        public Or6()
        {
            Inputs = [false, false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class Or7 : Cell
    {
        public Or7()
        {
            Inputs = [false, false, false, false, false, false, false];
            Outputs = [false];
        }
    }

    public class Or8 : Cell
    {
        public Or8()
        {
            Inputs = [false, false, false, false, false, false, false, false];
            Outputs = [false];
        }
    }

}