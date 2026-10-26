using CodeGenerator.Assistants;
using PluginSupport;

namespace CodeGenerator.Commands
{
    public class InversePinCommand(ContextKind context, Shape shape, bool isOutput, int pinIndex, bool state) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly Guid shapeId = shape.Id;
        private readonly bool isOutput = isOutput;
        private readonly int pinIndex = pinIndex;
        private readonly bool state = state;

        public string Name => "Изменить инверсию пина";

        public void Execute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            var founded = ContextAssistant.ContextShapes.FirstOrDefault(x => x.Id == shapeId);
            founded?.SetInvertStatePinByIndex(isOutput, pinIndex, !state);
            ContextAssistant.Context = oldcontext;
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            var founded = ContextAssistant.ContextShapes.FirstOrDefault(x => x.Id == shapeId);
            founded?.SetInvertStatePinByIndex(isOutput, pinIndex, state);
            ContextAssistant.Context = oldcontext;
        }
    }
}
