using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public abstract class Shape : ILocation
    {
        private PointF location;

        public PointF Location 
        { 
            get => location; 
            set 
            {
                if (location == value) return;
                location = value; 
                OnLocationChange?.Invoke(this, new LocationChangedEventArgs(location));
            } 
        }

        public abstract RectangleF Bounds { get; }
        public Color Foreground { get; set; } = Color.FromArgb(200, 200, 200);
        public Color Background { get; set; } = Color.FromArgb(50, 50, 50);

        public bool Selected { get; set; }
        public bool Hover { get; set; }
        public bool CanInputLink { get; set; }
        public bool CanOutputLink { get; set; }
        public int Index { get; set; }

        public event LocationChangedEventHandler? OnLocationChange;
        public event DeleteLinkFromTargetEventHandler? OnDeleteLink;

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
        public bool ContainsPoint(PointF point, float width)
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
        public bool IsPointInTargets(PointF point)
        {
            foreach (var t in GetTargets())
            {
                if (t.Target.Contains(point))
                    return true;
            }
            return false;
        }

        public bool IsOutputTargetsPoint(PointF point)
        {
            foreach (var t in GetTargets().Where(x => x.IsOutput))
            {
                if (t.Target.Contains(point))
                    return true;
            }
            return false;
        }

        public abstract object? GetOutputValue(int index = 0);
        public abstract void SetInputValue(int index, object? value);

        public abstract PointF? GetInputPinPoint(int index);
        public abstract PointF? GetOutputPinPoint(int index = 0);


        public bool IsInputTargetsPoint(PointF point, out int index)
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

        public bool IsOuputTargetsPoint(PointF point, out int index)
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
        public virtual ToolStripItem[] GetContextMenuItems(PointF point, bool several)
        {
            List<ToolStripItem> items = [];
            //ToolStripMenuItem item;
            //item = new ToolStripMenuItem() { Text = "Поднять наверх" };
            //items.Add(item);
            //item = new ToolStripMenuItem() { Text = "Поднять выше" };
            //items.Add(item);
            //item = new ToolStripMenuItem() { Text = "Опустить ниже" };
            //items.Add(item);
            //item = new ToolStripMenuItem() { Text = "Опустить вниз" };
            //items.Add(item);
            //items.Add((ToolStripItem)new ToolStripSeparator());
            //item = new ToolStripMenuItem() { Text = "Удалить" };
            //items.Add(item);
            return [.. items];
        }

        public abstract void Click(PointF point, Action<TargetInfo>? action = null);
        public abstract void Calculate();
        protected abstract void CalculateHeight();
        public abstract int CountInputs();
        public abstract int CountOutputs();
        public abstract void LinkInput(ILink? link, int index);
        public abstract void UnlinkInput(ILink? link, int index);
        public abstract bool IsLinked(int index);
        public abstract void UnlinkAllInputs();
        public abstract void UnlinkOutputFor(ILink? link);

        public abstract Type GetLinkTypeToCreate();

        public void DeleteLinkFromTarget(ILink? source, ILink target, int pinIndex)
        {
            OnDeleteLink?.Invoke(this, new DeleteLinkFromTargetEventArgs(source, target, pinIndex));
        }
    }

    public class DeleteLinkFromTargetEventArgs(ILink? source, ILink? target, int targetPinIndex) : EventArgs
    {
        public ILink? Source { get; set; } = source;
        public ILink? Target { get; set; } = target;
        public int TargetPinIndex { get; set; } = targetPinIndex;
    }

    public delegate void DeleteLinkFromTargetEventHandler(object sender, DeleteLinkFromTargetEventArgs e);
}
