using System.Drawing.Drawing2D;

namespace PluginSupport.Assistants
{
    public static class StamptAssistant
    {
        public static void DrawPageBorder(Graphics? graphics, Color foreground, Page page)
        {
            int kf = 5;
            var borderPen = new Pen(foreground, 0);
            var origin = new Point(page.Column * page.Width, page.Row * page.Height);
            var borderRect = new Rectangle(origin.X * kf - 20 * kf, origin.Y * kf - 5 * kf, page.Width * kf, page.Height * kf);
            graphics?.DrawRectangle(borderPen, borderRect);
            var outRect = new Rectangle(borderRect.X + 20 * kf, borderRect.Y + 5 * kf, borderRect.Width - 25 * kf, borderRect.Height - 10 * kf);
            graphics?.DrawRectangle(borderPen, outRect);
            if (page.Number == 1)
                DrawBigStampt(graphics, borderPen, outRect, foreground, page);
            else
                DrawSmallStampt(graphics, borderPen, outRect, foreground, page);
            DrawSideStampt(graphics, borderPen, outRect, foreground);
        }

        public static (Rectangle, string, int, Func<int, object?>)[] GetStamptTargets(Page page)
        {
            if (page == null) return [];
            var kf = Page.Koeff;
            List<(Rectangle, string, int, Func<int, object?>)> targets = [];
            if (page.Number == 1)
            {
                var rect = new Rectangle(page.Bounds.Right - 185 * kf, page.Bounds.Bottom - 55 * kf, 185 * kf, 55 * kf);
                rect.Offset(-5 * kf, -5 * kf);
                // номер проекта
                targets.Add((new Rectangle(rect.Right - 120 * kf, rect.Top, 120 * kf, 10 * kf), "DocumentNumber", 0, (n) => { return page.DocumentNumber; }));
                // наименование заказчика
                targets.Add((new Rectangle(rect.Right - 120 * kf, rect.Top + 10 * kf, 120 * kf, 15 * kf), "CustomerName", 0, (n) => { return page.CustomerName; }));
                // наименование проекта
                targets.Add((new Rectangle(rect.Right - 120 * kf, rect.Top + 25 * kf, 70 * kf, 15 * kf), "ProjectName", 0, (n) => { return page.ProjectName; }));
                // наименование документа
                targets.Add((new Rectangle(rect.Right - 120 * kf, rect.Top + 40 * kf, 70 * kf, 15 * kf), "DocumentName", 0, (n) => { return page.DocumentName; }));
                // стадия
                targets.Add((new Rectangle(rect.Right - 50 * kf, rect.Top + 30 * kf, 15 * kf, 10 * kf), "DocumentStage", 0, (n) => { return page.DocumentStage; }));
                // наименование исполнителя
                targets.Add((new Rectangle(rect.Right - 50 * kf, rect.Top + 40 * kf, 50 * kf, 15 * kf), "OrganizationName", 0, (n) => { return page.OrganizationName; }));
                var bottom = 15;
                for (var i = 0; i < 4; i++)
                {
                    targets.Add((new Rectangle(rect.Left, rect.Top + bottom * kf, 7 * kf, 5 * kf), $"Changes[{i}].ChangeNumber", i, (i) => { return page.Changes[i].ChangeNumber; }));
                    targets.Add((new Rectangle(rect.Left + 7 * kf, rect.Top + bottom * kf, 10 * kf, 5 * kf), $"Changes[{i}].PartCount", i, (i) => { return page.Changes[i].PartCount; }));
                    targets.Add((new Rectangle(rect.Left + 17 * kf, rect.Top + bottom * kf, 10 * kf, 5 * kf), $"Changes[{i}].MakePage", i, (i => { return page.Changes[i].MakePage; })));
                    targets.Add((new Rectangle(rect.Left + 27 * kf, rect.Top + bottom * kf, 13 * kf, 5 * kf), $"Changes[{i}].DocNumber", i, (i) => { return page.Changes[i].DocNumber; }));
                    targets.Add((new Rectangle(rect.Left + 40 * kf, rect.Top + bottom * kf, 15 * kf, 5 * kf), $"Changes[{i}].Signature", i, (i) => { return ""; /*page.Changes[i].Signature;*/ }));
                    targets.Add((new Rectangle(rect.Left + 55 * kf, rect.Top + bottom * kf, 10 * kf, 5 * kf), $"Changes[{i}].Date", i, (i) => { return page.Changes[i].Date; }));
                    bottom -= 5;
                }
                var top = 25;
                for (var i = 0; i < 6; i++)
                {
                    targets.Add((new Rectangle(rect.Left, rect.Top + top * kf, 17 * kf, 5 * kf), $"Signatures[{i}].Position", i, (i) => { return page.Signatures[i].Position; }));
                    targets.Add((new Rectangle(rect.Left + 17 * kf, rect.Top + top * kf, 23 * kf, 5 * kf), $"Signatures[{i}].Name", i, (i) => { return page.Signatures[i].Name; }));
                    targets.Add((new Rectangle(rect.Left + 40 * kf, rect.Top + top * kf, 15 * kf, 5 * kf), $"Signatures[{i}].Signature", i, (i) => { return ""; /*page.Signatures[i].Signature;*/ }));
                    targets.Add((new Rectangle(rect.Left + 55 * kf, rect.Top + top * kf, 10 * kf, 5 * kf), $"Signatures[{i}].Date", i, (i) => { return page.Signatures[i].Date; }));
                    top += 5;
                }
            }
            else if (page.Number > 1)
            {
                var rect = new Rectangle(page.Bounds.Right - 185 * kf, page.Bounds.Bottom - 15 * kf, 185 * kf, 15 * kf);
                rect.Offset(-5 * kf, -5 * kf);
                // номер проекта
                targets.Add((new Rectangle(rect.Right - 120 * kf, rect.Top, 110 * kf, 15 * kf), "DocumentNumber", 0, (n) => { return page.DocumentNumber; }));
                // изменения: изм колуч лист №док подпись дата
                var bottom = 5;
                for (var i = 0; i < 2; i++)
                {
                    targets.Add((new Rectangle(rect.Left, rect.Top + bottom * kf, 7 * kf, 5 * kf), $"Changes[{i}].ChangeNumber", i, (i) => { return page.Changes[i].ChangeNumber; }));
                    targets.Add((new Rectangle(rect.Left + 7 * kf, rect.Top + bottom * kf, 10 * kf, 5 * kf), $"Changes[{i}].PartCount", i, (i) => { return page.Changes[i].PartCount; }));
                    targets.Add((new Rectangle(rect.Left + 17 * kf, rect.Top + bottom * kf, 10 * kf, 5 * kf), $"Changes[{i}].MakePage", i, (i) => { return page.Changes[i].MakePage; }));
                    targets.Add((new Rectangle(rect.Left + 27 * kf, rect.Top + bottom * kf, 13 * kf, 5 * kf), $"Changes[{i}].DocNumber", i, (i) => { return page.Changes[i].DocNumber; }));
                    targets.Add((new Rectangle(rect.Left + 40 * kf, rect.Top + bottom * kf, 15 * kf, 5 * kf), $"Changes[{i}].Signature", i, (i) => { return ""; /*page.Changes[i].Signature;*/ }));
                    targets.Add((new Rectangle(rect.Left + 55 * kf, rect.Top + bottom * kf, 10 * kf, 5 * kf), $"Changes[{i}].Date", i, (i) => { return page.Changes[i].Date; }));
                    bottom -= 5;
                }
            }
            return [.. targets];
        }

        public static void DrawBigStamptFields(Graphics? graphics, Color foreground, Page? page)
        {
            if (page == null) return;
            var kf = Page.Koeff;
            var rect = new Rectangle(page.Bounds.Right - 185 * kf, page.Bounds.Bottom - 55 * kf, 185 * kf, 55 * kf);
            rect.Offset(-5 * kf, -5 * kf);
            List<(Rectangle, int, Func<int, object?>)> atargets = [];
            List<(Rectangle, int, Func<int, object?>, StringAlignment)> btargets = [];
            // номер проекта
            atargets.Add((new Rectangle(rect.Right - 120 * kf, rect.Top, 120 * kf, 10 * kf), -18, (i) => { return page.DocumentNumber; }));
            // наименование заказчика
            atargets.Add((new Rectangle(rect.Right - 120 * kf, rect.Top + 10 * kf, 120 * kf, 15 * kf), -16, (i) => { return page.CustomerName; }));
            // наименование проекта
            atargets.Add((new Rectangle(rect.Right - 120 * kf, rect.Top + 25 * kf, 70 * kf, 15 * kf), -11, (i) => { return page.ProjectName; }));
            // наименование документа
            atargets.Add((new Rectangle(rect.Right - 120 * kf, rect.Top + 40 * kf, 70 * kf, 15 * kf), -14, (i) => { return page.DocumentName; }));
            // стадия
            atargets.Add((new Rectangle(rect.Right - 50 * kf, rect.Top + 30 * kf, 15 * kf, 10 * kf), -16, (i) => { return page.DocumentStage; }));
            // наименование исполнителя
            atargets.Add((new Rectangle(rect.Right - 50 * kf, rect.Top + 40 * kf, 50 * kf, 15 * kf), -14, (i) => { return page.OrganizationName; }));
             // изменения: изм кол лист №док подпись дата
            var bottom = 15;
            for (var i = 0; i < 4; i++)
            {
                atargets.Add((new Rectangle(rect.Left, rect.Top + bottom * kf, 7 * kf, 5 * kf), i, (n) => { return page.Changes[n].ChangeNumber; }));
                atargets.Add((new Rectangle(rect.Left + 7 * kf, rect.Top + bottom * kf, 10 * kf, 5 * kf), i, (n) => { return page.Changes[n].PartCount; }));
                atargets.Add((new Rectangle(rect.Left + 17 * kf, rect.Top + bottom * kf, 10 * kf, 5 * kf), i, (n) => { return page.Changes[n].MakePage; }));
                atargets.Add((new Rectangle(rect.Left + 27 * kf, rect.Top + bottom * kf, 13 * kf, 5 * kf), i, (n) => { return page.Changes[n].DocNumber; }));
                atargets.Add((new Rectangle(rect.Left + 40 * kf, rect.Top + bottom * kf, 15 * kf, 5 * kf), i, (n) => { return ""; /*page.Changes[n].Signature;*/ }));
                atargets.Add((new Rectangle(rect.Left + 55 * kf, rect.Top + bottom * kf, 10 * kf, 5 * kf), i, (n) => { return page.Changes[n].Date; }));
                bottom -= 5;
            }
           // основные: должность фамилия подпись дата
            var top = 25;
            for (var i = 0; i < 6; i++)
            {
                btargets.Add((new Rectangle(rect.Left, rect.Top + top * kf, 17 * kf, 5 * kf), i, (n) => { return page.Signatures[n].Position; }, StringAlignment.Near));
                btargets.Add((new Rectangle(rect.Left + 17 * kf, rect.Top + top * kf, 23 * kf, 5 * kf), i, (n) => { return page.Signatures[n].Name; }, StringAlignment.Near));
                btargets.Add((new Rectangle(rect.Left + 40 * kf, rect.Top + top * kf, 15 * kf, 5 * kf), i, (n) => { return ""; /*page.Signatures[n].Signature;*/ }, StringAlignment.Center));
                btargets.Add((new Rectangle(rect.Left + 55 * kf, rect.Top + top * kf, 10 * kf, 5 * kf), i, (n) => { return page.Signatures[n].Date; }, StringAlignment.Center));
                top += 5;
            }

            using var brush = new SolidBrush(foreground);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            foreach (var target in atargets)
            {
                var r = target.Item1;
                var index = target.Item2;
                var value = target.Item3(index);
                using var stampfont = new Font("Arial Narrow", index < 0 ? Math.Abs(index) : 2f * kf);
                if (value is string[] lines)
                    graphics?.DrawString(string.Join("\r\n", lines), stampfont, brush, r, sf);
                else
                    graphics?.DrawString($"{value}", stampfont, brush, r, sf);
            }
            foreach (var target in btargets)
            {
                var r = target.Item1;
                var index = target.Item2;
                var value = target.Item3(index);
                sf.Alignment = target.Item4;
                using var stampfont = new Font("Arial Narrow", index < 0 ? Math.Abs(index) : 2f * kf);
                if (value is string[] lines)
                    graphics?.DrawString(string.Join("\r\n", lines), stampfont, brush, r, sf);
                else
                    graphics?.DrawString($"{value}", stampfont, brush, r, sf);
            }
        }

        public static void DrawSmallStamptFields(Graphics? graphics, Color foreground, Page? page)
        {
            if (page == null) return;
            var kf = Page.Koeff;
            var rect = new Rectangle(page.Bounds.Right - 185 * kf, page.Bounds.Bottom - 15 * kf, 185 * kf, 15 * kf);
            rect.Offset(-5 * kf, -5 * kf);
            List<(Rectangle, int, Func<int, string?>)> targets = [];
            // номер проекта
            targets.Add((new Rectangle(rect.Right - 120 * kf, rect.Top, 110 * kf, 15 * kf), -18, (i) => { return page.DocumentNumber; }));
            // изменения: изм колуч лист №док подпись дата
            var bottom = 5;
            for (var i = 0; i < 2; i++)
            {
                targets.Add((new Rectangle(rect.Left, rect.Top + bottom * kf, 7 * kf, 5 * kf), i, (n) => { return page.Changes[n].ChangeNumber; }));
                targets.Add((new Rectangle(rect.Left + 7 * kf, rect.Top + bottom * kf, 10 * kf, 5 * kf), i, (n) => { return page.Changes[n].PartCount; }));
                targets.Add((new Rectangle(rect.Left + 17 * kf, rect.Top + bottom * kf, 10 * kf, 5 * kf), i, (n) => { return page.Changes[n].MakePage; }));
                targets.Add((new Rectangle(rect.Left + 27 * kf, rect.Top + bottom * kf, 13 * kf, 5 * kf), i, (n) => { return page.Changes[n].DocNumber; }));
                targets.Add((new Rectangle(rect.Left + 40 * kf, rect.Top + bottom * kf, 15 * kf, 5 * kf), i, (n) => { return ""; /*page.Changes[n].Signature;*/ }));
                targets.Add((new Rectangle(rect.Left + 55 * kf, rect.Top + bottom * kf, 10 * kf, 5 * kf), i, (n) => { return page.Changes[n].Date; }));
                bottom -= 5;
            }
            using var brush = new SolidBrush(foreground);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            foreach (var target in targets)
            {
                var r = target.Item1;
                var index = target.Item2;
                var value = target.Item3(index);
                using var stampfont = new Font("Arial Narrow", index < 0 ? Math.Abs(index) : 2f * kf);
                graphics?.DrawString(value, stampfont, brush, r, sf);
            }
        }

        public static void DrawBigStampt(Graphics? graphics, Pen borderPen, Rectangle outRect, Color foreground, Page? page)
        {
            var kf = Page.Koeff;
            var stamptRect = new Rectangle(outRect.Right - 185 * kf, outRect.Bottom - 55 * kf, 185 * kf, 55 * kf);
            graphics?.DrawLine(borderPen, stamptRect.Location, new PointF(stamptRect.Right, stamptRect.Top));
            //if (Module.Size != PaperSize.A4 || Module.Orientation != PaperOrientation.Portrait)
            graphics?.DrawLine(borderPen, stamptRect.Location, new PointF(stamptRect.Left, stamptRect.Bottom));

            int[] steps = [7 * kf, 10 * kf, 10 * kf, 13 * kf, 15 * kf, 10 * kf];
            string[] names = ["Изм.", "Кол.уч", "Лист", "№ док.", "Подпись", "Дата"];
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            using var stampfont = new Font("Arial Narrow", 2f * kf);
            using var brush = new SolidBrush(foreground);
            var x = 0 * kf;
            for (var i = 0; i < steps.Length; i++)
            {
                x += steps[i];
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left + x, stamptRect.Top), new Point(stamptRect.Left + x, stamptRect.Top + 25 * kf));
                graphics?.DrawString(names[i], stampfont, brush,
                    new Point(stamptRect.Left + x - steps[i] / 2, stamptRect.Top + 23 * kf), sf);
            }
            var y = 0 * kf;
            for (var i = 0; i < 10; i++)
            {
                y += 5 * kf;
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left, stamptRect.Top + y),
                    new Point(stamptRect.Left + 65 * kf, stamptRect.Top + y));
            }
            steps = [17 * kf, 23 * kf, 15 * kf, 10 * kf];
            x = 0;
            for (var i = 0; i < steps.Length; i++)
            {
                x += steps[i];
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left + x, stamptRect.Top + 25 * kf), new Point(stamptRect.Left + x, stamptRect.Bottom));
            }
            y = stamptRect.Top + 10 * kf;
            for (var i = 0; i < 3; i++)
            {
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left + 65 * kf, y), new PointF(stamptRect.Right, y));
                y += 15 * kf;
            }
            steps = [15 * kf, 15 * kf, 20 * kf];
            names = ["Стадия", "Лист", "Листов"];
            sf.Alignment = StringAlignment.Center;
            x = stamptRect.Right - 50 * kf;
            graphics?.DrawLine(borderPen, new Point(x, stamptRect.Top + 25 * kf), new Point(x, stamptRect.Bottom));
            graphics?.DrawLine(borderPen, new Point(x, stamptRect.Top + 30 * kf), new Point(stamptRect.Right, stamptRect.Top + 30 * kf));
            for (var i = 0; i < steps.Length; i++)
            {
                x += steps[i];
                graphics?.DrawString(names[i], stampfont, brush,
                    new Point(x - steps[i] / 2, stamptRect.Top + 28 * kf), sf);
                if (i == steps.Length - 1) continue;
                graphics?.DrawLine(borderPen, new Point(x, stamptRect.Top + 25 * kf), new Point(x, stamptRect.Top + 40 * kf));
            }
            using var bigfont = new Font(stampfont.Name, 3f * kf);
            var pageNumRect = new Rectangle(stamptRect.Right - 35 * kf, stamptRect.Bottom - 25 * kf, 15 * kf, 10 * kf);
            graphics?.DrawString($"{page?.Number}", bigfont, brush,
                new Point(pageNumRect.X + pageNumRect.Width / 2, pageNumRect.Y + pageNumRect.Height / 2), sf);
            var pagesCountRect = new Rectangle(stamptRect.Right - 20 * kf, stamptRect.Bottom - 25 * kf, 20 * kf, 10 * kf);
            graphics?.DrawString($"{page?.PagesCount}", bigfont, brush,
                new Point(pagesCountRect.X + pagesCountRect.Width / 2, pagesCountRect.Y + pagesCountRect.Height / 2), sf);
            graphics?.DrawString("Формат A3", stampfont, brush,
                new Point(outRect.Right - 25 * kf, outRect.Bottom + 2 * kf), sf);
        }

        public static void DrawSmallStampt(Graphics? graphics, Pen borderPen, Rectangle outRect, Color foreground, Page? page)
        {
            var kf = Page.Koeff;
            var stamptRect = new Rectangle(outRect.Right - 185 * kf, outRect.Bottom - 15 * kf, 185 * kf, 15 * kf);
            graphics?.DrawLine(borderPen, stamptRect.Location, new Point(stamptRect.Right, stamptRect.Top));
            graphics?.DrawLine(borderPen, stamptRect.Location, new Point(stamptRect.Left, stamptRect.Bottom));
            int[] steps = [7 * kf, 10 * kf, 10 * kf, 13 * kf, 15 * kf, 10 * kf, 110 * kf];
            string[] names = ["Изм.", "Кол.уч.", "Лист", "№ док.", "Подп.", "Дата", ""];
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var stampfont = new Font("Arial Narrow", 2f * kf);
            var brush = new SolidBrush(foreground);
            var x = 0 * kf;
            for (var i = 0; i < steps.Length; i++)
            {
                x += steps[i];
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left + x, stamptRect.Top), new Point(stamptRect.Left + x, stamptRect.Bottom));
                graphics?.DrawString(names[i], stampfont, brush,
                    new Point(stamptRect.Left + x - steps[i] / 2, stamptRect.Bottom - 2 * kf), sf);
            }
            var y = 0 * kf;
            for (var i = 0; i < 3; i++)
            {
                y += 5 * kf;
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left, stamptRect.Top + y),
                    new Point(stamptRect.Left + 65 * kf, stamptRect.Top + y));
            }
            var pageRect = new Rectangle(outRect.Right - 10 * kf, outRect.Bottom - 15 * kf, 10 * kf, 7 * kf);
            using var bigfont = new Font(stampfont.Name, 3f * kf);
            var pageNumRect = new Rectangle(outRect.Right - 10 * kf, outRect.Bottom - 8 * kf, 10 * kf, 8 * kf);
            graphics?.DrawLine(borderPen, pageNumRect.Location, new PointF(pageNumRect.Right, pageNumRect.Top));

            graphics?.DrawString("Лист", stampfont, brush,
                new Point(pageRect.X + pageRect.Width / 2, pageRect.Y + pageRect.Height / 2), sf);
            if (true)
            {
                graphics?.DrawString($"{page?.Number}", bigfont, brush,
                    new Point(pageNumRect.X + pageNumRect.Width / 2, pageNumRect.Y + pageNumRect.Height / 2), sf);
            }
            graphics?.DrawString("Формат A3", stampfont, brush,
                new Point(outRect.Right - 25 * kf, outRect.Bottom + 2 * kf), sf);
        }

        public static void DrawSideStampt(Graphics? graphics, Pen borderPen, Rectangle outRect, Color foreground)
        {
            var kf = Page.Koeff;
            int[] steps = [25 * kf, 35 * kf, 25 * kf];
            string[] names = ["Инв.№ подл.", "Подп. и дата", "Взам. инв. №"];
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            using var stampfont = new Font("Arial Narrow", 3f * kf);
            using var brush = new SolidBrush(foreground);
            var y = outRect.Bottom;
            for (var i = 0; i < steps.Length; i++)
            {
                graphics?.DrawLine(borderPen, new PointF(outRect.Left - 12 * kf, y), new Point(outRect.Left, y));
                y -= steps[i];
                var p = new Point(outRect.Left - 9 * kf, y + steps[i] / 2);
                DrawVerticalText(graphics, p, names[i], stampfont, brush, sf);
            }
            graphics?.DrawLine(borderPen, new Point(outRect.Left - 12 * kf, outRect.Bottom),
                new Point(outRect.Left - 12 * kf, outRect.Bottom - 85 * kf));
            graphics?.DrawLine(borderPen, new PointF(outRect.Left - 7 * kf, outRect.Bottom),
                new Point(outRect.Left - 7 * kf, outRect.Bottom - 85 * kf));
            DrawVerticalText(graphics, new Point(outRect.Left - 17 * kf, y - 1 * kf), "Согласовано", stampfont, brush,
                new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
            steps = [20 * kf, 20 * kf, 15 * kf];
            graphics?.DrawLine(borderPen, new Point(outRect.Left - 20 * kf, y), new PointF(outRect.Left, y));
            for (var i = 0; i < steps.Length; i++)
            {
                y -= steps[i];
                graphics?.DrawLine(borderPen, new Point(outRect.Left - 15 * kf, y), new Point(outRect.Left, y));
            }
            y -= 10 * kf;
            graphics?.DrawLine(borderPen, new Point(outRect.Left - 20 * kf, y), new Point(outRect.Left, y));
            var x = outRect.Left - 15 * kf;
            for (var i = 0; i < 3; i++)
            {
                graphics?.DrawLine(borderPen, new Point(x, outRect.Bottom - 85 * kf),
                    new Point(x, outRect.Bottom - 150 * kf));
                x += 5 * kf;
            }
        }

        public static void DrawVerticalText(Graphics? graphics, Point p, string text, Font stampfont, SolidBrush brush, StringFormat sf)
        {
            using var path = new GraphicsPath();
            path.AddString(text, stampfont.FontFamily, 0, stampfont.Size, p, sf);
            using var rotateMatrix = new Matrix();
            rotateMatrix.RotateAt(-90f, p);
            path.Transform(rotateMatrix);
            graphics?.FillPath(brush, path);
        }

    }
}
