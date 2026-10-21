namespace CodeGenerator
{
    public interface IUndoRedoSupport
    {
        void Undo();
        void Redo();
        bool CanUndo();
        bool CanRedo();
    }
}
