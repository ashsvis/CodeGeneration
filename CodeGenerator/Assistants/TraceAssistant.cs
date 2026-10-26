using PluginSupport;

namespace CodeGenerator.Assistants
{
    public static class TraceAssistant
    {
        private const int margin = 3;
        private const int step = 12;
        private static Cell[,]? field;

        public static int CellSize => step;

        /// <summary>
        /// Построение поля для трассировки связей
        /// </summary>
        public static void InitField(IEnumerable<Shape> shapes, IEnumerable<Link> links)
        {
            if (shapes.Any())
            {
                Rectangle rect = Rectangle.Ceiling(shapes.First().Bounds);
                foreach (var shape in shapes.Skip(1))
                    rect = Rectangle.Union(rect, Rectangle.Ceiling(shape.Bounds));
                rect.Inflate(step * margin, step * margin);
                field = new Cell[rect.Width / step + 1, rect.Height / step + 1];
                // инициализация поля трассировки связей
                for (int i = 0; i < field.GetLength(0); i++)
                {
                    for (int j = 0; j < field.GetLength(1); j++)
                    {
                        field[i, j] = new Cell() { Node = new Point(i * step + rect.Left, j * step + rect.Top) };
                    }
                }
                // указание занятых ячеек поля, на которых размещены фигуры и связи
                foreach (var shape in shapes)
                {
                    if (shape is ITraceTransparent) continue;
                    var bounds = shape.Bounds;
                    var dx = (bounds.X - rect.X) / step;
                    var dy = (bounds.Y - rect.Y) / step;
                    for (int i = 0; i < bounds.Width / step + 2; i++)
                    {
                        for (var j = 0; j < bounds.Height / step + 1; j++)
                        {
                            field[i + dx, j + dy].Empty = ThroughPassage.None;
                        }
                    }
                    // занятие точек входов
                    for (var n = 0; n < shape.CountInputs(); n++)
                    {
                        var pt = shape.GetInputPinPoint(n);
                        if (pt is Point point)
                        {
                            var px = (point.X - rect.X) / step;
                            var py = (point.Y - rect.Y) / step;
                            field[px, py].Empty = ThroughPassage.Vertical;
                            if (px - 1 >= 0)
                                field[px - 1, py].Empty = ThroughPassage.Both;
                        }
                    }
                    // занятие точек выходов
                    for (var n = 0; n < shape.CountOutputs(); n++)
                    {
                        var pt = shape.GetOutputPinPoint(n);
                        if (pt is Point point)
                        {
                            var px = (point.X - rect.X) / step;
                            var py = (point.Y - rect.Y) / step;
                            field[px, py].Empty = ThroughPassage.Vertical;
                            if (px + 1 < field.GetLength(0))
                                field[px + 1, py].Empty = ThroughPassage.Both;
                        }
                    }
                }
                // занятие точек существующих связей
                foreach (var link in links)
                {
                    var points = link.GetPoints();
                    for (var i = 0; i < points.Length; i++)
                    {
                        var point = points[i];
                        var px = (point.X - rect.X) / step;
                        var py = (point.Y - rect.Y) / step;
                        if (px >= 0 && px < field.GetLength(0) &&
                            py >= 0 && py < field.GetLength(1))
                        {
                            field[px, py].Empty = ThroughPassage.None;
                            if (i > 0 && i < points.Length - 1)
                            {
                                var ptPrev = points[i - 1];
                                var ptNext = points[i + 1];
                                if (ptPrev.X == ptNext.X && point.X == ptPrev.X)
                                {
                                    field[px, py].Empty = ThroughPassage.Vertical;
                                }
                                else 
                                if (ptPrev.Y == ptNext.Y && point.Y == ptPrev.Y)
                                {
                                    field[px, py].Empty = ThroughPassage.Horizontal;
                                }
                            }
                        }
                    }
                }
            }
            else
                field = null;
        }

        public static List<Point> BuildWaveInField(Page? page, Link link)
        {
            List<Point> points = [];
            InitField(ModelAssistant.ShapesFrom(page), ModelAssistant.LinksFrom(page));
            if (field == null) return points;
            Rectangle rect = Rectangle.Ceiling(ModelAssistant.ShapesFrom(page).First().Bounds);
            foreach (var shape in ModelAssistant.ShapesFrom(page).Skip(1))
                rect = Rectangle.Union(rect, Rectangle.Ceiling(shape.Bounds));
            rect.Inflate(step * margin, step * margin);
            // установка начальной точки волны
            var n = 1;
            var start = link.StartPoint;
            var px = (start.X - rect.X) / step;
            var py = (start.Y - rect.Y) / step;
            if (px >= 0 && px < field.GetLength(0) &&
                py >= 0 && py < field.GetLength(1))
            {
                field[px, py].Wave = n;
                field[px, py].Empty = ThroughPassage.None;
            }
            // поиск других связей из этого же выхода
            foreach (var other in ModelAssistant.LinksFrom(page).Where(x => x != link && x.StartPoint == link.StartPoint))
            {
                foreach (var pt in other.GetPoints())
                {
                    // установка начальной точки волны во всех точках связи
                    var ox = (pt.X - rect.X) / step;
                    var oy = (pt.Y - rect.Y) / step;
                    if (ox >= 0 && ox < field.GetLength(0) &&
                        oy >= 0 && oy < field.GetLength(1))
                    {
                        field[ox, oy].Wave = n;
                        field[ox, oy].Empty = ThroughPassage.None;
                    }
                }
            }
            // установка конечной точки волны
            var end = link.EndPoint;
            var gx = (end.X - rect.X) / step;
            var gy = (end.Y - rect.Y) / step;
            if (gx >= 0 && gx < field.GetLength(0) &&
                gy >= 0 && gy < field.GetLength(1))
            {
                field[gx, gy].Empty = ThroughPassage.Both;
            }
            // заполнение свободных ячеек номером волны
            var found = false;
            while (!found)
            {
                var changed = false;
                for (int i = 0; i < field.GetLength(0); i++)
                {
                    for (int j = 0; j < field.GetLength(1); j++)
                    {
                        if (field[i, j].Wave == n)
                        {
                            if (i - 1 >= 0 &&
                                (field[i - 1, j].Empty == ThroughPassage.Both ||
                                 field[i - 1, j].Empty == ThroughPassage.Vertical))
                            {
                                field[i - 1, j].Wave = n + 1;
                                field[i - 1, j].Empty = ThroughPassage.None;
                                changed = true;
                                if (!found)
                                    found = gx == i - 1 && gy == j;
                            }
                            if (j - 1 >= 0 &&
                                (field[i, j - 1].Empty == ThroughPassage.Both ||
                                 field[i, j - 1].Empty == ThroughPassage.Horizontal))
                            {
                                field[i, j - 1].Wave = n + 1;
                                field[i, j - 1].Empty = ThroughPassage.None;
                                changed = true;
                                if (!found)
                                    found = gx == i && gy == j - 1;
                            }
                            if (i + 1 < field.GetLength(0) &&
                                (field[i + 1, j].Empty == ThroughPassage.Both ||
                                 field[i + 1, j].Empty == ThroughPassage.Vertical))
                            {
                                field[i + 1, j].Wave = n + 1;
                                field[i + 1, j].Empty = ThroughPassage.None;
                                changed = true;
                                if (!found)
                                    found = gx == i + 1 && gy == j;
                            }
                            if (j + 1 < field.GetLength(1) &&
                                (field[i, j + 1].Empty == ThroughPassage.Both ||
                                 field[i, j + 1].Empty == ThroughPassage.Horizontal))
                            {
                                field[i, j + 1].Wave = n + 1;
                                field[i, j + 1].Empty = ThroughPassage.None;
                                changed = true;
                                if (!found)
                                    found = gx == i && gy == j + 1;
                            }
                            if (found) break;
                        }
                        if (found) break;
                    }
                }
                if (!changed) break;
                n++;
            }
            var dir = Direction.Left;
            if (found)
            {
                var i = gx;
                var j = gy;
                var wave = field[i, j].Wave;
                points.Add(field[i, j].Node);
                while (wave > 0)
                {
                    // просмотр приоритетного направления
                    switch (dir)
                    {
                        case Direction.Left:
                            if (i - 1 >= 0 && field[i - 1, j].Wave == wave)
                            {
                                dir = Direction.Left;
                                i--;
                                points.Add(field[i, j].Node);
                                goto nextWave;
                            }
                            break;
                        case Direction.Right:
                            if (i + 1 < field.GetLength(0) && field[i + 1, j].Wave == wave)
                            {
                                dir = Direction.Right;
                                i++;
                                points.Add(field[i, j].Node);
                                goto nextWave;
                            }
                            break;
                        case Direction.Up:
                            if (j - 1 >= 0 && field[i, j - 1].Wave == wave)
                            {
                                dir = Direction.Up;
                                j--;
                                points.Add(field[i, j].Node);
                                goto nextWave;
                            }
                            break;
                        case Direction.Down:
                            if (j + 1 < field.GetLength(1) && field[i, j + 1].Wave == wave)
                            {
                                dir = Direction.Down;
                                j++;
                                points.Add(field[i, j].Node);
                                goto nextWave;
                            }
                            break;
                    }
                    // просмотр прочих направлений
                    if (i - 1 >= 0 && field[i - 1, j].Wave == wave)
                    {
                        dir = Direction.Left;
                        i--;
                        points.Add(field[i, j].Node);
                        goto nextWave;
                    }
                    if (j - 1 >= 0 && field[i, j - 1].Wave == wave)
                    {
                        dir = Direction.Up;
                        j--;
                        points.Add(field[i, j].Node);
                        goto nextWave;
                    }
                    if (j + 1 < field.GetLength(1) && field[i, j + 1].Wave == wave)
                    {
                        dir = Direction.Down;
                        j++;
                        points.Add(field[i, j].Node);
                        goto nextWave;
                    }
                    if (i + 1 < field.GetLength(0) && field[i + 1, j].Wave == wave)
                    {
                        dir = Direction.Right;
                        i++;
                        points.Add(field[i, j].Node);
                        goto nextWave;
                    }
                nextWave:
                    wave--;
                }
            }
            points.Reverse();
            return points;
        }

        public static void DrawField(Graphics? graphics)
        {
            if (field != null)
            {
                // рисуем поле трассировки связей
                for (int i = 0; i < field.GetLength(0); i++)
                {
                    for (int j = 0; j < field.GetLength(1); j++)
                    {
                        field[i, j].Draw(graphics);
                    }
                }
            }
        }
    }
}
