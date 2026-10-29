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
        public virtual int Width { get; set; }

        [Browsable(false)]
        public virtual int Height { get; set; }

        [Browsable(false)]
        public virtual Rectangle Bounds => new(Location.X, Location.Y, Width, Height);

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
        public event EventHandler? OnChange;
        public event PropertyChangedEventHandler? OnPropertyChange;

        public event ClickEventHandler? OnClick;

        public abstract GraphicsPath[] GetGraphicsPaths();

        public virtual GraphicsPath[] GetTextPaths() { return []; }

        public virtual GraphicsPath[] GetValuesPaths() { return []; }

        public virtual TargetInfo[] GetTargets() { return []; }
        public virtual PinInfo[] GetPinPoints() { return []; }


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

        public virtual double GetOutputValue(int index = 0) { return double.NaN; }
        public virtual ValueKind GetOutputValueKind(int index = 0) { return ValueKind.None; }
        public virtual void SetInputValue(int index, double value) { }
        public virtual double GetInputValue(int index) { return double.NaN; }

        public virtual void SetInputSource(int index, Shape source, int pinIndex, Guid linkId) { }
        public virtual void RemoveInputSource(int index) { }
        public virtual void SetOutputTarget(int index, Shape target, int pinIndex, Guid linkId) { }
        public virtual void RemoveOutputTarget(int index, Shape? target, int pinIndex) { }

        public virtual Point? GetInputPinPoint(int index) { return null; }
        public virtual Guid GetInputPinLinkId(int index) { return Guid.Empty; }
        public virtual void SetInputPinLinkId(int index, Guid guid) { }
        public virtual Point? GetOutputPinPoint(int index = 0) { return null; }
        public virtual Guid GetOutputPinLinkId(int index = 0) { return Guid.Empty; }
        public virtual void SetOutputPinLinkId(int index = 0, Guid guid = default) { }


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
            return [.. items];
        }

        public void Changed()
        {
            OnChange?.Invoke(this, EventArgs.Empty);
        }

        public void PropertyChanged(string? propName, object? oldValue, object? newValue)
        {
            OnPropertyChange?.Invoke(this, new PropertyChangedEventArgs(propName, oldValue, newValue));
        }

        public virtual void Click(bool runEmulate, Point? point = null) { }

        protected void Click(Action<MouseButtons, object[]> action,  params object[] args)
        {
            OnClick?.Invoke(this, new ClickEventArgs(action, args));
        }

        public virtual void Calculate() { }
        protected virtual void CalculateHeight() { }
        public virtual int CountInputs() { return 0; }
        public virtual int CountOutputs() { return 0; }
        public virtual bool IsLinked(int index) { return false; }

        public virtual Type GetLinkTypeToCreate() { return typeof(object);  }

        public void DeleteLinkFromTarget(Shape? source, Shape target, int pinIndex)
        {
            OnDeleteLink?.Invoke(this, new DeleteLinkFromTargetEventArgs(source, target, pinIndex));
        }

        public abstract Shape DeepClone();

        public abstract XElement WriteContent();
        public abstract void ReadContent(XElement element);
        public virtual bool NoDataToWrite() { return false; }
        public virtual ValueKind GetInputValueKind(int index) { return ValueKind.None; }
        public virtual void SetInvertStatePinByIndex(bool isOutput, int pinIndex, bool state) { }
    }
}
