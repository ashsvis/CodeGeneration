using PluginSupport;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace LogicModel
{
    public class FuncPresenter : Shape, ITraceTransparent, IAcceptClick
    {
        private Func? func = null;

        [Browsable(false)]
        public override int Width { get; set; } = 24;

        [Browsable(false)]
        public override int Height { get; set; } = 24;

        [Browsable(false)]
        public override Rectangle Bounds => new(Location.X, Location.Y, Width, Height);

        public bool CanAcceptClick => false;

        public override FuncPresenter DeepClone()
        {
            return new FuncPresenter
            {
                Width = Width,
                Height = Height,
            };
        }

        public override GraphicsPath[] GetGraphicsPaths()
        {
            if (func == null)
            {
                List<GraphicsPath> paths = [];
                GraphicsPath path = new();
                path.AddLine(new Point(Location.X, Location.Y), new Point(Location.X + Width, Location.Y));
                path.CloseFigure();
                path.AddLine(new Point(Location.X, Location.Y), new Point(Location.X, Location.Y + Height));
                path.CloseFigure();
                paths.Add(path);
                return [.. paths];
            }
            func.Location = Location;
            return func.GetGraphicsPaths();
        }

        public override void ReadContent(XElement xfunc)
        {
            if (xfunc == null || xfunc.Name != $"{this.GetType().FullName}") return;
            var sId = xfunc.Attribute("Id")?.Value;
            if (!string.IsNullOrWhiteSpace(sId) && Guid.TryParse(sId, out Guid objId))
                id = objId;
            var sLeft = xfunc.Attribute("X")?.Value;
            var sTop = xfunc.Attribute("Y")?.Value;
            if (!string.IsNullOrWhiteSpace(sLeft) && !string.IsNullOrWhiteSpace(sTop))
                Location = new Point(ParseHelper.ParseInteger(sLeft, 0), ParseHelper.ParseInteger(sTop, 0));
            var sWidth = xfunc.Attribute("Width")?.Value;
            if (!string.IsNullOrWhiteSpace(sWidth))
                Width = ParseHelper.ParseInteger(sWidth, 24);
            var sHeight = xfunc.Attribute("Height")?.Value;
            if (!string.IsNullOrWhiteSpace(sHeight))
                Height = ParseHelper.ParseInteger(sHeight, 24);
        }

        public override XElement WriteContent()
        {
            var xfunc = new XElement($"{this.GetType().FullName}");
            xfunc.Add(new XAttribute("Id", Id));
            xfunc.Add(new XAttribute("X", Location.X));
            xfunc.Add(new XAttribute("Y", Location.Y));
            xfunc.Add(new XAttribute("Width", Width));
            xfunc.Add(new XAttribute("Height", Height));
            return xfunc;
        }

        public override void Click(bool runEmulate, Point? point = null)
        {
            if (!runEmulate)
                Click(ChangeText, point ?? Point.Empty);
        }

        private void ChangeText(MouseButtons mouseButtons, params object[] args)
        {
            var dlg = new FuncPresenterTuningForm { };
            if (args.Length > 0 && args[0] is Point location)
            {
                dlg.StartPosition = FormStartPosition.Manual;
                dlg.Location = location;
            }
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                func = dlg.Selected;
                if (func != null)
                {
                    Width = func.Width;
                    Height = func.Height;
                }
                Changed();
            }
        }
    }
}