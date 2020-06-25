namespace NeoAxis.Editor
{
	partial class Component_FlowGraph_DocumentWindow
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
			this.timer50 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// timer50
			// 
			this.timer50.Interval = 50;
			this.timer50.Tick += new System.EventHandler(this.timer50_Tick);
			// 
			// Component_Flowchart_DocumentWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "Component_FlowGraph_DocumentWindow";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Component_FlowGraph_DocumentWindow_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Component_FlowGraph_DocumentWindow_DragEnter);
			this.DragOver += new System.Windows.Forms.DragEventHandler(this.Component_FlowGraph_DocumentWindow_DragOver);
			this.DragLeave += new System.EventHandler(this.Component_FlowGraph_DocumentWindow_DragLeave);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer timer50;
	}
}

