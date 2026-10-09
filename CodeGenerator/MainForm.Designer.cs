namespace CodeGenerator
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip1 = new MenuStrip();
            tsmiFile = new ToolStripMenuItem();
            tsmiCreate = new ToolStripMenuItem();
            tsmiOpenFile = new ToolStripMenuItem();
            toolStripSeparator = new ToolStripSeparator();
            tsmiSave = new ToolStripMenuItem();
            tsmiSaveAs = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            tsmiPrint = new ToolStripMenuItem();
            tsmiPreview = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            tsmiExit = new ToolStripMenuItem();
            tsmiChange = new ToolStripMenuItem();
            tsmiUndo = new ToolStripMenuItem();
            tsmiRedo = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            tsmiCut = new ToolStripMenuItem();
            tsmiCopy = new ToolStripMenuItem();
            tsmiPaste = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            tsmiSelectAll = new ToolStripMenuItem();
            tsmiTools = new ToolStripMenuItem();
            tsmiTuning = new ToolStripMenuItem();
            tsmiParameters = new ToolStripMenuItem();
            tsmiHelp = new ToolStripMenuItem();
            tsmiContent = new ToolStripMenuItem();
            tsmiIndex = new ToolStripMenuItem();
            tsmiSearch = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            tsmiAbout = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            tsbCreate = new ToolStripButton();
            tsbOpen = new ToolStripButton();
            tsbSave = new ToolStripButton();
            tsbPrint = new ToolStripButton();
            toolStripSeparator6 = new ToolStripSeparator();
            tsbCut = new ToolStripButton();
            tsbCopy = new ToolStripButton();
            tsbPaste = new ToolStripButton();
            toolStripSeparator7 = new ToolStripSeparator();
            справкаToolStripButton = new ToolStripButton();
            statusStrip1 = new StatusStrip();
            tsslStatus = new ToolStripStatusLabel();
            panLeft = new Panel();
            tcUtilites = new TabControl();
            tpLibrary = new TabPage();
            tvLibrary = new TreeView();
            tpProperties = new TabPage();
            pgProperties = new PropertyGrid();
            panRight = new Panel();
            splitterLeft = new Splitter();
            splitterRight = new Splitter();
            panCenter = new Panel();
            contextMenu = new ContextMenuStrip(components);
            testToolStripMenuItem = new ToolStripMenuItem();
            timerCalculate = new System.Windows.Forms.Timer(components);
            timerInterface = new System.Windows.Forms.Timer(components);
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            panLeft.SuspendLayout();
            tcUtilites.SuspendLayout();
            tpLibrary.SuspendLayout();
            tpProperties.SuspendLayout();
            contextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { tsmiFile, tsmiChange, tsmiTools, tsmiHelp });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // tsmiFile
            // 
            tsmiFile.DropDownItems.AddRange(new ToolStripItem[] { tsmiCreate, tsmiOpenFile, toolStripSeparator, tsmiSave, tsmiSaveAs, toolStripSeparator1, tsmiPrint, tsmiPreview, toolStripSeparator2, tsmiExit });
            tsmiFile.Name = "tsmiFile";
            tsmiFile.Size = new Size(48, 20);
            tsmiFile.Text = "&Файл";
            // 
            // tsmiCreate
            // 
            tsmiCreate.Image = (Image)resources.GetObject("tsmiCreate.Image");
            tsmiCreate.ImageTransparentColor = Color.Magenta;
            tsmiCreate.Name = "tsmiCreate";
            tsmiCreate.ShortcutKeys = Keys.Control | Keys.N;
            tsmiCreate.Size = new Size(233, 22);
            tsmiCreate.Text = "&Создать";
            tsmiCreate.Click += TsmiCreate_Click;
            // 
            // tsmiOpenFile
            // 
            tsmiOpenFile.Image = (Image)resources.GetObject("tsmiOpenFile.Image");
            tsmiOpenFile.ImageTransparentColor = Color.Magenta;
            tsmiOpenFile.Name = "tsmiOpenFile";
            tsmiOpenFile.ShortcutKeys = Keys.Control | Keys.O;
            tsmiOpenFile.Size = new Size(233, 22);
            tsmiOpenFile.Text = "&Открыть";
            tsmiOpenFile.Click += TsmiOpenFile_Click;
            // 
            // toolStripSeparator
            // 
            toolStripSeparator.Name = "toolStripSeparator";
            toolStripSeparator.Size = new Size(230, 6);
            // 
            // tsmiSave
            // 
            tsmiSave.Image = (Image)resources.GetObject("tsmiSave.Image");
            tsmiSave.ImageTransparentColor = Color.Magenta;
            tsmiSave.Name = "tsmiSave";
            tsmiSave.ShortcutKeys = Keys.Control | Keys.S;
            tsmiSave.Size = new Size(233, 22);
            tsmiSave.Text = "&Сохранить";
            tsmiSave.Click += TsmiSave_Click;
            // 
            // tsmiSaveAs
            // 
            tsmiSaveAs.Name = "tsmiSaveAs";
            tsmiSaveAs.Size = new Size(233, 22);
            tsmiSaveAs.Text = "Сохранить &как";
            tsmiSaveAs.Click += TsmiSaveAs_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(230, 6);
            // 
            // tsmiPrint
            // 
            tsmiPrint.Image = (Image)resources.GetObject("tsmiPrint.Image");
            tsmiPrint.ImageTransparentColor = Color.Magenta;
            tsmiPrint.Name = "tsmiPrint";
            tsmiPrint.ShortcutKeys = Keys.Control | Keys.P;
            tsmiPrint.Size = new Size(233, 22);
            tsmiPrint.Text = "&Печать";
            // 
            // tsmiPreview
            // 
            tsmiPreview.Image = (Image)resources.GetObject("tsmiPreview.Image");
            tsmiPreview.ImageTransparentColor = Color.Magenta;
            tsmiPreview.Name = "tsmiPreview";
            tsmiPreview.Size = new Size(233, 22);
            tsmiPreview.Text = "Предварительный про&смотр";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(230, 6);
            // 
            // tsmiExit
            // 
            tsmiExit.Name = "tsmiExit";
            tsmiExit.Size = new Size(233, 22);
            tsmiExit.Text = "Вы&ход";
            tsmiExit.Click += TsmiExit_Click;
            // 
            // tsmiChange
            // 
            tsmiChange.DropDownItems.AddRange(new ToolStripItem[] { tsmiUndo, tsmiRedo, toolStripSeparator3, tsmiCut, tsmiCopy, tsmiPaste, toolStripSeparator4, tsmiSelectAll });
            tsmiChange.Name = "tsmiChange";
            tsmiChange.Size = new Size(73, 20);
            tsmiChange.Text = "&Изменить";
            // 
            // tsmiUndo
            // 
            tsmiUndo.Name = "tsmiUndo";
            tsmiUndo.ShortcutKeys = Keys.Control | Keys.Z;
            tsmiUndo.Size = new Size(181, 22);
            tsmiUndo.Text = "&Отменить";
            // 
            // tsmiRedo
            // 
            tsmiRedo.Name = "tsmiRedo";
            tsmiRedo.ShortcutKeys = Keys.Control | Keys.Y;
            tsmiRedo.Size = new Size(181, 22);
            tsmiRedo.Text = "&Повторить";
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(178, 6);
            // 
            // tsmiCut
            // 
            tsmiCut.Image = (Image)resources.GetObject("tsmiCut.Image");
            tsmiCut.ImageTransparentColor = Color.Magenta;
            tsmiCut.Name = "tsmiCut";
            tsmiCut.ShortcutKeys = Keys.Control | Keys.X;
            tsmiCut.Size = new Size(181, 22);
            tsmiCut.Text = "В&ырезать";
            // 
            // tsmiCopy
            // 
            tsmiCopy.Image = (Image)resources.GetObject("tsmiCopy.Image");
            tsmiCopy.ImageTransparentColor = Color.Magenta;
            tsmiCopy.Name = "tsmiCopy";
            tsmiCopy.ShortcutKeys = Keys.Control | Keys.C;
            tsmiCopy.Size = new Size(181, 22);
            tsmiCopy.Text = "&Копировать";
            tsmiCopy.Click += TsmiCopy_Click;
            // 
            // tsmiPaste
            // 
            tsmiPaste.Image = (Image)resources.GetObject("tsmiPaste.Image");
            tsmiPaste.ImageTransparentColor = Color.Magenta;
            tsmiPaste.Name = "tsmiPaste";
            tsmiPaste.ShortcutKeys = Keys.Control | Keys.V;
            tsmiPaste.Size = new Size(181, 22);
            tsmiPaste.Text = "&Вставить";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(178, 6);
            // 
            // tsmiSelectAll
            // 
            tsmiSelectAll.Name = "tsmiSelectAll";
            tsmiSelectAll.Size = new Size(181, 22);
            tsmiSelectAll.Text = "Выбрать &все";
            // 
            // tsmiTools
            // 
            tsmiTools.DropDownItems.AddRange(new ToolStripItem[] { tsmiTuning, tsmiParameters });
            tsmiTools.Name = "tsmiTools";
            tsmiTools.Size = new Size(95, 20);
            tsmiTools.Text = "&Инструменты";
            // 
            // tsmiTuning
            // 
            tsmiTuning.Name = "tsmiTuning";
            tsmiTuning.Size = new Size(180, 22);
            tsmiTuning.Text = "&Настройки";
            // 
            // tsmiParameters
            // 
            tsmiParameters.Name = "tsmiParameters";
            tsmiParameters.Size = new Size(180, 22);
            tsmiParameters.Text = "&Параметры";
            // 
            // tsmiHelp
            // 
            tsmiHelp.DropDownItems.AddRange(new ToolStripItem[] { tsmiContent, tsmiIndex, tsmiSearch, toolStripSeparator5, tsmiAbout });
            tsmiHelp.Name = "tsmiHelp";
            tsmiHelp.Size = new Size(65, 20);
            tsmiHelp.Text = "&Справка";
            // 
            // tsmiContent
            // 
            tsmiContent.Name = "tsmiContent";
            tsmiContent.Size = new Size(180, 22);
            tsmiContent.Text = "&Содержимое";
            // 
            // tsmiIndex
            // 
            tsmiIndex.Name = "tsmiIndex";
            tsmiIndex.Size = new Size(180, 22);
            tsmiIndex.Text = "&Индекс";
            // 
            // tsmiSearch
            // 
            tsmiSearch.Name = "tsmiSearch";
            tsmiSearch.Size = new Size(180, 22);
            tsmiSearch.Text = "&Поиск";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(177, 6);
            // 
            // tsmiAbout
            // 
            tsmiAbout.Name = "tsmiAbout";
            tsmiAbout.Size = new Size(180, 22);
            tsmiAbout.Text = "&О программе…";
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbCreate, tsbOpen, tsbSave, tsbPrint, toolStripSeparator6, tsbCut, tsbCopy, tsbPaste, toolStripSeparator7, справкаToolStripButton });
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // tsbCreate
            // 
            tsbCreate.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbCreate.Image = (Image)resources.GetObject("tsbCreate.Image");
            tsbCreate.ImageTransparentColor = Color.Magenta;
            tsbCreate.Name = "tsbCreate";
            tsbCreate.Size = new Size(23, 22);
            tsbCreate.Text = "&Создать";
            tsbCreate.Click += TsmiCreate_Click;
            // 
            // tsbOpen
            // 
            tsbOpen.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbOpen.Image = (Image)resources.GetObject("tsbOpen.Image");
            tsbOpen.ImageTransparentColor = Color.Magenta;
            tsbOpen.Name = "tsbOpen";
            tsbOpen.Size = new Size(23, 22);
            tsbOpen.Text = "&Открыть";
            tsbOpen.Click += TsmiOpenFile_Click;
            // 
            // tsbSave
            // 
            tsbSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbSave.Image = (Image)resources.GetObject("tsbSave.Image");
            tsbSave.ImageTransparentColor = Color.Magenta;
            tsbSave.Name = "tsbSave";
            tsbSave.Size = new Size(23, 22);
            tsbSave.Text = "&Сохранить";
            tsbSave.Click += TsmiSave_Click;
            // 
            // tsbPrint
            // 
            tsbPrint.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbPrint.Image = (Image)resources.GetObject("tsbPrint.Image");
            tsbPrint.ImageTransparentColor = Color.Magenta;
            tsbPrint.Name = "tsbPrint";
            tsbPrint.Size = new Size(23, 22);
            tsbPrint.Text = "&Печать";
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 25);
            // 
            // tsbCut
            // 
            tsbCut.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbCut.Image = (Image)resources.GetObject("tsbCut.Image");
            tsbCut.ImageTransparentColor = Color.Magenta;
            tsbCut.Name = "tsbCut";
            tsbCut.Size = new Size(23, 22);
            tsbCut.Text = "Вы&резать";
            // 
            // tsbCopy
            // 
            tsbCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbCopy.Image = (Image)resources.GetObject("tsbCopy.Image");
            tsbCopy.ImageTransparentColor = Color.Magenta;
            tsbCopy.Name = "tsbCopy";
            tsbCopy.Size = new Size(23, 22);
            tsbCopy.Text = "&Копировать";
            tsbCopy.Click += TsmiCopy_Click;
            // 
            // tsbPaste
            // 
            tsbPaste.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbPaste.Image = (Image)resources.GetObject("tsbPaste.Image");
            tsbPaste.ImageTransparentColor = Color.Magenta;
            tsbPaste.Name = "tsbPaste";
            tsbPaste.Size = new Size(23, 22);
            tsbPaste.Text = "&Вставить";
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(6, 25);
            // 
            // справкаToolStripButton
            // 
            справкаToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            справкаToolStripButton.Image = (Image)resources.GetObject("справкаToolStripButton.Image");
            справкаToolStripButton.ImageTransparentColor = Color.Magenta;
            справкаToolStripButton.Name = "справкаToolStripButton";
            справкаToolStripButton.Size = new Size(23, 22);
            справкаToolStripButton.Text = "С&правка";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsslStatus });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // tsslStatus
            // 
            tsslStatus.Name = "tsslStatus";
            tsslStatus.Size = new Size(48, 17);
            tsslStatus.Text = "Готово.";
            // 
            // panLeft
            // 
            panLeft.Controls.Add(tcUtilites);
            panLeft.Dock = DockStyle.Left;
            panLeft.Location = new Point(0, 49);
            panLeft.Name = "panLeft";
            panLeft.Size = new Size(300, 379);
            panLeft.TabIndex = 3;
            // 
            // tcUtilites
            // 
            tcUtilites.Controls.Add(tpLibrary);
            tcUtilites.Controls.Add(tpProperties);
            tcUtilites.Dock = DockStyle.Fill;
            tcUtilites.Location = new Point(0, 0);
            tcUtilites.Margin = new Padding(0);
            tcUtilites.Name = "tcUtilites";
            tcUtilites.Padding = new Point(0, 0);
            tcUtilites.SelectedIndex = 0;
            tcUtilites.Size = new Size(300, 379);
            tcUtilites.SizeMode = TabSizeMode.FillToRight;
            tcUtilites.TabIndex = 0;
            // 
            // tpLibrary
            // 
            tpLibrary.Controls.Add(tvLibrary);
            tpLibrary.Location = new Point(4, 24);
            tpLibrary.Margin = new Padding(0);
            tpLibrary.Name = "tpLibrary";
            tpLibrary.Padding = new Padding(3);
            tpLibrary.Size = new Size(292, 351);
            tpLibrary.TabIndex = 0;
            tpLibrary.Text = "Библиотека";
            // 
            // tvLibrary
            // 
            tvLibrary.BorderStyle = BorderStyle.None;
            tvLibrary.Dock = DockStyle.Fill;
            tvLibrary.FullRowSelect = true;
            tvLibrary.HideSelection = false;
            tvLibrary.Location = new Point(3, 3);
            tvLibrary.Margin = new Padding(0);
            tvLibrary.Name = "tvLibrary";
            tvLibrary.Size = new Size(286, 345);
            tvLibrary.TabIndex = 0;
            tvLibrary.MouseDown += TvLibrary_MouseDown;
            // 
            // tpProperties
            // 
            tpProperties.Controls.Add(pgProperties);
            tpProperties.Location = new Point(4, 24);
            tpProperties.Margin = new Padding(0);
            tpProperties.Name = "tpProperties";
            tpProperties.Padding = new Padding(3);
            tpProperties.Size = new Size(292, 351);
            tpProperties.TabIndex = 1;
            tpProperties.Text = "Свойства";
            // 
            // pgProperties
            // 
            pgProperties.BackColor = SystemColors.Control;
            pgProperties.Dock = DockStyle.Fill;
            pgProperties.Location = new Point(3, 3);
            pgProperties.Margin = new Padding(0);
            pgProperties.Name = "pgProperties";
            pgProperties.Size = new Size(286, 345);
            pgProperties.TabIndex = 0;
            // 
            // panRight
            // 
            panRight.Dock = DockStyle.Right;
            panRight.Location = new Point(800, 49);
            panRight.Name = "panRight";
            panRight.Size = new Size(0, 379);
            panRight.TabIndex = 4;
            // 
            // splitterLeft
            // 
            splitterLeft.Location = new Point(300, 49);
            splitterLeft.MinSize = 0;
            splitterLeft.Name = "splitterLeft";
            splitterLeft.Size = new Size(3, 379);
            splitterLeft.TabIndex = 5;
            splitterLeft.TabStop = false;
            // 
            // splitterRight
            // 
            splitterRight.Dock = DockStyle.Right;
            splitterRight.Location = new Point(797, 49);
            splitterRight.MinSize = 0;
            splitterRight.Name = "splitterRight";
            splitterRight.Size = new Size(3, 379);
            splitterRight.TabIndex = 6;
            splitterRight.TabStop = false;
            // 
            // panCenter
            // 
            panCenter.AllowDrop = true;
            panCenter.Dock = DockStyle.Fill;
            panCenter.Location = new Point(303, 49);
            panCenter.Name = "panCenter";
            panCenter.Size = new Size(494, 379);
            panCenter.TabIndex = 7;
            // 
            // contextMenu
            // 
            contextMenu.Items.AddRange(new ToolStripItem[] { testToolStripMenuItem });
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new Size(94, 26);
            // 
            // testToolStripMenuItem
            // 
            testToolStripMenuItem.Name = "testToolStripMenuItem";
            testToolStripMenuItem.Size = new Size(93, 22);
            testToolStripMenuItem.Text = "test";
            // 
            // timerCalculate
            // 
            timerCalculate.Enabled = true;
            timerCalculate.Tick += TimerCalculate_Tick;
            // 
            // timerInterface
            // 
            timerInterface.Enabled = true;
            timerInterface.Interval = 50;
            timerInterface.Tick += TimerInterface_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panCenter);
            Controls.Add(splitterRight);
            Controls.Add(splitterLeft);
            Controls.Add(panRight);
            Controls.Add(panLeft);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            KeyPreview = true;
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "Генераторы контента";
            WindowState = FormWindowState.Maximized;
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            KeyDown += MainForm_KeyDown;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panLeft.ResumeLayout(false);
            tcUtilites.ResumeLayout(false);
            tpLibrary.ResumeLayout(false);
            tpProperties.ResumeLayout(false);
            contextMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem tsmiFile;
        private ToolStripMenuItem tsmiCreate;
        private ToolStripMenuItem tsmiOpenFile;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripMenuItem tsmiSave;
        private ToolStripMenuItem tsmiSaveAs;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem tsmiPrint;
        private ToolStripMenuItem tsmiPreview;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem tsmiExit;
        private ToolStripMenuItem tsmiChange;
        private ToolStripMenuItem tsmiUndo;
        private ToolStripMenuItem tsmiRedo;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem tsmiCut;
        private ToolStripMenuItem tsmiCopy;
        private ToolStripMenuItem tsmiPaste;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem tsmiSelectAll;
        private ToolStripMenuItem tsmiTools;
        private ToolStripMenuItem tsmiTuning;
        private ToolStripMenuItem tsmiParameters;
        private ToolStripMenuItem tsmiHelp;
        private ToolStripMenuItem tsmiContent;
        private ToolStripMenuItem tsmiIndex;
        private ToolStripMenuItem tsmiSearch;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem tsmiAbout;
        private ToolStrip toolStrip1;
        private ToolStripButton tsbCreate;
        private ToolStripButton tsbOpen;
        private ToolStripButton tsbSave;
        private ToolStripButton tsbPrint;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripButton tsbCut;
        private ToolStripButton tsbCopy;
        private ToolStripButton tsbPaste;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripButton справкаToolStripButton;
        private StatusStrip statusStrip1;
        private Panel panLeft;
        private Panel panRight;
        private Splitter splitterLeft;
        private Splitter splitterRight;
        private TabControl tcUtilites;
        private TabPage tpLibrary;
        private TabPage tpProperties;
        private TreeView tvLibrary;
        private PropertyGrid pgProperties;
        private Panel panCenter;
        private ToolStripStatusLabel tsslStatus;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.Timer timerCalculate;
        private System.Windows.Forms.Timer timerInterface;
    }
}
