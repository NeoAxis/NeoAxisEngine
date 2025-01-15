#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Internal.ComponentFactory.Krypton.Toolkit;
using System.IO;
using Internal.Aga.Controls.Tree;
using NeoAxis.Networking;

namespace NeoAxis.Editor
{
	partial class ContentBrowser
	{
		public void AddSortByToContextMenu( List<KryptonContextMenuItemBase> items )
		{
			KryptonContextMenuItem item;

			KryptonContextMenuItem itemSortBy = new KryptonContextMenuItem( Translate( "Sort by" ), null );
			List<KryptonContextMenuItemBase> items2 = new List<KryptonContextMenuItemBase>();

			//Name
			item = new KryptonContextMenuItem( Translate( "Name" ), null, delegate ( object sender, EventArgs e )
			{
				if( options.SortFilesBy != SortByItems.Name )
				{
					options.SortFilesBy = SortByItems.Name;
					Resort();
				}
			} );
			item.Checked = options.SortFilesBy == SortByItems.Name;
			items2.Add( item );

			//Date
			item = new KryptonContextMenuItem( Translate( "Date" ), null, delegate ( object sender, EventArgs e )
			{
				if( options.SortFilesBy != SortByItems.Date )
				{
					options.SortFilesBy = SortByItems.Date;
					Resort();
				}
			} );
			item.Checked = options.SortFilesBy == SortByItems.Date;
			items2.Add( item );

			//Type
			item = new KryptonContextMenuItem( Translate( "Type" ), null, delegate ( object sender, EventArgs e )
			{
				if( options.SortFilesBy != SortByItems.Type )
				{
					options.SortFilesBy = SortByItems.Type;
					Resort();
				}
			} );
			item.Checked = options.SortFilesBy == SortByItems.Type;
			items2.Add( item );

			//Size
			item = new KryptonContextMenuItem( Translate( "Size" ), null, delegate ( object sender, EventArgs e )
			{
				if( options.SortFilesBy != SortByItems.Size )
				{
					options.SortFilesBy = SortByItems.Size;
					Resort();
				}
			} );
			item.Checked = options.SortFilesBy == SortByItems.Size;
			items2.Add( item );

			//separator
			items2.Add( new KryptonContextMenuSeparator() );

			//Ascending
			item = new KryptonContextMenuItem( Translate( "Ascending" ), null, delegate ( object sender, EventArgs e )
			{
				options.SortFilesByAscending = !options.SortFilesByAscending;
				Resort();
			} );
			item.Checked = options.SortFilesByAscending;
			items2.Add( item );


			itemSortBy.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
			items.Add( itemSortBy );
		}

		void ShowContextMenu( Item contentItem, Control locationControl, Point locationPoint )
		{
			//!!!!совсем нет меню в read only?
			if( ReadOnlyHierarchy )
				return;

			//DocumentInstance document = null;
			//if( DocumentWindow != null )
			//	document = DocumentWindow.Document;

			var items = new List<KryptonContextMenuItemBase>();

			//_Component
			if( contentItem != null )
			{
				var componentItem = contentItem as ContentBrowserItem_Component;
				if( componentItem != null )
				{
					var documentOfComponent = EditorAPI2.GetDocumentByObject( componentItem.Component );

					//!!!!!слой, он как папка. ассоциация с ComponentsBrowser и ResourcesBrowser
					//!!!!!!!name: группа?
					//!!!!!!тут же еще Load/Save Selection. запоминать выделенные
					//!!!!!!!!!!а с ResourcesBrowser есть ассоциации?

					//folder specific. //!!!!а на ресурсе почему нельзя?

					//!!!!!!
					//ContentBrowser_FileItem currentOrParentDirectoryItem = null;
					//{
					//	var i = contentItem as ContentBrowser_FileItem;
					//	if( i != null )
					//	{
					//		if( !i.isDirectory )
					//		{
					//			ContentBrowser_FileItem parent = i.Parent as ContentBrowser_FileItem;
					//			if( parent != null && parent.isDirectory )
					//				currentOrParentDirectoryItem = parent;
					//		}
					//		else
					//			currentOrParentDirectoryItem = i;
					//	}
					//}

					//if( document != null )
					{
						//!!!!надо ли всем иконки. везде так
						//!!!!отсортировать более логично (с учетом как в тулбаре). везде так

						//!!!!вторым уровнем криптонового меню часто создаваемые?

						//!!!!!cut, copy


						//!!!!!
						//Editor
						{
							//!!!!кнопками открывать еще, рядом с "..."

							//!!!!

							//!!!!! imageListContextMenu.Images[ "Delete_16.png" ],
							//!!!!!name
							var item = new KryptonContextMenuItem( Translate( "Editor" ), EditorResourcesCache.Edit,
								delegate ( object s, EventArgs e2 )
								{
									EditorAPI2.OpenDocumentWindowForObject( DocumentWindow != null ? DocumentWindow.Document2 : null, componentItem.Component );
								} );
							item.Enabled = componentItem.Component != null &&
								EditorAPI2.IsDocumentObjectSupport( componentItem.Component ) &&
								!componentItem.Component.EditorReadOnlyInHierarchy;
							items.Add( item );
						}

						//Settings
						{
							var item = new KryptonContextMenuItem( Translate( "Settings" ), EditorResourcesCache.Settings,
								delegate ( object s, EventArgs e2 )
								{
									EditorAPI2.SelectDockWindow( EditorAPI2.FindWindow<SettingsWindow>() );
								} );
							items.Add( item );
						}

						//Separate Settings
						if( EditorUtility.AllowSeparateSettings )
						{
							var item = new KryptonContextMenuItem( Translate( "Separate Settings" ), EditorResourcesCache.Settings,
								delegate ( object s, EventArgs e2 )
								{
									if( componentItem != null )
									{
										bool canUseAlreadyOpened = !ModifierKeys.HasFlag( Keys.Shift );
										EditorAPI2.ShowObjectSettingsWindow( DocumentWindow.Document2, componentItem.Component, canUseAlreadyOpened );
									}
									else
									{
										//!!!!!
										Log.Warning( "impl" );
									}
								} );
							item.Enabled = componentItem.Component != null && !componentItem.Component.EditorReadOnlyInHierarchy && documentOfComponent != null;

							var res = ComponentUtility.GetResourceInstanceByComponent( componentItem.Component );
							if( res != null && res.InstanceType == Resource.InstanceType.Resource )
								item.Enabled = false;

							//item.Enabled = !rootNodeSelected;
							items.Add( item );
						}

						items.Add( new KryptonContextMenuSeparator() );

						//New object
						{
							EditorContextMenuWinForms.AddNewObjectItem( items, CanNewObject( out _ ), delegate ( Metadata.TypeInfo type, bool assetsFolderOnly )
							{
								TryNewObject( type );
							} );

							////!!!!! imageListContextMenu.Images[ "New_16.png" ] );
							//KryptonContextMenuItem item = new KryptonContextMenuItem( Translate( "New object" ), Properties.Resources.New_16,
							//	delegate ( object s, EventArgs e2 )
							//	{
							//		TryNewObject();
							//	} );
							//Item dummy;
							//item.Enabled = CanNewObject( out dummy );
							//items.Add( item );
						}

						//separator
						items.Add( new KryptonContextMenuSeparator() );

						//Cut
						{
							var item = new KryptonContextMenuItem( Translate( "Cut" ), EditorResourcesCache.Cut,
								delegate ( object s, EventArgs e2 )
								{
									Cut();
								} );
							item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Cut" );
							item.Enabled = CanCut();
							items.Add( item );
						}

						//Copy
						{
							var item = new KryptonContextMenuItem( Translate( "Copy" ), EditorResourcesCache.Copy,
								delegate ( object s, EventArgs e2 )
								{
									Copy();
								} );
							item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Copy" );
							item.Enabled = CanCopy();
							items.Add( item );
						}

						//Paste
						{
							var item = new KryptonContextMenuItem( Translate( "Paste" ), EditorResourcesCache.Paste,
								delegate ( object s, EventArgs e2 )
								{
									Paste();
								} );
							item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Paste" );
							item.Enabled = CanPaste( out _, out _, out _, out _ );
							items.Add( item );
						}

						//Clone
						{
							var item = new KryptonContextMenuItem( Translate( "Duplicate" ), EditorResourcesCache.Clone,
								delegate ( object s, EventArgs e2 )
								{
									TryCloneObjects();
								} );
							item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Duplicate" );
							item.Enabled = CanCloneObjects( out _ );
							items.Add( item );
						}

						//Export to File
						{
							var item = new KryptonContextMenuItem( Translate( "Export to File" ), null, delegate ( object s, EventArgs e2 )
							{
								EditorUtility2.ExportComponentToFile( componentItem.Component, IntPtr.Zero );
							} );
							item.Enabled = true;//oneSelectedComponent != null;
							items.Add( item );
						}

						//separator
						items.Add( new KryptonContextMenuSeparator() );

						//Delete
						{
							var item = new KryptonContextMenuItem( Translate( "Delete" ), EditorResourcesCache.Delete,
								delegate ( object s, EventArgs e2 )
								{
									TryDeleteObjects();
								} );
							item.Enabled = CanDeleteObjects( out _ );
							items.Add( item );
						}

						//Rename
						{
							var item = new KryptonContextMenuItem( Translate( "Rename" ), null,
								delegate ( object s, EventArgs e2 )
								{
									RenameBegin();
								} );
							item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Rename" );
							item.Enabled = CanRename();
							items.Add( item );
						}
					}

					//!!!!было
					//EditorContextMenu.AddAdditionalActionsToMenu( items );//, DocumentWindow );

					//if( items.Count != 0 )
					//	items.Add( new KryptonContextMenuSeparator() );

					////!!!!этого нет в Resources, хотя там тоже компонента. есть только во вложенных
					////Utils
					//{
					//	var utilsItem = new KryptonContextMenuItem( Translate( "Utils" ), null );
					//	List<KryptonContextMenuItemBase> items2 = new List<KryptonContextMenuItemBase>();

					//	{
					//		var item = new KryptonContextMenuItem( Translate( "Show object details" ), null,
					//			delegate ( object s, EventArgs e2 )
					//			{
					//				EditorUtils.ShowObjectDetailsAsDocument( componentItem.Component );
					//			} );

					//		item.Enabled = componentItem != null && componentItem.Component != null;
					//		items2.Add( item );
					//	}

					//	utilsItem.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
					//	items.Add( utilsItem );
					//}

					//string nodePath = GetNodePath( node );

					//if( node.Tag == null )
					//{
					//	//item = new KryptonContextMenuItem( Translate("New Folder" ),
					//	//   imageListContextMenu.Images[ "NewFolder_16.png" ],
					//	//   delegate( object s, EventArgs e2 ) { NewDirectory( nodePath ); } );
					//	//menu.Items.Add( item );

					//	{
					//		KryptonContextMenuItem newResourceItem = new KryptonContextMenuItem( Translate( "New" ),
					//			imageListContextMenu.Images[ "New_16.png" ] );

					//		menuItem = new KryptonContextMenuItem( Translate( "Folder" ),
					//			imageListContextMenu.Images[ "Folder_16.png" ], delegate ( object s, EventArgs e2 )
					//			{
					//				NewDirectory( nodePath );
					//			} );
					//		newResourceItem.DropDownItems.Add( menuItem );

					//		newResourceItem.DropDownItems.Add( new ToolStripSeparator() );

					//		//KryptonContextMenuItem newResourceItem = new KryptonContextMenuItem(
					//		//   Translate("New Resource" ),
					//		//   imageListContextMenu.Images[ "NewResource_16.png" ] );

					//		foreach( ResourceType type in ResourceTypeManager.Instance.Types )
					//		{
					//			if( type.AllowNewResource )
					//			{
					//				string displayName = Translate( type.DisplayName );
					//				menuItem = new KryptonContextMenuItem( displayName, type.Icon, NewResourceMenuClick );
					//				menuItem.Tag = new Pair<string, ResourceType>( nodePath, type );
					//				newResourceItem.DropDownItems.Add( menuItem );
					//			}
					//		}

					//		menu.Items.Add( newResourceItem );
					//	}

					//	//item = new KryptonContextMenuItem( Translate("New Resource" ),
					//	//   imageListContextMenu.Images[ "NewResource_16.png" ],
					//	//   delegate( object s, EventArgs e2 ) { NewResource( nodePath ); } );
					//	//menu.Items.Add( item );

					////separator
					//if( menu.Items.Count != 0 )
					//	menu.Items.Add( new ToolStripSeparator() );
					//{
					//	var item = new KryptonContextMenuItem( Translate( "Open folder in Explorer" ), null, delegate ( object s, EventArgs e2 )
					//	{
					//		//!!!!!!
					//		//string realPath = VirtualFileSystem.GetRealPathByVirtual( nodePath );
					//		//Win32Utils.ShellExecuteEx( null, realPath );
					//	} );
					//	menu.Items.Add( item );
					//}

					//if( node.Tag != null )
					//{
					//	//Edit
					//	{
					//		string extension = Path.GetExtension( nodePath );
					//		if( !string.IsNullOrEmpty( extension ) )
					//		{
					//			extension = extension.Substring( 1 );
					//			ResourceType resourceType = ResourceTypeManager.Instance.GetByExtension( extension );
					//			if( resourceType != null )
					//			{
					//				menuItem = new KryptonContextMenuItem( Translate( "Edit" ), imageListContextMenu.Images[ "Edit_16.png" ],
					//					delegate ( object s, EventArgs e2 )
					//					{
					//						TryBeginEditMode();
					//					} );
					//				menu.Items.Add( menuItem );
					//			}
					//		}
					//	}

					//	//Open in External Program
					//	{
					//		menuItem = new KryptonContextMenuItem( Translate( "Open in External Program" ), null, delegate ( object s, EventArgs e2 )
					//		{
					//			string realPath = VirtualFileSystem.GetRealPathByVirtual( nodePath );
					//			Win32Utils.ShellExecuteEx( null, realPath );
					//		} );
					//		menu.Items.Add( menuItem );
					//	}
					//}

					////Add resource type specific items
					//if( node.Tag as string != null )
					//{
					//	string path = GetNodePath( node );
					//	string extension = Path.GetExtension( path );
					//	if( !string.IsNullOrEmpty( extension ) )
					//	{
					//		extension = extension.Substring( 1 );
					//		ResourceType resourceType = ResourceTypeManager.Instance.GetByExtension( extension );
					//		if( resourceType != null )
					//			resourceType.DoResourcesTreeContextMenu( path, menu );
					//	}
					//}

					//if( selectedNodes.Count != 0 )
					//{
					//	bool rootNodeSelected = false;
					//	{
					//		foreach( MyTreeNode n in selectedNodes )
					//			if( n.Parent == null )
					//				rootNodeSelected = true;
					//	}

					//	//if( !rootNodeSelected )
					//	{
					//		//separator
					//		menu.Items.Add( new ToolStripSeparator() );

					//		//cut
					//		menuItem = new KryptonContextMenuItem( Translate( "Cut" ), imageListContextMenu.Images[ "Cut_16.png" ],
					//			delegate ( object s, EventArgs e2 ) { CutCopyFiles( true ); } );
					//		menuItem.Enabled = !rootNodeSelected;
					//		menu.Items.Add( menuItem );

					//		//copy
					//		menuItem = new KryptonContextMenuItem( Translate( "Copy" ), imageListContextMenu.Images[ "Copy_16.png" ],
					//			delegate ( object s, EventArgs e2 ) { CutCopyFiles( false ); } );
					//		menu.Items.Add( menuItem );
					//	}

					//	//paste
					//	//if( IsExistsDataToPaste() )
					//	if( node.Tag == null )
					//	{
					//		////separator
					//		//if( rootNodeSelected )
					//		//   menu.Items.Add( new ToolStripSeparator() );

					//		string directoryToPaste = null;
					//		if( IsExistsDataToPaste() )
					//			directoryToPaste = GetSelectedDirectoryToPaste();

					//		//string directoryToPaste = GetSelectedDirectoryToPaste();
					//		//if( directoryToPaste != null )
					//		{
					//			menuItem = new KryptonContextMenuItem( Translate( "Paste" ), imageListContextMenu.Images[ "Paste_16.png" ],
					//				delegate ( object s, EventArgs e2 ) { PasteFiles( directoryToPaste ); } );
					//			menuItem.Enabled = directoryToPaste != null;
					//			menu.Items.Add( menuItem );
					//		}
					//	}

					//	//if( !rootNodeSelected )
					//	{
					//		//separator
					//		menu.Items.Add( new ToolStripSeparator() );

					//		//delete
					//		menuItem = new KryptonContextMenuItem( Translate( "Delete" ), imageListContextMenu.Images[ "Delete_16.png" ],
					//			delegate ( object s, EventArgs e2 ) { TryDeleteObjects(); } );
					//		menuItem.Enabled = !rootNodeSelected;
					//		menu.Items.Add( menuItem );
					//	}
					//	//}

					//	//if( ( ( node.Tag == null && node.Parent != null ) || node.Tag != null ) && selectedNodes.Count == 1 )
					//	{
					//		//rename
					//		menuItem = new KryptonContextMenuItem( Translate( "Rename" ), null, delegate ( object s, EventArgs e2 )
					//		{
					//			//Try begin edit label
					//			if( node != treeView.SelectedNode )
					//				return;
					//			if( !node.IsEditing )
					//				node.BeginEdit();
					//		} );
					//		menuItem.Enabled = selectedNodes.Count == 1 && !rootNodeSelected;
					//		menu.Items.Add( menuItem );
					//	}
					//}

					////Sort by, Refresh
					//{
					//	//separator
					//	menu.Items.Add( new ToolStripSeparator() );

					//	AddSortByToContextMenu( menu );

					//	//Refresh
					//	{
					//		menuItem = new KryptonContextMenuItem( Translate( "Refresh" ), imageListContextMenu.Images[ "Refresh_16.png" ],
					//			delegate ( object s, EventArgs e2 )
					//			{
					//				RefreshPath( (MyTreeNode)node );
					//			} );
					//		menu.Items.Add( menuItem );
					//	}
					//}



					////separator
					//menu.Items.Add( new ToolStripSeparator() );

					////!!!!!?
					//{
					//	var item = new KryptonContextMenuItem( Translate( "Properties" ), null, delegate ( object s, EventArgs e2 )
					//	{
					//		//!!!!!
					//		//string realPath = VirtualPathUtils.GetRealPathByVirtual( nodePath );
					//		//Win32Utils.ShellExecuteEx( "properties", realPath );
					//	} );
					//	menu.Items.Add( item );
					//}
				}
			}

			//_File
			if( contentItem != null )
			{
				//folder specific. //!!!!а на ресурсе почему нельзя?

				ContentBrowserItem_File currentOrParentDirectoryItem = null;
				var fileItem = contentItem as ContentBrowserItem_File;
				{
					if( fileItem != null )
					{
						if( !fileItem.IsDirectory )
						{
							var parent = fileItem.Parent as ContentBrowserItem_File;
							if( parent != null && parent.IsDirectory )
								currentOrParentDirectoryItem = parent;
						}
						else
							currentOrParentDirectoryItem = fileItem;
					}
				}

				if( fileItem != null )
				{

					//!!!!!
					//Open
					if( fileItem != null && !fileItem.IsDirectory )
					{
						var item = new KryptonContextMenuItem( Translate( "Open" ), null, delegate ( object s, EventArgs e2 )
						{
							//!!!!!!
							bool handled = false;
							OpenFile( fileItem, ref handled );
						} );

						//!!!!было
						//item.Font = new Font( item.Font, FontStyle.Bold );

						item.Enabled = EditorAPI2.IsDocumentFileSupport( fileItem.FullPath );
						//item.Enabled = currentOrParentDirectoryItem != null;
						items.Add( item );
					}

					//Open with
					if( fileItem != null && !fileItem.IsDirectory )
					{
						//!!!!! imageListContextMenu.Images[ "New_16.png" ] );
						KryptonContextMenuItem itemOpenWith = new KryptonContextMenuItem( Translate( "Open with" ), null );
						//!!!!!
						itemOpenWith.Enabled = currentOrParentDirectoryItem != null || contentItem.Parent == null || ( contentItem.Parent != null && contentItem.Parent == favoritesItem );

						List<KryptonContextMenuItemBase> items2 = new List<KryptonContextMenuItemBase>();

						//!!!!!движковые сначала

						//!!!!!
						//Text editor
						{
							var item = new KryptonContextMenuItem( Translate( "Text editor" ), null,// imageListContextMenu.Images[ "NewFolder_16.png" ],
							   delegate ( object s, EventArgs e2 )
							   {
								   EditorAPI2.OpenFileAsDocument( fileItem.FullPath, true, true, specialMode: "TextEditor" );

								   //var text = File.ReadAllText( fileItem.FullPath );

								   //EditorForm.Instance.OpenTextAsDocument( text, Path.GetFileName( fileItem.path ) + " text", true );
								   //EditorAPI.OpenTextAsDocument( text, "Text of " + Path.GetFileName( fileItem.FullPath ), true );

								   //Log.Warning( "impl" );
								   //!!!!NewDirectory( nodePath );
							   } );
							items2.Add( item );
						}

						//!!!!!
						//External application
						{
							var item = new KryptonContextMenuItem( Translate( "External app" ), null,// imageListContextMenu.Images[ "NewFolder_16.png" ],
							delegate ( object s, EventArgs e2 )
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
							items2.Add( item );
						}

						////separator
						//items2.Add( new KryptonContextMenuSeparator() );

						////impl
						//{
						//	var item = new KryptonContextMenuItem( Translate( "impl" ), null,// imageListContextMenu.Images[ "NewFolder_16.png" ],
						//	   delegate ( object s, EventArgs e2 )
						//	   {
						//		   //!!!!!
						//	   } );
						//	items2.Add( item );
						//}

						itemOpenWith.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
						items.Add( itemOpenWith );
					}

					if( fileItem.IsDirectory )
					{
						//separator
						if( items.Count != 0 )
							items.Add( new KryptonContextMenuSeparator() );

						//New folder
						{
							var item = new KryptonContextMenuItem( Translate( "New Folder" ), EditorResourcesCache.NewFolder,
							   delegate ( object s, EventArgs e2 )
							   {
								   NewFolder();
							   } );
							item.Enabled = CanNewFolder( out _ );
							items.Add( item );
						}

						//New resource or txt file when it outside Assets folder
						{
							var canNewResource = CanNewResource( out var directoryPath );

							//var insideAssets = false;
							//try
							//{
							//	var virtualPath = VirtualPathUtility.GetVirtualPathByReal( directoryPath, true );
							//	insideAssets = directoryPath == VirtualFileSystem.Directories.Assets || !string.IsNullOrEmpty( virtualPath );
							//}
							//catch { }

							if( canNewResource )
							{
								EditorContextMenuWinForms.AddNewResourceItem( items, canNewResource/*, insideAssets*/, delegate ( Metadata.TypeInfo type, bool assetsFolderOnly )
								{
									NewResource( type, assetsFolderOnly );
								} );
							}
							else
							{
								var item = new KryptonContextMenuItem( Translate( "New Text File" ), EditorResourcesCache.New,
								   delegate ( object s, EventArgs e2 )
								   {
									   NewTextFileWhenOutsideAssets();
								   } );
								item.Enabled = CanNewTextFileWhenOutsideAssets( out _ );
								items.Add( item );
							}
						}

						////Import
						//{
						//	var item = new KryptonContextMenuItem( Translate( "Import" ), EditorResourcesCache.Import,
						//	   delegate ( object s, EventArgs e2 )
						//	   {
						//		   ImportResource();
						//	   } );
						//	item.Enabled = CanImportResource( out _ );
						//	items.Add( item );
						//}
					}

					////New
					//{
					//	KryptonContextMenuItem itemNew = new KryptonContextMenuItem( Translate( "New" ), null );//!!!!! imageListContextMenu.Images[ "New_16.png" ] );
					//	itemNew.Enabled = currentOrParentDirectoryItem != null;

					//	//Folder
					//	{
					//		var item = new KryptonContextMenuItem( Translate( "Folder" ), null,// imageListContextMenu.Images[ "NewFolder_16.png" ],
					//		   delegate ( object s, EventArgs e2 )
					//		   {
					//			   //!!!!
					//			   Log.Warning( "impl" );
					//			   //!!!!NewDirectory( nodePath );
					//		   } );
					//		itemNew.DropDownItems.Add( item );
					//	}

					//	//separator
					//	itemNew.DropDownItems.Add( new ToolStripSeparator() );

					//	//Resource
					//	{
					//		var item = new KryptonContextMenuItem( Translate( "Resource" ), null,// imageListContextMenu.Images[ "NewFolder_16.png" ],
					//		   delegate ( object s, EventArgs e2 )
					//		   {
					//			   string virtualPath = VirtualPathUtils.GetVirtualPathByReal( currentOrParentDirectoryItem.FullPath );
					//			   EditorForm.Instance.ShowNewObjectWindow( virtualPath, null );
					//		   } );
					//		item.Enabled = currentOrParentDirectoryItem != null &&
					//			VirtualPathUtils.GetVirtualPathByReal( currentOrParentDirectoryItem.FullPath ) != "";
					//		itemNew.DropDownItems.Add( item );
					//	}

					//	menu.Items.Add( itemNew );
					//}

					//string nodePath = GetNodePath( node );

					//if( node.Tag == null )
					//{
					//	//item = new KryptonContextMenuItem( Translate("New Folder" ),
					//	//   imageListContextMenu.Images[ "NewFolder_16.png" ],
					//	//   delegate( object s, EventArgs e2 ) { NewDirectory( nodePath ); } );
					//	//menu.Items.Add( item );

					//	{
					//		KryptonContextMenuItem newResourceItem = new KryptonContextMenuItem( Translate( "New" ),
					//			imageListContextMenu.Images[ "New_16.png" ] );

					//		menuItem = new KryptonContextMenuItem( Translate( "Folder" ),
					//			imageListContextMenu.Images[ "Folder_16.png" ], delegate ( object s, EventArgs e2 )
					//			{
					//				NewDirectory( nodePath );
					//			} );
					//		newResourceItem.DropDownItems.Add( menuItem );

					//		newResourceItem.DropDownItems.Add( new ToolStripSeparator() );

					//		//KryptonContextMenuItem newResourceItem = new KryptonContextMenuItem(
					//		//   Translate("New Resource" ),
					//		//   imageListContextMenu.Images[ "NewResource_16.png" ] );

					//		foreach( ResourceType type in ResourceTypeManager.Instance.Types )
					//		{
					//			if( type.AllowNewResource )
					//			{
					//				string displayName = Translate( type.DisplayName );
					//				menuItem = new KryptonContextMenuItem( displayName, type.Icon, NewResourceMenuClick );
					//				menuItem.Tag = new Pair<string, ResourceType>( nodePath, type );
					//				newResourceItem.DropDownItems.Add( menuItem );
					//			}
					//		}

					//		menu.Items.Add( newResourceItem );
					//	}

					//	//item = new KryptonContextMenuItem( Translate("New Resource" ),
					//	//   imageListContextMenu.Images[ "NewResource_16.png" ],
					//	//   delegate( object s, EventArgs e2 ) { NewResource( nodePath ); } );
					//	//menu.Items.Add( item );

					//Open folder in Explorer
					if( fileItem.IsDirectory )
					{
						//separator
						if( items.Count != 0 )
							items.Add( new KryptonContextMenuSeparator() );

						var item = new KryptonContextMenuItem( Translate( "Open Folder in Explorer" ), null, delegate ( object s, EventArgs e2 )
						{
							//!!!!!!
							string realPath = currentOrParentDirectoryItem.FullPath;// VirtualPathUtils.GetRealPathByVirtual( nodePath );
							Win32Utility.ShellExecuteEx( null, realPath );
						} );
						item.Enabled = currentOrParentDirectoryItem != null;
						items.Add( item );
					}

					//separator
					if( items.Count != 0 )
						items.Add( new KryptonContextMenuSeparator() );

					//if( node.Tag != null )
					//{
					//	//Edit
					//	{
					//		string extension = Path.GetExtension( nodePath );
					//		if( !string.IsNullOrEmpty( extension ) )
					//		{
					//			extension = extension.Substring( 1 );
					//			ResourceType resourceType = ResourceTypeManager.Instance.GetByExtension( extension );
					//			if( resourceType != null )
					//			{
					//				menuItem = new KryptonContextMenuItem( Translate( "Edit" ), imageListContextMenu.Images[ "Edit_16.png" ],
					//					delegate ( object s, EventArgs e2 )
					//					{
					//						TryBeginEditMode();
					//					} );
					//				menu.Items.Add( menuItem );
					//			}
					//		}
					//	}

					//	//Open in External Program
					//	{
					//		menuItem = new KryptonContextMenuItem( Translate( "Open in External Program" ), null, delegate ( object s, EventArgs e2 )
					//		{
					//			string realPath = VirtualFileSystem.GetRealPathByVirtual( nodePath );
					//			Win32Utils.ShellExecuteEx( null, realPath );
					//		} );
					//		menu.Items.Add( menuItem );
					//	}
					//}

					////Add resource type specific items
					//if( node.Tag as string != null )
					//{
					//	string path = GetNodePath( node );
					//	string extension = Path.GetExtension( path );
					//	if( !string.IsNullOrEmpty( extension ) )
					//	{
					//		extension = extension.Substring( 1 );
					//		ResourceType resourceType = ResourceTypeManager.Instance.GetByExtension( extension );
					//		if( resourceType != null )
					//			resourceType.DoResourcesTreeContextMenu( path, menu );
					//	}
					//}

					//if( selectedNodes.Count != 0 )
					//{
					//	bool rootNodeSelected = false;
					//	{
					//		foreach( MyTreeNode n in selectedNodes )
					//			if( n.Parent == null )
					//				rootNodeSelected = true;
					//	}

					//	//if( !rootNodeSelected )
					//	{
					//		//separator
					//		menu.Items.Add( new ToolStripSeparator() );

					//		//cut
					//		menuItem = new KryptonContextMenuItem( Translate( "Cut" ), imageListContextMenu.Images[ "Cut_16.png" ],
					//			delegate ( object s, EventArgs e2 ) { CutCopyFiles( true ); } );
					//		menuItem.Enabled = !rootNodeSelected;
					//		menu.Items.Add( menuItem );

					//		//copy
					//		menuItem = new KryptonContextMenuItem( Translate( "Copy" ), imageListContextMenu.Images[ "Copy_16.png" ],
					//			delegate ( object s, EventArgs e2 ) { CutCopyFiles( false ); } );
					//		menu.Items.Add( menuItem );
					//	}

					//	//paste
					//	//if( IsExistsDataToPaste() )
					//	if( node.Tag == null )
					//	{
					//		////separator
					//		//if( rootNodeSelected )
					//		//   menu.Items.Add( new ToolStripSeparator() );

					//		string directoryToPaste = null;
					//		if( IsExistsDataToPaste() )
					//			directoryToPaste = GetSelectedDirectoryToPaste();

					//		//string directoryToPaste = GetSelectedDirectoryToPaste();
					//		//if( directoryToPaste != null )
					//		{
					//			menuItem = new KryptonContextMenuItem( Translate( "Paste" ), imageListContextMenu.Images[ "Paste_16.png" ],
					//				delegate ( object s, EventArgs e2 ) { PasteFiles( directoryToPaste ); } );
					//			menuItem.Enabled = directoryToPaste != null;
					//			menu.Items.Add( menuItem );
					//		}
					//	}

					//	//if( !rootNodeSelected )
					//	{
					//		//separator
					//		menu.Items.Add( new ToolStripSeparator() );

					//		//delete
					//		menuItem = new KryptonContextMenuItem( Translate( "Delete" ), imageListContextMenu.Images[ "Delete_16.png" ],
					//			delegate ( object s, EventArgs e2 ) { TryDeleteObjects(); } );
					//		menuItem.Enabled = !rootNodeSelected;
					//		menu.Items.Add( menuItem );
					//	}
					//	//}

					//	//if( ( ( node.Tag == null && node.Parent != null ) || node.Tag != null ) && selectedNodes.Count == 1 )
					//	{
					//		//rename
					//		menuItem = new KryptonContextMenuItem( Translate( "Rename" ), null, delegate ( object s, EventArgs e2 )
					//		{
					//			//Try begin edit label
					//			if( node != treeView.SelectedNode )
					//				return;
					//			if( !node.IsEditing )
					//				node.BeginEdit();
					//		} );
					//		menuItem.Enabled = selectedNodes.Count == 1 && !rootNodeSelected;
					//		menu.Items.Add( menuItem );
					//	}
					//}

					////Sort by, Refresh
					//{
					//	//separator
					//	menu.Items.Add( new ToolStripSeparator() );

					//	AddSortByToContextMenu( menu );

					//	//Refresh
					//	{
					//		menuItem = new KryptonContextMenuItem( Translate( "Refresh" ), imageListContextMenu.Images[ "Refresh_16.png" ],
					//			delegate ( object s, EventArgs e2 )
					//			{
					//				RefreshPath( (MyTreeNode)node );
					//			} );
					//		menu.Items.Add( menuItem );
					//	}
					//}


#if CLOUD
					if( EngineInfo.EngineMode == EngineInfo.EngineModeEnum.CloudClient )
					{
						List<string> selectedFilesWithInsideSelectedFolders;
						{
							var selectedFilesSet = new ESet<string>();

							foreach( var item in SelectedItems )
							{
								var fileItem2 = item as ContentBrowserItem_File;
								if( fileItem2 != null )
								{
									//!!!!какие отсечь?

									if( fileItem2.IsDirectory )
									{
										try
										{
											foreach( var fullPath in Directory.GetFiles( fileItem2.FullPath, "*", SearchOption.AllDirectories ) )
											{
												var fileName = VirtualPathUtility.GetAllFilesPathByReal( fullPath );
												if( !string.IsNullOrEmpty( fileName ) )
													selectedFilesSet.AddWithCheckAlreadyContained( fileName );
											}
										}
										catch { }
									}
									else
									{
										var fileName = VirtualPathUtility.GetAllFilesPathByReal( fileItem2.FullPath );
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
							foreach( var item in SelectedItems )
							{
								var fileItem2 = item as ContentBrowserItem_File;
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
									RepositoryActionsWithServer.Get( selectedFullPathFolders, selectedFullPathFiles, EngineInfo.CloudProjectInfo.ID, true, VirtualFileSystem.Directories.AllFiles, EditorForm.Instance );
								} );
							item.Enabled = selectedFullPathFolders.Count != 0 || selectedFullPathFiles.Count != 0;
							items2.Add( item );
						}

						//Commit
						{
							var item = new KryptonContextMenuItem( Translate( "Commit Selected" ), EditorResourcesCache.MoveUp,
							   delegate ( object s, EventArgs e2 )
							   {
								   RepositoryActionsWithServer.Commit( selectedFullPathFolders, selectedFullPathFiles, EngineInfo.CloudProjectInfo.ID, true, true, VirtualFileSystem.Directories.AllFiles, EditorForm.Instance );
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

								EditorAPI2.FindWindow<ResourcesWindow>().Invalidate( true );
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
							//itemAdd.Enabled = selectedFilesWithInsideSelectedFolders.Count != 0;
							items2.Add( itemAdd );

							//var item = new KryptonContextMenuItem( Translate( "Add" ), EditorResourcesCache.Add,
							//	delegate ( object s, EventArgs e2 )
							//	{
							//		var formItems = new List<RepositoryItemsForm.Item>();

							//		foreach( var fileName in selectedFilesWithInsideSelectedFolders )
							//		{
							//			var formItem = new RepositoryItemsForm.Item();
							//			formItem.FileName = fileName;
							//			formItems.Add( formItem );
							//		}

							//		var form = new RepositoryItemsForm( "Add Files", "Select files to add:", formItems.ToArray(), true );
							//		if( form.ShowDialog() != DialogResult.OK )
							//			return;

							//		var checkedFormItems = form.GetCheckedItems();
							//		RepositoryLocal.Add( checkedFormItems.Select( i => i.FileName ).ToArray() );

							//		EditorAPI.FindWindow<ResourcesWindow>().Invalidate( true );
							//	} );

							//item.Enabled = selectedFilesWithInsideSelectedFolders.Count != 0;
							////item.Enabled = selectedFiles.Exists( fileName => WorldLocalRepository.GetFileItem( fileName ) == null );
							//items2.Add( item );
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
								//var form = new RepositoryItemsForm( "Set Sync Mode", "Select files to set sync mode:", formItems.ToArray(), true );
								if( form.ShowDialog() != DialogResult.OK )
									return;

								var checkedFormItems = form.GetCheckedItems();
								RepositoryLocal.SetSyncMode( checkedFormItems.Select( i => i.FileName ).ToArray(), mode );

								EditorAPI2.FindWindow<ResourcesWindow>().Invalidate( true );
							}

							var itemChangeSyncMode = new KryptonContextMenuItem( Translate( "Sync Mode" ), null );
							//var itemChangeSyncMode = new KryptonContextMenuItem( Translate( "Set Sync Mode" ), null );

							var items3 = new List<KryptonContextMenuItemBase>();

							{
								var item = new KryptonContextMenuItem( Translate( "Sync with Clients" ), EditorResourcesCache.Synchronize,
									delegate ( object s, EventArgs e2 )
									{
										SetSyncMode( RepositorySyncMode.Synchronize );
									} );
								//item.Enabled = selectedFilesWithInsideSelectedFolders.Exists( fileName => RepositoryServerState.GetFileItem( fileName ) != null || RepositoryLocal.FileItemExists( fileName ) );
								items3.Add( item );
							}

							{
								var item = new KryptonContextMenuItem( Translate( "Server Only" ), EditorResourcesCache.ServerOnly,
									delegate ( object s, EventArgs e2 )
									{
										SetSyncMode( RepositorySyncMode.ServerOnly );
									} );
								//item.Enabled = selectedFilesWithInsideSelectedFolders.Exists( fileName => RepositoryServerState.GetFileItem( fileName ) != null || RepositoryLocal.FileItemExists( fileName ) );
								items3.Add( item );
							}

							//{
							//	var item = new KryptonContextMenuItem( Translate( "Storage Only" ), EditorResourcesCache.StorageOnly,
							//		delegate ( object s, EventArgs e2 )
							//		{
							//			SetSyncMode( RepositorySyncMode.StorageOnly );
							//		} );
							//	item.Enabled = selectedFilesWithInsideSelectedFolders.Exists( fileName => RepositoryServerState.GetFileItem( fileName ) != null || RepositoryLocal.FileItemExists( fileName ) );
							//	items3.Add( item );
							//}

							//{
							//	var item = new KryptonContextMenuItem( Translate( "Revert" ), EditorResourcesCache.Undo,
							//		delegate ( object s, EventArgs e2 )
							//		{
							//			SetSyncMode( null );
							//		} );
							//	//item.Enabled = selectedFilesWithInsideSelectedFolders.Exists( fileName => RepositoryServerState.GetFileItem( fileName ) != null || RepositoryLocal.FileItemExists( fileName ) );
							//	items3.Add( item );
							//}

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

									EditorAPI2.FindWindow<ResourcesWindow>().Invalidate( true );
								} );

							item.Enabled = selectedFilesWithInsideSelectedFolders.Count != 0;
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

									EditorAPI2.FindWindow<ResourcesWindow>().Invalidate( true );
								} );

							item.Enabled = selectedFilesWithInsideSelectedFolders.Exists( fileName => RepositoryServerState.GetFileItem( fileName ) != null || RepositoryLocal.FileItemExists( fileName ) );
							//item.Enabled = selectedFilesWithInsideSelectedFolders.Count != 0;
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

									EditorAPI2.FindWindow<ResourcesWindow>().Invalidate( true );
								} );

							item.Enabled = selectedFilesWithInsideSelectedFolders.Exists( fileName => RepositoryLocal.FileItemExists( fileName ) );
							//item.Enabled = selectedFilesWithInsideSelectedFolders.Exists( fileName => RepositoryServerState.GetFileItem( fileName ) != null || RepositoryLocal.FileItemExists( fileName ) );
							//item.Enabled = selectedFilesWithInsideSelectedFolders.Count != 0;
							items2.Add( item );
						}

						////separator
						//items2.Add( new KryptonContextMenuSeparator() );

						////separator
						//items2.Add( new KryptonContextMenuSeparator() );

						////Settings
						//{
						//	var item = new KryptonContextMenuItem( Translate( "Settings" ), EditorResourcesCache.Properties,
						//		delegate ( object s, EventArgs e2 )
						//		{
						//			RepositorySettingsFile.Settings settings = null;

						//			var repositorySettingsFile = Path.Combine( VirtualFileSystem.Directories.Project, "Repository.settings" );
						//			if( File.Exists( repositorySettingsFile ) )
						//			{
						//				if( !RepositorySettingsFile.Load( repositorySettingsFile, out settings, out var error2 ) )
						//				{
						//					Log.Warning( "Unable to load Repository.settings. " + error2 );
						//					return;
						//				}
						//			}

						//			var form = new RepositorySettingsForm( settings );
						//			if( form.ShowDialog() != DialogResult.OK )
						//				return;

						//			var newSettings = form.GetNewSettings();

						//			RepositorySettingsFile.Save( repositorySettingsFile, newSettings, out var error );
						//			if( !string.IsNullOrEmpty( error ) )
						//			{
						//				Log.Warning( "Unable to save Repository.settings. " + error );
						//				return;
						//			}

						//			ScreenNotifications.Show( Translate( "The file was updated successfully." ) );
						//		} );

						//	item.Enabled = selectedFullPathFolders.Count != 0 || selectedFullPathFiles.Count != 0;
						//	//item.Enabled = selectedFiles.Exists( fileName => WorldLocalRepository.GetFileItem( fileName ) == null );
						//	items2.Add( item );
						//}

						itemWorld.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
						items.Add( itemWorld );

						//separator
						items.Add( new KryptonContextMenuSeparator() );
					}
#endif

					//Cut
					{
						var item = new KryptonContextMenuItem( Translate( "Cut" ), EditorResourcesCache.Cut,
							delegate ( object s, EventArgs e2 )
							{
								Cut();
							} );
						item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Cut" );
						item.Enabled = CanCut();
						items.Add( item );
					}

					//Copy
					{
						var item = new KryptonContextMenuItem( Translate( "Copy" ), EditorResourcesCache.Copy,
							delegate ( object s, EventArgs e2 )
							{
								Copy();
							} );
						item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Copy" );
						item.Enabled = CanCopy();
						items.Add( item );
					}

					//Paste
					if( fileItem.IsDirectory || ( !fileItem.IsDirectory && CanPaste( out _, out _, out _, out _ ) ) )
					{
						var item = new KryptonContextMenuItem( Translate( "Paste" ), EditorResourcesCache.Paste,
							delegate ( object s, EventArgs e2 )
							{
								Paste();
							} );
						item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Paste" );
						item.Enabled = CanPaste( out _, out _, out _, out _ );
						items.Add( item );
					}

					//Favorites
					if( EditorFavorites.AllowFavorites && !fileItem.IsDirectory )
					{
						//separator
						items.Add( new KryptonContextMenuSeparator() );

						//var virtualFileName = VirtualPathUtility.GetVirtualPathByReal( fileItem.FullPath );

						if( contentItem.Parent != favoritesItem )
						{
							//Add to Favorites

							var item = new KryptonContextMenuItem( Translate( "Add to Favorites" ), EditorResourcesCache.Add,
							   delegate ( object s, EventArgs e2 )
							   {
								   FavoritesAddSelectedItems();

								   //update Favorites group
								   ContentBrowser contentBrowser = EditorAPI2.FindWindow<ResourcesWindow>().ContentBrowser1;
								   if( contentBrowser != this )
									   contentBrowser.favoritesItem?.PerformChildrenChanged();
								   favoritesItem?.PerformChildrenChanged();
							   } );
							item.Enabled = FavoritesCheckEnableItem( true );
							items.Add( item );
						}
						else
						{
							//Remove from Favorites

							var item = new KryptonContextMenuItem( Translate( "Remove from Favorites" ), EditorResourcesCache.Delete,
							   delegate ( object s, EventArgs e2 )
							   {
								   FavoritesRemoveSelectedItems();

								   //update Favorites group
								   ContentBrowser contentBrowser = EditorAPI2.FindWindow<ResourcesWindow>().ContentBrowser1;
								   if( contentBrowser != this )
									   contentBrowser.favoritesItem?.PerformChildrenChanged();
								   favoritesItem?.PerformChildrenChanged();
							   } );
							item.Enabled = FavoritesCheckEnableItem( false );
							items.Add( item );
						}
					}

					//separator
					items.Add( new KryptonContextMenuSeparator() );

					//Delete
					{
						var item = new KryptonContextMenuItem( Translate( "Delete" ), EditorResourcesCache.Delete,
							delegate ( object s, EventArgs e2 )
							{
								TryDeleteObjects();
							} );
						item.Enabled = CanDeleteObjects( out _ );
						items.Add( item );
					}

					//Rename
					{
						var item = new KryptonContextMenuItem( Translate( "Rename" ), null,
							delegate ( object s, EventArgs e2 )
							{
								RenameBegin();
							} );
						item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Rename" );
						item.Enabled = CanRename();
						items.Add( item );
					}

					//separator
					items.Add( new KryptonContextMenuSeparator() );

					//Sort by
					if( fileItem != null && fileItem.IsDirectory )
					{
						AddSortByToContextMenu( items );

						//separator
						items.Add( new KryptonContextMenuSeparator() );
					}

					//Properties
					{
						var item = new KryptonContextMenuItem( Translate( "Properties" ), null, delegate ( object s, EventArgs e2 )
						{
							//!!!!!
							string realPath = fileItem.FullPath;
							//string realPath = currentOrParentDirectoryItem.FullPath;// VirtualPathUtils.GetRealPathByVirtual( nodePath );
							Win32Utility.ShellExecuteEx( "properties", realPath );
						} );
						item.Enabled = fileItem != null;
						//item.Enabled = currentOrParentDirectoryItem != null;
						items.Add( item );
					}
				}
			}

			//_Type
			if( contentItem != null )
			{
				var typeItem = contentItem as ContentBrowserItem_Type;
				if( typeItem != null )
				{
					//New resource
					{
						//var item = new KryptonContextMenuItem( Translate( "New Resource of This Type" ), EditorResourcesCache.New,
						var item = new KryptonContextMenuItem( Translate( "New Resource" ), EditorResourcesCache.New,
						   delegate ( object s, EventArgs e2 )
						   {
							   var initData = new NewObjectWindow.CreationDataClass();
							   initData.initFileCreationDirectory = VirtualDirectory.Exists( "New" ) ? "New" : "";
							   //initData.initFileCreationDirectory = VirtualDirectory.Exists( "Created" ) ? "Created" : "";
							   initData.initLockType = typeItem.Type;
							   initData.createdFromContentBrowser = this;
							   EditorAPI2.OpenNewObjectWindow( initData );
						   } );
						item.Enabled =
							MetadataManager.GetTypeOfNetType( typeof( Component ) ).IsAssignableFrom( typeItem.Type ) ||
							MetadataManager.GetTypeOfNetType( typeof( NewResourceType ) ).IsAssignableFrom( typeItem.Type );
						items.Add( item );
					}

					//Favorites
					if( EditorFavorites.AllowFavorites )
					{
						if( contentItem.Parent != favoritesItem )
						{
							//Add to Favorites

							var item = new KryptonContextMenuItem( Translate( "Add to Favorites" ), EditorResourcesCache.Add,
							   delegate ( object s, EventArgs e2 )
							   {
								   FavoritesAddSelectedItems();

								   //update Favorites group
								   ContentBrowser contentBrowser = EditorAPI2.FindWindow<ResourcesWindow>().ContentBrowser1;
								   if( contentBrowser != this )
									   contentBrowser.favoritesItem?.PerformChildrenChanged();
								   favoritesItem?.PerformChildrenChanged();
							   } );
							item.Enabled = FavoritesCheckEnableItem( true );
							items.Add( item );
						}
						else
						{
							//Remove from Favorites

							var item = new KryptonContextMenuItem( Translate( "Remove from Favorites" ), EditorResourcesCache.Delete,
							   delegate ( object s, EventArgs e2 )
							   {
								   FavoritesRemoveSelectedItems();

								   //update Favorites group
								   ContentBrowser contentBrowser = EditorAPI2.FindWindow<ResourcesWindow>().ContentBrowser1;
								   if( contentBrowser != this )
									   contentBrowser.favoritesItem?.PerformChildrenChanged();
								   favoritesItem?.PerformChildrenChanged();
							   } );
							item.Enabled = FavoritesCheckEnableItem( false );
							items.Add( item );
						}
					}

					//Learn more about the type
					{
						var type = typeItem.Type;
						var link = DocumentationLinksManager.GetFullLinkForType( type.GetNetType() );

						var item = new KryptonContextMenuItem( Translate( "Learn More" ), EditorResourcesCache.Help,
						   delegate ( object s, EventArgs e2 )
						   {
							   if( !string.IsNullOrEmpty( link ) )
								   Process.Start( new ProcessStartInfo( link ) { UseShellExecute = true } );
						   } );
						item.Enabled = !string.IsNullOrEmpty( link );
						items.Add( item );
					}
				}
			}

			var menuType = EditorActionContextMenuType.General;
			if( Mode == ModeEnum.Resources )
				menuType = EditorActionContextMenuType.Resources;
			else if( DocumentWindow != null )
				menuType = EditorActionContextMenuType.Document;

			EditorContextMenuWinForms.AddActionsToMenu( menuType, items );

			ShowContextMenuEvent?.Invoke( this, contentItem, items );

			EditorContextMenuWinForms.Show( items, locationControl, locationPoint );
		}
	}
}

#endif