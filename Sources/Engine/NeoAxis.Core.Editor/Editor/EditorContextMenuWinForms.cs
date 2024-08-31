#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Text;
using System.Drawing;
using Internal.ComponentFactory.Krypton.Toolkit;
using System.ComponentModel;
using System.Linq;

namespace NeoAxis.Editor
{
	//!!!!

	//!!!!всё унифицировать по контекстным меню
	//!!!!!!всем одинаковая возможность добавлять/удалять свои элементы

	public static class EditorContextMenuWinForms
	{
		//!!!!так? - Point? locationPoint
		public delegate void ShowEventDelegate( KryptonContextMenu menu, Control locationControl, Point? locationPoint );
		public static event ShowEventDelegate ShowEvent;

		//

		//public enum MenuTypeEnum
		//{
		//	None,
		//	General,
		//	Resources,
		//	Document,
		//}

		//

		public static KryptonContextMenu Create()
		{
			var menu = new KryptonContextMenu();
			return menu;
		}

		public static void Show( KryptonContextMenu menu, Control locationControl, Point locationPoint )
		{
			ShowEvent?.Invoke( menu, locationControl, locationPoint );

			//if( menu.Items.Count == 0 )
			//	return;

			//!!!!
			menu.Show( locationControl, locationControl.PointToScreen( locationPoint ) );
		}

		public static void Show( KryptonContextMenu menu, Control locationControl )
		{
			ShowEvent?.Invoke( menu, locationControl, null );

			menu.Show( locationControl, Cursor.Position );
			//menu.Show( locationControl, locationControl.PointToClient( Cursor.Position ) );
		}

		public static void Show( ICollection<KryptonContextMenuItemBase> items, Control locationControl, Point locationPoint )
		{
			if( items.Count == 0 )
				return;

			var menu = Create();

			var array = new KryptonContextMenuItemBase[ items.Count ];
			items.CopyTo( array, 0 );
			menu.Items.Add( new KryptonContextMenuItems( array ) );

			Show( menu, locationControl, locationPoint );
		}

		public static void Show( ICollection<KryptonContextMenuItemBase> items, Control locationControl )
		{
			if( items.Count == 0 )
				return;

			var menu = Create();

			var array = new KryptonContextMenuItemBase[ items.Count ];
			items.CopyTo( array, 0 );
			menu.Items.Add( new KryptonContextMenuItems( array ) );

			Show( menu, locationControl );
		}

		public static string Translate( string text )
		{
			return EditorLocalization2.Translate( "ContextMenu", text );
		}

		public static void AddTransformToolToMenu( ICollection<KryptonContextMenuItemBase> items, TransformToolClass transformTool )
		//public static void AddTransformToolToMenu( ICollection<KryptonContextMenuItemBase> items, string workareaModeName )// TransformTool transformTool )
		{
			KryptonContextMenuItem item;
			string text;

			//Select
			text = Translate( "Select" );
			item = new KryptonContextMenuItem( text, null, delegate ( object sender, EventArgs e )
			{
				EditorAPI2.EditorActionClick( EditorActionHolder.ContextMenu, "Select" );
			} );
			item.Checked = transformTool.Mode == TransformToolMode.None;
			//item.Checked = workareaModeName == "Transform Select";
			item.Image = EditorResourcesCache.Select;
			item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Select" );
			items.Add( item );

			//Move & Rotate
			text = Translate( "Move && Rotate" );
			item = new KryptonContextMenuItem( text, null, delegate ( object sender, EventArgs e )
			{
				EditorAPI2.EditorActionClick( EditorActionHolder.ContextMenu, "Move & Rotate" );
			} );
			item.Checked = transformTool.Mode == TransformToolMode.PositionRotation;
			//item.Checked = workareaModeName == "Transform Move";
			item.Image = EditorResourcesCache.MoveRotate;
			item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Move & Rotate" );
			items.Add( item );

			//Move
			text = Translate( "Move" );
			item = new KryptonContextMenuItem( text, null, delegate ( object sender, EventArgs e )
			{
				EditorAPI2.EditorActionClick( EditorActionHolder.ContextMenu, "Move" );
			} );
			item.Checked = transformTool.Mode == TransformToolMode.Position;
			//item.Checked = workareaModeName == "Transform Move";
			item.Image = EditorResourcesCache.Move;
			item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Move" );
			items.Add( item );

			//Rotate
			text = Translate( "Rotate" );
			item = new KryptonContextMenuItem( text, null, delegate ( object sender, EventArgs e )
			{
				EditorAPI2.EditorActionClick( EditorActionHolder.ContextMenu, "Rotate" );
			} );
			item.Checked = transformTool.Mode == TransformToolMode.Rotation;
			//item.Checked = workareaModeName == "Transform Rotate";
			item.Image = EditorResourcesCache.Rotate;
			item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Rotate" );
			items.Add( item );

			//Scale
			text = Translate( "Scale" );
			item = new KryptonContextMenuItem( text, null, delegate ( object sender, EventArgs e )
			{
				EditorAPI2.EditorActionClick( EditorActionHolder.ContextMenu, "Scale" );
			} );
			item.Checked = transformTool.Mode == TransformToolMode.Scale;
			//item.Checked = workareaModeName == "Transform Scale";
			item.Image = EditorResourcesCache.Scale;
			item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Scale" );
			items.Add( item );
		}

		public static void AddActionsToMenu( EditorActionContextMenuType menuType, ICollection<KryptonContextMenuItemBase> items )//, DocumentWindow documentWindow )
		{
			bool firstItem = true;

			foreach( var action in EditorActions.Actions )
			{
				if( !action.CompletelyDisabled && action.ContextMenuSupport != EditorActionContextMenuType.None && action.ContextMenuSupport == menuType )
				{
					var state = EditorAPI2.EditorActionGetState( EditorActionHolder.ContextMenu, action );
					//bool enabled = false;
					//action.PerformUpdateContextMenuEvent( documentWindow, ref enabled );

					if( state.Enabled )
					{
						if( firstItem )
						{
							if( items.Count != 0 )
								items.Add( new KryptonContextMenuSeparator() );

							firstItem = false;
						}

						var image = EditorAPI2.GetImageForDispalyScale( action.GetImageSmall(), action.GetImageBig() );
						var item = new KryptonContextMenuItem( Translate( action.GetContextMenuText() ), image, delegate ( object s, EventArgs e2 )
						{
							////var a = (EditorAction)item.Tag;

							////!!!!так?
							var state2 = EditorAPI2.EditorActionGetState( EditorActionHolder.ContextMenu, action );
							//check still enabled
							if( state2.Enabled )
								EditorAPI2.EditorActionClick( EditorActionHolder.ContextMenu, action.Name );

							//action.PerformClickContextMenuEvent( documentWindow );
						} );

						if( action.ActionType == EditorAction.ActionTypeEnum.DropDown )
						{
							var args = new CancelEventArgs();
							action.DropDownContextMenu.PerformOpening( args );
							//!!!!?
							//if( !args.Cancel)
							//{

							foreach( var child in action.DropDownContextMenu.Items )
								item.Items.Add( child );
						}

						//!!!!
						//item.Enabled = !rootNodeSelected;
						item.Tag = action;
						item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( action.Name );
						items.Add( item );
					}
				}
			}
		}

		public delegate void TryNewObjectDelegate( Metadata.TypeInfo type, bool assetsFolderOnly );

		//!!!!тут?
		public static void AddNewObjectItem( IList<KryptonContextMenuItemBase> items, bool enabled, TryNewObjectDelegate select )
		{
			//New object
			{
				var newObjectItem = new KryptonContextMenuItem( Translate( "New Object" ), EditorResourcesCache.New, null );
				newObjectItem.Enabled = enabled;// CanNewObject( out _ );

				var items2 = new List<KryptonContextMenuItemBase>();

				//Select
				{
					var item2 = new KryptonContextMenuItem( Translate( "Select..." ), null,
						delegate ( object s, EventArgs e2 )
						{
							select( null, true );
							//TryNewObject( mouse, null );
						} );
					items2.Add( item2 );
				}

				//separator
				items2.Add( new KryptonContextMenuSeparator() );

				//ResourcesWindowItems
				{
					var menuItems = new Dictionary<string, KryptonContextMenuItem>();

					KryptonContextMenuItem GetBrowserItemByPath( string path )
					{
						menuItems.TryGetValue( path, out var item );
						return item;
					}

					foreach( var item in ResourcesWindowItems.Items )
					{
						//custom filtering
						if( !EditorUtility2.PerformResourcesWindowItemVisibleFilter( item ) )
							continue;

						var canCreateOutsideAssets = typeof( NewResourceType_TextFile ).IsAssignableFrom( item.Type ) || typeof( NewResourceType_CSharpClass ).IsAssignableFrom( item.Type );

						//skip
						if( !typeof( Component ).IsAssignableFrom( item.Type ) )
							continue;

						//remove Base prefix from items
						var itemPathFixed = item.Path;
						{
							var prefix = "Base\\";
							if( itemPathFixed.Length > prefix.Length && itemPathFixed.Substring( 0, prefix.Length ) == prefix )
								itemPathFixed = itemPathFixed.Substring( prefix.Length );
						}

						var strings = itemPathFixed.Split( new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries );

						string path = "";
						for( int n = 0; n < strings.Length; n++ )
						{
							path = Path.Combine( path, strings[ n ] );

							//get parent item
							KryptonContextMenuItem parentItem = null;
							if( n != 0 )
								parentItem = GetBrowserItemByPath( Path.GetDirectoryName( path ) );

							if( GetBrowserItemByPath( path ) == null )
							{
								//add item

								KryptonContextMenuItem menuItem = null;

								//is folder
								bool isFolder = n < strings.Length - 1;
								if( isFolder )
								{
									var text = EditorLocalization2.Translate( "ContentBrowser.Group", strings[ n ] );
									menuItem = new KryptonContextMenuItem( text, null, null );

									//ResourcesWindowItems.GroupDescriptions.TryGetValue( path, out var description );
									//if( !string.IsNullOrEmpty( description ) )
									//	menuItem2.Description = description;
								}
								else
								{
									var type = MetadataManager.GetTypeOfNetType( item.Type );

									Image image;
									{
										var imageKey = EditorImageHelperComponentTypes.GetImageKey( type );
										image = EditorImageHelperBasicImages.Helper.GetImageScaledForTreeView( imageKey, item.Disabled );
									}

									menuItem = new KryptonContextMenuItem( strings[ n ], image/*EditorResourcesCache.Type*/,
										delegate ( object s, EventArgs e2 )
										{
											var type2 = (Metadata.TypeInfo)( (KryptonContextMenuItem)s ).Tag;
											select( type2, !canCreateOutsideAssets );
											//TryNewObject( mouse, type2 );
										} );
									menuItem.Tag = type;

									//menuItem2.imageKey = GetTypeImageKey( type );

									menuItem.Enabled = !item.Disabled;
								}

								if( parentItem != null )
								{
									if( parentItem.Items.Count == 0 )
										parentItem.Items.Add( new KryptonContextMenuItems( new KryptonContextMenuItemBase[ 0 ] ) );
									var list = (KryptonContextMenuItems)parentItem.Items[ 0 ];
									list.Items.Add( menuItem );
									//parentItem.children.Add( menuItem );
								}

								menuItems.Add( path, menuItem );
								if( n == 0 )
									items2.Add( menuItem );
							}
						}
					}

				}

				//Favorites
				if( EditorFavorites.AllowFavorites )
				{
					var favoritesItem = new KryptonContextMenuItem( EditorLocalization2.Translate( "ContentBrowser.Group", "Favorites" ), null, null );

					var types = new List<Metadata.TypeInfo>( 32 );
					var files = new List<string>( 32 );
					foreach( var name in EditorFavorites.Favorites.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries ) )
					{
						var type = MetadataManager.GetTypeOfNetType( name );
						if( type != null )
							types.Add( type );
						else if( VirtualFile.Exists( name ) )
							files.Add( name );
					}

					CollectionUtility.MergeSort( types, delegate ( Metadata.TypeInfo t1, Metadata.TypeInfo t2 )
					{
						return string.Compare( t1.Name, t2.Name );
					} );

					CollectionUtility.MergeSort( files, delegate ( string name1, string name2 )
					{
						var n1 = Path.GetFileName( name1 );
						var n2 = Path.GetFileName( name2 );
						return string.Compare( n1, n2 );
					} );

					foreach( var type in types )
					{
						var text = type.DisplayName;
						{
							var item = ResourcesWindowItems.GetItemByType( type.GetNetType() );
							if( item != null )
							{
								try
								{
									text = Path.GetFileName( item.Path );
								}
								catch { }
							}
						}

						Image image;
						{
							var imageKey = EditorImageHelperComponentTypes.GetImageKey( type );
							image = EditorImageHelperBasicImages.Helper.GetImageScaledForTreeView( imageKey, false );
						}

						var menuItem = new KryptonContextMenuItem( text, image/*EditorResourcesCache.Type*/,
							delegate ( object s, EventArgs e2 )
							{
								var type2 = (Metadata.TypeInfo)( (KryptonContextMenuItem)s ).Tag;
								select( type2, false );
							} );
						menuItem.Tag = type;

						if( favoritesItem.Items.Count == 0 )
							favoritesItem.Items.Add( new KryptonContextMenuItems( new KryptonContextMenuItemBase[ 0 ] ) );
						var list = (KryptonContextMenuItems)favoritesItem.Items[ 0 ];
						list.Items.Add( menuItem );
					}

					//!!!!impl
					//foreach( var file in files )
					//{
					//	var text = file;
					//	{
					//		try
					//		{
					//			text = Path.GetFileName( file );
					//		}
					//		catch { }
					//	}

					//	var fullPath = VirtualPathUtility.GetRealPathByVirtual( file );

					//	Image image = null;
					//	try
					//	{
					//		//preview image
					//		image = PreviewImagesManager.GetImageForResource( fullPath, true );

					//		//image key
					//		if( image == null )
					//		{
					//			var imageKey = ResourceManager.GetResourceImageKey( fullPath );
					//			image = ContentBrowserImageHelperBasicImages.Helper.GetImageScaledForTreeView( imageKey, false );
					//		}

					//		if( image == null )
					//			image = EditorResourcesCache.Resource;
					//	}
					//	catch { }


					//	var menuItem = new KryptonContextMenuItem( text, image, delegate ( object s, EventArgs e2 )
					//	{
					//		//!!!!
					//		var type2 = (Metadata.TypeInfo)( (KryptonContextMenuItem)s ).Tag;
					//		select( type2 );
					//	} );
					//	menuItem.Tag = file;

					//	if( favoritesItem.Items.Count == 0 )
					//		favoritesItem.Items.Add( new KryptonContextMenuItems( new KryptonContextMenuItemBase[ 0 ] ) );
					//	var list = (KryptonContextMenuItems)favoritesItem.Items[ 0 ];
					//	list.Items.Add( menuItem );
					//}

					items2.Add( favoritesItem );
				}

				newObjectItem.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
				items.Add( newObjectItem );

				//KryptonContextMenuItem item = new KryptonContextMenuItem( Translate( "New object" ), Properties.Resources.New_16, delegate ( object s, EventArgs e2 )
				//{
				//	TryNewObject( mouse, null );
				//} );
				//item.Enabled = CanNewObject( out _ );
				//items.Add( item );
			}
		}

		public static void AddNewResourceItem( IList<KryptonContextMenuItemBase> items, bool enabled/*, bool insideAssets*/, TryNewObjectDelegate select )
		{
			var insideAssets = enabled;

			//New Resource
			{
				var newObjectItem = new KryptonContextMenuItem( Translate( "New Resource" ), EditorResourcesCache.New, null );
				newObjectItem.Enabled = enabled;

				if( newObjectItem.Enabled )
				{
					var items2 = new List<KryptonContextMenuItemBase>();

					if( insideAssets )
					{
						//Select
						{
							var item2 = new KryptonContextMenuItem( Translate( "Select..." ), null,
								delegate ( object s, EventArgs e2 )
								{
									select( null, true );
								} );
							items2.Add( item2 );
						}

						//separator
						items2.Add( new KryptonContextMenuSeparator() );
					}

					//ResourcesWindowItems
					{
						var menuItems = new Dictionary<string, KryptonContextMenuItem>();

						KryptonContextMenuItem GetBrowserItemByPath( string path )
						{
							menuItems.TryGetValue( path, out var item );
							return item;
						}

						foreach( var item in ResourcesWindowItems.Items )
						{
							//custom filtering
							if( !EditorUtility2.PerformResourcesWindowItemVisibleFilter( item ) )
								continue;

							var canCreateOutsideAssets = typeof( NewResourceType_TextFile ).IsAssignableFrom( item.Type );// || typeof( NewResourceType_CSharpClass ).IsAssignableFrom( item.Type );

							var allow = insideAssets || canCreateOutsideAssets;
							if( !allow )
								continue;

							////skip
							//if( !typeof( Component ).IsAssignableFrom( item.type ) )
							//	continue;

							//remove Base prefix from items
							var itemPathFixed = item.Path;
							{
								var prefix = "Base\\";
								if( itemPathFixed.Length > prefix.Length && itemPathFixed.Substring( 0, prefix.Length ) == prefix )
									itemPathFixed = itemPathFixed.Substring( prefix.Length );
							}

							var strings = itemPathFixed.Split( new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries );

							string path = "";
							for( int n = 0; n < strings.Length; n++ )
							{
								path = Path.Combine( path, strings[ n ] );

								//get parent item
								KryptonContextMenuItem parentItem = null;
								if( n != 0 )
									parentItem = GetBrowserItemByPath( Path.GetDirectoryName( path ) );

								if( GetBrowserItemByPath( path ) == null )
								{
									//add item

									KryptonContextMenuItem menuItem = null;

									//is folder
									bool isFolder = n < strings.Length - 1;
									if( isFolder )
									{
										menuItem = new KryptonContextMenuItem( strings[ n ], null, null );

										//ResourcesWindowItems.GroupDescriptions.TryGetValue( path, out var description );
										//if( !string.IsNullOrEmpty( description ) )
										//	menuItem2.Description = description;
									}
									else
									{
										var type = MetadataManager.GetTypeOfNetType( item.Type );

										Image image;
										{
											var imageKey = EditorImageHelperComponentTypes.GetImageKey( type );
											image = EditorImageHelperBasicImages.Helper.GetImageScaledForTreeView( imageKey, item.Disabled );
										}

										menuItem = new KryptonContextMenuItem( strings[ n ], image/*EditorResourcesCache.Type*/,
											delegate ( object s, EventArgs e2 )
											{
												var type2 = (Metadata.TypeInfo)( (KryptonContextMenuItem)s ).Tag;
												select( type2, !canCreateOutsideAssets );
												//TryNewObject( mouse, type2 );
											} );
										menuItem.Tag = type;

										menuItem.Enabled = !item.Disabled;
									}

									if( parentItem != null )
									{
										if( parentItem.Items.Count == 0 )
											parentItem.Items.Add( new KryptonContextMenuItems( new KryptonContextMenuItemBase[ 0 ] ) );
										var list = (KryptonContextMenuItems)parentItem.Items[ 0 ];
										list.Items.Add( menuItem );
										//parentItem.children.Add( menuItem );
									}

									menuItems.Add( path, menuItem );
									if( n == 0 )
										items2.Add( menuItem );
								}
							}
						}
					}

					newObjectItem.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
				}

				items.Add( newObjectItem );
			}
		}

	}
}

#endif