namespace NeoAxis.Editor
{
    partial class ObjectSettingsWindow
	{
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.objectsBrowser1 = new NeoAxis.Editor.ContentBrowser();
			this.panelSettings = new System.Windows.Forms.Panel();
			this.objectSettingsHeader_ObjectInfo1 = new NeoAxis.Editor.ObjectSettingsHeader_ObjectInfo();
			this.kryptonButtonOK = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonSplitContainer1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.kryptonButtonApply = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
			this.kryptonSplitContainer1.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
			this.kryptonSplitContainer1.Panel2.SuspendLayout();
			this.kryptonSplitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Interval = 10;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// objectsBrowser1
			// 
			this.objectsBrowser1.CanSelectObjectSettings = false;
			this.objectsBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.objectsBrowser1.FilteringMode = null;
			this.objectsBrowser1.ListViewItemRendererOverride = null;
			this.objectsBrowser1.Location = new System.Drawing.Point(0, 0);
			this.objectsBrowser1.Margin = new System.Windows.Forms.Padding(4);
			this.objectsBrowser1.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Objects;
			this.objectsBrowser1.MultiSelect = true;
			this.objectsBrowser1.Name = "objectsBrowser1";
			this.objectsBrowser1.ReadOnlyHierarchy = false;
			this.objectsBrowser1.Size = new System.Drawing.Size(292, 545);
			this.objectsBrowser1.TabIndex = 0;
			this.objectsBrowser1.ThisIsSettingsWindow = false;
			this.objectsBrowser1.ItemAfterSelect += new NeoAxis.Editor.ContentBrowser.ItemAfterSelectDelegate(this.objectsBrowser1_ItemAfterSelect);
			this.objectsBrowser1.ItemAfterChoose += new NeoAxis.Editor.ContentBrowser.ItemAfterChooseDelegate(this.objectsBrowser1_ItemAfterChoose);
			// 
			// panelSettings
			// 
			this.panelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelSettings.Location = new System.Drawing.Point(0, 0);
			this.panelSettings.Name = "panelSettings";
			this.panelSettings.Size = new System.Drawing.Size(569, 545);
			this.panelSettings.Padding = new System.Windows.Forms.Padding( 8, 0, 0, 0 );
			this.panelSettings.TabIndex = 0;
			// 
			// objectSettingsHeader_ObjectInfo1
			// 
			//this.objectSettingsHeader_ObjectInfo1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
   //         | System.Windows.Forms.AnchorStyles.Right)));
			this.objectSettingsHeader_ObjectInfo1.Location = new System.Drawing.Point(9, 5);
			this.objectSettingsHeader_ObjectInfo1.Margin = new System.Windows.Forms.Padding(0);
			this.objectSettingsHeader_ObjectInfo1.Name = "objectSettingsHeader_ObjectInfo1";
			this.objectSettingsHeader_ObjectInfo1.Size = new System.Drawing.Size(880, 26);
			this.objectSettingsHeader_ObjectInfo1.TabIndex = 0;
			// 
			// kryptonButtonOK
			// 
			this.kryptonButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonOK.Location = new System.Drawing.Point(740, 589);
			this.kryptonButtonOK.Name = "kryptonButtonOK";
			this.kryptonButtonOK.Size = new System.Drawing.Size(147, 32);
			this.kryptonButtonOK.TabIndex = 2;
			this.kryptonButtonOK.Values.Text = "Close";
			this.kryptonButtonOK.Click += new System.EventHandler(this.kryptonButtonOK_Click);
			// 
			// kryptonSplitContainer1
			// 
			this.kryptonSplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
			this.kryptonSplitContainer1.Location = new System.Drawing.Point(12, 33);
			this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
			// 
			// kryptonSplitContainer1.Panel1
			// 
			this.kryptonSplitContainer1.Panel1.Controls.Add(this.objectsBrowser1);
			this.kryptonSplitContainer1.Panel1.StateCommon.Color1 = System.Drawing.SystemColors.Control;
			this.kryptonSplitContainer1.Panel1.StateCommon.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
			// 
			// kryptonSplitContainer1.Panel2
			// 
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.panelSettings);
			this.kryptonSplitContainer1.Panel2.StateCommon.Color1 = System.Drawing.SystemColors.Control;
			this.kryptonSplitContainer1.Panel2.StateCommon.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
			this.kryptonSplitContainer1.Size = new System.Drawing.Size(876, 545);
			this.kryptonSplitContainer1.SplitterDistance = 292;
			this.kryptonSplitContainer1.SplitterPercent = 0.33333333333333331D;
			this.kryptonSplitContainer1.SplitterWidth = 8;
			this.kryptonSplitContainer1.StateNormal.Back.Color1 = System.Drawing.SystemColors.Control;
			this.kryptonSplitContainer1.StateNormal.Back.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
			this.kryptonSplitContainer1.TabIndex = 2;
			// 
			// kryptonButtonApply
			// 
			this.kryptonButtonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonApply.Location = new System.Drawing.Point(584, 589);
			this.kryptonButtonApply.Name = "kryptonButtonApply";
			this.kryptonButtonApply.Size = new System.Drawing.Size(147, 32);
			this.kryptonButtonApply.TabIndex = 1;
			this.kryptonButtonApply.Values.Text = "Apply";
			this.kryptonButtonApply.Click += new System.EventHandler(this.kryptonButtonApply_Click);
			// 
			// ObjectSettingsWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.kryptonButtonApply);
			this.Controls.Add(this.kryptonSplitContainer1);
			this.Controls.Add(this.objectSettingsHeader_ObjectInfo1);
			this.Controls.Add(this.kryptonButtonOK);
			this.Name = "ObjectSettingsWindow";
			this.Size = new System.Drawing.Size(900, 632);
			this.WindowTitle = "Object Settings";
			this.Load += new System.EventHandler(this.ObjectSettingsWindow_Load);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
			this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
			this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
			this.kryptonSplitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion
		private System.Windows.Forms.Timer timer1;
		private ContentBrowser objectsBrowser1;
		private ObjectSettingsHeader_ObjectInfo objectSettingsHeader_ObjectInfo1;
		private System.Windows.Forms.Panel panelSettings;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonOK;
		private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonApply;
	}
}
