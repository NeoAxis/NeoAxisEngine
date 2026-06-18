// Copyright 2006–2026 Ivan Efimov. All rights reserved.
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
		/// <summary>
		/// The height of the item.
		/// </summary>
		[DefaultValue( "Screen 0.022" )]
		public Reference<UIMeasureValueDouble> ItemSize
		{
			get { if( _itemSize.BeginGet() ) ItemSize = _itemSize.Get( this ); return _itemSize.value; }
			set { if( _itemSize.BeginSet( this, ref value ) ) { try { ItemSizeChanged?.Invoke( this ); } finally { _itemSize.EndSet(); } } }
		}
		public event Action<UIContextMenu> ItemSizeChanged;
		ReferenceField<UIMeasureValueDouble> _itemSize = new UIMeasureValueDouble( UIMeasure.Screen, 0.022 );

		/// <summary>
		/// The margin of the item.
		/// </summary>
		[DefaultValue( "Parent 0 0" )]
		[Category( "Layout" )]
		public Reference<UIMeasureValueVector2> ItemMargin
		{
			get { if( _itemMargin.BeginGet() ) ItemMargin = _itemMargin.Get( this ); return _itemMargin.value; }
			set { if( _itemMargin.BeginSet( this, ref value ) ) { try { ItemMarginChanged?.Invoke( this ); } finally { _itemMargin.EndSet(); } } }
		}
		public event Action<UIControl> ItemMarginChanged;
		ReferenceField<UIMeasureValueVector2> _itemMargin;

		/// <summary>
		/// The font size of the text.
		/// </summary>
		[DefaultValue( "Units 20" )]
		public Reference<UIMeasureValueDouble> ItemFontSize
		{
			get { if( _itemFontSize.BeginGet() ) ItemFontSize = _itemFontSize.Get( this ); return _itemFontSize.value; }
			set { if( _itemFontSize.BeginSet( this, ref value ) ) { try { ItemFontSizeChanged?.Invoke( this ); } finally { _itemFontSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TitleBarFontSize"/> property value changes.</summary>
		public event Action<UIContextMenu> ItemFontSizeChanged;
		ReferenceField<UIMeasureValueDouble> _itemFontSize = new UIMeasureValueDouble( UIMeasure.Units, 20 );

		//

		[Browsable( false )]
		public List<ItemBase> Items
		{
			get { return items; }
			set { items = value; }
		}
		List<ItemBase> items = new List<ItemBase>();

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
			//!!!!
			public /*event*/ ClickDelegate Click;

			//

			//!!!!impl Image

			//public Item( string text, ImageComponent image, ClickDelegate click )
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

			public ImageComponent Image
			{
				get { return image; }
				set { image = value; }
			}
			ImageComponent image;

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

		public void Show( UIControl parent, Vector2 screenPosition )
		{
			//control to process mouse down outside context menu
			var backControl = parent.CreateComponent<UIControl>( enabled: false );
			backControl.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, 0, 0, 1, 1 );
			backControl.Size = new UIMeasureValueVector2( UIMeasure.Screen, 1, 1 );
			backControl.CoverOtherControls = CoverOtherControlsEnum.AllPreviousInHierarchy;
			backControl.MouseDown += BackControl_MouseDown;
			backControl.TopMost = true;

			//updating margins, sizes in the style classes

			backControl.AddComponent( this );
			initialScreenPosition = screenPosition;

			//TopMost = true;

			foreach( var itemBase in items )
			{
				var item = itemBase as Item;
				if( item != null )
				{
					var button = CreateComponent<UIButton>();
					button.Text = item.Text;
					button.ReadOnly = !item.Enabled;
					button.AnyData = item;
					button.FontSize = ItemFontSize;

					button.Click += delegate ( UIButton sender )
					{
						var item2 = (Item)sender.AnyData;
						item2.Click?.Invoke( this, item2 );

						Parent.RemoveFromParent( true );
					};
				}
			}

			backControl.Enabled = true;
		}

		public void Show( UIControl parent )
		{
			Show( parent, parent.ConvertLocalToScreen( parent.MousePosition ) );
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
