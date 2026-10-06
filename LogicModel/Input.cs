using PluginSupport;
using System.Xml.Linq;

namespace LogicModel
{
    public abstract class Input : IDeepCloneable<FuncDigitalInput>, IPersistent<FuncDigitalInput>
    {
        public abstract int Index { get; set; }
        public abstract string? Name { get; internal set; }
        public abstract bool Value { get; set; }
        public abstract bool IsInverted { get; set; }
        public abstract bool IsLinked { get; set; }
        public abstract ILinked? Link { get; set; }
        public abstract FuncDigitalInput DeepClone();
        public abstract bool NoDataToWrite();
        public abstract void ReadContent(XElement element);
        public abstract XElement WriteContent();
    }
}
