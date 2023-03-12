#if !DEPLOY

namespace NeoAxis.Editor
{
	partial class StoresWindow
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
			this.toolStripForTreeView = new NeoAxis.Editor.EngineToolStrip();
			this.toolStripButtonOptions = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonStores = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonFilter = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSearch = new NeoAxis.Editor.ToolStripTextBoxHost();
			this.timer1 = new System.Windows.Forms.Timer( this.components );
			this.contentBrowser1 = new NeoAxis.Editor.ContentBrowser();
			//this.toolTip1 = new NeoAxis.Editor.EngineToolTip( this.components );
			this.toolStripForTreeView.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip
			// 
			this.toolStripForTreeView.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStripForTreeView.ImageScalingSize = new System.Drawing.Size( 20, 20 );
			this.toolStripForTreeView.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
			this.toolStripButtonOptions,
			this.toolStripButtonRefresh,
			this.toolStripButtonStores,
			this.toolStripButtonFilter,
			this.toolStripButtonSearch} );
			this.toolStripForTreeView.Location = new System.Drawing.Point( 0, 0 );
			this.toolStripForTreeView.Padding = new System.Windows.Forms.Padding( 1, 1, 1, 1 );
			this.toolStripForTreeView.Name = "toolStripForTreeView";
			this.toolStripForTreeView.ShowItemToolTips = false;
			this.toolStripForTreeView.CanOverflow = false;
			this.toolStripForTreeView.AutoSize = false;
			this.toolStripForTreeView.Size = new System.Drawing.Size( 511, 26 );
			this.toolStripForTreeView.TabIndex = 3;
			this.toolStripForTreeView.Text = "toolStrip1";
			// 
			// toolStripButtonOptions
			// 
			this.toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonOptions.Image = global::NeoAxis.Properties.Resources.Options_16;
			this.toolStripButtonOptions.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonOptions.Name = "toolStripButtonOptions";
			this.toolStripButtonOptions.AutoSize = false;
			this.toolStripButtonOptions.Size = new System.Drawing.Size( 23, 22 );
			this.toolStripButtonOptions.Text = "Options";
			this.toolStripButtonOptions.Click += new System.EventHandler( this.toolStripButtonOptions_Click );
			// 
			// toolStripButtonRefresh
			// 
			this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonRefresh.Image = global::NeoAxis.Properties.Resources.Refresh_16;
			this.toolStripButtonRefresh.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
			this.toolStripButtonRefresh.AutoSize = false;
			this.toolStripButtonRefresh.Size = new System.Drawing.Size( 23, 22 );
			this.toolStripButtonRefresh.Text = "Refresh";
			this.toolStripButtonRefresh.Click += new System.EventHandler( this.toolStripButtonRefresh_Click );
			// 
			// toolStripButtonStores
			// 
			this.toolStripButtonStores.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonStores.Image = global::NeoAxis.Properties.Resources.Selection_16;
			this.toolStripButtonStores.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonStores.Name = "toolStripButtonStores";
			this.toolStripButtonStores.AutoSize = false;
			this.toolStripButtonStores.Size = new System.Drawing.Size( 23, 22 );
			this.toolStripButtonStores.Text = "Stores";
			this.toolStripButtonStores.Click += new System.EventHandler( this.toolStripButtonStores_Click );
			// 
			// toolStripButtonFilter
			// 
			this.toolStripButtonFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonFilter.Image = global::NeoAxis.Properties.Resources.Filter_32;
			this.toolStripButtonFilter.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonFilter.Name = "toolStripButtonFilter";
			this.toolStripButtonFilter.AutoSize = false;
			this.toolStripButtonFilter.Size = new System.Drawing.Size( 23, 22 );
			this.toolStripButtonFilter.Text = "Filter";
			this.toolStripButtonFilter.Click += new System.EventHandler( this.toolStripButtonFilter_Click );
			// 
			// toolStripButtonSearch
			// 
			this.toolStripButtonSearch.Font = new System.Drawing.Font( "Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 204 ) ) );
			this.toolStripButtonSearch.Name = "toolStripButtonSearch";
			this.toolStripButtonSearch.AutoSize = false;
			this.toolStripButtonSearch.Size = new System.Drawing.Size( 106, 22 );
			this.toolStripButtonSearch.Text = "";
			this.toolStripButtonSearch.ToolTipText = "Search";
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler( this.timer1_Tick );
			// 
			// resourcesBrowser1
			// 
			this.contentBrowser1.CanSelectObjectSettings = false;
			this.contentBrowser1.Dock = System.Windows.Forms.DockStyle.None;// System.Windows.Forms.DockStyle.Fill;
			this.contentBrowser1.FilteringMode = null;
			this.contentBrowser1.Location = new System.Drawing.Point( 0, 0 );
			this.contentBrowser1.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
			this.contentBrowser1.Mode = ContentBrowser.ModeEnum.Objects;
			this.contentBrowser1.MultiSelect = true;
			this.contentBrowser1.Name = "contentBrowser1";
			this.contentBrowser1.ReadOnlyHierarchy = false;
			this.contentBrowser1.Size = new System.Drawing.Size( 416, 596 );
			this.contentBrowser1.TabIndex = 0;
			this.contentBrowser1.ThisIsSettingsWindow = false;
			this.contentBrowser1.TreeViewBorderDraw = NeoAxis.Editor.BorderSides.Top;
			this.contentBrowser1.ListViewBorderDraw = NeoAxis.Editor.BorderSides.Top;
			// 
			// StoresWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 8F, 16F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.toolStripForTreeView );
			this.Controls.Add( this.contentBrowser1 );
			this.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
			this.Name = "StoresWindow";
			this.Size = new System.Drawing.Size( 416, 596 );
			this.WindowTitle = "Stores";
			this.Load += new System.EventHandler( this.StoresWindow_Load );
			this.toolStripForTreeView.ResumeLayout( false );
			this.toolStripForTreeView.PerformLayout();
			this.ResumeLayout( false );
			this.PerformLayout();
		}

		#endregion

		private NeoAxis.Editor.EngineToolStrip toolStripForTreeView;
		private System.Windows.Forms.Timer timer1;
		private ContentBrowser contentBrowser1;
		//private NeoAxis.Editor.EngineToolTip toolTip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonOptions;
		private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
		private System.Windows.Forms.ToolStripButton toolStripButtonStores;
		private System.Windows.Forms.ToolStripButton toolStripButtonFilter;
		private NeoAxis.Editor.ToolStripTextBoxHost toolStripButtonSearch;
	}
}

#endif