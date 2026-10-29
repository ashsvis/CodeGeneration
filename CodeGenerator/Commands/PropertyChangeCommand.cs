using PluginSupport;
using PluginSupport.Assistants;

namespace CodeGenerator.Commands
{
    public class PropertyChangeCommand(ContextKind context, Shape shape, string propertyName, object? oldValue, object? newValue) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly Guid shapeId = shape.Id;
        private readonly string propertyName = propertyName;
        private readonly object? oldValue = oldValue;
        private readonly object? newValue = newValue;

        public string Name => "Изменить значение свойства";

        public void Execute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            var founded = ContextAssistant.ContextShapes.FirstOrDefault(x => x.Id == shapeId);
            if (founded != null)
                ModelAssistant.ChangePropertyByName(founded, propertyName, newValue);
            ContextAssistant.Context = oldcontext;
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            var founded = ContextAssistant.ContextShapes.FirstOrDefault(x => x.Id == shapeId);
            if (founded != null)
                ModelAssistant.ChangePropertyByName(founded, propertyName, oldValue);
            ContextAssistant.Context = oldcontext;
        }
    }
}
