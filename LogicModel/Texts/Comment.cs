using PluginSupport;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace LogicModel
{
    public class Comment : Func, ITraceTransparent, IAcceptClick
    {
        public string[] Lines = [""];

        public Comment()
        {
            FuncDesc = "Комментарий";
            FuncName = "COMMENT";
            Width = 48 * 4;
            Height = 48 / 8;
            Inputs = [];
            Outputs = [];
            Background = SystemColors.Control;
            CalculateHeight();
        }

        public override Comment DeepClone()
        {
            return new Comment()
            {
                id = Id,
                Index = this.Index,
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [],
                Outputs = [],
                Lines = Lines,
            };
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowPins | AllowedFuncProperties.ShowBorder |
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        private bool IsEmpty => Lines.Length == 0 || Lines.Length == 1 && string.IsNullOrWhiteSpace(Lines[0]);

        public bool CanAcceptClick => true;

        protected override void CalculateHeight()
        {
            var fname = IsEmpty ? FuncName ?? "" : string.Join("\n", Lines);
            using var fontFunc = new Font("Arial Narrow", 14f);
            var sz = TextRenderer.MeasureText(fname, fontFunc);
            Width = (int)(sz.Width / 1.3);
            Height = 6;
        }

        [Browsable(false)]
        public override Rectangle Bounds
        {
            get
            {
                CalculateHeight();
                return new(Location.X, Location.Y, Width, Height);
            }
        }

        public override GraphicsPath[] GetTextPaths()
        {
            var paths = base.GetTextPaths().ToList();
            var rect = Bounds;
            var fname = IsEmpty ? FuncName ?? "" : string.Join("\n", Lines);
            using var fontFunc = new Font("Arial Narrow", 14f);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            var path = new GraphicsPath();
            path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Regular, fontFunc.Size, new Point(rect.Left, rect.Top + rect.Height / 2), sf);
            paths.Add(path);
            return [.. paths];
        }

        public override void Click(bool runEmulate, Point? point = null)
        {
            if (!runEmulate)
                Click(ChangeText, point ?? Point.Empty);
        }

        private void ChangeText(MouseButtons mouseButtons, params object[] args)
        {
            var dlg = new InputValueForm { Text = FuncDesc, LinesValue = Lines };
            if (args.Length > 0 && args[0] is Point location)
            {
                dlg.StartPosition = FormStartPosition.Manual;
                dlg.Location = location;
            }
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Lines = dlg.LinesValue;
                Changed();
            }
        }

        public override XElement WriteContent()
        {
            var xfunc = base.WriteContent();
            if (!IsEmpty)
            {
                var xlines = new XElement("Lines");
                xfunc.Add(xlines);
                foreach (var line in Lines)
                    xlines.Add(new XElement("Line", line));
            }
            return xfunc;
        }

        public override void ReadContent(XElement xfunc)
        {
            base.ReadContent(xfunc);
            var xlines = xfunc.Element("Lines");
            if (xlines != null)
            {
                List<string> lines = [];
                foreach (var xline in xlines.Elements("Line"))
                    lines.Add(xline.Value);
                Lines = [.. lines];
            }
            else
                Lines = [];
        }
    }
}
