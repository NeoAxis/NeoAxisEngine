// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// A component to manage the position of children controls in a list manner.
	/// </summary>
	public class UIControlList : UIControl
	{
		object touchDown;

		//when reset?
		//Dictionary<UIControl, UIMeasureValueVector2> originalSizes = new Dictionary<UIControl, UIMeasureValueVector2>();

		/////////////////////////////////////////

		//!!!!impl

		[Browsable( false )]
		public int NeedEnsureVisibleInStyle { get; set; } = -1;

		/// <summary>
		/// The horizontal alignment of managed controls.
		/// </summary>
		[DefaultValue( EHorizontalAlignment.Center )]
		[Serialize]
		[Category( "Controls" )]
		public Reference<EHorizontalAlignment> ControlsHorizontalAlignment
		{
			get { if( _controlsHorizontalAlignment.BeginGet() ) ControlsHorizontalAlignment = _controlsHorizontalAlignment.Get( this ); return _controlsHorizontalAlignment.value; }
			set { if( _controlsHorizontalAlignment.BeginSet( this, ref value ) ) { try { ControlsHorizontalAlignChanged?.Invoke( this ); } finally { _controlsHorizontalAlignment.EndSet(); } } }
		}
		public event Action<UIControlList> ControlsHorizontalAlignChanged;
		ReferenceField<EHorizontalAlignment> _controlsHorizontalAlignment = EHorizontalAlignment.Center;

		/// <summary>
		/// The vertical alignment of managed controls.
		/// </summary>
		[DefaultValue( EVerticalAlignment.Center )]
		[Serialize]
		[Category( "Controls" )]
		public Reference<EVerticalAlignment> ControlsVerticalAlignment
		{
			get { if( _controlsVerticalAlignment.BeginGet() ) ControlsVerticalAlignment = _controlsVerticalAlignment.Get( this ); return _controlsVerticalAlignment.value; }
			set { if( _controlsVerticalAlignment.BeginSet( this, ref value ) ) { try { ControlsVerticalAlignChanged?.Invoke( this ); } finally { _controlsVerticalAlignment.EndSet(); } } }
		}
		public event Action<UIControlList> ControlsVerticalAlignChanged;
		ReferenceField<EVerticalAlignment> _controlsVerticalAlignment = EVerticalAlignment.Center;

		/// <summary>
		/// The maximum number of columns for arranging managed controls.
		/// </summary>
		[DefaultValue( 1000 )]
		[Category( "Controls" )]
		public Reference<int> MaxColumns
		{
			get { if( _maxColumns.BeginGet() ) MaxColumns = _maxColumns.Get( this ); return _maxColumns.value; }
			set { if( _maxColumns.BeginSet( this, ref value ) ) { try { MaxColumnsChanged?.Invoke( this ); } finally { _maxColumns.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxColumns"/> property value changes.</summary>
		public event Action<UIControlList> MaxColumnsChanged;
		ReferenceField<int> _maxColumns = 1000;

		[DefaultValue( "Units 10" )]
		[Category( "Controls" )]
		public Reference<UIMeasureValueDouble> HorizontalIndent
		{
			get { if( _horizontalIndent.BeginGet() ) HorizontalIndent = _horizontalIndent.Get( this ); return _horizontalIndent.value; }
			set { if( _horizontalIndent.BeginSet( this, ref value ) ) { try { HorizontalIndentChanged?.Invoke( this ); } finally { _horizontalIndent.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HorizontalIndent"/> property value changes.</summary>
		public event Action<UIControlList> HorizontalIndentChanged;
		ReferenceField<UIMeasureValueDouble> _horizontalIndent = new UIMeasureValueDouble( UIMeasure.Units, 10 );

		[DefaultValue( "Units 10" )]
		[Category( "Controls" )]
		public Reference<UIMeasureValueDouble> VerticalIndent
		{
			get { if( _verticalIndent.BeginGet() ) VerticalIndent = _verticalIndent.Get( this ); return _verticalIndent.value; }
			set { if( _verticalIndent.BeginSet( this, ref value ) ) { try { VerticalIndentChanged?.Invoke( this ); } finally { _verticalIndent.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VerticalIndent"/> property value changes.</summary>
		public event Action<UIControlList> VerticalIndentChanged;
		ReferenceField<UIMeasureValueDouble> _verticalIndent = new UIMeasureValueDouble( UIMeasure.Units, 10 );

		///// <summary>
		///// Whether the scrollbar will be always visible or not.
		///// </summary>
		//[DefaultValue( false )]
		//public Reference<bool> AlwaysShowScroll
		//{
		//	get { if( _alwaysShowScroll.BeginGet() ) AlwaysShowScroll = _alwaysShowScroll.Get( this ); return _alwaysShowScroll.value; }
		//	set { if( _alwaysShowScroll.BeginSet( this, ref value ) ) { try { AlwaysShowScrollChanged?.Invoke( this ); } finally { _alwaysShowScroll.EndSet(); } } }
		//}
		//public event Action<UIControlContainer> AlwaysShowScrollChanged;
		//ReferenceField<bool> _alwaysShowScroll = false;

		/// <summary>
		/// Whether to allow multiple selection of items.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Multiselect
		{
			get { if( _multiselect.BeginGet() ) Multiselect = _multiselect.Get( this ); return _multiselect.value; }
			set { if( _multiselect.BeginSet( this, ref value ) ) { try { MultiselectChanged?.Invoke( this ); } finally { _multiselect.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Multiselect"/> property value changes.</summary>
		public event Action<UIControlList> MultiselectChanged;
		ReferenceField<bool> _multiselect = false;

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
		int selectedIndex;

		public delegate void SelectedIndexChangedDelegate( UIControlList sender );
		public event SelectedIndexChangedDelegate SelectedIndexChanged;

		/// <summary>
		/// The indexes of selected items for Multiselect mode.
		/// </summary>
		[Browsable( false )]
		public int[] SelectedIndices
		{
			get { return selectedIndices; }
			set
			{
				if( selectedIndices.SequenceEqual( value ) )
					return;
				selectedIndices = value;
				SelectedIndicesChanged?.Invoke( this );
			}
		}
		int[] selectedIndices = Array.Empty<int>();

		public delegate void SelectedIndicesChangedDelegate( UIControlList sender );
		public event SelectedIndicesChangedDelegate SelectedIndicesChanged;

		/// <summary>
		/// Get the selected control.
		/// </summary>
		[Browsable( false )]
		public UIControl SelectedItem
		{
			get
			{
				var index = SelectedIndex;
				var items = GetItems();
				if( index >= 0 && index < items.Length )
					return items[ index ];
				return null;
			}
		}

		/////////////////////////////////////////

		public UIControlList()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 400, 300 );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

			{
				var obj = CreateComponent<UIScroll>();
				obj.Name = "Horizontal Scroll";
				obj.Size = new UIMeasureValueVector2( UIMeasure.Units, 400, 30 );
				obj.Vertical = false;
				obj.CanBeSelected = false;
				obj.HorizontalAlignment = EHorizontalAlignment.Stretch;
				obj.VerticalAlignment = EVerticalAlignment.Bottom;
				obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 0, 2, 32, 2 );
			}

			{
				var obj = CreateComponent<UIScroll>();
				obj.Name = "Vertical Scroll";
				obj.Size = new UIMeasureValueVector2( UIMeasure.Units, 30, 400 );
				obj.Vertical = true;
				obj.CanBeSelected = false;
				obj.HorizontalAlignment = EHorizontalAlignment.Right;
				obj.VerticalAlignment = EVerticalAlignment.Stretch;
				obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 0, 2, 2, 2 );
			}
		}

		public UIScroll GetHorizontalScroll()
		{
			return GetComponentByPath( "Horizontal Scroll" ) as UIScroll;
		}

		public UIScroll GetVerticalScroll()
		{
			return GetComponentByPath( "Vertical Scroll" ) as UIScroll;
		}

		/////////////////////////////////////////////

		///// <summary>
		///// Whether control can be focused.
		///// </summary>
		//[Browsable( false )]
		//public override bool CanFocus
		//{
		//	get { return EnabledInHierarchy && VisibleInHierarchy && !ReadOnlyInHierarchy; }
		//}

		void UpdateSelectedIndices( int newIndex )
		{
			if( Multiselect && SelectedIndex != -1 && newIndex != -1 )
			{
				var control = ParentContainer.Viewport.IsKeyPressed( EKeys.Control );
				var shift = ParentContainer.Viewport.IsKeyPressed( EKeys.Shift );

				var list = new List<int>( ( control || shift ) ? selectedIndices : Array.Empty<int>() );

				if( shift )
				{
					var indicesBetween = new RangeI( Math.Min( SelectedIndex, newIndex ), Math.Max( SelectedIndex, newIndex ) );
					for( int n = indicesBetween.Minimum; n <= indicesBetween.Maximum; n++ )
					{
						if( !list.Contains( n ) )
							list.Add( n );
					}
				}
				else
				{
					if( !list.Contains( newIndex ) )
						list.Add( newIndex );
				}

				SelectedIndices = list.ToArray();
			}
		}

		protected override bool OnKeyDown( KeyEvent e )
		{
			//!!!!need?

			//if( Focused )
			//{
			//	switch( e.Key )
			//	{
			//	case EKeys.Up:
			//		{
			//			var index = SelectedIndex - 1;
			//			if( index < 0 )
			//				index = 0;
			//			if( index != SelectedIndex )
			//			{
			//				UpdateSelectedIndices( index );
			//				SelectedIndex = index;
			//				EnsureVisible( SelectedIndex );
			//			}
			//		}
			//		return true;

			//	case EKeys.Down:
			//		{
			//			var items = GetItems();

			//			var index = SelectedIndex + 1;
			//			if( index >= items.Length )
			//				index = items.Length - 1;
			//			if( index != SelectedIndex )
			//			{
			//				UpdateSelectedIndices( index );
			//				SelectedIndex = index;
			//				EnsureVisible( SelectedIndex );
			//			}
			//		}
			//		return true;

			//	case EKeys.Home:
			//		{
			//			var index = 0;
			//			if( index != SelectedIndex )
			//			{
			//				UpdateSelectedIndices( index );
			//				SelectedIndex = index;
			//				EnsureVisible( SelectedIndex );
			//			}
			//		}
			//		return true;

			//	case EKeys.End:
			//		{
			//			var items = GetItems();

			//			var index = items.Length - 1;
			//			if( index != SelectedIndex )
			//			{
			//				UpdateSelectedIndices( index );
			//				SelectedIndex = index;
			//				EnsureVisible( SelectedIndex );
			//			}
			//		}
			//		return true;

			//		//!!!!

			//		//case EKeys.PageUp:
			//		//	{
			//		//		var itemSize = ConvertOffsetY( ItemSize, UIMeasure.Screen );
			//		//		if( itemSize != 0 )
			//		//		{
			//		//			var step = (int)( GetScreenSize().Y / itemSize ) - 1;
			//		//			if( step <= 0 )
			//		//				step = 1;

			//		//			var index = SelectedIndex - step;
			//		//			if( index < 0 )
			//		//				index = 0;
			//		//			if( index != SelectedIndex )
			//		//			{
			//		//				UpdateSelectedIndices( index );
			//		//				SelectedIndex = index;
			//		//				EnsureVisible( SelectedIndex );
			//		//			}
			//		//		}
			//		//	}
			//		//	return true;

			//		//case EKeys.PageDown:
			//		//	{
			//		//		var itemSize = ConvertOffsetY( ItemSize, UIMeasure.Screen );
			//		//		if( itemSize != 0 )
			//		//		{
			//		//			var step = (int)( GetScreenSize().Y / itemSize ) - 1;
			//		//			if( step <= 0 )
			//		//				step = 1;

			//		//			var index = SelectedIndex + step;
			//		//			if( index >= Items.Count )
			//		//				index = Items.Count - 1;
			//		//			if( index != SelectedIndex )
			//		//			{
			//		//				UpdateSelectedIndices( index );
			//		//				SelectedIndex = index;
			//		//				EnsureVisible( SelectedIndex );
			//		//			}
			//		//		}
			//		//	}
			//		//	return true;

			//	}
			//}

			return base.OnKeyDown( e );
		}

		protected override bool OnMouseWheel( int delta )
		{
			var cursorInsideArea = CursorIsInArea( false );
			if( VisibleInHierarchy && cursorInsideArea && EnabledInHierarchy )
			{
				var scroll = GetVerticalScroll();
				if( scroll != null && scroll.EnabledInHierarchy && scroll.VisibleInHierarchy )
				{
					var v = scroll.Value.Value;
					v -= (double)delta / 700.0f / 10; //v -= (double)delta / 700.0f;
					MathEx.Clamp( ref v, scroll.ValueRange.Value.Minimum, scroll.ValueRange.Value.Maximum );
					scroll.Value = v;

					return true;
				}
			}

			return base.OnMouseWheel( delta );
		}

		public void EnsureVisible( int index )
		{
			var items = GetItems();
			if( index < 0 || index >= items.Length )
				return;
			NeedEnsureVisibleInStyle = index;
		}

		bool CursorIsInArea( bool checkCoveredByOther = true )
		{
			//control rectangle
			if( !( new Rectangle( Vector2.Zero, new Vector2( 1, 1 ) ) ).Contains( MousePosition ) )
				return false;

			if( checkCoveredByOther )
				if( ParentContainer != null && ParentContainer.IsControlCursorCoveredByOther( this ) && !TopMost )
					return false;

			return true;
		}

		protected override bool OnMouseDown( EMouseButtons button )
		{
			var cursorInsideArea = CursorIsInArea();
			if( VisibleInHierarchy && cursorInsideArea && EnabledInHierarchy && !ReadOnlyInHierarchy )
			{
				Focus();

				var renderer = ParentContainer?.Viewport.CanvasRenderer;
				if( renderer != null )
				{
					if( button == EMouseButtons.Left )
					{
						var index = GetItemIndexByScreenPosition( ParentContainer.MousePosition );
						if( index != -1 )
						{
							UpdateSelectedIndices( index );
							SelectedIndex = index;

							//return true;
						}
					}
				}
			}

			return base.OnMouseDown( button );
		}

		protected override bool OnMouseUp( EMouseButtons button )
		{
			var cursorInsideArea = CursorIsInArea();
			if( VisibleInHierarchy && cursorInsideArea && EnabledInHierarchy && !ReadOnlyInHierarchy )
			{
				Focus();

				var renderer = ParentContainer?.Viewport.CanvasRenderer;
				if( renderer != null )
				{
					var index = GetItemIndexByScreenPosition( ParentContainer.MousePosition );
					if( index != -1 && GetAllSelectedIndices().Contains( index ) )
					{
						if( CallItemMouseClick( button ) )
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
			if( VisibleInHierarchy && cursorInsideArea && EnabledInHierarchy && !ReadOnlyInHierarchy )
			{
				var renderer = ParentContainer?.Viewport.CanvasRenderer;
				if( renderer != null )
				{
					var index = GetItemIndexByScreenPosition( ParentContainer.MousePosition );
					if( index != -1 && SelectedIndex == index )
					{
						if( CallItemMouseDoubleClick( button ) )
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
					if( ParentContainer != null && ParentContainer.IsControlCursorCoveredByOther( this ) )
						break;

					GetScreenRectangle( out var rect );
					var rectInPixels = rect * ParentContainer.Viewport.SizeInPixels.ToVector2();
					var distanceInPixels = rectInPixels.GetPointDistance( e.PositionInPixels.ToVector2() );

					var item = new TouchData.TouchDownRequestToProcessTouch( this, 0/*1*/, distanceInPixels, null,
						delegate ( UIControl sender, TouchData touchData, object anyData )
						{
							Focus();

							//start touch
							touchDown = e.PointerIdentifier;
							var index = GetItemIndexByScreenPosition( e.Position );
							if( index != -1 )
							{
								UpdateSelectedIndices( index );
								SelectedIndex = index;
							}
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
					var index = GetItemIndexByScreenPosition( e.Position );
					if( index != -1 )
					{
						UpdateSelectedIndices( index );
						SelectedIndex = index;
					}
				}
				break;

				//case TouchData.ActionEnum.Cancel:
				//	break;

				//case TouchData.ActionEnum.Outside:
				//	break;
			}

			return base.OnTouch( e );
		}

		public int GetItemIndexByScreenPosition( Vector2 position )
		{
			var items = GetItems();
			for( int n = 0; n < items.Length; n++ )
			{
				var control = items[ n ];
				control.GetScreenRectangle( out var rect );
				if( rect.Contains( position ) )
					return n;
			}
			return -1;
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

		protected virtual bool OnItemMouseDoubleClick( EMouseButtons button ) { return false; }

		public delegate void ItemMouseDoubleClickDelegate( UIControl sender, EMouseButtons button, ref bool handled );
		public event ItemMouseDoubleClickDelegate ItemMouseDoubleClick;

		bool CallItemMouseDoubleClick( EMouseButtons button )
		{
			if( OnItemMouseDoubleClick( button ) )
				return true;

			bool handled = false;
			ItemMouseDoubleClick?.Invoke( this, button, ref handled );
			if( handled )
				return true;

			return false;
		}

		/////////////////////////////////////////

		public int SelectItem( UIControl control )
		{
			var controls = GetComponents<UIControl>();
			for( int n = 0; n < controls.Length; n++ )
			{
				if( ReferenceEquals( controls[ n ], control ) )
				{
					SelectedIndex = n;
					return n;
				}
			}
			return -1;
		}

		public override CoverOtherControlsEnum CoverOtherControls
		{
			get { return CoverOtherControlsEnum.OnlyBehind; }
		}

		public UIControl[] GetItems()
		{
			//!!!!slowly?

			return GetComponents<UIControl>().Where( c => c is not UIScroll && c is not UIContextMenu && c.Enabled ).ToArray();
		}

		public UIControl GetItem( int index )
		{
			var items = GetItems();
			if( index >= 0 && index < items.Length )
				return items[ index ];
			return null;
		}

		public void RemoveAllItems( bool queued = false )
		{
			var items = GetItems();
			foreach( var item in items )
				item.RemoveFromParent( queued );
		}

		/// <summary>
		/// Get the indexes of selected items in Multiselect mode combined with current selected index.
		/// </summary>
		/// <returns></returns>
		public int[] GetAllSelectedIndices()
		{
			if( SelectedIndices.Length > 0 )
			{
				if( SelectedIndex != -1 )
				{
					var list = new List<int>( SelectedIndices );
					if( !list.Contains( SelectedIndex ) )
						list.Add( SelectedIndex );
					return list.ToArray();
				}
				else
					return SelectedIndices;
			}
			else if( SelectedIndex != -1 )
				return new int[] { SelectedIndex };
			else
				return Array.Empty<int>();
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
				UpdateItemsPosition();
		}

		public void UpdateItemsPosition()
		{
			OnUpdateItemsPosition();
		}

		public Rectangle GetItemsClipRectangle()
		{
			var clipRectangle = GetScreenRectangle();

			var horizontalScroll = GetHorizontalScroll();
			if( horizontalScroll != null && horizontalScroll.Enabled )
			{
				var scrollRectangle = horizontalScroll.GetScreenRectangle();
				if( horizontalScroll.HorizontalAlignment.Value == EHorizontalAlignment.Right )
					clipRectangle.Bottom = Math.Min( clipRectangle.Bottom, scrollRectangle.Top );
			}
			var verticalScroll = GetVerticalScroll();
			if( verticalScroll != null && verticalScroll.Enabled )
			{
				var scrollRectangle = verticalScroll.GetScreenRectangle();
				if( horizontalScroll != null && horizontalScroll.VerticalAlignment.Value == EVerticalAlignment.Bottom )
					clipRectangle.Right = Math.Min( clipRectangle.Right, scrollRectangle.Left );
			}

			return clipRectangle;
		}

		protected virtual void OnUpdateItemsPosition()
		{
			var items = GetItems();
			var horizontalScroll = GetHorizontalScroll();
			var verticalScroll = GetVerticalScroll();

			//foreach( var item in items )
			//{
			//	if( !originalSizes.ContainsKey( item ) )
			//		originalSizes[ item ] = item.Size.Value;
			//}

			var itemScreenWidth = 0.0;
			if( items.Length != 0 )
				itemScreenWidth = items[ 0 ].GetScreenSize().X;
			if( itemScreenWidth < 0.001 )
				itemScreenWidth = 0.001;

			GetScreenRectangle( out var screenRectangle );
			var horizontalIndent = ConvertOffsetX( HorizontalIndent.Value, UIMeasure.Screen );
			var verticalIndent = ConvertOffsetY( VerticalIndent.Value, UIMeasure.Screen );
			var viewHeight = screenRectangle.Size.Y;

			//calculate total height
			var totalHeight = 0.0;
			for( int n = 0; n < items.Length; n++ )
			{
				var item = items[ n ];
				var itemScreenSize = item.GetScreenSize();
				totalHeight += itemScreenSize.Y + verticalIndent;
			}
			if( totalHeight <= 0.001 )
				totalHeight = 0.001;


			bool IsNeedVerticalScroll( bool applyScrollWidth, out int columns )
			{
				var width = screenRectangle.Size.X;
				if( applyScrollWidth && verticalScroll != null )
					width -= verticalScroll.GetScreenSize().X;
				if( width < 0 )
					width = 0;

				columns = (int)Math.Floor( width / ( itemScreenWidth + horizontalIndent ) );
				columns = MathEx.Clamp( columns, 1, MaxColumns.Value );

				//determine need scroll or not
				var needScroll = false;
				{
					var currentColumn = 0;
					var currentPositionY = 0.0;

					for( int n = 0; n < items.Length; n++ )
					{
						var item = items[ n ];
						var itemScreenSize = item.GetScreenSize();

						if( currentPositionY + itemScreenSize.Y + verticalIndent > viewHeight )
						{
							currentColumn++;
							currentPositionY = 0;
						}

						currentPositionY += itemScreenSize.Y + verticalIndent;
					}

					if( currentColumn >= columns )
						needScroll = true;
				}

				return needScroll;
			}

			var needVerticalScroll = IsNeedVerticalScroll( false, out var columns );
			if( needVerticalScroll )
				IsNeedVerticalScroll( true, out columns );

			var horizontalStretch = ControlsHorizontalAlignment.Value == EHorizontalAlignment.Stretch;

			//horizontal scroll

			var maxItemWidth = 0.0;
			if( columns == 1 )
			{
				for( int n = 0; n < items.Length; n++ )
				{
					var item = items[ n ];
					var itemScreenSize = item.GetScreenSize();
					if( itemScreenSize.X > maxItemWidth )
						maxItemWidth = itemScreenSize.X;
				}
			}

			//determine need horizontal scroll or not
			var needHorizontalScroll = maxItemWidth > screenRectangle.Size.X;
			if( horizontalScroll != null )
			{
				if( needHorizontalScroll )
				{
					horizontalScroll.ValueRange = new Range( 0, maxItemWidth - screenRectangle.Size.X );
					horizontalScroll.Enabled = true;
				}
				else
				{
					horizontalScroll.ValueRange = new Range( 0, 0 );
					horizontalScroll.Enabled = false;
				}
			}

			//calculate horizontal scroll offset
			var screenOffsetX = 0.0;
			if( horizontalScroll != null && horizontalScroll.Enabled )
				screenOffsetX -= horizontalScroll.Value.Value;

			if( !needHorizontalScroll )
			{
				switch( ControlsHorizontalAlignment.Value )
				{
				case EHorizontalAlignment.Center:
					screenOffsetX += ( screenRectangle.Size.X - ( itemScreenWidth + horizontalIndent ) * columns ) / 2;
					break;

				case EHorizontalAlignment.Right:
					screenOffsetX += screenRectangle.Size.X - ( itemScreenWidth + horizontalIndent ) * columns;
					break;

				case EHorizontalAlignment.Stretch:
					itemScreenWidth = ( screenRectangle.Size.X - horizontalIndent * ( columns - 1 ) ) / columns;
					break;
				}
			}

			//vertical scroll

			if( !needVerticalScroll )
			{
				//without vertical scroll

				//disable scroll
				if( verticalScroll != null )
				{
					verticalScroll.ValueRange = new Range( 0, 0 );
					verticalScroll.Enabled = false;
				}

				//update items positions
				{
					var maxHeight = 0.0;
					{
						var currentColumn = 0;
						var currentPositionY = 0.0;

						for( int n = 0; n < items.Length; n++ )
						{
							var item = items[ n ];
							var itemScreenSize = item.GetScreenSize();

							if( currentPositionY + itemScreenSize.Y + verticalIndent > viewHeight )
							{
								currentColumn++;
								currentPositionY = 0;
							}

							currentPositionY += itemScreenSize.Y + verticalIndent;

							if( currentPositionY > maxHeight )
								maxHeight = currentPositionY;
						}
					}

					{
						var screenOffsetY = 0.0;
						{
							switch( ControlsVerticalAlignment.Value )
							{
							case EVerticalAlignment.Center:
								screenOffsetY += ( viewHeight - maxHeight ) / 2;
								break;

							case EVerticalAlignment.Bottom:
								screenOffsetY += viewHeight - maxHeight;
								break;
							}
						}

						var currentColumn = 0;
						var currentPositionY = 0.0;

						for( int n = 0; n < items.Length; n++ )
						{
							var item = items[ n ];
							var itemScreenSize = item.GetScreenSize();

							if( currentPositionY + itemScreenSize.Y + verticalIndent > viewHeight )
							{
								currentColumn++;
								currentPositionY = 0;
							}

							var offsetX = screenRectangle.Left + screenOffsetX + ( itemScreenWidth + horizontalIndent ) * currentColumn;
							var offsetY = screenRectangle.Top + screenOffsetY + currentPositionY;
							item.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, offsetX, offsetY, offsetX + itemScreenSize.X, offsetY + itemScreenSize.Y );

							//if( !needHorizontalScroll && ControlsHorizontalAlignment.Value == EHorizontalAlignment.Stretch )
							//{
							//	originalSizes.TryGetValue( item, out var originalSize );

							//	var sizeY = ConvertOffsetY( new UIMeasureValueDouble( originalSize.Measure, originalSize.Y ), UIMeasure.Screen );
							//	item.Size = new UIMeasureValueVector2( UIMeasure.Screen, itemScreenWidth, sizeY );

							//	//var sizeX = ConvertOffsetX( new UIMeasureValueDouble( UIMeasure.Screen, itemScreenWidth ), item.Size.Value.Measure );
							//	//item.Size = new UIMeasureValueVector2( item.Size.Value.Measure, sizeX, item.Size.Value.Y );

							//	//var sizeY = ConvertOffsetY( new UIMeasureValueDouble( item.Size.Value.Measure, item.Size.Value.Y ), UIMeasure.Screen );
							//	//item.Size = new UIMeasureValueVector2( UIMeasure.Screen, itemScreenWidth, sizeY );
							//}

							currentPositionY += itemScreenSize.Y + verticalIndent;
						}
					}
				}
			}
			else
			{
				//with vertical scroll

				//calculate items positions and scroll height
				var itemsPositions = new Dictionary<UIControl, (int, double)>();
				var maxHeight = 0.0;
				{
					var currentColumn = 0;
					var currentPositionY = 0.0;
					var totalCurrentHeight = 0.0;

					for( int n = 0; n < items.Length; n++ )
					{
						var item = items[ n ];
						var itemScreenSize = item.GetScreenSize();

						var addY = itemScreenSize.Y;
						if( n != items.Length - 1 )
							addY += verticalIndent;

						var column = (int)( ( totalCurrentHeight + addY ) / totalHeight * columns );
						if( column >= columns )
							column = columns - 1;
						if( currentColumn != column )
						{
							currentColumn = column;
							currentPositionY = 0;
						}

						itemsPositions[ item ] = (currentColumn, currentPositionY);

						currentPositionY += addY;
						totalCurrentHeight += addY;

						maxHeight = Math.Max( maxHeight, currentPositionY );
					}
				}

				//update scroll
				if( maxHeight > viewHeight )
				{
					verticalScroll.ValueRange = new Range( 0, maxHeight - viewHeight );
					verticalScroll.Enabled = true;
				}
				else
				{
					verticalScroll.ValueRange = new Range( 0, 0 );
					verticalScroll.Enabled = false;
				}

				//update items positions
				{
					var screenPositionY = screenRectangle.Top;
					if( verticalScroll != null && verticalScroll.Enabled )
						screenPositionY -= verticalScroll.Value.Value;

					for( int n = 0; n < items.Length; n++ )
					{
						var item = items[ n ];
						var itemScreenSize = item.GetScreenSize();

						itemsPositions.TryGetValue( item, out var position );
						var currentColumn = position.Item1;
						var currentPositionY = position.Item2;

						var offsetX = screenRectangle.Left + screenOffsetX + ( itemScreenWidth + horizontalIndent ) * currentColumn;
						var offsetY = screenPositionY + currentPositionY;
						item.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, offsetX, offsetY, offsetX + itemScreenSize.X, offsetY + itemScreenSize.Y );

						//if( !needHorizontalScroll && ControlsHorizontalAlignment.Value == EHorizontalAlignment.Stretch )
						//{
						//	originalSizes.TryGetValue( item, out var originalSize );

						//	var sizeY = ConvertOffsetY( new UIMeasureValueDouble( originalSize.Measure, originalSize.Y ), UIMeasure.Screen );
						//	item.Size = new UIMeasureValueVector2( UIMeasure.Screen, itemScreenWidth, sizeY );

						//	//var sizeY = ConvertOffsetY( new UIMeasureValueDouble( item.Size.Value.Measure, item.Size.Value.Y ), UIMeasure.Screen );
						//	//item.Size = new UIMeasureValueVector2( UIMeasure.Screen, itemScreenWidth, sizeY );
						//}
					}
				}
			}

			var clipRectangleForItems = GetItemsClipRectangle();
			foreach( var item in items )
				item.ScreenClipRectangle = clipRectangleForItems;
		}
	}
}
