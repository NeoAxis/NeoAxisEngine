// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// A component to make instance of a vehicle in the scene.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Vehicle\Vehicle", 22003 )]
	public class Vehicle : MeshInSpace, InteractiveObjectInterface, IProcessDamage
	{
		static FastRandom staticRandom = new FastRandom( 0 );

		static double lastInvalidVehicleWarningTime;

		//

		DynamicDataClass dynamicData;
		bool needRecreateDynamicData;

		//to optimize hardware cache, OnSimulationStep doing access to many pointers. call less often code in OnSimulationStep
		float fullyDisabledRemainingTime;

		float mustBeDynamicRemainingTime;

		//bool needBeActiveBecauseDriverInput;

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

		bool duringTransformUpdateWithoutRecreating;

		bool driverInputNeedUpdate = true;

		double currentSteering;

		//float remainingTimeToUpdateObjectsOnSeat;

		float visualHeadlightLow;
		float visualHeadlightHigh;
		float visualBrake;
		float visualLeftTurnSignal;
		float visualRightTurnSignal;
		float visualMoveBack;
		//!!!!add more

		//VehicleInputProcessing inputProcessingCached;
		//!!!!
		//double allowToSleepTime;

		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		//Vector3 linearVelocityForSerialization;

		float motorOnRemainingTime;
		bool? motorOnPrevious;
		Sound currentMotorSound;
		SoundVirtualChannel motorSoundChannel;

		int currentGearSoundPlayed;

		//motor state. sent to clients
		bool sentToClientsMotorOn;
		int sentToClientsCurrentGear;
		float sentToClientsCurrentRPM;

		//tracks
		Curve lastTrackFragmentsCurveLeft;
		Curve lastTrackFragmentsCurveRight;
		double trackFragmentsLinearVelocityLeft;
		double trackFragmentsLinearVelocityRight;
		double trackFragmentsLinearPositionLeft;
		double trackFragmentsLinearPositionRight;

		////!!!!fix internal Jolt issue
		//float skipFirstUpdateFromLibrary = 0.1f;

		double randomTimeAddForTurnSignals;

		/////////////////////////////////////////
		//Basic

		const string vehicleTypeDefault = @"Content\Vehicles\Default\Default Vehicle.vehicletype";

		[DefaultValueReference( vehicleTypeDefault )]
		public Reference<VehicleType> VehicleType
		{
			get { if( _vehicleType.BeginGet() ) VehicleType = _vehicleType.Get( this ); return _vehicleType.value; }
			set { if( _vehicleType.BeginSet( this, ref value ) ) { try { VehicleTypeChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _vehicleType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VehicleType"/> property value changes.</summary>
		public event Action<Vehicle> VehicleTypeChanged;
		ReferenceField<VehicleType> _vehicleType = new Reference<VehicleType>( null, vehicleTypeDefault );

		//!!!!impl modes

		[DefaultValue( PhysicsModeEnum.Basic )]
		public Reference<PhysicsModeEnum> PhysicsMode
		{
			get { if( _physicsMode.BeginGet() ) PhysicsMode = _physicsMode.Get( this ); return _physicsMode.value; }
			set { if( _physicsMode.BeginSet( this, ref value ) ) { try { PhysicsModeChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _physicsMode.EndSet(); } } }
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
			set { if( _debugVisualization.BeginSet( this, ref value ) ) { try { DebugVisualizationChanged?.Invoke( this ); } finally { _debugVisualization.EndSet(); } } }
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

		/// <summary>
		/// The throttle parameter to control the vehicle.
		/// </summary>
		[Category( "Control" )]
		[DefaultValue( 0.0f )]
		[Range( -1, 1 )]
		//[NetworkSynchronize( false )]
		public Reference<float> Throttle
		{
			get { if( _throttle.BeginGet() ) Throttle = _throttle.Get( this ); return _throttle.value; }
			set { if( _throttle.BeginSet( this, ref value ) ) { try { ThrottleChanged?.Invoke( this ); driverInputNeedUpdate = true; } finally { _throttle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Throttle"/> property value changes.</summary>
		public event Action<Vehicle> ThrottleChanged;
		ReferenceField<float> _throttle = 0.0f;

		/// <summary>
		/// The brake parameter to control the vehicle.
		/// </summary>
		[Category( "Control" )]
		[DefaultValue( 0.0f )]
		[Range( 0, 1 )]
		//[NetworkSynchronize( false )]
		public Reference<float> Brake
		{
			get { if( _brake.BeginGet() ) Brake = _brake.Get( this ); return _brake.value; }
			set { if( _brake.BeginSet( this, ref value ) ) { try { BrakeChanged?.Invoke( this ); driverInputNeedUpdate = true; } finally { _brake.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Brake"/> property value changes.</summary>
		public event Action<Vehicle> BrakeChanged;
		ReferenceField<float> _brake = 0.0f;

		/// <summary>
		/// The hand brake parameter to control the vehicle.
		/// </summary>
		[Category( "Control" )]
		[DefaultValue( 1.0f )]
		[Range( 0, 1 )]
		//[NetworkSynchronize( false )]
		public Reference<float> HandBrake
		{
			get { if( _handHandBrake.BeginGet() ) HandBrake = _handHandBrake.Get( this ); return _handHandBrake.value; }
			set { if( _handHandBrake.BeginSet( this, ref value ) ) { try { HandBrakeChanged?.Invoke( this ); driverInputNeedUpdate = true; } finally { _handHandBrake.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HandBrake"/> property value changes.</summary>
		public event Action<Vehicle> HandBrakeChanged;
		ReferenceField<float> _handHandBrake = 1.0f;

		/// <summary>
		/// The steering parameter to control the vehicle.
		/// </summary>
		[Category( "Control" )]
		[DefaultValue( 0.0f )]
		[Range( -1, 1 )]
		[NetworkSynchronize( false )]
		public Reference<float> Steering
		{
			get { if( _steering.BeginGet() ) Steering = _steering.Get( this ); return _steering.value; }
			set { if( _steering.BeginSet( this, ref value ) ) { try { SteeringChanged?.Invoke( this ); driverInputNeedUpdate = true; } finally { _steering.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Steering"/> property value changes.</summary>
		public event Action<Vehicle> SteeringChanged;
		ReferenceField<float> _steering = 0.0f;

		[Category( "Lights" )]
		[DefaultValue( 0.0f )]
		[Range( 0, 1 )]
		public Reference<float> HeadlightsLow
		{
			get { if( _headlightsLow.BeginGet() ) HeadlightsLow = _headlightsLow.Get( this ); return _headlightsLow.value; }
			set { if( _headlightsLow.BeginSet( this, ref value ) ) { try { HeadlightsLowChanged?.Invoke( this ); } finally { _headlightsLow.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeadlightsLow"/> property value changes.</summary>
		public event Action<Vehicle> HeadlightsLowChanged;
		ReferenceField<float> _headlightsLow = 0.0f;

		[Category( "Lights" )]
		[DefaultValue( 0.0f )]
		[Range( 0, 1 )]
		public Reference<float> HeadlightsHigh
		{
			get { if( _headlightsHigh.BeginGet() ) HeadlightsHigh = _headlightsHigh.Get( this ); return _headlightsHigh.value; }
			set { if( _headlightsHigh.BeginSet( this, ref value ) ) { try { HeadlightsHighChanged?.Invoke( this ); } finally { _headlightsHigh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HeadlightsHigh"/> property value changes.</summary>
		public event Action<Vehicle> HeadlightsHighChanged;
		ReferenceField<float> _headlightsHigh = 0.0f;

		[Category( "Lights" )]
		[DefaultValue( 0.0f )]
		[Range( 0, 1 )]
		public Reference<float> LeftTurnSignal
		{
			get { if( _leftTurnSignal.BeginGet() ) LeftTurnSignal = _leftTurnSignal.Get( this ); return _leftTurnSignal.value; }
			set { if( _leftTurnSignal.BeginSet( this, ref value ) ) { try { LeftTurnSignalChanged?.Invoke( this ); } finally { _leftTurnSignal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LeftTurnSignal"/> property value changes.</summary>
		public event Action<Vehicle> LeftTurnSignalChanged;
		ReferenceField<float> _leftTurnSignal = 0.0f;

		[Category( "Lights" )]
		[DefaultValue( 0.0f )]
		public Reference<float> RightTurnSignal
		{
			get { if( _rightTurnSignal.BeginGet() ) RightTurnSignal = _rightTurnSignal.Get( this ); return _rightTurnSignal.value; }
			set { if( _rightTurnSignal.BeginSet( this, ref value ) ) { try { RightTurnSignalChanged?.Invoke( this ); } finally { _rightTurnSignal.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RightTurnSignal"/> property value changes.</summary>
		public event Action<Vehicle> RightTurnSignalChanged;
		ReferenceField<float> _rightTurnSignal = 0.0f;

		//network: no sense to send to clients
		[Browsable( false )]
		public Vector3? RequiredLookToPosition { get; set; }

		/////////////////////////////////////////

		/// <summary>
		/// The health of the vehicle.
		/// </summary>
		[Category( "Game Framework" )]
		[DefaultValue( 0.0f )]
		public Reference<float> Health
		{
			get { if( _health.BeginGet() ) Health = _health.Get( this ); return _health.value; }
			set { if( _health.BeginSet( this, ref value ) ) { try { HealthChanged?.Invoke( this ); driverInputNeedUpdate = true; } finally { _health.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Health"/> property value changes.</summary>
		public event Action<Vehicle> HealthChanged;
		ReferenceField<float> _health = 0.0f;

		/// <summary>
		/// The team index of the object.
		/// </summary>
		[Category( "Game Framework" )]
		[DefaultValue( 0 )]
		public Reference<int> Team
		{
			get { if( _team.BeginGet() ) Team = _team.Get( this ); return _team.value; }
			set { if( _team.BeginSet( this, ref value ) ) { try { TeamChanged?.Invoke( this ); } finally { _team.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Team"/> property value changes.</summary>
		public event Action<Vehicle> TeamChanged;
		ReferenceField<int> _team = 0;

		/////////////////////////////////////////

		[Browsable( false )]
		public Vector3 LinearVelocityToPredictBounds { get; set; }

		/////////////////////////////////////////

		public class DynamicDataClass
		{
			public VehicleType VehicleType;
			public VehicleTypeWheel[] VehicleTypeWheels;
			public int CreatedVersionOfType;
			public PhysicsModeEnum PhysicsMode;
			public Mesh Mesh;
			//public Mesh FrontWheelMesh;
			//public Mesh RearWheelMesh;
			public Bounds LocalBoundsDefault;
			public float LocalBoundsDefaultBoundingRadius;

			public WheelItem[] Wheels;
			public SeatItemData[] Seats;
			public Scene.PhysicsWorldClass.VehicleConstraint constraint;

			public TurretItem[] Turrets;
			public LightItem[] Lights;
			public bool LightsExistHeadlightLow;
			public bool LightsExistHeadlightHigh;
			//!!!!
			//public AeroEngineItem[] AeroEngines;

			public int CurrentGear;
			public bool IsSwitchingGear;
			public float CurrentRPM;

			public Mesh TrackFragmentMesh;
			//public TrackItem[] Tracks;

			/////////////////////

			public enum WhichWheel
			{
				//Wheels mode
				FrontLeft,
				FrontRight,
				RearLeft,
				RearRight,

				//Tracks mode
				TrackLeft,
				TrackRight,

				//Custom1, Custom2, etc
			}

			/////////////////////

			public struct WheelItem
			{
				//static data
				public WhichWheel Which;
				public int WhichIndex;

				//public bool WheelDrive;
				public float Diameter;
				public float Width;
				public Vector3F InitialPosition;
				public Mesh Mesh;

				//dynamic data
				public Vector3F CurrentPosition;
				////!!!!fix internal Jolt issue
				//public float[] LastPositions;

				public QuaternionF CurrentRotation;
				public float AngularVelocity;

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

			public class SeatItemData
			{
				public SeatItem SourceComponent;
				public bool Mirrored;

				//!!!!need make copy? in type can be with reference
				public Transform Transform;
				public Degree SpineAngle;
				public Degree LegsAngle;

				//public Vector3 EyeOffset;
				public Transform ExitTransform;
			}

			/////////////////////

			public class TurretItem
			{
				public Transform InitialTransform;
				//public Vector3F InitialPosition;
				public Turret Turret;

				//can add other types of constructions (side turrets)

				public Constraint_SixDOF HorizontalConstraint;
				public Weapon Weapon;
				public Constraint_SixDOF VerticalConstraint;

				public SoundVirtualChannel TurretTurnSoundChannel;
			}

			/////////////////////

			public enum LightTypePurpose
			{
				Unknown,

				Headlight_Low,
				Headlight_High,
				Brake,
				Left_Turn,
				Right_Turn,
				Position,//position lights will enabled when headlight low or high enabled
				Move_Back,

				//Front_Fog,
				//Daytime_Running
			}

			/////////////////////

			public class LightItem
			{
				public Light LightType;
				public bool Mirrored;
				public LightTypePurpose LightTypePurpose;

				public Light Light;
			}

			/////////////////////

			//!!!!
			//public class AeroEngineItem
			//{
			//	public VehicleAeroEngine EngineType;

			//	public VehicleAeroEngine Engine;
			//}

			/////////////////////

			//public struct TrackFragmentItem
			//{
			//	public Vector3F Position;
			//	public QuaternionF Rotation;
			//}

			/////////////////////

			//public class TrackItem
			//{
			//	public TrackFragmentItem[] Fragments;
			//}
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

				//not works
				//Static = !IsMustBeDynamic();


				var updated = false;
				SimulateVisualHeadlights( ref updated, true );
				SimulateVisualBrake( ref updated, true );
				SimulateVisualTurnSignals( ref updated, true );
				SimulateVisualMoveBack( ref updated, true );
				UpdateLightComponents();
			}
			else
			{
				if( motorSoundChannel != null )
				{
					motorSoundChannel.Stop();
					motorSoundChannel = null;
				}

				DestroyDynamicData();
			}
		}

		public void SetTransform( Transform value, bool recreate )
		{
			if( !recreate )
				duringTransformUpdateWithoutRecreating = true;
			Transform = value;
			if( !recreate )
				duringTransformUpdateWithoutRecreating = false;
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
				var type = dynamicData.VehicleType;
				var tracks = type != null && type.Chassis.Value == NeoAxis.VehicleType.ChassisEnum.Tracks;
				var wheels = !tracks;

				if( wheels )
					dynamicData.constraint.SetDriverInput( (float)Throttle, 0, (float)currentSteering, (float)Brake, (float)HandBrake, activateBody );

				if( tracks )
				{
					var forward = MathEx.Clamp( Math.Abs( Throttle ) + Math.Abs( currentSteering ), 0, 1 );
					if( Throttle < 0 )
						forward = -forward;

					var left = MathEx.Clamp( 1.0f + currentSteering, 0.01f, 1.0f );
					var right = MathEx.Clamp( 1.0f - currentSteering, 0.01f, 1.0f );

					var brake = MathEx.Saturate( Brake.Value + HandBrake.Value );

					// Set input from driver
					// @param inForward Value between -1 and 1 for auto transmission and value between 0 and 1 indicating desired driving direction and amount the gas pedal is pressed
					// @param inLeftRatio Value between -1 and 1 indicating an extra multiplier to the rotation rate of the left track (used for steering)
					// @param inRightRatio Value between -1 and 1 indicating an extra multiplier to the rotation rate of the right track (used for steering)
					// @param inBrake Value between 0 and 1 indicating how strong the brake pedal is pressed
					dynamicData.constraint.SetDriverInput( (float)forward, (float)left, (float)right, (float)brake, 0, activateBody );
				}

				mustBeDynamicRemainingTime = initialization ? 0 : 3.0f;
				//needBeActiveBecauseDriverInput = true;
			}
			//else
			//	needBeActiveBecauseDriverInput = false;

			driverInputNeedUpdate = false;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void UpdateObjectsOnSeats( ref bool updated )
		{
			if( dynamicData != null )
			{
				//when update not each time?

				//remainingTimeToUpdateObjectsOnSeat -= delta;
				//if( remainingTimeToUpdateObjectsOnSeat <= 0 )
				//{
				//remainingTimeToUpdateObjectsOnSeat = 0.25f + boundsPredictionAndUpdateRandom.Next( 0.05f );

				for( int seatIndex = 0; seatIndex < dynamicData.Seats.Length; seatIndex++ )
					UpdateObjectOnSeat( seatIndex, ref updated );

				//}
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( fullyDisabledRemainingTime > 0 )
			{
				fullyDisabledRemainingTime -= Time.SimulationDelta;
				if( fullyDisabledRemainingTime < 0 )
					fullyDisabledRemainingTime = 0;
			}
			else
			{
				var updated = false;

				var constraint = dynamicData?.constraint;
				if( constraint != null )
				{
					//update mustBeDynamicRemainingTime
					if( mustBeDynamicRemainingTime > 0 )
					{
						mustBeDynamicRemainingTime -= Time.SimulationDelta;
						if( mustBeDynamicRemainingTime < 0 )
							mustBeDynamicRemainingTime = 0;
					}

					var mustBeDynamic = IsMustBeDynamic();
					if( mustBeDynamic )
						updated = true;

					//update driver input
					if( mustBeDynamic || driverInputNeedUpdate )
					{
						updated = true;

						var lastSteering = currentSteering;

						var steering = Steering.Value;
						if( currentSteering != steering )
						{
							double steeringTime;
							if( dynamicData.VehicleTypeWheels[ 0 ].MaxSteeringAngle.Value > 0 )
								steeringTime = dynamicData.VehicleTypeWheels[ 0 ].SteeringTime;
							else
								steeringTime = dynamicData.VehicleTypeWheels[ 1 ].SteeringTime;
							if( steeringTime == 0 )
								steeringTime = 0.000001;

							if( currentSteering > steering )
							{
								currentSteering -= Time.SimulationDelta / steeringTime;
								//currentSteering -= Time.SimulationDelta / dynamicData.VehicleType.FrontWheelSteeringTime;
								if( currentSteering < steering )
									currentSteering = steering;
								if( currentSteering < -1 )
									currentSteering = -1;
							}
							else
							{
								currentSteering += Time.SimulationDelta / steeringTime;
								//currentSteering += Time.SimulationDelta / dynamicData.VehicleType.FrontWheelSteeringTime;
								if( currentSteering > steering )
									currentSteering = steering;
								if( currentSteering > 1 )
									currentSteering = 1;
							}
						}

						SetDriverInput( lastSteering, false );
					}

					//constraint.SetStepListenerMustBeAdded( mustBeDynamic );

					if( mustBeDynamic )
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

					var needStatic = !mustBeDynamic;

					if( needStatic != Static.Value )
					{
						updated = true;

						//can't be static some time after change from static to dynamic
						if( mustBeDynamic )
							mustBeDynamicRemainingTime = 0.5f;// 3.0;

						//if( needStatic )
						//{
						//	lastTransformToCalculateDynamicState = TransformV;
						//	needBeActiveBecauseTransformChange = false;
						//}
						//else
						//	cannotBeStaticRemainingTime = 1.0;

						Static = needStatic;
						StaticShadows = false;
					}

					//update additional items
					if( mustBeDynamic && PhysicalBody != null && PhysicalBody.Active && dynamicData.Wheels != null )//&& skipFirstUpdateFromLibrary == 0 )
					{
						unsafe
						{
							var wheelsData = stackalloc Scene.PhysicsWorldClass.VehicleWheelData[ dynamicData.Wheels.Length ];
							dynamicData.constraint.GetData( wheelsData, out var active, out dynamicData.CurrentGear, out dynamicData.IsSwitchingGear, out dynamicData.CurrentRPM );

							for( int nWheel = 0; nWheel < dynamicData.Wheels.Length; nWheel++ )
							{
								ref var wheel = ref dynamicData.Wheels[ nWheel ];
								var wheelData = wheelsData + nWheel;

								//fix "max suspension" issue
								if( PhysicalBody.ActiveUpdateCount > 2 )
								{
									wheel.CurrentPosition = wheel.InitialPosition;

									////!!!!fix internal Jolt issue
									//if( wheel.LastPositions == null )
									//{
									//	wheel.LastPositions = new float[ 5 ];
									//	for( int n = 0; n < wheel.LastPositions.Length; n++ )
									//		wheel.LastPositions[ n ] = wheelData->Position;
									//}
									//for( int n = 0; n < wheel.LastPositions.Length - 1; n++ )
									//	wheel.LastPositions[ n ] = wheel.LastPositions[ n + 1 ];
									//wheel.LastPositions[ wheel.LastPositions.Length - 1 ] = wheelData->Position;

									//wheel.CurrentPosition.Z = wheel.LastPositions[ 0 ];
									//for( int n = 1; n < wheel.LastPositions.Length; n++ )
									//{
									//	if( wheel.CurrentPosition.Z > wheel.LastPositions[ n ] )
									//		wheel.CurrentPosition.Z = wheel.LastPositions[ n ];
									//}

									//for without issue:
									wheel.CurrentPosition.Z = wheelData->Position;// = wheelData->SuspensionLength - wheel.Diameter * 0.5f;
								}

								wheel.CurrentRotation = QuaternionF.FromRotateByZ( -wheelData->SteerAngle ) * QuaternionF.FromRotateByY( -wheelData->RotationAngle );

								//!!!!use angular veclocity for motion blur?
								wheel.AngularVelocity = wheelData->AngularVelocity;

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

						//networking: send update to clients
						if( NetworkIsServer )
						{
							var m = BeginNetworkMessageToEveryone( "UpdateWheels" );
							var writer = m.Writer;

							//!!!!реже высылать, не высылать тоже самое, дискретизация, интерполировать

							writer.Write( (byte)dynamicData.Wheels.Length );
							for( int n = 0; n < dynamicData.Wheels.Length; n++ )
							{
								ref var wheel = ref dynamicData.Wheels[ n ];

								writer.Write( new HalfType( wheel.CurrentPosition.Z ) );
								writer.Write( wheel.CurrentRotation.ToQuaternionH() );
								//for tracks
								writer.Write( wheel.AngularVelocity );

								//!!!!send contact body?
							}

							m.End();
						}

						UpdateAdditionalItems();
					}

					//if( mustBeDynamic && skipFirstUpdateFromLibrary > 0 )
					//{
					//	skipFirstUpdateFromLibrary -= Time.SimulationDelta;
					//	if( skipFirstUpdateFromLibrary < 0 )
					//		skipFirstUpdateFromLibrary = 0;
					//}
				}

				UpdateObjectsOnSeats( ref updated );
				UpdateTurrets( ref updated );
				SimulateMotorOn( ref updated );
				SimulateMotorSound( ref updated );
				SimulateCurrentGearSound( ref updated );

				if( NetworkIsSingle )
				{
					var lightsUpdated = false;
					SimulateVisualHeadlights( ref lightsUpdated );
					SimulateVisualBrake( ref lightsUpdated );
					SimulateVisualTurnSignals( ref lightsUpdated );
					SimulateVisualMoveBack( ref lightsUpdated );
					if( lightsUpdated )
					{
						updated = true;
						UpdateLightComponents();
					}
				}

				SimulateAeroEngines();

				SimulateTrackFragmentsLinearPositions();

				//!!!!merge with UpdateWheels?
				if( NetworkIsServer && dynamicData != null )
				{
					if( MotorOn != sentToClientsMotorOn || dynamicData.CurrentGear != sentToClientsCurrentGear || dynamicData.CurrentRPM != sentToClientsCurrentRPM )
					{
						sentToClientsMotorOn = MotorOn;
						sentToClientsCurrentGear = dynamicData.CurrentGear;
						sentToClientsCurrentRPM = dynamicData.CurrentRPM;

						var m = BeginNetworkMessageToEveryone( "MotorState" );
						var writer = m.Writer;
						writer.Write( MotorOn );
						writer.WriteVariableInt32( dynamicData.CurrentGear );
						writer.Write( dynamicData.CurrentRPM );
						m.End();
					}
				}

				if( !updated )
					fullyDisabledRemainingTime = 2.0f + staticRandom.Next( 0.1f );
			}
		}

		protected override void OnSimulationStepClient()
		{
			base.OnSimulationStepClient();

			var updated = false;

			UpdateObjectsOnSeats( ref updated );
			UpdateTurrets( ref updated );
			SimulateMotorSound( ref updated );
			SimulateCurrentGearSound( ref updated );

			var lightsUpdated = false;
			SimulateVisualHeadlights( ref lightsUpdated );
			SimulateVisualBrake( ref lightsUpdated );
			SimulateVisualTurnSignals( ref lightsUpdated );
			SimulateVisualMoveBack( ref lightsUpdated );
			if( lightsUpdated )
			{
				updated = true;
				UpdateLightComponents();
			}

			SimulateTrackFragmentsLinearPositions();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void UpdateObjectOnSeat( int seatIndex, ref bool updated )
		{
			var objectOnSeat = GetObjectOnSeat( seatIndex );
			if( objectOnSeat != null )
			{
				updated = true;

				var seatItem = dynamicData.Seats[ seatIndex ];

				objectOnSeat.Visible = seatItem.SourceComponent.DisplayObject && Visible;

				var character = objectOnSeat as Character;
				if( character != null )
				{
					character.Collision = false;
					character.Sitting = true;
					character.SittingSpineAngle = seatItem.SpineAngle;
					character.SittingLegsAngle = seatItem.LegsAngle;

					if( NetworkIsSingleOrClient )
					{
						var seatTransform = seatItem.Transform;
						seatTransform = seatTransform.UpdatePosition( seatTransform.Position + new Vector3( 0, 0, -character.TypeCached.SitButtHeight ) );
						character.SetTransformAndTurnToDirectionInstantly( TransformV * seatTransform );
					}
				}
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( needRecreateDynamicData )
				CreateDynamicData();

			if( EngineApp.IsEditor )
			{
				var lightsUpdated = false;
				SimulateVisualHeadlights( ref lightsUpdated, true );
				SimulateVisualBrake( ref lightsUpdated, true );
				SimulateVisualTurnSignals( ref lightsUpdated, true );
				SimulateVisualMoveBack( ref lightsUpdated, true );
				if( lightsUpdated )
					UpdateLightComponents();

				//update when Version of type is updated
				if( dynamicData != null && dynamicData.CreatedVersionOfType != dynamicData.VehicleType.Version )
					needRecreateDynamicData = true;
			}
		}

		protected override void OnTransformChanged()
		{
			//UpdateSpaceBoundsOverride();

			base.OnTransformChanged();

			if( EngineApp.IsEditor && !duringTransformUpdateWithoutRecreating )
				NeedRecreateDynamicData();
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			//base.OnSpaceBoundsUpdate( ref newBounds );

			GetBox( out var box );
			box.ToBounds( out var realBounds );

			if( IsMustBeDynamic() )
			{
				if( dynamicData != null )
				{
					//bounds prediction to skip small updates in future steps

					////calculate actual bounds
					//var radius = dynamicData.LocalBoundsDefaultBoundingRadius;
					//var tr = TransformV;
					//var realBounds = new Bounds(
					//	tr.Position.X - radius, tr.Position.Y - radius, tr.Position.Z - radius,
					//	tr.Position.X + radius, tr.Position.Y + radius, tr.Position.Z + radius );
					newBounds = new SpaceBounds( realBounds );

					//check for update extended bounds
					if( !SpaceBoundsOctreeOverride.HasValue || !SpaceBoundsOctreeOverride.Value.Contains( ref realBounds ) )
					{
						//calculate extended bounds

						var radius = dynamicData.LocalBoundsDefaultBoundingRadius;
						var tr = TransformV;

						//update each 2-3 seconds
						var extendForSeconds = 2.0f + staticRandom.Next( 0.0f, 1.0f );
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
			else
			{
				newBounds = new SpaceBounds( realBounds );
				SpaceBoundsOctreeOverride = null;
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

		[MethodImpl( (MethodImplOptions)512 )]
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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

		[MethodImpl( (MethodImplOptions)512 )]
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

		[MethodImpl( (MethodImplOptions)512 )]
		public Box GetBox()
		{
			GetBox( out var box );
			return box;
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

		void DebugRenderSeat( ViewportRenderingContext context, DynamicDataClass.SeatItemData seatItem )
		{
			var renderer = context.Owner.Simple3DRenderer;
			var vehicleTransform = TransformV;

			//seat
			{
				var color = new ColorValue( 0, 1, 0 );
				renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
				var tr = vehicleTransform * seatItem.Transform;
				var p = tr * new Vector3( 0, 0, 0 );
				renderer.AddSphere( new Sphere( p, 0.02 ), 16 );
				renderer.AddArrow( p, tr * new Vector3( 1, 0, 0 ) );
			}

			//exit
			{
				var color = new ColorValue( 1, 0, 0 );
				renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
				var tr = vehicleTransform * seatItem.ExitTransform;
				var p = tr * new Vector3( 0, 0, 0 );
				renderer.AddSphere( new Sphere( p, 0.02 ), 16 );
				renderer.AddArrow( p, tr * new Vector3( 1, 0, 0 ) );
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
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
					var renderer = context.Owner.Simple3DRenderer;

					if( scene != null && context.SceneDisplayDevelopmentDataInThisApplication && dynamicData != null && renderer != null )//!!!! scene.DisplayPhysicalObjects )
					{
						var tr = TransformV;

						//wheels
						if( dynamicData.Wheels != null && ( !EngineApp.IsEditor || dynamicData.VehicleType.EditorDisplayWheels ) )
						{
							//foreach( var wheel in dynamicData.Wheels )
							for( int nWheel = 0; nWheel < dynamicData.Wheels.Length; nWheel++ )
							{
								ref var wheel = ref dynamicData.Wheels[ nWheel ];

								{
									var color = new ColorValue( 1, 0, 0 );
									renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

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
						if( dynamicData.Seats != null && ( !EngineApp.IsEditor || dynamicData.VehicleType.EditorDisplaySeats ) )
						{
							for( int n = 0; n < dynamicData.Seats.Length; n++ )
							{
								var seatItem = dynamicData.Seats[ n ];
								DebugRenderSeat( context, seatItem );
							}
						}

						//lights
						if( dynamicData.Lights != null && ( !EngineApp.IsEditor || dynamicData.VehicleType.EditorDisplayLights ) )
						{
							var color = ProjectSettings.Get.Colors.SceneShowLightColor.Value;
							color.Alpha *= 0.25f;
							renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							foreach( var lightItem in dynamicData.Lights )
								lightItem.Light?.DebugDraw( context.Owner );
						}

						//var tr = GetTransform();
						//var scaleFactor = tr.Scale.MaxComponent();

						//renderer.SetColor( new ColorValue( 1, 0, 0, 1 ) );

						////eye position
						//renderer.SetColor( new ColorValue( 0, 1, 0, 1 ) );
						//renderer.AddSphere( new Sphere( TransformV * EyePosition.Value, .05f ), 16 );
					}

					if( RequiredLookToPosition.HasValue )
					{
						renderer.SetColor( new ColorValue( 1, 0, 0 ) );
						renderer.AddSphere( new Sphere( RequiredLookToPosition.Value, .05 ) );
					}

					//track fragments curve
					if( dynamicData != null && dynamicData.VehicleType.Chassis.Value == NeoAxis.VehicleType.ChassisEnum.Tracks && ( !EngineApp.IsEditor || dynamicData.VehicleType.EditorDisplayWheels ) ) // Tracks != null )
					{
						for( int n = 0; n < 2; n++ )
						{
							var curve = n == 0 ? lastTrackFragmentsCurveLeft : lastTrackFragmentsCurveRight;
							if( curve != null )
							{
								var step = 0.2;

								renderer.SetColor( new ColorValue( 1, 0, 0 ) );
								var previousValue = Vector3.Zero;
								for( var t = 0.0; t < curve.Points[ curve.Points.Count - 1 ].Time; t += step )
								{
									var v = curve.CalculateValueByTime( t );
									if( t != 0 )
										renderer.AddLine( TransformV * previousValue, TransformV * v );
									previousValue = v;
								}
							}
						}
					}
				}

				//!!!!
				var showLabels = /*show &&*/ dynamicData == null;
				if( !showLabels )
					context2.disableShowingLabelForThisObject = true;

#if !DEPLOY
				//draw selection, touch Mesh of type
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

					//touch Mesh of type
					if( dynamicData != null )
					{
						var type = dynamicData?.VehicleType;
						if( type != null )
						{
							dynamicData.VehicleType.Mesh.Touch();
							foreach( var wheel in dynamicData.VehicleTypeWheels )
								wheel.Mesh.Touch();
							//if( dynamicData.Tracks != null )
							dynamicData.VehicleType.TrackFragmentMesh.Touch();
						}
					}
					//var type = dynamicData?.VehicleType;
					//if( type != null )
					//{
					//	type.Mesh.Touch();
					//	type.FrontWheelMesh.Touch();
					//	type.RearWheelMesh.Touch();
					//}
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

			if( useEyesPositionOfModel )
			{

				//!!!!select seat

				//!!!!turn head support


				if( dynamicData != null && dynamicData.Seats.Length != 0 )
				{
					var seatIndex = 0;

					var objectOnSeat = GetObjectOnSeat( seatIndex );
					if( objectOnSeat != null )
					{
						//var seatItem = dynamicData.Seats[ seatIndex ];

						var character = objectOnSeat as Character;
						if( character != null )
						{
							character.GetFirstPersonCameraPosition( true, out position, out forward, out up );

							//var seatTransform = seatItem.Transform;
							//position = vehicleTransform * ( seatTransform * seatItem.EyeOffset );
						}
					}
				}
			}
		}

		//public Vector3 GetSmoothPosition()
		//{
		//	return TransformV.Position + GetSmoothCameraOffset();
		//}

		public delegate void SoundPlayBeforeDelegate( Vehicle sender, ref Sound sound, ref bool handled );
		public event SoundPlayBeforeDelegate SoundPlayBefore;

		public virtual void SoundPlay( Sound sound )
		{
			var handled = false;
			SoundPlayBefore?.Invoke( this, ref sound, ref handled );
			if( handled )
				return;

			ParentScene?.SoundPlay( sound, TransformV.Position );
		}

		public void NeedRecreateDynamicData()
		{
			needRecreateDynamicData = true;
		}

		static void CalculateWheelWhichIndexByIndex( DynamicDataClass dynamicData /*VehicleType type, VehicleTypeWheel[] typeTrackWheels*/, int index, out DynamicDataClass.WhichWheel which, out int whichIndex )
		{
			var type = dynamicData.VehicleType;

			if( type.Chassis.Value == NeoAxis.VehicleType.ChassisEnum.Wheels )
			{
				var typeFrontWheel = dynamicData.VehicleTypeWheels[ 0 ];
				var typeRearWheel = dynamicData.VehicleTypeWheels[ 1 ];

				int currentIndex = 0;

				for( int n = 0; n < typeFrontWheel.Count; n++ )
				{
					if( index == currentIndex )
					{
						which = DynamicDataClass.WhichWheel.FrontLeft;
						whichIndex = n;
						return;
					}
					currentIndex++;
				}

				if( typeFrontWheel.Pair )
				{
					for( int n = 0; n < typeFrontWheel.Count; n++ )
					{
						if( index == currentIndex )
						{
							which = DynamicDataClass.WhichWheel.FrontRight;
							whichIndex = n;
							return;
						}
						currentIndex++;
					}
				}

				for( int n = 0; n < typeRearWheel.Count; n++ )
				{
					if( index == currentIndex )
					{
						which = DynamicDataClass.WhichWheel.RearLeft;
						whichIndex = n;
						return;
					}
					currentIndex++;
				}

				if( typeRearWheel.Pair )
				{
					for( int n = 0; n < typeRearWheel.Count; n++ )
					{
						if( index == currentIndex )
						{
							which = DynamicDataClass.WhichWheel.RearRight;
							whichIndex = n;
							return;
						}
						currentIndex++;
					}
				}
			}
			else if( type.Chassis.Value == NeoAxis.VehicleType.ChassisEnum.Tracks )
			{
				var typeWheels = dynamicData.VehicleTypeWheels;

				int currentIndex = 0;

				for( int n = 0; n < typeWheels.Length; n++ )
				{
					if( index == currentIndex )
					{
						which = DynamicDataClass.WhichWheel.TrackLeft;
						whichIndex = n;
						return;
					}
					currentIndex++;
				}

				for( int n = 0; n < typeWheels.Length; n++ )
				{
					if( index == currentIndex )
					{
						which = DynamicDataClass.WhichWheel.TrackRight;
						whichIndex = n;
						return;
					}
					currentIndex++;
				}

				//var leftIndex = 0;
				//for( int n = 0; n < typeWheels.Length; n++ )
				//{
				//	for( int n2 = 0; n2 < typeWheels[ n ].Count; n2++ )
				//	{
				//		if( index == currentIndex )
				//		{
				//			which = DynamicDataClass.WhichWheel.TrackLeft;
				//			whichIndex = leftIndex;// n;
				//			return;
				//		}
				//		currentIndex++;
				//		leftIndex++;
				//	}
				//}

				//var rightIndex = 0;
				//for( int n = 0; n < typeWheels.Length; n++ )
				//{
				//	for( int n2 = 0; n2 < typeWheels[ n ].Count; n2++ )
				//	{
				//		if( index == currentIndex )
				//		{
				//			which = DynamicDataClass.WhichWheel.TrackRight;
				//			whichIndex = rightIndex;// n;
				//			return;
				//		}
				//		currentIndex++;
				//		rightIndex++;
				//	}
				//}
			}

			which = 0;
			whichIndex = 0;
		}

		static int CalculateWheelIndex( DynamicDataClass dynamicData /*VehicleType type, VehicleTypeWheel[] typeTrackWheels*/, DynamicDataClass.WhichWheel which, int whichIndex )
		{
			var type = dynamicData.VehicleType;

			if( type.Chassis.Value == NeoAxis.VehicleType.ChassisEnum.Wheels )
			{
				var typeFrontWheel = dynamicData.VehicleTypeWheels[ 0 ];
				var typeRearWheel = dynamicData.VehicleTypeWheels[ 1 ];

				int currentIndex = 0;

				for( int n = 0; n < typeFrontWheel.Count; n++ )
				{
					if( which == DynamicDataClass.WhichWheel.FrontLeft && whichIndex == n )
						return currentIndex;
					currentIndex++;
				}

				if( typeFrontWheel.Pair )
				{
					for( int n = 0; n < typeFrontWheel.Count; n++ )
					{
						if( which == DynamicDataClass.WhichWheel.FrontRight && whichIndex == n )
							return currentIndex;
						currentIndex++;
					}
				}

				for( int n = 0; n < typeRearWheel.Count; n++ )
				{
					if( which == DynamicDataClass.WhichWheel.RearLeft && whichIndex == n )
						return currentIndex;
					currentIndex++;
				}

				if( typeRearWheel.Pair )
				{
					for( int n = 0; n < typeRearWheel.Count; n++ )
					{
						if( which == DynamicDataClass.WhichWheel.RearRight && whichIndex == n )
							return currentIndex;
						currentIndex++;
					}
				}
			}
			else if( type.Chassis.Value == NeoAxis.VehicleType.ChassisEnum.Tracks )
			{
				var typeWheels = dynamicData.VehicleTypeWheels;

				int currentIndex = 0;

				for( int n = 0; n < typeWheels.Length; n++ )
				{
					if( which == DynamicDataClass.WhichWheel.TrackLeft && whichIndex == n )
						return currentIndex;
					currentIndex++;
				}

				for( int n = 0; n < typeWheels.Length; n++ )
				{
					if( which == DynamicDataClass.WhichWheel.TrackRight && whichIndex == n )
						return currentIndex;
					currentIndex++;
				}
			}

			return 0;
		}

		public void CreateDynamicData()
		{
			DestroyDynamicData();

			if( !EnabledInHierarchyAndIsInstance )
				return;
			//if( NetworkIsClient )
			//	return;

			//const bool displayInEditor = false;// true;

			var scene = ParentScene;
			if( scene == null )
				return;

			var type = VehicleType.Value;
			if( type == null )
				return;

			//check vehicle type
			if( !type.IsValid( out var error ) )
			{
				if( Time.Current - lastInvalidVehicleWarningTime > 5 )
				{
					lastInvalidVehicleWarningTime = Time.Current;
					Log.Warning( "Invalid vehicle type. " + error );
				}
				return;
			}

			var wheels = type.Chassis.Value == NeoAxis.VehicleType.ChassisEnum.Wheels;
			var tracks = type.Chassis.Value == NeoAxis.VehicleType.ChassisEnum.Tracks;

			//!!!!GC
			var typeWheels = type.GetComponents<VehicleTypeWheel>().Where( w => w.Enabled ).ToArray();

			//for Wheels mode must be two VehicleTypeWheel components
			if( wheels && typeWheels.Length != 2 )
				return;
			if( tracks && typeWheels.Length < 2 )
				return;

			dynamicData = new DynamicDataClass();
			dynamicData.VehicleType = type;
			dynamicData.VehicleTypeWheels = typeWheels;
			dynamicData.CreatedVersionOfType = type.Version;
			dynamicData.PhysicsMode = PhysicsMode;
			dynamicData.Mesh = type.Mesh;
			//dynamicData.FrontWheelMesh = type.FrontWheelMesh;
			//dynamicData.RearWheelMesh = type.RearWheelMesh;

			VehicleTypeWheel typeFrontWheel = null;
			VehicleTypeWheel typeRearWheel = null;
			if( wheels )
			{
				typeFrontWheel = dynamicData.VehicleTypeWheels[ 0 ];
				typeRearWheel = dynamicData.VehicleTypeWheels[ 1 ];
			}

			var tr = TransformV;

			Mesh = dynamicData.Mesh;//type.Mesh;

			//!!!!Kinematic
			if( dynamicData.PhysicsMode == PhysicsModeEnum.Basic )
				Collision = true;

			//create wheels for Wheels mode
			if( wheels )
			{
				var totalCount = typeFrontWheel.Count.Value * ( typeFrontWheel.Pair ? 2 : 1 ) + typeRearWheel.Count.Value * ( typeRearWheel.Pair ? 2 : 1 );
				dynamicData.Wheels = new DynamicDataClass.WheelItem[ totalCount ];

				for( int n = 0; n < dynamicData.Wheels.Length; n++ )
				{
					ref var wheel = ref dynamicData.Wheels[ n ];

					CalculateWheelWhichIndexByIndex( dynamicData, n, out wheel.Which, out wheel.WhichIndex );

					switch( wheel.Which )
					{
					case DynamicDataClass.WhichWheel.FrontLeft:
						wheel.InitialPosition = ( typeFrontWheel.Position.Value + new Vector3( -typeFrontWheel.Distance.Value, 0, 0 ) * wheel.WhichIndex ).ToVector3F();
						if( wheel.InitialPosition.Y < 0 )
							wheel.InitialPosition.Y = -wheel.InitialPosition.Y;
						break;

					case DynamicDataClass.WhichWheel.FrontRight:
						wheel.InitialPosition = ( typeFrontWheel.Position.Value + new Vector3( -typeFrontWheel.Distance.Value, 0, 0 ) * wheel.WhichIndex ).ToVector3F();
						if( wheel.InitialPosition.Y > 0 )
							wheel.InitialPosition.Y = -wheel.InitialPosition.Y;
						break;

					case DynamicDataClass.WhichWheel.RearLeft:
						wheel.InitialPosition = ( typeRearWheel.Position.Value + new Vector3( -typeRearWheel.Distance.Value, 0, 0 ) * wheel.WhichIndex ).ToVector3F();
						if( wheel.InitialPosition.Y < 0 )
							wheel.InitialPosition.Y = -wheel.InitialPosition.Y;
						break;

					case DynamicDataClass.WhichWheel.RearRight:
						wheel.InitialPosition = ( typeRearWheel.Position.Value + new Vector3( -typeRearWheel.Distance.Value, 0, 0 ) * wheel.WhichIndex ).ToVector3F();
						if( wheel.InitialPosition.Y > 0 )
							wheel.InitialPosition.Y = -wheel.InitialPosition.Y;
						break;
					}

					wheel.CurrentPosition = wheel.InitialPosition;
					wheel.CurrentRotation = QuaternionF.Identity;

					switch( wheel.Which )
					{
					case DynamicDataClass.WhichWheel.FrontLeft:
					case DynamicDataClass.WhichWheel.FrontRight:
						wheel.Diameter = (float)typeFrontWheel.Diameter.Value;
						wheel.Width = (float)typeFrontWheel.Width.Value;
						wheel.Mesh = typeFrontWheel.Mesh;
						break;

					case DynamicDataClass.WhichWheel.RearLeft:
					case DynamicDataClass.WhichWheel.RearRight:
						wheel.Diameter = (float)typeRearWheel.Diameter.Value;
						wheel.Width = (float)typeRearWheel.Width.Value;
						wheel.Mesh = typeRearWheel.Mesh;
						break;
					}
				}
			}

			//create wheels for Tracks mode
			if( tracks )
			{
				dynamicData.Wheels = new DynamicDataClass.WheelItem[ typeWheels.Length * 2 ];

				//Count is not supported for tracks. with Count it harder to understand WhichIndex
				//for( int n = 0; n < typeWheels.Length; n++ )
				//	totalCount += typeWheels[ n ].Count.Value * 2;
				//dynamicData.Wheels = new DynamicDataClass.WheelItem[ totalCount ]; //typeWheels.Length * 2 ];

				for( int n = 0; n < dynamicData.Wheels.Length; n++ )
				{
					ref var wheel = ref dynamicData.Wheels[ n ];

					CalculateWheelWhichIndexByIndex( dynamicData, n, out wheel.Which, out wheel.WhichIndex );
					var trackWheel = typeWheels[ wheel.WhichIndex ];

					switch( wheel.Which )
					{
					case DynamicDataClass.WhichWheel.TrackLeft:
						wheel.InitialPosition = trackWheel.Position.Value.ToVector3F();
						if( wheel.InitialPosition.Y < 0 )
							wheel.InitialPosition.Y = -wheel.InitialPosition.Y;
						break;

					case DynamicDataClass.WhichWheel.TrackRight:
						wheel.InitialPosition = trackWheel.Position.Value.ToVector3F();
						if( wheel.InitialPosition.Y > 0 )
							wheel.InitialPosition.Y = -wheel.InitialPosition.Y;
						break;
					}

					//switch( wheel.Which )
					//{
					//case DynamicDataClass.WhichWheel.TrackLeft:
					//	wheel.InitialPosition = ( trackWheel.Position.Value + new Vector3( -trackWheel.Distance.Value, 0, 0 ) * wheel.WhichIndex ).ToVector3F();
					//	if( wheel.InitialPosition.Y < 0 )
					//		wheel.InitialPosition.Y = -wheel.InitialPosition.Y;
					//	break;

					//case DynamicDataClass.WhichWheel.TrackRight:
					//	wheel.InitialPosition = ( trackWheel.Position.Value + new Vector3( -trackWheel.Distance.Value, 0, 0 ) * wheel.WhichIndex ).ToVector3F();
					//	if( wheel.InitialPosition.Y > 0 )
					//		wheel.InitialPosition.Y = -wheel.InitialPosition.Y;
					//	break;
					//}

					wheel.CurrentPosition = wheel.InitialPosition;
					wheel.CurrentRotation = QuaternionF.Identity;

					wheel.Diameter = (float)trackWheel.Diameter;
					wheel.Width = (float)trackWheel.Width;
					wheel.Mesh = trackWheel.Mesh;
				}
			}

			//create constraint
			if( dynamicData.PhysicsMode == PhysicsModeEnum.Basic && ( wheels || tracks ) && PhysicalBody != null && !PhysicalBody.Disposed && PhysicalBody.MotionType == PhysicsMotionType.Dynamic )
			{
				//!!!!center offset?

				var physicsWorldData = PhysicalBody.PhysicsWorld;
				if( physicsWorldData != null )
				{
					unsafe
					{
						var toNativeFree = new List<IntPtr>();

						var wheelsSettings = stackalloc Scene.PhysicsWorldClass.VehicleWheelSettings[ dynamicData.Wheels.Length ];

						for( int nWheel = 0; nWheel < dynamicData.Wheels.Length; nWheel++ )
						{
							ref var wheel = ref dynamicData.Wheels[ nWheel ];
							ref var wheelSettings = ref wheelsSettings[ nWheel ];

							//!!!!maybe change?
							wheelSettings.Position = wheel.InitialPosition + new Vector3F( 0, 0, wheel.Diameter * 0.5f );
							//wheelSettings.Position = wheel.InitialPosition;
							wheelSettings.Direction = new Vector3F( 0, 0, -1 );

							wheelSettings.Radius = wheel.Diameter * 0.5f;
							wheelSettings.Width = wheel.Width;

							//Wheels mode
							if( wheels )
							{
								var isFront = wheel.Front;

								if( isFront )
								{
									wheelSettings.SuspensionMinLength = (float)typeFrontWheel.SuspensionMinLength;
									wheelSettings.SuspensionMaxLength = (float)typeFrontWheel.SuspensionMaxLength;
									wheelSettings.SuspensionPreloadLength = (float)typeFrontWheel.SuspensionPreloadLength;
									wheelSettings.SuspensionFrequency = (float)typeFrontWheel.SuspensionFrequency;
									wheelSettings.SuspensionDamping = (float)typeFrontWheel.SuspensionDamping;
								}
								else
								{
									wheelSettings.SuspensionMinLength = (float)typeRearWheel.SuspensionMinLength;
									wheelSettings.SuspensionMaxLength = (float)typeRearWheel.SuspensionMaxLength;
									wheelSettings.SuspensionPreloadLength = (float)typeRearWheel.SuspensionPreloadLength;
									wheelSettings.SuspensionFrequency = (float)typeRearWheel.SuspensionFrequency;
									wheelSettings.SuspensionDamping = (float)typeRearWheel.SuspensionDamping;
								}

								//Wheels specific

								double mass = isFront ? typeFrontWheel.Mass.Value : typeRearWheel.Mass.Value;
								wheelSettings.Inertia = 0.5f * (float)mass * wheelSettings.Radius * wheelSettings.Radius;

								if( isFront )
								{
									{
										wheelSettings.LongitudinalFrictionCount = typeFrontWheel.LongitudinalFriction.Count;

										var data = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Physics, wheelSettings.LongitudinalFrictionCount * 2 * sizeof( float ) );
										toNativeFree.Add( data );

										wheelSettings.LongitudinalFrictionData = (float*)data;
										for( int n = 0; n < wheelSettings.LongitudinalFrictionCount; n++ )
										{
											var item = typeFrontWheel.LongitudinalFriction[ n ].Value;
											wheelSettings.LongitudinalFrictionData[ n * 2 + 0 ] = item.Point;
											wheelSettings.LongitudinalFrictionData[ n * 2 + 1 ] = item.Value;
										}
									}

									{
										wheelSettings.LateralFrictionCount = typeFrontWheel.LateralFriction.Count;

										var data = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Physics, wheelSettings.LateralFrictionCount * 2 * sizeof( float ) );
										toNativeFree.Add( data );

										wheelSettings.LateralFrictionData = (float*)data;
										for( int n = 0; n < wheelSettings.LateralFrictionCount; n++ )
										{
											var item = typeFrontWheel.LateralFriction[ n ].Value;
											wheelSettings.LateralFrictionData[ n * 2 + 0 ] = item.Point;
											wheelSettings.LateralFrictionData[ n * 2 + 1 ] = item.Value;
										}
									}
								}
								else
								{
									{
										wheelSettings.LongitudinalFrictionCount = typeRearWheel.LongitudinalFriction.Count;

										var data = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Physics, wheelSettings.LongitudinalFrictionCount * 2 * sizeof( float ) );
										toNativeFree.Add( data );

										wheelSettings.LongitudinalFrictionData = (float*)data;
										for( int n = 0; n < wheelSettings.LongitudinalFrictionCount; n++ )
										{
											var item = typeRearWheel.LongitudinalFriction[ n ].Value;
											wheelSettings.LongitudinalFrictionData[ n * 2 + 0 ] = item.Point;
											wheelSettings.LongitudinalFrictionData[ n * 2 + 1 ] = item.Value;
										}
									}

									{
										wheelSettings.LateralFrictionCount = typeRearWheel.LateralFriction.Count;

										var data = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Physics, wheelSettings.LateralFrictionCount * 2 * sizeof( float ) );
										toNativeFree.Add( data );

										wheelSettings.LateralFrictionData = (float*)data;
										for( int n = 0; n < wheelSettings.LateralFrictionCount; n++ )
										{
											var item = typeRearWheel.LateralFriction[ n ].Value;
											wheelSettings.LateralFrictionData[ n * 2 + 0 ] = item.Point;
											wheelSettings.LateralFrictionData[ n * 2 + 1 ] = item.Value;
										}
									}
								}

								if( isFront )
								{
									wheelSettings.MaxSteerAngle = (float)typeFrontWheel.MaxSteeringAngle.Value.InRadians();
									wheelSettings.MaxBrakeTorque = (float)typeFrontWheel.MaxBrakeTorque;
									wheelSettings.MaxHandBrakeTorque = (float)typeFrontWheel.MaxHandBrakeTorque;
									wheelSettings.AngularDamping = (float)typeFrontWheel.AngularDamping;
								}
								else
								{
									wheelSettings.MaxSteerAngle = (float)typeRearWheel.MaxSteeringAngle.Value.InRadians();
									wheelSettings.MaxBrakeTorque = (float)typeRearWheel.MaxBrakeTorque;
									wheelSettings.MaxHandBrakeTorque = (float)typeRearWheel.MaxHandBrakeTorque;
									wheelSettings.AngularDamping = (float)typeRearWheel.AngularDamping;
								}
							}

							//Tracks mode
							if( tracks )
							{
								if( wheel.WhichIndex < typeWheels.Length )
								{
									var trackWheel = typeWheels[ wheel.WhichIndex ];

									wheelSettings.SuspensionMinLength = (float)trackWheel.SuspensionMinLength;
									wheelSettings.SuspensionMaxLength = (float)trackWheel.SuspensionMaxLength;
									wheelSettings.SuspensionPreloadLength = (float)trackWheel.SuspensionPreloadLength;
									wheelSettings.SuspensionFrequency = (float)trackWheel.SuspensionFrequency;
									wheelSettings.SuspensionDamping = (float)trackWheel.SuspensionDamping;

									//Tracks specific
									wheelSettings.TrackLongitudinalFriction = (float)trackWheel.TracksLongitudinalFriction;
									wheelSettings.TrackLateralFriction = (float)trackWheel.TracksLateralFriction;
								}
							}
						}

						var engineNormalizedTorque = new float[ type.EngineNormalizedTorque.Count * 2 ];
						for( int n = 0; n < type.EngineNormalizedTorque.Count; n++ )
						{
							var item = type.EngineNormalizedTorque[ n ].Value;
							engineNormalizedTorque[ n * 2 + 0 ] = item.Point;
							engineNormalizedTorque[ n * 2 + 1 ] = item.Value;
						}

						var transmissionGearRatios = type.TransmissionGearRatios.Value;
						var transmissionReverseGearRatios = type.TransmissionReverseGearRatios.Value;

						var antiRollbars = new OpenList<float>( 6 );
						{
							if( wheels )
							{
								if( typeFrontWheel.AntiRollBarStiffness != 0 && typeFrontWheel.Pair )
								{
									for( int n = 0; n < typeFrontWheel.Count; n++ )
									{
										antiRollbars.Add( CalculateWheelIndex( dynamicData, DynamicDataClass.WhichWheel.FrontLeft, n ) );
										antiRollbars.Add( CalculateWheelIndex( dynamicData, DynamicDataClass.WhichWheel.FrontRight, n ) );
										antiRollbars.Add( (float)typeFrontWheel.AntiRollBarStiffness );
									}
								}
								if( typeRearWheel.AntiRollBarStiffness != 0 && typeRearWheel.Pair )
								{
									for( int n = 0; n < typeRearWheel.Count; n++ )
									{
										antiRollbars.Add( CalculateWheelIndex( dynamicData, DynamicDataClass.WhichWheel.RearLeft, n ) );
										antiRollbars.Add( CalculateWheelIndex( dynamicData, DynamicDataClass.WhichWheel.RearRight, n ) );
										antiRollbars.Add( (float)typeRearWheel.AntiRollBarStiffness );
									}
								}
							}

							if( tracks )
							{
								//!!!!?
							}
						}

						var differentials = new OpenList<float>( 12 );
						if( wheels )
						{
							if( typeFrontWheel.Drive )
							{
								for( int n = 0; n < typeFrontWheel.Count; n++ )
								{
									differentials.Add( CalculateWheelIndex( dynamicData, DynamicDataClass.WhichWheel.FrontLeft, n ) );
									if( typeFrontWheel.Pair )
										differentials.Add( CalculateWheelIndex( dynamicData, DynamicDataClass.WhichWheel.FrontRight, n ) );
									else
										differentials.Add( -1 );
									differentials.Add( (float)typeFrontWheel.DifferentialRatio );
									differentials.Add( (float)typeFrontWheel.DifferentialLeftRightSplit );
									differentials.Add( (float)typeFrontWheel.DifferentialLimitedSlipRatio );
									differentials.Add( (float)typeFrontWheel.DifferentialEngineTorqueRatio );
								}
							}

							if( typeRearWheel.Drive )
							{
								for( int n = 0; n < typeRearWheel.Count; n++ )
								{
									differentials.Add( CalculateWheelIndex( dynamicData, DynamicDataClass.WhichWheel.RearLeft, n ) );
									if( typeRearWheel.Pair )
										differentials.Add( CalculateWheelIndex( dynamicData, DynamicDataClass.WhichWheel.RearRight, n ) );
									else
										differentials.Add( -1 );
									differentials.Add( (float)typeRearWheel.DifferentialRatio );
									differentials.Add( (float)typeRearWheel.DifferentialLeftRightSplit );
									differentials.Add( (float)typeRearWheel.DifferentialLimitedSlipRatio );
									differentials.Add( (float)typeRearWheel.DifferentialEngineTorqueRatio );
								}
							}
						}

						fixed( double* pTransmissionGearRatios = transmissionGearRatios, pTransmissionReverseGearRatios = transmissionReverseGearRatios )
						{
							fixed( float* pAntiRollbars = antiRollbars.Data, pEngineNormalizedTorque = engineNormalizedTorque, pDifferentials = differentials.Data )
							{
								var visualScale = Vector3F.One;

								dynamicData.constraint = physicsWorldData.CreateConstraintVehicle( this, PhysicalBody, dynamicData.Wheels.Length, wheelsSettings, ref visualScale, (float)( typeFrontWheel != null ? typeFrontWheel.AntiRollBarStiffness : 0 ), (float)( typeRearWheel != null ? typeRearWheel.AntiRollBarStiffness : 0 ), (float)type.MaxPitchRollAngle.Value.InRadians(), (float)type.EngineMaxTorque, (float)type.EngineMinRPM, (float)type.EngineMaxRPM, type.TransmissionAuto, transmissionGearRatios.Length, pTransmissionGearRatios, transmissionReverseGearRatios.Length, pTransmissionReverseGearRatios, (float)type.TransmissionSwitchTime, (float)type.TransmissionClutchReleaseTime, (float)type.TransmissionSwitchLatency, (float)type.TransmissionShiftUpRPM, (float)type.TransmissionShiftDownRPM, (float)type.TransmissionClutchStrength,/* type.FrontWheelDrive, type.RearWheelDrive, (float)type.FrontWheelDifferentialRatio, (float)type.FrontWheelDifferentialLeftRightSplit, (float)type.FrontWheelDifferentialLimitedSlipRatio, (float)type.FrontWheelDifferentialEngineTorqueRatio, (float)type.RearWheelDifferentialRatio, (float)type.RearWheelDifferentialLeftRightSplit, (float)type.RearWheelDifferentialLimitedSlipRatio, (float)type.RearWheelDifferentialEngineTorqueRatio, */type.MaxSlopeAngle.Value.InRadians().ToRadianF(), tracks, antiRollbars.Count / 3, pAntiRollbars, (float)type.DifferentialLimitedSlipRatio, type.EngineNormalizedTorque.Count, pEngineNormalizedTorque, (float)type.EngineInertia, (float)type.EngineAngularDamping, differentials.Count / 6, pDifferentials, type.TrackDrivenWheel, (float)type.TrackInertia, (float)type.TrackAngularDamping, (float)type.TrackMaxBrakeTorque, (float)type.TrackDifferentialRatio );
							}
						}

						foreach( var pointer in toNativeFree )
							NativeUtility.Free( pointer );
					}
				}
			}

			//!!!!slowly. make caching in the VehicleType

			var typeSeatItems = dynamicData.VehicleType.GetComponents<SeatItem>();
			var seatItems = new List<DynamicDataClass.SeatItemData>( typeSeatItems.Length * ( type.SeatPairs ? 2 : 1 ) );
			foreach( var typeSeatItem in typeSeatItems )
			{
				var typeTransform = typeSeatItem.Transform.Value;
				var typeExitTransform = typeSeatItem.ExitTransform.Value;

				if( typeSeatItem.Enabled )
				{
					{
						var seatItem = new DynamicDataClass.SeatItemData();
						seatItem.SourceComponent = typeSeatItem;
						seatItem.Transform = typeSeatItem.Transform;
						seatItem.SpineAngle = typeSeatItem.SpineAngle;
						seatItem.LegsAngle = typeSeatItem.LegsAngle;
						//seatItem.EyeOffset = seatComponent.EyeOffset;
						seatItem.ExitTransform = typeSeatItem.ExitTransform;

						seatItems.Add( seatItem );
					}

					if( type.SeatPairs && typeTransform.Position.Y != 0 )
					{
						var seatItem = new DynamicDataClass.SeatItemData();
						seatItem.SourceComponent = typeSeatItem;
						seatItem.Mirrored = true;

						var pos = typeTransform.Position;
						pos.Y = -pos.Y;
						seatItem.Transform = typeTransform.UpdatePosition( pos );

						seatItem.SpineAngle = typeSeatItem.SpineAngle;
						seatItem.LegsAngle = typeSeatItem.LegsAngle;

						var exitPos = typeExitTransform.Position;
						exitPos.Y = -exitPos.Y;
						seatItem.ExitTransform = typeExitTransform.UpdatePosition( exitPos );

						seatItems.Add( seatItem );
					}
				}
			}
			if( seatItems.Count != 0 )
				dynamicData.Seats = seatItems.ToArray();

			//also can take seats from Vehicle component

			if( dynamicData.Seats == null )
				dynamicData.Seats = Array.Empty<DynamicDataClass.SeatItemData>();

			//track fragments
			if( tracks )
			{
				dynamicData.TrackFragmentMesh = type.TrackFragmentMesh.Value;

				//if( dynamicData.TrackFragmentMesh != null )
				//{
				//	dynamicData.Tracks = new DynamicDataClass.TrackItem[ 2 ];
				//	for( int nTrack = 0; nTrack < 2; nTrack++ )
				//	{
				//		var trackItem = new DynamicDataClass.TrackItem();
				//		dynamicData.Tracks[ nTrack ] = trackItem;


				//		//!!!!calculate count at initialization?

				//		trackItem.Fragments = new DynamicDataClass.TrackFragmentItem[ 30 ];


				//		for( int nFragment = 0; nFragment < trackItem.Fragments.Length; nFragment++ )
				//		{
				//			ref var fragment = ref trackItem.Fragments[ nFragment ];
				//			fragment.Position = new Vector3F( (float)nFragment / 4, nTrack == 0 ? -2 : 2, 0 );
				//			fragment.Rotation = QuaternionF.Identity;
				//		}

				//		//!!!!
				//		//trackItem.Curve = new CurveBezier();

				//	}
				//}
			}

			UpdateAdditionalItems();

			//turrets
			if( !NetworkIsClient )
			{
				//network single or server
				//create turrets and other object in space from type

				var turrets = new List<DynamicDataClass.TurretItem>();
				//var lights = new List<DynamicDataClass.LightItem>();

				foreach( var objectInSpaceType in dynamicData.VehicleType.GetComponents<ObjectInSpace>() )
				{
					if( objectInSpaceType.Enabled )
					{
						var turretType = objectInSpaceType as Turret;
						if( turretType != null )
						{
							var obj = (Turret)turretType.Clone();
							obj.Enabled = false;
							obj.SaveSupport = false;
							obj.CanBeSelected = false;

							AddComponent( obj );

							//apply world transform
							obj.Transform = TransformV * obj.TransformV;
							foreach( var objectInSpace2 in obj.GetComponents<ObjectInSpace>( checkChildren: true ) )
								objectInSpace2.Transform = TransformV * objectInSpace2.TransformV;

							var turretItem = new DynamicDataClass.TurretItem();
							turretItem.InitialTransform = objectInSpaceType.TransformV;
							turretItem.Turret = obj;

							//can add other types of constructions (side turrets)

							var horizontalConstraint = obj.GetComponent<Constraint_SixDOF>();
							if( horizontalConstraint != null )
							{
								turretItem.HorizontalConstraint = horizontalConstraint;

								//configure constraint
								//BodyA
								if( !horizontalConstraint.BodyA.ReferenceOrValueSpecified )
									horizontalConstraint.BodyA = this;
								//BodyB. reset reference to optimize
								horizontalConstraint.BodyB = horizontalConstraint.BodyB.Value;

								var weapon = obj.GetComponent<Weapon>();
								if( weapon != null )
								{
									turretItem.Weapon = weapon;
									var verticalConstraint = weapon.GetComponent<Constraint_SixDOF>();
									if( verticalConstraint != null )
									{
										turretItem.VerticalConstraint = verticalConstraint;

										//configure constraint
										//BodyB. reset reference to optimize
										verticalConstraint.BodyA = verticalConstraint.BodyA.Value;
										//BodyB. reset reference to optimize
										verticalConstraint.BodyB = verticalConstraint.BodyB.Value;
									}
								}
							}

							turrets.Add( turretItem );

							obj.Enabled = true;
						}

						//if( NetworkIsSingle )
						//{
						//	var lightType = objectInSpaceType as Light;
						//	if( lightType != null )
						//	{
						//		var lightItem = new DynamicDataClass.LightItem();
						//		lightItem.LightType = lightType;
						//		lights.Add( lightItem );
						//	}
						//}
					}
				}

				if( turrets.Count != 0 )
					dynamicData.Turrets = turrets.ToArray();
				//if( lights.Count != 0 )
				//	dynamicData.Lights = lights.ToArray();
			}
			else
			{
				//network client

				//get synced turrets and weapons
				{
					var turrets = new List<DynamicDataClass.TurretItem>();

					foreach( var turret in GetComponents<Turret>() )
					{
						var turretItem = new DynamicDataClass.TurretItem();
						turretItem.Turret = turret;

						var weapon = turret.GetComponent<Weapon>();
						if( weapon != null )
							turretItem.Weapon = weapon;

						turrets.Add( turretItem );
					}

					if( turrets.Count != 0 )
						dynamicData.Turrets = turrets.ToArray();
				}
			}

			//lights
			if( NetworkIsSingleOrClient )
			{
				var lightTypeComponents = dynamicData.VehicleType.GetComponents<Light>();

				var lights = new List<DynamicDataClass.LightItem>();
				var existHeadlightLow = false;
				var existHeadlightHigh = false;

				//!!!!slowly?

				//var lowerNames = new ESet<string>();
				//foreach( var lightType in lightTypeComponents )
				//{
				//	if( lightType.Enabled )
				//	{
				//		var lowerName = lightType.Name.ToLower();
				//		lowerNames.AddWithCheckAlreadyContained( lowerName );
				//	}
				//}

				foreach( var lightType in lightTypeComponents )
				{
					if( lightType.Enabled )
					{
						var lowerName = lightType.Name.ToLower();

						var lightItem = new DynamicDataClass.LightItem();
						{
							lightItem.LightType = lightType;

							//detect purpose by component name
							foreach( var enumValue in Enum.GetValues( typeof( DynamicDataClass.LightTypePurpose ) ) )
							{
								var enumString = enumValue.ToString().ToLower().Replace( "_", " " );
								if( lowerName.Contains( enumString ) )
								{
									lightItem.LightTypePurpose = (DynamicDataClass.LightTypePurpose)enumValue;
									break;
								}
							}
							if( lightItem.LightTypePurpose == DynamicDataClass.LightTypePurpose.Unknown && lowerName.Contains( "turn" ) )
							{
								if( lightType.TransformV.Position.Y > 0 )
									lightItem.LightTypePurpose = DynamicDataClass.LightTypePurpose.Left_Turn;
								else
									lightItem.LightTypePurpose = DynamicDataClass.LightTypePurpose.Right_Turn;
							}

							lights.Add( lightItem );
						}

						//add mirrored
						if( dynamicData.VehicleType.LightPairs && lightType.TransformV.Position.Y != 0 )
						{
							var lightItem2 = new DynamicDataClass.LightItem();
							lightItem2.LightType = lightType;
							lightItem2.Mirrored = true;

							switch( lightItem.LightTypePurpose )
							{
							case DynamicDataClass.LightTypePurpose.Right_Turn:
								lightItem2.LightTypePurpose = DynamicDataClass.LightTypePurpose.Left_Turn;
								break;
							case DynamicDataClass.LightTypePurpose.Left_Turn:
								lightItem2.LightTypePurpose = DynamicDataClass.LightTypePurpose.Right_Turn;
								break;
							default:
								lightItem2.LightTypePurpose = lightItem.LightTypePurpose;
								break;
							}

							lights.Add( lightItem2 );


							//var originalIsLeft = lowerName.Contains( "left" );
							//var originalIsRight = lowerName.Contains( "right" );
							//if( originalIsLeft || originalIsRight )
							//{
							//	var mirroredLowerName = "";
							//	if( originalIsLeft )
							//		mirroredLowerName = lowerName.Replace( "left", "right" );
							//	else if( originalIsRight )
							//		mirroredLowerName = lowerName.Replace( "right", "left" );

							//	if( !lowerNames.Contains( mirroredLowerName ) )
							//	{
							//		var lightItem2 = new DynamicDataClass.LightItem();
							//		lightItem2.LightType = lightType;
							//		lightItem2.Mirrored = true;

							//		switch( lightItem.LightTypePurpose )
							//		{
							//		case DynamicDataClass.LightTypePurpose.Right_Turn:
							//			lightItem2.LightTypePurpose = DynamicDataClass.LightTypePurpose.Left_Turn;
							//			break;
							//		case DynamicDataClass.LightTypePurpose.Left_Turn:
							//			lightItem2.LightTypePurpose = DynamicDataClass.LightTypePurpose.Right_Turn;
							//			break;
							//		default:
							//			lightItem2.LightTypePurpose = lightItem.LightTypePurpose;
							//			break;
							//		}

							//		lights.Add( lightItem2 );
							//	}
							//}
						}

						if( lightItem.LightTypePurpose == DynamicDataClass.LightTypePurpose.Headlight_Low )
							existHeadlightLow = true;
						if( lightItem.LightTypePurpose == DynamicDataClass.LightTypePurpose.Headlight_High )
							existHeadlightHigh = true;
					}
				}

				//foreach( var objectInSpaceType in dynamicData.VehicleType.GetComponents<ObjectInSpace>() )
				//{
				//	if( objectInSpaceType.Enabled )
				//	{
				//		var lightType = objectInSpaceType as Light;
				//		if( lightType != null )
				//		{
				//			var lowerName = lightType.Name.ToLower();

				//			var lightItem = new DynamicDataClass.LightItem();
				//			lightItem.LightType = lightType;

				//			//detect purpose by component name
				//			foreach( var enumValue in Enum.GetValues<DynamicDataClass.LightTypePurpose>() )
				//			{
				//				var enumString = enumValue.ToString().ToLower().Replace( "_", " " );
				//				if( lowerName.Contains( enumString ) )
				//				{
				//					lightItem.LightTypePurpose = enumValue;
				//					break;
				//				}
				//			}

				//			lights.Add( lightItem );
				//		}
				//	}
				//}

				if( lights.Count != 0 )
				{
					dynamicData.Lights = lights.ToArray();
					dynamicData.LightsExistHeadlightLow = existHeadlightLow;
					dynamicData.LightsExistHeadlightHigh = existHeadlightHigh;
				}
			}

			//!!!!
			////aero engines
			//{
			//	var aeroEngines = new List<DynamicDataClass.AeroEngineItem>();

			//	foreach( var engineType in dynamicData.VehicleType.GetComponents<VehicleAeroEngine>() )
			//	{
			//		if( engineType.Enabled )
			//		{
			//			var engineItem = new DynamicDataClass.AeroEngineItem();
			//			engineItem.EngineType = engineType;

			//			//create engine instance
			//			{
			//				var obj = (VehicleAeroEngine)engineItem.EngineType.Clone();
			//				engineItem.Engine = obj;
			//				obj.Enabled = false;
			//				obj.SaveSupport = false;
			//				obj.CanBeSelected = false;

			//				AddComponent( obj );

			//				//apply world transform
			//				obj.Transform = TransformV * obj.TransformV;
			//				foreach( var objectInSpace2 in obj.GetComponents<ObjectInSpace>( checkChildren: true ) )
			//					objectInSpace2.Transform = TransformV * objectInSpace2.TransformV;

			//				ObjectInSpaceUtility.Attach( this, obj, TransformOffset.ModeEnum.Elements );

			//				obj.Enabled = true;

			//				//!!!!?
			//				//lightItem.Light.Transform.Touch();
			//			}

			//			aeroEngines.Add( engineItem );
			//		}
			//	}

			//	if( aeroEngines.Count != 0 )
			//		dynamicData.AeroEngines = aeroEngines.ToArray();
			//}

			//LocalBoundsDefault
			{
				var meshResult = dynamicData.Mesh?.Result;
				if( meshResult != null )
				{
					dynamicData.LocalBoundsDefault = meshResult.SpaceBounds.BoundingBox;

					//add local bounds of wheels
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

					//add local bounds of turrets
					if( !NetworkIsClient )
					{
						//network single or server
						if( dynamicData.Turrets != null )
						{
							foreach( var turretItem in dynamicData.Turrets )
							{
								var turretMeshResult = turretItem.Turret.Mesh.Value?.Result;
								if( turretMeshResult != null )
								{
									var b = turretItem.InitialTransform * turretMeshResult.SpaceBounds.BoundingBox;
									dynamicData.LocalBoundsDefault.Add( ref b );
								}
							}
						}
					}
					else
					{
						//network client
						foreach( var objectInSpaceType in dynamicData.VehicleType.GetComponents<ObjectInSpace>() )
						{
							var turretType = objectInSpaceType as Turret;
							if( turretType != null )
							{
								var turretMeshResult = turretType.Mesh.Value?.Result;
								if( turretMeshResult != null )
								{
									var b = turretType.TransformV * turretMeshResult.SpaceBounds.BoundingBox;
									dynamicData.LocalBoundsDefault.Add( ref b );
								}
							}
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

			//UpdateSpaceBoundsOverride();
			SpaceBoundsUpdate();

			//needBeActiveBecausePhysicsVelocity = false;
			//lastTransformToCalculateDynamicState = TransformV;
			//needBeActiveBecauseTransformChange = false;
			//lastLinearVelocity = Vector3.Zero;

			driverInputNeedUpdate = true;
			needRecreateDynamicData = false;
			fullyDisabledRemainingTime = 0;
			//skipFirstUpdateFromLibrary = 0.1f;
		}

		public void DestroyDynamicData()
		{
			if( dynamicData != null )
			{
				if( dynamicData.Turrets != null )
				{
					foreach( var turretItem in dynamicData.Turrets )
					{
						if( turretItem.TurretTurnSoundChannel != null )
						{
							turretItem.TurretTurnSoundChannel.Stop();
							turretItem.TurretTurnSoundChannel = null;
						}
					}
				}

				foreach( var turret in GetComponents<Turret>() )
					turret.RemoveFromParent( false );

				//!!!!
				//foreach( var engine in GetComponents<VehicleAeroEngine>() )
				//	engine.RemoveFromParent( false );

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

		public delegate void InteractionGetInfoEventDelegate( Vehicle sender, GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info );
		public event InteractionGetInfoEventDelegate InteractionGetInfoEvent;

		public virtual void InteractionGetInfo( GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info )
		{
			//control by a character
			var character = gameMode.ObjectControlledByPlayer.Value as Character;
			if( character != null && !character.Sitting )
			{
				var seatIndex = GetFreeSeat();
				if( seatIndex != -1 )
				{
					info = new InteractiveObjectObjectInfo();
					info.AllowInteract = true;

					var keyString = gameMode.KeyInteract1.Value.ToString();
					if( SystemSettings.MobileDevice )
						keyString = "Interact";
					info.Text = $"Press {keyString} to control the vehicle.";

					//!!!!impl mobile
					if( !SystemSettings.MobileDevice )
					{
						var lights = dynamicData.Lights;
						if( lights != null )
						{
							if( lights.FirstOrDefault( l => l.LightTypePurpose == DynamicDataClass.LightTypePurpose.Headlight_Low ) != null )
								info.Text += $"\n{gameMode.KeyHeadlightsLow1.Value} to use headlights low.";
							if( lights.FirstOrDefault( l => l.LightTypePurpose == DynamicDataClass.LightTypePurpose.Headlight_High ) != null )
								info.Text += $"\n{gameMode.KeyHeadlightsHigh1.Value} to use headlights high.";
							if( lights.FirstOrDefault( l => l.LightTypePurpose == DynamicDataClass.LightTypePurpose.Left_Turn ) != null )
								info.Text += $"\n{gameMode.KeyLeftTurnSignal1.Value} to use the left turn signal.";
							if( lights.FirstOrDefault( l => l.LightTypePurpose == DynamicDataClass.LightTypePurpose.Right_Turn ) != null )
								info.Text += $"\n{gameMode.KeyRightTurnSignal1.Value} to use the right turn signal.";
						}
					}
				}
			}
			InteractionGetInfoEvent?.Invoke( this, gameMode, initiator, ref info );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public ObjectInSpace GetObjectOnSeat( int seatIndex )
		{
			if( seatIndex < ObjectsOnSeats.Count )
				return ObjectsOnSeats[ seatIndex ];
			return null;
		}

		public virtual bool InteractionInputMessage( GameMode gameMode, Component initiator, InputMessage message )
		{
			var keyDown = message as InputMessageKeyDown;
			if( keyDown != null )
			{
				if( keyDown.Key == gameMode.KeyInteract1 || keyDown.Key == gameMode.KeyInteract2 )
				{
					//start control by a character
					var character = gameMode.ObjectControlledByPlayer.Value as Character;
					if( character != null )
					{
						var seatIndex = GetFreeSeat();
						if( seatIndex != -1 )
						{
							if( NetworkIsClient )
							{
								var m = BeginNetworkMessageToServer( "PutObjectToSeat" );
								if( m != null )
								{
									var writer = m.Writer;
									writer.WriteVariableInt32( seatIndex );
									writer.WriteVariableUInt64( (ulong)character.NetworkID );
									m.End();
								}
							}
							else
							{
								PutObjectToSeat( gameMode, seatIndex, character );
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

		public virtual void InteractionEnter( ObjectInteractionContext context )
		{
		}

		public virtual void InteractionExit( ObjectInteractionContext context )
		{
		}

		public virtual void InteractionUpdate( ObjectInteractionContext context )
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
						var gameMode = ParentScene?.GetComponent<GameMode>();
						if( gameMode != null )
						{
							PutObjectToSeat( gameMode, seatIndex, obj );
							networkLogic.ServerChangeObjectControlled( client.User, this );
						}
					}
				}
			}

			return true;
		}

		public virtual void PutObjectToSeat( GameMode gameMode, int seatIndex, ObjectInSpace obj )
		{
			//!!!!always need to create?
			//create VehicleInputProcessing if not exists
			{
				var inputProcessing = GetComponent<VehicleInputProcessing>();
				if( inputProcessing == null )
					inputProcessing = CreateComponent<VehicleInputProcessing>();
			}

			while( seatIndex >= ObjectsOnSeats.Count )
				ObjectsOnSeats.Add( null );
			ObjectsOnSeats[ seatIndex ] = ReferenceUtility.MakeRootReference( obj );

			//deactivate active items for character
			var character = obj as Character;
			if( character != null && gameMode != null )
				character.DeactivateAllItems( gameMode );

			if( NetworkIsSingleOrClient )//!!!!?
			{
				var updated = false;
				UpdateObjectOnSeat( seatIndex, ref updated );
			}

			//remainingTimeToUpdateObjectsOnSeat = 0;
			fullyDisabledRemainingTime = 0;
		}

		public virtual void RemoveObjectFromSeat( int seatIndex, bool resetDriverInput )
		{
			if( dynamicData != null )
			{
				DynamicDataClass.SeatItemData seatItem = null;
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
							character.Sitting = false;
							character.Collision = true;
							character.Visible = true;

							var tr = TransformV * seatItem.ExitTransform;
							var scaleFactor = character.GetScaleFactor();
							if( !CharacterUtility.FindFreePlace( character, tr.Position, character.TypeCached.Radius * scaleFactor * 3, -character.TypeCached.Height * scaleFactor / 3, character.TypeCached.Height * scaleFactor / 10, out var freePlacePosition ) )
							{
								if( !CharacterUtility.FindFreePlace( character, tr.Position, character.TypeCached.Radius * scaleFactor * 3, -character.TypeCached.Height * scaleFactor / 3, character.TypeCached.Height * scaleFactor / 3, out freePlacePosition ) )
								{
									if( !CharacterUtility.FindFreePlace( character, tr.Position, character.TypeCached.Radius * scaleFactor * 5, -character.TypeCached.Height * scaleFactor, character.TypeCached.Height * scaleFactor, out freePlacePosition ) )
									{
										freePlacePosition = tr.Position + new Vector3( 0, 0, character.TypeCached.Height * scaleFactor );
									}
								}
							}
							tr = tr.UpdatePosition( freePlacePosition );
							character.SetTransformAndTurnToDirectionInstantly( tr );
							character.NotifyInstantMovement();
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

			//remainingTimeToUpdateObjectsOnSeat = 0;
			fullyDisabledRemainingTime = 0;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void UpdateAdditionalItems()
		{
			if( dynamicData.Wheels != null )
			{
				var wheelCount = dynamicData.Wheels.Length;

				//track fragments
				var fragmentCountLeft = 0;
				//var fragmentTotalTimeLeft = 0.0;
				var fragmentCountRight = 0;
				//var fragmentTotalTimeRight = 0.0;
				if( dynamicData.TrackFragmentMesh != null )
				{
					for( int nTrack = 0; nTrack < 2; nTrack++ )
					{
						//var trackItem = dynamicData.Tracks[ nTrack ];

						int wheelFrom;
						int wheelTo;
						if( nTrack == 0 )
						{
							wheelFrom = 0;
							wheelTo = wheelCount / 2;
						}
						else
						{
							wheelFrom = wheelCount / 2;
							wheelTo = wheelCount;
						}

						ref var firstWheel = ref dynamicData.Wheels[ wheelFrom ];
						ref var lastWheel = ref dynamicData.Wheels[ wheelTo - 1 ];


						Curve curve;
						if( nTrack == 0 )
						{
							//optimization: can use float 2D curve
							if( lastTrackFragmentsCurveLeft == null )
								lastTrackFragmentsCurveLeft = new CurveCubicSpline();
							curve = lastTrackFragmentsCurveLeft;
						}
						else
						{
							if( lastTrackFragmentsCurveRight == null )
								lastTrackFragmentsCurveRight = new CurveCubicSpline();
							curve = lastTrackFragmentsCurveRight;
						}
						curve.Clear();


						var time = 0.0;
						var previousPoint = new Vector3( -10000, 0, 0 );

						//bottom points. start from second
						for( int n = wheelFrom + 1; n < wheelTo; n++ )
						{
							ref var wheel = ref dynamicData.Wheels[ n ];

							var p = wheel.CurrentPosition + new Vector3F( 0, 0, -wheel.Diameter * 0.5f );

							if( previousPoint == new Vector3( -10000, 0, 0 ) )
								previousPoint = p;
							time += ( p - previousPoint ).Length();
							previousPoint = p;

							curve.AddPoint( time, p );
						}

						//last wheel
						{
							var p = lastWheel.CurrentPosition + new Vector3F( -lastWheel.Diameter * 0.5f, 0, -lastWheel.Diameter * 0.5f ).GetNormalize() * ( lastWheel.Diameter * 0.5f );
							time += 2.0 * Math.PI * lastWheel.Diameter * 0.5 * 0.125;
							previousPoint = p;

							curve.AddPoint( time, p );
						}
						{
							var p = lastWheel.CurrentPosition + new Vector3F( -lastWheel.Diameter * 0.5f, 0, 0 );
							time += 2.0 * Math.PI * lastWheel.Diameter * 0.5 * 0.125;
							previousPoint = p;

							curve.AddPoint( time, p );
						}
						{
							var p = lastWheel.CurrentPosition + new Vector3F( -lastWheel.Diameter * 0.5f, 0, lastWheel.Diameter * 0.5f ).GetNormalize() * ( lastWheel.Diameter * 0.5f );
							time += 2.0 * Math.PI * lastWheel.Diameter * 0.5 * 0.125;
							previousPoint = p;

							curve.AddPoint( time, p );
						}

						//top points
						for( int n = wheelTo - 1; n >= wheelFrom; n-- )
						{
							ref var wheel = ref dynamicData.Wheels[ n ];

							var p = wheel.CurrentPosition + new Vector3F( 0, 0, wheel.Diameter * 0.5f );
							if( n == wheelTo - 1 )
								time += 2.0 * Math.PI * lastWheel.Diameter * 0.5 * 0.125;
							else
								time += ( p - previousPoint ).Length();
							previousPoint = p;

							curve.AddPoint( time, p );
						}

						//first wheel
						{
							var p = firstWheel.CurrentPosition + new Vector3F( firstWheel.Diameter * 0.5f, 0, firstWheel.Diameter * 0.5f ).GetNormalize() * ( lastWheel.Diameter * 0.5f );
							time += 2.0 * Math.PI * lastWheel.Diameter * 0.5 * 0.125;
							previousPoint = p;

							curve.AddPoint( time, p );
						}
						{
							var p = firstWheel.CurrentPosition + new Vector3F( firstWheel.Diameter * 0.5f, 0, 0 );
							time += 2.0 * Math.PI * lastWheel.Diameter * 0.5 * 0.125;
							previousPoint = p;

							curve.AddPoint( time, p );
						}
						{
							var p = firstWheel.CurrentPosition + new Vector3F( firstWheel.Diameter * 0.5f, 0, -firstWheel.Diameter * 0.5f ).GetNormalize() * ( lastWheel.Diameter * 0.5f );
							time += 2.0 * Math.PI * lastWheel.Diameter * 0.5 * 0.125;
							previousPoint = p;

							curve.AddPoint( time, p );
						}

						//bottom point
						{
							ref var wheel = ref dynamicData.Wheels[ wheelFrom ];

							var p = wheel.CurrentPosition + new Vector3F( 0, 0, -wheel.Diameter * 0.5f );
							time += 2.0 * Math.PI * lastWheel.Diameter * 0.5 * 0.125;
							previousPoint = p;

							curve.AddPoint( time, p );
						}

						//another bottom point
						{
							ref var wheel = ref dynamicData.Wheels[ wheelFrom + 1 ];

							var p = wheel.CurrentPosition + new Vector3F( 0, 0, -wheel.Diameter * 0.5f );
							time += ( p - previousPoint ).Length();
							previousPoint = p;

							curve.AddPoint( time, p );
						}


						var totalTime = time;

						int fragmentCount;
						{
							var step = dynamicData.VehicleType.TrackFragmentLength.Value;
							//anti freeze
							if( step <= 0.01 )
								step = 0.01;
							fragmentCount = (int)( totalTime / step ) + 1;
						}

						if( nTrack == 0 )
						{
							fragmentCountLeft = fragmentCount;
							//fragmentTotalTimeLeft = totalTime;
						}
						else
						{
							fragmentCountRight = fragmentCount;
							//fragmentTotalTimeRight = totalTime;
						}

						//!!!!strange DegreeToRadian

						if( nTrack == 0 )
							trackFragmentsLinearVelocityLeft = MathEx.DegreeToRadian( firstWheel.AngularVelocity ) * firstWheel.Diameter * 0.5;
						else
							trackFragmentsLinearVelocityRight = MathEx.DegreeToRadian( firstWheel.AngularVelocity ) * firstWheel.Diameter * 0.5;

						//if( nTrack == 0 )
						//	trackFragmentsLinearVelocityLeft = firstWheel.AngularVelocity * firstWheel.Diameter * 0.5;
						//else
						//	trackFragmentsLinearVelocityRight = firstWheel.AngularVelocity * firstWheel.Diameter * 0.5;
					}
				}

				//calculate total additional items count
				var itemCount = 0;
				for( int n = 0; n < wheelCount; n++ )
				{
					ref var wheel = ref dynamicData.Wheels[ n ];
					if( wheel.Mesh != null )
						itemCount++;
				}
				if( dynamicData.TrackFragmentMesh != null )
					itemCount += fragmentCountLeft + fragmentCountRight; //itemCount += dynamicData.Tracks[ 0 ].Fragments.Length * 2;

				var currentAdditionalItems = AdditionalItems;
				if( currentAdditionalItems == null || currentAdditionalItems.Length != itemCount )
				{
					if( itemCount != 0 )
						currentAdditionalItems = new AdditionalItem[ itemCount ];
					else
						currentAdditionalItems = null;
				}

				//wheels
				var currentIndex = 0;
				for( int n = 0; n < wheelCount; n++ )
				{
					ref var wheel = ref dynamicData.Wheels[ n ];
					if( wheel.Mesh != null )
					{
						ref var item = ref currentAdditionalItems[ currentIndex++ ];

						item.Mesh = wheel.Mesh;
						item.Position = wheel.CurrentPosition;
						if( wheel.Right )
							item.Rotation = wheel.CurrentRotation * Quaternion.FromRotateByZ( Math.PI );
						else
							item.Rotation = wheel.CurrentRotation;
						item.Scale = Vector3.One;
						item.Color = ColorValue.One;

						if( double.IsNaN( item.Rotation.X ) )
							Log.Fatal( "Vehicle: UpdateAdditionalItems: double.IsNaN( item.Rotation.X )." );
					}
				}

				//track fragments
				if( dynamicData.TrackFragmentMesh != null && itemCount != 0 )
				{
					for( int nTrack = 0; nTrack < 2; nTrack++ )
					{
						var curve = nTrack == 0 ? lastTrackFragmentsCurveLeft : lastTrackFragmentsCurveRight;
						//var totalTime = nTrack == 0 ? fragmentTotalTimeLeft : fragmentTotalTimeRight;
						var fragmentCount = nTrack == 0 ? fragmentCountLeft : fragmentCountRight;
						var trackFragmentsLinearPosition = nTrack == 0 ? trackFragmentsLinearPositionLeft : trackFragmentsLinearPositionRight;

						var step = dynamicData.VehicleType.TrackFragmentLength.Value;
						//anti freeze
						if( step <= 0.01 )
							step = 0.01;

						var t = trackFragmentsLinearPosition % step;
						//var t = 0.0;

						for( int n = 0; n < fragmentCount; n++ ) //for( var t = 0.0; t < totalTime; t += step )
						{
							var p = curve.CalculateValueByTime( t );

							if( currentIndex >= currentAdditionalItems.Length )
								Log.Fatal( "currentIndex >= currentAdditionalItems.Length." );

							ref var item = ref currentAdditionalItems[ currentIndex++ ];
							item.Mesh = dynamicData.TrackFragmentMesh;
							item.Position = p;

							//!!!!slowly? maybe use Quaternion.FromRotateBy()

							double t2 = t;
							if( n < fragmentCount / 2 )
								t2 += 0.01;
							else
								t2 -= 0.01;
							var p2 = curve.CalculateValueByTime( t2 );

							var p3 = ( p2 - p ).GetNormalize();
							var up = Vector3.Cross( p3, new Vector3( 0, 1, 0 ) );

							item.Rotation = new Matrix3( p3, new Vector3( 0, 1, 0 ), up ).ToQuaternion();

							if( double.IsNaN( item.Rotation.X ) )
								Log.Fatal( "Vehicle: UpdateAdditionalItems: double.IsNaN( item.Rotation.X )." );

							item.Scale = Vector3.One;
							item.Color = ColorValue.One;

							t += step;
						}



						//for( int nFragment = 0; nFragment < trackItem.Fragments.Length; nFragment++ )
						//{
						//	ref var fragment = ref trackItem.Fragments[ nFragment ];

						//	ref var item = ref currentAdditionalItems[ currentIndex++ ];
						//	item.Mesh = dynamicData.TrackFragmentMesh;

						//	//!!!!
						//	item.Position = fragment.Position;
						//	item.Rotation = Quaternion.Identity;


						//	//item.Position = wheel.CurrentPosition;
						//	//if( wheel.Right )
						//	//	item.Rotation = wheel.CurrentRotation * Quaternion.FromRotateByZ( Math.PI );
						//	//else
						//	//	item.Rotation = wheel.CurrentRotation;

						//	item.Scale = Vector3.One;
						//	item.Color = ColorValue.One;
						//}




						////var currentIndex = 0;

						////for( int n = 0; n < wheelCount; n++ )
						////{
						////	ref var wheel = ref dynamicData.Wheels[ n ];

						////	if( nTrack == 0 && wheel.Which == DynamicDataClass.WhichWheel.TrackLeft || nTrack == 1 && wheel.Which == DynamicDataClass.WhichWheel.TrackRight )
						////	{
						////		//wheel.CurrentPosition
						////		//wheel.Diameter

						////		curve.AddPoint( 0, wheel.CurrentPosition - new Vector3F( 0, 0, wheel.Diameter * 0.5f ) );

						////	}

						////	//if( wheel.Mesh != null )
						////	//{
						////	//	ref var item = ref currentAdditionalItems[ currentIndex++ ];

						////	//	item.Mesh = wheel.Mesh;
						////	//	item.Position = wheel.CurrentPosition;
						////	//	if( wheel.Right )
						////	//		item.Rotation = wheel.CurrentRotation * Quaternion.FromRotateByZ( Math.PI );
						////	//	else
						////	//		item.Rotation = wheel.CurrentRotation;
						////	//	item.Scale = Vector3.One;
						////	//	item.Color = ColorValue.One;
						////	//}
						////}



						////dynamicData.Wheels[0];

						////trackItem

						////curve.CalculateValueByTime

						////for( int nFragment = 0; nFragment < trackItem.Fragments.Length; nFragment++ )
						////{
						////	ref var fragment = ref trackItem.Fragments[ nFragment ];

						////	ref var item = ref currentAdditionalItems[ currentIndex++ ];
						////	item.Mesh = dynamicData.TrackFragmentMesh;

						////	//!!!!
						////	item.Position = fragment.Position;
						////	item.Rotation = Quaternion.Identity;


						////	//item.Position = wheel.CurrentPosition;
						////	//if( wheel.Right )
						////	//	item.Rotation = wheel.CurrentRotation * Quaternion.FromRotateByZ( Math.PI );
						////	//else
						////	//	item.Rotation = wheel.CurrentRotation;

						////	item.Scale = Vector3.One;
						////	item.Color = ColorValue.One;
						////}



						//for( int nFragment = 0; nFragment < trackItem.Fragments.Length; nFragment++ )
						//{
						//	ref var fragment = ref trackItem.Fragments[ nFragment ];

						//	ref var item = ref currentAdditionalItems[ currentIndex++ ];
						//	item.Mesh = dynamicData.TrackFragmentMesh;

						//	//!!!!
						//	item.Position = fragment.Position;
						//	item.Rotation = Quaternion.Identity;


						//	//item.Position = wheel.CurrentPosition;
						//	//if( wheel.Right )
						//	//	item.Rotation = wheel.CurrentRotation * Quaternion.FromRotateByZ( Math.PI );
						//	//else
						//	//	item.Rotation = wheel.CurrentRotation;

						//	item.Scale = Vector3.One;
						//	item.Color = ColorValue.One;
						//}

					}
				}

				AdditionalItems = currentAdditionalItems;


				//var wheelCount = dynamicData.Wheels.Length;

				//var itemCount = 0;
				//for( int n = 0; n < wheelCount; n++ )
				//{
				//	ref var wheel = ref dynamicData.Wheels[ n ];
				//	var mesh = wheel.Front ? dynamicData.FrontWheelMesh : dynamicData.RearWheelMesh;
				//	if( mesh != null )
				//		itemCount++;
				//}

				//var currentAdditionalItems = AdditionalItems;
				//if( currentAdditionalItems == null || currentAdditionalItems.Length != itemCount )
				//{
				//	if( itemCount != 0 )
				//		currentAdditionalItems = new AdditionalItem[ itemCount ];
				//	else
				//		currentAdditionalItems = null;
				//}

				//var currentIndex = 0;
				//for( int n = 0; n < wheelCount; n++ )
				//{
				//	ref var wheel = ref dynamicData.Wheels[ n ];
				//	var mesh = wheel.Front ? dynamicData.FrontWheelMesh : dynamicData.RearWheelMesh;

				//	if( mesh != null )
				//	{
				//		ref var item = ref currentAdditionalItems[ currentIndex++ ];

				//		item.Mesh = wheel.Front ? dynamicData.FrontWheelMesh : dynamicData.RearWheelMesh;
				//		item.Position = wheel.CurrentPosition;
				//		item.Rotation = wheel.CurrentRotation;

				//		if( wheel.Right )
				//			item.Rotation = wheel.CurrentRotation * Quaternion.FromRotateByZ( Math.PI );
				//		else
				//			item.Rotation = wheel.CurrentRotation;

				//		item.Scale = Vector3.One;
				//		item.Color = ColorValue.One;
				//	}
				//}

				//AdditionalItems = currentAdditionalItems;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		bool IsMustBeDynamic()
		{
			if( /*needBeActiveBecauseDriverInput || */mustBeDynamicRemainingTime > 0 )
				return true;

			var body = PhysicalBody;
			if( body != null && body.Active && ( body.LinearVelocity != Vector3F.Zero || body.AngularVelocity != Vector3F.Zero ) )
				return true;

			if( HeadlightsLow > 0 || HeadlightsHigh > 0 )
				return true;

			if( MotorOn )
				return true;
			//if( inputProcessingCached == null || inputProcessingCached.Parent != this )
			//	inputProcessingCached = GetComponent<VehicleInputProcessing>();
			//if( inputProcessingCached != null && inputProcessingCached.InputEnabled )
			//	return true;

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

			fullyDisabledRemainingTime = 0;
		}

		public delegate void ProcessDamageBeforeDelegate( Vehicle sender, long whoFired, ref float damage, ref object anyData, ref bool handled );
		public event ProcessDamageBeforeDelegate ProcessDamageBefore;
		public static event ProcessDamageBeforeDelegate ProcessDamageBeforeAll;

		public delegate void ProcessDamageAfterDelegate( Vehicle sender, long whoFired, float damage, object anyData, double oldHealth );
		public event ProcessDamageAfterDelegate ProcessDamageAfter;
		public static event ProcessDamageAfterDelegate ProcessDamageAfterAll;

		public void ProcessDamage( long whoFired, float damage, object anyData )
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

		protected override bool OnReceiveNetworkMessageFromServer( string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromServer( message, reader ) )
				return false;

			if( message == "UpdateWheels" )
			{
				var count = reader.ReadByte();

				var wheels = dynamicData?.Wheels;
				if( wheels != null && count == wheels.Length )
				{
					for( int n = 0; n < wheels.Length; n++ )
					{
						ref var wheel = ref dynamicData.Wheels[ n ];

						wheel.CurrentPosition.Z = reader.ReadHalf();
						wheel.CurrentRotation = reader.ReadQuaternionH().ToQuaternionF();
						//for tracks
						wheel.AngularVelocity = reader.ReadSingle();
					}

					if( !reader.Complete() )
						return false;

					//!!!!here? maybe to OnSimulationStepClient
					UpdateAdditionalItems();
				}
			}
			else if( message == "MotorState" )
			{
				var motorOn = reader.ReadBoolean();
				var currentGear = reader.ReadVariableInt32();
				var currentRPM = reader.ReadSingle();

				if( !reader.Complete() )
					return false;

				if( dynamicData != null )
				{
					SetMotorOn( motorOn );
					dynamicData.CurrentGear = currentGear;
					dynamicData.CurrentRPM = currentRPM;
				}
			}

			return true;
		}

		public void LookToPosition( Vector3? value )//, bool turnInstantly )
		{
			RequiredLookToPosition = value;

			//wake up
			if( RequiredLookToPosition.HasValue )
			{
				if( PhysicalBody != null )
					PhysicalBody.Active = true;
			}

			//!!!!
			//if( turnInstantly )
			//{
			//	CurrentLookToPosition = RequiredLookToPosition;
			//	RequiredLookToPosition = null;
			//}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void UpdateTurrets( ref bool updated )
		{
			var turrets = dynamicData?.Turrets;

			if( turrets != null )//&& RequiredLookToPosition.HasValue )
			{
				for( int nTurret = 0; nTurret < turrets.Length; nTurret++ )
				{
					var turretItem = turrets[ nTurret ];

					var weapon = turretItem.Weapon;
					if( weapon != null )
					{
						if( RequiredLookToPosition.HasValue )
						{
							updated = true;

							//control turret (horizontal)
							var horizontalConstraint = turretItem.HorizontalConstraint;
							if( horizontalConstraint != null && horizontalConstraint.AngularZAxis.Value != PhysicsAxisMode.Locked )
							{
								var worldDirection = RequiredLookToPosition.Value - weapon.TransformV.Position;
								var localDirection = TransformV.Rotation.GetInverse() * worldDirection;

								var direction2D = localDirection.ToVector2();
								if( direction2D != Vector2.Zero )
								{
									direction2D.Normalize();
									var horizontal = Math.Atan2( direction2D.Y, direction2D.X );

									horizontalConstraint.AngularZAxisMotorTarget = MathEx.RadianToDegree( horizontal );
								}
							}

							//control weapon (vertical)
							var verticalConstraint = turretItem.VerticalConstraint;
							if( verticalConstraint != null && verticalConstraint.AngularYAxis.Value != PhysicsAxisMode.Locked )
							{
								var worldDirection = RequiredLookToPosition.Value - weapon.TransformV.Position;
								var localDirection = turretItem.Turret.TransformV.Rotation.GetInverse() * worldDirection;

								var direction2D = new Vector2( localDirection.X, localDirection.Z );
								if( direction2D != Vector2.Zero )
								{
									direction2D.Normalize();
									var vertical = Math.Atan2( direction2D.Y, direction2D.X );

									//Log.Info( MathEx.RadianToDegree( vertical ).ToString() );

									verticalConstraint.AngularYAxisMotorTarget = MathEx.RadianToDegree( -vertical );
								}

								//var direction2D = localDirection.ToVector2();
								//if( direction2D != Vector2.Zero )
								//{
								//	direction2D.Normalize();
								//	var horizontal = Math.Atan2( direction2D.Y, direction2D.X );

								//	verticalConstraint.AngularZAxisMotorTarget = MathEx.RadianToDegree( horizontal );
								//}
							}
						}

						//turret turn sound
						{
							var needPlaySound = false;
							if( RequiredLookToPosition.HasValue )
							{
								var requiredDirection = ( RequiredLookToPosition.Value - weapon.TransformV.Position ).GetNormalize();
								var currentDirection = weapon.TransformV.Rotation.GetForward();
								needPlaySound = !requiredDirection.Equals( currentDirection, 0.03 );
							}

							if( needPlaySound )
							{
								if( turretItem.TurretTurnSoundChannel == null )
								{
									var turnTurretSound = dynamicData.VehicleType.TurretTurnSound.Value;
									turretItem.TurretTurnSoundChannel = ParentScene.SoundPlay( turnTurretSound, TransformV.Position, 0.3, 1, true );
								}
							}
							else
							{
								if( turretItem.TurretTurnSoundChannel != null )
								{
									turretItem.TurretTurnSoundChannel.Stop();
									turretItem.TurretTurnSoundChannel = null;
								}
							}

							if( turretItem.TurretTurnSoundChannel != null )
								updated = true;

							//if( NetworkIsServer )
							//{
							//}
						}
					}
				}
			}
		}

		[Browsable( false )]
		public DynamicDataClass DynamicData
		{
			get { return dynamicData; }
		}

		protected override void OnGetRenderSceneDataAddToFrameData( ViewportRenderingContext context, GetRenderSceneDataMode mode, ref RenderingPipeline.RenderSceneData.MeshItem item, ref bool skip )
		{
			base.OnGetRenderSceneDataAddToFrameData( context, mode, ref item, ref skip );

			if( visualHeadlightLow != 0 || visualHeadlightHigh != 0 || visualBrake != 0 || visualLeftTurnSignal != 0 || visualRightTurnSignal != 0 || visualMoveBack != 0 )
			{
				//format of shader parameters. use material in "Content\Vehicles\Default\Body.gltf" as example.

				//[ 0 ].X - packed two halfs
				//X - Headlight low beam. 0 - 1 interval.
				//Y - Headlight high beam. 0 - 1 interval.

				//[ 0 ].Y - packed two halfs
				//X - Brake. 0 - 1 interval.
				//Y - Left turn signal. 0 - 1 interval.

				//[ 0 ].Z - packed two halfs
				//X - Right turn signal. 0 - 1 interval.
				//Y - Move back. 0 - 1 interval.

				//!!!!impl
				////Position light
				////Y - Front fog light. 0 - 1 interval.
				////Y - Daytime running lights. 0 - 1 interval.

				var p = new Vector4F[ 2 ];
				p[ 0 ].X = HalfType.PackTwoHalfsToFloat( visualHeadlightLow, visualHeadlightHigh );
				p[ 0 ].Y = HalfType.PackTwoHalfsToFloat( visualBrake, GetVisualLightFactorByPurpose( DynamicDataClass.LightTypePurpose.Left_Turn ) );
				p[ 0 ].Z = HalfType.PackTwoHalfsToFloat( GetVisualLightFactorByPurpose( DynamicDataClass.LightTypePurpose.Right_Turn ), visualMoveBack );

				//!!!!position light

				item.ObjectInstanceParameters = p;
			}
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

		void SimulateMotorOn( ref bool updated )
		{
			if( motorOnRemainingTime > 0 )
			{
				updated = true;

				motorOnRemainingTime -= Time.SimulationDelta;
				if( motorOnRemainingTime < 0 )
					motorOnRemainingTime = 0;
			}
		}

		public void SetMotorOn( bool value = true )
		{
			motorOnRemainingTime = value ? 2 : 0;
		}

		[Browsable( false )]
		public bool MotorOn
		{
			get { return motorOnRemainingTime > 0; }
		}

		void SimulateMotorSound( ref bool updated )
		{
			if( dynamicData == null || SoundWorld.BackendNull )
				return;

			//motor on, off
			if( motorOnPrevious.HasValue && MotorOn != motorOnPrevious.Value )
			{
				updated = true;

				if( MotorOn )
					SoundPlay( dynamicData.VehicleType.MotorOnSound );
				else
					SoundPlay( dynamicData.VehicleType.MotorOffSound );
			}

			Sound needSound = null;
			if( MotorOn )
				needSound = dynamicData.VehicleType.GetGearSound( dynamicData.CurrentGear );

			if( needSound != currentMotorSound )
			{
				//change motor sound

				updated = true;

				if( motorSoundChannel != null )
				{
					motorSoundChannel.Stop();
					motorSoundChannel = null;
				}

				if( needSound != null )
					motorSoundChannel = ParentScene.SoundPlay( needSound, TransformV.Position, 0.3, 1, true );

				currentMotorSound = needSound;
			}

			//update motor channel position and pitch
			if( motorSoundChannel != null )
			{
				updated = true;

				var min = dynamicData.VehicleType.EngineMinRPM.Value;
				var max = dynamicData.VehicleType.EngineMaxRPM.Value;

				if( min != max )
				{
					var factor = ( dynamicData.CurrentRPM - min ) / ( max - min );
					motorSoundChannel.Pitch = factor + 0.5;
				}
				motorSoundChannel.Position = TransformV.Position;
			}

			motorOnPrevious = MotorOn;
		}

		void SimulateCurrentGearSound( ref bool updated )
		{
			if( dynamicData == null || SoundWorld.BackendNull )
				return;

			if( currentGearSoundPlayed != dynamicData.CurrentGear )
			{
				updated = true;

				if( MotorOn )
				{
					if( dynamicData.CurrentGear > currentGearSoundPlayed )
						ParentScene.SoundPlay( dynamicData.VehicleType.GearUpSound, TransformV.Position );
					else
						ParentScene.SoundPlay( dynamicData.VehicleType.GearDownSound, TransformV.Position );
				}

				currentGearSoundPlayed = dynamicData.CurrentGear;
			}
		}

		public void SimulateVisualHeadlights( ref bool updated, bool immediate = false )
		{
			{
				var headlights = HeadlightsLow.Value;
				if( visualHeadlightLow != headlights && dynamicData != null && dynamicData.LightsExistHeadlightLow )
				{
					updated = true;

					if( visualHeadlightLow < headlights )
					{
						visualHeadlightLow += immediate ? 10000 : Time.SimulationDelta * 10;
						if( visualHeadlightLow > headlights )
							visualHeadlightLow = headlights;
					}
					else
					{
						visualHeadlightLow -= immediate ? 10000 : Time.SimulationDelta * 2;
						if( visualHeadlightLow < headlights )
							visualHeadlightLow = headlights;
					}
				}
			}

			{
				var headlights = HeadlightsHigh.Value;
				if( visualHeadlightHigh != headlights && dynamicData != null && dynamicData.LightsExistHeadlightHigh )
				{
					updated = true;
					if( visualHeadlightHigh < headlights )
					{
						visualHeadlightHigh += immediate ? 10000 : Time.SimulationDelta * 10;
						if( visualHeadlightHigh > headlights )
							visualHeadlightHigh = headlights;
					}
					else
					{
						visualHeadlightHigh -= immediate ? 10000 : Time.SimulationDelta * 2;
						if( visualHeadlightHigh < headlights )
							visualHeadlightHigh = headlights;
					}
				}
			}
		}

		public void SimulateVisualBrake( ref bool updated, bool immediate = false )
		{
			var brake = Brake.Value;
			//if( Throttle < 0 )
			//	brake = Math.Max( brake, -Throttle.Value );

			if( visualBrake != brake )
			{
				updated = true;

				if( visualBrake < brake )
				{
					visualBrake += immediate ? 10000 : Time.SimulationDelta * 10;
					if( visualBrake > brake )
						visualBrake = brake;
				}
				else
				{
					visualBrake -= immediate ? 10000 : Time.SimulationDelta * 2;
					if( visualBrake < brake )
						visualBrake = brake;
				}
			}
		}

		public void SimulateVisualTurnSignals( ref bool updated, bool immediate = false )
		{
			{
				var signal = LeftTurnSignal.Value;
				if( visualLeftTurnSignal != signal )
				{
					updated = true;

					if( visualLeftTurnSignal < signal )
					{
						visualLeftTurnSignal += immediate ? 10000 : Time.SimulationDelta * 10;
						if( visualLeftTurnSignal > signal )
							visualLeftTurnSignal = signal;
					}
					else
					{
						visualLeftTurnSignal -= immediate ? 10000 : Time.SimulationDelta * 2;
						if( visualLeftTurnSignal < signal )
							visualLeftTurnSignal = signal;
					}
				}
			}

			{
				var signal = RightTurnSignal.Value;
				if( visualRightTurnSignal != signal )
				{
					updated = true;

					if( visualRightTurnSignal < signal )
					{
						visualRightTurnSignal += immediate ? 10000 : Time.SimulationDelta * 10;
						if( visualRightTurnSignal > signal )
							visualRightTurnSignal = signal;
					}
					else
					{
						visualRightTurnSignal -= immediate ? 10000 : Time.SimulationDelta * 2;
						if( visualRightTurnSignal < signal )
							visualRightTurnSignal = signal;
					}
				}
			}

			//set updated because of blinking
			if( visualLeftTurnSignal > 0 || visualRightTurnSignal > 0 )
				updated = true;
		}

		public void SimulateVisualMoveBack( ref bool updated, bool immediate = false )
		{
			var moveBack = 0.0f;
			if( IsOnGround() )
			{
				if( dynamicData.Wheels.Length != 0 ) //for( int n = 0; n < dynamicData.Wheels.Length; n++ )
				{
					var n = 0;
					ref var wheel = ref dynamicData.Wheels[ n ];
					//!!!! < -1.0
					if( /*wheel.ContactBody != null &&*/ wheel.AngularVelocity < -1.0 )
						moveBack = 1;
				}
			}

			if( visualMoveBack != moveBack )
			{
				updated = true;

				if( visualMoveBack < moveBack )
				{
					visualMoveBack += immediate ? 10000 : Time.SimulationDelta * 10;
					if( visualMoveBack > moveBack )
						visualMoveBack = moveBack;
				}
				else
				{
					visualMoveBack -= immediate ? 10000 : Time.SimulationDelta * 2;
					if( visualMoveBack < moveBack )
						visualMoveBack = moveBack;
				}
			}
		}

		float GetTurnBlinkingFactor()
		{
			if( randomTimeAddForTurnSignals == 0 )
				randomTimeAddForTurnSignals = staticRandom.Next( 100.0 );

			var contoller = ParentRoot.HierarchyController;
			var time = contoller != null ? contoller.SimulationTime : EngineApp.EngineTime;
			time += randomTimeAddForTurnSignals;

			//!!!!configure settings
			var factor = MathEx.Saturate( (float)MathEx.Sin( time * 4 ) + 0.5f );

			return factor;
		}

		float GetVisualLightFactorByPurpose( DynamicDataClass.LightTypePurpose purpose )
		{
			switch( purpose )
			{
			case DynamicDataClass.LightTypePurpose.Headlight_Low: return visualHeadlightLow;
			case DynamicDataClass.LightTypePurpose.Headlight_High: return visualHeadlightHigh;
			case DynamicDataClass.LightTypePurpose.Brake: return visualBrake;

			case DynamicDataClass.LightTypePurpose.Left_Turn:
				{
					var v = visualLeftTurnSignal;
					if( v > 0 )
						v *= GetTurnBlinkingFactor();
					return v;
				}

			case DynamicDataClass.LightTypePurpose.Right_Turn:
				{
					var v = visualRightTurnSignal;
					if( v > 0 )
						v *= GetTurnBlinkingFactor();
					return v;
				}

			case DynamicDataClass.LightTypePurpose.Move_Back: return visualMoveBack;
			case DynamicDataClass.LightTypePurpose.Position: return Math.Max( visualHeadlightLow, visualHeadlightHigh );
			}

			return 0;
		}

		public void UpdateLightComponents()
		{
			if( dynamicData?.Lights != null )
			{
				foreach( var lightItem in dynamicData.Lights )
				{
					var factor = GetVisualLightFactorByPurpose( lightItem.LightTypePurpose );

					if( factor > 0 )
					{
						if( lightItem.Light == null )
						{
							var obj = (Light)lightItem.LightType.Clone();

							//optimize lights
							if( dynamicData.VehicleType.LightOptimize )
							{
								var purpose = lightItem.LightTypePurpose;
								if( purpose == DynamicDataClass.LightTypePurpose.Brake || purpose == DynamicDataClass.LightTypePurpose.Left_Turn || purpose == DynamicDataClass.LightTypePurpose.Right_Turn )
								{
									obj.ShadowTextureSize = Light.ShadowTextureSizeType.Quarter;
									obj.ShadowLODWorst = true;
									obj.ShadowNearClipDistance = 0.01;
								}
							}

							lightItem.Light = obj;
							obj.Enabled = false;
							obj.SaveSupport = false;
							obj.CanBeSelected = false;

							AddComponent( obj );

							//apply world transform
							if( lightItem.Mirrored )
							{
								var sourceTr = obj.TransformV;
								var sourcePos = sourceTr.Position;
								var sourceRot = sourceTr.Rotation;

								var pos = sourcePos;
								var forward = sourceRot.GetForward();
								var up = sourceRot.GetUp();

								pos.Y = -pos.Y;
								forward.Y = -forward.Y;
								up.Y = -up.Y;

								var rot = Quaternion.LookAt( forward, up );

								var tr = new Transform( pos, rot, sourceTr.Scale );
								obj.Transform = TransformV * tr;
								//!!!!rotate?
								foreach( var objectInSpace2 in obj.GetComponents<ObjectInSpace>( checkChildren: true ) )
									objectInSpace2.Transform = TransformV * objectInSpace2.TransformV;
							}
							else
							{
								obj.Transform = TransformV * obj.TransformV;
								foreach( var objectInSpace2 in obj.GetComponents<ObjectInSpace>( checkChildren: true ) )
									objectInSpace2.Transform = TransformV * objectInSpace2.TransformV;
							}

							ObjectInSpaceUtility.Attach( this, obj, TransformOffset.ModeEnum.Elements );

							obj.Enabled = true;
						}

						lightItem.Light.Color = lightItem.LightType.Color * new ColorValue( factor, factor, factor );
						//lightItem.Light.Brightness = factor * lightItem.LightType.Brightness;

						lightItem.Light.Transform.Touch();
					}
					else
					{
						if( lightItem.Light != null )
						{
							lightItem.Light.RemoveFromParent( false );
							lightItem.Light = null;
						}
					}
				}
			}
		}

		protected virtual void SimulateAeroEngines()
		{
			//!!!!
			//if( dynamicData?.AeroEngines != null )
			//{
			//	foreach( var engineItem in dynamicData.AeroEngines )
			//	{
			//		if( PhysicalBody != null )
			//		{
			//			var engine = engineItem.Engine;

			//			var force = engine.Force.Value * Throttle * Time.SimulationDelta;
			//			if( force > 0 )
			//			{
			//				//simple implementation

			//				PhysicalBody.ApplyForce( ( engine.TransformV.Rotation * new Vector3( force, 0, 0 ) ).ToVector3F(), Vector3F.Zero );


			//				//var bodyTransform = new Matrix4( PhysicalBody.Rotation.ToMatrix3(), PhysicalBody.Position );
			//				//var engineTransform = engine.TransformV.ToMatrix4();
			//				//var relativeTransform = bodyTransform * engineTransform.GetInverse();
			//				//relativeTransform.Decompose( out var pos, out Quaternion _, out _ );
			//				//PhysicalBody.ApplyForce( PhysicalBody.Rotation * new Vector3F( (float)force, 0, 0 ), pos.ToVector3F() );
			//			}
			//		}
			//	}
			//}
		}

		protected override void OnClientConnectedAfterRootComponentEnabled( ServerNetworkService_Components.ClientItem client )
		{
			base.OnClientConnectedAfterRootComponentEnabled( client );

			if( dynamicData != null )
			{
				var m = BeginNetworkMessage( client, "MotorState" );
				m.Writer.Write( MotorOn );
				m.Writer.WriteVariableInt32( dynamicData.CurrentGear );
				m.Writer.Write( dynamicData.CurrentRPM );
				m.End();
			}
		}

		void SimulateTrackFragmentsLinearPositions()
		{
			if( dynamicData?.TrackFragmentMesh != null )
			{
				trackFragmentsLinearPositionLeft += trackFragmentsLinearVelocityLeft;
				trackFragmentsLinearPositionRight += trackFragmentsLinearVelocityRight;
			}
		}
	}
}
