
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

        public ThroughPassage Empty { get; set; } = ThroughPassage.Both;

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
                var color = Empty switch
                {
                    ThroughPassage.None => Color.Red,
                    ThroughPassage.Vertical or ThroughPassage.Horizontal => Color.Gray,
                    _ => SystemColors.ControlDarkDark,
                };
                using Pen pen = new(color, 0);
                if (Empty.HasFlag(ThroughPassage.Vertical))
                    g?.DrawLines(pen, hori);
                if (Empty.HasFlag(ThroughPassage.Horizontal))
                    g?.DrawLines(pen, vert);
                if (Empty.HasFlag(ThroughPassage.Both) || Empty == ThroughPassage.None)
                {
                    g?.DrawLines(pen, hori);
                    g?.DrawLines(pen, vert);
                }
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
