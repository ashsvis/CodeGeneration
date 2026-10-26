namespace PluginSupport
{
    partial class EditPropertyForm
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
            propGrid = new PropertyGrid();
            SuspendLayout();
            // 
            // propGrid
            // 
            propGrid.BackColor = SystemColors.Control;
            propGrid.Dock = DockStyle.Fill;
            propGrid.Location = new Point(0, 0);
            propGrid.Name = "propGrid";
            propGrid.Size = new Size(370, 450);
            propGrid.TabIndex = 0;
            propGrid.PropertyValueChanged += PropGrid_PropertyValueChanged;
            // 
            // EditPropertyForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(370, 450);
            Controls.Add(propGrid);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EditPropertyForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Свойства";
            Leave += EditPropertyForm_Leave;
            ResumeLayout(false);
        }

        #endregion

        private PropertyGrid propGrid;
    }
}