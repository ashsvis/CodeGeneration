namespace PluginSupport
{
    public interface IPageTargetLink
    {
        ValueKind ValueKind { get; }

        void RemoveTargetFromOutput(Shape? target, int targetPinIndex);
    }
}
