
namespace PluginSupport
{
    public interface IPlugin
    {
        string Name { get; }
        TreeNode[] TreeNodeItems();
    }

}
