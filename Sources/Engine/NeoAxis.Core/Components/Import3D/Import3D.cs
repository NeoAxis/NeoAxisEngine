// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using NeoAxis.Import;
using NeoAxis.Editor;
using Internal;

namespace NeoAxis
{
	/// <summary>
	/// The component for import of 3D content.
	/// </summary>
	[EditorControl( typeof( Import3DEditor ) )]
	[Preview( typeof( Import3DPreview ) )]
	[PreviewImage( typeof( Import3DPreviewImage ) )]
	[SettingsCell( typeof( Import3DSettingsCell ) )]
	public class Import3D : Component
	{
		bool insideDoUpdate;

		public enum ModeEnum
		{
			Auto,
			OneMesh,
			Meshes,
			//!!!!impl
			//Scene,
		}
		/// <summary>
		/// The mode of the import.
		/// </summary>
		[DefaultValue( ModeEnum.Auto )]
		[Serialize]
		[Category( "Basic" )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set { if( _mode.BeginSet( ref value ) ) { try { ModeChanged?.Invoke( this ); } finally { _mode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<Import3D> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum.Auto;

		/// <summary>
		/// Specifies a result position offset of imported 3D models.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Transform" )]
		public Reference<Vector3> Position
		{
			get { if( _position.BeginGet() ) Position = _position.Get( this ); return _position.value; }
			set { if( _position.BeginSet( ref value ) ) { try { PositionChanged?.Invoke( this ); } finally { _position.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Position"/> property value changes.</summary>
		public event Action<Import3D> PositionChanged;
		ReferenceField<Vector3> _position;

		/// <summary>
		/// Specifies a result rotation of imported 3D models.
		/// </summary>
		[DefaultValue( "0 0 0 1" )]
		[Serialize]
		[Category( "Transform" )]
		public Reference<Quaternion> Rotation
		{
			get { if( _rotation.BeginGet() ) Rotation = _rotation.Get( this ); return _rotation.value; }
			set { if( _rotation.BeginSet( ref value ) ) { try { RotationChanged?.Invoke( this ); } finally { _rotation.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Rotation"/> property value changes.</summary>
		public event Action<Import3D> RotationChanged;
		ReferenceField<Quaternion> _rotation = Quaternion.Identity;

		/// <summary>
		/// Specifies a result scale of imported 3D models.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Serialize]
		[Category( "Transform" )]
		public Reference<double> Scale
		{
			get { if( _scale.BeginGet() ) Scale = _scale.Get( this ); return _scale.value; }
			set { if( _scale.BeginSet( ref value ) ) { try { ScaleChanged?.Invoke( this ); } finally { _scale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Scale"/> property value changes.</summary>
		public event Action<Import3D> ScaleChanged;
		ReferenceField<double> _scale = 1.0;

		/// <summary>
		/// Whether to change a coordinate system to engine system.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[DisplayName( "Force Front X Axis" )]
		[Category( "Transform" )]
		public Reference<bool> ForceFrontXAxis
		{
			get { if( _forceFrontXAxis.BeginGet() ) ForceFrontXAxis = _forceFrontXAxis.Get( this ); return _forceFrontXAxis.value; }
			set { if( _forceFrontXAxis.BeginSet( ref value ) ) { try { ForceFrontXAxisChanged?.Invoke( this ); } finally { _forceFrontXAxis.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ForceFrontXAxis"/> property value changes.</summary>
		public event Action<Import3D> ForceFrontXAxisChanged;
		ReferenceField<bool> _forceFrontXAxis = true;

		/// <summary>
		/// Whether to move models to center by result bounding box.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Transform" )]
		public Reference<bool> CenterBySize
		{
			get { if( _centerBySize.BeginGet() ) CenterBySize = _centerBySize.Get( this ); return _centerBySize.value; }
			set { if( _centerBySize.BeginSet( ref value ) ) { try { CenterBySizeChanged?.Invoke( this ); } finally { _centerBySize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CenterBySize"/> property value changes.</summary>
		public event Action<Import3D> CenterBySizeChanged;
		ReferenceField<bool> _centerBySize = false;

		/// <summary>
		/// Whether to merge geometries with the same format.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "Geometry" )]
		public Reference<bool> MergeMeshGeometries
		{
			get { if( _mergeMeshGeometries.BeginGet() ) MergeMeshGeometries = _mergeMeshGeometries.Get( this ); return _mergeMeshGeometries.value; }
			set { if( _mergeMeshGeometries.BeginSet( ref value ) ) { try { MergeMeshGeometriesChanged?.Invoke( this ); } finally { _mergeMeshGeometries.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MergeMeshGeometries"/> property value changes.</summary>
		public event Action<Import3D> MergeMeshGeometriesChanged;
		ReferenceField<bool> _mergeMeshGeometries = true;

		/// <summary>
		/// Whether to optimize the mesh without losing quality. The optimization includes the merging of almost identical vertices, optimizing for vertex cache, for overdraw and for vertex fetch.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Geometry" )]
		public Reference<bool> Optimize
		{
			get { if( _optimize.BeginGet() ) Optimize = _optimize.Get( this ); return _optimize.value; }
			set { if( _optimize.BeginSet( ref value ) ) { try { OptimizeChanged?.Invoke( this ); } finally { _optimize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Optimize"/> property value changes.</summary>
		public event Action<Import3D> OptimizeChanged;
		ReferenceField<bool> _optimize = true;

		/// <summary>
		/// The threshold value when optimizes the mesh without losing quality. The parameter affects merging of almost identical vertices.
		/// </summary>
		[DefaultValue( 0.001 )]
		[Category( "Geometry" )]
		[Range( 0.0, 0.01, RangeAttribute.ConvenientDistributionEnum.Exponential, 5 )]
		public Reference<double> OptimizeThreshold
		{
			get { if( _optimizeThreshold.BeginGet() ) OptimizeThreshold = _optimizeThreshold.Get( this ); return _optimizeThreshold.value; }
			set { if( _optimizeThreshold.BeginSet( ref value ) ) { try { OptimizeThresholdChanged?.Invoke( this ); } finally { _optimizeThreshold.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OptimizeThreshold"/> property value changes.</summary>
		public event Action<Import3D> OptimizeThresholdChanged;
		ReferenceField<double> _optimizeThreshold = 0.001;

		/// <summary>
		/// Whether to generate level of detail meshes.
		/// </summary>
		[DefaultValue( false )]
		[DisplayName( "LOD Generate" )]
		[Category( "Geometry" )]
		public Reference<bool> LODGenerate
		{
			get { if( _lodGenerate.BeginGet() ) LODGenerate = _lodGenerate.Get( this ); return _lodGenerate.value; }
			set { if( _lodGenerate.BeginSet( ref value ) ) { try { LODGenerateChanged?.Invoke( this ); } finally { _lodGenerate.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODGenerate"/> property value changes.</summary>
		public event Action<Import3D> LODGenerateChanged;
		ReferenceField<bool> _lodGenerate = false;

		/// <summary>
		/// The quality of the first level of detail (LOD0).
		/// </summary>
		[DefaultValue( 1.0 )]
		[Range( 0, 1 )]
		[DisplayName( "LOD Initial Factor" )]
		[Category( "Geometry" )]
		public Reference<double> LODInitialFactor
		{
			get { if( _lODInitialFactor.BeginGet() ) LODInitialFactor = _lODInitialFactor.Get( this ); return _lODInitialFactor.value; }
			set { if( _lODInitialFactor.BeginSet( ref value ) ) { try { LODInitialFactorChanged?.Invoke( this ); } finally { _lODInitialFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODInitialFactor"/> property value changes.</summary>
		public event Action<Import3D> LODInitialFactorChanged;
		ReferenceField<double> _lODInitialFactor = 1.0;

		/// <summary>
		/// The quality factor for next level of detail, depending on the previous one.
		/// </summary>
		[DefaultValue( 0.4 )]
		[Range( 0, 1 )]
		[DisplayName( "LOD Reduction Factor" )]
		[Category( "Geometry" )]
		public Reference<double> LODReductionFactor
		{
			get { if( _lODReductionFactor.BeginGet() ) LODReductionFactor = _lODReductionFactor.Get( this ); return _lODReductionFactor.value; }
			set { if( _lODReductionFactor.BeginSet( ref value ) ) { try { LODReductionFactorChanged?.Invoke( this ); } finally { _lODReductionFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODReductionFactor"/> property value changes.</summary>
		public event Action<Import3D> LODReductionFactorChanged;
		ReferenceField<double> _lODReductionFactor = 0.4;

		/// <summary>
		/// The amount of levels of detail to generate.
		/// </summary>
		[DefaultValue( 4 )]
		[Range( 1, 6 )]
		[DisplayName( "LOD Levels" )]
		[Category( "Geometry" )]
		public Reference<int> LODLevels
		{
			get { if( _lODLevels.BeginGet() ) LODLevels = _lODLevels.Get( this ); return _lODLevels.value; }
			set { if( _lODLevels.BeginSet( ref value ) ) { try { LODLevelsChanged?.Invoke( this ); } finally { _lODLevels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODLevels"/> property value changes.</summary>
		public event Action<Import3D> LODLevelsChanged;
		ReferenceField<int> _lODLevels = 4;

		/// <summary>
		/// The distance from the previous to the next level of detail.
		/// </summary>
		[DisplayName( "LOD Distance" )]
		[Category( "Geometry" )]
		[DefaultValue( 15.0 )]
		[Range( 1, 50, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> LODDistance
		{
			get { if( _lODDistance.BeginGet() ) LODDistance = _lODDistance.Get( this ); return _lODDistance.value; }
			set { if( _lODDistance.BeginSet( ref value ) ) { try { LODDistanceChanged?.Invoke( this ); } finally { _lODDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODDistance"/> property value changes.</summary>
		public event Action<Import3D> LODDistanceChanged;
		ReferenceField<double> _lODDistance = 15.0;

		//public enum LODMethodEnum
		//{
		//	FastQuadricMeshSimplification,
		//}

		///// <summary>
		///// The method of automatic LOD generation.
		///// </summary>
		//[DefaultValue( LODMethodEnum.FastQuadricMeshSimplification )]
		//[DisplayName( "LOD Method" )]
		//[Category( "Import Settings" )]
		//public Reference<LODMethodEnum> LODMethod
		//{
		//	get { if( _lodMethod.BeginGet() ) LODMethod = _lodMethod.Get( this ); return _lodMethod.value; }
		//	set { if( _lodMethod.BeginSet( ref value ) ) { try { LODMethodChanged?.Invoke( this ); } finally { _lodMethod.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LODMethod"/> property value changes.</summary>
		//public event Action<Import3D> LODMethodChanged;
		//ReferenceField<LODMethodEnum> _lodMethod = LODMethodEnum.FastQuadricMeshSimplification;

		/// <summary>
		/// The mode of the last billboard LOD.
		/// </summary>
		[DefaultValue( MeshGeometry.BillboardDataModeEnum.None )]
		[Category( "Geometry" )]
		[DisplayName( "LOD Billboard Mode" )]
		public Reference<MeshGeometry.BillboardDataModeEnum> LODBillboardMode
		{
			get { if( _lodBillboardMode.BeginGet() ) LODBillboardMode = _lodBillboardMode.Get( this ); return _lodBillboardMode.value; }
			set { if( _lodBillboardMode.BeginSet( ref value ) ) { try { LODBillboardModeChanged?.Invoke( this ); } finally { _lodBillboardMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODBillboardMode"/> property value changes.</summary>
		public event Action<Import3D> LODBillboardModeChanged;
		ReferenceField<MeshGeometry.BillboardDataModeEnum> _lodBillboardMode = MeshGeometry.BillboardDataModeEnum.None;

		/// <summary>
		/// The images size of the last billboard LOD.
		/// </summary>
		[DefaultValue( 64 )]
		[Category( "Geometry" )]
		[DisplayName( "LOD Billboard Image Size" )]
		public Reference<int> LODBillboardImageSize
		{
			get { if( _lodBillboardImageSize.BeginGet() ) LODBillboardImageSize = _lodBillboardImageSize.Get( this ); return _lodBillboardImageSize.value; }
			set { if( _lodBillboardImageSize.BeginSet( ref value ) ) { try { LODBillboardImageSizeChanged?.Invoke( this ); } finally { _lodBillboardImageSize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODBillboardImageSize"/> property value changes.</summary>
		public event Action<Import3D> LODBillboardImageSizeChanged;
		ReferenceField<int> _lodBillboardImageSize = 64;

		////!!!!
		///// <summary>
		///// Whether to calculate the UV unwrapping channel.
		///// </summary>
		//[DefaultValue( false )]//!!!!включить по дефолту?
		//[Category( "Material" )]
		//[DisplayName( "UV Unwrap" )]
		////[Browsable( false )]//!!!!
		//public Reference<bool> UVUnwrap
		//{
		//	get { if( _uVUnwrap.BeginGet() ) UVUnwrap = _uVUnwrap.Get( this ); return _uVUnwrap.value; }
		//	set { if( _uVUnwrap.BeginSet( ref value ) ) { try { UVUnwrapChanged?.Invoke( this ); } finally { _uVUnwrap.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="UVUnwrap"/> property value changes.</summary>
		//public event Action<Import3D> UVUnwrapChanged;
		//ReferenceField<bool> _uVUnwrap = false;

		/// <summary>
		/// Whether to flip UV coordinates by vertical axis.
		/// </summary>
		[DefaultValue( false )]
		[Serialize]
		[Category( "Material" )]
		public Reference<bool> FlipUVs
		{
			get { if( _flipUVs.BeginGet() ) FlipUVs = _flipUVs.Get( this ); return _flipUVs.value; }
			set { if( _flipUVs.BeginSet( ref value ) ) { try { FlipUVsChanged?.Invoke( this ); } finally { _flipUVs.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlipUVs"/> property value changes.</summary>
		public event Action<Import3D> FlipUVsChanged;
		ReferenceField<bool> _flipUVs;

		/// <summary>
		/// Whether to delete unused materials.
		/// </summary>
		[DefaultValue( true )]
		[Serialize]
		[Category( "Material" )]
		public Reference<bool> DeleteUnusedMaterials
		{
			get { if( _deleteUnusedMaterials.BeginGet() ) DeleteUnusedMaterials = _deleteUnusedMaterials.Get( this ); return _deleteUnusedMaterials.value; }
			set { if( _deleteUnusedMaterials.BeginSet( ref value ) ) { try { DeleteUnusedMaterialsChanged?.Invoke( this ); } finally { _deleteUnusedMaterials.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DeleteUnusedMaterials"/> property value changes.</summary>
		public event Action<Import3D> DeleteUnusedMaterialsChanged;
		ReferenceField<bool> _deleteUnusedMaterials = true;

		/// <summary>
		/// Whether to import Displacement channel for materials.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Material" )]
		public Reference<bool> MaterialDisplacement
		{
			get { if( _materialDisplacement.BeginGet() ) MaterialDisplacement = _materialDisplacement.Get( this ); return _materialDisplacement.value; }
			set { if( _materialDisplacement.BeginSet( ref value ) ) { try { MaterialDisplacementChanged?.Invoke( this ); } finally { _materialDisplacement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialDisplacement"/> property value changes.</summary>
		public event Action<Import3D> MaterialDisplacementChanged;
		ReferenceField<bool> _materialDisplacement = false;

		//!!!!

		////!!!!description
		///// <summary>
		///// The mode of the mesh voxelization. For Auto mode the voxelized will use Basic mode when the number of mesh geometry less or equal 2.
		///// </summary>
		//[Category( "Voxelization" )]
		//[DefaultValue( MeshVoxelizationMode.Auto )]
		//public Reference<MeshVoxelizationMode> VoxelizationMode
		//{
		//	get { if( _voxelizationMode.BeginGet() ) VoxelizationMode = _voxelizationMode.Get( this ); return _voxelizationMode.value; }
		//	set { if( _voxelizationMode.BeginSet( ref value ) ) { try { VoxelizationModeChanged?.Invoke( this ); } finally { _voxelizationMode.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VoxelizationMode"/> property value changes.</summary>
		//public event Action<Import3D> VoxelizationModeChanged;
		//ReferenceField<MeshVoxelizationMode> _voxelizationMode = MeshVoxelizationMode.Auto;

		////!!!!default
		///// <summary>
		///// The size in texels of one side of the mesh voxelization data. It is used for NeoAxis Radiance global illumination technique.
		///// </summary>
		//[DefaultValue( MeshVoxelizationSize._32 )]
		//[Category( "Voxelization" )]
		//public Reference<MeshVoxelizationSize> VoxelizationSize
		//{
		//	get { if( _voxelizationSize.BeginGet() ) VoxelizationSize = _voxelizationSize.Get( this ); return _voxelizationSize.value; }
		//	set { if( _voxelizationSize.BeginSet( ref value ) ) { try { VoxelizationSizeChanged?.Invoke( this ); } finally { _voxelizationSize.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VoxelizationSize"/> property value changes.</summary>
		//public event Action<Import3D> VoxelizationSizeChanged;
		//ReferenceField<MeshVoxelizationSize> _voxelizationSize = MeshVoxelizationSize._32;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( OptimizeThreshold ):
					if( !Optimize )
						skip = true;
					break;

				case nameof( LODInitialFactor ):
				case nameof( LODReductionFactor ):
				case nameof( LODLevels ):
				case nameof( LODDistance ):
				case nameof( LODBillboardMode ):
					if( !LODGenerate )
						skip = true;
					break;

				case nameof( LODBillboardImageSize ):
					if( !LODGenerate || LODBillboardMode.Value == MeshGeometry.BillboardDataModeEnum.None )
						skip = true;
					break;

					//!!!!
				//case nameof( VoxelizationSize ):
				//	if( VoxelizationMode.Value == MeshVoxelizationMode.None )
				//		skip = true;
				//	break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( !insideDoUpdate && EnabledInHierarchy && IsNeedUpdate() )
			{
				if( !DoUpdate( null, out string error ) )
				{
					var virtualFileName = ParentRoot.HierarchyController?.CreatedByResource?.Owner.Name;
					if( string.IsNullOrEmpty( virtualFileName ) )
						virtualFileName = "NO FILE NAME";
					var error2 = $"Unable to load or import resource \"{virtualFileName}\".\r\n\r\n" + error;
					Log.Error( error2 );
					return;
				}

				//save to file
				if( HierarchyController != null && HierarchyController.CreatedByResource != null &&
					HierarchyController.CreatedByResource.InstanceType == Resource.InstanceType.Resource &&
					HierarchyController.CreatedByResource.Owner.LoadFromFile )
				{
					var resource = HierarchyController.CreatedByResource.Owner;
					string virtualPath = resource.Name + resource.GetSaveAddFileExtension();
					string realPath = VirtualPathUtility.GetRealPathByVirtual( virtualPath );

					if( !File.Exists( realPath ) && Components.Count != 0 )
					{
						if( !ComponentUtility.SaveComponentToFile( this, realPath, null, out error ) )
						{
							Log.Warning( error );
							return;
						}
					}
				}
			}
		}

		public bool IsNeedUpdate()
		{
			if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
			{
				if( Parent == null )
				{
					//!!!!good?
					if( Components.Count == 0 )
						return true;

					//!!!!хотя проверять тоже долго ведь. варианты: в дейплойменте сорцы удалить (сделать пустые файлы)
				}
			}

			return false;
		}

		/// <summary>
		/// Represents settings for reimporting data of <see cref="Import3D"/>.
		/// </summary>
		public class ReimportSettings
		{
			[DefaultValue( true )]
			[Category( "Options" )]
			//[Description( "" )]
			public bool UpdateMaterials { get; set; } = true;

			[DefaultValue( true )]
			[Category( "Options" )]
			public bool UpdateMeshes { get; set; } = true;

			[DefaultValue( true )]
			[Category( "Options" )]
			public bool UpdateObjectsInSpace { get; set; } = true;
		}
		
		//public delegate void UpdatePostProcessDelegate( Import3D import, ImportGeneral.Settings settings );
		//public static event UpdatePostProcessDelegate UpdatePostProcess;

		public bool DoUpdate( ReimportSettings reimportSettings, out string error )
		{
			ScreenNotifications.StickyNotificationItem notification = EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor ? ScreenNotifications.ShowSticky( "Importing..." ) : null;

			try
			{
				insideDoUpdate = true;

				if( reimportSettings == null )
					reimportSettings = new ReimportSettings();

				error = "";

				//bool createDocumentConfig = Components.Count == 0;

				//disable Import3D
				bool wasEnabled = Enabled;
				Enabled = false;
				try
				{
					var settings = new ImportGeneral.Settings();
					settings.updateMaterials = reimportSettings.UpdateMaterials;
					settings.updateMeshes = reimportSettings.UpdateMeshes;
					settings.updateObjectsInSpace = reimportSettings.UpdateObjectsInSpace;

					//save binding of materials settings
					if( !settings.updateMaterials )
					{
						var c = GetComponent( "Mesh" );
						if( c != null )
						{
							foreach( var geometry in c.GetComponents<MeshGeometry>( checkChildren: true ) )
							{
								var key = geometry.GetPathFromRoot();
								if( !string.IsNullOrEmpty( key ) && geometry.Material.ReferenceSpecified )
									settings.meshGeometryMaterialsToRestore[ key ] = geometry.Material.GetByReference;
							}
						}
						c = GetComponent( "Meshes" );
						if( c != null )
						{
							foreach( var geometry in c.GetComponents<MeshGeometry>( checkChildren: true ) )
							{
								var key = geometry.GetPathFromRoot();
								if( !string.IsNullOrEmpty( key ) && geometry.Material.ReferenceSpecified )
									settings.meshGeometryMaterialsToRestore[ key ] = geometry.Material.GetByReference;
							}
						}
					}

					//remove old objects
					if( settings.updateObjectsInSpace )
					{
						var c = GetComponent( "Object In Space" );
						if( c != null )
							RemoveComponent( c, false );
						c = GetComponent( "Objects In Space" );
						if( c != null )
							RemoveComponent( c, false );
					}
					if( settings.updateMeshes )
					{
						var c = GetComponent( "Mesh" );
						if( c != null )
							RemoveComponent( c, false );
						c = GetComponent( "Meshes" );
						if( c != null )
							RemoveComponent( c, false );
					}
					if( settings.updateMaterials )
					{
						var c = GetComponent( "Material" );
						if( c != null )
							RemoveComponent( c, false );
						c = GetComponent( "Materials" );
						if( c != null )
							RemoveComponent( c, false );
					}

					if( Parent != null || ParentRoot.HierarchyController == null || ParentRoot.HierarchyController.CreatedByResource == null ||
						!ParentRoot.HierarchyController.CreatedByResource.Owner.LoadFromFile )
					{
						error = "This component must be root of the resource.";
						return false;
					}
					var virtualFileName = ParentRoot.HierarchyController.CreatedByResource.Owner.Name;

					settings.component = this;
					settings.virtualFileName = virtualFileName;

					if( Path.GetExtension( virtualFileName ).ToLower() == ".fbx" )
					{
						//FBX

						//settings.loadAnimations = true;
						//!!!!to options?
						settings.frameStep = .25;

						EditorAssemblyInterface.Instance.ImportFBX( settings, out error );
						//Import.FBX.ImportFBX.DoImport( settings, out error );
						if( !string.IsNullOrEmpty( error ) )
							return false;
					}
					else
					{
						//Assimp

						//settings.loadAnimations = false;

						EditorAssemblyInterface.Instance.ImportAssimp( settings, out error );
						//ImportAssimp.DoImport( settings, out error );
						if( !string.IsNullOrEmpty( error ) )
							return false;
					}

					if( CenterBySize )
						DoCenterBySize();

					//if( UVUnwrap )
					//{
					//	if( !CalculateUVUnwrap( out error ) )
					//		return false;
					//}

					//merge equal vertices, remove extra triangles, 
					if( Optimize && OptimizeThreshold.Value > 0 )
						OptimizeByThreshold( false );

					//generate LODs and optimize
					if( LODGenerate )
					{
						if( !GenerateLODs( out error ) )
							return false;
					}

					//optimize for vertex cache
					if( Optimize && OptimizeThreshold.Value > 0 )
						OptimizeByThreshold( true );

					if( Optimize )
						OptimizeCaches();

					////generate LODs and optimize
					//if( LODGenerate || Optimize )
					//{
					//	if( !GenerateLODsAndOptimize( out error ) )
					//		return false;
					//}

					//UpdatePostProcess?.Invoke( this, settings );

					//DeleteUnusedMaterials
					if( settings.updateMaterials && DeleteUnusedMaterials && !settings.disableDeletionUnusedMaterials )
					{
						var usedMaterials = new ESet<Material>();
						foreach( var meshGeometry in GetComponents<MeshGeometry>( false, true ) )
						{
							var material = meshGeometry.Material.Value;
							if( material != null )
								usedMaterials.AddWithCheckAlreadyContained( material );
						}

						again:
						foreach( var material in GetComponents<Material>( false, true ) )
						{
							if( !usedMaterials.Contains( material ) )
							{
								material.RemoveFromParent( false );
								material.Dispose();
								goto again;
							}
						}
					}

					//restore materials of mesh geometries
					if( !settings.updateMaterials )
					{
						foreach( var item in settings.meshGeometryMaterialsToRestore )
						{
							var key = item.Key;
							var value = item.Value;

							var c = GetComponentByPath( key ) as MeshGeometry;
							if( c != null )
								c.Material = new Reference<Material>( null, value );
						}
					}

					//!!!!
					//if( VoxelizationMode.Value != MeshVoxelizationMode.None )
					//	Voxelize();
				}
				finally
				{
					//enable Import3D
					if( wasEnabled )
						Enabled = true;
				}

				//set EditorDocumentConfiguration
				//if( createDocumentConfig )
				{
					//!!!!что еще при Mode = Scene

					var toSelect = new List<Component>();

					//root object
					toSelect.Add( this );

					//mesh
					var mesh = GetComponent( "Mesh" ) as Mesh;
					if( mesh != null )
						toSelect.Add( mesh );

					//materials
					var materials = GetComponents<Material>( false, true );
					if( materials.Length <= 4 )
					{
						foreach( var material in materials )
						{
							var graph = material.GetComponent<FlowGraph>();
							if( graph != null && graph.TypeSettingsIsPublic() )
								toSelect.Add( graph );
						}
					}

					//select window with mesh or select window with root object
					var selectObject = mesh != null ? (Component)mesh : this;

					EditorDocumentConfiguration = KryptonConfigGenerator.CreateEditorDocumentXmlConfiguration( toSelect, selectObject );

					//update windows
					//!!!!какие-то еще проверки?
					if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor )
					{
						var document = EditorAPI.GetDocumentByObject( this );
						if( document != null )
						{
							////close old windows
							//EditorAPI.CloseAllDocumentWindowsOnSecondLevel( document );

							//open windows
							foreach( var obj in toSelect )
							{
								if( obj != this )
									EditorAPI.OpenDocumentWindowForObject( document, obj );
							}

							//delete old windows
							EditorAPI.CloseAllDocumentWindowsOnSecondLevelWithDeletedObjects( document );

							//select window
							var windows = EditorAPI.FindDocumentWindowsWithObject( selectObject );
							if( windows.Count != 0 )
								EditorAPI.SelectDockWindow( windows[ 0 ] );
						}

						//!!!!or restore window configuration from EditorDocumentConfiguration
					}

				}
			}
			finally
			{
				insideDoUpdate = false;

				notification?.Close();
			}

			return true;
		}

		[Browsable( false )]
		public long VersionForPreviewDisplay
		{
			get;
			set;
		}

		static void FixRootReferences( Import3D import3D )
		{
			bool Fix( IReference reference, out string newReference )
			{
				if( reference.ReferenceSpecified )
				{
					var referenceValue = reference.GetByReference;
					if( referenceValue.Length >= "root:".Length && referenceValue.Substring( 0, 5 ) == "root:" )
					{
						newReference = referenceValue.Insert( 5, "$Import3D\\" );
						return true;
					}
				}
				newReference = null;
				return false;
			}

			//MeshGeometry.Material
			foreach( var meshGeometry in import3D.GetComponents<MeshGeometry>( false, true ) )
			{
				if( Fix( meshGeometry.Material, out var newReference ) )
					meshGeometry.Material = ReferenceUtility.MakeReference<Material>( null, newReference );
			}

			//MeshInSpace.Mesh
			foreach( var meshInSpace in import3D.GetComponents<MeshInSpace>( false, true ) )
			{
				if( Fix( meshInSpace.Mesh, out var newReference ) )
					meshInSpace.Mesh = ReferenceUtility.MakeReference<Mesh>( null, newReference );
			}
		}

		public Import3D CreateForPreviewDisplay( Scene scene, out bool onlyOneMaterial, out Dictionary<Mesh, Transform> transformBySourceMesh )
		{
			onlyOneMaterial = false;
			transformBySourceMesh = new Dictionary<Mesh, Transform>();

			var createdObject = (Import3D)Clone();
			createdObject.Name = "Import3D";
			FixRootReferences( createdObject );
			scene.AddComponent( createdObject );

			//create MeshInSpace for one mesh
			{
				var mesh = createdObject.GetComponent( "Mesh" ) as Mesh;
				if( mesh != null )
				{
					var objectInSpace = createdObject.CreateComponent<MeshInSpace>();
					objectInSpace.Mesh = ReferenceUtility.MakeReference<Mesh>( null, ReferenceUtility.CalculateThisReference( objectInSpace, mesh ) );
				}
			}

			//create meshes in space for Meshes
			{
				var sourceMeshesGroup = GetComponent( "Meshes" );
				Mesh[] sourceMeshes = null;
				if( sourceMeshesGroup != null )
					sourceMeshes = sourceMeshesGroup.GetComponents<Mesh>();

				var createdMeshesGroup = createdObject.GetComponent( "Meshes" );
				if( createdMeshesGroup != null )
				{
					var createdMeshes = createdMeshesGroup.GetComponents<Mesh>();

					var positions = new Vector3[ createdMeshes.Length ];
					{
						if( sourceMeshesGroup != null )
						{
							double maxSizeY = 0;
							foreach( var sourceMesh in sourceMeshesGroup.GetComponents<Mesh>() )
							{
								if( sourceMesh.Result != null )
								{
									var bounds = sourceMesh.Result.SpaceBounds.CalculatedBoundingBox;
									var sizeY = bounds.GetSize().Y;
									if( sizeY > maxSizeY )
										maxSizeY = sizeY;
								}
							}
							var step = maxSizeY * 1.2;
							var totalSize = (double)( createdMeshes.Length - 1 ) * step;

							for( int n = 0; n < createdMeshes.Length; n++ )
							{
								var positionY = (double)n * step - totalSize / 2;
								positions[ n ] = new Vector3( 0, positionY, 0 );
							}
						}
					}

					for( int n = 0; n < createdMeshes.Length; n++ )
					{
						var createdMesh = createdMeshes[ n ];

						var transform = new Transform( positions[ n ], Quaternion.Identity );

						var objectInSpace = createdObject.CreateComponent<MeshInSpace>();
						objectInSpace.Transform = transform;
						objectInSpace.Mesh = ReferenceUtility.MakeReference<Mesh>( null, ReferenceUtility.CalculateThisReference( objectInSpace, createdMesh ) );

						if( sourceMeshes != null && sourceMeshes.Length == createdMeshes.Length )
							transformBySourceMesh[ sourceMeshes[ n ] ] = transform;
					}
				}
			}

			//create MeshInSpace for only one material
			{
				var material = createdObject.GetComponent( "Material" ) as Material;
				if( material != null )
				{
					var mesh = ProjectSettings.Get.General.MaterialPreviewMesh.Value;
					if( mesh != null )
					{
						var objectInSpace = createdObject.CreateComponent<MeshInSpace>();
						objectInSpace.Mesh = mesh;

						objectInSpace.ReplaceMaterial = ReferenceUtility.MakeReference<Material>( null, ReferenceUtility.CalculateThisReference( objectInSpace, material ) );

						onlyOneMaterial = true;
					}
				}
			}

			return createdObject;
		}

		static unsafe void VerticesWriteChannel<T>( VertexElement element, T[] data, byte[] writeToVertices, int vertexSize, int vertexCount ) where T : unmanaged
		{
			if( VertexElement.GetSizeInBytes( element.Type ) == sizeof( T ) )
			{
				fixed( byte* pVertices = writeToVertices )
				{
					byte* src = pVertices + element.Offset;
					for( int n = 0; n < vertexCount; n++ )
					{
						*(T*)src = data[ n ];
						src += vertexSize;
					}
				}
			}
		}

		List<Mesh> GetMeshes( bool lods )
		{
			var result = new List<Mesh>();

			var c = GetComponent( "Mesh" );
			if( c != null )
			{
				var mesh = c as Mesh;
				if( mesh != null )
					result.Add( mesh );
			}
			c = GetComponent( "Meshes" );
			if( c != null )
			{
				foreach( var cc in c.Components )
				{
					var mesh = cc as Mesh;
					if( mesh != null )
						result.Add( mesh );
				}
			}

			if( lods )
			{
				var newMeshes = new List<Mesh>();
				foreach( var mesh in result )
					newMeshes.AddRange( mesh.GetComponents<Mesh>( checkChildren: true ) );
				result = newMeshes;
			}

			return result;
		}

		bool GenerateLODs( out string error )
		{
			error = "";

#if !DEPLOY
			var sourceMeshes = GetMeshes( false );

			var initialFactor = LODGenerate.Value ? LODInitialFactor.Value : 1.0;

			foreach( var sourceMesh in sourceMeshes )
			{
				var currentQuality = initialFactor;

				//get source mesh geometries
				var sourceGeometries = new List<MeshGeometry>();
				{
					foreach( var geometry in sourceMesh.GetComponents<MeshGeometry>() )
						if( geometry.VertexStructure.Value != null && geometry.Vertices.Value != null && geometry.Indices.Value != null )
							sourceGeometries.Add( geometry );
				}

				var lodLevels = LODGenerate.Value ? LODLevels.Value : 1;

				var lodBillboardIndex = -1;
				if( LODGenerate.Value && LODBillboardMode.Value != MeshGeometry.BillboardDataModeEnum.None )
					lodBillboardIndex = lodLevels - 1;

				var meshes = new List<Mesh>();
				meshes.Add( sourceMesh );

				for( int lodIndex = 0; lodIndex < lodLevels; lodIndex++ )
				{

					if( lodIndex != 0 || initialFactor != 1 )//|| Optimize )
					{
						var newGeometries = new List<MeshGeometry>();

						foreach( var sourceGeometry in sourceGeometries )
						{
							var structure = sourceGeometry.VertexStructure.Value;
							VertexElements.GetInfo( structure, out var vertexSize, out _ );


							var simplifier = new MeshSimplifier();

							var options = MeshSimplifier.SimplificationOptionsStruct.Default;
							//options.PreserveBorderEdges = true;
							//options.PreserveUVSeamEdges = true;
							//options.PreserveUVFoldoverEdges = true;
							//options.PreserveSurfaceCurvature = true;
							options.VertexLinkDistance = 0.001;
							//options.VertexLinkDistance = 0.00001;
							//options.MaxIterationCount = 200;
							//options.Agressiveness = 14.0;
							simplifier.SimplificationOptions = options;

							foreach( var element in structure )
							{
								switch( element.Semantic )
								{
								case VertexElementSemantic.Position:
									{
										var v = sourceGeometry.VerticesExtractChannel<Vector3F>( element.Semantic );
										if( v != null )
											simplifier.Vertices = v.Select( v2 => v2.ToVector3() ).ToArray();
									}
									break;

								case VertexElementSemantic.Normal:
									{
										var v = sourceGeometry.VerticesExtractChannel<Vector3F>( element.Semantic );
										if( v != null )
											simplifier.Normals = v.Select( v2 => v2.ToVector3() ).ToArray();
									}
									break;

								case VertexElementSemantic.Tangent:
									{
										var v = sourceGeometry.VerticesExtractChannel<Vector4F>( element.Semantic );
										if( v != null )
											simplifier.Tangents = v.Select( v2 => v2.ToVector4() ).ToArray();
									}
									break;

								case VertexElementSemantic.TextureCoordinate0:
								case VertexElementSemantic.TextureCoordinate1:
								case VertexElementSemantic.TextureCoordinate2:
								case VertexElementSemantic.TextureCoordinate3:
								case VertexElementSemantic.TextureCoordinate4:
								case VertexElementSemantic.TextureCoordinate5:
								case VertexElementSemantic.TextureCoordinate6:
								case VertexElementSemantic.TextureCoordinate7:
									{
										var v = sourceGeometry.VerticesExtractChannel<Vector2F>( element.Semantic );
										if( v != null )
										{
											var channel = element.Semantic - VertexElementSemantic.TextureCoordinate0;
											simplifier.SetUVs( channel, v.Select( v2 => v2.ToVector2() ).ToArray() );
										}
									}
									break;

								case VertexElementSemantic.Color0:
									{
										if( element.Type == VertexElementType.Float4 )
										{
											var v = sourceGeometry.VerticesExtractChannel<Vector4F>( element.Semantic );
											simplifier.Colors = v.Select( v2 => v2.ToColorValue() ).ToArray();
										}
										else if( element.Type == VertexElementType.ColorABGR )
										{
											//!!!!check
											var v = sourceGeometry.VerticesExtractChannel<uint>( element.Semantic );
											simplifier.Colors = v.Select( v2 => ColorByte.FromABGR( v2 ).ToColorValue() ).ToArray();
										}
										else if( element.Type == VertexElementType.ColorARGB )
										{
											//!!!!check
											var v = sourceGeometry.VerticesExtractChannel<uint>( element.Semantic );
											simplifier.Colors = v.Select( v2 => ColorByte.FromARGB( v2 ).ToColorValue() ).ToArray();
										}
									}
									break;

								case VertexElementSemantic.BlendIndices:
									{
										var indices = sourceGeometry.VerticesExtractChannel<Vector4I>( VertexElementSemantic.BlendIndices );
										var weights = sourceGeometry.VerticesExtractChannel<Vector4F>( VertexElementSemantic.BlendWeights );
										if( indices != null && weights != null )
										{
											var array = new MeshSimplifier.BoneWeight[ indices.Length ];
											for( int n = 0; n < array.Length; n++ )
												array[ n ] = new MeshSimplifier.BoneWeight( indices[ n ], weights[ n ] );
											simplifier.BoneWeights = array;
										}
									}
									break;
								}
							}

							//this.bindposes = mesh.bindposes;

							simplifier.AddSubMeshTriangles( sourceGeometry.Indices );

							if( lodBillboardIndex != lodIndex )
							{
								try
								{
									//if( lodIndex == 0 && initialFactor == 1 && Optimize )
									//	simplifier.SimplifyMeshLossless( OptimizeThreshold );
									//else
									simplifier.SimplifyMesh( (float)currentQuality );

								}
								catch( Exception e )
								{
									error = "LOD generating failed. " + e.Message;
									return false;
								}
							}

							var vertexCount = simplifier.Vertices.Length;
							var newVertices = new byte[ vertexCount * vertexSize ];

							foreach( var element in structure )
							{
								switch( element.Semantic )
								{
								case VertexElementSemantic.Position:
									if( simplifier.Vertices != null )
									{
										var v = simplifier.Vertices.Select( v2 => v2.ToVector3F() ).ToArray();
										VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
									}
									break;

								case VertexElementSemantic.Normal:
									if( simplifier.Normals != null )
									{
										var v = simplifier.Normals.Select( v2 => v2.ToVector3F() ).ToArray();
										VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
									}
									break;

								case VertexElementSemantic.Tangent:
									if( simplifier.Tangents != null )
									{
										var v = simplifier.Tangents.Select( v2 => v2.ToVector4F() ).ToArray();
										VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
									}
									break;

								case VertexElementSemantic.TextureCoordinate0:
								case VertexElementSemantic.TextureCoordinate1:
								case VertexElementSemantic.TextureCoordinate2:
								case VertexElementSemantic.TextureCoordinate3:
								case VertexElementSemantic.TextureCoordinate4:
								case VertexElementSemantic.TextureCoordinate5:
								case VertexElementSemantic.TextureCoordinate6:
								case VertexElementSemantic.TextureCoordinate7:
									{
										var channel = element.Semantic - VertexElementSemantic.TextureCoordinate0;
										var uv = simplifier.GetUVs2D( channel );
										if( uv != null )
										{
											var v = uv.Select( v2 => v2.ToVector2F() ).ToArray();
											VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
										}
									}
									break;

								case VertexElementSemantic.Color0:
									if( simplifier.Colors != null )
									{
										if( element.Type == VertexElementType.Float4 )
										{
											var v = simplifier.Colors.Select( v2 => v2.ToVector4F() ).ToArray();
											VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
										}
										else if( element.Type == VertexElementType.ColorABGR )
										{
											//!!!!check
											var v = simplifier.Colors.Select( v2 => v2.ToColorPacked().ToABGR() ).ToArray();
											VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
										}
										else if( element.Type == VertexElementType.ColorARGB )
										{
											//!!!!check
											var v = simplifier.Colors.Select( v2 => v2.ToColorPacked().ToARGB() ).ToArray();
											VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
										}
									}
									break;

								case VertexElementSemantic.BlendIndices:
									if( simplifier.BoneWeights != null )
									{
										var v = simplifier.BoneWeights.Select( v2 => v2.Indices ).ToArray();
										VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
									}
									break;

								case VertexElementSemantic.BlendWeights:
									if( simplifier.BoneWeights != null )
									{
										var v = simplifier.BoneWeights.Select( v2 => v2.Weights ).ToArray();
										VerticesWriteChannel( element, v, newVertices, vertexSize, vertexCount );
									}
									break;
								}
							}

							var newIndices = simplifier.GetSubMeshTriangles( 0 );


							//create mesh geometry
							var newGeometry = ComponentUtility.CreateComponent<MeshGeometry>( null, false, true );
							newGeometry.Name = sourceGeometry.Name;
							newGeometry.VertexStructure = structure;
							newGeometry.UnwrappedUV = sourceGeometry.UnwrappedUV;
							newGeometry.Vertices = newVertices;
							newGeometry.Indices = newIndices;
							newGeometry.Material = sourceGeometry.Material;

							newGeometries.Add( newGeometry );
						}

						if( lodIndex == 0 )
						{
							//update source mesh

							foreach( var geometry in sourceMesh.GetComponents<MeshGeometry>() )
								sourceMesh.RemoveComponent( geometry, false );

							foreach( var geometry in newGeometries )
								sourceMesh.AddComponent( geometry );
						}
						else
						{
							//create level of detail components

							var lod = sourceMesh.CreateComponent<MeshLevelOfDetail>();
							lod.Name = "LOD " + lodIndex.ToString();
							lod.Distance = LODDistance.Value * (double)lodIndex;

							var lodMesh = lod.CreateComponent<Mesh>();
							lodMesh.Name = "Mesh";
							lod.Mesh = ReferenceUtility.MakeRootReference( lodMesh );

							foreach( var geometry in newGeometries )
								lodMesh.AddComponent( geometry );

							meshes.Add( lodMesh );
						}

					}

					currentQuality *= LODReductionFactor.Value;
				}

				//convert usual mesh to billboard
				if( lodBillboardIndex != -1 && lodBillboardIndex < meshes.Count )
				{
					var mesh = meshes[ lodBillboardIndex ];

					//!!!!CenteringByXY

					mesh.ConvertToBillboard( LODBillboardMode, LODBillboardImageSize, false, true );
					mesh.Billboard = true;
					//mesh.BillboardShadowOffset = 0;
				}

			}
#endif

			return true;
		}

		//bool CalculateUVUnwrap( out string error )
		//{
		//	error = "";

		//	//!!!!если канал есть то не нужно

		//	var sourceMeshes = new List<Mesh>();
		//	{
		//		var c = GetComponent( "Mesh" );
		//		if( c != null )
		//		{
		//			var mesh = c as Mesh;
		//			if( mesh != null )
		//				sourceMeshes.Add( mesh );
		//		}
		//		c = GetComponent( "Meshes" );
		//		if( c != null )
		//		{
		//			foreach( var cc in c.Components )
		//			{
		//				var mesh = cc as Mesh;
		//				if( mesh != null )
		//					sourceMeshes.Add( mesh );
		//			}
		//		}
		//	}

		//	foreach( var sourceMesh in sourceMeshes )
		//	{
		//		//get source mesh geometries
		//		var sourceGeometries = new List<MeshGeometry>();
		//		{
		//			foreach( var geometry in sourceMesh.GetComponents<MeshGeometry>() )
		//				if( geometry.VertexStructure.Value != null && geometry.Vertices.Value != null && geometry.Indices.Value != null )
		//					sourceGeometries.Add( geometry );
		//		}

		//		foreach( var geometry in sourceGeometries )
		//		{
		//			if( geometry.UnwrappedUV.Value == UnwrappedUVEnum.None )
		//			{

		//				//!!!!может для генерации лодов применить VerticesExtractStandardVertex. или еще где-то

		//				//!!!!настройки генератора

		//				//!!!!
		//				var structure = geometry.VertexStructure.Value;


		//				VertexElement? sourceTexCoordElement = null;
		//				if( structure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate0, out var element ) )
		//					sourceTexCoordElement = element;
		//				else if( structure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate1, out element ) )
		//					sourceTexCoordElement = element;
		//				else if( structure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate2, out element ) )
		//					sourceTexCoordElement = element;

		//				//!!!!
		//				//if( !texCoordElement.HasValue )
		//				//{
		//				//}

		//				if( sourceTexCoordElement.HasValue )
		//				{
		//					VertexElement? freeTexCoordElement = null;

		//					//!!!!replace last channel?

		//					var semantic = (VertexElementSemantic)( (int)sourceTexCoordElement.Value.Semantic + 1 );
		//					for( ; semantic <= VertexElementSemantic.TextureCoordinate2; semantic++ )
		//					{
		//						if( !structure.GetElementBySemantic( semantic, out element ) )
		//						{
		//							freeTexCoordElement = element;
		//							break;
		//						}
		//					}

		//					//!!!!
		//					//if( !freeTexCoordElement.HasValue )
		//					//{
		//					//}

		//					if( freeTexCoordElement.HasValue )
		//					{

		//						//!!!!try, catch

		//						//geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );

		//						//zzzzz;
		//						//geometry.extra

		//						//geometry.UnwrappedUV = (UnwrappedUVEnum)( (int)freeTexCoordElement.Value.Semantic - (int)VertexElementSemantic.TextureCoordinate0 + (int)UnwrappedUVEnum.TextureCoordinate0 );


		//						//!!!!undo?

		//					}
		//				}

		//			}
		//		}
		//	}

		//	return true;
		//}

		void DoCenterBySize()
		{
			var meshes = GetMeshes( false );

			foreach( var mesh in meshes )
			{
				var geometries = new List<MeshGeometry>();
				{
					foreach( var geometry in mesh.GetComponents<MeshGeometry>() )
						if( geometry.VertexStructure.Value != null && geometry.Vertices.Value != null && geometry.Indices.Value != null )
							geometries.Add( geometry );
				}

				//calculate offset
				Vector3F offset = Vector3F.Zero;
				{
					var bounds = BoundsF.Cleared;

					foreach( var geometry in geometries )
					{
						var structure = geometry.VertexStructure.Value;
						VertexElements.GetInfo( structure, out var vertexSize, out _ );

						if( structure.GetElementBySemantic( VertexElementSemantic.Position, out var element ) && element.Type == VertexElementType.Float3 )
						{
							var positions = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
							if( positions != null )
								bounds.Add( positions );
						}
					}

					offset = -bounds.GetCenter();
				}

				//update vertices
				foreach( var geometry in geometries )
				{
					var positions = geometry.VerticesExtractChannel<Vector3F>( VertexElementSemantic.Position );
					if( positions != null )
					{
						for( int n = 0; n < positions.Length; n++ )
							positions[ n ] = positions[ n ] + offset;

						var vertices = (byte[])geometry.Vertices.Value.Clone();
						geometry.VerticesWriteChannel( VertexElementSemantic.Position, positions, vertices );
						geometry.Vertices = vertices;
					}
				}
			}
		}

		void OptimizeByThreshold( bool lods )
		{
			var meshes = GetMeshes( lods );

			var vertexPositionEpsilon = (float)OptimizeThreshold.Value;
			var vertexOtherChannelsEpsilon = vertexPositionEpsilon * 5;

			foreach( var mesh in meshes )
			{
				if( !mesh.Billboard )
				{
					foreach( var geometry in mesh.GetComponents<MeshGeometry>() )
					{
						if( geometry.VertexStructure.Value != null && geometry.Vertices.Value != null && geometry.Indices.Value != null )
						{
							if( geometry.VerticesExtractStandardVertex( out var vertices, out var vertexComponents ) )
							{
								var indices = geometry.Indices.Value;

								MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( vertices, indices, 0, vertexPositionEpsilon, vertexOtherChannelsEpsilon, out var newVertices, out var newIndices, out _ );

								geometry.SetVertexData( newVertices, vertexComponents );
								geometry.Indices = newIndices;
							}
						}
					}
				}
			}

		}

		void OptimizeCaches()
		{
			var meshes = GetMeshes( false );
			meshes.AddRange( GetMeshes( true ) );

			foreach( var mesh in meshes )
			{
				if( !mesh.Billboard )
				{
					foreach( var geometry in mesh.GetComponents<MeshGeometry>() )
					{
						try
						{
							geometry.OptimizeVertexCache();
							geometry.OptimizeOverdraw();
							geometry.OptimizeVertexFetch();
						}
						catch { }
					}
				}
			}
		}

		//!!!!
		//void Voxelize()
		//{
		//	var meshes = GetMeshes( false );

		//	foreach( var mesh in meshes )
		//	{
		//		if( !mesh.Billboard )
		//			mesh.Voxelize( VoxelizationMode, VoxelizationSize );
		//	}
		//}
	}
}
