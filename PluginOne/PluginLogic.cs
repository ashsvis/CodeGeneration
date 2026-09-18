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
            return [.. items];
        }

        static partial void AddTreeNodeItems(List<TreeNode> items);

    }
}
