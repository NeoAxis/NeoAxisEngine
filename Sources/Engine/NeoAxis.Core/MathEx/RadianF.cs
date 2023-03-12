// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	///// <summary>
	///// Type converter for <see cref="RadianF"/> structure.
	///// </summary>
	//public class RadianFTypeConverter : TypeConverter
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
	//				return RadianF.Parse( (string)value );
	//			}
	//			catch( Exception )
	//			{
	//				return value;
	//			}
	//		}
	//		return base.ConvertFrom( context, culture, value );
	//	}
	//}

	//[TypeConverter( typeof( RadianFTypeConverter ) )]
	/// <summary>
	/// Represents a single precision radian unit value.
	/// </summary>
	public struct RadianF : IComparable<RadianF>/*dotfuscator not work, IComparable<Degree>*/, IComparable<float>
	{
		public static readonly RadianF Zero = new RadianF( 0.0f );

		float value;

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public RadianF( float r ) { value = r; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public RadianF( double r ) { value = (float)r; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public RadianF( int r ) { value = r; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RadianF( RadianF r ) { value = r.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RadianF( DegreeF d ) { value = d.InRadians(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public DegreeF InDegrees()
		{
			return MathEx.RadianToDegree( value );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator RadianF( float value ) { return new RadianF( value ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator RadianF( int value ) { return new RadianF( (float)value ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator RadianF( DegreeF value ) { return new RadianF( value ); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator float( RadianF value ) { return value.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public float ToFloat() { return value; }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator +( RadianF left, float right ) { return left.value + right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator +( RadianF left, int right ) { return left.value + right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator +( RadianF left, RadianF right ) { return left.value + right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator +( RadianF left, DegreeF right ) { return left + right.InRadians(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator -( RadianF r ) { return -r.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator -( RadianF left, float right ) { return left.value - right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator -( RadianF left, int right ) { return left.value - right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator -( RadianF left, RadianF right ) { return left.value - right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator -( RadianF left, DegreeF right ) { return left - right.InRadians(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator *( RadianF left, float right ) { return left.value * right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator *( RadianF left, int right ) { return left.value * right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator *( float left, RadianF right ) { return left * right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator *( int left, RadianF right ) { return left * right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator *( RadianF left, RadianF right ) { return left.value * right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator *( RadianF left, DegreeF right ) { return left.value * right.InRadians(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RadianF operator /( RadianF left, float right ) { return left.value / right; }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator <( RadianF left, RadianF right ) { return left.value < right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( RadianF left, RadianF right ) { return left.value == right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( RadianF left, RadianF right ) { return left.value != right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator >( RadianF left, RadianF right ) { return left.value > right.value; }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj ) { return ( obj is RadianF && this == (RadianF)obj ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode() { return value.GetHashCode(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int CompareTo( RadianF other ) { return this.value.CompareTo( other.value ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int CompareTo( DegreeF other ) { return this.value.CompareTo( other.InRadians() ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int CompareTo( float other ) { return this.value.CompareTo( other ); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public static RadianF Parse( string text ) { return new RadianF( float.Parse( text ) ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public override string ToString() { return value.ToString(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Radian ToRadian() { return new Radian( (double)value ); }

		//#if !DISABLE_IMPLICIT
		//		public static implicit operator Radian( RadianF v )
		//		{
		//			return (double)v;
		//		}
		//#endif
	}
}
