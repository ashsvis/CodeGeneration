namespace PluginSupport
{
    /// <summary>
    /// Допустимые операции объектных привязок
    /// </summary>
    [Flags]
    public enum AllowedFuncProperties : uint
    {
        None = 0x0,             // ничего нет
        PinInverted = 0x1,      // может инвертировать дискретные входы и выходы
        ShowBorder = 0x2,       // может показывать рамку фигуры
        ShowPins = 0x4,         // может показывать линии пинов
        ShowFuncName = 0x8,     // может показывать текстовое обозначение функции
        ShowLabelNumber = 0x10, // может показывать номер метки фигуры
        // новые режимы добавлять здесь
        All = 0xfffffff,        // всё можно
    }
}
