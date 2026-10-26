namespace PluginSupport
{
    public partial class EditPropertyForm : Form
    {
        public EditPropertyForm(object? sender)
        {
            InitializeComponent();
            propGrid.SelectedObject = sender;
        }

        public event EventHandler? PropertyChanged;

        private void EditPropertyForm_Leave(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PropGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            PropertyChanged?.Invoke(propGrid.SelectedObject, e);
        }
    }
}
