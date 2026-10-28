using PluginSupport;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace LogicModel
{
    public class FuncPresenter : Shape, ITraceTransparent, IAcceptClick, IGroupToPlace
    {
        private Shape? func = null;

        [Browsable(false)]
        public override int Width { get; set; } = 24;

        [Browsable(false)]
        public override int Height { get; set; } = 24;

        [Browsable(false)]
        public override Rectangle Bounds 
        {
            get
            {
                var paths = GetGraphicsPaths();
                var rect = paths.First().GetBounds();
                foreach (var path in paths.Skip(1))
                    rect = RectangleF.Union(rect, path.GetBounds());
                Width = (int)rect.Width;
                Height = (int)rect.Height;
                return new(Location.X, Location.Y, Width, Height);
            }
        }

        public bool CanAcceptClick => false;

        public override FuncPresenter DeepClone()
        {
            return new FuncPresenter
            {
                id = Id,
                func = func?.DeepClone(),
                Location = Location,
                Width = Width,
                Height = Height,
            };
        }

        public override GraphicsPath[] GetGraphicsPaths()
        {
            List<GraphicsPath> paths = [];
            if (func == null)
            {
                GraphicsPath path = new();
                path.AddLine(new Point(Location.X, Location.Y), new Point(Location.X + Width, Location.Y));
                path.CloseFigure();
                path.AddLine(new Point(Location.X, Location.Y), new Point(Location.X, Location.Y + Height));
                path.CloseFigure();
                paths.Add(path);
                return [.. paths];
            }
            func.Location = Location;
            paths.AddRange(func.GetGraphicsPaths());
            return [.. paths];
        }

        public override GraphicsPath[] GetTextPaths()
        {
            if (func == null) 
                return base.GetTextPaths();
            func.Location = Location;
            var paths = func.GetTextPaths().ToList();
            var rect = Bounds;
            rect.Height = 24;
            rect.Inflate(24, 0);
            rect.Offset(-10, -30);
            using var fontFunc = new Font("Arial Narrow", 10f);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            var path = new GraphicsPath();
            if (func is IFuncPresenter funcPresenter)
            {
                var fname = (funcPresenter.FuncDesc ?? "").Split('/').First();
                var sz = TextRenderer.MeasureText(fname, fontFunc);
                path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Regular, fontFunc.Size, rect, sf);
                paths.Add(path);

                rect.Offset(0, Bounds.Height + 40);
                rect.Height += 100;
                sf.LineAlignment = StringAlignment.Near;
                path = new GraphicsPath();
                fname = string.Join("\n", funcPresenter.FuncPresenter);
                sz = TextRenderer.MeasureText(fname, fontFunc, sz);
                path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Regular, fontFunc.Size, rect, sf);
                paths.Add(path);
            }
            return [..paths];
        }

        public override void ReadContent(XElement xfunc)
        {
            if (xfunc == null || xfunc.Name != $"{this.GetType().FullName}") return;
            var sId = xfunc.Attribute("Id")?.Value;
            if (!string.IsNullOrWhiteSpace(sId) && Guid.TryParse(sId, out Guid objId))
                id = objId;
            var sEmbeded = xfunc.Attribute("Embeded")?.Value;
            if (!string.IsNullOrWhiteSpace(sEmbeded))
            {
                var pl = new PluginLogic();
                if (pl.GetTypes().ContainsKey(sEmbeded))
                {
                    func = (Func?)Activator.CreateInstance(pl.GetTypes()[sEmbeded]);
                }
            }
            var sLeft = xfunc.Attribute("X")?.Value;
            var sTop = xfunc.Attribute("Y")?.Value;
            if (!string.IsNullOrWhiteSpace(sLeft) && !string.IsNullOrWhiteSpace(sTop))
                Location = new Point(ParseHelper.ParseInteger(sLeft, 0), ParseHelper.ParseInteger(sTop, 0));
        }

        public override XElement WriteContent()
        {
            var xfunc = new XElement($"{this.GetType().FullName}");
            xfunc.Add(new XAttribute("Id", Id));
            if (func != null)
                xfunc.Add(new XAttribute("Embeded", $"{func.GetType().FullName}"));
            xfunc.Add(new XAttribute("X", Location.X));
            xfunc.Add(new XAttribute("Y", Location.Y));
            return xfunc;
        }

        public override void Click(bool runEmulate, Point? point = null)
        {
            if (!runEmulate)
                Click(ChangePresendedFunction, point ?? Point.Empty);
        }

        private void ChangePresendedFunction(MouseButtons mouseButtons, params object[] args)
        {
            var dlg = new FuncPresenterTuningForm(func) { };
            if (args.Length > 0 && args[0] is Point location)
            {
                dlg.StartPosition = FormStartPosition.Manual;
                dlg.Location = location;
            }
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                func = dlg.Selected.First();
                Changed();
            }
        }

        public Shape[] GetShapes()
        {
            List<Func> funcs = [];
            var pl = new PluginLogic();
            foreach (var entry in pl.GetTypes())
            {
                var type = entry.Value;
                var obj = Activator.CreateInstance(type);
                if (obj is IFuncPresenter presenter && presenter.IsPresent)
                    funcs.Add((Func)obj);
            }
            return [.. funcs];
        }

        public void SetPresentFunction(Shape shape)
        {
            func = (Shape?)Activator.CreateInstance(shape.GetType());
        }
    }
}