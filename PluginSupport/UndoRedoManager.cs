namespace PluginSupport
{
    public class UndoRedoManager
    {
        private Stack<ICommand> UndoStack { get; set; } = new Stack<ICommand>();
        public List<ICommand> UndoHistory { get; set; } = [];
        private Stack<ICommand> RedoStack { get; set; } = new Stack<ICommand>();
        public List<ICommand> RedoHistory { get; set; } = [];

        public event EventHandler? OnStateChaned;

        public void Undo()
        {
            if (UndoStack.Count > 0)
            {
                //изымаем команду из стека
                var command = UndoStack.Pop();
                UndoHistory.RemoveAt(0);
                //отменяем действие команды
                command.UnExecute();
                //заносим команду в стек Redo
                RedoStack.Push(command);
                RedoHistory.Insert(0, command);
                //сигнализируем об изменениях
                StateChanged();
            }
        }

        public void Redo()
        {
            if (RedoStack.Count > 0)
            {
                //изымаем команду из стека
                var command = RedoStack.Pop();
                RedoHistory.RemoveAt(0);
                //выполняем действие команды
                command.Execute();
                //заносим команду в стек Undo
                UndoStack.Push(command);
                UndoHistory.Insert(0, command);
                //сигнализируем об изменениях
                StateChanged();
            }
        }

        //выполняем команду
        public void Execute(ICommand command)
        {
            //выполняем команду
            command.Execute();
            //заносим в стек Undo
            UndoStack.Push(command);
            UndoHistory.Insert(0, command);
            //очищаем стек Redo
            RedoStack.Clear();
            RedoHistory.Clear();
            //сигнализируем об изменениях
            StateChanged();
        }

        private void StateChanged()
        {
            OnStateChaned?.Invoke(this, EventArgs.Empty);
        }

        public bool UndoPossible()
        {
            return UndoStack.Count > 0;
        }

        public bool RedoPossible()
        {
            return RedoStack.Count > 0;
        }

        public void Clear()
        {
            UndoStack.Clear();
            UndoHistory.Clear();
            RedoStack.Clear();
            RedoHistory.Clear();
        }
    }
}
