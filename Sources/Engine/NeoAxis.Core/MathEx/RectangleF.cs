// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<RectF> ) )]
	/// <summary>
	/// Represents a rectangle with single precision floating values.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct RectangleF
	{
		public float Left;
		public float Top;
		public float Right;
		public float Bottom;

		public static readonly RectangleF Zero = new RectangleF( 0.0f, 0.0f, 0.0f, 0.0f );

		public static readonly RectangleF Cleared = new RectangleF(
			new Vector2F( float.MaxValue, float.MaxValue ),
			new Vector2F( float.MinValue, float.MinValue ) );

		public RectangleF( RectangleF source )
		{
			Left = source.Left;
			Top = source.Top;
			Right = source.Right;
			Bottom = source.Bottom;
		}

		public RectangleF( float left, float top, float right, float bottom )
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}

		public RectangleF( Vector2F leftTop, Vector2F rightBottom )
		{
			this.Left = leftTop.X;
			this.Top = leftTop.Y;
			this.Right = rightBottom.X;
			this.Bottom = rightBottom.Y;
		}

		public RectangleF( Vector2F v )
		{
			Left = v.X;
			Right = v.X;
			Top = v.Y;
			Bottom = v.Y;
		}

		[AutoConvertType]
		public static RectangleF Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (left top right bottom).", text ) );

			try
			{
				return new RectangleF(
					float.Parse( vals[ 0 ] ),
					float.Parse( vals[ 1 ] ),
					float.Parse( vals[ 2 ] ),
					float.Parse( vals[ 3 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		[AutoConvertType]
		public override string ToString()
		{
			return ToString( 8 );
			//return string.Format( "{0} {1} {2} {3}", left, top, right, bottom );
		}

		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "} {3:0." + format + "}";
			return string.Format( format, Left, Top, Right, Bottom );
		}

		public override bool Equals( object obj )
		{
			return ( obj is RectangleF && this == (RectangleF)obj );
		}

		public override int GetHashCode()
		{
			return ( Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode() );
		}

		public static RectangleF operator +( RectangleF r, Vector2F v )
		{
			RectangleF result;
			result.Left = r.Left + v.X;
			result.Top = r.Top + v.Y;
			result.Right = r.Right + v.X;
			result.Bottom = r.Bottom + v.Y;
			return result;
		}

		public static RectangleF operator +( Vector2F v, RectangleF r )
		{
			RectangleF result;
			result.Left = v.X + r.Left;
			result.Top = v.Y + r.Top;
			result.Right = v.X + r.Right;
			result.Bottom = v.Y + r.Bottom;
			return result;
		}

		public static RectangleF operator -( RectangleF r, Vector2F v )
		{
			RectangleF result;
			result.Left = r.Left - v.X;
			result.Top = r.Top - v.Y;
			result.Right = r.Right - v.X;
			result.Bottom = r.Bottom - v.Y;
			return result;
		}

		public static RectangleF operator -( Vector2F v, RectangleF r )
		{
			RectangleF result;
			result.Left = v.X - r.Left;
			result.Top = v.Y - r.Top;
			result.Right = v.X - r.Right;
			result.Bottom = v.Y - r.Bottom;
			return result;
		}

		public static RectangleF operator *( RectangleF r, Vector2F v )
		{
			RectangleF result;
			result.Left = r.Left * v.X;
			result.Top = r.Top * v.Y;
			result.Right = r.Right * v.X;
			result.Bottom = r.Bottom * v.Y;
			return result;
		}

		public static RectangleF operator *( Vector2F v, RectangleF r )
		{
			RectangleF result;
			result.Left = v.X * r.Left;
			result.Top = v.Y * r.Top;
			result.Right = v.X * r.Right;
			result.Bottom = v.Y * r.Bottom;
			return result;
		}

		public static RectangleF operator /( RectangleF r, Vector2F v )
		{
			RectangleF result;
			result.Left = r.Left / v.X;
			result.Top = r.Top / v.Y;
			result.Right = r.Right / v.X;
			result.Bottom = r.Bottom / v.Y;
			return result;
		}

		//public static RectF Add( RectF r, Vec2F v )
		//{
		//	RectF result;
		//	result.left = r.left + v.X;
		//	result.top = r.top + v.Y;
		//	result.right = r.right + v.X;
		//	result.bottom = r.bottom + v.Y;
		//	return result;
		//}

		//public static RectF Add( Vec2F v, RectF r )
		//{
		//	RectF result;
		//	result.left = v.X + r.left;
		//	result.top = v.Y + r.top;
		//	result.right = v.X + r.right;
		//	result.bottom = v.Y + r.bottom;
		//	return result;
		//}

		//public static RectF Subtract( RectF r, Vec2F v )
		//{
		//	RectF result;
		//	result.left = r.left - v.X;
		//	result.top = r.top - v.Y;
		//	result.right = r.right - v.X;
		//	result.bottom = r.bottom - v.Y;
		//	return result;
		//}

		//public static RectF Subtract( Vec2F v, RectF r )
		//{
		//	RectF result;
		//	result.left = v.X - r.left;
		//	result.top = v.Y - r.top;
		//	result.right = v.X - r.right;
		//	result.bottom = v.Y - r.bottom;
		//	return result;
		//}

		//public static void Add( RectF r, Vec2F v, out RectF result )
		//{
		//	result.left = r.left + v.X;
		//	result.top = r.top + v.Y;
		//	result.right = r.right + v.X;
		//	result.bottom = r.bottom + v.Y;
		//}

		//public static void Add( Vec2F v, RectF r, out RectF result )
		//{
		//	result.left = v.X + r.left;
		//	result.top = v.Y + r.top;
		//	result.right = v.X + r.right;
		//	result.bottom = v.Y + r.bottom;
		//}

		public static void Add( ref RectangleF r, ref Vector2F v, out RectangleF result )
		{
			result.Left = r.Left + v.X;
			result.Top = r.Top + v.Y;
			result.Right = r.Right + v.X;
			result.Bottom = r.Bottom + v.Y;
		}

		public static void Add( ref Vector2F v, ref RectangleF r, out RectangleF result )
		{
			result.Left = v.X + r.Left;
			result.Top = v.Y + r.Top;
			result.Right = v.X + r.Right;
			result.Bottom = v.Y + r.Bottom;
		}

		//public static void Subtract( RectF r, Vec2F v, out RectF result )
		//{
		//	result.left = r.left - v.X;
		//	result.top = r.top - v.Y;
		//	result.right = r.right - v.X;
		//	result.bottom = r.bottom - v.Y;
		//}

		//public static void Subtract( Vec2F v, RectF r, out RectF result )
		//{
		//	result.left = v.X - r.left;
		//	result.top = v.Y - r.top;
		//	result.right = v.X - r.right;
		//	result.bottom = v.Y - r.bottom;
		//}

		public static void Subtract( ref RectangleF r, ref Vector2F v, out RectangleF result )
		{
			result.Left = r.Left - v.X;
			result.Top = r.Top - v.Y;
			result.Right = r.Right - v.X;
			result.Bottom = r.Bottom - v.Y;
		}

		public static void Subtract( ref Vector2F v, ref RectangleF r, out RectangleF result )
		{
			result.Left = v.X - r.Left;
			result.Top = v.Y - r.Top;
			result.Right = v.X - r.Right;
			result.Bottom = v.Y - r.Bottom;
		}

		//public static void Multiply( RectF r, Vec2F v, out RectF result )
		//{
		//	result.left = r.left * v.X;
		//	result.top = r.top * v.Y;
		//	result.right = r.right * v.X;
		//	result.bottom = r.bottom * v.Y;
		//}

		//public static void Multiply( Vec2F v, RectF r, out RectF result )
		//{
		//	result.left = v.X * r.left;
		//	result.top = v.Y * r.top;
		//	result.right = v.X * r.right;
		//	result.bottom = v.Y * r.bottom;
		//}

		public static void Multiply( ref RectangleF r, ref Vector2F v, out RectangleF result )
		{
			result.Left = r.Left * v.X;
			result.Top = r.Top * v.Y;
			result.Right = r.Right * v.X;
			result.Bottom = r.Bottom * v.Y;
		}

		public static void Multiply( ref Vector2F v, ref RectangleF r, out RectangleF result )
		{
			result.Left = v.X * r.Left;
			result.Top = v.Y * r.Top;
			result.Right = v.X * r.Right;
			result.Bottom = v.Y * r.Bottom;
		}

		//public static void Divide( RectF r, Vec2F v, out RectF result )
		//{
		//	result.left = r.left / v.X;
		//	result.top = r.top / v.Y;
		//	result.right = r.right / v.X;
		//	result.bottom = r.bottom / v.Y;
		//}

		public static void Divide( ref RectangleF r, ref Vector2F v, out RectangleF result )
		{
			result.Left = r.Left / v.X;
			result.Top = r.Top / v.Y;
			result.Right = r.Right / v.X;
			result.Bottom = r.Bottom / v.Y;
		}

		public static bool operator ==( RectangleF v1, RectangleF v2 )
		{
			return ( v1.Left == v2.Left && v1.Top == v2.Top && v1.Right == v2.Right && v1.Bottom == v2.Bottom );
		}

		public static bool operator !=( RectangleF v1, RectangleF v2 )
		{
			return ( v1.Left != v2.Left || v1.Top != v2.Top || v1.Right != v2.Right || v1.Bottom != v2.Bottom );
		}

		public unsafe float this[ int index ]
		{
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( float* v = &this.Left )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( float* v = &this.Left )
				{
					v[ index ] = value;
				}
			}
		}

		public bool Equals( RectangleF v, float epsilon )
		{
			if( Math.Abs( Left - v.Left ) > epsilon )
				return false;
			if( Math.Abs( Top - v.Top ) > epsilon )
				return false;
			if( Math.Abs( Right - v.Right ) > epsilon )
				return false;
			if( Math.Abs( Bottom - v.Bottom ) > epsilon )
				return false;
			return true;
		}

		[Browsable( false )]
		public Vector2F Size
		{
			get
			{
				Vector2F result;
				result.X = Right - Left;
				result.Y = Bottom - Top;
				return result;
			}
		}

		public Vector2F GetSize()
		{
			Vector2F result;
			result.X = Right - Left;
			result.Y = Bottom - Top;
			return result;
		}

		public void GetSize( out Vector2F result )
		{
			result.X = Right - Left;
			result.Y = Bottom - Top;
		}

		[Browsable( false )]
		public Vector2F LeftTop
		{
			get
			{
				Vector2F result;
				result.X = Left;
				result.Y = Top;
				return result;
			}
			set { Left = value.X; Top = value.Y; }
		}

		[Browsable( false )]
		public Vector2F RightTop
		{
			get
			{
				Vector2F result;
				result.X = Right;
				result.Y = Top;
				return result;
			}
			set { Right = value.X; Top = value.Y; }
		}

		[Browsable( false )]
		public Vector2F LeftBottom
		{
			get
			{
				Vector2F result;
				result.X = Left;
				result.Y = Bottom;
				return result;
			}
			set { Left = value.X; Bottom = value.Y; }
		}

		[Browsable( false )]
		public Vector2F RightBottom
		{
			get
			{
				Vector2F result;
				result.X = Right;
				result.Y = Bottom;
				return result;
			}
			set { Right = value.X; Bottom = value.Y; }
		}

		[Browsable( false )]
		public Vector2F Minimum
		{
			get
			{
				Vector2F result;
				result.X = Left;
				result.Y = Top;
				return result;
			}
			set { Left = value.X; Top = value.Y; }
		}

		[Browsable( false )]
		public Vector2F Maximum
		{
			get
			{
				Vector2F result;
				result.X = Right;
				result.Y = Bottom;
				return result;
			}
			set { Right = value.X; Bottom = value.Y; }
		}

		public bool IsInvalid()
		{
			return Right < Left || Bottom < Top;
		}

		public bool Contains( Vector2F p )
		{
			if( p.X < Left || p.Y < Top || p.X > Right || p.Y > Bottom )
				return false;
			return true;
		}

		public bool Contains( ref Vector2F p )
		{
			if( p.X < Left || p.Y < Top || p.X > Right || p.Y > Bottom )
				return false;
			return true;
		}

		public bool Contains( RectangleF v )
		{
			if( v.Left < Left || v.Top < Top || v.Right > Right || v.Bottom > Bottom )
				return false;
			return true;
		}

		public bool Contains( ref RectangleF v )
		{
			if( v.Left < Left || v.Top < Top || v.Right > Right || v.Bottom > Bottom )
				return false;
			return true;
		}

		public bool Intersects( RectangleF v )
		{
			if( v.Right < Left || v.Bottom < Top || v.Left > Right || v.Top > Bottom )
				return false;
			return true;
		}

		public bool Intersects( ref RectangleF v )
		{
			if( v.Right < Left || v.Bottom < Top || v.Left > Right || v.Top > Bottom )
				return false;
			return true;
		}

		public void Add( Vector2F v )
		{
			if( v.X < Left )
				Left = v.X;
			if( v.X > Right )
				Right = v.X;
			if( v.Y < Top )
				Top = v.Y;
			if( v.Y > Bottom )
				Bottom = v.Y;
		}

		public void Add( RectangleF a )
		{
			if( a.Left < Left )
				Left = a.Left;
			if( a.Top < Top )
				Top = a.Top;
			if( a.Right > Right )
				Right = a.Right;
			if( a.Bottom > Bottom )
				Bottom = a.Bottom;
		}

		public void Expand( float d )
		{
			Left -= d;
			Top -= d;
			Right += d;
			Bottom += d;
		}

		public void Expand( Vector2F d )
		{
			Left -= d.X;
			Top -= d.Y;
			Right += d.X;
			Bottom += d.Y;
		}

		public bool IsCleared()
		{
			return Left > Right;
		}

		public RectangleF Intersection( RectangleF v )
		{
			RectangleF result;
			result.Left = ( v.Left > Left ) ? v.Left : Left;
			result.Top = ( v.Top > Top ) ? v.Top : Top;
			result.Right = ( v.Right < Right ) ? v.Right : Right;
			result.Bottom = ( v.Bottom < Bottom ) ? v.Bottom : Bottom;
			return result;
		}

		public void Intersection( ref RectangleF v, out RectangleF result )
		{
			result.Left = ( v.Left > Left ) ? v.Left : Left;
			result.Top = ( v.Top > Top ) ? v.Top : Top;
			result.Right = ( v.Right < Right ) ? v.Right : Right;
			result.Bottom = ( v.Bottom < Bottom ) ? v.Bottom : Bottom;
		}

		public Vector2F GetCenter()
		{
			Vector2F result;
			result.X = ( Left + Right ) * .5f;
			result.Y = ( Top + Bottom ) * .5f;
			return result;
		}

		public void GetCenter( out Vector2F result )
		{
			result.X = ( Left + Right ) * .5f;
			result.Y = ( Top + Bottom ) * .5f;
		}

		[AutoConvertType]
		public Rectangle ToRectangle()
		{
			Rectangle result;
			result.Left = Left;
			result.Top = Top;
			result.Right = Right;
			result.Bottom = Bottom;
			return result;
		}

		[AutoConvertType]
		public RectangleI ToRectangleI()
		{
			RectangleI result;
			result.Left = (int)Left;
			result.Top = (int)Top;
			result.Right = (int)Right;
			result.Bottom = (int)Bottom;
			return result;
		}

		public float GetPointDistanceSquared( Vector2F point )
		{
			float x;
			if( point.X < Left )
				x = Left - point.X;
			else if( point.X > Right )
				x = point.X - Right;
			else
				x = 0;

			float y;
			if( point.Y < Top )
				y = Top - point.Y;
			else if( point.Y > Bottom )
				y = point.Y - Bottom;
			else
				y = 0;

			float sqr = x * x + y * y ;
			return sqr;
		}

		public float GetPointDistance( Vector2F point )
		{
			float sqr = GetPointDistanceSquared( point );
			if( sqr == 0 )
				return 0;
			return MathEx.Sqrt( sqr );
		}

#if !DISABLE_IMPLICIT
		public static implicit operator Rectangle( RectangleF v )
		{
			return new Rectangle( v );
		}
#endif
	}
}
