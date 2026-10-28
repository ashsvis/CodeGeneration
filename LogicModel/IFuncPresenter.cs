using System.Drawing.Drawing2D;

namespace LogicModel
{
    internal interface IFuncPresenter
    {
        string? FuncDesc { get; }
        string[] FuncPresenter { get; }
        bool IsPresent { get; }
        GraphicsPath[] GetGraphicsPaths();
        GraphicsPath[] GetTextPaths();
    }
}
