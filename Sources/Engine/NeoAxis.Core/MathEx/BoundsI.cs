// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace NeoAxis
{
	//[TypeConverter( typeof( MathExGeneralTypeConverter<BoundsI> ) )]
	/// <summary>
	/// Represents an axis-aligned bounding box with integer values in three dimensional space.
	/// </summary>
	[StructLayout( LayoutKind.Sequential )]
	public struct BoundsI
	{
		public Vector3I Minimum;
		public Vector3I Maximum;

		public static readonly BoundsI Zero = new BoundsI( Vector3I.Zero, Vector3I.Zero );
		public static readonly BoundsI Cleared = new BoundsI(
			new Vector3I( int.MaxValue, int.MaxValue, int.MaxValue ),
			new Vector3I( int.MinValue, int.MinValue, int.MinValue ) );

		public BoundsI( BoundsI source )
		{
			Minimum = source.Minimum;
			Maximum = source.Maximum;
		}

		public BoundsI( Vector3I minimum, Vector3I maximum )
		{
			this.Minimum = minimum;
			this.Maximum = maximum;
		}

		public BoundsI( Vector3I v )
		{
			this.Minimum = v;
			this.Maximum = v;
		}

		public BoundsI( int minimumX, int minimumY, int minimumZ,
			int maximumX, int maximumY, int maximumZ )
		{
			Minimum.X = minimumX;
			Minimum.Y = minimumY;
			Minimum.Z = minimumZ;
			Maximum.X = maximumX;
			Maximum.Y = maximumY;
			Maximum.Z = maximumZ;
		}

		public override bool Equals( object obj )
		{
			return ( obj is BoundsI && this == (BoundsI)obj );
		}

		public override int GetHashCode()
		{
			return ( Minimum.GetHashCode() ^ Maximum.GetHashCode() );
		}

		public static bool operator ==( BoundsI b1, BoundsI b2 )
		{
			return ( b1.Minimum == b2.Minimum && b1.Maximum == b2.Maximum );
		}

		public static bool operator !=( BoundsI b1, BoundsI b2 )
		{
			return ( b1.Minimum != b2.Minimum || b1.Maximum != b2.Maximum );
		}

		public unsafe Vector3I this[ int index ]
		{
			get
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector3I* v = &this.Minimum )
				{
					return v[ index ];
				}
			}
			set
			{
				if( index < 0 || index > 1 )
					throw new ArgumentOutOfRangeException( "index" );
				fixed ( Vector3I* v = &this.Minimum )
				{
					v[ index ] = value;
				}
			}
		}

		public bool IsCleared()
		{
			return Minimum.X > Maximum.X;
		}

		public int GetVolume()
		{
			Vector3I s;
			Vector3I.Subtract( ref Maximum, ref Minimum, out s );
			return s.X * s.Y * s.Z;
		}

		public void Add( Vector3I v )
		{
			if( v.X < Minimum.X )
				Minimum.X = v.X;
			if( v.X > Maximum.X )
				Maximum.X = v.X;
			if( v.Y < Minimum.Y )
				Minimum.Y = v.Y;
			if( v.Y > Maximum.Y )
				Maximum.Y = v.Y;
			if( v.Z < Minimum.Z )
				Minimum.Z = v.Z;
			if( v.Z > Maximum.Z )
				Maximum.Z = v.Z;
		}

		public void Add( ref Vector3I v )
		{
			if( v.X < Minimum.X )
				Minimum.X = v.X;
			if( v.X > Maximum.X )
				Maximum.X = v.X;
			if( v.Y < Minimum.Y )
				Minimum.Y = v.Y;
			if( v.Y > Maximum.Y )
				Maximum.Y = v.Y;
			if( v.Z < Minimum.Z )
				Minimum.Z = v.Z;
			if( v.Z > Maximum.Z )
				Maximum.Z = v.Z;
		}

		public void Add( BoundsI b )
		{
			if( b.Minimum.X < Minimum.X )
				Minimum.X = b.Minimum.X;
			if( b.Minimum.Y < Minimum.Y )
				Minimum.Y = b.Minimum.Y;
			if( b.Minimum.Z < Minimum.Z )
				Minimum.Z = b.Minimum.Z;
			if( b.Maximum.X > Maximum.X )
				Maximum.X = b.Maximum.X;
			if( b.Maximum.Y > Maximum.Y )
				Maximum.Y = b.Maximum.Y;
			if( b.Maximum.Z > Maximum.Z )
				Maximum.Z = b.Maximum.Z;
		}

		public void Add( ICollection<Vector3I> collection )
		{
			foreach( var v in collection )
				Add( v );
		}

		public void Add( Vector3I[] collection )
		{
			for( int i = 0; i < collection.Length; i++ )
				Add( ref collection[ i ] );
		}

		public BoundsI Intersection( BoundsI v )
		{
			BoundsI result;
			result.Minimum.X = ( v.Minimum.X > Minimum.X ) ? v.Minimum.X : Minimum.X;
			result.Minimum.Y = ( v.Minimum.Y > Minimum.Y ) ? v.Minimum.Y : Minimum.Y;
			result.Minimum.Z = ( v.Minimum.Z > Minimum.Z ) ? v.Minimum.Z : Minimum.Z;
			result.Maximum.X = ( v.Maximum.X < Maximum.X ) ? v.Maximum.X : Maximum.X;
			result.Maximum.Y = ( v.Maximum.Y < Maximum.Y ) ? v.Maximum.Y : Maximum.Y;
			result.Maximum.Z = ( v.Maximum.Z < Maximum.Z ) ? v.Maximum.Z : Maximum.Z;
			return result;
		}

		public void Intersection( BoundsI v, out BoundsI result )
		{
			result.Minimum.X = ( v.Minimum.X > Minimum.X ) ? v.Minimum.X : Minimum.X;
			result.Minimum.Y = ( v.Minimum.Y > Minimum.Y ) ? v.Minimum.Y : Minimum.Y;
			result.Minimum.Z = ( v.Minimum.Z > Minimum.Z ) ? v.Minimum.Z : Minimum.Z;
			result.Maximum.X = ( v.Maximum.X < Maximum.X ) ? v.Maximum.X : Maximum.X;
			result.Maximum.Y = ( v.Maximum.Y < Maximum.Y ) ? v.Maximum.Y : Maximum.Y;
			result.Maximum.Z = ( v.Maximum.Z < Maximum.Z ) ? v.Maximum.Z : Maximum.Z;
		}

		public void Expand( int d )
		{
			Minimum.X -= d;
			Minimum.Y -= d;
			Minimum.Z -= d;
			Maximum.X += d;
			Maximum.Y += d;
			Maximum.Z += d;
		}

		public void Expand( Vector3I d )
		{
			Minimum.X -= d.X;
			Minimum.Y -= d.Y;
			Minimum.Z -= d.Z;
			Maximum.X += d.X;
			Maximum.Y += d.Y;
			Maximum.Z += d.Z;
		}

		public bool Contains( Vector3I p )
		{
			if( p.X < Minimum.X || p.Y < Minimum.Y || p.Z < Minimum.Z
				|| p.X > Maximum.X || p.Y > Maximum.Y || p.Z > Maximum.Z )
				return false;
			return true;
		}

		public bool Contains( BoundsI b )
		{
			if( b.Minimum.X < Minimum.X || b.Minimum.Y < Minimum.Y || b.Minimum.Z < Minimum.Z
				|| b.Maximum.X > Maximum.X || b.Maximum.Y > Maximum.Y || b.Maximum.Z > Maximum.Z )
				return false;
			return true;
		}

		public bool Intersects( BoundsI b )
		{
			if( b.Maximum.X < Minimum.X || b.Maximum.Y < Minimum.Y || b.Maximum.Z < Minimum.Z
				|| b.Minimum.X > Maximum.X || b.Minimum.Y > Maximum.Y || b.Minimum.Z > Maximum.Z )
				return false;
			return true;
		}

		public Vector3I GetSize()
		{
			Vector3I result;
			Vector3I.Subtract( ref Maximum, ref Minimum, out result );
			return result;
		}

		public Vector3I[] ToPoints()
		{
			Vector3I[] r = null;
			ToPoints( ref r );
			return r;
		}

		public void ToPoints( ref Vector3I[] points )
		{
			if( points.Length < 8 )
				points = new Vector3I[ 8 ];
			for( int i = 0; i < 8; i++ )
			{
				points[ i ].X = this[ ( i ^ ( i >> 1 ) ) & 1 ].X;
				points[ i ].Y = this[ ( i >> 1 ) & 1 ].Y;
				points[ i ].Z = this[ ( i >> 2 ) & 1 ].Z;
			}
		}

		[AutoConvertType]
		public static BoundsI Parse( string text )
		{
			if( string.IsNullOrEmpty( text ) )
				throw new ArgumentNullException( "The text parameter cannot be null or zero length." );

			string[] vals = text.Split( new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries );

			if( vals.Length != 6 )
				throw new FormatException( string.Format( "Cannot parse the text '{0}' because it does not have 6 parts separated by spaces in the form (x y z x y z).", text ) );

			try
			{
				return new BoundsI(
					int.Parse( vals[ 0 ] ),
					int.Parse( vals[ 1 ] ),
					int.Parse( vals[ 2 ] ),
					int.Parse( vals[ 3 ] ),
					int.Parse( vals[ 4 ] ),
					int.Parse( vals[ 5 ] ) );
			}
			catch( Exception )
			{
				throw new FormatException( "The parts of the vectors must be decimal numbers." );
			}
		}

		[AutoConvertType]
		public override string ToString()
		{
			return string.Format( "{0} {1} {2} {3} {4} {5}",
				Minimum.X, Minimum.Y, Minimum.Z,
				Maximum.X, Maximum.Y, Maximum.Z );
		}

		[AutoConvertType]
		public BoundsF ToBoundsF()
		{
			BoundsF result;
			result.Minimum.X = Minimum.X;
			result.Minimum.Y = Minimum.Y;
			result.Minimum.Z = Minimum.Z;
			result.Maximum.X = Maximum.X;
			result.Maximum.Y = Maximum.Y;
			result.Maximum.Z = Maximum.Z;
			return result;
		}

		[AutoConvertType]
		public Bounds ToBounds()
		{
			Bounds result;
			result.Minimum.X = Minimum.X;
			result.Minimum.Y = Minimum.Y;
			result.Minimum.Z = Minimum.Z;
			result.Maximum.X = Maximum.X;
			result.Maximum.Y = Maximum.Y;
			result.Maximum.Z = Maximum.Z;
			return result;
		}

		public static BoundsI Merge( BoundsI a, BoundsI b )
		{
			var v = a;
			v.Add( b );
			return v;
		}
	}
}
