// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//!!!!доделать до полноценного как Matrix4F

	/// <summary>
	/// A structure encapsulating a single precision 3x4 matrix.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Matrix3x4F
	{
		[Browsable( false )]
		public Vector4F Item0;
		[Browsable( false )]
		public Vector4F Item1;
		[Browsable( false )]
		public Vector4F Item2;

		//

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3F GetTranslation()
		{
			return new Vector3F( Item0.W, Item1.W, Item2.W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetTranslation( out Vector3F result )
		{
			result.X = Item0.W;
			result.Y = Item1.W;
			result.Z = Item2.W;
		}
	}
}
