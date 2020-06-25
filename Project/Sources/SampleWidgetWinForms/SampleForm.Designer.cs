namespace SampleWidgetWinForms
{
	partial class SampleForm
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
			this.buttonClose = new System.Windows.Forms.Button();
			this.widgetControl1 = new NeoAxis.Widget.WidgetControlWinForms();
			this.buttonNewForm = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.Location = new System.Drawing.Point(1021, 14);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(117, 32);
			this.buttonClose.TabIndex = 0;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
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
			this.widgetControl1.Size = new System.Drawing.Size(995, 718);
			this.widgetControl1.TabIndex = 1;
			// 
			// buttonNewForm
			// 
			this.buttonNewForm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonNewForm.Location = new System.Drawing.Point(1021, 60);
			this.buttonNewForm.Name = "buttonNewForm";
			this.buttonNewForm.Size = new System.Drawing.Size(117, 32);
			this.buttonNewForm.TabIndex = 2;
			this.buttonNewForm.Text = "Additional Form";
			this.buttonNewForm.UseVisualStyleBackColor = true;
			this.buttonNewForm.Click += new System.EventHandler(this.buttonNewForm_Click);
			// 
			// SampleForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1150, 746);
			this.Controls.Add(this.buttonNewForm);
			this.Controls.Add(this.widgetControl1);
			this.Controls.Add(this.buttonClose);
			this.Name = "SampleForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Sample Widget WinForms";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonClose;
		private NeoAxis.Widget.WidgetControlWinForms widgetControl1;
		private System.Windows.Forms.Button buttonNewForm;
	}
}

