using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public class PageChangeInfo
    {
        public string? Number { get; set; }
        public string? PartCount { get; set; }
        public string? MakePage { get; set; }
        public string? DocNumber { get; set; }
        public string? Name { get; set; }
        public GraphicsPath? Signature { get; set; }
        public string? Date { get; set; }
        public bool IsEmpty => string.IsNullOrWhiteSpace(Number) &&
            string.IsNullOrWhiteSpace(PartCount) &&
            string.IsNullOrWhiteSpace(MakePage) &&
            string.IsNullOrWhiteSpace(DocNumber) &&
            string.IsNullOrWhiteSpace(Name) &&
            string.IsNullOrWhiteSpace(Date);
    }

}
