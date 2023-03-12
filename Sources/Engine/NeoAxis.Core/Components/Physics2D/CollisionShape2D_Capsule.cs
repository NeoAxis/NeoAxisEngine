// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using Internal.tainicom.Aether.Physics2D.Dynamics;
using Internal.tainicom.Aether.Physics2D.Common;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// Capsule-based 2D collision shape.
	/// </summary>
	[NewObjectDefaultName( "Capsule Shape" )]
	[AddToResourcesWindow( @"Base\2D\Capsule Shape 2D", -7995.5 )]
	public class CollisionShape2D_Capsule : CollisionShape2D
	{
		/// <summary>
		/// The axis of the capsule (0 = X-axis, 1 = Y-axis).
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 0, 1 )]
		public Reference<int> Axis
		{
			get { if( _axis.BeginGet() ) Axis = _axis.Get( this ); return _axis.value; }
			set
			{
				if( value < 0 )
					value = new Reference<int>( 0, value.GetByReference );
				if( value > 1 )
					value = new Reference<int>( 1, value.GetByReference );
				if( _axis.BeginSet( ref value ) )
				{
					try
					{
						AxisChanged?.Invoke( this );
						NeedUpdateCachedArea();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _axis.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Axis"/> property value changes.</summary>
		public event Action<CollisionShape2D_Capsule> AxisChanged;
		ReferenceField<int> _axis = 1;

		/// <summary>
		/// The radius of the capsule.
		/// </summary>
		[DefaultValue( 0.5 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set
			{
				if( _radius.BeginSet( ref value ) )
				{
					try
					{
						RadiusChanged?.Invoke( this );
						NeedUpdateCachedArea();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _radius.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<CollisionShape2D_Capsule> RadiusChanged;
		ReferenceField<double> _radius = 0.5;

		/// <summary>
		/// The height of the capsule.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> Height
		{
			get { if( _height.BeginGet() ) Height = _height.Get( this ); return _height.value; }
			set
			{
				if( _height.BeginSet( ref value ) )
				{
					try
					{
						HeightChanged?.Invoke( this );
						NeedUpdateCachedArea();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _height.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<CollisionShape2D_Capsule> HeightChanged;
		ReferenceField<double> _height = 1;

		/// <summary>
		/// The number of edges. The more edges, the more it resembles a capsule.
		/// </summary>
		[DefaultValue( 64 )]
		public Reference<int> Edges
		{
			get { if( _edges.BeginGet() ) Edges = _edges.Get( this ); return _edges.value; }
			set
			{
				if( value < 4 )
					value = new Reference<int>( 3, value.GetByReference );
				if( _edges.BeginSet( ref value ) )
				{
					try
					{
						EdgesChanged?.Invoke( this );
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _edges.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Edges"/> property value changes.</summary>
		public event Action<CollisionShape2D_Capsule> EdgesChanged;
		ReferenceField<int> _edges = 64;

		/////////////////////////////////////////

		Vector2[] GenerateConvex()
		{
			var radius = Radius.Value;
			var height = Height.Value;
			var pointsSide = Math.Max( ( Edges - 2 ) / 2, 1 ) + 1;

			var result = new Vector2[ pointsSide * 2 ];
			for( int n = 0; n < pointsSide; n++ )
			{
				double factor = (double)n / (double)( pointsSide - 1 );
				var angle = factor * Math.PI;

				if( Axis.Value == 0 )
				{
					result[ n ] = new Vector2( height / 2, 0 ) + new Vector2( MathEx.Sin( angle ), MathEx.Cos( angle ) ) * radius;
					result[ pointsSide + n ] = new Vector2( -height / 2, 0 ) - new Vector2( MathEx.Sin( angle ), MathEx.Cos( angle ) ) * radius;
				}
				else
				{
					result[ n ] = new Vector2( 0, height / 2 ) + new Vector2( MathEx.Cos( angle ), MathEx.Sin( angle ) ) * radius;
					result[ pointsSide + n ] = new Vector2( 0, -height / 2 ) - new Vector2( MathEx.Cos( angle ), MathEx.Sin( angle ) ) * radius;
				}
			}
			return result;
		}

		protected internal override IList<Fixture> CreateShape( Body body, Transform shapeTransform, List<Vector2> rigidBodyLocalPoints )
		{
			var convex = GenerateConvex();

			var points = new Vector2[ convex.Length ];
			for( int n = 0; n < points.Length; n++ )
				points[ n ] = ( shapeTransform * new Vector3( convex[ n ], 0 ) ).ToVector2();

			var r = Rectangle.Cleared;
			foreach( var p in points )
				r.Add( p );
			rigidBodyLocalPoints.Add( r.LeftTop );
			rigidBodyLocalPoints.Add( r.RightTop );
			rigidBodyLocalPoints.Add( r.LeftBottom );
			rigidBodyLocalPoints.Add( r.RightBottom );

			var vertices = new Vertices( points.Length );
			foreach( var p in points )
				vertices.Add( Physics2DUtility.Convert( p ) );

			return new Fixture[] { body.CreatePolygon( vertices, 0 ) };
		}

		protected internal override void Render( Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered )
		{
			Matrix4 t = bodyTransform.ToMatrix4();
			var local = TransformRelativeToParent.Value;
			if( !local.IsIdentity )
				t *= local.ToMatrix4();

			var convex = GenerateConvex();

			var points = new Vector3[ convex.Length ];
			for( int n = 0; n < convex.Length; n++ )
				points[ n ] = t * new Vector3( convex[ n ], 0 );

			if( solid )
			{
				var indices = new int[ ( points.Length - 1 ) * 3 ];
				for( int n = 1; n < points.Length - 1; n++ )
				{
					indices[ n * 3 + 0 ] = 0;
					indices[ n * 3 + 1 ] = n;
					indices[ n * 3 + 2 ] = n + 1;
				}
				viewport.Simple3DRenderer.AddTriangles( points, indices, false, false );
				verticesRendered += points.Length;
			}
			else
			{
				for( int n = 0; n < convex.Length; n++ )
				{
					var n2 = ( n + 1 ) % convex.Length;
					viewport.Simple3DRenderer.AddLine( points[ n ], points[ n2 ] );
				}
				verticesRendered += convex.Length * 2;
			}
		}

		protected override double OnCalculateArea()
		{
			var scale = TransformRelativeToParent.Value.Scale.ToVector2();
			return ( Height * Radius * 2 + Math.PI * Radius * Radius ) * scale.X * scale.Y;
		}
	}
}
