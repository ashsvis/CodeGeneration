using PluginSupport;

namespace PluginOne
{
    public partial class PluginLogic : IPlugin
    {
        public string Name => "Элементы логики";

        public TreeNode[] TreeNodeItems()
        {
            List<TreeNode> items = [];
            TreeNode nodesGroup;
            nodesGroup = new TreeNode("Ввод сигнала") { Tag = typeof(DigitalInput) };
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("Дискретный ввод") { Tag = typeof(DigitalInput) });
            items.Add(new TreeNode("Инверсия") { Tag = typeof(Not) });
            AddTreeNodeItems(items);
            items.Add(new TreeNode("Исключающее ИЛИ") { Tag = typeof(Xor) });
            nodesGroup = new TreeNode("Триггер") { Tag = typeof(Rs) };
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("RS-триггер") { Tag = typeof(Rs) });
            nodesGroup.Nodes.Add(new TreeNode("SR-триггер") { Tag = typeof(Sr) });
            nodesGroup = new TreeNode("Детектор фронта") { Tag = typeof(Rtrig) };
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("Детектор фронта") { Tag = typeof(Rtrig) });
            nodesGroup.Nodes.Add(new TreeNode("Детектор спада") { Tag = typeof(Ftrig) });
            nodesGroup = new TreeNode("Вывод сигнала") { Tag = typeof(DigitalOutput) };
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("Дискретный вывод") { Tag = typeof(DigitalOutput) });
            return [.. items];
        }

        static partial void AddTreeNodeItems(List<TreeNode> items);

    }
}
