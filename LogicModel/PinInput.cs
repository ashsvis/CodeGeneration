using PluginSupport;
using System.Xml.Linq;

namespace LogicModel
{
    public class PinInput : IDeepCloneable<PinInput>, IPersistent<PinInput>
    {
        public int Index { get; set; }
        public string? Name { get; internal set; }
        public double Value { get; set; }
        public ValueKind Kind { get; set; }
        public bool IsInverted { get; set; }
        public bool IsLinked { get; set; }
        public PinInfo? Source { get; set; }

        public PinInput DeepClone()
        {
            return new PinInput { Index = Index, Value = Value, Kind = Kind, IsInverted = IsInverted, Source = null};
        }

        public void ReadContent(XElement xinput)
        {
            if (xinput == null || xinput.Name != "Input") return;
            var sindex = xinput.Attribute("Index")?.Value;
            if (!string.IsNullOrWhiteSpace(sindex))
                Index = ParseHelper.ParseInteger(sindex, 0);
            var svalue = xinput.Attribute("Value")?.Value;
            if (!string.IsNullOrWhiteSpace(svalue))
                Value = ParseHelper.ParseBoolean(svalue, false) ? 1 : 0;
            var sinverted = xinput.Attribute("IsInverted")?.Value;
            if (!string.IsNullOrWhiteSpace(sinverted))
                IsInverted = ParseHelper.ParseBoolean(sinverted, false);
            var slinked = xinput.Attribute("IsLinked")?.Value;
            if (!string.IsNullOrWhiteSpace(slinked))
                IsLinked = ParseHelper.ParseBoolean(slinked, false);
        }

        public bool NoDataToWrite()
        {
            return Value == 0 && !IsInverted && !IsLinked;
        }

        public XElement WriteContent()
        {
            var xfunc = new XElement("Input");
            if (!NoDataToWrite())
                xfunc.Add(new XAttribute("Index", Index));
            if (Value != 0)
                xfunc.Add(new XAttribute("Value", Value));
            if (IsInverted)
                xfunc.Add(new XAttribute("IsInverted", IsInverted));
            if (IsLinked)
                xfunc.Add(new XAttribute("IsLinked", IsLinked));
            return xfunc;
        }
    }
}
