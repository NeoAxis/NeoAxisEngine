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
	/// Capsule-based collision shape.
	/// </summary>
	public class Component_CollisionShape_Capsule : Component_CollisionShape
	{
		/// <summary>
		/// The axis of the capsule (0 = X-axis, 1 = Y-axis, 2 = Z-axis).
		/// </summary>
		[DefaultValue( 2 )]
		[Range( 0, 2 )]
		public Reference<int> Axis
		{
			get { if( _axis.BeginGet() ) Axis = _axis.Get( this ); return _axis.value; }
			set
			{
				if( value < 0 )
					value = new Reference<int>( 0, value.GetByReference );
				if( value > 2 )
					value = new Reference<int>( 2, value.GetByReference );
				if( _axis.BeginSet( ref value ) )
				{
					try
					{
						AxisChanged?.Invoke( this );
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _axis.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Axis"/> property value changes.</summary>
		public event Action<Component_CollisionShape_Capsule> AxisChanged;
		ReferenceField<int> _axis = 2;

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
						NeedUpdateCachedVolume();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _radius.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Radius"/> property value changes.</summary>
		public event Action<Component_CollisionShape_Capsule> RadiusChanged;
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
						NeedUpdateCachedVolume();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _height.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Height"/> property value changes.</summary>
		public event Action<Component_CollisionShape_Capsule> HeightChanged;
		ReferenceField<double> _height = 1;

		/////////////////////////////////////////

		protected internal override CollisionShape CreateShape()
		{
			switch( Axis.Value )
			{
			case 1: return new BulletSharp.CapsuleShape( BulletPhysicsUtility.Convert( Radius ), BulletPhysicsUtility.Convert( Height ) );
			case 2: return new BulletSharp.CapsuleShapeZ( BulletPhysicsUtility.Convert( Radius ), BulletPhysicsUtility.Convert( Height ) );
			default: return new BulletSharp.CapsuleShapeX( BulletPhysicsUtility.Convert( Radius ), BulletPhysicsUtility.Convert( Height ) );
			}
		}

		protected internal override void Render( Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered )
		{
			Matrix4 t = bodyTransform.ToMatrix4();
			var local = TransformRelativeToParent.Value;
			if( !local.IsIdentity )
				t *= local.ToMatrix4();
			int axis = Axis.Value;

			viewport.Simple3DRenderer.AddCapsule( t, axis, Radius.Value, Height.Value, 16, solid );

			if( solid )
			{
				int segments = 16;
				int hSegments = segments;
				int vSegments = segments / 2 * 2 + 1;
				verticesRendered += hSegments * ( vSegments - 1 ) + 2;
			}
			else
			{
				verticesRendered += 16 * 8;
				verticesRendered += 16 * 2 * 3 * 8;
			}
		}

		protected override double OnCalculateVolume()
		{
			var r = Radius.Value;
			var h = Height.Value;
			return Math.PI * r * r * h + ( 4.0 / 3.0 ) * Math.PI * r * r * r;
		}
	}
}
