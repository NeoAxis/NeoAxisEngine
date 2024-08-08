// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// A definition of the vehicle type.
	/// </summary>
	[ResourceFileExtension( "vehicletype" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Vehicle\Vehicle Type", 22000 )]
	[EditorControl( "NeoAxis.Editor.VehicleTypeEditor" )] //[EditorControl( typeof( VehicleTypeEditor ) )]
	[Preview( typeof( VehicleTypePreview ) )]
	[PreviewImage( typeof( VehicleTypePreviewImage ) )]
#endif
	public class VehicleType : Component
	{
		int version;

		//

		const string meshDefault = @"Content\Vehicles\Default\Body.gltf|$Mesh";

		/// <summary>
		/// The main mesh of the vehicle.
		/// </summary>
		[DefaultValueReference( meshDefault )]
		[Category( "Common" )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( this, ref value ) ) { try { MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<VehicleType> MeshChanged;
		ReferenceField<Mesh> _mesh = new Reference<Mesh>( null, meshDefault );

		/// <summary>
		/// The type of chassis.
		/// </summary>
		[DefaultValue( ChassisEnum.Wheels )]
		[Category( "Common" )]
		public Reference<ChassisEnum> Chassis
		{
			get { if( _chassis.BeginGet() ) Chassis = _chassis.Get( this ); return _chassis.value; }
			set { if( _chassis.BeginSet( this, ref value ) ) { try { ChassisChanged?.Invoke( this ); DataWasChanged(); } finally { _chassis.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Chassis"/> property value changes.</summary>
		public event Action<VehicleType> ChassisChanged;
		ReferenceField<ChassisEnum> _chassis = ChassisEnum.Wheels;

		///////////////////////////////////////////////

		/// <summary>
		/// Ratio max / min average wheel speed of each differential (measured at the clutch). When the ratio is exceeded all torque gets distributed to the differential with the minimal average velocity. This allows implementing a limited slip differential between differentials. Set to FLT_MAX for an open differential. Value should be > 1.
		/// </summary>
		[Category( "Wheels" )]
		[DefaultValue( 1.4 )]
		public Reference<double> DifferentialLimitedSlipRatio
		{
			get { if( _differentialLimitedSlipRatio.BeginGet() ) DifferentialLimitedSlipRatio = _differentialLimitedSlipRatio.Get( this ); return _differentialLimitedSlipRatio.value; }
			set { if( _differentialLimitedSlipRatio.BeginSet( this, ref value ) ) { try { DifferentialLimitedSlipRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _differentialLimitedSlipRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DifferentialLimitedSlipRatio"/> property value changes.</summary>
		public event Action<VehicleType> DifferentialLimitedSlipRatioChanged;
		ReferenceField<double> _differentialLimitedSlipRatio = 1.4;

		///////////////////////////////////////////////
		//tracks

		/// <summary>
		/// Which wheel on the track is connected to the engine.
		/// </summary>
		[DefaultValue( 0 )]
		[Category( "Tracks" )]
		public Reference<int> TrackDrivenWheel
		{
			get { if( _trackDrivenWheel.BeginGet() ) TrackDrivenWheel = _trackDrivenWheel.Get( this ); return _trackDrivenWheel.value; }
			set { if( _trackDrivenWheel.BeginSet( this, ref value ) ) { try { TrackDrivenWheelChanged?.Invoke( this ); DataWasChanged(); } finally { _trackDrivenWheel.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrackDrivenWheel"/> property value changes.</summary>
		public event Action<VehicleType> TrackDrivenWheelChanged;
		ReferenceField<int> _trackDrivenWheel = 0;

		/// <summary>
		/// Moment of inertia (kg m^2) of the track and its wheels as seen on the driven wheel.
		/// </summary>
		[DefaultValue( 10.0 )]
		[Category( "Tracks" )]
		public Reference<double> TrackInertia
		{
			get { if( _trackInertia.BeginGet() ) TrackInertia = _trackInertia.Get( this ); return _trackInertia.value; }
			set { if( _trackInertia.BeginSet( this, ref value ) ) { try { TrackInertiaChanged?.Invoke( this ); DataWasChanged(); } finally { _trackInertia.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrackInertia"/> property value changes.</summary>
		public event Action<VehicleType> TrackInertiaChanged;
		ReferenceField<double> _trackInertia = 10.0;

		/// <summary>
		/// Damping factor of track and its wheels: dw/dt = -c * w as seen on the driven wheel
		/// </summary>
		[DefaultValue( 0.5 )]
		[Category( "Tracks" )]
		public Reference<double> TrackAngularDamping
		{
			get { if( _trackAngularDamping.BeginGet() ) TrackAngularDamping = _trackAngularDamping.Get( this ); return _trackAngularDamping.value; }
			set { if( _trackAngularDamping.BeginSet( this, ref value ) ) { try { TrackAngularDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _trackAngularDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrackAngularDamping"/> property value changes.</summary>
		public event Action<VehicleType> TrackAngularDampingChanged;
		ReferenceField<double> _trackAngularDamping = 0.5;

		/// <summary>
		/// How much torque (Nm) the brakes can apply on the driven wheel.
		/// </summary>
		[DefaultValue( 15000.0 )]
		[Category( "Tracks" )]
		public Reference<double> TrackMaxBrakeTorque
		{
			get { if( _trackMaxBrakeTorque.BeginGet() ) TrackMaxBrakeTorque = _trackMaxBrakeTorque.Get( this ); return _trackMaxBrakeTorque.value; }
			set { if( _trackMaxBrakeTorque.BeginSet( this, ref value ) ) { try { TrackMaxBrakeTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _trackMaxBrakeTorque.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrackMaxBrakeTorque"/> property value changes.</summary>
		public event Action<VehicleType> TrackMaxBrakeTorqueChanged;
		ReferenceField<double> _trackMaxBrakeTorque = 15000.0;

		/// <summary>
		/// Ratio between rotation speed of gear box and driven wheel of track.
		/// </summary>
		[DefaultValue( 6.0 )]
		[Category( "Tracks" )]
		public Reference<double> TrackDifferentialRatio
		{
			get { if( _trackDifferentialRatio.BeginGet() ) TrackDifferentialRatio = _trackDifferentialRatio.Get( this ); return _trackDifferentialRatio.value; }
			set { if( _trackDifferentialRatio.BeginSet( this, ref value ) ) { try { TrackDifferentialRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _trackDifferentialRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrackDifferentialRatio"/> property value changes.</summary>
		public event Action<VehicleType> TrackDifferentialRatioChanged;
		ReferenceField<double> _trackDifferentialRatio = 6.0;

		const string trackFragmentMeshDefault = "";//@"Content\Vehicles\Default\Wheel.gltf|$Mesh";

		/// <summary>
		/// Mesh of the track.
		/// </summary>
		[DefaultValueReference( trackFragmentMeshDefault )]
		[Category( "Tracks" )]
		public Reference<Mesh> TrackFragmentMesh
		{
			get { if( _trackFragmentMesh.BeginGet() ) TrackFragmentMesh = _trackFragmentMesh.Get( this ); return _trackFragmentMesh.value; }
			set { if( _trackFragmentMesh.BeginSet( this, ref value ) ) { try { TrackFragmentMeshChanged?.Invoke( this ); DataWasChanged(); } finally { _trackFragmentMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrackFragmentMesh"/> property value changes.</summary>
		public event Action<VehicleType> TrackFragmentMeshChanged;
		ReferenceField<Mesh> _trackFragmentMesh = new Reference<Mesh>( null, trackFragmentMeshDefault );

		/// <summary>
		/// Length of the track piece.
		/// </summary>
		[DefaultValue( 0.3 )]
		[Range( 0.1, 1.0 )]
		[Category( "Tracks" )]
		public Reference<double> TrackFragmentLength
		{
			get { if( _trackFragmentLength.BeginGet() ) TrackFragmentLength = _trackFragmentLength.Get( this ); return _trackFragmentLength.value; }
			set { if( _trackFragmentLength.BeginSet( this, ref value ) ) { try { TrackFragmentLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _trackFragmentLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrackFragmentLength"/> property value changes.</summary>
		public event Action<VehicleType> TrackFragmentLengthChanged;
		ReferenceField<double> _trackFragmentLength = 0.3;

		/// <summary>
		/// Max amount of torque (Nm) that the engine can deliver.
		/// </summary>
		[DefaultValue( 500.0 )]
		[Category( "Engine" )]
		public Reference<double> EngineMaxTorque
		{
			get { if( _engineMaxTorque.BeginGet() ) EngineMaxTorque = _engineMaxTorque.Get( this ); return _engineMaxTorque.value; }
			set { if( _engineMaxTorque.BeginSet( this, ref value ) ) { try { EngineMaxTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _engineMaxTorque.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EngineMaxTorque"/> property value changes.</summary>
		public event Action<VehicleType> EngineMaxTorqueChanged;
		ReferenceField<double> _engineMaxTorque = 500.0;

		/// <summary>
		/// Min amount of revolutions per minute (rpm) the engine can produce without stalling.
		/// </summary>
		[DefaultValue( 1000.0 )]
		[Category( "Engine" )]
		[DisplayName( "Engine Min RPM" )]
		public Reference<double> EngineMinRPM
		{
			get { if( _engineMinRPM.BeginGet() ) EngineMinRPM = _engineMinRPM.Get( this ); return _engineMinRPM.value; }
			set { if( _engineMinRPM.BeginSet( this, ref value ) ) { try { EngineMinRPMChanged?.Invoke( this ); DataWasChanged(); } finally { _engineMinRPM.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EngineMinRPM"/> property value changes.</summary>
		public event Action<VehicleType> EngineMinRPMChanged;
		ReferenceField<double> _engineMinRPM = 1000.0;

		/// <summary>
		/// Max amount of revolutions per minute (rpm) the engine can generate.
		/// </summary>
		[DefaultValue( 6000.0 )]
		[Category( "Engine" )]
		[DisplayName( "Engine Max RPM" )]
		public Reference<double> EngineMaxRPM
		{
			get { if( _engineMaxRPM.BeginGet() ) EngineMaxRPM = _engineMaxRPM.Get( this ); return _engineMaxRPM.value; }
			set { if( _engineMaxRPM.BeginSet( this, ref value ) ) { try { EngineMaxRPMChanged?.Invoke( this ); DataWasChanged(); } finally { _engineMaxRPM.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EngineMaxRPM"/> property value changes.</summary>
		public event Action<VehicleType> EngineMaxRPMChanged;
		ReferenceField<double> _engineMaxRPM = 6000.0;

		/// <summary>
		/// Curve that describes a ratio of the max torque the engine can produce vs the fraction of the max RPM of the engine.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		[Category( "Engine" )]
		public ReferenceList<CurvePoint1F> EngineNormalizedTorque
		{
			get { return _engineNormalizedTorque; }
		}
		public delegate void EngineNormalizedTorqueChangedDelegate( VehicleType sender );
		public event EngineNormalizedTorqueChangedDelegate EngineNormalizedTorqueChanged;
		ReferenceList<CurvePoint1F> _engineNormalizedTorque;

		/// <summary>
		/// Moment of inertia (kg m^2) of the engine.
		/// </summary>
		[Category( "Engine" )]
		[DefaultValue( 0.5 )]
		public Reference<double> EngineInertia
		{
			get { if( _engineInertia.BeginGet() ) EngineInertia = _engineInertia.Get( this ); return _engineInertia.value; }
			set { if( _engineInertia.BeginSet( this, ref value ) ) { try { EngineInertiaChanged?.Invoke( this ); DataWasChanged(); } finally { _engineInertia.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EngineInertia"/> property value changes.</summary>
		public event Action<VehicleType> EngineInertiaChanged;
		ReferenceField<double> _engineInertia = 0.5;

		/// <summary>
		/// Angular damping factor of the wheel: dw/dt = -c * w.
		/// </summary>
		[Category( "Engine" )]
		[DefaultValue( 0.2 )]
		public Reference<double> EngineAngularDamping
		{
			get { if( _engineAngularDamping.BeginGet() ) EngineAngularDamping = _engineAngularDamping.Get( this ); return _engineAngularDamping.value; }
			set { if( _engineAngularDamping.BeginSet( this, ref value ) ) { try { EngineAngularDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _engineAngularDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="EngineAngularDamping"/> property value changes.</summary>
		public event Action<VehicleType> EngineAngularDampingChanged;
		ReferenceField<double> _engineAngularDamping = 0.2;


		//transmission

		/// <summary>
		/// How to switch gears.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Transmission" )]
		public Reference<bool> TransmissionAuto
		{
			get { if( _transmissionAuto.BeginGet() ) TransmissionAuto = _transmissionAuto.Get( this ); return _transmissionAuto.value; }
			set { if( _transmissionAuto.BeginSet( this, ref value ) ) { try { TransmissionAutoChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionAuto.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransmissionAuto"/> property value changes.</summary>
		public event Action<VehicleType> TransmissionAutoChanged;
		ReferenceField<bool> _transmissionAuto = true;

		//!!!!default value
		/// <summary>
		/// Ratio in rotation rate between engine and gear box, first element is 1st gear, 2nd element 2nd gear etc.
		/// </summary>
		[Cloneable( CloneType.Deep )]
		[Category( "Transmission" )]
		public Reference<double[]> TransmissionGearRatios
		{
			get { if( _transmissionGearRatios.BeginGet() ) TransmissionGearRatios = _transmissionGearRatios.Get( this ); return _transmissionGearRatios.value; }
			set { if( _transmissionGearRatios.BeginSet( this, ref value ) ) { try { TransmissionGearRatiosChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionGearRatios.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransmissionGearRatios"/> property value changes.</summary>
		public event Action<VehicleType> TransmissionGearRatiosChanged;
		ReferenceField<double[]> _transmissionGearRatios = new double[] { 2.66, 1.78, 1.3, 1.0, 0.74 };

		//!!!!default value
		/// <summary>
		/// Ratio in rotation rate between engine and gear box when driving in reverse.
		/// </summary>
		[Cloneable( CloneType.Deep )]
		[Category( "Transmission" )]
		public Reference<double[]> TransmissionReverseGearRatios
		{
			get { if( _transmissionReverseGearRatios.BeginGet() ) TransmissionReverseGearRatios = _transmissionReverseGearRatios.Get( this ); return _transmissionReverseGearRatios.value; }
			set { if( _transmissionReverseGearRatios.BeginSet( this, ref value ) ) { try { TransmissionReverseGearRatiosChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionReverseGearRatios.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransmissionReverseGearRatios"/> property value changes.</summary>
		public event Action<VehicleType> TransmissionReverseGearRatiosChanged;
		ReferenceField<double[]> _transmissionReverseGearRatios = new double[] { -2.90 };

		/// <summary>
		/// How long it takes to switch gears (s), only used in auto mode.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Category( "Transmission" )]
		public Reference<double> TransmissionSwitchTime
		{
			get { if( _transmissionSwitchTime.BeginGet() ) TransmissionSwitchTime = _transmissionSwitchTime.Get( this ); return _transmissionSwitchTime.value; }
			set { if( _transmissionSwitchTime.BeginSet( this, ref value ) ) { try { TransmissionSwitchTimeChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionSwitchTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransmissionSwitchTime"/> property value changes.</summary>
		public event Action<VehicleType> TransmissionSwitchTimeChanged;
		ReferenceField<double> _transmissionSwitchTime = 0.5;

		/// <summary>
		/// How long it takes to release the clutch (go to full friction), only used in auto mode.
		/// </summary>
		[DefaultValue( 0.3 )]
		[Category( "Transmission" )]
		public Reference<double> TransmissionClutchReleaseTime
		{
			get { if( _transmissionClutchReleaseTime.BeginGet() ) TransmissionClutchReleaseTime = _transmissionClutchReleaseTime.Get( this ); return _transmissionClutchReleaseTime.value; }
			set { if( _transmissionClutchReleaseTime.BeginSet( this, ref value ) ) { try { TransmissionClutchReleaseTimeChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionClutchReleaseTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransmissionClutchReleaseTime"/> property value changes.</summary>
		public event Action<VehicleType> TransmissionClutchReleaseTimeChanged;
		ReferenceField<double> _transmissionClutchReleaseTime = 0.3;

		/// <summary>
		/// How long to wait after releasing the clutch before another switch is attempted (s), only used in auto mode.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Category( "Transmission" )]
		public Reference<double> TransmissionSwitchLatency
		{
			get { if( _transmissionSwitchLatency.BeginGet() ) TransmissionSwitchLatency = _transmissionSwitchLatency.Get( this ); return _transmissionSwitchLatency.value; }
			set { if( _transmissionSwitchLatency.BeginSet( this, ref value ) ) { try { TransmissionSwitchLatencyChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionSwitchLatency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransmissionSwitchLatency"/> property value changes.</summary>
		public event Action<VehicleType> TransmissionSwitchLatencyChanged;
		ReferenceField<double> _transmissionSwitchLatency = 0.5;

		/// <summary>
		/// If RPM of engine is bigger then this we will shift a gear up, only used in auto mode.
		/// </summary>
		[DefaultValue( 4000.0 )]
		[Category( "Transmission" )]
		[DisplayName( "Transmission Shift Up RPM" )]
		public Reference<double> TransmissionShiftUpRPM
		{
			get { if( _transmissionShiftUpRPM.BeginGet() ) TransmissionShiftUpRPM = _transmissionShiftUpRPM.Get( this ); return _transmissionShiftUpRPM.value; }
			set { if( _transmissionShiftUpRPM.BeginSet( this, ref value ) ) { try { TransmissionShiftUpRPMChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionShiftUpRPM.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransmissionShiftUpRPM"/> property value changes.</summary>
		public event Action<VehicleType> TransmissionShiftUpRPMChanged;
		ReferenceField<double> _transmissionShiftUpRPM = 4000.0;

		/// <summary>
		/// If RPM of engine is smaller then this we will shift a gear down, only used in auto mode.
		/// </summary>
		[DefaultValue( 2000.0 )]
		[Category( "Transmission" )]
		[DisplayName( "Transmission Shift Down RPM" )]
		public Reference<double> TransmissionShiftDownRPM
		{
			get { if( _transmissionShiftDownRPM.BeginGet() ) TransmissionShiftDownRPM = _transmissionShiftDownRPM.Get( this ); return _transmissionShiftDownRPM.value; }
			set { if( _transmissionShiftDownRPM.BeginSet( this, ref value ) ) { try { TransmissionShiftDownRPMChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionShiftDownRPM.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransmissionShiftDownRPM"/> property value changes.</summary>
		public event Action<VehicleType> TransmissionShiftDownRPMChanged;
		ReferenceField<double> _transmissionShiftDownRPM = 2000.0;

		/// <summary>
		/// Strength of the clutch when fully engaged. Total torque a clutch applies is Torque = ClutchStrength * (Velocity Engine - Avg Velocity Wheels) (units: k m^2 s^-1).
		/// </summary>
		[DefaultValue( 10.0 )]
		[Category( "Transmission" )]
		public Reference<double> TransmissionClutchStrength
		{
			get { if( _transmissionClutchStrength.BeginGet() ) TransmissionClutchStrength = _transmissionClutchStrength.Get( this ); return _transmissionClutchStrength.value; }
			set { if( _transmissionClutchStrength.BeginSet( this, ref value ) ) { try { TransmissionClutchStrengthChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionClutchStrength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransmissionClutchStrength"/> property value changes.</summary>
		public event Action<VehicleType> TransmissionClutchStrengthChanged;
		ReferenceField<double> _transmissionClutchStrength = 10.0;

		/// <summary>
		/// Max angle that is considered for colliding wheels. This is to avoid colliding with vertical walls.
		/// </summary>
		[DefaultValue( 80 )]
		[Category( "Special" )]
		public Reference<Degree> MaxSlopeAngle
		{
			get { if( _maxSlopeAngle.BeginGet() ) MaxSlopeAngle = _maxSlopeAngle.Get( this ); return _maxSlopeAngle.value; }
			set { if( _maxSlopeAngle.BeginSet( this, ref value ) ) { try { MaxSlopeAngleChanged?.Invoke( this ); DataWasChanged(); } finally { _maxSlopeAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxSlopeAngle"/> property value changes.</summary>
		public event Action<VehicleType> MaxSlopeAngleChanged;
		ReferenceField<Degree> _maxSlopeAngle = new Degree( 80 );

		/// <summary>
		/// Defines the maximum pitch/roll angle, can be used to avoid the car from getting upside down. The vehicle up direction will stay within a cone centered around the up axis with half top angle MaxPitchRollAngle, set to 180 to turn off.
		/// </summary>
		[DefaultValue( 180 )]
		[Range( 0, 180 )]
		[Category( "Special" )]
		public Reference<Degree> MaxPitchRollAngle
		{
			get { if( _maxPitchRollAngle.BeginGet() ) MaxPitchRollAngle = _maxPitchRollAngle.Get( this ); return _maxPitchRollAngle.value; }
			set { if( _maxPitchRollAngle.BeginSet( this, ref value ) ) { try { MaxPitchRollAngleChanged?.Invoke( this ); DataWasChanged(); } finally { _maxPitchRollAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxPitchRollAngle"/> property value changes.</summary>
		public event Action<VehicleType> MaxPitchRollAngleChanged;
		ReferenceField<Degree> _maxPitchRollAngle = new Degree( 180 );

		const string motorOnSoundDefault = @"Content\Vehicles\Default\Sounds\MotorOn.ogg";

		[Category( "Sound" )]
		[DefaultValueReference( motorOnSoundDefault )]
		public Reference<Sound> MotorOnSound
		{
			get { if( _motorOnSound.BeginGet() ) MotorOnSound = _motorOnSound.Get( this ); return _motorOnSound.value; }
			set { if( _motorOnSound.BeginSet( this, ref value ) ) { try { MotorOnSoundChanged?.Invoke( this ); } finally { _motorOnSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MotorOnSound"/> property value changes.</summary>
		public event Action<VehicleType> MotorOnSoundChanged;
		ReferenceField<Sound> _motorOnSound = new Reference<Sound>( null, motorOnSoundDefault );

		const string motorOffSoundDefault = @"Content\Vehicles\Default\Sounds\MotorOff.ogg";

		[Category( "Sound" )]
		[DefaultValueReference( motorOffSoundDefault )]
		public Reference<Sound> MotorOffSound
		{
			get { if( _motorOffSound.BeginGet() ) MotorOffSound = _motorOffSound.Get( this ); return _motorOffSound.value; }
			set { if( _motorOffSound.BeginSet( this, ref value ) ) { try { MotorOffSoundChanged?.Invoke( this ); } finally { _motorOffSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MotorOffSound"/> property value changes.</summary>
		public event Action<VehicleType> MotorOffSoundChanged;
		ReferenceField<Sound> _motorOffSound = new Reference<Sound>( null, motorOffSoundDefault );

		[Serialize]
		[Cloneable( CloneType.Deep )]
		[Category( "Sound" )]
		public ReferenceList<Sound> GearSounds
		{
			get { return _gearSounds; }
		}
		public delegate void GearSoundsChangedDelegate( VehicleType sender );
		/// <summary>Occurs when the <see cref="GearSoundsChanged"/> property value changes.</summary>
		public event GearSoundsChangedDelegate GearSoundsChanged;
		ReferenceList<Sound> _gearSounds;

		[Category( "Sound" )]
		[DefaultValue( null )]
		public Reference<Sound> GearUpSound
		{
			get { if( _gearUpSound.BeginGet() ) GearUpSound = _gearUpSound.Get( this ); return _gearUpSound.value; }
			set { if( _gearUpSound.BeginSet( this, ref value ) ) { try { GearUpSoundChanged?.Invoke( this ); } finally { _gearUpSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GearUpSound"/> property value changes.</summary>
		public event Action<VehicleType> GearUpSoundChanged;
		ReferenceField<Sound> _gearUpSound = null;

		[Category( "Sound" )]
		[DefaultValue( null )]
		public Reference<Sound> GearDownSound
		{
			get { if( _gearDownSound.BeginGet() ) GearDownSound = _gearDownSound.Get( this ); return _gearDownSound.value; }
			set { if( _gearDownSound.BeginSet( this, ref value ) ) { try { GearDownSoundChanged?.Invoke( this ); } finally { _gearDownSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="GearDownSound"/> property value changes.</summary>
		public event Action<VehicleType> GearDownSoundChanged;
		ReferenceField<Sound> _gearDownSound = null;

		//!!!!can be several turrets. FirstTurretTurnSound or not here
		[Category( "Sound" )]
		[DefaultValue( null )]
		public Reference<Sound> TurretTurnSound
		{
			get { if( _turretTurnSound.BeginGet() ) TurretTurnSound = _turretTurnSound.Get( this ); return _turretTurnSound.value; }
			set { if( _turretTurnSound.BeginSet( this, ref value ) ) { try { TurretTurnSoundChanged?.Invoke( this ); } finally { _turretTurnSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TurretTurnSound"/> property value changes.</summary>
		public event Action<VehicleType> TurretTurnSoundChanged;
		ReferenceField<Sound> _turretTurnSound = null;

		///////////////////////////////////////////////

		[Serialize]
		[Browsable( false )]
		public Transform EditorCameraTransform;

		/// <summary>
		/// Whether to display the debug visualization of the wheels.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( true )]
		public bool EditorDisplayWheels { get; set; } = true;

		/// <summary>
		/// Whether to display the debug visualization of the seats.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( true )]
		public bool EditorDisplaySeats { get; set; } = true;

		/// <summary>
		/// Whether to display the debug visualization of the lights.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( true )]
		public bool EditorDisplayLights { get; set; } = true;

		/// <summary>
		/// Whether to display the debug visualization of the physics.
		/// </summary>
		[Serialize]
		[Browsable( false )]
		[DefaultValue( true )]
		public bool EditorDisplayPhysics { get; set; } = false;

		/////////////////////////////////////////

		public enum ChassisEnum
		{
			None,
			Wheels,
			Tracks,
		}

		/////////////////////////////////////////

		public enum WheelDriveEnum
		{
			Front,
			Rear,
			All,
		}

		/////////////////////////////////////////

		public struct GetMeshesInDefaultStateItem
		{
			public Mesh Mesh;
			public Transform Transform;

			public GetMeshesInDefaultStateItem( Mesh mesh, Transform transform )
			{
				Mesh = mesh;
				Transform = transform;
			}
		}

		/////////////////////////////////////////

		public VehicleType()
		{
			_gearSounds = new ReferenceList<Sound>( this, delegate () { GearSoundsChanged?.Invoke( this ); DataWasChanged(); } );
			_engineNormalizedTorque = new ReferenceList<CurvePoint1F>( this, delegate () { EngineNormalizedTorqueChanged?.Invoke( this ); DataWasChanged(); } );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( TransmissionSwitchTime ):
				case nameof( TransmissionClutchReleaseTime ):
				case nameof( TransmissionSwitchLatency ):
				case nameof( TransmissionShiftUpRPM ):
				case nameof( TransmissionShiftDownRPM ):
					if( !TransmissionAuto )
						skip = true;
					break;

				//case nameof( TrackWheels ):
				case nameof( TrackDrivenWheel ):
				case nameof( TrackInertia ):
				case nameof( TrackAngularDamping ):
				case nameof( TrackMaxBrakeTorque ):
				case nameof( TrackDifferentialRatio ):
				case nameof( TrackFragmentMesh ):
				case nameof( TrackFragmentLength ):
					if( Chassis.Value != ChassisEnum.Tracks )
						skip = true;
					break;

				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
			{
				if( EngineNormalizedTorque.Count == 0 )
				{
					EngineNormalizedTorque.Add( new CurvePoint1F( 0.0f, 0.8f ) );
					EngineNormalizedTorque.Add( new CurvePoint1F( 0.66f, 1.0f ) );
					EngineNormalizedTorque.Add( new CurvePoint1F( 1.0f, 0.8f ) );
				}
			}
		}

		[Browsable( false )]
		public int Version
		{
			get { return version; }
		}

		public void DataWasChanged()
		{
			unchecked
			{
				version++;
			}
		}

		public GetMeshesInDefaultStateItem[] GetMeshesInDefaultState()
		{
			var result = new List<GetMeshesInDefaultStateItem>( 8 );

			if( Mesh.Value != null )
				result.Add( new GetMeshesInDefaultStateItem( Mesh.Value, Transform.Identity ) );

			if( Chassis.Value != ChassisEnum.None )
			{
				var typeWheels = GetComponents<VehicleTypeWheel>().Where( w => w.Enabled ).ToArray();
				if( typeWheels.Length >= 2 )
				{
					foreach( var wheel in typeWheels )
					{
						var mesh = wheel.Mesh.Value;
						if( mesh != null )
						{
							for( int n = 0; n < wheel.Count; n++ )
							{
								var pos = wheel.Position + new Vector3( wheel.Distance, 0, 0 ) * n;
								result.Add( new GetMeshesInDefaultStateItem( mesh, new Transform( pos ) ) );
								result.Add( new GetMeshesInDefaultStateItem( mesh, new Transform( pos * new Vector3( 1, -1, 1 ), Quaternion.FromRotateByZ( Math.PI ) ) ) );
							}
						}
					}
				}

				//tracks
				if( Chassis.Value == ChassisEnum.Tracks )
				{

					//!!!!impl

				}
			}


			//!!!!turrets



			//if( Chassis.Value == ChassisEnum.Wheels )
			//{
			//if( FrontWheelMesh.Value != null )
			//{
			//	result.Add( new GetMeshesInDefaultStateItem( FrontWheelMesh.Value, new Transform( FrontWheelPosition ) ) );
			//	result.Add( new GetMeshesInDefaultStateItem( FrontWheelMesh.Value, new Transform( FrontWheelPosition * new Vector3( 1, -1, 1 ), Quaternion.FromRotateByZ( Math.PI ) ) ) );
			//}

			//if( RearWheelMesh.Value != null )
			//{
			//	result.Add( new GetMeshesInDefaultStateItem( RearWheelMesh.Value, new Transform( RearWheelPosition ) ) );
			//	result.Add( new GetMeshesInDefaultStateItem( RearWheelMesh.Value, new Transform( RearWheelPosition * new Vector3( 1, -1, 1 ), Quaternion.FromRotateByZ( Math.PI ) ) ) );
			//}
			//}

			return result.ToArray();
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			if( Components.Count == 0 )
			{
				if( GearSounds.Count == 0 )
				{
					GearSounds.Add( new Reference<Sound>( null, @"Content\Vehicles\Default\Sounds\MotorRun.ogg" ) );
					GearSounds.Add( new Reference<Sound>( null, @"Content\Vehicles\Default\Sounds\MotorIdle.ogg" ) );
					GearSounds.Add( new Reference<Sound>( null, @"Content\Vehicles\Default\Sounds\MotorRun.ogg" ) );
				}

				var frontWheel = CreateComponent<VehicleTypeWheel>();
				frontWheel.Name = "Wheel Front";
				frontWheel.Position = new Vector3( 1.58, 0.8, -0.08 );
				frontWheel.MaxSteeringAngle = new Degree( 45 );

				var rearWheel = CreateComponent<VehicleTypeWheel>();
				rearWheel.Name = "Wheel Rear";
				rearWheel.Position = new Vector3( -1.32, 0.8, -0.08 );


				{
					var seat = CreateComponent<SeatItem>();
					seat.Name = "Seat";
					seat.Transform = new Transform( new Vector3( 0.4, 0.38, 0.05 ), new Quaternion( 0, -0.17364817766693, 0, 0.984807753012208 ), Vector3.One );
					seat.SpineAngle = new Degree( 30 );
					seat.LegsAngle = new Degree( 50 );
					seat.ExitTransform = new Transform( new Vector3( 0, 2, -0.2 ), Quaternion.Identity, Vector3.One );
				}

				{
					var seat = CreateComponent<SeatItem>();
					seat.Name = "Seat 2";
					seat.Transform = new Transform( new Vector3( 0.4, -0.38, 0.05 ), new Quaternion( 0, -0.17364817766693, 0, 0.984807753012208 ), Vector3.One );
					seat.SpineAngle = new Degree( 30 );
					seat.LegsAngle = new Degree( 50 );
					seat.ExitTransform = new Transform( new Vector3( 0, -2, -0.2 ), Quaternion.Identity, Vector3.One );
				}

				{
					var seat = CreateComponent<SeatItem>();
					seat.Name = "Seat 3";
					seat.Transform = new Transform( new Vector3( -0.5, 0.38, 0.05 ), new Quaternion( 0, -0.17364817766693, 0, 0.984807753012208 ), Vector3.One );
					seat.SpineAngle = new Degree( 30 );
					seat.LegsAngle = new Degree( 50 );
					seat.ExitTransform = new Transform( new Vector3( -1, 2, -0.2 ), Quaternion.Identity, Vector3.One );
				}

				{
					var seat = CreateComponent<SeatItem>();
					seat.Name = "Seat 4";
					seat.Transform = new Transform( new Vector3( -0.5, -0.38, 0.05 ), new Quaternion( 0, -0.17364817766693, 0, 0.984807753012208 ), Vector3.One );
					seat.SpineAngle = new Degree( 30 );
					seat.LegsAngle = new Degree( 50 );
					seat.ExitTransform = new Transform( new Vector3( -1, -2, -0.2 ), Quaternion.Identity, Vector3.One );
				}


				{
					var light = CreateComponent<Light>();
					light.Name = "Headlight Low Left";
					light.Transform = new Transform( new Vector3( 2.45, 0.77, 0.22 ), new Quaternion( 0, 0.0871557427476582, 0, 0.996194698091746 ), Vector3.One );
					light.Type = Light.TypeEnum.Spotlight;
					light.Brightness = 250000;
					light.Color = new ColorValue( 0.987098f, 1f, 0.683647f );
					light.AttenuationFar = 43.263999999999996;
					light.SpotlightInnerAngle = new Degree( 53 );
					light.SpotlightOuterAngle = new Degree( 66 );
					light.StartDistance = 0.3;
					light.ShadowNearClipDistance = 0.2;

					light.FlareColor = new ColorValuePowered( 0.987098f, 1f, 0.812f, 1, 1.5 );
					light.FlareSize = new Vector2( 0.8, 0.8 );
					light.FlareSizeFadeByDistance = true;
					light.FlareDepthCheckOffset = 0.2;
					light.FlareImage = new Reference<ImageComponent>( null, @"Base\Images\Lens flares\sparkle_blurred.png" );
				}

				{
					var light = CreateComponent<Light>();
					light.Name = "Headlight Low Right";
					light.Transform = new Transform( new Vector3( 2.45, -0.77, 0.22 ), new Quaternion( 0, 0.0871557427476582, 0, 0.996194698091746 ), Vector3.One );
					light.Type = Light.TypeEnum.Spotlight;
					light.Brightness = 250000;
					light.Color = new ColorValue( 0.987098f, 1f, 0.683647f );
					light.AttenuationFar = 43.263999999999996;
					light.SpotlightInnerAngle = new Degree( 53 );
					light.SpotlightOuterAngle = new Degree( 66 );
					light.StartDistance = 0.3;
					light.ShadowNearClipDistance = 0.2;

					light.FlareColor = new ColorValuePowered( 0.987098f, 1f, 0.812f, 1, 1.5 );
					light.FlareSize = new Vector2( 0.8, 0.8 );
					light.FlareSizeFadeByDistance = true;
					light.FlareDepthCheckOffset = 0.2;
					light.FlareImage = new Reference<ImageComponent>( null, @"Base\Images\Lens flares\sparkle_blurred.png" );
				}

				{
					var light = CreateComponent<Light>();
					light.Name = "Headlight High Left";
					light.Transform = new Transform( new Vector3( 2.45, 0.6, 0.22 ), Quaternion.Identity, Vector3.One );
					light.Type = Light.TypeEnum.Spotlight;
					light.Brightness = 500000;
					light.Color = new ColorValue( 0.987098f, 1f, 0.683647f );
					light.AttenuationFar = 43.263999999999996;
					light.SpotlightInnerAngle = new Degree( 63 );
					light.SpotlightOuterAngle = new Degree( 66 );
					light.StartDistance = 0.3;
					light.ShadowNearClipDistance = 0.2;

					light.FlareColor = new ColorValuePowered( 0.987098f, 1f, 0.812f, 1, 1.5 );
					light.FlareSize = new Vector2( 0.8, 0.8 );
					light.FlareSizeFadeByDistance = true;
					light.FlareDepthCheckOffset = 0.2;
					light.FlareImage = new Reference<ImageComponent>( null, @"Base\Images\Lens flares\sparkle_blurred.png" );
				}

				{
					var light = CreateComponent<Light>();
					light.Name = "Headlight High Right";
					light.Transform = new Transform( new Vector3( 2.45, -0.6, 0.22 ), Quaternion.Identity, Vector3.One );
					light.Type = Light.TypeEnum.Spotlight;
					light.Brightness = 500000;
					light.Color = new ColorValue( 0.987098f, 1f, 0.683647f );
					light.AttenuationFar = 43.263999999999996;
					light.SpotlightInnerAngle = new Degree( 63 );
					light.SpotlightOuterAngle = new Degree( 66 );
					light.StartDistance = 0.3;
					light.ShadowNearClipDistance = 0.2;

					light.FlareColor = new ColorValuePowered( 0.987098f, 1f, 0.812f, 1, 1.5 );
					light.FlareSize = new Vector2( 0.8, 0.8 );
					light.FlareSizeFadeByDistance = true;
					light.FlareDepthCheckOffset = 0.2;
					light.FlareImage = new Reference<ImageComponent>( null, @"Base\Images\Lens flares\sparkle_blurred.png" );
				}


				{
					var light = CreateComponent<Light>();
					light.Name = "Brake Rear Left";
					light.Transform = new Transform( new Vector3( -2.6, 0.66, 0.14 ), Quaternion.Identity, Vector3.One );
					light.Type = Light.TypeEnum.Point;
					light.Brightness = 300000;
					light.Color = new ColorValue( 1, 0, 0 );
					light.AttenuationFar = 2;
					light.ShadowTextureSize = Light.ShadowTextureSizeType.Quarter;
					light.ShadowNearClipDistance = 0.01;
				}

				{
					var light = CreateComponent<Light>();
					light.Name = "Brake Rear Right";
					light.Transform = new Transform( new Vector3( -2.6, -0.66, 0.14 ), Quaternion.Identity, Vector3.One );
					light.Type = Light.TypeEnum.Point;
					light.Brightness = 300000;
					light.Color = new ColorValue( 1, 0, 0 );
					light.AttenuationFar = 2;
					light.ShadowTextureSize = Light.ShadowTextureSizeType.Quarter;
					light.ShadowNearClipDistance = 0.01;
				}


				{
					var light = CreateComponent<Light>();
					light.Name = "Left Turn Side Front";
					light.Transform = new Transform( new Vector3( 2.1, 1, 0.05 ), Quaternion.Identity, Vector3.One );
					light.Type = Light.TypeEnum.Point;
					light.Brightness = 300000;
					light.Color = new ColorValue( 1, 0.5, 0 );
					light.AttenuationFar = 2;
					light.ShadowTextureSize = Light.ShadowTextureSizeType.Quarter;
					light.ShadowNearClipDistance = 0.01;
				}

				{
					var light = CreateComponent<Light>();
					light.Name = "Left Turn Side Rear";
					light.Transform = new Transform( new Vector3( -2.22, 0.9, 0.07 ), Quaternion.Identity, Vector3.One );
					light.Type = Light.TypeEnum.Point;
					light.Brightness = 300000;
					light.Color = new ColorValue( 1, 0.5, 0 );
					light.AttenuationFar = 2;
					light.ShadowTextureSize = Light.ShadowTextureSizeType.Quarter;
					light.ShadowNearClipDistance = 0.01;
				}

				{
					var light = CreateComponent<Light>();
					light.Name = "Left Turn Rear";
					light.Transform = new Transform( new Vector3( -2.59, 0.65, 0.24 ), Quaternion.Identity, Vector3.One );
					light.Type = Light.TypeEnum.Point;
					light.Brightness = 300000;
					light.Color = new ColorValue( 1, 0, 0 );
					light.AttenuationFar = 2;
					light.ShadowTextureSize = Light.ShadowTextureSizeType.Quarter;
					light.ShadowNearClipDistance = 0.01;
				}


				{
					var light = CreateComponent<Light>();
					light.Name = "Right Turn Side Front";
					light.Transform = new Transform( new Vector3( 2.1, -1, 0.05 ), Quaternion.Identity, Vector3.One );
					light.Type = Light.TypeEnum.Point;
					light.Brightness = 300000;
					light.Color = new ColorValue( 1, 0.5, 0 );
					light.AttenuationFar = 2;
					light.ShadowTextureSize = Light.ShadowTextureSizeType.Quarter;
					light.ShadowNearClipDistance = 0.01;
				}

				{
					var light = CreateComponent<Light>();
					light.Name = "Right Turn Side Rear";
					light.Transform = new Transform( new Vector3( -2.22, -0.9, 0.07 ), Quaternion.Identity, Vector3.One );
					light.Type = Light.TypeEnum.Point;
					light.Brightness = 300000;
					light.Color = new ColorValue( 1, 0.5, 0 );
					light.AttenuationFar = 2;
					light.ShadowTextureSize = Light.ShadowTextureSizeType.Quarter;
					light.ShadowNearClipDistance = 0.01;
				}

				{
					var light = CreateComponent<Light>();
					light.Name = "Right Turn Rear";
					light.Transform = new Transform( new Vector3( -2.59, -0.65, 0.24 ), Quaternion.Identity, Vector3.One );
					light.Type = Light.TypeEnum.Point;
					light.Brightness = 300000;
					light.Color = new ColorValue( 1, 0, 0 );
					light.AttenuationFar = 2;
					light.ShadowTextureSize = Light.ShadowTextureSizeType.Quarter;
					light.ShadowNearClipDistance = 0.01;
				}


			}
		}

		public Sound GetGearSound( int gear )
		{
			if( GearSounds.Count != 0 )
			{
				var gear2 = gear + 1;
				if( gear2 < 0 )
					gear2 = 0;
				if( gear2 >= GearSounds.Count )
					gear2 = GearSounds.Count - 1;
				return GearSounds[ gear2 ];
			}
			return null;
		}

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			//old version compatibility
			if( Chassis.Value == ChassisEnum.Wheels && GetComponents<VehicleTypeWheel>().Length == 0 && ( block.AttributeExists( "FrontWheelPosition" ) || block.AttributeExists( "RearWheelPosition" ) ) )
			{
				var frontWheel = CreateComponent<VehicleTypeWheel>();
				frontWheel.Name = "Wheel";
				frontWheel.MaxSteeringAngle = new Degree( 45 );

				var rearWheel = CreateComponent<VehicleTypeWheel>();
				rearWheel.Name = "Wheel 2";

				try
				{
					{
						var wheel = frontWheel;

						if( block.AttributeExists( "FrontWheelPosition" ) )
							wheel.Position = Vector3.Parse( block.GetAttribute( "FrontWheelPosition" ) );
						else
							wheel.Position = new Vector3( 1.58, 0.8, -0.08 );

						if( block.AttributeExists( "FrontWheelDiameter" ) )
							wheel.Diameter = double.Parse( block.GetAttribute( "FrontWheelDiameter" ) );
						if( block.AttributeExists( "FrontWheelWidth" ) )
							wheel.Width = double.Parse( block.GetAttribute( "FrontWheelWidth" ) );
						if( block.AttributeExists( "FrontWheelCount" ) )
							wheel.Count = int.Parse( block.GetAttribute( "FrontWheelCount" ) );
						if( block.AttributeExists( "FrontWheelDistance" ) )
							wheel.Distance = double.Parse( block.GetAttribute( "FrontWheelDistance" ) );
						if( block.AttributeExists( "FrontWheelMass" ) )
							wheel.Mass = double.Parse( block.GetAttribute( "FrontWheelMass" ) );

						var meshBlock = block.FindChild( "FrontWheelMesh" );
						if( meshBlock != null )
							wheel.Mesh = new Reference<Mesh>( null, meshBlock.GetAttribute( "GetByReference" ) );

						if( block.AttributeExists( "FrontWheelSuspensionMinLength" ) )
							wheel.SuspensionMinLength = double.Parse( block.GetAttribute( "FrontWheelSuspensionMinLength" ) );
						if( block.AttributeExists( "FrontWheelSuspensionMaxLength" ) )
							wheel.SuspensionMaxLength = double.Parse( block.GetAttribute( "FrontWheelSuspensionMaxLength" ) );
						if( block.AttributeExists( "FrontWheelSuspensionPreloadLength" ) )
							wheel.SuspensionPreloadLength = double.Parse( block.GetAttribute( "FrontWheelSuspensionPreloadLength" ) );
						if( block.AttributeExists( "FrontWheelSuspensionFrequency" ) )
							wheel.SuspensionFrequency = double.Parse( block.GetAttribute( "FrontWheelSuspensionFrequency" ) );
						if( block.AttributeExists( "FrontWheelSuspensionDamping" ) )
							wheel.SuspensionDamping = double.Parse( block.GetAttribute( "FrontWheelSuspensionDamping" ) );
						if( block.AttributeExists( "FrontWheelAntiRollBarStiffness" ) )
							wheel.AntiRollBarStiffness = double.Parse( block.GetAttribute( "FrontWheelAntiRollBarStiffness" ) );
						if( block.AttributeExists( "FrontWheelMaxBrakeTorque" ) )
							wheel.MaxBrakeTorque = double.Parse( block.GetAttribute( "FrontWheelMaxBrakeTorque" ) );
						if( block.AttributeExists( "FrontWheelMaxHandBrakeTorque" ) )
							wheel.MaxHandBrakeTorque = double.Parse( block.GetAttribute( "FrontWheelMaxHandBrakeTorque" ) );
						if( block.AttributeExists( "FrontWheelAngularDamping" ) )
							wheel.AngularDamping = double.Parse( block.GetAttribute( "FrontWheelAngularDamping" ) );

						//FrontWheelLongitudinalFriction
						//FrontWheelLateralFriction

						if( block.AttributeExists( "FrontWheelMaxSteeringAngle" ) )
							wheel.MaxSteeringAngle = Degree.Parse( block.GetAttribute( "FrontWheelMaxSteeringAngle" ) );
						if( block.AttributeExists( "FrontWheelSteeringTime" ) )
							wheel.SteeringTime = double.Parse( block.GetAttribute( "FrontWheelSteeringTime" ) );
						if( block.AttributeExists( "FrontWheelDrive" ) )
							wheel.Drive = bool.Parse( block.GetAttribute( "FrontWheelDrive" ) );
						if( block.AttributeExists( "FrontWheelDifferentialRatio" ) )
							wheel.DifferentialRatio = double.Parse( block.GetAttribute( "FrontWheelDifferentialRatio" ) );
						if( block.AttributeExists( "FrontWheelDifferentialLeftRightSplit" ) )
							wheel.DifferentialLeftRightSplit = double.Parse( block.GetAttribute( "FrontWheelDifferentialLeftRightSplit" ) );
						if( block.AttributeExists( "FrontWheelDifferentialLimitedSlipRatio" ) )
							wheel.DifferentialLimitedSlipRatio = double.Parse( block.GetAttribute( "FrontWheelDifferentialLimitedSlipRatio" ) );
						if( block.AttributeExists( "FrontWheelDifferentialEngineTorqueRatio" ) )
							wheel.DifferentialEngineTorqueRatio = double.Parse( block.GetAttribute( "FrontWheelDifferentialEngineTorqueRatio" ) );
						if( block.AttributeExists( "FrontWheelRight" ) )
							wheel.Pair = bool.Parse( block.GetAttribute( "FrontWheelRight" ) );
					}

					{
						var wheel = rearWheel;

						if( block.AttributeExists( "RearWheelPosition" ) )
							wheel.Position = Vector3.Parse( block.GetAttribute( "RearWheelPosition" ) );
						if( block.AttributeExists( "RearWheelDiameter" ) )
							wheel.Diameter = double.Parse( block.GetAttribute( "RearWheelDiameter" ) );
						if( block.AttributeExists( "RearWheelWidth" ) )
							wheel.Width = double.Parse( block.GetAttribute( "RearWheelWidth" ) );
						if( block.AttributeExists( "RearWheelCount" ) )
							wheel.Count = int.Parse( block.GetAttribute( "RearWheelCount" ) );
						if( block.AttributeExists( "RearWheelDistance" ) )
							wheel.Distance = double.Parse( block.GetAttribute( "RearWheelDistance" ) );
						if( block.AttributeExists( "RearWheelMass" ) )
							wheel.Mass = double.Parse( block.GetAttribute( "RearWheelMass" ) );

						var meshBlock = block.FindChild( "RearWheelMesh" );
						if( meshBlock != null )
							wheel.Mesh = new Reference<Mesh>( null, meshBlock.GetAttribute( "GetByReference" ) );

						if( block.AttributeExists( "RearWheelSuspensionMinLength" ) )
							wheel.SuspensionMinLength = double.Parse( block.GetAttribute( "RearWheelSuspensionMinLength" ) );
						if( block.AttributeExists( "RearWheelSuspensionMaxLength" ) )
							wheel.SuspensionMaxLength = double.Parse( block.GetAttribute( "RearWheelSuspensionMaxLength" ) );
						if( block.AttributeExists( "RearWheelSuspensionPreloadLength" ) )
							wheel.SuspensionPreloadLength = double.Parse( block.GetAttribute( "RearWheelSuspensionPreloadLength" ) );
						if( block.AttributeExists( "RearWheelSuspensionFrequency" ) )
							wheel.SuspensionFrequency = double.Parse( block.GetAttribute( "RearWheelSuspensionFrequency" ) );
						if( block.AttributeExists( "RearWheelSuspensionDamping" ) )
							wheel.SuspensionDamping = double.Parse( block.GetAttribute( "RearWheelSuspensionDamping" ) );
						if( block.AttributeExists( "RearWheelAntiRollBarStiffness" ) )
							wheel.AntiRollBarStiffness = double.Parse( block.GetAttribute( "RearWheelAntiRollBarStiffness" ) );
						if( block.AttributeExists( "RearWheelMaxBrakeTorque" ) )
							wheel.MaxBrakeTorque = double.Parse( block.GetAttribute( "RearWheelMaxBrakeTorque" ) );
						if( block.AttributeExists( "RearWheelMaxHandBrakeTorque" ) )
							wheel.MaxHandBrakeTorque = double.Parse( block.GetAttribute( "RearWheelMaxHandBrakeTorque" ) );
						if( block.AttributeExists( "RearWheelAngularDamping" ) )
							wheel.AngularDamping = double.Parse( block.GetAttribute( "RearWheelAngularDamping" ) );

						//RearWheelLongitudinalFriction
						//RearWheelLateralFriction

						if( block.AttributeExists( "RearWheelMaxSteeringAngle" ) )
							wheel.MaxSteeringAngle = Degree.Parse( block.GetAttribute( "RearWheelMaxSteeringAngle" ) );
						if( block.AttributeExists( "RearWheelSteeringTime" ) )
							wheel.SteeringTime = double.Parse( block.GetAttribute( "RearWheelSteeringTime" ) );
						if( block.AttributeExists( "RearWheelDrive" ) )
							wheel.Drive = bool.Parse( block.GetAttribute( "RearWheelDrive" ) );
						if( block.AttributeExists( "RearWheelDifferentialRatio" ) )
							wheel.DifferentialRatio = double.Parse( block.GetAttribute( "RearWheelDifferentialRatio" ) );
						if( block.AttributeExists( "RearWheelDifferentialLeftRightSplit" ) )
							wheel.DifferentialLeftRightSplit = double.Parse( block.GetAttribute( "RearWheelDifferentialLeftRightSplit" ) );
						if( block.AttributeExists( "RearWheelDifferentialLimitedSlipRatio" ) )
							wheel.DifferentialLimitedSlipRatio = double.Parse( block.GetAttribute( "RearWheelDifferentialLimitedSlipRatio" ) );
						if( block.AttributeExists( "RearWheelDifferentialEngineTorqueRatio" ) )
							wheel.DifferentialEngineTorqueRatio = double.Parse( block.GetAttribute( "RearWheelDifferentialEngineTorqueRatio" ) );
						if( block.AttributeExists( "RearWheelRight" ) )
							wheel.Pair = bool.Parse( block.GetAttribute( "RearWheelRight" ) );
					}
				}
				catch { }
			}

			return true;
		}
	}
}
