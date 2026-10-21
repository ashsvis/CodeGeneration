using PluginSupport;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace LogicModel
{
    public class DigitalOutputTag : Func, IAcceptClick, INamedEmulationSource
    {
        public string? TagName { get; set; }

        public DigitalOutputTag()
        {
            FuncDesc = "Тег дискретного выхода";
            FuncName = "TAG_DO";
            Inputs = [new PinInput { Kind = ValueKind.DO }];
            Outputs = [];
            Background = SystemColors.Control;
            Width = 48 * 3;
            CalculateHeight();
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowBorder |
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        public bool CanAcceptClick => true;

        public double Value => Inputs.Length > 0 ? Inputs[0].Value : 0;

        public override DigitalOutputTag DeepClone()
        {
            return new DigitalOutputTag()
            {
                id = Id,
                Index = this.Index,
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [],
                TagName = TagName,
            };
        }

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var paths = base.GetGraphicsPaths().ToList();
            var rect = Bounds;
            var path = new GraphicsPath();
            path.AddArc(new Rectangle(rect.X, rect.Y, rect.Height, rect.Height), 90f, 180f);
            path.AddArc(new Rectangle(rect.X + rect.Width - rect.Height, rect.Y, rect.Height, rect.Height), 270f, 180f);
            path.CloseFigure();
            paths.Add(path);
            return [.. paths];
        }

        public override GraphicsPath[] GetTextPaths()
        {
            var paths = base.GetTextPaths().ToList();
            var rect = Bounds;
            rect.Inflate(rect.Height / 4, 0);
            var fname = TagName ?? FuncName ?? "";
            using var fontFunc = new Font("Consolas", 14f);
            var sz = TextRenderer.MeasureText(fname, fontFunc);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            var path = new GraphicsPath();
            path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Bold, fontFunc.Size, rect, sf);
            paths.Add(path);
            return [.. paths];
        }

        public override void DrawWhenDebug(Graphics? g, Brush defaultBrush)
        {
            if (Inputs.Length > 0)
            {
                using var brushOff = new SolidBrush(Color.FromArgb(100, Color.Red));
                using var brushOn = new SolidBrush(Color.FromArgb(100, Color.Lime));
                foreach (var p in GetGraphicsPaths())
                {
                    using var path = p;
                    g?.FillPath(Inputs[0].Value > 0 ? brushOn : brushOff, path);
                }
            }
        }

        public override void Click(bool runEmulate, Point? point = null)
        {
            if (!runEmulate)
                Click(ChangeText, point ?? Point.Empty);
        }

        private void ChangeText(MouseButtons mouseButtons, params object[] args)
        {
            var dlg = new InputValueForm { Text = "Имя тега", StrValue = TagName ?? "" };
            if (args.Length > 0 && args[0] is Point location)
            {
                dlg.StartPosition = FormStartPosition.Manual;
                dlg.Location = location;
            }
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                TagName = dlg.StrValue;
            }
        }

        public override XElement WriteContent()
        {
            var xfunc = base.WriteContent();
            if (!string.IsNullOrEmpty(TagName))
            {
                var xname = new XElement("Name", TagName);
                xfunc.Add(xname);
            }
            return xfunc;
        }

        public override void ReadContent(XElement xfunc)
        {
            base.ReadContent(xfunc);
            var xName = xfunc.Element("Name");
            if (xName != null)
            {
                TagName = xName.Value;
            }
        }
    }
}
