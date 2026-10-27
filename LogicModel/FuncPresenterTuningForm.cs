namespace LogicModel
{
    public partial class FuncPresenterTuningForm : Form
    {
        public FuncPresenterTuningForm()
        {
            InitializeComponent();
            BuildContent();
        }

        private void BuildContent()
        {
            var pl = new PluginLogic();
            lbFuncSelector.Items.Clear();
            foreach (var entry in pl.GetTypes())
            {
                var type = entry.Value;
                var obj = Activator.CreateInstance(type);
                if (obj is IFuncPresenter presenter && presenter.IsPresent)
                    lbFuncSelector.Items.Add(presenter.FuncDesc ?? entry.Key);
            }
        }

        private void LbFuncSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = true;
        }
    }
}
