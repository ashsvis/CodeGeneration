using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public class PageSignatureInfo
    {
        public int Index { get; set; }
        public string? Position { get; set; }
        public string? Name { get; set; }
        public GraphicsPath? Signature { get; set; }
        public string? Date { get; set; }
        public bool IsEmpty => string.IsNullOrWhiteSpace(Position) &&
            string.IsNullOrWhiteSpace(Name) &&
            string.IsNullOrWhiteSpace(Date);
    }

}
