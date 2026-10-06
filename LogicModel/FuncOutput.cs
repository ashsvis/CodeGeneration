using PluginSupport;
using System.Xml.Linq;

namespace LogicModel
{
    public class FuncOutput : IDeepCloneable<FuncOutput>, IPersistent<FuncOutput>
    {
        public int Index { get; set; }
        public string? Name { get; internal set; }
        public double Value { get; set; }
        public ValueKind Kind { get; set; }
        public bool IsInverted { get; set; }

        public FuncOutput DeepClone()
        {
            return new FuncOutput { Value = Value, IsInverted = IsInverted };
        }

        public void ReadContent(XElement xoutput)
        {
            if (xoutput == null || xoutput.Name != "Output") return;
            var sindex = xoutput.Attribute("Index")?.Value;
            if (!string.IsNullOrWhiteSpace(sindex))
                Index = ParseHelper.ParseInteger(sindex, 0);
            var svalue = xoutput.Attribute("Value")?.Value;
            if (!string.IsNullOrWhiteSpace(svalue))
                Value = ParseHelper.ParseBoolean(svalue, false) ? 1 : 0;
            var sinverted = xoutput.Attribute("IsInverted")?.Value;
            if (!string.IsNullOrWhiteSpace(sinverted))
                IsInverted = ParseHelper.ParseBoolean(sinverted, false);
        }

        public bool NoDataToWrite()
        {
            return Value == 0 && !IsInverted;
        }

        public XElement WriteContent()
        {
            var xfunc = new XElement("Output");
            if (!NoDataToWrite())
                xfunc.Add(new XAttribute("Index", Index));
            if (Value != 0)
                xfunc.Add(new XAttribute("Value", Value));
            if (IsInverted)
                xfunc.Add(new XAttribute("IsInverted", IsInverted));
            return xfunc;
        }
    }
}
