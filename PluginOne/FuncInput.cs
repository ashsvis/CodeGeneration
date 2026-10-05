using PluginSupport;
using System;
using System.Xml.Linq;

namespace PluginOne
{
    public class FuncInput : IDeepCloneable<FuncInput>, IPersistent<FuncInput>
    {
        public string? Name { get; set; }
        public bool Value { get; set; }
        public bool IsInverted { get; set; }
        public bool IsLinked { get; set; }
        public ILink? Link { get; set; }

        public FuncInput DeepClone()
        {
            return new FuncInput { Name = Name, Value = Value, IsInverted = IsInverted, Link = null };
        }

        public void ReadContent(XElement xinput)
        {
            if (xinput == null || xinput.Name != "Input") return;
            Name = xinput.Attribute("Name")?.Value;
            var svalue = xinput.Attribute("Value")?.Value;
            var sinverted = xinput.Attribute("IsInverted")?.Value;
            var slinked = xinput.Attribute("IsLinked")?.Value;
            if (!string.IsNullOrWhiteSpace(svalue) &&
                !string.IsNullOrWhiteSpace(sinverted) &&
                !string.IsNullOrWhiteSpace(slinked))
            {
                Value = ParseHelper.ParseBoolean(svalue, false);
                IsInverted = ParseHelper.ParseBoolean(sinverted, false);
                IsLinked = ParseHelper.ParseBoolean(slinked, false);
            }
        }

        public XElement WriteContent()
        {
            var xfunc = new XElement("Input");
            if (!string.IsNullOrEmpty(Name))
                xfunc.Add(new XAttribute("Name", Name));
            xfunc.Add(new XAttribute("Value", Value));
            xfunc.Add(new XAttribute("IsInverted", IsInverted));
            xfunc.Add(new XAttribute("IsLinked", IsLinked));
            return xfunc;
        }
    }
}
