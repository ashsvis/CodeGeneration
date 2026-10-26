using CodeGenerator.Assistants;
using PluginSupport;
using System.Xml.Linq;

namespace CodeGenerator.Commands
{
    public class PasteShapesAndLinksCommand(
        ContextKind context,
        Page? page,
        XElement xElementsCopyCuted,
        Point point,
        Action<Shape> subscribeAction) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly Page? page = page;
        private readonly XElement xElementsCopyCuted = xElementsCopyCuted;
        private readonly Point point = point;
        private readonly Action<Shape> subscribeAction = subscribeAction;
        private XElement? xundo;

        public string Name => "Вставить группу фигур, ранее скопированных или вырезанных, и связи между ними";

        public void Execute()
        {
            xundo = ModelAssistant.SaveUndoXml();
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            ModelAssistant.PasteFromXml(xElementsCopyCuted, point, subscribeAction);
            ContextAssistant.Context = oldcontext;
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            ModelAssistant.LoadUndoXml(xundo, subscribeAction);
            ContextAssistant.Context = oldcontext;
        }
    }
}
