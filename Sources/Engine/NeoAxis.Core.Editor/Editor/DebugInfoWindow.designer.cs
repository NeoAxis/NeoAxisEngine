#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class DebugInfoWindow
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
			this.splitContainer1 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.contentBrowserList = new NeoAxis.Editor.ContentBrowser();
			this.contentBrowserData = new NeoAxis.Editor.ContentBrowser();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.contentBrowserList);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.contentBrowserData);
			this.splitContainer1.Size = new System.Drawing.Size(625, 442);
			this.splitContainer1.SplitterDistance = 168;
			this.splitContainer1.SplitterPercent = 0.3;
			this.splitContainer1.TabIndex = 0;
			// 
			// contentBrowserList
			// 
			this.contentBrowserList.CanSelectObjectSettings = false;
			this.contentBrowserList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentBrowserList.FilteringMode = null;
			this.contentBrowserList.Location = new System.Drawing.Point(0, 0);
			this.contentBrowserList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.contentBrowserList.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowserList.Name = "contentBrowserList";
			this.contentBrowserList.ReadOnlyHierarchy = false;
			this.contentBrowserList.ShowToolBar = false;
			this.contentBrowserList.Size = new System.Drawing.Size(168, 442);
			this.contentBrowserList.TabIndex = 0;
			this.contentBrowserList.ThisIsSettingsWindow = false;
			this.contentBrowserList.ItemAfterSelect += new NeoAxis.Editor.ContentBrowser.ItemAfterSelectDelegate(this.contentBrowserList_ItemAfterSelect);
			// 
			// contentBrowserData
			// 
			this.contentBrowserData.CanSelectObjectSettings = false;
			this.contentBrowserData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentBrowserData.FilteringMode = null;
			this.contentBrowserData.Location = new System.Drawing.Point(0, 0);
			this.contentBrowserData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.contentBrowserData.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowserData.Name = "contentBrowserData";
			this.contentBrowserData.ReadOnlyHierarchy = false;
			this.contentBrowserData.ShowToolBar = false;
			this.contentBrowserData.Size = new System.Drawing.Size(453, 442);
			this.contentBrowserData.TabIndex = 1;
			this.contentBrowserData.ThisIsSettingsWindow = false;
			// 
			// timer1
			// 
			this.timer1.Interval = 500;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// DebugInfoWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.Controls.Add(this.splitContainer1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.471698F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "DebugInfoWindow";
			this.Size = new System.Drawing.Size(625, 442);
			this.WindowTitle = "Debug Info";
			this.Load += new System.EventHandler(this.DebugInfoForm_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Internal.ComponentFactory.Krypton.Toolkit.KryptonSplitContainer splitContainer1;
		private ContentBrowser contentBrowserList;
		private System.Windows.Forms.Timer timer1;
		private ContentBrowser contentBrowserData;
	}
}

#endif