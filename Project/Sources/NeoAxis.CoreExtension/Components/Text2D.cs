// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A scene object displaying 2D text on the screen.
	/// </summary>
	[NewObjectDefaultName( "Text 2D" )]
	[AddToResourcesWindow( @"Base\Scene objects\Additional\Text 2D", 0 )]
	public class Text2D : ObjectInSpace
	{
		/// <summary>
		/// The text to display.
		/// </summary>
		[DefaultValue( "Text" )]
		[Category( "Text" )]
#if !DEPLOY
		[Editor( typeof( Editor.HCItemTextBoxDropMultiline ), typeof( object ) )]
#endif
		public Reference<string> Text
		{
			get { if( _text.BeginGet() ) Text = _text.Get( this ); return _text.value; }
			set { if( _text.BeginSet( ref value ) ) { try { TextChanged?.Invoke( this ); } finally { _text.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Text"/> property value changes.</summary>
		public event Action<Text2D> TextChanged;
		ReferenceField<string> _text = "Text";

		/// <summary>
		/// Whether to display a text in multiline mode.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Text" )]
		public Reference<bool> Multiline
		{
			get { if( _multiline.BeginGet() ) Multiline = _multiline.Get( this ); return _multiline.value; }
			set { if( _multiline.BeginSet( ref value ) ) { try { MultilineChanged?.Invoke( this ); } finally { _multiline.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Multiline"/> property value changes.</summary>
		public event Action<Text2D> MultilineChanged;
		ReferenceField<bool> _multiline = true;

		/// <summary>
		/// The horizontal alignment of the multiline text.
		/// </summary>
		[DefaultValue( EHorizontalAlignment.Center )]
		[Category( "Text" )]
		public Reference<EHorizontalAlignment> TextHorizontalAlignment
		{
			get { if( _textHorizontalAlignment.BeginGet() ) TextHorizontalAlignment = _textHorizontalAlignment.Get( this ); return _textHorizontalAlignment.value; }
			set { if( _textHorizontalAlignment.BeginSet( ref value ) ) { try { TextHorizontalAlignmentChanged?.Invoke( this ); } finally { _textHorizontalAlignment.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TextHorizontalAlignment"/> property value changes.</summary>
		public event Action<Text2D> TextHorizontalAlignmentChanged;
		ReferenceField<EHorizontalAlignment> _textHorizontalAlignment = EHorizontalAlignment.Center;

		/// <summary>
		/// Vertical space between lines in multiline mode.
		/// </summary>
		[DefaultValue( "Units 0" )]
		[Category( "Text" )]
		public Reference<UIMeasureValueDouble> VerticalIndention
		{
			get { if( _verticalIndention.BeginGet() ) VerticalIndention = _verticalIndention.Get( this ); return _verticalIndention.value; }
			set { if( _verticalIndention.BeginSet( ref value ) ) { try { VerticalIndentionChanged?.Invoke( this ); } finally { _verticalIndention.EndSet(); } } }
		}
		public event Action<Text2D> VerticalIndentionChanged;
		ReferenceField<UIMeasureValueDouble> _verticalIndention = new UIMeasureValueDouble( UIMeasure.Units, 0 );

		/// <summary>
		/// The font of rendered text.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Text" )]
		public Reference<FontComponent> Font
		{
			get { if( _font.BeginGet() ) Font = _font.Get( this ); return _font.value; }
			set { if( _font.BeginSet( ref value ) ) { try { FontChanged?.Invoke( this ); } finally { _font.EndSet(); } } }
		}
		public event Action<Text2D> FontChanged;
		ReferenceField<FontComponent> _font = null;

		/// <summary>
		/// Font size of rendered text.
		/// </summary>
		[DefaultValue( "Screen 0.025" )]
		[Category( "Text" )]
		public Reference<UIMeasureValueDouble> FontSize
		{
			get { if( _fontSize.BeginGet() ) FontSize = _fontSize.Get( this ); return _fontSize.value; }
			set { if( _fontSize.BeginSet( ref value ) ) { try { FontSizeChanged?.Invoke( this ); } finally { _fontSize.EndSet(); } } }
		}
		public event Action<Text2D> FontSizeChanged;
		ReferenceField<UIMeasureValueDouble> _fontSize = new UIMeasureValueDouble( UIMeasure.Screen, 0.025 );

		/// <summary>
		/// The color of the text.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Category( "Text" )]
		public Reference<ColorValue> Color
		{
			get { if( _color.BeginGet() ) Color = _color.Get( this ); return _color.value; }
			set { if( _color.BeginSet( ref value ) ) { try { ColorChanged?.Invoke( this ); } finally { _color.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Color"/> property value changes.</summary>
		public event Action<Text2D> ColorChanged;
		ReferenceField<ColorValue> _color = ColorValue.One;

		/// <summary>
		/// Whether to use pixel aligned rendering of the text.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Text" )]
		public Reference<bool> PixelAlign
		{
			get { if( _pixelAlign.BeginGet() ) PixelAlign = _pixelAlign.Get( this ); return _pixelAlign.value; }
			set { if( _pixelAlign.BeginSet( ref value ) ) { try { PixelAlignChanged?.Invoke( this ); } finally { _pixelAlign.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PixelAlign"/> property value changes.</summary>
		public event Action<Text2D> PixelAlignChanged;
		ReferenceField<bool> _pixelAlign = true;

		/// <summary>
		/// Draw the shadow behind the text.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Shadow" )]
		public Reference<bool> Shadow
		{
			get { if( _shadow.BeginGet() ) Shadow = _shadow.Get( this ); return _shadow.value; }
			set { if( _shadow.BeginSet( ref value ) ) { try { ShadowChanged?.Invoke( this ); } finally { _shadow.EndSet(); } } }
		}
		public event Action<Text2D> ShadowChanged;
		ReferenceField<bool> _shadow = true;

		/// <summary>
		/// Extra offset added to the text shadow position.
		/// </summary>
		[DefaultValue( "Pixels 2 2" )]//!!!!Units 1 1
		[Category( "Shadow" )]
		public Reference<UIMeasureValueVector2> ShadowOffset
		{
			get { if( _shadowOffset.BeginGet() ) ShadowOffset = _shadowOffset.Get( this ); return _shadowOffset.value; }
			set { if( _shadowOffset.BeginSet( ref value ) ) { try { ShadowOffsetChanged?.Invoke( this ); } finally { _shadowOffset.EndSet(); } } }
		}
		public event Action<Text2D> ShadowOffsetChanged;
		ReferenceField<UIMeasureValueVector2> _shadowOffset = new UIMeasureValueVector2( UIMeasure.Pixels, 2, 2 );

		/// <summary>
		/// The color of the text shadow.
		/// </summary>
		[DefaultValue( "0 0 0 0.7" )]
		[Category( "Shadow" )]
		public Reference<ColorValue> ShadowColor
		{
			get { if( _shadowColor.BeginGet() ) ShadowColor = _shadowColor.Get( this ); return _shadowColor.value; }
			set { if( _shadowColor.BeginSet( ref value ) ) { try { ShadowColorChanged?.Invoke( this ); } finally { _shadowColor.EndSet(); } } }
		}
		public event Action<Text2D> ShadowColorChanged;
		ReferenceField<ColorValue> _shadowColor = new ColorValue( 0, 0, 0, 0.7 );

		/// <summary>
		/// Draw the back rentangle behind the text.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Back" )]
		public Reference<bool> Back
		{
			get { if( _back.BeginGet() ) Back = _back.Get( this ); return _back.value; }
			set { if( _back.BeginSet( ref value ) ) { try { BackChanged?.Invoke( this ); } finally { _back.EndSet(); } } }
		}
		public event Action<Text2D> BackChanged;
		ReferenceField<bool> _back = false;

		/// <summary>
		/// The color of the back rectangle.
		/// </summary>
		[DefaultValue( "0.7 0.7 0.7" )]
		[Category( "Back" )]
		public Reference<ColorValue> BackColor
		{
			get { if( _BackColor.BeginGet() ) BackColor = _BackColor.Get( this ); return _BackColor.value; }
			set { if( _BackColor.BeginSet( ref value ) ) { try { BackColorChanged?.Invoke( this ); } finally { _BackColor.EndSet(); } } }
		}
		public event Action<Text2D> BackColorChanged;
		ReferenceField<ColorValue> _BackColor = new ColorValue( 0.7, 0.7, 0.7 );

		public enum BackStyleEnum
		{
			Rectangle,
			RoundedRectangle,
		}

		/// <summary>
		/// The size increase of the back rectangle depending by font size.
		/// </summary>
		[DefaultValue( "0.3 0.3" )]
		[Range( 0, 1 )]
		[Category( "Back" )]
		public Reference<Vector2> BackSizeAdd
		{
			get { if( _backSizeAdd.BeginGet() ) BackSizeAdd = _backSizeAdd.Get( this ); return _backSizeAdd.value; }
			set { if( _backSizeAdd.BeginSet( ref value ) ) { try { BackSizeAddChanged?.Invoke( this ); } finally { _backSizeAdd.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BackSizeAdd"/> property value changes.</summary>
		public event Action<Text2D> BackSizeAddChanged;
		ReferenceField<Vector2> _backSizeAdd = new Vector2( 0.3, 0.3 );

		/// <summary>
		/// The form of the back rectangle.
		/// </summary>
		[DefaultValue( BackStyleEnum.Rectangle )]
		[Category( "Back" )]
		public Reference<BackStyleEnum> BackStyle
		{
			get { if( _backStyle.BeginGet() ) BackStyle = _backStyle.Get( this ); return _backStyle.value; }
			set { if( _backStyle.BeginSet( ref value ) ) { try { BackStyleChanged?.Invoke( this ); } finally { _backStyle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BackStyle"/> property value changes.</summary>
		public event Action<Text2D> BackStyleChanged;
		ReferenceField<BackStyleEnum> _backStyle = BackStyleEnum.Rectangle;

		/// <summary>
		/// The horizontal alignment on the screen.
		/// </summary>
		[DefaultValue( EHorizontalAlignment.Center )]
		[Category( "Displaying In Scene" )]
		public Reference<EHorizontalAlignment> HorizontalAlignment
		{
			get { if( _horizontalAlignment.BeginGet() ) HorizontalAlignment = _horizontalAlignment.Get( this ); return _horizontalAlignment.value; }
			set { if( _horizontalAlignment.BeginSet( ref value ) ) { try { HorizontalAlignmentChanged?.Invoke( this ); } finally { _horizontalAlignment.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HorizontalAlignment"/> property value changes.</summary>
		public event Action<Text2D> HorizontalAlignmentChanged;
		ReferenceField<EHorizontalAlignment> _horizontalAlignment = EHorizontalAlignment.Center;

		/// <summary>
		/// The vertical alignment on the screen.
		/// </summary>
		[DefaultValue( EVerticalAlignment.Center )]
		[Category( "Displaying In Scene" )]
		public Reference<EVerticalAlignment> VerticalAlignment
		{
			get { if( _verticalAlignment.BeginGet() ) VerticalAlignment = _verticalAlignment.Get( this ); return _verticalAlignment.value; }
			set { if( _verticalAlignment.BeginSet( ref value ) ) { try { VerticalAlignmentChanged?.Invoke( this ); } finally { _verticalAlignment.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VerticalAlignment"/> property value changes.</summary>
		public event Action<Text2D> VerticalAlignmentChanged;
		ReferenceField<EVerticalAlignment> _verticalAlignment = EVerticalAlignment.Center;

		/// <summary>
		/// How far from the camera lines are visible.
		/// </summary>
		[DefaultValue( 50.0 )]
		[Range( 1, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[Category( "Displaying In Scene" )]
		public Reference<double> VisibilityDistance
		{
			get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
			set { if( _visibilityDistance.BeginSet( ref value ) ) { try { VisibilityDistanceChanged?.Invoke( this ); } finally { _visibilityDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		public event Action<Text2D> VisibilityDistanceChanged;
		ReferenceField<double> _visibilityDistance = 50.0;

		/// <summary>
		/// The radius of object bounding sphere to detect visibility.
		/// </summary>
		[DefaultValue( 3.0 )]
		[Range( 1.0, 10.0 )]
		[Category( "Displaying In Scene" )]
		public Reference<double> BoundingRadius
		{
			get { if( _boundingRadius.BeginGet() ) BoundingRadius = _boundingRadius.Get( this ); return _boundingRadius.value; }
			set { if( _boundingRadius.BeginSet( ref value ) ) { try { BoundingRadiusChanged?.Invoke( this ); SpaceBoundsUpdate(); } finally { _boundingRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BoundingRadius"/> property value changes.</summary>
		public event Action<Text2D> BoundingRadiusChanged;
		ReferenceField<double> _boundingRadius = 3.0;

		/// <summary>
		/// Whether the component is visible in the simulation mode.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Displaying In Scene" )]
		public Reference<bool> DisplayInSimulation
		{
			get { if( _displayInSimulation.BeginGet() ) DisplayInSimulation = _displayInSimulation.Get( this ); return _displayInSimulation.value; }
			set { if( _displayInSimulation.BeginSet( ref value ) ) { try { DisplayInSimulationChanged?.Invoke( this ); } finally { _displayInSimulation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayInSimulation"/> property value changes.</summary>
		public event Action<Text2D> DisplayInSimulationChanged;
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
				case nameof( TextHorizontalAlignment ):
				case nameof( VerticalIndention ):
					if( !Multiline )
						skip = true;
					break;

				case nameof( BackColor ):
				case nameof( BackSizeAdd ):
				case nameof( BackStyle ):
					if( !Back )
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

		public delegate void RenderBeforeDelegate( Text2D sender, ViewportRenderingContext context, Vector2 screenPosition, ref bool handled );
		public event RenderBeforeDelegate RenderBefore;

		public delegate void RenderAfterDelegate( Text2D sender, ViewportRenderingContext context, Vector2 screenPosition );
		public event RenderAfterDelegate RenderAfter;

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

		//render after PrepareListsOfObjects with sorting by camera distance from far to near
		void Render( ViewportRenderingContext context )
		{
			if( EngineApp.IsEditor || EngineApp.IsSimulation && DisplayInSimulation )
			{
				var cameraPosition = context.Owner.CameraSettings.Position;
				var sphere = new Sphere( cameraPosition, VisibilityDistance );

				if( SpaceBounds.BoundingSphere.Intersects( ref sphere ) )
				{
					if( context.Owner.CameraSettings.ProjectToScreenCoordinates( TransformV.Position, out var screenPosition ) )
					{
						bool handled = false;
						RenderBefore?.Invoke( this, context, screenPosition, ref handled );
						if( !handled )
						{
							Render( context, screenPosition );
							RenderAfter?.Invoke( this, context, screenPosition );
						}
					}
				}
			}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum && context.Owner.CanvasRenderer != null && context.Owner.Simple3DRenderer != null )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				//render after PrepareListsOfObjects with sorting by camera distance from far to near
				var item = new RenderingPipeline.RenderSceneData.ActionToDoAfterPrepareListsOfObjectsSortedByDistance();
				item.DistanceToCamera = (float)( Transform.Value.Position - context.Owner.CameraSettings.Position ).Length();
				item.Action = Render;
				context.FrameData.RenderSceneData.ActionsToDoAfterPrepareListsOfObjectsSortedByDistance.Add( item );
			}
		}

		public delegate void RenderBackBeforeDelegate( Text2D sender, ViewportRenderingContext context, Rectangle rectangle, ref bool handled );
		public event RenderBackBeforeDelegate RenderBackBefore;

		public delegate void RenderBackAfterDelegate( Text2D sender, ViewportRenderingContext context, Rectangle rectangle );
		public event RenderBackAfterDelegate RenderBackAfter;

		void RenderBackRectangle( ViewportRenderingContext context, Rectangle rectangle, ColorValue color )
		{
			var renderer = context.Owner.CanvasRenderer;

			switch( BackStyle.Value )
			{
			case BackStyleEnum.Rectangle:
				renderer.AddQuad( rectangle, color );
				break;

			case BackStyleEnum.RoundedRectangle:
				{
					var rect2 = rectangle.ToRectangleF();
					var fontSize = GetFontSizeScreen( context );
					var roundSize = new Vector2F( renderer.AspectRatioInv, 1 ) * (float)fontSize * 0.4f;

					int steps = 16;

					var list = new List<Vector2F>( steps * 4 );

					for( int n = 0; n < steps; n++ )
					{
						var v = (float)n / (float)( steps - 1 );
						var angle = v * MathEx.PI / 2;
						list.Add( rect2.LeftTop + new Vector2F( 1.0f - MathEx.Cos( angle ), 1.0f - MathEx.Sin( angle ) ) * roundSize );
					}

					for( int n = steps - 1; n >= 0; n-- )
					{
						var v = (float)n / (float)( steps - 1 );
						var angle = v * MathEx.PI / 2;
						list.Add( rect2.RightTop + new Vector2F( MathEx.Cos( angle ) - 1.0f, 1.0f - MathEx.Sin( angle ) ) * roundSize );
					}

					for( int n = 0; n < steps; n++ )
					{
						var v = (float)n / (float)( steps - 1 );
						var angle = v * MathEx.PI / 2;
						list.Add( rect2.RightBottom + new Vector2F( MathEx.Cos( angle ) - 1.0f, MathEx.Sin( angle ) - 1.0f ) * roundSize );
					}

					for( int n = steps - 1; n >= 0; n-- )
					{
						var v = (float)n / (float)( steps - 1 );
						var angle = v * MathEx.PI / 2;
						list.Add( rect2.LeftBottom + new Vector2F( 1.0f - MathEx.Cos( angle ), MathEx.Sin( angle ) - 1.0f ) * roundSize );
					}

					var vertices = new List<CanvasRenderer.TriangleVertex>( 1 + list.Count );
					var indices = new List<int>( list.Count * 3 );

					vertices.Add( new CanvasRenderer.TriangleVertex( rect2.GetCenter(), color ) );
					foreach( var v in list )
						vertices.Add( new CanvasRenderer.TriangleVertex( v, color ) );

					for( int n = 0; n < list.Count; n++ )
					{
						indices.Add( 0 );
						indices.Add( 1 + n );
						indices.Add( 1 + ( n + 1 ) % list.Count );
					}

					renderer.AddTriangles( vertices, indices );
				}
				break;
			}
		}

		protected virtual void RenderBack( ViewportRenderingContext context, Rectangle rectangle )
		{
			var context2 = context.ObjectInSpaceRenderingContext;

			RenderBackRectangle( context, rectangle, BackColor );

			//selection rectangle
			if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
			{
				ColorValue color;
				if( context2.selectedObjects.Contains( this ) )
					color = ProjectSettings.Get.Colors.SelectedColor;
				else
					color = ProjectSettings.Get.Colors.CanSelectColor;

				color.Alpha *= 0.5f;

				RenderBackRectangle( context, rectangle, color );
			}
		}

		void RenderBack2( ViewportRenderingContext context, Rectangle rectangle )
		{
			var renderer = context.Owner.CanvasRenderer;
			var context2 = context.ObjectInSpaceRenderingContext;
			var fontSize = GetFontSizeScreen( context );

			//calculate rectangle
			var rect2 = rectangle;
			rect2.Expand( new Vector2( renderer.AspectRatioInv, 1 ) * fontSize * BackSizeAdd.Value );

			//render
			if( Back && BackColor.Value.Alpha > 0 || context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
			{
				var handled = false;
				RenderBackBefore?.Invoke( this, context, rect2, ref handled );
				if( !handled )
				{
					RenderBack( context, rect2 );
					RenderBackAfter?.Invoke( this, context, rect2 );
				}
			}

			//modify selection label
			if( EngineApp.IsEditor )
			{
				var item = context2.viewport.GetLastFrameScreenLabelByObjectInSpace( this );
				if( item != null )
				{
					item.ScreenRectangle = rect2;
					item.Color = ColorValue.Zero;
					item.Shape = Viewport.LastFrameScreenLabelItem.ShapeEnum.Rectangle;
				}
			}
		}

		void RenderMultilineDisabled( ViewportRenderingContext context, Vector2 screenPosition )
		{
			var text = Text.Value.Replace( '\n', ' ' ).Replace( "\r", "" );

			var renderer = context.Owner.CanvasRenderer;
			var fontSize = GetFontSizeScreen( context );
			var options = PixelAlign.Value ? CanvasRenderer.AddTextOptions.PixelAlign : 0;

			var font = Font.Value;
			if( font == null )
				font = renderer.DefaultFont;
			if( font == null || font.Disposed )
				return;

			//Back
			{
				var length = font.GetTextLength( fontSize, renderer, text );
				var offset = new Vector2( length, fontSize ) * 0.5;

				var pos = screenPosition;

				switch( HorizontalAlignment.Value )
				{
				case EHorizontalAlignment.Left:
				case EHorizontalAlignment.Stretch:
					pos.X += length * 0.5;
					break;
				case EHorizontalAlignment.Right:
					pos.X -= length * 0.5;
					break;
				}

				switch( VerticalAlignment.Value )
				{
				case EVerticalAlignment.Top:
				case EVerticalAlignment.Stretch:
					pos.Y += fontSize * 0.5;
					break;
				case EVerticalAlignment.Bottom:
					pos.Y -= fontSize * 0.5;
					break;
				}

				var rect = new Rectangle( pos - offset, pos + offset );
				RenderBack2( context, rect );
			}

			if( Shadow )
			{
				ColorValue color = ShadowColor.Value.GetSaturate();
				renderer.AddText( font, fontSize, text,
					screenPosition + ConvertOffsetToScreen( context, ShadowOffset ),
					HorizontalAlignment, VerticalAlignment, color, options );
			}

			renderer.AddText( font, fontSize, text, screenPosition, HorizontalAlignment, VerticalAlignment, Color, options );
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
				var strLines = text.Split( new char[] { '\n' }, StringSplitOptions.None );
				for( int n = 0; n < strLines.Length; n++ )
				{
					var alignByWidth = TextHorizontalAlignment.Value == EHorizontalAlignment.Stretch && n != strLines.Length - 1;
					lines.Add( new LineItem( strLines[ n ].Trim( '\r' ), alignByWidth ) );
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

				double height = fontSize * (double)lines.Count + screenVerticalIndention * ( (double)lines.Count - 1 );

				Rectangle rect;
				{
					var pos = screenPosition;

					switch( HorizontalAlignment.Value )
					{
					case EHorizontalAlignment.Center:
						pos.X -= screenSize * 0.5;
						break;
					case EHorizontalAlignment.Right:
						pos.X -= screenSize;
						break;
					}

					switch( VerticalAlignment.Value )
					{
					case EVerticalAlignment.Center:
						pos.Y -= height * 0.5;
						break;
					case EVerticalAlignment.Bottom:
						pos.Y -= height;
						break;
					}

					rect = new Rectangle( pos, pos + new Vector2( screenSize, height ) );
				}

				double stepY = fontSize + screenVerticalIndention;

				//Back
				RenderBack2( context, rect );

				//text
				for( int nStep = Shadow ? 0 : 1; nStep < 2; nStep++ )
				{
					double positionY = rect.Top;//startY;
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

							double posX = rect.Left;
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
							switch( TextHorizontalAlignment.Value )
							{
							case EHorizontalAlignment.Left:
							case EHorizontalAlignment.Stretch:
								positionX = rect.Left;
								break;
							case EHorizontalAlignment.Center:
								horizontalAlign = EHorizontalAlignment.Center;
								positionX = rect.GetCenter().X;
								break;
							case EHorizontalAlignment.Right:
								horizontalAlign = EHorizontalAlignment.Right;
								positionX = rect.Right;
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

		double GetParentContainerPixelScale( ViewportRenderingContext context )
		{
			var container = context.Owner?.UIContainer;
			if( container != null )
				return container.GetParentContainerPixelScale();
			else
				return 1;
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
			case UIMeasure.PixelsScaled:
				screen = DivideWithZeroCheck( value.Value * GetParentContainerPixelScale( context ), GetParentContainerSizeInPixels( context ) );
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
