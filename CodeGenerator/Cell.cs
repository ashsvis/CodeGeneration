
namespace CodeGenerator
{
    public class Cell
    {
        private Point node = Point.Empty;
        private PointF[] hori = [PointF.Empty, PointF.Empty];
        private PointF[] vert = [PointF.Empty, PointF.Empty];
        private Rectangle target = Rectangle.Empty;

        public Point Node 
        { 
            get => node; 
            set 
            { 
                if (node == value) return;
                node = value;
                hori = [new PointF(Node.X - 0.5f, Node.Y), new PointF(Node.X + 0.5f, Node.Y)];
                vert = [new PointF(Node.X, Node.Y - 0.5f), new PointF(Node.X, Node.Y + 0.5f)];
                CalculateTarget();
            }
        }

        private void CalculateTarget()
        {
            var xmin = (int)Math.Round(hori.Min(a => a.X));
            var ymin = (int)Math.Round(vert.Min(a => a.Y));
            var xmax = (int)Math.Round(hori.Max(a => a.X));
            var ymax = (int)Math.Round(vert.Max(a => a.Y));
            target = new Rectangle(xmin, ymin, xmax - xmin, ymax - xmin);
            target.Inflate(5, 5);
        }

        public Rectangle Target => target;

        public void Draw(Graphics g)
        {
            g.DrawLines(SystemPens.ControlDarkDark, hori);
            g.DrawLines(SystemPens.ControlDarkDark, vert);
        }
    }
}
