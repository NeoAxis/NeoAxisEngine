namespace NeoAxis.Editor
{
	partial class SetReferenceWindow
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
			this.components = new System.ComponentModel.Container();
			this.buttonSet = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.buttonClose = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.contentBrowser1 = new NeoAxis.Editor.ContentBrowser();
			this.buttonSetAndClose = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.labelSelectedReference = new NeoAxis.Editor.LabelEx();
			this.kryptonTextBox1 = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
			this.kryptonCheckBoxCanMakeRelativeFilePath = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
			this.SuspendLayout();
			// 
			// buttonSet
			// 
			this.buttonSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSet.Location = new System.Drawing.Point(218, 527);
			this.buttonSet.Name = "buttonSet";
			this.buttonSet.Size = new System.Drawing.Size(147, 32);
			this.buttonSet.TabIndex = 3;
			this.buttonSet.Values.Text = "Set";
			this.buttonSet.Click += new System.EventHandler(this.ButtonSet_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Location = new System.Drawing.Point(524, 527);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(147, 32);
			this.buttonClose.TabIndex = 5;
			this.buttonClose.Values.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
			// 
			// contentBrowser1
			// 
			this.contentBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.contentBrowser1.CanSelectObjectSettings = false;
			this.contentBrowser1.FilteringMode = null;
			this.contentBrowser1.Location = new System.Drawing.Point(12, 12);
			this.contentBrowser1.Margin = new System.Windows.Forms.Padding(4);
			this.contentBrowser1.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.SetReference;
			this.contentBrowser1.Name = "contentBrowser1";
			this.contentBrowser1.ReadOnlyHierarchy = false;
			this.contentBrowser1.Size = new System.Drawing.Size(658, 452);
			this.contentBrowser1.TabIndex = 0;
			this.contentBrowser1.ThisIsSettingsWindow = false;
			this.contentBrowser1.ItemAfterSelect += new NeoAxis.Editor.ContentBrowser.ItemAfterSelectDelegate(this.contentBrowser1_ItemAfterSelect);
			this.contentBrowser1.ItemAfterChoose += new NeoAxis.Editor.ContentBrowser.ItemAfterChooseDelegate(this.contentBrowser1_ItemAfterChoose);
			// 
			// buttonSetAndClose
			// 
			this.buttonSetAndClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSetAndClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonSetAndClose.Location = new System.Drawing.Point(371, 527);
			this.buttonSetAndClose.Name = "buttonSetAndClose";
			this.buttonSetAndClose.Size = new System.Drawing.Size(147, 32);
			this.buttonSetAndClose.TabIndex = 4;
			this.buttonSetAndClose.Values.Text = "Set and Close";
			this.buttonSetAndClose.Click += new System.EventHandler(this.ButtonSetAndClose_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 50;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// labelSelectedReference
			// 
			this.labelSelectedReference.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSelectedReference.Enabled = false;
			this.labelSelectedReference.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
			this.labelSelectedReference.Location = new System.Drawing.Point(31, 480);
			this.labelSelectedReference.Name = "labelSelectedReference";
			this.labelSelectedReference.Size = new System.Drawing.Size(651, 23);
			this.labelSelectedReference.TabIndex = 4;
			this.labelSelectedReference.Text = "(Selected reference)";
			this.labelSelectedReference.Visible = false;
			// 
			// kryptonTextBox1
			// 
			this.kryptonTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonTextBox1.Location = new System.Drawing.Point(12, 497);
			this.kryptonTextBox1.Name = "kryptonTextBox1";
			this.kryptonTextBox1.Size = new System.Drawing.Size(659, 21);
			this.kryptonTextBox1.TabIndex = 2;
			this.kryptonTextBox1.TextChanged += new System.EventHandler(this.kryptonTextBox1_TextChanged);
			// 
			// kryptonCheckBoxCanMakeRelativeFilePath
			// 
			this.kryptonCheckBoxCanMakeRelativeFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.kryptonCheckBoxCanMakeRelativeFilePath.Location = new System.Drawing.Point(12, 471);
			this.kryptonCheckBoxCanMakeRelativeFilePath.Name = "kryptonCheckBoxCanMakeRelativeFilePath";
			this.kryptonCheckBoxCanMakeRelativeFilePath.Size = new System.Drawing.Size(198, 20);
			this.kryptonCheckBoxCanMakeRelativeFilePath.TabIndex = 1;
			this.kryptonCheckBoxCanMakeRelativeFilePath.Values.Text = "Can make relative file path";
			// 
			// SetReferenceWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CloseByEscape = true;
			this.Controls.Add(this.kryptonCheckBoxCanMakeRelativeFilePath);
			this.Controls.Add(this.kryptonTextBox1);
			this.Controls.Add(this.labelSelectedReference);
			this.Controls.Add(this.contentBrowser1);
			this.Controls.Add(this.buttonSetAndClose);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.buttonSet);
			this.Name = "SetReferenceWindow";
			this.Size = new System.Drawing.Size(685, 572);
			this.WindowTitle = "Set Reference";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ComponentFactory.Krypton.Toolkit.KryptonButton buttonSet;
		private ComponentFactory.Krypton.Toolkit.KryptonButton buttonClose;
		private ContentBrowser contentBrowser1;
		private ComponentFactory.Krypton.Toolkit.KryptonButton buttonSetAndClose;
		private System.Windows.Forms.Timer timer1;
		private NeoAxis.Editor.LabelEx labelSelectedReference;
		private ComponentFactory.Krypton.Toolkit.KryptonTextBox kryptonTextBox1;
		private ComponentFactory.Krypton.Toolkit.KryptonCheckBox kryptonCheckBoxCanMakeRelativeFilePath;
	}
}