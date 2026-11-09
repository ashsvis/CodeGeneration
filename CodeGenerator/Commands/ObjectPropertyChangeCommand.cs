using PluginSupport;
using PluginSupport.Assistants;
using System.Diagnostics;

namespace CodeGenerator.Commands
{
    //public class ObjectPropertyChangeCommand(ContextKind context, object propertyOwner, string propertyName, object? oldValue, object? newValue) : ICommand
    //{
    //    private readonly ContextKind commandContext = context;
    //    private readonly object propertyOwner = propertyOwner;
    //    private readonly string propertyName = propertyName;
    //    private readonly object? oldValue = oldValue;
    //    private readonly object? newValue = newValue;

    //    public string Name => "Изменить значение свойства листа";

    //    public void Execute()
    //    {
    //        var oldcontext = ContextAssistant.Context;
    //        ContextAssistant.Context = commandContext;
    //        ModelAssistant.ChangePropertyByName(propertyOwner, propertyName, newValue);
    //        ContextAssistant.Context = oldcontext;
    //    }

    //    public void UnExecute()
    //    {
    //        var oldcontext = ContextAssistant.Context;
    //        ContextAssistant.Context = commandContext;
    //        ModelAssistant.ChangePropertyByName(propertyOwner, propertyName, oldValue);
    //        ContextAssistant.Context = oldcontext;
    //    }
    //}
}
