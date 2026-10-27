using PluginSupport;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Xml.Linq;

namespace LogicModel
{
    public class Ton : Func, IAcceptClick, IFuncPresenter
    {
        private readonly System.Timers.Timer timer;

        private bool input;
        private uint time;

        public uint MilliSeconds { private get; set; } = 2000;

        public Ton()
        {
            FuncName = "TON";
            FuncDesc = "Задержка включения";
            Inputs = [new PinInput { }];
            Outputs = [new PinOutput { }];
            CalculateHeight();
            timer = new System.Timers.Timer(MilliSeconds);
            timer.Elapsed += OnTimedEvent;
        }

        public bool IsPresent => true;

        public override Ton DeepClone()
        {
            return new Ton()
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
                (AllowedFuncProperties.PinInverted);

        public bool CanAcceptClick => false;

        private void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            Outputs[0].Value = 1;
            time = 0;
            Transfer();
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                if (time > 0 && timer.Enabled) time -= 100;
                var input = Inputs[0].Value > 0;
                if (this.input == input) return;
                this.input = input;
                if (input && !timer.Enabled)
                {
                    timer.Interval = MilliSeconds;
                    timer.Enabled = true;
                    time = MilliSeconds;
                }
                else if (!input)
                {
                    if (timer.Enabled)
                        timer.Enabled = false;
                    Outputs[0].Value = 0;
                    time = 0;
                    Transfer();
                }
            }
        }

        public override GraphicsPath[] GetTextPaths()
        {
            List<GraphicsPath> paths = [.. base.GetTextPaths()];
            // вывод времени задержки
            var path = new GraphicsPath();
            var fnumber = time > 0 ? $"{time / 1000f:0.0}" : $"{MilliSeconds / 1000f:0.#} с";
            using var fontNumber = new Font("Segoe UI", 10f);
            var sz = TextRenderer.MeasureText(fnumber, fontNumber);
            var trect = new Rectangle(Location, new Size(Width, Height));
            using var sf = new StringFormat();
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
            var dlg = new InputValueForm { Text = "Длительность задержки включения, сек", DoubleValue = MilliSeconds / 1000.0 };
            if (args.Length > 0 && args[0] is Point location)
            {
                dlg.StartPosition = FormStartPosition.Manual;
                dlg.Location = location;
            }
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                MilliSeconds = (uint)(dlg.DoubleValue * 1000);
                Changed();
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
