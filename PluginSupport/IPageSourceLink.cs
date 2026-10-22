namespace PluginSupport
{
    public interface IPageSourceLink
    {
        ValueKind ValueKind { get; }

        void SetLinkNumber(int number);
        int GetLinkNumber();
        Guid[] GetIdsForConnect();
        void AddTarget(Shape shape);
        void ReplaceTargetId(Guid oldId, Guid newId);
    }
}
