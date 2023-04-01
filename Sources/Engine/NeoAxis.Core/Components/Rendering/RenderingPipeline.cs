// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// The rendering pipeline of the scene.
	/// </summary>
	public abstract class RenderingPipeline : Component
	{
		public static double GlobalLODScale = 1;

		//

		[Browsable( false )]
		public ColorValue? BackgroundColorOverride;

		///// <summary>
		///// The background clear color.
		///// </summary>
		//[DefaultValue( "0 0 0" )]
		//[ColorValueNoAlpha]
		//[Category( "General" )]
		//public Reference<ColorValue> BackgroundColor
		//{
		//	get { if( _backgroundColor.BeginGet() ) BackgroundColor = _backgroundColor.Get( this ); return _backgroundColor.value; }
		//	set { if( _backgroundColor.BeginSet( ref value ) ) { try { BackgroundColorChanged?.Invoke( this ); } finally { _backgroundColor.EndSet(); } } }
		//}
		//public event Action<RenderingPipeline> BackgroundColorChanged;
		//ReferenceField<ColorValue> _backgroundColor = new ColorValue( 0, 0, 0 );

		//public enum RenderingMethodEnum
		//{
		//	RealTime,
		//	PhysicallyCorrect,
		//}

		///// <summary>
		///// A method for obtaining the resulting image of a scene.
		///// </summary>
		//[DefaultValue( RenderingMethodEnum.RealTime )]
		//[Category( "General" )]
		//public Reference<RenderingMethodEnum> RenderingMethod
		//{
		//	get { if( _renderingMethod.BeginGet() ) RenderingMethod = _renderingMethod.Get( this ); return _renderingMethod.value; }
		//	set { if( _renderingMethod.BeginSet( ref value ) ) { try { RenderingMethodChanged?.Invoke( this ); } finally { _renderingMethod.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="RenderingMethod"/> property value changes.</summary>
		//public event Action<RenderingPipeline> RenderingMethodChanged;
		//ReferenceField<RenderingMethodEnum> _renderingMethod = RenderingMethodEnum.RealTime;

		/// <summary>
		/// Enables the deferred shading. For Auto mode deferred shading is enabled, but not for Intel GPUs. Limited devices (mobile) are not support deferred shading.
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "General" )]
		public Reference<AutoTrueFalse> DeferredShading
		{
			get { if( _deferredShading.BeginGet() ) DeferredShading = _deferredShading.Get( this ); return _deferredShading.value; }
			set { if( _deferredShading.BeginSet( ref value ) ) { try { DeferredShadingChanged?.Invoke( this ); } finally { _deferredShading.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DeferredShading"/> property value changes.</summary>
		public event Action<RenderingPipeline> DeferredShadingChanged;
		ReferenceField<AutoTrueFalse> _deferredShading = AutoTrueFalse.Auto;

		/// <summary>
		/// Enables the high dynamic range rendering. When Auto mode is enabled, HDR is disabled for limited devices (mobile).
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "General" )]
		public Reference<AutoTrueFalse> HighDynamicRange
		{
			get { if( _highDynamicRange.BeginGet() ) HighDynamicRange = _highDynamicRange.Get( this ); return _highDynamicRange.value; }
			set { if( _highDynamicRange.BeginSet( ref value ) ) { try { HighDynamicRangeChanged?.Invoke( this ); } finally { _highDynamicRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="HighDynamicRange"/> property value changes.</summary>
		public event Action<RenderingPipeline> HighDynamicRangeChanged;
		ReferenceField<AutoTrueFalse> _highDynamicRange = AutoTrueFalse.Auto;

		/// <summary>
		/// Enables using additional render targets during rendering the frame.
		/// </summary>
		[DefaultValue( true )]
		[Category( "General" )]
		public Reference<bool> UseRenderTargets
		{
			get { if( _useRenderTargets.BeginGet() ) UseRenderTargets = _useRenderTargets.Get( this ); return _useRenderTargets.value; }
			set { if( _useRenderTargets.BeginSet( ref value ) ) { try { UseRenderTargetsChanged?.Invoke( this ); } finally { _useRenderTargets.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UseRenderTargets"/> property value changes.</summary>
		public event Action<RenderingPipeline> UseRenderTargetsChanged;
		ReferenceField<bool> _useRenderTargets = true;

		/// <summary>
		/// Enables using multi render targets during rendering the frame. It is enabled by default. MRT is not supported on limited devices (mobile).
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "General" )]
		public Reference<AutoTrueFalse> UseMultiRenderTargets
		{
			get { if( _useMultiRenderTargets.BeginGet() ) UseMultiRenderTargets = _useMultiRenderTargets.Get( this ); return _useMultiRenderTargets.value; }
			set { if( _useMultiRenderTargets.BeginSet( ref value ) ) { try { UseMultiRenderTargetsChanged?.Invoke( this ); } finally { _useMultiRenderTargets.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UseMultiRenderTargets"/> property value changes.</summary>
		public event Action<RenderingPipeline> UseMultiRenderTargetsChanged;
		ReferenceField<AutoTrueFalse> _useMultiRenderTargets = AutoTrueFalse.Auto;

		/// <summary>
		/// Enables antialising for simple geometry rendering. When Auto mode is enabled, antialiasing is disabled for limited devices (mobile).
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "General" )]
		public Reference<AutoTrueFalse> SimpleGeometryAntialiasing
		{
			get { if( _simpleGeometryAntialiasing.BeginGet() ) SimpleGeometryAntialiasing = _simpleGeometryAntialiasing.Get( this ); return _simpleGeometryAntialiasing.value; }
			set { if( _simpleGeometryAntialiasing.BeginSet( ref value ) ) { try { SimpleGeometryAntialiasingChanged?.Invoke( this ); } finally { _simpleGeometryAntialiasing.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="SimpleGeometryAntialiasing"/> property value changes.</summary>
		public event Action<RenderingPipeline> SimpleGeometryAntialiasingChanged;
		ReferenceField<AutoTrueFalse> _simpleGeometryAntialiasing = AutoTrueFalse.Auto;

		/////////////////////////////////////////

		/// <summary>
		/// Whether to enable GPU instancing to reduce the number of draw calls.
		/// </summary>
		[Category( "Instancing" )]
		[DefaultValue( true )]
		public Reference<bool> Instancing
		{
			get { if( _instancing.BeginGet() ) Instancing = _instancing.Get( this ); return _instancing.value; }
			set { if( _instancing.BeginSet( ref value ) ) { try { InstancingChanged?.Invoke( this ); } finally { _instancing.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Instancing"/> property value changes.</summary>
		public event Action<RenderingPipeline> InstancingChanged;
		ReferenceField<bool> _instancing = true;

		/// <summary>
		/// The minimum number of objects to enable GPU instancing.
		/// </summary>
		[Category( "Instancing" )]
		[DefaultValue( 4 )]
		[Range( 2, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<int> InstancingMinCount
		{
			get { if( _instancingMinCount.BeginGet() ) InstancingMinCount = _instancingMinCount.Get( this ); return _instancingMinCount.value; }
			set { if( _instancingMinCount.BeginSet( ref value ) ) { try { InstancingMinCountChanged?.Invoke( this ); } finally { _instancingMinCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InstancingMinCount"/> property value changes.</summary>
		public event Action<RenderingPipeline> InstancingMinCountChanged;
		ReferenceField<int> _instancingMinCount = 4;

		/// <summary>
		/// The maximum size of instancing buffer.
		/// </summary>
		[Category( "Instancing" )]
		[DefaultValue( 250 )]
		[Range( 2, 500, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<int> InstancingMaxCount
		{
			get { if( _instancingMaxCount.BeginGet() ) InstancingMaxCount = _instancingMaxCount.Get( this ); return _instancingMaxCount.value; }
			set { if( _instancingMaxCount.BeginSet( ref value ) ) { try { InstancingMaxCountChanged?.Invoke( this ); } finally { _instancingMaxCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InstancingMaxCount"/> property value changes.</summary>
		public event Action<RenderingPipeline> InstancingMaxCountChanged;
		ReferenceField<int> _instancingMaxCount = 250;

		/// <summary>
		/// Whether to enable GPU instancing for transparent objects.
		/// </summary>
		[Category( "Instancing" )]
		[DefaultValue( true )]
		public Reference<bool> InstancingTransparent
		{
			get { if( _instancingTransparent.BeginGet() ) InstancingTransparent = _instancingTransparent.Get( this ); return _instancingTransparent.value; }
			set { if( _instancingTransparent.BeginSet( ref value ) ) { try { InstancingTransparentChanged?.Invoke( this ); } finally { _instancingTransparent.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InstancingTransparent"/> property value changes.</summary>
		public event Action<RenderingPipeline> InstancingTransparentChanged;
		ReferenceField<bool> _instancingTransparent = true;

		/////////////////////////////////////////

		//!!!!
		///// <summary>
		///// Specifies the quality for procedural generation of 3D models and other assets. A value of 0.5 is optimal for real-time graphics. The higher values are more applicable for non real-time rendering.
		///// </summary>
		//[Category( "Procedural Generation" )]
		//[DefaultValue( 0.5 )]
		//[Range( 0, 1 )]
		//public Reference<double> ProceduralGenerationQuality
		//{
		//	get { if( _proceduralGenerationQuality.BeginGet() ) ProceduralGenerationQuality = _proceduralGenerationQuality.Get( this ); return _proceduralGenerationQuality.value; }
		//	set { if( _proceduralGenerationQuality.BeginSet( ref value ) ) { try { ProceduralGenerationQualityChanged?.Invoke( this ); } finally { _proceduralGenerationQuality.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="ProceduralGenerationQuality"/> property value changes.</summary>
		//public event Action<RenderingPipeline> ProceduralGenerationQualityChanged;
		//ReferenceField<double> _proceduralGenerationQuality = 0.5;

		/////////////////////////////////////////

		/// <summary>
		/// The distance multiplier when determining the level of detail.
		/// </summary>
		[Category( "Level Of Detail" )]
		[DefaultValue( 1.0 )]
		[DisplayName( "LOD Scale" )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential, 3 )]
		public Reference<double> LODScale
		{
			get { if( _lODScale.BeginGet() ) LODScale = _lODScale.Get( this ); return _lODScale.value; }
			set { if( _lODScale.BeginSet( ref value ) ) { try { LODScaleChanged?.Invoke( this ); } finally { _lODScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODScale"/> property value changes.</summary>
		[DisplayName( "LOD Scale Changed" )]
		public event Action<RenderingPipeline> LODScaleChanged;
		ReferenceField<double> _lODScale = 1.0;

		/// <summary>
		/// The minimum and maximum levels of detail.
		/// </summary>
		[Category( "Level Of Detail" )]
		[DefaultValue( "0 10" )]
		[DisplayName( "LOD Range" )]
		[Range( 0, 10 )]
		public Reference<RangeI> LODRange
		{
			get { if( _lODRange.BeginGet() ) LODRange = _lODRange.Get( this ); return _lODRange.value; }
			set { if( _lODRange.BeginSet( ref value ) ) { try { LODRangeChanged?.Invoke( this ); } finally { _lODRange.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODRange"/> property value changes.</summary>
		[DisplayName( "LOD Range Changed" )]
		public event Action<RenderingPipeline> LODRangeChanged;
		ReferenceField<RangeI> _lODRange = new RangeI( 0, 10 );

		/// <summary>
		/// The minimum visible size of object in pixels.
		/// </summary>
		[Category( "Visibility Distance" )]
		[DefaultValue( 4.0 )]
		[Range( 1, 16 )]
		public Reference<double> MinimumVisibleSizeOfObjects
		{
			get { if( _minimumVisibleSizeOfObjects.BeginGet() ) MinimumVisibleSizeOfObjects = _minimumVisibleSizeOfObjects.Get( this ); return _minimumVisibleSizeOfObjects.value; }
			set { if( _minimumVisibleSizeOfObjects.BeginSet( ref value ) ) { try { MinimumVisibleSizeOfObjectsChanged?.Invoke( this ); } finally { _minimumVisibleSizeOfObjects.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="MinimumVisibleSizeOfObjects"/> property value changes.</summary>
		public event Action<RenderingPipeline> MinimumVisibleSizeOfObjectsChanged;
		ReferenceField<double> _minimumVisibleSizeOfObjects = 4.0;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a list of material passes for shadow caster.
		/// </summary>
		public class ShadowCasterData
		{
			public GpuMaterialPassGroup[] passByLightType;
			//public GpuMaterialPass[] passByLightType;

			//public void Dispose()
			//{
			//	foreach( var pass in passByLightType )
			//		pass?.Dispose();
			//	passByLightType = null;
			//}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Prepared data for scene rendering.
		/// </summary>
		public class RenderSceneData
		{
			//!!!!can use list container with several arrays inside
			public OpenList<MeshItem> Meshes;// = new OpenList<MeshItem>( 2048 );
			public OpenList<BillboardItem> Billboards;// = new OpenList<BillboardItem>( 512 );
			public OpenList<LightItem> Lights;// = new OpenList<LightItem>( 256 );
			public OpenList<ReflectionProbeItem> ReflectionProbes;// = new OpenList<ReflectionProbeItem>( 128 );
			public OpenList<DecalItem> Decals;// = new OpenList<DecalItem>( 512 );
			public OpenList<CutVolumeItem> CutVolumes = new OpenList<CutVolumeItem>();
			public OpenList<TransparentRenderingAddOffsetWhenSortByDistanceVolumeItem> TransparentRenderingAddOffsetWhenSortByDistanceVolumes = new OpenList<TransparentRenderingAddOffsetWhenSortByDistanceVolumeItem>();
			public List<ActionToDoAfterPrepareListsOfObjectsSortedByDistance> ActionsToDoAfterPrepareListsOfObjectsSortedByDistance = new List<ActionToDoAfterPrepareListsOfObjectsSortedByDistance>();

			//!!!!don't forget to Clear() new fields

			////////////

			public RenderSceneData()
			{
				int multiplier = SystemSettings.LimitedDevice ? 1 : 4;

				Meshes = new OpenList<MeshItem>( 512 * multiplier );
				Billboards = new OpenList<BillboardItem>( 128 * multiplier );
				Lights = new OpenList<LightItem>( 32 * multiplier );
				ReflectionProbes = new OpenList<ReflectionProbeItem>( 32 * multiplier );
				Decals = new OpenList<DecalItem>( 128 * multiplier );
			}

			public void Clear()
			{
				Meshes.Clear();
				Billboards.Clear();
				Lights.Clear();
				ReflectionProbes.Clear();
				Decals.Clear();
				CutVolumes.Clear();
				TransparentRenderingAddOffsetWhenSortByDistanceVolumes.Clear();
				ActionsToDoAfterPrepareListsOfObjectsSortedByDistance.Clear();
			}

			/// <summary>
			/// Prepared mesh item data for scene rendering.
			/// </summary>
			public struct MeshItem
			{
				public object Creator;
				public object AnyData;
				//!!!!sense? take by BoundingSphere.Origin
				public Vector3 BoundingBoxCenter;//public Bounds BoundingBox;
				public Sphere BoundingSphere;

				public bool CastShadows;
				public bool ReceiveDecals;
				public float MotionBlurFactor;
				public Material ReplaceMaterial;
				public Material[] ReplaceMaterialSelectively;
				public ColorValue Color;
				public ColorByte ColorForInstancingData;
				public AnimationDataClass AnimationData;

				public IMeshData MeshData;
				//!!!!new GI
				//!!!!всем выставлять
				public IMeshData MeshDataLOD0;

				public Matrix4F Transform;
				public Vector3F PositionPreviousFrame;

				public bool InstancingEnabled;
				public GpuVertexBuffer InstancingVertexBuffer;
				public Internal.SharpBgfx.InstanceDataBuffer InstancingDataBuffer;
				public int InstancingStart;
				public int InstancingCount;
				public float InstancingMaxLocalBounds;

				public float VisibilityDistance;
				public bool OnlyForShadowGeneration;
				public float LODValue;

				public LayerItem[] Layers;
				public CutVolumeItem[] CutVolumes;
				public List<ObjectSpecialRenderingEffect> SpecialEffects;

				public bool TransparentRenderingAddOffsetWhenSortByDistance;

				//!!!!new
				public uint CullingByCameraDirectionData;

				//bool instancingDataCached;
				//ObjectInstanceData instancingDataCache;

				//!!!!without class?
				/// <summary>
				/// Prepared animation data of mesh for scene rendering.
				/// </summary>
				public class AnimationDataClass
				{
					public int Mode;
					public ImageComponent BonesTexture;
				}

				////////

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public void GetInstancingData( out ObjectInstanceData data )
				{
					Transform.GetTranspose( out data.Transform );
					data.PositionPreviousFrame = PositionPreviousFrame;
					//data.Unused = 0;

					data.Color = ColorForInstancingData;
					//data.Color = GetColorForInstancingData( ref Color );

					//ColorValue c;
					//c.Red = MathEx.Sqrt( Color.Red * 0.1f );
					//c.Green = MathEx.Sqrt( Color.Green * 0.1f );
					//c.Blue = MathEx.Sqrt( Color.Blue * 0.1f );
					//c.Alpha = MathEx.Sqrt( Color.Alpha * 0.1f );
					//data.Color = c.ToColorPacked();
					////data.Color = ( Color * 0.25f ).ToColorPacked();
					////data.Color = Color;

					data.LodValue = LODValue;
					data.VisibilityDistance = VisibilityDistance;
					//!!!!new
					data.ReceiveDecals = ReceiveDecals ? (byte)255 : (byte)0;
					data.MotionBlurFactor = (byte)( MotionBlurFactor * 255 );
					data.Unused1 = 0;
					data.Unused2 = 0;
					data.CullingByCameraDirectionData = CullingByCameraDirectionData;
					//data.ReceiveDecals = ReceiveDecals ? 1.0f : 0.0f;
					//data.MotionBlurFactor = MotionBlurFactor;


					//if( !instancingDataCached )
					//{
					//	Transform.GetTranspose( out instancingDataCache.Transform );
					//	instancingDataCache.PositionPreviousFrame = PositionPreviousFrame;
					//	//data.Unused = 0;

					//	instancingDataCache.Color = ColorForInstancingData;
					//	//data.Color = GetColorForInstancingData( ref Color );

					//	//ColorValue c;
					//	//c.Red = MathEx.Sqrt( Color.Red * 0.1f );
					//	//c.Green = MathEx.Sqrt( Color.Green * 0.1f );
					//	//c.Blue = MathEx.Sqrt( Color.Blue * 0.1f );
					//	//c.Alpha = MathEx.Sqrt( Color.Alpha * 0.1f );
					//	//data.Color = c.ToColorPacked();
					//	////data.Color = ( Color * 0.25f ).ToColorPacked();
					//	////data.Color = Color;

					//	instancingDataCache.LodValue = LODValue;
					//	instancingDataCache.VisibilityDistance = VisibilityDistance;
					//	instancingDataCache.ReceiveDecals = ReceiveDecals ? 1.0f : 0.0f;
					//	instancingDataCache.MotionBlurFactor = MotionBlurFactor;

					//	instancingDataCached = true;
					//}

					//data = instancingDataCache;
				}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public unsafe bool CanUseInstancingForTransparentWith( ref MeshItem meshItem )
				{
					//if( BoundingBoxCenter != meshItem.BoundingBoxCenter )
					//	return false;
					if( BoundingSphere != meshItem.BoundingSphere )
						return false;
					if( CastShadows != meshItem.CastShadows )
						return false;
					if( ReceiveDecals != meshItem.ReceiveDecals )
						return false;
					if( MotionBlurFactor != meshItem.MotionBlurFactor )
						return false;
					if( ReplaceMaterial != meshItem.ReplaceMaterial )
						return false;

					if( ReplaceMaterialSelectively != null || meshItem.ReplaceMaterialSelectively != null )
					{
						if( ReplaceMaterialSelectively != null && meshItem.ReplaceMaterialSelectively != null )
						{
							if( ReplaceMaterialSelectively.Length != meshItem.ReplaceMaterialSelectively.Length )
								return false;

							for( int n = 0; n < ReplaceMaterialSelectively.Length; n++ )
								if( ReplaceMaterialSelectively[ n ] != meshItem.ReplaceMaterialSelectively[ n ] )
									return false;
						}
						else
							return false;
					}

					if( AnimationData != null || meshItem.AnimationData != null )
						return false;
					if( MeshData != meshItem.MeshData )
						return false;

					//!!!!
					if( OnlyForShadowGeneration != meshItem.OnlyForShadowGeneration )
						return false;

					//if( VisibilityDistance != meshItem.VisibilityDistance )
					//	return false;

					//if( LODValue != meshItem.LODValue )
					//	return false;
					//if( LODRange != meshItem.LODRange )
					//	return false;
					//if( LODValue != 0 || meshItem.LODValue != 0 )
					//	return false;

					if( Layers != null || meshItem.Layers != null )
						return false;

					//public GpuVertexBuffer BatchingInstanceBuffer;

					//public object Creator;
					//public object AnyData;

					////public BillboardData[] BillboardArray;

					return true;
				}
			}

			/// <summary>
			/// Prepared rendering operation data of mesh for scene rendering.
			/// </summary>
			public class MeshDataRenderOperation
			{
				public object Creator;
				//!!!!is not used
				public int MeshGeometryIndex;
				public object AnyData;
				//!!!!
				//public MeshGeometry creator;
				//public bool disposeBuffersByCreator = true;
				////public Mesh disposeBuffersByObject;
				////public bool disposeBuffersByMesh = true;

				public VertexElement[] VertexStructure;
				public bool VertexStructureContainsColor;
				public UnwrappedUVEnum UnwrappedUV;

				public GpuVertexBuffer[] VertexBuffers;
				public int VertexStartOffset;
				public int VertexCount;

				public GpuIndexBuffer IndexBuffer;
				public int IndexStartOffset;
				public int IndexCount;

				public RangeI[] MaterialIndexRangesFromVertexData;
				public bool MaterialIndexRangesFromVertexDataCalculated;

				//RenderOperationType OperationType { get; }

				//!!!!pass/material
				public Material Material;

				//!!!!не хранить копию byte[] массива? юзать ArraySegment? где еще так
				public ImageComponent VoxelDataImage;
				public Vector4F[] VoxelDataInfo;
				//!!!!new for GI
				public byte[] SourceVoxelData;
				//public MeshGeometry.VoxelDataModeEnum VoxelDataMode;

				////!!!!не хранить копию byte[] массива? юзать ArraySegment? где еще так
				//public ImageComponent VirtualizedDataImage;
				////!!!!может без данных хранить. тогда не хранить исходно-загруженную. может в ImageComponent тоже
				//public byte[] VirtualizedData;
				//public Vector4F[] VirtualizedDataInfo;

				//can't allocate array before been used because threading
				internal int[] _currentRenderingFrameIndex = new int[ 16 ] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
				//public int _currentRenderingFrameIndex = -1;

				//

				public MeshDataRenderOperation()
				{
				}

				public MeshDataRenderOperation( object creator, int meshGeometryIndex )
				{
					Creator = creator;
					MeshGeometryIndex = meshGeometryIndex;
				}

				public bool ContainsDisposedBuffers()
				{
					if( VertexBuffers != null )
					{
						for( int n = 0; n < VertexBuffers.Length; n++ )
							if( VertexBuffers[ n ] != null && VertexBuffers[ n ].Disposed )
								return true;
					}
					if( IndexBuffer != null && IndexBuffer.Disposed )
						return true;
					return false;
				}

				public void DisposeBuffers()
				{
					if( VertexBuffers != null )
					{
						for( int n = 0; n < VertexBuffers.Length; n++ )
							VertexBuffers[ n ]?.Dispose();
					}
					IndexBuffer?.Dispose();

					VoxelDataImage?.Dispose();
					//VirtualizedDataImage?.Dispose();
				}

				public unsafe RangeI[] GetMaterialIndexRangesFromVertexDataOrFromVirtualizedData()
				{
					if( !MaterialIndexRangesFromVertexDataCalculated )
					{
						if( VertexBuffers.Length != 0 && IndexBuffer?.Indices != null && VertexStructure != null )
						{
							/*if( VirtualizedData != null )
							{
								MaterialIndexRangesFromVertexData = MeshGeometry.GetMaterialIndexRangesFromVirtualizedData( VirtualizedData );
							}
							else */
							if( VertexStructure.GetElementBySemantic( VertexElementSemantic.Color3, out var element ) && element.Type == VertexElementType.Float1 )
							{
								var vertexMaterialIndexes = VertexBuffers[ 0 ].ExtractChannelSingle( element.Offset );
								if( vertexMaterialIndexes != null )
								{
									var indices = IndexBuffer.Indices;
									var v = MeshGeometry.GetMaterialIndexRangesFromVertexMaterialIndexes( vertexMaterialIndexes, indices );
									if( v != null )
										MaterialIndexRangesFromVertexData = v;
								}
							}
						}

						MaterialIndexRangesFromVertexDataCalculated = true;
					}

					return MaterialIndexRangesFromVertexData;
				}

				//public unsafe Mesh.CompiledData.RayCastResult VirtualizedRayCast( Ray ray, Mesh.CompiledData.RayCastModes mode, bool twoSided )
				//{
				//	return VirtualizedDataUtility.VirtualizedRayCast( VirtualizedData, ray, mode, twoSided );
				//}
			}

			/// <summary>
			/// Prepared rendering LOD data of mesh for scene rendering.
			/// </summary>
			public struct IMeshDataLODLevel
			{
				public Mesh Mesh;
				public float Distance;//Squared;
				public int VoxelGridSize;

				//

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public float GetOutputDistance( ViewportRenderingContext context, float objectBoundingSize )
				{
					if( VoxelGridSize != 0 )
					{
						var voxelDistance = context.GetVoxelLodDistanceByObjectSize( VoxelGridSize, objectBoundingSize );
						if( voxelDistance != 0 && voxelDistance > Distance )
							return voxelDistance;
					}

					return Distance;
				}
			}

			/// <summary>
			/// Prepared mesh data for scene rendering.
			/// </summary>
			public interface IMeshData
			{
				object Creator { get; }
				object AnyData { get; }
				List<MeshDataRenderOperation> RenderOperations { get; }
				//IList<MeshDataRenderOperation> RenderOperations { get; }
				SpaceBounds SpaceBounds { get; }
				float VisibilityDistanceFactor { get; }
				//float VisibilityDistance { get; }
				float LODScale { get; }
				bool CastShadows { get; }
				IMeshDataLODLevel[] LODs { get; }
				int BillboardMode { get; }
				Vector3F BillboardPositionOffset { get; }
				float BillboardShadowOffset { get; }
				LayerItem[] PaintLayers { get; }

				bool ContainsDisposedBuffers();
			}

			//public class MeshData
			//{
			//	public object Creator;
			//	public object AnyData;
			//	public IList<RenderOperation> RenderOperations = new List<RenderOperation>();
			//	//public OpenList<RenderOperation> RenderOperations = new OpenList<RenderOperation>();

			//	//initial
			//	public Bounds BoundingBox;
			//	public Sphere BoundingSphere;

			//	public class RenderOperation
			//	{
			//		public object Creator;
			//		public object AnyData;
			//		//!!!!
			//		//public MeshGeometry creator;
			//		//public bool disposeBuffersByCreator = true;
			//		////public Mesh disposeBuffersByObject;
			//		////public bool disposeBuffersByMesh = true;

			//		//!!!!need?
			//		public VertexElement[] vertexStructure;//public GpuVertexDeclaration vertexDeclaration;

			//		//public GpuVertexBuffer vertexBuffer0;
			//		//public GpuVertexBuffer vertexBuffer1;
			//		//public GpuVertexBuffer vertexBuffer2;
			//		//public GpuVertexBuffer vertexBuffer3;
			//		public IList<GpuVertexBuffer> vertexBuffers;
			//		public int vertexStartOffset;
			//		public int vertexCount;

			//		public GpuIndexBuffer indexBuffer;
			//		public int indexStartOffset;
			//		public int indexCount;

			//		///// The type of operation to perform
			//		//OperationType operationType;

			//		//!!!!pass/material
			//		public Material material;

			//		//

			//		public bool ContainsDisposedBuffers()
			//		{
			//			if( vertexBuffers != null )
			//			{
			//				for( int n = 0; n < vertexBuffers.Count; n++ )
			//					if( vertexBuffers[ n ] != null && vertexBuffers[ n ].Disposed )
			//						return true;
			//			}
			//			//if( vertexBuffer0 != null && vertexBuffer0.Disposed )
			//			//	return true;
			//			//if( vertexBuffer1 != null && vertexBuffer1.Disposed )
			//			//	return true;
			//			//if( vertexBuffer2 != null && vertexBuffer2.Disposed )
			//			//	return true;
			//			//if( vertexBuffer3 != null && vertexBuffer3.Disposed )
			//			//	return true;
			//			if( indexBuffer != null && indexBuffer.Disposed )
			//				return true;
			//			return false;
			//		}

			//		//public GpuVertexBuffer GetVertexBuffer( int index )
			//		//{
			//		//	switch( index )
			//		//	{
			//		//	case 0: return vertexBuffer0;
			//		//	case 1: return vertexBuffer1;
			//		//	case 2: return vertexBuffer2;
			//		//	case 3: return vertexBuffer3;
			//		//	}
			//		//	return null;
			//		//}

			//		//public GpuVertexBuffer[] GetVertexBuffers()
			//		//{
			//		//	int count = 0;
			//		//}
			//	}

			//	public void DisposeBuffers()
			//	{
			//		//for( int n = 0; n < RenderOperations.Count; n++ )
			//		//{
			//		//	ref var oper = ref RenderOperations.Data[ n ];
			//		foreach( var oper in RenderOperations )
			//		{
			//			if( oper.vertexBuffers != null )
			//			{
			//				for( int n = 0; n < oper.vertexBuffers.Count; n++ )
			//					oper.vertexBuffers[ n ]?.Dispose();
			//			}
			//			//oper.vertexBuffer0?.Dispose();
			//			//oper.vertexBuffer1?.Dispose();
			//			//oper.vertexBuffer2?.Dispose();
			//			//oper.vertexBuffer3?.Dispose();
			//			oper.indexBuffer?.Dispose();
			//		}
			//	}

			//	public bool ContainsDisposedBuffers()
			//	{
			//		//!!!!slowly где вызывается?

			//		//	for( int n = 0; n < RenderOperations.Count; n++ )
			//		//{
			//		//	ref var oper = ref RenderOperations.Data[ n ];
			//		foreach( var oper in RenderOperations )
			//		{
			//			if( oper.ContainsDisposedBuffers() )
			//				return true;
			//		}
			//		return false;
			//	}
			//}

			/// <summary>
			/// Prepared billboard item data for scene rendering.
			/// </summary>
			public struct BillboardItem
			{
				public object Creator;
				public object AnyData;
				//!!!!double
				public Vector3 BoundingBoxCenter;//public Bounds BoundingBox;
				public Sphere BoundingSphere;

				public bool CastShadows;
				public float ShadowOffset;
				public bool ReceiveDecals;
				public float MotionBlurFactor;
				public Material Material;

				public Vector3F Position;
				public Vector2F Size;
				public RadianF RotationAngle;
				public QuaternionF RotationQuaternion;

				public ColorValue Color;
				public ColorByte ColorForInstancingData;
				public Vector3F PositionPreviousFrame;
				//!!!!
				//public BillboardData[] BillboardArray;

				public float VisibilityDistance;
				//public RangeF LODRange;//public float LODValue;

				public CutVolumeItem[] CutVolumes;
				public List<ObjectSpecialRenderingEffect> SpecialEffects;

				//!!!!
				public uint CullingByCameraDirectionData;

				//

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public void GetWorldMatrix( out Matrix4F result )
				{
					result.Item0.X = Size.X;
					result.Item0.Y = RotationAngle;
					result.Item0.Z = RotationQuaternion.X;
					result.Item0.W = 0;
					result.Item1.X = RotationQuaternion.Y;
					result.Item1.Y = 1;
					result.Item1.Z = RotationQuaternion.Z;
					result.Item1.W = 0;
					result.Item2.X = RotationQuaternion.W;
					result.Item2.Y = 0;
					result.Item2.Z = Size.Y;
					result.Item2.W = 0;
					result.Item3.X = Position.X;
					result.Item3.Y = Position.Y;
					result.Item3.Z = Position.Z;
					result.Item3.W = 1;

					//Vector3F scale = new Vector3F( Size.X, 1, Size.Y );
					////Vector3F scale = new Vector3F( Size.X, Size.X, Size.Y );
					//Matrix3F.FromScale( ref scale, out Matrix3F matrix3 );
					//Matrix4F.Construct( ref matrix3, ref Position, out result );
				}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public void GetWorldMatrixTranspose( out Matrix3x4F result )
				{
					result.Item0.X = Size.X;
					result.Item0.Y = RotationQuaternion.Y;
					result.Item0.Z = RotationQuaternion.W;
					result.Item0.W = Position.X;
					result.Item1.X = RotationAngle;
					result.Item1.Y = 1;
					result.Item1.Z = 0;
					result.Item1.W = Position.Y;
					result.Item2.X = RotationQuaternion.X;
					result.Item2.Y = RotationQuaternion.Z;
					result.Item2.Z = Size.Y;
					result.Item2.W = Position.Z;

					//GetWorldMatrix( out var worldMatrix );
					//worldMatrix.GetTranspose( out result );
				}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public void GetInstancingData( out ObjectInstanceData data )
				{
					GetWorldMatrixTranspose( out data.Transform );
					data.PositionPreviousFrame = PositionPreviousFrame;
					//data.Unused = 0;

					data.Color = ColorForInstancingData;
					//data.Color = GetColorForInstancingData( ref Color );

					//ColorValue c;
					//c.Red = MathEx.Sqrt( Color.Red * 0.1f );
					//c.Green = MathEx.Sqrt( Color.Green * 0.1f );
					//c.Blue = MathEx.Sqrt( Color.Blue * 0.1f );
					//c.Alpha = MathEx.Sqrt( Color.Alpha * 0.1f );
					//data.Color = c.ToColorPacked();
					////data.Color = ( Color * 0.25f ).ToColorPacked();
					////data.Color = BillboardOne.Color;

					data.LodValue = 0;
					data.VisibilityDistance = VisibilityDistance;
					//!!!!new
					data.ReceiveDecals = ReceiveDecals ? (byte)255 : (byte)0;
					data.MotionBlurFactor = (byte)( MotionBlurFactor * 255 );
					data.Unused1 = 0;
					data.Unused2 = 0;
					data.CullingByCameraDirectionData = CullingByCameraDirectionData;
					//data.ReceiveDecals = ReceiveDecals ? 1.0f : 0.0f;
					//data.MotionBlurFactor = MotionBlurFactor;
				}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public unsafe bool CanUseInstancingForTransparentWith( ref BillboardItem billboardItem )
				{
					if( BoundingBoxCenter != billboardItem.BoundingBoxCenter )
						return false;
					if( BoundingSphere != billboardItem.BoundingSphere )
						return false;
					if( CastShadows != billboardItem.CastShadows )
						return false;
					if( ReceiveDecals != billboardItem.ReceiveDecals )
						return false;
					if( MotionBlurFactor != billboardItem.MotionBlurFactor )
						return false;
					if( Material != billboardItem.Material )
						return false;

					//if( LODRange != billboardItem.LODRange )
					//	return false;
					//if( LODValue != billboardItem.LODValue )
					//	return false;
					//if( LODValue != 0 || billboardItem.LODValue != 0 )
					//	return false;

					//public object Creator;
					//public object AnyData;

					////public BillboardData[] BillboardArray;

					return true;
				}
			}

			/// <summary>
			/// Prepared light item data for scene rendering.
			/// </summary>
			public class LightItem
			//public struct LightItem
			{
				public object Creator;
				public object AnyData;
				public Bounds BoundingBox;
				public Sphere BoundingSphere;

				public Vector3 Position;
				public QuaternionF Rotation;
				public Vector3F Scale;
				//public Transform transform;

				public Light.TypeEnum Type;
				public Vector3F Power;
				public float AttenuationNear;
				public float AttenuationFar;
				public float AttenuationPower;
				public DegreeF SpotlightInnerAngle;
				public DegreeF SpotlightOuterAngle;
				public float SpotlightFalloff;
				public Plane[] SpotlightClipPlanes;
				//public float SourceRadiusOrAngle;
				public bool CastShadows;
				public float ShadowIntensity;
				//public float ShadowSoftness;
				public float ShadowBias;
				public float ShadowNormalBias;
				public float ShadowSoftness;
				public float ShadowContactLength;

				public ImageComponent Mask;
				public double MaskScale;
				//public Vec2 maskPosition;
				//public Vec2 maskScale;
				//public Mat4 maskMatrix;
				//public SpaceBounds spaceBounds;

				//!!!!good?
				public Component[] children;
				//public ILightChild[] children;
			}

			/// <summary>
			/// Prepared reflection probe item data for scene rendering.
			/// </summary>
			public struct ReflectionProbeItem
			{
				public object Creator;
				public object AnyData;
				public Bounds BoundingBox;

				public Sphere Sphere;
				public ImageComponent CubemapEnvironment;
				public Vector4F[] HarmonicsIrradiance;//public ImageComponent CubemapIrradiance;
				public QuaternionF Rotation;//public Matrix3F Rotation;
				public Vector3F Multiplier;
			}

			/// <summary>
			/// Prepared decal item data for scene rendering.
			/// </summary>
			public struct DecalItem
			{
				public object Creator;
				public object AnyData;
				public Bounds BoundingBox;
				//!!!!boundingSphere?

				public Vector3 Position;
				public QuaternionF Rotation;
				public Vector3F Scale;
				//!!!!? instancing?
				//public Matrix4F MeshInstanceOne;

				public Material Material;
				public ColorValue Color;
				public Decal.NormalsModeEnum NormalsMode;
				public double SortOrder;

				public float VisibilityDistance;
			}

			//!!!!need add support for billboards, particles
			public struct CutVolumeItem
			{
				public CutVolumeShape Shape;
				public Transform Transform;
				public CutVolumeFlags Flags;
				//public bool Invert;
				//public bool CutScene;
				//public bool CutShadows;
				//public bool CutSimple3DRenderer;

				//public Plane Plane;

				////////

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public bool Equals( ref CutVolumeItem other )
				{
					return Shape == other.Shape && Transform == other.Transform && /*Plane == other.Plane && */Flags == other.Flags;
					//return Shape == other.Shape && Transform == other.Transform && /*Plane == other.Plane && */Invert == other.Invert && CutScene == other.CutScene && CutShadows == other.CutShadows && CutSimple3DRenderer == other.CutSimple3DRenderer;
				}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public CutVolumeItem( Sphere sphere, CutVolumeFlags flags ) //bool invert, bool cutScene, bool cutShadows, bool cutSimple3DRenderer )
				{
					Shape = CutVolumeShape.Sphere;
					var scl = sphere.Radius * 2;
					Transform = new Transform( sphere.Center, Quaternion.Identity, new Vector3( scl, scl, scl ) );
					//Plane = Plane.Zero;
					Flags = flags;
					//Invert = invert;
					//CutScene = cutScene;
					//CutShadows = cutShadows;
					//CutSimple3DRenderer = cutSimple3DRenderer;
				}

				//public CutVolumeItem( Plane plane, bool invert, bool cutScene, bool cutShadows, bool cutSimple3DRenderer )
				//{
				//	Shape = CutVolumeShape.Plane;
				//	Transform = null;
				//	Plane = plane;
				//	Invert = invert;
				//	CutScene = cutScene;
				//	CutShadows = cutShadows;
				//	CutSimple3DRenderer = cutSimple3DRenderer;
				//}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public CutVolumeItem( Box box, CutVolumeFlags flags )//, bool invert, bool cutScene, bool cutShadows, bool cutSimple3DRenderer )
				{
					box.Axis.ToQuaternion( out var rot );

					Shape = CutVolumeShape.Box;
					Transform = new Transform( box.Center, rot, box.Extents );
					//Plane = Plane.Zero;
					Flags = flags;
					//Invert = invert;
					//CutScene = cutScene;
					//CutShadows = cutShadows;
					//CutSimple3DRenderer = cutSimple3DRenderer;
				}

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				public CutVolumeItem( Cylinder cylinder, CutVolumeFlags flags )//bool invert, bool cutScene, bool cutShadows, bool cutSimple3DRenderer )
				{
					var rot = Quaternion.FromDirectionZAxisUp( cylinder.GetDirection() );

					Shape = CutVolumeShape.Cylinder;
					Transform = new Transform( cylinder.GetCenter(), rot, new Vector3( cylinder.GetLength(), cylinder.Radius * 2, cylinder.Radius * 2 ) );
					//Plane = Plane.Zero;
					Flags = flags;
					//Invert = invert;
					//CutScene = cutScene;
					//CutShadows = cutShadows;
					//CutSimple3DRenderer = cutSimple3DRenderer;
				}

				public bool Invert
				{
					[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
					get { return ( Flags & CutVolumeFlags.Invert ) != 0; }
				}

				public bool CutScene
				{
					[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
					get { return ( Flags & CutVolumeFlags.CutScene ) != 0; }
				}

				public bool CutShadows
				{
					[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
					get { return ( Flags & CutVolumeFlags.CutShadows ) != 0; }
				}

				public bool CutSimple3DRenderer
				{
					[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
					get { return ( Flags & CutVolumeFlags.CutSimple3DRenderer ) != 0; }
				}
			}

			public struct TransparentRenderingAddOffsetWhenSortByDistanceVolumeItem
			{
				//!!!!add more shapes
				public Box Box;
			}

			/// <summary>
			/// Specifies instancing data of the object.
			/// </summary>
			[StructLayout( LayoutKind.Sequential, Pack = 1 )]
			public struct ObjectInstanceData
			{
				//48 bytes
				public Matrix3x4F Transform;

				//16 bytes
				public Vector3F PositionPreviousFrame;
				public ColorByte Color;

				//16 bytes
				public float LodValue;
				public float VisibilityDistance;
				public byte ReceiveDecals;
				public byte MotionBlurFactor;
				public byte Unused1;
				public byte Unused2;
				public uint CullingByCameraDirectionData;

				//public float LodValue;
				//public float VisibilityDistance;
				//public float ReceiveDecals;
				//public float MotionBlurFactor;

				//

				public void Init( ref Matrix4F transform, ref Vector3F positionPreviousFrame, ref ColorValue color, float lodValue, float visibilityDistance, bool receiveDecals, float motionBlurFactor, uint cullingByCameraDirectionData )
				{
					transform.GetTranspose( out Transform );

					PositionPreviousFrame = positionPreviousFrame;
					//Unused = 0;

					Color = GetColorForInstancingData( ref color );
					//ColorValue c;
					//c.Red = MathEx.Sqrt( color.Red * 0.1f );
					//c.Green = MathEx.Sqrt( color.Green * 0.1f );
					//c.Blue = MathEx.Sqrt( color.Blue * 0.1f );
					//c.Alpha = MathEx.Sqrt( color.Alpha * 0.1f );
					//Color = c.ToColorPacked();
					////Color = ( color * 0.25f ).ToColorPacked();
					////Color = new ColorByte( color );

					LodValue = lodValue;
					VisibilityDistance = visibilityDistance;
					ReceiveDecals = receiveDecals ? (byte)255 : (byte)0;
					MotionBlurFactor = (byte)( motionBlurFactor * 255 );
					CullingByCameraDirectionData = cullingByCameraDirectionData;
					//ReceiveDecals = receiveDecals ? 1.0f : 0.0f;
					//MotionBlurFactor = motionBlurFactor;
				}
			}

			/// <summary>
			/// Specifies data of the layer.
			/// </summary>
			public struct LayerItem
			{
				public PaintLayer Layer { get; }
				public PaintLayer.MaskFormatEnum MaskFormat { get; }
				public ImageComponent Mask { get; }
				public long UniqueMaskDataCounter { get; }

				public Material OriginalMaterial { get; }
				public PaintLayer.BlendModeEnum BlendMode { get; }
				public ColorValue MaterialColor { get; }

				public Surface Surface { get; }
				public bool SurfaceObjects { get; }
				public float SurfaceObjectsDistribution { get; }
				public float SurfaceObjectsScale { get; }
				public ColorValue SurfaceObjectsColor { get; }
				public float SurfaceObjectsVisibilityDistanceFactor { get; }
				public bool SurfaceObjectsCastShadows { get; }
				public bool SurfaceObjectsCollision { get; }

				Material.CompiledMaterialData resultMaterial;
				public Material.CompiledMaterialData ResultMaterial { get { return resultMaterial; } }

				//

				public LayerItem( PaintLayer layer, ImageComponent mask, long uniqueMaskDataCounter, bool calculateResultMaterial )
				{
					Layer = layer;
					MaskFormat = layer.MaskFormat;
					Mask = mask;
					UniqueMaskDataCounter = uniqueMaskDataCounter;

					OriginalMaterial = layer.GetMaterial();
					BlendMode = layer.BlendMode;
					MaterialColor = layer.MaterialColor;

					Surface = layer.Surface;
					SurfaceObjects = layer.SurfaceObjects;
					SurfaceObjectsDistribution = (float)layer.SurfaceObjectsDistribution;
					SurfaceObjectsScale = (float)layer.SurfaceObjectsScale;
					SurfaceObjectsColor = layer.SurfaceObjectsColor;
					SurfaceObjectsVisibilityDistanceFactor = (float)layer.SurfaceObjectsVisibilityDistanceFactor;
					SurfaceObjectsCastShadows = layer.SurfaceObjectsCastShadows;
					SurfaceObjectsCollision = layer.SurfaceObjectsCollision;

					//!!!!

					resultMaterial = null;

					if( calculateResultMaterial )
						CalculateResultMaterial();
				}

				public void CalculateResultMaterial()
				{
					if( OriginalMaterial != null )
					{
						if( OriginalMaterial.BlendMode.Value == Material.BlendModeEnum.Opaque || !OriginalMaterial.Opacity.ReferenceSpecified )
						{
							var useTransparent = false;
							switch( BlendMode )
							{
							case PaintLayer.BlendModeEnum.Auto: useTransparent = OriginalMaterial.Result != null && OriginalMaterial.Result.Transparent; break;
							case PaintLayer.BlendModeEnum.Masked: useTransparent = false; break;
							case PaintLayer.BlendModeEnum.Transparent: useTransparent = true; break;
							}

							resultMaterial = OriginalMaterial.Compile( useTransparent ? Material.CompiledMaterialData.SpecialMode.PaintLayerTransparent : Material.CompiledMaterialData.SpecialMode.PaintLayerMasked, null, 0, null, null, 0 );
						}
						else
							resultMaterial = OriginalMaterial.Result;
					}
					else
						resultMaterial = null;
				}

				public override bool Equals( object obj )
				{
					if( obj is LayerItem )
					{
						var obj2 = (LayerItem)obj;
						return Mask == obj2.Mask && UniqueMaskDataCounter == obj2.UniqueMaskDataCounter && OriginalMaterial == obj2.OriginalMaterial && BlendMode == obj2.BlendMode && MaterialColor == obj2.MaterialColor && Surface == obj2.Surface && SurfaceObjects == obj2.SurfaceObjects && SurfaceObjectsDistribution == obj2.SurfaceObjectsDistribution && SurfaceObjectsScale == obj2.SurfaceObjectsScale && SurfaceObjectsColor == obj2.SurfaceObjectsColor && SurfaceObjectsVisibilityDistanceFactor == obj2.SurfaceObjectsVisibilityDistanceFactor && SurfaceObjectsCastShadows == obj2.SurfaceObjectsCastShadows && SurfaceObjectsCollision == obj2.SurfaceObjectsCollision;
					}
					else
						return false;
				}

				public override int GetHashCode()
				{
					var result = 0;
					if( Mask != null )
						result ^= Mask.GetHashCode();
					unchecked
					{
						result ^= (int)UniqueMaskDataCounter;
					}
					if( OriginalMaterial != null )
						result ^= OriginalMaterial.GetHashCode();
					result ^= BlendMode.GetHashCode();
					result ^= MaterialColor.GetHashCode();
					if( Surface != null )
					{
						result ^= Surface.GetHashCode();
						result ^= SurfaceObjects.GetHashCode();
						result ^= SurfaceObjectsDistribution.GetHashCode();
						result ^= SurfaceObjectsScale.GetHashCode();
						result ^= SurfaceObjectsColor.GetHashCode();
						result ^= SurfaceObjectsVisibilityDistanceFactor.GetHashCode();
						result ^= SurfaceObjectsCastShadows.GetHashCode();
						result ^= SurfaceObjectsCollision.GetHashCode();
					}
					return result;
				}
			}

			public struct ActionToDoAfterPrepareListsOfObjectsSortedByDistance
			{
				public float DistanceToCamera;
				public Action<ViewportRenderingContext> Action;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents environment texture data.
		/// </summary>
		public struct EnvironmentTextureData
		{
			public ImageComponent Texture;
			public QuaternionF Rotation;
			//public Matrix3F rotation;
			public Vector4F MultiplierAndAffect;

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public EnvironmentTextureData( ImageComponent texture, float affect, ref QuaternionF rotation, ref Vector3F multiplier )
			//public EnvironmentTextureData( ImageComponent texture, float affect, ref Matrix3F rotation, ref Vector3F multiplier )
			{
				Texture = texture;
				Rotation = rotation;
				MultiplierAndAffect = new Vector4F( multiplier, affect );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public EnvironmentTextureData( ImageComponent texture, float affect )
			{
				this.Texture = texture;
				this.Rotation = QuaternionF.Identity;//Matrix3F.Identity;
				this.MultiplierAndAffect = new Vector4F( Vector3F.One, affect );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents environment irradiance data.
		/// </summary>
		public struct EnvironmentIrradianceData
		{
			public static readonly Vector4F[] BlackHarmonics = new Vector4F[ 9 ] { Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero };
			public static readonly Vector4F[] WhiteHarmonics = new Vector4F[ 9 ] { Vector4F.One, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero };
			public static readonly Vector4F[] GrayHarmonics = new Vector4F[ 9 ] { new Vector4F( 0.5f, 0.5f, 0.5f, 0 ), Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero };

			//

			public Vector4F[] Harmonics;
			public QuaternionF Rotation;
			public Vector4F MultiplierAndAffect;

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public EnvironmentIrradianceData( Vector4F[] harmonics, float affect, ref QuaternionF rotation, ref Vector3F multiplier )
			{
				Harmonics = harmonics;
				Rotation = rotation;
				MultiplierAndAffect = new Vector4F( multiplier, affect );
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public EnvironmentIrradianceData( Vector4F[] harmonics, float affect )
			{
				Harmonics = harmonics;
				Rotation = QuaternionF.Identity;
				MultiplierAndAffect = new Vector4F( Vector3F.One, affect );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public struct OccluderItem
		{
			public Vector3 Center;
			public Vector3[] Vertices;
			public int[] Indices;

			internal double tempDistanceSquared;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		static RenderingPipeline()
		{
			var one = ColorValue.One;
			colorOneForInstancingData = GetColorForInstancingData( ref one );

			unsafe
			{
				if( sizeof( RenderSceneData.ObjectInstanceData ) != 80 )
					Log.Fatal( "RenderingPipeline: sizeof( RenderSceneData.ObjectInstanceData ) != 80." );
			}
		}

		public abstract void Render( ViewportRenderingContext context );

		///// <summary>
		///// Represents an interface to access a data for rendering frame.
		///// </summary>
		//public interface IFrameData
		//{
		//	RenderSceneData RenderSceneData { get; }
		//}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( InstancingMinCount ):
				case nameof( InstancingMaxCount ):
				case nameof( InstancingTransparent ):
					if( !Instancing )
						skip = true;
					break;
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public virtual bool GetHighDynamicRange()
		{
			var result = HighDynamicRange.Value;
			if( result == AutoTrueFalse.Auto )
				result = SystemSettings.LimitedDevice ? AutoTrueFalse.False : AutoTrueFalse.True;
			return result == AutoTrueFalse.True;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public virtual bool GetUseMultiRenderTargets()
		{
			if( !UseRenderTargets.Value )
				return false;

			//multi render targets are not supported on limited devices
			if( SystemSettings.LimitedDevice )
				return false;

			var result = UseMultiRenderTargets.Value;
			if( result == AutoTrueFalse.Auto )
				result = SystemSettings.LimitedDevice ? AutoTrueFalse.False : AutoTrueFalse.True;
			return result == AutoTrueFalse.True;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public virtual bool GetSimpleGeometryAntialiasing()
		{
			var result = SimpleGeometryAntialiasing.Value;
			if( result == AutoTrueFalse.Auto )
				result = SystemSettings.LimitedDevice ? AutoTrueFalse.False : AutoTrueFalse.True;
			return result == AutoTrueFalse.True;
		}

		////!!!!
		//public virtual void PhysicallyCorrectRendering_ResetFrame()
		//{
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal protected virtual void SetCutVolumeSettingsUniforms( ViewportRenderingContext context, RenderSceneData.CutVolumeItem[] cutVolumes, bool forceUpdate ) { }

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal static bool IsEqualCutVolumes( RenderSceneData.CutVolumeItem[] array1, RenderSceneData.CutVolumeItem[] array2 )
		{
			if( array1 == null && array2 == null )
				return true;
			if( array1 != null && array2 == null )
				return false;
			if( array1 == null && array2 != null )
				return false;
			if( array1.Length != array2.Length )
				return false;
			for( int n = 0; n < array1.Length; n++ )
				if( !array1[ n ].Equals( ref array2[ n ] ) )
					return false;
			return true;
		}


		static float[] getColorForInstancingDataTable;

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static float GetColorForInstancingData( float v )
		{
			if( v >= 0 && v <= 0.1f )
			{
				var v2 = (int)( v * 10240 );
				if( v2 >= 0 && v2 < 1024 )
					return getColorForInstancingDataTable[ v2 ];
			}

			return MathEx.Sqrt( v );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static ColorByte GetColorForInstancingData( ref ColorValue color )
		{
			//var caps = EngineApp._DebugCapsLock;
			//if( caps )
			//{

			//for 0-0.1 range
			if( getColorForInstancingDataTable == null )
			{
				var table = new float[ 1024 ];
				for( int n = 0; n < 1024; n++ )
					table[ n ] = MathEx.Sqrt( ( (float)n ) / 1024 * 0.1f );
				getColorForInstancingDataTable = table;
			}

			ColorValue c;
			c.Red = GetColorForInstancingData( color.Red * 0.1f );
			c.Green = GetColorForInstancingData( color.Green * 0.1f );
			c.Blue = GetColorForInstancingData( color.Blue * 0.1f );
			c.Alpha = GetColorForInstancingData( color.Alpha * 0.1f );
			return c.ToColorPacked();
			//}
			//else
			//{
			//	ColorValue c;
			//	c.Red = MathEx.Sqrt( color.Red * 0.1f );
			//	c.Green = MathEx.Sqrt( color.Green * 0.1f );
			//	c.Blue = MathEx.Sqrt( color.Blue * 0.1f );
			//	c.Alpha = MathEx.Sqrt( color.Alpha * 0.1f );
			//	return c.ToColorPacked();
			//}
		}

		public static ColorByte ColorOneForInstancingData
		{
			get { return colorOneForInstancingData; }
		}
		static ColorByte colorOneForInstancingData;

		public static uint EncodeCullingByCameraDirectionData( Vector3F normal, float viewAngleFactor = 0 )
		{
			if( normal != Vector3F.Zero )
			{
				var value = new Vector4F( normal * 0.5f + new Vector3F( 0.5f, 0.5f, 0.5f ), viewAngleFactor );
				return new ColorByte( value ).PackedValue;
			}
			return 0;
		}
	}
}