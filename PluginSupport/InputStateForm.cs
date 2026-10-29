using System.ComponentModel;

namespace PluginSupport
{
    public partial class InputStateForm : Form
    {
        public InputStateForm()
        {
            InitializeComponent();
        }

        private bool _bvalue;

        [DefaultValue(null)]
        public bool BooleanValue
        {
            get
            {
                if (radioButton1.Checked) return false;
                if (radioButton2.Checked) return true;
                return false;
            }
            set
            {
                _bvalue = value;
                AcceptButton = btnOk;
                if (_bvalue)
                    radioButton2.Checked = true;
                else
                    radioButton1.Checked = true;
            }
        }
    }
}
