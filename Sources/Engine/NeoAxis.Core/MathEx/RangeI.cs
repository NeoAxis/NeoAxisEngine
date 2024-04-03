// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// A structure encapsulating an integer values range.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct RangeI
	{
		public int Minimum;
		public int Maximum;

		public static readonly RangeI Zero = new RangeI( 0, 0 );

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RangeI( RangeI a )
		{
			Minimum = a.Minimum;
			Maximum = a.Maximum;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RangeI( int minimum, int maximum )
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
		}

		[AutoConvertType]
		public static RangeI Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 2 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 2 parts separated by spaces in the form (x y).", text ) );

			try
			{
				return new RangeI(
					int.Parse( vals[ 0 ] ),
					int.Parse( vals[ 1 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be integer numbers." );
			}
		}

		public static bool TryParse( string text, out RangeI result )
		{
			try
			{
				result = Parse( text );
				return true;
			}
			catch
			{
				result = default;
				return false;
			}
		}

		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1}", Minimum, Maximum );
		}

		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "}";
			return string.Format( format, Minimum, Maximum );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is RangeI && this == (RangeI)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Minimum.GetHashCode() ^ Maximum.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RangeI operator *( RangeI v, int s )
		{
			RangeI result;
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RangeI operator *( int s, RangeI v )
		{
			RangeI result;
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RangeI operator /( RangeI v, int s )
		{
			RangeI result;
			result.Minimum = v.Minimum / s;
			result.Maximum = v.Maximum / s;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RangeI operator /( int s, RangeI v )
		{
			RangeI result;
			result.Minimum = s / v.Minimum;
			result.Maximum = s / v.Maximum;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RangeI operator -( RangeI v )
		{
			RangeI result;
			result.Minimum = -v.Minimum;
			result.Maximum = -v.Maximum;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref RangeI v, int s, out RangeI result )
		{
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( int s, ref RangeI v, out RangeI result )
		{
			result.Minimum = v.Minimum * s;
			result.Maximum = v.Maximum * s;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( ref RangeI v, int s, out RangeI result )
		{
			result.Minimum = v.Minimum / s;
			result.Maximum = v.Maximum / s;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( int s, ref RangeI v, out RangeI result )
		{
			result.Minimum = s / v.Minimum;
			result.Maximum = s / v.Maximum;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Negate( ref RangeI v, out RangeI result )
		{
			result.Minimum = -v.Minimum;
			result.Maximum = -v.Maximum;
		}

		//public static RangeI Multiply( RangeI v, int s )
		//{
		//	RangeI result;
		//	result.minimum = v.minimum * s;
		//	result.maximum = v.maximum * s;
		//	return result;
		//}

		//public static RangeI Multiply( int s, RangeI v )
		//{
		//	RangeI result;
		//	result.minimum = v.minimum * s;
		//	result.maximum = v.maximum * s;
		//	return result;
		//}

		//public static RangeI Divide( RangeI v, int s )
		//{
		//	RangeI result;
		//	result.minimum = v.minimum / s;
		//	result.maximum = v.maximum / s;
		//	return result;
		//}

		//public static RangeI Divide( int s, RangeI v )
		//{
		//	RangeI result;
		//	result.minimum = s / v.minimum;
		//	result.maximum = s / v.maximum;
		//	return result;
		//}

		//public static RangeI Negate( RangeI v )
		//{
		//	RangeI result;
		//	result.minimum = -v.minimum;
		//	result.maximum = -v.maximum;
		//	return result;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( RangeI v1, RangeI v2 )
		{
			return ( v1.Minimum == v2.Minimum && v1.Maximum == v2.Maximum );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( RangeI v1, RangeI v2 )
		{
			return ( v1.Minimum != v2.Minimum || v1.Maximum != v2.Maximum );
		}

		public unsafe int this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				//!!!!

				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( int* v = &this.Minimum )
				{
					return v[ index ];
				}
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( int* v = &this.Minimum )
				{
					v[ index ] = value;
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( RangeI v, int epsilon )
		{
			if( Math.Abs( Minimum - v.Minimum ) > epsilon )
				return false;
			if( Math.Abs( Maximum - v.Maximum ) > epsilon )
				return false;
			return true;
		}

		[Browsable( false )]
		public int Size
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return Maximum - Minimum; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector2I ToVector2I()
		{
			Vector2I result;
			result.X = Minimum;
			result.Y = Maximum;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public RangeF ToRangeF()
		{
			RangeF result;
			result.Minimum = Minimum;
			result.Maximum = Maximum;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Range ToRange()
		{
			Range result;
			result.Minimum = Minimum;
			result.Maximum = Maximum;
			return result;
		}
	}
}
