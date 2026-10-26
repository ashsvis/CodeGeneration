using CodeGenerator.Assistants;
using PluginSupport;
using System.Xml.Linq;

namespace CodeGenerator.Commands
{
    public class RemoveLinkCommand(ContextKind context, DeleteLinkFromTargetEventArgs info, Action<Shape> subscribeAction) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly Guid? sourceId = info.Source?.Id;
        private readonly Guid? targetId = info.Target?.Id;
        private readonly int targetPinIndex = info.TargetPinIndex;
        private XElement? xundo;
        private readonly Action<Shape> subscribeAction = subscribeAction;

        public string Name => "Удалить указанную ссылку";

        public void Execute()
        {
            xundo = ModelAssistant.SaveUndoXml();
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;

            var foundedSource = ContextAssistant.ContextShapes.FirstOrDefault(x => x.Id == sourceId);
            var foundedTarget = ContextAssistant.ContextShapes.FirstOrDefault(x => x.Id == targetId);

            var linksForDelete = ContextAssistant.ContextLinks.Where(x => x.Source == foundedSource &&
                x.Target == foundedTarget && x.TargetPinIndex == targetPinIndex).ToList();
            List<int> pageIndexes = [];
            foreach (var link in linksForDelete)
            {
                // ищем источник и цель
                var source = ContextAssistant.ContextShapes.FirstOrDefault(x => x == link.Source && link.SourcePinIndex == 0);
                var target = ContextAssistant.ContextShapes.FirstOrDefault(x => x == link.Target && link.TargetPinIndex == targetPinIndex);
                // если найдены оба, то отписывается
                if (source != null && target != null)
                    link.UnlinkToLocation(source, target);
                if (source is IPageTargetLink plink)
                    plink.RemoveTargetFromOutput(target, link.TargetPinIndex);
                // удаляем визуальную ссылку
                ContextAssistant.ContextLinks.Remove(link);
                pageIndexes.Add(link.Page);
            }
            foreach (var index in pageIndexes.Distinct())
            {
                var page = ContextAssistant.ContextPages.FirstOrDefault(x => x.Number == index);
                ModelAssistant.UpdateLinksOnPage(page);
            }

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
