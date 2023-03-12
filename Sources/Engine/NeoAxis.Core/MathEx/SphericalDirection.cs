// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<SphericalDirection> ) )]
	/// <summary>
	/// Represents a double precision spherical coordinate system direction.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct SphericalDirection
	{
		public double Horizontal;
		public double Vertical;

		public static readonly SphericalDirection Zero = new SphericalDirection( 0.0, 0.0 );

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SphericalDirection( SphericalDirection source )
		{
			Horizontal = source.Horizontal;
			Vertical = source.Vertical;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SphericalDirection( double horizontal, double vertical )
		{
			this.Horizontal = horizontal;
			this.Vertical = vertical;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SphericalDirection( SphericalDirectionF source )
		{
			Horizontal = source.Horizontal;
			Vertical = source.Vertical;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is SphericalDirection && this == (SphericalDirection)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Horizontal.GetHashCode() ^ Vertical.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( SphericalDirection v1, SphericalDirection v2 )
		{
			return ( v1.Horizontal == v2.Horizontal && v1.Vertical == v2.Vertical );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( SphericalDirection v1, SphericalDirection v2 )
		{
			return ( v1.Horizontal != v2.Horizontal || v1.Vertical != v2.Vertical );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( SphericalDirection v, double epsilon )
		{
			if( Math.Abs( Horizontal - v.Horizontal ) > epsilon )
				return false;
			if( Math.Abs( Vertical - v.Vertical ) > epsilon )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static SphericalDirection operator +( SphericalDirection v1, SphericalDirection v2 )
		{
			SphericalDirection result;
			result.Horizontal = v1.Horizontal + v2.Horizontal;
			result.Vertical = v1.Vertical + v2.Vertical;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static SphericalDirection operator -( SphericalDirection v1, SphericalDirection v2 )
		{
			SphericalDirection result;
			result.Horizontal = v1.Horizontal - v2.Horizontal;
			result.Vertical = v1.Vertical - v2.Vertical;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static SphericalDirection operator *( SphericalDirection d, Quaternion q )
		{
			SphericalDirection result;
			Multiply( ref d, ref q, out result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static SphericalDirection operator -( SphericalDirection v )
		{
			SphericalDirection result;
			result.Horizontal = -v.Horizontal;
			result.Vertical = -v.Vertical;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Add( ref SphericalDirection v1, ref SphericalDirection v2, out SphericalDirection result )
		{
			result.Horizontal = v1.Horizontal + v2.Horizontal;
			result.Vertical = v1.Vertical + v2.Vertical;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Subtract( ref SphericalDirection v1, ref SphericalDirection v2, out SphericalDirection result )
		{
			result.Horizontal = v1.Horizontal - v2.Horizontal;
			result.Vertical = v1.Vertical - v2.Vertical;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref SphericalDirection d, ref Quaternion q, out SphericalDirection result )
		{
			Vector3 vector;
			d.GetVector( out vector );
			Vector3 vector2;
			Quaternion.Multiply( ref vector, ref q, out vector2 );
			SphericalDirection.FromVector( ref vector2, out result );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Negate( ref SphericalDirection v, out SphericalDirection result )
		{
			result.Horizontal = -v.Horizontal;
			result.Vertical = -v.Vertical;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static SphericalDirection Add( SphericalDirection v1, SphericalDirection v2 )
		{
			SphericalDirection result;
			Add( ref v1, ref v2, out result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static SphericalDirection Subtract( SphericalDirection v1, SphericalDirection v2 )
		{
			SphericalDirection result;
			Subtract( ref v1, ref v2, out result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static SphericalDirection Multiply( SphericalDirection d, Quaternion q )
		{
			SphericalDirection result;
			Multiply( ref d, ref q, out result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static SphericalDirection Negate( SphericalDirection v )
		{
			SphericalDirection result;
			Negate( ref v, out result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static SphericalDirection FromVector( Vector3 dir )
		{
			SphericalDirection result;
			FromVector( ref dir, out result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void FromVector( ref Vector3 dir, out SphericalDirection result )
		{
			result.Horizontal = Math.Atan2( dir.Y, dir.X );
			double dir2Length = Math.Sqrt( dir.X * dir.X + dir.Y * dir.Y );
			result.Vertical = Math.Atan2( dir.Z, dir2Length );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector3 GetVector()
		{
			Vector3 result;
			GetVector( out result );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetVector( out Vector3 result )
		{
			result.X = Math.Cos( Vertical ) * Math.Cos( Horizontal );
			result.Y = Math.Cos( Vertical ) * Math.Sin( Horizontal );
			result.Z = Math.Sin( Vertical );
		}

		[AutoConvertType]
		public static SphericalDirection Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 2 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 2 parts separated by spaces in the form (h v).", text ) );

			try
			{
				return new SphericalDirection(
					double.Parse( vals[ 0 ] ),
					double.Parse( vals[ 1 ] ) );
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
			//return string.Format( "{0} {1}", horizontal, vertical );
		}

		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "}";
			return string.Format( format, Horizontal, Vertical );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SphericalDirectionF ToSphericalDirectionF()
		{
			SphericalDirectionF result;
			result.Horizontal = (float)Horizontal;
			result.Vertical = (float)Vertical;
			return result;
		}
	}
}
