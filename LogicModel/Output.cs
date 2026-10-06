using PluginSupport;
using System.Xml.Linq;

namespace LogicModel
{
    public abstract class Output : IDeepCloneable<FuncDigitalOutput>, IPersistent<FuncDigitalOutput>
    {
        public abstract int Index { get; set; }
        public abstract string? Name { get; internal set; }
        public abstract bool Value { get; set; }
        public abstract bool IsInverted { get; set; }
        public abstract FuncDigitalOutput DeepClone();
        public abstract bool NoDataToWrite();
        public abstract void ReadContent(XElement element);
        public abstract XElement WriteContent();
    }
}
