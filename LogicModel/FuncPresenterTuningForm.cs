using PluginSupport;

namespace LogicModel
{
    public partial class FuncPresenterTuningForm : Form
    {
        public FuncPresenterTuningForm(Shape? func)
        {
            InitializeComponent();
            BuildContent(func);
        }

        public Shape[] Selected => [..lbFuncSelector.SelectedItems.Cast<Shape>()];

        private void BuildContent(Shape? func)
        {
            var pl = new PluginLogic();
            lbFuncSelector.Items.Clear();
            foreach (var entry in pl.GetTypes())
            {
                var type = entry.Value;
                var obj = Activator.CreateInstance(type);
                if (obj is IFuncPresenter presenter && presenter.IsPresent)
                {
                    lbFuncSelector.Items.Add(obj);
                    if (func != null && func.GetType() == type)
                        lbFuncSelector.SelectedItem = obj;
                }
            }
        }

        private void LbFuncSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = true;
        }

        private void LbFuncSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks > 1)
                btnOk.PerformClick();
        }
    }
}
