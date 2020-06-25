namespace NeoAxis.Editor
{
	partial class HCGridTextBoxNumeric
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
			this.textBox1 = new NeoAxis.Editor.HCKryptonTextBox();
			this.trackBar = new ComponentFactory.Krypton.Toolkit.KryptonTrackBar();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(0, 3);
			this.textBox1.LookLikeLabel = false;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(64, 21);
			this.textBox1.TabIndex = 1;
			// 
			// trackBar
			// 
			this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.trackBar.DrawBackground = true;
			this.trackBar.Location = new System.Drawing.Point(69, 4);
			this.trackBar.Name = "trackBar";
			this.trackBar.Size = new System.Drawing.Size(237, 20);
			this.trackBar.TabIndex = 2;
			this.trackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBar.TrackBarSize = ComponentFactory.Krypton.Toolkit.PaletteTrackBarSize.Small;
			// 
			// HCGridTextBoxNumeric
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.trackBar);
			this.Controls.Add(this.textBox1);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "HCGridTextBoxNumeric";
			this.Size = new System.Drawing.Size(307, 28);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		public HCKryptonTextBox textBox1;
		private ComponentFactory.Krypton.Toolkit.KryptonTrackBar trackBar;
	}
}
