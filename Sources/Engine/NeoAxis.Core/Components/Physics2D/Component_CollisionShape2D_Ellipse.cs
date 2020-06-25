// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using tainicom.Aether.Physics2D.Dynamics;
using NeoAxis.Editor;
using tainicom.Aether.Physics2D.Common;

namespace NeoAxis
{
	/// <summary>
	/// Ellipse-based 2D collision shape.
	/// </summary>
	[NewObjectDefaultName( "Ellipse Shape" )]
	[AddToResourcesWindow( @"Base\2D\Ellipse Shape 2D", -7996 )]
	public class Component_CollisionShape2D_Ellipse : Component_CollisionShape2D
	{
		/// <summary>
		/// The size of the ellipse.
		/// </summary>
		[DefaultValue( "1 1" )]
		public Reference<Vector2> Dimensions
		{
			get { if( _dimensions.BeginGet() ) Dimensions = _dimensions.Get( this ); return _dimensions.value; }
			set
			{
				if( _dimensions.BeginSet( ref value ) )
				{
					try
					{
						DimensionsChanged?.Invoke( this );
						NeedUpdateCachedArea();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _dimensions.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Dimensions"/> property value changes.</summary>
		public event Action<Component_CollisionShape2D_Ellipse> DimensionsChanged;
		ReferenceField<Vector2> _dimensions = new Vector2( 1, 1 );

		/// <summary>
		/// The number of edges. The more edges, the more it resembles an ellipse.
		/// </summary>
		[DefaultValue( 64 )]
		public Reference<int> Edges
		{
			get { if( _edges.BeginGet() ) Edges = _edges.Get( this ); return _edges.value; }
			set
			{
				if( value < 3 )
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
		public event Action<Component_CollisionShape2D_Ellipse> EdgesChanged;
		ReferenceField<int> _edges = 64;

		/// <summary>
		/// Whether to create smooth circle instead polygon. The mode can works only for circles and when TransformRelativeToParent has default value.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> UseSmoothCircleWhenPossible
		{
			get { if( _useSmoothCircleWhenPossible.BeginGet() ) UseSmoothCircleWhenPossible = _useSmoothCircleWhenPossible.Get( this ); return _useSmoothCircleWhenPossible.value; }
			set
			{
				if( _useSmoothCircleWhenPossible.BeginSet( ref value ) )
				{
					try
					{
						UseSmoothCircleWhenPossibleChanged?.Invoke( this );
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _useSmoothCircleWhenPossible.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="UseSmoothCircleWhenPossible"/> property value changes.</summary>
		public event Action<Component_CollisionShape2D_Ellipse> UseSmoothCircleWhenPossibleChanged;
		ReferenceField<bool> _useSmoothCircleWhenPossible = true;

		/////////////////////////////////////////

		protected internal override IList<Fixture> CreateShape( Body body, Transform shapeTransform, List<Vector2> rigidBodyLocalPoints )
		{
			if( shapeTransform.IsPositionZero && shapeTransform.IsRotationIdentity )
			{
				var radius = Dimensions.Value * shapeTransform.Scale.ToVector2() * 0.5;

				rigidBodyLocalPoints.Add( new Vector2( -radius.X, -radius.Y ) );
				rigidBodyLocalPoints.Add( new Vector2( radius.X, -radius.Y ) );
				rigidBodyLocalPoints.Add( new Vector2( -radius.X, radius.Y ) );
				rigidBodyLocalPoints.Add( new Vector2( radius.X, radius.Y ) );

				if( Math.Abs( radius.X - radius.Y ) < 0.0001 && UseSmoothCircleWhenPossible )
					return new Fixture[] { body.CreateCircle( (float)radius.X, 0, new Microsoft.Xna.Framework.Vector2( 0, 0 ) ) };
				else
					return new Fixture[] { body.CreateEllipse( (float)radius.X, (float)radius.Y, Edges, 0 ) };
			}
			else
			{
				var radius = Dimensions.Value * 0.5;
				int edges = Edges.Value;

				var points = new Vector2[ edges ];
				for( int n = 0; n < edges; n++ )
				{
					float angle = (float)n / (float)edges * MathEx.PI * 2;
					points[ n ] = ( shapeTransform * new Vector3( MathEx.Cos( angle ) * radius.X, MathEx.Sin( angle ) * radius.Y, 0 ) ).ToVector2();
				}

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
		}

		protected internal override void Render( Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered )
		{
			Matrix4 t = bodyTransform.ToMatrix4();
			var local = TransformRelativeToParent.Value;
			if( !local.IsIdentity )
				t *= local.ToMatrix4();
			var d = Dimensions.Value;

			int edges = Edges;

			viewport.Simple3DRenderer.AddEllipse( ref d, edges, ref t, solid );
			verticesRendered += solid ? ( edges + 1 ) : ( edges * 2 );
		}

		protected override double OnCalculateArea()
		{
			var scale = TransformRelativeToParent.Value.Scale.ToVector2();
			var d = Dimensions.Value * scale;
			return Math.PI * ( d.X * 0.5 ) * ( d.Y * 0.5 );
		}
	}
}
