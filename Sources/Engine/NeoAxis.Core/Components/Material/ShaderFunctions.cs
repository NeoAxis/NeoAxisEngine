// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Provides shader functions to use in flow graphs for visual adjustment of materials and effects.
	/// </summary>
	public static class ShaderFunctions
	{
		[ShaderGenerationFunction( "uvTransform({translation},{rotation},{scale},{texCoord})" )]
		public static Vector2 UVTransform( Vector2 translation, float rotation, Vector2 scale, Vector2 texCoord )
		{
			return Vector2.Zero;
		}
	}
}
