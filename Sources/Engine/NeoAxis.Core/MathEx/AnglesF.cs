// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<AnglesF> ) )]
	/// <summary>
	/// Represents single precision Euler angles.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct AnglesF
	{
		/// <summary>
		/// The angle of rotation about the X axis in degrees.
		/// </summary>
		public float Roll;
		/// <summary>
		/// The angle of rotation about the Y axis in degrees.
		/// </summary>
		public float Pitch;
		/// <summary>
		/// The angle of rotation about the Z axis in degrees.
		/// </summary>
		public float Yaw;

		/// <summary>
		/// Returns the Euler angles with all of its components set to zero.
		/// </summary>
		public static readonly AnglesF Zero = new AnglesF( 0.0f, 0.0f, 0.0f );

		/// <summary>
		/// Constructs Euler angles with the given <see cref="Vector3F"/> object.
		/// </summary>
		/// <param name="v">The given vector.</param>
		public AnglesF( Vector3F v )
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
		public AnglesF( float roll, float pitch, float yaw )
		{
			this.Roll = roll;
			this.Pitch = pitch;
			this.Yaw = yaw;
		}

		/// <summary>
		/// Constructs Euler angles with another specified <see cref="AnglesF"/> object.
		/// </summary>
		/// <param name="source">Euler angles of <see cref="AnglesF"/> format.</param>
		public AnglesF( AnglesF source )
		{
			this.Roll = source.Roll;
			this.Pitch = source.Pitch;
			this.Yaw = source.Yaw;
		}

		/// <summary>
		/// Converts a string representation of Euler angles into the equivalent <see cref="AnglesF"/> structure.
		/// </summary>
		/// <param name="text">The string representation of Euler angles (with the space delimeters).</param>
		/// <returns>The equivalent <see cref="AnglesF"/> structure.</returns>
		[AutoConvertType]
		public static AnglesF Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 3 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (pitch yaw roll).", text ) );

			try
			{
				return new AnglesF(
					float.Parse( vals[ 0 ] ),
					float.Parse( vals[ 1 ] ),
					float.Parse( vals[ 2 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the angles must be decimal numbers." );
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="AnglesF"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="AnglesF"/>.</returns>
		[AutoConvertType]
		public override string ToString()
		{
			return ToString( 8 );
			//return string.Format( "{0} {1} {2}", roll, pitch, yaw );
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current instance of <see cref="AnglesF"/> with a given precision.
		/// </summary>
		/// <param name="precision">The precision value.</param>
		/// <returns>A <see cref="string"/> that represents the current instance of <see cref="AnglesF"/>.</returns>
		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "}";
			return string.Format( format, Roll, Pitch, Yaw );
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance of <see cref="AnglesF"/>.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance of <see cref="AnglesF"/>.</param>
		/// <returns>True if the specified object is equal to the current instance of <see cref="AnglesF"/>; otherwise, False.</returns>
		public override bool Equals( object obj )
		{
			return ( obj is AnglesF && this == (AnglesF)obj );
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
		public unsafe float this[ int index ]
		{
			get
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( float* v = &this.Roll )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 2 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( float* v = &this.Roll )
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
		public static bool operator ==( AnglesF a, AnglesF b )
		{
			return ( a.Roll == b.Roll && a.Pitch == b.Pitch && a.Yaw == b.Yaw );
		}

		/// <summary>
		/// Determines whether two given Euler angles are unequal.
		/// </summary>
		/// <param name="a">The first Euler angles to compare.</param>
		/// <param name="b">The second Euler angles to compare.</param>
		/// <returns>True if the Euler angles are unequal; False otherwise.</returns>
		public static bool operator !=( AnglesF a, AnglesF b )
		{
			return ( a.Roll != b.Roll || a.Pitch != b.Pitch || a.Yaw != b.Yaw );
		}

		/// <summary>
		/// Determines whether the specified Euler angles are equal to the current instance of <see cref="AnglesF"/>
		/// with a given precision.
		/// </summary>
		/// <param name="a">The Euler angles to compare.</param>
		/// <param name="epsilon">The precision value.</param>
		/// <returns>True if the specified Euler angles are equal to the current instance of <see cref="AnglesF"/>; False otherwise.</returns>
		public bool Equals( AnglesF a, float epsilon )
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
		public static AnglesF operator -( AnglesF a )
		{
			return new AnglesF( -a.Roll, -a.Pitch, -a.Yaw );
		}

		/// <summary>
		/// Adds two Euler angles.
		/// </summary>
		/// <param name="a">The first Euler angles to add.</param>
		/// <param name="b">The second Euler angles to add.</param>
		/// <returns>The sum of the two Euler angles.</returns>
		public static AnglesF operator +( AnglesF a, AnglesF b )
		{
			return new AnglesF( a.Roll + b.Roll, a.Pitch + b.Pitch, a.Yaw + b.Yaw );
		}

		/// <summary>
		/// Subtracts two Euler angles.
		/// </summary>
		/// <param name="a">The Euler angles to subtract from.</param>
		/// <param name="b">The Euler angles to be subtracted from another Euler angles.</param>
		/// <returns>The difference between the two Euler angles.</returns>
		public static AnglesF operator -( AnglesF a, AnglesF b )
		{
			return new AnglesF( a.Roll - b.Roll, a.Pitch - b.Pitch, a.Yaw - b.Yaw );
		}

		/// <summary>
		/// Multiplies Euler angles by a given value.
		/// </summary>
		/// <param name="a">The Euler angles to multiply.</param>
		/// <param name="b">The value by which to multiply.</param>
		/// <returns>The scaled Euler angles.</returns>
		public static AnglesF operator *( AnglesF a, float b )
		{
			return new AnglesF( a.Roll * b, a.Pitch * b, a.Yaw * b );
		}

		/// <summary>
		/// Divides Euler angles by a given scalar.
		/// </summary>
		/// <param name="a">The Euler angles to divide.</param>
		/// <param name="b">The scalar value.</param>
		/// <returns>The scaled Euler angles.</returns>
		public static AnglesF operator /( AnglesF a, float b )
		{
			float invb = 1.0f / b;
			return new AnglesF( a.Roll * invb, a.Pitch * invb, a.Yaw * invb );
		}

		//public static AnglesF operator *( float a, AnglesF b )
		//{
		//	float inva = 1.0f / a;
		//	return new AnglesF( b.Roll * inva, b.Pitch * inva, b.Yaw * inva );
		//}

		/// <summary>
		/// Restricts the current instance of <see cref="AnglesF"/> to be within a specified range.
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		public void Clamp( AnglesF min, AnglesF max )
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
		/// Converts the current instance of <see cref="AnglesF"/> into the equivalent <see cref="QuaternionF"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="QuaternionF"/> structure.</returns>
		public QuaternionF ToQuaternion()
		{
			float a;

			a = MathEx.DegreeToRadian( Yaw ) * .5f;
			float sz = MathEx.Sin( a );
			float cz = MathEx.Cos( a );

			a = MathEx.DegreeToRadian( Pitch ) * .5f;
			float sy = MathEx.Sin( a );
			float cy = MathEx.Cos( a );

			a = MathEx.DegreeToRadian( Roll ) * .5f;
			float sx = MathEx.Sin( a );
			float cx = MathEx.Cos( a );

			float sxcy = sx * cy;
			float cxcy = cx * cy;
			float sxsy = sx * sy;
			float cxsy = cx * sy;

			return new QuaternionF( cxsy * sz - sxcy * cz, -cxsy * cz - sxcy * sz, sxsy * cz - cxcy * sz, cxcy * cz + sxsy * sz );
		}

		/// <summary>
		/// Restricts the current instance of <see cref="AnglesF"/> to be within a range [0, 360].
		/// </summary>
		public void Normalize360()
		{
			int i;

			for( i = 0; i < 3; i++ )
			{
				if( ( this[ i ] >= 360.0f ) || ( this[ i ] < 0.0f ) )
				{
					this[ i ] -= MathEx.Floor( this[ i ] / 360.0f ) * 360.0f;

					if( this[ i ] >= 360.0f )
						this[ i ] -= 360.0f;
					if( this[ i ] < 0.0f )
						this[ i ] += 360.0f;
				}
			}
		}

		/// <summary>
		/// Restricts the current instance of <see cref="AnglesF"/> to be within a range [-180, 180].
		/// </summary>
		public void Normalize180()
		{
			Normalize360();
			if( Pitch > 180.0f )
				Pitch -= 360.0f;
			if( Yaw > 180.0f )
				Yaw -= 360.0f;
			if( Roll > 180.0f )
				Roll -= 360.0f;
		}

		/// <summary>
		/// Converts the current instance of <see cref="AnglesF"/> to the Euler angles of <see cref="Angles"/> format.
		/// </summary>
		/// <returns>The Euler angles of <see cref="Angles"/> format.</returns>
		[AutoConvertType]
		public Angles ToAngles()
		{
			Angles result;
			result.Roll = Roll;
			result.Pitch = Pitch;
			result.Yaw = Yaw;
			return result;
		}

#if !DISABLE_IMPLICIT
		/// <summary>
		/// Implicit conversion from <see cref="AnglesF"/> type to <see cref="Angles"/> type for the given value.
		/// </summary>
		/// <param name="v">The value to type convert.</param>
		public static implicit operator Angles( AnglesF v )
		{
			return new Angles( v );
		}
#endif
	}
}
