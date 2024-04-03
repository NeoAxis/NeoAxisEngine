#if !DEPLOY
namespace NeoAxis.Editor
{
    partial class SelectTypeWindow
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
			this.buttonSelect = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.buttonCancel = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.contentBrowser1 = new NeoAxis.Editor.ContentBrowser();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// buttonSelect
			// 
			this.buttonSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSelect.Location = new System.Drawing.Point(236, 529);
			this.buttonSelect.Name = "buttonSelect";
			this.buttonSelect.Size = new System.Drawing.Size(147, 32);
			this.buttonSelect.TabIndex = 1;
			this.buttonSelect.Values.Text = "Select";
			this.buttonSelect.Click += new System.EventHandler(this.ButtonSelect_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(389, 529);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(147, 32);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Values.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
			// 
			// contentBrowser1
			// 
			this.contentBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.contentBrowser1.CanSelectObjectSettings = false;
			this.contentBrowser1.FilteringMode = null;
			this.contentBrowser1.Location = new System.Drawing.Point(12, 4);
			this.contentBrowser1.Margin = new System.Windows.Forms.Padding(4);
			this.contentBrowser1.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.SetReference;
			this.contentBrowser1.Name = "contentBrowser1";
			this.contentBrowser1.ReadOnlyHierarchy = false;
			this.contentBrowser1.Size = new System.Drawing.Size(523, 514);
			this.contentBrowser1.TabIndex = 0;
			this.contentBrowser1.ThisIsSettingsWindow = false;
			this.contentBrowser1.ItemAfterSelect += new NeoAxis.Editor.ContentBrowser.ItemAfterSelectDelegate(this.contentBrowser1_ItemAfterSelect);
			this.contentBrowser1.ItemAfterChoose += new NeoAxis.Editor.ContentBrowser.ItemAfterChooseDelegate(this.contentBrowser1_ItemAfterChoose);
			// 
			// timer1
			// 
			this.timer1.Interval = 50;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// SelectTypeWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CloseByEscape = true;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonSelect);
			this.Controls.Add(this.contentBrowser1);
			this.Name = "SelectTypeWindow";
			this.Size = new System.Drawing.Size(549, 573);
			this.WindowTitle = "Select Type";
			this.ResumeLayout(false);

        }

        #endregion

        private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonSelect;
        private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonCancel;
		private ContentBrowser contentBrowser1;
		private System.Windows.Forms.Timer timer1;
	}
}
#endif