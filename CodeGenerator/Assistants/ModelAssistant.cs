using PluginSupport;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

#pragma warning disable IDE0130 // Пространство имен (namespace) не соответствует структуре папок.
namespace CodeGenerator
#pragma warning restore IDE0130 // Пространство имен (namespace) не соответствует структуре папок.
{
    public static class ModelAssistant
    {

        private static readonly Dictionary<Page, List<Shape>> cacheShapes = [];
        private static readonly List<Shape> cacheNoPageShapes = [];
        private static readonly Dictionary<string, Type> types = [];

        public static void ClearCache()
        {
            cacheShapes.Clear();
            cacheNoPageShapes.Clear();
        }

        public static void AddType(string key, Type type)
        {
            types.TryAdd(key, type);
        }

        public static List<Shape> ShapesFrom(Page? page)
        {
            if (page == null)
            {
                if (cacheNoPageShapes.Count > 0)
                    return cacheNoPageShapes;
            }
            else
            {
                if (cacheShapes.TryGetValue(page, out List<Shape>? value) && value.Count > 0)
                    return value;
            }
            var result = ContextAssistant.ContextShapes.Where(x => page == null && x.Page == 0 || page != null && x.Page == page.Number).ToList();
            result.Reverse();
            if (page == null)
                cacheNoPageShapes.AddRange(result);
            else
            {
                if (!cacheShapes.ContainsKey(page))
                    cacheShapes.Add(page, []);
                cacheShapes[page].AddRange(result);
            }
            return result;
        }

        public static List<Link> LinksFrom(Page? page)
        {
            return [.. ContextAssistant.ContextLinks.Where(x => page == null && x.Page == 0 || page != null && x.Page == page.Number)];
        }

        public static void SelectAllOnPage(Page? page)
        {
            if (ContextAssistant.ContextShapes != null)
                ShapesFrom(page).ForEach(x => x.Selected = true);
            if (ContextAssistant.ContextLinks != null)
                LinksFrom(page).ForEach(x => x.Selected = true);
        }

        public static void UnselectAll()
        {
            ContextAssistant.ContextShapes?.ForEach(shape => { shape.Selected = false; shape.Hover = false; });
            ContextAssistant.ContextLinks?.ForEach(link => { link.Selected = false; link.Hover = false; });
        }

        public static void UnselectAllOnPage(Page? page)
        {
            if (ContextAssistant.ContextShapes != null)
                ShapesFrom(page).ForEach(shape => { shape.Selected = false; shape.Hover = false; });
            if (ContextAssistant.ContextLinks != null)
                LinksFrom(page).ForEach(link => { link.Selected = false; link.Hover = false; });
        }

        private static readonly int Step = TraceAssistant.CellSize; // Размер клетки

        public static Point MovePointToGrid(Point point)
        {
            return new Point(point.X / Step * Step, point.Y / Step * Step);
        }

        public static XElement CopyPageToXml(Page? page)
        {
            var xmodel = new XElement("Copy");
            foreach (var shape in ShapesFrom(page))
            {
                var xshape = shape.WriteContent();
                xmodel.Add(xshape);
            }
            foreach (var link in LinksFrom(page))
            {
                var xlink = link.WriteContentExtended();
                xmodel.Add(xlink);
            }
            return xmodel;
        }

        public static XElement CopySelectedToXml()
        {
            var xmodel = new XElement("Copy");
            foreach (var shape in ContextAssistant.ContextShapes.Where(x => x.Selected))
            {
                var xshape = shape.WriteContent();
                xmodel.Add(xshape);
            }
            foreach (var link in ContextAssistant.ContextLinks.Where(x => x.Selected))
            {
                var xlink = link.WriteContentExtended();
                xmodel.Add(xlink);
            }
            return xmodel;
        }

        public static (GraphicsPath[], Rectangle, Point) PasteFromXml(XElement xmodel, Point? point = null)
        {
            if (xmodel == null) return ([], Rectangle.Empty, Point.Empty);
            List<Shape> pastedShapes = [];
            List<Link> pastedLinks = [];
            var rect = Rectangle.Empty;
            int n = 0;
            foreach (var xelement in xmodel.Descendants())
            {
                if (types.ContainsKey($"{xelement.Name}"))
                {
                    var type = types[$"{xelement.Name}"];
                    var obj = Activator.CreateInstance(type);
                    if (obj is Shape shape)
                    {
                        shape.Selected = true;
                        shape.Index = n++;
                        shape.ReadContent(xelement);
                        pastedShapes.Add(shape);
                        rect = rect.IsEmpty ? shape.Bounds : Rectangle.Union(rect, shape.Bounds);
                    }
                    else if (obj is Link link)
                    {
                        link.ReadContent(xelement);
                        var source = pastedShapes.FirstOrDefault(x => x.Id == link.SourceId);
                        var target = pastedShapes.FirstOrDefault(x => x.Id == link.TargetId);
                        if (source != null && target != null)
                        {
                            link.LinkToLocation(source, link.SourcePinIndex, link.StartPoint, 
                                target, link.TargetPinIndex, link.EndPoint, [.. link.GetPoints()]);
                            pastedLinks.Add(link);
                        }
                    }
                }
            }
            var zero = new Point(pastedShapes.Min(p => p.Location.X), pastedShapes.Min(p => p.Location.Y));
            foreach (var link in pastedLinks)
            {
                link.Selected = true;
                var source = pastedShapes.FirstOrDefault(x => x.Id == link.SourceId);
                if (source != null)
                {
                    var oldId = source.Id;
                    source.NewGuid();
                    link.SourceId = source.Id;
                }
                var target = pastedShapes.FirstOrDefault(x => x.Id == link.TargetId);
                if (target != null)
                {
                    var oldId = target.Id;
                    target.NewGuid();
                    link.TargetId = target.Id;
                }
            }

            //if (offset is Point point)
            //{
            //    //var p = new Point(
            //    //    pastedShapes.Where(x => x.Selected).Select(x => x.Location.X).Min(),
            //    //    pastedShapes.Where(x => x.Selected).Select(x => x.Location.Y).Min());
            //    var p = Point.Empty;
            //    var size = new Size(point.X, point.Y);
            //    foreach (var shape in pastedShapes)
            //    {
            //        shape.Location = Point.Add(shape.Location, size);
            //    }
            //    foreach (var link in pastedLinks)
            //    {
            //        link.StartPoint = Point.Add(link.StartPoint, size);
            //        link.EndPoint = Point.Add(link.EndPoint, size);
            //        var points = link.GetPoints();
            //        for (var i = 0; i < points.Length; i++)
            //            points[i] = Point.Add(points[i], size);
            //        link.SetPoints(points);
            //    }
            //}

            List<GraphicsPath> list = [];
            foreach (var shape in pastedShapes)
            {
                list.AddRange(shape.GetGraphicsPaths());
                list.AddRange(shape.GetTextPaths());
            }
            foreach (var link in pastedLinks)
            {
                list.AddRange(link.GetLinesPaths());
                list.AddRange(link.GetDotsPaths());
            }
            return ([.. list], rect, Point.Subtract(point ?? Point.Empty, new Size(zero.X, zero.Y)));
        }

        /// <summary>
        /// Вставка фигур и связей по конфигурации из xmodel
        /// </summary>
        /// <param name="xmodel"></param>
        /// <param name="subscribeShapeEvents"></param>
        public static void PastePageFromXml(XElement xmodel, Action<Shape> subscribeShapeEvents)
        {
            if (xmodel == null) return;
            Dictionary<Guid, Guid> guids = [];
            List<Shape> pastedShapes = [];
            List<Link> pastedLinks = [];
            int n = 0;
            foreach (var xelement in xmodel.Descendants())
            {
                if (types.ContainsKey($"{xelement.Name}"))
                {
                    var type = types[$"{xelement.Name}"];
                    var obj = Activator.CreateInstance(type);
                    if (obj is Shape shape)
                    {
                        shape.Index = n++;
                        shape.ReadContent(xelement);
                        subscribeShapeEvents(shape);
                        pastedShapes.Add(shape);

                        if (!guids.ContainsKey(shape.Id))
                        {
                            var id = Guid.NewGuid();
                            guids.Add(shape.Id, id);
                            shape.NewGuid(id);
                        }
                    }
                    else if (obj is Link link)
                    {
                        link.ReadContent(xelement);
                        var oldid = link.SourceId;
                        if (guids.TryGetValue(oldid, out Guid svalue))
                        {
                            link.SourceId = svalue;
                        }
                        oldid = link.TargetId;
                        if (guids.TryGetValue(oldid, out Guid tvalue))
                        {
                            link.TargetId = tvalue;
                        }
                        var source = pastedShapes.FirstOrDefault(x => x.Id == link.SourceId);
                        var target = pastedShapes.FirstOrDefault(x => x.Id == link.TargetId);
                        if (source != null && target != null)
                        {
                            link.LinkToLocation(source, link.SourcePinIndex, link.StartPoint, target, link.TargetPinIndex, link.EndPoint, [.. link.GetPoints()]);
                            pastedLinks.Add(link);
                        }
                    }
                }
            }

            ContextAssistant.ContextShapes.AddRange(pastedShapes);
            ContextAssistant.ContextLinks.AddRange(pastedLinks);
            SortAndAssignToPages();
        }

        public static void PasteFromXml(XElement xmodel, Point point, Action<Shape> subscribeShapeEvents)
        {
            if (xmodel == null) return;
            Dictionary<Guid, Guid> guids = [];
            List<Shape> pastedShapes = [];
            List<IPageSourceLink> pastedPageLinks = [];
            List<Link> pastedLinks = [];
            int n = 0;
            foreach (var xelement in xmodel.Descendants())
            {
                if (types.ContainsKey($"{xelement.Name}"))
                {
                    var type = types[$"{xelement.Name}"];
                    var obj = Activator.CreateInstance(type);
                    if (obj is Shape shape)
                    {
                        shape.Selected = true;
                        shape.Index = n++;
                        shape.ReadContent(xelement);
                        subscribeShapeEvents(shape);
                        pastedShapes.Add(shape);

                        if (shape is IPageSourceLink psl)
                            pastedPageLinks.Add(psl);

                        if (!guids.ContainsKey(shape.Id))
                        {
                            var id = Guid.NewGuid();
                            guids.Add(shape.Id, id);
                            shape.NewGuid(id);
                        }
                    }
                    else if (obj is Link link)
                    {
                        link.ReadContent(xelement);
                        var oldid = link.SourceId;
                        if (guids.TryGetValue(oldid, out Guid svalue))
                        {
                            link.SourceId = svalue;
                        }
                        oldid = link.TargetId;
                        if (guids.TryGetValue(oldid, out Guid tvalue))
                        {
                            link.TargetId = tvalue;
                        }
                        var source = pastedShapes.FirstOrDefault(x => x.Id == link.SourceId);
                        var target = pastedShapes.FirstOrDefault(x => x.Id == link.TargetId);
                        if (source != null && target != null)
                        {
                            link.LinkToLocation(source, link.SourcePinIndex, link.StartPoint, target, link.TargetPinIndex, link.EndPoint, [.. link.GetPoints()]);
                            link.Selected = true;
                            pastedLinks.Add(link);
                        }
                    }
                }
            }

            foreach (var psl in pastedPageLinks)
            {
                foreach (var id in psl.GetIdsForConnect())
                {
                    if (guids.TryGetValue(id, out Guid value))
                        psl.ReplaceTargetId(id, value);
                }
            }

            ContextAssistant.ContextShapes.AddRange(pastedShapes);
            ContextAssistant.ContextLinks.AddRange(pastedLinks);

            var p = new Point(
                pastedShapes.Where(x => x.Selected).Select(x => x.Location.X).Min(),
                pastedShapes.Where(x => x.Selected).Select(x => x.Location.Y).Min());
            foreach (var shape in pastedShapes)
            {
                shape.Location = Point.Add(shape.Location, new Size(point.X - p.X, point.Y - p.Y));
                shape.Location = MovePointToGrid(shape.Location);
            }
            foreach (var link in pastedLinks)
            {
                link.StartPoint = Point.Add(link.StartPoint, new Size(point.X - p.X, point.Y - p.Y));
                link.EndPoint = Point.Add(link.EndPoint, new Size(point.X - p.X, point.Y - p.Y));
                var points = link.GetPoints();
                for (var i = 0; i < points.Length; i++)
                    points[i] = Point.Add(points[i], new Size(point.X - p.X, point.Y - p.Y));
                link.SetPoints(points);

                link.StartPoint = MovePointToGrid(link.StartPoint);
                link.EndPoint = MovePointToGrid(link.EndPoint);
                points = link.GetPoints();
                for (var i = 0; i < points.Length; i++)
                    points[i] = MovePointToGrid(points[i]);
                link.SetPoints(points);
            }
            SortAndAssignToPages();
        }

        public static int ReadElements(int n, XElement xpage, Action<Shape> subscribeShapeEvents)
        {
            foreach (var xelement in xpage.Descendants())
            {
                if (types.ContainsKey($"{xelement.Name}"))
                {
                    var type = types[$"{xelement.Name}"];
                    var obj = Activator.CreateInstance(type);
                    if (obj is Shape shape)
                    {
                        shape.Index = n++;
                        shape.ReadContent(xelement);
                        subscribeShapeEvents(shape);
                        ContextAssistant.ContextShapes.Add(shape);
                    }
                    else if (obj is Link link)
                    {
                        link.ReadContent(xelement);
                        var source = ContextAssistant.ContextShapes.FirstOrDefault(x => x.Id == link.SourceId);
                        var target = ContextAssistant.ContextShapes.FirstOrDefault(x => x.Id == link.TargetId);
                        if (source != null && target != null)
                        {
                            link.LinkToLocation(source, link.SourcePinIndex, link.StartPoint, target, link.TargetPinIndex, link.EndPoint, [.. link.GetPoints()]);
                            ContextAssistant.ContextLinks.Add(link);
                        }
                    }
                }
            }
            return n;
        }

        public static void UpdateLinksOnPage(Page? page)
        {
            foreach (var item in LinksFrom(page))
                item.SetPoints([]);
            foreach (var item in LinksFrom(page).OrderBy(x => x.Length))
                item.SetPoints([.. TraceAssistant.BuildWaveInField(page, ContextAssistant.ContextShapes, ContextAssistant.ContextLinks, item)]);
        }

        public static void UpdateLinkOnPage(Page? page, Link? link)
        {
            link?.SetPoints([]);
            link?.SetPoints([.. TraceAssistant.BuildWaveInField(page, ContextAssistant.ContextShapes, ContextAssistant.ContextLinks, link)]);
        }

        public static void SortAndAssignToPages()
        {
            ClearCache();

            // сброс номеров страниц
            ContextAssistant.ContextShapes.ForEach(shape => shape.Page = 0);
            ContextAssistant.ContextLinks.ForEach(link => link.Page = 0);

            // назначение номеров страниц на основании попадания в область листа
            foreach (var page in ContextAssistant.ContextPages)
            {
                foreach (var shape in ContextAssistant.ContextShapes.Where(x => x.Page == 0))
                {
                    var p = ContextAssistant.ContextPages.FirstOrDefault(x => x.Bounds.IntersectsWith(shape.Bounds));
                    if (p != null) shape.Page = p.Number;
                }
                foreach (var link in ContextAssistant.ContextLinks.Where(x => x.Page == 0))
                {
                    var p = ContextAssistant.ContextPages.FirstOrDefault(x => x.Bounds.IntersectsWith(link.Bounds));
                    if (p != null) link.Page = p.Number;
                }
            }

            // назначение порядкового индекса для фигур
            var n = 0;
            foreach (var shape in ContextAssistant.ContextShapes.OrderBy(x => x.Page).ThenBy(x => x.Location.X).ThenBy(x => x.Location.Y))
            {
                shape.Index = n;    // назначение порядкового индекса
                n++;
            }
            ContextAssistant.ContextShapes.Sort(new ShapesComparer());

            // назначение порядкового индекса для исходящих внеших ссылок
            n = 1;
            foreach (var shape in ContextAssistant.ContextShapes.OrderBy(x => x.Index))
            {
                if (shape is IPageSourceLink link)
                {
                    link.SetLinkNumber(n++);
                }
            }

            ConnectExternalLinks();
        }

        class ShapesComparer : IComparer<Shape>
        {
            public int Compare(Shape? x, Shape? y)
            {
                return x == null || y == null ? 0 : x.Index > y.Index ? 1 : x.Index < y.Index ? -1 : 0;
            }
        }

        public static void ConnectExternalLinks()
        {
            // подключение внешних ссылок
            foreach (var shape in ContextAssistant.ContextShapes)
            {
                if (shape is IPageSourceLink plink)
                {
                    foreach (var id in plink.GetIdsForConnect())
                    {
                        var target = ContextAssistant.ContextShapes.FirstOrDefault(shape => shape.Id == id);
                        if (target == null) continue;
                        plink.AddTarget(target);
                    }
                }
            }
        }

        public static void RemoveSelected(Action<Shape> unsubscribeAction)
        {
            // сбор элементов для удаления на основании их выбранности
            var shapesForDelete = ContextAssistant.ContextShapes.Where(x => x.Selected).ToList();
            // сбор связей, которые используются удаляемыми элементами
            var linksForDelete = ContextAssistant.ContextLinks.Where(x => shapesForDelete.Any(y => y == x.Source || y == x.Target)).ToList();
            List<int> pageIndexes = [];
            foreach (var link in linksForDelete)
            {
                // ищем источник и цель
                var source = ContextAssistant.ContextShapes.FirstOrDefault(x => x == link.Source);
                var target = ContextAssistant.ContextShapes.FirstOrDefault(x => x == link.Target);

                // если найдены оба, то отписывается
                if (source != null && target != null)
                    link.UnlinkToLocation(source, target);

                pageIndexes.Add(link.Page);

                // удаляем визуальную ссылку
                ContextAssistant.ContextLinks.Remove(link);
            }
            // для всех удаляемых элементов
            foreach (var shape in shapesForDelete)
                unsubscribeAction(shape);
            foreach (var shape in shapesForDelete)
            {
                pageIndexes.Add(shape.Page);

                shape.Delete();
                ContextAssistant.ContextShapes.Remove(shape);
            }
            ModelAssistant.SortAndAssignToPages();

            foreach (var index in pageIndexes.Distinct())
            {
                var page = ContextAssistant.ContextPages.FirstOrDefault(x => x.Number == index);
                ModelAssistant.UpdateLinksOnPage(page);
            }
        }

        public static void RemoveInPage(Page? page, Action<Shape> unsubscribeAction)
        {
            // сбор элементов для удаления на основании их выбранности
            var shapesForDelete = ContextAssistant.ContextShapes.Where(x => page != null && x.Page == page.Number || page == null && x.Page == 0).ToList();
            // сбор связей, которые используются удаляемыми элементами
            var linksForDelete = ContextAssistant.ContextLinks.Where(x => shapesForDelete.Any(y => y == x.Source || y == x.Target)).ToList();
            List<int> pageIndexes = [];
            foreach (var link in linksForDelete)
            {
                // ищем источник и цель
                var source = ContextAssistant.ContextShapes.FirstOrDefault(x => x == link.Source);
                var target = ContextAssistant.ContextShapes.FirstOrDefault(x => x == link.Target);

                // если найдены оба, то отписывается
                if (source != null && target != null)
                    link.UnlinkToLocation(source, target);

                pageIndexes.Add(link.Page);

                // удаляем визуальную ссылку
                ContextAssistant.ContextLinks.Remove(link);
            }
            // для всех удаляемых элементов
            foreach (var shape in shapesForDelete)
                unsubscribeAction(shape);
            foreach (var shape in shapesForDelete)
            {
                pageIndexes.Add(shape.Page);

                shape.Delete();
                ContextAssistant.ContextShapes.Remove(shape);
            }
            SortAndAssignToPages();

            foreach (var index in pageIndexes.Distinct())
                UpdateLinksOnPage(page);
        }
    }
}
