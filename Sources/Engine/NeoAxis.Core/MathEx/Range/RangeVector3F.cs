// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A structure encapsulating a single precision vector 3 range.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct RangeVector3F
	{
		public Vector3F Minimum;
		public Vector3F Maximum;

		public static readonly RangeVector3F Zero = new RangeVector3F( Vector3F.Zero, Vector3F.Zero );

		public RangeVector3F( RangeVector3F a )
		{
			Minimum = a.Minimum;
			Maximum = a.Maximum;
		}

		public RangeVector3F( Vector3F minimum, Vector3F maximum )
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
		}

		[AutoConvertType]
		public static RangeVector3F Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 6 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 6 parts separated by spaces in the form (x y).", text ) );

			try
			{
				return new RangeVector3F(
					new Vector3F( float.Parse( vals[ 0 ] ), float.Parse( vals[ 1 ] ), float.Parse( vals[ 2 ] ) ),
					new Vector3F( float.Parse( vals[ 3 ] ), float.Parse( vals[ 4 ] ), float.Parse( vals[ 5 ] ) ) );
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
			//return string.Format( "{0} {1}", minimum, maximum );
		}

		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "} {3:0." + format + "} {4:0." + format + "} {5:0." + format + "}";
			return string.Format( format, Minimum.X, Minimum.Y, Minimum.Z, Maximum.X, Maximum.Y, Maximum.Z );
		}

		public override bool Equals( object obj )
		{
			return ( obj is RangeVector3F && this == (RangeVector3F)obj );
		}

		public override int GetHashCode()
		{
			return ( Minimum.GetHashCode() ^ Maximum.GetHashCode() );
		}

		//!!!!множить, делить на Vector3F

		public static RangeVector3F operator *( RangeVector3F v, float s )
		{
			RangeVector3F result;
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
			return result;
		}

		public static RangeVector3F operator *( float s, RangeVector3F v )
		{
			RangeVector3F result;
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
			return result;
		}

		public static RangeVector3F operator /( RangeVector3F v, float s )
		{
			RangeVector3F result;
			float invS = 1.0f / s;
			result.Minimum = v.Minimum * invS;
			result.Maximum = v.Maximum * invS;
			return result;
		}

		public static RangeVector3F operator /( float s, RangeVector3F v )
		{
			RangeVector3F result;
			result.Minimum = s / v.Minimum;
			result.Maximum = s / v.Maximum;
			return result;
		}

		public static RangeVector3F operator -( RangeVector3F v )
		{
			RangeVector3F result;
			result.Minimum = -v.Minimum;
			result.Maximum = -v.Maximum;
			return result;
		}

		public static void Multiply( ref RangeVector3F v, float s, out RangeVector3F result )
		{
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
		}

		public static void Multiply( float s, ref RangeVector3F v, out RangeVector3F result )
		{
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
		}

		public static void Divide( ref RangeVector3F v, float s, out RangeVector3F result )
		{
			float invS = 1.0f / s;
			result.Minimum = v.Minimum * invS;
			result.Maximum = v.Maximum * invS;
		}

		public static void Divide( float s, ref RangeVector3F v, out RangeVector3F result )
		{
			result.Minimum = s / v.Minimum;
			result.Maximum = s / v.Maximum;
		}

		public static void Negate( ref RangeVector3F v, out RangeVector3F result )
		{
			result.Minimum = -v.Minimum;
			result.Maximum = -v.Maximum;
		}

		//public static RangeVector3F Multiply( RangeVector3F v, float s )
		//{
		//	RangeVector3F result;
		//	result.minimum = v.minimum * s;
		//	result.maximum = v.maximum * s;
		//	return result;
		//}

		//public static RangeVector3F Multiply( float s, RangeVector3F v )
		//{
		//	RangeVector3F result;
		//	result.minimum = v.minimum * s;
		//	result.maximum = v.maximum * s;
		//	return result;
		//}

		//public static RangeVector3F Divide( RangeVector3F v, float s )
		//{
		//	RangeVector3F result;
		//	float invS = 1.0f / s;
		//	result.minimum = v.minimum * invS;
		//	result.maximum = v.maximum * invS;
		//	return result;
		//}

		//public static RangeVector3F Divide( float s, RangeVector3F v )
		//{
		//	RangeVector3F result;
		//	result.minimum = s / v.minimum;
		//	result.maximum = s / v.maximum;
		//	return result;
		//}

		//public static RangeVector3F Negate( RangeVector3F v )
		//{
		//	RangeVector3F result;
		//	result.minimum = -v.minimum;
		//	result.maximum = -v.maximum;
		//	return result;
		//}

		public static bool operator ==( RangeVector3F v1, RangeVector3F v2 )
		{
			return ( v1.Minimum == v2.Minimum && v1.Maximum == v2.Maximum );
		}

		public static bool operator !=( RangeVector3F v1, RangeVector3F v2 )
		{
			return ( v1.Minimum != v2.Minimum || v1.Maximum != v2.Maximum );
		}

		public unsafe Vector3F this[ int index ]
		{
			get
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector3F* v = &this.Minimum )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector3F* v = &this.Minimum )
				{
					v[ index ] = value;
				}
			}
		}

		public bool Equals( RangeVector3F v, float epsilon )
		{
			return Minimum.Equals( v.Minimum, epsilon ) && Maximum.Equals( v.Maximum, epsilon );
		}

		[Browsable( false )]
		public Vector3F Size
		{
			get { return Maximum - Minimum; }
		}

		//[AutoConvertType]
		//public Vector2F ToVector2()
		//{
		//	Vector2F result;
		//	result.X = Minimum;
		//	result.Y = Maximum;
		//	return result;
		//}

		//[AutoConvertType]
		//public Range ToRange()
		//{
		//	Range result;
		//	result.Minimum = Minimum;
		//	result.Maximum = Maximum;
		//	return result;
		//}

		//[AutoConvertType]
		//public RangeI ToRangeI()
		//{
		//	RangeI result;
		//	result.Minimum = (int)Minimum;
		//	result.Maximum = (int)Maximum;
		//	return result;
		//}

		//#if !DISABLE_IMPLICIT
		//		public static implicit operator Range( RangeVector3F v )
		//		{
		//			return new Range( v );
		//		}
		//#endif
	}
}
