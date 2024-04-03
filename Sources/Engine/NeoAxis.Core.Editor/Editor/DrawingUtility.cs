// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Drawing;

namespace NeoAxis.Editor
{
	static class DrawingUtility
	{
		public static ColorValue ToColorValue( Color source )
		{
			ColorValue v;
			v.Red = (float)source.R / 255;
			v.Green = (float)source.G / 255;
			v.Blue = (float)source.B / 255;
			v.Alpha = (float)source.A / 255;
			return v;
		}

		public static Color ToColor( ColorValue v )
		{
			return Color.FromArgb(
				(int)MathEx.Clamp( v.Alpha * 255, 0, 255 ),
				(int)MathEx.Clamp( v.Red * 255, 0, 255 ),
				(int)MathEx.Clamp( v.Green * 255, 0, 255 ),
				(int)MathEx.Clamp( v.Blue * 255, 0, 255 ) );
		}
	}
}
