#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class WorkspaceWindow
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
			this.kryptonDockableWorkspace = new LowProfileDockableWorkspace();
			((System.ComponentModel.ISupportInitialize)(this.kryptonDockableWorkspace)).BeginInit();
			this.SuspendLayout();
			// 
			// kryptonDockableWorkspace
			// 
			this.kryptonDockableWorkspace.AutoHiddenHost = false;
			this.kryptonDockableWorkspace.CompactFlags = ((Internal.ComponentFactory.Krypton.Workspace.CompactFlags)(((Internal.ComponentFactory.Krypton.Workspace.CompactFlags.RemoveEmptyCells | Internal.ComponentFactory.Krypton.Workspace.CompactFlags.RemoveEmptySequences) 
            | Internal.ComponentFactory.Krypton.Workspace.CompactFlags.PromoteLeafs)));
			this.kryptonDockableWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonDockableWorkspace.Location = new System.Drawing.Point(0, 0);
			this.kryptonDockableWorkspace.Name = "kryptonDockableWorkspace";
			// 
			// 
			// 
			this.kryptonDockableWorkspace.Root.UniqueName = "4A87E34386084E14BC81ED10FBED4D99";
			this.kryptonDockableWorkspace.Root.WorkspaceControl = this.kryptonDockableWorkspace;
			this.kryptonDockableWorkspace.ShowMaximizeButton = false;
			this.kryptonDockableWorkspace.Size = new System.Drawing.Size(266, 139);
			this.kryptonDockableWorkspace.TabIndex = 1;
			this.kryptonDockableWorkspace.TabStop = true;
			// 
			// WorkspaceWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.kryptonDockableWorkspace);
			this.Name = "WorkspaceWindow";
			((System.ComponentModel.ISupportInitialize)(this.kryptonDockableWorkspace)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Internal.ComponentFactory.Krypton.Docking.KryptonDockableWorkspace kryptonDockableWorkspace;
	}
}

#endif