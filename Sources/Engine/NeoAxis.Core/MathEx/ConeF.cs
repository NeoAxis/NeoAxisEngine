// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	/// <summary>
	/// Represents a single precision cone shape.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct ConeF
	{
		/// <summary>
		/// The origin of the cone.
		/// </summary>
		[Serialize]
		public Vector3F Origin;
		/// <summary>
		/// The axis of the cone.
		/// </summary>
		[Serialize]
		public Vector3F Axis;
		/// <summary>
		/// The angle of the cone.
		/// </summary>
		[Serialize]
		public RadianF Angle;

		//

		/// <summary>
		/// Constructs a cone with another specified <see cref="ConeF"/> object.
		/// </summary>
		/// <param name="source">A specified cone.</param>
		public ConeF( ConeF source )
		{
			Origin = source.Origin;
			Axis = source.Axis;
			Angle = source.Angle;
		}

		/// <summary>
		/// Constructs a cone with the given origin, axis and angle.
		/// </summary>
		/// <param name="origin">The origin.</param>
		/// <param name="axis">The axis.</param>
		/// <param name="angle">The angle.</param>
		public ConeF( Vector3F origin, Vector3F axis, RadianF angle )
		{
			this.Origin = origin;
			this.Axis = axis;
			this.Angle = angle;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="ConeF"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="ConeF"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="ConeF"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is ConeF && this == (ConeF)obj );
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return ( Origin.GetHashCode() ^ Axis.GetHashCode() ^ Angle.GetHashCode() );
		}

		/// <summary>
		/// Determines whether two given cones are equal.
		/// </summary>
		/// <param name="v1">The first cone to compare.</param>
		/// <param name="v2">The second cone to compare.</param>
		/// <returns>True if the cones are equal; False otherwise.</returns>
		public static bool operator ==( ConeF v1, ConeF v2 )
		{
			return ( v1.Origin == v2.Origin && v1.Axis == v2.Axis && v1.Angle == v2.Angle );
		}

		/// <summary>
		/// Determines whether two given cones are unequal.
		/// </summary>
		/// <param name="v1">The first cone to compare.</param>
		/// <param name="v2">The second cone to compare.</param>
		/// <returns>True if the cones are unequal; False otherwise.</returns>
		public static bool operator !=( ConeF v1, ConeF v2 )
		{
			return ( v1.Origin != v2.Origin || v1.Axis != v2.Axis || v1.Angle != v2.Angle );
		}

		/// <summary>
		/// Determines whether the specified cone is equal to the current instance of <see cref="ConeF"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The cone to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified cone is equal to the current instance of <see cref="ConeF"/>; False otherwise.</returns>
		public bool Equals( ConeF v, float epsilon )
		{
			if( !Origin.Equals( ref v.Origin, epsilon ) )
				return false;
			if( !Axis.Equals( ref v.Axis, epsilon ) )
				return false;
			if( Math.Abs( Angle - v.Angle ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Determines whether the given sphere intersects the current instance of <see cref="ConeF"/>.
		/// </summary>
		/// <param name="sphere">The sphere to check.</param>
		/// <returns>True if the given sphere intersects the current instance of <see cref="ConeF"/>; False otherwise.</returns>
		public bool Intersects( SphereF sphere )
		{
			float sinAngle = MathEx.Sin( Angle );
			float cosAngle = MathEx.Cos( Angle );
			float invSin = 1.0f / sinAngle;
			float cosSqr = cosAngle * cosAngle;

			Vector3F cmv = sphere.Center - Origin;
			Vector3F d = cmv + ( sphere.Radius * invSin ) * Axis;
			float lengthSqr = d.LengthSquared();
			float e = Vector3F.Dot( d, Axis );
			if( e > 0 && e * e >= lengthSqr * cosSqr )
			{
				float sinSqr = sinAngle * sinAngle;
				lengthSqr = cmv.LengthSquared();
				e = -Vector3F.Dot( cmv, Axis );
				if( e > 0.0f && e * e >= lengthSqr * sinSqr )
				{
					float rSqr = sphere.Radius * sphere.Radius;
					return lengthSqr <= rSqr;
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Converts the current instance of <see cref="ConeF"/> to the triangle of <see cref="Cone"/> format.
		/// </summary>
		/// <returns>The triangle of <see cref="Cone"/> format.</returns>
		[AutoConvertType]
		public Cone ToCone()
		{
			Cone result;
			result.Origin = Origin.ToVector3();
			result.Axis = Axis.ToVector3();
			result.Angle = Angle;
			return result;
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString

#if !DISABLE_IMPLICIT
		/// <summary>
		/// Implicit conversion from <see cref="ConeF"/> type to <see cref="Cone"/> type for the given value.
		/// </summary>
		/// <param name="v">The value to type convert.</param>
		public static implicit operator Cone( ConeF v )
		{
			return new Cone( v );
		}
#endif
	}
}
