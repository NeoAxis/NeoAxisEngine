// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// The style of the UI controls.
	/// </summary>
	[ResourceFileExtension( "uistyle" )]
	public class UIStyle : Component
	{
		/// <summary>
		/// Gets the default style.
		/// </summary>
		public static UIStyleDefault Default
		{
			get { return UIStyleDefault.Instance; }
		}

		/// <summary>
		/// Gets the empty style.
		/// </summary>
		public static UIStyleEmpty Empty
		{
			get { return UIStyleEmpty.Instance; }
		}

		//static UIStyleDefault _default;
		//public static UIStyleDefault Default
		//{
		//	get
		//	{
		//		if( _default == null )
		//		{
		//			_default = new UIStyleDefault();
		//			_default.Name = "Default";
		//		}
		//		return _default;
		//	}
		//}

		/////////////////////////////////////////

		protected virtual void OnBeforeRenderUIWithChildren( Component component, CanvasRenderer renderer ) { }

		public delegate void BeforeRenderUIWithChildrenDelegate( Component component, CanvasRenderer renderer );
		public event BeforeRenderUIWithChildrenDelegate BeforeRenderUIWithChildren;

		public void PerformBeforeRenderUIWithChildren( Component component, CanvasRenderer renderer )
		{
			OnBeforeRenderUIWithChildren( component, renderer );
			BeforeRenderUIWithChildren?.Invoke( component, renderer );
		}

		/////////////////////////////////////////

		protected virtual void OnAfterRenderUIWithChildren( Component component, CanvasRenderer renderer ) { }

		public delegate void AfterRenderUIWithChildrenDelegate( Component component, CanvasRenderer renderer );
		public event AfterRenderUIWithChildrenDelegate AfterRenderUIWithChildren;

		public void PerformAfterRenderUIWithChildren( Component component, CanvasRenderer renderer )
		{
			OnAfterRenderUIWithChildren( component, renderer );
			AfterRenderUIWithChildren?.Invoke( component, renderer );
		}

		/////////////////////////////////////////

		protected virtual void OnRenderComponent( Component component, CanvasRenderer renderer ) { }

		public delegate void RenderComponentDelegate( Component component, CanvasRenderer renderer );
		public event RenderComponentDelegate RenderComponent;

		public void PerformRenderComponent( Component component, CanvasRenderer renderer )
		{
			OnRenderComponent( component, renderer );
			RenderComponent?.Invoke( component, renderer );
		}

		/////////////////////////////////////////

		protected virtual void OnButtonMouseEnter( UIButton control )
		{
			if( control.Parent as UIContextMenu != null )
				control.ParentContainer?.PlaySound( ContextMenuButtonSoundMouseEnter );
			else
				control.ParentContainer?.PlaySound( ButtonSoundMouseEnter );
		}

		public delegate void ButtonMouseEnterDelegate( UIButton control );
		public event ButtonMouseEnterDelegate ButtonMouseEnter;

		public void PerformButtonMouseEnter( UIButton control )
		{
			OnButtonMouseEnter( control );
			ButtonMouseEnter?.Invoke( control );
		}

		////////////////

		protected virtual void OnButtonMouseLeave( UIButton control )
		{
			if( control.Parent as UIContextMenu != null )
				control.ParentContainer?.PlaySound( ContextMenuButtonSoundMouseLeave );
			else
				control.ParentContainer?.PlaySound( ButtonSoundMouseLeave );
		}

		public delegate void ButtonMouseLeaveDelegate( UIButton control );
		public event ButtonMouseLeaveDelegate ButtonMouseLeave;

		public void PerformButtonMouseLeave( UIButton control )
		{
			OnButtonMouseLeave( control );
			ButtonMouseLeave?.Invoke( control );
		}

		////////////////

		protected virtual void OnButtonClick( UIButton control )
		{
			if( control.Parent as UIContextMenu != null )
				control.ParentContainer?.PlaySound( ContextMenuButtonSoundClick );
			else
				control.ParentContainer?.PlaySound( ButtonSoundClick );
		}

		public delegate void ButtonClickDelegate( UIButton control );
		public event ButtonClickDelegate ButtonClick;

		public void PerformButtonClick( UIButton control )
		{
			OnButtonClick( control );
			ButtonClick?.Invoke( control );
		}

		////////////////

		/// <summary>
		/// The sound played when the mouse pointer enters the button.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> ButtonSoundMouseEnter
		{
			get { if( _buttonSoundMouseEnter.BeginGet() ) ButtonSoundMouseEnter = _buttonSoundMouseEnter.Get( this ); return _buttonSoundMouseEnter.value; }
			set { if( _buttonSoundMouseEnter.BeginSet( ref value ) ) { try { ButtonSoundMouseEnterChanged?.Invoke( this ); } finally { _buttonSoundMouseEnter.EndSet(); } } }
		}
		public event Action<UIStyle> ButtonSoundMouseEnterChanged;
		ReferenceField<Sound> _buttonSoundMouseEnter;

		/// <summary>
		/// The sound played when the mouse pointer leaves the button.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> ButtonSoundMouseLeave
		{
			get { if( _buttonSoundMouseLeave.BeginGet() ) ButtonSoundMouseLeave = _buttonSoundMouseLeave.Get( this ); return _buttonSoundMouseLeave.value; }
			set { if( _buttonSoundMouseLeave.BeginSet( ref value ) ) { try { ButtonSoundMouseLeaveChanged?.Invoke( this ); } finally { _buttonSoundMouseLeave.EndSet(); } } }
		}
		public event Action<UIStyle> ButtonSoundMouseLeaveChanged;
		ReferenceField<Sound> _buttonSoundMouseLeave;

		/// <summary>
		/// The sound played when the user clicks on the button.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> ButtonSoundClick
		{
			get { if( _buttonSoundClick.BeginGet() ) ButtonSoundClick = _buttonSoundClick.Get( this ); return _buttonSoundClick.value; }
			set { if( _buttonSoundClick.BeginSet( ref value ) ) { try { ButtonSoundClickChanged?.Invoke( this ); } finally { _buttonSoundClick.EndSet(); } } }
		}
		public event Action<UIStyle> ButtonSoundClickChanged;
		ReferenceField<Sound> _buttonSoundClick;

		/////////////////////////////////////////

		/// <summary>
		/// The sound played when the mouse pointer enters the button of context menu.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> ContextMenuButtonSoundMouseEnter
		{
			get { if( _contextMenuButtonSoundMouseEnter.BeginGet() ) ContextMenuButtonSoundMouseEnter = _contextMenuButtonSoundMouseEnter.Get( this ); return _contextMenuButtonSoundMouseEnter.value; }
			set { if( _contextMenuButtonSoundMouseEnter.BeginSet( ref value ) ) { try { ContextMenuButtonSoundMouseEnterChanged?.Invoke( this ); } finally { _contextMenuButtonSoundMouseEnter.EndSet(); } } }
		}
		public event Action<UIStyle> ContextMenuButtonSoundMouseEnterChanged;
		ReferenceField<Sound> _contextMenuButtonSoundMouseEnter;

		/// <summary>
		/// The sound played when the mouse pointer leaves the button of context menu.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> ContextMenuButtonSoundMouseLeave
		{
			get { if( _contextMenuButtonSoundMouseLeave.BeginGet() ) ContextMenuButtonSoundMouseLeave = _contextMenuButtonSoundMouseLeave.Get( this ); return _contextMenuButtonSoundMouseLeave.value; }
			set { if( _contextMenuButtonSoundMouseLeave.BeginSet( ref value ) ) { try { ContextMenuButtonSoundMouseLeaveChanged?.Invoke( this ); } finally { _contextMenuButtonSoundMouseLeave.EndSet(); } } }
		}
		public event Action<UIStyle> ContextMenuButtonSoundMouseLeaveChanged;
		ReferenceField<Sound> _contextMenuButtonSoundMouseLeave;

		/// <summary>
		/// The sound played when the user clicks on the button of context menu.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> ContextMenuButtonSoundClick
		{
			get { if( _contextMenuButtonSoundClick.BeginGet() ) ContextMenuButtonSoundClick = _contextMenuButtonSoundClick.Get( this ); return _contextMenuButtonSoundClick.value; }
			set { if( _contextMenuButtonSoundClick.BeginSet( ref value ) ) { try { ContextMenuButtonSoundClickChanged?.Invoke( this ); } finally { _contextMenuButtonSoundClick.EndSet(); } } }
		}
		public event Action<UIStyle> ContextMenuButtonSoundClickChanged;
		ReferenceField<Sound> _contextMenuButtonSoundClick;

		/////////////////////////////////////////

		protected virtual void OnCheckMouseEnter( UICheck control )
		{
			control.ParentContainer?.PlaySound( CheckSoundMouseEnter );
		}

		public delegate void CheckMouseEnterDelegate( UICheck control );
		public event CheckMouseEnterDelegate CheckMouseEnter;

		public void PerformCheckMouseEnter( UICheck control )
		{
			OnCheckMouseEnter( control );
			CheckMouseEnter?.Invoke( control );
		}

		////////////////

		protected virtual void OnCheckMouseLeave( UICheck control )
		{
			control.ParentContainer?.PlaySound( CheckSoundMouseLeave );
		}

		public delegate void CheckMouseLeaveDelegate( UICheck control );
		public event CheckMouseLeaveDelegate CheckMouseLeave;

		public void PerformCheckMouseLeave( UICheck control )
		{
			OnCheckMouseLeave( control );
			CheckMouseLeave?.Invoke( control );
		}

		////////////////

		protected virtual void OnCheckClick( UICheck control )
		{
			control.ParentContainer?.PlaySound( CheckSoundClick );
		}

		public delegate void CheckClickDelegate( UICheck control );
		public event CheckClickDelegate CheckClick;

		public void PerformCheckClick( UICheck control )
		{
			OnCheckClick( control );
			CheckClick?.Invoke( control );
		}

		////////////////

		/// <summary>
		/// The sound played when the mouse pointer enters the check.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> CheckSoundMouseEnter
		{
			get { if( _checkSoundMouseEnter.BeginGet() ) CheckSoundMouseEnter = _checkSoundMouseEnter.Get( this ); return _checkSoundMouseEnter.value; }
			set { if( _checkSoundMouseEnter.BeginSet( ref value ) ) { try { CheckSoundMouseEnterChanged?.Invoke( this ); } finally { _checkSoundMouseEnter.EndSet(); } } }
		}
		public event Action<UIStyle> CheckSoundMouseEnterChanged;
		ReferenceField<Sound> _checkSoundMouseEnter;

		/// <summary>
		/// The sound played when the mouse pointer leaves the check.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> CheckSoundMouseLeave
		{
			get { if( _checkSoundMouseLeave.BeginGet() ) CheckSoundMouseLeave = _checkSoundMouseLeave.Get( this ); return _checkSoundMouseLeave.value; }
			set { if( _checkSoundMouseLeave.BeginSet( ref value ) ) { try { CheckSoundMouseLeaveChanged?.Invoke( this ); } finally { _checkSoundMouseLeave.EndSet(); } } }
		}
		public event Action<UIStyle> CheckSoundMouseLeaveChanged;
		ReferenceField<Sound> _checkSoundMouseLeave;

		/// <summary>
		/// The sound played when user clicks on the check.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> CheckSoundClick
		{
			get { if( _checkSoundClick.BeginGet() ) CheckSoundClick = _checkSoundClick.Get( this ); return _checkSoundClick.value; }
			set { if( _checkSoundClick.BeginSet( ref value ) ) { try { CheckSoundClickChanged?.Invoke( this ); } finally { _checkSoundClick.EndSet(); } } }
		}
		public event Action<UIStyle> CheckSoundClickChanged;
		ReferenceField<Sound> _checkSoundClick;

		////////////////

		public virtual int GetListItemIndexByScreenPosition( UIList control, Vector2 position ) { return -1; }
		public virtual void ListEnsureVisible( UIList control, int index ) { }

		////////////////

		public virtual Ray2 GetSliderValuesRayInScreenCoords( UISlider control ) { return new Ray2(); }

		//[DefaultValue( "Units 2 2 2 2" )]
		//public Reference<UIMeasureValueRectangle> EditTextMargin
		//{
		//	get { if( _editTextMargin.BeginGet() ) EditTextMargin = _editTextMargin.Get( this ); return _editTextMargin.value; }
		//	set { if( _editTextMargin.BeginSet( ref value ) ) { try { EditTextMarginChanged?.Invoke( this ); } finally { _editTextMargin.EndSet(); } } }
		//}
		//public event Action<UIStyle> EditTextMarginChanged;
		//ReferenceField<UIMeasureValueRectangle> _editTextMargin = new UIMeasureValueRectangle( UIMeasure.Units, 2, 2, 2, 2 );
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Represents an default UI style.
	/// </summary>
	public class UIStyleDefault : UIStyle
	{
		//int listEnsureVisible = -1;

		/////////////////////////////////////////

		static UIStyleDefault instance;
		public static UIStyleDefault Instance
		{
			get
			{
				if( instance == null )
				{
					instance = new UIStyleDefault();
					instance.Name = "Default";
				}
				return instance;
			}
		}

		protected override void OnBeforeRenderUIWithChildren( Component component, CanvasRenderer renderer )
		{
			base.OnBeforeRenderUIWithChildren( component, renderer );

			var control = component as UIControl;
			if( control != null )
			{
				if( control is UIButton && control.ReadOnlyInHierarchy )
					renderer.PushColorMultiplier( new ColorValue( 0.5, 0.5, 0.5 ) );

				if( control is UIEdit && control.ReadOnlyInHierarchy )
				{
					var parentCombo = control.Parent as UICombo;
					var insideCombo = parentCombo != null && parentCombo.GetTextControl() == control;
					if( !insideCombo )
						renderer.PushColorMultiplier( new ColorValue( 0.5, 0.5, 0.5 ) );
				}

				if( control is UICombo && control.ReadOnlyInHierarchy )
					renderer.PushColorMultiplier( new ColorValue( 0.5, 0.5, 0.5 ) );

				if( control is UIScroll && control.ReadOnlyInHierarchy )
					renderer.PushColorMultiplier( new ColorValue( 0.5, 0.5, 0.5 ) );

				if( control is UISlider && control.ReadOnlyInHierarchy )
					renderer.PushColorMultiplier( new ColorValue( 0.5, 0.5, 0.5 ) );

				if( control is UIList && control.ReadOnlyInHierarchy )
					renderer.PushColorMultiplier( new ColorValue( 0.5, 0.5, 0.5 ) );

				if( control is UIText && control.ReadOnlyInHierarchy )
				{
					var parentEdit = control.Parent as UIEdit;
					var insideEdit = parentEdit != null && parentEdit.GetTextControl() == control;
					if( !insideEdit )
						renderer.PushColorMultiplier( new ColorValue( 0.5, 0.5, 0.5 ) );
				}

				if( control is UICheck && control.ReadOnlyInHierarchy )
					renderer.PushColorMultiplier( new ColorValue( 0.5, 0.5, 0.5 ) );
			}
		}

		protected override void OnAfterRenderUIWithChildren( Component component, CanvasRenderer renderer )
		{
			var control = component as UIControl;
			if( control != null )
			{
				if( control is UIButton && control.ReadOnlyInHierarchy )
					renderer.PopColorMultiplier();

				if( control is UIEdit && control.ReadOnlyInHierarchy )
				{
					var parentCombo = control.Parent as UICombo;
					var insideCombo = parentCombo != null && parentCombo.GetTextControl() == control;
					if( !insideCombo )
						renderer.PopColorMultiplier();
				}

				if( control is UICombo && control.ReadOnlyInHierarchy )
					renderer.PopColorMultiplier();

				if( control is UIScroll && control.ReadOnlyInHierarchy )
					renderer.PopColorMultiplier();

				if( control is UISlider && control.ReadOnlyInHierarchy )
					renderer.PopColorMultiplier();

				if( control is UIList && control.ReadOnlyInHierarchy )
					renderer.PopColorMultiplier();

				if( control is UIText && control.ReadOnlyInHierarchy )
				{
					var parentEdit = control.Parent as UIEdit;
					var insideEdit = parentEdit != null && parentEdit.GetTextControl() == control;
					if( !insideEdit )
						renderer.PopColorMultiplier();
				}

				if( control is UICheck && control.ReadOnlyInHierarchy )
					renderer.PopColorMultiplier();
			}

			base.OnAfterRenderUIWithChildren( component, renderer );
		}

		protected virtual void OnRenderBackground( UIControl control, CanvasRenderer renderer )
		{
			//BackgroundColor
			var backgroundColor = control.BackgroundColor.Value;
			if( backgroundColor.Alpha > 0 )
			{
				control.GetScreenRectangle( out var rect );
				var color = backgroundColor.GetSaturate();
				if( color.Alpha > 0 )
					renderer.AddQuad( rect, color );
			}
		}

		protected override void OnRenderComponent( Component component, CanvasRenderer renderer )
		{
			base.OnRenderComponent( component, renderer );

			var control = component as UIControl;
			if( control != null )
				OnRenderBackground( control, renderer );

			//control classes
			if( component is UIButton button )
				OnRenderButton( button, renderer );
			else if( component is UICheck check )
				OnRenderCheck( check, renderer );
			else if( component is UIEdit edit )
				OnRenderEdit( edit, renderer );
			else if( component is UIText text )
				OnRenderText( text, renderer );
			else if( component is UIScroll scroll )
				OnRenderScroll( scroll, renderer );
			else if( component is UIList list )
				OnRenderList( list, renderer );
			else if( component is UIWindow window )
				OnRenderWindow( window, renderer );
			else if( component is UIProgress progress )
				OnRenderProgress( progress, renderer );
			else if( component is UISlider slider )
				OnRenderSlider( slider, renderer );
			else if( component is UIGrid grid )
				OnRenderGrid( grid, renderer );
			else if( component is UICombo combo )
				OnRenderCombo( combo, renderer );
			else if( component is UITooltip tooltip )
				OnRenderTooltip( tooltip, renderer );
			else if( component is UIContextMenu contextMenu )
				OnRenderContextMenu( contextMenu, renderer );
			else if( component is UIToolbar toolbar )
				OnRenderToolbar( toolbar, renderer );
			else if( component is UISplitContainer splitContainer )
				OnRenderSplitContainer( splitContainer, renderer );
			else if( component is UITabControl tabControl )
				OnRenderTabControl( tabControl, renderer );
		}

		protected virtual void OnRenderButton( UIButton control, CanvasRenderer renderer )
		{
			if( control.Parent as UIContextMenu != null )
			{
				//context menu button

				var styleColor = ColorValue.Zero;
				switch( control.State )
				{
				case UIButton.StateEnum.Normal: styleColor = new ColorValue( 0.5, 0.5, 0.5 ); break;
				case UIButton.StateEnum.Hover: styleColor = new ColorValue( 0.65, 0.65, 0.65 ); break;
				case UIButton.StateEnum.Pushed: styleColor = new ColorValue( 0.8, 0.8, 0.8 ); break;
				case UIButton.StateEnum.Highlighted: styleColor = new ColorValue( 0.6, 0.6, 0.6 ); break;
				//case UIButton.StateEnum.Highlighted: styleColor = new ColorValue( 0.6, 0.6, 0 ); break;
				case UIButton.StateEnum.Disabled: styleColor = new ColorValue( 0.8, 0.8, 0.8 ); break;
					//case UIButton.StateEnum.Disabled: styleColor = new ColorValue( 0.5, 0.5, 0.5 ); break;
				}

				control.GetScreenRectangle( out var rect );
				var color = styleColor.GetSaturate();
				if( color.Alpha > 0 )
				{
					//back
					renderer.AddQuad( rect, color );

					//!!!!image

					//text
					var position = new Vector2( rect.Left + control.GetScreenOffsetByValueX( new UIMeasureValueDouble( UIMeasure.Units, 10 ) ), rect.GetCenter().Y ) + new Vector2( 0, renderer.DefaultFontSize / 10 );
					var textColor = new ColorValue( 1, 1, 1 );
					//var textColor = control.State == UIButton.StateEnum.Disabled ? new ColorValue( 0.7, 0.7, 0.7 ) : new ColorValue( 1, 1, 1 );
					renderer.AddText( control.Text, position, EHorizontalAlignment.Left, EVerticalAlignment.Center, textColor );
				}
			}
			else
			{
				//usual button

				var styleColor = ColorValue.Zero;
				switch( control.State )
				{
				case UIButton.StateEnum.Normal: styleColor = control.Focused ? new ColorValue( 0.75, 0.75, 0.75 ) : new ColorValue( 0.5, 0.5, 0.5 ); break;
				//case UIButton.StateEnum.Normal: styleColor = new ColorValue( 0.5, 0.5, 0.5 ); break;
				case UIButton.StateEnum.Hover: styleColor = new ColorValue( 0.65, 0.65, 0.65 ); break;
				case UIButton.StateEnum.Pushed: styleColor = new ColorValue( 0.8, 0.8, 0.8 ); break;
				case UIButton.StateEnum.Highlighted: styleColor = control.Focused ? new ColorValue( 0.8, 0.8, 0.8 ) : new ColorValue( 0.6, 0.6, 0.6 ); break;
				//case UIButton.StateEnum.Highlighted: styleColor = new ColorValue( 0.6, 0.6, 0 ); break;
				case UIButton.StateEnum.Disabled: styleColor = new ColorValue( 0.8, 0.8, 0.8 ); break;
					//case UIButton.StateEnum.Disabled: styleColor = new ColorValue( 0.5, 0.5, 0.5 ); break;
				}

				control.GetScreenRectangle( out var rect );
				var color = styleColor.GetSaturate();
				if( color.Alpha > 0 )
				{
					//back
					renderer.AddQuad( rect, color );

					//image
					if( control.Image.Value != null )
					{
						var image = control.Image.Value;
						if( control.ReadOnlyInHierarchy && control.ImageDisabled.Value != null )
							image = control.ImageDisabled.Value;
						//if( control.ReadOnly && control.ImageDisabled.Value != null )
						//	image = control.ImageDisabled.Value;

						var imageRect = rect;
						imageRect.Expand( -control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 4, 4 ) ) );
						renderer.AddQuad( imageRect, new Rectangle( 0, 0, 1, 1 ), image, new ColorValue( 1, 1, 1 ), true );
					}

					//text
					var position = rect.GetCenter() + new Vector2( 0, renderer.DefaultFontSize / 10 );
					var textColor = new ColorValue( 1, 1, 1 );
					//var textColor = control.State == UIButton.StateEnum.Disabled ? new ColorValue( 0.7, 0.7, 0.7 ) : new ColorValue( 1, 1, 1 );
					renderer.AddText( control.Text, position, EHorizontalAlignment.Center, EVerticalAlignment.Center, textColor );
				}
			}
		}

		protected static Vector2 Multiply( Rectangle rectangle, Vector2 value )
		{
			return new Vector2( rectangle.LeftTop + rectangle.Size * value );
		}

		protected static Rectangle Multiply( Rectangle rectangle, Rectangle value )
		{
			return new Rectangle( Multiply( rectangle, value.LeftTop ), Multiply( rectangle, value.RightBottom ) );
		}

		protected virtual void OnRenderCheck( UICheck control, CanvasRenderer renderer )
		{
			var borderColor = control.Focused ? new ColorValue( 0.7, 0.7, 0.7 ) : new ColorValue( 0.5, 0.5, 0.5 );
			var insideColor = new ColorValue( 0, 0, 0 );

			var checkColor = new ColorValue( 0.8, 0.8, 0.8 );
			//var checkColor = new ColorValue( 1, 1, 0 );
			var textColor = new ColorValue( 1, 1, 1 );

			switch( control.State )
			{
			//case UICheck.StateEnum.Hover:
			//	checkColor = new ColorValue( 1, 1, 1 );
			//	break;

			case UICheck.StateEnum.Pushed:
				checkColor = new ColorValue( 1, 1, 1 );
				break;

			case UICheck.StateEnum.Disabled:
				borderColor = new ColorValue( 0.8, 0.8, 0.8 );
				checkColor = new ColorValue( 0.8, 0.8, 0.8 );
				textColor = new ColorValue( 1, 1, 1 );
				//checkColor = new ColorValue( 0.7, 0.7, 0.7 );
				//textColor = new ColorValue( 0.5, 0.5, 0.5 );
				break;
			}

			var colorMultiplier = new ColorValue( 1, 1, 1 );
			//var colorMultiplier = control.GetTotalColorMultiplier();
			//if( colorMultiplier.Alpha > 0 )
			//{
			control.GetScreenRectangle( out var controlRect );

			var imageRect = new Rectangle( controlRect.Left, controlRect.Top, controlRect.Left + controlRect.Size.Y * renderer.AspectRatioInv, controlRect.Bottom );

			renderer.AddQuad( imageRect, borderColor * colorMultiplier );
			renderer.AddQuad( Multiply( imageRect, new Rectangle( 0.1, 0.1, 0.9, 0.9 ) ), insideColor * colorMultiplier );

			//Checked image
			if( control.Checked.Value == UICheck.CheckValue.Checked )
			{
				var points = new Vector2[]
				{
					new Vector2( 290.04, 33.286 ),
					new Vector2( 118.861, 204.427 ),
					new Vector2( 52.32, 137.907 ),
					new Vector2( 0, 190.226 ),
					new Vector2( 118.861, 309.071 ),
					new Vector2( 342.357, 85.606 ),
				};
				var points2 = new Vector2[ points.Length ];
				for( int n = 0; n < points2.Length; n++ )
					points2[ n ] = points[ n ] / new Vector2( 342.357, 342.357 );

				var color2 = checkColor * colorMultiplier;

				var vertices = new CanvasRenderer.TriangleVertex[ points2.Length ];
				for( int n = 0; n < points2.Length; n++ )
					vertices[ n ] = new CanvasRenderer.TriangleVertex( Multiply( imageRect, points2[ n ] ).ToVector2F(), color2 );

				var indices = new int[] { 0, 1, 5, 5, 4, 1, 1, 2, 3, 3, 1, 4 };

				renderer.AddTriangles( vertices, indices );
			}

			//Indeterminate image
			if( control.Checked.Value == UICheck.CheckValue.Indeterminate )
				renderer.AddQuad( Multiply( imageRect, new Rectangle( 0.3, 0.3, 0.7, 0.7 ) ), checkColor * colorMultiplier );

			//!!!!странно рисует чуть ниже, чем посередине
			//text
			renderer.AddText( " " + control.Text, new Vector2( imageRect.Right, imageRect.GetCenter().Y ), EHorizontalAlignment.Left, EVerticalAlignment.Center, textColor * colorMultiplier );
			//renderer.AddText( " " + control.Text, new Vector2( rect.Right, rect.Top ), EHorizontalAlignment.Left, EVerticalAlignment.Top, textColor * colorMultiplier );
			//}
		}

		protected virtual void OnRenderEdit( UIEdit control, CanvasRenderer renderer )
		{
			var rect = control.GetScreenRectangle();
			renderer.AddQuad( rect, new ColorValue( 0, 0, 0 ) );

			var rect2 = rect;
			//rect2.Expand( -control.ConvertOffset( new UIMeasureValueVector2( UIMeasure.Units, 2, 2 ), UIMeasure.Screen ) );
			rect2.Expand( -control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 2, 2 ) ) );


			var focused = control.Focused;

			//!!!!good?
			var parentCombo = control.Parent as UICombo;
			if( parentCombo != null && parentCombo.GetTextControl() == control && parentCombo.Focused )
				focused = true;

			var color = focused ? new ColorValue( 1, 1, 1 ) : new ColorValue( 0.75, 0.75, 0.75 );

			renderer.AddQuad( new Rectangle( rect.Left, rect.Top, rect2.Left, rect.Bottom ), color );
			renderer.AddQuad( new Rectangle( rect2.Left, rect.Top, rect2.Right, rect2.Top ), color );
			renderer.AddQuad( new Rectangle( rect2.Right, rect.Top, rect.Right, rect.Bottom ), color );
			renderer.AddQuad( new Rectangle( rect.Left, rect2.Bottom, rect2.Right, rect.Bottom ), color );

			//renderer.AddQuad( control.GetScreenRectangle(), new ColorValue( 0.75, 0.75, 0.75 ) );

			//var textControl = control.GetTextControl();
			//if( textControl != null )
			//	renderer.AddQuad( textControl.GetScreenRectangle(), new ColorValue( 0, 0, 0 ) );
		}

		/////////////////////////////////////////

		protected virtual void OnRenderText( UIText control, CanvasRenderer renderer )
		{
			UIText.RenderParentEditData parentEditData = null;

			var parentEdit = control.Parent as UIEdit;
			if( parentEdit != null && parentEdit.GetTextControl() == control )
			{
				parentEditData = new UIText.RenderParentEditData();
				parentEditData.Edit = parentEdit;
				parentEditData.SelectionColor = new ColorValue( 0.75, 0.75, 0.75, 0.75 );
				parentEditData.CaretColor = new ColorValue( 1, 1, 1 );
				if( ( EngineApp.EngineTime - parentEdit.EditingLastTime ) % 1 > 0.5 )
					parentEditData.CaretColor.Alpha = 0;
			}

			control.RenderDefaultStyle( renderer, parentEditData );
		}

		/////////////////////////////////////////

		void GetScrollBarButtonsScreenRectangles( UIScroll control, out Rectangle up, out Rectangle down )
		{
			//!!!!margin

			var rect = control.GetScreenRectangle();

			if( control.Vertical )
			{
				var vsize = rect.Size.X * control.ParentContainer.AspectRatio;
				up = new Rectangle( rect.Left, rect.Top, rect.Right, rect.Top + vsize );
				down = new Rectangle( rect.Left, rect.Bottom - vsize, rect.Right, rect.Bottom );
			}
			else
			{
				var hsize = rect.Size.Y / control.ParentContainer.AspectRatio;
				up = new Rectangle( rect.Left, rect.Top, rect.Left + hsize, rect.Bottom );
				down = new Rectangle( rect.Right - hsize, rect.Top, rect.Right, rect.Bottom );
			}
		}

		protected virtual void OnRenderScroll( UIScroll control, CanvasRenderer renderer )
		{
			var rect = control.GetScreenRectangle();
			GetScrollBarButtonsScreenRectangles( control, out var up, out var down );

			renderer.AddQuad( rect, new ColorValue( 0.5, 0.5, 0.5, 0.5 ) );
			//renderer.AddQuad( rect, new ColorValue( 0, 0, 0 ) );
			//renderer.AddQuad( rect, new ColorValue( 0.5, 0.5, 0.5 ) );

			double factor = control.GetValueFactor();

			var r = new Rectangle(
				Vector2.Lerp( up.LeftTop, down.LeftTop, factor ),
				Vector2.Lerp( up.RightBottom, down.RightBottom, factor ) );

			var c = new ColorValue( 1, 1, 1 );
			renderer.AddQuad( r, c );

			//renderer.AddQuad( up, new ColorValue( 1, 0, 0 ) );
			//renderer.AddQuad( down, new ColorValue( 0, 0, 1 ) );

			////!!!!
			//renderer.AddQuad( rect, new ColorValue( 0.5, 0.5, 0.5 ) );
			////renderer.AddQuad( rect, new ColorValue( 0, 0, 0 ) );

			//var rect2 = rect;
			//rect2.Expand( -control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 2, 2 ) ) );

			//var color = new ColorValue( 0.75, 0.75, 0.75 );
			//renderer.AddQuad( new Rectangle( rect.Left, rect.Top, rect2.Left, rect.Bottom ), color );
			//renderer.AddQuad( new Rectangle( rect2.Left, rect.Top, rect2.Right, rect2.Top ), color );
			//renderer.AddQuad( new Rectangle( rect2.Right, rect.Top, rect.Right, rect.Bottom ), color );
			//renderer.AddQuad( new Rectangle( rect.Left, rect2.Bottom, rect2.Right, rect.Bottom ), color );
		}

		/////////////////////////////////////////

		//!!!!тут? так?
		public double GetListItemSizeScreen( UIList control, CanvasRenderer renderer )
		{
			//var font = control.Font.Value;
			//if( font == null )
			//	font = renderer.DefaultFont;
			//var fontSize = control.GetFontSizeScreen();

			var itemSize = control.ConvertOffsetY( control.ItemSize.Value, UIMeasure.Screen );

			//snap by pixel size
			if( renderer.IsScreen )
			{
				itemSize *= (double)renderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y;
				itemSize = Math.Ceiling( itemSize );
				itemSize /= (double)renderer.ViewportForScreenCanvasRenderer.SizeInPixels.Y;
			}

			return itemSize;
		}

		protected virtual void OnRenderList( UIList control, CanvasRenderer renderer )
		{
			var rect = control.GetScreenRectangle();
			renderer.AddQuad( rect, new ColorValue( 0, 0, 0 ) );
			//renderer.AddQuad( rect, new ColorValue( 0.2, 0.2, 0.2 ) );

			var rect2 = rect;
			rect2.Expand( -control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 2, 2 ) ) );

			//border
			var color = control.Focused ? new ColorValue( 1, 1, 1 ) : new ColorValue( 0.75, 0.75, 0.75 );
			renderer.AddQuad( new Rectangle( rect.Left, rect.Top, rect2.Left, rect.Bottom ), color );
			renderer.AddQuad( new Rectangle( rect2.Left, rect.Top, rect2.Right, rect2.Top ), color );
			renderer.AddQuad( new Rectangle( rect2.Right, rect.Top, rect.Right, rect.Bottom ), color );
			renderer.AddQuad( new Rectangle( rect.Left, rect2.Bottom, rect2.Right, rect.Bottom ), color );

			var font = control.Font.Value;
			if( font == null )
				font = renderer.DefaultFont;
			var fontSize = control.GetFontSizeScreen();
			var itemSize = GetListItemSizeScreen( control, renderer );
			var totalItemsHeight = itemSize * control.Items.Count;
			var scrollBar = control.GetScroll();

			//!!!!тут?
			//update scroll bar properties
			if( scrollBar != null )
			{
				double screenSizeY = rect2.Size.Y;
				double scrollScreenSizeY = totalItemsHeight - screenSizeY;

				scrollBar.Visible = control.AlwaysShowScroll || totalItemsHeight > screenSizeY;
				if( scrollBar.Visible )
					scrollBar.ValueRange = new NeoAxis.Range( 0, scrollScreenSizeY );

				//ensure visible
				if( control.NeedEnsureVisibleInStyle != -1 )
				{
					var index = control.NeedEnsureVisibleInStyle;

					if( (float)index * itemSize > screenSizeY / 2 )
					{
						var factor = (float)index / (float)( control.Items.Count - 1 );
						var v = scrollScreenSizeY * factor;
						scrollBar.Value = MathEx.Clamp( v, 0, scrollBar.ValueRange.Value.Maximum );
					}
					else
						scrollBar.Value = 0;

					control.NeedEnsureVisibleInStyle = -1;
				}

				//if( scrollBar.Visible )
				//{
				//	if( scrollScreenSizeY > 0 )
				//	{
				//		double currentScrollScreenPosY = scrollBar.Value * scrollScreenSizeY;

				//		double itemScrollScreenPosY = itemSize * (double)control.SelectedIndex;
				//		Range itemScrollScreenRangeY = new Range( itemScrollScreenPosY, itemScrollScreenPosY + itemSize );

				//		if( itemScrollScreenRangeY.Minimum < currentScrollScreenPosY )
				//		{
				//			currentScrollScreenPosY = itemScrollScreenRangeY.Minimum;
				//		}
				//		else
				//		{
				//			if( itemScrollScreenRangeY.Maximum > currentScrollScreenPosY + screenSizeY )
				//				currentScrollScreenPosY = itemScrollScreenRangeY.Maximum + itemSize - screenSizeY;
				//		}

				//		scrollBar.Value = currentScrollScreenPosY / scrollScreenSizeY;
				//	}
				//	else
				//		scrollBar.Value = 0;
				//}
				//else
				//	scrollBar.Value = 0;
			}

			//items
			if( control.Items.Count != 0 )
			{
				renderer.PushClipRectangle( rect2 );

				var positionY = rect2.Top;
				if( scrollBar != null && scrollBar.VisibleInHierarchy && scrollBar.EnabledInHierarchy )
					positionY -= scrollBar.Value;

				for( int n = 0; n < control.Items.Count; n++ )
				{
					var item = control.Items[ n ];
					var itemRectangle = new Rectangle( rect2.Left, positionY, rect2.Right, positionY + itemSize );
					if( scrollBar != null && scrollBar.EnabledInHierarchy && scrollBar.VisibleInHierarchy )
						itemRectangle.Right -= scrollBar.GetScreenSize().X;

					if( itemRectangle.Intersects( rect2 ) )
					{
						renderer.PushClipRectangle( itemRectangle );

						if( n == control.SelectedIndex )
						{
							var color2 = new ColorValue( 0.5, 0.5, 0.5 );
							//var color2 = control.ReadOnlyInHierarchy ? new ColorValue( 0.5, 0.5, 0.5 ) : new ColorValue( 0, 0, 0.8 );
							renderer.AddQuad( itemRectangle, color2 );
						}

						var positionX = rect2.Left + control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 2, 0 ) ).X;
						renderer.AddText( font, fontSize, item, new Vector2( positionX, itemRectangle.GetCenter().Y ), EHorizontalAlignment.Left, EVerticalAlignment.Center, new ColorValue( 1, 1, 1 ) );

						renderer.PopClipRectangle();
					}

					positionY += itemSize;
				}

				renderer.PopClipRectangle();
			}
		}

		public override int GetListItemIndexByScreenPosition( UIList control, Vector2 position )
		{
			var renderer = control.ParentContainer.Viewport.CanvasRenderer;
			if( renderer != null )
			{
				var rect = control.GetScreenRectangle();

				var rect2 = rect;
				rect2.Expand( -control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 2, 2 ) ) );

				var font = control.Font.Value;
				if( font == null )
					font = renderer.DefaultFont;
				var fontSize = control.GetFontSizeScreen();
				var itemSize = GetListItemSizeScreen( control, renderer );
				var totalItemsHeight = itemSize * control.Items.Count;
				var scrollBar = control.GetScroll();

				//items
				if( control.Items.Count != 0 && rect2.Contains( position ) )
				{
					var positionY = rect2.Top;
					if( scrollBar != null && scrollBar.VisibleInHierarchy && scrollBar.EnabledInHierarchy )
						positionY -= scrollBar.Value;

					for( int n = 0; n < control.Items.Count; n++ )
					{
						var item = control.Items[ n ];
						var itemRectangle = new Rectangle( rect2.Left, positionY, rect2.Right, positionY + itemSize );
						if( scrollBar != null && scrollBar.EnabledInHierarchy && scrollBar.VisibleInHierarchy )
							itemRectangle.Right -= scrollBar.GetScreenSize().X;

						if( itemRectangle.Intersects( rect2 ) && itemRectangle.Contains( position ) )
							return n;

						positionY += itemSize;
					}
				}
			}

			return -1;
		}

		public override void ListEnsureVisible( UIList control, int index )
		{
			//listEnsureVisible = index;
		}

		/////////////////////////////////////////

		protected virtual void OnRenderWindow( UIWindow control, CanvasRenderer renderer )
		{
			var rect = control.GetScreenRectangle();
			renderer.AddQuad( rect, new ColorValue( 0.15, 0.15, 0.15 ) );

			var rect2 = rect;
			rect2.Expand( -control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 4, 4 ) ) );

			var color = new ColorValue( 0.5, 0.5, 0.5 );
			renderer.AddQuad( new Rectangle( rect.Left, rect.Top, rect2.Left, rect.Bottom ), color );
			renderer.AddQuad( new Rectangle( rect2.Left, rect.Top, rect2.Right, rect2.Top ), color );
			renderer.AddQuad( new Rectangle( rect2.Right, rect.Top, rect.Right, rect.Bottom ), color );
			renderer.AddQuad( new Rectangle( rect.Left, rect2.Bottom, rect2.Right, rect.Bottom ), color );

			if( control.TitleBar.Value )
			{
				double titleBarHeight = 30;
				double screenY = rect.Top + control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 0, titleBarHeight ) ).Y;
				double screenY2 = screenY + control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 0, 4 ) ).Y;

				var rect3 = new Rectangle( rect2.Left, rect2.Top, rect2.Right, screenY2 );
				renderer.AddQuad( rect3, color );

				if( !string.IsNullOrEmpty( control.Text ) )
				{
					var pos = new Vector2( rect.GetCenter().X, ( rect2.Top + screenY ) / 2 );
					renderer.AddText( control.Text, pos, EHorizontalAlignment.Center, EVerticalAlignment.Center, new ColorValue( 1, 1, 1 ) );
				}
			}
		}

		/////////////////////////////////////////

		protected virtual void OnRenderProgress( UIProgress control, CanvasRenderer renderer )
		{
			var rect = control.GetScreenRectangle();
			renderer.AddQuad( rect, new ColorValue( 0, 0, 0 ) );

			if( control.Maximum.Value != 0 )
			{
				double progress = control.Value.Value / control.Maximum.Value;
				if( progress > 0 )
				{
					var rect2 = rect;
					rect2.Expand( -control.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 4, 4 ) ) );
					rect2.Right = MathEx.Lerp( rect2.Left, rect2.Right, progress );

					renderer.AddQuad( rect2, new ColorValue( 1, 1, 1 ) );
				}
			}
		}

		/////////////////////////////////////////

		public override Ray2 GetSliderValuesRayInScreenCoords( UISlider control )
		{
			var rect = control.GetScreenRectangle();
			var center = rect.GetCenter();

			if( control.Vertical )
			{
				var vsize = rect.Size.X * control.ParentContainer.AspectRatio / 4;
				var offset = vsize * 2;

				var start = new Vector2( center.X, rect.Top + offset );
				var end = new Vector2( center.X, rect.Bottom - offset );

				return new Ray2( start, end - start );
			}
			else
			{
				var hsize = rect.Size.Y / control.ParentContainer.AspectRatio / 4;
				var offset = hsize * 2;

				var start = new Vector2( rect.Left + offset, center.Y );
				var end = new Vector2( rect.Right - offset, center.Y );

				return new Ray2( start, end - start );
			}
		}

		public enum SliderElement
		{
			Tick,
			ValueBar,
		}

		public virtual Vector2 GetSliderElementSizeInScreenCoords( UISlider control, SliderElement element )
		{
			var rect = control.GetScreenRectangle();

			if( control.Vertical )
			{
				var hsize = rect.Size.X;
				var vsize = rect.Size.X * control.ParentContainer.AspectRatio / 4;

				if( element == SliderElement.Tick )
				{
					hsize /= 2;
					vsize /= 2;
				}

				return new Vector2( hsize, vsize );
			}
			else
			{
				var hsize = rect.Size.Y / control.ParentContainer.AspectRatio / 4;
				var vsize = rect.Size.Y;

				if( element == SliderElement.Tick )
				{
					hsize /= 2;
					vsize /= 2;
				}

				return new Vector2( hsize, vsize );
			}
		}

		protected virtual void OnRenderSlider( UISlider control, CanvasRenderer renderer )
		{
			var rect = control.GetScreenRectangle();

			renderer.AddQuad( rect, control.Focused ? new ColorValue( 0.7, 0.7, 0.7, 0.5 ) : new ColorValue( 0.5, 0.5, 0.5, 0.5 ) );

			var valuesRay = GetSliderValuesRayInScreenCoords( control );

			//visualize ticks
			var tickBarSize = GetSliderElementSizeInScreenCoords( control, SliderElement.Tick );
			foreach( var factor in control.GetTickFactors() )
			{
				var position = valuesRay.GetPointOnRay( factor );

				var r = new Rectangle( position );
				r.Expand( tickBarSize / 2 );

				var c = new ColorValue( 0.7, 0.7, 0.7, 0.5 );
				renderer.AddQuad( r, c );
			}

			//visualize current value
			{
				var position = valuesRay.GetPointOnRay( control.GetValueFactor() );

				var valueBarSize = GetSliderElementSizeInScreenCoords( control, SliderElement.ValueBar );
				var r = new Rectangle( position );
				r.Expand( valueBarSize / 2 );

				var c = new ColorValue( 1, 1, 1 );
				renderer.AddQuad( r, c );
			}
		}

		protected virtual void OnRenderGrid( UIGrid control, CanvasRenderer renderer )
		{
			if( control.AutoPosition )
				control.UpdateCellsPositionAndSize();

			if( control.DisplayBorders )
			{
				foreach( var cell in control.GetCells() )
				{
					cell.GetScreenRectangle( out var rect );
					renderer.AddRectangle( rect, new ColorValue( 0.5, 0.5, 0.5 ) );
				}
			}
		}

		protected virtual void OnRenderCombo( UICombo control, CanvasRenderer renderer )
		{
			control.RenderDefaultStyle( renderer );
		}

		protected virtual void OnRenderTooltip( UITooltip tooltip, CanvasRenderer renderer )
		{
			var container = tooltip.FindParent<UIContainer>();
			var parentControl = tooltip.Parent as UIControl;
			if( container != null && parentControl != null )
			{
				var text = tooltip.Text.Value;
				if( !string.IsNullOrEmpty( text ) )
				{
					var position = container.ContainerGetMousePosition();

					//cursor offset
					if( container.LastCursorRectangle.Size.Y != 0 )
						position.Y += container.LastCursorRectangle.Size.Y;
					else
						position.Y += parentControl.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Pixels, 0, 32 ) ).Y;

					//get text size
					var width = renderer.DefaultFont.GetTextLength( renderer.DefaultFontSize, renderer, text );
					var height = renderer.DefaultFontSize;

					var rect = new Rectangle( position.X, position.Y, position.X + width, position.Y + height );
					rect.Expand( parentControl.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 4, 4 ) ) );

					//fix rectangle when outside screen
					if( rect.Right > 1 )
					{
						var offset = rect.Right - 1.0;
						rect.Left -= offset;
						rect.Right -= offset;
					}
					if( rect.Bottom > 1 )
					{
						var offset = rect.Bottom - 1.0;
						rect.Top -= offset;
						rect.Bottom -= offset;
					}

					//back
					renderer.AddQuad( rect, new ColorValue( 0.3, 0.3, 0.3 ) );

					//text
					renderer.AddText( text, rect.LeftTop + parentControl.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 2, 2 ) ) );
				}
			}
		}

		protected virtual void OnRenderContextMenu( UIContextMenu menu, CanvasRenderer renderer )
		{
			var borderIndents = menu.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 10, 10 ) );

			var maxTextWidth = 0.0;
			{
				foreach( var itemBase in menu.Items )
				{
					var item = itemBase as UIContextMenu.Item;
					if( item != null )
					{
						var width = renderer.DefaultFont.GetTextLength( renderer.DefaultFontSize, renderer, item.Text );
						if( width > maxTextWidth )
							maxTextWidth = width;
					}
				}
			}

			var textHeight = renderer.DefaultFontSize;

			var buttonSize = new Vector2( maxTextWidth, textHeight ) + menu.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 20, 20 ) );
			var separatorHeight = menu.GetScreenOffsetByValue( new UIMeasureValueVector2( UIMeasure.Units, 0, 10 ) ).Y;

			//update menu rectangle
			{
				var width = borderIndents.X * 2 + buttonSize.X;
				var height = borderIndents.Y * 2;

				foreach( var itemBase in menu.Items )
				{
					var item = itemBase as UIContextMenu.Item;
					if( item != null )
						height += buttonSize.Y;

					var separator = itemBase as UIContextMenu.Separator;
					if( separator != null )
						height += separatorHeight;
				}

				//update menu rectangle

				var rect = new Rectangle( menu.InitialScreenPosition, menu.InitialScreenPosition + new Vector2( width, height ) );

				//fix rectangle when outside screen
				if( rect.Right > 1 )
				{
					var offset = rect.Right - 1.0;
					rect.Left -= offset;
					rect.Right -= offset;
				}
				if( rect.Bottom > 1 )
				{
					var offset = rect.Bottom - 1.0;
					rect.Top -= offset;
					rect.Bottom -= offset;
				}

				menu.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, new Rectangle( rect.LeftTop, Vector2.Zero ) );
				menu.Size = new UIMeasureValueVector2( UIMeasure.Screen, rect.Size );
			}

			//update buttons
			{
				var buttons = menu.GetComponents<UIButton>();

				UIButton GetButtonByItem( UIContextMenu.Item item )
				{
					foreach( var button in buttons )
						if( button.AnyData == item )
							return button;
					return null;
				};

				var currentPositionY = menu.GetScreenPosition().Y + borderIndents.Y;

				foreach( var itemBase in menu.Items )
				{
					var item = itemBase as UIContextMenu.Item;
					if( item != null )
					{
						var button = GetButtonByItem( item );
						if( button != null )
						{
							button.Margin = new UIMeasureValueRectangle( UIMeasure.Screen, menu.GetScreenPosition().X + borderIndents.X, currentPositionY, 0, 0 );
							button.Size = new UIMeasureValueVector2( UIMeasure.Screen, buttonSize );

							currentPositionY += buttonSize.Y;
						}
					}

					var separator = itemBase as UIContextMenu.Separator;
					if( separator != null )
						currentPositionY += separatorHeight;
				}
			}

			//draw default background
			if( menu.BackgroundColor.Value == ColorValue.Zero )
				renderer.AddQuad( menu.GetScreenRectangle(), new ColorValue( 0.3, 0.3, 0.3 ) );
		}

		protected virtual void OnRenderToolbar( UIToolbar control, CanvasRenderer renderer )
		{
			//draw default background
			if( control.BackgroundColor.Value == ColorValue.Zero )
				renderer.AddQuad( control.GetScreenRectangle(), new ColorValue( 0.3, 0.3, 0.3 ) );
		}

		protected virtual void OnRenderSplitContainer( UISplitContainer control, CanvasRenderer renderer )
		{
			//draw default background
			if( control.BackgroundColor.Value == ColorValue.Zero )
				renderer.AddQuad( control.GetScreenRectangle(), new ColorValue( 0.3, 0.3, 0.3 ) );
		}

		protected virtual void OnRenderTabControl( UITabControl control, CanvasRenderer renderer )
		{
		}

	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Represents a empty UI style.
	/// </summary>
	public class UIStyleEmpty : UIStyle
	{
		static UIStyleEmpty instance;
		public static UIStyleEmpty Instance
		{
			get
			{
				if( instance == null )
				{
					instance = new UIStyleEmpty();
					instance.Name = "Empty";
				}
				return instance;
			}
		}
	}
}
