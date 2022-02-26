// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Drawing.Design;
using Internal.BulletSharp;
using Internal.BulletSharp.Math;

namespace NeoAxis
{
	/// <summary>
	/// Rigid physical body.
	/// </summary>
	public class RigidBody : PhysicalBody
	{
		Internal.BulletSharp.CollisionShape collisionShape;
		Internal.BulletSharp.RigidBody rigidBody;
		Vector3 rigidBodyCreatedTransformScale;

		bool duringCreateDestroy;
		bool updatePropertiesWithoutUpdatingBody;

		Matrix4 centerOfMassOffset;
		Matrix4 centerOfMassOffsetInverted;

		public struct ContactsDataItem
		{
			public int SimulationSubStep;
			public Vector3 PositionWorldOnA;
			public Vector3 PositionWorldOnB;
			public float AppliedImpulse;
			public float Distance;
			public RigidBody BodyB;
		}
		OpenList<ContactsDataItem> contactsData;

		////!!!!maybe temp. может какую-то более унифицированную штуку сделать
		//List<Action> actionsBeforeBodyDestroy = new List<Action>();

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
		[Serialize]
		[DefaultValue( MotionTypeEnum.Static )]//.Dynamic )]
		[Category( "Rigid Body" )]
		public Reference<MotionTypeEnum> MotionType
		{
			get { if( _motionType.BeginGet() ) MotionType = _motionType.Get( this ); return _motionType.value; }
			set
			{
				if( _motionType.BeginSet( ref value ) )
				{
					try
					{
						MotionTypeChanged?.Invoke( this );
						//!!!!об€зательно ли пересоздавать?
						if( rigidBody != null )
							RecreateBody();
					}
					finally { _motionType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MotionType"/> property value changes.</summary>
		public event Action<RigidBody> MotionTypeChanged;
		ReferenceField<MotionTypeEnum> _motionType = MotionTypeEnum.Static;// Dynamic;

		/// <summary>
		/// The mass of the rigid body.
		/// </summary>
		[Serialize]
		[DefaultValue( 1.0 )]
		[Category( "Rigid Body" )]
		public Reference<double> Mass
		{
			get { if( _mass.BeginGet() ) Mass = _mass.Get( this ); return _mass.value; }
			set
			{
				if( value < 0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _mass.BeginSet( ref value ) )
				{
					try
					{
						MassChanged?.Invoke( this );

						//!!!!
					}
					finally { _mass.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mass"/> property value changes.</summary>
		public event Action<RigidBody> MassChanged;
		ReferenceField<double> _mass = 1;


		//EnableGravity
		ReferenceField<bool> _enableGravity = true;

		/// <summary>
		/// Whether the rigid body is affected by the gravity.
		/// </summary>
		[Serialize]
		[DefaultValue( true )]
		[Category( "Rigid Body" )]
		public Reference<bool> EnableGravity
		{
			get
			{
				if( _enableGravity.BeginGet() )
					EnableGravity = _enableGravity.Get( this );
				return _enableGravity.value;
			}
			set
			{
				if( _enableGravity.BeginSet( ref value ) )
				{
					try
					{
						EnableGravityChanged?.Invoke( this );
						if( rigidBody != null )
						{
							if( !EnableGravity )
								rigidBody.Flags |= RigidBodyFlags.DisableWorldGravity;
							else
								rigidBody.Flags &= ~RigidBodyFlags.DisableWorldGravity;
						}
					}
					finally { _enableGravity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="EnableGravity"/> property value changes.</summary>
		public event Action<RigidBody> EnableGravityChanged;


		//!!!!defaults
		//LinearDamping
		ReferenceField<double> _linearDamping = 0.1;
		/// <summary>
		/// The linear reduction of velocity over time.
		/// </summary>
		[Serialize]
		[DefaultValue( 0.1 )]
		[Range( 0, 1 )]
		[Category( "Rigid Body" )]
		public Reference<double> LinearDamping
		{
			get
			{
				if( _linearDamping.BeginGet() )
					LinearDamping = _linearDamping.Get( this );
				return _linearDamping.value;
			}
			set
			{
				if( _linearDamping.BeginSet( ref value ) )
				{
					try
					{
						LinearDampingChanged?.Invoke( this );
						rigidBody?.SetDamping( (float)LinearDamping, (float)AngularDamping );
					}
					finally { _linearDamping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearDamping"/> property value changes.</summary>
		public event Action<RigidBody> LinearDampingChanged;


		//!!!!defaults
		//AngularDamping
		ReferenceField<double> _angularDamping = 0.1;

		/// <summary>
		/// The angular reduction of velocity over time.
		/// </summary>
		[Serialize]
		[DefaultValue( 0.1 )]
		[Range( 0, 1 )]
		[Category( "Rigid Body" )]
		public Reference<double> AngularDamping
		{
			get
			{
				if( _angularDamping.BeginGet() )
					AngularDamping = _angularDamping.Get( this );
				return _angularDamping.value;
			}
			set
			{
				if( _angularDamping.BeginSet( ref value ) )
				{
					try
					{
						AngularDampingChanged?.Invoke( this );
						rigidBody?.SetDamping( (float)LinearDamping, (float)AngularDamping );
					}
					finally { _angularDamping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularDamping"/> property value changes.</summary>
		public event Action<RigidBody> AngularDampingChanged;

		/// <summary>
		/// The physical material used by the rigid body.
		/// </summary>
		[Serialize]
		[DefaultValue( null )]
		[Category( "Rigid Body" )]
		public Reference<PhysicalMaterial> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set
			{
				if( _material.BeginSet( ref value ) )
				{
					try
					{
						MaterialChanged?.Invoke( this );
						if( rigidBody != null )
							SetMaterial( rigidBody );
					}
					finally { _material.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<RigidBody> MaterialChanged;
		ReferenceField<PhysicalMaterial> _material;

		/// <summary>
		/// The type of friction applied on the rigid body.
		/// </summary>
		[DefaultValue( PhysicalMaterial.FrictionModeEnum.Simple )]
		[Serialize]
		[Category( "Rigid Body" )]
		public Reference<PhysicalMaterial.FrictionModeEnum> MaterialFrictionMode
		{
			get { if( _materialFrictionMode.BeginGet() ) MaterialFrictionMode = _materialFrictionMode.Get( this ); return _materialFrictionMode.value; }
			set
			{
				if( _materialFrictionMode.BeginSet( ref value ) )
				{
					try
					{
						MaterialFrictionModeChanged?.Invoke( this );
						if( rigidBody != null )
							SetMaterial( rigidBody );
					}
					finally { _materialFrictionMode.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialFrictionMode"/> property value changes.</summary>
		public event Action<RigidBody> MaterialFrictionModeChanged;
		ReferenceField<PhysicalMaterial.FrictionModeEnum> _materialFrictionMode = PhysicalMaterial.FrictionModeEnum.Simple;

		/// <summary>
		/// The amount of friction applied on the rigid body.
		/// </summary>
		[Serialize]
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		[Category( "Rigid Body" )]
		public Reference<double> MaterialFriction
		{
			get { if( _materialFriction.BeginGet() ) MaterialFriction = _materialFriction.Get( this ); return _materialFriction.value; }
			set
			{
				if( _materialFriction.BeginSet( ref value ) )
				{
					try
					{
						MaterialFrictionChanged?.Invoke( this );
						if( rigidBody != null )
							SetMaterial( rigidBody );
					}
					finally { _materialFriction.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialFriction"/> property value changes.</summary>
		public event Action<RigidBody> MaterialFrictionChanged;
		ReferenceField<double> _materialFriction = 0.5;

		/// <summary>
		/// The amount of directional friction applied on the rigid body.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Serialize]
		//[ApplicableRange( 0, 1 )]
		[Category( "Rigid Body" )]
		public Reference<Vector3> MaterialAnisotropicFriction
		{
			get { if( _materialAnisotropicFriction.BeginGet() ) MaterialAnisotropicFriction = _materialAnisotropicFriction.Get( this ); return _materialAnisotropicFriction.value; }
			set
			{
				if( _materialAnisotropicFriction.BeginSet( ref value ) )
				{
					try { MaterialAnisotropicFrictionChanged?.Invoke( this ); }
					finally { _materialAnisotropicFriction.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AnisotropicFriction"/> property value changes.</summary>
		public event Action<RigidBody> MaterialAnisotropicFrictionChanged;
		ReferenceField<Vector3> _materialAnisotropicFriction = Vector3.One;

		/// <summary>
		/// The amount of friction applied when rigid body is spinning.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Serialize]
		[Range( 0, 1 )]
		[Category( "Rigid Body" )]
		public Reference<double> MaterialSpinningFriction
		{
			get { if( _materialSpinningFriction.BeginGet() ) MaterialSpinningFriction = _materialSpinningFriction.Get( this ); return _materialSpinningFriction.value; }
			set
			{
				if( _materialSpinningFriction.BeginSet( ref value ) )
				{
					try
					{
						MaterialSpinningFrictionChanged?.Invoke( this );
						if( rigidBody != null )
							SetMaterial( rigidBody );
					}
					finally { _materialSpinningFriction.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialSpinningFriction"/> property value changes.</summary>
		public event Action<RigidBody> MaterialSpinningFrictionChanged;
		ReferenceField<double> _materialSpinningFriction = 0.5;

		/// <summary>
		/// The amount of friction applied when rigid body is rolling.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Serialize]
		[Range( 0, 1 )]
		[Category( "Rigid Body" )]
		public Reference<double> MaterialRollingFriction
		{
			get { if( _materialRollingFriction.BeginGet() ) MaterialRollingFriction = _materialRollingFriction.Get( this ); return _materialRollingFriction.value; }
			set
			{
				if( _materialRollingFriction.BeginSet( ref value ) )
				{
					try
					{
						MaterialRollingFrictionChanged?.Invoke( this );
						if( rigidBody != null )
							SetMaterial( rigidBody );
					}
					finally { _materialRollingFriction.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialRollingFriction"/> property value changes.</summary>
		public event Action<RigidBody> MaterialRollingFrictionChanged;
		ReferenceField<double> _materialRollingFriction = 0.5;

		/// <summary>
		/// The ratio of the final relative velocity to initial relative velocity of the rigid body after collision.
		/// </summary>
		[Serialize]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Category( "Rigid Body" )]
		public Reference<double> MaterialRestitution
		{
			get { if( _materialRestitution.BeginGet() ) MaterialRestitution = _materialRestitution.Get( this ); return _materialRestitution.value; }
			set
			{
				if( _materialRestitution.BeginSet( ref value ) )
				{
					try
					{
						MaterialRestitutionChanged?.Invoke( this );
						if( rigidBody != null && Material.Value == null )
							rigidBody.Restitution = (float)MaterialRestitution;
					}
					finally { _materialRestitution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialRestitution"/> property value changes.</summary>
		public event Action<RigidBody> MaterialRestitutionChanged;
		ReferenceField<double> _materialRestitution;

		/// <summary>
		/// Whether the rigid body is using continious collision detection.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Rigid Body" )]
		public Reference<bool> CCD
		{
			get { if( _ccd.BeginGet() ) CCD = _ccd.Get( this ); return _ccd.value; }
			set
			{
				if( _ccd.BeginSet( ref value ) )
				{
					try
					{
						CcdChanged?.Invoke( this );
						if( rigidBody != null )
							UpdateCCD();
					}
					finally { _ccd.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Ccd"/> property value changes.</summary>
		public event Action<RigidBody> CcdChanged;
		ReferenceField<bool> _ccd = false;

		/// <summary>
		/// The swept sphere radius of continious collision detection.
		/// </summary>
		[DefaultValue( 0.05 )]
		[Serialize]
		[DisplayName( "CCD Swept Sphere Radius" )]
		[Category( "Rigid Body" )]
		public Reference<double> CcdSweptSphereRadius
		{
			get
			{
				if( _ccdSweptSphereRadius.BeginGet() )
					CcdSweptSphereRadius = _ccdSweptSphereRadius.Get( this );
				return _ccdSweptSphereRadius.value;
			}
			set
			{
				if( _ccdSweptSphereRadius.BeginSet( ref value ) )
				{
					try
					{
						CcdSweptSphereRadiusChanged?.Invoke( this );
						if( rigidBody != null )
							UpdateCCD();
					}
					finally { _ccdSweptSphereRadius.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CcdSweptSphereRadius"/> property value changes.</summary>
		public event Action<RigidBody> CcdSweptSphereRadiusChanged;
		ReferenceField<double> _ccdSweptSphereRadius = 0.05;

		/// <summary>
		/// The motion threshold below which the continuous collision detection will not update. Increase the value to improve performance.
		/// </summary>
		[DefaultValue( 0.0000001 )]
		[Serialize]
		[DisplayName( "CCD Motion Threshold" )]
		[Category( "Rigid Body" )]
		public Reference<double> CcdMotionThreshold
		{
			get
			{
				if( _ccdMotionThreshold.BeginGet() )
					CcdMotionThreshold = _ccdMotionThreshold.Get( this );
				return _ccdMotionThreshold.value;
			}
			set
			{
				if( _ccdMotionThreshold.BeginSet( ref value ) )
				{
					try
					{
						CcdMotionThresholdChanged?.Invoke( this );
						if( rigidBody != null )
							UpdateCCD();
					}
					finally { _ccdMotionThreshold.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CcdMotionThreshold"/> property value changes.</summary>
		public event Action<RigidBody> CcdMotionThresholdChanged;
		ReferenceField<double> _ccdMotionThreshold = 0.0000001;

		//CenterOfMassManual
		ReferenceField<bool> _centerOfMassManual = false;
		/// <summary>
		/// Whether the rigid body is using manual center of mass.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Rigid Body" )]
		public Reference<bool> CenterOfMassManual
		{
			get
			{
				if( _centerOfMassManual.BeginGet() )
					CenterOfMassManual = _centerOfMassManual.Get( this );
				return _centerOfMassManual.value;
			}
			set
			{
				if( _centerOfMassManual.BeginSet( ref value ) )
				{
					try
					{
						CenterOfMassManualChanged?.Invoke( this );

						//!!!!об€зательно ли пересоздавать?
						if( rigidBody != null )
							RecreateBody();
					}
					finally { _centerOfMassManual.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CenterOfMassManual"/> property value changes.</summary>
		public event Action<RigidBody> CenterOfMassManualChanged;

		//CenterOfMassPosition
		ReferenceField<Vector3> _centerOfMassPosition = Vector3.Zero;
		/// <summary>
		/// The position of center of mass.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Serialize]
		[Category( "Rigid Body" )]
		public Reference<Vector3> CenterOfMassPosition
		{
			get
			{
				if( _centerOfMassPosition.BeginGet() )
					CenterOfMassPosition = _centerOfMassPosition.Get( this );
				return _centerOfMassPosition.value;
			}
			set
			{
				if( _centerOfMassPosition.BeginSet( ref value ) )
				{
					try
					{
						CenterOfMassPositionChanged?.Invoke( this );

						//!!!!об€зательно ли пересоздавать?
						if( rigidBody != null )
							RecreateBody();
					}
					finally { _centerOfMassPosition.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CenterOfMassPosition"/> property value changes.</summary>
		public event Action<RigidBody> CenterOfMassPositionChanged;

		//InertiaTensorFactor
		ReferenceField<Vector3> _inertiaTensorFactor = Vector3.One;
		/// <summary>
		/// The moment of inertia.
		/// </summary>
		[DefaultValue( "1 1 1" )]
		[Serialize]
		[Category( "Rigid Body" )]
		public Reference<Vector3> InertiaTensorFactor
		{
			get
			{
				if( _inertiaTensorFactor.BeginGet() )
					InertiaTensorFactor = _inertiaTensorFactor.Get( this );
				return _inertiaTensorFactor.value;
			}
			set
			{
				if( _inertiaTensorFactor.BeginSet( ref value ) )
				{
					try
					{
						InertiaTensorFactorChanged?.Invoke( this );

						//!!!!об€зательно ли пересоздавать?

						//!!!!new, вроде как не работает
						//if( rigidBody != null )
						//{
						//	rigidBody.SetMassProps( Mass.Value, rigidBody.CollisionShape.CalculateLocalInertia( Mass.Value ) * BulletUtils.Convert( value ) );
						//	rigidBody.UpdateInertiaTensor();
						//}
						if( rigidBody != null )
							RecreateBody();
					}
					finally { _inertiaTensorFactor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="InertiaTensorFactor"/> property value changes.</summary>
		public event Action<RigidBody> InertiaTensorFactorChanged;

		//LinearSleepingThreshold
		ReferenceField<double> _linearSleepingThreshold = 0.5;
		/// <summary>
		/// The linear velocity threshold below which the body will stop movement.
		/// </summary>
		[DefaultValue( 0.5 )]//1.0 )]
		[Serialize]
		[Category( "Rigid Body" )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> LinearSleepingThreshold
		{
			get
			{
				if( _linearSleepingThreshold.BeginGet() )
					LinearSleepingThreshold = _linearSleepingThreshold.Get( this );
				return _linearSleepingThreshold.value;
			}
			set
			{
				if( _linearSleepingThreshold.BeginSet( ref value ) )
				{
					try
					{
						LinearSleepingThresholdChanged?.Invoke( this );
						rigidBody?.SetSleepingThresholds( value, AngularSleepingThreshold.Value );
					}
					finally { _linearSleepingThreshold.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearSleepingThreshold"/> property value changes.</summary>
		public event Action<RigidBody> LinearSleepingThresholdChanged;

		//AngularSleepingThreshold
		ReferenceField<double> _angularSleepingThreshold = 0.5;
		/// <summary>
		/// The angular velocity threshold below which the body will stop rotating.
		/// </summary>
		[DefaultValue( 0.5 )]//0.8 )]
		[Serialize]
		[Category( "Rigid Body" )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> AngularSleepingThreshold
		{
			get
			{
				if( _angularSleepingThreshold.BeginGet() )
					AngularSleepingThreshold = _angularSleepingThreshold.Get( this );
				return _angularSleepingThreshold.value;
			}
			set
			{
				if( _angularSleepingThreshold.BeginSet( ref value ) )
				{
					try
					{
						AngularSleepingThresholdChanged?.Invoke( this );
						rigidBody?.SetSleepingThresholds( LinearSleepingThreshold.Value, value );
					}
					finally { _angularSleepingThreshold.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularSleepingThreshold"/> property value changes.</summary>
		public event Action<RigidBody> AngularSleepingThresholdChanged;

		//!!!!name: ContactGroup, ContactMask
		//CollisionGroup
		ReferenceField<int> _collisionGroup = 1;

		/// <summary>
		/// The collision filtering group.
		/// </summary>
		[DefaultValue( 1 )]
		[Serialize]
		[Category( "Collision Filtering" )]
		public Reference<int> CollisionGroup
		{
			get
			{
				if( _collisionGroup.BeginGet() )
					CollisionGroup = _collisionGroup.Get( this );
				return _collisionGroup.value;
			}
			set
			{
				if( _collisionGroup.BeginSet( ref value ) )
				{
					try
					{
						CollisionGroupChanged?.Invoke( this );
						if( rigidBody != null )
							RecreateBody();
					}
					finally { _collisionGroup.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionGroup"/> property value changes.</summary>
		public event Action<RigidBody> CollisionGroupChanged;

		//!!!!default value
		//CollisionMask
		ReferenceField<int> _collisionMask = 1;

		/// <summary>
		/// The collision filtering mask.
		/// </summary>
		[DefaultValue( 1 )]
		[Serialize]
		[Category( "Collision Filtering" )]
		public Reference<int> CollisionMask
		{
			get
			{
				if( _collisionMask.BeginGet() )
					CollisionMask = _collisionMask.Get( this );
				return _collisionMask.value;
			}
			set
			{
				if( _collisionMask.BeginSet( ref value ) )
				{
					try
					{
						CollisionMaskChanged?.Invoke( this );
						if( rigidBody != null )
							RecreateBody();
					}
					finally { _collisionMask.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionMask"/> property value changes.</summary>
		public event Action<RigidBody> CollisionMaskChanged;

		//LinearVelocity
		ReferenceField<Vector3> _linearVelocity = Vector3.Zero;
		/// <summary>
		/// The initial linear velocity of the body.
		/// </summary>
		[Serialize]
		[DefaultValue( "0 0 0" )]
		[Category( "Velocity" )]
		public Reference<Vector3> LinearVelocity
		{
			get
			{
				if( _linearVelocity.BeginGet() )
					LinearVelocity = _linearVelocity.Get( this );
				return _linearVelocity.value;
			}
			set
			{
				if( _linearVelocity.BeginSet( ref value ) )
				{
					try
					{
						LinearVelocityChanged?.Invoke( this );

						//!!!!new
						if( rigidBody != null && !updatePropertiesWithoutUpdatingBody )
							rigidBody.LinearVelocity = BulletPhysicsUtility.Convert( value );
					}
					finally { _linearVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearVelocity"/> property value changes.</summary>
		public event Action<RigidBody> LinearVelocityChanged;


		//AngularVelocity
		ReferenceField<Vector3> _angularVelocity = Vector3.Zero;
		/// <summary>
		/// The initial angular velocity of the body.
		/// </summary>
		[Serialize]
		[DefaultValue( "0 0 0" )]
		[Category( "Velocity" )]
		public Reference<Vector3> AngularVelocity
		{
			get
			{
				if( _angularVelocity.BeginGet() )
					AngularVelocity = _angularVelocity.Get( this );
				return _angularVelocity.value;
			}
			set
			{
				if( _angularVelocity.BeginSet( ref value ) )
				{
					try
					{
						AngularVelocityChanged?.Invoke( this );

						//!!!!new
						if( rigidBody != null && !updatePropertiesWithoutUpdatingBody )
							rigidBody.AngularVelocity = BulletPhysicsUtility.Convert( value );
					}
					finally { _angularVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularVelocity"/> property value changes.</summary>
		public event Action<RigidBody> AngularVelocityChanged;

		/// <summary>
		/// Whether to collect collision contacts data. Use <see cref="ContactsData"/> property to get data.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Contacts" )]
		public Reference<bool> ContactsCollect
		{
			get { if( _contactsCollect.BeginGet() ) ContactsCollect = _contactsCollect.Get( this ); return _contactsCollect.value; }
			set
			{
				if( _contactsCollect.BeginSet( ref value ) )
				{
					try
					{
						ContactsCollectChanged?.Invoke( this );
						if( rigidBody != null )
						{
							if( ContactsCollect )
								rigidBody.CollisionFlags |= CollisionFlags.CollectContacts;
							else
								rigidBody.CollisionFlags &= ~CollisionFlags.CollectContacts;
						}
					}
					finally { _contactsCollect.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ContactsCollect"/> property value changes.</summary>
		public event Action<RigidBody> ContactsCollectChanged;
		ReferenceField<bool> _contactsCollect = false;

		/// <summary>
		/// Whether to display collected collision contacts data.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Contacts" )]
		public Reference<bool> ContactsDisplay
		{
			get { if( _contactsDisplay.BeginGet() ) ContactsDisplay = _contactsDisplay.Get( this ); return _contactsDisplay.value; }
			set { if( _contactsDisplay.BeginSet( ref value ) ) { try { ContactsDisplayChanged?.Invoke( this ); } finally { _contactsDisplay.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ContactsDisplay"/> property value changes.</summary>
		public event Action<RigidBody> ContactsDisplayChanged;
		ReferenceField<bool> _contactsDisplay = false;

		////CanCreate
		////Useful for Collision Definition.
		//ReferenceField<bool> _canCreate = true;
		///// <summary>
		///// Is it possible to create rigid body?
		///// </summary>
		//[DefaultValue( true )]
		//[Serialize]
		//[Category( "Utils" )]
		//public Reference<bool> CanCreate
		//{
		//	get
		//	{
		//		if( _canCreate.BeginGet() )
		//			CanCreate = _canCreate.Get( this );
		//		return _canCreate.value;
		//	}
		//	set
		//	{
		//		if( _canCreate.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CanCreateChanged?.Invoke( this );
		//				RecreateBody();
		//			}
		//			finally { _canCreate.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<RigidBody> CanCreateChanged;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( CCD ):
				case nameof( Mass ):
				case nameof( EnableGravity ):
				case nameof( LinearDamping ):
				case nameof( AngularDamping ):
				case nameof( LinearVelocity ):
				case nameof( AngularVelocity ):
				case nameof( CenterOfMassManual ):
				case nameof( InertiaTensorFactor ):
				case nameof( LinearSleepingThreshold ):
				case nameof( AngularSleepingThreshold ):
					if( MotionType.Value != MotionTypeEnum.Dynamic )
						skip = true;
					break;

				case nameof( CenterOfMassPosition ):
					if( MotionType.Value != MotionTypeEnum.Dynamic || !CenterOfMassManual.Value )
						skip = true;
					break;

				case nameof( MaterialFrictionMode ):
					if( Material.Value != null )
						skip = true;
					break;

				case nameof( MaterialFriction ):
					if( Material.Value != null )
						skip = true;
					break;

				case nameof( MaterialRollingFriction ):
				case nameof( MaterialSpinningFriction ):
				case nameof( MaterialAnisotropicFriction ):
					if( MaterialFrictionMode.Value == PhysicalMaterial.FrictionModeEnum.Simple || Material.Value != null )
						skip = true;
					break;

				case nameof( MaterialRestitution ):
					if( Material.Value != null )
						skip = true;
					break;

				case nameof( CcdMotionThreshold ):
				case nameof( CcdSweptSphereRadius ):
					if( MotionType.Value != MotionTypeEnum.Dynamic || !CCD.Value )
						skip = true;
					break;

				case nameof( ContactsDisplay ):
					if( !ContactsCollect )
						skip = true;
					break;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[Browsable( false )]
		public Internal.BulletSharp.CollisionShape InternalCollisionShape
		{
			get { return collisionShape; }
		}

		[Browsable( false )]
		public Internal.BulletSharp.RigidBody InternalRigidBody
		{
			get { return rigidBody; }
		}

		[Browsable( false )]
		public OpenList<ContactsDataItem> ContactsData
		{
			get { return contactsData; }
			set { contactsData = value; }
		}

		static void GetComponentShapesRecursive( CollisionShape shape, Matrix4 shapeTransform, List<Tuple<CollisionShape, Matrix4>> result )
		{
			result.Add( new Tuple<CollisionShape, Matrix4>( shape, shapeTransform ) );

			foreach( var child in shape.GetComponents<CollisionShape>( false, false, true ) )
			{
				var childTransform = shapeTransform * child.TransformRelativeToParent.Value.ToMatrix4();
				//var childTransform = child.LocalTransform.Value.ToMat4() * shapeTransform;
				GetComponentShapesRecursive( child, childTransform, result );
			}
		}

		protected override void OnTransformChanged()
		{
			if( rigidBody != null && !updatePropertiesWithoutUpdatingBody )
			{
				var bodyTransform = Transform.Value;

				//bool alwaysRecreate = false;
				//bool alwaysRecreate = MotionType.Value == MotionTypeEnum.Dynamic;

				if( rigidBodyCreatedTransformScale != bodyTransform.Scale )//|| alwaysRecreate )
				{
					RecreateBody();
				}
				else
				{
					//update transform

					var t = new Matrix4( bodyTransform.Rotation.ToMatrix3(), bodyTransform.Position );
					if( MotionType.Value == MotionTypeEnum.Dynamic )
						t *= centerOfMassOffset;

					BulletPhysicsUtility.Convert( ref t, out var tb );

					rigidBody.WorldTransform = tb;
					rigidBody.InterpolationWorldTransform = tb;
					if( rigidBody.MotionState != null )
						rigidBody.MotionState.WorldTransform = tb;

					//GetPhysicsWorldData().world.SynchronizeSingleMotionState( rigidBody );

					//update AABB
					GetPhysicsWorldData().world.UpdateSingleAabb( rigidBody );

					//GetPhysicsWorldData().world.ComputeOverlappingPairs();

					//update constraints
					if( rigidBody.NumConstraintRefs != 0 )
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

			//!!!!так?

			if( rigidBody != null )
			{
				rigidBody.GetAabb( out Internal.BulletSharp.Math.BVector3 min, out Internal.BulletSharp.Math.BVector3 max );
				var b = new SpaceBounds( new Bounds( BulletPhysicsUtility.Convert( min ), BulletPhysicsUtility.Convert( max ) ), null );
				newBounds = SpaceBounds.Merge( newBounds, b );
			}
			else
			{
				//!!!!как тут быть?
			}
		}

		public Scene.PhysicsWorldDataClass GetPhysicsWorldData()
		{
			//!!!!slowly?
			var scene = ParentScene;
			if( scene != null )
				return scene.PhysicsWorldData;
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

			var physicsWorldData = GetPhysicsWorldData();
			if( physicsWorldData != null )
			{
				if( rigidBody != null )
					Log.Fatal( "RigidBody: CreateBody: rigidBody != null." );
				if( !EnabledInHierarchy )
					Log.Fatal( "RigidBody: CreateBody: !EnabledInHierarchy." );

				//!!!!обновл€ть положение где-то не только тут. или пересоздавать

				var bodyTransform = Transform.Value;
				var bodyTransformScaleMatrix = Matrix3.FromScale( bodyTransform.Scale ).ToMatrix4();

				//get shapes. calculate local transforms with applied body scaling.
				var componentShapes = new List<Tuple<CollisionShape, Matrix4>>();
				foreach( var child in GetComponents<CollisionShape>( false, false, true ) )
					GetComponentShapesRecursive( child, bodyTransformScaleMatrix * child.TransformRelativeToParent.Value.ToMatrix4(), componentShapes );
				//foreach( var child in GetComponents<CollisionShape>( false, false, true ) )
				//	GetComponentShapesRecursive( child, child.LocalTransform.Value.ToMat4() * bodyTransformScaleMatrix, componentShapes );

				if( componentShapes.Count > 0 )
				{
					//!!!!new
					////!!!!always compound. need compound for shape local transform and when center of mass is not 0,0,0.
					bool needCompound = false;
					if( componentShapes.Count > 1 || MotionType.Value == MotionTypeEnum.Dynamic )
						needCompound = true;
					else
					{
						var tuple = componentShapes[ 0 ];
						var componentShape = tuple.Item1;
						var transformShape = tuple.Item2;

						if( !transformShape.Decompose( out Vector3 pos, out Matrix3 rot, out Vector3 scl ) )
						{
							//!!!!
						}

						const double zeroTolerance = 1e-6f;
						if( !rot.Equals( Matrix3.Identity, zeroTolerance ) || !pos.Equals( Vector3.Zero, zeroTolerance ) )
						{
							needCompound = true;
						}
					}

					//start transform
					//body transform without scaling. Scaling is already applied to shapes.
					var startTransform = new Matrix4( bodyTransform.Rotation.ToMatrix3(), bodyTransform.Position );

					//center of mass offset
					centerOfMassOffset = Matrix4.Identity;
					if( MotionType.Value == MotionTypeEnum.Dynamic )
					{
						if( CenterOfMassManual )
							centerOfMassOffset = Matrix4.FromTranslate( CenterOfMassPosition.Value );
						else
						{
							Vector3 totalWeighted = Vector3.Zero;
							double totalVolume = 0;

							foreach( var tuple in componentShapes )
							{
								var componentShape = tuple.Item1;
								var transformShape = tuple.Item2;

								if( !transformShape.Decompose( out Vector3 pos, out Matrix3 rot, out Vector3 scl ) )
								{
									//!!!!
								}

								var shapeCenterOfMassPosition = new Matrix4( rot, pos ) * componentShape.GetCenterOfMassPositionNotScaledByParent();
								var shapeVolume = componentShape.VolumeNotScaledByParent * scl.X * scl.Y * scl.Z;

								totalWeighted += shapeCenterOfMassPosition * shapeVolume;
								totalVolume += shapeVolume;
							}

							var centerOfMassPosition = totalWeighted / totalVolume;
							centerOfMassOffset = Matrix4.FromTranslate( centerOfMassPosition );

							//centerOfMassOffset = Mat4.FromTranslate( BulletUtils.Convert( BulletUtils.GetCenterOfMass( shape ) ) );
						}
						centerOfMassOffsetInverted = centerOfMassOffset.GetInverse();
					}

					Internal.BulletSharp.CollisionShape shape;

					if( needCompound )
					{
						//use CompoundShape

						var compoundShape = new CompoundShape();
						foreach( var tuple in componentShapes )
						{
							var componentShape = tuple.Item1;
							var transformShape = tuple.Item2;

							if( !transformShape.Decompose( out Vector3 pos, out Matrix3 rot, out Vector3 scl ) )
							{
								//!!!!
							}

							var shape2 = componentShape.CreateShape();
							if( shape2 != null )
							{
								shape2.UserObject = componentShape;
								shape2.LocalScaling = BulletPhysicsUtility.Convert( scl );

								var tr = new Matrix4( rot, pos );
								if( MotionType.Value == MotionTypeEnum.Dynamic )
									tr *= centerOfMassOffsetInverted;

								compoundShape.AddChildShape( BulletPhysicsUtility.Convert( tr ), shape2 );
							}
						}

						//no children
						if( compoundShape.NumChildShapes == 0 )
						{
							compoundShape.Dispose();
							compoundShape = null;
						}

						shape = compoundShape;
					}
					//!!!!new
					else
					{
						//no CompoundShape

						var tuple = componentShapes[ 0 ];
						var componentShape = tuple.Item1;
						var transformShape = tuple.Item2;

						transformShape.Decompose( out var pos, out Matrix3 rot, out var scl );

						shape = componentShape.CreateShape();
						if( shape != null )
						{
							shape.UserObject = componentShape;
							shape.LocalScaling = BulletPhysicsUtility.Convert( scl );
						}
					}

					if( shape != null )
					{
						//use local variable to prevent double update inside properties.
						Internal.BulletSharp.RigidBody body = null;

						if( MotionType.Value == MotionTypeEnum.Dynamic )
						{
							double mass = Mass.Value;

							Internal.BulletSharp.Math.BVector3 localInertia = shape.CalculateLocalInertia( mass ) * BulletPhysicsUtility.Convert( InertiaTensorFactor.Value );

							//!!!!надо ли?
							// Using a motion state is recommended,
							// it provides interpolation capabilities and only synchronizes "active" objects

							var motionState = new DefaultMotionState( BulletPhysicsUtility.Convert( startTransform * centerOfMassOffset ) );
							//var motionState = new DefaultMotionState( BulletUtils.Convert( startTransform * centerOfMassOffset ), BulletUtils.Convert( centerOfMassOffset ) );

							// RigidBody Info
							using( var rbInfo = new RigidBodyConstructionInfo( mass, motionState, shape, localInertia ) )
							{
								body = new Internal.BulletSharp.RigidBody( rbInfo );
							}

							body.SetSleepingThresholds( LinearSleepingThreshold, AngularSleepingThreshold );
							body.SetDamping( LinearDamping, AngularDamping );
							body.LinearVelocity = BulletPhysicsUtility.Convert( LinearVelocity );
							body.AngularVelocity = BulletPhysicsUtility.Convert( AngularVelocity );
							if( !EnableGravity )
								body.Flags |= RigidBodyFlags.DisableWorldGravity;
						}
						else
						{
							using( var rbInfo = new RigidBodyConstructionInfo( 0, null, shape ) )
							{
								rbInfo.StartWorldTransform = BulletPhysicsUtility.Convert( startTransform );
								body = new Internal.BulletSharp.RigidBody( rbInfo );
							}

							if( MotionType.Value == MotionTypeEnum.Kinematic )
								body.CollisionFlags |= CollisionFlags.KinematicObject;
							else
								body.CollisionFlags |= CollisionFlags.StaticObject;
						}

						if( ContactsCollect )
							body.CollisionFlags |= CollisionFlags.CollectContacts;

						SetMaterial( body );


						//!!!!все доступные в BulletSharpInvoke настройки:

						//public float ContactProcessingThreshold { get; set; }
						//public float DeactivationTime { get; set; }
						//public float HitFraction { get; set; }
						//public float CcdSweptSphereRadius { get; set; }
						//public float CcdMotionThreshold { get; set; }
						//public Matrix WorldTransform { get; set; }
						//public ActivationState ActivationState { get; set; }

						//public void SetContactStiffnessAndDamping( float stiffness, float damping );
						//public void SetCustomDebugColor( Vector3 colorRgb );
						//public void SetIgnoreCollisionCheck( CollisionObject co, bool ignoreCollisionCheck );

						//!!!!new
						collisionShape = shape;

						rigidBody = body;
						rigidBody.UserObject = this;

						physicsWorldData.world.AddRigidBody( rigidBody, CollisionGroup.Value, CollisionMask.Value );
						rigidBodyCreatedTransformScale = bodyTransform.Scale;
					}
				}

				//!!!!
				SpaceBoundsUpdate();

				if( rigidBody != null )
					UpdateCCD();
			}

			duringCreateDestroy = false;
		}

		List<Constraint> GetLinkedCreatedConstraints()
		{
			var list = new List<Constraint>( rigidBody.NumConstraintRefs );
			for( int n = 0; n < rigidBody.NumConstraintRefs; n++ )
			{
				var c = rigidBody.GetConstraintRef( n );
				var c2 = c.Userobject as Constraint;
				if( c2 != null )
					list.Add( c2 );
			}
			return list;
		}

		void DestroyBody()
		{
			duringCreateDestroy = true;

			var physicsWorldData = GetPhysicsWorldData();
			if( physicsWorldData != null )
			{
				//!!!!что еще удал€ть?
				//!!!!правильно ли тут всЄ?

				//destroy linked constraints
				{
					if( rigidBody != null && rigidBody.NumConstraintRefs != 0 )
					{
						foreach( var c in GetLinkedCreatedConstraints() )
							c.DestroyConstraint();
					}

					////!!!!что еще также? и где?
					//if( rigidBody != null )
					//{
					//	//var cachedArray = actionsBeforeBodyDestroy.ToArray();
					//	//foreach( var action in cachedArray )
					//	//	action();
					//}
					//actionsBeforeBodyDestroy.Clear();
				}


				if( rigidBody != null )
				{
					centerOfMassOffset = Matrix4.Identity;
					centerOfMassOffsetInverted = Matrix4.Identity;

					rigidBodyCreatedTransformScale = Vector3.Zero;

					//!!!!slowly
					physicsWorldData.world.RemoveRigidBody( rigidBody );

					if( rigidBody.MotionState != null )
						rigidBody.MotionState.Dispose();
					rigidBody.Dispose();
					rigidBody = null;
				}

				if( collisionShape != null )
				{
					//!!!!need?
					var compound = collisionShape as CompoundShape;
					if( compound != null )
					{
						for( int n = 0; n < compound.NumChildShapes; n++ )
						{
							var child = compound.GetChildShape( n );
							child.Dispose();

							var meshShape = child as BvhTriangleMeshShape;
							meshShape?.MeshInterface?.Dispose();
						}
					}
					//!!!!new
					var meshShape2 = collisionShape as BvhTriangleMeshShape;
					meshShape2?.MeshInterface?.Dispose();

					collisionShape.Dispose();
					collisionShape = null;
				}


				//!!!!что отсюда?

				//var shapes = new HashSet<CollisionShape>();

				//for( int i = world.NumCollisionObjects - 1; i >= 0; i-- )
				//{
				//	CollisionObject obj = world.CollisionObjectArray[ i ];
				//	var body = obj as RigidBody;
				//	if( body != null && body.MotionState != null )
				//	{
				//		body.MotionState.Dispose();
				//	}
				//	world.RemoveCollisionObject( obj );
				//	GetShapeWithChildShapes( obj.CollisionShape, shapes );

				//	obj.Dispose();
				//}

				//foreach( var shape in shapes )
				//{
				//	shape.Dispose();
				//}
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
				//после загрузки создаетс€ через шейп, т.к. срабатывает RecreateBody()
				if( rigidBody == null )
					CreateBody();
			}
			else
				DestroyBody();
		}

		static AnisotropicFrictionFlags ConvertFrictionMode( PhysicalMaterial.FrictionModeEnum value )
		{
			switch( value )
			{
			case PhysicalMaterial.FrictionModeEnum.Simple:
				return AnisotropicFrictionFlags.FrictionDisabled;
			case PhysicalMaterial.FrictionModeEnum.Anisotropic:
				return AnisotropicFrictionFlags.Friction;
			case PhysicalMaterial.FrictionModeEnum.AnisotropicRolling:
				return AnisotropicFrictionFlags.RollingFriction;
			}
			return 0;
		}

		void SetMaterial( Internal.BulletSharp.RigidBody b )
		{
			//material settings
			PhysicalMaterial mat = Material;
			if( mat != null )
			{
				var mode = mat.FrictionMode.Value;

				if( mode == PhysicalMaterial.FrictionModeEnum.Simple )
				{
					b.SetAnisotropicFriction( Internal.BulletSharp.Math.BVector3.One, AnisotropicFrictionFlags.FrictionDisabled );
					b.RollingFriction = 0;// default value
					b.SpinningFriction = 0;// default value
				}
				else
				{
					b.RollingFriction = mat.RollingFriction;
					b.SpinningFriction = mat.SpinningFriction;
					//dir = b.CollisionShape.AnisotropicRollingFrictionDirection;

					b.SetAnisotropicFriction( BulletPhysicsUtility.Convert( mat.AnisotropicFriction.Value ), ConvertFrictionMode( mode ) );
				}

				b.Friction = mat.Friction;
				b.Restitution = mat.RigidRestitution;
			}
			else
			{
				var mode = MaterialFrictionMode.Value;

				if( mode == PhysicalMaterial.FrictionModeEnum.Simple )
				{
					b.SetAnisotropicFriction( Internal.BulletSharp.Math.BVector3.One, AnisotropicFrictionFlags.FrictionDisabled );
					b.RollingFriction = 0;// default value
					b.SpinningFriction = 0;// default value
				}
				else
				{
					b.RollingFriction = MaterialRollingFriction;
					b.SpinningFriction = MaterialSpinningFriction;
					//dir = b.CollisionShape.AnisotropicRollingFrictionDirection;

					b.SetAnisotropicFriction( BulletPhysicsUtility.Convert( MaterialAnisotropicFriction.Value ), ConvertFrictionMode( mode ) );
				}

				b.Friction = MaterialFriction;
				b.Restitution = MaterialRestitution;
			}
		}

		public override void UpdateDataFromPhysicsEngine()
		{
			//!!!!когда не вызывать?

			//if( ContactsData != null )
			//	ContactsData.Clear();

			if( MotionType.Value == MotionTypeEnum.Dynamic && rigidBody != null )
			{
				rigidBody.GetWorldTransform( out var bulletT );

				Matrix4 tr;
				BulletPhysicsUtility.Convert( ref bulletT, out tr );
				tr *= centerOfMassOffsetInverted;

				tr.Decompose( out Vector3 pos, out Quaternion rot, out Vector3 scl );
				var oldT = Transform.Value;
				//!!!!scale?
				var newT = new Transform( pos, rot, oldT.Scale );


				//bulletT = BulletUtils.Convert( centerOfMassOffsetInverted ) * bulletT;
				//bulletT.Decompose( out Vector3 scale, out Quaternion rotation, out Vector3 translation );
				//var oldT = Transform.Value;
				////!!!!scale?
				//var newT = new Transform( BulletUtils.Convert( translation ), BulletUtils.Convert( rotation ), oldT.Scale );

				//!!!!

				try
				{
					updatePropertiesWithoutUpdatingBody = true;
					Transform = newT;
					LinearVelocity = BulletPhysicsUtility.Convert( rigidBody.LinearVelocity );
					AngularVelocity = BulletPhysicsUtility.Convert( rigidBody.AngularVelocity );
				}
				finally
				{
					updatePropertiesWithoutUpdatingBody = false;
				}
			}
		}

		//public void AddActionBeforeBodyDestroy( Action action )
		//{
		//	actionsBeforeBodyDestroy.Add( action );
		//}

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

				var item = new PhysicsRayTestItem( context.ray, CollisionGroup.Value, -1, PhysicsRayTestItem.ModeEnum.OneClosest, rigidBody );
				ParentScene.PhysicsRayTest( item );
				if( item.Result.Length != 0 )
					context.thisObjectResultRayScale = item.Result[ 0 ].DistanceScale;
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

		public override void Render( ViewportRenderingContext context, out int verticesRendered )
		{
			verticesRendered = 0;

			var context2 = context.ObjectInSpaceRenderingContext;

			var scene = ParentScene;
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
						color = ProjectSettings.Get.General.SceneShowPhysicsStaticColor;
					else if( rigidBody.IsActive )
						color = ProjectSettings.Get.General.SceneShowPhysicsDynamicActiveColor;
					else
						color = ProjectSettings.Get.General.SceneShowPhysicsDynamicInactiveColor;
					viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.General.HiddenByOtherObjectsColorMultiplier );

					//!!!!checkChildren?
					foreach( var shape in GetComponents<CollisionShape>( false, true, true ) )
						shape.Render( viewport, Transform, false, ref verticesRendered );

					//center of mass
					if( MotionType.Value == MotionTypeEnum.Dynamic )
					{
						var center = rigidBody.CenterOfMassPosition;
						double radius = SpaceBounds.CalculatedBoundingSphere.Radius / 16;

						var item = GetCenterOfMassGeometry( (float)radius );
						var transform = Matrix4.FromTranslate( BulletPhysicsUtility.Convert( center ) );
						context.Owner.Simple3DRenderer.AddTriangles( item.positions, item.indices, ref transform, false, true );
						//context.viewport.Simple3DRenderer.AddSphere( BulletPhysicsUtility.Convert( center ), radius, 16, true );

						//Vector3 center = Vector3.Zero;
						//double radius = 1.0;
						//rigidBody.CollisionShape.GetBoundingSphere( out center, out radius );
						//center = rigidBody.CenterOfMassPosition;
						//radius /= 16.0;
						//context.viewport.DebugGeometry.AddSphere( BulletUtils.Convert( center ), radius );

						verticesRendered += item.positions.Length;
					}

					//ccd sphere radius
					if( CCD && MotionType.Value == MotionTypeEnum.Dynamic )
					{
						context.Owner.Simple3DRenderer.AddSphere( new Sphere( TransformV.Position, rigidBody.CcdSweptSphereRadius ), 16, false );
						verticesRendered += 16 * 3 * 8;
					}
				}

				//!!!!copy code
				//draw selection
				if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
				{
					ColorValue color;
					if( context2.selectedObjects.Contains( this ) )
						color = ProjectSettings.Get.General.SelectedColor;
					else if( context2.canSelectObjects.Contains( this ) )
						color = ProjectSettings.Get.General.CanSelectColor;
					else
						color = ProjectSettings.Get.General.SceneShowPhysicsDynamicActiveColor;

					//!!!!или невидимое лучше габаритами подсвечивать?

					color.Alpha *= .5f;
					viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.General.HiddenByOtherObjectsColorMultiplier );

					foreach( var shape in GetComponents<CollisionShape>( false, true, true ) )
						shape.Render( viewport, Transform, true, ref verticesRendered );

					//context.viewport.DebugGeometry.AddBounds( SpaceBounds.CalculatedBoundingBox );
				}

				//display collision contacts
				if( ContactsCollect && ContactsDisplay && ContactsData != null && ContactsData.Count != 0 )
				{
					var size3 = SpaceBounds.CalculatedBoundingBox.GetSize();
					var scale = (float)Math.Min( size3.X, Math.Min( size3.Y, size3.Z ) ) / 30;

					renderer.SetColor( new ColorValue( 1, 0, 0 ) );

					var data = ContactsData;
					for( int n = 0; n < data.Count; n++ )
					{
						ref var item = ref data.Data[ n ];
						if( item.Distance < 0 && item.SimulationSubStep == 0 )
						{
							var pos = item.PositionWorldOnA;
							var bounds = new Bounds(
								pos - new Vector3( scale, scale, scale ),
								pos + new Vector3( scale, scale, scale ) );

							renderer.AddBounds( bounds, true );
						}
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

				//var scene = ParentScene;
				//bool show = ( scene.GetDisplayDevelopmentDataInThisApplication() && scene.DisplayPhysicalObjects ) ||
				//	context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.dragDropCreateObject == this;
				//if( show && rigidBody != null && context.Owner.Simple3DRenderer != null )
				//{
				//	if( context2.displayPhysicalObjectsCounter < context2.displayPhysicalObjectsMax )
				//	{
				//		context2.displayPhysicalObjectsCounter++;

				//		//!!!!выделенный еще. шейпы тоже

				//		//draw body
				//		{
				//			var viewport = context.Owner;

				//			ColorValue color;
				//			if( MotionType.Value == MotionTypeEnum.Static )
				//				color = ProjectSettings.Get.SceneShowPhysicsStaticColor;
				//			else if( rigidBody.IsActive )
				//				color = ProjectSettings.Get.SceneShowPhysicsDynamicActiveColor;
				//			else
				//				color = ProjectSettings.Get.SceneShowPhysicsDynamicInactiveColor;
				//			viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

				//			foreach( var shape in GetComponents<CollisionShape>( false, true, true ) )
				//				shape.DebugRender( viewport, Transform, false );

				//			//center of mass
				//			if( MotionType.Value == MotionTypeEnum.Dynamic )
				//			{
				//				var center = rigidBody.CenterOfMassPosition;
				//				double radius = SpaceBounds.CalculatedBoundingSphere.Radius / 16;

				//				var item = GetCenterOfMassGeometry( (float)radius );
				//				var transform = Matrix4.FromTranslate( BulletPhysicsUtility.Convert( center ) );
				//				context.Owner.Simple3DRenderer.AddTriangles( item.positions, item.indices, ref transform, false, true );
				//				//context.viewport.Simple3DRenderer.AddSphere( BulletPhysicsUtility.Convert( center ), radius, 16, true );

				//				//Vector3 center = Vector3.Zero;
				//				//double radius = 1.0;
				//				//rigidBody.CollisionShape.GetBoundingSphere( out center, out radius );
				//				//center = rigidBody.CenterOfMassPosition;
				//				//radius /= 16.0;
				//				//context.viewport.DebugGeometry.AddSphere( BulletUtils.Convert( center ), radius );
				//			}
				//		}

				//		//!!!!copy code
				//		//draw selection
				//		if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
				//		{
				//			ColorValue color;
				//			if( context2.selectedObjects.Contains( this ) )
				//				color = ProjectSettings.Get.SelectedColor;
				//			else if( context2.canSelectObjects.Contains( this ) )
				//				color = ProjectSettings.Get.CanSelectColor;
				//			else
				//				color = ProjectSettings.Get.SceneShowPhysicsDynamicActiveColor;

				//			var viewport = context.Owner;

				//			//!!!!или невидимое лучше габаритами подсвечивать?

				//			color.Alpha *= .5f;
				//			viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

				//			foreach( var shape in GetComponents<CollisionShape>( false, true, true ) )
				//				shape.DebugRender( viewport, Transform, true );

				//			//context.viewport.DebugGeometry.AddBounds( SpaceBounds.CalculatedBoundingBox );
				//		}
				//	}
				//}
				var showLabels = /*show &&*/ rigidBody == null;
				if( !showLabels )
					context2.disableShowingLabelForThisObject = true;
			}
		}

		void UpdateCCD()
		{
			if( CCD && MotionType.Value == MotionTypeEnum.Dynamic )
			{
				rigidBody.CcdSweptSphereRadius = CcdSweptSphereRadius * rigidBodyCreatedTransformScale.MaxComponent();
				rigidBody.CcdMotionThreshold = CcdMotionThreshold;

				//double radius = SpaceBounds.CalculatedBoundingSphere.Radius;
				////double radius = 0.5 * ( SpaceBounds.CalculatedBoundingBox.Maximum - SpaceBounds.CalculatedBoundingBox.Minimum ).Length();
				//rigidBody.CcdSweptSphereRadius = radius * CcdSweptSphereRadiusFactor;
				//rigidBody.CcdMotionThreshold = CcdMotionThreshold;
			}
			else
			{
				rigidBody.CcdSweptSphereRadius = 0;
				rigidBody.CcdMotionThreshold = 0;
			}
		}

		[Browsable( false )]
		public override CollisionObject BulletBody
		{
			get { return rigidBody; }
		}

		[Browsable( false )]
		public bool Active
		{
			get
			{
				if( rigidBody != null )
					return rigidBody.IsActive;
				return false;
			}
		}

		public void Activate()
		{
			if( rigidBody != null )
			{
				rigidBody.ForceActivationState( ActivationState.ActiveTag );
				rigidBody.Activate();
			}
		}

		public void WantsDeactivation()
		{
			rigidBody?.ForceActivationState( ActivationState.WantsDeactivation );
		}

		public void ApplyForce( Vector3 force, Vector3 relativePosition )
		{
			if( rigidBody != null && MotionType.Value == MotionTypeEnum.Dynamic && force != Vector3.Zero )
			{
				Activate();

				BulletPhysicsUtility.Convert( ref force, out var bForce );
				BulletPhysicsUtility.Convert( ref relativePosition, out var bRelPos );
				rigidBody?.ApplyForceRef( ref bForce, ref bRelPos );
			}
		}
	}
}
