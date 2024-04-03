#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class MultilineTextDropDownControl
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
			this.engineTextBox = new NeoAxis.Editor.EngineTextBox();
			this.SuspendLayout();
			// 
			// engineTextBox
			// 
			//this.engineTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			//         | System.Windows.Forms.AnchorStyles.Left) 
			//         | System.Windows.Forms.AnchorStyles.Right)));
			this.engineTextBox.Location = new System.Drawing.Point(5, 4);
			this.engineTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.engineTextBox.Name = "engineTextBox";
			this.engineTextBox.Size = new System.Drawing.Size(615, 206);
			this.engineTextBox.TabIndex = 2;
			this.engineTextBox.AutoSize = false;
			this.engineTextBox.Multiline = true;
			this.engineTextBox.AcceptsReturn = true;
			this.engineTextBox.AcceptsTab = true;
			// 
			// MultilineTextDropDownControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.engineTextBox);
			this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
			this.Name = "MultilineTextDropDownControl";
			this.Size = new System.Drawing.Size(624, 260);
			this.ResumeLayout(false);

		}

		#endregion
		private EngineTextBox engineTextBox;
	}
}

#endif