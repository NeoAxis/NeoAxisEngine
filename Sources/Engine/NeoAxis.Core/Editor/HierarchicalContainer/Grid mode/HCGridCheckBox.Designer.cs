namespace NeoAxis.Editor
{
	partial class HCGridCheckBox
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
			this.checkBox1 = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
			this.SuspendLayout();
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(0, 6);
			this.checkBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(58, 20);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Values.Text = "Text";
			// 
			// HCGridCheckBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.checkBox1);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "HCGridCheckBox";
			this.Size = new System.Drawing.Size(516, 28);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public ComponentFactory.Krypton.Toolkit.KryptonCheckBox checkBox1;
	}
}
