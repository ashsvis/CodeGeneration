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
    }
}
