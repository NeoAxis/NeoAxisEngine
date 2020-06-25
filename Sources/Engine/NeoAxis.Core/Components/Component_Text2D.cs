// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// A scene object displaying 2D text on the screen.
	/// </summary>
	[NewObjectDefaultName( "Text 2D" )]
	public class Component_Text2D : Component_ObjectInSpace
	{
		/// <summary>
		/// The text to display.
		/// </summary>
		[DefaultValue( "Text" )]
		[Category( "Text 2D" )]
		public Reference<string> Text
		{
			get { if( _text.BeginGet() ) Text = _text.Get( this ); return _text.value; }
			set { if( _text.BeginSet( ref value ) ) { try { TextChanged?.Invoke( this ); } finally { _text.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Text"/> property value changes.</summary>
		public event Action<Component_Text2D> TextChanged;
		ReferenceField<string> _text = "Text";

		/// <summary>
		/// The font of rendered text.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Text 2D" )]
		public Reference<Component_Font> Font
		{
			get { if( _font.BeginGet() ) Font = _font.Get( this ); return _font.value; }
			set { if( _font.BeginSet( ref value ) ) { try { FontChanged?.Invoke( this ); } finally { _font.EndSet(); } } }
		}
		public event Action<Component_Text2D> FontChanged;
		ReferenceField<Component_Font> _font = null;

		/// <summary>
		/// Font size of rendered text.
		/// </summary>
		[DefaultValue( "Screen 0.02" )]
		[Category( "Text 2D" )]
		public Reference<UIMeasureValueDouble> FontSize
		{
			get { if( _fontSize.BeginGet() ) FontSize = _fontSize.Get( this ); return _fontSize.value; }
			set { if( _fontSize.BeginSet( ref value ) ) { try { FontSizeChanged?.Invoke( this ); } finally { _fontSize.EndSet(); } } }
		}
		public event Action<Component_Text2D> FontSizeChanged;
		ReferenceField<UIMeasureValueDouble> _fontSize = new UIMeasureValueDouble( UIMeasure.Screen, 0.02 );

		/// <summary>
		/// Whether to display a text in multiline mode. Parsing "\n".
		/// </summary>
		[DefaultValue( true )]
		[Category( "Text 2D" )]
		public Reference<bool> Multiline
		{
			get { if( _multiline.BeginGet() ) Multiline = _multiline.Get( this ); return _multiline.value; }
			set { if( _multiline.BeginSet( ref value ) ) { try { MultilineChanged?.Invoke( this ); } finally { _multiline.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Multiline"/> property value changes.</summary>
		public event Action<Component_Text2D> MultilineChanged;
		ReferenceField<bool> _multiline = true;

		/// <summary>
		/// Vertical space between the margin of control and the start of text.
		/// </summary>
		[DefaultValue( "Units 0" )]
		[Category( "Text 2D" )]
		public Reference<UIMeasureValueDouble> VerticalIndention
		{
			get { if( _verticalIndention.BeginGet() ) VerticalIndention = _verticalIndention.Get( this ); return _verticalIndention.value; }
			set { if( _verticalIndention.BeginSet( ref value ) ) { try { VerticalIndentionChanged?.Invoke( this ); } finally { _verticalIndention.EndSet(); } } }
		}
		public event Action<Component_Text2D> VerticalIndentionChanged;
		ReferenceField<UIMeasureValueDouble> _verticalIndention = new UIMeasureValueDouble( UIMeasure.Units, 0 );

		/// <summary>
		/// The horizontal alignment of the text.
		/// </summary>
		[DefaultValue( EHorizontalAlignment.Center )]
		[Category( "Text 2D" )]
		public Reference<EHorizontalAlignment> HorizontalAlign
		{
			get { if( _horizontalAlign.BeginGet() ) HorizontalAlign = _horizontalAlign.Get( this ); return _horizontalAlign.value; }
			set { if( _horizontalAlign.BeginSet( ref value ) ) { try { HorizontalAlignChanged?.Invoke( this ); } finally { _horizontalAlign.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HorizontalAlign"/> property value changes.</summary>
		public event Action<Component_Text2D> HorizontalAlignChanged;
		ReferenceField<EHorizontalAlignment> _horizontalAlign = EHorizontalAlignment.Center;

		/// <summary>
		/// The vertical alignment of the text.
		/// </summary>
		[DefaultValue( EVerticalAlignment.Center )]
		[Category( "Text 2D" )]
		public Reference<EVerticalAlignment> VerticalAlign
		{
			get { if( _verticalAlign.BeginGet() ) VerticalAlign = _verticalAlign.Get( this ); return _verticalAlign.value; }
			set { if( _verticalAlign.BeginSet( ref value ) ) { try { VerticalAlignChanged?.Invoke( this ); } finally { _verticalAlign.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VerticalAlign"/> property value changes.</summary>
		public event Action<Component_Text2D> VerticalAlignChanged;
		ReferenceField<EVerticalAlignment> _verticalAlign = EVerticalAlignment.Center;

		/// <summary>
		/// The color of the text.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Text 2D" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Component_Text2D> ColorChanged;
		ReferenceField<ColorValue> _color = ColorValue.One;

		///// <summary>
		///// The color of the lines when they are behind other objects.
		///// </summary>
		//[DefaultValue( "1 1 1 0.3" )]
		//public Reference<ColorValue> ColorInvisible
		//{
		//	get { if( _colorInvisible.BeginGet() ) ColorInvisible = _colorInvisible.Get( this ); return _colorInvisible.value; }
		//	set { if( _colorInvisible.BeginSet( ref value ) ) { try { ColorInvisibleChanged?.Invoke( this ); } finally { _colorInvisible.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ColorInvisible"/> property value changes.</summary>
		//public event Action<Component_Text2D> ColorInvisibleChanged;
		//ReferenceField<ColorValue> _colorInvisible = new ColorValue( 1, 1, 1, 0.3 );

		/// <summary>
		/// Draw the shadow behind the text.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Text 2D" )]
		public Reference<bool> Shadow
		{
			get { if( _shadow.BeginGet() ) Shadow = _shadow.Get( this ); return _shadow.value; }
			set { if( _shadow.BeginSet( ref value ) ) { try { ShadowChanged?.Invoke( this ); } finally { _shadow.EndSet(); } } }
		}
		public event Action<Component_Text2D> ShadowChanged;
		ReferenceField<bool> _shadow = false;

		/// <summary>
		/// Extra offset added to the text shadow position.
		/// </summary>
		[DefaultValue( "Units 1 1" )]
		[Category( "Text 2D" )]
		public Reference<UIMeasureValueVector2> ShadowOffset
		{
			get { if( _shadowOffset.BeginGet() ) ShadowOffset = _shadowOffset.Get( this ); return _shadowOffset.value; }
			set { if( _shadowOffset.BeginSet( ref value ) ) { try { ShadowOffsetChanged?.Invoke( this ); } finally { _shadowOffset.EndSet(); } } }
		}
		public event Action<Component_Text2D> ShadowOffsetChanged;
		ReferenceField<UIMeasureValueVector2> _shadowOffset = new UIMeasureValueVector2( UIMeasure.Units, 1, 1 );

		/// <summary>
		/// The color of the text shadow.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Text 2D" )]
		public Reference<ColorValue> ShadowColor
		{
			get { if( _shadowColor.BeginGet() ) ShadowColor = _shadowColor.Get( this ); return _shadowColor.value; }
			set { if( _shadowColor.BeginSet( ref value ) ) { try { ShadowColorChanged?.Invoke( this ); } finally { _shadowColor.EndSet(); } } }
		}
		public event Action<Component_Text2D> ShadowColorChanged;
		ReferenceField<ColorValue> _shadowColor = new ColorValue( 0, 0, 0 );

		/// <summary>
		/// Whether to use pixel aligned rendering of the text.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Text 2D" )]
		public Reference<bool> PixelAlign
		{
			get { if( _pixelAlign.BeginGet() ) PixelAlign = _pixelAlign.Get( this ); return _pixelAlign.value; }
			set { if( _pixelAlign.BeginSet( ref value ) ) { try { PixelAlignChanged?.Invoke( this ); } finally { _pixelAlign.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PixelAlign"/> property value changes.</summary>
		public event Action<Component_Text2D> PixelAlignChanged;
		ReferenceField<bool> _pixelAlign = true;

		/// <summary>
		/// How far from the camera lines are visible.
		/// </summary>
		[DefaultValue( 50.0 )]
		[Range( 1, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[Category( "Text 2D" )]
		public Reference<double> VisibilityDistance
		{
			get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
			set { if( _visibilityDistance.BeginSet( ref value ) ) { try { VisibilityDistanceChanged?.Invoke( this ); } finally { _visibilityDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		public event Action<Component_Text2D> VisibilityDistanceChanged;
		ReferenceField<double> _visibilityDistance = 50.0;

		/// <summary>
		/// The radius of object bounding sphere to detect visibility.
		/// </summary>
		[DefaultValue( 3.0 )]
		[Range( 1.0, 10.0 )]
		[Category( "Text 2D" )]
		public Reference<double> BoundingRadius
		{
			get { if( _boundingRadius.BeginGet() ) BoundingRadius = _boundingRadius.Get( this ); return _boundingRadius.value; }
			set { if( _boundingRadius.BeginSet( ref value ) ) { try { BoundingRadiusChanged?.Invoke( this ); SpaceBoundsUpdate(); } finally { _boundingRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BoundingRadius"/> property value changes.</summary>
		public event Action<Component_Text2D> BoundingRadiusChanged;
		ReferenceField<double> _boundingRadius = 3.0;

		/// <summary>
		/// Whether the component is visible in the simulation mode.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Text 2D" )]
		public Reference<bool> DisplayInSimulation
		{
			get { if( _displayInSimulation.BeginGet() ) DisplayInSimulation = _displayInSimulation.Get( this ); return _displayInSimulation.value; }
			set { if( _displayInSimulation.BeginSet( ref value ) ) { try { DisplayInSimulationChanged?.Invoke( this ); } finally { _displayInSimulation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayInSimulation"/> property value changes.</summary>
		public event Action<Component_Text2D> DisplayInSimulationChanged;
		ReferenceField<bool> _displayInSimulation = true;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( VerticalIndention ):
					if( !Multiline )
						skip = true;
					break;

				case nameof( ShadowOffset ):
				case nameof( ShadowColor ):
					if( !Shadow )
						skip = true;
					break;
				}
			}
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			newBounds = new SpaceBounds( new Sphere( TransformV.Position, BoundingRadius ) );
		}

		protected virtual void Render( ViewportRenderingContext context, Vector2 screenPosition )
		{
			if( !string.IsNullOrEmpty( Text ) )
			{
				if( Multiline )
					RenderMultilineEnabled( context, screenPosition );
				else
					RenderMultilineDisabled( context, screenPosition );
			}
		}

		public override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode )
		{
			base.OnGetRenderSceneData( context, mode );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.objectInSpaceRenderingContext;

				if( EngineApp.ApplicationType != EngineApp.ApplicationTypeEnum.Simulation ||
					EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation && DisplayInSimulation )
				{
					var cameraPosition = context.Owner.CameraSettings.Position;
					var sphere = new Sphere( cameraPosition, VisibilityDistance );

					if( SpaceBounds.CalculatedBoundingSphere.Intersects( ref sphere ) )
					{
						var canvasRenderer = context.Owner.CanvasRenderer;
						var renderer = context.Owner.Simple3DRenderer;

						if( canvasRenderer != null && renderer != null )
						{
							if( context.Owner.CameraSettings.ProjectToScreenCoordinates( TransformV.Position, out var screenPosition ) )
								Render( context, screenPosition );
						}
					}
				}
			}
		}

		void RenderMultilineDisabled( ViewportRenderingContext context, Vector2 screenPosition )
		{
			var renderer = context.Owner.CanvasRenderer;
			var fontSize = GetFontSizeScreen( context );
			var options = PixelAlign.Value ? CanvasRenderer.AddTextOptions.PixelAlign : 0;

			if( Shadow )
			{
				ColorValue color = ShadowColor.Value.GetSaturate();
				renderer.AddText( Font, fontSize, Text,
					screenPosition + ConvertOffsetToScreen( context, ShadowOffset ),
					HorizontalAlign, VerticalAlign, color, options );
			}

			{
				ColorValue color = Color.Value.GetSaturate();
				renderer.AddText( Font, fontSize, Text, screenPosition, HorizontalAlign, VerticalAlign, Color, options );
			}
		}

		struct LineItem
		{
			public string text;
			public bool alignByWidth;
			public LineItem( string text, bool alignByWidth )
			{
				this.text = text;
				this.alignByWidth = alignByWidth;
			}
		}

		void RenderMultilineEnabled( ViewportRenderingContext context, Vector2 screenPosition )
		{
			var renderer = context.Owner.CanvasRenderer;
			var text = Text.Value;
			var options = PixelAlign.Value ? CanvasRenderer.AddTextOptions.PixelAlign : 0;

			var verticalIndention = VerticalIndention.Value;
			var screenVerticalIndention = ConvertOffsetToScreen( context, new UIMeasureValueVector2( verticalIndention.Measure, 0, verticalIndention.Value ) ).Y;

			var font = Font.Value;
			if( font == null )
				font = renderer.DefaultFont;
			if( font == null || font.Disposed )
				return;

			var fontSize = GetFontSizeScreen( context );

			var lines = new List<LineItem>();
			{
				var strLines = text.Split( new string[] { "\\n" }, StringSplitOptions.None );
				for( int n = 0; n < strLines.Length; n++ )
				{
					var alignByWidth = HorizontalAlign.Value == EHorizontalAlignment.Stretch && n != strLines.Length - 1;
					lines.Add( new LineItem( strLines[ n ], alignByWidth ) );
				}
			}

			if( lines.Count != 0 )
			{
				var shadowColor = ShadowColor.Value;
				var textColor = Color.Value;
				var shadowScreenOffset = Shadow ? ConvertOffsetToScreen( context, ShadowOffset ) : Vector2.Zero;

				var screenSize = 0.0;
				foreach( LineItem line in lines )
				{
					var size = font.GetTextLength( fontSize, renderer, line.text );
					screenSize = Math.Max( screenSize, size );
				}

				double startY = 0;
				switch( VerticalAlign.Value )
				{
				case EVerticalAlignment.Top:
				case EVerticalAlignment.Stretch:
					startY = screenPosition.Y;
					break;
				case EVerticalAlignment.Center:
					{
						double height = fontSize * (double)lines.Count + screenVerticalIndention * ( (double)lines.Count - 1 );
						startY = screenPosition.Y - height / 2;
					}
					break;
				case EVerticalAlignment.Bottom:
					{
						double height = fontSize * (double)lines.Count + screenVerticalIndention * ( (double)lines.Count - 1 );
						startY = screenPosition.Y - height;
					}
					break;
				}

				double stepY = fontSize + screenVerticalIndention;

				for( int nStep = Shadow ? 0 : 1; nStep < 2; nStep++ )
				{
					double positionY = startY;
					foreach( LineItem line in lines )
					{
						if( line.alignByWidth )
						{
							string[] words = line.text.Split( new char[] { ' ' } );
							double[] lengths = new double[ words.Length ];
							double totalLength = 0;
							for( int n = 0; n < lengths.Length; n++ )
							{
								double length = font.GetTextLength( fontSize, renderer, words[ n ] );
								lengths[ n ] = length;
								totalLength += length;
							}

							double space = 0;
							if( words.Length > 1 )
								space = ( screenSize - totalLength ) / ( words.Length - 1 );

							double posX = screenPosition.X;
							for( int n = 0; n < words.Length; n++ )
							{
								Vector2 pos = new Vector2( posX, positionY );// + ConvertOffsetToScreen( context, Offset );

								if( nStep == 0 )
								{
									renderer.AddText( font, fontSize, words[ n ],
										pos + shadowScreenOffset,//LocalToScreen( ScreenToLocal( pos ) + shadowLocalOffset ),
										EHorizontalAlignment.Left, EVerticalAlignment.Top, shadowColor, options );
								}
								else
									renderer.AddText( font, fontSize, words[ n ], pos, EHorizontalAlignment.Left, EVerticalAlignment.Top, textColor, options );

								posX += lengths[ n ] + space;
							}
						}
						else
						{
							var horizontalAlign = EHorizontalAlignment.Left;

							double positionX = 0;
							switch( HorizontalAlign.Value )
							{
							case EHorizontalAlignment.Left:
							case EHorizontalAlignment.Stretch:
								positionX = screenPosition.X;
								break;
							case EHorizontalAlignment.Center:
								horizontalAlign = EHorizontalAlignment.Center;
								positionX = screenPosition.X;
								//positionX = screenPosition.X - screenSize / 2;
								break;
							case EHorizontalAlignment.Right:
								horizontalAlign = EHorizontalAlignment.Right;
								positionX = screenPosition.X;
								//positionX = screenPosition.X - screenSize;
								break;
							}

							Vector2 pos = new Vector2( positionX, positionY );// + ConvertOffsetToScreen( context, Offset );

							if( nStep == 0 )
							{
								renderer.AddText( font, fontSize, line.text,
									pos + shadowScreenOffset,//LocalToScreen( ScreenToLocal( pos ) + shadowLocalOffset ),
									horizontalAlign, EVerticalAlignment.Top, shadowColor, options );
							}
							else
								renderer.AddText( font, fontSize, line.text, pos, horizontalAlign, EVerticalAlignment.Top, textColor, options );
						}

						positionY += stepY;
					}
				}
			}
		}

		Vector2 DivideWithZeroCheck( Vector2 v1, Vector2 v2 )
		{
			Vector2 v = Vector2.Zero;
			if( v2.X != 0 )
				v.X = v1.X / v2.X;
			if( v2.Y != 0 )
				v.Y = v1.Y / v2.Y;
			return v;
		}

		double GetParentContainerAspectRatio( ViewportRenderingContext context )
		{
			var sizeInPixels = context.Owner.SizeInPixels;
			return (double)sizeInPixels.X / (double)sizeInPixels.Y;
		}

		Vector2 GetParentContainerSizeInUnits( ViewportRenderingContext context )
		{
			double baseHeight = 1000;//UIControlsWorld.ScaleByResolutionBaseHeight;
			return new Vector2( baseHeight * GetParentContainerAspectRatio( context ), baseHeight );
		}

		Vector2 GetParentContainerSizeInPixels( ViewportRenderingContext context )
		{
			return context.Owner.SizeInPixels.ToVector2();
		}

		Vector2 ConvertOffsetToScreen( ViewportRenderingContext context, UIMeasureValueVector2 value )
		{
			if( value.Value == Vector2.Zero )
				return Vector2.Zero;

			Vector2 screen = Vector2.Zero;

			//from
			switch( value.Measure )
			{
			case UIMeasure.Parent:
				screen = value.Value;// * GetScreenSize();
				break;
			case UIMeasure.Screen:
				screen = value.Value;
				break;
			case UIMeasure.Units:
				screen = DivideWithZeroCheck( value.Value, GetParentContainerSizeInUnits( context ) );
				break;
			case UIMeasure.Pixels:
				screen = DivideWithZeroCheck( value.Value, GetParentContainerSizeInPixels( context ) );
				break;
			}

			return screen;
		}

		double GetFontSizeScreen( ViewportRenderingContext context )
		{
			var value = FontSize.Value;
			var fontSize = ConvertOffsetToScreen( context, new UIMeasureValueVector2( value.Measure, 0, value.Value ) ).Y;
			return fontSize;
		}
	}
}
