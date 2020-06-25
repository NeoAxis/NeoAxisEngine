// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace NeoAxis
{
	struct OgreWrapper
	{
		public const string library = "NeoAxisCoreNative";
		public const CallingConvention convention = CallingConvention.Cdecl;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	struct OgreNativeWrapper
	{
		[DllImport( OgreWrapper.library, EntryPoint = "OgreNativeWrapper_CheckNativeBridge", CallingConvention = OgreWrapper.convention )]
		public unsafe static extern void CheckNativeBridge( int parameterTypeTextureCubeValue );

		[DllImport( OgreWrapper.library, EntryPoint = "OgreNativeWrapper_FreeOutString", CallingConvention = OgreWrapper.convention ), SuppressUnmanagedCodeSecurity]
		public unsafe static extern void FreeOutString( IntPtr pointer );

		public static string GetOutString( IntPtr pointer )
		{
			if( pointer != IntPtr.Zero )
			{
				string result = Marshal.PtrToStringUni( pointer );
				FreeOutString( pointer );
				return result;
			}
			else
				return null;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	[StructLayout( LayoutKind.Sequential )]
	struct OgreQuaternion
	{
		float w;
		float x;
		float y;
		float z;

		//

		public static readonly OgreQuaternion Identity = new OgreQuaternion( 1.0f, 0.0f, 0.0f, 0.0f );

		public OgreQuaternion( float w, float x, float y, float z )
		{
			this.w = w;
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static void Convert( ref QuaternionF quat, out OgreQuaternion result )
		{
			result.x = quat.X;
			result.y = quat.Y;
			result.z = quat.Z;
			result.w = quat.W;
		}

		public static void Convert( ref OgreQuaternion quat, out QuaternionF result )
		{
			result = new QuaternionF( quat.x, quat.y, quat.z, quat.w );
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	[StructLayout( LayoutKind.Sequential )]
	struct OgreMatrix3
	{
		internal Vector3F mat0;
		internal Vector3F mat1;
		internal Vector3F mat2;

		//

		public void Transpose()
		{
			float v;
			v = mat0.Y; mat0.Y = mat1.X; mat1.X = v;
			v = mat0.Z; mat0.Z = mat2.X; mat2.X = v;
			v = mat1.Z; mat1.Z = mat2.Y; mat2.Y = v;
		}

		public static void Convert( ref Matrix3F mat, out OgreMatrix3 result )
		{
			result.mat0 = mat.Item0;
			result.mat1 = mat.Item1;
			result.mat2 = mat.Item2;
			result.Transpose();
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	[StructLayout( LayoutKind.Sequential )]
	struct OgreMatrix4
	{
		internal Vector4F mat0;
		internal Vector4F mat1;
		internal Vector4F mat2;
		internal Vector4F mat3;

		//

		public static readonly OgreMatrix4 Identity =
			new OgreMatrix4( 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 );

		//

		public OgreMatrix4( float xx, float xy, float xz, float xw,
			float yx, float yy, float yz, float yw,
			float zx, float zy, float zz, float zw,
			float wx, float wy, float wz, float ww )
		{
			mat0 = new Vector4F( xx, xy, xz, xw );
			mat1 = new Vector4F( yx, yy, yz, yw );
			mat2 = new Vector4F( zx, zy, zz, zw );
			mat3 = new Vector4F( wx, wy, wz, ww );
		}

		public void Transpose()
		{
			float v;
			v = mat0.Y; mat0.Y = mat1.X; mat1.X = v;
			v = mat0.Z; mat0.Z = mat2.X; mat2.X = v;
			v = mat0.W; mat0.W = mat3.X; mat3.X = v;
			v = mat1.Z; mat1.Z = mat2.Y; mat2.Y = v;
			v = mat1.W; mat1.W = mat3.Y; mat3.Y = v;
			v = mat2.W; mat2.W = mat3.Z; mat3.Z = v;
		}

		public static void Convert( ref Matrix4F mat, out OgreMatrix4 result )
		{
			result.mat0 = mat.Item0;
			result.mat1 = mat.Item1;
			result.mat2 = mat.Item2;
			result.mat3 = mat.Item3;
			result.Transpose();
		}

		//public static void Convert( ref OgreMatrix4 mat, out Mat4 result )
		//{
		//   result = new Mat4( mat.mat0, mat.mat1, mat.mat2, mat.mat3 );
		//   result.Transpose();
		//}

		public static unsafe void Convert( OgreMatrix4* mat, out Matrix4F result )
		{
			result = new Matrix4F( mat->mat0, mat->mat1, mat->mat2, mat->mat3 );
			result.Transpose();
		}
	}

}
