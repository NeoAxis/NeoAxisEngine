namespace NeoAxis.Editor
{
	partial class MessageLogWindow
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
			this.toolStrip1 = new NeoAxis.Editor.EngineToolStrip();
			this.toolStripButtonOptions = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparatorFilteringMode = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonClear = new System.Windows.Forms.ToolStripButton();
			this.kryptonSplitContainer1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.contentBrowser1 = new NeoAxis.Editor.ContentBrowser();
			//this.kryptonRichTextBox1 = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
			this.kryptonSplitContainer1.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
			this.kryptonSplitContainer1.Panel2.SuspendLayout();
			this.kryptonSplitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonOptions,
            this.toolStripSeparatorFilteringMode,
            this.toolStripButtonClear});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.Size = new System.Drawing.Size(713, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonOptions
			// 
			this.toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonOptions.Image = global::NeoAxis.Properties.Resources.Options_16;
			this.toolStripButtonOptions.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonOptions.Name = "toolStripButtonOptions";
			this.toolStripButtonOptions.AutoSize = false;
			this.toolStripButtonOptions.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonOptions.Text = "Options";
			this.toolStripButtonOptions.Click += new System.EventHandler(this.toolStripButtonOptions_Click);
			// 
			// toolStripSeparatorFilteringMode
			// 
			this.toolStripSeparatorFilteringMode.Name = "toolStripSeparatorFilteringMode";
			this.toolStripSeparatorFilteringMode.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonClear
			// 
			this.toolStripButtonClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonClear.Image = global::NeoAxis.Properties.Resources.Delete_16;
			this.toolStripButtonClear.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonClear.Name = "toolStripButtonClear";
			this.toolStripButtonClear.AutoSize = false;
			this.toolStripButtonClear.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonClear.Text = "Clear";
			this.toolStripButtonClear.Click += new System.EventHandler(this.toolStripButtonClear_Click);
			// 
			// kryptonSplitContainer1
			// 
			this.kryptonSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonSplitContainer1.Location = new System.Drawing.Point(0, 25);
			this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
			// 
			// kryptonSplitContainer1.Panel1
			// 
			this.kryptonSplitContainer1.Panel1.Controls.Add(this.contentBrowser1);
			// 
			// kryptonSplitContainer1.Panel2
			// 
			//this.kryptonSplitContainer1.Panel2.Controls.Add(this.kryptonRichTextBox1);
			this.kryptonSplitContainer1.Size = new System.Drawing.Size(713, 165);
			this.kryptonSplitContainer1.SplitterDistance = 345;
			this.kryptonSplitContainer1.SplitterPercent = 0.4838709677419355D;
			this.kryptonSplitContainer1.TabIndex = 2;
			// 
			// contentBrowser1
			// 
			this.contentBrowser1.CanSelectObjectSettings = false;
			this.contentBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentBrowser1.FilteringMode = null;
			this.contentBrowser1.ListViewModeOverride = null;
			this.contentBrowser1.Location = new System.Drawing.Point(0, 0);
			this.contentBrowser1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.contentBrowser1.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Objects;
			this.contentBrowser1.Name = "contentBrowser1";
			this.contentBrowser1.ReadOnlyHierarchy = false;
			this.contentBrowser1.ShowToolBar = false;
			this.contentBrowser1.Size = new System.Drawing.Size(345, 165);
			this.contentBrowser1.TabIndex = 2;
			this.contentBrowser1.ThisIsSettingsWindow = false;
			this.contentBrowser1.ItemAfterSelect += new NeoAxis.Editor.ContentBrowser.ItemAfterSelectDelegate(this.contentBrowser1_ItemAfterSelect);
			//// 
			//// kryptonRichTextBox1
			//// 
			//this.kryptonRichTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			//this.kryptonRichTextBox1.Location = new System.Drawing.Point(0, 0);
			//this.kryptonRichTextBox1.Name = "kryptonRichTextBox1";
			//this.kryptonRichTextBox1.ReadOnly = true;
			//this.kryptonRichTextBox1.Size = new System.Drawing.Size(363, 165);
			//this.kryptonRichTextBox1.StateCommon.Content.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			//this.kryptonRichTextBox1.TabIndex = 2;
			//this.kryptonRichTextBox1.Text = "";
			//this.kryptonRichTextBox1.WordWrap = false;
			// 
			// timer1
			// 
			this.timer1.Interval = 50;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// MessageLogWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.Controls.Add(this.kryptonSplitContainer1);
			this.Controls.Add(this.toolStrip1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.471698F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "MessageLogWindow";
			this.Size = new System.Drawing.Size(713, 190);
			this.WindowTitle = "Message Log";
			this.Load += new System.EventHandler(this.MessageLogWindow_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
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
		private NeoAxis.Editor.EngineToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonClear;
		private System.Windows.Forms.ToolStripButton toolStripButtonOptions;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparatorFilteringMode;
		private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
		//private ComponentFactory.Krypton.Toolkit.KryptonRichTextBox kryptonRichTextBox1;
		private ContentBrowser contentBrowser1;
		private System.Windows.Forms.Timer timer1;
	}
}
