using PluginSupport;
using PluginSupport.Assistants;

namespace CodeGenerator.Commands
{
    public class AddBlankPageCommand(ContextKind context, int totalPagesInRow) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly int totalPagesInRow = totalPagesInRow;

        public string Name => "Добавить пустой лист в конец списка";

        public void Execute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            var p = ContextAssistant.ContextPages.Count + 1;
            var row = (p - 1) / totalPagesInRow;
            var column = (p - 1) - row * totalPagesInRow;
            ContextAssistant.ContextPages.Add(new Page(p, 420, 297, row, column) { PagesCount = p });
            if (ContextAssistant.ContextPages.Count > 0) ContextAssistant.ContextPages[0].PagesCount = ContextAssistant.ContextPages.Count;
            ModelAssistant.SortAndAssignToPages();
            ContextAssistant.Context = oldcontext;
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            ContextAssistant.ContextPages.RemoveAt(ContextAssistant.ContextPages.Count - 1);
            if (ContextAssistant.ContextPages.Count > 0) ContextAssistant.ContextPages[0].PagesCount = ContextAssistant.ContextPages.Count;
            ModelAssistant.SortAndAssignToPages();
            ContextAssistant.Context = oldcontext;
        }
    }
}
