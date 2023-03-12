// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using NeoAxis.Editor;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<RectI> ) )]
	/// <summary>
	/// Represents a rectangle with integer values.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	[HCExpandable]
	public struct RectangleI
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public static readonly RectangleI Zero = new RectangleI( 0, 0, 0, 0 );
		public static readonly RectangleI Cleared = new RectangleI( int.MaxValue, int.MaxValue,
			int.MinValue, int.MinValue );

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RectangleI( RectangleI source )
		{
			Left = source.Left;
			Top = source.Top;
			Right = source.Right;
			Bottom = source.Bottom;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RectangleI( int left, int top, int right, int bottom )
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public RectangleI( Vector2I min, Vector2I max )
		{
			Left = min.X;
			Top = min.Y;
			Right = max.X;
			Bottom = max.Y;
		}

		[AutoConvertType]
		public static RectangleI Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 4 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 4 parts separated by spaces in the form (left top right bottom).", text ) );

			try
			{
				return new RectangleI(
					int.Parse( vals[ 0 ] ),
					int.Parse( vals[ 1 ] ),
					int.Parse( vals[ 2 ] ),
					int.Parse( vals[ 3 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1} {2} {3}", Left, Top, Right, Bottom );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is RectangleI && this == (RectangleI)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return ( Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode() );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RectangleI operator +( RectangleI r, Vector2I v )
		{
			RectangleI result;
			result.Left = r.Left + v.X;
			result.Top = r.Top + v.Y;
			result.Right = r.Right + v.X;
			result.Bottom = r.Bottom + v.Y;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static RectangleI operator -( RectangleI r, Vector2I v )
		{
			RectangleI result;
			result.Left = r.Left - v.X;
			result.Top = r.Top - v.Y;
			result.Right = r.Right - v.X;
			result.Bottom = r.Bottom - v.Y;
			return result;
		}

		//public static RectI Add( RectI r, Vec2I v )
		//{
		//	RectI result;
		//	result.left = r.left + v.X;
		//	result.top = r.top + v.Y;
		//	result.right = r.right + v.X;
		//	result.bottom = r.bottom + v.Y;
		//	return result;
		//}

		//public static RectI Subtract( RectI r, Vec2I v )
		//{
		//	RectI result;
		//	result.left = r.left - v.X;
		//	result.top = r.top - v.Y;
		//	result.right = r.right - v.X;
		//	result.bottom = r.bottom - v.Y;
		//	return result;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Add( ref RectangleI r, ref Vector2I v, out RectangleI result )
		{
			result.Left = r.Left + v.X;
			result.Top = r.Top + v.Y;
			result.Right = r.Right + v.X;
			result.Bottom = r.Bottom + v.Y;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static void Subtract( ref RectangleI r, ref Vector2I v, out RectangleI result )
		{
			result.Left = r.Left - v.X;
			result.Top = r.Top - v.Y;
			result.Right = r.Right - v.X;
			result.Bottom = r.Bottom - v.Y;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( RectangleI v1, RectangleI v2 )
		{
			return ( v1.Left == v2.Left && v1.Top == v2.Top && v1.Right == v2.Right && v1.Bottom == v2.Bottom );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( RectangleI v1, RectangleI v2 )
		{
			return ( v1.Left != v2.Left || v1.Top != v2.Top || v1.Right != v2.Right || v1.Bottom != v2.Bottom );
		}

		public unsafe int this[ int index ]
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( int* v = &this.Left )
				{
					return v[ index ];
				}
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set
			{
				if( index < 0 || index > 3 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( int* v = &this.Left )
				{
					v[ index ] = value;
				}
			}
		}

		[Browsable( false )]
		public Vector2I Size
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2I result;
				result.X = Right - Left;
				result.Y = Bottom - Top;
				return result;
			}
		}

		//public Vec2I GetSize()
		//{
		//	Vec2I result;
		//	result.x = right - left;
		//	result.y = bottom - top;
		//	return result;
		//}

		//public void GetSize( out Vec2I result )
		//{
		//	result.x = right - left;
		//	result.y = bottom - top;
		//}

		[Browsable( false )]
		public Vector2I LeftTop
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2I result;
				result.X = Left;
				result.Y = Top;
				return result;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Left = value.X; Top = value.Y; }
		}

		[Browsable( false )]
		public Vector2I RightTop
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2I result;
				result.X = Right;
				result.Y = Top;
				return result;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Right = value.X; Top = value.Y; }
		}

		[Browsable( false )]
		public Vector2I LeftBottom
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2I result;
				result.X = Left;
				result.Y = Bottom;
				return result;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Left = value.X; Bottom = value.Y; }
		}

		[Browsable( false )]
		public Vector2I RightBottom
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2I result;
				result.X = Right;
				result.Y = Bottom;
				return result;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Right = value.X; Bottom = value.Y; }
		}

		[Browsable( false )]
		public Vector2I Minimum
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2I result;
				result.X = Left;
				result.Y = Top;
				return result;
			}
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			set { Left = value.X; Top = value.Y; }
		}

		[Browsable( false )]
		public Vector2I Maximum
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				Vector2I result;
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
		public void Add( Vector2I v )
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
		public void Add( RectangleI a )
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
		public void Expand( int d )
		{
			Left -= d;
			Top -= d;
			Right += d;
			Bottom += d;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Expand( Vector2I d )
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
		public RectangleI Intersection( RectangleI v )
		{
			RectangleI result;
			result.Left = ( v.Left > Left ) ? v.Left : Left;
			result.Top = ( v.Top > Top ) ? v.Top : Top;
			result.Right = ( v.Right < Right ) ? v.Right : Right;
			result.Bottom = ( v.Bottom < Bottom ) ? v.Bottom : Bottom;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void Intersection( ref RectangleI v, out RectangleI result )
		{
			result.Left = ( v.Left > Left ) ? v.Left : Left;
			result.Top = ( v.Top > Top ) ? v.Top : Top;
			result.Right = ( v.Right < Right ) ? v.Right : Right;
			result.Bottom = ( v.Bottom < Bottom ) ? v.Bottom : Bottom;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public Vector2I GetCenter()
		{
			Vector2I result;
			result.X = ( Left + Right ) / 2;
			result.Y = ( Top + Bottom ) / 2;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void GetCenter( out Vector2I result )
		{
			result.X = ( Left + Right ) / 2;
			result.Y = ( Top + Bottom ) / 2;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( Vector2I p )
		{
			if( p.X < Left || p.Y < Top || p.X > Right || p.Y > Bottom )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( ref Vector2I p )
		{
			if( p.X < Left || p.Y < Top || p.X > Right || p.Y > Bottom )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( RectangleI v )
		{
			if( v.Left < Left || v.Top < Top || v.Right > Right || v.Bottom > Bottom )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Contains( ref RectangleI v )
		{
			if( v.Left < Left || v.Top < Top || v.Right > Right || v.Bottom > Bottom )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( RectangleI v )
		{
			if( v.Right < Left || v.Bottom < Top || v.Left > Right || v.Top > Bottom )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public bool Intersects( ref RectangleI v )
		{
			if( v.Right < Left || v.Bottom < Top || v.Left > Right || v.Top > Bottom )
				return false;
			return true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		[AutoConvertType]
		public RectangleF ToRectangleF()
		{
			RectangleF result;
			result.Left = Left;
			result.Top = Top;
			result.Right = Right;
			result.Bottom = Bottom;
			return result;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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
	}
}
