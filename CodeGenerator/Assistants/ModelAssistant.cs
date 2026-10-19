using PluginSupport;

namespace CodeGenerator
{
    public static class ModelAssistant
    {

        private static readonly Dictionary<Page, List<Shape>> cacheShapes = [];
        private static readonly List<Shape> cacheNoPageShapes = [];

        public static void ClearCache()
        {
            cacheShapes.Clear();
            cacheNoPageShapes.Clear();
        }

        public static List<Shape> ShapesFrom(Page? page, IEnumerable<Shape> shapes)
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
            var result = shapes.Where(x => page == null && x.Page == 0 || page != null && x.Page == page.Number).ToList();
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

        public static List<Link> LinksFrom(Page? page, IEnumerable<Link> links)
        {
            return [.. links.Where(x => page == null && x.Page == 0 || page != null && x.Page == page.Number)];
        }

        public static void SelectAllOnPage(Page? page, List<Shape>? shapes, List<Link>? links)
        {
            if (shapes != null)
                ShapesFrom(page, shapes).ForEach(x => x.Selected = true);
            if (links != null)
                LinksFrom(page, links).ForEach(x => x.Selected = true);
        }

        public static void UnselectAll(List<Shape>? shapes, List<Link>? links)
        {
            shapes?.ForEach(shape => { shape.Selected = false; shape.Hover = false; });
            links?.ForEach(link => { link.Selected = false; link.Hover = false; });
        }

        public static void UnselectAllOnPage(Page? page, List<Shape>? shapes, List<Link>? links)
        {
            if (shapes != null)
                ShapesFrom(page, shapes).ForEach(shape => { shape.Selected = false; shape.Hover = false; });
            if (links != null)
                LinksFrom(page, links).ForEach(link => { link.Selected = false; link.Hover = false; });
        }
    }
}
