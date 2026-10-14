using PluginSupport;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace LogicModel
{
    public class AnalogInputPageLink : Func, IPageTargetLink
    {
        public AnalogInputPageLink()
        {
            FuncName = "-->A";
            FuncDesc = "Входящая аналоговая ссылка на страницу";
            Inputs = [];
            Outputs = [new PinOutput { Kind = ValueKind.Analog }];
            CalculateHeight();
        }

        public override AnalogInputPageLink DeepClone()
        {
            return new AnalogInputPageLink()
            {
                id = Id,
                Index = this.Index,
                Location = Location,
                FuncName = FuncName,
                FuncDesc = FuncDesc,
                Inputs = [],
                Outputs = [.. Outputs.Select(x => x.DeepClone())],
            };
        }

        public override AllowedFuncProperties AllowedFuncProperties => AllowedFuncProperties.All ^
                (AllowedFuncProperties.PinInverted | AllowedFuncProperties.ShowBorder | AllowedFuncProperties.ShowPins |
                AllowedFuncProperties.ShowFuncName | AllowedFuncProperties.ShowLabelNumber);

        public ValueKind ValueKind => ValueKind.Analog;

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var paths = base.GetGraphicsPaths().ToList();
            var rect = Bounds;
            var path = new GraphicsPath();
            var pinPoint = GetOutputPinPoint(0) ?? Point.Empty;
            path.AddLine(new Point(pinPoint.X - rect.Width / 4, pinPoint.Y), new Point(pinPoint.X, pinPoint.Y - rect.Height / 8));
            path.CloseFigure();
            path.AddLine(new Point(pinPoint.X - rect.Width / 4, pinPoint.Y), new Point(pinPoint.X, pinPoint.Y + rect.Height / 8));
            path.CloseFigure();
            path.AddLine(new Point(pinPoint.X - rect.Width / 4, pinPoint.Y), new Point(pinPoint.X, pinPoint.Y));
            path.CloseFigure();
            paths.Add(path);
            return [.. paths];
        }

        [ReadOnly(true)]
        public AnalogOutputPageLink? SourcePageLink;

        public override GraphicsPath[] GetTextPaths()
        {
            var paths = base.GetTextPaths().ToList();
            using var fontFunc = new Font("Segoe UI", 14f);
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Center;
            var path = new GraphicsPath();
            var fname = SourcePageLink != null ? $"Ссылка {SourcePageLink.Number} с листа {SourcePageLink.Page}" : "Ссылка с листа (ниоткуда)";
            var pinPoint = GetOutputPinPoint(0) ?? Point.Empty;
            path.AddString(fname, fontFunc.FontFamily, (int)FontStyle.Regular,
                fontFunc.Size, Point.Add(pinPoint, new Size(-24, 0)), sf);
            paths.Add(path);
            return [.. paths];
        }

        public override bool ContainsPoint(Point point, float width)
        {
            using var pen = new Pen(Foreground, width);
            foreach (var p in GetTextPaths())
            {
                using var path = p;
                if (path.IsOutlineVisible(point, pen) || path.IsVisible(point))
                    return true;
            }
            return false;
        }

        public override void SetInputValue(int index, double value)
        {
            Outputs[0].Value = value;
            Transfer();
        }

        /// <summary>
        /// Добавление пунктов в контекстное меню
        /// </summary>
        /// <param name="point">Точка нажатия на элементе</param>
        /// <param name="several">Признак выбора нескольких элементов</param>
        /// <returns></returns>
        public override ToolStripItem[] GetContextMenuItems(Point point, bool several)
        {
            List<ToolStripItem> items = [];
            var item = new ToolStripMenuItem() { Text = "Свойства..." };
            item.Click += (s, e) => ShowProperties();
            items.Add(item);
            var results = Query(typeof(AnalogOutputPageLink));
            if (results.Length > 0)
            {
                items.Add(new ToolStripSeparator());
                item = new ToolStripMenuItem("Установить связь");
                foreach (var r in results)
                {
                    if (r is AnalogOutputPageLink analog)
                    {
                        var dropItem = new ToolStripMenuItem() { Text = $"Ссылка {analog.Number} с листа {analog.Page}" };
                        dropItem.Click += (s, e) =>
                        {
                            SourcePageLink?.Targets.Remove(this);
                            SourcePageLink = analog;
                            if (!SourcePageLink.Targets.Contains(this))
                                SourcePageLink.Targets.Add(this);
                        };
                        item.DropDownItems.Add(dropItem);
                    }
                }
                items.Add(item);
            }
            items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem() { Text = "Удалить" };
            item.Click += (s, e) => Delete();
            items.Add(item);
            return [.. items];
        }

        public override void Delete()
        {
            SourcePageLink?.Targets.Remove(this);
            base.Delete();
        }

        public void RemoveTargetFromOutput(Shape? target, int targetPinIndex)
        {
            foreach (var output in Outputs)
            {
                foreach(var tar in output.Targets)
                {
                    if (tar.Owner == target && tar.PinIndex == targetPinIndex)
                    {
                        tar.Owner = null;
                        tar.PinIndex = 0;
                    }
                }
            }
        }
    }
}
