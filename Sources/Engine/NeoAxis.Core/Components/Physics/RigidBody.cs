// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace NeoAxis
{
	/// <summary>
	/// A rigid physical body.
	/// </summary>
	public class RigidBody : PhysicalBody
	{
		//!!!!хранить тут как можно меньше

		Scene.PhysicsWorldClass.Body physicalBody;
		Vector3 physicalBodyCreatedTransformScale;

		//internal CollisionShape[] collisionShapeByIndex;

		bool duringCreateDestroy;
		bool updatePropertiesWithoutUpdatingBody;

		//!!!!need?
		//Matrix4 centerOfMassOffset;
		//Matrix4 centerOfMassOffsetInverted;

		//!!!!
		class CenterOfMassGeometry
		{
			public float radius;
			public Vector3F[] positions;
			public int[] indices;
		}
		static List<CenterOfMassGeometry> centerOfMassGeometryCache = new List<CenterOfMassGeometry>();

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// The type of motion used.
		/// </summary>
		[DefaultValue( PhysicsMotionType.Static )]
		[Category( "Rigid Body" )]
		public Reference<PhysicsMotionType> MotionType
		{
			get { if( _motionType.BeginGet() ) MotionType = _motionType.Get( this ); return _motionType.value; }
			set
			{
				if( _motionType.BeginSet( ref value ) )
				{
					try
					{
						MotionTypeChanged?.Invoke( this );

						//!!!!обязательно ли пересоздавать? констрейнты?
						if( physicalBody != null )
							RecreateBody();
					}
					finally { _motionType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MotionType"/> property value changes.</summary>
		public event Action<RigidBody> MotionTypeChanged;
		ReferenceField<PhysicsMotionType> _motionType = PhysicsMotionType.Static;

		/// <summary>
		/// The method of detecting collisions to solve issues on a high velocity.
		/// </summary>
		[DefaultValue( PhysicsMotionQuality.Discrete )]
		[Category( "Rigid Body" )]
		public Reference<PhysicsMotionQuality> MotionQuality
		{
			get { if( _motionQuality.BeginGet() ) MotionQuality = _motionQuality.Get( this ); return _motionQuality.value; }
			set
			{
				if( _motionQuality.BeginSet( ref value ) )
				{
					try
					{
						MotionQualityChanged?.Invoke( this );
						if( physicalBody != null )
							physicalBody.MotionQuality = MotionQuality;
					}
					finally { _motionQuality.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MotionQuality"/> property value changes.</summary>
		public event Action<RigidBody> MotionQualityChanged;
		ReferenceField<PhysicsMotionQuality> _motionQuality = PhysicsMotionQuality.Discrete;

		/// <summary>
		/// The mass of the rigid body.
		/// </summary>
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

						//!!!!обязательно ли пересоздавать? констрейнты?
						if( physicalBody != null )
							RecreateBody();
					}
					finally { _mass.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mass"/> property value changes.</summary>
		public event Action<RigidBody> MassChanged;
		ReferenceField<double> _mass = 1;

		/// <summary>
		/// The factor of affect global gravity.
		/// </summary>
		[Category( "Rigid Body" )]
		[DefaultValue( 1.0 )]
		[Range( 0, 2 )]
		public Reference<double> GravityFactor
		{
			get { if( _gravityFactor.BeginGet() ) GravityFactor = _gravityFactor.Get( this ); return _gravityFactor.value; }
			set
			{
				if( _gravityFactor.BeginSet( ref value ) )
				{
					try
					{
						GravityFactorChanged?.Invoke( this );
						if( physicalBody != null )
							physicalBody.GravityFactor = (float)GravityFactor;
					}
					finally { _gravityFactor.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="GravityFactor"/> property value changes.</summary>
		public event Action<RigidBody> GravityFactorChanged;
		ReferenceField<double> _gravityFactor = 1.0;

		///// <summary>
		///// Whether the rigid body is affected by the gravity.
		///// </summary>
		//[DefaultValue( true )]
		//[Category( "Rigid Body" )]
		//public Reference<bool> EnableGravity
		//{
		//	get { if( _enableGravity.BeginGet() ) EnableGravity = _enableGravity.Get( this ); return _enableGravity.value; }
		//	set
		//	{
		//		if( _enableGravity.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				EnableGravityChanged?.Invoke( this );
		//				if( rigidBody != null )
		//				{
		//					impl
		//					//if( !EnableGravity )
		//					//	rigidBody.Flags |= RigidBodyFlags.DisableWorldGravity;
		//					//else
		//					//	rigidBody.Flags &= ~RigidBodyFlags.DisableWorldGravity;
		//				}
		//			}
		//			finally { _enableGravity.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="EnableGravity"/> property value changes.</summary>
		//public event Action<RigidBody> EnableGravityChanged;
		//ReferenceField<bool> _enableGravity = true;

		/// <summary>
		/// The linear reduction of velocity over time.
		/// </summary>
		[DefaultValue( 0.05 )]//1 )]
		[Range( 0, 1 )]
		[Category( "Rigid Body" )]
		public Reference<double> LinearDamping
		{
			get { if( _linearDamping.BeginGet() ) LinearDamping = _linearDamping.Get( this ); return _linearDamping.value; }
			set
			{
				if( _linearDamping.BeginSet( ref value ) )
				{
					try
					{
						LinearDampingChanged?.Invoke( this );
						if( physicalBody != null )
							physicalBody.LinearDamping = (float)value;
					}
					finally { _linearDamping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearDamping"/> property value changes.</summary>
		public event Action<RigidBody> LinearDampingChanged;
		ReferenceField<double> _linearDamping = 0.05;//0.1;

		/// <summary>
		/// The angular reduction of velocity over time.
		/// </summary>
		[DefaultValue( 0.05 )] //0.1
		[Range( 0, 1 )]
		[Category( "Rigid Body" )]
		public Reference<double> AngularDamping
		{
			get { if( _angularDamping.BeginGet() ) AngularDamping = _angularDamping.Get( this ); return _angularDamping.value; }
			set
			{
				if( _angularDamping.BeginSet( ref value ) )
				{
					try
					{
						AngularDampingChanged?.Invoke( this );
						if( physicalBody != null )
							physicalBody.AngularDamping = (float)value;
					}
					finally { _angularDamping.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularDamping"/> property value changes.</summary>
		public event Action<RigidBody> AngularDampingChanged;
		ReferenceField<double> _angularDamping = 0.05;//0.1;

		/// <summary>
		/// The physical material used by the rigid body.
		/// </summary>
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
						if( physicalBody != null )
							SetMaterial( physicalBody );
					}
					finally { _material.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<RigidBody> MaterialChanged;
		ReferenceField<PhysicalMaterial> _material;

		//!!!!
		///// <summary>
		///// The type of friction applied on the rigid body.
		///// </summary>
		//[DefaultValue( PhysicalMaterial.FrictionModeEnum.Simple )]
		//[Category( "Rigid Body" )]
		//public Reference<PhysicalMaterial.FrictionModeEnum> MaterialFrictionMode
		//{
		//	get { if( _materialFrictionMode.BeginGet() ) MaterialFrictionMode = _materialFrictionMode.Get( this ); return _materialFrictionMode.value; }
		//	set
		//	{
		//		if( _materialFrictionMode.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				MaterialFrictionModeChanged?.Invoke( this );
		//				if( rigidBody != null )
		//					SetMaterial( rigidBody );
		//			}
		//			finally { _materialFrictionMode.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="MaterialFrictionMode"/> property value changes.</summary>
		//public event Action<RigidBody> MaterialFrictionModeChanged;
		//ReferenceField<PhysicalMaterial.FrictionModeEnum> _materialFrictionMode = PhysicalMaterial.FrictionModeEnum.Simple;

		/// <summary>
		/// The amount of friction applied on the rigid body.
		/// </summary>
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
						if( physicalBody != null )
							SetMaterial( physicalBody );
					}
					finally { _materialFriction.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialFriction"/> property value changes.</summary>
		public event Action<RigidBody> MaterialFrictionChanged;
		ReferenceField<double> _materialFriction = 0.5;

		//!!!!
		///// <summary>
		///// The amount of directional friction applied on the rigid body.
		///// </summary>
		//[DefaultValue( "1 1 1" )]
		////[ApplicableRange( 0, 1 )]
		//[Category( "Rigid Body" )]
		//public Reference<Vector3> MaterialAnisotropicFriction
		//{
		//	get { if( _materialAnisotropicFriction.BeginGet() ) MaterialAnisotropicFriction = _materialAnisotropicFriction.Get( this ); return _materialAnisotropicFriction.value; }
		//	set
		//	{
		//		if( _materialAnisotropicFriction.BeginSet( ref value ) )
		//		{
		//			try { MaterialAnisotropicFrictionChanged?.Invoke( this ); }
		//			finally { _materialAnisotropicFriction.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AnisotropicFriction"/> property value changes.</summary>
		//public event Action<RigidBody> MaterialAnisotropicFrictionChanged;
		//ReferenceField<Vector3> _materialAnisotropicFriction = Vector3.One;

		//!!!!
		///// <summary>
		///// The amount of friction applied when rigid body is spinning.
		///// </summary>
		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//[Category( "Rigid Body" )]
		//public Reference<double> MaterialSpinningFriction
		//{
		//	get { if( _materialSpinningFriction.BeginGet() ) MaterialSpinningFriction = _materialSpinningFriction.Get( this ); return _materialSpinningFriction.value; }
		//	set
		//	{
		//		if( _materialSpinningFriction.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				MaterialSpinningFrictionChanged?.Invoke( this );
		//				if( rigidBody != null )
		//					SetMaterial( rigidBody );
		//			}
		//			finally { _materialSpinningFriction.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="MaterialSpinningFriction"/> property value changes.</summary>
		//public event Action<RigidBody> MaterialSpinningFrictionChanged;
		//ReferenceField<double> _materialSpinningFriction = 0.5;

		//!!!!
		///// <summary>
		///// The amount of friction applied when rigid body is rolling.
		///// </summary>
		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//[Category( "Rigid Body" )]
		//public Reference<double> MaterialRollingFriction
		//{
		//	get { if( _materialRollingFriction.BeginGet() ) MaterialRollingFriction = _materialRollingFriction.Get( this ); return _materialRollingFriction.value; }
		//	set
		//	{
		//		if( _materialRollingFriction.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				MaterialRollingFrictionChanged?.Invoke( this );
		//				if( rigidBody != null )
		//					SetMaterial( rigidBody );
		//			}
		//			finally { _materialRollingFriction.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="MaterialRollingFriction"/> property value changes.</summary>
		//public event Action<RigidBody> MaterialRollingFrictionChanged;
		//ReferenceField<double> _materialRollingFriction = 0.5;

		/// <summary>
		/// The ratio of the final relative velocity to initial relative velocity of the rigid body after collision.
		/// </summary>
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
						//!!!!
						if( physicalBody != null )//&& Material.Value == null )
						{
							SetMaterial( physicalBody );
							//rigidBody.Restitution = (float)MaterialRestitution;
						}

						//if( rigidBody != null && Material.Value == null )
						//{
						//rigidBody.Restitution = (float)MaterialRestitution;
						//}
					}
					finally { _materialRestitution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaterialRestitution"/> property value changes.</summary>
		public event Action<RigidBody> MaterialRestitutionChanged;
		ReferenceField<double> _materialRestitution;

		//!!!!impl
		///// <summary>
		///// Whether the rigid body is using manual center of mass.
		///// </summary>
		//[DefaultValue( false )]
		//[Category( "Rigid Body" )]
		//public Reference<bool> CenterOfMassManual
		//{
		//	get { if( _centerOfMassManual.BeginGet() ) CenterOfMassManual = _centerOfMassManual.Get( this ); return _centerOfMassManual.value; }
		//	set
		//	{
		//		if( _centerOfMassManual.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CenterOfMassManualChanged?.Invoke( this );

		//				//!!!!обязательно ли пересоздавать?
		//				if( rigidBody != null )
		//					RecreateBody();
		//			}
		//			finally { _centerOfMassManual.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="CenterOfMassManual"/> property value changes.</summary>
		//public event Action<RigidBody> CenterOfMassManualChanged;
		//ReferenceField<bool> _centerOfMassManual = false;

		//!!!!impl
		///// <summary>
		///// The position of center of mass.
		///// </summary>
		//[DefaultValue( "0 0 0" )]
		//[Category( "Rigid Body" )]
		//public Reference<Vector3> CenterOfMassPosition
		//{
		//	get { if( _centerOfMassPosition.BeginGet() ) CenterOfMassPosition = _centerOfMassPosition.Get( this ); return _centerOfMassPosition.value; }
		//	set
		//	{
		//		if( _centerOfMassPosition.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CenterOfMassPositionChanged?.Invoke( this );

		//				//!!!!обязательно ли пересоздавать? где еще такое
		//				if( rigidBody != null )
		//					RecreateBody();
		//			}
		//			finally { _centerOfMassPosition.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="CenterOfMassPosition"/> property value changes.</summary>
		//public event Action<RigidBody> CenterOfMassPositionChanged;
		//ReferenceField<Vector3> _centerOfMassPosition = Vector3.Zero;

		//!!!!impl
		///// <summary>
		///// The moment of inertia.
		///// </summary>
		//[DefaultValue( "1 1 1" )]
		//[Category( "Rigid Body" )]
		//public Reference<Vector3> InertiaTensorFactor
		//{
		//	get { if( _inertiaTensorFactor.BeginGet() ) InertiaTensorFactor = _inertiaTensorFactor.Get( this ); return _inertiaTensorFactor.value; }
		//	set
		//	{
		//		if( _inertiaTensorFactor.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				InertiaTensorFactorChanged?.Invoke( this );

		//				//!!!!обязательно ли пересоздавать?

		//				//!!!!new, вроде как не работает
		//				//if( rigidBody != null )
		//				//{
		//				//	rigidBody.SetMassProps( Mass.Value, rigidBody.CollisionShape.CalculateLocalInertia( Mass.Value ) * BulletUtils.Convert( value ) );
		//				//	rigidBody.UpdateInertiaTensor();
		//				//}
		//				if( rigidBody != null )
		//					RecreateBody();
		//			}
		//			finally { _inertiaTensorFactor.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="InertiaTensorFactor"/> property value changes.</summary>
		//public event Action<RigidBody> InertiaTensorFactorChanged;
		//ReferenceField<Vector3> _inertiaTensorFactor = Vector3.One;

		//!!!!need?

		///// <summary>
		///// The linear velocity threshold below which the body will stop movement.
		///// </summary>
		//[DefaultValue( 0.5 )]//1.0 )]
		//[Category( "Rigid Body" )]
		//[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> LinearSleepingThreshold
		//{
		//	get { if( _linearSleepingThreshold.BeginGet() ) LinearSleepingThreshold = _linearSleepingThreshold.Get( this ); return _linearSleepingThreshold.value; }
		//	set
		//	{
		//		if( _linearSleepingThreshold.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				LinearSleepingThresholdChanged?.Invoke( this );

		//				//!!!!impl

		//				//rigidBody?.SetSleepingThresholds( value, AngularSleepingThreshold.Value );
		//			}
		//			finally { _linearSleepingThreshold.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="LinearSleepingThreshold"/> property value changes.</summary>
		//public event Action<RigidBody> LinearSleepingThresholdChanged;
		//ReferenceField<double> _linearSleepingThreshold = 0.5;

		///// <summary>
		///// The angular velocity threshold below which the body will stop rotating.
		///// </summary>
		//[DefaultValue( 0.5 )]//0.8 )]
		//[Category( "Rigid Body" )]
		//[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> AngularSleepingThreshold
		//{
		//	get { if( _angularSleepingThreshold.BeginGet() ) AngularSleepingThreshold = _angularSleepingThreshold.Get( this ); return _angularSleepingThreshold.value; }
		//	set
		//	{
		//		if( _angularSleepingThreshold.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				AngularSleepingThresholdChanged?.Invoke( this );

		//				//!!!!impl


		//				//rigidBody?.SetSleepingThresholds( LinearSleepingThreshold.Value, value );
		//			}
		//			finally { _angularSleepingThreshold.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="AngularSleepingThreshold"/> property value changes.</summary>
		//public event Action<RigidBody> AngularSleepingThresholdChanged;
		//ReferenceField<double> _angularSleepingThreshold = 0.5;


		//!!!!сделать как в 2D?

		////!!!!name: ContactGroup, ContactMask
		///// <summary>
		///// The collision filtering group.
		///// </summary>
		//[DefaultValue( 1 )]
		//[Category( "Collision Filtering" )]
		//public Reference<int> CollisionGroup
		//{
		//	get { if( _collisionGroup.BeginGet() ) CollisionGroup = _collisionGroup.Get( this ); return _collisionGroup.value; }
		//	set
		//	{
		//		if( _collisionGroup.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CollisionGroupChanged?.Invoke( this );
		//				if( rigidBody != null )
		//					RecreateBody();
		//			}
		//			finally { _collisionGroup.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="CollisionGroup"/> property value changes.</summary>
		//public event Action<RigidBody> CollisionGroupChanged;
		//ReferenceField<int> _collisionGroup = 1;

		///// <summary>
		///// The collision filtering mask.
		///// </summary>
		//[DefaultValue( 1 )]
		//[Category( "Collision Filtering" )]
		//public Reference<int> CollisionMask
		//{
		//	get { if( _collisionMask.BeginGet() ) CollisionMask = _collisionMask.Get( this ); return _collisionMask.value; }
		//	set
		//	{
		//		if( _collisionMask.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CollisionMaskChanged?.Invoke( this );
		//				if( rigidBody != null )
		//					RecreateBody();
		//			}
		//			finally { _collisionMask.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="CollisionMask"/> property value changes.</summary>
		//public event Action<RigidBody> CollisionMaskChanged;
		//ReferenceField<int> _collisionMask = 1;

		/// <summary>
		/// The initial linear velocity of the body.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Velocity" )]
		public Reference<Vector3> LinearVelocity
		{
			get { if( _linearVelocity.BeginGet() ) LinearVelocity = _linearVelocity.Get( this ); return _linearVelocity.value; }
			set
			{
				if( _linearVelocity.BeginSet( ref value ) )
				{
					try
					{
						LinearVelocityChanged?.Invoke( this );
						if( physicalBody != null && !updatePropertiesWithoutUpdatingBody )
							physicalBody.LinearVelocity = value.Value.ToVector3F();
					}
					finally { _linearVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="LinearVelocity"/> property value changes.</summary>
		public event Action<RigidBody> LinearVelocityChanged;
		ReferenceField<Vector3> _linearVelocity = Vector3.Zero;

		/// <summary>
		/// The initial angular velocity of the body.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Velocity" )]
		public Reference<Vector3> AngularVelocity
		{
			get { if( _angularVelocity.BeginGet() ) AngularVelocity = _angularVelocity.Get( this ); return _angularVelocity.value; }
			set
			{
				if( _angularVelocity.BeginSet( ref value ) )
				{
					try
					{
						AngularVelocityChanged?.Invoke( this );
						if( physicalBody != null && !updatePropertiesWithoutUpdatingBody )
							physicalBody.AngularVelocity = value.Value.ToVector3F();
					}
					finally { _angularVelocity.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AngularVelocity"/> property value changes.</summary>
		public event Action<RigidBody> AngularVelocityChanged;
		ReferenceField<Vector3> _angularVelocity = Vector3.Zero;

		/// <summary>
		/// Whether to display collected collision contacts data.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Contacts" )]
		public Reference<bool> DisplayContacts
		{
			get { if( _displayContacts.BeginGet() ) DisplayContacts = _displayContacts.Get( this ); return _displayContacts.value; }
			set { if( _displayContacts.BeginSet( ref value ) ) { try { DisplayContactsChanged?.Invoke( this ); } finally { _displayContacts.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayContacts"/> property value changes.</summary>
		public event Action<RigidBody> DisplayContactsChanged;
		ReferenceField<bool> _displayContacts = false;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( MotionQuality ):
				case nameof( Mass ):
				//!!!!impl
				//case nameof( CenterOfMassManual ):
				//case nameof( InertiaTensorFactor ):
				//case nameof( LinearSleepingThreshold ):
				//case nameof( AngularSleepingThreshold ):
					//!!!!
					if( MotionType.Value != PhysicsMotionType.Dynamic )
						skip = true;
					break;

				case nameof( GravityFactor ):
				case nameof( LinearDamping ):
				case nameof( AngularDamping ):
					if( MotionType.Value != PhysicsMotionType.Dynamic )
						skip = true;
					break;

				case nameof( LinearVelocity ):
				case nameof( AngularVelocity ):
					if( MotionType.Value == PhysicsMotionType.Static )
						skip = true;
					break;

				//!!!!impl
				//case nameof( CenterOfMassPosition ):
				//	if( MotionType.Value != MotionTypeEnum.Dynamic || !CenterOfMassManual.Value )
				//		skip = true;
				//	break;

				//!!!!
				//case nameof( MaterialFrictionMode ):
				//	if( Material.Value != null )
				//		skip = true;
				//	break;

				case nameof( MaterialFriction ):
					if( Material.Value != null )
						skip = true;
					break;

				//!!!!
				//case nameof( MaterialRollingFriction ):
				//case nameof( MaterialSpinningFriction ):
				//case nameof( MaterialAnisotropicFriction ):
				//	if( MaterialFrictionMode.Value == PhysicalMaterial.FrictionModeEnum.Simple || Material.Value != null )
				//		skip = true;
				//	break;

				case nameof( MaterialRestitution ):
					if( Material.Value != null )
						skip = true;
					break;

					//case nameof( DisplayContacts ):
					//	if( !ContactsCollect )
					//		skip = true;
					//	break;
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[Browsable( false )]
		public Scene.PhysicsWorldClass.Body PhysicalBody
		{
			get { return physicalBody; }
		}

		protected override void OnTransformChanged()
		{
			if( physicalBody != null && !updatePropertiesWithoutUpdatingBody )
			{
				var bodyTransform = Transform.Value;

				//bool alwaysRecreate = false;
				//bool alwaysRecreate = MotionType.Value == MotionTypeEnum.Dynamic;

				if( physicalBodyCreatedTransformScale != bodyTransform.Scale )//|| alwaysRecreate )
				{
					RecreateBody();
				}
				else
				{
					//update transform


					//var t = new Matrix4( bodyTransform.Rotation.ToMatrix3(), bodyTransform.Position );

					//!!!!
					//if( MotionType.Value == MotionTypeEnum.Dynamic )
					//	t *= centerOfMassOffset;


					//!!!!
					var activate = true;

					var pos = bodyTransform.Position;
					var rot = bodyTransform.Rotation.ToQuaternionF();
					physicalBody?.SetTransform( ref pos, ref rot, activate );



					//!!!!impl

					////update constraints
					//if( rigidBody.NumConstraintRefs != 0 )
					//{
					//	foreach( var c in GetLinkedCreatedConstraints() )
					//		c.RecreateConstraint();
					//}




					//var t = new Matrix4( bodyTransform.Rotation.ToMatrix3(), bodyTransform.Position );
					//if( MotionType.Value == MotionTypeEnum.Dynamic )
					//	t *= centerOfMassOffset;

					//BulletPhysicsUtility.Convert( ref t, out var tb );

					//rigidBody.WorldTransform = tb;
					//rigidBody.InterpolationWorldTransform = tb;
					//if( rigidBody.MotionState != null )
					//	rigidBody.MotionState.WorldTransform = tb;

					////GetPhysicsWorldData().world.SynchronizeSingleMotionState( rigidBody );

					////update AABB
					//GetPhysicsWorldData().world.UpdateSingleAabb( rigidBody );

					////GetPhysicsWorldData().world.ComputeOverlappingPairs();

					////update constraints
					//if( rigidBody.NumConstraintRefs != 0 )
					//{
					//	foreach( var c in GetLinkedCreatedConstraints() )
					//		c.RecreateConstraint();
					//}
				}
			}

			base.OnTransformChanged();
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			if( physicalBody != null )
			{
				physicalBody.GetBounds( out var bounds );
				//if( bounds.Minimum.X > bounds.Maximum.X || bounds.Minimum.Y > bounds.Maximum.Y || bounds.Minimum.Z > bounds.Maximum.Z )
				//	Log.Fatal( "bounds" );

				var b = new SpaceBounds( bounds );//, null );
				newBounds = SpaceBounds.Merge( newBounds, b );
			}
		}

		Scene.PhysicsWorldClass GetPhysicsWorldData()
		{
			//!!!!slowly?
			var scene = ParentScene;
			if( scene != null )
				return scene.PhysicsWorld;
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

		////!!!!slowly?
		////!!!!need recursive many levels?
		//static void GetComponentShapesRecursive( CollisionShape shape, Matrix4 shapeTransform, List<(CollisionShape, Matrix4)> result )
		//{
		//	result.Add( (shape, shapeTransform) );

		//	foreach( var child in shape.GetComponents<CollisionShape>( false, false, true ) )
		//	{
		//		var childTransform = shapeTransform * child.TransformRelativeToParent.Value.ToMatrix4();
		//		//var childTransform = child.LocalTransform.Value.ToMat4() * shapeTransform;
		//		GetComponentShapesRecursive( child, childTransform, result );
		//	}
		//}

		void CreateBody()
		{
			if( !CanCreate )
				return;

			duringCreateDestroy = true;

			var physicsWorldData = GetPhysicsWorldData();
			if( physicsWorldData != null )
			{
				if( physicalBody != null )
					Log.Fatal( "RigidBody: CreateBody: physicalBody != null." );
				if( !EnabledInHierarchy )
					Log.Fatal( "RigidBody: CreateBody: !EnabledInHierarchy." );

				//!!!!обновлять положение где-то не только тут. или пересоздавать

				var bodyTransform = Transform.Value;
				//var bodyTransformScaleMatrix = Matrix3.FromScale( bodyTransform.Scale ).ToMatrix4();

				////get shapes. calculate local transforms with applied body scaling.
				//var componentShapes = new List<(CollisionShape, Matrix4)>();
				//foreach( var child in GetComponents<CollisionShape>( false, false, true ) )
				//	GetComponentShapesRecursive( child, bodyTransformScaleMatrix * child.TransformRelativeToParent.Value.ToMatrix4(), componentShapes );

				//if( componentShapes.Count > 0 )
				{

					//!!!!impl
					//bool needCompound = true;

					////!!!!new
					//////!!!!always compound. need compound for shape local transform and when center of mass is not 0,0,0.
					//bool needCompound = false;
					//if( componentShapes.Count > 1 || MotionType.Value == MotionTypeEnum.Dynamic )
					//	needCompound = true;
					//else
					//{
					//	var tuple = componentShapes[ 0 ];
					//	var componentShape = tuple.Item1;
					//	var transformShape = tuple.Item2;

					//	if( !transformShape.Decompose( out Vector3 pos, out Matrix3 rot, out Vector3 scl ) )
					//	{
					//		//!!!!
					//	}

					//	const double zeroTolerance = 1e-6f;
					//	if( !rot.Equals( Matrix3.Identity, zeroTolerance ) || !pos.Equals( Vector3.Zero, zeroTolerance ) )
					//		needCompound = true;
					//}


					////start transform
					////body transform without scaling. Scaling is already applied to shapes.
					//var startTransform = new Matrix4( bodyTransform.Rotation.ToMatrix3(), bodyTransform.Position );

					//!!!!
					////center of mass offset
					//centerOfMassOffset = Matrix4.Identity;
					//if( MotionType.Value == MotionTypeEnum.Dynamic )
					//{
					//	if( CenterOfMassManual )
					//		centerOfMassOffset = Matrix4.FromTranslate( CenterOfMassPosition.Value );
					//	else
					//	{
					//		Vector3 totalWeighted = Vector3.Zero;
					//		double totalVolume = 0;

					//		foreach( var tuple in componentShapes )
					//		{
					//			var componentShape = tuple.Item1;
					//			var transformShape = tuple.Item2;

					//			if( !transformShape.Decompose( out Vector3 pos, out Matrix3 rot, out Vector3 scl ) )
					//			{
					//				//!!!!
					//			}

					//			var shapeCenterOfMassPosition = new Matrix4( rot, pos ) * componentShape.GetCenterOfMassPositionNotScaledByParent();
					//			var shapeVolume = componentShape.VolumeNotScaledByParent * scl.X * scl.Y * scl.Z;

					//			totalWeighted += shapeCenterOfMassPosition * shapeVolume;
					//			totalVolume += shapeVolume;
					//		}

					//		var centerOfMassPosition = totalWeighted / totalVolume;
					//		centerOfMassOffset = Matrix4.FromTranslate( centerOfMassPosition );

					//		//centerOfMassOffset = Mat4.FromTranslate( BulletUtils.Convert( BulletUtils.GetCenterOfMass( shape ) ) );
					//	}
					//	centerOfMassOffsetInverted = centerOfMassOffset.GetInverse();
					//}

					//Internal.BulletSharp.CollisionShape shape;

					var nativeShape = physicsWorldData.AllocateShape( this, bodyTransform.Scale );

					//if( needCompound )
					//{
					//use compound shape

					//nativeShape = physicsWorldData.AllocateShape( this, bodyTransform.Scale );
					//nativeShapeItem = physicsWorldData.AllocateNativeShape( componentShapes );


					//!!!!
					//if( MotionType.Value == MotionTypeEnum.Dynamic )
					//	tr *= centerOfMassOffsetInverted;



					//var compoundShape = new CompoundShape();
					//foreach( var tuple in componentShapes )
					//{
					//	var componentShape = tuple.Item1;
					//	var transformShape = tuple.Item2;

					//	if( !transformShape.Decompose( out Vector3 pos, out Matrix3 rot, out Vector3 scl ) )
					//	{
					//		//!!!!
					//	}

					//	(var shape, var meshShapeCacheItem) = componentShape.CreateShape( physicsWorldData, ref scl );
					//	if( shape != null )
					//	{
					//		//!!!!what to do with scaling and big mesh cache
					//		shape.LocalScaling = BulletPhysicsUtility.Convert( scl );
					//		//shape.UserObject = componentShape;

					//		if( meshShapeCacheItem != null )
					//		{
					//			if( meshShapeCacheItemsToFree == null )
					//				meshShapeCacheItemsToFree = new List<Scene.PhysicsWorldDataClass.MeshShapeCacheItem>();
					//			meshShapeCacheItemsToFree.Add( meshShapeCacheItem );
					//		}
					//		else
					//		{
					//			if( collisionShapesToDispose == null )
					//				collisionShapesToDispose = new List<Internal.BulletSharp.CollisionShape>();
					//			collisionShapesToDispose.Add( shape );
					//		}

					//		//!!!!
					//		//var tr = new Matrix4( rot * Matrix3.FromScale( scl ), pos );
					//		var tr = new Matrix4( rot, pos );
					//		if( MotionType.Value == MotionTypeEnum.Dynamic )
					//			tr *= centerOfMassOffsetInverted;

					//		compoundShape.AddChildShape( BulletPhysicsUtility.Convert( tr ), shape );
					//	}
					//}

					////no children
					//if( compoundShape.NumChildShapes == 0 )
					//{
					//	compoundShape.Dispose();
					//	compoundShape = null;
					//}

					//if( compoundShape != null )
					//{
					//	if( collisionShapesToDispose == null )
					//		collisionShapesToDispose = new List<Internal.BulletSharp.CollisionShape>();
					//	collisionShapesToDispose.Add( compoundShape );

					//	collisionShape = compoundShape;
					//}
					//}
					//else
					//{
					//	//no CompoundShape


					//	//!!!!impl
					//	//If there's only 1 part, we can use a RotatedTranslatedShape instead


					//	//var tuple = componentShapes[ 0 ];
					//	//var componentShape = tuple.Item1;
					//	//var transformShape = tuple.Item2;

					//	//transformShape.Decompose( out var pos, out Matrix3 rot, out var scl );

					//	//(var shape, var meshShapeCacheItem) = componentShape.CreateShape( physicsWorldData, ref scl );
					//	//if( shape != null )
					//	//{
					//	//	//!!!!
					//	//	shape.LocalScaling = BulletPhysicsUtility.Convert( scl );
					//	//	//shape.UserObject = componentShape;

					//	//	if( meshShapeCacheItem != null )
					//	//	{
					//	//		if( meshShapeCacheItemsToFree == null )
					//	//			meshShapeCacheItemsToFree = new List<Scene.PhysicsWorldDataClass.MeshShapeCacheItem>();
					//	//		meshShapeCacheItemsToFree.Add( meshShapeCacheItem );
					//	//	}
					//	//	else
					//	//	{
					//	//		if( collisionShapesToDispose == null )
					//	//			collisionShapesToDispose = new List<Internal.BulletSharp.CollisionShape>();
					//	//		collisionShapesToDispose.Add( shape );
					//	//	}

					//	//	collisionShape = shape;
					//	//}
					//}

					if( nativeShape != null )
					{
						var motionType = MotionType.Value;

						var activate = false;
						if( motionType == PhysicsMotionType.Dynamic )
						{
							//!!!!maybe property bool ActivateOnStart
							activate = true;
						}

						//!!!!
						if( motionType == PhysicsMotionType.Kinematic && ( LinearVelocity.Value != Vector3.Zero || AngularVelocity.Value != Vector3.Zero ) )
						{
							activate = true;
						}

						var pos = bodyTransform.Position;
						var rot = bodyTransform.Rotation.ToQuaternionF();

						//use local variable to prevent double update inside properties
						//!!!!
						var centerOfMassManual = false;//CenterOfMassManual.Value;
						var centerOfMassPosition = Vector3F.Zero;//CenterOfMassPosition.Value.ToVector3F();
						var inertiaTensorFactor = Vector3F.One;//InertiaTensorFactor.Value.ToVector3F();

						var body = physicsWorldData.CreateRigidBody( nativeShape, true, this, motionType, (float)LinearDamping.Value, (float)AngularDamping.Value, ref pos, ref rot, activate, (float)Mass, centerOfMassManual, ref centerOfMassPosition, ref inertiaTensorFactor, MotionQuality.Value );

						if( body != null )
						{
							if( motionType != PhysicsMotionType.Static )
							{
								body.LinearVelocity = LinearVelocity.Value.ToVector3F();
								body.AngularVelocity = AngularVelocity.Value.ToVector3F();
								body.GravityFactor = (float)GravityFactor;
							}

							if( motionType == PhysicsMotionType.Dynamic )
							{

								//!!!!

								//double mass = Mass.Value;
								//var localInertia = collisionShape.CalculateLocalInertia( mass ) * BulletPhysicsUtility.Convert( InertiaTensorFactor.Value );
								//var motionState = new DefaultMotionState( BulletPhysicsUtility.Convert( startTransform * centerOfMassOffset ) );
								//using( var rbInfo = new RigidBodyConstructionInfo( mass, motionState, collisionShape, localInertia ) )
								//	body = new Internal.BulletSharp.RigidBody( rbInfo );


								//!!!!
								//body.SetSleepingThresholds( LinearSleepingThreshold, AngularSleepingThreshold );

							}

							SetMaterial( body );

							physicalBody = body;
							physicalBodyCreatedTransformScale = bodyTransform.Scale;

							//!!!!collision group

							//collisionShapeByIndex = new CollisionShape[ componentShapes.Count ];
							//for( int n = 0; n < componentShapes.Count; n++ )
							//	collisionShapeByIndex[ n ] = componentShapes[ n ].Item1;
						}
					}
					else
						DestroyBody();
				}

				//!!!!
				SpaceBoundsUpdate();
			}

			duringCreateDestroy = false;
		}

		void DestroyBody()
		{
			duringCreateDestroy = true;

			//!!!!что еще удалять?
			//!!!!правильно ли тут всё?

			if( physicalBody != null )
			{
				//!!!!
				//centerOfMassOffset = Matrix4.Identity;
				//centerOfMassOffsetInverted = Matrix4.Identity;

				physicalBodyCreatedTransformScale = Vector3.Zero;
				physicalBody.Dispose();
				physicalBody = null;
			}

			//!!!!
			////free from mesh shape cache
			//if( meshShapeCacheItemsToFree != null )
			//{
			//	foreach( var item in meshShapeCacheItemsToFree )
			//		physicsWorldData.FreeShapeInCache( item );
			//	meshShapeCacheItemsToFree = null;
			//}


			//!!!!
			//collisionShapeByIndex = null;

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
				//после загрузки создается через шейп, т.к. срабатывает RecreateBody()
				if( physicalBody == null )
					CreateBody();
			}
			else
				DestroyBody();
		}

		//!!!!
		//static AnisotropicFrictionFlags ConvertFrictionMode( PhysicalMaterial.FrictionModeEnum value )
		//{
		//	switch( value )
		//	{
		//	case PhysicalMaterial.FrictionModeEnum.Simple:
		//		return AnisotropicFrictionFlags.FrictionDisabled;
		//	case PhysicalMaterial.FrictionModeEnum.Anisotropic:
		//		return AnisotropicFrictionFlags.Friction;
		//	case PhysicalMaterial.FrictionModeEnum.AnisotropicRolling:
		//		return AnisotropicFrictionFlags.RollingFriction;
		//	}
		//	return 0;
		//}

		void SetMaterial( Scene.PhysicsWorldClass.Body b )
		{

			//!!!!когда не нужно всё обновлять


			//material settings
			PhysicalMaterial mat = Material.Value;
			if( mat != null )
			{

				//!!!!impl


				//var mode = mat.FrictionMode.Value;

				//if( mode == PhysicalMaterial.FrictionModeEnum.Simple )
				//{
				//	b.SetAnisotropicFriction( Internal.BulletSharp.Math.BVector3.One, AnisotropicFrictionFlags.FrictionDisabled );
				//	b.RollingFriction = 0;// default value
				//	b.SpinningFriction = 0;// default value
				//}
				//else
				//{
				//	b.RollingFriction = mat.RollingFriction;
				//	b.SpinningFriction = mat.SpinningFriction;
				//	//dir = b.CollisionShape.AnisotropicRollingFrictionDirection;

				//	b.SetAnisotropicFriction( BulletPhysicsUtility.Convert( mat.AnisotropicFriction.Value ), ConvertFrictionMode( mode ) );
				//}

				b.Friction = (float)mat.Friction;
				b.Restitution = (float)mat.RigidRestitution;
				//PhysicsNative.JBody_SetFriction( b, (float)mat.Friction.Value );
				//PhysicsNative.JBody_SetRestitution( b, (float)mat.RigidRestitution.Value );
			}
			else
			{

				//!!!!impl


				//var mode = MaterialFrictionMode.Value;

				//if( mode == PhysicalMaterial.FrictionModeEnum.Simple )
				//{
				//	b.SetAnisotropicFriction( Internal.BulletSharp.Math.BVector3.One, AnisotropicFrictionFlags.FrictionDisabled );
				//	b.RollingFriction = 0;// default value
				//	b.SpinningFriction = 0;// default value
				//}
				//else
				//{
				//	b.RollingFriction = MaterialRollingFriction;
				//	b.SpinningFriction = MaterialSpinningFriction;
				//	//dir = b.CollisionShape.AnisotropicRollingFrictionDirection;

				//	b.SetAnisotropicFriction( BulletPhysicsUtility.Convert( MaterialAnisotropicFriction.Value ), ConvertFrictionMode( mode ) );
				//}

				b.Friction = (float)MaterialFriction;
				b.Restitution = (float)MaterialRestitution;
				//PhysicsNative.JBody_SetFriction( b, (float)MaterialFriction.Value );
				//PhysicsNative.JBody_SetRestitution( b, (float)MaterialRestitution.Value );
			}
		}

		//public override void UpdateDataFromPhysicsEngine()
		internal void UpdateDataFromPhysicalBody()
		{
			//!!!!когда не вызывать?

			//if( ContactsData != null )
			//	ContactsData.Clear();

			if( MotionType.Value != PhysicsMotionType.Static && physicalBody != null )
			{
				var pos = physicalBody.Position;
				var rot = physicalBody.Rotation;
				var linearVelocity = physicalBody.LinearVelocity;
				var angularVelocity = physicalBody.AngularVelocity;

				//physicalBody.GetData( out var pos, out var rot, out var linearVelocity, out var angularVelocity, out var newActive );

				//!!!!
				//tr *= centerOfMassOffsetInverted;


				var oldT = Transform.Value;
				//!!!!scale?
				var newT = new Transform( pos, rot, oldT.Scale );


				//rigidBody.GetWorldTransform( out var bulletT );

				//Matrix4 tr;
				//BulletPhysicsUtility.Convert( ref bulletT, out tr );
				//tr *= centerOfMassOffsetInverted;

				//tr.Decompose( out Vector3 pos, out Quaternion rot, out Vector3 scl );
				//var oldT = Transform.Value;
				////!!!!scale?
				//var newT = new Transform( pos, rot, oldT.Scale );


				//bulletT = BulletUtils.Convert( centerOfMassOffsetInverted ) * bulletT;
				//bulletT.Decompose( out Vector3 scale, out Quaternion rotation, out Vector3 translation );
				//var oldT = Transform.Value;
				////!!!!scale?
				//var newT = new Transform( BulletUtils.Convert( translation ), BulletUtils.Convert( rotation ), oldT.Scale );

				try
				{
					updatePropertiesWithoutUpdatingBody = true;
					Transform = newT;
					LinearVelocity = linearVelocity.ToVector3();
					AngularVelocity = angularVelocity.ToVector3();
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
			if( physicalBody != null )
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

			if( physicalBody != null )
			{
				context.thisObjectWasChecked = true;

				var item = new PhysicsRayTestItem( context.ray, PhysicsRayTestItem.ModeEnum.OneClosest, PhysicsRayTestItem.FlagsEnum.None );
				physicalBody.Shape.RayTest( item, physicalBody.Position, physicalBody.Rotation );
				if( item.Result.Length != 0 )
					context.thisObjectResultRayScale = item.Result[ 0 ].DistanceScale;

				//var item = new PhysicsRayTestItem( context.ray, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.None );
				//ParentScene.PhysicsRayTest( item );
				//foreach( var resultItem in item.Result )
				//{
				//	if( resultItem.Body == physicalBody )
				//	{
				//		context.thisObjectResultRayScale = resultItem.DistanceScale;
				//		break;
				//	}
				//}

				////var item = new PhysicsRayTestItem( context.ray, PhysicsRayTestItem.ModeEnum.OneClosest, PhysicsRayTestItem.FlagsEnum.None, rigidBody );
				//////var item = new PhysicsRayTestItem( context.ray, CollisionGroup.Value, -1, PhysicsRayTestItem.ModeEnum.OneClosest, rigidBody );
				////ParentScene.PhysicsRayTest( item );
				////if( item.Result.Length != 0 )
				////	context.thisObjectResultRayScale = item.Result[ 0 ].DistanceScale;
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

		public void Render( ViewportRenderingContext context, bool renderActive, bool renderSelected, bool renderCanSelect, Transform bodyTransform, ref int verticesRendered )
		{
			//verticesRendered = 0;

			var context2 = context.ObjectInSpaceRenderingContext;

			//var scene = ParentScene;

			//bool show = ( scene.GetDisplayDevelopmentDataInThisApplication() && scene.DisplayPhysicalObjects ) ||
			//	context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.dragDropCreateObject == this;
			if( /*show && */ /*rigidBody != null && */context.Owner.Simple3DRenderer != null )
			{
				var viewport = context.Owner;
				var renderer = viewport.Simple3DRenderer;

				//!!!!
				//if( context2.displayPhysicalObjectsCounter < context2.displayPhysicalObjectsMax )
				//{
				//	context2.displayPhysicalObjectsCounter++;

				//draw body
				{
					ColorValue color;
					if( MotionType.Value == PhysicsMotionType.Static )
						color = ProjectSettings.Get.Colors.SceneShowPhysicsStaticColor;
					else if( renderActive )// Active )
						color = ProjectSettings.Get.Colors.SceneShowPhysicsDynamicActiveColor;
					else
						color = ProjectSettings.Get.Colors.SceneShowPhysicsDynamicInactiveColor;
					viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

					foreach( var shape in GetComponents<CollisionShape>() )
					{
						if( shape.Enabled )
							shape.Render( viewport, bodyTransform, false, ref verticesRendered );
					}
					//foreach( var shape in GetComponents<CollisionShape>( onlyEnabledInHierarchy: true ) )
					//	shape.Render( viewport, bodyTransform, false, ref verticesRendered );
					////foreach( var shape in GetComponents<CollisionShape>( false, true, true ) )
					////	shape.Render( viewport, Transform, false, ref verticesRendered );

					//center of mass
					if( MotionType.Value == PhysicsMotionType.Dynamic )
					{
						//!!!!impl

						//var center = rigidBody.CenterOfMassPosition;
						//double radius = SpaceBounds.CalculatedBoundingSphere.Radius / 16;

						//var item = GetCenterOfMassGeometry( (float)radius );
						//var transform = Matrix4.FromTranslate( BulletPhysicsUtility.Convert( center ) );
						//context.Owner.Simple3DRenderer.AddTriangles( item.positions, item.indices, ref transform, false, true );
						////context.viewport.Simple3DRenderer.AddSphere( BulletPhysicsUtility.Convert( center ), radius, 16, true );

						////Vector3 center = Vector3.Zero;
						////double radius = 1.0;
						////rigidBody.CollisionShape.GetBoundingSphere( out center, out radius );
						////center = rigidBody.CenterOfMassPosition;
						////radius /= 16.0;
						////context.viewport.DebugGeometry.AddSphere( BulletUtils.Convert( center ), radius );

						//verticesRendered += item.positions.Length;
					}

					//!!!!impl

					////ccd sphere radius
					//if( CCD && MotionType.Value == MotionTypeEnum.Dynamic )
					//{
					//	context.Owner.Simple3DRenderer.AddSphere( new Sphere( TransformV.Position, rigidBody.CcdSweptSphereRadius ), 16, false );
					//	verticesRendered += 16 * 3 * 8;
					//}
				}

				//!!!!copy code
				//draw selection
				if( renderSelected || renderCanSelect )
				//if( renderSelected && ( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) ) )
				{
					ColorValue color;
					if( renderSelected )
						color = ProjectSettings.Get.Colors.SelectedColor;
					else if( renderCanSelect )
						color = ProjectSettings.Get.Colors.CanSelectColor;
					else
						color = ProjectSettings.Get.Colors.SceneShowPhysicsDynamicActiveColor;

					//ColorValue color;
					//if( context2.selectedObjects.Contains( this ) )
					//	color = ProjectSettings.Get.Colors.SelectedColor;
					//else if( context2.canSelectObjects.Contains( this ) )
					//	color = ProjectSettings.Get.Colors.CanSelectColor;
					//else
					//	color = ProjectSettings.Get.Colors.SceneShowPhysicsDynamicActiveColor;

					//!!!!или невидимое лучше габаритами подсвечивать?

					color.Alpha *= .5f;
					viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

					foreach( var shape in GetComponents<CollisionShape>() )
					{
						if( shape.Enabled )
							shape.Render( viewport, bodyTransform, true, ref verticesRendered );
					}
					//foreach( var shape in GetComponents<CollisionShape>( onlyEnabledInHierarchy: true ) )
					//	shape.Render( viewport, bodyTransform, true, ref verticesRendered );
					////foreach( var shape in GetComponents<CollisionShape>( false, true, true ) )
					////	shape.Render( viewport, Transform, true, ref verticesRendered );

					//context.viewport.DebugGeometry.AddBounds( SpaceBounds.CalculatedBoundingBox );
				}

				//display collision contacts
				if( DisplayContacts && PhysicalBody != null )
				{
					var contacts = PhysicalBody.GetContacts();
					if( contacts.Count != 0 )
					{
						var size3 = SpaceBounds.BoundingBox.GetSize();
						var scale = (float)Math.Min( size3.X, Math.Min( size3.Y, size3.Z ) ) / 30;

						renderer.SetColor( new ColorValue( 1, 0, 0 ) );

						for( int nContact = 0; nContact < contacts.Count; nContact++ )
						{
							ref var contact = ref contacts.Array[ contacts.Offset + nContact ];//var contact = contacts[ n ];

							var pos = contact.WorldPositionOn1;//var pos = contact.PositionWorldOnA;
							var bounds = new Bounds(
								pos - new Vector3( scale, scale, scale ),
								pos + new Vector3( scale, scale, scale ) );

							renderer.AddBounds( bounds, true );
						}
					}
				}

				////display collision contacts
				//if( DisplayContacts &&  ContactsData != null && ContactsData.Count != 0 )
				//{


				//	var size3 = SpaceBounds.CalculatedBoundingBox.GetSize();
				//	var scale = (float)Math.Min( size3.X, Math.Min( size3.Y, size3.Z ) ) / 30;

				//	renderer.SetColor( new ColorValue( 1, 0, 0 ) );

				//	var data = ContactsData;
				//	for( int n = 0; n < data.Count; n++ )
				//	{
				//		ref var item = ref data.Data[ n ];
				//		if( item.Distance < 0 && item.SimulationSubStep == 0 )
				//		{
				//			var pos = item.PositionWorldOnA;
				//			var bounds = new Bounds(
				//				pos - new Vector3( scale, scale, scale ),
				//				pos + new Vector3( scale, scale, scale ) );

				//			renderer.AddBounds( bounds, true );
				//		}
				//	}
				//}

				//}
			}
		}

		//public override void RenderPhysicalObject( ViewportRenderingContext context, out int verticesRendered )
		//{
		//	if( physicalBody != null )
		//	{
		//		var context2 = context.ObjectInSpaceRenderingContext;
		//		Render( context, Active, context2.selectedObjects.Contains( this ), context2.canSelectObjects.Contains( this ), Transform, out verticesRendered );
		//	}
		//	else
		//		verticesRendered = 0;
		//}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				bool show = context.SceneDisplayDevelopmentDataInThisApplication && ParentScene.DisplayPhysicalObjects;
#if !DEPLOY
				if( !show )
					show = context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) || context2.objectToCreate == this;
#endif

				if( show && physicalBody != null )
				{
					var verticesRendered = 0;
					Render( context, Active, context2.selectedObjects.Contains( this ), context2.canSelectObjects.Contains( this ), Transform, ref verticesRendered );
				}

				var showLabels = /*show &&*/ physicalBody == null;
				if( !showLabels )
					context2.disableShowingLabelForThisObject = true;
			}
		}

		//!!!!maybe event ActiveChanged
		[Browsable( false )]
		public bool Active
		{
			get
			{
				if( physicalBody != null )
					return physicalBody.Active;
				return false;
			}
			set
			{
				if( physicalBody != null )
					physicalBody.Active = value;
			}
		}

		//public void Activate()
		//{
		//	physicalBody?.Activate();
		//}

		//public void WantsDeactivation()
		//{
		//	//!!!!если сразу деактивирует, то переименовать

		//	physicalBody?.Deactivate();
		//}

		public void ApplyForce( Vector3 force, Vector3 relativePosition )
		{
			if( physicalBody != null && MotionType.Value == PhysicsMotionType.Dynamic && force != Vector3.Zero )
			{
				var force2 = force.ToVector3F();
				var relativePosition2 = relativePosition.ToVector3F();
				physicalBody.ApplyForce( ref force2, ref relativePosition2 );

				////Activate();
				////BulletPhysicsUtility.Convert( ref force, out var bForce );
				////BulletPhysicsUtility.Convert( ref relativePosition, out var bRelPos );
				////rigidBody?.ApplyForceRef( ref bForce, ref bRelPos );
			}
		}
	}
}
