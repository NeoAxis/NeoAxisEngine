// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Defines the position in space. This class is immutable for optimization purposes.
	/// </summary>
	[HCExpandable]
	public sealed class Transform
	{
		Vector3 position;
		Quaternion rotation = Quaternion.Identity;
		Vector3 scale = Vector3.One;

		Matrix4 matrix4;
		bool matrix4Calculated;
		//Matrix4? mat4;

		bool? positionZero;
		bool? rotationIdentity;
		bool? scaleOne;

		public static readonly Transform Identity = new Transform( Vector3.Zero, Quaternion.Identity, Vector3.One );
		public const string IdentityAsString = "0 0 0; 0 0 0 1; 1 1 1";
		public static readonly Transform Zero = new Transform( Vector3.Zero, Quaternion.Zero, Vector3.Zero );
		public const string ZeroAsString = "0 0 0; 0 0 0 0; 0 0 0";

		//

		public Transform()
		{
		}

		//public Transform( Transform source )
		//{
		//	this.position = source.position;
		//	this.rotation = source.rotation;
		//	this.scale = source.scale;
		//}

		//!!!!конструктор из матрицы или еще из каких-то типов

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform( Vector3 position, Quaternion rotation, Vector3 scale )
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform( Vector3 position, Quaternion rotation )
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = Vector3.One;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform( Vector3 position, Angles rotation, Vector3 scale )
		{
			this.position = position;
			this.rotation = rotation.ToQuaternion();
			this.scale = scale;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform( Vector3 position, Angles rotation )
		{
			this.position = position;
			this.rotation = rotation.ToQuaternion();
			this.scale = Vector3.One;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform( Vector3 position )
		{
			this.position = position;
			this.rotation = Quaternion.Identity;
			this.scale = Vector3.One;
		}

		//[DefaultValue( "0 0 0" )]
		//[Serialize]//!!!!или как simple type
		/// <summary>
		/// The position of the transform.
		/// </summary>
		public Vector3 Position
		{
			get { return position; }
		}

		//[DefaultValue( "0 0 0" )]//!!!!Quat жеж. как быть? определять по количеству цифер?
		//[Serialize]
		/// <summary>
		/// The rotation of the transform.
		/// </summary>
		public Quaternion Rotation
		{
			get { return rotation; }
		}

		//[DefaultValue( "1 1 1" )]
		//[Serialize]
		/// <summary>
		/// The scale of the transform.
		/// </summary>
		public Vector3 Scale
		{
			get { return scale; }
		}

		//!!!!

		[Browsable( false )]
		public bool IsPositionZero
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( positionZero == null )
					positionZero = position == Vector3.Zero;
				return positionZero.Value;
			}
		}

		[Browsable( false )]
		public bool IsRotationIdentity
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( rotationIdentity == null )
					rotationIdentity = rotation == Quaternion.Identity;
				return rotationIdentity.Value;
			}
		}

		[Browsable( false )]
		public bool IsScaleOne
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( scaleOne == null )
					scaleOne = scale == Vector3.One;
				return scaleOne.Value;
			}
		}

		[Browsable( false )]
		public bool IsIdentity
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return IsPositionZero && IsRotationIdentity && IsScaleOne; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Transform && this == (Transform)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( position.GetHashCode() ^ rotation.GetHashCode() ^ scale.GetHashCode() );
		}

		//!!!!maybe slowly, зато оверрайдить можно поведение, если нужно новые свойства добавить
		/*!!!!protected sealed virtual*/
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		bool EqualsImpl( Transform a )
		{
			if( ReferenceEquals( this, a ) )
				return true;
			return position.Equals( ref a.position ) && rotation.Equals( ref a.rotation ) && scale.Equals( ref a.scale );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Transform a, Transform b )
		{
			bool aNull = ReferenceEquals( a, null );
			bool bNull = ReferenceEquals( b, null );
			if( aNull || bNull )
				return aNull && bNull;
			return a.EqualsImpl( b );

			//if( ReferenceEquals( a, b ) )
			//	return true;
			//return ( a.position == b.position && a.rotation == b.rotation && a.scale == b.scale );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Transform a, Transform b )
		{
			bool aNull = ReferenceEquals( a, null );
			bool bNull = ReferenceEquals( b, null );
			if( aNull || bNull )
				return !( aNull && bNull );
			return !a.EqualsImpl( b );

			//if( ReferenceEquals( a, b ) )
			//	return false;
			//return ( a.position != b.position || a.rotation != b.rotation || a.scale != b.scale );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public /*!!!!sealed virtual*/ bool Equals( Transform a, double epsilon )
		{
			if( ReferenceEquals( this, a ) )
				return true;

			if( !position.Equals( ref a.position, epsilon ) )
				return false;
			if( !rotation.Equals( ref a.rotation, epsilon ) )
				return false;
			if( !scale.Equals( ref a.scale, epsilon ) )
				return false;
			return true;
		}

		//!!!!!

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Vector3 operator *( Transform transform, Vector3 v )
		{
			transform.CalculateMatrix4();
			return transform.matrix4 * v;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Multiply( ref Bounds bounds, out Bounds result )
		{
			//!!!!slowly

			CalculateMatrix4();

			unsafe
			{
				Vector3* points = stackalloc Vector3[ 8 ];
				bounds.ToPoints( points );
				result = new Bounds( matrix4 * points[ 0 ] );
				for( int n = 1; n < 8; n++ )
				{
					Matrix4.Multiply( ref matrix4, ref points[ n ], out var point );
					result.Add( ref point );
					//b2.Add( transform.matrix4 * points[ n ] );
				}
			}

			//Vector3[] points = v.ToPoints();
			//var b2 = Bounds.Cleared;
			//foreach( var p in points )
			//	b2.Add( transform.matrix4 * p );
			//return b2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Bounds operator *( Transform transform, Bounds v )
		{
			transform.Multiply( ref v, out var result );
			return result;

			//transform.CalculateMatrix4();

			//Vector3[] points = v.ToPoints();
			//var b2 = Bounds.Cleared;
			//foreach( var p in points )
			//	b2.Add( transform.matrix4 * p );
			//return b2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Multiply( ref Sphere sphere, out Sphere result )
		{
			//!!!!так?

			CalculateMatrix4();

			var c = matrix4 * sphere.Center;

			var max = Math.Max( Scale.X, Math.Max( Scale.Y, Scale.Z ) );
			var r = sphere.Radius * max;

			result = new Sphere( c, r );

			//var b = transform * v.ToBounds();
			//return b.ToBoundingSphere();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Sphere operator *( Transform transform, Sphere v )
		{
			transform.Multiply( ref v, out var result );
			return result;

			////!!!!так?

			//transform.CalculateMatrix4();
			//var c = transform.matrix4 * v.Center;

			//var max = Math.Max( transform.Scale.X, Math.Max( transform.Scale.Y, transform.Scale.Z ) );
			//var r = v.Radius * max;

			//return new Sphere( c, r );

			////var b = transform * v.ToBounds();
			////return b.ToBoundingSphere();
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void CalculateMatrix4()
		{
			if( !matrix4Calculated )
			{
				Matrix3 mat3;
				if( !IsRotationIdentity )
				{
					if( !IsScaleOne )
					{
						rotation.ToMatrix3( out var rot );
						Matrix3.FromScale( ref scale, out var scl );
						Matrix3.Multiply( ref rot, ref scl, out mat3 );
					}
					else
						rotation.ToMatrix3( out mat3 );
				}
				else
					Matrix3.FromScale( ref scale, out mat3 );
				matrix4 = new Matrix4( ref mat3, ref position );
				matrix4Calculated = true;

				//rotation.ToMatrix3( out var rot );
				//Matrix3.FromScale( ref scale, out var scl );
				//Matrix3.Multiply( ref rot, ref scl, out var rotScl );
				//mat4 = new Matrix4( rotScl, position );

				//mat4 = new Matrix4( rotation.ToMatrix3() * Matrix3.FromScale( scale ), position );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ref Matrix4 ToMatrix4()
		{
			CalculateMatrix4();
			return ref matrix4;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void ToMatrix4( bool useTraslation, bool useRotation, bool useScaling, out Matrix4 result )
		{
			if( useTraslation && useRotation && useScaling )
				result = ToMatrix4();
			else
			{
				//!!!!slowly

				var mat3 = Matrix3.Identity;

				if( useRotation && !IsRotationIdentity )
					mat3 = rotation.ToMatrix3();

				if( useScaling && !IsScaleOne )
					mat3 *= Matrix3.FromScale( scale );

				result = new Matrix4( mat3, useTraslation ? position : Vector3.Zero );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Matrix4 ToMatrix4( bool useTraslation = true, bool useRotation = true, bool useScaling = true )
		{
			ToMatrix4( useTraslation, useRotation, useScaling, out var result );
			return result;
		}

		[AutoConvertType]//!!!!?
		public static Transform Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Replace( ';', ' ' ).Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 10 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 10 parts separated by spaces.", text ) );

			try
			{
				return new Transform(
					new Vector3(
						double.Parse( vals[ 0 ] ),
						double.Parse( vals[ 1 ] ),
						double.Parse( vals[ 2 ] ) ),
					new Quaternion(
						double.Parse( vals[ 3 ] ),
						double.Parse( vals[ 4 ] ),
						double.Parse( vals[ 5 ] ),
						double.Parse( vals[ 6 ] ) ),
					new Vector3(
						double.Parse( vals[ 7 ] ),
						double.Parse( vals[ 8 ] ),
						double.Parse( vals[ 9 ] ) ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		[AutoConvertType]//!!!!?
		public override string ToString()
		{
			return string.Format( "{0}; {1}; {2}", position, rotation, scale );
		}

		//!!!!new
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Transform operator *( Transform v1, Transform v2 )
		{
			//!!!!так? или через матрицы и декомпозировать?

			//!!!!slowly. check IsIdentity, etc

			if( v1.IsIdentity )
				return v2;
			if( v2.IsIdentity )
				return v1;

			Vector3 pos = v1.Position;
			Quaternion rot = v1.Rotation;
			Vector3 scl = v1.Scale;

			pos += rot * ( v2.Position * scl );
			rot *= v2.Rotation;
			scl *= v2.Scale;

			return new Transform( pos, rot, scl );
		}

		//!!!name: Offset, AddOffset
		//!!!!new
		//!!!!default values
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform ApplyOffset( Vector3 positionOffset, Quaternion rotationOffset, Vector3 scaleOffset )
		{
			Vector3 pos = Position;
			Quaternion rot = Rotation;
			Vector3 scl = Scale;

			pos += rot * ( positionOffset * scl );
			rot *= rotationOffset;
			scl *= scaleOffset;

			return new Transform( pos, rot, scl );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform UpdatePosition( Vector3 position )
		{
			return new Transform( position, Rotation, Scale );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform UpdateRotation( Quaternion rotation )
		{
			return new Transform( Position, rotation, Scale );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform UpdateScale( Vector3 scale )
		{
			return new Transform( Position, Rotation, scale );
		}


		//!!!!good?

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform Translate( Vector3 position )
		{
			return new Transform( Position + position, Rotation, Scale );
		}

		//!!!!Rotate (eulers or Angles)

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform Rotate( Quaternion rotation )
		{
			return new Transform( Position, Rotation * rotation, Scale );
		}

		//!!!!need?

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform MultiplyScale( Vector3 scale )
		{
			return new Transform( Position, Rotation, Scale * scale );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Transform MultiplyScale( double scale )
		{
			return new Transform( Position, Rotation, Scale * scale );
		}
	}
}






//!!!!
//if( transform.IsScaleOne )
//{
//	xx xx;

//	if( spaceBounds.boundingSphere != null )
//	{
//		var v = spaceBounds.boundingSphere.Value;
//		s = new Sphere( transform.Position+ v.origin, v.radius );
//	}
//}
//else
//{
//	if( spaceBounds.boundingSphere != null )
//	{
//		//!!!!use rotation

//		var max = Math.Max( transform.Scale.X, Math.Max( transform.Scale.Y, transform.Scale.Z ) );
//		var v = spaceBounds.boundingSphere.Value;
//		s = new Sphere( transform.Position+ v.origin, v.radius * max );
//	}

//}


//public static Mat4 operator +( Mat4 v1, Mat4 v2 )
//{
//	Mat4 result;
//	result.mat0.x = v1.mat0.x + v2.mat0.x;
//	result.mat0.y = v1.mat0.y + v2.mat0.y;
//	result.mat0.z = v1.mat0.z + v2.mat0.z;
//	result.mat0.w = v1.mat0.w + v2.mat0.w;
//	result.mat1.x = v1.mat1.x + v2.mat1.x;
//	result.mat1.y = v1.mat1.y + v2.mat1.y;
//	result.mat1.z = v1.mat1.z + v2.mat1.z;
//	result.mat1.w = v1.mat1.w + v2.mat1.w;
//	result.mat2.x = v1.mat2.x + v2.mat2.x;
//	result.mat2.y = v1.mat2.y + v2.mat2.y;
//	result.mat2.z = v1.mat2.z + v2.mat2.z;
//	result.mat2.w = v1.mat2.w + v2.mat2.w;
//	result.mat3.x = v1.mat3.x + v2.mat3.x;
//	result.mat3.y = v1.mat3.y + v2.mat3.y;
//	result.mat3.z = v1.mat3.z + v2.mat3.z;
//	result.mat3.w = v1.mat3.w + v2.mat3.w;
//	return result;
//}

//public static Mat4 operator -( Mat4 v1, Mat4 v2 )
//{
//	Mat4 result;
//	result.mat0.x = v1.mat0.x - v2.mat0.x;
//	result.mat0.y = v1.mat0.y - v2.mat0.y;
//	result.mat0.z = v1.mat0.z - v2.mat0.z;
//	result.mat0.w = v1.mat0.w - v2.mat0.w;
//	result.mat1.x = v1.mat1.x - v2.mat1.x;
//	result.mat1.y = v1.mat1.y - v2.mat1.y;
//	result.mat1.z = v1.mat1.z - v2.mat1.z;
//	result.mat1.w = v1.mat1.w - v2.mat1.w;
//	result.mat2.x = v1.mat2.x - v2.mat2.x;
//	result.mat2.y = v1.mat2.y - v2.mat2.y;
//	result.mat2.z = v1.mat2.z - v2.mat2.z;
//	result.mat2.w = v1.mat2.w - v2.mat2.w;
//	result.mat3.x = v1.mat3.x - v2.mat3.x;
//	result.mat3.y = v1.mat3.y - v2.mat3.y;
//	result.mat3.z = v1.mat3.z - v2.mat3.z;
//	result.mat3.w = v1.mat3.w - v2.mat3.w;
//	return result;
//}

//public static Mat4 operator *( Mat4 m, double s )
//{
//	Mat4 result;
//	result.mat0.x = m.mat0.x * s;
//	result.mat0.y = m.mat0.y * s;
//	result.mat0.z = m.mat0.z * s;
//	result.mat0.w = m.mat0.w * s;
//	result.mat1.x = m.mat1.x * s;
//	result.mat1.y = m.mat1.y * s;
//	result.mat1.z = m.mat1.z * s;
//	result.mat1.w = m.mat1.w * s;
//	result.mat2.x = m.mat2.x * s;
//	result.mat2.y = m.mat2.y * s;
//	result.mat2.z = m.mat2.z * s;
//	result.mat2.w = m.mat2.w * s;
//	result.mat3.x = m.mat3.x * s;
//	result.mat3.y = m.mat3.y * s;
//	result.mat3.z = m.mat3.z * s;
//	result.mat3.w = m.mat3.w * s;
//	return result;
//}

//public static Mat4 operator *( double s, Mat4 m )
//{
//	Mat4 result;
//	result.mat0.x = m.mat0.x * s;
//	result.mat0.y = m.mat0.y * s;
//	result.mat0.z = m.mat0.z * s;
//	result.mat0.w = m.mat0.w * s;
//	result.mat1.x = m.mat1.x * s;
//	result.mat1.y = m.mat1.y * s;
//	result.mat1.z = m.mat1.z * s;
//	result.mat1.w = m.mat1.w * s;
//	result.mat2.x = m.mat2.x * s;
//	result.mat2.y = m.mat2.y * s;
//	result.mat2.z = m.mat2.z * s;
//	result.mat2.w = m.mat2.w * s;
//	result.mat3.x = m.mat3.x * s;
//	result.mat3.y = m.mat3.y * s;
//	result.mat3.z = m.mat3.z * s;
//	result.mat3.w = m.mat3.w * s;
//	return result;
//}

//public static Ray operator *( Mat4 m, Ray r )
//{
//	Ray result;
//	Mat4.Multiply( ref r.origin, ref m, out result.origin );
//	Vec3 sourceTo;
//	Vec3.Add( ref r.origin, ref r.direction, out sourceTo );
//	Vec3 to;
//	Mat4.Multiply( ref sourceTo, ref m, out to );
//	Vec3.Subtract( ref to, ref result.origin, out result.direction );
//	return result;
//	//Vec3D origin = r.Origin * m;
//	//Vec3D to = ( r.Origin + r.Direction ) * m;
//	//return new Ray( origin, to - origin );
//}

//public static Ray operator *( Ray r, Mat4 m )
//{
//	Ray result;
//	Mat4.Multiply( ref r.origin, ref m, out result.origin );
//	Vec3 sourceTo;
//	Vec3.Add( ref r.origin, ref r.direction, out sourceTo );
//	Vec3 to;
//	Mat4.Multiply( ref sourceTo, ref m, out to );
//	Vec3.Subtract( ref to, ref result.origin, out result.direction );
//	return result;
//}

//public static Vec3 operator *( Mat4 m, Vec3 v )
//{
//	Vec3 result;
//	result.x = m.mat0.x * v.X + m.mat1.x * v.Y + m.mat2.x * v.Z + m.mat3.x;
//	result.y = m.mat0.y * v.X + m.mat1.y * v.Y + m.mat2.y * v.Z + m.mat3.y;
//	result.z = m.mat0.z * v.X + m.mat1.z * v.Y + m.mat2.z * v.Z + m.mat3.z;
//	return result;
//}

//public static Vec3 operator *( Vec3 v, Mat4 m )
//{
//	Vec3 result;
//	result.x = m.mat0.x * v.X + m.mat1.x * v.Y + m.mat2.x * v.Z + m.mat3.x;
//	result.y = m.mat0.y * v.X + m.mat1.y * v.Y + m.mat2.y * v.Z + m.mat3.y;
//	result.z = m.mat0.z * v.X + m.mat1.z * v.Y + m.mat2.z * v.Z + m.mat3.z;
//	return result;
//}

//public static Vec4 operator *( Mat4 m, Vec4 v )
//{
//	Vec4 result;
//	result.x = m.mat0.x * v.X + m.mat1.x * v.Y + m.mat2.x * v.Z + m.mat3.x * v.W;
//	result.y = m.mat0.y * v.X + m.mat1.y * v.Y + m.mat2.y * v.Z + m.mat3.y * v.W;
//	result.z = m.mat0.z * v.X + m.mat1.z * v.Y + m.mat2.z * v.Z + m.mat3.z * v.W;
//	result.w = m.mat0.w * v.X + m.mat1.w * v.Y + m.mat2.w * v.Z + m.mat3.w * v.W;
//	return result;
//}

//public static Vec4 operator *( Vec4 v, Mat4 m )
//{
//	Vec4 result;
//	result.x = m.mat0.x * v.X + m.mat1.x * v.Y + m.mat2.x * v.Z + m.mat3.x * v.W;
//	result.y = m.mat0.y * v.X + m.mat1.y * v.Y + m.mat2.y * v.Z + m.mat3.y * v.W;
//	result.z = m.mat0.z * v.X + m.mat1.z * v.Y + m.mat2.z * v.Z + m.mat3.z * v.W;
//	result.w = m.mat0.w * v.X + m.mat1.w * v.Y + m.mat2.w * v.Z + m.mat3.w * v.W;
//	return result;
//}

//public static Transform operator *( Transform a, Transform b )
//{
//	//!!!!slowly

//	transform.CalculateMat4();

//	Vec3[] points = v.ToPoints();
//	var b2 = Bounds.Cleared;
//	foreach( var p in points )
//		b2.Add( transform.mat4.Value * p );
//	return b2;
//}
//public static Mat4 operator *( Mat4 v1, Mat4 v2 )
//{
//	Mat4 result;
//	result.mat0.x = v1.mat0.x * v2.mat0.x + v1.mat1.x * v2.mat0.y + v1.mat2.x * v2.mat0.z + v1.mat3.x * v2.mat0.w;
//	result.mat0.y = v1.mat0.y * v2.mat0.x + v1.mat1.y * v2.mat0.y + v1.mat2.y * v2.mat0.z + v1.mat3.y * v2.mat0.w;
//	result.mat0.z = v1.mat0.z * v2.mat0.x + v1.mat1.z * v2.mat0.y + v1.mat2.z * v2.mat0.z + v1.mat3.z * v2.mat0.w;
//	result.mat0.w = v1.mat0.w * v2.mat0.x + v1.mat1.w * v2.mat0.y + v1.mat2.w * v2.mat0.z + v1.mat3.w * v2.mat0.w;
//	result.mat1.x = v1.mat0.x * v2.mat1.x + v1.mat1.x * v2.mat1.y + v1.mat2.x * v2.mat1.z + v1.mat3.x * v2.mat1.w;
//	result.mat1.y = v1.mat0.y * v2.mat1.x + v1.mat1.y * v2.mat1.y + v1.mat2.y * v2.mat1.z + v1.mat3.y * v2.mat1.w;
//	result.mat1.z = v1.mat0.z * v2.mat1.x + v1.mat1.z * v2.mat1.y + v1.mat2.z * v2.mat1.z + v1.mat3.z * v2.mat1.w;
//	result.mat1.w = v1.mat0.w * v2.mat1.x + v1.mat1.w * v2.mat1.y + v1.mat2.w * v2.mat1.z + v1.mat3.w * v2.mat1.w;
//	result.mat2.x = v1.mat0.x * v2.mat2.x + v1.mat1.x * v2.mat2.y + v1.mat2.x * v2.mat2.z + v1.mat3.x * v2.mat2.w;
//	result.mat2.y = v1.mat0.y * v2.mat2.x + v1.mat1.y * v2.mat2.y + v1.mat2.y * v2.mat2.z + v1.mat3.y * v2.mat2.w;
//	result.mat2.z = v1.mat0.z * v2.mat2.x + v1.mat1.z * v2.mat2.y + v1.mat2.z * v2.mat2.z + v1.mat3.z * v2.mat2.w;
//	result.mat2.w = v1.mat0.w * v2.mat2.x + v1.mat1.w * v2.mat2.y + v1.mat2.w * v2.mat2.z + v1.mat3.w * v2.mat2.w;
//	result.mat3.x = v1.mat0.x * v2.mat3.x + v1.mat1.x * v2.mat3.y + v1.mat2.x * v2.mat3.z + v1.mat3.x * v2.mat3.w;
//	result.mat3.y = v1.mat0.y * v2.mat3.x + v1.mat1.y * v2.mat3.y + v1.mat2.y * v2.mat3.z + v1.mat3.y * v2.mat3.w;
//	result.mat3.z = v1.mat0.z * v2.mat3.x + v1.mat1.z * v2.mat3.y + v1.mat2.z * v2.mat3.z + v1.mat3.z * v2.mat3.w;
//	result.mat3.w = v1.mat0.w * v2.mat3.x + v1.mat1.w * v2.mat3.y + v1.mat2.w * v2.mat3.z + v1.mat3.w * v2.mat3.w;
//	return result;
//}

//public static Mat4 operator -( Mat4 v )
//{
//	Mat4 result;
//	result.mat0.x = -v.mat0.x;
//	result.mat0.y = -v.mat0.y;
//	result.mat0.z = -v.mat0.z;
//	result.mat0.w = -v.mat0.w;
//	result.mat1.x = -v.mat1.x;
//	result.mat1.y = -v.mat1.y;
//	result.mat1.z = -v.mat1.z;
//	result.mat1.w = -v.mat1.w;
//	result.mat2.x = -v.mat2.x;
//	result.mat2.y = -v.mat2.y;
//	result.mat2.z = -v.mat2.z;
//	result.mat2.w = -v.mat2.w;
//	result.mat3.x = -v.mat3.x;
//	result.mat3.y = -v.mat3.y;
//	result.mat3.z = -v.mat3.z;
//	result.mat3.w = -v.mat3.w;
//	return result;
//}

//public static void Add( ref Mat4 v1, ref Mat4 v2, out Mat4 result )
//{
//	result.mat0.x = v1.mat0.x + v2.mat0.x;
//	result.mat0.y = v1.mat0.y + v2.mat0.y;
//	result.mat0.z = v1.mat0.z + v2.mat0.z;
//	result.mat0.w = v1.mat0.w + v2.mat0.w;
//	result.mat1.x = v1.mat1.x + v2.mat1.x;
//	result.mat1.y = v1.mat1.y + v2.mat1.y;
//	result.mat1.z = v1.mat1.z + v2.mat1.z;
//	result.mat1.w = v1.mat1.w + v2.mat1.w;
//	result.mat2.x = v1.mat2.x + v2.mat2.x;
//	result.mat2.y = v1.mat2.y + v2.mat2.y;
//	result.mat2.z = v1.mat2.z + v2.mat2.z;
//	result.mat2.w = v1.mat2.w + v2.mat2.w;
//	result.mat3.x = v1.mat3.x + v2.mat3.x;
//	result.mat3.y = v1.mat3.y + v2.mat3.y;
//	result.mat3.z = v1.mat3.z + v2.mat3.z;
//	result.mat3.w = v1.mat3.w + v2.mat3.w;
//}

//public static void Subtract( ref Mat4 v1, ref Mat4 v2, out Mat4 result )
//{
//	result.mat0.x = v1.mat0.x - v2.mat0.x;
//	result.mat0.y = v1.mat0.y - v2.mat0.y;
//	result.mat0.z = v1.mat0.z - v2.mat0.z;
//	result.mat0.w = v1.mat0.w - v2.mat0.w;
//	result.mat1.x = v1.mat1.x - v2.mat1.x;
//	result.mat1.y = v1.mat1.y - v2.mat1.y;
//	result.mat1.z = v1.mat1.z - v2.mat1.z;
//	result.mat1.w = v1.mat1.w - v2.mat1.w;
//	result.mat2.x = v1.mat2.x - v2.mat2.x;
//	result.mat2.y = v1.mat2.y - v2.mat2.y;
//	result.mat2.z = v1.mat2.z - v2.mat2.z;
//	result.mat2.w = v1.mat2.w - v2.mat2.w;
//	result.mat3.x = v1.mat3.x - v2.mat3.x;
//	result.mat3.y = v1.mat3.y - v2.mat3.y;
//	result.mat3.z = v1.mat3.z - v2.mat3.z;
//	result.mat3.w = v1.mat3.w - v2.mat3.w;
//}

//public static void Multiply( ref Mat4 m, double s, out Mat4 result )
//{
//	result.mat0.x = m.mat0.x * s;
//	result.mat0.y = m.mat0.y * s;
//	result.mat0.z = m.mat0.z * s;
//	result.mat0.w = m.mat0.w * s;
//	result.mat1.x = m.mat1.x * s;
//	result.mat1.y = m.mat1.y * s;
//	result.mat1.z = m.mat1.z * s;
//	result.mat1.w = m.mat1.w * s;
//	result.mat2.x = m.mat2.x * s;
//	result.mat2.y = m.mat2.y * s;
//	result.mat2.z = m.mat2.z * s;
//	result.mat2.w = m.mat2.w * s;
//	result.mat3.x = m.mat3.x * s;
//	result.mat3.y = m.mat3.y * s;
//	result.mat3.z = m.mat3.z * s;
//	result.mat3.w = m.mat3.w * s;
//}

//public static void Multiply( double s, ref Mat4 m, out Mat4 result )
//{
//	result.mat0.x = m.mat0.x * s;
//	result.mat0.y = m.mat0.y * s;
//	result.mat0.z = m.mat0.z * s;
//	result.mat0.w = m.mat0.w * s;
//	result.mat1.x = m.mat1.x * s;
//	result.mat1.y = m.mat1.y * s;
//	result.mat1.z = m.mat1.z * s;
//	result.mat1.w = m.mat1.w * s;
//	result.mat2.x = m.mat2.x * s;
//	result.mat2.y = m.mat2.y * s;
//	result.mat2.z = m.mat2.z * s;
//	result.mat2.w = m.mat2.w * s;
//	result.mat3.x = m.mat3.x * s;
//	result.mat3.y = m.mat3.y * s;
//	result.mat3.z = m.mat3.z * s;
//	result.mat3.w = m.mat3.w * s;
//}

//public static void Multiply( ref Mat4 m, ref Ray r, out Ray result )
//{
//	Mat4.Multiply( ref r.origin, ref m, out result.origin );
//	Vec3 sourceTo;
//	Vec3.Add( ref r.origin, ref r.direction, out sourceTo );
//	Vec3 to;
//	Mat4.Multiply( ref sourceTo, ref m, out to );
//	Vec3.Subtract( ref to, ref result.origin, out result.direction );
//}

//public static void Multiply( ref Ray r, ref Mat4 m, out Ray result )
//{
//	Mat4.Multiply( ref r.origin, ref m, out result.origin );
//	Vec3 sourceTo;
//	Vec3.Add( ref r.origin, ref r.direction, out sourceTo );
//	Vec3 to;
//	Mat4.Multiply( ref sourceTo, ref m, out to );
//	Vec3.Subtract( ref to, ref result.origin, out result.direction );
//}

//public static void Multiply( ref Mat4 m, ref Vec3 v, out Vec3 result )
//{
//	result.x = m.mat0.x * v.X + m.mat1.x * v.Y + m.mat2.x * v.Z + m.mat3.x;
//	result.y = m.mat0.y * v.X + m.mat1.y * v.Y + m.mat2.y * v.Z + m.mat3.y;
//	result.z = m.mat0.z * v.X + m.mat1.z * v.Y + m.mat2.z * v.Z + m.mat3.z;
//}

//public static void Multiply( ref Vec3 v, ref Mat4 m, out Vec3 result )
//{
//	result.x = m.mat0.x * v.X + m.mat1.x * v.Y + m.mat2.x * v.Z + m.mat3.x;
//	result.y = m.mat0.y * v.X + m.mat1.y * v.Y + m.mat2.y * v.Z + m.mat3.y;
//	result.z = m.mat0.z * v.X + m.mat1.z * v.Y + m.mat2.z * v.Z + m.mat3.z;
//}

//public static void Multiply( ref Mat4 m, ref Vec4 v, out Vec4 result )
//{
//	result.x = m.mat0.x * v.X + m.mat1.x * v.Y + m.mat2.x * v.Z + m.mat3.x * v.W;
//	result.y = m.mat0.y * v.X + m.mat1.y * v.Y + m.mat2.y * v.Z + m.mat3.y * v.W;
//	result.z = m.mat0.z * v.X + m.mat1.z * v.Y + m.mat2.z * v.Z + m.mat3.z * v.W;
//	result.w = m.mat0.w * v.X + m.mat1.w * v.Y + m.mat2.w * v.Z + m.mat3.w * v.W;
//}

//public static void Multiply( ref Vec4 v, ref Mat4 m, out Vec4 result )
//{
//	result.x = m.mat0.x * v.X + m.mat1.x * v.Y + m.mat2.x * v.Z + m.mat3.x * v.W;
//	result.y = m.mat0.y * v.X + m.mat1.y * v.Y + m.mat2.y * v.Z + m.mat3.y * v.W;
//	result.z = m.mat0.z * v.X + m.mat1.z * v.Y + m.mat2.z * v.Z + m.mat3.z * v.W;
//	result.w = m.mat0.w * v.X + m.mat1.w * v.Y + m.mat2.w * v.Z + m.mat3.w * v.W;
//}

//public static void Multiply( ref Mat4 v1, ref Mat4 v2, out Mat4 result )
//{
//	result.mat0.x = v1.mat0.x * v2.mat0.x + v1.mat1.x * v2.mat0.y + v1.mat2.x * v2.mat0.z + v1.mat3.x * v2.mat0.w;
//	result.mat0.y = v1.mat0.y * v2.mat0.x + v1.mat1.y * v2.mat0.y + v1.mat2.y * v2.mat0.z + v1.mat3.y * v2.mat0.w;
//	result.mat0.z = v1.mat0.z * v2.mat0.x + v1.mat1.z * v2.mat0.y + v1.mat2.z * v2.mat0.z + v1.mat3.z * v2.mat0.w;
//	result.mat0.w = v1.mat0.w * v2.mat0.x + v1.mat1.w * v2.mat0.y + v1.mat2.w * v2.mat0.z + v1.mat3.w * v2.mat0.w;
//	result.mat1.x = v1.mat0.x * v2.mat1.x + v1.mat1.x * v2.mat1.y + v1.mat2.x * v2.mat1.z + v1.mat3.x * v2.mat1.w;
//	result.mat1.y = v1.mat0.y * v2.mat1.x + v1.mat1.y * v2.mat1.y + v1.mat2.y * v2.mat1.z + v1.mat3.y * v2.mat1.w;
//	result.mat1.z = v1.mat0.z * v2.mat1.x + v1.mat1.z * v2.mat1.y + v1.mat2.z * v2.mat1.z + v1.mat3.z * v2.mat1.w;
//	result.mat1.w = v1.mat0.w * v2.mat1.x + v1.mat1.w * v2.mat1.y + v1.mat2.w * v2.mat1.z + v1.mat3.w * v2.mat1.w;
//	result.mat2.x = v1.mat0.x * v2.mat2.x + v1.mat1.x * v2.mat2.y + v1.mat2.x * v2.mat2.z + v1.mat3.x * v2.mat2.w;
//	result.mat2.y = v1.mat0.y * v2.mat2.x + v1.mat1.y * v2.mat2.y + v1.mat2.y * v2.mat2.z + v1.mat3.y * v2.mat2.w;
//	result.mat2.z = v1.mat0.z * v2.mat2.x + v1.mat1.z * v2.mat2.y + v1.mat2.z * v2.mat2.z + v1.mat3.z * v2.mat2.w;
//	result.mat2.w = v1.mat0.w * v2.mat2.x + v1.mat1.w * v2.mat2.y + v1.mat2.w * v2.mat2.z + v1.mat3.w * v2.mat2.w;
//	result.mat3.x = v1.mat0.x * v2.mat3.x + v1.mat1.x * v2.mat3.y + v1.mat2.x * v2.mat3.z + v1.mat3.x * v2.mat3.w;
//	result.mat3.y = v1.mat0.y * v2.mat3.x + v1.mat1.y * v2.mat3.y + v1.mat2.y * v2.mat3.z + v1.mat3.y * v2.mat3.w;
//	result.mat3.z = v1.mat0.z * v2.mat3.x + v1.mat1.z * v2.mat3.y + v1.mat2.z * v2.mat3.z + v1.mat3.z * v2.mat3.w;
//	result.mat3.w = v1.mat0.w * v2.mat3.x + v1.mat1.w * v2.mat3.y + v1.mat2.w * v2.mat3.z + v1.mat3.w * v2.mat3.w;
//}

//public static void Negate( ref Mat4 m, out Mat4 result )
//{
//	result.mat0.x = -m.mat0.x;
//	result.mat0.y = -m.mat0.y;
//	result.mat0.z = -m.mat0.z;
//	result.mat0.w = -m.mat0.w;
//	result.mat1.x = -m.mat1.x;
//	result.mat1.y = -m.mat1.y;
//	result.mat1.z = -m.mat1.z;
//	result.mat1.w = -m.mat1.w;
//	result.mat2.x = -m.mat2.x;
//	result.mat2.y = -m.mat2.y;
//	result.mat2.z = -m.mat2.z;
//	result.mat2.w = -m.mat2.w;
//	result.mat3.x = -m.mat3.x;
//	result.mat3.y = -m.mat3.y;
//	result.mat3.z = -m.mat3.z;
//	result.mat3.w = -m.mat3.w;
//}

//public static Mat4 Add( Mat4 v1, Mat4 v2 )
//{
//	Mat4 result;
//	Add( ref v1, ref v2, out result );
//	return result;
//}

//public static Mat4 Subtract( Mat4 v1, Mat4 v2 )
//{
//	Mat4 result;
//	Subtract( ref v1, ref v2, out result );
//	return result;
//}

//public static Mat4 Multiply( Mat4 m, double s )
//{
//	Mat4 result;
//	Multiply( ref m, s, out result );
//	return result;
//}

//public static Mat4 Multiply( double s, Mat4 m )
//{
//	Mat4 result;
//	Multiply( ref m, s, out result );
//	return result;
//}

//public static Ray Multiply( Mat4 m, Ray r )
//{
//	Ray result;
//	Multiply( ref m, ref r, out result );
//	return result;
//}

//public static Ray Multiply( Ray r, Mat4 m )
//{
//	Ray result;
//	Multiply( ref m, ref r, out result );
//	return result;
//}

//public static Vec3 Multiply( Mat4 m, Vec3 v )
//{
//	Vec3 result;
//	Multiply( ref m, ref v, out result );
//	return result;
//}

//public static Vec3 Multiply( Vec3 v, Mat4 m )
//{
//	Vec3 result;
//	Multiply( ref m, ref v, out result );
//	return result;
//}

//public static Vec4 Multiply( Mat4 m, Vec4 v )
//{
//	Vec4 result;
//	Multiply( ref m, ref v, out result );
//	return result;
//}

//public static Vec4 Multiply( Vec4 v, Mat4 m )
//{
//	Vec4 result;
//	Multiply( ref m, ref v, out result );
//	return result;
//}

//public static Mat4 Multiply( Mat4 v1, Mat4 v2 )
//{
//	Mat4 result;
//	Multiply( ref v1, ref v2, out result );
//	return result;
//}

//public static Mat4 Negate( Mat4 m )
//{
//	Mat4 result;
//	Negate( ref m, out result );
//	return result;
//}
