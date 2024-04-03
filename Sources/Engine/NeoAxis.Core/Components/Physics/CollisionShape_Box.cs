// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Box-based collision shape.
	/// </summary>
	public class CollisionShape_Box : CollisionShape
	{
		/// <summary>
		/// The size of the box.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		public Reference<Vector3> Dimensions
		{
			get { if( _dimensions.BeginGet() ) Dimensions = _dimensions.Get( this ); return _dimensions.value; }
			set
			{
				if( _dimensions.BeginSet( this, ref value ) )
				{
					try
					{
						DimensionsChanged?.Invoke( this );
						NeedUpdateCachedVolume();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _dimensions.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Dimensions"/> property value changes.</summary>
		public event Action<CollisionShape_Box> DimensionsChanged;
		ReferenceField<Vector3> _dimensions = new Vector3( 1, 1, 1 );

		///////////////////////////////////////////////

		protected internal override void GetShapeKey( StringBuilder key )
		{
			base.GetShapeKey( key );

			var dimensions = Dimensions.Value.ToVector3F();
			key.Append( " box " );
			key.Append( dimensions.X );
			key.Append( ' ' );
			key.Append( dimensions.Y );
			key.Append( ' ' );
			key.Append( dimensions.Z );
		}

		protected internal override void CreateShape( Scene scene, IntPtr nativeShape, ref Vector3F position, ref QuaternionF rotation, ref Vector3F localScaling, ref Scene.PhysicsWorldClass.Shape.CollisionShapeData collisionShapeData )
		{
			var dimensions = Dimensions.Value.ToVector3F() * localScaling;
			var convexRadius = scene.PhysicsAdvancedSettings ? scene.PhysicsDefaultConvexRadius.Value : Scene.physicsDefaultConvexRadiusDefault;
			PhysicsNative.JShape_AddBox( nativeShape, ref position, ref rotation, ref dimensions, (float)convexRadius );
		}

		protected internal override void Render( Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered )
		{
			Matrix4 t = bodyTransform.ToMatrix4();
			var local = LocalTransform.Value;
			if( !local.IsIdentity )
				t *= local.ToMatrix4();
			var d = Dimensions.Value;
			viewport.Simple3DRenderer.AddBox( new Box( new Bounds( -d * .5, d * .5 ) ) * t, solid );
			verticesRendered += solid ? 8 : 96;
		}

		protected override double OnCalculateVolume()
		{
			var d = Dimensions.Value;
			return d.X * d.Y * d.Z;
		}
	}
}
