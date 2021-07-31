// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	/// Box-based collision shape.
	/// </summary>
	public class Component_CollisionShape_Box : Component_CollisionShape
	{
		/// <summary>
		/// The size of the box.
		/// </summary>
		[Serialize]
		[DefaultValue( "1 1 1" )]
		public Reference<Vector3> Dimensions
		{
			get { if( _dimensions.BeginGet() ) Dimensions = _dimensions.Get( this ); return _dimensions.value; }
			set
			{
				if( _dimensions.BeginSet( ref value ) )
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
		public event Action<Component_CollisionShape_Box> DimensionsChanged;
		ReferenceField<Vector3> _dimensions = new Vector3( 1, 1, 1 );


		protected internal override CollisionShape CreateShape()
		{
			return new BulletSharp.BoxShape( BulletPhysicsUtility.Convert( Dimensions.Value / 2 ) );
		}

		protected internal override void Render( Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered )
		{
			Matrix4 t = bodyTransform.ToMatrix4();
			var local = TransformRelativeToParent.Value;
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
