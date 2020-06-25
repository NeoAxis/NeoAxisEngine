namespace NeoAxis.Editor
{
	partial class HCGridLabel
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
			this.label2 = new NeoAxis.Editor.HCKryptonTextBox();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(0, 3);
			this.label2.LookLikeLabel = true;
			this.label2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.label2.Name = "label2";
			this.label2.ReadOnly = true;
			this.label2.Size = new System.Drawing.Size(451, 23);
			this.label2.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.label2.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.label2.TabIndex = 2;
			this.label2.TabStop = false;
			this.label2.Text = "label2";
			// 
			// HCGridLabel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label2);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "HCGridLabel";
			this.Size = new System.Drawing.Size(452, 28);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		public HCKryptonTextBox label2;
	}
}
