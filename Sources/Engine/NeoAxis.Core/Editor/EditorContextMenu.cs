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
using ComponentFactory.Krypton.Toolkit;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	//!!!!

	//!!!!всё унифицировать по контекстным меню
	//!!!!!!всем одинаковая возможность добавлять/удалять свои элементы

	public static class EditorContextMenu
	{
		//!!!!так? - Point? locationPoint
		public delegate void ShowEventDelegate( KryptonContextMenu menu, Control locationControl, Point? locationPoint );
		public static event ShowEventDelegate ShowEvent;

		//

		public enum MenuTypeEnum
		{
			None,
			General,
			Resources,
			Document,
		}

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
			return EditorLocalization.Translate( "ContextMenu", text );
		}

		public static void AddTransformToolToMenu( ICollection<KryptonContextMenuItemBase> items, TransformTool transformTool )
		//public static void AddTransformToolToMenu( ICollection<KryptonContextMenuItemBase> items, string workareaModeName )// TransformTool transformTool )
		{
			KryptonContextMenuItem item;
			string text;

			//Move
			text = Translate( "Move" );
			item = new KryptonContextMenuItem( text, null, delegate ( object sender, EventArgs e )
			{
				EditorAPI.EditorActionClick( EditorAction.HolderEnum.ContextMenu, "Move" );
			} );
			item.Checked = transformTool.Mode == TransformTool.ModeEnum.Position;
			//item.Checked = workareaModeName == "Transform Move";
			item.Image = EditorResourcesCache.Move;
			item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Move" );
			items.Add( item );

			//Rotate
			text = Translate( "Rotate" );
			item = new KryptonContextMenuItem( text, null, delegate ( object sender, EventArgs e )
			{
				EditorAPI.EditorActionClick( EditorAction.HolderEnum.ContextMenu, "Rotate" );
			} );
			item.Checked = transformTool.Mode == TransformTool.ModeEnum.Rotation;
			//item.Checked = workareaModeName == "Transform Rotate";
			item.Image = EditorResourcesCache.Rotate;
			item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Rotate" );
			items.Add( item );

			//Scale
			text = Translate( "Scale" );
			item = new KryptonContextMenuItem( text, null, delegate ( object sender, EventArgs e )
			{
				EditorAPI.EditorActionClick( EditorAction.HolderEnum.ContextMenu, "Scale" );
			} );
			item.Checked = transformTool.Mode == TransformTool.ModeEnum.Scale;
			//item.Checked = workareaModeName == "Transform Scale";
			item.Image = EditorResourcesCache.Scale;
			item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Scale" );
			items.Add( item );

			//Select
			text = Translate( "Select" );
			item = new KryptonContextMenuItem( text, null, delegate ( object sender, EventArgs e )
			{
				EditorAPI.EditorActionClick( EditorAction.HolderEnum.ContextMenu, "Select" );
			} );
			item.Checked = transformTool.Mode == TransformTool.ModeEnum.None;
			//item.Checked = workareaModeName == "Transform Select";
			item.Image = EditorResourcesCache.Select;
			item.ShortcutKeyDisplayString = EditorActions.GetFirstShortcutKeyString( "Select" );
			items.Add( item );
		}

		public static void AddActionsToMenu( MenuTypeEnum menuType, ICollection<KryptonContextMenuItemBase> items )//, DocumentWindow documentWindow )
		{
			bool firstItem = true;

			foreach( var action in EditorActions.Actions )
			{
				if( action.ContextMenuSupport != MenuTypeEnum.None && action.ContextMenuSupport == menuType )
				{
					var state = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.ContextMenu, action );
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

						var image = EditorAPI.GetImageForDispalyScale( action.GetImageSmall(), action.GetImageBig() );
						var item = new KryptonContextMenuItem( Translate( action.GetContextMenuText() ), image, delegate ( object s, EventArgs e2 )
						{
							////var a = (EditorAction)item.Tag;

							////!!!!так?
							var state2 = EditorAPI.EditorActionGetState( EditorAction.HolderEnum.ContextMenu, action );
							//check still enabled
							if( state2.Enabled )
								EditorAPI.EditorActionClick( EditorAction.HolderEnum.ContextMenu, action.Name );

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

		public delegate void TryNewObjectDelegate( Metadata.TypeInfo type );

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
							select( null );
							//TryNewObject( mouse, null );
						} );
					items2.Add( item2 );
				}

				//separator
				items2.Add( new KryptonContextMenuSeparator() );

				//ResourcesWindowItems
				{
					ResourcesWindowItems.PrepareItems();

					var menuItems = new Dictionary<string, KryptonContextMenuItem>();

					KryptonContextMenuItem GetBrowserItemByPath( string path )
					{
						menuItems.TryGetValue( path, out var item );
						return item;
					}

					foreach( var item in ResourcesWindowItems.Items )
					{
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
									var text = EditorLocalization.Translate( "ContentBrowser.Group", strings[ n ] );
									menuItem = new KryptonContextMenuItem( text, null, null );

									//ResourcesWindowItems.GroupDescriptions.TryGetValue( path, out var description );
									//if( !string.IsNullOrEmpty( description ) )
									//	menuItem2.Description = description;
								}
								else
								{
									var type = MetadataManager.GetTypeOfNetType( item.Type );
									menuItem = new KryptonContextMenuItem( strings[ n ], EditorResourcesCache.Type,
										delegate ( object s, EventArgs e2 )
										{
											var type2 = (Metadata.TypeInfo)( (KryptonContextMenuItem)s ).Tag;
											select( type2 );
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
				{
					var favoritesItem = new KryptonContextMenuItem( EditorLocalization.Translate( "ContentBrowser.Group", "Favorites" ), null, null );

					var types = new List<Metadata.TypeInfo>();
					foreach( var typeName in EditorFavorites.Favorites.Split( new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries ) )
					{
						var type = MetadataManager.GetType( typeName );
						if( type != null )
							types.Add( type );
					}

					foreach( var type in types )
					{
						var menuItem = new KryptonContextMenuItem( type.DisplayName, EditorResourcesCache.Type,
							delegate ( object s, EventArgs e2 )
							{
								var type2 = (Metadata.TypeInfo)( (KryptonContextMenuItem)s ).Tag;
								select( type2 );
							} );
						menuItem.Tag = type;

						if( favoritesItem.Items.Count == 0 )
							favoritesItem.Items.Add( new KryptonContextMenuItems( new KryptonContextMenuItemBase[ 0 ] ) );
						var list = (KryptonContextMenuItems)favoritesItem.Items[ 0 ];
						list.Items.Add( menuItem );
					}

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

		public static void AddNewResourceItem( IList<KryptonContextMenuItemBase> items, bool enabled, TryNewObjectDelegate select )
		{
			//New Resource
			{
				var newObjectItem = new KryptonContextMenuItem( Translate( "New Resource" ), EditorResourcesCache.New, null );
				newObjectItem.Enabled = enabled;

				var items2 = new List<KryptonContextMenuItemBase>();

				//Select
				{
					var item2 = new KryptonContextMenuItem( Translate( "Select..." ), null,
						delegate ( object s, EventArgs e2 )
						{
							select( null );
						} );
					items2.Add( item2 );
				}

				//separator
				items2.Add( new KryptonContextMenuSeparator() );

				//ResourcesWindowItems
				{
					ResourcesWindowItems.PrepareItems();

					var menuItems = new Dictionary<string, KryptonContextMenuItem>();

					KryptonContextMenuItem GetBrowserItemByPath( string path )
					{
						menuItems.TryGetValue( path, out var item );
						return item;
					}

					foreach( var item in ResourcesWindowItems.Items )
					{
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
									menuItem = new KryptonContextMenuItem( strings[ n ], EditorResourcesCache.Type,
										delegate ( object s, EventArgs e2 )
										{
											var type2 = (Metadata.TypeInfo)( (KryptonContextMenuItem)s ).Tag;
											select( type2 );
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

				newObjectItem.Items.Add( new KryptonContextMenuItems( items2.ToArray() ) );
				items.Add( newObjectItem );
			}
		}

	}
}
