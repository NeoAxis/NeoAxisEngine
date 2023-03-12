#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class RangeDropDownControl
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
			this.minLabel = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.maxLabel = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.minTextBox = new NeoAxis.Editor.EngineTextBox();
			this.maxTextBox = new NeoAxis.Editor.EngineTextBox();
			this.minTrackBar = new Internal.ComponentFactory.Krypton.Toolkit.KryptonTrackBar();
			this.maxTrackBar = new Internal.ComponentFactory.Krypton.Toolkit.KryptonTrackBar();
			this.SuspendLayout();
			// 
			// minLabel
			// 
			this.minLabel.AutoSize = false;
			this.minLabel.Location = new System.Drawing.Point(4, 13);
			this.minLabel.Margin = new System.Windows.Forms.Padding(4);
			this.minLabel.Name = "minLabel";
			this.minLabel.Size = new System.Drawing.Size(91, 25);
			this.minLabel.TabIndex = 0;
			this.minLabel.Values.Text = "Minimum:";
			// 
			// maxLabel
			// 
			this.maxLabel.AutoSize = false;
			this.maxLabel.Location = new System.Drawing.Point(4, 41);
			this.maxLabel.Margin = new System.Windows.Forms.Padding(4);
			this.maxLabel.Name = "maxLabel";
			this.maxLabel.Size = new System.Drawing.Size(91, 31);
			this.maxLabel.TabIndex = 1;
			this.maxLabel.Values.Text = "Maximum:";
			// 
			// minTextBox
			// 
			this.minTextBox.Location = new System.Drawing.Point(81, 15);
			this.minTextBox.LikeLabel = false;
			this.minTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.minTextBox.Name = "minTextBox";
			this.minTextBox.Size = new System.Drawing.Size(119, 21);
			this.minTextBox.TabIndex = 2;
			this.minTextBox.TextChanged += new System.EventHandler(this.anyTextBox_TextChanged);
			this.minTextBox.Validated += new System.EventHandler(this.anyTextBox_Validated);
			// 
			// maxTextBox
			// 
			this.maxTextBox.Location = new System.Drawing.Point(81, 47);
			this.maxTextBox.LikeLabel = false;
			this.maxTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.maxTextBox.Name = "maxTextBox";
			this.maxTextBox.Size = new System.Drawing.Size(119, 21);
			this.maxTextBox.TabIndex = 3;
			this.maxTextBox.TextChanged += new System.EventHandler(this.anyTextBox_TextChanged);
			this.maxTextBox.Validated += new System.EventHandler(this.anyTextBox_Validated);
			// 
			// minTrackBar
			// 
			this.minTrackBar.DrawBackground = true;
			this.minTrackBar.Location = new System.Drawing.Point(208, 12);
			this.minTrackBar.Margin = new System.Windows.Forms.Padding(4);
			this.minTrackBar.Name = "minTrackBar";
			this.minTrackBar.Size = new System.Drawing.Size(151, 26);
			this.minTrackBar.TabIndex = 4;
			this.minTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			// 
			// maxTrackBar
			// 
			this.maxTrackBar.DrawBackground = true;
			this.maxTrackBar.Location = new System.Drawing.Point(208, 44);
			this.maxTrackBar.Margin = new System.Windows.Forms.Padding(4);
			this.maxTrackBar.Name = "maxTrackBar";
			this.maxTrackBar.Size = new System.Drawing.Size(151, 26);
			this.maxTrackBar.TabIndex = 5;
			this.maxTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
			// 
			// RangeDropDownControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.maxTrackBar);
			this.Controls.Add(this.minTrackBar);
			this.Controls.Add(this.maxTextBox);
			this.Controls.Add(this.minTextBox);
			this.Controls.Add(this.maxLabel);
			this.Controls.Add(this.minLabel);
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "RangeDropDownControl";
			this.Size = new System.Drawing.Size(374, 87);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel minLabel;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel maxLabel;
		private EngineTextBox minTextBox;
		private EngineTextBox maxTextBox;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonTrackBar minTrackBar;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonTrackBar maxTrackBar;
	}
}

#endif