// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using BulletSharp;

namespace NeoAxis
{
	/// <summary>
	/// Sphere-based collision shape.
	/// </summary>
	public class Component_CollisionShape_Sphere : Component_CollisionShape
	{
		//Radius
		ReferenceField<double> _radius = 0.5;
		/// <summary>
		/// The radius of the sphere.
		/// </summary>
		[Serialize]
		[DefaultValue( 0.5 )]
		//!!!!крутилки. везде в шейпах
		[Range( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> Radius
		{
			get
			{
				if( _radius.BeginGet() )
					Radius = _radius.Get( this );
				return _radius.value;
			}
			set
			{
				//!!!!check value. везде в шейпах

				if( _radius.BeginSet( ref value ) )
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
		public event Action<Component_CollisionShape_Sphere> RadiusChanged;


		protected internal override CollisionShape CreateShape()
		{
			return new BulletSharp.SphereShape( BulletPhysicsUtility.Convert( Radius ) );
		}

		protected internal override void Render( Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered )
		{
			Matrix4 t = bodyTransform.ToMatrix4();
			var local = TransformRelativeToParent.Value;
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
