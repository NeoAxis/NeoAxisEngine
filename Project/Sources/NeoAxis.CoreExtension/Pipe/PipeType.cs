// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;
using System.Linq;

//!!!!всё тут

//коллижен: возможность делать коллижен проще геометрии

//optimization: при загрузке много раз обновляет logicalData. не только в пайпах так.

//optimization: еще в процессе transform tool делать меньшую детализацию.
////или даже только сплайну рисовать.

//пайпы могут быть динамические, например на автомобиле

//мультипоточно генерировать

//при небольших CurvatureRadius совсем иначе нужно делать. несколько шагов в одной точке. да и геометрия иначе

//опция гранености краёв. SharpEdges

//лоды, секторы

//опция рисовать ли внутреньнюю часть

//трехмерный Cross. в разные стороны-оси

//override material для мешей. для трубы тоже
//может типы делать белыми

//максимальная сплошная длина. добавлять меши Socket. указывать свойство максимальную длину


//quality pipes: https://sketchfab.com/3d-models/industrial-armatures-set-503ab0c60fb24f5683acf0bee0a2497d

//32 Lessons I Learned from Creating 32 Modular Pipe Assets
//https://www.worldofleveldesign.com/categories/ue4/lessons-learned-modular-pipes.php

//Blender learning
//https://www.youtube.com/watch?v=q7fCWyEhSGw

//как создавать шланги
//https://www.youtube.com/watch?v=MMnuRx8bX_U&ab_channel=MultProsvet


//https://engineer3dcad.blogspot.com/2017/09/piping-system-din-iso-3d-model-cad-flie.html
//https://grabcad.com/library/tag/piping
//https://grabcad.com/shubham.nikumbh-1/models

//https://grabcad.com/library/ball-valve-3d-model-1

//лежать в сторе 10 типов пайпов. они поддерживают Pipe Constructor.

//несколько видов: заводские, тонкие в ванной, 

//квадратные пайпы, как тут https://www.cgtrader.com/3d-models/industrial/part/industrial-factory-environment-pipeline
//это по сути просто 4 сегмента. тут вопрос о том чтобы они как нужно ориентированы были
//еще дальше - форму указывать в виде 2д полигона

//пример больших конструкций
//https://www.cgtrader.com/3d-models/industrial/part/industrial-factory-environment-pipeline

//бесплатный сет
//https://www.cgtrader.com/free-3d-models/industrial/part/autodesk-inventor-cad-library-piping-no-1-lt

//silo
//https://www.cgtrader.com/free-3d-models/industrial/part/silo-lowpoly

//пример завода
//https://www.cgtrader.com/3d-models/exterior/industrial/factory-kitbash-03


//https://sketchfab.com/
//Creative Commons Attribution

//https://sketchfab.com/search?features=downloadable&sort_by=-pertinence&type=models

//"modular pipes"

//FREE
//https://sketchfab.com/3d-models/modular-pipes-0c1852d3a77043fcba7847b8b8a1fc23

//https://www.worldofleveldesign.com/categories/ue4/lessons-learned-modular-pipes.php
//https://3dexport.com/3dmodel-pbr-modular-pipes-set-185691.htm

//https://sketchfab.com/3d-models/techlab-modular-scifi-pipes-a481a1af0598431da712a10d11b24321



namespace NeoAxis
{
	/// <summary>
	/// A definition of the pipe type.
	/// </summary>
	[ResourceFileExtension( "pipetype" )]
#if !DEPLOY
	[AddToResourcesWindow( @"Addons\Pipe\Pipe Type", 9100 )]
	[EditorControl( typeof( PipeTypeEditor ) )]
	[Preview( typeof( PipeTypePreview ) )]
	[PreviewImage( typeof( PipeTypePreviewImage ) )]
#endif
	public class PipeType : Component
	{
		int version;

		[DefaultValue( 0.1 )]
		[Range( 0.01, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> OutsideDiameter
		{
			get { if( _outsideDiameter.BeginGet() ) OutsideDiameter = _outsideDiameter.Get( this ); return _outsideDiameter.value; }
			set { if( _outsideDiameter.BeginSet( ref value ) ) { try { OutsideDiameterChanged?.Invoke( this ); DataWasChanged(); } finally { _outsideDiameter.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OutsideDiameter"/> property value changes.</summary>
		public event Action<PipeType> OutsideDiameterChanged;
		ReferenceField<double> _outsideDiameter = 0.1;

		[DefaultValue( 0.5 )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> CurvatureRadius
		{
			get { if( _curvatureRadius.BeginGet() ) CurvatureRadius = _curvatureRadius.Get( this ); return _curvatureRadius.value; }
			set { if( _curvatureRadius.BeginSet( ref value ) ) { try { CurvatureRadiusChanged?.Invoke( this ); DataWasChanged(); } finally { _curvatureRadius.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CurvatureRadius"/> property value changes.</summary>
		public event Action<PipeType> CurvatureRadiusChanged;
		ReferenceField<double> _curvatureRadius = 0.5;

		//!!!!default value
		[DefaultValue( 0.1 )]
		[Range( 0.05, 0.5, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		public Reference<double> SegmentsLength
		{
			get { if( _segmentsLength.BeginGet() ) SegmentsLength = _segmentsLength.Get( this ); return _segmentsLength.value; }
			set { if( _segmentsLength.BeginSet( ref value ) ) { try { SegmentsLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _segmentsLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SegmentsLength"/> property value changes.</summary>
		public event Action<PipeType> SegmentsLengthChanged;
		ReferenceField<double> _segmentsLength = 0.1;

		[DefaultValue( 16 )]
		[Range( 4, 64, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<int> SegmentsCircle
		{
			get { if( _segmentsCircle.BeginGet() ) SegmentsCircle = _segmentsCircle.Get( this ); return _segmentsCircle.value; }
			set { if( _segmentsCircle.BeginSet( ref value ) ) { try { SegmentsCircleChanged?.Invoke( this ); DataWasChanged(); } finally { _segmentsCircle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SegmentsCircle"/> property value changes.</summary>
		public event Action<PipeType> SegmentsCircleChanged;
		ReferenceField<int> _segmentsCircle = 16;

		[DefaultValue( null )]
		public Reference<Material> Material
		{
			get { if( _material.BeginGet() ) Material = _material.Get( this ); return _material.value; }
			set { if( _material.BeginSet( ref value ) ) { try { MaterialChanged?.Invoke( this ); DataWasChanged(); } finally { _material.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Material"/> property value changes.</summary>
		public event Action<PipeType> MaterialChanged;
		ReferenceField<Material> _material = null;

		[DefaultValue( 1.0 )]
		[Range( 0.1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		[DisplayName( "UV Tiles Length" )]
		public Reference<double> UVTilesLength
		{
			get { if( _uVTilesLength.BeginGet() ) UVTilesLength = _uVTilesLength.Get( this ); return _uVTilesLength.value; }
			set { if( _uVTilesLength.BeginSet( ref value ) ) { try { UVTilesLengthChanged?.Invoke( this ); DataWasChanged(); } finally { _uVTilesLength.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVTilesLength"/> property value changes.</summary>
		public event Action<PipeType> UVTilesLengthChanged;
		ReferenceField<double> _uVTilesLength = 1.0;

		[DefaultValue( 1.0 )]
		[Range( 1, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		[DisplayName( "UV Tiles Circle" )]
		public Reference<double> UVTilesCircle
		{
			get { if( _uVTilesCircle.BeginGet() ) UVTilesCircle = _uVTilesCircle.Get( this ); return _uVTilesCircle.value; }
			set { if( _uVTilesCircle.BeginSet( ref value ) ) { try { UVTilesCircleChanged?.Invoke( this ); DataWasChanged(); } finally { _uVTilesCircle.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVTilesCircle"/> property value changes.</summary>
		public event Action<PipeType> UVTilesCircleChanged;
		ReferenceField<double> _uVTilesCircle = 1.0;

		[DefaultValue( false )]
		[DisplayName( "UV Flip" )]
		public Reference<bool> UVFlip
		{
			get { if( _uVFlip.BeginGet() ) UVFlip = _uVFlip.Get( this ); return _uVFlip.value; }
			set { if( _uVFlip.BeginSet( ref value ) ) { try { UVFlipChanged?.Invoke( this ); DataWasChanged(); } finally { _uVFlip.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UVFlip"/> property value changes.</summary>
		public event Action<PipeType> UVFlipChanged;
		ReferenceField<bool> _uVFlip = false;

		//!!!!
		//[DefaultValue( false )]
		//public Reference<bool> SharpEdges
		//{
		//	get { if( _sharpEdges.BeginGet() ) SharpEdges = _sharpEdges.Get( this ); return _sharpEdges.value; }
		//	set { if( _sharpEdges.BeginSet( ref value ) ) { try { SharpEdgesChanged?.Invoke( this ); DataWasChanged(); } finally { _sharpEdges.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SharpEdges"/> property value changes.</summary>
		//public event Action<PipeType> SharpEdgesChanged;
		//ReferenceField<bool> _sharpEdges = false;

		/// <summary>
		/// The factor of maximum visibility distance. The maximum distance is calculated based on the size of the object on the screen.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> VisibilityDistanceFactor
		{
			get { if( _visibilityDistanceFactor.BeginGet() ) VisibilityDistanceFactor = _visibilityDistanceFactor.Get( this ); return _visibilityDistanceFactor.value; }
			set { if( _visibilityDistanceFactor.BeginSet( ref value ) ) { try { VisibilityDistanceFactorChanged?.Invoke( this ); } finally { _visibilityDistanceFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="VisibilityDistanceFactor"/> property value changes.</summary>
		public event Action<PipeType> VisibilityDistanceFactorChanged;
		ReferenceField<double> _visibilityDistanceFactor = 1.0;

		///// <summary>
		///// Maximum visibility range of the pipe geometry.
		///// </summary>
		//[DefaultValue( 10000.0 )]
		//[Range( 0, 10000, RangeAttribute.ConvenientDistributionEnum.Exponential, 4 )]
		//public Reference<double> VisibilityDistance
		//{
		//	get { if( _visibilityDistance.BeginGet() ) VisibilityDistance = _visibilityDistance.Get( this ); return _visibilityDistance.value; }
		//	set { if( _visibilityDistance.BeginSet( ref value ) ) { try { VisibilityDistanceChanged?.Invoke( this ); DataWasChanged(); } finally { _visibilityDistance.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VisibilityDistance"/> property value changes.</summary>
		//public event Action<PipeType> VisibilityDistanceChanged;
		//ReferenceField<double> _visibilityDistance = 10000.0;

		[Category( "Meshes" )]
		[DefaultValue( null )]
		public Reference<Mesh> OpenHole
		{
			get { if( _openHole.BeginGet() ) OpenHole = _openHole.Get( this ); return _openHole.value; }
			set { if( _openHole.BeginSet( ref value ) ) { try { OpenHoleChanged?.Invoke( this ); DataWasChanged(); } finally { _openHole.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OpenHole"/> property value changes.</summary>
		public event Action<PipeType> OpenHoleChanged;
		ReferenceField<Mesh> _openHole = null;

		[Category( "Meshes" )]
		[DefaultValue( null )]
		public Reference<Mesh> Cap
		{
			get { if( _cap.BeginGet() ) Cap = _cap.Get( this ); return _cap.value; }
			set { if( _cap.BeginSet( ref value ) ) { try { CapChanged?.Invoke( this ); DataWasChanged(); } finally { _cap.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Cap"/> property value changes.</summary>
		public event Action<PipeType> CapChanged;
		ReferenceField<Mesh> _cap = null;

		[Category( "Meshes" )]
		[DefaultValue( null )]
		public Reference<Mesh> Socket
		{
			get { if( _socket.BeginGet() ) Socket = _socket.Get( this ); return _socket.value; }
			set { if( _socket.BeginSet( ref value ) ) { try { SocketChanged?.Invoke( this ); DataWasChanged(); } finally { _socket.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Socket"/> property value changes.</summary>
		public event Action<PipeType> SocketChanged;
		ReferenceField<Mesh> _socket = null;

		[Category( "Meshes" )]
		[DefaultValue( null )]
		public Reference<Mesh> Holder
		{
			get { if( _holder.BeginGet() ) Holder = _holder.Get( this ); return _holder.value; }
			set { if( _holder.BeginSet( ref value ) ) { try { HolderChanged?.Invoke( this ); DataWasChanged(); } finally { _holder.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Holder"/> property value changes.</summary>
		public event Action<PipeType> HolderChanged;
		ReferenceField<Mesh> _holder = null;

		[Category( "Meshes" )]
		[DefaultValue( null )]
		[DisplayName( "Elbow 90" )]
		public Reference<Mesh> Elbow90
		{
			get { if( _elbow90.BeginGet() ) Elbow90 = _elbow90.Get( this ); return _elbow90.value; }
			set { if( _elbow90.BeginSet( ref value ) ) { try { Elbow90Changed?.Invoke( this ); DataWasChanged(); } finally { _elbow90.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Elbow90"/> property value changes.</summary>
		public event Action<PipeType> Elbow90Changed;
		ReferenceField<Mesh> _elbow90 = null;

		//!!!!impl
		//[Category( "Meshes" )]
		//[DefaultValue( null )]
		//[DisplayName( "Elbow 135" )]
		//public Reference<Mesh> Elbow135
		//{
		//	get { if( _elbow135.BeginGet() ) Elbow135 = _elbow135.Get( this ); return _elbow135.value; }
		//	set { if( _elbow135.BeginSet( ref value ) ) { try { Elbow135Changed?.Invoke( this ); DataWasChanged(); } finally { _elbow135.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Elbow135"/> property value changes.</summary>
		//public event Action<PipeType> Elbow135Changed;
		//ReferenceField<Mesh> _elbow135 = null;

		//!!!!impl
		//[Category( "Meshes" )]
		//[DefaultValue( null )]
		//[DisplayName( "Tee 45" )]
		//public Reference<Mesh> Tee45
		//{
		//	get { if( _tee45.BeginGet() ) Tee45 = _tee45.Get( this ); return _tee45.value; }
		//	set { if( _tee45.BeginSet( ref value ) ) { try { Tee45Changed?.Invoke( this ); DataWasChanged(); } finally { _tee45.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see45 cref="Tee45"/> property value changes.</summary>
		//public event Action<PipeType> Tee45Changed;
		//ReferenceField<Mesh> _tee45 = null;

		[Category( "Meshes" )]
		[DefaultValue( null )]
		[DisplayName( "Tee 90" )]
		public Reference<Mesh> Tee90
		{
			get { if( _tee90.BeginGet() ) Tee90 = _tee90.Get( this ); return _tee90.value; }
			set { if( _tee90.BeginSet( ref value ) ) { try { Tee90Changed?.Invoke( this ); DataWasChanged(); } finally { _tee90.EndSet(); } } }
		}
		/// <summary>Occurs when the <see90 cref="Tee90"/> property value changes.</summary>
		public event Action<PipeType> Tee90Changed;
		ReferenceField<Mesh> _tee90 = null;

		[Category( "Meshes" )]
		[DefaultValue( null )]
		public Reference<Mesh> Cross
		{
			get { if( _cross.BeginGet() ) Cross = _cross.Get( this ); return _cross.value; }
			set { if( _cross.BeginSet( ref value ) ) { try { CrossChanged?.Invoke( this ); DataWasChanged(); } finally { _cross.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Cross"/> property value changes.</summary>
		public event Action<PipeType> CrossChanged;
		ReferenceField<Mesh> _cross = null;


		/// <summary>
		/// The physical material used by the rigid body.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Physics" )]
		public Reference<PhysicalMaterial> CollisionMaterial
		{
			get { if( _collisionMaterial.BeginGet() ) CollisionMaterial = _collisionMaterial.Get( this ); return _collisionMaterial.value; }
			set
			{
				if( _collisionMaterial.BeginSet( ref value ) )
				{
					try
					{
						CollisionMaterialChanged?.Invoke( this );
						DataWasChanged();
					}
					finally { _collisionMaterial.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionMaterial"/> property value changes.</summary>
		public event Action<PipeType> CollisionMaterialChanged;
		ReferenceField<PhysicalMaterial> _collisionMaterial;

		//!!!!
		///// <summary>
		///// The type of friction applied on the rigid body.
		///// </summary>
		//[DefaultValue( PhysicalMaterial.FrictionModeEnum.Simple )]
		//[Category( "Physics" )]
		//public Reference<PhysicalMaterial.FrictionModeEnum> CollisionFrictionMode
		//{
		//	get { if( _collisionFrictionMode.BeginGet() ) CollisionFrictionMode = _collisionFrictionMode.Get( this ); return _collisionFrictionMode.value; }
		//	set
		//	{
		//		if( _collisionFrictionMode.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CollisionFrictionModeChanged?.Invoke( this );
		//				DataWasChanged();
		//			}
		//			finally { _collisionFrictionMode.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="CollisionFrictionMode"/> property value changes.</summary>
		//public event Action<PipeType> CollisionFrictionModeChanged;
		//ReferenceField<PhysicalMaterial.FrictionModeEnum> _collisionFrictionMode = PhysicalMaterial.FrictionModeEnum.Simple;

		/// <summary>
		/// The amount of friction applied on the rigid body.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		[Category( "Physics" )]
		public Reference<double> CollisionFriction
		{
			get { if( _collisionFriction.BeginGet() ) CollisionFriction = _collisionFriction.Get( this ); return _collisionFriction.value; }
			set
			{
				if( _collisionFriction.BeginSet( ref value ) )
				{
					try
					{
						CollisionFrictionChanged?.Invoke( this );
						DataWasChanged();
					}
					finally { _collisionFriction.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionFriction"/> property value changes.</summary>
		public event Action<PipeType> CollisionFrictionChanged;
		ReferenceField<double> _collisionFriction = 0.5;

		//!!!!
		///// <summary>
		///// The amount of directional friction applied on the rigid body.
		///// </summary>
		//[DefaultValue( "1 1 1" )]
		//[Category( "Physics" )]
		//public Reference<Vector3> CollisionAnisotropicFriction
		//{
		//	get { if( _collisionAnisotropicFriction.BeginGet() ) CollisionAnisotropicFriction = _collisionAnisotropicFriction.Get( this ); return _collisionAnisotropicFriction.value; }
		//	set
		//	{
		//		if( _collisionAnisotropicFriction.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CollisionAnisotropicFrictionChanged?.Invoke( this );
		//				DataWasChanged();
		//			}
		//			finally { _collisionAnisotropicFriction.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="CollisionAnisotropicFriction"/> property value changes.</summary>
		//public event Action<PipeType> CollisionAnisotropicFrictionChanged;
		//ReferenceField<Vector3> _collisionAnisotropicFriction = Vector3.One;

		//!!!!
		///// <summary>
		///// The amount of friction applied when rigid body is spinning.
		///// </summary>
		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//[Category( "Physics" )]
		//public Reference<double> CollisionSpinningFriction
		//{
		//	get { if( _collisionSpinningFriction.BeginGet() ) CollisionSpinningFriction = _collisionSpinningFriction.Get( this ); return _collisionSpinningFriction.value; }
		//	set
		//	{
		//		if( _collisionSpinningFriction.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CollisionSpinningFrictionChanged?.Invoke( this );
		//				DataWasChanged();
		//			}
		//			finally { _collisionSpinningFriction.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="CollisionSpinningFriction"/> property value changes.</summary>
		//public event Action<PipeType> CollisionSpinningFrictionChanged;
		//ReferenceField<double> _collisionSpinningFriction = 0.5;

		//!!!!
		///// <summary>
		///// The amount of friction applied when rigid body is rolling.
		///// </summary>
		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//[Category( "Physics" )]
		//public Reference<double> CollisionRollingFriction
		//{
		//	get { if( _collisionRollingFriction.BeginGet() ) CollisionRollingFriction = _collisionRollingFriction.Get( this ); return _collisionRollingFriction.value; }
		//	set
		//	{
		//		if( _collisionRollingFriction.BeginSet( ref value ) )
		//		{
		//			try
		//			{
		//				CollisionRollingFrictionChanged?.Invoke( this );
		//				DataWasChanged();
		//			}
		//			finally { _collisionRollingFriction.EndSet(); }
		//		}
		//	}
		//}
		///// <summary>Occurs when the <see cref="CollisionRollingFriction"/> property value changes.</summary>
		//public event Action<PipeType> CollisionRollingFrictionChanged;
		//ReferenceField<double> _collisionRollingFriction = 0.5;

		/// <summary>
		/// The ratio of the final relative velocity to initial relative velocity of the rigid body after collision.
		/// </summary>
		[DefaultValue( 0.0 )]
		[Range( 0, 1 )]
		[Category( "Physics" )]
		public Reference<double> CollisionRestitution
		{
			get { if( _collisionRestitution.BeginGet() ) CollisionRestitution = _collisionRestitution.Get( this ); return _collisionRestitution.value; }
			set
			{
				if( _collisionRestitution.BeginSet( ref value ) )
				{
					try
					{
						CollisionRestitutionChanged?.Invoke( this );
						DataWasChanged();
					}
					finally { _collisionRestitution.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CollisionRestitution"/> property value changes.</summary>
		public event Action<PipeType> CollisionRestitutionChanged;
		ReferenceField<double> _collisionRestitution;

		//

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				//case nameof( CollisionMaterial ):
				//	if( !Collision )
				//		skip = true;
				//	break;

				//!!!!
				//case nameof( CollisionFrictionMode ):
				//	if( /*!Collision || */CollisionMaterial.Value != null )
				//		skip = true;
				//	break;

				case nameof( CollisionFriction ):
					if( /*!Collision || */CollisionMaterial.Value != null )
						skip = true;
					break;

				//!!!!
				//case nameof( CollisionRollingFriction ):
				//case nameof( CollisionSpinningFriction ):
				//case nameof( CollisionAnisotropicFriction ):
				//	if( /*!Collision || */CollisionFrictionMode.Value == PhysicalMaterial.FrictionModeEnum.Simple || CollisionMaterial.Value != null )
				//		skip = true;
				//	break;

				case nameof( CollisionRestitution ):
					if( /*!Collision || */CollisionMaterial.Value != null )
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
	}
}
