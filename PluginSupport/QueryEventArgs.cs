namespace PluginSupport
{
    public class QueryEventArgs(object[] args) : EventArgs
    {
        public object[] Args { get; set; } = args;
        public object[] Results { get; set; } = [];
    }

    public delegate void QueryEventHandler(object sender, QueryEventArgs e);
}
