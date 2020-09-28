// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// The rendering pipeline of the scene.
	/// </summary>
	public abstract class Component_RenderingPipeline : Component
	{
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
		//public event Action<Component_RenderingPipeline> BackgroundColorChanged;
		//ReferenceField<ColorValue> _backgroundColor = new ColorValue( 0, 0, 0 );

		/// <summary>
		/// Enables the deferred shading. When Auto mode is enabled, deferred shading is enabled, but other than Intel GPUs.
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "General" )]
		public Reference<AutoTrueFalse> DeferredShading
		{
			get { if( _deferredShading.BeginGet() ) DeferredShading = _deferredShading.Get( this ); return _deferredShading.value; }
			set { if( _deferredShading.BeginSet( ref value ) ) { try { DeferredShadingChanged?.Invoke( this ); } finally { _deferredShading.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DeferredShading"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline> DeferredShadingChanged;
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
		public event Action<Component_RenderingPipeline> HighDynamicRangeChanged;
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
		public event Action<Component_RenderingPipeline> UseRenderTargetsChanged;
		ReferenceField<bool> _useRenderTargets = true;

		/// <summary>
		/// Enables using multi render targets during rendering the frame. When Auto mode is enabled, MRT is disabled for limited devices (mobile).
		/// </summary>
		[DefaultValue( AutoTrueFalse.Auto )]
		[Category( "General" )]
		public Reference<AutoTrueFalse> UseMultiRenderTargets
		{
			get { if( _useMultiRenderTargets.BeginGet() ) UseMultiRenderTargets = _useMultiRenderTargets.Get( this ); return _useMultiRenderTargets.value; }
			set { if( _useMultiRenderTargets.BeginSet( ref value ) ) { try { UseMultiRenderTargetsChanged?.Invoke( this ); } finally { _useMultiRenderTargets.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="UseMultiRenderTargets"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline> UseMultiRenderTargetsChanged;
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
		public event Action<Component_RenderingPipeline> SimpleGeometryAntialiasingChanged;
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
		public event Action<Component_RenderingPipeline> InstancingChanged;
		ReferenceField<bool> _instancing = true;

		/// <summary>
		/// The minimum number of objects to enable GPU instancing.
		/// </summary>
		[Category( "Instancing" )]
		[DefaultValue( 10 )]
		[Range( 2, 1000, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<int> InstancingMinCount
		{
			get { if( _instancingMinCount.BeginGet() ) InstancingMinCount = _instancingMinCount.Get( this ); return _instancingMinCount.value; }
			set { if( _instancingMinCount.BeginSet( ref value ) ) { try { InstancingMinCountChanged?.Invoke( this ); } finally { _instancingMinCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="InstancingMinCount"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline> InstancingMinCountChanged;
		ReferenceField<int> _instancingMinCount = 10;

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
		public event Action<Component_RenderingPipeline> InstancingMaxCountChanged;
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
		public event Action<Component_RenderingPipeline> InstancingTransparentChanged;
		ReferenceField<bool> _instancingTransparent = true;

		/////////////////////////////////////////

		/// <summary>
		/// Specifies the quality for procedural generation of 3D models and other assets. A value of 0.75 is optimal for real-time graphics. The higher values are more applicable for non real-time rendering.
		/// </summary>
		[Category( "Procedural Generation" )]
		[DefaultValue( 0.75 )]
		[Range( 0, 1 )]
		public Reference<double> ProceduralGenerationQuality
		{
			get { if( _proceduralGenerationQuality.BeginGet() ) ProceduralGenerationQuality = _proceduralGenerationQuality.Get( this ); return _proceduralGenerationQuality.value; }
			set { if( _proceduralGenerationQuality.BeginSet( ref value ) ) { try { ProceduralGenerationQualityChanged?.Invoke( this ); } finally { _proceduralGenerationQuality.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ProceduralGenerationQuality"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline> ProceduralGenerationQualityChanged;
		ReferenceField<double> _proceduralGenerationQuality = 0.75;

		/////////////////////////////////////////

		/// <summary>
		/// Specifies the distance multiplier when determining the level of detail.
		/// </summary>
		[Category( "Level Of Detail" )]
		[DefaultValue( 1.0 )]
		[DisplayName( "LOD Scale" )]
		[Range( 0, 10, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		public Reference<double> LODScale
		{
			get { if( _lODScale.BeginGet() ) LODScale = _lODScale.Get( this ); return _lODScale.value; }
			set { if( _lODScale.BeginSet( ref value ) ) { try { LODScaleChanged?.Invoke( this ); } finally { _lODScale.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LODScale"/> property value changes.</summary>
		public event Action<Component_RenderingPipeline> LODScaleChanged;
		ReferenceField<double> _lODScale = 1.0;

		/// <summary>
		/// Specifies the minimum and maximum levels of detail.
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
		public event Action<Component_RenderingPipeline> LODRangeChanged;
		ReferenceField<RangeI> _lODRange = new RangeI( 0, 10 );

		///// <summary>
		///// Specifies the distance in the case when the distances of the levels of detail are not specified in a 3D model.
		///// </summary>
		//[Category( "Level Of Detail" )]
		//[DefaultValue( 20.0 )]
		//[DisplayName( "LOD Auto Distance" )]
		//[Range( 10, 100, RangeAttribute.ConvenientDistributionEnum.Exponential )]
		//public Reference<double> LODAutoDistance
		//{
		//	get { if( _lODAutoDistance.BeginGet() ) LODAutoDistance = _lODAutoDistance.Get( this ); return _lODAutoDistance.value; }
		//	set { if( _lODAutoDistance.BeginSet( ref value ) ) { try { LODAutoDistanceChanged?.Invoke( this ); } finally { _lODAutoDistance.EndSet(); } } }
		//}
		//public event Action<Component_RenderingPipeline> LODAutoDistanceChanged;
		//ReferenceField<double> _lODAutoDistance = 20.0;

		/// <summary>
		/// Specifies the transition time between levels of detail.
		/// </summary>
		[Category( "Level Of Detail" )]
		[DefaultValue( 1.0 )]
		[DisplayName( "LOD Transition Time" )]
		[Range( 0, 2 )]
		public Reference<double> LODTransitionTime
		{
			get { if( _lODTransitionTime.BeginGet() ) LODTransitionTime = _lODTransitionTime.Get( this ); return _lODTransitionTime.value; }
			set { if( _lODTransitionTime.BeginSet( ref value ) ) { try { LODTransitionTimeChanged?.Invoke( this ); } finally { _lODTransitionTime.EndSet(); } } }
		}
		public event Action<Component_RenderingPipeline> LODTransitionTimeChanged;
		ReferenceField<double> _lODTransitionTime = 1.0;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a list of material passes for shadow caster.
		/// </summary>
		public class ShadowCasterData
		{
			public GpuMaterialPass[] passByLightType;

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
			public OpenList<MeshItem> Meshes = new OpenList<MeshItem>( 2048 );
			public OpenList<BillboardItem> Billboards = new OpenList<BillboardItem>( 512 );
			public OpenList<LightItem> Lights = new OpenList<LightItem>( 256 );
			public OpenList<ReflectionProbeItem> ReflectionProbes = new OpenList<ReflectionProbeItem>( 128 );
			public OpenList<DecalItem> Decals = new OpenList<DecalItem>( 512 );

			/// <summary>
			/// Prepared mesh item data for scene rendering.
			/// </summary>
			public struct MeshItem
			{
				//!!!!меньше занимать памяти

				public object Creator;
				public object AnyData;
				//!!!!double
				public Vector3 BoundingBoxCenter;//public Bounds BoundingBox;
				public Sphere BoundingSphere;

				public bool CastShadows;
				public bool ReceiveDecals;
				public Component_Material ReplaceMaterial;
				public Component_Material[] ReplaceMaterialSelectively;
				public ColorValue Color;
				public AnimationDataClass AnimationData;

				public IMeshData MeshData;
				//public Component_Mesh Mesh;

				public Matrix4F Transform;
				public Vector3F PositionPreviousFrame;

				public GpuVertexBuffer BatchingInstanceBuffer;

				public float LODValue;

				//!!!!GC. fixed array. или уже подготовленный массив выставляется
				public LayerItem[] Layers;

				//public MeshInstanceData? MeshInstanceOne;
				//public MeshInstanceData[] MeshInstanceArray;

				////////

				//public struct MeshInstanceData
				//{
				//	public Vector3 Position;
				//	//Matrix3F RotationScale;
				//	public QuaternionF Rotation;
				//	public Vector3F Scale;
				//	//public ColorValue VertexColorMultiplier;//!!!!additional data?
				//}

				//!!!!without class?
				/// <summary>
				/// Prepared animation data of mesh for scene rendering.
				/// </summary>
				public class AnimationDataClass
				{
					public int Mode;
					public Component_Image BonesTexture;
				}

				////////

				public void GetInstancingData( out ObjectInstanceData data )
				{
					Transform.GetTranspose( out data.Transform );
					data.PositionPreviousFrame = PositionPreviousFrame;
					//data.Unused = 0;
					//!!!!slowly?
					ColorValue c;
					c.Red = MathEx.Sqrt( Color.Red / 10 );
					c.Green = MathEx.Sqrt( Color.Green / 10 );
					c.Blue = MathEx.Sqrt( Color.Blue / 10 );
					c.Alpha = MathEx.Sqrt( Color.Alpha / 10 );
					data.Color = c.ToColorPacked();
					//data.Color = ( Color * 0.25f ).ToColorPacked();
					//data.Color = Color;
				}

				public unsafe bool CanUseInstancingForTransparentWith( ref MeshItem meshItem )
				{
					if( BoundingBoxCenter != meshItem.BoundingBoxCenter )
						return false;
					if( BoundingSphere != meshItem.BoundingSphere )
						return false;
					if( CastShadows != meshItem.CastShadows )
						return false;
					if( ReceiveDecals != meshItem.ReceiveDecals )
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

					if( LODValue != meshItem.LODValue )
						return false;
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
				public object AnyData;
				//!!!!
				//public Component_MeshGeometry creator;
				//public bool disposeBuffersByCreator = true;
				////public Component_Mesh disposeBuffersByObject;
				////public bool disposeBuffersByMesh = true;

				public VertexElement[] VertexStructure;
				public bool VertexStructureContainsColor;
				public UnwrappedUVEnum UnwrappedUV;

				//!!!!

				public IList<GpuVertexBuffer> VertexBuffers;
				public int VertexStartOffset;
				public int VertexCount;

				public GpuIndexBuffer IndexBuffer;
				public int IndexStartOffset;
				public int IndexCount;

				////!!!!new
				/////// The type of operation to perform
				//RenderOperationType OperationType { get; }

				//!!!!pass/material
				public Component_Material Material;

				public int _currentRenderingFrameIndex = -1;

				//

				public MeshDataRenderOperation()
				{
				}

				public MeshDataRenderOperation( object creator )
				{
					Creator = creator;
				}

				public bool ContainsDisposedBuffers()
				{
					if( VertexBuffers != null )
					{
						for( int n = 0; n < VertexBuffers.Count; n++ )
							if( VertexBuffers[ n ] != null && VertexBuffers[ n ].Disposed )
								return true;
					}
					if( IndexBuffer != null && IndexBuffer.Disposed )
						return true;
					return false;
				}
			}

			/// <summary>
			/// Prepared rendering LOD data of mesh for scene rendering.
			/// </summary>
			public struct IMeshDataLODLevel
			{
				public Component_Mesh Mesh;
				public float DistanceSquared;
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
				IMeshDataLODLevel[] LODs { get; }
				int BillboardMode { get; }
				LayerItem[] PaintLayers { get; }

				bool ContainsDisposedBuffers();
			}

			//public class MeshData
			//{
			//	public object Creator;
			//	public object AnyData;
			//	public IList<RenderOperation> RenderOperations = new List<RenderOperation>();
			//	//public OpenList<RenderOperation> RenderOperations = new OpenList<RenderOperation>();

			//	xx xx;//initial
			//	public Bounds BoundingBox;
			//	public Sphere BoundingSphere;

			//	public class RenderOperation
			//	{
			//		public object Creator;
			//		public object AnyData;
			//		//!!!!
			//		//public Component_MeshGeometry creator;
			//		//public bool disposeBuffersByCreator = true;
			//		////public Component_Mesh disposeBuffersByObject;
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
			//		public Component_Material material;

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
				//!!!!меньше занимать памяти

				public object Creator;
				public object AnyData;
				//!!!!double
				public Vector3 BoundingBoxCenter;//public Bounds BoundingBox;
				public Sphere BoundingSphere;

				public bool CastShadows;
				public bool ReceiveDecals;
				public Component_Material Material;

				public Vector3F Position;
				public Vector2F Size;
				public RadianF Rotation;
				public ColorValue Color;
				public Vector3F PositionPreviousFrame;
				//!!!!
				//public BillboardData[] BillboardArray;

				public float LODValue;

				//

				public void GetWorldMatrix( out Matrix4F result )
				{
					result.Item0.X = Size.X;
					result.Item0.Y = Rotation;// 0;
					result.Item0.Z = 0;
					result.Item0.W = 0;
					result.Item1.X = 0;
					result.Item1.Y = 1;
					result.Item1.Z = 0;
					result.Item1.W = 0;
					result.Item2.X = 0;
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

				public void GetWorldMatrixTranspose( out Matrix3x4F result )
				{
					result.Item0.X = Size.X;
					result.Item0.Y = 0;
					result.Item0.Z = 0;
					result.Item0.W = Position.X;
					result.Item1.X = Rotation;// 0
					result.Item1.Y = 1;
					result.Item1.Z = 0;
					result.Item1.W = Position.Y;
					result.Item2.X = 0;
					result.Item2.Y = 0;
					result.Item2.Z = Size.Y;
					result.Item2.W = Position.Z;

					//GetWorldMatrix( out var worldMatrix );
					//worldMatrix.GetTranspose( out result );
				}

				public void GetInstancingData( out ObjectInstanceData data )
				{
					GetWorldMatrixTranspose( out data.Transform );
					data.PositionPreviousFrame = PositionPreviousFrame;
					//data.Unused = 0;
					//!!!!slowly?
					ColorValue c;
					c.Red = MathEx.Sqrt( Color.Red / 10 );
					c.Green = MathEx.Sqrt( Color.Green / 10 );
					c.Blue = MathEx.Sqrt( Color.Blue / 10 );
					c.Alpha = MathEx.Sqrt( Color.Alpha / 10 );
					data.Color = c.ToColorPacked();
					//data.Color = ( Color * 0.25f ).ToColorPacked();
					//data.Color = BillboardOne.Color;
				}

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
					if( Material != billboardItem.Material )
						return false;

					if( LODValue != billboardItem.LODValue )
						return false;
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

				public Component_Light.TypeEnum Type;
				public Vector3F Power;
				public float AttenuationNear;
				public float AttenuationFar;
				public float AttenuationPower;
				public DegreeF SpotlightInnerAngle;
				public DegreeF SpotlightOuterAngle;
				public float SpotlightFalloff;
				public Plane[] SpotlightClipPlanes;
				public bool CastShadows;
				public float ShadowIntensity;
				//public float ShadowSoftness;
				public float ShadowBias;
				public float ShadowNormalBias;
				public Component_Image Mask;
				//!!!!
				public double MaskScale;
				//public Vec2 maskPosition;
				//public Vec2 maskScale;
				//public Mat4 maskMatrix;
				//public SpaceBounds spaceBounds;

				//!!!!good?
				public Component[] children;
				//public IComponent_LightChild[] children;
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
				public Component_Image CubemapEnvironment;
				public Component_Image CubemapIrradiance;
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

				public Component_Material Material;
				public ColorValue Color;
				public Component_Decal.NormalsModeEnum NormalsMode;
				public double SortOrder;
			}

			/// <summary>
			/// Specifies instancing data of the object.
			/// </summary>
			[StructLayout( LayoutKind.Sequential, Pack = 1 )]
			public struct ObjectInstanceData
			{
				public Matrix3x4F Transform;
				//можно было бы хранить смещение, тогда 16-bit точности хватило бы. хотя в 16-bit долго конвертировать
				public Vector3F PositionPreviousFrame;
				public ColorByte Color;
				//public float Unused;

				//

				public void Init( ref Matrix4F transform, ref Vector3F positionPreviousFrame, ref ColorValue color )
				{
					transform.GetTranspose( out Transform );
					PositionPreviousFrame = positionPreviousFrame;
					//Unused = 0;
					//!!!!slowly?
					ColorValue c;
					c.Red = MathEx.Sqrt( color.Red / 10 );
					c.Green = MathEx.Sqrt( color.Green / 10 );
					c.Blue = MathEx.Sqrt( color.Blue / 10 );
					c.Alpha = MathEx.Sqrt( color.Alpha / 10 );
					Color = c.ToColorPacked();
					//Color = ( color * 0.25f ).ToColorPacked();
					//Color = new ColorByte( color );
				}
			}

			/// <summary>
			/// Specifies data of the layer.
			/// </summary>
			public struct LayerItem
			{
				public Component_Material Material;
				public Component_Image Mask;
				public long UniqueMaskDataCounter;
				public ColorValue Color;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents environment texture data.
		/// </summary>
		public struct EnvironmentTextureData
		{
			public Component_Image texture;
			public Matrix3F rotation;
			public Vector3F multiplier;

			public EnvironmentTextureData( Component_Image texture, ref Matrix3F rotation, ref Vector3F multiplier )
			{
				this.texture = texture;
				this.rotation = rotation;
				this.multiplier = multiplier;
			}

			public EnvironmentTextureData( Component_Image texture )
			{
				this.texture = texture;
				this.rotation = Matrix3F.Identity;
				this.multiplier = Vector3F.One;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public abstract void Render( ViewportRenderingContext context );

		/// <summary>
		/// Represents an interface to access a data for rendering frame.
		/// </summary>
		public interface IFrameData
		{
			RenderSceneData RenderSceneData { get; }
		}

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

		public virtual bool GetHighDynamicRange()
		{
			var result = HighDynamicRange.Value;
			if( result == AutoTrueFalse.Auto )
				result = SystemSettings.LimitedDevice ? AutoTrueFalse.False : AutoTrueFalse.True;
			return result == AutoTrueFalse.True;
		}

		public virtual bool GetUseMultiRenderTargets()
		{
			if( !UseRenderTargets.Value )
				return false;

			var result = UseMultiRenderTargets.Value;
			if( result == AutoTrueFalse.Auto )
				result = SystemSettings.LimitedDevice ? AutoTrueFalse.False : AutoTrueFalse.True;
			return result == AutoTrueFalse.True;
		}

		public virtual bool GetSimpleGeometryAntialiasing()
		{
			var result = SimpleGeometryAntialiasing.Value;
			if( result == AutoTrueFalse.Auto )
				result = SystemSettings.LimitedDevice ? AutoTrueFalse.False : AutoTrueFalse.True;
			return result == AutoTrueFalse.True;
		}
	}
}
