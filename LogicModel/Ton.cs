using PluginSupport;
using System.Drawing.Drawing2D;
using System.Timers;

namespace LogicModel
{
    public class Ton : Func
    {
        private readonly System.Timers.Timer timer;

        private bool input;
        private uint time;

        public uint MilliSeconds { private get; set; } = 2000;

        public Ton()
        {
            FuncName = "TON";
            FuncDesc = "Задержка включения";
            Inputs = [new FuncInput { }];
            Outputs = [new FuncOutput { }];
            CalculateHeight();
            timer = new System.Timers.Timer(MilliSeconds);
            timer.Elapsed += OnTimedEvent;
        }

        public override Ton DeepClone()
        {
            return new Ton()
            {
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

        private void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            Outputs[0].Value = 1;
            time = 0;
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
    }
}
