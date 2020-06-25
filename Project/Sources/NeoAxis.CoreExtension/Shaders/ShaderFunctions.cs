// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis
{
	public static class ShaderFunctions
	{
		//Shader file: Assets\Base\Shaders\FragmentFunctions.sh
		[ShaderGenerationFunction( "dither(gl_FragCoord, {value})" )]
		public static double Dither( double value )
		{
			return 0;
		}
	}
}
