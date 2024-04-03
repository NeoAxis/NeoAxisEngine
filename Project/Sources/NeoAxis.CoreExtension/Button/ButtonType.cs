// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A definition of the button in space.
	/// </summary>
	[ResourceFileExtension( "buttontype" )]
	[NewObjectDefaultName( "Button Type" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Button\Button Type", 420 )]
	[EditorControl( typeof( ButtonTypeEditor ) )]
	[Preview( typeof( ButtonTypePreview ) )]
	[PreviewImage( typeof( ButtonTypePreviewImage ) )]
#endif
	public class ButtonType : Component
	{
		/// <summary>
		/// The mesh of the base.
		/// </summary>
		[DefaultValueReference( @"Content\Buttons\Default\Base.mesh" )]
		public Reference<Mesh> BaseMesh
		{
			get { if( _baseMesh.BeginGet() ) BaseMesh = _baseMesh.Get( this ); return _baseMesh.value; }
			set { if( _baseMesh.BeginSet( this, ref value ) ) { try { BaseMeshChanged?.Invoke( this ); } finally { _baseMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BaseMesh"/> property value changes.</summary>
		public event Action<ButtonType> BaseMeshChanged;
		ReferenceField<Mesh> _baseMesh = new Reference<Mesh>( null, @"Content\Buttons\Default\Base.mesh" );

		/// <summary>
		/// The mesh of the button.
		/// </summary>
		[DefaultValueReference( @"Content\Buttons\Default\Button.mesh" )]
		public Reference<Mesh> ButtonMesh
		{
			get { if( _buttonMesh.BeginGet() ) ButtonMesh = _buttonMesh.Get( this ); return _buttonMesh.value; }
			set { if( _buttonMesh.BeginSet( this, ref value ) ) { try { ButtonMeshChanged?.Invoke( this ); } finally { _buttonMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ButtonMesh"/> property value changes.</summary>
		public event Action<ButtonType> ButtonMeshChanged;
		ReferenceField<Mesh> _buttonMesh = new Reference<Mesh>( null, @"Content\Buttons\Default\Button.mesh" );

		/// <summary>
		/// The position offset of the button mesh in the default state.
		/// </summary>
		[DefaultValue( "0.05 0 0" )]
		public Reference<Vector3> ButtonMeshPosition
		{
			get { if( _buttonMeshPosition.BeginGet() ) ButtonMeshPosition = _buttonMeshPosition.Get( this ); return _buttonMeshPosition.value; }
			set { if( _buttonMeshPosition.BeginSet( this, ref value ) ) { try { ButtonMeshPositionChanged?.Invoke( this ); } finally { _buttonMeshPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ButtonMeshPosition"/> property value changes.</summary>
		public event Action<ButtonType> ButtonMeshPositionChanged;
		ReferenceField<Vector3> _buttonMeshPosition = new Vector3( 0.05, 0, 0 );

		/// <summary>
		/// The position offset of the button mesh in the pushed state.
		/// </summary>
		[DefaultValue( "0.01 0 0" )]
		public Reference<Vector3> ButtonMeshPositionPushed
		{
			get { if( _buttonMeshPositionPushed.BeginGet() ) ButtonMeshPositionPushed = _buttonMeshPositionPushed.Get( this ); return _buttonMeshPositionPushed.value; }
			set { if( _buttonMeshPositionPushed.BeginSet( this, ref value ) ) { try { ButtonMeshPositionPushedChanged?.Invoke( this ); } finally { _buttonMeshPositionPushed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ButtonMeshPositionPushed"/> property value changes.</summary>
		public event Action<ButtonType> ButtonMeshPositionPushedChanged;
		ReferenceField<Vector3> _buttonMeshPositionPushed = new Vector3( 0.01, 0, 0 );

		/// <summary>
		/// The mesh of the indicator.
		/// </summary>
		[DefaultValueReference( @"Content\Buttons\Default\Indicator.mesh" )]
		public Reference<Mesh> IndicatorMesh
		{
			get { if( _indicatorMesh.BeginGet() ) IndicatorMesh = _indicatorMesh.Get( this ); return _indicatorMesh.value; }
			set { if( _indicatorMesh.BeginSet( this, ref value ) ) { try { IndicatorMeshChanged?.Invoke( this ); } finally { _indicatorMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndicatorMesh"/> property value changes.</summary>
		public event Action<ButtonType> IndicatorMeshChanged;
		ReferenceField<Mesh> _indicatorMesh = new Reference<Mesh>( null, @"Content\Buttons\Default\Indicator.mesh" );

		/// <summary>
		/// The position offset of the indicator mesh.
		/// </summary>
		[DefaultValue( "0.03 0 0.12" )]
		public Reference<Vector3> IndicatorMeshPosition
		{
			get { if( _indicatorMeshPosition.BeginGet() ) IndicatorMeshPosition = _indicatorMeshPosition.Get( this ); return _indicatorMeshPosition.value; }
			set { if( _indicatorMeshPosition.BeginSet( this, ref value ) ) { try { IndicatorMeshPositionChanged?.Invoke( this ); } finally { _indicatorMeshPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndicatorMeshPosition"/> property value changes.</summary>
		public event Action<ButtonType> IndicatorMeshPositionChanged;
		ReferenceField<Vector3> _indicatorMeshPosition = new Vector3( 0.03, 0, 0.12 );

		/// <summary>
		/// The material of the indicator in activated state.
		/// </summary>
		[DefaultValueReference( @"Content\Buttons\Default\Indicator activated.material" )]
		public Reference<Material> IndicatorMeshMaterialActivated
		{
			get { if( _indicatorMeshMaterialActivated.BeginGet() ) IndicatorMeshMaterialActivated = _indicatorMeshMaterialActivated.Get( this ); return _indicatorMeshMaterialActivated.value; }
			set { if( _indicatorMeshMaterialActivated.BeginSet( this, ref value ) ) { try { IndicatorMeshMaterialActivatedChanged?.Invoke( this ); } finally { _indicatorMeshMaterialActivated.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndicatorMeshMaterialActivated"/> property value changes.</summary>
		public event Action<ButtonType> IndicatorMeshMaterialActivatedChanged;
		ReferenceField<Material> _indicatorMeshMaterialActivated = new Reference<Material>( null, @"Content\Buttons\Default\Indicator activated.material" );

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
		public event Action<ButtonType> ExpandSpaceBoundsXChanged;
		ReferenceField<Vector3> _expandSpaceBounds = new Vector3( 0.05, 0, 0 );


		/// <summary>
		/// Total time of clicking animation.
		/// </summary>
		[DefaultValue( 0.4 )]
		public Reference<double> ClickingTotalTime
		{
			get { if( _clickingTotalTime.BeginGet() ) ClickingTotalTime = _clickingTotalTime.Get( this ); return _clickingTotalTime.value; }
			set { if( _clickingTotalTime.BeginSet( this, ref value ) ) { try { ClickingTotalTimeChanged?.Invoke( this ); } finally { _clickingTotalTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ClickingTotalTime"/> property value changes.</summary>
		public event Action<ButtonType> ClickingTotalTimeChanged;
		ReferenceField<double> _clickingTotalTime = 0.4;

		/// <summary>
		/// The time of the click during clicking animation.
		/// </summary>
		[DefaultValue( 0.2 )]
		public Reference<double> ClickingClickTime
		{
			get { if( _clickingClickTime.BeginGet() ) ClickingClickTime = _clickingClickTime.Get( this ); return _clickingClickTime.value; }
			set { if( _clickingClickTime.BeginSet( this, ref value ) ) { try { ClickingClickTimeChanged?.Invoke( this ); } finally { _clickingClickTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ClickingClickTime"/> property value changes.</summary>
		public event Action<ButtonType> ClickingClickTimeChanged;
		ReferenceField<double> _clickingClickTime = 0.2;

		/// <summary>
		/// The sound that is played when the clicking begins.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> SoundClickingBegin
		{
			get { if( _soundClickingBegin.BeginGet() ) SoundClickingBegin = _soundClickingBegin.Get( this ); return _soundClickingBegin.value; }
			set { if( _soundClickingBegin.BeginSet( this, ref value ) ) { try { SoundClickingBeginChanged?.Invoke( this ); } finally { _soundClickingBegin.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundClickingBegin"/> property value changes.</summary>
		public event Action<ButtonType> SoundClickingBeginChanged;
		ReferenceField<Sound> _soundClickingBegin = null;

		/// <summary>
		/// The sound that is played when a click occurs.
		/// </summary>
		[DefaultValueReference( @"Base\UI\Styles\Sounds\ButtonClick.ogg" )]
		public Reference<Sound> SoundClick
		{
			get { if( _soundClick.BeginGet() ) SoundClick = _soundClick.Get( this ); return _soundClick.value; }
			set { if( _soundClick.BeginSet( this, ref value ) ) { try { SoundClickChanged?.Invoke( this ); } finally { _soundClick.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundClick"/> property value changes.</summary>
		public event Action<ButtonType> SoundClickChanged;
		ReferenceField<Sound> _soundClick = new Reference<Sound>( null, @"Base\UI\Styles\Sounds\ButtonClick.ogg" );

		/// <summary>
		/// The sound that is played when the clicking ends.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Sound> SoundClickingEnd
		{
			get { if( _soundClickingEnd.BeginGet() ) SoundClickingEnd = _soundClickingEnd.Get( this ); return _soundClickingEnd.value; }
			set { if( _soundClickingEnd.BeginSet( this, ref value ) ) { try { SoundClickingEndChanged?.Invoke( this ); } finally { _soundClickingEnd.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SoundClickingEnd"/> property value changes.</summary>
		public event Action<ButtonType> SoundClickingEndChanged;
		ReferenceField<Sound> _soundClickingEnd = null;

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
				case nameof( ButtonMeshPositionPushed ):
					if( !ButtonMesh.ReferenceOrValueSpecified )
						skip = true;
					break;

				case nameof( IndicatorMeshPosition ):
				case nameof( IndicatorMeshMaterialActivated ):
					if( !IndicatorMesh.ReferenceOrValueSpecified )
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
