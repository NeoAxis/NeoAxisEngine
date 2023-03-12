// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	//Car
	//Truck
	//Tractor
	//Tank
	//Airplane with wheels
	//Helicopter
	//Bike
	//Three wheel bike

	/// <summary>
	/// A component to make instance of a vehicle in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Vehicle\Vehicle", 22002 )]
	public class Vehicle : MeshInSpace, InteractiveObject, IProcessDamage
	{
		static FastRandom boundsPredictionAndUpdateRandom = new FastRandom( 0 );

		//

		DynamicData dynamicData;
		bool needRecreateDynamicData;

		//bool needBeActiveBecauseDriverInput;
		double cannotBeStaticRemainingTime;

		//!!!!by idea it must by solved inside Jolt
		////going sleep after 10 seconds. fix for Jolt
		//double needStaticTotalTime;

		//bool needBeActiveBecausePhysicsVelocity;
		//Transform lastTransformToCalculateDynamicState;
		//bool needBeActiveBecauseTransformChange;
		////Vector3 lastTransformPosition;
		////Quaternion lastTransformRotation;
		////Vector3 lastLinearVelocity;
		////smooth?

		Vector3 groundRelativeVelocity;
		Vector3[] groundRelativeVelocitySmoothArray;
		Vector3 groundRelativeVelocitySmooth;

		bool duringTransformUpdateWithoutRecrecting;

		bool driverInputNeedUpdate = true;

		double currentSteering;

		float remainingTimeToUpdateObjectsOnSeat;

		//!!!!
		//double allowToSleepTime;

		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		//Vector3 linearVelocityForSerialization;

		/////////////////////////////////////////
		//Basic

		const string vehicleTypeDefault = @"Content\Vehicles\Default\Default Vehicle.vehicletype";

		[DefaultValueReference( vehicleTypeDefault )]
		//[Category( "General" )]
		public Reference<VehicleType> VehicleType
		{
			get { if( _vehicleType.BeginGet() ) VehicleType = _vehicleType.Get( this ); return _vehicleType.value; }
			set { if( _vehicleType.BeginSet( ref value ) ) { try { VehicleTypeChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _vehicleType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VehicleType"/> property value changes.</summary>
		public event Action<Vehicle> VehicleTypeChanged;
		ReferenceField<VehicleType> _vehicleType = new Reference<VehicleType>( null, vehicleTypeDefault );

		//!!!!impl modes

		[DefaultValue( PhysicsModeEnum.Basic )]
		public Reference<PhysicsModeEnum> PhysicsMode
		{
			get { if( _physicsMode.BeginGet() ) PhysicsMode = _physicsMode.Get( this ); return _physicsMode.value; }
			set { if( _physicsMode.BeginSet( ref value ) ) { try { PhysicsModeChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _physicsMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PhysicsMode"/> property value changes.</summary>
		public event Action<Vehicle> PhysicsModeChanged;
		ReferenceField<PhysicsModeEnum> _physicsMode = PhysicsModeEnum.Basic;

		/////////////////////////////////////////

		/// <summary>
		/// Whether to visualize debug info.
		/// </summary>
		//[Category( "Debug" )]
		[DefaultValue( false )]
		public Reference<bool> DebugVisualization
		{
			get { if( _debugVisualization.BeginGet() ) DebugVisualization = _debugVisualization.Get( this ); return _debugVisualization.value; }
			set { if( _debugVisualization.BeginSet( ref value ) ) { try { DebugVisualizationChanged?.Invoke( this ); } finally { _debugVisualization.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugVisualization"/> property value changes.</summary>
		public event Action<Vehicle> DebugVisualizationChanged;
		ReferenceField<bool> _debugVisualization = false;

		[Cloneable( CloneType.Deep )]
		[Serialize]
		public ReferenceList<ObjectInSpace> ObjectsOnSeats
		{
			get { return _objectsOnSeats; }
		}
		public delegate void ObjectsOnSeatsChangedDelegate( Vehicle sender );
		public event ObjectsOnSeatsChangedDelegate ObjectsOnSeatsChanged;
		ReferenceList<ObjectInSpace> _objectsOnSeats;

		/////////////////////////////////////////

		//!!!!Tracked chassis

		/// <summary>
		/// The throttle parameter to control the vehicle.
		/// </summary>
		[Category( "Control" )]
		[DefaultValue( 0.0 )]
		[Range( -1, 1 )]
		public Reference<double> Throttle
		{
			get { if( _throttle.BeginGet() ) Throttle = _throttle.Get( this ); return _throttle.value; }
			set { if( _throttle.BeginSet( ref value ) ) { try { ThrottleChanged?.Invoke( this ); driverInputNeedUpdate = true; } finally { _throttle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Throttle"/> property value changes.</summary>
		public event Action<Vehicle> ThrottleChanged;
		ReferenceField<double> _throttle = 0.0;

		/// <summary>
		/// The brake parameter to control the vehicle.
		/// </summary>
		[Category( "Control" )]
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		public Reference<double> Brake
		{
			get { if( _brake.BeginGet() ) Brake = _brake.Get( this ); return _brake.value; }
			set { if( _brake.BeginSet( ref value ) ) { try { BrakeChanged?.Invoke( this ); driverInputNeedUpdate = true; } finally { _brake.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Brake"/> property value changes.</summary>
		public event Action<Vehicle> BrakeChanged;
		ReferenceField<double> _brake = 0.0;

		/// <summary>
		/// The hand brake parameter to control the vehicle.
		/// </summary>
		[Category( "Control" )]
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		public Reference<double> HandBrake
		{
			get { if( _handHandBrake.BeginGet() ) HandBrake = _handHandBrake.Get( this ); return _handHandBrake.value; }
			set { if( _handHandBrake.BeginSet( ref value ) ) { try { HandBrakeChanged?.Invoke( this ); driverInputNeedUpdate = true; } finally { _handHandBrake.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HandBrake"/> property value changes.</summary>
		public event Action<Vehicle> HandBrakeChanged;
		ReferenceField<double> _handHandBrake = 1.0;

		/// <summary>
		/// The steering parameter to control the vehicle.
		/// </summary>
		[Category( "Control" )]
		[DefaultValue( 0.0 )]
		[Range( -1, 1 )]
		public Reference<double> Steering
		{
			get { if( _steering.BeginGet() ) Steering = _steering.Get( this ); return _steering.value; }
			set { if( _steering.BeginSet( ref value ) ) { try { SteeringChanged?.Invoke( this ); driverInputNeedUpdate = true; } finally { _steering.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Steering"/> property value changes.</summary>
		public event Action<Vehicle> SteeringChanged;
		ReferenceField<double> _steering = 0.0;

		/////////////////////////////////////////

		/// <summary>
		/// The health of the vehicle.
		/// </summary>
		[Category( "Game Framework" )]
		[DefaultValue( 0.0 )]
		public Reference<double> Health
		{
			get { if( _health.BeginGet() ) Health = _health.Get( this ); return _health.value; }
			set { if( _health.BeginSet( ref value ) ) { try { HealthChanged?.Invoke( this ); driverInputNeedUpdate = true; } finally { _health.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Health"/> property value changes.</summary>
		public event Action<Vehicle> HealthChanged;
		ReferenceField<double> _health = 0.0;

		/// <summary>
		/// The team index of the object.
		/// </summary>
		[Category( "Game Framework" )]
		[DefaultValue( 0 )]
		public Reference<int> Team
		{
			get { if( _team.BeginGet() ) Team = _team.Get( this ); return _team.value; }
			set { if( _team.BeginSet( ref value ) ) { try { TeamChanged?.Invoke( this ); } finally { _team.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Team"/> property value changes.</summary>
		public event Action<Vehicle> TeamChanged;
		ReferenceField<int> _team = 0;

		/////////////////////////////////////////

		[Browsable( false )]
		public Vector3 LinearVelocityToPredictBounds { get; set; }

		/////////////////////////////////////////

		internal class DynamicData
		{
			public VehicleType VehicleType;
			public PhysicsModeEnum PhysicsMode;
			public Mesh Mesh;
			public Mesh FrontWheelMesh;
			public Mesh RearWheelMesh;
			public Bounds LocalBoundsDefault;
			public float LocalBoundsDefaultBoundingRadius;

			public WheelItem[] Wheels;
			public SeatItem[] Seats;
			public Scene.PhysicsWorldClass.VehicleConstraint constraint;

			/////////////////////

			public enum WhichWheel
			{
				FrontLeft,
				FrontRight,
				RearLeft,
				RearRight,
				//Custom1, Custom2, etc
			}

			/////////////////////

			public struct WheelItem
			{
				//static data
				public WhichWheel Which;
				//public bool WheelDrive;
				public float Diameter;
				public float Width;
				public Vector3F InitialPosition;

				//dynamic data
				public Vector3F CurrentPosition;
				public QuaternionF CurrentRotation;

				////RealPhysics mode
				//public RigidBody RigidBody;
				//public Constraint Constraint;
				//public MeshInSpace MeshInSpace;

				//public ESet<RigidBody> LastContactBodies;

				public Scene.PhysicsWorldClass.Body ContactBody;
				//no sense
				//public double ContactBodyTimeUpdated;
				//public double LastGroundTime;

				//

				public bool Front
				{
					get { return Which == WhichWheel.FrontLeft || Which == WhichWheel.FrontRight; }
				}

				public bool Rear
				{
					get { return Which == WhichWheel.RearLeft || Which == WhichWheel.RearRight; }
				}

				public bool Left
				{
					get { return Which == WhichWheel.FrontLeft || Which == WhichWheel.RearLeft; }
				}

				public bool Right
				{
					get { return Which == WhichWheel.FrontRight || Which == WhichWheel.RearRight; }
				}
			}

			/////////////////////

			public class SeatItem
			{
				public VehicleSeat SeatComponent;

				//!!!!need make copy? in type can be with reference
				public Transform Transform;
				public Vector3 EyeOffset;
				public Transform ExitTransform;
			}
		}

		/////////////////////////////////////////

		public Vehicle()
		{
			_objectsOnSeats = new ReferenceList<ObjectInSpace>( this, () => ObjectsOnSeatsChanged?.Invoke( this ) );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				//these properties are under control by the class
				case nameof( Mesh ):
				case nameof( Collision ):
					skip = true;
					break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchyAndIsInstance )
			{
				currentSteering = Steering;
				CreateDynamicData();

				//!!!!crashes
				//if( dynamicData != null )
				//	SetDriverInput( currentSteering, true );

				//!!!!что еще восстанавливать помимо скорости
				//if( mainBody != null )
				//	mainBody.LinearVelocity = linearVelocityForSerialization;
			}
			else
				DestroyDynamicData();
		}

		public void SetTransform( Transform value, bool recreate )
		{
			if( !recreate )
				duringTransformUpdateWithoutRecrecting = true;
			Transform = value;
			if( !recreate )
				duringTransformUpdateWithoutRecrecting = false;
		}

		//public void SetTransform( Transform value, bool recreate )
		//{
		//	//!!!!

		//	//!!!!?
		//	if( !Transform.ReferenceSpecified )
		//		Transform = value;

		//	//!!!!сразу обновлять?
		//	CreateDynamicData();
		//}

		//public void SetTransformWithoutRecreating( Transform value )
		//{
		//	duringTransformUpdateWithoutRecrecting = true;
		//	Transform = value;
		//	duringTransformUpdateWithoutRecrecting = false;
		//}

		//public double GetScaleFactor()
		//{
		//	var result = TransformV.Scale.MaxComponent();
		//	//var result = GetTransform().Scale.MaxComponent();
		//	if( result == 0 )
		//		result = 0.0001;
		//	return result;
		//}

		///////////////////////////////////////////

		public bool IsOnGround()
		{
			if( dynamicData != null && dynamicData.Wheels != null )
			{
				for( int n = 0; n < dynamicData.Wheels.Length; n++ )
				{
					ref var wheel = ref dynamicData.Wheels[ n ];

					if( wheel.ContactBody != null && !wheel.ContactBody.Disposed )
						return true;
					//no sense
					//if( EngineApp.EngineTime < wheel.LastGroundTime + 0.5 )
					//	return true;
				}
			}

			return false;
		}

		//protected override void OnSave( TextBlock block )
		//{
		//	if( mainBody != null )
		//		linearVelocityForSerialization = mainBody.LinearVelocity;

		//	base.OnSave( block );
		//}

		//protected override void OnSuspendPhysicsDuringMapLoading( bool suspend )
		//{
		//	base.OnSuspendPhysicsDuringMapLoading( suspend );

		//	//After loading a map, the physics simulate 5 seconds, that bodies have fallen asleep.
		//	//During this time we will disable physics for this entity.
		//		foreach( Body body in PhysicsModel.Bodies )
		//		{
		//			body.Static = suspend;
		//			if( !suspend )
		//				mainBody.Sleeping = false;
		//		}
		//}

		void SetDriverInput( double lastSteering, bool initialization )
		{
			var activateBody = Throttle != 0 || currentSteering != 0;
			if( driverInputNeedUpdate || activateBody || lastSteering != currentSteering )
			{
				dynamicData.constraint.SetDriverInput( (float)Throttle, (float)currentSteering, (float)Brake, (float)HandBrake, activateBody );
				cannotBeStaticRemainingTime = initialization ? 0 : 3.0;

				//needBeActiveBecauseDriverInput = true;
			}
			//else
			//	needBeActiveBecauseDriverInput = false;

			driverInputNeedUpdate = false;
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( dynamicData != null && dynamicData.constraint != null )
			{
				//update cannotBeStaticReamainingTime
				if( cannotBeStaticRemainingTime > 0 )
				{
					cannotBeStaticRemainingTime -= Time.SimulationDelta;
					if( cannotBeStaticRemainingTime < 0 )
						cannotBeStaticRemainingTime = 0;
				}

				var mustBeDynamic = IsMustBeDynamic();


				//update driver input
				if( mustBeDynamic || driverInputNeedUpdate )
				{
					var lastSteering = currentSteering;

					var steering = Steering.Value;
					if( currentSteering != steering )
					{
						if( currentSteering > steering )
						{
							currentSteering -= Time.SimulationDelta / dynamicData.VehicleType.FrontWheelSteeringTime;
							if( currentSteering < steering )
								currentSteering = steering;
							if( currentSteering < -1 )
								currentSteering = -1;
						}
						else
						{
							currentSteering += Time.SimulationDelta / dynamicData.VehicleType.FrontWheelSteeringTime;
							if( currentSteering > steering )
								currentSteering = steering;
							if( currentSteering > 1 )
								currentSteering = 1;
						}
					}

					SetDriverInput( lastSteering, false );

					//var activateBody = Throttle != 0 || currentSteering != 0;
					//if( driverInputNeedUpdate || activateBody || lastSteering != currentSteering )
					//{
					//	dynamicData.constraint.SetDriverInput( (float)Throttle, (float)currentSteering, (float)Brake, (float)HandBrake, activateBody );
					//	cannotBeStaticRemainingTime = 3.0;
					//	//needBeActiveBecauseDriverInput = true;
					//}
					////else
					////	needBeActiveBecauseDriverInput = false;

					//driverInputNeedUpdate = false;
				}

				dynamicData.constraint.SetStepListenerAddedMustBeAdded( mustBeDynamic );

				if( mustBeDynamic )//if( PhysicalBody != null && PhysicalBody.Active )
				{
					CalculateGroundRelativeVelocity();

					//var tr = TransformV;
					//needBeActiveBecauseTransformChange = lastTransformToCalculateDynamicState.Equals(
					//lastTransformToCalculateDynamicState = tr;

					//var trPosition = TransformV.Position;
					//if( lastTransformPosition.HasValue )
					//	lastLinearVelocity = ( trPosition - lastTransformPosition.Value ) / Time.SimulationDelta;
					//else
					//	lastLinearVelocity = Vector3.Zero;
					//lastTransformPosition = trPosition;
				}
				//else
				//{
				//	//needBeActiveBecausePhysicsVelocity;
				//	lastTransformToCalculateDynamicState = TransformV;
				//	needBeActiveBecauseTransformChange = false;
				//}

				//if( mustBeDynamic )//if( PhysicalBody != null && PhysicalBody.Active )
				//{
				//	CalculateGroundRelativeVelocity();

				//	var tr = TransformV;
				//	needBeActiveBecauseTransformChange = lastTransformToCalculateDynamicState.Equals(
				//	lastTransformToCalculateDynamicState = tr;

				//	//var trPosition = TransformV.Position;
				//	//if( lastTransformPosition.HasValue )
				//	//	lastLinearVelocity = ( trPosition - lastTransformPosition.Value ) / Time.SimulationDelta;
				//	//else
				//	//	lastLinearVelocity = Vector3.Zero;
				//	//lastTransformPosition = trPosition;
				//}
				//else
				//{
				//	lastTransformToCalculateDynamicState = TransformV;
				//	needBeActiveBecauseTransformChange = false;
				//}

				////if( PhysicalBody != null && PhysicalBody.Active )//if( !Static )//&& false )
				////{
				////	CalculateGroundRelativeVelocity();

				////	var trPosition = TransformV.Position;
				////	if( lastTransformPosition.HasValue )
				////		lastLinearVelocity = ( trPosition - lastTransformPosition.Value ) / Time.SimulationDelta;
				////	else
				////		lastLinearVelocity = Vector3.Zero;
				////	lastTransformPosition = trPosition;
				////}

				var needStatic = !mustBeDynamic;// !IsMustBeDynamic();
				if( needStatic != Static )
				{
					//can't be static some time after change from static to dynamic
					if( mustBeDynamic )
						cannotBeStaticRemainingTime = 3.0;

					//if( needStatic )
					//{
					//	lastTransformToCalculateDynamicState = TransformV;
					//	needBeActiveBecauseTransformChange = false;
					//}
					//else
					//	cannotBeStaticRemainingTime = 1.0;

					Static = needStatic;

					//ScreenMessages.Add( "STATIC update" );
				}

				//ScreenMessages.Add( "Static: " + Static.ToString() );


				//!!!!by idea it must by solved inside Jolt
				////to sleep after 10 seconds of anactivity (fix for Jolt)
				//if( needStatic )
				//	needStaticTotalTime += Time.SimulationDelta;
				//else
				//	needStaticTotalTime = 0;
				//if( PhysicalBody != null && PhysicalBody.Active && needStaticTotalTime > 10.0 )
				//	PhysicalBody.Active = false;


				//update additional items
				if( mustBeDynamic && PhysicalBody != null && PhysicalBody.Active && dynamicData.Wheels != null )
				{
					unsafe
					{
						var wheelsData = stackalloc Scene.PhysicsWorldClass.VehicleWheelData[ dynamicData.Wheels.Length ];
						dynamicData.constraint.GetData( wheelsData, out var active );

						for( int n = 0; n < dynamicData.Wheels.Length; n++ )
						{
							ref var wheel = ref dynamicData.Wheels[ n ];
							var wheelData = wheelsData + n;

							wheel.CurrentPosition = wheel.InitialPosition;
							wheel.CurrentPosition.Z = wheelData->Position;// = wheelData->SuspensionLength - wheel.Diameter * 0.5f;
							wheel.CurrentRotation = QuaternionF.FromRotateByZ( -wheelData->SteerAngle ) * QuaternionF.FromRotateByY( -wheelData->RotationAngle );
							//!!!!use angular veclocity for motion blur?
							//wheelData->AngularVelocity

							Scene.PhysicsWorldClass.Body contactBody = null;
							if( wheelData->ContactBody != 0xffffffff )
							{
								var physicsWorldData = dynamicData.constraint.PhysicsWorld;
								contactBody = physicsWorldData.GetBodyById( wheelData->ContactBody );
							}

							wheel.ContactBody = contactBody;

							//no sense
							//wheel.ContactBodyTimeUpdated = EngineApp.EngineTime;
							//wheel.LastGroundTime
						}
					}

					UpdateAdditionalItems();
				}
			}






			//if( Static != CanBeStatic() )
			//	SetStatic( !Static );

			////if( !Static )
			////{
			////	if( CanSwitchToStaticState() )
			////		SetStatic( true );
			////}
			////else
			////{
			////	if( !CanSwitchToStaticState() )
			////		SetStatic( false );
			////}


			////update
			//if( dynamicData != null && dynamicData.MainBody != null && dynamicData.Wheels != null )
			//{
			//	var type = dynamicData.VehicleType;

			//	var tr = TransformV;
			//	var mainBody = dynamicData.MainBody;

			//	var throttle = Throttle.Value;
			//	var brake = Brake.Value;
			//	var steering = Steering.Value;

			//	//!!!!still need?
			//	//!!!!engine bug fix. motor doesn't wake up rigid body
			//	if( throttle != 0 || /*brake != 0 || */steering != 0 )
			//		dynamicData.MainBody.Activate();

			//	for( int nWheel = 0; nWheel < dynamicData.Wheels.Length; nWheel++ )
			//	//foreach( var wheel in dynamicData.Wheels )
			//	{
			//		var wheel = dynamicData.Wheels[ nWheel ];

			//		//SimplePhysics mode
			//		if( dynamicData.SimulationMode == SimulationModeEnum.RealPhysics )
			//		{
			//			var c = wheel.Constraint;
			//			if( c != null )
			//			{
			//				//!!!!impl
			//				////suspension
			//				//if( c.InternalConstraintRigid != null )
			//				//{
			//				//	//!!!!

			//				//	var offset = c.InternalConstraintRigid.GetRelativePivotPosition( 2 );

			//				//	var suspensionTravel = SuspensionTravel.Value;

			//				//	//!!!!
			//				//	//if( nWheel == 0 )
			//				//	{
			//				//		var travelCenter = suspensionTravel.GetCenter();
			//				//		var diff = offset - travelCenter;

			//				//		//if( offset > 0 && suspensionTravel.Maximum > 0 )
			//				//		if( diff > 0 && suspensionTravel.Maximum != suspensionTravel.Minimum )
			//				//		{
			//				//			var factor = diff / ( suspensionTravel.Maximum - suspensionTravel.Minimum );

			//				//			//apply to chassis
			//				//			{
			//				//				//var offsetFactor = offset / suspensionTravel.Maximum;

			//				//				//!!!!
			//				//				//Log.Info( offsetFactor.ToString() );


			//				//				//!!!!в 4 раза мньше нужно


			//				//				var force = 10.0 * factor * mainBody.Mass.Value;
			//				//				//var force = 7000.0 * factor;

			//				//				//var force = 5000.0 * offsetFactor;

			//				//				var forceVector = tr.Rotation.GetUp();

			//				//				mainBody.ApplyForce( forceVector * force, wheel.LocalPosition );
			//				//			}

			//				//			//apply to wheel
			//				//			if( wheel.RigidBody != null )
			//				//			{
			//				//				//!!!!
			//				//				var force = 2.0 * factor * wheel.RigidBody.Mass.Value;

			//				//				var forceVector = -tr.Rotation.GetUp();

			//				//				wheel.RigidBody.ApplyForce( forceVector * force, Vector3.Zero );
			//				//			}

			//				//		}
			//				//	}

			//				//	//Log.Info( offset.ToString() );


			//				//	//!!!!
			//				//	//SpringRate

			//				//	//if( offset > 0 )
			//				//	//	c.LinearAxisZMotorMaxForce = SpringRate * -offset * 300;
			//				//	//else
			//				//	//	c.LinearAxisZMotorMaxForce = 0;

			//				//	//c.LinearAxisZMotorTargetVelocity = 0.5;

			//				//	//c.LinearAxisZMotorMaxForce = SpringRate * -offset * 200;
			//				//	//c.LinearAxisZMotorTargetVelocity = 0.5;
			//				//}

			//				//throttle, brake
			//				if( brake != 0 )
			//				{
			//					c.AngularAxisXMotorTargetVelocity = 0;
			//					c.AngularAxisXMotorMaxForce = type.BrakeForce * brake;
			//				}
			//				else
			//				{
			//					if( wheel.WheelDrive )
			//					{
			//						//!!!!
			//						c.AngularAxisXMotorTargetVelocity = -type.ThrottleTargetVelocity * throttle;
			//						//c.AngularAxisXMotorTargetVelocity = ThrottleTargetVelocity * throttle;
			//						//!!!!
			//						c.AngularAxisXMotorMaxForce = throttle != 0 ? type.ThrottleForce : 0;
			//					}
			//					else
			//					{
			//						c.AngularAxisXMotorTargetVelocity = 0;
			//						c.AngularAxisXMotorMaxForce = 0;
			//					}
			//				}

			//				//steering
			//				if( wheel.Front )
			//				{
			//					c.AngularAxisZServoTarget = c.AngularAxisZLimitHigh.Value * steering;
			//					c.AngularAxisZMotorTargetVelocity = 1;
			//					c.AngularAxisZMotorMaxForce = type.SteeringForce;
			//				}
			//			}
			//		}

			//		////Raycast mode
			//		//if( dynamicData.SimulationMode == SimulationModeEnum.Raycast )
			//		//{
			//		//	var vehicle = dynamicData.raycastVehicle;
			//		//	if( vehicle != null )
			//		//	{
			//		//		var wheelInfo = vehicle.GetWheelInfo( nWheel );

			//		//		if( wheel.WheelDrive )
			//		//		{
			//		//			//!!!!не так, не только это

			//		//			wheelInfo.EngineForce = ThrottleForce * 100;

			//		//		}

			//		//		//!!!!
			//		//		//wheelInfo.Brake = zzzzz;

			//		//		//EngineForce *= ( 1.0f - timeStep );

			//		//		//vehicle.ApplyEngineForce( EngineForce, 2 );
			//		//		//vehicle.SetBrake( BreakingForce, 2 );
			//		//		//vehicle.SetSteeringValue( VehicleSteering, 0 );

			//		//		if( wheel.Front )
			//		//		{
			//		//			//!!!!
			//		//			//wheelInfo.Steering = zzzzz;
			//		//		}

			//		//	}
			//		//}


			//		//!!!!need


			//		////update LastContactsBodies
			//		//wheel.LastContactBodies?.Clear();
			//		//var contacts = wheel.RigidBody?.ContactsData;
			//		//if( contacts != null )
			//		//{
			//		//	for( int n = 0; n < contacts.Count; n++ )
			//		//	{
			//		//		ref var contact = ref contacts.Data[ n ];

			//		//		var bodyB = contact.BodyB;

			//		//		if( bodyB == null )
			//		//			continue;
			//		//		if( bodyB == dynamicData.MainBody )
			//		//			continue;
			//		//		//!!!!only static?
			//		//		if( bodyB.MotionType.Value != PhysicsMotionType.Static )
			//		//			continue;

			//		//		if( wheel.LastContactBodies == null )
			//		//			wheel.LastContactBodies = new ESet<RigidBody>();
			//		//		wheel.LastContactBodies.AddWithCheckAlreadyContained( bodyB );
			//		//	}
			//		//}

			//		if( wheel.LastContactBodies != null && wheel.LastContactBodies.Count != 0 )
			//			wheel.LastGroundTime = EngineApp.EngineTime;
			//	}
			//}


			//ScreenMessages.Add( "speed: " + IsOnGround().ToString() + " " + GroundRelativeVelocitySmooth.ToVector2().Length().ToString() );
		}

		protected override void OnSimulationStepClient()
		{
			base.OnSimulationStepClient();

			//!!!!?
		}

		void UpdateObjectOnSeat( int seatIndex )
		{
			var objectOnSeat = GetObjectOnSeat( seatIndex );
			if( objectOnSeat != null )
			{
				var seatItem = dynamicData.Seats[ seatIndex ];

				//!!!!animation

				objectOnSeat.Visible = seatItem.SeatComponent.Visible && Visible;


				//!!!!slowly. update not each call

				var character = objectOnSeat as Character;
				if( character != null )
				{
					character.Collision = false;
					//character.DestroyCollisionBody();

					character.SetTransformAndTurnToDirectionInstantly( TransformV * seatItem.Transform );
				}
			}
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( needRecreateDynamicData )
				CreateDynamicData();

			//update objects on seats
			if( dynamicData != null && !NetworkIsClient )
			{
				remainingTimeToUpdateObjectsOnSeat -= delta;
				if( remainingTimeToUpdateObjectsOnSeat <= 0 )
				{
					remainingTimeToUpdateObjectsOnSeat = 0.25f + boundsPredictionAndUpdateRandom.Next( 0.05f );

					for( int seatIndex = 0; seatIndex < dynamicData.Seats.Length; seatIndex++ )
						UpdateObjectOnSeat( seatIndex );
				}
			}
		}

		protected override void OnTransformChanged()
		{
			//UpdateSpaceBoundsOverride();

			base.OnTransformChanged();

			if( EngineApp.IsEditor && !duringTransformUpdateWithoutRecrecting )
				NeedRecreateDynamicData();
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			//base.OnSpaceBoundsUpdate( ref newBounds );

			GetBox( out var box );
			box.ToBounds( out var realBounds );

			if( !IsMustBeDynamic() )
			{
				newBounds = new SpaceBounds( realBounds );
				SpaceBoundsOctreeOverride = null;
			}
			else
			{
				if( dynamicData != null )
				{
					//here is a bounds prediction to skip small updates in future steps

					////calculate actual bounds
					//var radius = dynamicData.LocalBoundsDefaultBoundingRadius;
					//var tr = TransformV;
					//var realBounds = new Bounds(
					//	tr.Position.X - radius, tr.Position.Y - radius, tr.Position.Z - radius,
					//	tr.Position.X + radius, tr.Position.Y + radius, tr.Position.Z + radius );
					newBounds = new SpaceBounds( realBounds );

					//check for update extended bounds
					if( !SpaceBoundsOctreeOverride.HasValue || !SpaceBoundsOctreeOverride.Value.Contains( realBounds ) )
					{
						//calculate extended bounds

						var radius = dynamicData.LocalBoundsDefaultBoundingRadius;
						var tr = TransformV;

						//update each 2-3 seconds
						var extendForSeconds = 2.0f + boundsPredictionAndUpdateRandom.Next( 0.0f, 1.0f );
						var radiusExtended = radius * 1.1f;

						Vector3 velocity;
						if( PhysicalBody != null )
							velocity = PhysicalBody.LinearVelocity;
						else
							velocity = LinearVelocityToPredictBounds;

						var pos2 = tr.Position + velocity * extendForSeconds;
						var b2 = new Bounds(
							pos2.X - radiusExtended, pos2.Y - radiusExtended, pos2.Z - radiusExtended,
							pos2.X + radiusExtended, pos2.Y + radiusExtended, pos2.Z + radiusExtended );

						var bTotal = new Bounds(
							tr.Position.X - radiusExtended, tr.Position.Y - radiusExtended, tr.Position.Z - radiusExtended,
							tr.Position.X + radiusExtended, tr.Position.Y + radiusExtended, tr.Position.Z + radiusExtended );
						bTotal.Add( ref b2 );

						SpaceBoundsOctreeOverride = bTotal;
					}
				}
			}

			//GetBox( out var box );
			//box.ToBounds( out var bounds );
			//newBounds = SpaceBounds.Merge( newBounds, new SpaceBounds( bounds ) );
		}

		protected override void OnCheckSelectionByRay( CheckSelectionByRayContext context )
		{
			base.OnCheckSelectionByRay( context );

			context.thisObjectWasChecked = true;
			GetBox( out var box );
			if( box.Intersects( context.ray, out var scale1, out var scale2 ) )
				context.thisObjectResultRayScale = Math.Min( scale1, scale2 );
		}

		void CalculateGroundRelativeVelocity()
		{
			groundRelativeVelocity = GetLinearVelocity();
			//if( PhysicalBody != null )
			//{
			//	groundRelativeVelocity = GetLinearVelocity();
			//	//!!!!moving ground
			//	//if( groundBody != null && groundBody.AngularVelocity.Value.LengthSquared() < .3f )
			//	//	groundRelativeVelocity -= groundBody.LinearVelocity;
			//}
			//else
			//	groundRelativeVelocity = Vector3.Zero;

			//groundRelativeVelocityToSmooth
			if( groundRelativeVelocitySmoothArray == null )
			{
				var seconds = .2f;
				var count = ( seconds / Time.SimulationDelta ) + .999f;
				groundRelativeVelocitySmoothArray = new Vector3[ (int)count ];
			}
			for( int n = 0; n < groundRelativeVelocitySmoothArray.Length - 1; n++ )
				groundRelativeVelocitySmoothArray[ n ] = groundRelativeVelocitySmoothArray[ n + 1 ];
			groundRelativeVelocitySmoothArray[ groundRelativeVelocitySmoothArray.Length - 1 ] = groundRelativeVelocity;
			groundRelativeVelocitySmooth = Vector3.Zero;
			for( int n = 0; n < groundRelativeVelocitySmoothArray.Length; n++ )
				groundRelativeVelocitySmooth += groundRelativeVelocitySmoothArray[ n ];
			groundRelativeVelocitySmooth /= (float)groundRelativeVelocitySmoothArray.Length;
		}

		[Browsable( false )]
		public Vector3 GroundRelativeVelocity
		{
			get { return groundRelativeVelocity; }
		}

		[Browsable( false )]
		public Vector3 GroundRelativeVelocitySmooth
		{
			get { return groundRelativeVelocitySmooth; }
		}

		public Vector3 GetLinearVelocity()
		{
			if( PhysicalBody != null )
				return PhysicalBody.LinearVelocity;

			//!!!!impl for non-physics mode
			return Vector3.Zero;

			//return lastLinearVelocity;

			//if( EngineApp.IsSimulation )
			//	return ( GetTransform().Position - lastSimulationStepPosition ) / Time.SimulationDelta;
			//return Vector3.Zero;
		}

		public void GetBox( out Box box )
		{
			var tr = TransformV;
			var trPosition = tr.Position;
			tr.Rotation.ToMatrix3( out var rot );

			if( dynamicData != null )
				box = new Box( ref dynamicData.LocalBoundsDefault, ref trPosition, ref rot );
			else
				box = new Box( trPosition, Vector3.One, rot );
		}

		void DebugDraw( Viewport viewport )
		{
			var renderer = viewport.Simple3DRenderer;
			GetBox( out var box );
			var points = box.ToPoints();

			renderer.AddLine( points[ 0 ], points[ 1 ], -1 );
			renderer.AddLine( points[ 1 ], points[ 2 ], -1 );
			renderer.AddLine( points[ 3 ], points[ 2 ], -1 );
			renderer.AddLine( points[ 3 ], points[ 0 ], -1 );

			renderer.AddLine( points[ 4 ], points[ 5 ], -1 );
			renderer.AddLine( points[ 5 ], points[ 6 ], -1 );
			renderer.AddLine( points[ 7 ], points[ 6 ], -1 );
			renderer.AddLine( points[ 7 ], points[ 4 ], -1 );

			renderer.AddLine( points[ 0 ], points[ 4 ], -1 );
			renderer.AddLine( points[ 1 ], points[ 5 ], -1 );
			renderer.AddLine( points[ 2 ], points[ 6 ], -1 );
			renderer.AddLine( points[ 3 ], points[ 7 ], -1 );

			//renderer.AddArrow( points[ 0 ], points[ 1 ], 0, 0, true, 0 );
			//renderer.AddLine( points[ 1 ], points[ 2 ], -1 );
			//renderer.AddArrow( points[ 3 ], points[ 2 ], 0, 0, true, 0 );
			//renderer.AddLine( points[ 3 ], points[ 0 ], -1 );

			//renderer.AddArrow( points[ 4 ], points[ 5 ], 0, 0, true, 0 );
			//renderer.AddLine( points[ 5 ], points[ 6 ], -1 );
			//renderer.AddArrow( points[ 7 ], points[ 6 ], 0, 0, true, 0 );
			//renderer.AddLine( points[ 7 ], points[ 4 ], -1 );

			//renderer.AddLine( points[ 0 ], points[ 4 ], -1 );
			//renderer.AddLine( points[ 1 ], points[ 5 ], -1 );
			//renderer.AddLine( points[ 2 ], points[ 6 ], -1 );
			//renderer.AddLine( points[ 3 ], points[ 7 ], -1 );
		}

		void DebugRenderSeat( ViewportRenderingContext context, DynamicData.SeatItem seatItem )
		{
			var renderer = context.Owner.Simple3DRenderer;
			var vehicleTransform = TransformV;

			//seat
			{
				var color = new ColorValue( 0, 1, 0 );
				renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
				var tr = vehicleTransform * seatItem.Transform;
				var p = tr * new Vector3( 0, 0, 0 );
				renderer.AddSphere( new Sphere( p, 0.1 ), 16 );
				renderer.AddArrow( p, tr * new Vector3( 1, 0, 0 ) );
			}

			//exit
			{
				var color = new ColorValue( 1, 0, 0 );
				renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
				var tr = vehicleTransform * seatItem.ExitTransform;
				var p = tr * new Vector3( 0, 0, 0 );
				renderer.AddSphere( new Sphere( p, 0.1 ), 16 );
				renderer.AddArrow( p, tr * new Vector3( 1, 0, 0 ) );
			}
		}

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			//!!!!slowly?

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;

				if( DebugVisualization )
				{
					var scene = context.Owner.AttachedScene;

					if( scene != null && context.SceneDisplayDevelopmentDataInThisApplication && dynamicData != null )//!!!! scene.DisplayPhysicalObjects )
					{
						var renderer = context.Owner.Simple3DRenderer;
						var tr = TransformV;

						//wheels
						if( dynamicData.Wheels != null )
						{
							//foreach( var wheel in dynamicData.Wheels )
							for( int nWheel = 0; nWheel < dynamicData.Wheels.Length; nWheel++ )
							{
								ref var wheel = ref dynamicData.Wheels[ nWheel ];

								{
									var color = new ColorValue( 1, 0, 0 );
									renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

									//!!!!slowly

									var m1 = new Matrix4( wheel.CurrentRotation.ToMatrix3(), wheel.CurrentPosition + new Vector3( 0, -wheel.Width * 0.5, 0 ) );
									var m2 = new Matrix4( wheel.CurrentRotation.ToMatrix3(), wheel.CurrentPosition + new Vector3( 0, wheel.Width * 0.5, 0 ) );

									var pos1 = ( tr.ToMatrix4() * m1 ).GetTranslation();
									var pos2 = ( tr.ToMatrix4() * m2 ).GetTranslation();


									////!!!!
									//PhysicalBody.GetShapeCenterOfMass( out var shapeCenterOfMass );
									//pos1 -= shapeCenterOfMass;
									//pos2 -= shapeCenterOfMass;


									//var pos1 = tr * ( wheel.LocalPosition + new Vector3( 0, -wheel.Width * 0.5, 0 ) );
									//var pos2 = tr * ( wheel.LocalPosition + new Vector3( 0, wheel.Width * 0.5, 0 ) );

									var cylinder = new Cylinder( pos1, pos2, wheel.Diameter * 0.5 );
									renderer.AddCylinder( cylinder );
								}

								//можно transform указывать
								//var wheelTransform = Matrix4.FromTranslate(tr
								//renderer.AddCylinder( z, 1, WheelDiameter * 0.5, WheelWidth );

								//if( vehicle != null )
								//{
								//	var wheelInfo = vehicle.GetWheelInfo( nWheel );

								//	ColorValue wheelColor;
								//	if( wheelInfo.RaycastInfo.IsInContact )
								//		wheelColor = new ColorValue( 0, 0, 1 );
								//	else
								//		wheelColor = new ColorValue( 1, 0, 1 );

								//	var transform = wheelInfo.WorldTransform;
								//	var wheelPosWS = transform.Origin;

								//	int RightAxis = 1;

								//	var axle = new Internal.BulletSharp.Math.BVector3(
								//		transform[ 0, RightAxis ],
								//		transform[ 1, RightAxis ],
								//		transform[ 2, RightAxis ] );

								//	var to1 = wheelPosWS + axle;
								//	var to2 = wheelInfo.RaycastInfo.ContactPointWS;

								//	//debug wheels (cylinders)

								//	renderer.SetColor( wheelColor, wheelColor * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );
								//	renderer.AddLine( BulletPhysicsUtility.Convert( wheelPosWS ), BulletPhysicsUtility.Convert( to1 ) );
								//	renderer.AddLine( BulletPhysicsUtility.Convert( wheelPosWS ), BulletPhysicsUtility.Convert( to2 ) );

								//	//debugDrawer.DrawLine( ref wheelPosWS, ref to1, ref wheelColor );
								//	//debugDrawer.DrawLine( ref wheelPosWS, ref to2, ref wheelColor );
								//}
							}

							//if( dynamicData.constraint != null )
							//{
							//	dynamicData.constraint.GetData2( out var wheel0, out var wheel1, out var wheel2, out var wheel3 );
							//	renderer.AddSphere( new Sphere( wheel0, 0.01 ) );
							//	renderer.AddSphere( new Sphere( wheel1, 0.01 ) );
							//	renderer.AddSphere( new Sphere( wheel2, 0.01 ) );
							//	renderer.AddSphere( new Sphere( wheel3, 0.01 ) );
							//}

						}

						//seats
						for( int n = 0; n < dynamicData.Seats.Length; n++ )
						{
							var seatItem = dynamicData.Seats[ n ];
							DebugRenderSeat( context, seatItem );
						}

						//!!!!

						//var tr = GetTransform();
						//var scaleFactor = tr.Scale.MaxComponent();

						//renderer.SetColor( new ColorValue( 1, 0, 0, 1 ) );

						////eye position
						//renderer.SetColor( new ColorValue( 0, 1, 0, 1 ) );
						//renderer.AddSphere( new Sphere( TransformV * EyePosition.Value, .05f ), 16 );
					}
				}

				//!!!!
				var showLabels = /*show &&*/ dynamicData == null;
				if( !showLabels )
					context2.disableShowingLabelForThisObject = true;

#if !DEPLOY
				//draw selection
				if( EngineApp.IsEditor )
				{
					if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
					{
						ColorValue color;
						if( context2.selectedObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.SelectedColor;
						else //if( context2.canSelectObjects.Contains( this ) )
							color = ProjectSettings.Get.Colors.CanSelectColor;
						//else
						//	color = ProjectSettings.Get.SceneShowPhysicsDynamicActiveColor;

						var viewport = context.Owner;

						viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
						DebugDraw( viewport );
					}
				}
#endif
			}
		}

		//public bool GetEyesPosition( out Vector3 position )
		//{
		//	//slowly?

		//	var meshInSpace = GetComponent<MeshInSpace>( onlyEnabledInHierarchy: true );
		//	if( meshInSpace != null )
		//	{
		//		var controller = GetAnimationController();
		//		if( controller != null && controller.Bones != null )
		//		{
		//			var bones = new List<SkeletonBone>();
		//			foreach( var bone in controller.Bones )
		//			{
		//				if( bone.Name.Contains( "eye", StringComparison.OrdinalIgnoreCase ) )
		//					bones.Add( bone );
		//			}

		//			if( bones.Count != 0 )
		//			{
		//				position = Vector3.Zero;

		//				foreach( var bone in bones )
		//				{
		//					var boneIndex = controller.GetBoneIndex( bone );
		//					Matrix4F globalMatrix = Matrix4F.Zero;
		//					controller.GetBoneGlobalTransform( boneIndex, ref globalMatrix );

		//					var m = meshInSpace.TransformV.ToMatrix4() * globalMatrix;

		//					position += m.GetTranslation();
		//				}

		//				position /= bones.Count;

		//				return true;
		//			}
		//		}
		//	}

		//	position = Vector3.Zero;
		//	return false;
		//}

		public virtual void GetFirstPersonCameraPosition( bool useEyesPositionOfModel, out Vector3 position, out Vector3 forward, out Vector3 up )
		{
			var vehicleTransform = TransformV;

			position = vehicleTransform.Position;
			forward = vehicleTransform.Rotation.GetForward();
			up = vehicleTransform.Rotation.GetUp();


			//var positionCalculated = false;

			////get eyes position from skeleton
			//if( useEyesPositionOfModel )
			//{
			//	if( GetEyesPosition( out position ) )
			//		positionCalculated = true;
			//}

			////calculate position
			//if( !positionCalculated )
			//{


			//!!!!select seat

			if( dynamicData != null )
			{
				var seatItem = dynamicData.Seats[ 0 ];

				//!!!!slowly

				var seatTransform = seatItem.Transform;
				position = vehicleTransform * ( seatTransform * seatItem.EyeOffset );
			}
		}

		//public Vector3 GetSmoothPosition()
		//{
		//	return TransformV.Position + GetSmoothCameraOffset();
		//}

		public void SoundPlay( Sound sound )
		{
			ParentScene?.SoundPlay( sound, TransformV.Position );
		}

		//public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		//{
		//	if( Components.Count == 0 )
		//	{
		//		//var inputProcessing = CreateComponent<VehicleInputProcessing>();
		//		//inputProcessing.Name = "Vehicle Input Processing";

		//		//var ai = CreateComponent<VehicleAI>();
		//		//ai.Name = "Vehicle AI";
		//		//ai.NetworkMode = NetworkModeEnum.False;
		//	}
		//}

		public void NeedRecreateDynamicData()
		{
			needRecreateDynamicData = true;
		}

		public void CreateDynamicData()
		{
			DestroyDynamicData();

			if( !EnabledInHierarchyAndIsInstance )
				return;
			if( NetworkIsClient )
				return;

			//const bool displayInEditor = false;// true;

			var scene = ParentScene;
			if( scene == null )
				return;

			var type = VehicleType.Value;
			if( type == null )
				return;

			dynamicData = new DynamicData();
			dynamicData.VehicleType = type;
			dynamicData.PhysicsMode = PhysicsMode;
			dynamicData.Mesh = type.Mesh;
			dynamicData.FrontWheelMesh = type.FrontWheelMesh;
			dynamicData.RearWheelMesh = type.RearWheelMesh;

			var tr = TransformV;

			Mesh = dynamicData.Mesh;//type.Mesh;

			//!!!!Kinematic
			if( dynamicData.PhysicsMode == PhysicsModeEnum.Basic )
				Collision = true;

			//create wheels
			if( type.Chassis.Value == NeoAxis.VehicleType.ChassisEnum._4Wheels )
			{
				dynamicData.Wheels = new DynamicData.WheelItem[ 4 ];

				//var wheelDrive = type.WheelDrive.Value;
				//var frontDrive = wheelDrive == NeoAxis.VehicleType.WheelDriveEnum.Front || wheelDrive == NeoAxis.VehicleType.WheelDriveEnum.All;
				//var rearDrive = wheelDrive == NeoAxis.VehicleType.WheelDriveEnum.Rear || wheelDrive == NeoAxis.VehicleType.WheelDriveEnum.All;

				for( int n = 0; n < dynamicData.Wheels.Length; n++ )
				{
					ref var wheel = ref dynamicData.Wheels[ n ];
					wheel.Which = (DynamicData.WhichWheel)n;

					//if( ( wheel.Which == DynamicData.WhichWheel.FrontLeft || wheel.Which == DynamicData.WhichWheel.FrontRight ) && frontDrive )
					//	wheel.WheelDrive = true;
					//if( ( wheel.Which == DynamicData.WhichWheel.RearLeft || wheel.Which == DynamicData.WhichWheel.RearRight ) && rearDrive )
					//	wheel.WheelDrive = true;

					switch( wheel.Which )
					{
					case DynamicData.WhichWheel.FrontLeft:
						wheel.InitialPosition = type.FrontWheelPosition.Value.ToVector3F();
						if( wheel.InitialPosition.Y < 0 )
							wheel.InitialPosition.Y = -wheel.InitialPosition.Y;
						break;

					case DynamicData.WhichWheel.FrontRight:
						wheel.InitialPosition = type.FrontWheelPosition.Value.ToVector3F();
						if( wheel.InitialPosition.Y > 0 )
							wheel.InitialPosition.Y = -wheel.InitialPosition.Y;
						break;

					case DynamicData.WhichWheel.RearLeft:
						wheel.InitialPosition = type.RearWheelPosition.Value.ToVector3F();
						if( wheel.InitialPosition.Y < 0 )
							wheel.InitialPosition.Y = -wheel.InitialPosition.Y;
						break;

					case DynamicData.WhichWheel.RearRight:
						wheel.InitialPosition = type.RearWheelPosition.Value.ToVector3F();
						if( wheel.InitialPosition.Y > 0 )
							wheel.InitialPosition.Y = -wheel.InitialPosition.Y;
						break;
					}

					wheel.CurrentPosition = wheel.InitialPosition;
					wheel.CurrentRotation = QuaternionF.Identity;

					switch( wheel.Which )
					{
					case DynamicData.WhichWheel.FrontLeft:
					case DynamicData.WhichWheel.FrontRight:
						wheel.Diameter = (float)type.FrontWheelDiameter.Value;
						wheel.Width = (float)type.FrontWheelWidth.Value;
						break;

					case DynamicData.WhichWheel.RearLeft:
					case DynamicData.WhichWheel.RearRight:
						wheel.Diameter = (float)type.RearWheelDiameter.Value;
						wheel.Width = (float)type.RearWheelWidth.Value;
						break;
					}




					////init physics

					//if( dynamicData.MainBody != null )
					//{
					//	//SimplePhysics mode
					//	if( dynamicData.SimulationMode == SimulationModeEnum.RealPhysics )
					//	{
					//		//create a rigid body of wheel
					//		{
					//			//need set ShowInEditor = false before AddComponent
					//			var body = ComponentUtility.CreateComponent<RigidBody>( null, false, false );
					//			body.DisplayInEditor = displayInEditor;
					//			AddComponent( body );
					//			//var body = CreateComponent<RigidBody>( enabled: false );

					//			body.CanBeSelected = false;
					//			body.SaveSupport = false;
					//			body.CloneSupport = false;

					//			body.Name = "Wheel " + wheel.Which.ToString() + " Collision Body";
					//			body.MotionType = PhysicsMotionType.Dynamic;
					//			body.Mass = wheel.Front ? type.FrontWheelMass : type.RearWheelMass;

					//			body.MaterialFriction = type.WheelFriction;
					//			//!!!!
					//			//body.MaterialFrictionMode = PhysicalMaterial.FrictionModeEnum.AnisotropicRolling;
					//			//MaterialAnisotropicFriction
					//			//MaterialSpinningFriction
					//			//MaterialRollingFriction
					//			//body.MaterialRestitution

					//			body.Transform = tr * new Transform( wheel.LocalPosition, Quaternion.Identity );


					//			//!!!!instancing geometry
					//			//!!!!round
					//			//!!!!more segments

					//			SimpleMeshGenerator.GenerateCylinder( 1, wheel.Diameter / 2, wheel.Width, 64, true, true, true, out Vector3F[] vertices, out var indices );

					//			var shape = body.CreateComponent<CollisionShape_Mesh>();
					//			shape.Vertices = vertices;
					//			shape.Indices = indices;
					//			shape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.Convex;

					//			//var shape = body.CreateComponent<CollisionShape_Sphere>();
					//			//shape.Radius = wheel.Diameter / 2;

					//			//body.ContactsCollect = true;

					//			body.Enabled = true;

					//			wheel.RigidBody = body;
					//		}

					//		//create constraint
					//		if( wheel.RigidBody != null )
					//		{
					//			//need set ShowInEditor = false before AddComponent
					//			var c = ComponentUtility.CreateComponent<Constraint>( null, false, false );
					//			c.DisplayInEditor = displayInEditor;
					//			AddComponent( c );
					//			//var body = CreateComponent<RigidBody>( enabled: false );

					//			c.CanBeSelected = false;
					//			c.SaveSupport = false;
					//			c.CloneSupport = false;

					//			c.Name = "Wheel " + wheel.Which.ToString() + " Constraint";
					//			c.CollisionsBetweenLinkedBodies = false;
					//			c.BodyA = ReferenceUtility.MakeThisReference( c, dynamicData.MainBody );
					//			c.BodyB = ReferenceUtility.MakeThisReference( c, wheel.RigidBody );

					//			c.Transform = new Transform( wheel.RigidBody.TransformV.Position, tr.Rotation * Quaternion.FromRotateByZ( -Math.PI / 2 ) );
					//			//c.Transform = new Transform( wheel.RigidBody.TransformV.Position, tr.Rotation * Quaternion.FromRotateByZ( Math.PI / 2 ) );


					//			//!!!!need

					//			//c.AngularAxisX = PhysicsAxisMode.Free;
					//			//c.AngularAxisXMotor = true;


					//			//steering
					//			if( wheel.Which == DynamicData.WhichWheel.FrontLeft || wheel.Which == DynamicData.WhichWheel.FrontRight )
					//			{
					//				c.AngularAxisZ = PhysicsAxisMode.Limited;
					//				c.AngularAxisZLimitLow = -type.MaxSteeringAngle.Value;
					//				c.AngularAxisZLimitHigh = type.MaxSteeringAngle.Value;
					//				c.AngularAxisZMotor = true;
					//				c.AngularAxisZServo = true;
					//			}

					//			//!!!!impl
					//			////suspension
					//			//c.LinearAxisZ = PhysicsAxisMode.Limited;
					//			//c.LinearAxisZLimitHigh = SuspensionTravel.Value.Maximum;
					//			//c.LinearAxisZLimitLow = SuspensionTravel.Value.Minimum;

					//			////!!!!
					//			//c.LinearAxisZLimitHigh = 0;
					//			//c.LinearAxisZLimitLow = 0;

					//			//c.LinearAxisZLimitHigh = -0.2;// 0.1;
					//			//c.LinearAxisZLimitLow = 0.1;

					//			//c.LinearAxisZMotor = true;
					//			//c.LinearAxisZMotorMaxForce = 0;
					//			//c.LinearAxisZMotorTargetVelocity = 0;

					//			//c.LinearAxisZSpring = true;
					//			//c.LinearAxisZSpringStiffness = 1000000000;
					//			//c.LinearAxisZSpringDamping = 0;

					//			//!!!!засыпать
					//			//c.AutoActivateBodies = true;

					//			c.Enabled = true;

					//			wheel.Constraint = c;
					//		}
					//	}

					//	////Raycast mode
					//	//if( dynamicData.SimulationMode == SimulationModeEnum.Raycast )
					//	//{
					//	//	var vehicle = dynamicData.raycastVehicle;

					//	//	//!!!!
					//	//	float suspensionRestLength = 0.6f;

					//	//	//!!!!
					//	//	//const float connectionHeight = 1.2f;

					//	//	//!!!!
					//	//	//float wheelRadius = 0.7f;
					//	//	//float wheelWidth = 0.4f;

					//	//	var connectionPoint = BulletPhysicsUtility.Convert( wheel.LocalPosition );
					//	//	//var connectionPoint = BulletPhysicsUtility.Convert( new Vector3( 1.0 - ( 0.3f * wheelWidth ), connectionHeight, 2 * 1.0 - wheelRadius ) );

					//	//	//!!!!
					//	//	var wheelDirection = BulletPhysicsUtility.Convert( new Vector3( 0, 0, -1 ) );
					//	//	//var wheelAxle = BulletPhysicsUtility.Convert( new Vector3( 1, 0, 0 ) );
					//	//	var wheelAxle = BulletPhysicsUtility.Convert( new Vector3( 0, -1, 0 ) );

					//	//	var wheelInfo = vehicle.AddWheel( connectionPoint, wheelDirection, wheelAxle, suspensionRestLength, wheel.Diameter / 2, vehicleTuning, wheel.Front );


					//	//	//Vector3 wheelDirection = Vector3.Zero;
					//	//	//Vector3 wheelAxle = Vector3.Zero;

					//	//	//wheelDirection[ upIndex ] = -1;
					//	//	//wheelAxle[ rightIndex ] = -1;


					//	//	//bool isFrontWheel = true;
					//	//	//var connectionPoint = new Vector3( CUBE_HALF_EXTENTS - ( 0.3f * wheelWidth ), connectionHeight, 2 * CUBE_HALF_EXTENTS - wheelRadius );
					//	//	//vehicle.AddWheel( connectionPoint, wheelDirection, wheelAxle, suspensionRestLength, wheelRadius, tuning, isFrontWheel );

					//	//	//connectionPoint = new Vector3( -CUBE_HALF_EXTENTS + ( 0.3f * wheelWidth ), connectionHeight, 2 * CUBE_HALF_EXTENTS - wheelRadius );
					//	//	//vehicle.AddWheel( connectionPoint, wheelDirection, wheelAxle, suspensionRestLength, wheelRadius, tuning, isFrontWheel );

					//	//	//isFrontWheel = false;
					//	//	//connectionPoint = new Vector3( -CUBE_HALF_EXTENTS + ( 0.3f * wheelWidth ), connectionHeight, -2 * CUBE_HALF_EXTENTS + wheelRadius );
					//	//	//vehicle.AddWheel( connectionPoint, wheelDirection, wheelAxle, suspensionRestLength, wheelRadius, tuning, isFrontWheel );

					//	//	//connectionPoint = new Vector3( CUBE_HALF_EXTENTS - ( 0.3f * wheelWidth ), connectionHeight, -2 * CUBE_HALF_EXTENTS + wheelRadius );
					//	//	//vehicle.AddWheel( connectionPoint, wheelDirection, wheelAxle, suspensionRestLength, wheelRadius, tuning, isFrontWheel );


					//	//	//!!!!
					//	//	float wheelFriction = 1000; //float.MaxValue
					//	//	float suspensionStiffness = 20.0f;
					//	//	float suspensionDamping = 2.3f;
					//	//	float suspensionCompression = 4.4f;
					//	//	float rollInfluence = 0.1f; //1.0f;


					//	//	//for( int i = 0; i < vehicle.NumWheels; i++ )
					//	//	//{
					//	//	//	WheelInfo wheelInfo = vehicle.GetWheelInfo( i );
					//	//	wheelInfo.SuspensionStiffness = suspensionStiffness;
					//	//	wheelInfo.WheelsDampingRelaxation = suspensionDamping;
					//	//	wheelInfo.WheelsDampingCompression = suspensionCompression;
					//	//	wheelInfo.FrictionSlip = wheelFriction;
					//	//	wheelInfo.RollInfluence = rollInfluence;
					//	//	//}


					//	//}
					//}


					//!!!!


					////create mesh in space
					//{
					//	//need set ShowInEditor = false before AddComponent
					//	var meshInSpace = ComponentUtility.CreateComponent<MeshInSpace>( null, false, false );
					//	meshInSpace.DisplayInEditor = displayInEditor;
					//	AddComponent( meshInSpace, -1 );
					//	//var meshInSpace = CreateComponent<MeshInSpace>( enabled: false );

					//	meshInSpace.CanBeSelected = false;
					//	meshInSpace.SaveSupport = false;
					//	meshInSpace.CloneSupport = false;

					//	meshInSpace.Name = "Wheel " + wheel.Which.ToString() + " Mesh In Space";
					//	meshInSpace.Mesh = wheel.Front ? type.FrontWheelMesh : type.RearWheelMesh;

					//	//SimplePhysics mode
					//	if( dynamicData.SimulationMode == SimulationModeEnum.RealPhysics )
					//	{
					//		//attach to the body
					//		if( wheel.RigidBody != null )
					//		{
					//			if( wheel.Right )
					//			{
					//				var offset = meshInSpace.CreateComponent<TransformOffset>();
					//				offset.Name = "Transform Offset";
					//				offset.RotationOffset = Quaternion.FromRotateByZ( Math.PI );
					//				offset.Source = ReferenceUtility.MakeThisReference( offset, wheel.RigidBody, "Transform" );

					//				meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, offset, "Result" );
					//			}
					//			else
					//				meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, wheel.RigidBody, "Transform" );
					//		}
					//	}

					//	////Raycast mode
					//	//if( dynamicData.SimulationMode == SimulationModeEnum.Raycast )
					//	//{

					//	//	//!!!!

					//	//}


					//	//else
					//	//{
					//	//	//var offset = meshInSpace.CreateComponent<TransformOffset>();
					//	//	//offset.Name = "Transform Offset";
					//	//	//offset.PositionOffset = wheel.LocalPosition;

					//	//	////!!!!может тело повернуть
					//	//	////if(wheel.Right)
					//	//	////offset.RotationOffset = z;

					//	//	//offset.Source = ReferenceUtility.MakeThisReference( offset, this, "Transform" );

					//	//	////!!!!slowly maybe. where else
					//	//	//meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, offset, "Result" );
					//	//}

					//	meshInSpace.Enabled = true;

					//	wheel.MeshInSpace = meshInSpace;
					//}

				}
			}

			//create constraint
			if( dynamicData.PhysicsMode == PhysicsModeEnum.Basic && type.Chassis.Value == NeoAxis.VehicleType.ChassisEnum._4Wheels && PhysicalBody != null && !PhysicalBody.Disposed && PhysicalBody.MotionType == PhysicsMotionType.Dynamic )
			{
				//!!!!center offset

				var physicsWorldData = PhysicalBody.PhysicsWorld;
				if( physicsWorldData != null )
				{
					unsafe
					{
						var wheelsSettings = stackalloc Scene.PhysicsWorldClass.VehicleWheelSettings[ dynamicData.Wheels.Length ];

						for( int n = 0; n < dynamicData.Wheels.Length; n++ )
						{
							var which = (DynamicData.WhichWheel)n;
							var isFront = which == DynamicData.WhichWheel.FrontLeft || which == DynamicData.WhichWheel.FrontRight;

							ref var wheel = ref dynamicData.Wheels[ n ];
							ref var wheelSettings = ref wheelsSettings[ n ];

							wheelSettings.Position = wheel.InitialPosition + new Vector3F( 0, 0, wheel.Diameter * 0.5f );
							//wheelSettings.Position = wheel.InitialPosition;
							wheelSettings.Direction = new Vector3F( 0, 0, -1 );

							if( isFront )
							{
								wheelSettings.SuspensionMinLength = (float)type.FrontWheelSuspensionMinLength;
								wheelSettings.SuspensionMaxLength = (float)type.FrontWheelSuspensionMaxLength;
								wheelSettings.SuspensionPreloadLength = (float)type.FrontWheelSuspensionPreloadLength;
								wheelSettings.SuspensionFrequency = (float)type.FrontWheelSuspensionFrequency;
								wheelSettings.SuspensionDamping = (float)type.FrontWheelSuspensionDamping;
							}
							else
							{
								wheelSettings.SuspensionMinLength = (float)type.RearWheelSuspensionMinLength;
								wheelSettings.SuspensionMaxLength = (float)type.RearWheelSuspensionMaxLength;
								wheelSettings.SuspensionPreloadLength = (float)type.RearWheelSuspensionPreloadLength;
								wheelSettings.SuspensionFrequency = (float)type.RearWheelSuspensionFrequency;
								wheelSettings.SuspensionDamping = (float)type.RearWheelSuspensionDamping;
							}

							wheelSettings.Radius = wheel.Diameter * 0.5f;
							wheelSettings.Width = wheel.Width;

							//Wheeled specific
							double mass = isFront ? type.FrontWheelMass.Value : type.RearWheelMass.Value;
							wheelSettings.Inertia = 0.5f * (float)mass * wheelSettings.Radius * wheelSettings.Radius;

							if( isFront )
							{
								wheelSettings.MaxSteerAngle = (float)type.FrontWheelMaxSteeringAngle.Value.InRadians();
								wheelSettings.MaxBrakeTorque = (float)type.FrontWheelMaxBrakeTorque;
								wheelSettings.MaxHandBrakeTorque = (float)type.FrontWheelMaxHandBrakeTorque;
								wheelSettings.AngularDamping = (float)type.FrontWheelAngularDamping;
							}
							else
							{
								wheelSettings.MaxSteerAngle = (float)type.RearWheelMaxSteeringAngle.Value.InRadians();
								wheelSettings.MaxBrakeTorque = (float)type.RearWheelMaxBrakeTorque;
								wheelSettings.MaxHandBrakeTorque = (float)type.RearWheelMaxHandBrakeTorque;
								wheelSettings.AngularDamping = (float)type.RearWheelAngularDamping;
							}
						}

						var transmissionGearRatios = type.TransmissionGearRatios.Value;
						var transmissionReverseGearRatios = type.TransmissionReverseGearRatios.Value;

						fixed( double* pTransmissionGearRatios = transmissionGearRatios, pTransmissionReverseGearRatios = transmissionReverseGearRatios )
						{
							var visualScale = Vector3F.One;
							dynamicData.constraint = physicsWorldData.CreateConstraintVehicle( this, PhysicalBody, dynamicData.Wheels.Length, wheelsSettings, ref visualScale, (float)type.FrontWheelAntiRollBarStiffness, (float)type.RearWheelAntiRollBarStiffness, (float)type.MaxPitchRollAngle.Value.InRadians(), (float)type.EngineMaxTorque, (float)type.EngineMinRPM, (float)type.EngineMaxRPM, type.TransmissionAuto, transmissionGearRatios.Length, pTransmissionGearRatios, transmissionReverseGearRatios.Length, pTransmissionReverseGearRatios, (float)type.TransmissionSwitchTime, (float)type.TransmissionClutchReleaseTime, (float)type.TransmissionSwitchLatency, (float)type.TransmissionShiftUpRPM, (float)type.TransmissionShiftDownRPM, (float)type.TransmissionClutchStrength, type.FrontWheelDrive, type.RearWheelDrive, (float)type.FrontWheelDifferentialRatio, (float)type.FrontWheelDifferentialLeftRightSplit, (float)type.FrontWheelDifferentialLimitedSlipRatio, (float)type.FrontWheelDifferentialEngineTorqueRatio, (float)type.RearWheelDifferentialRatio, (float)type.RearWheelDifferentialLeftRightSplit, (float)type.RearWheelDifferentialLimitedSlipRatio, (float)type.RearWheelDifferentialEngineTorqueRatio, type.MaxSlopeAngle.Value.InRadians().ToRadianF() );
						}
					}
				}
			}


			//LocalBoundsDefault
			{
				var meshResult = dynamicData.Mesh?.Result;
				if( meshResult != null )
				{
					dynamicData.LocalBoundsDefault = meshResult.SpaceBounds.BoundingBox;

					if( dynamicData.Wheels != null )
					{
						for( int n = 0; n < dynamicData.Wheels.Length; n++ )
						{
							ref var wheel = ref dynamicData.Wheels[ n ];

							var minZ = wheel.InitialPosition.Z - wheel.Diameter * 0.5f;
							if( minZ < dynamicData.LocalBoundsDefault.Minimum.Z )
								dynamicData.LocalBoundsDefault.Minimum.Z = minZ;
						}
					}
				}
				else
					dynamicData.LocalBoundsDefault = new Bounds( -0.5, -0.5, -0.5, 0.5, 0.5, 0.5 );

				foreach( var p in dynamicData.LocalBoundsDefault.ToPoints() )
				{
					var l = (float)p.Length();
					if( l > dynamicData.LocalBoundsDefaultBoundingRadius )
						dynamicData.LocalBoundsDefaultBoundingRadius = l;
				}
			}

			//!!!!slowly. make caching in the VehicleType
			var seatComponents = dynamicData.VehicleType.GetComponents<VehicleSeat>();
			if( seatComponents.Length != 0 )
			{
				var seatItems = new List<DynamicData.SeatItem>( seatComponents.Length );

				foreach( var seatComponent in seatComponents )
				{
					if( seatComponent.Enabled )
					{
						var seatItem = new DynamicData.SeatItem();
						seatItem.SeatComponent = seatComponent;
						seatItem.Transform = seatComponent.Transform;
						seatItem.EyeOffset = seatComponent.EyeOffset;
						seatItem.ExitTransform = seatComponent.ExitTransform;

						seatItems.Add( seatItem );
					}
				}

				if( seatItems.Count != 0 )
					dynamicData.Seats = seatItems.ToArray();
			}

			//also can take seats from Vehicle component

			if( dynamicData.Seats == null )
				dynamicData.Seats = Array.Empty<DynamicData.SeatItem>();

			UpdateAdditionalItems();

			//UpdateSpaceBoundsOverride();
			SpaceBoundsUpdate();

			//needBeActiveBecausePhysicsVelocity = false;
			//lastTransformToCalculateDynamicState = TransformV;
			//needBeActiveBecauseTransformChange = false;
			//lastLinearVelocity = Vector3.Zero;

			driverInputNeedUpdate = true;
			needRecreateDynamicData = false;
		}

		public void DestroyDynamicData()
		{
			if( dynamicData != null )
			{
				AdditionalItems = null;

				//if( dynamicData.Wheels != null )
				//{
				//	for( int n = 0; n < dynamicData.Wheels.Length; n++ )
				//	{
				//		ref var wheel = ref dynamicData.Wheels[ n ];

				//		//wheel.MeshInSpace?.Dispose();
				//		//wheel.Constraint?.Dispose();
				//		//wheel.RigidBody?.Dispose();
				//	}
				//}

				dynamicData.constraint?.Dispose();
				dynamicData.constraint = null;

				//!!!!Kinematic
				if( dynamicData.PhysicsMode == PhysicsModeEnum.Basic )
					Collision = false;

				dynamicData = null;
				//needBeActiveBecauseDriverInput = false;
			}
		}

		///////////////////////////////////////////////

		public int GetFreeSeat()// ObjectInSpace forObject )
		{
			if( dynamicData != null )
			{
				var count = dynamicData.Seats.Length;
				for( int n = 0; n < count; n++ )
				{
					if( n >= ObjectsOnSeats.Count || ObjectsOnSeats[ n ].Value == null )
						return n;
				}
			}
			return -1;
		}

		public int GetSeatIndexByObject( ObjectInSpace obj )
		{
			if( dynamicData != null )
			{
				var count = dynamicData.Seats.Length;
				for( int n = 0; n < count; n++ )
				{
					if( n < ObjectsOnSeats.Count && ObjectsOnSeats[ n ].Value == obj )
						return n;
				}
			}
			return -1;
		}

		public void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
			//control by a character
			var character = gameMode.ObjectControlledByPlayer.Value as Character;
			if( character != null )
			{
				var seatIndex = GetFreeSeat();// character );
				if( seatIndex != -1 )
				{
					info = new InteractiveObjectObjectInfo();
					info.AllowInteract = true;
					info.SelectionTextInfo.Add( Name );

					//!!!!!too hardcoded EKeys and code

					info.SelectionTextInfo.Add( "Press E to control the vehicle." );
				}
			}
		}

		public ObjectInSpace GetObjectOnSeat( int seatIndex )
		{
			if( seatIndex < ObjectsOnSeats.Count )
				return ObjectsOnSeats[ seatIndex ];
			return null;
		}

		public virtual bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message )
		{
			var keyDown = message as InputMessageKeyDown;
			if( keyDown != null )
			{
				//!!!!too hardcoded EKeys and code

				if( keyDown.Key == EKeys.E )
				{
					//start control by a character
					var character = gameMode.ObjectControlledByPlayer.Value as Character;
					if( character != null )
					{
						var seatIndex = GetFreeSeat();
						if( seatIndex != -1 )
						{
							//create VehicleInputProcessing if not exists
							{
								//!!!!check for networking

								var inputProcessing = GetComponent<VehicleInputProcessing>();
								if( inputProcessing == null )
									inputProcessing = CreateComponent<VehicleInputProcessing>();
							}

							if( NetworkIsClient )
							{
								var writer = BeginNetworkMessageToServer( "PutObjectToSeat" );
								if( writer != null )
								{
									writer.WriteVariableInt32( seatIndex );
									writer.WriteVariableUInt64( (ulong)character.NetworkID );
									EndNetworkMessage();
								}
							}
							else
							{
								PutObjectToSeat( seatIndex, character );
								gameMode.ObjectControlledByPlayer = ReferenceUtility.MakeRootReference( this );
								GameMode.PlayScreen?.ParentContainer?.Viewport?.NotifyInstantCameraMovement();
							}

							return true;
						}


						//var seat = GetComponent<VehicleSeat>();
						//if( seat != null && !seat.Character.ReferenceOrValueSpecified )
						//{
						//	if( NetworkIsClient )
						//	{
						//		var writer = BeginNetworkMessageToServer( "PutObjectToSeat" );
						//		if( writer != null )
						//		{
						//			writer.WriteVariableUInt64( (ulong)character.NetworkID );
						//			writer.WriteVariableUInt64( (ulong)seat.NetworkID );
						//			EndNetworkMessage();
						//		}
						//	}
						//	else
						//	{
						//		seat.PutCharacterToSeat( character );
						//		gameMode.ObjectControlledByPlayer = ReferenceUtility.MakeRootReference( this );

						//		GameMode.PlayScreen?.ParentContainer?.Viewport?.NotifyInstantCameraMovement();
						//	}

						//	return true;
						//}
					}
				}
			}

			return false;
		}

		public void ObjectInteractionEnter( ObjectInteractionContext context )
		{
		}

		public void ObjectInteractionExit( ObjectInteractionContext context )
		{
		}

		public void ObjectInteractionUpdate( ObjectInteractionContext context )
		{
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;


			//!!!!security check is needed


			if( message == "PutObjectToSeat" )
			{
				var seatIndex = reader.ReadVariableInt32();
				var characterNetworkID = (long)reader.ReadVariableUInt64();
				if( !reader.Complete() )
					return false;

				var obj = ParentRoot.HierarchyController.GetComponentByNetworkID( characterNetworkID ) as ObjectInSpace;
				if( obj != null )
				{
					var networkLogic = NetworkLogicUtility.GetNetworkLogic( obj );
					if( networkLogic != null )
					{
						PutObjectToSeat( seatIndex, obj );
						networkLogic.ServerChangeObjectControlled( client.User, this );
					}
				}
			}

			return true;
		}

		public virtual void PutObjectToSeat( int seatIndex, ObjectInSpace obj )
		{
			while( seatIndex >= ObjectsOnSeats.Count )
				ObjectsOnSeats.Add( null );
			ObjectsOnSeats[ seatIndex ] = ReferenceUtility.MakeRootReference( obj );

			UpdateObjectOnSeat( seatIndex );

			remainingTimeToUpdateObjectsOnSeat = 0;
		}

		public virtual void RemoveObjectFromSeat( int seatIndex, bool resetDriverInput )
		{
			if( dynamicData != null )
			{
				DynamicData.SeatItem seatItem = null;
				if( seatIndex < dynamicData.Seats.Length )
					seatItem = dynamicData.Seats[ seatIndex ];

				var objectOnSeat = GetObjectOnSeat( seatIndex );
				if( objectOnSeat != null && seatItem != null )
				{
					if( !objectOnSeat.Disposed )
					{
						var character = objectOnSeat as Character;
						if( character != null && !character.Disposed )
						{
							character.Collision = true;
							//character.UpdateCollisionBody();
							character.Visible = true;
							character.SetTransformAndTurnToDirectionInstantly( TransformV * seatItem.ExitTransform );
						}
					}

					ObjectsOnSeats[ seatIndex ] = null;
				}
			}

			if( resetDriverInput )
			{
				Throttle = 0;
				Brake = 0;
				HandBrake = 1;
				Steering = 0;
			}

			remainingTimeToUpdateObjectsOnSeat = 0;
		}

		void UpdateAdditionalItems()
		{
			if( dynamicData.Wheels != null )
			{
				var wheelCount = dynamicData.Wheels.Length;

				var itemCount = 0;
				for( int n = 0; n < wheelCount; n++ )
				{
					ref var wheel = ref dynamicData.Wheels[ n ];
					var mesh = wheel.Front ? dynamicData.FrontWheelMesh : dynamicData.RearWheelMesh;
					if( mesh != null )
						itemCount++;
				}

				var currentAdditionalItems = AdditionalItems;
				if( currentAdditionalItems == null || currentAdditionalItems.Length != itemCount )
				{
					if( itemCount != 0 )
						currentAdditionalItems = new AdditionalItem[ itemCount ];
					else
						currentAdditionalItems = null;
				}

				var currentIndex = 0;
				for( int n = 0; n < wheelCount; n++ )
				{
					ref var wheel = ref dynamicData.Wheels[ n ];
					var mesh = wheel.Front ? dynamicData.FrontWheelMesh : dynamicData.RearWheelMesh;

					if( mesh != null )
					{
						ref var item = ref currentAdditionalItems[ currentIndex++ ];

						item.Mesh = wheel.Front ? dynamicData.FrontWheelMesh : dynamicData.RearWheelMesh;
						item.Position = wheel.CurrentPosition;
						item.Rotation = wheel.CurrentRotation;

						if( wheel.Right )
							item.Rotation = wheel.CurrentRotation * Quaternion.FromRotateByZ( Math.PI );
						else
							item.Rotation = wheel.CurrentRotation;

						item.Scale = Vector3.One;
						item.Color = ColorValue.One;
					}
				}

				AdditionalItems = currentAdditionalItems;
			}
		}

		bool IsMustBeDynamic()
		{
			if( /*needBeActiveBecauseDriverInput || */cannotBeStaticRemainingTime > 0 )
				return true;

			var body = PhysicalBody;
			if( body != null && body.Active && ( body.LinearVelocity != Vector3F.Zero || body.AngularVelocity != Vector3F.Zero ) )
				return true;

			return false;

			//return needBeActiveBecauseDriverInput || needBeActiveBecausePhysicsVelocity || cannotBeStaticRemainingTime > 0;
		}

		protected override void OnUpdateDataFromPhysicalBody( Transform currentTransform, ref Transform newTransform, ref Vector3F linearVelocity, ref Vector3F angularVelocity )
		{
			if( !IsMustBeDynamic() )
			{
				//!!!!0.03

				//if( newTransform.Equals( currentTransform, 0.3 ) )
				if( newTransform.Equals( currentTransform, 0.03 ) )
				{
					newTransform = currentTransform;
					linearVelocity = Vector3F.Zero;
					angularVelocity = Vector3F.Zero;
				}
			}
		}

		public delegate void ProcessDamageBeforeDelegate( Vehicle sender, long whoFired, ref double damage, ref object anyData, ref bool handled );
		public event ProcessDamageBeforeDelegate ProcessDamageBefore;
		public static event ProcessDamageBeforeDelegate ProcessDamageBeforeAll;

		public delegate void ProcessDamageAfterDelegate( Vehicle sender, long whoFired, double damage, object anyData, double oldHealth );
		public event ProcessDamageAfterDelegate ProcessDamageAfter;
		public static event ProcessDamageAfterDelegate ProcessDamageAfterAll;

		public void ProcessDamage( long whoFired, double damage, object anyData )
		{
			var oldHealth = Health.Value;

			var damage2 = damage;
			var anyData2 = anyData;
			var handled = false;
			ProcessDamageBefore?.Invoke( this, whoFired, ref damage2, ref anyData2, ref handled );
			ProcessDamageBeforeAll?.Invoke( this, whoFired, ref damage2, ref anyData2, ref handled );

			if( !handled )
			{
				var health = Health.Value;
				if( health > 0 )
				{
					Health = health - damage;

					if( Health.Value <= 0 )
						RemoveFromParent( true );
				}
			}

			ProcessDamageAfter?.Invoke( this, whoFired, damage2, anyData2, oldHealth );
			ProcessDamageAfterAll?.Invoke( this, whoFired, damage2, anyData2, oldHealth );
		}

		//bool CanBeStatic()
		//{
		//	if( needBeActiveBecauseDriverInput )
		//		return false;


		//	//////the result dependings current state
		//	////if( Static )
		//	////{
		//	////}
		//	////else
		//	////{
		//	////}

		//	//!!!!не переходит ли обратно сразу

		//	//!!!!Jolt активирует если персонаж ходит по граунду
		//	//if( PhysicalBody != null && PhysicalBody.LinearVelocity.LengthSquared() > 0.01f )// != Vector3F.Zero )
		//	//	return false;
		//	if( PhysicalBody != null && PhysicalBody.Active )
		//		return false;

		//	////is any object on seat
		//	//if( dynamicData != null )
		//	//{
		//	//	for( int seatIndex = 0; seatIndex < dynamicData.Seats.Length; seatIndex++ )
		//	//	{
		//	//		if( GetObjectOnSeat( seatIndex ) != null )
		//	//			return false;
		//	//	}
		//	//}

		//	return true;
		//}

		//void SetStatic( bool value )
		//{
		//	////!!!!can switch only to dynamic from static. can't switch to static from dynamic
		//	//if( value )
		//	//	return;

		//	//!!!!может рекурсивно что-то обновляется

		//	//!!!!может клонстрент создавать по событию создания CollisionBody
		//	//!!!!смотреть чтобы где-то не было need update

		//	//Log.Info( "SetStatic: " + value.ToString() );

		//	Static = value;

		//	if( Static )
		//	{
		//		lastLinearVelocity = Vector3.Zero;
		//		lastTransformPosition = null;
		//		//!!!!what else to reset
		//	}
		//}
	}
}
