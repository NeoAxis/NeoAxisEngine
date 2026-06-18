// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// A definition of the button in 3D space.
	/// </summary>
	[ResourceFileExtension( "buttontype" )]
	[NewObjectDefaultName( "Button 3D Type" )]
	[AddToResourcesWindow( @"Addons\Button 3D\Button 3D Type", 420 )]
	[EditorControl( typeof( Button3DTypeEditor ) )]
	[Preview( typeof( Button3DTypePreview ) )]
	[PreviewImage( typeof( Button3DTypePreviewImage ) )]
	public class Button3DType : Component
	{
		/// <summary>
		/// The mesh of the base.
		/// </summary>
		[DefaultValueReference( @"Content\Buttons 3D\Default\Base.mesh" )]
		public Reference<Mesh> BaseMesh
		{
			get { if( _baseMesh.BeginGet() ) BaseMesh = _baseMesh.Get( this ); return _baseMesh.value; }
			set { if( _baseMesh.BeginSet( this, ref value ) ) { try { BaseMeshChanged?.Invoke( this ); } finally { _baseMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="BaseMesh"/> property value changes.</summary>
		public event Action<Button3DType> BaseMeshChanged;
		ReferenceField<Mesh> _baseMesh = new Reference<Mesh>( null, @"Content\Buttons 3D\Default\Base.mesh" );

		/// <summary>
		/// The mesh of the button.
		/// </summary>
		[DefaultValueReference( @"Content\Buttons 3D\Default\Button.mesh" )]
		[DisplayName( "Button 3D Mesh" )]
		public Reference<Mesh> Button3DMesh
		{
			get { if( _buttonMesh.BeginGet() ) Button3DMesh = _buttonMesh.Get( this ); return _buttonMesh.value; }
			set { if( _buttonMesh.BeginSet( this, ref value ) ) { try { Button3DMeshChanged?.Invoke( this ); } finally { _buttonMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Button3DMesh"/> property value changes.</summary>
		public event Action<Button3DType> Button3DMeshChanged;
		ReferenceField<Mesh> _buttonMesh = new Reference<Mesh>( null, @"Content\Buttons 3D\Default\Button.mesh" );

		/// <summary>
		/// The position offset of the button mesh in the default state.
		/// </summary>
		[DefaultValue( "0.05 0 0" )]
		[DisplayName( "Button 3D Mesh Position" )]
		public Reference<Vector3> Button3DMeshPosition
		{
			get { if( _buttonMeshPosition.BeginGet() ) Button3DMeshPosition = _buttonMeshPosition.Get( this ); return _buttonMeshPosition.value; }
			set { if( _buttonMeshPosition.BeginSet( this, ref value ) ) { try { Button3DMeshPositionChanged?.Invoke( this ); } finally { _buttonMeshPosition.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Button3DMeshPosition"/> property value changes.</summary>
		public event Action<Button3DType> Button3DMeshPositionChanged;
		ReferenceField<Vector3> _buttonMeshPosition = new Vector3( 0.05, 0, 0 );

		/// <summary>
		/// The position offset of the button mesh in the pushed state.
		/// </summary>
		[DefaultValue( "0.01 0 0" )]
		[DisplayName( "Button 3D Mesh Position Pushed" )]
		public Reference<Vector3> Button3DMeshPositionPushed
		{
			get { if( _buttonMeshPositionPushed.BeginGet() ) Button3DMeshPositionPushed = _buttonMeshPositionPushed.Get( this ); return _buttonMeshPositionPushed.value; }
			set { if( _buttonMeshPositionPushed.BeginSet( this, ref value ) ) { try { Button3DMeshPositionPushedChanged?.Invoke( this ); } finally { _buttonMeshPositionPushed.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Button3DMeshPositionPushed"/> property value changes.</summary>
		public event Action<Button3DType> Button3DMeshPositionPushedChanged;
		ReferenceField<Vector3> _buttonMeshPositionPushed = new Vector3( 0.01, 0, 0 );

		/// <summary>
		/// The mesh of the indicator.
		/// </summary>
		[DefaultValueReference( @"Content\Buttons 3D\Default\Indicator.mesh" )]
		public Reference<Mesh> IndicatorMesh
		{
			get { if( _indicatorMesh.BeginGet() ) IndicatorMesh = _indicatorMesh.Get( this ); return _indicatorMesh.value; }
			set { if( _indicatorMesh.BeginSet( this, ref value ) ) { try { IndicatorMeshChanged?.Invoke( this ); } finally { _indicatorMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndicatorMesh"/> property value changes.</summary>
		public event Action<Button3DType> IndicatorMeshChanged;
		ReferenceField<Mesh> _indicatorMesh = new Reference<Mesh>( null, @"Content\Buttons 3D\Default\Indicator.mesh" );

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
		public event Action<Button3DType> IndicatorMeshPositionChanged;
		ReferenceField<Vector3> _indicatorMeshPosition = new Vector3( 0.03, 0, 0.12 );

		/// <summary>
		/// The material of the indicator in activated state.
		/// </summary>
		[DefaultValueReference( @"Content\Buttons 3D\Default\Indicator activated.material" )]
		public Reference<Material> IndicatorMeshMaterialActivated
		{
			get { if( _indicatorMeshMaterialActivated.BeginGet() ) IndicatorMeshMaterialActivated = _indicatorMeshMaterialActivated.Get( this ); return _indicatorMeshMaterialActivated.value; }
			set { if( _indicatorMeshMaterialActivated.BeginSet( this, ref value ) ) { try { IndicatorMeshMaterialActivatedChanged?.Invoke( this ); } finally { _indicatorMeshMaterialActivated.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="IndicatorMeshMaterialActivated"/> property value changes.</summary>
		public event Action<Button3DType> IndicatorMeshMaterialActivatedChanged;
		ReferenceField<Material> _indicatorMeshMaterialActivated = new Reference<Material>( null, @"Content\Buttons 3D\Default\Indicator activated.material" );

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
		public event Action<Button3DType> ExpandSpaceBoundsXChanged;
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
		public event Action<Button3DType> ClickingTotalTimeChanged;
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
		public event Action<Button3DType> ClickingClickTimeChanged;
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
		public event Action<Button3DType> SoundClickingBeginChanged;
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
		public event Action<Button3DType> SoundClickChanged;
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
		public event Action<Button3DType> SoundClickingEndChanged;
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

				case nameof( Button3DMeshPosition ):
				case nameof( Button3DMeshPositionPushed ):
					if( !Button3DMesh.ReferenceOrValueSpecified )
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
