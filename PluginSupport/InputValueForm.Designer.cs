namespace PluginSupport
{
    partial class InputValueForm
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
            tbValue = new TextBox();
            btnOk = new Button();
            btnCancel = new Button();
            panelFrame = new Panel();
            panelFrame.SuspendLayout();
            SuspendLayout();
            // 
            // tbValue
            // 
            tbValue.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbValue.BorderStyle = BorderStyle.FixedSingle;
            tbValue.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            tbValue.Location = new Point(12, 12);
            tbValue.MaxLength = 60;
            tbValue.Name = "tbValue";
            tbValue.Size = new Size(343, 25);
            tbValue.TabIndex = 0;
            tbValue.TextChanged += TbValue_TextChanged;
            // 
            // btnOk
            // 
            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Font = new Font("Segoe UI", 9.75F);
            btnOk.Location = new Point(199, 48);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 26);
            btnOk.TabIndex = 1;
            btnOk.Text = "Ввод";
            btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 9.75F);
            btnCancel.Location = new Point(280, 48);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 26);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // panelFrame
            // 
            panelFrame.Controls.Add(tbValue);
            panelFrame.Controls.Add(btnCancel);
            panelFrame.Controls.Add(btnOk);
            panelFrame.Dock = DockStyle.Fill;
            panelFrame.Location = new Point(0, 0);
            panelFrame.Name = "panelFrame";
            panelFrame.Size = new Size(367, 86);
            panelFrame.TabIndex = 3;
            // 
            // InputValueForm
            // 
            AutoScaleDimensions = new SizeF(7F, 14F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(367, 86);
            Controls.Add(panelFrame);
            Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(373, 24);
            Name = "InputValueForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Введите значение";
            panelFrame.ResumeLayout(false);
            panelFrame.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private Panel panelFrame;
    }
}