namespace NeoAxis.Editor
{
    partial class StartPageWindow
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
			this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.contentBrowserNewScene = new NeoAxis.Editor.ContentBrowser();
			this.buttonCreateScene = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonLightTheme = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonDarkTheme = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.contentBrowserOpenScene = new NeoAxis.Editor.ContentBrowser();
			this.kryptonButtonOpenScene = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonOpenStore = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.toolTip1 = new NeoAxis.Editor.EngineToolTip( this.components);
			this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.kryptonCheckBoxMinimizeRibbon = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
			this.kryptonCheckBoxShowQATBelowRibbon = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// kryptonLabel1
			// 
			this.kryptonLabel1.Location = new System.Drawing.Point(23, 182);
			this.kryptonLabel1.Name = "kryptonLabel1";
			this.kryptonLabel1.Size = new System.Drawing.Size(88, 20);
			this.kryptonLabel1.TabIndex = 0;
			this.kryptonLabel1.Values.Text = "New resource:";
			// 
			// kryptonLabel2
			// 
			this.kryptonLabel2.Location = new System.Drawing.Point(23, 23);
			this.kryptonLabel2.Name = "kryptonLabel2";
			this.kryptonLabel2.Size = new System.Drawing.Size(101, 20);
			this.kryptonLabel2.TabIndex = 0;
			this.kryptonLabel2.Values.Text = "Select theme:";
			// 
			// kryptonLabel3
			// 
			this.kryptonLabel3.Location = new System.Drawing.Point(23, 487);
			this.kryptonLabel3.Name = "kryptonLabel3";
			this.kryptonLabel3.Size = new System.Drawing.Size(96, 20);
			this.kryptonLabel3.TabIndex = 0;
			this.kryptonLabel3.Values.Text = "Open scene:";
			// 
			// contentBrowserNewScene
			// 
			this.contentBrowserNewScene.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.contentBrowserNewScene.CanSelectObjectSettings = false;
			this.contentBrowserNewScene.FilteringMode = null;
			this.contentBrowserNewScene.ListViewModeOverride = null;
			this.contentBrowserNewScene.Location = new System.Drawing.Point(26, 207);
			this.contentBrowserNewScene.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.contentBrowserNewScene.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowserNewScene.Name = "contentBrowserNewScene";
			this.contentBrowserNewScene.ReadOnlyHierarchy = false;
			this.contentBrowserNewScene.ShowToolBar = false;
			this.contentBrowserNewScene.Size = new System.Drawing.Size(901, 213);
			this.contentBrowserNewScene.TabIndex = 5;
			this.contentBrowserNewScene.ThisIsSettingsWindow = false;
			// 
			// buttonCreateScene
			// 
			this.buttonCreateScene.Location = new System.Drawing.Point(26, 426);
			this.buttonCreateScene.Margin = new System.Windows.Forms.Padding(4);
			this.buttonCreateScene.Name = "buttonCreateScene";
			this.buttonCreateScene.Size = new System.Drawing.Size(117, 32);
			this.buttonCreateScene.TabIndex = 3;
			this.buttonCreateScene.Values.Text = "Create";
			this.buttonCreateScene.Click += new System.EventHandler(this.buttonCreateScene_Click);
			// 
			// kryptonButtonLightTheme
			// 
			this.kryptonButtonLightTheme.Location = new System.Drawing.Point(26, 50);
			this.kryptonButtonLightTheme.Margin = new System.Windows.Forms.Padding(4);
			this.kryptonButtonLightTheme.Name = "kryptonButtonLightTheme";
			this.kryptonButtonLightTheme.Size = new System.Drawing.Size(140, 110);
			this.kryptonButtonLightTheme.StateCommon.Back.Image = global::NeoAxis.Properties.Resources.LightThemePreview;
			this.kryptonButtonLightTheme.StateCommon.Back.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
			this.kryptonButtonLightTheme.TabIndex = 0;
			this.toolTip1.SetToolTip(this.kryptonButtonLightTheme, "Set the light theme.");
			this.kryptonButtonLightTheme.Values.Text = "";
			this.kryptonButtonLightTheme.Click += new System.EventHandler(this.kryptonButtonLightTheme_Click);
			// 
			// kryptonButtonDarkTheme
			// 
			this.kryptonButtonDarkTheme.Location = new System.Drawing.Point(174, 50);
			this.kryptonButtonDarkTheme.Margin = new System.Windows.Forms.Padding(4);
			this.kryptonButtonDarkTheme.Name = "kryptonButtonDarkTheme";
			this.kryptonButtonDarkTheme.Size = new System.Drawing.Size(140, 110);
			this.kryptonButtonDarkTheme.StateCommon.Back.Image = global::NeoAxis.Properties.Resources.DarkThemePreview;
			this.kryptonButtonDarkTheme.StateCommon.Back.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.Inherit;
			this.kryptonButtonDarkTheme.TabIndex = 1;
			this.toolTip1.SetToolTip(this.kryptonButtonDarkTheme, "Set the dark theme.");
			this.kryptonButtonDarkTheme.Values.Text = "";
			this.kryptonButtonDarkTheme.Click += new System.EventHandler(this.kryptonButtonDarkTheme_Click);
			// 
			// contentBrowserOpenScene
			// 
			this.contentBrowserOpenScene.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.contentBrowserOpenScene.CanSelectObjectSettings = false;
			this.contentBrowserOpenScene.FilteringMode = null;
			this.contentBrowserOpenScene.ListViewModeOverride = null;
			this.contentBrowserOpenScene.Location = new System.Drawing.Point(26, 512);
			this.contentBrowserOpenScene.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.contentBrowserOpenScene.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowserOpenScene.Name = "contentBrowserOpenScene";
			this.contentBrowserOpenScene.ReadOnlyHierarchy = false;
			this.contentBrowserOpenScene.ShowToolBar = false;
			this.contentBrowserOpenScene.Size = new System.Drawing.Size(901, 217);
			this.contentBrowserOpenScene.TabIndex = 6;
			this.contentBrowserOpenScene.ThisIsSettingsWindow = false;
			// 
			// kryptonButtonOpenScene
			// 
			this.kryptonButtonOpenScene.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.kryptonButtonOpenScene.Location = new System.Drawing.Point(26, 735);
			this.kryptonButtonOpenScene.Margin = new System.Windows.Forms.Padding(4);
			this.kryptonButtonOpenScene.Name = "kryptonButtonOpenScene";
			this.kryptonButtonOpenScene.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonOpenScene.TabIndex = 7;
			this.kryptonButtonOpenScene.Values.Text = "Open Scene";
			this.kryptonButtonOpenScene.Click += new System.EventHandler(this.kryptonButtonOpenScene_Click);
			// 
			// kryptonButtonOpenStore
			// 
			this.kryptonButtonOpenStore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonOpenStore.Location = new System.Drawing.Point(780, 23);
			this.kryptonButtonOpenStore.Margin = new System.Windows.Forms.Padding(4);
			this.kryptonButtonOpenStore.Name = "kryptonButtonOpenStore";
			this.kryptonButtonOpenStore.Size = new System.Drawing.Size(147, 32);
			this.kryptonButtonOpenStore.TabIndex = 4;
			this.kryptonButtonOpenStore.Values.Text = "Get Basic Content";// Open Store";
			this.kryptonButtonOpenStore.Click += new System.EventHandler(this.kryptonButtonOpenStore_Click);
			// 
			// kryptonLabel4
			// 
			this.kryptonLabel4.Location = new System.Drawing.Point(349, 23);
			this.kryptonLabel4.Name = "kryptonLabel4";
			this.kryptonLabel4.Size = new System.Drawing.Size(142, 20);
			this.kryptonLabel4.TabIndex = 7;
			this.kryptonLabel4.Values.Text = "Editor configuration:";
			// 
			// kryptonCheckBoxMinimizeRibbon
			// 
			this.kryptonCheckBoxMinimizeRibbon.Location = new System.Drawing.Point(355, 50);
			this.kryptonCheckBoxMinimizeRibbon.Name = "kryptonCheckBoxMinimizeRibbon";
			this.kryptonCheckBoxMinimizeRibbon.Size = new System.Drawing.Size(158, 20);
			this.kryptonCheckBoxMinimizeRibbon.TabIndex = 2;
			this.kryptonCheckBoxMinimizeRibbon.Values.Text = "Minimize the Ribbon";
			this.kryptonCheckBoxMinimizeRibbon.CheckedChanged += new System.EventHandler(this.kryptonCheckBoxMinimizeRibbon_CheckedChanged);
			// 
			// kryptonCheckBoxShowQATBelowRibbon
			// 
			this.kryptonCheckBoxShowQATBelowRibbon.Location = new System.Drawing.Point(355, 76);
			this.kryptonCheckBoxShowQATBelowRibbon.Name = "kryptonCheckBoxShowQATBelowRibbon";
			this.kryptonCheckBoxShowQATBelowRibbon.Size = new System.Drawing.Size(320, 20);
			this.kryptonCheckBoxShowQATBelowRibbon.TabIndex = 3;
			this.kryptonCheckBoxShowQATBelowRibbon.Values.Text = "Show Quick Access Toolbar below the Ribbon";
			this.kryptonCheckBoxShowQATBelowRibbon.CheckedChanged += new System.EventHandler(this.kryptonCheckBoxShowQATBelowRibbon_CheckedChanged);
			// 
			// StartPageWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.kryptonCheckBoxShowQATBelowRibbon);
			this.Controls.Add(this.kryptonCheckBoxMinimizeRibbon);
			this.Controls.Add(this.kryptonLabel4);
			this.Controls.Add(this.kryptonButtonOpenStore);
			this.Controls.Add(this.kryptonButtonOpenScene);
			this.Controls.Add(this.contentBrowserOpenScene);
			this.Controls.Add(this.kryptonButtonDarkTheme);
			this.Controls.Add(this.kryptonButtonLightTheme);
			this.Controls.Add(this.buttonCreateScene);
			this.Controls.Add(this.contentBrowserNewScene);
			this.Controls.Add(this.kryptonLabel3);
			this.Controls.Add(this.kryptonLabel2);
			this.Controls.Add(this.kryptonLabel1);
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "StartPageWindow";
			this.Size = new System.Drawing.Size(957, 789);
			this.WindowTitle = "Start Page";
			this.Load += new System.EventHandler(this.StartPageWindow_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion
		private System.Windows.Forms.Timer timer1;
		private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
		private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
		private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
		private ContentBrowser contentBrowserNewScene;
		private ComponentFactory.Krypton.Toolkit.KryptonButton buttonCreateScene;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonLightTheme;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonDarkTheme;
		private ContentBrowser contentBrowserOpenScene;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonOpenScene;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonOpenStore;
		private NeoAxis.Editor.EngineToolTip toolTip1;
		private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
		private ComponentFactory.Krypton.Toolkit.KryptonCheckBox kryptonCheckBoxMinimizeRibbon;
		private ComponentFactory.Krypton.Toolkit.KryptonCheckBox kryptonCheckBoxShowQATBelowRibbon;
	}
}
