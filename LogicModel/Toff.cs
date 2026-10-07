using PluginSupport;
using System.Timers;

namespace LogicModel
{
    public class Toff : Func, ILinked
    {
        private readonly System.Timers.Timer timer;

        private bool input;

        public uint MilliSeconds { private get; set; } = 1000;

        public Toff()
        {
            FuncName = "TOF";
            FuncDesc = "Задержка выключения";
            Inputs = [new FuncInput { }];
            Outputs = [new FuncOutput { }];
            CalculateHeight();
            timer = new System.Timers.Timer(MilliSeconds);
            timer.Elapsed += OnTimedEvent;
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
            OnOutputChange?.Invoke(this, new OutputChangedEventArgs(0));
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var input = Inputs[0].Value > 0;
                if (this.input == input) return;
                this.input = input;
                if (input)
                {
                    Outputs[0].Value = 1;
                    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(1));
                }
                else
                {
                    if (!timer.Enabled)
                    {
                        timer.Interval = MilliSeconds;
                        timer.Enabled = true;
                    }
                    else
                    {
                        Outputs[0].Value = 0;
                        OnOutputChange?.Invoke(this, new OutputChangedEventArgs(0));
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
    }
}
