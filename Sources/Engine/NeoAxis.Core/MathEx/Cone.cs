// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NeoAxis
{
	/// <summary>
	/// Represents a double precision cone shape.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct Cone
	{
		/// <summary>
		/// The origin of the cone.
		/// </summary>
		[Serialize]
		public Vector3 Origin;
		/// <summary>
		/// The axis of the cone.
		/// </summary>
		[Serialize]
		public Vector3 Axis;
		/// <summary>
		/// The angle of the cone.
		/// </summary>
		[Serialize]
		public Radian Angle;

		//

		/// <summary>
		/// Constructs a cone with another specified <see cref="Cone"/> object.
		/// </summary>
		/// <param name="source">A specified cone.</param>
		public Cone( Cone source )
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
		public Cone( Vector3 origin, Vector3 axis, Radian angle )
		{
			this.Origin = origin;
			this.Axis = axis;
			this.Angle = angle;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Cone"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Cone"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Cone"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is Cone && this == (Cone)obj );
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
		public static bool operator ==( Cone v1, Cone v2 )
		{
			return ( v1.Origin == v2.Origin && v1.Axis == v2.Axis && v1.Angle == v2.Angle );
		}

		/// <summary>
		/// Determines whether two given cones are unequal.
		/// </summary>
		/// <param name="v1">The first cone to compare.</param>
		/// <param name="v2">The second cone to compare.</param>
		/// <returns>True if the cones are unequal; False otherwise.</returns>
		public static bool operator !=( Cone v1, Cone v2 )
		{
			return ( v1.Origin != v2.Origin || v1.Axis != v2.Axis || v1.Angle != v2.Angle );
		}

		/// <summary>
		/// Determines whether the specified cone is equal to the current instance of <see cref="Cone"/>
		/// with a given precision.
		/// </summary>
		/// <param name="v">The cone to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified cone is equal to the current instance of <see cref="Cone"/>; False otherwise.</returns>
		public bool Equals( Cone v, double epsilon )
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
		/// Determines whether the given sphere intersects the current instance of <see cref="Cone"/>.
		/// </summary>
		/// <param name="sphere">The sphere to check.</param>
		/// <returns>True if the given sphere intersects the current instance of <see cref="Cone"/>; False otherwise.</returns>
		public bool Intersects( Sphere sphere )
		{
			double sinAngle = Math.Sin( Angle );
			double cosAngle = Math.Cos( Angle );
			double invSin = 1.0 / sinAngle;
			double cosSqr = cosAngle * cosAngle;

			Vector3 cmv = sphere.Origin - Origin;
			Vector3 d = cmv + ( sphere.Radius * invSin ) * Axis;
			double lengthSqr = d.LengthSquared();
			double e = Vector3.Dot( d, Axis );
			if( e > 0 && e * e >= lengthSqr * cosSqr )
			{
				double sinSqr = sinAngle * sinAngle;
				lengthSqr = cmv.LengthSquared();
				e = -Vector3.Dot( cmv, Axis );
				if( e > 0.0 && e * e >= lengthSqr * sinSqr )
				{
					double rSqr = sphere.Radius * sphere.Radius;
					return lengthSqr <= rSqr;
				}
				return true;
			}
			return false;
		}

		//!!!!
		//[AutoConvertType]
		//Parse, ToString

	}
}
