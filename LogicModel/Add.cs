using PluginSupport;

namespace LogicModel
{
    public class Add : Func
    {
        public Add()
        {
            FuncName = "ADD";
            FuncDesc = "Сложение";
            Inputs = [new FuncInput { Kind = ValueKind.Analog }, new FuncInput { Kind = ValueKind.Analog }];
            Outputs = [new FuncOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }

        public override Add DeepClone()
        {
            return new Add()
            {
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
            };
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted);

        //public event OutputChangedEventHandler? OnOutputChange;

        //public virtual void UpdateLinked(double result)
        //{
        //    OnOutputChange?.Invoke(this, new OutputChangedEventArgs(result));
        //}

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value + Inputs[1].Value;
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    //UpdateLinked(result);
                }
            }
        }

        //public override void LinkInput(ILinked? link, int index)
        //{
        //    if (link == null) return;
        //    switch (index)
        //    {
        //        case 0:
        //            link.OnOutputChange += MakeChangesForFirst;
        //            break;
        //        case 1:
        //            link.OnOutputChange += MakeChangesForSecond;
        //            break;
        //    }
        //    Inputs[index].Link = link;
        //    Inputs[index].IsLinked = true;
        //}

        //public override void UnlinkInput(ILinked? link, int index)
        //{
        //    if (link == null) return;
        //    switch (index)
        //    {
        //        case 0:
        //            link.OnOutputChange -= MakeChangesForFirst;
        //            break;
        //        case 1:
        //            link.OnOutputChange -= MakeChangesForSecond;
        //            break;
        //    }
        //    Inputs[index].Link = null;
        //    Inputs[index].IsLinked = false;
        //}

        //public void MakeChangesForFirst(object? sender, OutputChangedEventArgs e)
        //{
        //    Inputs[0].Value = e.NewValue;
        //}

        //public void MakeChangesForSecond(object? sender, OutputChangedEventArgs e)
        //{
        //    Inputs[1].Value = e.NewValue;
        //}
    }
}
