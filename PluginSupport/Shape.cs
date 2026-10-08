using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace PluginSupport
{
    public abstract class Shape : /*ILocation, */IDeepCloneable<Shape>, IPersistent<Shape>
    {
        private Guid id = Guid.Empty;
        //private Point location;
        public Guid Id
        {
            get 
            { 
                if (id == Guid.Empty) id = Guid.NewGuid();
                return id;
            }
        }

        public Point Location { get; set; }
        //public Point Location 
        //{ 
        //    get => location; 
        //    set 
        //    {
        //        if (location == value) return;
        //        location = value; 
        //        OnLocationChange?.Invoke(this, new LocationChangedEventArgs(location));
        //    } 
        //}

        public abstract Rectangle Bounds { get; }
        public Color Foreground { get; set; } = Color.FromArgb(200, 200, 200);
        public Color Background { get; set; } = Color.FromArgb(50, 50, 50);

        public bool Selected { get; set; }
        public bool Hover { get; set; }
        public bool CanInputLink { get; set; }
        public bool CanOutputLink { get; set; }
        public int Index { get; set; }

        //public event LocationChangedEventHandler? OnLocationChange;
        public event DeleteLinkFromTargetEventHandler? OnDeleteLink;
        public event EventHandler? OnShowProperties;
        public event EventHandler? OnDelete;
        public event EventHandler? OnMakeCopy;

        public abstract GraphicsPath[] GetGraphicsPaths();

        public virtual GraphicsPath[] GetTextPaths()
        {
            return [];
        }

        public abstract TargetInfo[] GetTargets();
        public abstract PinInfo[] GetPinPoints();

        public virtual void Draw(Graphics? g, Pen pen, Brush brush)
        {
            foreach (var p in GetGraphicsPaths())
            {
                using var path = p;
                g?.FillPath(brush, path);
                g?.DrawPath(pen, path);
            }
            using var text = new SolidBrush(pen.Color);
            foreach (var p in GetTextPaths())
            {
                using var path = p;
                g?.FillPath(text, path);
            }
        }

        /// <summary>
        /// Указанная точка попадает в фигуру
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool ContainsPoint(Point point, float width)
        {
            using var pen = new Pen(Foreground, width);
            foreach (var p in GetGraphicsPaths())
            {
                using var path = p;
                if (path.IsOutlineVisible(point, pen) || path.IsVisible(point))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Указанная точка попадает в таргет
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointInTargets(Point point)
        {
            foreach (var t in GetTargets())
            {
                if (t.Target.Contains(point))
                    return true;
            }
            return false;
        }

        public bool IsOutputTargetsPoint(Point point)
        {
            foreach (var t in GetTargets().Where(x => x.IsOutput))
            {
                if (t.Target.Contains(point))
                    return true;
            }
            return false;
        }

        public abstract double GetOutputValue(int index = 0);
        public abstract ValueKind GetOutputValueKind(int index = 0);
        public abstract void SetInputValue(int index, double value);

        public abstract Point? GetInputPinPoint(int index);
        public abstract Point? GetOutputPinPoint(int index = 0);


        public bool IsInputTargetsPoint(Point point, out int index)
        {
            index = -1;
            foreach (var t in GetTargets().Where(x => !x.IsOutput))
            {
                if (t.Target.Contains(point))
                {
                    index = t.PinIndex;
                    return true;
                }
            }
            return false;
        }

        public bool IsOuputTargetsPoint(Point point, out int index)
        {
            index = -1;
            foreach (var t in GetTargets().Where(x => x.IsOutput))
            {
                if (t.Target.Contains(point))
                {
                    index = t.PinIndex;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Добавление пунктов в контекстное меню
        /// </summary>
        /// <param name="point">Точка нажатия на элементе</param>
        /// <param name="several">Признак выбора нескольких элементов</param>
        /// <returns></returns>
        public virtual ToolStripItem[] GetContextMenuItems(Point point, bool several)
        {
            List<ToolStripItem> items = [];
            ToolStripMenuItem item;
            item = new ToolStripMenuItem() { Text = "Свойства..." };
            item.Click += (s, e) => OnShowProperties?.Invoke(this, EventArgs.Empty);
            items.Add(item);
            items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem() { Text = "Дублировать" };
            item.Click += (s, e) => OnMakeCopy?.Invoke(this, EventArgs.Empty);
            items.Add(item);
            items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem() { Text = "Удалить" };
            item.Click += (s, e) => OnDelete?.Invoke(this, EventArgs.Empty);  
            items.Add(item);
            return [.. items];
        }

        public abstract void Click(Point point, Action<TargetInfo>? action = null);
        public virtual void Calculate() { }
        protected abstract void CalculateHeight();
        public abstract int CountInputs();
        public abstract int CountOutputs();
        //public abstract void LinkInput(Shape? link, int index);
        //public abstract void UnlinkInput(Shape? link, int index);
        public abstract bool IsLinked(int index);
        //public abstract void UnlinkAllInputs();
        //public abstract void UnlinkOutputFor(Shape? link);

        public abstract Type GetLinkTypeToCreate();

        public void DeleteLinkFromTarget(Shape? source, Shape target, int pinIndex)
        {
            OnDeleteLink?.Invoke(this, new DeleteLinkFromTargetEventArgs(source, target, pinIndex));
        }

        public abstract Shape DeepClone();

        public abstract XElement WriteContent();
        public abstract void ReadContent(XElement element);
        public abstract bool NoDataToWrite();
        public abstract ValueKind GetInputValueKnd(int index);
    }

    public class DeleteLinkFromTargetEventArgs(Shape? source, Shape? target, int targetPinIndex) : EventArgs
    {
        public Shape? Source { get; set; } = source;
        public Shape? Target { get; set; } = target;
        public int TargetPinIndex { get; set; } = targetPinIndex;
    }

    public delegate void DeleteLinkFromTargetEventHandler(object sender, DeleteLinkFromTargetEventArgs e);
}
