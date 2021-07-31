// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A structure encapsulating a single precision range.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct RangeF
	{
		public float Minimum;
		public float Maximum;

		public static readonly RangeF Zero = new RangeF( 0.0f, 0.0f );

		public RangeF( RangeF a )
		{
			Minimum = a.Minimum;
			Maximum = a.Maximum;
		}

		public RangeF( float minimum, float maximum )
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
		}

		[AutoConvertType]
		public static RangeF Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 2 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 2 parts separated by spaces in the form (x y).", text ) );

			try
			{
				return new RangeF(
					float.Parse( vals[ 0 ] ),
					float.Parse( vals[ 1 ] ) );
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
			format = "{0:0." + format + "} {1:0." + format + "}";
			return string.Format( format, Minimum, Maximum );
		}

		public override bool Equals( object obj )
		{
			return ( obj is RangeF && this == (RangeF)obj );
		}

		public override int GetHashCode()
		{
			return ( Minimum.GetHashCode() ^ Maximum.GetHashCode() );
		}

		public static RangeF operator *( RangeF v, float s )
		{
			RangeF result;
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
			return result;
		}

		public static RangeF operator *( float s, RangeF v )
		{
			RangeF result;
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
			return result;
		}

		public static RangeF operator /( RangeF v, float s )
		{
			RangeF result;
			float invS = 1.0f / s;
			result.Minimum = v.Minimum * invS;
			result.Maximum = v.Maximum * invS;
			return result;
		}

		public static RangeF operator /( float s, RangeF v )
		{
			RangeF result;
			result.Minimum = s / v.Minimum;
			result.Maximum = s / v.Maximum;
			return result;
		}

		public static RangeF operator -( RangeF v )
		{
			RangeF result;
			result.Minimum = -v.Minimum;
			result.Maximum = -v.Maximum;
			return result;
		}

		public static void Multiply( ref RangeF v, float s, out RangeF result )
		{
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
		}

		public static void Multiply( float s, ref RangeF v, out RangeF result )
		{
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
		}

		public static void Divide( ref RangeF v, float s, out RangeF result )
		{
			float invS = 1.0f / s;
			result.Minimum = v.Minimum * invS;
			result.Maximum = v.Maximum * invS;
		}

		public static void Divide( float s, ref RangeF v, out RangeF result )
		{
			result.Minimum = s / v.Minimum;
			result.Maximum = s / v.Maximum;
		}

		public static void Negate( ref RangeF v, out RangeF result )
		{
			result.Minimum = -v.Minimum;
			result.Maximum = -v.Maximum;
		}

		//public static RangeF Multiply( RangeF v, float s )
		//{
		//	RangeF result;
		//	result.minimum = v.minimum * s;
		//	result.maximum = v.maximum * s;
		//	return result;
		//}

		//public static RangeF Multiply( float s, RangeF v )
		//{
		//	RangeF result;
		//	result.minimum = v.minimum * s;
		//	result.maximum = v.maximum * s;
		//	return result;
		//}

		//public static RangeF Divide( RangeF v, float s )
		//{
		//	RangeF result;
		//	float invS = 1.0f / s;
		//	result.minimum = v.minimum * invS;
		//	result.maximum = v.maximum * invS;
		//	return result;
		//}

		//public static RangeF Divide( float s, RangeF v )
		//{
		//	RangeF result;
		//	result.minimum = s / v.minimum;
		//	result.maximum = s / v.maximum;
		//	return result;
		//}

		//public static RangeF Negate( RangeF v )
		//{
		//	RangeF result;
		//	result.minimum = -v.minimum;
		//	result.maximum = -v.maximum;
		//	return result;
		//}

		public static bool operator ==( RangeF v1, RangeF v2 )
		{
			return ( v1.Minimum == v2.Minimum && v1.Maximum == v2.Maximum );
		}

		public static bool operator !=( RangeF v1, RangeF v2 )
		{
			return ( v1.Minimum != v2.Minimum || v1.Maximum != v2.Maximum );
		}

		public unsafe float this[ int index ]
		{
			get
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( float* v = &this.Minimum )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( float* v = &this.Minimum )
				{
					v[ index ] = value;
				}
			}
		}

		public bool Equals( RangeF v, float epsilon )
		{
			if( Math.Abs( Minimum - v.Minimum ) > epsilon )
				return false;
			if( Math.Abs( Maximum - v.Maximum ) > epsilon )
				return false;
			return true;
		}

		[Browsable( false )]
		public float Size
		{
			get { return Maximum - Minimum; }
		}

		[AutoConvertType]
		public Vector2F ToVector2()
		{
			Vector2F result;
			result.X = Minimum;
			result.Y = Maximum;
			return result;
		}

		[AutoConvertType]
		public Range ToRange()
		{
			Range result;
			result.Minimum = Minimum;
			result.Maximum = Maximum;
			return result;
		}

		[AutoConvertType]
		public RangeI ToRangeI()
		{
			RangeI result;
			result.Minimum = (int)Minimum;
			result.Maximum = (int)Maximum;
			return result;
		}

#if !DISABLE_IMPLICIT
		public static implicit operator Range( RangeF v )
		{
			return new Range( v );
		}
#endif

		public float GetCenter()
		{
			return ( Minimum + Maximum ) / 2;
		}
	}
}
