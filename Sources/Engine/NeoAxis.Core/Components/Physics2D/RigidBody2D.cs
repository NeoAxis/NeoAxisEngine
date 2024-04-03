// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Internal.tainicom.Aether.Physics2D.Dynamics;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// 2D rigid physical body.
	/// </summary>
	[NewObjectDefaultName( "Rigid Body 2D" )]
	[AddToResourcesWindow( @"Base\2D\Rigid Body 2D", -7998 )]
	public class RigidBody2D : PhysicalBody2D
	{
		Body rigidBody;
		Vector3 rigidBodyCreatedTransformScale;
		List<Vector2> rigidBodyLocalPoints = new List<Vector2>();

		bool duringCreateDestroy;
		bool updatePropertiesWithoutUpdatingBody;

		class CenterOfMassGeometry
		{
			public float radius;
			public Vector3F[] positions;
			public int[] indices;
		}
		static List<CenterOfMassGeometry> centerOfMassGeometryCache = new List<CenterOfMassGeometry>();

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public enum MotionTypeEnum
		{
			Static,
			Dynamic,
			Kinematic,
		}

		/// <summary>
		/// The type of motion used.
		/// </summary>
		[DefaultValue( MotionTypeEnum.Static )]
		[Category( "Rigid Body 2D" )]
		public Reference<MotionTypeEnum> MotionType
		{
			get { if( _motionType.BeginGet() ) MotionType = _motionType.Get( this ); return _motionType.value; }
			set
			{
				if( _motionType.BeginSet( this, ref value ) )
				{
					try
					{
						MotionTypeChanged?.Invoke( this );
						if( rigidBody != null )
							RecreateBody();
					}
					finally { _motionType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MotionType"/> property value changes.</summary>
		public event Action<RigidBody2D> MotionTypeChanged;
		ReferenceField<MotionTypeEnum> _motionType = MotionTypeEnum.Static;// Dynamic;

		/// <summary>
		/// The mass of the rigid body.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Rigid Body 2D" )]
		public Reference<double> Mass
		{
			get { if( _mass.BeginGet() ) Mass = _mass.Get( this ); return _mass.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _mass.BeginSet( this, ref value ) )
				{
					try
					{
						MassChanged?.Invoke( this );
						if( rigidBody != null )
							UpdateMassAndInertia( rigidBody );
					}
					finally { _mass.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mass"/> property value changes.</summary>
		public event Action<RigidBody2D> MassChanged;
		ReferenceField<double> _mass = 1;

		/// <summary>
		/// Gets or sets the rotational inertia of the body about the local origin.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Rigid Body 2D" )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> Inertia
		{
			get { if( _inertia.BeginGet() ) Inertia = _inertia.Get( this ); return _inertia.value; }
			set
			{
				if( _inertia.BeginSet( this, ref value ) )
				{
					try
					{
						InertiaChanged?.Invoke( this );
						if( rigidBody != null )
							UpdateMassAndInertia( rigidBody );
					}
					finally { _inertia.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Inertia"/> property value changes.</summary>
		public event Action<RigidBody2D> InertiaChanged;
		ReferenceField<double> _inertia = 1.0;

		/// <summary>
		/// Gets or sets the local position of the center of mass.
		/// </summary>
		[DefaultValue( "0 0" )]
		[Category( "Rigid Body 2D" )]
		public Reference<Vector2> LocalCenter
		{
			get { if( _localCenter.BeginGet() ) LocalCenter = _localCenter.Get( this ); return _localCenter.value; }
			set
			{
				if( _localCenter.BeginSet( this, ref value ) )
				{
					try
					{
						LocalCenterChanged?.Invoke( this );
						if( rigidBody != null )
							rigidBody.LocalCenter = Physics2DUtility.Convert( _localCenter.value );
					}
					finally { _localCenter.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LocalCenter"/> property value changes.</summary>
		public event Action<RigidBody2D> LocalCenterChanged;
		ReferenceField<Vector2> _localCenter;

		/// <summary>
		/// Whether the rigid body is affected by the gravity.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Rigid Body 2D" )]
		public Reference<bool> EnableGravity
		{
			get { if( _enableGravity.BeginGet() ) EnableGravity = _enableGravity.Get( this ); return _enableGravity.value; }
			set
			{
				if( _enableGravity.BeginSet( this, ref value ) )
				{
					try
					{
						EnableGravityChanged?.Invoke( this );
						if( rigidBody != null )
							rigidBody.IgnoreGravity = !EnableGravity;
					}
					finally { _enableGravity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="EnableGravity"/> property value changes.</summary>
		public event Action<RigidBody2D> EnableGravityChanged;
		ReferenceField<bool> _enableGravity = true;

		/// <summary>
		/// The linear reduction of velocity over time.
		/// </summary>
		[DefaultValue( 0.05 )]//1 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Rigid Body 2D" )]
		[NetworkSynchronize( false )]
		public Reference<double> LinearDamping
		{
			get { if( _linearDamping.BeginGet() ) LinearDamping = _linearDamping.Get( this ); return _linearDamping.value; }
			set
			{
				if( _linearDamping.BeginSet( this, ref value ) )
				{
					try
					{
						LinearDampingChanged?.Invoke( this );
						if( rigidBody != null )
							rigidBody.LinearDamping = (float)_linearDamping.value;
					}
					finally { _linearDamping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearDamping"/> property value changes.</summary>
		public event Action<RigidBody2D> LinearDampingChanged;
		ReferenceField<double> _linearDamping = 0.05;//0.1;

		/// <summary>
		/// The angular reduction of velocity over time.
		/// </summary>
		[DefaultValue( 0.05 )]//1 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		[Category( "Rigid Body 2D" )]
		[NetworkSynchronize( false )]
		public Reference<double> AngularDamping
		{
			get { if( _angularDamping.BeginGet() ) AngularDamping = _angularDamping.Get( this ); return _angularDamping.value; }
			set
			{
				if( _angularDamping.BeginSet( this, ref value ) )
				{
					try
					{
						AngularDampingChanged?.Invoke( this );
						if( rigidBody != null )
							rigidBody.AngularDamping = (float)_angularDamping.value;
					}
					finally { _angularDamping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularDamping"/> property value changes.</summary>
		public event Action<RigidBody2D> AngularDampingChanged;
		ReferenceField<double> _angularDamping = 0.05;//0.1;

		/// <summary>
		/// Whether the body to have fixed rotation.
		/// </summary>
		/// <value><c>true</c> if it has fixed rotation; otherwise, <c>false</c>.</value>
		[DefaultValue( false )]
		[Category( "Rigid Body 2D" )]
		public Reference<bool> FixedRotation
		{
			get { if( _fixedRotation.BeginGet() ) FixedRotation = _fixedRotation.Get( this ); return _fixedRotation.value; }
			set
			{
				if( _fixedRotation.BeginSet( this, ref value ) )
				{
					try
					{
						FixedRotationChanged?.Invoke( this );
						if( rigidBody != null )
							rigidBody.FixedRotation = _fixedRotation.value;
					}
					finally { _fixedRotation.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="FixedRotation"/> property value changes.</summary>
		public event Action<RigidBody2D> FixedRotationChanged;
		ReferenceField<bool> _fixedRotation = false;

		/// <summary>
		/// Allows sleep the body.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Rigid Body 2D" )]
		public Reference<bool> AllowSleep
		{
			get { if( _allowSleep.BeginGet() ) AllowSleep = _allowSleep.Get( this ); return _allowSleep.value; }
			set
			{
				if( _allowSleep.BeginSet( this, ref value ) )
				{
					try
					{
						AllowSleepChanged?.Invoke( this );
						if( rigidBody != null )
							rigidBody.SleepingAllowed = AllowSleep;
					}
					finally { _allowSleep.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AllowSleep"/> property value changes.</summary>
		public event Action<RigidBody2D> AllowSleepChanged;
		ReferenceField<bool> _allowSleep = true;

		/// <summary>
		/// Gets or sets a value indicating whether this body should be included in the CCD solver.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Rigid Body 2D" )]
		[DisplayName( "CCD" )]
		public Reference<bool> CCD
		{
			get { if( _ccd.BeginGet() ) CCD = _ccd.Get( this ); return _ccd.value; }
			set
			{
				if( _ccd.BeginSet( this, ref value ) )
				{
					try
					{
						CcdChanged?.Invoke( this );
						if( rigidBody != null )
							rigidBody.IsBullet = CCD;
					}
					finally { _ccd.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CCD"/> property value changes.</summary>
		public event Action<RigidBody2D> CcdChanged;
		ReferenceField<bool> _ccd = false;

		/// <summary>
		/// The initial linear velocity of the body.
		/// </summary>
		[DefaultValue( "0 0" )]
		[Category( "Velocity" )]
		[NetworkSynchronize( false )]
		public Reference<Vector2> LinearVelocity
		{
			get { if( _linearVelocity.BeginGet() ) LinearVelocity = _linearVelocity.Get( this ); return _linearVelocity.value; }
			set
			{
				if( _linearVelocity.BeginSet( this, ref value ) )
				{
					try
					{
						LinearVelocityChanged?.Invoke( this );

						if( rigidBody != null && !updatePropertiesWithoutUpdatingBody )
							rigidBody.LinearVelocity = Physics2DUtility.Convert( value );
					}
					finally { _linearVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearVelocity"/> property value changes.</summary>
		public event Action<RigidBody2D> LinearVelocityChanged;
		ReferenceField<Vector2> _linearVelocity = Vector2.Zero;

		/// <summary>
		/// The initial angular velocity of the body.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Category( "Velocity" )]
		[Range( -360, 360 )]
		[NetworkSynchronize( false )]
		public Reference<double> AngularVelocity
		{
			get { if( _angularVelocity.BeginGet() ) AngularVelocity = _angularVelocity.Get( this ); return _angularVelocity.value; }
			set
			{
				if( _angularVelocity.BeginSet( this, ref value ) )
				{
					try
					{
						AngularVelocityChanged?.Invoke( this );

						if( rigidBody != null && !updatePropertiesWithoutUpdatingBody )
							rigidBody.AngularVelocity = (float)value;
					}
					finally { _angularVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularVelocity"/> property value changes.</summary>
		public event Action<RigidBody2D> AngularVelocityChanged;
		ReferenceField<double> _angularVelocity = 0.0;

		/// <summary>
		/// Whether to display collected collision contacts data.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Contacts" )]
		public Reference<bool> ContactsDisplay
		{
			get { if( _contactsDisplay.BeginGet() ) ContactsDisplay = _contactsDisplay.Get( this ); return _contactsDisplay.value; }
			set { if( _contactsDisplay.BeginSet( this, ref value ) ) { try { ContactsDisplayChanged?.Invoke( this ); } finally { _contactsDisplay.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ContactsDisplay"/> property value changes.</summary>
		public event Action<RigidBody2D> ContactsDisplayChanged;
		ReferenceField<bool> _contactsDisplay = false;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( Mass ):
				case nameof( Inertia ):
				case nameof( EnableGravity ):
				case nameof( LinearDamping ):
				case nameof( AngularDamping ):
				case nameof( LinearVelocity ):
				case nameof( AngularVelocity ):
				case nameof( AllowSleep ):
				case nameof( LocalCenter ):
				case nameof( CCD ):
				case nameof( FixedRotation ):
					if( MotionType.Value != MotionTypeEnum.Dynamic )
						skip = true;
					break;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//static void GetComponentShapesRecursive( CollisionShape2D shape, Transform shapeTransform, List<Tuple<CollisionShape2D, Transform>> result )
		//{
		//	result.Add( new Tuple<CollisionShape2D, Transform>( shape, shapeTransform ) );

		//	//foreach( var child in shape.GetComponents<CollisionShape2D>( false, false, true ) )
		//	//{
		//	//	var childTransform = shapeTransform * child.TransformRelativeToParent.Value;
		//	//	GetComponentShapesRecursive( child, childTransform, result );
		//	//}
		//}

		protected override void OnTransformChanged()
		{
			if( rigidBody != null && !updatePropertiesWithoutUpdatingBody )
			{
				var bodyTransform = Transform.Value;

				if( rigidBodyCreatedTransformScale != bodyTransform.Scale )
				{
					RecreateBody();
				}
				else
				{
					//update transform
					rigidBody.Position = Physics2DUtility.Convert( bodyTransform.Position.ToVector2() );
					rigidBody.Rotation = -(float)MathEx.DegreeToRadian( bodyTransform.Rotation.ToAngles().Yaw );

					//update constraints
					if( rigidBody.JointList != null )
					{
						foreach( var c in GetLinkedCreatedConstraints() )
							c.RecreateConstraint();
					}
				}
			}

			base.OnTransformChanged();
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			if( rigidBody != null )
			{
				var tr = Transform.Value;
				var trNoScale = new Transform( tr.Position, tr.Rotation );

				Bounds bounds = Bounds.Cleared;
				foreach( var p in rigidBodyLocalPoints )
					bounds.Add( trNoScale * new Vector3( p, 0 ) );

				if( !bounds.IsCleared() )
				{
					bounds.Expand( new Vector3( 0, 0, 0.001 ) );

					var b = new SpaceBounds( bounds );
					newBounds = SpaceBounds.Merge( newBounds, b );
				}
			}
		}

		World GetPhysicsWorldData( bool canInit )
		{
			var scene = ParentScene;
			if( scene != null )
				return scene.Physics2DGetWorld( canInit );
			return null;
		}

		[Browsable( false )]
		bool CanCreate
		{
			get
			{
				if( Name == "Collision Definition" && Parent as Mesh != null )
					return false;
				return true;
			}
		}

		void CreateBody()
		{
			if( !CanCreate )
				return;

			duringCreateDestroy = true;

			var physicsWorldData = GetPhysicsWorldData( true );
			if( physicsWorldData != null )
			{
				if( rigidBody != null )
					Log.Fatal( "RigidBody2D: CreateBody: rigidBody != null." );
				if( !EnabledInHierarchy )
					Log.Fatal( "RigidBody2D: CreateBody: !EnabledInHierarchy." );

				var bodyTransform = Transform.Value;
				var bodyTransformScale = new Transform( Vector3.Zero, Quaternion.Identity, bodyTransform.Scale );

				//get shapes. calculate local transforms with applied body scaling.
				var componentShapes = new List<(CollisionShape2D, Transform)>();
				foreach( var child in GetComponents<CollisionShape2D>() )
				{
					if( child.Enabled )
						componentShapes.Add( (child, bodyTransformScale * child.TransformRelativeToParent.Value) );
				}
				//var componentShapes = new List<Tuple<CollisionShape2D, Transform>>();
				//foreach( var child in GetComponents<CollisionShape2D>( false, false, true ) )
				//	GetComponentShapesRecursive( child, bodyTransformScale * child.TransformRelativeToParent.Value, componentShapes );

				if( componentShapes.Count > 0 )
				{
					//use local variable to prevent double update inside properties.
					Body body = new Body();

					foreach( var shapeItem in componentShapes )
					{
						var shape = shapeItem.Item1;
						var fixtures = shape.CreateShape( body, shapeItem.Item2, rigidBodyLocalPoints );
						if( fixtures != null )
						{
							foreach( var fixture in fixtures )
							{
								fixture.Tag = shape;
								fixture.Friction = (float)shape.Friction;
								fixture.Restitution = (float)shape.Restitution;
								fixture.CollisionCategories = (Category)shape.CollisionCategories.Value;
								fixture.CollidesWith = (Category)shape.CollidesWith.Value;
								fixture.CollisionGroup = (short)shape.CollisionGroup;
							}
						}
					}

					if( body.FixtureList.Count != 0 )
					{
						body.Position = Physics2DUtility.Convert( bodyTransform.Position.ToVector2() );
						body.Rotation = -(float)MathEx.DegreeToRadian( bodyTransform.Rotation.ToAngles().Yaw );
						body.FixedRotation = FixedRotation;

						switch( MotionType.Value )
						{
						case MotionTypeEnum.Static: body.BodyType = BodyType.Static; break;
						case MotionTypeEnum.Dynamic: body.BodyType = BodyType.Dynamic; break;
						case MotionTypeEnum.Kinematic: body.BodyType = BodyType.Kinematic; break;
						}

						body.IsBullet = CCD;

						if( MotionType.Value == MotionTypeEnum.Dynamic )
						{
							UpdateMassAndInertia( body );

							body.SleepingAllowed = AllowSleep;
							body.LocalCenter = Physics2DUtility.Convert( LocalCenter );

							body.LinearDamping = (float)LinearDamping;
							body.AngularDamping = (float)AngularDamping;
							body.LinearVelocity = Physics2DUtility.Convert( LinearVelocity );
							body.AngularVelocity = (float)MathEx.DegreeToRadian( AngularVelocity );
							body.IgnoreGravity = !EnableGravity;
						}

						rigidBody = body;
						rigidBody.Tag = this;

						physicsWorldData.Add( rigidBody );
						rigidBodyCreatedTransformScale = bodyTransform.Scale;
					}
				}

				SpaceBoundsUpdate();
			}

			duringCreateDestroy = false;
		}

		void UpdateMassAndInertia( Body body )
		{
			body.Mass = (float)Mass.Value;
			body.Inertia = (float)Inertia.Value;
		}

		List<Constraint2D> GetLinkedCreatedConstraints()
		{
			var list = new List<Constraint2D>();

			if( rigidBody.JointList != null )
			{
				for( var edge = rigidBody.JointList; edge != null; edge = edge.Next )
				{
					var joint = edge.Joint;

					var constraint = joint.Tag as Constraint2D;
					if( constraint != null )
						list.Add( constraint );
				}
			}

			return list;
		}

		void DestroyBody()
		{
			duringCreateDestroy = true;

			var physicsWorldData = GetPhysicsWorldData( false );
			if( physicsWorldData != null )
			{
				//destroy linked constraints
				if( rigidBody != null && rigidBody.JointList != null )
				{
					foreach( var c in GetLinkedCreatedConstraints() )
						c.DestroyConstraint();
				}

				if( rigidBody != null )
				{
					rigidBodyCreatedTransformScale = Vector3.Zero;
					physicsWorldData.Remove( rigidBody );
					rigidBody = null;
					rigidBodyLocalPoints.Clear();
				}
			}

			duringCreateDestroy = false;
		}

		public void RecreateBody()
		{
			if( EnabledInHierarchy && !duringCreateDestroy )
			{
				DestroyBody();
				CreateBody();
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
			{
				if( rigidBody == null )//after loading it is created through shape, because RecreateBody() called
					CreateBody();
			}
			else
				DestroyBody();
		}

		public override void UpdateDataFromPhysicsEngine()
		{
			if( MotionType.Value == MotionTypeEnum.Dynamic && rigidBody != null )
			{
				var position = Physics2DUtility.Convert( rigidBody.Position );
				var rotation = -rigidBody.Rotation;

				var oldT = Transform.Value;

				Matrix3F.FromRotateByZ( rotation, out var mat3 );
				var rot2 = mat3.ToQuaternion();
				//var rot2 = new Angles( 0, 0, MathEx.RadianToDegree( rot ) );

				var newT = new Transform( new Vector3( position.X, position.Y, oldT.Position.Z ), rot2, oldT.Scale );

				try
				{
					updatePropertiesWithoutUpdatingBody = true;
					Transform = newT;
					LinearVelocity = Physics2DUtility.Convert( rigidBody.LinearVelocity );
					AngularVelocity = MathEx.RadianToDegree( rigidBody.AngularVelocity );
				}
				finally
				{
					updatePropertiesWithoutUpdatingBody = false;
				}
			}
		}

		protected override bool OnEnabledSelectionByCursor()
		{
			var scene = ParentScene;
			if( !scene.GetDisplayDevelopmentDataInThisApplication() )
				return false;
			if( rigidBody != null )
			{
				if( !scene.DisplayPhysicalObjects )
					return false;
			}
			else
			{
				if( !scene.DisplayLabels )
					return false;
			}
			return base.OnEnabledSelectionByCursor();
		}

		protected override void OnCheckSelectionByRay( CheckSelectionByRayContext context )
		{
			base.OnCheckSelectionByRay( context );

			if( rigidBody != null )
			{
				context.thisObjectWasChecked = true;

				var worldData = GetPhysicsWorldData( false );
				if( worldData != null )
				{
					if( context.viewport.CameraSettings.Projection == ProjectionType.Orthographic )
					{
						var point = Physics2DUtility.Convert( context.ray.Origin.ToVector2() );
						foreach( var fixture in rigidBody.FixtureList )
						{
							if( fixture.TestPoint( ref point ) )
							{
								context.thisObjectResultRayScale = 0;
								break;
							}
						}
					}
					else
					{
						var bounds = SpaceBounds.BoundingBox;
						if( bounds.Intersects( ref context.ray, out double scale ) )
							context.thisObjectResultRayScale = scale;
					}
				}
			}
		}

		CenterOfMassGeometry GetCenterOfMassGeometry( float radius )
		{
			for( int n = 0; n < centerOfMassGeometryCache.Count; n++ )
			{
				var item2 = centerOfMassGeometryCache[ n ];
				if( Math.Abs( item2.radius - radius ) < .01 )
					return item2;
			}

			while( centerOfMassGeometryCache.Count > 15 )
				centerOfMassGeometryCache.RemoveAt( 0 );

			var item = new CenterOfMassGeometry();
			item.radius = radius;
			var segments = 10;
			SimpleMeshGenerator.GenerateSphere( radius, segments, ( ( segments + 1 ) / 2 ) * 2, false, out item.positions, out item.indices );
			centerOfMassGeometryCache.Add( item );

			return item;
		}

		public override void RenderPhysicalObject( ViewportRenderingContext context, out int verticesRendered )
		{
			verticesRendered = 0;

			var context2 = context.ObjectInSpaceRenderingContext;

			//var scene = ParentScene;
			//bool show = ( scene.GetDisplayDevelopmentDataInThisApplication() && scene.DisplayPhysicalObjects ) ||
			//	context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.dragDropCreateObject == this;
			if( /*show && */rigidBody != null && context.Owner.Simple3DRenderer != null )
			{
				var viewport = context.Owner;
				var renderer = viewport.Simple3DRenderer;

				//if( context2.displayPhysicalObjectsCounter < context2.displayPhysicalObjectsMax )
				//{
				//	context2.displayPhysicalObjectsCounter++;

				//draw body
				{
					ColorValue color;
					if( MotionType.Value == MotionTypeEnum.Static )
						color = ProjectSettings.Get.Colors.SceneShowPhysicsStaticColor;
					else if( rigidBody.Awake )
						color = ProjectSettings.Get.Colors.SceneShowPhysicsDynamicActiveColor;
					else
						color = ProjectSettings.Get.Colors.SceneShowPhysicsDynamicInactiveColor;
					viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

					Transform tr;
					{
						tr = Transform.Value;
						var newRotation = new Angles( 0, 0, tr.Rotation.ToAngles().Yaw ).ToQuaternion();
						tr = tr.UpdateRotation( newRotation );
					}

					foreach( var shape in GetComponents<CollisionShape2D>() )
					{
						if( shape.Enabled )
							shape.Render( viewport, tr, false, ref verticesRendered );
					}
					//foreach( var shape in GetComponents<CollisionShape2D>( onlyEnabledInHierarchy: true ) )
					//	shape.Render( viewport, tr, false, ref verticesRendered );

					//foreach( var shape in GetComponents<CollisionShape2D>( false, true, true ) )
					//	shape.Render( viewport, tr, false, ref verticesRendered );

					////center of mass
					//if( MotionType.Value == MotionTypeEnum.Dynamic )
					//{
					//	var center = rigidBody.LocalCenter;//CenterOfMassPosition;
					//	double radius = SpaceBounds.CalculatedBoundingSphere.Radius / 16;

					//	var item = GetCenterOfMassGeometry( (float)radius );
					//	var transform = Matrix4.FromTranslate( Physics2DUtility.Convert( center ) );
					//	context.Owner.Simple3DRenderer.AddTriangles( item.positions, item.indices, ref transform, false, true );
					//	//context.viewport.Simple3DRenderer.AddSphere( BulletPhysicsUtility.Convert( center ), radius, 16, true );

					//	//Vector3 center = Vector3.Zero;
					//	//double radius = 1.0;
					//	//rigidBody.CollisionShape.GetBoundingSphere( out center, out radius );
					//	//center = rigidBody.CenterOfMassPosition;
					//	//radius /= 16.0;
					//	//context.viewport.DebugGeometry.AddSphere( BulletUtils.Convert( center ), radius );

					//	verticesRendered += item.positions.Length;
					//}
				}

				//draw selection
				if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
				{
					ColorValue color;
					if( context2.selectedObjects.Contains( this ) )
						color = ProjectSettings.Get.Colors.SelectedColor;
					else if( context2.canSelectObjects.Contains( this ) )
						color = ProjectSettings.Get.Colors.CanSelectColor;
					else
						color = ProjectSettings.Get.Colors.SceneShowPhysicsDynamicActiveColor;

					color.Alpha *= .5f;
					viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

					foreach( var shape in GetComponents<CollisionShape2D>() )
					{
						if( shape.Enabled )
							shape.Render( viewport, Transform, true, ref verticesRendered );
					}
					//foreach( var shape in GetComponents<CollisionShape2D>( onlyEnabledInHierarchy: true ) )
					//	shape.Render( viewport, Transform, true, ref verticesRendered );

					//foreach( var shape in GetComponents<CollisionShape2D>( false, true, true ) )
					//	shape.Render( viewport, Transform, true, ref verticesRendered );

					//context.viewport.DebugGeometry.AddBounds( SpaceBounds.CalculatedBoundingBox );
				}

				//display collision contacts
				if( ContactsDisplay && rigidBody.ContactList != null && EngineApp.IsSimulation )
				{
					var size3 = SpaceBounds.BoundingBox.GetSize();
					var scale = (float)Math.Min( size3.X, size3.Y ) / 30;
					//var scale = (float)Math.Max( size3.X, Math.Max( size3.Y, size3.Z ) ) / 40;

					var edge = rigidBody.ContactList;
					while( edge != null )
					{
						var contact = edge.Contact;

						int pointCount = contact.Manifold.PointCount;
						if( pointCount > 0 )
						{
							if( pointCount > 2 )
								pointCount = 2;

							renderer.SetColor( new ColorValue( 1, 0, 0 ) );

							contact.GetWorldManifold( out _, out var points );

							for( int n = 0; n < pointCount; n++ )
							{
								var point = Physics2DUtility.Convert( points[ n ] );

								var pos = new Vector3( point, TransformV.Position.Z );
								var bounds = new Bounds(
									pos - new Vector3( scale, scale, scale ),
									pos + new Vector3( scale, scale, scale ) );

								renderer.AddBounds( bounds, true );
							}

						}

						edge = edge.Next;
					}
				}

				//}
			}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				var showLabels = /*show &&*/ rigidBody == null;
				if( !showLabels )
					context2.disableShowingLabelForThisObject = true;
			}
		}

		[Browsable( false )]
		public override Body Physics2DBody
		{
			get { return rigidBody; }
		}

		/// <summary>
		/// Gets the sleep state of the body. A sleeping body has very low CPU cost.
		/// </summary>
		[Browsable( false )]
		public bool Active
		{
			get
			{
				if( rigidBody != null )
					return rigidBody.Awake;
				return false;
			}
			set
			{
				if( rigidBody != null )
					rigidBody.Awake = true;
			}
		}

		//[Browsable( false )]
		//public List<ContactsDataItem> ContactsData
		//{
		//	get { return contactsData; }
		//	set { contactsData = value; }
		//}

		/// <summary>
		/// Apply a force at a world point. If the force is not applied at the center of mass, it will generate a torque and affect the angular velocity. This wakes up the body.
		/// </summary>
		/// <param name="force">The world force vector, usually in Newtons (N).</param>
		/// <param name="point">The world position of the point of application.</param>
		public void ApplyForce( Vector2 force, Vector2 point )
		{
			rigidBody?.ApplyForce( Physics2DUtility.Convert( force ), Physics2DUtility.Convert( point ) );
			//rigidBody?.ApplyForce( Physics2DUtility.Convert( force ), rigidBody.Position + Physics2DUtility.Convert( point ) );
		}

		/// <summary>
		/// Applies a force at the center of mass.
		/// </summary>
		/// <param name="force">The force.</param>
		public void ApplyForce( Vector2 force )
		{
			rigidBody?.ApplyForce( Physics2DUtility.Convert( force ) );
		}

		/// <summary>
		/// Apply a torque. This affects the angular velocity without affecting the linear velocity of the center of mass. This wakes up the body.
		/// </summary>
		/// <param name="torque">The torque about the z-axis (out of the screen), usually in N-m.</param>
		public void ApplyTorque( double torque )
		{
			rigidBody?.ApplyTorque( (float)torque );
		}

		/// <summary>
		/// Apply an impulse at a point. This immediately modifies the velocity.
		/// This wakes up the body.
		/// </summary>
		/// <param name="impulse">The world impulse vector, usually in N-seconds or kg-m/s.</param>
		public void ApplyLinearImpulse( Vector2 impulse )
		{
			rigidBody?.ApplyLinearImpulse( Physics2DUtility.Convert( impulse ) );
		}

		/// <summary>
		/// Apply an impulse at a point. This immediately modifies the velocity. It also modifies the angular velocity if the point of application
		/// is not at the center of mass. This wakes up the body.
		/// </summary>
		/// <param name="impulse">The world impulse vector, usually in N-seconds or kg-m/s.</param>
		/// <param name="point">The world position of the point of application.</param>
		public void ApplyLinearImpulse( Vector2 impulse, Vector2 point )
		{
			rigidBody?.ApplyLinearImpulse( Physics2DUtility.Convert( impulse ), Physics2DUtility.Convert( point ) );
			//rigidBody?.ApplyLinearImpulse( Physics2DUtility.Convert( impulse ), rigidBody.Position + Physics2DUtility.Convert( point ) );
		}

		/// <summary>
		/// Apply an angular impulse.
		/// </summary>
		/// <param name="impulse">The angular impulse in units of kg*m*m/s.</param>
		public void ApplyAngularImpulse( double impulse )
		{
			rigidBody?.ApplyAngularImpulse( (float)impulse );
		}

	}
}
