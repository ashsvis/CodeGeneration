using PluginSupport;

namespace LogicModel
{
    public partial class PluginLogic : IPlugin
    {
        public string Name => "Элементы логики";

        public TreeNode[] TreeNodeItems()
        {
            List<TreeNode> items = [];
            TreeNode nodesGroup;
            nodesGroup = new TreeNode("Текст");
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("Комментарий") { Tag = typeof(Comment) });
            nodesGroup.Nodes.Add(new TreeNode("Дескриптор сигнала") { Tag = typeof(DescriptionBox) });
            //
            nodesGroup = new TreeNode("Ввод сигнала");
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("Тег входной дискретный") { Tag = typeof(DigitalInputTag) });
            nodesGroup.Nodes.Add(new TreeNode("Тег входной аналоговый") { Tag = typeof(AnalogInputTag) });
            nodesGroup.Nodes.Add(new TreeNode("Дискретная входящая ссылка") { Tag = typeof(DigitalInputPageLink) });
            nodesGroup.Nodes.Add(new TreeNode("Дискретный ввод") { Tag = typeof(DigitalInput) });
            nodesGroup.Nodes.Add(new TreeNode("Аналоговая входящая ссылка") { Tag = typeof(AnalogInputPageLink) });
            nodesGroup.Nodes.Add(new TreeNode("Аналоговый ввод") { Tag = typeof(AnalogInput) });
            nodesGroup.Nodes.Add(new TreeNode("Флаг") { Tag = typeof(Flag) });
            nodesGroup.Nodes.Add(new TreeNode("Число") { Tag = typeof(Numeric) });
            //
            nodesGroup = new TreeNode("Вывод сигнала");
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("Дискретный вывод") { Tag = typeof(DigitalOutput) });
            nodesGroup.Nodes.Add(new TreeNode("Дискретная исходящая ссылка") { Tag = typeof(DigitalOutputPageLink) });
            nodesGroup.Nodes.Add(new TreeNode("Аналоговый вывод") { Tag = typeof(AnalogOutput) });
            nodesGroup.Nodes.Add(new TreeNode("Аналоговая исходящая ссылка") { Tag = typeof(AnalogOutputPageLink) });
            nodesGroup.Nodes.Add(new TreeNode("Тег выходной дискретный") { Tag = typeof(DigitalOutputTag) });
            nodesGroup.Nodes.Add(new TreeNode("Тег выходной аналоговый") { Tag = typeof(AnalogOutputTag) });
            //
            items.Add(new TreeNode("Инверсия") { Tag = typeof(Not) });
            AddTreeNodeLogicItems(items);
            items.Add(new TreeNode("Исключающее ИЛИ") { Tag = typeof(Xor) });
            //
            nodesGroup = new TreeNode("Триггеры");
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("RS-триггер") { Tag = typeof(Rs) });
            nodesGroup.Nodes.Add(new TreeNode("SR-триггер") { Tag = typeof(Sr) });
            nodesGroup.Nodes.Add(new TreeNode("D-триггер") { Tag = typeof(Dt) });
            //
            nodesGroup = new TreeNode("Детектор фронта") { Tag = typeof(Rtrig) };
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("Детектор фронта") { Tag = typeof(Rtrig) });
            nodesGroup.Nodes.Add(new TreeNode("Детектор спада") { Tag = typeof(Ftrig) });
            //
            nodesGroup = new TreeNode("Таймеры");
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("Одновибратор") { Tag = typeof(Pulse) });
            nodesGroup.Nodes.Add(new TreeNode("Задержка включения") { Tag = typeof(Ton) });
            nodesGroup.Nodes.Add(new TreeNode("Задержка выключения") { Tag = typeof(Toff) });
            //
            nodesGroup = new TreeNode("Селекторы");
            items.Add(nodesGroup);
            nodesGroup.Nodes.Add(new TreeNode("Выбор дискретного сигнала") { Tag = typeof(Seld) });
            nodesGroup.Nodes.Add(new TreeNode("Выбор аналогового сигнала") { Tag = typeof(Sela) });
            //
            nodesGroup = new TreeNode("Арифметика");
            items.Add(nodesGroup);
            AddTreeNodeArithmeticItems(nodesGroup.Nodes);
            //
            nodesGroup = new TreeNode("Сравнение");
            items.Add(nodesGroup);
            AddTreeNodeCompareItems(nodesGroup.Nodes);
            return [.. items];
        }

        static partial void AddTreeNodeLogicItems(List<TreeNode> items);
        static partial void AddTreeNodeCompareItems(TreeNodeCollection items);
        static partial void AddTreeNodeArithmeticItems(TreeNodeCollection items);

        public Dictionary<string, Type> GetTypes()
        {
            Dictionary<string, Type> types = [];
            var range = this.TreeNodeItems();
            foreach (var category in range)
            {
                if (category.Tag is Type type && !string.IsNullOrEmpty(type.FullName))
                    types.TryAdd(type.FullName, type);
                foreach (var node in category.Nodes.Cast<TreeNode>())
                {
                    if (node.Tag is Type childType && !string.IsNullOrEmpty(childType.FullName))
                        types.TryAdd(childType.FullName, childType);
                }
            }
            var linkType = typeof(Link);
            if (!string.IsNullOrEmpty(linkType.FullName))
                types.Add(linkType.FullName, linkType);
            return types;
        }
    }
}
