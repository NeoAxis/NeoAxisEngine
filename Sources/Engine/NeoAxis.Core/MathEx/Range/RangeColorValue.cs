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
	public struct RangeColorValue
	{
		public ColorValue Minimum;
		public ColorValue Maximum;

		public static readonly RangeColorValue Zero = new RangeColorValue( ColorValue.Zero, ColorValue.Zero );

		public RangeColorValue( RangeColorValue a )
		{
			Minimum = a.Minimum;
			Maximum = a.Maximum;
		}

		public RangeColorValue( ColorValue minimum, ColorValue maximum )
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
		}

		[AutoConvertType]
		public static RangeColorValue Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 8 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 8 parts separated by spaces in the form (x y).", text ) );

			try
			{
				return new RangeColorValue(
					new ColorValue( float.Parse( vals[ 0 ] ), float.Parse( vals[ 1 ] ), float.Parse( vals[ 2 ] ), float.Parse( vals[ 3 ] ) ),
					new ColorValue( float.Parse( vals[ 4 ] ), float.Parse( vals[ 5 ] ), float.Parse( vals[ 6 ] ), float.Parse( vals[ 7 ] ) ) );
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
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "} {3:0." + format + "} {4:0." + format + "} {5:0." + format + "} {6:0." + format + "} {7:0." + format + "}";
			return string.Format( format, Minimum[ 0 ], Minimum[ 1 ], Minimum[ 2 ], Minimum[ 3 ], Maximum[ 0 ], Maximum[ 1 ], Maximum[ 2 ], Maximum[ 3 ] );
		}

		public override bool Equals( object obj )
		{
			return ( obj is RangeColorValue && this == (RangeColorValue)obj );
		}

		public override int GetHashCode()
		{
			return ( Minimum.GetHashCode() ^ Maximum.GetHashCode() );
		}

		//!!!!множить, делить на Vector3F

		public static RangeColorValue operator *( RangeColorValue v, float s )
		{
			RangeColorValue result;
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
			return result;
		}

		public static RangeColorValue operator *( float s, RangeColorValue v )
		{
			RangeColorValue result;
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
			return result;
		}

		public static RangeColorValue operator /( RangeColorValue v, float s )
		{
			RangeColorValue result;
			float invS = 1.0f / s;
			result.Minimum = v.Minimum * invS;
			result.Maximum = v.Maximum * invS;
			return result;
		}

		public static RangeColorValue operator /( float s, RangeColorValue v )
		{
			RangeColorValue result;
			result.Minimum = s / v.Minimum;
			result.Maximum = s / v.Maximum;
			return result;
		}

		public static RangeColorValue operator -( RangeColorValue v )
		{
			RangeColorValue result;
			result.Minimum = -v.Minimum;
			result.Maximum = -v.Maximum;
			return result;
		}

		public static void Multiply( ref RangeColorValue v, float s, out RangeColorValue result )
		{
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
		}

		public static void Multiply( float s, ref RangeColorValue v, out RangeColorValue result )
		{
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
		}

		public static void Divide( ref RangeColorValue v, float s, out RangeColorValue result )
		{
			float invS = 1.0f / s;
			result.Minimum = v.Minimum * invS;
			result.Maximum = v.Maximum * invS;
		}

		public static void Divide( float s, ref RangeColorValue v, out RangeColorValue result )
		{
			result.Minimum = s / v.Minimum;
			result.Maximum = s / v.Maximum;
		}

		public static void Negate( ref RangeColorValue v, out RangeColorValue result )
		{
			result.Minimum = -v.Minimum;
			result.Maximum = -v.Maximum;
		}

		//public static RangeColorValue Multiply( RangeColorValue v, float s )
		//{
		//	RangeColorValue result;
		//	result.minimum = v.minimum * s;
		//	result.maximum = v.maximum * s;
		//	return result;
		//}

		//public static RangeColorValue Multiply( float s, RangeColorValue v )
		//{
		//	RangeColorValue result;
		//	result.minimum = v.minimum * s;
		//	result.maximum = v.maximum * s;
		//	return result;
		//}

		//public static RangeColorValue Divide( RangeColorValue v, float s )
		//{
		//	RangeColorValue result;
		//	float invS = 1.0f / s;
		//	result.minimum = v.minimum * invS;
		//	result.maximum = v.maximum * invS;
		//	return result;
		//}

		//public static RangeColorValue Divide( float s, RangeColorValue v )
		//{
		//	RangeColorValue result;
		//	result.minimum = s / v.minimum;
		//	result.maximum = s / v.maximum;
		//	return result;
		//}

		//public static RangeColorValue Negate( RangeColorValue v )
		//{
		//	RangeColorValue result;
		//	result.minimum = -v.minimum;
		//	result.maximum = -v.maximum;
		//	return result;
		//}

		public static bool operator ==( RangeColorValue v1, RangeColorValue v2 )
		{
			return ( v1.Minimum == v2.Minimum && v1.Maximum == v2.Maximum );
		}

		public static bool operator !=( RangeColorValue v1, RangeColorValue v2 )
		{
			return ( v1.Minimum != v2.Minimum || v1.Maximum != v2.Maximum );
		}

		public unsafe ColorValue this[ int index ]
		{
			get
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( ColorValue* v = &this.Minimum )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( ColorValue* v = &this.Minimum )
				{
					v[ index ] = value;
				}
			}
		}

		public bool Equals( RangeColorValue v, float epsilon )
		{
			return Minimum.Equals( v.Minimum, epsilon ) && Maximum.Equals( v.Maximum, epsilon );
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
		//		public static implicit operator Range( RangeColorValue v )
		//		{
		//			return new Range( v );
		//		}
		//#endif
	}
}
