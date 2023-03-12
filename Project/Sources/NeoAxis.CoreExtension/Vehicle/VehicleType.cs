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
	[EditorControl( typeof( VehicleTypeEditor ) )]
	[Preview( typeof( VehicleTypePreview ) )]
	[PreviewImage( typeof( VehicleTypePreviewImage ) )]
#endif
	public class VehicleType : Component
	{
		int version;

		//

		//!!!!
		//DataWasChanged()


		const string meshDefault = @"Content\Vehicles\Default\Body.obj|$Mesh";

		/// <summary>
		/// The main mesh of the vehicle.
		/// </summary>
		[DefaultValueReference( meshDefault )]
		[Category( "Common" )]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set { if( _mesh.BeginSet( ref value ) ) { try { MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _mesh.EndSet(); } } }
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
			set { if( _chassis.BeginSet( ref value ) ) { try { ChassisChanged?.Invoke( this ); DataWasChanged(); } finally { _chassis.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Chassis"/> property value changes.</summary>
		public event Action<VehicleType> ChassisChanged;
		ReferenceField<ChassisEnum> _chassis = ChassisEnum._4Wheels;

		/// <summary>
		/// The position offset for front wheels.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( "1.24 0.7 0.03" )]
		[Category( "Front Wheel" )]
		public Reference<Vector3> FrontWheelPosition
		{
			get { if( _frontWheelPosition.BeginGet() ) FrontWheelPosition = _frontWheelPosition.Get( this ); return _frontWheelPosition.value; }
			set { if( _frontWheelPosition.BeginSet( ref value ) ) { try { FrontWheelPositionChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelPosition"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelPositionChanged;
		ReferenceField<Vector3> _frontWheelPosition = new Vector3( 1.24, 0.7, 0.03 );

		/// <summary>
		/// The diameter of a front wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( 0.665 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelDiameter
		{
			get { if( _frontWheelDiameter.BeginGet() ) FrontWheelDiameter = _frontWheelDiameter.Get( this ); return _frontWheelDiameter.value; }
			set { if( _frontWheelDiameter.BeginSet( ref value ) ) { try { FrontWheelDiameterChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelDiameter.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelDiameter"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelDiameterChanged;
		ReferenceField<double> _frontWheelDiameter = 0.665;

		/// <summary>
		/// The width of a front wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( 0.364 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelWidth
		{
			get { if( _frontWheelWidth.BeginGet() ) FrontWheelWidth = _frontWheelWidth.Get( this ); return _frontWheelWidth.value; }
			set { if( _frontWheelWidth.BeginSet( ref value ) ) { try { FrontWheelWidthChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelWidth"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelWidthChanged;
		ReferenceField<double> _frontWheelWidth = 0.364;

		/// <summary>
		/// The mass of a front wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( 18.0 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelMass
		{
			get { if( _frontWheelMass.BeginGet() ) FrontWheelMass = _frontWheelMass.Get( this ); return _frontWheelMass.value; }
			set { if( _frontWheelMass.BeginSet( ref value ) ) { try { FrontWheelMassChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelMass.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelMass"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelMassChanged;
		ReferenceField<double> _frontWheelMass = 18.0;

		const string wheelMeshDefault = @"Content\Vehicles\Default\Wheel.obj|$Mesh";

		/// <summary>
		/// The mesh of a front wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValueReference( wheelMeshDefault )]
		[Category( "Front Wheel" )]
		public Reference<Mesh> FrontWheelMesh
		{
			get { if( _frontWheelMesh.BeginGet() ) FrontWheelMesh = _frontWheelMesh.Get( this ); return _frontWheelMesh.value; }
			set { if( _frontWheelMesh.BeginSet( ref value ) ) { try { FrontWheelMeshChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelMesh"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelMeshChanged;
		ReferenceField<Mesh> _frontWheelMesh = new Reference<Mesh>( null, wheelMeshDefault );

		[DefaultValue( 0.3 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelSuspensionMinLength
		{
			get { if( _frontWheelSuspensionMinLength.BeginGet() ) FrontWheelSuspensionMinLength = _frontWheelSuspensionMinLength.Get( this ); return _frontWheelSuspensionMinLength.value; }
			set { if( _frontWheelSuspensionMinLength.BeginSet( ref value ) ) { try { FrontWheelSuspensionMinLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelSuspensionMinLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelSuspensionMinLength"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelSuspensionMinLengthChanged;
		ReferenceField<double> _frontWheelSuspensionMinLength = 0.3;

		[DefaultValue( 0.5 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelSuspensionMaxLength
		{
			get { if( _frontWheelSuspensionMaxLength.BeginGet() ) FrontWheelSuspensionMaxLength = _frontWheelSuspensionMaxLength.Get( this ); return _frontWheelSuspensionMaxLength.value; }
			set { if( _frontWheelSuspensionMaxLength.BeginSet( ref value ) ) { try { FrontWheelSuspensionMaxLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelSuspensionMaxLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelSuspensionMaxLength"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelSuspensionMaxLengthChanged;
		ReferenceField<double> _frontWheelSuspensionMaxLength = 0.5;

		[DefaultValue( 0.0 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelSuspensionPreloadLength
		{
			get { if( _frontWheelSuspensionPreloadLength.BeginGet() ) FrontWheelSuspensionPreloadLength = _frontWheelSuspensionPreloadLength.Get( this ); return _frontWheelSuspensionPreloadLength.value; }
			set { if( _frontWheelSuspensionPreloadLength.BeginSet( ref value ) ) { try { FrontWheelSuspensionPreloadLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelSuspensionPreloadLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelSuspensionPreloadLength"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelSuspensionPreloadLengthChanged;
		ReferenceField<double> _frontWheelSuspensionPreloadLength = 0.0;

		[DefaultValue( 1.5 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelSuspensionFrequency
		{
			get { if( _frontWheelSuspensionFrequency.BeginGet() ) FrontWheelSuspensionFrequency = _frontWheelSuspensionFrequency.Get( this ); return _frontWheelSuspensionFrequency.value; }
			set { if( _frontWheelSuspensionFrequency.BeginSet( ref value ) ) { try { FrontWheelSuspensionFrequencyChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelSuspensionFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelSuspensionFrequency"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelSuspensionFrequencyChanged;
		ReferenceField<double> _frontWheelSuspensionFrequency = 1.5;

		[DefaultValue( 0.5 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelSuspensionDamping
		{
			get { if( _frontWheelSuspensionDamping.BeginGet() ) FrontWheelSuspensionDamping = _frontWheelSuspensionDamping.Get( this ); return _frontWheelSuspensionDamping.value; }
			set { if( _frontWheelSuspensionDamping.BeginSet( ref value ) ) { try { FrontWheelSuspensionDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelSuspensionDamping.EndSet(); } } }
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
			set { if( _frontWheelAntiRollBarStiffness.BeginSet( ref value ) ) { try { FrontWheelAntiRollBarStiffnessChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelAntiRollBarStiffness.EndSet(); } } }
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
			set { if( _frontWheelMaxBrakeTorque.BeginSet( ref value ) ) { try { FrontWheelMaxBrakeTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelMaxBrakeTorque.EndSet(); } } }
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
			set { if( _frontWheelMaxHandBrakeTorque.BeginSet( ref value ) ) { try { FrontWheelMaxHandBrakeTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelMaxHandBrakeTorque.EndSet(); } } }
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
			set { if( _frontWheelAngularDamping.BeginSet( ref value ) ) { try { FrontWheelAngularDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelAngularDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelAngularDamping"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelAngularDampingChanged;
		ReferenceField<double> _frontWheelAngularDamping = 0.2;

		/// <summary>
		/// The maximal steering angle of front wheels.
		/// </summary>
		[DefaultValue( 45.0 )]
		[Category( "Front Wheel" )]
		public Reference<Degree> FrontWheelMaxSteeringAngle
		{
			get { if( _frontWheelMaxSteeringAngle.BeginGet() ) FrontWheelMaxSteeringAngle = _frontWheelMaxSteeringAngle.Get( this ); return _frontWheelMaxSteeringAngle.value; }
			set { if( _frontWheelMaxSteeringAngle.BeginSet( ref value ) ) { try { FrontWheelMaxSteeringAngleChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelMaxSteeringAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelMaxSteeringAngle"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelMaxSteeringAngleChanged;
		ReferenceField<Degree> _frontWheelMaxSteeringAngle = new Degree( 45.0 );

		[DefaultValue( 0.5 )]
		[Category( "Front Wheel" )]
		public Reference<double> FrontWheelSteeringTime
		{
			get { if( _frontWheelSteeringTime.BeginGet() ) FrontWheelSteeringTime = _frontWheelSteeringTime.Get( this ); return _frontWheelSteeringTime.value; }
			set { if( _frontWheelSteeringTime.BeginSet( ref value ) ) { try { FrontWheelSteeringTimeChanged?.Invoke( this ); } finally { _frontWheelSteeringTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FrontWheelSteeringTime"/> property value changes.</summary>
		public event Action<VehicleType> FrontWheelSteeringTimeChanged;
		ReferenceField<double> _frontWheelSteeringTime = 0.5;

		[DefaultValue( true )]
		[Category( "Front Wheel" )]
		public Reference<bool> FrontWheelDrive
		{
			get { if( _frontWheelDrive.BeginGet() ) FrontWheelDrive = _frontWheelDrive.Get( this ); return _frontWheelDrive.value; }
			set { if( _frontWheelDrive.BeginSet( ref value ) ) { try { FrontWheelDriveChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelDrive.EndSet(); } } }
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
			set { if( _frontWheelDifferentialRatio.BeginSet( ref value ) ) { try { FrontWheelDifferentialRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelDifferentialRatio.EndSet(); } } }
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
			set { if( _frontWheelDifferentialLeftRightSplit.BeginSet( ref value ) ) { try { FrontWheelDifferentialLeftRightSplitChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelDifferentialLeftRightSplit.EndSet(); } } }
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
			set { if( _frontWheelDifferentialLimitedSlipRatio.BeginSet( ref value ) ) { try { FrontWheelDifferentialLimitedSlipRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelDifferentialLimitedSlipRatio.EndSet(); } } }
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
			set { if( _frontWheelDifferentialEngineTorqueRatio.BeginSet( ref value ) ) { try { FrontWheelDifferentialEngineTorqueRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _frontWheelDifferentialEngineTorqueRatio.EndSet(); } } }
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
			set { if( _rearWheelPosition.BeginSet( ref value ) ) { try { RearWheelPositionChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelPosition.EndSet(); } } }
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
			set { if( _rearWheelDiameter.BeginSet( ref value ) ) { try { RearWheelDiameterChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelDiameter.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelDiameter"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelDiameterChanged;
		ReferenceField<double> _rearWheelDiameter = 0.665;

		/// <summary>
		/// The width of a rear wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( 0.364 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelWidth
		{
			get { if( _rearWheelWidth.BeginGet() ) RearWheelWidth = _rearWheelWidth.Get( this ); return _rearWheelWidth.value; }
			set { if( _rearWheelWidth.BeginSet( ref value ) ) { try { RearWheelWidthChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelWidth.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelWidth"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelWidthChanged;
		ReferenceField<double> _rearWheelWidth = 0.364;

		/// <summary>
		/// The mass of a rear wheel.
		/// </summary>
		//[Category( "Configuration" )]
		[DefaultValue( 18.0 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelMass
		{
			get { if( _rearWheelMass.BeginGet() ) RearWheelMass = _rearWheelMass.Get( this ); return _rearWheelMass.value; }
			set { if( _rearWheelMass.BeginSet( ref value ) ) { try { RearWheelMassChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelMass.EndSet(); } } }
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
			set { if( _rearWheelMesh.BeginSet( ref value ) ) { try { RearWheelMeshChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelMesh"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelMeshChanged;
		ReferenceField<Mesh> _rearWheelMesh = new Reference<Mesh>( null, wheelMeshDefault );

		[DefaultValue( 0.3 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelSuspensionMinLength
		{
			get { if( _rearWheelSuspensionMinLength.BeginGet() ) RearWheelSuspensionMinLength = _rearWheelSuspensionMinLength.Get( this ); return _rearWheelSuspensionMinLength.value; }
			set { if( _rearWheelSuspensionMinLength.BeginSet( ref value ) ) { try { RearWheelSuspensionMinLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelSuspensionMinLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelSuspensionMinLength"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelSuspensionMinLengthChanged;
		ReferenceField<double> _rearWheelSuspensionMinLength = 0.3;

		[DefaultValue( 0.5 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelSuspensionMaxLength
		{
			get { if( _rearWheelSuspensionMaxLength.BeginGet() ) RearWheelSuspensionMaxLength = _rearWheelSuspensionMaxLength.Get( this ); return _rearWheelSuspensionMaxLength.value; }
			set { if( _rearWheelSuspensionMaxLength.BeginSet( ref value ) ) { try { RearWheelSuspensionMaxLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelSuspensionMaxLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelSuspensionMaxLength"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelSuspensionMaxLengthChanged;
		ReferenceField<double> _rearWheelSuspensionMaxLength = 0.5;

		[DefaultValue( 0.0 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelSuspensionPreloadLength
		{
			get { if( _rearWheelSuspensionPreloadLength.BeginGet() ) RearWheelSuspensionPreloadLength = _rearWheelSuspensionPreloadLength.Get( this ); return _rearWheelSuspensionPreloadLength.value; }
			set { if( _rearWheelSuspensionPreloadLength.BeginSet( ref value ) ) { try { RearWheelSuspensionPreloadLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelSuspensionPreloadLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelSuspensionPreloadLength"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelSuspensionPreloadLengthChanged;
		ReferenceField<double> _rearWheelSuspensionPreloadLength = 0.0;

		[DefaultValue( 1.5 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelSuspensionFrequency
		{
			get { if( _rearWheelSuspensionFrequency.BeginGet() ) RearWheelSuspensionFrequency = _rearWheelSuspensionFrequency.Get( this ); return _rearWheelSuspensionFrequency.value; }
			set { if( _rearWheelSuspensionFrequency.BeginSet( ref value ) ) { try { RearWheelSuspensionFrequencyChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelSuspensionFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelSuspensionFrequency"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelSuspensionFrequencyChanged;
		ReferenceField<double> _rearWheelSuspensionFrequency = 1.5;

		[DefaultValue( 0.5 )]
		[Category( "Rear Wheel" )]
		public Reference<double> RearWheelSuspensionDamping
		{
			get { if( _rearWheelSuspensionDamping.BeginGet() ) RearWheelSuspensionDamping = _rearWheelSuspensionDamping.Get( this ); return _rearWheelSuspensionDamping.value; }
			set { if( _rearWheelSuspensionDamping.BeginSet( ref value ) ) { try { RearWheelSuspensionDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelSuspensionDamping.EndSet(); } } }
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
			set { if( _rearWheelAntiRollBarStiffness.BeginSet( ref value ) ) { try { RearWheelAntiRollBarStiffnessChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelAntiRollBarStiffness.EndSet(); } } }
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
			set { if( _rearWheelMaxBrakeTorque.BeginSet( ref value ) ) { try { RearWheelMaxBrakeTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelMaxBrakeTorque.EndSet(); } } }
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
			set { if( _rearWheelMaxHandBrakeTorque.BeginSet( ref value ) ) { try { RearWheelMaxHandBrakeTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelMaxHandBrakeTorque.EndSet(); } } }
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
			set { if( _rearWheelAngularDamping.BeginSet( ref value ) ) { try { RearWheelAngularDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelAngularDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="RearWheelAngularDamping"/> property value changes.</summary>
		public event Action<VehicleType> RearWheelAngularDampingChanged;
		ReferenceField<double> _rearWheelAngularDamping = 0.2;

		/// <summary>
		/// The maximal steering angle of rear wheels.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Category( "Rear Wheel" )]
		public Reference<Degree> RearWheelMaxSteeringAngle
		{
			get { if( _rearWheelMaxSteeringAngle.BeginGet() ) RearWheelMaxSteeringAngle = _rearWheelMaxSteeringAngle.Get( this ); return _rearWheelMaxSteeringAngle.value; }
			set { if( _rearWheelMaxSteeringAngle.BeginSet( ref value ) ) { try { RearWheelMaxSteeringAngleChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelMaxSteeringAngle.EndSet(); } } }
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
		//	set { if( _rearWheelSteeringSpeed.BeginSet( ref value ) ) { try { RearWheelSteeringSpeedChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelSteeringSpeed.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RearWheelSteeringSpeed"/> property value changes.</summary>
		//public event Action<VehicleType> RearWheelSteeringSpeedChanged;
		//ReferenceField<Degree> _rearWheelSteeringSpeed = new Degree( 90.0 );

		[DefaultValue( false )]
		[Category( "Rear Wheel" )]
		public Reference<bool> RearWheelDrive
		{
			get { if( _rearWheelDrive.BeginGet() ) RearWheelDrive = _rearWheelDrive.Get( this ); return _rearWheelDrive.value; }
			set { if( _rearWheelDrive.BeginSet( ref value ) ) { try { RearWheelDriveChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelDrive.EndSet(); } } }
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
			set { if( _rearWheelDifferentialRatio.BeginSet( ref value ) ) { try { RearWheelDifferentialRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelDifferentialRatio.EndSet(); } } }
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
			set { if( _rearWheelDifferentialLeftRightSplit.BeginSet( ref value ) ) { try { RearWheelDifferentialLeftRightSplitChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelDifferentialLeftRightSplit.EndSet(); } } }
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
			set { if( _rearWheelDifferentialLimitedSlipRatio.BeginSet( ref value ) ) { try { RearWheelDifferentialLimitedSlipRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelDifferentialLimitedSlipRatio.EndSet(); } } }
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
			set { if( _rearWheelDifferentialEngineTorqueRatio.BeginSet( ref value ) ) { try { RearWheelDifferentialEngineTorqueRatioChanged?.Invoke( this ); DataWasChanged(); } finally { _rearWheelDifferentialEngineTorqueRatio.EndSet(); } } }
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
		//	set { if( _WheelDrive.BeginSet( ref value ) ) { try { WheelDriveChanged?.Invoke( this ); DataWasChanged(); } finally { _WheelDrive.EndSet(); } } }
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
		//	set { if( _wheelFriction.BeginSet( ref value ) ) { try { WheelFrictionChanged?.Invoke( this ); DataWasChanged(); } finally { _wheelFriction.EndSet(); } } }
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
		//	set { if( _suspensionTravel.BeginSet( ref value ) ) { try { SuspensionTravelChanged?.Invoke( this ); DataWasChanged(); } finally { _suspensionTravel.EndSet(); } } }
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
		//	set { if( _springRate.BeginSet( ref value ) ) { try { SpringRateChanged?.Invoke( this ); } finally { _springRate.EndSet(); } } }
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
		//	set { if( _throttleTargetVelocity.BeginSet( ref value ) ) { try { ThrottleTargetVelocityChanged?.Invoke( this ); } finally { _throttleTargetVelocity.EndSet(); } } }
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
		//	set { if( _throttleForce.BeginSet( ref value ) ) { try { ThrottleForceChanged?.Invoke( this ); } finally { _throttleForce.EndSet(); } } }
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
		//	set { if( _brakeForce.BeginSet( ref value ) ) { try { BrakeForceChanged?.Invoke( this ); } finally { _brakeForce.EndSet(); } } }
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
		//	set { if( _steeringForce.BeginSet( ref value ) ) { try { SteeringForceChanged?.Invoke( this ); } finally { _steeringForce.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SteeringForce"/> property value changes.</summary>
		//public event Action<VehicleType> SteeringForceChanged;
		//ReferenceField<double> _steeringForce = 10.0;


		[DefaultValue( 500.0 )]
		[Category( "Engine" )]
		public Reference<double> EngineMaxTorque
		{
			get { if( _engineMaxTorque.BeginGet() ) EngineMaxTorque = _engineMaxTorque.Get( this ); return _engineMaxTorque.value; }
			set { if( _engineMaxTorque.BeginSet( ref value ) ) { try { EngineMaxTorqueChanged?.Invoke( this ); DataWasChanged(); } finally { _engineMaxTorque.EndSet(); } } }
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
			set { if( _engineMinRPM.BeginSet( ref value ) ) { try { EngineMinRPMChanged?.Invoke( this ); DataWasChanged(); } finally { _engineMinRPM.EndSet(); } } }
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
			set { if( _engineMaxRPM.BeginSet( ref value ) ) { try { EngineMaxRPMChanged?.Invoke( this ); DataWasChanged(); } finally { _engineMaxRPM.EndSet(); } } }
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
			set { if( _transmissionAuto.BeginSet( ref value ) ) { try { TransmissionAutoChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionAuto.EndSet(); } } }
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
			set { if( _transmissionGearRatios.BeginSet( ref value ) ) { try { TransmissionGearRatiosChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionGearRatios.EndSet(); } } }
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
			set { if( _transmissionReverseGearRatios.BeginSet( ref value ) ) { try { TransmissionReverseGearRatiosChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionReverseGearRatios.EndSet(); } } }
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
			set { if( _transmissionSwitchTime.BeginSet( ref value ) ) { try { TransmissionSwitchTimeChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionSwitchTime.EndSet(); } } }
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
			set { if( _transmissionClutchReleaseTime.BeginSet( ref value ) ) { try { TransmissionClutchReleaseTimeChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionClutchReleaseTime.EndSet(); } } }
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
			set { if( _transmissionSwitchLatency.BeginSet( ref value ) ) { try { TransmissionSwitchLatencyChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionSwitchLatency.EndSet(); } } }
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
			set { if( _transmissionShiftUpRPM.BeginSet( ref value ) ) { try { TransmissionShiftUpRPMChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionShiftUpRPM.EndSet(); } } }
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
			set { if( _transmissionShiftDownRPM.BeginSet( ref value ) ) { try { TransmissionShiftDownRPMChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionShiftDownRPM.EndSet(); } } }
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
			set { if( _transmissionClutchStrength.BeginSet( ref value ) ) { try { TransmissionClutchStrengthChanged?.Invoke( this ); DataWasChanged(); } finally { _transmissionClutchStrength.EndSet(); } } }
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
			set { if( _maxSlopeAngle.BeginSet( ref value ) ) { try { MaxSlopeAngleChanged?.Invoke( this ); DataWasChanged(); } finally { _maxSlopeAngle.EndSet(); } } }
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
			set { if( _maxPitchRollAngle.BeginSet( ref value ) ) { try { MaxPitchRollAngleChanged?.Invoke( this ); DataWasChanged(); } finally { _maxPitchRollAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaxPitchRollAngle"/> property value changes.</summary>
		public event Action<VehicleType> MaxPitchRollAngleChanged;
		ReferenceField<Degree> _maxPitchRollAngle = new Degree( 180 );

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
				var seat = CreateComponent<VehicleSeat>();
				seat.Name = "Vehicle Seat";
				seat.ExitTransform = new Transform( new Vector3( 0, 2, 1 ), Quaternion.Identity, Vector3.One );
			}
		}
	}
}
