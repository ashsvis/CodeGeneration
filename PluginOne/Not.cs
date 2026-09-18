namespace PluginOne
{
    public class Not : Cell
    {
        public Not() 
        {
            Inputs = [false];
            Outputs = [true];
        }



        //public override void Draw(Graphics? g, Pen? pen = null, Brush? brush = null)
        //{
        //    if (g == null) return;
        //    base.Draw(g, pen, brush);
        //    var rect = new RectangleF(Location.X - Width / 2f, Location.Y - Height / 2f, Width, Height);
        //    using var sf = new StringFormat();
        //    sf.Alignment = StringAlignment.Center;
        //    sf.LineAlignment = StringAlignment.Center;
        //    g.DrawString(Location.ToString(), SystemFonts.DefaultFont, SystemBrushes.ControlText, rect, sf);
        //}
    }
}
