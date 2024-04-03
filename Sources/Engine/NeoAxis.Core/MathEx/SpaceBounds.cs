// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Represents a class to contain axis aligned bounds box and bounding sphere.
	/// </summary>
	public class SpaceBounds
	{
		bool boundingBoxOriginal;
		bool boundingSphereOriginal;
		internal Bounds boundingBox;
		internal Sphere boundingSphere;

		//

		public SpaceBounds()
		{
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SpaceBounds( Bounds boundingBox, Sphere boundingSphere )
		{
			this.boundingBox = boundingBox;
			boundingBoxOriginal = true;
			this.boundingSphere = boundingSphere;
			boundingSphereOriginal = true;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SpaceBounds( Bounds boundingBox )
		{
			this.boundingBox = boundingBox;
			boundingBoxOriginal = true;
			boundingBox.GetBoundingSphere( out boundingSphere );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SpaceBounds( ref Bounds boundingBox )
		{
			this.boundingBox = boundingBox;
			boundingBoxOriginal = true;
			boundingBox.GetBoundingSphere( out boundingSphere );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SpaceBounds( Sphere boundingSphere )
		{
			this.boundingSphere = boundingSphere;
			boundingSphereOriginal = true;
			boundingSphere.ToBounds( out boundingBox );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public SpaceBounds( ref Sphere boundingSphere )
		{
			this.boundingSphere = boundingSphere;
			boundingSphereOriginal = true;
			boundingSphere.ToBounds( out boundingBox );
		}

		public bool BoundingBoxOriginal { get { return boundingBoxOriginal; } }
		public bool BoundingSphereOriginal { get { return boundingSphereOriginal; } }

		public Bounds BoundingBox
		{
			get { return boundingBox; }
		}

		public Sphere BoundingSphere
		{
			get { return boundingSphere; }
		}


		//public Bounds CalculatedBoundingBox
		//{
		//	get { return boundingBox; }
		//}

		//public Sphere CalculatedBoundingSphere
		//{
		//	get { return boundingSphere; }
		//}

		//public double CalculatedBoundingSphereRadius
		//{
		//	get { return boundingSphere.Radius; }
		//}


		//public Bounds? BoundingBox
		//{
		//	[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//	get { return boundingBox; }
		//}

		//public Sphere? BoundingSphere
		//{
		//	[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//	get { return boundingSphere; }
		//}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//public void GetCalculatedBoundingBox( out Bounds result )
		//{
		//	if( !calculatedBoundingBox_Bool )
		//	{
		//		if( boundingBox.HasValue )
		//			calculatedBoundingBox = boundingBox.Value;
		//		else
		//			boundingSphere.Value.ToBounds( out calculatedBoundingBox );
		//		calculatedBoundingBox_Bool = true;
		//	}
		//	result = calculatedBoundingBox;
		//}

		//public Bounds CalculatedBoundingBox
		//{
		//	[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//	get
		//	{
		//		GetCalculatedBoundingBox( out var result );
		//		return result;
		//	}
		//}

		////public double CalculatedBoundingBoxMaxSide
		////{
		////	get
		////	{
		////		if( !calculatedBoundingBox_Bool )
		////		{
		////			if( boundingBox.HasValue )
		////				calculatedBoundingBox = boundingBox.Value;
		////			else
		////				calculatedBoundingBox = boundingSphere.Value.ToBounds();
		////			calculatedBoundingBox_Bool = true;
		////		}
		////		//!!!!can be cached
		////		calculatedBoundingBox.GetSize( out var size );
		////		return size.MaxComponent();
		////	}
		////}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//void TouchCalculatedBoundingSphere()
		//{
		//	if( !calculatedBoundingSphere_Bool )
		//	{
		//		if( boundingSphere.HasValue )
		//			calculatedBoundingSphere = boundingSphere.Value;
		//		else
		//			boundingBox.Value.GetBoundingSphere( out calculatedBoundingSphere );
		//		calculatedBoundingSphere_Bool = true;
		//	}
		//}

		//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//public void GetCalculatedBoundingSphere( out Sphere result )
		//{
		//	TouchCalculatedBoundingSphere();
		//	result = calculatedBoundingSphere;
		//}

		//public Sphere CalculatedBoundingSphere
		//{
		//	[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//	get
		//	{
		//		TouchCalculatedBoundingSphere();
		//		return calculatedBoundingSphere;
		//		//GetCalculatedBoundingSphere( out var result );
		//		//return result;
		//	}
		//}

		//public double CalculatedBoundingSphereRadius
		//{
		//	[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		//	get
		//	{
		//		TouchCalculatedBoundingSphere();
		//		return calculatedBoundingSphere.Radius;
		//	}
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override bool Equals( object obj )
		{
			return ( obj is SpaceBounds && this == (SpaceBounds)obj );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public override int GetHashCode()
		{
			return boundingBox.GetHashCode() ^ boundingSphere.GetHashCode() ^ boundingBoxOriginal.GetHashCode() ^ boundingSphereOriginal.GetHashCode();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator ==( SpaceBounds a, SpaceBounds b )
		{
			bool aNull = ReferenceEquals( a, null );
			bool bNull = ReferenceEquals( b, null );
			if( aNull || bNull )
				return aNull && bNull;
			if( ReferenceEquals( a, b ) )
				return true;

			return a.boundingBox == b.boundingBox && a.boundingSphere == b.boundingSphere && a.boundingBoxOriginal == b.boundingBoxOriginal && a.boundingSphereOriginal == b.boundingSphereOriginal;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static bool operator !=( SpaceBounds a, SpaceBounds b )
		{
			bool aNull = ReferenceEquals( a, null );
			bool bNull = ReferenceEquals( b, null );
			if( aNull || bNull )
				return !( aNull && bNull );
			if( ReferenceEquals( a, b ) )
				return false;

			return a.boundingBox != b.boundingBox || a.boundingSphere != b.boundingSphere || a.boundingBoxOriginal != b.boundingBoxOriginal || a.boundingSphereOriginal != b.boundingSphereOriginal;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public static SpaceBounds Merge( SpaceBounds a, SpaceBounds b )
		{
			if( a == null )
				return b;
			if( b == null )
				return a;

			Bounds.Merge( ref a.boundingBox, ref b.boundingBox, out var bounds );
			Sphere.Merge( ref a.boundingSphere, ref b.boundingSphere, out var sphere );
			return new SpaceBounds( bounds, sphere );

			//Bounds bounds;
			//if( a.boundingBoxOriginal && b.boundingBoxOriginal )
			//	Bounds.Merge( ref a.boundingBox, ref b.boundingBox, out bounds );
			//else if( a.boundingBoxOriginal )
			//	bounds = a.boundingBox;
			//else //if( b.boundingBoxOriginal )
			//	bounds = b.boundingBox;

			//Sphere sphere;
			//if( a.boundingSphereOriginal && b.boundingSphereOriginal )
			//	Sphere.Merge( ref a.boundingSphere, ref b.boundingSphere, out sphere );
			//else if( a.boundingSphereOriginal )
			//	sphere = a.boundingSphere;
			//else //if( b.boundingSphereOriginal )
			//	sphere = b.boundingSphere;

			//return new SpaceBounds( bounds, sphere );


			//Bounds? bounds = null;
			//if( a.boundingBox != null && b.boundingBox != null )
			//	bounds = Bounds.Merge( a.boundingBox.Value, b.boundingBox.Value );
			//else if( a.boundingBox != null )
			//	bounds = a.boundingBox.Value;
			//else if( b.boundingBox != null )
			//	bounds = b.boundingBox.Value;

			//Sphere? sphere = null;
			//if( a.boundingSphere != null && b.boundingSphere != null )
			//	sphere = Sphere.Merge( a.boundingSphere.Value, b.boundingSphere.Value );
			//else if( a.boundingSphere != null )
			//	sphere = a.boundingSphere.Value;
			//else if( b.boundingSphere != null )
			//	sphere = b.boundingSphere.Value;

			//return new SpaceBounds( bounds, sphere );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static SpaceBounds Multiply( Transform transform, SpaceBounds spaceBounds )
		{
			if( spaceBounds.boundingBoxOriginal )
			{
				if( spaceBounds.boundingSphereOriginal )
					return new SpaceBounds( transform * spaceBounds.boundingBox, transform * spaceBounds.boundingSphere );
				else
					return new SpaceBounds( transform * spaceBounds.boundingBox );
			}
			else
				return new SpaceBounds( transform * spaceBounds.boundingSphere );


			//Bounds? b = null;
			//Sphere? s = null;

			////!!!!!slowly
			////!!!!а может еще как-то смешивать сферу и bounds
			////!!!еще в конце расчета может еще как-то оптимизировать/уменьшить

			//if( spaceBounds.boundingBox != null )
			//	b = transform * spaceBounds.boundingBox.Value;
			//if( spaceBounds.boundingSphere != null )
			//	s = transform * spaceBounds.boundingSphere.Value;

			//return new SpaceBounds( b, s );
		}
	}
}
