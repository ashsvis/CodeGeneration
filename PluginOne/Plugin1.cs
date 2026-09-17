using PluginSupport;

namespace PluginOne
{
    public class Plugin1 : IPlugin
    {
        public string Name => "Геометрические фигуры";

        public TreeNode[] TreeNodeItems()
        {
            List<TreeNode> items = [];
            items.Add(new TreeNode("Круг") { Tag = typeof(Circle) });
            items.Add(new TreeNode("Прямоугольник") { Tag = typeof(Rect) });
            return [..items];
        }
    }
}
