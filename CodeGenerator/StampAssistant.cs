using System.Drawing.Drawing2D;

namespace CodeGenerator
{
    public static class StampAssistant
    {
        public static void DrawPageBorder(Graphics? graphics, Point origin, int width, int height, bool bigStamp = false)
        {
            int kf = 5;
            var borderPen = new Pen(Color.FromArgb(127, Color.Gray), 0)
            {
                DashStyle = DashStyle.Dash
            };
            var borderRect = new Rectangle(origin.X * kf - 20 * kf, origin.Y * kf - 5 * kf, width * kf, height * kf);
            graphics?.DrawRectangle(borderPen, borderRect);
            var outRect = new Rectangle(borderRect.X + 20 * kf, borderRect.Y + 5 * kf,
                            borderRect.Width - 25 * kf, borderRect.Height - 10 * kf);
            graphics?.DrawRectangle(borderPen, outRect);
            if (bigStamp)
                StampAssistant.DrawBigStamp(graphics, borderPen, outRect, kf);
            else
                StampAssistant.DrawSmallStamp(graphics, borderPen, outRect, kf);
            StampAssistant.DrawSideStamp(graphics, borderPen, outRect, kf);
        }

        public static void DrawBigStamp(Graphics? graphics, Pen borderPen, Rectangle outRect, int kf)
        {
            var stamptRect = new Rectangle(outRect.Right - 185 * kf, outRect.Bottom - 55 * kf, 185 * kf, 55 * kf);
            graphics?.DrawLine(borderPen, stamptRect.Location, new PointF(stamptRect.Right, stamptRect.Top));
            //if (Module.Size != PaperSize.A4 || Module.Orientation != PaperOrientation.Portrait)
            graphics?.DrawLine(borderPen, stamptRect.Location, new PointF(stamptRect.Left, stamptRect.Bottom));

            int[] steps = [10 * kf, 10 * kf, 10 * kf, 10 * kf, 15 * kf, 10 * kf];
            string[] names = ["Изм.", "Кол.уч", "Лист", "№ док.", "Подпись", "Дата"];
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            using var stampfont = new Font("Arial", 2f * kf);
            using var brush = new SolidBrush(Color.FromArgb(127, Color.Gray));
            var x = 0 * kf;
            for (var i = 0; i < steps.Length; i++)
            {
                x += steps[i];
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left + x, stamptRect.Top), new Point(stamptRect.Left + x, stamptRect.Top + 25 * kf));
                graphics?.DrawString(names[i], stampfont, brush,
                    new Point(stamptRect.Left + x - (steps[i] / 2), stamptRect.Top + 23 * kf), sf);
            }
            var y = 0 * kf;
            for (var i = 0; i < 10; i++)
            {
                y += 5 * kf;
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left, stamptRect.Top + y),
                    new Point(stamptRect.Left + 65 * kf, stamptRect.Top + y));
            }
            steps = [20 * kf, 20 * kf, 15 * kf, 10 * kf];
            names = ["Разраб.", "Пров.", "Т.контр.", "", "Н.контр.", "Утв."];
            x = 0;
            for (var i = 0; i < steps.Length; i++)
            {
                x += steps[i];
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left + x, stamptRect.Top + 25 * kf), new Point(stamptRect.Left + x, stamptRect.Bottom));
            }
            sf.Alignment = StringAlignment.Near;
            y = stamptRect.Top + 23 * kf;
            for (var i = 0; i < names.Length; i++)
            {
                y += 5 * kf;
                graphics?.DrawString(names[i], stampfont, brush, new Point(stamptRect.Left + 1 * kf, y), sf);
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
                    new Point(x - (steps[i] / 2), stamptRect.Top + 28 * kf), sf);
                if (i == steps.Length - 1) continue;
                graphics?.DrawLine(borderPen, new Point(x, stamptRect.Top + 25 * kf), new Point(x, stamptRect.Top + 40 * kf));
            }
            using var bigfont = new Font(stampfont.Name, 3f * kf);
            var pageNumRect = new Rectangle(stamptRect.Right - 35 * kf, stamptRect.Bottom - 25 * kf, 15 * kf, 10 * kf);
            graphics?.DrawString("1", bigfont, brush,
                new Point(pageNumRect.X + pageNumRect.Width / 2, pageNumRect.Y + pageNumRect.Height / 2), sf);
            var pagesCountRect = new Rectangle(stamptRect.Right - 20 * kf, stamptRect.Bottom - 25 * kf, 20 * kf, 10 * kf);
            graphics?.DrawString("3", bigfont, brush,
                new Point(pagesCountRect.X + pagesCountRect.Width / 2, pagesCountRect.Y + pagesCountRect.Height / 2), sf);
            graphics?.DrawString("Формат A3", stampfont, brush,
                new Point(outRect.Right - 25 * kf, outRect.Bottom + 2 * kf), sf);
        }

        public static void DrawSmallStamp(Graphics? graphics, Pen borderPen, Rectangle outRect, int kf)
        {
            var stamptRect = new Rectangle(outRect.Right - 185 * kf, outRect.Bottom - 15 * kf, 185 * kf, 15 * kf);
            graphics?.DrawLine(borderPen, stamptRect.Location, new Point(stamptRect.Right, stamptRect.Top));
            graphics?.DrawLine(borderPen, stamptRect.Location, new Point(stamptRect.Left, stamptRect.Bottom));
            int[] steps = [7 * kf, 10 * kf, 23 * kf, 15 * kf, 10 * kf, 110 * kf];
            string[] names = ["Изм.", "Лист", "№ докум.", "Подп.", "Дата", ""];
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var stampfont = new Font("Arial", 2f * kf);
            var brush = new SolidBrush(Color.FromArgb(127, Color.Gray));
            var x = 0 * kf;
            for (var i = 0; i < steps.Length; i++)
            {
                x += steps[i];
                graphics?.DrawLine(borderPen, new Point(stamptRect.Left + x, stamptRect.Top), new Point(stamptRect.Left + x, stamptRect.Bottom));
                graphics?.DrawString(names[i], stampfont, brush,
                    new Point(stamptRect.Left + x - (steps[i] / 2), stamptRect.Bottom - 2 * kf), sf);
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
                graphics?.DrawString("2", bigfont, brush,
                    new Point(pageNumRect.X + pageNumRect.Width / 2, pageNumRect.Y + pageNumRect.Height / 2), sf);
            }
            graphics?.DrawString("Формат A3", stampfont, brush,
                new Point(outRect.Right - 25 * kf, outRect.Bottom + 2 * kf), sf);
        }

        public static void DrawSideStamp(Graphics? graphics, Pen borderPen, Rectangle outRect, int kf)
        {
            int[] steps = [25 * kf, 35 * kf, 25 * kf];
            string[] names = ["Инв.№ подл.", "Подп. и дата", "Взам. инв. №"];
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            using var stampfont = new Font("Arial", 3f * kf);
            using var brush = new SolidBrush(Color.FromArgb(127, Color.Gray));
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
