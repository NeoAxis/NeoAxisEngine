// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A definition of the vehicle type.
	/// </summary>
	[ResourceFileExtension( "vehicletype" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Vehicle\Vehicle Type", 22000 )]
	[EditorControl( typeof( VehicleTypeEditor ) )]
	[Preview( typeof( VehicleTypePreview ) )]
	[PreviewImage( typeof( VehicleTypePreviewImage ) )]
#endif
	public class VehicleType : Component
	{
		int version;

		//

		//!!!!use Version
		//DataWasChanged()

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
		//[Category( "Configuration" )]
		[DefaultValue( ChassisEnum._4Wheels )]
		[Category( "Common" )]
		public Reference<ChassisEnum> Chassis
		{
			get { if( _chassis.BeginGet() ) Chassis = _chassis.Get( this ); return _chassis.value; }
			set { if( _chassis.BeginSet( this, ref value ) ) { try { ChassisChanged?.Invoke( this ); DataWasChanged(); } finally { _chassis.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Chassis"/> property value changes.</summary>
		public event Action<VehicleType> ChassisChanged;
		ReferenceField<ChassisEnum> _chassis = ChassisEnum._4Wheels;

		/// <summary>
		/// The position offset for front wheels.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( "1.58 0.8 -0.08" )]// "1.24 0.7 0.03" )]
		[Category( "Front Wheel" )]
		public Reference<Vector3> FrontWheelPosition
		{
			get { if( _frontWheelPosition.BeginGet() ) FrontWheelPosition = _frontWheelPosition.Get( this ); return _frontWheelPosition.value; }
			set { if( _frontWheelPosition.BeginSet( this, ref value ) ) { try { FrontWheelPositionChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelPosition"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelPositionChanged;
		ReferenceField<Vector3> _frontWheelPosition = new Vector3( 1.58, 0.8, -0.08 );// 1.24, 0.7, 0.03 );

		/// <summary>
		/// The diameter of a front wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( 0.665 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelDiameter
		{
			get { if( _frontWheelDiameter.BeginGet() ) FrontWheelDiameter = _frontWheelDiameter.Get( this ); return _frontWheelDiameter.value; }
			set { if( _frontWheelDiameter.BeginSet( this, ref value ) ) { try { FrontWheelDiameterChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelDiameter.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelDiameter"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelDiameterChanged;
		ReferenceField<double> _frontWheelDiameter = 0.665;

		/// <summary>
		/// The width of a front wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( 0.25 )]//0.364 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelWidth
		{
			get { if( _frontWheelWidth.BeginGet() ) FrontWheelWidth = _frontWheelWidth.Get( this ); return _frontWheelWidth.value; }
			set { if( _frontWheelWidth.BeginSet( this, ref value ) ) { try { FrontWheelWidthChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelWidth"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelWidthChanged;
		ReferenceField<double> _frontWheelWidth = 0.25;//0.364;

		/// <summary>
		/// The mass of a front wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( 18.0 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelMass
		{
			get { if( _frontWheelMass.BeginGet() ) FrontWheelMass = _frontWheelMass.Get( this ); return _frontWheelMass.value; }
			set { if( _frontWheelMass.BeginSet( this, ref value ) ) { try { FrontWheelMassChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelMass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelMass"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelMassChanged;
		ReferenceField<double> _frontWheelMass = 18.0;

		const string wheelMeshDefault = @"Content\Vehicles\Default\Wheel.gltf|$Mesh";

		/// <summary>
		/// The mesh of a front wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValueReference( wheelMeshDefault )]
		[Category( "Front Wheel" )]
		public Reference<Mesh> FrontWheelMesh
		{
			get { if( _frontWheelMesh.BeginGet() ) FrontWheelMesh = _frontWheelMesh.Get( this ); return _frontWheelMesh.value; }
			set { if( _frontWheelMesh.BeginSet( this, ref value ) ) { try { FrontWheelMeshChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelMesh"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelMeshChanged;
		ReferenceField<Mesh> _frontWheelMesh = new Reference<Mesh>( null, wheelMeshDefault );

		[DefaultValue( 0.25 )]//0.3 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelSuspensionMinLength
		{
			get { if( _frontWheelSuspensionMinLength.BeginGet() ) FrontWheelSuspensionMinLength = _frontWheelSuspensionMinLength.Get( this ); return _frontWheelSuspensionMinLength.value; }
			set { if( _frontWheelSuspensionMinLength.BeginSet( this, ref value ) ) { try { FrontWheelSuspensionMinLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelSuspensionMinLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelSuspensionMinLength"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelSuspensionMinLengthChanged;
		ReferenceField<double> _frontWheelSuspensionMinLength = 0.25;//0.3;

		[DefaultValue( 0.45 )]//0.5 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelSuspensionMaxLength
		{
			get { if( _frontWheelSuspensionMaxLength.BeginGet() ) FrontWheelSuspensionMaxLength = _frontWheelSuspensionMaxLength.Get( this ); return _frontWheelSuspensionMaxLength.value; }
			set { if( _frontWheelSuspensionMaxLength.BeginSet( this, ref value ) ) { try { FrontWheelSuspensionMaxLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelSuspensionMaxLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelSuspensionMaxLength"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelSuspensionMaxLengthChanged;
		ReferenceField<double> _frontWheelSuspensionMaxLength = 0.45;//0.5;

		[DefaultValue( 0.0 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelSuspensionPreloadLength
		{
			get { if( _frontWheelSuspensionPreloadLength.BeginGet() ) FrontWheelSuspensionPreloadLength = _frontWheelSuspensionPreloadLength.Get( this ); return _frontWheelSuspensionPreloadLength.value; }
			set { if( _frontWheelSuspensionPreloadLength.BeginSet( this, ref value ) ) { try { FrontWheelSuspensionPreloadLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelSuspensionPreloadLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelSuspensionPreloadLength"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelSuspensionPreloadLengthChanged;
		ReferenceField<double> _frontWheelSuspensionPreloadLength = 0.0;

		[DefaultValue( 1.5 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelSuspensionFrequency
		{
			get { if( _frontWheelSuspensionFrequency.BeginGet() ) FrontWheelSuspensionFrequency = _frontWheelSuspensionFrequency.Get( this ); return _frontWheelSuspensionFrequency.value; }
			set { if( _frontWheelSuspensionFrequency.BeginSet( this, ref value ) ) { try { FrontWheelSuspensionFrequencyChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelSuspensionFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelSuspensionFrequency"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelSuspensionFrequencyChanged;
		ReferenceField<double> _frontWheelSuspensionFrequency = 1.5;

		[DefaultValue( 0.5 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelSuspensionDamping
		{
			get { if( _frontWheelSuspensionDamping.BeginGet() ) FrontWheelSuspensionDamping = _frontWheelSuspensionDamping.Get( this ); return _frontWheelSuspensionDamping.value; }
			set { if( _frontWheelSuspensionDamping.BeginSet( this, ref value ) ) { try { FrontWheelSuspensionDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelSuspensionDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelSuspensionDamping"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelSuspensionDampingChanged;
		ReferenceField<double> _frontWheelSuspensionDamping = 0.5;

		/// <summary>
		/// Stiffness (spring constant in N/m) of front wheel anti rollbar. Can be 0 to disable the anti-rollbar.
		/// </summary>
		[DefaultValue( 1000.0 )]
		//[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelAntiRollBarStiffness
		{
			get { if( _frontWheelAntiRollBarStiffness.BeginGet() ) FrontWheelAntiRollBarStiffness = _frontWheelAntiRollBarStiffness.Get( this ); return _frontWheelAntiRollBarStiffness.value; }
			set { if( _frontWheelAntiRollBarStiffness.BeginSet( this, ref value ) ) { try { FrontWheelAntiRollBarStiffnessChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelAntiRollBarStiffness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelAntiRollBarStiffness"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelAntiRollBarStiffnessChanged;
		ReferenceField<double> _frontWheelAntiRollBarStiffness = 1000.0;

		/// <summary>
		/// How much torque (Nm) the brakes can apply to front wheel.
		/// </summary>
		[DefaultValue( 1500.0 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelMaxBrakeTorque
		{
			get { if( _frontWheelMaxBrakeTorque.BeginGet() ) FrontWheelMaxBrakeTorque = _frontWheelMaxBrakeTorque.Get( this ); return _frontWheelMaxBrakeTorque.value; }
			set { if( _frontWheelMaxBrakeTorque.BeginSet( this, ref value ) ) { try { FrontWheelMaxBrakeTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelMaxBrakeTorque.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelMaxBrakeTorque"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelMaxBrakeTorqueChanged;
		ReferenceField<double> _frontWheelMaxBrakeTorque = 1500.0;

		/// <summary>
		/// How much torque (Nm) the hand brake can apply to front wheel.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelMaxHandBrakeTorque
		{
			get { if( _frontWheelMaxHandBrakeTorque.BeginGet() ) FrontWheelMaxHandBrakeTorque = _frontWheelMaxHandBrakeTorque.Get( this ); return _frontWheelMaxHandBrakeTorque.value; }
			set { if( _frontWheelMaxHandBrakeTorque.BeginSet( this, ref value ) ) { try { FrontWheelMaxHandBrakeTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelMaxHandBrakeTorque.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelMaxHandBrakeTorque"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelMaxHandBrakeTorqueChanged;
		ReferenceField<double> _frontWheelMaxHandBrakeTorque = 0.0;

		/// <summary>
		/// Angular damping factor of the wheel: dw/dt = -c * w.
		/// </summary>
		[DefaultValue( 0.2 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelAngularDamping
		{
			get { if( _frontWheelAngularDamping.BeginGet() ) FrontWheelAngularDamping = _frontWheelAngularDamping.Get( this ); return _frontWheelAngularDamping.value; }
			set { if( _frontWheelAngularDamping.BeginSet( this, ref value ) ) { try { FrontWheelAngularDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelAngularDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelAngularDamping"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelAngularDampingChanged;
		ReferenceField<double> _frontWheelAngularDamping = 0.2;

		/// <summary>
		/// Friction in forward direction of tire as a function of the slip ratio (fraction): (omega_wheel * r_wheel - v_longitudinal) / |v_longitudinal|.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		[Category( "Front Wheel" )]
		public ReferenceList<CurvePoint1F> FrontWheelLongitudinalFriction
		{
			get { return _frontWheelLongitudinalFriction; }
		}
		public delegate void FrontWheelLongitudinalFrictionChangedDelegate( VehicleType sender );
		public event FrontWheelLongitudinalFrictionChangedDelegate FrontWheelLongitudinalFrictionChanged;
		ReferenceList<CurvePoint1F> _frontWheelLongitudinalFriction;

		/// <summary>
		/// Friction in sideway direction of tire as a function of the slip angle (degrees): angle between relative contact velocity and vehicle direction.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		[Category( "Front Wheel" )]
		public ReferenceList<CurvePoint1F> FrontWheelLateralFriction
		{
			get { return _frontWheelLateralFriction; }
		}
		public delegate void FrontWheelLateralFrictionChangedDelegate( VehicleType sender );
		public event FrontWheelLateralFrictionChangedDelegate FrontWheelLateralFrictionChanged;
		ReferenceList<CurvePoint1F> _frontWheelLateralFriction;

		/// <summary>
		/// The maximal steering angle of front wheels.
		/// </summary>
		[DefaultValue( 45.0 )]
		[Category( "Front Wheel" )]
		public Reference<Degree> FrontWheelMaxSteeringAngle
		{
			get { if( _frontWheelMaxSteeringAngle.BeginGet() ) FrontWheelMaxSteeringAngle = _frontWheelMaxSteeringAngle.Get( this ); return _frontWheelMaxSteeringAngle.value; }
			set { if( _frontWheelMaxSteeringAngle.BeginSet( this, ref value ) ) { try { FrontWheelMaxSteeringAngleChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelMaxSteeringAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelMaxSteeringAngle"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelMaxSteeringAngleChanged;
		ReferenceField<Degree> _frontWheelMaxSteeringAngle = new Degree( 45.0 );

		[DefaultValue( 0.5 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelSteeringTime
		{
			get { if( _frontWheelSteeringTime.BeginGet() ) FrontWheelSteeringTime = _frontWheelSteeringTime.Get( this ); return _frontWheelSteeringTime.value; }
			set { if( _frontWheelSteeringTime.BeginSet( this, ref value ) ) { try { FrontWheelSteeringTimeChanged?.Invoke( this ); } finally { _frontWheelSteeringTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelSteeringTime"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelSteeringTimeChanged;
		ReferenceField<double> _frontWheelSteeringTime = 0.5;

		[DefaultValue( true )]
		[Category( "Front Wheel" )]
		public Reference<bool> FrontWheelDrive
		{
			get { if( _frontWheelDrive.BeginGet() ) FrontWheelDrive = _frontWheelDrive.Get( this ); return _frontWheelDrive.value; }
			set { if( _frontWheelDrive.BeginSet( this, ref value ) ) { try { FrontWheelDriveChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelDrive.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelDrive"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelDriveChanged;
		ReferenceField<bool> _frontWheelDrive = true;

		/// <summary>
		/// Ratio between rotation speed of gear box and wheels.
		/// </summary>
		[DefaultValue( 3.42 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelDifferentialRatio
		{
			get { if( _frontWheelDifferentialRatio.BeginGet() ) FrontWheelDifferentialRatio = _frontWheelDifferentialRatio.Get( this ); return _frontWheelDifferentialRatio.value; }
			set { if( _frontWheelDifferentialRatio.BeginSet( this, ref value ) ) { try { FrontWheelDifferentialRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelDifferentialRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelDifferentialRatio"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelDifferentialRatioChanged;
		ReferenceField<double> _frontWheelDifferentialRatio = 3.42;

		/// <summary>
		/// Defines how the engine torque is split across the left and right wheel (0 = left, 0.5 = center, 1 = right).
		/// </summary>
		[DefaultValue( 0.5 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelDifferentialLeftRightSplit
		{
			get { if( _frontWheelDifferentialLeftRightSplit.BeginGet() ) FrontWheelDifferentialLeftRightSplit = _frontWheelDifferentialLeftRightSplit.Get( this ); return _frontWheelDifferentialLeftRightSplit.value; }
			set { if( _frontWheelDifferentialLeftRightSplit.BeginSet( this, ref value ) ) { try { FrontWheelDifferentialLeftRightSplitChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelDifferentialLeftRightSplit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelDifferentialLeftRightSplit"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelDifferentialLeftRightSplitChanged;
		ReferenceField<double> _frontWheelDifferentialLeftRightSplit = 0.5;

		/// <summary>
		/// Ratio max / min wheel speed. When this ratio is exceeded, all torque gets distributed to the slowest moving wheel. This allows implementing a limited slip differential. Set to FLT_MAX (3.402823466e+38F) for an open differential. Value should be > 1.
		/// </summary>
		[DefaultValue( 1.4 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelDifferentialLimitedSlipRatio
		{
			get { if( _frontWheelDifferentialLimitedSlipRatio.BeginGet() ) FrontWheelDifferentialLimitedSlipRatio = _frontWheelDifferentialLimitedSlipRatio.Get( this ); return _frontWheelDifferentialLimitedSlipRatio.value; }
			set { if( _frontWheelDifferentialLimitedSlipRatio.BeginSet( this, ref value ) ) { try { FrontWheelDifferentialLimitedSlipRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelDifferentialLimitedSlipRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelDifferentialLimitedSlipRatio"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelDifferentialLimitedSlipRatioChanged;
		ReferenceField<double> _frontWheelDifferentialLimitedSlipRatio = 1.4;

		/// <summary>
		/// How much of the engines torque is applied to this differential (0 = none, 1 = full).
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelDifferentialEngineTorqueRatio
		{
			get { if( _frontWheelDifferentialEngineTorqueRatio.BeginGet() ) FrontWheelDifferentialEngineTorqueRatio = _frontWheelDifferentialEngineTorqueRatio.Get( this ); return _frontWheelDifferentialEngineTorqueRatio.value; }
			set { if( _frontWheelDifferentialEngineTorqueRatio.BeginSet( this, ref value ) ) { try { FrontWheelDifferentialEngineTorqueRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelDifferentialEngineTorqueRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelDifferentialEngineTorqueRatio"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelDifferentialEngineTorqueRatioChanged;
		ReferenceField<double> _frontWheelDifferentialEngineTorqueRatio = 1.0;

		///////////////////////////////////////////////

		/// <summary>
		/// The position offset for rear wheels.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( "-1.3 0.812 0.03" )]
		[Category( "Rear Wheel" )]
		public Reference<Vector3> RearWheelPosition
		{
			get { if( _rearWheelPosition.BeginGet() ) RearWheelPosition = _rearWheelPosition.Get( this ); return _rearWheelPosition.value; }
			set { if( _rearWheelPosition.BeginSet( this, ref value ) ) { try { RearWheelPositionChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelPosition"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelPositionChanged;
		ReferenceField<Vector3> _rearWheelPosition = new Vector3( -1.3, 0.812, 0.03 );

		/// <summary>
		/// The diameter of a rear wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( 0.665 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelDiameter
		{
			get { if( _rearWheelDiameter.BeginGet() ) RearWheelDiameter = _rearWheelDiameter.Get( this ); return _rearWheelDiameter.value; }
			set { if( _rearWheelDiameter.BeginSet( this, ref value ) ) { try { RearWheelDiameterChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelDiameter.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelDiameter"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelDiameterChanged;
		ReferenceField<double> _rearWheelDiameter = 0.665;

		/// <summary>
		/// The width of a rear wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( 0.25 )]//0.364 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelWidth
		{
			get { if( _rearWheelWidth.BeginGet() ) RearWheelWidth = _rearWheelWidth.Get( this ); return _rearWheelWidth.value; }
			set { if( _rearWheelWidth.BeginSet( this, ref value ) ) { try { RearWheelWidthChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelWidth"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelWidthChanged;
		ReferenceField<double> _rearWheelWidth = 0.25;//0.364;

		/// <summary>
		/// The mass of a rear wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( 18.0 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelMass
		{
			get { if( _rearWheelMass.BeginGet() ) RearWheelMass = _rearWheelMass.Get( this ); return _rearWheelMass.value; }
			set { if( _rearWheelMass.BeginSet( this, ref value ) ) { try { RearWheelMassChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelMass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelMass"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelMassChanged;
		ReferenceField<double> _rearWheelMass = 18.0;

		/// <summary>
		/// The mesh of a rear wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValueReference( wheelMeshDefault )]
		[Category( "Rear Wheel" )]
		public Reference<Mesh> RearWheelMesh
		{
			get { if( _rearWheelMesh.BeginGet() ) RearWheelMesh = _rearWheelMesh.Get( this ); return _rearWheelMesh.value; }
			set { if( _rearWheelMesh.BeginSet( this, ref value ) ) { try { RearWheelMeshChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelMesh"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelMeshChanged;
		ReferenceField<Mesh> _rearWheelMesh = new Reference<Mesh>( null, wheelMeshDefault );

		[DefaultValue( 0.25 )]// 0.3 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelSuspensionMinLength
		{
			get { if( _rearWheelSuspensionMinLength.BeginGet() ) RearWheelSuspensionMinLength = _rearWheelSuspensionMinLength.Get( this ); return _rearWheelSuspensionMinLength.value; }
			set { if( _rearWheelSuspensionMinLength.BeginSet( this, ref value ) ) { try { RearWheelSuspensionMinLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelSuspensionMinLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelSuspensionMinLength"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelSuspensionMinLengthChanged;
		ReferenceField<double> _rearWheelSuspensionMinLength = 0.25;//0.3;

		[DefaultValue( 0.45 )]//0.5 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelSuspensionMaxLength
		{
			get { if( _rearWheelSuspensionMaxLength.BeginGet() ) RearWheelSuspensionMaxLength = _rearWheelSuspensionMaxLength.Get( this ); return _rearWheelSuspensionMaxLength.value; }
			set { if( _rearWheelSuspensionMaxLength.BeginSet( this, ref value ) ) { try { RearWheelSuspensionMaxLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelSuspensionMaxLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelSuspensionMaxLength"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelSuspensionMaxLengthChanged;
		ReferenceField<double> _rearWheelSuspensionMaxLength = 0.45;//0.5;

		[DefaultValue( 0.0 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelSuspensionPreloadLength
		{
			get { if( _rearWheelSuspensionPreloadLength.BeginGet() ) RearWheelSuspensionPreloadLength = _rearWheelSuspensionPreloadLength.Get( this ); return _rearWheelSuspensionPreloadLength.value; }
			set { if( _rearWheelSuspensionPreloadLength.BeginSet( this, ref value ) ) { try { RearWheelSuspensionPreloadLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelSuspensionPreloadLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelSuspensionPreloadLength"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelSuspensionPreloadLengthChanged;
		ReferenceField<double> _rearWheelSuspensionPreloadLength = 0.0;

		[DefaultValue( 1.5 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelSuspensionFrequency
		{
			get { if( _rearWheelSuspensionFrequency.BeginGet() ) RearWheelSuspensionFrequency = _rearWheelSuspensionFrequency.Get( this ); return _rearWheelSuspensionFrequency.value; }
			set { if( _rearWheelSuspensionFrequency.BeginSet( this, ref value ) ) { try { RearWheelSuspensionFrequencyChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelSuspensionFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelSuspensionFrequency"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelSuspensionFrequencyChanged;
		ReferenceField<double> _rearWheelSuspensionFrequency = 1.5;

		[DefaultValue( 0.5 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelSuspensionDamping
		{
			get { if( _rearWheelSuspensionDamping.BeginGet() ) RearWheelSuspensionDamping = _rearWheelSuspensionDamping.Get( this ); return _rearWheelSuspensionDamping.value; }
			set { if( _rearWheelSuspensionDamping.BeginSet( this, ref value ) ) { try { RearWheelSuspensionDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelSuspensionDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelSuspensionDamping"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelSuspensionDampingChanged;
		ReferenceField<double> _rearWheelSuspensionDamping = 0.5;

		/// <summary>
		/// Stiffness (spring constant in N/m) of rear wheel anti rollbar. Can be 0 to disable the anti-rollbar.
		/// </summary>
		[DefaultValue( 1000.0 )]
		//[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelAntiRollBarStiffness
		{
			get { if( _rearWheelAntiRollBarStiffness.BeginGet() ) RearWheelAntiRollBarStiffness = _rearWheelAntiRollBarStiffness.Get( this ); return _rearWheelAntiRollBarStiffness.value; }
			set { if( _rearWheelAntiRollBarStiffness.BeginSet( this, ref value ) ) { try { RearWheelAntiRollBarStiffnessChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelAntiRollBarStiffness.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelAntiRollBarStiffness"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelAntiRollBarStiffnessChanged;
		ReferenceField<double> _rearWheelAntiRollBarStiffness = 1000.0;

		/// <summary>
		/// How much torque (Nm) the brakes can apply to rear wheel.
		/// </summary>
		[DefaultValue( 1500.0 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelMaxBrakeTorque
		{
			get { if( _rearWheelMaxBrakeTorque.BeginGet() ) RearWheelMaxBrakeTorque = _rearWheelMaxBrakeTorque.Get( this ); return _rearWheelMaxBrakeTorque.value; }
			set { if( _rearWheelMaxBrakeTorque.BeginSet( this, ref value ) ) { try { RearWheelMaxBrakeTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelMaxBrakeTorque.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelMaxBrakeTorque"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelMaxBrakeTorqueChanged;
		ReferenceField<double> _rearWheelMaxBrakeTorque = 1500.0;

		/// <summary>
		/// How much torque (Nm) the hand brake can apply to rear wheel.
		/// </summary>
		[DefaultValue( 4000.0 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelMaxHandBrakeTorque
		{
			get { if( _rearWheelMaxHandBrakeTorque.BeginGet() ) RearWheelMaxHandBrakeTorque = _rearWheelMaxHandBrakeTorque.Get( this ); return _rearWheelMaxHandBrakeTorque.value; }
			set { if( _rearWheelMaxHandBrakeTorque.BeginSet( this, ref value ) ) { try { RearWheelMaxHandBrakeTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelMaxHandBrakeTorque.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelMaxHandBrakeTorque"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelMaxHandBrakeTorqueChanged;
		ReferenceField<double> _rearWheelMaxHandBrakeTorque = 4000.0;

		/// <summary>
		/// Angular damping factor of the wheel: dw/dt = -c * w.
		/// </summary>
		[DefaultValue( 0.2 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelAngularDamping
		{
			get { if( _rearWheelAngularDamping.BeginGet() ) RearWheelAngularDamping = _rearWheelAngularDamping.Get( this ); return _rearWheelAngularDamping.value; }
			set { if( _rearWheelAngularDamping.BeginSet( this, ref value ) ) { try { RearWheelAngularDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelAngularDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelAngularDamping"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelAngularDampingChanged;
		ReferenceField<double> _rearWheelAngularDamping = 0.2;

		/// <summary>
		/// Friction in forward direction of tire as a function of the slip ratio (fraction): (omega_wheel * r_wheel - v_longitudinal) / |v_longitudinal|.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		[Category( "Rear Wheel" )]
		public ReferenceList<CurvePoint1F> RearWheelLongitudinalFriction
		{
			get { return _rearWheelLongitudinalFriction; }
		}
		public delegate void RearWheelLongitudinalFrictionChangedDelegate( VehicleType sender );
		public event RearWheelLongitudinalFrictionChangedDelegate RearWheelLongitudinalFrictionChanged;
		ReferenceList<CurvePoint1F> _rearWheelLongitudinalFriction;

		/// <summary>
		/// Friction in sideway direction of tire as a function of the slip angle (degrees): angle between relative contact velocity and vehicle direction.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Deep )]
		[Category( "Rear Wheel" )]
		public ReferenceList<CurvePoint1F> RearWheelLateralFriction
		{
			get { return _rearWheelLateralFriction; }
		}
		public delegate void RearWheelLateralFrictionChangedDelegate( VehicleType sender );
		public event RearWheelLateralFrictionChangedDelegate RearWheelLateralFrictionChanged;
		ReferenceList<CurvePoint1F> _rearWheelLateralFriction;

		/// <summary>
		/// The maximal steering angle of rear wheels.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Category( "Rear Wheel" )]
		public Reference<Degree> RearWheelMaxSteeringAngle
		{
			get { if( _rearWheelMaxSteeringAngle.BeginGet() ) RearWheelMaxSteeringAngle = _rearWheelMaxSteeringAngle.Get( this ); return _rearWheelMaxSteeringAngle.value; }
			set { if( _rearWheelMaxSteeringAngle.BeginSet( this, ref value ) ) { try { RearWheelMaxSteeringAngleChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelMaxSteeringAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelMaxSteeringAngle"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelMaxSteeringAngleChanged;
		ReferenceField<Degree> _rearWheelMaxSteeringAngle = new Degree( 0.0 );

		//!!!!it is not supported
		//[DefaultValue( 90.0 )]
		//[Category( "Rear Wheel" )]
		//public Reference<Degree> RearWheelSteeringSpeed
		//{
		//	get { if( _rearWheelSteeringSpeed.BeginGet() ) RearWheelSteeringSpeed = _rearWheelSteeringSpeed.Get( this ); return _rearWheelSteeringSpeed.value; }
		//	set { if( _rearWheelSteeringSpeed.BeginSet( this, ref value ) ) { try { RearWheelSteeringSpeedChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelSteeringSpeed.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RearWheelSteeringSpeed"/> property value changes.</summary>
		//public event Action<VehicleType> RearWheelSteeringSpeedChanged;
		//ReferenceField<Degree> _rearWheelSteeringSpeed = new Degree( 90.0 );

		[DefaultValue( false )]
		[Category( "Rear Wheel" )]
		public Reference<bool> RearWheelDrive
		{
			get { if( _rearWheelDrive.BeginGet() ) RearWheelDrive = _rearWheelDrive.Get( this ); return _rearWheelDrive.value; }
			set { if( _rearWheelDrive.BeginSet( this, ref value ) ) { try { RearWheelDriveChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelDrive.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelDrive"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelDriveChanged;
		ReferenceField<bool> _rearWheelDrive = false;

		/// <summary>
		/// Ratio between rotation speed of gear box and wheels.
		/// </summary>
		[DefaultValue( 3.42 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelDifferentialRatio
		{
			get { if( _rearWheelDifferentialRatio.BeginGet() ) RearWheelDifferentialRatio = _rearWheelDifferentialRatio.Get( this ); return _rearWheelDifferentialRatio.value; }
			set { if( _rearWheelDifferentialRatio.BeginSet( this, ref value ) ) { try { RearWheelDifferentialRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelDifferentialRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelDifferentialRatio"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelDifferentialRatioChanged;
		ReferenceField<double> _rearWheelDifferentialRatio = 3.42;

		/// <summary>
		/// Defines how the engine torque is split across the left and right wheel (0 = left, 0.5 = center, 1 = right).
		/// </summary>
		[DefaultValue( 0.5 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelDifferentialLeftRightSplit
		{
			get { if( _rearWheelDifferentialLeftRightSplit.BeginGet() ) RearWheelDifferentialLeftRightSplit = _rearWheelDifferentialLeftRightSplit.Get( this ); return _rearWheelDifferentialLeftRightSplit.value; }
			set { if( _rearWheelDifferentialLeftRightSplit.BeginSet( this, ref value ) ) { try { RearWheelDifferentialLeftRightSplitChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelDifferentialLeftRightSplit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelDifferentialLeftRightSplit"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelDifferentialLeftRightSplitChanged;
		ReferenceField<double> _rearWheelDifferentialLeftRightSplit = 0.5;

		/// <summary>
		/// Ratio max / min wheel speed. When this ratio is exceeded, all torque gets distributed to the slowest moving wheel. This allows implementing a limited slip differential. Set to FLT_MAX (3.402823466e+38F) for an open differential. Value should be > 1.
		/// </summary>
		[DefaultValue( 1.4 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelDifferentialLimitedSlipRatio
		{
			get { if( _rearWheelDifferentialLimitedSlipRatio.BeginGet() ) RearWheelDifferentialLimitedSlipRatio = _rearWheelDifferentialLimitedSlipRatio.Get( this ); return _rearWheelDifferentialLimitedSlipRatio.value; }
			set { if( _rearWheelDifferentialLimitedSlipRatio.BeginSet( this, ref value ) ) { try { RearWheelDifferentialLimitedSlipRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelDifferentialLimitedSlipRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelDifferentialLimitedSlipRatio"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelDifferentialLimitedSlipRatioChanged;
		ReferenceField<double> _rearWheelDifferentialLimitedSlipRatio = 1.4;

		/// <summary>
		/// How much of the engines torque is applied to this differential (0 = none, 1 = full).
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelDifferentialEngineTorqueRatio
		{
			get { if( _rearWheelDifferentialEngineTorqueRatio.BeginGet() ) RearWheelDifferentialEngineTorqueRatio = _rearWheelDifferentialEngineTorqueRatio.Get( this ); return _rearWheelDifferentialEngineTorqueRatio.value; }
			set { if( _rearWheelDifferentialEngineTorqueRatio.BeginSet( this, ref value ) ) { try { RearWheelDifferentialEngineTorqueRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelDifferentialEngineTorqueRatio.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelDifferentialEngineTorqueRatio"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelDifferentialEngineTorqueRatioChanged;
		ReferenceField<double> _rearWheelDifferentialEngineTorqueRatio = 1.0;

		///// <summary>
		///// The drive wheel type.
		///// </summary>
		////[Category( "Configuration" )]
		//[DefaultValue( WheelDriveEnum.Front )]
		//[Category( "UPDATE" )]
		//public Reference<WheelDriveEnum> WheelDrive
		//{
		//	get { if( _WheelDrive.BeginGet() ) WheelDrive = _WheelDrive.Get( this ); return _WheelDrive.value; }
		//	set { if( _WheelDrive.BeginSet( this, ref value ) ) { try { WheelDriveChanged?.Invoke( this ); DataWasChanged(); } finally { _WheelDrive.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="WheelDrive"/> property value changes.</summary>
		//public event Action<VehicleType> WheelDriveChanged;
		//ReferenceField<WheelDriveEnum> _WheelDrive = WheelDriveEnum.Front;

		////!!!!default
		///// <summary>
		///// The physics material friction of the wheels.
		///// </summary>
		////[Category( "Configuration" )]
		//[DefaultValue( 2.0 )]
		//[Category( "Wheel" )]
		//public Reference<double> WheelFriction
		//{
		//	get { if( _wheelFriction.BeginGet() ) WheelFriction = _wheelFriction.Get( this ); return _wheelFriction.value; }
		//	set { if( _wheelFriction.BeginSet( this, ref value ) ) { try { WheelFrictionChanged?.Invoke( this ); DataWasChanged(); } finally { _wheelFriction.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="WheelFriction"/> property value changes.</summary>
		//public event Action<VehicleType> WheelFrictionChanged;
		//ReferenceField<double> _wheelFriction = 2.0;

		///// <summary>
		///// The range of suspension travel.
		///// </summary>
		////[Category( "Configuration" )]
		//[DefaultValue( "-0.1 0.05" )]
		//[Category( "UPDATE" )]
		//public Reference<Range> SuspensionTravel
		//{
		//	get { if( _suspensionTravel.BeginGet() ) SuspensionTravel = _suspensionTravel.Get( this ); return _suspensionTravel.value; }
		//	set { if( _suspensionTravel.BeginSet( this, ref value ) ) { try { SuspensionTravelChanged?.Invoke( this ); DataWasChanged(); } finally { _suspensionTravel.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SuspensionTravel"/> property value changes.</summary>
		//public event Action<VehicleType> SuspensionTravelChanged;
		//ReferenceField<Range> _suspensionTravel = new Range( -0.1, 0.05 );

		////!!!!default
		///// <summary>
		///// The spring rate of the chassis.
		///// </summary>
		////[Category( "Configuration" )]
		//[DefaultValue( 1.0 )]
		//[Category( "UPDATE" )]
		//public Reference<double> SpringRate
		//{
		//	get { if( _springRate.BeginGet() ) SpringRate = _springRate.Get( this ); return _springRate.value; }
		//	set { if( _springRate.BeginSet( this, ref value ) ) { try { SpringRateChanged?.Invoke( this ); } finally { _springRate.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SpringRate"/> property value changes.</summary>
		//public event Action<VehicleType> SpringRateChanged;
		//ReferenceField<double> _springRate = 1.0;

		///// <summary>
		///// The maximal target velocity of throttle.
		///// </summary>
		////[Category( "Configuration" )]
		//[DefaultValue( 30.0 )]
		//[Category( "UPDATE" )]
		//public Reference<double> ThrottleTargetVelocity
		//{
		//	get { if( _throttleTargetVelocity.BeginGet() ) ThrottleTargetVelocity = _throttleTargetVelocity.Get( this ); return _throttleTargetVelocity.value; }
		//	set { if( _throttleTargetVelocity.BeginSet( this, ref value ) ) { try { ThrottleTargetVelocityChanged?.Invoke( this ); } finally { _throttleTargetVelocity.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ThrottleTargetVelocity"/> property value changes.</summary>
		//public event Action<VehicleType> ThrottleTargetVelocityChanged;
		//ReferenceField<double> _throttleTargetVelocity = 30.0;

		////!!!!
		////!!!!default
		///// <summary>
		///// The maximal physical force of throttle.
		///// </summary>
		////[Category( "Configuration" )]
		//[DefaultValue( 10.0 )]
		//[Category( "UPDATE" )]
		//public Reference<double> ThrottleForce
		//{
		//	get { if( _throttleForce.BeginGet() ) ThrottleForce = _throttleForce.Get( this ); return _throttleForce.value; }
		//	set { if( _throttleForce.BeginSet( this, ref value ) ) { try { ThrottleForceChanged?.Invoke( this ); } finally { _throttleForce.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ThrottleForce"/> property value changes.</summary>
		//public event Action<VehicleType> ThrottleForceChanged;
		//ReferenceField<double> _throttleForce = 10.0;

		////!!!!default
		///// <summary>
		///// The maximal brake force.
		///// </summary>
		////[Category( "Configuration" )]
		//[DefaultValue( 10.0 )]
		//public Reference<double> BrakeForce
		//{
		//	get { if( _brakeForce.BeginGet() ) BrakeForce = _brakeForce.Get( this ); return _brakeForce.value; }
		//	set { if( _brakeForce.BeginSet( this, ref value ) ) { try { BrakeForceChanged?.Invoke( this ); } finally { _brakeForce.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="BrakeForce"/> property value changes.</summary>
		//public event Action<VehicleType> BrakeForceChanged;
		//ReferenceField<double> _brakeForce = 10.0;


		////!!!!default
		///// <summary>
		///// The maximal steering force.
		///// </summary>
		////[Category( "Configuration" )]
		//[DefaultValue( 10.0 )]
		//[Category( "UPDATE" )]
		//public Reference<double> SteeringForce
		//{
		//	get { if( _steeringForce.BeginGet() ) SteeringForce = _steeringForce.Get( this ); return _steeringForce.value; }
		//	set { if( _steeringForce.BeginSet( this, ref value ) ) { try { SteeringForceChanged?.Invoke( this ); } finally { _steeringForce.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SteeringForce"/> property value changes.</summary>
		//public event Action<VehicleType> SteeringForceChanged;
		//ReferenceField<double> _steeringForce = 10.0;


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

		//!!!!"Engine"

		//LinearCurve mNormalizedTorque;                          ///< Curve that describes a ratio of the max torque the engine can produce vs the fraction of the max RPM of the engine
		//float mInertia = 0.5f;                          ///< Moment of inertia (kg m^2) of the engine
		//float mAngularDamping = 0.2f;                       ///< Angular damping factor of the wheel: dw/dt = -c * w

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
		/// <summary>Occurs when the <see cref="SpecialEffects"/> property value changes.</summary>
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
		/// <summary>Occurs when the <see cref="SpecialEffects"/> property value changes.</summary>
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

		[Category( "Sound" )]
		[DefaultValue( null )]
		public Reference<Sound> MotorOnSound
		{
			get { if( _motorOnSound.BeginGet() ) MotorOnSound = _motorOnSound.Get( this ); return _motorOnSound.value; }
			set { if( _motorOnSound.BeginSet( this, ref value ) ) { try { MotorOnSoundChanged?.Invoke( this ); } finally { _motorOnSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MotorOnSound"/> property value changes.</summary>
		public event Action<VehicleType> MotorOnSoundChanged;
		ReferenceField<Sound> _motorOnSound = null;

		[Category( "Sound" )]
		[DefaultValue( null )]
		public Reference<Sound> MotorOffSound
		{
			get { if( _motorOffSound.BeginGet() ) MotorOffSound = _motorOffSound.Get( this ); return _motorOffSound.value; }
			set { if( _motorOffSound.BeginSet( this, ref value ) ) { try { MotorOffSoundChanged?.Invoke( this ); } finally { _motorOffSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MotorOffSound"/> property value changes.</summary>
		public event Action<VehicleType> MotorOffSoundChanged;
		ReferenceField<Sound> _motorOffSound = null;

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

		//!!!!can be several turrets
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

		[Serialize]
		[Browsable( false )]
		public Transform EditorCameraTransform;

		/////////////////////////////////////////

		public enum ChassisEnum
		{
			None,

			[DisplayNameEnum( "4 Wheels" )]
			_4Wheels,

			//Tracks,
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
			_frontWheelLongitudinalFriction = new ReferenceList<CurvePoint1F>( this, delegate () { FrontWheelLongitudinalFrictionChanged?.Invoke( this ); DataWasChanged(); } );
			_frontWheelLateralFriction = new ReferenceList<CurvePoint1F>( this, delegate () { FrontWheelLateralFrictionChanged?.Invoke( this ); DataWasChanged(); } );
			_rearWheelLongitudinalFriction = new ReferenceList<CurvePoint1F>( this, delegate () { RearWheelLongitudinalFrictionChanged?.Invoke( this ); DataWasChanged(); } );
			_rearWheelLateralFriction = new ReferenceList<CurvePoint1F>( this, delegate () { RearWheelLateralFrictionChanged?.Invoke( this ); DataWasChanged(); } );
			_gearSounds = new ReferenceList<Sound>( this, delegate () { GearSoundsChanged?.Invoke( this ); DataWasChanged(); } );
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( FrontWheelPosition ):
				case nameof( FrontWheelDiameter ):
				case nameof( FrontWheelWidth ):
				case nameof( FrontWheelMass ):
				case nameof( FrontWheelMesh ):
				case nameof( FrontWheelSuspensionMinLength ):
				case nameof( FrontWheelSuspensionMaxLength ):
				case nameof( FrontWheelSuspensionPreloadLength ):
				case nameof( FrontWheelSuspensionFrequency ):
				case nameof( FrontWheelSuspensionDamping ):
				case nameof( FrontWheelLongitudinalFriction ):
				case nameof( FrontWheelLateralFriction ):
				case nameof( FrontWheelDrive ):
				case nameof( FrontWheelAntiRollBarStiffness ):
				case nameof( FrontWheelMaxBrakeTorque ):
				case nameof( FrontWheelMaxHandBrakeTorque ):
				case nameof( FrontWheelAngularDamping ):
				case nameof( FrontWheelMaxSteeringAngle ):
				case nameof( FrontWheelSteeringTime ):
				case nameof( RearWheelPosition ):
				case nameof( RearWheelDiameter ):
				case nameof( RearWheelWidth ):
				case nameof( RearWheelMass ):
				case nameof( RearWheelMesh ):
				case nameof( RearWheelSuspensionMinLength ):
				case nameof( RearWheelSuspensionMaxLength ):
				case nameof( RearWheelSuspensionPreloadLength ):
				case nameof( RearWheelSuspensionFrequency ):
				case nameof( RearWheelSuspensionDamping ):
				case nameof( RearWheelLongitudinalFriction ):
				case nameof( RearWheelLateralFriction ):
				case nameof( RearWheelDrive ):
				case nameof( RearWheelAntiRollBarStiffness ):
				case nameof( RearWheelMaxBrakeTorque ):
				case nameof( RearWheelMaxHandBrakeTorque ):
				case nameof( RearWheelAngularDamping ):
				case nameof( RearWheelMaxSteeringAngle ):
					if( Chassis.Value != ChassisEnum._4Wheels )
						skip = true;
					break;

				case nameof( FrontWheelDifferentialRatio ):
				case nameof( FrontWheelDifferentialLeftRightSplit ):
				case nameof( FrontWheelDifferentialLimitedSlipRatio ):
				case nameof( FrontWheelDifferentialEngineTorqueRatio ):
					if( Chassis.Value != ChassisEnum._4Wheels || !FrontWheelDrive )
						skip = true;
					break;

				case nameof( RearWheelDifferentialRatio ):
				case nameof( RearWheelDifferentialLeftRightSplit ):
				case nameof( RearWheelDifferentialLimitedSlipRatio ):
				case nameof( RearWheelDifferentialEngineTorqueRatio ):
					if( Chassis.Value != ChassisEnum._4Wheels || !RearWheelDrive )
						skip = true;
					break;

				case nameof( TransmissionSwitchTime ):
				case nameof( TransmissionClutchReleaseTime ):
				case nameof( TransmissionSwitchLatency ):
				case nameof( TransmissionShiftUpRPM ):
				case nameof( TransmissionShiftDownRPM ):
					//case nameof( TransmissionClutchStrength ):
					if( !TransmissionAuto )
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
				if( FrontWheelLongitudinalFriction.Count == 0 )
				{
					FrontWheelLongitudinalFriction.Add( new CurvePoint1F( 0, 0 ) );
					FrontWheelLongitudinalFriction.Add( new CurvePoint1F( 0.06f, 1.2f ) );
					FrontWheelLongitudinalFriction.Add( new CurvePoint1F( 0.2f, 1.0f ) );
				}

				if( FrontWheelLateralFriction.Count == 0 )
				{
					FrontWheelLateralFriction.Add( new CurvePoint1F( 0, 0 ) );
					FrontWheelLateralFriction.Add( new CurvePoint1F( 3.0f, 1.2f ) );
					FrontWheelLateralFriction.Add( new CurvePoint1F( 20.0f, 1.0f ) );
				}

				if( RearWheelLongitudinalFriction.Count == 0 )
				{
					RearWheelLongitudinalFriction.Add( new CurvePoint1F( 0, 0 ) );
					RearWheelLongitudinalFriction.Add( new CurvePoint1F( 0.06f, 1.2f ) );
					RearWheelLongitudinalFriction.Add( new CurvePoint1F( 0.2f, 1.0f ) );
				}

				if( RearWheelLateralFriction.Count == 0 )
				{
					RearWheelLateralFriction.Add( new CurvePoint1F( 0, 0 ) );
					RearWheelLateralFriction.Add( new CurvePoint1F( 3.0f, 1.2f ) );
					RearWheelLateralFriction.Add( new CurvePoint1F( 20.0f, 1.0f ) );
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

			if( Chassis.Value == ChassisEnum._4Wheels )
			{
				if( FrontWheelMesh.Value != null )
				{
					result.Add( new GetMeshesInDefaultStateItem( FrontWheelMesh.Value, new Transform( FrontWheelPosition ) ) );
					result.Add( new GetMeshesInDefaultStateItem( FrontWheelMesh.Value, new Transform( FrontWheelPosition * new Vector3( 1, -1, 1 ), Quaternion.FromRotateByZ( Math.PI ) ) ) );
				}

				if( RearWheelMesh.Value != null )
				{
					result.Add( new GetMeshesInDefaultStateItem( RearWheelMesh.Value, new Transform( RearWheelPosition ) ) );
					result.Add( new GetMeshesInDefaultStateItem( RearWheelMesh.Value, new Transform( RearWheelPosition * new Vector3( 1, -1, 1 ), Quaternion.FromRotateByZ( Math.PI ) ) ) );
				}
			}

			return result.ToArray();
		}

		public override void NewObjectSetDefaultConfiguration( bool createdFromNewObjectWindow = false )
		{
			if( Components.Count == 0 )
			{
				var seat = CreateComponent<SeatItem>();
				seat.Name = "Vehicle Seat";
				seat.ExitTransform = new Transform( new Vector3( 0, 2, 1 ), Quaternion.Identity, Vector3.One );
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
	}
}
