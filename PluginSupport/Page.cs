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

        public string? DocumentNumber { get; set; } = "00148599-ПИР/РНД 9-23-1сп-659/1-АК.ПБ";
        public string? CustomerName { get; set; } = "ООО \"ЛУКОЙЛ-Волгограднефтепереработка\"";
        public string? ProjectName { get; set; } = "Техническое перевооружение.\r\n«Модернизация узла слива высокооктановых добавок на площадке переработки нефти ОПО № А39-00045-001»";
        public string? DocumentName { get; set; } = "Описание алгоритма";
        public string? OrganizationName { get; set; } = "ООО «ПКБ «АИСТ»\r\nг. Волгоград";

        public PageChangeInfo[] Changes { get; set; } = 
        [
            new(), new(), new(), new()
        ];

        public PageSignatureInfo[] Signatures { get; set; } =
        [
            new() { Position = "Разраб."},
            new() { Position = "Пров."},
            new(),
            new(),
            new() { Position = "Н.контр."},
            new() { Position = "ГИП"},
        ];

        public void Draw(Graphics? graphics, Color foreground)
        {
            StampAssistant.DrawPageBorder(graphics, foreground, this);
            if (Number == 1)
                StampAssistant.DrawBigStampFields(graphics, foreground, this, kf);
            else if (Number > 1)
                StampAssistant.DrawSmallStampFields(graphics, foreground, this, kf);
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
