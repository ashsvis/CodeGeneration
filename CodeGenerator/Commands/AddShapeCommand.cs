using PluginSupport;
using PluginSupport.Assistants;

namespace CodeGenerator.Commands
{
    public class AddShapeCommand(ContextKind context, Shape draggedShape, Point insertPoint, Action<Shape> subscribeAction, Action<Shape> unsubscribeAction) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly Shape shape = draggedShape;
        private readonly Guid shapeId = draggedShape.Id;
        private readonly Point point = insertPoint;
        private readonly Action<Shape> subscribeShapeEvents = subscribeAction;
        private readonly Action<Shape> unsubscribeShapeEvents = unsubscribeAction;

        public string Name => "Добавить новую фигуру";

        public void Execute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            shape.Location = point;
            shape.Selected = true;
            subscribeShapeEvents(shape);
            ContextAssistant.ContextShapes.Add(shape);
            ModelAssistant.SortAndAssignToPages();
            ContextAssistant.Context = oldcontext;
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            var founded = ContextAssistant.ContextShapes.FirstOrDefault(x => x.Id == shapeId);
            if (founded != null)
            {
                unsubscribeShapeEvents(founded);
                founded.Delete();
                ContextAssistant.ContextShapes.Remove(founded);
                ModelAssistant.SortAndAssignToPages();
            }
            ContextAssistant.Context = oldcontext;
        }
    }
}
