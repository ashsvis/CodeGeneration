using CodeGenerator.Assistants;
using PluginSupport;
using System.Xml.Linq;

namespace CodeGenerator.Commands
{
    public class PasteShapesAndLinksCommand(
        ContextKind context,
        Page? page,
        XElement xElementsBeforeCopyCuted,
        XElement xElementsCopyCuted,
        Point point,
        Action<Shape> subscribeAction,
        Action<Shape> unsubscribeAction) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly Page? page = page;
        private readonly XElement xElementsBeforeCopyCuted = xElementsBeforeCopyCuted;
        private readonly XElement xElementsCopyCuted = xElementsCopyCuted;
        private readonly Point point = point;
        private readonly Action<Shape> subscribeShapeEvents = subscribeAction;
        private readonly Action<Shape> unsubscribeShapeEvents = unsubscribeAction;

        public string Name => "Вставить группу фигур, ранее скопированных или вырезанных, и связи между ними";

        public void Execute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            ModelAssistant.PasteFromXml(xElementsCopyCuted, point, subscribeShapeEvents);
            ContextAssistant.Context = oldcontext;
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            ModelAssistant.RemoveInPage(page, unsubscribeShapeEvents);
            ModelAssistant.PastePageFromXml(xElementsBeforeCopyCuted, subscribeShapeEvents);
            ContextAssistant.Context = oldcontext;
        }
    }
}
