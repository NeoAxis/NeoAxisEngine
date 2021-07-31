// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Drawing;

namespace NeoAxis
{
	/// <summary>
	/// Describes a 32-bit packed color.
	/// </summary>
	//[TypeConverter( typeof( MathExGeneralTypeConverter<ColorByte> ) )]
	[StructLayout( LayoutKind.Sequential )]
	public struct ColorByte : IEquatable<ColorByte>
	{
		public static readonly ColorByte Zero = new ColorByte( 0 );
		public static readonly ColorByte One = new ColorByte( uint.MaxValue );

		// Stored as RGBA with R in the least significant octet:
		// |-------|-------|-------|-------
		// A       B       G       R
		private uint _packedValue;

		/// <summary>
		/// Constructs an RGBA color from a packed value.
		/// The value is a 32-bit unsigned integer, with R in the least significant octet.
		/// </summary>
		/// <param name="packedValue">The packed value.</param>
		[CLSCompliant( false )]
		public ColorByte( uint packedValue )
		{
			_packedValue = packedValue;
		}

		/// <summary>
		/// Constructs an RGBA color from the XYZW unit length components of a vector.
		/// </summary>
		/// <param name="color">A <see cref="Vector4"/> representing color.</param>
		[AutoConvertType]
		public ColorByte( Vector4 color )
			: this( (int)( color.X * 255 ), (int)( color.Y * 255 ), (int)( color.Z * 255 ), (int)( color.W * 255 ) )
		{
		}

		/// <summary>
		/// Constructs an RGBA color from the XYZ unit length components of a vector. Alpha value will be opaque.
		/// </summary>
		/// <param name="color">A <see cref="Vector3"/> representing color.</param>
		[AutoConvertType]
		public ColorByte( Vector3 color )
			: this( (int)( color.X * 255 ), (int)( color.Y * 255 ), (int)( color.Z * 255 ) )
		{
		}

		/// <summary>
		/// Constructs an RGBA color from the XYZW unit length components of a vector.
		/// </summary>
		/// <param name="color">A <see cref="Vector4"/> representing color.</param>
		[AutoConvertType]
		public ColorByte( Vector4F color )
			: this( (int)( color.X * 255 ), (int)( color.Y * 255 ), (int)( color.Z * 255 ), (int)( color.W * 255 ) )
		{
		}

		/// <summary>
		/// Constructs an RGBA color from the XYZ unit length components of a vector. Alpha value will be opaque.
		/// </summary>
		/// <param name="color">A <see cref="Vector3"/> representing color.</param>
		[AutoConvertType]
		public ColorByte( Vector3F color )
			: this( (int)( color.X * 255 ), (int)( color.Y * 255 ), (int)( color.Z * 255 ) )
		{
		}

		///// <summary>
		///// Constructs an RGBA color from a <see cref="ColorPacked"/> and an alpha value.
		///// </summary>
		///// <param name="color">A <see cref="ColorPacked"/> for RGB values of new <see cref="ColorPacked"/> instance.</param>
		///// <param name="alpha">The alpha component value from 0 to 255.</param>
		//public ColorPacked( ColorPacked color, int alpha )
		//{
		//	if( ( alpha & 0xFFFFFF00 ) != 0 )
		//	{
		//		var clampedA = (uint)MathEx.Clamp( alpha, Byte.MinValue, Byte.MaxValue );

		//		_packedValue = ( color._packedValue & 0x00FFFFFF ) | ( clampedA << 24 );
		//	}
		//	else
		//	{
		//		_packedValue = ( color._packedValue & 0x00FFFFFF ) | ( (uint)alpha << 24 );
		//	}
		//}

		///// <summary>
		///// Constructs an RGBA color from color and alpha value.
		///// </summary>
		///// <param name="color">A <see cref="ColorPacked"/> for RGB values of new <see cref="ColorPacked"/> instance.</param>
		///// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
		//public ColorPacked( ColorPacked color, float alpha ) :
		//	this( color, (int)( alpha * 255 ) )
		//{
		//}

		///// <summary>
		///// Constructs an RGBA color from scalars representing red, green and blue values. Alpha value will be opaque.
		///// </summary>
		///// <param name="r">Red component value from 0.0f to 1.0f.</param>
		///// <param name="g">Green component value from 0.0f to 1.0f.</param>
		///// <param name="b">Blue component value from 0.0f to 1.0f.</param>
		//public ColorPacked( float r, float g, float b )
		//	: this( (int)( r * 255 ), (int)( g * 255 ), (int)( b * 255 ) )
		//{
		//}

		/// <summary>
		/// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
		/// </summary>
		/// <param name="r">Red component value from 0.0f to 1.0f.</param>
		/// <param name="g">Green component value from 0.0f to 1.0f.</param>
		/// <param name="b">Blue component value from 0.0f to 1.0f.</param>
		/// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
		public ColorByte( float r, float g, float b, float alpha = 1 )
			: this( (int)( r * 255 ), (int)( g * 255 ), (int)( b * 255 ), (int)( alpha * 255 ) )
		{
		}

		///// <summary>
		///// Constructs an RGBA color from scalars representing red, green and blue values. Alpha value will be opaque.
		///// </summary>
		///// <param name="r">Red component value from 0 to 255.</param>
		///// <param name="g">Green component value from 0 to 255.</param>
		///// <param name="b">Blue component value from 0 to 255.</param>
		//public ColorPacked( int r, int g, int b )
		//{
		//	_packedValue = 0xFF000000; // A = 255

		//	if( ( ( r | g | b ) & 0xFFFFFF00 ) != 0 )
		//	{
		//		var clampedR = (uint)MathHelper.Clamp( r, Byte.MinValue, Byte.MaxValue );
		//		var clampedG = (uint)MathHelper.Clamp( g, Byte.MinValue, Byte.MaxValue );
		//		var clampedB = (uint)MathHelper.Clamp( b, Byte.MinValue, Byte.MaxValue );

		//		_packedValue |= ( clampedB << 16 ) | ( clampedG << 8 ) | ( clampedR );
		//	}
		//	else
		//	{
		//		_packedValue |= ( (uint)b << 16 ) | ( (uint)g << 8 ) | ( (uint)r );
		//	}
		//}

		/// <summary>
		/// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
		/// </summary>
		/// <param name="red">Red component value from 0 to 255.</param>
		/// <param name="green">Green component value from 0 to 255.</param>
		/// <param name="blue">Blue component value from 0 to 255.</param>
		/// <param name="alpha">Alpha component value from 0 to 255.</param>
		public ColorByte( int red, int green, int blue, int alpha = 255 )
		{
			if( ( ( red | green | blue | alpha ) & 0xFFFFFF00 ) != 0 )
			{
				var clampedR = (uint)MathEx.Clamp( red, Byte.MinValue, Byte.MaxValue );
				var clampedG = (uint)MathEx.Clamp( green, Byte.MinValue, Byte.MaxValue );
				var clampedB = (uint)MathEx.Clamp( blue, Byte.MinValue, Byte.MaxValue );
				var clampedA = (uint)MathEx.Clamp( alpha, Byte.MinValue, Byte.MaxValue );

				_packedValue = ( clampedA << 24 ) | ( clampedB << 16 ) | ( clampedG << 8 ) | ( clampedR );
			}
			else
			{
				_packedValue = ( (uint)alpha << 24 ) | ( (uint)blue << 16 ) | ( (uint)green << 8 ) | ( (uint)red );
			}
		}

		/// <summary>
		/// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
		/// </summary>
		/// <remarks>
		/// This overload sets the values directly without clamping, and may therefore be faster than the other overloads.
		/// </remarks>
		/// <param name="red"></param>
		/// <param name="green"></param>
		/// <param name="blue"></param>
		/// <param name="alpha"></param>
		public ColorByte( byte red, byte green, byte blue, byte alpha = 255 )
		{
			_packedValue = ( (uint)alpha << 24 ) | ( (uint)blue << 16 ) | ( (uint)green << 8 ) | ( red );
		}

		[AutoConvertType]
		public ColorByte( ColorValue source )
			: this( (int)( source.Red * 255 ), (int)( source.Green * 255 ), (int)( source.Blue * 255 ), (int)( source.Alpha * 255 ) )
		{
		}

		public ColorByte( ref ColorValue source )
			: this( (int)( source.Red * 255 ), (int)( source.Green * 255 ), (int)( source.Blue * 255 ), (int)( source.Alpha * 255 ) )
		{
		}

		[AutoConvertType]
		public ColorByte( Color source )
			: this( source.R, source.G, source.B, source.A )
		{
		}

		/// <summary>
		/// Gets or sets the blue component.
		/// </summary>
		public byte Blue
		{
			get
			{
				unchecked
				{
					return (byte)( this._packedValue >> 16 );
				}
			}
			set
			{
				this._packedValue = ( this._packedValue & 0xff00ffff ) | ( (uint)value << 16 );
			}
		}

		/// <summary>
		/// Gets or sets the green component.
		/// </summary>
		public byte Green
		{
			get
			{
				unchecked
				{
					return (byte)( this._packedValue >> 8 );
				}
			}
			set
			{
				this._packedValue = ( this._packedValue & 0xffff00ff ) | ( (uint)value << 8 );
			}
		}

		/// <summary>
		/// Gets or sets the red component.
		/// </summary>
		public byte Red
		{
			get
			{
				unchecked
				{
					return (byte)this._packedValue;
				}
			}
			set
			{
				this._packedValue = ( this._packedValue & 0xffffff00 ) | value;
			}
		}

		/// <summary>
		/// Gets or sets the alpha component.
		/// </summary>
		public byte Alpha
		{
			get
			{
				unchecked
				{
					return (byte)( this._packedValue >> 24 );
				}
			}
			set
			{
				this._packedValue = ( this._packedValue & 0x00ffffff ) | ( (uint)value << 24 );
			}
		}

		/// <summary>
		/// Compares whether two <see cref="ColorByte"/> instances are equal.
		/// </summary>
		/// <param name="a"><see cref="ColorByte"/> instance on the left of the equal sign.</param>
		/// <param name="b"><see cref="ColorByte"/> instance on the right of the equal sign.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public static bool operator ==( ColorByte a, ColorByte b )
		{
			return ( a._packedValue == b._packedValue );
		}

		/// <summary>
		/// Compares whether two <see cref="ColorByte"/> instances are not equal.
		/// </summary>
		/// <param name="a"><see cref="ColorByte"/> instance on the left of the not equal sign.</param>
		/// <param name="b"><see cref="ColorByte"/> instance on the right of the not equal sign.</param>
		/// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>	
		public static bool operator !=( ColorByte a, ColorByte b )
		{
			return ( a._packedValue != b._packedValue );
		}

		/// <summary>
		/// Gets the hash code of this <see cref="ColorByte"/>.
		/// </summary>
		/// <returns>Hash code of this <see cref="ColorByte"/>.</returns>
		public override int GetHashCode()
		{
			return this._packedValue.GetHashCode();
		}

		/// <summary>
		/// Compares whether current instance is equal to specified object.
		/// </summary>
		/// <param name="obj">The <see cref="ColorByte"/> to compare.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public override bool Equals( object obj )
		{
			return ( ( obj is ColorByte ) && this.Equals( (ColorByte)obj ) );
		}

		/// <summary>
		/// Performs linear interpolation of <see cref="ColorByte"/>.
		/// </summary>
		/// <param name="value1">Source <see cref="ColorByte"/>.</param>
		/// <param name="value2">Destination <see cref="ColorByte"/>.</param>
		/// <param name="amount">Interpolation factor.</param>
		/// <returns>Interpolated <see cref="ColorByte"/>.</returns>
		public static ColorByte Lerp( ColorByte value1, ColorByte value2, double amount )
		{
			amount = MathEx.Clamp( amount, 0, 1 );
			return new ColorByte(
				(int)MathEx.Lerp( value1.Red, value2.Red, amount ),
				(int)MathEx.Lerp( value1.Green, value2.Green, amount ),
				(int)MathEx.Lerp( value1.Blue, value2.Blue, amount ),
				(int)MathEx.Lerp( value1.Alpha, value2.Alpha, amount ) );
		}

		///// <summary>
		///// <see cref="ColorPacked.Lerp"/> should be used instead of this function.
		///// </summary>
		///// <returns>Interpolated <see cref="ColorPacked"/>.</returns>
		//[Obsolete( "Color.Lerp should be used instead of this function." )]
		//public static ColorPacked LerpPrecise( ColorPacked value1, ColorPacked value2, Single amount )
		//{
		//	amount = MathHelper.Clamp( amount, 0, 1 );
		//	return new ColorPacked(
		//		(int)MathHelper.LerpPrecise( value1.Red, value2.Red, amount ),
		//		(int)MathHelper.LerpPrecise( value1.Green, value2.Green, amount ),
		//		(int)MathHelper.LerpPrecise( value1.Blue, value2.Blue, amount ),
		//		(int)MathHelper.LerpPrecise( value1.Alpha, value2.Alpha, amount ) );
		//}

		///// <summary>
		///// Multiply <see cref="ColorPacked"/> by value.
		///// </summary>
		///// <param name="value">Source <see cref="ColorPacked"/>.</param>
		///// <param name="scale">Multiplicator.</param>
		///// <returns>Multiplication result.</returns>
		//public static ColorPacked Multiply( ColorPacked value, float scale )
		//{
		//	return new ColorPacked( (int)( value.Red * scale ), (int)( value.Green * scale ), (int)( value.Blue * scale ), (int)( value.Alpha * scale ) );
		//}

		///// <summary>
		///// Multiply <see cref="ColorPacked"/> by value.
		///// </summary>
		///// <param name="value">Source <see cref="ColorPacked"/>.</param>
		///// <param name="scale">Multiplicator.</param>
		///// <returns>Multiplication result.</returns>
		//public static ColorPacked operator *( ColorPacked value, float scale )
		//{
		//	return new ColorPacked( (int)( value.Red * scale ), (int)( value.Green * scale ), (int)( value.Blue * scale ), (int)( value.Alpha * scale ) );
		//}

		/// <summary>
		/// Gets or sets packed value of this <see cref="ColorPacked"/>.
		/// </summary>
		[CLSCompliant( false )]
		[Browsable( false )]
		public uint PackedValue
		{
			get { return _packedValue; }
			set { _packedValue = value; }
		}

		///// <summary>
		///// Translate a non-premultipled alpha <see cref="ColorPacked"/> to a <see cref="ColorPacked"/> that contains premultiplied alpha.
		///// </summary>
		///// <param name="vector">A <see cref="Vector4"/> representing color.</param>
		///// <returns>A <see cref="ColorPacked"/> which contains premultiplied alpha data.</returns>
		//public static ColorPacked FromNonPremultiplied( Vector4 vector )
		//{
		//	return new ColorPacked( vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W, vector.W );
		//}

		///// <summary>
		///// Translate a non-premultipled alpha <see cref="ColorPacked"/> to a <see cref="ColorPacked"/> that contains premultiplied alpha.
		///// </summary>
		///// <param name="r">Red component value.</param>
		///// <param name="g">Green component value.</param>
		///// <param name="b">Blue component value.</param>
		///// <param name="a">Alpha component value.</param>
		///// <returns>A <see cref="ColorPacked"/> which contains premultiplied alpha data.</returns>
		//public static ColorPacked FromNonPremultiplied( int r, int g, int b, int a )
		//{
		//	return new ColorPacked( r * a / 255, g * a / 255, b * a / 255, a );
		//}

		/// <summary>
		/// Compares whether current instance is equal to specified <see cref="ColorPacked"/>.
		/// </summary>
		/// <param name="other">The <see cref="ColorPacked"/> to compare.</param>
		/// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
		public bool Equals( ColorByte other )
		{
			return this.PackedValue == other.PackedValue;
		}

		public static ColorByte Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 3 && vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 3 or 4 parts separated by spaces in the form (r g b [a]).", text ) );

			try
			{
				return new ColorByte(
					int.Parse( vals[ 0 ] ),
					int.Parse( vals[ 1 ] ),
					int.Parse( vals[ 2 ] ),
					( vals.Length == 4 ) ? int.Parse( vals[ 3 ].Trim() ) : 255 );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the color must be decimal numbers." );
			}
		}

		public override string ToString()
		{
			if( Alpha != 255 )
				return string.Format( "{0} {1} {2} {3}", Red, Green, Blue, Alpha );
			else
				return string.Format( "{0} {1} {2}", Red, Green, Blue );
		}

		public unsafe int this[ int index ]
		{
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				switch( index )
				{
				case 0: return Red;
				case 1: return Green;
				case 2: return Blue;
				case 3: return Alpha;
				}
				return 0;
			}
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				switch( index )
				{
				case 0: Red = (byte)value; break;
				case 1: Green = (byte)value; break;
				case 2: Blue = (byte)value; break;
				case 3: Alpha = (byte)value; break;
				}
			}
		}

		//public void Clamp( ColorPacked min, ColorPacked max )
		//{
		//	if( Red < min.Red )
		//		Red = min.Red;
		//	else if( Red > max.Red )
		//		Red = max.Red;

		//	if( g < min.g )
		//		g = min.g;
		//	else if( g > max.g )
		//		g = max.g;

		//	if( b < min.b )
		//		b = min.b;
		//	else if( b > max.b )
		//		b = max.b;

		//	if( a < min.a )
		//		a = min.a;
		//	else if( a > max.a )
		//		a = max.a;
		//}

		//[ShaderGenerationFunction( "clamp({this}, {min}, {max})" )]
		//public ColorValue GetClamp( ColorValue min, ColorValue max )
		//{
		//	ColorValue r = this;
		//	r.Clamp( min, max );
		//	return r;
		//}

		[AutoConvertType]
		public Vector4 ToVector4()
		{
			return new Vector4( (double)Red / 255.0f, (double)Green / 255.0f, (double)Blue / 255.0f, (double)Alpha / 255.0f );
		}

		[AutoConvertType]
		public Vector3 ToVector3()
		{
			return new Vector3( (float)Red / 255.0f, (float)Green / 255.0f, (float)Blue / 255.0f );
		}

		[AutoConvertType]
		public Vector4F ToVector4F()
		{
			return new Vector4F( (float)Red / 255.0f, (float)Green / 255.0f, (float)Blue / 255.0f, (float)Alpha / 255.0f );
		}

		[AutoConvertType]
		public Vector3F ToVector3F()
		{
			return new Vector3F( (float)Red / 255.0f, (float)Green / 255.0f, (float)Blue / 255.0f );
		}

		[AutoConvertType]
		public ColorValue ToColorValue()
		{
			return new ColorValue( (float)Red / 255.0f, (float)Green / 255.0f, (float)Blue / 255.0f, (float)Alpha / 255.0f );
		}

		[AutoConvertType]
		public ColorValuePowered ToColorValuePowered()
		{
			return new ColorValuePowered( (float)Red / 255.0f, (float)Green / 255.0f, (float)Blue / 255.0f, (float)Alpha / 255.0f );
		}

		[AutoConvertType]
		public Color ToColor()
		{
			return Color.FromArgb( Alpha, Red, Green, Blue );
		}

		//[ShaderGenerationFunction( "{pick1} ? {v1} : {v2}" )]
		public static ColorByte Select( ColorByte v1, ColorByte v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		public static ColorByte FromABGR( uint value )
		{
			//!!!!check

			return new ColorByte( value );
			//var a = (byte)( ( 0xFF000000 & value ) >> 24 );
			//var b = (byte)( ( 0x00FF0000 & value ) >> 16 );
			//var g = (byte)( ( 0x0000FF00 & value ) >> 8 );
			//var r = (byte)( 0x000000FF & value );
			//return new ColorPacked( r, g, b, a );
		}

		public static ColorByte FromARGB( uint value )
		{
			//!!!!check

			var a = (byte)( ( 0xFF000000 & value ) >> 24 );
			var r = (byte)( ( 0x00FF0000 & value ) >> 16 );
			var g = (byte)( ( 0x0000FF00 & value ) >> 8 );
			var b = (byte)( 0x000000FF & value );
			return new ColorByte( r, g, b, a );
		}
	}
}
