// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;


/*
				4---{4}---5
 +         /|        /|
 Z      {7} {8}   {5} |
 -     /    |    /    {9}
		7--{6}----6     |
		|     |   |     |
	 {11}    0---|-{0}-1
		|    /    |    /       -
		| {3}  {10} {1}       Y
		|/        |/         +
		3---{2}---2

		 - X +

*/

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<BoxF> ) )]
	/// <summary>
	/// Represents a single precision oriented bounding box in three dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct BoxF
	{
		/// <summary>
		/// The center of the box.
		/// </summary>
		[Serialize]
		public Vector3F Center;
		/// <summary>
		/// The extents of the box.
		/// </summary>
		[Serialize]
		public Vector3F Extents;
		/// <summary>
		/// Axis of the box.
		/// </summary>
		[Serialize]
		public Matrix3F Axis;

		/// <summary>
		/// The box with zero size.
		/// </summary>
		public static readonly BoxF Zero = new BoxF( Vector3F.Zero, Vector3F.Zero, Matrix3F.Identity );

		/// <summary>
		/// Not initialized box.
		/// </summary>
		public static readonly BoxF Cleared = new BoxF(
			Vector3F.Zero,
			new Vector3F( float.MinValue, float.MinValue, float.MinValue ),
			Matrix3F.Identity );

		/// <summary>
		/// Constructs a box with another specified <see cref="BoxF"/> object.
		/// </summary>
		/// <param name="source">A specified box.</param>
		public BoxF( BoxF source )
		{
			Center = source.Center;
			Extents = source.Extents;
			Axis = source.Axis;
		}

		/// <summary>
		/// Constructs a box with the given center, extents and axis.
		/// </summary>
		/// <param name="center">The center <see cref="Vector3F"/>.</param>
		/// <param name="extents">The extents <see cref="Vector3F"/>.</param>
		/// <param name="axis">The axis <see cref="Matrix3F"/>.</param>
		public BoxF( Vector3F center, Vector3F extents, Matrix3F axis )
		{
			this.Center = center;
			this.Extents = extents;
			this.Axis = axis;
		}

		/// <summary>
		/// Constructs a box with the given center point.
		/// </summary>
		/// <param name="point">The center point <see cref="Vector3F"/>.</param>
		public BoxF( Vector3F point )
		{
			Center = point;
			Extents = Vector3F.Zero;
			Axis = Matrix3F.Identity;
		}

		/// <summary>
		/// Constructs a box with the given bounds.
		/// </summary>
		/// <param name="bounds">The <see cref="BoundsF"/>.</param>
		public BoxF( BoundsF bounds )
		{
			bounds.GetCenter( out Center );
			Vector3F.Subtract( ref bounds.Maximum, ref Center, out Extents );
			Axis = Matrix3F.Identity;
		}

		/// <summary>
		/// Constructs a box with the given bounds, origin and axis.
		/// </summary>
		/// <param name="bounds">The <see cref="BoundsF"/>.</param>
		/// <param name="origin">The origin <see cref="Vector3F"/>.</param>
		/// <param name="axis">The axis <see cref="Matrix3F"/>.</param>
		public BoxF( BoundsF bounds, Vector3F origin, Matrix3F axis )
		{
			Vector3F temp;
			Vector3F temp2;
			bounds.GetCenter( out temp );
			Vector3F.Subtract( ref bounds.Maximum, ref temp, out Extents );
			Matrix3F.Multiply( ref temp, ref axis, out temp2 );
			Vector3F.Add( ref origin, ref temp2, out Center );
			this.Axis = axis;

			//center = bounds.GetCenter();
			//extents = bounds.Maximum - center;
			//center = origin + center * axis;
			//this.axis = axis;
		}

		/// <summary>
		/// Constructs a box with the given bounds, origin and axis.
		/// </summary>
		/// <param name="bounds">The <see cref="BoundsF"/>.</param>
		/// <param name="origin">The origin <see cref="Vector3F"/>.</param>
		/// <param name="axis">The axis <see cref="Matrix3F"/>.</param>
		public BoxF( ref BoundsF bounds, ref Vector3F origin, ref Matrix3F axis )
		{
			Vector3F temp;
			Vector3F temp2;
			bounds.GetCenter( out temp );
			Vector3F.Subtract( ref bounds.Maximum, ref temp, out Extents );
			Matrix3F.Multiply( ref temp, ref axis, out temp2 );
			Vector3F.Add( ref origin, ref temp2, out Center );
			this.Axis = axis;

			//center = bounds.GetCenter();
			//extents = bounds.Maximum - center;
			//center = origin + center * axis;
			//this.axis = axis;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="BoxF"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="BoxF"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="BoxF"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is BoxF && this == (BoxF)obj );
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return ( Center.GetHashCode() ^ Extents.GetHashCode() ^ Axis.GetHashCode() );
		}

		/// <summary>
		/// Determines whether two given boxes are equal.
		/// </summary>
		/// <param name="v1">The first box to compare.</param>
		/// <param name="v2">The second box to compare.</param>
		/// <returns>True if the boxes are equal; False otherwise.</returns>
		public static bool operator ==( BoxF v1, BoxF v2 )
		{
			return ( v1.Center == v2.Center && v1.Extents == v2.Extents && v1.Axis == v2.Axis );
		}

		/// <summary>
		/// Determines whether two given boxes are unequal.
		/// </summary>
		/// <param name="v1">The first box to compare.</param>
		/// <param name="v2">The second box to compare.</param>
		/// <returns>True if the boxes are unequal; False otherwise.</returns>
		public static bool operator !=( BoxF v1, BoxF v2 )
		{
			return ( v1.Center != v2.Center || v1.Extents != v2.Extents || v1.Axis != v2.Axis );
		}

		/// <summary>
		/// Determines whether the specified box is equal to the current instance of <see cref="BoxF"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The box to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified box is equal to the current instance of <see cref="BoxF"/>; False otherwise.</returns>
		public bool Equals( BoxF v, float epsilon )
		{
			if( !Center.Equals( ref v.Center, epsilon ) )
				return false;
			if( !Extents.Equals( ref v.Extents, epsilon ) )
				return false;
			if( !Axis.Equals( ref v.Axis, epsilon ) )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the box is not initialized.
		/// </summary>
		/// <returns>True if the box is not initialized; False otherwise.</returns>
		public bool IsCleared()
		{
			return Extents.X < 0.0f;
		}

		/// <summary>
		/// Returns the volume of the current instance of <see cref="BoxF"/>.
		/// </summary>
		/// <returns>The volume of the box.</returns>
		public float GetVolume()
		{
			float x = Extents.X * 2.0f;
			float y = Extents.Y * 2.0f;
			float z = Extents.Z * 2.0f;
			return x * y * z;
			//return x * x + y * y + z * z;
		}

		/// <summary>
		/// Expands the current instance of <see cref="BoxF"/> by a given value.
		/// </summary>
		/// <param name="d">The value by which to expand.</param>
		public void Expand( float d )
		{
			Extents.X += d;
			Extents.Y += d;
			Extents.Z += d;
		}

		/// <summary>
		/// Addition of a given box and a vector.
		/// </summary>
		/// <param name="b">The <see cref="BoxF"/> to add.</param>
		/// <param name="v">The <see cref="Vector3F"/> to add.</param>
		/// <returns>The resulting box.</returns>
		public static BoxF operator +( BoxF b, Vector3F v )
		{
			BoxF result;
			Vector3F.Add( ref b.Center, ref v, out result.Center );
			result.Extents = b.Extents;
			result.Axis = b.Axis;
			return result;
			//return new Box( b.center + v, b.extents, b.axis );
		}

		/// <summary>
		/// Multiplication of a given box and a matrix.
		/// </summary>
		/// <param name="b">The <see cref="BoxF"/> to multiply.</param>
		/// <param name="m">The <see cref="Matrix3F"/> to multiply.</param>
		/// <returns>The resulting box.</returns>
		public static BoxF operator *( BoxF b, Matrix3F m )
		{
			BoxF result;
			Matrix3F.Multiply( ref b.Center, ref m, out result.Center );
			result.Extents = b.Extents;
			Matrix3F.Multiply( ref b.Axis, ref m, out result.Axis );
			return result;
			//return new Box( b.center * m, b.extents, b.axis * m );
		}

		/// <summary>
		/// Multiplication of a given box and a matrix.
		/// </summary>
		/// <param name="b">The <see cref="BoxF"/> to multiply.</param>
		/// <param name="m">The <see cref="Matrix4F"/> to multiply.</param>
		/// <returns>The resulting box.</returns>
		public static BoxF operator *( BoxF b, Matrix4F m )
		{
			BoxF result;
			Matrix3F m3;
			m.ToMatrix3( out m3 );
			Multiply( ref b, ref m3, out result );
			result.Center.X += m.Item3.X;
			result.Center.Y += m.Item3.Y;
			result.Center.Z += m.Item3.Z;
			return result;
			//return ( b * m.ToMat3() ) + m.mat3.ToVec3();
		}

		/// <summary>
		/// Addition of a given box and a vector.
		/// </summary>
		/// <param name="b">The <see cref="BoxF"/> to add.</param>
		/// <param name="v">The <see cref="Vector3F"/> to add.</param>
		/// <param name="result">When the method completes, contains the resulting box.</param>
		public static void Add( ref BoxF b, ref Vector3F v, out BoxF result )
		{
			Vector3F.Add( ref b.Center, ref v, out result.Center );
			result.Extents = b.Extents;
			result.Axis = b.Axis;
		}

		/// <summary>
		/// Multiplication of a given box and a matrix.
		/// </summary>
		/// <param name="b">The <see cref="BoxF"/> to multiply.</param>
		/// <param name="m">The <see cref="Matrix3F"/> to multiply.</param>
		/// <param name="result">When the method completes, contains the resulting box.</param>
		public static void Multiply( ref BoxF b, ref Matrix3F m, out BoxF result )
		{
			Matrix3F.Multiply( ref b.Center, ref m, out result.Center );
			result.Extents = b.Extents;
			Matrix3F.Multiply( ref b.Axis, ref m, out result.Axis );
		}

		/// <summary>
		/// Multiplication of a given box and a matrix.
		/// </summary>
		/// <param name="b">The <see cref="BoxF"/> to multiply.</param>
		/// <param name="m">The <see cref="Matrix4F"/> to multiply.</param>
		/// <param name="result">When the method completes, contains the resulting box.</param>
		public static void Multiply( ref BoxF b, ref Matrix4F m, out BoxF result )
		{
			Matrix3F m3;
			m.ToMatrix3( out m3 );
			Multiply( ref b, ref m3, out result );
			result.Center.X += m.Item3.X;
			result.Center.Y += m.Item3.Y;
			result.Center.Z += m.Item3.Z;
		}

		/// <summary>
		/// Addition of a given box and a vector.
		/// </summary>
		/// <param name="b">The <see cref="BoxF"/> to add.</param>
		/// <param name="v">The <see cref="Vector3F"/> to add.</param>
		/// <returns>The resulting box.</returns>
		public static BoxF Add( ref BoxF b, ref Vector3F v )
		{
			BoxF result;
			Vector3F.Add( ref b.Center, ref v, out result.Center );
			result.Extents = b.Extents;
			result.Axis = b.Axis;
			return result;
		}

		/// <summary>
		/// Multiplication of a given box and a matrix.
		/// </summary>
		/// <param name="b">The <see cref="BoxF"/> to multiply.</param>
		/// <param name="m">The <see cref="Matrix3F"/> to multiply.</param>
		/// <returns>The resulting box.</returns>
		public static BoxF Multiply( ref BoxF b, ref Matrix3F m )
		{
			BoxF result;
			Multiply( ref b, ref m, out result );
			return result;
		}

		/// <summary>
		/// Multiplication of a given box and a matrix.
		/// </summary>
		/// <param name="b">The <see cref="BoxF"/> to multiply.</param>
		/// <param name="m">The <see cref="Matrix4F"/> to multiply.</param>
		/// <returns>The resulting box.</returns>
		public static BoxF Multiply( ref BoxF b, ref Matrix4F m )
		{
			BoxF result;
			Multiply( ref b, ref m, out result );
			return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="BoxF"/> into the box corners array and returns the result.
		/// </summary>
		/// <returns>The resulting box corners array.</returns>
		public Vector3F[] ToPoints()
		{
			Vector3F[] r = null;
			ToPoints( ref r );
			return r;
		}

		/// <summary>
		/// Converts the current instance of <see cref="BoxF"/> into the box corners array.
		/// </summary>
		/// <param name="points">The array for the box corners.</param>
		public void ToPoints( ref Vector3F[] points )
		{
			if( points == null || points.Length < 8 )
				points = new Vector3F[ 8 ];

			Vector3F axMat0;
			Vector3F.Multiply( Extents.X, ref Axis.Item0, out axMat0 );
			//Vec3 axMat0 = extents.x * axis.mat0;

			Vector3F axMat1;
			Vector3F.Multiply( Extents.Y, ref  Axis.Item1, out axMat1 );
			//Vec3 axMat1 = extents.y * axis.mat1;

			Vector3F axMat2;
			Vector3F.Multiply( Extents.Z, ref Axis.Item2, out axMat2 );
			//Vec3 axMat2 = extents.z * axis.mat2;

			Vector3F temp0;
			Vector3F.Subtract( ref Center, ref axMat0, out temp0 );
			//Vec3 temp0 = center - axMat0;

			Vector3F temp1;
			Vector3F.Add( ref Center, ref axMat0, out temp1 );
			//Vec3 temp1 = center + axMat0;

			Vector3F temp2;
			Vector3F.Subtract( ref axMat1, ref axMat2, out temp2 );
			//Vec3 temp2 = axMat1 - axMat2;

			Vector3F temp3;
			Vector3F.Add( ref axMat1, ref axMat2, out temp3 );
			//Vec3 temp3 = axMat1 + axMat2;

			points[ 0 ] = temp0 - temp3;
			points[ 1 ] = temp1 - temp3;
			points[ 2 ] = temp1 + temp2;
			points[ 3 ] = temp0 + temp2;
			points[ 4 ] = temp0 - temp2;
			points[ 5 ] = temp1 - temp2;
			points[ 6 ] = temp1 + temp3;
			points[ 7 ] = temp0 + temp3;
		}

		/// <summary>
		/// Converts the current instance of <see cref="BoxF"/> into the box corners array.
		/// </summary>
		/// <param name="points">The pointer to an array for the box corners.</param>
		unsafe internal void ToPoints( Vector3F* points )
		{
			Vector3F axMat0;
			Vector3F.Multiply( Extents.X, ref Axis.Item0, out axMat0 );
			//Vec3 axMat0 = extents.x * axis.mat0;

			Vector3F axMat1;
			Vector3F.Multiply( Extents.Y, ref  Axis.Item1, out axMat1 );
			//Vec3 axMat1 = extents.y * axis.mat1;

			Vector3F axMat2;
			Vector3F.Multiply( Extents.Z, ref Axis.Item2, out axMat2 );
			//Vec3 axMat2 = extents.z * axis.mat2;

			Vector3F temp0;
			Vector3F.Subtract( ref Center, ref axMat0, out temp0 );
			//Vec3 temp0 = center - axMat0;

			Vector3F temp1;
			Vector3F.Add( ref Center, ref axMat0, out temp1 );
			//Vec3 temp1 = center + axMat0;

			Vector3F temp2;
			Vector3F.Subtract( ref axMat1, ref axMat2, out temp2 );
			//Vec3 temp2 = axMat1 - axMat2;

			Vector3F temp3;
			Vector3F.Add( ref axMat1, ref axMat2, out temp3 );
			//Vec3 temp3 = axMat1 + axMat2;

			points[ 0 ] = temp0 - temp3;
			points[ 1 ] = temp1 - temp3;
			points[ 2 ] = temp1 + temp2;
			points[ 3 ] = temp0 + temp2;
			points[ 4 ] = temp0 - temp2;
			points[ 5 ] = temp1 - temp2;
			points[ 6 ] = temp1 + temp3;
			points[ 7 ] = temp0 + temp3;
		}

		/// <summary>
		/// Converts the current instance of <see cref="BoxF"/> into the equivalent <see cref="BoundsF"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="BoundsF"/> structure.</returns>
		public BoundsF ToBounds()
		{
			Vector3F halfSize = new Vector3F(
				Math.Abs( Axis.Item0.X * Extents.X ) + Math.Abs( Axis.Item1.X * Extents.Y ) + Math.Abs( Axis.Item2.X * Extents.Z ),
				Math.Abs( Axis.Item0.Y * Extents.X ) + Math.Abs( Axis.Item1.Y * Extents.Y ) + Math.Abs( Axis.Item2.Y * Extents.Z ),
				Math.Abs( Axis.Item0.Z * Extents.X ) + Math.Abs( Axis.Item1.Z * Extents.Y ) + Math.Abs( Axis.Item2.Z * Extents.Z ) );
			return new BoundsF( Center - halfSize, Center + halfSize );

			//Vec3 halfSize =
			//   Vec3Abs( axis.mat0 * extents.x ) +
			//   Vec3Abs( axis.mat1 * extents.y ) +
			//   Vec3Abs( axis.mat2 * extents.z );
			//return new Bounds( center - halfSize, center + halfSize );


			//Bounds result;

			//Vec3 axMat0, axMat1, axMat2;
			//Vec3.Multiply( extents.x, ref axis.mat0, out axMat0 );
			//Vec3.Multiply( extents.y, ref axis.mat1, out axMat1 );
			//Vec3.Multiply( extents.z, ref axis.mat2, out axMat2 );

			//Vec3 temp0, temp1, temp2, temp3;
			//Vec3.Subtract( ref center, ref axMat0, out temp0 );
			//Vec3.Add( ref center, ref axMat0, out temp1 );
			//Vec3.Subtract( ref axMat1, ref axMat2, out temp2 );
			//Vec3.Add( ref axMat1, ref axMat2, out temp3 );

			//Vec3 v;
			//Vec3.Subtract( ref temp0, ref temp3, out v );
			//result.minimum = v;
			//result.maximum = v;

			//Vec3.Subtract( ref temp1, ref temp3, out v );
			//result.Add( ref v );

			//Vec3.Add( ref temp1, ref temp2, out v );
			//result.Add( ref v );

			//Vec3.Add( ref temp0, ref temp2, out v );
			//result.Add( ref v );

			//Vec3.Subtract( ref temp0, ref temp2, out v );
			//result.Add( ref v );

			//Vec3.Subtract( ref temp1, ref temp2, out v );
			//result.Add( ref v );

			//Vec3.Add( ref temp1, ref temp3, out v );
			//result.Add( ref v );

			//Vec3.Add( ref temp0, ref temp3, out v );
			//result.Add( ref v );



			//Vec3 axMat0 = extents.x * axis.mat0;
			//Vec3 axMat1 = extents.y * axis.mat1;
			//Vec3 axMat2 = extents.z * axis.mat2;

			//Vec3 temp0 = new Vec3( center - axMat0 );
			//Vec3 temp1 = new Vec3( center + axMat0 );
			//Vec3 temp2 = new Vec3( axMat1 - axMat2 );
			//Vec3 temp3 = new Vec3( axMat1 + axMat2 );

			//Bounds result = new Bounds( temp0 - temp3 );
			//result.Add( temp1 - temp3 );
			//result.Add( temp1 + temp2 );
			//result.Add( temp0 + temp2 );
			//result.Add( temp0 - temp2 );
			//result.Add( temp1 - temp2 );
			//result.Add( temp1 + temp3 );
			//result.Add( temp0 + temp3 );

			//return result;
		}

		/// <summary>
		/// Converts the current instance of <see cref="BoxF"/> into the equivalent <see cref="BoundsF"/> structure.
		/// </summary>
		/// <param name="result">When the method completes, contains the equivalent <see cref="BoundsF"/> structure.</param>
		public void ToBounds( out BoundsF result )
		{
			Vector3F halfSize = new Vector3F(
				Math.Abs( Axis.Item0.X * Extents.X ) + Math.Abs( Axis.Item1.X * Extents.Y ) + Math.Abs( Axis.Item2.X * Extents.Z ),
				Math.Abs( Axis.Item0.Y * Extents.X ) + Math.Abs( Axis.Item1.Y * Extents.Y ) + Math.Abs( Axis.Item2.Y * Extents.Z ),
				Math.Abs( Axis.Item0.Z * Extents.X ) + Math.Abs( Axis.Item1.Z * Extents.Y ) + Math.Abs( Axis.Item2.Z * Extents.Z ) );
			result.Minimum = Center - halfSize;
			result.Maximum = Center + halfSize;

			//Vec3 axMat0, axMat1, axMat2;
			//Vec3.Multiply( extents.x, ref axis.mat0, out axMat0 );
			//Vec3.Multiply( extents.y, ref axis.mat1, out axMat1 );
			//Vec3.Multiply( extents.z, ref axis.mat2, out axMat2 );

			//Vec3 temp0, temp1, temp2, temp3;
			//Vec3.Subtract( ref center, ref axMat0, out temp0 );
			//Vec3.Add( ref center, ref axMat0, out temp1 );
			//Vec3.Subtract( ref axMat1, ref axMat2, out temp2 );
			//Vec3.Add( ref axMat1, ref axMat2, out temp3 );

			//Vec3 v;
			//Vec3.Subtract( ref temp0, ref temp3, out v );
			//result.minimum = v;
			//result.maximum = v;

			//Vec3.Subtract( ref temp1, ref temp3, out v );
			//result.Add( ref v );

			//Vec3.Add( ref temp1, ref temp2, out v );
			//result.Add( ref v );

			//Vec3.Add( ref temp0, ref temp2, out v );
			//result.Add( ref v );

			//Vec3.Subtract( ref temp0, ref temp2, out v );
			//result.Add( ref v );

			//Vec3.Subtract( ref temp1, ref temp2, out v );
			//result.Add( ref v );

			//Vec3.Add( ref temp1, ref temp3, out v );
			//result.Add( ref v );

			//Vec3.Add( ref temp0, ref temp3, out v );
			//result.Add( ref v );
		}

		/// <summary>
		/// Determines whether the current instance of <see cref="BoxF"/> contains a given point.
		/// </summary>
		/// <param name="point">A point to check.</param>
		/// <returns>True if the current instance of <see cref="BoxF"/> contains a given point; False otherwise.</returns>
		public bool Contains( ref Vector3F point )
		{
			Vector3F localPoint;
			Vector3F.Subtract( ref point, ref Center, out localPoint );
			//Vec3 localPoint = point - center;
			if( Math.Abs( Vector3F.Dot( ref localPoint, ref Axis.Item0 ) ) > Extents.X ||
				Math.Abs( Vector3F.Dot( ref localPoint, ref Axis.Item1 ) ) > Extents.Y ||
				Math.Abs( Vector3F.Dot( ref localPoint, ref Axis.Item2 ) ) > Extents.Z )
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Determines whether the current instance of <see cref="BoxF"/> contains a given point.
		/// </summary>
		/// <param name="point">A point to check.</param>
		/// <returns>True if the current instance of <see cref="BoxF"/> contains a given point; False otherwise.</returns>
		public bool Contains( Vector3F point )
		{
			return Contains( ref point );
		}

		/// <summary>
		/// Determines whether the current instance of <see cref="BoxF"/> contains given bounds.
		/// </summary>
		/// <param name="bounds">Bounds to check.</param>
		/// <returns>True if the current instance of <see cref="BoxF"/> contains given bounds; False otherwise.</returns>
		public bool Contains( ref BoundsF bounds )
		{
			//slowly
			Vector3F p;

			p = new Vector3F( bounds.Minimum.X, bounds.Minimum.Y, bounds.Minimum.Z );
			if( !Contains( ref p ) )
				return false;
			p = new Vector3F( bounds.Minimum.X, bounds.Minimum.Y, bounds.Maximum.Z );
			if( !Contains( ref p ) )
				return false;
			p = new Vector3F( bounds.Minimum.X, bounds.Maximum.Y, bounds.Minimum.Z );
			if( !Contains( ref p ) )
				return false;
			p = new Vector3F( bounds.Minimum.X, bounds.Maximum.Y, bounds.Maximum.Z );
			if( !Contains( ref p ) )
				return false;
			p = new Vector3F( bounds.Maximum.X, bounds.Minimum.Y, bounds.Minimum.Z );
			if( !Contains( ref p ) )
				return false;
			p = new Vector3F( bounds.Maximum.X, bounds.Minimum.Y, bounds.Maximum.Z );
			if( !Contains( ref p ) )
				return false;
			p = new Vector3F( bounds.Maximum.X, bounds.Maximum.Y, bounds.Minimum.Z );
			if( !Contains( ref p ) )
				return false;
			p = new Vector3F( bounds.Maximum.X, bounds.Maximum.Y, bounds.Maximum.Z );
			if( !Contains( ref p ) )
				return false;

			return true;
		}

		/// <summary>
		/// Determines whether the current instance of <see cref="BoxF"/> contains given bounds.
		/// </summary>
		/// <param name="bounds">Bounds to check.</param>
		/// <returns>True if the current instance of <see cref="BoxF"/> contains given bounds; False otherwise.</returns>
		public bool Contains( BoundsF bounds )
		{
			return Contains( ref bounds );
		}

		/// <summary>
		/// Determines whether the current instance of <see cref="BoxF"/> contains the given sphere.
		/// </summary>
		/// <param name="s">The sphere to check.</param>
		/// <returns>True if the current instance of <see cref="BoxF"/> contains the given sphere; False otherwise.</returns>
		public bool Contains( ref SphereF s )
		{
			Vector3F localPoint;
			Vector3F.Subtract( ref s.Center, ref Center, out localPoint );

			float x = Math.Abs( Vector3F.Dot( ref localPoint, ref Axis.Item0 ) ) - Extents.X;
			if( x > 0 || -x < s.Radius )
				return false;
			float y = Math.Abs( Vector3F.Dot( ref localPoint, ref Axis.Item1 ) ) - Extents.Y;
			if( y > 0 || -y < s.Radius )
				return false;
			float z = Math.Abs( Vector3F.Dot( ref localPoint, ref Axis.Item2 ) ) - Extents.Z;
			if( z > 0 || -z < s.Radius )
				return false;

			return true;
		}

		/// <summary>
		/// Determines whether the current instance of <see cref="BoxF"/> contains the given sphere.
		/// </summary>
		/// <param name="s">The sphere to check.</param>
		/// <returns>True if the current instance of <see cref="BoxF"/> contains the given sphere; False otherwise.</returns>
		public bool Contains( SphereF s )
		{
			return Contains( ref s );
		}

		/// <summary>
		/// Determines whether the current instance of <see cref="BoxF"/> contains another instance of <see cref="BoxF"/>.
		/// </summary>
		/// <param name="box">The box to check.</param>
		/// <returns>True if the current instance of <see cref="BoxF"/> contains the given box; False otherwise.</returns>
		public bool Contains( ref BoxF box )
		{
			Vector3F[] points = null;
			box.ToPoints( ref points );
			foreach( Vector3F point in points )
			{
				if( !Contains( point ) )
					return false;
			}
			return true;
		}

		/// <summary>
		/// Determines whether the current instance of <see cref="BoxF"/> contains another instance of <see cref="BoxF"/>.
		/// </summary>
		/// <param name="box">The box to check.</param>
		/// <returns>True if the current instance of <see cref="BoxF"/> contains the given box; False otherwise.</returns>
		public bool Contains( BoxF box )
		{
			return Contains( ref box );
		}

		static bool BoxPlaneClip( float denom, float numer, ref float scale0, ref float scale1 )
		{
			if( denom > 0.0f )
			{
				if( numer > denom * scale1 )
					return false;
				if( numer > denom * scale0 )
					scale0 = numer / denom;
				return true;
			}
			else if( denom < 0.0f )
			{
				if( numer > denom * scale0 )
					return false;
				if( numer > denom * scale1 )
					scale1 = numer / denom;
				return true;
			}
			else
				return ( numer <= 0.0f );
		}

		/// <summary>
		/// Determines whether the given ray intersects the current instance of <see cref="BoxF"/>.
		/// </summary>
		/// <param name="ray">The ray to check.</param>
		/// <param name="scale1">When the method completes, contains the ray and box intersection min point.</param>
		/// <param name="scale2">When the method completes, contains the ray and box intersection max point.</param>
		/// <returns>True if the given ray intersects the current instance of <see cref="BoxF"/>; False otherwise.</returns>
		public bool Intersects( RayF ray, out float scale1, out float scale2 )
		{
			Matrix3F transposedAxis;
			Axis.GetTranspose( out transposedAxis );

			Vector3F localStart;
			Vector3F diff;
			Vector3F.Subtract( ref ray.Origin, ref Center, out diff );
			Matrix3F.Multiply( ref diff, ref transposedAxis, out localStart );
			//Vec3 localStart = ( ray.Origin - center ) * transposedAxis;

			Vector3F localDir;
			Matrix3F.Multiply( ref ray.Direction, ref transposedAxis, out localDir );
			//Vec3 localDir = ray.Direction * transposedAxis;

			float s1 = float.MinValue;
			float s2 = float.MaxValue;
			bool ret =
				BoxPlaneClip( localDir.X, -localStart.X - Extents.X, ref s1, ref s2 ) &&
				BoxPlaneClip( -localDir.X, localStart.X - Extents.X, ref s1, ref s2 ) &&
				BoxPlaneClip( localDir.Y, -localStart.Y - Extents.Y, ref s1, ref s2 ) &&
				BoxPlaneClip( -localDir.Y, localStart.Y - Extents.Y, ref s1, ref s2 ) &&
				BoxPlaneClip( localDir.Z, -localStart.Z - Extents.Z, ref s1, ref s2 ) &&
				BoxPlaneClip( -localDir.Z, localStart.Z - Extents.Z, ref s1, ref s2 );

			scale1 = s1;
			scale2 = s2;

			return ret;
		}

		/// <summary>
		/// Determines whether the given ray intersects the current instance of <see cref="BoxF"/>.
		/// </summary>
		/// <param name="ray">The ray to check.</param>
		/// <returns>True if the given ray intersects the current instance of <see cref="BoxF"/>; False otherwise.</returns>
		public bool Intersects( RayF ray )
		{
			float scale1;
			float scale2;
			return Intersects( ray, out scale1, out scale2 );
		}

		////not used
		//static float[] ld = new float[ 3 ];
		//public bool LineIntersection( Vec3 start, Vec3 end )
		//{
		//   Vec3 lineDir = 0.5f * ( end - start );
		//   Vec3 lineCenter = start + lineDir;
		//   Vec3 dir = lineCenter - center;

		//   ld[ 0 ] = Math.Abs( Vec3.Dot( lineDir, axis[ 0 ] ) );
		//   if( Math.Abs( Vec3.Dot( dir, axis[ 0 ] ) ) > extents[ 0 ] + ld[ 0 ] )
		//      return false;

		//   ld[ 1 ] = Math.Abs( Vec3.Dot( lineDir, axis[ 1 ] ) );
		//   if( Math.Abs( Vec3.Dot( dir, axis[ 1 ] ) ) > extents[ 1 ] + ld[ 1 ] )
		//      return false;

		//   ld[ 2 ] = Math.Abs( Vec3.Dot( lineDir, axis[ 2 ] ) );
		//   if( Math.Abs( Vec3.Dot( dir, axis[ 2 ] ) ) > extents[ 2 ] + ld[ 2 ] )
		//      return false;

		//   Vec3 cross = lineDir.Cross( dir );

		//   if( Math.Abs( Vec3.Dot( cross, axis[ 0 ] ) ) > extents[ 1 ] * ld[ 2 ] + extents[ 2 ] * ld[ 1 ] )
		//      return false;

		//   if( Math.Abs( Vec3.Dot( cross, axis[ 1 ] ) ) > extents[ 0 ] * ld[ 2 ] + extents[ 2 ] * ld[ 0 ] )
		//      return false;

		//   if( Math.Abs( Vec3.Dot( cross, axis[ 2 ] ) ) > extents[ 0 ] * ld[ 1 ] + extents[ 1 ] * ld[ 0 ] )
		//      return false;

		//   return true;
		//}

		/// <summary>
		/// Determines whether another instance of <see cref="BoxF"/> intersects the current instance of <see cref="BoxF"/>.
		/// </summary>
		/// <param name="box">The box to check.</param>
		/// <returns>True if another instance of <see cref="BoxF"/> intersects the current instance of <see cref="BoxF"/>; False otherwise.</returns>
		public bool Intersects( ref BoxF box )
		{
			float c00, c01, c02, c10, c11, c12, c20, c21, c22;
			//float c[3][3];		// matrix c = axis.Transpose() * a.axis
			float ac00, ac01, ac02, ac10, ac11, ac12, ac20, ac21, ac22;
			//float ac[3][3];		// absolute values of c

			Vector3F axisDir = Vector3F.Zero;	// axis[i] * dir
			float d, e0, e1;	// distance between centers and projected extents

			Vector3F dir;
			Vector3F.Subtract( ref box.Center, ref Center, out dir );
			//Vec3 dir = b.center - center;

			// axis C0 + t * A0
			c00 = Vector3F.Dot( ref Axis.Item0, ref box.Axis.Item0 );
			c01 = Vector3F.Dot( ref Axis.Item0, ref box.Axis.Item1 );
			c02 = Vector3F.Dot( ref Axis.Item0, ref box.Axis.Item2 );
			axisDir.X = Vector3F.Dot( ref Axis.Item0, ref dir );
			ac00 = Math.Abs( c00 );
			ac01 = Math.Abs( c01 );
			ac02 = Math.Abs( c02 );

			d = Math.Abs( axisDir.X );
			e0 = Extents.X;
			e1 = box.Extents.X * ac00 + box.Extents.Y * ac01 + box.Extents.Z * ac02;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A1
			c10 = Vector3F.Dot( ref Axis.Item1, ref box.Axis.Item0 );
			c11 = Vector3F.Dot( ref  Axis.Item1, ref box.Axis.Item1 );
			c12 = Vector3F.Dot( ref Axis.Item1, ref box.Axis.Item2 );
			axisDir.Y = Vector3F.Dot( ref Axis.Item1, ref  dir );
			ac10 = Math.Abs( c10 );
			ac11 = Math.Abs( c11 );
			ac12 = Math.Abs( c12 );

			d = Math.Abs( axisDir.Y );
			e0 = Extents.Y;
			e1 = box.Extents.X * ac10 + box.Extents.Y * ac11 + box.Extents.Z * ac12;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A2
			c20 = Vector3F.Dot( ref Axis.Item2, ref box.Axis.Item0 );
			c21 = Vector3F.Dot( ref Axis.Item2, ref box.Axis.Item1 );
			c22 = Vector3F.Dot( ref Axis.Item2, ref box.Axis.Item2 );
			axisDir.Z = Vector3F.Dot( ref  Axis.Item2, ref dir );
			ac20 = Math.Abs( c20 );
			ac21 = Math.Abs( c21 );
			ac22 = Math.Abs( c22 );

			d = Math.Abs( axisDir.Z );
			e0 = Extents.Z;
			e1 = box.Extents.X * ac20 + box.Extents.Y * ac21 + box.Extents.Z * ac22;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * B0
			d = Math.Abs( Vector3F.Dot( ref  box.Axis.Item0, ref  dir ) );
			e0 = Extents.X * ac00 + Extents.Y * ac10 + Extents.Z * ac20;
			e1 = box.Extents.X;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * B1
			d = Math.Abs( Vector3F.Dot( ref  box.Axis.Item1, ref dir ) );
			e0 = Extents.X * ac01 + Extents.Y * ac11 + Extents.Z * ac21;
			e1 = box.Extents.Y;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * B2
			d = Math.Abs( Vector3F.Dot( ref box.Axis.Item2, ref dir ) );
			e0 = Extents.X * ac02 + Extents.Y * ac12 + Extents.Z * ac22;
			e1 = box.Extents.Z;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A0xB0
			d = Math.Abs( axisDir.Z * c10 - axisDir.Y * c20 );
			e0 = Extents.Y * ac20 + Extents.Z * ac10;
			e1 = box.Extents.Y * ac02 + box.Extents.Z * ac01;
			if( d > e0 + e1 )
			{
				return false;
			}

			// axis C0 + t * A0xB1
			d = Math.Abs( axisDir.Z * c11 - axisDir.Y * c21 );
			e0 = Extents.Y * ac21 + Extents.Z * ac11;
			e1 = box.Extents.X * ac02 + box.Extents.Z * ac00;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A0xB2
			d = Math.Abs( axisDir.Z * c12 - axisDir.Y * c22 );
			e0 = Extents.Y * ac22 + Extents.Z * ac12;
			e1 = box.Extents.X * ac01 + box.Extents.Y * ac00;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A1xB0
			d = Math.Abs( axisDir.X * c20 - axisDir.Z * c00 );
			e0 = Extents.X * ac20 + Extents.Z * ac00;
			e1 = box.Extents.Y * ac12 + box.Extents.Z * ac11;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A1xB1
			d = Math.Abs( axisDir.X * c21 - axisDir.Z * c01 );
			e0 = Extents.X * ac21 + Extents.Z * ac01;
			e1 = box.Extents.X * ac12 + box.Extents.Z * ac10;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A1xB2
			d = Math.Abs( axisDir.X * c22 - axisDir.Z * c02 );
			e0 = Extents.X * ac22 + Extents.Z * ac02;
			e1 = box.Extents.X * ac11 + box.Extents.Y * ac10;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A2xB0
			d = Math.Abs( axisDir.Y * c00 - axisDir.X * c10 );
			e0 = Extents.X * ac10 + Extents.Y * ac00;
			e1 = box.Extents.Y * ac22 + box.Extents.Z * ac21;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A2xB1
			d = Math.Abs( axisDir.Y * c01 - axisDir.X * c11 );
			e0 = Extents.X * ac11 + Extents.Y * ac01;
			e1 = box.Extents.X * ac22 + box.Extents.Z * ac20;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A2xB2
			d = Math.Abs( axisDir.Y * c02 - axisDir.X * c12 );
			e0 = Extents.X * ac12 + Extents.Y * ac02;
			e1 = box.Extents.X * ac21 + box.Extents.Y * ac20;
			if( d > e0 + e1 )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether another instance of <see cref="BoxF"/> intersects the current instance of <see cref="BoxF"/>.
		/// </summary>
		/// <param name="box">The box to check.</param>
		/// <returns>True if another instance of <see cref="BoxF"/> intersects the current instance of <see cref="BoxF"/>; False otherwise.</returns>
		public bool Intersects( BoxF box )
		{
			return Intersects( ref box );
		}

		/// <summary>
		/// Determines whether the given bounds intersect the current instance of <see cref="BoxF"/>.
		/// </summary>
		/// <param name="bounds">The bounds to check.</param>
		/// <returns>True if the given bounds intersect the current instance of <see cref="BoxF"/>; False otherwise.</returns>
		public bool Intersects( ref BoundsF bounds )
		{
			float c00, c01, c02, c10, c11, c12, c20, c21, c22;
			//float c[3][3]; // matrix c = axis.Transpose() * a.axis
			float ac00, ac01, ac02, ac10, ac11, ac12, ac20, ac21, ac22;
			//float ac[3][3]; // absolute values of c

			float d, e0, e1;	// distance between centers and projected extents

			Vector3F boundsCenter;
			bounds.GetCenter( out boundsCenter );
			//Vec3 boundsCenter = bounds.GetCenter();

			// vector between centers
			Vector3F dir;
			Vector3F.Subtract( ref boundsCenter, ref Center, out dir );
			//Vec3 dir = boundsCenter - center;

			Vector3F axisDir = Vector3F.Zero;// axis[ i ] * dir

			Vector3F boundsExtents;
			Vector3F.Subtract( ref bounds.Maximum, ref boundsCenter, out boundsExtents );
			//Vec3 boundsExtents = bounds.Maximum - boundsCenter;

			// axis C0 + t * A0
			c00 = Axis.Item0.X;//Vec3.Dot( axis[ 0 ], Vec3.XAxis );
			c01 = Axis.Item0.Y;//Vec3.Dot( axis[ 0 ], Vec3.YAxis );
			c02 = Axis.Item0.Z;//Vec3.Dot( axis[ 0 ], Vec3.ZAxis );
			axisDir.X = Vector3F.Dot( ref Axis.Item0, ref dir );
			ac00 = Math.Abs( c00 );
			ac01 = Math.Abs( c01 );
			ac02 = Math.Abs( c02 );

			d = Math.Abs( axisDir.X );
			e0 = Extents.X;
			e1 = boundsExtents.X * ac00 + boundsExtents.Y * ac01 + boundsExtents.Z * ac02;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A1
			c10 = Axis.Item1.X;// Vec3.Dot( axis[ 1 ], Vec3.XAxis );
			c11 = Axis.Item1.Y;//Vec3.Dot( axis[ 1 ], Vec3.YAxis );
			c12 = Axis.Item1.Z;//Vec3.Dot( axis[ 1 ], Vec3.ZAxis );
			axisDir.Y = Vector3F.Dot( ref Axis.Item1, ref dir );
			ac10 = Math.Abs( c10 );
			ac11 = Math.Abs( c11 );
			ac12 = Math.Abs( c12 );

			d = Math.Abs( axisDir.Y );
			e0 = Extents.Y;
			e1 = boundsExtents.X * ac10 + boundsExtents.Y * ac11 + boundsExtents.Z * ac12;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A2
			c20 = Axis.Item2.X;// Vec3.Dot( axis[ 2 ], Vec3.XAxis );
			c21 = Axis.Item2.Y;// Vec3.Dot( axis[ 2 ], Vec3.YAxis );
			c22 = Axis.Item2.Z;// Vec3.Dot( axis[ 2 ], Vec3.ZAxis );
			axisDir.Z = Vector3F.Dot( ref Axis.Item2, ref dir );
			ac20 = Math.Abs( c20 );
			ac21 = Math.Abs( c21 );
			ac22 = Math.Abs( c22 );

			d = Math.Abs( axisDir.Z );
			e0 = Extents.Z;
			e1 = boundsExtents.X * ac20 + boundsExtents.Y * ac21 + boundsExtents.Z * ac22;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * B0
			d = Math.Abs( dir.X );//Vec3.Dot( Vec3.XAxis, dir ) );
			e0 = Extents.X * ac00 + Extents.Y * ac10 + Extents.Z * ac20;
			e1 = boundsExtents.X;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * B1
			d = Math.Abs( dir.Y );//Vec3.Dot( Vec3.YAxis, dir ) );
			e0 = Extents.X * ac01 + Extents.Y * ac11 + Extents.Z * ac21;
			e1 = boundsExtents.Y;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * B2
			d = Math.Abs( dir.Z );//Vec3.Dot( Vec3.ZAxis, dir ) );
			e0 = Extents.X * ac02 + Extents.Y * ac12 + Extents.Z * ac22;
			e1 = boundsExtents.Z;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A0xB0
			d = Math.Abs( axisDir.Z * c10 - axisDir.Y * c20 );
			e0 = Extents.Y * ac20 + Extents.Z * ac10;
			e1 = boundsExtents.Y * ac02 + boundsExtents.Z * ac01;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A0xB1
			d = Math.Abs( axisDir.Z * c11 - axisDir.Y * c21 );
			e0 = Extents.Y * ac21 + Extents.Z * ac11;
			e1 = boundsExtents.X * ac02 + boundsExtents.Z * ac00;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A0xB2
			d = Math.Abs( axisDir.Z * c12 - axisDir.Y * c22 );
			e0 = Extents.Y * ac22 + Extents.Z * ac12;
			e1 = boundsExtents.X * ac01 + boundsExtents.Y * ac00;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A1xB0
			d = Math.Abs( axisDir.X * c20 - axisDir.Z * c00 );
			e0 = Extents.X * ac20 + Extents.Z * ac00;
			e1 = boundsExtents.Y * ac12 + boundsExtents.Z * ac11;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A1xB1
			d = Math.Abs( axisDir.X * c21 - axisDir.Z * c01 );
			e0 = Extents.X * ac21 + Extents.Z * ac01;
			e1 = boundsExtents.X * ac12 + boundsExtents.Z * ac10;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A1xB2
			d = Math.Abs( axisDir.X * c22 - axisDir.Z * c02 );
			e0 = Extents.X * ac22 + Extents.Z * ac02;
			e1 = boundsExtents.X * ac11 + boundsExtents.Y * ac10;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A2xB0
			d = Math.Abs( axisDir.Y * c00 - axisDir.X * c10 );
			e0 = Extents.X * ac10 + Extents.Y * ac00;
			e1 = boundsExtents.Y * ac22 + boundsExtents.Z * ac21;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A2xB1
			d = Math.Abs( axisDir.Y * c01 - axisDir.X * c11 );
			e0 = Extents.X * ac11 + Extents.Y * ac01;
			e1 = boundsExtents.X * ac22 + boundsExtents.Z * ac20;
			if( d > e0 + e1 )
				return false;

			// axis C0 + t * A2xB2
			d = Math.Abs( axisDir.Y * c02 - axisDir.X * c12 );
			e0 = Extents.X * ac12 + Extents.Y * ac02;
			e1 = boundsExtents.X * ac21 + boundsExtents.Y * ac20;
			if( d > e0 + e1 )
				return false;

			return true;
		}

		/// <summary>
		/// Determines whether the given bounds intersect the current instance of <see cref="BoxF"/>.
		/// </summary>
		/// <param name="bounds">The bounds to check.</param>
		/// <returns>True if the given bounds intersect the current instance of <see cref="BoxF"/>; False otherwise.</returns>
		public bool Intersects( BoundsF bounds )
		{
			return Intersects( ref bounds );
		}

		/// <summary>
		/// Determines from which side of the plane the box is on.
		/// </summary>
		/// <param name="plane">The plane to check against.</param>
		/// <returns>The resulting side of the plane.</returns>
		public PlaneF.Side GetPlaneSide( ref PlaneF plane )
		{
			Vector3F localNormal;
			Matrix3F transposedAxis;
			Axis.GetTranspose( out transposedAxis );
			Vector3F normal = plane.Normal;
			Matrix3F.Multiply( ref normal, ref transposedAxis, out localNormal );
			//Vec3 localNormal = plane.Normal * axis.GetTranspose();

			float d1 = plane.GetDistance( ref Center );
			float d2 = Math.Abs( Extents.X * localNormal.X ) +
				Math.Abs( Extents.Y * localNormal.Y ) +
				Math.Abs( Extents.Z * localNormal.Z );

			if( d1 - d2 > 0 )
				return PlaneF.Side.Positive;
			if( d1 + d2 < 0 )
				return PlaneF.Side.Negative;
			return PlaneF.Side.No;
		}

		/// <summary>
		/// Determines from which side of the plane the box is on.
		/// </summary>
		/// <param name="plane">The plane to check against.</param>
		/// <returns>The resulting side of the plane.</returns>
		public PlaneF.Side GetPlaneSide( PlaneF plane )
		{
			return GetPlaneSide( ref plane );
		}

		/// <summary>
		/// Calculates the distance between the current instance of <see cref="BoxF"/> and the given plane.
		/// </summary>
		/// <param name="plane">The plane to calculate the distance from.</param>
		/// <returns>The resulting distance.</returns>
		public float GetPlaneDistance( ref PlaneF plane )
		{
			Vector3F localNormal;
			Matrix3F transposedAxis;
			Axis.GetTranspose( out transposedAxis );
			Vector3F normal = plane.Normal;
			Matrix3F.Multiply( ref normal, ref transposedAxis, out localNormal );
			//Vec3 localNormal = plane.Normal * axis.GetTranspose();

			float d1 = plane.GetDistance( ref Center );
			float d2 = Math.Abs( Extents.X * localNormal.X ) +
				Math.Abs( Extents.Y * localNormal.Y ) +
				Math.Abs( Extents.Z * localNormal.Z );

			if( d1 - d2 > 0 )
				return d1 - d2;
			if( d1 + d2 < 0 )
				return d1 + d2;
			return 0;
		}

		/// <summary>
		/// Calculates the distance between the current instance of <see cref="BoxF"/> and the given plane.
		/// </summary>
		/// <param name="plane">The plane to calculate the distance from.</param>
		/// <returns>The resulting distance.</returns>
		public float GetPlaneDistance( PlaneF plane )
		{
			return GetPlaneDistance( ref plane );
		}

		/// <summary>
		/// Calculates the squared distance between the current instance of <see cref="BoxF"/> and the given point.
		/// </summary>
		/// <param name="point">The point to calculate the squared distance from.</param>
		/// <returns>The resulting distance.</returns>
		public float GetPointDistanceSquared( Vector3F point )
		{
			Vector3F localPoint;
			Vector3F.Subtract( ref point, ref Center, out localPoint );
			//Vec3 localPoint = point - center;

			float sqr = 0;

			float x = Math.Abs( Vector3F.Dot( ref localPoint, ref Axis.Item0 ) ) - Extents.X;
			if( x > 0 )
				sqr += x * x;
			float y = Math.Abs( Vector3F.Dot( ref localPoint, ref Axis.Item1 ) ) - Extents.Y;
			if( y > 0 )
				sqr += y * y;
			float z = Math.Abs( Vector3F.Dot( ref localPoint, ref Axis.Item2 ) ) - Extents.Z;
			if( z > 0 )
				sqr += z * z;

			return sqr;
		}

		/// <summary>
		/// Calculates the distance between the current instance of <see cref="BoxF"/> and the given point.
		/// </summary>
		/// <param name="point">The point to calculate the distance from.</param>
		/// <returns>The resulting distance.</returns>
		public float GetPointDistance( Vector3F point )
		{
			float sqr = GetPointDistanceSquared( point );
			if( sqr == 0 )
				return 0;
			return MathEx.Sqrt( sqr );
		}

		/// <summary>
		/// Converts the current instance of <see cref="BoxF"/> to the box of <see cref="Box"/> format.
		/// </summary>
		/// <returns>The box of <see cref="Box"/> format.</returns>
		[AutoConvertType]
		public Box ToBox()
		{
			Box result;
			result.Center = Center.ToVector3();
			result.Extents = Extents.ToVector3();
			result.Axis = Axis.ToMatrix3();
			return result;
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString

#if !DISABLE_IMPLICIT
		/// <summary>
		/// Implicit conversion from <see cref="BoxF"/> type to <see cref="Box"/> type for the given value.
		/// </summary>
		/// <param name="v">The value to type convert.</param>
		public static implicit operator Box( BoxF v )
		{
			return new Box( v );
		}
#endif
	}
}
