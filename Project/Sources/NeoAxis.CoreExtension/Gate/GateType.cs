// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A definition of the gate type.
	/// </summary>
	[ResourceFileExtension( "gatetype" )]
	[NewObjectDefaultName( "Gate Type" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Gate\Gate Type", 620 )]
	[EditorControl( typeof( GateTypeEditor ) )]
	[Preview( typeof( GateTypePreview ) )]
	[PreviewImage( typeof( GateTypePreviewImage ) )]
#endif
	public class GateType : Component
	{
		const string defaultBaseMesh = @"Content\Gates\Default\Base.mesh"; //@"Content\Gates\Default\Base.gltf|$Mesh";

		/// <summary>
		/// The mesh of the base.
		/// </summary>
		[DefaultValueReference( defaultBaseMesh )]
		public Reference<Mesh> BaseMesh
		{
			get { if( _baseMesh.BeginGet() ) BaseMesh = _baseMesh.Get( this ); return _baseMesh.value; }
			set { if( _baseMesh.BeginSet( this, ref value ) ) { try { BaseMeshChanged?.Invoke( this ); DataWasChanged(); } finally { _baseMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BaseMesh"/> property value changes.</summary>
		public event Action<GateType> BaseMeshChanged;
		ReferenceField<Mesh> _baseMesh = new Reference<Mesh>( null, defaultBaseMesh );

		public enum OpeningTypeEnum
		{
			SliderHorizontal,
			SliderVertical,
			HingeSide,
			HingeUp,
		}

		[DefaultValue( OpeningTypeEnum.SliderHorizontal )]
		public Reference<OpeningTypeEnum> OpeningType
		{
			get { if( _openingType.BeginGet() ) OpeningType = _openingType.Get( this ); return _openingType.value; }
			set { if( _openingType.BeginSet( this, ref value ) ) { try { OpeningTypeChanged?.Invoke( this ); DataWasChanged(); } finally { _openingType.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OpeningType"/> property value changes.</summary>
		public event Action<GateType> OpeningTypeChanged;
		ReferenceField<OpeningTypeEnum> _openingType = OpeningTypeEnum.SliderHorizontal;


		const string defaultDoor1Mesh = @"Content\Gates\Default\Door.mesh";

		/// <summary>
		/// The mesh of the door 1.
		/// </summary>
		[DefaultValueReference( defaultDoor1Mesh )]
		[DisplayName( "Door 1 Mesh" )]
		public Reference<Mesh> Door1Mesh
		{
			get { if( _door1Mesh.BeginGet() ) Door1Mesh = _door1Mesh.Get( this ); return _door1Mesh.value; }
			set { if( _door1Mesh.BeginSet( this, ref value ) ) { try { Door1MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _door1Mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Door1Mesh"/> property value changes.</summary>
		public event Action<GateType> Door1MeshChanged;
		ReferenceField<Mesh> _door1Mesh = new Reference<Mesh>( null, defaultDoor1Mesh );

		/// <summary>
		/// The position offset for the door 1.
		/// </summary>
		[DefaultValue( "-0.5 0 1.125" )]
		[DisplayName( "Door 1 Position" )]
		public Reference<Vector3> Door1Position
		{
			get { if( _door1Position.BeginGet() ) Door1Position = _door1Position.Get( this ); return _door1Position.value; }
			set { if( _door1Position.BeginSet( this, ref value ) ) { try { Door1PositionChanged?.Invoke( this ); DataWasChanged(); } finally { _door1Position.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Door1Position"/> property value changes.</summary>
		public event Action<GateType> Door1PositionChanged;
		ReferenceField<Vector3> _door1Position = new Vector3( -0.5, 0, 1.125 );

		/// <summary>
		/// The constraint position for the door 1.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[DisplayName( "Door 1 Constraint Position" )]
		public Reference<Vector3> Door1ConstraintPosition
		{
			get { if( _door1ConstraintPosition.BeginGet() ) Door1ConstraintPosition = _door1ConstraintPosition.Get( this ); return _door1ConstraintPosition.value; }
			set { if( _door1ConstraintPosition.BeginSet( this, ref value ) ) { try { Door1ConstraintPositionChanged?.Invoke( this ); DataWasChanged(); } finally { _door1ConstraintPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Door1ConstraintPosition"/> property value changes.</summary>
		public event Action<GateType> Door1ConstraintPositionChanged;
		ReferenceField<Vector3> _door1ConstraintPosition = Vector3.Zero;


		const string defaultDoor2Mesh = @"Content\Gates\Default\Door.mesh";

		/// <summary>
		/// The mesh of the door 2.
		/// </summary>
		[DefaultValueReference( defaultDoor2Mesh )]
		[DisplayName( "Door 2 Mesh" )]
		public Reference<Mesh> Door2Mesh
		{
			get { if( _door2Mesh.BeginGet() ) Door2Mesh = _door2Mesh.Get( this ); return _door2Mesh.value; }
			set { if( _door2Mesh.BeginSet( this, ref value ) ) { try { Door2MeshChanged?.Invoke( this ); DataWasChanged(); } finally { _door2Mesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Door2Mesh"/> property value changes.</summary>
		public event Action<GateType> Door2MeshChanged;
		ReferenceField<Mesh> _door2Mesh = new Reference<Mesh>( null, defaultDoor1Mesh );

		/// <summary>
		/// The offset for the door 2.
		/// </summary>
		[DefaultValue( "0.5 0 1.125" )]
		[DisplayName( "Door 2 Position" )]
		public Reference<Vector3> Door2Position
		{
			get { if( _door2Position.BeginGet() ) Door2Position = _door2Position.Get( this ); return _door2Position.value; }
			set { if( _door2Position.BeginSet( this, ref value ) ) { try { Door2PositionChanged?.Invoke( this ); DataWasChanged(); } finally { _door2Position.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Door2Position"/> property value changes.</summary>
		public event Action<GateType> Door2PositionChanged;
		ReferenceField<Vector3> _door2Position = new Vector3( 0.5, 0, 1.125 );

		/// <summary>
		/// The constraint position for the door 2.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[DisplayName( "Door 2 Constraint Position" )]
		public Reference<Vector3> Door2ConstraintPosition
		{
			get { if( _door2ConstraintPosition.BeginGet() ) Door2ConstraintPosition = _door2ConstraintPosition.Get( this ); return _door2ConstraintPosition.value; }
			set { if( _door2ConstraintPosition.BeginSet( this, ref value ) ) { try { Door2ConstraintPositionChanged?.Invoke( this ); DataWasChanged(); } finally { _door2ConstraintPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Door2ConstraintPosition"/> property value changes.</summary>
		public event Action<GateType> Door2ConstraintPositionChanged;
		ReferenceField<Vector3> _door2ConstraintPosition = Vector3.Zero;

		/// <summary>
		/// The constraint limits for doors component. Only for the slider mode.
		/// </summary>
		[DefaultValue( "0 1.3" )]
		public Reference<Range> DoorLinearLimit
		{
			get { if( _doorLinearLimit.BeginGet() ) DoorLinearLimit = _doorLinearLimit.Get( this ); return _doorLinearLimit.value; }
			set { if( _doorLinearLimit.BeginSet( this, ref value ) ) { try { DoorLinearLimitChanged?.Invoke( this ); DataWasChanged(); } finally { _doorLinearLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DoorLinearLimit"/> property value changes.</summary>
		public event Action<GateType> DoorLinearLimitChanged;
		ReferenceField<Range> _doorLinearLimit = new Range( 0, 1.3 );

		/// <summary>
		/// The offset for doors component when they are open. Only for the slider mode.
		/// </summary>
		[DefaultValue( -1.05 )]
		public Reference<double> DoorOpenOffset
		{
			get { if( _doorOpenOffset.BeginGet() ) DoorOpenOffset = _doorOpenOffset.Get( this ); return _doorOpenOffset.value; }
			set { if( _doorOpenOffset.BeginSet( this, ref value ) ) { try { DoorOpenOffsetChanged?.Invoke( this ); DataWasChanged(); } finally { _doorOpenOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DoorOpenOffset"/> property value changes.</summary>
		public event Action<GateType> DoorOpenOffsetChanged;
		ReferenceField<double> _doorOpenOffset = -1.05;

		/// <summary>
		/// The constraint limits for doors component. Only for the hinge mode.
		/// </summary>
		[DefaultValue( "-130 130" )]//[DefaultValue( "-90 90" )]
		public Reference<Range> DoorAngularLimit
		{
			get { if( _doorAngularLimit.BeginGet() ) DoorAngularLimit = _doorAngularLimit.Get( this ); return _doorAngularLimit.value; }
			set { if( _doorAngularLimit.BeginSet( this, ref value ) ) { try { DoorAngularLimitChanged?.Invoke( this ); } finally { _doorAngularLimit.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DoorAngularLimit"/> property value changes.</summary>
		public event Action<GateType> DoorAngularLimitChanged;
		ReferenceField<Range> _doorAngularLimit = new Range( -130.0, 130.0 );// new Range( -90.0, 90.0 );

		/// <summary>
		/// The angle for doors component when they are open. Only for the hinge mode.
		/// </summary>
		[DefaultValue( 110.0 )]//[DefaultValue( 90.0 )]
		public Reference<Degree> DoorOpenAngle
		{
			get { if( _doorOpenAngle.BeginGet() ) DoorOpenAngle = _doorOpenAngle.Get( this ); return _doorOpenAngle.value; }
			set { if( _doorOpenAngle.BeginSet( this, ref value ) ) { try { DoorOpenAngleChanged?.Invoke( this ); DataWasChanged(); } finally { _doorOpenAngle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DoorOpenAngle"/> property value changes.</summary>
		public event Action<GateType> DoorOpenAngleChanged;
		ReferenceField<Degree> _doorOpenAngle = new Degree( 110 );//30 );

		[DefaultValue( 2.0 )]
		public Reference<double> DoorMotorFrequency
		{
			get { if( _doorMotorFrequency.BeginGet() ) DoorMotorFrequency = _doorMotorFrequency.Get( this ); return _doorMotorFrequency.value; }
			set { if( _doorMotorFrequency.BeginSet( this, ref value ) ) { try { DoorMotorFrequencyChanged?.Invoke( this ); DataWasChanged(); } finally { _doorMotorFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DoorMotorFrequency"/> property value changes.</summary>
		public event Action<GateType> DoorMotorFrequencyChanged;
		ReferenceField<double> _doorMotorFrequency = 2.0;

		[DefaultValue( 1.0 )]
		public Reference<double> DoorMotorDamping
		{
			get { if( _doorMotorDamping.BeginGet() ) DoorMotorDamping = _doorMotorDamping.Get( this ); return _doorMotorDamping.value; }
			set { if( _doorMotorDamping.BeginSet( this, ref value ) ) { try { DoorMotorDampingChanged?.Invoke( this ); DataWasChanged(); } finally { _doorMotorDamping.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DoorMotorDamping"/> property value changes.</summary>
		public event Action<GateType> DoorMotorDampingChanged;
		ReferenceField<double> _doorMotorDamping = 1.0;

		/// <summary>
		/// The time to open, close the door when the body is Kinematic or Static.
		/// </summary>
		[DefaultValue( 1.0 )]
		public Reference<double> DoorOpeningTime
		{
			get { if( _doorOpeningTime.BeginGet() ) DoorOpeningTime = _doorOpeningTime.Get( this ); return _doorOpeningTime.value; }
			set { if( _doorOpeningTime.BeginSet( this, ref value ) ) { try { DoorOpeningTimeChanged?.Invoke( this ); } finally { _doorOpeningTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DoorOpeningTime"/> property value changes.</summary>
		public event Action<GateType> DoorOpeningTimeChanged;
		ReferenceField<double> _doorOpeningTime = 1.0;


		//!!!!SoundOpenStart, SoundOpenEnd, SoundCloseStart, SoundCloseEnd

		const string defaultOpenSound = @"Content\Gates\Default\Sounds\Open.ogg";

		/// <summary>
		/// The sound played when the gate begins opening.
		/// </summary>
		[DefaultValueReference( defaultOpenSound )]
		public Reference<Sound> OpenSound
		{
			get { if( _openSound.BeginGet() ) OpenSound = _openSound.Get( this ); return _openSound.value; }
			set { if( _openSound.BeginSet( this, ref value ) ) { try { OpenSoundChanged?.Invoke( this ); } finally { _openSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OpenSound"/> property value changes.</summary>
		public event Action<GateType> OpenSoundChanged;
		ReferenceField<Sound> _openSound = new Reference<Sound>( null, defaultOpenSound );

		const string defaultCloseSound = @"Content\Gates\Default\Sounds\Close.ogg";

		/// <summary>
		/// The sound played when the gate begins closing.
		/// </summary>
		[DefaultValueReference( defaultCloseSound )]
		public Reference<Sound> CloseSound
		{
			get { if( _closeSound.BeginGet() ) CloseSound = _closeSound.Get( this ); return _closeSound.value; }
			set { if( _closeSound.BeginSet( this, ref value ) ) { try { CloseSoundChanged?.Invoke( this ); } finally { _closeSound.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CloseSound"/> property value changes.</summary>
		public event Action<GateType> CloseSoundChanged;
		ReferenceField<Sound> _closeSound = new Reference<Sound>( null, defaultCloseSound );

		///////////////////////////////////////////////

		[Serialize]
		[Browsable( false )]
		public Transform EditorCameraTransform;

		///////////////////////////////////////////////

		int version;

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

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Door1Position ):
					if( !Door1Mesh.ReferenceSpecified )
						skip = true;
					break;
				case nameof( Door2Position ):
					if( !Door2Mesh.ReferenceSpecified )
						skip = true;
					break;

				case nameof( Door1ConstraintPosition ):
					if( OpeningType.Value != OpeningTypeEnum.HingeSide && OpeningType.Value != OpeningTypeEnum.HingeUp )
						skip = true;
					if( !Door1Mesh.ReferenceSpecified )
						skip = true;
					break;
				case nameof( Door2ConstraintPosition ):
					if( OpeningType.Value != OpeningTypeEnum.HingeSide && OpeningType.Value != OpeningTypeEnum.HingeUp )
						skip = true;
					if( !Door2Mesh.ReferenceSpecified )
						skip = true;
					break;

				case nameof( DoorLinearLimit ):
				case nameof( DoorOpenOffset ):
					if( OpeningType.Value != OpeningTypeEnum.SliderHorizontal && OpeningType.Value != OpeningTypeEnum.SliderVertical )
						skip = true;
					break;

				case nameof( DoorAngularLimit ):
				case nameof( DoorOpenAngle ):
					if( OpeningType.Value != OpeningTypeEnum.HingeSide && OpeningType.Value != OpeningTypeEnum.HingeUp )
						skip = true;
					break;

					//case nameof( DoorOpeningTime ):
					//	break;
				}
			}
		}
	}
}
