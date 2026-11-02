using PluginSupport;
using PluginSupport.Assistants;

namespace CodeGenerator.Commands
{
    public class PagePropertyChangeCommand(ContextKind context, Page page, string propertyName, object? oldValue, object? newValue) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly int pageNumber = page.Number;
        private readonly string propertyName = propertyName;
        private readonly object? oldValue = oldValue;
        private readonly object? newValue = newValue;

        public string Name => "Изменить значение свойства листа";

        public void Execute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            var founded = ContextAssistant.ContextPages.FirstOrDefault(x => x.Number == pageNumber);
            if (founded != null)
                ModelAssistant.ChangePropertyByName(founded, propertyName, newValue);
            ContextAssistant.Context = oldcontext;
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            var founded = ContextAssistant.ContextPages.FirstOrDefault(x => x.Number == pageNumber);
            if (founded != null)
                ModelAssistant.ChangePropertyByName(founded, propertyName, oldValue);
            ContextAssistant.Context = oldcontext;
        }
    }
}
