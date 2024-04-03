// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Cone-based collision shape.
	/// </summary>
	public class CollisionShape_Cone : CollisionShape
	{
		/// <summary>
		/// The axis of the cone (0 = X-axis, 1 = Y-axis, 2 = Z-axis).
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
				if( _axis.BeginSet( this, ref value ) )
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
		public event Action<CollisionShape_Cone> AxisChanged;
		ReferenceField<int> _axis = 2;

		/// <summary>
		/// The radius of the cone.
		/// </summary>
		[DefaultValue( 0.5 )]
		public Reference<double> Radius
		{
			get { if( _radius.BeginGet() ) Radius = _radius.Get( this ); return _radius.value; }
			set
			{
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
		public event Action<CollisionShape_Cone> RadiusChanged;
		ReferenceField<double> _radius = 0.5;

		/// <summary>
		/// The height of the cone.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> Height
		{
			get { if( _height.BeginGet() ) Height = _height.Get( this ); return _height.value; }
			set
			{
				if( _height.BeginSet( this, ref value ) )
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
		public event Action<CollisionShape_Cone> HeightChanged;
		ReferenceField<double> _height = 1;

		///////////////////////////////////////////////

		protected internal override void GetShapeKey( StringBuilder key )
		{
			base.GetShapeKey( key );

			key.Append( " con " );
			key.Append( Axis.Value );
			key.Append( ' ' );
			key.Append( (float)Radius.Value );
			key.Append( ' ' );
			key.Append( (float)Height.Value );
		}

		protected internal override void CreateShape( Scene scene, IntPtr nativeShape, ref Vector3F position, ref QuaternionF rotation, ref Vector3F localScaling, ref Scene.PhysicsWorldClass.Shape.CollisionShapeData collisionShapeData )
		{
			unsafe
			{

				//!!!!impl by Jolt


				//convex hull implementation

				var axis = Axis.Value;

				int steps = 32;
				var pointCount = steps + 1;
				var points = stackalloc Vector3F[ pointCount ];

				float halfHeightOfCylinder = (float)Height.Value * 0.5f;
				float radius = (float)Radius;

				if( axis == 0 )
					points[ 0 ] = new Vector3F( halfHeightOfCylinder, 0, 0 );
				else if( axis == 1 )
					points[ 0 ] = new Vector3F( 0, halfHeightOfCylinder, 0 );
				else
					points[ 0 ] = new Vector3F( 0, 0, halfHeightOfCylinder );

				for( int n = 0; n < steps; n++ )
				{
					var angle = (float)n / steps * MathEx.PI * 2;
					var x = MathEx.Cos( angle );
					var y = MathEx.Sin( angle );
					if( axis == 0 )
						points[ 1 + n ] = new Vector3F( -halfHeightOfCylinder, x * radius, y * radius );
					else if( axis == 1 )
						points[ 1 + n ] = new Vector3F( x * radius, -halfHeightOfCylinder, y * radius );
					else
						points[ 1 + n ] = new Vector3F( x * radius, y * radius, -halfHeightOfCylinder );
				}

				for( int n = 0; n < pointCount; n++ )
					points[ n ] *= localScaling;

				var localScaling2 = new Vector3F( 1, 1, 1 );

				var convexRadius = scene.PhysicsAdvancedSettings ? scene.PhysicsDefaultConvexRadius.Value : Scene.physicsDefaultConvexRadiusDefault;

				PhysicsNative.JShape_AddConvexHull( nativeShape, ref position, ref rotation, ref localScaling2, points, pointCount, (float)convexRadius );
			}

			//QuaternionF rotation2 = rotation;
			//switch( Axis.Value )
			//{
			//case 0:
			//	rotation2 = rotation * QuaternionF.FromRotateByZ( MathEx.PI / 2 );
			//	break;
			//case 2:
			//	rotation2 = rotation * QuaternionF.FromRotateByX( MathEx.PI / 2 );
			//	break;
			//}

			//float halfHeightOfCylinder = (float)Height.Value * 0.5f;

			//PhysicsNative.JShape_AddCone( nativeShape, ref position, ref rotation2, halfHeightOfCylinder, (float)Radius );
		}

		protected internal override void Render( Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered )
		{
			Matrix4 t = bodyTransform.ToMatrix4();
			var local = LocalTransform.Value;
			if( !local.IsIdentity )
				t *= local.ToMatrix4();
			int axis = Axis.Value;

			viewport.Simple3DRenderer.AddCone( t, axis, SimpleMeshGenerator.ConeOrigin.Center, Radius, Height.Value, 16, 16, solid );

			if( solid )
				verticesRendered += 16 + 2;
			else
			{
				verticesRendered += 16 * 8;
				verticesRendered += 16 * 8;
			}
		}

		protected override double OnCalculateVolume()
		{
			var r = Radius.Value;
			var h = Height.Value;
			return ( 1.0 / 3.0 ) * Math.PI * r * r * h;
		}

		internal override Vector3 GetCenterOfMassPositionNotScaledByParent()
		{
			var result = Vector3.Zero;
			result[ Axis.Value ] = -Height.Value / 4.0;
			return result;
		}
	}
}
