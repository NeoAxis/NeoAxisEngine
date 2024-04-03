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
	/// Represents a color in the form of RGBA with power component.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct ColorValuePowered
	{
		[Browsable( false )]
		public ColorValue Color;
		public float Power;

		public static readonly ColorValuePowered Zero = new ColorValuePowered( 0, 0, 0, 0, 0 );
		public static readonly ColorValuePowered One = new ColorValuePowered( 1, 1, 1, 1, 1 );

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValuePowered( ColorValuePowered source )
		{
			this.Color = source.Color;
			this.Power = source.Power;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValuePowered( ColorValue color, float power )
		{
			this.Color = color;
			this.Power = power;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public ColorValuePowered( ColorValue source )
		{
			float p = 1;
			if( source.Red > 1 && source.Red > p )
				p = source.Red;
			if( source.Green > 1 && source.Green > p )
				p = source.Green;
			if( source.Blue > 1 && source.Blue > p )
				p = source.Blue;

			Color = new ColorValue( source.Red / p, source.Green / p, source.Blue / p, source.Alpha );
			Power = p;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public ColorValuePowered( Vector4 source )
		{
			float p = 1;
			if( source.X > 1 && source.X > p )
				p = (float)source.X;
			if( source.Y > 1 && source.Y > p )
				p = (float)source.Y;
			if( source.Z > 1 && source.Z > p )
				p = (float)source.Z;
			Color = new ColorValue( source.X / p, source.Y / p, source.Z / p, source.W );
			Power = p;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public ColorValuePowered( Vector4F source )
		{
			float p = 1;
			if( source.X > 1 && source.X > p )
				p = source.X;
			if( source.Y > 1 && source.Y > p )
				p = source.Y;
			if( source.Z > 1 && source.Z > p )
				p = source.Z;
			Color = new ColorValue( source.X / p, source.Y / p, source.Z / p, source.W );
			Power = p;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public ColorValuePowered( Vector3 source )
		{
			float p = 1;
			if( source.X > 1 && source.X > p )
				p = (float)source.X;
			if( source.Y > 1 && source.Y > p )
				p = (float)source.Y;
			if( source.Z > 1 && source.Z > p )
				p = (float)source.Z;
			Color = new ColorValue( source.X / p, source.Y / p, source.Z / p, 1 );
			Power = p;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public ColorValuePowered( Vector3F source )
		{
			float p = 1;
			if( source.X > 1 && source.X > p )
				p = source.X;
			if( source.Y > 1 && source.Y > p )
				p = source.Y;
			if( source.Z > 1 && source.Z > p )
				p = source.Z;
			Color = new ColorValue( source.X / p, source.Y / p, source.Z / p, 1 );
			Power = p;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValuePowered( double r, double g, double b, double a = 1, double p = 1 )
		{
			Color = new ColorValue( r, g, b, a );
			Power = (float)p;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValuePowered( float r, float g, float b, float a = 1, float p = 1 )
		{
			Color = new ColorValue( r, g, b, a );
			Power = p;
		}

		public float Red
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return Color.Red; }
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Color.Red = value; }
		}

		public float Green
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return Color.Green; }
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Color.Green = value; }
		}

		public float Blue
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return Color.Blue; }
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Color.Blue = value; }
		}

		public float Alpha
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get { return Color.Alpha; }
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Color.Alpha = value; }
		}

		//[Browsable( false )]
		//public ColorValue Color
		//{
		//	get { return color; }
		//	//set { color = value; }
		//}

		//public float Power
		//{
		//	get { return power; }
		//	set { power = value; }
		//}

		public static ColorValuePowered Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			try
			{
				var index = text.IndexOf( ';' );
				if( index != -1 )
				{
					var part1 = text.Substring( 0, index );
					var part2 = text.Substring( index + 1 );
					var color = ColorValue.Parse( part1 );
					var power = float.Parse( part2 );
					return new ColorValuePowered( color, power );
				}
				else
					return new ColorValuePowered( ColorValue.Parse( text ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the color powered must be decimal numbers in the form (r g b [a]; power) or (r g b [a])." );
			}
		}

		public static bool TryParse( string text, out ColorValuePowered result )
		{
			try
			{
				result = Parse( text );
				return true;
			}
			catch
			{
				result = default( ColorValuePowered );
				return false;
			}
		}

		public override string ToString()
		{
			return ToString( 10 );
			//if( power != 1.0f )
			//	return string.Format( "{0}; {1}", color, power );
			//else
			//	return color.ToString();
		}

		public string ToString( int precision )
		{
			if( Power != 1.0f )
			{
				return string.Format( "{0}; {1}", Color.ToString( precision ), Power.ToString( "0.########" ) );

				//string format = "";
				//format = format.PadLeft( precision, '#' );
				//color.ToString( precision );

				//format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "} {3:0." + format + "} {4:0." + format + "}";
				//return string.Format( format, color.Red, color.Green, color.Blue, color.Alpha, power );
			}
			else
				return Color.ToString( precision );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is ColorValuePowered && this == (ColorValuePowered)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return Color.GetHashCode() ^ Power.GetHashCode();
		}

		//public static ColorValue operator +( ColorValue v1, ColorValue v2 )
		//{
		//	ColorValue result;
		//	result.r = v1.r + v2.r;
		//	result.g = v1.g + v2.g;
		//	result.b = v1.b + v2.b;
		//	result.a = v1.a + v2.a;
		//	return result;
		//}

		//public static ColorValue operator -( ColorValue v1, ColorValue v2 )
		//{
		//	ColorValue result;
		//	result.r = v1.r - v2.r;
		//	result.g = v1.g - v2.g;
		//	result.b = v1.b - v2.b;
		//	result.a = v1.a - v2.a;
		//	return result;
		//}

		//public static ColorValue operator *( ColorValue v1, ColorValue v2 )
		//{
		//	ColorValue result;
		//	result.r = v1.r * v2.r;
		//	result.g = v1.g * v2.g;
		//	result.b = v1.b * v2.b;
		//	result.a = v1.a * v2.a;
		//	return result;
		//}

		//public static ColorValue operator *( ColorValue v, float s )
		//{
		//	ColorValue result;
		//	result.r = v.r * s;
		//	result.g = v.g * s;
		//	result.b = v.b * s;
		//	result.a = v.a * s;
		//	return result;
		//}

		//public static ColorValue operator *( float s, ColorValue v )
		//{
		//	ColorValue result;
		//	result.r = v.r * s;
		//	result.g = v.g * s;
		//	result.b = v.b * s;
		//	result.a = v.a * s;
		//	return result;
		//}

		//public static ColorValue operator /( ColorValue v1, ColorValue v2 )
		//{
		//	ColorValue result;
		//	result.r = v1.r / v2.r;
		//	result.g = v1.g / v2.g;
		//	result.b = v1.b / v2.b;
		//	result.a = v1.a / v2.a;
		//	return result;
		//}

		//public static ColorValue operator /( ColorValue v, float s )
		//{
		//	ColorValue result;
		//	float invS = 1.0f / s;
		//	result.r = v.r * invS;
		//	result.g = v.g * invS;
		//	result.b = v.b * invS;
		//	result.a = v.a * invS;
		//	return result;
		//}

		//public static ColorValue operator /( float s, ColorValue v )
		//{
		//	ColorValue result;
		//	result.r = s / v.r;
		//	result.g = s / v.g;
		//	result.b = s / v.b;
		//	result.a = s / v.a;
		//	return result;
		//}

		//public static ColorValue operator -( ColorValue v )
		//{
		//	ColorValue result;
		//	result.r = -v.r;
		//	result.g = -v.g;
		//	result.b = -v.b;
		//	result.a = -v.a;
		//	return result;
		//}

		//public static void Add( ref ColorValue v1, ref ColorValue v2, out ColorValue result )
		//{
		//	result.r = v1.r + v2.r;
		//	result.g = v1.g + v2.g;
		//	result.b = v1.b + v2.b;
		//	result.a = v1.a + v2.a;
		//}

		//public static void Subtract( ref ColorValue v1, ref ColorValue v2, out ColorValue result )
		//{
		//	result.r = v1.r - v2.r;
		//	result.g = v1.g - v2.g;
		//	result.b = v1.b - v2.b;
		//	result.a = v1.a - v2.a;
		//}

		//public static void Multiply( ref ColorValue v1, ref ColorValue v2, out ColorValue result )
		//{
		//	result.r = v1.r * v2.r;
		//	result.g = v1.g * v2.g;
		//	result.b = v1.b * v2.b;
		//	result.a = v1.a * v2.a;
		//}

		//public static void Multiply( ref ColorValue v, float s, out ColorValue result )
		//{
		//	result.r = v.r * s;
		//	result.g = v.g * s;
		//	result.b = v.b * s;
		//	result.a = v.a * s;
		//}

		//public static void Multiply( float s, ref ColorValue v, out ColorValue result )
		//{
		//	result.r = v.r * s;
		//	result.g = v.g * s;
		//	result.b = v.b * s;
		//	result.a = v.a * s;
		//}

		//public static void Divide( ref ColorValue v1, ref ColorValue v2, out ColorValue result )
		//{
		//	result.r = v1.r / v2.r;
		//	result.g = v1.g / v2.g;
		//	result.b = v1.b / v2.b;
		//	result.a = v1.a / v2.a;
		//}

		//public static void Divide( ref ColorValue v, float s, out ColorValue result )
		//{
		//	float invS = 1.0f / s;
		//	result.r = v.r * invS;
		//	result.g = v.g * invS;
		//	result.b = v.b * invS;
		//	result.a = v.a * invS;
		//}

		//public static void Divide( float s, ref ColorValue v, out ColorValue result )
		//{
		//	result.r = s / v.r;
		//	result.g = s / v.g;
		//	result.b = s / v.b;
		//	result.a = s / v.a;
		//}

		//public static void Negate( ref ColorValue v, out ColorValue result )
		//{
		//	result.r = -v.r;
		//	result.g = -v.g;
		//	result.b = -v.b;
		//	result.a = -v.a;
		//}

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
		public static bool operator ==( ColorValuePowered v1, ColorValuePowered v2 )
		{
			return ( v1.Color == v2.Color && v1.Power == v2.Power );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( ColorValuePowered v1, ColorValuePowered v2 )
		{
			return ( v1.Color != v2.Color || v1.Power != v2.Power );
		}

		public unsafe float this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index < 0 || index > 4 )
					throw new ArgumentOutOfRangeException( "index" );
				switch( index )
				{
				case 0: return Color.Red;
				case 1: return Color.Green;
				case 2: return Color.Blue;
				case 3: return Color.Alpha;
				case 4: return Power;
				}
				return 0;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 4 )
					throw new ArgumentOutOfRangeException( "index" );
				switch( index )
				{
				case 0: Color.Red = value; break;
				case 1: Color.Green = value; break;
				case 2: Color.Blue = value; break;
				case 3: Color.Alpha = value; break;
				case 4: Power = value; break;
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( ColorValuePowered c, float epsilon )
		{
			if( !Color.Equals( c.Color, epsilon ) )
				return false;
			if( Math.Abs( Power - c.Power ) > epsilon )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Clamp( ColorValuePowered min, ColorValuePowered max )
		{
			Color.Clamp( min.Color, max.Color );

			if( Power < min.Power )
				Power = min.Power;
			else if( Power > max.Power )
				Power = max.Power;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValuePowered GetClamp( ColorValuePowered min, ColorValuePowered max )
		{
			ColorValuePowered r = this;
			r.Clamp( min, max );
			return r;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Saturate()
		{
			Color.Saturate();

			if( Power < 0 )
				Power = 0;
			else if( Power > 1 )
				Power = 1;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ColorValuePowered GetSaturate()
		{
			ColorValuePowered r = this;
			r.Saturate();
			return r;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector4 ToVector4()
		{
			return new Vector4( Color.Red * Power, Color.Green * Power, Color.Blue * Power, Color.Alpha );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector3 ToVector3()
		{
			return new Vector3( Color.Red * Power, Color.Green * Power, Color.Blue * Power );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector3H ToVector3H()
		{
			return new Vector3H( Color.Red * Power, Color.Green * Power, Color.Blue * Power );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector4F ToVector4F()
		{
			return new Vector4F( Color.Red * Power, Color.Green * Power, Color.Blue * Power, Color.Alpha );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public Vector3F ToVector3F()
		{
			return new Vector3F( Color.Red * Power, Color.Green * Power, Color.Blue * Power );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public ColorValue ToColorValue()
		{
			return new ColorValue( Color.Red * Power, Color.Green * Power, Color.Blue * Power, Color.Alpha );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static ColorValuePowered Lerp( ColorValuePowered v1, ColorValuePowered v2, float amount )
		{
			ColorValuePowered result;
			ColorValue.Lerp( ref v1.Color, ref v2.Color, amount, out result.Color );
			result.Power = v1.Power + ( ( v2.Power - v1.Power ) * amount );
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Lerp( ref ColorValuePowered v1, ref ColorValuePowered v2, float amount, out ColorValuePowered result )
		{
			ColorValue.Lerp( ref v1.Color, ref v2.Color, amount, out result.Color );
			result.Power = v1.Power + ( ( v2.Power - v1.Power ) * amount );
		}
	}
}
