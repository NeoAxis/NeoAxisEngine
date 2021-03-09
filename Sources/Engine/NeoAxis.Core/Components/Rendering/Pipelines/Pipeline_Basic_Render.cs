// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using SharpBgfx;
using System.Threading.Tasks;
using System.Threading;

namespace NeoAxis
{
	public partial class Component_RenderingPipeline_Basic
	{
		const int instancingSectorCount = 10;

		// Don't add many non static fields. Rendering pipeline is created for each temporary render target during frame rendering.

		static Uniform? u_reflectionProbeData;
		static Uniform? u_environmentTextureRotation;
		static Uniform? u_environmentTextureIBLRotation;
		static Uniform? u_environmentTextureMultiplierAndAffect;
		static Uniform? u_environmentTextureIBLMultiplierAndAffect;

		static Uniform? u_environmentTexture1Rotation;
		static Uniform? u_environmentTexture2Rotation;
		static Uniform? u_environmentTexture1MultiplierAndAffect;
		static Uniform? u_environmentTexture2MultiplierAndAffect;
		static Uniform? u_environmentBlendingFactor;

		static Uniform? u_renderOperationData;
		static Vector4F[] u_renderOperationDataCurrentValue = new Vector4F[] { new Vector4F( float.MaxValue, 0, 0, 0 ), Vector4F.Zero, Vector4F.Zero, Vector4F.Zero, Vector4F.Zero };

		static Component_Image nullShadowTexture2D;
		static Component_Image nullShadowTextureCube;
		static Component_Image brdfLUT;

		static ShadowCasterData defaultShadowCasterData;
		static DeferredShadingData deferredShadingData;
		static Component_Mesh decalMesh;

		static OutputInstancingManager outputInstancingManager = new OutputInstancingManager();

		static byte[] prepareMaterialsTempBuffer;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public delegate void InitLightDataBuffersEventDelegate( Component_RenderingPipeline_Basic sender, ViewportRenderingContext context, LightItem lightItem );
		public event InitLightDataBuffersEventDelegate InitLightDataBuffersEvent;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		class DeferredShadingData
		{
			public PassItem[] passesPerLightWithoutShadows = new PassItem[ 4 ];
			public PassItem[] passesPerLightWithShadowsLow = new PassItem[ 4 ];
			public PassItem[] passesPerLightWithShadowsHigh = new PassItem[ 4 ];

			public Component_Mesh clearBackgroundMesh;
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
				passesPerLightWithShadowsLow = null;
				passesPerLightWithShadowsHigh = null;

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

			public OpenList<ObjectInSpaceItem> ObjectInSpaces;

			public OpenList<MeshItem> Meshes;
			public OpenList<BillboardItem> Billboards;
			public List<LightItem> Lights;
			public List<ReflectionProbeItem> ReflectionProbes;
			public List<DecalItem> Decals;

			public int[] LightsInFrustumSorted = new int[ 0 ];

			public List<Vector2I> RenderableGroupsInFrustum;

			public Component_Sky Sky;
			public Component_Fog Fog;

			public OpenList<Component_Material.CompiledMaterialData> Materials;
			public Component_Image MaterialsTexture;

			////////////

			/// <summary>
			/// Represents an object in space data of <see cref="FrameData"/>.
			/// </summary>
			public struct ObjectInSpaceItem
			{
				public Component_ObjectInSpace ObjectInSpace;
				public RangeI MeshRange;
				public RangeI BillboardRange;
				//public RangeI LightRange;
				//public RangeI ReflectionProbeRange;
				//public RangeI DecalRange;

				//!!!!
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

				Materials = new OpenList<Component_Material.CompiledMaterialData>( 512 * multiplier );
			}

			void AddItems( ViewportRenderingContext context, int meshes, int meshes2, int billboards, int billboards2, int lights, int lights2, int reflectionProbes, int reflectionProbes2, int decals, int decals2, bool addMaterialsAddOnlySpecialShadowCasters )
			{
				var cameraPosition = context.Owner.CameraSettings.Position;

				for( int n = meshes; n < meshes2; n++ )
				{
					ref var meshItem = ref RenderSceneData.Meshes.Data[ n ];
					var meshData = meshItem.MeshData;

					var data = new MeshItem();

					//!!!!так?
					//meshItem.BoundingBoxCenter.GetCenter( out var center );
					data.DistanceToCameraSquared = (float)( meshItem.BoundingBoxCenter - cameraPosition ).LengthSquared();

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
							var pipeline = (Component_RenderingPipeline_Basic)context.RenderingPipeline;
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

						if( materialData.ShadingModel != Component_Material.ShadingModelEnum.Unlit )
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
							var materialData = layer.Material;//?.Result;
							if( materialData == null )
								materialData = ResourceUtility.MaterialNull.Result;

							//if( materialData != null )
							//{
							if( materialData.deferredShadingSupport )
							{
								var pipeline = (Component_RenderingPipeline_Basic)context.RenderingPipeline;
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

							if( materialData.ShadingModel != Component_Material.ShadingModelEnum.Unlit )
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
					data.DistanceToCameraSquared = (float)( data2.BoundingBoxCenter - cameraPosition ).LengthSquared();

					//Flags
					bool lit = false;

					var materialData = GetBillboardMaterialData( ref data2, false );

					if( materialData.deferredShadingSupport )
					{
						var pipeline = (Component_RenderingPipeline_Basic)context.RenderingPipeline;
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

					if( materialData.ShadingModel != Component_Material.ShadingModelEnum.Unlit )
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

			public void SceneGetRenderSceneData( ViewportRenderingContext context, Component_Scene scene )
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
				item.Type = Component_Light.TypeEnum.Ambient;
				context.FrameData.RenderSceneData.Lights.Add( ref item );

				int lights2 = RenderSceneData.Lights.Count;

				AddItems( context, 0, 0, 0, 0, lights, lights2, 0, 0, 0, 0, false );
			}

			public int RegisterObjectInSpace( ViewportRenderingContext context, Component_ObjectInSpace objectInSpace, GetRenderSceneDataMode mode, Component_Scene.GetObjectsInSpaceItem modeGetObjectsItem )
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
					if( lights2 > lights )
					{
						//data.LightRange = new RangeI( lights, lights2 );
						data.ContainsData = true;
					}
					if( reflectionProbes2 > reflectionProbes )
					{
						//data.ReflectionProbeRange = new RangeI( reflectionProbes, reflectionProbes2 );
						data.ContainsData = true;
					}
					if( decals2 > decals )
					{
						//data.DecalRange = new RangeI( decals, decals2 );
						data.ContainsData = true;
					}
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

			//!!!!new
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

			public void AddMaterial( ViewportRenderingContext context, Component_Material.CompiledMaterialData materialData )
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
			public Component_Image shadowTexture;
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

				////!!!!new
				public Vector4F unitDistanceWorldShadowTexelSizes;

				public float lightShadowBias;
				public float lightShadowNormalBias;
				public float unused1;
				public float unused2;

				//public float lightShadowSoftness;
				//public float unused3;

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

				if( data.Type == Component_Light.TypeEnum.Directional )
					distanceToCameraSquared = 10000000000.0f;
				else //if( data.transform != null )
					distanceToCameraSquared = (float)( context.Owner.CameraSettings.Position - data.Position ).LengthSquared();
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

			void InitLightDataBuffers( Component_RenderingPipeline_Basic pipeline, ViewportRenderingContext context )//, double shadowIntensity )
			{
				//!!!!double

				//!!!!!кешировать и раньше рассчитывать. в RenderSceneData по идее

				Vector3F direction = Vector3F.XAxis;
				if( data.Type != Component_Light.TypeEnum.Ambient )
					direction = data.Rotation.GetForward();

				Vector4 position = Vector4.Zero;
				if( data.Type != Component_Light.TypeEnum.Ambient )
				{
					if( data.Type == Component_Light.TypeEnum.Directional )
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
				if( data.Type == Component_Light.TypeEnum.Spotlight )
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
					if( data.Type == Component_Light.TypeEnum.Spotlight || data.Type == Component_Light.TypeEnum.Directional )
					{
						Matrix4F toImageSpace = new Matrix4F(
							0.5f, 0, 0, 0,
							0, -0.5f, 0, 0,
							0, 0, 1, 0,
							0.5f, 0.5f, 0, 1 );
						//const float sy = caps->originBottomLeft ? 0.5f : -0.5f;
						//const float sz = caps->homogeneousDepth ? 0.5f : 1.0f;
						//const float tz = caps->homogeneousDepth ? 0.5f : 0.0f;
						//const float mtxCrop[ 16 ] =
						//{
						//	0.5f, 0.0f, 0.0f, 0.0f,
						//	0.0f,   sy, 0.0f, 0.0f,
						//	0.0f, 0.0f, sz,   0.0f,
						//	0.5f, 0.5f, tz,   1.0f,
						//};

						if( data.Type == Component_Light.TypeEnum.Directional )
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

					if( data.Type == Component_Light.TypeEnum.Point )
					{
						Matrix4F shadowProjectionMatrix = shadowViewport.CameraSettings.ProjectionMatrix.ToMatrix4F();
						//Log.Info( shadowProjectionMatrix );
						lightDataFragment.unitDistanceWorldShadowTexelSizes[ 0 ] =
							1.414213562f // sqrt(2), texel diagonal (maximum) size
							* ( 2.0f / shadowProjectionMatrix[ 0 ][ 0 ] ) // (r - l) / (near), world X texel size at unit distance
							/ lightDataFragment.lightShadowTextureSize; // how many texels in shadow map side
					}

					//lightShadowCascades
					if( data.Type == Component_Light.TypeEnum.Directional )
					{
						lightDataFragment.lightShadowCascades = Vector4F.Zero;
						lightDataFragment.lightShadowCascades[ 0 ] = pipeline.ShadowDirectionalLightCascades;
						var splitDistances = pipeline.GetShadowCascadeSplitDistances( context );
						if( pipeline.ShadowDirectionalLightCascades >= 2 )
							lightDataFragment.lightShadowCascades[ 1 ] = (float)splitDistances[ 1 ];
						if( pipeline.ShadowDirectionalLightCascades >= 3 )
							lightDataFragment.lightShadowCascades[ 2 ] = (float)splitDistances[ 2 ];
						if( pipeline.ShadowDirectionalLightCascades >= 4 )
							lightDataFragment.lightShadowCascades[ 3 ] = (float)splitDistances[ 3 ];
					}

					//lightDataFragment.lightShadowSoftness = data.ShadowSoftness;
				}

				//mask
				if( data.Mask != null )
				{
					//!!!!если .Zero

					Matrix4F mat = Matrix4F.Zero;

					switch( data.Type )
					{
					case Component_Light.TypeEnum.Directional:
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

					case Component_Light.TypeEnum.Point:
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

					case Component_Light.TypeEnum.Spotlight:
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

				lightDataFragment.lightShadowBias = data.ShadowBias;
				lightDataFragment.lightShadowNormalBias = data.ShadowNormalBias;

				pipeline.InitLightDataBuffersEvent?.Invoke( pipeline, context, this );
			}

			public unsafe void Bind( Component_RenderingPipeline_Basic pipeline, ViewportRenderingContext context )
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
				distanceToCameraSquared = (float)( context.Owner.CameraSettings.Position - data.Sphere.Origin ).LengthSquared();

				if( data.Sphere.Radius <= 0 )
					Log.Fatal( "Component_RenderingPipeline_Basic: ReflectionProbeItem: Constructor: data.Sphere.Radius <= 0." );
			}

			//void InitReflectionProbeDataBuffers( Component_RenderingPipeline_Basic pipeline, ViewportRenderingContext context )
			//{
			//	double radius = data.Sphere.Radius;
			//	var position = new Vector4( data.Sphere.Origin, 1 );

			//	reflectionProbeDataFragment.reflectionProbePosition = position.ToVector4F();
			//	reflectionProbeDataFragment.reflectionProbeRadius = (float)radius;
			//}

			//public unsafe void Bind( Component_RenderingPipeline_Basic pipeline, ViewportRenderingContext context )
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

				//!!!! 4
				data.passByLightType = new GpuMaterialPass[ 4 ];

				foreach( Component_Light.TypeEnum lightType in Enum.GetValues( typeof( Component_Light.TypeEnum ) ) )
				{
					if( lightType == Component_Light.TypeEnum.Ambient )
						continue;

					//generate compile arguments
					var vertexDefines = new List<(string, string)>();
					var fragmentDefines = new List<(string, string)>();
					{
						var generalDefines = new List<(string, string)>();
						generalDefines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );

						vertexDefines.AddRange( generalDefines );
						fragmentDefines.AddRange( generalDefines );
					}

					string error2;

					//vertex program
					GpuProgram vertexProgram = GpuProgramManager.GetProgram( "ShadowCasterDefault_Vertex_", GpuProgramType.Vertex,
						@"Base\Shaders\ShadowCasterDefault_vs.sc", /*"main_vp", */vertexDefines, out error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						Log.Fatal( error2 );

					//fragment program
					GpuProgram fragmentProgram = GpuProgramManager.GetProgram( "ShadowCasterDefault_Fragment_", GpuProgramType.Fragment,
						@"Base\Shaders\ShadowCasterDefault_fs.sc", /*"main_fp", */fragmentDefines, out error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						Log.Fatal( error2 );

					var pass = new GpuMaterialPass( vertexProgram, fragmentProgram );
					data.passByLightType[ (int)lightType ] = pass;

					//!!!!
					//pass.CullingMode = CullingMode.None;
				}
			}
		}

		unsafe static bool Intersects( Plane[] frustumPlanes, Vector3* points, int pointCount )
		{
			foreach( var plane in frustumPlanes )
			{
				bool allClipped = true;
				for( int n = 0; n < pointCount; n++ )//foreach( var p in points )
				{
					if( plane.GetSide( ref points[ n ] ) == Plane.Side.Negative )
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
			public bool needCheck;
			public Vector3 position;
			public Sphere boundingSphere;
		}

		/////////////////////////////////////////

		struct LightAffectedObjectsItem
		{
			public Component_Scene.GetObjectsInSpaceItem item0;
			public Component_Scene.GetObjectsInSpaceItem item1;
			public Component_Scene.GetObjectsInSpaceItem item2;
			public Component_Scene.GetObjectsInSpaceItem item3;

			public Component_Scene.GetObjectsInSpaceItem GetItem( int index )
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

			public void SetItem( int index, Component_Scene.GetObjectsInSpaceItem value )
			{
				switch( index )
				{
				case 0: item0 = value; break;
				case 1: item1 = value; break;
				case 2: item2 = value; break;
				case 3: item3 = value; break;
				}
			}

			//public Component_Scene.GetObjectsInSpaceItem.ResultItem[] result0;
			//public Component_Scene.GetObjectsInSpaceItem.ResultItem[] result1;
			//public Component_Scene.GetObjectsInSpaceItem.ResultItem[] result2;
			//public Component_Scene.GetObjectsInSpaceItem.ResultItem[] result3;

			//public Component_Scene.GetObjectsInSpaceItem.ResultItem[] GetResult( int index )
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

			//public void SetResult( int index, Component_Scene.GetObjectsInSpaceItem.ResultItem[] value )
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
			OpenList<Component_Material.CompiledMaterialData> materialDatas;

			public OpenList<OutputItem> outputItems;

			////////////

			struct Item
			{
				public Vector2I renderableGroup;
				public int operationIndex;
				public RenderSceneData.MeshDataRenderOperation operation;
				public Component_Material.CompiledMaterialData materialData;
				public bool allowInstancing;
			}

			////////////

			public struct OutputItem
			{
				public RenderSceneData.MeshDataRenderOperation operation;
				public Component_Material.CompiledMaterialData materialData;
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
				materialDatas = new OpenList<Component_Material.CompiledMaterialData>( 32 * multiplier );
				outputItems = new OpenList<OutputItem>( 128 * multiplier );
			}

			public void Init( Component_RenderingPipeline pipeline )
			{
				//drop renderableItemArrays when InstancingMaxCount was changed
				if( instancingMaxCount != pipeline.InstancingMaxCount )
				{
					while( renderableItemArrays.Count != 0 )
						NativeUtility.Free( renderableItemArrays.Pop() );
				}

				instancingMaxCount = pipeline.InstancingMaxCount;
			}

			public unsafe void Add( Vector2I renderableGroup, int operationIndex, RenderSceneData.MeshDataRenderOperation operation, Component_Material.CompiledMaterialData materialData, bool allowInstancing )
			{
				if( materialData == null )
					Log.Fatal( "Component_RenderingPipeline_Basic: OutputInstancingManager: Add: materialData == null." );

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

		protected virtual void PrepareListsOfObjects( ViewportRenderingContext context, FrameData frameData )
		{
			Viewport viewportOwner = context.Owner;
			var renderSceneData = frameData.RenderSceneData;

			var scene = viewportOwner.AttachedScene;
			if( scene == null || !scene.EnabledInHierarchy )
				return;

			//find sky, fog. ambient, directional lights
			var ambientDirectionalLights = new List<Component_Light>();
			foreach( var c in scene.CachedObjectsInSpaceToFastFindByRenderingPipeline )//scene.GetComponents( false, true, true, delegate ( Component c )
			{
				if( c is Component_Sky sky && viewportOwner.CameraSettings.RenderSky )
				{
					if( frameData.Sky == null )
						frameData.Sky = sky;
				}
				else if( c is Component_Fog fog )
				{
					if( frameData.Fog == null )
						frameData.Fog = fog;
				}
				else if( c is Component_Light light )
				{
					if( light.VisibleInHierarchy )
					{
						var type = light.Type.Value;
						if( type == Component_Light.TypeEnum.Ambient || type == Component_Light.TypeEnum.Directional )
							ambientDirectionalLights.Add( light );
					}
				}
			}// );

			//Component_Scene.GetRenderSceneData
			frameData.SceneGetRenderSceneData( context, scene );

			{
				var cameraFrustum = viewportOwner.CameraSettings.Frustum;
				var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, cameraFrustum );

				//add ambient, directional lights
				foreach( var light in ambientDirectionalLights )
					frameData.RegisterObjectInSpace( context, light, GetRenderSceneDataMode.InsideFrustum, getObjectsItem );

				//get visible objects
				{
					//!!!!фильтры. еще что-то. может группы в octree юзать

					scene.GetObjectsInSpace( getObjectsItem );

					for( int nObject = 0; nObject < getObjectsItem.Result.Length; nObject++ )
					{
						var obj = getObjectsItem.Result[ nObject ].Object;
						if( obj.EnabledInHierarchy && obj.VisibleInHierarchy )
						{
							//if( ComponentsHidePublic.GetRenderSceneIndex( obj ) != -1 )
							//	Log.Fatal( "Component_RenderingPipeline_Render: PrepareListOfObjects: obj._internalRenderSceneIndex != -1." );

							var objIndex = frameData.RegisterObjectInSpace( context, obj, GetRenderSceneDataMode.InsideFrustum, getObjectsItem );
						}
					}
				}
			}

			//update ambient lights
			int[] disableLights = null;
			{
				var ambientLights = new List<int>();
				for( int lightIndex = 0; lightIndex < frameData.Lights.Count; lightIndex++ )
				{
					var lightItem = frameData.Lights[ lightIndex ];
					if( lightItem.data.Type == Component_Light.TypeEnum.Ambient )
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
			if( Shadows && ShadowIntensity > 0 && UseRenderTargets && DebugMode.Value == DebugModeEnum.None )
			{
				int[] remains = new int[ 4 ];
				remains[ (int)Component_Light.TypeEnum.Directional ] = ShadowDirectionalLightMaxCount;
				remains[ (int)Component_Light.TypeEnum.Point ] = ShadowPointLightMaxCount;
				remains[ (int)Component_Light.TypeEnum.Spotlight ] = ShadowSpotlightMaxCount;

				var cameraPosition = viewportOwner.CameraSettings.Position;
				double shadowFarDistance = ShadowFarDistance;

				//!!!!так распределять? просто по расстоянию от центра
				foreach( var lightIndex in frameData.LightsInFrustumSorted )
				{
					var item = frameData.Lights[ lightIndex ];

					if( item.data.CastShadows && item.data.ShadowIntensity > 0 && remains[ (int)item.data.Type ] > 0 )
					{
						bool skip = false;
						if( item.data.Type == Component_Light.TypeEnum.Point )
						{
							if( ( cameraPosition - item.data.Position ).LengthSquared() >
								( shadowFarDistance + item.data.AttenuationFar ) * ( shadowFarDistance + item.data.AttenuationFar ) )
								skip = true;
						}
						else if( item.data.Type == Component_Light.TypeEnum.Spotlight )
						{
							if( item.data.BoundingBox.GetPointDistance( cameraPosition ) > shadowFarDistance )
								skip = true;
						}

						//!!!!temp
						if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android ) skip = true;

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
				var lightAffectedObjectsGetObjectsList = new List<Component_Scene.GetObjectsInSpaceItem>( frameData.Lights.Count + 9 );
				{
					foreach( var lightIndex in frameData.LightsInFrustumSorted )
					{
						var lightItem = frameData.Lights[ lightIndex ];

						if( lightItem.data.Type == Component_Light.TypeEnum.Directional )
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

									var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, planes, bounds );

									item.SetItem( nIteration, getObjectsItem );
									lightAffectedObjectsGetObjectsList.Add( getObjectsItem );
								}

								lightAffectedObjects[ lightIndex ] = item;
							}
						}
						else if( lightItem.data.Type == Component_Light.TypeEnum.Point )
						{
							//Point light

							var sphere = new Sphere( lightItem.data.Position, lightItem.data.AttenuationFar );
							var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );

							var item = new LightAffectedObjectsItem();
							item.SetItem( 0, getObjectsItem );
							lightAffectedObjects[ lightIndex ] = item;
							lightAffectedObjectsGetObjectsList.Add( getObjectsItem );
						}
						else if( lightItem.data.Type == Component_Light.TypeEnum.Spotlight )
						{
							//Spotlight

							var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, lightItem.data.SpotlightClipPlanes, lightItem.data.BoundingBox );

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

					if( lightItem.data.Type == Component_Light.TypeEnum.Directional )
					{
						//Directional light

						//shadowsAffectedMeshesInSpace
						if( lightItem.prepareShadows )
						{
							for( int nIteration = 0; nIteration < ShadowDirectionalLightCascades; nIteration++ )
							{
								var getObjectsItem = lightAffectedObjects[ lightIndex ].GetItem( nIteration );
								//GetDirectionalLightShadowsCascadeHullPlanes( context, lightItem, nIteration, out var planes, out var bounds );
								//var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, planes, bounds );
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
								double shadowMapFarClipDistance = ShadowDirectionalLightExtrusionDistance * 2;
								//var lightExtrusionDistance = ShadowDirectionalLightExtrusionDistance.Value;

								//get objects data
								var objectsData = new ShadowsObjectData[ getObjectsItem.Result.Length ];
								for( int nObject = 0; nObject < objectsData.Length; nObject++ )
								{
									var obj = getObjectsItem.Result[ nObject ].Object;
									if( obj.VisibleInHierarchy )
									{
										var item = new ShadowsObjectData();
										item.needCheck = true;
										item.position = obj.TransformV.Position;
										item.boundingSphere = obj.SpaceBounds.CalculatedBoundingSphere;
										objectsData[ nObject ] = item;
									}
								}

								//get passed list
								var passedList = new int[ objectsData.Length ];
								//var passedList = new bool[ objectsData.Length ];
								//var spinLock = new SpinLock();

								unsafe void Calculate( int nObject )
								{
									ref var objectData = ref objectsData[ nObject ];
									if( objectData.needCheck )
									{
										//!!!!slowly

										ref var position = ref objectData.position;
										ref var boundingSphere = ref objectData.boundingSphere;
										var radius = (float)( ( boundingSphere.Origin - position ).Length() + boundingSphere.Radius );

										Vector3* points = stackalloc Vector3[ 8 ];//var points = new Vector3[ 8 ];
										{
											points[ 0 ] = position + lightRotationMatrix * new Vector3F( -radius, radius, radius );
											points[ 1 ] = position + lightRotationMatrix * new Vector3F( -radius, radius, -radius );
											points[ 2 ] = position + lightRotationMatrix * new Vector3F( -radius, -radius, -radius );
											points[ 3 ] = position + lightRotationMatrix * new Vector3F( -radius, -radius, radius );

											var offset = lightDirection * ( (float)shadowMapFarClipDistance + radius * 2 );
											//var offset = lightDirection * 30;
											for( int n = 0; n < 4; n++ )
												points[ n + 4 ] = points[ n ] + offset;
										}

										//!!!!
										var cascadeFrustumPlanes = cascadeFrustum.Planes;
										if( Intersects( cascadeFrustumPlanes, points, 8 ) )
										{
											passedList[ nObject ] = 1;

											//bool lockTaken = false;
											//spinLock.Enter( ref lockTaken );

											//passedList[ nObject ] = true;

											//if( lockTaken )
											//	spinLock.Exit();
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
									}

								}

								Parallel.For( 0, objectsData.Length, Calculate );
								//for( int nObject = 0; nObject < objectsData.Length; nObject++ )
								//	Calculate( nObject );

								//process passed objects
								for( int nObject = 0; nObject < passedList.Length; nObject++ )
								{
									if( passedList[ nObject ] != 0 )
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
					else if( lightItem.data.Type == Component_Light.TypeEnum.Point )
					{
						//Point light

						var getObjectsItem = lightAffectedObjects[ lightIndex ].GetItem( 0 );
						//var sphere = new Sphere( lightItem.data.Position, lightItem.data.AttenuationFar );
						//var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, sphere );
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
					else if( lightItem.data.Type == Component_Light.TypeEnum.Spotlight )
					{
						//Spotlight

						var getObjectsItem = lightAffectedObjects[ lightIndex ].GetItem( 0 );
						//var getObjectsItem = new Component_Scene.GetObjectsInSpaceItem( Component_Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, lightItem.data.SpotlightClipPlanes, lightItem.data.BoundingBox );
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

		static Component_Material GetMeshMaterial2( ref RenderSceneData.MeshItem meshItem, RenderSceneData.MeshDataRenderOperation operation, int operationIndex )
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
				var creator = operation.Creator as Component_MeshGeometry;
				if( creator != null )
				{
					var mesh = creator.ParentMesh;
					if( mesh != null )
						mesh.ShouldRecompile = true;
				}
			}

			return material;
		}

		public static Component_Material.CompiledMaterialData GetMeshMaterialData( ref RenderSceneData.MeshItem meshItem, RenderSceneData.MeshDataRenderOperation operation, int operationIndex, bool materialDataMustBePrepared )
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

		//static Component_Material.CompiledMaterialData GetMeshMaterialData( ref RenderSceneData.MeshItem meshItem, RenderSceneData.MeshDataRenderOperation operation, int operationIndex, bool materialDataMustBePrepared )
		//{
		//	Component_Material.CompiledMaterialData materialData = null;

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

		static Component_Material GetBillboardMaterial2( ref RenderSceneData.BillboardItem billboardItem )
		{
			return billboardItem.Material;
		}

		public static Component_Material.CompiledMaterialData GetBillboardMaterialData( ref RenderSceneData.BillboardItem billboardItem, bool materialDataMustBePrepared )
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

		static Component_Material GetDecalMaterial2( ref RenderSceneData.DecalItem decalItem )
		{
			return decalItem.Material;
		}

		static Component_Material.CompiledMaterialData GetDecalMaterialData( ref RenderSceneData.DecalItem decalItem, bool materialDataMustBePrepared )
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

		static Component_Material.CompiledMaterialData GetLayerMaterial2( ref RenderSceneData.LayerItem layerItem )
		{
			return layerItem.Material;
		}

		static Component_Material.CompiledMaterialData GetLayerMaterialData( ref RenderSceneData.LayerItem layerItem, bool materialDataMustBePrepared )
		{
			var materialData = GetLayerMaterial2( ref layerItem );//?.Result;

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

		unsafe void LightPrepareShadows( ViewportRenderingContext context, FrameData frameData, LightItem lightItem )
		{
			//init default shadow caster
			InitDefaultShadowCasterData();

			////bool atiHardwareShadows = false;
			////bool nvidiaHardwareShadows = false;
			////{
			////	if( RenderingSystem.Capabilities.Vendor == GPUVendor.ATI )
			////		atiHardwareShadows = true;
			////	if( RenderingSystem.Capabilities.Vendor == GPUVendor.NVidia )
			////		nvidiaHardwareShadows = true;
			////}
			////set texture formats
			////var textureFormatForDirectionalLight = PixelFormat.Float32R;
			////var textureFormatForSpotLight = PixelFormat.Float32R;
			////var textureFormatForPointLight = PixelFormat.Float32R;
			////if( nvidiaHardwareShadows )
			////{
			////	textureFormatForDirectionalLight = PixelFormat.Depth;
			////	textureFormatForSpotLight = PixelFormat.Depth;
			////}

			//temp
			////bias
			//Vec2 shadowLightBiasDirectionalLight = Vec2.Zero;
			//Vec2 shadowLightBiasPointLight = Vec2.Zero;
			//Vec2 shadowLightBiasSpotLight = Vec2.Zero;
			//{
			//	float qualityFactor;
			//	{
			//		//!!!!
			//		//if( needShadowTechnique == ShadowTechniques.ShadowmapHigh ||
			//		//	needShadowTechnique == ShadowTechniques.ShadowmapHighPSSM ||
			//		//	needShadowTechnique == ShadowTechniques.ShadowmapMedium ||
			//		//	needShadowTechnique == ShadowTechniques.ShadowmapMediumPSSM )
			//		//{
			//		qualityFactor = 1.5f;
			//		//}
			//		//else
			//		//{
			//		//	qualityFactor = 1;
			//		//}
			//	}

			//	//if( RenderSystem.Instance.IsDirect3D() )
			//	{
			//		//directional light
			//		{
			//			//NVIDIA: Depth24 texture format
			//			//ATI: Float32 texture format

			//			//float[] factors = null;

			//			//!!!!
			//			//factors = new float[] { 1.5f };
			//			//switch( needShadowTechnique )
			//			//{
			//			//case ShadowTechniques.ShadowmapLow:
			//			//	factors = new float[] { 1.0f };
			//			//	break;
			//			//case ShadowTechniques.ShadowmapMedium:
			//			//	factors = new float[] { 1.5f };
			//			//	break;
			//			//case ShadowTechniques.ShadowmapHigh:
			//			//	factors = new float[] { 1.5f };
			//			//	break;
			//			//case ShadowTechniques.ShadowmapLowPSSM:
			//			//	factors = new float[] { 1.0f, 1.0f, 1.0f };
			//			//	break;
			//			//case ShadowTechniques.ShadowmapMediumPSSM:
			//			//	factors = new float[] { 1.5f, 1.0f, 1.0f };
			//			//	break;
			//			//case ShadowTechniques.ShadowmapHighPSSM:
			//			//	factors = new float[] { 1.5f, 1.5f, 1.0f };
			//			//	break;
			//			//}

			//			//!!!!
			//			//Float32 texture format
			//			shadowLightBiasDirectionalLight = new Vec2( .0001f + .00005f * (float)1, 1.5f );

			//			//float iterationCount = pssm ? 3 : 1;
			//			//for( int index = 0; index < iterationCount; index++ )
			//			//{
			//			//	if( nvidiaHardwareShadows )
			//			//	{
			//			//		//Depth24 texture format
			//			//		shadowLightBiasDirectionalLight[ index ] =
			//			//			new Vec2( .0001f + .00005f * (float)index, factors[ index ] );
			//			//	}
			//			//	else
			//			//	{
			//			//		//Float32 texture format
			//			//		shadowLightBiasDirectionalLight[ index ] =
			//			//			new Vec2( .0001f + .00005f * (float)index, factors[ index ] );
			//			//	}
			//			//}
			//		}

			//		//point light
			//		{
			//			//!!!!

			//			//Float32 texture format (both for NVIDIA and ATI)
			//			shadowLightBiasPointLight = new Vec2( .05f * qualityFactor, 0 );
			//			//shadowLightBiasPointLight = new Vec2( .2f * qualityFactor, .5f * qualityFactor );
			//		}

			//		//spot light
			//		{
			//			//!!!!
			//			//if( nvidiaHardwareShadows )
			//			//{
			//			//	//Depth24 texture format
			//			//	float textureSize = Map.Instance.GetShadowSpotLightTextureSizeAsInteger();
			//			//	float textureSizeFactor = 1024.0f / textureSize;
			//			//	shadowLightBiasSpotLight =
			//			//		new Vec2( .001f * qualityFactor * textureSizeFactor, .001f * qualityFactor );
			//			//}
			//			//else
			//			//{
			//			//Float32 texture format
			//			shadowLightBiasSpotLight = new Vec2( .1f * qualityFactor, 1.0f * qualityFactor );

			//			//}
			//		}
			//	}
			//	//else
			//	//{
			//	//	shadowLightBiasDirectionalLight[ 0 ] = new Vec2( .0003f * qualityFactor, 0 );
			//	//	shadowLightBiasPointLight = new Vec2( .15f * qualityFactor, 0 );
			//	//	shadowLightBiasSpotLight = new Vec2( .15f * qualityFactor, 0 );
			//	//}
			//}

			var lightData = lightItem.data;

			/*
			//shadow bias
			Vector2F shadowBias = Vector2F.Zero;
			{
				float qualityFactor = 1.5f;//1.0f

				//!!!!

				switch( lightData.Type )
				{
				case Component_Light.TypeEnum.Directional:
					shadowBias = new Vector2F( .00015f, qualityFactor );
					break;
				case Component_Light.TypeEnum.Point:
					shadowBias = new Vector2F( .05f * qualityFactor, 0 );
					break;
				case Component_Light.TypeEnum.Spotlight:
					shadowBias = new Vector2F( .1f * qualityFactor, 1.0f * qualityFactor );
					break;
				}

				//!!!!qq
				shadowBias.X *= lightData.ShadowBias * 2;
				//shadowBias.Y *= lightData.ShadowBias;
				shadowBias.Y *= lightData.ShadowNormalBias * 2;
			}
			*/

			//texture size
			int textureSize;
			{
				ShadowTextureSize textureSizeEnum;
				if( lightData.Type == Component_Light.TypeEnum.Spotlight )
					textureSizeEnum = ShadowSpotlightTextureSize.Value;
				else if( lightData.Type == Component_Light.TypeEnum.Point )
					textureSizeEnum = ShadowPointLightTextureSize.Value;
				else
					textureSizeEnum = ShadowDirectionalLightTextureSize.Value;
				textureSize = int.Parse( textureSizeEnum.ToString().Replace( "_", "" ) );
			}

			//texture format
			var textureFormat = PixelFormat.Float32R;

			//create render target
			Component_Image shadowTexture = null;
			if( lightData.Type == Component_Light.TypeEnum.Point )
				shadowTexture = context.RenderTargetCube_Alloc( new Vector2I( textureSize, textureSize ), textureFormat );
			else if( lightData.Type == Component_Light.TypeEnum.Spotlight )
				shadowTexture = context.RenderTarget2D_Alloc( new Vector2I( textureSize, textureSize ), textureFormat );
			else if( lightData.Type == Component_Light.TypeEnum.Directional )
				shadowTexture = context.RenderTarget2D_Alloc( new Vector2I( textureSize, textureSize ), textureFormat, arrayLayers: ShadowDirectionalLightCascades );

			//add to namedTextures
			{
				var prefix = "shadow" + lightData.Type.ToString();
				for( int counter = 1; ; counter++ )
				{
					var name = prefix + counter.ToString();
					if( !context.objectsDuringUpdate.namedTextures.ContainsKey( name ) )
					{
						context.objectsDuringUpdate.namedTextures[ name ] = shadowTexture;
						break;
					}
				}
			}

			//create depth texture
			var shadowTextureDepth = context.RenderTarget2D_Alloc( new Vector2I( textureSize, textureSize ), PixelFormat.Depth24S8 );

			lightItem.shadowTexture = shadowTexture;

			if( lightItem.data.Type == Component_Light.TypeEnum.Directional )
				lightItem.shadowCascadesProjectionViewMatrices = new (Matrix4F, Matrix4F)[ ShadowDirectionalLightCascades ];

			int iterationCount = 1;
			if( lightData.Type == Component_Light.TypeEnum.Point )
				iterationCount = 6;
			if( lightData.Type == Component_Light.TypeEnum.Directional )
				iterationCount = ShadowDirectionalLightCascades;

			for( int nIteration = 0; nIteration < iterationCount; nIteration++ )
			{
				Viewport shadowViewport = shadowTexture.Result.GetRenderTarget( 0, nIteration ).Viewports[ 0 ];

				//camera settings
				if( lightData.Type == Component_Light.TypeEnum.Spotlight )
				{
					//spotlight
					Degree fov = lightData.SpotlightOuterAngle * 1.05;
					if( fov > 179 )
						fov = 179;
					Vector3 dir = lightData.Rotation.GetForward();
					Vector3 up = lightData.Rotation.GetUp();

					shadowViewport.CameraSettings = new Viewport.CameraSettingsClass( shadowViewport, 1, fov, context.Owner.CameraSettings.NearClipDistance, lightData.AttenuationFar * 1.05, lightData.Position, dir, up, ProjectionType.Perspective, 1, 0, 0 );
				}
				else if( lightData.Type == Component_Light.TypeEnum.Point )
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

					//!!!!ToVec3F()
					Vector3F cameraPosition = shadowViewport.CameraSettings.Position.ToVector3F();
					container.Set( "u_cameraPosition", ParameterType.Vector3, 1, &cameraPosition, sizeof( Vector3F ) );

					float farClipDistance = (float)shadowViewport.CameraSettings.FarClipDistance;
					container.Set( "u_farClipDistance", ParameterType.Float, 1, &farClipDistance, sizeof( float ) );

					//generalContainer.Set( "u_shadowBias", ParameterType.Vector2, 1, &shadowBias, sizeof( Vector2F ) );
					//Vec2F bias;
					//if( lightItem.data.type == Component_Light.TypeEnum.Spotlight )
					//	bias = shadowLightBiasSpotLight.ToVec2F();
					//else if( lightItem.data.type == Component_Light.TypeEnum.Point )
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
						if( lightItem.data.Type == Component_Light.TypeEnum.Directional )
						{
							//!!!!new
							lightPosition = context.Owner.CameraSettings.Position - lightItem.data.Rotation.ToQuaternion().GetForward() * 10000.0;
							//lightPosition = -lightItem.data.Rotation.ToQuaternion().GetForward() * 1000000.0;
						}
						else
							lightPosition = lightItem.data.Position;

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

					for( int nSector = 0; nSector < instancingSectorCount; nSector++ )
					{
						int indexFrom = (int)( (float)shadowsAffectedRenderableGroups.Length * nSector / instancingSectorCount );
						int indexTo = (int)( (float)shadowsAffectedRenderableGroups.Length * ( nSector + 1 ) / instancingSectorCount );
						if( nSector == instancingSectorCount - 1 )
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
									var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

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
								Component_Material.CompiledMaterialData materialBinded = null;

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
									var pass = shadowCasterData.passByLightType[ (int)lightData.Type ];

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

										//bind operation data
										var firstRenderableItem = outputItem.renderableItemFirst;
										if( firstRenderableItem.X == 0 )
										{
											//meshes
											ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ firstRenderableItem.Y ];
											var meshData = meshItem.MeshData;

											BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance );
										}
										else if( firstRenderableItem.X == 1 )
										{
											//billboards
											ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ firstRenderableItem.Y ];
											var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

											BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance );
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

												BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, batchInstancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance );

												if( !batchInstancing )
													fixed( Matrix4F* p = &meshItem.Transform )
														Bgfx.SetTransform( (float*)p );

												RenderOperation( context, outputItem.operation, pass, null, meshItem.CutVolumes, meshItem.BatchingInstanceBuffer );
											}
											else if( renderableItem.X == 1 )
											{
												//billboards
												ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
												var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

												BindRenderOperationData( context, frameData, specialShadowCasterData != null ? materialData : null, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance );

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




					//Component_Material.CompiledMaterialData materialBinded = null;

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

					//			var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

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

			context.DynamicTexture_Free( shadowTextureDepth );
		}

		public unsafe virtual void RenderSky( ViewportRenderingContext context, FrameData frameData )
		{
			frameData.Sky?.Render( this, context, frameData );
		}

		public unsafe virtual void Render3DSceneDeferred( ViewportRenderingContext context, FrameData frameData )
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


			for( int nSector = 0; nSector < instancingSectorCount; nSector++ )
			{
				int indexFrom = (int)( (float)renderableGroupsToDraw.Length * nSector / instancingSectorCount );
				int indexTo = (int)( (float)renderableGroupsToDraw.Length * ( nSector + 1 ) / instancingSectorCount );
				if( nSector == instancingSectorCount - 1 )
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
							var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

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
						Component_Material.CompiledMaterialData materialBinded = null;

						var outputItems = outputInstancingManager.outputItems;
						for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
						{
							ref var outputItem = ref outputItems.Data[ nOutputItem ];
							var materialData = outputItem.materialData;

							//material
							var pass = materialData.deferredShadingPass;
							bool materialWasChanged = materialBinded != materialData;
							materialBinded = materialData;

							if( Instancing && outputItem.renderableItemsCount >= InstancingMinCount )
							{
								//with instancing

								//bind material data
								materialData.BindCurrentFrameData( context, false, materialWasChanged );

								//bind operation data
								var firstRenderableItem = outputItem.renderableItemFirst;
								if( firstRenderableItem.X == 0 )
								{
									//meshes
									ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ firstRenderableItem.Y ];
									var meshData = meshItem.MeshData;

									BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance );
								}
								else if( firstRenderableItem.X == 1 )
								{
									//billboards
									ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ firstRenderableItem.Y ];
									var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

									BindRenderOperationData( context, frameData, materialData, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance );
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

										BindRenderOperationData( context, frameData, materialData, batchInstancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance );

										if( !batchInstancing )
											fixed( Matrix4F* p = &meshItem.Transform )
												Bgfx.SetTransform( (float*)p );

										RenderOperation( context, outputItem.operation, pass, null, meshItem.CutVolumes, meshItem.BatchingInstanceBuffer );
									}
									else if( renderableItem.X == 1 )
									{
										//billboards
										ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
										var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

										BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance );

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

				//render layers
				if( DebugDrawLayers )
				{
					Component_Material.CompiledMaterialData materialBinded = null;

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

									if( materialData.deferredShadingPass != null )
									{
										bool materialWasChanged = materialBinded != materialData;
										materialBinded = materialData;

										//bind material data
										materialData.BindCurrentFrameData( context, false, materialWasChanged );
										materialData.BindCurrentFrameData_MaskTextures( context, layer.Mask );

										if( !batchInstancing )
											fixed( Matrix4F* p = &meshItem.Transform )
												Bgfx.SetTransform( (float*)p );

										var color = meshItem.Color * layer.Color;

										for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
										{
											var oper = meshData.RenderOperations[ nOperation ];

											BindRenderOperationData( context, frameData, materialData, batchInstancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance );

											RenderOperation( context, oper, materialData.deferredShadingPass, null, meshItem.CutVolumes, meshItem.BatchingInstanceBuffer );
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
			//	Component_Material.CompiledMaterialData materialBinded = null;

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
			//					//var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

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
			//					var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

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

			//	Component_Material.CompiledMaterialData materialBinded = null;

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

			//			var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

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
			decalMesh = ComponentUtility.CreateComponent<Component_Mesh>( null, true, false );
			decalMesh.CreateComponent<Component_MeshGeometry_Box>();
			decalMesh.Enabled = true;
		}

		public unsafe virtual void RenderDecalsDeferred( ViewportRenderingContext context, FrameData frameData, Component_Image depthTexture, Component_Image gBuffer1TextureCopy, Component_Image gBuffer4TextureCopy, Component_Image gBuffer5TextureCopy )
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

			Component_Material.CompiledMaterialData materialBinded = null;
			var currentDecalNormalTangent0 = new Vector4F( float.MaxValue, 0, 0, 0 );
			var currentDecalNormalTangent1 = new Vector4F( float.MaxValue, 0, 0, 0 );

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

						BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, false/*receiveAnotherDecals*/, ref zeroVector, 0, oper.UnwrappedUV, ref decalData.Color, oper.VertexStructureContainsColor, false, decalData.VisibilityDistance );

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
							if( decalData.NormalsMode == Component_Decal.NormalsModeEnum.VectorOfDecal )
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

						context.BindTexture( new ViewportRenderingContext.BindTextureData( 2/* "depthTexture"*/, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
						context.BindTexture( new ViewportRenderingContext.BindTextureData( 3/* "gBuffer1TextureCopy"*/, gBuffer1TextureCopy, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
						context.BindTexture( new ViewportRenderingContext.BindTextureData( 4/* "gBuffer4TextureCopy"*/, gBuffer4TextureCopy, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );
						context.BindTexture( new ViewportRenderingContext.BindTextureData( 5/* "gBuffer5TextureCopy"*/, gBuffer5TextureCopy, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

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
					generalDefines.Add( ("LIGHT_TYPE_" + Component_Light.TypeEnum.Ambient.ToString().ToUpper(), "") );
					vertexDefines.AddRange( generalDefines );
					fragmentDefines.AddRange( generalDefines );
				}

				string error2;

				//vertex program
				var vertexProgram = GpuProgramManager.GetProgram( "DeferredEnvironmentLight_",
					GpuProgramType.Vertex, @"Base\Shaders\DeferredEnvironmentLight_vs.sc", vertexDefines, out error2 );
				if( !string.IsNullOrEmpty( error2 ) )
				{
					Log.Fatal( error2 );
					return;
				}

				var fragmentProgram = GpuProgramManager.GetProgram( "DeferredEnvironmentLight_",
					GpuProgramType.Fragment, @"Base\Shaders\DeferredEnvironmentLight_fs.sc", fragmentDefines, out error2 );
				if( !string.IsNullOrEmpty( error2 ) )
				{
					//!!!!
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
			{
				bool shadows = nShadows != 0;

				for( int nLight = 1; nLight < 4; nLight++ )//for( int nLight = 0; nLight < 4; nLight++ )
				{
					var lightType = (Component_Light.TypeEnum)nLight;
					//if( shadows && lightType == Component_Light.TypeEnum.Ambient )
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
							generalDefines.Add( ("SHADOW_MAP", "") );

							if( nShadows == 2 )
								generalDefines.Add( ("SHADOW_MAP_HIGH", "") );
							else
								generalDefines.Add( ("SHADOW_MAP_LOW", "") );
						}

						vertexDefines.AddRange( generalDefines );
						fragmentDefines.AddRange( generalDefines );
					}

					string error2;

					//vertex program
					GpuProgram vertexProgram = GpuProgramManager.GetProgram( "DeferredDirectLight_",
						GpuProgramType.Vertex, @"Base\Shaders\DeferredDirectLight_vs.sc", vertexDefines, out error2 );
					if( !string.IsNullOrEmpty( error2 ) )
					{
						//!!!!
						Log.Fatal( error2 );
						return;
					}

					//fragment program
					GpuProgram fragmentProgram = GpuProgramManager.GetProgram( "DeferredDirectLight_",
						GpuProgramType.Fragment, @"Base\Shaders\DeferredDirectLight_fs.sc", fragmentDefines, out error2 );
					if( !string.IsNullOrEmpty( error2 ) )
					{
						//!!!!
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
						deferredShadingData.passesPerLightWithShadowsHigh[ nLight ] = passItem;
					if( nShadows == 1 )
						deferredShadingData.passesPerLightWithShadowsLow[ nLight ] = passItem;
					else
						deferredShadingData.passesPerLightWithoutShadows[ nLight ] = passItem;
					//if( !shadows )
					//	deferredShadingData.passesPerLightWithoutShadows[ nLight ] = passItem;
					//else
					//	deferredShadingData.passesPerLightWithShadows[ nLight ] = passItem;
				}
			}
		}

		unsafe void RenderEnvironmentLightDeferred( ViewportRenderingContext context, FrameData frameData, Component_Image sceneTexture, Component_Image normalTexture, Component_Image gBuffer2Texture, Component_Image gBuffer3Texture, Component_Image gBuffer4Texture, Component_Image gBuffer5Texture, Component_Image depthTexture )
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
			if( u_environmentTextureRotation == null )
				u_environmentTextureRotation = GpuProgramManager.RegisterUniform( "u_environmentTextureRotation", UniformType.Matrix3x3, 1 );
			if( u_environmentTextureIBLRotation == null )
				u_environmentTextureIBLRotation = GpuProgramManager.RegisterUniform( "u_environmentTextureIBLRotation", UniformType.Matrix3x3, 1 );
			if( u_environmentTextureMultiplierAndAffect == null )
				u_environmentTextureMultiplierAndAffect = GpuProgramManager.RegisterUniform( "u_environmentTextureMultiplierAndAffect", UniformType.Vector4, 1 );
			if( u_environmentTextureIBLMultiplierAndAffect == null )
				u_environmentTextureIBLMultiplierAndAffect = GpuProgramManager.RegisterUniform( "u_environmentTextureIBLMultiplierAndAffect", UniformType.Vector4, 1 );

			GetBackgroundEnvironmentTextures( context, frameData, out var ambientLightTexture, out var ambientLightTextureIBL );

			//ambient light
			{
				//set u_reflectionProbeData
				var reflectionProbeData = new Vector4F( 0, 0, 0, 0 );
				Bgfx.SetUniform( u_reflectionProbeData.Value, &reflectionProbeData, 1 );

				var container = new ParameterContainer();

				//bind env textures, parameters
				{
					container.Set( new ViewportRenderingContext.BindTextureData( 6/*"s_environmentTexture"*/,
						ambientLightTexture.Value.texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					container.Set( new ViewportRenderingContext.BindTextureData( 7/*"environmentTextureIBL"*/,
						ambientLightTextureIBL.Value.texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					var environmentTextureRotation = ambientLightTexture.Value.rotation;
					var environmentTextureIBLRotation = ambientLightTextureIBL.Value.rotation;
					var environmentTextureMultiplierAndAffect = ambientLightTexture.Value.multiplierAndAffect;
					var environmentTextureIBLMultiplierAndAffect = ambientLightTextureIBL.Value.multiplierAndAffect;
					Bgfx.SetUniform( u_environmentTextureRotation.Value, &environmentTextureRotation, 1 );
					Bgfx.SetUniform( u_environmentTextureIBLRotation.Value, &environmentTextureIBLRotation, 1 );
					Bgfx.SetUniform( u_environmentTextureMultiplierAndAffect.Value, &environmentTextureMultiplierAndAffect, 1 );
					Bgfx.SetUniform( u_environmentTextureIBLMultiplierAndAffect.Value, &environmentTextureIBLMultiplierAndAffect, 1 );
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
				var reflectionProbeData = new Vector4F( item.data.Sphere.Origin.ToVector3F(), (float)item.data.Sphere.Radius );
				Bgfx.SetUniform( u_reflectionProbeData.Value, &reflectionProbeData, 1 );
				//reflectionProbeItem.Bind( this, context );

				var container = new ParameterContainer();

				//bind env textures, parameters
				{
					//use reflection from the reflection probe
					var texture = item.data.CubemapEnvironment;
					if( texture == null )
						texture = ResourceUtility.GrayTextureCube;
					container.Set( new ViewportRenderingContext.BindTextureData( 6/*"s_environmentTexture"*/,
						texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					//use lighting from the ambient light
					container.Set( new ViewportRenderingContext.BindTextureData( 7/*"environmentTextureIBL"*/,
						ambientLightTextureIBL.Value.texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

					var environmentTextureRotation = item.data.Rotation;
					var environmentTextureIBLRotation = ambientLightTextureIBL.Value.rotation;
					var environmentTextureMultiplierAndAffect = new Vector4F( item.data.Multiplier, 1 );
					var environmentTextureIBLMultiplierAndAffect = ambientLightTextureIBL.Value.multiplierAndAffect;
					Bgfx.SetUniform( u_environmentTextureRotation.Value, &environmentTextureRotation, 1 );
					Bgfx.SetUniform( u_environmentTextureIBLRotation.Value, &environmentTextureIBLRotation, 1 );
					Bgfx.SetUniform( u_environmentTextureMultiplierAndAffect.Value, &environmentTextureMultiplierAndAffect, 1 );
					Bgfx.SetUniform( u_environmentTextureIBLMultiplierAndAffect.Value, &environmentTextureIBLMultiplierAndAffect, 1 );
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

		unsafe void RenderDirectLightDeferred( ViewportRenderingContext context, FrameData frameData, Component_Image sceneTexture, Component_Image normalTexture, Component_Image gBuffer2Texture, Component_Image gBuffer3Texture, Component_Image gBuffer4Texture, Component_Image gBuffer5Texture, Component_Image depthTexture, LightItem lightItem )
		{
			Viewport viewportOwner = context.Owner;

			bool skip = false;

			List<CanvasRenderer.TriangleVertex> screenTriangles = null;

			//Point, Spotlight
			if( lightItem.data.Type == Component_Light.TypeEnum.Point || lightItem.data.Type == Component_Light.TypeEnum.Spotlight )
			{
				Vector3[] positions;
				int[] indices;

				if( lightItem.data.Type == Component_Light.TypeEnum.Point )
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

				var screenPositions = new Vector2F[ positions.Length ];
				for( int n = 0; n < screenPositions.Length; n++ )
				{
					context.Owner.CameraSettings.ProjectToScreenCoordinates( positions[ n ], out var screen, false );
					screenPositions[ n ] = screen.ToVector2F();
					//if( context.Owner.CameraSettings.ProjectToScreenCoordinates( positions[ n ], out var screen ) )
					//	screenPositions[ n ] = screen.ToVec2F();
					//else
					//	screenPositions[ n ] = new Vec2F( float.NaN, float.NaN );
				}

				screenTriangles = new List<CanvasRenderer.TriangleVertex>( indices.Length / 2 );

				for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
				{
					var p0 = screenPositions[ indices[ nTriangle * 3 + 0 ] ];
					var p1 = screenPositions[ indices[ nTriangle * 3 + 1 ] ];
					var p2 = screenPositions[ indices[ nTriangle * 3 + 2 ] ];

					if( !float.IsNaN( p0.X ) && !float.IsNaN( p1.X ) && !float.IsNaN( p2.X ) )
					{
						if( ( ( p1.Y - p0.Y ) * ( p2.X - p1.X ) - ( p2.Y - p1.Y ) * ( p1.X - p0.X ) ) < 0 )
						{
							if( !( p0.X < 0 && p1.X < 0 && p2.X < 0 ) && !( p0.X > 1 && p1.X > 1 && p2.X > 1 ) )
							{
								if( !( p0.Y < 0 && p1.Y < 0 && p2.Y < 0 ) && !( p0.Y > 1 && p1.Y > 1 && p2.Y > 1 ) )
								{
									//context.Owner.CanvasRenderer.AddLine( p0, p1, ColorValue.One );
									//context.Owner.CanvasRenderer.AddLine( p1, p2, ColorValue.One );
									//context.Owner.CanvasRenderer.AddLine( p2, p0, ColorValue.One );

									screenTriangles.Add( new CanvasRenderer.TriangleVertex( p0, ColorValue.One, p0 ) );
									screenTriangles.Add( new CanvasRenderer.TriangleVertex( p1, ColorValue.One, p1 ) );
									screenTriangles.Add( new CanvasRenderer.TriangleVertex( p2, ColorValue.One, p2 ) );
								}
							}
						}
					}
				}

				//check to use fullscreen quad and skip light
				if( screenTriangles.Count != 0 )
				{
					var plane = viewportOwner.CameraSettings.Frustum.Planes[ 0 ];

					int backsideCount = 0;
					foreach( var pos in positions )
					{
						if( plane.GetSide( pos ) == Plane.Side.Positive )
							backsideCount++;
					}

					//all backside
					if( backsideCount == positions.Length )
						skip = true;
					//use fullscreen quad when exists points backward camera
					if( backsideCount != 0 )
						screenTriangles = null;
				}

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
				DeferredShadingData.PassItem passItem = null;
				{
					if( lightItem.prepareShadows )
					{
						if( ShadowQuality.Value == ShadowQualityEnum.High )
							passItem = deferredShadingData.passesPerLightWithShadowsHigh[ (int)lightItem.data.Type ];
						else
							passItem = deferredShadingData.passesPerLightWithShadowsLow[ (int)lightItem.data.Type ];
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
						if( lightItem.data.Type != Component_Light.TypeEnum.Ambient )
						{
							if( lightItem.data.Type == Component_Light.TypeEnum.Point )
							{
								var texture = lightItem.data.Mask;
								if( texture == null || texture.Result.TextureType != Component_Image.TypeEnum.Cube )
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
								if( texture == null || texture.Result.TextureType != Component_Image.TypeEnum._2D )
									texture = ResourceUtility.WhiteTexture2D;

								var clamp = lightItem.data.Type == Component_Light.TypeEnum.Spotlight;

								generalContainer.Set( new ViewportRenderingContext.BindTextureData( 4/*"s_lightMask"*/, texture,
									clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
							}
						}

						//if( lightItem.prepareShadows )
						{
							//Viewport shadowViewport = lightItem.shadowTexture.Result.GetRenderTarget().Viewports[ 0 ];

							//shadowMap
							{
								Component_Image texture;
								if( lightItem.prepareShadows )
								{
									texture = lightItem.shadowTexture;
								}
								else
								{
									if( lightItem.data.Type == Component_Light.TypeEnum.Point )
										texture = nullShadowTextureCube;
									else
										texture = nullShadowTexture2D;
								}

								var textureValue = new ViewportRenderingContext.BindTextureData( 5/*"s_shadowMap"*/,
									texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None );
								textureValue.AdditionFlags |= TextureFlags.CompareLessEqual;
								generalContainer.Set( ref textureValue );
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

						if( screenTriangles != null )
							context.RenderTrianglesToCurrentViewport( shader, screenTriangles, CanvasRenderer.BlendingType.Add );
						else
							context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
					}
				}
			}
		}

		public unsafe virtual void RenderLightsDeferred( ViewportRenderingContext context, FrameData frameData, Component_Image sceneTexture, Component_Image normalTexture, Component_Image gBuffer2Texture, Component_Image gBuffer3Texture, Component_Image gBuffer4Texture, Component_Image gBuffer5Texture, Component_Image depthTexture )
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

				if( lightItem.data.Type == Component_Light.TypeEnum.Ambient )
					RenderEnvironmentLightDeferred( context, frameData, sceneTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, depthTexture );
				else
					RenderDirectLightDeferred( context, frameData, sceneTexture, normalTexture, gBuffer2Texture, gBuffer3Texture, gBuffer4Texture, gBuffer5Texture, depthTexture, lightItem );
			}
		}

		public unsafe void ForwardBindGeneralTexturesUniforms( ViewportRenderingContext context, FrameData frameData, ref Sphere objectSphere, LightItem lightItem, bool receiveShadows, bool setUniforms )
		{
			GetEnvironmentTexturesByBoundingSphereIntersection( context, frameData, ref objectSphere/*meshItem.BoundingSphere*/, out var texture1, out var textureIBL1, out var texture2, out var textureIBL2, out var environmentBlendingFactor );

			//environment map
			{
				context.BindTexture( new ViewportRenderingContext.BindTextureData( 2/*"environmentTexture1"*/, texture1.Value.texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				context.BindTexture( new ViewportRenderingContext.BindTextureData( 3/*"environmentTextureIBL1"*/, textureIBL1.Value.texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				context.BindTexture( new ViewportRenderingContext.BindTextureData( 4/*"environmentTexture2"*/, texture2.Value.texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				context.BindTexture( new ViewportRenderingContext.BindTextureData( 5/*"environmentTextureIBL2"*/, textureIBL2.Value.texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				if( setUniforms )
				{
					if( u_environmentTexture1Rotation == null )
						u_environmentTexture1Rotation = GpuProgramManager.RegisterUniform( "u_environmentTexture1Rotation", UniformType.Matrix3x3, 1 );
					if( u_environmentTexture2Rotation == null )
						u_environmentTexture2Rotation = GpuProgramManager.RegisterUniform( "u_environmentTexture2Rotation", UniformType.Matrix3x3, 1 );
					if( u_environmentTexture1MultiplierAndAffect == null )
						u_environmentTexture1MultiplierAndAffect = GpuProgramManager.RegisterUniform( "u_environmentTexture1MultiplierAndAffect", UniformType.Vector4, 1 );
					if( u_environmentTexture2MultiplierAndAffect == null )
						u_environmentTexture2MultiplierAndAffect = GpuProgramManager.RegisterUniform( "u_environmentTexture2MultiplierAndAffect", UniformType.Vector4, 1 );

					var rotation1 = texture1.Value.rotation;
					var rotation2 = texture2.Value.rotation;
					var multiplier1 = texture1.Value.multiplierAndAffect;
					var multiplier2 = texture2.Value.multiplierAndAffect;
					Bgfx.SetUniform( u_environmentTexture1Rotation.Value, &rotation1, 1 );
					Bgfx.SetUniform( u_environmentTexture2Rotation.Value, &rotation2, 1 );
					Bgfx.SetUniform( u_environmentTexture1MultiplierAndAffect.Value, &multiplier1, 1 );
					Bgfx.SetUniform( u_environmentTexture2MultiplierAndAffect.Value, &multiplier2, 1 );
				}
			}

			//s_brdfLUT
			context.BindTexture( new ViewportRenderingContext.BindTextureData( 6/*"s_brdfLUT"*/, BrdfLUT, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

			//light mask
			if( lightItem.data.Type != Component_Light.TypeEnum.Ambient )
			{
				if( lightItem.data.Type == Component_Light.TypeEnum.Point )
				{
					var texture = lightItem.data.Mask;
					if( texture == null || texture.Result.TextureType != Component_Image.TypeEnum.Cube )
						texture = ResourceUtility.WhiteTextureCube;

					//!!!!anisotropic? где еще

					context.BindTexture( new ViewportRenderingContext.BindTextureData( 7/*"s_lightMask"*/,
						texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
				}
				else
				{
					var texture = lightItem.data.Mask;
					if( texture == null || texture.Result.TextureType != Component_Image.TypeEnum._2D )
						texture = ResourceUtility.WhiteTexture2D;

					var clamp = lightItem.data.Type == Component_Light.TypeEnum.Spotlight;
					context.BindTexture( new ViewportRenderingContext.BindTextureData( 7/*"s_lightMask"*/,
						texture, clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap,
						FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
				}
			}

			//receive shadows
			//if( receiveShadows )
			{
				//Viewport shadowViewport = lightItem.shadowTexture.Result.GetRenderTarget().Viewports[ 0 ];

				//shadowMap
				{
					Component_Image texture;
					if( receiveShadows )
					{
						texture = lightItem.shadowTexture;
					}
					else
					{
						if( lightItem.data.Type == Component_Light.TypeEnum.Point )
							texture = nullShadowTextureCube;
						else
							texture = nullShadowTexture2D;
					}

					var textureValue = new ViewportRenderingContext.BindTextureData( 8/*"g_shadowMap"*/,
						texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear,
						FilterOption.None );
					textureValue.AdditionFlags |= TextureFlags.CompareLessEqual;
					context.BindTexture( ref textureValue );
				}

				//Vector2F shadowBias = new Vector2F( lightItem.data.ShadowBias, lightItem.data.ShadowNormalBias );
				//generalContainer.Set( "u_shadowBias", ParameterType.Vector2, 1, &shadowBias, sizeof( Vector2F ) );
			}

			//set u_environmentBlendingFactor
			if( setUniforms )
			{
				if( u_environmentBlendingFactor == null )
					u_environmentBlendingFactor = GpuProgramManager.RegisterUniform( "u_environmentBlendingFactor", UniformType.Vector4, 1 );

				var value = new Vector4F( environmentBlendingFactor, 0, 0, 0 );
				Bgfx.SetUniform( u_environmentBlendingFactor.Value, &value, 1 );
			}
		}

		public unsafe virtual void Render3DSceneForwardOpaque( ViewportRenderingContext context, FrameData frameData )
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


			LightItem lightItemBinded = null;

			for( int nLightsInFrustumSorted = 0; nLightsInFrustumSorted < frameData.LightsInFrustumSorted.Length; nLightsInFrustumSorted++ )
			{
				var lightIndex = frameData.LightsInFrustumSorted[ nLightsInFrustumSorted ];
				var lightItem = frameData.Lights[ lightIndex ];

				int sectorCount = nLightsInFrustumSorted == 0 ? instancingSectorCount : 1;
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
								if( lightType == Component_Light.TypeEnum.Point || lightType == Component_Light.TypeEnum.Spotlight )
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
								if( lightType == Component_Light.TypeEnum.Point || lightType == Component_Light.TypeEnum.Spotlight )
									affectedByLight = billboardItem2.ContainsPointOrSpotLight( lightIndex );
								else
									affectedByLight = true;

								if( affectedByLight )
								{
									ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];
									var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

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
							Component_Material.CompiledMaterialData materialBinded = null;

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
								bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadowsLow != null;

								GpuMaterialPass pass;
								if( receiveShadows )
								{
									if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
										pass = passesGroup.passWithShadowsHigh;
									else
										pass = passesGroup.passWithShadowsLow;
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

										ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true );

										BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance );
									}
									else if( firstRenderableItem.X == 1 )
									{
										//billboards
										ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ firstRenderableItem.Y ];
										var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

										ForwardBindGeneralTexturesUniforms( context, frameData, ref billboardItem.BoundingSphere, lightItem, receiveShadows, true );

										BindRenderOperationData( context, frameData, materialData, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance );
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

											ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, nRenderableItem == 0 );

											var batchInstancing = meshItem.BatchingInstanceBuffer != null;

											BindRenderOperationData( context, frameData, materialData, batchInstancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance );

											if( !batchInstancing )
												fixed( Matrix4F* p = &meshItem.Transform )
													Bgfx.SetTransform( (float*)p );

											RenderOperation( context, outputItem.operation, pass, null, meshItem.CutVolumes, meshItem.BatchingInstanceBuffer );
										}
										else if( renderableItem.X == 1 )
										{
											//billboards
											ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
											var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

											ForwardBindGeneralTexturesUniforms( context, frameData, ref billboardItem.BoundingSphere, lightItem, receiveShadows, nRenderableItem == 0 );

											BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance );

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

					int sectorCount = nLightsInFrustumSorted == 0 ? instancingSectorCount : 1;
					for( int nSector = 0; nSector < sectorCount; nSector++ )
					{
						int indexFrom = (int)( (float)renderableGroupsToDraw.Length * nSector / sectorCount );
						int indexTo = (int)( (float)renderableGroupsToDraw.Length * ( nSector + 1 ) / sectorCount );
						if( nSector == sectorCount - 1 )
							indexTo = renderableGroupsToDraw.Length;


						Component_Material.CompiledMaterialData materialBinded = null;

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
									if( lightType == Component_Light.TypeEnum.Point || lightType == Component_Light.TypeEnum.Spotlight )
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
													bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadowsLow != null;

													GpuMaterialPass pass;
													if( receiveShadows )
													{
														if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
															pass = passesGroup.passWithShadowsHigh;
														else
															pass = passesGroup.passWithShadowsLow;
													}
													else
														pass = passesGroup.passWithoutShadows;

													bool materialWasChanged = materialBinded != materialData;
													materialBinded = materialData;
													materialData.BindCurrentFrameData( context, false, materialWasChanged );
													materialData.BindCurrentFrameData_MaskTextures( context, layer.Mask );


													var batchInstancing = meshItem.BatchingInstanceBuffer != null;

													ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true );

													if( !batchInstancing )
														fixed( Matrix4F* p = &meshItem.Transform )
															Bgfx.SetTransform( (float*)p );

													var color = meshItem.Color * layer.Color;

													for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
													{
														var oper = meshData.RenderOperations[ nOperation ];

														BindRenderOperationData( context, frameData, materialData, batchInstancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance );

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

		public unsafe virtual void RenderTransparentLayersOnOpaqueBaseObjectsForward( ViewportRenderingContext context, FrameData frameData )
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
			Component_Material.CompiledMaterialData materialBinded = null;

			int ambientDirectionalLightCount = 0;
			foreach( var lightIndex in frameData.LightsInFrustumSorted )
			{
				var lightItem = frameData.Lights[ lightIndex ];
				if( lightItem.data.Type == Component_Light.TypeEnum.Point || lightItem.data.Type == Component_Light.TypeEnum.Spotlight )
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
									bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadowsLow != null;

									GpuMaterialPass pass;
									if( receiveShadows )
									{
										if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
											pass = passesGroup.passWithShadowsHigh;
										else
											pass = passesGroup.passWithShadowsLow;
									}
									else
										pass = passesGroup.passWithoutShadows;

									ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true );

									bool materialWasChanged = materialBinded != materialData;
									materialBinded = materialData;

									//bind material data
									materialData.BindCurrentFrameData( context, false, materialWasChanged );
									materialData.BindCurrentFrameData_MaskTextures( context, layer.Mask );

									fixed( Matrix4F* p = &meshItem.Transform )
										Bgfx.SetTransform( (float*)p );

									var color = meshItem.Color * layer.Color;

									for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
									{
										var oper = meshData.RenderOperations[ nOperation ];

										BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance );

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

		//!!!!new
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

		public unsafe virtual void Render3DSceneForwardTransparent( ViewportRenderingContext context, FrameData frameData )
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
			Component_Material.CompiledMaterialData materialBinded = null;

			int ambientDirectionalLightCount = 0;
			foreach( var lightIndex in frameData.LightsInFrustumSorted )
			{
				var lightItem = frameData.Lights[ lightIndex ];
				if( lightItem.data.Type == Component_Light.TypeEnum.Point || lightItem.data.Type == Component_Light.TypeEnum.Spotlight )
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
									bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadowsLow != null;

									GpuMaterialPass pass;
									if( receiveShadows )
									{
										if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
											pass = passesGroup.passWithShadowsHigh;
										else
											pass = passesGroup.passWithShadowsLow;
									}
									else
										pass = passesGroup.passWithoutShadows;

									ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true );

									bool materialWasChanged = materialBinded != materialData;
									materialBinded = materialData;

									materialData.BindCurrentFrameData( context, false, materialWasChanged );

									BindRenderOperationData( context, frameData, materialData, instancing, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref meshItem.Color, oper.VertexStructureContainsColor, false, meshItem.VisibilityDistance );

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
											bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadowsLow != null;

											GpuMaterialPass pass;
											if( receiveShadows )
											{
												if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
													pass = passesGroup.passWithShadowsHigh;
												else
													pass = passesGroup.passWithShadowsLow;
											}
											else
												pass = passesGroup.passWithoutShadows;

											ForwardBindGeneralTexturesUniforms( context, frameData, ref meshItem.BoundingSphere, lightItem, receiveShadows, true );

											bool materialWasChanged = materialBinded != materialData;
											materialBinded = materialData;

											//bind material data
											materialData.BindCurrentFrameData( context, false, materialWasChanged );
											materialData.BindCurrentFrameData_MaskTextures( context, layer.Mask );

											fixed( Matrix4F* p = &meshItem.Transform )
												Bgfx.SetTransform( (float*)p );

											var color = meshItem.Color * layer.Color;

											for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
											{
												var oper = meshData.RenderOperations[ nOperation ];

												BindRenderOperationData( context, frameData, materialData, false, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance );

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

					var meshData = Component_Billboard.GetBillboardMesh().Result.MeshData;

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
								bool receiveShadows = lightItem.prepareShadows && passesGroup.passWithShadowsLow != null;

								GpuMaterialPass pass;
								if( receiveShadows )
								{
									if( ShadowQuality.Value == ShadowQualityEnum.High && passesGroup.passWithShadowsHigh != null )
										pass = passesGroup.passWithShadowsHigh;
									else
										pass = passesGroup.passWithShadowsLow;
								}
								else
									pass = passesGroup.passWithoutShadows;

								ForwardBindGeneralTexturesUniforms( context, frameData, ref billboardItem.BoundingSphere, lightItem, receiveShadows, true );

								bool materialWasChanged = materialBinded != materialData;
								materialBinded = materialData;

								materialData.BindCurrentFrameData( context, false, materialWasChanged );

								BindRenderOperationData( context, frameData, materialData, instancing, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, oper.UnwrappedUV, ref billboardItem.Color, oper.VertexStructureContainsColor, false, billboardItem.VisibilityDistance );

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

		public void RenderEffectsInternal( ViewportRenderingContext context, FrameData frameData, string groupName, ref Component_Image actualTexture, bool onlyAO )
		{
			var group = GetComponent( groupName, true );
			if( group != null )
			{
				//!!!!children. где еще
				foreach( var effect in group.GetComponents<Component_RenderingEffect>( false, true, true ) )
				{
					if( effect.IsSupported )
					{
						bool isAO = effect is Component_RenderingEffect_AmbientOcclusion;
						if( onlyAO && isAO || !onlyAO && !isAO )
							effect.Render( context, frameData, ref actualTexture );
					}
				}
			}
		}

		public void CopyToCurrentViewport( ViewportRenderingContext context, Component_Image sourceTexture, CanvasRenderer.BlendingType blending = CanvasRenderer.BlendingType.Opaque )//, bool flipY = false )
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

			context.RenderQuadToCurrentViewport( shader, blending );//, flipY );
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
				deferredShadingData.clearBackgroundMesh = ComponentUtility.CreateComponent<Component_Mesh>( null, true, false );
				deferredShadingData.clearBackgroundMesh.CreateComponent<Component_MeshGeometry_Box>();
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
					@"Base\Shaders\DeferredBackgroundColor_vs.sc", generalDefines, out error2 );
				if( !string.IsNullOrEmpty( error2 ) )
				{
					//!!!!
					return;
					//Log.Fatal( error2 );
				}

				//fragment program
				var fragmentProgram = GpuProgramManager.GetProgram( "DeferredBackgroundColor_Fragment_", GpuProgramType.Fragment,
					@"Base\Shaders\DeferredBackgroundColor_fs.sc", generalDefines, out error2 );
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
				Matrix4F worldMatrix = Matrix4.FromTranslate( context.Owner.CameraSettings.Position ).ToMatrix4F();

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

		////!!!!new
		public delegate void RenderDeferredShadingEndDelegate( Component_RenderingPipeline_Basic sender, ViewportRenderingContext context, FrameData frameData, ref Component_Image sceneTexture );
		public event RenderDeferredShadingEndDelegate RenderDeferredShadingEnd;

		T GetSceneEffect<T>() where T : Component_RenderingEffect
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
			foreach( var lightIndex in frameData.LightsInFrustumSorted )
			{
				var item = frameData.Lights[ lightIndex ];
				if( item.prepareShadows )
					LightPrepareShadows( context, frameData, item );
			}

			/////////////////////////////////////
			//detect destination size, antialiasing technique
			Component_RenderingEffect_Antialiasing antialiasingEffect = null;
			Vector2I destinationSize = owner.SizeInPixels;
			int msaaLevel = 0;
			{
				var effect = GetSceneEffect<Component_RenderingEffect_Antialiasing>();
				if( effect != null && effect.Intensity > 0 && effect.IsSupported )
				{
					antialiasingEffect = effect;
					destinationSize = ( destinationSize.ToVector2() * effect.GetResolutionMultiplier() ).ToVector2I();
					msaaLevel = effect.GetMSAALevel();
				}
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
			Component_Image normalTexture = null;
			if( GetUseMultiRenderTargets() )
			{
				normalTexture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.A8R8G8B8, msaaLevel );
				context.objectsDuringUpdate.namedTextures[ "normalTexture" ] = normalTexture;
			}

			/////////////////////////////////////
			//create motion vectors texture
			var motionBlur = GetSceneEffect<Component_RenderingEffect_MotionBlur>();
			Component_Image motionTexture = null;
			if( motionBlur != null && motionBlur.IsSupported && GetUseMultiRenderTargets() )
			{
				motionTexture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.Float16GR );
				context.objectsDuringUpdate.namedTextures[ "motionTexture" ] = motionTexture;
			}

			/////////////////////////////////////
			//create depth texture
			var depthTexture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.Depth24S8, msaaLevel );
			context.objectsDuringUpdate.namedTextures[ "depthTexture" ] = depthTexture;

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

			////!!!!видать правильнее небо рисовать после opaque объктов
			////sky at beginning
			//if( frameData.sky != null )
			//{
			//	context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix );
			//	RenderSky( context, frameData );
			//}

			//deferred shading
			if( GetDeferredShading() && DebugDrawDeferredPass )
			{
				context.objectsDuringUpdate.namedTextures[ "gBuffer0Texture" ] = sceneTexture;
				context.objectsDuringUpdate.namedTextures[ "gBuffer1Texture" ] = normalTexture;

				//!!!!maybe clear all together

				var gBuffer2Texture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.A8R8G8B8 );
				context.objectsDuringUpdate.namedTextures[ "gBuffer2Texture" ] = gBuffer2Texture;
				//no sense to clear, but clean gbuffer
				context.SetViewport( gBuffer2Texture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity,
					FrameBufferTypes.Color, ColorValue.Zero );

				var gBuffer3Texture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.A8R8G8B8 );//Float16RGBA
				context.objectsDuringUpdate.namedTextures[ "gBuffer3Texture" ] = gBuffer3Texture;
				//no sense to clear, but clean gbuffer
				context.SetViewport( gBuffer3Texture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity,
					FrameBufferTypes.Color, ColorValue.Zero );

				//create GBuffer 4 texture
				var gBuffer4Texture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.A8R8G8B8 );
				context.objectsDuringUpdate.namedTextures[ "gBuffer4Texture" ] = gBuffer4Texture;
				//no sense to clear, but clean gbuffer
				context.SetViewport( gBuffer4Texture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity,
					FrameBufferTypes.Color, ColorValue.Zero );

				//create GBuffer 5 texture
				var gBuffer5Texture = context.RenderTarget2D_Alloc( destinationSize, PixelFormat.A8R8G8B8 );
				context.objectsDuringUpdate.namedTextures[ "gBuffer5Texture" ] = gBuffer5Texture;
				//no sense to clear, but clean gbuffer
				context.SetViewport( gBuffer5Texture.Result.GetRenderTarget().Viewports[ 0 ], Matrix4F.Identity, Matrix4F.Identity,
					FrameBufferTypes.Color, ColorValue.Zero );

				if( motionTexture != null )
					context.objectsDuringUpdate.namedTextures[ "gBuffer6Texture" ] = motionTexture;

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

				//!!!!new
				RenderDeferredShadingEnd?.Invoke( this, context, frameData, ref sceneTexture );

				////!!!!new
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
			if( frameData.Fog != null )
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
				Component_Image dummyNormalTexture = null;
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
				Render3DSceneForwardTransparent( context, frameData );

				if( dummyNormalTexture != null )
					context.DynamicTexture_Free( dummyNormalTexture );
			}

			////Antialiasing effect: rescale to demanded size
			//if( destinationSize != owner.SizeInPixels )
			//	antialiasingEffect.RenderDownscale( context, frameData, destinationSize, ref sceneTexture );

			//Scene effects (skip AO)
			if( owner.Mode == Viewport.ModeEnum.Default )
			{
				//antialiasing: downscale
				if( antialiasingEffect != null && antialiasingEffect.DownscaleBeforeSceneEffects && antialiasingEffect.IsSupported )
					antialiasingEffect.RenderDownscale( context, frameData, sceneTexture.Result.ResultSize, ref sceneTexture );

				RenderEffectsInternal( context, frameData, "Scene Effects", ref sceneTexture, false );
			}

			//outline effect for selected objects
			if( context.objectInSpaceRenderingContext.selectedObjects.Count != 0 )
				RenderOutlineEffectForSelectedObjects( context, frameData, ref sceneTexture );

			//convert to LDR
			ConvertToLDR( context, ref sceneTexture );

			/////////////////////////////////////
			//Simple 3D Renderer data
			if( owner.Simple3DRenderer != null )
			{
				if( DebugDrawSimple3DRenderer )
				{
					if( owner.Simple3DRenderer._ViewportRendering_PrepareRenderables() )
					{
						if( GetSimpleGeometryAntialiasing() )
						{
							//!!!!owner.SizeInPixels * 2
							var simpleDataTexture = context.RenderTarget2D_Alloc( owner.SizeInPixels * 2, PixelFormat.A8R8G8B8 );
							context.SetViewport( simpleDataTexture.Result.GetRenderTarget().Viewports[ 0 ], viewMatrix, projectionMatrix, FrameBufferTypes.Color, new ColorValue( 0, 0, 0, 0 ) );

							CopyToCurrentViewport( context, sceneTexture );

							//render simple geometry
							//if( Simple3DRendererOpacity != 0 )
							owner.Simple3DRenderer._ViewportRendering_RenderToCurrentViewport( context );

							//copy to scene texture with downscale
							{
								context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ] );

								CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
								shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
								shader.FragmentProgramFileName = @"Base\Shaders\Effects\Downscale2_fs.sc";

								shader.Parameters.Set( "sourceSizeInv", new Vector2F( 1, 1 ) / simpleDataTexture.Result.ResultSize.ToVector2F() );

								//bind textures
								{
									shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0/*"sourceTexture"*/,
										simpleDataTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );
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
							owner.Simple3DRenderer._ViewportRendering_RenderToCurrentViewport( context );
						}
					}
				}

				//!!!!!когда еще чистить?
				owner.Simple3DRenderer._Clear();
			}

			/////////////////////////////////////
			//GUI
			if( DebugDrawUI && owner.CanvasRenderer != null )
			{
				context.SetViewport( sceneTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				//!!!!new
				owner.CanvasRenderer._ViewportRendering_RenderToCurrentViewport( context, false, owner.LastUpdateTime );
				//owner.CanvasRenderer._ViewportRendering_RenderToCurrentViewport( context, true, owner.LastUpdateTime );
			}
			//Component_Texture guiTexture;
			//{
			//	//!!!!A8R8G8B8?
			//	guiTexture = context.RenderTarget2D_Alloc( owner.SizeInPixels, PixelFormat.A8R8G8B8 );

			//	//!!!!!EngineDebugSettings.DrawScreenGUI

			//	Viewport viewport = guiTexture.Result.GetRenderTarget().Viewports[ 0 ];
			//	context.SetViewport( viewport, Mat4F.Identity, Mat4F.Identity, FrameBufferTypes.Color, ColorValue.Zero );

			//	//!!!!new
			//	owner.CanvasRenderer._ViewportRendering_RenderToCurrentViewport( context, false, owner.LastUpdateTime );
			//	//owner.CanvasRenderer._ViewportRendering_RenderToCurrentViewport( context, true, owner.LastUpdateTime );

			//	//GUI effects
			//	RenderEffectsInternal( context, "GUI Effects", ref guiTexture, false );
			//}

			//Final image effects
			if( owner.Mode == Viewport.ModeEnum.Default )
				RenderEffectsInternal( context, frameData, "Final Image Effects", ref sceneTexture, false );

			///////////////////////////////////////
			////Final composition

			////Blend scene and GUI textures
			//Component_Texture finalTexture;
			//{
			//	finalTexture = context.RenderTarget2D_Alloc( owner.SizeInPixels, PixelFormat.A8R8G8B8 );

			//	context.SetViewport( finalTexture.Result.GetRenderTarget().Viewports[ 0 ] );

			//	CanvasRenderer.ShaderItem shader = new CanvasRenderer.ShaderItem();
			//	shader.VertexProgramFileName = @"Base\Shaders\ViewportComposition_vs.sc";
			//	shader.FragmentProgramFileName = @"Base\Shaders\ViewportComposition_fs.sc";
			//	//bind textures
			//	{
			//		var textureValue = new GpuMaterialPass.TextureParameterValue( sceneTexture,
			//			TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );
			//		shader.Parameters.Set( "0"/* "sceneTexture"*/, textureValue, ParameterType.Texture2D );
			//	}
			//	{
			//		var textureValue = new GpuMaterialPass.TextureParameterValue( debugGeometryTexture,
			//			TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );
			//		shader.Parameters.Set( "1"/* "debugGeometryTexture"*/, textureValue, ParameterType.Texture2D );
			//	}
			//	{
			//		var textureValue = new GpuMaterialPass.TextureParameterValue( guiTexture,
			//			TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );
			//		shader.Parameters.Set( "2"/* "guiTexture"*/, textureValue, ParameterType.Texture2D );
			//	}
			//	shader.Parameters.Set( "debugGeometryOpacity", (float)Simple3DRendererOpacity );
			//	context.RenderQuadToCurrentViewport( shader );

			//	//Final image effects
			//	RenderEffectsInternal( context, "Final Image Effects", ref finalTexture, false );
			//}

			/////////////////////////////////////
			//Copy to output target
			context.SetViewport( owner );
			//if( SystemSettings.CurrentPlatform == SystemSettings.Platform.Android )
			//	CopyToCurrentViewport( context, sceneTexture, flipY: true );
			//else
			CopyToCurrentViewport( context, sceneTexture );

			//clear or destroy something maybe
		}

		protected virtual void RenderWithoutRenderTargets( ViewportRenderingContext context, FrameData frameData )
		{
			var owner = context.Owner;

			Matrix4F viewMatrix = owner.CameraSettings.ViewMatrix.ToMatrix4F();
			Matrix4F projectionMatrix = owner.CameraSettings.ProjectionMatrix.ToMatrix4F();

			var backgroundColor = context.GetBackgroundColor();
			if( DebugMode.Value != DebugModeEnum.None )
				backgroundColor = new ColorValue( 0, 0, 0 );

			context.SetViewport( owner, viewMatrix, projectionMatrix, FrameBufferTypes.All, backgroundColor );

			/////////////////////////////////////
			//Scene
			//RenderSky( context, frameData );
			if( DebugDrawForwardOpaquePass )
				Render3DSceneForwardOpaque( context, frameData );
			RenderSky( context, frameData );
			if( DebugDrawForwardTransparentPass )
				Render3DSceneForwardTransparent( context, frameData );

			/////////////////////////////////////
			//debug geometry
			//if( Simple3DRendererOpacity != 0 )
			if( owner.Simple3DRenderer != null )
			{
				if( owner.Simple3DRenderer._ViewportRendering_PrepareRenderables() )
					owner.Simple3DRenderer._ViewportRendering_RenderToCurrentViewport( context );
				owner.Simple3DRenderer._Clear();
			}

			/////////////////////////////////////
			//GUI
			context.SetViewport( owner );

			//!!!!new
			owner.CanvasRenderer?._ViewportRendering_RenderToCurrentViewport( context, false, owner.LastUpdateTime );
			//owner.CanvasRenderer._ViewportRendering_RenderToCurrentViewport( context, true, owner.LastUpdateTime );

			//clear, destroy
		}

		//!!!!new
		public delegate void RenderBeginDelegate( Component_RenderingPipeline_Basic sender, ViewportRenderingContext context, FrameData frameData );
		public event RenderBeginDelegate RenderBegin;

		//!!!!new
		public delegate void RenderEndDelegate( Component_RenderingPipeline_Basic sender, ViewportRenderingContext context, FrameData frameData );
		public event RenderEndDelegate RenderEnd;

		public override void Render( ViewportRenderingContext context )
		{
			if( context.FrameData != null )
				Log.Fatal( "Component_RenderingPipeline_Basic: Render: context.FrameData != null." );
			//!!!!create each time?
			var frameData = new FrameData();
			context.FrameData = frameData;

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

				//display bounds for Component_ObjectInSpace
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

			//!!!!!чистить еще что-то? может выше что-то
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
			public float unused1;
		}

		public unsafe void SetViewportOwnerSettingsUniform( ViewportRenderingContext context, DebugModeEnum? overrideDebugMode = null )
		{
			//!!!!double

			var data = new ViewportOwnerSettingsUniform();
			data.cameraPosition = context.Owner.CameraSettings.Position.ToVector3F();
			data.nearClipDistance = (float)context.Owner.CameraSettings.NearClipDistance;
			data.farClipDistance = (float)context.Owner.CameraSettings.FarClipDistance;
			data.fieldOfView = (float)context.Owner.CameraSettings.FieldOfView.InRadians();
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

			data.emissiveMaterialsFactor = (float)context.Owner.CameraSettings.EmissiveFactor;
			data.cameraExposure = (float)context.Owner.CameraSettings.Exposure;
			data.displacementScale = (float)DisplacementMappingScale;
			data.displacementMaxSteps = DisplacementMappingMaxSteps;
			data.removeTextureTiling = (float)RemoveTextureTiling;

			int vec4Count = sizeof( ViewportOwnerSettingsUniform ) / sizeof( Vector4F );
			if( vec4Count != 4 )
				Log.Fatal( "Component_RenderingPipeline: Render: vec4Count != 4." );
			context.SetUniform( "u_viewportOwnerSettings", ParameterType.Vector4, vec4Count, &data );
		}

		static bool IsEqualCutVolumes( RenderSceneData.CutVolumeItem[] array1, RenderSceneData.CutVolumeItem[] array2 )
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

		OpenList<RenderSceneData.CutVolumeItem> tempCutVolumes;

		unsafe void SetCutVolumeSettingsUniforms( ViewportRenderingContext context, RenderSceneData.CutVolumeItem[] cutVolumes, bool forceUpdate )
		{
			var needUpdate = !IsEqualCutVolumes( context.currentCutVolumes, cutVolumes ) || forceUpdate;
			if( !needUpdate )
				return;

			context.currentCutVolumes = cutVolumes;


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


			//also need change 'GLOBAL_CUT_VOLUME_MAX_COUNT' in Assets\Base\Shaders\GlobalSettings.sh.
			var GLOBAL_CUT_VOLUME_MAX_COUNT = 10;

			var count = Math.Min( list.Count, GLOBAL_CUT_VOLUME_MAX_COUNT );

			var settings = new Vector4F( count, 0, 0, 0 );
			var data = stackalloc Matrix4F[ GLOBAL_CUT_VOLUME_MAX_COUNT ];

			for( int n = 0; n < count; n++ )
			{
				ref var item = ref list.Data[ n ];

				if( item.Shape == Component_CutVolume.ShapeEnum.Plane )
				{
					//!!!!double
					new Matrix4F( item.Plane.ToPlaneF().ToVector4F(), Vector4F.Zero, Vector4F.Zero, Vector4F.Zero ).GetTranspose( out data[ n ] );
				}
				else
				{
					item.Transform.ToMatrix4().GetInverse( out var inv );

					//!!!!double
					inv.ToMatrix4F( out data[ n ] );
				}

				//save shape type in the matrix
				data[ n ].Item3.W = (int)item.Shape;
			}

			context.SetUniform( "u_viewportCutVolumeSettings", ParameterType.Vector4, 1, &settings );
			if( count != 0 )
				context.SetUniform( "u_viewportCutVolumeData", ParameterType.Matrix4x4, GLOBAL_CUT_VOLUME_MAX_COUNT, data );
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public virtual void GetBackgroundEnvironmentTextures( ViewportRenderingContext context, FrameData frameData, out EnvironmentTextureData? texture, out EnvironmentTextureData? textureIBL )
		{
			//Sky
			if( context.Owner.AttachedScene != null && frameData.Sky != null )
			{
				if( frameData.Sky.GetEnvironmentTextureData( out var texture2, out var textureIBL2 ) )
				{
					texture = texture2;
					textureIBL = textureIBL2;
					return;
				}

				//var skybox = frameData.Sky as Component_Skybox;
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
					color = context.GetBackgroundColor();

				var affect = context.GetBackgroundColorAffectLighting();

				var rotation = Matrix3F.Identity;
				var multiplier = color.ToVector3F();
				texture = new EnvironmentTextureData( ResourceUtility.WhiteTextureCube, affect, ref rotation, ref multiplier );
				textureIBL = new EnvironmentTextureData( ResourceUtility.WhiteTextureCube, affect, ref rotation, ref multiplier );
			}
		}

		void GetEnvironmentTexturesByBoundingSphereIntersection( ViewportRenderingContext context, FrameData frameData, ref Sphere objectSphere,
			out EnvironmentTextureData? texture1, out EnvironmentTextureData? textureIBL1,
			out EnvironmentTextureData? texture2, out EnvironmentTextureData? textureIBL2,
			out float blendingFactor )
		{
			texture1 = null;
			textureIBL1 = null;
			texture2 = null;
			textureIBL2 = null;
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
						var d = ( probeSphere.Origin - objectSphere.Origin ).Length();
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
					if( probe1.data.CubemapIrradiance != null )
						textureIBL1 = new EnvironmentTextureData( probe1.data.CubemapIrradiance, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
					if( probe2.data.CubemapEnvironment != null )
						texture2 = new EnvironmentTextureData( probe2.data.CubemapEnvironment, 1, ref probe2.data.Rotation, ref probe2.data.Multiplier );
					if( probe2.data.CubemapIrradiance != null )
						textureIBL2 = new EnvironmentTextureData( probe2.data.CubemapIrradiance, 1, ref probe2.data.Rotation, ref probe2.data.Multiplier );
					blendingFactor = (float)( probe1Intensity / ( probe1Intensity + probe2Intensity ) );
				}
				else if( probe1 != null )
				{
					if( probe1.data.CubemapEnvironment != null )
						texture1 = new EnvironmentTextureData( probe1.data.CubemapEnvironment, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
					if( probe1.data.CubemapIrradiance != null )
						textureIBL1 = new EnvironmentTextureData( probe1.data.CubemapIrradiance, 1, ref probe1.data.Rotation, ref probe1.data.Multiplier );
					blendingFactor = (float)probe1Intensity;
				}
			}

			//init nulls by sky texture or white cube
			GetBackgroundEnvironmentTextures( context, frameData, out var backgroundTexture, out var backgroundTextureIBL );
			if( texture1 == null )
				texture1 = backgroundTexture;
			if( texture2 == null )
				texture2 = backgroundTexture;
			if( textureIBL1 == null )
				textureIBL1 = backgroundTextureIBL;
			if( textureIBL2 == null )
				textureIBL2 = backgroundTextureIBL;
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

		public delegate void FogExtensionGetUniformDataDelegate( ViewportRenderingContext context, Component_Fog fog, IntPtr data, int dataSizeInBytes );

		static bool fogExtensionEnabled;
		static int fogExtensionShaderUniformVec4Count;
		static string fogExtensionShaderCode;
		static FogExtensionGetUniformDataDelegate fogExtensionGetUniformData;

		/// <summary>
		/// Need call before engine initialization from AssemblyUtils.AssemblyRegistration class.
		/// </summary>
		/// <param name="shaderUniformVec4Count"></param>
		/// <param name="shaderCode"></param>
		/// <param name="getUniformData"></param>
		public static void SetFogExtension(/* bool enable, */int shaderUniformVec4Count, string shaderCode, FogExtensionGetUniformDataDelegate getUniformData )
		{
			//!!!!дефайном shaderUniformVec4Count, shaderCode
			Log.Fatal( "Component_RenderingPipeline_Basic: SetFogExtension: impl." );

			unsafe
			{
				if( shaderUniformVec4Count < sizeof( FogDefaultUniform ) / sizeof( Vector4F ) )
					Log.Fatal( "Component_RenderingPipeline_Basic: SetFogExtension: shaderUniformVec4Count < sizeof( FogDefaultUniform ) / sizeof( Vec4F )." );
			}

			fogExtensionEnabled = true;// enable;
			fogExtensionShaderUniformVec4Count = shaderUniformVec4Count;
			fogExtensionShaderCode = shaderCode;
			fogExtensionGetUniformData = getUniformData;
		}

		unsafe void SetFogUniform( ViewportRenderingContext context, Component_Fog fog )
		{
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
				if( mode.HasFlag( Component_Fog.Modes.Exp ) )
					data->distanceMode = 1;
				if( mode.HasFlag( Component_Fog.Modes.Exp2 ) )
					data->distanceMode = 2;
				data->startDistance = (float)fog.StartDistance;
				data->density = (float)fog.Density;
				if( mode.HasFlag( Component_Fog.Modes.Height ) )
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
				var texture = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
				texture.CreateType = Component_Image.TypeEnum._2D;
				texture.CreateSize = new Vector2I( 1, 1 );
				texture.CreateMipmaps = false;
				texture.CreateFormat = PixelFormat.Float32R;
				texture.CreateUsage = Component_Image.Usages.WriteOnly;
				texture.Enabled = true;

				if( texture.Result != null )
				{
					var data = new byte[ 4 ];
					var d = new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( 0, 0, data ) };
					texture.Result.SetData( d );
				}

				nullShadowTexture2D = texture;
			}

			if( nullShadowTextureCube == null )
			{
				var texture = ComponentUtility.CreateComponent<Component_Image>( null, true, false );
				texture.CreateType = Component_Image.TypeEnum.Cube;
				texture.CreateSize = new Vector2I( 1, 1 );
				texture.CreateMipmaps = false;
				texture.CreateFormat = PixelFormat.Float32R;
				texture.CreateUsage = Component_Image.Usages.WriteOnly;
				texture.Enabled = true;

				if( texture.Result != null )
				{
					var data = new byte[ 4 ];
					var d = new GpuTexture.SurfaceData[ 6 ];
					for( int n = 0; n < 6; n++ )
						d[ n ] = new GpuTexture.SurfaceData( 0, 0, data );
					texture.Result.SetData( d );
				}

				nullShadowTextureCube = texture;
			}
		}

		public unsafe void BindRenderOperationData( ViewportRenderingContext context, FrameData frameData, Component_Material.CompiledMaterialData materialData, bool instancing, RenderSceneData.MeshItem.AnimationDataClass meshItemAnimationData, int billboardMode, double billboardRadius, bool receiveDecals, ref Vector3F previousWorldPosition, float lodValue, UnwrappedUVEnum unwrappedUV, ref ColorValue color, bool vertexStructureContainsColor, bool isLayer, float visibilityDistance )
		{
			//!!!!can merge with bit flags
			Vector4F data0 = Vector4F.Zero;
			Vector4F data1 = Vector4F.Zero;
			Vector4F data2 = Vector4F.Zero;
			Vector4F data3 = Vector4F.Zero;
			Vector4F data4;

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

			data2 = new Vector4F( previousWorldPosition, lodValue );

			data3.X = (int)unwrappedUV;
			if( vertexStructureContainsColor )
				data3.Y = 1;
			if( isLayer )
				data3.Z = 1;

			data4 = color.ToVector4F();

			//set u_renderOperationData
			if( u_renderOperationDataCurrentValue[ 0 ] != data0 || u_renderOperationDataCurrentValue[ 1 ] != data1 || u_renderOperationDataCurrentValue[ 2 ] != data2 || u_renderOperationDataCurrentValue[ 3 ] != data3 || u_renderOperationDataCurrentValue[ 4 ] != data4 )
			{
				u_renderOperationDataCurrentValue[ 0 ] = data0;
				u_renderOperationDataCurrentValue[ 1 ] = data1;
				u_renderOperationDataCurrentValue[ 2 ] = data2;
				u_renderOperationDataCurrentValue[ 3 ] = data3;
				u_renderOperationDataCurrentValue[ 4 ] = data4;

				if( u_renderOperationData == null )
					u_renderOperationData = GpuProgramManager.RegisterUniform( "u_renderOperationData", UniformType.Vector4, 5 );
				fixed( Vector4F* pData = u_renderOperationDataCurrentValue )
					Bgfx.SetUniform( u_renderOperationData.Value, pData, 5 );
			}

			//!!!!не сбрасывать текстуры

			//bind bones texture
			{
				var bonesTexture = meshItemAnimationData != null ? meshItemAnimationData.BonesTexture : ResourceUtility.BlackTexture2D;
				context.BindTexture( new ViewportRenderingContext.BindTextureData( 0/*"s_bones"*/, bonesTexture, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None ) );
			}

			//bind materials data texture
			if( materialData != null )
			{
				if( materialData.currentFrameIndex == -1 )
					Log.Fatal( "Component_RenderingPipeline_Basic: BindRenderOperationData: materialData.currentFrameIndex == -1." );

				context.BindTexture( new ViewportRenderingContext.BindTextureData( 1/*"s_materials"*/, frameData.MaterialsTexture, TextureAddressingMode.Clamp, FilterOption.None, FilterOption.None, FilterOption.None ) );
			}
		}

		[Browsable( false )]
		public static Component_Image BrdfLUT
		{
			get
			{
				if( brdfLUT == null )
					brdfLUT = ResourceManager.LoadResource<Component_Image>( @"Base\Images\brdfLUT.dds" );
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

			int tempBufferSize = size.X * size.Y * 16;
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

					var pData2 = pData + n * sizeof( Component_Material.CompiledMaterialData.DynamicParametersFragmentUniform );
					var pData3 = (Component_Material.CompiledMaterialData.DynamicParametersFragmentUniform*)pData2;
					*pData3 = materialData.dynamicParametersFragmentUniformData;
				}
			}

			frameData.MaterialsTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, Component_Image.TypeEnum._2D, size, PixelFormat.Float32RGBA, 0, false );
			frameData.MaterialsTexture.Result.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( 0, 0, data ) } );
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

		void RenderOutlineEffectForSelectedObjects( ViewportRenderingContext context, FrameData frameData, ref Component_Image actualTexture )
		{
			var outline = new Component_RenderingEffect_Outline();
			outline.GroupsInterval = new RangeI( int.MaxValue, int.MaxValue );
			outline.Scale = ProjectSettings.Get.SceneEditorSelectOutlineEffectScale;

			outline.Render( context, frameData, ref actualTexture );
		}
	}
}
