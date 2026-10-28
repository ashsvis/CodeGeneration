using PluginSupport;
using PluginSupport.Assistants;

namespace CodeGenerator.Commands
{
    public class BuildLinkCommand(ContextKind context, Page? page, Shape source, int sourceIndex, Shape target, int targetIndex) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly Shape sourceShape = source;
        private readonly int sourceIndex = sourceIndex;
        private readonly Shape targetShape = target;
        private readonly int targetIndex = targetIndex;
        private readonly Page? page = page;
        private PluginSupport.Link? createdLink;

        public string Name => "Построить новую связь";

        public void Execute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            // создание представления связи
            var type = targetShape.GetLinkTypeToCreate();
            var link = (PluginSupport.Link?)Activator.CreateInstance(type);
            if (link != null)
            {
                createdLink = link;
                var startPoint = sourceShape.GetOutputPinPoint(sourceIndex);
                var endPoint = targetShape.GetInputPinPoint(targetIndex);
                if (startPoint != null && endPoint != null)
                {
                    link.StartPoint = (Point)startPoint;
                    link.EndPoint = (Point)endPoint;
                    // построение волны и точек визуальной связи
                    var points = TraceAssistant.BuildWaveInField(page, link);

                    link.LinkToLocation(sourceShape, sourceIndex, link.StartPoint, targetShape, targetIndex, link.EndPoint, points);
                    ContextAssistant.ContextLinks.Add(link);
                    // настройка фигуры для установления связи
                    targetShape.SetInputValue(targetIndex, sourceShape.GetOutputValue(sourceIndex));

                    // сохранение настроек для визуальной связи
                    link.Source = sourceShape;
                    link.SourcePinIndex = sourceIndex;
                    link.Target = targetShape;
                    link.TargetPinIndex = targetIndex;
                    // сохранение настроек связи для источника
                    sourceShape.SetOutputTarget(link.SourcePinIndex, link.Target, link.TargetPinIndex, link.Id);
                    // сохранение настроек связи для цели
                    targetShape.SetInputSource(link.TargetPinIndex, link.Source, link.SourcePinIndex, link.Id);
                    ModelAssistant.SortAndAssignToPages();
                }
            }
            ContextAssistant.Context = oldcontext;
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;

            if (createdLink is Link link)
            {
                // ищем источник и цель
                var source = ContextAssistant.ContextShapes.FirstOrDefault(x => x == link.Source && link.SourcePinIndex == sourceIndex);
                var target = ContextAssistant.ContextShapes.FirstOrDefault(x => x == link.Target && link.TargetPinIndex == targetIndex);
                // если найдены оба, то отписывается
                if (source != null && target != null)
                    link.UnlinkToLocation(source, target);
                if (source is IPageTargetLink plink)
                    plink.RemoveTargetFromOutput(target, link.TargetPinIndex);
                // удаляем визуальную ссылку
                ContextAssistant.ContextLinks.Remove(link);
                var page = ContextAssistant.ContextPages.FirstOrDefault(x => x.Number == link.Page);
                ModelAssistant.UpdateLinkOnPage(page, link);
            }

            ContextAssistant.Context = oldcontext;
        }
    }
}
