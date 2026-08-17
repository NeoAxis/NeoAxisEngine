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
			buttonClose = new System.Windows.Forms.Button();
			widgetControl1 = new NeoAxis.Widget.WidgetControlWinForms();
			buttonNewForm = new System.Windows.Forms.Button();
			SuspendLayout();
			// 
			// buttonClose
			// 
			buttonClose.Anchor =  System.Windows.Forms.AnchorStyles.Top  |  System.Windows.Forms.AnchorStyles.Right ;
			buttonClose.Location = new System.Drawing.Point( 1021, 18 );
			buttonClose.Margin = new System.Windows.Forms.Padding( 3, 4, 3, 4 );
			buttonClose.Name = "buttonClose";
			buttonClose.Size = new System.Drawing.Size( 117, 40 );
			buttonClose.TabIndex = 0;
			buttonClose.Text = "Close";
			buttonClose.UseVisualStyleBackColor = true;
			buttonClose.Click +=  buttonClose_Click ;
			// 
			// widgetControl1
			// 
			widgetControl1.AllowCreateRenderWindow = true;
			widgetControl1.Anchor =    System.Windows.Forms.AnchorStyles.Top  |  System.Windows.Forms.AnchorStyles.Bottom   |  System.Windows.Forms.AnchorStyles.Left   |  System.Windows.Forms.AnchorStyles.Right ;
			widgetControl1.AutomaticUpdateFPS = 60F;
			widgetControl1.BackColor = System.Drawing.Color.Black;
			widgetControl1.DisableRecreationRenderWindow = true;
			widgetControl1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8F );
			widgetControl1.Location = new System.Drawing.Point( 14, 18 );
			widgetControl1.Margin = new System.Windows.Forms.Padding( 5, 6, 5, 6 );
			widgetControl1.Name = "widgetControl1";
			widgetControl1.OneFrameChangeCursor = null;
			widgetControl1.OverrideCameraSettings = null;
			widgetControl1.RenderWindow = null;
			widgetControl1.Size = new System.Drawing.Size( 995, 898 );
			widgetControl1.TabIndex = 1;
			widgetControl1.TransformTool = null;
			widgetControl1.Viewport = null;
			// 
			// buttonNewForm
			// 
			buttonNewForm.Anchor =  System.Windows.Forms.AnchorStyles.Top  |  System.Windows.Forms.AnchorStyles.Right ;
			buttonNewForm.Location = new System.Drawing.Point( 1021, 75 );
			buttonNewForm.Margin = new System.Windows.Forms.Padding( 3, 4, 3, 4 );
			buttonNewForm.Name = "buttonNewForm";
			buttonNewForm.Size = new System.Drawing.Size( 117, 40 );
			buttonNewForm.TabIndex = 2;
			buttonNewForm.Text = "Additional Form";
			buttonNewForm.UseVisualStyleBackColor = true;
			buttonNewForm.Click +=  buttonNewForm_Click ;
			// 
			// SampleForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF( 8F, 20F );
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size( 1150, 932 );
			Controls.Add( buttonNewForm );
			Controls.Add( widgetControl1 );
			Controls.Add( buttonClose );
			Margin = new System.Windows.Forms.Padding( 3, 4, 3, 4 );
			Name = "SampleForm";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "Sample Widget WinForms";
			ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.Button buttonClose;
		private NeoAxis.Widget.WidgetControlWinForms widgetControl1;
		private System.Windows.Forms.Button buttonNewForm;
	}
}

