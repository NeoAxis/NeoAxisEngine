#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class DocumentWindowWithViewport : IDocumentWindowWithViewport
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
			this.viewportControl = new NeoAxis.Editor.EngineViewportControl();
			this.SuspendLayout();
			// 
			// viewportControl
			// 
			this.viewportControl.AllowCreateRenderWindow = true;
			this.viewportControl.AutomaticUpdateFPS = 200F;
			this.viewportControl.BackColor = System.Drawing.Color.Transparent;
			this.viewportControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewportControl.Location = new System.Drawing.Point(0, 0);
			this.viewportControl.Margin = new System.Windows.Forms.Padding(0);
			this.viewportControl.Name = "viewportControl";
			this.viewportControl.OneFrameChangeCursor = null;
			this.viewportControl.Size = new System.Drawing.Size(354, 171);
			this.viewportControl.TabIndex = 0;
			// 
			// DocumentWindowWithViewport
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.viewportControl);
			this.Name = "DocumentWindowWithViewport";
			this.ResumeLayout(false);

		}

		#endregion

		private NeoAxis.Editor.EngineViewportControl viewportControl;
	}
}

#endif