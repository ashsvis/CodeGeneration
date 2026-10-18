using System.ComponentModel;
using System.Globalization;

namespace PluginSupport
{
    public partial class InputValueForm : Form
    {
        public InputValueForm()
        {
            InitializeComponent();
        }

        [DefaultValue(null)]
        public string StrValue
        {
            get { return tbValue.Text; }
            set
            {
                Text = @"Введите строку символов";
                tbValue.Text = value;
                tbValue.Multiline = false;
                tbValue.TextAlign = HorizontalAlignment.Left;
                tbValue.SelectAll();
                AcceptButton = btnOk;
            }
        }

        [DefaultValue(null)]
        public string[] LinesValue
        {
            get { return tbValue.Lines; }
            set
            {
                Text = @"Введите многострочный текст";
                tbValue.Lines = value;
                tbValue.Multiline = true;
                tbValue.TextAlign = HorizontalAlignment.Left;
                AcceptButton = null;
                Height += value.Length * 22;
                tbValue.Focus();
                tbValue.SelectionStart = 0;
                tbValue.SelectionLength = 0;
                tbValue.SelectAll();
            }
        }

        //private float _fvalue;

        //[DefaultValue(null)]
        //public float FloatValue
        //{
        //    get
        //    {
        //        Text = @"Введите натуральное число";
        //        var fp = CultureInfo.GetCultureInfo("en-US");
        //        float.TryParse(tbValue.Text.Replace(',', '.'), NumberStyles.Float, fp, out _fvalue);
        //        return _fvalue;
        //    }
        //    set
        //    {
        //        _fvalue = value;
        //        var fp = CultureInfo.GetCultureInfo("en-US");
        //        tbValue.Text = _fvalue.ToString(fp);
        //        tbValue.Multiline = false;
        //        tbValue.TextAlign = HorizontalAlignment.Right;
        //        AcceptButton = btnOk;
        //        tbValue.SelectAll();
        //    }
        //}

        private double _dvalue;

        [DefaultValue(null)]
        public double DoubleValue
        {
            get
            {
                Text = @"Введите натуральное число";
                var fp = CultureInfo.GetCultureInfo("en-US");
                double.TryParse(tbValue.Text.Replace(',', '.'), NumberStyles.Float, fp, out _dvalue);
                return _dvalue;
            }
            set
            {
                _dvalue = value;
                var fp = CultureInfo.GetCultureInfo("en-US");
                tbValue.Text = _dvalue.ToString(fp);
                tbValue.Multiline = false;
                tbValue.TextAlign = HorizontalAlignment.Right;
                AcceptButton = btnOk;
                tbValue.SelectAll();
            }
        }

        private int _ivalue;

        private void TbValue_TextChanged(object sender, EventArgs e)
        {
            if (tbValue.Multiline)
            {
                Height = 119 + tbValue.Lines.Length * 22;
            }
        }

        [DefaultValue(null)]
        public int IntValue
        {
            get
            {
                Text = @"Введите целое число";
                var fp = CultureInfo.GetCultureInfo("en-US");
                int.TryParse(tbValue.Text, NumberStyles.Float, fp, out _ivalue);
                return _ivalue;
            }
            set
            {
                _ivalue = value;
                var fp = CultureInfo.GetCultureInfo("en-US");
                tbValue.Text = _ivalue.ToString(fp);
                tbValue.Multiline = false;
                tbValue.TextAlign = HorizontalAlignment.Right;
                AcceptButton = btnOk;
                tbValue.SelectAll();
            }
        }
    }
}
