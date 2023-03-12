// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/*public */static class RendererUtility
	{
		//!!!!по сути времнно пока нет корректной толщины

		public static void AddLineSegmented( Simple3DRenderer renderer, Vector3 start, Vector3 end, int steps = -1 )
		{
			//draw line segments so that there is no problem with the thickness of the lines
			var ray = new Ray( start, end - start );

			int steps2 = steps;
			if( steps2 < 0 )
				steps2 = (int)MathEx.Lerp( 2, 10, MathEx.Saturate( Math.Pow( ray.Direction.Length() / 100, 1.3 ) ) );

			for( int n = 0; n < steps2; n++ )
			{
				var p0 = ray.GetPointOnRay( (double)n / steps2 );
				var p1 = ray.GetPointOnRay( (double)( n + 1 ) / steps2 );
				renderer.AddLine( p0, p1 );
			}
		}

		public static void AddBoundsSegmented( Simple3DRenderer renderer, Bounds bounds, int steps = -1 )
		{
			ref Vector3 bmin = ref bounds.Minimum;
			ref Vector3 bmax = ref bounds.Maximum;

			//var lineThickness2 = lineThickness;

			////optimization
			//if( lineThickness2 == 0 )
			//{
			//	bounds.GetCenter( out var center );
			//	if( center.X - bmin.X < fastLineThicknessThreshold && center.Y - bmin.Y < fastLineThicknessThreshold && center.Z - bmin.Z < fastLineThicknessThreshold )
			//		lineThickness2 = GetThicknessByPixelSize( ref center, ProjectSettings.Get.LineThickness );
			//}

			AddLineSegmented( renderer, new Vector3( bmin.X, bmin.Y, bmin.Z ), new Vector3( bmax.X, bmin.Y, bmin.Z ) );
			AddLineSegmented( renderer, new Vector3( bmax.X, bmin.Y, bmin.Z ), new Vector3( bmax.X, bmax.Y, bmin.Z ) );
			AddLineSegmented( renderer, new Vector3( bmax.X, bmax.Y, bmin.Z ), new Vector3( bmin.X, bmax.Y, bmin.Z ) );
			AddLineSegmented( renderer, new Vector3( bmin.X, bmax.Y, bmin.Z ), new Vector3( bmin.X, bmin.Y, bmin.Z ) );

			AddLineSegmented( renderer, new Vector3( bmin.X, bmin.Y, bmax.Z ), new Vector3( bmax.X, bmin.Y, bmax.Z ) );
			AddLineSegmented( renderer, new Vector3( bmax.X, bmin.Y, bmax.Z ), new Vector3( bmax.X, bmax.Y, bmax.Z ) );
			AddLineSegmented( renderer, new Vector3( bmax.X, bmax.Y, bmax.Z ), new Vector3( bmin.X, bmax.Y, bmax.Z ) );
			AddLineSegmented( renderer, new Vector3( bmin.X, bmax.Y, bmax.Z ), new Vector3( bmin.X, bmin.Y, bmax.Z ) );

			AddLineSegmented( renderer, new Vector3( bmin.X, bmin.Y, bmin.Z ), new Vector3( bmin.X, bmin.Y, bmax.Z ) );
			AddLineSegmented( renderer, new Vector3( bmax.X, bmin.Y, bmin.Z ), new Vector3( bmax.X, bmin.Y, bmax.Z ) );
			AddLineSegmented( renderer, new Vector3( bmax.X, bmax.Y, bmin.Z ), new Vector3( bmax.X, bmax.Y, bmax.Z ) );
			AddLineSegmented( renderer, new Vector3( bmin.X, bmax.Y, bmin.Z ), new Vector3( bmin.X, bmax.Y, bmax.Z ) );
		}
	}
}
