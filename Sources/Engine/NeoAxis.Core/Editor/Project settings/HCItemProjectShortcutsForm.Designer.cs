#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class HCItemProjectShortcutsForm
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
			this.hierarchicalContainerSelected = new NeoAxis.Editor.HierarchicalContainer();
			this.kryptonButtonReset = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonLabel2 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.kryptonLabel1 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
			this.kryptonSplitContainer1.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
			this.kryptonSplitContainer1.Panel2.SuspendLayout();
			this.kryptonSplitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// kryptonSplitContainer1
			// 
			this.kryptonSplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
			this.kryptonSplitContainer1.Location = new System.Drawing.Point(3, 30);
			this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
			// 
			// kryptonSplitContainer1.Panel1
			// 
			this.kryptonSplitContainer1.Panel1.Controls.Add(this.contentBrowserAll);
			// 
			// kryptonSplitContainer1.Panel2
			// 
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.hierarchicalContainerSelected);
			this.kryptonSplitContainer1.Size = new System.Drawing.Size(602, 506);
			this.kryptonSplitContainer1.SplitterDistance = 301;
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
			this.contentBrowserAll.Margin = new System.Windows.Forms.Padding(4);
			this.contentBrowserAll.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowserAll.Name = "contentBrowserAll";
			this.contentBrowserAll.ReadOnlyHierarchy = false;
			this.contentBrowserAll.ShowToolBar = false;
			this.contentBrowserAll.Size = new System.Drawing.Size(301, 506);
			this.contentBrowserAll.TabIndex = 3;
			this.contentBrowserAll.ThisIsSettingsWindow = false;
			// 
			// hierarchicalContainerSelected
			// 
			this.hierarchicalContainerSelected.ContentMode = NeoAxis.Editor.HierarchicalContainer.ContentModeEnum.Properties;
			this.hierarchicalContainerSelected.Dock = System.Windows.Forms.DockStyle.Fill;
			this.hierarchicalContainerSelected.Location = new System.Drawing.Point(0, 0);
			this.hierarchicalContainerSelected.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.hierarchicalContainerSelected.Name = "hierarchicalContainerSelected";
			this.hierarchicalContainerSelected.Size = new System.Drawing.Size(296, 506);
			this.hierarchicalContainerSelected.SplitterPosition = 118;
			this.hierarchicalContainerSelected.SplitterRatio = 0.4F;
			this.hierarchicalContainerSelected.TabIndex = 6;
			// 
			// kryptonButtonReset
			// 
			this.kryptonButtonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.kryptonButtonReset.Location = new System.Drawing.Point(3, 543);
			this.kryptonButtonReset.Name = "kryptonButtonReset";
			this.kryptonButtonReset.Size = new System.Drawing.Size(147, 32);
			this.kryptonButtonReset.TabIndex = 5;
			this.kryptonButtonReset.Values.Text = "Reset to Default";
			// 
			// kryptonLabel2
			// 
			this.kryptonLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonLabel2.Location = new System.Drawing.Point(493, 4);
			this.kryptonLabel2.Name = "kryptonLabel2";
			this.kryptonLabel2.Size = new System.Drawing.Size(112, 20);
			this.kryptonLabel2.TabIndex = 9;
			this.kryptonLabel2.Values.Text = "Selected action";
			this.kryptonLabel2.Visible = false;
			// 
			// kryptonLabel1
			// 
			this.kryptonLabel1.Location = new System.Drawing.Point(0, 4);
			this.kryptonLabel1.Name = "kryptonLabel1";
			this.kryptonLabel1.Size = new System.Drawing.Size(79, 20);
			this.kryptonLabel1.TabIndex = 8;
			this.kryptonLabel1.Values.Text = "All actions";
			// 
			// HCItemProjectShortcutsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.kryptonLabel2);
			this.Controls.Add(this.kryptonLabel1);
			this.Controls.Add(this.kryptonButtonReset);
			this.Controls.Add(this.kryptonSplitContainer1);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "HCItemProjectShortcutsForm";
			this.Size = new System.Drawing.Size(608, 578);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
			this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
			this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
			this.kryptonSplitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public ContentBrowser contentBrowserAll;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
		public Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonReset;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
		public HierarchicalContainer hierarchicalContainerSelected;
	}
}

#endif