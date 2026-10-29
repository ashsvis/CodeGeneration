using PluginSupport;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace LogicModel
{
    public class Toff : Func, IAcceptClick, IFuncPresenter, IResetable
    {
        private uint time;

        public uint MilliSeconds { private get; set; } = 1000;

        public Toff()
        {
            FuncName = "TOF";
            FuncDesc = "Задержка выключения";
            Inputs = [new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public bool IsPresent => true;
        public string[] FuncPresenter => ["t2 - время задержки заднего фронта"];

        public override Toff DeepClone()
        {
            return new Toff()
            {
                id = Id,
                Index = this.Index,
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
                MilliSeconds = MilliSeconds,
            };
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowFuncName);

        public bool CanAcceptClick => false;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var input = Inputs[0].Value > 0;
                if (!input)
                {
                    if (time > 0)
                    {
                        if (Outputs[0].Value != 1)
                        {
                            Outputs[0].Value = 1;
                            Transfer();
                        }
                        time -= 100;
                    }
                    else
                    {
                        time = 0;
                        if (Outputs[0].Value != 0)
                        {
                            Outputs[0].Value = 0;
                            Transfer();
                        }
                    }
                }
                else
                {
                    time = MilliSeconds;
                    if (Outputs[0].Value != 1)
                    {
                        Outputs[0].Value = 1;
                        Transfer();
                    }
                }
            }
        }

        public void Reset()
        {
            time = 0;
            if (!Inputs[0].IsLinked)
                Inputs[0].Value = 0;
            if (Outputs[0].Value != 0)
            {
                Outputs[0].Value = 0;
                Transfer();
            }
        }

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var paths = base.GetGraphicsPaths().ToList();
            var rect = Bounds;
            rect.Height /= 2;
            rect.Inflate(-6, -9);
            rect.Offset(0, 3);
            var path = new GraphicsPath();
            path.AddLine(new Point(rect.Left, rect.Top + rect.Height / 2), new Point(rect.Right, rect.Top + rect.Height / 2));
            path.CloseFigure();
            path.AddLine(new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom));
            path.CloseFigure();
            paths.Add(path);
            return [.. paths];
        }

        public override GraphicsPath[] GetTextPaths()
        {
            List<GraphicsPath> paths = [.. base.GetTextPaths()];
            var path = new GraphicsPath();
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Far;
            var rect = Bounds;
            rect.Height /= 2;
            rect.Inflate(-6, -9);
            rect.Offset(0, 3);
            using var tNumber = new Font("Segoe UI", 8f);
            path.AddString("t2", tNumber.FontFamily, (int)FontStyle.Regular, tNumber.Size, new Point(rect.Right, rect.Top), sf);

            // вывод времени задержки
            var fnumber = time > 0 && time < MilliSeconds ? $"{time / 1000f:0.0}" : $"{MilliSeconds / 1000f:0.#} с";
            using var fontNumber = new Font("Segoe UI", 10f);
            var sz = TextRenderer.MeasureText(fnumber, fontNumber);
            var trect = new Rectangle(Location, new Size(Width, Height));
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            path.AddString(fnumber, fontNumber.FontFamily, (int)FontStyle.Regular, fontNumber.Size, trect, sf);
            paths.Add(path);
            return [.. paths];
        }

        public override void Click(bool runEmulate, Point? point = null)
        {
            if (!runEmulate)
                Click(ChangeValue, point ?? Point.Empty);
        }

        private void ChangeValue(MouseButtons mouseButtons, params object[] args)
        {
            var dlg = new InputValueForm { Text = "Длительность задержки выключения, сек", DoubleValue = MilliSeconds / 1000.0 };
            if (args.Length > 0 && args[0] is Point location)
            {
                dlg.StartPosition = FormStartPosition.Manual;
                dlg.Location = location;
            }
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                time = 0;
                var oldMilliSeconds = MilliSeconds;
                MilliSeconds = (uint)(dlg.DoubleValue * 1000);
                PropertyChanged(nameof(MilliSeconds), oldMilliSeconds, MilliSeconds);
            }
        }

        public override XElement WriteContent()
        {
            var xfunc = base.WriteContent();
            if (MilliSeconds > 0)
            {
                var xvalue = new XElement("Seconds", (MilliSeconds / 1000.0).ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")));
                xfunc.Add(xvalue);
            }
            return xfunc;
        }

        public override void ReadContent(XElement xfunc)
        {
            base.ReadContent(xfunc);
            var xvalue = xfunc.Element("Seconds")?.Value;
            if (xvalue != null)
            {
                MilliSeconds = (uint)(ParseHelper.ParseDouble(xvalue, System.Globalization.CultureInfo.GetCultureInfo("en-US"), 0) * 1000);
            }
        }
    }
}
