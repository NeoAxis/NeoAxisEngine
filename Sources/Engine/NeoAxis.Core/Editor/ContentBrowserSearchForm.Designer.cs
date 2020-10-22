namespace NeoAxis.Editor
{
	partial class ContentBrowserSearchForm
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
			this.buttonClose = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonSearch = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonTextBoxFilterByName = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
			this.labelEx1 = new NeoAxis.Editor.EngineLabel();
			this.SuspendLayout();
			// 
			// buttonClose
			// 
			//this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Location = new System.Drawing.Point(433, 431);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(117, 32);
			this.buttonClose.TabIndex = 2;
			this.buttonClose.Values.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// kryptonButtonSearch
			// 
			//this.kryptonButtonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonSearch.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.kryptonButtonSearch.Location = new System.Drawing.Point(310, 431);
			this.kryptonButtonSearch.Name = "kryptonButtonSearch";
			this.kryptonButtonSearch.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonSearch.TabIndex = 1;
			this.kryptonButtonSearch.Values.Text = "Search";
			this.kryptonButtonSearch.Click += new System.EventHandler(this.kryptonButtonSearch_Click);
			// 
			// kryptonTextBoxFilterByName
			// 
			//this.kryptonTextBoxFilterByName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
   //         | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonTextBoxFilterByName.Location = new System.Drawing.Point(12, 37);
			this.kryptonTextBoxFilterByName.Name = "kryptonTextBoxFilterByName";
			this.kryptonTextBoxFilterByName.Size = new System.Drawing.Size(538, 23);
			this.kryptonTextBoxFilterByName.TabIndex = 0;
			this.kryptonTextBoxFilterByName.Text = "Light*";
			// 
			// labelEx1
			// 
			this.labelEx1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelEx1.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
			this.labelEx1.Location = new System.Drawing.Point(12, 12);
			this.labelEx1.Name = "labelEx1";
			this.labelEx1.Size = new System.Drawing.Size(538, 23);
			this.labelEx1.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
			this.labelEx1.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.labelEx1.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.labelEx1.StateCommon.Border.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
			this.labelEx1.TabIndex = 7;
			this.labelEx1.Text = "Filter by name:";
			// 
			// ContentBrowserSearchForm
			// 
			this.AcceptButton = this.kryptonButtonSearch;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(562, 475);
			this.Controls.Add(this.kryptonTextBoxFilterByName);
			this.Controls.Add(this.labelEx1);
			this.Controls.Add(this.kryptonButtonSearch);
			this.Controls.Add(this.buttonClose);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ContentBrowserSearchForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Search";
			this.Load += new System.EventHandler(this.ContentBrowserOptionsForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private ComponentFactory.Krypton.Toolkit.KryptonButton buttonClose;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonSearch;
		private EngineLabel labelEx1;
		private ComponentFactory.Krypton.Toolkit.KryptonTextBox kryptonTextBoxFilterByName;
	}
}