//// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Diagnostics;
//using System.ComponentModel;
//using System.Runtime.InteropServices;

//namespace NeoAxis
//{
//	//[TypeConverter( typeof( MathExGeneralTypeConverter<Box> ) )]
//	/// <summary>
//	/// Represents a double precision oriented bounding box in three dimensional space.
//	/// </summary>
//	[StructLayout( LayoutKind.Sequential )]
//	public struct Box2
//	{
//		/// <summary>
//		/// The center of the box.
//		/// </summary>
//		[Serialize]
//		public Vector2 Center;
//		/// <summary>
//		/// The extents of the box.
//		/// </summary>
//		[Serialize]
//		public Vector2 Extents;
//		/// <summary>
//		/// Axis of the box.
//		/// </summary>
//		[Serialize]
//		public Matrix2 Axis;

//		/// <summary>
//		/// The box with zero size.
//		/// </summary>
//		public static readonly Box2 Zero = new Box2( Vector2.Zero, Vector2.Zero, Matrix2.Identity );

//		/// <summary>
//		/// Not initialized box.
//		/// </summary>
//		public static readonly Box2 Cleared = new Box2(
//			Vector2.Zero, new Vector2( double.MinValue, double.MinValue ), Matrix2.Identity );

//		/// <summary>
//		/// Constructs a box with another specified <see cref="Box2"/> object.
//		/// </summary>
//		/// <param name="source">A specified box.</param>
//		public Box2( Box2 source )
//		{
//			Center = source.Center;
//			Extents = source.Extents;
//			Axis = source.Axis;
//		}

//		/// <summary>
//		/// Constructs a box with the given center, extents and axis.
//		/// </summary>
//		/// <param name="center">The center <see cref="Vector2"/>.</param>
//		/// <param name="extents">The extents <see cref="Vector2"/>.</param>
//		/// <param name="axis">The axis <see cref="Matrix3"/>.</param>
//		public Box2( Vector2 center, Vector2 extents, Matrix2 axis )
//		{
//			this.Center = center;
//			this.Extents = extents;
//			this.Axis = axis;
//		}

//		/// <summary>
//		/// Constructs a box with the given center point.
//		/// </summary>
//		/// <param name="point">The center point <see cref="Vector3"/>.</param>
//		public Box2( Vector2 point )
//		{
//			Center = point;
//			Extents = Vector2.Zero;
//			Axis = Matrix2.Identity;
//		}

//		/// <summary>
//		/// Constructs a box with the given bounds.
//		/// </summary>
//		/// <param name="bounds">The <see cref="Bounds"/>.</param>
//		public Box2( Rectangle bounds )
//		{
//			xx xx;

//			bounds.GetCenter( out Center );
//			Vector2.Subtract( ref bounds.Maximum, ref Center, out Extents );
//			Axis = Matrix2.Identity;
//		}

//		/// <summary>
//		/// Constructs a box with the given bounds, origin and axis.
//		/// </summary>
//		/// <param name="bounds">The <see cref="Bounds"/>.</param>
//		/// <param name="origin">The origin <see cref="Vector2"/>.</param>
//		/// <param name="axis">The axis <see cref="Matrix2"/>.</param>
//		public Box2( Rectangle bounds, Vector2 origin, Matrix2 axis )
//		{
//			xx xx;

//			Vector3 temp;
//			Vector3 temp2;
//			bounds.GetCenter( out temp );
//			Vector3.Subtract( ref bounds.Maximum, ref temp, out Extents );
//			Matrix3.Multiply( ref temp, ref axis, out temp2 );
//			Vector3.Add( ref origin, ref temp2, out Center );
//			this.Axis = axis;

//			//center = bounds.GetCenter();
//			//extents = bounds.Maximum - center;
//			//center = origin + center * axis;
//			//this.axis = axis;
//		}

//		/// <summary>
//		/// Constructs a box with the given bounds, origin and axis.
//		/// </summary>
//		/// <param name="bounds">The <see cref="Bounds"/>.</param>
//		/// <param name="origin">The origin <see cref="Vector3"/>.</param>
//		/// <param name="axis">The axis <see cref="Matrix3"/>.</param>
//		public Box2( ref Rectangle bounds, ref Vector2 origin, ref Matrix2 axis )
//		{
//			xx xx;

//			Vector3 temp;
//			Vector3 temp2;
//			bounds.GetCenter( out temp );
//			Vector3.Subtract( ref bounds.Maximum, ref temp, out Extents );
//			Matrix3.Multiply( ref temp, ref axis, out temp2 );
//			Vector3.Add( ref origin, ref temp2, out Center );
//			this.Axis = axis;

//			//center = bounds.GetCenter();
//			//extents = bounds.Maximum - center;
//			//center = origin + center * axis;
//			//this.axis = axis;
//		}

//		/// <summary>
//		/// Constructs a box with another specified box of <see cref="BoxF"/> format.
//		/// </summary>
//		/// <param name="source">A specified box.</param>
//		public Box2( Box2F source )
//		{
//			Center = source.Center.ToVector3();
//			Extents = source.Extents.ToVector3();
//			Axis = source.Axis.ToMatrix3();
//		}

//		/// <summary>
//		/// Determines whether the specified object is equal to the current instance of <see cref="Box2"/>.
//		/// </summary>
//		/// <param name="obj">The object to compare with the current instance of <see cref="Box2"/>.</param>
//		/// <returns>True if the specified object is equal to the current instance of <see cref="Box2"/>; otherwise, False.</returns>
//		public override bool Equals( object obj )
//		{
//			return ( obj is Box2 && this == (Box2)obj );
//		}

//		/// <summary>
//		/// Returns a hash code for this instance.
//		/// </summary>
//		/// <returns>A hash code for this instance.</returns>
//		public override int GetHashCode()
//		{
//			return ( Center.GetHashCode() ^ Extents.GetHashCode() ^ Axis.GetHashCode() );
//		}

//		/// <summary>
//		/// Determines whether two given boxes are equal.
//		/// </summary>
//		/// <param name="v1">The first box to compare.</param>
//		/// <param name="v2">The second box to compare.</param>
//		/// <returns>True if the boxes are equal; False otherwise.</returns>
//		public static bool operator ==( Box2 v1, Box2 v2 )
//		{
//			return ( v1.Center == v2.Center && v1.Extents == v2.Extents && v1.Axis == v2.Axis );
//		}

//		/// <summary>
//		/// Determines whether two given boxes are unequal.
//		/// </summary>
//		/// <param name="v1">The first box to compare.</param>
//		/// <param name="v2">The second box to compare.</param>
//		/// <returns>True if the boxes are unequal; False otherwise.</returns>
//		public static bool operator !=( Box2 v1, Box2 v2 )
//		{
//			return ( v1.Center != v2.Center || v1.Extents != v2.Extents || v1.Axis != v2.Axis );
//		}

//		/// <summary>
//		/// Determines whether the specified box is equal to the current instance of <see cref="Box2"/>
//		/// with a given precision.
//		/// </summary>
//		/// <param name="v">The box to compare.</param>
//		/// <param name="epsilon">The precision value.</param>
//		/// <returns>True if the specified box is equal to the current instance of <see cref="Box2"/>; False otherwise.</returns>
//		public bool Equals( Box2 v, double epsilon )
//		{
//			if( !Center.Equals( ref v.Center, epsilon ) )
//				return false;
//			if( !Extents.Equals( ref v.Extents, epsilon ) )
//				return false;
//			if( !Axis.Equals( ref v.Axis, epsilon ) )
//				return false;
//			return true;
//		}

//		/// <summary>
//		/// Determines whether the box is not initialized.
//		/// </summary>
//		/// <returns>True if the box is not initialized; False otherwise.</returns>
//		public bool IsCleared()
//		{
//			return Extents.X < 0.0;
//		}

//		/// <summary>
//		/// Returns the volume of the current instance of <see cref="Box2"/>.
//		/// </summary>
//		/// <returns>The volume of the box.</returns>
//		public double GetVolume()
//		{
//			double x = Extents.X * 2.0;
//			double y = Extents.Y * 2.0;
//			double z = Extents.Z * 2.0;
//			return x * y * z;
//			//return x * x + y * y + z * z;
//		}

//		/// <summary>
//		/// Expands the current instance of <see cref="Box2"/> by a given value.
//		/// </summary>
//		/// <param name="d">The value by which to expand.</param>
//		public void Expand( double d )
//		{
//			Extents.X += d;
//			Extents.Y += d;
//			Extents.Z += d;
//		}

//		/// <summary>
//		/// Addition of a given box and a vector.
//		/// </summary>
//		/// <param name="b">The <see cref="Box2"/> to add.</param>
//		/// <param name="v">The <see cref="Vector3"/> to add.</param>
//		/// <returns>The resulting box.</returns>
//		public static Box2 operator +( Box2 b, Vector2 v )
//		{
//			Box result;
//			Vector3.Add( ref b.Center, ref v, out result.Center );
//			result.Extents = b.Extents;
//			result.Axis = b.Axis;
//			return result;
//			//return new Box2( b.center + v, b.extents, b.axis );
//		}

//		/// <summary>
//		/// Multiplication of a given box and a matrix.
//		/// </summary>
//		/// <param name="b">The <see cref="Box2"/> to multiply.</param>
//		/// <param name="m">The <see cref="Matrix2"/> to multiply.</param>
//		/// <returns>The resulting box.</returns>
//		public static Box2 operator *( Box2 b, Matrix2 m )
//		{
//			Box result;
//			Matrix3.Multiply( ref b.Center, ref m, out result.Center );
//			result.Extents = b.Extents;
//			Matrix3.Multiply( ref b.Axis, ref m, out result.Axis );
//			return result;
//			//return new Box( b.center * m, b.extents, b.axis * m );
//		}

//		/// <summary>
//		/// Multiplication of a given box and a matrix.
//		/// </summary>
//		/// <param name="b">The <see cref="Box2"/> to multiply.</param>
//		/// <param name="m">The <see cref="Matrix4"/> to multiply.</param>
//		/// <returns>The resulting box.</returns>
//		public static Box2 operator *( Box2 b, Matrix4 m )
//		{
//			Box result;
//			Matrix3 m3;
//			m.ToMatrix3( out m3 );
//			Multiply( ref b, ref m3, out result );
//			result.Center.X += m.Item3.X;
//			result.Center.Y += m.Item3.Y;
//			result.Center.Z += m.Item3.Z;
//			return result;
//			//return ( b * m.ToMat3() ) + m.mat3.ToVec3D();
//		}

//		/// <summary>
//		/// Addition of a given box and a vector.
//		/// </summary>
//		/// <param name="b">The <see cref="Box2"/> to add.</param>
//		/// <param name="v">The <see cref="Vector3"/> to add.</param>
//		/// <param name="result">When the method completes, contains the resulting box.</param>
//		public static void Add( ref Box2 b, ref Vector2 v, out Box2 result )
//		{
//			Vector3.Add( ref b.Center, ref v, out result.Center );
//			result.Extents = b.Extents;
//			result.Axis = b.Axis;
//		}

//		/// <summary>
//		/// Multiplication of a given box and a matrix.
//		/// </summary>
//		/// <param name="b">The <see cref="Box2"/> to multiply.</param>
//		/// <param name="m">The <see cref="Matrix2"/> to multiply.</param>
//		/// <param name="result">When the method completes, contains the resulting box.</param>
//		public static void Multiply( ref Box2 b, ref Matrix2 m, out Box2 result )
//		{
//			Matrix3.Multiply( ref b.Center, ref m, out result.Center );
//			result.Extents = b.Extents;
//			Matrix3.Multiply( ref b.Axis, ref m, out result.Axis );
//		}

//		/// <summary>
//		/// Multiplication of a given box and a matrix.
//		/// </summary>
//		/// <param name="b">The <see cref="Box2"/> to multiply.</param>
//		/// <param name="m">The <see cref="Matrix4"/> to multiply.</param>
//		/// <param name="result">When the method completes, contains the resulting box.</param>
//		public static void Multiply( ref Box2 b, ref Matrix4 m, out Box2 result )
//		{
//			Matrix3 m3;
//			m.ToMatrix3( out m3 );
//			Multiply( ref b, ref m3, out result );
//			result.Center.X += m.Item3.X;
//			result.Center.Y += m.Item3.Y;
//			result.Center.Z += m.Item3.Z;
//		}

//		/// <summary>
//		/// Addition of a given box and a vector.
//		/// </summary>
//		/// <param name="b">The <see cref="Box2"/> to add.</param>
//		/// <param name="v">The <see cref="Vector2"/> to add.</param>
//		/// <returns>The resulting box.</returns>
//		public static Box2 Add( ref Box2 b, ref Vector2 v )
//		{
//			Box result;
//			Vector3.Add( ref b.Center, ref v, out result.Center );
//			result.Extents = b.Extents;
//			result.Axis = b.Axis;
//			return result;
//		}

//		/// <summary>
//		/// Multiplication of a given box and a matrix.
//		/// </summary>
//		/// <param name="b">The <see cref="Box2"/> to multiply.</param>
//		/// <param name="m">The <see cref="Matrix2"/> to multiply.</param>
//		/// <returns>The resulting box.</returns>
//		public static Box2 Multiply( ref Box2 b, ref Matrix2 m )
//		{
//			Box result;
//			Multiply( ref b, ref m, out result );
//			return result;
//		}

//		/// <summary>
//		/// Multiplication of a given box and a matrix.
//		/// </summary>
//		/// <param name="b">The <see cref="Box2"/> to multiply.</param>
//		/// <param name="m">The <see cref="Matrix4"/> to multiply.</param>
//		/// <returns>The resulting box.</returns>
//		public static Box2 Multiply( ref Box2 b, ref Matrix4 m )
//		{
//			Box result;
//			Multiply( ref b, ref m, out result );
//			return result;
//		}

//		/// <summary>
//		/// Converts the current instance of <see cref="Box2"/> into the box corners array and returns the result.
//		/// </summary>
//		/// <returns>The resulting box corners array.</returns>
//		public Vector2[] ToPoints()
//		{
//			Vector2[] r = null;
//			ToPoints( ref r );
//			return r;
//		}

//		/// <summary>
//		/// Converts the current instance of <see cref="Box2"/> into the box corners array.
//		/// </summary>
//		/// <param name="points">The array for the box corners.</param>
//		public void ToPoints( ref Vector2[] points )
//		{
//			xxx xx;

//			if( points == null || points.Length < 8 )
//				points = new Vector3[ 8 ];

//			Vector3 axMat0;
//			Vector3.Multiply( Extents.X, ref Axis.Item0, out axMat0 );
//			//Vec3D axMat0 = extents.x * axis.mat0;

//			Vector3 axMat1;
//			Vector3.Multiply( Extents.Y, ref  Axis.Item1, out axMat1 );
//			//Vec3D axMat1 = extents.y * axis.mat1;

//			Vector3 axMat2;
//			Vector3.Multiply( Extents.Z, ref Axis.Item2, out axMat2 );
//			//Vec3D axMat2 = extents.z * axis.mat2;

//			Vector3 temp0;
//			Vector3.Subtract( ref Center, ref axMat0, out temp0 );
//			//Vec3D temp0 = center - axMat0;

//			Vector3 temp1;
//			Vector3.Add( ref Center, ref axMat0, out temp1 );
//			//Vec3D temp1 = center + axMat0;

//			Vector3 temp2;
//			Vector3.Subtract( ref axMat1, ref axMat2, out temp2 );
//			//Vec3D temp2 = axMat1 - axMat2;

//			Vector3 temp3;
//			Vector3.Add( ref axMat1, ref axMat2, out temp3 );
//			//Vec3D temp3 = axMat1 + axMat2;

//			points[ 0 ] = temp0 - temp3;
//			points[ 1 ] = temp1 - temp3;
//			points[ 2 ] = temp1 + temp2;
//			points[ 3 ] = temp0 + temp2;
//			points[ 4 ] = temp0 - temp2;
//			points[ 5 ] = temp1 - temp2;
//			points[ 6 ] = temp1 + temp3;
//			points[ 7 ] = temp0 + temp3;
//		}

//		/// <summary>
//		/// Converts the current instance of <see cref="Box2"/> into the box corners array.
//		/// </summary>
//		/// <param name="points">The pointer to an array for the box corners.</param>
//		unsafe internal void ToPoints( Vector2* points )
//		{
//			Vector3 axMat0;
//			Vector3.Multiply( Extents.X, ref Axis.Item0, out axMat0 );
//			//Vec3D axMat0 = extents.x * axis.mat0;

//			Vector3 axMat1;
//			Vector3.Multiply( Extents.Y, ref  Axis.Item1, out axMat1 );
//			//Vec3D axMat1 = extents.y * axis.mat1;

//			Vector3 axMat2;
//			Vector3.Multiply( Extents.Z, ref Axis.Item2, out axMat2 );
//			//Vec3D axMat2 = extents.z * axis.mat2;

//			Vector3 temp0;
//			Vector3.Subtract( ref Center, ref axMat0, out temp0 );
//			//Vec3D temp0 = center - axMat0;

//			Vector3 temp1;
//			Vector3.Add( ref Center, ref axMat0, out temp1 );
//			//Vec3D temp1 = center + axMat0;

//			Vector3 temp2;
//			Vector3.Subtract( ref axMat1, ref axMat2, out temp2 );
//			//Vec3D temp2 = axMat1 - axMat2;

//			Vector3 temp3;
//			Vector3.Add( ref axMat1, ref axMat2, out temp3 );
//			//Vec3D temp3 = axMat1 + axMat2;

//			points[ 0 ] = temp0 - temp3;
//			points[ 1 ] = temp1 - temp3;
//			points[ 2 ] = temp1 + temp2;
//			points[ 3 ] = temp0 + temp2;
//			points[ 4 ] = temp0 - temp2;
//			points[ 5 ] = temp1 - temp2;
//			points[ 6 ] = temp1 + temp3;
//			points[ 7 ] = temp0 + temp3;
//		}

//		/// <summary>
//		/// Converts the current instance of <see cref="Box2"/> into the equivalent <see cref="Bounds"/> structure.
//		/// </summary>
//		/// <returns>The equivalent <see cref="Bounds"/> structure.</returns>
//		public Rectangle ToBounds()
//		{
//			Vector3 halfSize = new Vector3(
//				Math.Abs( Axis.Item0.X * Extents.X ) + Math.Abs( Axis.Item1.X * Extents.Y ) + Math.Abs( Axis.Item2.X * Extents.Z ),
//				Math.Abs( Axis.Item0.Y * Extents.X ) + Math.Abs( Axis.Item1.Y * Extents.Y ) + Math.Abs( Axis.Item2.Y * Extents.Z ),
//				Math.Abs( Axis.Item0.Z * Extents.X ) + Math.Abs( Axis.Item1.Z * Extents.Y ) + Math.Abs( Axis.Item2.Z * Extents.Z ) );
//			return new Bounds( Center - halfSize, Center + halfSize );

//			//BoundsD result;

//			//Vec3D axMat0, axMat1, axMat2;
//			//Vec3D.Multiply( extents.x, ref axis.mat0, out axMat0 );
//			//Vec3D.Multiply( extents.y, ref axis.mat1, out axMat1 );
//			//Vec3D.Multiply( extents.z, ref axis.mat2, out axMat2 );

//			//Vec3D temp0, temp1, temp2, temp3;
//			//Vec3D.Subtract( ref center, ref axMat0, out temp0 );
//			//Vec3D.Add( ref center, ref axMat0, out temp1 );
//			//Vec3D.Subtract( ref axMat1, ref axMat2, out temp2 );
//			//Vec3D.Add( ref axMat1, ref axMat2, out temp3 );

//			//Vec3D v;
//			//Vec3D.Subtract( ref temp0, ref temp3, out v );
//			//result.minimum = v;
//			//result.maximum = v;

//			//Vec3D.Subtract( ref temp1, ref temp3, out v );
//			//result.Add( ref v );

//			//Vec3D.Add( ref temp1, ref temp2, out v );
//			//result.Add( ref v );

//			//Vec3D.Add( ref temp0, ref temp2, out v );
//			//result.Add( ref v );

//			//Vec3D.Subtract( ref temp0, ref temp2, out v );
//			//result.Add( ref v );

//			//Vec3D.Subtract( ref temp1, ref temp2, out v );
//			//result.Add( ref v );

//			//Vec3D.Add( ref temp1, ref temp3, out v );
//			//result.Add( ref v );

//			//Vec3D.Add( ref temp0, ref temp3, out v );
//			//result.Add( ref v );



//			//Vec3D axMat0 = extents.x * axis.mat0;
//			//Vec3D axMat1 = extents.y * axis.mat1;
//			//Vec3D axMat2 = extents.z * axis.mat2;

//			//Vec3D temp0 = new Vec3D( center - axMat0 );
//			//Vec3D temp1 = new Vec3D( center + axMat0 );
//			//Vec3D temp2 = new Vec3D( axMat1 - axMat2 );
//			//Vec3D temp3 = new Vec3D( axMat1 + axMat2 );

//			//Bounds result = new Bounds( temp0 - temp3 );
//			//result.Add( temp1 - temp3 );
//			//result.Add( temp1 + temp2 );
//			//result.Add( temp0 + temp2 );
//			//result.Add( temp0 - temp2 );
//			//result.Add( temp1 - temp2 );
//			//result.Add( temp1 + temp3 );
//			//result.Add( temp0 + temp3 );

//			//return result;
//		}

//		/// <summary>
//		/// Converts the current instance of <see cref="Box2"/> into the equivalent <see cref="Bounds"/> structure.
//		/// </summary>
//		/// <param name="result">When the method completes, contains the equivalent <see cref="Bounds"/> structure.</param>
//		public void ToBounds( out Rectangle result )
//		{
//			Vector3 halfSize = new Vector3(
//				Math.Abs( Axis.Item0.X * Extents.X ) + Math.Abs( Axis.Item1.X * Extents.Y ) + Math.Abs( Axis.Item2.X * Extents.Z ),
//				Math.Abs( Axis.Item0.Y * Extents.X ) + Math.Abs( Axis.Item1.Y * Extents.Y ) + Math.Abs( Axis.Item2.Y * Extents.Z ),
//				Math.Abs( Axis.Item0.Z * Extents.X ) + Math.Abs( Axis.Item1.Z * Extents.Y ) + Math.Abs( Axis.Item2.Z * Extents.Z ) );
//			result.Minimum = Center - halfSize;
//			result.Maximum = Center + halfSize;

//			//Vec3D axMat0, axMat1, axMat2;
//			//Vec3D.Multiply( extents.x, ref axis.mat0, out axMat0 );
//			//Vec3D.Multiply( extents.y, ref axis.mat1, out axMat1 );
//			//Vec3D.Multiply( extents.z, ref axis.mat2, out axMat2 );

//			//Vec3D temp0, temp1, temp2, temp3;
//			//Vec3D.Subtract( ref center, ref axMat0, out temp0 );
//			//Vec3D.Add( ref center, ref axMat0, out temp1 );
//			//Vec3D.Subtract( ref axMat1, ref axMat2, out temp2 );
//			//Vec3D.Add( ref axMat1, ref axMat2, out temp3 );

//			//Vec3D v;
//			//Vec3D.Subtract( ref temp0, ref temp3, out v );
//			//result.minimum = v;
//			//result.maximum = v;

//			//Vec3D.Subtract( ref temp1, ref temp3, out v );
//			//result.Add( ref v );

//			//Vec3D.Add( ref temp1, ref temp2, out v );
//			//result.Add( ref v );

//			//Vec3D.Add( ref temp0, ref temp2, out v );
//			//result.Add( ref v );

//			//Vec3D.Subtract( ref temp0, ref temp2, out v );
//			//result.Add( ref v );

//			//Vec3D.Subtract( ref temp1, ref temp2, out v );
//			//result.Add( ref v );

//			//Vec3D.Add( ref temp1, ref temp3, out v );
//			//result.Add( ref v );

//			//Vec3D.Add( ref temp0, ref temp3, out v );
//			//result.Add( ref v );
//		}

//		/// <summary>
//		/// Determines whether the current instance of <see cref="Box2"/> contains a given point.
//		/// </summary>
//		/// <param name="point">A point to check.</param>
//		/// <returns>True if the current instance of <see cref="Box2"/> contains a given point; False otherwise.</returns>
//		public bool Contains( ref Vector2 point )
//		{
//			Vector3 localPoint;
//			Vector3.Subtract( ref point, ref Center, out localPoint );
//			//Vec3D localPoint = point - center;
//			if( Math.Abs( Vector3.Dot( ref localPoint, ref Axis.Item0 ) ) > Extents.X ||
//				Math.Abs( Vector3.Dot( ref localPoint, ref Axis.Item1 ) ) > Extents.Y ||
//				Math.Abs( Vector3.Dot( ref localPoint, ref Axis.Item2 ) ) > Extents.Z )
//			{
//				return false;
//			}
//			return true;
//		}

//		/// <summary>
//		/// Determines whether the current instance of <see cref="Box2"/> contains a given point.
//		/// </summary>
//		/// <param name="point">A point to check.</param>
//		/// <returns>True if the current instance of <see cref="Box2"/> contains a given point; False otherwise.</returns>
//		public bool Contains( Vector2 point )
//		{
//			return Contains( ref point );
//		}

//		/// <summary>
//		/// Determines whether the current instance of <see cref="Box2"/> contains given bounds.
//		/// </summary>
//		/// <param name="bounds">Bounds to check.</param>
//		/// <returns>True if the current instance of <see cref="Box2"/> contains given bounds; False otherwise.</returns>
//		public bool Contains( ref Rectangle bounds )
//		{
//			//slowly
//			Vector3 p;

//			p = new Vector3( bounds.Minimum.X, bounds.Minimum.Y, bounds.Minimum.Z );
//			if( !Contains( ref p ) )
//				return false;
//			p = new Vector3( bounds.Minimum.X, bounds.Minimum.Y, bounds.Maximum.Z );
//			if( !Contains( ref p ) )
//				return false;
//			p = new Vector3( bounds.Minimum.X, bounds.Maximum.Y, bounds.Minimum.Z );
//			if( !Contains( ref p ) )
//				return false;
//			p = new Vector3( bounds.Minimum.X, bounds.Maximum.Y, bounds.Maximum.Z );
//			if( !Contains( ref p ) )
//				return false;
//			p = new Vector3( bounds.Maximum.X, bounds.Minimum.Y, bounds.Minimum.Z );
//			if( !Contains( ref p ) )
//				return false;
//			p = new Vector3( bounds.Maximum.X, bounds.Minimum.Y, bounds.Maximum.Z );
//			if( !Contains( ref p ) )
//				return false;
//			p = new Vector3( bounds.Maximum.X, bounds.Maximum.Y, bounds.Minimum.Z );
//			if( !Contains( ref p ) )
//				return false;
//			p = new Vector3( bounds.Maximum.X, bounds.Maximum.Y, bounds.Maximum.Z );
//			if( !Contains( ref p ) )
//				return false;

//			return true;
//		}

//		/// <summary>
//		/// Determines whether the current instance of <see cref="Box2"/> contains given bounds.
//		/// </summary>
//		/// <param name="bounds">Bounds to check.</param>
//		/// <returns>True if the current instance of <see cref="Box2"/> contains given bounds; False otherwise.</returns>
//		public bool Contains( Rectangle bounds )
//		{
//			return Contains( ref bounds );
//		}

//		///// <summary>
//		///// Determines whether the current instance of <see cref="Box2"/> contains the given sphere.
//		///// </summary>
//		///// <param name="s">The sphere to check.</param>
//		///// <returns>True if the current instance of <see cref="Box2"/> contains the given sphere; False otherwise.</returns>
//		//public bool Contains( ref Sphere s )
//		//{
//		//	Vector3 localPoint;
//		//	Vector3.Subtract( ref s.Origin, ref Center, out localPoint );

//		//	double x = Math.Abs( Vector3.Dot( ref localPoint, ref Axis.Item0 ) ) - Extents.X;
//		//	if( x > 0 || -x < s.Radius )
//		//		return false;
//		//	double y = Math.Abs( Vector3.Dot( ref localPoint, ref Axis.Item1 ) ) - Extents.Y;
//		//	if( y > 0 || -y < s.Radius )
//		//		return false;
//		//	double z = Math.Abs( Vector3.Dot( ref localPoint, ref Axis.Item2 ) ) - Extents.Z;
//		//	if( z > 0 || -z < s.Radius )
//		//		return false;

//		//	return true;
//		//}

//		///// <summary>
//		///// Determines whether the current instance of <see cref="Box2"/> contains the given sphere.
//		///// </summary>
//		///// <param name="s">The sphere to check.</param>
//		///// <returns>True if the current instance of <see cref="Box2"/> contains the given sphere; False otherwise.</returns>
//		//public bool Contains( Sphere s )
//		//{
//		//	return Contains( ref s );
//		//}

//		/// <summary>
//		/// Determines whether the current instance of <see cref="Box2"/> contains another instance of <see cref="Box2"/>.
//		/// </summary>
//		/// <param name="box">The box to check.</param>
//		/// <returns>True if the current instance of <see cref="Box2"/> contains the given box; False otherwise.</returns>
//		public bool Contains( ref Box2 box )
//		{
//			Vector3[] points = null;
//			box.ToPoints( ref points );
//			foreach( Vector3 point in points )
//			{
//				if( !Contains( point ) )
//					return false;
//			}
//			return true;
//		}

//		/// <summary>
//		/// Determines whether the current instance of <see cref="Box2"/> contains another instance of <see cref="Box2"/>.
//		/// </summary>
//		/// <param name="box">The box to check.</param>
//		/// <returns>True if the current instance of <see cref="Box2"/> contains the given box; False otherwise.</returns>
//		public bool Contains( Box2 box )
//		{
//			return Contains( ref box );
//		}

//		static bool BoxPlaneClip( double denom, double numer, ref double scale0, ref double scale1 )
//		{
//			if( denom > 0.0 )
//			{
//				if( numer > denom * scale1 )
//					return false;
//				if( numer > denom * scale0 )
//					scale0 = numer / denom;
//				return true;
//			}
//			else if( denom < 0.0 )
//			{
//				if( numer > denom * scale0 )
//					return false;
//				if( numer > denom * scale1 )
//					scale1 = numer / denom;
//				return true;
//			}
//			else
//				return ( numer <= 0.0 );
//		}

//		/// <summary>
//		/// Determines whether the given ray intersects the current instance of <see cref="Box2"/>.
//		/// </summary>
//		/// <param name="ray">The ray to check.</param>
//		/// <param name="scale1">When the method completes, contains the ray and box intersection min point.</param>
//		/// <param name="scale2">When the method completes, contains the ray and box intersection max point.</param>
//		/// <returns>True if the given ray intersects the current instance of <see cref="Box2"/>; False otherwise.</returns>
//		public bool Intersects( Ray ray, out double scale1, out double scale2 )
//		{
//			Matrix3 transposedAxis;
//			Axis.GetTranspose( out transposedAxis );

//			Vector3 localStart;
//			Vector3 diff;
//			Vector3.Subtract( ref ray.Origin, ref Center, out diff );
//			Matrix3.Multiply( ref diff, ref transposedAxis, out localStart );
//			//Vec3D localStart = ( ray.Origin - center ) * transposedAxis;

//			Vector3 localDir;
//			Matrix3.Multiply( ref ray.Direction, ref transposedAxis, out localDir );
//			//Vec3D localDir = ray.Direction * transposedAxis;

//			double s1 = double.MinValue;
//			double s2 = double.MaxValue;
//			bool ret =
//				BoxPlaneClip( localDir.X, -localStart.X - Extents.X, ref s1, ref s2 ) &&
//				BoxPlaneClip( -localDir.X, localStart.X - Extents.X, ref s1, ref s2 ) &&
//				BoxPlaneClip( localDir.Y, -localStart.Y - Extents.Y, ref s1, ref s2 ) &&
//				BoxPlaneClip( -localDir.Y, localStart.Y - Extents.Y, ref s1, ref s2 ) &&
//				BoxPlaneClip( localDir.Z, -localStart.Z - Extents.Z, ref s1, ref s2 ) &&
//				BoxPlaneClip( -localDir.Z, localStart.Z - Extents.Z, ref s1, ref s2 );

//			scale1 = s1;
//			scale2 = s2;

//			return ret;
//		}

//		/// <summary>
//		/// Determines whether the given ray intersects the current instance of <see cref="Box2"/>.
//		/// </summary>
//		/// <param name="ray">The ray to check.</param>
//		/// <returns>True if the given ray intersects the current instance of <see cref="Box2"/>; False otherwise.</returns>
//		public bool Intersects( Ray ray )
//		{
//			double scale1;
//			double scale2;
//			return Intersects( ray, out scale1, out scale2 );
//		}

//		////not used
//		//static double[] ld = new double[ 3 ];
//		//public bool LineIntersection( Vec3D start, Vec3D end )
//		//{
//		//   Vec3D lineDir = 0.5f * ( end - start );
//		//   Vec3D lineCenter = start + lineDir;
//		//   Vec3D dir = lineCenter - center;

//		//   ld[ 0 ] = Math.Abs( Vec3D.Dot( lineDir, axis[ 0 ] ) );
//		//   if( Math.Abs( Vec3D.Dot( dir, axis[ 0 ] ) ) > extents[ 0 ] + ld[ 0 ] )
//		//      return false;

//		//   ld[ 1 ] = Math.Abs( Vec3D.Dot( lineDir, axis[ 1 ] ) );
//		//   if( Math.Abs( Vec3D.Dot( dir, axis[ 1 ] ) ) > extents[ 1 ] + ld[ 1 ] )
//		//      return false;

//		//   ld[ 2 ] = Math.Abs( Vec3D.Dot( lineDir, axis[ 2 ] ) );
//		//   if( Math.Abs( Vec3D.Dot( dir, axis[ 2 ] ) ) > extents[ 2 ] + ld[ 2 ] )
//		//      return false;

//		//   Vec3D cross = lineDir.Cross( dir );

//		//   if( Math.Abs( Vec3D.Dot( cross, axis[ 0 ] ) ) > extents[ 1 ] * ld[ 2 ] + extents[ 2 ] * ld[ 1 ] )
//		//      return false;

//		//   if( Math.Abs( Vec3D.Dot( cross, axis[ 1 ] ) ) > extents[ 0 ] * ld[ 2 ] + extents[ 2 ] * ld[ 0 ] )
//		//      return false;

//		//   if( Math.Abs( Vec3D.Dot( cross, axis[ 2 ] ) ) > extents[ 0 ] * ld[ 1 ] + extents[ 1 ] * ld[ 0 ] )
//		//      return false;

//		//   return true;
//		//}

//		/// <summary>
//		/// Determines whether another instance of <see cref="Box2"/> intersects the current instance of <see cref="Box2"/>.
//		/// </summary>
//		/// <param name="Box2">The box to check.</param>
//		/// <returns>True if another instance of <see cref="Box2"/> intersects the current instance of <see cref="Box2"/>; False otherwise.</returns>
//		public bool Intersects( ref Box box )
//		{
//			double c00, c01, c02, c10, c11, c12, c20, c21, c22;
//			//double c[3][3];		// matrix c = axis.Transpose() * a.axis
//			double ac00, ac01, ac02, ac10, ac11, ac12, ac20, ac21, ac22;
//			//double ac[3][3];		// absolute values of c

//			Vector3 axisDir = Vector3.Zero;	// axis[i] * dir
//			double d, e0, e1;	// distance between centers and projected extents

//			Vector3 dir;
//			Vector3.Subtract( ref box.Center, ref Center, out dir );
//			//Vec3D dir = b.center - center;

//			// axis C0 + t * A0
//			c00 = Vector3.Dot( ref Axis.Item0, ref box.Axis.Item0 );
//			c01 = Vector3.Dot( ref Axis.Item0, ref box.Axis.Item1 );
//			c02 = Vector3.Dot( ref Axis.Item0, ref box.Axis.Item2 );
//			axisDir.X = Vector3.Dot( ref Axis.Item0, ref dir );
//			ac00 = Math.Abs( c00 );
//			ac01 = Math.Abs( c01 );
//			ac02 = Math.Abs( c02 );

//			d = Math.Abs( axisDir.X );
//			e0 = Extents.X;
//			e1 = box.Extents.X * ac00 + box.Extents.Y * ac01 + box.Extents.Z * ac02;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A1
//			c10 = Vector3.Dot( ref Axis.Item1, ref box.Axis.Item0 );
//			c11 = Vector3.Dot( ref  Axis.Item1, ref box.Axis.Item1 );
//			c12 = Vector3.Dot( ref Axis.Item1, ref box.Axis.Item2 );
//			axisDir.Y = Vector3.Dot( ref Axis.Item1, ref  dir );
//			ac10 = Math.Abs( c10 );
//			ac11 = Math.Abs( c11 );
//			ac12 = Math.Abs( c12 );

//			d = Math.Abs( axisDir.Y );
//			e0 = Extents.Y;
//			e1 = box.Extents.X * ac10 + box.Extents.Y * ac11 + box.Extents.Z * ac12;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A2
//			c20 = Vector3.Dot( ref Axis.Item2, ref box.Axis.Item0 );
//			c21 = Vector3.Dot( ref Axis.Item2, ref box.Axis.Item1 );
//			c22 = Vector3.Dot( ref Axis.Item2, ref box.Axis.Item2 );
//			axisDir.Z = Vector3.Dot( ref  Axis.Item2, ref dir );
//			ac20 = Math.Abs( c20 );
//			ac21 = Math.Abs( c21 );
//			ac22 = Math.Abs( c22 );

//			d = Math.Abs( axisDir.Z );
//			e0 = Extents.Z;
//			e1 = box.Extents.X * ac20 + box.Extents.Y * ac21 + box.Extents.Z * ac22;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * B0
//			d = Math.Abs( Vector3.Dot( ref  box.Axis.Item0, ref  dir ) );
//			e0 = Extents.X * ac00 + Extents.Y * ac10 + Extents.Z * ac20;
//			e1 = box.Extents.X;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * B1
//			d = Math.Abs( Vector3.Dot( ref  box.Axis.Item1, ref dir ) );
//			e0 = Extents.X * ac01 + Extents.Y * ac11 + Extents.Z * ac21;
//			e1 = box.Extents.Y;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * B2
//			d = Math.Abs( Vector3.Dot( ref box.Axis.Item2, ref dir ) );
//			e0 = Extents.X * ac02 + Extents.Y * ac12 + Extents.Z * ac22;
//			e1 = box.Extents.Z;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A0xB0
//			d = Math.Abs( axisDir.Z * c10 - axisDir.Y * c20 );
//			e0 = Extents.Y * ac20 + Extents.Z * ac10;
//			e1 = box.Extents.Y * ac02 + box.Extents.Z * ac01;
//			if( d > e0 + e1 )
//			{
//				return false;
//			}

//			// axis C0 + t * A0xB1
//			d = Math.Abs( axisDir.Z * c11 - axisDir.Y * c21 );
//			e0 = Extents.Y * ac21 + Extents.Z * ac11;
//			e1 = box.Extents.X * ac02 + box.Extents.Z * ac00;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A0xB2
//			d = Math.Abs( axisDir.Z * c12 - axisDir.Y * c22 );
//			e0 = Extents.Y * ac22 + Extents.Z * ac12;
//			e1 = box.Extents.X * ac01 + box.Extents.Y * ac00;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A1xB0
//			d = Math.Abs( axisDir.X * c20 - axisDir.Z * c00 );
//			e0 = Extents.X * ac20 + Extents.Z * ac00;
//			e1 = box.Extents.Y * ac12 + box.Extents.Z * ac11;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A1xB1
//			d = Math.Abs( axisDir.X * c21 - axisDir.Z * c01 );
//			e0 = Extents.X * ac21 + Extents.Z * ac01;
//			e1 = box.Extents.X * ac12 + box.Extents.Z * ac10;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A1xB2
//			d = Math.Abs( axisDir.X * c22 - axisDir.Z * c02 );
//			e0 = Extents.X * ac22 + Extents.Z * ac02;
//			e1 = box.Extents.X * ac11 + box.Extents.Y * ac10;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A2xB0
//			d = Math.Abs( axisDir.Y * c00 - axisDir.X * c10 );
//			e0 = Extents.X * ac10 + Extents.Y * ac00;
//			e1 = box.Extents.Y * ac22 + box.Extents.Z * ac21;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A2xB1
//			d = Math.Abs( axisDir.Y * c01 - axisDir.X * c11 );
//			e0 = Extents.X * ac11 + Extents.Y * ac01;
//			e1 = box.Extents.X * ac22 + box.Extents.Z * ac20;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A2xB2
//			d = Math.Abs( axisDir.Y * c02 - axisDir.X * c12 );
//			e0 = Extents.X * ac12 + Extents.Y * ac02;
//			e1 = box.Extents.X * ac21 + box.Extents.Y * ac20;
//			if( d > e0 + e1 )
//				return false;
//			return true;
//		}

//		/// <summary>
//		/// Determines whether another instance of <see cref="Box2"/> intersects the current instance of <see cref="Box2"/>.
//		/// </summary>
//		/// <param name="Box2">The box to check.</param>
//		/// <returns>True if another instance of <see cref="Box2"/> intersects the current instance of <see cref="Box2"/>; False otherwise.</returns>
//		public bool Intersects( Box box )
//		{
//			return Intersects( ref box );
//		}

//		/// <summary>
//		/// Determines whether the given bounds intersect the current instance of <see cref="Box2"/>.
//		/// </summary>
//		/// <param name="bounds">The bounds to check.</param>
//		/// <returns>True if the given bounds intersect the current instance of <see cref="Box2"/>; False otherwise.</returns>
//		public bool Intersects( ref Bounds bounds )
//		{
//			double c00, c01, c02, c10, c11, c12, c20, c21, c22;
//			//double c[3][3]; // matrix c = axis.Transpose() * a.axis
//			double ac00, ac01, ac02, ac10, ac11, ac12, ac20, ac21, ac22;
//			//double ac[3][3]; // absolute values of c

//			double d, e0, e1;	// distance between centers and projected extents

//			Vector3 boundsCenter;
//			bounds.GetCenter( out boundsCenter );
//			//Vec3D boundsCenter = bounds.GetCenter();

//			// vector between centers
//			Vector3 dir;
//			Vector3.Subtract( ref boundsCenter, ref Center, out dir );
//			//Vec3D dir = boundsCenter - center;

//			Vector3 axisDir = Vector3.Zero;// axis[ i ] * dir

//			Vector3 boundsExtents;
//			Vector3.Subtract( ref bounds.Maximum, ref boundsCenter, out boundsExtents );
//			//Vec3D boundsExtents = bounds.Maximum - boundsCenter;

//			// axis C0 + t * A0
//			c00 = Axis.Item0.X;//Vec3D.Dot( axis[ 0 ], Vec3D.XAxis );
//			c01 = Axis.Item0.Y;//Vec3D.Dot( axis[ 0 ], Vec3D.YAxis );
//			c02 = Axis.Item0.Z;//Vec3D.Dot( axis[ 0 ], Vec3D.ZAxis );
//			axisDir.X = Vector3.Dot( ref Axis.Item0, ref dir );
//			ac00 = Math.Abs( c00 );
//			ac01 = Math.Abs( c01 );
//			ac02 = Math.Abs( c02 );

//			d = Math.Abs( axisDir.X );
//			e0 = Extents.X;
//			e1 = boundsExtents.X * ac00 + boundsExtents.Y * ac01 + boundsExtents.Z * ac02;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A1
//			c10 = Axis.Item1.X;// Vec3D.Dot( axis[ 1 ], Vec3D.XAxis );
//			c11 = Axis.Item1.Y;//Vec3D.Dot( axis[ 1 ], Vec3D.YAxis );
//			c12 = Axis.Item1.Z;//Vec3D.Dot( axis[ 1 ], Vec3D.ZAxis );
//			axisDir.Y = Vector3.Dot( ref Axis.Item1, ref dir );
//			ac10 = Math.Abs( c10 );
//			ac11 = Math.Abs( c11 );
//			ac12 = Math.Abs( c12 );

//			d = Math.Abs( axisDir.Y );
//			e0 = Extents.Y;
//			e1 = boundsExtents.X * ac10 + boundsExtents.Y * ac11 + boundsExtents.Z * ac12;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A2
//			c20 = Axis.Item2.X;// Vec3D.Dot( axis[ 2 ], Vec3D.XAxis );
//			c21 = Axis.Item2.Y;// Vec3D.Dot( axis[ 2 ], Vec3D.YAxis );
//			c22 = Axis.Item2.Z;// Vec3D.Dot( axis[ 2 ], Vec3D.ZAxis );
//			axisDir.Z = Vector3.Dot( ref Axis.Item2, ref dir );
//			ac20 = Math.Abs( c20 );
//			ac21 = Math.Abs( c21 );
//			ac22 = Math.Abs( c22 );

//			d = Math.Abs( axisDir.Z );
//			e0 = Extents.Z;
//			e1 = boundsExtents.X * ac20 + boundsExtents.Y * ac21 + boundsExtents.Z * ac22;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * B0
//			d = Math.Abs( dir.X );//Vec3D.Dot( Vec3D.XAxis, dir ) );
//			e0 = Extents.X * ac00 + Extents.Y * ac10 + Extents.Z * ac20;
//			e1 = boundsExtents.X;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * B1
//			d = Math.Abs( dir.Y );//Vec3D.Dot( Vec3D.YAxis, dir ) );
//			e0 = Extents.X * ac01 + Extents.Y * ac11 + Extents.Z * ac21;
//			e1 = boundsExtents.Y;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * B2
//			d = Math.Abs( dir.Z );//Vec3D.Dot( Vec3D.ZAxis, dir ) );
//			e0 = Extents.X * ac02 + Extents.Y * ac12 + Extents.Z * ac22;
//			e1 = boundsExtents.Z;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A0xB0
//			d = Math.Abs( axisDir.Z * c10 - axisDir.Y * c20 );
//			e0 = Extents.Y * ac20 + Extents.Z * ac10;
//			e1 = boundsExtents.Y * ac02 + boundsExtents.Z * ac01;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A0xB1
//			d = Math.Abs( axisDir.Z * c11 - axisDir.Y * c21 );
//			e0 = Extents.Y * ac21 + Extents.Z * ac11;
//			e1 = boundsExtents.X * ac02 + boundsExtents.Z * ac00;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A0xB2
//			d = Math.Abs( axisDir.Z * c12 - axisDir.Y * c22 );
//			e0 = Extents.Y * ac22 + Extents.Z * ac12;
//			e1 = boundsExtents.X * ac01 + boundsExtents.Y * ac00;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A1xB0
//			d = Math.Abs( axisDir.X * c20 - axisDir.Z * c00 );
//			e0 = Extents.X * ac20 + Extents.Z * ac00;
//			e1 = boundsExtents.Y * ac12 + boundsExtents.Z * ac11;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A1xB1
//			d = Math.Abs( axisDir.X * c21 - axisDir.Z * c01 );
//			e0 = Extents.X * ac21 + Extents.Z * ac01;
//			e1 = boundsExtents.X * ac12 + boundsExtents.Z * ac10;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A1xB2
//			d = Math.Abs( axisDir.X * c22 - axisDir.Z * c02 );
//			e0 = Extents.X * ac22 + Extents.Z * ac02;
//			e1 = boundsExtents.X * ac11 + boundsExtents.Y * ac10;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A2xB0
//			d = Math.Abs( axisDir.Y * c00 - axisDir.X * c10 );
//			e0 = Extents.X * ac10 + Extents.Y * ac00;
//			e1 = boundsExtents.Y * ac22 + boundsExtents.Z * ac21;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A2xB1
//			d = Math.Abs( axisDir.Y * c01 - axisDir.X * c11 );
//			e0 = Extents.X * ac11 + Extents.Y * ac01;
//			e1 = boundsExtents.X * ac22 + boundsExtents.Z * ac20;
//			if( d > e0 + e1 )
//				return false;

//			// axis C0 + t * A2xB2
//			d = Math.Abs( axisDir.Y * c02 - axisDir.X * c12 );
//			e0 = Extents.X * ac12 + Extents.Y * ac02;
//			e1 = boundsExtents.X * ac21 + boundsExtents.Y * ac20;
//			if( d > e0 + e1 )
//				return false;

//			return true;
//		}

//		/// <summary>
//		/// Determines whether the given bounds intersect the current instance of <see cref="Box2"/>.
//		/// </summary>
//		/// <param name="bounds">The bounds to check.</param>
//		/// <returns>True if the given bounds intersect the current instance of <see cref="Box2"/>; False otherwise.</returns>
//		public bool Intersects( Bounds bounds )
//		{
//			return Intersects( ref bounds );
//		}

//		/// <summary>
//		/// Determines from which side of the plane the box is on.
//		/// </summary>
//		/// <param name="plane">The plane to check against.</param>
//		/// <returns>The resulting side of the plane.</returns>
//		public Plane.Side GetPlaneSide( ref Plane plane )
//		{
//			Vector3 localNormal;
//			Matrix3 transposedAxis;
//			Axis.GetTranspose( out transposedAxis );
//			Vector3 normal = plane.Normal;
//			Matrix3.Multiply( ref normal, ref transposedAxis, out localNormal );
//			//Vec3D localNormal = plane.Normal * axis.GetTranspose();

//			double d1 = plane.GetDistance( ref Center );
//			double d2 = Math.Abs( Extents.X * localNormal.X ) +
//				Math.Abs( Extents.Y * localNormal.Y ) +
//				Math.Abs( Extents.Z * localNormal.Z );

//			if( d1 - d2 > 0 )
//				return Plane.Side.Positive;
//			if( d1 + d2 < 0 )
//				return Plane.Side.Negative;
//			return Plane.Side.No;
//		}

//		/// <summary>
//		/// Determines from which side of the plane the box is on.
//		/// </summary>
//		/// <param name="plane">The plane to check against.</param>
//		/// <returns>The resulting side of the plane.</returns>
//		public Plane.Side GetPlaneSide( Plane plane )
//		{
//			return GetPlaneSide( ref plane );
//		}

//		/// <summary>
//		/// Calculates the distance between the current instance of <see cref="Box2"/> and the given plane.
//		/// </summary>
//		/// <param name="plane">The plane to calculate the distance from.</param>
//		/// <returns>The resulting distance.</returns>
//		public double GetPlaneDistance( ref Plane plane )
//		{
//			Vector3 localNormal;
//			Matrix3 transposedAxis;
//			Axis.GetTranspose( out transposedAxis );
//			Vector3 normal = plane.Normal;
//			Matrix3.Multiply( ref normal, ref transposedAxis, out localNormal );
//			//Vec3D localNormal = plane.Normal * axis.GetTranspose();

//			double d1 = plane.GetDistance( ref Center );
//			double d2 = Math.Abs( Extents.X * localNormal.X ) +
//				Math.Abs( Extents.Y * localNormal.Y ) +
//				Math.Abs( Extents.Z * localNormal.Z );

//			if( d1 - d2 > 0 )
//				return d1 - d2;
//			if( d1 + d2 < 0 )
//				return d1 + d2;
//			return 0;
//		}

//		/// <summary>
//		/// Calculates the distance between the current instance of <see cref="Box2"/> and the given plane.
//		/// </summary>
//		/// <param name="plane">The plane to calculate the distance from.</param>
//		/// <returns>The resulting distance.</returns>
//		public double GetPlaneDistance( Plane plane )
//		{
//			return GetPlaneDistance( ref plane );
//		}

//		/// <summary>
//		/// Calculates the squared distance between the current instance of <see cref="Box2"/> and the given point.
//		/// </summary>
//		/// <param name="point">The point to calculate the squared distance from.</param>
//		/// <returns>The resulting distance.</returns>
//		public double GetPointDistanceSquared( Vector3 point )
//		{
//			Vector3 localPoint;
//			Vector3.Subtract( ref point, ref Center, out localPoint );
//			//Vec3D localPoint = point - center;

//			double sqr = 0;

//			double x = Math.Abs( Vector3.Dot( ref localPoint, ref Axis.Item0 ) ) - Extents.X;
//			if( x > 0 )
//				sqr += x * x;
//			double y = Math.Abs( Vector3.Dot( ref localPoint, ref Axis.Item1 ) ) - Extents.Y;
//			if( y > 0 )
//				sqr += y * y;
//			double z = Math.Abs( Vector3.Dot( ref localPoint, ref Axis.Item2 ) ) - Extents.Z;
//			if( z > 0 )
//				sqr += z * z;

//			return sqr;
//		}

//		/// <summary>
//		/// Calculates the distance between the current instance of <see cref="Box2"/> and the given point.
//		/// </summary>
//		/// <param name="point">The point to calculate the distance from.</param>
//		/// <returns>The resulting distance.</returns>
//		public double GetPointDistance( Vector3 point )
//		{
//			double sqr = GetPointDistanceSquared( point );
//			if( sqr == 0 )
//				return 0;
//			return Math.Sqrt( sqr );
//		}

//		/// <summary>
//		/// Converts the current instance of <see cref="Box2"/> to the box of <see cref="BoxF"/> format.
//		/// </summary>
//		/// <returns>The box of <see cref="BoxF"/> format.</returns>
//		[AutoConvertType]
//		public BoxF ToBoxF()
//		{
//			BoxF result;
//			result.Center = Center.ToVector3F();
//			result.Extents = Extents.ToVector3F();
//			result.Axis = Axis.ToMatrix3F();
//			return result;
//		}

//		//!!!!
//		//[AutoConvertType]
//		//Parse, ToString

//	}
//}
