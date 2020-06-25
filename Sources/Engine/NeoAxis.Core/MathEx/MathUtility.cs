// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary class for working with mathematical types.
	/// </summary>
	public static class MathUtility
	{
		public static Vector2F[] ToVector2FArray( this Vector2[] source )
		{
			var result = new Vector2F[ source.Length ];
			for( int n = 0; n < source.Length; n++ )
				result[ n ] = source[ n ].ToVector2F();
			return result;
		}

		public static Vector2[] ToVector2Array( this Vector2F[] source )
		{
			var result = new Vector2[ source.Length ];
			for( int n = 0; n < source.Length; n++ )
				result[ n ] = source[ n ].ToVector2();
			return result;
		}

		public static Vector3F[] ToVector3FArray( this Vector3[] source )
		{
			var result = new Vector3F[ source.Length ];
			for( int n = 0; n < source.Length; n++ )
				result[ n ] = source[ n ].ToVector3F();
			return result;
		}

		public static Vector3[] ToVector3Array( this Vector3F[] source )
		{
			var result = new Vector3[ source.Length ];
			for( int n = 0; n < source.Length; n++ )
				result[ n ] = source[ n ].ToVector3();
			return result;
		}

		public static Vector4F[] ToVector4FArray( this Vector4[] source )
		{
			var result = new Vector4F[ source.Length ];
			for( int n = 0; n < source.Length; n++ )
				result[ n ] = source[ n ].ToVector4F();
			return result;
		}

		public static Vector4[] ToVector4Array( this Vector4F[] source )
		{
			var result = new Vector4[ source.Length ];
			for( int n = 0; n < source.Length; n++ )
				result[ n ] = source[ n ].ToVector4();
			return result;
		}
	}
}
