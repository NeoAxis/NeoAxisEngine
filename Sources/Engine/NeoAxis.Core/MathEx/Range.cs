// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	//[TypeConverter( typeof( RangeConverter ) )]
	/// <summary>
	/// A structure encapsulating a double precision range.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Range
	{
		public double Minimum;
		public double Maximum;

		public static readonly Range Zero = new Range( 0.0, 0.0 );

		public Range( Range a )
		{
			Minimum = a.Minimum;
			Maximum = a.Maximum;
		}

		public Range( double minimum, double maximum )
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
		}

		public Range( RangeF a )
		{
			Minimum = a.Minimum;
			Maximum = a.Maximum;
		}

		[AutoConvertType]
		public static Range Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 2 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 2 parts separated by spaces in the form (x y).", text ) );

			try
			{
				return new Range(
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
			return ( obj is Range && this == (Range)obj );
		}

		public override int GetHashCode()
		{
			return ( Minimum.GetHashCode() ^ Maximum.GetHashCode() );
		}

		public static Range operator *( Range v, double s )
		{
			Range result;
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
			return result;
		}

		public static Range operator *( double s, Range v )
		{
			Range result;
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
			return result;
		}

		public static Range operator /( Range v, double s )
		{
			Range result;
			double invS = 1.0 / s;
			result.Minimum = v.Minimum * invS;
			result.Maximum = v.Maximum * invS;
			return result;
		}

		public static Range operator /( double s, Range v )
		{
			Range result;
			result.Minimum = s / v.Minimum;
			result.Maximum = s / v.Maximum;
			return result;
		}

		public static Range operator -( Range v )
		{
			Range result;
			result.Minimum = -v.Minimum;
			result.Maximum = -v.Maximum;
			return result;
		}

		public static void Multiply( ref Range v, double s, out Range result )
		{
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
		}

		public static void Multiply( double s, ref Range v, out Range result )
		{
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
		}

		public static void Divide( ref Range v, double s, out Range result )
		{
			double invS = 1.0 / s;
			result.Minimum = v.Minimum * invS;
			result.Maximum = v.Maximum * invS;
		}

		public static void Divide( double s, ref Range v, out Range result )
		{
			result.Minimum = s / v.Minimum;
			result.Maximum = s / v.Maximum;
		}

		public static void Negate( ref Range v, out Range result )
		{
			result.Minimum = -v.Minimum;
			result.Maximum = -v.Maximum;
		}

		//public static Range Multiply( Range v, double s )
		//{
		//	Range result;
		//	result.minimum = v.minimum * s;
		//	result.maximum = v.maximum * s;
		//	return result;
		//}

		//public static Range Multiply( double s, Range v )
		//{
		//	Range result;
		//	result.minimum = v.minimum * s;
		//	result.maximum = v.maximum * s;
		//	return result;
		//}

		//public static Range Divide( Range v, double s )
		//{
		//	Range result;
		//	double invS = 1.0 / s;
		//	result.minimum = v.minimum * invS;
		//	result.maximum = v.maximum * invS;
		//	return result;
		//}

		//public static Range Divide( double s, Range v )
		//{
		//	Range result;
		//	result.minimum = s / v.minimum;
		//	result.maximum = s / v.maximum;
		//	return result;
		//}

		//public static Range Negate( Range v )
		//{
		//	Range result;
		//	result.minimum = -v.minimum;
		//	result.maximum = -v.maximum;
		//	return result;
		//}

		public static bool operator ==( Range v1, Range v2 )
		{
			return ( v1.Minimum == v2.Minimum && v1.Maximum == v2.Maximum );
		}

		public static bool operator !=( Range v1, Range v2 )
		{
			return ( v1.Minimum != v2.Minimum || v1.Maximum != v2.Maximum );
		}

		public unsafe double this[ int index ]
		{
			get
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( double* v = &this.Minimum )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( double* v = &this.Minimum )
				{
					v[ index ] = value;
				}
			}
		}

		public bool Equals( Range v, double epsilon )
		{
			if( Math.Abs( Minimum - v.Minimum ) > epsilon )
				return false;
			if( Math.Abs( Maximum - v.Maximum ) > epsilon )
				return false;
			return true;
		}

		[Browsable( false )]
		public double Size
		{
			get { return Maximum - Minimum; }
		}

		[AutoConvertType]
		public Vector2 ToVector2()
		{
			Vector2 result;
			result.X = Minimum;
			result.Y = Maximum;
			return result;
		}

		[AutoConvertType]
		public RangeF ToRangeF()
		{
			RangeF result;
			result.Minimum = (float)Minimum;
			result.Maximum = (float)Maximum;
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

		public double GetCenter()
		{
			return ( Minimum + Maximum ) / 2;
		}
	}
}
