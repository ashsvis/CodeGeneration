using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace PluginSupport
{
    public abstract class Shape : IDeepCloneable<Shape>, IPersistent<Shape>
    {
        protected Guid id = Guid.Empty;
        public Guid Id
        {
            get 
            { 
                if (id == Guid.Empty) id = Guid.NewGuid();
                return id;
            }
        }

        public void NewGuid()
        {
            id = Guid.NewGuid();
        }

        public void NewGuid(Guid guid)
        {
            id = guid;
        }

        [Category("Common"), DisplayName("Page"), ReadOnly(true)]
        public int Page { get; set; }

        [Browsable(false)]
        public Point Location { get; set; }

        [Browsable(false)]
        public abstract Rectangle Bounds { get; }

        [Browsable(false)]
        public Color Foreground { get; set; } = Color.FromArgb(200, 200, 200);

        [Browsable(false)]
        public Color Background { get; set; } = Color.FromArgb(50, 50, 50);

        [Browsable(false)]
        public bool Selected { get; set; }

        [Browsable(false)]
        public bool Hover { get; set; }

        [Browsable(false)]
        public bool CanInputLink { get; set; }

        [Browsable(false)]
        public bool CanOutputLink { get; set; }

        [Browsable(false)]
        public int Index { get; set; }

        public event DeleteLinkFromTargetEventHandler? OnDeleteLink;
        public event EventHandler? OnShowProperties;
        public event EventHandler? OnDelete;
        public event EventHandler? OnMakeCopy;
        public event EventHandler? OnChange;
        public event ClickEventHandler? OnClick;

        public abstract GraphicsPath[] GetGraphicsPaths();

        public virtual GraphicsPath[] GetTextPaths() { return []; }

        public virtual GraphicsPath[] GetValuesPaths() { return []; }

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

        public virtual void DrawWhenDebug(Graphics? g, Brush brush)
        {
            foreach (var p in GetValuesPaths())
            {
                using var path = p;
                g?.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Указанная точка попадает в фигуру
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual bool ContainsPoint(Point point, float width)
        {
            return Bounds.Contains(point);
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
        public abstract double GetInputValue(int index);

        public abstract void SetInputSource(int index, Shape source, int pinIndex, Guid linkId);
        public abstract void RemoveInputSource(int index);
        public abstract void SetOutputTarget(int index, Shape target, int pinIndex, Guid linkId);
        public abstract void RemoveOutputTarget(int index, Shape? target, int pinIndex);

        public abstract Point? GetInputPinPoint(int index);
        public abstract Guid GetInputPinLinkId(int index);
        public abstract void SetInputPinLinkId(int index, Guid guid);
        public abstract Point? GetOutputPinPoint(int index = 0);
        public abstract Guid GetOutputPinLinkId(int index = 0);
        public abstract void SetOutputPinLinkId(int index = 0, Guid guid = default);


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

        public bool IsOutputTargetsPoint(Point point, out int index)
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
            item = new ToolStripMenuItem() { Text = "Дублировать" };
            item.Click += (s, e) => MakeCopy();
            items.Add(item);
            items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem() { Text = "Удалить" };
            item.Click += (s, e) => Delete();  
            items.Add(item);
            return [.. items];
        }

        public void ShowProperties()
        {
            OnShowProperties?.Invoke(this, EventArgs.Empty);
        }

        public void MakeCopy()
        {
            OnMakeCopy?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Delete()
        {
            OnDelete?.Invoke(this, EventArgs.Empty);
        }

        public void Changed()
        {
            OnChange?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Click(bool runEmulate, Point? point = null) { }

        protected void Click(Action<MouseButtons, object[]> action,  params object[] args)
        {
            OnClick?.Invoke(this, new ClickEventArgs(action, args));
        }

        public abstract void Calculate();
        protected abstract void CalculateHeight();
        public abstract int CountInputs();
        public abstract int CountOutputs();
        public abstract bool IsLinked(int index);

        public abstract Type GetLinkTypeToCreate();

        public void DeleteLinkFromTarget(Shape? source, Shape target, int pinIndex)
        {
            OnDeleteLink?.Invoke(this, new DeleteLinkFromTargetEventArgs(source, target, pinIndex));
        }

        public abstract Shape DeepClone();

        public abstract XElement WriteContent();
        public abstract void ReadContent(XElement element);
        public abstract bool NoDataToWrite();
        public abstract ValueKind GetInputValueKind(int index);
        public virtual void SetInvertStatePinByIndex(bool isOutput, int pinIndex, bool state) { }
    }
}
