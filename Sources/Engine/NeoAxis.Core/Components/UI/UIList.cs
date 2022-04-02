// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Standard control that represents a list.
	/// </summary>
	public class UIList : UIControl
	{
		object touchDown;

		[Browsable( false )]
		public int NeedEnsureVisibleInStyle { get; set; } = -1;

		/// <summary>
		/// The height of a list element.
		/// </summary>
		[DefaultValue( "Screen 0.022" )]
		public Reference<UIMeasureValueDouble> ItemSize
		{
			get { if( _itemSize.BeginGet() ) ItemSize = _itemSize.Get( this ); return _itemSize.value; }
			set { if( _itemSize.BeginSet( ref value ) ) { try { ItemSizeChanged?.Invoke( this ); } finally { _itemSize.EndSet(); } } }
		}
		public event Action<UIList> ItemSizeChanged;
		ReferenceField<UIMeasureValueDouble> _itemSize = new UIMeasureValueDouble( UIMeasure.Screen, 0.022 );

		/// <summary>
		/// The font of a list element.
		/// </summary>
		[DefaultValue( null )]
		public Reference<FontComponent> Font
		{
			get { if( _font.BeginGet() ) Font = _font.Get( this ); return _font.value; }
			set { if( _font.BeginSet( ref value ) ) { try { FontChanged?.Invoke( this ); } finally { _font.EndSet(); } } }
		}
		public event Action<UIList> FontChanged;
		ReferenceField<FontComponent> _font = null;

		/// <summary>
		/// The font size of a list element.
		/// </summary>
		[DefaultValue( "Screen 0.02" )]
		public Reference<UIMeasureValueDouble> FontSize
		{
			get { if( _fontSize.BeginGet() ) FontSize = _fontSize.Get( this ); return _fontSize.value; }
			set { if( _fontSize.BeginSet( ref value ) ) { try { FontSizeChanged?.Invoke( this ); } finally { _fontSize.EndSet(); } } }
		}
		public event Action<UIList> FontSizeChanged;
		ReferenceField<UIMeasureValueDouble> _fontSize = new UIMeasureValueDouble( UIMeasure.Screen, 0.02 );

		/// <summary>
		/// Whether the scrollbar will be always visible or not.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> AlwaysShowScroll
		{
			get { if( _alwaysShowScroll.BeginGet() ) AlwaysShowScroll = _alwaysShowScroll.Get( this ); return _alwaysShowScroll.value; }
			set { if( _alwaysShowScroll.BeginSet( ref value ) ) { try { AlwaysShowScrollChanged?.Invoke( this ); } finally { _alwaysShowScroll.EndSet(); } } }
		}
		public event Action<UIList> AlwaysShowScrollChanged;
		ReferenceField<bool> _alwaysShowScroll = false;

		//!!!!
		////UpdateActiveStateOfSelectedItem();
		///// <summary>
		///// Whether to hide item selection when list box is disabled.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> HideSelectionWhenDisabled
		//{
		//	get { if( _hideSelectionWhenDisabled.BeginGet() ) HideSelectionWhenDisabled = _hideSelectionWhenDisabled.Get( this ); return _hideSelectionWhenDisabled.value; }
		//	set { if( _hideSelectionWhenDisabled.BeginSet( ref value ) ) { try { HideSelectionWhenDisabledChanged?.Invoke( this ); } finally { _hideSelectionWhenDisabled.EndSet(); } } }
		//}
		//public event Action<UIList> HideSelectionWhenDisabledChanged;
		//ReferenceField<bool> _hideSelectionWhenDisabled = false;

		//!!!!string? но нужен редактор
		/// <summary>
		/// The list of items.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		public List<string> Items { get; set; } = new List<string>();
		//public List<object> Items { get; } = new List<object>();

		///////////////////////////////////////////////

		/// <summary>
		/// The index of the selected item.
		/// </summary>
		[DefaultValue( 0 )]
		//[Browsable( false )]
		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				if( selectedIndex == value )
					return;
				selectedIndex = value;
				SelectedIndexChanged?.Invoke( this );
			}
		}
		int selectedIndex;//= -1;

		public delegate void SelectedIndexChangedDelegate( UIList sender );
		public event SelectedIndexChangedDelegate SelectedIndexChanged;

		/// <summary>
		/// The selected item.
		/// </summary>
		[Browsable( false )]
		public string SelectedItem
		{
			get
			{
				if( SelectedIndex >= 0 && SelectedIndex < Items.Count )
					return Items[ SelectedIndex ];
				return null;
			}
		}

		/////////////////////////////////////////

		public UIList()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 400, 300 );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			{
				var obj = CreateComponent<UIScroll>();
				obj.Name = "Scroll";
				obj.Size = new UIMeasureValueVector2( UIMeasure.Units, 30, 400 );
				obj.Vertical = true;
				obj.CanBeSelected = false;
				obj.HorizontalAlignment = EHorizontalAlignment.Right;
				obj.VerticalAlignment = EVerticalAlignment.Stretch;
				obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 0, 2, 2, 2 );
			}
		}

		//[Browsable( false )]
		//public UIScrollBar ScrollBar
		//{
		//	get { return GetComponentByPath( "ScrollBar" ) as UIScrollBar; }
		//}
		public UIScroll GetScroll()
		{
			return GetComponentByPath( "Scroll" ) as UIScroll;
		}

		/////////////////////////////////////////////

		//public class ItemMouseEventArgs : EventArgs
		//{
		//	int itemIndex;
		//	object item;

		//	public ItemMouseEventArgs( int itemIndex, object item )
		//	{
		//		this.itemIndex = itemIndex;
		//		this.item = item;
		//	}

		//	public int ItemIndex
		//	{
		//		get { return itemIndex; }
		//	}

		//	public object Item
		//	{
		//		get { return item; }
		//	}

		//}

		//public delegate void ItemMouseEventHandler( object sender, ItemMouseEventArgs e );
		//public event ItemMouseEventHandler ItemMouseDoubleClick;

		///// <summary>
		///// The index of selected item from the item list.
		///// </summary>
		//[DefaultValue( -1 )]
		//[Category( "List Box" )]
		//public int SelectedIndex
		//{
		//	get { return selectedIndex; }
		//	set
		//	{
		//		if( value < -1 || value >= items.Count )
		//			throw new Exception( "EComboBox: SelectedIndex: Set invalid value" );

		//		if( selectedIndex != value )
		//		{
		//			if( selectedIndex != -1 && itemButtons.Count > selectedIndex )
		//				itemButtons[ selectedIndex ].Highlighted = false;

		//			selectedIndex = value;
		//			OnSelectedIndexChange();

		//			if( selectedIndex != -1 && itemButtons.Count > selectedIndex )
		//			{
		//				bool active = true;
		//				if( hideSelectionWhenDisabled && !EnabledInHierarchy )
		//					active = false;
		//				if( active )
		//					itemButtons[ selectedIndex ].Highlighted = true;
		//			}
		//		}

		//		//change scroll bar position
		//		if( scrollBar != null && itemButton != null && selectedIndex != -1 )
		//		{
		//			double itemScreenSizeY = GetItemScreenSizeY();

		//			double screenSizeY = GetScreenSize().Y - GetOffsetByTypeFromLocal( ScaleType.Screen,
		//				GetLocalOffsetByValue( clipRectangleBorders ) ).Y * 2;

		//			double itemsScreenSizeY = itemScreenSizeY * (double)items.Count;
		//			double scrollScreenSizeY = itemsScreenSizeY - screenSizeY;

		//			if( scrollScreenSizeY > 0 )
		//			{
		//				double currentScrollScreenPosY = scrollBar.Value * scrollScreenSizeY;

		//				double itemScrollScreenPosY = itemScreenSizeY * (double)selectedIndex;
		//				Range itemScrollScreenRangeY = new Range( itemScrollScreenPosY,
		//					itemScrollScreenPosY + itemScreenSizeY );

		//				if( itemScrollScreenRangeY.Minimum < currentScrollScreenPosY )
		//				{
		//					currentScrollScreenPosY = itemScrollScreenRangeY.Minimum;
		//				}
		//				else
		//				{
		//					if( itemScrollScreenRangeY.Maximum > currentScrollScreenPosY + screenSizeY )
		//					{
		//						currentScrollScreenPosY = itemScrollScreenRangeY.Maximum
		//							+ itemScreenSizeY - screenSizeY;
		//					}
		//				}

		//				scrollBar.Value = currentScrollScreenPosY / scrollScreenSizeY;
		//			}
		//		}
		//	}
		//}

		//void ItemButton_MouseDoubleClick( object sender, EMouseButtons button )
		//{
		//	if( !EnabledInHierarchy )
		//		return;

		//	UIButton b = (UIButton)sender;
		//	if( b.UserData == null )
		//		return;

		//	int index = (int)b.UserData;
		//	if( index >= items.Count )
		//		return;
		//	OnItemMouseDoubleClick( index );
		//}

		//protected void OnItemMouseDoubleClick( int itemIndex )
		//{
		//	if( ItemMouseDoubleClick != null )
		//		ItemMouseDoubleClick( this, new ItemMouseEventArgs( itemIndex, items[ itemIndex ] ) );
		//}

		/// <summary>
		/// Whether control can be focused.
		/// </summary>
		[Browsable( false )]
		public override bool CanFocus
		{
			get { return EnabledInHierarchy && VisibleInHierarchy && !ReadOnlyInHierarchy; }
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			if( Focused )
			{
				switch( e.Key )
				{
				case EKeys.Up:
					{
						var index = SelectedIndex - 1;
						if( index < 0 )
							index = 0;
						if( index != SelectedIndex )
						{
							SelectedIndex = index;
							EnsureVisible( SelectedIndex );
						}
					}
					return true;

				case EKeys.Down:
					{
						var index = SelectedIndex + 1;
						if( index >= Items.Count )
							index = Items.Count - 1;
						if( index != SelectedIndex )
						{
							SelectedIndex = index;
							EnsureVisible( SelectedIndex );
						}
					}
					return true;

				case EKeys.Home:
					{
						var index = 0;
						if( index != SelectedIndex )
						{
							SelectedIndex = index;
							EnsureVisible( SelectedIndex );
						}
					}
					return true;

				case EKeys.End:
					{
						var index = Items.Count - 1;
						if( index != SelectedIndex )
						{
							SelectedIndex = index;
							EnsureVisible( SelectedIndex );
						}
					}
					return true;

				case EKeys.PageUp:
					{
						var itemSize = ConvertOffsetY( ItemSize, UIMeasure.Screen );
						if( itemSize != 0 )
						{
							var step = (int)( GetScreenSize().Y / itemSize ) - 1;
							if( step <= 0 )
								step = 1;

							var index = SelectedIndex - step;
							if( index < 0 )
								index = 0;
							if( index != SelectedIndex )
							{
								SelectedIndex = index;
								EnsureVisible( SelectedIndex );
							}
						}
					}
					return true;

				case EKeys.PageDown:
					{
						var itemSize = ConvertOffsetY( ItemSize, UIMeasure.Screen );
						if( itemSize != 0 )
						{
							var step = (int)( GetScreenSize().Y / itemSize ) - 1;
							if( step <= 0 )
								step = 1;

							var index = SelectedIndex + step;
							if( index >= Items.Count )
								index = Items.Count - 1;
							if( index != SelectedIndex )
							{
								SelectedIndex = index;
								EnsureVisible( SelectedIndex );
							}
						}
					}
					return true;
				}
			}

			return base.OnKeyDown( e );
		}

		protected override bool OnMouseWheel( int delta )
		{
			var cursorInsideArea = CursorIsInArea();
			if( VisibleInHierarchy && cursorInsideArea /*new Rectangle( 0, 0, 1, 1 ).Contains( MousePosition )*/ && EnabledInHierarchy )
			{
				var scroll = GetScroll();
				if( scroll != null && scroll.EnabledInHierarchy && scroll.VisibleInHierarchy )
				{
					var v = scroll.Value.Value;
					//!!!!
					v -= (double)delta / 700.0f / 10;
					//v -= (double)delta / 700.0f;
					MathEx.Clamp( ref v, scroll.ValueRange.Value.Minimum, scroll.ValueRange.Value.Maximum );
					scroll.Value = v;

					return true;
				}
			}

			return base.OnMouseWheel( delta );
		}

		public double GetFontSizeScreen()
		{
			var value = FontSize.Value;
			var fontSize = GetScreenOffsetByValue( new UIMeasureValueVector2( value.Measure, 0, value.Value ) ).Y;
			return fontSize;
		}

		public void EnsureVisible( int index )
		{
			if( index < 0 || index >= Items.Count )
				return;

			NeedEnsureVisibleInStyle = index;
			//GetStyle().ListEnsureVisible( this, index );
			//if( GetScrollBar() != null )
			//	GetScrollBar().Value = MathEx.Saturate( (float)index / (float)Items.Count );
		}

		bool CursorIsInArea()
		{
			//control rectangle
			if( !( new Rectangle( Vector2.Zero, new Vector2( 1, 1 ) ) ).Contains( MousePosition ) )
				return false;

			if( ParentContainer != null && ParentContainer.IsControlCursorCoveredByOther( this ) )
				return false;

			return true;
		}

		protected override bool OnMouseDown( EMouseButtons button )
		{
			var cursorInsideArea = CursorIsInArea();
			if( button == EMouseButtons.Left && VisibleInHierarchy && cursorInsideArea && EnabledInHierarchy && !ReadOnlyInHierarchy )
			{
				Focus();

				var renderer = ParentContainer?.Viewport.CanvasRenderer;
				if( renderer != null )
				{
					var index = GetListItemIndexByScreenPosition( ParentContainer.MousePosition );
					if( index != -1 )
					{
						SelectedIndex = index;
						//CallItemMouseClick( button );
						return true;
					}

					//return true;
				}
			}

			return base.OnMouseDown( button );
		}

		protected override bool OnMouseUp( EMouseButtons button )
		{
			var cursorInsideArea = CursorIsInArea();
			if( button == EMouseButtons.Left && VisibleInHierarchy && cursorInsideArea && EnabledInHierarchy && !ReadOnlyInHierarchy )
			{
				Focus();

				var renderer = ParentContainer?.Viewport.CanvasRenderer;
				if( renderer != null )
				{
					var index = GetListItemIndexByScreenPosition( ParentContainer.MousePosition );
					if( index != -1 && index == SelectedIndex )
					{
						CallItemMouseClick( button );
						return true;
					}

					//return true;
				}
			}

			return base.OnMouseUp( button );
		}

		protected override bool OnMouseDoubleClick( EMouseButtons button )
		{
			var cursorInsideArea = CursorIsInArea();
			if( button == EMouseButtons.Left && VisibleInHierarchy && cursorInsideArea && EnabledInHierarchy && !ReadOnlyInHierarchy )
			{
				var renderer = ParentContainer?.Viewport.CanvasRenderer;
				if( renderer != null )
				{
					var index = GetListItemIndexByScreenPosition( ParentContainer.MousePosition );
					if( index != -1 && SelectedIndex == index )
					{
						if( CallItemMouseDoubleClick( button ) )//, index ) )
							return true;
					}
				}
			}

			return base.OnMouseDoubleClick( button );
		}

		protected override bool OnTouch( TouchData e )
		{
			switch( e.Action )
			{
			case TouchData.ActionEnum.Down:
				if( VisibleInHierarchy && EnabledInHierarchy && !ReadOnlyInHierarchy && touchDown == null )
				{
					GetScreenRectangle( out var rect );
					var rectInPixels = rect * ParentContainer.Viewport.SizeInPixels.ToVector2();
					var distanceInPixels = rectInPixels.GetPointDistance( e.PositionInPixels.ToVector2() );

					var item = new TouchData.TouchDownRequestToProcessTouch( this, 1, distanceInPixels, null,
						delegate ( UIControl sender, TouchData touchData, object anyData )
						{
							//start touch
							touchDown = e.PointerIdentifier;
							var index = GetListItemIndexByScreenPosition( e.Position );
							if( index != -1 )
								SelectedIndex = index;
						} );
					e.TouchDownRequestToControlActions.Add( item );
				}
				break;

			case TouchData.ActionEnum.Up:
				if( touchDown != null && ReferenceEquals( e.PointerIdentifier, touchDown ) )
					touchDown = null;
				break;

			case TouchData.ActionEnum.Move:
				if( touchDown != null && ReferenceEquals( e.PointerIdentifier, touchDown ) )
				{
					var index = GetListItemIndexByScreenPosition( e.Position );
					if( index != -1 )
						SelectedIndex = index;
				}
				break;

				//case TouchData.ActionEnum.Cancel:
				//	break;

				//case TouchData.ActionEnum.Outside:
				//	break;
			}

			return base.OnTouch( e );
		}

		public int GetListItemIndexByScreenPosition( Vector2 position )
		{
			return GetStyle().GetListItemIndexByScreenPosition( this, position );
		}

		/////////////////////////////////////////

		protected virtual bool OnItemMouseClick( EMouseButtons button ) { return false; }

		public delegate void ItemMouseClickDelegate( UIControl sender, EMouseButtons button, ref bool handled );
		public event ItemMouseClickDelegate ItemMouseClick;

		bool CallItemMouseClick( EMouseButtons button )
		{
			if( OnItemMouseClick( button ) )
				return true;

			bool handled = false;
			ItemMouseClick?.Invoke( this, button, ref handled );
			if( handled )
				return true;

			return false;
		}

		/////////////////////////////////////////

		protected virtual bool OnItemMouseDoubleClick( EMouseButtons button/*, int index*/ ) { return false; }

		public delegate void ItemMouseDoubleClickDelegate( UIControl sender, EMouseButtons button/*, int index*/, ref bool handled );
		public event ItemMouseDoubleClickDelegate ItemMouseDoubleClick;

		bool CallItemMouseDoubleClick( EMouseButtons button )//, int index )
		{
			if( OnItemMouseDoubleClick( button ) )//, index ) )
				return true;

			bool handled = false;
			ItemMouseDoubleClick?.Invoke( this, button/*, index*/, ref handled );
			if( handled )
				return true;

			return false;
		}

		/////////////////////////////////////////

		public bool SelectItem( string value )
		{
			for( int n = 0; n < Items.Count; n++ )
			{
				if( Items[ n ] == value )
				{
					SelectedIndex = n;
					return true;
				}
			}
			return false;
		}

		public override CoverOtherControlsEnum CoverOtherControls
		{
			get { return CoverOtherControlsEnum.OnlyBehind; }
		}
	}
}
