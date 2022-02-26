// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

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

		[AutoConvertType]
		public DegreeF( float r ) { value = r; }
		[AutoConvertType]
		public DegreeF( double r ) { value = (float)r; }
		[AutoConvertType]
		public DegreeF( int r ) { value = r; }
		public DegreeF( DegreeF d ) { value = d.value; }
		public DegreeF( RadianF r ) { value = r.InDegrees(); }

		[AutoConvertType]
		public RadianF InRadians()
		{
			return MathEx.DegreeToRadian( value );
		}

		public static implicit operator DegreeF( float value ) { return new DegreeF( value ); }
		public static implicit operator DegreeF( int value ) { return new DegreeF( (float)value ); }
		public static implicit operator DegreeF( RadianF value ) { return new DegreeF( value ); }

		public static implicit operator float( DegreeF value ) { return value.value; }
		[AutoConvertType]
		public float ToFloat() { return value; }

		public static DegreeF operator +( DegreeF left, float right ) { return left.value + right; }
		public static DegreeF operator +( DegreeF left, int right ) { return left.value + right; }
		public static DegreeF operator +( DegreeF left, DegreeF right ) { return left.value + right.value; }
		public static DegreeF operator +( DegreeF left, RadianF right ) { return left + right.InDegrees(); }

		public static DegreeF operator -( DegreeF r ) { return -r.value; }
		public static DegreeF operator -( DegreeF left, float right ) { return left.value - right; }
		public static DegreeF operator -( DegreeF left, int right ) { return left.value - right; }
		public static DegreeF operator -( DegreeF left, DegreeF right ) { return left.value - right.value; }
		public static DegreeF operator -( DegreeF left, RadianF right ) { return left - right.InDegrees(); }

		public static DegreeF operator *( DegreeF left, float right ) { return left.value * right; }
		public static DegreeF operator *( DegreeF left, int right ) { return left.value * right; }
		public static DegreeF operator *( float left, DegreeF right ) { return left * right.value; }
		public static DegreeF operator *( int left, DegreeF right ) { return left * right.value; }
		public static DegreeF operator *( DegreeF left, DegreeF right ) { return left.value * right.value; }
		public static DegreeF operator *( DegreeF left, RadianF right ) { return left.value * right.InDegrees(); }

		public static DegreeF operator /( DegreeF left, float right ) { return left.value / right; }

		public static bool operator <( DegreeF left, DegreeF right ) { return left.value < right.value; }
		public static bool operator ==( DegreeF left, DegreeF right ) { return left.value == right.value; }
		public static bool operator !=( DegreeF left, DegreeF right ) { return left.value != right.value; }
		public static bool operator >( DegreeF left, DegreeF right ) { return left.value > right.value; }

		public override bool Equals( object obj ) { return ( obj is DegreeF && this == (DegreeF)obj ); }
		public override int GetHashCode() { return value.GetHashCode(); }

		public int CompareTo( DegreeF other ) { return value.CompareTo( other ); }
		public int CompareTo( RadianF other ) { return value.CompareTo( other.InDegrees() ); }
		public int CompareTo( float other ) { return value.CompareTo( other ); }

		[AutoConvertType]
		public static DegreeF Parse( string text ) { return new DegreeF( float.Parse( text ) ); }
		[AutoConvertType]
		public override string ToString() { return value.ToString(); }

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
