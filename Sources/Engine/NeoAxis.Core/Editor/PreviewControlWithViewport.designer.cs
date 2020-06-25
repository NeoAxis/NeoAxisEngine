namespace NeoAxis.Editor
{
	partial class PreviewControlWithViewport
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.viewportControl = new NeoAxis.Widget.EngineViewportControl();
			this.SuspendLayout();
			// 
			// viewportControl
			// 
			this.viewportControl.AllowCreateRenderWindow = true;
			this.viewportControl.AutomaticUpdateFPS = 60F;
			this.viewportControl.BackColor = System.Drawing.Color.Transparent;
			this.viewportControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewportControl.Location = new System.Drawing.Point(0, 0);
			this.viewportControl.Margin = new System.Windows.Forms.Padding(0);
			this.viewportControl.Name = "viewportControl";
			this.viewportControl.Size = new System.Drawing.Size(785, 518);
			this.viewportControl.TabIndex = 0;

			this.Controls.Add( this.viewportControl );

			// 
			// DocumentItem
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "DocumentItem";
			this.ResumeLayout(false);

		}

		#endregion

		private NeoAxis.Widget.EngineViewportControl viewportControl;
	}
}
