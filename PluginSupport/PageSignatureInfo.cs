using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public class PageSignatureInfo
    {
        public string? Position { get; set; }
        public string? Name { get; set; }
        public GraphicsPath? Signature { get; set; }
        public string? Date { get; set; }
    }

}
