// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public class HCColorPreviewButton : KryptonButton
	{
		public HCColorPreviewButton()
		{
			Text = "";
		}

		ColorValue previewColor;

		public ColorValue PreviewColor
		{
			get
			{
				return previewColor;
			}
			set
			{
				if( previewColor == value )
					return;
				previewColor = value;
				Invalidate();
			}
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			var rect = DisplayRectangle;
			rect.Inflate( -1, -1 );

			int[] color = new int[ 4 ];
			for( int n = 0; n < 4; n++ )
			{
				int c = (int)( PreviewColor[ n ] * 255.0f );
				if( c < 0 )
					c = 0;
				if( c > 255 )
					c = 255;
				color[ n ] = c;
			}

#if !ANDROID
			if( PreviewColor.Alpha != 1 )
			{
				using( HatchBrush brush = new HatchBrush( HatchStyle.LargeCheckerBoard,
					Color.FromArgb( 128, 128, 128 ), Color.FromArgb( 192, 192, 192 ) ) )
				{
					e.Graphics.FillRectangle( brush, rect );
				}
			}

			Color topColor = Color.FromArgb( 255, color[ 0 ], color[ 1 ], color[ 2 ] );
			Color bottomColor = Color.FromArgb( color[ 3 ], color[ 0 ], color[ 1 ], color[ 2 ] );

			using( LinearGradientBrush brush = new LinearGradientBrush( rect, topColor, bottomColor, 90, false ) )
			{
				e.Graphics.FillRectangle( brush, rect );
			}
#endif //!ANDROID
		}
	}
}
