using PluginSupport.Assistants;
using System.Xml.Linq;

namespace PluginSupport
{
    public class Page(int number, int width, int height, int row, int column)
    {
        const int kf = 5;
        public static int Koeff => kf;
        public int Number { get; set; } = number;
        public int PagesCount { get; set; }
        public int Width { get; set; } = width;
        public int Height { get; set; } = height;
        public int Row { get; } = row;
        public int Column { get; } = column;

        public Rectangle Bounds => new(Column * Width * kf - 100, Row * Height * kf - 25, Width * kf, Height * kf);

        public string? DocumentNumber { get; set; } = "00148599-ПИР/РНД 9-23-1сп-659/1-АК.ПБ";
        public string? CustomerName { get; set; } = "ООО \"ЛУКОЙЛ-Волгограднефтепереработка\"";
        public string[] ProjectName { get; set; } = ["Техническое перевооружение.", "«Модернизация узла слива высокооктановых добавок на площадке переработки нефти ОПО № А39-00045-001»"];
        public string? DocumentName { get; set; } = "Описание алгоритма";
        public string[] OrganizationName { get; set; } = ["ООО «ПКБ «АИСТ»", "г. Волгоград"];

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
            StamptAssistant.DrawPageBorder(graphics, foreground, this);
            if (Number == 1)
                StamptAssistant.DrawBigStamptFields(graphics, foreground, this);
            else if (Number > 1)
                StamptAssistant.DrawSmallStamptFields(graphics, foreground, this);
        }

        private Rectangle[] foundTargets = [];

        public bool StampContain(Point point)
        {
            var targets = StamptAssistant.GetStamptTargets(this);
            foundTargets = [..targets.Select(x => x.Item1).Where(x => x.Contains(point))];
            return foundTargets.Length > 0;
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
