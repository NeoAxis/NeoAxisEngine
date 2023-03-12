// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<Rect> ) )]
	/// <summary>
	/// Represents a rectangle with double precision floating values.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct Rectangle
	{
		public double Left;
		public double Top;
		public double Right;
		public double Bottom;

		public static readonly Rectangle Zero = new Rectangle( 0.0, 0.0, 0.0, 0.0 );

		public static readonly Rectangle Cleared = new Rectangle(
			new Vector2( double.MaxValue, double.MaxValue ),
			new Vector2( double.MinValue, double.MinValue ) );

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Rectangle( Rectangle source )
		{
			Left = source.Left;
			Top = source.Top;
			Right = source.Right;
			Bottom = source.Bottom;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Rectangle( double left, double top, double right, double bottom )
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Rectangle( Vector2 leftTop, Vector2 rightBottom )
		{
			this.Left = leftTop.X;
			this.Top = leftTop.Y;
			this.Right = rightBottom.X;
			this.Bottom = rightBottom.Y;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Rectangle( RectangleF source )
		{
			Left = source.Left;
			Top = source.Top;
			Right = source.Right;
			Bottom = source.Bottom;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Rectangle( RectangleI source )
		{
			Left = source.Left;
			Top = source.Top;
			Right = source.Right;
			Bottom = source.Bottom;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Rectangle( Vector2 v )
		{
			Left = v.X;
			Right = v.X;
			Top = v.Y;
			Bottom = v.Y;
		}

		[AutoConvertType]
		public static Rectangle Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (left top right bottom).", text ) );

			try
			{
				return new Rectangle(
					double.Parse( vals[ 0 ] ),
					double.Parse( vals[ 1 ] ),
					double.Parse( vals[ 2 ] ),
					double.Parse( vals[ 3 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		[AutoConvertType]
		public override string ToString()
		{
			return ToString( 17 );
			//return string.Format( "{0} {1} {2} {3}", left, top, right, bottom );
		}

		public string ToString( int precision )
		{
			string format = "";
			format = format.PadLeft( precision, '#' );
			format = "{0:0." + format + "} {1:0." + format + "} {2:0." + format + "} {3:0." + format + "}";
			return string.Format( format, Left, Top, Right, Bottom );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is Rectangle && this == (Rectangle)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Rectangle operator +( Rectangle r, Vector2 v )
		{
			Rectangle result;
			result.Left = r.Left + v.X;
			result.Top = r.Top + v.Y;
			result.Right = r.Right + v.X;
			result.Bottom = r.Bottom + v.Y;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Rectangle operator +( Vector2 v, Rectangle r )
		{
			Rectangle result;
			result.Left = v.X + r.Left;
			result.Top = v.Y + r.Top;
			result.Right = v.X + r.Right;
			result.Bottom = v.Y + r.Bottom;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Rectangle operator -( Rectangle r, Vector2 v )
		{
			Rectangle result;
			result.Left = r.Left - v.X;
			result.Top = r.Top - v.Y;
			result.Right = r.Right - v.X;
			result.Bottom = r.Bottom - v.Y;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Rectangle operator -( Vector2 v, Rectangle r )
		{
			Rectangle result;
			result.Left = v.X - r.Left;
			result.Top = v.Y - r.Top;
			result.Right = v.X - r.Right;
			result.Bottom = v.Y - r.Bottom;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Rectangle operator *( Rectangle r, Vector2 v )
		{
			Rectangle result;
			result.Left = r.Left * v.X;
			result.Top = r.Top * v.Y;
			result.Right = r.Right * v.X;
			result.Bottom = r.Bottom * v.Y;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Rectangle operator *( Vector2 v, Rectangle r )
		{
			Rectangle result;
			result.Left = v.X * r.Left;
			result.Top = v.Y * r.Top;
			result.Right = v.X * r.Right;
			result.Bottom = v.Y * r.Bottom;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Rectangle operator /( Rectangle r, Vector2 v )
		{
			Rectangle result;
			result.Left = r.Left / v.X;
			result.Top = r.Top / v.Y;
			result.Right = r.Right / v.X;
			result.Bottom = r.Bottom / v.Y;
			return result;
		}

		//public static Rect Add( Rect r, Vec2 v )
		//{
		//	Rect result;
		//	result.left = r.left + v.X;
		//	result.top = r.top + v.Y;
		//	result.right = r.right + v.X;
		//	result.bottom = r.bottom + v.Y;
		//	return result;
		//}

		//public static Rect Add( Vec2 v, Rect r )
		//{
		//	Rect result;
		//	result.left = v.X + r.left;
		//	result.top = v.Y + r.top;
		//	result.right = v.X + r.right;
		//	result.bottom = v.Y + r.bottom;
		//	return result;
		//}

		//public static Rect Subtract( Rect r, Vec2 v )
		//{
		//	Rect result;
		//	result.left = r.left - v.X;
		//	result.top = r.top - v.Y;
		//	result.right = r.right - v.X;
		//	result.bottom = r.bottom - v.Y;
		//	return result;
		//}

		//public static Rect Subtract( Vec2 v, Rect r )
		//{
		//	Rect result;
		//	result.left = v.X - r.left;
		//	result.top = v.Y - r.top;
		//	result.right = v.X - r.right;
		//	result.bottom = v.Y - r.bottom;
		//	return result;
		//}

		//public static void Add( Rect r, Vec2 v, out Rect result )
		//{
		//	result.left = r.left + v.X;
		//	result.top = r.top + v.Y;
		//	result.right = r.right + v.X;
		//	result.bottom = r.bottom + v.Y;
		//}

		//public static void Add( Vec2 v, Rect r, out Rect result )
		//{
		//	result.left = v.X + r.left;
		//	result.top = v.Y + r.top;
		//	result.right = v.X + r.right;
		//	result.bottom = v.Y + r.bottom;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Add( ref Rectangle r, ref Vector2 v, out Rectangle result )
		{
			result.Left = r.Left + v.X;
			result.Top = r.Top + v.Y;
			result.Right = r.Right + v.X;
			result.Bottom = r.Bottom + v.Y;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Add( ref Vector2 v, ref Rectangle r, out Rectangle result )
		{
			result.Left = v.X + r.Left;
			result.Top = v.Y + r.Top;
			result.Right = v.X + r.Right;
			result.Bottom = v.Y + r.Bottom;
		}

		//public static void Subtract( Rect r, Vec2 v, out Rect result )
		//{
		//	result.left = r.left - v.X;
		//	result.top = r.top - v.Y;
		//	result.right = r.right - v.X;
		//	result.bottom = r.bottom - v.Y;
		//}

		//public static void Subtract( Vec2 v, Rect r, out Rect result )
		//{
		//	result.left = v.X - r.left;
		//	result.top = v.Y - r.top;
		//	result.right = v.X - r.right;
		//	result.bottom = v.Y - r.bottom;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Subtract( ref Rectangle r, ref Vector2 v, out Rectangle result )
		{
			result.Left = r.Left - v.X;
			result.Top = r.Top - v.Y;
			result.Right = r.Right - v.X;
			result.Bottom = r.Bottom - v.Y;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Subtract( ref Vector2 v, ref Rectangle r, out Rectangle result )
		{
			result.Left = v.X - r.Left;
			result.Top = v.Y - r.Top;
			result.Right = v.X - r.Right;
			result.Bottom = v.Y - r.Bottom;
		}

		//public static void Multiply( Rect r, Vec2 v, out Rect result )
		//{
		//	result.left = r.left * v.X;
		//	result.top = r.top * v.Y;
		//	result.right = r.right * v.X;
		//	result.bottom = r.bottom * v.Y;
		//}

		//public static void Multiply( Vec2 v, Rect r, out Rect result )
		//{
		//	result.left = v.X * r.left;
		//	result.top = v.Y * r.top;
		//	result.right = v.X * r.right;
		//	result.bottom = v.Y * r.bottom;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Rectangle r, ref Vector2 v, out Rectangle result )
		{
			result.Left = r.Left * v.X;
			result.Top = r.Top * v.Y;
			result.Right = r.Right * v.X;
			result.Bottom = r.Bottom * v.Y;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Multiply( ref Vector2 v, ref Rectangle r, out Rectangle result )
		{
			result.Left = v.X * r.Left;
			result.Top = v.Y * r.Top;
			result.Right = v.X * r.Right;
			result.Bottom = v.Y * r.Bottom;
		}

		//public static void Divide( Rect r, Vec2 v, out Rect result )
		//{
		//	result.left = r.left / v.X;
		//	result.top = r.top / v.Y;
		//	result.right = r.right / v.X;
		//	result.bottom = r.bottom / v.Y;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Divide( ref Rectangle r, ref Vector2 v, out Rectangle result )
		{
			result.Left = r.Left / v.X;
			result.Top = r.Top / v.Y;
			result.Right = r.Right / v.X;
			result.Bottom = r.Bottom / v.Y;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( Rectangle v1, Rectangle v2 )
		{
			return ( v1.Left == v2.Left && v1.Top == v2.Top && v1.Right == v2.Right && v1.Bottom == v2.Bottom );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( Rectangle v1, Rectangle v2 )
		{
			return ( v1.Left != v2.Left || v1.Top != v2.Top || v1.Right != v2.Right || v1.Bottom != v2.Bottom );
		}

		public unsafe double this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( double* v = &this.Left )
				{
					return v[ index ];
				}
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed( double* v = &this.Left )
				{
					v[ index ] = value;
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Equals( Rectangle v, double epsilon )
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
		public Vector2 Size
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2 result;
				result.X = Right - Left;
				result.Y = Bottom - Top;
				return result;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2 GetSize()
		{
			Vector2 result;
			result.X = Right - Left;
			result.Y = Bottom - Top;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetSize( out Vector2 result )
		{
			result.X = Right - Left;
			result.Y = Bottom - Top;
		}

		[Browsable( false )]
		public Vector2 LeftTop
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2 result;
				result.X = Left;
				result.Y = Top;
				return result;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Left = value.X; Top = value.Y; }
		}

		[Browsable( false )]
		public Vector2 RightTop
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2 result;
				result.X = Right;
				result.Y = Top;
				return result;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Right = value.X; Top = value.Y; }
		}

		[Browsable( false )]
		public Vector2 LeftBottom
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2 result;
				result.X = Left;
				result.Y = Bottom;
				return result;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Left = value.X; Bottom = value.Y; }
		}

		[Browsable( false )]
		public Vector2 RightBottom
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2 result;
				result.X = Right;
				result.Y = Bottom;
				return result;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Right = value.X; Bottom = value.Y; }
		}

		[Browsable( false )]
		public Vector2 Minimum
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2 result;
				result.X = Left;
				result.Y = Top;
				return result;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Left = value.X; Top = value.Y; }
		}

		[Browsable( false )]
		public Vector2 Maximum
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2 result;
				result.X = Right;
				result.Y = Bottom;
				return result;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Right = value.X; Bottom = value.Y; }
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IsInvalid()
		{
			return Right < Left || Bottom < Top;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( Vector2 p )
		{
			if( p.X < Left || p.Y < Top || p.X > Right || p.Y > Bottom )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( ref Vector2 p )
		{
			if( p.X < Left || p.Y < Top || p.X > Right || p.Y > Bottom )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( Rectangle v )
		{
			if( v.Left < Left || v.Top < Top || v.Right > Right || v.Bottom > Bottom )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( ref Rectangle v )
		{
			if( v.Left < Left || v.Top < Top || v.Right > Right || v.Bottom > Bottom )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( Rectangle v )
		{
			if( v.Right < Left || v.Bottom < Top || v.Left > Right || v.Top > Bottom )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref Rectangle v )
		{
			if( v.Right < Left || v.Bottom < Top || v.Left > Right || v.Top > Bottom )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Add( Vector2 v )
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Add( ref Vector2 v )
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Add( Rectangle a )
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Expand( double d )
		{
			Left -= d;
			Top -= d;
			Right += d;
			Bottom += d;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Expand( Vector2 d )
		{
			Left -= d.X;
			Top -= d.Y;
			Right += d.X;
			Bottom += d.Y;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool IsCleared()
		{
			return Left > Right;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Rectangle Intersection( Rectangle v )
		{
			Rectangle result;
			result.Left = ( v.Left > Left ) ? v.Left : Left;
			result.Top = ( v.Top > Top ) ? v.Top : Top;
			result.Right = ( v.Right < Right ) ? v.Right : Right;
			result.Bottom = ( v.Bottom < Bottom ) ? v.Bottom : Bottom;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Intersection( ref Rectangle v, out Rectangle result )
		{
			result.Left = ( v.Left > Left ) ? v.Left : Left;
			result.Top = ( v.Top > Top ) ? v.Top : Top;
			result.Right = ( v.Right < Right ) ? v.Right : Right;
			result.Bottom = ( v.Bottom < Bottom ) ? v.Bottom : Bottom;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2 GetCenter()
		{
			Vector2 result;
			result.X = ( Left + Right ) * .5;
			result.Y = ( Top + Bottom ) * .5;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetCenter( out Vector2 result )
		{
			result.X = ( Left + Right ) * .5;
			result.Y = ( Top + Bottom ) * .5;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public RectangleF ToRectangleF()
		{
			RectangleF result;
			result.Left = (float)Left;
			result.Top = (float)Top;
			result.Right = (float)Right;
			result.Bottom = (float)Bottom;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double GetPointDistanceSquared( ref Vector2 point )
		{
			double x;
			if( point.X < Left )
				x = Left - point.X;
			else if( point.X > Right )
				x = point.X - Right;
			else
				x = 0;

			double y;
			if( point.Y < Top )
				y = Top - point.Y;
			else if( point.Y > Bottom )
				y = point.Y - Bottom;
			else
				y = 0;

			double sqr = x * x + y * y;
			return sqr;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double GetPointDistanceSquared( Vector2 point )
		{
			return GetPointDistanceSquared( ref point );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public double GetPointDistance( Vector2 point )
		{
			double sqr = GetPointDistanceSquared( point );
			if( sqr == 0 )
				return 0;
			return Math.Sqrt( sqr );
		}

		//!!!!check

		//[MethodImpl( (MethodImplOptions)512 )]
		//public bool Intersects( ref Triangle2 triangle )
		//{
		//	//https://github.com/juj/MathGeoLib/blob/master/src/Geometry/Triangle.cpp#L697

		//	var a = triangle.A;
		//	var b = triangle.B;
		//	var c = triangle.C;

		//	var triangleBounds = new Rectangle( a );
		//	triangleBounds.Add( ref b );
		//	triangleBounds.Add( ref c );
		//	var tMin = triangleBounds.Minimum;
		//	var tMax = triangleBounds.Maximum;
		//	//var tMin = a.Min( b.Min( c ) );
		//	//var tMax = a.Max( b.Max( c ) );

		//	if( tMin.X >= Maximum.X || tMax.X <= Minimum.X || tMin.Y >= Maximum.Y || tMax.Y <= Minimum.Y )
		//		return false;

		//	var center = ( Minimum + Maximum ) * 0.5f;
		//	var h = Maximum - center;

		//	var t0 = b - a;
		//	var t1 = c - a;
		//	var t2 = c - b;

		//	var ac = a - center;

		//	var n = Vector2.Cross( t0, t1 );
		//	var s = n.Dot( ac );
		//	var r = Math.Abs( h.Dot( Vector2.Abs( n ) ) );
		//	if( Math.Abs( s ) >= r )
		//		return false;

		//	var at0 = Vector2.Abs( t0 );
		//	var at1 = Vector2.Abs( t1 );
		//	var at2 = Vector2.Abs( t2 );

		//	var bc = b - center;
		//	var cc = c - center;

		//	// eZ <cross> t[0]
		//	var d1 = t0.X * ac.Y - t0.Y * ac.X;
		//	var d2 = t0.X * cc.Y - t0.Y * cc.X;
		//	var tc = ( d1 + d2 ) * 0.5f;
		//	r = Math.Abs( h.Y * at0.X + h.X * at0.Y );
		//	if( r + Math.Abs( tc - d1 ) < Math.Abs( tc ) )
		//		return false;

		//	// eZ <cross> t[1]
		//	d1 = t1.X * ac.Y - t1.Y * ac.X;
		//	d2 = t1.X * bc.Y - t1.Y * bc.X;
		//	tc = ( d1 + d2 ) * 0.5f;
		//	r = Math.Abs( h.Y * at1.X + h.X * at1.Y );
		//	if( r + Math.Abs( tc - d1 ) < Math.Abs( tc ) )
		//		return false;

		//	// eZ <cross> t[2]
		//	d1 = t2.X * ac.Y - t2.Y * ac.X;
		//	d2 = t2.X * bc.Y - t2.Y * bc.X;
		//	tc = ( d1 + d2 ) * 0.5f;
		//	r = Math.Abs( h.Y * at2.X + h.X * at2.Y );
		//	if( r + Math.Abs( tc - d1 ) < Math.Abs( tc ) )
		//		return false;

		//	// No separating axis exists, the AABB and triangle intersect.
		//	return true;
		//}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//public bool Intersects( Triangle2 triangle )
		//{
		//	return Intersects( ref triangle );
		//}

	}
}
