using CodeGenerator.Assistants;
using PluginSupport;

namespace CodeGenerator.Commands
{
    public class MoveShapesAndLinksCommand(
        ContextKind context,
        Page? page, int dx, int dy,
        Shape[] shapes) : ICommand
    {
        private readonly ContextKind commandContext = context;
        private readonly Page? page = page;
        private readonly int dx = dx;
        private readonly int dy = dy;
        private readonly Shape[] shapes = shapes;

        public string Name => "Перетащить мышью выделенные фигуры и связи между ними";

        public void Execute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            foreach (var shape in shapes)
                shape.Location = Point.Add(shape.Location, new Size(dx, dy));
            foreach (var link in ModelAssistant.LinksFrom(page))
            {
                var source = ModelAssistant.ShapesFrom(page).FirstOrDefault(x => x.Id == link.SourceId);
                if (source != null && source.GetOutputPinPoint(link.SourcePinIndex) is Point sourcePinPoint)
                    link.StartPoint = sourcePinPoint;
                var target = ModelAssistant.ShapesFrom(page).FirstOrDefault(x => x.Id == link.TargetId);
                if (target != null && target.GetInputPinPoint(link.TargetPinIndex) is Point targetPinPoint)
                    link.EndPoint = targetPinPoint;
            }
            ModelAssistant.UpdateLinksOnPage(page);
            ContextAssistant.Context = oldcontext;
        }

        public void UnExecute()
        {
            var oldcontext = ContextAssistant.Context;
            ContextAssistant.Context = commandContext;
            foreach (var shape in shapes)
                shape.Location = Point.Add(shape.Location, new Size(-dx, -dy));
            foreach (var link in ModelAssistant.LinksFrom(page))
            {
                var source = ModelAssistant.ShapesFrom(page).FirstOrDefault(x => x.Id == link.SourceId);
                if (source != null && source.GetOutputPinPoint(link.SourcePinIndex) is Point sourcePinPoint)
                    link.StartPoint = sourcePinPoint;
                var target = ModelAssistant.ShapesFrom(page).FirstOrDefault(x => x.Id == link.TargetId);
                if (target != null && target.GetInputPinPoint(link.TargetPinIndex) is Point targetPinPoint)
                    link.EndPoint = targetPinPoint;
            }
            ModelAssistant.UpdateLinksOnPage(page);
            ContextAssistant.Context = oldcontext;
        }
    }
}
