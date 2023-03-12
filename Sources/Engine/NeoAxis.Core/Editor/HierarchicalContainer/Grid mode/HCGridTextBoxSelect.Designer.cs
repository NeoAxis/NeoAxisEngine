#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class HCGridTextBoxSelect
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
			this.buttonSelect = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.textBox = new NeoAxis.Editor.EngineTextBox();
			this.SuspendLayout();
			// 
			// buttonSelect
			// 
			this.buttonSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSelect.Location = new System.Drawing.Point(259, 3);
			this.buttonSelect.Name = "buttonSelect";
			this.buttonSelect.Size = new System.Drawing.Size(26, 22);
			this.buttonSelect.TabIndex = 1;
			this.buttonSelect.Values.Image = global::NeoAxis.Properties.Resources.DropDownButton;
			this.buttonSelect.Values.Text = "";
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Location = new System.Drawing.Point(0, 3);
			this.textBox.LikeLabel = false;
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(257, 21);
			this.textBox.TabIndex = 0;
			// 
			// HCGridTextBoxSelect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonSelect);
			this.Controls.Add(this.textBox);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "HCGridTextBoxSelect";
			this.Size = new System.Drawing.Size(285, 28);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		public EngineTextBox textBox;
		public Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonSelect;
	}
}

#endif