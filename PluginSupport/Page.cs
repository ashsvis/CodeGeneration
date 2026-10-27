using PluginSupport.Assistants;
using System.Xml.Linq;

namespace PluginSupport
{
    public class Page(int number, int width, int height, int row, int column)
    {
        const int kf = 5;
        public int Number { get; set; } = number;
        public int PagesCount { get; set; }
        public int Width { get; set; } = width;
        public int Height { get; set; } = height;
        public int Row { get; } = row;
        public int Column { get; } = column;

        public Rectangle Bounds => new(Column * Width * kf - 100, Row * Height * kf - 25, Width * kf, Height * kf);

        public void Draw(Graphics? graphics, Color foreground)
        {
            StampAssistant.DrawPageBorder(graphics, new Point(Column * Width, Row * Height), Width, Height, foreground, Number, PagesCount);
        }

        public XElement WriteContent()
        {
            var xpage = new XElement("Page");
            xpage.Add(new XAttribute("Number", Number));
            xpage.Add(new XAttribute("Row", Row));
            xpage.Add(new XAttribute("Column", Column));
            xpage.Add(new XAttribute("Width", Width));
            xpage.Add(new XAttribute("Height", Height));
            xpage.Add(new XAttribute("Bounds", $"{Bounds.X},{Bounds.Y},{Bounds.Width},{Bounds.Height}"));
            return xpage;
        }
    }
}
