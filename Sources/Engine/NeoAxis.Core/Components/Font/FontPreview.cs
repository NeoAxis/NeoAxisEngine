#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class FontPreview : PreviewControlWithViewport
	{
		public FontPreview()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			CreateScene( true );
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			var renderer = viewport.CanvasRenderer;
			var font = ObjectOfPreview as FontComponent;

			var corverOffset = new Vector2( 0.1 * Viewport.CanvasRenderer.AspectRatioInv * 0.5, 0.1 * 0.3 );

			var positionY = corverOffset.Y;

			for( int n = 0; n < 3; n++ )
			{
				var fontSize = 0.1 * (double)( n + 1 );
				var text = "Font preview. 0 1 2 3 4 5 6 7 8 9.";

				renderer.AddLine( new Vector2( 0, positionY ), new Vector2( 1, positionY ), new ColorValue( 1, 1, 1, 0.5 ) );
				renderer.AddLine( new Vector2( 0, positionY + fontSize ), new Vector2( 1, positionY + fontSize ), new ColorValue( 1, 1, 1, 0.5 ) );

				renderer.AddText( font, fontSize, text, new Vector2( corverOffset.X, positionY ), EHorizontalAlignment.Left, EVerticalAlignment.Top, new ColorValue( 1, 1, 1 ) );

				positionY += fontSize + corverOffset.Y;
			}
		}
	}
}
#endif