// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoAxis.Editor;
using Internal;
using Microsoft.CodeAnalysis;

namespace NeoAxis
{
	/// <summary>
	/// The component for import of 3D content.
	/// </summary>
#if !DEPLOY
	[EditorControl( "NeoAxis.Editor.Import3DEditor" )]
	[Preview( "NeoAxis.Editor.Import3DPreview" )]
	[PreviewImage( "NeoAxis.Editor.Import3DPreviewImage" )]
	[SettingsCell( "NeoAxis.Editor.Import3DSettingsCell" )]
#endif
	public class Import3D : Component
	{
#if !DEPLOY
		bool insideDoUpdate;

		public enum ModeEnum
		{
			//Auto,
			OneMesh,
			Meshes,
			//Scene,
		}
		/// <summary>
		/// The mode of the import. You can import into one mesh or to make a set of separated meshes.
		/// </summary>
		[DefaultValue( ModeEnum.OneMesh )]
		[Serialize]
		[Category( "Basic" )]
		public Reference<ModeEnum> Mode
		{
			get { if( _mode.BeginGet() ) Mode = _mode.Get( this ); return _mode.value; }
			set { if( _mode.BeginSet( this, ref value ) ) { try { ModeChanged?.Invoke( this ); } finally { _mode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Mode"/> property value changes.</summary>
		public event Action<Import3D> ModeChanged;
		ReferenceField<ModeEnum> _mode = ModeEnum.OneMesh;//Auto;

		/// <summary>
		/// Specifies a result position offset of imported 3D models.
		/// </summary>
		[DefaultValue( "0 0 0" )]
		[Category( "Transform" )]
		public Reference<Vector3> Position
		{
			get { if( _position.BeginGet() ) Position = _position.Get( this ); return _position.value; }
			set { if( _position.BeginSet( this, ref value ) ) { try { PositionChanged?.Invoke( this ); } finally { _position.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Position"/> property value changes.</summary>
		public event Action<Import3D> PositionChanged;
		ReferenceField<Vector3> _position;

		/// <summary>
		/// Specifies a result rotation of imported 3D models. Expand the property to specify rotation by euler angles.
		/// </summary>
		[DefaultValue( "0 0 0 1" )]
		[Serialize]
		[Category( "Transform" )]
		public Reference<Quaternion> Rotation
		{
			get { if( _rotation.BeginGet() ) Rotation = _rotation.Get( this ); return _rotation.value; }
			set { if( _rotation.BeginSet( this, ref value ) ) { try { RotationChanged?.Invoke( this ); } finally { _rotation.EndSet(); } } }
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
			set { if( _scale.BeginSet( this, ref value ) ) { try { ScaleChanged?.Invoke( this ); } finally { _scale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Scale"/> property value changes.</summary>
		public event Action<Import3D> ScaleChanged;
		ReferenceField<double> _scale = 1.0;

		/// <summary>
		/// Whether to rotate models to engine's coordinate system. X axis is forward, Z axis is up.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Transform" )]
		public Reference<bool> FixAxes
		{
			get { if( _fixAxes.BeginGet() ) FixAxes = _fixAxes.Get( this ); return _fixAxes.value; }
			set { if( _fixAxes.BeginSet( this, ref value ) ) { try { FixAxesChanged?.Invoke( this ); } finally { _fixAxes.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FixAxes"/> property value changes.</summary>
		public event Action<Import3D> FixAxesChanged;
		ReferenceField<bool> _fixAxes = true;

		/// <summary>
		/// Whether to move models to center by result bounding box.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Transform" )]
		public Reference<bool> CenterBySize
		{
			get { if( _centerBySize.BeginGet() ) CenterBySize = _centerBySize.Get( this ); return _centerBySize.value; }
			set { if( _centerBySize.BeginSet( this, ref value ) ) { try { CenterBySizeChanged?.Invoke( this ); } finally { _centerBySize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CenterBySize"/> property value changes.</summary>
		public event Action<Import3D> CenterBySizeChanged;
		ReferenceField<bool> _centerBySize = false;

		public enum SimplifyMethodEnum
		{
			None,
			Careful,
			Usual,
		}

		/// <summary>
		/// The way to reduce amount of triangles.
		/// </summary>
		[DefaultValue( SimplifyMethodEnum.None )]
		[Category( "Geometry" )]
		public Reference<SimplifyMethodEnum> Simplify
		{
			get { if( _simplify.BeginGet() ) Simplify = _simplify.Get( this ); return _simplify.value; }
			set { if( _simplify.BeginSet( this, ref value ) ) { try { SimplifyChanged?.Invoke( this ); } finally { _simplify.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Simplify"/> property value changes.</summary>
		public event Action<Import3D> SimplifyChanged;
		ReferenceField<SimplifyMethodEnum> _simplify = SimplifyMethodEnum.None;

		/// <summary>
		/// The factor to simplify initial geometry. The reduction makes less amount of triangles with some quality decreasing.
		/// </summary>
		[DefaultValue( 0.5 )]
		[Range( 0, 1 )]
		[Category( "Geometry" )]
		public Reference<double> SimplifyQuality
		{
			get { if( _simplifyQuality.BeginGet() ) SimplifyQuality = _simplifyQuality.Get( this ); return _simplifyQuality.value; }
			set { if( _simplifyQuality.BeginSet( this, ref value ) ) { try { SimplifyQualityChanged?.Invoke( this ); } finally { _simplifyQuality.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SimplifyQuality"/> property value changes.</summary>
		public event Action<Import3D> SimplifyQualityChanged;
		ReferenceField<double> _simplifyQuality = 0.5;

		///// <summary>
		///// The factor to simplify initial geometry. The reduction makes less amount of triangles with some quality decreasing.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//[Range( 0, 1 )]
		//[Category( "Geometry" )]
		//public Reference<double> Simplify
		//{
		//	get { if( _simplify.BeginGet() ) Simplify = _simplify.Get( this ); return _simplify.value; }
		//	set { if( _simplify.BeginSet( this, ref value ) ) { try { SimplifyChanged?.Invoke( this ); } finally { _simplify.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Simplify"/> property value changes.</summary>
		//public event Action<Import3D> SimplifyChanged;
		//ReferenceField<double> _simplify = 1.0;

		///// <summary>
		///// The factor to simplify initial geometry. The reduction makes less amount of triangles with some quality decreasing.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//[Range( 0, 1 )]
		//[Category( "Geometry" )]
		//public Reference<double> Simplify
		//{
		//	get { if( _simplify.BeginGet() ) Simplify = _simplify.Get( this ); return _simplify.value; }
		//	set { if( _simplify.BeginSet( this, ref value ) ) { try { SimplifyChanged?.Invoke( this ); } finally { _simplify.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Simplify"/> property value changes.</summary>
		//public event Action<Import3D> SimplifyChanged;
		//ReferenceField<double> _simplify = 1.0;

		public enum MergeGeometriesEnum
		{
			False,
			MergeWithSameFormat,
			MultiMaterial
		}

		/// <summary>
		/// Whether to merge mesh geometries. The mesh with one geometry will be rendered most effectively, it can be done by using Multi Material setting.
		/// </summary>
		[DefaultValue( MergeGeometriesEnum.MultiMaterial )]
		[Category( "Geometry" )]
		public Reference<MergeGeometriesEnum> MergeGeometries
		{
			get { if( _mergeGeometries.BeginGet() ) MergeGeometries = _mergeGeometries.Get( this ); return _mergeGeometries.value; }
			set { if( _mergeGeometries.BeginSet( this, ref value ) ) { try { MergeGeometriesChanged?.Invoke( this ); } finally { _mergeGeometries.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MergeGeometries"/> property value changes.</summary>
		public event Action<Import3D> MergeGeometriesChanged;
		ReferenceField<MergeGeometriesEnum> _mergeGeometries = MergeGeometriesEnum.MultiMaterial;

		///// <summary>
		///// Whether to use enable a virtualized geometry optimization which indended to optimize the rendering of high-poly models.
		///// </summary>
		//[DefaultValue( false )]
		//[Category( "Geometry" )]
		//public Reference<bool> Virtualize
		//{
		//	get { if( _virtualize.BeginGet() ) Virtualize = _virtualize.Get( this ); return _virtualize.value; }
		//	set { if( _virtualize.BeginSet( this, ref value ) ) { try { VirtualizeChanged?.Invoke( this ); } finally { _virtualize.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Virtualize"/> property value changes.</summary>
		//public event Action<Import3D> VirtualizeChanged;
		//ReferenceField<bool> _virtualize = false;

		//[DefaultValue( 0.0 )]
		//[Range( 0, 1 )]
		//[Category( "Geometry" )]
		//public Reference<double> VirtualizeProxyFactor
		//{
		//	get { if( _virtualizeProxyFactor.BeginGet() ) VirtualizeProxyFactor = _virtualizeProxyFactor.Get( this ); return _virtualizeProxyFactor.value; }
		//	set { if( _virtualizeProxyFactor.BeginSet( this, ref value ) ) { try { VirtualizeProxyFactorChanged?.Invoke( this ); } finally { _virtualizeProxyFactor.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VirtualizeProxyFactor"/> property value changes.</summary>
		//public event Action<Import3D> VirtualizeProxyFactorChanged;
		//ReferenceField<double> _virtualizeProxyFactor = 0.0;


		///// <summary>
		///// Whether to calculate clusters, which are intended to increase rendering speed of high-poly models. The clusters do not simplify the model, there is no quality loss.
		///// </summary>
		//[DefaultValue( false )]
		//[Category( "Geometry" )]
		//public Reference<bool> Clusters
		//{
		//	get { if( _clusters.BeginGet() ) Clusters = _clusters.Get( this ); return _clusters.value; }
		//	set { if( _clusters.BeginSet( this, ref value ) ) { try { ClustersChanged?.Invoke( this ); } finally { _clusters.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="Clusters"/> property value changes.</summary>
		//public event Action<Import3D> ClustersChanged;
		//ReferenceField<bool> _clusters = false;

		///// <summary>
		///// The allowed cell size range of an internal grid, which is intended to speed up the rendering. '0 0' value means the cell size will select automatically.
		///// </summary>
		//[DefaultValue( "0 0" )]
		//[Category( "Geometry" )]
		//public Reference<Range> ClusterCellSize
		//{
		//	get { if( _clusterCellSize.BeginGet() ) ClusterCellSize = _clusterCellSize.Get( this ); return _clusterCellSize.value; }
		//	set { if( _clusterCellSize.BeginSet( this, ref value ) ) { try { ClusterCellSizeChanged?.Invoke( this ); } finally { _clusterCellSize.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ClusterCellSize"/> property value changes.</summary>
		//public event Action<Import3D> ClusterCellSizeChanged;
		//ReferenceField<Range> _clusterCellSize = Range.Zero;

		/// <summary>
		/// Whether to generate level of detail meshes.
		/// </summary>
		[DefaultValue( true )]
		[DisplayName( "LODs" )]
		[Category( "Geometry" )]
		public Reference<bool> LODs
		{
			get { if( _lods.BeginGet() ) LODs = _lods.Get( this ); return _lods.value; }
			set { if( _lods.BeginSet( this, ref value ) ) { try { LODsChanged?.Invoke( this ); } finally { _lods.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODs"/> property value changes.</summary>
		public event Action<Import3D> LODsChanged;
		ReferenceField<bool> _lods = true;

		/// <summary>
		/// The result amount of levels of detail.
		/// </summary>
		[DefaultValue( 4 )]
		[Range( 1, 6 )]
		[DisplayName( "LOD Levels" )]
		[Category( "Geometry" )]
		public Reference<int> LODLevels
		{
			get { if( _lODLevels.BeginGet() ) LODLevels = _lODLevels.Get( this ); return _lODLevels.value; }
			set { if( _lODLevels.BeginSet( this, ref value ) ) { try { LODLevelsChanged?.Invoke( this ); } finally { _lODLevels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODLevels"/> property value changes.</summary>
		public event Action<Import3D> LODLevelsChanged;
		ReferenceField<int> _lODLevels = 4;

		///// <summary>
		///// The method of calculation a simplified mesh.
		///// </summary>
		//[DefaultValue( MeshSimplificationMethod.Clusters )]
		//[DisplayName( "LOD Method" )]
		//[Category( "Geometry" )]
		//public Reference<MeshSimplificationMethod> LODMethod
		//{
		//	get { if( _lODMethod.BeginGet() ) LODMethod = _lODMethod.Get( this ); return _lODMethod.value; }
		//	set { if( _lODMethod.BeginSet( this, ref value ) ) { try { LODMethodChanged?.Invoke( this ); } finally { _lODMethod.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LODMethod"/> property value changes.</summary>
		//public event Action<Import3D> LODMethodChanged;
		//ReferenceField<MeshSimplificationMethod> _lODMethod = MeshSimplificationMethod.Clusters;

		/// <summary>
		/// The quality factor for next level of detail, depending on the previous one. Set 0 to use auto mode.
		/// </summary>
		[DefaultValue( 0.0 )]//0.5 )]
		[Range( 0, 1 )]
		[DisplayName( "LOD Reduction" )]
		[Category( "Geometry" )]
		public Reference<double> LODReduction
		{
			get { if( _lODReduction.BeginGet() ) LODReduction = _lODReduction.Get( this ); return _lODReduction.value; }
			set { if( _lODReduction.BeginSet( this, ref value ) ) { try { LODReductionChanged?.Invoke( this ); } finally { _lODReduction.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODReduction"/> property value changes.</summary>
		public event Action<Import3D> LODReductionChanged;
		ReferenceField<double> _lODReduction = 0.0;//0.5;

		/// <summary>
		/// The distance from the previous to the next level of detail. In the default scheme with the one voxel lod, this parameter has sense only when want to move the start distance of the voxel lod. Regardless of the settings, the voxel lod will not turn on while the quality is too small.
		/// </summary>
		[DisplayName( "LOD Distance" )]
		[Category( "Geometry" )]
		[DefaultValue( 0.0 )]//15.0 )]
		[Range( 1, 50, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> LODDistance
		{
			get { if( _lODDistance.BeginGet() ) LODDistance = _lODDistance.Get( this ); return _lODDistance.value; }
			set { if( _lODDistance.BeginSet( this, ref value ) ) { try { LODDistanceChanged?.Invoke( this ); } finally { _lODDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODDistance"/> property value changes.</summary>
		public event Action<Import3D> LODDistanceChanged;
		ReferenceField<double> _lODDistance = 0.0;//15.0;

		/// <summary>
		/// The distance multiplier when determining the level of detail.
		/// </summary>
		[DefaultValue( 1.0 )]
		[DisplayName( "LOD Scale" )]
		[Category( "Geometry" )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> LODScale
		{
			get { if( _lODScale.BeginGet() ) LODScale = _lODScale.Get( this ); return _lODScale.value; }
			set { if( _lODScale.BeginSet( this, ref value ) ) { try { LODScaleChanged?.Invoke( this ); } finally { _lODScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODScale"/> property value changes.</summary>
		[DisplayName( "LOD Scale Changed" )]
		public event Action<Import3D> LODScaleChanged;
		ReferenceField<double> _lODScale = 1.0;

		/// <summary>
		/// The distance multiplier when determining the level of detail for shadows. Set 100 or more to always use the best LOD for shadows.
		/// </summary>
		[DefaultValue( 1.0 )]
		[DisplayName( "LOD Scale Shadows" )]
		[Category( "Geometry" )]
		[Range( 0, 6, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> LODScaleShadows
		{
			get { if( _lODScaleShadows.BeginGet() ) LODScaleShadows = _lODScaleShadows.Get( this ); return _lODScaleShadows.value; }
			set { if( _lODScaleShadows.BeginSet( this, ref value ) ) { try { LODScaleShadowsChanged?.Invoke( this ); } finally { _lODScaleShadows.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODScaleShadows"/> property value changes.</summary>
		[DisplayName( "LOD Scale Shadows Changed" )]
		public event Action<Import3D> LODScaleShadowsChanged;
		ReferenceField<double> _lODScaleShadows = 1.0;

		/// <summary>
		/// Whether to generate a voxel grid for a last LOD. In auto mode voxel LOD is disabled when imported mesh has a skeleton.
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[DisplayName( "LOD Voxels" )]
		[Category( "Geometry" )]
		public Reference<AutoTrueFalse> LODVoxels
		{
			get { if( _lODVoxels.BeginGet() ) LODVoxels = _lODVoxels.Get( this ); return _lODVoxels.value; }
			set { if( _lODVoxels.BeginSet( this, ref value ) ) { try { LODVoxelsChanged?.Invoke( this ); } finally { _lODVoxels.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODVoxels"/> property value changes.</summary>
		public event Action<Import3D> LODVoxelsChanged;
		ReferenceField<AutoTrueFalse> _lODVoxels = AutoTrueFalse.Auto;

		///// <summary>
		///// Whether to generate a voxel grid for a last LOD.
		///// </summary>
		//[DefaultValue( true )]
		//[DisplayName( "LOD Voxels" )]
		//[Category( "Geometry" )]
		//public Reference<bool> LODVoxels
		//{
		//	get { if( _lODVoxels.BeginGet() ) LODVoxels = _lODVoxels.Get( this ); return _lODVoxels.value; }
		//	set { if( _lODVoxels.BeginSet( this, ref value ) ) { try { LODVoxelsChanged?.Invoke( this ); } finally { _lODVoxels.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="LODVoxels"/> property value changes.</summary>
		//public event Action<Import3D> LODVoxelsChanged;
		//ReferenceField<bool> _lODVoxels = true;

		/// <summary>
		/// The size of a voxel grid of a last LOD.
		/// </summary>
		[DefaultValue( VoxelGridSizeEnum._64 )]
		[DisplayName( "LOD Voxel Grid" )]
		[Category( "Geometry" )]
		public Reference<VoxelGridSizeEnum> LODVoxelGrid
		{
			get { if( _lODVoxelGrid.BeginGet() ) LODVoxelGrid = _lODVoxelGrid.Get( this ); return _lODVoxelGrid.value; }
			set { if( _lODVoxelGrid.BeginSet( this, ref value ) ) { try { LODVoxelGridChanged?.Invoke( this ); } finally { _lODVoxelGrid.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODVoxelGrid"/> property value changes.</summary>
		public event Action<Import3D> LODVoxelGridChanged;
		ReferenceField<VoxelGridSizeEnum> _lODVoxelGrid = VoxelGridSizeEnum._64;

		/// <summary>
		/// The factor to changing the visibility of thin objects. It is also useful when your model has holes in its shape, the algorithm thinks your model is empty inside.
		/// </summary>
		[DefaultValue( 1.0 )] //0.5
		[Range( 0, 1 )]
		[DisplayName( "LOD Voxel Thin Factor" )]
		[Category( "Geometry" )]
		public Reference<double> LODVoxelThinFactor
		{
			get { if( _lODVoxelThinFactor.BeginGet() ) LODVoxelThinFactor = _lODVoxelThinFactor.Get( this ); return _lODVoxelThinFactor.value; }
			set { if( _lODVoxelThinFactor.BeginSet( this, ref value ) ) { try { LODVoxelThinFactorChanged?.Invoke( this ); } finally { _lODVoxelThinFactor.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODVoxelThinFactor"/> property value changes.</summary>
		public event Action<Import3D> LODVoxelThinFactorChanged;
		ReferenceField<double> _lODVoxelThinFactor = 1.0; //0.5;

		/// <summary>
		/// Whether to apply the material opacity during the voxel calculation. The baked opacity works faster. Not baked mode supports 4 transparency steps, instead of baked which supports 1 transparency step.
		/// </summary>
		[DefaultValue( true )]
		[DisplayName( "LOD Voxel Bake Opacity" )]
		[Category( "Geometry" )]
		public Reference<bool> LODVoxelBakeOpacity
		{
			get { if( _lODVoxelBakeOpacity.BeginGet() ) LODVoxelBakeOpacity = _lODVoxelBakeOpacity.Get( this ); return _lODVoxelBakeOpacity.value; }
			set { if( _lODVoxelBakeOpacity.BeginSet( this, ref value ) ) { try { LODVoxelBakeOpacityChanged?.Invoke( this ); } finally { _lODVoxelBakeOpacity.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODVoxelBakeOpacity"/> property value changes.</summary>
		public event Action<Import3D> LODVoxelBakeOpacityChanged;
		ReferenceField<bool> _lODVoxelBakeOpacity = true;

		/// <summary>
		/// Whether to modify materials of the voxels to work with maximal performance (deferred rendering support).
		/// </summary>
		[DisplayName( "LOD Voxel Optimize Materials" )]
		[Category( "Geometry" )]
		[DefaultValue( true )]
		public Reference<bool> LODVoxelOptimizeMaterials
		{
			get { if( _lODVoxelOptimizeMaterials.BeginGet() ) LODVoxelOptimizeMaterials = _lODVoxelOptimizeMaterials.Get( this ); return _lODVoxelOptimizeMaterials.value; }
			set { if( _lODVoxelOptimizeMaterials.BeginSet( this, ref value ) ) { try { LODVoxelOptimizeMaterialsChanged?.Invoke( this ); } finally { _lODVoxelOptimizeMaterials.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODVoxelOptimizeMaterials"/> property value changes.</summary>
		public event Action<Import3D> LODVoxelOptimizeMaterialsChanged;
		ReferenceField<bool> _lODVoxelOptimizeMaterials = true;

		/// <summary>
		/// The maximal distance to fill holes, which happens when ray matching can't find the result because reach max steps limitations.
		/// </summary>
		[DisplayName( "LOD Voxel Fill Holes Distance" )]
		[Category( "Geometry" )]
		[DefaultValue( 1.1 )]
		[Range( 0, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential, 5 )]
		public Reference<double> LODVoxelFillHolesDistance
		{
			get { if( _lODVoxelFillHolesDistance.BeginGet() ) LODVoxelFillHolesDistance = _lODVoxelFillHolesDistance.Get( this ); return _lODVoxelFillHolesDistance.value; }
			set { if( _lODVoxelFillHolesDistance.BeginSet( this, ref value ) ) { try { LODVoxelFillHolesDistanceChanged?.Invoke( this ); } finally { _lODVoxelFillHolesDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODVoxelFillHolesDistance"/> property value changes.</summary>
		public event Action<Import3D> LODVoxelFillHolesDistanceChanged;
		ReferenceField<double> _lODVoxelFillHolesDistance = 1.1;

		///// <summary>
		///// The format of vertices after clusterization. Basic format contains normal, tangent and texCoord0. Full format also contains texCoord1, texCoord2 and color.
		///// </summary>
		//[DefaultValue( VertexFormatEnum.Auto )]
		//[Category( "Geometry Advanced" )]
		//public Reference<VertexFormatEnum> VertexFormat
		//{
		//	get { if( _vertexFormat.BeginGet() ) VertexFormat = _vertexFormat.Get( this ); return _vertexFormat.value; }
		//	set { if( _vertexFormat.BeginSet( this, ref value ) ) { try { VertexFormatChanged?.Invoke( this ); } finally { _vertexFormat.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="VertexFormat"/> property value changes.</summary>
		//public event Action<Import3D> VertexFormatChanged;
		//ReferenceField<VertexFormatEnum> _vertexFormat = VertexFormatEnum.Auto;

		///// <summary>
		///// Whether to compress texture coordinates from Float32 to Half16.
		///// </summary>
		//[DefaultValue( true )]
		//[Category( "Geometry" )]
		//public Reference<bool> CompressTextureCoordinates
		//{
		//	get { if( _compressTextureCoordinates.BeginGet() ) CompressTextureCoordinates = _compressTextureCoordinates.Get( this ); return _compressTextureCoordinates.value; }
		//	set { if( _compressTextureCoordinates.BeginSet( this, ref value ) ) { try { CompressTextureCoordinatesChanged?.Invoke( this ); } finally { _compressTextureCoordinates.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="CompressTextureCoordinates"/> property value changes.</summary>
		//public event Action<Import3D> CompressTextureCoordinatesChanged;
		//ReferenceField<bool> _compressTextureCoordinates = true;

		///// <summary>
		///// Whether to calculate the UV unwrapping channel.
		///// </summary>
		//[DefaultValue( false )]включить по дефолту?
		//[Category( "Material" )]
		//[DisplayName( "UV Unwrap" )]
		//public Reference<bool> UVUnwrap
		//{
		//	get { if( _uVUnwrap.BeginGet() ) UVUnwrap = _uVUnwrap.Get( this ); return _uVUnwrap.value; }
		//	set { if( _uVUnwrap.BeginSet( this, ref value ) ) { try { UVUnwrapChanged?.Invoke( this ); } finally { _uVUnwrap.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="UVUnwrap"/> property value changes.</summary>
		//public event Action<Import3D> UVUnwrapChanged;
		//ReferenceField<bool> _uVUnwrap = false;

		public enum TransparentMaterialBlendingEnum
		{
			Opaque,
			Masked,
			MaskedDithering,
			Transparent,
		}

		/// <summary>
		/// The blend mode of transparent materials.
		/// </summary>
		[DefaultValue( TransparentMaterialBlendingEnum.Transparent )]//MaskedDithering )]
		[Category( "Material" )]
		public Reference<TransparentMaterialBlendingEnum> TransparentMaterialBlending
		{
			get { if( _transparentMaterialFormat.BeginGet() ) TransparentMaterialBlending = _transparentMaterialFormat.Get( this ); return _transparentMaterialFormat.value; }
			set { if( _transparentMaterialFormat.BeginSet( this, ref value ) ) { try { TransparentMaterialsBlendingChanged?.Invoke( this ); } finally { _transparentMaterialFormat.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TransparentMaterialBlending"/> property value changes.</summary>
		public event Action<Import3D> TransparentMaterialsBlendingChanged;
		ReferenceField<TransparentMaterialBlendingEnum> _transparentMaterialFormat = TransparentMaterialBlendingEnum.Transparent;//MaskedDithering;

		/// <summary>
		/// Whether to flip UV coordinates by vertical axis.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Material" )]
		public Reference<bool> FlipUVs
		{
			get { if( _flipUVs.BeginGet() ) FlipUVs = _flipUVs.Get( this ); return _flipUVs.value; }
			set { if( _flipUVs.BeginSet( this, ref value ) ) { try { FlipUVsChanged?.Invoke( this ); } finally { _flipUVs.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="FlipUVs"/> property value changes.</summary>
		public event Action<Import3D> FlipUVsChanged;
		ReferenceField<bool> _flipUVs;

		/// <summary>
		/// Whether to delete unused materials.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Material" )]
		public Reference<bool> DeleteUnusedMaterials
		{
			get { if( _deleteUnusedMaterials.BeginGet() ) DeleteUnusedMaterials = _deleteUnusedMaterials.Get( this ); return _deleteUnusedMaterials.value; }
			set { if( _deleteUnusedMaterials.BeginSet( this, ref value ) ) { try { DeleteUnusedMaterialsChanged?.Invoke( this ); } finally { _deleteUnusedMaterials.EndSet(); } } }
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
			set { if( _materialDisplacement.BeginSet( this, ref value ) ) { try { MaterialDisplacementChanged?.Invoke( this ); } finally { _materialDisplacement.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MaterialDisplacement"/> property value changes.</summary>
		public event Action<Import3D> MaterialDisplacementChanged;
		ReferenceField<bool> _materialDisplacement = false;

		/// <summary>
		/// Whether to optimize the mesh without losing quality. The optimization includes the merging of almost identical vertices, optimizing for vertex cache, for overdraw and for vertex fetch.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Advanced" )]//[Category( "Geometry Advanced" )]
		public Reference<bool> Optimize
		{
			get { if( _optimize.BeginGet() ) Optimize = _optimize.Get( this ); return _optimize.value; }
			set { if( _optimize.BeginSet( this, ref value ) ) { try { OptimizeChanged?.Invoke( this ); } finally { _optimize.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Optimize"/> property value changes.</summary>
		public event Action<Import3D> OptimizeChanged;
		ReferenceField<bool> _optimize = true;

		/// <summary>
		/// The threshold value when optimizes the mesh without losing quality. The parameter affects merging of almost identical vertices.
		/// </summary>
		[DefaultValue( 0.001 )]
		[Category( "Advanced" )]//[Category( "Geometry Advanced" )]
		[Range( 0.0, 0.01, RangeAttribute.ConvenientDistributionEnum.Exponential, 5 )]
		public Reference<double> OptimizeThreshold
		{
			get { if( _optimizeThreshold.BeginGet() ) OptimizeThreshold = _optimizeThreshold.Get( this ); return _optimizeThreshold.value; }
			set { if( _optimizeThreshold.BeginSet( this, ref value ) ) { try { OptimizeThresholdChanged?.Invoke( this ); } finally { _optimizeThreshold.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="OptimizeThreshold"/> property value changes.</summary>
		public event Action<Import3D> OptimizeThresholdChanged;
		ReferenceField<double> _optimizeThreshold = 0.001;

		//!!!!color, weights can be Byte8
		/// <summary>
		/// Whether to compress normals, tangents, colors, texture coordinates and blend weights from Float32 to Half16.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Advanced" )]//[Category( "Geometry Advanced" )]
		public Reference<bool> Compress
		{
			get { if( _compress.BeginGet() ) Compress = _compress.Get( this ); return _compress.value; }
			set { if( _compress.BeginSet( this, ref value ) ) { try { CompressChanged?.Invoke( this ); } finally { _compress.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Compress"/> property value changes.</summary>
		public event Action<Import3D> CompressChanged;
		ReferenceField<bool> _compress = true;

		///// <summary>
		///// Converts PNG to JPG when the texture has no alpha channel.
		///// </summary>
		//[DefaultValue( true )]
		//[Category( "Advanced" )]
		//public Reference<bool> OptimizeTextures
		//{
		//	get { if( _optimizeTextures.BeginGet() ) OptimizeTextures = _optimizeTextures.Get( this ); return _optimizeTextures.value; }
		//	set { if( _optimizeTextures.BeginSet( this, ref value ) ) { try { OptimizeTexturesChanged?.Invoke( this ); } finally { _optimizeTextures.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="OptimizeTextures"/> property value changes.</summary>
		//public event Action<Import3D> OptimizeTexturesChanged;
		//ReferenceField<bool> _optimizeTextures = true;


		///// <summary>
		///// Whether to reset rotation and scale of bones and apply to key frames.
		///// </summary>
		//[DefaultValue( true )]
		//[Category( "Advanced" )]
		//public Reference<bool> NormalizeSkeleton
		//{
		//	get { if( _normalizeSkeleton.BeginGet() ) NormalizeSkeleton = _normalizeSkeleton.Get( this ); return _normalizeSkeleton.value; }
		//	set { if( _normalizeSkeleton.BeginSet( this, ref value ) ) { try { NormalizeSkeletonChanged?.Invoke( this ); } finally { _normalizeSkeleton.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="NormalizeSkeleton"/> property value changes.</summary>
		//public event Action<Import3D> NormalizeSkeletonChanged;
		//ReferenceField<bool> _normalizeSkeleton = true;

		[Serialize]
		[Browsable( false )]
		public Transform EditorCameraTransform;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( Name ):
				case nameof( Enabled ):
				case nameof( ScreenLabel ):
				case nameof( NetworkMode ):
					skip = true;
					break;

				case nameof( OptimizeThreshold ):
					if( !Optimize )
						skip = true;
					break;

				//case nameof( LODMethod ):
				case nameof( LODReduction ):
					if( !LODs || LODLevels.Value < 2 )
						skip = true;
					if( LODLevels.Value == 2 && LODVoxels.Value != AutoTrueFalse.False )
						skip = true;
					break;

				case nameof( LODLevels ):
				case nameof( LODDistance ):
				case nameof( LODScale ):
				case nameof( LODScaleShadows ):
				case nameof( LODVoxels ):
					if( !LODs )
						skip = true;
					break;

				case nameof( LODVoxelGrid ):
				//case nameof( LODVoxelFormat ):
				case nameof( LODVoxelThinFactor ):
				case nameof( LODVoxelBakeOpacity ):
				case nameof( LODVoxelOptimizeMaterials ):
				case nameof( LODVoxelFillHolesDistance ):
					if( !LODs || LODVoxels.Value == AutoTrueFalse.False )
						skip = true;
					break;

				case nameof( SimplifyQuality ):
					if( Simplify.Value == SimplifyMethodEnum.None )
						skip = true;
					break;

					//case nameof( VirtualizeProxyFactor ):
					//	if( !Virtualize )
					//		skip = true;
					//	break;

					//case nameof( ClusterCellSize ):
					//	//case nameof( ClusterFormat ):
					//	if( !Clusters )
					//		skip = true;
					//	break;
				}
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			//autoupdate is disabled by default, big files take a lot of time
			if( ProjectSettings.Get.General.AutoImport3DModels )
				DoAutoUpdate();

			////autoupdate is disabled because big files take a lot of time
			////if( !insideDoUpdate && EnabledInHierarchy && IsNeedUpdate() )
			////{
			////	if( !DoUpdate( null, out string error ) )
			////	{
			////		var virtualFileName = ParentRoot.HierarchyController?.CreatedByResource?.Owner.Name;
			////		if( string.IsNullOrEmpty( virtualFileName ) )
			////			virtualFileName = "NO FILE NAME";
			////		var error2 = $"Unable to load or import resource \"{virtualFileName}\".\r\n\r\n" + error;
			////		Log.Error( error2 );
			////		return;
			////	}

			////	//save to file
			////	if( HierarchyController != null && HierarchyController.CreatedByResource != null &&
			////		HierarchyController.CreatedByResource.InstanceType == Resource.InstanceType.Resource &&
			////		HierarchyController.CreatedByResource.Owner.LoadFromFile )
			////	{
			////		var resource = HierarchyController.CreatedByResource.Owner;
			////		string virtualPath = resource.Name + resource.GetSaveAddFileExtension();
			////		string realPath = VirtualPathUtility.GetRealPathByVirtual( virtualPath );

			////		if( !File.Exists( realPath ) && Components.Count != 0 )
			////		{
			////			if( !ComponentUtility.SaveComponentToFile( this, realPath, null, out error ) )
			////			{
			////				Log.Warning( error );
			////				return;
			////			}
			////		}
			////	}
			////}
		}

		public bool IsNeedUpdate()
		{
			if( EngineApp.IsEditor )
			{
				if( Parent == null )
				{
					if( Components.Count == 0 )
						return true;

					//!!!!хотя проверять тоже долго ведь. варианты: в дейплойменте сорцы удалить (сделать пустые файлы)
				}
			}
			return false;
		}

		public void DoAutoUpdate()
		{
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
			//[Description( "" )]
			public bool UpdateMaterialsOnMeshes { get; set; } = true;

			//[DefaultValue( true )]
			//[Category( "Options" )]
			//public bool UpdateMeshes { get; set; } = true;

			//[DefaultValue( true )]
			//[Category( "Options" )]
			//[DisplayName( "Update Mesh LODs" )]
			//public bool UpdateMeshLODs { get; set; } = true;

			//[DefaultValue( true )]
			//[Category( "Options" )]
			//public bool UpdateObjectsInSpace { get; set; } = true;

			[DefaultValue( false )]
			[Category( "Options" )]
			public bool ResetCollision { get; set; } = false;

			[DefaultValue( false )]
			[Category( "Options" )]
			public bool ResetEditorSettings { get; set; } = false;
		}

		//public delegate void UpdatePostProcessDelegate( Import3D import, ImportGeneral.Settings settings );
		//public static event UpdatePostProcessDelegate UpdatePostProcess;

		public bool DoUpdate( ReimportSettings reimportSettings, out string error )
		{
			ScreenNotifications.IStickyNotificationItem notification = EngineApp.IsEditor ? ScreenNotifications.ShowSticky( "Importing..." ) : null;

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
					settings.updateMaterialsOnMeshes = reimportSettings.UpdateMaterialsOnMeshes;
					//settings.updateMeshes = reimportSettings.UpdateMeshes;
					//settings.updateMeshLODs = reimportSettings.UpdateMeshLODs;
					//settings.updateObjectsInSpace = reimportSettings.UpdateObjectsInSpace;
					settings.resetCollision = reimportSettings.ResetCollision;
					settings.resetEditorSettings = reimportSettings.ResetEditorSettings;

					//save binding of materials settings
					if( !settings.updateMaterialsOnMeshes )
					{
						var c = GetComponent( "Mesh" );
						if( c != null )
						{
							foreach( var geometry in c.GetComponents<MeshGeometry>( checkChildren: true ) )
							{
								////!!!!
								//if( !settings.updateMeshLODs && geometry.FindParent<MeshLevelOfDetail>() != null )
								//	continue;

								var key = geometry.GetPathFromRoot();
								if( !string.IsNullOrEmpty( key ) )
								{
									if( geometry.Material.ReferenceSpecified )
										settings.meshGeometryMaterialsToRestore[ key ] = geometry.Material.GetByReference;

									var multiMaterial = geometry.GetComponent<MultiMaterial>( "Material" );
									if( multiMaterial != null )
									{
										var key2 = multiMaterial.GetPathFromRoot();
										settings.meshGeometryMultiMaterialsToRestore[ key2 ] = (MultiMaterial)multiMaterial.Clone();
									}
								}
							}
						}
						c = GetComponent( "Meshes" );
						if( c != null )
						{
							foreach( var geometry in c.GetComponents<MeshGeometry>( checkChildren: true ) )
							{
								////!!!!
								//if( !settings.updateMeshLODs && geometry.FindParent<MeshLevelOfDetail>() != null )
								//	continue;

								var key = geometry.GetPathFromRoot();
								if( !string.IsNullOrEmpty( key ) )
								{
									if( geometry.Material.ReferenceSpecified )
										settings.meshGeometryMaterialsToRestore[ key ] = geometry.Material.GetByReference;

									var multiMaterial = geometry.GetComponent<MultiMaterial>( "Material" );
									if( multiMaterial != null )
									{
										var key2 = multiMaterial.GetPathFromRoot();
										settings.meshGeometryMultiMaterialsToRestore[ key2 ] = (MultiMaterial)multiMaterial.Clone();
									}
								}
							}
						}
					}

					//save collision of meshes and remove
					//if( !settings.resetCollision )
					//{
					foreach( var mesh in GetMeshes( false ) )
					{
						var collision = mesh.GetComponent<RigidBody>( "Collision Definition" );
						if( collision != null )
						{
							if( !settings.resetCollision )
							{
								var key = mesh.GetPathFromRoot();
								if( !string.IsNullOrEmpty( key ) )
								{
									//make a clone
									settings.collisionToRestore[ key ] = (RigidBody)collision.Clone();
								}
							}

							collision.RemoveFromParent( false );
						}
					}
					//}

					//if( settings.updateMeshLODs )
					//{
					//	var c = GetComponent( "Mesh" );
					//	if( c != null )
					//	{
					//		foreach( var lod in c.GetComponents<MeshLevelOfDetail>( checkChildren: true ) )
					//			lod.RemoveFromParent( false );
					//	}
					//	c = GetComponent( "Meshes" );
					//	if( c != null )
					//	{
					//		foreach( var lod in c.GetComponents<MeshLevelOfDetail>( checkChildren: true ) )
					//			lod.RemoveFromParent( false );
					//	}
					//}

					//save editor settings
					if( !settings.resetEditorSettings )
					{
						foreach( var mesh in GetMeshes( false ) )
						{
							var key = mesh.GetPathFromRoot();
							if( !string.IsNullOrEmpty( key ) )
							{
								var item = new ImportGeneral.Settings.MeshEditorSettings();

								item.EditorDisplayPivot = mesh.EditorDisplayPivot;
								item.EditorDisplayBounds = mesh.EditorDisplayBounds;
								item.EditorDisplayTriangles = mesh.EditorDisplayTriangles;
								item.EditorDisplayVertices = mesh.EditorDisplayVertices;
								item.EditorDisplayNormals = mesh.EditorDisplayNormals;
								item.EditorDisplayTangents = mesh.EditorDisplayTangents;
								item.EditorDisplayBinormals = mesh.EditorDisplayBinormals;
								item.EditorDisplayVertexColor = mesh.EditorDisplayVertexColor;
								item.EditorDisplayUV = mesh.EditorDisplayUV;
								item.EditorDisplayProxyMesh = mesh.EditorDisplayProxyMesh;
								item.EditorDisplayLOD = mesh.EditorDisplayLOD;
								item.EditorDisplayCollision = mesh.EditorDisplayCollision;
								item.EditorDisplaySkeleton = mesh.EditorDisplaySkeleton;
								item.EditorPlayAnimation = mesh.EditorPlayAnimation;
								item.EditorCameraTransform = mesh.EditorCameraTransform;

								settings.meshEditorSeetingsToRestore[ key ] = item;
							}
						}
					}

					////remove old objects
					//if( settings.updateObjectsInSpace )
					//{
					//	var c = GetComponent( "Object In Space" );
					//	if( c != null )
					//		RemoveComponent( c, false );
					//	c = GetComponent( "Objects In Space" );
					//	if( c != null )
					//		RemoveComponent( c, false );
					//}
					//if( settings.updateMeshes )
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

						////settings.loadAnimations = true;
						//settings.frameStep = 0.25;

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

					//if( settings.updateMeshes )
					//{
					if( CenterBySize )
						DoCenterBySize();
					//}

					//if( UVUnwrap )
					//{
					//	if( !CalculateUVUnwrap( out error ) )
					//		return false;
					//}

					//simplify
					//if( settings.updateMeshes )
					//{

					if( Simplify.Value != SimplifyMethodEnum.None )//if( Simplify.Value < 1 )
					{
						if( !GenerateLODsOrReductLOD0( true, out error ) )
							return false;
					}

					//}

					//merge equal vertices, remove extra triangles, 
					//if( settings.updateMeshes )
					//{
					if( Optimize && OptimizeThreshold.Value >= 0 )
						OptimizeByThreshold( false );
					//}

					//generate LODs
					//if( settings.updateMeshLODs )
					//{
					if( LODs )
					{
						if( !GenerateLODsOrReductLOD0( false, out error ) )
							return false;
					}
					//}

					////generate LODs and optimize
					//if( LODs || Simplify.Value < 1 )
					//{
					//	if( !GenerateLODsAndReductLOD0( out error ) )
					//		return false;
					//}

					//optimize for vertex cache
					//if( settings.updateMeshLODs )
					//{
					if( Optimize && OptimizeThreshold.Value >= 0 )
						OptimizeByThreshold( true );
					//}

					//compress vertices
					if( Compress )//&& !Virtualize )
					{
						//if( settings.updateMeshes )
						CompressVertices( false );
						//if( settings.updateMeshLODs )
						CompressVertices( true );
					}

					//optimize
					if( Optimize )//&& !Virtualize )
					{
						//if( settings.updateMeshes )
						OptimizeCaches( false );
						//if( settings.updateMeshLODs )
						OptimizeCaches( true );
					}
					//if( Optimize )//&& !Virtualize )
					//	OptimizeCaches();

					//merge to multimaterial
					if( MergeGeometries.Value == MergeGeometriesEnum.MultiMaterial )
					{
						//if( settings.updateMeshes )
						{
							var meshes = GetMeshes( false );
							foreach( var mesh in meshes )
								MergeToMultiMaterial( mesh );
						}
						//if( settings.updateMeshLODs )
						{
							var meshes = GetMeshes( true );
							foreach( var mesh in meshes )
								MergeToMultiMaterial( mesh );
						}
					}
					//if( MergeGeometries.Value == MergeGeometriesEnum.MultiMaterial )
					//{
					//	var meshes = GetMeshes( false );
					//	meshes.AddRange( GetMeshes( true ) );

					//	foreach( var mesh in meshes )
					//		MergeToMultiMaterial( mesh );
					//}

					////calculate virtualized data
					//if( Virtualize )
					//{
					//	var meshes = GetMeshes( false );
					//	meshes.AddRange( GetMeshes( true ) );

					//	foreach( var mesh in meshes )
					//		CalculateVirtualizedMesh( mesh );
					//}

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
							{
								usedMaterials.AddWithCheckAlreadyContained( material );

								var multiMaterial = material as MultiMaterial;
								if( multiMaterial != null )
								{
									foreach( var m in multiMaterial.Materials )
									{
										if( m.Value != null )
											usedMaterials.AddWithCheckAlreadyContained( m.Value );
									}
								}
							}
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
					if( !settings.updateMaterialsOnMeshes )
					{
						foreach( var item in settings.meshGeometryMaterialsToRestore )
						{
							var key = item.Key;
							var value = item.Value;

							var c = GetComponentByPath( key ) as MeshGeometry;
							if( c != null )
								c.Material = new Reference<Material>( null, value );
						}

						foreach( var item in settings.meshGeometryMultiMaterialsToRestore )
						{
							var key = item.Key;
							var multiMaterial = item.Value;

							var sourceMultiMaterial = GetComponentByPath( key ) as MultiMaterial;
							if( sourceMultiMaterial != null )
							{
								var parent = sourceMultiMaterial.Parent;
								sourceMultiMaterial.Dispose();
								parent.AddComponent( multiMaterial );
							}
						}
					}

					//restore collision of meshes
					if( !settings.resetCollision )
					{
						foreach( var item in settings.collisionToRestore )
						{
							var key = item.Key;
							var value = item.Value;

							var mesh = GetComponentByPath( key ) as Mesh;
							if( mesh != null )
								mesh.AddComponent( value );
						}
					}

					//save editor settings
					if( !settings.resetEditorSettings )
					{
						foreach( var item in settings.meshEditorSeetingsToRestore )
						{
							var key = item.Key;
							var value = item.Value;

							var mesh = GetComponentByPath( key ) as Mesh;
							if( mesh != null )
							{
								mesh.EditorDisplayPivot = value.EditorDisplayPivot;
								mesh.EditorDisplayBounds = value.EditorDisplayBounds;
								mesh.EditorDisplayTriangles = value.EditorDisplayTriangles;
								mesh.EditorDisplayVertices = value.EditorDisplayVertices;
								mesh.EditorDisplayNormals = value.EditorDisplayNormals;
								mesh.EditorDisplayTangents = value.EditorDisplayTangents;
								mesh.EditorDisplayBinormals = value.EditorDisplayBinormals;
								mesh.EditorDisplayVertexColor = value.EditorDisplayVertexColor;
								mesh.EditorDisplayUV = value.EditorDisplayUV;
								mesh.EditorDisplayProxyMesh = value.EditorDisplayProxyMesh;
								mesh.EditorDisplayLOD = value.EditorDisplayLOD;
								mesh.EditorDisplayCollision = value.EditorDisplayCollision;
								mesh.EditorDisplaySkeleton = value.EditorDisplaySkeleton;
								mesh.EditorPlayAnimation = value.EditorPlayAnimation;
								mesh.EditorCameraTransform = value.EditorCameraTransform;
							}
						}
					}

					//reset editor settings
					if( settings.resetEditorSettings )
						EditorCameraTransform = null;

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

					EditorDocumentConfiguration = EditorAPI.CreateEditorDocumentXmlConfiguration( toSelect, selectObject );

					//update windows
					if( EngineApp.IsEditor )
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
							if( windows.Length != 0 )
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

			//MultiMaterials
			foreach( var multiMaterial in import3D.GetComponents<MultiMaterial>( false, true ) )
			{
				for( int n = 0; n < multiMaterial.Materials.Count; n++ )
				{
					var v = multiMaterial.Materials[ n ];
					if( Fix( v, out var newReference ) )
						multiMaterial.Materials[ n ] = ReferenceUtility.MakeReference<Material>( null, newReference );
				}
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
									var bounds = sourceMesh.Result.SpaceBounds.BoundingBox;
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
					var mesh = ProjectSettings.Get.Preview.MaterialPreviewMesh.Value;
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

		bool GenerateLODsOrReductLOD0( bool simplifyLOD0, out string error )
		{
			error = "";

#if !DEPLOY
			var sourceMeshes = GetMeshes( false );

			var initialFactor = LODReduction.Value;//Simplify.Value; //var initialFactor = LODs.Value ? ReductionFactor.Value : 1.0;

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

				var lodLevels = LODs.Value ? LODLevels.Value : 1;

				var voxelLodIndex = -1;
				if( LODs && ( LODVoxels.Value == AutoTrueFalse.True || LODVoxels.Value == AutoTrueFalse.Auto && !sourceMesh.Skeleton.ReferenceOrValueSpecified ) )
				{
					//if( LODs && LODVoxels )// LODBillboardMode.Value != MeshGeometry.BillboardDataModeEnum.None )
					voxelLodIndex = lodLevels - 1;
				}

				var meshes = new List<Mesh>();
				meshes.Add( sourceMesh );

				for( int lodIndex = 0; lodIndex < lodLevels; lodIndex++ )
				{
					if( ( simplifyLOD0 && lodIndex == 0 ) || ( !simplifyLOD0 && lodIndex != 0 ) )//|| initialFactor != 1 )
					{
						var newGeometries = new List<MeshGeometry>();

						foreach( var sourceGeometry in sourceGeometries )
						{
							bool carefully;
							if( lodIndex == 0 )
								carefully = Simplify.Value == SimplifyMethodEnum.Careful;
							else
								carefully = LODReduction.Value == 0;
							//var carefully = lodIndex != 0 && LODReduction.Value == 0;

							var voxelLOD = voxelLodIndex != -1 && lodIndex == lodLevels - 1;

							if( voxelLOD )
							{
								//skip voxel lod from simplification

								//create mesh geometry
								var newGeometry = ComponentUtility.CreateComponent<MeshGeometry>( null, false, true );
								newGeometry.Name = sourceGeometry.Name;
								newGeometry.VertexStructure = sourceGeometry.VertexStructure;
								newGeometry.UnwrappedUV = sourceGeometry.UnwrappedUV;
								newGeometry.Vertices = sourceGeometry.Vertices;
								newGeometry.Indices = sourceGeometry.Indices;
								newGeometry.Material = sourceGeometry.Material;

								newGeometries.Add( newGeometry );
							}
							else
							{
								//simplify

								if( !sourceGeometry.CalculateSimplification( carefully, lodIndex /*LODMethod*/, currentQuality, SimplifyQuality, out var newVertices, out var newVertexStructure, out var newIndices, out error ) )
									return false;

								//create mesh geometry
								var newGeometry = ComponentUtility.CreateComponent<MeshGeometry>( null, false, true );
								newGeometry.Name = sourceGeometry.Name;
								newGeometry.VertexStructure = newVertexStructure;
								newGeometry.UnwrappedUV = sourceGeometry.UnwrappedUV;
								newGeometry.Vertices = newVertices;
								newGeometry.Indices = newIndices;
								newGeometry.Material = sourceGeometry.Material;

								newGeometries.Add( newGeometry );
							}
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

							var lodDistance = LODDistance.Value * lodIndex;
							if( LODDistance.Value == 0 && voxelLodIndex == -1 )
								lodDistance = 15;
							lod.Distance = lodDistance;
							//lod.Distance = LODDistance.Value * (double)lodIndex;

							var lodMesh = lod.CreateComponent<Mesh>();
							lodMesh.Name = "Mesh";
							lod.Mesh = ReferenceUtility.MakeRootReference( lodMesh );

							foreach( var geometry in newGeometries )
								lodMesh.AddComponent( geometry );

							meshes.Add( lodMesh );
						}
					}

					currentQuality *= LODReduction.Value;
				}

				//convert usual mesh to voxel
				if( voxelLodIndex != -1 && voxelLodIndex < meshes.Count )
				{
					var mesh = meshes[ voxelLodIndex ];

					var gridSize = int.Parse( LODVoxelGrid.Value.ToString().Replace( "_", "" ) );

					mesh.ConvertToVoxel( gridSize/*, LODVoxelFormat*/, LODVoxelThinFactor, LODVoxelBakeOpacity, LODVoxelOptimizeMaterials, LODVoxelFillHolesDistance );
				}

				sourceMesh.LODScale = LODScale;
				sourceMesh.LODScaleShadows = LODScaleShadows;
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

								MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( vertices, indices, 0, vertexPositionEpsilon, vertexOtherChannelsEpsilon, true, true, out var newVertices, out var newIndices, out _ );

								geometry.SetVertexData( newVertices, vertexComponents );
								geometry.Indices = newIndices;
							}
						}
					}
				}
			}
		}

		void OptimizeCaches( bool lods )
		{
			var meshes = GetMeshes( lods );
			//meshes.AddRange( GetMeshes( true ) );

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

		void CompressVertices( bool lods )
		{
			var meshes = GetMeshes( lods );

			foreach( var mesh in meshes )
			{
				foreach( var geometry in mesh.GetComponents<MeshGeometry>() )
					geometry.CompressVertices();
			}
		}

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			//old version compatibility
			if( block.AttributeExists( "MergeMeshGeometries" ) && bool.TryParse( block.GetAttribute( "MergeMeshGeometries" ), out var v ) && !v )
				MergeGeometries = MergeGeometriesEnum.False;
			if( block.AttributeExists( "ForceFrontXAxis" ) && bool.TryParse( block.GetAttribute( "ForceFrontXAxis" ), out var v2 ) )
				FixAxes = v2;

			return true;
		}

		class MergeGroup
		{
			public List<MeshGeometry> Geometries = new List<MeshGeometry>();

			public bool CanMerge( MeshGeometry geometry )
			{
				var geometry0 = Geometries[ 0 ];

				if( !VertexElements.Equals( geometry.VertexStructure.Value, geometry0.VertexStructure.Value ) )
					return false;

				return true;
			}
		}

		void MergeToMultiMaterial( Mesh mesh )
		{
			if( mesh.Billboard )
				return;

			var geometries = mesh.GetComponents<MeshGeometry>();
			if( geometries.Length < 2 )
				return;

			foreach( var geometry in geometries )
			{
				if( geometry.VertexStructure.Value == null || geometry.Vertices.Value == null || geometry.Indices.Value == null )
					return;
				if( geometry.VoxelData.Value != null )
					return;
			}

			//save materials of geometries
			var materialByGeometry = new Dictionary<MeshGeometry, Material>();
			foreach( var geometry in geometries )
				materialByGeometry[ geometry ] = geometry.Material.Value;

			//remove old geometries
			foreach( var geometry in geometries.GetReverse() )
				mesh.RemoveComponent( geometry, false );

			//get groups
			var groups = new List<MergeGroup>();
			foreach( var geometry in geometries )
			{
				var group = groups.FirstOrDefault( g => g.CanMerge( geometry ) );
				if( group == null )
				{
					group = new MergeGroup();
					groups.Add( group );
				}

				group.Geometries.Add( geometry );
			}

			var insertIndex = 0;

			foreach( var group in groups )
			{
				if( group.Geometries.Count > 1 )
				{
					//sort group. combined deferred first
					CollectionUtility.MergeSort( group.Geometries, delegate ( MeshGeometry g1, MeshGeometry g2 )
					{
						materialByGeometry.TryGetValue( g1, out var m1 );
						materialByGeometry.TryGetValue( g2, out var m2 );

						var points1 = 0;
						if( m1 != null )
						{
							points1++;
							if( string.IsNullOrEmpty( m1.PerformCheckDeferredShadingSupport() ) )
								points1 += 2;
						}

						var points2 = 0;
						if( m2 != null )
						{
							points2++;
							if( string.IsNullOrEmpty( m2.PerformCheckDeferredShadingSupport() ) )
								points2 += 2;
						}

						if( points1 > points2 )
							return -1;
						if( points1 < points2 )
							return 1;
						return 0;
					} );

					var geometry = mesh.CreateComponent<MeshGeometry>( insertIndex++ );

					var baseName = "Geometry";
					if( mesh.GetComponent( baseName ) != null )
						geometry.Name = mesh.Components.GetUniqueName( baseName, true, 2 );
					else
						geometry.Name = baseName;

					var geometry0 = group.Geometries[ 0 ];
					var vertexStructure0 = geometry0.VertexStructure.Value;

					vertexStructure0.GetInfo( out var oldVertexSize, out _ );

					VertexElement[] vertexStructure;
					{
						var list = new List<VertexElement>( vertexStructure0 );
						list.Add( new VertexElement( 0, oldVertexSize, VertexElementType.Float1, VertexElementSemantic.Color3 ) );
						vertexStructure = list.ToArray();
					}

					vertexStructure.GetInfo( out var vertexSize, out _ );

					var totalVertexCount = 0;
					var totalIndexCount = 0;
					foreach( var g in group.Geometries )
					{
						totalVertexCount += g.Vertices.Value.Length / oldVertexSize;
						totalIndexCount += g.Indices.Value.Length;
					}

					var resultVertices = new byte[ totalVertexCount * vertexSize ];
					var resultIndices = new int[ totalIndexCount ];

					var currentVertex = 0;
					var currentIndex = 0;

					for( int nGeometry = 0; nGeometry < group.Geometries.Count; nGeometry++ )
					{
						var g = group.Geometries[ nGeometry ];
						var vertices = g.Vertices.Value;
						var indices = g.Indices.Value;
						var vertexCount = vertices.Length / oldVertexSize;

						var vertexIndexStart = currentVertex;

						for( int n = 0; n < vertexCount; n++ )
						{
							//write source vertex data
							Buffer.BlockCopy( vertices, n * oldVertexSize, resultVertices, currentVertex * vertexSize, oldVertexSize );

							//write material index
							unsafe
							{
								fixed( byte* pResultVertices = resultVertices )
									*(float*)( pResultVertices + currentVertex * vertexSize + oldVertexSize ) = nGeometry;
							}

							currentVertex++;
						}

						for( int n = 0; n < indices.Length; n++ )
						{
							resultIndices[ currentIndex ] = indices[ n ] + vertexIndexStart;
							currentIndex++;
						}
					}

					geometry.VertexStructure = vertexStructure;
					geometry.Vertices = resultVertices;
					geometry.Indices = resultIndices;

					var material = geometry.CreateComponent<MultiMaterial>();
					material.Name = "Material";
					foreach( var sourceGeometry in group.Geometries )
					{
						var subMaterialReference = sourceGeometry.Material;
						material.Materials.Add( subMaterialReference );
					}
					geometry.Material = ReferenceUtility.MakeThisReference( geometry, material );
				}
				else
					mesh.AddComponent( group.Geometries[ 0 ], insertIndex++ );
			}
		}

		//void CalculateVirtualizedMesh( Mesh mesh )
		//{
		//	if( mesh.Billboard )
		//		return;

		//	var geometries = mesh.GetComponents<MeshGeometry>();

		//	foreach( var geometry in geometries )
		//		geometry.CalculateVirtualizedData( VirtualizeProxyFactor, Compress, Optimize );
		//}

#endif

	}
}
