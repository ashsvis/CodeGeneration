using PluginSupport;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace LogicModel
{
    public class Flag : Func, IAcceptClick
    {
        public bool Value { get; set; }

        public Flag()
        {
            FuncName = "FL";
            FuncDesc = "Флаг";
            Inputs = [new PinInput { Kind = ValueKind.Digital }];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public override Flag DeepClone()
        {
            return new Flag()
            {
                id = Id,
                Index = this.Index,
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
            };
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted);

        public bool CanAcceptClick => true;

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {

                double result;
                if (Inputs[0].IsLinked)
                {
                    result = Inputs[0].Value;
                    Value = result > 0;
                }
                else
                {
                    result = Inputs[0].Value = Value ? 1.0 : 0.0;
                }
                if (Outputs.Length > 0 && Outputs[0].Value != result)
                {
                    Outputs[0].Value = result;
                    Transfer();
                }
            }
        }

        public override void Click(bool runEmulate, Point? point = null)
        {
            if (!runEmulate)
                Click(ChangeValue);
        }

        private void ChangeValue(MouseButtons mouseButtons, params object[] args)
        {
            var dlg = new InputStateForm { Text = "Укажите состояние", BooleanValue = Value };
            if (args.Length > 0 && args[0] is Point location)
            {
                dlg.StartPosition = FormStartPosition.Manual;
                dlg.Location = location;
            }
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var oldValue = Value;
                Value = dlg.BooleanValue;
                if (oldValue != Value)
                    PropertyChanged(nameof(Value), oldValue, Value);
            }
        }

        public override GraphicsPath[] GetTextPaths()
        {
            List<GraphicsPath> paths = [.. base.GetTextPaths()];
            // вывод времени задержки
            var path = new GraphicsPath();
            var fnumber = Value ? "\"1\"" : "\"0\"";
            using var fontNumber = new Font("Consolas", 14f, FontStyle.Bold);
            var sz = TextRenderer.MeasureText(fnumber, fontNumber);
            var trect = new Rectangle(Location, new Size(Width, Height));
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            path.AddString(fnumber, fontNumber.FontFamily, (int)FontStyle.Regular, fontNumber.Size, trect, sf);
            paths.Add(path);
            return [.. paths];
        }

        public override XElement WriteContent()
        {
            var xfunc = base.WriteContent();
            if (Value)
            {
                var xvalue = new XElement("Value", Value.ToString());
                xfunc.Add(xvalue);
            }
            return xfunc;
        }

        public override void ReadContent(XElement xfunc)
        {
            base.ReadContent(xfunc);
            var xvalue = xfunc.Element("Value")?.Value;
            if (xvalue != null)
            {
                Value = ParseHelper.ParseBoolean(xvalue, false);
            }
        }
    }
}