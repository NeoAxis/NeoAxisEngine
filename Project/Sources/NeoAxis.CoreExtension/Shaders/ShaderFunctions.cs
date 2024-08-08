// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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

		//Vehicle specific
		[ShaderGenerationFunction( "vehicleLamps( {mask}, {emission}, {instanceParameter1}, {instanceParameter2}, {texCoord0} )" )]
		public static Vector3 VehicleLamps( Vector3 mask, Vector3 emission, Vector4 instanceParameter1, Vector4 instanceParameter2, Vector2 texCoord0 )
		{
			return Vector3.Zero;
		}
	}
}
