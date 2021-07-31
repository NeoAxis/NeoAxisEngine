// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Angles> ) )]
	/// <summary>
	/// Represents double precision Euler angles.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Angles
	{
		/// <summary>
		/// The angle of rotation about the X axis in degrees.
		/// </summary>
		public double Roll;
		/// <summary>
		/// The angle of rotation about the Y axis in degrees.
		/// </summary>
		public double Pitch;
		/// <summary>
		/// The angle of rotation about the Z axis in degrees.
		/// </summary>
		public double Yaw;

		/// <summary>
		/// Returns the Euler angles with all of its components set to zero.
		/// </summary>
		public static readonly Angles Zero = new Angles( 0.0, 0.0, 0.0 );

		/// <summary>
		/// Constructs Euler angles with the given <see cref="Vector3"/> object.
		/// </summary>
		/// <param name="v">The given vector.</param>
		public Angles( Vector3 v )
		{
			this.Roll = v.Z;
			this.Pitch = v.X;
			this.Yaw = v.Y;
		}

		/// <summary>
		/// Constructs Euler angles with the given angles of rotation.
		/// </summary>
		/// <param name="roll">The angle of rotation about the X axis.</param>
		/// <param name="pitch">The angle of rotation about the Y axis.</param>
		/// <param name="yaw">The angle of rotation about the Z axis.</param>
		public Angles( double roll, double pitch, double yaw )
		{
			this.Roll = roll;
			this.Pitch = pitch;
			this.Yaw = yaw;
		}

		/// <summary>
		/// Constructs Euler angles with another specified <see cref="Angles"/> object.
		/// </summary>
		/// <param name="source">Euler angles of <see cref="Angles"/> format.</param>
		public Angles( Angles source )
		{
			this.Roll = source.Roll;
			this.Pitch = source.Pitch;
			this.Yaw = source.Yaw;
		}

		/// <summary>
		/// Converts a string representation of Euler angles into the equivalent <see cref="Angles"/> structure.
		/// </summary>
		/// <param name="text">The string representation of Euler angles (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="Angles"/> structure.</returns>
		[AutoConvertType]
		public static Angles Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 3 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 3 parts separated by spaces in the form (roll pitch yaw).", text ) );

			try
			{
				return new Angles(
					double.Parse( vals[ 0 ] ),
					double.Parse( vals[ 1 ] ),
					double.Parse( vals[ 2 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the angles must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Angles"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Angles"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return ToString( 17 );
			//return string.Format( "{0} {1} {2}", roll, pitch, yaw );
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="Angles"/> with a given precision.
		/// </summary>
		/// <param name="precision">The precision value.</param>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="Angles"/>.</returns>
		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "}";
			return string.Format( format, Roll, Pitch, Yaw );
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="Angles"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="Angles"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="Angles"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is Angles && this == (Angles)obj );
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return ( Roll.GetHashCode() ^ Pitch.GetHashCode() ^ Yaw.GetHashCode() );
		}

		/// <summary>
		/// Gets or sets the component at the specified index.
		/// </summary>
		/// <value>The value of the roll, pitch, or yaw component, depending on the index.</value>
		/// <param name="index">The index of the component to access. Use 0 for the roll component, 1 for the pitch component, and 2 for the yaw component.</param>
		/// <returns>The value of the component at the specified index.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
		public unsafe double this[ int index ]
		{
			get
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( double* v = &this.Roll )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( double* v = &this.Roll )
				{
					v[ index ] = value;
				}
			}
		}

		/// <summary>
		/// Determines whether two given Euler angles are equal.
		/// </summary>
		/// <param name="a">The first Euler angles to compare.</param>
		/// <param name="b">The second Euler angles to compare.</param>
		/// <returns>True if the Euler angles are equal; False otherwise.</returns>
		public static bool operator ==( Angles a, Angles b )
		{
			return ( a.Roll == b.Roll && a.Pitch == b.Pitch && a.Yaw == b.Yaw );
		}

		/// <summary>
		/// Determines whether two given Euler angles are unequal.
		/// </summary>
		/// <param name="a">The first Euler angles to compare.</param>
		/// <param name="b">The second Euler angles to compare.</param>
		/// <returns>True if the Euler angles are unequal; False otherwise.</returns>
		public static bool operator !=( Angles a, Angles b )
		{
			return ( a.Roll != b.Roll || a.Pitch != b.Pitch || a.Yaw != b.Yaw );
		}

		/// <summary>
		/// Determines whether the specified Euler angles are equal to the current instance of <see cref="Angles"/>
		/// with a given precision.
		/// </summary>
		/// <param name="a">The Euler angles to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified Euler angles are equal to the current instance of <see cref="Angles"/>; False otherwise.</returns>
		public bool Equals( Angles a, double epsilon )
		{
			if( Math.Abs( Roll - a.Roll ) > epsilon )
				return false;
			if( Math.Abs( Pitch - a.Pitch ) > epsilon )
				return false;
			if( Math.Abs( Yaw - a.Yaw ) > epsilon )
				return false;
			return true;
		}

		/// <summary>
		/// Negates Euler angles.
		/// </summary>
		/// <param name="a">The Euler angles to negate.</param>
		/// <returns>The negated Euler angles.</returns>
		public static Angles operator -( Angles a )
		{
			return new Angles( -a.Roll, -a.Pitch, -a.Yaw );
		}

		/// <summary>
		/// Adds two Euler angles.
		/// </summary>
		/// <param name="a">The first Euler angles to add.</param>
		/// <param name="b">The second Euler angles to add.</param>
		/// <returns>The sum of the two Euler angles.</returns>
		public static Angles operator +( Angles a, Angles b )
		{
			return new Angles( a.Roll + b.Roll, a.Pitch + b.Pitch, a.Yaw + b.Yaw );
		}

		/// <summary>
		/// Subtracts Euler angles.
		/// </summary>
		/// <param name="a">The Euler angles to subtract from.</param>
		/// <param name="b">The Euler angles to be subtracted from another Euler angles.</param>
		/// <returns>The difference between the two Euler angles.</returns>
		public static Angles operator -( Angles a, Angles b )
		{
			return new Angles( a.Roll - b.Roll, a.Pitch - b.Pitch, a.Yaw - b.Yaw );
		}

		/// <summary>
		/// Multiplies Euler angles by a given value.
		/// </summary>
		/// <param name="a">The Euler angles to multiply.</param>
		/// <param name="b">The value by which to multiply.</param>
		/// <returns>The scaled Euler angles.</returns>
		public static Angles operator *( Angles a, double b )
		{
			return new Angles( a.Roll * b, a.Pitch * b, a.Yaw * b );
		}

		/// <summary>
		/// Divides Euler angles by a given scalar.
		/// </summary>
		/// <param name="a">The Euler angles to divide.</param>
		/// <param name="b">The scalar value.</param>
		/// <returns>The scaled Euler angles.</returns>
		public static Angles operator /( Angles a, double b )
		{
			double invb = 1.0 / b;
			return new Angles( a.Roll * invb, a.Pitch * invb, a.Yaw * invb );
		}

		//public static Angles operator *( double a, Angles b )
		//{
		//	double inva = 1.0 / a;
		//	return new Angles( b.Roll * inva, b.Pitch * inva, b.Yaw * inva );
		//}

		/// <summary>
		/// Restricts the current instance of <see cref="Angles"/> to be within a specified range.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		public void Clamp( Angles min, Angles max )
		{
			if( Roll < min.Roll )
				Roll = min.Roll;
			else if( Roll > max.Roll )
				Roll = max.Roll;
			if( Pitch < min.Pitch )
				Pitch = min.Pitch;
			else if( Pitch > max.Pitch )
				Pitch = max.Pitch;
			if( Yaw < min.Yaw )
				Yaw = min.Yaw;
			else if( Yaw > max.Yaw )
				Yaw = max.Yaw;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Angles"/> into the equivalent <see cref="Quaternion"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Quaternion"/> structure.</returns>
		public Quaternion ToQuaternion()
		{
			double a;

			a = MathEx.DegreeToRadian( Yaw ) * .5;
			double sz = Math.Sin( a );
			double cz = Math.Cos( a );

			a = MathEx.DegreeToRadian( Pitch ) * .5;
			double sy = Math.Sin( a );
			double cy = Math.Cos( a );

			a = MathEx.DegreeToRadian( Roll ) * .5;
			double sx = Math.Sin( a );
			double cx = Math.Cos( a );

			double sxcy = sx * cy;
			double cxcy = cx * cy;
			double sxsy = sx * sy;
			double cxsy = cx * sy;

			return new Quaternion( cxsy * sz - sxcy * cz, -cxsy * cz - sxcy * sz, sxsy * cz - cxcy * sz, cxcy * cz + sxsy * sz );
		}

		/// <summary>
		/// Restricts the current instance of <see cref="Angles"/> to be within a range [0, 360].
		/// </summary>
		public void Normalize360()
		{
			int i;

			for( i = 0; i < 3; i++ )
			{
				if( ( this[ i ] >= 360.0 ) || ( this[ i ] < 0.0 ) )
				{
					this[ i ] -= Math.Floor( this[ i ] / 360.0 ) * 360.0;

					if( this[ i ] >= 360.0 )
						this[ i ] -= 360.0;
					if( this[ i ] < 0.0 )
						this[ i ] += 360.0;
				}
			}
		}

		/// <summary>
		/// Restricts the current instance of <see cref="Angles"/> to be within a range [-180, 180].
		/// </summary>
		public void Normalize180()
		{
			Normalize360();
			if( Pitch > 180.0 )
				Pitch -= 360.0;
			if( Yaw > 180.0 )
				Yaw -= 360.0;
			if( Roll > 180.0 )
				Roll -= 360.0;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Angles"/> to the Euler angles of <see cref="AnglesF"/> format.
		/// </summary>
		/// <returns>The Euler angles of <see cref="AnglesF"/> format.</returns>
		[AutoConvertType]
		public AnglesF ToAnglesF()
		{
			AnglesF result;
			result.Pitch = (float)Pitch;
			result.Roll = (float)Roll;
			result.Yaw = (float)Yaw;
			return result;
		}
	}
}
