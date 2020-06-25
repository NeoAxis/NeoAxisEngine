namespace SampleWidgetWinForms
{
	partial class AdditionalForm
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
			this.widgetControl1 = new NeoAxis.Widget.WidgetControlWinForms();
			this.buttonClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// widgetControl1
			// 
			this.widgetControl1.AllowCreateRenderWindow = true;
			this.widgetControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.widgetControl1.AutomaticUpdateFPS = 60F;
			this.widgetControl1.BackColor = System.Drawing.Color.Black;
			this.widgetControl1.Location = new System.Drawing.Point(14, 14);
			this.widgetControl1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
			this.widgetControl1.Name = "widgetControl1";
			this.widgetControl1.OneFrameChangeCursor = null;
			this.widgetControl1.Size = new System.Drawing.Size(675, 499);
			this.widgetControl1.TabIndex = 0;
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.Location = new System.Drawing.Point(698, 14);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(117, 32);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// AdditionalForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(827, 527);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.widgetControl1);
			this.Name = "AdditionalForm";
			this.Text = "AdditionalForm";
			this.ResumeLayout(false);

		}

		#endregion

		private NeoAxis.Widget.WidgetControlWinForms widgetControl1;
		private System.Windows.Forms.Button buttonClose;
	}
}