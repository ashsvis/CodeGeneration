using System.Drawing.Drawing2D;

namespace PluginSupport
{
    public abstract class Shape : ILocation
    {
        public PointF Location { get; set; }
        public Color Foreground { get; set; } = Color.FromArgb(200, 200, 200);
        public Color Background { get; set; } = Color.FromArgb(50, 50, 50);

        public bool Selected { get; set; }
        public bool Hover { get; set; }

        public abstract GraphicsPath[] GetGraphicsPaths();
        
        public virtual GraphicsPath[] GetTextPaths()
        {
            return [];
        }

        public virtual Tuple<bool, int, RectangleF>[] GetTargets()
        {
            return [];
        }

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
        public bool ContainsPoint(PointF point)
        {
            using var pen = new Pen(Foreground, 1);
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
        public bool TargetsPoint(PointF point)
        {
            foreach (var r in GetTargets())
            {
                if (r.Item3.Contains(point))
                    return true;
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

        public abstract void Click(PointF point);
    }
}
