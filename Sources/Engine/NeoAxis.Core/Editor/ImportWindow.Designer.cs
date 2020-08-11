namespace NeoAxis.Editor
{
    partial class ImportWindow
	{
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.buttonImport = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.buttonClose = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonSplitContainer1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.contentBrowser1 = new NeoAxis.Editor.ContentBrowser();
			this.panelName = new System.Windows.Forms.Panel();
			this.labelName = new System.Windows.Forms.Label();
			this.textBoxDestinationFolder = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
			this.buttonDestinationFolderBrowse = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.labelError = new System.Windows.Forms.Label();
			this.labelCreationPath = new System.Windows.Forms.Label();
			this.eUserControl1 = new NeoAxis.EUserControl();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
			this.kryptonSplitContainer1.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
			this.kryptonSplitContainer1.Panel2.SuspendLayout();
			this.kryptonSplitContainer1.SuspendLayout();
			this.panelName.SuspendLayout();
			this.eUserControl1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonImport
			// 
			this.buttonImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonImport.Location = new System.Drawing.Point(375, 655);
			this.buttonImport.Name = "buttonImport";
			this.buttonImport.Size = new System.Drawing.Size(147, 32);
			this.buttonImport.TabIndex = 0;
			this.buttonImport.Values.Text = "Import";
			this.buttonImport.Click += new System.EventHandler(this.ButtonImport_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Location = new System.Drawing.Point(528, 655);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(147, 32);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Values.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
			// 
			// kryptonSplitContainer1
			// 
			this.kryptonSplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonSplitContainer1.Location = new System.Drawing.Point(12, 12);
			this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
			// 
			// kryptonSplitContainer1.Panel1
			// 
			this.kryptonSplitContainer1.Panel1.Controls.Add(this.contentBrowser1);
			// 
			// kryptonSplitContainer1.Panel2
			// 
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.panelName);
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.labelError);
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.labelCreationPath);
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.eUserControl1);
			this.kryptonSplitContainer1.Size = new System.Drawing.Size(1030, 690);
			this.kryptonSplitContainer1.SplitterDistance = 350;
			this.kryptonSplitContainer1.SplitterPercent = 0.33980582524271846D;
			this.kryptonSplitContainer1.TabIndex = 3;
			// 
			// contentBrowser1
			// 
			this.contentBrowser1.CanSelectObjectSettings = false;
			this.contentBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentBrowser1.FilteringMode = null;
			this.contentBrowser1.Location = new System.Drawing.Point(0, 0);
			this.contentBrowser1.Margin = new System.Windows.Forms.Padding(4);
			this.contentBrowser1.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowser1.MultiSelect = true;
			this.contentBrowser1.Name = "contentBrowser1";
			this.contentBrowser1.ReadOnlyHierarchy = false;
			this.contentBrowser1.Size = new System.Drawing.Size(350, 690);
			this.contentBrowser1.TabIndex = 2;
			this.contentBrowser1.ThisIsSettingsWindow = false;
			this.contentBrowser1.ItemAfterChoose += new NeoAxis.Editor.ContentBrowser.ItemAfterChooseDelegate(this.contentBrowser1_ItemAfterChoose);
			// 
			// panelName
			// 
			//this.panelName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
   //         | System.Windows.Forms.AnchorStyles.Right)));
			this.panelName.Controls.Add(this.labelName);
			this.panelName.Controls.Add(this.textBoxDestinationFolder);
			this.panelName.Controls.Add(this.buttonDestinationFolderBrowse);
			this.panelName.Location = new System.Drawing.Point(11, 22);
			this.panelName.Name = "panelName";
			this.panelName.Size = new System.Drawing.Size(664, 26);
			this.panelName.TabIndex = 10;
			// 
			// labelName
			// 
			this.labelName.AutoSize = true;
			this.labelName.Location = new System.Drawing.Point(4, 5);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(54, 17);
			this.labelName.TabIndex = 6;
			this.labelName.Text = "Assets\\";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textBoxDestinationFolder
			// 
			//this.textBoxDestinationFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
   //         | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxDestinationFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.textBoxDestinationFolder.Location = new System.Drawing.Point(63, 2);
			this.textBoxDestinationFolder.Name = "textBoxDestinationFolder";
			this.textBoxDestinationFolder.Size = new System.Drawing.Size(566, 21);
			this.textBoxDestinationFolder.TabIndex = 3;
			// 
			// buttonDestinationFolderBrowse
			// 
			//this.buttonDestinationFolderBrowse.Dock = System.Windows.Forms.DockStyle.Right;
			this.buttonDestinationFolderBrowse.Location = new System.Drawing.Point(634, 0);
			this.buttonDestinationFolderBrowse.Name = "buttonDestinationFolderBrowse";
			this.buttonDestinationFolderBrowse.Size = new System.Drawing.Size(30, 26);
			this.buttonDestinationFolderBrowse.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonDestinationFolderBrowse.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.buttonDestinationFolderBrowse.StateNormal.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonDestinationFolderBrowse.TabIndex = 5;
			this.buttonDestinationFolderBrowse.Values.Image = global::NeoAxis.Properties.Resources.SelectFolder_16;
			this.buttonDestinationFolderBrowse.Values.Text = "";
			this.buttonDestinationFolderBrowse.Click += new System.EventHandler(this.buttonDestinationFolderBrowse_Click);
			// 
			// labelError
			// 
			this.labelError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelError.ForeColor = System.Drawing.Color.Red;
			this.labelError.Location = new System.Drawing.Point(222, 635);
			this.labelError.Name = "labelError";
			this.labelError.Size = new System.Drawing.Size(453, 17);
			this.labelError.TabIndex = 3;
			this.labelError.Text = "Error";
			this.labelError.Visible = false;
			// 
			// labelCreationPath
			// 
			this.labelCreationPath.AutoSize = true;
			this.labelCreationPath.Location = new System.Drawing.Point(26, 112);
			this.labelCreationPath.Name = "labelCreationPath";
			this.labelCreationPath.Size = new System.Drawing.Size(199, 17);
			this.labelCreationPath.TabIndex = 7;
			this.labelCreationPath.Text = "Creation path: CC\\FF\\BB.mesh";
			this.labelCreationPath.Visible = false;
			// 
			// eUserControl1
			// 
			this.eUserControl1.Controls.Add(this.tableLayoutPanel1);
			this.eUserControl1.Controls.Add(this.label1);
			this.eUserControl1.Controls.Add(this.buttonImport);
			this.eUserControl1.Controls.Add(this.buttonClose);
			this.eUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.eUserControl1.Location = new System.Drawing.Point(0, 0);
			this.eUserControl1.Margin = new System.Windows.Forms.Padding(4);
			this.eUserControl1.Name = "eUserControl1";
			this.eUserControl1.Size = new System.Drawing.Size(675, 690);
			this.eUserControl1.TabIndex = 2;
			this.eUserControl1.Load += new System.EventHandler(this.eUserControl1_Load);
			// 
			// tableLayoutPanel1
			// 
			//this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
   //         | System.Windows.Forms.AnchorStyles.Left) 
   //         | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 132);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(662, 500);
			this.tableLayoutPanel1.TabIndex = 9;
			this.tableLayoutPanel1.Visible = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(123, 17);
			this.label1.TabIndex = 4;
			this.label1.Text = "Destination folder:";
			// 
			// timer1
			// 
			this.timer1.Interval = 50;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// ImportWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CloseByEscape = true;
			this.Controls.Add(this.kryptonSplitContainer1);
			this.Name = "ImportWindow";
			this.Size = new System.Drawing.Size(1054, 714);
			this.WindowTitle = "Import";
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
			this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
			this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
			this.kryptonSplitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
			this.kryptonSplitContainer1.ResumeLayout(false);
			this.panelName.ResumeLayout(false);
			this.panelName.PerformLayout();
			this.eUserControl1.ResumeLayout(false);
			this.eUserControl1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton buttonImport;
        private ComponentFactory.Krypton.Toolkit.KryptonButton buttonClose;
		private ContentBrowser contentBrowser1;
		private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
		private System.Windows.Forms.Label label1;
		private ComponentFactory.Krypton.Toolkit.KryptonButton buttonDestinationFolderBrowse;
		private System.Windows.Forms.Label labelCreationPath;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Timer timer1;
		private EUserControl eUserControl1;
		private System.Windows.Forms.Label labelError;
		private System.Windows.Forms.Panel panelName;
		private System.Windows.Forms.Label labelName;
		private ComponentFactory.Krypton.Toolkit.KryptonTextBox textBoxDestinationFolder;
	}
}