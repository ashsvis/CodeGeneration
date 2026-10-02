
namespace CodeGenerator
{
    public class Cell
    {
        private Point node = Point.Empty;
        private Point[] hori = [Point.Empty, Point.Empty];
        private Point[] vert = [Point.Empty, Point.Empty];
        private Rectangle target = Rectangle.Empty;

        public Point Node 
        { 
            get => node; 
            set 
            { 
                if (!value.IsEmpty && node == value) return;
                node = value;
                hori = [new Point(Node.X - 1, Node.Y), new Point(Node.X + 1, Node.Y)];
                vert = [new Point(Node.X, Node.Y - 1), new Point(Node.X, Node.Y + 1)];
                CalculateTarget();
            }
        }

        public bool Empty { get; set; } = true;

        public int Wave { get; set; }

        private void CalculateTarget()
        {
            var xmin = hori.Min(a => a.X);
            var ymin = vert.Min(a => a.Y);
            var xmax = hori.Max(a => a.X);
            var ymax = vert.Max(a => a.Y);
            target = new Rectangle(xmin, ymin, xmax - xmin, ymax - xmin);
            target.Inflate(5, 5);
        }

        public Rectangle Target => target;

        public void Draw(Graphics? g)
        {
            if (Wave == 0)
            {
                using Pen pen = new(Empty ? SystemColors.ControlDarkDark : Color.Red, 0);
                g?.DrawLines(pen, hori);
                g?.DrawLines(pen, vert);
            }
            else
            {
                using var sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g?.DrawString($"{Wave}", SystemFonts.DefaultFont, SystemBrushes.ControlDark, Node, sf);
            }
        }

        public override string ToString()
        {
            return Wave.ToString();
        }
    }
}
