#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class HCItemProjectRibbonAndToolbarActionsForm
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.kryptonSplitContainer1 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.contentBrowserAll = new NeoAxis.Editor.ContentBrowser();
			this.kryptonSplitContainer2 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonEnabled = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonNewGroup = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonRename = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonMoveUp = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonMoveDown = new System.Windows.Forms.ToolStripButton();
			this.contentBrowserProject = new NeoAxis.Editor.ContentBrowser();
			this.kryptonButtonReset = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonLabel1 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.kryptonLabel2 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
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
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// kryptonSplitContainer1
			// 
			this.kryptonSplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
			this.kryptonSplitContainer1.Location = new System.Drawing.Point(2, 24);
			this.kryptonSplitContainer1.Margin = new System.Windows.Forms.Padding(2);
			this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
			// 
			// kryptonSplitContainer1.Panel1
			// 
			this.kryptonSplitContainer1.Panel1.Controls.Add(this.contentBrowserAll);
			// 
			// kryptonSplitContainer1.Panel2
			// 
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.kryptonSplitContainer2);
			this.kryptonSplitContainer1.Size = new System.Drawing.Size(452, 411);
			this.kryptonSplitContainer1.SplitterDistance = 226;
			this.kryptonSplitContainer1.SplitterPercent = 0.5D;
			this.kryptonSplitContainer1.TabIndex = 4;
			// 
			// contentBrowserAll
			// 
			this.contentBrowserAll.CanSelectObjectSettings = false;
			this.contentBrowserAll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentBrowserAll.FilteringMode = null;
			this.contentBrowserAll.ListViewModeOverride = null;
			this.contentBrowserAll.Location = new System.Drawing.Point(0, 0);
			this.contentBrowserAll.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.contentBrowserAll.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowserAll.Name = "contentBrowserAll";
			this.contentBrowserAll.ReadOnlyHierarchy = false;
			this.contentBrowserAll.ShowToolBar = false;
			this.contentBrowserAll.Size = new System.Drawing.Size(226, 411);
			this.contentBrowserAll.TabIndex = 3;
			this.contentBrowserAll.ThisIsSettingsWindow = false;
			// 
			// kryptonSplitContainer2
			// 
			this.kryptonSplitContainer2.Cursor = System.Windows.Forms.Cursors.Default;
			this.kryptonSplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonSplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.kryptonSplitContainer2.IsSplitterFixed = true;
			this.kryptonSplitContainer2.Location = new System.Drawing.Point(0, 0);
			this.kryptonSplitContainer2.Name = "kryptonSplitContainer2";
			this.kryptonSplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// kryptonSplitContainer2.Panel1
			// 
			this.kryptonSplitContainer2.Panel1.Controls.Add(this.toolStrip1);
			this.kryptonSplitContainer2.Panel1MinSize = 10;
			// 
			// kryptonSplitContainer2.Panel2
			// 
			this.kryptonSplitContainer2.Panel2.Controls.Add(this.contentBrowserProject);
			this.kryptonSplitContainer2.Size = new System.Drawing.Size(221, 411);
			this.kryptonSplitContainer2.SplitterDistance = 24;
			this.kryptonSplitContainer2.SplitterPercent = 0.058394160583941604D;
			this.kryptonSplitContainer2.SplitterWidth = 0;
			this.kryptonSplitContainer2.TabIndex = 6;
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEnabled,
            this.toolStripSeparator1,
            this.toolStripButtonNewGroup,
            this.toolStripButtonAdd,
            this.toolStripButtonDelete,
            this.toolStripButtonRename,
            this.toolStripButtonMoveUp,
            this.toolStripButtonMoveDown});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Padding = new System.Windows.Forms.Padding( 1, 1, 1, 1 );
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.Size = new System.Drawing.Size(221, 26);
			this.toolStrip1.TabIndex = 5;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonEnabled
			// 
			this.toolStripButtonEnabled.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonEnabled.Image = global::NeoAxis.Editor.Properties.Resources.Checked_16;
			this.toolStripButtonEnabled.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonEnabled.Name = "toolStripButtonEnabled";
			this.toolStripButtonEnabled.AutoSize = false;
			this.toolStripButtonEnabled.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonEnabled.Text = "Enabled";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonNewGroup
			// 
			this.toolStripButtonNewGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonNewGroup.Image = global::NeoAxis.Editor.Properties.Resources.NewFolder_16;
			this.toolStripButtonNewGroup.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonNewGroup.Name = "toolStripButtonNewGroup";
			this.toolStripButtonNewGroup.AutoSize = false;
			this.toolStripButtonNewGroup.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonNewGroup.Text = "New Group";
			// 
			// toolStripButtonAdd
			// 
			this.toolStripButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonAdd.Image = global::NeoAxis.Editor.Properties.Resources.Add_16;
			this.toolStripButtonAdd.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonAdd.Name = "toolStripButtonAdd";
			this.toolStripButtonAdd.AutoSize = false;
			this.toolStripButtonAdd.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonAdd.Text = "Add";
			// 
			// toolStripButtonDelete
			// 
			this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonDelete.Image = global::NeoAxis.Editor.Properties.Resources.Delete_16;
			this.toolStripButtonDelete.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonDelete.Name = "toolStripButtonDelete";
			this.toolStripButtonDelete.AutoSize = false;
			this.toolStripButtonDelete.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonDelete.Text = "Delete";
			// 
			// toolStripButtonRename
			// 
			this.toolStripButtonRename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonRename.Image = global::NeoAxis.Editor.Properties.Resources.Rename_16x;
			this.toolStripButtonRename.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonRename.Name = "toolStripButtonRename";
			this.toolStripButtonRename.AutoSize = false;
			this.toolStripButtonRename.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonRename.Text = "Rename";
			// 
			// toolStripButtonMoveUp
			// 
			this.toolStripButtonMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonMoveUp.Image = global::NeoAxis.Editor.Properties.Resources.MoveUp_16;
			this.toolStripButtonMoveUp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonMoveUp.Name = "toolStripButtonMoveUp";
			this.toolStripButtonMoveUp.AutoSize = false;
			this.toolStripButtonMoveUp.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonMoveUp.Text = "Move Up";
			// 
			// toolStripButtonMoveDown
			// 
			this.toolStripButtonMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonMoveDown.Image = global::NeoAxis.Editor.Properties.Resources.MoveDown_16;
			this.toolStripButtonMoveDown.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonMoveDown.Name = "toolStripButtonMoveDown";
			this.toolStripButtonMoveDown.AutoSize = false;
			this.toolStripButtonMoveDown.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonMoveDown.Text = "Move Down";
			// 
			// contentBrowserProject
			// 
			this.contentBrowserProject.CanSelectObjectSettings = false;
			this.contentBrowserProject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentBrowserProject.FilteringMode = null;
			this.contentBrowserProject.ListViewModeOverride = null;
			this.contentBrowserProject.Location = new System.Drawing.Point(0, 0);
			this.contentBrowserProject.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.contentBrowserProject.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowserProject.MultiSelect = true;
			this.contentBrowserProject.Name = "contentBrowserProject";
			this.contentBrowserProject.ReadOnlyHierarchy = false;
			this.contentBrowserProject.ShowToolBar = false;
			this.contentBrowserProject.Size = new System.Drawing.Size(221, 387);
			this.contentBrowserProject.TabIndex = 4;
			this.contentBrowserProject.ThisIsSettingsWindow = false;
			// 
			// kryptonButtonReset
			// 
			this.kryptonButtonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.kryptonButtonReset.Location = new System.Drawing.Point(2, 441);
			this.kryptonButtonReset.Margin = new System.Windows.Forms.Padding(2);
			this.kryptonButtonReset.Name = "kryptonButtonReset";
			this.kryptonButtonReset.Size = new System.Drawing.Size(110, 26);
			this.kryptonButtonReset.TabIndex = 5;
			this.kryptonButtonReset.Values.Text = "Reset to Default";
			// 
			// kryptonLabel1
			// 
			this.kryptonLabel1.Location = new System.Drawing.Point(0, 3);
			this.kryptonLabel1.Margin = new System.Windows.Forms.Padding(2);
			this.kryptonLabel1.Name = "kryptonLabel1";
			this.kryptonLabel1.Size = new System.Drawing.Size(62, 16);
			this.kryptonLabel1.TabIndex = 6;
			this.kryptonLabel1.Values.Text = "All actions";
			// 
			// kryptonLabel2
			// 
			this.kryptonLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonLabel2.Location = new System.Drawing.Point(342, 3);
			this.kryptonLabel2.Margin = new System.Windows.Forms.Padding(2);
			this.kryptonLabel2.Name = "kryptonLabel2";
			this.kryptonLabel2.Size = new System.Drawing.Size(112, 16);
			this.kryptonLabel2.TabIndex = 7;
			this.kryptonLabel2.Values.Text = "Current configuration";
			// 
			// HCItemProjectRibbonAndToolbarActionsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.kryptonLabel2);
			this.Controls.Add(this.kryptonLabel1);
			this.Controls.Add(this.kryptonButtonReset);
			this.Controls.Add(this.kryptonSplitContainer1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "HCItemProjectRibbonAndToolbarActionsForm";
			this.Size = new System.Drawing.Size(456, 470);
			this.Load += new System.EventHandler( this.HCItemProjectRibbonAndToolbarActionsForm_Load );
			( (System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
			this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
			this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
			this.kryptonSplitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel1)).EndInit();
			this.kryptonSplitContainer2.Panel1.ResumeLayout(false);
			this.kryptonSplitContainer2.Panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel2)).EndInit();
			this.kryptonSplitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2)).EndInit();
			this.kryptonSplitContainer2.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public ContentBrowser contentBrowserAll;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
		public ContentBrowser contentBrowserProject;
		public Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonReset;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
		private System.Windows.Forms.ToolStrip toolStrip1;
		public System.Windows.Forms.ToolStripButton toolStripButtonDelete;
		public System.Windows.Forms.ToolStripButton toolStripButtonAdd;
		public System.Windows.Forms.ToolStripButton toolStripButtonRename;
		public System.Windows.Forms.ToolStripButton toolStripButtonMoveUp;
		public System.Windows.Forms.ToolStripButton toolStripButtonMoveDown;
		public System.Windows.Forms.ToolStripButton toolStripButtonEnabled;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		public System.Windows.Forms.ToolStripButton toolStripButtonNewGroup;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer2;
	}
}

#endif