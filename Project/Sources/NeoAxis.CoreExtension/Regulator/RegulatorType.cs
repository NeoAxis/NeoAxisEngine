// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Domaxica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A definition of the regulator in space.
	/// </summary>
	[ResourceFileExtension( "regulatortype" )]
	[NewObjectDefaultName( "Regulator Type" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Regulator\Regulator Type", 10100 )]
	[EditorControl( typeof( RegulatorTypeEditor ) )]
	[Preview( typeof( RegulatorTypePreview ) )]
	[PreviewImage( typeof( RegulatorTypePreviewImage ) )]
#endif
	public class RegulatorType : Component
	{
		/// <summary>
		/// The mesh of the base.
		/// </summary>
		[DefaultValueReference( @"Content\Regulators\Default\Base.mesh" )]
		public Reference<Mesh> BaseMesh
		{
			get { if( _baseMesh.BeginGet() ) BaseMesh = _baseMesh.Get( this ); return _baseMesh.value; }
			set { if( _baseMesh.BeginSet( this, ref value ) ) { try { BaseMeshChanged?.Invoke( this ); } finally { _baseMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BaseMesh"/> property value changes.</summary>
		public event Action<RegulatorType> BaseMeshChanged;
		ReferenceField<Mesh> _baseMesh = new Reference<Mesh>( null, @"Content\Regulators\Default\Base.mesh" );

		/// <summary>
		/// The mesh of the button.
		/// </summary>
		[DefaultValueReference( @"Content\Regulators\Default\Button.mesh" )]
		public Reference<Mesh> ButtonMesh
		{
			get { if( _buttonMesh.BeginGet() ) ButtonMesh = _buttonMesh.Get( this ); return _buttonMesh.value; }
			set { if( _buttonMesh.BeginSet( this, ref value ) ) { try { ButtonMeshChanged?.Invoke( this ); } finally { _buttonMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ButtonMesh"/> property value changes.</summary>
		public event Action<RegulatorType> ButtonMeshChanged;
		ReferenceField<Mesh> _buttonMesh = new Reference<Mesh>( null, @"Content\Regulators\Default\Button.mesh" );

		/// <summary>
		/// The position offset of the button mesh.
		/// </summary>
		[DefaultValue( "0.05 0 0" )]
		public Reference<Vector3> ButtonMeshPosition
		{
			get { if( _buttonMeshPosition.BeginGet() ) ButtonMeshPosition = _buttonMeshPosition.Get( this ); return _buttonMeshPosition.value; }
			set { if( _buttonMeshPosition.BeginSet( this, ref value ) ) { try { ButtonMeshPositionChanged?.Invoke( this ); } finally { _buttonMeshPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ButtonMeshPosition"/> property value changes.</summary>
		public event Action<RegulatorType> ButtonMeshPositionChanged;
		ReferenceField<Vector3> _buttonMeshPosition = new Vector3( 0.05, 0, 0 );


		/// <summary>
		/// The mesh of the max indicator.
		/// </summary>
		[DefaultValueReference( @"Content\Regulators\Default\Indicator.mesh" )]
		public Reference<Mesh> IndicatorMinMesh
		{
			get { if( _indicatorMinMesh.BeginGet() ) IndicatorMinMesh = _indicatorMinMesh.Get( this ); return _indicatorMinMesh.value; }
			set { if( _indicatorMinMesh.BeginSet( this, ref value ) ) { try { IndicatorMinMeshChanged?.Invoke( this ); } finally { _indicatorMinMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndicatorMinMesh"/> property value changes.</summary>
		public event Action<RegulatorType> IndicatorMinMeshChanged;
		ReferenceField<Mesh> _indicatorMinMesh = new Reference<Mesh>( null, @"Content\Regulators\Default\Indicator.mesh" );

		/// <summary>
		/// The position offset of the min indicator mesh.
		/// </summary>
		[DefaultValue( "0.03 -0.11 0.11" )]
		public Reference<Vector3> IndicatorMinMeshPosition
		{
			get { if( _indicatorMinMeshPosition.BeginGet() ) IndicatorMinMeshPosition = _indicatorMinMeshPosition.Get( this ); return _indicatorMinMeshPosition.value; }
			set { if( _indicatorMinMeshPosition.BeginSet( this, ref value ) ) { try { IndicatorMinMeshPositionChanged?.Invoke( this ); } finally { _indicatorMinMeshPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndicatorMinMeshPosition"/> property value changes.</summary>
		public event Action<RegulatorType> IndicatorMinMeshPositionChanged;
		ReferenceField<Vector3> _indicatorMinMeshPosition = new Vector3( 0.03, -0.11, 0.11 );

		/// <summary>
		/// The material of the min indicator in activated state.
		/// </summary>
		[DefaultValueReference( @"Content\Regulators\Default\Indicator min activated.material" )]
		public Reference<Material> IndicatorMinMaterialActivated
		{
			get { if( _indicatorMinMaterialActivated.BeginGet() ) IndicatorMinMaterialActivated = _indicatorMinMaterialActivated.Get( this ); return _indicatorMinMaterialActivated.value; }
			set { if( _indicatorMinMaterialActivated.BeginSet( this, ref value ) ) { try { IndicatorMinMaterialActivatedChanged?.Invoke( this ); } finally { _indicatorMinMaterialActivated.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndicatorMinMaterialActivated"/> property value changes.</summary>
		public event Action<RegulatorType> IndicatorMinMaterialActivatedChanged;
		ReferenceField<Material> _indicatorMinMaterialActivated = new Reference<Material>( null, @"Content\Regulators\Default\Indicator min activated.material" );


		/// <summary>
		/// The mesh of the max indicator.
		/// </summary>
		[DefaultValueReference( @"Content\Regulators\Default\Indicator.mesh" )]
		public Reference<Mesh> IndicatorMaxMesh
		{
			get { if( _indicatorMaxMesh.BeginGet() ) IndicatorMaxMesh = _indicatorMaxMesh.Get( this ); return _indicatorMaxMesh.value; }
			set { if( _indicatorMaxMesh.BeginSet( this, ref value ) ) { try { IndicatorMaxMeshChanged?.Invoke( this ); } finally { _indicatorMaxMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndicatorMaxMesh"/> property value changes.</summary>
		public event Action<RegulatorType> IndicatorMaxMeshChanged;
		ReferenceField<Mesh> _indicatorMaxMesh = new Reference<Mesh>( null, @"Content\Regulators\Default\Indicator.mesh" );

		/// <summary>
		/// The position offset of the max indicator mesh.
		/// </summary>
		[DefaultValue( "0.03 0.11 0.11" )]
		public Reference<Vector3> IndicatorMaxMeshPosition
		{
			get { if( _indicatorMaxMeshPosition.BeginGet() ) IndicatorMaxMeshPosition = _indicatorMaxMeshPosition.Get( this ); return _indicatorMaxMeshPosition.value; }
			set { if( _indicatorMaxMeshPosition.BeginSet( this, ref value ) ) { try { IndicatorMaxMeshPositionChanged?.Invoke( this ); } finally { _indicatorMaxMeshPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndicatorMaxMeshPosition"/> property value changes.</summary>
		public event Action<RegulatorType> IndicatorMaxMeshPositionChanged;
		ReferenceField<Vector3> _indicatorMaxMeshPosition = new Vector3( 0.03, 0.11, 0.11 );

		/// <summary>
		/// The material of the max indicator in activated state.
		/// </summary>
		[DefaultValueReference( @"Content\Regulators\Default\Indicator max activated.material" )]
		public Reference<Material> IndicatorMaxMaterialActivated
		{
			get { if( _indicatorMaxMaterialActivated.BeginGet() ) IndicatorMaxMaterialActivated = _indicatorMaxMaterialActivated.Get( this ); return _indicatorMaxMaterialActivated.value; }
			set { if( _indicatorMaxMaterialActivated.BeginSet( this, ref value ) ) { try { IndicatorMaxMaterialActivatedChanged?.Invoke( this ); } finally { _indicatorMaxMaterialActivated.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndicatorMaxMaterialActivated"/> property value changes.</summary>
		public event Action<RegulatorType> IndicatorMaxMaterialActivatedChanged;
		ReferenceField<Material> _indicatorMaxMaterialActivated = new Reference<Material>( null, @"Content\Regulators\Default\Indicator max activated.material" );


		/// <summary>
		/// The mesh of the max marker.
		/// </summary>
		[DefaultValueReference( @"Content\Regulators\Default\Marker min max.mesh" )]
		public Reference<Mesh> MarkerMinMesh
		{
			get { if( _markerMinMesh.BeginGet() ) MarkerMinMesh = _markerMinMesh.Get( this ); return _markerMinMesh.value; }
			set { if( _markerMinMesh.BeginSet( this, ref value ) ) { try { MarkerMinMeshChanged?.Invoke( this ); } finally { _markerMinMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MarkerMinMesh"/> property value changes.</summary>
		public event Action<RegulatorType> MarkerMinMeshChanged;
		ReferenceField<Mesh> _markerMinMesh = new Reference<Mesh>( null, @"Content\Regulators\Default\Marker min max.mesh" );

		/// <summary>
		/// The position offset of the min marker mesh.
		/// </summary>
		[DefaultValue( "0.01 0 0" )]
		public Reference<Vector3> MarkerMinMeshPosition
		{
			get { if( _markerMinMeshPosition.BeginGet() ) MarkerMinMeshPosition = _markerMinMeshPosition.Get( this ); return _markerMinMeshPosition.value; }
			set { if( _markerMinMeshPosition.BeginSet( this, ref value ) ) { try { MarkerMinMeshPositionChanged?.Invoke( this ); } finally { _markerMinMeshPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MarkerMinMeshPosition"/> property value changes.</summary>
		public event Action<RegulatorType> MarkerMinMeshPositionChanged;
		ReferenceField<Vector3> _markerMinMeshPosition = new Vector3( 0.01, 0, 0 );


		/// <summary>
		/// The mesh of the max marker.
		/// </summary>
		[DefaultValueReference( @"Content\Regulators\Default\Marker min max.mesh" )]
		public Reference<Mesh> MarkerMaxMesh
		{
			get { if( _markerMaxMesh.BeginGet() ) MarkerMaxMesh = _markerMaxMesh.Get( this ); return _markerMaxMesh.value; }
			set { if( _markerMaxMesh.BeginSet( this, ref value ) ) { try { MarkerMaxMeshChanged?.Invoke( this ); } finally { _markerMaxMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MarkerMaxMesh"/> property value changes.</summary>
		public event Action<RegulatorType> MarkerMaxMeshChanged;
		ReferenceField<Mesh> _markerMaxMesh = new Reference<Mesh>( null, @"Content\Regulators\Default\Marker min max.mesh" );

		/// <summary>
		/// The position offset of the max marker mesh.
		/// </summary>
		[DefaultValue( "0.01 0 0" )]
		public Reference<Vector3> MarkerMaxMeshPosition
		{
			get { if( _markerMaxMeshPosition.BeginGet() ) MarkerMaxMeshPosition = _markerMaxMeshPosition.Get( this ); return _markerMaxMeshPosition.value; }
			set { if( _markerMaxMeshPosition.BeginSet( this, ref value ) ) { try { MarkerMaxMeshPositionChanged?.Invoke( this ); } finally { _markerMaxMeshPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MarkerMaxMeshPosition"/> property value changes.</summary>
		public event Action<RegulatorType> MarkerMaxMeshPositionChanged;
		ReferenceField<Vector3> _markerMaxMeshPosition = new Vector3( 0.01, 0, 0 );


		/// <summary>
		/// The mesh of the current marker.
		/// </summary>
		[DefaultValueReference( @"Content\Regulators\Default\Marker current.mesh" )]
		public Reference<Mesh> MarkerCurrentMesh
		{
			get { if( _markerCurrentMesh.BeginGet() ) MarkerCurrentMesh = _markerCurrentMesh.Get( this ); return _markerCurrentMesh.value; }
			set { if( _markerCurrentMesh.BeginSet( this, ref value ) ) { try { MarkerCurrentMeshChanged?.Invoke( this ); } finally { _markerCurrentMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MarkerCurrentMesh"/> property value changes.</summary>
		public event Action<RegulatorType> MarkerCurrentMeshChanged;
		ReferenceField<Mesh> _markerCurrentMesh = new Reference<Mesh>( null, @"Content\Regulators\Default\Marker current.mesh" );

		/// <summary>
		/// The position offset of the current marker mesh.
		/// </summary>
		[DefaultValue( "0.05 0 0" )]
		public Reference<Vector3> MarkerCurrentMeshPosition
		{
			get { if( _markerCurrentMeshPosition.BeginGet() ) MarkerCurrentMeshPosition = _markerCurrentMeshPosition.Get( this ); return _markerCurrentMeshPosition.value; }
			set { if( _markerCurrentMeshPosition.BeginSet( this, ref value ) ) { try { MarkerCurrentMeshPositionChanged?.Invoke( this ); } finally { _markerCurrentMeshPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MarkerCurrentMeshPosition"/> property value changes.</summary>
		public event Action<RegulatorType> MarkerCurrentMeshPositionChanged;
		ReferenceField<Vector3> _markerCurrentMeshPosition = new Vector3( 0.05, 0, 0 );


		/// <summary>
		/// The position offset of the markers depending current rotation angle.
		/// </summary>
		[DefaultValue( 0.04 )]
		public Reference<double> MarkerMeshPositionDependingAngleOffset
		{
			get { if( _markerMeshPositionDependingAngleOffset.BeginGet() ) MarkerMeshPositionDependingAngleOffset = _markerMeshPositionDependingAngleOffset.Get( this ); return _markerMeshPositionDependingAngleOffset.value; }
			set { if( _markerMeshPositionDependingAngleOffset.BeginSet( this, ref value ) ) { try { MarkerMeshPositionDependingAngleOffsetChanged?.Invoke( this ); } finally { _markerMeshPositionDependingAngleOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MarkerMeshPositionDependingAngleOffset"/> property value changes.</summary>
		public event Action<RegulatorType> MarkerMeshPositionDependingAngleOffsetChanged;
		ReferenceField<double> _markerMeshPositionDependingAngleOffset = 0.04;

		/// <summary>
		/// The additional expanding the component's bounds by axes.
		/// </summary>
		[DefaultValue( "0.05 0 0" )]
		public Reference<Vector3> ExpandSpaceBounds
		{
			get { if( _expandSpaceBounds.BeginGet() ) ExpandSpaceBounds = _expandSpaceBounds.Get( this ); return _expandSpaceBounds.value; }
			set { if( _expandSpaceBounds.BeginSet( this, ref value ) ) { try { ExpandSpaceBoundsXChanged?.Invoke( this ); } finally { _expandSpaceBounds.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ExpandSpaceBounds"/> property value changes.</summary>
		public event Action<RegulatorType> ExpandSpaceBoundsXChanged;
		ReferenceField<Vector3> _expandSpaceBounds = new Vector3( 0.05, 0, 0 );

		/// <summary>
		/// Specifies the range of possible values.
		/// </summary>
		[DefaultValue( "0 1" )]
		public Reference<Range> ValueRange
		{
			get { if( _valueRange.BeginGet() ) ValueRange = _valueRange.Get( this ); return _valueRange.value; }
			set { if( _valueRange.BeginSet( this, ref value ) ) { try { ValueRangeChanged?.Invoke( this ); } finally { _valueRange.EndSet(); } } }
		}
		public event Action<RegulatorType> ValueRangeChanged;
		ReferenceField<Range> _valueRange = new Range( 0, 1 );

		/// <summary>
		/// Specifies the angle value of the markers for maximum and maximum value.
		/// </summary>
		[DefaultValue( "-45 45" )]
		[Range( -360, 360 )]
		public Reference<Range> AngleRange
		{
			get { if( _angleRange.BeginGet() ) AngleRange = _angleRange.Get( this ); return _angleRange.value; }
			set { if( _angleRange.BeginSet( this, ref value ) ) { try { AngleRangeChanged?.Invoke( this ); } finally { _angleRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AngleRange"/> property value changes.</summary>
		public event Action<RegulatorType> AngleRangeChanged;
		ReferenceField<Range> _angleRange = new Range( -45, 45 );

		/// <summary>
		/// Change the value of an object per second.
		/// </summary>
		[DefaultValue( 0.5 )]
		public Reference<double> ChangeSpeed
		{
			get { if( _changeSpeed.BeginGet() ) ChangeSpeed = _changeSpeed.Get( this ); return _changeSpeed.value; }
			set { if( _changeSpeed.BeginSet( this, ref value ) ) { try { ChangeSpeedChanged?.Invoke( this ); } finally { _changeSpeed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ChangeSpeed"/> property value changes.</summary>
		public event Action<RegulatorType> ChangeSpeedChanged;
		ReferenceField<double> _changeSpeed = 0.5;

		/// <summary>
		/// The sound that is played when changing.
		/// </summary>
		[DefaultValueReference( @"Base\UI\Styles\Sounds\ButtonClick.ogg" )]
		public Reference<Sound> SoundTick
		{
			get { if( _soundTick.BeginGet() ) SoundTick = _soundTick.Get( this ); return _soundTick.value; }
			set { if( _soundTick.BeginSet( this, ref value ) ) { try { SoundTickChanged?.Invoke( this ); } finally { _soundTick.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundTick"/> property value changes.</summary>
		public event Action<RegulatorType> SoundTickChanged;
		ReferenceField<Sound> _soundTick = new Reference<Sound>( null, @"Base\UI\Styles\Sounds\ButtonClick.ogg" );

		/// <summary>
		/// The frequency of tick sounds.
		/// </summary>
		[DefaultValue( 0.05 )]
		public Reference<double> SoundTickFrequency
		{
			get { if( _soundTickFrequency.BeginGet() ) SoundTickFrequency = _soundTickFrequency.Get( this ); return _soundTickFrequency.value; }
			set { if( _soundTickFrequency.BeginSet( this, ref value ) ) { try { SoundTickFrequencyChanged?.Invoke( this ); } finally { _soundTickFrequency.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundTickFrequency"/> property value changes.</summary>
		public event Action<RegulatorType> SoundTickFrequencyChanged;
		ReferenceField<double> _soundTickFrequency = 0.05;


		/// <summary>
		/// The offset by X from the switch to the center of the valve.
		/// </summary>
		[DefaultValue( 0.1 )]
		public Reference<double> ValveOffset
		{
			get { if( _valveOffset.BeginGet() ) ValveOffset = _valveOffset.Get( this ); return _valveOffset.value; }
			set { if( _valveOffset.BeginSet( this, ref value ) ) { try { ValveOffsetChanged?.Invoke( this ); } finally { _valveOffset.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ValveOffset"/> property value changes.</summary>
		public event Action<RegulatorType> ValveOffsetChanged;
		ReferenceField<double> _valveOffset = 0.1;

		/// <summary>
		/// The radius of the valve.
		/// </summary>
		[DefaultValue( 0.1 )]
		public Reference<double> ValveRadius
		{
			get { if( _valveRadius.BeginGet() ) ValveRadius = _valveRadius.Get( this ); return _valveRadius.value; }
			set { if( _valveRadius.BeginSet( this, ref value ) ) { try { ValveRadiusChanged?.Invoke( this ); } finally { _valveRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ValveRadius"/> property value changes.</summary>
		public event Action<RegulatorType> ValveRadiusChanged;
		ReferenceField<double> _valveRadius = 0.1;


		/// <summary>
		/// Whether to allow user interaction with the object.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> AllowInteract
		{
			get { if( _allowInteract.BeginGet() ) AllowInteract = _allowInteract.Get( this ); return _allowInteract.value; }
			set { if( _allowInteract.BeginSet( this, ref value ) ) { try { AllowInteractChanged?.Invoke( this ); } finally { _allowInteract.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AllowInteract"/> property value changes.</summary>
		public event Action<RegulatorType> AllowInteractChanged;
		ReferenceField<bool> _allowInteract = true;

		///////////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				//these properties are under control by the class
				case nameof( Mesh ):
					skip = true;
					break;

				case nameof( ButtonMeshPosition ):
					if( !ButtonMesh.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( IndicatorMinMeshPosition ):
				case nameof( IndicatorMinMaterialActivated ):
					if( !IndicatorMinMesh.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( IndicatorMaxMeshPosition ):
				case nameof( IndicatorMaxMaterialActivated ):
					if( !IndicatorMaxMesh.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( MarkerMinMeshPosition ):
					if( !MarkerMinMesh.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( MarkerMaxMeshPosition ):
					if( !MarkerMaxMesh.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( MarkerCurrentMeshPosition ):
					if( !MarkerCurrentMesh.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( MarkerMeshPositionDependingAngleOffset ):
					if( !MarkerMinMesh.ReferenceOrValueSpecified && !MarkerMaxMesh.ReferenceOrValueSpecified && !MarkerCurrentMesh.ReferenceOrValueSpecified )
						skip = true;
					break;
				}
			}
		}



		//int version;

		//not used
		//[Browsable( false )]
		//public int Version
		//{
		//	get { return version; }
		//}

		//public void DataWasChanged()
		//{
		//	unchecked
		//	{
		//		version++;
		//	}
		//}
	}
}
