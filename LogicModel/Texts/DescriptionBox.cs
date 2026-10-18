using PluginSupport;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace LogicModel
{
    public class DescriptionBox : Func
    {
        public string[] Lines = [""];

        public DescriptionBox()
        {
            FuncDesc = "Описание параметра";
            FuncName = "TAG_DESCRIPTION";
            Width = 48 * 5;
            Height = 48 * 2;
            Inputs = [new PinInput { }];
            Outputs = [new PinOutput { }];
            Background = SystemColors.Control;
            CalculateHeight();
        }

        public override DescriptionBox DeepClone()
        {
            return new DescriptionBox()
            {
                id = Id,
                Index = this.Index,
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
                Lines = Lines,
            };
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowPins |
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        private bool IsEmpty => Lines.Length == 0 || Lines.Length == 1 && string.IsNullOrWhiteSpace(Lines[0]);

        public override GraphicsPath[] GetTextPaths()
        {
            var paths = base.GetTextPaths().ToList();
            var rect = Bounds;
            var fname = IsEmpty ? FuncName ?? "" : string.Join("\n", Lines);
            using var fontFunc = new Font("Segoe UI", 14f);
            var sz = TextRenderer.MeasureText(fname, fontFunc);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            var path = new GraphicsPath();
            path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Bold, fontFunc.Size, rect, sf);
            paths.Add(path);
            return [.. paths];
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    Transfer();
                }
            }
        }

        public override PinInfo[] GetPinPoints()
        {
            var infos = base.GetPinPoints();
            var sizeTarget = BaseWidth / 4;
            foreach (var pin in infos)
            {
                if (pin.PinPoint is Point point)
                {
                    if (pin.IsOutput)
                        pin.PinPoint = Point.Subtract(point, new Size(sizeTarget, 0));
                    else
                        pin.PinPoint = Point.Add(point, new Size(sizeTarget, 0));
                }
            }
            return infos;
        }

        public override void Click(bool runEmulate)
        {
            if (!runEmulate)
                Click(ChangeText);
        }

        private void ChangeText(MouseButtons mouseButtons, params object[] args)
        {
            var dlg = new InputValueForm { LinesValue = Lines };
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
