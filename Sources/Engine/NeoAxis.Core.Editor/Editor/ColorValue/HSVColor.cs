using System;
using System.Drawing;

namespace NeoAxis.Editor
{
	struct HSVColor
	{
		double hue; // 0-360
		double saturation; // 0-1
		double value; // 0-1

		public double Hue
		{
			get { return hue; }
			set { hue = value; }
		}

		public double Saturation
		{
			get { return saturation; }
			set { saturation = value; }
		}

		public double Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		public static bool operator ==( HSVColor lhs, HSVColor rhs )
		{
			if( ( lhs.Hue == rhs.Hue ) &&
				 ( lhs.Saturation == rhs.Saturation ) &&
				 ( lhs.Value == rhs.Value ) )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool operator !=( HSVColor lhs, HSVColor rhs )
		{
			return !( lhs == rhs );
		}

		public override bool Equals( object obj )
		{
			return ( obj is HSVColor ) && this == (HSVColor)obj;
		}

		public override int GetHashCode()
		{
			return Hue.GetHashCode() ^ Saturation.GetHashCode() ^ Value.GetHashCode();
		}

		public HSVColor( double hue, double saturation, double value )
		{
			MathEx.Clamp( ref hue, 0, 360 );
			MathEx.Clamp( ref saturation, 0, 1 );
			MathEx.Clamp( ref value, 0, 1 );
			//if( hue < 0 || hue > 360 )
			//	throw new ArgumentOutOfRangeException( "hue", "must be in the range [0, 360]" );
			//if( saturation < 0 || saturation > 100 )
			//	throw new ArgumentOutOfRangeException( "saturation", "must be in the range [0, 100]" );
			//if( value < 0 || value > 100 )
			//	throw new ArgumentOutOfRangeException( "value", "must be in the range [0, 100]" );

			//if( hue < 0 || hue > 360 )
			//	throw new ArgumentOutOfRangeException( "hue", "must be in the range [0, 360]" );
			//if( saturation < 0 || saturation > 100 )
			//	throw new ArgumentOutOfRangeException( "saturation", "must be in the range [0, 100]" );
			//if( value < 0 || value > 100 )
			//	throw new ArgumentOutOfRangeException( "value", "must be in the range [0, 100]" );

			this.hue = hue;
			this.saturation = saturation;
			this.value = value;
		}

		//public static HsvColor FromColor( Color color )
		//{
		//	RgbColor rgb = new RgbColor( color.R, color.G, color.B );
		//	return rgb.ToHsv();
		//}

		//public Color ToColor()
		//{
		//	RgbColor rgb = ToRGB();
		//	return Color.FromArgb( rgb.Red, rgb.Green, rgb.Blue );
		//}

		public static HSVColor FromRGB( ColorValue color )
		{
			// In this function, R, G, and B values must be scaled 
			// to be between 0 and 1.
			// HsvColor.Hue will be a value between 0 and 360, and 
			// HsvColor.Saturation and value are between 0 and 1.

			double min;
			double max;
			double delta;

			double r = color.Red;
			double g = color.Green;
			double b = color.Blue;

			double h;
			double s;
			double v;

			min = Math.Min( Math.Min( r, g ), b );
			max = Math.Max( Math.Max( r, g ), b );
			v = max;
			delta = max - min;

			if( max == 0 || delta == 0 )
			{
				// R, G, and B must be 0, or all the same.
				// In this case, S is 0, and H is undefined.
				// Using H = 0 is as good as any...
				s = 0;
				h = 0;
			}
			else
			{
				s = delta / max;
				if( r == max )
				{
					// Between Yellow and Magenta
					h = ( g - b ) / delta;
				}
				else if( g == max )
				{
					// Between Cyan and Yellow
					h = 2 + ( b - r ) / delta;
				}
				else
				{
					// Between Magenta and Cyan
					h = 4 + ( r - g ) / delta;
				}

			}
			// Scale h to be between 0 and 360. 
			// This may require adding 360, if the value
			// is negative.
			h *= 60;

			if( h < 0 )
			{
				h += 360;
			}

			return new HSVColor( h, s, v );
		}

		public ColorValue ToColorValue()
		{
			// HsvColor contains values scaled as in the color wheel:

			double h;
			double s;
			double v;

			double r = 0;
			double g = 0;
			double b = 0;

			// Scale Hue to be between 0 and 360. Saturation
			// and value scale to be between 0 and 1.
			h = Hue % 360;
			s = Saturation;// / 100;
			v = Value;// / 100;

			if( s == 0 )
			{
				// If s is 0, all colors are the same.
				// This is some flavor of gray.
				r = v;
				g = v;
				b = v;
			}
			else
			{
				double p;
				double q;
				double t;

				double fractionalSector;
				int sectorNumber;
				double sectorPos;

				// The color wheel consists of 6 sectors.
				// Figure out which sector you're in.
				sectorPos = h / 60;
				sectorNumber = (int)( Math.Floor( sectorPos ) );

				// get the fractional part of the sector.
				// That is, how many degrees into the sector
				// are you?
				fractionalSector = sectorPos - sectorNumber;

				// Calculate values for the three axes
				// of the color. 
				p = v * ( 1 - s );
				q = v * ( 1 - ( s * fractionalSector ) );
				t = v * ( 1 - ( s * ( 1 - fractionalSector ) ) );

				// Assign the fractional colors to r, g, and b
				// based on the sector the angle is in.
				switch( sectorNumber )
				{
				case 0: r = v; g = t; b = p; break;
				case 1: r = q; g = v; b = p; break;
				case 2: r = p; g = v; b = t; break;
				case 3: r = p; g = q; b = v; break;
				case 4: r = t; g = p; b = v; break;
				case 5: r = v; g = p; b = q; break;
				}
			}
			return new ColorValue( r, g, b );
		}

		public Color ToColor()
		{
			var v = ToColorValue();
			return Color.FromArgb( (int)( v.Red * 255 ), (int)( v.Green * 255 ), (int)( v.Blue * 255 ) );
		}

		public override string ToString()
		{
			return String.Format( "({0}, {1}, {2})", Hue, Saturation, Value );
		}
	}
}
