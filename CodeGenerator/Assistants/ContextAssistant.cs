#pragma warning disable IDE0130 // Пространство имен (namespace) не соответствует структуре папок.
using PluginSupport;
using System.ComponentModel;

namespace CodeGenerator
#pragma warning restore IDE0130 // Пространство имен (namespace) не соответствует структуре папок.
{
    public enum ContextKind
    {
        B7,
        B8,
    }

    public static class ContextAssistant
    {
        private static ContextKind context = ContextKind.B7;

        public static ContextKind Context 
        { 
            get => context; 
            set 
            {
                if (context == value) return;
                context = value;
                switch (context)
                {
                    case ContextKind.B7:
                        ContextPages = pagesB7;
                        ContextShapes = shapesB7;
                        ContextLinks = linksB7;
                        break;
                    case ContextKind.B8:
                        ContextPages = pagesB8;
                        ContextShapes = shapesB8;
                        ContextLinks = linksB8;
                        break;
                }
            }
        }

        private static readonly List<Page> pagesB7 = [];
        private static readonly List<Shape> shapesB7 = [];
        private static readonly List<Link> linksB7 = [];
        private static readonly List<Page> pagesB8 = [];
        private static readonly List<Shape> shapesB8 = [];
        private static readonly List<Link> linksB8 = [];

        public static List<Page> ContextPages { get; private set; } = [];

        public static List<Shape> ContextShapes { get; private set; } = [];

        [DefaultValue(null)]
        public static List<Link> ContextLinks { get; private set; } = [];
    }
}
