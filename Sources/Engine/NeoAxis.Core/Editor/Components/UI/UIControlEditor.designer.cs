#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class UIControlEditor
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
			this.SuspendLayout();
			// 
			// EUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 8F, 16F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding( 4 );
			this.Name = "UIControl_DocumentWindow";
			this.Size = new System.Drawing.Size( 785, 518 );
			this.DragDrop += new System.Windows.Forms.DragEventHandler( this.UIControl_DocumentWindow_DragDrop );
			this.DragEnter += new System.Windows.Forms.DragEventHandler( this.UIControl_DocumentWindow_DragEnter );
			this.DragOver += new System.Windows.Forms.DragEventHandler( this.UIControl_DocumentWindow_DragOver );
			this.DragLeave += new System.EventHandler( this.UIControl_DocumentWindow_DragLeave );
			this.ResumeLayout( false );

		}

		#endregion
	}
}


#endif