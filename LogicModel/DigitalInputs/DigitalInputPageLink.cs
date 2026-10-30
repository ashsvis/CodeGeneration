using PluginSupport;
using PluginSupport.Assistants;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace LogicModel
{
    public class DigitalInputPageLink : Func, IPageTargetLink, IBeforeRemove
    {
        public DigitalInputPageLink()
        {
            FuncName = "-->D";
            FuncDesc = "Входящая дискретная ссылка на страницу";
            Width = BaseWidth * 3;
            Height = BaseHeight / 2;
            Inputs = [];
            Outputs = [new PinOutput { Kind = ValueKind.Digital }];
            CalculateHeight();
        }

        public override DigitalInputPageLink DeepClone()
        {
            return new DigitalInputPageLink()
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

        public ValueKind ValueKind => ValueKind.Digital;

        public override GraphicsPath[] GetGraphicsPaths()
        {
            var paths = base.GetGraphicsPaths().ToList();
            var rect = Bounds;
            rect.Width = BaseWidth;
            rect.Height = BaseHeight;
            var path = new GraphicsPath();
            var pinPoint = GetOutputPinPoint(0) ?? Point.Empty;
            path.AddLine(new Point(pinPoint.X - rect.Width / 4, pinPoint.Y), new Point(pinPoint.X, pinPoint.Y - rect.Height / 8));
            path.CloseFigure();
            path.AddLine(new Point(pinPoint.X - rect.Width / 4, pinPoint.Y), new Point(pinPoint.X, pinPoint.Y + rect.Height / 8));
            path.CloseFigure();
            path.AddLine(new Point(pinPoint.X - rect.Width /4, pinPoint.Y), new Point(pinPoint.X, pinPoint.Y));
            path.CloseFigure();
            paths.Add(path);
            return [.. paths];
        }

        [ReadOnly(true)]
        public DigitalOutputPageLink? SourcePageLink;

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
            ToolStripMenuItem item;

            List<object> results = [];
            var contexts = ContextAssistant.ShapesFromAllContexts.Values;
            foreach (var shapes in contexts)
                results.AddRange(shapes.Where(x => x.GetType() == typeof(DigitalOutputPageLink)));

            if (results.Count > 0)
            {
                items.Add(new ToolStripSeparator());
                item = new ToolStripMenuItem("Установить связь");
                foreach (var r in results)
                {
                    if (r is DigitalOutputPageLink digital)
                    {
                        var dropItem = new ToolStripMenuItem() { Text = $"Ссылка {digital.Number} с листа {digital.Page}" };
                        dropItem.Click += (s, e) =>
                        {
                            SourcePageLink?.Targets.Remove(this);
                            SourcePageLink = digital;
                            if (!SourcePageLink.Targets.Contains(this))
                                SourcePageLink.Targets.Add(this);
                            Changed();
                        };
                        item.DropDownItems.Add(dropItem);
                    }
                }
                items.Add(item);
            }
            return [.. items];
        }

        public void Delete()
        {
            SourcePageLink?.Targets.Remove(this);
        }

        public void RemoveTargetFromOutput(Shape? target, int targetPinIndex)
        {
            foreach (var output in Outputs)
            {
                foreach (var tar in output.Targets)
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
