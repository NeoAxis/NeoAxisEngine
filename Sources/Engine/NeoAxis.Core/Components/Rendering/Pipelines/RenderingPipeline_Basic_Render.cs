// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Linq;
using Internal.SharpBgfx;
using System.Threading.Tasks;

namespace NeoAxis
{
	public partial class RenderingPipeline_Basic
	{
		const int sectorCountByDistance = 10;

		// Don't add many non static fields. Rendering pipeline is created for each temporary render target during frame rendering.

		static Uniform? u_reflectionProbeData;
		static Uniform? u_deferredEnvironmentData;
		static Uniform? u_deferredEnvironmentIrradiance;
		//static Uniform? u_environmentTextureRotation;
		//static Uniform? u_environmentTextureIBLRotation;
		//static Uniform? u_environmentTextureMultiplierAndAffect;
		//static Uniform? u_environmentTextureIBLMultiplierAndAffect;

		static Uniform? u_forwardEnvironmentData;
		static Uniform? u_forwardEnvironmentIrradiance1;
		static Uniform? u_forwardEnvironmentIrradiance2;
		//static Uniform? u_environmentTexture1Rotation;
		//static Uniform? u_environmentTexture2Rotation;
		//static Uniform? u_environmentTexture1MultiplierAndAffect;
		//static Uniform? u_environmentTexture2MultiplierAndAffect;
		//static Uniform? u_environmentBlendingFactor;

		static Uniform? u_renderOperationData;
		static RenderOperationDataStructure renderOperationDataCurrent;

		static Uniform? u_viewportCutVolumeSettings;
		static Uniform? u_viewportCutVolumeData;

		static ImageComponent nullShadowTexture2D;
		static ImageComponent nullShadowTextureCube;
		static ImageComponent brdfLUT;

		static ShadowCasterData defaultShadowCasterData;
		static DeferredShadingData deferredShadingData;
		static Mesh decalMesh;

		static OutputInstancingManager outputInstancingManager = new OutputInstancingManager();

		static byte[] prepareMaterialsTempBuffer;

		static bool duringRenderShadows;
		static bool duringRenderSimple3DRenderer;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public delegate void InitLightDataBuffersEventDelegate( RenderingPipeline_Basic sender, ViewportRenderingContext context, LightItem lightItem );
		public event InitLightDataBuffersEventDelegate InitLightDataBuffersEvent;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		class DeferredShadingData
		{
			public PassItem[] passesPerLightWithoutShadows = new PassItem[ 4 ];
			public PassItem[] passesPerLightWithShadows = new PassItem[ 4 ];
			public PassItem[] passesPerLightWithShadowsContactShadows = new PassItem[ 4 ];
			//public PassItem[] passesPerLightWithShadowsLow = new PassItem[ 4 ];
			//public PassItem[] passesPerLightWithShadowsHigh = new PassItem[ 4 ];

			public Mesh clearBackgroundMesh;
			public GpuMaterialPass clearBackgroundPass;

			/////////////////////////////////////

			public class PassItem
			{
				//dispose?
				public GpuProgram vertexProgram;
				public GpuProgram fragmentProgram;
			}

			/////////////////////////////////////

			public void Dispose()
			{
				passesPerLightWithoutShadows = null;
				passesPerLightWithShadows = null;
				passesPerLightWithShadowsContactShadows = null;
				//passesPerLightWithShadowsLow = null;
				//passesPerLightWithShadowsHigh = null;

				//clearBackgroundPass?.Dispose();
				clearBackgroundPass = null;
				clearBackgroundMesh?.Dispose();
				clearBackgroundMesh = null;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a data for rendering frame.
		/// </summary>
		public class FrameData : IFrameData
		{
			RenderSceneData renderSceneData = new RenderSceneData();
			public RenderSceneData RenderSceneData { get { return renderSceneData; } }

			//!!!!can use list container with several arrays inside

			public OpenList<ObjectInSpaceItem> ObjectInSpaces;

			public OpenList<MeshItem> Meshes;
			public OpenList<BillboardItem> Billboards;
			public List<LightItem> Lights;
			public List<ReflectionProbeItem> ReflectionProbes;
			public List<DecalItem> Decals;

			public int[] LightsInFrustumSorted = new int[ 0 ];

			public List<Vector2I> RenderableGroupsInFrustum;

			public Sky Sky;
			public Fog Fog;

			public OpenList<Material.CompiledMaterialData> Materials;
			public ImageComponent MaterialsTexture;

			////////////

			/// <summary>
			/// Represents an object in space data of <see cref="FrameData"/>.
			/// </summary>
			public struct ObjectInSpaceItem
			{
				public ObjectInSpace ObjectInSpace;
				public RangeI MeshRange;
				public RangeI BillboardRange;
				//public RangeI LightRange;
				//public RangeI ReflectionProbeRange;
				//public RangeI DecalRange;

				public bool ContainsData;
				public bool InsideFrustum;
			}

			////////////

			/// <summary>
			/// Represents mesh data of <see cref="FrameData"/>.
			/// </summary>
			public struct MeshItem
			{
				public FlagsEnum Flags;
				public float DistanceToCameraSquared;
				public int PointSpotLightCount;
				public unsafe fixed int PointSpotLightsFixed[ 6 ];
				public List<int> PointSpotLightsMore;

				//

				[Flags]
				public enum FlagsEnum
				{
					//InsideFrustum = 1,
					UseDeferred = 2,
					UseForwardOpaque = 4,
					UseForwardTransparent = 8,
					ContainsForwardOpaqueLayersOnOpaqueBaseObjects = 16,
					ContainsTransparentLayersOnOpaqueBaseObjects = 32,
					ContainsTransparentLayersOnTransparentBaseObjects = 64,
					CalculateAffectedLights = 128,
				}

				//

				public unsafe void AddPointSpotLight( int lightIndex )
				{
					if( PointSpotLightCount < 6 )
					{
						fixed( int* p = PointSpotLightsFixed )
							p[ PointSpotLightCount ] = lightIndex;
					}
					else if( PointSpotLightCount == 6 )
					{
						PointSpotLightsMore = new List<int>();
						PointSpotLightsMore.Add( lightIndex );
					}
					else
						PointSpotLightsMore.Add( lightIndex );
					PointSpotLightCount++;
				}

				public unsafe int GetPointSpotLight( int n )
				{
					if( n < 6 )
					{
						fixed( int* p = PointSpotLightsFixed )
							return p[ n ];
					}
					else
						return PointSpotLightsMore[ n - 6 ];
				}

				public bool ContainsPointOrSpotLight( int lightIndex )
				{
					for( int n = 0; n < PointSpotLightCount; n++ )
						if( GetPointSpotLight( n ) == lightIndex )
							return true;
					return false;
				}

				public unsafe bool CanUseInstancingForTransparentWith( ref MeshItem meshItem )
				{
					//!!!!need?
					if( Flags != meshItem.Flags )
						return false;

					if( PointSpotLightCount != meshItem.PointSpotLightCount )
						return false;

					for( int n = 0; n < Math.Min( PointSpotLightCount, 6 ); n++ )
						if( PointSpotLightsFixed[ n ] != meshItem.PointSpotLightsFixed[ n ] )
							return false;

					if( PointSpotLightsMore != null )
					{
						for( int n = 0; n < PointSpotLightsMore.Count; n++ )
							if( PointSpotLightsMore[ n ] != meshItem.PointSpotLightsMore[ n ] )
								return false;
					}

					return true;
				}
			}

			////////////

			/// <summary>
			/// Represents billboard data of <see cref="FrameData"/>.
			/// </summary>
			public struct BillboardItem
			{
				public FlagsEnum Flags;
				public float DistanceToCameraSquared;
				public int PointSpotLightCount;
				public unsafe fixed int PointSpotLightsFixed[ 6 ];
				public List<int> PointSpotLightsMore;

				//

				[Flags]
				public enum FlagsEnum
				{
					//InsideFrustum = 1,
					UseDeferred = 2,
					UseForwardOpaque = 4,
					UseForwardTransparent = 8,
					CalculateAffectedLights = 16,
				}

				//

				public unsafe void AddPointSpotLight( int lightIndex )
				{
					if( PointSpotLightCount < 6 )
					{
						fixed( int* p = PointSpotLightsFixed )
							p[ PointSpotLightCount ] = lightIndex;
					}
					else if( PointSpotLightCount == 6 )
					{
						PointSpotLightsMore = new List<int>();
						PointSpotLightsMore.Add( lightIndex );
					}
					else
						PointSpotLightsMore.Add( lightIndex );
					PointSpotLightCount++;
				}

				public unsafe int GetPointSpotLight( int n )
				{
					if( n < 6 )
					{
						fixed( int* p = PointSpotLightsFixed )
							return p[ n ];
					}
					else
						return PointSpotLightsMore[ n - 6 ];
				}

				public bool ContainsPointOrSpotLight( int lightIndex )
				{
					for( int n = 0; n < PointSpotLightCount; n++ )
						if( GetPointSpotLight( n ) == lightIndex )
							return true;
					return false;
				}

				public unsafe bool CanUseInstancingForTransparentWith( ref BillboardItem billboardItem )
				{
					//!!!!need?
					if( Flags != billboardItem.Flags )
						return false;

					if( PointSpotLightCount != billboardItem.PointSpotLightCount )
						return false;

					for( int n = 0; n < Math.Min( PointSpotLightCount, 6 ); n++ )
						if( PointSpotLightsFixed[ n ] != billboardItem.PointSpotLightsFixed[ n ] )
							return false;

					if( PointSpotLightsMore != null )
					{
						for( int n = 0; n < PointSpotLightsMore.Count; n++ )
							if( PointSpotLightsMore[ n ] != billboardItem.PointSpotLightsMore[ n ] )
								return false;
					}

					return true;
				}
			}

			////////////

			public FrameData()
			{
				int multiplier = SystemSettings.LimitedDevice ? 1 : 4;

				ObjectInSpaces = new OpenList<ObjectInSpaceItem>( 512 * multiplier );

				Meshes = new OpenList<MeshItem>( 512 * multiplier );
				Billboards = new OpenList<BillboardItem>( 128 * multiplier );
				Lights = new List<LightItem>( 64 * multiplier );
				ReflectionProbes = new List<ReflectionProbeItem>( 32 * multiplier );
				Decals = new List<DecalItem>( 128 * multiplier );

				RenderableGroupsInFrustum = new List<Vector2I>( 512 * multiplier );

				Materials = new OpenList<Material.CompiledMaterialData>( 512 * multiplier );
			}

			void AddItems( ViewportRenderingContext context, int meshes, int meshes2, int billboards, int billboards2, int lights, int lights2, int reflectionProbes, int reflectionProbes2, int decals, int decals2, bool addMaterialsAddOnlySpecialShadowCasters )
			{
				for( int n = meshes; n < meshes2; n++ )
				{
					ref var meshItem = ref RenderSceneData.Meshes.Data[ n ];
					var meshData = meshItem.MeshData;

					var data = new MeshItem();

					//!!!!так?
					//meshItem.BoundingBoxCenter.GetCenter( out var center );
					data.DistanceToCameraSquared = (float)( meshItem.BoundingBoxCenter - context.OwnerCameraSettingsPosition ).LengthSquared();

					//Flags
					bool lit = false;

					//add materials of mesh item
					for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
					{
						var oper = meshData.RenderOperations[ nOperation ];
						if( oper.ContainsDisposedBuffers() )
							continue;

						var materialData = GetMeshMaterialData( ref meshItem, oper, nOperation, false );

						if( materialData.deferredShadingSupport )
						{
							var pipeline = (RenderingPipeline_Basic)context.RenderingPipeline;
							if( pipeline.GetDeferredShading() && pipeline.UseRenderTargets && pipeline.DebugMode.Value == DebugModeEnum.None )
								data.Flags |= MeshItem.FlagsEnum.UseDeferred;
							else
								data.Flags |= MeshItem.FlagsEnum.UseForwardOpaque;
						}
						else
						{
							if( materialData.Transparent )
								data.Flags |= MeshItem.FlagsEnum.UseForwardTransparent;
							else
								data.Flags |= MeshItem.FlagsEnum.UseForwardOpaque;
						}

						if( materialData.ShadingModel != Material.ShadingModelEnum.Unlit )
							lit = true;

						//add material
						if( !addMaterialsAddOnlySpecialShadowCasters || ( addMaterialsAddOnlySpecialShadowCasters && materialData.specialShadowCasterData != null ) )
							AddMaterial( context, materialData );
					}

					//add materials of the layers
					if( meshItem.Layers != null )
					{
						var meshFlags = data.Flags;

						foreach( var layer in meshItem.Layers )
						{
							var materialData = layer.ResultMaterial;//?.Result;
							if( materialData == null )
								continue;
							//if( materialData == null )
							//	materialData = ResourceUtility.MaterialNull.Result;

							//if( materialData != null )
							//{
							if( materialData.deferredShadingSupport )
							{
								var pipeline = (RenderingPipeline_Basic)context.RenderingPipeline;
								if( pipeline.GetDeferredShading() && pipeline.UseRenderTargets && pipeline.DebugMode.Value == DebugModeEnum.None )
									data.Flags |= MeshItem.FlagsEnum.UseDeferred;
								else
								{
									if( meshFlags.HasFlag( MeshItem.FlagsEnum.UseDeferred ) || meshFlags.HasFlag( MeshItem.FlagsEnum.UseForwardOpaque ) )
										data.Flags |= MeshItem.FlagsEnum.UseForwardOpaque | MeshItem.FlagsEnum.ContainsForwardOpaqueLayersOnOpaqueBaseObjects;
								}
							}
							else
							{
								if( materialData.Transparent )
								{
									if( meshFlags.HasFlag( MeshItem.FlagsEnum.UseForwardTransparent ) )
										data.Flags |= MeshItem.FlagsEnum.ContainsTransparentLayersOnTransparentBaseObjects;
									else
										data.Flags |= MeshItem.FlagsEnum.ContainsTransparentLayersOnOpaqueBaseObjects;
								}
								else
								{
									if( meshFlags.HasFlag( MeshItem.FlagsEnum.UseDeferred ) || meshFlags.HasFlag( MeshItem.FlagsEnum.UseForwardOpaque ) )
										data.Flags |= MeshItem.FlagsEnum.UseForwardOpaque | MeshItem.FlagsEnum.ContainsForwardOpaqueLayersOnOpaqueBaseObjects;
								}
							}

							if( materialData.ShadingModel != Material.ShadingModelEnum.Unlit )
								lit = true;

							//add material
							if( !addMaterialsAddOnlySpecialShadowCasters || ( addMaterialsAddOnlySpecialShadowCasters && materialData.specialShadowCasterData != null ) )
								AddMaterial( context, materialData );
							//}
						}
					}

					//CalculateAffectedLights
					if( lit && ( ( data.Flags & MeshItem.FlagsEnum.UseForwardOpaque ) != 0 || ( data.Flags & MeshItem.FlagsEnum.UseForwardTransparent ) != 0 ) )
						data.Flags |= MeshItem.FlagsEnum.CalculateAffectedLights;

					Meshes.Add( ref data );
				}

				for( int n = billboards; n < billboards2; n++ )
				{
					ref var data2 = ref RenderSceneData.Billboards.Data[ n ];
					//!!!!так?
					//data2.BoundingBox.GetCenter( out var center );

					var data = new BillboardItem();
					data.DistanceToCameraSquared = (float)( data2.BoundingBoxCenter - context.OwnerCameraSettingsPosition ).LengthSquared();

					//Flags
					bool lit = false;

					var materialData = GetBillboardMaterialData( ref data2, false );

					if( materialData.deferredShadingSupport )
					{
						var pipeline = (RenderingPipeline_Basic)context.RenderingPipeline;
						if( pipeline.GetDeferredShading() && pipeline.UseRenderTargets && pipeline.DebugMode.Value == DebugModeEnum.None )
							data.Flags |= BillboardItem.FlagsEnum.UseDeferred;
						else
							data.Flags |= BillboardItem.FlagsEnum.UseForwardOpaque;
					}
					else
					{
						if( materialData.Transparent )
							data.Flags |= BillboardItem.FlagsEnum.UseForwardTransparent;
						else
							data.Flags |= BillboardItem.FlagsEnum.UseForwardOpaque;
					}

					if( materialData.ShadingModel != Material.ShadingModelEnum.Unlit )
						lit = true;

					//CalculateAffectedLights
					if( lit && ( ( data.Flags & BillboardItem.FlagsEnum.UseForwardOpaque ) != 0 || ( data.Flags & BillboardItem.FlagsEnum.UseForwardTransparent ) != 0 ) )
						data.Flags |= BillboardItem.FlagsEnum.CalculateAffectedLights;

					Billboards.Add( ref data );

					//add material
					if( !addMaterialsAddOnlySpecialShadowCasters || ( addMaterialsAddOnlySpecialShadowCasters && materialData.specialShadowCasterData != null ) )
						AddMaterial( context, materialData );
				}

				for( int n = lights; n < lights2; n++ )
				{
					ref var data2 = ref renderSceneData.Lights.Data[ n ];
					Lights.Add( new LightItem( data2, context ) );
				}

				for( int n = reflectionProbes; n < reflectionProbes2; n++ )
				{
					ref var data2 = ref renderSceneData.ReflectionProbes.Data[ n ];
					ReflectionProbes.Add( new ReflectionProbeItem( data2, context ) );
				}

				for( int n = decals; n < decals2; n++ )
				{
					ref var data2 = ref renderSceneData.Decals.Data[ n ];

					var materialData = GetDecalMaterialData( ref data2, false );

					Decals.Add( new DecalItem() );

					//add material
					if( !addMaterialsAddOnlySpecialShadowCasters || ( addMaterialsAddOnlySpecialShadowCasters && materialData.specialShadowCasterData != null ) )
						AddMaterial( context, materialData );
				}
			}

			public void SceneGetRenderSceneData( ViewportRenderingContext context, Scene scene )
			{
				int meshes = RenderSceneData.Meshes.Count;
				int billboards = RenderSceneData.Billboards.Count;
				int lights = RenderSceneData.Lights.Count;
				int reflectionProbes = RenderSceneData.ReflectionProbes.Count;
				int decals = RenderSceneData.Decals.Count;

				ComponentsHidePublic.PerformGetRenderSceneData( scene, context );

				int meshes2 = RenderSceneData.Meshes.Count;
				int billboards2 = RenderSceneData.Billboards.Count;
				int lights2 = RenderSceneData.Lights.Count;
				int reflectionProbes2 = RenderSceneData.ReflectionProbes.Count;
				int decals2 = RenderSceneData.Decals.Count;

				AddItems( context, meshes, meshes2, billboards, billboards2, lights, lights2, reflectionProbes, reflectionProbes2, decals, decals2, false );

				//!!!!? what with shadows?
				//for( int n = meshes; n < meshes2; n++ )
				//	RenderableGroupsInFrustum.Add( new Vector2I( 0, n ) );
			}

			public void AddZeroAmbientLight( ViewportRenderingContext context )
			{
				int lights = RenderSceneData.Lights.Count;

				var item = new RenderSceneData.LightItem();
				item.Creator = this;
				item.Type = Light.TypeEnum.Ambient;
				context.FrameData.RenderSceneData.Lights.Add( ref item );

				int lights2 = RenderSceneData.Lights.Count;

				AddItems( context, 0, 0, 0, 0, lights, lights2, 0, 0, 0, 0, false );
			}

			public int RegisterObjectInSpace( ViewportRenderingContext context, ObjectInSpace objectInSpace, GetRenderSceneDataMode mode, Scene.GetObjectsInSpaceItem modeGetObjectsItem )
			{
				if( ComponentsHidePublic.GetRenderSceneIndex( objectInSpace ) == -1 )
				{
					int meshes = RenderSceneData.Meshes.Count;
					int billboards = RenderSceneData.Billboards.Count;
					int lights = RenderSceneData.Lights.Count;
					int reflectionProbes = RenderSceneData.ReflectionProbes.Count;
					int decals = RenderSceneData.Decals.Count;

					ComponentsHidePublic.PerformGetRenderSceneData( objectInSpace, context, mode, modeGetObjectsItem );

					int meshes2 = RenderSceneData.Meshes.Count;
					int billboards2 = RenderSceneData.Billboards.Count;
					int lights2 = RenderSceneData.Lights.Count;
					int reflectionProbes2 = RenderSceneData.ReflectionProbes.Count;
					int decals2 = RenderSceneData.Decals.Count;

					AddItems( context, meshes, meshes2, billboards, billboards2, lights, lights2, reflectionProbes, reflectionProbes2, decals, decals2, mode == GetRenderSceneDataMode.ShadowCasterOutsideFrustum );

					var data = new ObjectInSpaceItem();
					data.ObjectInSpace = objectInSpace;

					if( meshes2 > meshes )
					{
						data.MeshRange = new RangeI( meshes, meshes2 );
						data.ContainsData = true;
					}
					if( billboards2 > billboards )
					{
						data.BillboardRange = new RangeI( billboards, billboards2 );
						data.ContainsData = true;
					}

					if( !data.ContainsData )
						data.ContainsData = lights2 > lights || reflectionProbes2 > reflectionProbes || decals2 > decals;
					//if( lights2 > lights )
					//{
					//	//data.LightRange = new RangeI( lights, lights2 );
					//	data.ContainsData = true;
					//}
					//if( reflectionProbes2 > reflectionProbes )
					//{
					//	//data.ReflectionProbeRange = new RangeI( reflectionProbes, reflectionProbes2 );
					//	data.ContainsData = true;
					//}
					//if( decals2 > decals )
					//{
					//	//data.DecalRange = new RangeI( decals, decals2 );
					//	data.ContainsData = true;
					//}

					data.InsideFrustum = mode == GetRenderSceneDataMode.InsideFrustum;//insideFrustum;

					ObjectInSpaces.Add( ref data );
					ComponentsHidePublic.SetRenderSceneIndex( objectInSpace, ObjectInSpaces.Count - 1 );

					//RenderableGroupsInFrustum
					if( data.ContainsData && data.InsideFrustum )
					{
						for( int n = data.MeshRange.Minimum; n < data.MeshRange.Maximum; n++ )
							RenderableGroupsInFrustum.Add( new Vector2I( 0, n ) );
						for( int n = data.BillboardRange.Minimum; n < data.BillboardRange.Maximum; n++ )
							RenderableGroupsInFrustum.Add( new Vector2I( 1, n ) );
					}
				}

				return ComponentsHidePublic.GetRenderSceneIndex( objectInSpace );
				//objectInSpaceData = ObjectInSpaceData[ objectInSpace._InternalRenderSceneIndex ];
			}

			public float GetObjectGroupDistanceToCameraSquared( ref Vector2I index )
			{
				switch( index.X )
				{
				case 0: return Meshes.Data[ index.Y ].DistanceToCameraSquared;
				case 1: return Billboards.Data[ index.Y ].DistanceToCameraSquared;
				case 2: return Lights[ index.Y ].distanceToCameraSquared;
				case 3: return ReflectionProbes[ index.Y ].distanceToCameraSquared;
				}
				return 0;
			}

			public bool GetObjectGroupBoundingBoxCenter( ref Vector2I index, out Vector3 center )
			{
				switch( index.X )
				{
				case 0: center = renderSceneData.Meshes.Data[ index.Y ].BoundingBoxCenter; return true;
				case 1: center = renderSceneData.Billboards.Data[ index.Y ].BoundingBoxCenter; return true;
					//case 2: center = renderSceneData.Lights[ index.Y ].
					//case 3: center = renderSceneData.ReflectionProbes[ index.Y ].distanceToCamera;
				}
				center = Vector3.Zero;
				return false;
			}

			public float GetObjectGroupDistanceToPointSquared( ref Vector2I index, ref Vector3 point )
			{
				switch( index.X )
				{
				case 0:
					{
						ref var data = ref RenderSceneData.Meshes.Data[ index.Y ];
						//data.BoundingBox.GetCenter( out var center );
						return (float)( data.BoundingBoxCenter - point ).LengthSquared();
					}
				case 1:
					{
						ref var data = ref RenderSceneData.Billboards.Data[ index.Y ];
						//data.BoundingBox.GetCenter( out var center );
						return (float)( data.BoundingBoxCenter - point ).LengthSquared();
					}
				case 2:
					return (float)( Lights[ index.Y ].data.Position - point ).LengthSquared();
				case 3:
					{
						ref var data = ref RenderSceneData.ReflectionProbes.Data[ index.Y ];
						data.BoundingBox.GetCenter( out var center );
						return (float)( center - point ).LengthSquared();
					}
					//return ReflectionProbes[ index.Y ].distanceToCamera;

				}

				return 0;
			}

			public void AddMaterial( ViewportRenderingContext context, Material.CompiledMaterialData materialData )
			{
				if( materialData == null )
					return;

				if( materialData.currentFrameIndex == -1 )
				{
					materialData.currentFrameIndex = Materials.Count;
					materialData.PrepareCurrentFrameData( context, false );
					if( materialData.specialShadowCasterData != null )
						materialData.PrepareCurrentFrameData( context, true );

					Materials.Add( materialData );
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents a calculated data of a light during scene rendering.
		/// </summary>
		public class LightItem
		{
			public RenderSceneData.LightItem data;

			//precalculated data
			public float distanceToCameraSquared;

			//shadows
			public bool prepareShadows;
			public ImageComponent shadowTexture;
			public (Matrix4F, Matrix4F)[] shadowCascadesProjectionViewMatrices;

			public OpenListNative<Vector2I>[] shadowsAffectedRenderableGroups;
			//public List<Vector2I>[] shadowsAffectedRenderableGroups;

			//data for GPU
			public LightDataVertex lightDataVertex;
			public LightDataFragment lightDataFragment;
			public bool lightDataBuffersInitialized;

			//uniforms
			public static Uniform? u_lightDataVertex;
			public static Uniform? u_lightDataFragment;

			/////////////////////////////////////

			//!!!!need?
			/// <summary>
			/// Represents data of light for a vertex shader.
			/// </summary>
			[StructLayout( LayoutKind.Sequential )]
			public struct LightDataVertex
			{
				public Vector4F lightPosition;

				////!!!!теперь не надо. делается в пиксельном шейдере
				////!!!!если нет теней, то не надо
				//public Vec4F lightShadowTextureViewProjMatrix0_0;
				//public Vec4F lightShadowTextureViewProjMatrix0_1;
				//public Vec4F lightShadowTextureViewProjMatrix0_2;
				//public Vec4F lightShadowTextureViewProjMatrix0_3;
			}

			/////////////////////////////////////

			/// <summary>
			/// Represents data of light for a fragment shader.
			/// </summary>
			[StructLayout( LayoutKind.Sequential )]
			public struct LightDataFragment
			{
				//general parameters
				public Vector4F lightPosition;
				public Vector4F/*Vec3F*/ lightDirection;
				public Vector4F/*Vec3F*/ lightPower;
				public Vector4F lightAttenuation;
				public Vector4F lightSpot;

				//shadows
				public float lightShadowIntensity;
				public float lightShadowTextureSize;
				public float lightShadowMapFarClipDistance;
				public float lightShadowCascadesVisualize;
				public Matrix4F lightShadowTextureViewProjMatrix0;
				public Matrix4F lightShadowTextureViewProjMatrix1;
				public Matrix4F lightShadowTextureViewProjMatrix2;
				public Matrix4F lightShadowTextureViewProjMatrix3;
				public Vector4F lightShadowCascades;

				//mask
				public Matrix4F lightMaskMatrix;

				public Vector4F unitDistanceWorldShadowTexelSizes;

				public float lightShadowBias;
				public float lightShadowNormalBias;
				public float lightSourceRadiusOrAngle;
				public float lightShadowContactLength;

				//

				public void SetLightShadowTextureViewProjMatrix( int index, ref Matrix4F value )
				{
					switch( index )
					{
					case 0: lightShadowTextureViewProjMatrix0 = value; break;
					case 1: lightShadowTextureViewProjMatrix1 = value; break;
					case 2: lightShadowTextureViewProjMatrix2 = value; break;
					case 3: lightShadowTextureViewProjMatrix3 = value; break;
					}
				}
			}

			/////////////////////////////////////

			public LightItem( RenderSceneData.LightItem data, ViewportRenderingContext context )
			{
				this.data = data;

				if( data.Type == Light.TypeEnum.Directional )
					distanceToCameraSquared = 10000000000.0f;
				else //if( data.transform != null )
					distanceToCameraSquared = (float)( context.OwnerCameraSettingsPosition - data.Position ).LengthSquared();
			}

			//!!!!
			static Matrix4 MakeProjectionMatrixForSpotlight( Radian outerAngle )
			{
				//?
				double mNearDist = .1f;
				double mFarDist = 100;

				// Common calcs
				double left, right, bottom, top;
				{
					Radian thetaY = outerAngle * 0.5;
					double tanThetaY = Math.Tan( thetaY );
					double tanThetaX = tanThetaY * 1;// mAspect;

					double nearFocal = mNearDist;// / mFocalLength;

					//double nearOffsetX = 0;// mFrustumOffset.x* nearFocal;
					//double nearOffsetY = 0;// mFrustumOffset.y* nearFocal;

					double half_w = tanThetaX * mNearDist;
					double half_h = tanThetaY * mNearDist;

					left = -half_w;// +nearOffsetX;
					right = +half_w;// +nearOffsetX;
					bottom = -half_h;// +nearOffsetY;
					top = +half_h;// +nearOffsetY;
				}

				// The code below will dealing with general projection 
				// parameters, similar glFrustum and glOrtho.
				// Doesn't optimise manually except division operator, so the 
				// code more self-explaining.

				double inv_w = 1.0f / ( right - left );
				double inv_h = 1.0f / ( top - bottom );
				double inv_d = 1.0f / ( mFarDist - mNearDist );

				// Calc matrix elements
				double A = 2.0f * mNearDist * inv_w;
				double B = 2.0f * mNearDist * inv_h;
				double C = ( right + left ) * inv_w;
				double D = ( top + bottom ) * inv_h;

				double q = -( mFarDist + mNearDist ) * inv_d;
				double qn = -2.0f * ( mFarDist * mNearDist ) * inv_d;

				// NB: This creates 'uniform' perspective projection matrix,
				// which depth range [-1,1], right-handed rules
				//
				// [ A   0   C   0  ]
				// [ 0   B   D   0  ]
				// [ 0   0   q   qn ]
				// [ 0   0   -1  0  ]
				//
				// A = 2 * near / (right - left)
				// B = 2 * near / (top - bottom)
				// C = (right + left) / (right - left)
				// D = (top + bottom) / (top - bottom)
				// q = - (far + near) / (far - near)
				// qn = - 2 * (far * near) / (far - near)

				Matrix4 result = Matrix4.Zero;
				result[ 0, 0 ] = A;
				result[ 2, 0 ] = C;
				result[ 1, 1 ] = B;
				result[ 2, 1 ] = D;
				result[ 2, 2 ] = q;
				result[ 3, 2 ] = qn;
				result[ 2, 3 ] = -1;

				return result;
			}

			static Matrix4 MakeViewMatrixForSpotlight( Vector3 position, QuaternionF rotation )
			{
				// View matrix is:
				//
				//  [ Lx  Uy  Dz  Tx  ]
				//  [ Lx  Uy  Dz  Ty  ]
				//  [ Lx  Uy  Dz  Tz  ]
				//  [ 0   0   0   1   ]
				//
				// Where T = -(Transposed(Rot) * Pos)

				Matrix3 rotationMatrix = rotation.ToMatrix3();
				rotationMatrix *= Matrix3.FromRotateByY( MathEx.PI / 2 );

				// Make the translation relative to new axes
				Matrix3 rotationMatrixT = rotationMatrix.GetTranspose();
				Vector3 trans = -( rotationMatrixT ) * position;

				return new Matrix4( rotationMatrixT, trans );
			}

			void InitLightDataBuffers( RenderingPipeline_Basic pipeline, ViewportRenderingContext context )//, double shadowIntensity )
			{
				//!!!!double

				//!!!!!кешировать и раньше рассчитывать. в RenderSceneData по идее

				Vector3F direction = Vector3F.XAxis;
				if( data.Type != Light.TypeEnum.Ambient )
					direction = data.Rotation.GetForward();

				Vector4 position = Vector4.Zero;
				if( data.Type != Light.TypeEnum.Ambient )
				{
					if( data.Type == Light.TypeEnum.Directional )
						position = new Vector4( -direction, 0 );
					else
						position = new Vector4( data.Position, 1 );
				}
				lightDataVertex.lightPosition = position.ToVector4F();
				lightDataFragment.lightPosition = position.ToVector4F();
				lightDataFragment.lightDirection = new Vector4F( direction, 0 );
				lightDataFragment.lightPower = new Vector4F( data.Power, 0 );

				var near = data.AttenuationNear;
				var far = data.AttenuationFar;
				near = MathEx.Clamp( near, 0, far - MathEx.Epsilon );
				var power = MathEx.Clamp( data.AttenuationPower, MathEx.Epsilon, data.AttenuationPower );
				lightDataFragment.lightAttenuation = new Vector4( near, far, power, far - near ).ToVector4F();

				Vector4 spot;
				if( data.Type == Light.TypeEnum.Spotlight )
				{
					double inner = data.SpotlightInnerAngle.InRadians();
					double outer = data.SpotlightOuterAngle.InRadians();
					outer = MathEx.Clamp( outer, .01, Math.PI );
					inner = MathEx.Clamp( inner, 0, outer - .0001 );

					spot = new Vector4(
						Math.Cos( inner * .5 ),
						Math.Cos( outer * .5 ),
						MathEx.Clamp( data.SpotlightFalloff, MathEx.Epsilon, 1 ),
						1 );
				}
				else
					spot = new Vector4( 1, 0, 0, 1 );
				lightDataFragment.lightSpot = spot.ToVector4F();

				//shadows
				if( shadowTexture != null )
				{
					Viewport shadowViewport = shadowTexture.Result.GetRenderTarget().Viewports[ 0 ];

					lightDataFragment.lightShadowIntensity = MathEx.Saturate( (float)( pipeline.ShadowIntensity.Value * data.ShadowIntensity ) );
					lightDataFragment.lightShadowTextureSize = shadowTexture.CreateSize.Value.X;
					lightDataFragment.lightShadowMapFarClipDistance = (float)shadowViewport.CameraSettings.FarClipDistance;
					lightDataFragment.lightShadowCascadesVisualize = pipeline.ShadowDirectionalLightCascadeVisualize ? 1 : -1;

					//lightShadowTextureViewProjMatrix
					if( data.Type == Light.TypeEnum.Spotlight || data.Type == Light.TypeEnum.Directional )
					{
						//PC: OriginBottomLeft = false, HomogeneousDepth = false
						//Android: OriginBottomLeft = true, HomogeneousDepth = true
						var caps = RenderingSystem.Capabilities;
						float sy = caps.OriginBottomLeft ? 0.5f : -0.5f;
						float sz = /*caps.HomogeneousDepth ? 0.5f :*/ 1.0f;
						float tz = /*caps.HomogeneousDepth ? 0.5f :*/ 0.0f;

						var toImageSpace = new Matrix4F(
							0.5f, 0, 0, 0,
							0, sy, 0, 0,
							0, 0, sz, 0,
							0.5f, 0.5f, tz, 1 );

						if( data.Type == Light.TypeEnum.Directional )
						{
							for( int n = 0; n < shadowCascadesProjectionViewMatrices.Length; n++ )
							{
								ref var item = ref shadowCascadesProjectionViewMatrices[ n ];
								ref Matrix4F shadowProjectionMatrix = ref item.Item1;
								ref Matrix4F shadowViewMatrix = ref item.Item2;

								var mat = toImageSpace * shadowProjectionMatrix * shadowViewMatrix;
								mat.Transpose();

								lightDataFragment.SetLightShadowTextureViewProjMatrix( n, ref mat );
								lightDataFragment.unitDistanceWorldShadowTexelSizes[ n ] =
									1.414213562f // sqrt(2), texel diagonal (maximum) size
									* ( 2.0f / shadowProjectionMatrix[ 0 ][ 0 ] ) // r - l
									/ lightDataFragment.lightShadowTextureSize; // how many texels in shadow map side

							}
						}
						else // Spot light
						{
							//!!!!double
							Matrix4F shadowViewMatrix = shadowViewport.CameraSettings.ViewMatrix.ToMatrix4F();
							Matrix4F shadowProjectionMatrix = shadowViewport.CameraSettings.ProjectionMatrix.ToMatrix4F();

							var mat = toImageSpace * shadowProjectionMatrix * shadowViewMatrix;
							mat.Transpose();

							lightDataFragment.lightShadowTextureViewProjMatrix0 = mat;
							lightDataFragment.unitDistanceWorldShadowTexelSizes[ 0 ] =
								1.414213562f // sqrt(2), texel diagonal (maximum) size
								* ( 2.0f / shadowProjectionMatrix[ 0 ][ 0 ] ) // (r - l) / (near), world X texel size at unit distance
								/ lightDataFragment.lightShadowTextureSize; // how many texels in shadow map side
						}
					}

					if( data.Type == Light.TypeEnum.Point )
					{
						Matrix4F shadowProjectionMatrix = shadowViewport.CameraSettings.ProjectionMatrix.ToMatrix4F();

						lightDataFragment.unitDistanceWorldShadowTexelSizes[ 0 ] =
							1.414213562f // sqrt(2), texel diagonal (maximum) size
							* ( 2.0f / shadowProjectionMatrix[ 0 ][ 0 ] ) // (r - l) / (near), world X texel size at unit distance
							/ lightDataFragment.lightShadowTextureSize; // how many texels in shadow map side
					}

					//lightShadowCascades
					if( data.Type == Light.TypeEnum.Directional )
					{
						lightDataFragment.lightShadowCascades = Vector4F.Zero;
						lightDataFragment.lightShadowCascades[ 0 ] = pipeline.ShadowDirectionalLightCascades;
						var splitDistances = pipeline.GetShadowCascadeSplitDistances( context );

						if( pipeline.ShadowDirectionalLightCascades >= 2 )
							lightDataFragment.lightShadowCascades[ 1 ] = (float)splitDistances[ 1 ];
						else
							lightDataFragment.lightShadowCascades[ 1 ] = (float)pipeline.ShadowFarDistance;

						if( pipeline.ShadowDirectionalLightCascades >= 3 )
							lightDataFragment.lightShadowCascades[ 2 ] = (float)splitDistances[ 2 ];
						else
							lightDataFragment.lightShadowCascades[ 2 ] = (float)pipeline.ShadowFarDistance;

						if( pipeline.ShadowDirectionalLightCascades >= 4 )
							lightDataFragment.lightShadowCascades[ 3 ] = (float)splitDistances[ 3 ];
						else
							lightDataFragment.lightShadowCascades[ 3 ] = (float)pipeline.ShadowFarDistance;
					}
				}

				//mask
				if( RenderingSystem.LightMask )
				{
					if( data.Mask != null )
					{
						//!!!!если .Zero

						Matrix4F mat = Matrix4F.Zero;

						switch( data.Type )
						{
						case Light.TypeEnum.Directional:
							{
								//!!!!double

								Vector3F dir = -data.Rotation.GetForward();
								double s = 0;
								if( data.MaskScale != 0 )
									s = 1.0 / data.MaskScale;
								Matrix4 m = new Matrix4(
									s, 0, dir.X * s, -data.Position.X * s,
									0, s, dir.Y * s, -data.Position.Y * s,
									0, 0, 1, 0,
									0, 0, 0, 1 );
								mat = m.GetTranspose().ToMatrix4F();

								//var m = new Mat4( Mat3.FromScale( data.maskScale ), data.maskPosition );
								//mat = m.ToMat4F();

								//Vec2 scale = Vec2.Zero;
								//if( data.maskScale.X != 0 )
								//	scale.X = 1.0 / data.maskScale.X;
								//if( data.maskScale.Y != 0 )
								//	scale.Y = 1.0 / data.maskScale.Y;
								//var m = new Mat4( Mat3.FromScale( new Vec3( scale, 1 ) ), new Vec3( data.maskPosition, 0 ) );
								//mat = m.ToMat4F();
							}
							break;

						case Light.TypeEnum.Point:
							{
								Matrix3 m = data.Rotation.ToMatrix3().GetTranspose();
								//!!!!fix flipped cubemaps
								//dir = double3( -dir.y, dir.z, dir.x );
								Matrix3 flipMatrix = new Matrix3(
									0, -1, 0,
									0, 0, 1,
									1, 0, 0 );
								//!!!!double
								mat = ( flipMatrix.GetTranspose() * m ).ToMatrix4().ToMatrix4F();
							}
							break;

						case Light.TypeEnum.Spotlight:
							{
								Matrix4 projectionClipSpace2DToImageSpace = new Matrix4(
									0.5f, 0, 0, 0,
									0, -0.5f, 0, 0,
									0, 0, 1, 0,
									0.5f, 0.5f, 0, 1 );
								Matrix4 projectionMatrix = MakeProjectionMatrixForSpotlight( data.SpotlightOuterAngle.InRadians() );
								Matrix4 viewMatrix = MakeViewMatrixForSpotlight( data.Position, data.Rotation );
								//!!!!double
								//!!!!не наоборот projectionMatrix * viewMatrix?
								mat = ( projectionClipSpace2DToImageSpace * projectionMatrix * viewMatrix ).ToMatrix4F();
							}
							break;
						}

						mat.Transpose();
						lightDataFragment.lightMaskMatrix = mat;
					}
					else
						lightDataFragment.lightMaskMatrix = Matrix4F.Zero;
				}

				lightDataFragment.lightShadowBias = data.ShadowBias;
				lightDataFragment.lightShadowNormalBias = data.ShadowNormalBias;
				lightDataFragment.lightSourceRadiusOrAngle = data.SourceRadiusOrAngle;
				lightDataFragment.lightShadowContactLength = data.ShadowContactLength;
				//lightDataFragment.lightShadowQuality = pipeline.ShadowQuality.Value == ShadowQualityEnum.High ? 1 : 0;

				pipeline.InitLightDataBuffersEvent?.Invoke( pipeline, context, this );
			}

			public unsafe void Bind( RenderingPipeline_Basic pipeline, ViewportRenderingContext context )
			{
				//init
				if( !lightDataBuffersInitialized )
				{
					InitLightDataBuffers( pipeline, context );
					lightDataBuffersInitialized = true;
				}

				//set vertex data uniform
				{
					int vec4Count = sizeof( LightDataVertex ) / sizeof( Vector4F );
					if( vec4Count != 1 )
						Log.Fatal( "ViewportRenderingContext: SetUniforms: vec4Count != 1." );
					if( u_lightDataVertex == null )
						u_lightDataVertex = GpuProgramManager.RegisterUniform( "u_lightDataVertex", UniformType.Vector4, vec4Count );
					fixed( LightDataVertex* p = &lightDataVertex )
						Bgfx.SetUniform( u_lightDataVertex.Value, p, vec4Count );
				}

				//set fragment data uniform
				{
					int vec4Count = sizeof( LightDataFragment ) / sizeof( Vector4F );
					if( vec4Count != 29 )
						Log.Fatal( "ViewportRenderingContext: SetUniforms: vec4Count != 29." );
					if( u_lightDataFragment == null )
						u_lightDataFragment = GpuProgramManager.RegisterUniform( "u_lightDataFragment", UniformType.Vector4, vec4Count );
					fixed( LightDataFragment* p = &lightDataFragment )
						Bgfx.SetUniform( u_lightDataFragment.Value, p, vec4Count );
				}
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//!!!!struct
		/// <summary>
		/// Represents a calculated data of a reflection probe during scene rendering.
		/// </summary>
		public class ReflectionProbeItem
		{
			public RenderSceneData.ReflectionProbeItem data;
			public float distanceToCameraSquared;

			////uniforms
			////public static Uniform reflectionProbeDataVertexUniform;
			//public static Uniform reflectionProbeDataFragmentUniform;
			//public bool reflectionProbeDataBuffersInitialized;

			///////////////////////////////////////

			////reflectionProbeDataVertex
			////[StructLayout(LayoutKind.Sequential)]
			////public struct ReflectionProbeDataVertex
			////{
			////    public Vec4F reflectionProbePosition;
			////    public float reflectionProbeRadius;
			////    public float unused1;
			////    public float unused2;
			////    public float unused3;
			////}
			////public ReflectionProbeDataVertex reflectionProbeDataVertex;
			////public ReflectionProbeDataVertex reflectionProbeDataVertex;

			///////////////////////////////////////

			////reflectionProbeDataFragment
			//[StructLayout( LayoutKind.Sequential )]
			//public struct ReflectionProbeDataFragment
			//{
			//	//!!!!
			//	public Vector4F reflectionProbePosition;
			//	public float reflectionProbeRadius;
			//	public float unused1;
			//	public float unused2;
			//	public float unused3;
			//}
			//public ReflectionProbeDataFragment reflectionProbeDataFragment;

			///////////////////////////////////////

			public ReflectionProbeItem( RenderSceneData.ReflectionProbeItem data, ViewportRenderingContext context )
			{
				this.data = data;
				distanceToCameraSquared = (float)( context.OwnerCameraSettingsPosition - data.Sphere.Center ).LengthSquared();

				if( data.Sphere.Radius <= 0 )
					Log.Fatal( "RenderingPipeline_Basic: ReflectionProbeItem: Constructor: data.Sphere.Radius <= 0." );
			}

			//void InitReflectionProbeDataBuffers( RenderingPipeline_Basic pipeline, ViewportRenderingContext context )
			//{
			//	double radius = data.Sphere.Radius;
			//	var position = new Vector4( data.Sphere.Origin, 1 );

			//	reflectionProbeDataFragment.reflectionProbePosition = position.ToVector4F();
			//	reflectionProbeDataFragment.reflectionProbeRadius = (float)radius;
			//}

			//public unsafe void Bind( RenderingPipeline_Basic pipeline, ViewportRenderingContext context )
			//{
			//	//init
			//	if( !reflectionProbeDataBuffersInitialized )
			//	{
			//		InitReflectionProbeDataBuffers( pipeline, context );
			//		reflectionProbeDataBuffersInitialized = true;
			//	}

			//	// set vertex data uniform
			//	// maybe we'll need it later
			//	// {
			//	//    int vec4Count = sizeof(ReflectionProbeDataVertex) / sizeof(Vec4F);
			//	//    if (vec4Count != 2)
			//	//        Log.Fatal("ViewportRenderingContext: SetUniforms: vec4Count != 2.");
			//	//    if (reflectionProbeDataVertexUniform == Uniform.Invalid)
			//	//        reflectionProbeDataVertexUniform = GpuProgramManager.RegisterUniform("u_reflectionProbeDataVertex", UniformType.Vector4, vec4Count);
			//	//    fixed (ReflectionProbeDataVertex* p = &reflectionProbeDataVertex)
			//	//        Bgfx.SetUniform(reflectionProbeDataVertexUniform, p, vec4Count);
			//	// }

			//	//set fragment data uniform
			//	{
			//		int vec4Count = sizeof( ReflectionProbeDataFragment ) / sizeof( Vector4F );
			//		if( vec4Count != 2 )
			//			Log.Fatal( "ViewportRenderingContext: SetUniforms: vec4Count != 2." );
			//		if( reflectionProbeDataFragmentUniform == Uniform.Invalid )
			//			reflectionProbeDataFragmentUniform = GpuProgramManager.RegisterUniform( "u_reflectionProbeDataFragment", UniformType.Vector4, vec4Count );
			//		fixed ( ReflectionProbeDataFragment* p = &reflectionProbeDataFragment )
			//			Bgfx.SetUniform( reflectionProbeDataFragmentUniform, p, vec4Count );
			//	}
			//}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents calculated data of a decal during scene rendering.
		/// </summary>
		public struct DecalItem
		{
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void InitDefaultShadowCasterData()
		{
			if( defaultShadowCasterData == null )
			{
				EngineThreading.CheckMainThread();

				defaultShadowCasterData = new ShadowCasterData();
				var data = defaultShadowCasterData;

				data.passByLightType = new GpuMaterialPassGroup[ 4 ];

				foreach( Light.TypeEnum lightType in Enum.GetValues( typeof( Light.TypeEnum ) ) )
				{
					if( lightType == Light.TypeEnum.Ambient )
						continue;

					for( int nPassType = 0; nPassType < 2; nPassType++ )
					{
						var billboardPass = nPassType == 1;

						//generate compile arguments
						var vertexDefines = new List<(string, string)>();
						var fragmentDefines = new List<(string, string)>();
						{
							var generalDefines = new List<(string, string)>();
							generalDefines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );
							if( billboardPass )
								generalDefines.Add( ("BILLBOARD", "") );

							vertexDefines.AddRange( generalDefines );
							fragmentDefines.AddRange( generalDefines );
						}

						string error2;

						//vertex program
						var vertexProgram = GpuProgramManager.GetProgram( "ShadowCasterDefault_Vertex_", GpuProgramType.Vertex,
							@"Base\Shaders\ShadowCasterDefault_vs.sc", vertexDefines, true, out error2 );
						if( !string.IsNullOrEmpty( error2 ) )
							Log.Fatal( error2 );

						//fragment program
						var fragmentProgram = GpuProgramManager.GetProgram( "ShadowCasterDefault_Fragment_", GpuProgramType.Fragment,
							@"Base\Shaders\ShadowCasterDefault_fs.sc", fragmentDefines, true, out error2 );
						if( !string.IsNullOrEmpty( error2 ) )
							Log.Fatal( error2 );

						var pass = new GpuMaterialPass( vertexProgram, fragmentProgram );
						data.passByLightType[ (int)lightType ].Set( pass, billboardPass );

						//!!!!
						//pass.CullingMode = CullingMode.None;
					}
				}
			}
		}

		unsafe static bool Intersects( Plane* frustumPlanes, Vector3* points, int pointCount )
		//unsafe static bool Intersects( Plane[] frustumPlanes, Vector3* points, int pointCount )
		{
			//foreach( var plane in frustumPlanes )
			for( int nPlane = 0; nPlane < 6; nPlane++ )
			{
				bool allClipped = true;
				for( int n = 0; n < pointCount; n++ )//foreach( var p in points )
				{
					if( frustumPlanes[ nPlane ].GetSide( ref points[ n ] ) == Plane.Side.Negative )
					{
						allClipped = false;
						break;
					}
				}
				if( allClipped )
					return false;
			}

			return true;
		}

		/////////////////////////////////////////

		struct ShadowsObjectData
		{
			public Vector3 position;
			public Sphere boundingSphere;
			public int passed;
		}

		/////////////////////////////////////////

		struct LightAffectedObjectsItem
		{
			public Scene.GetObjectsInSpaceItem item0;
			public Scene.GetObjectsInSpaceItem item1;
			public Scene.GetObjectsInSpaceItem item2;
			public Scene.GetObjectsInSpaceItem item3;

			public Scene.GetObjectsInSpaceItem GetItem( int index )
			{
				switch( index )
				{
				case 0: return item0;
				case 1: return item1;
				case 2: return item2;
				case 3: return item3;
				}
				return null;
			}

			public void SetItem( int index, Scene.GetObjectsInSpaceItem value )
			{
				switch( index )
				{
				case 0: item0 = value; break;
				case 1: item1 = value; break;
				case 2: item2 = value; break;
				case 3: item3 = value; break;
				}
			}

			//public Scene.GetObjectsInSpaceItem.ResultItem[] result0;
			//public Scene.GetObjectsInSpaceItem.ResultItem[] result1;
			//public Scene.GetObjectsInSpaceItem.ResultItem[] result2;
			//public Scene.GetObjectsInSpaceItem.ResultItem[] result3;

			//public Scene.GetObjectsInSpaceItem.ResultItem[] GetResult( int index )
			//{
			//	switch( index )
			//	{
			//	case 0: return result0;
			//	case 1: return result1;
			//	case 2: return result2;
			//	case 3: return result3;
			//	}
			//	return null;
			//}

			//public void SetResult( int index, Scene.GetObjectsInSpaceItem.ResultItem[] value )
			//{
			//	switch( index )
			//	{
			//	case 0: result0 = value; break;
			//	case 1: result1 = value; break;
			//	case 2: result2 = value; break;
			//	case 3: result3 = value; break;
			//	}
			//}
		}

		/////////////////////////////////////////

		class OutputInstancingManager
		{
			int instancingMaxCount;
			Stack<IntPtr> renderableItemArrays;

			int[] notCompletedOutputItemsTable = new int[ 0 ];
			OpenList<int> notCompletedOutputItemsTableToClear;

			OpenList<Item> items;
			OpenList<RenderSceneData.MeshDataRenderOperation> operations;
			OpenList<Material.CompiledMaterialData> materialDatas;

			public OpenList<OutputItem> outputItems;

			////////////

			struct Item
			{
				public Vector2I renderableGroup;
				public int operationIndex;
				public RenderSceneData.MeshDataRenderOperation operation;
				public Material.CompiledMaterialData materialData;
				public bool allowInstancing;
			}

			////////////

			public struct OutputItem
			{
				public RenderSceneData.MeshDataRenderOperation operation;
				public Material.CompiledMaterialData materialData;
				public bool allowInstancing;

				public Vector3I renderableItemFirst;
				public unsafe Vector3I* renderableItems;
				public int renderableItemsCount;
			}

			////////////

			public OutputInstancingManager()
			{
				int multiplier = SystemSettings.LimitedDevice ? 1 : 4;

				renderableItemArrays = new Stack<IntPtr>( 16 * multiplier );
				notCompletedOutputItemsTableToClear = new OpenList<int>( 32 * multiplier );
				items = new OpenList<Item>( 128 * multiplier );
				operations = new OpenList<RenderSceneData.MeshDataRenderOperation>( 64 * multiplier );
				materialDatas = new OpenList<Material.CompiledMaterialData>( 32 * multiplier );
				outputItems = new OpenList<OutputItem>( 128 * multiplier );
			}

			public void Init( RenderingPipeline pipeline )
			{
				//drop renderableItemArrays when InstancingMaxCount was changed
				if( instancingMaxCount != pipeline.InstancingMaxCount )
				{
					while( renderableItemArrays.Count != 0 )
						NativeUtility.Free( renderableItemArrays.Pop() );
				}

				instancingMaxCount = pipeline.InstancingMaxCount;
			}

			public unsafe void Add( Vector2I renderableGroup, int operationIndex, RenderSceneData.MeshDataRenderOperation operation, Material.CompiledMaterialData materialData, bool allowInstancing )
			{
				if( materialData == null )
					Log.Fatal( "RenderingPipeline_Basic: OutputInstancingManager: Add: materialData == null." );

				var item = new Item();
				item.renderableGroup = renderableGroup;
				item.operationIndex = operationIndex;
				item.operation = operation;
				item.materialData = materialData;
				item.allowInstancing = allowInstancing;
				items.Add( ref item );

				if( operation._currentRenderingFrameIndex == -1 )
				{
					operation._currentRenderingFrameIndex = operations.Count;
					operations.Add( operation );
				}
				if( materialData._currentRenderingFrameIndex == -1 )
				{
					materialData._currentRenderingFrameIndex = materialDatas.Count;
					materialDatas.Add( materialData );
				}


				//if( allowInstancing )
				//{
				//	//!!!!slowly?

				//	for( int nOutputItem = outputItems.Count - 1; nOutputItem >= 0; nOutputItem-- )
				//	{
				//		ref var outputItem = ref outputItems.Data[ nOutputItem ];

				//		if( outputItem.allowInstancing && outputItem.operation == operation && outputItem.materialData == materialData )
				//		{
				//			//last is full
				//			if( outputItem.renderableItemsCount == instancingMaxCount )
				//				break;

				//			//add to exist list
				//			if( outputItem.renderableItemsCount == 1 )
				//			{
				//				if( renderableItemArrays.Count != 0 )
				//					outputItem.renderableItems = (Vector3I*)renderableItemArrays.Pop();
				//				else
				//					outputItem.renderableItems = (Vector3I*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, sizeof( Vector3I ) * instancingMaxCount );
				//				outputItem.renderableItems[ 0 ] = outputItem.renderableItemFirst;
				//			}
				//			outputItem.renderableItems[ outputItem.renderableItemsCount++ ] = new Vector3I( renderableGroup, operationIndex );
				//			return;
				//		}
				//	}
				//}

				////create new
				//{
				//	var outputItem = new OutputItem();
				//	outputItem.operation = operation;
				//	outputItem.materialData = materialData;
				//	outputItem.allowInstancing = allowInstancing;
				//	outputItem.renderableItemFirst = new Vector3I( renderableGroup, operationIndex );
				//	outputItem.renderableItemsCount = 1;
				//	outputItems.Add( ref outputItem );
				//}
			}

			public unsafe void Prepare()
			{
				//init table
				int tableSize = operations.Count * materialDatas.Count;
				if( notCompletedOutputItemsTable.Length < tableSize )
				{
					notCompletedOutputItemsTable = new int[ tableSize ];
					for( int n = 0; n < notCompletedOutputItemsTable.Length; n++ )
						notCompletedOutputItemsTable[ n ] = -1;
				}

				//process items
				for( int nItem = 0; nItem < items.Count; nItem++ )
				{
					ref var item = ref items.Data[ nItem ];

					if( item.allowInstancing )
					{
						//with instancing

						int tableIndex = item.operation._currentRenderingFrameIndex * materialDatas.Count + item.materialData._currentRenderingFrameIndex;
						var notCompletedItemIndex = notCompletedOutputItemsTable[ tableIndex ];

						if( notCompletedItemIndex != -1 )
						{
							//found not completed item
							ref var outputItem = ref outputItems.Data[ notCompletedItemIndex ];

							//add
							if( outputItem.renderableItemsCount == 1 )
							{
								if( renderableItemArrays.Count != 0 )
									outputItem.renderableItems = (Vector3I*)renderableItemArrays.Pop();
								else
									outputItem.renderableItems = (Vector3I*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, sizeof( Vector3I ) * instancingMaxCount );
								outputItem.renderableItems[ 0 ] = outputItem.renderableItemFirst;
							}
							outputItem.renderableItems[ outputItem.renderableItemsCount++ ] = new Vector3I( item.renderableGroup, item.operationIndex );

							//check now is completed
							if( outputItem.renderableItemsCount == instancingMaxCount )
							{
								//remove from the table
								notCompletedOutputItemsTable[ tableIndex ] = -1;
							}
						}
						else
						{
							//create new output item
							var outputItem = new OutputItem();
							outputItem.operation = item.operation;
							outputItem.materialData = item.materialData;
							outputItem.allowInstancing = true;
							outputItem.renderableItemFirst = new Vector3I( item.renderableGroup, item.operationIndex );
							outputItem.renderableItemsCount = 1;
							outputItems.Add( ref outputItem );

							//add to the table
							notCompletedOutputItemsTable[ tableIndex ] = outputItems.Count - 1;
							notCompletedOutputItemsTableToClear.Add( tableIndex );
						}
					}
					else
					{
						//without instancing
						var outputItem = new OutputItem();
						outputItem.operation = item.operation;
						outputItem.materialData = item.materialData;
						outputItem.allowInstancing = false;
						outputItem.renderableItemFirst = new Vector3I( item.renderableGroup, item.operationIndex );
						outputItem.renderableItemsCount = 1;
						outputItems.Add( ref outputItem );
					}
				}

				//clear table
				for( int n = 0; n < notCompletedOutputItemsTableToClear.Count; n++ )
				{
					int tableIndex = notCompletedOutputItemsTableToClear.Data[ n ];
					notCompletedOutputItemsTable[ tableIndex ] = -1;
				}
				notCompletedOutputItemsTableToClear.Clear();
			}

			public unsafe void Clear()
			{
				items.Clear();

				for( int n = 0; n < operations.Count; n++ )
					operations.Data[ n ]._currentRenderingFrameIndex = -1;
				operations.Clear();

				for( int n = 0; n < materialDatas.Count; n++ )
					materialDatas.Data[ n ]._currentRenderingFrameIndex = -1;
				materialDatas.Clear();

				for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
				{
					ref var outputItem = ref outputItems.Data[ nOutputItem ];

					if( outputItem.renderableItems != null )
						renderableItemArrays.Push( (IntPtr)outputItem.renderableItems );
				}
				outputItems.Clear();
			}
		}

		/////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		struct RenderOperationDataStructure
		{
			public Vector4F data0;
			public Vector4F data1;
			public Vector4F data2;
			public Vector4F data3;
			public ColorValue data4;
		}

		/////////////////////////////////////////

		static RenderingPipeline_Basic()
		{
			renderOperationDataCurrent.data0.X = float.MaxValue;
		}

		protected virtual void PrepareListsOfObjects( ViewportRenderingContext context, FrameData frameData )
		{
			Viewport viewportOwner = context.Owner;
			var renderSceneData = frameData.RenderSceneData;

			var scene = viewportOwner.AttachedScene;
			if( scene == null || !scene.EnabledInHierarchy )
				return;

			OcclusionCullingBuffer sceneOcclusionCullingBuffer = null;

			//init occlusion culling buffer for main viewport and get occluders
			if( OcclusionCullingBuffer && NeoAxis.OcclusionCullingBuffer.Supported && OcclusionCullingBufferSize.Value > 0 )
			{
				var cameraFrustum = viewportOwner.CameraSettings.Frustum;

				var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, cameraFrustum );
				getObjectsItem.GroupMask = 0x2;
				scene.GetObjectsInSpace( getObjectsItem );

				var occluders = new OpenList<OccluderItem>( getObjectsItem.Result.Length );

				for( int nObject = 0; nObject < getObjectsItem.Result.Length; nObject++ )
				{
					var obj = getObjectsItem.Result[ nObject ].Object;
					if( obj.EnabledInHierarchy )
						obj.PerformOcclusionCullingDataGet( context, GetRenderSceneDataMode.InsideFrustum, getObjectsItem, occluders );
				}

				if( occluders.Count != 0 )
				{
					var demandedSize = NeoAxis.OcclusionCullingBuffer.GetSizeByHeight( context.Owner.SizeInPixels, OcclusionCullingBufferSize );
					var buffer = context.OcclusionCullingBuffer_Alloc( demandedSize );

					OcclusionCullingBuffer_RenderOccluders( context, /*frameData, */context.Owner.CameraSettings, buffer, occluders, true );

					sceneOcclusionCullingBuffer = buffer;
				}
			}

			//find sky, fog. ambient, directional lights
			var ambientDirectionalLights = new List<Light>();
			foreach( var c in scene.CachedObjectsInSpaceToFastFindByRenderingPipeline )//scene.GetComponents( false, true, true, delegate ( Component c )
			{
				if( c is Sky sky && viewportOwner.CameraSettings.RenderSky )
				{
					if( frameData.Sky == null )
						frameData.Sky = sky;
				}
				else if( c is Fog fog )
				{
					if( frameData.Fog == null )
						frameData.Fog = fog;
				}
				else if( c is Light light )
				{
					if( light.VisibleInHierarchy )
					{
						var type = light.Type.Value;
						if( type == Light.TypeEnum.Ambient || type == Light.TypeEnum.Directional )
							ambientDirectionalLights.Add( light );
					}
				}
			}// );

			//Scene.GetRenderSceneData
			frameData.SceneGetRenderSceneData( context, scene );

			{
				var cameraSettings = context.Owner.CameraSettings;
				var cameraFrustum = viewportOwner.CameraSettings.Frustum;
				var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, cameraFrustum );

				var extensionData = new Scene.GetObjectsInSpaceItem.ExtensionDataStructure();
				if( sceneOcclusionCullingBuffer != null )
				{
					extensionData.OcclusionCullingBuffer = sceneOcclusionCullingBuffer.NativeObject;

					var viewProjectionMatrix = cameraSettings.GetViewProjectionMatrix();
					//!!!!double
					viewProjectionMatrix.ToMatrix4F( out extensionData.ViewProjectionMatrix );

					extensionData.OcclusionCullingBufferCullNodes = OcclusionCullingBufferCullNodes.Value ? 1 : 0;
					extensionData.OcclusionCullingBufferCullObjects = OcclusionCullingBufferCullObjects.Value ? 1 : 0;

					unsafe
					{
						getObjectsItem.ExtensionData = new IntPtr( &extensionData );
					}
				}

				//add ambient, directional lights
				foreach( var light in ambientDirectionalLights )
					frameData.RegisterObjectInSpace( context, light, GetRenderSceneDataMode.InsideFrustum, getObjectsItem );

				//get visible objects
				{
					//if( EngineApp._DebugCapsLock )
					//{

					var getObjectsItems = new Scene.GetObjectsInSpaceItem[ 4 ];
					for( int n = 0; n < getObjectsItems.Length; n++ )
						getObjectsItems[ n ] = getObjectsItem.Clone();

					var cameraPlanes = cameraFrustum.Planes;
					Bounds bounds = new Bounds( cameraFrustum.Points[ 0 ] );
					for( int n = 1; n < 8; n++ )
						bounds.Add( cameraFrustum.Points[ n ] );

					//split screen to 4 frustums, find objects on 4 threads

					//!!!!лишние плоскости можно убрать
					//!!!!bounds можно уменьшить

					for( int n = 0; n < 4; n++ )
					{
						ref var item = ref getObjectsItems[ n ];

						var planes = new List<Plane>( cameraPlanes.Length + 2 );

						switch( n )
						{
						case 0:
							planes.Add( Plane.FromPointAndNormal( cameraSettings.Position, -cameraSettings.Up ) );
							planes.Add( Plane.FromPointAndNormal( cameraSettings.Position, -cameraSettings.Right ) );
							break;

						case 1:
							planes.Add( Plane.FromPointAndNormal( cameraSettings.Position, -cameraSettings.Up ) );
							planes.Add( Plane.FromPointAndNormal( cameraSettings.Position, cameraSettings.Right ) );
							break;

						case 2:
							planes.Add( Plane.FromPointAndNormal( cameraSettings.Position, cameraSettings.Up ) );
							planes.Add( Plane.FromPointAndNormal( cameraSettings.Position, -cameraSettings.Right ) );
							break;

						case 3:
							planes.Add( Plane.FromPointAndNormal( cameraSettings.Position, cameraSettings.Up ) );
							planes.Add( Plane.FromPointAndNormal( cameraSettings.Position, cameraSettings.Right ) );
							break;
						}

						planes.AddRange( cameraPlanes );

						item.Frustum = null;
						item.Planes = planes.ToArray();
						item.Bounds = bounds;
					}


					scene.GetObjectsInSpace( getObjectsItems );


					foreach( var item in getObjectsItems )
					{
						for( int nObject = 0; nObject < item.Result.Length; nObject++ )
						{
							var obj = item.Result[ nObject ].Object;
							if( obj.EnabledInHierarchy && obj.VisibleInHierarchy )
								frameData.RegisterObjectInSpace( context, obj, GetRenderSceneDataMode.InsideFrustum, getObjectsItem );
						}
					}

					//}
					//else
					//{
					//	scene.GetObjectsInSpace( getObjectsItem );

					//	for( int nObject = 0; nObject < getObjectsItem.Result.Length; nObject++ )
					//	{
					//		var obj = getObjectsItem.Result[ nObject ].Object;
					//		if( obj.EnabledInHierarchy && obj.VisibleInHierarchy )
					//		{
					//			//if( ComponentsHidePublic.GetRenderSceneIndex( obj ) != -1 )
					//			//	Log.Fatal( "RenderingPipeline_Render: PrepareListOfObjects: obj._internalRenderSceneIndex != -1." );

					//			frameData.RegisterObjectInSpace( context, obj, GetRenderSceneDataMode.InsideFrustum, getObjectsItem );
					//		}
					//	}
					//}
				}
			}

			//update ambient lights
			int[] disableLights = null;
			{
				var ambientLights = new List<int>();
				for( int lightIndex = 0; lightIndex < frameData.Lights.Count; lightIndex++ )
				{
					var lightItem = frameData.Lights[ lightIndex ];
					if( lightItem.data.Type == Light.TypeEnum.Ambient )
						ambientLights.Add( lightIndex );
				}

				//add ambient light if not exists
				if( ambientLights.Count == 0 )
					frameData.AddZeroAmbientLight( context );

				//many ambient lights warning
				if( ambientLights.Count > 1 )
				{
					Log.Warning( "Too many ambient lights. Must be only one." );

					//disable lights
					var disableLights2 = new List<int>();
					for( int n = 1; n < ambientLights.Count; n++ )
						disableLights2.Add( ambientLights[ n ] );
					disableLights = disableLights2.ToArray();
				}
			}

			//get sorted lights. sort lights by type and by distance
			{
				var list = new List<int>( frameData.Lights.Count );
				for( int lightIndex = 0; lightIndex < frameData.Lights.Count; lightIndex++ )
				{
					if( disableLights == null || !disableLights.Contains( lightIndex ) )
						list.Add( lightIndex );
				}

				frameData.LightsInFrustumSorted = list.ToArray();
				//frameData.LightsSorted = new int[ frameData.Lights.Count ];
				//for( int n = 0; n < frameData.LightsSorted.Length; n++ )
				//frameData.LightsSorted[ n ] = n;
				CollectionUtility.MergeSort( frameData.LightsInFrustumSorted, delegate ( int index1, int index2 )
				{
					var light1 = frameData.Lights[ index1 ];
					var light2 = frameData.Lights[ index2 ];
					if( (int)light1.data.Type < (int)light2.data.Type )
						return -1;
					if( (int)light1.data.Type > (int)light2.data.Type )
						return 1;
					if( light1.distanceToCameraSquared < light2.distanceToCameraSquared )
						return -1;
					if( light1.distanceToCameraSquared > light2.distanceToCameraSquared )
						return 1;
					return 0;
				} );
			}

			//LightItem.prepareShadows
			if( Shadows && ShadowIntensity > 0 && UseRenderTargets && DebugMode.Value == DebugModeEnum.None && RenderingSystem.ShadowTechnique != ProjectSettingsPage_Rendering.ShadowTechniqueEnum.None )
			{
				int[] remains = new int[ 4 ];
				remains[ (int)Light.TypeEnum.Directional ] = ShadowDirectionalLightMaxCount;
				remains[ (int)Light.TypeEnum.Point ] = ShadowPointLightMaxCount;
				remains[ (int)Light.TypeEnum.Spotlight ] = ShadowSpotlightMaxCount;

				var cameraPosition = viewportOwner.CameraSettings.Position;
				double shadowFarDistance = ShadowFarDistance;

				//!!!!так распределять? просто по расстоянию от центра
				foreach( var lightIndex in frameData.LightsInFrustumSorted )
				{
					var item = frameData.Lights[ lightIndex ];

					if( item.data.CastShadows && item.data.ShadowIntensity > 0 && remains[ (int)item.data.Type ] > 0 )
					{
						bool skip = false;
						if( item.data.Type == Light.TypeEnum.Point )
						{
							if( ( cameraPosition - item.data.Position ).LengthSquared() >
								( shadowFarDistance + item.data.AttenuationFar ) * ( shadowFarDistance + item.data.AttenuationFar ) )
								skip = true;
						}
						else if( item.data.Type == Light.TypeEnum.Spotlight )
						{
							if( item.data.BoundingBox.GetPointDistance( cameraPosition ) > shadowFarDistance )
								skip = true;
						}

						if( !skip )
						{
							item.prepareShadows = true;
							remains[ (int)item.data.Type ] = remains[ (int)item.data.Type ] - 1;
						}
					}
				}
			}

			//get affected objects for lights, shadows affected meshes in space
			{
				//step 1. prepare data to get affects objects of the scene.
				var lightAffectedObjects = new LightAffectedObjectsItem[ frameData.Lights.Count ];
				var lightAffectedObjectsGetObjectsList = new List<Scene.GetObjectsInSpaceItem>( frameData.Lights.Count + 9 );
				{
					foreach( var lightIndex in frameData.LightsInFrustumSorted )
					{
						var lightItem = frameData.Lights[ lightIndex ];

						if( lightItem.data.Type == Light.TypeEnum.Directional )
						{
							//Directional light

							//shadowsAffectedMeshesInSpace
							if( lightItem.prepareShadows )
							{
								var item = new LightAffectedObjectsItem();

								for( int nIteration = 0; nIteration < ShadowDirectionalLightCascades; nIteration++ )
								{
									//!!!!slowly?
									GetDirectionalLightShadowsCascadeHullPlanes( context, lightItem, nIteration, out var planes, out var bounds );

									var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, planes, bounds );

									item.SetItem( nIteration, getObjectsItem );
									lightAffectedObjectsGetObjectsList.Add( getObjectsItem );
								}

								lightAffectedObjects[ lightIndex ] = item;
							}
						}
						else if( lightItem.data.Type == Light.TypeEnum.Point )
						{
							//Point light

							var sphere = new Sphere( lightItem.data.Position, lightItem.data.AttenuationFar );
							var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );

							var item = new LightAffectedObjectsItem();
							item.SetItem( 0, getObjectsItem );
							lightAffectedObjects[ lightIndex ] = item;
							lightAffectedObjectsGetObjectsList.Add( getObjectsItem );
						}
						else if( lightItem.data.Type == Light.TypeEnum.Spotlight )
						{
							//Spotlight

							var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, lightItem.data.SpotlightClipPlanes, lightItem.data.BoundingBox );

							var item = new LightAffectedObjectsItem();
							item.SetItem( 0, getObjectsItem );
							lightAffectedObjects[ lightIndex ] = item;
							lightAffectedObjectsGetObjectsList.Add( getObjectsItem );
						}
					}
				}

				//step 2. get objects.
				scene.GetObjectsInSpace( lightAffectedObjectsGetObjectsList );

				//step 3. process objects.
				foreach( var lightIndex in frameData.LightsInFrustumSorted )
				{
					var lightItem = frameData.Lights[ lightIndex ];

					if( lightItem.data.Type == Light.TypeEnum.Directional )
					{
						//Directional light

						//shadowsAffectedMeshesInSpace
						if( lightItem.prepareShadows )
						{
							for( int nIteration = 0; nIteration < ShadowDirectionalLightCascades; nIteration++ )
							{
								var getObjectsItem = lightAffectedObjects[ lightIndex ].GetItem( nIteration );
								//GetDirectionalLightShadowsCascadeHullPlanes( context, lightItem, nIteration, out var planes, out var bounds );
								//var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, planes, bounds );
								//scene.GetObjectsInSpace( getObjectsItem );

								//!!!!
								//? проверить есль ли вообще тени у всего меша. не только по подмешам.
								//? пообъектное отсечение делать после того как определено что это шадов кастер.

								var cascadeFrustum = GetDirectionalLightShadowsFrustum( context, nIteration );
								//need precalculate Planes array for Parallel
								//var cascadeFrustumPlanes = cascadeFrustum.Planes;

								//var cameraFrustum = viewportOwner.CameraSettings.Frustum;
								var lightDirection = lightItem.data.Rotation.GetForward();//.GetNormalize();
								var lightRotationMatrix = lightItem.data.Rotation.ToMatrix3();
								var shadowMapFarClipDistance = (float)ShadowDirectionalLightExtrusionDistance * 2;
								//var lightExtrusionDistance = ShadowDirectionalLightExtrusionDistance.Value;

								//get objects data
								var objectsData = new OpenList<ShadowsObjectData>( getObjectsItem.Result.Length );
								for( int nObject = 0; nObject < getObjectsItem.Result.Length; nObject++ )
								{
									var obj = getObjectsItem.Result[ nObject ].Object;
									if( obj.VisibleInHierarchy )
									{
										var item = new ShadowsObjectData();
										item.position = obj.TransformV.Position;
										obj.SpaceBounds.GetCalculatedBoundingSphere( out item.boundingSphere );
										objectsData.Add( ref item );
									}
								}


								unsafe void Calculate( int nObject )
								{
									ref var objectData = ref objectsData.Data[ nObject ];

									ref var position = ref objectData.position;
									ref var boundingSphere = ref objectData.boundingSphere;
									var radius = (float)( ( boundingSphere.Center - position ).Length() + boundingSphere.Radius );


									Vector3* points = stackalloc Vector3[ 8 ];
									{
										{
											var r = new Vector3F( -radius, radius, radius );
											Matrix3F.Multiply( ref lightRotationMatrix, ref r, out var r2 );
											points[ 0 ] = position + r2;
										}

										{
											var r = new Vector3F( -radius, radius, -radius );
											Matrix3F.Multiply( ref lightRotationMatrix, ref r, out var r2 );
											points[ 1 ] = position + r2;
										}

										{
											var r = new Vector3F( -radius, -radius, -radius );
											Matrix3F.Multiply( ref lightRotationMatrix, ref r, out var r2 );
											points[ 2 ] = position + r2;
										}

										{
											var r = new Vector3F( -radius, -radius, radius );
											Matrix3F.Multiply( ref lightRotationMatrix, ref r, out var r2 );
											points[ 3 ] = position + r2;
										}

										//points[ 0 ] = position + lightRotationMatrix * new Vector3F( -radius, radius, radius );
										//points[ 1 ] = position + lightRotationMatrix * new Vector3F( -radius, radius, -radius );
										//points[ 2 ] = position + lightRotationMatrix * new Vector3F( -radius, -radius, -radius );
										//points[ 3 ] = position + lightRotationMatrix * new Vector3F( -radius, -radius, radius );

										//!!!!shadowMapFarClipDistance
										var offset = lightDirection * ( shadowMapFarClipDistance + radius * 2 );
										//var offset = lightDirection * 30;
										for( int n = 0; n < 4; n++ )
											points[ n + 4 ] = points[ n ] + offset;
									}

									var cascadeFrustumPlanes = cascadeFrustum.Planes;
									unsafe
									{
										fixed( Plane* pCascadeFrustumPlanes = cascadeFrustumPlanes )
										{
											if( Intersects( pCascadeFrustumPlanes, points, 8 ) )
												objectData.passed = 1;
										}
									}

									//viewportOwner.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
									//foreach( var p in points )
									//	viewportOwner.Simple3DRenderer.AddSphere( new Sphere( p, .1 ), 8, false, -1 );

									//var center = position + lightDirection.ToVector3() * ( shadowMapFarClipDistance / 2 );
									//var extents = new Vector3( shadowMapFarClipDistance * 0.5 + radius, radius, radius );
									//var box = new Box( center, extents, lightRotationMatrix );

									////viewportOwner.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
									////viewportOwner.Simple3DRenderer.AddBox( box, false, -1 );

									//if( !cascadeFrustum.Intersects( box ) )
									//	continue;
									//}

								}

								Parallel.For( 0, objectsData.Count, Calculate );
								//for( int nObject = 0; nObject < objectsData.Length; nObject++ )
								//	Calculate( nObject );

								//process passed objects
								for( int nObject = 0; nObject < objectsData.Count; nObject++ )
								{
									ref var objectData = ref objectsData.Data[ nObject ];

									if( objectData.passed != 0 )//if( passedList[ nObject ] != 0 )
									{
										var obj = getObjectsItem.Result[ nObject ].Object;

										var objIndex = frameData.RegisterObjectInSpace( context, obj, GetRenderSceneDataMode.ShadowCasterOutsideFrustum, getObjectsItem );
										ref var data = ref frameData.ObjectInSpaces.Data[ objIndex ];

										if( data.ContainsData )
										{
											for( int n = data.MeshRange.Minimum; n < data.MeshRange.Maximum; n++ )
											{
												ref var item = ref renderSceneData.Meshes.Data[ n ];
												if( item.CastShadows )
												{
													if( lightItem.shadowsAffectedRenderableGroups == null )
														lightItem.shadowsAffectedRenderableGroups = new OpenListNative<Vector2I>[ ShadowDirectionalLightCascades ];
													if( lightItem.shadowsAffectedRenderableGroups[ nIteration ] == null )
														lightItem.shadowsAffectedRenderableGroups[ nIteration ] = new OpenListNative<Vector2I>( getObjectsItem.Result.Length + 512 );
													lightItem.shadowsAffectedRenderableGroups[ nIteration ].Add( new Vector2I( 0, n ) );
												}
											}
											for( int n = data.BillboardRange.Minimum; n < data.BillboardRange.Maximum; n++ )
											{
												ref var item = ref renderSceneData.Billboards.Data[ n ];
												if( item.CastShadows )
												{
													if( lightItem.shadowsAffectedRenderableGroups == null )
														lightItem.shadowsAffectedRenderableGroups = new OpenListNative<Vector2I>[ ShadowDirectionalLightCascades ];
													if( lightItem.shadowsAffectedRenderableGroups[ nIteration ] == null )
														lightItem.shadowsAffectedRenderableGroups[ nIteration ] = new OpenListNative<Vector2I>( getObjectsItem.Result.Length + 512 );
													lightItem.shadowsAffectedRenderableGroups[ nIteration ].Add( new Vector2I( 1, n ) );
												}
											}
										}
									}
								}

								//foreach( var objItem in getObjectsItem.Result )
								//{
								//	var obj = objItem.Object;
								//	if( obj.VisibleInHierarchy )
								//	{
								//		temp
								//		if( !CapsLock )
								//		{
								//			//!!!!slowly

								//			var position = obj.TransformV.Position;
								//			//!!!!
								//			var boundingSphere = obj.SpaceBounds.CalculatedBoundingSphere;
								//			var radius = (float)( ( boundingSphere.Origin - position ).Length() + boundingSphere.Radius );

								//			var points = new Vector3[ 8 ];
								//			{
								//				points[ 0 ] = position + lightRotationMatrix * new Vector3F( -radius, radius, radius );
								//				points[ 1 ] = position + lightRotationMatrix * new Vector3F( -radius, radius, -radius );
								//				points[ 2 ] = position + lightRotationMatrix * new Vector3F( -radius, -radius, -radius );
								//				points[ 3 ] = position + lightRotationMatrix * new Vector3F( -radius, -radius, radius );

								//				var offset = lightDirection * ( (float)shadowMapFarClipDistance + radius * 2 );
								//				//var offset = lightDirection * 30;
								//				for( int n = 0; n < 4; n++ )
								//					points[ n + 4 ] = points[ n ] + offset;
								//			}

								//			//viewportOwner.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
								//			//foreach( var p in points )
								//			//	viewportOwner.Simple3DRenderer.AddSphere( new Sphere( p, .1 ), 8, false, -1 );

								//			if( !Intersects( cascadeFrustum, points ) )
								//				continue;

								//			//var center = position + lightDirection.ToVector3() * ( shadowMapFarClipDistance / 2 );
								//			//var extents = new Vector3( shadowMapFarClipDistance * 0.5 + radius, radius, radius );
								//			//var box = new Box( center, extents, lightRotationMatrix );

								//			////viewportOwner.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
								//			////viewportOwner.Simple3DRenderer.AddBox( box, false, -1 );

								//			//if( !cascadeFrustum.Intersects( box ) )
								//			//	continue;
								//		}


								//		//var ray = new Ray( pos - lightDirection * 1000, lightDirection * 2000 );

								//		//!!!!
								//		//var box2 = new Box( center, extents, lightRotationMatrix.GetTranspose() );
								//		//if( !cameraFrustum.IntersectsFast( box2 ) )
								//		//	continue;
								//		//var box = new Box( center, extents, lightRotationMatrix );


								//		temp
								//		//if( !CapsLock )
								//		//{
								//		//	var pos = obj.TransformV.Position;
								//		//	var lightDir = lightItem.data.Rotation.GetForward();
								//		//	var ray = new Ray( pos - lightDir * 1000, lightDir * 2000 );
								//		//	if( !cameraFrustum.IntersectsFast( new Bounds( pos ) ) )
								//		//		continue;
								//		//}

								//		var objIndex = frameData.RegisterObjectInSpace( context, obj, GetRenderSceneDataMode.ShadowCasterOutsideFrustum );
								//		ref var data = ref frameData.ObjectInSpaces.Data[ objIndex ];

								//		if( data.ContainsData )
								//		{
								//			for( int n = data.MeshRange.Minimum; n < data.MeshRange.Maximum; n++ )
								//			{
								//				ref var item = ref renderSceneData.Meshes.Data[ n ];
								//				if( item.CastShadows )
								//				{
								//					if( lightItem.shadowsAffectedRenderableGroups == null )
								//						lightItem.shadowsAffectedRenderableGroups = new List<Vector2I>[ ShadowDirectionalLightCascades ];
								//					if( lightItem.shadowsAffectedRenderableGroups[ nIteration ] == null )
								//						lightItem.shadowsAffectedRenderableGroups[ nIteration ] = new List<Vector2I>( 512 );
								//					lightItem.shadowsAffectedRenderableGroups[ nIteration ].Add( new Vector2I( 0, n ) );
								//				}
								//			}
								//			for( int n = data.BillboardRange.Minimum; n < data.BillboardRange.Maximum; n++ )
								//			{
								//				ref var item = ref renderSceneData.Billboards.Data[ n ];
								//				if( item.CastShadows )
								//				{
								//					if( lightItem.shadowsAffectedRenderableGroups == null )
								//						lightItem.shadowsAffectedRenderableGroups = new List<Vector2I>[ ShadowDirectionalLightCascades ];
								//					if( lightItem.shadowsAffectedRenderableGroups[ nIteration ] == null )
								//						lightItem.shadowsAffectedRenderableGroups[ nIteration ] = new List<Vector2I>( 512 );
								//					lightItem.shadowsAffectedRenderableGroups[ nIteration ].Add( new Vector2I( 1, n ) );
								//				}
								//			}
								//		}
								//	}
								//}

							}
						}
					}
					else if( lightItem.data.Type == Light.TypeEnum.Point )
					{
						//Point light

						var getObjectsItem = lightAffectedObjects[ lightIndex ].GetItem( 0 );
						//var sphere = new Sphere( lightItem.data.Position, lightItem.data.AttenuationFar );
						//var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );
						//scene.GetObjectsInSpace( getObjectsItem );

						bool[] addFaces = new bool[ 6 ];

						for( int nObject = 0; nObject < getObjectsItem.Result.Length; nObject++ )
						{
							var obj = getObjectsItem.Result[ nObject ].Object;
							if( obj.VisibleInHierarchy )
							{
								var objIndex = frameData.RegisterObjectInSpace( context, obj, GetRenderSceneDataMode.ShadowCasterOutsideFrustum, getObjectsItem );//, false );
								ref var data = ref frameData.ObjectInSpaces.Data[ objIndex ];

								if( data.ContainsData )
								{
									for( int n = data.MeshRange.Minimum; n < data.MeshRange.Maximum; n++ )
									{
										ref var item = ref renderSceneData.Meshes.Data[ n ];
										ref var data2 = ref frameData.Meshes.Data[ n ];

										if( ( data2.Flags & FrameData.MeshItem.FlagsEnum.CalculateAffectedLights ) != 0 )
											data2.AddPointSpotLight( lightIndex );

										//LightItem.shadowsAffectedRenderableGroups
										if( item.CastShadows && lightItem.prepareShadows )
										{
											PointLightShadowGenerationCheckAddFaces( ref lightItem.data.Position, ref item.BoundingSphere, addFaces );

											for( int nFace = 0; nFace < 6; nFace++ )
											{
												if( addFaces[ nFace ] )
												{
													if( lightItem.shadowsAffectedRenderableGroups == null )
														lightItem.shadowsAffectedRenderableGroups = new OpenListNative<Vector2I>[ 6 ];
													if( lightItem.shadowsAffectedRenderableGroups[ nFace ] == null )
														lightItem.shadowsAffectedRenderableGroups[ nFace ] = new OpenListNative<Vector2I>( getObjectsItem.Result.Length + 128 );
													lightItem.shadowsAffectedRenderableGroups[ nFace ].Add( new Vector2I( 0, n ) );
												}
											}
										}
									}
									for( int n = data.BillboardRange.Minimum; n < data.BillboardRange.Maximum; n++ )
									{
										ref var item = ref renderSceneData.Billboards.Data[ n ];
										ref var data2 = ref frameData.Billboards.Data[ n ];

										if( ( data2.Flags & FrameData.BillboardItem.FlagsEnum.CalculateAffectedLights ) != 0 )
											data2.AddPointSpotLight( lightIndex );

										//LightItem.shadowsAffectedRenderableGroups
										if( item.CastShadows && lightItem.prepareShadows )
										{
											Sphere meshSphere = item.BoundingSphere;// meshInSpace.SpaceBounds.CalculatedBoundingSphere;
											PointLightShadowGenerationCheckAddFaces( ref lightItem.data.Position, ref meshSphere, addFaces );

											for( int nFace = 0; nFace < 6; nFace++ )
											{
												if( addFaces[ nFace ] )
												{
													if( lightItem.shadowsAffectedRenderableGroups == null )
														lightItem.shadowsAffectedRenderableGroups = new OpenListNative<Vector2I>[ 6 ];
													if( lightItem.shadowsAffectedRenderableGroups[ nFace ] == null )
														lightItem.shadowsAffectedRenderableGroups[ nFace ] = new OpenListNative<Vector2I>( getObjectsItem.Result.Length + 128 );
													lightItem.shadowsAffectedRenderableGroups[ nFace ].Add( new Vector2I( 1, n ) );
												}
											}
										}
									}
								}
							}
						}
					}
					else if( lightItem.data.Type == Light.TypeEnum.Spotlight )
					{
						//Spotlight

						var getObjectsItem = lightAffectedObjects[ lightIndex ].GetItem( 0 );
						//var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, lightItem.data.SpotlightClipPlanes, lightItem.data.BoundingBox );
						//scene.GetObjectsInSpace( getObjectsItem );

						for( int nObject = 0; nObject < getObjectsItem.Result.Length; nObject++ )
						{
							var obj = getObjectsItem.Result[ nObject ].Object;
							if( obj.VisibleInHierarchy )
							{
								var objIndex = frameData.RegisterObjectInSpace( context, obj, GetRenderSceneDataMode.ShadowCasterOutsideFrustum, getObjectsItem );//, false );
								ref var data = ref frameData.ObjectInSpaces.Data[ objIndex ];

								if( data.ContainsData )
								{
									for( int n = data.MeshRange.Minimum; n < data.MeshRange.Maximum; n++ )
									{
										ref var item = ref renderSceneData.Meshes.Data[ n ];
										ref var data2 = ref frameData.Meshes.Data[ n ];

										if( ( data2.Flags & FrameData.MeshItem.FlagsEnum.CalculateAffectedLights ) != 0 )
											data2.AddPointSpotLight( lightIndex );

										//LightItem.shadowsAffectedRenderableGroups
										if( item.CastShadows && lightItem.prepareShadows )
										{
											if( lightItem.shadowsAffectedRenderableGroups == null )
												lightItem.shadowsAffectedRenderableGroups = new OpenListNative<Vector2I>[ 1 ];
											if( lightItem.shadowsAffectedRenderableGroups[ 0 ] == null )
												lightItem.shadowsAffectedRenderableGroups[ 0 ] = new OpenListNative<Vector2I>( getObjectsItem.Result.Length + 128 );
											lightItem.shadowsAffectedRenderableGroups[ 0 ].Add( new Vector2I( 0, n ) );
										}
									}
									for( int n = data.BillboardRange.Minimum; n < data.BillboardRange.Maximum; n++ )
									{
										ref var item = ref renderSceneData.Billboards.Data[ n ];
										ref var data2 = ref frameData.Billboards.Data[ n ];

										if( ( data2.Flags & FrameData.BillboardItem.FlagsEnum.CalculateAffectedLights ) != 0 )
											data2.AddPointSpotLight( lightIndex );

										//LightItem.shadowsAffectedRenderableGroups
										if( item.CastShadows && lightItem.prepareShadows )
										{
											if( lightItem.shadowsAffectedRenderableGroups == null )
												lightItem.shadowsAffectedRenderableGroups = new OpenListNative<Vector2I>[ 1 ];
											if( lightItem.shadowsAffectedRenderableGroups[ 0 ] == null )
												lightItem.shadowsAffectedRenderableGroups[ 0 ] = new OpenListNative<Vector2I>( getObjectsItem.Result.Length + 128 );
											lightItem.shadowsAffectedRenderableGroups[ 0 ].Add( new Vector2I( 1, n ) );
										}
									}
								}
							}
						}
					}
				}
			}

			//All data are prepared for rendering

			context.UpdateStatisticsCurrent.Lights = frameData.LightsInFrustumSorted.Length;
			context.UpdateStatisticsCurrent.ReflectionProbes = frameData.ReflectionProbes.Count;

			//!!!!
			// пока так, потом, видимо так же как источники света сортировать
			//sort. from biggest radius to smallest
			CollectionUtility.MergeSort( frameData.ReflectionProbes, ( x, y ) => y.data.Sphere.Radius.CompareTo( x.data.Sphere.Radius ) );
			//frameData.ReflectionProbes.Sort( ( x, y ) => y.data.Sphere.Radius.CompareTo( x.data.Sphere.Radius ) );
		}

		protected virtual void ClearTempData( ViewportRenderingContext context, FrameData frameData )
		{
			for( int n = 0; n < frameData.ObjectInSpaces.Count; n++ )
			{
				ref var data = ref frameData.ObjectInSpaces.Data[ n ];
				ComponentsHidePublic.SetRenderSceneIndex( data.ObjectInSpace, -1 );
			}

			for( int n = 0; n < frameData.Materials.Count; n++ )
			{
				var materialData = frameData.Materials.Data[ n ];
				materialData.currentFrameIndex = -1;
				materialData.currentFrameData?.Clear();
				materialData.currentFrameDataSpecialShadowCaster?.Clear();
			}

			for( int n = 0; n < frameData.Lights.Count; n++ )
			{
				var lightItem = frameData.Lights[ n ];
				if( lightItem.shadowsAffectedRenderableGroups != null )
				{
					foreach( var list in lightItem.shadowsAffectedRenderableGroups )
						list?.Dispose();
					lightItem.shadowsAffectedRenderableGroups = null;
				}
			}
		}

		bool ContainsDisposedVertexIndexBuffers( RenderSceneData.MeshDataRenderOperation op )
		{
			for( int n = 0; n < op.VertexBuffers.Count; n++ )
			{
				var buffer = op.VertexBuffers[ n ];
				if( buffer.Disposed )
					return true;
			}
			var indexBuffer = op.IndexBuffer;
			if( indexBuffer != null )
			{
				if( indexBuffer.Disposed )
					return true;
			}
			return false;
		}

		void SetVertexIndexBuffers( ViewportRenderingContext context, RenderSceneData.MeshDataRenderOperation op )
		{
			for( int n = 0; n < op.VertexBuffers.Count; n++ )
			{
				var buffer = op.VertexBuffers[ n ];
				context.SetVertexBuffer( n, buffer, op.VertexStartOffset, op.VertexCount );
			}
			var indexBuffer = op.IndexBuffer;
			if( indexBuffer != null )
				context.SetIndexBuffer( indexBuffer, op.IndexStartOffset, op.IndexCount );
		}

		public void RenderOperation( ViewportRenderingContext context, RenderSceneData.MeshDataRenderOperation op, GpuMaterialPass pass, List<ParameterContainer> parameterContainers, RenderSceneData.CutVolumeItem[] cutVolumes = null, GpuVertexBuffer instanceBuffer = null, int instanceCount = -1 )
		{
			if( ContainsDisposedVertexIndexBuffers( op ) )
				return;
			if( instanceBuffer != null && instanceBuffer.Disposed )
				return;

			//!!!!можно склеить PInvoke вызовы

			SetCutVolumeSettingsUniforms( context, cutVolumes, false );
			SetVertexIndexBuffers( context, op );

			if( instanceBuffer != null )
			{
				var instanceCount2 = instanceCount;
				if( instanceCount2 == -1 )
					instanceCount2 = instanceBuffer.VertexCount;
				context.SetInstanceDataBuffer( instanceBuffer, 0, instanceCount2 );
				context.SetPassAndSubmit( pass, RenderOperationType.TriangleList, parameterContainers );
				context.UpdateStatisticsCurrent.Triangles += ( op.IndexBuffer != null ? op.IndexCount / 3 : op.VertexCount / 3 ) * instanceCount2;
			}
			else
			{
				context.SetPassAndSubmit( pass, RenderOperationType.TriangleList, parameterContainers );
				context.UpdateStatisticsCurrent.Triangles += op.IndexBuffer != null ? op.IndexCount / 3 : op.VertexCount / 3;
			}
		}

		public void RenderOperation( ViewportRenderingContext context, RenderSceneData.MeshDataRenderOperation op, GpuMaterialPass pass, List<ParameterContainer> parameterContainers, RenderSceneData.CutVolumeItem[] cutVolumes, ref InstanceDataBuffer instanceBuffer, int instanceCount )
		{
			if( ContainsDisposedVertexIndexBuffers( op ) )
				return;

			//!!!!можно склеить PInvoke вызовы

			SetCutVolumeSettingsUniforms( context, cutVolumes, false );
			SetVertexIndexBuffers( context, op );
			Bgfx.SetInstanceDataBuffer( ref instanceBuffer, 0, instanceCount );
			context.SetPassAndSubmit( pass, RenderOperationType.TriangleList, parameterContainers );
			context.UpdateStatisticsCurrent.Triangles += ( op.IndexBuffer != null ? op.IndexCount / 3 : op.VertexCount / 3 ) * instanceCount;
		}

		//public void RenderOperation( ViewportRenderingContext context, RenderSceneData.MeshDataRenderOperation op, GpuMaterialPass pass, List<ParameterContainer> parameterContainers, GpuVertexBuffer instanceBuffer, int instanceCount = -1 )
		//{
		//	if( !SetVertexIndexBuffers( context, op ) )
		//		return;

		//	var instanceCount2 = instanceCount;
		//	if( instanceCount2 == -1 )
		//		instanceCount2 = instanceBuffer.VertexCount;

		//	context.SetInstanceDataBuffer( instanceBuffer, 0, instanceCount2 );
		//	context.SetPassAndSubmit( pass, RenderOperationType.TriangleList, parameterContainers );
		//	context.UpdateStatisticsCurrent.Triangles += ( op.IndexBuffer != null ? op.IndexCount / 3 : op.VertexCount / 3 ) * instanceCount2;
		//}

		//!!!!!было нужное из RenderSystem
		//virtual void _setTextureLayerAnisotropy( size_t unit, unsigned int maxAnisotropy ) = 0;
		//virtual void _setTextureMipmapBias( size_t unit, float bias ) = 0;
		//Stencil
		//virtual void setStencilCheckEnabled( bool enabled ) = 0;
		//virtual void setStencilBufferParams( CompareFunction func = CMPF_ALWAYS_PASS,
		//	uint32 refValue = 0, uint32 mask = 0xFFFFFFFF,
		//	StencilOperation stencilFailOp = SOP_KEEP,
		//	StencilOperation depthFailOp = SOP_KEEP,
		//	StencilOperation passOp = SOP_KEEP,
		//	bool twoSidedOperation = false ) = 0;
		//Clip planes
		//virtual void setClipPlanes(const PlaneList& clipPlanes);
		//virtual void resetClipPlanes();

		static Material GetMeshMaterial2( ref RenderSceneData.MeshItem meshItem, RenderSceneData.MeshDataRenderOperation operation, int operationIndex )
		{
			if( meshItem.ReplaceMaterialSelectively != null && operationIndex < meshItem.ReplaceMaterialSelectively.Length )
			{
				var m = meshItem.ReplaceMaterialSelectively[ operationIndex ];
				if( m != null )
					return m;
			}

			if( meshItem.ReplaceMaterial != null )
				return meshItem.ReplaceMaterial;

			var material = operation.Material;

			//should recompile when disposed
			if( material != null && material.Disposed )
			{
				var creator = operation.Creator as MeshGeometry;
				if( creator != null )
				{
					var mesh = creator.ParentMesh;
					if( mesh != null )
						mesh.ShouldRecompile = true;
				}
			}

			return material;
		}

		public static Material.CompiledMaterialData GetMeshMaterialData( ref RenderSceneData.MeshItem meshItem, RenderSceneData.MeshDataRenderOperation operation, int operationIndex, bool materialDataMustBePrepared )
		{
			var materialData = GetMeshMaterial2( ref meshItem, operation, operationIndex )?.Result;

			//check for updated during rendering frame
			if( materialData != null && materialDataMustBePrepared && materialData.currentFrameIndex == -1 )
				materialData = null;

			//null material
			if( materialData == null )
				materialData = ResourceUtility.MaterialNull.Result;

			//invalid material
			if( !string.IsNullOrEmpty( materialData.error ) )
				materialData = ResourceUtility.MaterialInvalid.Result;

			return materialData;
		}

		//static Material.CompiledMaterialData GetMeshMaterialData( ref RenderSceneData.MeshItem meshItem, RenderSceneData.MeshDataRenderOperation operation, int operationIndex, bool materialDataMustBePrepared )
		//{
		//	Material.CompiledMaterialData materialData = null;

		//	if( meshItem.ReplaceMaterialSelectively != null && operationIndex < meshItem.ReplaceMaterialSelectively.Length )
		//	{
		//		var m = meshItem.ReplaceMaterialSelectively[ operationIndex ];
		//		if( m != null )
		//			materialData = m.Result;
		//	}
		//	if( meshItem.ReplaceMaterial != null )
		//		materialData = meshItem.ReplaceMaterial.Result;

		//	if( materialData == null && operation.Material != null )
		//		materialData = operation.Material.Result;

		//	//null material
		//	if( materialData == null )
		//		materialData = ResourceUtility.MaterialStandardNull.Result;

		//	//invalid material
		//	if( !string.IsNullOrEmpty( materialData.error ) )
		//		materialData = ResourceUtility.MaterialStandardInvalid.Result;

		//	return materialData;
		//}

		static Material GetBillboardMaterial2( ref RenderSceneData.BillboardItem billboardItem )
		{
			return billboardItem.Material;
		}

		public static Material.CompiledMaterialData GetBillboardMaterialData( ref RenderSceneData.BillboardItem billboardItem, bool materialDataMustBePrepared )
		{
			var materialData = GetBillboardMaterial2( ref billboardItem )?.Result;

			//check for updated during rendering frame
			if( materialData != null && materialDataMustBePrepared && materialData.currentFrameIndex == -1 )
				materialData = null;

			//null material
			if( materialData == null )
				materialData = ResourceUtility.MaterialNull.Result;

			//invalid material
			if( !string.IsNullOrEmpty( materialData.error ) )
				materialData = ResourceUtility.MaterialInvalid.Result;

			return materialData;
		}

		static Material GetDecalMaterial2( ref RenderSceneData.DecalItem decalItem )
		{
			return decalItem.Material;
		}

		static Material.CompiledMaterialData GetDecalMaterialData( ref RenderSceneData.DecalItem decalItem, bool materialDataMustBePrepared )
		{
			var materialData = GetDecalMaterial2( ref decalItem )?.Result;

			//check for updated during rendering frame
			if( materialData != null && materialDataMustBePrepared && materialData.currentFrameIndex == -1 )
				materialData = null;

			//null material
			if( materialData == null )
				materialData = ResourceUtility.MaterialNull.Result;

			//invalid material
			if( !string.IsNullOrEmpty( materialData.error ) )
				materialData = ResourceUtility.MaterialInvalid.Result;

			return materialData;
		}

		static Material.CompiledMaterialData GetLayerMaterial2( ref RenderSceneData.LayerItem layerItem )
		{
			return layerItem.ResultMaterial;
		}

		static Material.CompiledMaterialData GetLayerMaterialData( ref RenderSceneData.LayerItem layerItem, bool materialDataMustBePrepared )
		{
			var materialData = GetLayerMaterial2( ref layerItem );//?.Result;

			//check for updated during rendering frame
			if( materialData != null && materialDataMustBePrepared && materialData.currentFrameIndex == -1 )
				materialData = null;

			////null material
			//if( materialData == null )
			//	materialData = ResourceUtility.MaterialNull.Result;

			//invalid material
			if( materialData != null && !string.IsNullOrEmpty( materialData.error ) )
				materialData = ResourceUtility.MaterialInvalid.Result;

			return materialData;
		}

		unsafe void LightPrepareShadows( ViewportRenderingContext context, FrameData frameData, LightItem lightItem )
		{
			//var useEVSM = RenderingSystem.ShadowTechnique == ProjectSettingsPage_Rendering.ShadowTechniqueEnum.ExponentialVarianceShadowMaps;

			//init default shadow caster
			InitDefaultShadowCasterData();

			var lightData = lightItem.data;

			//texture size
			int textureSize;
			{
				ShadowTextureSize textureSizeEnum;
				if( lightData.Type == Light.TypeEnum.Spotlight )
					textureSizeEnum = ShadowSpotlightTextureSize.Value;
				else if( lightData.Type == Light.TypeEnum.Point )
					textureSizeEnum = ShadowPointLightTextureSize.Value;
				else
					textureSizeEnum = ShadowDirectionalLightTextureSize.Value;
				textureSize = int.Parse( textureSizeEnum.ToString().Replace( "_", "" ) );
			}

			//texture format
			var textureFormat = RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4 ? PixelFormat.A8R8G8B8 : PixelFormat.Float32R;
			//var textureFormat = /*useEVSM ? PixelFormat.Float32GR :*/ PixelFormat.Float32R;

			//create render target
			ImageComponent shadowTexture = null;
			if( lightData.Type == Light.TypeEnum.Point )
				shadowTexture = context.RenderTargetCube_Alloc( new Vector2I( textureSize, textureSize ), textureFormat );
			else if( lightData.Type == Light.TypeEnum.Spotlight )
				shadowTexture = context.RenderTarget2D_Alloc( new Vector2I( textureSize, textureSize ), textureFormat );
			else if( lightData.Type == Light.TypeEnum.Directional )
				shadowTexture = context.RenderTarget2D_Alloc( new Vector2I( textureSize, textureSize ), textureFormat, arrayLayers: ShadowDirectionalLightCascades );

			//create depth texture
			var shadowTextureDepth = context.RenderTarget2D_Alloc( new Vector2I( textureSize, textureSize ), PixelFormat.Depth24S8 );

			if( lightItem.data.Type == Light.TypeEnum.Directional )
				lightItem.shadowCascadesProjectionViewMatrices = new (Matrix4F, Matrix4F)[ ShadowDirectionalLightCascades ];

			int iterationCount = 1;
			if( lightData.Type == Light.TypeEnum.Point )
				iterationCount = 6;
			if( lightData.Type == Light.TypeEnum.Directional )
				iterationCount = ShadowDirectionalLightCascades;

			for( int nIteration = 0; nIteration < iterationCount; nIteration++ )
			{
				Viewport shadowViewport = shadowTexture.Result.GetRenderTarget( 0, nIteration ).Viewports[ 0 ];

				//camera settings
				if( lightData.Type == Light.TypeEnum.Spotlight )
				{
					//spotlight
					Degree fov = lightData.SpotlightOuterAngle * 1.05;
					if( fov > 179 )
						fov = 179;
					Vector3 dir = lightData.Rotation.GetForward();
					Vector3 up = lightData.Rotation.GetUp();

					shadowViewport.CameraSettings = new Viewport.CameraSettingsClass( shadowViewport, 1, fov, context.Owner.CameraSettings.NearClipDistance, lightData.AttenuationFar * 1.05, lightData.Position, dir, up, ProjectionType.Perspective, 1, 0, 0 );
				}
				else if( lightData.Type == Light.TypeEnum.Point )
				{
					//point light

					Vector3 dir = Vector3.Zero;
					Vector3 up = Vector3.Zero;

					//flipped
					switch( nIteration )
					{
					case 0: dir = -Vector3.YAxis; up = Vector3.ZAxis; break;
					case 1: dir = Vector3.YAxis; up = Vector3.ZAxis; break;
					case 2: dir = Vector3.ZAxis; up = -Vector3.XAxis; break;
					case 3: dir = -Vector3.ZAxis; up = Vector3.XAxis; break;
					case 4: dir = Vector3.XAxis; up = Vector3.ZAxis; break;
					case 5: dir = -Vector3.XAxis; up = Vector3.ZAxis; break;
					}

					//PC: OriginBottomLeft = false, HomogeneousDepth = false
					//Android: OriginBottomLeft = true, HomogeneousDepth = true
					var caps = RenderingSystem.Capabilities;
					if( caps.OriginBottomLeft )
					{
						switch( nIteration )
						{
						case 2: dir = -Vector3.ZAxis; up = Vector3.XAxis; break;
						case 3: dir = Vector3.ZAxis; up = -Vector3.XAxis; break;
						}
					}

					shadowViewport.CameraSettings = new Viewport.CameraSettingsClass( shadowViewport, 1, 90, context.Owner.CameraSettings.NearClipDistance, lightData.AttenuationFar * 1.05, lightData.Position, dir, up, ProjectionType.Perspective, 1, 0, 0 );
				}
				else
				{
					//directional light

					Vector3 dir = lightData.Rotation.GetForward();
					Vector3[] cornerPoints = GetDirectionalLightShadowsCameraCornerPoints( context, nIteration );
					Vector3 destinationPoint = GetDirectionalLightCameraDestinationPoint( context, cornerPoints );

					var pos = destinationPoint - dir * ShadowDirectionalLightExtrusionDistance;

					//!!!!пока так
					var up = Vector3.ZAxis;
					if( Math.Abs( Vector3.Dot( up, dir ) ) >= .99f )
						up = Vector3.YAxis;
					//up = lightData.transform.Rotation.GetUp();

					double maxDistance = 0;
					foreach( Vector3 point in cornerPoints )
					{
						double distance = ( point - destinationPoint ).Length();
						if( distance > maxDistance )
							maxDistance = distance;
					}

					var orthoSize = maxDistance * 2 * 1.05f;
					//fix epsilon error
					orthoSize = ( (int)( orthoSize / 5 ) ) * 5 + 5;

					//fix jittering
					{
						//!!!!good?
						Quaternion lightRotation = Quaternion.FromDirectionZAxisUp( dir );
						//Quat lightRotation = lightData.transform.Rotation;

						//convert world space camera position into light space
						Vector3 lightSpacePos = lightRotation.GetInverse() * pos;

						//snap to nearest texel
						//!!!!good?
						double worldTexelSize = orthoSize / textureSize;
						lightSpacePos.Y -= Math.IEEERemainder( lightSpacePos.Y, worldTexelSize );
						lightSpacePos.Z -= Math.IEEERemainder( lightSpacePos.Z, worldTexelSize );

						//convert back to world space
						pos = lightRotation * lightSpacePos;
					}

					double shadowMapFarClipDistance = ShadowDirectionalLightExtrusionDistance * 2;

					shadowViewport.CameraSettings = new Viewport.CameraSettingsClass( shadowViewport, 1, 90,
						context.Owner.CameraSettings.NearClipDistance, shadowMapFarClipDistance, pos, dir, up,
						ProjectionType.Orthographic, orthoSize, 0, 0 );

					//!!!!double
					lightItem.shadowCascadesProjectionViewMatrices[ nIteration ] =
						(shadowViewport.CameraSettings.ProjectionMatrix.ToMatrix4F(), shadowViewport.CameraSettings.ViewMatrix.ToMatrix4F());
				}

				//set viewport
				//!!!!double
				Matrix4F viewMatrix = shadowViewport.CameraSettings.ViewMatrix.ToMatrix4F();
				Matrix4F projectionMatrix = shadowViewport.CameraSettings.ProjectionMatrix.ToMatrix4F();

				//create target
				var mrt = context.MultiRenderTarget_Create( new[] {
					new MultiRenderTarget.Item( shadowTexture, 0, nIteration ),
					new MultiRenderTarget.Item( shadowTextureDepth ) } );
				context.SetViewport( mrt.Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.All, new ColorValue( 1, 1, 1 ) );
				//context.SetViewport( shadowViewport, viewMatrix, projectionMatrix, FrameBufferTypes.All, new ColorValue( 1, 1, 1 ) );


				//bind textures for all render operations
				BindMaterialsTexture( context, frameData );


				//bind general parameters
				{
					var container = new ParameterContainer();

					//Mat4F worldViewProjMatrix = projectionMatrix * viewMatrix * worldMatrix;
					//Mat4F viewProjMatrix = projectionMatrix * viewMatrix;

					//!!!!
					Vector2F texelOffsets = new Vector2F( -0.5f, -0.5f ) / textureSize;

					//!!!!
					//texelOffsets *= lightData.ShadowNormalBias;
					////!!!!
					//texelOffsets = Vec2F.Zero;

					container.Set( "u_shadowTexelOffsets", ParameterType.Vector2, 1, &texelOffsets, sizeof( Vector2F ) );
					//Vec2F texelOffsets = new Vec2F( -0.5f, -0.5f );
					//Vec4F texelOffsets2 = new Vec4F( texelOffsets.X, texelOffsets.Y, texelOffsets.X / textureSize, texelOffsets.Y / textureSize );
					//generalContainer.Set( "texelOffsets", ParameterType.Vector4, 1, &texelOffsets, sizeof( Vec4F ) );

					//!!!!double
					Vector3F cameraPosition = shadowViewport.CameraSettings.Position.ToVector3F();
					container.Set( "u_cameraPosition", ParameterType.Vector3, 1, &cameraPosition, sizeof( Vector3F ) );

					float farClipDistance = (float)shadowViewport.CameraSettings.FarClipDistance;
					container.Set( "u_farClipDistance", ParameterType.Float, 1, &farClipDistance, sizeof( float ) );

					var maskFactor = (float)ShadowMaterialOpacityMaskThresholdFactor;
					container.Set( "u_shadowMaterialOpacityMaskThresholdFactor", ParameterType.Float, 1, &maskFactor, sizeof( float ) );

					//generalContainer.Set( "u_shadowBias", ParameterType.Vector2, 1, &shadowBias, sizeof( Vector2F ) );
					//Vec2F bias;
					//if( lightItem.data.type == Light.TypeEnum.Spotlight )
					//	bias = shadowLightBiasSpotLight.ToVec2F();
					//else if( lightItem.data.type == Light.TypeEnum.Point )
					//	bias = shadowLightBiasPointLight.ToVec2F();
					//else
					//	bias = shadowLightBiasDirectionalLight.ToVec2F();
					//generalContainer.Set( "u_shadowBias", ParameterType.Vector2, 1, &bias, sizeof( Vec2F ) );

					context.BindParameterContainer( container );
				}

				if( lightItem.shadowsAffectedRenderableGroups != null && lightItem.shadowsAffectedRenderableGroups[ nIteration ] != null )
				{
					//convert to array
					var shadowsAffectedRenderableGroups = lightItem.shadowsAffectedRenderableGroups[ nIteration ].ToArray();
					//var shadowsAffectedRenderableGroups = lightItem.shadowsAffectedRenderableGroups[ nIteration ];

					//sort by distance
					{
						Vector3 lightPosition;
						if( lightItem.data.Type == Light.TypeEnum.Directional )
						{
							lightPosition = context.Owner.CameraSettings.Position - lightItem.data.Rotation.ToQuaternion().GetForward() * 10000.0;
							//lightPosition = -lightItem.data.Rotation.ToQuaternion().GetForward() * 1000000.0;
						}
						else
							lightPosition = lightItem.data.Position;

						//!!!!maybe alloc native memory. где еще
						var meshDistances = new float[ frameData.Meshes.Count ];
						var billboardDistances = new float[ frameData.Billboards.Count ];
						foreach( var renderableGroup in shadowsAffectedRenderableGroups )
						{
							var index = renderableGroup;
							switch( renderableGroup.X )
							{
							case 0:
								meshDistances[ renderableGroup.Y ] = frameData.GetObjectGroupDistanceToPointSquared( ref index, ref lightPosition );
								break;
							case 1:
								billboardDistances[ renderableGroup.Y ] = frameData.GetObjectGroupDistanceToPointSquared( ref index, ref lightPosition );
								break;
							}
						}

						float GetDistanceSquared( ref Vector2I index )
						{
							if( index.X == 0 )
								return meshDistances[ index.Y ];
							else
								return billboardDistances[ index.Y ];
						}

						CollectionUtility.MergeSort( shadowsAffectedRenderableGroups, delegate ( Vector2I item1, Vector2I item2 )
						{
							var distanceSqr1 = GetDistanceSquared( ref item1 );
							var distanceSqr2 = GetDistanceSquared( ref item2 );
							if( distanceSqr1 < distanceSqr2 )
								return -1;
							if( distanceSqr1 > distanceSqr2 )
								return 1;
							return 0;
						}, true );
					}

					for( int nSector = 0; nSector < sectorCountByDistance; nSector++ )
					{
						int indexFrom = (int)( (float)shadowsAffectedRenderableGroups.Length * nSector / sectorCountByDistance );
						int indexTo = (int)( (float)shadowsAffectedRenderableGroups.Length * ( nSector + 1 ) / sectorCountByDistance );
						if( nSector == sectorCountByDistance - 1 )
							indexTo = shadowsAffectedRenderableGroups.Length;

						try
						{
							//fill output manager
							for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
							{
								var renderableGroup = shadowsAffectedRenderableGroups[ nRenderableGroup ];

								if( renderableGroup.X == 0 )
								{
									//meshes

									ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
									var meshData = meshItem.MeshData;

									for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
									{
										var oper = meshData.RenderOperations[ nOperation ];

										//!!!!все ли операции рисуют тени?

										//get special shadow caster if available
										var materialData = GetMeshMaterialData( ref meshItem, oper, nOperation, true );
										if( materialData.specialShadowCasterData == null )
											materialData = ResourceUtility.MaterialNull.Result;

										bool instancing = Instancing && meshItem.AnimationData == null && meshItem.BatchingInstanceBuffer == null && meshItem.CutVolumes == null;

										outputInstancingManager.Add( renderableGroup, nOperation, oper, materialData, instancing );
									}
								}
								else if( renderableGroup.X == 1 )
								{
									//billboards

									ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];
									var meshData = Billboard.GetBillboardMesh().Result.MeshData;

									for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
									{
										var oper = meshData.RenderOperations[ nOperation ];

										var materialData = GetBillboardMaterialData( ref billboardItem, true );
										if( materialData.specialShadowCasterData == null )
											materialData = ResourceUtility.MaterialNull.Result;

										//!!!!или если уже собран из GroupOfObjects

										bool instancing = Instancing && billboardItem.CutVolumes == null;
										outputInstancingManager.Add( renderableGroup, nOperation, oper, materialData, instancing );
									}
								}
							}

							outputInstancingManager.Prepare();

							//render output items
							{
								Material.CompiledMaterialData materialBinded = null;

								var outputItems = outputInstancingManager.outputItems;
								for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
								{
									ref var outputItem = ref outputItems.Data[ nOutputItem ];
									var materialData = outputItem.materialData;

									//get shadow caster data
									var specialShadowCasterData = materialData.specialShadowCasterData;
									ShadowCasterData shadowCasterData = specialShadowCasterData;
									if( shadowCasterData == null )
										shadowCasterData = defaultShadowCasterData;
									var passGroup = shadowCasterData.passByLightType[ (int)lightData.Type ];

									//render operation
									if( Instancing && outputItem.renderableItemsCount >= InstancingMinCount )
									{
										//with instancing

										//bind material data
										if( specialShadowCasterData != null )
										{
											bool materialWasChanged = materialBinded != materialData;
											materialBinded = materialData;
											materialData.BindCurrentFrameData( context, true, materialWasChanged );
										}

										GpuMaterialPass pass = null;

										//bind operation data
										var firstRenderableItem = outputItem.renderableItemFirst;
										if( firstRenderableItem.X == 0 )
										{
											//meshes
											ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ firstRenderableItem.Y ];
											var meshData = meshItem.MeshData;

											BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.BillboardDataMode, outputItem.operation.BillboardDataImage, outputItem.operation.MeshGeometryIndex );

											pass = passGroup.Get( meshData.BillboardMode != 0 );
										}
										else if( firstRenderableItem.X == 1 )
										{
											//billboards
											ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ firstRenderableItem.Y ];
											var meshData = Billboard.GetBillboardMesh().Result.MeshData;

											BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItem.operation.BillboardDataMode, outputItem.operation.BillboardDataImage, outputItem.operation.MeshGeometryIndex );

											pass = passGroup.Billboard;
										}

										//no sense to use RenderSceneData.InstancingObjectData
										int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
										int instanceCount = outputItem.renderableItemsCount;

										if( InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) == instanceCount )
										{
											var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );

											//get instancing matrices
											RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
											int currentMatrix = 0;
											for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
											{
												var renderableItem = outputItem.renderableItems[ nRenderableItem ];

												if( renderableItem.X == 0 )
												{
													//meshes
													ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
													meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
												}
												else if( renderableItem.X == 1 )
												{
													//billboards
													ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
													billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
												}
											}

											RenderOperation( context, outputItem.operation, pass, null, null, ref instanceBuffer, instanceCount );
										}
										//else
										//	Log.Fatal( "InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) != instanceCount." );
									}
									else
									{
										//without instancing

										for( int nRenderableItem = 0; nRenderableItem < outputItem.renderableItemsCount; nRenderableItem++ )
										{
											Vector3I renderableItem;
											if( nRenderableItem != 0 )
												renderableItem = outputItem.renderableItems[ nRenderableItem ];
											else
												renderableItem = outputItem.renderableItemFirst;

											//bind material data
											if( specialShadowCasterData != null )
											{
												bool materialWasChanged = materialBinded != materialData;
												materialBinded = materialData;
												materialData.BindCurrentFrameData( context, true, materialWasChanged && nRenderableItem == 0 );
											}

											//bind render operation data, set matrix
											if( renderableItem.X == 0 )
											{
												//meshes
												ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
												var meshData = meshItem.MeshData;

												var batchInstancing = meshItem.BatchingInstanceBuffer != null;

												BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, batchInstancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.BillboardDataMode, outputItem.operation.BillboardDataImage, outputItem.operation.MeshGeometryIndex );

												if( !batchInstancing )
													fixed( Matrix4F* p = &meshItem.Transform )
														Bgfx.SetTransform( (float*)p );

												var pass = passGroup.Get( meshData.BillboardMode != 0 );

												RenderOperation( context, outputItem.operation, pass, null, meshItem.CutVolumes, meshItem.BatchingInstanceBuffer );
											}
											else if( renderableItem.X == 1 )
											{
												//billboards
												ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
												var meshData = Billboard.GetBillboardMesh().Result.MeshData;

												BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItem.operation.BillboardDataMode, outputItem.operation.BillboardDataImage, outputItem.operation.MeshGeometryIndex );

												billboardItem.GetWorldMatrix( out var worldMatrix );
												Bgfx.SetTransform( (float*)&worldMatrix );

												var pass = passGroup.Billboard;

												RenderOperation( context, outputItem.operation, pass, null, billboardItem.CutVolumes );
											}
										}
									}
								}
							}
						}
						finally
						{
							outputInstancingManager.Clear();
						}
					}




					//Material.CompiledMaterialData materialBinded = null;

					////render each mesh
					//foreach( var renderableGroup in shadowsAffectedRenderableGroups )
					//{
					//	switch( renderableGroup.X )
					//	{
					//	case 0:
					//		{
					//			//mesh

					//			ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
					//			var meshData = meshItem.MeshData;

					//			for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
					//			{
					//				var oper = meshData.RenderOperations[ nOperation ];

					//				//!!!!все ли операции рисуют тени?

					//				Matrix4F worldMatrix = meshItem.MeshInstanceOne;

					//				//get special shadow caster if available
					//				var materialData = GetMeshMaterialData( ref meshItem, oper, nOperation, true );
					//				var specialShadowCasterData = materialData.specialShadowCasterData;

					//				//get shadow caster data
					//				ShadowCasterData shadowCasterData = specialShadowCasterData;
					//				if( shadowCasterData == null )
					//					shadowCasterData = defaultShadowCasterData;

					//				//!!!!instancing

					//				if( specialShadowCasterData != null )
					//				{
					//					bool materialWasChanged = materialBinded != materialData;
					//					materialBinded = materialData;

					//					materialData.BindCurrentFrameData( context, true, materialWasChanged, true );
					//				}

					//				BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, false, meshItem.AnimationData, meshData.BillboardMode, meshData.SpaceBounds.BoundingSphere.Value.Radius );

					//				//render
					//				{
					//					var pass = shadowCasterData.passByLightType[ (int)lightData.Type ];

					//					Bgfx.SetTransform( (float*)&worldMatrix );
					//					RenderOperation( context, oper, pass, null );
					//				}
					//			}
					//		}
					//		break;

					//	case 1:
					//		{
					//			//billboard

					//			ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];

					//			var meshData = Billboard.GetBillboardMesh().Result.MeshData;

					//			for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
					//			{
					//				var oper = meshData.RenderOperations[ nOperation ];

					//				var billboard = billboardItem.BillboardOne.Value;

					//				//!!!!!double, relative camera position
					//				var worldMatrix = new Matrix4F( Matrix3F.FromScale( new Vector3F( 1, billboard.Size.X, billboard.Size.Y ) ), billboard.Position );

					//				//get special shadow caster if available
					//				var materialData = GetBillboardMaterialData( ref billboardItem, true );
					//				var specialShadowCasterData = materialData.specialShadowCasterData;

					//				//get shadow caster data
					//				ShadowCasterData shadowCasterData = specialShadowCasterData;
					//				if( shadowCasterData == null )
					//					shadowCasterData = defaultShadowCasterData;

					//				if( specialShadowCasterData != null )
					//				{
					//					bool materialWasChanged = materialBinded != materialData;
					//					materialBinded = materialData;

					//					materialData.BindCurrentFrameData( context, true, materialWasChanged, true );
					//				}

					//				BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, false, null, meshData.BillboardMode, meshData.SpaceBounds.BoundingSphere.Value.Radius );

					//				//render
					//				{
					//					var pass = shadowCasterData.passByLightType[ (int)lightData.Type ];

					//					Bgfx.SetTransform( (float*)&worldMatrix );
					//					RenderOperation( context, oper, pass, null );
					//				}
					//			}
					//		}
					//		break;
					//	}
					//}
				}
			}

			////blur
			//if( useEVSM )
			//{
			//	//!!!!Point

			//	if( lightData.Type == Light.TypeEnum.Spotlight || lightData.Type == Light.TypeEnum.Directional )
			//	{
			//		//!!!!

			//		//!!!!если кубемапа

			//		var blur = lightData.SourceRadiusOrAngle;//!!!! ShadowSoftness * ShadowSoftness;

			//		//var blur = 1.0;
			//		//if( EngineApp._DebugCapsLock )
			//		//	blur /= 4;

			//		//!!!!пересмотреть везде GaussianBlur

			//		//!!!!
			//		if( blur > 0 )
			//			GaussianBlurShadowVSM( context, lightData.Type, shadowTexture, blur );
			//		//var blurTexture = GaussianBlur( context, null, shadowTexture, blur, DownscalingModeEnum.Manual, 0 );
			//		//var blurTexture = GaussianBlur2( context, null, shadowTexture, blur, DownscalingModeEnum.Manual, 0 );

			//		//int nIteration = 0;

			//		//Viewport blurViewport = blurTexture.Result.GetRenderTarget( 0, nIteration ).Viewports[ 0 ];

			//		//Viewport shadowViewport = shadowTexture.Result.GetRenderTarget().Viewports[ 0 ];
			//		//blurViewport.CameraSettings = shadowViewport.CameraSettings;

			//		//context.DynamicTexture_Free( shadowTexture );

			//		//shadowTexture = blurTexture;
			//	}
			//}

			//add to namedTextures
			{
				var prefix = "shadow" + lightData.Type.ToString();
				for( int counter = 1; ; counter++ )
				{
					var name = prefix + counter.ToString();
					if( !context.ObjectsDuringUpdate.namedTextures.ContainsKey( name ) )
					{
						context.ObjectsDuringUpdate.namedTextures[ name ] = shadowTexture;
						break;
					}
				}
			}

			//connect the shadow texture with the light
			lightItem.shadowTexture = shadowTexture;


			context.DynamicTexture_Free( shadowTextureDepth );
		}

		protected unsafe virtual void RenderSky( ViewportRenderingContext context, FrameData frameData )
		{
			frameData.Sky?.Render( this, context, frameData );
		}

		protected unsafe virtual void Render3DSceneDeferred( ViewportRenderingContext context, FrameData frameData )
		{
			Viewport viewportOwner = context.Owner;

			//get renderableGroupsToDraw
			Vector2I[] renderableGroupsToDraw;
			{
				var list = new OpenList<Vector2I>( frameData.RenderableGroupsInFrustum.Count );

				foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
				{
					bool add = false;

					if( renderableGroup.X == 0 )
					{
						ref var data = ref frameData.Meshes.Data[ renderableGroup.Y ];
						add = ( data.Flags & FrameData.MeshItem.FlagsEnum.UseDeferred ) != 0;
					}
					else
					{
						ref var data = ref frameData.Billboards.Data[ renderableGroup.Y ];
						add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseDeferred ) != 0;
					}

					if( add )
						list.Add( renderableGroup );
				}

				renderableGroupsToDraw = list.ToArray();
			}

			if( renderableGroupsToDraw.Length == 0 )
				return;

			//sort by distance
			CollectionUtility.MergeSort( renderableGroupsToDraw, delegate ( Vector2I a, Vector2I b )
			{
				var distanceASquared = frameData.GetObjectGroupDistanceToCameraSquared( ref a );
				var distanceBSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref b );
				if( distanceASquared < distanceBSquared )
					return -1;
				if( distanceASquared > distanceBSquared )
					return 1;
				return 0;
			}, true );


			//bind textures for all render operations
			BindMaterialsTexture( context, frameData );


			for( int nSector = 0; nSector < sectorCountByDistance; nSector++ )
			{
				int indexFrom = (int)( (float)renderableGroupsToDraw.Length * nSector / sectorCountByDistance );
				int indexTo = (int)( (float)renderableGroupsToDraw.Length * ( nSector + 1 ) / sectorCountByDistance );
				if( nSector == sectorCountByDistance - 1 )
					indexTo = renderableGroupsToDraw.Length;

				try
				{
					//fill output manager
					for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
					{
						var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

						if( renderableGroup.X == 0 )
						{
							//meshes

							ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
							var meshData = meshItem.MeshData;

							if( !meshItem.OnlyForShadowGeneration )
							{
								for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
								{
									var oper = meshData.RenderOperations[ nOperation ];

									var materialData = GetMeshMaterialData( ref meshItem, oper, nOperation, true );
									if( materialData.deferredShadingSupport )
									{
										bool instancing = Instancing && meshItem.AnimationData == null && meshItem.BatchingInstanceBuffer == null && meshItem.CutVolumes == null;
										outputInstancingManager.Add( renderableGroup, nOperation, oper, materialData, instancing );
									}
								}
							}
						}
						else if( renderableGroup.X == 1 )
						{
							//billboards

							ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];
							var meshData = Billboard.GetBillboardMesh().Result.MeshData;

							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
							{
								var oper = meshData.RenderOperations[ nOperation ];

								var materialData = GetBillboardMaterialData( ref billboardItem, true );
								if( materialData.deferredShadingSupport )
								{
									bool instancing = Instancing && billboardItem.CutVolumes == null;
									outputInstancingManager.Add( renderableGroup, nOperation, oper, materialData, instancing );
								}
							}
						}
					}

					outputInstancingManager.Prepare();

					//render output items
					{
						Material.CompiledMaterialData materialBinded = null;

						var outputItems = outputInstancingManager.outputItems;
						for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
						{
							ref var outputItem = ref outputItems.Data[ nOutputItem ];
							var materialData = outputItem.materialData;

							//material
							//var pass = materialData.deferredShadingPass;
							bool materialWasChanged = materialBinded != materialData;
							materialBinded = materialData;

							if( Instancing && outputItem.renderableItemsCount >= InstancingMinCount )
							{
								//with instancing

								//bind material data
								materialData.BindCurrentFrameData( context, false, materialWasChanged );

								GpuMaterialPass pass = null;

								//bind operation data
								var firstRenderableItem = outputItem.renderableItemFirst;
								if( firstRenderableItem.X == 0 )
								{
									//meshes
									ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ firstRenderableItem.Y ];
									var meshData = meshItem.MeshData;

									BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.BillboardDataMode, outputItem.operation.BillboardDataImage, outputItem.operation.MeshGeometryIndex );

									pass = materialData.deferredShadingPass.Get( meshData.BillboardMode != 0 );
								}
								else if( firstRenderableItem.X == 1 )
								{
									//billboards
									ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ firstRenderableItem.Y ];
									var meshData = Billboard.GetBillboardMesh().Result.MeshData;

									BindRenderOperationData( context, frameData, materialData, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItem.operation.BillboardDataMode, outputItem.operation.BillboardDataImage, outputItem.operation.MeshGeometryIndex );

									pass = materialData.deferredShadingPass.Billboard;
								}

								int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
								int instanceCount = outputItem.renderableItemsCount;

								if( InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) == instanceCount )
								{
									var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );

									//get instancing matrices
									RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
									int currentMatrix = 0;
									for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
									{
										var renderableItem = outputItem.renderableItems[ nRenderableItem ];

										if( renderableItem.X == 0 )
										{
											//meshes
											ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
											meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
										}
										else if( renderableItem.X == 1 )
										{
											//billboards
											ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
											billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
										}
									}

									RenderOperation( context, outputItem.operation, pass, null, null, ref instanceBuffer, instanceCount );
								}
								//else
								//	Log.Fatal( "InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) != instanceCount." );
							}
							else
							{
								//without instancing

								for( int nRenderableItem = 0; nRenderableItem < outputItem.renderableItemsCount; nRenderableItem++ )
								{
									Vector3I renderableItem;
									if( nRenderableItem != 0 )
										renderableItem = outputItem.renderableItems[ nRenderableItem ];
									else
										renderableItem = outputItem.renderableItemFirst;

									//bind material data
									materialData.BindCurrentFrameData( context, false, materialWasChanged && nRenderableItem == 0 );

									//bind render operation data, set matrix
									if( renderableItem.X == 0 )
									{
										//meshes
										ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
										var meshData = meshItem.MeshData;

										var batchInstancing = meshItem.BatchingInstanceBuffer != null;

										BindRenderOperationData( context, frameData, materialData, batchInstancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.BillboardDataMode, outputItem.operation.BillboardDataImage, outputItem.operation.MeshGeometryIndex );

										if( !batchInstancing )
											fixed( Matrix4F* p = &meshItem.Transform )
												Bgfx.SetTransform( (float*)p );

										var pass = materialData.deferredShadingPass.Get( meshData.BillboardMode != 0 );

										RenderOperation( context, outputItem.operation, pass, null, meshItem.CutVolumes, meshItem.BatchingInstanceBuffer );
									}
									else if( renderableItem.X == 1 )
									{
										//billboards
										ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
										var meshData = Billboard.GetBillboardMesh().Result.MeshData;

										BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItem.operation.BillboardDataMode, outputItem.operation.BillboardDataImage, outputItem.operation.MeshGeometryIndex );

										billboardItem.GetWorldMatrix( out var worldMatrix );
										Bgfx.SetTransform( (float*)&worldMatrix );

										var pass = materialData.deferredShadingPass.Billboard;

										RenderOperation( context, outputItem.operation, pass, null, billboardItem.CutVolumes );
									}
								}
							}
						}
					}
				}
				finally
				{
					outputInstancingManager.Clear();
				}

				//render layers
				if( DebugDrawLayers )
				{
					Material.CompiledMaterialData materialBinded = null;

					for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
					{
						var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

						if( renderableGroup.X == 0 )
						{
							//meshes

							ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
							var meshData = meshItem.MeshData;

							if( meshItem.Layers != null )
							{
								var batchInstancing = meshItem.BatchingInstanceBuffer != null;

								for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
								{
									ref var layer = ref meshItem.Layers[ nLayer ];
									var materialData = GetLayerMaterialData( ref layer, true );
									if( materialData == null )
										continue;

									if( materialData.deferredShadingPass.Usual != null )
									{
										bool materialWasChanged = materialBinded != materialData;
										materialBinded = materialData;

										//bind material data
										materialData.BindCurrentFrameData( context, false, materialWasChanged );
										materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );

										if( !batchInstancing )
											fixed( Matrix4F* p = &meshItem.Transform )
												Bgfx.SetTransform( (float*)p );

										var color = /*meshItem.Color * */ layer.MaterialColor;

										var pass = materialData.deferredShadingPass.Get( meshData.BillboardMode != 0 );

										for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
										{
											var oper = meshData.RenderOperations[ nOperation ];

											BindRenderOperationData( context, frameData, materialData, batchInstancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, 0, null, oper.MeshGeometryIndex );

											RenderOperation( context, oper, pass, null, meshItem.CutVolumes, meshItem.BatchingInstanceBuffer );
										}
									}
								}
							}
						}
					}
				}

			}


			////render instance groups
			//{
			//	Material.CompiledMaterialData materialBinded = null;

			//	foreach( var pair in instanceGroups )
			//	{
			//		var key = pair.Key;
			//		var instanceGroup = pair.Value;
			//		var operation = key.Item1;
			//		var materialData = key.Item2;
			//		//var operation = operations[ key.X ];
			//		//var materialData = materialDatas[ key.Y ];

			//		if( true )
			//		{

			//			var pass = materialData.deferredShadingPass;

			//			//bind material data
			//			{
			//				bool materialWasChanged = materialBinded != materialData;
			//				materialBinded = materialData;

			//				materialData.BindCurrentFrameData( context, false, materialWasChanged, true );

			//				//!!!!
			//				BindRenderOperationData( context, frameData, materialData, true, null, 0, 0 );
			//			}

			//			//!!!!
			//			int instanceStride = sizeof( _Matrix3x4F );
			//			int numInstances = instanceGroup.renderableGroups.Count;


			//			//!!!!
			//			if( InstanceDataBuffer.GetAvailableSpace( numInstances, instanceStride ) != numInstances )
			//				Log.Fatal( "InstanceDataBuffer.GetAvailableSpace( numInstances, instanceStride ) != numInstances" );
			//			var instanceBuffer = new InstanceDataBuffer( numInstances, instanceStride );

			//			_Matrix3x4F* pMatrices = (_Matrix3x4F*)instanceBuffer.Data;
			//			int currentMatrix = 0;

			//			foreach( var renderableGroup in instanceGroup.renderableGroups )
			//			{
			//				if( renderableGroup.X == 0 )
			//				{
			//					//meshes

			//					ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
			//					//var meshData = meshItem.MeshData;

			//					ref Matrix4F worldMatrix = ref meshItem.MeshInstanceOne;
			//					worldMatrix.GetTranspose( out pMatrices[ currentMatrix ] );
			//					currentMatrix++;
			//				}
			//				else if( renderableGroup.X == 1 )
			//				{
			//					//billboards

			//					ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];
			//					//var meshData = Billboard.GetBillboardMesh().Result.MeshData;

			//					var billboard = billboardItem.BillboardOne.Value;

			//					//!!!!!double, relative camera position
			//					var worldMatrix = new Matrix4F( Matrix3F.FromScale( new Vector3F( 1, billboard.Size.X, billboard.Size.Y ) ), billboard.Position );
			//					worldMatrix.GetTranspose( out pMatrices[ currentMatrix ] );
			//					currentMatrix++;
			//				}
			//			}

			//			RenderOperation( context, operation, pass, null, ref instanceBuffer, numInstances );
			//		}
			//		else
			//		{
			//			//!!!!


			//			foreach( var renderableGroup in instanceGroup.renderableGroups )
			//			{
			//				if( renderableGroup.X == 0 )
			//				{
			//					//meshes

			//					ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
			//					var meshData = meshItem.MeshData;

			//					var nOperation = renderableGroup.Z;

			//					var oper = meshData.RenderOperations[ renderableGroup.Z ];
			//					//!!!!
			//					if( oper != operation )
			//						Log.Fatal( "oper != operation." );

			//					if( materialData.deferredShadingSupport )
			//					{
			//						Matrix4F worldMatrix = meshItem.MeshInstanceOne;
			//						var pass = materialData.deferredShadingPass;

			//						bool materialWasChanged = materialBinded != materialData;
			//						materialBinded = materialData;

			//						materialData.BindCurrentFrameData( context, false, materialWasChanged, true );

			//						BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.SpaceBounds.BoundingSphere.Value.Radius );

			//						Bgfx.SetTransform( (float*)&worldMatrix );

			//						RenderOperation( context, oper, pass, null );
			//					}
			//				}
			//				else if( renderableGroup.X == 1 )
			//				{
			//					//billboards

			//					ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];
			//					var meshData = Billboard.GetBillboardMesh().Result.MeshData;

			//					var nOperation = renderableGroup.Z;

			//					var oper = meshData.RenderOperations[ nOperation ];
			//					//!!!!
			//					if( oper != operation )
			//						Log.Fatal( "oper != operation." );

			//					if( materialData.deferredShadingSupport )
			//					{
			//						var billboard = billboardItem.BillboardOne.Value;

			//						//!!!!!double, relative camera position
			//						var worldMatrix = new Matrix4F( Matrix3F.FromScale( new Vector3F( 1, billboard.Size.X, billboard.Size.Y ) ), billboard.Position );

			//						var pass = materialData.deferredShadingPass;

			//						bool materialWasChanged = materialBinded != materialData;
			//						materialBinded = materialData;

			//						materialData.BindCurrentFrameData( context, false, materialWasChanged, true );

			//						BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, meshData.SpaceBounds.BoundingSphere.Value.Radius );

			//						Bgfx.SetTransform( (float*)&worldMatrix );

			//						RenderOperation( context, oper, pass, null );
			//					}
			//				}
			//			}
			//		}
			//	}
			//}



			//if( Instancing )
			//if( true )
			//{


			//}
			//else
			//{
			//	//without instancing

			//	Material.CompiledMaterialData materialBinded = null;

			//	//draw
			//	foreach( var renderableGroup in renderableGroupsToDraw )
			//	{
			//		if( renderableGroup.X == 0 )
			//		{
			//			//meshes

			//			//ref var meshItem2 = ref frameData.Meshes.Data[ renderableGroup.Y ];
			//			//if( ( meshItem2.Flags & FrameData.MeshItem.FlagsEnum.UseDeferred ) != 0 )
			//			//{

			//			ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
			//			var meshData = meshItem.MeshData;

			//			for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
			//			{
			//				var oper = meshData.RenderOperations[ nOperation ];

			//				var materialData = GetMeshMaterialData( ref meshItem, oper, nOperation, true );
			//				if( materialData.deferredShadingSupport )
			//				{
			//					Matrix4F worldMatrix = meshItem.MeshInstanceOne;
			//					var pass = materialData.deferredShadingPass;

			//					bool materialWasChanged = materialBinded != materialData;
			//					materialBinded = materialData;

			//					materialData.BindCurrentFrameData( context, false, materialWasChanged, true );

			//					BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.SpaceBounds.BoundingSphere.Value.Radius );

			//					Bgfx.SetTransform( (float*)&worldMatrix );

			//					RenderOperation( context, oper, pass, null );
			//				}
			//			}

			//			//}
			//		}
			//		else if( renderableGroup.X == 1 )
			//		{
			//			//billboards

			//			//ref var billboardItem2 = ref frameData.Billboards.Data[ renderableGroup.Y ];
			//			//if( ( billboardItem2.Flags & FrameData.BillboardItem.FlagsEnum.UseDeferred ) != 0 )
			//			//{

			//			ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];

			//			var meshData = Billboard.GetBillboardMesh().Result.MeshData;

			//			for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
			//			{
			//				var oper = meshData.RenderOperations[ nOperation ];

			//				var materialData = GetBillboardMaterialData( ref billboardItem, true );
			//				if( materialData.deferredShadingSupport )
			//				{
			//					var billboard = billboardItem.BillboardOne.Value;

			//					//!!!!!double, relative camera position
			//					var worldMatrix = new Matrix4F( Matrix3F.FromScale( new Vector3F( 1, billboard.Size.X, billboard.Size.Y ) ), billboard.Position );

			//					var pass = materialData.deferredShadingPass;

			//					bool materialWasChanged = materialBinded != materialData;
			//					materialBinded = materialData;

			//					materialData.BindCurrentFrameData( context, false, materialWasChanged, true );

			//					BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, meshData.SpaceBounds.BoundingSphere.Value.Radius );

			//					Bgfx.SetTransform( (float*)&worldMatrix );

			//					RenderOperation( context, oper, pass, null );
			//				}
			//			}

			//			//}
			//		}
			//	}
			//}
		}

		void InitDecalMesh()
		{
			decalMesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );
			decalMesh.CreateComponent<MeshGeometry_Box>();
			decalMesh.Enabled = true;
		}

		protected unsafe virtual void RenderDecalsDeferred( ViewportRenderingContext context, FrameData frameData, ImageComponent depthTexture, ImageComponent gBuffer1TextureCopy, ImageComponent gBuffer4TextureCopy, ImageComponent gBuffer5TextureCopy )
		{
			if( frameData.Decals.Count == 0 )
				return;

			if( decalMesh == null )
				InitDecalMesh();

			Uniform u_decalMatrix = GpuProgramManager.RegisterUniform( "u_decalMatrix", UniformType.Matrix4x4, 1 );
			Uniform u_decalNormalTangent = GpuProgramManager.RegisterUniform( "u_decalNormalTangent", UniformType.Vector4, 2 );

			Viewport viewportOwner = context.Owner;

			//!!!!GC
			//!!!!instancing

			var decalsData = frameData.RenderSceneData.Decals;

			var sortOrders = new double[ frameData.Decals.Count ];
			for( int n = 0; n < sortOrders.Length; n++ )
				sortOrders[ n ] = decalsData.Data[ n ].SortOrder;

			var sortedIndexes = new int[ frameData.Decals.Count ];
			for( int n = 0; n < sortOrders.Length; n++ )
				sortedIndexes[ n ] = n;

			if( sortedIndexes.Length == 0 )
				return;

			//sort by sort order
			CollectionUtility.MergeSort( sortedIndexes, delegate ( int a, int b )
			{
				var sortOrderA = sortOrders[ a ];
				var sortOrderB = sortOrders[ b ];
				if( sortOrderA < sortOrderB )
					return -1;
				if( sortOrderA > sortOrderB )
					return 1;
				return 0;
			}, true );

			Material.CompiledMaterialData materialBinded = null;
			var currentDecalNormalTangent0 = new Vector4F( float.MaxValue, 0, 0, 0 );
			var currentDecalNormalTangent1 = new Vector4F( float.MaxValue, 0, 0, 0 );


			//bind textures for all render operations
			BindMaterialsTexture( context, frameData );


			foreach( var decalIndex in sortedIndexes )
			{
				ref var decalData = ref decalsData.Data[ decalIndex ];
				var meshData = decalMesh.Result.MeshData;

				for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
				{
					var oper = meshData.RenderOperations[ nOperation ];

					var materialData = GetDecalMaterialData( ref decalData, true );
					if( materialData.decalSupport )
					{
						//material
						var pass = materialData.decalShadingPass;
						bool materialWasChanged = materialBinded != materialData;
						materialBinded = materialData;

						//bind material data
						materialData.BindCurrentFrameData( context, false, materialWasChanged );

						//var receiveAnotherDecals = true;

						var zeroVector = Vector3F.Zero;

						BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, false/*receiveAnotherDecals*/, ref zeroVector, 0, oper.UnwrappedUV, ref decalData.Color, oper.VertexStructureContainsColor, false, decalData.VisibilityDistance, 1.0f, false, 0, null, oper.MeshGeometryIndex );

						//!!!!double

						var worldMatrix = new Matrix4F( decalData.Rotation.ToMatrix3() * Matrix3F.FromScale( decalData.Scale ), decalData.Position.ToVector3F() );
						Bgfx.SetTransform( (float*)&worldMatrix );
						//fixed ( Matrix4F* p = &meshItem.MeshInstanceOne )
						//	Bgfx.SetTransform( (float*)p );

						//u_decalMatrix
						{
							//!!!!slowly
							Matrix3F.FromRotateByY( MathEx.PI / 2, out var rotate3 );
							rotate3.ToMatrix4( out var rotate4 );
							worldMatrix.GetInverse( out var inverse );
							Matrix4F.Multiply( ref rotate4, ref inverse, out var decalMatrix );
							//Matrix4F decalMatrix = Matrix3F.FromRotateByY( MathEx.PI / 2 ).ToMatrix4() * worldMatrix.GetInverse();

							Bgfx.SetUniform( u_decalMatrix, &decalMatrix );
						}

						//u_decalNormalTangent
						{
							Vector4F* normalTangent = stackalloc Vector4F[ 2 ];
							if( decalData.NormalsMode == Decal.NormalsModeEnum.VectorOfDecal )
							{
								var r = decalData.Rotation;
								normalTangent[ 0 ] = new Vector4F( -r.GetForward(), 0 );
								var left = -Vector3F.Cross( -r.GetForward(), r.GetUp() );
								normalTangent[ 1 ] = new Vector4F( left, -1 );
							}
							else
							{
								normalTangent[ 0 ] = Vector4F.Zero;
								normalTangent[ 1 ] = Vector4F.Zero;
							}

							if( normalTangent[ 0 ] != currentDecalNormalTangent0 || normalTangent[ 1 ] != currentDecalNormalTangent1 )
							{
								currentDecalNormalTangent0 = normalTangent[ 0 ];
								currentDecalNormalTangent1 = normalTangent[ 1 ];

								Bgfx.SetUniform( u_decalNormalTangent, normalTangent, 2 );
							}
						}

						context.BindTexture( 3/* "depthTexture"*/, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );
						context.BindTexture( 4/* "gBuffer1TextureCopy"*/, gBuffer1TextureCopy, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );
						context.BindTexture( 5/* "gBuffer4TextureCopy"*/, gBuffer4TextureCopy, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );
						context.BindTexture( 6/* "gBuffer5TextureCopy"*/, gBuffer5TextureCopy, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );

						RenderOperation( context, oper, pass, null, null );

						////!!!!или если уже собран из GroupOfObjects
						//bool instancing = Instancing && meshItem.AnimationData == null;
						//outputInstancingManager.Add( renderableGroup, nOperation, oper, materialData, instancing );
					}
				}
			}
		}

		void InitDeferredShadingData()
		{
			deferredShadingData = new DeferredShadingData();

			//environment light
			{
				//generate compile arguments
				var vertexDefines = new List<(string, string)>();
				var fragmentDefines = new List<(string, string)>();
				{
					var generalDefines = new List<(string, string)>();
					generalDefines.Add( ("LIGHT_TYPE_" + Light.TypeEnum.Ambient.ToString().ToUpper(), "") );
					vertexDefines.AddRange( generalDefines );
					fragmentDefines.AddRange( generalDefines );
				}

				string error2;

				//vertex program
				var vertexProgram = GpuProgramManager.GetProgram( "DeferredEnvironmentLight_",
					GpuProgramType.Vertex, @"Base\Shaders\DeferredEnvironmentLight_vs.sc", vertexDefines, true, out error2 );
				if( !string.IsNullOrEmpty( error2 ) )
				{
					Log.Fatal( error2 );
					return;
				}

				var fragmentProgram = GpuProgramManager.GetProgram( "DeferredEnvironmentLight_",
					GpuProgramType.Fragment, @"Base\Shaders\DeferredEnvironmentLight_fs.sc", fragmentDefines, true, out error2 );
				if( !string.IsNullOrEmpty( error2 ) )
				{
					Log.Fatal( error2 );
					return;
				}

				var passItem = new DeferredShadingData.PassItem();
				passItem.vertexProgram = vertexProgram;
				passItem.fragmentProgram = fragmentProgram;
				//deferredShadingData.passesPerLightWithoutShadows[ nLight ] = passItem;

				//!!!!
				//var pass = GpuMaterialPass.CreatePass( vertexProgram, fragmentProgram );
				////!!!!
				//pass.DepthCheck = false;
				//pass.DepthWrite = false;
				//pass.SourceBlendFactor = SceneBlendFactor.One;
				//pass.DestBlendFactor = SceneBlendFactor.One;

				////!!!!
				////pass.CullingMode = CullingMode.None;
				////!!!!с дептом тогда не то
				////pass.CullingMode = CullingMode.Anticlockwise;

				////if( TwoSided )
				////	pass.CullingMode = CullingMode.None;

				deferredShadingData.passesPerLightWithoutShadows[ 0 ] = passItem;
			}

			//direct lights
			for( int nShadows = 0; nShadows < 3; nShadows++ )
			//for( int nShadows = 0; nShadows < 3; nShadows++ )
			{
				var shadows = nShadows != 0;
				var contactShadows = nShadows == 2;

				if( RenderingSystem.ShadowTechnique == ProjectSettingsPage_Rendering.ShadowTechniqueEnum.None && shadows )
					continue;

				for( int nLight = 1; nLight < 4; nLight++ )//for( int nLight = 0; nLight < 4; nLight++ )
				{
					var lightType = (Light.TypeEnum)nLight;
					//if( shadows && lightType == Light.TypeEnum.Ambient )
					//	continue;

					//generate compile arguments
					var vertexDefines = new List<(string, string)>();
					var fragmentDefines = new List<(string, string)>();
					{
						var generalDefines = new List<(string, string)>();
						generalDefines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );

						//receive shadows support
						if( shadows )
						{
							fragmentDefines.Add( ("SHADOW_MAP", "") );
							if( contactShadows )
								fragmentDefines.Add( ("SHADOW_CONTACT", "") );

							//if( nShadows == 2 )
							//	fragmentDefines.Add( ("SHADOW_MAP_HIGH", "") );
							//else
							//	fragmentDefines.Add( ("SHADOW_MAP_LOW", "") );
						}

						vertexDefines.AddRange( generalDefines );
						fragmentDefines.AddRange( generalDefines );
					}

					string error2;

					//vertex program
					GpuProgram vertexProgram = GpuProgramManager.GetProgram( "DeferredDirectLight_",
						GpuProgramType.Vertex, @"Base\Shaders\DeferredDirectLight_vs.sc", vertexDefines, true, out error2 );
					if( !string.IsNullOrEmpty( error2 ) )
					{
						Log.Fatal( error2 );
						return;
					}

					//fragment program
					GpuProgram fragmentProgram = GpuProgramManager.GetProgram( "DeferredDirectLight_",
						GpuProgramType.Fragment, @"Base\Shaders\DeferredDirectLight_fs.sc", fragmentDefines, true, out error2 );
					if( !string.IsNullOrEmpty( error2 ) )
					{
						Log.Fatal( error2 );
						return;
					}

					var passItem = new DeferredShadingData.PassItem();
					passItem.vertexProgram = vertexProgram;
					passItem.fragmentProgram = fragmentProgram;
					//deferredShadingData.passesPerLightWithoutShadows[ nLight ] = passItem;

					//!!!!
					//var pass = GpuMaterialPass.CreatePass( vertexProgram, fragmentProgram );
					////!!!!
					//pass.DepthCheck = false;
					//pass.DepthWrite = false;
					//pass.SourceBlendFactor = SceneBlendFactor.One;
					//pass.DestBlendFactor = SceneBlendFactor.One;

					////!!!!
					////pass.CullingMode = CullingMode.None;
					////!!!!с дептом тогда не то
					////pass.CullingMode = CullingMode.Anticlockwise;

					////if( TwoSided )
					////	pass.CullingMode = CullingMode.None;

					if( nShadows == 2 )
						deferredShadingData.passesPerLightWithShadowsContactShadows[ nLight ] = passItem;
					else if( nShadows == 1 )
						deferredShadingData.passesPerLightWithShadows[ nLight ] = passItem;
					else
						deferredShadingData.passesPerLightWithoutShadows[ nLight ] = passItem;

					//if( nShadows == 2 )
					//	deferredShadingData.passesPerLightWithShadowsHigh[ nLight ] = passItem;
					//if( nShadows == 1 )
					//	deferredShadingData.passesPerLightWithShadowsLow[ nLight ] = passItem;
					//else
					//	deferredShadingData.passesPerLightWithoutShadows[ nLight ] = passItem;
				}
			}
		}

		[StructLayout( LayoutKind.Sequential )]
		struct DeferredEnvironmentDataUniform
		{
			public QuaternionF rotation;
			public Vector4F multiplierAndAffect;
			public QuaternionF iblRotation;
			public Vector4F iblMultiplierAndAffect;
		}

		unsafe void RenderEnvironmentLightDeferred( ViewportRenderingContext context, FrameData frameData, ImageComponent sceneTexture, ImageComponent normalTexture, ImageComponent gBuffer2Texture, ImageComponent gBuffer3Texture, ImageComponent gBuffer4Texture, ImageComponent gBuffer5Texture, ImageComponent depthTexture )
		{
			var ambientLight = frameData.Lights[ frameData.LightsInFrustumSorted[ 0 ] ];
			ambientLight.Bind( this, context );

			var passItem = deferredShadingData.passesPerLightWithoutShadows[ 0 ];

			var generalContainer = new ParameterContainer();
			{
				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 0/* "sceneTexture"*/,
					sceneTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 1/* "normalTexture"*/,
					normalTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 2/* "gBuffer2Texture"*/,
					gBuffer2Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 9/* "gBuffer3Texture"*/,
					gBuffer3Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 3/* "depthTexture"*/,
					depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 10/* "gBuffer4Texture"*/,
					gBuffer4Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 11/* "gBuffer5Texture"*/,
					gBuffer5Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 8/*"s_brdfLUT"*/,
					BrdfLUT, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
			}

			if( u_reflectionProbeData == null )
				u_reflectionProbeData = GpuProgramManager.RegisterUniform( "u_reflectionProbeData", UniformType.Vector4, 1 );
			if( u_deferredEnvironmentData == null )
				u_deferredEnvironmentData = GpuProgramManager.RegisterUniform( "u_deferredEnvironmentData", UniformType.Vector4, 4 );
			if( u_deferredEnvironmentIrradiance == null )
				u_deferredEnvironmentIrradiance = GpuProgramManager.RegisterUniform( "u_deferredEnvironmentIrradiance", UniformType.Vector4, 9 );
			//if( u_environmentTextureIBLRotation == null )
			//	u_environmentTextureIBLRotation = GpuProgramManager.RegisterUniform( "u_environmentTextureIBLRotation", UniformType.Matrix3x3, 1 );
			//if( u_environmentTextureMultiplierAndAffect == null )
			//	u_environmentTextureMultiplierAndAffect = GpuProgramManager.RegisterUniform( "u_environmentTextureMultiplierAndAffect", UniformType.Vector4, 1 );
			//if( u_environmentTextureIBLMultiplierAndAffect == null )
			//	u_environmentTextureIBLMultiplierAndAffect = GpuProgramManager.RegisterUniform( "u_environmentTextureIBLMultiplierAndAffect", UniformType.Vector4, 1 );

			GetBackgroundEnvironmentData( context, frameData, out var ambientLightTexture, out var ambientLightIrradiance );

			//ambient light
			{
				//set u_reflectionProbeData
				var reflectionProbeData = new Vector4F( 0, 0, 0, 0 );
				Bgfx.SetUniform( u_reflectionProbeData.Value, &reflectionProbeData, 1 );

				var container = new ParameterContainer();

				//bind env textures, parameters
				{
					container.Set( new ViewportRenderingContext.BindTextureData( 6/*"s_environmentTexture"*/,
						ambientLightTexture.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					//container.Set( new ViewportRenderingContext.BindTextureData( 7/*"environmentTextureIBL"*/,
					//	ambientLightIrradiance.Value.texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					var data = new DeferredEnvironmentDataUniform();
					data.rotation = ambientLightTexture.Value.Rotation;
					data.multiplierAndAffect = ambientLightTexture.Value.MultiplierAndAffect;
					data.iblRotation = ambientLightIrradiance.Value.Rotation;
					data.iblMultiplierAndAffect = ambientLightIrradiance.Value.MultiplierAndAffect;
					Bgfx.SetUniform( u_deferredEnvironmentData.Value, &data, 4 );

					fixed( Vector4F* harmonics = ambientLightIrradiance.Value.Harmonics ?? EnvironmentIrradianceData.GrayHarmonics )
						Bgfx.SetUniform( u_deferredEnvironmentIrradiance.Value, harmonics, 9 );

					//var environmentTextureRotation = ambientLightTexture.Value.Rotation;
					//var environmentTextureIBLRotation = ambientLightIrradiance.Value.Rotation;
					//var environmentTextureMultiplierAndAffect = ambientLightTexture.Value.MultiplierAndAffect;
					//var environmentTextureIBLMultiplierAndAffect = ambientLightIrradiance.Value.MultiplierAndAffect;
					//Bgfx.SetUniform( u_environmentTextureRotation.Value, &environmentTextureRotation, 1 );
					//Bgfx.SetUniform( u_environmentTextureIBLRotation.Value, &environmentTextureIBLRotation, 1 );
					//Bgfx.SetUniform( u_environmentTextureMultiplierAndAffect.Value, &environmentTextureMultiplierAndAffect, 1 );
					//Bgfx.SetUniform( u_environmentTextureIBLMultiplierAndAffect.Value, &environmentTextureIBLMultiplierAndAffect, 1 );
				}

				var shader = new CanvasRenderer.ShaderItem();
				shader.CompiledVertexProgram = passItem.vertexProgram;
				shader.CompiledFragmentProgram = passItem.fragmentProgram;
				shader.AdditionalParameterContainers.Add( generalContainer );
				shader.AdditionalParameterContainers.Add( container );

				context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
			}

			//reflection probes
			foreach( var item in frameData.ReflectionProbes )
			{
				//set u_reflectionProbeData
				//!!!!double
				var reflectionProbeData = new Vector4F( item.data.Sphere.Center.ToVector3F(), (float)item.data.Sphere.Radius );
				Bgfx.SetUniform( u_reflectionProbeData.Value, &reflectionProbeData, 1 );
				//reflectionProbeItem.Bind( this, context );

				var container = new ParameterContainer();

				//bind env textures, parameters
				{
					//use reflection from the reflection probe
					container.Set( new ViewportRenderingContext.BindTextureData( 6/*"s_environmentTexture"*/, item.data.CubemapEnvironment ?? ResourceUtility.GrayTextureCube, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					////use lighting from the ambient light
					//container.Set( new ViewportRenderingContext.BindTextureData( 7/*"environmentTextureIBL"*/,
					//	ambientLightIrradiance.Value.texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					var data = new DeferredEnvironmentDataUniform();
					data.rotation = item.data.Rotation;
					data.iblRotation = ambientLightIrradiance.Value.Rotation;
					data.multiplierAndAffect = new Vector4F( item.data.Multiplier, 1 );
					data.iblMultiplierAndAffect = ambientLightIrradiance.Value.MultiplierAndAffect;
					Bgfx.SetUniform( u_deferredEnvironmentData.Value, &data, 4 );

					fixed( Vector4F* harmonics = ambientLightIrradiance.Value.Harmonics ?? EnvironmentIrradianceData.GrayHarmonics )
						Bgfx.SetUniform( u_deferredEnvironmentIrradiance.Value, harmonics, 9 );
					//fixed( Vector4F* harmonics = item.data.HarmonicsIrradiance ?? EnvironmentIrradianceData.GrayHarmonics )
					//	Bgfx.SetUniform( u_deferredEnvironmentIrradiance.Value, harmonics, 9 );

					//var environmentTextureRotation = item.data.Rotation;
					//var environmentTextureIBLRotation = ambientLightIrradiance.Value.Rotation;
					//var environmentTextureMultiplierAndAffect = new Vector4F( item.data.Multiplier, 1 );
					//var environmentTextureIBLMultiplierAndAffect = ambientLightIrradiance.Value.MultiplierAndAffect;
					//Bgfx.SetUniform( u_environmentTextureRotation.Value, &environmentTextureRotation, 1 );
					//Bgfx.SetUniform( u_environmentTextureIBLRotation.Value, &environmentTextureIBLRotation, 1 );
					//Bgfx.SetUniform( u_environmentTextureMultiplierAndAffect.Value, &environmentTextureMultiplierAndAffect, 1 );
					//Bgfx.SetUniform( u_environmentTextureIBLMultiplierAndAffect.Value, &environmentTextureIBLMultiplierAndAffect, 1 );
				}

				var shader = new CanvasRenderer.ShaderItem();
				shader.CompiledVertexProgram = passItem.vertexProgram;
				shader.CompiledFragmentProgram = passItem.fragmentProgram;
				shader.AdditionalParameterContainers.Add( generalContainer );
				shader.AdditionalParameterContainers.Add( container );

				//!!!!triangles

				//if (screenTriangles != null)
				//    context.RenderTrianglesToCurrentViewport(shader, screenTriangles, CanvasRenderer.BlendingType.Add);
				//else
				context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.AlphaBlend );
			}
		}

		unsafe void RenderDirectLightDeferred( ViewportRenderingContext context, FrameData frameData, ImageComponent sceneTexture, ImageComponent normalTexture, ImageComponent gBuffer2Texture, ImageComponent gBuffer3Texture, ImageComponent gBuffer4Texture, ImageComponent gBuffer5Texture, ImageComponent depthTexture, LightItem lightItem )
		{
			Viewport viewportOwner = context.Owner;

			bool skip = false;

			List<CanvasRenderer.TriangleVertex> screenTriangles = null;

			//Point, Spotlight
			if( lightItem.data.Type == Light.TypeEnum.Point || lightItem.data.Type == Light.TypeEnum.Spotlight )
			{
				Vector3[] positions;
				int[] indices;

				if( lightItem.data.Type == Light.TypeEnum.Point )
				{
					//Point
					var radius = lightItem.data.AttenuationFar * 1.1;
					SimpleMeshGenerator.GenerateSphere( radius, 8, 8, false, out positions, out indices );
					for( int n = 0; n < positions.Length; n++ )
						positions[ n ] += lightItem.data.Position;
				}
				else
				{
					//!!!!не оптимально. генерировать округлый конус

					//Spotlight
					var outer = lightItem.data.SpotlightOuterAngle;
					if( outer > 179 )
						outer = 179;
					var radius = lightItem.data.AttenuationFar * Math.Tan( outer.InRadians() / 2 );
					var height = lightItem.data.AttenuationFar;
					SimpleMeshGenerator.GenerateCone( 0, SimpleMeshGenerator.ConeOrigin.Center, radius, height, 16, true, true, out positions, out indices );

					var mat = new Matrix4(
						lightItem.data.Rotation.ToMatrix3() * Matrix3.FromRotateByY( Math.PI ) * Matrix3.FromScale( new Vector3( 1.1, 1.1, 1.1 ) ),
						lightItem.data.Position + lightItem.data.Rotation.GetForward() * lightItem.data.AttenuationFar / 2 );

					for( int n = 0; n < positions.Length; n++ )
						positions[ n ] = mat * positions[ n ];
				}

				//var screenPositions = new Vector2F[ positions.Length ];
				//for( int n = 0; n < screenPositions.Length; n++ )
				//{
				//	context.Owner.CameraSettings.ProjectToScreenCoordinates( positions[ n ], out var screen, false );
				//	screenPositions[ n ] = screen.ToVector2F();
				//	//if( context.Owner.CameraSettings.ProjectToScreenCoordinates( positions[ n ], out var screen ) )
				//	//	screenPositions[ n ] = screen.ToVec2F();
				//	//else
				//	//	screenPositions[ n ] = new Vec2F( float.NaN, float.NaN );
				//}

				screenTriangles = new List<CanvasRenderer.TriangleVertex>( indices.Length / 2 );

				var planes = viewportOwner.CameraSettings.Frustum.Planes;

				for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
				{
					var polygon = new Vector3[ 3 ];
					polygon[ 0 ] = positions[ indices[ nTriangle * 3 + 0 ] ];
					polygon[ 1 ] = positions[ indices[ nTriangle * 3 + 1 ] ];
					polygon[ 2 ] = positions[ indices[ nTriangle * 3 + 2 ] ];

					//clamp by frustum side planes
					for( int nPlane = 2; nPlane < 6; nPlane++ )
					{
						ref var plane = ref planes[ nPlane ];
						polygon = MathAlgorithms.ClipPolygonByPlane( polygon, -plane );
					}

					//convert world to screen coordinates
					var screenPolygon = new Vector2[ polygon.Length ];
					for( int n = 0; n < screenPolygon.Length; n++ )
						context.Owner.CameraSettings.ProjectToScreenCoordinates( polygon[ n ], out screenPolygon[ n ], false );

					//triangulate
					for( int n = 1; n < screenPolygon.Length - 1; n++ )
					{
						var p0 = screenPolygon[ 0 ].ToVector2F();
						var p1 = screenPolygon[ n ].ToVector2F();
						var p2 = screenPolygon[ n + 1 ].ToVector2F();

						if( ( ( p1.Y - p0.Y ) * ( p2.X - p1.X ) - ( p2.Y - p1.Y ) * ( p1.X - p0.X ) ) < 0 )
						{
							screenTriangles.Add( new CanvasRenderer.TriangleVertex( p0, ColorValue.One, p0 ) );
							screenTriangles.Add( new CanvasRenderer.TriangleVertex( p1, ColorValue.One, p1 ) );
							screenTriangles.Add( new CanvasRenderer.TriangleVertex( p2, ColorValue.One, p2 ) );
						}
					}
				}

				//var screenPositions = new Vector2F[ positions.Length ];
				//for( int n = 0; n < screenPositions.Length; n++ )
				//{
				//	context.Owner.CameraSettings.ProjectToScreenCoordinates( positions[ n ], out var screen, false );
				//	screenPositions[ n ] = screen.ToVector2F();
				//	//if( context.Owner.CameraSettings.ProjectToScreenCoordinates( positions[ n ], out var screen ) )
				//	//	screenPositions[ n ] = screen.ToVec2F();
				//	//else
				//	//	screenPositions[ n ] = new Vec2F( float.NaN, float.NaN );
				//}

				//screenTriangles = new List<CanvasRenderer.TriangleVertex>( indices.Length / 2 );

				//for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
				//{
				//	var p0 = screenPositions[ indices[ nTriangle * 3 + 0 ] ];
				//	var p1 = screenPositions[ indices[ nTriangle * 3 + 1 ] ];
				//	var p2 = screenPositions[ indices[ nTriangle * 3 + 2 ] ];

				//	if( !float.IsNaN( p0.X ) && !float.IsNaN( p1.X ) && !float.IsNaN( p2.X ) )
				//	{
				//		if( ( ( p1.Y - p0.Y ) * ( p2.X - p1.X ) - ( p2.Y - p1.Y ) * ( p1.X - p0.X ) ) < 0 )
				//		{
				//			if( !( p0.X < 0 && p1.X < 0 && p2.X < 0 ) && !( p0.X > 1 && p1.X > 1 && p2.X > 1 ) )
				//			{
				//				if( !( p0.Y < 0 && p1.Y < 0 && p2.Y < 0 ) && !( p0.Y > 1 && p1.Y > 1 && p2.Y > 1 ) )
				//				{
				//					//context.Owner.CanvasRenderer.AddLine( p0, p1, ColorValue.One );
				//					//context.Owner.CanvasRenderer.AddLine( p1, p2, ColorValue.One );
				//					//context.Owner.CanvasRenderer.AddLine( p2, p0, ColorValue.One );

				//					screenTriangles.Add( new CanvasRenderer.TriangleVertex( p0, ColorValue.One, p0 ) );
				//					screenTriangles.Add( new CanvasRenderer.TriangleVertex( p1, ColorValue.One, p1 ) );
				//					screenTriangles.Add( new CanvasRenderer.TriangleVertex( p2, ColorValue.One, p2 ) );
				//				}
				//			}
				//		}
				//	}
				//}

				////check to use fullscreen quad and skip light
				//if( screenTriangles.Count != 0 )
				//{
				//	var plane = viewportOwner.CameraSettings.Frustum.Planes[ 0 ];

				//	int backsideCount = 0;
				//	foreach( var pos in positions )
				//	{
				//		if( plane.GetSide( pos ) == Plane.Side.Positive )
				//			backsideCount++;
				//	}

				//	//all backside
				//	if( backsideCount == positions.Length )
				//		skip = true;
				//	//use fullscreen quad when exists points backward camera
				//	if( backsideCount != 0 )
				//		screenTriangles = null;
				//}

				//all triangles outside screen
				if( screenTriangles != null && screenTriangles.Count == 0 )
					skip = true;
			}

			if( !skip )
			{
				//set lightItem uniforms
				//if( lightItemBinded != lightItem )
				//{
				lightItem.Bind( this, context );
				//, ShadowIntensity );
				//	lightItemBinded = lightItem;
				//}

				//pass
				//GpuMaterialPass pass;
				DeferredShadingData.PassItem passItem;
				{
					if( lightItem.prepareShadows )
					{
						if( lightItem.data.ShadowContactLength > 0 )
							passItem = deferredShadingData.passesPerLightWithShadowsContactShadows[ (int)lightItem.data.Type ];
						else
							passItem = deferredShadingData.passesPerLightWithShadows[ (int)lightItem.data.Type ];

						//if( ShadowQuality.Value == ShadowQualityEnum.High )
						//	passItem = deferredShadingData.passesPerLightWithShadowsHigh[ (int)lightItem.data.Type ];
						//else
						//	passItem = deferredShadingData.passesPerLightWithShadowsLow[ (int)lightItem.data.Type ];
					}
					else
						passItem = deferredShadingData.passesPerLightWithoutShadows[ (int)lightItem.data.Type ];
				}

				{
					//var item = mesh.Result.RenderOperations[ 0 ];
					//var operation = item.operation;

					var generalContainer = new ParameterContainer();

					{
						//bind textures
						generalContainer.Set( new ViewportRenderingContext.BindTextureData( 0/* "sceneTexture"*/, sceneTexture,
							TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

						generalContainer.Set( new ViewportRenderingContext.BindTextureData( 1/* "normalTexture"*/, normalTexture,
							TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

						generalContainer.Set( new ViewportRenderingContext.BindTextureData( 2/* "gBuffer2Texture"*/, gBuffer2Texture,
							TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

						generalContainer.Set( new ViewportRenderingContext.BindTextureData( 8/*"s_brdfLUT"*/, BrdfLUT,
							TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

						generalContainer.Set( new ViewportRenderingContext.BindTextureData( 9/* "gBuffer2Texture"*/, gBuffer3Texture,
							TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

						generalContainer.Set( new ViewportRenderingContext.BindTextureData( 3/* "depthTexture"*/, depthTexture,
							TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

						generalContainer.Set( new ViewportRenderingContext.BindTextureData( 10/* "gBuffer4Texture"*/, gBuffer4Texture,
							TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

						generalContainer.Set( new ViewportRenderingContext.BindTextureData( 11/* "gBuffer5Texture"*/, gBuffer5Texture,
							TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

						//light mask
						if( RenderingSystem.LightMask && lightItem.data.Type != Light.TypeEnum.Ambient )
						{
							if( lightItem.data.Type == Light.TypeEnum.Point )
							{
								var texture = lightItem.data.Mask;
								if( texture == null || texture.Result.TextureType != ImageComponent.TypeEnum.Cube )
									texture = ResourceUtility.WhiteTextureCube;

								//!!!!anisotropic? где еще

								generalContainer.Set( new ViewportRenderingContext.BindTextureData( 4/*"s_lightMask"*/, texture,
									TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
								//var textureValue = new GpuMaterialPass.TextureParameterValue( texture,
								//	TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
								//generalContainer.Set( "4"/*"s_lightMask"*/, textureValue, ParameterType.TextureCube );
							}
							else
							{
								var texture = lightItem.data.Mask;
								if( texture == null || texture.Result.TextureType != ImageComponent.TypeEnum._2D )
									texture = ResourceUtility.WhiteTexture2D;

								var clamp = lightItem.data.Type == Light.TypeEnum.Spotlight;

								generalContainer.Set( new ViewportRenderingContext.BindTextureData( 4/*"s_lightMask"*/, texture,
									clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
							}
						}

						//if( lightItem.prepareShadows )
						{
							//Viewport shadowViewport = lightItem.shadowTexture.Result.GetRenderTarget().Viewports[ 0 ];

							//shadowMap
							{
								ImageComponent texture;
								if( lightItem.prepareShadows )
								{
									texture = lightItem.shadowTexture;
								}
								else
								{
									if( lightItem.data.Type == Light.TypeEnum.Point )
										texture = nullShadowTextureCube;
									else
										texture = nullShadowTexture2D;
								}

								ViewportRenderingContext.BindTextureData textureValue;
								if( RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4 )
								{
									textureValue = new ViewportRenderingContext.BindTextureData( 5/*"s_shadowMap"*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None );
								}
								else
								{
									textureValue = new ViewportRenderingContext.BindTextureData( 5/*"s_shadowMap"*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None, TextureFlags.CompareLessEqual );
								}

								generalContainer.Set( ref textureValue );

								//{
								//	//!!!!как для forward делать?

								//	var textureValue2 = new ViewportRenderingContext.BindTextureData( 6/*"s_shadowMap"*/, texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None );

								//	generalContainer.Set( ref textureValue2 );
								//}

							}

							//Vector2F shadowBias = new Vector2F( lightItem.data.ShadowBias, lightItem.data.ShadowNormalBias );
							//generalContainer.Set( "u_shadowBias", ParameterType.Vector2, 1, &shadowBias, sizeof( Vector2F ) );
						}
					}

					//List<ParameterContainer> containers = new List<ParameterContainer>();
					//containers.Add( generalContainer );

					{
						var shader = new CanvasRenderer.ShaderItem();
						shader.CompiledVertexProgram = passItem.vertexProgram;
						shader.CompiledFragmentProgram = passItem.fragmentProgram;
						//shader.CompiledVertexProgram = pass.Programs[ (int)GpuProgramType.Vertex ];
						//shader.CompiledFragmentProgram = pass.Programs[ (int)GpuProgramType.Fragment ];
						shader.AdditionalParameterContainers.Add( generalContainer );
						//shader.AdditionalParameterContainers.AddRange( containers );

						if( lightItem.data.ShadowContactLength > 0 )
						{
							//Vector3 cameraPos = context.Owner.CameraSettings.Position;
							////!!!!double
							//Vector3F cameraPosition = cameraPos.ToVector3F();

							//!!!!double
							Matrix4F projectionMatrix = context.Owner.CameraSettings.ProjectionMatrix.ToMatrix4F();
							Matrix4F viewMatrix = context.Owner.CameraSettings.ViewMatrix.ToMatrix4F();

							Matrix4F viewProjMatrix = projectionMatrix * viewMatrix;
							viewProjMatrix.GetInverse( out var invViewProjMatrix );

							//float aspectRatio = (float)context.Owner.CameraSettings.AspectRatio;
							//float fov = (float)context.Owner.CameraSettings.FieldOfView;
							//float zNear = (float)context.Owner.CameraSettings.NearClipDistance;
							//float zFar = (float)context.Owner.CameraSettings.FarClipDistance;

							//shader.Parameters.Set( "viewMatrix", viewMatrix );
							//shader.Parameters.Set( "projMatrix", projectionMatrix );

							shader.Parameters.Set( "viewProj", viewProjMatrix );
							shader.Parameters.Set( "invViewProj", invViewProjMatrix );
							//shader.Parameters.Set( "cameraPosition", cameraPosition );
							//shader.Parameters.Set( "edgeFactorPower", (float)EdgeFactorPower );
							//shader.Parameters.Set( "initialStepScale", (float)InitialStepScale.Value );
							//shader.Parameters.Set( "worldThickness", (float)WorldThickness.Value );

							//shader.Parameters.Set( "colorTextureSize", new Vector4F( (float)actualTexture.Result.ResultSize.X, (float)actualTexture.Result.ResultSize.Y, 0.0f, 0.0f ) );
							//shader.Parameters.Set( "zNear", zNear );
							//shader.Parameters.Set( "zFar", zFar );
							//shader.Parameters.Set( "fov", fov );
							//shader.Parameters.Set( "aspectRatio", aspectRatio );
						}

						if( screenTriangles != null )
							context.RenderTrianglesToCurrentViewport( shader, screenTriangles, CanvasRenderer.BlendingType.Add );
						else
							context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
					}
				}
			}
		}

		protected unsafe virtual void RenderLightsDeferred( ViewportRenderingContext context, FrameData frameData, ImageComponent sceneTexture, ImageComponent normalTexture, ImageComponent gBuffer2Texture, ImageComponent gBuffer3Texture, ImageComponent gBuffer4Texture, ImageComponent gBuffer5Texture, ImageComponent depthTexture )
		{
			if( deferredShadingData == null )
				InitDeferredShadingData();

			////!!!!temp
			////save depth buffer to texture
			//PhysicallyCorrectRendering_RenderLightsDeferred( context, out var handled );
			//if( handled )
			//	return;

			//!!!!
			//stencil optimization. сначала проход заполнить стенсиль проверяя depth. второй проход с проверкой

			foreach( var lightIndex in frameData.LightsInFrustumSorted )
			{
				var lightItem = frameData.Lights[ lightIndex ];

				if( lightItem.data.Type == Light.TypeEnum.Ambient )
					RenderEnvironmentLightDeferred( context, frameData, sceneTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, depthTexture );
				else
					RenderDirectLightDeferred( context, frameData, sceneTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, depthTexture, lightItem );
			}
		}

		[StructLayout( LayoutKind.Sequential )]
		struct ForwardEnvironmentDataUniform
		{
			public QuaternionF rotation1;
			public Vector4F multiplierAndAffect1;
			public QuaternionF rotation2;
			public Vector4F multiplierAndAffect2;
			public float blendingFactor;
			public float unused1;
			public float unused2;
			public float unused3;
		}

		public unsafe void ForwardBindGeneralTexturesUniforms( ViewportRenderingContext context, FrameData frameData, ref Sphere objectSphere, LightItem lightItem, bool receiveShadows, bool setUniforms, bool bindBrdfLUT = true )
		{
			GetEnvironmentTexturesByBoundingSphereIntersection( context, frameData, ref objectSphere/*meshItem.BoundingSphere*/, out var texture1, out var harmonics1, out var texture2, out var harmonics2, out var environmentBlendingFactor );

			//environment map
			{
				context.BindTexture( 3/*"environmentTexture1"*/, texture1.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );

				//context.BindTexture( new ViewportRenderingContext.BindTextureData( 3/*"environmentTextureIBL1"*/, harmonics1.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				context.BindTexture( 4/*"environmentTexture2"*/, texture2.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );

				//context.BindTexture( new ViewportRenderingContext.BindTextureData( 5/*"environmentTextureIBL2"*/, harmonics2.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				if( setUniforms )
				{
					if( u_forwardEnvironmentData == null )
						u_forwardEnvironmentData = GpuProgramManager.RegisterUniform( "u_forwardEnvironmentData", UniformType.Vector4, 5 );
					if( u_forwardEnvironmentIrradiance1 == null )
						u_forwardEnvironmentIrradiance1 = GpuProgramManager.RegisterUniform( "u_forwardEnvironmentIrradiance1", UniformType.Vector4, 9 );
					if( u_forwardEnvironmentIrradiance2 == null )
						u_forwardEnvironmentIrradiance2 = GpuProgramManager.RegisterUniform( "u_forwardEnvironmentIrradiance2", UniformType.Vector4, 9 );

					var data = new ForwardEnvironmentDataUniform();
					data.rotation1 = texture1.Value.Rotation;
					data.multiplierAndAffect1 = texture1.Value.MultiplierAndAffect;
					data.rotation2 = texture2.Value.Rotation;
					data.multiplierAndAffect2 = texture2.Value.MultiplierAndAffect;
					data.blendingFactor = environmentBlendingFactor;
					Bgfx.SetUniform( u_forwardEnvironmentData.Value, &data, 5 );

					fixed( Vector4F* harmonics = harmonics1.Value.Harmonics ?? EnvironmentIrradianceData.GrayHarmonics )
						Bgfx.SetUniform( u_forwardEnvironmentIrradiance1.Value, harmonics, 9 );

					fixed( Vector4F* harmonics = harmonics2.Value.Harmonics ?? EnvironmentIrradianceData.GrayHarmonics )
						Bgfx.SetUniform( u_forwardEnvironmentIrradiance2.Value, harmonics, 9 );
				}

				//context.BindTexture( new ViewportRenderingContext.BindTextureData( 2/*"environmentTexture1"*/, texture1.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				//context.BindTexture( new ViewportRenderingContext.BindTextureData( 3/*"environmentTextureIBL1"*/, harmonics1.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				//context.BindTexture( new ViewportRenderingContext.BindTextureData( 4/*"environmentTexture2"*/, texture2.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				//context.BindTexture( new ViewportRenderingContext.BindTextureData( 5/*"environmentTextureIBL2"*/, harmonics2.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				//if( setUniforms )
				//{
				//	if( u_environmentTexture1Rotation == null )
				//		u_environmentTexture1Rotation = GpuProgramManager.RegisterUniform( "u_environmentTexture1Rotation", UniformType.Matrix3x3, 1 );
				//	if( u_environmentTexture2Rotation == null )
				//		u_environmentTexture2Rotation = GpuProgramManager.RegisterUniform( "u_environmentTexture2Rotation", UniformType.Matrix3x3, 1 );
				//	if( u_environmentTexture1MultiplierAndAffect == null )
				//		u_environmentTexture1MultiplierAndAffect = GpuProgramManager.RegisterUniform( "u_environmentTexture1MultiplierAndAffect", UniformType.Vector4, 1 );
				//	if( u_environmentTexture2MultiplierAndAffect == null )
				//		u_environmentTexture2MultiplierAndAffect = GpuProgramManager.RegisterUniform( "u_environmentTexture2MultiplierAndAffect", UniformType.Vector4, 1 );

				//	var rotation1 = texture1.Value.Rotation;
				//	var rotation2 = texture2.Value.Rotation;
				//	var multiplier1 = texture1.Value.MultiplierAndAffect;
				//	var multiplier2 = texture2.Value.MultiplierAndAffect;
				//	Bgfx.SetUniform( u_environmentTexture1Rotation.Value, &rotation1, 1 );
				//	Bgfx.SetUniform( u_environmentTexture2Rotation.Value, &rotation2, 1 );
				//	Bgfx.SetUniform( u_environmentTexture1MultiplierAndAffect.Value, &multiplier1, 1 );
				//	Bgfx.SetUniform( u_environmentTexture2MultiplierAndAffect.Value, &multiplier2, 1 );
				//}
			}

			//s_brdfLUT
			if( bindBrdfLUT )
				BindBrdfLUT( context );

			//light mask
			if( RenderingSystem.LightMask && lightItem.data.Type != Light.TypeEnum.Ambient )
			{
				if( lightItem.data.Type == Light.TypeEnum.Point )
				{
					var texture = lightItem.data.Mask;
					if( texture == null || texture.Result.TextureType != ImageComponent.TypeEnum.Cube )
						texture = ResourceUtility.WhiteTextureCube;

					//!!!!anisotropic? где еще

					context.BindTexture( 7/*"s_lightMask"*/,
						texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
				}
				else
				{
					var texture = lightItem.data.Mask;
					if( texture == null || texture.Result.TextureType != ImageComponent.TypeEnum._2D )
						texture = ResourceUtility.WhiteTexture2D;

					var clamp = lightItem.data.Type == Light.TypeEnum.Spotlight;
					context.BindTexture( 7/*"s_lightMask"*/,
						texture, clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap,
						FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
				}
			}

			//receive shadows
			//if( receiveShadows )
			{
				//Viewport shadowViewport = lightItem.shadowTexture.Result.GetRenderTarget().Viewports[ 0 ];

				//shadowMap
				{
					ImageComponent texture;
					if( receiveShadows )
					{
						texture = lightItem.shadowTexture;
					}
					else
					{
						if( lightItem.data.Type == Light.TypeEnum.Point )
							texture = nullShadowTextureCube;
						else
							texture = nullShadowTexture2D;
					}

					if( RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4 )
					{
						context.BindTexture( 8/*"g_shadowMap"*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None );
					}
					else
					{
						context.BindTexture( 8/*"g_shadowMap"*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None, TextureFlags.CompareLessEqual );
					}

					//var textureValue = new ViewportRenderingContext.BindTextureData( 8/*"g_shadowMap"*/,
					//	texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear,
					//	FilterOption.None );
					//textureValue.AdditionalFlags |= TextureFlags.CompareLessEqual;
					//context.BindTexture( ref textureValue );
				}

				//Vector2F shadowBias = new Vector2F( lightItem.data.ShadowBias, lightItem.data.ShadowNormalBias );
				//generalContainer.Set( "u_shadowBias", ParameterType.Vector2, 1, &shadowBias, sizeof( Vector2F ) );
			}

			////set u_environmentBlendingFactor
			//if( setUniforms )
			//{
			//	if( u_environmentBlendingFactor == null )
			//		u_environmentBlendingFactor = GpuProgramManager.RegisterUniform( "u_environmentBlendingFactor", UniformType.Vector4, 1 );

			//	var value = new Vector4F( environmentBlendingFactor, 0, 0, 0 );
			//	Bgfx.SetUniform( u_environmentBlendingFactor.Value, &value, 1 );
			//}
		}

		protected unsafe virtual void Render3DSceneForwardOpaque( ViewportRenderingContext context, FrameData frameData )
		{
			Viewport viewportOwner = context.Owner;

			//get renderableGroupsToDraw
			Vector2I[] renderableGroupsToDraw;
			{
				var list = new OpenList<Vector2I>( frameData.RenderableGroupsInFrustum.Count );

				foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
				{
					bool add = false;

					if( renderableGroup.X == 0 )
					{
						ref var data = ref frameData.Meshes.Data[ renderableGroup.Y ];
						add = ( data.Flags & FrameData.MeshItem.FlagsEnum.UseForwardOpaque ) != 0;
					}
					else
					{
						ref var data = ref frameData.Billboards.Data[ renderableGroup.Y ];
						add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseForwardOpaque ) != 0;
					}

					if( add )
						list.Add( renderableGroup );
				}

				renderableGroupsToDraw = list.ToArray();
			}

			//sort by distance
			CollectionUtility.MergeSort( renderableGroupsToDraw, delegate ( Vector2I a, Vector2I b )
			{
				var distanceASquared = frameData.GetObjectGroupDistanceToCameraSquared( ref a );
				var distanceBSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref b );
				if( distanceASquared < distanceBSquared )
					return -1;
				if( distanceASquared > distanceBSquared )
					return 1;
				return 0;
			}, true );


			//bind textures for all render operations
			BindBrdfLUT( context );
			BindMaterialsTexture( context, frameData );


			LightItem lightItemBinded = null;

			for( int nLightsInFrustumSorted = 0; nLightsInFrustumSorted < frameData.LightsInFrustumSorted.Length; nLightsInFrustumSorted++ )
			{
				var lightIndex = frameData.LightsInFrustumSorted[ nLightsInFrustumSorted ];
				var lightItem = frameData.Lights[ lightIndex ];

				int sectorCount = nLightsInFrustumSorted == 0 ? sectorCountByDistance : 1;
				for( int nSector = 0; nSector < sectorCount; nSector++ )
				{
					int indexFrom = (int)( (float)renderableGroupsToDraw.Length * nSector / sectorCount );
					int indexTo = (int)( (float)renderableGroupsToDraw.Length * ( nSector + 1 ) / sectorCount );
					if( nSector == sectorCount - 1 )
						indexTo = renderableGroupsToDraw.Length;

					try
					{
						//fill output manager
						for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
						{
							var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

							if( renderableGroup.X == 0 )
							{
								//meshes

								ref var meshItem2 = ref frameData.Meshes.Data[ renderableGroup.Y ];

								bool affectedByLight;
								var lightType = lightItem.data.Type;
								if( lightType == Light.TypeEnum.Point || lightType == Light.TypeEnum.Spotlight )
									affectedByLight = meshItem2.ContainsPointOrSpotLight( lightIndex );
								else
									affectedByLight = true;

								if( affectedByLight )
								{
									ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
									var meshData = meshItem.MeshData;

									if( !meshItem.OnlyForShadowGeneration )
									{
										for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
										{
											var oper = meshData.RenderOperations[ nOperation ];
											var materialData = GetMeshMaterialData( ref meshItem, oper, nOperation, true );

											bool add = !materialData.Transparent;
											if( materialData.deferredShadingSupport && GetDeferredShading() && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
												add = false;
											if( add )
											{
												if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
												{
													bool instancing = Instancing && meshItem.AnimationData == null && meshItem.BatchingInstanceBuffer == null && meshItem.CutVolumes == null;
													outputInstancingManager.Add( renderableGroup, nOperation, oper, materialData, instancing );
												}
											}
										}
									}
								}
							}
							else if( renderableGroup.X == 1 )
							{
								//billboards

								ref var billboardItem2 = ref frameData.Billboards.Data[ renderableGroup.Y ];

								bool affectedByLight;
								var lightType = lightItem.data.Type;
								if( lightType == Light.TypeEnum.Point || lightType == Light.TypeEnum.Spotlight )
									affectedByLight = billboardItem2.ContainsPointOrSpotLight( lightIndex );
								else
									affectedByLight = true;

								if( affectedByLight )
								{
									ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];
									var meshData = Billboard.GetBillboardMesh().Result.MeshData;

									for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
									{
										var oper = meshData.RenderOperations[ nOperation ];
										var materialData = GetBillboardMaterialData( ref billboardItem, true );

										bool add = !materialData.Transparent;
										if( materialData.deferredShadingSupport && GetDeferredShading() && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
											add = false;
										if( add )
										{
											if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
											{
												//!!!!или если уже собран из GroupOfObjects
												bool instancing = Instancing && billboardItem.CutVolumes == null;
												outputInstancingManager.Add( renderableGroup, nOperation, oper, materialData, instancing );
											}
										}
									}
								}
							}
						}

						outputInstancingManager.Prepare();

						//render output items
						{
							Material.CompiledMaterialData materialBinded = null;

							var outputItems = outputInstancingManager.outputItems;
							for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
							{
								ref var outputItem = ref outputItems.Data[ nOutputItem ];
								var materialData = outputItem.materialData;

								//set lightItem uniforms
								if( lightItemBinded != lightItem )
								{
									lightItem.Bind( this, context );
									lightItemBinded = lightItem;
								}

								//bind material data

								var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
								bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

								GpuMaterialPass pass;
								if( receiveShadows )
								{
									pass = passesGroup.passWithShadows;
									//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
									//	pass = passesGroup.passWithShadowsHigh;
									//else
									//	pass = passesGroup.passWithShadowsLow;
								}
								else
									pass = passesGroup.passWithoutShadows;

								if( Instancing && outputItem.renderableItemsCount >= InstancingMinCount )
								{
									//with instancing

									bool materialWasChanged = materialBinded != materialData;
									materialBinded = materialData;
									materialData.BindCurrentFrameData( context, false, materialWasChanged );

									//bind operation data
									var firstRenderableItem = outputItem.renderableItemFirst;
									if( firstRenderableItem.X == 0 )
									{
										//meshes
										ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ firstRenderableItem.Y ];
										var meshData = meshItem.MeshData;

										ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

										BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.BillboardDataMode, outputItem.operation.BillboardDataImage, outputItem.operation.MeshGeometryIndex );
									}
									else if( firstRenderableItem.X == 1 )
									{
										//billboards
										ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ firstRenderableItem.Y ];
										var meshData = Billboard.GetBillboardMesh().Result.MeshData;

										ForwardBindGeneralTexturesUniforms( context, frameData, ref billboardItem.BoundingSphere, lightItem, receiveShadows, true, false );

										BindRenderOperationData( context, frameData, materialData, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItem.operation.BillboardDataMode, outputItem.operation.BillboardDataImage, outputItem.operation.MeshGeometryIndex );
									}

									int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
									int instanceCount = outputItem.renderableItemsCount;

									if( InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) == instanceCount )
									{
										var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );

										//get instancing matrices
										RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
										int currentMatrix = 0;
										for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
										{
											var renderableItem = outputItem.renderableItems[ nRenderableItem ];

											if( renderableItem.X == 0 )
											{
												//meshes
												ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
												meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
											}
											else if( renderableItem.X == 1 )
											{
												//billboards
												ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
												billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
											}
										}

										RenderOperation( context, outputItem.operation, pass, null, null, ref instanceBuffer, instanceCount );
									}
									//else
									//	Log.Fatal( "InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) != instanceCount." );
								}
								else
								{
									//without instancing

									for( int nRenderableItem = 0; nRenderableItem < outputItem.renderableItemsCount; nRenderableItem++ )
									{
										Vector3I renderableItem;
										if( nRenderableItem != 0 )
											renderableItem = outputItem.renderableItems[ nRenderableItem ];
										else
											renderableItem = outputItem.renderableItemFirst;

										//bind material data
										bool materialWasChanged = materialBinded != materialData;
										materialBinded = materialData;
										materialData.BindCurrentFrameData( context, false, materialWasChanged && nRenderableItem == 0 );

										//bind render operation data, set matrix
										if( renderableItem.X == 0 )
										{
											//meshes
											ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
											var meshData = meshItem.MeshData;

											ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, nRenderableItem == 0, false );

											var batchInstancing = meshItem.BatchingInstanceBuffer != null;

											BindRenderOperationData( context, frameData, materialData, batchInstancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.BillboardDataMode, outputItem.operation.BillboardDataImage, outputItem.operation.MeshGeometryIndex );

											if( !batchInstancing )
												fixed( Matrix4F* p = &meshItem.Transform )
													Bgfx.SetTransform( (float*)p );

											RenderOperation( context, outputItem.operation, pass, null, meshItem.CutVolumes, meshItem.BatchingInstanceBuffer );
										}
										else if( renderableItem.X == 1 )
										{
											//billboards
											ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
											var meshData = Billboard.GetBillboardMesh().Result.MeshData;

											ForwardBindGeneralTexturesUniforms( context, frameData, ref billboardItem.BoundingSphere, lightItem, receiveShadows, nRenderableItem == 0, false );

											BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItem.operation.BillboardDataMode, outputItem.operation.BillboardDataImage, outputItem.operation.MeshGeometryIndex );

											billboardItem.GetWorldMatrix( out var worldMatrix );
											Bgfx.SetTransform( (float*)&worldMatrix );

											RenderOperation( context, outputItem.operation, pass, null, billboardItem.CutVolumes );
										}
									}
								}
							}
						}
					}
					finally
					{
						outputInstancingManager.Clear();
					}
				}
			}

			//render layers
			if( DebugDrawLayers )
			{
				for( int nLightsInFrustumSorted = 0; nLightsInFrustumSorted < frameData.LightsInFrustumSorted.Length; nLightsInFrustumSorted++ )
				{
					var lightIndex = frameData.LightsInFrustumSorted[ nLightsInFrustumSorted ];
					var lightItem = frameData.Lights[ lightIndex ];

					int sectorCount = nLightsInFrustumSorted == 0 ? sectorCountByDistance : 1;
					for( int nSector = 0; nSector < sectorCount; nSector++ )
					{
						int indexFrom = (int)( (float)renderableGroupsToDraw.Length * nSector / sectorCount );
						int indexTo = (int)( (float)renderableGroupsToDraw.Length * ( nSector + 1 ) / sectorCount );
						if( nSector == sectorCount - 1 )
							indexTo = renderableGroupsToDraw.Length;


						Material.CompiledMaterialData materialBinded = null;

						for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
						{
							var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

							if( renderableGroup.X == 0 )
							{
								//meshes

								ref var meshItem2 = ref frameData.Meshes.Data[ renderableGroup.Y ];

								if( meshItem2.Flags.HasFlag( FrameData.MeshItem.FlagsEnum.ContainsForwardOpaqueLayersOnOpaqueBaseObjects ) )
								{
									bool affectedByLight;
									var lightType = lightItem.data.Type;
									if( lightType == Light.TypeEnum.Point || lightType == Light.TypeEnum.Spotlight )
										affectedByLight = meshItem2.ContainsPointOrSpotLight( lightIndex );
									else
										affectedByLight = true;

									if( affectedByLight )
									{
										ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
										var meshData = meshItem.MeshData;

										for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
										{
											ref var layer = ref meshItem.Layers[ nLayer ];
											var materialData = GetLayerMaterialData( ref layer, true );
											if( materialData == null )
												continue;

											bool add = !materialData.Transparent;
											if( materialData.deferredShadingSupport && GetDeferredShading() && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
												add = false;
											if( add )
											{
												if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
												{
													//set lightItem uniforms
													if( lightItemBinded != lightItem )
													{
														lightItem.Bind( this, context );
														lightItemBinded = lightItem;
													}

													//bind material data

													var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
													bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

													GpuMaterialPass pass;
													if( receiveShadows )
													{
														pass = passesGroup.passWithShadows;
														//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
														//	pass = passesGroup.passWithShadowsHigh;
														//else
														//	pass = passesGroup.passWithShadowsLow;
													}
													else
														pass = passesGroup.passWithoutShadows;

													bool materialWasChanged = materialBinded != materialData;
													materialBinded = materialData;
													materialData.BindCurrentFrameData( context, false, materialWasChanged );
													materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );


													var batchInstancing = meshItem.BatchingInstanceBuffer != null;

													ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

													if( !batchInstancing )
														fixed( Matrix4F* p = &meshItem.Transform )
															Bgfx.SetTransform( (float*)p );

													var color = /*meshItem.Color * */ layer.MaterialColor;

													for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
													{
														var oper = meshData.RenderOperations[ nOperation ];

														BindRenderOperationData( context, frameData, materialData, batchInstancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, oper.BillboardDataMode, oper.BillboardDataImage, oper.MeshGeometryIndex );

														RenderOperation( context, oper, pass, null, meshItem.CutVolumes, meshItem.BatchingInstanceBuffer );
													}

												}
											}

										}
									}
								}
							}
						}
					}
				}
			}
		}

		protected unsafe virtual void RenderTransparentLayersOnOpaqueBaseObjectsForward( ViewportRenderingContext context, FrameData frameData )
		{
			Viewport viewportOwner = context.Owner;

			//get renderableGroupsToDraw
			Vector2I[] renderableGroupsToDraw;
			{
				var list = new OpenList<Vector2I>( frameData.RenderableGroupsInFrustum.Count );

				foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
				{
					bool add = false;

					if( renderableGroup.X == 0 )
					{
						ref var data = ref frameData.Meshes.Data[ renderableGroup.Y ];
						add = ( data.Flags & FrameData.MeshItem.FlagsEnum.ContainsTransparentLayersOnOpaqueBaseObjects ) != 0;
					}
					//else
					//{
					//	ref var data = ref frameData.Billboards.Data[ renderableGroup.Y ];
					//	add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.ContainsTransparentLayersOnOpaqueBaseObjects ) != 0;
					//}

					if( add )
						list.Add( renderableGroup );
				}

				renderableGroupsToDraw = list.ToArray();
			}

			if( renderableGroupsToDraw.Length == 0 )
				return;

			//sort by distance
			CollectionUtility.MergeSort( renderableGroupsToDraw, delegate ( Vector2I a, Vector2I b )
			{
				var distanceASquared = frameData.GetObjectGroupDistanceToCameraSquared( ref a );
				var distanceBSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref b );
				if( distanceASquared > distanceBSquared )
					return -1;
				if( distanceASquared < distanceBSquared )
					return 1;
				return 0;
			}, true );

			LightItem lightItemBinded = null;
			Material.CompiledMaterialData materialBinded = null;

			int ambientDirectionalLightCount = 0;
			foreach( var lightIndex in frameData.LightsInFrustumSorted )
			{
				var lightItem = frameData.Lights[ lightIndex ];
				if( lightItem.data.Type == Light.TypeEnum.Point || lightItem.data.Type == Light.TypeEnum.Spotlight )
					break;
				ambientDirectionalLightCount++;
			}

			int[] tempIntArray = Array.Empty<int>();
			int[] GetTempIntArray( int minSize )
			{
				if( tempIntArray.Length < minSize )
					tempIntArray = new int[ minSize ];
				return tempIntArray;
			}


			//bind textures for all render operations
			BindBrdfLUT( context );
			BindMaterialsTexture( context, frameData );


			//draw
			foreach( var renderableGroup in renderableGroupsToDraw )
			{
				if( renderableGroup.X == 0 )
				{
					//Mesh rendering

					ref var meshItem2 = ref frameData.Meshes.Data[ renderableGroup.Y ];
					ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
					var meshData = meshItem.MeshData;

					//!!!!не всегда нужно
					var affectedLightsCount = ambientDirectionalLightCount + meshItem2.PointSpotLightCount;
					var affectedLights = GetTempIntArray( affectedLightsCount );//var affectedLights = stackalloc int[ affectedLightsCount ];
					for( int n = 0; n < ambientDirectionalLightCount; n++ )
						affectedLights[ n ] = frameData.LightsInFrustumSorted[ n ];
					for( int n = 0; n < meshItem2.PointSpotLightCount; n++ )
						affectedLights[ ambientDirectionalLightCount + n ] = meshItem2.GetPointSpotLight( n );

					//render layers
					if( meshItem.Layers != null )
					{
						for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
						{
							ref var layer = ref meshItem.Layers[ nLayer ];
							var materialData = GetLayerMaterialData( ref layer, true );
							if( materialData == null )
								continue;

							bool add = materialData.Transparent;
							if( materialData.deferredShadingSupport && GetDeferredShading() && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
								add = false;
							if( !add )
								continue;

							for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
							{
								var lightIndex = affectedLights[ nAffectedLightIndex ];
								var lightItem = frameData.Lights[ lightIndex ];

								if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
								{
									//set lightItem uniforms
									if( lightItemBinded != lightItem )
									{
										lightItem.Bind( this, context );
										lightItemBinded = lightItem;
									}

									var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
									bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

									GpuMaterialPass pass;
									if( receiveShadows )
									{
										pass = passesGroup.passWithShadows;
										//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
										//	pass = passesGroup.passWithShadowsHigh;
										//else
										//	pass = passesGroup.passWithShadowsLow;
									}
									else
										pass = passesGroup.passWithoutShadows;

									ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

									bool materialWasChanged = materialBinded != materialData;
									materialBinded = materialData;

									//bind material data
									materialData.BindCurrentFrameData( context, false, materialWasChanged );
									materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );

									fixed( Matrix4F* p = &meshItem.Transform )
										Bgfx.SetTransform( (float*)p );

									var color = /*meshItem.Color * */ layer.MaterialColor;

									for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
									{
										var oper = meshData.RenderOperations[ nOperation ];

										BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, oper.BillboardDataMode, oper.BillboardDataImage, oper.MeshGeometryIndex );

										RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
									}
								}
							}
						}
					}

				}
			}
		}

		struct Render3DSceneForwardTransparentItem
		{
			public Vector2I index;
			public float distanceSquared;
		}

		void ApplyTransparentRenderingAddOffsetWhenSortByDistance( FrameData frameData, ref Render3DSceneForwardTransparentItem item )
		{
			bool applied = false;

			//!!!!slowly. много раз вызывается при сортировке

			//apply TransparentRenderingAddOffsetWhenSortByDistance
			switch( item.index.X )
			{
			case 0:
				if( frameData.RenderSceneData.Meshes.Data[ item.index.Y ].TransparentRenderingAddOffsetWhenSortByDistance )
				{
					//!!!!good?
					item.distanceSquared += 10000000;
					applied = true;
				}
				break;
			}

			//apply TransparentRenderingAddOffsetWhenSortByDistanceVolumes
			if( !applied )
			{
				var list = frameData.RenderSceneData.TransparentRenderingAddOffsetWhenSortByDistanceVolumes;
				if( list.Count != 0 )
				{
					if( frameData.GetObjectGroupBoundingBoxCenter( ref item.index, out var center ) )
					{
						for( int n = 0; n < list.Count; n++ )
						{
							ref var box = ref list.Data[ n ].Box;

							if( box.Contains( ref center ) )
							{
								//!!!!good?
								item.distanceSquared += 20000000;
								break;
							}
						}
					}
				}
			}
		}

		protected unsafe virtual void Render3DSceneForwardTransparent( ViewportRenderingContext context, FrameData frameData, ImageComponent colorDepthTextureCopy )
		{
			Viewport viewportOwner = context.Owner;

			//get renderableGroupsToDraw
			Render3DSceneForwardTransparentItem/*Vector2I*/[] renderableGroupsToDraw;
			{
				var list = new OpenList<Render3DSceneForwardTransparentItem/*Vector2I*/>( frameData.RenderableGroupsInFrustum.Count );

				foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
				{
					bool add = false;

					if( renderableGroup.X == 0 )
					{
						ref var data = ref frameData.Meshes.Data[ renderableGroup.Y ];
						add = ( data.Flags & FrameData.MeshItem.FlagsEnum.UseForwardTransparent ) != 0;
					}
					else
					{
						ref var data = ref frameData.Billboards.Data[ renderableGroup.Y ];
						add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseForwardTransparent ) != 0;
					}

					if( add )
					{
						var item = new Render3DSceneForwardTransparentItem();
						item.index = renderableGroup;
						item.distanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.index );
						ApplyTransparentRenderingAddOffsetWhenSortByDistance( frameData, ref item );

						list.Add( item );
						//list.Add( renderableGroup );
					}
				}

				renderableGroupsToDraw = list.ToArray();
			}

			if( renderableGroupsToDraw.Length == 0 )
				return;

			//sort by distance
			CollectionUtility.MergeSort( renderableGroupsToDraw, delegate ( Render3DSceneForwardTransparentItem a, Render3DSceneForwardTransparentItem b )
			{
				if( a.distanceSquared > b.distanceSquared )
					return -1;
				if( a.distanceSquared < b.distanceSquared )
					return 1;
				return 0;
			}, true );

			////sort by distance
			//CollectionUtility.MergeSort( renderableGroupsToDraw, delegate ( Vector2I a, Vector2I b )
			//{
			//	var distanceA = frameData.GetObjectGroupDistanceToCamera( ref a );
			//	var distanceB = frameData.GetObjectGroupDistanceToCamera( ref b );
			//	if( distanceA > distanceB )
			//		return -1;
			//	if( distanceA < distanceB )
			//		return 1;
			//	return 0;
			//}, true );

			LightItem lightItemBinded = null;
			Material.CompiledMaterialData materialBinded = null;

			int ambientDirectionalLightCount = 0;
			foreach( var lightIndex in frameData.LightsInFrustumSorted )
			{
				var lightItem = frameData.Lights[ lightIndex ];
				if( lightItem.data.Type == Light.TypeEnum.Point || lightItem.data.Type == Light.TypeEnum.Spotlight )
					break;
				ambientDirectionalLightCount++;
			}

			int[] tempIntArray = null;
			int[] GetTempIntArray( int minSize )
			{
				if( tempIntArray == null || tempIntArray.Length < minSize )
					tempIntArray = new int[ minSize ];
				return tempIntArray;
			}


			//bind textures for all render operations
			context.BindTexture( 5/*"s_colorDepthTextureCopy"*/, colorDepthTextureCopy ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None );
			//context.BindTexture( 5/*"s_colorDepthTextureCopy"*/, colorDepthTextureCopy ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point );
			BindBrdfLUT( context );
			BindMaterialsTexture( context, frameData );


			//draw
			for( int nRenderableGroupsToDraw = 0; nRenderableGroupsToDraw < renderableGroupsToDraw.Length; nRenderableGroupsToDraw++ )
			{
				var renderableGroup = renderableGroupsToDraw[ nRenderableGroupsToDraw ].index;

				if( renderableGroup.X == 0 )
				{
					//Mesh rendering

					ref var meshItem2 = ref frameData.Meshes.Data[ renderableGroup.Y ];
					ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
					var meshData = meshItem.MeshData;

					//!!!!не всегда нужно
					var affectedLightsCount = ambientDirectionalLightCount + meshItem2.PointSpotLightCount;
					var affectedLights = GetTempIntArray( affectedLightsCount );//var affectedLights = stackalloc int[ affectedLightsCount ];
					for( int n = 0; n < ambientDirectionalLightCount; n++ )
						affectedLights[ n ] = frameData.LightsInFrustumSorted[ n ];
					for( int n = 0; n < meshItem2.PointSpotLightCount; n++ )
						affectedLights[ ambientDirectionalLightCount + n ] = meshItem2.GetPointSpotLight( n );

					//get data for instancing
					int additionInstancingRendered = 0;
					if( Instancing && InstancingTransparent )
					{
						int nRenderableGroupsToDraw2 = nRenderableGroupsToDraw + 1;
						while( nRenderableGroupsToDraw2 < renderableGroupsToDraw.Length )
						{
							if( additionInstancingRendered >= InstancingMaxCount )
								break;

							var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw2 ].index;
							if( renderableGroup2.X == 0 )
							{
								ref var meshItem2_2 = ref frameData.Meshes.Data[ renderableGroup2.Y ];
								if( meshItem2.CanUseInstancingForTransparentWith( ref meshItem2_2 ) )
								{
									ref var meshItem_2 = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup2.Y ];
									if( meshItem.CanUseInstancingForTransparentWith( ref meshItem_2 ) )
									{
										additionInstancingRendered++;
										nRenderableGroupsToDraw2++;
										continue;
									}
								}
							}

							break;
						}

						if( additionInstancingRendered < InstancingMinCount )
							additionInstancingRendered = 0;
					}
					bool instancing = additionInstancingRendered != 0;


					//init instance buffer
					int instanceCount = 0;
					InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;
					if( instancing )
					{
						int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
						instanceCount = additionInstancingRendered + 1;

						if( InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) == instanceCount )
						{
							instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );

							//get instancing matrices
							RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
							int currentMatrix = 0;
							for( int n = 0; n < instanceCount; n++ )
							{
								var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw + n ].index;

								ref var meshItem_2 = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup2.Y ];
								meshItem_2.GetInstancingData( out instancingData[ currentMatrix++ ] );
							}
						}
						//else
						//	Log.Fatal( "InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) != instanceCount." );
					}


					//render mesh item
					if( !meshItem.OnlyForShadowGeneration )
					{
						for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
						{
							var oper = meshData.RenderOperations[ nOperation ];
							var materialData = GetMeshMaterialData( ref meshItem, oper, nOperation, true );

							bool add = materialData.Transparent;
							if( materialData.deferredShadingSupport && GetDeferredShading() && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
								add = false;
							if( !add )
								continue;

							for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
							{
								var lightIndex = affectedLights[ nAffectedLightIndex ];
								var lightItem = frameData.Lights[ lightIndex ];

								if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
								{
									//set lightItem uniforms
									if( lightItemBinded != lightItem )
									{
										lightItem.Bind( this, context );
										lightItemBinded = lightItem;
									}

									var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
									bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

									GpuMaterialPass pass;
									if( receiveShadows )
									{
										pass = passesGroup.passWithShadows;
										//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
										//	pass = passesGroup.passWithShadowsHigh;
										//else
										//	pass = passesGroup.passWithShadowsLow;
									}
									else
										pass = passesGroup.passWithoutShadows;

									ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

									bool materialWasChanged = materialBinded != materialData;
									materialBinded = materialData;

									materialData.BindCurrentFrameData( context, false, materialWasChanged );
									//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

									BindRenderOperationData( context, frameData, materialData, instancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref meshItem.Color, oper.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, oper.BillboardDataMode, oper.BillboardDataImage, oper.MeshGeometryIndex );

									if( instancing )
									{
										if( instanceBuffer != InstanceDataBuffer.Invalid )
											RenderOperation( context, oper, pass, null, null, ref instanceBuffer, instanceCount );
									}
									else
									{
										fixed( Matrix4F* p = &meshItem.Transform )
											Bgfx.SetTransform( (float*)p );
										RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
									}
								}
							}
						}

						//render layers
						if( meshItem2.Flags.HasFlag( FrameData.MeshItem.FlagsEnum.ContainsTransparentLayersOnTransparentBaseObjects ) && DebugDrawLayers )
						{
							if( meshItem.Layers != null )
							{
								for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
								{
									ref var layer = ref meshItem.Layers[ nLayer ];
									var materialData = GetLayerMaterialData( ref layer, true );
									if( materialData == null )
										continue;

									bool add = materialData.Transparent;
									if( materialData.deferredShadingSupport && GetDeferredShading() && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
										add = false;
									if( !add )
										continue;

									for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
									{
										var lightIndex = affectedLights[ nAffectedLightIndex ];
										var lightItem = frameData.Lights[ lightIndex ];

										if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
										{
											//set lightItem uniforms
											if( lightItemBinded != lightItem )
											{
												lightItem.Bind( this, context );
												lightItemBinded = lightItem;
											}

											var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
											bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

											GpuMaterialPass pass;
											if( receiveShadows )
											{
												pass = passesGroup.passWithShadows;
												//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
												//	pass = passesGroup.passWithShadowsHigh;
												//else
												//	pass = passesGroup.passWithShadowsLow;
											}
											else
												pass = passesGroup.passWithoutShadows;

											ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

											bool materialWasChanged = materialBinded != materialData;
											materialBinded = materialData;

											//bind material data
											materialData.BindCurrentFrameData( context, false, materialWasChanged );
											materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );
											//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

											fixed( Matrix4F* p = &meshItem.Transform )
												Bgfx.SetTransform( (float*)p );

											var color = /*meshItem.Color * */ layer.MaterialColor;

											for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
											{
												var oper = meshData.RenderOperations[ nOperation ];

												BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, oper.BillboardDataMode, oper.BillboardDataImage, oper.MeshGeometryIndex );

												RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
											}
										}
									}
								}
							}
						}
					}

					nRenderableGroupsToDraw += additionInstancingRendered;
				}
				else if( renderableGroup.X == 1 )
				{
					//Billboard rendering

					ref var billboardItem2 = ref frameData.Billboards.Data[ renderableGroup.Y ];
					ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];

					//!!!!не всегда нужно
					var affectedLightsCount = ambientDirectionalLightCount + billboardItem2.PointSpotLightCount;
					var affectedLights = GetTempIntArray( affectedLightsCount );//var affectedLights = stackalloc int[ affectedLightsCount ];
					for( int n = 0; n < ambientDirectionalLightCount; n++ )
						affectedLights[ n ] = frameData.LightsInFrustumSorted[ n ];
					for( int n = 0; n < billboardItem2.PointSpotLightCount; n++ )
						affectedLights[ ambientDirectionalLightCount + n ] = billboardItem2.GetPointSpotLight( n );

					var meshData = Billboard.GetBillboardMesh().Result.MeshData;

					//get data for instancing
					int additionInstancingRendered = 0;
					if( Instancing && InstancingTransparent )
					{
						int nRenderableGroupsToDraw2 = nRenderableGroupsToDraw + 1;
						while( nRenderableGroupsToDraw2 < renderableGroupsToDraw.Length )
						{
							if( additionInstancingRendered >= InstancingMaxCount )
								break;

							var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw2 ].index;
							if( renderableGroup2.X == 1 )
							{
								ref var billboardItem2_2 = ref frameData.Billboards.Data[ renderableGroup2.Y ];
								if( billboardItem2.CanUseInstancingForTransparentWith( ref billboardItem2_2 ) )
								{
									ref var billboardItem_2 = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup2.Y ];
									if( billboardItem.CanUseInstancingForTransparentWith( ref billboardItem_2 ) )
									{
										additionInstancingRendered++;
										nRenderableGroupsToDraw2++;
										continue;
									}
								}
							}

							break;
						}

						if( additionInstancingRendered < InstancingMinCount )
							additionInstancingRendered = 0;
					}
					bool instancing = additionInstancingRendered != 0;


					//init instance buffer
					int instanceCount = 0;
					InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;
					if( instancing )
					{
						int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
						instanceCount = additionInstancingRendered + 1;

						if( InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) == instanceCount )
						{
							instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );

							//get instancing matrices
							RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
							int currentMatrix = 0;
							for( int n = 0; n < instanceCount; n++ )
							{
								var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw + n ].index;

								ref var billboardItem_2 = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup2.Y ];
								billboardItem_2.GetInstancingData( out instancingData[ currentMatrix++ ] );
							}
						}
						//else
						//	Log.Fatal( "InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) != instanceCount." );
					}


					for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
					{
						var oper = meshData.RenderOperations[ nOperation ];

						var materialData = GetBillboardMaterialData( ref billboardItem, true );

						bool add = materialData.Transparent;
						if( materialData.deferredShadingSupport && GetDeferredShading() && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
							add = false;
						if( !add )
							continue;

						for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
						{
							var lightIndex = affectedLights[ nAffectedLightIndex ];
							var lightItem = frameData.Lights[ lightIndex ];

							if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
							{
								//set lightItem uniforms
								if( lightItemBinded != lightItem )
								{
									lightItem.Bind( this, context );
									lightItemBinded = lightItem;
								}

								var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
								bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

								GpuMaterialPass pass;
								if( receiveShadows )
								{
									pass = passesGroup.passWithShadows;
									//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
									//	pass = passesGroup.passWithShadowsHigh;
									//else
									//	pass = passesGroup.passWithShadowsLow;
								}
								else
									pass = passesGroup.passWithoutShadows;

								ForwardBindGeneralTexturesUniforms( context, frameData, ref billboardItem.BoundingSphere, lightItem, receiveShadows, true, false );

								bool materialWasChanged = materialBinded != materialData;
								materialBinded = materialData;

								materialData.BindCurrentFrameData( context, false, materialWasChanged );
								//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

								BindRenderOperationData( context, frameData, materialData, instancing, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, oper.UnwrappedUV, ref billboardItem.Color, oper.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, oper.BillboardDataMode, oper.BillboardDataImage, oper.MeshGeometryIndex );

								if( instancing )
								{
									if( instanceBuffer != InstanceDataBuffer.Invalid )
										RenderOperation( context, oper, pass, null, null, ref instanceBuffer, instanceCount );
								}
								else
								{
									billboardItem.GetWorldMatrix( out var worldMatrix );
									Bgfx.SetTransform( (float*)&worldMatrix );
									RenderOperation( context, oper, pass, null, null );
								}
							}
						}
					}

					nRenderableGroupsToDraw += additionInstancingRendered;
				}
			}
		}

		void RenderEffectsInternal( ViewportRenderingContext context, FrameData frameData, string groupName, ref ImageComponent actualTexture, bool onlyAO )
		{
			var group = GetComponent( groupName, true );
			if( group != null )
			{
				foreach( var effect in group.GetComponents<RenderingEffect>( false, false, true ) )
				{
					if( effect.IsSupported )
					{
						bool isAO = effect is RenderingEffect_AmbientOcclusion;
						if( onlyAO && isAO || !onlyAO && !isAO )
							effect.Render( context, frameData, ref actualTexture );
					}
				}
			}
		}

		public void CopyToCurrentViewport( ViewportRenderingContext context, ImageComponent sourceTexture, CanvasRenderer.BlendingType blending = CanvasRenderer.BlendingType.Opaque, bool flipY = false )
		{
			CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			shader.FragmentProgramFileName = @"Base\Shaders\Copy_fs.sc";

			//bind textures
			{
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/,
					sourceTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
				//var textureValue = new GpuMaterialPass.TextureParameterValue( sourceTexture,
				//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );
				//shader.Parameters.Set( "0"/* "sourceTexture"*/, textureValue, ParameterType.Texture2D );
			}

			context.RenderQuadToCurrentViewport( shader, blending, flipY );
			//context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Opaque );
			//context.RenderQuadToCurrentViewport( shader );
		}

		void RenderDeferredBackgroundColor( ViewportRenderingContext context )
		{
			if( deferredShadingData == null )
				InitDeferredShadingData();

			//create mesh
			if( deferredShadingData.clearBackgroundMesh == null )
			{
				deferredShadingData.clearBackgroundMesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );
				deferredShadingData.clearBackgroundMesh.CreateComponent<MeshGeometry_Box>();
				deferredShadingData.clearBackgroundMesh.Enabled = true;
			}

			//create rendering pass
			if( deferredShadingData.clearBackgroundPass == null )
			{
				//generate compile arguments
				var generalDefines = new List<(string, string)>();

				string error2;

				//vertex program
				var vertexProgram = GpuProgramManager.GetProgram( "DeferredBackgroundColor_Vertex_", GpuProgramType.Vertex,
					@"Base\Shaders\DeferredBackgroundColor_vs.sc", generalDefines, true, out error2 );
				if( !string.IsNullOrEmpty( error2 ) )
				{
					//!!!!
					return;
					//Log.Fatal( error2 );
				}

				//fragment program
				var fragmentProgram = GpuProgramManager.GetProgram( "DeferredBackgroundColor_Fragment_", GpuProgramType.Fragment,
					@"Base\Shaders\DeferredBackgroundColor_fs.sc", generalDefines, true, out error2 );
				if( !string.IsNullOrEmpty( error2 ) )
				{
					//!!!!
					return;
					//Log.Fatal( error2 );
				}

				var pass = new GpuMaterialPass( vertexProgram, fragmentProgram );

				pass.CullingMode = CullingMode.None;
				pass.DepthCheck = true;
				pass.DepthWrite = false;

				deferredShadingData.clearBackgroundPass = pass;
			}

			//render
			unsafe
			{
				var cameraSettings = context.Owner.CameraSettings;

				double scale;
				if( cameraSettings.Projection == ProjectionType.Orthographic )
					scale = Math.Max( cameraSettings.AspectRatio, 1 ) * cameraSettings.OrthographicHeight * 1.1;
				else
					scale = 1.1;

				//!!!!double
				var worldMatrix = new Matrix4F( Matrix3F.FromScale( (float)scale ), cameraSettings.Position.ToVector3F() );

				foreach( var item in deferredShadingData.clearBackgroundMesh.Result.MeshData.RenderOperations )
				{
					var generalContainer = new ParameterContainer();
					generalContainer.Set( "backgroundColor", context.GetBackgroundColor() );

					var containers = new List<ParameterContainer>();
					containers.Add( generalContainer );

					Bgfx.SetTransform( (float*)&worldMatrix );
					RenderOperation( context, item, deferredShadingData.clearBackgroundPass, containers, null );
				}
			}
		}

		public delegate void RenderDeferredShadingEndDelegate( RenderingPipeline_Basic sender, ViewportRenderingContext context, FrameData frameData, ref ImageComponent sceneTexture );
		public event RenderDeferredShadingEndDelegate RenderDeferredShadingEnd;

		public T GetSceneEffect<T>() where T : RenderingEffect
		{
			var group = GetComponent( "Scene Effects", true );
			if( group != null )
				return group.GetComponent<T>( false, true );
			return null;
		}

		protected virtual void RenderWithRenderTargets( ViewportRenderingContext context, FrameData frameData )
		{
			var owner = context.Owner;

			/////////////////////////////////////
			//prepare shadows
			{
				duringRenderShadows = true;
				SetCutVolumeSettingsUniforms( context, null, true );

				foreach( var lightIndex in frameData.LightsInFrustumSorted )
				{
					var item = frameData.Lights[ lightIndex ];
					if( item.prepareShadows )
						LightPrepareShadows( context, frameData, item );
				}

				duringRenderShadows = false;
				SetCutVolumeSettingsUniforms( context, null, true );
			}

			/////////////////////////////////////
			//detect destination size, SizeInPixelsLowResolutionBeforeUpscale, antialiasing and resolution upscale techniques
			RenderingEffect_Antialiasing antialiasing = null;
			int msaaLevel = 0;
			RenderingEffect_ResolutionUpscale resolutionUpscale = null;
			Vector2I destinationSize;
			{
				{
					var effect = GetSceneEffect<RenderingEffect_Antialiasing>();
					if( effect != null && effect.Intensity > 0 && effect.IsSupported )
					{
						antialiasing = effect;
						msaaLevel = effect.GetMSAALevel();
					}
				}

				{
					var effect = GetSceneEffect<RenderingEffect_ResolutionUpscale>();
					if( effect != null && /*effect.Intensity > 0 && */effect.IsSupported )
						resolutionUpscale = effect;
				}

				var size = owner.SizeInPixels.ToVector2();
				if( resolutionUpscale != null )
					size *= resolutionUpscale.GetResolutionMultiplier();

				context.SizeInPixelsLowResolutionBeforeUpscale = size.ToVector2I();

				if( antialiasing != null )
					size *= antialiasing.GetResolutionMultiplier();
				destinationSize = size.ToVector2I();
			}

			/////////////////////////////////////
			//create scene texture, draw background effects

			var sceneTexture = context.RenderTarget2D_Alloc( destinationSize, ( GetHighDynamicRange() && !GetDeferredShading() ) ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8, msaaLevel );
			context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, context.GetBackgroundColor() );
			//!!!!так? что с небом?
			//Background effects
			RenderEffectsInternal( context, frameData, "Background Effects", ref sceneTexture, false );

			/////////////////////////////////////
			//create normal texture
			ImageComponent normalTexture = null;
			if( GetUseMultiRenderTargets() )
			{
				normalTexture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.A8R8G8B8, msaaLevel );
				context.ObjectsDuringUpdate.namedTextures[ "normalTexture" ] = normalTexture;
			}

			/////////////////////////////////////
			//create motion vectors texture
			var motionBlur = GetSceneEffect<RenderingEffect_MotionBlur>();
			ImageComponent motionTexture = null;
			if( GetUseMultiRenderTargets() )
			{
				if( motionBlur != null && motionBlur.IsSupported || antialiasing != null && antialiasing.GetMotionTechnique() == RenderingEffect_Antialiasing.MotionTechniqueEnum.TAA )
				{
					motionTexture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.Float16GR );
					context.ObjectsDuringUpdate.namedTextures[ "motionTexture" ] = motionTexture;
				}
			}

			/////////////////////////////////////
			//create depth texture
			var depthTexture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.Depth24S8, msaaLevel );
			context.ObjectsDuringUpdate.namedTextures[ "depthTexture" ] = depthTexture;

			/////////////////////////////////////
			//clear normal, motion, depth textures
			if( GetUseMultiRenderTargets() )
			{
				MultiRenderTarget normalMotionDepthMRT;
				{
					var items = new List<MultiRenderTarget.Item>();
					items.Add( new MultiRenderTarget.Item( normalTexture ) );
					if( motionTexture != null )
						items.Add( new MultiRenderTarget.Item( motionTexture ) );
					items.Add( new MultiRenderTarget.Item( depthTexture ) );
					normalMotionDepthMRT = context.MultiRenderTarget_Create( items.ToArray() );
				}

				context.SetViewport( normalMotionDepthMRT.Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.All, ColorValue.Zero );
			}
			else
			{
				var sceneDepthMRT = context.MultiRenderTarget_Create( new[] {
					new MultiRenderTarget.Item( sceneTexture ),
					new MultiRenderTarget.Item( depthTexture ) } );
				context.SetViewport( sceneDepthMRT.Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Depth, ColorValue.Zero );
			}

			//!!!!double
			Matrix4F viewMatrix = owner.CameraSettings.ViewMatrix.ToMatrix4F();
			Matrix4F projectionMatrix = owner.CameraSettings.ProjectionMatrix.ToMatrix4F();

			//deferred shading
			if( GetDeferredShading() && DebugDrawDeferredPass )
			{
				context.ObjectsDuringUpdate.namedTextures[ "gBuffer0Texture" ] = sceneTexture;
				context.ObjectsDuringUpdate.namedTextures[ "gBuffer1Texture" ] = normalTexture;

				//!!!!maybe clear all together

				var gBuffer2Texture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.A8R8G8B8 );
				context.ObjectsDuringUpdate.namedTextures[ "gBuffer2Texture" ] = gBuffer2Texture;
				//no sense to clear, but clean gbuffer
				context.SetViewport( gBuffer2Texture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity,
					FrameBufferTypes.Color, ColorValue.Zero );

				var gBuffer3Texture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.A8R8G8B8 );//Float16RGBA
				context.ObjectsDuringUpdate.namedTextures[ "gBuffer3Texture" ] = gBuffer3Texture;
				//no sense to clear, but clean gbuffer
				context.SetViewport( gBuffer3Texture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity,
					FrameBufferTypes.Color, ColorValue.Zero );

				//create GBuffer 4 texture
				var gBuffer4Texture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.A8R8G8B8 );
				context.ObjectsDuringUpdate.namedTextures[ "gBuffer4Texture" ] = gBuffer4Texture;
				//no sense to clear, but clean gbuffer
				context.SetViewport( gBuffer4Texture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity,
					FrameBufferTypes.Color, ColorValue.Zero );

				//create GBuffer 5 texture
				var gBuffer5Texture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.A8R8G8B8 );
				context.ObjectsDuringUpdate.namedTextures[ "gBuffer5Texture" ] = gBuffer5Texture;
				//no sense to clear, but clean gbuffer
				context.SetViewport( gBuffer5Texture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity,
					FrameBufferTypes.Color, ColorValue.Zero );

				if( motionTexture != null )
					context.ObjectsDuringUpdate.namedTextures[ "gBuffer6Texture" ] = motionTexture;

				//create scene, normal, depth MRT
				MultiRenderTarget deferredMRT;
				{
					var items = new List<MultiRenderTarget.Item>();
					items.Add( new MultiRenderTarget.Item( sceneTexture ) );
					items.Add( new MultiRenderTarget.Item( normalTexture ) );
					items.Add( new MultiRenderTarget.Item( gBuffer2Texture ) );
					items.Add( new MultiRenderTarget.Item( gBuffer3Texture ) );
					items.Add( new MultiRenderTarget.Item( gBuffer4Texture ) );
					items.Add( new MultiRenderTarget.Item( gBuffer5Texture ) );
					if( motionTexture != null )
						items.Add( new MultiRenderTarget.Item( motionTexture ) );
					items.Add( new MultiRenderTarget.Item( depthTexture ) );
					deferredMRT = context.MultiRenderTarget_Create( items.ToArray() );
				}

				//render opaque objects (deferred)
				context.SetViewport( deferredMRT.Viewports[ 0 ], viewMatrix, projectionMatrix );
				Render3DSceneDeferred( context, frameData );

				//decals
				if( DebugDrawDecals && frameData.Decals.Count != 0 )
				{
					//make copy of gBuffer1Texture
					var gBuffer1TextureCopy = context.RenderTarget2D_Alloc( destinationSize, normalTexture/*gBuffer1Texture*/.Result.ResultFormat );
					context.SetViewport( gBuffer1TextureCopy.Result.GetRenderTarget().Viewports[ 0 ] );
					//, Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, ColorValue.Zero );
					CopyToCurrentViewport( context, normalTexture/*gBuffer1Texture*/ );

					//make copy of gBuffer4Texture
					var gBuffer4TextureCopy = context.RenderTarget2D_Alloc( destinationSize, gBuffer4Texture.Result.ResultFormat );
					context.SetViewport( gBuffer4TextureCopy.Result.GetRenderTarget().Viewports[ 0 ] );
					//, Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, ColorValue.Zero );
					CopyToCurrentViewport( context, gBuffer4Texture );

					//make copy of gBuffer5Texture
					var gBuffer5TextureCopy = context.RenderTarget2D_Alloc( destinationSize, gBuffer5Texture.Result.ResultFormat );
					context.SetViewport( gBuffer5TextureCopy.Result.GetRenderTarget().Viewports[ 0 ] );
					//, Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, ColorValue.Zero );
					CopyToCurrentViewport( context, gBuffer5Texture );

					//create scene, normal MRT
					MultiRenderTarget deferredMRT2;
					{
						var items = new List<MultiRenderTarget.Item>();
						items.Add( new MultiRenderTarget.Item( sceneTexture ) );
						items.Add( new MultiRenderTarget.Item( normalTexture ) );
						items.Add( new MultiRenderTarget.Item( gBuffer2Texture ) );
						items.Add( new MultiRenderTarget.Item( gBuffer3Texture ) );
						items.Add( new MultiRenderTarget.Item( gBuffer4Texture ) );
						items.Add( new MultiRenderTarget.Item( gBuffer5Texture ) );
						deferredMRT2 = context.MultiRenderTarget_Create( items.ToArray() );
					}

					context.SetViewport( deferredMRT2.Viewports[ 0 ], viewMatrix, projectionMatrix );
					RenderDecalsDeferred( context, frameData, depthTexture, gBuffer1TextureCopy, gBuffer4TextureCopy, gBuffer5TextureCopy );

					context.DynamicTexture_Free( gBuffer1TextureCopy );
					context.DynamicTexture_Free( gBuffer4TextureCopy );
					context.DynamicTexture_Free( gBuffer5TextureCopy );
				}

				////!!!!temp
				////save depth buffer to texture
				//PhysicallyCorrectRendering_CallSaveDepthBufferToTexture( context );

				var deferredLightTexture = context.RenderTarget2D_Alloc( destinationSize, GetHighDynamicRange() ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8 );

				//lighting pass for deferred shading
				context.SetViewport( deferredLightTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.Color, ColorValue.Zero );
				RenderLightsDeferred( context, frameData, sceneTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, depthTexture );

				//copy result of deferred shading to sceneTexture. save gBuffer0Texture
				sceneTexture = deferredLightTexture;
				//context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ] );
				//CopyToCurrentViewport( context, deferredLightTexture );

				RenderDeferredShadingEnd?.Invoke( this, context, frameData, ref sceneTexture );

				//context.RenderTarget_Free( deferredLightTexture );
			}

			//render opaque objects (forward)
			if( DebugDrawForwardOpaquePass )
			{
				MultiRenderTarget sceneNormalMotionDepthMRT;
				{
					var items = new List<MultiRenderTarget.Item>();
					items.Add( new MultiRenderTarget.Item( sceneTexture ) );
					if( GetUseMultiRenderTargets() )
					{
						items.Add( new MultiRenderTarget.Item( normalTexture ) );
						if( motionTexture != null )
							items.Add( new MultiRenderTarget.Item( motionTexture ) );
					}
					items.Add( new MultiRenderTarget.Item( depthTexture ) );
					sceneNormalMotionDepthMRT = context.MultiRenderTarget_Create( items.ToArray() );
				}

				context.SetViewport( sceneNormalMotionDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix );
				Render3DSceneForwardOpaque( context, frameData );
				//Render3DSceneForward_NoInstancing( context, frameData, false );
			}

			if( frameData.Sky != null )
			{
				//create scene, depth MRT
				var sceneDepthMRT = context.MultiRenderTarget_Create( new[] {
					new MultiRenderTarget.Item( sceneTexture ),
					new MultiRenderTarget.Item( depthTexture ) } );
				context.SetViewport( sceneDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix );

				//context.SetViewport(sceneTexture.Result.GetRenderTarget().Viewports[0], viewMatrix, projectionMatrix);
				RenderSky( context, frameData );
			}
			else
			{
				//deferred. clear not filled background with background color.
				if( GetDeferredShading() && context.GetBackgroundColor() != new ColorValue( 0, 0, 0 ) )
				{
					//create scene, depth MRT
					var sceneDepthMRT = context.MultiRenderTarget_Create( new[] {
						new MultiRenderTarget.Item( sceneTexture ),
						new MultiRenderTarget.Item( depthTexture ) } );
					context.SetViewport( sceneDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix );

					RenderDeferredBackgroundColor( context );
				}
			}

			//render transparent layers on opaque base objects (forward)
			if( DebugDrawForwardTransparentPass && DebugDrawLayers )
			{
				//create scene, motion, depth MRT
				MultiRenderTarget sceneNormalMotionDepthMRT;
				{
					var items = new List<MultiRenderTarget.Item>();
					items.Add( new MultiRenderTarget.Item( sceneTexture ) );
					if( GetUseMultiRenderTargets() )
					{
						items.Add( new MultiRenderTarget.Item( normalTexture ) );
						if( motionTexture != null )
							items.Add( new MultiRenderTarget.Item( motionTexture ) );
					}
					items.Add( new MultiRenderTarget.Item( depthTexture ) );
					sceneNormalMotionDepthMRT = context.MultiRenderTarget_Create( items.ToArray() );
				}

				context.SetViewport( sceneNormalMotionDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix );
				RenderTransparentLayersOnOpaqueBaseObjectsForward( context, frameData );
			}

			//!!!!тоже типа как RenderDeferredShadingEnd сделать
			//AO effect
			//? AO не должен создавать, подменять sceneTexture
			if( owner.Mode == Viewport.ModeEnum.Default )
				RenderEffectsInternal( context, frameData, "Scene Effects", ref sceneTexture, true );

			//fog
			if( RenderingSystem.Fog && frameData.Fog != null )
			{
				context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix );

				CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\Fog_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"depthTexture"*/,
					depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
				//shader.Parameters.Set( "0"/*"depthTexture"*/, new GpuMaterialPass.TextureParameterValue( depthTexture,
				//	TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				shader.Parameters.Set( "affectBackground", new Vector4F( (float)frameData.Fog.AffectBackground.Value, 0, 0, 0 ) );

				context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.AlphaBlend );
			}

			//render transparent objects (forward)
			if( DebugDrawForwardTransparentPass )
			{
				//make copy of scene color and depth textures to use as a sampler
				ImageComponent colorDepthTextureCopy = null;
				if( IsProvideColorDepthTextureCopy() )
				{
					//!!!!no sense to use Float32 for color, but depth must be Float32
					colorDepthTextureCopy = context.RenderTarget2D_Alloc( sceneTexture.Result.ResultSize, PixelFormat.Float32RGBA );
					//colorDepthTextureCopy = context.RenderTarget2D_Alloc( sceneTexture.Result.ResultSize, PixelFormat.Float32GR );
					context.SetViewport( colorDepthTextureCopy.Result.GetRenderTarget().Viewports[ 0 ] );

					CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
					shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
					shader.FragmentProgramFileName = @"Base\Shaders\MakeColorDepthTextureCopy_fs.sc";

					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"colorTexture"*/, sceneTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
					shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"depthTexture"*/, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					context.RenderQuadToCurrentViewport( shader );
				}

				////make copy of depth texture to use as a sampler (for soft particles)
				////must use Float32R format, because can't use Depth24S8 as render target
				//ImageComponent depthTextureCopy = null;
				//if( IsAllowSoftParticles() )
				//{
				//	depthTextureCopy = context.RenderTarget2D_Alloc( depthTexture.Result.ResultSize, PixelFormat.Float32R );
				//	context.SetViewport( depthTextureCopy.Result.GetRenderTarget().Viewports[ 0 ] );
				//	CopyToCurrentViewport( context, depthTexture );
				//}

				ImageComponent dummyNormalTexture = null;
				if( GetUseMultiRenderTargets() )
					dummyNormalTexture = context.RenderTarget2D_Alloc( normalTexture.Result.ResultSize, normalTexture.Result.ResultFormat );

				//create scene, motion, depth MRT
				MultiRenderTarget sceneMotionDepthMRT;
				{
					var items = new List<MultiRenderTarget.Item>();
					items.Add( new MultiRenderTarget.Item( sceneTexture ) );
					if( GetUseMultiRenderTargets() )
					{
						items.Add( new MultiRenderTarget.Item( dummyNormalTexture ) );// normalTexture ) );
						if( motionTexture != null )
							items.Add( new MultiRenderTarget.Item( motionTexture ) );
					}
					items.Add( new MultiRenderTarget.Item( depthTexture ) );
					sceneMotionDepthMRT = context.MultiRenderTarget_Create( items.ToArray() );
				}

				////create scene, depth MRT
				//var sceneDepthMRT = context.MultiRenderTarget_Create( new[] {
				//	new MultiRenderTarget.Item( sceneTexture ),
				//	new MultiRenderTarget.Item( depthTexture ) } );

				context.SetViewport( sceneMotionDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix );
				Render3DSceneForwardTransparent( context, frameData, colorDepthTextureCopy );

				if( colorDepthTextureCopy != null )
					context.DynamicTexture_Free( colorDepthTextureCopy );

				if( dummyNormalTexture != null )
					context.DynamicTexture_Free( dummyNormalTexture );
			}

			//Scene effects (skip AO)
			if( owner.Mode == Viewport.ModeEnum.Default )
			{
				//antialiasing: downscale
				if( antialiasing != null && antialiasing.DownscaleBeforeSceneEffects && antialiasing.IsSupported )
					antialiasing.RenderDownscale( context, frameData, /*sceneTexture.Result.ResultSize, */ref sceneTexture );

				RenderEffectsInternal( context, frameData, "Scene Effects", ref sceneTexture, false );
			}

			//outline effect for selected objects
			if( context.ObjectInSpaceRenderingContext.selectedObjects.Count != 0 )
				RenderOutlineEffectForSelectedObjects( context, frameData, ref sceneTexture );

			//convert to LDR
			ConvertToLDR( context, ref sceneTexture );

			/////////////////////////////////////
			//Simple 3D Renderer data
			if( owner.Simple3DRenderer != null )
			{
				if( DebugDrawSimple3DRenderer )
				{
					if( owner.Simple3DRenderer.ViewportRendering_PrepareRenderables() )
					{
						duringRenderSimple3DRenderer = true;
						SetCutVolumeSettingsUniforms( context, null, true );

						if( GetSimpleGeometryAntialiasing() )
						{
							//!!!!owner.SizeInPixels * 2
							var simpleDataTexture = context.RenderTarget2D_Alloc( owner.SizeInPixels * 2, PixelFormat.A8R8G8B8 );
							context.SetViewport( simpleDataTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.Color, new ColorValue( 0, 0, 0, 0 ) );

							CopyToCurrentViewport( context, sceneTexture );

							//render simple geometry
							owner.Simple3DRenderer.ViewportRendering_RenderToCurrentViewport( context );

							//copy to scene texture with downscale
							{
								context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ] );

								CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
								shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
								shader.FragmentProgramFileName = @"Base\Shaders\Effects\Downscale2_fs.sc";

								shader.Parameters.Set( "sourceSizeInv", new Vector2F( 1, 1 ) / simpleDataTexture.Result.ResultSize.ToVector2F() );

								//bind textures
								{
									shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, simpleDataTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
									//FilterOption.Point, FilterOption.Point, FilterOption.Point
								}

								//context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.AlphaBlend );
								context.RenderQuadToCurrentViewport( shader );
							}

							context.DynamicTexture_Free( simpleDataTexture );

							//var simpleDataTexture = context.RenderTarget2D_Alloc( owner.SizeInPixels, PixelFormat.A8R8G8B8, 4 );

							////clear
							////!!!!Depth
							//context.SetViewport( simpleDataTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.Color | FrameBufferTypes.Depth, new ColorValue( 0, 0, 0, 0 ) );
							////context.SetViewport( simpleDataTexture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, new ColorValue( 0, 0, 0, 0 ) );

							//////create scene, depth MRT
							////var simpleDataWithDepth = context.MultiRenderTarget_Create( new[] {
							////	new MultiRenderTarget.Item( simpleDataTexture ),
							////	new MultiRenderTarget.Item( depthTexture ) } );
							////context.SetViewport( simpleDataWithDepth.Viewports[ 0 ], viewMatrix, projectionMatrix );

							//owner.Simple3DRenderer._ViewportRendering_RenderToCurrentViewport( context );

							////copy to sceneTexture
							//context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix );
							//CopyToCurrentViewport( context, simpleDataTexture, CanvasRenderer.BlendingType.AlphaBlend );
						}
						else
						{
							context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix );
							owner.Simple3DRenderer.ViewportRendering_RenderToCurrentViewport( context );
						}

						duringRenderSimple3DRenderer = false;
						SetCutVolumeSettingsUniforms( context, null, true );
					}
				}

				//!!!!!когда еще чистить?
				owner.Simple3DRenderer.ViewportRendering_Clear();
			}

			/////////////////////////////////////
			//GUI
			if( DebugDrawUI && owner.CanvasRenderer != null )
			{
				context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ] );
				owner.CanvasRenderer.ViewportRendering_RenderToCurrentViewport( context, false, owner.LastUpdateTime );
			}
			//Texture guiTexture;
			//{
			//	//!!!!A8R8G8B8?
			//	guiTexture = context.RenderTarget2D_Alloc( owner.SizeInPixels, PixelFormat.A8R8G8B8 );

			//	//!!!!!EngineDebugSettings.DrawScreenGUI

			//	Viewport viewport = guiTexture.Result.GetRenderTarget().Viewports[ 0 ];
			//	context.SetViewport( viewport, Mat4F.Identity, Mat4F.Identity, FrameBufferTypes.Color, ColorValue.Zero );

			//	owner.CanvasRenderer._ViewportRendering_RenderToCurrentViewport( context, false, owner.LastUpdateTime );
			//	//owner.CanvasRenderer._ViewportRendering_RenderToCurrentViewport( context, true, owner.LastUpdateTime );

			//	//GUI effects
			//	RenderEffectsInternal( context, "GUI Effects", ref guiTexture, false );
			//}

			//Final image effects
			if( owner.Mode == Viewport.ModeEnum.Default )
				RenderEffectsInternal( context, frameData, "Final Image Effects", ref sceneTexture, false );

			/////////////////////////////////////
			//Copy to output target
			context.SetViewport( owner );
			CopyToCurrentViewport( context, sceneTexture, flipY: owner.OutputFlipY );

			///////////////////////////////////////
			////Render video to file
			//RenderVideoToFile( context, sceneTexture );

			//clear or destroy something maybe
		}

		protected virtual void RenderWithoutRenderTargets( ViewportRenderingContext context, FrameData frameData )
		{
			var owner = context.Owner;

			if( context.Owner.Parent is RenderTexture renderTexture )
			{
				//render to RenderTexture
				//for rendering to texture anyway must use additional targets, or depth will be broken

				Matrix4F viewMatrix = owner.CameraSettings.ViewMatrix.ToMatrix4F();
				Matrix4F projectionMatrix = owner.CameraSettings.ProjectionMatrix.ToMatrix4F();

				var backgroundColor = context.GetBackgroundColor();
				if( DebugMode.Value != DebugModeEnum.None )
					backgroundColor = new ColorValue( 0, 0, 0 );

				//create scene texture
				var sceneTexture = context.RenderTarget2D_Alloc( context.owner.SizeInPixels, renderTexture.Creator.ResultFormat, 0 );

				//create depth texture
				var depthTexture = context.RenderTarget2D_Alloc( owner.SizeInPixels, PixelFormat.Depth24S8, 0 );
				context.ObjectsDuringUpdate.namedTextures[ "depthTexture" ] = depthTexture;

				var sceneDepthMRT = context.MultiRenderTarget_Create( new[] {
					new MultiRenderTarget.Item( sceneTexture ),
					new MultiRenderTarget.Item( depthTexture ) } );
				context.SetViewport( sceneDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.All, backgroundColor );

				/////////////////////////////////////
				//Scene
				if( DebugDrawForwardOpaquePass )
					Render3DSceneForwardOpaque( context, frameData );
				RenderSky( context, frameData );
				if( DebugDrawForwardTransparentPass )
					Render3DSceneForwardTransparent( context, frameData, null );

				/////////////////////////////////////
				//debug geometry
				if( owner.Simple3DRenderer != null )
				{
					if( DebugDrawSimple3DRenderer )
					{
						if( owner.Simple3DRenderer.ViewportRendering_PrepareRenderables() )
						{
							duringRenderSimple3DRenderer = true;
							SetCutVolumeSettingsUniforms( context, null, true );

							owner.Simple3DRenderer.ViewportRendering_RenderToCurrentViewport( context );

							duringRenderSimple3DRenderer = false;
							SetCutVolumeSettingsUniforms( context, null, true );
						}
					}
					owner.Simple3DRenderer.ViewportRendering_Clear();
				}

				/////////////////////////////////////
				//GUI
				if( DebugDrawUI && owner.CanvasRenderer != null )
				{
					context.SetViewport( sceneDepthMRT.Viewports[ 0 ] );
					owner.CanvasRenderer.ViewportRendering_RenderToCurrentViewport( context, false, owner.LastUpdateTime );
				}

				/////////////////////////////////////
				//Copy to output target
				context.SetViewport( owner );
				CopyToCurrentViewport( context, sceneTexture, flipY: owner.OutputFlipY );
			}
			else
			{
				//render to RenderWindow

				Matrix4F viewMatrix = owner.CameraSettings.ViewMatrix.ToMatrix4F();
				Matrix4F projectionMatrix = owner.CameraSettings.ProjectionMatrix.ToMatrix4F();

				var backgroundColor = context.GetBackgroundColor();
				if( DebugMode.Value != DebugModeEnum.None )
					backgroundColor = new ColorValue( 0, 0, 0 );

				context.SetViewport( owner, viewMatrix, projectionMatrix, FrameBufferTypes.All, backgroundColor );

				/////////////////////////////////////
				//Scene
				if( DebugDrawForwardOpaquePass )
					Render3DSceneForwardOpaque( context, frameData );
				RenderSky( context, frameData );
				if( DebugDrawForwardTransparentPass )
					Render3DSceneForwardTransparent( context, frameData, null );

				/////////////////////////////////////
				//debug geometry
				if( owner.Simple3DRenderer != null )
				{
					if( DebugDrawSimple3DRenderer )
					{
						if( owner.Simple3DRenderer.ViewportRendering_PrepareRenderables() )
							owner.Simple3DRenderer.ViewportRendering_RenderToCurrentViewport( context );
					}
					owner.Simple3DRenderer.ViewportRendering_Clear();
				}

				/////////////////////////////////////
				//GUI
				if( DebugDrawUI && owner.CanvasRenderer != null )
				{
					context.SetViewport( owner );
					owner.CanvasRenderer.ViewportRendering_RenderToCurrentViewport( context, false, owner.LastUpdateTime );
				}
			}
		}

		public delegate void RenderBeginDelegate( RenderingPipeline_Basic sender, ViewportRenderingContext context, FrameData frameData );
		public event RenderBeginDelegate RenderBegin;

		public delegate void RenderEndDelegate( RenderingPipeline_Basic sender, ViewportRenderingContext context, FrameData frameData );
		public event RenderEndDelegate RenderEnd;

		public override void Render( ViewportRenderingContext context )
		{
			if( context.FrameData != null )
				Log.Fatal( "RenderingPipeline_Basic: Render: context.FrameData != null." );

			//!!!!create each time?
			var frameData = new FrameData();
			context.FrameData = frameData;

			context.SizeInPixelsLowResolutionBeforeUpscale = context.Owner.SizeInPixels;
			context.OwnerCameraSettings = context.Owner.CameraSettings;
			context.OwnerCameraSettingsPosition = context.OwnerCameraSettings.Position;
			context.ShadowObjectVisibilityDistanceFactor = ShadowObjectVisibilityDistanceFactor;
			context.UpdateGetVisibilityDistanceByObjectSize();

			duringRenderShadows = false;
			duringRenderSimple3DRenderer = false;

			RenderBegin?.Invoke( this, context, frameData );

			//set viewport to give the ability to use compute shaders
			{
				//!!!!double
				var owner = context.Owner;
				Matrix4F viewMatrix = owner.CameraSettings.ViewMatrix.ToMatrix4F();
				Matrix4F projectionMatrix = owner.CameraSettings.ProjectionMatrix.ToMatrix4F();

				context.SetViewport( owner, viewMatrix, projectionMatrix, 0, ColorValue.Zero );
			}

			SetViewportOwnerSettingsUniform( context );

			//get lists of visible objects
			PrepareListsOfObjects( context, frameData );

			//additional actions after PrepareListsOfObjects sorted by camera distance from far to near
			{
				CollectionUtility.MergeSort( context.FrameData.RenderSceneData.ActionsToDoAfterPrepareListsOfObjectsSortedByDistance, delegate ( RenderSceneData.ActionToDoAfterPrepareListsOfObjectsSortedByDistance item1, RenderSceneData.ActionToDoAfterPrepareListsOfObjectsSortedByDistance item2 )
				{
					if( item1.DistanceToCamera > item2.DistanceToCamera )
						return -1;
					if( item1.DistanceToCamera < item2.DistanceToCamera )
						return 1;
					return 0;
				}, true );

				foreach( var item in context.FrameData.RenderSceneData.ActionsToDoAfterPrepareListsOfObjectsSortedByDistance )
					item.Action( context );
			}

			//display additional data
			if( context.Owner.AttachedScene != null && context.Owner.Simple3DRenderer != null )
			{
				//display physical objects
				DisplayPhysicalObjects( context, frameData );

				//display bounds for ObjectInSpace
				if( context.Owner.AttachedScene.GetDisplayDevelopmentDataInThisApplication() && context.Owner.AttachedScene.DisplayObjectInSpaceBounds )
					DisplayObjectInSpaceBounds( context, frameData );

				//sort and display object's labels
				SortObjectInSpaceLabels( context );
				DisplayObjectInSpaceLabels( context );
			}

			//render UI. must call before PrepareListsOfObjects.
			context.Owner.PerformUpdateBeforeOutputEvents();

			//prepare materials
			PrepareMaterialsTexture( context, frameData );

			//set view, projection matrices previous frame
			unsafe
			{
				var viewProjMatrix = context.Owner.ProjectionMatrixPreviousFrame * context.Owner.ViewMatrixPreviousFrame;
				//!!!!double
				var matrixF = viewProjMatrix.ToMatrix4F();
				context.SetUniform( "u_viewProjPrevious", ParameterType.Matrix4x4, 1, &matrixF );
			}

			SetFogUniform( context, frameData.Fog );
			SetCutVolumeSettingsUniforms( context, null, true );

			Bgfx.SetDebugFeatures( DebugMode.Value == DebugModeEnum.Wireframe ? DebugFeatures.Wireframe : DebugFeatures.None );

			UpdateNullShadowTextures();

			outputInstancingManager.Init( this );

			if( UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
				RenderWithRenderTargets( context, frameData );
			else
				RenderWithoutRenderTargets( context, frameData );

			RenderEnd?.Invoke( this, context, frameData );

			ClearTempData( context, frameData );
			context.FrameData = null;

			//clear or destroy something maybe
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential )]
		struct ViewportOwnerSettingsUniform
		{
			public Vector3F cameraPosition;
			public float nearClipDistance;

			public float farClipDistance;
			public float fieldOfView;
			public float debugMode;
			public float emissiveMaterialsFactor;//public float cameraEv100;//public float unused1;

			public Vector3F shadowFarDistance;
			public float cameraExposure;

			public float displacementScale;
			public float displacementMaxSteps;
			public float removeTextureTiling;
			public float provideColorDepthTextureCopy;

			public Vector3F cameraDirection;
			public float engineTime;

			public Vector3F cameraUp;
			public float mipBias;

			public Vector2F windSpeed;
			public float shadowObjectVisibilityDistanceFactor;
			public float unused1;
		}

		public unsafe void SetViewportOwnerSettingsUniform( ViewportRenderingContext context, DebugModeEnum? overrideDebugMode = null )
		{
			var cameraSettings = context.Owner.CameraSettings;

			//!!!!double

			var data = new ViewportOwnerSettingsUniform();
			data.cameraPosition = cameraSettings.Position.ToVector3F();
			data.nearClipDistance = (float)cameraSettings.NearClipDistance;
			data.farClipDistance = (float)cameraSettings.FarClipDistance;
			data.fieldOfView = (float)cameraSettings.FieldOfView.InRadians();
			data.debugMode = (float)( overrideDebugMode.HasValue ? overrideDebugMode.Value : DebugMode.Value );

			//shadowFarDistance
			{
				float farDistance = (float)ShadowFarDistance;
				//!!!!?
				float shadowTextureFadeStart = .9f;
				float fadeMinDistance = farDistance * shadowTextureFadeStart;
				data.shadowFarDistance = new Vector3F(
					farDistance,
					farDistance - fadeMinDistance * 2.0f,
					1.0f / ( Math.Max( farDistance - fadeMinDistance, .0001f ) ) );
			}

			data.emissiveMaterialsFactor = (float)cameraSettings.EmissiveFactor;
			data.cameraExposure = (float)cameraSettings.Exposure;
			data.displacementScale = (float)DisplacementMappingScale;
			data.displacementMaxSteps = DisplacementMappingMaxSteps;
			data.removeTextureTiling = (float)RemoveTextureTiling;
			data.provideColorDepthTextureCopy = IsProvideColorDepthTextureCopy() ? 1.0f : 0.0f;

			data.cameraDirection = cameraSettings.Rotation.GetForward().ToVector3F();
			data.engineTime = (float)EngineApp.EngineTime;

			data.cameraUp = cameraSettings.Rotation.GetUp().ToVector3F();

			var resolutionUpscale = GetSceneEffect<RenderingEffect_ResolutionUpscale>();
			if( resolutionUpscale != null )
				data.mipBias = (float)resolutionUpscale.GetMipBias();

			var scene = context.Owner.AttachedScene;
			if( scene != null )
				data.windSpeed = scene.GetWindSpeedVector().ToVector2F();

			data.shadowObjectVisibilityDistanceFactor = (float)ShadowObjectVisibilityDistanceFactor;

			int vec4Count = sizeof( ViewportOwnerSettingsUniform ) / sizeof( Vector4F );
			if( vec4Count != 7 )
				Log.Fatal( "RenderingPipeline: Render: vec4Count != 7." );
			context.SetUniform( "u_viewportOwnerSettings", ParameterType.Vector4, vec4Count, &data );
		}

		static OpenList<RenderSceneData.CutVolumeItem> tempCutVolumes;
		static Matrix4F[] tempCutVolumesMatrixArray;

		internal protected override unsafe void SetCutVolumeSettingsUniforms( ViewportRenderingContext context, RenderSceneData.CutVolumeItem[] cutVolumes, bool forceUpdate )
		{

			//!!!!maybe send indexes of cut volumes. make cutvolumes texture like materials texture or combine material and cutvolumes texture


			if( RenderingSystem.CutVolumeMaxAmount <= 0 )
				return;
			//compare cut volumes of object
			var needUpdate = !IsEqualCutVolumes( context.CurrentCutVolumes, cutVolumes ) || forceUpdate;
			if( !needUpdate )
				return;

			context.CurrentCutVolumes = cutVolumes;

			var list = context.FrameData.RenderSceneData.CutVolumes;
			if( cutVolumes != null )
			{
				if( tempCutVolumes == null )
					tempCutVolumes = new OpenList<RenderSceneData.CutVolumeItem>();
				tempCutVolumes.Clear();

				for( int n = 0; n < list.Count; n++ )
					tempCutVolumes.Add( ref list.Data[ n ] );
				for( int n = 0; n < cutVolumes.Length; n++ )
					tempCutVolumes.Add( ref cutVolumes[ n ] );

				list = tempCutVolumes;
			}

			if( tempCutVolumesMatrixArray == null )
				tempCutVolumesMatrixArray = new Matrix4F[ RenderingSystem.CutVolumeMaxAmount ];
			var data = tempCutVolumesMatrixArray;
			//var data = stackalloc Matrix4F[ GLOBAL_CUT_VOLUME_MAX_COUNT ];
			//NativeUtility.ZeroMemory( data, sizeof( Matrix4F ) * GLOBAL_CUT_VOLUME_MAX_COUNT );

			var count = 0;

			var duringRenderScene = !duringRenderShadows && !duringRenderSimple3DRenderer;

			for( int n = 0; n < list.Count; n++ )
			{
				if( count >= RenderingSystem.CutVolumeMaxAmount )
					break;

				ref var item = ref list.Data[ n ];

				if( ( !duringRenderScene || item.CutScene ) && ( !duringRenderShadows || item.CutShadows ) && ( !duringRenderSimple3DRenderer || item.CutSimple3DRenderer ) )
				{
					//if( item.Shape == CutVolumeShape.Plane )
					//{
					//	//!!!!double
					//	new Matrix4F( item.Plane.ToPlaneF().ToVector4F(), Vector4F.Zero, Vector4F.Zero, Vector4F.Zero ).GetTranspose( out data[ count ] );
					//}
					//else
					//{
					item.Transform.ToMatrix4().GetInverse( out var inv );
					//!!!!double
					inv.ToMatrix4F( out data[ count ] );
					//}

					//save shape type in the matrix
					var item3W = (int)item.Shape + 1;
					if( item.Invert )
						item3W = -item3W;
					data[ count ].Item3.W = item3W;

					count++;
				}
			}

			if( u_viewportCutVolumeSettings == null )
				u_viewportCutVolumeSettings = GpuProgramManager.RegisterUniform( "u_viewportCutVolumeSettings", UniformType.Vector4, 1 );
			if( u_viewportCutVolumeData == null )
				u_viewportCutVolumeData = GpuProgramManager.RegisterUniform( "u_viewportCutVolumeData", UniformType.Matrix4x4, RenderingSystem.CutVolumeMaxAmount );

			var settings = new Vector4F( count, 0, 0, 0 );
			context.SetUniform( u_viewportCutVolumeSettings.Value, ParameterType.Vector4, 1, &settings );
			if( count != 0 )
			{
				fixed( Matrix4F* pData = data )
					context.SetUniform( u_viewportCutVolumeData.Value, ParameterType.Matrix4x4, RenderingSystem.CutVolumeMaxAmount, pData );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public virtual void GetBackgroundEnvironmentData( ViewportRenderingContext context, FrameData frameData, out EnvironmentTextureData? texture, out EnvironmentIrradianceData? harmonics )
		{
			//Sky
			if( context.Owner.AttachedScene != null && frameData.Sky != null )
			{
				if( frameData.Sky.GetEnvironmentTextureData( out var texture2, out var textureIBL2 ) )
				{
					texture = texture2;
					harmonics = textureIBL2;
					return;
				}

				//var skybox = frameData.Sky as Skybox;
				//if( skybox != null )
				//{
				//	skybox.GetEnvironmentCubemaps( out var environmentCubemap, out var irradianceCubemap );
				//	skybox.GetRotationMatrix( out var rotation );
				//	var multiplier = skybox.Multiplier.Value.ToVector3F() * skybox.MultiplierReflection.Value.ToVector3F();

				//	if( environmentCubemap != null )
				//		texture = new EnvironmentTextureData( environmentCubemap, ref rotation, ref multiplier );
				//	else
				//		texture = new EnvironmentTextureData( ResourceUtility.GrayTextureCube );

				//	if( irradianceCubemap != null )
				//		textureIBL = new EnvironmentTextureData( irradianceCubemap, ref rotation, ref multiplier );
				//	else
				//		textureIBL = new EnvironmentTextureData( ResourceUtility.GrayTextureCube );

				//	return;
				//}
			}

			//BackgroundColor
			{
				ColorValue color;
				if( context.Owner.AttachedScene != null && context.Owner.AttachedScene.BackgroundColorEnvironmentOverride.HasValue )
					color = context.Owner.AttachedScene.BackgroundColorEnvironmentOverride.Value;
				else
				{
					color = context.GetBackgroundColor();
					if( DebugMode.Value != DebugModeEnum.None )
						color = new ColorValue( 0, 0, 0 );
				}

				var affect = context.GetBackgroundColorAffectLighting();

				var rotation = QuaternionF.Identity;
				var multiplier = color.ToVector3F();
				texture = new EnvironmentTextureData( ResourceUtility.WhiteTextureCube, affect, ref rotation, ref multiplier );
				harmonics = new EnvironmentIrradianceData( EnvironmentIrradianceData.WhiteHarmonics, affect, ref rotation, ref multiplier );
				//textureIBL = new EnvironmentTextureData( ResourceUtility.WhiteTextureCube, affect, ref rotation, ref multiplier );
			}
		}

		void GetEnvironmentTexturesByBoundingSphereIntersection( ViewportRenderingContext context, FrameData frameData, ref Sphere objectSphere,
			out EnvironmentTextureData? texture1, out EnvironmentIrradianceData? harmonics1,
			out EnvironmentTextureData? texture2, out EnvironmentIrradianceData? harmonics2,
			out float blendingFactor )
		{
			texture1 = null;
			harmonics1 = null;
			texture2 = null;
			harmonics2 = null;
			blendingFactor = 1;

			if( context.Owner.AttachedScene != null )
			{
				ReflectionProbeItem probe1 = null;
				double probe1Intensity = 0;
				ReflectionProbeItem probe2 = null;
				double probe2Intensity = 0;

				//!!!!slowly

				//smallest first, biggest last
				for( int nProbe = frameData.ReflectionProbes.Count - 1; nProbe >= 0; nProbe-- )
				{
					var probe = frameData.ReflectionProbes[ nProbe ];
					var probeSphere = probe.data.Sphere;

					if( probeSphere.Intersects( ref objectSphere ) )
					{
						var d = ( probeSphere.Center - objectSphere.Center ).Length();
						var intensity = MathEx.Saturate( probeSphere.Radius + objectSphere.Radius - d );
						if( intensity != 0 )
						{
							if( probe1 == null )
							{
								probe1 = probe;
								probe1Intensity = intensity;
							}
							else
							{
								probe2 = probe;
								probe2Intensity = intensity;
								break;
							}
						}
					}
				}

				if( probe1 != null && probe2 != null )
				{
					if( probe1.data.CubemapEnvironment != null )
						texture1 = new EnvironmentTextureData( probe1.data.CubemapEnvironment, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
					if( probe1.data.HarmonicsIrradiance != null )
						harmonics1 = new EnvironmentIrradianceData( probe1.data.HarmonicsIrradiance, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
					if( probe2.data.CubemapEnvironment != null )
						texture2 = new EnvironmentTextureData( probe2.data.CubemapEnvironment, 1, ref probe2.data.Rotation, ref probe2.data.Multiplier );
					if( probe2.data.HarmonicsIrradiance != null )
						harmonics2 = new EnvironmentIrradianceData( probe2.data.HarmonicsIrradiance, 1, ref probe2.data.Rotation, ref probe2.data.Multiplier );
					blendingFactor = (float)( probe1Intensity / ( probe1Intensity + probe2Intensity ) );
				}
				else if( probe1 != null )
				{
					if( probe1.data.CubemapEnvironment != null )
						texture1 = new EnvironmentTextureData( probe1.data.CubemapEnvironment, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
					if( probe1.data.HarmonicsIrradiance != null )
						harmonics1 = new EnvironmentIrradianceData( probe1.data.HarmonicsIrradiance, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
					blendingFactor = (float)probe1Intensity;
				}
			}

			//init nulls by sky texture or white cube
			GetBackgroundEnvironmentData( context, frameData, out var backgroundTexture, out var backgroundTextureIBL );
			if( texture1 == null )
				texture1 = backgroundTexture;
			if( texture2 == null )
				texture2 = backgroundTexture;
			if( harmonics1 == null )
				harmonics1 = backgroundTextureIBL;
			if( harmonics2 == null )
				harmonics2 = backgroundTextureIBL;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//Fog

		/// <summary>
		/// Represents default fog data for a shader.
		/// </summary>
		[StructLayout( LayoutKind.Sequential )]
		public struct FogDefaultUniform
		{
			public ColorValue color;

			public float distanceMode;//0 - disabled
			public float startDistance;
			public float density;
			public float heightMode;//0,1

			public float height;
			public float heightScale;
			public Vector2F unused;
		}

		public delegate void FogExtensionGetUniformDataDelegate( ViewportRenderingContext context, Fog fog, IntPtr data, int dataSizeInBytes );

		static bool fogExtensionEnabled;
		static int fogExtensionShaderUniformVec4Count;
		static string fogExtensionShaderCode;
		static FogExtensionGetUniformDataDelegate fogExtensionGetUniformData;

		/// <summary>
		/// Need call before engine initialization from AssemblyRegistration class.
		/// </summary>
		/// <param name="shaderUniformVec4Count"></param>
		/// <param name="shaderCode"></param>
		/// <param name="getUniformData"></param>
		public static void SetFogExtension(/* bool enable, */int shaderUniformVec4Count, string shaderCode, FogExtensionGetUniformDataDelegate getUniformData )
		{
			//!!!!дефайном shaderUniformVec4Count, shaderCode
			Log.Fatal( "RenderingPipeline_Basic: SetFogExtension: impl." );

			unsafe
			{
				if( shaderUniformVec4Count < sizeof( FogDefaultUniform ) / sizeof( Vector4F ) )
					Log.Fatal( "RenderingPipeline_Basic: SetFogExtension: shaderUniformVec4Count < sizeof( FogDefaultUniform ) / sizeof( Vec4F )." );
			}

			fogExtensionEnabled = true;// enable;
			fogExtensionShaderUniformVec4Count = shaderUniformVec4Count;
			fogExtensionShaderCode = shaderCode;
			fogExtensionGetUniformData = getUniformData;
		}

		unsafe void SetFogUniform( ViewportRenderingContext context, Fog fog )
		{
			if( !RenderingSystem.Fog )
				return;

			int size;
			if( fogExtensionEnabled )
				size = fogExtensionShaderUniformVec4Count * sizeof( Vector4F );
			else
				size = sizeof( FogDefaultUniform );

			IntPtr uniformData = NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, size );
			NativeUtility.ZeroMemory( uniformData, size );

			if( fog != null && fog.EnabledInHierarchy )
			{
				var data = (FogDefaultUniform*)uniformData;

				var mode = fog.Mode.Value;
				data->color = fog.Color;
				if( mode.HasFlag( Fog.Modes.Exp ) )
					data->distanceMode = 1;
				if( mode.HasFlag( Fog.Modes.Exp2 ) )
					data->distanceMode = 2;
				data->startDistance = (float)fog.StartDistance;
				data->density = (float)fog.Density;
				if( mode.HasFlag( Fog.Modes.Height ) )
					data->heightMode = 1;
				data->height = (float)fog.Height;
				data->heightScale = (float)fog.HeightScale;
			}
			if( fogExtensionEnabled && fogExtensionGetUniformData != null )
				fogExtensionGetUniformData( context, fog, uniformData, size );

			context.SetUniform( "u_fogSettings", ParameterType.Vector4, size / sizeof( Vector4F ), uniformData );

			NativeUtility.Free( uniformData );
		}

		void UpdateNullShadowTextures()
		{
			if( nullShadowTexture2D == null || nullShadowTexture2D.Disposed )
				nullShadowTexture2D = null;
			if( nullShadowTextureCube == null || nullShadowTextureCube.Disposed )
				nullShadowTextureCube = null;

			if( nullShadowTexture2D == null )
			{
				var texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
				texture.CreateType = ImageComponent.TypeEnum._2D;
				texture.CreateSize = new Vector2I( 1, 1 );
				texture.CreateMipmaps = false;
				texture.CreateFormat = RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4 ? PixelFormat.A8R8G8B8 : PixelFormat.Float32R;
				texture.CreateUsage = ImageComponent.Usages.WriteOnly;
				texture.Enabled = true;

				if( texture.Result != null )
				{
					var data = new byte[ 4 ];
					var d = new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( data ) };
					texture.Result.SetData( d );
				}

				nullShadowTexture2D = texture;
			}

			if( nullShadowTextureCube == null )
			{
				var texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
				texture.CreateType = ImageComponent.TypeEnum.Cube;
				texture.CreateSize = new Vector2I( 1, 1 );
				texture.CreateMipmaps = false;
				texture.CreateFormat = RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4 ? PixelFormat.A8R8G8B8 : PixelFormat.Float32R;
				texture.CreateUsage = ImageComponent.Usages.WriteOnly;
				texture.Enabled = true;

				if( texture.Result != null )
				{
					var data = new byte[ 4 ];
					var d = new GpuTexture.SurfaceData[ 6 ];
					for( int n = 0; n < 6; n++ )
						d[ n ] = new GpuTexture.SurfaceData( data );
					texture.Result.SetData( d );
				}

				nullShadowTextureCube = texture;
			}
		}

		public unsafe void BindRenderOperationData( ViewportRenderingContext context, FrameData frameData, Material.CompiledMaterialData materialData, bool instancing, RenderSceneData.MeshItem.AnimationDataClass meshItemAnimationData, int billboardMode, double billboardRadius, bool receiveDecals, ref Vector3F previousWorldPosition, float lodValue, UnwrappedUVEnum unwrappedUV, ref ColorValue color, bool vertexStructureContainsColor, bool isLayer, float visibilityDistance, float motionBlurFactor, bool layerMaskFormatTriangles, MeshGeometry.BillboardDataModeEnum billboardDataMode, ImageComponent billboardDataImage, int meshGeometryIndex )//, bool bindMaterialsTexture )
		{
			//!!!!can merge with bit flags
			Vector4F data0 = Vector4F.Zero;
			Vector4F data1 = Vector4F.Zero;
			Vector4F data2 = Vector4F.Zero;
			Vector4F data3 = Vector4F.Zero;
			//Vector4F data4;

			if( materialData != null )
				data0.X = materialData.currentFrameIndex;
			if( instancing )
				data0.Y = -1;
			if( meshItemAnimationData != null )
				data0.Y = meshItemAnimationData.Mode;
			if( billboardMode != 0 )
			{
				data0.Z = billboardMode;
				data0.W = (float)billboardRadius;
			}

			if( receiveDecals )
				data1.X = 1;
			data1.Y = visibilityDistance;
			data1.Z = motionBlurFactor;
			data1.W = (int)billboardDataMode;

			data2 = new Vector4F( previousWorldPosition, lodValue );

			data3.X = (int)unwrappedUV;
			if( vertexStructureContainsColor )
				data3.Y = 1;
			if( isLayer )
				data3.Z = layerMaskFormatTriangles ? 2 : 1;
			//if( isLayer )
			//	data3.Z = 1;
			data3.W = meshGeometryIndex;

			//data4 = color.ToVector4F();

			//set u_renderOperationData
			if( !Vector4F.Equals( ref renderOperationDataCurrent.data0, ref data0 ) ||
				!Vector4F.Equals( ref renderOperationDataCurrent.data1, ref data1 ) ||
				!Vector4F.Equals( ref renderOperationDataCurrent.data2, ref data2 ) ||
				!Vector4F.Equals( ref renderOperationDataCurrent.data3, ref data3 ) ||
				!ColorValue.Equals( ref renderOperationDataCurrent.data4, ref color ) )
			{
				renderOperationDataCurrent.data0 = data0;
				renderOperationDataCurrent.data1 = data1;
				renderOperationDataCurrent.data2 = data2;
				renderOperationDataCurrent.data3 = data3;
				renderOperationDataCurrent.data4 = color;

				if( u_renderOperationData == null )
					u_renderOperationData = GpuProgramManager.RegisterUniform( "u_renderOperationData", UniformType.Vector4, 5 );
				fixed( RenderOperationDataStructure* pData = &renderOperationDataCurrent )
					Bgfx.SetUniform( u_renderOperationData.Value, pData, 5 );
			}

			//bind bones texture
			var bonesTexture = meshItemAnimationData?.BonesTexture;
			if( bonesTexture != null )
			{
				context.BindTexture( 0/*"s_bones"*/, bonesTexture, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None );
			}

			////bind materials data texture
			//if( bindMaterialsTexture && materialData != null )
			//{
			//	if( materialData.currentFrameIndex == -1 )
			//		Log.Fatal( "RenderingPipeline_Basic: BindRenderOperationData: materialData.currentFrameIndex == -1." );

			//	BindMaterialsTexture( context, frameData );
			//	//context.BindTexture( 1/*"s_materials"*/, frameData.MaterialsTexture, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None );
			//}

			//bind billboard data. billboard with geometry data mode
			if( billboardDataImage != null )
			{
				context.BindTexture( 2/*"s_billboardData"*/, billboardDataImage, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None );
			}
		}

		[Browsable( false )]
		public static ImageComponent BrdfLUT
		{
			get
			{
				if( brdfLUT == null )
					brdfLUT = ResourceManager.LoadResource<ImageComponent>( @"Base\Images\brdfLUT.dds" );
				return brdfLUT;
			}
		}

		unsafe void PrepareMaterialsTexture( ViewportRenderingContext context, FrameData frameData )
		{
			//add default materials
			frameData.AddMaterial( context, ResourceUtility.MaterialNull.Result );
			frameData.AddMaterial( context, ResourceUtility.MaterialInvalid.Result );

			//prepare texture

			int horizontalCount = 64;

			Vector2I size = new Vector2I( horizontalCount * 8, Math.Max( 32, MathEx.NextPowerOfTwo( frameData.Materials.Count / horizontalCount + 1 ) ) );
			if( size.Y > RenderingSystem.Capabilities.MaxTextureSize )
				size.Y = RenderingSystem.Capabilities.MaxTextureSize;
			int totalCount = horizontalCount * size.Y;


			int tempBufferSize = size.X * size.Y * 8;//16
			if( prepareMaterialsTempBuffer == null || prepareMaterialsTempBuffer.Length != tempBufferSize )
				prepareMaterialsTempBuffer = new byte[ tempBufferSize ];
			var data = prepareMaterialsTempBuffer;

			fixed( byte* pData = data )
			{
				for( int n = 0; n < frameData.Materials.Count; n++ )
				{
					if( n >= totalCount )
						break;

					var materialData = frameData.Materials.Data[ n ];

					materialData.UpdateDynamicParametersFragmentUniformData( context );

					var pData2 = pData + n * sizeof( Material.CompiledMaterialData.DynamicParametersFragmentUniform );
					var pData3 = (Material.CompiledMaterialData.DynamicParametersFragmentUniform*)pData2;
					*pData3 = materialData.dynamicParametersFragmentUniformData;
				}
			}

			frameData.MaterialsTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, size, PixelFormat.Float16RGBA, 0, false );//Float32RGBA
			frameData.MaterialsTexture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( data ) } );
		}

		static bool? isIntelGPU;
		bool GetDeferredShading()
		{
			if( !GetUseMultiRenderTargets() )
				return false;

			//deferred shading is not supported on limited devices
			if( SystemSettings.LimitedDevice )
				return false;

			var result = DeferredShading.Value;

			if( result == AutoTrueFalse.Auto )
			{
				//disable deferred on Intel by default

				if( isIntelGPU == null )
					isIntelGPU = Bgfx.GetGPUDescription().Contains( "Intel" );

				if( isIntelGPU.Value )
					result = AutoTrueFalse.False;
				else
					result = AutoTrueFalse.True;
			}

			return result == AutoTrueFalse.True;
		}

		void RenderOutlineEffectForSelectedObjects( ViewportRenderingContext context, FrameData frameData, ref ImageComponent actualTexture )
		{
			var outline = new RenderingEffect_Outline();
			outline.GroupsInterval = new RangeI( int.MaxValue, int.MaxValue );
			outline.Scale = ProjectSettings.Get.SceneEditor.SceneEditorSelectOutlineEffectScale;

			outline.Render( context, frameData, ref actualTexture );
		}

		bool IsProvideColorDepthTextureCopy()
		{
			if( UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
			{
				var value = ProvideColorDepthTextureCopy.Value;
				if( value == AutoTrueFalse.Auto )
					value = SystemSettings.LimitedDevice ? AutoTrueFalse.False : AutoTrueFalse.True;
				return value == AutoTrueFalse.True;
			}
			else
				return false;
		}

		//bool IsAllowSoftParticles()
		//{
		//	if( UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//	{
		//		var value = SoftParticles.Value;
		//		if( value == AutoTrueFalse.Auto )
		//			value = SystemSettings.LimitedDevice ? AutoTrueFalse.False : AutoTrueFalse.True;
		//		return value == AutoTrueFalse.True;
		//	}
		//	else
		//		return false;
		//}

		public void BindMaterialsTexture( ViewportRenderingContext context, FrameData frameData )
		{
			context.BindTexture( 1/*"s_materials"*/, frameData.MaterialsTexture, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None );
		}

		public void BindBrdfLUT( ViewportRenderingContext context )
		{
			context.BindTexture( 6/*"s_brdfLUT"*/, BrdfLUT, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
		}


		//void RenderVideoToFile( ViewportRenderingContext context, ImageComponent sceneTexture )
		//{
		//	может сначала рендерить в RenderTexture, далее отдавать из неё

		//	if( EngineApp.RenderVideoToFileData != null )
		//	{
		//		ImageComponent texture = null;
		//		ImageComponent textureRead = null;

		//		try
		//		{
		//			var format = PixelFormat.A8R8G8B8;
		//			var imageSize = sceneTexture.Result.ResultSize;

		//			texture = context.RenderTarget2D_Alloc( imageSize, format, 0 );
		//			var textureViewport = texture.Result.GetRenderTarget().Viewports[ 0 ];
		//			context.SetViewport( textureViewport, Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.All, new ColorValue( 0, 0, 0 ) );
		//			CopyToCurrentViewport( context, sceneTexture );

		//			textureRead = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
		//			textureRead.CreateType = ImageComponent.TypeEnum._2D;
		//			textureRead.CreateSize = imageSize;
		//			textureRead.CreateMipmaps = false;
		//			textureRead.CreateFormat = format;
		//			textureRead.CreateUsage = ImageComponent.Usages.ReadBack | ImageComponent.Usages.BlitDestination;
		//			textureRead.CreateFSAA = 0;
		//			textureRead.Enabled = true;

		//			//viewport.Update( true, cameraSettings );

		//			//!!!!
		//			texture.Result.GetRealObject( true ).BlitTo( context.CurrentViewNumber, textureRead.Result.GetRealObject( true ), 0, 0 );
		//			//texture.Result.GetRealObject( true ).BlitTo( textureViewport.RenderingContext.CurrentViewNumber, textureRead.Result.GetRealObject( true ), 0, 0 );

		//			//get data
		//			var totalBytes = PixelFormatUtility.GetNumElemBytes( format ) * imageSize.X * imageSize.Y;
		//			var data = new byte[ totalBytes ];
		//			unsafe
		//			{
		//				fixed( byte* pBytes = data )
		//				{
		//					var demandedFrame = textureRead.Result.GetRealObject( true ).Read( (IntPtr)pBytes, 0 );
		//					while( RenderingSystem.CallBgfxFrame() < demandedFrame ) { }
		//				}
		//			}

		//			var image = new ImageUtility.Image2D( format, imageSize, data );

		//			//!!!!

		//			//make bitmap from clamped image
		//			System.Drawing.Bitmap bitmap2;
		//			unsafe
		//			{
		//				fixed( byte* pImage2 = image.Data )
		//				{
		//					bitmap2 = new System.Drawing.Bitmap( image.Size.X, image.Size.Y, image.Size.X * PixelFormatUtility.GetNumElemBytes( format ), System.Drawing.Imaging.PixelFormat.Format32bppArgb, (IntPtr)pImage2 );
		//				}
		//			}

		//			if( System.IO.File.Exists( @"C:\______________sdjflsjdflsdf\Text.png" ) )
		//				System.IO.File.Delete( @"C:\______________sdjflsjdflsdf\Text.png" );
		//			bitmap2.Save( @"C:\______________sdjflsjdflsdf\Text.png" );


		//		}
		//		finally
		//		{
		//			context.DynamicTexture_Free( texture );
		//			//!!!!
		//			//!!!!texture?.Dispose();

		//			textureRead?.Dispose();
		//		}
		//	}
		//}

		unsafe void OcclusionCullingBuffer_RenderOccluders( ViewportRenderingContext context, /*FrameData frameData, */Viewport.CameraSettingsClass cameraSettings, OcclusionCullingBuffer buffer, OpenList<OccluderItem> occluders, bool sceneOccluder )
		{
			//var cameraSettings = context.Owner.CameraSettings;
			var cameraPosition = cameraSettings.Position;

			//sort occluders by distance. it changes items of the input list

			for( int nOccluder = 0; nOccluder < occluders.Count; nOccluder++ )
			{
				ref var occluderItem = ref occluders.Data[ nOccluder ];
				occluderItem.tempDistanceSquared = ( occluderItem.Center - cameraPosition ).LengthSquared();
			}

			var occluderIndices = (int*)NativeUtility.Alloc( NativeUtility.MemoryAllocationType.Renderer, occluders.Count * 4 );
			for( int nOccluder = 0; nOccluder < occluders.Count; nOccluder++ )
				occluderIndices[ nOccluder ] = nOccluder;

			//!!!!threading
			CollectionUtility.MergeSort( occluderIndices, occluders.Count, delegate ( int index1, int index2 )
			{
				ref var item1 = ref occluders.Data[ index1 ];
				ref var item2 = ref occluders.Data[ index2 ];
				if( item1.tempDistanceSquared < item2.tempDistanceSquared )
					return -1;
				if( item1.tempDistanceSquared > item2.tempDistanceSquared )
					return 1;
				return 0;
			} );//, true );


			//render to the buffer

			var viewProjectionMatrix = cameraSettings.GetViewProjectionMatrix();
			//!!!!double
			viewProjectionMatrix.ToMatrix4F( out var viewProjMatrixFloat );

			buffer.ClearBuffer();

			for( int nOccluder = 0; nOccluder < occluders.Count; nOccluder++ )
			{
				ref var occluderItem = ref occluders.Data[ occluderIndices[ nOccluder ] ];

				var vertices = occluderItem.Vertices;
				var indices = occluderItem.Indices;

				//!!!!double
				var verticesFloat = stackalloc Vector3F[ vertices.Length ];
				for( int n = 0; n < vertices.Length; n++ )
					verticesFloat[ n ] = vertices[ n ].ToVector3F();

				fixed( int* pIndices = indices )
					buffer.RenderTriangles( (float*)verticesFloat, (uint*)pIndices, indices.Length / 3, (float*)&viewProjMatrixFloat );
			}

			NativeUtility.Free( occluderIndices );


			//debug visualization
			if( sceneOccluder && DebugMode.Value == DebugModeEnum.OcclusionCullingBuffer )
			{
				var pixelData = new byte[ buffer.Size.X * buffer.Size.Y * 4 ];
				fixed( byte* pPixelData = pixelData )
				{
					float* pPixelData2 = (float*)pPixelData;

					buffer.ComputePixelDepthBuffer( pPixelData2 );

					for( int n = 0; n < buffer.Size.X * buffer.Size.Y; n++ )
						pPixelData2[ n ] = MathEx.Sqrt( pPixelData2[ n ] );
				}

				//!!!!mobile PixelFormat.Float32R
				var texture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, buffer.Size, PixelFormat.Float32R, 0, false );
				texture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( pixelData ) } );

				context.Owner.CanvasRenderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new RectangleF( 0, 0, 1, 1 ), texture );
			}
		}

	}
}
