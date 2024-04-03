// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( _QuatAsAnglesDConverter ) )]
	/// <summary>
	/// Represents a double precision four-dimensional mathematical quaternion.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Quaternion
	{
		public double X;
		public double Y;
		public double Z;
		public double W;

		public static readonly Quaternion Identity = new Quaternion( 0.0, 0.0, 0.0, 1.0 );
		public const string IdentityAsString = "0 0 0 1";
		public static readonly Quaternion Zero = new Quaternion( 0.0, 0.0, 0.0, 0.0 );
		public const string ZeroAsString = "0 0 0 0";

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Quaternion( Vector3 v, double w )
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = v.Z;
			this.W = w;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Quaternion( double x, double y, double z, double w )
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Quaternion( Quaternion source )
		{
			this.X = source.X;
			this.Y = source.Y;
			this.Z = source.Z;
			this.W = source.W;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Quaternion( QuaternionF source )
		{
			this.X = source.X;
			this.Y = source.Y;
			this.Z = source.Z;
			this.W = source.W;
		}

		[AutoConvertType]
		public static Quaternion Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			//parse from Angles (special case).
			if( vals.Length == 3 )
				return Angles.Parse( text ).ToQuaternion();

			if( vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (x y z w).", text ) );

			try
			{
				return new Quaternion(
					double.Parse( vals[ 0 ] ),
					double.Parse( vals[ 1 ] ),
					double.Parse( vals[ 2 ] ),
					double.Parse( vals[ 3 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		[AutoConvertType]
		public override string ToString()
		{
			return ToString( 17 );
			//return string.Format( "{0} {1} {2} {3}", x, y, z, w );
		}

		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "} {3:0." + format + "}";
			return string.Format( format, X, Y, Z, W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Quaternion && this == (Quaternion)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Quaternion operator +( Quaternion v1, Quaternion v2 )
		{
			Quaternion result;
			result.X = v1.X + v2.X;
			result.Y = v1.Y + v2.Y;
			result.Z = v1.Z + v2.Z;
			result.W = v1.W + v2.W;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Quaternion operator -( Quaternion v1, Quaternion v2 )
		{
			Quaternion result;
			result.X = v1.X - v2.X;
			result.Y = v1.Y - v2.Y;
			result.Z = v1.Z - v2.Z;
			result.W = v1.W - v2.W;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Quaternion operator *( Quaternion v1, Quaternion v2 )
		{
			Quaternion result;
			result.X = v1.W * v2.X + v1.X * v2.W + v1.Y * v2.Z - v1.Z * v2.Y;
			result.Y = v1.W * v2.Y + v1.Y * v2.W + v1.Z * v2.X - v1.X * v2.Z;
			result.Z = v1.W * v2.Z + v1.Z * v2.W + v1.X * v2.Y - v1.Y * v2.X;
			result.W = v1.W * v2.W - v1.X * v2.X - v1.Y * v2.Y - v1.Z * v2.Z;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector3 operator *( Quaternion q, Vector3 v )
		{
			Vector3 result;
			Multiply( ref q, ref v, out result );
			return result;
			//Vec3D uv, uuv;
			//Vec3D qvec = new Vec3D( q.X, q.Y, q.Z );
			//uv = Vec3.Cross( qvec, v );
			//uuv = Vec3.Cross( qvec, uv );
			//uv *= ( 2.0f * q.W );
			//uuv *= 2.0f;
			//return v + uv + uuv;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector3 operator *( Vector3 v, Quaternion q )
		{
			Vector3 result;
			Multiply( ref q, ref v, out result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Quaternion operator *( Quaternion q, double v )
		{
			Quaternion result;
			result.X = q.X * v;
			result.Y = q.Y * v;
			result.Z = q.Z * v;
			result.W = q.W * v;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Quaternion operator *( double v, Quaternion q )
		{
			Quaternion result;
			result.X = q.X * v;
			result.Y = q.Y * v;
			result.Z = q.Z * v;
			result.W = q.W * v;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Quaternion operator -( Quaternion v )
		{
			Quaternion result;
			result.X = -v.X;
			result.Y = -v.Y;
			result.Z = -v.Z;
			result.W = -v.W;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Add( ref Quaternion v1, ref Quaternion v2, out Quaternion result )
		{
			result.X = v1.X + v2.X;
			result.Y = v1.Y + v2.Y;
			result.Z = v1.Z + v2.Z;
			result.W = v1.W + v2.W;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Subtract( ref Quaternion v1, ref Quaternion v2, out Quaternion result )
		{
			result.X = v1.X - v2.X;
			result.Y = v1.Y - v2.Y;
			result.Z = v1.Z - v2.Z;
			result.W = v1.W - v2.W;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Quaternion v1, ref Quaternion v2, out Quaternion result )
		{
			result.X = v1.W * v2.X + v1.X * v2.W + v1.Y * v2.Z - v1.Z * v2.Y;
			result.Y = v1.W * v2.Y + v1.Y * v2.W + v1.Z * v2.X - v1.X * v2.Z;
			result.Z = v1.W * v2.Z + v1.Z * v2.W + v1.X * v2.Y - v1.Y * v2.X;
			result.W = v1.W * v2.W - v1.X * v2.X - v1.Y * v2.Y - v1.Z * v2.Z;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Quaternion q, ref Vector3 v, out Vector3 result )
		{
			Vector3 uv, uuv;
			Vector3 qvec = new Vector3( q.X, q.Y, q.Z );
			Vector3.Cross( ref qvec, ref v, out uv );
			Vector3.Cross( ref qvec, ref uv, out uuv );
			double qw2 = 2.0 * q.W;
			uv.X *= qw2;
			uv.Y *= qw2;
			uv.Z *= qw2;
			//uv *= ( 2.0f * q.W );
			uuv.X *= 2.0;
			uuv.Y *= 2.0;
			uuv.Z *= 2.0;
			//uuv *= 2.0f;
			result.X = v.X + uv.X + uuv.X;
			result.Y = v.Y + uv.Y + uuv.Y;
			result.Z = v.Z + uv.Z + uuv.Z;
			//return v + uv + uuv;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Vector3 v, ref Quaternion q, out Vector3 result )
		{
			Multiply( ref q, ref v, out result );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Quaternion q, double v, out Quaternion result )
		{
			result.X = q.X * v;
			result.Y = q.Y * v;
			result.Z = q.Z * v;
			result.W = q.W * v;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( double v, ref Quaternion q, out Quaternion result )
		{
			result.X = q.X * v;
			result.Y = q.Y * v;
			result.Z = q.Z * v;
			result.W = q.W * v;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Negate( ref Quaternion v, out Quaternion result )
		{
			result.X = -v.X;
			result.Y = -v.Y;
			result.Z = -v.Z;
			result.W = -v.W;
		}

		//public static Quat Add( Quat v1, Quat v2 )
		//{
		//	Quat result;
		//	result.x = v1.x + v2.x;
		//	result.y = v1.y + v2.y;
		//	result.z = v1.z + v2.z;
		//	result.w = v1.w + v2.w;
		//	return result;
		//}

		//public static Quat Subtract( Quat v1, Quat v2 )
		//{
		//	Quat result;
		//	result.x = v1.x - v2.x;
		//	result.y = v1.y - v2.y;
		//	result.z = v1.z - v2.z;
		//	result.w = v1.w - v2.w;
		//	return result;
		//}

		//public static Quat Multiply( Quat v1, Quat v2 )
		//{
		//	Quat result;
		//	result.x = v1.w * v2.x + v1.x * v2.w + v1.y * v2.z - v1.z * v2.y;
		//	result.y = v1.w * v2.y + v1.y * v2.w + v1.z * v2.x - v1.x * v2.z;
		//	result.z = v1.w * v2.z + v1.z * v2.w + v1.x * v2.y - v1.y * v2.x;
		//	result.w = v1.w * v2.w - v1.x * v2.x - v1.y * v2.y - v1.z * v2.z;
		//	return result;
		//}

		//public static Quat Multiply( Quat q, double v )
		//{
		//	Quat result;
		//	result.x = q.x * v;
		//	result.y = q.y * v;
		//	result.z = q.z * v;
		//	result.w = q.w * v;
		//	return result;
		//}

		//public static Quat Multiply( double v, Quat q )
		//{
		//	Quat result;
		//	result.x = q.x * v;
		//	result.y = q.y * v;
		//	result.z = q.z * v;
		//	result.w = q.w * v;
		//	return result;
		//}

		//public static Quat Negate( Quat v )
		//{
		//	Quat result;
		//	result.x = -v.x;
		//	result.y = -v.y;
		//	result.z = -v.z;
		//	result.w = -v.w;
		//	return result;
		//}

		public unsafe double this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( double* v = &this.X )
				{
					return v[ index ];
				}
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( double* v = &this.X )
				{
					v[ index ] = value;
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Quaternion v1, Quaternion v2 )
		{
			return ( v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z && v1.W == v2.W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Quaternion v1, Quaternion v2 )
		{
			return ( v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z || v1.W != v2.W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Quaternion v, double epsilon )
		{
			if( Math.Abs( X - v.X ) > epsilon )
				return false;
			if( Math.Abs( Y - v.Y ) > epsilon )
				return false;
			if( Math.Abs( Z - v.Z ) > epsilon )
				return false;
			if( Math.Abs( W - v.W ) > epsilon )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref Quaternion v, double epsilon )
		{
			if( Math.Abs( X - v.X ) > epsilon )
				return false;
			if( Math.Abs( Y - v.Y ) > epsilon )
				return false;
			if( Math.Abs( Z - v.Z ) > epsilon )
				return false;
			if( Math.Abs( W - v.W ) > epsilon )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Inverse()
		{
			X = -X;
			Y = -Y;
			Z = -Z;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Quaternion GetInverse()
		{
			Quaternion result;
			result.X = -X;
			result.Y = -Y;
			result.Z = -Z;
			result.W = W;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double Length()
		{
			return Math.Sqrt( X * X + Y * Y + Z * Z + W * W );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Normalize()
		{
			double len = Math.Sqrt( X * X + Y * Y + Z * Z + W * W );
			if( len != 0 )
			{
				double ilength = 1.0 / len;
				X *= ilength;
				Y *= ilength;
				Z *= ilength;
				W *= ilength;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Quaternion GetNormalize()
		{
			Quaternion result;
			double len = Math.Sqrt( X * X + Y * Y + Z * Z + W * W );
			if( len != 0 )
			{
				double ilength = 1.0 / len;
				result.X = X * ilength;
				result.Y = Y * ilength;
				result.Z = Z * ilength;
				result.W = W * ilength;
			}
			else
			{
				result.X = X;
				result.Y = Y;
				result.Z = Z;
				result.W = W;
			}
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void GetNormalize( ref Quaternion q, out Quaternion result )
		{
			double len = Math.Sqrt( q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W );
			if( len != 0 )
			{
				double ilength = 1.0 / len;
				result.X = q.X * ilength;
				result.Y = q.Y * ilength;
				result.Z = q.Z * ilength;
				result.W = q.W * ilength;
			}
			else
			{
				result.X = q.X;
				result.Y = q.Y;
				result.Z = q.Z;
				result.W = q.W;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Matrix3 ToMatrix3()
		{
			Matrix3 result;
			ToMatrix3( out result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ToMatrix3( out Matrix3 result )
		{
			double x2 = X + X;
			double y2 = Y + Y;
			double z2 = Z + Z;
			double xx = X * x2;
			double xy = X * y2;
			double xz = X * z2;
			double yy = Y * y2;
			double yz = Y * z2;
			double zz = Z * z2;
			double wx = W * x2;
			double wy = W * y2;
			double wz = W * z2;

			result.Item0.X = 1.0 - ( yy + zz );
			result.Item0.Y = xy + wz;
			result.Item0.Z = xz - wy;
			result.Item1.X = xy - wz;
			result.Item1.Y = 1.0 - ( xx + zz );
			result.Item1.Z = yz + wx;
			result.Item2.X = xz + wy;
			result.Item2.Y = yz - wx;
			result.Item2.Z = 1.0 - ( xx + yy );
			//return new Mat3( 1.0f - ( yy + zz ), xy + wz, xz - wy,
			//   xy - wz, 1.0f - ( xx + zz ), yz + wx,
			//   xz + wy, yz - wx, 1.0f - ( xx + yy ) );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Quaternion Slerp( Quaternion from, Quaternion to, double t )
		{
			Quaternion result;
			Slerp( ref from, ref to, t, out result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Slerp( ref Quaternion from, ref Quaternion to, double t, out Quaternion result )
		{
			Quaternion temp;
			double omega, cosom, sinom, scale0, scale1;

			if( t <= 0.0 )
			{
				result = from;
				return;
			}
			if( t >= 1.0 )
			{
				result = to;
				return;
			}
			if( from == to )
			{
				result = to;
				return;
			}

			cosom = from.X * to.X + from.Y * to.Y + from.Z * to.Z + from.W * to.W;
			if( cosom < 0.0 )
			{
				temp = -to;
				cosom = -cosom;
			}
			else
				temp = to;

			if( ( 1.0 - cosom ) > MathEx.Epsilon )
			{
				scale0 = 1.0 - cosom * cosom;
				sinom = 1.0 / Math.Sqrt( scale0 );
				omega = Math.Atan2( scale0 * sinom, cosom );
				scale0 = Math.Sin( ( 1.0 - t ) * omega ) * sinom;
				scale1 = Math.Sin( t * omega ) * sinom;
			}
			else
			{
				scale0 = 1.0 - t;
				scale1 = t;
			}

			Quaternion s1, s2;
			Quaternion.Multiply( scale0, ref from, out s1 );
			Quaternion.Multiply( scale1, ref temp, out s2 );
			Quaternion.Add( ref s1, ref s2, out result );
			//return ( scale0 * from ) + ( scale1 * temp );
		}

		//need for editor Rotation properties
		public Angles Angles
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return ToAngles(); }
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { this = value.ToQuaternion(); }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Angles ToAngles()
		{
			Angles result;
			Matrix3 mat;
			ToMatrix3( out mat );
			mat.ToAngles( out result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void ToAngles( out Angles result )
		{
			Matrix3 mat;
			ToMatrix3( out mat );
			mat.ToAngles( out result );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3 GetForward()
		{
			Vector3 result;
			result.X = 1.0 - ( Y * Y + Z * Z ) * 2.0;
			result.Y = ( Z * W + X * Y ) * 2.0;
			result.Z = ( X * Z - Y * W ) * 2.0;
			//result.x = 1.0f + ( -y * y - z * z ) * 2.0f;
			//result.y = ( z * w + ( x * y ) ) * 2.0f;
			//result.z = ( -y * w + ( x * z ) ) * 2.0f;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetForward( out Vector3 result )
		{
			result.X = 1.0 - ( Y * Y + Z * Z ) * 2.0;
			result.Y = ( Z * W + X * Y ) * 2.0;
			result.Z = ( X * Z - Y * W ) * 2.0;

			//result.x = 1.0f + ( -y * y - z * z ) * 2.0f;
			//result.y = ( z * w + ( x * y ) ) * 2.0f;
			//result.z = ( -y * w + ( x * z ) ) * 2.0f;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3 GetUp()
		{
			Vector3 result;
			result.X = ( Y * W + Z * X ) * 2.0;
			result.Y = ( Z * Y - X * W ) * 2.0;
			result.Z = 1.0 - ( X * X + Y * Y ) * 2.0;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetUp( out Vector3 result )
		{
			result.X = ( Y * W + Z * X ) * 2.0;
			result.Y = ( Z * Y - X * W ) * 2.0;
			result.Z = 1.0 - ( X * X + Y * Y ) * 2.0;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3 GetLeft()
		{
			//!!!!slowly
			return this * new Vector3( 0, 1, 0 );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3 GetRight()
		{
			//!!!!slowly
			return this * new Vector3( 0, -1, 0 );
		}

		/// <summary>
		/// Similar to Quat.LookAt( direction, Vec3.ZAxis ) with fix for vertical direction.
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		[MethodImpl( (MethodImplOptions)512 )]
		public static void FromDirectionZAxisUp( ref Vector3 direction, out Quaternion result )
		{
			//SphereDir sphereDir = SphereDir.FromVector( direction );
			//rotation = new Angles( 0, 0, MathFunctions.RadToDeg(
			//   -sphereDir.Horizontal ) ).ToQuatD();
			//rotation *= new Angles( 0,
			//   MathFunctions.RadToDeg( sphereDir.Vertical ), 0 ).ToQuatD();

			double horizontal;
			double vertical;
			{
				horizontal = Math.Atan2( direction.Y, direction.X );
				double dir2Length = Math.Sqrt( direction.X * direction.X + direction.Y * direction.Y );
				vertical = Math.Atan2( direction.Z, dir2Length );
			}

			//QuatD horizRotation;
			double horizRotationZ;
			double horizRotationW;
			{
				double a = -horizontal * .5;
				horizRotationZ = -Math.Sin( a );
				horizRotationW = Math.Cos( a );
				//double sz = Math.Sin( a );
				//double cz = Math.Cos( a );
				//horizRotation = new QuatD( 0, 0, -sz, cz );
			}

			//QuatD vertRotation;
			double vertRotationY;
			double vertRotationW;
			{
				double a = vertical * .5;
				vertRotationY = -Math.Sin( a );
				vertRotationW = Math.Cos( a );
				//double sy = Math.Sin( a );
				//double cy = Math.Cos( a );
				//vertRotation = new QuatD( 0, -sy, 0, cy );
			}

			result = new Quaternion(
				-horizRotationZ * vertRotationY, horizRotationW * vertRotationY,
				horizRotationZ * vertRotationW, horizRotationW * vertRotationW );
			//return horizRotation * vertRotation;
		}

		/// <summary>
		/// Similar to Quat.LookAt( direction, Vec3.ZAxis ) with fix for vertical direction.
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Quaternion FromDirectionZAxisUp( Vector3 direction )
		{
			FromDirectionZAxisUp( ref direction, out var result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian GetAngle( Quaternion v1, Quaternion v2 )
		{
			double a = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
			return new Radian( Math.Acos( a ) * 2 );
		}

		[AutoConvertType]
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public QuaternionF ToQuaternionF()
		{
			QuaternionF result;
			result.X = (float)X;
			result.Y = (float)Y;
			result.Z = (float)Z;
			result.W = (float)W;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public QuaternionH ToQuaternionH()
		{
			QuaternionH result;
			result.X = new HalfType( X );
			result.Y = new HalfType( Y );
			result.Z = new HalfType( Z );
			result.W = new HalfType( W );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void LookAt( ref Vector3 direction, ref Vector3 up, out Quaternion result )
		{
			Matrix3.LookAt( ref direction, ref up, out var matrix );
			matrix.ToQuaternion( out result );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Quaternion LookAt( Vector3 direction, Vector3 up )
		{
			LookAt( ref direction, ref up, out var result );
			return result;
			//return Matrix3.LookAt( direction, up ).ToQuaternion();
		}

		/////////////////////////////////////////

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Quaternion FromRotateByX( Radian angle )
		{
			Matrix3.FromRotateByX( angle, out var mat );
			return mat.ToQuaternion();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void FromRotateByX( Radian angle, out Quaternion result )
		{
			Matrix3.FromRotateByX( angle, out var mat );
			mat.ToQuaternion( out result );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Quaternion FromRotateByY( Radian angle )
		{
			Matrix3.FromRotateByY( angle, out var mat );
			return mat.ToQuaternion();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void FromRotateByY( Radian angle, out Quaternion result )
		{
			Matrix3.FromRotateByY( angle, out var mat );
			mat.ToQuaternion( out result );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Quaternion FromRotateByZ( Radian angle )
		{
			Matrix3.FromRotateByZ( angle, out var mat );
			return mat.ToQuaternion();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void FromRotateByZ( Radian angle, out Quaternion result )
		{
			Matrix3.FromRotateByZ( angle, out var mat );
			mat.ToQuaternion( out result );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref Quaternion v )
		{
			return X == v.X && Y == v.Y && Z == v.Z && W == v.W;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool Equals( ref Quaternion v1, ref Quaternion v2 )
		{
			return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z && v1.W == v2.W;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector4 ToVector4()
		{
			return new Vector4( X, Y, Z, W );
		}
	}
}
