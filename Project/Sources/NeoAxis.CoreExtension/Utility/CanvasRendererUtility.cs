// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// This class contains auxiliary functional for the <see cref="CanvasRenderer"/>.
	/// </summary>
	public static class CanvasRendererUtility
	{
		public static Vector2 ShadowOffsetInPixels = new Vector2( 1, 1 );

		//

		public static void AddTextWithShadow( Viewport viewport, FontComponent font, double fontSize, string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			var renderer = viewport.CanvasRenderer;

			if( font == null || font.Disposed )
				font = renderer.DefaultFont;
			if( font == null || font.Disposed )
				return;
			if( fontSize < 0 )
				fontSize = renderer.DefaultFontSize;

			Vector2 shadowOffset = ShadowOffsetInPixels / viewport.SizeInPixels.ToVector2();
			renderer.AddText( font, fontSize, text, position + shadowOffset, horizontalAlign, verticalAlign, new ColorValue( 0, 0, 0, color.Alpha / 2 ) );
			renderer.AddText( font, fontSize, text, position, horizontalAlign, verticalAlign, color );
		}

		public static void AddTextWithShadow( Viewport viewport, string text, Vector2 position, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			AddTextWithShadow( viewport, null, -1, text, position, horizontalAlign, verticalAlign, color );
		}

		public static void AddTextLinesWithShadow( Viewport viewport, FontComponent font, double fontSize, IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			if( lines.Count == 0 )
				return;

			var renderer = viewport.CanvasRenderer;

			if( font == null || font.Disposed )
				font = renderer.DefaultFont;
			if( font == null || font.Disposed )
				return;
			if( fontSize < 0 )
				fontSize = renderer.DefaultFontSize;

			Vector2 shadowOffset = ShadowOffsetInPixels / viewport.SizeInPixels.ToVector2();
			float linesHeight = (float)lines.Count * (float)fontSize;

			double posY = 0;
			switch( verticalAlign )
			{
			case EVerticalAlignment.Top:
				posY = rectangle.Top;
				break;
			case EVerticalAlignment.Center:
				posY = rectangle.Top + ( rectangle.Size.Y - linesHeight ) / 2;
				break;
			case EVerticalAlignment.Bottom:
				posY = rectangle.Bottom - linesHeight;
				break;
			}

			for( int n = 0; n < lines.Count; n++ )
			{
				string line = lines[ n ];

				double posX = 0;
				switch( horizontalAlign )
				{
				case EHorizontalAlignment.Left:
					posX = rectangle.Left;
					break;
				case EHorizontalAlignment.Center:
					posX = rectangle.Left + ( rectangle.Size.X - font.GetTextLength( fontSize, renderer, line ) ) / 2;
					break;
				case EHorizontalAlignment.Right:
					posX = rectangle.Right - font.GetTextLength( fontSize, renderer, line );
					break;
				}

				Vector2 position = new Vector2( posX, posY );

				renderer.AddText( font, fontSize, line, position + shadowOffset, EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 0, 0, 0, color.Alpha / 2 ) );
				renderer.AddText( font, fontSize, line, position, EHorizontalAlignment.Left, EVerticalAlignment.Top, color );
				posY += fontSize;
			}
		}

		public static void AddTextLinesWithShadow( Viewport viewport, IList<string> lines, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			AddTextLinesWithShadow( viewport, null, -1, lines, rectangle, horizontalAlign, verticalAlign, color );
		}

		public static int AddTextWordWrapWithShadow( Viewport viewport, FontComponent font, double fontSize, string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			var renderer = viewport.CanvasRenderer;

			if( font == null || font.Disposed )
				font = renderer.DefaultFont;
			if( font == null || font.Disposed )
				return 0;
			if( fontSize < 0 )
				fontSize = renderer.DefaultFontSize;

			var items = font.GetWordWrapLines( fontSize, renderer, text, rectangle.Size.X );

			string[] lines = new string[ items.Length ];
			for( int n = 0; n < lines.Length; n++ )
				lines[ n ] = items[ n ].Text;

			AddTextLinesWithShadow( viewport, font, fontSize, lines, rectangle, horizontalAlign, verticalAlign, color );

			return lines.Length;
		}

		public static int AddTextWordWrapWithShadow( Viewport viewport, string text, Rectangle rectangle, EHorizontalAlignment horizontalAlign, EVerticalAlignment verticalAlign, ColorValue color )
		{
			return AddTextWordWrapWithShadow( viewport, null, -1, text, rectangle, horizontalAlign, verticalAlign, color );
		}
	}
}
