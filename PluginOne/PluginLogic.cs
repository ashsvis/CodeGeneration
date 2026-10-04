using PluginSupport;

namespace PluginOne
{
    public partial class PluginLogic : IPlugin
    {
        public string Name => "Элементы логики";

        public TreeNode[] TreeNodeItems()
        {
            List<TreeNode> items = [];
            items.Add(new TreeNode("Инверсия") { Tag = typeof(Not) });
            AddTreeNodeItems(items);
            items.Add(new TreeNode("Исключающее ИЛИ") { Tag = typeof(Xor) });
            items.Add(new TreeNode("RS-триггер") { Tag = typeof(Rs) });
            items.Add(new TreeNode("SR-триггер") { Tag = typeof(Sr) });
            items.Add(new TreeNode("Детектор фронта") { Tag = typeof(Rtrig) });
            items.Add(new TreeNode("Детектор спада") { Tag = typeof(Ftrig) });
            return [.. items];
        }

        static partial void AddTreeNodeItems(List<TreeNode> items);

    }
}
