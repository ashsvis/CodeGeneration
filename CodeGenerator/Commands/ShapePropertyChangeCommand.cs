using PluginSupport;
using PluginSupport.Assistants;
using System.Diagnostics;

namespace CodeGenerator.Commands
{
    public class ShapePropertyChangeCommand(ContextKind context, Shape shape, string propertyName, object? oldValue, object? newValue) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly Shape shape = shape;
        private readonly Guid shapeId = shape.Id;
        private readonly string propertyName = propertyName;
        private readonly object? oldValue = oldValue;
        private readonly object? newValue = newValue;

        public string Name => "Изменить значение свойства фигуры";

        public void Execute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            var founded = ContextAssistant.ContextShapes.FirstOrDefault(x => x.Id == shapeId);
            if (founded != null)
                ModelAssistant.ChangePropertyByName(founded, propertyName, newValue);
            ContextAssistant.Context = oldcontext;
            var oldStr = oldValue?.GetType() == typeof(string[]) ? string.Join(" ", (string[])oldValue) : $"{oldValue}";
            var newStr = newValue?.GetType() == typeof(string[]) ? string.Join(" ", (string[])newValue) : $"{newValue}";
            Debug.WriteLine($"Execute {shapeId} {shape.GetType()}.{propertyName} ({oldStr} -> {newStr})");
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            var founded = ContextAssistant.ContextShapes.FirstOrDefault(x => x.Id == shapeId);
            if (founded != null)
                ModelAssistant.ChangePropertyByName(founded, propertyName, oldValue);
            ContextAssistant.Context = oldcontext;
            var oldStr = oldValue?.GetType() == typeof(string[]) ? string.Join(" ", (string[])oldValue) : $"{oldValue}";
            var newStr = newValue?.GetType() == typeof(string[]) ? string.Join(" ", (string[])newValue) : $"{newValue}";
            Debug.WriteLine($"UnExecute {shapeId} {shape.GetType()}.{propertyName} ({newStr} -> {oldStr})");
        }
    }
}
