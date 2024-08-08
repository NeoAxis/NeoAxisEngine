#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Start Page.
	/// </summary>
	[RestoreDockWindowAfterEditorReload]
	public partial class StartPageWindow : DocumentWindow, ControlDoubleBufferComposited.IDoubleBufferComposited
	{
		string[] currentOpenScenes = new string[ 0 ];
		double currentOpenScenesLastUpdate;

		static Image previewImageNewUIControl;
		static Image previewImageNewResource;

		//breaking when it in designer code
		private NeoAxis.Editor.EngineToolTip toolTip1;

		//store items
		//ESet<string> featuredStoreItemIdentifiers = new ESet<string>();
		bool featuredStoreItemsWereUpdated;

		//

		public delegate void UpdateNewScenesDelegate( ref List<ContentBrowser.Item> list );
		public static event UpdateNewScenesDelegate UpdateNewScenes;

		public StartPageWindow()
		{
			InitializeComponent();

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			toolTip1 = new EngineToolTip();
			//!!!!language
			this.toolTip1.SetToolTip( this.kryptonButtonLightTheme, "Set the light theme." );
			this.toolTip1.SetToolTip( this.kryptonButtonDarkTheme, "Set the dark theme." );

			if( EditorAPI2.DarkTheme )
				BackColor = Color.FromArgb( 40, 40, 40 );

			var tileImageSize = 68;// 100;

			//new resource
			{
				contentBrowserNewScene.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
				contentBrowserNewScene.Options.ListMode = ContentBrowser.ListModeEnum.Tiles;
				contentBrowserNewScene.UseSelectedTreeNodeAsRootForList = false;
				contentBrowserNewScene.Options.Breadcrumb = false;
				contentBrowserNewScene.Options.TileImageSize = tileImageSize;

				//add items
				try
				{
					var items = new List<ContentBrowser.Item>();

					//scenes
					foreach( var template in SceneNewObjectSettings.GetTemplates() )
					{
						contentBrowserNewScene.AddImageKey( template.Name, template.Preview );

						var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, template.ToString() + " scene" );
						item.Tag = template;
						item.imageKey = template.Name;

						items.Add( item );
					}

					//UI Control
					{
						var path = VirtualPathUtility.GetRealPathByVirtual( @"Base\Tools\NewResourceTemplates\UIControl.ui" );
						if( File.Exists( path ) )
						{
							var name = Path.GetFileNameWithoutExtension( path );

							if( previewImageNewUIControl == null )
							{
								var previewPath = Path.Combine( Path.GetDirectoryName( path ), name + ".png" );
								previewImageNewUIControl = File.Exists( previewPath ) ? Image.FromFile( previewPath ) : null;
							}

							if( previewImageNewUIControl != null )
								contentBrowserNewScene.AddImageKey( name, previewImageNewUIControl );

							var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, "UI Control" );
							item.Tag = "UIControl";
							if( previewImageNewUIControl != null )
								item.imageKey = name;

							items.Add( item );
						}
					}

					//Select resource
					{
						var name = "SelectResource";

						if( previewImageNewResource == null )
						{
							var previewPath = VirtualPathUtility.GetRealPathByVirtual( @"Base\Tools\NewResourceTemplates\Resource.png" );
							previewImageNewResource = File.Exists( previewPath ) ? Image.FromFile( previewPath ) : null;
						}

						if( previewImageNewResource != null )
							contentBrowserNewScene.AddImageKey( name, previewImageNewResource );

						var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, "Select type" );
						item.Tag = "Resource";
						if( previewImageNewResource != null )
							item.imageKey = name;

						items.Add( item );
					}

					////!!!!
					//{
					//	var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, "C# Class Library" );
					//	//!!!!
					//	//item.Tag = template;
					//	item.imageKey = "";//template.Name;

					//	item.ShowDisabled = true;

					//	items.Add( item );
					//}

					////!!!!
					//{
					//	var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, "Executable App" );
					//	//!!!!
					//	//item.Tag = template;
					//	item.imageKey = "";//template.Name;

					//	item.ShowDisabled = true;

					//	items.Add( item );
					//}

					UpdateNewScenes?.Invoke( ref items );

					if( items.Count != 0 )
					{
						contentBrowserNewScene.SetData( items, false );
						contentBrowserNewScene.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );
					}
				}
				catch( Exception e2 )
				{
					Log.Warning( e2.Message );
				}

				contentBrowserNewScene.ItemAfterChoose += ContentBrowserNewScene_ItemAfterChoose;
				buttonCreateScene.Click += buttonCreateScene_Click;

				ConfigureNewSceneContextMenu();
			}

			//install store items
			{
				//featuredStoreItemIdentifiers.Add( "City_Demo" );
				////featuredStoreItemIdentifiers.Add( "Sci_fi_Demo" );
				//featuredStoreItemIdentifiers.Add( "Nature_Demo" );
				//featuredStoreItemIdentifiers.Add( "Basic_Materials_2K" );
				//featuredStoreItemIdentifiers.Add( "Basic_Environments_4K" );
				//if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.Standalone )
				//	featuredStoreItemIdentifiers.Add( "Platform_Tools" );

				contentBrowserStoreItems.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
				contentBrowserStoreItems.Options.ListMode = ContentBrowser.ListModeEnum.Tiles;
				contentBrowserStoreItems.UseSelectedTreeNodeAsRootForList = false;
				contentBrowserStoreItems.Options.Breadcrumb = false;
				contentBrowserStoreItems.Options.TileImageSize = tileImageSize;

				contentBrowserStoreItems.AddImageKey( "Default_512", Properties.Resources.Default_512 );

				contentBrowserStoreItems.ItemAfterChoose += ContentBrowserStoreItems_ItemAfterChoose;
				kryptonButtonInstallStoreItem.Click += kryptonButtonInstallStoreItem_Click;

				ConfigureStoreItemsContextMenu();
			}

			//open scenes
			{
				contentBrowserOpenScene.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
				contentBrowserOpenScene.Options.ListMode = ContentBrowser.ListModeEnum.List;
				contentBrowserOpenScene.UseSelectedTreeNodeAsRootForList = false;
				contentBrowserOpenScene.Options.Breadcrumb = false;

				var imageSize = EditorAPI2.DPIScale >= 1.25f ? 13 : 16;

				contentBrowserOpenScene.Options.ListImageSize = imageSize;
				contentBrowserOpenScene.Options.ListColumnWidth = 10000;
				contentBrowserOpenScene.ListViewModeOverride = new EngineListView.DefaultListMode( contentBrowserOpenScene.GetListView(), imageSize );

				contentBrowserOpenScene.PreloadResourceOnSelection = false;

				UpdateOpenScenes();
			}

			WindowTitle = Translate( WindowTitle );
		}

		private void StartPageWindow_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			EditorLocalization2.TranslateForm( "StartPageWindow", this );
			EditorLocalization2.TranslateForm( "StartPageWindow", panelNewResource );
			EditorLocalization2.TranslateForm( "StartPageWindow", panelStoreItems );

			timer1.Start();

			UpdateControls();

			if( EditorActions.GetByName( "Store" ) != null && EditorActions.GetByName( "Store" ).CompletelyDisabled )
				kryptonButtonOpenStore.Visible = false;
		}

		void UpdateControls()
		{
			try //was exception on exit
			{
				buttonCreateScene.Enabled = contentBrowserNewScene.SelectedItemsOnlyListView.Length == 1;
				kryptonButtonInstallStoreItem.Enabled = contentBrowserStoreItems.SelectedItemsOnlyListView.Length == 1;
				kryptonButtonOpenScene.Enabled = contentBrowserOpenScene.SelectedItems.Length == 1;

				kryptonButtonLightTheme.Enabled = ProjectSettings.Get.General.Theme.Value != ProjectSettingsPage_General.ThemeEnum.Light;
				kryptonButtonDarkTheme.Enabled = ProjectSettings.Get.General.Theme.Value != ProjectSettingsPage_General.ThemeEnum.Dark;

				kryptonCheckBoxMinimizeRibbon.Checked = EditorForm.Instance.kryptonRibbon.MinimizedMode;
				kryptonCheckBoxShowQATBelowRibbon.Checked = EditorForm.Instance.kryptonRibbon.QATLocation == Internal.ComponentFactory.Krypton.Ribbon.QATLocation.Below;
			}
			catch { }
		}

		string Translate( string text )
		{
			return EditorLocalization2.Translate( "StartPageWindow", text );
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			UpdateControls();

			var time = Time.Current;
			if( time > currentOpenScenesLastUpdate + 1.0 )
			{
				currentOpenScenesLastUpdate = time;
				UpdateOpenScenes();
			}

			//update store items
			if( !featuredStoreItemsWereUpdated )
			{
				var store = StoreManager.GetStore( "NeoAxis Store" );
				if( store != null )
				{
					var packages = StoreManager.GetPackages( store );
					if( packages.Length != 0 )
					{
						featuredStoreItemsWereUpdated = true;
						UpdateStoreItems( packages );
					}
				}
			}

			UpdateStoreItemImages();
		}

		private void kryptonButtonOpenStore_Click( object sender, EventArgs e )
		{
			EditorAPI2.OpenStoresWindow();
			//EditorAPI.OpenStore( true );
		}

		private void kryptonButtonLightTheme_Click( object sender, EventArgs e )
		{
			if( EditorMessageBox.ShowQuestion( "Set the light theme and restart the editor to apply changes?", EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
				return;

			ProjectSettings.Get.General.Theme = ProjectSettingsPage_General.ThemeEnum.Light;
			ProjectSettings.SaveToFileAndUpdate();
			EditorAPI2.BeginRestartApplication();
		}

		private void kryptonButtonDarkTheme_Click( object sender, EventArgs e )
		{
			if( EditorMessageBox.ShowQuestion( "Set the dark theme and restart the editor to apply changes?", EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
				return;

			ProjectSettings.Get.General.Theme = ProjectSettingsPage_General.ThemeEnum.Dark;
			ProjectSettings.SaveToFileAndUpdate();
			EditorAPI2.BeginRestartApplication();
		}

		private void kryptonButtonOpenScene_Click( object sender, EventArgs e )
		{
			if( contentBrowserOpenScene.SelectedItems.Length == 1 )
			{
				var item = contentBrowserOpenScene.SelectedItems[ 0 ];
				var fileName = item.Tag as string;

				if( !string.IsNullOrEmpty( fileName ) )
					EditorAPI2.OpenFileAsDocument( VirtualPathUtility.GetRealPathByVirtual( fileName ), true, true );
			}
		}

		private void buttonCreateScene_Click( object sender, EventArgs e )
		{
			if( contentBrowserNewScene.SelectedItemsOnlyListView.Length == 1 )
			{
				var item = contentBrowserNewScene.SelectedItemsOnlyListView[ 0 ];
				CreateResource( item );
			}
		}

		private void ContentBrowserNewScene_ItemAfterChoose( ContentBrowser sender, ContentBrowser.Item item, ref bool handled )
		{
			if( item != null )
				CreateResource( item );
		}

		void CreateResource( ContentBrowser.Item item )
		{
			if( item.Tag as SceneNewObjectSettings.TemplateClass != null )
				CreateScene( item );
			else if( item.Tag as string != null && item.Tag as string == "UIControl" )
				CreateUIControl();
			else
				SelectCreateResource();
		}

		void CreateScene( ContentBrowser.Item item )
		{
			try
			{
				var prefix = VirtualDirectory.Exists( "Scenes" ) ? @"Scenes\" : "";

				string fileName = null;
				for( int n = 1; ; n++ )
				{
					string f = prefix + string.Format( @"New{0}.scene", n > 1 ? n.ToString() : "" );
					if( !VirtualFile.Exists( f ) )
					{
						fileName = f;
						break;
					}
				}

				if( !string.IsNullOrEmpty( fileName ) )
				{
					var realFileName = VirtualPathUtility.GetRealPathByVirtual( fileName );

					var template = (SceneNewObjectSettings.TemplateClass)item.Tag;
					string name = template.Name + ".scene";
					var sourceFile = VirtualPathUtility.GetRealPathByVirtual( @"Base\Tools\NewResourceTemplates\" + name );

					//copy scene file

					var text = File.ReadAllText( sourceFile );

					var directoryName = Path.GetDirectoryName( realFileName );
					if( !Directory.Exists( directoryName ) )
						Directory.CreateDirectory( directoryName );

					File.WriteAllText( realFileName, text );

					//copy additional folder if exist
					var sourceFolderPath = sourceFile + "_Files";
					if( Directory.Exists( sourceFolderPath ) )
					{
						var destFolderPath = realFileName + "_Files";
						IOUtility.CopyDirectory( sourceFolderPath, destFolderPath );
					}

					EditorAPI2.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { realFileName } );
					EditorAPI2.OpenFileAsDocument( realFileName, true, true );
				}
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				//Log.Warning( e.Message );
				return;
			}
		}

		void CreateUIControl()
		{
			try
			{
				var prefix = VirtualDirectory.Exists( "UI" ) ? @"UI\" : "";

				string fileName = null;
				for( int n = 1; ; n++ )
				{
					string f = prefix + string.Format( @"New{0}.ui", n > 1 ? n.ToString() : "" );
					if( !VirtualFile.Exists( f ) )
					{
						fileName = f;
						break;
					}
				}

				if( !string.IsNullOrEmpty( fileName ) )
				{
					var realFileName = VirtualPathUtility.GetRealPathByVirtual( fileName );

					var sourceFile = VirtualPathUtility.GetRealPathByVirtual( @"Base\Tools\NewResourceTemplates\UIControl.ui" );

					var text = VirtualFile.ReadAllText( sourceFile );

					var directoryName = Path.GetDirectoryName( realFileName );
					if( !Directory.Exists( directoryName ) )
						Directory.CreateDirectory( directoryName );

					File.WriteAllText( realFileName, text );

					EditorAPI2.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { realFileName } );
					EditorAPI2.OpenFileAsDocument( realFileName, true, true );
				}
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				//Log.Warning( e.Message );
				return;
			}
		}

		void SelectCreateResource()
		{
			var initData = new NewObjectWindow.CreationDataClass();
			EditorAPI2.OpenNewObjectWindow( initData );
		}

		void UpdateOpenScenes()
		{
			string[] newFiles = new string[ 0 ];
			try
			{
				newFiles = VirtualDirectory.GetFiles( "", "*.scene", SearchOption.AllDirectories );

				CollectionUtility.MergeSort( newFiles, delegate ( string name1, string name2 )
				{
					var isScenes1 = name1.Length > "Scenes\\".Length && name1.Substring( 0, "Scenes\\".Length ) == "Scenes\\";
					var isScenes2 = name2.Length > "Scenes\\".Length && name2.Substring( 0, "Scenes\\".Length ) == "Scenes\\";
					if( isScenes1 && !isScenes2 )
						return -1;
					if( !isScenes1 && isScenes2 )
						return 1;

					var isSamples1 = name1.Length > "Samples\\".Length && name1.Substring( 0, "Samples\\".Length ) == "Samples\\";
					var isSamples2 = name2.Length > "Samples\\".Length && name2.Substring( 0, "Samples\\".Length ) == "Samples\\";
					if( isSamples1 && !isSamples2 )
						return -1;
					if( !isSamples1 && isSamples2 )
						return 1;

					var s1 = name1.Replace( "\\", " \\" );
					var s2 = name2.Replace( "\\", " \\" );
					return string.Compare( s1, s2 );
				} );
			}
			catch { }

			bool needUpdate = !newFiles.SequenceEqual( currentOpenScenes );
			if( !needUpdate )
				return;

			currentOpenScenes = newFiles;

			//add items
			try
			{
				var items = new List<ContentBrowser.Item>();

				foreach( var file in currentOpenScenes )
				{
					if( !file.Contains( @"Base\Tools\NewResourceTemplates" ) )
					{
						var realFileName = VirtualPathUtility.GetRealPathByVirtual( file );

						var item = new ContentBrowserItem_File( contentBrowserOpenScene, null, realFileName, false );
						item.SetText( file );
						item.Tag = file;
						item.imageKey = "Scene";

						items.Add( item );
					}
				}

				contentBrowserOpenScene.SetData( items, false );
				if( items.Count != 0 )
					contentBrowserOpenScene.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );
			}
			catch( Exception e2 )
			{
				Log.Warning( e2.Message );
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams handleParam = base.CreateParams;
				handleParam.ExStyle |= 0x02000000;//WS_EX_COMPOSITED       
				return handleParam;
			}
		}

		private void kryptonCheckBoxMinimizeRibbon_CheckedChanged( object sender, EventArgs e )
		{
			EditorForm.Instance.kryptonRibbon.MinimizedMode = kryptonCheckBoxMinimizeRibbon.Checked;
		}

		private void kryptonCheckBoxShowQATBelowRibbon_CheckedChanged( object sender, EventArgs e )
		{
			EditorForm.Instance.kryptonRibbon.QATLocation = kryptonCheckBoxShowQATBelowRibbon.Checked ? Internal.ComponentFactory.Krypton.Ribbon.QATLocation.Below : Internal.ComponentFactory.Krypton.Ribbon.QATLocation.Above;
		}

		//private void kryptonButtonDonate_Click( object sender, EventArgs e )
		//{
		//	Process.Start( new ProcessStartInfo( "https://www.neoaxis.com/licensing#donate" ) { UseShellExecute = true } );
		//}

		//private void kryptonButtonSubscribeToPro_Click( object sender, EventArgs e )
		//{
		//	Process.Start( new ProcessStartInfo( "https://www.neoaxis.com/licensing" ) { UseShellExecute = true } );
		//}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			//update panels
			if( IsHandleCreated )
			{
				var marginBetween = panelStoreItems.Location.X - ( panelNewResource.Location.X + panelNewResource.Width );

				panelNewResource.Width = Width / 2 - marginBetween / 2 - panelNewResource.Location.X;
				panelStoreItems.Location = new Point( panelNewResource.Location.X + panelNewResource.Width + marginBetween, panelStoreItems.Location.Y );
				panelStoreItems.Width = panelNewResource.Width;
			}
		}

		void UpdateStoreItems( string[] packages )
		{
			try
			{
				var packagesSet = new ESet<string>();
				foreach( var packageId in packages )
					packagesSet.AddWithCheckAlreadyContained( packageId );

				var items = new List<ContentBrowser.Item>();

				foreach( var packageId in NeoAxisStoreImplementation.FeaturedStoreItems )
				{
					if( packageId == "Platform_Tools" && EngineInfo.EngineMode != EngineInfo.EngineModeEnum.Standalone )
						continue;

					if( packagesSet.Contains( packageId ) )
					{
						var package = StoreManager.GetPackageInfo( packageId, false );

						if( package != null )
						{
							var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, package.Title );
							item.Tag = package.Identifier;
							item.imageKey = "Default_512";
							item.Description = package.GetTooltipDescription();
							items.Add( item );
						}
					}
				}

				//foreach( var packageId in featuredStoreItemIdentifiers )
				////foreach( var packageId in packages )
				//{
				//	if( packagesSet.Contains( packageId ) )
				//	//if( featuredStoreItemIdentifiers.Contains( packageId ) )
				//	{
				//		var package = StoreManager.GetPackageInfo( packageId, false );
				//		//var package = GetPackage( packageId, false );

				//		if( package != null )
				//		{
				//			var item = new ContentBrowserItem_Virtual( contentBrowserNewScene, null, package.Title );
				//			item.Tag = package.Identifier;
				//			item.imageKey = "Default_512";
				//			item.Description = package.GetTooltipDescription();
				//			items.Add( item );
				//		}
				//	}
				//}

				//UpdateNewScenes?.Invoke( ref items );

				contentBrowserStoreItems.SetData( items, false );
				//if( items.Count != 0 )
				//	contentBrowserStoreItems.SelectItems( new ContentBrowser.Item[] { items[ 0 ] } );
			}
			catch( Exception e2 )
			{
				Log.Warning( e2.Message );
			}
		}

		private void kryptonButtonInstallStoreItem_Click( object sender, EventArgs e )
		{
			if( contentBrowserStoreItems.SelectedItemsOnlyListView.Length == 1 )
			{
				var item = contentBrowserStoreItems.SelectedItemsOnlyListView[ 0 ];
				InstallStoreItem( item );
			}
		}

		private void ContentBrowserStoreItems_ItemAfterChoose( ContentBrowser sender, ContentBrowser.Item item, ref bool handled )
		{
			if( item != null )
				InstallStoreItem( item );
		}

		void InstallStoreItem( ContentBrowser.Item item )
		{
			EditorAPI2.OpenPackages( (string)item.Tag, false );
		}

		void UpdateStoreItemImages()
		{
#if !DEPLOY
			try
			{
				var time = EngineApp.GetSystemTime();

				foreach( var item in contentBrowserStoreItems.GetAllItems() )
				{
					var packageId = (string)item.Tag;

					var package = StoreManager.GetPackageInfo( packageId, false );
					if( package != null && !string.IsNullOrEmpty( package.Thumbnail ) )
					{
						var image = StoreManager.ImageManager.GetSquareImage( package.Thumbnail, time );
						if( image != null && item.image != image )
						{
							item.image = image;

							//!!!!не все обновлять
							contentBrowserStoreItems.needUpdateImages = true;
						}
					}
				}
			}
			catch( Exception e2 )
			{
				Log.Warning( e2.Message );
			}
#endif
		}

		void ConfigureNewSceneContextMenu()
		{
			contentBrowserNewScene.ShowContextMenuEvent += delegate ( ContentBrowser sender, ContentBrowser.Item contentItem, List<KryptonContextMenuItemBase> items )
			{
				if( contentItem != null && sender.SelectedItemsOnlyListView.Length != 0 )
				{
					var item = new KryptonContextMenuItem( Translate( "Create" ), null, delegate ( object s, EventArgs e2 )
					{
						CreateResource( contentItem );
					} );
					items.Add( item );
				}
			};
		}

		void ConfigureStoreItemsContextMenu()
		{
			contentBrowserStoreItems.ShowContextMenuEvent += delegate ( ContentBrowser sender, ContentBrowser.Item contentItem, List<KryptonContextMenuItemBase> items )
			{
				if( contentItem != null && sender.SelectedItemsOnlyListView.Length != 0 )
				{
					var item = new KryptonContextMenuItem( Translate( "Learn More" ), null, delegate ( object s, EventArgs e2 )
					{
						InstallStoreItem( contentItem );
					} );
					items.Add( item );
				}
			};
		}

	}
}

#endif