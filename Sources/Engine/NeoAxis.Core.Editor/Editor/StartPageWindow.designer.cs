#if !DEPLOY
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
			this.kryptonLabelSelectTheme = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.kryptonLabelOpenScene = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.kryptonButtonLightTheme = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonDarkTheme = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.contentBrowserOpenScene = new NeoAxis.Editor.ContentBrowser();
			this.kryptonButtonOpenScene = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonOpenStore = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonLabelEditorConfiguration = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.kryptonCheckBoxMinimizeRibbon = new Internal.ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
			this.kryptonCheckBoxShowQATBelowRibbon = new Internal.ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
			this.panelNewResource = new System.Windows.Forms.Panel();
			this.contentBrowserNewScene = new NeoAxis.Editor.ContentBrowser();
			this.buttonCreateScene = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonLabelNewResource = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.panelStoreItems = new System.Windows.Forms.Panel();
			this.contentBrowserStoreItems = new NeoAxis.Editor.ContentBrowser();
			this.kryptonButtonInstallStoreItem = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonLabelStoreItems = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.panelNewResource.SuspendLayout();
			this.panelStoreItems.SuspendLayout();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// kryptonLabelSelectTheme
			// 
			this.kryptonLabelSelectTheme.Location = new System.Drawing.Point(23, 23);
			this.kryptonLabelSelectTheme.Name = "kryptonLabelSelectTheme";
			this.kryptonLabelSelectTheme.Size = new System.Drawing.Size(95, 19);
			this.kryptonLabelSelectTheme.TabIndex = 0;
			this.kryptonLabelSelectTheme.Values.Text = "Select theme:";
			// 
			// kryptonLabelOpenScene
			// 
			this.kryptonLabelOpenScene.Location = new System.Drawing.Point(23, 530);
			this.kryptonLabelOpenScene.Name = "kryptonLabelOpenScene";
			this.kryptonLabelOpenScene.Size = new System.Drawing.Size(90, 19);
			this.kryptonLabelOpenScene.TabIndex = 0;
			this.kryptonLabelOpenScene.Values.Text = "Open scene:";
			// 
			// kryptonButtonLightTheme
			// 
			this.kryptonButtonLightTheme.Location = new System.Drawing.Point(26, 48);
			this.kryptonButtonLightTheme.Margin = new System.Windows.Forms.Padding(4);
			this.kryptonButtonLightTheme.Name = "kryptonButtonLightTheme";
			this.kryptonButtonLightTheme.Size = new System.Drawing.Size(140, 102);
			this.kryptonButtonLightTheme.StateCommon.Back.Image = global::NeoAxis.Editor.Properties.Resources.LightThemePreview;
			this.kryptonButtonLightTheme.TabIndex = 0;
			this.kryptonButtonLightTheme.Values.Text = "";
			this.kryptonButtonLightTheme.Click += new System.EventHandler(this.kryptonButtonLightTheme_Click);
			// 
			// kryptonButtonDarkTheme
			// 
			this.kryptonButtonDarkTheme.Location = new System.Drawing.Point(174, 48);
			this.kryptonButtonDarkTheme.Margin = new System.Windows.Forms.Padding(4);
			this.kryptonButtonDarkTheme.Name = "kryptonButtonDarkTheme";
			this.kryptonButtonDarkTheme.Size = new System.Drawing.Size(140, 102);
			this.kryptonButtonDarkTheme.StateCommon.Back.Image = global::NeoAxis.Editor.Properties.Resources.DarkThemePreview;
			this.kryptonButtonDarkTheme.TabIndex = 1;
			this.kryptonButtonDarkTheme.Values.Text = "";
			this.kryptonButtonDarkTheme.Click += new System.EventHandler(this.kryptonButtonDarkTheme_Click);
			// 
			// contentBrowserOpenScene
			// 
			this.contentBrowserOpenScene.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.contentBrowserOpenScene.FilteringMode = null;
			this.contentBrowserOpenScene.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.contentBrowserOpenScene.ListViewModeOverride = null;
			this.contentBrowserOpenScene.Location = new System.Drawing.Point(26, 554);
			this.contentBrowserOpenScene.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.contentBrowserOpenScene.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowserOpenScene.Name = "contentBrowserOpenScene";
			this.contentBrowserOpenScene.ReadOnlyHierarchy = false;
			this.contentBrowserOpenScene.ShowToolBar = false;
			this.contentBrowserOpenScene.Size = new System.Drawing.Size(904, 175);
			this.contentBrowserOpenScene.TabIndex = 8;
			this.contentBrowserOpenScene.ThisIsSettingsWindow = false;
			// 
			// kryptonButtonOpenScene
			// 
			this.kryptonButtonOpenScene.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.kryptonButtonOpenScene.Location = new System.Drawing.Point(26, 735);
			this.kryptonButtonOpenScene.Margin = new System.Windows.Forms.Padding(4);
			this.kryptonButtonOpenScene.Name = "kryptonButtonOpenScene";
			this.kryptonButtonOpenScene.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonOpenScene.TabIndex = 9;
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
			this.kryptonButtonOpenStore.TabIndex = 10;
			this.kryptonButtonOpenStore.Values.Text = "Get Content";
			this.kryptonButtonOpenStore.Visible = false;
			this.kryptonButtonOpenStore.Click += new System.EventHandler(this.kryptonButtonOpenStore_Click);
			// 
			// kryptonLabelEditorConfiguration
			// 
			this.kryptonLabelEditorConfiguration.Location = new System.Drawing.Point(349, 23);
			this.kryptonLabelEditorConfiguration.Name = "kryptonLabelEditorConfiguration";
			this.kryptonLabelEditorConfiguration.Size = new System.Drawing.Size(131, 19);
			this.kryptonLabelEditorConfiguration.TabIndex = 7;
			this.kryptonLabelEditorConfiguration.Values.Text = "Editor configuration:";
			// 
			// kryptonCheckBoxMinimizeRibbon
			// 
			this.kryptonCheckBoxMinimizeRibbon.Location = new System.Drawing.Point(355, 50);
			this.kryptonCheckBoxMinimizeRibbon.Name = "kryptonCheckBoxMinimizeRibbon";
			this.kryptonCheckBoxMinimizeRibbon.Size = new System.Drawing.Size(150, 19);
			this.kryptonCheckBoxMinimizeRibbon.TabIndex = 2;
			this.kryptonCheckBoxMinimizeRibbon.Values.Text = "Minimize the Ribbon";
			this.kryptonCheckBoxMinimizeRibbon.CheckedChanged += new System.EventHandler(this.kryptonCheckBoxMinimizeRibbon_CheckedChanged);
			// 
			// kryptonCheckBoxShowQATBelowRibbon
			// 
			this.kryptonCheckBoxShowQATBelowRibbon.Location = new System.Drawing.Point(355, 76);
			this.kryptonCheckBoxShowQATBelowRibbon.Name = "kryptonCheckBoxShowQATBelowRibbon";
			this.kryptonCheckBoxShowQATBelowRibbon.Size = new System.Drawing.Size(306, 19);
			this.kryptonCheckBoxShowQATBelowRibbon.TabIndex = 3;
			this.kryptonCheckBoxShowQATBelowRibbon.Values.Text = "Show Quick Access Toolbar below the Ribbon";
			this.kryptonCheckBoxShowQATBelowRibbon.CheckedChanged += new System.EventHandler(this.kryptonCheckBoxShowQATBelowRibbon_CheckedChanged);
			// 
			// panelNewResource
			// 
			this.panelNewResource.Controls.Add(this.contentBrowserNewScene);
			this.panelNewResource.Controls.Add(this.buttonCreateScene);
			this.panelNewResource.Controls.Add(this.kryptonLabelNewResource);
			this.panelNewResource.Location = new System.Drawing.Point(26, 172);
			this.panelNewResource.Name = "panelNewResource";
			this.panelNewResource.Size = new System.Drawing.Size(404, 336);
			this.panelNewResource.TabIndex = 9;
			// 
			// contentBrowserNewScene
			// 
			this.contentBrowserNewScene.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.contentBrowserNewScene.FilteringMode = null;
			this.contentBrowserNewScene.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.contentBrowserNewScene.ListViewModeOverride = null;
			this.contentBrowserNewScene.Location = new System.Drawing.Point(0, 24);
			this.contentBrowserNewScene.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.contentBrowserNewScene.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowserNewScene.Name = "contentBrowserNewScene";
			this.contentBrowserNewScene.ReadOnlyHierarchy = false;
			this.contentBrowserNewScene.ShowToolBar = false;
			this.contentBrowserNewScene.Size = new System.Drawing.Size(404, 274);
			this.contentBrowserNewScene.TabIndex = 4;
			this.contentBrowserNewScene.ThisIsSettingsWindow = false;
			// 
			// buttonCreateScene
			// 
			this.buttonCreateScene.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCreateScene.Location = new System.Drawing.Point(0, 304);
			this.buttonCreateScene.Margin = new System.Windows.Forms.Padding(4);
			this.buttonCreateScene.Name = "buttonCreateScene";
			this.buttonCreateScene.Size = new System.Drawing.Size(117, 32);
			this.buttonCreateScene.TabIndex = 5;
			this.buttonCreateScene.Values.Text = "Create";
			// 
			// kryptonLabelNewResource
			// 
			this.kryptonLabelNewResource.Location = new System.Drawing.Point(-2, 0);
			this.kryptonLabelNewResource.Name = "kryptonLabelNewResource";
			this.kryptonLabelNewResource.Size = new System.Drawing.Size(100, 19);
			this.kryptonLabelNewResource.TabIndex = 1;
			this.kryptonLabelNewResource.Values.Text = "New resource:";
			// 
			// panelStoreItems
			// 
			this.panelStoreItems.Controls.Add(this.contentBrowserStoreItems);
			this.panelStoreItems.Controls.Add(this.kryptonButtonInstallStoreItem);
			this.panelStoreItems.Controls.Add(this.kryptonLabelStoreItems);
			this.panelStoreItems.Location = new System.Drawing.Point(436, 172);
			this.panelStoreItems.Name = "panelStoreItems";
			this.panelStoreItems.Size = new System.Drawing.Size(491, 336);
			this.panelStoreItems.TabIndex = 10;
			// 
			// contentBrowserStoreItems
			// 
			this.contentBrowserStoreItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.contentBrowserStoreItems.FilteringMode = null;
			this.contentBrowserStoreItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.contentBrowserStoreItems.ListViewModeOverride = null;
			this.contentBrowserStoreItems.Location = new System.Drawing.Point(0, 24);
			this.contentBrowserStoreItems.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.contentBrowserStoreItems.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowserStoreItems.Name = "contentBrowserStoreItems";
			this.contentBrowserStoreItems.ReadOnlyHierarchy = false;
			this.contentBrowserStoreItems.ShowToolBar = false;
			this.contentBrowserStoreItems.Size = new System.Drawing.Size(491, 274);
			this.contentBrowserStoreItems.TabIndex = 6;
			this.contentBrowserStoreItems.ThisIsSettingsWindow = false;
			// 
			// kryptonButtonInstallStoreItem
			// 
			this.kryptonButtonInstallStoreItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.kryptonButtonInstallStoreItem.Location = new System.Drawing.Point(0, 304);
			this.kryptonButtonInstallStoreItem.Margin = new System.Windows.Forms.Padding(4);
			this.kryptonButtonInstallStoreItem.Name = "kryptonButtonInstallStoreItem";
			this.kryptonButtonInstallStoreItem.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonInstallStoreItem.TabIndex = 7;
			this.kryptonButtonInstallStoreItem.Values.Text = "Learn More";
			// 
			// kryptonLabelStoreItems
			// 
			this.kryptonLabelStoreItems.Location = new System.Drawing.Point(-2, 0);
			this.kryptonLabelStoreItems.Name = "kryptonLabelStoreItems";
			this.kryptonLabelStoreItems.Size = new System.Drawing.Size(139, 19);
			this.kryptonLabelStoreItems.TabIndex = 9;
			this.kryptonLabelStoreItems.Values.Text = "Featured store items:";
			// 
			// StartPageWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelStoreItems);
			this.Controls.Add(this.panelNewResource);
			this.Controls.Add(this.kryptonCheckBoxShowQATBelowRibbon);
			this.Controls.Add(this.kryptonCheckBoxMinimizeRibbon);
			this.Controls.Add(this.kryptonLabelEditorConfiguration);
			this.Controls.Add(this.kryptonButtonOpenStore);
			this.Controls.Add(this.kryptonButtonOpenScene);
			this.Controls.Add(this.contentBrowserOpenScene);
			this.Controls.Add(this.kryptonButtonDarkTheme);
			this.Controls.Add(this.kryptonButtonLightTheme);
			this.Controls.Add(this.kryptonLabelOpenScene);
			this.Controls.Add(this.kryptonLabelSelectTheme);
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "StartPageWindow";
			this.Size = new System.Drawing.Size(957, 789);
			this.WindowTitle = "Start Page";
			this.Load += new System.EventHandler(this.StartPageWindow_Load);
			this.panelNewResource.ResumeLayout(false);
			this.panelNewResource.PerformLayout();
			this.panelStoreItems.ResumeLayout(false);
			this.panelStoreItems.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion
		private System.Windows.Forms.Timer timer1;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabelSelectTheme;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabelOpenScene;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonLightTheme;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonDarkTheme;
		private ContentBrowser contentBrowserOpenScene;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonOpenScene;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonOpenStore;
		//private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonDonate;
		//private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonSubscribeToPro;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabelEditorConfiguration;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonCheckBox kryptonCheckBoxMinimizeRibbon;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonCheckBox kryptonCheckBoxShowQATBelowRibbon;
		private System.Windows.Forms.Panel panelNewResource;
		private ContentBrowser contentBrowserNewScene;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonCreateScene;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabelNewResource;
		private System.Windows.Forms.Panel panelStoreItems;
		private ContentBrowser contentBrowserStoreItems;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonInstallStoreItem;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabelStoreItems;
	}
}

#endif