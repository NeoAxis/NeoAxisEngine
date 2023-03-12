// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Helper for the toolset. Turns off the ability to change alpha channel for color in the properties.
	/// </summary>
	[AttributeUsage( AttributeTargets.Property )]
	public class ColorValueNoAlphaAttribute : Attribute
	{
		public ColorValueNoAlphaAttribute() { }
	}

	//[Editor( typeof( ColorValueEditor ), typeof( UITypeEditor ) )]
	//[TypeConverter( typeof( ColorValueAsByteConverter ) )]
	/// <summary>
	/// Represents a color in the form of RGBA.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct ColorValue
	{
		[ShaderGenerationFunction( "{this}.r" )]
		public float Red;
		[ShaderGenerationFunction( "{this}.g" )]
		public float Green;
		[ShaderGenerationFunction( "{this}.b" )]
		public float Blue;
		[ShaderGenerationFunction( "{this}.a" )]
		public float Alpha;

		public static readonly ColorValue Zero = new ColorValue( 0, 0, 0, 0 );
		public static readonly ColorValue One = new ColorValue( 1, 1, 1, 1 );

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValue( ColorValue source )
		{
			this.Red = source.Red;
			this.Green = source.Green;
			this.Blue = source.Blue;
			this.Alpha = source.Alpha;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public ColorValue( Vector4 source )
		{
			this.Red = (float)source.X;
			this.Green = (float)source.Y;
			this.Blue = (float)source.Z;
			this.Alpha = (float)source.W;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public ColorValue( Vector4F source )
		{
			this.Red = source.X;
			this.Green = source.Y;
			this.Blue = source.Z;
			this.Alpha = source.W;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public ColorValue( Vector3 source )
		{
			this.Red = (float)source.X;
			this.Green = (float)source.Y;
			this.Blue = (float)source.Z;
			this.Alpha = 1;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public ColorValue( Vector3F source )
		{
			this.Red = source.X;
			this.Green = source.Y;
			this.Blue = source.Z;
			this.Alpha = 1;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "float4({r}, {g}, {b}, {a})" )]
		public ColorValue( double r, double g, double b, double a = 1 )
		{
			this.Red = (float)r;
			this.Green = (float)g;
			this.Blue = (float)b;
			this.Alpha = (float)a;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValue( float r, float g, float b, float a = 1 )
		{
			this.Red = r;
			this.Green = g;
			this.Blue = b;
			this.Alpha = a;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValue( ColorByte source )
		{
			Red = (float)source.Red / 255;
			Green = (float)source.Green / 255;
			Blue = (float)source.Blue / 255;
			Alpha = (float)source.Alpha / 255;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValue( Color source )
		{
			Red = (float)source.R / 255;
			Green = (float)source.G / 255;
			Blue = (float)source.B / 255;
			Alpha = (float)source.A / 255;
		}

		//[ShaderGenerationFunction( "{this}.r" )]
		//public float Red
		//{
		//	get { return r; }
		//	set { r = value; }
		//}

		//[ShaderGenerationFunction( "{this}.g" )]
		//public float Green
		//{
		//	get { return g; }
		//	set { g = value; }
		//}

		//[ShaderGenerationFunction( "{this}.b" )]
		//public float Blue
		//{
		//	get { return b; }
		//	set { b = value; }
		//}

		//[ShaderGenerationFunction( "{this}.a" )]
		//public float Alpha
		//{
		//	get { return a; }
		//	set { a = value; }
		//}

		public static ColorValue Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 3 && vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 3 or 4 parts separated by spaces in the form (r g b [a]).", text ) );

			try
			{
				return new ColorValue(
					float.Parse( vals[ 0 ] ),
					float.Parse( vals[ 1 ] ),
					float.Parse( vals[ 2 ] ),
					( vals.Length == 4 ) ? float.Parse( vals[ 3 ].Trim() ) : 1 );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the color must be decimal numbers." );
			}
		}

		public override string ToString()
		{
			return ToString( 8 );
			//if( a != 1.0f )
			//	return string.Format( "{0} {1} {2} {3}", r, g, b, a );
			//else
			//	return string.Format( "{0} {1} {2}", r, g, b );
		}

		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );

			if( Alpha != 1.0f )
			{
				format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "} {3:0." + format + "}";
				return string.Format( format, Red, Green, Blue, Alpha );
			}
			else
			{
				format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "}";
				return string.Format( format, Red, Green, Blue );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is ColorValue && this == (ColorValue)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Red.GetHashCode() ^ Green.GetHashCode() ^ Blue.GetHashCode() ^ Alpha.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} + {v2}" )]
		public static ColorValue operator +( ColorValue v1, ColorValue v2 )
		{
			ColorValue result;
			result.Red = v1.Red + v2.Red;
			result.Green = v1.Green + v2.Green;
			result.Blue = v1.Blue + v2.Blue;
			result.Alpha = v1.Alpha + v2.Alpha;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} - {v2}" )]
		public static ColorValue operator -( ColorValue v1, ColorValue v2 )
		{
			ColorValue result;
			result.Red = v1.Red - v2.Red;
			result.Green = v1.Green - v2.Green;
			result.Blue = v1.Blue - v2.Blue;
			result.Alpha = v1.Alpha - v2.Alpha;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} * {v2}" )]
		public static ColorValue operator *( ColorValue v1, ColorValue v2 )
		{
			ColorValue result;
			result.Red = v1.Red * v2.Red;
			result.Green = v1.Green * v2.Green;
			result.Blue = v1.Blue * v2.Blue;
			result.Alpha = v1.Alpha * v2.Alpha;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v} * {s}" )]
		public static ColorValue operator *( ColorValue v, float s )
		{
			ColorValue result;
			result.Red = v.Red * s;
			result.Green = v.Green * s;
			result.Blue = v.Blue * s;
			result.Alpha = v.Alpha * s;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{s} * {v}" )]
		public static ColorValue operator *( float s, ColorValue v )
		{
			ColorValue result;
			result.Red = v.Red * s;
			result.Green = v.Green * s;
			result.Blue = v.Blue * s;
			result.Alpha = v.Alpha * s;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v1} / {v2}" )]
		public static ColorValue operator /( ColorValue v1, ColorValue v2 )
		{
			ColorValue result;
			result.Red = v1.Red / v2.Red;
			result.Green = v1.Green / v2.Green;
			result.Blue = v1.Blue / v2.Blue;
			result.Alpha = v1.Alpha / v2.Alpha;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{v} / {s}" )]
		public static ColorValue operator /( ColorValue v, float s )
		{
			ColorValue result;
			float invS = 1.0f / s;
			result.Red = v.Red * invS;
			result.Green = v.Green * invS;
			result.Blue = v.Blue * invS;
			result.Alpha = v.Alpha * invS;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{s} / {v}" )]
		public static ColorValue operator /( float s, ColorValue v )
		{
			ColorValue result;
			result.Red = s / v.Red;
			result.Green = s / v.Green;
			result.Blue = s / v.Blue;
			result.Alpha = s / v.Alpha;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "-{v}" )]
		public static ColorValue operator -( ColorValue v )
		{
			ColorValue result;
			result.Red = -v.Red;
			result.Green = -v.Green;
			result.Blue = -v.Blue;
			result.Alpha = -v.Alpha;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Add( ref ColorValue v1, ref ColorValue v2, out ColorValue result )
		{
			result.Red = v1.Red + v2.Red;
			result.Green = v1.Green + v2.Green;
			result.Blue = v1.Blue + v2.Blue;
			result.Alpha = v1.Alpha + v2.Alpha;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Subtract( ref ColorValue v1, ref ColorValue v2, out ColorValue result )
		{
			result.Red = v1.Red - v2.Red;
			result.Green = v1.Green - v2.Green;
			result.Blue = v1.Blue - v2.Blue;
			result.Alpha = v1.Alpha - v2.Alpha;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref ColorValue v1, ref ColorValue v2, out ColorValue result )
		{
			result.Red = v1.Red * v2.Red;
			result.Green = v1.Green * v2.Green;
			result.Blue = v1.Blue * v2.Blue;
			result.Alpha = v1.Alpha * v2.Alpha;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref ColorValue v, float s, out ColorValue result )
		{
			result.Red = v.Red * s;
			result.Green = v.Green * s;
			result.Blue = v.Blue * s;
			result.Alpha = v.Alpha * s;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( float s, ref ColorValue v, out ColorValue result )
		{
			result.Red = v.Red * s;
			result.Green = v.Green * s;
			result.Blue = v.Blue * s;
			result.Alpha = v.Alpha * s;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( ref ColorValue v1, ref ColorValue v2, out ColorValue result )
		{
			result.Red = v1.Red / v2.Red;
			result.Green = v1.Green / v2.Green;
			result.Blue = v1.Blue / v2.Blue;
			result.Alpha = v1.Alpha / v2.Alpha;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( ref ColorValue v, float s, out ColorValue result )
		{
			float invS = 1.0f / s;
			result.Red = v.Red * invS;
			result.Green = v.Green * invS;
			result.Blue = v.Blue * invS;
			result.Alpha = v.Alpha * invS;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( float s, ref ColorValue v, out ColorValue result )
		{
			result.Red = s / v.Red;
			result.Green = s / v.Green;
			result.Blue = s / v.Blue;
			result.Alpha = s / v.Alpha;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Negate( ref ColorValue v, out ColorValue result )
		{
			result.Red = -v.Red;
			result.Green = -v.Green;
			result.Blue = -v.Blue;
			result.Alpha = -v.Alpha;
		}

		//public static ColorValue Add( ColorValue v1, ColorValue v2 )
		//{
		//	ColorValue result;
		//	result.r = v1.r + v2.r;
		//	result.g = v1.g + v2.g;
		//	result.b = v1.b + v2.b;
		//	result.a = v1.a + v2.a;
		//	return result;
		//}

		//public static ColorValue Subtract( ColorValue v1, ColorValue v2 )
		//{
		//	ColorValue result;
		//	result.r = v1.r - v2.r;
		//	result.g = v1.g - v2.g;
		//	result.b = v1.b - v2.b;
		//	result.a = v1.a - v2.a;
		//	return result;
		//}

		//public static ColorValue Multiply( ColorValue v1, ColorValue v2 )
		//{
		//	ColorValue result;
		//	result.r = v1.r * v2.r;
		//	result.g = v1.g * v2.g;
		//	result.b = v1.b * v2.b;
		//	result.a = v1.a * v2.a;
		//	return result;
		//}

		//public static ColorValue Multiply( ColorValue v, float s )
		//{
		//	ColorValue result;
		//	result.r = v.r * s;
		//	result.g = v.g * s;
		//	result.b = v.b * s;
		//	result.a = v.a * s;
		//	return result;
		//}

		//public static ColorValue Multiply( float s, ColorValue v )
		//{
		//	ColorValue result;
		//	result.r = v.r * s;
		//	result.g = v.g * s;
		//	result.b = v.b * s;
		//	result.a = v.a * s;
		//	return result;
		//}

		//public static ColorValue Divide( ColorValue v1, ColorValue v2 )
		//{
		//	ColorValue result;
		//	result.r = v1.r / v2.r;
		//	result.g = v1.g / v2.g;
		//	result.b = v1.b / v2.b;
		//	result.a = v1.a / v2.a;
		//	return result;
		//}

		//public static ColorValue Divide( ColorValue v, float s )
		//{
		//	ColorValue result;
		//	float invS = 1.0f / s;
		//	result.r = v.r * invS;
		//	result.g = v.g * invS;
		//	result.b = v.b * invS;
		//	result.a = v.a * invS;
		//	return result;
		//}

		//public static ColorValue Divide( float s, ColorValue v )
		//{
		//	ColorValue result;
		//	result.r = s / v.r;
		//	result.g = s / v.g;
		//	result.b = s / v.b;
		//	result.a = s / v.a;
		//	return result;
		//}

		//public static ColorValue Negate( ColorValue v )
		//{
		//	ColorValue result;
		//	result.r = -v.r;
		//	result.g = -v.g;
		//	result.b = -v.b;
		//	result.a = -v.a;
		//	return result;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( ColorValue v1, ColorValue v2 )
		{
			return ( v1.Red == v2.Red && v1.Green == v2.Green && v1.Blue == v2.Blue && v1.Alpha == v2.Alpha );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( ColorValue v1, ColorValue v2 )
		{
			return ( v1.Red != v2.Red || v1.Green != v2.Green || v1.Blue != v2.Blue || v1.Alpha != v2.Alpha );
		}

		public unsafe float this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( float* v = &this.Red )
				{
					return v[ index ];
				}
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( float* v = &this.Red )
				{
					v[ index ] = value;
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ColorValue c, float epsilon )
		{
			if( Math.Abs( Red - c.Red ) > epsilon )
				return false;
			if( Math.Abs( Green - c.Green ) > epsilon )
				return false;
			if( Math.Abs( Blue - c.Blue ) > epsilon )
				return false;
			if( Math.Abs( Alpha - c.Alpha ) > epsilon )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref ColorValue c, float epsilon )
		{
			if( Math.Abs( Red - c.Red ) > epsilon )
				return false;
			if( Math.Abs( Green - c.Green ) > epsilon )
				return false;
			if( Math.Abs( Blue - c.Blue ) > epsilon )
				return false;
			if( Math.Abs( Alpha - c.Alpha ) > epsilon )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Clamp( ColorValue min, ColorValue max )
		{
			if( Red < min.Red )
				Red = min.Red;
			else if( Red > max.Red )
				Red = max.Red;

			if( Green < min.Green )
				Green = min.Green;
			else if( Green > max.Green )
				Green = max.Green;

			if( Blue < min.Blue )
				Blue = min.Blue;
			else if( Blue > max.Blue )
				Blue = max.Blue;

			if( Alpha < min.Alpha )
				Alpha = min.Alpha;
			else if( Alpha > max.Alpha )
				Alpha = max.Alpha;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "clamp({this}, {min}, {max})" )]
		public ColorValue GetClamp( ColorValue min, ColorValue max )
		{
			ColorValue r = this;
			r.Clamp( min, max );
			return r;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Saturate()
		{
			if( Red < 0 ) Red = 0;
			else if( Red > 1 ) Red = 1;
			if( Green < 0 ) Green = 0;
			else if( Green > 1 ) Green = 1;
			if( Blue < 0 ) Blue = 0;
			else if( Blue > 1 ) Blue = 1;
			if( Alpha < 0 ) Alpha = 0;
			else if( Alpha > 1 ) Alpha = 1;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "saturate({this})" )]
		public ColorValue GetSaturate()
		{
			ColorValue r = this;
			r.Saturate();
			return r;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector4 ToVector4()
		{
			return new Vector4( Red, Green, Blue, Alpha );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector3 ToVector3()
		{
			return new Vector3( Red, Green, Blue );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector3H ToVector3H()
		{
			return new Vector3H( Red, Green, Blue );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector4F ToVector4F()
		{
			return new Vector4F( Red, Green, Blue, Alpha );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector3F ToVector3F()
		{
			return new Vector3F( Red, Green, Blue );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValuePowered ToColorValuePowered()
		{
			return new ColorValuePowered( this );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Color ToColor()
		{
			return Color.FromArgb(
				(int)MathEx.Clamp( Alpha * 255, 0, 255 ),
				(int)MathEx.Clamp( Red * 255, 0, 255 ),
				(int)MathEx.Clamp( Green * 255, 0, 255 ),
				(int)MathEx.Clamp( Blue * 255, 0, 255 ) );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorByte ToColorPacked()
		{
			return new ColorByte( ref this );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "lerp({v1}, {v2}, {amount})" )]
		public static ColorValue Lerp( ColorValue v1, ColorValue v2, float amount )
		{
			ColorValue result;
			result.Red = v1.Red + ( ( v2.Red - v1.Red ) * amount );
			result.Green = v1.Green + ( ( v2.Green - v1.Green ) * amount );
			result.Blue = v1.Blue + ( ( v2.Blue - v1.Blue ) * amount );
			result.Alpha = v1.Alpha + ( ( v2.Alpha - v1.Alpha ) * amount );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Lerp( ref ColorValue v1, ref ColorValue v2, float amount, out ColorValue result )
		{
			result.Red = v1.Red + ( ( v2.Red - v1.Red ) * amount );
			result.Green = v1.Green + ( ( v2.Green - v1.Green ) * amount );
			result.Blue = v1.Blue + ( ( v2.Blue - v1.Blue ) * amount );
			result.Alpha = v1.Alpha + ( ( v2.Alpha - v1.Alpha ) * amount );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[ShaderGenerationFunction( "{pick1} ? {v1} : {v2}" )]
		public static ColorValue Select( ColorValue v1, ColorValue v2, bool pick1 )
		{
			return pick1 ? v1 : v2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ref ColorValue v )
		{
			return Red == v.Red && Green == v.Green && Blue == v.Blue && Alpha == v.Alpha;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool Equals( ref ColorValue v1, ref ColorValue v2 )
		{
			return v1.Red == v2.Red && v1.Green == v2.Green && v1.Blue == v2.Blue && v1.Alpha == v2.Alpha;
		}

		/// <summary>
		/// Converts the current instance of <see cref="Vector2F"/> into the equivalent <see cref="Vector2H"/> structure.
		/// </summary>
		/// <returns>The equivalent <see cref="Vector2H"/> structure.</returns>
		[AutoConvertType]
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector4H ToVector4H()
		{
			Vector4H result;
			result.X = new HalfType( Red );
			result.Y = new HalfType( Green );
			result.Z = new HalfType( Blue );
			result.W = new HalfType( Alpha );
			return result;
		}
	}
}
