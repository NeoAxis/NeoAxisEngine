// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<SphericalDirectionF> ) )]
	/// <summary>
	/// Represents a single precision spherical coordinate system direction.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct SphericalDirectionF
	{
		public float Horizontal;
		public float Vertical;

		public static readonly SphericalDirectionF Zero = new SphericalDirectionF( 0.0f, 0.0f );

		public SphericalDirectionF( SphericalDirectionF source )
		{
			Horizontal = source.Horizontal;
			Vertical = source.Vertical;
		}

		public SphericalDirectionF( float horizontal, float vertical )
		{
			this.Horizontal = horizontal;
			this.Vertical = vertical;
		}

		public override bool Equals( object obj )
		{
			return ( obj is SphericalDirectionF && this == (SphericalDirectionF)obj );
		}

		public override int GetHashCode()
		{
			return ( Horizontal.GetHashCode() ^ Vertical.GetHashCode() );
		}

		public static bool operator ==( SphericalDirectionF v1, SphericalDirectionF v2 )
		{
			return ( v1.Horizontal == v2.Horizontal && v1.Vertical == v2.Vertical );
		}

		public static bool operator !=( SphericalDirectionF v1, SphericalDirectionF v2 )
		{
			return ( v1.Horizontal != v2.Horizontal || v1.Vertical != v2.Vertical );
		}

		public bool Equals( SphericalDirectionF v, float epsilon )
		{
			if( Math.Abs( Horizontal - v.Horizontal ) > epsilon )
				return false;
			if( Math.Abs( Vertical - v.Vertical ) > epsilon )
				return false;
			return true;
		}

		public static SphericalDirectionF operator +( SphericalDirectionF v1, SphericalDirectionF v2 )
		{
			SphericalDirectionF result;
			result.Horizontal = v1.Horizontal + v2.Horizontal;
			result.Vertical = v1.Vertical + v2.Vertical;
			return result;
		}

		public static SphericalDirectionF operator -( SphericalDirectionF v1, SphericalDirectionF v2 )
		{
			SphericalDirectionF result;
			result.Horizontal = v1.Horizontal - v2.Horizontal;
			result.Vertical = v1.Vertical - v2.Vertical;
			return result;
		}

		public static SphericalDirectionF operator *( SphericalDirectionF d, QuaternionF q )
		{
			SphericalDirectionF result;
			Multiply( ref d, ref q, out result );
			return result;
		}

		public static SphericalDirectionF operator -( SphericalDirectionF v )
		{
			SphericalDirectionF result;
			result.Horizontal = -v.Horizontal;
			result.Vertical = -v.Vertical;
			return result;
		}

		public static void Add( ref SphericalDirectionF v1, ref SphericalDirectionF v2, out SphericalDirectionF result )
		{
			result.Horizontal = v1.Horizontal + v2.Horizontal;
			result.Vertical = v1.Vertical + v2.Vertical;
		}

		public static void Subtract( ref SphericalDirectionF v1, ref SphericalDirectionF v2, out SphericalDirectionF result )
		{
			result.Horizontal = v1.Horizontal - v2.Horizontal;
			result.Vertical = v1.Vertical - v2.Vertical;
		}

		public static void Multiply( ref SphericalDirectionF d, ref QuaternionF q, out SphericalDirectionF result )
		{
			Vector3F vector;
			d.GetVector( out vector );
			Vector3F vector2;
			QuaternionF.Multiply( ref vector, ref q, out vector2 );
			SphericalDirectionF.FromVector( ref vector2, out result );
		}

		public static void Negate( ref SphericalDirectionF v, out SphericalDirectionF result )
		{
			result.Horizontal = -v.Horizontal;
			result.Vertical = -v.Vertical;
		}

		public static SphericalDirectionF Add( SphericalDirectionF v1, SphericalDirectionF v2 )
		{
			SphericalDirectionF result;
			Add( ref v1, ref v2, out result );
			return result;
		}

		public static SphericalDirectionF Subtract( SphericalDirectionF v1, SphericalDirectionF v2 )
		{
			SphericalDirectionF result;
			Subtract( ref v1, ref v2, out result );
			return result;
		}

		public static SphericalDirectionF Multiply( SphericalDirectionF d, QuaternionF q )
		{
			SphericalDirectionF result;
			Multiply( ref d, ref q, out result );
			return result;
		}

		public static SphericalDirectionF Negate( SphericalDirectionF v )
		{
			SphericalDirectionF result;
			Negate( ref v, out result );
			return result;
		}

		public static SphericalDirectionF FromVector( Vector3F dir )
		{
			SphericalDirectionF result;
			FromVector( ref dir, out result );
			return result;
		}

		public static void FromVector( ref Vector3F dir, out SphericalDirectionF result )
		{
			result.Horizontal = MathEx.Atan2( dir.Y, dir.X );
			float dir2Length = MathEx.Sqrt( dir.X * dir.X + dir.Y * dir.Y );
			result.Vertical = MathEx.Atan2( dir.Z, dir2Length );
		}

		public Vector3F GetVector()
		{
			Vector3F result;
			GetVector( out result );
			return result;
		}

		public void GetVector( out Vector3F result )
		{
			result.X = MathEx.Cos( Vertical ) * MathEx.Cos( Horizontal );
			result.Y = MathEx.Cos( Vertical ) * MathEx.Sin( Horizontal );
			result.Z = MathEx.Sin( Vertical );
		}

		[AutoConvertType]
		public static SphericalDirectionF Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' }, 
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 2 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 2 parts separated by spaces in the form (h v).", text ) );

			try
			{
				return new SphericalDirectionF(
					float.Parse( vals[ 0 ] ),
					float.Parse( vals[ 1 ] ));
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
			//return string.Format( "{0} {1}", horizontal, vertical);
		}

		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "}";
			return string.Format( format, Horizontal, Vertical );
		}

#if !DISABLE_IMPLICIT
		public static implicit operator SphericalDirection( SphericalDirectionF v )
		{
			return new SphericalDirection( v );
		}
#endif
	}
}
