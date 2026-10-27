using PluginSupport;
using PluginSupport.Assistants;
using System.Xml.Linq;

namespace CodeGenerator.Commands
{
    public class RemoveShapeCommand(ContextKind context, Shape shape, Action<Shape> subscribeAction, Action<Shape> unsubscribeAction) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly Guid shapeId = shape.Id;
        private XElement? xundo;
        private readonly Action<Shape> subscribeAction = subscribeAction;
        private readonly Action<Shape> unsubscribeAction = unsubscribeAction;

        public string Name => "Удалить указанную фигуру";

        public void Execute()
        {
            xundo = ModelAssistant.SaveUndoXml();
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            var founded = ContextAssistant.ContextShapes.FirstOrDefault(x => x.Id == shapeId);
            if (founded != null)
                ModelAssistant.RemoveShape(founded, unsubscribeAction);
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
