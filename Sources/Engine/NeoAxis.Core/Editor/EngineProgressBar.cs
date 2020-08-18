// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace NeoAxis.Editor
{
	public class EngineProgressBar : ProgressBar
	{
		public EngineProgressBar()
		{
			SetStyle( ControlStyles.UserPaint, true );
		}

		protected override void OnPaint( PaintEventArgs e )
		{
#if !DEPLOY

			System.Drawing.Rectangle r = e.ClipRectangle;

			r.Width = (int)( r.Width * ( (double)Value / Maximum ) ) - 4;

			e.Graphics.FillRectangle( new SolidBrush( EditorAPI.DarkTheme ? Color.FromArgb( 40, 40, 40 ) : Color.FromArgb( 230, 230, 230 ) ), e.ClipRectangle );

			var r2 = e.ClipRectangle;
			r2.Width--;
			r2.Height--;

			e.Graphics.DrawRectangle( new Pen( EditorAPI.DarkTheme ? Color.FromArgb( 80, 80, 80 ) : Color.FromArgb( 188, 188, 188 ) ), r2 );

			//if( ProgressBarRenderer.IsSupported )
			//	ProgressBarRenderer.DrawHorizontalBar( e.Graphics, e.ClipRectangle );

			r.Height = r.Height - 4;
			e.Graphics.FillRectangle( new SolidBrush( EditorAPI.DarkTheme ? Color.FromArgb( 0, 150, 0 ) : Color.FromArgb( 0, 190, 0 ) ), 2, 2, r.Width, r.Height );

			//r.Height = r.Height - 2;
			//e.Graphics.FillRectangle( new SolidBrush( Color.FromArgb( 70, 150, 0 ) ), 1, 1, r.Width, r.Height );

			//rec.Width = (int)( rec.Width * ( (double)Value / Maximum ) ) - 4;
			//if( ProgressBarRenderer.IsSupported )
			//	ProgressBarRenderer.DrawHorizontalBar( e.Graphics, e.ClipRectangle );
			//rec.Height = rec.Height - 4;
			//e.Graphics.FillRectangle( new SolidBrush( Color.FromArgb( 70, 170, 0 ) ), 2, 2, rec.Width, rec.Height );

#endif
		}
	}
}
