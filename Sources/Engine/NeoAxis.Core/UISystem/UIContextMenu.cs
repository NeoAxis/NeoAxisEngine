// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

//!!!!итемы могут быть вложенными. может тогда как в UIMenu компонентами

namespace NeoAxis
{
	/// <summary>
	/// Represents a shortcut menu.
	/// </summary>
	public class UIContextMenu : UIControl
	{
		[Browsable( false )]
		public List<ItemBase> Items
		{
			get { return items; }
		}
		List<ItemBase> items;

		[Browsable( false )]
		public Vector2 InitialScreenPosition
		{
			get { return initialScreenPosition; }
		}
		Vector2 initialScreenPosition;

		/////////////////////////////////////////

		public abstract class ItemBase
		{
		}

		/////////////////////////////////////////

		public class Item : ItemBase
		{
			public delegate void ClickDelegate( UIContextMenu sender, Item item );
			public /*event*/ ClickDelegate Click;

			//

			//!!!!impl Image

			//public Item( string text, Component_Image image, ClickDelegate click )
			//{
			//	Text = text;
			//	Image = image;
			//	Click = click;
			//}

			public Item( string text, ClickDelegate click )
			//	: this( text, null, click )
			{
				Text = text;
				Click = click;
			}

			public string Text
			{
				get { return text; }
				set { text = value; }
			}
			string text = "";

			public Component_Image Image
			{
				get { return image; }
				set { image = value; }
			}
			Component_Image image;

			public bool Enabled
			{
				get { return enabled; }
				set { enabled = value; }
			}
			bool enabled = true;

			//public string ShortcutKeyDisplayString
			//{
			//	get { return item.ShortcutKeyDisplayString; }
			//	set { item.ShortcutKeyDisplayString = value; }
			//}
		}

		/////////////////////////////////////////

		public class Separator : ItemBase
		{
			public Separator()
			{
			}
		}

		/////////////////////////////////////////

		public UIContextMenu()
		{
		}

		public static UIContextMenu Show( UIControl parent, ICollection<ItemBase> items, Vector2 screenPosition )
		{
			//control to process mouse down outside context menu
			var backControl = parent.CreateComponent<UIControl>( enabled: false );
			backControl.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, 0, 0, 1, 1 );
			backControl.Size = new UIMeasureValueVector2( UIMeasure.Screen, 1, 1 );
			backControl.CoverOtherControls = CoverOtherControlsEnum.AllPreviousInHierarchy;
			backControl.MouseDown += BackControl_MouseDown;

			//updating margins, sizes in the style classes

			var menu = backControl.CreateComponent<UIContextMenu>();
			menu.items = new List<ItemBase>( items );
			menu.initialScreenPosition = screenPosition;

			foreach( var itemBase in items )
			{
				var item = itemBase as Item;
				if( item != null )
				{
					var button = menu.CreateComponent<UIButton>();
					button.Text = item.Text;
					button.ReadOnly = !item.Enabled;
					button.AnyData = item;

					button.Click += delegate ( UIButton sender )
					{
						var item2 = (Item)sender.AnyData;
						item2.Click?.Invoke( menu, item2 );

						menu.Parent.RemoveFromParent( true );
					};
				}
			}

			backControl.Enabled = true;
			return menu;
		}

		public static UIContextMenu Show( UIControl parent, ICollection<ItemBase> items )
		{
			return Show( parent, items, parent.ConvertLocalToScreen( parent.MousePosition ) );
		}

		private static void BackControl_MouseDown( UIControl sender, EMouseButtons button, ref bool handled )
		{
			var menu = sender.GetComponent<UIContextMenu>();

			//check clicking outside rectangle of the menu
			if( !new Rectangle( 0, 0, 1, 1 ).Contains( menu.MousePosition ) )
			{
				sender.RemoveFromParent( true );
				handled = true;
			}
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( e.Key == EKeys.Escape )
			{
				Parent.RemoveFromParent( true );
				return true;
			}

			return base.OnKeyDown( e );
		}

	}
}
