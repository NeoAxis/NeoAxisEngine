namespace NeoAxis.Editor
{
	partial class ContentBrowserOptionsForm
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
			this.hierarchicalContainer1 = new NeoAxis.Editor.HierarchicalContainer();
			this.buttonClose = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.SuspendLayout();
			// 
			// hierarchicalContainer1
			// 
			this.hierarchicalContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.hierarchicalContainer1.ContentMode = NeoAxis.Editor.HierarchicalContainer.ContentModeEnum.Properties;
			this.hierarchicalContainer1.DisplayGroups = false;
			this.hierarchicalContainer1.Location = new System.Drawing.Point(0, 14);
			this.hierarchicalContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.hierarchicalContainer1.Name = "hierarchicalContainer1";
			this.hierarchicalContainer1.Size = new System.Drawing.Size(550, 407);
			this.hierarchicalContainer1.SplitterPosition = 220;
			this.hierarchicalContainer1.SplitterRatio = 0.4F;
			this.hierarchicalContainer1.TabIndex = 0;
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new System.Drawing.Point(433, 431);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(117, 32);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Values.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// ContentBrowserOptionsForm
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(562, 475);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.hierarchicalContainer1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ContentBrowserOptionsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Content Browser Options";
			this.Load += new System.EventHandler(this.ContentBrowserOptionsForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private HierarchicalContainer hierarchicalContainer1;
		private ComponentFactory.Krypton.Toolkit.KryptonButton buttonClose;
	}
}