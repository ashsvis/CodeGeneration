using PluginSupport;
using System.Drawing.Drawing2D;
using System.Timers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace LogicModel
{
    public class Pulse : Func, ILinked
    {
        private readonly System.Timers.Timer timer;

        private bool input;
        private uint time;

        public uint MilliSeconds { private get; set; } = 2000;

        public Pulse()
        {
            FuncName = "TP";
            FuncDesc = "Одновибратор";
            Inputs = [new FuncInput { }];
            Outputs = [new FuncOutput { }];
            CalculateHeight();
            timer = new System.Timers.Timer(MilliSeconds);
            timer.Elapsed += OnTimedEvent;
        }

        public override Pulse DeepClone()
        {
            return new Pulse()
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

        public event OutputChangedEventHandler? OnOutputChange;

        public virtual void UpdateLinked(double result)
        {
            OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
        }

        private void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            Outputs[0].Value = 0;
            time = 0;
            OnOutputChange?.Invoke(this, new OutputChangedEventArgs(0));
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                if (time > 0 && timer.Enabled) time -= 100;
                var input = Inputs[0].Value > 0;
                if (this.input == input) return;
                this.input = input;
                if (input)
                {
                    Outputs[0].Value = 1;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(1));
                    if (!timer.Enabled)
                    {
                        timer.Interval = MilliSeconds;
                        timer.Enabled = true;
                        time = MilliSeconds;
                    }
                }
            }
        }

        public override void LinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            link.OnOutputChange += MakeChanges;
            Inputs[index].Link = link;
            Inputs[index].IsLinked = true;
        }

        public override void UnlinkInput(ILinked? link, int index)
        {
            if (link == null) return;
            link.OnOutputChange -= MakeChanges;
            Inputs[index].Link = null;
            Inputs[index].IsLinked = false;
        }

        public void MakeChanges(object? sender, OutputChangedEventArgs e)
        {
            Inputs[0].Value = e.NewValue;
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
