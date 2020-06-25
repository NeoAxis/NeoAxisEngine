using Aga.Controls.Tree;

namespace NeoAxis.Editor
{
	partial class SolutionExplorer
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
			this.resourcesBrowser1 = new NeoAxis.Editor.ContentBrowser();
			this.SuspendLayout();
			// 
			// resourcesBrowser1
			// 
			this.resourcesBrowser1.CanSelectObjectSettings = false;
			this.resourcesBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.resourcesBrowser1.FilteringMode = null;
			this.resourcesBrowser1.Location = new System.Drawing.Point(0, 0);
			this.resourcesBrowser1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.resourcesBrowser1.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.resourcesBrowser1.MultiSelect = true;
			this.resourcesBrowser1.Name = "resourcesBrowser1";
			this.resourcesBrowser1.ReadOnlyHierarchy = false;
			this.resourcesBrowser1.Size = new System.Drawing.Size(416, 596);
			this.resourcesBrowser1.TabIndex = 0;
			this.resourcesBrowser1.ThisIsSettingsWindow = false;
			this.resourcesBrowser1.TreeViewBorderDraw = NeoAxis.Editor.BorderSides.Top;
			this.resourcesBrowser1.ListViewBorderDraw = NeoAxis.Editor.BorderSides.Top;
			// 
			// SolutionExplorer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.Controls.Add(this.resourcesBrowser1);
			this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.Name = "SolutionExplorer";
			this.Size = new System.Drawing.Size(416, 596);
			this.WindowTitle = "Solution Explorer";
			this.ResumeLayout(false);

		}

		#endregion

		private ContentBrowser resourcesBrowser1;
	}
}
