using PluginSupport;

namespace PluginOne
{
    public class FuncInput
    {
        public string? Name { get; set; }
        public bool Value { get; set; }
        public bool IsInverted { get; set; }
        public bool IsLinked { get; set; }
        public ILink? Link { get; set; }
    }
}
