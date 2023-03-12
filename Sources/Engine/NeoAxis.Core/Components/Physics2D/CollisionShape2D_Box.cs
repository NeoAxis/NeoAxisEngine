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
using NeoAxis.Editor;
using Internal.tainicom.Aether.Physics2D.Common;

namespace NeoAxis
{
	/// <summary>
	/// Box-based 2D collision shape.
	/// </summary>
	[NewObjectDefaultName( "Box Shape" )]
	[AddToResourcesWindow( @"Base\2D\Box Shape 2D", -7997 )]
	public class CollisionShape2D_Box : CollisionShape2D
	{
		/// <summary>
		/// The size of the box.
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
		public event Action<CollisionShape2D_Box> DimensionsChanged;
		ReferenceField<Vector2> _dimensions = new Vector2( 1, 1 );

		/////////////////////////////////////////

		protected internal override IList<Fixture> CreateShape( Body body, Transform shapeTransform, List<Vector2> rigidBodyLocalPoints )
		{
			if( shapeTransform.IsPositionZero && shapeTransform.IsRotationIdentity )
			{
				var size = Dimensions.Value * shapeTransform.Scale.ToVector2();
				var sizeHalf = size * 0.5;

				rigidBodyLocalPoints.Add( new Vector2( -sizeHalf.X, -sizeHalf.Y ) );
				rigidBodyLocalPoints.Add( new Vector2( sizeHalf.X, -sizeHalf.Y ) );
				rigidBodyLocalPoints.Add( new Vector2( -sizeHalf.X, sizeHalf.Y ) );
				rigidBodyLocalPoints.Add( new Vector2( sizeHalf.X, sizeHalf.Y ) );

				return new Fixture[] { body.CreateRectangle( (float)size.X, (float)size.Y, 0, Physics2DUtility.Convert( Vector2.Zero ) ) };
			}
			else
			{
				var halfDimensions = Dimensions.Value * 0.5;

				var points = new Vector2[ 4 ];
				points[ 0 ] = ( shapeTransform * new Vector3( -halfDimensions.X, -halfDimensions.Y, 0 ) ).ToVector2();
				points[ 1 ] = ( shapeTransform * new Vector3( halfDimensions.X, -halfDimensions.Y, 0 ) ).ToVector2();
				points[ 2 ] = ( shapeTransform * new Vector3( halfDimensions.X, halfDimensions.Y, 0 ) ).ToVector2();
				points[ 3 ] = ( shapeTransform * new Vector3( -halfDimensions.X, halfDimensions.Y, 0 ) ).ToVector2();

				foreach( var p in points )
					rigidBodyLocalPoints.Add( p );

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

			var rectangle = new Rectangle( -d * 0.5, d * 0.5 );

			viewport.Simple3DRenderer.AddRectangle( ref rectangle, ref t, solid );
			verticesRendered += solid ? 4 : 8;
		}

		protected override double OnCalculateArea()
		{
			var scale = TransformRelativeToParent.Value.Scale.ToVector2();
			var d = Dimensions.Value * scale;
			return d.X * d.Y;
		}
	}
}
