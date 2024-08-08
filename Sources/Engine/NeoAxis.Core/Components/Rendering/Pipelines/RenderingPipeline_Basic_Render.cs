// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Linq;
using Internal.SharpBgfx;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	public partial class RenderingPipeline_Basic
	{
		// Don't add many non static fields. Rendering pipeline is created for each temporary render target during frame rendering.

		static Uniform? u_environmentLightParams;
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

		static Uniform u_renderOperationData;
		//static RenderOperationDataStructure renderOperationDataCurrent;

		static Uniform? u_viewportCutVolumeSettings;
		static Uniform? u_viewportCutVolumeData;

		static Uniform? u_prepareShadowsSettings;

		static Uniform u_objectInstanceParameters;
		static Vector4F objectInstanceParametersLast1;
		static Vector4F objectInstanceParametersLast2;
		static bool objectInstanceParametersIsNull;

		static ImageComponent brdfLUT;

		static ShadowCasterData defaultShadowCasterData;
		static DeferredShadingData deferredShadingData;
		static Mesh decalMesh;

		static List<OutputInstancingManager> outputInstancingManagers = new List<OutputInstancingManager>();

		static byte[] prepareMaterialsBonesLightsTempBuffer;

		static bool duringRenderShadows;
		static bool duringRenderSimple3DRenderer;

		internal static Vector3F vector3FZero;

		static bool[] pointLightShadowGenerationAddFaces = new bool[ 6 ];

		static Program? lightGridGenerateProgram;
		//static Program? blurCubemapProgram;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		////!!!!remove?
		//public delegate void InitLightDataBuffersEventDelegate( RenderingPipeline_Basic sender, ViewportRenderingContext context, LightItem lightItem );
		//public event InitLightDataBuffersEventDelegate InitLightDataBuffersEvent;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		class DeferredShadingData
		{
			public PassItem ambientPass;
			//public PassItem[] passesPerLightWithoutShadows = new PassItem[ 4 ];
			//public PassItem[] passesPerLightWithShadows = new PassItem[ 4 ];
			//public PassItem[] passesPerLightWithShadowsContactShadows = new PassItem[ 4 ];
			////public PassItem[] passesPerLightWithShadowsLow = new PassItem[ 4 ];
			////public PassItem[] passesPerLightWithShadowsHigh = new PassItem[ 4 ];

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
				ambientPass = null;
				//passesPerLightWithoutShadows = null;
				//passesPerLightWithShadows = null;
				//passesPerLightWithShadowsContactShadows = null;
				////passesPerLightWithShadowsLow = null;
				////passesPerLightWithShadowsHigh = null;

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
		public class FrameData// : IFrameData
		{
			public RenderSceneData RenderSceneData = new RenderSceneData();
			//public RenderSceneData RenderSceneData { get { return renderSceneData; } }

			//!!!!can use list container with several arrays inside

			public OpenList<ObjectInSpaceItem> ObjectInSpaces;

			public OpenList<MeshItem> Meshes;
			public OpenList<BillboardItem> Billboards;
			public List<LightItem> Lights;
			public List<ReflectionProbeItem> ReflectionProbes;
			public List<DecalItem> Decals;

			public int[] LightsInFrustumSorted = Array.Empty<int>();

			public List<Vector2I> RenderableGroupsInFrustum;
			public List<Vector2I> RenderableGroupsForGI;

			public Sky Sky;
			public Fog Fog;
			//public RenderingEffect_IndirectLighting.FrameData IndirectLightingFrameData;

			public OpenList<Material.CompiledMaterialData> Materials;
			public ImageComponent MaterialsTexture;

			public ImageComponent BonesTexture;

			public ImageComponent LightsTexture;
			public ImageComponent LightGrid;

			public ImageComponent ShadowTextureArrayDirectional;
			public ImageComponent ShadowTextureArraySpot;
			public ImageComponent ShadowTextureArrayPoint;
			//for mobile
			public int ShadowTextureArrayDirectionalUsedForShadows;
			public int ShadowTextureArraySpotUsedForShadows;
			public int ShadowTextureArrayPointUsedForShadows;

			public ImageComponent MaskTextureArrayDirectional;
			public ImageComponent MaskTextureArraySpot;
			public ImageComponent MaskTextureArrayPoint;

			public GIData GIData;

			//!!!!don't forget to Clear() new fields


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
				//public int PointSpotLightCount;
				//public unsafe fixed int PointSpotLightsFixed[ 6 ];
				//public List<int> PointSpotLightsMore;

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
					UseGI = 128,
					//CalculateAffectedLights = 128,
				}

				//

				//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				//public unsafe bool CanUseInstancingForTransparentWith( ref MeshItem meshItem )
				//{
				//	//!!!!need?
				//	if( Flags != meshItem.Flags )
				//		return false;

				//	//if( PointSpotLightCount != meshItem.PointSpotLightCount )
				//	//	return false;

				//	//for( int n = 0; n < Math.Min( PointSpotLightCount, 6 ); n++ )
				//	//	if( PointSpotLightsFixed[ n ] != meshItem.PointSpotLightsFixed[ n ] )
				//	//		return false;

				//	//if( PointSpotLightsMore != null )
				//	//{
				//	//	for( int n = 0; n < PointSpotLightsMore.Count; n++ )
				//	//		if( PointSpotLightsMore[ n ] != meshItem.PointSpotLightsMore[ n ] )
				//	//			return false;
				//	//}

				//	return true;
				//}
			}

			////////////

			/// <summary>
			/// Represents billboard data of <see cref="FrameData"/>.
			/// </summary>
			public struct BillboardItem
			{
				public FlagsEnum Flags;
				public float DistanceToCameraSquared;
				//public int PointSpotLightCount;
				//public unsafe fixed int PointSpotLightsFixed[ 6 ];
				//public List<int> PointSpotLightsMore;

				//

				[Flags]
				public enum FlagsEnum
				{
					//InsideFrustum = 1,
					UseDeferred = 2,
					UseForwardOpaque = 4,
					UseForwardTransparent = 8,
					UseGI = 16,
					//CalculateAffectedLights = 16,
				}

				//

				//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				//public unsafe void AddPointSpotLight( int lightIndex )
				//{
				//	if( PointSpotLightCount < 6 )
				//	{
				//		fixed( int* p = PointSpotLightsFixed )
				//			p[ PointSpotLightCount ] = lightIndex;
				//	}
				//	else if( PointSpotLightCount == 6 )
				//	{
				//		PointSpotLightsMore = new List<int>();
				//		PointSpotLightsMore.Add( lightIndex );
				//	}
				//	else
				//		PointSpotLightsMore.Add( lightIndex );
				//	PointSpotLightCount++;
				//}

				//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				//public unsafe int GetPointSpotLight( int n )
				//{
				//	if( n < 6 )
				//	{
				//		fixed( int* p = PointSpotLightsFixed )
				//			return p[ n ];
				//	}
				//	else
				//		return PointSpotLightsMore[ n - 6 ];
				//}

				//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				//public bool ContainsPointOrSpotLight( int lightIndex )
				//{
				//	for( int n = 0; n < PointSpotLightCount; n++ )
				//		if( GetPointSpotLight( n ) == lightIndex )
				//			return true;
				//	return false;
				//}

				//[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
				//public unsafe bool CanUseInstancingForTransparentWith( ref BillboardItem billboardItem )
				//{
				//	//!!!!need?
				//	if( Flags != billboardItem.Flags )
				//		return false;

				//	//if( PointSpotLightCount != billboardItem.PointSpotLightCount )
				//	//	return false;

				//	//for( int n = 0; n < Math.Min( PointSpotLightCount, 6 ); n++ )
				//	//	if( PointSpotLightsFixed[ n ] != billboardItem.PointSpotLightsFixed[ n ] )
				//	//		return false;

				//	//if( PointSpotLightsMore != null )
				//	//{
				//	//	for( int n = 0; n < PointSpotLightsMore.Count; n++ )
				//	//		if( PointSpotLightsMore[ n ] != billboardItem.PointSpotLightsMore[ n ] )
				//	//			return false;
				//	//}

				//	return true;
				//}
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
				RenderableGroupsForGI = new List<Vector2I>( 512 * multiplier );

				Materials = new OpenList<Material.CompiledMaterialData>( 512 * multiplier );
			}

			[MethodImpl( (MethodImplOptions)512 )]
			public void Clear()
			{
				RenderSceneData.Clear();
				ObjectInSpaces.Clear();
				Meshes.Clear();
				Billboards.Clear();
				Lights.Clear();
				ReflectionProbes.Clear();
				Decals.Clear();
				LightsInFrustumSorted = Array.Empty<int>();
				RenderableGroupsInFrustum.Clear();
				RenderableGroupsForGI.Clear();
				Sky = null;
				Fog = null;
				//IndirectLightingFrameData = null;
				Materials.Clear();
				MaterialsTexture = null;
				BonesTexture = null;
				LightsTexture = null;
				LightGrid = null;
				ShadowTextureArrayDirectional = null;
				ShadowTextureArraySpot = null;
				ShadowTextureArrayPoint = null;
				ShadowTextureArrayDirectionalUsedForShadows = 0;
				ShadowTextureArraySpotUsedForShadows = 0;
				ShadowTextureArrayPointUsedForShadows = 0;
				MaskTextureArrayDirectional = null;
				MaskTextureArraySpot = null;
				MaskTextureArrayPoint = null;
				GIData = null;
			}

			[MethodImpl( (MethodImplOptions)512 )]
			void AddItems( ViewportRenderingContext context, int meshes, int meshes2, int billboards, int billboards2, int lights, int lights2, int reflectionProbes, int reflectionProbes2, int decals, int decals2, bool addMaterialsAddOnlySpecialShadowCasters )
			{
				for( int n = meshes; n < meshes2; n++ )
				{
					ref var meshItem = ref RenderSceneData.Meshes.Data[ n ];
					var meshData = meshItem.MeshData;
					var meshDataShadows = meshItem.MeshDataShadows;

					var data = new MeshItem();

					//calculate DistanceToCameraSquared
					{
						var lengthSqr = ( context.OwnerCameraSettingsPosition - meshItem.BoundingSphere.Center ).LengthSquared();
						if( lengthSqr < meshItem.BoundingSphere.Radius * meshItem.BoundingSphere.Radius )
							data.DistanceToCameraSquared = 0;
						else
							data.DistanceToCameraSquared = (float)lengthSqr;

						//if( meshItem.BoundingSphere.Contains( ref context.OwnerCameraSettingsPosition ) )
						//	data.DistanceToCameraSquared = 0;
						//else
						//	data.DistanceToCameraSquared = (float)( context.OwnerCameraSettingsPosition - meshItem.BoundingSphere.Center ).LengthSquared();

						//data.DistanceToCameraSquared = (float)( meshItem.BoundingBoxCenter - context.OwnerCameraSettingsPosition ).LengthSquared();
						////meshItem.BoundingBoxCenter.GetCenter( out var center );
					}

					//Flags
					//bool lit = false;

					//add materials of mesh item
					for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
					{
						var oper = meshData.RenderOperations[ nOperation ];
						if( oper.ContainsDisposedBuffers() )
							continue;

						foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, false, context.DeferredShading ) )
						{
							if( materialData.deferredShadingSupport )
							{
								var pipeline = context.RenderingPipeline;
								if( context.DeferredShading && pipeline.UseRenderTargets && pipeline.DebugMode.Value == DebugModeEnum.None )
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

							if( materialData.giSupport )
								data.Flags |= MeshItem.FlagsEnum.UseGI;

							//if( materialData.ShadingModel != Material.ShadingModelEnum.Unlit )
							//	lit = true;

							//add material
							if( !addMaterialsAddOnlySpecialShadowCasters || ( addMaterialsAddOnlySpecialShadowCasters && materialData.specialShadowCasterData != null ) )
								AddMaterial( context, materialData );
						}
					}

					//!!!!always need to add?
					//add materials of mesh item for shadows
					if( meshDataShadows != null )
					{
						for( int nOperation = 0; nOperation < meshDataShadows.RenderOperations.Count; nOperation++ )
						{
							var oper = meshDataShadows.RenderOperations[ nOperation ];
							if( oper.ContainsDisposedBuffers() )
								continue;

							foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, false, context.DeferredShading ) )
							{
								//add material
								if( !addMaterialsAddOnlySpecialShadowCasters || ( addMaterialsAddOnlySpecialShadowCasters && materialData.specialShadowCasterData != null ) )
									AddMaterial( context, materialData );
							}
						}
					}

					//add materials of the layers
					if( meshItem.Layers != null )
					{
						var meshFlags = data.Flags;

						for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
						{
							ref var layer = ref meshItem.Layers[ nLayer ];

							foreach( var materialData in GetLayerMaterialData( ref layer, false, context.DeferredShading ) ) //layer.ResultMaterial;//?.Result;
							{
								//if( materialData == null )
								//	continue;
								////if( materialData == null )
								////	materialData = ResourceUtility.MaterialNull.Result;

								//if( materialData != null )
								//{
								if( materialData.deferredShadingSupport )
								{
									var pipeline = context.RenderingPipeline;
									if( context.DeferredShading && pipeline.UseRenderTargets && pipeline.DebugMode.Value == DebugModeEnum.None )
										data.Flags |= MeshItem.FlagsEnum.UseDeferred;
									else
									{
										if( ( ( meshFlags & MeshItem.FlagsEnum.UseDeferred ) != 0 ) || ( ( meshFlags & MeshItem.FlagsEnum.UseForwardOpaque ) != 0 ) )
											data.Flags |= MeshItem.FlagsEnum.UseForwardOpaque | MeshItem.FlagsEnum.ContainsForwardOpaqueLayersOnOpaqueBaseObjects;
									}
								}
								else
								{
									if( materialData.Transparent )
									{
										if( ( meshFlags & MeshItem.FlagsEnum.UseForwardTransparent ) != 0 )
											data.Flags |= MeshItem.FlagsEnum.ContainsTransparentLayersOnTransparentBaseObjects;
										else
											data.Flags |= MeshItem.FlagsEnum.ContainsTransparentLayersOnOpaqueBaseObjects;
									}
									else
									{
										if( ( ( meshFlags & MeshItem.FlagsEnum.UseDeferred ) != 0 ) || ( ( meshFlags & MeshItem.FlagsEnum.UseForwardOpaque ) != 0 ) )
											data.Flags |= MeshItem.FlagsEnum.UseForwardOpaque | MeshItem.FlagsEnum.ContainsForwardOpaqueLayersOnOpaqueBaseObjects;
									}
								}

								if( materialData.giSupport )
									data.Flags |= MeshItem.FlagsEnum.UseGI;

								//if( materialData.ShadingModel != Material.ShadingModelEnum.Unlit )
								//	lit = true;

								//add material
								if( !addMaterialsAddOnlySpecialShadowCasters || ( addMaterialsAddOnlySpecialShadowCasters && materialData.specialShadowCasterData != null ) )
									AddMaterial( context, materialData );
								//}
							}
						}
					}

					////CalculateAffectedLights
					//if( lit && ( ( data.Flags & MeshItem.FlagsEnum.UseForwardOpaque ) != 0 || ( data.Flags & MeshItem.FlagsEnum.UseForwardTransparent ) != 0 ) )
					//	data.Flags |= MeshItem.FlagsEnum.CalculateAffectedLights;

					Meshes.Add( ref data );
				}

				for( int n = billboards; n < billboards2; n++ )
				{
					ref var data2 = ref RenderSceneData.Billboards.Data[ n ];
					//!!!!так?
					//data2.BoundingBox.GetCenter( out var center );

					var data = new BillboardItem();

					//calculate DistanceToCameraSquared
					{
						var lengthSqr = ( context.OwnerCameraSettingsPosition - data2.BoundingSphere.Center ).LengthSquared();
						if( lengthSqr < data2.BoundingSphere.Radius * data2.BoundingSphere.Radius )
							data.DistanceToCameraSquared = 0;
						else
							data.DistanceToCameraSquared = (float)lengthSqr;

						//data.DistanceToCameraSquared = (float)( data2.BoundingBoxCenter - context.OwnerCameraSettingsPosition ).LengthSquared();
					}

					////Flags
					//bool lit = false;

					foreach( var materialData in GetBillboardMaterialData( ref data2, false, context.DeferredShading ) )
					{
						if( materialData.deferredShadingSupport )
						{
							var pipeline = context.RenderingPipeline;
							if( context.DeferredShading && pipeline.UseRenderTargets && pipeline.DebugMode.Value == DebugModeEnum.None )
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

						if( materialData.giSupport )
							data.Flags |= BillboardItem.FlagsEnum.UseGI;

						//if( materialData.ShadingModel != Material.ShadingModelEnum.Unlit )
						//	lit = true;

						////CalculateAffectedLights
						//if( lit && ( ( data.Flags & BillboardItem.FlagsEnum.UseForwardOpaque ) != 0 || ( data.Flags & BillboardItem.FlagsEnum.UseForwardTransparent ) != 0 ) )
						//	data.Flags |= BillboardItem.FlagsEnum.CalculateAffectedLights;

						Billboards.Add( ref data );

						//add material
						if( !addMaterialsAddOnlySpecialShadowCasters || ( addMaterialsAddOnlySpecialShadowCasters && materialData.specialShadowCasterData != null ) )
							AddMaterial( context, materialData );
					}
				}

				for( int n = lights; n < lights2; n++ )
				{
					ref var data2 = ref RenderSceneData.Lights.Data[ n ];
					Lights.Add( new LightItem( data2, context ) );
				}

				for( int n = reflectionProbes; n < reflectionProbes2; n++ )
				{
					ref var data2 = ref RenderSceneData.ReflectionProbes.Data[ n ];
					ReflectionProbes.Add( new ReflectionProbeItem( data2, context ) );
				}

				for( int n = decals; n < decals2; n++ )
				{
					ref var data2 = ref RenderSceneData.Decals.Data[ n ];

					foreach( var materialData in GetDecalMaterialData( ref data2, false, context.DeferredShading ) )
					{
						Decals.Add( new DecalItem() );

						//add material
						if( !addMaterialsAddOnlySpecialShadowCasters || ( addMaterialsAddOnlySpecialShadowCasters && materialData.specialShadowCasterData != null ) )
							AddMaterial( context, materialData );
					}
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
			}

			//public void SceneGetRenderSceneDataAfterObjects( ViewportRenderingContext context, Scene scene )
			//{
			//	int meshes = RenderSceneData.Meshes.Count;
			//	int billboards = RenderSceneData.Billboards.Count;
			//	int lights = RenderSceneData.Lights.Count;
			//	int reflectionProbes = RenderSceneData.ReflectionProbes.Count;
			//	int decals = RenderSceneData.Decals.Count;

			//	ComponentsHidePublic.PerformGetRenderSceneDataAfterObjects( scene, context );

			//	int meshes2 = RenderSceneData.Meshes.Count;
			//	int billboards2 = RenderSceneData.Billboards.Count;
			//	int lights2 = RenderSceneData.Lights.Count;
			//	int reflectionProbes2 = RenderSceneData.ReflectionProbes.Count;
			//	int decals2 = RenderSceneData.Decals.Count;

			//	AddItems( context, meshes, meshes2, billboards, billboards2, lights, lights2, reflectionProbes, reflectionProbes2, decals, decals2, false );

			//	////use ObjectInSpaceItem because useful
			//	//var data = new ObjectInSpaceItem();
			//	////data.ObjectInSpace = objectInSpace;

			//	//if( meshes2 > meshes )
			//	//{
			//	//	data.MeshRange = new RangeI( meshes, meshes2 );
			//	//	data.ContainsData = true;
			//	//}
			//	//if( billboards2 > billboards )
			//	//{
			//	//	data.BillboardRange = new RangeI( billboards, billboards2 );
			//	//	data.ContainsData = true;
			//	//}

			//	//if( !data.ContainsData )
			//	//	data.ContainsData = lights2 > lights || reflectionProbes2 > reflectionProbes || decals2 > decals;

			//	//var insideFrustum = mode == GetRenderSceneDataMode.InsideFrustum;//insideFrustum;

			//	////ObjectInSpaces.Add( ref data );
			//	////ComponentsHidePublic.SetRenderSceneIndex( objectInSpace, ObjectInSpaces.Count - 1 );

			//	////RenderableGroupsInFrustum
			//	//if( data.ContainsData && data.InsideFrustum )
			//	//{
			//	//	for( int n = data.MeshRange.Minimum; n < data.MeshRange.Maximum; n++ )
			//	//		RenderableGroupsInFrustum.Add( new Vector2I( 0, n ) );
			//	//	for( int n = data.BillboardRange.Minimum; n < data.BillboardRange.Maximum; n++ )
			//	//		RenderableGroupsInFrustum.Add( new Vector2I( 1, n ) );
			//	//}

			//	////RenderableGroupsForGI
			//	//if( data.ContainsData && mode == GetRenderSceneDataMode.GlobalIllumination )
			//	//{
			//	//	for( int n = data.MeshRange.Minimum; n < data.MeshRange.Maximum; n++ )
			//	//		RenderableGroupsForGI.Add( new Vector2I( 0, n ) );
			//	//	for( int n = data.BillboardRange.Minimum; n < data.BillboardRange.Maximum; n++ )
			//	//		RenderableGroupsForGI.Add( new Vector2I( 1, n ) );
			//	//}
			//}

			public void AddZeroAmbientLight( ViewportRenderingContext context )
			{
				int lights = RenderSceneData.Lights.Count;

				var item = new RenderSceneData.LightItem();
				//item.Creator = this;
				item.Type = Light.TypeEnum.Ambient;
				context.FrameData.RenderSceneData.Lights.Add( ref item );

				int lights2 = RenderSceneData.Lights.Count;

				AddItems( context, 0, 0, 0, 0, lights, lights2, 0, 0, 0, 0, false );
			}

			[MethodImpl( (MethodImplOptions)512 )]
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

					//RenderableGroupsForGI
					if( data.ContainsData && mode == GetRenderSceneDataMode.GlobalIllumination )
					{
						for( int n = data.MeshRange.Minimum; n < data.MeshRange.Maximum; n++ )
							RenderableGroupsForGI.Add( new Vector2I( 0, n ) );
						for( int n = data.BillboardRange.Minimum; n < data.BillboardRange.Maximum; n++ )
							RenderableGroupsForGI.Add( new Vector2I( 1, n ) );
					}
				}

				return ComponentsHidePublic.GetRenderSceneIndex( objectInSpace );
				//objectInSpaceData = ObjectInSpaceData[ objectInSpace._InternalRenderSceneIndex ];
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public bool GetObjectGroupBoundingBoxCenter( ref Vector2I index, out Vector3 center )
			{
				switch( index.X )
				{
				case 0: center = RenderSceneData.Meshes.Data[ index.Y ].BoundingSphere.Center/*BoundingBoxCenter*/; return true;
				case 1: center = RenderSceneData.Billboards.Data[ index.Y ].BoundingBoxCenter; return true;
					//case 2: center = RenderSceneData.Lights[ index.Y ].
					//case 3: center = RenderSceneData.ReflectionProbes[ index.Y ].distanceToCamera;
				}
				center = Vector3.Zero;
				return false;
			}

			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			public float GetObjectGroupDistanceToPointSquared( ref Vector2I index, ref Vector3 point )
			{
				switch( index.X )
				{
				case 0:
					{
						ref var data = ref RenderSceneData.Meshes.Data[ index.Y ];

						var lengthSqr = ( point - data.BoundingSphere.Center ).LengthSquared();
						if( lengthSqr < data.BoundingSphere.Radius * data.BoundingSphere.Radius )
							return 0;
						else
							return (float)lengthSqr;

						//return (float)( data.BoundingBoxCenter - point ).LengthSquared();
						////data.BoundingBox.GetCenter( out var center );
					}
				case 1:
					{
						ref var data = ref RenderSceneData.Billboards.Data[ index.Y ];

						var lengthSqr = ( point - data.BoundingSphere.Center ).LengthSquared();
						if( lengthSqr < data.BoundingSphere.Radius * data.BoundingSphere.Radius )
							return 0;
						else
							return (float)lengthSqr;

						//return (float)( data.BoundingBoxCenter - point ).LengthSquared();
						////data.BoundingBox.GetCenter( out var center );
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

			[MethodImpl( (MethodImplOptions)512 )]
			public void AddMaterial( ViewportRenderingContext context, Material.CompiledMaterialData materialData )
			{
				if( materialData == null )
					return;

				//add original materials to Materials texture
				if( materialData.multiMaterialReferencedSeparateMaterialsOfCombinedGroup != null )
				{
					foreach( var materialData2 in materialData.multiMaterialReferencedSeparateMaterialsOfCombinedGroup )
					{
						if( materialData2.currentFrameIndex == -1 )
						{
							materialData2.currentFrameIndex = Materials.Count;
							//need do full preparing here too, because source material can be added separately for other objects
							materialData2.PrepareCurrentFrameData( context, false );
							if( materialData2.specialShadowCasterData != null )
								materialData2.PrepareCurrentFrameData( context, true );
							Materials.Add( materialData2 );
						}
					}
				}

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
			public int shadowMapIndex;
			public Viewport.CameraSettingsClass shadowCameraSettings;
			public int shadowSourceTextureSize;
			//public ImageComponent shadowTexture;
			public (Matrix4F, Matrix4F)[] shadowCascadesProjectionViewMatrices;

			public OpenListNative<Vector2I>[] shadowsAffectedRenderableGroups;
			//public List<Vector2I>[] shadowsAffectedRenderableGroups;

			public int lightMaskIndex = -1;

			////data for GPU
			////public LightDataVertex lightDataVertex;
			//public LightDataFragment lightDataFragment;
			//public bool lightDataBuffersInitialized;

			////uniforms
			////public static Uniform? u_lightDataVertex;
			//public static Uniform? u_lightDataFragment;

			/////////////////////////////////////

			/////// <summary>
			/////// Represents data of light for a vertex shader.
			/////// </summary>
			////[StructLayout( LayoutKind.Sequential )]
			////public struct LightDataVertex
			////{
			////	public Vector4F lightPosition;

			////	////!!!!теперь не надо. делается в пиксельном шейдере
			////	////!!!!если нет теней, то не надо
			////	//public Vec4F lightShadowTextureViewProjMatrix0_0;
			////	//public Vec4F lightShadowTextureViewProjMatrix0_1;
			////	//public Vec4F lightShadowTextureViewProjMatrix0_2;
			////	//public Vec4F lightShadowTextureViewProjMatrix0_3;
			////}

			/////////////////////////////////////

			///// <summary>
			///// Represents data of light for a fragment shader.
			///// </summary>
			//[StructLayout( LayoutKind.Sequential )]
			//public struct LightDataFragment
			//{
			//	//general parameters
			//	public Vector4F lightPosition;
			//	public Vector3F lightDirection;
			//	public float startDistance;
			//	public Vector3F lightPower;
			//	public float unused;// public float lightShadowCascadeOverlapping;
			//	public Vector4F lightAttenuation;
			//	public Vector4F lightSpot;

			//	public float lightShadowIntensity;
			//	public float lightShadowTextureSize;
			//	public float lightShadowMapFarClipDistance;
			//	public float lightShadowCascadesVisualize;
			//	public Matrix4F lightShadowTextureViewProjMatrix0;
			//	public Matrix4F lightShadowTextureViewProjMatrix1;
			//	public Matrix4F lightShadowTextureViewProjMatrix2;
			//	public Matrix4F lightShadowTextureViewProjMatrix3;
			//	public Vector4F lightShadowCascades;

			//	public Matrix4F lightMaskMatrix;

			//	public Vector4F unitDistanceWorldShadowTexelSizes;

			//	public float lightShadowBias;
			//	public float lightShadowNormalBias;
			//	public float lightShadowSoftness;//public float lightSourceRadiusOrAngle;
			//	public float lightShadowContactLength;

			//	//

			//	[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			//	public void SetLightShadowTextureViewProjMatrix( int index, ref Matrix4F value )
			//	{
			//		switch( index )
			//		{
			//		case 0: lightShadowTextureViewProjMatrix0 = value; break;
			//		case 1: lightShadowTextureViewProjMatrix1 = value; break;
			//		case 2: lightShadowTextureViewProjMatrix2 = value; break;
			//		case 3: lightShadowTextureViewProjMatrix3 = value; break;
			//		}
			//	}
			//}

			/////////////////////////////////////

			/// <summary>
			/// Represents data of light for a fragment shader.
			/// </summary>
			[StructLayout( LayoutKind.Sequential )]
			public struct LightDataFragmentMultiLight
			{
				//0
				public Vector3F lightPosition;
				public float lightBoundingRadius;

				//1
				public Vector3F lightDirection;
				public float startDistance;

				//2
				public Vector3F lightPower;
				public float lightType;// public float lightShadowCascadeOverlapping;

				//3
				public Vector3F lightAttenuation;
				public float lightMaskIndex;

				//4
				public Vector3F lightSpot;//public Vector4F lightSpot;
				public float shadowMapIndex;

				//5, 6, 7, 8
				public Matrix4F lightMaskMatrix;

				//9
				public float lightShadowIntensity;
				public float lightShadowTextureSize;
				public float lightShadowMapFarClipDistance;
				public float lightShadowCascadesVisualize;

				//10
				public float lightShadowBias;
				public float lightShadowNormalBias;
				public float lightShadowSoftness;//public float lightSourceRadiusOrAngle;
				public float lightShadowContactLength;

				//11
				public Vector4F unitDistanceWorldShadowTexelSizes;

				//12, 13, 14, 15
				public Matrix4F lightShadowTextureViewProjMatrix0;
				//16, 17, 18, 19
				public Matrix4F lightShadowTextureViewProjMatrix1;
				//20, 21, 22, 23
				public Matrix4F lightShadowTextureViewProjMatrix2;
				//24, 25, 26, 27
				public Matrix4F lightShadowTextureViewProjMatrix3;

				//28
				public Vector4F lightShadowCascades;

				//public float unused1;
				//public float unused2;
				//public float unused3;
				//public float unused4;

				//

				[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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
				{
					//!!!!может проверять по сфере с учетом радиуса как у мешей и билбордов

					distanceToCameraSquared = (float)( context.OwnerCameraSettingsPosition - data.Position ).LengthSquared();
				}
			}

			[MethodImpl( (MethodImplOptions)512 )]
			static void MakeProjectionMatrixForSpotlight( Radian outerAngle, out Matrix4F result )
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

					//double nearFocal = mNearDist;// / mFocalLength;
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

				result = Matrix4F.Zero;
				result[ 0, 0 ] = (float)A;
				result[ 2, 0 ] = (float)C;
				result[ 1, 1 ] = (float)B;
				result[ 2, 1 ] = (float)D;
				result[ 2, 2 ] = (float)q;
				result[ 3, 2 ] = (float)qn;
				result[ 2, 3 ] = -1;
			}

			//[MethodImpl( (MethodImplOptions)512 )]
			//static Matrix4 MakeProjectionMatrixForSpotlight( Radian outerAngle )
			//{
			//	//?
			//	double mNearDist = .1f;
			//	double mFarDist = 100;

			//	// Common calcs
			//	double left, right, bottom, top;
			//	{
			//		Radian thetaY = outerAngle * 0.5;
			//		double tanThetaY = Math.Tan( thetaY );
			//		double tanThetaX = tanThetaY * 1;// mAspect;

			//		double nearFocal = mNearDist;// / mFocalLength;

			//		//double nearOffsetX = 0;// mFrustumOffset.x* nearFocal;
			//		//double nearOffsetY = 0;// mFrustumOffset.y* nearFocal;

			//		double half_w = tanThetaX * mNearDist;
			//		double half_h = tanThetaY * mNearDist;

			//		left = -half_w;// +nearOffsetX;
			//		right = +half_w;// +nearOffsetX;
			//		bottom = -half_h;// +nearOffsetY;
			//		top = +half_h;// +nearOffsetY;
			//	}

			//	// The code below will dealing with general projection 
			//	// parameters, similar glFrustum and glOrtho.
			//	// Doesn't optimise manually except division operator, so the 
			//	// code more self-explaining.

			//	double inv_w = 1.0f / ( right - left );
			//	double inv_h = 1.0f / ( top - bottom );
			//	double inv_d = 1.0f / ( mFarDist - mNearDist );

			//	// Calc matrix elements
			//	double A = 2.0f * mNearDist * inv_w;
			//	double B = 2.0f * mNearDist * inv_h;
			//	double C = ( right + left ) * inv_w;
			//	double D = ( top + bottom ) * inv_h;

			//	double q = -( mFarDist + mNearDist ) * inv_d;
			//	double qn = -2.0f * ( mFarDist * mNearDist ) * inv_d;

			//	// NB: This creates 'uniform' perspective projection matrix,
			//	// which depth range [-1,1], right-handed rules
			//	//
			//	// [ A   0   C   0  ]
			//	// [ 0   B   D   0  ]
			//	// [ 0   0   q   qn ]
			//	// [ 0   0   -1  0  ]
			//	//
			//	// A = 2 * near / (right - left)
			//	// B = 2 * near / (top - bottom)
			//	// C = (right + left) / (right - left)
			//	// D = (top + bottom) / (top - bottom)
			//	// q = - (far + near) / (far - near)
			//	// qn = - 2 * (far * near) / (far - near)

			//	Matrix4 result = Matrix4.Zero;
			//	result[ 0, 0 ] = A;
			//	result[ 2, 0 ] = C;
			//	result[ 1, 1 ] = B;
			//	result[ 2, 1 ] = D;
			//	result[ 2, 2 ] = q;
			//	result[ 3, 2 ] = qn;
			//	result[ 2, 3 ] = -1;

			//	return result;
			//}

			[MethodImpl( (MethodImplOptions)512 )]
			static void MakeViewMatrixForSpotlight( ref Vector3F position, ref QuaternionF rotation, out Matrix4F result )
			{
				// View matrix is:
				//
				//  [ Lx  Uy  Dz  Tx  ]
				//  [ Lx  Uy  Dz  Ty  ]
				//  [ Lx  Uy  Dz  Tz  ]
				//  [ 0   0   0   1   ]
				//
				// Where T = -(Transposed(Rot) * Pos)

				Matrix3F rotationMatrix = rotation.ToMatrix3();
				rotationMatrix *= Matrix3F.FromRotateByY( MathEx.PI / 2 );

				// Make the translation relative to new axes
				var rotationMatrixT = rotationMatrix.GetTranspose();
				var trans = -( rotationMatrixT ) * position;

				result = new Matrix4F( ref rotationMatrixT, ref trans );
			}

			////[MethodImpl( (MethodImplOptions)512 )]
			////static Matrix4 MakeViewMatrixForSpotlight( Vector3 position, QuaternionF rotation )
			////{
			////	// View matrix is:
			////	//
			////	//  [ Lx  Uy  Dz  Tx  ]
			////	//  [ Lx  Uy  Dz  Ty  ]
			////	//  [ Lx  Uy  Dz  Tz  ]
			////	//  [ 0   0   0   1   ]
			////	//
			////	// Where T = -(Transposed(Rot) * Pos)

			////	Matrix3 rotationMatrix = rotation.ToMatrix3();
			////	rotationMatrix *= Matrix3.FromRotateByY( MathEx.PI / 2 );

			////	// Make the translation relative to new axes
			////	Matrix3 rotationMatrixT = rotationMatrix.GetTranspose();
			////	Vector3 trans = -( rotationMatrixT ) * position;

			////	return new Matrix4( rotationMatrixT, trans );
			////}

			//[MethodImpl( (MethodImplOptions)512 )]
			//void InitLightDataBuffers( RenderingPipeline_Basic pipeline, ViewportRenderingContext context )//, double shadowIntensity )
			//{
			//	//!!!!!кешировать и раньше рассчитывать. в RenderSceneData по идее

			//	Vector3F direction;
			//	if( data.Type != Light.TypeEnum.Ambient )
			//		direction = data.Rotation.GetForward();
			//	else
			//		direction = Vector3F.XAxis;

			//	Vector4F position;
			//	if( data.Type != Light.TypeEnum.Ambient )
			//	{
			//		if( data.Type == Light.TypeEnum.Directional )
			//			position = new Vector4F( -direction, 0 );
			//		else
			//		{
			//			//!!!!
			//			context.ConvertToRelative( ref data.Position, out var position3 );
			//			position = new Vector4F( position3, 1 );
			//			//position = new Vector4( data.Position, 1 );
			//		}
			//	}
			//	else
			//		position = Vector4F.Zero;

			//	//lightDataVertex.lightPosition = position;
			//	lightDataFragment.lightPosition = position;
			//	//lightDataVertex.lightPosition = position.ToVector4F();
			//	//lightDataFragment.lightPosition = position.ToVector4F();
			//	lightDataFragment.lightDirection = direction;
			//	lightDataFragment.startDistance = data.StartDistance;
			//	lightDataFragment.lightPower = data.Power;

			//	var near = data.AttenuationNear;
			//	var far = data.AttenuationFar;
			//	near = MathEx.Clamp( near, 0, far - MathEx.Epsilon );
			//	var power = MathEx.Clamp( data.AttenuationPower, MathEx.Epsilon, data.AttenuationPower );
			//	lightDataFragment.lightAttenuation = new Vector4( near, far, power, far - near ).ToVector4F();

			//	Vector4 spot;
			//	if( data.Type == Light.TypeEnum.Spotlight )
			//	{
			//		double inner = data.SpotlightInnerAngle.InRadians();
			//		double outer = data.SpotlightOuterAngle.InRadians();
			//		outer = MathEx.Clamp( outer, .01, Math.PI );
			//		inner = MathEx.Clamp( inner, 0, outer - .0001 );

			//		spot = new Vector4(
			//			Math.Cos( inner * .5 ),
			//			Math.Cos( outer * .5 ),
			//			MathEx.Clamp( data.SpotlightFalloff, MathEx.Epsilon, 1 ),
			//			1 );
			//	}
			//	else
			//		spot = new Vector4( 1, 0, 0, 1 );
			//	lightDataFragment.lightSpot = spot.ToVector4F();

			//	//shadows
			//	if( shadowTexture != null )
			//	{
			//		var shadowViewport = shadowTexture.Result.GetRenderTarget().Viewports[ 0 ];

			//		lightDataFragment.lightShadowIntensity = MathEx.Saturate( (float)( pipeline.ShadowIntensity.Value * data.ShadowIntensity ) );
			//		lightDataFragment.lightShadowTextureSize = shadowTexture.CreateSize.Value.X;
			//		lightDataFragment.lightShadowMapFarClipDistance = (float)shadowViewport.CameraSettings.FarClipDistance;
			//		lightDataFragment.lightShadowCascadesVisualize = pipeline.ShadowDirectionalLightCascadeVisualize ? 1 : -1;
			//		//lightDataFragment.lightShadowCascadeOverlapping = (float)pipeline.ShadowDirectionalLightCascadeOverlapping;

			//		//lightShadowTextureViewProjMatrix
			//		if( data.Type == Light.TypeEnum.Spotlight || data.Type == Light.TypeEnum.Directional )
			//		{
			//			//PC: OriginBottomLeft = false, HomogeneousDepth = false
			//			//Android: OriginBottomLeft = true, HomogeneousDepth = true
			//			var caps = RenderingSystem.Capabilities;
			//			float sy = caps.OriginBottomLeft ? 0.5f : -0.5f;
			//			float sz = /*caps.HomogeneousDepth ? 0.5f :*/ 1.0f;
			//			float tz = /*caps.HomogeneousDepth ? 0.5f :*/ 0.0f;

			//			var toImageSpace = new Matrix4F(
			//				0.5f, 0, 0, 0,
			//				0, sy, 0, 0,
			//				0, 0, sz, 0,
			//				0.5f, 0.5f, tz, 1 );

			//			if( data.Type == Light.TypeEnum.Directional )
			//			{
			//				for( int n = 0; n < shadowCascadesProjectionViewMatrices.Length; n++ )
			//				{
			//					ref var item = ref shadowCascadesProjectionViewMatrices[ n ];
			//					ref Matrix4F shadowProjectionMatrix = ref item.Item1;
			//					ref Matrix4F shadowViewMatrix = ref item.Item2;

			//					var mat = toImageSpace * shadowProjectionMatrix * shadowViewMatrix;
			//					mat.Transpose();

			//					lightDataFragment.SetLightShadowTextureViewProjMatrix( n, ref mat );
			//					lightDataFragment.unitDistanceWorldShadowTexelSizes[ n ] =
			//						1.414213562f // sqrt(2), texel diagonal (maximum) size
			//						* ( 2.0f / shadowProjectionMatrix[ 0 ][ 0 ] ) // r - l
			//						/ lightDataFragment.lightShadowTextureSize; // how many texels in shadow map side

			//				}
			//			}
			//			else // Spot light
			//			{
			//				var shadowViewMatrix = shadowViewport.CameraSettings.ViewMatrixRelative;
			//				var shadowProjectionMatrix = shadowViewport.CameraSettings.ProjectionMatrix;

			//				var mat = toImageSpace * shadowProjectionMatrix * shadowViewMatrix;
			//				mat.Transpose();

			//				lightDataFragment.lightShadowTextureViewProjMatrix0 = mat;
			//				lightDataFragment.unitDistanceWorldShadowTexelSizes[ 0 ] =
			//					1.414213562f // sqrt(2), texel diagonal (maximum) size
			//					* ( 2.0f / shadowProjectionMatrix[ 0 ][ 0 ] ) // (r - l) / (near), world X texel size at unit distance
			//					/ lightDataFragment.lightShadowTextureSize; // how many texels in shadow map side
			//			}
			//		}

			//		if( data.Type == Light.TypeEnum.Point )
			//		{
			//			var shadowProjectionMatrix = shadowViewport.CameraSettings.ProjectionMatrix;

			//			lightDataFragment.unitDistanceWorldShadowTexelSizes[ 0 ] =
			//				1.414213562f // sqrt(2), texel diagonal (maximum) size
			//				* ( 2.0f / shadowProjectionMatrix[ 0 ][ 0 ] ) // (r - l) / (near), world X texel size at unit distance
			//				/ lightDataFragment.lightShadowTextureSize; // how many texels in shadow map side
			//		}

			//		//lightShadowCascades
			//		if( data.Type == Light.TypeEnum.Directional )
			//		{
			//			lightDataFragment.lightShadowCascades = Vector4F.Zero;
			//			lightDataFragment.lightShadowCascades[ 0 ] = pipeline.ShadowDirectionalLightCascades;
			//			var splitDistances = pipeline.GetShadowCascadeSplitDistances( context );

			//			if( pipeline.ShadowDirectionalLightCascades >= 2 )
			//				lightDataFragment.lightShadowCascades[ 1 ] = (float)splitDistances[ 1 ];
			//			else
			//				lightDataFragment.lightShadowCascades[ 1 ] = (float)pipeline.ShadowDirectionalDistance;

			//			if( pipeline.ShadowDirectionalLightCascades >= 3 )
			//				lightDataFragment.lightShadowCascades[ 2 ] = (float)splitDistances[ 2 ];
			//			else
			//				lightDataFragment.lightShadowCascades[ 2 ] = (float)pipeline.ShadowDirectionalDistance;

			//			if( pipeline.ShadowDirectionalLightCascades >= 4 )
			//				lightDataFragment.lightShadowCascades[ 3 ] = (float)splitDistances[ 3 ];
			//			else
			//				lightDataFragment.lightShadowCascades[ 3 ] = (float)pipeline.ShadowDirectionalDistance;
			//		}
			//	}

			//	//mask
			//	if( RenderingSystem.LightMask )
			//	{
			//		if( data.Mask != null )
			//		{
			//			Matrix4F matrix;

			//			switch( data.Type )
			//			{
			//			case Light.TypeEnum.Directional:
			//				{
			//					var maskTransform = data.MaskTransform;

			//					var offset = context.OwnerCameraSettingsPosition * maskTransform.Scale;
			//					var maskTransformWithOffset = maskTransform.UpdatePosition( new Vector3( offset ) );
			//					maskTransformWithOffset.ToMatrix4().ToMatrix4F( out matrix );

			//					//Vector3F dir = -data.Rotation.GetForward();
			//					//double s = 0;
			//					//if( data.MaskScale != 0 )
			//					//	s = 1.0 / data.MaskScale;
			//					//Matrix4 m = new Matrix4(
			//					//	s, 0, dir.X * s, -data.Position.X * s,
			//					//	0, s, dir.Y * s, -data.Position.Y * s,
			//					//	0, 0, 1, 0,
			//					//	0, 0, 0, 1 );
			//					//mat = m.GetTranspose().ToMatrix4F();
			//				}
			//				break;

			//			case Light.TypeEnum.Point:
			//				{
			//					//!!!!slowly

			//					Matrix3 m = data.Rotation.ToMatrix3().GetTranspose();
			//					//!!!!fix flipped cubemaps
			//					//dir = double3( -dir.y, dir.z, dir.x );
			//					Matrix3 flipMatrix = new Matrix3(
			//						0, -1, 0,
			//						0, 0, 1,
			//						1, 0, 0 );

			//					matrix = ( flipMatrix.GetTranspose() * m ).ToMatrix4().ToMatrix4F();


			//					//Matrix3 m = data.Rotation.ToMatrix3().GetTranspose();
			//					////!!!!fix flipped cubemaps
			//					////dir = double3( -dir.y, dir.z, dir.x );
			//					//Matrix3 flipMatrix = new Matrix3(
			//					//	0, -1, 0,
			//					//	0, 0, 1,
			//					//	1, 0, 0 );

			//					////!!!!double
			//					//mat = ( flipMatrix.GetTranspose() * m ).ToMatrix4().ToMatrix4F();
			//				}
			//				break;

			//			case Light.TypeEnum.Spotlight:
			//				{
			//					//!!!!slowly

			//					context.ConvertToRelative( ref data.Position, out var position3 );

			//					var projectionClipSpace2DToImageSpace = new Matrix4F(
			//						0.5f, 0, 0, 0,
			//						0, -0.5f, 0, 0,
			//						0, 0, 1, 0,
			//						0.5f, 0.5f, 0, 1 );
			//					MakeProjectionMatrixForSpotlight( data.SpotlightOuterAngle.InRadians(), out var projectionMatrix );
			//					MakeViewMatrixForSpotlight( ref position3, ref data.Rotation, out var viewMatrix );

			//					matrix = projectionClipSpace2DToImageSpace * projectionMatrix * viewMatrix;


			//					//Matrix4 projectionClipSpace2DToImageSpace = new Matrix4(
			//					//	0.5f, 0, 0, 0,
			//					//	0, -0.5f, 0, 0,
			//					//	0, 0, 1, 0,
			//					//	0.5f, 0.5f, 0, 1 );
			//					//Matrix4 projectionMatrix = MakeProjectionMatrixForSpotlight( data.SpotlightOuterAngle.InRadians() );
			//					//Matrix4 viewMatrix = MakeViewMatrixForSpotlight( data.Position, data.Rotation );

			//					////!!!!double
			//					//mat = ( projectionClipSpace2DToImageSpace * projectionMatrix * viewMatrix ).ToMatrix4F();
			//				}
			//				break;

			//			default:
			//				matrix = Matrix4F.Zero;
			//				break;
			//			}

			//			matrix.Transpose();
			//			lightDataFragment.lightMaskMatrix = matrix;
			//		}
			//		else
			//			lightDataFragment.lightMaskMatrix = Matrix4F.Zero;
			//	}

			//	lightDataFragment.lightShadowBias = data.ShadowBias;
			//	lightDataFragment.lightShadowNormalBias = data.ShadowNormalBias;
			//	lightDataFragment.lightShadowSoftness = data.ShadowSoftness;
			//	//lightDataFragment.lightSourceRadiusOrAngle = data.SourceRadiusOrAngle;
			//	lightDataFragment.lightShadowContactLength = data.ShadowContactLength;
			//	//lightDataFragment.lightShadowQuality = pipeline.ShadowQuality.Value == ShadowQualityEnum.High ? 1 : 0;

			//	//pipeline.InitLightDataBuffersEvent?.Invoke( pipeline, context, this );
			//}

			//[MethodImpl( (MethodImplOptions)512 )]
			//public unsafe void Bind( RenderingPipeline_Basic pipeline, ViewportRenderingContext context )
			//{
			//	//init
			//	if( !lightDataBuffersInitialized )
			//	{
			//		InitLightDataBuffers( pipeline, context );
			//		lightDataBuffersInitialized = true;
			//	}

			//	////set vertex data uniform
			//	//{
			//	//	int vec4Count = sizeof( LightDataVertex ) / sizeof( Vector4F );
			//	//	if( vec4Count != 1 )
			//	//		Log.Fatal( "ViewportRenderingContext: SetUniforms: vec4Count != 1." );
			//	//	if( !u_lightDataVertex.HasValue )
			//	//		u_lightDataVertex = GpuProgramManager.RegisterUniform( "u_lightDataVertex", UniformType.Vector4, vec4Count );
			//	//	fixed( LightDataVertex* p = &lightDataVertex )
			//	//		Bgfx.SetUniform( u_lightDataVertex.Value, p, vec4Count );
			//	//}

			//	//set fragment data uniform
			//	{
			//		int vec4Count = sizeof( LightDataFragment ) / sizeof( Vector4F );
			//		if( vec4Count != 29 )
			//			Log.Fatal( "ViewportRenderingContext: SetUniforms: vec4Count != 29." );
			//		if( !u_lightDataFragment.HasValue )
			//			u_lightDataFragment = GpuProgramManager.RegisterUniform( "u_lightDataFragment", UniformType.Vector4, vec4Count );
			//		fixed( LightDataFragment* p = &lightDataFragment )
			//			Bgfx.SetUniform( u_lightDataFragment.Value, p, vec4Count );
			//	}
			//}

			[MethodImpl( (MethodImplOptions)512 )]
			public unsafe void InitLightDataBuffersMultiLight( RenderingPipeline_Basic pipeline, ViewportRenderingContext context, LightDataFragmentMultiLight* output )
			{
				//!!!!!может что-то кешировать, раньше рассчитывать. в RenderSceneData по идее

				output->lightType = (float)data.Type;
				if( prepareShadows )
					output->shadowMapIndex = shadowMapIndex;
				else
					output->shadowMapIndex = -1;

				output->lightMaskIndex = lightMaskIndex;


				if( data.Type == Light.TypeEnum.Directional || data.Type == Light.TypeEnum.Ambient )
					output->lightBoundingRadius = 1000000.0f;
				else
					output->lightBoundingRadius = data.AttenuationFar;


				Vector3F direction;
				if( data.Type != Light.TypeEnum.Ambient )
					direction = data.Rotation.GetForward();
				else
					direction = Vector3F.XAxis;

				Vector4F position;
				if( data.Type != Light.TypeEnum.Ambient )
				{
					if( data.Type == Light.TypeEnum.Directional )
						position = new Vector4F( -direction, 0 );
					else
					{
						context.ConvertToRelative( ref data.Position, out var position3 );
						position = new Vector4F( position3, 1 );
						//position = new Vector4( data.Position, 1 );
					}
				}
				else
					position = Vector4F.Zero;

				//lightDataVertex.lightPosition = position;
				output->lightPosition = position.ToVector3F();
				//lightDataVertex.lightPosition = position.ToVector4F();
				//output->lightPosition = position.ToVector4F();
				output->lightDirection = direction;
				output->startDistance = data.StartDistance;
				output->lightPower = data.Power / 10000.0f;

				if( data.Type == Light.TypeEnum.Spotlight || data.Type == Light.TypeEnum.Point )
				{
					var near = data.AttenuationNear;
					var far = data.AttenuationFar;
					near = MathEx.Clamp( near, 0, far - MathEx.Epsilon );
					var power = MathEx.Clamp( data.AttenuationPower, MathEx.Epsilon, data.AttenuationPower );
					output->lightAttenuation = new Vector3F( near, power, far - near );
					//output->lightAttenuation = new Vector4( near, far, power, far - near ).ToVector4F();
				}
				else
					output->lightAttenuation = Vector3F.Zero;

				if( data.Type == Light.TypeEnum.Spotlight )
				{
					float inner = data.SpotlightInnerAngle.InRadians();
					float outer = data.SpotlightOuterAngle.InRadians();
					outer = MathEx.Clamp( outer, .01f, MathEx.PI );
					inner = MathEx.Clamp( inner, 0, outer - .0001f );

					output->lightSpot = new Vector3F(
						MathEx.Cos( inner * 0.5f ),
						MathEx.Cos( outer * 0.5f ),
						MathEx.Clamp( data.SpotlightFalloff, MathEx.Epsilon, 1 ) );
				}
				else
					output->lightSpot = Vector3F.Zero;

				//Vector4 spot;
				//if( data.Type == Light.TypeEnum.Spotlight )
				//{
				//	double inner = data.SpotlightInnerAngle.InRadians();
				//	double outer = data.SpotlightOuterAngle.InRadians();
				//	outer = MathEx.Clamp( outer, .01, Math.PI );
				//	inner = MathEx.Clamp( inner, 0, outer - .0001 );

				//	spot = new Vector4(
				//		Math.Cos( inner * .5 ),
				//		Math.Cos( outer * .5 ),
				//		MathEx.Clamp( data.SpotlightFalloff, MathEx.Epsilon, 1 ),
				//		1 );
				//}
				//else
				//	spot = new Vector4( 1, 0, 0, 1 );
				//output->lightSpot = spot.ToVector4F();

				//mask
				if( RenderingSystem.LightMask )
				{
					if( data.Mask != null && output->lightMaskIndex >= 0 )
					{
						Matrix4F matrix;

						switch( data.Type )
						{
						case Light.TypeEnum.Directional:
							{
								var maskTransform = data.MaskTransform;

								var offset = context.OwnerCameraSettingsPosition * maskTransform.Scale;
								var maskTransformWithOffset = maskTransform.UpdatePosition( new Vector3( offset ) );
								maskTransformWithOffset.ToMatrix4().ToMatrix4F( out matrix );

								//Vector3F dir = -data.Rotation.GetForward();
								//double s = 0;
								//if( data.MaskScale != 0 )
								//	s = 1.0 / data.MaskScale;
								//Matrix4 m = new Matrix4(
								//	s, 0, dir.X * s, -data.Position.X * s,
								//	0, s, dir.Y * s, -data.Position.Y * s,
								//	0, 0, 1, 0,
								//	0, 0, 0, 1 );
								//mat = m.GetTranspose().ToMatrix4F();
							}
							break;

						case Light.TypeEnum.Point:
							{
								//!!!!slowly

								Matrix3 m = data.Rotation.ToMatrix3().GetTranspose();
								//!!!!fix flipped cubemaps
								//dir = double3( -dir.y, dir.z, dir.x );
								Matrix3 flipMatrix = new Matrix3(
									0, -1, 0,
									0, 0, 1,
									1, 0, 0 );

								matrix = ( flipMatrix.GetTranspose() * m ).ToMatrix4().ToMatrix4F();


								//Matrix3 m = data.Rotation.ToMatrix3().GetTranspose();
								////!!!!fix flipped cubemaps
								////dir = double3( -dir.y, dir.z, dir.x );
								//Matrix3 flipMatrix = new Matrix3(
								//	0, -1, 0,
								//	0, 0, 1,
								//	1, 0, 0 );

								////!!!!double
								//mat = ( flipMatrix.GetTranspose() * m ).ToMatrix4().ToMatrix4F();
							}
							break;

						case Light.TypeEnum.Spotlight:
							{
								//!!!!slowly

								context.ConvertToRelative( ref data.Position, out var position3 );

								var projectionClipSpace2DToImageSpace = new Matrix4F(
									0.5f, 0, 0, 0,
									0, -0.5f, 0, 0,
									0, 0, 1, 0,
									0.5f, 0.5f, 0, 1 );
								MakeProjectionMatrixForSpotlight( data.SpotlightOuterAngle.InRadians(), out var projectionMatrix );
								MakeViewMatrixForSpotlight( ref position3, ref data.Rotation, out var viewMatrix );

								matrix = projectionClipSpace2DToImageSpace * projectionMatrix * viewMatrix;


								//Matrix4 projectionClipSpace2DToImageSpace = new Matrix4(
								//	0.5f, 0, 0, 0,
								//	0, -0.5f, 0, 0,
								//	0, 0, 1, 0,
								//	0.5f, 0.5f, 0, 1 );
								//Matrix4 projectionMatrix = MakeProjectionMatrixForSpotlight( data.SpotlightOuterAngle.InRadians() );
								//Matrix4 viewMatrix = MakeViewMatrixForSpotlight( data.Position, data.Rotation );

								////!!!!double
								//mat = ( projectionClipSpace2DToImageSpace * projectionMatrix * viewMatrix ).ToMatrix4F();
							}
							break;

						default:
							matrix = Matrix4F.Zero;
							break;
						}

						matrix.GetTranspose( out output->lightMaskMatrix );
						//matrix.Transpose();
						//output->lightMaskMatrix = matrix;
					}
					//else
					//	output->lightMaskMatrix = Matrix4F.Zero;
				}

				//shadows
				if( /*shadowTexture != null &&*/ output->shadowMapIndex >= 0 && shadowCameraSettings != null )
				{
					//var shadowViewport = shadowTexture.Result.GetRenderTarget().Viewports[ 0 ];

					output->lightShadowIntensity = MathEx.Saturate( (float)( pipeline.ShadowIntensity.Value * data.ShadowIntensity ) );
					output->lightShadowTextureSize = shadowSourceTextureSize;//shadowTexture.CreateSize.Value.X;
					output->lightShadowMapFarClipDistance = (float)shadowCameraSettings/*shadowViewport.CameraSettings*/.FarClipDistance;
					output->lightShadowCascadesVisualize = pipeline.ShadowDirectionalLightCascadeVisualize ? 1 : -1;
					//output->lightShadowCascadeOverlapping = (float)pipeline.ShadowDirectionalLightCascadeOverlapping;

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

								output->SetLightShadowTextureViewProjMatrix( n, ref mat );
								output->unitDistanceWorldShadowTexelSizes[ n ] =
									1.414213562f // sqrt(2), texel diagonal (maximum) size
									* ( 2.0f / shadowProjectionMatrix[ 0 ][ 0 ] ) // r - l
									/ output->lightShadowTextureSize; // how many texels in shadow map side

							}
						}
						else // Spot light
						{
							var shadowViewMatrix = shadowCameraSettings/*shadowViewport.CameraSettings*/.ViewMatrixRelative;
							var shadowProjectionMatrix = shadowCameraSettings/*shadowViewport.CameraSettings*/.ProjectionMatrix;

							var mat = toImageSpace * shadowProjectionMatrix * shadowViewMatrix;
							mat.Transpose();

							output->lightShadowTextureViewProjMatrix0 = mat;
							output->unitDistanceWorldShadowTexelSizes[ 0 ] =
								1.414213562f // sqrt(2), texel diagonal (maximum) size
								* ( 2.0f / shadowProjectionMatrix[ 0 ][ 0 ] ) // (r - l) / (near), world X texel size at unit distance
								/ output->lightShadowTextureSize; // how many texels in shadow map side
						}
					}

					if( data.Type == Light.TypeEnum.Point )
					{
						var shadowProjectionMatrix = shadowCameraSettings/*shadowViewport.CameraSettings*/.ProjectionMatrix;

						output->unitDistanceWorldShadowTexelSizes[ 0 ] =
							1.414213562f // sqrt(2), texel diagonal (maximum) size
							* ( 2.0f / shadowProjectionMatrix[ 0 ][ 0 ] ) // (r - l) / (near), world X texel size at unit distance
							/ output->lightShadowTextureSize; // how many texels in shadow map side
					}

					//lightShadowCascades
					if( data.Type == Light.TypeEnum.Directional )
					{
						output->lightShadowCascades = Vector4F.Zero;
						output->lightShadowCascades[ 0 ] = pipeline.ShadowDirectionalLightCascades;
						var splitDistances = pipeline.GetShadowCascadeSplitDistances( context );

						if( pipeline.ShadowDirectionalLightCascades >= 2 )
							output->lightShadowCascades[ 1 ] = (float)splitDistances[ 1 ];
						else
							output->lightShadowCascades[ 1 ] = (float)pipeline.ShadowDirectionalDistance;

						if( pipeline.ShadowDirectionalLightCascades >= 3 )
							output->lightShadowCascades[ 2 ] = (float)splitDistances[ 2 ];
						else
							output->lightShadowCascades[ 2 ] = (float)pipeline.ShadowDirectionalDistance;

						if( pipeline.ShadowDirectionalLightCascades >= 4 )
							output->lightShadowCascades[ 3 ] = (float)splitDistances[ 3 ];
						else
							output->lightShadowCascades[ 3 ] = (float)pipeline.ShadowDirectionalDistance;
					}
				}

				output->lightShadowBias = data.ShadowBias;
				output->lightShadowNormalBias = data.ShadowNormalBias;
				output->lightShadowSoftness = data.ShadowSoftness;
				//output->lightSourceRadiusOrAngle = data.SourceRadiusOrAngle;
				output->lightShadowContactLength = data.ShadowContactLength;
				//output->lightShadowQuality = pipeline.ShadowQuality.Value == ShadowQualityEnum.High ? 1 : 0;

				//pipeline.InitLightDataBuffersEvent?.Invoke( pipeline, context, this );
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

			public ImageComponent CubemapEnvironmentWithMipmapsAndBlur;

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

					for( int nPassType = 0; nPassType < 3; nPassType++ )//for( int nPassType = 0; nPassType < 4; nPassType++ )
					{
						var voxelPass = nPassType == 1;
						//var virtualizedPass = nPassType == 2;
						var billboardPass = nPassType == 2;// 3;

						//generate compile arguments
						var vertexDefines = new List<(string, string)>();
						var fragmentDefines = new List<(string, string)>();
						{
							var generalDefines = new List<(string, string)>();
							generalDefines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );
							if( voxelPass )
								generalDefines.Add( ("VOXEL", "") );
							//if( virtualizedPass )
							//	generalDefines.Add( ("VIRTUALIZED", "") );
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

						var pass = new GpuMaterialPass( null, vertexProgram, fragmentProgram );
						data.passByLightType[ (int)lightType ].Set( pass, voxelPass/*, virtualizedPass*/, billboardPass );

						//pass.CullingMode = CullingMode.None;
					}
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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
			int managerIndex;

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

			public OutputInstancingManager( int managerIndex )
			{
				this.managerIndex = managerIndex;

				int multiplier = SystemSettings.LimitedDevice ? 1 : 4;

				renderableItemArrays = new Stack<IntPtr>( 16 * multiplier );
				notCompletedOutputItemsTableToClear = new OpenList<int>( 32 * multiplier );
				items = new OpenList<Item>( 128 * multiplier );
				operations = new OpenList<RenderSceneData.MeshDataRenderOperation>( 64 * multiplier );
				materialDatas = new OpenList<Material.CompiledMaterialData>( 32 * multiplier );
				outputItems = new OpenList<OutputItem>( 128 * multiplier );
			}

			public void Init( RenderingPipeline pipeline, bool forGI )
			{
				var demandedInstancingMaxCount = forGI ? 64 : pipeline.InstancingMaxCount.Value;

				//drop renderableItemArrays when InstancingMaxCount was changed
				if( instancingMaxCount != demandedInstancingMaxCount )
				{
					while( renderableItemArrays.Count != 0 )
						NativeUtility.Free( renderableItemArrays.Pop() );
				}

				instancingMaxCount = demandedInstancingMaxCount;
			}

			public void Dispose()
			{
				while( renderableItemArrays.Count != 0 )
					NativeUtility.Free( renderableItemArrays.Pop() );
			}

			[MethodImpl( (MethodImplOptions)512 )]
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

				if( operation._currentRenderingFrameIndex[ managerIndex ] == -1 )
				{
					operation._currentRenderingFrameIndex[ managerIndex ] = operations.Count;
					operations.Add( operation );
				}
				if( materialData._currentRenderingFrameIndex[ managerIndex ] == -1 )
				{
					materialData._currentRenderingFrameIndex[ managerIndex ] = materialDatas.Count;
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

			[MethodImpl( (MethodImplOptions)512 )]
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

						int tableIndex = item.operation._currentRenderingFrameIndex[ managerIndex ] * materialDatas.Count + item.materialData._currentRenderingFrameIndex[ managerIndex ];

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
							ref var outputItem = ref outputItems.AddNotInitialized(); //var outputItem = new OutputItem();
							outputItem.operation = item.operation;
							outputItem.materialData = item.materialData;
							outputItem.allowInstancing = true;
							outputItem.renderableItemFirst = new Vector3I( item.renderableGroup, item.operationIndex );
							outputItem.renderableItemsCount = 1;
							outputItem.renderableItems = null;
							//outputItems.Add( ref outputItem );

							//add to the table
							notCompletedOutputItemsTable[ tableIndex ] = outputItems.Count - 1;
							notCompletedOutputItemsTableToClear.Add( tableIndex );
						}
					}
					else
					{
						//without instancing
						ref var outputItem = ref outputItems.AddNotInitialized(); //var outputItem = new OutputItem();
						outputItem.operation = item.operation;
						outputItem.materialData = item.materialData;
						outputItem.allowInstancing = false;
						outputItem.renderableItemFirst = new Vector3I( item.renderableGroup, item.operationIndex );
						outputItem.renderableItemsCount = 1;
						outputItem.renderableItems = null;
						//outputItems.Add( ref outputItem );
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

			[MethodImpl( (MethodImplOptions)512 )]
			public unsafe void Clear()
			{
				items.Clear();

				for( int n = 0; n < operations.Count; n++ )
					operations.Data[ n ]._currentRenderingFrameIndex[ managerIndex ] = -1;
				operations.Clear();

				for( int n = 0; n < materialDatas.Count; n++ )
					materialDatas.Data[ n ]._currentRenderingFrameIndex[ managerIndex ] = -1;
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

		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		struct RenderOperationDataStructure
		{
			public Vector4F data0;
			public Vector4F data1;
			public Vector4F data2;
			public Vector4F data3;
			public ColorValue data4;
			public Vector4F data5;
			public Vector4F data6;

			public Vector3F data71;
			public float data72;
		}

		/////////////////////////////////////////

		static RenderingPipeline_Basic()
		{
			//renderOperationDataCurrent.data0.X = float.MaxValue;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void PrepareSoftwareOcclusionBuffer( ViewportRenderingContext context, Scene scene, Viewport.CameraSettingsClass cameraSettings, bool sceneOccluder, out OcclusionCullingBuffer sceneOcclusionCullingBuffer, out bool occlusionCullingBufferRendered )
		{
			sceneOcclusionCullingBuffer = null;
			occlusionCullingBufferRendered = false;

			if( OcclusionCullingBuffer.Supported && OcclusionCullingBufferSize.Value > 0 )
			{
				var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, false, cameraSettings.Frustum );
				getObjectsItem.GetFromOctree = Scene.SceneObjectFlags.Occluder;
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
					var demandedSize = OcclusionCullingBuffer.GetSizeByHeight( context.Owner.SizeInPixels, OcclusionCullingBufferSize );
					var buffer = context.OcclusionCullingBuffer_Alloc( demandedSize, cameraSettings.Projection == ProjectionType.Orthographic );

					OcclusionCullingBuffer_RenderOccluders( context, cameraSettings, buffer, occluders, sceneOccluder, ref occlusionCullingBufferRendered );

					sceneOcclusionCullingBuffer = buffer;
				}
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected virtual void PrepareListsOfObjects( ViewportRenderingContext context, FrameData frameData )
		{
			Viewport viewportOwner = context.Owner;
			var renderSceneData = frameData.RenderSceneData;

			var frameDataMeshes = frameData.Meshes;
			var frameDataBillboards = frameData.Billboards;
			var renderSceneDataMeshes = renderSceneData.Meshes;
			var renderSceneDataBillboards = renderSceneData.Billboards;

			var scene = viewportOwner.AttachedScene;
			if( scene == null || !scene.EnabledInHierarchy )
				return;

			OcclusionCullingBuffer sceneOcclusionCullingBuffer = null;
			var occlusionCullingBufferRendered = false;
			if( OcclusionCullingBufferScene )
			{
				PrepareSoftwareOcclusionBuffer( context, scene, viewportOwner.CameraSettings, true, out sceneOcclusionCullingBuffer, out occlusionCullingBufferRendered );
			}
			if( !occlusionCullingBufferRendered && DebugMode.Value == DebugModeEnum.OcclusionCullingBuffer )
				context.Owner.CanvasRenderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new ColorValue( 0, 0, 0 ) );

			context.SceneOcclusionCullingBuffer = sceneOcclusionCullingBuffer;

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

#if __
			//find indirect lighting component. full mode
			if( context.DeferredShading && DebugIndirectLighting )
			{
				var effect = GetSceneEffect<RenderingEffect_IndirectLighting>();
				if( effect != null && effect.Intensity > 0 && effect.IsSupported )
				{
					if( effect.Technique.Value == RenderingEffect_IndirectLighting.TechniqueEnum.Full )
					{
						var cameraSettings = context.Owner.CameraSettings;

						var data = new RenderingEffect_IndirectLighting.FrameData();
						data.Owner = effect;
						data.Levels = effect.Levels.Value;

						//!!!!плавное смещение центра или еще что

						data.GridCenter = cameraSettings.Position;
						data.GridResolution = effect.GetGridSize();

						data.DistanceByLevel = new float[ data.Levels ];
						data.DistanceByLevel[ data.Levels - 1 ] = (float)effect.Distance.Value;
						for( int n = data.Levels - 2; n >= 0; n-- )
							data.DistanceByLevel[ n ] = data.DistanceByLevel[ n + 1 ] / 2;

						data.CellSizeLevel0 = ( data.DistanceByLevel[ 0 ] * 2.0f ) / data.GridResolution;

						data.BoundsByLevel = new Bounds[ data.Levels ];
						for( int level = 0; level < data.Levels; level++ )
						{
							var levelBounds = new Bounds( data.GridCenter );
							levelBounds.Expand( data.DistanceByLevel[ level ] );
							data.BoundsByLevel[ level ] = levelBounds;
						}

						data.TotalGridBounds = new Bounds( data.GridCenter );
						data.TotalGridBounds.Expand( effect.Distance );


						//var gridSideSize = distance * 2;

						//data.CellSize = gridSideSize / gridSize;
						//data.CellSizeInv = 1.0f / data.CellSize;
						//data.GridPosition = cameraSettings.Position - new Vector3( distance, distance, distance );

						//data.GridBounds = new Bounds( data.GridPosition, data.GridPosition + data.CellSize * new Vector3( gridSize, gridSize, gridSize ) );

						frameData.IndirectLightingFrameData = data;
					}
				}
			}
#endif

			//Scene.GetRenderSceneData
			frameData.SceneGetRenderSceneData( context, scene );

			//get objects
			{
				var cameraSettings = context.Owner.CameraSettings;
				var cameraFrustum = viewportOwner.CameraSettings.Frustum;
				var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, cameraFrustum );
				getObjectsItem.GetFromOctree = Scene.SceneObjectFlags.Visual;

				var extensionData = new Scene.GetObjectsInSpaceItem.ExtensionDataStructure();
				if( sceneOcclusionCullingBuffer != null )
				{
					extensionData.OcclusionCullingBuffer = sceneOcclusionCullingBuffer.NativeObject;
					extensionData.CameraPosition = context.OwnerCameraSettingsPosition;
					extensionData.ViewProjectionMatrix = cameraSettings.GetViewProjectionMatrixRelative();
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

			////Scene.GetRenderSceneDataAfterObjects
			//frameData.SceneGetRenderSceneDataAfterObjects( context, scene );


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
				using( var list = new OpenListNative<int>( frameData.Lights.Count ) )
				{
					for( int lightIndex = 0; lightIndex < frameData.Lights.Count; lightIndex++ )
					{
						//remove all except ambient light for debug modes
						if( DebugMode.Value != DebugModeEnum.None && DebugMode.Value != DebugModeEnum.Wireframe )
						{
							var lightItem = frameData.Lights[ lightIndex ];
							if( lightItem.data.Type != Light.TypeEnum.Ambient )
								continue;
						}

						if( disableLights == null || !disableLights.Contains( lightIndex ) )
							list.Add( lightIndex );
					}
					//!!!!GC
					frameData.LightsInFrustumSorted = list.ToArray();
				}

				CollectionUtility.MergeSortUnmanaged( frameData.LightsInFrustumSorted, delegate ( int index1, int index2 )
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
				}, true );

				//max light count
				if( frameData.LightsInFrustumSorted.Length > LightMaxCount )
				{
					var old = frameData.LightsInFrustumSorted;
					frameData.LightsInFrustumSorted = new int[ LightMaxCount ];
					Array.Copy( old, 0, frameData.LightsInFrustumSorted, 0, frameData.LightsInFrustumSorted.Length );
				}
			}

			//LightItem.prepareShadows
			if( Shadows && GlobalShadowQuality > 0 && ShadowIntensity > 0 && UseRenderTargets && DebugMode.Value == DebugModeEnum.None && RenderingSystem.ShadowTechnique != ProjectSettingsPage_Rendering.ShadowTechniqueEnum.None && DebugDrawShadows )
			{
				int[] remains = new int[ 4 ];
				remains[ (int)Light.TypeEnum.Directional ] = 1;//ShadowDirectionalLightMaxCount;
				remains[ (int)Light.TypeEnum.Point ] = ShadowPointLightMaxCount;
				remains[ (int)Light.TypeEnum.Spotlight ] = ShadowSpotlightMaxCount;

				var cameraPosition = viewportOwner.CameraSettings.Position;
				double shadowPointSpotlightDistance = ShadowPointSpotlightDistance;

				var shadowMapIndexCounterDirectional = 0;
				var shadowMapIndexCounterSpot = 0;
				var shadowMapIndexCounterPoint = 0;

				//!!!!так распределять? просто по расстоянию от центра
				foreach( var lightIndex in frameData.LightsInFrustumSorted )
				{
					var item = frameData.Lights[ lightIndex ];

					if( item.data.CastShadows && item.data.ShadowIntensity > 0 && remains[ (int)item.data.Type ] > 0 )
					{
						bool skip = false;
						if( item.data.Type == Light.TypeEnum.Point )
						{
							if( ( cameraPosition - item.data.Position ).LengthSquared() > ( shadowPointSpotlightDistance + item.data.AttenuationFar ) * ( shadowPointSpotlightDistance + item.data.AttenuationFar ) )
								skip = true;
						}
						else if( item.data.Type == Light.TypeEnum.Spotlight )
						{
							if( item.data.BoundingBox.GetPointDistance( cameraPosition ) > shadowPointSpotlightDistance )
								skip = true;
						}

						if( !skip )
						{
							item.prepareShadows = true;
							remains[ (int)item.data.Type ] = remains[ (int)item.data.Type ] - 1;

							if( item.data.Type == Light.TypeEnum.Directional )
								item.shadowMapIndex = shadowMapIndexCounterDirectional++;
							else if( item.data.Type == Light.TypeEnum.Spotlight )
								item.shadowMapIndex = shadowMapIndexCounterSpot++;
							else
								item.shadowMapIndex = shadowMapIndexCounterPoint++;
						}
					}
				}
			}

			//get affected objects for lights, shadows affected meshes in space
			{
				//step 1. prepare data to get affects objects of the scene.
				//!!!!GC
				var lightAffectedObjects = new LightAffectedObjectsItem[ frameData.Lights.Count ];
				//!!!!GC
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
									getObjectsItem.GetFromOctree = Scene.SceneObjectFlags.Visual;


									//software occlusion buffer
									if( OcclusionCullingBufferDirectionalLight )
									{
										//!!!!threading?

										//!!!!copy code

										var lightData = lightItem.data;

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

										double shadowMapFarClipDistance = ShadowDirectionalLightExtrusionDistance * 2;

										var cascadeCameraSettings = new Viewport.CameraSettingsClass( context.OwnerCameraSettingsPosition, null, 1, 90, lightData.ShadowNearClipDistance/*shadowMapFarClipDistance / 1000.0*/, shadowMapFarClipDistance, pos, dir, up, ProjectionType.Orthographic, orthoSize, 0, 0 );

										PrepareSoftwareOcclusionBuffer( context, scene, cascadeCameraSettings, false, out var cascadeOcclusionCullingBuffer, out var cascadeOcclusionCullingBufferRendered );

										var extensionData = new Scene.GetObjectsInSpaceItem.ExtensionDataStructure();
										if( cascadeOcclusionCullingBuffer != null )
										{
											extensionData.OcclusionCullingBuffer = cascadeOcclusionCullingBuffer.NativeObject;
											extensionData.CameraPosition = context.OwnerCameraSettingsPosition;
											extensionData.ViewProjectionMatrix = cascadeCameraSettings.GetViewProjectionMatrixRelative();
											extensionData.OcclusionCullingBufferCullNodes = OcclusionCullingBufferCullNodes.Value ? 1 : 0;
											extensionData.OcclusionCullingBufferCullObjects = OcclusionCullingBufferCullObjects.Value ? 1 : 0;
											unsafe
											{
												getObjectsItem.ExtensionData = new IntPtr( &extensionData );
											}
										}
									}

									item.SetItem( nIteration, getObjectsItem );
									lightAffectedObjectsGetObjectsList.Add( getObjectsItem );
								}

								lightAffectedObjects[ lightIndex ] = item;
							}
						}
						else if( lightItem.data.Type == Light.TypeEnum.Point )
						{
							//Point light

							//!!!!new
							if( lightItem.prepareShadows )
							{
								var sphere = new Sphere( lightItem.data.Position, lightItem.data.AttenuationFar );
								var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );
								getObjectsItem.GetFromOctree = Scene.SceneObjectFlags.Visual;

								var item = new LightAffectedObjectsItem();
								item.SetItem( 0, getObjectsItem );
								lightAffectedObjects[ lightIndex ] = item;
								lightAffectedObjectsGetObjectsList.Add( getObjectsItem );
							}
						}
						else if( lightItem.data.Type == Light.TypeEnum.Spotlight )
						{
							//Spotlight

							//!!!!new
							if( lightItem.prepareShadows )
							{
								var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, lightItem.data.SpotlightClipPlanes, lightItem.data.BoundingBox );
								getObjectsItem.GetFromOctree = Scene.SceneObjectFlags.Visual;

								var item = new LightAffectedObjectsItem();
								item.SetItem( 0, getObjectsItem );
								lightAffectedObjects[ lightIndex ] = item;
								lightAffectedObjectsGetObjectsList.Add( getObjectsItem );
							}
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
								using( var objectsData = new OpenListNative<ShadowsObjectData>( getObjectsItem.Result.Length ) )
								{
									for( int nObject = 0; nObject < getObjectsItem.Result.Length; nObject++ )
									{
										var obj = getObjectsItem.Result[ nObject ].Object;
										if( obj.VisibleInHierarchy )
										{
											var item = new ShadowsObjectData();
											item.position = obj.TransformV.Position;
											item.boundingSphere = obj.SpaceBounds.boundingSphere;
											//obj.SpaceBounds.GetCalculatedBoundingSphere( out item.boundingSphere );
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


										//!!!!толку в этом всём мало. да и глючит либа на больших объектах
										//!!!!
										//{
										//	if( sceneOcclusionCullingBuffer != null && EngineApp._DebugCapsLock )
										//	{

										//		//!!!!
										//		if( radius < 5 )
										//		{

										//			//!!!!отсекать в группе объектов. только мелкие объекты?

										//			//!!!!как индексы


										//			Vector3F* pointsRelative = stackalloc Vector3F[ 8 ];
										//			for( int n = 0; n < 8; n++ )
										//				context.ConvertToRelative( ref points[ n ], out pointsRelative[ n ] );

										//			var m = context.OwnerCameraSettings.GetViewProjectionMatrixRelative();
										//			float* modelToClipMatrix = (float*)&m;

										//			//fixed( Vector3F* pPositions = positions2 )
										//			{
										//				fixed( int* pIndices = boxIndices )
										//				{

										//					var result = sceneOcclusionCullingBuffer.TestTriangles( (float*)pointsRelative, (uint*)pIndices, boxIndices.Length / 3, modelToClipMatrix );

										//					//!!!!билборды тоже

										//					if( ( result & NeoAxis.OcclusionCullingBuffer.CullingResult.Occluded ) != 0 )
										//						return;
										//				}
										//			}
										//		}
										//	}
										//}



										var cascadeFrustumPlanes = cascadeFrustum.Planes;
										unsafe
										{
											fixed( Plane* pCascadeFrustumPlanes = cascadeFrustumPlanes )
											{
												if( Intersects( pCascadeFrustumPlanes, points, 8 ) )
													objectData.passed = 1;
											}
										}


										////viewportOwner.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
										////foreach( var p in points )
										////	viewportOwner.Simple3DRenderer.AddSphere( new Sphere( p, .1 ), 8, false, -1 );

										////var center = position + lightDirection.ToVector3() * ( shadowMapFarClipDistance / 2 );
										////var extents = new Vector3( shadowMapFarClipDistance * 0.5 + radius, radius, radius );
										////var box = new Box( center, extents, lightRotationMatrix );

										//////viewportOwner.Simple3DRenderer.SetColor( new ColorValue( 1, 0, 0 ) );
										//////viewportOwner.Simple3DRenderer.AddBox( box, false, -1 );

										////if( !cascadeFrustum.Intersects( box ) )
										////	continue;
										////}

									}

									Parallel.For( 0, objectsData.Count, Calculate );
									//for( int nObject = 0; nObject < objectsData.Length; nObject++ )
									//	Calculate( nObject );

									//process passed objects
									for( int nObject = 0; nObject < objectsData.Count; nObject++ )
									{
										unsafe
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
														ref var item = ref renderSceneDataMeshes.Data[ n ];
														if( item.CastShadows )
														{
															//!!!!толку в этом всём мало. да и глючит либа на больших объектах

															//!!!!достаточно геометрии которая к камере обращена
															//!!!!может где-то делается такая форма
															//!!!!threading
															//!!!!свойством, а то вдруг глючить будет


															//if( sceneOcclusionCullingBuffer != null && EngineApp._DebugCapsLock )
															//{
															//	SimpleMeshGenerator.GenerateBox( item.BoundingSphere.ToBounds(), out var positions, out var indices );

															//	var positions2 = new Vector3F[ positions.Length ];
															//	for( int n2 = 0; n2 < positions2.Length; n2++ )
															//		context.ConvertToRelative( positions[ n2 ], out positions2[ n2 ] );

															//	var m = context.OwnerCameraSettings.GetViewProjectionMatrixRelative();
															//	float* modelToClipMatrix = (float*)&m;

															//	fixed( Vector3F* pPositions = positions2 )
															//	{
															//		fixed( int* pIndices = indices )
															//		{

															//			var result = sceneOcclusionCullingBuffer.TestTriangles( (float*)pPositions, (uint*)pIndices, indices.Length / 3, modelToClipMatrix );

															//			//!!!!билборды тоже

															//			if( ( result & NeoAxis.OcclusionCullingBuffer.CullingResult.Occluded ) != 0 )
															//				continue;

															//		}
															//	}
															//}

															if( lightItem.shadowsAffectedRenderableGroups == null )
																lightItem.shadowsAffectedRenderableGroups = new OpenListNative<Vector2I>[ ShadowDirectionalLightCascades ];
															if( lightItem.shadowsAffectedRenderableGroups[ nIteration ] == null )
																lightItem.shadowsAffectedRenderableGroups[ nIteration ] = new OpenListNative<Vector2I>( getObjectsItem.Result.Length + 512 );
															lightItem.shadowsAffectedRenderableGroups[ nIteration ].Add( new Vector2I( 0, n ) );
														}
													}
													for( int n = data.BillboardRange.Minimum; n < data.BillboardRange.Maximum; n++ )
													{
														ref var item = ref renderSceneDataBillboards.Data[ n ];
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
					}
					else if( lightItem.data.Type == Light.TypeEnum.Point )
					{
						//Point light

						//!!!!new
						if( lightItem.prepareShadows )
						{
							var getObjectsItem = lightAffectedObjects[ lightIndex ].GetItem( 0 );
							//var sphere = new Sphere( lightItem.data.Position, lightItem.data.AttenuationFar );
							//var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );
							//scene.GetObjectsInSpace( getObjectsItem );

							//bool[] addFaces = new bool[ 6 ];

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
											ref var item = ref renderSceneDataMeshes.Data[ n ];
											ref var data2 = ref frameDataMeshes.Data[ n ];

											//if( ( data2.Flags & FrameData.MeshItem.FlagsEnum.CalculateAffectedLights ) != 0 )
											//	data2.AddPointSpotLight( lightIndex );

											//LightItem.shadowsAffectedRenderableGroups
											if( item.CastShadows )//&& lightItem.prepareShadows )
											{
												PointLightShadowGenerationCheckAddFaces( ref lightItem.data.Position, ref item.BoundingSphere, pointLightShadowGenerationAddFaces );

												for( int nFace = 0; nFace < 6; nFace++ )
												{
													if( pointLightShadowGenerationAddFaces[ nFace ] )
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
											ref var item = ref renderSceneDataBillboards.Data[ n ];
											ref var data2 = ref frameDataBillboards.Data[ n ];

											//if( ( data2.Flags & FrameData.BillboardItem.FlagsEnum.CalculateAffectedLights ) != 0 )
											//	data2.AddPointSpotLight( lightIndex );

											//LightItem.shadowsAffectedRenderableGroups
											if( item.CastShadows )//&& lightItem.prepareShadows )
											{
												//Sphere meshSphere = item.BoundingSphere;// meshInSpace.SpaceBounds.CalculatedBoundingSphere;
												PointLightShadowGenerationCheckAddFaces( ref lightItem.data.Position, ref item.BoundingSphere/*meshSphere*/, pointLightShadowGenerationAddFaces );

												for( int nFace = 0; nFace < 6; nFace++ )
												{
													if( pointLightShadowGenerationAddFaces[ nFace ] )
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
					}
					else if( lightItem.data.Type == Light.TypeEnum.Spotlight )
					{
						//Spotlight

						//!!!!new
						if( lightItem.prepareShadows )
						{
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
											ref var item = ref renderSceneDataMeshes.Data[ n ];
											ref var data2 = ref frameDataMeshes.Data[ n ];

											//if( ( data2.Flags & FrameData.MeshItem.FlagsEnum.CalculateAffectedLights ) != 0 )
											//	data2.AddPointSpotLight( lightIndex );

											//LightItem.shadowsAffectedRenderableGroups
											if( item.CastShadows )//&& lightItem.prepareShadows )
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
											ref var item = ref renderSceneDataBillboards.Data[ n ];
											ref var data2 = ref frameDataBillboards.Data[ n ];

											//if( ( data2.Flags & FrameData.BillboardItem.FlagsEnum.CalculateAffectedLights ) != 0 )
											//	data2.AddPointSpotLight( lightIndex );

											//LightItem.shadowsAffectedRenderableGroups
											if( item.CastShadows )//&& lightItem.prepareShadows )
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
			}

			//get objects for GI
			if( frameData.GIData != null )
			{

				//!!!!threading. split to sectors or threading inside GetObjectsInSpace

				var giData = frameData.GIData;
				var cameraSettings = viewportOwner.CameraSettings;

				//!!!!sphere shape?

				//!!!!cascades
				var cascade = 0;

				//!!!!cascades partial update

				var cascadeItem = giData.Cascades[ cascade ];

				var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, cascadeItem.BoundsWorld );
				getObjectsItem.GetFromOctree = Scene.SceneObjectFlags.Visual;

				//var r = frameData.IndirectLighting.Radius;
				//var bounds = new Bounds( cameraSettings.Position - new Vector3( r, r, r ), cameraSettings.Position + new Vector3( r, r, r ) );

				//var sphere = new Sphere( cameraSettings.Position, frameData.IndirectLighting.Radius );
				//var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );

				scene.GetObjectsInSpace( getObjectsItem );

				for( int nObject = 0; nObject < getObjectsItem.Result.Length; nObject++ )
				{
					var obj = getObjectsItem.Result[ nObject ].Object;
					if( obj.VisibleInHierarchy )//!!!!? && obj.SpaceBounds.CalculatedBoundingSphere.Intersects( ref sphere ) )
					{
						//!!!!как еще отсечь?

						var objIndex = frameData.RegisterObjectInSpace( context, obj, GetRenderSceneDataMode.GlobalIllumination, getObjectsItem );
					}
				}
			}

			////get objects for GI
			//if( frameData.IndirectLightingFrameData != null )
			//{
			//	var indirectFrameData = frameData.IndirectLightingFrameData;

			//	//!!!!threading. split to sectors or threading inside GetObjectsInSpace

			//	var cameraSettings = viewportOwner.CameraSettings;

			//	//!!!!sphere shape?

			//	//!!!!levels. partial update

			//	var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, indirectFrameData.TotalGridBounds );
			//	getObjectsItem.GetFromOctree = Scene.SceneObjectFlags.Visual;

			//	//var r = frameData.IndirectLighting.Radius;
			//	//var bounds = new Bounds( cameraSettings.Position - new Vector3( r, r, r ), cameraSettings.Position + new Vector3( r, r, r ) );

			//	//var sphere = new Sphere( cameraSettings.Position, frameData.IndirectLighting.Radius );
			//	//var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );

			//	scene.GetObjectsInSpace( getObjectsItem );

			//	for( int nObject = 0; nObject < getObjectsItem.Result.Length; nObject++ )
			//	{
			//		var obj = getObjectsItem.Result[ nObject ].Object;
			//		if( obj.VisibleInHierarchy )//!!!!? && obj.SpaceBounds.CalculatedBoundingSphere.Intersects( ref sphere ) )
			//		{
			//			//!!!!как еще отсечь?

			//			var objIndex = frameData.RegisterObjectInSpace( context, obj, GetRenderSceneDataMode.GlobalIllumination, getObjectsItem );
			//		}
			//	}
			//}

			//All data are prepared for rendering

			context.UpdateStatisticsCurrent.Lights = frameData.LightsInFrustumSorted.Length;
			context.UpdateStatisticsCurrent.ReflectionProbes = frameData.ReflectionProbes.Count;

			//sort. from biggest radius to smallest
			CollectionUtility.MergeSort( frameData.ReflectionProbes, ( x, y ) => y.data.Sphere.Radius.CompareTo( x.data.Sphere.Radius ) );
			//frameData.ReflectionProbes.Sort( ( x, y ) => y.data.Sphere.Radius.CompareTo( x.data.Sphere.Radius ) );
		}

		[MethodImpl( (MethodImplOptions)512 )]
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		bool ContainsDisposedVertexIndexBuffers( RenderSceneData.MeshDataRenderOperation op )
		{
			for( int n = 0; n < op.VertexBuffers.Length; n++ )
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		void SetVertexIndexBuffers( ViewportRenderingContext context, RenderSceneData.MeshDataRenderOperation op, GpuMaterialPass pass, ViewportRenderingContext.TessellationCacheItem tessellationItem, out int outputTriangleCount )
		{
			var indexStartOffset = op.IndexStartOffset;
			var indexTo = op.IndexCount - indexStartOffset;

			//clamp indexes range by material index of multi material
			var materialData = pass.Owner;
			if( materialData != null )
			{
				if( materialData.specialMode == Material.CompiledMaterialData.SpecialMode.MultiMaterialSeparatePass )
				{
					var materialIndexRanges = op.GetMaterialIndexRangesFromVertexDataOrFromVirtualizedData();
					if( materialIndexRanges != null && materialData.multiSubMaterialSeparatePassIndex < materialIndexRanges.Length )
					{
						var range = materialIndexRanges[ materialData.multiSubMaterialSeparatePassIndex ];

						if( range.Minimum > indexStartOffset )
							indexStartOffset = range.Minimum;
						if( range.Maximum < indexTo )
							indexTo = range.Maximum;
					}
				}
				else if( materialData.specialMode == Material.CompiledMaterialData.SpecialMode.MultiMaterialCombinedPass )
				{
					var materialIndexRanges = op.GetMaterialIndexRangesFromVertexDataOrFromVirtualizedData();
					if( materialIndexRanges != null )
					{
						var indexFirst = materialData.multiMaterialStartIndexOfCombinedGroup;
						var indexLast = indexFirst + materialData.multiMaterialReferencedSeparateMaterialsOfCombinedGroup.Length - 1;

						if( indexFirst < materialIndexRanges.Length && indexLast < materialIndexRanges.Length )
						{
							RangeI range;
							range.Minimum = materialIndexRanges[ indexFirst ].Minimum;
							range.Maximum = materialIndexRanges[ indexLast ].Maximum;

							if( range.Minimum > indexStartOffset )
								indexStartOffset = range.Minimum;
							if( range.Maximum < indexTo )
								indexTo = range.Maximum;
						}
					}
				}
			}

			if( tessellationItem != null )
			{
				context.SetVertexBuffer( 0, tessellationItem.VertexBuffer );
				context.SetIndexBuffer( tessellationItem.IndexBuffer );
				outputTriangleCount = tessellationItem.IndexBuffer.IndexCount / 3;
			}
			else
			{
				for( int n = 0; n < op.VertexBuffers.Length; n++ )
					context.SetVertexBuffer( n, op.VertexBuffers[ n ], op.VertexStartOffset, op.VertexCount );

				var indexBuffer = op.IndexBuffer;
				if( indexBuffer != null )
				{
					var indexCount = indexTo - indexStartOffset;
					context.SetIndexBuffer( indexBuffer, indexStartOffset, indexCount );

					outputTriangleCount = indexCount / 3;
				}
				else
					outputTriangleCount = op.VertexCount / 3;
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void RenderOperation( ViewportRenderingContext context, RenderSceneData.MeshDataRenderOperation op, GpuMaterialPass pass, List<ParameterContainer> parameterContainers, RenderSceneData.CutVolumeItem[] cutVolumes, bool instancingEnabled, GpuVertexBuffer instancingBuffer, ref InstanceDataBuffer instancingDataBuffer, int instancingStart, int instancingCount, ViewportRenderingContext.TessellationCacheItem tessellationItem = null )
		{
			if( ContainsDisposedVertexIndexBuffers( op ) )
				return;
			//if( instancingBuffer != null && instancingBuffer.Disposed )
			//	return;

			if( !context.DebugDrawMeshes && op.VoxelDataInfo == null )
				return;
			if( !context.DebugDrawVoxels && op.VoxelDataInfo != null )
				return;
			//if( !DebugDrawBatchedData && instancingBuffer != null )
			//	return;
			//if( !DebugDrawNotBatchedData && instancingBuffer == null )
			//	return;

			//!!!!можно склеить PInvoke вызовы

			SetCutVolumeSettingsUniforms( context, cutVolumes, false );
			SetVertexIndexBuffers( context, op, pass, tessellationItem, out var triangleCount );

			var statistics = context.updateStatisticsCurrent;

			if( instancingEnabled )
			{
				if( instancingBuffer != null )
				{
					if( instancingBuffer.Disposed )
						return;
					if( !context.DebugDrawBatchedData )
						return;

					var instancingCount2 = instancingCount;
					if( instancingCount2 == -1 )
						instancingCount2 = instancingBuffer.VertexCount - instancingStart;
					context.SetInstanceDataBuffer( instancingBuffer, instancingStart, instancingCount2 );
					context.SetPassAndSubmit( pass, RenderOperationType.TriangleList, parameterContainers, null, op.VoxelDataInfo != null, false );
					statistics.Triangles += triangleCount * instancingCount2;
					statistics.Instances += instancingCount2;
				}
				else
				{
					if( !context.DebugDrawNotBatchedData )
						return;

					var instancingCount2 = instancingCount;
					if( instancingCount2 == -1 )
						instancingCount2 = instancingDataBuffer.Size - instancingStart;
					Bgfx.SetInstanceDataBuffer( ref instancingDataBuffer, instancingStart, instancingCount2 );// instancingCount );
					context.SetPassAndSubmit( pass, RenderOperationType.TriangleList, parameterContainers, null, op.VoxelDataInfo != null, false );
					statistics.Triangles += triangleCount * instancingCount2;
					statistics.Instances += instancingCount2;
				}
			}
			else
			{
				if( !context.DebugDrawNotBatchedData )
					return;

				context.SetPassAndSubmit( pass, RenderOperationType.TriangleList, parameterContainers, null, op.VoxelDataInfo != null, false );
				statistics.Triangles += triangleCount;
				statistics.Instances++;
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void RenderOperation( ViewportRenderingContext context, RenderSceneData.MeshDataRenderOperation op, GpuMaterialPass pass, List<ParameterContainer> parameterContainers, RenderSceneData.CutVolumeItem[] cutVolumes = null, ViewportRenderingContext.TessellationCacheItem tessellationItem = null )
		{
			if( ContainsDisposedVertexIndexBuffers( op ) )
				return;

			if( !context.DebugDrawMeshes && op.VoxelDataInfo == null )
				return;
			if( !context.DebugDrawVoxels && op.VoxelDataInfo != null )
				return;
			if( !context.DebugDrawNotBatchedData )
				return;

			//!!!!можно склеить PInvoke вызовы

			SetCutVolumeSettingsUniforms( context, cutVolumes, false );
			SetVertexIndexBuffers( context, op, pass, tessellationItem, out var triangleCount );

			context.SetPassAndSubmit( pass, RenderOperationType.TriangleList, parameterContainers, null, op.VoxelDataInfo != null, false );
			var statistics = context.updateStatisticsCurrent;
			statistics.Triangles += triangleCount;
			statistics.Instances++;

			//var instancingDataBuffer = new InstanceDataBuffer();
			//RenderOperation( context, op, pass, parameterContainers, cutVolumes, false, null, ref instancingDataBuffer, 0, -1 );
		}


		//public void RenderOperation( ViewportRenderingContext context, RenderSceneData.MeshDataRenderOperation op, GpuMaterialPass pass, List<ParameterContainer> parameterContainers, RenderSceneData.CutVolumeItem[] cutVolumes = null, GpuVertexBuffer instanceBuffer = null, int instanceStart = 0, int instanceCount = -1 )
		////public void RenderOperation( ViewportRenderingContext context, RenderSceneData.MeshDataRenderOperation op, GpuMaterialPass pass, List<ParameterContainer> parameterContainers, RenderSceneData.CutVolumeItem[] cutVolumes = null, GpuVertexBuffer instanceBuffer = null, int instanceCount = -1 )
		//{
		//	if( ContainsDisposedVertexIndexBuffers( op ) )
		//		return;
		//	if( instanceBuffer != null && instanceBuffer.Disposed )
		//		return;

		//	if( !DebugDrawMeshes && op.VoxelDataInfo == null )
		//		return;
		//	if( !DebugDrawVoxels && op.VoxelDataInfo != null )
		//		return;

		//	//!!!!можно склеить PInvoke вызовы

		//	SetCutVolumeSettingsUniforms( context, cutVolumes, false );
		//	SetVertexIndexBuffers( context, op, pass, out var triangleCount );

		//	if( instanceBuffer != null )
		//	{
		//		var instanceCount2 = instanceCount;
		//		if( instanceCount2 == -1 )
		//			instanceCount2 = instanceBuffer.VertexCount - instanceStart;
		//		context.SetInstanceDataBuffer( instanceBuffer, instanceStart, instanceCount2 );
		//		context.SetPassAndSubmit( pass, RenderOperationType.TriangleList, parameterContainers, null, op.VoxelDataInfo != null );
		//		context.UpdateStatisticsCurrent.Triangles += triangleCount * instanceCount2;
		//		//context.UpdateStatisticsCurrent.Triangles += ( op.IndexBuffer != null ? op.IndexCount / 3 : op.VertexCount / 3 ) * instanceCount2;
		//	}
		//	else
		//	{
		//		context.SetPassAndSubmit( pass, RenderOperationType.TriangleList, parameterContainers, null, op.VoxelDataInfo != null );
		//		context.UpdateStatisticsCurrent.Triangles += triangleCount;
		//	}
		//}

		//[MethodImpl( (MethodImplOptions)512 )]
		//public void RenderOperation( ViewportRenderingContext context, RenderSceneData.MeshDataRenderOperation op, GpuMaterialPass pass, List<ParameterContainer> parameterContainers, RenderSceneData.CutVolumeItem[] cutVolumes, ref InstanceDataBuffer instanceBuffer, int instanceStart, int instanceCount )
		//{
		//	if( ContainsDisposedVertexIndexBuffers( op ) )
		//		return;

		//	if( !DebugDrawMeshes && op.VoxelDataInfo == null )
		//		return;
		//	if( !DebugDrawVoxels && op.VoxelDataInfo != null )
		//		return;

		//	//!!!!можно склеить PInvoke вызовы

		//	SetCutVolumeSettingsUniforms( context, cutVolumes, false );
		//	SetVertexIndexBuffers( context, op, pass, out var triangleCount );

		//	Bgfx.SetInstanceDataBuffer( ref instanceBuffer, instanceStart, instanceCount );
		//	context.SetPassAndSubmit( pass, RenderOperationType.TriangleList, parameterContainers, null, op.VoxelDataInfo != null );
		//	context.UpdateStatisticsCurrent.Triangles += triangleCount * instanceCount;
		//}

		////public void RenderOperation( ViewportRenderingContext context, RenderSceneData.MeshDataRenderOperation op, GpuMaterialPass pass, List<ParameterContainer> parameterContainers, GpuVertexBuffer instanceBuffer, int instanceCount = -1 )
		////{
		////	if( !SetVertexIndexBuffers( context, op ) )
		////		return;

		////	var instanceCount2 = instanceCount;
		////	if( instanceCount2 == -1 )
		////		instanceCount2 = instanceBuffer.VertexCount;

		////	context.SetInstanceDataBuffer( instanceBuffer, 0, instanceCount2 );
		////	context.SetPassAndSubmit( pass, RenderOperationType.TriangleList, parameterContainers );
		////	context.UpdateStatisticsCurrent.Triangles += ( op.IndexBuffer != null ? op.IndexCount / 3 : op.VertexCount / 3 ) * instanceCount2;
		////}

		[MethodImpl( (MethodImplOptions)512 )]
		static Material.CompiledMaterialData[] GetCommonMaterialData( /*ViewportRenderingContext context, */Material.CompiledMaterialData materialData, bool materialDataMustBePrepared, bool deferredShading, bool gi )
		{
			//null material
			if( materialData == null )
				return new Material.CompiledMaterialData[] { ResourceUtility.MaterialNull.Result };

			var output = materialData.GetOutputMaterials( deferredShading, gi );// getForwardRenderingData ? false : context.DeferredShading );

			var changed = false;
			for( int n = 0; n < output.Length; n++ )
			{
				var materialData2 = output[ n ];

				//check for updated during rendering frame
				if( materialData2 != null && materialDataMustBePrepared && materialData2.currentFrameIndex == -1 )
				{
					changed = true;
					break;
				}

				//invalid material
				if( materialData2.error != null ) //if( !string.IsNullOrEmpty( materialData2.error ) )
				{
					changed = true;
					break;
				}
			}

			if( changed )
			{
				var newOutput = new Material.CompiledMaterialData[ output.Length ];

				for( int n = 0; n < output.Length; n++ )
				{
					var materialData2 = output[ n ];

					//check for updated during rendering frame
					if( materialData2 != null && materialDataMustBePrepared && materialData2.currentFrameIndex == -1 )
						materialData2 = null;

					//null material
					if( materialData2 == null )
						materialData2 = ResourceUtility.MaterialNull.Result;

					//invalid material
					if( materialData2.error != null )//if( !string.IsNullOrEmpty( materialData2.error ) )
						materialData2 = ResourceUtility.MaterialInvalid.Result;

					newOutput[ n ] = materialData2;
				}

				return newOutput;
			}
			else
				return output;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static Material GetMeshMaterial2( ref RenderSceneData.MeshItem meshItem, RenderSceneData.MeshDataRenderOperation operation, int operationIndex )
		{
			//if( meshItem.ReplaceMaterialSelectively != null && operationIndex < meshItem.ReplaceMaterialSelectively.Length )
			//{
			//	var m = meshItem.ReplaceMaterialSelectively[ operationIndex ];
			//	if( m != null )
			//		return m;
			//}

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

		//public static void FixInvalidMaterialData( ref Material.CompiledMaterialData materialData, bool materialDataMustBePrepared )
		//{
		//	//check for updated during rendering frame
		//	if( materialData != null && materialDataMustBePrepared && materialData.currentFrameIndex == -1 )
		//		materialData = null;

		//	//null material
		//	if( materialData == null )
		//		materialData = ResourceUtility.MaterialNull.Result;

		//	//invalid material
		//	if( !string.IsNullOrEmpty( materialData.error ) )
		//		materialData = ResourceUtility.MaterialInvalid.Result;
		//}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Material.CompiledMaterialData[] GetMeshMaterialData( /*ViewportRenderingContext context, */ref RenderSceneData.MeshItem meshItem, RenderSceneData.MeshDataRenderOperation operation, int operationIndex, bool materialDataMustBePrepared, bool deferredShading, bool gi = false )
		{
			var materialData = GetMeshMaterial2( ref meshItem, operation, operationIndex )?.Result;
			return GetCommonMaterialData( /*context, */materialData, materialDataMustBePrepared, deferredShading, gi );


			//var materialData = GetMeshMaterial2( ref meshItem, operation, operationIndex )?.Result;

			////check for updated during rendering frame
			//if( materialData != null && materialDataMustBePrepared && materialData.currentFrameIndex == -1 )
			//	materialData = null;

			////null material
			//if( materialData == null )
			//	materialData = ResourceUtility.MaterialNull.Result;

			////invalid material
			//if( !string.IsNullOrEmpty( materialData.error ) )
			//	materialData = ResourceUtility.MaterialInvalid.Result;

			//return materialData.GetOutputMaterials();
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static Material GetBillboardMaterial2( ref RenderSceneData.BillboardItem billboardItem )
		{
			return billboardItem.Material;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public static Material.CompiledMaterialData[] GetBillboardMaterialData( /*ViewportRenderingContext context, */ref RenderSceneData.BillboardItem billboardItem, bool materialDataMustBePrepared, bool deferredShading, bool gi = false )
		{
			var materialData = GetBillboardMaterial2( ref billboardItem )?.Result;
			return GetCommonMaterialData( /*context, */materialData, materialDataMustBePrepared, deferredShading, gi );

			////check for updated during rendering frame
			//if( materialData != null && materialDataMustBePrepared && materialData.currentFrameIndex == -1 )
			//	materialData = null;

			////null material
			//if( materialData == null )
			//	materialData = ResourceUtility.MaterialNull.Result;

			////invalid material
			//if( !string.IsNullOrEmpty( materialData.error ) )
			//	materialData = ResourceUtility.MaterialInvalid.Result;

			//return materialData.GetOutputMaterials();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static Material GetDecalMaterial2( ref RenderSceneData.DecalItem decalItem )
		{
			return decalItem.Material;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static Material.CompiledMaterialData[] GetDecalMaterialData( /*ViewportRenderingContext context, */ref RenderSceneData.DecalItem decalItem, bool materialDataMustBePrepared, bool deferredShading, bool gi = false )
		{
			var materialData = GetDecalMaterial2( ref decalItem )?.Result;
			return GetCommonMaterialData( /*context, */materialData, materialDataMustBePrepared, deferredShading, gi );

			////check for updated during rendering frame
			//if( materialData != null && materialDataMustBePrepared && materialData.currentFrameIndex == -1 )
			//	materialData = null;

			////null material
			//if( materialData == null )
			//	materialData = ResourceUtility.MaterialNull.Result;

			////invalid material
			//if( !string.IsNullOrEmpty( materialData.error ) )
			//	materialData = ResourceUtility.MaterialInvalid.Result;

			//return materialData.GetOutputMaterials();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static Material.CompiledMaterialData GetLayerMaterial2( ref RenderSceneData.LayerItem layerItem )
		{
			return layerItem.ResultMaterial;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static Material.CompiledMaterialData[] GetLayerMaterialData( /*ViewportRenderingContext context, */ref RenderSceneData.LayerItem layerItem, bool materialDataMustBePrepared, bool deferredShading, bool gi = false )
		{
			var materialData = GetLayerMaterial2( ref layerItem );//?.Result;

			//material data of the layer can be null
			if( materialData == null )
				return Array.Empty<Material.CompiledMaterialData>();

			return GetCommonMaterialData( /*context, */materialData, materialDataMustBePrepared, deferredShading, gi );

			////check for updated during rendering frame
			//if( materialData != null && materialDataMustBePrepared && materialData.currentFrameIndex == -1 )
			//	materialData = null;

			//////null material
			////if( materialData == null )
			////	materialData = ResourceUtility.MaterialNull.Result;

			////invalid material
			//if( materialData != null && !string.IsNullOrEmpty( materialData.error ) )
			//	materialData = ResourceUtility.MaterialInvalid.Result;

			//return materialData;
		}

		struct RenderableGroupWithDistance
		{
			public Vector2I RenderableGroup;
			public float DistanceSquared;
		}

		static ShadowTextureSizeEnum GetMin( ShadowTextureSizeEnum v1, ShadowTextureSizeEnum v2 )
		{
			return (ShadowTextureSizeEnum)Math.Min( (int)v1, (int)v2 );
		}

		static int GetShadowTextureSizeEnumAsInteger( ShadowTextureSizeEnum value )
		{
			return 128 << (int)value;
		}

		[StructLayout( LayoutKind.Sequential )]
		struct PrepareShadowsSettingsUniform
		{
			public Vector3F cameraPosition;
			public float startDistance;
			public float farClipDistance;
			public float nearClipDistance;
			public float shadowMaterialOpacityMaskThresholdFactor;
			public float shadowTexelOffset;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		int GetShadowMapSize( LightItem lightItem )
		{
			var lightData = lightItem.data;

			int textureSize;

			ShadowTextureSizeEnum textureSizeEnum;
			if( lightData.ShadowTextureSize == Light.ShadowTextureSizeType.Value )
			{
				if( lightData.Type == Light.TypeEnum.Spotlight )
					textureSizeEnum = GetMin( lightData.ShadowTextureSizeValue, RenderingSystem.ShadowMaxTextureSizeSpotLight );
				else if( lightData.Type == Light.TypeEnum.Point )
					textureSizeEnum = GetMin( lightData.ShadowTextureSizeValue, RenderingSystem.ShadowMaxTextureSizePointLight );
				else
					textureSizeEnum = GetMin( lightData.ShadowTextureSizeValue, RenderingSystem.ShadowMaxTextureSizeDirectionalLight );
			}
			else
			{
				if( lightData.Type == Light.TypeEnum.Spotlight )
					textureSizeEnum = GetMin( ShadowSpotlightTextureSize.Value, RenderingSystem.ShadowMaxTextureSizeSpotLight );
				else if( lightData.Type == Light.TypeEnum.Point )
					textureSizeEnum = GetMin( ShadowPointLightTextureSize.Value, RenderingSystem.ShadowMaxTextureSizePointLight );
				else
					textureSizeEnum = GetMin( ShadowDirectionalLightTextureSize.Value, RenderingSystem.ShadowMaxTextureSizeDirectionalLight );
			}

			textureSize = GetShadowTextureSizeEnumAsInteger( textureSizeEnum );
			//textureSize = int.Parse( textureSizeEnum.ToString().Replace( "_", "" ) );

			if( lightData.ShadowTextureSize != Light.ShadowTextureSizeType.Default )
			{
				if( lightData.ShadowTextureSize == Light.ShadowTextureSizeType.x2 )
					textureSize *= 2;
				else if( lightData.ShadowTextureSize == Light.ShadowTextureSizeType.x4 )
					textureSize *= 4;
				else if( lightData.ShadowTextureSize == Light.ShadowTextureSizeType.Half )
					textureSize /= 2;
				else if( lightData.ShadowTextureSize == Light.ShadowTextureSizeType.Quarter )
					textureSize /= 4;
			}

			if( GlobalShadowQuality != 1 )
				textureSize = MathEx.NextPowerOfTwo( (int)( textureSize * GlobalShadowQuality ) );

			//!!!!new
			{
				if( lightData.Type == Light.TypeEnum.Spotlight )
					textureSize = Math.Min( textureSize, GetShadowMapSizeMultiLightSpot() );
				else if( lightData.Type == Light.TypeEnum.Point )
					textureSize = Math.Min( textureSize, GetShadowMapSizeMultiLightPoint() );
				//else
			}

			if( textureSize > RenderingSystem.Capabilities.MaxTextureSize )
				textureSize = RenderingSystem.Capabilities.MaxTextureSize;

			return textureSize;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		int GetShadowMapSizeMultiLightSpot()
		{
			var textureSize = GetShadowTextureSizeEnumAsInteger( ShadowSpotlightTextureSize );

			if( GlobalShadowQuality != 1 )
				textureSize = MathEx.NextPowerOfTwo( (int)( textureSize * GlobalShadowQuality ) );

			if( textureSize > RenderingSystem.Capabilities.MaxTextureSize )
				textureSize = RenderingSystem.Capabilities.MaxTextureSize;

			return textureSize;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		int GetShadowMapSizeMultiLightPoint()
		{
			var textureSize = GetShadowTextureSizeEnumAsInteger( ShadowPointLightTextureSize );

			if( GlobalShadowQuality != 1 )
				textureSize = MathEx.NextPowerOfTwo( (int)( textureSize * GlobalShadowQuality ) );

			if( textureSize > RenderingSystem.Capabilities.MaxTextureSize )
				textureSize = RenderingSystem.Capabilities.MaxTextureSize;

			return textureSize;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		static ViewportRenderingContext.LightStaticShadowsItem GetStaticShadowsFreeItem( Dictionary<int, Stack<ViewportRenderingContext.LightStaticShadowsItem>> staticShadowsFreeItems, int key )
		{
			if( staticShadowsFreeItems.TryGetValue( key, out var stack ) )
			{
				if( stack.Count > 0 )
					return stack.Pop();
			}
			return null;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe ImageComponent LightPrepareShadows( ViewportRenderingContext context, FrameData frameData, LightItem lightItem, ViewportRenderingContext.LightStaticShadowsItem lightStaticShadowsItem, bool needStaticShadows, Dictionary<int, Stack<ViewportRenderingContext.LightStaticShadowsItem>> staticShadowsFreeItems, ImageComponent overrideShadowTexture )//, ref int staticShadowsLimit )
		{
			var lightData = lightItem.data;

			var sectorsByDistance = context.SectorsByDistance;// SectorsByDistance.Value;
			if( lightData.Type == Light.TypeEnum.Spotlight || lightData.Type == Light.TypeEnum.Point )
			{
				if( sectorsByDistance > 2 )
					sectorsByDistance = 2;
			}

			var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
			var frameDataRenderSceneDataBillboards = frameData.RenderSceneData.Billboards;
			var staticShadowsMode = lightStaticShadowsItem != null;

			//var useEVSM = RenderingSystem.ShadowTechnique == ProjectSettingsPage_Rendering.ShadowTechniqueEnum.ExponentialVarianceShadowMaps;

			//init default shadow caster
			InitDefaultShadowCasterData();

			int textureSize = GetShadowMapSize( lightItem );
			var textureFormat = RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4 ? PixelFormat.A8R8G8B8 : PixelFormat.Float32R;
			//var textureFormat = /*useEVSM ? PixelFormat.Float32GR :*/ PixelFormat.Float32R;

			//create render target
			ImageComponent shadowTexture = overrideShadowTexture;
			if( shadowTexture == null )
			{
				if( staticShadowsMode )
				{
					var isPointLight = lightData.Type == Light.TypeEnum.Point;
					var staticShadowsFreeItemsKey = textureSize * ( isPointLight ? 1 : -1 );

					var staticShadowsFreeItem = GetStaticShadowsFreeItem( staticShadowsFreeItems, staticShadowsFreeItemsKey );
					if( staticShadowsFreeItem != null )
					{
						shadowTexture = staticShadowsFreeItem.Image;
					}
					else
					{
						var texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
						texture.CreateType = ImageComponent.TypeEnum._2D;
						texture.CreateSize = new Vector2I( textureSize, textureSize );
						texture.CreateArrayLayers = lightData.Type == Light.TypeEnum.Point ? 6 : 1;
						texture.CreateMipmaps = false;
						texture.CreateFormat = textureFormat;
						texture.CreateUsage = ImageComponent.Usages.RenderTarget;
						texture.Enabled = true;

						int faces = lightData.Type == Light.TypeEnum.Point ? 6 : 1;
						for( int face = 0; face < faces; face++ )
						{
							RenderTexture renderTexture = texture.Result.GetRenderTarget( 0, face );
							var viewport = renderTexture.AddViewport( false, false );

							viewport.RenderingPipelineCreate();
							viewport.RenderingPipelineCreated.UseRenderTargets = false;
						}

						shadowTexture = texture;
					}
				}
				else
				{
					if( lightData.Type == Light.TypeEnum.Point )
						shadowTexture = context.RenderTarget2D_Alloc( new Vector2I( textureSize, textureSize ), textureFormat, arrayLayers: 6 );
					else if( lightData.Type == Light.TypeEnum.Spotlight )
						shadowTexture = context.RenderTarget2D_Alloc( new Vector2I( textureSize, textureSize ), textureFormat );
					else if( lightData.Type == Light.TypeEnum.Directional )
					{
						frameData.ShadowTextureArrayDirectionalUsedForShadows = ShadowDirectionalLightCascades.Value;

						if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
						{
							//!!!!workaround for Android. 1 depth arrays are not arrays

							//also add slices for masks
							var count = ShadowDirectionalLightCascades.Value;
							count += GetAttachedToShadowMapsMaskCount( frameData, Light.TypeEnum.Directional );

							shadowTexture = context.RenderTarget2D_Alloc( new Vector2I( textureSize, textureSize ), textureFormat, arrayLayers: Math.Max( count, 2 ) );
						}
						else
						{
							shadowTexture = context.RenderTarget2D_Alloc( new Vector2I( textureSize, textureSize ), textureFormat, arrayLayers: ShadowDirectionalLightCascades );
						}
					}
				}
			}

			//create depth texture
			var shadowTextureDepth = context.RenderTarget2D_Alloc( new Vector2I( textureSize, textureSize ), RenderingSystem.DepthBuffer32Float ? PixelFormat.Depth32F : PixelFormat.Depth24S8 );

			if( lightData.Type == Light.TypeEnum.Directional )
				lightItem.shadowCascadesProjectionViewMatrices = new (Matrix4F, Matrix4F)[ ShadowDirectionalLightCascades ];

			int iterationCount = 1;
			if( lightData.Type == Light.TypeEnum.Point )
				iterationCount = 6;
			if( lightData.Type == Light.TypeEnum.Directional )
				iterationCount = ShadowDirectionalLightCascades;

			for( int nIteration = 0; nIteration < iterationCount; nIteration++ )
			{
				var shadowViewport = shadowTexture.Result.GetRenderTarget( 0, nIteration ).Viewports[ 0 ];

				//camera settings
				if( lightData.Type == Light.TypeEnum.Spotlight )
				{
					//spotlight
					Degree fov = lightData.SpotlightOuterAngle * 1.05;
					if( fov > 179 )
						fov = 179;
					Vector3 dir = lightData.Rotation.GetForward();
					Vector3 up = lightData.Rotation.GetUp();

					//!!!!context.Owner.CameraSettings.NearClipDistance?

					shadowViewport.CameraSettings = new Viewport.CameraSettingsClass( context.OwnerCameraSettingsPosition, shadowViewport, 1, fov, lightData.ShadowNearClipDistance/*context.Owner.CameraSettings.NearClipDistance*/, lightData.AttenuationFar * 1.05, lightData.Position, dir, up, ProjectionType.Perspective, 1, 0, 0 );
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

					shadowViewport.CameraSettings = new Viewport.CameraSettingsClass( context.OwnerCameraSettingsPosition, shadowViewport, 1, 90, lightData.ShadowNearClipDistance/*context.Owner.CameraSettings.NearClipDistance*/, lightData.AttenuationFar * 1.05, lightData.Position, dir, up, ProjectionType.Perspective, 1, 0, 0 );
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

					shadowViewport.CameraSettings = new Viewport.CameraSettingsClass( context.OwnerCameraSettingsPosition, shadowViewport, 1, 90, lightData.ShadowNearClipDistance/*shadowMapFarClipDistance / 1000.0*/ /*context.Owner.CameraSettings.NearClipDistance*/, shadowMapFarClipDistance, pos, dir, up, ProjectionType.Orthographic, orthoSize, 0, 0 );

					lightItem.shadowCascadesProjectionViewMatrices[ nIteration ] = (shadowViewport.CameraSettings.ProjectionMatrix, shadowViewport.CameraSettings.ViewMatrixRelative);
				}

				//set viewport
				var viewMatrix = shadowViewport.CameraSettings.ViewMatrixRelative;
				var projectionMatrix = shadowViewport.CameraSettings.ProjectionMatrix;

				//create target
				//!!!!call Free for mrt?
				var mrt = context.MultiRenderTarget_Create( new[] {
					new MultiRenderTarget.Item( shadowTexture, 0, nIteration ),
					new MultiRenderTarget.Item( shadowTextureDepth ) } );
				context.SetViewport( mrt.Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.All, new ColorValue( 1, 1, 1 ), RenderingSystem.ReversedZ ? 0 : 1 );
				//context.SetViewport( shadowViewport, viewMatrix, projectionMatrix, FrameBufferTypes.All, new ColorValue( 1, 1, 1 ) );


				////skip static shadows update because frame limit
				//if( staticShadowsMode && staticShadowsLimit <= 0 )
				//	continue;


				//bind textures for all render operations
				BindMaterialsTexture( context, frameData );
				BindBonesTexture( context, frameData );
				BindSamplersForTextureOnlySlots( context, true, false );
				BindMaterialData( context, null, false, false );


				//bind general parameters
				{
					var settings = new PrepareShadowsSettingsUniform();

					context.ConvertToRelative( shadowViewport.CameraSettings.Position, out settings.cameraPosition );
					//settings.cameraPosition = shadowViewport.CameraSettings.Position.ToVector3F();

					settings.startDistance = lightData.StartDistance;
					settings.farClipDistance = (float)shadowViewport.CameraSettings.FarClipDistance;
					settings.nearClipDistance = (float)shadowViewport.CameraSettings.NearClipDistance;
					settings.shadowMaterialOpacityMaskThresholdFactor = (float)ShadowMaterialOpacityMaskThresholdFactor.Value;
					settings.shadowTexelOffset = -0.5f / textureSize;

					if( !u_prepareShadowsSettings.HasValue )
						u_prepareShadowsSettings = GpuProgramManager.RegisterUniform( "u_prepareShadowsSettings", UniformType.Vector4, 2 );
					Bgfx.SetUniform( u_prepareShadowsSettings.Value, &settings, 2 );


					//var container = new ParameterContainer();

					////Mat4F worldViewProjMatrix = projectionMatrix * viewMatrix * worldMatrix;
					////Mat4F viewProjMatrix = projectionMatrix * viewMatrix;

					////!!!!
					//Vector2F texelOffsets = new Vector2F( -0.5f, -0.5f ) / textureSize;

					////!!!!
					////texelOffsets *= lightData.ShadowNormalBias;
					//////!!!!
					////texelOffsets = Vec2F.Zero;

					//container.Set( "u_shadowTexelOffsets", ParameterType.Vector2, 1, &texelOffsets, sizeof( Vector2F ) );
					////Vec2F texelOffsets = new Vec2F( -0.5f, -0.5f );
					////Vec4F texelOffsets2 = new Vec4F( texelOffsets.X, texelOffsets.Y, texelOffsets.X / textureSize, texelOffsets.Y / textureSize );
					////generalContainer.Set( "texelOffsets", ParameterType.Vector4, 1, &texelOffsets, sizeof( Vec4F ) );

					////!!!!double
					//Vector3F cameraPosition = shadowViewport.CameraSettings.Position.ToVector3F();
					//container.Set( "u_cameraPosition", ParameterType.Vector3, 1, &cameraPosition, sizeof( Vector3F ) );

					//float farClipDistance = (float)shadowViewport.CameraSettings.FarClipDistance;
					//container.Set( "u_farClipDistance", ParameterType.Float, 1, &farClipDistance, sizeof( float ) );

					//float nearClipDistance = (float)shadowViewport.CameraSettings.NearClipDistance;
					//container.Set( "u_nearClipDistance", ParameterType.Float, 1, &nearClipDistance, sizeof( float ) );

					//var maskFactor = (float)ShadowMaterialOpacityMaskThresholdFactor;
					//container.Set( "u_shadowMaterialOpacityMaskThresholdFactor", ParameterType.Float, 1, &maskFactor, sizeof( float ) );

					////generalContainer.Set( "u_shadowBias", ParameterType.Vector2, 1, &shadowBias, sizeof( Vector2F ) );
					////Vec2F bias;
					////if( lightItem.data.type == Light.TypeEnum.Spotlight )
					////	bias = shadowLightBiasSpotLight.ToVec2F();
					////else if( lightItem.data.type == Light.TypeEnum.Point )
					////	bias = shadowLightBiasPointLight.ToVec2F();
					////else
					////	bias = shadowLightBiasDirectionalLight.ToVec2F();
					////generalContainer.Set( "u_shadowBias", ParameterType.Vector2, 1, &bias, sizeof( Vec2F ) );

					//context.BindParameterContainer( container, false );
				}

				if( lightItem.shadowsAffectedRenderableGroups != null && lightItem.shadowsAffectedRenderableGroups[ nIteration ] != null )
				{
					var sourceList = lightItem.shadowsAffectedRenderableGroups[ nIteration ];

					using( var shadowsAffectedRenderableGroups = new OpenListNative<RenderableGroupWithDistance>( sourceList.Count ) )
					////clone
					//using( var shadowsAffectedRenderableGroups = lightItem.shadowsAffectedRenderableGroups[ nIteration ].Clone() )
					{
						Vector3 lightPosition;
						if( lightData.Type == Light.TypeEnum.Directional )
						{
							lightPosition = context.Owner.CameraSettings.Position - lightData.Rotation.ToQuaternion().GetForward() * 10000.0;
							//lightPosition = -lightItem.data.Rotation.ToQuaternion().GetForward() * 1000000.0;
						}
						else
							lightPosition = lightData.Position;


						for( int n = 0; n < sourceList.Count; n++ )
						{
							var item = new RenderableGroupWithDistance();
							item.RenderableGroup = sourceList.Data[ n ];
							item.DistanceSquared = frameData.GetObjectGroupDistanceToPointSquared( ref item.RenderableGroup, ref lightPosition );
							shadowsAffectedRenderableGroups.Add( ref item );
						}

						//sort by distance
						CollectionUtility.MergeSortUnmanaged( shadowsAffectedRenderableGroups.Data, shadowsAffectedRenderableGroups.Count, delegate ( RenderableGroupWithDistance* a, RenderableGroupWithDistance* b )
						{
							if( a->DistanceSquared < b->DistanceSquared )
								return -1;
							if( a->DistanceSquared > b->DistanceSquared )
								return 1;
							return 0;
						}, true );


						////var shadowsAffectedRenderableGroups = lightItem.shadowsAffectedRenderableGroups[ nIteration ].ToArray();
						////var shadowsAffectedRenderableGroups = lightItem.shadowsAffectedRenderableGroups[ nIteration ];

						////sort by distance
						//{
						//	Vector3 lightPosition;
						//	if( lightItem.data.Type == Light.TypeEnum.Directional )
						//	{
						//		lightPosition = context.Owner.CameraSettings.Position - lightItem.data.Rotation.ToQuaternion().GetForward() * 10000.0;
						//		//lightPosition = -lightItem.data.Rotation.ToQuaternion().GetForward() * 1000000.0;
						//	}
						//	else
						//		lightPosition = lightItem.data.Position;

						//	//!!!!maybe alloc native memory. где еще
						//	var meshDistances = new float[ frameData.Meshes.Count ];
						//	var billboardDistances = new float[ frameData.Billboards.Count ];
						//	foreach( var renderableGroup in shadowsAffectedRenderableGroups )
						//	{
						//		var index = renderableGroup;
						//		switch( renderableGroup.X )
						//		{
						//		case 0:
						//			meshDistances[ renderableGroup.Y ] = frameData.GetObjectGroupDistanceToPointSquared( ref index, ref lightPosition );
						//			break;
						//		case 1:
						//			billboardDistances[ renderableGroup.Y ] = frameData.GetObjectGroupDistanceToPointSquared( ref index, ref lightPosition );
						//			break;
						//		}
						//	}

						//	float GetDistanceSquared( ref Vector2I index )
						//	{
						//		if( index.X == 0 )
						//			return meshDistances[ index.Y ];
						//		else
						//			return billboardDistances[ index.Y ];
						//	}

						//	CollectionUtility.MergeSortUnmanaged( shadowsAffectedRenderableGroups.Data, shadowsAffectedRenderableGroups.Count, delegate ( Vector2I item1, Vector2I item2 )
						//	{
						//		var distanceSqr1 = GetDistanceSquared( ref item1 );
						//		var distanceSqr2 = GetDistanceSquared( ref item2 );
						//		if( distanceSqr1 < distanceSqr2 )
						//			return -1;
						//		if( distanceSqr1 > distanceSqr2 )
						//			return 1;
						//		return 0;
						//	}, true );
						//}


						//prepare outputInstancingManagers
						Parallel.For( 0, sectorsByDistance, delegate ( int nSector )
						{
							var manager = outputInstancingManagers[ nSector ];

							int indexFrom = (int)( (float)shadowsAffectedRenderableGroups.Count * nSector / sectorsByDistance );
							int indexTo = (int)( (float)shadowsAffectedRenderableGroups.Count * ( nSector + 1 ) / sectorsByDistance );
							if( nSector == sectorsByDistance - 1 )
								indexTo = shadowsAffectedRenderableGroups.Count;

							//fill output manager
							for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
							{
								var renderableGroup = shadowsAffectedRenderableGroups[ nRenderableGroup ];

								if( renderableGroup.RenderableGroup.X == 0 )
								{
									//meshes

									ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.RenderableGroup.Y ];

									RenderSceneData.IMeshData meshData;
									meshData = meshItem.MeshDataShadows ?? meshItem.MeshData;
									//if( staticShadowsMode && !meshItem.MeshDataShadowsForceBestLOD )
									//	meshData = meshItem.MeshDataLastVoxelLOD ?? meshItem.MeshDataShadows ?? meshItem.MeshData;
									//else
									//	meshData = meshItem.MeshDataShadows ?? meshItem.MeshData;
									////if( staticShadowsMode )
									////	meshData = meshItem.MeshDataLOD0 ?? meshItem.MeshData;
									////else
									////	meshData = meshItem.MeshDataShadows ?? meshItem.MeshData;

									for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
									{
										var oper = meshData.RenderOperations[ nOperation ];

										//get special shadow caster if available
										foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, context.DeferredShading, false ) )
										{
											var materialData2 = materialData;
											if( materialData2.specialShadowCasterData == null && !meshItem.Tessellation )
												materialData2 = ResourceUtility.MaterialNull.Result;

											//what else or apply before in the meshItem.StaticShadows
											var allowStaticShadows = meshItem.StaticShadows && materialData.staticShadows && meshItem.AnimationData == null;

											if( allowStaticShadows == staticShadowsMode || !needStaticShadows )// !context.StaticShadows )
											{
												bool instancing = Instancing && meshItem.AnimationData == null && !meshItem.InstancingEnabled && meshItem.CutVolumes == null && meshItem.ObjectInstanceParameters == null && !meshItem.Tessellation;
												manager.Add( renderableGroup.RenderableGroup, nOperation, oper, materialData2, instancing );
											}
										}
									}
								}
								else if( renderableGroup.RenderableGroup.X == 1 )
								{
									//billboards

									ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup.RenderableGroup.Y ];
									var meshData = Billboard.GetBillboardMesh().Result.MeshData;

									for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
									{
										var oper = meshData.RenderOperations[ nOperation ];

										foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, context.DeferredShading ) )
										{
											var materialData2 = materialData;
											if( materialData2.specialShadowCasterData == null )
												materialData2 = ResourceUtility.MaterialNull.Result;

											var allowStaticShadows = billboardItem.AllowStaticShadows && materialData.staticShadows;

											if( allowStaticShadows == staticShadowsMode || !needStaticShadows ) // !context.StaticShadows )
											{
												//!!!!или если уже собран из GroupOfObjects
												bool instancing = Instancing && billboardItem.CutVolumes == null && billboardItem.MaterialInstanceParameters == null;
												manager.Add( renderableGroup.RenderableGroup, nOperation, oper, materialData2, instancing );
											}
										}
									}
								}
							}

							manager.Prepare();
						} );

						//push to GPU
						for( int nSector = 0; nSector < sectorsByDistance; nSector++ )
						{
							var manager = outputInstancingManagers[ nSector ];

							int indexFrom = (int)( (float)shadowsAffectedRenderableGroups.Count * nSector / sectorsByDistance );
							int indexTo = (int)( (float)shadowsAffectedRenderableGroups.Count * ( nSector + 1 ) / sectorsByDistance );
							if( nSector == sectorsByDistance - 1 )
								indexTo = shadowsAffectedRenderableGroups.Count;

							//render output items
							{
								var outputItems = manager.outputItems;
								for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
								{
									ref var outputItem = ref outputItems.Data[ nOutputItem ];
									var materialData = outputItem.materialData;
									var outputItemoperation = outputItem.operation;
									var outputItemoperationVoxelDataInfo = outputItemoperation.VoxelDataInfo;
									var voxelRendering = outputItemoperationVoxelDataInfo != null;

									//get shadow caster data
									var specialShadowCasterData = materialData.specialShadowCasterData;
									ShadowCasterData shadowCasterData = specialShadowCasterData;
									if( shadowCasterData == null )
										shadowCasterData = defaultShadowCasterData;
									var passGroup = shadowCasterData.passByLightType[ (int)lightData.Type ];

									//render operation
									if( Instancing && outputItem.renderableItemsCount >= 2 )//InstancingMinCount )
									{
										//with instancing

										//bind material data
										if( specialShadowCasterData != null )
											BindMaterialData( context, materialData, true, voxelRendering );
										BindSamplersForTextureOnlySlots( context, false, voxelRendering );

										GpuMaterialPass pass = null;

										//bind operation data
										var firstRenderableItem = outputItem.renderableItemFirst;
										if( firstRenderableItem.X == 0 )
										{
											//meshes
											ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ firstRenderableItem.Y ];

											RenderSceneData.IMeshData meshData;
											meshData = meshItem.MeshDataShadows ?? meshItem.MeshData;
											//if( staticShadowsMode && !meshItem.MeshDataShadowsForceBestLOD )
											//	meshData = meshItem.MeshDataLastVoxelLOD ?? meshItem.MeshDataShadows ?? meshItem.MeshData;
											//else
											//	meshData = meshItem.MeshDataShadows ?? meshItem.MeshData;
											////if( staticShadowsMode )
											////	meshData = meshItem.MeshDataLOD0 ?? meshItem.MeshData;
											////else
											////	meshData = meshItem.MeshDataShadows ?? meshItem.MeshData;

											BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItemoperation.UnwrappedUV, ref meshItem.Color, outputItemoperation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, null, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

											pass = passGroup.Get( outputItemoperationVoxelDataInfo != null/*, outputItemoperation.VirtualizedData != null*/, meshData.BillboardMode != 0 );
										}
										else if( firstRenderableItem.X == 1 )
										{
											//billboards
											ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ firstRenderableItem.Y ];
											var meshData = Billboard.GetBillboardMesh().Result.MeshData;

											BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, outputItemoperation.UnwrappedUV, ref billboardItem.Color, outputItemoperation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, null, 0, ref vector3FZero );

											pass = passGroup.Billboard;
										}

										//no sense to use RenderSceneData.InstancingObjectData
										int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
										int instanceCount = outputItem.renderableItemsCount;

										var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
										if( instanceBuffer.Valid )
										{
											//get instancing matrices
											RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
											int currentMatrix = 0;
											for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
											{
												var renderableItem = outputItem.renderableItems[ nRenderableItem ];

												if( renderableItem.X == 0 )
												{
													//meshes
													ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
													meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
												}
												else if( renderableItem.X == 1 )
												{
													//billboards
													ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
													billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
												}
											}

											RenderOperation( context, outputItemoperation, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
										}
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
												BindMaterialData( context, materialData, true, voxelRendering );
											BindSamplersForTextureOnlySlots( context, false, voxelRendering );

											//bind render operation data, set matrix
											if( renderableItem.X == 0 )
											{
												//meshes
												ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];

												RenderSceneData.IMeshData meshData;
												meshData = meshItem.MeshDataShadows ?? meshItem.MeshData;
												//if( staticShadowsMode && !meshItem.MeshDataShadowsForceBestLOD )
												//	meshData = meshItem.MeshDataLastVoxelLOD ?? meshItem.MeshDataShadows ?? meshItem.MeshData;
												//else
												//	meshData = meshItem.MeshDataShadows ?? meshItem.MeshData;
												////if( staticShadowsMode )
												////	meshData = meshItem.MeshDataLOD0 ?? meshItem.MeshData;
												////else
												////	meshData = meshItem.MeshDataShadows ?? meshItem.MeshData;

												BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItemoperation.UnwrappedUV, ref meshItem.Color, outputItemoperation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, meshItem.ObjectInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

												if( !meshItem.InstancingEnabled )
													fixed( Matrix4F* p = &meshItem.TransformRelative )
														Bgfx.SetTransform( (float*)p );

												var pass = passGroup.Get( outputItemoperationVoxelDataInfo != null/*, outputItemoperation.VirtualizedData != null*/, meshData.BillboardMode != 0 );

												var tessItem = meshItem.Tessellation ? TessellationGetItem( context, outputItemoperation, materialData ) : null;

												RenderOperation( context, outputItemoperation, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount, tessItem );
											}
											else if( renderableItem.X == 1 )
											{
												//billboards
												ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
												var meshData = Billboard.GetBillboardMesh().Result.MeshData;

												BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, outputItemoperation.UnwrappedUV, ref billboardItem.Color, outputItemoperation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, billboardItem.MaterialInstanceParameters, 0, ref vector3FZero );

												billboardItem.GetWorldMatrixRelative( out var worldMatrix );
												Bgfx.SetTransform( (float*)&worldMatrix );

												var pass = passGroup.Billboard;

												RenderOperation( context, outputItemoperation, pass, null, billboardItem.CutVolumes );
											}
										}
									}
								}
							}
						}

						//clear outputInstancingManagers
						foreach( var manager in outputInstancingManagers )
							manager.Clear();
					}
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

			Bgfx.Discard( DiscardFlags.All );

			context.DynamicTexture_Free( shadowTextureDepth );

			//if( staticShadowsMode )
			//	staticShadowsLimit--;

			return shadowTexture;
		}

		protected unsafe virtual void RenderSky( ViewportRenderingContext context, FrameData frameData )
		{
			frameData.Sky?.Render( this, context, frameData );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected unsafe virtual void Render3DSceneDeferred( ViewportRenderingContext context, FrameData frameData )
		{
			var viewportOwner = context.Owner;
			var sectorsByDistance = context.SectorsByDistance;// SectorsByDistance.Value;

			var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
			var frameDataRenderSceneDataBillboards = frameData.RenderSceneData.Billboards;
			var frameDataMeshes = frameData.Meshes;
			var frameDataBillboards = frameData.Billboards;

			using( var renderableGroupsToDraw = new OpenListNative<RenderableGroupWithDistance>( frameData.RenderableGroupsInFrustum.Count ) )
			{
				foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
				{
					bool add = false;

					if( renderableGroup.X == 0 )
					{
						ref var data = ref frameDataMeshes.Data[ renderableGroup.Y ];
						if( ( data.Flags & FrameData.MeshItem.FlagsEnum.UseDeferred ) != 0 )
						{
							//!!!!need?
							ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];
							if( !meshItem.OnlyForShadowGeneration )
								add = true;
						}
						//add = ( data.Flags & FrameData.MeshItem.FlagsEnum.UseDeferred ) != 0;
					}
					else
					{
						ref var data = ref frameDataBillboards.Data[ renderableGroup.Y ];
						add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseDeferred ) != 0;
					}

					if( add )
					{
						var item = new RenderableGroupWithDistance();
						item.RenderableGroup = renderableGroup;
						item.DistanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.RenderableGroup );
						renderableGroupsToDraw.Add( ref item );
						//renderableGroupsToDraw.Add( renderableGroup );
					}
				}

				if( renderableGroupsToDraw.Count == 0 )
					return;

				//sort by distance
				CollectionUtility.MergeSortUnmanaged( renderableGroupsToDraw.Data, renderableGroupsToDraw.Count, delegate ( RenderableGroupWithDistance* a, RenderableGroupWithDistance* b )
				{
					if( a->DistanceSquared < b->DistanceSquared )
						return -1;
					if( a->DistanceSquared > b->DistanceSquared )
						return 1;
					return 0;
				}, true );


				//prepare outputInstancingManagers
				Parallel.For( 0, sectorsByDistance, delegate ( int nSector )
				{
					var manager = outputInstancingManagers[ nSector ];

					int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorsByDistance );
					int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorsByDistance );
					if( nSector == sectorsByDistance - 1 )
						indexTo = renderableGroupsToDraw.Count;

					//fill output manager
					for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
					{
						var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

						if( renderableGroup.RenderableGroup.X == 0 )
						{
							//meshes

							ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.RenderableGroup.Y ];
							var meshData = meshItem.MeshData;

							//if( !meshItem.OnlyForShadowGeneration )
							//{
							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
							{
								var oper = meshData.RenderOperations[ nOperation ];

								foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, true ) )
								{
									if( materialData.deferredShadingSupport )
									{
										bool instancing = Instancing && meshItem.AnimationData == null && !meshItem.InstancingEnabled && meshItem.CutVolumes == null && meshItem.ObjectInstanceParameters == null && !meshItem.Tessellation;
										manager.Add( renderableGroup.RenderableGroup, nOperation, oper, materialData, instancing );
									}
								}
							}
							//}
						}
						else if( renderableGroup.RenderableGroup.X == 1 )
						{
							//billboards

							ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup.RenderableGroup.Y ];
							var meshData = Billboard.GetBillboardMesh().Result.MeshData;

							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
							{
								var oper = meshData.RenderOperations[ nOperation ];

								foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, true ) )
								{
									if( materialData.deferredShadingSupport )
									{
										bool instancing = Instancing && billboardItem.CutVolumes == null && billboardItem.MaterialInstanceParameters == null;
										manager.Add( renderableGroup.RenderableGroup, nOperation, oper, materialData, instancing );
									}
								}
							}
						}
					}

					manager.Prepare();
				} );

				////!!!!temp
				//var sector = -1;
				//var mainViewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
				//if( mainViewport.IsKeyPressed( EKeys.P ) )
				//	sector = 0;
				//if( mainViewport.IsKeyPressed( EKeys.L ) )
				//	sector = 1;
				//if( mainViewport.IsKeyPressed( EKeys.O ) )
				//	sector = 2;
				//if( mainViewport.IsKeyPressed( EKeys.K ) )
				//	sector = 3;
				//if( mainViewport.IsKeyPressed( EKeys.M ) )
				//	sector = 4;
				//if( mainViewport.IsKeyPressed( EKeys.I ) )
				//	sector = 5;
				//if( mainViewport.IsKeyPressed( EKeys.J ) )
				//	sector = 6;
				//if( mainViewport.IsKeyPressed( EKeys.N ) )
				//	sector = 7;


				//bind textures for all render operations
				BindMaterialsTexture( context, frameData );
				BindBonesTexture( context, frameData );
				BindSamplersForTextureOnlySlots( context, true, false );
				BindMaterialData( context, null, false, false );

				//push to GPU
				for( int nSector = 0; nSector < sectorsByDistance; nSector++ )
				{
					////!!!!temp
					//if( sector != -1 && sector != nSector )
					//	continue;

					var manager = outputInstancingManagers[ nSector ];

					int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorsByDistance );
					int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorsByDistance );
					if( nSector == sectorsByDistance - 1 )
						indexTo = renderableGroupsToDraw.Count;

					//render output items
					{
						var outputItems = manager.outputItems;
						for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
						{
							ref var outputItem = ref outputItems.Data[ nOutputItem ];
							var materialData = outputItem.materialData;
							var outputItemoperation = outputItem.operation;
							var outputItemoperationVoxelDataInfo = outputItemoperation.VoxelDataInfo;
							var voxelRendering = outputItemoperationVoxelDataInfo != null;

							if( Instancing && outputItem.renderableItemsCount >= 2 )//InstancingMinCount )
							{
								//with instancing

								//bind material data
								BindMaterialData( context, materialData, false, voxelRendering );
								BindSamplersForTextureOnlySlots( context, false, voxelRendering );

								GpuMaterialPass pass = null;

								//bind operation data
								var firstRenderableItem = outputItem.renderableItemFirst;
								if( firstRenderableItem.X == 0 )
								{
									//meshes
									ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ firstRenderableItem.Y ];
									var meshData = meshItem.MeshData;

									BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItemoperation.UnwrappedUV, ref meshItem.Color, outputItemoperation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, null, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

									pass = materialData.deferredShadingPass.Get( outputItemoperationVoxelDataInfo != null/*, outputItemoperation.VirtualizedData != null*/, meshData.BillboardMode != 0 );
								}
								else if( firstRenderableItem.X == 1 )
								{
									//billboards
									ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ firstRenderableItem.Y ];
									var meshData = Billboard.GetBillboardMesh().Result.MeshData;

									BindRenderOperationData( context, frameData, materialData, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, outputItemoperation.UnwrappedUV, ref billboardItem.Color, outputItemoperation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, null, 0, ref vector3FZero );

									pass = materialData.deferredShadingPass.Billboard;
								}

								int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
								int instanceCount = outputItem.renderableItemsCount;

								var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
								if( instanceBuffer.Valid )
								{
									//get instancing matrices
									RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
									int currentMatrix = 0;
									for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
									{
										var renderableItem = outputItem.renderableItems[ nRenderableItem ];

										if( renderableItem.X == 0 )
										{
											//meshes
											ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
											//!!!!slowly because no threading? where else
											meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
										}
										else if( renderableItem.X == 1 )
										{
											//billboards
											ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
											billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
										}
									}

									RenderOperation( context, outputItemoperation, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
								}
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
									BindMaterialData( context, materialData, false, voxelRendering );
									BindSamplersForTextureOnlySlots( context, false, voxelRendering );

									//bind render operation data, set matrix
									if( renderableItem.X == 0 )
									{
										//meshes
										ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
										var meshData = meshItem.MeshData;


										////!!!!
										//var objectIdStart = 0;

										//if( indirectLightingFullModeData != null )
										//{
										//	var data = indirectLightingFullModeData;


										//	var voxelData = RenderingEffect_IndirectLighting.GetVoxelData( meshData );
										//	if( voxelData != null && voxelData.Length >= sizeof( MeshGeometry.VoxelDataHeader ) )
										//	{
										//		fixed( byte* pVoxelData = voxelData )
										//		{
										//			var header = new MeshGeometry.VoxelDataHeader();
										//			NativeUtility.CopyMemory( &header, pVoxelData, sizeof( MeshGeometry.VoxelDataHeader ) );


										//			var gridSize = RenderingEffect_IndirectLighting.GridSize;

										//			//!!!!slowly
										//			meshItem.Transform.Decompose( out var position, out QuaternionF rotation, out var scale );


										//			var emissive = scale.Equals( new Vector3F( 2, 2, 2 ), 0.01f );


										//			//var voxelCount = 0;
										//			//{
										//			//	fixed( byte* pVoxelData2 = voxelData )
										//			//	{
										//			//		voxelCount = ( (MeshGeometry.VoxelDataHeader*)pVoxelData2 )->VoxelCount;
										//			//	}
										//			//}


										//			float* pGrid = (float*)( pVoxelData + sizeof( MeshGeometry.VoxelDataHeader ) );

										//			for( int z = 0; z < header.GridSize.Z; z++ )
										//			{
										//				for( int y = 0; y < header.GridSize.Y; y++ )
										//				{
										//					for( int x = 0; x < header.GridSize.X; x++ )
										//					{
										//						var voxelGridIndex = new Vector3I( x, y, z );

										//						var arrayIndex = ( z * header.GridSize.Y + y ) * header.GridSize.X + x;

										//						var voxelValue = pGrid[ arrayIndex ];
										//						if( voxelValue > 0.0f )
										//						{
										//							var p = voxelGridIndex.ToVector3F() + new Vector3F( 0.5f, 0.5f, 0.5f );
										//							var p2 = header.BoundsMin + p * header.CellSize;
										//							var worldPosition = meshItem.Transform * p2;

										//							var sceneGridPosition = ( worldPosition - data.GridPosition ) / data.CellSize;
										//							var sceneGridIndex = sceneGridPosition.ToVector3I();

										//							if( sceneGridIndex.X >= 0 && sceneGridIndex.Y >= 0 && sceneGridIndex.Z >= 0 && sceneGridIndex.X < gridSize && sceneGridIndex.Y < gridSize && sceneGridIndex.Z < gridSize )
										//							{
										//								fixed( byte* pGridData = data.GridData )
										//								{
										//									float* pGridData2 = (float*)pGridData;

										//									float value = 0.01f;
										//									if( emissive )
										//									{
										//										value = 1;
										//										//if( arrayIndex % 2 == 0 )
										//										//	value = 0.5f;
										//									}

										//									pGridData2[ ( sceneGridIndex.Z * gridSize + sceneGridIndex.Y ) * gridSize + sceneGridIndex.X ] = value;
										//								}
										//							}

										//							//	fixed( byte* pGridData = data.GridData )
										//							//	{
										//							//		float* pGridData2 = (float*)pGridData;

										//							//		float value = 0;
										//							//		if( emissive )
										//							//			value = 1;

										//							//		pGridData2[ ( gridIndex.Z * gridSize + gridIndex.Y ) * gridSize + gridIndex.X ] = value;
										//							//	}


										//							//var gridIndex256 = MeshConvertToVoxel.GetNearestVoxelWithDataIndex256( ref gridIndex );
										//							//pList[ writeOffset++ ] = gridIndex256;
										//						}
										//					}
										//				}
										//			}


										//			//for( int n = 0; n < voxelCount; n++ )
										//			//{
										//			//	var voxelIndex = new Vector3I(  );

										//			//	var worldPosition = z;

										//			//	var gridPosition = z;

										//			//	var gridIndex = new Vector3I(  );


										//			//	fixed( byte* pGridData = data.GridData )
										//			//	{
										//			//		float* pGridData2 = (float*)pGridData;

										//			//		float value = 0;
										//			//		if( emissive )
										//			//			value = 1;

										//			//		z;
										//			//		pGridData2[ ( gridIndex.Z * gridSize + gridIndex.Y ) * gridSize + gridIndex.X ] = value;
										//			//	}
										//			//}

										//		}
										//	}
										//}



										////!!!!
										//var objectIdStart = 0;
										//if( indirectLightingFullModeData != null )
										//{
										//	var data = indirectLightingFullModeData;

										//	//var tr = obj.TransformV;

										//	var voxelData = RenderingEffect_IndirectLighting.GetVoxelData( meshData );
										//	if( voxelData != null )//&& RenderingEffect_IndirectLighting.ContainsGpuVoxelData( voxelData ) )
										//	{

										//		//meshData.SpaceBounds

										//		//!!!!
										//		if( !data.objectTypeOffsetByVoxelData.TryGetValue( voxelData, out var objectTypeIndex ) )
										//		{
										//			objectTypeIndex = data.objectTypes.Count / 4;

										//			var gpuVoxelData = RenderingEffect_IndirectLighting.GetGpuVoxelData( voxelData );
										//			data.objectTypes.AddRange( gpuVoxelData );

										//			data.objectTypeOffsetByVoxelData[ voxelData ] = objectTypeIndex;
										//		}
										//		//var objectTypeIndex = data.objectTypes.Count / 4;
										//		//data.objectTypes.AddRange( gpuVoxelData );


										//		var item = new RenderingEffect_IndirectLighting.GPUObjectData();

										//		//!!!!double

										//		//!!!!slowly
										//		meshItem.Transform.Decompose( out var position, out QuaternionF rotation, out var scale );
										//		//meshItem.Transform.Decompose( out item.Position, out item.Rotation, out item.Scale );

										//		//!!!!
										//		//meshItem.Transform.GetInverse( out item.ObjectTransform );
										//		//meshItem.Transform.GetInverse().GetTranspose( out item.ObjectTransform );
										//		meshItem.Transform.GetTranspose( out item.ObjectTransform );

										//		//item.ObjectTransform.Item0 = meshItem.Transform.Item0;
										//		//item.ObjectTransform.Item1 = meshItem.Transform.Item1;
										//		//item.ObjectTransform.Item2 = meshItem.Transform.Item2;

										//		//item.Position = tr.Position.ToVector3F();
										//		//item.Rotation = tr.Rotation.ToQuaternionF();
										//		//item.Scale = tr.Scale.ToVector3F();
										//		item.ObjectTypeOffset = objectTypeIndex;


										//		//!!!!


										//		item.RadianceOffset = data.directRadiance.Count;
										//		//item.RadianceOffset = data.radiance.Count / 4;

										//		var voxelCount = 0;
										//		{
										//			if( voxelData != null && voxelData.Length >= sizeof( MeshGeometry.VoxelDataHeader ) )
										//			{
										//				fixed( byte* pData2 = voxelData )
										//				{
										//					voxelCount = ( (MeshGeometry.VoxelDataHeader*)pData2 )->VoxelCount;
										//				}
										//			}
										//		}


										//		var radiaceData = new float[ voxelCount ];

										//		var emissive = scale.Equals( new Vector3F( 2, 2, 2 ), 0.01f );
										//		//var emissive = item.Scale.Equals( new Vector3F( 2, 2, 2 ), 0.01f );
										//		if( emissive )
										//		{
										//			for( int n = 0; n < voxelCount; n++ )
										//			{
										//				//if( ( n % 10 ) == 0 )
										//				radiaceData[ n ] = 1.0f;
										//			}

										//			//for( int n = 0; n < voxelCount; n++ )
										//			//	radiaceData[ n ] = 1.0f;


										//			//fixed( float* pRadiance = radiaceData )
										//			//{
										//			//	for( int n = 0; n < voxelCount; n++ )
										//			//		pRadiance[ n ] = 1.0f;
										//			//}
										//		}

										//		//var radiaceData = new byte[ voxelCount * 4 ];

										//		//var emissive = item.Scale.Equals( new Vector3F( 2, 2, 2 ), 0.01f );
										//		//if( emissive )
										//		//{
										//		//	fixed( byte* pRadiance = radiaceData )
										//		//	{
										//		//		float* pRadiance2 = (float*)pRadiance;

										//		//		for( int n = 0; n < voxelCount; n++ )
										//		//			pRadiance2[ n ] = 1.0f;
										//		//	}
										//		//}

										//		objectIdStart = data.objects.Count;


										//		data.directRadiance.AddRange( radiaceData );

										//		var objectIds = new float[ radiaceData.Length ];
										//		for( int n = 0; n < objectIds.Length; n++ )
										//			objectIds[ n ] = objectIdStart;
										//		data.radianceObjectIds.AddRange( objectIds );

										//		var localVoxelIndices = new float[ radiaceData.Length ];
										//		for( int n = 0; n < localVoxelIndices.Length; n++ )
										//			localVoxelIndices[ n ] = n;
										//		data.radianceLocalVoxelIndices.AddRange( localVoxelIndices );


										//		data.objects.Add( ref item );

										//		data.objectsLocalSpaceBounds.Add( meshData.SpaceBounds );

										//		//!!!!
										//		var tr = new Transform( position, rotation, scale );
										//		//var tr = new Transform( item.Position, item.Rotation, item.Scale );
										//		data.objectsWorldSpaceBounds.Add( SpaceBounds.Multiply( tr, meshData.SpaceBounds ) );
										//	}
										//}




										////!!!!
										//var objectIdStart = 0;
										//if( indirectLightingFullModeData != null )
										//{
										//	var data = indirectLightingFullModeData;

										//	//var tr = obj.TransformV;

										//	var voxelData = RenderingEffect_IndirectLighting.GetVoxelData( meshData );
										//	if( voxelData != null && RenderingEffect_IndirectLighting.ContainsGpuVoxelData( voxelData ) )
										//	{
										//		//meshData.SpaceBounds

										//		//!!!!
										//		if( !data.objectTypeOffsetByVoxelData.TryGetValue( voxelData, out var objectTypeIndex ) )
										//		{
										//			objectTypeIndex = data.objectTypes.Count / 4;

										//			var gpuVoxelData = RenderingEffect_IndirectLighting.GetGpuVoxelData( voxelData );
										//			data.objectTypes.AddRange( gpuVoxelData );

										//			data.objectTypeOffsetByVoxelData[ voxelData ] = objectTypeIndex;
										//		}
										//		//var objectTypeIndex = data.objectTypes.Count / 4;
										//		//data.objectTypes.AddRange( gpuVoxelData );


										//		var item = new RenderingEffect_IndirectLighting.GPUObjectData();

										//		//!!!!double

										//		//!!!!slowly
										//		meshItem.Transform.Decompose( out var position, out QuaternionF rotation, out var scale );
										//		//meshItem.Transform.Decompose( out item.Position, out item.Rotation, out item.Scale );

										//		//!!!!
										//		//meshItem.Transform.GetInverse( out item.ObjectTransform );
										//		//meshItem.Transform.GetInverse().GetTranspose( out item.ObjectTransform );
										//		meshItem.Transform.GetTranspose( out item.ObjectTransform );

										//		//item.ObjectTransform.Item0 = meshItem.Transform.Item0;
										//		//item.ObjectTransform.Item1 = meshItem.Transform.Item1;
										//		//item.ObjectTransform.Item2 = meshItem.Transform.Item2;

										//		//item.Position = tr.Position.ToVector3F();
										//		//item.Rotation = tr.Rotation.ToQuaternionF();
										//		//item.Scale = tr.Scale.ToVector3F();
										//		item.ObjectTypeOffset = objectTypeIndex;


										//		//!!!!


										//		item.RadianceOffset = data.directRadiance.Count;
										//		//item.RadianceOffset = data.radiance.Count / 4;

										//		var voxelCount = 0;
										//		{
										//			if( voxelData != null && voxelData.Length >= sizeof( MeshGeometry.VoxelDataHeader ) )
										//			{
										//				fixed( byte* pData2 = voxelData )
										//				{
										//					voxelCount = ( (MeshGeometry.VoxelDataHeader*)pData2 )->VoxelCount;
										//				}
										//			}
										//		}


										//		var radiaceData = new float[ voxelCount ];

										//		var emissive = scale.Equals( new Vector3F( 2, 2, 2 ), 0.01f );
										//		//var emissive = item.Scale.Equals( new Vector3F( 2, 2, 2 ), 0.01f );
										//		if( emissive )
										//		{
										//			for( int n = 0; n < voxelCount; n++ )
										//			{
										//				//if( ( n % 10 ) == 0 )
										//				radiaceData[ n ] = 1.0f;
										//			}

										//			//for( int n = 0; n < voxelCount; n++ )
										//			//	radiaceData[ n ] = 1.0f;


										//			//fixed( float* pRadiance = radiaceData )
										//			//{
										//			//	for( int n = 0; n < voxelCount; n++ )
										//			//		pRadiance[ n ] = 1.0f;
										//			//}
										//		}

										//		//var radiaceData = new byte[ voxelCount * 4 ];

										//		//var emissive = item.Scale.Equals( new Vector3F( 2, 2, 2 ), 0.01f );
										//		//if( emissive )
										//		//{
										//		//	fixed( byte* pRadiance = radiaceData )
										//		//	{
										//		//		float* pRadiance2 = (float*)pRadiance;

										//		//		for( int n = 0; n < voxelCount; n++ )
										//		//			pRadiance2[ n ] = 1.0f;
										//		//	}
										//		//}

										//		objectIdStart = data.objects.Count;


										//		data.directRadiance.AddRange( radiaceData );

										//		var objectIds = new float[ radiaceData.Length ];
										//		for( int n = 0; n < objectIds.Length; n++ )
										//			objectIds[ n ] = objectIdStart;
										//		data.radianceObjectIds.AddRange( objectIds );

										//		var localVoxelIndices = new float[ radiaceData.Length ];
										//		for( int n = 0; n < localVoxelIndices.Length; n++ )
										//			localVoxelIndices[ n ] = n;
										//		data.radianceLocalVoxelIndices.AddRange( localVoxelIndices );


										//		data.objects.Add( ref item );

										//		data.objectsLocalSpaceBounds.Add( meshData.SpaceBounds );

										//		//!!!!
										//		var tr = new Transform( position, rotation, scale );
										//		//var tr = new Transform( item.Position, item.Rotation, item.Scale );
										//		data.objectsWorldSpaceBounds.Add( SpaceBounds.Multiply( tr, meshData.SpaceBounds ) );
										//	}


										//	//var tr = obj.TransformV;

										//	//var geometry = RenderingEffect_IndirectLighting.GetVoxelRenderOperation( meshData );
										//	//if( geometry != null )
										//	//{
										//	//	var voxelData = geometry.VoxelData.Value;
										//	//	if( voxelData != null )
										//	//	{
										//	//		var gpuVoxelData = RenderingEffect_IndirectLighting.GetGpuVoxelData( voxelData );
										//	//		if( gpuVoxelData != null )
										//	//		{
										//	//			if( !data.objectTypeIndexByMeshGeometry.TryGetValue( geometry/* gpuVoxelData*/, out var objectTypeIndex ) )
										//	//			{
										//	//				objectTypeIndex = data.objectTypes.Count;
										//	//				data.objectTypes.AddRange( gpuVoxelData );

										//	//				data.objectTypeIndexByMeshGeometry[ geometry/* gpuVoxelData*/ ] = objectTypeIndex;
										//	//			}

										//	//			var item = new RenderingEffect_IndirectLighting.GPUObjectData();

										//	//			//!!!!double
										//	//			item.Position = tr.Position.ToVector3F();
										//	//			item.Rotation = tr.Rotation.ToQuaternionF();
										//	//			item.Scale = tr.Scale.ToVector3F();
										//	//			item.ObjectTypeIndex = objectTypeIndex;

										//	//			objectIdStart = data.objects.Count;

										//	//			data.objects.Add( ref item );
										//	//		}
										//	//	}
										//	//}


										//	////!!!!
										//	//var objectIdStart = objectIdCounter;
										//	//objectIdCounter++;


										//}

										BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItemoperation.UnwrappedUV, ref meshItem.Color, outputItemoperation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, meshItem.ObjectInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

										if( !meshItem.InstancingEnabled )
											fixed( Matrix4F* p = &meshItem.TransformRelative )
												Bgfx.SetTransform( (float*)p );

										var pass = materialData.deferredShadingPass.Get( outputItemoperationVoxelDataInfo != null/*, outputItemoperation.VirtualizedData != null*/, meshData.BillboardMode != 0 );

										var tessItem = meshItem.Tessellation ? TessellationGetItem( context, outputItemoperation, materialData ) : null;

										RenderOperation( context, outputItemoperation, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount, tessItem );
									}
									else if( renderableItem.X == 1 )
									{
										//billboards
										ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
										var meshData = Billboard.GetBillboardMesh().Result.MeshData;

										BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, outputItemoperation.UnwrappedUV, ref billboardItem.Color, outputItemoperation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, billboardItem.MaterialInstanceParameters, 0, ref vector3FZero );

										billboardItem.GetWorldMatrixRelative( out var worldMatrix );
										Bgfx.SetTransform( (float*)&worldMatrix );

										var pass = materialData.deferredShadingPass.Billboard;

										RenderOperation( context, outputItemoperation, pass, null, billboardItem.CutVolumes );
									}
								}
							}
						}
					}

					//render layers
					if( DebugDrawLayers )
					{
						for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
						{
							var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

							if( renderableGroup.RenderableGroup.X == 0 )
							{
								//meshes

								ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.RenderableGroup.Y ];
								var meshData = meshItem.MeshData;

								if( meshItem.Layers != null )
								{
									for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
									{
										ref var layer = ref meshItem.Layers[ nLayer ];
										foreach( var materialData in GetLayerMaterialData( ref layer, true, true ) )
										{
											if( materialData.deferredShadingPass.Usual != null )
											{
												if( !meshItem.InstancingEnabled )
													fixed( Matrix4F* p = &meshItem.TransformRelative )
														Bgfx.SetTransform( (float*)p );

												var color = /*meshItem.Color * */ layer.MaterialColor;

												for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
												{
													var oper = meshData.RenderOperations[ nOperation ];
													var voxelRendering = oper.VoxelDataInfo != null;

													//bind material data
													BindMaterialData( context, materialData, false, voxelRendering );
													BindSamplersForTextureOnlySlots( context, false, voxelRendering );
													materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );

													BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, null, oper.VoxelDataInfo, meshItem.ObjectInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative, layer.UVScale );

													var pass = materialData.deferredShadingPass.Get( oper.VoxelDataInfo != null/*, oper.VirtualizedData != null*/, meshData.BillboardMode != 0 );

													RenderOperation( context, oper, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount );
												}
											}
										}
									}
								}
							}
						}
					}
				}

				//clear outputInstancingManagers
				foreach( var manager in outputInstancingManagers )
					manager.Clear();
			}

			Bgfx.Discard( DiscardFlags.All );
		}

		void InitDecalMesh()
		{
			decalMesh = ComponentUtility.CreateComponent<Mesh>( null, true, false );
			decalMesh.CreateComponent<MeshGeometry_Box>();
			decalMesh.Enabled = true;
		}

		[MethodImpl( (MethodImplOptions)512 )]
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
			CollectionUtility.MergeSortUnmanaged( sortedIndexes, delegate ( int a, int b )
			{
				var sortOrderA = sortOrders[ a ];
				var sortOrderB = sortOrders[ b ];
				if( sortOrderA < sortOrderB )
					return -1;
				if( sortOrderA > sortOrderB )
					return 1;
				return 0;
			}, true );

			var currentDecalNormalTangent0 = new Vector4F( float.MaxValue, 0, 0, 0 );
			var currentDecalNormalTangent1 = new Vector4F( float.MaxValue, 0, 0, 0 );


			//bind textures for all render operations
			BindMaterialsTexture( context, frameData );
			BindBonesTexture( context, frameData );
			BindSamplersForTextureOnlySlots( context, true, false );
			BindMaterialData( context, null, false, false );

			Vector4F* normalTangent = stackalloc Vector4F[ 2 ];

			foreach( var decalIndex in sortedIndexes )
			{
				ref var decalData = ref decalsData.Data[ decalIndex ];
				var meshData = decalMesh.Result.MeshData;

				for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
				{
					var oper = meshData.RenderOperations[ nOperation ];
					var voxelRendering = oper.VoxelDataInfo != null;

					foreach( var materialData in GetDecalMaterialData( ref decalData, true, true ) )
					{
						if( materialData.decalSupport )
						{
							//bind material data
							BindMaterialData( context, materialData, false, voxelRendering );
							BindSamplersForTextureOnlySlots( context, false, voxelRendering );

							//var receiveAnotherDecals = true;

							var zeroVector = Vector3F.Zero;

							BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, false/*receiveAnotherDecals*/, ref zeroVector, 0, oper.UnwrappedUV, ref decalData.Color, oper.VertexStructureContainsColor, false, decalData.VisibilityDistance, 1.0f, false, null, oper.VoxelDataInfo, null, 0, ref vector3FZero );

							var worldMatrixAbsolute = new Matrix4( decalData.Rotation.ToMatrix3() * Matrix3F.FromScale( decalData.Scale ), decalData.Position );
							context.ConvertToRelative( ref worldMatrixAbsolute, out var worldMatrixRelative );
							Bgfx.SetTransform( (float*)&worldMatrixRelative );

							//var worldMatrixAbsolute = new Matrix4F( decalData.Rotation.ToMatrix3() * Matrix3F.FromScale( decalData.Scale ), decalData.Position.ToVector3F() );
							//Bgfx.SetTransform( (float*)&worldMatrix );

							//u_decalMatrix
							{
								//!!!!slowly

								Matrix3F.FromRotateByY( MathEx.PI / 2, out var rotate3 );
								rotate3.ToMatrix4( out var rotate4 );
								worldMatrixRelative.GetInverse( out var inverse );
								Matrix4F.Multiply( ref rotate4, ref inverse, out var decalMatrixRelative );
								//Matrix4F decalMatrix = Matrix3F.FromRotateByY( MathEx.PI / 2 ).ToMatrix4() * worldMatrix.GetInverse();

								Bgfx.SetUniform( u_decalMatrix, &decalMatrixRelative );


								//Matrix3.FromRotateByY( MathEx.PI / 2, out var rotate3 );
								//rotate3.ToMatrix4( out var rotate4 );
								//worldMatrixAbsolute.GetInverse( out var inverse );
								//Matrix4.Multiply( ref rotate4, ref inverse, out var decalMatrixAbsolute );
								////Matrix4F decalMatrix = Matrix3F.FromRotateByY( MathEx.PI / 2 ).ToMatrix4() * worldMatrix.GetInverse();

								//context.ConvertToRelative( ref decalMatrixAbsolute, out var decalMatrix );
								//Bgfx.SetUniform( u_decalMatrix, &decalMatrix );

								////!!!!slowly
								//Matrix3F.FromRotateByY( MathEx.PI / 2, out var rotate3 );
								//rotate3.ToMatrix4( out var rotate4 );
								//worldMatrix.GetInverse( out var inverse );
								//Matrix4F.Multiply( ref rotate4, ref inverse, out var decalMatrix );
								////Matrix4F decalMatrix = Matrix3F.FromRotateByY( MathEx.PI / 2 ).ToMatrix4() * worldMatrix.GetInverse();

								//Bgfx.SetUniform( u_decalMatrix, &decalMatrix );
							}

							//u_decalNormalTangent
							{
								//Vector4F* normalTangent = stackalloc Vector4F[ 2 ];
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

							context.BindTexture( 3/* "depthTexture"*/, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
							context.BindTexture( 4/* "gBuffer1TextureCopy"*/, gBuffer1TextureCopy, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
							context.BindTexture( 5/* "gBuffer4TextureCopy"*/, gBuffer4TextureCopy, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
							context.BindTexture( 6/* "gBuffer5TextureCopy"*/, gBuffer5TextureCopy, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
							//!!!!
							//motionAndObjectIdTexture
							//context.BindTexture( 7/* "gBuffer6TextureCopy"*/, gBuffer6TextureCopy, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );

							RenderOperation( context, oper, materialData.decalShadingPass, null );

							////!!!!или если уже собран из GroupOfObjects
							//bool instancing = Instancing && meshItem.AnimationData == null;
							//outputInstancingManager.Add( renderableGroup, nOperation, oper, materialData, instancing );
						}
					}
				}
			}

			Bgfx.Discard( DiscardFlags.All );
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

				deferredShadingData.ambientPass = passItem;
				//deferredShadingData.passesPerLightWithoutShadows[ 0 ] = passItem;
			}

			////direct lights
			//for( int nShadows = 0; nShadows < 3; nShadows++ )
			//{
			//	var shadows = nShadows != 0;
			//	var contactShadows = nShadows == 2;

			//	if( RenderingSystem.ShadowTechnique == ProjectSettingsPage_Rendering.ShadowTechniqueEnum.None && shadows )
			//		continue;

			//	for( int nLight = 1; nLight < 4; nLight++ )//for( int nLight = 0; nLight < 4; nLight++ )
			//	{
			//		var lightType = (Light.TypeEnum)nLight;
			//		//if( shadows && lightType == Light.TypeEnum.Ambient )
			//		//	continue;

			//		//generate compile arguments
			//		var vertexDefines = new List<(string, string)>();
			//		var fragmentDefines = new List<(string, string)>();
			//		{
			//			var generalDefines = new List<(string, string)>();
			//			generalDefines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );

			//			//receive shadows support
			//			if( shadows )
			//			{
			//				fragmentDefines.Add( ("SHADOW_MAP", "") );
			//				if( contactShadows )
			//					fragmentDefines.Add( ("SHADOW_CONTACT", "") );

			//				//if( nShadows == 2 )
			//				//	fragmentDefines.Add( ("SHADOW_MAP_HIGH", "") );
			//				//else
			//				//	fragmentDefines.Add( ("SHADOW_MAP_LOW", "") );
			//			}

			//			vertexDefines.AddRange( generalDefines );
			//			fragmentDefines.AddRange( generalDefines );
			//		}

			//		string error2;

			//		//vertex program
			//		GpuProgram vertexProgram = GpuProgramManager.GetProgram( "DeferredDirectLight_",
			//			GpuProgramType.Vertex, @"Base\Shaders\DeferredDirectLight_vs.sc", vertexDefines, true, out error2 );
			//		if( !string.IsNullOrEmpty( error2 ) )
			//		{
			//			Log.Fatal( error2 );
			//			return;
			//		}

			//		//fragment program
			//		GpuProgram fragmentProgram = GpuProgramManager.GetProgram( "DeferredDirectLight_",
			//			GpuProgramType.Fragment, @"Base\Shaders\DeferredDirectLight_fs.sc", fragmentDefines, true, out error2 );
			//		if( !string.IsNullOrEmpty( error2 ) )
			//		{
			//			Log.Fatal( error2 );
			//			return;
			//		}

			//		var passItem = new DeferredShadingData.PassItem();
			//		passItem.vertexProgram = vertexProgram;
			//		passItem.fragmentProgram = fragmentProgram;
			//		//deferredShadingData.passesPerLightWithoutShadows[ nLight ] = passItem;

			//		//!!!!
			//		//var pass = GpuMaterialPass.CreatePass( vertexProgram, fragmentProgram );
			//		////!!!!
			//		//pass.DepthCheck = false;
			//		//pass.DepthWrite = false;
			//		//pass.SourceBlendFactor = SceneBlendFactor.One;
			//		//pass.DestBlendFactor = SceneBlendFactor.One;

			//		////!!!!
			//		////pass.CullingMode = CullingMode.None;
			//		////!!!!с дептом тогда не то
			//		////pass.CullingMode = CullingMode.Anticlockwise;

			//		////if( TwoSided )
			//		////	pass.CullingMode = CullingMode.None;

			//		if( nShadows == 2 )
			//			deferredShadingData.passesPerLightWithShadowsContactShadows[ nLight ] = passItem;
			//		else if( nShadows == 1 )
			//			deferredShadingData.passesPerLightWithShadows[ nLight ] = passItem;
			//		else
			//			deferredShadingData.passesPerLightWithoutShadows[ nLight ] = passItem;

			//		//if( nShadows == 2 )
			//		//	deferredShadingData.passesPerLightWithShadowsHigh[ nLight ] = passItem;
			//		//if( nShadows == 1 )
			//		//	deferredShadingData.passesPerLightWithShadowsLow[ nLight ] = passItem;
			//		//else
			//		//	deferredShadingData.passesPerLightWithoutShadows[ nLight ] = passItem;
			//	}
			//}
		}

		[StructLayout( LayoutKind.Sequential )]
		internal struct DeferredEnvironmentDataUniform
		{
			public QuaternionF rotation;
			public Vector4F multiplierAndAffect;
			public QuaternionF iblRotation;
			public Vector4F iblMultiplierAndAffect;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void RenderEnvironmentLightDeferred( ViewportRenderingContext context, FrameData frameData, ImageComponent sceneTexture, ImageComponent normalTexture, ImageComponent gBuffer2Texture, ImageComponent gBuffer3Texture, ImageComponent gBuffer4Texture, ImageComponent gBuffer5Texture, ImageComponent depthTexture )
		{
			//var ambientLight = frameData.Lights[ frameData.LightsInFrustumSorted[ 0 ] ];
			//ambientLight.Bind( this, context );

			var passItem = deferredShadingData.ambientPass;
			//var passItem = deferredShadingData.passesPerLightWithoutShadows[ 0 ];

			context.ObjectsDuringUpdate.namedTextures.TryGetValue( "ssr", out var ssrTexture );

			var generalContainer = new ParameterContainer();
			{
				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 0/* "sceneTexture"*/, sceneTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 1/* "normalTexture"*/, normalTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 2/* "gBuffer2Texture"*/, gBuffer2Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 9/* "gBuffer3Texture"*/, gBuffer3Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 3/* "depthTexture"*/, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 10/* "gBuffer4Texture"*/, gBuffer4Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 11/* "gBuffer5Texture"*/, gBuffer5Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 8/*"s_brdfLUT"*/, BrdfLUT, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 12, ssrTexture ?? ResourceUtility.DummyTexture2DArrayARGB8, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );
			}

			if( u_environmentLightParams == null )
				u_environmentLightParams = GpuProgramManager.RegisterUniform( "u_environmentLightParams", UniformType.Vector4, 1 );
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

			//u_environmentLightParams
			{
				var ambientLight = frameData.Lights[ frameData.LightsInFrustumSorted[ 0 ] ];

				var parameters = new Vector4F( ambientLight.data.Power, ssrTexture != null ? 1 : 0 );
				Bgfx.SetUniform( u_environmentLightParams.Value, &parameters, 1 );
			}

			//ambient light
			{
				//set u_reflectionProbeData
				var reflectionProbeData = new Vector4F( 0, 0, 0, 0 );
				Bgfx.SetUniform( u_reflectionProbeData.Value, &reflectionProbeData, 1 );

				var container = new ParameterContainer();

				//bind env textures, parameters
				{
					container.Set( new ViewportRenderingContext.BindTextureData( 6/*"s_environmentTexture"*/, ambientLightTexture.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					//container.Set( new ViewportRenderingContext.BindTextureData( 7/*"environmentTextureIBL"*/,
					//	ambientLightIrradiance.Value.texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					var data = new DeferredEnvironmentDataUniform();
					data.rotation = ambientLightTexture.Rotation;
					data.multiplierAndAffect = ambientLightTexture.MultiplierAndAffect;
					data.iblRotation = ambientLightIrradiance.Rotation;
					data.iblMultiplierAndAffect = ambientLightIrradiance.MultiplierAndAffect;
					Bgfx.SetUniform( u_deferredEnvironmentData.Value, &data, 4 );

					fixed( Vector4F* harmonics2 = ambientLightIrradiance.Harmonics ?? EnvironmentIrradianceData.GrayHarmonics )
						Bgfx.SetUniform( u_deferredEnvironmentIrradiance.Value, harmonics2, 9 );

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

				//program already compiled
				//if( ssrMin != null )
				//	shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "SSR" ) );

				context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
			}

			var harmonics = stackalloc Vector4F[ 9 ];

			//reflection probes
			foreach( var item in frameData.ReflectionProbes )
			{
				//set u_reflectionProbeData
				context.ConvertToRelative( ref item.data.Sphere.Center, out var center );
				var reflectionProbeData = new Vector4F( center, (float)item.data.Sphere.Radius );
				//var reflectionProbeData = new Vector4F( item.data.Sphere.Center.ToVector3F(), (float)item.data.Sphere.Radius );
				Bgfx.SetUniform( u_reflectionProbeData.Value, &reflectionProbeData, 1 );
				//reflectionProbeItem.Bind( this, context );

				var container = new ParameterContainer();

				//bind env textures, parameters
				{
					//use reflection from the reflection probe
					container.Set( new ViewportRenderingContext.BindTextureData( 6/*"s_environmentTexture"*/, item.CubemapEnvironmentWithMipmapsAndBlur ?? item.data.CubemapEnvironment ?? ResourceUtility.GrayTextureCube, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					////use lighting from the ambient light
					//container.Set( new ViewportRenderingContext.BindTextureData( 7/*"environmentTextureIBL"*/,
					//	ambientLightIrradiance.Value.texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					var data = new DeferredEnvironmentDataUniform();
					data.rotation = item.data.Rotation;
					data.iblRotation = ambientLightIrradiance.Rotation;
					data.multiplierAndAffect = new Vector4F( item.data.Multiplier, item.data.Intensity );
					data.iblMultiplierAndAffect = ambientLightIrradiance.MultiplierAndAffect;
					Bgfx.SetUniform( u_deferredEnvironmentData.Value, &data, 4 );

					{
						Vector4F[] h1;
						if( item.CubemapEnvironmentWithMipmapsAndBlur != null )
							h1 = EnvironmentIrradianceData.GetHarmonicsToUseMap( item.CubemapEnvironmentWithMipmapsAndBlur, item.data.CubemapEnvironmentAffectLightingLodOffset );
						else
							h1 = item.data.HarmonicsIrradiance ?? EnvironmentIrradianceData.GrayHarmonics;

						var h2 = ambientLightIrradiance.Harmonics;
						for( int n = 0; n < h1.Length; n++ )
							harmonics[ n ] = Vector4F.Lerp( h2[ n ], h1[ n ], item.data.Intensity );
					}

					Bgfx.SetUniform( u_deferredEnvironmentIrradiance.Value, harmonics, 9 );


					//fixed( Vector4F* harmonics = item.data.HarmonicsIrradiance ?? EnvironmentIrradianceData.GrayHarmonics )
					//	Bgfx.SetUniform( u_deferredEnvironmentIrradiance.Value, harmonics, 9 );

					////fixed( Vector4F* harmonics = ambientLightIrradiance.Harmonics ?? EnvironmentIrradianceData.GrayHarmonics )
					////	Bgfx.SetUniform( u_deferredEnvironmentIrradiance.Value, harmonics, 9 );

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

				//if( ssrMin != null )
				//	shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "SSR" ) );

				//!!!!triangles

				//if (screenTriangles != null)
				//    context.RenderTrianglesToCurrentViewport(shader, screenTriangles, CanvasRenderer.BlendingType.Add);
				//else
				context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.AlphaBlend );
			}
		}

		//static List<CanvasRenderer.TriangleVertex> tempScreenTrianglesForDeferred;
		static ParameterContainer tempGeneralContainerForDeferred = new ParameterContainer();
		////static CanvasRenderer.ShaderItem tempShaderForDeferred;

		//[MethodImpl( (MethodImplOptions)512 )]
		//unsafe void RenderDirectLightDeferred( ViewportRenderingContext context, FrameData frameData, ImageComponent sceneTexture, ImageComponent normalTexture, ImageComponent gBuffer2Texture, ImageComponent gBuffer3Texture, ImageComponent gBuffer4Texture, ImageComponent gBuffer5Texture, ImageComponent depthTexture, LightItem lightItem, bool firstDeferredLight )
		//{
		//	var viewportOwner = context.Owner;

		//	bool skip = false;

		//	//!!!!можно рисовать 3D геометрией если камера за пределами источника. тогда будет depth test
		//	//!!!!!!нужно кешировать меши источников

		//	if( tempScreenTrianglesForDeferred == null )
		//		tempScreenTrianglesForDeferred = new List<CanvasRenderer.TriangleVertex>();
		//	tempScreenTrianglesForDeferred.Clear();
		//	//List<CanvasRenderer.TriangleVertex> screenTriangles = null;
		//	var screenTriangles = tempScreenTrianglesForDeferred;


		//	//Point, Spotlight
		//	if( lightItem.data.Type == Light.TypeEnum.Point || lightItem.data.Type == Light.TypeEnum.Spotlight )
		//	{
		//		Vector3[] positions;
		//		int[] indices;

		//		//!!!!GC

		//		if( lightItem.data.Type == Light.TypeEnum.Point )
		//		{
		//			//Point

		//			//!!!!maybe sphere

		//			var b = new Bounds( lightItem.data.Position );
		//			b.Expand( lightItem.data.AttenuationFar * 1.01 );
		//			SimpleMeshGenerator.GenerateBox( b, out positions, out indices );

		//			//var radius = lightItem.data.AttenuationFar * 1.1;
		//			//SimpleMeshGenerator.GenerateSphere( radius, 8, 8, false, out positions, out indices );
		//			//for( int n = 0; n < positions.Length; n++ )
		//			//	positions[ n ] += lightItem.data.Position;
		//		}
		//		else
		//		{
		//			//!!!!can be less amount of triangles

		//			//Spotlight
		//			var outer = lightItem.data.SpotlightOuterAngle;
		//			if( outer > 179 )
		//				outer = 179;
		//			var radius = lightItem.data.AttenuationFar * Math.Tan( outer.InRadians() / 2 );
		//			var height = lightItem.data.AttenuationFar;
		//			SimpleMeshGenerator.GenerateCone( 0, SimpleMeshGenerator.ConeOrigin.Center, radius, height, 16, true, true, out positions, out indices );

		//			var mat = new Matrix4(
		//				lightItem.data.Rotation.ToMatrix3() * Matrix3.FromRotateByY( Math.PI ) * Matrix3.FromScale( new Vector3( 1.1, 1.1, 1.1 ) ),
		//				lightItem.data.Position + lightItem.data.Rotation.GetForward() * lightItem.data.AttenuationFar / 2 );

		//			for( int n = 0; n < positions.Length; n++ )
		//				positions[ n ] = mat * positions[ n ];
		//		}

		//		////var screenPositions = new Vector2F[ positions.Length ];
		//		////for( int n = 0; n < screenPositions.Length; n++ )
		//		////{
		//		////	context.Owner.CameraSettings.ProjectToScreenCoordinates( positions[ n ], out var screen, false );
		//		////	screenPositions[ n ] = screen.ToVector2F();
		//		////	//if( context.Owner.CameraSettings.ProjectToScreenCoordinates( positions[ n ], out var screen ) )
		//		////	//	screenPositions[ n ] = screen.ToVec2F();
		//		////	//else
		//		////	//	screenPositions[ n ] = new Vec2F( float.NaN, float.NaN );
		//		////}

		//		//screenTriangles = new List<CanvasRenderer.TriangleVertex>( indices.Length / 2 );

		//		var planes = viewportOwner.CameraSettings.Frustum.Planes;

		//		for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
		//		{
		//			var polygon = new Vector3[ 3 ];
		//			polygon[ 0 ] = positions[ indices[ nTriangle * 3 + 0 ] ];
		//			polygon[ 1 ] = positions[ indices[ nTriangle * 3 + 1 ] ];
		//			polygon[ 2 ] = positions[ indices[ nTriangle * 3 + 2 ] ];

		//			//clamp by frustum side planes
		//			for( int nPlane = 2; nPlane < 6; nPlane++ )
		//			{
		//				ref var plane = ref planes[ nPlane ];
		//				polygon = MathAlgorithms.ClipPolygonByPlane( polygon, -plane );
		//			}

		//			//convert world to screen coordinates
		//			var screenPolygon = new Vector2[ polygon.Length ];
		//			for( int n = 0; n < screenPolygon.Length; n++ )
		//				context.Owner.CameraSettings.ProjectToScreenCoordinates( polygon[ n ], out screenPolygon[ n ], false );

		//			//triangulate
		//			for( int n = 1; n < screenPolygon.Length - 1; n++ )
		//			{
		//				var p0 = screenPolygon[ 0 ].ToVector2F();
		//				var p1 = screenPolygon[ n ].ToVector2F();
		//				var p2 = screenPolygon[ n + 1 ].ToVector2F();

		//				if( ( ( p1.Y - p0.Y ) * ( p2.X - p1.X ) - ( p2.Y - p1.Y ) * ( p1.X - p0.X ) ) < 0 )
		//				{
		//					screenTriangles.Add( new CanvasRenderer.TriangleVertex( p0, ColorValue.One, p0 ) );
		//					screenTriangles.Add( new CanvasRenderer.TriangleVertex( p1, ColorValue.One, p1 ) );
		//					screenTriangles.Add( new CanvasRenderer.TriangleVertex( p2, ColorValue.One, p2 ) );
		//				}
		//			}
		//		}

		//		////var screenPositions = new Vector2F[ positions.Length ];
		//		////for( int n = 0; n < screenPositions.Length; n++ )
		//		////{
		//		////	context.Owner.CameraSettings.ProjectToScreenCoordinates( positions[ n ], out var screen, false );
		//		////	screenPositions[ n ] = screen.ToVector2F();
		//		////	//if( context.Owner.CameraSettings.ProjectToScreenCoordinates( positions[ n ], out var screen ) )
		//		////	//	screenPositions[ n ] = screen.ToVec2F();
		//		////	//else
		//		////	//	screenPositions[ n ] = new Vec2F( float.NaN, float.NaN );
		//		////}

		//		////screenTriangles = new List<CanvasRenderer.TriangleVertex>( indices.Length / 2 );

		//		////for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
		//		////{
		//		////	var p0 = screenPositions[ indices[ nTriangle * 3 + 0 ] ];
		//		////	var p1 = screenPositions[ indices[ nTriangle * 3 + 1 ] ];
		//		////	var p2 = screenPositions[ indices[ nTriangle * 3 + 2 ] ];

		//		////	if( !float.IsNaN( p0.X ) && !float.IsNaN( p1.X ) && !float.IsNaN( p2.X ) )
		//		////	{
		//		////		if( ( ( p1.Y - p0.Y ) * ( p2.X - p1.X ) - ( p2.Y - p1.Y ) * ( p1.X - p0.X ) ) < 0 )
		//		////		{
		//		////			if( !( p0.X < 0 && p1.X < 0 && p2.X < 0 ) && !( p0.X > 1 && p1.X > 1 && p2.X > 1 ) )
		//		////			{
		//		////				if( !( p0.Y < 0 && p1.Y < 0 && p2.Y < 0 ) && !( p0.Y > 1 && p1.Y > 1 && p2.Y > 1 ) )
		//		////				{
		//		////					//context.Owner.CanvasRenderer.AddLine( p0, p1, ColorValue.One );
		//		////					//context.Owner.CanvasRenderer.AddLine( p1, p2, ColorValue.One );
		//		////					//context.Owner.CanvasRenderer.AddLine( p2, p0, ColorValue.One );

		//		////					screenTriangles.Add( new CanvasRenderer.TriangleVertex( p0, ColorValue.One, p0 ) );
		//		////					screenTriangles.Add( new CanvasRenderer.TriangleVertex( p1, ColorValue.One, p1 ) );
		//		////					screenTriangles.Add( new CanvasRenderer.TriangleVertex( p2, ColorValue.One, p2 ) );
		//		////				}
		//		////			}
		//		////		}
		//		////	}
		//		////}

		//		//////check to use fullscreen quad and skip light
		//		////if( screenTriangles.Count != 0 )
		//		////{
		//		////	var plane = viewportOwner.CameraSettings.Frustum.Planes[ 0 ];

		//		////	int backsideCount = 0;
		//		////	foreach( var pos in positions )
		//		////	{
		//		////		if( plane.GetSide( pos ) == Plane.Side.Positive )
		//		////			backsideCount++;
		//		////	}

		//		////	//all backside
		//		////	if( backsideCount == positions.Length )
		//		////		skip = true;
		//		////	//use fullscreen quad when exists points backward camera
		//		////	if( backsideCount != 0 )
		//		////		screenTriangles = null;
		//		////}

		//		//all triangles outside screen
		//		if( /*screenTriangles != null &&*/ screenTriangles.Count == 0 )
		//			skip = true;
		//	}


		//	if( !skip )
		//	{
		//		lightItem.Bind( this, context );

		//		//pass
		//		//GpuMaterialPass pass;
		//		DeferredShadingData.PassItem passItem;
		//		{
		//			if( lightItem.prepareShadows )
		//			{
		//				if( lightItem.data.ShadowContactLength > 0 )
		//					passItem = deferredShadingData.passesPerLightWithShadowsContactShadows[ (int)lightItem.data.Type ];
		//				else
		//					passItem = deferredShadingData.passesPerLightWithShadows[ (int)lightItem.data.Type ];

		//				//if( ShadowQuality.Value == ShadowQualityEnum.High )
		//				//	passItem = deferredShadingData.passesPerLightWithShadowsHigh[ (int)lightItem.data.Type ];
		//				//else
		//				//	passItem = deferredShadingData.passesPerLightWithShadowsLow[ (int)lightItem.data.Type ];
		//			}
		//			else
		//				passItem = deferredShadingData.passesPerLightWithoutShadows[ (int)lightItem.data.Type ];
		//		}

		//		{
		//			if( tempGeneralContainerForDeferred == null )
		//				tempGeneralContainerForDeferred = new ParameterContainer();
		//			tempGeneralContainerForDeferred.Clear();
		//			var generalContainer = tempGeneralContainerForDeferred;//var generalContainer = new ParameterContainer();

		//			{
		//				//bind textures

		//				//!!!!firstDeferredLight

		//				//if( firstDeferredLight )not works
		//				//{

		//				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 0/* "sceneTexture"*/, sceneTexture,
		//					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 1/* "normalTexture"*/, normalTexture,
		//					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 2/* "gBuffer2Texture"*/, gBuffer2Texture,
		//					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 8/*"s_brdfLUT"*/, BrdfLUT,
		//					TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

		//				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 9/* "gBuffer2Texture"*/, gBuffer3Texture,
		//					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 3/* "depthTexture"*/, depthTexture,
		//					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 10/* "gBuffer4Texture"*/, gBuffer4Texture,
		//					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 11/* "gBuffer5Texture"*/, gBuffer5Texture,
		//					TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//				//}

		//				//light mask
		//				if( RenderingSystem.LightMask && lightItem.data.Type != Light.TypeEnum.Ambient )
		//				{
		//					if( lightItem.data.Type == Light.TypeEnum.Point )
		//					{
		//						var texture = lightItem.data.Mask;
		//						if( texture?.Result == null || texture.Result.TextureType != ImageComponent.TypeEnum.Cube )
		//							texture = ResourceUtility.WhiteTextureCube;

		//						//!!!!anisotropic? где еще

		//						generalContainer.Set( new ViewportRenderingContext.BindTextureData( 4/*"s_lightMask"*/, texture,
		//							TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
		//						//var textureValue = new GpuMaterialPass.TextureParameterValue( texture,
		//						//	TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
		//						//generalContainer.Set( "4"/*"s_lightMask"*/, textureValue, ParameterType.TextureCube );
		//					}
		//					else
		//					{
		//						var texture = lightItem.data.Mask;
		//						if( texture?.Result == null || texture.Result.TextureType != ImageComponent.TypeEnum._2D )
		//							texture = ResourceUtility.WhiteTexture2D;

		//						var clamp = lightItem.data.Type == Light.TypeEnum.Spotlight;

		//						generalContainer.Set( new ViewportRenderingContext.BindTextureData( 4/*"s_lightMask"*/, texture,
		//							clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
		//					}
		//				}

		//				//if( lightItem.prepareShadows )
		//				{
		//					//Viewport shadowViewport = lightItem.shadowTexture.Result.GetRenderTarget().Viewports[ 0 ];

		//					//shadowMap
		//					{

		//						ImageComponent texture;
		//						if( lightItem.prepareShadows )
		//							texture = lightItem.shadowTexture;
		//						else
		//							texture = lightItem.data.Type == Light.TypeEnum.Point ? nullShadowTextureCube : nullShadowTexture2D;

		//						ViewportRenderingContext.BindTextureData textureValue;
		//						if( RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4 )
		//						{
		//							textureValue = new ViewportRenderingContext.BindTextureData( 5/*"s_shadowMap"*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None );
		//						}
		//						else
		//						{
		//							textureValue = new ViewportRenderingContext.BindTextureData( 5/*"s_shadowMap"*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None, TextureFlags.CompareLessEqual );
		//						}

		//						generalContainer.Set( ref textureValue );
		//					}

		//					//Vector2F shadowBias = new Vector2F( lightItem.data.ShadowBias, lightItem.data.ShadowNormalBias );
		//					//generalContainer.Set( "u_shadowBias", ParameterType.Vector2, 1, &shadowBias, sizeof( Vector2F ) );
		//				}
		//			}

		//			{
		//				//caching is not works
		//				//if( tempShaderForDeferred == null )
		//				//	tempShaderForDeferred = new CanvasRenderer.ShaderItem();
		//				//tempShaderForDeferred.Clear();

		//				//var shader = tempShaderForDeferred;//var shader = new CanvasRenderer.ShaderItem();
		//				var shader = new CanvasRenderer.ShaderItem();
		//				shader.CompiledVertexProgram = passItem.vertexProgram;
		//				shader.CompiledFragmentProgram = passItem.fragmentProgram;
		//				shader.AdditionalParameterContainers.Add( generalContainer );

		//				if( screenTriangles.Count != 0 /*screenTriangles != null*/ )
		//					context.RenderTrianglesToCurrentViewport( shader, screenTriangles, CanvasRenderer.BlendingType.Add, registerUniformsAndSetIdentityMatrix: firstDeferredLight );
		//				else
		//					context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add, registerUniformsAndSetIdentityMatrix: firstDeferredLight );
		//			}
		//		}
		//	}
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void RenderDirectLightsDeferredMultiLight( ViewportRenderingContext context, FrameData frameData, ImageComponent sceneTexture, ImageComponent normalTexture, ImageComponent gBuffer2Texture, ImageComponent gBuffer3Texture, ImageComponent gBuffer4Texture, ImageComponent gBuffer5Texture, ImageComponent depthTexture )
		{
			//var viewportOwner = context.Owner;

			if( sizeof( LightItem.LightDataFragmentMultiLight ) / sizeof( Vector4F ) != 29 )
				Log.Fatal( "RenderingPipeline_Basic: RenderDirectLightsDeferredMultiLight: Internal error." );

			var shader = new CanvasRenderer.ShaderItem();

			////pass
			////GpuMaterialPass pass;
			//DeferredShadingData.PassItem passItem;
			//{
			//	if( lightItem.prepareShadows )
			//	{
			//		if( lightItem.data.ShadowContactLength > 0 )
			//			passItem = deferredShadingData.passesPerLightWithShadowsContactShadows[ (int)lightItem.data.Type ];
			//		else
			//			passItem = deferredShadingData.passesPerLightWithShadows[ (int)lightItem.data.Type ];

			//		//if( ShadowQuality.Value == ShadowQualityEnum.High )
			//		//	passItem = deferredShadingData.passesPerLightWithShadowsHigh[ (int)lightItem.data.Type ];
			//		//else
			//		//	passItem = deferredShadingData.passesPerLightWithShadowsLow[ (int)lightItem.data.Type ];
			//	}
			//	else
			//		passItem = deferredShadingData.passesPerLightWithoutShadows[ (int)lightItem.data.Type ];
			//}

			//{
			if( tempGeneralContainerForDeferred == null )
				tempGeneralContainerForDeferred = new ParameterContainer();
			tempGeneralContainerForDeferred.Clear();
			var generalContainer = tempGeneralContainerForDeferred;//var generalContainer = new ParameterContainer();

			//bind textures

			generalContainer.Set( new ViewportRenderingContext.BindTextureData( 0/* "sceneTexture"*/, sceneTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

			generalContainer.Set( new ViewportRenderingContext.BindTextureData( 1/* "normalTexture"*/, normalTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

			generalContainer.Set( new ViewportRenderingContext.BindTextureData( 2/* "gBuffer2Texture"*/, gBuffer2Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

			generalContainer.Set( new ViewportRenderingContext.BindTextureData( 8/*"s_brdfLUT"*/, BrdfLUT, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

			generalContainer.Set( new ViewportRenderingContext.BindTextureData( 16/*9*/ /* "gBuffer3Texture"*/, gBuffer3Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

			generalContainer.Set( new ViewportRenderingContext.BindTextureData( 3/* "depthTexture"*/, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

			generalContainer.Set( new ViewportRenderingContext.BindTextureData( 17/*10*//* "gBuffer4Texture"*/, gBuffer4Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

			generalContainer.Set( new ViewportRenderingContext.BindTextureData( 11/* "gBuffer5Texture"*/, gBuffer5Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

			generalContainer.Set( new ViewportRenderingContext.BindTextureData( 4/* "s_lightsTexture"*/, frameData.LightsTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

			generalContainer.Set( new ViewportRenderingContext.BindTextureData( 15/* "s_lightGrid"*/, frameData.LightGrid ?? ResourceUtility.DummyTexture3DFloat32RGBA, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

			//gi
			var giData = frameData.GIData;
			if( giData != null )
			{
				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "GI_GRID" ) );

				generalContainer.Set( new ViewportRenderingContext.BindTextureData( 9/* "s_giGrid1"*/, frameData.GIData.Grid1Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 10/* "s_giGrid2"*/, frameData.GIData.Grid2TextureIndirected ?? frameData.GIData.Grid2Texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

				GISetRayCastInfoUniform( context, true );

				////!!!!cascades

				//var cascadeItem = giData.Cascades[ 0 ];

				////var cascadeBounds = giData.CascadesBoundsRelative[ 0 ];

				////!!!!
				//var gridPosition = cascadeItem.BoundsRelative.Minimum;//.ToVector3F();
				//													  //var gridPosition = cascadeBounds.Minimum.ToVector3F();
				//var gridSize = giData.GridSize;
				////var cellSize = cascadeBounds.GetSize().X / gridSize;
				////!!!!
				//var cascade = 0;

				//var p0 = new Vector4F( gridPosition, gridSize );
				//var p1 = new Vector4F( cascadeItem.CellSize, cascade, 0, 0 );

				//shader.Parameters.Set( "showRenderTargetGI0", p0 );
				//shader.Parameters.Set( "showRenderTargetGI1", p1 );
			}

			//shadows
			{
				var isByte4Format = RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4;
				var filtering = isByte4Format ? FilterOption.Point : FilterOption.Linear;
				////too chaotic
				//////to make linear light masks on mobile
				////var filtering = FilterOption.Linear;
				var textureFlags = isByte4Format ? 0 : TextureFlags.CompareLessEqual;

				//shadow map directional
				{
					var shadowMap = frameData.ShadowTextureArrayDirectional;
					if( shadowMap == null )
						shadowMap = isByte4Format ? ResourceUtility.DummyTexture2DArrayARGB8 : ResourceUtility.DummyShadowMap2DArrayFloat32R;

					var wrap = isByte4Format;

					var textureValue = new ViewportRenderingContext.BindTextureData( 5/*s_shadowMapShadowDirectional*/, shadowMap, wrap ? TextureAddressingMode.Wrap : TextureAddressingMode.Clamp, filtering, filtering, FilterOption.None, textureFlags );
					generalContainer.Set( ref textureValue );
				}

				//shadow map array spot
				{
					var shadowMap = frameData.ShadowTextureArraySpot;
					if( shadowMap == null )
						shadowMap = isByte4Format ? ResourceUtility.DummyTexture2DArrayARGB8 : ResourceUtility.DummyShadowMap2DArrayFloat32R;

					var textureValue = new ViewportRenderingContext.BindTextureData( 6/*s_shadowMapShadowSpot*/, shadowMap, TextureAddressingMode.Clamp, filtering, filtering, FilterOption.None, textureFlags );
					generalContainer.Set( ref textureValue );
				}

				//shadow map array point
				{
					var shadowMap = frameData.ShadowTextureArrayPoint;
					if( shadowMap == null )
						shadowMap = isByte4Format ? ResourceUtility.DummyTextureCubeArrayARGB8/*WhiteTextureCube*/ : ResourceUtility.DummyShadowMapCubeArrayFloat32R;

					var textureValue = new ViewportRenderingContext.BindTextureData( 7/*s_shadowMapShadowPoint*/, shadowMap, TextureAddressingMode.Clamp, filtering, filtering, FilterOption.None, textureFlags );
					generalContainer.Set( ref textureValue );
				}
			}

			//light masks
			if( RenderingSystem.LightMask )
			{
				//mask array directional
				{
					var texture = frameData.MaskTextureArrayDirectional ?? ResourceUtility.WhiteTexture2D;
					generalContainer.Set( new ViewportRenderingContext.BindTextureData( 12/*s_lightMaskDirectional*/, texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
				}

				//mask array spot
				{
					var texture = frameData.MaskTextureArraySpot ?? ResourceUtility.WhiteTexture2D;
					generalContainer.Set( new ViewportRenderingContext.BindTextureData( 13/*s_lightMaskSpot*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
				}

				//mask array point
				{
					var texture = frameData.MaskTextureArrayPoint ?? ResourceUtility.WhiteTextureCube;
					generalContainer.Set( new ViewportRenderingContext.BindTextureData( 14/*s_lightMaskPoint*/, texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
				}


				////ImageComponent shadowMapDirectional = null;
				//ImageComponent maskDirectional = null;

				//foreach( var lightIndex in frameData.LightsInFrustumSorted )
				//{
				//	var lightItem = frameData.Lights[ lightIndex ];

				//	if( lightItem.data.Type == Light.TypeEnum.Directional )
				//	{
				//		//!!!!
				//		maskDirectional = lightItem.data.Mask;
				//	}
				//}

				////if( shadowMapDirectional == null )
				////	shadowMapDirectional = nullShadowTexture2D;

				////!!!!
				//if( maskDirectional?.Result == null || maskDirectional.Result.TextureType != ImageComponent.TypeEnum._2D )
				//	maskDirectional = ResourceUtility.WhiteTexture2D;


				//{
				//	//		var clamp = lightItem.data.Type == Light.TypeEnum.Spotlight;

				//	//		generalContainer.Set( new ViewportRenderingContext.BindTextureData( 4/*"s_lightMask"*/, texture,
				//	//			clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				//	//!!!!
				//	var clamp = false;
				//	//var clamp = lightItem.data.Type == Light.TypeEnum.Spotlight;

				//	generalContainer.Set( new ViewportRenderingContext.BindTextureData( 7/*"s_lightMaskDirectional"*/, maskDirectional, clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
				//}



				//!!!!
				////light mask
				//if( RenderingSystem.LightMask && lightItem.data.Type != Light.TypeEnum.Ambient )
				//{
				//	if( lightItem.data.Type == Light.TypeEnum.Point )
				//	{
				//		var texture = lightItem.data.Mask;
				//		if( texture?.Result == null || texture.Result.TextureType != ImageComponent.TypeEnum.Cube )
				//			texture = ResourceUtility.WhiteTextureCube;

				//		//!!!!anisotropic? где еще

				//		generalContainer.Set( new ViewportRenderingContext.BindTextureData( 4/*"s_lightMask"*/, texture,
				//			TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
				//		//var textureValue = new GpuMaterialPass.TextureParameterValue( texture,
				//		//	TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
				//		//generalContainer.Set( "4"/*"s_lightMask"*/, textureValue, ParameterType.TextureCube );
				//	}
				//	else
				//	{
				//		var texture = lightItem.data.Mask;
				//		if( texture?.Result == null || texture.Result.TextureType != ImageComponent.TypeEnum._2D )
				//			texture = ResourceUtility.WhiteTexture2D;

				//		var clamp = lightItem.data.Type == Light.TypeEnum.Spotlight;

				//		generalContainer.Set( new ViewportRenderingContext.BindTextureData( 4/*"s_lightMask"*/, texture,
				//			clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
				//	}
				//}



				////ImageComponent shadowMapDirectional = null;
				//ImageComponent maskDirectional = null;

				//foreach( var lightIndex in frameData.LightsInFrustumSorted )
				//{
				//	var lightItem = frameData.Lights[ lightIndex ];

				//	if( lightItem.data.Type == Light.TypeEnum.Directional )
				//	{
				//		//if( lightItem.prepareShadows )
				//		//{
				//		//	shadowMapDirectional = lightItem.shadowTexture;
				//		//	//break;
				//		//}

				//		//!!!!
				//		maskDirectional = lightItem.data.Mask;
				//	}
				//}

				////if( shadowMapDirectional == null )
				////	shadowMapDirectional = nullShadowTexture2D;

				////!!!!
				//if( maskDirectional?.Result == null || maskDirectional.Result.TextureType != ImageComponent.TypeEnum._2D )
				//	maskDirectional = ResourceUtility.WhiteTexture2D;


				//{
				//	//		var clamp = lightItem.data.Type == Light.TypeEnum.Spotlight;

				//	//		generalContainer.Set( new ViewportRenderingContext.BindTextureData( 4/*"s_lightMask"*/, texture,
				//	//			clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				//	//!!!!
				//	var clamp = false;
				//	//var clamp = lightItem.data.Type == Light.TypeEnum.Spotlight;

				//	generalContainer.Set( new ViewportRenderingContext.BindTextureData( 7/*"s_lightMaskDirectional"*/, maskDirectional, clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
				//}
			}


			//!!!!forward
			var existsPrepareShadows = false;
			var existsContactShadows = false;
			{
				foreach( var lightIndex in frameData.LightsInFrustumSorted )
				{
					var lightItem = frameData.Lights[ lightIndex ];
					var lightData = lightItem.data;

					if( lightItem.prepareShadows )
					{
						existsPrepareShadows = true;
						if( lightData.ShadowContactLength != 0 )
						{
							existsContactShadows = true;
							break;
						}
					}
				}
			}

			if( existsPrepareShadows )
				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "SHADOW_MAP" ) );
			if( existsContactShadows )
				shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "SHADOW_CONTACT" ) );

			shader.VertexProgramFileName = @"Base\Shaders\DeferredDirectLight_vs.sc";
			shader.FragmentProgramFileName = @"Base\Shaders\DeferredDirectLight_fs.sc";
			shader.AdditionalParameterContainers.Add( generalContainer );
			context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
			//}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected unsafe virtual void RenderLightsDeferred( ViewportRenderingContext context, FrameData frameData, ImageComponent sceneTexture, ImageComponent normalTexture, ImageComponent gBuffer2Texture, ImageComponent gBuffer3Texture, ImageComponent gBuffer4Texture, ImageComponent gBuffer5Texture, ImageComponent depthTexture )
		{
			if( deferredShadingData == null )
				InitDeferredShadingData();

			//if( MultiLightOptimization )
			//{

			//ambient lighting
			if( frameData.LightsInFrustumSorted.Length > 0 )
			{
				var lightIndex = frameData.LightsInFrustumSorted[ 0 ];
				var lightItem = frameData.Lights[ lightIndex ];

				if( lightItem.data.Type != Light.TypeEnum.Ambient )
					Log.Fatal( "RenderingPipeline_Basic: RenderLightsDeferred: Internal error. lightItem.data.Type != Light.TypeEnum.Ambient" );

				RenderEnvironmentLightDeferred( context, frameData, sceneTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, depthTexture );
			}

			//direct lighting
			if( frameData.LightsInFrustumSorted.Length > 1 )
			{
				RenderDirectLightsDeferredMultiLight( context, frameData, sceneTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, depthTexture );
			}

			//}
			//else
			//{
			//	var firstDeferredLight = true;

			//	foreach( var lightIndex in frameData.LightsInFrustumSorted )
			//	{
			//		var lightItem = frameData.Lights[ lightIndex ];

			//		if( lightItem.data.Type == Light.TypeEnum.Ambient )
			//			RenderEnvironmentLightDeferred( context, frameData, sceneTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, depthTexture );
			//		else
			//		{
			//			RenderDirectLightDeferred( context, frameData, sceneTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, depthTexture, lightItem, firstDeferredLight );
			//			firstDeferredLight = false;
			//		}
			//	}
			//}
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

		static EnvironmentTextureData forwardBindTexture1;
		static EnvironmentIrradianceData forwardBindHarmonics1;
		static EnvironmentTextureData forwardBindTexture2;
		static EnvironmentIrradianceData forwardBindHarmonics2;


		[MethodImpl( (MethodImplOptions)512 )]
		public unsafe void ForwardBindGeneralTexturesUniforms( ViewportRenderingContext context, FrameData frameData/*, ref Sphere objectSphere, LightItem lightItem, bool receiveShadows*/, bool setUniforms )//, bool bindBrdfLUT = true )
		{

			//!!!!why bind each call

			//!!!!
			var objectSphere = new Sphere( Vector3.Zero, 1000000 );

			GetEnvironmentTexturesByBoundingSphereIntersection( context, frameData, ref objectSphere/*meshItem.BoundingSphere*/, ref forwardBindTexture1, ref forwardBindHarmonics1, ref forwardBindTexture2, ref forwardBindHarmonics2, out var environmentBlendingFactor );
			//GetEnvironmentTexturesByBoundingSphereIntersection( context, frameData, ref objectSphere/*meshItem.BoundingSphere*/, out var texture1, out var harmonics1, out var texture2, out var harmonics2, out var environmentBlendingFactor );

			//environment map
			{
				context.BindTexture( 3/*"environmentTexture1"*/, forwardBindTexture1.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, false );

				//context.BindTexture( new ViewportRenderingContext.BindTextureData( 3/*"environmentTextureIBL1"*/, harmonics1.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				context.BindTexture( 4/*"environmentTexture2"*/, forwardBindTexture2.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, false );

				//context.BindTexture( new ViewportRenderingContext.BindTextureData( 5/*"environmentTextureIBL2"*/, harmonics2.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				if( setUniforms )
				{
					if( !u_forwardEnvironmentData.HasValue )
					{
						u_forwardEnvironmentData = GpuProgramManager.RegisterUniform( "u_forwardEnvironmentData", UniformType.Vector4, 5 );
						u_forwardEnvironmentIrradiance1 = GpuProgramManager.RegisterUniform( "u_forwardEnvironmentIrradiance1", UniformType.Vector4, 9 );
						u_forwardEnvironmentIrradiance2 = GpuProgramManager.RegisterUniform( "u_forwardEnvironmentIrradiance2", UniformType.Vector4, 9 );
					}

					var data = new ForwardEnvironmentDataUniform();
					data.rotation1 = forwardBindTexture1.Rotation;
					data.multiplierAndAffect1 = forwardBindTexture1.MultiplierAndAffect;
					data.rotation2 = forwardBindTexture2.Rotation;
					data.multiplierAndAffect2 = forwardBindTexture2.MultiplierAndAffect;
					data.blendingFactor = environmentBlendingFactor;
					Bgfx.SetUniform( u_forwardEnvironmentData.Value, &data, 5 );

					fixed( Vector4F* harmonics = forwardBindHarmonics1.Harmonics ?? EnvironmentIrradianceData.GrayHarmonics )
						Bgfx.SetUniform( u_forwardEnvironmentIrradiance1.Value, harmonics, 9 );

					fixed( Vector4F* harmonics = forwardBindHarmonics2.Harmonics ?? EnvironmentIrradianceData.GrayHarmonics )
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

			////s_brdfLUT
			//if( bindBrdfLUT )
			//	BindBrdfLUT( context );


			////light mask
			//if( RenderingSystem.LightMask && lightItem.data.Type != Light.TypeEnum.Ambient )
			//{
			//	if( lightItem.data.Type == Light.TypeEnum.Point )
			//	{
			//		var texture = lightItem.data.Mask;
			//		if( texture == null || texture.Result.TextureType != ImageComponent.TypeEnum.Cube )
			//			texture = ResourceUtility.WhiteTextureCube;

			//		//!!!!anisotropic? где еще

			//		context.BindTexture( 7/*"s_lightMask"*/,
			//			texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, false );
			//	}
			//	else
			//	{
			//		var texture = lightItem.data.Mask;
			//		if( texture == null || texture.Result.TextureType != ImageComponent.TypeEnum._2D )
			//			texture = ResourceUtility.WhiteTexture2D;

			//		var clamp = lightItem.data.Type == Light.TypeEnum.Spotlight;
			//		context.BindTexture( 7/*"s_lightMask"*/,
			//			texture, clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap,
			//			FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, false );
			//	}
			//}


			////receive shadows
			////if( receiveShadows )
			//{
			//	//Viewport shadowViewport = lightItem.shadowTexture.Result.GetRenderTarget().Viewports[ 0 ];

			//	//shadowMap
			//	{
			//		ImageComponent texture;
			//		if( receiveShadows )
			//		{
			//			texture = lightItem.shadowTexture;
			//		}
			//		else
			//		{
			//			if( lightItem.data.Type == Light.TypeEnum.Point )
			//				texture = nullShadowTextureCube;
			//			else
			//				texture = nullShadowTexture2D;
			//		}

			//		if( RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4 )
			//		{
			//			context.BindTexture( 8/*"g_shadowMap"*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None, 0, false );
			//		}
			//		else
			//		{
			//			context.BindTexture( 8/*"g_shadowMap"*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None, TextureFlags.CompareLessEqual, false );
			//		}

			//		//var textureValue = new ViewportRenderingContext.BindTextureData( 8/*"g_shadowMap"*/,
			//		//	texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear,
			//		//	FilterOption.None );
			//		//textureValue.AdditionalFlags |= TextureFlags.CompareLessEqual;
			//		//context.BindTexture( ref textureValue );
			//	}

			//	//Vector2F shadowBias = new Vector2F( lightItem.data.ShadowBias, lightItem.data.ShadowNormalBias );
			//	//generalContainer.Set( "u_shadowBias", ParameterType.Vector2, 1, &shadowBias, sizeof( Vector2F ) );
			//}

			//////set u_environmentBlendingFactor
			////if( setUniforms )
			////{
			////	if( u_environmentBlendingFactor == null )
			////		u_environmentBlendingFactor = GpuProgramManager.RegisterUniform( "u_environmentBlendingFactor", UniformType.Vector4, 1 );

			////	var value = new Vector4F( environmentBlendingFactor, 0, 0, 0 );
			////	Bgfx.SetUniform( u_environmentBlendingFactor.Value, &value, 1 );
			////}
		}


		//[MethodImpl( (MethodImplOptions)512 )]
		//public unsafe void ForwardBindGeneralTexturesUniforms( ViewportRenderingContext context, FrameData frameData, ref Sphere objectSphere, LightItem lightItem, bool receiveShadows, bool setUniforms, bool bindBrdfLUT = true )
		//{
		//	GetEnvironmentTexturesByBoundingSphereIntersection( context, frameData, ref objectSphere/*meshItem.BoundingSphere*/, ref forwardBindTexture1, ref forwardBindHarmonics1, ref forwardBindTexture2, ref forwardBindHarmonics2, out var environmentBlendingFactor );
		//	//GetEnvironmentTexturesByBoundingSphereIntersection( context, frameData, ref objectSphere/*meshItem.BoundingSphere*/, out var texture1, out var harmonics1, out var texture2, out var harmonics2, out var environmentBlendingFactor );

		//	//environment map
		//	{
		//		context.BindTexture( 3/*"environmentTexture1"*/, forwardBindTexture1.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, false );

		//		//context.BindTexture( new ViewportRenderingContext.BindTextureData( 3/*"environmentTextureIBL1"*/, harmonics1.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

		//		context.BindTexture( 4/*"environmentTexture2"*/, forwardBindTexture2.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, false );

		//		//context.BindTexture( new ViewportRenderingContext.BindTextureData( 5/*"environmentTextureIBL2"*/, harmonics2.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

		//		if( setUniforms )
		//		{
		//			if( !u_forwardEnvironmentData.HasValue )
		//			{
		//				u_forwardEnvironmentData = GpuProgramManager.RegisterUniform( "u_forwardEnvironmentData", UniformType.Vector4, 5 );
		//				u_forwardEnvironmentIrradiance1 = GpuProgramManager.RegisterUniform( "u_forwardEnvironmentIrradiance1", UniformType.Vector4, 9 );
		//				u_forwardEnvironmentIrradiance2 = GpuProgramManager.RegisterUniform( "u_forwardEnvironmentIrradiance2", UniformType.Vector4, 9 );
		//			}

		//			var data = new ForwardEnvironmentDataUniform();
		//			data.rotation1 = forwardBindTexture1.Rotation;
		//			data.multiplierAndAffect1 = forwardBindTexture1.MultiplierAndAffect;
		//			data.rotation2 = forwardBindTexture2.Rotation;
		//			data.multiplierAndAffect2 = forwardBindTexture2.MultiplierAndAffect;
		//			data.blendingFactor = environmentBlendingFactor;
		//			Bgfx.SetUniform( u_forwardEnvironmentData.Value, &data, 5 );

		//			fixed( Vector4F* harmonics = forwardBindHarmonics1.Harmonics ?? EnvironmentIrradianceData.GrayHarmonics )
		//				Bgfx.SetUniform( u_forwardEnvironmentIrradiance1.Value, harmonics, 9 );

		//			fixed( Vector4F* harmonics = forwardBindHarmonics2.Harmonics ?? EnvironmentIrradianceData.GrayHarmonics )
		//				Bgfx.SetUniform( u_forwardEnvironmentIrradiance2.Value, harmonics, 9 );
		//		}

		//		//context.BindTexture( new ViewportRenderingContext.BindTextureData( 2/*"environmentTexture1"*/, texture1.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

		//		//context.BindTexture( new ViewportRenderingContext.BindTextureData( 3/*"environmentTextureIBL1"*/, harmonics1.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

		//		//context.BindTexture( new ViewportRenderingContext.BindTextureData( 4/*"environmentTexture2"*/, texture2.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

		//		//context.BindTexture( new ViewportRenderingContext.BindTextureData( 5/*"environmentTextureIBL2"*/, harmonics2.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

		//		//if( setUniforms )
		//		//{
		//		//	if( u_environmentTexture1Rotation == null )
		//		//		u_environmentTexture1Rotation = GpuProgramManager.RegisterUniform( "u_environmentTexture1Rotation", UniformType.Matrix3x3, 1 );
		//		//	if( u_environmentTexture2Rotation == null )
		//		//		u_environmentTexture2Rotation = GpuProgramManager.RegisterUniform( "u_environmentTexture2Rotation", UniformType.Matrix3x3, 1 );
		//		//	if( u_environmentTexture1MultiplierAndAffect == null )
		//		//		u_environmentTexture1MultiplierAndAffect = GpuProgramManager.RegisterUniform( "u_environmentTexture1MultiplierAndAffect", UniformType.Vector4, 1 );
		//		//	if( u_environmentTexture2MultiplierAndAffect == null )
		//		//		u_environmentTexture2MultiplierAndAffect = GpuProgramManager.RegisterUniform( "u_environmentTexture2MultiplierAndAffect", UniformType.Vector4, 1 );

		//		//	var rotation1 = texture1.Value.Rotation;
		//		//	var rotation2 = texture2.Value.Rotation;
		//		//	var multiplier1 = texture1.Value.MultiplierAndAffect;
		//		//	var multiplier2 = texture2.Value.MultiplierAndAffect;
		//		//	Bgfx.SetUniform( u_environmentTexture1Rotation.Value, &rotation1, 1 );
		//		//	Bgfx.SetUniform( u_environmentTexture2Rotation.Value, &rotation2, 1 );
		//		//	Bgfx.SetUniform( u_environmentTexture1MultiplierAndAffect.Value, &multiplier1, 1 );
		//		//	Bgfx.SetUniform( u_environmentTexture2MultiplierAndAffect.Value, &multiplier2, 1 );
		//		//}
		//	}

		//	//s_brdfLUT
		//	if( bindBrdfLUT )
		//		BindBrdfLUT( context );

		//	//light mask
		//	if( RenderingSystem.LightMask && lightItem.data.Type != Light.TypeEnum.Ambient )
		//	{
		//		if( lightItem.data.Type == Light.TypeEnum.Point )
		//		{
		//			var texture = lightItem.data.Mask;
		//			if( texture == null || texture.Result.TextureType != ImageComponent.TypeEnum.Cube )
		//				texture = ResourceUtility.WhiteTextureCube;

		//			//!!!!anisotropic? где еще

		//			context.BindTexture( 7/*"s_lightMask"*/,
		//				texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, false );
		//		}
		//		else
		//		{
		//			var texture = lightItem.data.Mask;
		//			if( texture == null || texture.Result.TextureType != ImageComponent.TypeEnum._2D )
		//				texture = ResourceUtility.WhiteTexture2D;

		//			var clamp = lightItem.data.Type == Light.TypeEnum.Spotlight;
		//			context.BindTexture( 7/*"s_lightMask"*/,
		//				texture, clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap,
		//				FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, false );
		//		}
		//	}

		//	//receive shadows
		//	//if( receiveShadows )
		//	{
		//		//Viewport shadowViewport = lightItem.shadowTexture.Result.GetRenderTarget().Viewports[ 0 ];

		//		//shadowMap
		//		{
		//			ImageComponent texture;
		//			if( receiveShadows )
		//			{
		//				texture = lightItem.shadowTexture;
		//			}
		//			else
		//			{
		//				if( lightItem.data.Type == Light.TypeEnum.Point )
		//					texture = nullShadowTextureCube;
		//				else
		//					texture = nullShadowTexture2D;
		//			}

		//			if( RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4 )
		//			{
		//				context.BindTexture( 8/*"g_shadowMap"*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None, 0, false );
		//			}
		//			else
		//			{
		//				context.BindTexture( 8/*"g_shadowMap"*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None, TextureFlags.CompareLessEqual, false );
		//			}

		//			//var textureValue = new ViewportRenderingContext.BindTextureData( 8/*"g_shadowMap"*/,
		//			//	texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear,
		//			//	FilterOption.None );
		//			//textureValue.AdditionalFlags |= TextureFlags.CompareLessEqual;
		//			//context.BindTexture( ref textureValue );
		//		}

		//		//Vector2F shadowBias = new Vector2F( lightItem.data.ShadowBias, lightItem.data.ShadowNormalBias );
		//		//generalContainer.Set( "u_shadowBias", ParameterType.Vector2, 1, &shadowBias, sizeof( Vector2F ) );
		//	}

		//	////set u_environmentBlendingFactor
		//	//if( setUniforms )
		//	//{
		//	//	if( u_environmentBlendingFactor == null )
		//	//		u_environmentBlendingFactor = GpuProgramManager.RegisterUniform( "u_environmentBlendingFactor", UniformType.Vector4, 1 );

		//	//	var value = new Vector4F( environmentBlendingFactor, 0, 0, 0 );
		//	//	Bgfx.SetUniform( u_environmentBlendingFactor.Value, &value, 1 );
		//	//}
		//}

		static bool IsDirectionalAmbientOnlyModeEnabled( ViewportRenderingContext context, out bool prepareShadows )
		{
			if( SystemSettings.LimitedDevice )
			{
				var frameData = context.FrameData;

				if( frameData.LightsInFrustumSorted.Length <= 1 )
				{
					prepareShadows = false;
					return true;
				}

				if( frameData.LightsInFrustumSorted.Length == 2 )
				{
					var lightItem = frameData.Lights[ frameData.LightsInFrustumSorted[ 1 ] ];

					if( lightItem.data.Type == Light.TypeEnum.Directional )
					{
						prepareShadows = lightItem.prepareShadows;
						return true;
					}
				}
			}

			prepareShadows = false;
			return false;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected unsafe virtual void Render3DSceneForwardOpaque( ViewportRenderingContext context, FrameData frameData )
		{
			Viewport viewportOwner = context.Owner;
			var sectorsByDistance = context.SectorsByDistance;// SectorsByDistance.Value;

			var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
			var frameDataRenderSceneDataBillboards = frameData.RenderSceneData.Billboards;
			var frameDataMeshes = frameData.Meshes;
			var frameDataBillboards = frameData.Billboards;

			using( var renderableGroupsToDraw = new OpenListNative<RenderableGroupWithDistance>( frameData.RenderableGroupsInFrustum.Count ) )
			{
				foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
				{
					bool add = false;

					if( renderableGroup.X == 0 )
					{
						ref var data = ref frameDataMeshes.Data[ renderableGroup.Y ];
						if( ( data.Flags & FrameData.MeshItem.FlagsEnum.UseForwardOpaque ) != 0 )
						{
							ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];
							if( !meshItem.OnlyForShadowGeneration )
								add = true;
						}
						//add = ( data.Flags & FrameData.MeshItem.FlagsEnum.UseForwardOpaque ) != 0;
					}
					else
					{
						ref var data = ref frameDataBillboards.Data[ renderableGroup.Y ];
						add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseForwardOpaque ) != 0;
					}

					if( add )
					{
						var item = new RenderableGroupWithDistance();
						item.RenderableGroup = renderableGroup;
						item.DistanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.RenderableGroup );
						renderableGroupsToDraw.Add( ref item );
					}
				}

				if( renderableGroupsToDraw.Count == 0 )
					return;

				//sort by distance
				CollectionUtility.MergeSortUnmanaged( renderableGroupsToDraw.Data, renderableGroupsToDraw.Count, delegate ( RenderableGroupWithDistance* a, RenderableGroupWithDistance* b )
				{
					if( a->DistanceSquared < b->DistanceSquared )
						return -1;
					if( a->DistanceSquared > b->DistanceSquared )
						return 1;
					return 0;
				}, true );


				//bind textures for all render operations
				BindBrdfLUT( context );
				BindMaterialsTexture( context, frameData );
				BindBonesTexture( context, frameData );
				BindSamplersForTextureOnlySlots( context, true, false );
				BindMaterialData( context, null, false, false );
				BindForwardLightAndShadows( context, frameData );

				var directionalAmbientOnly = IsDirectionalAmbientOnlyModeEnabled( context, out var directionalAmbientOnlyPrepareShadows );

				{
					int sectorCount = sectorsByDistance;// nLightsInFrustumSorted == 0 ? sectorsByDistance : 1;

					//prepare outputInstancingManagers
					Parallel.For( 0, sectorCount, delegate ( int nSector )
					{
						var manager = outputInstancingManagers[ nSector ];

						int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorCount );
						int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorCount );
						if( nSector == sectorCount - 1 )
							indexTo = renderableGroupsToDraw.Count;

						//fill output manager
						for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
						{
							var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

							if( renderableGroup.RenderableGroup.X == 0 )
							{
								//meshes

								ref var meshItem2 = ref frameDataMeshes.Data[ renderableGroup.RenderableGroup.Y ];

								ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.RenderableGroup.Y ];
								var meshData = meshItem.MeshData;

								if( !meshItem.OnlyForShadowGeneration )
								{
									for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
									{
										var oper = meshData.RenderOperations[ nOperation ];
										foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, false ) )
										{
											bool add = !materialData.Transparent;
											if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
												add = false;
											if( add )
											{
												if( materialData.AllPasses.Count != 0 )//&& (int)lightItem.data.Type < materialData.passesByLightType.Length )
												{
													bool instancing = Instancing && meshItem.AnimationData == null && !meshItem.InstancingEnabled && meshItem.CutVolumes == null && meshItem.ObjectInstanceParameters == null && !meshItem.Tessellation;
													manager.Add( renderableGroup.RenderableGroup, nOperation, oper, materialData, instancing );
												}
											}
										}
									}
								}
							}
							else if( renderableGroup.RenderableGroup.X == 1 )
							{
								//billboards

								ref var billboardItem2 = ref frameDataBillboards.Data[ renderableGroup.RenderableGroup.Y ];

								ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup.RenderableGroup.Y ];
								var meshData = Billboard.GetBillboardMesh().Result.MeshData;

								for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
								{
									var oper = meshData.RenderOperations[ nOperation ];
									foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, false ) )
									{
										bool add = !materialData.Transparent;
										if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
											add = false;
										if( add )
										{
											if( materialData.AllPasses.Count != 0 )//&& (int)lightItem.data.Type < materialData.passesByLightType.Length )
											{
												//!!!!или если уже собран из GroupOfObjects
												bool instancing = Instancing && billboardItem.CutVolumes == null && billboardItem.MaterialInstanceParameters == null;
												manager.Add( renderableGroup.RenderableGroup, nOperation, oper, materialData, instancing );
											}
										}
									}
								}
							}
						}

						manager.Prepare();
					} );

					//push to GPU
					for( int nSector = 0; nSector < sectorCount; nSector++ )
					{
						var manager = outputInstancingManagers[ nSector ];

						int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorCount );
						int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorCount );
						if( nSector == sectorCount - 1 )
							indexTo = renderableGroupsToDraw.Count;

						//render output items
						{
							var outputItems = manager.outputItems;
							for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
							{
								ref var outputItem = ref outputItems.Data[ nOutputItem ];
								var materialData = outputItem.materialData;
								var outputItemoperation = outputItem.operation;
								var outputItemoperationVoxelDataInfo = outputItemoperation.VoxelDataInfo;
								var voxelRendering = outputItemoperationVoxelDataInfo != null;

								var pass = materialData.GetForwardShadingPass( directionalAmbientOnly, directionalAmbientOnlyPrepareShadows, voxelRendering );
								//var pass = materialData.forwardShadingPass;

								//var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
								//bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

								//GpuMaterialPass pass;
								//if( receiveShadows )
								//{
								//	pass = passesGroup.passWithShadows;
								//	//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
								//	//	pass = passesGroup.passWithShadowsHigh;
								//	//else
								//	//	pass = passesGroup.passWithShadowsLow;
								//}
								//else
								//	pass = passesGroup.passWithoutShadows;

								if( Instancing && outputItem.renderableItemsCount >= 2 )//InstancingMinCount )
								{
									//with instancing

									//bind material data
									BindMaterialData( context, materialData, false, voxelRendering );
									BindSamplersForTextureOnlySlots( context, false, voxelRendering );

									//bind operation data
									var firstRenderableItem = outputItem.renderableItemFirst;
									if( firstRenderableItem.X == 0 )
									{
										//meshes
										ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ firstRenderableItem.Y ];
										var meshData = meshItem.MeshData;

										ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, true );

										BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItemoperation.UnwrappedUV, ref meshItem.Color, outputItemoperation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, null, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );
									}
									else if( firstRenderableItem.X == 1 )
									{
										//billboards
										ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ firstRenderableItem.Y ];
										var meshData = Billboard.GetBillboardMesh().Result.MeshData;

										ForwardBindGeneralTexturesUniforms( context, frameData/*, ref billboardItem.BoundingSphere, lightItem, receiveShadows*/, true );

										BindRenderOperationData( context, frameData, materialData, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, outputItemoperation.UnwrappedUV, ref billboardItem.Color, outputItemoperation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, null, 0, ref vector3FZero );
									}

									int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
									int instanceCount = outputItem.renderableItemsCount;

									var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
									if( instanceBuffer.Valid )
									{
										//get instancing matrices
										RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
										int currentMatrix = 0;
										for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
										{
											var renderableItem = outputItem.renderableItems[ nRenderableItem ];

											if( renderableItem.X == 0 )
											{
												//meshes
												ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
												meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
											}
											else if( renderableItem.X == 1 )
											{
												//billboards
												ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
												billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
											}
										}

										RenderOperation( context, outputItemoperation, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
									}
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
										BindMaterialData( context, materialData, false, voxelRendering );
										BindSamplersForTextureOnlySlots( context, false, voxelRendering );

										//bind render operation data, set matrix
										if( renderableItem.X == 0 )
										{
											//meshes
											ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
											var meshData = meshItem.MeshData;

											ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, nRenderableItem == 0 );

											BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItemoperation.UnwrappedUV, ref meshItem.Color, outputItemoperation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, meshItem.ObjectInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

											if( !meshItem.InstancingEnabled )
												fixed( Matrix4F* p = &meshItem.TransformRelative )
													Bgfx.SetTransform( (float*)p );

											var tessItem = meshItem.Tessellation ? TessellationGetItem( context, outputItemoperation, materialData ) : null;

											RenderOperation( context, outputItemoperation, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount, tessItem );
										}
										else if( renderableItem.X == 1 )
										{
											//billboards
											ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
											var meshData = Billboard.GetBillboardMesh().Result.MeshData;

											ForwardBindGeneralTexturesUniforms( context, frameData/*, ref billboardItem.BoundingSphere, lightItem, receiveShadows*/, nRenderableItem == 0 );

											BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, outputItemoperation.UnwrappedUV, ref billboardItem.Color, outputItemoperation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, billboardItem.MaterialInstanceParameters, 0, ref vector3FZero );

											billboardItem.GetWorldMatrixRelative( out var worldMatrix );
											Bgfx.SetTransform( (float*)&worldMatrix );

											RenderOperation( context, outputItemoperation, pass, null, billboardItem.CutVolumes );
										}
									}
								}
							}
						}
					}

					//clear outputInstancingManagers
					foreach( var manager in outputInstancingManagers )
						manager.Clear();
				}

				//render layers
				if( DebugDrawLayers )
				{
					for( int nLightsInFrustumSorted = 0; nLightsInFrustumSorted < frameData.LightsInFrustumSorted.Length; nLightsInFrustumSorted++ )
					{
						var lightIndex = frameData.LightsInFrustumSorted[ nLightsInFrustumSorted ];
						var lightItem = frameData.Lights[ lightIndex ];

						int sectorCount = nLightsInFrustumSorted == 0 ? sectorsByDistance : 1;
						for( int nSector = 0; nSector < sectorCount; nSector++ )
						{
							int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorCount );
							int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorCount );
							if( nSector == sectorCount - 1 )
								indexTo = renderableGroupsToDraw.Count;


							for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
							{
								var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

								if( renderableGroup.RenderableGroup.X == 0 )
								{
									//meshes

									ref var meshItem2 = ref frameDataMeshes.Data[ renderableGroup.RenderableGroup.Y ];

									if( ( meshItem2.Flags & FrameData.MeshItem.FlagsEnum.ContainsForwardOpaqueLayersOnOpaqueBaseObjects ) != 0 )
									{
										ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.RenderableGroup.Y ];
										var meshData = meshItem.MeshData;

										for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
										{
											ref var layer = ref meshItem.Layers[ nLayer ];
											foreach( var materialData in GetLayerMaterialData( ref layer, true, false ) )
											{
												bool add = !materialData.Transparent;
												if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
													add = false;
												if( add )
												{
													if( materialData.AllPasses.Count != 0 )
													{
														//bind material data

														var pass = materialData.GetForwardShadingPass( directionalAmbientOnly, directionalAmbientOnlyPrepareShadows, false );
														//var pass = materialData.forwardShadingPass;

														//var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
														//bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

														//GpuMaterialPass pass;
														//if( receiveShadows )
														//{
														//	pass = passesGroup.passWithShadows;
														//	//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
														//	//	pass = passesGroup.passWithShadowsHigh;
														//	//else
														//	//	pass = passesGroup.passWithShadowsLow;
														//}
														//else
														//	pass = passesGroup.passWithoutShadows;

														ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, true );

														if( !meshItem.InstancingEnabled )
															fixed( Matrix4F* p = &meshItem.TransformRelative )
																Bgfx.SetTransform( (float*)p );

														var color = /*meshItem.Color * */ layer.MaterialColor;

														for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
														{
															var oper = meshData.RenderOperations[ nOperation ];
															var voxelRendering = oper.VoxelDataInfo != null;

															BindMaterialData( context, materialData, false, voxelRendering );
															BindSamplersForTextureOnlySlots( context, false, oper.VoxelDataInfo != null );
															materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );

															BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, oper.VoxelDataImage, oper.VoxelDataInfo, null, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative, layer.UVScale );

															RenderOperation( context, oper, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount );
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

			Bgfx.Discard( DiscardFlags.All );
		}

		//[MethodImpl( (MethodImplOptions)512 )]
		//protected unsafe virtual void Render3DSceneForwardOpaque( ViewportRenderingContext context, FrameData frameData )
		//{
		//	Viewport viewportOwner = context.Owner;
		//	var sectorsByDistance = SectorsByDistance.Value;

		//	var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
		//	var frameDataRenderSceneDataBillboards = frameData.RenderSceneData.Billboards;
		//	var frameDataMeshes = frameData.Meshes;
		//	var frameDataBillboards = frameData.Billboards;

		//	using( var renderableGroupsToDraw = new OpenListNative<RenderableGroupWithDistance>( frameData.RenderableGroupsInFrustum.Count ) )
		//	{
		//		foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
		//		{
		//			bool add = false;

		//			if( renderableGroup.X == 0 )
		//			{
		//				ref var data = ref frameDataMeshes.Data[ renderableGroup.Y ];
		//				if( ( data.Flags & FrameData.MeshItem.FlagsEnum.UseForwardOpaque ) != 0 )
		//				{
		//					ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];
		//					if( !meshItem.OnlyForShadowGeneration )
		//						add = true;
		//				}
		//				//add = ( data.Flags & FrameData.MeshItem.FlagsEnum.UseForwardOpaque ) != 0;
		//			}
		//			else
		//			{
		//				ref var data = ref frameDataBillboards.Data[ renderableGroup.Y ];
		//				add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseForwardOpaque ) != 0;
		//			}

		//			if( add )
		//			{
		//				var item = new RenderableGroupWithDistance();
		//				item.RenderableGroup = renderableGroup;
		//				item.DistanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.RenderableGroup );
		//				renderableGroupsToDraw.Add( ref item );
		//			}
		//		}

		//		if( renderableGroupsToDraw.Count == 0 )
		//			return;

		//		//sort by distance
		//		CollectionUtility.MergeSortUnmanaged( renderableGroupsToDraw.Data, renderableGroupsToDraw.Count, delegate ( RenderableGroupWithDistance* a, RenderableGroupWithDistance* b )
		//		{
		//			if( a->DistanceSquared < b->DistanceSquared )
		//				return -1;
		//			if( a->DistanceSquared > b->DistanceSquared )
		//				return 1;
		//			return 0;
		//		}, true );


		//		//bind textures for all render operations
		//		BindBrdfLUT( context );
		//		BindMaterialsTexture( context, frameData );
		//		BindBonesTexture( context, frameData );
		//		BindSamplersForTextureOnlySlots( context, true, false );
		//		BindMaterialData( context, null, false, false );


		//		LightItem lightItemBinded = null;

		//		for( int nLightsInFrustumSorted = 0; nLightsInFrustumSorted < frameData.LightsInFrustumSorted.Length; nLightsInFrustumSorted++ )
		//		{
		//			var lightIndex = frameData.LightsInFrustumSorted[ nLightsInFrustumSorted ];
		//			var lightItem = frameData.Lights[ lightIndex ];

		//			int sectorCount = nLightsInFrustumSorted == 0 ? sectorsByDistance : 1;


		//			//prepare outputInstancingManagers
		//			Parallel.For( 0, sectorCount, delegate ( int nSector )
		//			{
		//				var manager = outputInstancingManagers[ nSector ];

		//				int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorCount );
		//				int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorCount );
		//				if( nSector == sectorCount - 1 )
		//					indexTo = renderableGroupsToDraw.Count;

		//				//fill output manager
		//				for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
		//				{
		//					var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

		//					if( renderableGroup.RenderableGroup.X == 0 )
		//					{
		//						//meshes

		//						ref var meshItem2 = ref frameDataMeshes.Data[ renderableGroup.RenderableGroup.Y ];

		//						bool affectedByLight;
		//						var lightType = lightItem.data.Type;
		//						if( lightType == Light.TypeEnum.Point || lightType == Light.TypeEnum.Spotlight )
		//							affectedByLight = meshItem2.ContainsPointOrSpotLight( lightIndex );
		//						else
		//							affectedByLight = true;

		//						if( affectedByLight )
		//						{
		//							ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.RenderableGroup.Y ];
		//							var meshData = meshItem.MeshData;

		//							if( !meshItem.OnlyForShadowGeneration )
		//							{
		//								for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//								{
		//									var oper = meshData.RenderOperations[ nOperation ];
		//									foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, false ) )
		//									{
		//										bool add = !materialData.Transparent;
		//										if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//											add = false;
		//										if( add )
		//										{
		//											if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		//											{
		//												bool instancing = Instancing && meshItem.AnimationData == null && !meshItem.InstancingEnabled && meshItem.CutVolumes == null && meshItem.MaterialInstanceParameters == null;
		//												manager.Add( renderableGroup.RenderableGroup, nOperation, oper, materialData, instancing );
		//											}
		//										}
		//									}
		//								}
		//							}
		//						}
		//					}
		//					else if( renderableGroup.RenderableGroup.X == 1 )
		//					{
		//						//billboards

		//						ref var billboardItem2 = ref frameDataBillboards.Data[ renderableGroup.RenderableGroup.Y ];

		//						bool affectedByLight;
		//						var lightType = lightItem.data.Type;
		//						if( lightType == Light.TypeEnum.Point || lightType == Light.TypeEnum.Spotlight )
		//							affectedByLight = billboardItem2.ContainsPointOrSpotLight( lightIndex );
		//						else
		//							affectedByLight = true;

		//						if( affectedByLight )
		//						{
		//							ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup.RenderableGroup.Y ];
		//							var meshData = Billboard.GetBillboardMesh().Result.MeshData;

		//							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//							{
		//								var oper = meshData.RenderOperations[ nOperation ];
		//								foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, false ) )
		//								{
		//									bool add = !materialData.Transparent;
		//									if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//										add = false;
		//									if( add )
		//									{
		//										if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		//										{
		//											//!!!!или если уже собран из GroupOfObjects
		//											bool instancing = Instancing && billboardItem.CutVolumes == null && billboardItem.MaterialInstanceParameters == null;
		//											manager.Add( renderableGroup.RenderableGroup, nOperation, oper, materialData, instancing );
		//										}
		//									}
		//								}
		//							}
		//						}
		//					}
		//				}

		//				manager.Prepare();
		//			} );

		//			//push to GPU
		//			for( int nSector = 0; nSector < sectorCount; nSector++ )
		//			{
		//				var manager = outputInstancingManagers[ nSector ];

		//				int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorCount );
		//				int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorCount );
		//				if( nSector == sectorCount - 1 )
		//					indexTo = renderableGroupsToDraw.Count;

		//				//render output items
		//				{
		//					var outputItems = manager.outputItems;
		//					for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
		//					{
		//						ref var outputItem = ref outputItems.Data[ nOutputItem ];
		//						var materialData = outputItem.materialData;
		//						var outputItemoperation = outputItem.operation;
		//						var outputItemoperationVoxelDataInfo = outputItemoperation.VoxelDataInfo;
		//						var voxelRendering = outputItemoperationVoxelDataInfo != null;

		//						//set lightItem uniforms
		//						if( lightItemBinded != lightItem )
		//						{
		//							lightItem.Bind( this, context );
		//							lightItemBinded = lightItem;
		//						}

		//						var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//						bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//						GpuMaterialPass pass;
		//						if( receiveShadows )
		//						{
		//							pass = passesGroup.passWithShadows;
		//							//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//							//	pass = passesGroup.passWithShadowsHigh;
		//							//else
		//							//	pass = passesGroup.passWithShadowsLow;
		//						}
		//						else
		//							pass = passesGroup.passWithoutShadows;

		//						if( Instancing && outputItem.renderableItemsCount >= 2 )//InstancingMinCount )
		//						{
		//							//with instancing

		//							//bind material data
		//							BindMaterialData( context, materialData, false, voxelRendering );
		//							BindSamplersForTextureOnlySlots( context, false, voxelRendering );

		//							//bind operation data
		//							var firstRenderableItem = outputItem.renderableItemFirst;
		//							if( firstRenderableItem.X == 0 )
		//							{
		//								//meshes
		//								ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ firstRenderableItem.Y ];
		//								var meshData = meshItem.MeshData;

		//								ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

		//								BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItemoperation.UnwrappedUV, ref meshItem.Color, outputItemoperation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, null, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );
		//							}
		//							else if( firstRenderableItem.X == 1 )
		//							{
		//								//billboards
		//								ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ firstRenderableItem.Y ];
		//								var meshData = Billboard.GetBillboardMesh().Result.MeshData;

		//								ForwardBindGeneralTexturesUniforms( context, frameData, ref billboardItem.BoundingSphere, lightItem, receiveShadows, true, false );

		//								BindRenderOperationData( context, frameData, materialData, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, outputItemoperation.UnwrappedUV, ref billboardItem.Color, outputItemoperation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, null, 0, ref vector3FZero );
		//							}

		//							int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
		//							int instanceCount = outputItem.renderableItemsCount;

		//							var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
		//							if( instanceBuffer.Valid )
		//							{
		//								//get instancing matrices
		//								RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
		//								int currentMatrix = 0;
		//								for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
		//								{
		//									var renderableItem = outputItem.renderableItems[ nRenderableItem ];

		//									if( renderableItem.X == 0 )
		//									{
		//										//meshes
		//										ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
		//										meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
		//									}
		//									else if( renderableItem.X == 1 )
		//									{
		//										//billboards
		//										ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
		//										billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
		//									}
		//								}

		//								RenderOperation( context, outputItemoperation, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
		//							}
		//						}
		//						else
		//						{
		//							//without instancing

		//							for( int nRenderableItem = 0; nRenderableItem < outputItem.renderableItemsCount; nRenderableItem++ )
		//							{
		//								Vector3I renderableItem;
		//								if( nRenderableItem != 0 )
		//									renderableItem = outputItem.renderableItems[ nRenderableItem ];
		//								else
		//									renderableItem = outputItem.renderableItemFirst;

		//								//bind material data
		//								BindMaterialData( context, materialData, false, voxelRendering );
		//								BindSamplersForTextureOnlySlots( context, false, voxelRendering );

		//								//bind render operation data, set matrix
		//								if( renderableItem.X == 0 )
		//								{
		//									//meshes
		//									ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
		//									var meshData = meshItem.MeshData;

		//									ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, nRenderableItem == 0, false );

		//									BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItemoperation.UnwrappedUV, ref meshItem.Color, outputItemoperation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, meshItem.MaterialInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

		//									if( !meshItem.InstancingEnabled )
		//										fixed( Matrix4F* p = &meshItem.TransformRelative )
		//											Bgfx.SetTransform( (float*)p );

		//									RenderOperation( context, outputItemoperation, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount );
		//								}
		//								else if( renderableItem.X == 1 )
		//								{
		//									//billboards
		//									ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
		//									var meshData = Billboard.GetBillboardMesh().Result.MeshData;

		//									ForwardBindGeneralTexturesUniforms( context, frameData, ref billboardItem.BoundingSphere, lightItem, receiveShadows, nRenderableItem == 0, false );

		//									BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, outputItemoperation.UnwrappedUV, ref billboardItem.Color, outputItemoperation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, billboardItem.MaterialInstanceParameters, 0, ref vector3FZero );

		//									billboardItem.GetWorldMatrixRelative( out var worldMatrix );
		//									Bgfx.SetTransform( (float*)&worldMatrix );

		//									RenderOperation( context, outputItemoperation, pass, null, billboardItem.CutVolumes );
		//								}
		//							}
		//						}
		//					}
		//				}
		//			}

		//			//clear outputInstancingManagers
		//			foreach( var manager in outputInstancingManagers )
		//				manager.Clear();
		//		}

		//		//render layers
		//		if( DebugDrawLayers )
		//		{
		//			for( int nLightsInFrustumSorted = 0; nLightsInFrustumSorted < frameData.LightsInFrustumSorted.Length; nLightsInFrustumSorted++ )
		//			{
		//				var lightIndex = frameData.LightsInFrustumSorted[ nLightsInFrustumSorted ];
		//				var lightItem = frameData.Lights[ lightIndex ];

		//				int sectorCount = nLightsInFrustumSorted == 0 ? sectorsByDistance : 1;
		//				for( int nSector = 0; nSector < sectorCount; nSector++ )
		//				{
		//					int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorCount );
		//					int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorCount );
		//					if( nSector == sectorCount - 1 )
		//						indexTo = renderableGroupsToDraw.Count;


		//					for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
		//					{
		//						var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

		//						if( renderableGroup.RenderableGroup.X == 0 )
		//						{
		//							//meshes

		//							ref var meshItem2 = ref frameDataMeshes.Data[ renderableGroup.RenderableGroup.Y ];

		//							if( ( meshItem2.Flags & FrameData.MeshItem.FlagsEnum.ContainsForwardOpaqueLayersOnOpaqueBaseObjects ) != 0 )
		//							{
		//								bool affectedByLight;
		//								var lightType = lightItem.data.Type;
		//								if( lightType == Light.TypeEnum.Point || lightType == Light.TypeEnum.Spotlight )
		//									affectedByLight = meshItem2.ContainsPointOrSpotLight( lightIndex );
		//								else
		//									affectedByLight = true;

		//								if( affectedByLight )
		//								{
		//									ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.RenderableGroup.Y ];
		//									var meshData = meshItem.MeshData;

		//									for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
		//									{
		//										ref var layer = ref meshItem.Layers[ nLayer ];
		//										foreach( var materialData in GetLayerMaterialData( ref layer, true, false ) )
		//										{
		//											bool add = !materialData.Transparent;
		//											if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//												add = false;
		//											if( add )
		//											{
		//												if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		//												{
		//													//set lightItem uniforms
		//													if( lightItemBinded != lightItem )
		//													{
		//														lightItem.Bind( this, context );
		//														lightItemBinded = lightItem;
		//													}

		//													//bind material data

		//													var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//													bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//													GpuMaterialPass pass;
		//													if( receiveShadows )
		//													{
		//														pass = passesGroup.passWithShadows;
		//														//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//														//	pass = passesGroup.passWithShadowsHigh;
		//														//else
		//														//	pass = passesGroup.passWithShadowsLow;
		//													}
		//													else
		//														pass = passesGroup.passWithoutShadows;

		//													ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

		//													if( !meshItem.InstancingEnabled )
		//														fixed( Matrix4F* p = &meshItem.TransformRelative )
		//															Bgfx.SetTransform( (float*)p );

		//													var color = /*meshItem.Color * */ layer.MaterialColor;

		//													for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//													{
		//														var oper = meshData.RenderOperations[ nOperation ];
		//														var voxelRendering = oper.VoxelDataInfo != null;

		//														BindMaterialData( context, materialData, false, voxelRendering );
		//														BindSamplersForTextureOnlySlots( context, false, oper.VoxelDataInfo != null );
		//														materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );

		//														BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, oper.VoxelDataImage, oper.VoxelDataInfo, null, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

		//														RenderOperation( context, oper, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount );
		//													}

		//												}
		//											}
		//										}
		//									}
		//								}
		//							}
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}
		//}


		[MethodImpl( (MethodImplOptions)512 )]
		protected unsafe virtual void RenderTransparentLayersOnOpaqueBaseObjectsForward( ViewportRenderingContext context, FrameData frameData )
		{
			Viewport viewportOwner = context.Owner;

			var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
			var frameDataRenderSceneDataBillboards = frameData.RenderSceneData.Billboards;
			var frameDataMeshes = frameData.Meshes;
			var frameDataBillboards = frameData.Billboards;

			using( var renderableGroupsToDraw = new OpenListNative<Vector2I>( frameData.RenderableGroupsInFrustum.Count ) )
			{
				foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
				{
					bool add = false;

					if( renderableGroup.X == 0 )
					{
						ref var data = ref frameDataMeshes.Data[ renderableGroup.Y ];
						add = ( data.Flags & FrameData.MeshItem.FlagsEnum.ContainsTransparentLayersOnOpaqueBaseObjects ) != 0;
					}
					//else
					//{
					//	ref var data = ref frameData.Billboards.Data[ renderableGroup.Y ];
					//	add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.ContainsTransparentLayersOnOpaqueBaseObjects ) != 0;
					//}

					if( add )
						renderableGroupsToDraw.Add( renderableGroup );
				}

				if( renderableGroupsToDraw.Count == 0 )
					return;

				//!!!!slowly

				//sort by distance
				CollectionUtility.MergeSortUnmanaged( renderableGroupsToDraw.Data, renderableGroupsToDraw.Count, delegate ( Vector2I a, Vector2I b )
				{
					var distanceASquared = frameData.GetObjectGroupDistanceToCameraSquared( ref a );
					var distanceBSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref b );
					if( distanceASquared > distanceBSquared )
						return -1;
					if( distanceASquared < distanceBSquared )
						return 1;
					return 0;
				}, true );


				//bind textures for all render operations
				BindBrdfLUT( context );
				BindMaterialsTexture( context, frameData );
				BindBonesTexture( context, frameData );
				BindSamplersForTextureOnlySlots( context, true, false );
				BindMaterialData( context, null, false, false );
				BindForwardLightAndShadows( context, frameData );

				var directionalAmbientOnly = IsDirectionalAmbientOnlyModeEnabled( context, out var directionalAmbientOnlyPrepareShadows );

				//draw
				foreach( var renderableGroup in renderableGroupsToDraw )
				{
					if( renderableGroup.X == 0 )
					{
						//Mesh rendering

						ref var meshItem2 = ref frameDataMeshes.Data[ renderableGroup.Y ];
						ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];
						var meshData = meshItem.MeshData;

						//render layers
						if( meshItem.Layers != null )
						{
							for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
							{
								ref var layer = ref meshItem.Layers[ nLayer ];
								foreach( var materialData in GetLayerMaterialData( ref layer, true, false ) )
								{
									//if( materialData == null )
									//	continue;

									bool add = materialData.Transparent;
									if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
										add = false;
									if( !add )
										continue;

									{
										if( materialData.AllPasses.Count != 0 )
										{
											var pass = materialData.GetForwardShadingPass( directionalAmbientOnly, directionalAmbientOnlyPrepareShadows, false );
											//var pass = materialData.forwardShadingPass;

											//var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
											//bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

											//GpuMaterialPass pass;
											//if( receiveShadows )
											//{
											//	pass = passesGroup.passWithShadows;
											//	//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
											//	//	pass = passesGroup.passWithShadowsHigh;
											//	//else
											//	//	pass = passesGroup.passWithShadowsLow;
											//}
											//else
											//	pass = passesGroup.passWithoutShadows;

											ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, true );

											//bool materialWasChanged = materialBinded != materialData;
											//materialBinded = materialData;

											fixed( Matrix4F* p = &meshItem.TransformRelative )
												Bgfx.SetTransform( (float*)p );

											var color = /*meshItem.Color * */ layer.MaterialColor;

											for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
											{
												var oper = meshData.RenderOperations[ nOperation ];
												var voxelRendering = oper.VoxelDataInfo != null;

												//bind material data
												BindMaterialData( context, materialData, false, voxelRendering );
												BindSamplersForTextureOnlySlots( context, false, voxelRendering );
												materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );

												BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, oper.VoxelDataImage, oper.VoxelDataInfo, meshItem.ObjectInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative, layer.UVScale );

												RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
											}
										}
									}
								}
							}
						}

					}
				}
			}

			Bgfx.Discard( DiscardFlags.All );
		}

		//[MethodImpl( (MethodImplOptions)512 )]
		//protected unsafe virtual void RenderTransparentLayersOnOpaqueBaseObjectsForward( ViewportRenderingContext context, FrameData frameData )
		//{
		//	Viewport viewportOwner = context.Owner;

		//	var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
		//	var frameDataRenderSceneDataBillboards = frameData.RenderSceneData.Billboards;
		//	var frameDataMeshes = frameData.Meshes;
		//	var frameDataBillboards = frameData.Billboards;

		//	using( var renderableGroupsToDraw = new OpenListNative<Vector2I>( frameData.RenderableGroupsInFrustum.Count ) )
		//	{
		//		foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
		//		{
		//			bool add = false;

		//			if( renderableGroup.X == 0 )
		//			{
		//				ref var data = ref frameDataMeshes.Data[ renderableGroup.Y ];
		//				add = ( data.Flags & FrameData.MeshItem.FlagsEnum.ContainsTransparentLayersOnOpaqueBaseObjects ) != 0;
		//			}
		//			//else
		//			//{
		//			//	ref var data = ref frameData.Billboards.Data[ renderableGroup.Y ];
		//			//	add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.ContainsTransparentLayersOnOpaqueBaseObjects ) != 0;
		//			//}

		//			if( add )
		//				renderableGroupsToDraw.Add( renderableGroup );
		//		}

		//		if( renderableGroupsToDraw.Count == 0 )
		//			return;

		//		//!!!!slowly

		//		//sort by distance
		//		CollectionUtility.MergeSortUnmanaged( renderableGroupsToDraw.Data, renderableGroupsToDraw.Count, delegate ( Vector2I a, Vector2I b )
		//		{
		//			var distanceASquared = frameData.GetObjectGroupDistanceToCameraSquared( ref a );
		//			var distanceBSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref b );
		//			if( distanceASquared > distanceBSquared )
		//				return -1;
		//			if( distanceASquared < distanceBSquared )
		//				return 1;
		//			return 0;
		//		}, true );

		//		LightItem lightItemBinded = null;
		//		Material.CompiledMaterialData materialBinded = null;

		//		int ambientDirectionalLightCount = 0;
		//		foreach( var lightIndex in frameData.LightsInFrustumSorted )
		//		{
		//			var lightItem = frameData.Lights[ lightIndex ];
		//			if( lightItem.data.Type == Light.TypeEnum.Point || lightItem.data.Type == Light.TypeEnum.Spotlight )
		//				break;
		//			ambientDirectionalLightCount++;
		//		}

		//		int[] tempIntArray = Array.Empty<int>();
		//		int[] GetTempIntArray( int minSize )
		//		{
		//			if( tempIntArray.Length < minSize )
		//				tempIntArray = new int[ minSize ];
		//			return tempIntArray;
		//		}


		//		//bind textures for all render operations
		//		BindBrdfLUT( context );
		//		BindMaterialsTexture( context, frameData );
		//		BindBonesTexture( context, frameData );
		//		BindSamplersForTextureOnlySlots( context, true, false );
		//		BindMaterialData( context, null, false, false );


		//		//draw
		//		foreach( var renderableGroup in renderableGroupsToDraw )
		//		{
		//			if( renderableGroup.X == 0 )
		//			{
		//				//Mesh rendering

		//				ref var meshItem2 = ref frameDataMeshes.Data[ renderableGroup.Y ];
		//				ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];
		//				var meshData = meshItem.MeshData;

		//				//!!!!не всегда нужно
		//				var affectedLightsCount = ambientDirectionalLightCount + meshItem2.PointSpotLightCount;
		//				var affectedLights = GetTempIntArray( affectedLightsCount );//var affectedLights = stackalloc int[ affectedLightsCount ];
		//				for( int n = 0; n < ambientDirectionalLightCount; n++ )
		//					affectedLights[ n ] = frameData.LightsInFrustumSorted[ n ];
		//				for( int n = 0; n < meshItem2.PointSpotLightCount; n++ )
		//					affectedLights[ ambientDirectionalLightCount + n ] = meshItem2.GetPointSpotLight( n );

		//				//render layers
		//				if( meshItem.Layers != null )
		//				{
		//					for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
		//					{
		//						ref var layer = ref meshItem.Layers[ nLayer ];
		//						foreach( var materialData in GetLayerMaterialData( ref layer, true, false ) )
		//						{
		//							//if( materialData == null )
		//							//	continue;

		//							bool add = materialData.Transparent;
		//							if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//								add = false;
		//							if( !add )
		//								continue;

		//							for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
		//							{
		//								var lightIndex = affectedLights[ nAffectedLightIndex ];
		//								var lightItem = frameData.Lights[ lightIndex ];

		//								if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		//								{
		//									//set lightItem uniforms
		//									if( lightItemBinded != lightItem )
		//									{
		//										lightItem.Bind( this, context );
		//										lightItemBinded = lightItem;
		//									}

		//									var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//									bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//									GpuMaterialPass pass;
		//									if( receiveShadows )
		//									{
		//										pass = passesGroup.passWithShadows;
		//										//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//										//	pass = passesGroup.passWithShadowsHigh;
		//										//else
		//										//	pass = passesGroup.passWithShadowsLow;
		//									}
		//									else
		//										pass = passesGroup.passWithoutShadows;

		//									ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

		//									bool materialWasChanged = materialBinded != materialData;
		//									materialBinded = materialData;

		//									fixed( Matrix4F* p = &meshItem.TransformRelative )
		//										Bgfx.SetTransform( (float*)p );

		//									var color = /*meshItem.Color * */ layer.MaterialColor;

		//									for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//									{
		//										var oper = meshData.RenderOperations[ nOperation ];
		//										var voxelRendering = oper.VoxelDataInfo != null;

		//										//bind material data
		//										BindMaterialData( context, materialData, false, voxelRendering );
		//										BindSamplersForTextureOnlySlots( context, false, voxelRendering );
		//										materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );

		//										BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, oper.VoxelDataImage, oper.VoxelDataInfo, meshItem.MaterialInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

		//										RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
		//									}
		//								}
		//							}
		//						}
		//					}
		//				}

		//			}
		//		}
		//	}
		//}

		[MethodImpl( (MethodImplOptions)512 )]
		void ApplyTransparentRenderingAddOffsetWhenSortByDistance( FrameData frameData, ref RenderableGroupWithDistance item )
		{
			var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
			//var frameDataRenderSceneDataBillboards = frameData.RenderSceneData.Billboards;

			bool applied = false;

			//!!!!slowly. много раз вызывается при сортировке

			//apply TransparentRenderingAddOffsetWhenSortByDistance
			switch( item.RenderableGroup.X )
			{
			case 0:
				if( frameDataRenderSceneDataMeshes.Data[ item.RenderableGroup.Y ].TransparentRenderingAddOffsetWhenSortByDistance )
				{
					//!!!!good?
					item.DistanceSquared += 10000000;
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
					if( frameData.GetObjectGroupBoundingBoxCenter( ref item.RenderableGroup, out var center ) )
					{
						for( int n = 0; n < list.Count; n++ )
						{
							ref var box = ref list.Data[ n ].Box;

							if( box.Contains( ref center ) )
							{
								//!!!!good?
								item.DistanceSquared += 20000000;
								break;
							}
						}
					}
				}
			}
		}



		//!!!!not finished, something wrong with writing to image/buffer from pixel shader. implement oit on dx12, vulkan level
		//[MethodImpl( (MethodImplOptions)512 )]
		//protected unsafe virtual void Render3DSceneForwardTransparentWithOIT( ViewportRenderingContext context, FrameData frameData, ImageComponent colorDepthTextureCopy, MultiRenderTarget sceneMotionDepthMRT, ref Matrix4F viewMatrix, ref Matrix4F projectionMatrix, ImageComponent sceneTexture )
		//{
		//	var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
		//	var frameDataRenderSceneDataBillboards = frameData.RenderSceneData.Billboards;
		//	var frameDataMeshes = frameData.Meshes;
		//	var frameDataBillboards = frameData.Billboards;

		//	var viewportOwner = context.Owner;

		//	using( var renderableGroupsToDraw = new OpenListNative<Vector2I>( frameData.RenderableGroupsInFrustum.Count ) )
		//	{
		//		foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
		//		{
		//			bool add = false;

		//			if( renderableGroup.X == 0 )
		//			{
		//				ref var data = ref frameDataMeshes.Data[ renderableGroup.Y ];
		//				add = ( data.Flags & FrameData.MeshItem.FlagsEnum.UseForwardTransparent ) != 0;
		//			}
		//			else
		//			{
		//				ref var data = ref frameDataBillboards.Data[ renderableGroup.Y ];
		//				add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseForwardTransparent ) != 0;
		//			}

		//			if( add )
		//				renderableGroupsToDraw.Add( renderableGroup );
		//		}

		//		if( renderableGroupsToDraw.Count == 0 )
		//			return;



		//		//!!!!максимальный размер

		//		//prepare oit buffers
		//		ImageComponent oitScreen;
		//		ImageComponent oitLists;
		//		{
		//			//!!!!на буферах может быстрее? для буферов нет манагера

		//			//!!!!
		//			var oitScreenSize = context.CurrentViewport.SizeInPixels;
		//			//var oitScreenSize = context.CurrentViewport.SizeInPixels + new Vector2I( 1, 0 );

		//			//oitScreen = context.RenderTarget2D_Alloc( oitScreenSize, PixelFormat.A8B8G8R8 );
		//			//oitScreen = context.RenderTarget2D_Alloc( oitScreenSize, PixelFormat.R32_UInt );

		//			oitScreen = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, ImageComponent.TypeEnum._2D, oitScreenSize, PixelFormat.A8B8G8R8, 0, false );
		//			//oitScreen = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, ImageComponent.TypeEnum._2D, oitScreenSize, PixelFormat.A8R8G8B8, 0, false );

		//			//oitScreen = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, ImageComponent.TypeEnum._2D, oitScreenSize, PixelFormat.A8B8G8R8, 0, false );
		//			//oitScreen = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, ImageComponent.TypeEnum._2D, oitScreenSize, PixelFormat.R32_UInt, 0, false );

		//			//!!!!
		//			var oitListsSize = context.CurrentViewport.SizeInPixels * new Vector2I( 1, 2 );
		//			//oitLists = context.RenderTarget2D_Alloc( oitListsSize, PixelFormat.Float32RGBA );

		//			////!!!!
		//			//var oitListsSize = context.CurrentViewport.SizeInPixels * new Vector2I( 1, 2 );
		//			//oitLists = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, ImageComponent.TypeEnum._2D, oitListsSize, PixelFormat.Float32RGBA, 0, false );

		//			//clear screen buffer
		//			//!!!!
		//			//context.SetViewport( oitScreen.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, ColorValue.Zero );


		//			//var oitBufferVertexDeclaration = new VertexElement[] { new VertexElement( 0, 0, VertexElementType.Float4, VertexElementSemantic.Position ) }.CreateVertexDeclaration( 0 );
		//			//var oitBuffer = GpuBufferManager.CreateVertexBuffer( destVertexCount, oitBufferVertexDeclaration, GpuBufferFlags.ComputeWrite );

		//		}


		//		{
		//			//!!!!temp
		//			context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ] );
		//			//context.SetViewport( sceneMotionDepthMRT.Viewports[ 0 ] );

		//			context.BindComputeImage( 1, oitScreen, 0, ComputeBufferAccessEnum.ReadWrite );

		//			var shader = new CanvasRenderer.ShaderItem();
		//			shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
		//			shader.FragmentProgramFileName = @"Base\Shaders\ComposeOIT_Test_fs.sc";

		//			//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, oitScreen, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

		//			//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, oitLists, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

		//			//context.BindComputeImage( 0, oitScreen, 0, ComputeBufferAccessEnum.ReadWrite );
		//			//context.BindComputeImage( 0, oitScreen, 0, ComputeBufferAccessEnum.Read );

		//			//context.BindComputeImage( 1, oitLists, 0, ComputeBufferAccessEnum.Read );

		//			//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"colorTexture"*/, colorTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
		//			//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"secondTexture"*/, secondTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//			context.RenderQuadToCurrentViewport( shader );

		//		}


		//		//draw scene objects
		//		if( false )
		//		{

		//			//!!!!set invalid
		//			//context.SetViewport( sceneMotionDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix, 0, ColorValue.Zero, setInvalidFrameBuffer: true );
		//			context.SetViewport( sceneMotionDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix, 0, ColorValue.Zero );

		//			//bind textures for all render operations
		//			context.BindTexture( 5/*"s_colorDepthTextureCopy"*/, colorDepthTextureCopy ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None, 0, false );
		//			BindBrdfLUT( context );
		//			BindMaterialsTexture( context, frameData );
		//			BindBonesTexture( context, frameData );
		//			BindSamplersForTextureOnlySlots( context, true, false );
		//			BindMaterialData( context, null, false, false );
		//			BindForwardLightAndShadows( context, frameData );


		//			//!!!!переименовать метод
		//			context.BindComputeImage( 3, oitScreen, 0, ComputeBufferAccessEnum.ReadWrite );
		//			//context.BindComputeImage( 4, oitLists, 0, ComputeBufferAccessEnum.Write );



		//			var directionalAmbientOnly = IsDirectionalAmbientOnlyModeEnabled( context, out var directionalAmbientOnlyPrepareShadows );

		//			{
		//				//more sectors to optimize CPU because parallel, but get more draw calls
		//				int sectorCount = context.SectorsByDistance; //int sectorCount = Math.Min( context.SectorsByDistance, 4 ); //2 );

		//				//prepare outputInstancingManagers
		//				Parallel.For( 0, sectorCount, delegate ( int nSector )
		//				{
		//					var manager = outputInstancingManagers[ nSector ];

		//					int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorCount );
		//					int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorCount );
		//					if( nSector == sectorCount - 1 )
		//						indexTo = renderableGroupsToDraw.Count;

		//					//fill output manager
		//					for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
		//					{
		//						var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

		//						if( renderableGroup.X == 0 )
		//						{
		//							//meshes

		//							ref var meshItem2 = ref frameDataMeshes.Data[ renderableGroup.Y ];

		//							ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];
		//							var meshData = meshItem.MeshData;

		//							if( !meshItem.OnlyForShadowGeneration )
		//							{
		//								for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//								{
		//									var oper = meshData.RenderOperations[ nOperation ];
		//									foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, false ) )
		//									{
		//										if( materialData.Transparent && materialData.AllPasses.Count != 0 )
		//										{
		//											bool instancing = Instancing /*&& InstancingTransparent */&& meshItem.AnimationData == null && !meshItem.InstancingEnabled && meshItem.CutVolumes == null && meshItem.ObjectInstanceParameters == null && !meshItem.Tessellation;
		//											manager.Add( renderableGroup, nOperation, oper, materialData, instancing );
		//										}
		//									}
		//								}
		//							}
		//						}
		//						else if( renderableGroup.X == 1 )
		//						{
		//							//billboards

		//							ref var billboardItem2 = ref frameDataBillboards.Data[ renderableGroup.Y ];

		//							ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup.Y ];
		//							var meshData = Billboard.GetBillboardMesh().Result.MeshData;

		//							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//							{
		//								var oper = meshData.RenderOperations[ nOperation ];
		//								foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, false ) )
		//								{
		//									if( materialData.Transparent && materialData.AllPasses.Count != 0 )
		//									{
		//										//!!!!или если уже собран из GroupOfObjects
		//										bool instancing = Instancing && billboardItem.CutVolumes == null && billboardItem.MaterialInstanceParameters == null;
		//										manager.Add( renderableGroup, nOperation, oper, materialData, instancing );
		//									}
		//								}
		//							}
		//						}
		//					}

		//					manager.Prepare();
		//				} );


		//				//push to GPU
		//				for( int nSector = 0; nSector < sectorCount; nSector++ )
		//				{
		//					var manager = outputInstancingManagers[ nSector ];

		//					int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorCount );
		//					int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorCount );
		//					if( nSector == sectorCount - 1 )
		//						indexTo = renderableGroupsToDraw.Count;

		//					//render output items
		//					{
		//						var outputItems = manager.outputItems;
		//						for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
		//						{
		//							ref var outputItem = ref outputItems.Data[ nOutputItem ];
		//							var materialData = outputItem.materialData;
		//							var outputItemoperation = outputItem.operation;
		//							var outputItemoperationVoxelDataInfo = outputItemoperation.VoxelDataInfo;
		//							var voxelRendering = outputItemoperationVoxelDataInfo != null;

		//							var pass = materialData.GetForwardShadingPass( directionalAmbientOnly, directionalAmbientOnlyPrepareShadows, voxelRendering );

		//							if( Instancing /*&& InstancingTransparent */&& outputItem.renderableItemsCount >= 2 )
		//							{
		//								//with instancing

		//								//bind material data
		//								BindMaterialData( context, materialData, false, voxelRendering );
		//								BindSamplersForTextureOnlySlots( context, false, voxelRendering );

		//								//bind operation data
		//								var firstRenderableItem = outputItem.renderableItemFirst;
		//								if( firstRenderableItem.X == 0 )
		//								{
		//									//meshes
		//									ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ firstRenderableItem.Y ];
		//									var meshData = meshItem.MeshData;

		//									ForwardBindGeneralTexturesUniforms( context, frameData, true, true );

		//									BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItemoperation.UnwrappedUV, ref meshItem.Color, outputItemoperation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, null, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );
		//								}
		//								else if( firstRenderableItem.X == 1 )
		//								{
		//									//billboards
		//									ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ firstRenderableItem.Y ];
		//									var meshData = Billboard.GetBillboardMesh().Result.MeshData;

		//									ForwardBindGeneralTexturesUniforms( context, frameData/*, ref billboardItem.BoundingSphere, lightItem, receiveShadows*/, true, true );

		//									BindRenderOperationData( context, frameData, materialData, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, outputItemoperation.UnwrappedUV, ref billboardItem.Color, outputItemoperation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, null, 0, ref vector3FZero );
		//								}

		//								int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
		//								int instanceCount = outputItem.renderableItemsCount;

		//								var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
		//								if( instanceBuffer.Valid )
		//								{
		//									//get instancing matrices
		//									RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
		//									int currentMatrix = 0;
		//									for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
		//									{
		//										var renderableItem = outputItem.renderableItems[ nRenderableItem ];

		//										if( renderableItem.X == 0 )
		//										{
		//											//meshes
		//											ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
		//											meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
		//										}
		//										else if( renderableItem.X == 1 )
		//										{
		//											//billboards
		//											ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
		//											billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
		//										}
		//									}

		//									RenderOperation( context, outputItemoperation, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
		//								}
		//							}
		//							else
		//							{
		//								//without instancing

		//								for( int nRenderableItem = 0; nRenderableItem < outputItem.renderableItemsCount; nRenderableItem++ )
		//								{
		//									Vector3I renderableItem;
		//									if( nRenderableItem != 0 )
		//										renderableItem = outputItem.renderableItems[ nRenderableItem ];
		//									else
		//										renderableItem = outputItem.renderableItemFirst;

		//									//bind material data
		//									BindMaterialData( context, materialData, false, voxelRendering );
		//									BindSamplersForTextureOnlySlots( context, false, voxelRendering );

		//									//bind render operation data, set matrix
		//									if( renderableItem.X == 0 )
		//									{
		//										//meshes
		//										ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
		//										var meshData = meshItem.MeshData;

		//										ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, nRenderableItem == 0, true );

		//										BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItemoperation.UnwrappedUV, ref meshItem.Color, outputItemoperation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, meshItem.ObjectInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

		//										if( !meshItem.InstancingEnabled )
		//											fixed( Matrix4F* p = &meshItem.TransformRelative )
		//												Bgfx.SetTransform( (float*)p );

		//										var tessItem = meshItem.Tessellation ? TessellationGetItem( context, outputItemoperation, materialData ) : null;

		//										RenderOperation( context, outputItemoperation, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount, tessItem );
		//									}
		//									else if( renderableItem.X == 1 )
		//									{
		//										//billboards
		//										ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
		//										var meshData = Billboard.GetBillboardMesh().Result.MeshData;

		//										ForwardBindGeneralTexturesUniforms( context, frameData/*, ref billboardItem.BoundingSphere, lightItem, receiveShadows*/, nRenderableItem == 0, true );

		//										BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, outputItemoperation.UnwrappedUV, ref billboardItem.Color, outputItemoperation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, billboardItem.MaterialInstanceParameters, 0, ref vector3FZero );

		//										billboardItem.GetWorldMatrixRelative( out var worldMatrix );
		//										Bgfx.SetTransform( (float*)&worldMatrix );

		//										RenderOperation( context, outputItemoperation, pass, null, billboardItem.CutVolumes );
		//									}
		//								}
		//							}
		//						}
		//					}
		//				}


		//				//clear outputInstancingManagers
		//				foreach( var manager in outputInstancingManagers )
		//					manager.Clear();
		//			}

		//			Bgfx.Discard( DiscardFlags.All );
		//		}


		//		//final pass
		//		{

		//			//!!!!temp
		//			context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ] );
		//			//context.SetViewport( sceneMotionDepthMRT.Viewports[ 0 ] );

		//			var shader = new CanvasRenderer.ShaderItem();
		//			shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
		//			shader.FragmentProgramFileName = @"Base\Shaders\ComposeOIT_fs.sc";

		//			shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, oitScreen, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );
		//			//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, oitLists, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

		//			//context.BindComputeImage( 0, oitScreen, 0, ComputeBufferAccessEnum.ReadWrite );
		//			//context.BindComputeImage( 0, oitScreen, 0, ComputeBufferAccessEnum.Read );

		//			//context.BindComputeImage( 1, oitLists, 0, ComputeBufferAccessEnum.Read );

		//			//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"colorTexture"*/, colorTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
		//			//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"secondTexture"*/, secondTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

		//			context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.AlphaBlend );
		//		}


		//		//!!!!удалить буферы

		//	}

		//	Bgfx.Discard( DiscardFlags.All );
		//}



		////push to GPU
		//for( int nSector = 0; nSector < sectorCount; nSector++ )
		//{
		//	var manager = outputInstancingManagers[ nSector ];

		//	int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorCount );
		//	int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorCount );
		//	if( nSector == sectorCount - 1 )
		//		indexTo = renderableGroupsToDraw.Count;

		//	//render output items
		//	{
		//		var outputItems = manager.outputItems;
		//		for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
		//		{
		//			ref var outputItem = ref outputItems.Data[ nOutputItem ];
		//			var materialData = outputItem.materialData;
		//			var outputItemoperation = outputItem.operation;
		//			var outputItemoperationVoxelDataInfo = outputItemoperation.VoxelDataInfo;
		//			var voxelRendering = outputItemoperationVoxelDataInfo != null;

		//			var pass = materialData.GetForwardShadingPass( directionalAmbientOnly, directionalAmbientOnlyPrepareShadows, voxelRendering );
		//			//var pass = materialData.forwardShadingPass;

		//			//var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//			//bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//			//GpuMaterialPass pass;
		//			//if( receiveShadows )
		//			//{
		//			//	pass = passesGroup.passWithShadows;
		//			//	//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//			//	//	pass = passesGroup.passWithShadowsHigh;
		//			//	//else
		//			//	//	pass = passesGroup.passWithShadowsLow;
		//			//}
		//			//else
		//			//	pass = passesGroup.passWithoutShadows;

		//			if( Instancing && outputItem.renderableItemsCount >= 2 )//InstancingMinCount )
		//			{
		//				//with instancing

		//				//bind material data
		//				BindMaterialData( context, materialData, false, voxelRendering );
		//				BindSamplersForTextureOnlySlots( context, false, voxelRendering );

		//				//bind operation data
		//				var firstRenderableItem = outputItem.renderableItemFirst;
		//				if( firstRenderableItem.X == 0 )
		//				{
		//					//meshes
		//					ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ firstRenderableItem.Y ];
		//					var meshData = meshItem.MeshData;

		//					ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, true );

		//					BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItemoperation.UnwrappedUV, ref meshItem.Color, outputItemoperation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, null, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );
		//				}
		//				else if( firstRenderableItem.X == 1 )
		//				{
		//					//billboards
		//					ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ firstRenderableItem.Y ];
		//					var meshData = Billboard.GetBillboardMesh().Result.MeshData;

		//					ForwardBindGeneralTexturesUniforms( context, frameData/*, ref billboardItem.BoundingSphere, lightItem, receiveShadows*/, true );

		//					BindRenderOperationData( context, frameData, materialData, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, outputItemoperation.UnwrappedUV, ref billboardItem.Color, outputItemoperation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, null, 0, ref vector3FZero );
		//				}

		//				int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
		//				int instanceCount = outputItem.renderableItemsCount;

		//				var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
		//				if( instanceBuffer.Valid )
		//				{
		//					//get instancing matrices
		//					RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
		//					int currentMatrix = 0;
		//					for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
		//					{
		//						var renderableItem = outputItem.renderableItems[ nRenderableItem ];

		//						if( renderableItem.X == 0 )
		//						{
		//							//meshes
		//							ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
		//							meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
		//						}
		//						else if( renderableItem.X == 1 )
		//						{
		//							//billboards
		//							ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
		//							billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
		//						}
		//					}

		//					RenderOperation( context, outputItemoperation, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
		//				}
		//			}
		//			else
		//			{
		//				//without instancing

		//				for( int nRenderableItem = 0; nRenderableItem < outputItem.renderableItemsCount; nRenderableItem++ )
		//				{
		//					Vector3I renderableItem;
		//					if( nRenderableItem != 0 )
		//						renderableItem = outputItem.renderableItems[ nRenderableItem ];
		//					else
		//						renderableItem = outputItem.renderableItemFirst;

		//					//bind material data
		//					BindMaterialData( context, materialData, false, voxelRendering );
		//					BindSamplersForTextureOnlySlots( context, false, voxelRendering );

		//					//bind render operation data, set matrix
		//					if( renderableItem.X == 0 )
		//					{
		//						//meshes
		//						ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
		//						var meshData = meshItem.MeshData;

		//						ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, nRenderableItem == 0 );

		//						BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItemoperation.UnwrappedUV, ref meshItem.Color, outputItemoperation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, meshItem.ObjectInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

		//						if( !meshItem.InstancingEnabled )
		//							fixed( Matrix4F* p = &meshItem.TransformRelative )
		//								Bgfx.SetTransform( (float*)p );

		//						var tessItem = meshItem.Tessellation ? TessellationGetItem( context, outputItemoperation, materialData ) : null;

		//						RenderOperation( context, outputItemoperation, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount, tessItem );
		//					}
		//					else if( renderableItem.X == 1 )
		//					{
		//						//billboards
		//						ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
		//						var meshData = Billboard.GetBillboardMesh().Result.MeshData;

		//						ForwardBindGeneralTexturesUniforms( context, frameData/*, ref billboardItem.BoundingSphere, lightItem, receiveShadows*/, nRenderableItem == 0 );

		//						BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, outputItemoperation.UnwrappedUV, ref billboardItem.Color, outputItemoperation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItemoperation.VoxelDataImage, outputItemoperationVoxelDataInfo, billboardItem.MaterialInstanceParameters, 0, ref vector3FZero );

		//						billboardItem.GetWorldMatrixRelative( out var worldMatrix );
		//						Bgfx.SetTransform( (float*)&worldMatrix );

		//						RenderOperation( context, outputItemoperation, pass, null, billboardItem.CutVolumes );
		//					}
		//				}
		//			}
		//		}
		//	}
		//}



		//InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;


		////draw
		//for( int nRenderableGroupsToDraw = 0; nRenderableGroupsToDraw < renderableGroupsToDraw.Count; nRenderableGroupsToDraw++ )
		//{
		//	var renderableGroup = renderableGroupsToDraw[ nRenderableGroupsToDraw ].RenderableGroup;

		//	if( renderableGroup.X == 0 )
		//	{
		//		//Mesh rendering

		//		ref var meshItem2 = ref frameDataMeshes.Data[ renderableGroup.Y ];
		//		ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];
		//		var meshData = meshItem.MeshData;

		//		//get data for instancing
		//		int additionInstancingRendered = 0;

		//		//!!!!need?
		//		//!!!!need?
		//		//!!!!need?

		//		//!!!!
		//		//if( Instancing && InstancingTransparent )
		//		//{
		//		//	int nRenderableGroupsToDraw2 = nRenderableGroupsToDraw + 1;
		//		//	while( nRenderableGroupsToDraw2 < renderableGroupsToDraw.Count )
		//		//	{
		//		//		if( additionInstancingRendered >= InstancingMaxCount )
		//		//			break;


		//		//		//!!!!probes?


		//		//		var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw2 ].RenderableGroup;
		//		//		if( renderableGroup2.X == 0 )
		//		//		{
		//		//			ref var meshItem2_2 = ref frameDataMeshes.Data[ renderableGroup2.Y ];
		//		//			if( meshItem2.CanUseInstancingForTransparentWith( ref meshItem2_2 ) )
		//		//			{
		//		//				ref var meshItem_2 = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup2.Y ];
		//		//				if( meshItem.CanUseInstancingForTransparentWith( ref meshItem_2 ) )
		//		//				{
		//		//					additionInstancingRendered++;
		//		//					nRenderableGroupsToDraw2++;
		//		//					continue;
		//		//				}
		//		//			}
		//		//		}

		//		//		break;
		//		//	}

		//		//	if( additionInstancingRendered < 2/*InstancingMinCount*/ )
		//		//		additionInstancingRendered = 0;
		//		//}
		//		bool instancing = additionInstancingRendered != 0;


		//		//init instance buffer
		//		int instanceCount = 0;
		//		//InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;
		//		if( instancing )
		//		{
		//			int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
		//			instanceCount = additionInstancingRendered + 1;

		//			instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
		//			if( instanceBuffer.Valid )
		//			{
		//				//get instancing matrices
		//				RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
		//				int currentMatrix = 0;
		//				for( int n = 0; n < instanceCount; n++ )
		//				{
		//					var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw + n ].RenderableGroup;

		//					ref var meshItem_2 = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup2.Y ];
		//					meshItem_2.GetInstancingData( out instancingData[ currentMatrix++ ] );
		//				}
		//			}
		//		}


		//		//render mesh item
		//		if( !meshItem.OnlyForShadowGeneration )
		//		{
		//			for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//			{
		//				var oper = meshData.RenderOperations[ nOperation ];
		//				var voxelRendering = oper.VoxelDataImage != null;

		//				foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, false ) )
		//				{
		//					bool add = materialData.Transparent;
		//					if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//						add = false;
		//					if( !add )
		//						continue;

		//					if( materialData.AllPasses.Count != 0 )
		//					{
		//						var pass = materialData.GetForwardShadingPass( directionalAmbientOnly, directionalAmbientOnlyPrepareShadows, voxelRendering );
		//						//var pass = materialData.forwardShadingPass;

		//						//var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//						//bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//						//GpuMaterialPass pass;
		//						//if( receiveShadows )
		//						//{
		//						//	pass = passesGroup.passWithShadows;
		//						//	//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//						//	//	pass = passesGroup.passWithShadowsHigh;
		//						//	//else
		//						//	//	pass = passesGroup.passWithShadowsLow;
		//						//}
		//						//else
		//						//	pass = passesGroup.passWithoutShadows;

		//						ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, true );

		//						BindMaterialData( context, materialData, false, voxelRendering );
		//						BindSamplersForTextureOnlySlots( context, false, oper.VoxelDataInfo != null );
		//						//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

		//						BindRenderOperationData( context, frameData, materialData, instancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref meshItem.Color, oper.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, oper.VoxelDataImage, oper.VoxelDataInfo, instancing ? null : meshItem.ObjectInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

		//						if( instancing )
		//						{
		//							if( instanceBuffer.Valid )
		//								RenderOperation( context, oper, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
		//						}
		//						else
		//						{
		//							var tessItem = meshItem.Tessellation ? TessellationGetItem( context, oper, materialData ) : null;

		//							fixed( Matrix4F* p = &meshItem.TransformRelative )
		//								Bgfx.SetTransform( (float*)p );
		//							RenderOperation( context, oper, pass, null, meshItem.CutVolumes, tessItem );
		//						}
		//					}
		//					//}
		//				}
		//			}

		//			//render layers
		//			if( ( ( meshItem2.Flags & FrameData.MeshItem.FlagsEnum.ContainsTransparentLayersOnTransparentBaseObjects ) != 0 ) && DebugDrawLayers )
		//			{
		//				if( meshItem.Layers != null )
		//				{
		//					for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
		//					{
		//						ref var layer = ref meshItem.Layers[ nLayer ];
		//						foreach( var materialData in GetLayerMaterialData( ref layer, true, false ) )
		//						{
		//							//if( materialData == null )
		//							//	continue;

		//							bool add = materialData.Transparent;
		//							if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//								add = false;
		//							if( !add )
		//								continue;

		//							if( materialData.AllPasses.Count != 0 )
		//							{
		//								var pass = materialData.GetForwardShadingPass( directionalAmbientOnly, directionalAmbientOnlyPrepareShadows, false );
		//								//var pass = materialData.forwardShadingPass;

		//								//var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//								//bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//								//GpuMaterialPass pass;
		//								//if( receiveShadows )
		//								//{
		//								//	pass = passesGroup.passWithShadows;
		//								//	//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//								//	//	pass = passesGroup.passWithShadowsHigh;
		//								//	//else
		//								//	//	pass = passesGroup.passWithShadowsLow;
		//								//}
		//								//else
		//								//	pass = passesGroup.passWithoutShadows;

		//								ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, true );

		//								fixed( Matrix4F* p = &meshItem.TransformRelative )
		//									Bgfx.SetTransform( (float*)p );

		//								var color = /*meshItem.Color * */ layer.MaterialColor;

		//								for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//								{
		//									var oper = meshData.RenderOperations[ nOperation ];
		//									var voxelRendering = oper.VoxelDataInfo != null;

		//									//bind material data
		//									BindMaterialData( context, materialData, false, voxelRendering );
		//									BindSamplersForTextureOnlySlots( context, false, voxelRendering );
		//									materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );
		//									//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

		//									BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, oper.VoxelDataImage, oper.VoxelDataInfo, meshItem.ObjectInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative, layer.UVScale );

		//									RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
		//								}
		//							}
		//							//}
		//						}
		//					}
		//				}
		//			}
		//		}

		//		nRenderableGroupsToDraw += additionInstancingRendered;
		//	}
		//	else if( renderableGroup.X == 1 )
		//	{
		//		//Billboard rendering

		//		ref var billboardItem2 = ref frameDataBillboards.Data[ renderableGroup.Y ];
		//		ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup.Y ];

		//		var meshData = Billboard.GetBillboardMesh().Result.MeshData;

		//		//get data for instancing
		//		int additionInstancingRendered = 0;

		//		//!!!!need?
		//		//!!!!need?
		//		//!!!!need?

		//		//if( Instancing && InstancingTransparent )
		//		//{
		//		//	int nRenderableGroupsToDraw2 = nRenderableGroupsToDraw + 1;
		//		//	while( nRenderableGroupsToDraw2 < renderableGroupsToDraw.Count )
		//		//	{
		//		//		if( additionInstancingRendered >= InstancingMaxCount )
		//		//			break;

		//		//		var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw2 ].RenderableGroup;
		//		//		if( renderableGroup2.X == 1 )
		//		//		{
		//		//			ref var billboardItem2_2 = ref frameDataBillboards.Data[ renderableGroup2.Y ];
		//		//			if( billboardItem2.CanUseInstancingForTransparentWith( ref billboardItem2_2 ) )
		//		//			{
		//		//				ref var billboardItem_2 = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup2.Y ];
		//		//				if( billboardItem.CanUseInstancingForTransparentWith( ref billboardItem_2 ) )
		//		//				{
		//		//					additionInstancingRendered++;
		//		//					nRenderableGroupsToDraw2++;
		//		//					continue;
		//		//				}
		//		//			}
		//		//		}

		//		//		break;
		//		//	}

		//		//	if( additionInstancingRendered < 2/*InstancingMinCount*/ )
		//		//		additionInstancingRendered = 0;
		//		//}
		//		bool instancing = additionInstancingRendered != 0;


		//		//init instance buffer
		//		int instanceCount = 0;
		//		//InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;
		//		if( instancing )
		//		{
		//			int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
		//			instanceCount = additionInstancingRendered + 1;

		//			instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
		//			if( instanceBuffer.Valid )
		//			{
		//				//get instancing matrices
		//				RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
		//				int currentMatrix = 0;
		//				for( int n = 0; n < instanceCount; n++ )
		//				{
		//					var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw + n ].RenderableGroup;

		//					ref var billboardItem_2 = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup2.Y ];
		//					billboardItem_2.GetInstancingData( out instancingData[ currentMatrix++ ] );
		//				}
		//			}
		//		}


		//		for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//		{
		//			var oper = meshData.RenderOperations[ nOperation ];
		//			var voxelRendering = oper.VoxelDataInfo != null;

		//			foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, false ) )
		//			{
		//				bool add = materialData.Transparent;
		//				if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//					add = false;
		//				if( !add )
		//					continue;

		//				if( materialData.AllPasses.Count != 0 )
		//				{
		//					var pass = materialData.GetForwardShadingPass( directionalAmbientOnly, directionalAmbientOnlyPrepareShadows, voxelRendering );
		//					//var pass = materialData.forwardShadingPass;

		//					//var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//					//bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//					//GpuMaterialPass pass;
		//					//if( receiveShadows )
		//					//{
		//					//	pass = passesGroup.passWithShadows;
		//					//	//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//					//	//	pass = passesGroup.passWithShadowsHigh;
		//					//	//else
		//					//	//	pass = passesGroup.passWithShadowsLow;
		//					//}
		//					//else
		//					//	pass = passesGroup.passWithoutShadows;

		//					ForwardBindGeneralTexturesUniforms( context, frameData/*, ref billboardItem.BoundingSphere, lightItem, receiveShadows*/, true );

		//					BindMaterialData( context, materialData, false, voxelRendering );
		//					BindSamplersForTextureOnlySlots( context, false, oper.VoxelDataInfo != null );
		//					//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

		//					BindRenderOperationData( context, frameData, materialData, instancing, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, oper.UnwrappedUV, ref billboardItem.Color, oper.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, oper.VoxelDataImage, oper.VoxelDataInfo, instancing ? null : billboardItem.MaterialInstanceParameters, 0, ref vector3FZero );

		//					if( instancing )
		//					{
		//						if( instanceBuffer.Valid )
		//							RenderOperation( context, oper, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
		//					}
		//					else
		//					{
		//						billboardItem.GetWorldMatrixRelative( out var worldMatrix );
		//						Bgfx.SetTransform( (float*)&worldMatrix );
		//						RenderOperation( context, oper, pass, null, null );
		//					}
		//				}
		//			}
		//		}

		//		nRenderableGroupsToDraw += additionInstancingRendered;
		//	}
		//}




		//[MethodImpl( (MethodImplOptions)512 )]
		//protected unsafe virtual void Render3DSceneForwardTransparentWithOIT( ViewportRenderingContext context, FrameData frameData, ImageComponent colorDepthTextureCopy )
		//{



		//		//Material.CompiledMaterialData materialBinded = null;


		//		//draw
		//		for( int nRenderableGroupsToDraw = 0; nRenderableGroupsToDraw < renderableGroupsToDraw.Count; nRenderableGroupsToDraw++ )
		//		{
		//			var renderableGroup = renderableGroupsToDraw[ nRenderableGroupsToDraw ].RenderableGroup;

		//			if( renderableGroup.X == 0 )
		//			{
		//				//Mesh rendering

		//				ref var meshItem2 = ref frameDataMeshes.Data[ renderableGroup.Y ];
		//				ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];
		//				var meshData = meshItem.MeshData;

		//				//get data for instancing
		//				int additionInstancingRendered = 0;
		//				if( Instancing && InstancingTransparent )
		//				{
		//					int nRenderableGroupsToDraw2 = nRenderableGroupsToDraw + 1;
		//					while( nRenderableGroupsToDraw2 < renderableGroupsToDraw.Count )
		//					{
		//						if( additionInstancingRendered >= InstancingMaxCount )
		//							break;


		//						//!!!!probes?


		//						var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw2 ].RenderableGroup;
		//						if( renderableGroup2.X == 0 )
		//						{
		//							ref var meshItem2_2 = ref frameDataMeshes.Data[ renderableGroup2.Y ];
		//							if( meshItem2.CanUseInstancingForTransparentWith( ref meshItem2_2 ) )
		//							{
		//								ref var meshItem_2 = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup2.Y ];
		//								if( meshItem.CanUseInstancingForTransparentWith( ref meshItem_2 ) )
		//								{
		//									additionInstancingRendered++;
		//									nRenderableGroupsToDraw2++;
		//									continue;
		//								}
		//							}
		//						}

		//						break;
		//					}

		//					if( additionInstancingRendered < 2/*InstancingMinCount*/ )
		//						additionInstancingRendered = 0;
		//				}
		//				bool instancing = additionInstancingRendered != 0;


		//				//init instance buffer
		//				int instanceCount = 0;
		//				//InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;
		//				if( instancing )
		//				{
		//					int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
		//					instanceCount = additionInstancingRendered + 1;

		//					instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
		//					if( instanceBuffer.Valid )
		//					{
		//						//get instancing matrices
		//						RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
		//						int currentMatrix = 0;
		//						for( int n = 0; n < instanceCount; n++ )
		//						{
		//							var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw + n ].RenderableGroup;

		//							ref var meshItem_2 = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup2.Y ];
		//							meshItem_2.GetInstancingData( out instancingData[ currentMatrix++ ] );
		//						}
		//					}
		//				}


		//				//render mesh item
		//				if( !meshItem.OnlyForShadowGeneration )
		//				{
		//					for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//					{
		//						var oper = meshData.RenderOperations[ nOperation ];
		//						var voxelRendering = oper.VoxelDataImage != null;

		//						foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, false ) )
		//						{
		//							bool add = materialData.Transparent;
		//							if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//								add = false;
		//							if( !add )
		//								continue;

		//							for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
		//							{
		//								var lightIndex = affectedLights[ nAffectedLightIndex ];
		//								var lightItem = frameData.Lights[ lightIndex ];

		//								if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		//								{
		//									//set lightItem uniforms
		//									if( lightItemBinded != lightItem )
		//									{
		//										lightItem.Bind( this, context );
		//										lightItemBinded = lightItem;
		//									}

		//									var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//									bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//									GpuMaterialPass pass;
		//									if( receiveShadows )
		//									{
		//										pass = passesGroup.passWithShadows;
		//										//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//										//	pass = passesGroup.passWithShadowsHigh;
		//										//else
		//										//	pass = passesGroup.passWithShadowsLow;
		//									}
		//									else
		//										pass = passesGroup.passWithoutShadows;

		//									ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

		//									BindMaterialData( context, materialData, false, voxelRendering );
		//									BindSamplersForTextureOnlySlots( context, false, oper.VoxelDataInfo != null );
		//									//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

		//									BindRenderOperationData( context, frameData, materialData, instancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref meshItem.Color, oper.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, oper.VoxelDataImage, oper.VoxelDataInfo, instancing ? null : meshItem.MaterialInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

		//									if( instancing )
		//									{
		//										if( instanceBuffer.Valid )
		//											RenderOperation( context, oper, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
		//									}
		//									else
		//									{
		//										fixed( Matrix4F* p = &meshItem.TransformRelative )
		//											Bgfx.SetTransform( (float*)p );
		//										RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
		//									}
		//								}
		//							}
		//						}
		//					}

		//					//render layers
		//					if( ( ( meshItem2.Flags & FrameData.MeshItem.FlagsEnum.ContainsTransparentLayersOnTransparentBaseObjects ) != 0 ) && DebugDrawLayers )
		//					{
		//						if( meshItem.Layers != null )
		//						{
		//							for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
		//							{
		//								ref var layer = ref meshItem.Layers[ nLayer ];
		//								foreach( var materialData in GetLayerMaterialData( ref layer, true, false ) )
		//								{
		//									//if( materialData == null )
		//									//	continue;

		//									bool add = materialData.Transparent;
		//									if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//										add = false;
		//									if( !add )
		//										continue;

		//									for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
		//									{
		//										var lightIndex = affectedLights[ nAffectedLightIndex ];
		//										var lightItem = frameData.Lights[ lightIndex ];

		//										if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		//										{
		//											//set lightItem uniforms
		//											if( lightItemBinded != lightItem )
		//											{
		//												lightItem.Bind( this, context );
		//												lightItemBinded = lightItem;
		//											}

		//											var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//											bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//											GpuMaterialPass pass;
		//											if( receiveShadows )
		//											{
		//												pass = passesGroup.passWithShadows;
		//												//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//												//	pass = passesGroup.passWithShadowsHigh;
		//												//else
		//												//	pass = passesGroup.passWithShadowsLow;
		//											}
		//											else
		//												pass = passesGroup.passWithoutShadows;

		//											ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

		//											fixed( Matrix4F* p = &meshItem.TransformRelative )
		//												Bgfx.SetTransform( (float*)p );

		//											var color = /*meshItem.Color * */ layer.MaterialColor;

		//											for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//											{
		//												var oper = meshData.RenderOperations[ nOperation ];
		//												var voxelRendering = oper.VoxelDataInfo != null;

		//												//bind material data
		//												BindMaterialData( context, materialData, false, voxelRendering );
		//												BindSamplersForTextureOnlySlots( context, false, voxelRendering );
		//												materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );
		//												//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

		//												BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, oper.VoxelDataImage, oper.VoxelDataInfo, meshItem.MaterialInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

		//												RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
		//											}
		//										}
		//									}
		//								}
		//							}
		//						}
		//					}
		//				}

		//				nRenderableGroupsToDraw += additionInstancingRendered;
		//			}
		//			else if( renderableGroup.X == 1 )
		//			{
		//				//Billboard rendering

		//				ref var billboardItem2 = ref frameDataBillboards.Data[ renderableGroup.Y ];
		//				ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup.Y ];

		//				var meshData = Billboard.GetBillboardMesh().Result.MeshData;

		//				//get data for instancing
		//				int additionInstancingRendered = 0;
		//				if( Instancing && InstancingTransparent )
		//				{
		//					int nRenderableGroupsToDraw2 = nRenderableGroupsToDraw + 1;
		//					while( nRenderableGroupsToDraw2 < renderableGroupsToDraw.Count )
		//					{
		//						if( additionInstancingRendered >= InstancingMaxCount )
		//							break;

		//						var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw2 ].RenderableGroup;
		//						if( renderableGroup2.X == 1 )
		//						{
		//							ref var billboardItem2_2 = ref frameDataBillboards.Data[ renderableGroup2.Y ];
		//							if( billboardItem2.CanUseInstancingForTransparentWith( ref billboardItem2_2 ) )
		//							{
		//								ref var billboardItem_2 = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup2.Y ];
		//								if( billboardItem.CanUseInstancingForTransparentWith( ref billboardItem_2 ) )
		//								{
		//									additionInstancingRendered++;
		//									nRenderableGroupsToDraw2++;
		//									continue;
		//								}
		//							}
		//						}

		//						break;
		//					}

		//					if( additionInstancingRendered < 2/*InstancingMinCount*/ )
		//						additionInstancingRendered = 0;
		//				}
		//				bool instancing = additionInstancingRendered != 0;


		//				//init instance buffer
		//				int instanceCount = 0;
		//				//InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;
		//				if( instancing )
		//				{
		//					int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
		//					instanceCount = additionInstancingRendered + 1;

		//					instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
		//					if( instanceBuffer.Valid )
		//					{
		//						//get instancing matrices
		//						RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
		//						int currentMatrix = 0;
		//						for( int n = 0; n < instanceCount; n++ )
		//						{
		//							var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw + n ].RenderableGroup;

		//							ref var billboardItem_2 = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup2.Y ];
		//							billboardItem_2.GetInstancingData( out instancingData[ currentMatrix++ ] );
		//						}
		//					}
		//				}


		//				for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//				{
		//					var oper = meshData.RenderOperations[ nOperation ];
		//					var voxelRendering = oper.VoxelDataInfo != null;

		//					foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, false ) )
		//					{
		//						bool add = materialData.Transparent;
		//						if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//							add = false;
		//						if( !add )
		//							continue;

		//						for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
		//						{
		//							var lightIndex = affectedLights[ nAffectedLightIndex ];
		//							var lightItem = frameData.Lights[ lightIndex ];

		//							if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		//							{
		//								//set lightItem uniforms
		//								if( lightItemBinded != lightItem )
		//								{
		//									lightItem.Bind( this, context );
		//									lightItemBinded = lightItem;
		//								}

		//								var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//								bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//								GpuMaterialPass pass;
		//								if( receiveShadows )
		//								{
		//									pass = passesGroup.passWithShadows;
		//									//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//									//	pass = passesGroup.passWithShadowsHigh;
		//									//else
		//									//	pass = passesGroup.passWithShadowsLow;
		//								}
		//								else
		//									pass = passesGroup.passWithoutShadows;

		//								ForwardBindGeneralTexturesUniforms( context, frameData, ref billboardItem.BoundingSphere, lightItem, receiveShadows, true, false );

		//								BindMaterialData( context, materialData, false, voxelRendering );
		//								BindSamplersForTextureOnlySlots( context, false, oper.VoxelDataInfo != null );
		//								//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

		//								BindRenderOperationData( context, frameData, materialData, instancing, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, oper.UnwrappedUV, ref billboardItem.Color, oper.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, oper.VoxelDataImage, oper.VoxelDataInfo, instancing ? null : billboardItem.MaterialInstanceParameters, 0, ref vector3FZero );

		//								if( instancing )
		//								{
		//									if( instanceBuffer.Valid )
		//										RenderOperation( context, oper, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
		//								}
		//								else
		//								{
		//									billboardItem.GetWorldMatrixRelative( out var worldMatrix );
		//									Bgfx.SetTransform( (float*)&worldMatrix );
		//									RenderOperation( context, oper, pass, null, null );
		//								}
		//							}
		//						}
		//					}
		//				}

		//				nRenderableGroupsToDraw += additionInstancingRendered;
		//			}
		//		}
		//	}
		//}






		////[MethodImpl( (MethodImplOptions)512 )]
		////protected unsafe virtual void Render3DSceneForwardTransparentWithOIT( ViewportRenderingContext context, FrameData frameData, ImageComponent colorDepthTextureCopy )
		////{

		////	var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
		////	var frameDataRenderSceneDataBillboards = frameData.RenderSceneData.Billboards;
		////	var frameDataMeshes = frameData.Meshes;
		////	var frameDataBillboards = frameData.Billboards;

		////	var viewportOwner = context.Owner;

		////	using( var renderableGroupsToDraw = new OpenListNative<RenderableGroupWithDistance>( frameData.RenderableGroupsInFrustum.Count ) )
		////	{
		////		foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
		////		{
		////			bool add = false;

		////			if( renderableGroup.X == 0 )
		////			{
		////				ref var data = ref frameDataMeshes.Data[ renderableGroup.Y ];
		////				add = ( data.Flags & FrameData.MeshItem.FlagsEnum.UseForwardTransparent ) != 0;
		////			}
		////			else
		////			{
		////				ref var data = ref frameDataBillboards.Data[ renderableGroup.Y ];
		////				add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseForwardTransparent ) != 0;
		////			}

		////			if( add )
		////			{
		////				var item = new RenderableGroupWithDistance();
		////				item.RenderableGroup = renderableGroup;

		////				//!!!!need?

		////				item.DistanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.RenderableGroup );
		////				//no sense to sort
		////				//ApplyTransparentRenderingAddOffsetWhenSortByDistance( frameData, ref item );
		////				renderableGroupsToDraw.Add( item );
		////			}
		////		}

		////		if( renderableGroupsToDraw.Count == 0 )
		////			return;

		////		//no sense to sort
		////		////sort by distance
		////		//CollectionUtility.MergeSortUnmanaged( renderableGroupsToDraw.Data, renderableGroupsToDraw.Count, delegate ( RenderableGroupWithDistance* a, RenderableGroupWithDistance* b )
		////		//{
		////		//	if( a->DistanceSquared > b->DistanceSquared )
		////		//		return -1;
		////		//	if( a->DistanceSquared < b->DistanceSquared )
		////		//		return 1;
		////		//	return 0;
		////		//}, true );


		////		LightItem lightItemBinded = null;
		////		//Material.CompiledMaterialData materialBinded = null;

		////		int ambientDirectionalLightCount = 0;
		////		foreach( var lightIndex in frameData.LightsInFrustumSorted )
		////		{
		////			var lightItem = frameData.Lights[ lightIndex ];
		////			if( lightItem.data.Type == Light.TypeEnum.Point || lightItem.data.Type == Light.TypeEnum.Spotlight )
		////				break;
		////			ambientDirectionalLightCount++;
		////		}

		////		int[] tempIntArray = null;
		////		int[] GetTempIntArray( int minSize )
		////		{
		////			if( tempIntArray == null || tempIntArray.Length < minSize )
		////				tempIntArray = new int[ minSize ];
		////			return tempIntArray;
		////		}


		////		//bind textures for all render operations
		////		context.BindTexture( 5/*"s_colorDepthTextureCopy"*/, colorDepthTextureCopy ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None, 0, false );
		////		//context.BindTexture( 5/*"s_colorDepthTextureCopy"*/, colorDepthTextureCopy ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point );
		////		BindBrdfLUT( context );
		////		BindMaterialsTexture( context, frameData );
		////		BindBonesTexture( context, frameData );
		////		BindSamplersForTextureOnlySlots( context, true, false );
		////		BindMaterialData( context, null, false, false );

		////		InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;


		////		//!!!!
		////		//инстансить
		////		//что про секторы


		////		//draw
		////		for( int nRenderableGroupsToDraw = 0; nRenderableGroupsToDraw < renderableGroupsToDraw.Count; nRenderableGroupsToDraw++ )
		////		{
		////			var renderableGroup = renderableGroupsToDraw[ nRenderableGroupsToDraw ].RenderableGroup;

		////			if( renderableGroup.X == 0 )
		////			{
		////				//Mesh rendering

		////				ref var meshItem2 = ref frameDataMeshes.Data[ renderableGroup.Y ];
		////				ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];
		////				var meshData = meshItem.MeshData;

		////				//!!!!не всегда нужно
		////				var affectedLightsCount = ambientDirectionalLightCount + meshItem2.PointSpotLightCount;
		////				var affectedLights = GetTempIntArray( affectedLightsCount );//var affectedLights = stackalloc int[ affectedLightsCount ];
		////				for( int n = 0; n < ambientDirectionalLightCount; n++ )
		////					affectedLights[ n ] = frameData.LightsInFrustumSorted[ n ];
		////				for( int n = 0; n < meshItem2.PointSpotLightCount; n++ )
		////					affectedLights[ ambientDirectionalLightCount + n ] = meshItem2.GetPointSpotLight( n );

		////				//get data for instancing
		////				int additionInstancingRendered = 0;
		////				if( Instancing && InstancingTransparent )
		////				{
		////					int nRenderableGroupsToDraw2 = nRenderableGroupsToDraw + 1;
		////					while( nRenderableGroupsToDraw2 < renderableGroupsToDraw.Count )
		////					{
		////						if( additionInstancingRendered >= InstancingMaxCount )
		////							break;


		////						//!!!!probes?


		////						var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw2 ].RenderableGroup;
		////						if( renderableGroup2.X == 0 )
		////						{
		////							ref var meshItem2_2 = ref frameDataMeshes.Data[ renderableGroup2.Y ];
		////							if( meshItem2.CanUseInstancingForTransparentWith( ref meshItem2_2 ) )
		////							{
		////								ref var meshItem_2 = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup2.Y ];
		////								if( meshItem.CanUseInstancingForTransparentWith( ref meshItem_2 ) )
		////								{
		////									additionInstancingRendered++;
		////									nRenderableGroupsToDraw2++;
		////									continue;
		////								}
		////							}
		////						}

		////						break;
		////					}

		////					if( additionInstancingRendered < 2/*InstancingMinCount*/ )
		////						additionInstancingRendered = 0;
		////				}
		////				bool instancing = additionInstancingRendered != 0;


		////				//init instance buffer
		////				int instanceCount = 0;
		////				//InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;
		////				if( instancing )
		////				{
		////					int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
		////					instanceCount = additionInstancingRendered + 1;

		////					instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
		////					if( instanceBuffer.Valid )
		////					{
		////						//get instancing matrices
		////						RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
		////						int currentMatrix = 0;
		////						for( int n = 0; n < instanceCount; n++ )
		////						{
		////							var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw + n ].RenderableGroup;

		////							ref var meshItem_2 = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup2.Y ];
		////							meshItem_2.GetInstancingData( out instancingData[ currentMatrix++ ] );
		////						}
		////					}
		////				}


		////				//render mesh item
		////				if( !meshItem.OnlyForShadowGeneration )
		////				{
		////					for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		////					{
		////						var oper = meshData.RenderOperations[ nOperation ];
		////						var voxelRendering = oper.VoxelDataImage != null;

		////						foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, false ) )
		////						{
		////							bool add = materialData.Transparent;
		////							if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		////								add = false;
		////							if( !add )
		////								continue;

		////							for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
		////							{
		////								var lightIndex = affectedLights[ nAffectedLightIndex ];
		////								var lightItem = frameData.Lights[ lightIndex ];

		////								if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		////								{
		////									//set lightItem uniforms
		////									if( lightItemBinded != lightItem )
		////									{
		////										lightItem.Bind( this, context );
		////										lightItemBinded = lightItem;
		////									}

		////									var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		////									bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		////									GpuMaterialPass pass;
		////									if( receiveShadows )
		////									{
		////										pass = passesGroup.passWithShadows;
		////										//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		////										//	pass = passesGroup.passWithShadowsHigh;
		////										//else
		////										//	pass = passesGroup.passWithShadowsLow;
		////									}
		////									else
		////										pass = passesGroup.passWithoutShadows;

		////									ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

		////									BindMaterialData( context, materialData, false, voxelRendering );
		////									BindSamplersForTextureOnlySlots( context, false, oper.VoxelDataInfo != null );
		////									//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

		////									BindRenderOperationData( context, frameData, materialData, instancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref meshItem.Color, oper.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, oper.VoxelDataImage, oper.VoxelDataInfo, instancing ? null : meshItem.MaterialInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

		////									if( instancing )
		////									{
		////										if( instanceBuffer.Valid )
		////											RenderOperation( context, oper, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
		////									}
		////									else
		////									{
		////										fixed( Matrix4F* p = &meshItem.TransformRelative )
		////											Bgfx.SetTransform( (float*)p );
		////										RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
		////									}
		////								}
		////							}
		////						}
		////					}

		////					//render layers
		////					if( ( ( meshItem2.Flags & FrameData.MeshItem.FlagsEnum.ContainsTransparentLayersOnTransparentBaseObjects ) != 0 ) && DebugDrawLayers )
		////					{
		////						if( meshItem.Layers != null )
		////						{
		////							for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
		////							{
		////								ref var layer = ref meshItem.Layers[ nLayer ];
		////								foreach( var materialData in GetLayerMaterialData( ref layer, true, false ) )
		////								{
		////									//if( materialData == null )
		////									//	continue;

		////									bool add = materialData.Transparent;
		////									if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		////										add = false;
		////									if( !add )
		////										continue;

		////									for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
		////									{
		////										var lightIndex = affectedLights[ nAffectedLightIndex ];
		////										var lightItem = frameData.Lights[ lightIndex ];

		////										if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		////										{
		////											//set lightItem uniforms
		////											if( lightItemBinded != lightItem )
		////											{
		////												lightItem.Bind( this, context );
		////												lightItemBinded = lightItem;
		////											}

		////											var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		////											bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		////											GpuMaterialPass pass;
		////											if( receiveShadows )
		////											{
		////												pass = passesGroup.passWithShadows;
		////												//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		////												//	pass = passesGroup.passWithShadowsHigh;
		////												//else
		////												//	pass = passesGroup.passWithShadowsLow;
		////											}
		////											else
		////												pass = passesGroup.passWithoutShadows;

		////											ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

		////											fixed( Matrix4F* p = &meshItem.TransformRelative )
		////												Bgfx.SetTransform( (float*)p );

		////											var color = /*meshItem.Color * */ layer.MaterialColor;

		////											for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		////											{
		////												var oper = meshData.RenderOperations[ nOperation ];
		////												var voxelRendering = oper.VoxelDataInfo != null;

		////												//bind material data
		////												BindMaterialData( context, materialData, false, voxelRendering );
		////												BindSamplersForTextureOnlySlots( context, false, voxelRendering );
		////												materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );
		////												//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

		////												BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, oper.VoxelDataImage, oper.VoxelDataInfo, meshItem.MaterialInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

		////												RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
		////											}
		////										}
		////									}
		////								}
		////							}
		////						}
		////					}
		////				}

		////				nRenderableGroupsToDraw += additionInstancingRendered;
		////			}
		////			else if( renderableGroup.X == 1 )
		////			{
		////				//Billboard rendering

		////				ref var billboardItem2 = ref frameDataBillboards.Data[ renderableGroup.Y ];
		////				ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup.Y ];

		////				//!!!!не всегда нужно
		////				var affectedLightsCount = ambientDirectionalLightCount + billboardItem2.PointSpotLightCount;
		////				var affectedLights = GetTempIntArray( affectedLightsCount );//var affectedLights = stackalloc int[ affectedLightsCount ];
		////				for( int n = 0; n < ambientDirectionalLightCount; n++ )
		////					affectedLights[ n ] = frameData.LightsInFrustumSorted[ n ];
		////				for( int n = 0; n < billboardItem2.PointSpotLightCount; n++ )
		////					affectedLights[ ambientDirectionalLightCount + n ] = billboardItem2.GetPointSpotLight( n );

		////				var meshData = Billboard.GetBillboardMesh().Result.MeshData;

		////				//get data for instancing
		////				int additionInstancingRendered = 0;
		////				if( Instancing && InstancingTransparent )
		////				{
		////					int nRenderableGroupsToDraw2 = nRenderableGroupsToDraw + 1;
		////					while( nRenderableGroupsToDraw2 < renderableGroupsToDraw.Count )
		////					{
		////						if( additionInstancingRendered >= InstancingMaxCount )
		////							break;

		////						var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw2 ].RenderableGroup;
		////						if( renderableGroup2.X == 1 )
		////						{
		////							ref var billboardItem2_2 = ref frameDataBillboards.Data[ renderableGroup2.Y ];
		////							if( billboardItem2.CanUseInstancingForTransparentWith( ref billboardItem2_2 ) )
		////							{
		////								ref var billboardItem_2 = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup2.Y ];
		////								if( billboardItem.CanUseInstancingForTransparentWith( ref billboardItem_2 ) )
		////								{
		////									additionInstancingRendered++;
		////									nRenderableGroupsToDraw2++;
		////									continue;
		////								}
		////							}
		////						}

		////						break;
		////					}

		////					if( additionInstancingRendered < 2/*InstancingMinCount*/ )
		////						additionInstancingRendered = 0;
		////				}
		////				bool instancing = additionInstancingRendered != 0;


		////				//init instance buffer
		////				int instanceCount = 0;
		////				//InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;
		////				if( instancing )
		////				{
		////					int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
		////					instanceCount = additionInstancingRendered + 1;

		////					instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
		////					if( instanceBuffer.Valid )
		////					{
		////						//get instancing matrices
		////						RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
		////						int currentMatrix = 0;
		////						for( int n = 0; n < instanceCount; n++ )
		////						{
		////							var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw + n ].RenderableGroup;

		////							ref var billboardItem_2 = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup2.Y ];
		////							billboardItem_2.GetInstancingData( out instancingData[ currentMatrix++ ] );
		////						}
		////					}
		////				}


		////				for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		////				{
		////					var oper = meshData.RenderOperations[ nOperation ];
		////					var voxelRendering = oper.VoxelDataInfo != null;

		////					foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, false ) )
		////					{
		////						bool add = materialData.Transparent;
		////						if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		////							add = false;
		////						if( !add )
		////							continue;

		////						for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
		////						{
		////							var lightIndex = affectedLights[ nAffectedLightIndex ];
		////							var lightItem = frameData.Lights[ lightIndex ];

		////							if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		////							{
		////								//set lightItem uniforms
		////								if( lightItemBinded != lightItem )
		////								{
		////									lightItem.Bind( this, context );
		////									lightItemBinded = lightItem;
		////								}

		////								var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		////								bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		////								GpuMaterialPass pass;
		////								if( receiveShadows )
		////								{
		////									pass = passesGroup.passWithShadows;
		////									//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		////									//	pass = passesGroup.passWithShadowsHigh;
		////									//else
		////									//	pass = passesGroup.passWithShadowsLow;
		////								}
		////								else
		////									pass = passesGroup.passWithoutShadows;

		////								ForwardBindGeneralTexturesUniforms( context, frameData, ref billboardItem.BoundingSphere, lightItem, receiveShadows, true, false );

		////								BindMaterialData( context, materialData, false, voxelRendering );
		////								BindSamplersForTextureOnlySlots( context, false, oper.VoxelDataInfo != null );
		////								//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

		////								BindRenderOperationData( context, frameData, materialData, instancing, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, oper.UnwrappedUV, ref billboardItem.Color, oper.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, oper.VoxelDataImage, oper.VoxelDataInfo, instancing ? null : billboardItem.MaterialInstanceParameters, 0, ref vector3FZero );

		////								if( instancing )
		////								{
		////									if( instanceBuffer.Valid )
		////										RenderOperation( context, oper, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
		////								}
		////								else
		////								{
		////									billboardItem.GetWorldMatrixRelative( out var worldMatrix );
		////									Bgfx.SetTransform( (float*)&worldMatrix );
		////									RenderOperation( context, oper, pass, null, null );
		////								}
		////							}
		////						}
		////					}
		////				}

		////				nRenderableGroupsToDraw += additionInstancingRendered;
		////			}
		////		}
		////	}
		////}

		[MethodImpl( (MethodImplOptions)512 )]
		protected unsafe virtual void Render3DSceneForwardTransparentWithoutOIT( ViewportRenderingContext context, FrameData frameData, ImageComponent colorDepthTextureCopy )
		{
			var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
			var frameDataRenderSceneDataBillboards = frameData.RenderSceneData.Billboards;
			var frameDataMeshes = frameData.Meshes;
			var frameDataBillboards = frameData.Billboards;

			var viewportOwner = context.Owner;

			using( var renderableGroupsToDraw = new OpenListNative<RenderableGroupWithDistance>( frameData.RenderableGroupsInFrustum.Count ) )
			{
				foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
				{
					bool add = false;

					if( renderableGroup.X == 0 )
					{
						ref var data = ref frameDataMeshes.Data[ renderableGroup.Y ];
						add = ( data.Flags & FrameData.MeshItem.FlagsEnum.UseForwardTransparent ) != 0;
					}
					else
					{
						ref var data = ref frameDataBillboards.Data[ renderableGroup.Y ];
						add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseForwardTransparent ) != 0;
					}

					if( add )
					{
						var item = new RenderableGroupWithDistance();
						item.RenderableGroup = renderableGroup;
						item.DistanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.RenderableGroup );
						ApplyTransparentRenderingAddOffsetWhenSortByDistance( frameData, ref item );
						renderableGroupsToDraw.Add( ref item );
					}
				}

				if( renderableGroupsToDraw.Count == 0 )
					return;

				//sort by distance
				CollectionUtility.MergeSortUnmanaged( renderableGroupsToDraw.Data, renderableGroupsToDraw.Count, delegate ( RenderableGroupWithDistance* a, RenderableGroupWithDistance* b )
				{
					if( a->DistanceSquared > b->DistanceSquared )
						return -1;
					if( a->DistanceSquared < b->DistanceSquared )
						return 1;
					return 0;
				}, true );

				//bind textures for all render operations
				context.BindTexture( 5/*"s_colorDepthTextureCopy"*/, colorDepthTextureCopy ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None, 0, false );
				BindBrdfLUT( context );
				BindMaterialsTexture( context, frameData );
				BindBonesTexture( context, frameData );
				BindSamplersForTextureOnlySlots( context, true, false );
				BindMaterialData( context, null, false, false );
				BindForwardLightAndShadows( context, frameData );

				var directionalAmbientOnly = IsDirectionalAmbientOnlyModeEnabled( context, out var directionalAmbientOnlyPrepareShadows );

				InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;

				//draw
				for( int nRenderableGroupsToDraw = 0; nRenderableGroupsToDraw < renderableGroupsToDraw.Count; nRenderableGroupsToDraw++ )
				{
					var renderableGroup = renderableGroupsToDraw[ nRenderableGroupsToDraw ].RenderableGroup;

					if( renderableGroup.X == 0 )
					{
						//Mesh rendering

						ref var meshItem2 = ref frameDataMeshes.Data[ renderableGroup.Y ];
						ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];
						var meshData = meshItem.MeshData;

						//get data for instancing
						int additionInstancingRendered = 0;
						//instancing works when OIT is enabled
						////if( Instancing && InstancingTransparent )
						////{
						////	int nRenderableGroupsToDraw2 = nRenderableGroupsToDraw + 1;
						////	while( nRenderableGroupsToDraw2 < renderableGroupsToDraw.Count )
						////	{
						////		if( additionInstancingRendered >= InstancingMaxCount )
						////			break;

						////		//!!!!probes?

						////		var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw2 ].RenderableGroup;
						////		if( renderableGroup2.X == 0 )
						////		{
						////			ref var meshItem2_2 = ref frameDataMeshes.Data[ renderableGroup2.Y ];
						////			if( meshItem2.CanUseInstancingForTransparentWith( ref meshItem2_2 ) )
						////			{
						////				ref var meshItem_2 = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup2.Y ];
						////				if( meshItem.CanUseInstancingForTransparentWith( ref meshItem_2 ) )
						////				{
						////					additionInstancingRendered++;
						////					nRenderableGroupsToDraw2++;
						////					continue;
						////				}
						////			}
						////		}

						////		break;
						////	}

						////	if( additionInstancingRendered < 2/*InstancingMinCount*/ )
						////		additionInstancingRendered = 0;
						////}
						bool instancing = additionInstancingRendered != 0;


						//init instance buffer
						int instanceCount = 0;
						//InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;
						if( instancing )
						{
							int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
							instanceCount = additionInstancingRendered + 1;

							instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
							if( instanceBuffer.Valid )
							{
								//get instancing matrices
								RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
								int currentMatrix = 0;
								for( int n = 0; n < instanceCount; n++ )
								{
									var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw + n ].RenderableGroup;

									ref var meshItem_2 = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup2.Y ];
									meshItem_2.GetInstancingData( out instancingData[ currentMatrix++ ] );
								}
							}
						}


						//render mesh item
						if( !meshItem.OnlyForShadowGeneration )
						{
							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
							{
								var oper = meshData.RenderOperations[ nOperation ];
								var voxelRendering = oper.VoxelDataImage != null;

								foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, false ) )
								{
									bool add = materialData.Transparent;
									if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
										add = false;
									if( !add )
										continue;

									if( materialData.AllPasses.Count != 0 )
									{
										var pass = materialData.GetForwardShadingPass( directionalAmbientOnly, directionalAmbientOnlyPrepareShadows, voxelRendering );
										//var pass = materialData.forwardShadingPass;

										//var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
										//bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

										//GpuMaterialPass pass;
										//if( receiveShadows )
										//{
										//	pass = passesGroup.passWithShadows;
										//	//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
										//	//	pass = passesGroup.passWithShadowsHigh;
										//	//else
										//	//	pass = passesGroup.passWithShadowsLow;
										//}
										//else
										//	pass = passesGroup.passWithoutShadows;

										ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, true );

										BindMaterialData( context, materialData, false, voxelRendering );
										BindSamplersForTextureOnlySlots( context, false, oper.VoxelDataInfo != null );
										//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

										BindRenderOperationData( context, frameData, materialData, instancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref meshItem.Color, oper.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, oper.VoxelDataImage, oper.VoxelDataInfo, instancing ? null : meshItem.ObjectInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

										if( instancing )
										{
											if( instanceBuffer.Valid )
												RenderOperation( context, oper, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
										}
										else
										{
											var tessItem = meshItem.Tessellation ? TessellationGetItem( context, oper, materialData ) : null;

											fixed( Matrix4F* p = &meshItem.TransformRelative )
												Bgfx.SetTransform( (float*)p );
											RenderOperation( context, oper, pass, null, meshItem.CutVolumes, tessItem );
										}
									}
									//}
								}
							}

							//render layers
							if( ( ( meshItem2.Flags & FrameData.MeshItem.FlagsEnum.ContainsTransparentLayersOnTransparentBaseObjects ) != 0 ) && DebugDrawLayers )
							{
								if( meshItem.Layers != null )
								{
									for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
									{
										ref var layer = ref meshItem.Layers[ nLayer ];
										foreach( var materialData in GetLayerMaterialData( ref layer, true, false ) )
										{
											//if( materialData == null )
											//	continue;

											bool add = materialData.Transparent;
											if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
												add = false;
											if( !add )
												continue;

											if( materialData.AllPasses.Count != 0 )
											{
												var pass = materialData.GetForwardShadingPass( directionalAmbientOnly, directionalAmbientOnlyPrepareShadows, false );
												//var pass = materialData.forwardShadingPass;

												//var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
												//bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

												//GpuMaterialPass pass;
												//if( receiveShadows )
												//{
												//	pass = passesGroup.passWithShadows;
												//	//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
												//	//	pass = passesGroup.passWithShadowsHigh;
												//	//else
												//	//	pass = passesGroup.passWithShadowsLow;
												//}
												//else
												//	pass = passesGroup.passWithoutShadows;

												ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, true );

												fixed( Matrix4F* p = &meshItem.TransformRelative )
													Bgfx.SetTransform( (float*)p );

												var color = /*meshItem.Color * */ layer.MaterialColor;

												for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
												{
													var oper = meshData.RenderOperations[ nOperation ];
													var voxelRendering = oper.VoxelDataInfo != null;

													//bind material data
													BindMaterialData( context, materialData, false, voxelRendering );
													BindSamplersForTextureOnlySlots( context, false, voxelRendering );
													materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );
													//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

													BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, oper.VoxelDataImage, oper.VoxelDataInfo, meshItem.ObjectInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative, layer.UVScale );

													RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
												}
											}
											//}
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

						ref var billboardItem2 = ref frameDataBillboards.Data[ renderableGroup.Y ];
						ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup.Y ];

						var meshData = Billboard.GetBillboardMesh().Result.MeshData;

						//get data for instancing
						int additionInstancingRendered = 0;

						//!!!!need?
						//!!!!need?
						//!!!!need?

						//if( Instancing && InstancingTransparent )
						//{
						//	int nRenderableGroupsToDraw2 = nRenderableGroupsToDraw + 1;
						//	while( nRenderableGroupsToDraw2 < renderableGroupsToDraw.Count )
						//	{
						//		if( additionInstancingRendered >= InstancingMaxCount )
						//			break;

						//		var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw2 ].RenderableGroup;
						//		if( renderableGroup2.X == 1 )
						//		{
						//			ref var billboardItem2_2 = ref frameDataBillboards.Data[ renderableGroup2.Y ];
						//			if( billboardItem2.CanUseInstancingForTransparentWith( ref billboardItem2_2 ) )
						//			{
						//				ref var billboardItem_2 = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup2.Y ];
						//				if( billboardItem.CanUseInstancingForTransparentWith( ref billboardItem_2 ) )
						//				{
						//					additionInstancingRendered++;
						//					nRenderableGroupsToDraw2++;
						//					continue;
						//				}
						//			}
						//		}

						//		break;
						//	}

						//	if( additionInstancingRendered < 2/*InstancingMinCount*/ )
						//		additionInstancingRendered = 0;
						//}
						bool instancing = additionInstancingRendered != 0;


						//init instance buffer
						int instanceCount = 0;
						//InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;
						if( instancing )
						{
							int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
							instanceCount = additionInstancingRendered + 1;

							instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
							if( instanceBuffer.Valid )
							{
								//get instancing matrices
								RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
								int currentMatrix = 0;
								for( int n = 0; n < instanceCount; n++ )
								{
									var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw + n ].RenderableGroup;

									ref var billboardItem_2 = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup2.Y ];
									billboardItem_2.GetInstancingData( out instancingData[ currentMatrix++ ] );
								}
							}
						}


						for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
						{
							var oper = meshData.RenderOperations[ nOperation ];
							var voxelRendering = oper.VoxelDataInfo != null;

							foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, false ) )
							{
								bool add = materialData.Transparent;
								if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
									add = false;
								if( !add )
									continue;

								if( materialData.AllPasses.Count != 0 )
								{
									var pass = materialData.GetForwardShadingPass( directionalAmbientOnly, directionalAmbientOnlyPrepareShadows, voxelRendering );
									//var pass = materialData.forwardShadingPass;

									//var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
									//bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

									//GpuMaterialPass pass;
									//if( receiveShadows )
									//{
									//	pass = passesGroup.passWithShadows;
									//	//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
									//	//	pass = passesGroup.passWithShadowsHigh;
									//	//else
									//	//	pass = passesGroup.passWithShadowsLow;
									//}
									//else
									//	pass = passesGroup.passWithoutShadows;

									ForwardBindGeneralTexturesUniforms( context, frameData/*, ref billboardItem.BoundingSphere, lightItem, receiveShadows*/, true );

									BindMaterialData( context, materialData, false, voxelRendering );
									BindSamplersForTextureOnlySlots( context, false, oper.VoxelDataInfo != null );
									//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

									BindRenderOperationData( context, frameData, materialData, instancing, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, oper.UnwrappedUV, ref billboardItem.Color, oper.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, oper.VoxelDataImage, oper.VoxelDataInfo, instancing ? null : billboardItem.MaterialInstanceParameters, 0, ref vector3FZero );

									if( instancing )
									{
										if( instanceBuffer.Valid )
											RenderOperation( context, oper, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
									}
									else
									{
										billboardItem.GetWorldMatrixRelative( out var worldMatrix );
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

			Bgfx.Discard( DiscardFlags.All );
		}

		//[MethodImpl( (MethodImplOptions)512 )]
		//protected unsafe virtual void Render3DSceneForwardTransparentWithoutOIT( ViewportRenderingContext context, FrameData frameData, ImageComponent colorDepthTextureCopy )
		//{
		//	var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
		//	var frameDataRenderSceneDataBillboards = frameData.RenderSceneData.Billboards;
		//	var frameDataMeshes = frameData.Meshes;
		//	var frameDataBillboards = frameData.Billboards;

		//	var viewportOwner = context.Owner;

		//	using( var renderableGroupsToDraw = new OpenListNative<RenderableGroupWithDistance>( frameData.RenderableGroupsInFrustum.Count ) )
		//	{
		//		foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
		//		{
		//			bool add = false;

		//			if( renderableGroup.X == 0 )
		//			{
		//				ref var data = ref frameDataMeshes.Data[ renderableGroup.Y ];
		//				add = ( data.Flags & FrameData.MeshItem.FlagsEnum.UseForwardTransparent ) != 0;
		//			}
		//			else
		//			{
		//				ref var data = ref frameDataBillboards.Data[ renderableGroup.Y ];
		//				add = ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseForwardTransparent ) != 0;
		//			}

		//			if( add )
		//			{
		//				var item = new RenderableGroupWithDistance();
		//				item.RenderableGroup = renderableGroup;
		//				item.DistanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.RenderableGroup );
		//				ApplyTransparentRenderingAddOffsetWhenSortByDistance( frameData, ref item );
		//				renderableGroupsToDraw.Add( item );
		//			}
		//		}

		//		if( renderableGroupsToDraw.Count == 0 )
		//			return;

		//		//sort by distance
		//		CollectionUtility.MergeSortUnmanaged( renderableGroupsToDraw.Data, renderableGroupsToDraw.Count, delegate ( RenderableGroupWithDistance* a, RenderableGroupWithDistance* b )
		//		{
		//			if( a->DistanceSquared > b->DistanceSquared )
		//				return -1;
		//			if( a->DistanceSquared < b->DistanceSquared )
		//				return 1;
		//			return 0;
		//		}, true );


		//		LightItem lightItemBinded = null;
		//		//Material.CompiledMaterialData materialBinded = null;

		//		int ambientDirectionalLightCount = 0;
		//		foreach( var lightIndex in frameData.LightsInFrustumSorted )
		//		{
		//			var lightItem = frameData.Lights[ lightIndex ];
		//			if( lightItem.data.Type == Light.TypeEnum.Point || lightItem.data.Type == Light.TypeEnum.Spotlight )
		//				break;
		//			ambientDirectionalLightCount++;
		//		}

		//		int[] tempIntArray = null;
		//		int[] GetTempIntArray( int minSize )
		//		{
		//			if( tempIntArray == null || tempIntArray.Length < minSize )
		//				tempIntArray = new int[ minSize ];
		//			return tempIntArray;
		//		}


		//		//bind textures for all render operations
		//		context.BindTexture( 5/*"s_colorDepthTextureCopy"*/, colorDepthTextureCopy ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None, 0, false );
		//		//context.BindTexture( 5/*"s_colorDepthTextureCopy"*/, colorDepthTextureCopy ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point );
		//		BindBrdfLUT( context );
		//		BindMaterialsTexture( context, frameData );
		//		BindBonesTexture( context, frameData );
		//		BindSamplersForTextureOnlySlots( context, true, false );
		//		BindMaterialData( context, null, false, false );

		//		InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;

		//		//draw
		//		for( int nRenderableGroupsToDraw = 0; nRenderableGroupsToDraw < renderableGroupsToDraw.Count; nRenderableGroupsToDraw++ )
		//		{
		//			var renderableGroup = renderableGroupsToDraw[ nRenderableGroupsToDraw ].RenderableGroup;

		//			if( renderableGroup.X == 0 )
		//			{
		//				//Mesh rendering

		//				ref var meshItem2 = ref frameDataMeshes.Data[ renderableGroup.Y ];
		//				ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];
		//				var meshData = meshItem.MeshData;

		//				//!!!!не всегда нужно
		//				var affectedLightsCount = ambientDirectionalLightCount + meshItem2.PointSpotLightCount;
		//				var affectedLights = GetTempIntArray( affectedLightsCount );//var affectedLights = stackalloc int[ affectedLightsCount ];
		//				for( int n = 0; n < ambientDirectionalLightCount; n++ )
		//					affectedLights[ n ] = frameData.LightsInFrustumSorted[ n ];
		//				for( int n = 0; n < meshItem2.PointSpotLightCount; n++ )
		//					affectedLights[ ambientDirectionalLightCount + n ] = meshItem2.GetPointSpotLight( n );

		//				//get data for instancing
		//				int additionInstancingRendered = 0;
		//				if( Instancing && InstancingTransparent )
		//				{
		//					int nRenderableGroupsToDraw2 = nRenderableGroupsToDraw + 1;
		//					while( nRenderableGroupsToDraw2 < renderableGroupsToDraw.Count )
		//					{
		//						if( additionInstancingRendered >= InstancingMaxCount )
		//							break;


		//						//!!!!probes?


		//						var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw2 ].RenderableGroup;
		//						if( renderableGroup2.X == 0 )
		//						{
		//							ref var meshItem2_2 = ref frameDataMeshes.Data[ renderableGroup2.Y ];
		//							if( meshItem2.CanUseInstancingForTransparentWith( ref meshItem2_2 ) )
		//							{
		//								ref var meshItem_2 = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup2.Y ];
		//								if( meshItem.CanUseInstancingForTransparentWith( ref meshItem_2 ) )
		//								{
		//									additionInstancingRendered++;
		//									nRenderableGroupsToDraw2++;
		//									continue;
		//								}
		//							}
		//						}

		//						break;
		//					}

		//					if( additionInstancingRendered < 2/*InstancingMinCount*/ )
		//						additionInstancingRendered = 0;
		//				}
		//				bool instancing = additionInstancingRendered != 0;


		//				//init instance buffer
		//				int instanceCount = 0;
		//				//InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;
		//				if( instancing )
		//				{
		//					int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
		//					instanceCount = additionInstancingRendered + 1;

		//					instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
		//					if( instanceBuffer.Valid )
		//					{
		//						//get instancing matrices
		//						RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
		//						int currentMatrix = 0;
		//						for( int n = 0; n < instanceCount; n++ )
		//						{
		//							var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw + n ].RenderableGroup;

		//							ref var meshItem_2 = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup2.Y ];
		//							meshItem_2.GetInstancingData( out instancingData[ currentMatrix++ ] );
		//						}
		//					}
		//				}


		//				//render mesh item
		//				if( !meshItem.OnlyForShadowGeneration )
		//				{
		//					for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//					{
		//						var oper = meshData.RenderOperations[ nOperation ];
		//						var voxelRendering = oper.VoxelDataImage != null;

		//						foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, false ) )
		//						{
		//							bool add = materialData.Transparent;
		//							if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//								add = false;
		//							if( !add )
		//								continue;

		//							for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
		//							{
		//								var lightIndex = affectedLights[ nAffectedLightIndex ];
		//								var lightItem = frameData.Lights[ lightIndex ];

		//								if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		//								{
		//									//set lightItem uniforms
		//									if( lightItemBinded != lightItem )
		//									{
		//										lightItem.Bind( this, context );
		//										lightItemBinded = lightItem;
		//									}

		//									var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//									bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//									GpuMaterialPass pass;
		//									if( receiveShadows )
		//									{
		//										pass = passesGroup.passWithShadows;
		//										//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//										//	pass = passesGroup.passWithShadowsHigh;
		//										//else
		//										//	pass = passesGroup.passWithShadowsLow;
		//									}
		//									else
		//										pass = passesGroup.passWithoutShadows;

		//									ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

		//									BindMaterialData( context, materialData, false, voxelRendering );
		//									BindSamplersForTextureOnlySlots( context, false, oper.VoxelDataInfo != null );
		//									//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

		//									BindRenderOperationData( context, frameData, materialData, instancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref meshItem.Color, oper.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, oper.VoxelDataImage, oper.VoxelDataInfo, instancing ? null : meshItem.MaterialInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

		//									if( instancing )
		//									{
		//										if( instanceBuffer.Valid )
		//											RenderOperation( context, oper, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
		//									}
		//									else
		//									{
		//										fixed( Matrix4F* p = &meshItem.TransformRelative )
		//											Bgfx.SetTransform( (float*)p );
		//										RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
		//									}
		//								}
		//							}
		//						}
		//					}

		//					//render layers
		//					if( ( ( meshItem2.Flags & FrameData.MeshItem.FlagsEnum.ContainsTransparentLayersOnTransparentBaseObjects ) != 0 ) && DebugDrawLayers )
		//					{
		//						if( meshItem.Layers != null )
		//						{
		//							for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
		//							{
		//								ref var layer = ref meshItem.Layers[ nLayer ];
		//								foreach( var materialData in GetLayerMaterialData( ref layer, true, false ) )
		//								{
		//									//if( materialData == null )
		//									//	continue;

		//									bool add = materialData.Transparent;
		//									if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//										add = false;
		//									if( !add )
		//										continue;

		//									for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
		//									{
		//										var lightIndex = affectedLights[ nAffectedLightIndex ];
		//										var lightItem = frameData.Lights[ lightIndex ];

		//										if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		//										{
		//											//set lightItem uniforms
		//											if( lightItemBinded != lightItem )
		//											{
		//												lightItem.Bind( this, context );
		//												lightItemBinded = lightItem;
		//											}

		//											var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//											bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//											GpuMaterialPass pass;
		//											if( receiveShadows )
		//											{
		//												pass = passesGroup.passWithShadows;
		//												//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//												//	pass = passesGroup.passWithShadowsHigh;
		//												//else
		//												//	pass = passesGroup.passWithShadowsLow;
		//											}
		//											else
		//												pass = passesGroup.passWithoutShadows;

		//											ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true, false );

		//											fixed( Matrix4F* p = &meshItem.TransformRelative )
		//												Bgfx.SetTransform( (float*)p );

		//											var color = /*meshItem.Color * */ layer.MaterialColor;

		//											for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//											{
		//												var oper = meshData.RenderOperations[ nOperation ];
		//												var voxelRendering = oper.VoxelDataInfo != null;

		//												//bind material data
		//												BindMaterialData( context, materialData, false, voxelRendering );
		//												BindSamplersForTextureOnlySlots( context, false, voxelRendering );
		//												materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );
		//												//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

		//												BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, oper.VoxelDataImage, oper.VoxelDataInfo, meshItem.MaterialInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

		//												RenderOperation( context, oper, pass, null, meshItem.CutVolumes );
		//											}
		//										}
		//									}
		//								}
		//							}
		//						}
		//					}
		//				}

		//				nRenderableGroupsToDraw += additionInstancingRendered;
		//			}
		//			else if( renderableGroup.X == 1 )
		//			{
		//				//Billboard rendering

		//				ref var billboardItem2 = ref frameDataBillboards.Data[ renderableGroup.Y ];
		//				ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup.Y ];

		//				//!!!!не всегда нужно
		//				var affectedLightsCount = ambientDirectionalLightCount + billboardItem2.PointSpotLightCount;
		//				var affectedLights = GetTempIntArray( affectedLightsCount );//var affectedLights = stackalloc int[ affectedLightsCount ];
		//				for( int n = 0; n < ambientDirectionalLightCount; n++ )
		//					affectedLights[ n ] = frameData.LightsInFrustumSorted[ n ];
		//				for( int n = 0; n < billboardItem2.PointSpotLightCount; n++ )
		//					affectedLights[ ambientDirectionalLightCount + n ] = billboardItem2.GetPointSpotLight( n );

		//				var meshData = Billboard.GetBillboardMesh().Result.MeshData;

		//				//get data for instancing
		//				int additionInstancingRendered = 0;
		//				if( Instancing && InstancingTransparent )
		//				{
		//					int nRenderableGroupsToDraw2 = nRenderableGroupsToDraw + 1;
		//					while( nRenderableGroupsToDraw2 < renderableGroupsToDraw.Count )
		//					{
		//						if( additionInstancingRendered >= InstancingMaxCount )
		//							break;

		//						var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw2 ].RenderableGroup;
		//						if( renderableGroup2.X == 1 )
		//						{
		//							ref var billboardItem2_2 = ref frameDataBillboards.Data[ renderableGroup2.Y ];
		//							if( billboardItem2.CanUseInstancingForTransparentWith( ref billboardItem2_2 ) )
		//							{
		//								ref var billboardItem_2 = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup2.Y ];
		//								if( billboardItem.CanUseInstancingForTransparentWith( ref billboardItem_2 ) )
		//								{
		//									additionInstancingRendered++;
		//									nRenderableGroupsToDraw2++;
		//									continue;
		//								}
		//							}
		//						}

		//						break;
		//					}

		//					if( additionInstancingRendered < 2/*InstancingMinCount*/ )
		//						additionInstancingRendered = 0;
		//				}
		//				bool instancing = additionInstancingRendered != 0;


		//				//init instance buffer
		//				int instanceCount = 0;
		//				//InstanceDataBuffer instanceBuffer = InstanceDataBuffer.Invalid;
		//				if( instancing )
		//				{
		//					int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
		//					instanceCount = additionInstancingRendered + 1;

		//					instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
		//					if( instanceBuffer.Valid )
		//					{
		//						//get instancing matrices
		//						RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
		//						int currentMatrix = 0;
		//						for( int n = 0; n < instanceCount; n++ )
		//						{
		//							var renderableGroup2 = renderableGroupsToDraw[ nRenderableGroupsToDraw + n ].RenderableGroup;

		//							ref var billboardItem_2 = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup2.Y ];
		//							billboardItem_2.GetInstancingData( out instancingData[ currentMatrix++ ] );
		//						}
		//					}
		//				}


		//				for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
		//				{
		//					var oper = meshData.RenderOperations[ nOperation ];
		//					var voxelRendering = oper.VoxelDataInfo != null;

		//					foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, false ) )
		//					{
		//						bool add = materialData.Transparent;
		//						if( materialData.deferredShadingSupport && context.DeferredShading && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
		//							add = false;
		//						if( !add )
		//							continue;

		//						for( int nAffectedLightIndex = 0; nAffectedLightIndex < affectedLightsCount; nAffectedLightIndex++ )
		//						{
		//							var lightIndex = affectedLights[ nAffectedLightIndex ];
		//							var lightItem = frameData.Lights[ lightIndex ];

		//							if( materialData.AllPasses.Count != 0 && (int)lightItem.data.Type < materialData.passesByLightType.Length )
		//							{
		//								//set lightItem uniforms
		//								if( lightItemBinded != lightItem )
		//								{
		//									lightItem.Bind( this, context );
		//									lightItemBinded = lightItem;
		//								}

		//								var passesGroup = materialData.passesByLightType[ (int)lightItem.data.Type ];
		//								bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadows != null;

		//								GpuMaterialPass pass;
		//								if( receiveShadows )
		//								{
		//									pass = passesGroup.passWithShadows;
		//									//if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
		//									//	pass = passesGroup.passWithShadowsHigh;
		//									//else
		//									//	pass = passesGroup.passWithShadowsLow;
		//								}
		//								else
		//									pass = passesGroup.passWithoutShadows;

		//								ForwardBindGeneralTexturesUniforms( context, frameData, ref billboardItem.BoundingSphere, lightItem, receiveShadows, true, false );

		//								BindMaterialData( context, materialData, false, voxelRendering );
		//								BindSamplersForTextureOnlySlots( context, false, oper.VoxelDataInfo != null );
		//								//materialData.BindCurrentFrameDataDepthTexture( context, depthTextureCopy );

		//								BindRenderOperationData( context, frameData, materialData, instancing, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, billboardItem.ReceiveDecals, ref billboardItem.PreviousFramePositionChange, 0, oper.UnwrappedUV, ref billboardItem.Color, oper.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, oper.VoxelDataImage, oper.VoxelDataInfo, instancing ? null : billboardItem.MaterialInstanceParameters, 0, ref vector3FZero );

		//								if( instancing )
		//								{
		//									if( instanceBuffer.Valid )
		//										RenderOperation( context, oper, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
		//								}
		//								else
		//								{
		//									billboardItem.GetWorldMatrixRelative( out var worldMatrix );
		//									Bgfx.SetTransform( (float*)&worldMatrix );
		//									RenderOperation( context, oper, pass, null, null );
		//								}
		//							}
		//						}
		//					}
		//				}

		//				nRenderableGroupsToDraw += additionInstancingRendered;
		//			}
		//		}
		//	}
		//}

		void RenderEffectsInternal( ViewportRenderingContext context, FrameData frameData, string groupName, ref ImageComponent actualTexture, bool onlyAO )
		{
			var group = GetComponent( groupName, true );
			if( group != null )
			{
				var rendered = false;

				foreach( var effect in group.GetComponents<RenderingEffect>( false, false, true ) )
				{
					if( effect.IsSupported )
					{
						bool isAO = effect is RenderingEffect_AmbientOcclusion;
						if( onlyAO && isAO || !onlyAO && !isAO )
						{
							effect.Render( context, frameData, ref actualTexture );
							rendered = true;
						}
					}
				}

				if( rendered )
					Bgfx.Discard( DiscardFlags.All );
			}
		}

		public void CopyToCurrentViewport( ViewportRenderingContext context, ImageComponent sourceTexture, CanvasRenderer.BlendingType blending = CanvasRenderer.BlendingType.Opaque, FilterOption filtering = FilterOption.Point, bool flipY = false, bool sourceIs2DArray = false, int arrayIndex = 0 )
		{
			var shader = new CanvasRenderer.ShaderItem();
			shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
			shader.FragmentProgramFileName = @"Base\Shaders\Copy_fs.sc";

			//bind textures, defines
			{
				//var filter = context.CurrentViewport.SizeInPixels != sourceTexture.Result.ResultSize ? FilterOption.Linear : FilterOption.Point;

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, sourceTexture, TextureAddressingMode.Clamp, filtering, filtering, FilterOption.Point ) );

				if( sourceIs2DArray )
				{
					shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "SOURCE_IS_2D_ARRAY" ) );
					shader.Parameters.Set( "u_copyArrayIndex", new Vector4F( arrayIndex, 0, 0, 0 ) );
				}

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

				var pass = new GpuMaterialPass( null, vertexProgram, fragmentProgram );

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

				context.ConvertToRelative( cameraSettings.Position, out var positionRelative );
				var worldMatrix = new Matrix4F( Matrix3F.FromScale( (float)scale ), positionRelative );
				//var worldMatrix = new Matrix4F( Matrix3F.FromScale( (float)scale ), cameraSettings.Position.ToVector3F() );

				foreach( var item in deferredShadingData.clearBackgroundMesh.Result.MeshData.RenderOperations )
				{
					var generalContainer = new ParameterContainer();
					generalContainer.Set( "backgroundColor", context.GetBackgroundColor() );

					var containers = new List<ParameterContainer>();
					containers.Add( generalContainer );

					Bgfx.SetTransform( (float*)&worldMatrix );
					RenderOperation( context, item, deferredShadingData.clearBackgroundPass, containers, null );
					Bgfx.Discard( DiscardFlags.All );
				}
			}
		}

		public delegate void RenderDeferredShadingGBufferReadyDelegate( RenderingPipeline_Basic sender, ViewportRenderingContext context, FrameData frameData, ref ImageComponent sceneTexture );
		public event RenderDeferredShadingGBufferReadyDelegate RenderDeferredShadingGBufferReady;

		//public delegate void RenderDeferredShadingEndDelegate( RenderingPipeline_Basic sender, ViewportRenderingContext context, FrameData frameData, ref ImageComponent sceneTexture );
		//public event RenderDeferredShadingEndDelegate RenderDeferredShadingEnd;

		public T GetSceneEffect<T>() where T : RenderingEffect
		{
			var group = GetComponent( "Scene Effects", true );
			if( group != null )
				return group.GetComponent<T>( false, true );
			return null;
		}

		//!!!!
		//TAA jittering test
		//static long jitteringCounter;

		//Android only
		[MethodImpl( (MethodImplOptions)512 )]
		int GetAttachedToShadowMapsMaskCount( FrameData frameData, Light.TypeEnum lightType )
		{
			if( RenderingSystem.LightMask )
			{
				var dictionary = new EDictionary<ImageComponent, int>();
				//var maxSize = 0;

				foreach( var lightIndex in frameData.LightsInFrustumSorted )
				{
					var lightItem = frameData.Lights[ lightIndex ];
					var lightData = lightItem.data;

					if( lightData.Type == lightType )
					{
						//lightItem.lightMaskIndex = -1;

						var mask = lightData.Mask;
						if( mask?.Result != null )
						{
							if( !dictionary.TryGetValue( mask, out var index ) )
							{
								index = dictionary.Count;
								dictionary[ mask ] = index;
								//var size = mask.Result.ResultSize.X;
								//if( size > maxSize )
								//	maxSize = size;
							}
							//lightItem.lightMaskIndex = index;
						}
					}
				}

				return dictionary.Count;
			}

			return 0;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected virtual void RenderWithRenderTargets( ViewportRenderingContext context, FrameData frameData )
		{
			var owner = context.Owner;

			//prepare tessellation
			TessellationPrepare( context );

			/////////////////////////////////////
			//prepare shadows
			{
				var existsShadows = false;
				{
					foreach( var lightIndex in frameData.LightsInFrustumSorted )
					{
						var lightItem = frameData.Lights[ lightIndex ];
						if( lightItem.prepareShadows )
						{
							existsShadows = true;
							break;
						}
					}
				}

				if( existsShadows )
				{
					var drawCallsBeforeShadows = context.updateStatisticsCurrent.DrawCalls;

					duringRenderShadows = true;
					SetCutVolumeSettingsUniforms( context, null, true );

					//allocate shadow arrays for point, spotlights
					{
						var textureFormat = RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4 ? PixelFormat.A8R8G8B8 : PixelFormat.Float32R;

						var spotCount = ShadowSpotlightMaxCount.Value;
						var pointCount = ShadowPointLightMaxCount.Value;

						frameData.ShadowTextureArraySpotUsedForShadows = spotCount;
						frameData.ShadowTextureArrayPointUsedForShadows = pointCount;


						//var spotCount = int.MaxValue;
						//var pointCount = int.MaxValue;

						////minimize memory use on limited device
						//if( SystemSettings.LimitedDevice )
						//{
						//	foreach( var lightIndex in frameData.LightsInFrustumSorted )
						//	{
						//		var item = frameData.Lights[ lightIndex ];
						//		if( item.prepareShadows )
						//		{
						//			if( item.data.Type == Light.TypeEnum.Spotlight )
						//				spotCount++;
						//			else if( item.data.Type == Light.TypeEnum.Point )
						//				pointCount++;
						//		}
						//	}

						//	//var realSpotCount = 0;
						//	//var realPointCount = 0;

						//	//foreach( var lightIndex in frameData.LightsInFrustumSorted )
						//	//{
						//	//	var item = frameData.Lights[ lightIndex ];
						//	//	if( item.prepareShadows )
						//	//	{
						//	//		if( item.data.Type == Light.TypeEnum.Spotlight )
						//	//			realSpotCount++;
						//	//		else if( item.data.Type == Light.TypeEnum.Point )
						//	//			realPointCount++;
						//	//	}
						//	//}

						//	//if( realSpotCount < 1 )
						//	//	realSpotCount = 1;
						//	//if( realPointCount < 1 )
						//	//	realPointCount = 1;

						//	//if( realSpotCount < spotCount )
						//	//	spotCount = realSpotCount;
						//	//if( realPointCount < pointCount )
						//	//	pointCount = realPointCount;
						//}

						//spotCount = Math.Min( spotCount, ShadowSpotlightMaxCount.Value );
						//pointCount = Math.Min( pointCount, ShadowPointLightMaxCount.Value );


						//add slices for masks
						if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
						{
							spotCount += GetAttachedToShadowMapsMaskCount( frameData, Light.TypeEnum.Spotlight );
							pointCount += GetAttachedToShadowMapsMaskCount( frameData, Light.TypeEnum.Point );
						}


						//!!!!workaround for Android. 1 depth arrays are not arrays

						if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
						{
							if( spotCount == 1 )
								spotCount = 2;
							if( pointCount == 1 )
								pointCount = 2;
						}


						if( spotCount != 0 )
						{
							var textureSize = GetShadowMapSizeMultiLightSpot();
							frameData.ShadowTextureArraySpot = context.RenderTarget2D_Alloc( new Vector2I( textureSize, textureSize ), textureFormat, arrayLayers: spotCount );
						}

						if( pointCount != 0 )
						{
							var textureSize = GetShadowMapSizeMultiLightPoint();
							frameData.ShadowTextureArrayPoint = context.RenderTargetCube_Alloc( new Vector2I( textureSize, textureSize ), textureFormat, arrayLayers: pointCount );
						}
					}

					//update Light.UniqueIdentifierForStaticShadows
					foreach( var lightIndex in frameData.LightsInFrustumSorted )
					{
						var lightItem = frameData.Lights[ lightIndex ];
						var lightData = lightItem.data;

						if( lightData.StaticShadows )
						{
							var light = lightData.Creator;// as Light;
							if( light != null )
								lightData.UniqueIdentifierForStaticShadows = light.GetUniqueIdentifierForStaticShadows();
						}
					}

					//static shadows
					if( context.LightStaticShadowsItems == null )
						context.LightStaticShadowsItems = new Dictionary<(long, int), ViewportRenderingContext.LightStaticShadowsItem>();

					//static shadows. update LastTimeIsUsed
					{
						foreach( var item in context.LightStaticShadowsItems.Values )
							item.LastTimeIsUsed = false;

						//process per light
						foreach( var lightIndex in frameData.LightsInFrustumSorted )
						{
							var item = frameData.Lights[ lightIndex ];
							if( item.prepareShadows )
							{
								var needStaticShadows = item.data.UniqueIdentifierForStaticShadows != 0L;
								if( needStaticShadows )
								{
									var key = (item.data.UniqueIdentifierForStaticShadows, GetShadowMapSize( item ));

									if( context.LightStaticShadowsItems.TryGetValue( key, out var lightStaticShadowsItem ) )
										lightStaticShadowsItem.LastTimeIsUsed = true;
								}
							}
						}
					}

					//static shadows. select static shadows items which can be reused
					//key is size. +size is for point lights, -size for spolights
					var staticShadowsFreeItems = new Dictionary<int, Stack<ViewportRenderingContext.LightStaticShadowsItem>>( context.LightStaticShadowsItems.Count );
					{
						List<(long, int)> toRemove = null;

						foreach( var item in context.LightStaticShadowsItems.Values )
						{
							if( !item.LastTimeIsUsed )
							{
								var textureSize = item.Image.Result.ResultSize.X;
								var isPointLight = item.Image.Result.ArrayLayers == 6;

								if( toRemove == null )
									toRemove = new List<(long, int)>();
								toRemove.Add( (item.UniqueIdentifierForStaticShadows, textureSize) );

								var key = textureSize * ( isPointLight ? 1 : -1 );
								if( !staticShadowsFreeItems.TryGetValue( key, out var stack ) )
								{
									stack = new Stack<ViewportRenderingContext.LightStaticShadowsItem>();
									staticShadowsFreeItems[ key ] = stack;
								}
								stack.Push( item );
							}
						}

						if( toRemove != null )
						{
							foreach( var key in toRemove )
							{
								if( !context.LightStaticShadowsItems.Remove( key ) )
									Log.Fatal( "RenderingPipeline: RenderWithRenderTargets: Internal error." );
							}
						}
					}

					//select one static shadows item to update depending update time. it is periodical update to refresh with better lods
					ViewportRenderingContext.LightStaticShadowsItem forceUpdateStaticShadowsItem = null;
					{
						foreach( var item in context.LightStaticShadowsItems.Values )
						{
							if( forceUpdateStaticShadowsItem == null || item.LastUpdateTime < forceUpdateStaticShadowsItem.LastUpdateTime )
								forceUpdateStaticShadowsItem = item;

							//if( forceUpdateStaticShadowsItem == null || item.LastUpdateTime > forceUpdateStaticShadowsItem.LastUpdateTime )
							//	forceUpdateStaticShadowsItem = item;
						}
					}

					//var staticShadowsLimit = 3;

					//process per light
					foreach( var lightIndex in frameData.LightsInFrustumSorted )
					{
						var item = frameData.Lights[ lightIndex ];
						if( item.prepareShadows )
						{
							var lightData = item.data;
							var needStaticShadows = item.data.UniqueIdentifierForStaticShadows != 0L;

							//prepare dynamic shadows
							var dynamicShadowTexture = LightPrepareShadows( context, frameData, item, null, needStaticShadows, null, null );//, ref staticShadowsLimit );

							if( needStaticShadows )
							{
								//mix dynamic and static shadows

								//prepare static shadows
								var lightStaticShadowsItemKey = (item.data.UniqueIdentifierForStaticShadows, GetShadowMapSize( item ));
								if( !context.LightStaticShadowsItems.TryGetValue( lightStaticShadowsItemKey, out var lightStaticShadowsItem ) )
								{
									lightStaticShadowsItem = new ViewportRenderingContext.LightStaticShadowsItem();
									context.LightStaticShadowsItems[ lightStaticShadowsItemKey ] = lightStaticShadowsItem;

									lightStaticShadowsItem.UniqueIdentifierForStaticShadows = item.data.UniqueIdentifierForStaticShadows;
									lightStaticShadowsItem.Image = LightPrepareShadows( context, frameData, item, lightStaticShadowsItem, needStaticShadows, staticShadowsFreeItems, null );//, ref staticShadowsLimit );

									lightStaticShadowsItem.LastUpdateTime = EngineApp.EngineTime;
								}
								else
								{
									//update already created early static shadows item
									if( forceUpdateStaticShadowsItem != null && forceUpdateStaticShadowsItem == lightStaticShadowsItem )
									{
										LightPrepareShadows( context, frameData, item, lightStaticShadowsItem, needStaticShadows, null, lightStaticShadowsItem.Image );//, ref staticShadowsLimit );

										lightStaticShadowsItem.LastUpdateTime = EngineApp.EngineTime;
									}
								}

								lightStaticShadowsItem.LastTimeIsUsed = true;
								//lightStaticShadowsItem.LastUpdateTime = EngineApp.EngineTime;


								//merge spot
								if( lightData.Type == Light.TypeEnum.Spotlight && frameData.ShadowTextureArraySpot != null )
								{
									var slice = item.shadowMapIndex;
									if( slice >= 0 )
									{
										var shadowViewport = frameData.ShadowTextureArraySpot.Result.GetRenderTarget( slice: slice ).Viewports[ 0 ];
										context.SetViewport( shadowViewport );

										var shader = new CanvasRenderer.ShaderItem();
										shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
										shader.FragmentProgramFileName = @"Base\Shaders\MergeShadows_fs.sc";

										shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, dynamicShadowTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
										shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, lightStaticShadowsItem.Image, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

										shader.Parameters.Set( "u_mergeShadowsParams", new Vector4F( 0, 0, 0, 0 ) );

										context.RenderQuadToCurrentViewport( shader );


										item.shadowCameraSettings = dynamicShadowTexture.Result.GetRenderTarget().Viewports[ 0 ].CameraSettings;
										item.shadowSourceTextureSize = dynamicShadowTexture.Result.ResultSize.X;
									}
								}

								//merge point
								if( lightData.Type == Light.TypeEnum.Point && frameData.ShadowTextureArrayPoint != null )
								{
									if( item.shadowMapIndex >= 0 )
									{
										for( int face = 0; face < 6; face++ )
										{
											var slice = item.shadowMapIndex * 6 + face;
											var shadowViewport = frameData.ShadowTextureArrayPoint.Result.GetRenderTarget( slice: slice ).Viewports[ 0 ];
											context.SetViewport( shadowViewport );

											var shader = new CanvasRenderer.ShaderItem();
											shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
											shader.FragmentProgramFileName = @"Base\Shaders\MergeShadows_fs.sc";

											shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, dynamicShadowTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
											shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, lightStaticShadowsItem.Image, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

											shader.Parameters.Set( "u_mergeShadowsParams", new Vector4F( face, 0, 0, 0 ) );

											context.RenderQuadToCurrentViewport( shader );
										}


										//for point light camera settings used only to get the projection matrix. face 0 is ok
										item.shadowCameraSettings = dynamicShadowTexture.Result.GetRenderTarget().Viewports[ 0 ].CameraSettings;
										item.shadowSourceTextureSize = dynamicShadowTexture.Result.ResultSize.X;
									}
								}


								////int faces = lightData.Type == Light.TypeEnum.Point ? 6 : 1;
								////for( int face = 0; face < faces; face++ )
								////{
								////	var mergedViewport = merged.Result.GetRenderTarget( 0, face ).Viewports[ 0 ];
								////	var dynamicShadowTextureViewport = dynamicShadowTexture.Result.GetRenderTarget( 0, face ).Viewports[ 0 ];

								////	//copy data
								////	{
								////		context.SetViewport( mergedViewport );

								////		var shader = new CanvasRenderer.ShaderItem();
								////		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
								////		shader.FragmentProgramFileName = @"Base\Shaders\MergeShadows_fs.sc";

								////		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, dynamicShadowTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
								////		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, lightStaticShadowsItem.Image, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

								////		//shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "POINT_LIGHT" ) );

								////		shader.Parameters.Set( "u_mergeShadowsParams", new Vector4F( face, 0, 0, 0 ) );

								////		context.RenderQuadToCurrentViewport( shader );
								////	}

								////	//copy camera settings
								////	mergedViewport.CameraSettings = dynamicShadowTextureViewport.CameraSettings;
								////}



								////shadowTexture = merged;


								////ImageComponent merged;
								////if( lightData.Type == Light.TypeEnum.Point )
								////{
								////	merged = context.RenderTarget2D_Alloc( dynamicShadowTexture.Result.ResultSize, dynamicShadowTexture.Result.ResultFormat, arrayLayers: 6 );
								////	//merged = context.RenderTargetCube_Alloc( dynamicShadowTexture.Result.ResultSize, dynamicShadowTexture.Result.ResultFormat );
								////}
								////else
								////	merged = context.RenderTarget2D_Alloc( dynamicShadowTexture.Result.ResultSize, dynamicShadowTexture.Result.ResultFormat );

								////int faces = lightData.Type == Light.TypeEnum.Point ? 6 : 1;
								////for( int face = 0; face < faces; face++ )
								////{
								////	var mergedViewport = merged.Result.GetRenderTarget( 0, face ).Viewports[ 0 ];
								////	var dynamicShadowTextureViewport = dynamicShadowTexture.Result.GetRenderTarget( 0, face ).Viewports[ 0 ];

								////	//copy data
								////	{
								////		context.SetViewport( mergedViewport );

								////		var shader = new CanvasRenderer.ShaderItem();
								////		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
								////		shader.FragmentProgramFileName = @"Base\Shaders\MergeShadows_fs.sc";

								////		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, dynamicShadowTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
								////		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, lightStaticShadowsItem.Image, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

								////		//shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "POINT_LIGHT" ) );

								////		shader.Parameters.Set( "u_mergeShadowsParams", new Vector4F( face, 0, 0, 0 ) );

								////		context.RenderQuadToCurrentViewport( shader );
								////	}

								////	//copy camera settings
								////	mergedViewport.CameraSettings = dynamicShadowTextureViewport.CameraSettings;
								////}

								////shadowTexture = merged;
								////}

								context.DynamicTexture_Free( dynamicShadowTexture );
							}
							else
							{
								//dynamic shadows only

								if( lightData.Type == Light.TypeEnum.Directional )
								{
									frameData.ShadowTextureArrayDirectional = dynamicShadowTexture;

									item.shadowCameraSettings = dynamicShadowTexture.Result.GetRenderTarget().Viewports[ 0 ].CameraSettings;
									item.shadowSourceTextureSize = dynamicShadowTexture.Result.ResultSize.X;
								}

								if( lightData.Type == Light.TypeEnum.Spotlight && frameData.ShadowTextureArraySpot != null )
								{
									var slice = item.shadowMapIndex;
									if( slice >= 0 )
									{
										var shadowViewport = frameData.ShadowTextureArraySpot.Result.GetRenderTarget( slice: slice ).Viewports[ 0 ];
										context.SetViewport( shadowViewport );
										CopyToCurrentViewport( context, dynamicShadowTexture );

										item.shadowCameraSettings = dynamicShadowTexture.Result.GetRenderTarget().Viewports[ 0 ].CameraSettings;
										item.shadowSourceTextureSize = dynamicShadowTexture.Result.ResultSize.X;
									}

									context.DynamicTexture_Free( dynamicShadowTexture );
								}

								if( lightData.Type == Light.TypeEnum.Point && frameData.ShadowTextureArrayPoint != null )
								{
									if( item.shadowMapIndex >= 0 )
									{
										for( int face = 0; face < 6; face++ )
										{
											var slice = item.shadowMapIndex * 6 + face;
											var shadowViewport = frameData.ShadowTextureArrayPoint.Result.GetRenderTarget( slice: slice ).Viewports[ 0 ];
											context.SetViewport( shadowViewport );
											CopyToCurrentViewport( context, dynamicShadowTexture, sourceIs2DArray: true, arrayIndex: face );
										}

										//for point light camera settings used only to get the projection matrix. face 0 is ok
										item.shadowCameraSettings = dynamicShadowTexture.Result.GetRenderTarget().Viewports[ 0 ].CameraSettings;
										item.shadowSourceTextureSize = dynamicShadowTexture.Result.ResultSize.X;
									}

									context.DynamicTexture_Free( dynamicShadowTexture );
								}
							}


							////if( lightData.Type == Light.TypeEnum.Spotlight && shadowTexture != null )
							////{
							////	if( item.prepareShadows && frameData.ShadowTextureArraySpot != null )
							////	{
							////		var slice = item.shadowMapIndex;
							////		if( slice >= 0 )
							////		{
							////			var shadowViewport = frameData.ShadowTextureArraySpot.Result.GetRenderTarget( slice: slice ).Viewports[ 0 ];
							////			context.SetViewport( shadowViewport );
							////			CopyToCurrentViewport( context, shadowTexture );

							////			item.shadowCameraSettings = shadowTexture.Result.GetRenderTarget().Viewports[ 0 ].CameraSettings;
							////			item.shadowSourceTextureSize = shadowTexture.Result.ResultSize.X;
							////		}
							////	}

							////	context.DynamicTexture_Free( shadowTexture );
							////}

							////if( lightData.Type == Light.TypeEnum.Point && shadowTexture != null )
							////{
							////	if( item.prepareShadows && frameData.ShadowTextureArrayPoint != null )
							////	{
							////		if( item.shadowMapIndex >= 0 )
							////		{
							////			for( int face = 0; face < 6; face++ )
							////			{
							////				var slice = item.shadowMapIndex * 6 + face;
							////				var shadowViewport = frameData.ShadowTextureArrayPoint.Result.GetRenderTarget( slice: slice ).Viewports[ 0 ];
							////				context.SetViewport( shadowViewport );

							////				CopyToCurrentViewport( context, shadowTexture, arrayIndex: face );

							////				//for point light camera settings used only to get the projection matrix. face 0 is ok
							////				item.shadowCameraSettings = shadowTexture.Result.GetRenderTarget().Viewports[ 0 ].CameraSettings;
							////				item.shadowSourceTextureSize = shadowTexture.Result.ResultSize.X;
							////			}
							////		}
							////	}

							////	context.DynamicTexture_Free( shadowTexture );
							////}


							//////add to namedTextures
							////{
							////	var prefix = "shadow" + lightData.Type.ToString();
							////	for( int counter = 1; ; counter++ )
							////	{
							////		var name = prefix + counter.ToString();
							////		if( !context.ObjectsDuringUpdate.namedTextures.ContainsKey( name ) )
							////		{
							////			context.ObjectsDuringUpdate.namedTextures[ name ] = shadowTexture;
							////			break;
							////		}
							////	}
							////}

							//////connect the shadow texture with the light
							////item.shadowTexture = shadowTexture;
						}
					}

					//remove unused static shadows items
					foreach( var stack in staticShadowsFreeItems.Values )
					{
						foreach( var item in stack )
							item.Dispose();
					}



					//////about static shadows
					////if( context.LightStaticShadowsItems == null )
					////	context.LightStaticShadowsItems = new Dictionary<long, ViewportRenderingContext.LightStaticShadowsItem>();
					////foreach( var item in context.LightStaticShadowsItems.Values )
					////	item.LastTimeIsUsed = false;

					//////process per light
					////foreach( var lightIndex in frameData.LightsInFrustumSorted )
					////{
					////	var item = frameData.Lights[ lightIndex ];
					////	if( item.prepareShadows )
					////	{
					////		var lightData = item.data;
					////		var needStaticShadows = item.data.UniqueIdentifierForStaticShadows != 0L;

					////		//dynamic shadows
					////		var dynamicShadowTexture = LightPrepareShadows( context, frameData, item, null, needStaticShadows );

					////		ImageComponent shadowTexture;

					////		if( needStaticShadows )
					////		{
					////			//static shadows

					////			if( !context.LightStaticShadowsItems.TryGetValue( item.data.UniqueIdentifierForStaticShadows, out var lightStaticShadowsItem ) )
					////			{
					////				lightStaticShadowsItem?.Dispose();

					////				lightStaticShadowsItem = new ViewportRenderingContext.LightStaticShadowsItem();
					////				context.LightStaticShadowsItems[ item.data.UniqueIdentifierForStaticShadows ] = lightStaticShadowsItem;

					////				lightStaticShadowsItem.UniqueIdentifierForStaticShadows = item.data.UniqueIdentifierForStaticShadows;
					////				lightStaticShadowsItem.Image = LightPrepareShadows( context, frameData, item, lightStaticShadowsItem, needStaticShadows );
					////			}

					////			lightStaticShadowsItem.LastTimeIsUsed = true;

					////			//merge
					////			{
					////				ImageComponent merged;
					////				if( lightData.Type == Light.TypeEnum.Point )
					////					merged = context.RenderTargetCube_Alloc( dynamicShadowTexture.Result.ResultSize, dynamicShadowTexture.Result.ResultFormat );
					////				else
					////					merged = context.RenderTarget2D_Alloc( dynamicShadowTexture.Result.ResultSize, dynamicShadowTexture.Result.ResultFormat );

					////				int faces = lightData.Type == Light.TypeEnum.Point ? 6 : 1;
					////				for( int face = 0; face < faces; face++ )
					////				{
					////					var mergedViewport = merged.Result.GetRenderTarget( 0, face ).Viewports[ 0 ];
					////					var dynamicShadowTextureViewport = dynamicShadowTexture.Result.GetRenderTarget( 0, face ).Viewports[ 0 ];

					////					//copy data
					////					{
					////						context.SetViewport( mergedViewport );

					////						var shader = new CanvasRenderer.ShaderItem();
					////						shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
					////						shader.FragmentProgramFileName = @"Base\Shaders\MergeShadows_fs.sc";

					////						shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, dynamicShadowTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
					////						shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, lightStaticShadowsItem.Image, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

					////						shader.Defines.Add( new CanvasRenderer.ShaderItem.DefineItem( "POINT_LIGHT" ) );

					////						shader.Parameters.Set( "u_mergeShadowsParams", new Vector4F( face, 0, 0, 0 ) );

					////						context.RenderQuadToCurrentViewport( shader );
					////					}

					////					//copy camera settings
					////					mergedViewport.CameraSettings = dynamicShadowTextureViewport.CameraSettings;
					////				}

					////				shadowTexture = merged;
					////			}

					////			context.DynamicTexture_Free( dynamicShadowTexture );
					////		}
					////		else
					////		{
					////			//dynamic shadows only
					////			shadowTexture = dynamicShadowTexture;
					////		}

					////		//add to namedTextures
					////		{
					////			var prefix = "shadow" + lightData.Type.ToString();
					////			for( int counter = 1; ; counter++ )
					////			{
					////				var name = prefix + counter.ToString();
					////				if( !context.ObjectsDuringUpdate.namedTextures.ContainsKey( name ) )
					////				{
					////					context.ObjectsDuringUpdate.namedTextures[ name ] = shadowTexture;
					////					break;
					////				}
					////			}
					////		}
					////		//connect the shadow texture with the light
					////		item.shadowTexture = shadowTexture;
					////	}
					////}

					//////remove unused static shadows items
					////{
					////	//!!!!может полезно чтобы не дребежжало (часто не создавалось, удалялось). добавить время неиспользования

					////	List<ViewportRenderingContext.LightStaticShadowsItem> toRemove = null;

					////	foreach( var item in context.LightStaticShadowsItems.Values )
					////	{
					////		if( !item.LastTimeIsUsed )
					////		{
					////			if( toRemove == null )
					////				toRemove = new List<ViewportRenderingContext.LightStaticShadowsItem>();
					////			toRemove.Add( item );
					////		}
					////	}

					////	if( toRemove != null )
					////	{
					////		foreach( var item in toRemove )
					////		{
					////			context.LightStaticShadowsItems.Remove( item.UniqueIdentifierForStaticShadows );
					////			item.Dispose();
					////		}
					////	}
					////}

					duringRenderShadows = false;
					SetCutVolumeSettingsUniforms( context, null, true );

					context.updateStatisticsCurrent.DrawCallsShadows = context.updateStatisticsCurrent.DrawCalls - drawCallsBeforeShadows;
				}
			}

			/////////////////////////////////////
			//prepare light masks

			var lightMasks = RenderingSystem.LightMask;
			if( lightMasks && owner.Mode == Viewport.ModeEnum.ReflectionProbeCubemap )
			{
				var probe = context.Owner.AnyData as ReflectionProbe;
				if( probe != null && !probe.LightMasks )
					lightMasks = false;
			}

			if( lightMasks )// RenderingSystem.LightMask )
			{

				//!!!!for modern graphics API use dynamic texture indexing


				if( SystemSettings.MobileDevice )
				{
					//mobile specific. on limited devices masks and shadow maps are managed inside one shadow map array

					for( var lightType = Light.TypeEnum.Directional; lightType <= Light.TypeEnum.Spotlight; lightType++ )
					{
						var dictionary = new EDictionary<ImageComponent, int>();
						var maxSize = 0;

						foreach( var lightIndex in frameData.LightsInFrustumSorted )
						{
							var lightItem = frameData.Lights[ lightIndex ];
							var lightData = lightItem.data;

							if( lightData.Type == lightType )
							{
								lightItem.lightMaskIndex = -1;

								var mask = lightData.Mask;
								if( mask?.Result != null )
								{
									if( !dictionary.TryGetValue( mask, out var index ) )
									{
										index = dictionary.Count;
										dictionary[ mask ] = index;
										var size = mask.Result.ResultSize.X;
										if( size > maxSize )
											maxSize = size;
									}
									lightItem.lightMaskIndex = index;

									//add offset used by shadow maps
									{
										switch( lightType )
										{
										case Light.TypeEnum.Directional:
											lightItem.lightMaskIndex += frameData.ShadowTextureArrayDirectionalUsedForShadows;
											break;
										case Light.TypeEnum.Spotlight:
											lightItem.lightMaskIndex += frameData.ShadowTextureArraySpotUsedForShadows;
											break;
										case Light.TypeEnum.Point:
											lightItem.lightMaskIndex += frameData.ShadowTextureArrayPointUsedForShadows;
											break;
										}
									}
								}
							}
						}

						if( dictionary.Count > 0 )
						{
							if( maxSize > RenderingSystem.Capabilities.MaxTextureSize )
								maxSize = RenderingSystem.Capabilities.MaxTextureSize;

							if( lightType == Light.TypeEnum.Point )
							{
								//point

								//create array if no shadows
								if( frameData.ShadowTextureArrayPoint == null )
								{
									//!!!!workaround for Android. 1 depth arrays are not arrays
									frameData.ShadowTextureArrayPoint = context.RenderTargetCube_Alloc( new Vector2I( maxSize, maxSize ), PixelFormat.A8R8G8B8, arrayLayers: Math.Max( dictionary.Count, 2 ) );
								}

								var arrayTexture = frameData.ShadowTextureArrayPoint;
								if( arrayTexture != null )
								{
									var index = frameData.ShadowTextureArrayPointUsedForShadows;

									foreach( var mask in dictionary.Keys )
									{
										if( mask.Result.TextureType == ImageComponent.TypeEnum._2D )
										{
											//when mask is 2D texture

											for( int face = 0; face < 6; face++ )
											{
												var slice = index * 6 + face;
												var viewport = arrayTexture.Result.GetRenderTarget( slice: slice ).Viewports[ 0 ];
												context.SetViewport( viewport );
												CopyToCurrentViewport( context, mask );
											}
										}
										else if( mask.Result.TextureType == ImageComponent.TypeEnum.Cube )
										{
											//when mask is cube texture

											for( int face = 0; face < 6; face++ )
											{
												var slice = index * 6 + face;
												var viewport = arrayTexture.Result.GetRenderTarget( slice: slice ).Viewports[ 0 ];
												context.SetViewport( viewport );

												//!!!!GC. where else
												//!!!!also can copy with one draw call, use mrt or compute
												//but it only for current gen, for modern gen use dynamic indexing

												var shader = new CanvasRenderer.ShaderItem();
												shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
												shader.FragmentProgramFileName = @"Base\Shaders\CopyCubemapFace_fs.sc";

												shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, mask, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

												shader.Parameters.Set( "u_copyCubemapFaceIndex", new Vector4F( face, 0, 0, 0 ) );

												context.RenderQuadToCurrentViewport( shader );

												//CopyToCurrentViewport( context, mask );
											}
										}

										index++;
									}
								}
							}
							else if( lightType == Light.TypeEnum.Spotlight )
							{
								//spot

								//create array if no shadows
								if( frameData.ShadowTextureArraySpot == null )
								{
									//!!!!workaround for Android. 1 depth arrays are not arrays
									frameData.ShadowTextureArraySpot = context.RenderTarget2D_Alloc( new Vector2I( maxSize, maxSize ), PixelFormat.A8R8G8B8, arrayLayers: Math.Max( dictionary.Count, 2 ) );
								}

								var arrayTexture = frameData.ShadowTextureArraySpot;
								if( arrayTexture != null )
								{
									var index = frameData.ShadowTextureArraySpotUsedForShadows;
									foreach( var mask in dictionary.Keys )
									{
										var viewport = arrayTexture.Result.GetRenderTarget( slice: index ).Viewports[ 0 ];
										context.SetViewport( viewport );
										CopyToCurrentViewport( context, mask );
										index++;
									}
								}
							}
							else
							{
								//directional

								//create array if no shadows
								if( frameData.ShadowTextureArrayDirectional == null )
								{
									//!!!!workaround for Android. 1 depth arrays are not arrays
									frameData.ShadowTextureArrayDirectional = context.RenderTarget2D_Alloc( new Vector2I( maxSize, maxSize ), PixelFormat.A8R8G8B8, arrayLayers: Math.Max( dictionary.Count, 2 ) );
								}

								var arrayTexture = frameData.ShadowTextureArrayDirectional;
								if( arrayTexture != null )
								{
									var index = frameData.ShadowTextureArrayDirectionalUsedForShadows;
									foreach( var mask in dictionary.Keys )
									{
										var viewport = arrayTexture.Result.GetRenderTarget( slice: index ).Viewports[ 0 ];
										context.SetViewport( viewport );
										CopyToCurrentViewport( context, mask );
										index++;
									}
								}
							}
						}
					}
				}
				else
				{
					//basic device

					for( var lightType = Light.TypeEnum.Directional; lightType <= Light.TypeEnum.Spotlight; lightType++ )
					{
						var dictionary = new EDictionary<ImageComponent, int>();
						var maxSize = 0;

						foreach( var lightIndex in frameData.LightsInFrustumSorted )
						{
							var lightItem = frameData.Lights[ lightIndex ];
							var lightData = lightItem.data;

							if( lightData.Type == lightType )
							{
								lightItem.lightMaskIndex = -1;

								var mask = lightData.Mask;
								if( mask?.Result != null )
								{
									if( !dictionary.TryGetValue( mask, out var index ) )
									{
										index = dictionary.Count;
										dictionary[ mask ] = index;
										var size = mask.Result.ResultSize.X;
										if( size > maxSize )
											maxSize = size;
									}
									lightItem.lightMaskIndex = index;
								}
							}
						}

						if( dictionary.Count > 0 )
						{
							ImageComponent arrayTexture;

							if( lightType == Light.TypeEnum.Point )
							{
								//point

								arrayTexture = context.RenderTargetCube_Alloc( new Vector2I( maxSize, maxSize ), PixelFormat.Float16RGBA, arrayLayers: dictionary.Count );

								var index = 0;

								foreach( var mask in dictionary.Keys )
								{
									if( mask.Result.TextureType == ImageComponent.TypeEnum._2D )
									{
										//when mask is 2D texture

										for( int face = 0; face < 6; face++ )
										{
											var slice = index * 6 + face;
											var viewport = arrayTexture.Result.GetRenderTarget( slice: slice ).Viewports[ 0 ];
											context.SetViewport( viewport );
											CopyToCurrentViewport( context, mask );
										}
									}
									else if( mask.Result.TextureType == ImageComponent.TypeEnum.Cube )
									{
										//when mask is cube texture

										for( int face = 0; face < 6; face++ )
										{
											var slice = index * 6 + face;
											var viewport = arrayTexture.Result.GetRenderTarget( slice: slice ).Viewports[ 0 ];
											context.SetViewport( viewport );

											//!!!!GC. where else
											//!!!!also can copy with one draw call, use mrt or compute
											//but it only for current gen, for modern gen use dynamic indexing

											var shader = new CanvasRenderer.ShaderItem();
											shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
											shader.FragmentProgramFileName = @"Base\Shaders\CopyCubemapFace_fs.sc";

											shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, mask, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

											shader.Parameters.Set( "u_copyCubemapFaceIndex", new Vector4F( face, 0, 0, 0 ) );

											context.RenderQuadToCurrentViewport( shader );

											//CopyToCurrentViewport( context, mask );
										}
									}

									index++;
								}
							}
							else
							{
								//directional, spot

								if( dictionary.Count == 1 )
								{
									arrayTexture = dictionary.Keys.First();
								}
								else
								{
									arrayTexture = context.RenderTarget2D_Alloc( new Vector2I( maxSize, maxSize ), PixelFormat.Float16RGBA, arrayLayers: dictionary.Count );

									var index = 0;

									foreach( var mask in dictionary.Keys )
									{
										var viewport = arrayTexture.Result.GetRenderTarget( slice: index ).Viewports[ 0 ];
										context.SetViewport( viewport );
										CopyToCurrentViewport( context, mask );

										index++;
									}
								}
							}

							if( lightType == Light.TypeEnum.Directional )
								frameData.MaskTextureArrayDirectional = arrayTexture;
							else if( lightType == Light.TypeEnum.Spotlight )
								frameData.MaskTextureArraySpot = arrayTexture;
							else
								frameData.MaskTextureArrayPoint = arrayTexture;
						}
					}
				}
			}

			//prepare dynamic textures
			PrepareLightsTexture( context, frameData );
			unsafe
			{
				var parameters = new Vector4F( frameData.LightsInFrustumSorted.Length, 0, 0, 0 );
				context.SetUniform( "u_multiLightParams", ParameterType.Vector4, 1, &parameters );
			}


			//compute operations
			//enable view for compute operations
			context.SetComputePass();
			//var computePassActivated = false;

			//prepare light grid
			unsafe
			{
				var lightGridInitialized = false;

				var lightGrid = RenderingSystem.LightGrid && GetUseMultiRenderTargets() && LightGrid.Value != AutoTrueFalse.False;
				if( lightGrid )
				{
					var gridSize = int.Parse( LightGridResolution.Value.ToString().Replace( "_", "" ) );

					var maxLength = (float)owner.CameraSettings.FarClipDistance;

					//#define d_lightGridEnabled lightGridParams.x
					//#define d_lightGridSize lightGridParams.y
					//#define d_lightGridStart lightGridParams.z
					//#define d_lightGridCellSize lightGridParams.w

					var cellSize = maxLength * 2 / gridSize;
					var lightGridParams = new Vector4F( 1, gridSize, -maxLength, cellSize );
					context.SetUniform( "u_lightGridParams", ParameterType.Vector4, 1, &lightGridParams );
					frameData.LightGrid = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize, gridSize, 8 ), PixelFormat.Float32RGBA, 0, false );

					if( frameData.LightGrid != null )
					{
						lightGridInitialized = true;

						////enable view for compute operations
						//if( !computePassActivated )
						//{
						//	context.SetComputePass();
						//	computePassActivated = true;
						//}

						context.BindComputeImage( 0, frameData.LightGrid, 0, ComputeBufferAccessEnum.Write );
						context.BindTexture( 1/* "s_lightsTexture"*/, frameData.LightsTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );

						if( lightGridGenerateProgram == null )
						{
							var program = GpuProgramManager.GetProgram( "LightGrid", GpuProgramType.Compute, @"Base\Shaders\LightGrid.sc", null, true, out var error2 );
							if( !string.IsNullOrEmpty( error2 ) )
								Log.Fatal( "RenderingPipeline_Basic: RenderWithRenderTargets: " + error2 );
							else
								lightGridGenerateProgram = new Program( program.RealObject );
						}

						var jobSize = new Vector3I( (int)Math.Ceiling( gridSize / 32.0 ), (int)Math.Ceiling( gridSize / 32.0 ), 1 );
						//var jobSize = new Vector3I( (int)Math.Ceiling( gridSize / 8.0 ), (int)Math.Ceiling( gridSize / 8.0 ), 1 );

						context.Dispatch( lightGridGenerateProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
						//Bgfx.Dispatch( (ushort)context.CurrentViewNumber, lightGridGenerateProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
						//context.UpdateStatisticsCurrent.ComputeDispatches++;
					}
				}

				if( !lightGridInitialized )
				{
					var lightGridParams = new Vector4F( 0, 0, 0, 0 );
					context.SetUniform( "u_lightGridParams", ParameterType.Vector4, 1, &lightGridParams );
				}
			}

			//prepare global illumination
			if( frameData.GIData != null )
			{
				//if( !computePassActivated )
				//{
				//	context.SetComputePass();
				//	computePassActivated = true;
				//}

				GIPrepare( context );
			}

			//end of compute


			/////////////////////////////////////
			//detect destination size, SizeInPixelsLowResolutionBeforeUpscale, antialiasing and resolution upscale techniques
			RenderingEffect_Antialiasing antialiasing = null;
			int msaaLevel = 0;
			RenderingEffect_ResolutionUpscale resolutionUpscale = null;
			Vector2I destinationSize;
			//RenderingEffect_IndirectLighting indirectLighting = null; //indirectLightingFullMode
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

				//if( DebugIndirectLighting )
				//{
				//	var effect = GetSceneEffect<RenderingEffect_IndirectLighting>();
				//	if( effect != null && effect.Intensity > 0 && effect.IsSupported )
				//	{
				//		//if( effect.Technique.Value == RenderingEffect_IndirectLighting.TechniqueEnum.Full )
				//		indirectLighting = effect;
				//	}
				//}
			}


			////!!!!
			////TAA jittering test
			//unsafe
			//{
			//	var jitteringTest = new Vector4F( 0, 0, 0, 0 );

			//	//if( !EngineApp._DebugCapsLock )
			//	if( false )
			//	{
			//		var values = new Vector2[ 16 ];
			//		values[ 0 ] = new Vector2( 0.500000, 0.333333 );
			//		values[ 1 ] = new Vector2( 0.250000, 0.666667 );
			//		values[ 2 ] = new Vector2( 0.750000, 0.111111 );
			//		values[ 3 ] = new Vector2( 0.125000, 0.444444 );
			//		values[ 4 ] = new Vector2( 0.625000, 0.777778 );
			//		values[ 5 ] = new Vector2( 0.375000, 0.222222 );
			//		values[ 6 ] = new Vector2( 0.875000, 0.555556 );
			//		values[ 7 ] = new Vector2( 0.062500, 0.888889 );
			//		values[ 8 ] = new Vector2( 0.562500, 0.037037 );
			//		values[ 9 ] = new Vector2( 0.312500, 0.370370 );
			//		values[ 10 ] = new Vector2( 0.812500, 0.703704 );
			//		values[ 11 ] = new Vector2( 0.187500, 0.148148 );
			//		values[ 12 ] = new Vector2( 0.687500, 0.481481 );
			//		values[ 13 ] = new Vector2( 0.437500, 0.814815 );
			//		values[ 14 ] = new Vector2( 0.937500, 0.259259 );
			//		values[ 15 ] = new Vector2( 0.031250, 0.592593 );

			//		//if( !EngineApp._DebugCapsLock )
			//		{
			//			unchecked
			//			{
			//				jitteringCounter++;
			//			}
			//			var value = values[ jitteringCounter % 16 ];

			//			var size = destinationSize;// context.Owner.SizeInPixels.ToVector2F();

			//			jitteringTest.X = ( ( (float)value.X - 0.5f ) / size.X ) * 2;
			//			jitteringTest.Y = ( ( (float)value.Y - 0.5f ) / size.Y ) * 2;

			//			////!!!!
			//			//if( EngineApp._DebugCapsLock )
			//			//{
			//			//	jitteringTest.X = ( ( (float)value.X - 0.5f ) / size.X ) * 1;
			//			//	jitteringTest.Y = ( ( (float)value.Y - 0.5f ) / size.Y ) * 1;
			//			//}
			//		}
			//	}

			//	context.SetUniform( "jitteringTest", ParameterType.Vector4, 1, &jitteringTest );
			//}

			/////////////////////////////////////
			//create scene texture, draw background effects

			var sceneTexture = context.RenderTarget2D_Alloc( destinationSize, ( GetHighDynamicRange() && !GetDeferredShading() ) ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8, msaaLevel );
			context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, context.GetBackgroundColor() );
			//Background effects
			RenderEffectsInternal( context, frameData, "Background Effects", ref sceneTexture, false );

			/////////////////////////////////////
			//create normal texture
			ImageComponent normalTexture = null;
			if( GetUseMultiRenderTargets() )
			{
				normalTexture = context.RenderTarget2D_Alloc( destinationSize, GetHighDynamicRange() ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8, msaaLevel );
				context.ObjectsDuringUpdate.namedTextures[ "normalTexture" ] = normalTexture;
			}

			/////////////////////////////////////
			//create motion vectors texture
			var motionBlur = GetSceneEffect<RenderingEffect_MotionBlur>();
			ImageComponent motionAndObjectIdTexture = null;
			if( GetUseMultiRenderTargets() )
			{
				if( motionBlur != null && motionBlur.IsSupported || antialiasing != null && antialiasing.GetMotionTechnique() == RenderingEffect_Antialiasing.MotionTechniqueEnum.TAA )//|| indirectLightingFullMode != null )
				{
					//motionAndObjectIdTexture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.Float32RGBA );
					//motionAndObjectIdTexture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.Float16RGBA );
					motionAndObjectIdTexture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.Float16GR );
					context.ObjectsDuringUpdate.namedTextures[ "motionAndObjectIdTexture" ] = motionAndObjectIdTexture;
				}
			}

			/////////////////////////////////////
			//create depth texture
			var depthTexture = context.RenderTarget2D_Alloc( destinationSize, RenderingSystem.DepthBuffer32Float ? PixelFormat.Depth32F : PixelFormat.Depth24S8, msaaLevel );
			context.ObjectsDuringUpdate.namedTextures[ "depthTexture" ] = depthTexture;

			/////////////////////////////////////
			//clear normal, motion, depth textures
			if( GetUseMultiRenderTargets() )
			{
				MultiRenderTarget normalMotionDepthMRT;
				{
					var items = new List<MultiRenderTarget.Item>();
					items.Add( new MultiRenderTarget.Item( normalTexture ) );
					if( motionAndObjectIdTexture != null )
						items.Add( new MultiRenderTarget.Item( motionAndObjectIdTexture ) );
					items.Add( new MultiRenderTarget.Item( depthTexture ) );
					normalMotionDepthMRT = context.MultiRenderTarget_Create( items.ToArray() );
				}

				context.SetViewport( normalMotionDepthMRT.Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.All, ColorValue.Zero, RenderingSystem.ReversedZ ? 0 : 1 );
			}
			else
			{
				var sceneDepthMRT = context.MultiRenderTarget_Create( new[] {
					new MultiRenderTarget.Item( sceneTexture ),
					new MultiRenderTarget.Item( depthTexture ) } );
				context.SetViewport( sceneDepthMRT.Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Depth, ColorValue.Zero, RenderingSystem.ReversedZ ? 0 : 1 );
			}

			var viewMatrix = owner.CameraSettings.ViewMatrixRelative;
			var projectionMatrix = owner.CameraSettings.ProjectionMatrix;

			//deferred shading
			if( GetDeferredShading() && DebugDrawDeferredPass )
			{
				var drawCallsBeforeDeferred = context.updateStatisticsCurrent.DrawCalls;

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
				var gBuffer4Texture = context.RenderTarget2D_Alloc( destinationSize, GetHighDynamicRange() ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8 );
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

				if( motionAndObjectIdTexture != null )
					context.ObjectsDuringUpdate.namedTextures[ "gBuffer6Texture" ] = motionAndObjectIdTexture;

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
					if( motionAndObjectIdTexture != null )
						items.Add( new MultiRenderTarget.Item( motionAndObjectIdTexture ) );
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
					CopyToCurrentViewport( context, normalTexture/*gBuffer1Texture*/ );

					//make copy of gBuffer4Texture
					var gBuffer4TextureCopy = context.RenderTarget2D_Alloc( destinationSize, gBuffer4Texture.Result.ResultFormat );
					context.SetViewport( gBuffer4TextureCopy.Result.GetRenderTarget().Viewports[ 0 ] );
					CopyToCurrentViewport( context, gBuffer4Texture );

					//make copy of gBuffer5Texture
					var gBuffer5TextureCopy = context.RenderTarget2D_Alloc( destinationSize, gBuffer5Texture.Result.ResultFormat );
					context.SetViewport( gBuffer5TextureCopy.Result.GetRenderTarget().Viewports[ 0 ] );
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

				//prepare SSR before lighting pass
				RenderDeferredShadingGBufferReady?.Invoke( this, context, frameData, ref sceneTexture );

				////!!!!temp
				////save depth buffer to texture
				//PhysicallyCorrectRendering_CallSaveDepthBufferToTexture( context );

				var deferredLightTexture = context.RenderTarget2D_Alloc( destinationSize, GetHighDynamicRange() ? PixelFormat.Float16RGBA : PixelFormat.A8R8G8B8 );

				//lighting pass for deferred shading
				context.SetViewport( deferredLightTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.Color, ColorValue.Zero );

				if( DebugDirectLighting )
					RenderLightsDeferred( context, frameData, sceneTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, depthTexture );

				////indirect lighting full mode
				//if( frameData.IndirectLightingFrameData != null )
				//{
				//	//!!!!только для deferred? как на forward влияет?

				//	RenderIndirectLightingFullTechnique( context, frameData, ref deferredLightTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, motionAndObjectIdTexture, depthTexture );
				//}

				//copy result of deferred shading to sceneTexture. save gBuffer0Texture
				sceneTexture = deferredLightTexture;
				//context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ] );
				//CopyToCurrentViewport( context, deferredLightTexture );

				//RenderDeferredShadingEnd?.Invoke( this, context, frameData, ref sceneTexture );

				//context.RenderTarget_Free( deferredLightTexture );

				context.updateStatisticsCurrent.DrawCallsDeferred = context.updateStatisticsCurrent.DrawCalls - drawCallsBeforeDeferred;
			}

			var drawCallsBeforeForward = context.updateStatisticsCurrent.DrawCalls;

			//render opaque objects (forward)
			if( DebugDrawForwardOpaquePass && DebugDirectLighting )
			{
				MultiRenderTarget sceneNormalMotionDepthMRT;
				{
					var items = new List<MultiRenderTarget.Item>();
					items.Add( new MultiRenderTarget.Item( sceneTexture ) );
					if( GetUseMultiRenderTargets() )
					{
						items.Add( new MultiRenderTarget.Item( normalTexture ) );
						if( motionAndObjectIdTexture != null )
							items.Add( new MultiRenderTarget.Item( motionAndObjectIdTexture ) );
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
				if( DebugDirectLighting )
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
			if( DebugDrawForwardTransparentPass && DebugDrawLayers && DebugDirectLighting )
			{
				//create scene, motion, depth MRT
				MultiRenderTarget sceneNormalMotionDepthMRT;
				{
					var items = new List<MultiRenderTarget.Item>();
					items.Add( new MultiRenderTarget.Item( sceneTexture ) );
					if( GetUseMultiRenderTargets() )
					{
						items.Add( new MultiRenderTarget.Item( normalTexture ) );
						if( motionAndObjectIdTexture != null )
							items.Add( new MultiRenderTarget.Item( motionAndObjectIdTexture ) );
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
			if( DebugDrawForwardTransparentPass && DebugDirectLighting )
			{
				//make copy of scene color and depth textures to use as a sampler
				ImageComponent colorDepthTextureCopy = null;
				if( IsProvideColorDepthTextureCopy() )
				{
					colorDepthTextureCopy = context.RenderTarget2D_Alloc( sceneTexture.Result.ResultSize, PixelFormat.Float32RGBA );

					//not enough color precision
					////r: color. packed to one float
					////g: depth
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

				//use dummy texture to disable writing to normalTexture
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
						if( motionAndObjectIdTexture != null )
							items.Add( new MultiRenderTarget.Item( motionAndObjectIdTexture ) );
					}
					items.Add( new MultiRenderTarget.Item( depthTexture ) );
					sceneMotionDepthMRT = context.MultiRenderTarget_Create( items.ToArray() );
				}

				//if( GetOrderIndependentTransparency() )
				//{
				//	Render3DSceneForwardTransparentWithOIT( context, frameData, colorDepthTextureCopy, sceneMotionDepthMRT, ref viewMatrix, ref projectionMatrix, sceneTexture );
				//}
				//else
				{
					context.SetViewport( sceneMotionDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix );
					Render3DSceneForwardTransparentWithoutOIT( context, frameData, colorDepthTextureCopy );
				}

				if( dummyNormalTexture != null )
					context.DynamicTexture_Free( dummyNormalTexture );
				if( colorDepthTextureCopy != null )
					context.DynamicTexture_Free( colorDepthTextureCopy );

				////OIT second pass
				//if( GetOrderIndependentTransparency() )
				//{



				//context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				//var shader = new CanvasRenderer.ShaderItem();
				//shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				//shader.FragmentProgramFileName = @"Base\Shaders\ComposeOIT_fs.sc";

				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"colorTexture"*/, colorTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"secondTexture"*/, secondTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				//context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.ComposeOIT );






				////!!!!ResultFormat? rgba16f
				//var colorTexture = context.RenderTarget2D_Alloc( sceneTexture.Result.ResultSize, sceneTexture.Result.ResultFormat );
				//var secondTexture = context.RenderTarget2D_Alloc( sceneTexture.Result.ResultSize, PixelFormat.Float16R );

				////clear
				//context.SetViewport( colorTexture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, ColorValue.Zero );
				//context.SetViewport( secondTexture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, ColorValue.One );

				////create scene, motion, depth MRT
				//MultiRenderTarget sceneMotionDepthMRT;
				//{
				//	var items = new List<MultiRenderTarget.Item>();
				//	items.Add( new MultiRenderTarget.Item( colorTexture ) );
				//	items.Add( new MultiRenderTarget.Item( secondTexture ) );
				//	if( motionAndObjectIdTexture != null )
				//		items.Add( new MultiRenderTarget.Item( motionAndObjectIdTexture ) );
				//	items.Add( new MultiRenderTarget.Item( depthTexture ) );
				//	sceneMotionDepthMRT = context.MultiRenderTarget_Create( items.ToArray() );
				//}



				////enable oit
				//GpuMaterialPass.GlobalComposeOIT = true;
				//unsafe
				//{
				//	var forwardOIT = new Vector4F( 1, 0, 0, 0 );
				//	context.SetUniform( "u_forwardOIT", ParameterType.Vector4, 1, &forwardOIT );
				//}

				//context.SetViewport( sceneMotionDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix );
				//Render3DSceneForwardTransparentWithOIT( context, frameData, colorDepthTextureCopy );

				////disable oit
				//GpuMaterialPass.GlobalComposeOIT = false;
				//unsafe
				//{
				//	var forwardOIT = new Vector4F( 0, 0, 0, 0 );
				//	context.SetUniform( "u_forwardOIT", ParameterType.Vector4, 1, &forwardOIT );
				//}

				//context.DynamicTexture_Free( colorTexture );
				//context.DynamicTexture_Free( secondTexture );




				////!!!!ResultFormat? rgba16f
				//var colorTexture = context.RenderTarget2D_Alloc( sceneTexture.Result.ResultSize, sceneTexture.Result.ResultFormat );
				//var secondTexture = context.RenderTarget2D_Alloc( sceneTexture.Result.ResultSize, PixelFormat.Float16R );

				////clear
				//context.SetViewport( colorTexture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, ColorValue.Zero );
				//context.SetViewport( secondTexture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity, FrameBufferTypes.Color, ColorValue.One );

				////enable oit
				//GpuMaterialPass.GlobalComposeOIT = true;
				//unsafe
				//{
				//	var forwardOIT = new Vector4F( 1, 0, 0, 0 );
				//	context.SetUniform( "u_forwardOIT", ParameterType.Vector4, 1, &forwardOIT );
				//}

				////create scene, motion, depth MRT
				//MultiRenderTarget sceneMotionDepthMRT;
				//{
				//	var items = new List<MultiRenderTarget.Item>();
				//	items.Add( new MultiRenderTarget.Item( colorTexture ) );
				//	items.Add( new MultiRenderTarget.Item( secondTexture ) );
				//	if( motionAndObjectIdTexture != null )
				//		items.Add( new MultiRenderTarget.Item( motionAndObjectIdTexture ) );
				//	items.Add( new MultiRenderTarget.Item( depthTexture ) );
				//	sceneMotionDepthMRT = context.MultiRenderTarget_Create( items.ToArray() );
				//}

				//context.SetViewport( sceneMotionDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix );
				////!!!!
				//Render3DSceneForwardTransparentWithoutOIT( context, frameData, colorDepthTextureCopy );
				////Render3DSceneForwardTransparentWithOIT( context, frameData, colorDepthTextureCopy );

				////disable oit
				//GpuMaterialPass.GlobalComposeOIT = false;
				//unsafe
				//{
				//	var forwardOIT = new Vector4F( 0, 0, 0, 0 );
				//	context.SetUniform( "u_forwardOIT", ParameterType.Vector4, 1, &forwardOIT );
				//}

				////second pass
				//{
				//	context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				//	var shader = new CanvasRenderer.ShaderItem();
				//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				//	shader.FragmentProgramFileName = @"Base\Shaders\ComposeOIT_fs.sc";

				//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"colorTexture"*/, colorTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
				//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1/*"secondTexture"*/, secondTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				//	context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.ComposeOIT );
				//}

				//context.DynamicTexture_Free( colorTexture );
				//context.DynamicTexture_Free( secondTexture );

				//}

				//else
				//{
				////create scene, motion, depth MRT
				//MultiRenderTarget sceneMotionDepthMRT;
				//{
				//	var items = new List<MultiRenderTarget.Item>();
				//	items.Add( new MultiRenderTarget.Item( sceneTexture ) );
				//	if( GetUseMultiRenderTargets() )
				//	{
				//		items.Add( new MultiRenderTarget.Item( dummyNormalTexture ) );// normalTexture ) );
				//		if( motionAndObjectIdTexture != null )
				//			items.Add( new MultiRenderTarget.Item( motionAndObjectIdTexture ) );
				//	}
				//	items.Add( new MultiRenderTarget.Item( depthTexture ) );
				//	sceneMotionDepthMRT = context.MultiRenderTarget_Create( items.ToArray() );
				//}

				//////create scene, depth MRT
				////var sceneDepthMRT = context.MultiRenderTarget_Create( new[] {
				////	new MultiRenderTarget.Item( sceneTexture ),
				////	new MultiRenderTarget.Item( depthTexture ) } );

				//context.SetViewport( sceneMotionDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix );
				//Render3DSceneForwardTransparentWithoutOIT( context, frameData, colorDepthTextureCopy );

				//if( dummyNormalTexture != null )
				//	context.DynamicTexture_Free( dummyNormalTexture );
				//}

			}

			context.updateStatisticsCurrent.DrawCallsForward = context.updateStatisticsCurrent.DrawCalls - drawCallsBeforeForward;

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
				//antialiasing. it makes texts blurry
				if( owner.CanvasRenderer.ScreenAntialisingForThisFrame )
				{
					var simpleDataTexture = context.RenderTarget2D_Alloc( owner.SizeInPixels * 2, PixelFormat.A8R8G8B8 );
					context.SetViewport( simpleDataTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.Color, new ColorValue( 0, 0, 0, 0 ) );

					CopyToCurrentViewport( context, sceneTexture );

					//render UI
					owner.CanvasRenderer.ViewportRendering_RenderToCurrentViewport( context, false, owner.LastUpdateTime );

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
				}
				else
				{
					context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ] );
					owner.CanvasRenderer.ViewportRendering_RenderToCurrentViewport( context, false, owner.LastUpdateTime );
				}
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
			context.SetViewport( owner.OutputViewport ?? owner );
			CopyToCurrentViewport( context, sceneTexture, flipY: owner.OutputFlipY );

			///////////////////////////////////////
			////Render video to file
			//RenderVideoToFile( context, sceneTexture );

			//clear or destroy something maybe

			Bgfx.Discard( DiscardFlags.All );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected virtual void RenderWithoutRenderTargets( ViewportRenderingContext context, FrameData frameData )
		{
			var owner = context.Owner;
			var viewMatrix = owner.CameraSettings.ViewMatrixRelative;
			var projectionMatrix = owner.CameraSettings.ProjectionMatrix;

			//reset disabled light settings
			{
				foreach( var lightIndex in frameData.LightsInFrustumSorted )
				{
					var lightItem = frameData.Lights[ lightIndex ];
					lightItem.lightMaskIndex = -1;
					lightItem.shadowMapIndex = -1;
				}
			}

			//prepare dynamic textures
			PrepareLightsTexture( context, frameData );
			unsafe
			{
				var parameters = new Vector4F( frameData.LightsInFrustumSorted.Length, 0, 0, 0 );
				context.SetUniform( "u_multiLightParams", ParameterType.Vector4, 1, &parameters );
			}

			//light grid
			unsafe
			{
				var lightGridParams = new Vector4F( 0, 0, 0, 0 );
				context.SetUniform( "u_lightGridParams", ParameterType.Vector4, 1, &lightGridParams );
			}

			//prepare tessellation
			TessellationPrepare( context );

			if( context.Owner.Parent is RenderTexture renderTexture )
			{
				//render to RenderTexture
				//for rendering to texture anyway must use additional targets, or depth will be broken

				var backgroundColor = context.GetBackgroundColor();
				if( DebugMode.Value != DebugModeEnum.None )
					backgroundColor = new ColorValue( 0, 0, 0 );

				//create scene texture
				var overrideSize = owner.SizeInPixels;
				//if( RenderWithoutRenderTargetsApplyScale != 1.0 )
				//	overrideSize = ( overrideSize.ToVector2() * RenderWithoutRenderTargetsApplyScale ).ToVector2I();
				////var overrideSize = RenderWithoutRenderTargetsOverrideSize ?? context.owner.SizeInPixels;
				if( overrideSize.X < 1 )
					overrideSize.X = 1;
				if( overrideSize.Y < 1 )
					overrideSize.Y = 1;
				var sceneTexture = context.RenderTarget2D_Alloc( overrideSize/*owner.SizeInPixels*/, renderTexture.Creator.ResultFormat, 0 );

				//create depth texture
				var depthTexture = context.RenderTarget2D_Alloc( overrideSize/*owner.SizeInPixels*/, RenderingSystem.DepthBuffer32Float ? PixelFormat.Depth32F : PixelFormat.Depth24S8, 0 );
				context.ObjectsDuringUpdate.namedTextures[ "depthTexture" ] = depthTexture;

				var sceneDepthMRT = context.MultiRenderTarget_Create( new[] {
					new MultiRenderTarget.Item( sceneTexture ),
					new MultiRenderTarget.Item( depthTexture ) } );
				context.SetViewport( sceneDepthMRT.Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.All, backgroundColor, RenderingSystem.ReversedZ ? 0 : 1 );

				/////////////////////////////////////
				//Scene
				if( DebugDirectLighting )
				{
					if( DebugDrawForwardOpaquePass )
						Render3DSceneForwardOpaque( context, frameData );
					RenderSky( context, frameData );
					if( DebugDrawForwardTransparentPass )
						Render3DSceneForwardTransparentWithoutOIT( context, frameData, null );
				}

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
				context.SetViewport( owner.OutputViewport ?? owner );
				CopyToCurrentViewport( context, sceneTexture, flipY: owner.OutputFlipY );
			}
			else
			{
				//render to RenderWindow

				var backgroundColor = context.GetBackgroundColor();
				if( DebugMode.Value != DebugModeEnum.None )
					backgroundColor = new ColorValue( 0, 0, 0 );

				context.SetViewport( owner, viewMatrix, projectionMatrix, FrameBufferTypes.All, backgroundColor, RenderingSystem.ReversedZ ? 0 : 1 );

				/////////////////////////////////////
				//Scene
				if( DebugDirectLighting )
				{
					if( DebugDrawForwardOpaquePass )
						Render3DSceneForwardOpaque( context, frameData );
					RenderSky( context, frameData );
					if( DebugDrawForwardTransparentPass )
						Render3DSceneForwardTransparentWithoutOIT( context, frameData, null );
				}

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

			//remove unused static shadows items when it switched from RenderWithRenderTargets
			if( context.LightStaticShadowsItems != null )
			{
				foreach( var item in context.LightStaticShadowsItems.Values )
					item.Dispose();
				context.LightStaticShadowsItems.Clear();
			}

			////remove unused tessellation items when it switched from RenderWithRenderTargets
			//if( context.TessellationCacheItems != null )
			//{
			//	foreach( var item in context.TessellationCacheItems.Values )
			//		item.Dispose();
			//	context.TessellationCacheItems.Clear();
			//}

			Bgfx.Discard( DiscardFlags.All );
		}

		public delegate void RenderBeginDelegate( RenderingPipeline_Basic sender, ViewportRenderingContext context, FrameData frameData );
		public event RenderBeginDelegate RenderBegin;

		public delegate void RenderEndDelegate( RenderingPipeline_Basic sender, ViewportRenderingContext context, FrameData frameData );
		public event RenderEndDelegate RenderEnd;

		[MethodImpl( (MethodImplOptions)512 )]
		public override void Render( ViewportRenderingContext context )
		{
			u_renderOperationData = GpuProgramManager.RegisterUniform( "u_renderOperationData", UniformType.Vector4, 8 );
			u_objectInstanceParameters = GpuProgramManager.RegisterUniform( "u_objectInstanceParameters", UniformType.Vector4, 2 );

			if( context.FrameData == null )
				context.FrameData = new FrameData();
			var frameData = context.FrameData;

			context.SizeInPixelsLowResolutionBeforeUpscale = context.Owner.SizeInPixels;
			context.OwnerCameraSettings = context.Owner.CameraSettings;
			context.OwnerCameraSettingsPosition = context.OwnerCameraSettings.Position;
			context.OwnerCameraSettingsPositionPrevious = context.Owner.PreviousFramePosition ?? context.OwnerCameraSettingsPosition;
			context.OwnerCameraSettingsPositionPreviousChange = context.Owner.PreviousFramePositionChange;
			context.ShadowObjectVisibilityDistanceFactor = (float)ShadowObjectVisibilityDistanceFactor.Value;
			context.UpdateGetVisibilityDistanceByObjectSize();
			context.UpdateVoxelLodVisibilityDistanceByObjectSize();
			context.DeferredShading = GetDeferredShading();
			if( context.Owner.AttachedScene != null )
				context.SceneDisplayDevelopmentDataInThisApplication = context.Owner.AttachedScene.GetDisplayDevelopmentDataInThisApplication();
			context.LODRange = LODRange.Value;
			//additional 1.5 multiplier to add more quality
			context.LODScale = (float)( LODScale.Value * GlobalLODScale * 1.5f );
			context.LODScaleShadowsSquared = (float)( LODScaleShadows.Value * GlobalLODScaleShadows );
			context.LODScaleShadowsSquared *= context.LODScaleShadowsSquared;
			context.SmoothLOD = RenderingSystem.SmoothLOD;
			context.LightFarDistance = (float)LightMaxDistance;
			context.DebugDrawMeshes = DebugDrawMeshes;
			context.DebugDrawVoxels = DebugDrawVoxels;
			context.DebugDrawBatchedData = DebugDrawBatchedData;
			context.DebugDrawNotBatchedData = DebugDrawNotBatchedData;
			context.StaticShadows = RenderingSystem.StaticShadows && ShadowStatic;
			context.SectorsByDistance = MathEx.Clamp( SectorsByDistance.Value, 1, 16 );
			//context.TessellationDistance = RenderingSystem.Tessellation ? (float)TessellationDistance : -1;

			duringRenderShadows = false;
			duringRenderSimple3DRenderer = false;

			RenderBegin?.Invoke( this, context, frameData );

			//set viewport to give the ability to use compute shaders
			{
				var owner = context.Owner;
				var viewMatrix = owner.CameraSettings.ViewMatrixRelative;
				var projectionMatrix = owner.CameraSettings.ProjectionMatrix;

				context.SetViewport( owner, viewMatrix, projectionMatrix, 0, ColorValue.Zero );
			}

			GIInit( context );

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

			//!!!!parallel?
			//prepare dynamic textures
			PrepareMaterialsTexture( context, frameData );
			PrepareBonesTexture( context, frameData );
			PrepareReflectionProbesCubemapEnvironmentMipmapsAndBlur( context, frameData );

			//set u_viewportOwnerMatrices
			unsafe
			{
				var cameraSettings = context.Owner.CameraSettings;
				var viewProjection = cameraSettings.ProjectionMatrix * cameraSettings.ViewMatrixRelative;

				Matrix4F* matrices = stackalloc Matrix4F[ 7 ];

				matrices[ 0 ] = viewProjection;
				matrices[ 1 ] = cameraSettings.ViewMatrixRelative;
				matrices[ 2 ] = cameraSettings.ProjectionMatrix;
				matrices[ 3 ] = context.Owner.PreviousFrameProjectionMatrix * context.Owner.PreviousFrameViewMatrixRelative;
				cameraSettings.ViewMatrixRelative.GetInverse( out matrices[ 4 ] );
				cameraSettings.ProjectionMatrix.GetInverse( out matrices[ 5 ] );
				viewProjection.GetInverse( out matrices[ 6 ] );

				context.SetUniform( "u_viewportOwnerViewProjection", ParameterType.Matrix4x4, 1, &matrices[ 0 ] );
				context.SetUniform( "u_viewportOwnerView", ParameterType.Matrix4x4, 1, &matrices[ 1 ] );
				context.SetUniform( "u_viewportOwnerProjection", ParameterType.Matrix4x4, 1, &matrices[ 2 ] );
				context.SetUniform( "u_viewportOwnerViewProjectionPrevious", ParameterType.Matrix4x4, 1, &matrices[ 3 ] );
				context.SetUniform( "u_viewportOwnerViewInverse", ParameterType.Matrix4x4, 1, &matrices[ 4 ] );
				context.SetUniform( "u_viewportOwnerProjectionInverse", ParameterType.Matrix4x4, 1, &matrices[ 5 ] );
				context.SetUniform( "u_viewportOwnerViewProjectionInverse", ParameterType.Matrix4x4, 1, &matrices[ 6 ] );

				//context.SetUniform( "u_viewportOwnerMatrices", ParameterType.Matrix4x4, 7, matrices );
			}

			SetFogUniform( context, frameData.Fog );
			SetCutVolumeSettingsUniforms( context, null, true );

			Bgfx.SetDebugFeatures( DebugMode.Value == DebugModeEnum.Wireframe ? DebugFeatures.Wireframe : DebugFeatures.None );

			unsafe
			{
				var forwardOIT = new Vector4F( 0, 0, 0, 0 );
				context.SetUniform( "u_forwardOIT", ParameterType.Vector4, 1, &forwardOIT );
			}

			//!!!!new
			context.Owner.AttachedScene?.PerformRenderAfterSetCommonUniforms( this, context, frameData );

			//init outputInstancingManagers
			{
				//!!!!они статичные. что тут

				var sectorsByDistance = context.SectorsByDistance;// SectorsByDistance.Value;
				while( sectorsByDistance < outputInstancingManagers.Count )
				{
					var manager = outputInstancingManagers[ outputInstancingManagers.Count - 1 ];
					outputInstancingManagers.RemoveAt( outputInstancingManagers.Count - 1 );
					manager.Dispose();
				}
				while( sectorsByDistance > outputInstancingManagers.Count )
					outputInstancingManagers.Add( new OutputInstancingManager( outputInstancingManagers.Count ) );
				foreach( var manager in outputInstancingManagers )
					manager.Init( this, false );
			}

			if( UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
				RenderWithRenderTargets( context, frameData );
			else
				RenderWithoutRenderTargets( context, frameData );

			RenderEnd?.Invoke( this, context, frameData );

			ClearTempData( context, frameData );
			context.FrameData.Clear();//context.FrameData = null;

			//clear or destroy something maybe
			if( context.Owner.CanvasRenderer != null )
				context.Owner.CanvasRenderer.ScreenAntialisingForThisFrame = false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
		struct ViewportOwnerSettingsUniform
		{
			public Vector3F cameraPositionSinglePrecision;//public Vector3F cameraPosition;
			public float nearClipDistance;

			public float farClipDistance;
			public float fieldOfView;
			public float debugMode;
			public float emissiveMaterialsFactor;//public float cameraEv100;//public float unused1;

			public Vector3F shadowDirectionalDistance;
			public float cameraExposure;

			public float displacementScale;
			public float displacementMaxSteps;
			public float removeTextureTiling;
			public float provideColorDepthTextureCopy;

			public Vector3F cameraDirection;
			public float engineTime;

			public Vector3F cameraUp;
			//mip bias is disabled
			public float unused; //public float mipBias;

			public Vector2F windSpeed;
			public float shadowObjectVisibilityDistanceFactor;
			public float debugCapsLock;

			public Vector3F cameraPositionPreviousFrameChange;
			public float temperature;

			public Vector3F cameraPositionDivide987654Remainder;
			public float precipitationFalling;

			public Vector3F shadowPointSpotlightDistance;
			//!!!!
			public HalfType precipitationFallen;
			public HalfType timeOfDay;
			//public float precipitationFallen;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public unsafe void SetViewportOwnerSettingsUniform( ViewportRenderingContext context, DebugModeEnum? overrideDebugMode = null )
		{
			var cameraSettings = context.Owner.CameraSettings;

			var data = new ViewportOwnerSettingsUniform();

			data.cameraPositionSinglePrecision = cameraSettings.Position.ToVector3F();

			data.nearClipDistance = (float)cameraSettings.NearClipDistance;
			data.farClipDistance = (float)cameraSettings.FarClipDistance;
			data.fieldOfView = (float)cameraSettings.FieldOfView.InRadians();
			data.debugMode = (float)( overrideDebugMode.HasValue ? overrideDebugMode.Value : DebugMode.Value );

			//shadowDirectionalDistance
			{
				float farDistance = (float)ShadowDirectionalDistance;
				//!!!!?
				float shadowTextureFadeStart = .9f;
				float fadeMinDistance = farDistance * shadowTextureFadeStart;
				data.shadowDirectionalDistance = new Vector3F(
					farDistance,
					farDistance - fadeMinDistance * 2.0f,
					1.0f / ( Math.Max( farDistance - fadeMinDistance, .0001f ) ) );
			}

			//shadowPointSpotlightDistance
			{
				float farDistance = (float)ShadowPointSpotlightDistance;
				//!!!!?
				float shadowTextureFadeStart = .9f;
				float fadeMinDistance = farDistance * shadowTextureFadeStart;
				data.shadowPointSpotlightDistance = new Vector3F(
					farDistance,
					farDistance - fadeMinDistance * 2.0f,
					1.0f / ( Math.Max( farDistance - fadeMinDistance, .0001f ) ) );
			}

			data.emissiveMaterialsFactor = (float)cameraSettings.EmissiveFactor;
			data.cameraExposure = (float)cameraSettings.Exposure;
			data.displacementScale = (float)DisplacementMappingScale;
			if( GlobalTextureQuality <= 0 )
				data.displacementScale = 0;
			data.displacementMaxSteps = (float)( DisplacementMappingMaxSteps * GlobalTextureQuality );
			data.removeTextureTiling = (float)( RemoveTextureTiling * MathEx.Saturate( GlobalTextureQuality ) );
			data.provideColorDepthTextureCopy = IsProvideColorDepthTextureCopy() ? 1.0f : 0.0f;

			data.cameraDirection = cameraSettings.Rotation.GetForward().ToVector3F();
			data.engineTime = (float)EngineApp.EngineTime;

			data.cameraUp = cameraSettings.Rotation.GetUp().ToVector3F();

			//mip bias is disabled
			//var resolutionUpscale = GetSceneEffect<RenderingEffect_ResolutionUpscale>();
			//if( resolutionUpscale != null )
			//	data.mipBias = (float)resolutionUpscale.GetMipBias();

			var scene = context.Owner.AttachedScene;
			if( scene != null )
				data.windSpeed = scene.GetWindSpeedVector().ToVector2F();

			data.shadowObjectVisibilityDistanceFactor = (float)ShadowObjectVisibilityDistanceFactor;
			data.debugCapsLock = EngineApp._DebugCapsLock ? 1 : 0;

			data.cameraPositionPreviousFrameChange = ( context.OwnerCameraSettingsPosition - context.OwnerCameraSettingsPositionPrevious ).ToVector3F();
			data.temperature = scene != null ? (float)scene.Temperature : 20;

			data.cameraPositionDivide987654Remainder.X = (float)( cameraSettings.Position.X % 987654.0 );
			data.cameraPositionDivide987654Remainder.Y = (float)( cameraSettings.Position.Y % 987654.0 );
			data.cameraPositionDivide987654Remainder.Z = (float)( cameraSettings.Position.Z % 987654.0 );
			if( scene != null )
				data.precipitationFalling = (float)scene.PrecipitationFalling;

			if( scene != null )
			{
				data.precipitationFallen = new HalfType( scene.PrecipitationFallen.Value );
				data.timeOfDay = new HalfType( scene.TimeOfDay.Value );
				//data.precipitationFallen = (float)scene.PrecipitationFallen;
			}

			int vec4Count = sizeof( ViewportOwnerSettingsUniform ) / sizeof( Vector4F );
			if( vec4Count != 10 || sizeof( ViewportOwnerSettingsUniform ) != sizeof( Vector4F ) * 10 )
				Log.Fatal( "RenderingPipeline: Render: vec4Count != 10 || sizeof( ViewportOwnerSettingsUniform ) != sizeof( Vector4F ) * 10." );
			context.SetUniform( "u_viewportOwnerSettings", ParameterType.Vector4, vec4Count, &data );
		}

		static OpenList<RenderSceneData.CutVolumeItem> tempCutVolumes;
		//static Matrix4F[] tempCutVolumesMatrixArray;

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		internal protected override unsafe void SetCutVolumeSettingsUniforms( ViewportRenderingContext context, RenderSceneData.CutVolumeItem[] cutVolumes, bool forceUpdate )
		{

			//!!!!maybe send indexes of cut volumes. make cutvolumes texture like materials texture or combine material and cutvolumes texture


			if( RenderingSystem.CutVolumeMaxAmount <= 0 )
				return;

			//!!!!slowly?
			//compare cut volumes of object
			var needUpdate = !IsEqualCutVolumes( context.CurrentCutVolumes, cutVolumes ) || forceUpdate;
			if( !needUpdate )
				return;

			context.CurrentCutVolumes = cutVolumes;

			//merge scene, object cut volumes
			var list = context.FrameData.RenderSceneData.CutVolumes;
			if( cutVolumes != null && cutVolumes.Length != 0 )
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


			var data = stackalloc Matrix4F[ RenderingSystem.CutVolumeMaxAmount ];

			//if( tempCutVolumesMatrixArray == null )
			//	tempCutVolumesMatrixArray = new Matrix4F[ RenderingSystem.CutVolumeMaxAmount ];
			//var data = tempCutVolumesMatrixArray;
			////var data = stackalloc Matrix4F[ GLOBAL_CUT_VOLUME_MAX_COUNT ];
			////NativeUtility.ZeroMemory( data, sizeof( Matrix4F ) * GLOBAL_CUT_VOLUME_MAX_COUNT );

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

					//!!!!slowly? precalculate

					context.ConvertToRelative( ref item.Transform.ToMatrix4(), out var relative );
					relative.GetInverse( out data[ count ] );

					//item.Transform.ToMatrix4().GetInverse( out var inv );
					//inv.ToMatrix4F( out data[ count ] );

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
				context.SetUniform( u_viewportCutVolumeData.Value, ParameterType.Matrix4x4, RenderingSystem.CutVolumeMaxAmount, data );
				//fixed( Matrix4F* pData = data )
				//	context.SetUniform( u_viewportCutVolumeData.Value, ParameterType.Matrix4x4, RenderingSystem.CutVolumeMaxAmount, pData );
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		[MethodImpl( (MethodImplOptions)512 )]
		public virtual void GetBackgroundEnvironmentData( ViewportRenderingContext context, FrameData frameData, out EnvironmentTextureData texture, out EnvironmentIrradianceData harmonics )
		//public virtual void GetBackgroundEnvironmentData( ViewportRenderingContext context, FrameData frameData, out EnvironmentTextureData? texture, out EnvironmentIrradianceData? harmonics )
		{
			//Sky
			if( context.Owner.AttachedScene != null && frameData.Sky != null )
			{
				if( frameData.Sky.GetEnvironmentTextureData( out texture, out harmonics ) )
					return;

				//if( frameData.Sky.GetEnvironmentTextureData( out var texture2, out var textureIBL2 ) )
				//{
				//	texture = texture2;
				//	harmonics = textureIBL2;
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

		[MethodImpl( (MethodImplOptions)512 )]
		void GetEnvironmentTexturesByBoundingSphereIntersection( ViewportRenderingContext context, FrameData frameData, ref Sphere objectSphere,
			ref EnvironmentTextureData texture1, ref EnvironmentIrradianceData harmonics1,
			ref EnvironmentTextureData texture2, ref EnvironmentIrradianceData harmonics2,
			out float blendingFactor )
		//void GetEnvironmentTexturesByBoundingSphereIntersection( ViewportRenderingContext context, FrameData frameData, ref Sphere objectSphere,
		//	out EnvironmentTextureData? texture1, out EnvironmentIrradianceData? harmonics1,
		//	out EnvironmentTextureData? texture2, out EnvironmentIrradianceData? harmonics2,
		//	out float blendingFactor )
		{
			var texture1Set = false;
			var harmonics1Set = false;
			var texture2Set = false;
			var harmonics2Set = false;
			//texture1 = null;
			//harmonics1 = null;
			//texture2 = null;
			//harmonics2 = null;
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
								probe1Intensity = intensity * probe.data.Intensity;
							}
							else
							{
								probe2 = probe;
								probe2Intensity = intensity * probe.data.Intensity;
								break;
							}
						}
					}
				}

				if( probe1 != null && probe2 != null )
				{
					if( probe1.CubemapEnvironmentWithMipmapsAndBlur != null )
					{
						texture1 = new EnvironmentTextureData( probe1.CubemapEnvironmentWithMipmapsAndBlur, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
						texture1Set = true;

						harmonics1 = new EnvironmentIrradianceData( EnvironmentIrradianceData.GetHarmonicsToUseMap( probe1.CubemapEnvironmentWithMipmapsAndBlur, probe1.data.CubemapEnvironmentAffectLightingLodOffset ), 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
						harmonics1Set = true;
					}
					else
					{
						if( probe1.data.CubemapEnvironment != null )
						{
							texture1 = new EnvironmentTextureData( probe1.data.CubemapEnvironment, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
							texture1Set = true;
						}
						if( probe1.data.HarmonicsIrradiance != null )
						{
							harmonics1 = new EnvironmentIrradianceData( probe1.data.HarmonicsIrradiance, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
							harmonics1Set = true;
						}
					}

					if( probe2.CubemapEnvironmentWithMipmapsAndBlur != null )
					{
						texture2 = new EnvironmentTextureData( probe2.CubemapEnvironmentWithMipmapsAndBlur, 1, ref probe2.data.Rotation, ref probe2.data.Multiplier );
						texture2Set = true;

						harmonics2 = new EnvironmentIrradianceData( EnvironmentIrradianceData.GetHarmonicsToUseMap( probe2.CubemapEnvironmentWithMipmapsAndBlur, probe2.data.CubemapEnvironmentAffectLightingLodOffset ), 1, ref probe2.data.Rotation, ref probe2.data.Multiplier );
						harmonics2Set = true;
					}
					else
					{
						if( probe2.data.CubemapEnvironment != null )
						{
							texture2 = new EnvironmentTextureData( probe2.data.CubemapEnvironment, 1, ref probe2.data.Rotation, ref probe2.data.Multiplier );
							texture2Set = true;
						}
						if( probe2.data.HarmonicsIrradiance != null )
						{
							harmonics2 = new EnvironmentIrradianceData( probe2.data.HarmonicsIrradiance, 1, ref probe2.data.Rotation, ref probe2.data.Multiplier );
							harmonics2Set = true;
						}
					}

					var sum = probe1Intensity + probe2Intensity;
					if( sum < 0.0001 )
						sum = 0.0001;
					blendingFactor = MathEx.Saturate( (float)( probe1Intensity / sum ) );
				}
				else if( probe1 != null )
				{
					if( probe1.CubemapEnvironmentWithMipmapsAndBlur != null )
					{
						texture1 = new EnvironmentTextureData( probe1.CubemapEnvironmentWithMipmapsAndBlur, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
						texture1Set = true;

						harmonics1 = new EnvironmentIrradianceData( EnvironmentIrradianceData.GetHarmonicsToUseMap( probe1.CubemapEnvironmentWithMipmapsAndBlur, probe1.data.CubemapEnvironmentAffectLightingLodOffset ), 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
						harmonics1Set = true;
					}
					else
					{
						if( probe1.data.CubemapEnvironment != null )
						{
							texture1 = new EnvironmentTextureData( probe1.data.CubemapEnvironment, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
							texture1Set = true;
						}
						if( probe1.data.HarmonicsIrradiance != null )
						{
							harmonics1 = new EnvironmentIrradianceData( probe1.data.HarmonicsIrradiance, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
							harmonics1Set = true;
						}
					}

					blendingFactor = MathEx.Saturate( (float)probe1Intensity );
				}
			}

			//init nulls by sky texture or white cube
			GetBackgroundEnvironmentData( context, frameData, out var backgroundTexture, out var backgroundTextureIBL );

			if( !texture1Set )
				texture1 = backgroundTexture;
			if( !texture2Set )
				texture2 = backgroundTexture;
			if( !harmonics1Set )
				harmonics1 = backgroundTextureIBL;
			if( !harmonics2Set )
				harmonics2 = backgroundTextureIBL;

			//if( texture1 == null )
			//	texture1 = backgroundTexture;
			//if( texture2 == null )
			//	texture2 = backgroundTexture;
			//if( harmonics1 == null )
			//	harmonics1 = backgroundTextureIBL;
			//if( harmonics2 == null )
			//	harmonics2 = backgroundTextureIBL;
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
			public float affectBackground;
			public float unused;
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

		[MethodImpl( (MethodImplOptions)512 )]
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
				if( ( mode & Fog.Modes.Exp ) != 0 )
					data->distanceMode = 1;
				if( ( mode & Fog.Modes.Exp2 ) != 0 )
					data->distanceMode = 2;
				data->startDistance = (float)fog.StartDistance;
				data->density = (float)fog.Density;
				if( ( mode & Fog.Modes.Height ) != 0 )
					data->heightMode = 1;
				data->height = (float)fog.Height;
				data->heightScale = (float)fog.HeightScale;
				data->affectBackground = (float)fog.AffectBackground;
			}
			if( fogExtensionEnabled && fogExtensionGetUniformData != null )
				fogExtensionGetUniformData( context, fog, uniformData, size );

			context.SetUniform( "u_fogSettings", ParameterType.Vector4, size / sizeof( Vector4F ), uniformData );

			NativeUtility.Free( uniformData );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public unsafe void BindRenderOperationData( ViewportRenderingContext context, FrameData frameData, Material.CompiledMaterialData materialData, bool instancing, RenderSceneData.MeshItem.AnimationDataClass meshItemAnimationData, int billboardMode, double billboardRadius, bool receiveDecals, ref Vector3F previousFramePositionChange, float lodValue, UnwrappedUVEnum unwrappedUV, ref ColorValue color, bool vertexStructureContainsColor, bool isLayer, float visibilityDistance, float motionBlurFactor, bool layerMaskFormatTriangles, ImageComponent voxelDataImage, Vector4F[] voxelDataInfo, Vector4F[] objectInstanceParameters, uint cullingByCameraDirectionData, ref Vector3F instancingPositionOffsetRelative/*, bool computeGI = false*/, float uvScale = 1.0f )//, ViewportRenderingContext.TessellationCacheItem tessItem = null, int tessIteration = 0 )
		{
			//can merge some parameters

			//!!!!without zero memory
			//var data44 = stackalloc RenderOperationDataStructure[1];

			var data = new RenderOperationDataStructure();

			if( materialData != null )
				data.data0.X = materialData.currentFrameIndex;
			if( instancing )
				data.data0.Y = -1;
			else if( meshItemAnimationData != null )
				data.data0.Y = meshItemAnimationData.BonesIndex + 1;//data.data0.Y = meshItemAnimationData.Mode;
			if( billboardMode != 0 )
			{
				data.data0.Z = billboardMode;
				data.data0.W = (float)billboardRadius;
			}

			if( receiveDecals )
				data.data1.X = 1;
			data.data1.Y = visibilityDistance;
			data.data1.Z = motionBlurFactor;
			data.data1.W = voxelDataInfo != null ? 1 : 0;

			data.data2 = new Vector4F( previousFramePositionChange, lodValue );

			data.data3.X = (int)unwrappedUV;
			if( vertexStructureContainsColor )
				data.data3.Y = 1;
			if( isLayer )
				data.data3.Z = layerMaskFormatTriangles ? 2 : 1;

			unsafe
			{
				*(uint*)&data.data3.W = cullingByCameraDirectionData;
			}

			//if( isLayer )
			//	data3.Z = 1;
			//data.data3.W = depthSortingLevel;
			//data.data3.W = objectIdStart;
			// virtualizedDataInfo != null ? 1 : 0;
			//data.data3.W = meshGeometryIndex;

			data.data4 = color;
			//data4 = color.ToVector4F();

			if( voxelDataInfo != null )
			{
				data.data5 = voxelDataInfo[ 0 ];
				data.data6 = voxelDataInfo[ 1 ];
			}
			//else if( virtualizedDataInfo != null )
			//{
			//	data.data5 = virtualizedDataInfo[ 0 ];
			//	//data.data6 = virtualizedDataInfo[ 1 ];
			//}

			if( instancing )
				data.data71 = instancingPositionOffsetRelative;

			data.data72 = uvScale;

			//if( tessItem != null )
			//{
			//	if( tessIteration == 0 )
			//		data.data72 = context.TessellationDistance;
			//	else
			//		data.data72 = -context.TessellationDistance;
			//	//data.data72 = tessIteration + 1;
			//}

			//set u_renderOperationData

			//if( !u_renderOperationData.HasValue )
			//	u_renderOperationData = GpuProgramManager.RegisterUniform( "u_renderOperationData", UniformType.Vector4, 7 );
			Bgfx.SetUniform( u_renderOperationData, &data, 8 );

			//it is slower because mostly 95%+ next render operation will be different

			//bool equal;
			//fixed( RenderOperationDataStructure* s = &renderOperationDataCurrent )
			//	equal = NativeUtility.CompareMemory( s, &data, sizeof( RenderOperationDataStructure ) ) == 0;

			//if( !equal )
			////if( !Vector4F.Equals( ref renderOperationDataCurrent.data0, ref data0 ) ||
			////	!Vector4F.Equals( ref renderOperationDataCurrent.data1, ref data1 ) ||
			////	!Vector4F.Equals( ref renderOperationDataCurrent.data2, ref data2 ) ||
			////	!Vector4F.Equals( ref renderOperationDataCurrent.data3, ref data3 ) ||
			////	!ColorValue.Equals( ref renderOperationDataCurrent.data4, ref color ) )
			//{

			//	renderOperationDataCurrent = data;
			//	//renderOperationDataCurrent.data0 = data0;
			//	//renderOperationDataCurrent.data1 = data1;
			//	//renderOperationDataCurrent.data2 = data2;
			//	//renderOperationDataCurrent.data3 = data3;
			//	//renderOperationDataCurrent.data4 = color;

			//	if( u_renderOperationData == null )
			//		u_renderOperationData = GpuProgramManager.RegisterUniform( "u_renderOperationData", UniformType.Vector4, 5 );
			//	fixed( RenderOperationDataStructure* pData = &renderOperationDataCurrent )
			//		Bgfx.SetUniform( u_renderOperationData.Value, pData, 5 );
			//}


			////bind bones texture
			//var bonesTexture = meshItemAnimationData?.BonesTexture;
			//if( bonesTexture != null )
			//{
			//	context.BindTexture( 0/*"s_bones"*/, bonesTexture, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None, 0, false );
			//}

			////bind materials data texture
			//if( bindMaterialsTexture && materialData != null )
			//{
			//	if( materialData.currentFrameIndex == -1 )
			//		Log.Fatal( "RenderingPipeline_Basic: BindRenderOperationData: materialData.currentFrameIndex == -1." );

			//	BindMaterialsTexture( context, frameData );
			//	//context.BindTexture( 1/*"s_materials"*/, frameData.MaterialsTexture, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None );
			//}

			//bind voxel data
			//if( voxelDataImage != null )
			{
				//!!!!slowly? когда null остается может не обновлять

				//!!!!
				context.BindTexture( 2/*"s_voxelData"*/, voxelDataImage ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None, 0, false );
				//context.BindTexture( computeGI ? 17 : 2/*"s_voxelData"*/, voxelDataImage ?? ResourceUtility.BlackTexture2D, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None, 0, false );
			}

			//set uniforms of object instance parameters
			{
				//if( !u_materialInstanceParameters.HasValue )
				//	u_materialInstanceParameters = GpuProgramManager.RegisterUniform( "u_materialInstanceParameters", UniformType.Vector4, 2 );

				if( objectInstanceParameters != null )
				{
					if( objectInstanceParametersIsNull || objectInstanceParametersLast1 != objectInstanceParameters[ 0 ] || objectInstanceParametersLast2 != objectInstanceParameters[ 1 ] )
					{
						objectInstanceParametersLast1 = objectInstanceParameters[ 0 ];
						objectInstanceParametersLast2 = objectInstanceParameters[ 1 ];
						objectInstanceParametersIsNull = false;

						fixed( Vector4F* parameters = objectInstanceParameters )
							Bgfx.SetUniform( u_objectInstanceParameters, parameters, 2 );
					}
				}
				else
				{
					if( !objectInstanceParametersIsNull )
					{
						objectInstanceParametersIsNull = true;

						var parameters = stackalloc Vector4F[ 2 ];
						parameters[ 0 ] = Vector4F.Zero;
						parameters[ 1 ] = Vector4F.Zero;
						Bgfx.SetUniform( u_objectInstanceParameters, parameters, 2 );
					}
				}
			}

			////bind virtualized data
			//if( virtualizedDataImage != null )
			//{
			//	context.BindTexture( 11/*"s_virtualizedData"*/, virtualizedDataImage, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None );
			//}
		}

		[Browsable( false )]
		public static ImageComponent BrdfLUT
		{
			[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
			get
			{
				if( brdfLUT == null )
					brdfLUT = ResourceManager.LoadResource<ImageComponent>( @"Base\Images\brdfLUT.dds" );
				return brdfLUT;
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void PrepareMaterialsTexture( ViewportRenderingContext context, FrameData frameData )
		{
			//add default materials
			frameData.AddMaterial( context, ResourceUtility.MaterialNull.Result );
			frameData.AddMaterial( context, ResourceUtility.MaterialInvalid.Result );

			//prepare texture


			var materialCount = frameData.Materials.Count;
			if( materialCount > RenderingSystem.Capabilities.MaxTextureSize )
				materialCount = RenderingSystem.Capabilities.MaxTextureSize;

			var size = new Vector2I( 8, MathEx.NextPowerOfTwo( materialCount ) );
			if( size.Y > RenderingSystem.Capabilities.MaxTextureSize )
				size.Y = RenderingSystem.Capabilities.MaxTextureSize;

			//it is ok to use static temp array because SetData loads data to Gpu immediately
			int bufferSize = size.X * size.Y * 8;
			if( prepareMaterialsBonesLightsTempBuffer == null || bufferSize > prepareMaterialsBonesLightsTempBuffer.Length )
				prepareMaterialsBonesLightsTempBuffer = new byte[ bufferSize ];
			var data = prepareMaterialsBonesLightsTempBuffer;

			fixed( byte* pData = data )
			{
				for( int n = 0; n < materialCount; n++ )
				{
					var materialData = frameData.Materials.Data[ n ];

					materialData.UpdateDynamicParametersFragmentUniformData( context );

					var pData2 = pData + n * sizeof( Material.CompiledMaterialData.DynamicParametersFragmentUniform );
					var pData3 = (Material.CompiledMaterialData.DynamicParametersFragmentUniform*)pData2;
					*pData3 = materialData.materialDataDynamicParametersData;
				}
			}



			//int horizontalCount = 64;

			//Vector2I size = new Vector2I( horizontalCount * 8, Math.Max( 32, MathEx.NextPowerOfTwo( frameData.Materials.Count / horizontalCount + 1 ) ) );
			//if( size.Y > RenderingSystem.Capabilities.MaxTextureSize )
			//	size.Y = RenderingSystem.Capabilities.MaxTextureSize;
			//int totalCount = horizontalCount * size.Y;

			////it is ok to use static temp array because SetData loads data to Gpu immediately
			//int bufferSize = size.X * size.Y * 8;
			//if( prepareMaterialsBonesLightsTempBuffer == null || bufferSize > prepareMaterialsBonesLightsTempBuffer.Length )
			//	prepareMaterialsBonesLightsTempBuffer = new byte[ bufferSize ];
			//var data = prepareMaterialsBonesLightsTempBuffer;

			//fixed( byte* pData = data )
			//{
			//	for( int n = 0; n < frameData.Materials.Count; n++ )
			//	{
			//		if( n >= totalCount )
			//			break;

			//		var materialData = frameData.Materials.Data[ n ];

			//		materialData.UpdateDynamicParametersFragmentUniformData( context );

			//		var pData2 = pData + n * sizeof( Material.CompiledMaterialData.DynamicParametersFragmentUniform );
			//		var pData3 = (Material.CompiledMaterialData.DynamicParametersFragmentUniform*)pData2;
			//		*pData3 = materialData.materialDataDynamicParametersData;
			//	}
			//}

			frameData.MaterialsTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, size, PixelFormat.Float16RGBA, 0, false );
			frameData.MaterialsTexture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( new ArraySegment<byte>( data, 0, bufferSize ) ) } );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void PrepareBonesTexture( ViewportRenderingContext context, FrameData frameData )
		{
			if( context.AnimationBonesData.Count != 0 )
			{
				if( context.AnimationBonesDataTasks.Count != 0 )
					Task.WaitAll( context.AnimationBonesDataTasks.ToArray() );

				var maxCount = 0;
				foreach( var bones in context.AnimationBonesData )
				{
					if( bones.Length > maxCount )
						maxCount = bones.Length;
				}

				var sizeX = MathEx.NextPowerOfTwo( maxCount * 4 );
				var sizeY = MathEx.NextPowerOfTwo( context.AnimationBonesData.Count );
				if( sizeY > RenderingSystem.Capabilities.MaxTextureSize )
					sizeY = RenderingSystem.Capabilities.MaxTextureSize;
				var size = new Vector2I( sizeX, sizeY );


				//use half on mobile
				if( SystemSettings.LimitedDevice )
				{
					//half

					var bufferSize = sizeX * sizeY * 8;
					if( prepareMaterialsBonesLightsTempBuffer == null || bufferSize > prepareMaterialsBonesLightsTempBuffer.Length )
						prepareMaterialsBonesLightsTempBuffer = new byte[ bufferSize ];
					var data = prepareMaterialsBonesLightsTempBuffer;

					fixed( byte* pData = data )
					{
						for( int nBones = 0; nBones < context.AnimationBonesData.Count; nBones++ )
						{
							if( nBones >= size.Y )
								break;

							var bones = context.AnimationBonesData[ nBones ];
							//fixed( Matrix4F* pBones = bones )
							//{
							var pData2 = pData + nBones * sizeX * 8;
							Matrix4H* pData3 = (Matrix4H*)pData2;

							for( int n = 0; n < bones.Length; n++ )
								bones[ n ].ToMatrix4H( out pData3[ n ] );
							//}
						}
					}

					frameData.BonesTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, size, PixelFormat.Float16RGBA, 0, false );
					frameData.BonesTexture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( new ArraySegment<byte>( data, 0, bufferSize ) ) } );
				}
				else
				{
					//float

					var bufferSize = sizeX * sizeY * 16;
					if( prepareMaterialsBonesLightsTempBuffer == null || bufferSize > prepareMaterialsBonesLightsTempBuffer.Length )
						prepareMaterialsBonesLightsTempBuffer = new byte[ bufferSize ];
					var data = prepareMaterialsBonesLightsTempBuffer;

					fixed( byte* pData = data )
					{
						for( int nBones = 0; nBones < context.AnimationBonesData.Count; nBones++ )
						{
							if( nBones >= size.Y )
								break;

							var bones = context.AnimationBonesData[ nBones ];
							fixed( Matrix4F* pBones = bones )
							{
								var pData2 = pData + nBones * sizeX * 16;
								NativeUtility.CopyMemory( pData2, pBones, bones.Length * sizeof( Matrix4F ) );
							}
						}
					}

					frameData.BonesTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, size, PixelFormat.Float32RGBA, 0, false );
					frameData.BonesTexture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( new ArraySegment<byte>( data, 0, bufferSize ) ) } );
				}
			}
			else
				frameData.BonesTexture = ResourceUtility.BlackTexture2D;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void PrepareLightsTexture( ViewportRenderingContext context, FrameData frameData )
		{
			if( sizeof( LightItem.LightDataFragmentMultiLight ) > 512 )
				Log.Fatal( "RenderingPipeline_Basic: PrepareLightsTexture: sizeof( LightItem.LightDataFragmentMultiLight ) > 512." );

			//!!!!можно в ряд тоже выкладывать

			var size = new Vector2I( 32, Math.Max( 32, MathEx.NextPowerOfTwo( frameData.LightsInFrustumSorted.Length ) ) );
			if( size.Y > RenderingSystem.Capabilities.MaxTextureSize )
				size.Y = RenderingSystem.Capabilities.MaxTextureSize;

			int lightCountToWrite = Math.Min( frameData.LightsInFrustumSorted.Length, size.Y );

			//use half on mobile
			if( SystemSettings.LimitedDevice ) // !Texture.IsValid( 1, false, 1, TextureFormat.RGBA32F ) )
			{
				//half

				//it is ok to use static temp array because SetData loads data to Gpu immediately
				int bufferSize = size.X * size.Y * 8;
				if( prepareMaterialsBonesLightsTempBuffer == null || bufferSize > prepareMaterialsBonesLightsTempBuffer.Length )
					prepareMaterialsBonesLightsTempBuffer = new byte[ bufferSize ];
				var data = prepareMaterialsBonesLightsTempBuffer;

				fixed( byte* pData = data )
				{
					for( int n = 0; n < lightCountToWrite; n++ )
					{
						var lightIndex = frameData.LightsInFrustumSorted[ n ];
						var lightItem = frameData.Lights[ lightIndex ];

						var pData2 = (HalfType*)( pData + n * 256 );

						LightItem.LightDataFragmentMultiLight data3;
						lightItem.InitLightDataBuffersMultiLight( this, context, &data3 );
						float* pData3 = (float*)&data3;

						var count = sizeof( LightItem.LightDataFragmentMultiLight ) / 4;
						for( int z = 0; z < count; z++ )
							*( pData2 + z ) = new HalfType( *( pData3 + z ) );
					}
				}

				frameData.LightsTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, size, PixelFormat.Float16RGBA, 0, false );
				frameData.LightsTexture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( new ArraySegment<byte>( data, 0, bufferSize ) ) } );
			}
			else
			{
				//float

				//it is ok to use static temp array because SetData loads data to Gpu immediately
				int bufferSize = size.X * size.Y * 16;
				if( prepareMaterialsBonesLightsTempBuffer == null || bufferSize > prepareMaterialsBonesLightsTempBuffer.Length )
					prepareMaterialsBonesLightsTempBuffer = new byte[ bufferSize ];
				var data = prepareMaterialsBonesLightsTempBuffer;

				fixed( byte* pData = data )
				{
					for( int n = 0; n < lightCountToWrite; n++ )
					{
						var lightIndex = frameData.LightsInFrustumSorted[ n ];
						var lightItem = frameData.Lights[ lightIndex ];

						var pData2 = pData + n * 512;
						lightItem.InitLightDataBuffersMultiLight( this, context, (LightItem.LightDataFragmentMultiLight*)pData2 );
					}
				}

				frameData.LightsTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, size, PixelFormat.Float32RGBA, 0, false );
				frameData.LightsTexture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( new ArraySegment<byte>( data, 0, bufferSize ) ) } );
			}
		}

		//static bool? isIntelGPU;
		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		bool GetDeferredShading()
		{
			if( !GetUseMultiRenderTargets() )
				return false;

			if( !RenderingSystem.DeferredShading )
				return false;

			//deferred shading is not supported on limited devices
			if( SystemSettings.LimitedDevice )
				return false;

			var result = DeferredShading.Value;

			if( result == AutoTrueFalse.Auto )
			{
				result = AutoTrueFalse.True;

				//!!!!temp
				//if( EngineApp._DebugCapsLock )
				//	result = AutoTrueFalse.False;

				//disable deferred on Intel by default

				//if( isIntelGPU == null )
				//	isIntelGPU = Bgfx.GetGPUDescription().Contains( "Intel" );

				//if( isIntelGPU.Value )
				//	result = AutoTrueFalse.False;
				//else
				//	result = AutoTrueFalse.True;
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
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

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void BindMaterialsTexture( ViewportRenderingContext context, FrameData frameData )//, bool compute = false )
		{
			//if( compute )
			//{
			//	context.BindTexture( 1, frameData.MaterialsTexture, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None, 0, false );
			//}
			//else
			//{
			context.BindTexture( 1, frameData.MaterialsTexture, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None, 0, false );
			//}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void BindBonesTexture( ViewportRenderingContext context, FrameData frameData )//, bool compute = false )
		{
			//if( compute )
			//{
			//	context.BindTexture( 0, frameData.BonesTexture, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None, 0, false );
			//}
			//else
			//{
			context.BindTexture( 0, frameData.BonesTexture, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None, 0, false );
			//}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void BindBrdfLUT( ViewportRenderingContext context )//not good sampler (clamp), bool computeGI = false )
		{
			//if( computeGI )
			//{
			//	context.BindTexture( 17/*"s_brdfLUT"*/, BrdfLUT, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, false );
			//}
			//else
			//{

			context.BindTexture( 6/*"s_brdfLUT"*/, BrdfLUT, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, false );

			//}
		}

		static bool currentBindSamplersForTextureOnlySlotsVoxelRendering;

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void BindSamplersForTextureOnlySlots( ViewportRenderingContext context, bool forceUpdate, bool disableAnisotropic )
		{
			if( !SystemSettings.LimitedDevice )
			{
				if( forceUpdate || currentBindSamplersForTextureOnlySlotsVoxelRendering != disableAnisotropic )
				{
					currentBindSamplersForTextureOnlySlotsVoxelRendering = disableAnisotropic;

					var filtering = ( !disableAnisotropic && RenderingSystem.AnisotropicFiltering ) ? FilterOption.Anisotropic : FilterOption.Linear;

					context.BindTexture( 9/*"s_linearSamplerVertex"*/, ResourceUtility.WhiteTexture2D, TextureAddressingMode.Wrap, filtering, filtering, FilterOption.Linear, 0, disableAnisotropic );

					//context.BindTexture( 10/*"s_linearSamplerFragment"*/, ResourceUtility.WhiteTexture2D, TextureAddressingMode.Wrap, filtering, filtering, FilterOption.Linear, 0, disableAnisotropic );

					////context.BindTexture( 9/*"s_linearSamplerVertex"*/, ResourceUtility.WhiteTexture2D, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
					////context.BindTexture( 10/*"s_linearSamplerFragment"*/, ResourceUtility.WhiteTexture2D, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
				}
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void BindSamplersForTextureOnlySlots( CanvasRenderer.ShaderItem shader )
		{
			if( !SystemSettings.LimitedDevice )
			{
				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 9/*"s_linearSamplerVertex"*/, ResourceUtility.WhiteTexture2D, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
			}
		}

		static Material.CompiledMaterialData currentBindMaterialData;

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		public void BindMaterialData( ViewportRenderingContext context, Material.CompiledMaterialData materialData, bool specialShadowCaster, bool disableAnisotropic )
		{
			if( currentBindMaterialData != materialData )
			{
				currentBindMaterialData = materialData;
				materialData?.BindCurrentFrameData( context, specialShadowCaster, disableAnisotropic );
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public void BindForwardLightAndShadows( ViewportRenderingContext context, FrameData frameData, bool giCompute = false )
		{
			//lights texture
			if( giCompute )
			{
				context.BindTexture( 16/* "s_lightsTexture"*/, frameData.LightsTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );
			}
			else
			{
				context.BindTexture( 7/* "s_lightsTexture"*/, frameData.LightsTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );
			}

			//light grid
			if( RenderingSystem.LightGrid )
				context.BindTexture( 8/* "s_lightGrid"*/, frameData.LightGrid ?? ResourceUtility.DummyTexture3DFloat32RGBA, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );

			//shadows
			{
				var isByte4Format = RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4;
				var filtering = isByte4Format ? FilterOption.Point : FilterOption.Linear;
				////too chaotic
				//////to make linear light masks on mobile
				////var filtering = FilterOption.Linear;
				var textureFlags = isByte4Format ? 0 : TextureFlags.CompareLessEqual;

				//shadow map directional
				{
					var shadowMap = frameData.ShadowTextureArrayDirectional;
					if( shadowMap == null )
						shadowMap = isByte4Format ? ResourceUtility.DummyTexture2DArrayARGB8 : ResourceUtility.DummyShadowMap2DArrayFloat32R;

					var wrap = isByte4Format;

					context.BindTexture( SystemSettings.LimitedDevice ? 8 : 10/*s_shadowMapShadowDirectional*/, shadowMap, wrap ? TextureAddressingMode.Wrap : TextureAddressingMode.Clamp, filtering, filtering, FilterOption.None, textureFlags );
				}

				//shadow map array spot
				{
					var shadowMap = frameData.ShadowTextureArraySpot;
					if( shadowMap == null )
						shadowMap = isByte4Format ? ResourceUtility.DummyTexture2DArrayARGB8 : ResourceUtility.DummyShadowMap2DArrayFloat32R;

					context.BindTexture( SystemSettings.LimitedDevice ? 9 : 11/*s_shadowMapShadowSpot*/, shadowMap, TextureAddressingMode.Clamp, filtering, filtering, FilterOption.None, textureFlags );
				}

				//shadow map array point
				{
					var shadowMap = frameData.ShadowTextureArrayPoint;
					if( shadowMap == null )
						shadowMap = isByte4Format ? ResourceUtility.DummyTextureCubeArrayARGB8 : ResourceUtility.DummyShadowMapCubeArrayFloat32R;

					context.BindTexture( SystemSettings.LimitedDevice ? 10 : 12/*s_shadowMapShadowPoint*/, shadowMap, TextureAddressingMode.Clamp, filtering, filtering, FilterOption.None, textureFlags );
				}
			}

			//light masks
			//mobile specific. on limited devices masks and shadow maps are managed inside one shadow map array
			if( RenderingSystem.LightMask && !SystemSettings.MobileDevice )
			{
				//mask array directional
				{
					var texture = frameData.MaskTextureArrayDirectional ?? ResourceUtility.WhiteTexture2D;
					context.BindTexture( 13/*s_lightMaskDirectional*/, texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
				}

				//mask array spot
				{
					var texture = frameData.MaskTextureArraySpot ?? ResourceUtility.WhiteTexture2D;
					context.BindTexture( 14/*s_lightMaskSpot*/, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
				}

				//mask array point
				{
					var texture = frameData.MaskTextureArrayPoint ?? ResourceUtility.WhiteTextureCube;
					context.BindTexture( 15/*s_lightMaskPoint*/, texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear );
				}
			}
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

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void RenderOccluder( ViewportRenderingContext context, OcclusionCullingBuffer buffer, ref OccluderItem occluderItem, Matrix4F* viewProjMatrixFloat )
		{
			var vertices = occluderItem.Vertices;
			var indices = occluderItem.Indices;

#if UWP
			//!!!!
			fixed( Vector3F* verticesFloat = new Vector3F[ vertices.Length ] )
#else
			fixed( Vector3F* verticesFloat = vertices.Length < 512 ? stackalloc Vector3F[ vertices.Length ] : new Vector3F[ vertices.Length ] )
#endif
			{
				for( int n = 0; n < vertices.Length; n++ )
					context.ConvertToRelative( ref vertices[ n ], out verticesFloat[ n ] );

				fixed( int* pIndices = indices )
					buffer.RenderTriangles( (float*)verticesFloat, (uint*)pIndices, indices.Length / 3, (float*)viewProjMatrixFloat );
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void OcclusionCullingBuffer_RenderOccluders( ViewportRenderingContext context, /*FrameData frameData, */Viewport.CameraSettingsClass cameraSettings, OcclusionCullingBuffer buffer, OpenList<OccluderItem> occluders, bool sceneOccluder, ref bool occlusionCullingBufferRendered )
		{
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

			CollectionUtility.MergeSortUnmanaged( occluderIndices, occluders.Count, delegate ( int index1, int index2 )
			{
				ref var item1 = ref occluders.Data[ index1 ];
				ref var item2 = ref occluders.Data[ index2 ];
				if( item1.tempDistanceSquared < item2.tempDistanceSquared )
					return -1;
				if( item1.tempDistanceSquared > item2.tempDistanceSquared )
					return 1;
				return 0;
			}, true );


			//render to the buffer

			var viewProjMatrixFloat = cameraSettings.GetViewProjectionMatrixRelative();

			buffer.ClearBuffer();

			var maxCount = OcclusionCullingBufferMaxOccluders.Value;

			for( int nOccluder = 0; nOccluder < occluders.Count; nOccluder++ )
			{
				if( nOccluder >= maxCount )
					break;

				ref var occluderItem = ref occluders.Data[ occluderIndices[ nOccluder ] ];
				RenderOccluder( context, buffer, ref occluderItem, &viewProjMatrixFloat );

				//var vertices = occluderItem.Vertices;
				//var indices = occluderItem.Indices;

				////!!!!double
				//var verticesFloat = stackalloc Vector3F[ vertices.Length ];
				//for( int n = 0; n < vertices.Length; n++ )
				//	verticesFloat[ n ] = vertices[ n ].ToVector3F();

				//fixed( int* pIndices = indices )
				//	buffer.RenderTriangles( (float*)verticesFloat, (uint*)pIndices, indices.Length / 3, (float*)&viewProjMatrixFloat );
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

				occlusionCullingBufferRendered = true;
			}
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( !EnabledInHierarchy )
			{
				//they are static

				//while( outputInstancingManagers.Count != 0 )
				//{
				//	var manager = outputInstancingManagers[ outputInstancingManagers.Count - 1 ];
				//	outputInstancingManagers.RemoveAt( outputInstancingManagers.Count - 1 );
				//	manager.Dispose();
				//}
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void PrepareReflectionProbesCubemapEnvironmentMipmapsAndBlur( ViewportRenderingContext context, FrameData frameData )
		{
			foreach( var item in frameData.ReflectionProbes )
			{
				if( item.data.CubemapEnvironmentMipmapsAndBlurRequired > 0 )
				{
					var sourceTexture = item.data.CubemapEnvironment;
					var blurFactor = item.data.CubemapEnvironmentMipmapsAndBlurRequired;

					var settings = new GaussianBlurSettings();
					settings.SourceTexture = sourceTexture;
					settings.BlurFactor = blurFactor;
					settings.DownscalingMode = DownscalingModeEnum.Manual;
					settings.DownscalingValue = 0;
					settings.GenerateMips = true;

					item.CubemapEnvironmentWithMipmapsAndBlur = GaussianBlur( context, settings );





					//item.CubemapEnvironmentWithMipmapsAndBlur = GaussianBlur( context, sourceTexture, blurFactor, DownscalingModeEnum.Manual, 0 );


					//var textureSize = sourceTexture.Result.ResultSize.X;

					//item.CubemapEnvironmentWithMipmapsAndBlur = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.RenderTarget, ImageComponent.TypeEnum.Cube, new Vector2I( textureSize, textureSize ), sourceTexture.Result.ResultFormat, 0, false );

					//for( var face = 0; face < 6; face++ )
					//{
					//	var slice = face;// index * 6 + face;

					//	var viewport = item.CubemapEnvironmentWithMipmapsAndBlur.Result.GetRenderTarget( slice: slice ).Viewports[ 0 ];
					//	context.SetViewport( viewport );

					//	//!!!!GC. where else
					//	//!!!!also can copy with one draw call, use mrt or compute

					//	var shader = new CanvasRenderer.ShaderItem();
					//	shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
					//	shader.FragmentProgramFileName = @"Base\Shaders\BlurCubemap_fs.sc";

					//	shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/, sourceTexture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

					//	//!!!!
					//	var sourceMip = 0;

					//	shader.Parameters.Set( "u_blurCubemapParameters", new Vector4F( face, sourceMip, 0, 0 ) );

					//	context.RenderQuadToCurrentViewport( shader );
					//}







					//!!!!how to write to cubemap


					//context.SetComputePass();

					////!!!!mips

					//item.CubemapEnvironmentWithMipmapsAndBlur = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, ImageComponent.TypeEnum.Cube, new Vector2I( textureSize, textureSize ), sourceTexture.Result.ResultFormat, 0, false );


					//for( var face = 0; face < 6; face++ )
					//{
					//	qqvar v = new Vector4F( face, 0, 0, 0 );
					//	context.SetUniform( "u_blurCubemapFaceIndex", ParameterType.Vector4, 1, &v );

					//	//!!!!
					//	var mip = face;

					//	context.BindComputeImage( 0, item.CubemapEnvironmentWithMipmapsAndBlur, mip, ComputeBufferAccessEnum.Write );

					//	context.BindTexture( 1/* "s_sourceTexture"*/, sourceTexture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Point );

					//	if( blurCubemapProgram == null )
					//	{
					//		var program = GpuProgramManager.GetProgram( "BlurCubemap", GpuProgramType.Compute, @"Base\Shaders\BlurCubemap.sc", null, true, out var error2 );
					//		if( !string.IsNullOrEmpty( error2 ) )
					//			Log.Fatal( "RenderingPipeline_Basic: PrepareReflectionProbesCubemapEnvironmentMipmapsAndBlur: " + error2 );
					//		else
					//			blurCubemapProgram = new Program( program.RealObject );
					//	}

					//	var jobSize = new Vector3I( (int)Math.Ceiling( textureSize / 32.0 ), (int)Math.Ceiling( textureSize / 32.0 ), 1 );

					//	Bgfx.Dispatch( context.CurrentViewNumber, blurCubemapProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
					//	context.UpdateStatisticsCurrent.ComputeDispatches++;
					//}

				}
			}
		}
	}
}
