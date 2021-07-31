// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	//[TypeConverter( typeof( _QuatFAsAnglesConverter ) )]
	/// <summary>
	/// Represents a double single precision four-dimensional mathematical quaternion.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct QuaternionF
	{
		public float X;
		public float Y;
		public float Z;
		public float W;

		public static readonly QuaternionF Identity = new QuaternionF( 0.0f, 0.0f, 0.0f, 1.0f );

		public QuaternionF( Vector3F v, float w )
		{
			this.X = v.X;
			this.Y = v.Y;
			this.Z = v.Z;
			this.W = w;
		}

		public QuaternionF( float x, float y, float z, float w )
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
			this.W = w;
		}

		public QuaternionF( QuaternionF source )
		{
			this.X = source.X;
			this.Y = source.Y;
			this.Z = source.Z;
			this.W = source.W;
		}

		[AutoConvertType]
		public static QuaternionF Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			//parse from Angles (special case).
			if( vals.Length == 3 )
				return AnglesF.Parse( text ).ToQuaternion();

			if( vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (x y z w).", text ) );

			try
			{
				return new QuaternionF(
					float.Parse( vals[ 0 ] ),
					float.Parse( vals[ 1 ] ),
					float.Parse( vals[ 2 ] ),
					float.Parse( vals[ 3 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		[AutoConvertType]
		public override string ToString()
		{
			return ToString( 8 );
			//return string.Format( "{0} {1} {2} {3}", x, y, z, w );
		}

		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "} {3:0." + format + "}";
			return string.Format( format, (float)X, (float)Y, (float)Z, (float)W );
		}

		public override bool Equals( object obj )
		{
			return ( obj is QuaternionF && this == (QuaternionF)obj );
		}

		public override int GetHashCode()
		{
			return ( X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode() );
		}

		public static QuaternionF operator +( QuaternionF v1, QuaternionF v2 )
		{
			QuaternionF result;
			result.X = v1.X + v2.X;
			result.Y = v1.Y + v2.Y;
			result.Z = v1.Z + v2.Z;
			result.W = v1.W + v2.W;
			return result;
		}

		public static QuaternionF operator -( QuaternionF v1, QuaternionF v2 )
		{
			QuaternionF result;
			result.X = v1.X - v2.X;
			result.Y = v1.Y - v2.Y;
			result.Z = v1.Z - v2.Z;
			result.W = v1.W - v2.W;
			return result;
		}

		public static QuaternionF operator *( QuaternionF v1, QuaternionF v2 )
		{
			QuaternionF result;
			result.X = v1.W * v2.X + v1.X * v2.W + v1.Y * v2.Z - v1.Z * v2.Y;
			result.Y = v1.W * v2.Y + v1.Y * v2.W + v1.Z * v2.X - v1.X * v2.Z;
			result.Z = v1.W * v2.Z + v1.Z * v2.W + v1.X * v2.Y - v1.Y * v2.X;
			result.W = v1.W * v2.W - v1.X * v2.X - v1.Y * v2.Y - v1.Z * v2.Z;
			return result;
		}

		public static Vector3F operator *( QuaternionF q, Vector3F v )
		{
			Vector3F result;
			Multiply( ref q, ref v, out result );
			return result;
			//Vec3 uv, uuv;
			//Vec3 qvec = new Vec3( q.X, q.Y, q.Z );
			//uv = Vec3.Cross( qvec, v );
			//uuv = Vec3.Cross( qvec, uv );
			//uv *= ( 2.0f * q.W );
			//uuv *= 2.0f;
			//return v + uv + uuv;
		}

		public static Vector3F operator *( Vector3F v, QuaternionF q )
		{
			Vector3F result;
			Multiply( ref q, ref v, out result );
			return result;
		}

		public static QuaternionF operator *( QuaternionF q, float v )
		{
			QuaternionF result;
			result.X = q.X * v;
			result.Y = q.Y * v;
			result.Z = q.Z * v;
			result.W = q.W * v;
			return result;
		}

		public static QuaternionF operator *( float v, QuaternionF q )
		{
			QuaternionF result;
			result.X = q.X * v;
			result.Y = q.Y * v;
			result.Z = q.Z * v;
			result.W = q.W * v;
			return result;
		}

		public static QuaternionF operator -( QuaternionF v )
		{
			QuaternionF result;
			result.X = -v.X;
			result.Y = -v.Y;
			result.Z = -v.Z;
			result.W = -v.W;
			return result;
		}

		public static void Add( ref QuaternionF v1, ref QuaternionF v2, out QuaternionF result )
		{
			result.X = v1.X + v2.X;
			result.Y = v1.Y + v2.Y;
			result.Z = v1.Z + v2.Z;
			result.W = v1.W + v2.W;
		}

		public static void Subtract( ref QuaternionF v1, ref QuaternionF v2, out QuaternionF result )
		{
			result.X = v1.X - v2.X;
			result.Y = v1.Y - v2.Y;
			result.Z = v1.Z - v2.Z;
			result.W = v1.W - v2.W;
		}

		public static void Multiply( ref QuaternionF v1, ref QuaternionF v2, out QuaternionF result )
		{
			result.X = v1.W * v2.X + v1.X * v2.W + v1.Y * v2.Z - v1.Z * v2.Y;
			result.Y = v1.W * v2.Y + v1.Y * v2.W + v1.Z * v2.X - v1.X * v2.Z;
			result.Z = v1.W * v2.Z + v1.Z * v2.W + v1.X * v2.Y - v1.Y * v2.X;
			result.W = v1.W * v2.W - v1.X * v2.X - v1.Y * v2.Y - v1.Z * v2.Z;
		}

		public static void Multiply( ref QuaternionF q, ref Vector3F v, out Vector3F result )
		{
			Vector3F uv, uuv;
			Vector3F qvec = new Vector3F( q.X, q.Y, q.Z );
			Vector3F.Cross( ref qvec, ref v, out uv );
			Vector3F.Cross( ref qvec, ref uv, out uuv );
			float qw2 = 2.0f * q.W;
			uv.X *= qw2;
			uv.Y *= qw2;
			uv.Z *= qw2;
			//uv *= ( 2.0f * q.W );
			uuv.X *= 2.0f;
			uuv.Y *= 2.0f;
			uuv.Z *= 2.0f;
			//uuv *= 2.0f;
			result.X = v.X + uv.X + uuv.X;
			result.Y = v.Y + uv.Y + uuv.Y;
			result.Z = v.Z + uv.Z + uuv.Z;
			//return v + uv + uuv;
		}

		public static void Multiply( ref Vector3F v, ref QuaternionF q, out Vector3F result )
		{
			Multiply( ref q, ref v, out result );
		}

		public static void Multiply( ref QuaternionF q, float v, out QuaternionF result )
		{
			result.X = q.X * v;
			result.Y = q.Y * v;
			result.Z = q.Z * v;
			result.W = q.W * v;
		}

		public static void Multiply( float v, ref QuaternionF q, out QuaternionF result )
		{
			result.X = q.X * v;
			result.Y = q.Y * v;
			result.Z = q.Z * v;
			result.W = q.W * v;
		}

		public static void Negate( ref QuaternionF v, out QuaternionF result )
		{
			result.X = -v.X;
			result.Y = -v.Y;
			result.Z = -v.Z;
			result.W = -v.W;
		}

		//public static QuatF Add( QuatF v1, QuatF v2 )
		//{
		//	QuatF result;
		//	result.x = v1.x + v2.x;
		//	result.y = v1.y + v2.y;
		//	result.z = v1.z + v2.z;
		//	result.w = v1.w + v2.w;
		//	return result;
		//}

		//public static QuatF Subtract( QuatF v1, QuatF v2 )
		//{
		//	QuatF result;
		//	result.x = v1.x - v2.x;
		//	result.y = v1.y - v2.y;
		//	result.z = v1.z - v2.z;
		//	result.w = v1.w - v2.w;
		//	return result;
		//}

		//public static QuatF Multiply( QuatF v1, QuatF v2 )
		//{
		//	QuatF result;
		//	result.x = v1.w * v2.x + v1.x * v2.w + v1.y * v2.z - v1.z * v2.y;
		//	result.y = v1.w * v2.y + v1.y * v2.w + v1.z * v2.x - v1.x * v2.z;
		//	result.z = v1.w * v2.z + v1.z * v2.w + v1.x * v2.y - v1.y * v2.x;
		//	result.w = v1.w * v2.w - v1.x * v2.x - v1.y * v2.y - v1.z * v2.z;
		//	return result;
		//}

		//public static QuatF Multiply( QuatF q, float v )
		//{
		//	QuatF result;
		//	result.x = q.x * v;
		//	result.y = q.y * v;
		//	result.z = q.z * v;
		//	result.w = q.w * v;
		//	return result;
		//}

		//public static QuatF Multiply( float v, QuatF q )
		//{
		//	QuatF result;
		//	result.x = q.x * v;
		//	result.y = q.y * v;
		//	result.z = q.z * v;
		//	result.w = q.w * v;
		//	return result;
		//}

		//public static QuatF Negate( QuatF v )
		//{
		//	QuatF result;
		//	result.x = -v.x;
		//	result.y = -v.y;
		//	result.z = -v.z;
		//	result.w = -v.w;
		//	return result;
		//}

		public unsafe float this[ int index ]
		{
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( float* v = &this.X )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( float* v = &this.X )
				{
					v[ index ] = value;
				}
			}
		}

		public static bool operator ==( QuaternionF v1, QuaternionF v2 )
		{
			return ( v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z && v1.W == v2.W );
		}

		public static bool operator !=( QuaternionF v1, QuaternionF v2 )
		{
			return ( v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z || v1.W != v2.W );
		}

		public bool Equals( QuaternionF v, float epsilon )
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

		public bool Equals( ref QuaternionF v, float epsilon )
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

		public void Inverse()
		{
			X = -X;
			Y = -Y;
			Z = -Z;
		}

		public QuaternionF GetInverse()
		{
			QuaternionF result;
			result.X = -X;
			result.Y = -Y;
			result.Z = -Z;
			result.W = W;
			return result;
		}

		public float Length()
		{
			return MathEx.Sqrt( X * X + Y * Y + Z * Z + W * W );
		}

		public void Normalize()
		{
			float len = MathEx.Sqrt( X * X + Y * Y + Z * Z + W * W );
			if( len != 0 )
			{
				float ilength = 1.0f / len;
				X *= ilength;
				Y *= ilength;
				Z *= ilength;
				W *= ilength;
			}
		}

		public QuaternionF GetNormalize()
		{
			QuaternionF result;
			float len = MathEx.Sqrt( X * X + Y * Y + Z * Z + W * W );
			if( len != 0 )
			{
				float ilength = 1.0f / len;
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

		public static void GetNormalize( ref QuaternionF q, out QuaternionF result )
		{
			float len = MathEx.Sqrt( q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W );
			if( len != 0 )
			{
				float ilength = 1.0f / len;
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

		[AutoConvertType]
		public Matrix3F ToMatrix3()
		{
			Matrix3F result;
			ToMatrix3( out result );
			return result;
		}

		public void ToMatrix3( out Matrix3F result )
		{
			float x2 = X + X;
			float y2 = Y + Y;
			float z2 = Z + Z;
			float xx = X * x2;
			float xy = X * y2;
			float xz = X * z2;
			float yy = Y * y2;
			float yz = Y * z2;
			float zz = Z * z2;
			float wx = W * x2;
			float wy = W * y2;
			float wz = W * z2;

			result.Item0.X = 1.0f - ( yy + zz );
			result.Item0.Y = xy + wz;
			result.Item0.Z = xz - wy;
			result.Item1.X = xy - wz;
			result.Item1.Y = 1.0f - ( xx + zz );
			result.Item1.Z = yz + wx;
			result.Item2.X = xz + wy;
			result.Item2.Y = yz - wx;
			result.Item2.Z = 1.0f - ( xx + yy );
			//return new Mat3( 1.0f - ( yy + zz ), xy + wz, xz - wy,
			//   xy - wz, 1.0f - ( xx + zz ), yz + wx,
			//   xz + wy, yz - wx, 1.0f - ( xx + yy ) );
		}

		public static QuaternionF Slerp( QuaternionF from, QuaternionF to, float t )
		{
			QuaternionF result;
			Slerp( ref from, ref to, t, out result );
			return result;
		}

		public static void Slerp( ref QuaternionF from, ref QuaternionF to, float t, out QuaternionF result )
		{
			QuaternionF temp;
			float omega, cosom, sinom, scale0, scale1;

			if( t <= 0.0f )
				result = from;
			if( t >= 1.0f )
				result = to;
			if( from == to )
				result = to;

			cosom = from.X * to.X + from.Y * to.Y + from.Z * to.Z + from.W * to.W;
			if( cosom < 0.0f )
			{
				temp = -to;
				cosom = -cosom;
			}
			else
				temp = to;

			if( ( 1.0f - cosom ) > MathEx.Epsilon )
			{
				scale0 = 1.0f - cosom * cosom;
				sinom = MathEx.InvSqrt( scale0 );
				omega = MathEx.Atan2( scale0 * sinom, cosom );
				scale0 = MathEx.Sin( ( 1.0f - t ) * omega ) * sinom;
				scale1 = MathEx.Sin( t * omega ) * sinom;
			}
			else
			{
				scale0 = 1.0f - t;
				scale1 = t;
			}

			QuaternionF s1, s2;
			QuaternionF.Multiply( scale0, ref from, out s1 );
			QuaternionF.Multiply( scale1, ref temp, out s2 );
			QuaternionF.Add( ref s1, ref s2, out result );
			//return ( scale0 * from ) + ( scale1 * temp );
		}

		public AnglesF Angles
		{
			get { return ToAngles(); }
			set { this = value.ToQuaternion(); }
		}

		//!!!![AutoConvertType]? где еще так
		public AnglesF ToAngles()
		{
			AnglesF result;
			Matrix3F mat;
			ToMatrix3( out mat );
			mat.ToAngles( out result );
			return result;
		}

		public void ToAngles( out AnglesF result )
		{
			Matrix3F mat;
			ToMatrix3( out mat );
			mat.ToAngles( out result );
		}

		public Vector3F GetForward()
		{
			Vector3F result;
			result.X = 1.0f - ( Y * Y + Z * Z ) * 2.0f;
			result.Y = ( Z * W + X * Y ) * 2.0f;
			result.Z = ( X * Z - Y * W ) * 2.0f;
			//result.x = 1.0f + ( -y * y - z * z ) * 2.0f;
			//result.y = ( z * w + ( x * y ) ) * 2.0f;
			//result.z = ( -y * w + ( x * z ) ) * 2.0f;
			return result;
		}

		public void GetForward( out Vector3F result )
		{
			result.X = 1.0f - ( Y * Y + Z * Z ) * 2.0f;
			result.Y = ( Z * W + X * Y ) * 2.0f;
			result.Z = ( X * Z - Y * W ) * 2.0f;

			//result.x = 1.0f + ( -y * y - z * z ) * 2.0f;
			//result.y = ( z * w + ( x * y ) ) * 2.0f;
			//result.z = ( -y * w + ( x * z ) ) * 2.0f;
		}

		public Vector3F GetUp()
		{
			Vector3F result;
			result.X = ( Y * W + Z * X ) * 2.0f;
			result.Y = ( Z * Y - X * W ) * 2.0f;
			result.Z = 1.0f - ( X * X + Y * Y ) * 2.0f;
			return result;
		}

		public void GetUp( out Vector3F result )
		{
			result.X = ( Y * W + Z * X ) * 2.0f;
			result.Y = ( Z * Y - X * W ) * 2.0f;
			result.Z = 1.0f - ( X * X + Y * Y ) * 2.0f;
		}

		public Vector3 GetLeft()
		{
			//!!!!slowly
			return this * new Vector3F( 0, 1, 0 );
		}

		public Vector3 GetRight()
		{
			//!!!!slowly
			return this * new Vector3F( 0, -1, 0 );
		}

		/// <summary>
		/// Similar to Mat3F.LookAt( direction, Vec3F.ZAxis ) with fix for vertical direction.
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		public static void FromDirectionZAxisUp( ref Vector3F direction, out QuaternionF result )
		{
			//SphereDir sphereDir = SphereDir.FromVector( direction );
			//rotation = new Angles( 0, 0, MathFunctions.RadToDeg(
			//   -sphereDir.Horizontal ) ).ToQuat();
			//rotation *= new Angles( 0,
			//   MathFunctions.RadToDeg( sphereDir.Vertical ), 0 ).ToQuat();

			float horizontal;
			float vertical;
			{
				horizontal = MathEx.Atan2( direction.Y, direction.X );
				float dir2Length = MathEx.Sqrt( direction.X * direction.X + direction.Y * direction.Y );
				vertical = MathEx.Atan2( direction.Z, dir2Length );
			}

			//Quat horizRotation;
			float horizRotationZ;
			float horizRotationW;
			{
				float a = -horizontal * .5f;
				horizRotationZ = -MathEx.Sin( a );
				horizRotationW = MathEx.Cos( a );
				//float sz = MathFunctions.Sin( a );
				//float cz = MathFunctions.Cos( a );
				//horizRotation = new Quat( 0, 0, -sz, cz );
			}

			//Quat vertRotation;
			float vertRotationY;
			float vertRotationW;
			{
				float a = vertical * .5f;
				vertRotationY = -MathEx.Sin( a );
				vertRotationW = MathEx.Cos( a );
				//float sy = MathFunctions.Sin( a );
				//float cy = MathFunctions.Cos( a );
				//vertRotation = new Quat( 0, -sy, 0, cy );
			}

			result = new QuaternionF(
				-horizRotationZ * vertRotationY, horizRotationW * vertRotationY,
				horizRotationZ * vertRotationW, horizRotationW * vertRotationW );
			//return horizRotation * vertRotation;
		}

		/// <summary>
		/// Similar to Mat3F.LookAt( direction, Vec3F.ZAxis ) with fix for vertical direction.
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		public static QuaternionF FromDirectionZAxisUp( Vector3F direction )
		{
			FromDirectionZAxisUp( ref direction, out var result );
			return result;
		}

		public static RadianF GetAngle( QuaternionF v1, QuaternionF v2 )
		{
			float a = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z + v1.W * v2.W;
			return new RadianF( MathEx.Acos( a ) * 2 );
		}

		[AutoConvertType]
		public Quaternion ToQuaternion()
		{
			Quaternion result;
			result.X = X;
			result.Y = Y;
			result.Z = Z;
			result.W = W;
			return result;
		}

#if !DISABLE_IMPLICIT
		public static implicit operator Quaternion( QuaternionF v )
		{
			return new Quaternion( v );
		}
#endif

		public static QuaternionF LookAt( Vector3F direction, Vector3F up )
		{
			return Matrix3F.LookAt( direction, up ).ToQuaternion();
		}

		/////////////////////////////////////////

		public static QuaternionF FromRotateByX( RadianF angle )
		{
			Matrix3F.FromRotateByX( angle, out var mat );
			return mat.ToQuaternion();
		}

		public static void FromRotateByX( RadianF angle, out QuaternionF result )
		{
			Matrix3F.FromRotateByX( angle, out var mat );
			mat.ToQuaternion( out result );
		}

		public static QuaternionF FromRotateByY( RadianF angle )
		{
			Matrix3F.FromRotateByY( angle, out var mat );
			return mat.ToQuaternion();
		}

		public static void FromRotateByY( RadianF angle, out QuaternionF result )
		{
			Matrix3F.FromRotateByY( angle, out var mat );
			mat.ToQuaternion( out result );
		}

		public static QuaternionF FromRotateByZ( RadianF angle )
		{
			Matrix3F.FromRotateByZ( angle, out var mat );
			return mat.ToQuaternion();
		}

		public static void FromRotateByZ( RadianF angle, out QuaternionF result )
		{
			Matrix3F.FromRotateByZ( angle, out var mat );
			mat.ToQuaternion( out result );
		}
	}
}
