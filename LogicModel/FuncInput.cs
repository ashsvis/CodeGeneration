using PluginSupport;
using System;
using System.Xml.Linq;

namespace LogicModel
{
    public class FuncInput : IDeepCloneable<FuncInput>, IPersistent<FuncInput>
    {
        public int Index { get; set; }
        public string? Name { get; internal set; }
        public bool Value { get; set; }
        public bool IsInverted { get; set; }
        public bool IsLinked { get; set; }
        public ILinked? Link { get; set; }

        public FuncInput DeepClone()
        {
            return new FuncInput { Index = Index, Value = Value, IsInverted = IsInverted, Link = null };
        }

        public void ReadContent(XElement xinput)
        {
            if (xinput == null || xinput.Name != "Input") return;
            var sindex = xinput.Attribute("Index")?.Value;
            if (!string.IsNullOrWhiteSpace(sindex))
                Index = ParseHelper.ParseInteger(sindex, 0);
            var svalue = xinput.Attribute("Value")?.Value;
            if (!string.IsNullOrWhiteSpace(svalue))
                Value = ParseHelper.ParseBoolean(svalue, false);
            var sinverted = xinput.Attribute("IsInverted")?.Value;
            if (!string.IsNullOrWhiteSpace(sinverted))
                IsInverted = ParseHelper.ParseBoolean(sinverted, false);
            var slinked = xinput.Attribute("IsLinked")?.Value;
            if (!string.IsNullOrWhiteSpace(slinked))
                IsLinked = ParseHelper.ParseBoolean(slinked, false);
        }

        public bool NoDataToWrite()
        {
            return !Value && !IsInverted && !IsLinked;
        }

        public XElement WriteContent()
        {
            var xfunc = new XElement("Input");
            if (!NoDataToWrite())
                xfunc.Add(new XAttribute("Index", Index));
            if (Value)
                xfunc.Add(new XAttribute("Value", Value));
            if (IsInverted)
                xfunc.Add(new XAttribute("IsInverted", IsInverted));
            if (IsLinked)
                xfunc.Add(new XAttribute("IsLinked", IsLinked));
            return xfunc;
        }
    }
}
