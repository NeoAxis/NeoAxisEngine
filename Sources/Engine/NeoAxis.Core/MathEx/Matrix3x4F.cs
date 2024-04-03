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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void ConstructTransposeMatrix3( ref Matrix3F matrix3, ref Vector3F translation, out Matrix3x4F result )
		{
			result.Item0.X = matrix3.Item0.X;
			result.Item0.Y = matrix3.Item1.X;
			result.Item0.Z = matrix3.Item2.X;
			result.Item0.W = translation.X;

			result.Item1.X = matrix3.Item0.Y;
			result.Item1.Y = matrix3.Item1.Y;
			result.Item1.Z = matrix3.Item2.Y;
			result.Item1.W = translation.Y;

			result.Item2.X = matrix3.Item0.Z;
			result.Item2.Y = matrix3.Item1.Z;
			result.Item2.Z = matrix3.Item2.Z;
			result.Item2.W = translation.Z;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool DecomposeScale( out Vector3F scale )
		{
			//Source: Unknown
			//References: http://www.gamedev.net/community/forums/topic.asp?topic_id=441695

			//Scaling is the length of the rows.
			scale = new Vector3F(
				(float)Math.Sqrt( ( Item0.X * Item0.X ) + ( Item0.Y * Item0.Y ) + ( Item0.Z * Item0.Z ) ),
				(float)Math.Sqrt( ( Item1.X * Item1.X ) + ( Item1.Y * Item1.Y ) + ( Item1.Z * Item1.Z ) ),
				(float)Math.Sqrt( ( Item2.X * Item2.X ) + ( Item2.Y * Item2.Y ) + ( Item2.Z * Item2.Z ) ) );

			//!!!!1e-6f?
			const float zeroTolerance = 1e-6f;
			//If any of the scaling factors are zero, than the rotation matrix can not exist.
			if( Math.Abs( scale.X ) < zeroTolerance ||
				Math.Abs( scale.Y ) < zeroTolerance ||
				Math.Abs( scale.Z ) < zeroTolerance )
			{
				return false;
			}

			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public float DecomposeScaleMaxComponent()
		{
			var scaleSquared = new Vector3F(
				 Item0.X * Item0.X + Item0.Y * Item0.Y + Item0.Z * Item0.Z,
				 Item1.X * Item1.X + Item1.Y * Item1.Y + Item1.Z * Item1.Z,
				 Item2.X * Item2.X + Item2.Y * Item2.Y + Item2.Z * Item2.Z );

			return MathEx.Sqrt( scaleSquared.MaxComponent() );
		}
	}
}
