namespace CodeGenerator
{
    public class Page(int number, int width, int height)
    {
        public int Number { get; set; } = number;
        public int PagesCount { get; set; }
        public int Width { get; set; } = width;
        public int Height { get; set; } = height;

        public Rectangle Bounds => new((Number - 1) * Width, 0, Width, Height);

        public void Draw(Graphics? graphics, Color foreground)
        {
            StampAssistant.DrawPageBorder(graphics, new Point((Number - 1) * Width, 0), Width, Height, foreground, Number, PagesCount);
        }
    }
}
