using PluginSupport;
using System.Xml.Linq;

namespace CodeGenerator.Commands
{
    public class DragShapesAndLinksCommand(
        ContextKind context,
        Page? page,
        XElement xElementsOnCurrentPageBeforeDrag,
        XElement xElementOnCurrentPageAfterDrag, 
        Action<Shape> subscribeAction,
        Action<Shape> unsubscribeAction) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly Page? page = page;
        private readonly XElement xElementsOnCurrentPageBeforeDrag = xElementsOnCurrentPageBeforeDrag;
        private readonly XElement xElementsOnCurrentPageAfterDrag = xElementOnCurrentPageAfterDrag;
        private readonly Action<Shape> subscribeShapeEvents = subscribeAction;
        private readonly Action<Shape> unsubscribeShapeEvents = unsubscribeAction;

        public string Name => "Перетащить мышью выделенные фигуры и связи между ними";

        public void Execute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            ModelAssistant.RemoveInPage(page, unsubscribeShapeEvents);
            ModelAssistant.PastePageFromXml(xElementsOnCurrentPageAfterDrag, subscribeShapeEvents);
            ContextAssistant.Context = oldcontext;
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            ModelAssistant.RemoveInPage(page, unsubscribeShapeEvents);
            ModelAssistant.PastePageFromXml(xElementsOnCurrentPageBeforeDrag, subscribeShapeEvents);
            ContextAssistant.Context = oldcontext;
        }
    }
}
