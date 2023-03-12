// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	///// <summary>
	///// Type converter for <see cref="DegreeF"/> structure.
	///// </summary>
	//public class DegreeFTypeConverter : TypeConverter
	//{
	//	public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
	//	{
	//		return sourceType == typeof( string );
	//	}

	//	public override object ConvertFrom( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value )
	//	{
	//		if( value.GetType() == typeof( string ) )
	//		{
	//			try
	//			{
	//				return DegreeF.Parse( (string)value );
	//			}
	//			catch( Exception )
	//			{
	//				return value;
	//			}
	//		}
	//		return base.ConvertFrom( context, culture, value );
	//	}
	//}

	//[TypeConverter( typeof( DegreeFTypeConverter ) )]
	/// <summary>
	/// Represents a single precision degree unit value.
	/// </summary>
	public struct DegreeF : IComparable<DegreeF>/*dotfuscator not work, IComparable<Radian>*/, IComparable<float>
	{
		public static readonly DegreeF Zero = new DegreeF( 0.0f );

		float value;

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public DegreeF( float r ) { value = r; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public DegreeF( double r ) { value = (float)r; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public DegreeF( int r ) { value = r; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public DegreeF( DegreeF d ) { value = d.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public DegreeF( RadianF r ) { value = r.InDegrees(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public RadianF InRadians()
		{
			return MathEx.DegreeToRadian( value );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator DegreeF( float value ) { return new DegreeF( value ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator DegreeF( int value ) { return new DegreeF( (float)value ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator DegreeF( RadianF value ) { return new DegreeF( value ); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator float( DegreeF value ) { return value.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public float ToFloat() { return value; }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator +( DegreeF left, float right ) { return left.value + right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator +( DegreeF left, int right ) { return left.value + right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator +( DegreeF left, DegreeF right ) { return left.value + right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator +( DegreeF left, RadianF right ) { return left + right.InDegrees(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator -( DegreeF r ) { return -r.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator -( DegreeF left, float right ) { return left.value - right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator -( DegreeF left, int right ) { return left.value - right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator -( DegreeF left, DegreeF right ) { return left.value - right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator -( DegreeF left, RadianF right ) { return left - right.InDegrees(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator *( DegreeF left, float right ) { return left.value * right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator *( DegreeF left, int right ) { return left.value * right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator *( float left, DegreeF right ) { return left * right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator *( int left, DegreeF right ) { return left * right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator *( DegreeF left, DegreeF right ) { return left.value * right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator *( DegreeF left, RadianF right ) { return left.value * right.InDegrees(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static DegreeF operator /( DegreeF left, float right ) { return left.value / right; }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator <( DegreeF left, DegreeF right ) { return left.value < right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( DegreeF left, DegreeF right ) { return left.value == right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( DegreeF left, DegreeF right ) { return left.value != right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator >( DegreeF left, DegreeF right ) { return left.value > right.value; }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj ) { return ( obj is DegreeF && this == (DegreeF)obj ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode() { return value.GetHashCode(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int CompareTo( DegreeF other ) { return value.CompareTo( other ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int CompareTo( RadianF other ) { return value.CompareTo( other.InDegrees() ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int CompareTo( float other ) { return value.CompareTo( other ); }

		[AutoConvertType]
		public static DegreeF Parse( string text ) { return new DegreeF( float.Parse( text ) ); }
		[AutoConvertType]
		public override string ToString() { return value.ToString(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Degree ToDegree() { return new Degree( (double)value ); }

		//#if !DISABLE_IMPLICIT
		//		public static implicit operator Degree( DegreeF v )
		//		{
		//			return (double)v;
		//		}
		//#endif
	}
}
