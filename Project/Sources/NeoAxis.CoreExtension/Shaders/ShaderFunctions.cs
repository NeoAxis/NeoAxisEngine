// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis
{
	public static class ShaderFunctions
	{
		//Shader file: Assets\Base\Shaders\FragmentFunctions.sh
		[ShaderGenerationFunction( "dither(getFragCoord(), {value})" )]
		public static double Dither( double value )
		{
			return 0;
		}
	}
}
