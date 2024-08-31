#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Globalization;
using System.Drawing.Drawing2D;
using Internal.ComponentFactory.Krypton.Toolkit;
using Downloader;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents the Stores window.
	/// </summary>
	public partial class StoresWindow : DockWindow
	{
		double toolbarMustVisibleForTime;

		StoreManager.StoreItem selectedStore;

		Dictionary<string, ContentBrowserItem_StoreGroupItem> groupItemByCategory = new Dictionary<string, ContentBrowserItem_StoreGroupItem>();

		double needUpdateListTime;
		bool needUpdateListReset;
		bool needUpdateListRefreshFromStores;

		//never cleared
		Dictionary<string, PackageState> packageStates = new Dictionary<string, PackageState>();

		string needSelectPackage;
		bool needSelectPackageInstall;

		//bool - open after install
		List<(string, bool)> needStartInstallPackages = new List<(string, bool)>();

		Dictionary<string, CategoryItem> categoryByName = new Dictionary<string, CategoryItem>();

		double lastListsUpdate;

		StoreManager.FilterSettingsClass filterSettings = new StoreManager.FilterSettingsClass();

		public static bool needOpenOptions;

		///////////////////////////////////////////////

		public class ContentBrowserItem_StoreGroupItem : ContentBrowserItem_Virtual
		{
			public StoreGroupType groupType;

			public ContentBrowserItem_StoreGroupItem( ContentBrowser owner, ContentBrowser.Item parent, string text, StoreGroupType groupType )
				: base( owner, parent, text )
			{
				this.groupType = groupType;
			}
		}

		///////////////////////////////////////////////

		[Preview( typeof( StoreItemPreview ) )]
		public class ContentBrowserItem_StoreItem : ContentBrowserItem_Virtual
		{
			public StoresWindow storesWindow;
			public string packageId;

			public double stateProgress;
			public Color stateColor;

			public Image createdImage;
			public double createdImageStateProgress;
			public Color createdImageStateColor;
			public double createdImageLastUsedTime;

			//

			public ContentBrowserItem_StoreItem( ContentBrowser owner, ContentBrowser.Item parent, string text, StoresWindow storesWindow, string packageId )
				: base( owner, parent, text )
			{
				this.storesWindow = storesWindow;
				this.packageId = packageId;
			}

			public override string GetDescription()
			{
				var package = storesWindow.GetPackage( packageId, false );
				if( package != null )
					return package.GetTooltipDescription();
				return "";
			}

			public override void Dispose()
			{
				createdImage?.Dispose();
				createdImage = null;

				base.Dispose();
			}

			public override object ContainedObject
			{
				get
				{
					return this;
					//return package;
				}
			}

			public (PackageManager.PackageInfo.FileTypeToDrop type, string reference) GetFileToDrop( bool allowAutoInstall )
			{
				var package = storesWindow.GetPackage( packageId, false );
				if( package != null )
				{
					var result = package.GetFileToDrop();

					//start auto install
					if( allowAutoInstall && result.type != PackageManager.PackageInfo.FileTypeToDrop.None )
					{
						var state = storesWindow.GetPackageState( packageId );
						if( state == null || ( !state.Installed && !state.Downloaded ) )
							storesWindow.StartDownload( package.Identifier, true, false, true );
					}

					return result;
				}

				return (PackageManager.PackageInfo.FileTypeToDrop.None, "");
			}

			public override void CalculateReferenceValue( Component from, Metadata.TypeInfo expectedType, out string referenceValue, out bool canSet )
			{
				referenceValue = "";
				canSet = false;

				var (type, reference) = GetFileToDrop( false );

				if( !string.IsNullOrEmpty( reference ) )
				{
					switch( type )
					{
					case PackageManager.PackageInfo.FileTypeToDrop.Mesh:
						if( MetadataManager.GetTypeOfNetType( typeof( Mesh ) ).IsAssignableFrom( expectedType ) )
						{
							referenceValue = reference;
							canSet = true;
						}
						break;

					case PackageManager.PackageInfo.FileTypeToDrop.Material:
						if( MetadataManager.GetTypeOfNetType( typeof( Material ) ).IsAssignableFrom( expectedType ) )
						{
							referenceValue = reference;
							canSet = true;
						}
						break;

					case PackageManager.PackageInfo.FileTypeToDrop.Surface:
						if( MetadataManager.GetTypeOfNetType( typeof( Surface ) ).IsAssignableFrom( expectedType ) )
						{
							referenceValue = reference;
							canSet = true;
						}
						break;
					}
				}

				//start download and install
				if( canSet )
					GetFileToDrop( true );
			}
		}

		///////////////////////////////////////////////

		public class StoresContentBrowserOptions : ContentBrowserOptions
		{
			public StoresContentBrowserOptions( ContentBrowser owner )
				: base( owner )
			{
			}

			protected override void MetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
			{
				var p = member as Metadata.Property;
				if( p != null )
				{
					if( p.Name != nameof( TileImageSize ) && p.Name != nameof( Breadcrumb ) )
						skip = true;
				}

				base.MetadataGetMembersFilter( context, member, ref skip );
			}
		}

		///////////////////////////////////////////////

		public enum StoreGroupType
		{
			None,
			//Images,
			//Materials,
			Models,
			//Environments,
			//Surfaces,
		}

		///////////////////////////////////////////////

		public class PackageState
		{
			public StoresWindow storesWindow;
			public string packageId;

			public volatile string downloadingAddress = "";
			public volatile string downloadingDestinationPath = "";
			public volatile float downloadProgress;
			public volatile bool downloadingInstallAfterDownload;
			//public volatile WebClient downloadingClient;
			public volatile Downloader.DownloadService downloadingDownloader;

			public bool AutoImport;

			//

			public bool Installed
			{
				get { return PackageManager.IsInstalled( packageId, false ); }
			}

			public bool Downloaded
			{
				get
				{
					var package = storesWindow.GetPackage( packageId, false );
					if( package == null )
						return false;
					return !string.IsNullOrEmpty( package.FullFilePath ) && PackageManager.ReadPackageArchiveInfo_CheckOnly( package.FullFilePath, out _ );
				}
			}

			public bool Downloading
			{
				get { return !string.IsNullOrEmpty( downloadingAddress ); }
			}

			public bool CanDownload
			{
				get
				{
					var package = storesWindow.GetPackage( packageId, false );
					if( package == null )
						return false;
					return !string.IsNullOrEmpty( package.FreeDownload ) || package.SecureDownload && IsPurchasedProduct( package.Identifier );
				}
			}
		}

		///////////////////////////////////////////////

		public class CategoryItem
		{
			public string Name;
			public CategoryItem Parent;
			//public List<CategoryItem> Children = new List<CategoryItem>();

			public CategoryItem( string name )
			{
				Name = name;
			}
		}

		///////////////////////////////////////////////

		public StoresWindow()
		{
			InitializeComponent();

			InitCategories();

			toolStripButtonOptions.Image = EditorResourcesCache.Options;
			toolStripButtonRefresh.Image = EditorResourcesCache.Refresh;
			toolStripButtonStores.Image = EditorResourcesCache.Selection;
			toolStripButtonFilter.Image = EditorResourcesCache.Filter;

			foreach( var item in toolStripForTreeView.Items )
			{
				var button = item as ToolStripButton;
				if( button != null )
					button.Text = EditorLocalization2.Translate( "StoresWindow", button.Text );

				var button2 = item as ToolStripDropDownButton;
				if( button2 != null )
					button2.Text = EditorLocalization2.Translate( "StoresWindow", button2.Text );
			}

			toolStripForTreeView.Renderer = EditorThemeUtility.GetToolbarToolStripRenderer();


			//var data = new ContentBrowser.ResourcesModeDataClass();
			//data.selectionMode = ResourceSelectionMode.None;
			contentBrowser1.Init( null, null, /*data, */null );
			contentBrowser1.Options = new StoresContentBrowserOptions( contentBrowser1 );

			contentBrowser1.Options.TileImageSize = 60;

			ContentBrowserSetConstantOptions();

			Config_Load();
			EngineConfig.SaveEvent += Config_SaveEvent;

			WindowTitle = EditorLocalization2.Translate( "Windows", WindowTitle );
			//EditorThemeUtility.ApplyDarkThemeToForm( panelToolbar );

			toolStripButtonSearch.TextChanged += toolStripButtonSearch_TextChanged;
			toolStripForTreeView.Layout += ToolStripForTreeView_Layout;
			Resize += StoresWindow_Resize;
		}

		void InitCategories()
		{
			var rootCategories = new[] { "2D", "Audio", "Basic Content", "Buildings", "Characters", "Demos", "Environments", "Extensions", "Fences", "Materials", "Models", "Pipes", "Surfaces", "Vehicles", "Visual Effects", "Weapons", "Uncategorized", "Installed" };
			foreach( var name in rootCategories )
			{
				var category = new CategoryItem( name );
				categoryByName.Add( category.Name, category );
			}

			//Audio
			{
				var parent = categoryByName[ "Audio" ];
				var categories = new[] { "Ambient Sounds", "Music", "Sound Effects" };
				foreach( var name in categories )
				{
					var category = new CategoryItem( name );
					category.Parent = parent;
					categoryByName.Add( category.Name, category );
				}
			}

			//Extensions
			{
				var parent = categoryByName[ "Extensions" ];
				var categories = new[] { "Basic Extensions", "Components", "Constructors", "Kits" };
				foreach( var name in categories )
				{
					var category = new CategoryItem( name );
					category.Parent = parent;
					categoryByName.Add( category.Name, category );
				}
			}



			//var rootCategories = new[] { /*"2D", */ /*"Audio",*/ "Basic Content", "Demos", "Environments", "Extensions", /*"Images",*/ "Materials", "Models", "Functional Objects", "Surfaces", "Visual Effects", "Uncategorized", "Installed" };
			//foreach( var name in rootCategories )
			//{
			//	var category = new CategoryItem( name );
			//	categoryByName.Add( category.Name, category );
			//}

			//////Audio
			////{
			////	var parent = categoryByName[ "Audio" ];
			////	var categories = new[] { "Ambient Sounds", "Music", "Sound Effects" };
			////	foreach( var name in categories )
			////	{
			////		var category = new CategoryItem( name );
			////		category.Parent = parent;
			////		categoryByName.Add( category.Name, category );
			////	}
			////}

			////Extensions
			//{
			//	var parent = categoryByName[ "Extensions" ];
			//	var categories = new[] { "Basic Extensions", "Components", "Constructors", "Kits" };
			//	foreach( var name in categories )
			//	{
			//		var category = new CategoryItem( name );
			//		category.Parent = parent;
			//		categoryByName.Add( category.Name, category );
			//	}
			//}

			////Models
			//{
			//	var parent = categoryByName[ "Models" ];
			//	var categories = new[] { "Animals", "Architecture", "Characters", "Exterior", "Food", "Industrial", "Interior", "Nature", /*"Plant",*/ "Uncategorized Models", "Vehicles", "Weapons" };
			//	foreach( var name in categories )
			//	{
			//		var category = new CategoryItem( name );
			//		category.Parent = parent;
			//		categoryByName.Add( category.Name, category );
			//	}
			//}
		}

		void ContentBrowserSetConstantOptions()
		{
			contentBrowser1.Options.PanelMode = ContentBrowser.PanelModeEnum.List;
			contentBrowser1.Options.SplitterPosition = 3.0 / 5.0;
			contentBrowser1.Options.EditorButton = false;
			contentBrowser1.Options.SettingsButton = false;
			contentBrowser1.Options.DisplayPropertiesEditorSettingsButtons = false;
			contentBrowser1.Options.ListMode = ContentBrowser.ListModeEnum.Tiles;
			contentBrowser1.ShowToolBar = false;
		}

		public override bool HideOnRemoving { get { return true; } }

		private void StoresWindow_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			toolbarMustVisibleForTime = Time.Current;
			timer1.Start();


			//update toolstrip sizes
			{
				var dpiScale = Math.Min( EditorAPI2.DPIScale, 2 );

				void UpdateSize( ToolStripItem item )
				{
					int width = 20;
					if( item is ToolStripDropDownButton )
						width = 28;
					item.Size = new Size( (int)( width * dpiScale ), (int)( 20 * dpiScale + 2 ) );
					//item.Size = new Size( (int)( width * EditorAPI.DPIScale + 2 ), (int)( 20 * EditorAPI.DPIScale + 2 ) );
				}

				toolStripForTreeView.Padding = new Padding( (int)dpiScale );
				toolStripForTreeView.Size = new Size( 10, (int)( 21 * dpiScale + 2 ) );

				foreach( var item in toolStripForTreeView.Items )
				{
					var button = item as ToolStripButton;
					if( button != null )
						UpdateSize( button );

					var button2 = item as ToolStripDropDownButton;
					if( button2 != null )
						UpdateSize( button2 );
				}


				toolStripForTreeView.Padding = new Padding( (int)dpiScale );
				toolStripForTreeView.Size = new Size( 10, (int)( 21 * dpiScale + 2 ) );
			}


			UpdateControls();

			ConfigureContextMenu();

			EditorAPI.ClosingApplicationChanged += EditorAPI_ClosingApplicationChanged;

			//create EnginePackages folder
			try
			{
				if( !Directory.Exists( PackageManager.PackagesFolder ) )
					Directory.CreateDirectory( PackageManager.PackagesFolder );
			}
			catch( Exception e2 )
			{
				Log.Warning( e2.Message );
			}

			CreateStructureItems();

			//called from thread
			StoreManager.DownloadedListOfPackagesUpdated += delegate ()
			{
				needUpdateListTime = EngineApp.GetSystemTime();
			};

			PerformRefreshFromStores( false );
			PackageManager.GetInstalledPackages( true );

			//!!!!
			//EditorLocalization.TranslateForm( "StoresWindow", kryptonSplitContainer2.Panel1 );
			//EditorLocalization.TranslateForm( "StoresWindow", this );
		}


		protected override void OnDestroy()
		{
			EditorAPI.ClosingApplicationChanged -= EditorAPI_ClosingApplicationChanged;

			base.OnDestroy();
		}

		[Browsable( false )]
		public ContentBrowser ContentBrowser1
		{
			get { return contentBrowser1; }
		}

		public override ObjectsInFocus GetObjectsInFocus()
		{
			return new ObjectsInFocus( null, contentBrowser1.SelectedItems );
			//return new ObjectsInFocus( null, resourcesBrowser1.GetSelectedContainedObjects() );
		}

		void Config_Load()
		{
			var windowBlock = EngineConfig.TextBlock.FindChild( nameof( StoresWindow ) );
			if( windowBlock != null )
			{
				var browserBlock = windowBlock.FindChild( "ContentBrowser" );
				if( browserBlock != null )
				{
					ContentBrowser1.Options.Load( browserBlock );
					ContentBrowserSetConstantOptions();
				}

				if( windowBlock.AttributeExists( "Store" ) )
				{
					var store = windowBlock.GetAttribute( "Store" );
					selectedStore = StoreManager.Stores.Find( s => s.Name == store );
				}

				var filterBlock = windowBlock.FindChild( "Filter" );
				if( filterBlock != null )
					filterSettings.Load( filterBlock );

				SelectedStoreChanged();
			}
		}

		void Config_SaveEvent()
		{
			var configBlock = EngineConfig.TextBlock;

			var old = configBlock.FindChild( nameof( StoresWindow ) );
			if( old != null )
				configBlock.DeleteChild( old );

			var windowBlock = configBlock.AddChild( nameof( StoresWindow ) );
			var browserBlock = windowBlock.AddChild( "ContentBrowser" );
			ContentBrowser1.Options.Save( browserBlock );

			if( selectedStore != null )
				windowBlock.SetAttribute( "Store", selectedStore.Name );

			var filterBlock = windowBlock.AddChild( "Filter" );
			filterSettings.Save( filterBlock );
		}

		void UpdateControls()
		{
			var parentSize = contentBrowser1.Parent.ClientSize;
			contentBrowser1.Location = new Point( 0, toolStripForTreeView.Height );
			contentBrowser1.Size = new Size( parentSize.Width, parentSize.Height - toolStripForTreeView.Height );
		}

		private void ToolStripForTreeView_Layout( object sender, LayoutEventArgs e )
		{
			int width = toolStripForTreeView.DisplayRectangle.Width;

			foreach( ToolStripItem item in toolStripForTreeView.Items )
			{
				if( !( item == toolStripButtonSearch ) )
				{
					width -= item.Width;
					width -= item.Margin.Horizontal;
				}
			}

			toolStripButtonSearch.Width = Math.Max( 0, width - toolStripButtonSearch.Margin.Horizontal - 1 );
		}

		private void StoresWindow_Resize( object sender, EventArgs e )
		{
			UpdateControls();
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( !IsHandleCreated || WinFormsUtility.IsDesignerHosted( this ) || EditorAPI.ClosingApplication )
				return;

			UpdateControls();

			var currentTime = EngineApp.GetSystemTime();

			if( needUpdateListTime != 0 && currentTime > needUpdateListTime )
			{
				var reset = needUpdateListReset;
				var refreshFromStores = needUpdateListRefreshFromStores;

				needUpdateListReset = false;
				needUpdateListRefreshFromStores = false;
				needUpdateListTime = 0;

				if( refreshFromStores )
					PerformRefreshFromStores( false );
				else
					UpdateList( false, reset );

				//to update list items
				lastListsUpdate = 0;
			}
			//if( needUpdateList )
			//{
			//	needUpdateList = false;
			//	UpdateList( false );
			//}

			if( lastListsUpdate + 1.0 < currentTime )
			{
				lastListsUpdate = currentTime;

				PackageManager.GetPackagesInfoByFileArchives( true );
				PackageManager.GetInstalledPackages( true );

				StoreManager.ImageManager.DeleteItemsNotUsedForLongTime();

				try
				{
					UpdateListItems();
				}
				catch { }

				UpdateStoreImplemetationToGetNextItems();
			}

			//need install packages
			{
				List<(string, bool)> needStartInstallPackages2;
				lock( needStartInstallPackages )
				{
					needStartInstallPackages2 = new List<(string, bool)>( needStartInstallPackages );
					needStartInstallPackages.Clear();
				}
				foreach( var item in needStartInstallPackages2 )
					StartInstall( item.Item1, item.Item2 );
			}

			//need select package
			if( !string.IsNullOrEmpty( needSelectPackage ) )
			{
				foreach( var item in contentBrowser1.GetAllItemsByItemHierarchy( true ) )
				{
					var storeItem = item as ContentBrowserItem_StoreItem;
					if( storeItem != null && storeItem.packageId == needSelectPackage )
					{
						contentBrowser1.SelectItemsList( new ContentBrowser.Item[] { item }, true );

						if( needSelectPackageInstall )
							StartDownload( needSelectPackage, true, true );

						break;
					}
				}

				needSelectPackage = null;
				needSelectPackageInstall = false;
			}

			if( needOpenOptions )
			{
				needOpenOptions = false;
				toolStripButtonOptions_Click( null, null );
			}

		}

		void SelectedStoreChanged()
		{
			toolStripButtonStores.Image = selectedStore != null ? selectedStore.IconScaled : EditorResourcesCache.Selection;
		}

		private void toolStripButtonStores_Click( object sender, EventArgs e )
		{
			var items = new List<KryptonContextMenuItemBase>();

			{
				var item = new KryptonContextMenuItem( Translate( "All" ), EditorResourcesCache.Selection, delegate ( object s, EventArgs e2 )
				{
					var oldStore = GetSelectedStore();

					selectedStore = null;

					if( oldStore != selectedStore )
					{
						SelectedStoreChanged();
						contentBrowser1.SelectItems( new List<ContentBrowser.Item>() );
						PerformRefreshFromStores( false );
						//needUpdateListTime = EngineApp.GetSystemTime();
					}
				} );
				item.Checked = selectedStore == null;
				items.Add( item );
			}

			foreach( var store in StoreManager.Stores )
			{
				var item = new KryptonContextMenuItem( store.Name, store.IconScaled, delegate ( object s, EventArgs e2 )
				{
					var oldStore = GetSelectedStore();

					selectedStore = (StoreManager.StoreItem)( (KryptonContextMenuItem)s ).Tag;

					if( oldStore != selectedStore )
					{
						SelectedStoreChanged();
						contentBrowser1.SelectItems( new List<ContentBrowser.Item>() );
						PerformRefreshFromStores( false );
						//needUpdateListTime = EngineApp.GetSystemTime();
					}
				} );
				item.Checked = GetSelectedStore() == store;
				item.Tag = store;
				items.Add( item );
			}

			//EditorContextMenuWinForms.AddActionsToMenu( EditorContextMenuWinForms.MenuTypeEnum.None, items );
			//ShowContextMenuEvent?.Invoke( this, null, items );

			EditorContextMenuWinForms.Show( items, this, PointToClient( Cursor.Position ) );
		}

		private void toolStripButtonOptions_Click( object sender, EventArgs e )
		{
			var form = new ContentBrowserOptionsForm( contentBrowser1 );

			if( EditorForm.Instance == null )
				form.ShowDialog();
			else
			{
				EditorForm.Instance.WorkspaceController.BlockAutoHideAndDoAction( this, () =>
				{
					form.ShowDialog();
				} );
			}
		}

		private void toolStripButtonRefresh_Click( object sender, EventArgs e )
		{
			StoreManager.ImageManager.Clear();
			//PackageManager.GetPackagesInfoByFileArchives( true );
			PerformRefreshFromStores( true );

			//needUpdateListReset = true;
			//needUpdateListRefreshFromStores = true;
			//needUpdateListTime = EngineApp.GetSystemTime() - 0.01;
		}

		private void toolStripButtonSearch_TextChanged( object sender, EventArgs e )
		{
			needUpdateListReset = true;
			needUpdateListRefreshFromStores = true;
			needUpdateListTime = EngineApp.GetSystemTime() + 1.0;
		}

		static string Translate( string text )
		{
			return EditorLocalization2.Translate( "StoresWindow", text );
		}

		void ConfigureContextMenu()
		{
			contentBrowser1.ShowContextMenuEvent += delegate ( ContentBrowser sender, ContentBrowser.Item contentItem, List<KryptonContextMenuItemBase> items )
			{
				//Store item
				var storeItem = contentItem as ContentBrowserItem_StoreItem;
				if( storeItem != null )
				{
					var package = GetPackage( storeItem.packageId, false );
					if( package != null )
					{
						var state = GetPackageState( storeItem.packageId );

						//!!!!
						//{
						//	var item = new KryptonContextMenuItem( Translate( "Explore Content" ), EditorResourcesCache.Info, delegate ( object s, EventArgs e2 )
						//	{

						//	} );
						//	item.Enabled = false;
						//	items.Add( item );
						//}

						if( package.CostNumber > 0 )
						{
							var item = new KryptonContextMenuItem( Translate( "Buy" ), EditorResourcesCache.Money, delegate ( object s, EventArgs e2 )
							{
								var link = package.Permalink;
								Process.Start( new ProcessStartInfo( link ) { UseShellExecute = true } );
							} );
							item.Enabled = !state.CanDownload;
							items.Add( item );
						}

						////separator
						//items.Add( new KryptonContextMenuSeparator() );

						var canCancelInstallation = state.Downloading;
						if( canCancelInstallation )
						{
							var item = new KryptonContextMenuItem( Translate( "Cancel Installation" ), EditorResourcesCache.Download, delegate ( object s, EventArgs e2 )
							{
								CancelInstallation( storeItem.packageId );
							} );
							//item.Enabled = ;
							items.Add( item );
						}
						else
						{
							var item = new KryptonContextMenuItem( Translate( "Install" ), EditorResourcesCache.Download, delegate ( object s, EventArgs e2 )
							{
								StartDownload( storeItem.packageId, true, true );
							} );
							item.Enabled = !state.Installed && ( state.CanDownload || state.Downloaded );
							//item.Enabled = !state.Installed && state.CanDownload || state.Downloaded;// && state.CanDownload;
							items.Add( item );
						}

						{
							var item = new KryptonContextMenuItem( Translate( "Delete" ), EditorResourcesCache.Delete, delegate ( object s, EventArgs e2 )
							{
								TryStartUninstall( storeItem.packageId, true );
							} );
							item.Enabled = ( state.Installed || state.Downloaded ) && !state.Downloading;
							items.Add( item );
						}

						if( state.Installed )
						{
							var item = new KryptonContextMenuItem( Translate( "Delete (Save archive)" ), EditorResourcesCache.Delete, delegate ( object s, EventArgs e2 )
							{
								TryStartUninstall( storeItem.packageId, false );
							} );
							item.Enabled = ( state.Installed || state.Downloaded ) && !state.Downloading;
							items.Add( item );
						}

						//separator
						items.Add( new KryptonContextMenuSeparator() );

						{
							var topDirectory = "";
							try
							{
								if( !string.IsNullOrEmpty( package.FullFilePath ) )
								{
									var archiveInfo = PackageManager.ReadPackageArchiveInfo( package.FullFilePath, out _ );
									if( archiveInfo != null )
									{
										foreach( var file in archiveInfo.Files )
										{
											var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );
											if( File.Exists( fullName ) )
											{
												var directory = Path.GetDirectoryName( fullName );
												if( string.IsNullOrEmpty( topDirectory ) || directory.Length < topDirectory.Length )
													topDirectory = directory;
											}
										}
									}
								}
								else if( !string.IsNullOrEmpty( package.Files ) )
								{
									foreach( var file in package.GetFiles() )
									{
										var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );
										if( File.Exists( fullName ) )
										{
											var directory = Path.GetDirectoryName( fullName );
											if( string.IsNullOrEmpty( topDirectory ) || directory.Length < topDirectory.Length )
												topDirectory = directory;
										}
									}
								}
							}
							catch { }

							var item = new KryptonContextMenuItem( Translate( "Go to Folder" ), EditorResourcesCache.SelectFolder, delegate ( object s, EventArgs e2 )
							{
								if( !string.IsNullOrEmpty( topDirectory ) )
									EditorAPI2.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { topDirectory }, true );
							} );
							item.Enabled = state.Installed && !string.IsNullOrEmpty( topDirectory );
							items.Add( item );
						}

						{
							var item = new KryptonContextMenuItem( Translate( "View on Website" ), null, delegate ( object s, EventArgs e2 )
							{
								Process.Start( new ProcessStartInfo( package.Permalink ) { UseShellExecute = true } );
							} );
							item.Enabled = !string.IsNullOrEmpty( package.Permalink );
							items.Add( item );
						}
					}
				}
			};
		}

		void CreateStructureItems()
		{
			groupItemByCategory.Clear();

			var roots = new List<ContentBrowser.Item>();

			contentBrowser1.AddImageKey( "Folder_512", Properties.Resources.Folder_512 );
			contentBrowser1.AddImageKey( "Default_512", Properties.Resources.Default_512 );

			var rootItem = new ContentBrowserItem_Virtual( contentBrowser1, null, "Root" );
			rootItem.imageKey = "Folder_512";
			roots.Add( rootItem );

			foreach( var categoryNameItem in categoryByName.Values )
			{
				if( categoryNameItem.Parent == null )
				{
					var category = categoryNameItem.Name;

					StoreGroupType groupType;
					if( !Enum.TryParse( category, out groupType ) )
						groupType = StoreGroupType.None;

					var categoryItem = new ContentBrowserItem_StoreGroupItem( contentBrowser1, rootItem, category, groupType );
					categoryItem.imageKey = "Folder_512";
					categoryItem.Tag = category;
					rootItem.children.Add( categoryItem );

					groupItemByCategory[ category ] = categoryItem;
				}
			}

			contentBrowser1.SetData( roots );
		}

		ContentBrowserItem_StoreGroupItem GetGroupItemByCategory( string category, bool canFindParent )
		{
			var current = category;

next:
			if( groupItemByCategory.TryGetValue( current, out var item ) )
				return item;

			if( canFindParent )
			{
				if( categoryByName.TryGetValue( current, out var categoryItem ) && categoryItem.Parent != null )
				{
					current = categoryItem.Parent.Name;
					goto next;
				}
			}

			return null;
		}

		ContentBrowserItem_StoreItem GetPackageItemInGroup( ContentBrowserItem_StoreGroupItem groupItem, string packageId )
		{
			foreach( var item in groupItem.GetChildren( true ) )
			{
				var storeItem = item as ContentBrowserItem_StoreItem;
				if( storeItem != null && storeItem.packageId == packageId )
					return storeItem;
			}
			return null;
		}

		void UpdateList( bool removeOnly, bool reset )
		{
			var changedGroupItems = new ESet<ContentBrowserItem_StoreGroupItem>();

			//delete old
			if( reset )
			{
				foreach( var groupItem in groupItemByCategory.Values )
				{
					if( groupItem.children.Count != 0 )
					{
						groupItem.DeleteChildren();

						groupItem.SetText( (string)groupItem.Tag );

						//changedGroupItems.AddWithCheckAlreadyContained( groupItem );
						groupItem.PerformChildrenChanged();
					}
				}
			}

			//clear Uncategorized group
			{
				var groupItem = GetGroupItemByCategory( "Uncategorized", true );
				if( groupItem != null && groupItem.children.Count != 0 )
				{
					groupItem.DeleteChildren();

					changedGroupItems.AddWithCheckAlreadyContained( groupItem );
				}
			}

			//string lastSelected = "";
			//if( contentBrowser1.SelectedItems.Length != 0 )
			//	lastSelected = contentBrowser1.SelectedItems[ 0 ].Text;


			if( !removeOnly )
			{
				//var alreadyAddedPackages = new ESet<string>();
				//{
				//	foreach( var item in contentBrowser1.GetAllItemsByItemHierarchy( true ) )
				//	{
				//		var storeItem = item as ContentBrowserItem_StoreItem;
				//		if( storeItem != null )
				//			alreadyAddedPackages.AddWithCheckAlreadyContained( storeItem.packageId );
				//	}
				//}

				foreach( var packageId in GetAllPackages( true ) )
				{
					var package = GetPackage( packageId, false );
					if( package != null )
					{
						var state = GetPackageState( packageId );

						var alreadyAdded_excludingInstalledGroup = false;
						//var wasAddedToGroup = new ESet<ContentBrowserItem_Virtual>();

						var categories = package.Categories;
						if( categories == null )
							categories = "";
						if( state != null && state.Installed )
						{
							if( !string.IsNullOrEmpty( categories ) )
								categories += ", ";
							categories += "Installed";
						}

						foreach( var category2 in categories.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries ) )
						{
							var category = category2.Trim();

							var groupItem = GetGroupItemByCategory( category, true );
							if( groupItem != null )
							{
								if( GetPackageItemInGroup( groupItem, packageId ) == null )
								{
									var item = new ContentBrowserItem_StoreItem( contentBrowser1, groupItem, package.Title, this, packageId );
									item.imageKey = "Default_512";
									groupItem.children.Add( item );

									changedGroupItems.AddWithCheckAlreadyContained( groupItem );
								}

								if( category != "Installed" )
									alreadyAdded_excludingInstalledGroup = true;
								//wasAddedToGroup.Add( groupItem );
							}

							//if( groupItem != null && !wasAddedToGroup.Contains( groupItem ) )
							//{
							//	if( !alreadyAddedPackages.Contains( packageId ) )
							//	{
							//		var item = new ContentBrowserItem_StoreItem( contentBrowser1, groupItem, package.Title, this, packageId );
							//		item.imageKey = "Default_512";
							//		groupItem.children.Add( item );

							//		changedGroupItems.AddWithCheckAlreadyContained( groupItem );
							//	}

							//	if( category != "Installed" )
							//		alreadyAdded_excludingInstalledGroup = true;
							//	wasAddedToGroup.Add( groupItem );
							//}
						}

						if( !alreadyAdded_excludingInstalledGroup )
						{
							//if( !alreadyAddedPackages.Contains( packageId ) )
							//{

							var groupItem = GetGroupItemByCategory( "Uncategorized", true );

							if( GetPackageItemInGroup( groupItem, packageId ) == null )
							{
								var item = new ContentBrowserItem_StoreItem( contentBrowser1, groupItem, package.Title, this, packageId );
								item.imageKey = "Default_512";
								groupItem.children.Add( item );

								changedGroupItems.AddWithCheckAlreadyContained( groupItem );
							}

							//}
						}
					}
				}
			}

			foreach( var groupItem in changedGroupItems )
			{
				//update text of the groups
				{
					var text = (string)groupItem.Tag;
					if( groupItem.children.Count != 0 )
					{
						//!!!!hack
						var addPlus = "";
						if( groupItem.children.Count >= 24 && groupItem.groupType == StoreGroupType.Models )
							addPlus = "+";

						text += $" ({groupItem.children.Count}{addPlus})";
					}
					groupItem.SetText( text );
				}

				//sort children by store index
				CollectionUtility.InsertionSort( groupItem.children, delegate ( ContentBrowser.Item item1, ContentBrowser.Item item2 )
				{
					var i1 = (ContentBrowserItem_StoreItem)item1;
					var i2 = (ContentBrowserItem_StoreItem)item2;

					var store1 = StoreManager.GetPackageStore( i1.packageId );
					var store2 = StoreManager.GetPackageStore( i2.packageId );

					if( store1 != null && store2 != null )
					{
						var index1 = StoreManager.Stores.IndexOf( store1 );
						var index2 = StoreManager.Stores.IndexOf( store2 );
						if( index1 < index2 )
							return -1;
						if( index1 > index2 )
							return 1;
					}

					return 0;
				} );

				//process

				//disable scrollbar reset to start
				var listView = contentBrowser1.GetListView();
				listView.SetItemsScrollBarPositionReset = false;

				groupItem.PerformChildrenChanged();

				listView.SetItemsScrollBarPositionReset = true;
			}

			//update bread crumb
			if( reset || changedGroupItems.Count != 0 )
				contentBrowser1.UpdateBreadcrumb();
		}

		void ShowRestartLabel()
		{
			//labelRestart.Visible = true;
		}

		//static string HtmlToPlainText( string html )
		//{
		//	if( string.IsNullOrEmpty( html ) )
		//		return "";

		//	const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
		//	const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
		//	const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
		//	var lineBreakRegex = new Regex( lineBreak, RegexOptions.Multiline );
		//	var stripFormattingRegex = new Regex( stripFormatting, RegexOptions.Multiline );
		//	var tagWhiteSpaceRegex = new Regex( tagWhiteSpace, RegexOptions.Multiline );

		//	var text = html;
		//	//Decode html specific characters
		//	text = System.Net.WebUtility.HtmlDecode( text );
		//	//Remove tag whitespace/line breaks
		//	text = tagWhiteSpaceRegex.Replace( text, "><" );
		//	//Replace <br /> with line breaks
		//	text = lineBreakRegex.Replace( text, Environment.NewLine );
		//	//Strip formatting
		//	text = stripFormattingRegex.Replace( text, string.Empty );

		//	return text;
		//}

		static bool IsPurchasedProduct( string productIdentifier )
		{
			if( LoginUtility.GetCurrentLicense( out var email, out _ ) )
			{
				if( LoginUtility.GetRequestedFullLicenseInfo( out var license, out var purchasedProducts, /*out _,*/ out _ ) )
				{
					if( StoreManager.ModeratorMode )
						return true;

					if( !string.IsNullOrEmpty( license ) )
						return purchasedProducts.Contains( productIdentifier );
				}
			}
			return false;
		}

		string GetSearchText()
		{
			var search = toolStripButtonSearch.Text.Trim().ToLower().Replace( '&', ' ' ).Replace( '\t', ' ' );
			search = new string( search.Where( c => !char.IsControl( c ) ).ToArray() );
			return search;
		}

		void PerformRefreshFromStores( bool updateLogin )
		{
			if( updateLogin )
			{
				LoginUtility.RequestFullLicenseInfo();
				Thread.Sleep( 500 );
			}

			UpdateList( true, true );

			StoreManager.ClearDownloadedListOfPackages();

			//begin request update from stores

			var search = GetSearchText();
			var filterSettings2 = filterSettings.Clone();

			foreach( var store in StoreManager.Stores )
			{
				if( GetSelectedStore() == store || GetSelectedStore() == null )
					store.Implementation.StartRefreshPackageList( search, filterSettings2 );
				else
					store.Implementation.StopCurrentTask();
			}
		}

		//string GetDateAsString( string value )
		//{
		//	var v2 = value.Replace( "-", "" );
		//	DateTime dt = DateTime.ParseExact( v2, "yyyyMMdd", CultureInfo.InvariantCulture );
		//	return dt.ToString( "dd MMMM yyyy", CultureInfo.InvariantCulture );
		//	//return dt.ToString();
		//}

		void StartDownload( string packageId, bool installAfterDownload, bool openAfterInstall, bool autoImport = false )
		{
			var package = GetPackage( packageId, true );
			if( package == null )
				return;
			var state = GetPackageState( packageId );
			state.AutoImport = autoImport;

			//check already downloading
			if( !string.IsNullOrEmpty( state.downloadingAddress ) )
				return;

			if( state.CanDownload && !state.Downloaded )
			{
				if( !string.IsNullOrEmpty( package.FreeDownload ) )
				{
					state.downloadingAddress = package.FreeDownload;

					var fileName = Path.GetFileName( state.downloadingAddress );
					if( Path.GetExtension( fileName ) != ".neoaxispackage" )
						fileName = $"{package.Identifier}-{package.Version}.neoaxispackage";

					state.downloadingDestinationPath = Path.Combine( PackageManager.PackagesFolder, fileName );
					//state.downloadingDestinationPath = Path.Combine( PackageManager.PackagesFolder, Path.GetFileName( state.downloadingAddress ) );
				}
				else if( package.SecureDownload )
				{
					if( !LoginUtility.GetCurrentLicense( out var email, out var hash ) )
						return;

					var productIdentifier = package.Identifier;
					var version = package.Version;

					var email64 = StringUtility.EncodeToBase64URL( email );
					var hash64 = StringUtility.EncodeToBase64URL( hash );
					var product64 = StringUtility.EncodeToBase64URL( productIdentifier );
					var action64 = StringUtility.EncodeToBase64URL( "download" );
					//var email64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( email/*.ToLower()*/ ) ).Replace( "=", "" );
					//var hash64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( hash ) ).Replace( "=", "" );
					//var product64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( productIdentifier ) ).Replace( "=", "" );
					//var action64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( "download" ) ).Replace( "=", "" );

					//var version64 = Convert.ToBase64String( Encoding.UTF8.GetBytes( version ) ).Replace( "=", "" );

					state.downloadingAddress = EngineInfo.StoreAddress + $@"/api/secure_download/?email={email64}&hash={hash64}&product={product64}&action={action64}";

					var fileName = $"{productIdentifier}-{version}.neoaxispackage";
					state.downloadingDestinationPath = Path.Combine( PackageManager.PackagesFolder, fileName );
				}

				state.downloadProgress = 0;
				state.downloadingInstallAfterDownload = installAfterDownload;

				Thread thread1 = new Thread( ThreadDownload );
				thread1.IsBackground = true;
				thread1.Start( (packageId, openAfterInstall, package) );
			}
			else if( state.Downloaded && !state.Installed )
				StartInstall( packageId, openAfterInstall );
		}

		void CancelInstallation( string packageId )
		{
			var state = GetPackageState( packageId );

			//state.downloadingClient?.CancelAsync();
			state.downloadingDownloader?.CancelAsync();

			//state.downloadingCancel = true;
			//Thread.Sleep( 500 );

			state.downloadingAddress = "";
			//state.downloadingDestinationPath = "";
			state.downloadProgress = 0;
			state.downloadingInstallAfterDownload = false;
			//state.downloadingClient = null;
			state.downloadingDownloader = null;
			//state.downloadingCancel = false;
		}

		public class ThreadDownloadData
		{
			public PackageManager.PackageInfo Package;
			public PackageState State;
			public bool Cancelled;
			public Exception Error;
			public bool ShowWarningAnyway;
		}

		void ThreadDownload( object obj )
		{
			var (packageId, openAfterInstall, package) = ((string, bool, PackageManager.PackageInfo))obj;
			var state = GetPackageState( packageId );

			var data = new ThreadDownloadData();
			data.Package = package;
			data.State = state;

			try
			{
				package.Store?.Implementation?.ThreadDownloadBody( data );
			}
			catch( Exception e )
			{
				if( !e.Message.Contains( "A task may only be disposed if it is in a completion state (RanToCompletion, Faulted or Canceled)." ) )
					Log.Warning( e.Message );

				data.Error = e;

				return;
			}
			finally
			{
				try
				{
					if( !data.Cancelled )
					{
						if( File.Exists( state.downloadingDestinationPath ) && new FileInfo( state.downloadingDestinationPath ).Length == 0 )
						{
							File.Delete( state.downloadingDestinationPath );
							data.Cancelled = true;

							data.Error = new Exception( "Invalid package (empty file)." );
							data.ShowWarningAnyway = true;
						}
					}
					if( !data.Cancelled && !File.Exists( state.downloadingDestinationPath ) )
						data.Cancelled = true;

					if( data.Cancelled || data.Error != null )
					{
						if( File.Exists( state.downloadingDestinationPath ) )
							File.Delete( state.downloadingDestinationPath );
					}
				}
				catch { }

				if( data.Error != null && !data.Cancelled || data.ShowWarningAnyway )
					Log.Warning( ( data.Error.InnerException ?? data.Error ).Message );

				var installAfterDownload = state.downloadingInstallAfterDownload;

				state.downloadingAddress = "";
				state.downloadingDestinationPath = "";
				state.downloadProgress = 0;
				state.downloadingInstallAfterDownload = false;
				//state.downloadingClient = null;
				state.downloadingDownloader = null;
				//state.downloadingCancel = false;

				//auto install after download
				if( installAfterDownload && !data.Cancelled )
				{
					lock( needStartInstallPackages )
						needStartInstallPackages.Add( (packageId, openAfterInstall) );
				}
			}

			if( !data.Cancelled )
			{
				if( data.Error != null )
					ScreenNotifications2.Show( EditorLocalization2.Translate( "General", "Error downloading the package." ), true );
				else
					ScreenNotifications2.Show( EditorLocalization2.Translate( "General", "The package has been successfully downloaded." ) );
			}
		}

		static List<string> ExtractToDirectory( string zipPath, string destinationFolder, bool overwriteExisting )
		{
			var extractedFiles = new List<string>();

			using( var archive = ZipFile.OpenRead( zipPath ) )
			{
				foreach( var entry in archive.Entries )
				{
					// Skip directories
					if( entry.FullName.EndsWith( "/" ) || entry.FullName.EndsWith( "\\" ) )
						continue;

					var filePath = PathUtility.NormalizePath( Path.Combine( destinationFolder, entry.FullName ) );

					// Ensure the directory is created
					var directoryPath = Path.GetDirectoryName( filePath );
					if( !Directory.Exists( directoryPath ) )
						Directory.CreateDirectory( directoryPath );

					// Extract file
					if( overwriteExisting || !File.Exists( filePath ) )
						entry.ExtractToFile( filePath, overwriteExisting );

					// Add to the list of extracted files
					extractedFiles.Add( filePath );
				}
			}

			return extractedFiles;
		}


		static void DownloadAndExtractExternalSource( string address, string destinationFolder, List<string> extractedFullPaths )
		{
			using( WebClient client = new WebClient() )
			{
				var tempFileName = Path.Combine( Path.GetTempPath(), "Temp9" + Path.GetRandomFileName() );

				client.DownloadFile( address, tempFileName );

				//extract archive
				var extractedFiles = ExtractToDirectory( tempFileName, destinationFolder, true );
				extractedFullPaths.AddRange( extractedFiles );
				//ZipFile.ExtractToDirectory( tempFileName, destinationFolder, true );

				//delete temp file
				try
				{
					if( File.Exists( tempFileName ) )
						File.Delete( tempFileName );
				}
				catch { }
			}
		}

		//not works
		//static void DownloadAndExtractExternalSource( string address, string destinationFolder )
		//{
		//	var downloaderOptions = new DownloadConfiguration();
		//	downloaderOptions.MaxTryAgainOnFailover = 4;
		//	//long chunkSize = 30 * 1024 * 1024;
		//	//if( data.Package.Size > chunkSize )
		//	//	downloaderOptions.ChunkCount = (int)Math.Max( data.Package.Size / chunkSize + 1, 1 );

		//	using( var downloader = new DownloadService( downloaderOptions ) )
		//	{
		//		var tempFileName = Path.Combine( Path.GetTempPath(), "Temp9" + Path.GetRandomFileName() );

		//		//downloader.DownloadProgressChanged += delegate ( object sender, Downloader.DownloadProgressChangedEventArgs e )
		//		//{
		//		//	//check already ended
		//		//	if( data.Cancelled )
		//		//		return;

		//		//	if( e.TotalBytesToReceive != 0 )
		//		//		state.downloadProgress = MathEx.Saturate( (float)e.ReceivedBytesSize / (float)e.TotalBytesToReceive );
		//		//};

		//		downloader.DownloadFileCompleted += delegate ( object sender, AsyncCompletedEventArgs e )
		//		{
		//			if( e.Error != null )
		//				throw new Exception( e.Error.Message );
		//			if( e.Cancelled )
		//				throw new Exception( "Download cancelled" );

		//			//extract archive
		//			ZipFile.ExtractToDirectory( tempFileName, destinationFolder, true );

		//			//delete temp file
		//			try
		//			{
		//				if( File.Exists( tempFileName ) )
		//					File.Delete( tempFileName );
		//			}
		//			catch { }
		//		};

		//		using( var task = downloader.DownloadFileTaskAsync( address, tempFileName ) )
		//		{
		//			while( /*!string.IsNullOrEmpty( address ) && */ !task.Wait( 10 ) )
		//			{
		//			}
		//		}
		//	}
		//}

		void StartInstall( string packageId, bool openAfterInstall )
		{
			var package = GetPackage( packageId, true );
			if( package == null )
				return;

			var archiveInfo = PackageManager.ReadPackageArchiveInfo( package.FullFilePath, out var error );
			if( archiveInfo == null )
			{
				//Download & Install
				StartDownload( packageId, true, openAfterInstall );
				return;
			}

			var filesToCopy = new ESet<string>();
			foreach( var file in archiveInfo.Files )
			{
				filesToCopy.AddWithCheckAlreadyContained( file );
				//var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );
				//filesToCopy.AddWithCheckAlreadyContained( fullName );
			}

			var existsFiles = new List<string>();
			foreach( var file in filesToCopy )
			{
				var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );
				if( File.Exists( fullName ) )
					existsFiles.Add( file );
			}

			if( existsFiles.Count != 0 )
			{
				var text = $"{existsFiles.Count} files already exist. Overwrite?";
				text += "\r\n";

				int counter = 0;
				foreach( var file in filesToCopy )
				{
					text += "\r\n";
					if( counter > 10 )
					{
						text += "...";
						break;
					}
					text += file;
					counter++;
				}

				if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.OKCancel ) == EDialogResult.Cancel )
					return;
			}

			//var text = string.Format( Translate( "Install {0}?\n\n{1} files will created." ), package.Title, filesToCopy.Count );
			//if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) != EDialogResult.Yes )
			//	return;

			var notification = ScreenNotifications2.ShowSticky( "Installing the package..." );

			//disable file watcher for preview images
			PreviewImagesManager.EnableVirtualFileWatcherUpdate = false;

			var extractedFullPaths = new List<string>();

			try
			{
				using( var archive = ZipFile.OpenRead( package.FullFilePath ) )
				{
					foreach( var entry in archive.Entries )
					{
						var fileName = entry.FullName;
						bool directory = fileName[ fileName.Length - 1 ] == '/';
						if( fileName != "Package.info" && !directory )
						{
							var fullPath = Path.Combine( VirtualFileSystem.Directories.Project, fileName );

							var directoryName = Path.GetDirectoryName( fullPath );
							if( !Directory.Exists( directoryName ) )
								Directory.CreateDirectory( directoryName );

							entry.ExtractToFile( fullPath, true );

							extractedFullPaths.Add( fullPath );
						}
					}
				}

				PackageManager.ChangeInstalledState( package.Identifier, true );

				//process updated files by file watcher
				Thread.Sleep( 1000 );
				VirtualFileWatcher.ProcessEvents();
			}
			catch( Exception e2 )
			{
				EditorMessageBox.ShowWarning( e2.Message );
				return;
			}
			finally
			{
				//restore file watcher for preview images
				PreviewImagesManager.EnableVirtualFileWatcherUpdate = true;

				notification.Close();
			}

			//ExternalSource.info
			try
			{
				var extractedFullPathsCopy = extractedFullPaths.ToArray();

				foreach( var fullPath in extractedFullPathsCopy )
				{
					if( Path.GetFileName( fullPath ).ToLower() == "externalsource.info" )
					{
						var b = TextBlockUtility.LoadFromRealFile( fullPath );
						var address = b.GetAttribute( "Source" );

						if( string.IsNullOrEmpty( address ) )
							throw new Exception( string.Format( "Invalid \"{0}\". Empty \"Source\" attribute.", Path.GetFileName( fullPath ) ) );

						var text = "The package requires content from an external source. Download?\r\n\r\nAddress:\r\n" + address;
						if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
						{
							var destinationFolder = Path.GetDirectoryName( fullPath );

							var notification2 = ScreenNotifications2.ShowSticky( "Downloading from external source..." );
							try
							{
								DownloadAndExtractExternalSource( address, destinationFolder, extractedFullPaths );
							}
							finally
							{
								notification2.Close();
							}
						}
					}
				}
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				Log.Warning( e.Message );
			}

			//C# files
			var addedCSFilesToAssetsReal = new List<string>();
			var addedCSFilesToAssetsVirtual = new List<string>();
			try
			{
				foreach( var fullPath in extractedFullPaths )
				{
					var virtualPath = VirtualPathUtility.GetVirtualPathByReal( fullPath );
					if( !string.IsNullOrEmpty( virtualPath ) && Path.GetExtension( fullPath ).ToLower() == ".cs" )
					{
						addedCSFilesToAssetsReal.Add( fullPath );
						addedCSFilesToAssetsVirtual.Add( virtualPath );
					}
				}
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				Log.Warning( e.Message );
			}

			if( addedCSFilesToAssetsReal.Count != 0 )//if( !string.IsNullOrEmpty( archiveInfo.AddCSharpFilesToProject ) )
			{
				var toAdd = new ESet<string>();
				toAdd.AddRangeWithCheckAlreadyContained( addedCSFilesToAssetsReal );

				//var path = Path.Combine( VirtualFileSystem.Directories.Assets, archiveInfo.AddCSharpFilesToProject );
				//if( Directory.Exists( path ) )
				//{
				//	var fullPaths = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true );
				//	var files = Directory.GetFiles( path, "*.cs", SearchOption.AllDirectories );
				//	foreach( var file in files )
				//	{
				//		if( !fullPaths.Contains( file ) )
				//			toAdd.AddWithCheckAlreadyContained( file );
				//	}
				//}

				////	if( !fileItem.IsDirectory && Path.GetExtension( fileItem.FullPath ).ToLower() == ".cs" )
				////	{
				////		bool added = CSharpProjectFileUtility.GetProjectFileCSFiles( false, true ).Contains( fileItem.FullPath );
				////		if( !added )
				////			toAdd.Add( fileItem.FullPath );
				////	}

				//if( toAdd.Count != 0 )
				//{

				var text = string.Format( toAdd.Count == 1 ? "{0} C# file was added. Add it to the Project.csproj?" : "{0} C# files were added. Add them to the Project.csproj?", toAdd.Count ) + "\r\n\r\n";
				var counter = 0;
				foreach( var item in addedCSFilesToAssetsVirtual )
				{
					if( counter > 10 )
					{
						text += "...";
						break;
					}
					text += item + "\r\n";
					counter++;
				}
				if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
				{
					if( CSharpProjectFileUtility.UpdateProjectFile( toAdd, null, out var error2 ) )
					{
						if( toAdd.Count > 1 )
							Log.Info( EditorLocalization2.Translate( "General", "Items have been added to the Project.csproj." ) );
						else
							Log.Info( EditorLocalization2.Translate( "General", "The item has been added to the Project.csproj." ) );

						EditorAPI2.BuildProjectSolution( false );
					}
					else
						Log.Warning( error2 );
				}

				//}
			}

			//load .neoaxisbaking files
			foreach( var path in extractedFullPaths )
			{
				try
				{
					var extension = Path.GetExtension( path ).ToLower();
					if( extension == ".neoaxisbaking" )
						ArchiveManager.LoadBakingFile( path );
				}
				catch( Exception e )
				{
					Log.Warning( e.Message );
				}
			}

			//!!!!может чистить файлы в нём
			////delete archive file
			//{
			//	try
			//	{
			//		if( File.Exists( package.FullFilePath ) )
			//			File.Delete( package.FullFilePath );
			//	}
			//	catch( Exception e2 )
			//	{
			//		EditorMessageBox.ShowWarning( e2.Message );
			//	}
			//}

			if( archiveInfo.MustRestart )
				ShowRestartLabel();

			//open after install
			if( openAfterInstall && !string.IsNullOrEmpty( archiveInfo.OpenAfterInstall ) )
			{
				var realFileName = VirtualPathUtility.GetRealPathByVirtual( archiveInfo.OpenAfterInstall );

				if( archiveInfo.MustRestart )
				{
					EditorSettingsSerialization.OpenFileAtStartup = realFileName;
				}
				else
				{
					EditorAPI2.SelectFilesOrDirectoriesInMainResourcesWindow( new string[] { realFileName }, Directory.Exists( realFileName ) );
					EditorAPI2.OpenFileAsDocument( realFileName, true, true );
				}
			}

			ScreenNotifications2.Show( EditorLocalization2.Translate( "General", "The package has been successfully installed." ) );

			//restart application
			if( archiveInfo.MustRestart )
			{
				var text2 = EditorLocalization2.Translate( "General", "To apply changes need restart the editor. Restart?" );
				if( EditorMessageBox.ShowQuestion( text2, EMessageBoxButtons.YesNo ) == EDialogResult.Yes )
					EditorAPI2.BeginRestartApplication();
			}

			//auto import when drop to scene
			var state = GetPackageState( packageId );
			if( state != null && state.AutoImport )
			{
				try
				{
					foreach( var file in archiveInfo.Files )
					{
						var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );
						var virtualPath = VirtualPathUtility.GetVirtualPathByReal( fullName );

						var extension = Path.GetExtension( virtualPath );

						var type = ResourceManager.GetTypeByFileExtension( extension );
						if( type != null && type.Name == "Import 3D" )
						{
							var resource = ResourceManager.LoadResource( virtualPath, true );

							var import3D = resource?.ResultComponent as Import3D;
							if( import3D != null )
								import3D.DoAutoUpdate();
						}
					}
				}
				catch( Exception e )
				{
					EditorMessageBox.ShowWarning( e.Message );
				}
			}

			//!!!!
			//PackageManager.GetPackagesInfoByFileArchives( true );
			//PackageManager.GetInstalledPackages( true );
			//needUpdateListTime = EngineApp.GetSystemTime();
		}

		bool IsDirectoryEmpty( string path )
		{
			return !Directory.EnumerateFileSystemEntries( path ).Any();
		}

		bool StartUninstall( string packageId )
		{
			var package = GetPackage( packageId, true );
			if( package == null )
				return false;

			var filesToDelete = new List<string>();
			bool mustRestart = false;

			//get list of files to delete
			if( !string.IsNullOrEmpty( package.FullFilePath ) && File.Exists( package.FullFilePath ) )
			{
				//get list of files from the package archive if exists

				var archiveInfo = PackageManager.ReadPackageArchiveInfo( package.FullFilePath, out var error );
				if( archiveInfo == null )
				{
					ScreenNotifications2.Show( "Could not read the package info.", true );
					Log.Warning( error );
					return false;
				}

				foreach( var file in archiveInfo.Files )
				{
					var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );
					if( File.Exists( fullName ) )
						filesToDelete.Add( file );
				}

				mustRestart = archiveInfo.MustRestart;
			}
			else
			{
				//get list of files from selectedPackage.Files in case when the archive file is not exists

				foreach( var file in package.GetFiles() )
				{
					var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );
					if( File.Exists( fullName ) )
						filesToDelete.Add( file );
				}

				//!!!!mustRestart
			}

			//if( filesToDelete.Count == 0 )
			//	return false;

			var text = string.Format( Translate( "Uninstall {0}?\n\n{1} files will deleted." ), package.Title, filesToDelete.Count );
			if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) != EDialogResult.Yes )
				return false;

			var filesToDeletionAtStartup = new List<string>();

			try
			{
				//remove cs files from Project.csproj
				try
				{
					var toRemove = new List<string>();

					foreach( var file in filesToDelete )
					{
						var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );
						if( Path.GetExtension( fullName ).ToLower() == ".cs" )
							toRemove.Add( VirtualPathUtility.NormalizePath( fullName ) );
					}

					if( toRemove.Count != 0 )
						CSharpProjectFileUtility.UpdateProjectFile( null, toRemove, out _ );
				}
				catch { }

				//unload .neoaxisbaking files
				foreach( var fileName in filesToDelete )
				{
					try
					{
						var extension = Path.GetExtension( fileName ).ToLower();
						if( extension == ".neoaxisbaking" )
						{
							var path = Path.Combine( VirtualFileSystem.Directories.Project, fileName );
							ArchiveManager.UnloadBakingFile( path );
						}
					}
					catch( Exception e )
					{
						Log.Warning( e.Message );
					}
				}

				//delete files
				foreach( var file in filesToDelete )
				{
					var fullName = Path.Combine( VirtualFileSystem.Directories.Project, file );

					try
					{
						File.Delete( fullName );
					}
					catch( UnauthorizedAccessException )
					{
						filesToDeletionAtStartup.Add( file );
					}
					catch( IOException )
					{
						filesToDeletionAtStartup.Add( file );
					}
				}

				//delete empty folders
				{
					var allFolders = new ESet<string>();
					foreach( var file in filesToDelete )
					{
						var f = Path.GetDirectoryName( file );
						while( !string.IsNullOrEmpty( f ) )
						{
							allFolders.AddWithCheckAlreadyContained( f );
							f = Path.GetDirectoryName( f );
						}
					}

					var list = allFolders.ToArray();
					CollectionUtility.MergeSort( list, delegate ( string f1, string f2 )
					{
						var levels1 = f1.Split( new char[] { '\\' } ).Length;
						var levels2 = f2.Split( new char[] { '\\' } ).Length;
						return levels2 - levels1;
					} );

					foreach( var folder in list )
					{
						var fullName = Path.Combine( VirtualFileSystem.Directories.Project, folder );

						if( Directory.Exists( fullName ) && IsDirectoryEmpty( fullName ) )
							Directory.Delete( fullName );
					}
				}

				PackageManager.ChangeInstalledState( package.Identifier, false );
			}
			catch( Exception e2 )
			{
				EditorMessageBox.ShowWarning( e2.Message );
				return false;
			}

			if( filesToDeletionAtStartup.Count != 0 )
				PackageManager.AddFilesToDeletionAtStartup( filesToDeletionAtStartup );

			if( mustRestart )
				ShowRestartLabel();

			ScreenNotifications2.Show( EditorLocalization2.Translate( "General", "The package has been successfully uninstalled." ) );

			//!!!!
			//StoreManager.ImageManager.Clear();
			////PackageManager.GetPackagesInfoByFileArchives( true );
			//PerformRefreshFromStores( false );
			//PackageManager.GetPackagesInfoByFileArchives( true );
			//PackageManager.GetInstalledPackages( true );
			//needUpdateListTime = EngineApp.GetSystemTime();

			return true;
		}

		void TryStartUninstall( string packageId, bool deletePackageArchive )
		{
			var package = GetPackage( packageId, true );
			if( package == null )
				return;

			bool installed = PackageManager.IsInstalled( packageId, true );

			//Uninstall & Delete
			var skipDeleteMessage = false;
			if( installed )
			{
				if( !StartUninstall( packageId ) )
					return;
				skipDeleteMessage = true;
			}

			if( deletePackageArchive )
			{
				//check can delete
				if( !File.Exists( package.FullFilePath ) )
					return;

				//ask to delete
				if( !skipDeleteMessage )
				{
					var template = Translate( "Are you sure you want to uninstall \'{0}\'?" );
					var text = string.Format( template, package.FullFilePath );
					if( EditorMessageBox.ShowQuestion( text, EMessageBoxButtons.YesNo ) == EDialogResult.No )
						return;
				}

				//delete
				try
				{
					File.Delete( package.FullFilePath );
				}
				catch( Exception e2 )
				{
					EditorMessageBox.ShowWarning( e2.Message );
					return;
				}
			}
		}

		private void EditorAPI_ClosingApplicationChanged()
		{
			if( EditorAPI.ClosingApplication )
			{
				foreach( var state in packageStates.Values.ToArray() )
				{
					try
					{
						//state.downloadingClient?.CancelAsync();
						state.downloadingDownloader?.CancelAsync();
					}
					catch { }
				}
			}
		}

		public void NeedSelectPackage( string packageId, bool install )
		{
			needSelectPackage = packageId;
			needSelectPackageInstall = install;
		}

		void UpdateListItems()
		{
#if !DEPLOY
			var time = EngineApp.GetSystemTime();

			var visibleItems = new ESet<ContentBrowser.Item>( contentBrowser1.GetVisibleItemsByListView() );

			foreach( var item in contentBrowser1.GetAllItemsByItemHierarchy( true ) )
			{
				var storeItem = item as ContentBrowserItem_StoreItem;
				if( storeItem != null )
				{
					if( visibleItems.Contains( storeItem ) )
					{
						var package = GetPackage( storeItem.packageId, false );

						if( package != null && !string.IsNullOrEmpty( package.Thumbnail ) )
						{
							var image = StoreManager.ImageManager.GetSquareImage( package.Thumbnail, time );
							if( image != null )
							{
								var state = GetPackageState( storeItem.packageId );

								//update image state
								if( state.Installed )
								{
									storeItem.stateProgress = 1;
									storeItem.stateColor = Color.FromArgb( 0, 255, 0 );
								}
								else if( state.Downloading )
								{
									storeItem.stateProgress = state.downloadProgress;
									storeItem.stateColor = Color.FromArgb( 255, 255, 0 );
								}
								else if( state.Downloaded )
								{
									storeItem.stateProgress = 1;
									storeItem.stateColor = Color.FromArgb( 255, 255, 0 );
								}
								else
								{
									storeItem.stateProgress = 0;
									storeItem.stateColor = Color.FromArgb( 0, 0, 0, 0 );
								}

								//update image
								if( storeItem.createdImage == null || storeItem.stateProgress != storeItem.createdImageStateProgress || storeItem.stateColor != storeItem.createdImageStateColor )
								{
									storeItem.createdImage = new Bitmap( image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
									storeItem.createdImageStateProgress = storeItem.stateProgress;
									storeItem.createdImageStateColor = storeItem.stateColor;

									using( var g = Graphics.FromImage( storeItem.createdImage ) )
									{
										g.CompositingMode = CompositingMode.SourceOver;
										g.DrawImage( image, 0, 0 );

										if( storeItem.createdImageStateProgress > 0 && storeItem.createdImageStateColor.A > 0 )
										{
											using( var brush = new SolidBrush( storeItem.createdImageStateColor ) )
											{
												var size = storeItem.createdImage.Size;

												var toX = (int)( storeItem.createdImageStateProgress * size.Width );
												var fromY = (int)( 0.93 * size.Height );

												g.FillRectangle( brush, 0, fromY, toX, size.Height - fromY );
											}
										}

										//store icon
										var store = StoreManager.GetPackageStore( storeItem.packageId );
										if( store != null )
										{
											var icon = store.Icon32;
											var size = storeItem.createdImage.Width / 7;
											g.DrawImage( icon, new System.Drawing.RectangleF( size / 6, size / 6, size, size ), new System.Drawing.RectangleF( 0, 0, icon.Width, icon.Height ), GraphicsUnit.Pixel );
										}

										//money icon
										if( package.CostNumber > 0 )
										{
											var icon = Properties.Resources.Money_32;//MoneyGrayscale_32;
											var size = storeItem.createdImage.Width / 7;
											g.DrawImage( icon, new System.Drawing.RectangleF( storeItem.createdImage.Width - size, size / 6, size, size ), new System.Drawing.RectangleF( 0, 0, icon.Width, icon.Height ), GraphicsUnit.Pixel );
										}
									}

									storeItem.imageKey = null;
									storeItem.image = storeItem.createdImage;

									//!!!!не все обновлять
									contentBrowser1.needUpdateImages = true;
								}

								//update last used time
								storeItem.createdImageLastUsedTime = time;
							}
						}
					}
					else
					{
						//10 seconds
						if( storeItem.createdImage != null && storeItem.createdImageLastUsedTime + 10 < time )
						{
							storeItem.imageKey = null;
							storeItem.image = null;
							storeItem.createdImage?.Dispose();
							storeItem.createdImage = null;

							storeItem.imageKey = "Default_512";

							//!!!!не все обновлять
							contentBrowser1.needUpdateImages = true;
						}
					}
				}
			}
#endif
		}

		PackageState GetPackageState( string packageId )
		{
			PackageState state = null;
			lock( packageStates )
			{
				if( !packageStates.TryGetValue( packageId, out state ) )
				{
					state = new PackageState();
					state.storesWindow = this;
					state.packageId = packageId;
					packageStates[ packageId ] = state;
				}
			}
			return state;
		}

		public PackageManager.PackageInfo GetPackage( string packageId, bool updateDownloadedPackages, bool needDetailedInfo = false )
		{
			PackageManager.PackageInfo result = null;

			//package data from archive
			{
				var packages = PackageManager.GetPackagesInfoByFileArchives( updateDownloadedPackages );
				if( packages.TryGetValue( packageId, out var package ) )
					result = package;

				result?.UpdateDataFromArchive();
			}

			//!!!!needDetailedInfo
			//!!!!если архив скачан, то качать список файлов не нужно

			//package data from stores
			{
				var package = StoreManager.GetPackageInfo( packageId, needDetailedInfo );
				if( package != null )
				{
					if( result != null )
						result = PackageManager.PackageInfo.Merge( package, result );
					else
						result = package;
				}
			}

			return result;
		}

		bool CheckShowItemWithSearchFiltering( string[] searchWords, PackageManager.PackageInfo package )
		{
			if( searchWords.Length != 0 )
			{
				foreach( var searchWord in searchWords )
				{
					if( !package.Title.ToLower().Contains( searchWord ) )
						return false;
				}

				//!!!!
				////!!!!slowly
				//if( package.Tags != null )
				//{
				//	foreach( var tag2 in package.Tags.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries ) )
				//	{
				//		var tag = tag2.Trim();
				//		if( tag.ToLower().Contains( filter ) )
				//			return true;
				//	}
				//}
			}

			return true;
		}

		public ESet<string> GetAllPackages( bool updateDownloadedPackages )
		{
			var result = new ESet<string>();

			////package data from archive
			//foreach( var packageId in PackageManager.GetPackagesInfoByFileArchives( updateDownloadedPackages ).Keys )
			//	result.AddWithCheckAlreadyContained( packageId );

			//package data from stores
			foreach( var packageId in StoreManager.GetPackages() )
				result.AddWithCheckAlreadyContained( packageId );


			//package data from archives

			var search = GetSearchText();
			var searchWords = search.Split( ' ' );

			foreach( var packageId in PackageManager.GetPackagesInfoByFileArchives( updateDownloadedPackages ).Keys )
			{
				var package = GetPackage( packageId, false );

				if( package != null )
				{
					if( !CheckShowItemWithSearchFiltering( searchWords, package ) )
						continue;

					if( selectedStore != null )
					{
						var packageStore = package.Store ?? StoreManager.Stores[ 0 ];
						if( selectedStore != packageStore )
							continue;
					}
				}

				result.AddWithCheckAlreadyContained( packageId );
			}

			return result;
		}

		public StoreManager.StoreItem GetSelectedStore()
		{
			var result = selectedStore;
			//if( result == null )
			//	result = StoreManager.DefaultStore;
			return result;
		}

		void UpdateStoreImplemetationToGetNextItems()
		{
			var needGetNextItemsForStores = new List<StoreManager.StoreItem>();

			//if exists items of selected store, and also the last one item is visible.

			var storeItems = contentBrowser1.GetItemsByListView().Where( i => i is ContentBrowserItem_StoreItem ).Cast<ContentBrowserItem_StoreItem>();
			var visibleItems = new ESet<ContentBrowser.Item>( contentBrowser1.GetVisibleItemsByListView() );

			//enumerate stores
			for( int n = 0; n < StoreManager.Stores.Count; n++ )
			{
				var store = StoreManager.Stores[ n ];

				//foreach from the end
				foreach( var storeItem in storeItems.Reverse() )
				{
					//find item of the store
					var storeOfItem = StoreManager.GetPackageStore( storeItem.packageId );
					if( store == storeOfItem )
					{
						//check item is visible
						if( visibleItems.Contains( storeItem ) )
							needGetNextItemsForStores.Add( store );

						break;
					}
				}
			}

			lock( StoreManager.needGetNextItemsForStores )
			{
				StoreManager.needGetNextItemsForStores.Clear();
				StoreManager.needGetNextItemsForStores.AddRange( needGetNextItemsForStores );
			}
		}

		private void toolStripButtonFilter_Click( object sender, EventArgs e )
		{
			//!!!!SpecifyParametersForm: кнопка для сброса всех в дефолты

			var settings = filterSettings.Clone();

			var form = new SpecifyParametersForm( "Filters", settings );
			form.Size = new Size( form.Size.Width, (int)( (double)form.Size.Height * 1.3 ) );

			if( form.ShowDialog() != DialogResult.OK )
				return;

			if( !filterSettings.Equals( settings ) )
			{
				filterSettings = settings;

				needUpdateListReset = true;
				needUpdateListRefreshFromStores = true;
				needUpdateListTime = EngineApp.GetSystemTime();
			}
		}

		[Browsable( false )]
		public StoreManager.FilterSettingsClass FilterSettings
		{
			get { return filterSettings; }
		}

	}
}

#endif