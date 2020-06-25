// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Represents a type to indicate position and size for UI elements.
	/// </summary>
	public enum UIMeasure
	{
		Parent,
		Units,
		Pixels,
		Screen,
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Represents a value to indicate a floating point value for UI elements.
	/// </summary>
	[HCExpandable]
	public struct UIMeasureValueDouble
	{
		public UIMeasureValueDouble( UIMeasure type, double value )
		{
			Measure = type;
			Value = value;
		}

		[Serialize]
		//[DefaultValue( UIMeasure.Parent )]
		public UIMeasure Measure
		{
			get;
			set;
		}

		[Serialize]
		//[DefaultValue( 0.0 )]
		public double Value
		{
			get;
			set;
		}

		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1}", Measure, Value );
		}

		[AutoConvertType]
		public static UIMeasureValueDouble Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 2 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 2 parts separated by spaces in the form (Measure Value).", text ) );

			try
			{
				return new UIMeasureValueDouble(
					(UIMeasure)Enum.Parse( typeof( UIMeasure ), vals[ 0 ] ),
					double.Parse( vals[ 1 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "Invalid format." );
			}
		}

		public override bool Equals( object obj )
		{
			return ( obj is UIMeasureValueDouble && this == (UIMeasureValueDouble)obj );
		}

		public override int GetHashCode()
		{
			return ( Measure.GetHashCode() ^ Value.GetHashCode() );
		}

		public static bool operator ==( UIMeasureValueDouble v1, UIMeasureValueDouble v2 )
		{
			return ( v1.Measure == v2.Measure && v1.Value == v2.Value );
		}

		public static bool operator !=( UIMeasureValueDouble v1, UIMeasureValueDouble v2 )
		{
			return ( v1.Measure != v2.Measure || v1.Value != v2.Value );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Represents a value to indicate a two-dimensional vector value for UI elements.
	/// </summary>
	[HCExpandable]
	public struct UIMeasureValueVector2
	//public sealed class SizeValueType. класс тогда должен быть иммутабельным, а то можно двум контролям присвить один инстанс.
	{
		public UIMeasureValueVector2( UIMeasure measure, double x, double y )
		{
			Measure = measure;
			X = x;
			Y = y;
		}

		public UIMeasureValueVector2( UIMeasure type, Vector2 value )
		{
			Measure = type;
			X = value.X;
			Y = value.Y;
		}

		[Serialize]
		//[DefaultValue( UIMeasure.Parent )]
		public UIMeasure Measure
		{
			get;
			set;
		}

		[Serialize]
		//[DefaultValue( 0.0 )]
		public double X
		{
			get;
			set;
		}

		[Serialize]
		//[DefaultValue( 0.0 )]
		public double Y
		{
			get;
			set;
		}

		[Browsable( false )]
		public Vector2 Value
		{
			get { return new Vector2( X, Y ); }
			set { X = value.X; Y = value.Y; }
		}

		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1} {2}", Measure, X, Y );
		}

		[AutoConvertType]
		public static UIMeasureValueVector2 Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 3 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 3 parts separated by spaces in the form (Measure X Y).", text ) );

			try
			{
				return new UIMeasureValueVector2(
					(UIMeasure)Enum.Parse( typeof( UIMeasure ), vals[ 0 ] ),
					double.Parse( vals[ 1 ] ),
					double.Parse( vals[ 2 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "Invalid format." );
			}
		}

		public override bool Equals( object obj )
		{
			return ( obj is UIMeasureValueVector2 && this == (UIMeasureValueVector2)obj );
		}

		public override int GetHashCode()
		{
			return ( Measure.GetHashCode() ^ X.GetHashCode() ^ Y.GetHashCode() );
		}

		public static bool operator ==( UIMeasureValueVector2 v1, UIMeasureValueVector2 v2 )
		{
			return ( v1.Measure == v2.Measure && v1.X == v2.X && v1.Y == v2.Y );
		}

		public static bool operator !=( UIMeasureValueVector2 v1, UIMeasureValueVector2 v2 )
		{
			return ( v1.Measure != v2.Measure || v1.X != v2.X || v1.Y != v2.Y );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Represents a value to indicate a rectangle value for UI elements.
	/// </summary>
	[HCExpandable]
	public struct UIMeasureValueRectangle
	{
		public UIMeasureValueRectangle( UIMeasure measure, double left, double top, double right, double bottom )
		{
			Measure = measure;
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public UIMeasureValueRectangle( UIMeasure measure, Rectangle value )
		{
			Measure = measure;
			Left = value.Left;
			Top = value.Top;
			Right = value.Right;
			Bottom = value.Bottom;
		}

		[Serialize]
		//[DefaultValue( UIMeasure.Parent )]
		public UIMeasure Measure
		{
			get;
			set;
		}

		[Serialize]
		//[DefaultValue( 0.0 )]
		public double Left
		{
			get;
			set;
		}

		[Serialize]
		//[DefaultValue( 0.0 )]
		public double Top
		{
			get;
			set;
		}

		[Serialize]
		//[DefaultValue( 0.0 )]
		public double Right
		{
			get;
			set;
		}

		[Serialize]
		//[DefaultValue( 0.0 )]
		public double Bottom
		{
			get;
			set;
		}

		[Browsable( false )]
		public Rectangle Value
		{
			get { return new Rectangle( Left, Top, Right, Bottom ); }
			set { Left = value.Left; Top = value.Top; Right = value.Right; Bottom = value.Bottom; }
		}

		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1} {2} {3} {4}", Measure, Left, Top, Right, Bottom );
		}

		[AutoConvertType]
		public static UIMeasureValueRectangle Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 5 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 5 parts separated by spaces in the form (Measure Left Top Right Bottom).", text ) );

			try
			{
				return new UIMeasureValueRectangle(
					(UIMeasure)Enum.Parse( typeof( UIMeasure ), vals[ 0 ] ),
					double.Parse( vals[ 1 ] ),
					double.Parse( vals[ 2 ] ),
					double.Parse( vals[ 3 ] ),
					double.Parse( vals[ 4 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "Invalid format." );
			}
		}

		public override bool Equals( object obj )
		{
			return ( obj is UIMeasureValueVector2 && this == (UIMeasureValueRectangle)obj );
		}

		public override int GetHashCode()
		{
			return ( Measure.GetHashCode() ^ Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode() );
		}

		public static bool operator ==( UIMeasureValueRectangle v1, UIMeasureValueRectangle v2 )
		{
			return ( v1.Measure == v2.Measure && v1.Left == v2.Left && v1.Top == v2.Top && v1.Right == v2.Right && v1.Bottom == v2.Bottom );
		}

		public static bool operator !=( UIMeasureValueRectangle v1, UIMeasureValueRectangle v2 )
		{
			return ( v1.Measure != v2.Measure || v1.Left != v2.Left || v1.Top != v2.Top || v1.Right != v2.Right || v1.Bottom != v2.Bottom );
		}
	}
}
