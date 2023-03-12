// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	///// <summary>
	///// Type converter for <see cref="Radian"/> structure.
	///// </summary>
	//public class RadianTypeConverter : TypeConverter
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
	//				return Radian.Parse( (string)value );
	//			}
	//			catch( Exception )
	//			{
	//				return value;
	//			}
	//		}
	//		return base.ConvertFrom( context, culture, value );
	//	}
	//}

	//[TypeConverter( typeof( RadianTypeConverter ) )]
	/// <summary>
	/// Represents a double precision radian unit value.
	/// </summary>
	public struct Radian : IComparable<Radian>/*dotfuscator not work, IComparable<Degree>*/, IComparable<double>
	{
		public static readonly Radian Zero = new Radian( 0.0 );

		double value;

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Radian( double r ) { value = r; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Radian( float r ) { value = r; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Radian( int r ) { value = r; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Radian( Radian r ) { value = r.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Radian( Degree d ) { value = d.InRadians(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Degree InDegrees()
		{
			return MathEx.RadianToDegree( value );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Radian( double value ) { return new Radian( value ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Radian( int value ) { return new Radian( (double)value ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Radian( Degree value ) { return new Radian( value ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator Radian( RadianF value ) { return new Radian( (double)value ); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static implicit operator double( Radian value ) { return value.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public double ToDouble() { return value; }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator +( Radian left, double right ) { return left.value + right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator +( Radian left, int right ) { return left.value + right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator +( Radian left, Radian right ) { return left.value + right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator +( Radian left, Degree right ) { return left + right.InRadians(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator -( Radian r ) { return -r.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator -( Radian left, double right ) { return left.value - right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator -( Radian left, int right ) { return left.value - right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator -( Radian left, Radian right ) { return left.value - right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator -( Radian left, Degree right ) { return left - right.InRadians(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator *( Radian left, double right ) { return left.value * right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator *( Radian left, int right ) { return left.value * right; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator *( double left, Radian right ) { return left * right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator *( int left, Radian right ) { return left * right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator *( Radian left, Radian right ) { return left.value * right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator *( Radian left, Degree right ) { return left.value * right.InRadians(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Radian operator /( Radian left, double right ) { return left.value / right; }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator <( Radian left, Radian right ) { return left.value < right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Radian left, Radian right ) { return left.value == right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Radian left, Radian right ) { return left.value != right.value; }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator >( Radian left, Radian right ) { return left.value > right.value; }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj ) { return ( obj is Radian && this == (Radian)obj ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode() { return value.GetHashCode(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int CompareTo( Radian other ) { return this.value.CompareTo( other.value ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int CompareTo( Degree other ) { return this.value.CompareTo( other.InRadians() ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public int CompareTo( double other ) { return this.value.CompareTo( other ); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public static Radian Parse( string text ) { return new Radian( double.Parse( text ) ); }
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public override string ToString() { return value.ToString(); }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public RadianF ToRadianF() { return new RadianF( (float)value ); }
	}
}
