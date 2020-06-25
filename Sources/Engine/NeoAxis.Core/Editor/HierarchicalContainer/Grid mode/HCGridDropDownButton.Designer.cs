namespace NeoAxis.Editor
{
	partial class HCGridDropDownButton
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
			this.kryptonDropButton = new ComponentFactory.Krypton.Toolkit.KryptonDropButton();
			this.SuspendLayout();
			// 
			// kryptonDropButton
			// 
			this.kryptonDropButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonDropButton.ButtonStyle = ComponentFactory.Krypton.Toolkit.ButtonStyle.Custom3;
			this.kryptonDropButton.Images.Common = global::NeoAxis.Properties.Resources.DropDownButton;
			this.kryptonDropButton.Location = new System.Drawing.Point(0, 3);
			this.kryptonDropButton.Margin = new System.Windows.Forms.Padding(4);
			this.kryptonDropButton.Name = "kryptonDropButton";
			this.kryptonDropButton.Size = new System.Drawing.Size(285, 23);
			this.kryptonDropButton.Splitter = false;
			this.kryptonDropButton.TabIndex = 0;
			this.kryptonDropButton.Values.Text = "kryptonDropButton1";
			// 
			// HCGridDropDownButton
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.kryptonDropButton);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "HCGridDropDownButton";
			this.Size = new System.Drawing.Size(285, 28);
			this.ResumeLayout(false);

		}

		#endregion

		private ComponentFactory.Krypton.Toolkit.KryptonDropButton kryptonDropButton;
	}
}
