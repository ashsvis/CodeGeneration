using PluginSupport;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace LogicModel
{
    public class AnalogOutputPageLink : Func, IPageSourceLink, IBeforeRemove
    {
        public AnalogOutputPageLink()
        {
            FuncName = "A-->";
            FuncDesc = "»сход€ща€ аналогова€ ссылка со страницы";
            Width = BaseWidth * 3;
            Height = BaseHeight / 2;
            Inputs = [new PinInput { Kind = ValueKind.Analog }];
            Outputs = [];
            CalculateHeight();
        }

        [ReadOnly(true)]
        public int Number { get; set; }

        [Browsable(false)]
        public List<AnalogInputPageLink> Targets = [];

        public override AnalogOutputPageLink DeepClone()
        {
            return new AnalogOutputPageLink()
            {
                id = Id,
                Index = this.Index,
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [.. Inputs.Select(x => x.DeepClone())],
                Outputs = [],
            };
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowBorder | AllowedFuncProperties.ShowPins|
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        public ValueKind ValueKind => ValueKind.Analog;

        public override XElement WriteContent()
        {
            var xfunc = base.WriteContent();
            if (Targets.Count > 0)
            {
                var xtargets = new XElement("Targets");
                xfunc.Add(xtargets);
                foreach (var target in Targets)
                {
                    var xtarget = new XElement("Id", target.Id);
                    xtargets.Add(xtarget);
                }
            }
            return xfunc;
        }

        private readonly List<Guid> targets = [];

        public override void ReadContent(XElement xfunc)
        {
            base.ReadContent(xfunc);
            targets.Clear();
            var xTargets = xfunc.Element("Targets");
            if (xTargets != null)
            {
                foreach (var xid in xTargets.Elements("Id"))
                {
                    var id = ParseHelper.ParseGuid(xid.Value, Guid.Empty);
                    if (id == Guid.Empty) continue;
                    targets.Add(id);
                }
            }
        }

        public Guid[] GetIdsForConnect()
        {
            return [.. targets];
        }

        public void AddTarget(Shape shape)
        {
            if (shape is AnalogInputPageLink plink && !Targets.Contains(plink))
            {
                plink.SourcePageLink = this;
                Targets.Add(plink);
            }
        }

        public void ReplaceTargetId(Guid oldId, Guid newId)
        {
            for (var i = 0; i < targets.Count; i++)
            {
                if (targets[i] == oldId) targets[i] = newId;
            }
        }

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var paths = base.GetGraphicsPaths().ToList();
            var rect = Bounds;
            rect.Width = BaseWidth;
            rect.Height = BaseHeight;
            var path = new GraphicsPath();
            var pinPoint = GetInputPinPoint(0) ?? Point.Empty;
            path.AddLine(new Point(pinPoint.X, pinPoint.Y - rect.Height / 8), new Point(pinPoint.X + rect.Width / 4, pinPoint.Y));
            path.CloseFigure();
            path.AddLine(new Point(pinPoint.X, pinPoint.Y + rect.Height / 8), new Point(pinPoint.X + rect.Width / 4, pinPoint.Y));
            path.CloseFigure();
            path.AddLine(new Point(pinPoint.X, pinPoint.Y), new Point(pinPoint.X + rect.Width / 4, pinPoint.Y));
            path.CloseFigure();
            paths.Add(path);
            return [.. paths];
        }

        public override GraphicsPath[] GetTextPaths()
        {
            var paths = base.GetTextPaths().ToList();
            var rect = Bounds;
            using var fontFunc = new Font("Segoe UI", 14f);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            var path = new GraphicsPath();
            var fname = $"—сылка {Number} с листа {Page} (никуда)";
            if (Targets.Count > 0)
            {
                var items = Targets.OrderBy(x => x.Page).Select(x => x.Page).Distinct().ToArray();
                if (items.Length == 1)
                    fname = $"—сылка {Number} на лист {items[0]}";
                else
                    fname = $"—сылка {Number} на листы {string.Join(", ", items)}";
            }
            var sz = TextRenderer.MeasureText(fname, fontFunc);
            rect.Width = sz.Width;
            path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Regular,
                fontFunc.Size, new Point(rect.Left + 12, rect.Top + rect.Height / 2), sf);
            paths.Add(path);
            return [.. paths];
        }

        public void Delete()
        {
            foreach (var item in Targets)
                item.SourcePageLink = null;
        }

        public override void Calculate()
        {
            if (Inputs.Length > 0)
            {
                var result = Inputs[0].Value;
                foreach (var target in Targets)
                {
                    target.SetInputValue(0, result);
                }
                Transfer();
            }
        }

        public void SetLinkNumber(int number)
        {
            Number = number;
        }

        public int GetLinkNumber()
        {
            return Number;
        }
    }
}
