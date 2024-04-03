#if !DEPLOY
namespace NeoAxis.Editor
{
    partial class EditorForm
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
			this.components = new System.ComponentModel.Container();
			this.kryptonManager = new Internal.ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
			this.kryptonPanel = new Internal.ComponentFactory.Krypton.Toolkit.KryptonPanel();
			this.kryptonDockableWorkspace = new NeoAxis.Editor.LowProfileDockableWorkspace();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.backstageMenu1 = new NeoAxis.Editor.BackstageMenu();
			this.kryptonRibbon = new Internal.ComponentFactory.Krypton.Ribbon.KryptonRibbon();
			this.buttonSpecHelp = new Internal.ComponentFactory.Krypton.Toolkit.ButtonSpecAny();
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
			this.kryptonPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonDockableWorkspace)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonRibbon)).BeginInit();
			this.SuspendLayout();
			// 
			// kryptonPanel
			// 
			this.kryptonPanel.Controls.Add(this.kryptonDockableWorkspace);
			this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonPanel.Location = new System.Drawing.Point(0, 136);
			this.kryptonPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.kryptonPanel.Name = "kryptonPanel";
			this.kryptonPanel.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.kryptonPanel.Size = new System.Drawing.Size(1059, 783);
			this.kryptonPanel.TabIndex = 2;
			// 
			// kryptonDockableWorkspace
			// 
			this.kryptonDockableWorkspace.AutoHiddenHost = false;
			this.kryptonDockableWorkspace.CompactFlags = ((Internal.ComponentFactory.Krypton.Workspace.CompactFlags)(((Internal.ComponentFactory.Krypton.Workspace.CompactFlags.RemoveEmptyCells | Internal.ComponentFactory.Krypton.Workspace.CompactFlags.RemoveEmptySequences) 
            | Internal.ComponentFactory.Krypton.Workspace.CompactFlags.PromoteLeafs)));
			this.kryptonDockableWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonDockableWorkspace.Location = new System.Drawing.Point(5, 4);
			this.kryptonDockableWorkspace.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.kryptonDockableWorkspace.Name = "kryptonDockableWorkspace";
			// 
			// 
			// 
			this.kryptonDockableWorkspace.Root.UniqueName = "EditorFormKryptonDockableWorkspace";
			this.kryptonDockableWorkspace.Root.WorkspaceControl = this.kryptonDockableWorkspace;
			this.kryptonDockableWorkspace.ShowMaximizeButton = false;
			this.kryptonDockableWorkspace.Size = new System.Drawing.Size(1049, 775);
			this.kryptonDockableWorkspace.TabIndex = 0;
			this.kryptonDockableWorkspace.TabStop = true;
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 10;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// backstageMenu1
			// 
			this.backstageMenu1.Location = new System.Drawing.Point(141, 114);
			this.backstageMenu1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.backstageMenu1.Name = "backstageMenu1";
			this.backstageMenu1.Ribbon = this.kryptonRibbon;
			this.backstageMenu1.Size = new System.Drawing.Size(1061, 959);
			this.backstageMenu1.TabIndex = 1;
			this.backstageMenu1.Visible = false;
			// 
			// kryptonRibbon
			// 
			this.kryptonRibbon.AllowFormIntegrate = false;
			this.kryptonRibbon.ApplicationButtonDropDownMenu = this.backstageMenu1;
			this.kryptonRibbon.ButtonSpecs.AddRange(new Internal.ComponentFactory.Krypton.Toolkit.ButtonSpecAny[] {
            this.buttonSpecHelp});
			this.kryptonRibbon.HideRibbonSize = new System.Drawing.Size(375, 312);
			this.kryptonRibbon.InDesignHelperMode = true;
			this.kryptonRibbon.Name = "kryptonRibbon";
			this.kryptonRibbon.QATUserChange = false;
			//this.kryptonRibbon.RibbonAppButton.AppButtonImage = ((System.Drawing.Image)(resources.GetObject("kryptonRibbon.RibbonAppButton.AppButtonImage")));
			this.kryptonRibbon.RibbonAppButton.AppButtonMinRecentSize = new System.Drawing.Size(300, 250);
			this.kryptonRibbon.RibbonAppButton.AppButtonText = "Project";
			this.kryptonRibbon.RibbonStrings.RecentDocuments = "Create New Outlook Item";
			this.kryptonRibbon.SelectedTab = null;
			this.kryptonRibbon.Size = new System.Drawing.Size(1059, 136);
			this.kryptonRibbon.TabIndex = 0;
			this.kryptonRibbon.SelectedTabChanged += new System.EventHandler(this.kryptonRibbon_SelectedTabChanged);
			this.kryptonRibbon.AppButtonMenuOpening += new System.ComponentModel.CancelEventHandler(this.kryptonRibbon_AppButtonMenuOpening);
			// 
			// buttonSpecHelp
			// 
			this.buttonSpecHelp.Image = global::NeoAxis.Editor.Properties.Resources.Help_16;
			this.buttonSpecHelp.UniqueName = "E0D28D217A1E48CEE0D28D217A1E48CE";
			this.buttonSpecHelp.Visible = false;
			// 
			// EditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1059, 919);
			this.Controls.Add(this.backstageMenu1);
			this.Controls.Add(this.kryptonPanel);
			this.Controls.Add(this.kryptonRibbon);
			//this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "EditorForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.StateCommon.Border.DrawBorders = ((Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.StateCommon.Header.Content.LongText.TextH = Internal.ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
			this.StateCommon.Header.Content.Padding = new System.Windows.Forms.Padding(8, -1, -1, -1);
			this.StateCommon.Header.Content.ShortText.TextH = Internal.ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
			this.Text = "NeoAxis Engine";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Activated += new System.EventHandler(this.EditorForm_Activated);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditorForm_FormClosing);
			this.Load += new System.EventHandler(this.EditorForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
			this.kryptonPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonDockableWorkspace)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonRibbon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private Internal.ComponentFactory.Krypton.Toolkit.ButtonSpecAny buttonSpecHelp;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
        private System.Windows.Forms.Timer timer1;
		public Internal.ComponentFactory.Krypton.Ribbon.KryptonRibbon kryptonRibbon;
		private LowProfileDockableWorkspace kryptonDockableWorkspace;
		public BackstageMenu backstageMenu1;
	}
}


#endif