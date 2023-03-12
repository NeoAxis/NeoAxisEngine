#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class KryptonRibbonGroupListBoxControl
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
			this.kryptonLabel1 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.kryptonSplitContainer1 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.contentBrowser1 = new NeoAxis.Editor.ContentBrowser();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
			this.kryptonSplitContainer1.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
			this.kryptonSplitContainer1.Panel2.SuspendLayout();
			this.kryptonSplitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// kryptonLabel1
			// 
			this.kryptonLabel1.AutoSize = false;
			this.kryptonLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonLabel1.Location = new System.Drawing.Point(0, 0);
			this.kryptonLabel1.Name = "kryptonLabel1";
			this.kryptonLabel1.Size = new System.Drawing.Size(169, 18);
			this.kryptonLabel1.StateCommon.Padding = new System.Windows.Forms.Padding(0);
			this.kryptonLabel1.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.kryptonLabel1.StateCommon.ShortText.TextH = Internal.ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
			this.kryptonLabel1.StateCommon.ShortText.TextV = Internal.ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
			this.kryptonLabel1.TabIndex = 0;
			this.kryptonLabel1.Values.Text = "kryptonLabel1";
			// 
			// kryptonSplitContainer1
			// 
			this.kryptonSplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
			this.kryptonSplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.kryptonSplitContainer1.IsSplitterFixed = true;
			this.kryptonSplitContainer1.Location = new System.Drawing.Point(0, 0);
			this.kryptonSplitContainer1.Margin = new System.Windows.Forms.Padding(0);
			this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
			this.kryptonSplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// kryptonSplitContainer1.Panel1
			// 
			this.kryptonSplitContainer1.Panel1.Controls.Add(this.contentBrowser1);
			// 
			// kryptonSplitContainer1.Panel2
			// 
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.kryptonLabel1);
			this.kryptonSplitContainer1.Panel2MinSize = 18;
			this.kryptonSplitContainer1.Size = new System.Drawing.Size(169, 89);
			this.kryptonSplitContainer1.SplitterDistance = 70;
			this.kryptonSplitContainer1.SplitterPercent = 0.7865168539325843D;
			this.kryptonSplitContainer1.SplitterWidth = 1;
			this.kryptonSplitContainer1.TabIndex = 3;
			// 
			// contentBrowser1
			// 
			this.contentBrowser1.CanSelectObjectSettings = false;
			this.contentBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentBrowser1.FilteringMode = null;
			this.contentBrowser1.ListViewModeOverride = null;
			this.contentBrowser1.Location = new System.Drawing.Point(0, 0);
			this.contentBrowser1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.contentBrowser1.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowser1.Name = "contentBrowser1";
			this.contentBrowser1.ReadOnlyHierarchy = false;
			this.contentBrowser1.ShowToolBar = false;
			this.contentBrowser1.Size = new System.Drawing.Size(169, 70);
			this.contentBrowser1.TabIndex = 2;
			this.contentBrowser1.ThisIsSettingsWindow = false;
			// 
			// KryptonRibbonGroupListBoxControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.kryptonSplitContainer1);
			this.Name = "KryptonRibbonGroupListBoxControl";
			this.Size = new System.Drawing.Size(172, 92);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
			this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
			this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
			this.kryptonSplitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		public Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
		public ContentBrowser contentBrowser1;
		public Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
	}
}

#endif