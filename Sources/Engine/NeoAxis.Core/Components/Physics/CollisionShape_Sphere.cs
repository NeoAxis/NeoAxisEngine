// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Sphere-based collision shape.
	/// </summary>
	public class CollisionShape_Sphere : CollisionShape
	{
		/// <summary>
		/// The radius of the sphere.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set
			{
				//!!!!check value. везде в шейпах

				if( _radius.BeginSet( this, ref value ) )
				{
					try
					{
						RadiusChanged?.Invoke( this );
						NeedUpdateCachedVolume();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _radius.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<CollisionShape_Sphere> RadiusChanged;
		ReferenceField<double> _radius = 0.5;

		///////////////////////////////////////////////

		protected internal override void GetShapeKey( StringBuilder key )
		{
			base.GetShapeKey( key );

			key.Append( " sph " );
			key.Append( (float)Radius.Value );
		}

		protected internal override void CreateShape( Scene scene, IntPtr nativeShape, ref Vector3F position, ref QuaternionF rotation, ref Vector3F localScaling, ref Scene.PhysicsWorldClass.Shape.CollisionShapeData collisionShapeData )
		{
			var scaling = localScaling.MaxComponent();
			PhysicsNative.JShape_AddSphere( nativeShape, ref position, ref rotation, (float)Radius * scaling );
		}

		protected internal override void Render( Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered )
		{
			Matrix4 t = bodyTransform.ToMatrix4();
			var local = LocalTransform.Value;
			if( !local.IsIdentity )
				t *= local.ToMatrix4();

			viewport.Simple3DRenderer.AddSphere( t, Radius.Value, 16, solid );

			if( solid )
			{
				int segments = 16;
				int hSegments = segments;
				int vSegments = ( segments + 1 ) / 2 * 2;
				verticesRendered += hSegments * ( vSegments - 1 ) + 2;
			}
			else
				verticesRendered += 16 * 3 * 8;
		}

		protected override double OnCalculateVolume()
		{
			var r = Radius.Value;
			return ( 4.0 / 3.0 ) * MathEx.PI * r * r * r;
		}
	}
}
