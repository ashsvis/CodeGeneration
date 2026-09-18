using PluginSupport;

namespace PluginOne
{
    public partial class PluginLogic : IPlugin
    {
        public string Name => "Элементы логики";

        public TreeNode[] TreeNodeItems()
        {
            List<TreeNode> items = [];
            //items.Add(new TreeNode("Ячейка") { Tag = typeof(Cell) });
            items.Add(new TreeNode("NOT") { Tag = typeof(Not) });
            AddTreeNodeItems(items);
            //items.Add(new TreeNode("Круг") { Tag = typeof(Circle) });
            //items.Add(new TreeNode("Прямоугольник") { Tag = typeof(Rect) });
            return [.. items];
        }

        static partial void AddTreeNodeItems(List<TreeNode> items);

    }
}
