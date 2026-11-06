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
        public int Row { get; set; } = row;
        public int Column { get; set; } = column;

        public Rectangle Bounds => new(Column * Width * kf - 100, Row * Height * kf - 25, Width * kf, Height * kf);

        public override string ToString()
        {
            return $"Page #{Number} Bounds={Bounds} Row={Row} Column={Column}";
        }

        public string? DocumentNumber { get; set; }
        public string? CustomerName { get; set; }
        public string[] ProjectName { get; set; } = [];
        public string? DocumentName { get; set; }
        public string? DocumentStage { get; set; }
        public string[] OrganizationName { get; set; } = [];

        public PageChangeInfo[] Changes { get; set; } = 
        [
            new(), new(), new(), new()
        ];

        public PageSignatureInfo[] Signatures { get; set; } =
        [
            new() { Index = 0 }, new() { Index = 1 }, new() { Index = 2 }, new() { Index = 3 }, new() { Index = 4 }, new() { Index = 5 }
        ];

        public void Draw(Graphics? graphics, Color foreground)
        {
            StampAssistant.DrawPageBorder(graphics, foreground, this);
            if (Number == 1)
                StampAssistant.DrawBigStampFields(graphics, foreground, this);
            else if (Number > 1)
                StampAssistant.DrawSmallStampFields(graphics, foreground, this);
        }

        private Rectangle[] foundTargets = [];

        public bool StampContain(Point point)
        {
            var targets = StampAssistant.GetStampTargets(this);
            foundTargets = [..targets.Select(x => x.Item1).Where(x => x.Contains(point))];
            return foundTargets.Length > 0;
        }

        public void ReadContent(XElement xpage)
        {
            var xwidth = xpage.Attribute("Width")?.Value;
            if (!string.IsNullOrWhiteSpace(xwidth))
                Width = ParseHelper.ParseInteger(xwidth, 420);
            var xheight = xpage.Attribute("Height")?.Value;
            if (!string.IsNullOrWhiteSpace(xheight))
                Height = ParseHelper.ParseInteger(xheight, 297);
            var xpageNumber = xpage.Attribute("Number")?.Value;
            if (!string.IsNullOrWhiteSpace(xpageNumber))
            {
                var pageNumber = ParseHelper.ParseInteger(xpageNumber, 0);
                var xstamp = xpage.Element("Stamp");
                if (xstamp != null)
                {
                    DocumentNumber = xstamp.Element("Number")?.Value;
                    ReadChanges(pageNumber, xstamp);
                    if (pageNumber == 1)
                    {
                        CustomerName = xstamp.Element("Customer")?.Value;
                        var xprojname = xstamp.Element("Project");
                        if (xprojname != null)
                        {
                            List<string> lines = [];
                            foreach (var xline in xprojname.Elements("Line"))
                                lines.Add(xline.Value);
                            ProjectName = [.. lines];
                        }
                        else
                            ProjectName = [];
                        DocumentName = xstamp.Element("Name")?.Value;
                        DocumentStage = xstamp.Element("Stage")?.Value;
                        var xorgname = xstamp.Element("Organization");
                        if (xorgname != null)
                        {
                            List<string> lines = [];
                            foreach (var xline in xorgname.Elements("Line"))
                                lines.Add(xline.Value);
                            OrganizationName = [.. lines];
                        }
                        else
                            OrganizationName = [];
                        ReadSignatures(xstamp);
                    }
                }
            }
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
            var xstamp = new XElement("Stamp");
            xpage.Add(xstamp);
            xstamp.Add(new XElement("Number", DocumentNumber));
            WriteChanges(xstamp);
            if (Number == 1)
            {
                xstamp.Add(new XElement("Customer", CustomerName));
                var xprojname = new XElement("Project");
                xstamp.Add(xprojname);
                foreach (var line in ProjectName)
                    xprojname.Add(new XElement("Line", line));
                xstamp.Add(new XElement("Name", DocumentName));
                xstamp.Add(new XElement("Stage", DocumentStage));
                var xorgname = new XElement("Organization");
                xstamp.Add(xorgname);
                foreach (var line in OrganizationName)
                    xorgname.Add(new XElement("Line", line));
                WriteSignatures(xstamp);
            }
            return xpage;
        }

        private void ReadChanges(int pageNumber, XElement xstamp)
        {
            var n = 0;
            foreach (var xsignature in xstamp.Elements("Change"))
            {
                Changes[n].Number = xsignature.Attribute("ChangeNumber")?.Value;
                Changes[n].PartCount = xsignature.Attribute("PartCount")?.Value;
                Changes[n].MakePage = xsignature.Attribute("MakePage")?.Value;
                Changes[n].DocNumber = xsignature.Attribute("DocNumber")?.Value;
                Changes[n].Name = xsignature.Attribute("Name")?.Value;
                Changes[n].Date = xsignature.Attribute("Date")?.Value;

                if (pageNumber > 1 && n > 1) break;
                n++;
            }
        }

        private void WriteChanges(XElement xstamp)
        {
            foreach (var change in Changes.Where(x => !x.IsEmpty))
            {
                var xchange = new XElement("Change");
                xstamp.Add(xchange);
                if (!string.IsNullOrWhiteSpace(change.Number))
                    xchange.Add(new XAttribute("ChangeNumber", change.Number ?? ""));
                if (!string.IsNullOrWhiteSpace(change.PartCount))
                    xchange.Add(new XAttribute("PartCount", change.PartCount ?? ""));
                if (!string.IsNullOrWhiteSpace(change.MakePage))
                    xchange.Add(new XAttribute("MakePage", change.MakePage ?? ""));
                if (!string.IsNullOrWhiteSpace(change.DocNumber))
                    xchange.Add(new XAttribute("DocNumber", change.DocNumber ?? ""));
                if (!string.IsNullOrWhiteSpace(change.Name))
                    xchange.Add(new XAttribute("Name", change.Name ?? ""));
                if (!string.IsNullOrWhiteSpace(change.Date))
                    xchange.Add(new XAttribute("Date", change.Date ?? ""));
            }
        }

        private void ReadSignatures(XElement xstamp)
        {
            foreach (var xsignature in xstamp.Elements("Signature"))
            {
                var xindex = xsignature.Attribute("Index")?.Value;
                if (!string.IsNullOrWhiteSpace(xindex))
                {
                    var index = ParseHelper.ParseInteger(xindex, 0);
                    if (index >= 0 && index < Signatures.Length)
                    {
                        var xpos = xsignature.Attribute("Position")?.Value;
                        if (!string.IsNullOrWhiteSpace(xpos))
                            Signatures[index].Position = xpos;
                        var xname = xsignature.Attribute("Name")?.Value;
                        if (!string.IsNullOrWhiteSpace(xname))
                            Signatures[index].Name = xname;
                        var xdate = xsignature.Attribute("Date")?.Value;
                        if (!string.IsNullOrWhiteSpace(xdate))
                            Signatures[index].Date = xdate;
                    }
                }
            }
        }

        private void WriteSignatures(XElement xstamp)
        {
            foreach (var signature in Signatures.Where(x => !x.IsEmpty))
            {
                var xsignature = new XElement("Signature");
                xstamp.Add(xsignature);
                xsignature.Add(new XAttribute("Index", signature.Index));
                xsignature.Add(new XAttribute("Position", signature.Position ?? ""));
                if (!string.IsNullOrWhiteSpace(signature.Name))
                    xsignature.Add(new XAttribute("Name", signature.Name ?? ""));
                if (!string.IsNullOrWhiteSpace(signature.Date))
                    xsignature.Add(new XAttribute("Date", signature.Date ?? ""));
            }
        }
    }

}
