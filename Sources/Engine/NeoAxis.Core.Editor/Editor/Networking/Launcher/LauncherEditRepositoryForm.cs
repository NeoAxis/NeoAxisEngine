#if CLOUD
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using NeoAxis.Networking;
using Internal.ComponentFactory.Krypton.Toolkit;
using System.IO;
using System.Diagnostics;

namespace NeoAxis.Editor
{
	public partial class LauncherEditRepositoryForm : EngineForm
	{
		static LauncherEditRepositoryForm instance;

		//bool firstTick = true;
		public long projectID;
		public string defaultProjectFolder;

		public RepositoryFileWatcher fileWatcher;
		public string projectFolder;

		public delegate void ChangeProjectFolderCallbackDelegate( LauncherEditRepositoryForm sender, string projectFolder );
		public event ChangeProjectFolderCallbackDelegate ChangeProjectFolderCallback;
		public bool reopen;

		///////////////////////////////////////////////

		public class LauncherContentBrowserOptions : ContentBrowserOptions
		{
			public LauncherContentBrowserOptions( ContentBrowser owner )
				: base( owner )
			{
			}

			protected override void MetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
			{
				var p = member as Metadata.Property;
				if( p != null )
				{
					if( p.Name == nameof( FilteringModeButton ) )
						skip = true;
					if( p.Name == nameof( OpenButton ) )
						skip = true;
					if( p.Name == nameof( ButtonsForEditing ) )
						skip = true;
				}

				base.MetadataGetMembersFilter( context, member, ref skip );
			}
		}

		///////////////////////////////////////////////

		public static LauncherEditRepositoryForm Instance
		{
			get { return instance; }
		}

		public LauncherEditRepositoryForm()
		{
			instance = this;

			InitializeComponent();

			toolStripButtonOptions.Image = EditorResourcesCache.Options;
			toolStripButtonGet.Image = EditorResourcesCache.MoveDown;
			toolStripButtonCommit.Image = EditorResourcesCache.MoveUp;

			foreach( var item in toolStripForTreeView.Items )
			{
				var button = item as ToolStripButton;
				if( button != null )
					button.Text = EditorLocalization2.Translate( "LauncherEditRepositoryForm", button.Text );

				var button2 = item as ToolStripDropDownButton;
				if( button2 != null )
					button2.Text = EditorLocalization2.Translate( "LauncherEditRepositoryForm", button2.Text );
			}

			toolStripForTreeView.Renderer = EditorThemeUtility.GetToolbarToolStripRenderer();

			contentBrowser1.Init( null, null, /*data, */null );
			contentBrowser1.Options = new LauncherContentBrowserOptions( contentBrowser1 );
			contentBrowser1.Options.PanelMode = ContentBrowser.PanelModeEnum.TwoPanelsSplitVertically;
			contentBrowser1.Options.SplitterPosition = 2.0 / 5.0;
			contentBrowser1.ShowToolBar = false;
			contentBrowser1.Options.FilteringModeButton = false;
			contentBrowser1.Options.MembersButton = false;
			contentBrowser1.Options.OpenButton = false;
			contentBrowser1.Options.EditorButton = false;
			contentBrowser1.Options.SettingsButton = false;
			contentBrowser1.Options.ButtonsForEditing = false;
			contentBrowser1.Options.SearchButton = false;
			contentBrowser1.Options.DisplayPropertiesEditorSettingsButtons = false;
			contentBrowser1.FilteringMode = new LauncherEditRepositoryContentBrowserFilteringMode();
			contentBrowser1.UpdateDataEvent += ContentBrowser_UpdateDataEvent;
			contentBrowser1.ShowContextMenuEvent += ContentBrowser_ShowContextMenuEvent;

			EditorThemeUtility.ApplyDarkThemeToForm( this );
		}

		public string GetProjectFolder()
		{
			return projectFolder;
			//return CloudProjectCommon.GetAppProjectFolder( projectID, true );
		}

		private void ContentBrowser_UpdateDataEvent( ContentBrowser sender, IList<ContentBrowser.Item> roots )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			//var projectFolder = CloudProjectCommon.GetAppProjectFolder( projectID, true );

			var dataItem = new LauncherEditRepositoryContentBrowserItem( this, sender, null, projectFolder, true );
			dataItem.SetText( EditorLocalization2.Translate( "ContentBrowser.Group", projectFolder ) );
			//dataItem.SetText( EditorLocalization2.Translate( "ContentBrowser.Group", "'Root'" ) );// "Files" ) );
			dataItem.imageKey = "Folder";
			dataItem.expandAtStartup = true;
			roots.Add( dataItem );
		}

		private void LauncherEditRepositoryForm_Load( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			UpdateControls();

			//if( EditorAPI2.DarkTheme )
			//	bordersContainer1.BorderColor = Color.FromArgb( 80, 80, 80 );

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
				toolStripForTreeView.Size = new Size( 1000 * (int)dpiScale, (int)( 21 * dpiScale + 2 ) );

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
				toolStripForTreeView.Size = new Size( 1000 * (int)dpiScale, (int)( 21 * dpiScale + 2 ) );
			}

			if( fileWatcher != null )
				fileWatcher.Update += FileWatcher_Update;

			timer1.Start();
		}

		private void LauncherEditRepositoryForm_FormClosing( object sender, FormClosingEventArgs e )
		{
			//if( DialogResult == DialogResult.OK )
			//{
			//	if( mustSelectSomething && GetCheckedItems().Length == 0 )
			//	{
			//		e.Cancel = true;
			//		return;
			//	}
			//}

			if( fileWatcher != null )
				fileWatcher.Update -= FileWatcher_Update;

			instance = null;
		}

		void UpdateControls()
		{
			buttonCancel.Location = new Point( ClientSize.Width - buttonCancel.Size.Width - DpiHelper.Default.ScaleValue( 12 ), ClientSize.Height - buttonCancel.Size.Height - DpiHelper.Default.ScaleValue( 12 ) );
			//buttonOK.Location = new Point( buttonCancel.Location.X - buttonOK.Size.Width - DpiHelper.Default.ScaleValue( 8 ), buttonCancel.Location.Y );
			//kryptonButtonSelectAll.Location = new Point( kryptonButtonSelectAll.Location.X, buttonOK.Location.Y );
			//kryptonButtonDeselectAll.Location = new Point( kryptonButtonDeselectAll.Location.X, buttonOK.Location.Y );
			//labelSelected.Location = new Point( labelSelected.Location.X, buttonOK.Location.Y + DpiHelper.Default.ScaleValue( 6 ) );

			//var parentSize = contentBrowser1.Parent.ClientSize;
			contentBrowser1.Location = new Point( contentBrowser1.Location.X, toolStripForTreeView.Location.Y + toolStripForTreeView.Height + 1 );
			//contentBrowser1.Size = new Size( parentSize.Width, parentSize.Height - toolStripForTreeView.Height );

			contentBrowser1.Size = new Size( ClientSize.Width - DpiHelper.Default.ScaleValue( 12 ) - contentBrowser1.Location.X, buttonCancel.Location.Y - DpiHelper.Default.ScaleValue( 8 ) - contentBrowser1.Location.Y );
			//bordersContainer1.Size = new Size( ClientSize.Width - DpiHelper.Default.ScaleValue( 12 ) - bordersContainer1.Location.X, buttonOK.Location.Y - DpiHelper.Default.ScaleValue( 8 ) - bordersContainer1.Location.Y );
			//contentBrowser.Size = new Size( ClientSize.Width - DpiHelper.Default.ScaleValue( 12 ) - contentBrowser.Location.X - 2, buttonOK.Location.Y - DpiHelper.Default.ScaleValue( 8 ) - contentBrowser.Location.Y - 2 );
		}

		protected override void OnResize( EventArgs e )
		{
			base.OnResize( e );

			if( IsHandleCreated )
				UpdateControls();
		}

		//protected override void OnHandleCreated( EventArgs e )
		//{
		//	base.OnHandleCreated( e );

		//	if( WinFormsUtility.IsDesignerHosted( this ) )
		//		return;

		//	KryptonWinFormsUtility.LockFormUpdate( this );
		//}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if( WinFormsUtility.IsDesignerHosted( this ) )
				return;

			EngineToolTipManager.Update();
			fileWatcher?.ProcessEvents();

			//if( firstTick )
			//	KryptonWinFormsUtility.LockFormUpdate( null );

			//firstTick = false;
		}

		string Translate( string text )
		{
			return EditorLocalization2.Translate( "ContentBrowser", text );
		}

		private void ContentBrowser_ShowContextMenuEvent( ContentBrowser sender, ContentBrowser.Item contentItem, List<KryptonContextMenuItemBase> items )
		{
			//LauncherEditRepositoryContentBrowserItem
			if( contentItem != null )
			{
				LauncherEditRepositoryContentBrowserItem currentOrParentDirectoryItem = null;
				var fileItem = contentItem as LauncherEditRepositoryContentBrowserItem;
				{
					if( fileItem != null )
					{
						if( !fileItem.IsDirectory )
						{
							var parent = fileItem.Parent as LauncherEditRepositoryContentBrowserItem;
							if( parent != null && parent.IsDirectory )
								currentOrParentDirectoryItem = parent;
						}
						else
							currentOrParentDirectoryItem = fileItem;
					}
				}

				if( fileItem != null )
				{
					////Open
					//if( !fileItem.IsDirectory )
					//{
					//	var item = new KryptonContextMenuItem( Translate( "Open" ), null, delegate ( object s, EventArgs e2 )
					//	{
					//		bool handled = false;
					//		OpenFile( fileItem, ref handled );
					//	} );

					//	//!!!!было
					//	//item.Font = new Font( item.Font, FontStyle.Bold );

					//	item.Enabled = EditorAPI2.IsDocumentFileSupport( fileItem.FullPath );
					//	//item.Enabled = currentOrParentDirectoryItem != null;
					//	items.Add( item );
					//}

					//Open with
					if( !fileItem.IsDirectory )
					{
						var item = new KryptonContextMenuItem( Translate( "Open" ), null, delegate ( object s, EventArgs e2 )
						{
							string filePath = fileItem.FullPath;
							try
							{
								Process.Start( "rundll32.exe", "shell32.dll, OpenAs_RunDLL " + filePath );
							}
							catch( Exception e )
							{
								EditorMessageBox.ShowWarning( e.Message );
							}
						} );
						item.Enabled = currentOrParentDirectoryItem != null || contentItem.Parent == null;
						items.Add( item );


						//KryptonContextMenuItem itemOpenWith = new KryptonContextMenuItem( Translate( "Open with" ), null );
						//itemOpenWith.Enabled = currentOrParentDirectoryItem != null || contentItem.Parent == null;

						//List<KryptonContextMenuItemBase> items2 = new List<KryptonContextMenuItemBase>();

						////External application
						//{
						//	var item = new KryptonContextMenuItem( Translate( "External app" ), null,
						//	delegate ( object s, EventArgs e2 )
						//	{
						//		string filePath = fileItem.FullPath;
						//		try
						//		{
						//			Process.Start( "rundll32.exe", "shell32.dll, OpenAs_RunDLL " + filePath );
						//		}
						//		catch( Exception e )
						//		{
						//			EditorMessageBox.ShowWarning( e.Message );
						//		}
						//	} );
						//	items2.Add( item );
						//}

						//itemOpenWith.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
						//items.Add( itemOpenWith );
					}

					if( fileItem.IsDirectory )
					{
						//separator
						if( items.Count != 0 )
							items.Add( new KryptonContextMenuSeparator() );

						////New folder
						//{
						//	var item = new KryptonContextMenuItem( Translate( "New Folder" ), EditorResourcesCache.NewFolder,
						//	   delegate ( object s, EventArgs e2 )
						//	   {
						//		   NewFolder();
						//	   } );
						//	item.Enabled = CanNewFolder( out _ );
						//	items.Add( item );
						//}

						////New resource or txt file when it outside Assets folder
						//{
						//	var canNewResource = CanNewResource( out var directoryPath );

						//	if( canNewResource )
						//	{
						//		EditorContextMenuWinForms.AddNewResourceItem( items, canNewResource/*, insideAssets*/, delegate ( Metadata.TypeInfo type, bool assetsFolderOnly )
						//		{
						//			NewResource( type, assetsFolderOnly );
						//		} );
						//	}
						//	else
						//	{
						//		var item = new KryptonContextMenuItem( Translate( "New Text File" ), EditorResourcesCache.New,
						//		   delegate ( object s, EventArgs e2 )
						//		   {
						//			   NewTextFileWhenOutsideAssets();
						//		   } );
						//		item.Enabled = CanNewTextFileWhenOutsideAssets( out _ );
						//		items.Add( item );
						//	}
						//}
					}

					//Open folder in Explorer
					if( fileItem.IsDirectory )
					{
						//separator
						if( items.Count != 0 )
							items.Add( new KryptonContextMenuSeparator() );

						var item = new KryptonContextMenuItem( Translate( "Open Folder in Explorer" ), null, delegate ( object s, EventArgs e2 )
						{
							string realPath = currentOrParentDirectoryItem.FullPath;
							Win32Utility.ShellExecuteEx( null, realPath );
						} );
						item.Enabled = currentOrParentDirectoryItem != null;
						items.Add( item );
					}

					//separator
					if( items.Count != 0 )
						items.Add( new KryptonContextMenuSeparator() );

					//repository
					{
						List<string> selectedFilesWithInsideSelectedFolders;
						//List<string> selectedFoldersRealPath = new List<string>();
						{
							var selectedFilesSet = new ESet<string>();

							foreach( var item in contentBrowser1.SelectedItems )
							{
								var fileItem2 = item as LauncherEditRepositoryContentBrowserItem;
								if( fileItem2 != null )
								{
									if( fileItem2.IsDirectory )
									{
										try
										{
											foreach( var fullPath in Directory.GetFiles( fileItem2.FullPath, "*", SearchOption.AllDirectories ) )
											{
												var fileName = RepositoryUtility.GetAllFilesPathByReal( GetProjectFolder(), fullPath );
												//var fileName = VirtualPathUtility.GetAllFilesPathByReal( fullPath );
												if( !string.IsNullOrEmpty( fileName ) )
													selectedFilesSet.AddWithCheckAlreadyContained( fileName );
											}
										}
										catch { }

										//selectedFoldersRealPath.Add( fileItem2.FullPath );
									}
									else
									{
										var fileName = RepositoryUtility.GetAllFilesPathByReal( GetProjectFolder(), fileItem2.FullPath );
										//var fileName = VirtualPathUtility.GetAllFilesPathByReal( fileItem2.FullPath );
										if( !string.IsNullOrEmpty( fileName ) )
											selectedFilesSet.AddWithCheckAlreadyContained( fileName );
									}
								}
							}

							selectedFilesWithInsideSelectedFolders = new List<string>( selectedFilesSet );
							CollectionUtility.MergeSort( selectedFilesWithInsideSelectedFolders, delegate ( string f1, string f2 )
							{
								return string.Compare( f1 + " ", f2 + " ", false );
							}, true );
						}

						var selectedFullPathFolders = new List<string>();
						var selectedFullPathFiles = new List<string>();
						{
							foreach( var item in contentBrowser1.SelectedItems )
							{
								var fileItem2 = item as LauncherEditRepositoryContentBrowserItem;
								if( fileItem2 != null )
								{
									if( fileItem2.IsDirectory )
										selectedFullPathFolders.Add( fileItem2.FullPath );
									else
										selectedFullPathFiles.Add( fileItem2.FullPath );
								}
							}
						}


						var itemWorld = new KryptonContextMenuItem( Translate( "Repository" ), EditorResourcesCache.Database, null );

						var items2 = new List<KryptonContextMenuItemBase>();

						//Get
						{
							var item = new KryptonContextMenuItem( Translate( "Get Selected" ), EditorResourcesCache.MoveDown,
								delegate ( object s, EventArgs e2 )
								{
									Close();
									RepositoryActionsWithServer.Get( selectedFullPathFolders, selectedFullPathFiles, projectID, false, GetProjectFolder(), new Win32WindowWrapper( EngineApp.CreatedInsideEngineWindow.Handle ) );
								} );
							item.Enabled = selectedFullPathFolders.Count != 0 || selectedFullPathFiles.Count != 0;
							items2.Add( item );
						}

						//Commit
						{
							var item = new KryptonContextMenuItem( Translate( "Commit Selected" ), EditorResourcesCache.MoveUp,
							   delegate ( object s, EventArgs e2 )
							   {
								   Close();
								   RepositoryActionsWithServer.Commit( selectedFullPathFolders, selectedFullPathFiles, projectID, false, false, GetProjectFolder(), new Win32WindowWrapper( EngineApp.CreatedInsideEngineWindow.Handle ) );
							   } );
							item.Enabled = selectedFullPathFolders.Count != 0 || selectedFullPathFiles.Count != 0;
							items2.Add( item );
						}

						//separator
						items2.Add( new KryptonContextMenuSeparator() );

						//Add
						{
							void Add( RepositorySyncMode mode )
							{
								var formItems = new List<RepositoryItemsForm.Item>();

								foreach( var fileName in selectedFilesWithInsideSelectedFolders )
								{
									var formItem = new RepositoryItemsForm.Item();
									formItem.FileName = fileName;
									formItems.Add( formItem );
								}

								var form = new RepositoryItemsForm( "Add Files", "Select files to add:", formItems.ToArray(), true );
								if( form.ShowDialog() != DialogResult.OK )
									return;

								var checkedFormItems = form.GetCheckedItems();
								RepositoryLocal.Add( checkedFormItems.Select( i => i.FileName ).ToArray(), mode );

								Invalidate( true );
								//EditorAPI2.FindWindow<ResourcesWindow>().Invalidate( true );
							}

							var itemAdd = new KryptonContextMenuItem( Translate( "Add" ), null );
							var items3 = new List<KryptonContextMenuItemBase>();

							{
								var item = new KryptonContextMenuItem( Translate( "Default sync mode" ), EditorResourcesCache.Synchronize,
									delegate ( object s, EventArgs e2 )
									{
										Add( RepositorySyncMode.Synchronize );
									} );
								items3.Add( item );
							}

							items3.Add( new KryptonContextMenuSeparator() );

							{
								var item = new KryptonContextMenuItem( Translate( "Sync with Clients" ), EditorResourcesCache.Synchronize,
									delegate ( object s, EventArgs e2 )
									{
										Add( RepositorySyncMode.Synchronize );
									} );
								items3.Add( item );
							}

							{
								var item = new KryptonContextMenuItem( Translate( "Server Only" ), EditorResourcesCache.ServerOnly,
									delegate ( object s, EventArgs e2 )
									{
										Add( RepositorySyncMode.ServerOnly );
									} );
								items3.Add( item );
							}

							itemAdd.Items.Add( new KryptonContextMenuItems( items3.ToArray() ) );
							itemAdd.Enabled = selectedFilesWithInsideSelectedFolders.Exists( fileName => RepositoryServerState.GetFileItem( fileName ) == null && !RepositoryLocal.FileItemExists( fileName ) );
							items2.Add( itemAdd );
						}

						//set sync mode
						{
							void SetSyncMode( RepositorySyncMode? mode )
							{
								var formItems = new List<RepositoryItemsForm.Item>();

								foreach( var fileName in selectedFilesWithInsideSelectedFolders )
								{
									if( RepositoryServerState.GetFileItem( fileName ) != null || RepositoryLocal.FileItemExists( fileName ) )
									{
										var formItem = new RepositoryItemsForm.Item();
										formItem.FileName = fileName;
										formItems.Add( formItem );
									}
								}

								var form = new RepositoryItemsForm( "Sync Mode", "Select files to set sync mode:", formItems.ToArray(), true );
								if( form.ShowDialog() != DialogResult.OK )
									return;

								var checkedFormItems = form.GetCheckedItems();
								RepositoryLocal.SetSyncMode( checkedFormItems.Select( i => i.FileName ).ToArray(), mode );

								Invalidate( true );
								//EditorAPI2.FindWindow<ResourcesWindow>().Invalidate( true );
							}

							var itemChangeSyncMode = new KryptonContextMenuItem( Translate( "Sync Mode" ), null );

							var items3 = new List<KryptonContextMenuItemBase>();

							{
								var item = new KryptonContextMenuItem( Translate( "Sync with Clients" ), EditorResourcesCache.Synchronize,
									delegate ( object s, EventArgs e2 )
									{
										SetSyncMode( RepositorySyncMode.Synchronize );
									} );
								items3.Add( item );
							}

							{
								var item = new KryptonContextMenuItem( Translate( "Server Only" ), EditorResourcesCache.ServerOnly,
									delegate ( object s, EventArgs e2 )
									{
										SetSyncMode( RepositorySyncMode.ServerOnly );
									} );
								items3.Add( item );
							}

							itemChangeSyncMode.Items.Add( new KryptonContextMenuItems( items3.ToArray() ) );
							itemChangeSyncMode.Enabled = selectedFilesWithInsideSelectedFolders.Exists( fileName => RepositoryServerState.GetFileItem( fileName ) != null || RepositoryLocal.FileItemExists( fileName ) );
							items2.Add( itemChangeSyncMode );
						}

						//Delete
						{
							var item = new KryptonContextMenuItem( Translate( "Delete" ), EditorResourcesCache.Delete,
								delegate ( object s, EventArgs e2 )
								{
									var formItems = new List<RepositoryItemsForm.Item>();

									foreach( var fileName in selectedFilesWithInsideSelectedFolders )
									{
										var formItem = new RepositoryItemsForm.Item();
										formItem.FileName = fileName;
										formItem.Prefix = "DELETE";
										formItems.Add( formItem );
									}

									var form = new RepositoryItemsForm( "Delete Files", "Select files to delete:", formItems.ToArray(), true );
									if( form.ShowDialog() != DialogResult.OK )
										return;

									var checkedFormItems = form.GetCheckedItems();
									RepositoryLocal.Delete( checkedFormItems.Select( i => i.FileName ).ToArray(), false );

									//foreach( var folderName in selectedFoldersRealPath )
									//{
									//	if( Directory.Exists( folderName ) )
									//		Directory.Delete( folderName, true );
									//}

									Invalidate( true );
									//EditorAPI2.FindWindow<ResourcesWindow>().Invalidate( true );
								} );

							item.Enabled = selectedFilesWithInsideSelectedFolders.Count != 0;// || selectedFoldersRealPath.Count != 0;
							items2.Add( item );
						}

						//Delete (Keep local)
						{
							var item = new KryptonContextMenuItem( Translate( "Delete (Keep local)" ), EditorResourcesCache.Delete,
								delegate ( object s, EventArgs e2 )
								{
									var formItems = new List<RepositoryItemsForm.Item>();

									foreach( var fileName in selectedFilesWithInsideSelectedFolders )
									{
										var formItem = new RepositoryItemsForm.Item();
										formItem.FileName = fileName;
										formItem.Prefix = "DELETE";
										formItems.Add( formItem );
									}

									var form = new RepositoryItemsForm( "Delete Files", "Select files to delete (Keep local):", formItems.ToArray(), true );
									if( form.ShowDialog() != DialogResult.OK )
										return;

									var checkedFormItems = form.GetCheckedItems();
									RepositoryLocal.Delete( checkedFormItems.Select( i => i.FileName ).ToArray(), true );

									Invalidate( true );
									//EditorAPI2.FindWindow<ResourcesWindow>().Invalidate( true );
								} );
							item.Enabled = selectedFilesWithInsideSelectedFolders.Exists( fileName => RepositoryServerState.GetFileItem( fileName ) != null || RepositoryLocal.FileItemExists( fileName ) );
							items2.Add( item );
						}

						//Revert local changes
						{
							//"Revert Local Changes"
							var item = new KryptonContextMenuItem( Translate( "Revert" ), EditorResourcesCache.Undo,
								delegate ( object s, EventArgs e2 )
								{
									var formItems = new List<RepositoryItemsForm.Item>();

									foreach( var fileName in selectedFilesWithInsideSelectedFolders )
									{
										var formItem = new RepositoryItemsForm.Item();
										formItem.FileName = fileName;
										formItems.Add( formItem );
									}

									var form = new RepositoryItemsForm( "Revert Status Changes In Local Repository", "Select files to revert:", formItems.ToArray(), true );
									if( form.ShowDialog() != DialogResult.OK )
										return;

									var checkedFormItems = form.GetCheckedItems();
									RepositoryLocal.Revert( checkedFormItems.Select( i => i.FileName ).ToArray() );

									Invalidate( true );
									//EditorAPI2.FindWindow<ResourcesWindow>().Invalidate( true );
								} );

							item.Enabled = selectedFilesWithInsideSelectedFolders.Exists( fileName => RepositoryLocal.FileItemExists( fileName ) );
							items2.Add( item );
						}

						itemWorld.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
						items.Add( itemWorld );

						//separator
						items.Add( new KryptonContextMenuSeparator() );
					}


					////Cut
					//{
					//	var item = new KryptonContextMenuItem( Translate( "Cut" ), EditorResourcesCache.Cut,
					//		delegate ( object s, EventArgs e2 )
					//		{
					//			Cut();
					//		} );
					//	item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Cut" );
					//	item.Enabled = CanCut();
					//	items.Add( item );
					//}

					////Copy
					//{
					//	var item = new KryptonContextMenuItem( Translate( "Copy" ), EditorResourcesCache.Copy,
					//		delegate ( object s, EventArgs e2 )
					//		{
					//			Copy();
					//		} );
					//	item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Copy" );
					//	item.Enabled = CanCopy();
					//	items.Add( item );
					//}

					////Paste
					//if( fileItem.IsDirectory || ( !fileItem.IsDirectory && CanPaste( out _, out _, out _, out _ ) ) )
					//{
					//	var item = new KryptonContextMenuItem( Translate( "Paste" ), EditorResourcesCache.Paste,
					//		delegate ( object s, EventArgs e2 )
					//		{
					//			Paste();
					//		} );
					//	item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Paste" );
					//	item.Enabled = CanPaste( out _, out _, out _, out _ );
					//	items.Add( item );
					//}

					////Favorites
					//if( EditorFavorites.AllowFavorites && !fileItem.IsDirectory )
					//{
					//	//separator
					//	items.Add( new KryptonContextMenuSeparator() );

					//	//var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath );

					//	if( contentItem.Parent != favoritesItem )
					//	{
					//		//Add to Favorites

					//		var item = new KryptonContextMenuItem( Translate( "Add to Favorites" ), EditorResourcesCache.Add,
					//		   delegate ( object s, EventArgs e2 )
					//		   {
					//			   FavoritesAddSelectedItems();

					//			   //update Favorites group
					//			   ContentBrowser contentBrowser = EditorAPI2.FindWindow<ResourcesWindow>().ContentBrowser1;
					//			   if( contentBrowser != this )
					//				   contentBrowser.favoritesItem?.PerformChildrenChanged();
					//			   favoritesItem?.PerformChildrenChanged();
					//		   } );
					//		item.Enabled = FavoritesCheckEnableItem( true );
					//		items.Add( item );
					//	}
					//	else
					//	{
					//		//Remove from Favorites

					//		var item = new KryptonContextMenuItem( Translate( "Remove from Favorites" ), EditorResourcesCache.Delete,
					//		   delegate ( object s, EventArgs e2 )
					//		   {
					//			   FavoritesRemoveSelectedItems();

					//			   //update Favorites group
					//			   ContentBrowser contentBrowser = EditorAPI2.FindWindow<ResourcesWindow>().ContentBrowser1;
					//			   if( contentBrowser != this )
					//				   contentBrowser.favoritesItem?.PerformChildrenChanged();
					//			   favoritesItem?.PerformChildrenChanged();
					//		   } );
					//		item.Enabled = FavoritesCheckEnableItem( false );
					//		items.Add( item );
					//	}
					//}

					////separator
					//items.Add( new KryptonContextMenuSeparator() );

					////Delete
					//{
					//	var item = new KryptonContextMenuItem( Translate( "Delete" ), EditorResourcesCache.Delete,
					//		delegate ( object s, EventArgs e2 )
					//		{
					//			TryDeleteObjects();
					//		} );
					//	item.Enabled = CanDeleteObjects( out _ );
					//	items.Add( item );
					//}

					////Rename
					//{
					//	var item = new KryptonContextMenuItem( Translate( "Rename" ), null,
					//		delegate ( object s, EventArgs e2 )
					//		{
					//			RenameBegin();
					//		} );
					//	item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Rename" );
					//	item.Enabled = CanRename();
					//	items.Add( item );
					//}

					////separator
					//items.Add( new KryptonContextMenuSeparator() );

					//Sort by
					if( fileItem != null && fileItem.IsDirectory )
					{
						contentBrowser1.AddSortByToContextMenu( items );

						//separator
						items.Add( new KryptonContextMenuSeparator() );
					}

					//Properties
					{
						var item = new KryptonContextMenuItem( Translate( "Properties" ), null, delegate ( object s, EventArgs e2 )
						{
							string realPath = fileItem.FullPath;
							Win32Utility.ShellExecuteEx( "properties", realPath );
						} );
						item.Enabled = fileItem != null;
						items.Add( item );
					}
				}
			}
		}

		private void toolStripButtonOptions_Click( object sender, EventArgs e )
		{
			var form = new ContentBrowserOptionsForm( contentBrowser1 );
			form.ShowDialog();
		}

		private void toolStripButtonChange_Click( object sender, EventArgs e )
		{
			//!!!!Default button to reset

			var folder = projectFolder;
			if( string.IsNullOrEmpty( folder ) )
				folder = defaultProjectFolder ?? "";

			var form = new OKCancelTextBoxForm( "Select the local folder for the repository:", folder, "Cloudbox",
				delegate ( string text, ref string error )
				{
					try
					{
						bool IsPathValid( string path )
						{
							var invalidChars = Path.GetInvalidPathChars();
							foreach( var invalidChar in invalidChars )
							{
								if( path.Contains( invalidChar ) )
									return false;
							}
							return true;
						}

						if( !IsPathValid( text ) || !Path.IsPathRooted( text ) )
						{
							error = "Invalid file path.";
							return false;
						}
					}
					catch
					{
						error = "Invalid file path.";
						return false;
					}

					return true;
				},
				delegate ( string text, ref string error )
				{
					projectFolder = text;
					ChangeProjectFolderCallback?.Invoke( this, projectFolder );
					reopen = true;
					return true;
				}
			);

			if( form.ShowDialog( new Win32WindowWrapper( EngineApp.CreatedInsideEngineWindow.Handle ) ) != DialogResult.OK )
				return;

			reopen = true;
			Close();
		}

		private void toolStripButtonOpen_Click( object sender, EventArgs e )
		{
			//var projectFolder = CloudProjectCommon.GetAppProjectFolder( projectID, true );
			Win32Utility.ShellExecuteEx( null, projectFolder );
		}

		private void toolStripButtonGet_Click( object sender, EventArgs e )
		{
			Close();
			RepositoryActionsWithServer.Get( new string[] { GetProjectFolder() }, new string[ 0 ], projectID, false, GetProjectFolder(), new Win32WindowWrapper( EngineApp.CreatedInsideEngineWindow.Handle ) );
		}

		private void toolStripButtonCommit_Click( object sender, EventArgs e )
		{
			Close();
			RepositoryActionsWithServer.Commit( new string[] { GetProjectFolder() }, new string[ 0 ], projectID, false, false, GetProjectFolder(), new Win32WindowWrapper( EngineApp.CreatedInsideEngineWindow.Handle ) );
		}

		static List<LauncherEditRepositoryContentBrowserItem> FindCreatedItemsByRealPath( ContentBrowser browser, string realPath )
		{
			var result = new List<LauncherEditRepositoryContentBrowserItem>();

			var realPath2 = VirtualPathUtility.NormalizePath( realPath );

			foreach( var item in browser.GetAllItems() )
			{
				var fileItem = item as LauncherEditRepositoryContentBrowserItem;
				if( fileItem != null && string.Compare( fileItem.FullPath, realPath2, true ) == 0 )
					result.Add( fileItem );
			}

			return result;
		}

		void CallPerformChildrenChanged( string realPath )
		{
			//update project folders

			//commented because not work when file updated in the root of the project
			//var allFilesPath = RepositoryUtility.GetAllFilesPathByReal( GetProjectFolder(), realPath );
			//if( !string.IsNullOrEmpty( allFilesPath ) )
			{
				foreach( var currentItem in /*ContentBrowserUtility.*/FindCreatedItemsByRealPath( contentBrowser1, realPath ) )
				{
					//update children. need update parent items for C# filtering mode (when directories without items are hided)
					ContentBrowser.Item item = currentItem;
					while( item != null )
					{
						item.PerformChildrenChanged();
						item = item.Parent;
					}
				}
			}
		}

		private void FileWatcher_Update( FileSystemEventArgs args )
		{
			try
			{
				switch( args.ChangeType )
				{
				case WatcherChangeTypes.Created:
				case WatcherChangeTypes.Deleted:
					{
						var directoryName = Path.GetDirectoryName( args.FullPath );
						CallPerformChildrenChanged( directoryName );
					}
					break;

				case WatcherChangeTypes.Renamed:
					{
						RenamedEventArgs args2 = (RenamedEventArgs)args;

						var directoryNameOld = Path.GetDirectoryName( args2.OldFullPath );
						CallPerformChildrenChanged( directoryNameOld );

						var directoryName = Path.GetDirectoryName( args.FullPath );
						if( string.Compare( directoryNameOld, directoryName, true ) != 0 )
							CallPerformChildrenChanged( directoryName );
					}
					break;
				}
			}
			catch { }
		}
	}
}
#endif