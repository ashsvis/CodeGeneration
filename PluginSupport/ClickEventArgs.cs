namespace PluginSupport
{
    public class ClickEventArgs(Action<MouseButtons, object[]> action, object[] args) : EventArgs
    {
        public Action<MouseButtons, object[]> Action { get; set; } = action;
        public object[] Args { get; set; } = args;
    }

    public delegate void ClickEventHandler(object sender, ClickEventArgs e);
}
