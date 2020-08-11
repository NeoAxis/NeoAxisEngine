namespace NeoAxis.Editor
{
	partial class HCGridColorValuePowered
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
			this.previewButton = new NeoAxis.Editor.HCColorPreviewButton();
			this.trackBarPower = new ComponentFactory.Krypton.Toolkit.KryptonTrackBar();
			this.textBox1 = new NeoAxis.Editor.EngineTextBox();
			this.SuspendLayout();
			// 
			// previewButton
			// 
			this.previewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.previewButton.Location = new System.Drawing.Point(203, 3);
			this.previewButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.previewButton.Name = "previewButton";
			this.previewButton.Size = new System.Drawing.Size(27, 22);
			this.previewButton.TabIndex = 1;
			this.previewButton.Values.Text = "";
			// 
			// trackBarPower
			// 
			this.trackBarPower.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.trackBarPower.DrawBackground = true;
			this.trackBarPower.Location = new System.Drawing.Point(232, 4);
			this.trackBarPower.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.trackBarPower.Name = "trackBarPower";
			this.trackBarPower.Size = new System.Drawing.Size(71, 20);
			this.trackBarPower.TabIndex = 2;
			this.trackBarPower.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBarPower.TrackBarSize = ComponentFactory.Krypton.Toolkit.PaletteTrackBarSize.Small;
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point( 0, 4 );
			//this.textBox1.Location = new System.Drawing.Point(0, 3);
			this.textBox1.LikeLabel = false;
			this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(200, 22);
			this.textBox1.TabIndex = 0;
			// 
			// HCGridColorValuePowered
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.trackBarPower);
			this.Controls.Add(this.previewButton);
			this.Controls.Add(this.textBox1);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "HCGridColorValuePowered";
			this.Size = new System.Drawing.Size(308, 28);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		public EngineTextBox textBox1;
		public HCColorPreviewButton previewButton;
		private ComponentFactory.Krypton.Toolkit.KryptonTrackBar trackBarPower;
	}
}
