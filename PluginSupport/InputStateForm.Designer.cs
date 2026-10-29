namespace PluginSupport
{
    partial class InputStateForm
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
            btnOk = new Button();
            btnCancel = new Button();
            panelFrame = new Panel();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            panelFrame.SuspendLayout();
            SuspendLayout();
            // 
            // btnOk
            // 
            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOk.DialogResult = DialogResult.OK;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Font = new Font("Segoe UI", 9.75F);
            btnOk.Location = new Point(189, 48);
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
            btnCancel.Location = new Point(270, 48);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 26);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // panelFrame
            // 
            panelFrame.Controls.Add(radioButton2);
            panelFrame.Controls.Add(radioButton1);
            panelFrame.Controls.Add(btnCancel);
            panelFrame.Controls.Add(btnOk);
            panelFrame.Dock = DockStyle.Fill;
            panelFrame.Location = new Point(0, 0);
            panelFrame.Name = "panelFrame";
            panelFrame.Size = new Size(357, 86);
            panelFrame.TabIndex = 3;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(22, 13);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(125, 18);
            radioButton1.TabIndex = 3;
            radioButton1.TabStop = true;
            radioButton1.Text = "Выключено (OFF)";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(22, 37);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(113, 18);
            radioButton2.TabIndex = 3;
            radioButton2.TabStop = true;
            radioButton2.Text = "Включено (ON)";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // InputStateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 14F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(357, 86);
            Controls.Add(panelFrame);
            Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(373, 24);
            Name = "InputStateForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Укажите состояние";
            panelFrame.ResumeLayout(false);
            panelFrame.PerformLayout();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private Panel panelFrame;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
    }
}