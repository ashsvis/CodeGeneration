namespace PluginSupport
{
    public interface IGroupToPlace
    {
        Shape[] GetShapes();
        void SetPresentFunction(Shape shape);
    }
}
