using PluginSupport;

#pragma warning disable IDE0130 // Пространство имен (namespace) не соответствует структуре папок.
namespace CodeGenerator
#pragma warning restore IDE0130 // Пространство имен (namespace) не соответствует структуре папок.
{
    public enum ContextKind
    {
        Model,
        Field,
    }

    public static class ContextAssistant
    {
        public static ContextKind Context { get; set; } = ContextKind.Model;

        private static readonly List<Page> pagesModel = [];
        private static readonly List<Shape> shapesModel = [];
        private static readonly List<Link> linksModel = [];
        private static KeepEnvironment environmentModel = new();
        private static readonly List<Page> pagesField = [];
        private static readonly List<Shape> shapesField = [];
        private static readonly List<Link> linksField = [];
        private static KeepEnvironment environmentField = new();

        public static KeepEnvironment ContextEnvironment 
        {
            get
            {
                return Context switch
                {
                    ContextKind.Model => environmentModel,
                    ContextKind.Field => environmentField,
                    _ => new KeepEnvironment(),
                };
            }
            set
            {
                switch (Context)
                {
                    case ContextKind.Model:
                        environmentModel.SetValues(value);
                        break;
                    case ContextKind.Field:
                        environmentField.SetValues(value);
                        break;
                }
            }
        }

        public static List<Page> ContextPages 
        { 
            get
            {
                return Context switch
                {
                    ContextKind.Model => pagesModel,
                    ContextKind.Field => pagesField,
                    _ => [],
                };
            }
        }

        public static Dictionary<ContextKind, List<Shape>> AllShapes
        {
            get 
            {
                Dictionary<ContextKind, List<Shape>> dict = [];
                dict.Add(ContextKind.Model, shapesModel);
                dict.Add(ContextKind.Field, shapesField);
                return dict;
            }
        }

        public static List<Shape> ContextShapes 
        {
            get
            {
                return Context switch
                {
                    ContextKind.Model => shapesModel,
                    ContextKind.Field => shapesField,
                    _ => [],
                };
            }
        }

        public static List<Link> ContextLinks 
        {
            get
            {
                return Context switch
                {
                    ContextKind.Model => linksModel,
                    ContextKind.Field => linksField,
                    _ => [],
                };
            }
        }
    }
}
