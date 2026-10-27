namespace LogicModel
{
    partial class FuncPresenterTuningForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lbFuncSelector = new ListBox();
            btnCancel = new Button();
            btnOk = new Button();
            SuspendLayout();
            // 
            // lbFuncSelector
            // 
            lbFuncSelector.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lbFuncSelector.BorderStyle = BorderStyle.FixedSingle;
            lbFuncSelector.FormattingEnabled = true;
            lbFuncSelector.Location = new Point(12, 12);
            lbFuncSelector.Name = "lbFuncSelector";
            lbFuncSelector.Size = new Size(265, 227);
            lbFuncSelector.TabIndex = 0;
            lbFuncSelector.SelectedIndexChanged += LbFuncSelector_SelectedIndexChanged;
            lbFuncSelector.MouseDown += LbFuncSelector_MouseDown;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 9.75F);
            btnCancel.Location = new Point(202, 245);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 26);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Enabled = false;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Font = new Font("Segoe UI", 9.75F);
            btnOk.Location = new Point(121, 245);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 26);
            btnOk.TabIndex = 3;
            btnOk.Text = "Ввод";
            btnOk.UseVisualStyleBackColor = true;
            // 
            // FuncPresenterTuningForm
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(289, 283);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(lbFuncSelector);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FuncPresenterTuningForm";
            StartPosition = FormStartPosition.Manual;
            Text = "Презентатор функции";
            ResumeLayout(false);
        }

        #endregion

        private ListBox lbFuncSelector;
        private Button btnCancel;
        private Button btnOk;
    }
}