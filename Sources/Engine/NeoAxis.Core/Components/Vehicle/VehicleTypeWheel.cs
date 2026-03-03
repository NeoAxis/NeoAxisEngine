// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// A definition of the wheel for the vehicle type.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Vehicle\Vehicle Type Wheel", 22001 )]
	[NewObjectDefaultName( "Wheel" )]
	public class VehicleTypeWheel : Component
	{
		/// <summary>
		/// The position offset for the wheel.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Basic" )]
		public Reference<Vector3> Position
		{
			get { if( _position.BeginGet() ) Position = _position.Get( this ); return _position.value; }
			set { if( _position.BeginSet( this, ref value ) ) { try { PositionChanged?.Invoke( this ); DataWasChanged(); } finally { _position.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Position"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> PositionChanged;
		ReferenceField<Vector3> _position;

		/// <summary>
		/// The diameter of the wheel.
		/// </summary>
		[DefaultValue( 0.665 )]
		[Category( "Basic" )]
		public Reference<double> Diameter
		{
			get { if( _diameter.BeginGet() ) Diameter = _diameter.Get( this ); return _diameter.value; }
			set { if( _diameter.BeginSet( this, ref value ) ) { try { DiameterChanged?.Invoke( this ); DataWasChanged(); } finally { _diameter.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Diameter"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> DiameterChanged;
		ReferenceField<double> _diameter = 0.665;

		/// <summary>
		/// The width of the wheel.
		/// </summary>
		[DefaultValue( 0.25 )]
		[Category( "Basic" )]
		public Reference<double> Width
		{
			get { if( _width.BeginGet() ) Width = _width.Get( this ); return _width.value; }
			set { if( _width.BeginSet( this, ref value ) ) { try { WidthChanged?.Invoke( this ); DataWasChanged(); } finally { _width.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Width"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> WidthChanged;
		ReferenceField<double> _width = 0.25;

		/// <summary>
		/// The amount of wheels on one side.
		/// </summary>
		[DefaultValue( 1 )]
		[Range( 1, 10 )]
		[Category( "Basic" )]
		public Reference<int> Count
		{
			get { if( _count.BeginGet() ) Count = _count.Get( this ); return _count.value; }
			set { if( _count.BeginSet( this, ref value ) ) { try { CountChanged?.Invoke( this ); DataWasChanged(); } finally { _count.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Count"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> CountChanged;
		ReferenceField<int> _count = 1;

		/// <summary>
		/// The distance between wheels on one side.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Basic" )]
		public Reference<double> Distance
		{
			get { if( _distance.BeginGet() ) Distance = _distance.Get( this ); return _distance.value; }
			set { if( _distance.BeginSet( this, ref value ) ) { try { DistanceChanged?.Invoke( this ); DataWasChanged(); } finally { _distance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Distance"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> DistanceChanged;
		ReferenceField<double> _distance = 1.0;

		/// <summary>
		/// Whether to create two wheels, instead of one. Use it to make three-wheellers, airplanes.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Basic" )]
		public Reference<bool> Pair
		{
			get { if( _pair.BeginGet() ) Pair = _pair.Get( this ); return _pair.value; }
			set { if( _pair.BeginSet( this, ref value ) ) { try { PairChanged?.Invoke( this ); DataWasChanged(); } finally { _pair.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Pair"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> PairChanged;
		ReferenceField<bool> _pair = true;


		const string meshDefault = @"Content\Vehicles\Default\Wheel.gltf|$Mesh";

		/// <summary>
		/// The mesh of the wheel.
		/// </summary>
		[DefaultValueReference( meshDefault )]
		[Category( "Visual" )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( this, ref value ) ) { try { MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> MeshChanged;
		ReferenceField<Mesh> _mesh = new Reference<Mesh>( null, meshDefault );

		/// <summary>
		/// The mass of the wheel.
		/// </summary>
		[DefaultValue( 18.0 )]
		[Category( "Physics" )]
		public Reference<double> Mass
		{
			get { if( _mass.BeginGet() ) Mass = _mass.Get( this ); return _mass.value; }
			set { if( _mass.BeginSet( this, ref value ) ) { try { MassChanged?.Invoke( this ); DataWasChanged(); } finally { _mass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mass"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> MassChanged;
		ReferenceField<double> _mass = 18.0;

		/// <summary>
		/// How long the suspension is in max raised position relative to the attachment point (m).
		/// </summary>
		[DefaultValue( 0.25 )]//0.3 )]
		[Category( "Physics" )]//[Category( "Suspension" )]
		public Reference<double> SuspensionMinLength
		{
			get { if( _suspensionMinLength.BeginGet() ) SuspensionMinLength = _suspensionMinLength.Get( this ); return _suspensionMinLength.value; }
			set { if( _suspensionMinLength.BeginSet( this, ref value ) ) { try { SuspensionMinLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _suspensionMinLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SuspensionMinLength"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> SuspensionMinLengthChanged;
		ReferenceField<double> _suspensionMinLength = 0.25;//0.3;

		/// <summary>
		/// How long the suspension is in max droop position relative to the attachment point (m).
		/// </summary>
		[DefaultValue( 0.45 )]//0.5 )]
		[Category( "Physics" )]//[Category( "Suspension" )]
		public Reference<double> SuspensionMaxLength
		{
			get { if( _suspensionMaxLength.BeginGet() ) SuspensionMaxLength = _suspensionMaxLength.Get( this ); return _suspensionMaxLength.value; }
			set { if( _suspensionMaxLength.BeginSet( this, ref value ) ) { try { SuspensionMaxLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _suspensionMaxLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SuspensionMaxLength"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> SuspensionMaxLengthChanged;
		ReferenceField<double> _suspensionMaxLength = 0.45;//0.5;

		/// <summary>
		/// The natural length (m) of the suspension spring is defined as SuspensionMaxLength + SuspensionPreloadLength.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Category( "Physics" )]//[Category( "Suspension" )]
		public Reference<double> SuspensionPreloadLength
		{
			get { if( _suspensionPreloadLength.BeginGet() ) SuspensionPreloadLength = _suspensionPreloadLength.Get( this ); return _suspensionPreloadLength.value; }
			set { if( _suspensionPreloadLength.BeginSet( this, ref value ) ) { try { SuspensionPreloadLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _suspensionPreloadLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SuspensionPreloadLength"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> SuspensionPreloadLengthChanged;
		ReferenceField<double> _suspensionPreloadLength = 0.0;

		[DefaultValue( 1.5 )]
		[Category( "Physics" )]//[Category( "Suspension" )]
		public Reference<double> SuspensionFrequency
		{
			get { if( _suspensionFrequency.BeginGet() ) SuspensionFrequency = _suspensionFrequency.Get( this ); return _suspensionFrequency.value; }
			set { if( _suspensionFrequency.BeginSet( this, ref value ) ) { try { SuspensionFrequencyChanged?.Invoke( this ); DataWasChanged(); } finally { _suspensionFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SuspensionFrequency"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> SuspensionFrequencyChanged;
		ReferenceField<double> _suspensionFrequency = 1.5;

		[DefaultValue( 0.5 )]
		[Category( "Physics" )]//[Category( "Suspension" )]
		public Reference<double> SuspensionDamping
		{
			get { if( _suspensionDamping.BeginGet() ) SuspensionDamping = _suspensionDamping.Get( this ); return _suspensionDamping.value; }
			set { if( _suspensionDamping.BeginSet( this, ref value ) ) { try { SuspensionDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _suspensionDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SuspensionDamping"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> SuspensionDampingChanged;
		ReferenceField<double> _suspensionDamping = 0.5;


		//Wheels mode

		/// <summary>
		/// Stiffness (spring constant in N/m) of front wheel anti rollbar. Can be 0 to disable the anti-rollbar.
		/// </summary>
		[DefaultValue( 1000.0 )]
		//[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Physics" )]
		public Reference<double> AntiRollBarStiffness
		{
			get { if( _antiRollBarStiffness.BeginGet() ) AntiRollBarStiffness = _antiRollBarStiffness.Get( this ); return _antiRollBarStiffness.value; }
			set { if( _antiRollBarStiffness.BeginSet( this, ref value ) ) { try { AntiRollBarStiffnessChanged?.Invoke( this ); DataWasChanged(); } finally { _antiRollBarStiffness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AntiRollBarStiffness"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> AntiRollBarStiffnessChanged;
		ReferenceField<double> _antiRollBarStiffness = 1000.0;

		/// <summary>
		/// How much torque (Nm) the brakes can apply to front wheel.
		/// </summary>
		[DefaultValue( 1500.0 )]
		[Category( "Physics" )]
		public Reference<double> MaxBrakeTorque
		{
			get { if( _maxBrakeTorque.BeginGet() ) MaxBrakeTorque = _maxBrakeTorque.Get( this ); return _maxBrakeTorque.value; }
			set { if( _maxBrakeTorque.BeginSet( this, ref value ) ) { try { MaxBrakeTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _maxBrakeTorque.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxBrakeTorque"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> MaxBrakeTorqueChanged;
		ReferenceField<double> _maxBrakeTorque = 1500.0;

		/// <summary>
		/// How much torque (Nm) the hand brake can apply to front wheel.
		/// </summary>
		[DefaultValue( 2000.0 )]
		[Category( "Physics" )]
		public Reference<double> MaxHandBrakeTorque
		{
			get { if( _maxHandBrakeTorque.BeginGet() ) MaxHandBrakeTorque = _maxHandBrakeTorque.Get( this ); return _maxHandBrakeTorque.value; }
			set { if( _maxHandBrakeTorque.BeginSet( this, ref value ) ) { try { MaxHandBrakeTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _maxHandBrakeTorque.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxHandBrakeTorque"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> MaxHandBrakeTorqueChanged;
		ReferenceField<double> _maxHandBrakeTorque = 2000.0;

		/// <summary>
		/// Angular damping factor of the wheel: dw/dt = -c * w.
		/// </summary>
		[DefaultValue( 0.2 )]
		[Category( "Physics" )]
		public Reference<double> AngularDamping
		{
			get { if( _angularDamping.BeginGet() ) AngularDamping = _angularDamping.Get( this ); return _angularDamping.value; }
			set { if( _angularDamping.BeginSet( this, ref value ) ) { try { AngularDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _angularDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngularDamping"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> AngularDampingChanged;
		ReferenceField<double> _angularDamping = 0.2;

		/// <summary>
		/// Friction in forward direction of tire as a function of the slip ratio (fraction): (omega_wheel * r_wheel - v_longitudinal) / |v_longitudinal|.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		[Category( "Physics" )]
		public ReferenceList<CurvePoint1F> LongitudinalFriction
		{
			get { return _longitudinalFriction; }
		}
		public delegate void LongitudinalFrictionChangedDelegate( VehicleTypeWheel sender );
		public event LongitudinalFrictionChangedDelegate LongitudinalFrictionChanged;
		ReferenceList<CurvePoint1F> _longitudinalFriction;

		/// <summary>
		/// Friction in sideway direction of tire as a function of the slip angle (degrees): angle between relative contact velocity and vehicle direction.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		[Category( "Physics" )]
		public ReferenceList<CurvePoint1F> LateralFriction
		{
			get { return _lateralFriction; }
		}
		public delegate void LateralFrictionChangedDelegate( VehicleTypeWheel sender );
		public event LateralFrictionChangedDelegate LateralFrictionChanged;
		ReferenceList<CurvePoint1F> _lateralFriction;

		/// <summary>
		/// The maximal steering angle of front wheels.
		/// </summary>
		[DefaultValue( 0.0 )]//45.0 )]
		[Category( "Physics" )]
		public Reference<Degree> MaxSteeringAngle
		{
			get { if( _maxSteeringAngle.BeginGet() ) MaxSteeringAngle = _maxSteeringAngle.Get( this ); return _maxSteeringAngle.value; }
			set { if( _maxSteeringAngle.BeginSet( this, ref value ) ) { try { MaxSteeringAngleChanged?.Invoke( this ); DataWasChanged(); } finally { _maxSteeringAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxSteeringAngle"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> MaxSteeringAngleChanged;
		ReferenceField<Degree> _maxSteeringAngle = new Degree( 0 );//45.0 );

		[DefaultValue( 0.5 )]
		[Category( "Physics" )]
		public Reference<double> SteeringTime
		{
			get { if( _steeringTime.BeginGet() ) SteeringTime = _steeringTime.Get( this ); return _steeringTime.value; }
			set { if( _steeringTime.BeginSet( this, ref value ) ) { try { SteeringTimeChanged?.Invoke( this ); } finally { _steeringTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SteeringTime"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> SteeringTimeChanged;
		ReferenceField<double> _steeringTime = 0.5;

		[DefaultValue( true )]
		[Category( "Physics" )]
		public Reference<bool> Drive
		{
			get { if( _drive.BeginGet() ) Drive = _drive.Get( this ); return _drive.value; }
			set { if( _drive.BeginSet( this, ref value ) ) { try { DriveChanged?.Invoke( this ); DataWasChanged(); } finally { _drive.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Drive"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> DriveChanged;
		ReferenceField<bool> _drive = true;

		/// <summary>
		/// Ratio between rotation speed of gear box and wheels.
		/// </summary>
		[DefaultValue( 3.42 )]
		[Category( "Physics" )]
		public Reference<double> DifferentialRatio
		{
			get { if( _differentialRatio.BeginGet() ) DifferentialRatio = _differentialRatio.Get( this ); return _differentialRatio.value; }
			set { if( _differentialRatio.BeginSet( this, ref value ) ) { try { DifferentialRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _differentialRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DifferentialRatio"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> DifferentialRatioChanged;
		ReferenceField<double> _differentialRatio = 3.42;

		/// <summary>
		/// Defines how the engine torque is split across the left and right wheel (0 = left, 0.5 = center, 1 = right).
		/// </summary>
		[DefaultValue( 0.5 )]
		[Category( "Physics" )]
		public Reference<double> DifferentialLeftRightSplit
		{
			get { if( _differentialLeftRightSplit.BeginGet() ) DifferentialLeftRightSplit = _differentialLeftRightSplit.Get( this ); return _differentialLeftRightSplit.value; }
			set { if( _differentialLeftRightSplit.BeginSet( this, ref value ) ) { try { DifferentialLeftRightSplitChanged?.Invoke( this ); DataWasChanged(); } finally { _differentialLeftRightSplit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DifferentialLeftRightSplit"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> DifferentialLeftRightSplitChanged;
		ReferenceField<double> _differentialLeftRightSplit = 0.5;

		/// <summary>
		/// Ratio max / min wheel speed. When this ratio is exceeded, all torque gets distributed to the slowest moving wheel. This allows implementing a limited slip differential. Set to FLT_MAX (3.402823466e+38F) for an open differential. Value should be > 1.
		/// </summary>
		[DefaultValue( 1.4 )]
		[Category( "Physics" )]
		public Reference<double> DifferentialLimitedSlipRatio
		{
			get { if( _differentialLimitedSlipRatio.BeginGet() ) DifferentialLimitedSlipRatio = _differentialLimitedSlipRatio.Get( this ); return _differentialLimitedSlipRatio.value; }
			set { if( _differentialLimitedSlipRatio.BeginSet( this, ref value ) ) { try { DifferentialLimitedSlipRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _differentialLimitedSlipRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DifferentialLimitedSlipRatio"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> DifferentialLimitedSlipRatioChanged;
		ReferenceField<double> _differentialLimitedSlipRatio = 1.4;

		/// <summary>
		/// How much of the engines torque is applied to this differential (0 = none, 1 = full).
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Physics" )]
		public Reference<double> DifferentialEngineTorqueRatio
		{
			get { if( _differentialEngineTorqueRatio.BeginGet() ) DifferentialEngineTorqueRatio = _differentialEngineTorqueRatio.Get( this ); return _differentialEngineTorqueRatio.value; }
			set { if( _differentialEngineTorqueRatio.BeginSet( this, ref value ) ) { try { DifferentialEngineTorqueRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _differentialEngineTorqueRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DifferentialEngineTorqueRatio"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> DifferentialEngineTorqueRatioChanged;
		ReferenceField<double> _differentialEngineTorqueRatio = 1.0;


		//Tracks mode

		/// <summary>
		/// Friction in forward direction of tire. For Tracks mode only.
		/// </summary>
		[DefaultValue( 4.0 )]
		[Category( "Physics" )]//[Category( "Friction" )]
		[DisplayName( "Longitudinal Friction" )]
		public Reference<double> TracksLongitudinalFriction
		{
			get { if( _tracksLongitudinalFriction.BeginGet() ) TracksLongitudinalFriction = _tracksLongitudinalFriction.Get( this ); return _tracksLongitudinalFriction.value; }
			set { if( _tracksLongitudinalFriction.BeginSet( this, ref value ) ) { try { TracksLongitudinalFrictionChanged?.Invoke( this ); DataWasChanged(); } finally { _tracksLongitudinalFriction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TracksLongitudinalFriction"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> TracksLongitudinalFrictionChanged;
		ReferenceField<double> _tracksLongitudinalFriction = 4.0;

		/// <summary>
		/// Friction in sideway direction of tire. For Tracks mode only.
		/// </summary>
		[DefaultValue( 2.0 )]
		[Category( "Physics" )]//[Category( "Friction" )]
		[DisplayName( "Lateral Friction" )]
		public Reference<double> TracksLateralFriction
		{
			get { if( _tracksLateralFriction.BeginGet() ) TracksLateralFriction = _tracksLateralFriction.Get( this ); return _tracksLateralFriction.value; }
			set { if( _tracksLateralFriction.BeginSet( this, ref value ) ) { try { TracksLateralFrictionChanged?.Invoke( this ); DataWasChanged(); } finally { _tracksLateralFriction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TracksLateralFriction"/> property value changes.</summary>
		public event Action<VehicleTypeWheel> TracksLateralFrictionChanged;
		ReferenceField<double> _tracksLateralFriction = 2.0;

		///////////////////////////////////////////////

		public VehicleTypeWheel()
		{
			_longitudinalFriction = new ReferenceList<CurvePoint1F>( this, delegate () { LongitudinalFrictionChanged?.Invoke( this ); DataWasChanged(); } );
			_lateralFriction = new ReferenceList<CurvePoint1F>( this, delegate () { LateralFrictionChanged?.Invoke( this ); DataWasChanged(); } );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Distance ):
					{
						if( Count.Value == 1 )
							skip = true;

						var parent = Parent as VehicleType;
						var wheels = parent != null && parent.Chassis.Value == VehicleType.ChassisEnum.Wheels;
						if( !wheels )
							skip = true;
					}
					break;

				case nameof( Pair ):
				case nameof( Mass ):
				case nameof( LongitudinalFriction ):
				case nameof( LateralFriction ):
				case nameof( Drive ):
				case nameof( AntiRollBarStiffness ):
				case nameof( MaxBrakeTorque ):
				case nameof( MaxHandBrakeTorque ):
				case nameof( AngularDamping ):
				case nameof( MaxSteeringAngle ):
				case nameof( SteeringTime ):
				case nameof( Count ):
					{
						var parent = Parent as VehicleType;
						var wheels = parent != null && parent.Chassis.Value == VehicleType.ChassisEnum.Wheels;
						if( !wheels )
							skip = true;
					}
					break;

				case nameof( DifferentialRatio ):
				case nameof( DifferentialLeftRightSplit ):
				case nameof( DifferentialLimitedSlipRatio ):
				case nameof( DifferentialEngineTorqueRatio ):
					{
						var parent = Parent as VehicleType;
						var wheels = parent != null && parent.Chassis.Value == VehicleType.ChassisEnum.Wheels;
						if( !wheels || !Drive )
							skip = true;
					}
					break;

				case nameof( TracksLongitudinalFriction ):
				case nameof( TracksLateralFriction ):
					{
						var parent = Parent as VehicleType;
						var tracks = parent != null && parent.Chassis.Value == VehicleType.ChassisEnum.Tracks;
						if( !tracks )
							skip = true;
					}
					break;
				}
			}
		}

		public void DataWasChanged()
		{
			var parent = Parent as VehicleType;
			parent?.DataWasChanged();
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
			{
				if( LongitudinalFriction.Count == 0 )
				{
					LongitudinalFriction.Add( new CurvePoint1F( 0, 0 ) );
					LongitudinalFriction.Add( new CurvePoint1F( 0.06f, 3.6f ) );//LongitudinalFriction.Add( new CurvePoint1F( 0.06f, 1.2f ) );
					LongitudinalFriction.Add( new CurvePoint1F( 0.2f, 3.0f ) );//LongitudinalFriction.Add( new CurvePoint1F( 0.2f, 1.0f ) );
				}

				if( LateralFriction.Count == 0 )
				{
					LateralFriction.Add( new CurvePoint1F( 0, 0 ) );
					LateralFriction.Add( new CurvePoint1F( 3.0f, 3.6f ) );//LateralFriction.Add( new CurvePoint1F( 3.0f, 1.2f ) );
					LateralFriction.Add( new CurvePoint1F( 20.0f, 3.0f ) );//LateralFriction.Add( new CurvePoint1F( 20.0f, 1.0f ) );
				}
			}
		}
	}
}
