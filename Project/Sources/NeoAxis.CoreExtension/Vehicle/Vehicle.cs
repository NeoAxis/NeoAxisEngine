// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
////!!!!
//using Internal.BulletSharp;

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
	/// A basic class to make vehicles of various types.
	/// </summary>
	[ResourceFileExtension( "vehicle" )]
	[AddToResourcesWindow( @"Base\3D\Vehicle", -7999 )]
#if !DEPLOY
	[Editor.EditorControl( typeof( Editor.VehicleEditor ), true )]
	[Editor.Preview( typeof( Editor.VehiclePreview ) )]
	[Editor.PreviewImage( typeof( Editor.VehiclePreviewImage ) )]
#endif
	public class Vehicle : ObjectInSpace, InteractiveObject
	{
		DynamicData dynamicData;
		bool needRecreateDynamicData;

		//!!!!smooth?
		Vector3? lastTransformPosition;
		Vector3 lastLinearVelocity;

		//[FieldSerialize( FieldSerializeSerializationTypes.World )]
		//Vector3 linearVelocityForSerialization;

		Vector3 groundRelativeVelocity;
		Vector3[] groundRelativeVelocitySmoothArray;
		Vector3 groundRelativeVelocitySmooth;

		//!!!!
		//double allowToSleepTime;

		/////////////////////////////////////////
		//Basic

		//!!!!default
		/// <summary>
		/// The method of simulating the vehicle.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( SimulationModeEnum.RealPhysics )]
		public Reference<SimulationModeEnum> SimulationMode
		{
			get { if( _simulationMode.BeginGet() ) SimulationMode = _simulationMode.Get( this ); return _simulationMode.value; }
			set { if( _simulationMode.BeginSet( ref value ) ) { try { SimulationModeChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _simulationMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SimulationMode"/> property value changes.</summary>
		public event Action<Vehicle> SimulationModeChanged;
		ReferenceField<SimulationModeEnum> _simulationMode = SimulationModeEnum.RealPhysics;

		//[Category( "Configuration" )]
		//[DefaultValue( WheelShapeEnum.RigidBody )]
		//public Reference<WheelShapeEnum> WheelShape
		//{
		//	get { if( _wheelShape.BeginGet() ) WheelShape = _wheelShape.Get( this ); return _wheelShape.value; }
		//	set { if( _wheelShape.BeginSet( ref value ) ) { try { WheelShapeChanged?.Invoke( this ); } finally { _wheelShape.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="WheelShape"/> property value changes.</summary>
		//public event Action<Vehicle> WheelShapeChanged;
		//ReferenceField<WheelShapeEnum> _wheelShape = WheelShapeEnum.RigidBody;

		/// <summary>
		/// The type of chassis.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( ChassisEnum._4Wheels )]
		public Reference<ChassisEnum> Chassis
		{
			get { if( _chassis.BeginGet() ) Chassis = _chassis.Get( this ); return _chassis.value; }
			set { if( _chassis.BeginSet( ref value ) ) { try { ChassisChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _chassis.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Chassis"/> property value changes.</summary>
		public event Action<Vehicle> ChassisChanged;
		ReferenceField<ChassisEnum> _chassis = ChassisEnum._4Wheels;

		/// <summary>
		/// The position offset for front wheels.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( "1.24 0.7 0.03" )]
		public Reference<Vector3> FrontWheelPosition
		{
			get { if( _frontWheelPosition.BeginGet() ) FrontWheelPosition = _frontWheelPosition.Get( this ); return _frontWheelPosition.value; }
			set { if( _frontWheelPosition.BeginSet( ref value ) ) { try { FrontWheelPositionChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _frontWheelPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelPosition"/> property value changes.</summary>
		public event Action<Vehicle> FrontWheelPositionChanged;
		ReferenceField<Vector3> _frontWheelPosition = new Vector3( 1.24, 0.7, 0.03 );

		/// <summary>
		/// The diameter of a front wheel.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 0.665 )]
		public Reference<double> FrontWheelDiameter
		{
			get { if( _frontWheelDiameter.BeginGet() ) FrontWheelDiameter = _frontWheelDiameter.Get( this ); return _frontWheelDiameter.value; }
			set { if( _frontWheelDiameter.BeginSet( ref value ) ) { try { FrontWheelDiameterChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _frontWheelDiameter.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelDiameter"/> property value changes.</summary>
		public event Action<Vehicle> FrontWheelDiameterChanged;
		ReferenceField<double> _frontWheelDiameter = 0.665;

		/// <summary>
		/// The width of a front wheel.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 0.364 )]
		public Reference<double> FrontWheelWidth
		{
			get { if( _frontWheelWidth.BeginGet() ) FrontWheelWidth = _frontWheelWidth.Get( this ); return _frontWheelWidth.value; }
			set { if( _frontWheelWidth.BeginSet( ref value ) ) { try { FrontWheelWidthChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _frontWheelWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelWidth"/> property value changes.</summary>
		public event Action<Vehicle> FrontWheelWidthChanged;
		ReferenceField<double> _frontWheelWidth = 0.364;

		/// <summary>
		/// The mass of a front wheel.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 18.0 )]
		public Reference<double> FrontWheelMass
		{
			get { if( _frontWheelMass.BeginGet() ) FrontWheelMass = _frontWheelMass.Get( this ); return _frontWheelMass.value; }
			set { if( _frontWheelMass.BeginSet( ref value ) ) { try { FrontWheelMassChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _frontWheelMass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelMass"/> property value changes.</summary>
		public event Action<Vehicle> FrontWheelMassChanged;
		ReferenceField<double> _frontWheelMass = 18.0;

		const string wheelMeshDefault = @"Content\Vehicles\Default\Wheel\scene.gltf|$Mesh";

		/// <summary>
		/// The mesh of a front wheel.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValueReference( wheelMeshDefault )]
		public Reference<Mesh> FrontWheelMesh
		{
			get { if( _frontWheelMesh.BeginGet() ) FrontWheelMesh = _frontWheelMesh.Get( this ); return _frontWheelMesh.value; }
			set { if( _frontWheelMesh.BeginSet( ref value ) ) { try { FrontWheelMeshChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _frontWheelMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelMesh"/> property value changes.</summary>
		public event Action<Vehicle> FrontWheelMeshChanged;
		ReferenceField<Mesh> _frontWheelMesh = new Reference<Mesh>( null, wheelMeshDefault );

		/// <summary>
		/// The position offset for rear wheels.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( "-1.3 0.812 0.03" )]
		public Reference<Vector3> RearWheelPosition
		{
			get { if( _rearWheelPosition.BeginGet() ) RearWheelPosition = _rearWheelPosition.Get( this ); return _rearWheelPosition.value; }
			set { if( _rearWheelPosition.BeginSet( ref value ) ) { try { RearWheelPositionChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _rearWheelPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelPosition"/> property value changes.</summary>
		public event Action<Vehicle> RearWheelPositionChanged;
		ReferenceField<Vector3> _rearWheelPosition = new Vector3( -1.3, 0.812, 0.03 );

		/// <summary>
		/// The diameter of a rear wheel.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 0.665 )]
		public Reference<double> RearWheelDiameter
		{
			get { if( _rearWheelDiameter.BeginGet() ) RearWheelDiameter = _rearWheelDiameter.Get( this ); return _rearWheelDiameter.value; }
			set { if( _rearWheelDiameter.BeginSet( ref value ) ) { try { RearWheelDiameterChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _rearWheelDiameter.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelDiameter"/> property value changes.</summary>
		public event Action<Vehicle> RearWheelDiameterChanged;
		ReferenceField<double> _rearWheelDiameter = 0.665;

		/// <summary>
		/// The width of a rear wheel.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 0.364 )]
		public Reference<double> RearWheelWidth
		{
			get { if( _rearWheelWidth.BeginGet() ) RearWheelWidth = _rearWheelWidth.Get( this ); return _rearWheelWidth.value; }
			set { if( _rearWheelWidth.BeginSet( ref value ) ) { try { RearWheelWidthChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _rearWheelWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelWidth"/> property value changes.</summary>
		public event Action<Vehicle> RearWheelWidthChanged;
		ReferenceField<double> _rearWheelWidth = 0.364;

		/// <summary>
		/// The mass of a rear wheel.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 18.0 )]
		public Reference<double> RearWheelMass
		{
			get { if( _rearWheelMass.BeginGet() ) RearWheelMass = _rearWheelMass.Get( this ); return _rearWheelMass.value; }
			set { if( _rearWheelMass.BeginSet( ref value ) ) { try { RearWheelMassChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _rearWheelMass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelMass"/> property value changes.</summary>
		public event Action<Vehicle> RearWheelMassChanged;
		ReferenceField<double> _rearWheelMass = 18.0;

		/// <summary>
		/// The mesh of a rear wheel.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValueReference( wheelMeshDefault )]
		public Reference<Mesh> RearWheelMesh
		{
			get { if( _rearWheelMesh.BeginGet() ) RearWheelMesh = _rearWheelMesh.Get( this ); return _rearWheelMesh.value; }
			set { if( _rearWheelMesh.BeginSet( ref value ) ) { try { RearWheelMeshChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _rearWheelMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelMesh"/> property value changes.</summary>
		public event Action<Vehicle> RearWheelMeshChanged;
		ReferenceField<Mesh> _rearWheelMesh = new Reference<Mesh>( null, wheelMeshDefault );

		/// <summary>
		/// The drive wheel type.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( WheelDriveEnum.Front )]
		public Reference<WheelDriveEnum> WheelDrive
		{
			get { if( _WheelDrive.BeginGet() ) WheelDrive = _WheelDrive.Get( this ); return _WheelDrive.value; }
			set { if( _WheelDrive.BeginSet( ref value ) ) { try { WheelDriveChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _WheelDrive.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WheelDrive"/> property value changes.</summary>
		public event Action<Vehicle> WheelDriveChanged;
		ReferenceField<WheelDriveEnum> _WheelDrive = WheelDriveEnum.Front;

		//!!!!default
		/// <summary>
		/// The physics material friction of the wheels.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 2.0 )]
		public Reference<double> WheelFriction
		{
			get { if( _wheelFriction.BeginGet() ) WheelFriction = _wheelFriction.Get( this ); return _wheelFriction.value; }
			set { if( _wheelFriction.BeginSet( ref value ) ) { try { WheelFrictionChanged?.Invoke( this ); } finally { _wheelFriction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="WheelFriction"/> property value changes.</summary>
		public event Action<Vehicle> WheelFrictionChanged;
		ReferenceField<double> _wheelFriction = 2.0;

		/// <summary>
		/// The range of suspension travel.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( "-0.1 0.05" )]
		public Reference<Range> SuspensionTravel
		{
			get { if( _suspensionTravel.BeginGet() ) SuspensionTravel = _suspensionTravel.Get( this ); return _suspensionTravel.value; }
			set { if( _suspensionTravel.BeginSet( ref value ) ) { try { SuspensionTravelChanged?.Invoke( this ); } finally { _suspensionTravel.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SuspensionTravel"/> property value changes.</summary>
		public event Action<Vehicle> SuspensionTravelChanged;
		ReferenceField<Range> _suspensionTravel = new Range( -0.1, 0.05 );

		//!!!!default
		/// <summary>
		/// The spring rate of the chassis.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 1.0 )]
		public Reference<double> SpringRate
		{
			get { if( _springRate.BeginGet() ) SpringRate = _springRate.Get( this ); return _springRate.value; }
			set { if( _springRate.BeginSet( ref value ) ) { try { SpringRateChanged?.Invoke( this ); } finally { _springRate.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SpringRate"/> property value changes.</summary>
		public event Action<Vehicle> SpringRateChanged;
		ReferenceField<double> _springRate = 1.0;

		//!!!!
		/// <summary>
		/// The maximal target velocity of throttle.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 30.0 )]
		public Reference<double> ThrottleTargetVelocity
		{
			get { if( _throttleTargetVelocity.BeginGet() ) ThrottleTargetVelocity = _throttleTargetVelocity.Get( this ); return _throttleTargetVelocity.value; }
			set { if( _throttleTargetVelocity.BeginSet( ref value ) ) { try { ThrottleTargetVelocityChanged?.Invoke( this ); } finally { _throttleTargetVelocity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThrottleTargetVelocity"/> property value changes.</summary>
		public event Action<Vehicle> ThrottleTargetVelocityChanged;
		ReferenceField<double> _throttleTargetVelocity = 30.0;

		//!!!!
		//!!!!default
		/// <summary>
		/// The maximal physical force of throttle.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 10.0 )]
		public Reference<double> ThrottleForce
		{
			get { if( _throttleForce.BeginGet() ) ThrottleForce = _throttleForce.Get( this ); return _throttleForce.value; }
			set { if( _throttleForce.BeginSet( ref value ) ) { try { ThrottleForceChanged?.Invoke( this ); } finally { _throttleForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ThrottleForce"/> property value changes.</summary>
		public event Action<Vehicle> ThrottleForceChanged;
		ReferenceField<double> _throttleForce = 10.0;

		//!!!!default
		/// <summary>
		/// The maximal brake force.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 10.0 )]
		public Reference<double> BrakeForce
		{
			get { if( _brakeForce.BeginGet() ) BrakeForce = _brakeForce.Get( this ); return _brakeForce.value; }
			set { if( _brakeForce.BeginSet( ref value ) ) { try { BrakeForceChanged?.Invoke( this ); } finally { _brakeForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BrakeForce"/> property value changes.</summary>
		public event Action<Vehicle> BrakeForceChanged;
		ReferenceField<double> _brakeForce = 10.0;

		//!!!!default
		/// <summary>
		/// The maximal steering force.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 10.0 )]
		public Reference<double> SteeringForce
		{
			get { if( _steeringForce.BeginGet() ) SteeringForce = _steeringForce.Get( this ); return _steeringForce.value; }
			set { if( _steeringForce.BeginSet( ref value ) ) { try { SteeringForceChanged?.Invoke( this ); } finally { _steeringForce.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SteeringForce"/> property value changes.</summary>
		public event Action<Vehicle> SteeringForceChanged;
		ReferenceField<double> _steeringForce = 10.0;

		/// <summary>
		/// The maximal steering angle.
		/// </summary>
		[Category( "Configuration" )]
		[DefaultValue( 45.0 )]
		[Range( 0, 90 )]
		public Reference<Degree> MaxSteeringAngle
		{
			get { if( _maxSteeringAngle.BeginGet() ) MaxSteeringAngle = _maxSteeringAngle.Get( this ); return _maxSteeringAngle.value; }
			set { if( _maxSteeringAngle.BeginSet( ref value ) ) { try { MaxSteeringAngleChanged?.Invoke( this ); NeedRecreateDynamicData(); } finally { _maxSteeringAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxSteeringAngle"/> property value changes.</summary>
		public event Action<Vehicle> MaxSteeringAngleChanged;
		ReferenceField<Degree> _maxSteeringAngle = new Degree( 45.0 );

		/////////////////////////////////////////

		/// <summary>
		/// Whether to visualize debug info.
		/// </summary>
		[Category( "Debug" )]
		[DefaultValue( false )]
		public Reference<bool> DebugVisualization
		{
			get { if( _debugVisualization.BeginGet() ) DebugVisualization = _debugVisualization.Get( this ); return _debugVisualization.value; }
			set { if( _debugVisualization.BeginSet( ref value ) ) { try { DebugVisualizationChanged?.Invoke( this ); } finally { _debugVisualization.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugVisualization"/> property value changes.</summary>
		public event Action<Vehicle> DebugVisualizationChanged;
		ReferenceField<bool> _debugVisualization = false;

		/////////////////////////////////////////

		//!!!!
		//[Category( "Advanced" )]
		//[DefaultValue( 1.0/*0.6*/ )]
		//public Reference<double> MinSpeedToSleepBody
		//{
		//	get { if( _minSpeedToSleepBody.BeginGet() ) MinSpeedToSleepBody = _minSpeedToSleepBody.Get( this ); return _minSpeedToSleepBody.value; }
		//	set { if( _minSpeedToSleepBody.BeginSet( ref value ) ) { try { MinSpeedToSleepBodyChanged?.Invoke( this ); } finally { _minSpeedToSleepBody.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="MinSpeedToSleepBody"/> property value changes.</summary>
		//public event Action<Vehicle> MinSpeedToSleepBodyChanged;
		//ReferenceField<double> _minSpeedToSleepBody = 1.0;//0.6;

		/////////////////////////////////////////

		/// <summary>
		/// The throttle parameter to control the vehicle.
		/// </summary>
		[Category( "Control" )]
		[DefaultValue( 0.0 )]
		[Range( -1, 1 )]
		public Reference<double> Throttle
		{
			get { if( _throttle.BeginGet() ) Throttle = _throttle.Get( this ); return _throttle.value; }
			set { if( _throttle.BeginSet( ref value ) ) { try { ThrottleChanged?.Invoke( this ); } finally { _throttle.EndSet(); } } }
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
			set { if( _brake.BeginSet( ref value ) ) { try { BrakeChanged?.Invoke( this ); } finally { _brake.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Brake"/> property value changes.</summary>
		public event Action<Vehicle> BrakeChanged;
		ReferenceField<double> _brake = 0.0;

		/// <summary>
		/// The steering parameter to control the vehicle.
		/// </summary>
		[Category( "Control" )]
		[DefaultValue( 0.0 )]
		[Range( -1, 1 )]
		public Reference<double> Steering
		{
			get { if( _steering.BeginGet() ) Steering = _steering.Get( this ); return _steering.value; }
			set { if( _steering.BeginSet( ref value ) ) { try { SteeringChanged?.Invoke( this ); } finally { _steering.EndSet(); } } }
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
			set { if( _health.BeginSet( ref value ) ) { try { HealthChanged?.Invoke( this ); } finally { _health.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Health"/> property value changes.</summary>
		public event Action<Vehicle> HealthChanged;
		ReferenceField<double> _health = 0.0;

		/////////////////////////////////////////

		public enum SimulationModeEnum
		{
			RealPhysics,//!!!!для оптимизации: просто выключать констрейнты, тем самым делать упрощенную модель

			//!!!!impl Raycast,
			//!!!!EmulationWithoutPhysicsWithCollision,//!!!!name
			//!!!!EmulationWithoutPhysics,//!!!!name
		}

		/////////////////////////////////////////

		//public enum WheelShapeEnum
		//{
		//	RigidBody,
		//	SoftBody
		//}

		/////////////////////////////////////////

		public enum ChassisEnum
		{
			[DisplayNameEnum( "4 Wheels" )]
			_4Wheels,
			//Caterpillars,
		}

		/////////////////////////////////////////

		public enum WheelDriveEnum
		{
			Front,
			Rear,
			All,
		}

		/////////////////////////////////////////

		class DynamicData
		{
			public SimulationModeEnum SimulationMode;
			public RigidBody MainBody;
			public WheelData[] Wheels;

			////Raycast mode
			//public RaycastVehicle raycastVehicle;

			/////////////////////

			public enum WhichWheel
			{
				FrontLeft,
				FrontRight,
				RearLeft,
				RearRight,
				//Other,
			}

			/////////////////////

			public class WheelData
			{
				public WhichWheel Which;
				public bool WheelDrive;
				public Vector3 LocalPosition;
				public double Diameter;
				public double Width;

				//RealPhysics mode
				public RigidBody RigidBody;
				public Constraint Constraint;

				public MeshInSpace MeshInSpace;

				public ESet<RigidBody> LastContactBodies;
				public double LastGroundTime;

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
		}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//if( member is Metadata.Property )
			//{
			//	switch( member.Name )
			//	{
			//	case nameof( RunForwardMaxSpeed ):
			//		if( !RunSupport )
			//			skip = true;
			//		break;
			//	}
			//}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
			{
				CreateDynamicData();

				//!!!!что еще восстанавливать помимо скорости
				//if( mainBody != null )
				//	mainBody.LinearVelocity = linearVelocityForSerialization;
			}
			else
				DestroyDynamicData();
		}

		public void SetTransform( Transform value )
		{
			RigidBody body;
			if( dynamicData != null && dynamicData.MainBody != null )
				body = dynamicData.MainBody;
			else
				body = GetComponent<RigidBody>( "Collision Body" );

			if( body != null && !body.Transform.ReferenceSpecified )
				body.Transform = value;

			if( !Transform.ReferenceSpecified )
				Transform = value;

			CreateDynamicData();
		}

		//!!!!
		//public double GetScaleFactor()
		//{
		//	//!!!!cache
		//	var result = TransformV.Scale.MaxComponent();
		//	//var result = GetTransform().Scale.MaxComponent();
		//	if( result == 0 )
		//		result = 0.0001;
		//	return result;
		//}

		///////////////////////////////////////////

		public bool IsOnGround()
		{
			if( dynamicData != null )
			{
				foreach( var wheel in dynamicData.Wheels )
				{
					if( EngineApp.EngineTime < wheel.LastGroundTime + 0.5 )
						return true;
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

		//!!!!
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

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//update
			if( dynamicData != null && dynamicData.MainBody != null && dynamicData.Wheels != null )
			{
				var tr = TransformV;
				var mainBody = dynamicData.MainBody;

				var throttle = Throttle.Value;
				var brake = Brake.Value;
				var steering = Steering.Value;

				//!!!!still need?
				//!!!!engine bug fix. motor doesn't wake up rigid body
				if( throttle != 0 || /*brake != 0 || */steering != 0 )
					dynamicData.MainBody.Activate();

				for( int nWheel = 0; nWheel < dynamicData.Wheels.Length; nWheel++ )
				//foreach( var wheel in dynamicData.Wheels )
				{
					var wheel = dynamicData.Wheels[ nWheel ];

					//SimplePhysics mode
					if( dynamicData.SimulationMode == SimulationModeEnum.RealPhysics )
					{
						var c = wheel.Constraint;
						if( c != null )
						{
							//!!!!impl
							////suspension
							//if( c.InternalConstraintRigid != null )
							//{
							//	//!!!!

							//	var offset = c.InternalConstraintRigid.GetRelativePivotPosition( 2 );

							//	var suspensionTravel = SuspensionTravel.Value;

							//	//!!!!
							//	//if( nWheel == 0 )
							//	{
							//		var travelCenter = suspensionTravel.GetCenter();
							//		var diff = offset - travelCenter;

							//		//if( offset > 0 && suspensionTravel.Maximum > 0 )
							//		if( diff > 0 && suspensionTravel.Maximum != suspensionTravel.Minimum )
							//		{
							//			var factor = diff / ( suspensionTravel.Maximum - suspensionTravel.Minimum );

							//			//apply to chassis
							//			{
							//				//var offsetFactor = offset / suspensionTravel.Maximum;

							//				//!!!!
							//				//Log.Info( offsetFactor.ToString() );


							//				//!!!!в 4 раза мньше нужно


							//				var force = 10.0 * factor * mainBody.Mass.Value;
							//				//var force = 7000.0 * factor;

							//				//var force = 5000.0 * offsetFactor;

							//				var forceVector = tr.Rotation.GetUp();

							//				mainBody.ApplyForce( forceVector * force, wheel.LocalPosition );
							//			}

							//			//apply to wheel
							//			if( wheel.RigidBody != null )
							//			{
							//				//!!!!
							//				var force = 2.0 * factor * wheel.RigidBody.Mass.Value;

							//				var forceVector = -tr.Rotation.GetUp();

							//				wheel.RigidBody.ApplyForce( forceVector * force, Vector3.Zero );
							//			}

							//		}
							//	}

							//	//Log.Info( offset.ToString() );


							//	//!!!!
							//	//SpringRate

							//	//if( offset > 0 )
							//	//	c.LinearAxisZMotorMaxForce = SpringRate * -offset * 300;
							//	//else
							//	//	c.LinearAxisZMotorMaxForce = 0;

							//	//c.LinearAxisZMotorTargetVelocity = 0.5;

							//	//c.LinearAxisZMotorMaxForce = SpringRate * -offset * 200;
							//	//c.LinearAxisZMotorTargetVelocity = 0.5;
							//}

							//throttle, brake
							if( brake != 0 )
							{
								c.AngularAxisXMotorTargetVelocity = 0;
								c.AngularAxisXMotorMaxForce = BrakeForce * brake;
							}
							else
							{
								if( wheel.WheelDrive )
								{
									//!!!!
									c.AngularAxisXMotorTargetVelocity = -ThrottleTargetVelocity * throttle;
									//c.AngularAxisXMotorTargetVelocity = ThrottleTargetVelocity * throttle;
									//!!!!
									c.AngularAxisXMotorMaxForce = throttle != 0 ? ThrottleForce : 0;
								}
								else
								{
									c.AngularAxisXMotorTargetVelocity = 0;
									c.AngularAxisXMotorMaxForce = 0;
								}
							}

							//steering
							if( wheel.Front )
							{
								c.AngularAxisZServoTarget = c.AngularAxisZLimitHigh.Value * steering;
								c.AngularAxisZMotorTargetVelocity = 1;
								c.AngularAxisZMotorMaxForce = SteeringForce;
							}
						}
					}

					////Raycast mode
					//if( dynamicData.SimulationMode == SimulationModeEnum.Raycast )
					//{
					//	var vehicle = dynamicData.raycastVehicle;
					//	if( vehicle != null )
					//	{
					//		var wheelInfo = vehicle.GetWheelInfo( nWheel );

					//		if( wheel.WheelDrive )
					//		{
					//			//!!!!не так, не только это

					//			wheelInfo.EngineForce = ThrottleForce * 100;

					//		}

					//		//!!!!
					//		//wheelInfo.Brake = zzzzz;

					//		//EngineForce *= ( 1.0f - timeStep );

					//		//vehicle.ApplyEngineForce( EngineForce, 2 );
					//		//vehicle.SetBrake( BreakingForce, 2 );
					//		//vehicle.SetSteeringValue( VehicleSteering, 0 );

					//		if( wheel.Front )
					//		{
					//			//!!!!
					//			//wheelInfo.Steering = zzzzz;
					//		}

					//	}
					//}

					//update LastContactsBodies
					wheel.LastContactBodies?.Clear();
					var contacts = wheel.RigidBody?.ContactsData;
					if( contacts != null )
					{
						for( int n = 0; n < contacts.Count; n++ )
						{
							ref var contact = ref contacts.Data[ n ];

							var bodyB = contact.BodyB;

							if( bodyB == null )
								continue;
							if( bodyB == dynamicData.MainBody )
								continue;
							//!!!!only static?
							if( bodyB.MotionType.Value != RigidBody.MotionTypeEnum.Static )
								continue;

							if( wheel.LastContactBodies == null )
								wheel.LastContactBodies = new ESet<RigidBody>();
							wheel.LastContactBodies.AddWithCheckAlreadyContained( bodyB );
						}
					}

					if( wheel.LastContactBodies != null && wheel.LastContactBodies.Count != 0 )
						wheel.LastGroundTime = EngineApp.EngineTime;
				}
			}

			CalculateGroundRelativeVelocity();

			var trPosition = TransformV.Position;
			//var trPosition = GetTransform().Position;
			if( lastTransformPosition.HasValue )
				lastLinearVelocity = ( trPosition - lastTransformPosition.Value ) / Time.SimulationDelta;
			else
				lastLinearVelocity = Vector3.Zero;
			lastTransformPosition = trPosition;


			////!!!!temp
			//Log.Info( "speed: " + IsOnGround().ToString() + " " + GroundRelativeVelocitySmooth.ToVector2().Length().ToString() );
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( needRecreateDynamicData )
				CreateDynamicData();

			//!!!!
			//UpdateEnabledItemTransform( delta );

			//!!!!
			//touch Transform to update vehicle AABB
			if( EnabledInHierarchy && VisibleInHierarchy )
			{
				var t = Transform;
			}
		}

		protected override void OnTransformChanged()
		{
			base.OnTransformChanged();

			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
				NeedRecreateDynamicData();
		}

		protected override void OnSpaceBoundsUpdate( ref SpaceBounds newBounds )
		{
			base.OnSpaceBoundsUpdate( ref newBounds );

			GetBox( out var box );
			box.ToBounds( out var bounds );
			newBounds = SpaceBounds.Merge( newBounds, new SpaceBounds( bounds ) );
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
			if( dynamicData?.MainBody != null )
			{
				groundRelativeVelocity = GetLinearVelocity();
				//!!!!moving ground
				//if( groundBody != null && groundBody.AngularVelocity.Value.LengthSquared() < .3f )
				//	groundRelativeVelocity -= groundBody.LinearVelocity;
			}
			else
				groundRelativeVelocity = Vector3.Zero;

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
			return lastLinearVelocity;
			//if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
			//	return ( GetTransform().Position - lastSimulationStepPosition ) / Time.SimulationDelta;
			//return Vector3.Zero;
		}

		void GetBox( out Box box )
		{
			var tr = TransformV;
			tr.Rotation.ToMatrix3( out var rot );

			var meshInSpace = GetComponent<MeshInSpace>( "Mesh In Space" );
			if( meshInSpace?.MeshOutput?.Result != null )
			{
				var bounds = meshInSpace.MeshOutput.Result.SpaceBounds.CalculatedBoundingBox;

				//!!!!scale support

				//add wheels
				if( dynamicData != null && dynamicData.Wheels != null )
				{
					foreach( var wheel in dynamicData.Wheels )
					{
						var offsetZ = wheel.LocalPosition.Z - wheel.Diameter * 0.5;
						bounds.Minimum.Z = Math.Min( bounds.Minimum.Z, offsetZ );
					}
				}

				box = new Box( bounds, tr.Position, rot );
			}
			else
				box = new Box( tr.Position, new Vector3( tr.Scale.X, tr.Scale.Y, tr.Scale.Z ) * 0.5, rot );
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

		protected override void OnGetRenderSceneData( ViewportRenderingContext context, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
		{
			base.OnGetRenderSceneData( context, mode, modeGetObjectsItem );

			if( mode == GetRenderSceneDataMode.InsideFrustum )
			{
				var context2 = context.ObjectInSpaceRenderingContext;
				var scene = context.Owner.AttachedScene;

				if( scene != null && scene.GetDisplayDevelopmentDataInThisApplication() && DebugVisualization && dynamicData != null )//!!!! scene.DisplayPhysicalObjects )
				{
					var renderer = context.Owner.Simple3DRenderer;
					var tr = TransformV;

					//var vehicle = dynamicData.raycastVehicle;

					//wheels
					if( dynamicData.Wheels != null )
					{
						//foreach( var wheel in dynamicData.Wheels )
						for( int nWheel = 0; nWheel < dynamicData.Wheels.Length; nWheel++ )
						{
							var wheel = dynamicData.Wheels[ nWheel ];

							{
								var color = new ColorValue( 1, 0, 0 );
								renderer.SetColor( color, color * ProjectSettings.Get.General.HiddenByOtherObjectsColorMultiplier );

								//!!!!scale

								var pos1 = tr * ( wheel.LocalPosition + new Vector3( 0, -wheel.Width * 0.5, 0 ) );
								var pos2 = tr * ( wheel.LocalPosition + new Vector3( 0, wheel.Width * 0.5, 0 ) );
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
					}

					//seats
					foreach( var seat in GetComponents<VehicleCharacterSeat>() )
						seat.DebugRender( context );

					//!!!!

					//var tr = GetTransform();
					//var scaleFactor = tr.Scale.MaxComponent();

					//renderer.SetColor( new ColorValue( 1, 0, 0, 1 ) );

					////eye position
					//renderer.SetColor( new ColorValue( 0, 1, 0, 1 ) );
					//renderer.AddSphere( new Sphere( TransformV * EyePosition.Value, .05f ), 16 );
				}

				//!!!!
				var showLabels = /*show &&*/ dynamicData == null;
				if( !showLabels )
					context2.disableShowingLabelForThisObject = true;

				//draw selection
				if( context2.selectedObjects.Contains( this ) || context2.canSelectObjects.Contains( this ) )
				{
					ColorValue color;
					if( context2.selectedObjects.Contains( this ) )
						color = ProjectSettings.Get.General.SelectedColor;
					else //if( context2.canSelectObjects.Contains( this ) )
						color = ProjectSettings.Get.General.CanSelectColor;
					//else
					//	color = ProjectSettings.Get.SceneShowPhysicsDynamicActiveColor;

					var viewport = context.Owner;

					viewport.Simple3DRenderer.SetColor( color, color * ProjectSettings.Get.General.HiddenByOtherObjectsColorMultiplier );
					DebugDraw( viewport );
				}
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
			var seat = GetComponent<VehicleCharacterSeat>();
			if( seat != null )
			{
				var seatTransform = seat.Transform.Value;
				position = vehicleTransform * ( seatTransform * seat.EyeOffset.Value );

				//var seatTransformWorld = tr * seatTransform;
				//position = seatTransformWorld.Position;
			}

			//}
		}

		//public Vector3 GetSmoothPosition()
		//{
		//	return TransformV.Position + GetSmoothCameraOffset();
		//}

		public void SoundPlay( Sound sound )
		{
			ParentScene?.SoundPlay( sound, TransformV.Position );
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			if( Components.Count == 0 )
			{
				//create rigid body
				var body = CreateComponent<RigidBody>();
				body.Name = "Collision Body";
				body.CanBeSelected = false;
				body.MotionType = RigidBody.MotionTypeEnum.Dynamic;
				body.Mass = 1000;

				//create collision shape
				var collisionShape = body.CreateComponent<CollisionShape_Mesh>();
				collisionShape.Name = "Collision Shape";
				collisionShape.Vertices = ReferenceUtility.MakeReference( @"Content\Vehicles\Default\Body\scene.gltf|$Mesh\$Collision Definition\$\Vertices" );
				collisionShape.Indices = ReferenceUtility.MakeReference( @"Content\Vehicles\Default\Body\scene.gltf|$Mesh\$Collision Definition\$\Indices" );
				collisionShape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.Convex;

				//reference tranform of the vehicle to the body
				Transform = ReferenceUtility.MakeThisReference( this, body, "Transform" );

				//create mesh in space
				var meshInSpace = CreateComponent<MeshInSpace>();
				meshInSpace.Name = "Mesh In Space";
				meshInSpace.CanBeSelected = false;
				meshInSpace.Mesh = new Reference<Mesh>( null, @"Content\Vehicles\Default\Body\scene.gltf|$Mesh" );
				meshInSpace.Transform = new Reference<Transform>( NeoAxis.Transform.Identity, "this:..\\Transform" );
				//meshInSpace.Transform = new Reference<Transform>( NeoAxis.Transform.Identity, "this:$Transform Offset\\Result" );

				//var transformOffset = meshInSpace.CreateComponent<TransformOffset>();
				//transformOffset.Name = "Transform Offset";
				//transformOffset.PositionOffset = new Vector3( 0, 0, 0 );
				////transformOffset.PositionOffset = new Vector3( 0, 0, -1.15 );
				//transformOffset.Source = new Reference<Transform>( NeoAxis.Transform.Identity, "this:..\\..\\Transform" );
				////transformOffset.Source = new Reference<Transform>( NeoAxis.Transform.Identity, "this:..\\..\\$Collision Body\\Transform" );

				var inputProcessing = CreateComponent<VehicleInputProcessing>();
				inputProcessing.Name = "Vehicle Input Processing";

				var seat = CreateComponent<VehicleCharacterSeat>();
				seat.Name = "Vehicle Character Seat";
				seat.Transform = new Transform( new Vector3( 0.3, 0.5, 0 ), Quaternion.Identity );
				seat.ExitTransform = new Transform( new Vector3( 0.3, 1.8, 0.3 ), Quaternion.Identity );

				var ai = CreateComponent<VehicleAI>();
				ai.Name = "Vehicle AI";
			}
		}

		public void NeedRecreateDynamicData()
		{
			needRecreateDynamicData = true;
		}

		public void CreateDynamicData()
		{
			needRecreateDynamicData = false;

			DestroyDynamicData();

			if( !EnabledInHierarchyAndIsNotResource )
				return;

			const bool displayInEditor = false;// true;

			var scene = ParentScene;
			if( scene == null )
				return;

			var tr = TransformV;

			dynamicData = new DynamicData();
			dynamicData.SimulationMode = SimulationMode;
			dynamicData.MainBody = GetComponent<RigidBody>( "Collision Body" );

			//VehicleTuning vehicleTuning = null;

			//if( dynamicData.SimulationMode == SimulationModeEnum.Raycast )
			//{
			//	vehicleTuning = new VehicleTuning();
			//	//!!!!

			//	var vehicleRayCaster = new DefaultVehicleRaycaster( scene.PhysicsWorldData.world );

			//	//!!!!tuning внутри не используется
			//	dynamicData.raycastVehicle = new RaycastVehicle( vehicleTuning, dynamicData.MainBody.InternalRigidBody, vehicleRayCaster );
			//	var vehicle = dynamicData.raycastVehicle;

			//	//!!!!

			//	//var chassisShape = new BoxShape( 1.0f, 0.5f, 2.0f );

			//	//var compound = new CompoundShape();

			//	////localTrans effectively shifts the center of mass with respect to the chassis
			//	//Matrix localTrans = Matrix.Translation( Vector3.UnitY );
			//	//compound.AddChildShape( localTrans, chassisShape );
			//	//RigidBody carChassis = PhysicsHelper.CreateBody( 800, Matrix.Identity, compound, World );
			//	//carChassis.UserObject = "Chassis";
			//	////carChassis.SetDamping(0.2f, 0.2f);


			//	//!!!!
			//	//carChassis.ActivationState = ActivationState.DisableDeactivation;


			//	scene.PhysicsWorldData.world.AddAction( vehicle );

			//	// choose coordinate system
			//	vehicle.SetCoordinateSystem( 1, 2, 0 );
			//	//vehicle.SetCoordinateSystem( rightIndex, upIndex, forwardIndex );


			//	//vehicle.RigidBody.WorldTransform = transform;
			//}

			//create wheels

			dynamicData.Wheels = new DynamicData.WheelData[ 4 ];

			var wheelDrive = WheelDrive.Value;
			var front = wheelDrive == WheelDriveEnum.Front || wheelDrive == WheelDriveEnum.All;
			var rear = wheelDrive == WheelDriveEnum.Rear || wheelDrive == WheelDriveEnum.All;

			for( int n = 0; n < dynamicData.Wheels.Length; n++ )
			{
				//init basic data

				var wheel = new DynamicData.WheelData();
				dynamicData.Wheels[ n ] = wheel;
				wheel.Which = (DynamicData.WhichWheel)n;

				if( ( wheel.Which == DynamicData.WhichWheel.FrontLeft || wheel.Which == DynamicData.WhichWheel.FrontRight ) && front )
					wheel.WheelDrive = true;
				if( ( wheel.Which == DynamicData.WhichWheel.RearLeft || wheel.Which == DynamicData.WhichWheel.RearRight ) && rear )
					wheel.WheelDrive = true;

				switch( wheel.Which )
				{
				case DynamicData.WhichWheel.FrontLeft:
					wheel.LocalPosition = FrontWheelPosition.Value;
					if( wheel.LocalPosition.Y < 0 )
						wheel.LocalPosition.Y = -wheel.LocalPosition.Y;
					break;

				case DynamicData.WhichWheel.FrontRight:
					wheel.LocalPosition = FrontWheelPosition.Value;
					if( wheel.LocalPosition.Y > 0 )
						wheel.LocalPosition.Y = -wheel.LocalPosition.Y;
					break;

				case DynamicData.WhichWheel.RearLeft:
					wheel.LocalPosition = RearWheelPosition.Value;
					if( wheel.LocalPosition.Y < 0 )
						wheel.LocalPosition.Y = -wheel.LocalPosition.Y;
					break;

				case DynamicData.WhichWheel.RearRight:
					wheel.LocalPosition = RearWheelPosition.Value;
					if( wheel.LocalPosition.Y > 0 )
						wheel.LocalPosition.Y = -wheel.LocalPosition.Y;
					break;
				}

				switch( wheel.Which )
				{
				case DynamicData.WhichWheel.FrontLeft:
				case DynamicData.WhichWheel.FrontRight:
					wheel.Diameter = FrontWheelDiameter;
					wheel.Width = FrontWheelWidth;
					break;

				case DynamicData.WhichWheel.RearLeft:
				case DynamicData.WhichWheel.RearRight:
					wheel.Diameter = RearWheelDiameter;
					wheel.Width = RearWheelWidth;
					break;
				}

				//init physics

				if( dynamicData.MainBody != null )
				{
					//SimplePhysics mode
					if( dynamicData.SimulationMode == SimulationModeEnum.RealPhysics )
					{
						//create a rigid body of wheel
						{
							//need set ShowInEditor = false before AddComponent
							var body = ComponentUtility.CreateComponent<RigidBody>( null, false, false );
							body.DisplayInEditor = displayInEditor;
							AddComponent( body );
							//var body = CreateComponent<RigidBody>( enabled: false );

							body.CanBeSelected = false;
							body.SaveSupport = false;
							body.CloneSupport = false;

							body.Name = "Wheel " + wheel.Which.ToString() + " Collision Body";
							body.MotionType = RigidBody.MotionTypeEnum.Dynamic;
							body.Mass = wheel.Front ? FrontWheelMass : RearWheelMass;

							body.MaterialFriction = WheelFriction;
							//!!!!
							//body.MaterialFrictionMode = PhysicalMaterial.FrictionModeEnum.AnisotropicRolling;
							//MaterialAnisotropicFriction
							//MaterialSpinningFriction
							//MaterialRollingFriction
							//body.MaterialRestitution

							body.Transform = tr * new Transform( wheel.LocalPosition, Quaternion.Identity );


							//!!!!instancing geometry
							//!!!!round
							//!!!!more segments

							SimpleMeshGenerator.GenerateCylinder( 1, wheel.Diameter / 2, wheel.Width, 64, true, true, true, out Vector3F[] vertices, out var indices );

							var shape = body.CreateComponent<CollisionShape_Mesh>();
							shape.Vertices = vertices;
							shape.Indices = indices;
							shape.ShapeType = CollisionShape_Mesh.ShapeTypeEnum.Convex;

							//var shape = body.CreateComponent<CollisionShape_Sphere>();
							//shape.Radius = wheel.Diameter / 2;

							body.ContactsCollect = true;

							body.Enabled = true;

							wheel.RigidBody = body;
						}

						//create constraint
						if( wheel.RigidBody != null )
						{
							//need set ShowInEditor = false before AddComponent
							var c = ComponentUtility.CreateComponent<Constraint>( null, false, false );
							c.DisplayInEditor = displayInEditor;
							AddComponent( c );
							//var body = CreateComponent<RigidBody>( enabled: false );

							c.CanBeSelected = false;
							c.SaveSupport = false;
							c.CloneSupport = false;

							c.Name = "Wheel " + wheel.Which.ToString() + " Constraint";
							c.CollisionsBetweenLinkedBodies = false;
							c.BodyA = ReferenceUtility.MakeThisReference( c, dynamicData.MainBody );
							c.BodyB = ReferenceUtility.MakeThisReference( c, wheel.RigidBody );

							c.Transform = new Transform( wheel.RigidBody.TransformV.Position, tr.Rotation * Quaternion.FromRotateByZ( -Math.PI / 2 ) );
							//c.Transform = new Transform( wheel.RigidBody.TransformV.Position, tr.Rotation * Quaternion.FromRotateByZ( Math.PI / 2 ) );

							c.AngularAxisX = PhysicsAxisMode.Free;
							c.AngularAxisXMotor = true;

							//steering
							if( wheel.Which == DynamicData.WhichWheel.FrontLeft || wheel.Which == DynamicData.WhichWheel.FrontRight )
							{
								c.AngularAxisZ = PhysicsAxisMode.Limited;
								c.AngularAxisZLimitLow = -MaxSteeringAngle.Value;
								c.AngularAxisZLimitHigh = MaxSteeringAngle.Value;
								c.AngularAxisZMotor = true;
								c.AngularAxisZServo = true;
							}

							//!!!!impl
							////suspension
							//c.LinearAxisZ = PhysicsAxisMode.Limited;
							//c.LinearAxisZLimitHigh = SuspensionTravel.Value.Maximum;
							//c.LinearAxisZLimitLow = SuspensionTravel.Value.Minimum;

							////!!!!
							//c.LinearAxisZLimitHigh = 0;
							//c.LinearAxisZLimitLow = 0;

							//c.LinearAxisZLimitHigh = -0.2;// 0.1;
							//c.LinearAxisZLimitLow = 0.1;

							//c.LinearAxisZMotor = true;
							//c.LinearAxisZMotorMaxForce = 0;
							//c.LinearAxisZMotorTargetVelocity = 0;

							//c.LinearAxisZSpring = true;
							//c.LinearAxisZSpringStiffness = 1000000000;
							//c.LinearAxisZSpringDamping = 0;

							//!!!!засыпать
							//c.AutoActivateBodies = true;

							c.Enabled = true;

							wheel.Constraint = c;
						}
					}

					////Raycast mode
					//if( dynamicData.SimulationMode == SimulationModeEnum.Raycast )
					//{
					//	var vehicle = dynamicData.raycastVehicle;

					//	//!!!!
					//	float suspensionRestLength = 0.6f;

					//	//!!!!
					//	//const float connectionHeight = 1.2f;

					//	//!!!!
					//	//float wheelRadius = 0.7f;
					//	//float wheelWidth = 0.4f;

					//	var connectionPoint = BulletPhysicsUtility.Convert( wheel.LocalPosition );
					//	//var connectionPoint = BulletPhysicsUtility.Convert( new Vector3( 1.0 - ( 0.3f * wheelWidth ), connectionHeight, 2 * 1.0 - wheelRadius ) );

					//	//!!!!
					//	var wheelDirection = BulletPhysicsUtility.Convert( new Vector3( 0, 0, -1 ) );
					//	//var wheelAxle = BulletPhysicsUtility.Convert( new Vector3( 1, 0, 0 ) );
					//	var wheelAxle = BulletPhysicsUtility.Convert( new Vector3( 0, -1, 0 ) );

					//	var wheelInfo = vehicle.AddWheel( connectionPoint, wheelDirection, wheelAxle, suspensionRestLength, wheel.Diameter / 2, vehicleTuning, wheel.Front );


					//	//Vector3 wheelDirection = Vector3.Zero;
					//	//Vector3 wheelAxle = Vector3.Zero;

					//	//wheelDirection[ upIndex ] = -1;
					//	//wheelAxle[ rightIndex ] = -1;


					//	//bool isFrontWheel = true;
					//	//var connectionPoint = new Vector3( CUBE_HALF_EXTENTS - ( 0.3f * wheelWidth ), connectionHeight, 2 * CUBE_HALF_EXTENTS - wheelRadius );
					//	//vehicle.AddWheel( connectionPoint, wheelDirection, wheelAxle, suspensionRestLength, wheelRadius, tuning, isFrontWheel );

					//	//connectionPoint = new Vector3( -CUBE_HALF_EXTENTS + ( 0.3f * wheelWidth ), connectionHeight, 2 * CUBE_HALF_EXTENTS - wheelRadius );
					//	//vehicle.AddWheel( connectionPoint, wheelDirection, wheelAxle, suspensionRestLength, wheelRadius, tuning, isFrontWheel );

					//	//isFrontWheel = false;
					//	//connectionPoint = new Vector3( -CUBE_HALF_EXTENTS + ( 0.3f * wheelWidth ), connectionHeight, -2 * CUBE_HALF_EXTENTS + wheelRadius );
					//	//vehicle.AddWheel( connectionPoint, wheelDirection, wheelAxle, suspensionRestLength, wheelRadius, tuning, isFrontWheel );

					//	//connectionPoint = new Vector3( CUBE_HALF_EXTENTS - ( 0.3f * wheelWidth ), connectionHeight, -2 * CUBE_HALF_EXTENTS + wheelRadius );
					//	//vehicle.AddWheel( connectionPoint, wheelDirection, wheelAxle, suspensionRestLength, wheelRadius, tuning, isFrontWheel );


					//	//!!!!
					//	float wheelFriction = 1000; //float.MaxValue
					//	float suspensionStiffness = 20.0f;
					//	float suspensionDamping = 2.3f;
					//	float suspensionCompression = 4.4f;
					//	float rollInfluence = 0.1f; //1.0f;


					//	//for( int i = 0; i < vehicle.NumWheels; i++ )
					//	//{
					//	//	WheelInfo wheelInfo = vehicle.GetWheelInfo( i );
					//	wheelInfo.SuspensionStiffness = suspensionStiffness;
					//	wheelInfo.WheelsDampingRelaxation = suspensionDamping;
					//	wheelInfo.WheelsDampingCompression = suspensionCompression;
					//	wheelInfo.FrictionSlip = wheelFriction;
					//	wheelInfo.RollInfluence = rollInfluence;
					//	//}


					//}
				}

				//create mesh in space
				{
					//need set ShowInEditor = false before AddComponent
					var meshInSpace = ComponentUtility.CreateComponent<MeshInSpace>( null, false, false );
					meshInSpace.DisplayInEditor = displayInEditor;
					AddComponent( meshInSpace, -1 );
					//var meshInSpace = CreateComponent<MeshInSpace>( enabled: false );

					meshInSpace.CanBeSelected = false;
					meshInSpace.SaveSupport = false;
					meshInSpace.CloneSupport = false;

					meshInSpace.Name = "Wheel " + wheel.Which.ToString() + " Mesh In Space";
					meshInSpace.Mesh = wheel.Front ? FrontWheelMesh : RearWheelMesh;

					//SimplePhysics mode
					if( dynamicData.SimulationMode == SimulationModeEnum.RealPhysics )
					{
						//attach to the body
						if( wheel.RigidBody != null )
						{
							if( wheel.Right )
							{
								var offset = meshInSpace.CreateComponent<TransformOffset>();
								offset.Name = "Transform Offset";
								offset.RotationOffset = Quaternion.FromRotateByZ( Math.PI );
								offset.Source = ReferenceUtility.MakeThisReference( offset, wheel.RigidBody, "Transform" );

								meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, offset, "Result" );
							}
							else
								meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, wheel.RigidBody, "Transform" );
						}
					}

					////Raycast mode
					//if( dynamicData.SimulationMode == SimulationModeEnum.Raycast )
					//{

					//	//!!!!

					//}


					//else
					//{
					//	//var offset = meshInSpace.CreateComponent<TransformOffset>();
					//	//offset.Name = "Transform Offset";
					//	//offset.PositionOffset = wheel.LocalPosition;

					//	////!!!!может тело повернуть
					//	////if(wheel.Right)
					//	////offset.RotationOffset = z;

					//	//offset.Source = ReferenceUtility.MakeThisReference( offset, this, "Transform" );

					//	////!!!!slowly maybe. where else
					//	//meshInSpace.Transform = ReferenceUtility.MakeThisReference( meshInSpace, offset, "Result" );
					//}

					meshInSpace.Enabled = true;

					wheel.MeshInSpace = meshInSpace;
				}

			}

			SpaceBoundsUpdate();
		}

		//public void UpdateDynamicData()
		//{
		//}

		public void DestroyDynamicData()
		{
			if( dynamicData != null )
			{
				if( dynamicData.Wheels != null )
				{
					foreach( var wheel in dynamicData.Wheels )
					{
						wheel.MeshInSpace?.Dispose();
						wheel.Constraint?.Dispose();
						wheel.RigidBody?.Dispose();
					}
				}

				//if( dynamicData.raycastVehicle != null )
				//{
				//	var scene = ParentScene;
				//	if( scene?.PhysicsWorldData?.world != null )
				//		scene.PhysicsWorldData.world.RemoveAction( dynamicData.raycastVehicle );
				//}

				dynamicData = null;
			}
		}

		///////////////////////////////////////////////

		public void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
			//take by a character
			var character = gameMode.ObjectControlledByPlayer.Value as Character;
			if( character != null )
			{
				var seat = GetComponent<VehicleCharacterSeat>();
				if( seat != null && !seat.Character.ReferenceOrValueSpecified )
				{
					info = new InteractiveObjectObjectInfo();
					info.AllowInteract = true;
					info.SelectionTextInfo.Add( Name );
					info.SelectionTextInfo.Add( "Press E to control the vehicle." );
				}
			}
		}

		public virtual bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message )
		{
			var keyDown = message as InputMessageKeyDown;
			if( keyDown != null )
			{
				if( keyDown.Key == EKeys.E )
				{
					//start control by a character
					var character = gameMode.ObjectControlledByPlayer.Value as Character;
					if( character != null )
					{
						var seat = GetComponent<VehicleCharacterSeat>();
						if( seat != null && !seat.Character.ReferenceOrValueSpecified )
						{
							seat.PutCharacterToSeat( character );
							gameMode.ObjectControlledByPlayer = ReferenceUtility.MakeRootReference( this );

							GameMode.PlayScreen?.ParentContainer?.Viewport?.NotifyInstantCameraMovement();

							return true;
						}
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
	}
}
