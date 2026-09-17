using PluginSupport;

namespace PluginOne
{
    public class Plugin1 : IPlugin
    {
        public string Name => "Геометрические фигуры";

        public TreeNode[] TreeNodeItems()
        {
            List<TreeNode> items = [];
            items.Add(new TreeNode("Ячейка") { Tag = typeof(Cell) });
            items.Add(new TreeNode("NOT") { Tag = typeof(Not) });
            items.Add(new TreeNode("OR") { Tag = typeof(Or2) });
            items.Add(new TreeNode("OR3") { Tag = typeof(Or3) });
            //items.Add(new TreeNode("Круг") { Tag = typeof(Circle) });
            //items.Add(new TreeNode("Прямоугольник") { Tag = typeof(Rect) });
            return [..items];
        }
    }
}
