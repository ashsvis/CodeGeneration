namespace PluginSupport
{
    public interface IPageLink
    {
        ValueKind ValueKind { get; }

        void SetLinkNumber(int number);
    }
}
