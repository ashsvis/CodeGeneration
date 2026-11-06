using PluginSupport;
using PluginSupport.Assistants;
using System.Xml.Linq;

namespace CodeGenerator.Commands
{
    public class InsertBlankPageCommand(ContextKind context, int beforePageNumber, int totalPagesInRow, Action<Shape> subscribeAction) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly int beforePageNumber = beforePageNumber;
        private readonly int totalPagesInRow = totalPagesInRow;
        private XElement? xundo;
        private readonly Action<Shape> subscribeAction = subscribeAction;

        public string Name => "Вставить пустой лист перед указанным";

        public void Execute()
        {
            xundo = ModelAssistant.SaveUndoXml();
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            RightShiftDrawPositionForShapesAndLinks(beforePageNumber, totalPagesInRow);
            foreach (var page in ContextAssistant.ContextPages
                .Where(x => x.Number >= beforePageNumber)
                .OrderBy(x => x.Number).ToList())
            {
                page.Number++;
                page.Row = (page.Number - 1) / totalPagesInRow;
                page.Column = (page.Number - 1) - page.Row * totalPagesInRow;
            }
            var p = beforePageNumber;
            var row = (p - 1) / totalPagesInRow;
            var column = (p - 1) - row * totalPagesInRow;
            ContextAssistant.ContextPages.Insert(beforePageNumber - 1, new Page(p, 420, 297, row, column) { PagesCount = p });
            if (ContextAssistant.ContextPages.Count > 0) ContextAssistant.ContextPages[0].PagesCount = ContextAssistant.ContextPages.Count;
            ModelAssistant.SortAndAssignToPages();
            ContextAssistant.Context = oldcontext;
        }

        private static void RightShiftDrawPositionForShapesAndLinks(int pageNumber, int totalPagesInRow)
        {
            var dx = 420 * Page.Koeff;
            var dy = 297 * Page.Koeff;
            foreach (var shape in ContextAssistant.ContextShapes
                .Where(x => ModelAssistant.GetPageNumber(x) >= pageNumber)
                .OrderByDescending(ModelAssistant.GetPageNumber).ToList())
            {
                var p = ModelAssistant.GetPageNumber(shape);
                var row = (p - 1) / totalPagesInRow;
                var column = (p - 1) - row * totalPagesInRow;
                if (column == totalPagesInRow - 1)
                    shape.Shift(-dx * (totalPagesInRow - 1), dy);
                else
                    shape.Shift(dx, 0);
            }
            foreach (var link in ContextAssistant.ContextLinks
                .Where(x => ModelAssistant.GetPageNumber(x) >= pageNumber)
                .OrderByDescending(ModelAssistant.GetPageNumber).ToList())
            {
                var p = ModelAssistant.GetPageNumber(link);
                var row = (p - 1) / totalPagesInRow;
                var column = (p - 1) - row * totalPagesInRow;
                if (column == totalPagesInRow - 1)
                    link.Shift(-dx * (totalPagesInRow - 1), dy);
                else
                    link.Shift(dx, 0);
            }
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            ContextAssistant.ContextPages.RemoveAt(ContextAssistant.ContextPages.Count - 1);
            ModelAssistant.LoadUndoXml(xundo, subscribeAction);
            if (ContextAssistant.ContextPages.Count > 0) ContextAssistant.ContextPages[0].PagesCount = ContextAssistant.ContextPages.Count;
            ModelAssistant.SortAndAssignToPages();
            ContextAssistant.Context = oldcontext;
        }
    }
}
