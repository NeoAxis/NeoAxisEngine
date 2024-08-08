#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis.Editor
{
	public static class EditorContextMenu
	{
		public abstract class ItemBase
		{
		}

		/////////////////////////////////////////

		public abstract class Item : ItemBase
		{
			//string text;
			//EventHandler clickHandler;

			//protected Item( string text, EventHandler clickHandler )
			//{
			//	this.text = text;
			//	this.clickHandler = clickHandler;
			//}

			public string Text
			{
				get;
				set;
			}

			//public EventHandler ClickHandler
			//{
			//	get;
			//	set;
			//}

			public bool Enabled
			{
				get;
				set;
			}

			public string ShortcutKeyDisplayString
			{
				get;
				set;
			}
		}

		/////////////////////////////////////////

		public abstract class Separator : ItemBase
		{
		}

		/////////////////////////////////////////

		public static Item NewItem( string text, EventHandler clickHandler )
		{
			return EditorAssemblyInterface.Instance.EditorContextMenuNewItem( text, clickHandler );
		}

		public static Separator NewSeparator()
		{
			return EditorAssemblyInterface.Instance.EditorContextMenuNewSeparator();
		}

		public static void Show( ICollection<ItemBase> items, Vector2I screenPosition )
		{
			EditorAssemblyInterface.Instance.EditorContextMenuShow( items, screenPosition );

			//if( items.Count == 0 )
			//	return;

			//var realItems = new List<KryptonContextMenuItemBase>();
			//foreach( var item in items )
			//	realItems.Add( item.RealItem );

			//var menu = new KryptonContextMenu();
			//menu.Items.Add( new KryptonContextMenuItems( realItems.ToArray() ) );
			//menu.Show( EditorForm.Instance, new Point( screenPosition.X, screenPosition.Y ) );
		}

		public static void Show( ICollection<ItemBase> items )
		{
			EditorAssemblyInterface.Instance.EditorContextMenuShow( items, null );

			//var p = Cursor.Position;
			//Show( items, new Vector2I( p.X, p.Y ) );
		}

		public static string Translate( string text )
		{
			return EditorLocalization.Translate( "ContextMenu", text );
		}
	}
}
#endif