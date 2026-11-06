using PluginSupport;
using PluginSupport.Assistants;
using System.Xml.Linq;

namespace CodeGenerator.Commands
{
    public class RemovePageCommand(ContextKind context, Page removedPage, int totalPagesInRow, Action<Shape> subscribeAction, Action<Shape> unsubscribeAction) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly Page removedPage = removedPage;
        private readonly int pageNumber = removedPage.Number;
        private readonly int totalPagesInRow = totalPagesInRow;
        private XElement? xundo;
        private readonly Action<Shape> subscribeAction = subscribeAction;
        private readonly Action<Shape> unsubscribeAction = unsubscribeAction;

        public string Name => "Удалить указанный лист";

        public void Execute()
        {
            xundo = ModelAssistant.SaveUndoXml();
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            foreach (var shape in ModelAssistant.ShapesFrom(removedPage))
                ModelAssistant.RemoveShape(shape, unsubscribeAction);
            LeftShiftDrawPositionForShapesAndLinks(pageNumber, totalPagesInRow);
            foreach (var page in ContextAssistant.ContextPages
                .Where(x => x.Number >= removedPage.Number)
                .OrderBy(x => x.Number).ToList())
            {
                page.Number--;
                page.Row = (page.Number - 1) / totalPagesInRow;
                page.Column = (page.Number - 1) - page.Row * totalPagesInRow;
            }
            ContextAssistant.ContextPages.Remove(removedPage);
            ModelAssistant.SortAndAssignToPages();
            if (ContextAssistant.ContextPages.Count > 0) ContextAssistant.ContextPages[0].PagesCount = ContextAssistant.ContextPages.Count;
            ContextAssistant.Context = oldcontext;
        }

        private static void LeftShiftDrawPositionForShapesAndLinks(int pageNumber, int totalPagesInRow)
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
                if (column == 0)
                    shape.Shift(dx * (totalPagesInRow - 1), -dy);
                else
                    shape.Shift(-dx, 0);
            }
            foreach (var link in ContextAssistant.ContextLinks
                .Where(x => ModelAssistant.GetPageNumber(x) >= pageNumber)
                .OrderByDescending(ModelAssistant.GetPageNumber).ToList())
            {
                var p = ModelAssistant.GetPageNumber(link);
                var row = (p - 1) / totalPagesInRow;
                var column = (p - 1) - row * totalPagesInRow;
                if (column == 0)
                    link.Shift(dx * (totalPagesInRow - 1), -dy);
                else
                    link.Shift(-dx, 0);
            }
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            ModelAssistant.LoadUndoXml(xundo, subscribeAction);
            ModelAssistant.SortAndAssignToPages();
            if (ContextAssistant.ContextPages.Count > 0) ContextAssistant.ContextPages[0].PagesCount = ContextAssistant.ContextPages.Count;
            ContextAssistant.Context = oldcontext;
        }
    }
}
