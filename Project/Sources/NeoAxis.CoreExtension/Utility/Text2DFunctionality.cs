// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace Project
{
	/// <summary>
	/// An auxiliary class to draw text on the screen. Similar to Text2D component.
	/// </summary>
	public class Text2DFunctionality
	{
		public string Text = "";
		public bool Multiline = false;
		public EHorizontalAlignment TextHorizontalAlignment = EHorizontalAlignment.Center;
		public UIMeasureValueDouble VerticalIndention = new UIMeasureValueDouble( UIMeasure.Units, 0 );
		public FontComponent Font = null;
		public UIMeasureValueDouble FontSize = new UIMeasureValueDouble( UIMeasure.Screen, 0.025 );
		public ColorValue Color = ColorValue.One;
		public bool PixelAlign = true;

		public bool Shadow = true;
		public UIMeasureValueVector2 ShadowOffset = new UIMeasureValueVector2( UIMeasure.Pixels, 2, 2 );
		public ColorValue ShadowColor = new ColorValue( 0, 0, 0, 0.7 );

		public bool Back = true;
		public ColorValue BackColor = new ColorValue( 0.7, 0.7, 0.7 );
		public Vector2 BackSizeAdd = new Vector2( 0.3, 0.3 );
		public BackStyleEnum BackStyle = BackStyleEnum.RoundedRectangle;

		public EHorizontalAlignment HorizontalAlignment = EHorizontalAlignment.Center;
		public EVerticalAlignment VerticalAlignment = EVerticalAlignment.Center;

		///////////////////////////////////////////////

		public enum BackStyleEnum
		{
			Rectangle,
			RoundedRectangle,
		}

		///////////////////////////////////////////////

		public void Render( ViewportRenderingContext context, Vector2 screenPosition )
		{
			if( !string.IsNullOrEmpty( Text ) )
			{
				if( Multiline )
					RenderMultilineEnabled( context, screenPosition );
				else
					RenderMultilineDisabled( context, screenPosition );
			}
		}

		void RenderBackRectangle( ViewportRenderingContext context, Rectangle rectangle, ColorValue color )
		{
			var renderer = context.Owner.CanvasRenderer;

			switch( BackStyle )
			{
			case BackStyleEnum.Rectangle:
				renderer.AddQuad( rectangle, color );
				break;

			case BackStyleEnum.RoundedRectangle:
				{
					var fontSize = GetFontSizeScreen( context );
					renderer.AddRoundedQuad( rectangle, fontSize * 0.4f, CanvasRenderer.AddRoundedQuadMode.Antialiasing, color );
				}
				break;
			}
		}

		void RenderBack( ViewportRenderingContext context, Rectangle rectangle )
		{
			RenderBackRectangle( context, rectangle, BackColor );
		}

		void RenderBack2( ViewportRenderingContext context, Rectangle rectangle )
		{
			var renderer = context.Owner.CanvasRenderer;
			var fontSize = GetFontSizeScreen( context );

			//calculate rectangle
			var rect2 = rectangle;
			rect2.Expand( new Vector2( renderer.AspectRatioInv, 1 ) * fontSize * BackSizeAdd );

			//render
			if( Back && BackColor.Alpha > 0 )
				RenderBack( context, rect2 );
		}

		void RenderMultilineDisabled( ViewportRenderingContext context, Vector2 screenPosition )
		{
			var text = Text.Replace( '\n', ' ' ).Replace( "\r", "" );

			var renderer = context.Owner.CanvasRenderer;
			var fontSize = GetFontSizeScreen( context );
			var options = PixelAlign ? CanvasRenderer.AddTextOptions.PixelAlign : 0;

			var font = Font;
			if( font == null )
				font = renderer.DefaultFont;
			if( font == null || font.Disposed )
				return;

			//Back
			{
				var length = font.GetTextLength( fontSize, renderer, text );
				var offset = new Vector2( length, fontSize ) * 0.5;

				var pos = screenPosition;

				switch( HorizontalAlignment )
				{
				case EHorizontalAlignment.Left:
				case EHorizontalAlignment.Stretch:
					pos.X += length * 0.5;
					break;
				case EHorizontalAlignment.Right:
					pos.X -= length * 0.5;
					break;
				}

				switch( VerticalAlignment )
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
				var color = ShadowColor * new ColorValue( 1, 1, 1, Color.Alpha );
				//var color = ShadowColor.GetSaturate() * new ColorValue( 1, 1, 1, Color.Alpha );
				renderer.AddText( font, fontSize, text, screenPosition + ConvertOffsetToScreen( context, ShadowOffset ), HorizontalAlignment, VerticalAlignment, color, options );
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
			var text = Text;
			var options = PixelAlign ? CanvasRenderer.AddTextOptions.PixelAlign : 0;

			var verticalIndention = VerticalIndention;
			var screenVerticalIndention = ConvertOffsetToScreen( context, new UIMeasureValueVector2( verticalIndention.Measure, 0, verticalIndention.Value ) ).Y;

			var font = Font;
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
					var alignByWidth = TextHorizontalAlignment == EHorizontalAlignment.Stretch && n != strLines.Length - 1;
					lines.Add( new LineItem( strLines[ n ].Trim( '\r' ), alignByWidth ) );
				}
			}

			if( lines.Count != 0 )
			{
				var shadowColor = ShadowColor * new ColorValue( 1, 1, 1, Color.Alpha );
				var textColor = Color;
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

					switch( HorizontalAlignment )
					{
					case EHorizontalAlignment.Center:
						pos.X -= screenSize * 0.5;
						break;
					case EHorizontalAlignment.Right:
						pos.X -= screenSize;
						break;
					}

					switch( VerticalAlignment )
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
							switch( TextHorizontalAlignment )
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
				screen = value.Value;
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
			var value = FontSize;
			var fontSize = ConvertOffsetToScreen( context, new UIMeasureValueVector2( value.Measure, 0, value.Value ) ).Y;
			return fontSize;
		}
	}
}