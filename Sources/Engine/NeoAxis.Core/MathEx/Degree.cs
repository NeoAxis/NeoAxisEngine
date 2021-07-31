// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NeoAxis
{
	///// <summary>
	///// Type converter for <see cref="Degree"/> structure.
	///// </summary>
	//public class DegreeTypeConverter : TypeConverter
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
	//				return Degree.Parse( (string)value );
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
	/// Represents a double precision degree unit value.
	/// </summary>
	public struct Degree : IComparable<Degree>/*dotfuscator not work, IComparable<RadianD>*/, IComparable<double>
	{
		public static readonly Degree Zero = new Degree( 0.0 );

		double value;

		[AutoConvertType]
		public Degree( double r ) { value = r; }
		[AutoConvertType]
		public Degree( float r ) { value = r; }
		[AutoConvertType]
		public Degree( int r ) { value = r; }
		public Degree( Degree d ) { value = d.value; }
		public Degree( Radian r ) { value = r.InDegrees(); }

		[AutoConvertType]
		public Radian InRadians()
		{
			return MathEx.DegreeToRadian( value );
		}

		public static implicit operator Degree( double value ) { return new Degree( value ); }
		public static implicit operator Degree( int value ) { return new Degree( (double)value ); }
		public static implicit operator Degree( Radian value ) { return new Degree( value ); }
		public static implicit operator Degree( DegreeF value ) { return new Degree( (double)value ); }

		public static implicit operator double( Degree value ) { return value.value; }
		[AutoConvertType]
		public double ToDouble() { return value; }

		public static Degree operator +( Degree left, double right ) { return left.value + right; }
		public static Degree operator +( Degree left, int right ) { return left.value + right; }
		public static Degree operator +( Degree left, Degree right ) { return left.value + right.value; }
		public static Degree operator +( Degree left, Radian right ) { return left + right.InDegrees(); }

		public static Degree operator -( Degree r ) { return -r.value; }
		public static Degree operator -( Degree left, double right ) { return left.value - right; }
		public static Degree operator -( Degree left, int right ) { return left.value - right; }
		public static Degree operator -( Degree left, Degree right ) { return left.value - right.value; }
		public static Degree operator -( Degree left, Radian right ) { return left - right.InDegrees(); }

		public static Degree operator *( Degree left, double right ) { return left.value * right; }
		public static Degree operator *( Degree left, int right ) { return left.value * right; }
		public static Degree operator *( double left, Degree right ) { return left * right.value; }
		public static Degree operator *( int left, Degree right ) { return left * right.value; }
		public static Degree operator *( Degree left, Degree right ) { return left.value * right.value; }
		public static Degree operator *( Degree left, Radian right ) { return left.value * right.InDegrees(); }

		public static Degree operator /( Degree left, double right ) { return left.value / right; }

		public static bool operator <( Degree left, Degree right ) { return left.value < right.value; }
		public static bool operator ==( Degree left, Degree right ) { return left.value == right.value; }
		public static bool operator !=( Degree left, Degree right ) { return left.value != right.value; }
		public static bool operator >( Degree left, Degree right ) { return left.value > right.value; }

		public override bool Equals( object obj ) { return ( obj is Degree && this == (Degree)obj ); }
		public override int GetHashCode() { return value.GetHashCode(); }

		public int CompareTo( Degree other ) { return value.CompareTo( other ); }
		public int CompareTo( Radian other ) { return value.CompareTo( other.InDegrees() ); }
		public int CompareTo( double other ) { return value.CompareTo( other ); }

		[AutoConvertType]
		public static Degree Parse( string text ) { return new Degree( double.Parse( text ) ); }
		[AutoConvertType]
		public override string ToString() { return value.ToString(); }

		[AutoConvertType]
		public DegreeF ToDegreeF() { return new DegreeF( (float)value ); }
	}
}
