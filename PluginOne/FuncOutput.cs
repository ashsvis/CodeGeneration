using PluginSupport;

namespace PluginOne
{
    public class FuncOutput : IDeepCloneable<FuncOutput>
    {
        public string? Name { get; set; }
        public bool Value { get; set; }
        public bool IsInverted { get; set; }

        public FuncOutput DeepClone()
        {
            return new FuncOutput { Name = Name, Value = Value, IsInverted = IsInverted };
        }
    }
}
