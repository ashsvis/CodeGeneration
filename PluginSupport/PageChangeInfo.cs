using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public class PageChangeInfo
    {
        public string? ChangeNumber { get; set; }
        public string? PartCount { get; set; }
        public string? MakePage { get; set; }
        public string? DocNumber { get; set; }
        public GraphicsPath? Signature { get; set; }
        public string? Date { get; set; }
    }

}
