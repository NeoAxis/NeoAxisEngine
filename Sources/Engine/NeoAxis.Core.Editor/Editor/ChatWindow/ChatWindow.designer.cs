namespace NeoAxis.Editor
{
	partial class ChatWindow
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
			//this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
			this.timer1 = new System.Windows.Forms.Timer( this.components );
			this.textBox1 = new NeoAxis.Editor.EngineTextBox();
			this.toolStripForTreeView.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip
			// 
			this.toolStripForTreeView.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStripForTreeView.ImageScalingSize = new System.Drawing.Size( 20, 20 );
			this.toolStripForTreeView.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
			this.toolStripButtonOptions } );
			//this.toolStripButtonRefresh } );
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
			this.toolStripButtonOptions.Image = global::NeoAxis.Editor.Properties.Resources.Options_16;
			this.toolStripButtonOptions.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonOptions.Name = "toolStripButtonOptions";
			this.toolStripButtonOptions.AutoSize = false;
			this.toolStripButtonOptions.Size = new System.Drawing.Size( 23, 22 );
			this.toolStripButtonOptions.Text = "Options";
			this.toolStripButtonOptions.Click += new System.EventHandler( this.toolStripButtonOptions_Click );
			// 
			// toolStripButtonRefresh
			// 
			//this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			//this.toolStripButtonRefresh.Image = global::NeoAxis.Editor.Properties.Resources.Refresh_16;
			//this.toolStripButtonRefresh.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			//this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
			//this.toolStripButtonRefresh.AutoSize = false;
			//this.toolStripButtonRefresh.Size = new System.Drawing.Size( 23, 22 );
			//this.toolStripButtonRefresh.Text = "Refresh";
			//this.toolStripButtonRefresh.Click += new System.EventHandler( this.toolStripButtonRefresh_Click );
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler( this.timer1_Tick );
			// 
			// textBox1
			//
			this.textBox1.Dock = System.Windows.Forms.DockStyle.None;// System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Location = new System.Drawing.Point( 0, 0 );
			this.textBox1.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
			this.textBox1.Size = new System.Drawing.Size( 307, 200 );
			//this.textBox1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
			//| System.Windows.Forms.AnchorStyles.Right ) ) );
			//this.textBox1.Location = new System.Drawing.Point( 0, 3 );
			this.textBox1.LikeLabel = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.TabIndex = 0;
			this.textBox1.Multiline = true;
			// 
			// ChatWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 8F, 16F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.toolStripForTreeView );
			this.Controls.Add( this.textBox1 );
			this.Margin = new System.Windows.Forms.Padding( 5, 4, 5, 4 );
			this.Name = "ChatWindow";
			this.Size = new System.Drawing.Size( 416, 596 );
			this.WindowTitle = "Chat";
			this.Load += new System.EventHandler( this.ChatWindow_Load );
			this.toolStripForTreeView.ResumeLayout( false );
			this.toolStripForTreeView.PerformLayout();
			this.ResumeLayout( false );
			this.PerformLayout();
		}

		#endregion

		private NeoAxis.Editor.EngineToolStrip toolStripForTreeView;
		private System.Windows.Forms.Timer timer1;
		private NeoAxis.Editor.EngineTextBox textBox1;
		private System.Windows.Forms.ToolStripButton toolStripButtonOptions;
		//private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
	}
}