// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	/// <summary>
	/// Represents a slider.
	/// </summary>
	public class UISlider : UIControl
	{
		bool cursorInsideArea;
		bool pushed;
		object touchDown;

		/////////////////////////////////////////

		/// <summary>
		/// Whether the slider is vertical.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> Vertical
		{
			get { if( _vertical.BeginGet() ) Vertical = _vertical.Get( this ); return _vertical.value; }
			set { if( _vertical.BeginSet( ref value ) ) { try { VerticalChanged?.Invoke( this ); } finally { _vertical.EndSet(); } } }
		}
		public event Action<UISlider> VerticalChanged;
		ReferenceField<bool> _vertical = false;

		/// <summary>
		/// Specifies the range of possible values.
		/// </summary>
		[DefaultValue( "0 10" )]
		public Reference<Range> ValueRange
		{
			get { if( _valueRange.BeginGet() ) ValueRange = _valueRange.Get( this ); return _valueRange.value; }
			set { if( _valueRange.BeginSet( ref value ) ) { try { ValueRangeChanged?.Invoke( this ); } finally { _valueRange.EndSet(); } } }
		}
		public event Action<UISlider> ValueRangeChanged;
		ReferenceField<Range> _valueRange = new Range( 0, 10 );

		/// <summary>
		/// Specifies the current position of the slider.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> Value
		{
			get { if( _value.BeginGet() ) Value = _value.Get( this ); return _value.value; }
			set { if( _value.BeginSet( ref value ) ) { try { ValueChanged?.Invoke( this ); } finally { _value.EndSet(); } } }
		}
		public event Action<UISlider> ValueChanged;
		ReferenceField<double> _value = 0.0;

		/// <summary>
		/// The step of possible values.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> Step
		{
			get { if( _step.BeginGet() ) Step = _step.Get( this ); return _step.value; }
			set { if( _step.BeginSet( ref value ) ) { try { StepChanged?.Invoke( this ); } finally { _step.EndSet(); } } }
		}
		public event Action<UISlider> StepChanged;
		ReferenceField<double> _step = 1.0;

		/// <summary>
		/// The step of visual ticks.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> TickFrequency
		{
			get { if( _tickFrequency.BeginGet() ) TickFrequency = _tickFrequency.Get( this ); return _tickFrequency.value; }
			set { if( _tickFrequency.BeginSet( ref value ) ) { try { TickFrequencyChanged?.Invoke( this ); } finally { _tickFrequency.EndSet(); } } }
		}
		public event Action<UISlider> TickFrequencyChanged;
		ReferenceField<double> _tickFrequency = 1.0;

		/////////////////////////////////////////

		public UISlider()
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

		public double[] GetTickFactors()
		{
			var result = new List<double>();

			var tickFrequency = TickFrequency.Value;
			if( tickFrequency > 0 )
			{
				var range = ValueRange.Value;
				double current = range.Minimum;
				while( current < range.Maximum + tickFrequency / 1000 )
				{
					var factor = MathEx.Saturate( ( current - range[ 0 ] ) / ( range[ 1 ] - range[ 0 ] ) );
					result.Add( factor );
					current += tickFrequency;
				}
			}

			return result.ToArray();
		}

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
				case EKeys.Left:
				case EKeys.Up:
					{
						var value = Value.Value - Step.Value;
						MathEx.Clamp( ref value, ValueRange.Value );
						Value = value;
					}
					return true;

				case EKeys.Right:
				case EKeys.Down:
					{
						var value = Value.Value + Step.Value;
						MathEx.Clamp( ref value, ValueRange.Value );
						Value = value;
					}
					return true;

				case EKeys.Home:
				case EKeys.PageUp:
					Value = ValueRange.Value.Minimum;
					return true;

				case EKeys.End:
				case EKeys.PageDown:
					Value = ValueRange.Value.Maximum;
					return true;
				}
			}

			return base.OnKeyDown( e );
		}

		protected override bool OnMouseDown( EMouseButtons button )
		{
			if( button == EMouseButtons.Left && VisibleInHierarchy && cursorInsideArea && EnabledInHierarchy && !ReadOnlyInHierarchy )
			{
				Focus();
				pushed = true;
				Capture = true;
				UpdateValueByCursor();
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
			//touch
			if( touchDown != null )
				return true;

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
				UpdateValueByCursor();
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

					var item = new TouchData.TouchDownRequestToProcessTouch( this, 0, distanceInPixels, null,
						delegate ( UIControl sender, TouchData touchData, object anyData )
						{
							//start touch
							touchDown = e.PointerIdentifier;
							UpdateValueByCursor( e.Position );
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
					UpdateValueByCursor( e.Position );
				break;

				//case TouchData.ActionEnum.Cancel:
				//	break;

				//case TouchData.ActionEnum.Outside:
				//	break;
			}

			return base.OnTouch( e );
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

		void UpdateValueByCursor( Vector2? cursorScreenPosition = null )
		{
			var style = GetStyle();
			var valuesRay = style.GetSliderValuesRayInScreenCoords( this );
			Vector2 mouse = cursorScreenPosition != null ? cursorScreenPosition.Value : ConvertLocalToScreen( MousePosition );

			double valueCoef = 0;
			if( Vertical )
			{
				if( valuesRay.Direction.Y != 0 )
					valueCoef = ( mouse.Y - valuesRay.Origin.Y ) / valuesRay.Direction.Y;
			}
			else
			{
				if( valuesRay.Direction.X != 0 )
					valueCoef = ( mouse.X - valuesRay.Origin.X ) / valuesRay.Direction.X;
			}

			var valueRange = ValueRange.Value;
			var value = valueRange[ 0 ] + valueCoef * ( valueRange[ 1 ] - valueRange[ 0 ] );

			var step = Step.Value;
			if( step != 0 )
			{
				value += step / 2;
				value /= step;
				value = (int)value;
				value *= step;
			}

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
