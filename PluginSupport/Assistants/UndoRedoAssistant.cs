namespace PluginSupport.Assistants
{
    public static class UndoRedoAssistant
    {
        private static UndoRedoManager? undoRedoManager = null;

        public static UndoRedoManager UndoRedoManager
        {
            get
            {
                undoRedoManager ??= new();
                return undoRedoManager;
            }
        }
    }
}
