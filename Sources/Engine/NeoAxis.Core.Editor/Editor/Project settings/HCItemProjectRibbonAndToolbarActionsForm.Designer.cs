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
			kryptonSplitContainer1 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			contentBrowserAll = new ContentBrowser();
			kryptonSplitContainer2 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			toolStrip1 = new System.Windows.Forms.ToolStrip();
			toolStripButtonEnabled = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripButtonNewGroup = new System.Windows.Forms.ToolStripButton();
			toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
			toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
			toolStripButtonRename = new System.Windows.Forms.ToolStripButton();
			toolStripButtonMoveUp = new System.Windows.Forms.ToolStripButton();
			toolStripButtonMoveDown = new System.Windows.Forms.ToolStripButton();
			contentBrowserProject = new ContentBrowser();
			kryptonButtonReset = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			kryptonLabel1 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			kryptonLabel2 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			( (System.ComponentModel.ISupportInitialize)kryptonSplitContainer1 ).BeginInit();
			( kryptonSplitContainer1.Panel1 ).BeginInit();
			kryptonSplitContainer1.Panel1.SuspendLayout();
			( kryptonSplitContainer1.Panel2 ).BeginInit();
			kryptonSplitContainer1.Panel2.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize)kryptonSplitContainer2 ).BeginInit();
			( kryptonSplitContainer2.Panel1 ).BeginInit();
			kryptonSplitContainer2.Panel1.SuspendLayout();
			( kryptonSplitContainer2.Panel2 ).BeginInit();
			kryptonSplitContainer2.Panel2.SuspendLayout();
			toolStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// kryptonSplitContainer1
			// 
			kryptonSplitContainer1.Anchor =   System.Windows.Forms.AnchorStyles.Top  |  System.Windows.Forms.AnchorStyles.Left   |  System.Windows.Forms.AnchorStyles.Right ;
			kryptonSplitContainer1.Location = new System.Drawing.Point( 3, 28 );
			kryptonSplitContainer1.Margin = new System.Windows.Forms.Padding( 3, 4, 3, 4 );
			kryptonSplitContainer1.Name = "kryptonSplitContainer1";
			// 
			// 
			// 
			kryptonSplitContainer1.Panel1.Controls.Add( contentBrowserAll );
			// 
			// 
			// 
			kryptonSplitContainer1.Panel2.Controls.Add( kryptonSplitContainer2 );
			kryptonSplitContainer1.Size = new System.Drawing.Size( 602, 632 );
			kryptonSplitContainer1.SplitterDistance = 301;
			kryptonSplitContainer1.SplitterPercent = 0.5D;
			kryptonSplitContainer1.TabIndex = 4;
			// 
			// contentBrowserAll
			// 
			contentBrowserAll.Dock = System.Windows.Forms.DockStyle.Fill;
			contentBrowserAll.FilteringMode = null;
			contentBrowserAll.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8F );
			contentBrowserAll.ListViewModeOverride = null;
			contentBrowserAll.Location = new System.Drawing.Point( 0, 0 );
			contentBrowserAll.Margin = new System.Windows.Forms.Padding( 4, 5, 4, 5 );
			contentBrowserAll.Mode = ContentBrowser.ModeEnum.Resources;
			contentBrowserAll.Name = "contentBrowserAll";
			contentBrowserAll.ReadOnlyHierarchy = false;
			contentBrowserAll.ShowToolBar = false;
			contentBrowserAll.Size = new System.Drawing.Size( 301, 632 );
			contentBrowserAll.TabIndex = 3;
			contentBrowserAll.ThisIsSettingsWindow = false;
			// 
			// kryptonSplitContainer2
			// 
			kryptonSplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			kryptonSplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			kryptonSplitContainer2.IsSplitterFixed = true;
			kryptonSplitContainer2.Location = new System.Drawing.Point( 0, 0 );
			kryptonSplitContainer2.Margin = new System.Windows.Forms.Padding( 3, 4, 3, 4 );
			kryptonSplitContainer2.Name = "kryptonSplitContainer2";
			kryptonSplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// 
			// 
			kryptonSplitContainer2.Panel1.Controls.Add( toolStrip1 );
			kryptonSplitContainer2.Panel1MinSize = 10;
			// 
			// 
			// 
			kryptonSplitContainer2.Panel2.Controls.Add( contentBrowserProject );
			kryptonSplitContainer2.Size = new System.Drawing.Size( 296, 632 );
			kryptonSplitContainer2.SplitterDistance = 24;
			kryptonSplitContainer2.SplitterPercent = 0.0379746835443038D;
			kryptonSplitContainer2.SplitterWidth = 0;
			kryptonSplitContainer2.TabIndex = 6;
			// 
			// toolStrip1
			// 
			toolStrip1.AutoSize = false;
			toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			toolStrip1.ImageScalingSize = new System.Drawing.Size( 20, 20 );
			toolStrip1.Items.AddRange( new System.Windows.Forms.ToolStripItem[] { toolStripButtonEnabled, toolStripSeparator1, toolStripButtonNewGroup, toolStripButtonAdd, toolStripButtonDelete, toolStripButtonRename, toolStripButtonMoveUp, toolStripButtonMoveDown } );
			toolStrip1.Location = new System.Drawing.Point( 0, 0 );
			toolStrip1.Name = "toolStrip1";
			toolStrip1.Padding = new System.Windows.Forms.Padding( 1 );
			toolStrip1.Size = new System.Drawing.Size( 296, 32 );
			toolStrip1.TabIndex = 5;
			toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonEnabled
			// 
			toolStripButtonEnabled.AutoSize = false;
			toolStripButtonEnabled.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			toolStripButtonEnabled.Image = Properties.Resources.Checked_16;
			toolStripButtonEnabled.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			toolStripButtonEnabled.Name = "toolStripButtonEnabled";
			toolStripButtonEnabled.Size = new System.Drawing.Size( 23, 22 );
			toolStripButtonEnabled.Text = "Enabled";
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size( 6, 30 );
			// 
			// toolStripButtonNewGroup
			// 
			toolStripButtonNewGroup.AutoSize = false;
			toolStripButtonNewGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			toolStripButtonNewGroup.Image = Properties.Resources.NewFolder_16;
			toolStripButtonNewGroup.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			toolStripButtonNewGroup.Name = "toolStripButtonNewGroup";
			toolStripButtonNewGroup.Size = new System.Drawing.Size( 23, 22 );
			toolStripButtonNewGroup.Text = "New Group";
			// 
			// toolStripButtonAdd
			// 
			toolStripButtonAdd.AutoSize = false;
			toolStripButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			toolStripButtonAdd.Image = Properties.Resources.Add_16;
			toolStripButtonAdd.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			toolStripButtonAdd.Name = "toolStripButtonAdd";
			toolStripButtonAdd.Size = new System.Drawing.Size( 23, 22 );
			toolStripButtonAdd.Text = "Add";
			// 
			// toolStripButtonDelete
			// 
			toolStripButtonDelete.AutoSize = false;
			toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			toolStripButtonDelete.Image = Properties.Resources.Delete_16;
			toolStripButtonDelete.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			toolStripButtonDelete.Name = "toolStripButtonDelete";
			toolStripButtonDelete.Size = new System.Drawing.Size( 23, 22 );
			toolStripButtonDelete.Text = "Delete";
			// 
			// toolStripButtonRename
			// 
			toolStripButtonRename.AutoSize = false;
			toolStripButtonRename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			toolStripButtonRename.Image = Properties.Resources.Rename_16x;
			toolStripButtonRename.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			toolStripButtonRename.Name = "toolStripButtonRename";
			toolStripButtonRename.Size = new System.Drawing.Size( 23, 22 );
			toolStripButtonRename.Text = "Rename";
			// 
			// toolStripButtonMoveUp
			// 
			toolStripButtonMoveUp.AutoSize = false;
			toolStripButtonMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			toolStripButtonMoveUp.Image = Properties.Resources.MoveUp_16;
			toolStripButtonMoveUp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			toolStripButtonMoveUp.Name = "toolStripButtonMoveUp";
			toolStripButtonMoveUp.Size = new System.Drawing.Size( 23, 22 );
			toolStripButtonMoveUp.Text = "Move Up";
			// 
			// toolStripButtonMoveDown
			// 
			toolStripButtonMoveDown.AutoSize = false;
			toolStripButtonMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			toolStripButtonMoveDown.Image = Properties.Resources.MoveDown_16;
			toolStripButtonMoveDown.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			toolStripButtonMoveDown.Name = "toolStripButtonMoveDown";
			toolStripButtonMoveDown.Size = new System.Drawing.Size( 23, 22 );
			toolStripButtonMoveDown.Text = "Move Down";
			// 
			// contentBrowserProject
			// 
			contentBrowserProject.Dock = System.Windows.Forms.DockStyle.Fill;
			contentBrowserProject.FilteringMode = null;
			contentBrowserProject.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8F );
			contentBrowserProject.ListViewModeOverride = null;
			contentBrowserProject.Location = new System.Drawing.Point( 0, 0 );
			contentBrowserProject.Margin = new System.Windows.Forms.Padding( 2 );
			contentBrowserProject.Mode = ContentBrowser.ModeEnum.Resources;
			contentBrowserProject.MultiSelect = true;
			contentBrowserProject.Name = "contentBrowserProject";
			contentBrowserProject.ReadOnlyHierarchy = false;
			contentBrowserProject.ShowToolBar = false;
			contentBrowserProject.Size = new System.Drawing.Size( 296, 608 );
			contentBrowserProject.TabIndex = 4;
			contentBrowserProject.ThisIsSettingsWindow = false;
			// 
			// kryptonButtonReset
			// 
			kryptonButtonReset.Anchor =  System.Windows.Forms.AnchorStyles.Bottom  |  System.Windows.Forms.AnchorStyles.Left ;
			kryptonButtonReset.Location = new System.Drawing.Point( 3, 669 );
			kryptonButtonReset.Margin = new System.Windows.Forms.Padding( 2 );
			kryptonButtonReset.Name = "kryptonButtonReset";
			kryptonButtonReset.Size = new System.Drawing.Size( 147, 32 );
			kryptonButtonReset.TabIndex = 5;
			kryptonButtonReset.Values.Text = "Reset to Default";
			// 
			// kryptonLabel1
			// 
			kryptonLabel1.Location = new System.Drawing.Point( 0, 5 );
			kryptonLabel1.Margin = new System.Windows.Forms.Padding( 3, 4, 3, 4 );
			kryptonLabel1.Name = "kryptonLabel1";
			kryptonLabel1.Size = new System.Drawing.Size( 75, 19 );
			kryptonLabel1.TabIndex = 6;
			kryptonLabel1.Values.Text = "All actions";
			// 
			// kryptonLabel2
			// 
			kryptonLabel2.Anchor =  System.Windows.Forms.AnchorStyles.Top  |  System.Windows.Forms.AnchorStyles.Right ;
			kryptonLabel2.Location = new System.Drawing.Point( 466, 4 );
			kryptonLabel2.Margin = new System.Windows.Forms.Padding( 2 );
			kryptonLabel2.Name = "kryptonLabel2";
			kryptonLabel2.Size = new System.Drawing.Size( 135, 19 );
			kryptonLabel2.TabIndex = 7;
			kryptonLabel2.Values.Text = "Current configuration";
			// 
			// HCItemProjectRibbonAndToolbarActionsForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF( 8F, 20F );
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			Controls.Add( kryptonLabel2 );
			Controls.Add( kryptonLabel1 );
			Controls.Add( kryptonButtonReset );
			Controls.Add( kryptonSplitContainer1 );
			Margin = new System.Windows.Forms.Padding( 3, 2, 3, 2 );
			Name = "HCItemProjectRibbonAndToolbarActionsForm";
			Size = new System.Drawing.Size( 608, 722 );
			Load +=  HCItemProjectRibbonAndToolbarActionsForm_Load ;
			( kryptonSplitContainer1.Panel1 ).EndInit();
			kryptonSplitContainer1.Panel1.ResumeLayout( false );
			( kryptonSplitContainer1.Panel2 ).EndInit();
			kryptonSplitContainer1.Panel2.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)kryptonSplitContainer1 ).EndInit();
			( kryptonSplitContainer2.Panel1 ).EndInit();
			kryptonSplitContainer2.Panel1.ResumeLayout( false );
			( kryptonSplitContainer2.Panel2 ).EndInit();
			kryptonSplitContainer2.Panel2.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)kryptonSplitContainer2 ).EndInit();
			toolStrip1.ResumeLayout( false );
			toolStrip1.PerformLayout();
			ResumeLayout( false );
			PerformLayout();

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