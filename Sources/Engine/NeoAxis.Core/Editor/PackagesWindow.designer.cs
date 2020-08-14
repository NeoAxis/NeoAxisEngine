namespace NeoAxis.Editor
{
    partial class PackagesWindow
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.buttonUpdateList = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.labelRestart = new System.Windows.Forms.Label();
			this.kryptonButtonDelete = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonSplitContainer1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.kryptonSplitContainer2 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.contentBrowser1 = new NeoAxis.Editor.ContentBrowser();
			this.labelExPackageSize = new NeoAxis.Editor.EngineLabel();
			this.progressBarPackageProgress = new NeoAxis.Editor.EngineProgressBar();
			this.labelExPackageStatus = new NeoAxis.Editor.EngineLabel();
			this.labelExPackageVersion = new NeoAxis.Editor.EngineLabel();
			this.labelExPackageDeveloper = new NeoAxis.Editor.EngineLabel();
			this.labelExPackageName = new NeoAxis.Editor.EngineLabel();
			this.labelExPackageInfo = new NeoAxis.Editor.EngineLabel();
			this.kryptonButtonUninstall = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonInstall = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonDownload = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonBuy = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
			this.kryptonSplitContainer1.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
			this.kryptonSplitContainer1.Panel2.SuspendLayout();
			this.kryptonSplitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel1)).BeginInit();
			this.kryptonSplitContainer2.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel2)).BeginInit();
			this.kryptonSplitContainer2.Panel2.SuspendLayout();
			this.kryptonSplitContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// buttonUpdateList
			// 
			this.buttonUpdateList.Location = new System.Drawing.Point(267, 0);
			this.buttonUpdateList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonUpdateList.Name = "buttonUpdateList";
			this.buttonUpdateList.Size = new System.Drawing.Size(31, 26);
			this.buttonUpdateList.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonUpdateList.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.buttonUpdateList.StateDisabled.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonUpdateList.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
			this.buttonUpdateList.StateNormal.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonUpdateList.TabIndex = 1;
			this.toolTip1.SetToolTip(this.buttonUpdateList, "Refrest the list of packages.");
			this.buttonUpdateList.Values.Image = global::NeoAxis.Properties.Resources.Refresh_16;
			this.buttonUpdateList.Values.Text = "";
			this.buttonUpdateList.Click += new System.EventHandler(this.buttonUpdateList_Click);
			// 
			// labelRestart
			// 
			this.labelRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelRestart.AutoSize = true;
			this.labelRestart.ForeColor = System.Drawing.Color.Red;
			this.labelRestart.Location = new System.Drawing.Point(25, 748);
			this.labelRestart.Name = "labelRestart";
			this.labelRestart.Size = new System.Drawing.Size(198, 17);
			this.labelRestart.TabIndex = 10;
			this.labelRestart.Text = "Restart the editor to apply changes.";
			this.labelRestart.Visible = false;
			// 
			// kryptonButtonDelete
			// 
			this.kryptonButtonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonDelete.Location = new System.Drawing.Point(953, 186);
			this.kryptonButtonDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.kryptonButtonDelete.Name = "kryptonButtonDelete";
			this.kryptonButtonDelete.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonDelete.TabIndex = 4;
			this.kryptonButtonDelete.Values.Text = "Delete";
			this.kryptonButtonDelete.Visible = false;
			this.kryptonButtonDelete.Click += new System.EventHandler(this.kryptonButtonDelete_Click);
			// 
			// kryptonSplitContainer1
			// 
			this.kryptonSplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
			this.kryptonSplitContainer1.Location = new System.Drawing.Point(25, 7);
			this.kryptonSplitContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
			// 
			// kryptonSplitContainer1.Panel1
			// 
			this.kryptonSplitContainer1.Panel1.Controls.Add(this.kryptonSplitContainer2);
			// 
			// kryptonSplitContainer1.Panel2
			// 
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.labelExPackageSize);
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.progressBarPackageProgress);
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.labelExPackageStatus);
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.labelExPackageVersion);
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.labelExPackageDeveloper);
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.labelExPackageName);
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.labelExPackageInfo);
			this.kryptonSplitContainer1.Size = new System.Drawing.Size(905, 732);
			this.kryptonSplitContainer1.SplitterDistance = 297;
			this.kryptonSplitContainer1.SplitterPercent = 0.32817679558011048D;
			this.kryptonSplitContainer1.TabIndex = 9;
			// 
			// kryptonSplitContainer2
			// 
			this.kryptonSplitContainer2.Cursor = System.Windows.Forms.Cursors.Default;
			this.kryptonSplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonSplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.kryptonSplitContainer2.IsSplitterFixed = true;
			this.kryptonSplitContainer2.Location = new System.Drawing.Point(0, 0);
			this.kryptonSplitContainer2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.kryptonSplitContainer2.Name = "kryptonSplitContainer2";
			this.kryptonSplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// kryptonSplitContainer2.Panel1
			// 
			this.kryptonSplitContainer2.Panel1.Controls.Add(this.kryptonLabel1);
			this.kryptonSplitContainer2.Panel1.Controls.Add(this.buttonUpdateList);
			this.kryptonSplitContainer2.Panel1MinSize = 23;
			// 
			// kryptonSplitContainer2.Panel2
			// 
			this.kryptonSplitContainer2.Panel2.Controls.Add(this.contentBrowser1);
			this.kryptonSplitContainer2.Size = new System.Drawing.Size(297, 732);
			this.kryptonSplitContainer2.SplitterDistance = 23;
			this.kryptonSplitContainer2.SplitterPercent = 0.031420765027322405D;
			this.kryptonSplitContainer2.SplitterWidth = 0;
			this.kryptonSplitContainer2.TabIndex = 6;
			// 
			// kryptonLabel1
			// 
			this.kryptonLabel1.Location = new System.Drawing.Point(0, 5);
			this.kryptonLabel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.kryptonLabel1.Name = "kryptonLabel1";
			this.kryptonLabel1.Size = new System.Drawing.Size(141, 20);
			this.kryptonLabel1.TabIndex = 5;
			this.kryptonLabel1.Values.Text = "Available packages:";
			// 
			// contentBrowser1
			// 
			this.contentBrowser1.CanSelectObjectSettings = false;
			this.contentBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentBrowser1.FilteringMode = null;
			this.contentBrowser1.ListViewItemRendererOverride = null;
			this.contentBrowser1.Location = new System.Drawing.Point(0, 0);
			this.contentBrowser1.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
			this.contentBrowser1.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Objects;
			this.contentBrowser1.Name = "contentBrowser1";
			this.contentBrowser1.ReadOnlyHierarchy = false;
			this.contentBrowser1.ShowToolBar = false;
			this.contentBrowser1.Size = new System.Drawing.Size(297, 709);
			this.contentBrowser1.TabIndex = 0;
			this.contentBrowser1.ThisIsSettingsWindow = false;
			this.contentBrowser1.ItemAfterSelect += new NeoAxis.Editor.ContentBrowser.ItemAfterSelectDelegate(this.contentBrowser1_ItemAfterSelect);
			// 
			// labelExPackageSize
			// 
			this.labelExPackageSize.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
			this.labelExPackageSize.Location = new System.Drawing.Point(7, 124);
			this.labelExPackageSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.labelExPackageSize.Name = "labelExPackageSize";
			this.labelExPackageSize.Size = new System.Drawing.Size(592, 26);
			this.labelExPackageSize.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelExPackageSize.TabIndex = 3;
			this.labelExPackageSize.Text = "Size";
			// 
			// progressBarPackageProgress
			// 
			this.progressBarPackageProgress.Location = new System.Drawing.Point(217, 151);
			this.progressBarPackageProgress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.progressBarPackageProgress.Name = "progressBarPackageProgress";
			this.progressBarPackageProgress.Size = new System.Drawing.Size(381, 32);
			this.progressBarPackageProgress.TabIndex = 5;
			// 
			// labelExPackageStatus
			// 
			this.labelExPackageStatus.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
			this.labelExPackageStatus.Location = new System.Drawing.Point(7, 151);
			this.labelExPackageStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.labelExPackageStatus.Name = "labelExPackageStatus";
			this.labelExPackageStatus.Size = new System.Drawing.Size(592, 26);
			this.labelExPackageStatus.StateCommon.Content.Color1 = System.Drawing.Color.Red;
			this.labelExPackageStatus.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelExPackageStatus.TabIndex = 4;
			this.labelExPackageStatus.Text = "Status";
			// 
			// labelExPackageVersion
			// 
			this.labelExPackageVersion.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
			this.labelExPackageVersion.Location = new System.Drawing.Point(7, 100);
			this.labelExPackageVersion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.labelExPackageVersion.Name = "labelExPackageVersion";
			this.labelExPackageVersion.Size = new System.Drawing.Size(592, 26);
			this.labelExPackageVersion.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelExPackageVersion.TabIndex = 2;
			this.labelExPackageVersion.Text = "Version";
			// 
			// labelExPackageDeveloper
			// 
			this.labelExPackageDeveloper.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
			this.labelExPackageDeveloper.Location = new System.Drawing.Point(7, 69);
			this.labelExPackageDeveloper.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.labelExPackageDeveloper.Name = "labelExPackageDeveloper";
			this.labelExPackageDeveloper.Size = new System.Drawing.Size(592, 31);
			this.labelExPackageDeveloper.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelExPackageDeveloper.TabIndex = 1;
			this.labelExPackageDeveloper.Text = "Developer";
			// 
			// labelExPackageName
			// 
			this.labelExPackageName.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
			this.labelExPackageName.Location = new System.Drawing.Point(7, 23);
			this.labelExPackageName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.labelExPackageName.Name = "labelExPackageName";
			this.labelExPackageName.Size = new System.Drawing.Size(592, 47);
			this.labelExPackageName.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelExPackageName.TabIndex = 0;
			this.labelExPackageName.Text = "Name";
			// 
			// labelExPackageInfo
			// 
			this.labelExPackageInfo.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
			this.labelExPackageInfo.Location = new System.Drawing.Point(7, 187);
			this.labelExPackageInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.labelExPackageInfo.Multiline = true;
			this.labelExPackageInfo.Name = "labelExPackageInfo";
			this.labelExPackageInfo.Size = new System.Drawing.Size(592, 543);
			this.labelExPackageInfo.StateCommon.Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
			this.labelExPackageInfo.TabIndex = 6;
			// 
			// kryptonButtonUninstall
			// 
			this.kryptonButtonUninstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonUninstall.Location = new System.Drawing.Point(953, 148);
			this.kryptonButtonUninstall.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.kryptonButtonUninstall.Name = "kryptonButtonUninstall";
			this.kryptonButtonUninstall.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonUninstall.TabIndex = 3;
			this.kryptonButtonUninstall.Values.Text = "Uninstall";
			this.kryptonButtonUninstall.Click += new System.EventHandler(this.kryptonButtonUninstall_Click);
			// 
			// kryptonButtonInstall
			// 
			this.kryptonButtonInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonInstall.Location = new System.Drawing.Point(953, 111);
			this.kryptonButtonInstall.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.kryptonButtonInstall.Name = "kryptonButtonInstall";
			this.kryptonButtonInstall.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonInstall.TabIndex = 2;
			this.kryptonButtonInstall.Values.Text = "Install";
			this.kryptonButtonInstall.Click += new System.EventHandler(this.kryptonButtonInstall_Click);
			// 
			// kryptonButtonDownload
			// 
			this.kryptonButtonDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonDownload.Location = new System.Drawing.Point(953, 71);
			this.kryptonButtonDownload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.kryptonButtonDownload.Name = "kryptonButtonDownload";
			this.kryptonButtonDownload.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonDownload.TabIndex = 1;
			this.kryptonButtonDownload.Values.Text = "Download";
			this.kryptonButtonDownload.Click += new System.EventHandler(this.kryptonButtonDownload_Click);
			// 
			// kryptonButtonBuy
			// 
			this.kryptonButtonBuy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonBuy.Location = new System.Drawing.Point(953, 33);
			this.kryptonButtonBuy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.kryptonButtonBuy.Name = "kryptonButtonBuy";
			this.kryptonButtonBuy.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonBuy.TabIndex = 0;
			this.kryptonButtonBuy.Values.Text = "Buy";
			this.kryptonButtonBuy.Click += new System.EventHandler(this.kryptonButtonBuy_Click);
			// 
			// PackagesWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.kryptonButtonBuy);
			this.Controls.Add(this.kryptonButtonDownload);
			this.Controls.Add(this.labelRestart);
			this.Controls.Add(this.kryptonButtonDelete);
			this.Controls.Add(this.kryptonSplitContainer1);
			this.Controls.Add(this.kryptonButtonUninstall);
			this.Controls.Add(this.kryptonButtonInstall);
			this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
			this.Name = "PackagesWindow";
			this.Size = new System.Drawing.Size(1095, 789);
			this.WindowTitle = "Packages";
			this.Load += new System.EventHandler(this.PackagesDocumentWindow_Load);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
			this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
			this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
			this.kryptonSplitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
			this.kryptonSplitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel1)).EndInit();
			this.kryptonSplitContainer2.Panel1.ResumeLayout(false);
			this.kryptonSplitContainer2.Panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel2)).EndInit();
			this.kryptonSplitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2)).EndInit();
			this.kryptonSplitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label labelRestart;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonDelete;
		private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
		private ContentBrowser contentBrowser1;
		private EngineLabel labelExPackageInfo;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonUninstall;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonInstall;
		private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
		private EngineLabel labelExPackageName;
		private EngineLabel labelExPackageDeveloper;
		private EngineLabel labelExPackageVersion;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonDownload;
		private EngineLabel labelExPackageStatus;
		private NeoAxis.Editor.EngineProgressBar progressBarPackageProgress;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonBuy;
		private ComponentFactory.Krypton.Toolkit.KryptonButton buttonUpdateList;
		private EngineLabel labelExPackageSize;
		private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer2;
	}
}
