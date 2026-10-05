using PluginSupport;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace PluginOne
{
    public class FuncOutput : IDeepCloneable<FuncOutput>, IPersistent<FuncOutput>
    {
        public string? Name { get; set; }
        public bool Value { get; set; }
        public bool IsInverted { get; set; }

        public FuncOutput DeepClone()
        {
            return new FuncOutput { Name = Name, Value = Value, IsInverted = IsInverted };
        }

        public void ReadContent(XElement xoutput)
        {
            if (xoutput == null || xoutput.Name != "Input") return;
            Name = xoutput.Attribute("Name")?.Value;
            var svalue = xoutput.Attribute("Value")?.Value;
            var sinverted = xoutput.Attribute("IsInverted")?.Value;
            if (!string.IsNullOrWhiteSpace(svalue) &&
                !string.IsNullOrWhiteSpace(sinverted))
            {
                Value = ParseHelper.ParseBoolean(svalue, false);
                IsInverted = ParseHelper.ParseBoolean(sinverted, false);
            }
        }

        public XElement WriteContent()
        {
            var xfunc = new XElement("Output");
            if (!string.IsNullOrEmpty(Name))
                xfunc.Add(new XAttribute("Name", Name));
            xfunc.Add(new XAttribute("Value", Value));
            xfunc.Add(new XAttribute("IsInverted", IsInverted));
            return xfunc;
        }
    }
}
