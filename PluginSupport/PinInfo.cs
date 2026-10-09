using System.ComponentModel;

namespace PluginSupport
{
    public class PinInfo(Shape? owner, bool isOutput, int pinIndex, Point? pinPoint, Guid linkId)
    {
        [Category("Owner"), DisplayName("Function")]
        public Shape? Owner { get; set; } = owner;

        [Category("Owner"), DisplayName("Label")]
        public string OwnerLabel => Owner == null ? "" : $"L{Owner.Index + 1}";
        [Category("Owner"), DisplayName("Id")]
        public string OwnerId => Owner == null ? "" : $"{Owner.Id}";

        [Browsable(false)]
        public bool IsOutput { get; set; } = isOutput;

        [Category("Pin Info"), DisplayName("Direction")]
        public string PinDirect => IsOutput ? "Output" : "Input";


        [Category("Pin Info"), DisplayName("Index"), ReadOnly(true)]
        public int PinIndex { get; set; } = pinIndex;

        [Browsable(false)]
        public Point? PinPoint { get; set; } = pinPoint;

        [Browsable(false)]
        public Guid LinkId { get; set; } = linkId;

        [Category("Linked To"), DisplayName("Id")]
        public string LinkInfo => LinkId == Guid.Empty ? "" : LinkId.ToString();
    }
}
