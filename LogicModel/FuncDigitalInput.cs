using PluginSupport;
using System.Xml.Linq;

namespace LogicModel
{
    public class FuncDigitalInput : Input
    {
        public override int Index { get; set; }
        public override string? Name { get; internal set; }
        public override bool Value { get; set; }
        public override bool IsInverted { get; set; }
        public override bool IsLinked { get; set; }
        public override ILinked? Link { get; set; }

        public override FuncDigitalInput DeepClone()
        {
            return new FuncDigitalInput { Index = Index, Value = Value, IsInverted = IsInverted, Link = null };
        }

        public override void ReadContent(XElement xinput)
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

        public override bool NoDataToWrite()
        {
            return !Value && !IsInverted && !IsLinked;
        }

        public override XElement WriteContent()
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
