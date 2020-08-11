namespace NeoAxis.Editor
{
	partial class OutputWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutputWindow));
			this.toolStrip1 = new NeoAxis.Editor.EngineToolStrip();
			this.toolStripButtonOptions = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparatorFilteringMode = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonClear = new System.Windows.Forms.ToolStripButton();
			this.messageImageList = new System.Windows.Forms.ImageList(this.components);
			//this.kryptonRichTextBox1 = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox();
			this.toolStrip1.SuspendLayout();
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
			this.toolStripButtonOptions.Enabled = false;
			this.toolStripButtonOptions.Image = global::NeoAxis.Properties.Resources.Options_16;
			this.toolStripButtonOptions.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonOptions.Name = "toolStripButtonOptions";
			this.toolStripButtonOptions.AutoSize = false;
			this.toolStripButtonOptions.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonOptions.Text = "Options";
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
			// messageImageList
			// 
			this.messageImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("messageImageList.ImageStream")));
			this.messageImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.messageImageList.Images.SetKeyName(0, "StatusAnnotations_Information_16xLG_color.png");
			this.messageImageList.Images.SetKeyName(1, "StatusAnnotations_Warning_16xLG_color.png");
			this.messageImageList.Images.SetKeyName(2, "StatusAnnotations_Critical_16xLG_color.png");
			//// 
			//// kryptonRichTextBox1
			//// 
			//this.kryptonRichTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			//this.kryptonRichTextBox1.Location = new System.Drawing.Point(0, 25);
			//this.kryptonRichTextBox1.Name = "kryptonRichTextBox1";
			//this.kryptonRichTextBox1.ReadOnly = true;
			//this.kryptonRichTextBox1.Size = new System.Drawing.Size(713, 165);
			//this.kryptonRichTextBox1.StateCommon.Content.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			//this.kryptonRichTextBox1.TabIndex = 2;
			//this.kryptonRichTextBox1.Text = "";
			//this.kryptonRichTextBox1.WordWrap = false;
			// 
			// OutputWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			//this.Controls.Add(this.kryptonRichTextBox1);
			this.Controls.Add(this.toolStrip1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.471698F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "OutputWindow";
			this.Size = new System.Drawing.Size(713, 190);
			this.WindowTitle = "Output";
			this.Load += new System.EventHandler( this.OutputWindow_Load );
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private NeoAxis.Editor.EngineToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonClear;
		private System.Windows.Forms.ToolStripButton toolStripButtonOptions;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparatorFilteringMode;
		private System.Windows.Forms.ImageList messageImageList;
		//private ComponentFactory.Krypton.Toolkit.KryptonRichTextBox kryptonRichTextBox1;
	}
}
