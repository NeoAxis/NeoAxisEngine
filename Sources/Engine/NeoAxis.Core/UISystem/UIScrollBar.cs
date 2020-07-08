// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Standard UI element for scrolling.
	/// </summary>
	public class UIScrollBar : UIControl
	{
		bool cursorInsideArea;
		bool pushed;

		/////////////////////////////////////////

		/// <summary>
		/// Whether the scroll bar is vertical.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Vertical
		{
			get { if( _vertical.BeginGet() ) Vertical = _vertical.Get( this ); return _vertical.value; }
			set { if( _vertical.BeginSet( ref value ) ) { try { VerticalChanged?.Invoke( this ); } finally { _vertical.EndSet(); } } }
		}
		public event Action<UIScrollBar> VerticalChanged;
		ReferenceField<bool> _vertical = false;

		/// <summary>
		/// Specifies the range of possible values.
		/// </summary>
		[DefaultValue( "0 1" )]
		public Reference<Range> ValueRange
		{
			get { if( _valueRange.BeginGet() ) ValueRange = _valueRange.Get( this ); return _valueRange.value; }
			set { if( _valueRange.BeginSet( ref value ) ) { try { ValueRangeChanged?.Invoke( this ); } finally { _valueRange.EndSet(); } } }
		}
		public event Action<UIScrollBar> ValueRangeChanged;
		ReferenceField<Range> _valueRange = new Range( 0, 1 );

		/// <summary>
		/// Specifies the current position of the scroll bar.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> Value
		{
			get { if( _value.BeginGet() ) Value = _value.Get( this ); return _value.value; }
			set { if( _value.BeginSet( ref value ) ) { try { ValueChanged?.Invoke( this ); } finally { _value.EndSet(); } } }
		}
		public event Action<UIScrollBar> ValueChanged;
		ReferenceField<double> _value = 0.0;


		public UIScrollBar()
		{
			Size = new UIMeasureValueVector2( UIMeasure.Units, 400, 30 );
		}

		public double GetValueFactor()
		{
			var range = ValueRange.Value;
			var value = Value.Value;

			double factor;
			if( range[ 1 ] - range[ 0 ] > 0 )
				factor = MathEx.Saturate( ( value - range[ 0 ] ) / ( range[ 1 ] - range[ 0 ] ) );
			else
				factor = range[ 0 ];

			return factor;
		}

		protected override bool OnMouseDown( EMouseButtons button )
		{
			if( button == EMouseButtons.Left && VisibleInHierarchy && cursorInsideArea && EnabledInHierarchy && !ReadOnlyInHierarchy )
			{
				pushed = true;
				Capture = true;
				UpdateValueByMouse();
				return true;
			}

			return base.OnMouseDown( button );
		}

		protected override bool OnMouseUp( EMouseButtons button )
		{
			if( pushed && VisibleInHierarchy && EnabledInHierarchy && !ReadOnlyInHierarchy )
			{
				Capture = false;
				pushed = false;
				return true;
			}

			return base.OnMouseUp( button );
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


		protected override void OnMouseMove( Vector2 mouse )
		{
			base.OnMouseMove( mouse );

			if( EnabledInHierarchy && VisibleInHierarchy && !ReadOnlyInHierarchy )
				cursorInsideArea = CursorIsInArea();
			if( pushed && EnabledInHierarchy )
				UpdateValueByMouse();
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( ( pushed || cursorInsideArea ) && ( !VisibleInHierarchy || !EnabledInHierarchy || ReadOnlyInHierarchy ) )
			{
				if( pushed )
				{
					pushed = false;
					Capture = false;
				}
				cursorInsideArea = false;
			}

			if( EnabledInHierarchy && VisibleInHierarchy && !ReadOnlyInHierarchy )
				cursorInsideArea = CursorIsInArea();

			if( pushed && ( !VisibleInHierarchy || !EnabledInHierarchy || ReadOnlyInHierarchy ) )
			{
				pushed = false;
				Capture = false;
			}
		}

		//Vector2 GetScreenTextureBaseSize()
		//{
		//	double baseHeight = UIControlsWorld.ScaleByResolutionBaseHeight;
		//	return new Vector2( baseHeight * ParentContainer.AspectRatio, baseHeight );
		//}

		void UpdateValueByMouse()
		{
			//!!!!может в стиль

			var rectangle = GetScreenRectangle() * new Vector2( ParentContainer.AspectRatio, 1 );
			//var baseSize = GetScreenTextureBaseSize();
			//rectangle.LeftTop *= baseSize;
			//rectangle.RightBottom *= baseSize;

			double valueCoef;

			if( !Vertical )
			{
				valueCoef = MousePosition.X;
				valueCoef -= .5f;
				valueCoef *= rectangle.Size.X / ( rectangle.Size.X - rectangle.Size.Y );
				valueCoef += .5f;
			}
			else
			{
				valueCoef = MousePosition.Y;
				valueCoef -= .5f;
				valueCoef *= rectangle.Size.Y / ( rectangle.Size.Y - rectangle.Size.X );
				valueCoef += .5f;
			}

			var valueRange = ValueRange.Value;
			var value = valueRange[ 0 ] + valueCoef * ( valueRange[ 1 ] - valueRange[ 0 ] );
			MathEx.Clamp( ref value, valueRange.Minimum, valueRange.Maximum );

			Value = value;
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( pushed && !EnabledInHierarchy )
			{
				pushed = false;
				Capture = false;
			}
		}

		[Browsable( false )]
		public bool CursorInsideArea
		{
			get { return cursorInsideArea; }
		}

		[Browsable( false )]
		public bool Pushed
		{
			get { return pushed; }
		}

		/////////////////////////////////////////

		//protected override void OnUpdate( double delta )
		//{
		//	base.OnUpdate( delta );

		//	var up = GetUpButton();
		//	if( up != null )
		//	{
		//	}
		//}

		//public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		//{
		//	base.NewObjectSetDefaultConfiguration( createdFromNewObjectWindow );

		//	{
		//		var obj = CreateComponent<UIButton>();
		//		obj.Name = "Up";
		//		obj.Text = "Up";
		//		//obj.TextHorizontalAlignment = EHorizontalAlignment.Left;
		//		//obj.TextHorizontalAlignment = ReferenceUtility.MakeThisReference( obj, this, "TextHorizontalAlignment" );

		//		obj.Size = new UIMeasureValueVector2( UIMeasure.Parent, 1, 1 );

		//		obj.CanBeSelected = false;
		//		obj.HorizontalAlignment = EHorizontalAlignment.Left;
		//		obj.VerticalAlignment = EVerticalAlignment.Top;
		//		//obj.Offset = new UIMeasureValueVector2( UIMeasure.Units, 4, 0 );
		//		//obj.ClipRectangle = true;

		//		//UIStyle.EditTextMargin
		//		obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 2, 2, 2, 2 );
		//		//obj.Margin = new UIMeasureValueRectangle( UIMeasure.Units, 2, 2, 2, 2 );
		//	}
		//}

		//public UIButton GetUpButton()
		//{
		//	return GetComponentByPath( "Up" ) as UIButton;
		//}

		//public UIButton GetDownButton()
		//{
		//	return GetComponentByPath( "Down" ) as UIButton;
		//}

		////double valueStep;

		//UIControl barControl;

		////[Serialize]
		////[DefaultValue( 0.0f )]
		////[Category( "Scroll Bar" )]
		////public double ValueStep
		////{
		////   get { return valueStep; }
		////   set { valueStep = value; }
		////}

		//[Serialize]
		//[Browsable( false )]
		//public UIControl BarControl
		//{
		//	get { return barControl; }
		//	set
		//	{
		//		//!!!!!видать по другому это всё
		//		Log.Fatal( "impl" );
		//		//if( barControl != null )
		//		//	Controls.Remove( barControl );
		//		//barControl = value;
		//		//if( barControl != null && barControl.Parent == null )
		//		//	Controls.Add( value );

		//		UpdateBarPosition();
		//	}
		//}

		//protected override void OnEnabledChanged()
		//{
		//	base.OnEnabledChanged();

		//	if( barControl != null )
		//		barControl.Enabled = Enabled;
		//}

		//void UpdateBarPosition()
		//{
		//	if( barControl == null )
		//		return;

		//	Vec2 baseSize = GetScreenTextureBaseSize();

		//	Rect rectangle = GetScreenRectangle();
		//	rectangle.LeftTop *= baseSize;
		//	rectangle.RightBottom *= baseSize;

		//	Vec2 barSize = barControl.GetScreenSize() * baseSize;

		//	Vec2 startPosition;
		//	Vec2 barOffset;

		//	if( !vertical )
		//	{
		//		startPosition = new Vec2( 0, rectangle.Size.Y / 2 - barSize.Y / 2 );
		//		barOffset = new Vec2( rectangle.Size.X - barSize.X, 0 );
		//		//startPosition = new Vec2( rectangle.Size.Y / 2, rectangle.Size.Y / 2 ) - barSize / 2;
		//		//barOffset = new Vec2( rectangle.Size.X - rectangle.Size.Y, 0 );
		//	}
		//	else
		//	{
		//		startPosition = new Vec2( rectangle.Size.X / 2 - barSize.X / 2, 0 );
		//		barOffset = new Vec2( 0, rectangle.Size.Y - barSize.Y );
		//		//startPosition = new Vec2( rectangle.Size.X / 2, rectangle.Size.X / 2 ) - barSize / 2;
		//		//barOffset = new Vec2( 0, rectangle.Size.Y - rectangle.Size.X );
		//	}

		//	double valueCoef;
		//	if( valueRange[ 1 ] - valueRange[ 0 ] >= 0 )
		//		valueCoef = ( value - valueRange[ 0 ] ) / ( valueRange[ 1 ] - valueRange[ 0 ] );
		//	else
		//		valueCoef = 0;

		//	barControl.Position = new ScaleValue( ScaleType.ScaleByResolution,
		//		startPosition + barOffset * valueCoef );
		//}

		//protected override void OnResize()
		//{
		//	base.OnResize();
		//	UpdateBarPosition();
		//}

		//protected override void OnAddedToParent()
		//{
		//	base.OnAddedToParent();
		//	UpdateBarPosition();
		//}

		////protected override UIControl.StandardChildSlotItem[] OnGetStandardChildSlots()
		////{
		////	UIControl.StandardChildSlotItem[] array = new StandardChildSlotItem[ 1 ];
		////	array[ 0 ] = new StandardChildSlotItem( "BarControl", BarControl );
		////	return array;
		////}

		//protected override void OnComponentRemoved( Component component )
		//{
		//	base.OnComponentRemoved( component );

		//	if( component == BarControl )
		//		BarControl = null;
		//}
	}
}
