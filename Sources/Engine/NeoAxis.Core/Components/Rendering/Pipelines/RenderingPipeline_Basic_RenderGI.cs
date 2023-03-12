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

		static Program? giClearGBufferGridProgram;
		static Program? giClearLightingGridProgram;
		static Uniform? giObjectDataUniform;
		static Uniform? giRenderLightParametersUniform;
		static Program? giCopyLightingGridProgram;
		static GIDeferredShadingData giDeferredShadingData;

		//!!!!temp
		static bool? giShaderFilesExists;

		///////////////////////////////////////////////

		class GIDeferredShadingData
		{
			public Program[] passesPerLightWithoutShadowsSimple = new Program[ 4 ];
			public Program[] passesPerLightWithShadowsSimple = new Program[ 4 ];
			public Program[] passesPerLightWithoutShadowsFull = new Program[ 4 ];
			public Program[] passesPerLightWithShadowsFull = new Program[ 4 ];
		}

		///////////////////////////////////////////////

		static Program? GIClearGBufferGridProgram
		{
			get
			{
				if( giClearGBufferGridProgram == null )
				{
					var defines = new List<(string, string)>();
					//defines.Add( ("MAX_BONES", maxBones.ToString()) );

					var program = GpuProgramManager.GetProgram( "GIClearGBufferGrid", GpuProgramType.Compute, @"Base\Shaders\GIClearGBufferGrid.sc", defines, true, out var error2 );
					if( !string.IsNullOrEmpty( error2 ) )
					{
						Log.Fatal( error2 );
						////var error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
						////Log.Error( error );
						//Log.Warning( error2 );
					}
					else
						giClearGBufferGridProgram = new Program( program.RealObject );
				}

				return giClearGBufferGridProgram;
			}
		}

		static Program? GIClearLightingGridProgram
		{
			get
			{
				if( giClearLightingGridProgram == null )
				{
					var defines = new List<(string, string)>();

					var program = GpuProgramManager.GetProgram( "GIClearLightingGrid", GpuProgramType.Compute, @"Base\Shaders\GIClearLightingGrid.sc", defines, true, out var error2 );
					if( !string.IsNullOrEmpty( error2 ) )
					{
						Log.Fatal( error2 );
						////var error = GpuProgramManager.GetGpuProgramCompilationErrorText( this, error2 );
						////Log.Error( error );
						//Log.Warning( error2 );
					}
					else
						giClearLightingGridProgram = new Program( program.RealObject );
				}

				return giClearLightingGridProgram;
			}
		}

		static Program? GICopyLightingGridProgram
		{
			get
			{
				if( giCopyLightingGridProgram == null )
				{
					var defines = new List<(string, string)>();

					var program = GpuProgramManager.GetProgram( "GICopyLightingGrid", GpuProgramType.Compute, @"Base\Shaders\GICopyLightingGrid.sc", defines, true, out var error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						Log.Fatal( error2 );
					else
						giCopyLightingGridProgram = new Program( program.RealObject );
				}

				return giCopyLightingGridProgram;
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected internal unsafe virtual void GIClearGBufferGrid( ViewportRenderingContext context, FrameData frameData, ImageComponent giGBufferGridTexture )
		{
			if( !GIClearGBufferGridProgram.HasValue )
				return;

			var data = frameData.IndirectLightingFrameData;
			var gridSize = data.GridResolution;
			var levels = data.Levels;

			context.BindComputeImage( 0, giGBufferGridTexture, 0, ComputeBufferAccessEnum.Write );

			var parameters = new Vector4F( gridSize * levels, gridSize, gridSize, 0 );
			//var parameters = new Vector4F( gridSize, 0, 0, 0 );
			var uniform = GpuProgramManager.RegisterUniform( "giClearGridParameters", UniformType.Vector4, 1 );
			Bgfx.SetUniform( uniform, &parameters );

			var jobSize = new Vector3I( (int)Math.Ceiling( gridSize * levels / 8.0 ), (int)Math.Ceiling( gridSize / 8.0 ), (int)Math.Ceiling( gridSize / 8.0 ) );
			//var jobSize = (int)Math.Ceiling( gridSize / 8.0 );

			Bgfx.Dispatch( context.CurrentViewNumber, GIClearGBufferGridProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
			context.UpdateStatisticsCurrent.ComputeDispatches++;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected internal unsafe virtual void GIClearLightingGrid( ViewportRenderingContext context, FrameData frameData, ImageComponent giLightingGridTexture )
		{
			if( !GIClearLightingGridProgram.HasValue )
				return;

			var data = frameData.IndirectLightingFrameData;
			var gridSize = data.GridResolution;
			var levels = data.Levels;

			context.BindComputeImage( 0, giLightingGridTexture, 0, ComputeBufferAccessEnum.Write );

			var parameters = new Vector4F( gridSize * levels, gridSize, gridSize, 0 );
			//var parameters = new Vector4F( gridSize, 0, 0, 0 );
			var uniform = GpuProgramManager.RegisterUniform( "giClearGridParameters", UniformType.Vector4, 1 );
			Bgfx.SetUniform( uniform, &parameters );

			var jobSize = new Vector3I( (int)Math.Ceiling( gridSize * levels / 8.0 ), (int)Math.Ceiling( gridSize / 8.0 ), (int)Math.Ceiling( gridSize / 8.0 ) );
			//var jobSize = (int)Math.Ceiling( gridSize / 8.0 );

			Bgfx.Dispatch( context.CurrentViewNumber, GIClearLightingGridProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
			context.UpdateStatisticsCurrent.ComputeDispatches++;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct GIObjectsData
		{
			public Matrix3x4F Transform;
			public ColorValue Color;
			public Vector4F GridIndexesMin;
			public Vector4F GridIndexesMax;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public unsafe void RenderOperationGI( ViewportRenderingContext context, Program? giVoxelProgram, GIObjectsData* objectData, ref BoundsI gridIndexes, RenderSceneData.CutVolumeItem[] cutVolumes )//, int level ) // RenderSceneData.MeshDataRenderOperation op, GpuMaterialPass pass, List<ParameterContainer> parameterContainers, RenderSceneData.CutVolumeItem[] cutVolumes, bool instancingEnabled, GpuVertexBuffer instancingBuffer, ref InstanceDataBuffer instancingDataBuffer, int instancingStart, int instancingCount )
		{
			if( !giVoxelProgram.HasValue )
				return;

			SetCutVolumeSettingsUniforms( context, cutVolumes, false );

			if( giObjectDataUniform == null )
				giObjectDataUniform = GpuProgramManager.RegisterUniform( "giObjectData", UniformType.Vector4, 6 );
			Bgfx.SetUniform( giObjectDataUniform.Value, objectData, 6 );

			var size = gridIndexes.GetSize() + new Vector3I( 1, 1, 1 );
			var jobSize = new Vector3I( (int)Math.Ceiling( size.X / 8.0 ), (int)Math.Ceiling( size.Y / 8.0 ), (int)Math.Ceiling( size.Z / 8.0 ) );

			var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

			Bgfx.Dispatch( context.CurrentViewNumber, giVoxelProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, discardFlags );
			context.UpdateStatisticsCurrent.ComputeDispatches++;


			//var data = context.FrameData.IndirectLightingFrameData;

			//var cellSizeOfLevel = data.CellSizeLevel0 * MathEx.Pow( 2.0f, level );
			//var gridSizeHalf = cellSizeOfLevel * data.GridResolution * 0.5f;
			//var gridPosition = data.GridCenter - new Vector3( gridSizeHalf, gridSizeHalf, gridSizeHalf );
			//var voxelizationFactor = (float)data.Owner.VoxelizationFactor.Value;

			//{
			//	var parameters = new Vector4F( cellSizeOfLevel, voxelizationFactor, data.GridResolution, level );
			//	if( giRenderToGridParametersUniform == null )
			//		giRenderToGridParametersUniform = GpuProgramManager.RegisterUniform( "giRenderToGridParameters", UniformType.Vector4, 1 );
			//	Bgfx.SetUniform( giRenderToGridParametersUniform.Value, &parameters );
			//}

			//{
			//	//!!!!double
			//	var v = new Vector4F( gridPosition.ToVector3F(), 0 );
			//	var uniform = GpuProgramManager.RegisterUniform( "gridPosition", UniformType.Vector4, 1 );
			//	Bgfx.SetUniform( uniform, &v );
			//}

			//{
			//	var v = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
			//	var uniform = GpuProgramManager.RegisterUniform( "giGridIndexesMin", UniformType.Vector4, 1 );
			//	Bgfx.SetUniform( uniform, &v );
			//}
			//{
			//	var v = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );
			//	var uniform = GpuProgramManager.RegisterUniform( "giGridIndexesMax", UniformType.Vector4, 1 );
			//	Bgfx.SetUniform( uniform, &v );
			//}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected internal unsafe virtual void Render3DSceneGI( ViewportRenderingContext context, FrameData frameData, ImageComponent giGBufferGridTexture, ImageComponent giLightingGridTexture, int level )
		{
			var indirectFrameData = frameData.IndirectLightingFrameData;

			ref var levelBounds = ref indirectFrameData.BoundsByLevel[ level ];

			var viewportOwner = context.Owner;
			var sectorsByDistance = SectorsByDistance.Value;

			using( var renderableGroupsToDraw = new OpenListNative<Vector2I>( frameData.RenderableGroupsInFrustum.Count + frameData.RenderableGroupsForGI.Count ) )
			{
				//RenderableGroupsInFrustum
				foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
				{
					bool add = false;

					if( renderableGroup.X == 0 )
					{
						ref var data = ref frameData.Meshes.Data[ renderableGroup.Y ];
						if( ( data.Flags & FrameData.MeshItem.FlagsEnum.UseDeferred ) != 0 )
						{
							ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
							if( levelBounds.Intersects( ref meshItem.BoundingSphere ) )
								add = true;
						}
					}
					else
					{
						ref var data = ref frameData.Billboards.Data[ renderableGroup.Y ];
						if( ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseDeferred ) != 0 )
						{
							ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];
							if( levelBounds.Intersects( ref billboardItem.BoundingSphere ) )
								add = true;
						}
					}

					if( add )
					{
						renderableGroupsToDraw.Add( renderableGroup );
						//var item = new RenderableGroupWithDistance();
						//item.RenderableGroup = renderableGroup;
						//item.DistanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.RenderableGroup );
						//renderableGroupsToDraw.Add( ref item );
					}
				}

				//RenderableGroupsForGI
				foreach( var renderableGroup in frameData.RenderableGroupsForGI )
				{
					bool add = false;

					if( renderableGroup.X == 0 )
					{
						ref var data = ref frameData.Meshes.Data[ renderableGroup.Y ];
						if( ( data.Flags & FrameData.MeshItem.FlagsEnum.UseDeferred ) != 0 )
						{
							ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
							if( levelBounds.Intersects( ref meshItem.BoundingSphere ) )
								add = true;
						}
					}
					else
					{
						ref var data = ref frameData.Billboards.Data[ renderableGroup.Y ];
						if( ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseDeferred ) != 0 )
						{
							ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];
							if( levelBounds.Intersects( ref billboardItem.BoundingSphere ) )
								add = true;
						}
					}

					if( add )
					{
						renderableGroupsToDraw.Add( renderableGroup );
						//var item = new RenderableGroupWithDistance();
						//item.RenderableGroup = renderableGroup;
						//item.DistanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.RenderableGroup );
						//renderableGroupsToDraw.Add( ref item );
					}
				}

				if( renderableGroupsToDraw.Count == 0 )
					return;


				////sort by distance
				//CollectionUtility.MergeSortUnmanaged( renderableGroupsToDraw.Data, renderableGroupsToDraw.Count, delegate ( RenderableGroupWithDistance* a, RenderableGroupWithDistance* b )
				//{
				//	if( a->DistanceSquared < b->DistanceSquared )
				//		return -1;
				//	if( a->DistanceSquared > b->DistanceSquared )
				//		return 1;
				//	return 0;
				//}, true );


				var indirectData = context.FrameData.IndirectLightingFrameData;
				var cellSizeOfLevel = indirectData.CellSizeLevel0 * MathEx.Pow( 2.0f, level );
				var cellSizeOfLevelInv = 1.0f / cellSizeOfLevel;
				var gridSizeHalf = cellSizeOfLevel * indirectData.GridResolution * 0.5f;
				var gridPosition = indirectData.GridCenter - new Vector3( gridSizeHalf, gridSizeHalf, gridSizeHalf );
				var voxelizationFactor = (float)indirectData.Owner.VoxelizationExpandFactor.Value;
				var emissiveLighting = indirectData.Owner.EmissiveLighting.Value;

				var parameters = stackalloc Vector4F[ 2 ];
				parameters[ 0 ] = new Vector4F( cellSizeOfLevel, voxelizationFactor, indirectData.GridResolution, level );
				//!!!!double
				parameters[ 1 ] = new Vector4F( gridPosition.ToVector3F(), (float)emissiveLighting );
				var uniform = GpuProgramManager.RegisterUniform( "giRenderToGridParameters", UniformType.Vector4, 2 );
				Bgfx.SetUniform( uniform, parameters, 2 );


				context.BindComputeImage( 0, giGBufferGridTexture, 0, ComputeBufferAccessEnum.Write );
				context.BindComputeImage( 1, giLightingGridTexture, 0, ComputeBufferAccessEnum.Write );
				//context.BindComputeImage( 3, giGBufferGridTexture, 0, ComputeBufferAccessEnum.Write );
				//context.BindComputeImage( 4, giLightingGridTexture, 0, ComputeBufferAccessEnum.Write );


				//bind textures for all render operations

				SetCutVolumeSettingsUniforms( context, null, true );

				//!!!!temp?
				BindMaterialsTexture( context, frameData, true );
				//BindMaterialsTexture( context, frameData );

				BindSamplersForTextureOnlySlots( context, true, true );
				BindMaterialData( context, null, false, true );


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

						if( renderableGroup/*.RenderableGroup*/.X == 0 )
						{
							//meshes

							ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup/*.RenderableGroup*/.Y ];

							var meshData = meshItem.MeshDataLOD0 ?? meshItem.MeshData;
							var lods = meshData.LODs;
							if( lods != null )
								meshData = lods[ lods.Length - 1 ].Mesh?.Result?.MeshData ?? meshData;

							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
							{
								var oper = meshData.RenderOperations[ nOperation ];

								if( oper.VoxelDataInfo != null )
								{
									foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, true ) )
									{
										if( materialData.deferredShadingSupport )
										{
											bool instancing = Instancing && meshItem.AnimationData == null && !meshItem.InstancingEnabled && meshItem.CutVolumes == null;
											manager.Add( renderableGroup/*.RenderableGroup*/, nOperation, oper, materialData, instancing );
										}
									}
								}
							}
						}
						else if( renderableGroup/*.RenderableGroup*/.X == 1 )
						{
							//billboards

							ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup/*.RenderableGroup*/.Y ];
							var meshData = Billboard.GetBillboardMesh().Result.MeshData;

							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
							{
								var oper = meshData.RenderOperations[ nOperation ];

								if( oper.VoxelDataInfo != null )
								{
									foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, true ) )
									{
										if( materialData.deferredShadingSupport )
										{
											bool instancing = Instancing && billboardItem.CutVolumes == null;
											manager.Add( renderableGroup/*.RenderableGroup*/, nOperation, oper, materialData, instancing );
										}
									}
								}
							}
						}
					}

					manager.Prepare();
				} );

				//!!!!can render parallel for each instance. deferred lighting and deferred lighting gi are also can be optimized

				//push to GPU
				for( int nSector = 0; nSector < sectorsByDistance; nSector++ )
				{
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
							var voxelRendering = outputItem.operation.VoxelDataInfo != null;

							if( Instancing && outputItem.renderableItemsCount >= InstancingMinCount )
							{
								//with instancing

								//bind material data
								BindMaterialData( context, materialData, false, voxelRendering );
								BindSamplersForTextureOnlySlots( context, false, voxelRendering );

								//GpuMaterialPass pass = null;

								//bind operation data
								var firstRenderableItem = outputItem.renderableItemFirst;
								if( firstRenderableItem.X == 0 )
								{
									//meshes
									ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ firstRenderableItem.Y ];
									var meshData = meshItem.MeshData;

									BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo );

									//pass = materialData.deferredShadingPass.Get( outputItem.operation.VoxelDataInfo != null, meshData.BillboardMode != 0 );


									//!!!!slowly instancing


									int instanceCount = outputItem.renderableItemsCount;
									for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
									{
										var renderableItem = outputItem.renderableItems[ nRenderableItem ];
										ref var meshItem2 = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];

										meshItem2.BoundingSphere.ToBounds( out var bounds );
										if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
										{
											var objectData = new GIObjectsData();
											meshItem2.Transform.GetTranspose( out objectData.Transform );
											objectData.Color = meshItem2.Color;
											objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
											objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

											RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
										}
									}

								}
								else if( firstRenderableItem.X == 1 )
								{
									//!!!!

									////billboards
									//ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ firstRenderableItem.Y ];
									//var meshData = Billboard.GetBillboardMesh().Result.MeshData;

									//BindRenderOperationData( context, frameData, materialData, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo );

									//pass = materialData.deferredShadingPass.Billboard;
								}


								//int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
								//int instanceCount = outputItem.renderableItemsCount;

								//if( InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) == instanceCount )
								//{
								//	var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );

								//	//get instancing matrices
								//	RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
								//	int currentMatrix = 0;
								//	for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
								//	{
								//		var renderableItem = outputItem.renderableItems[ nRenderableItem ];

								//		if( renderableItem.X == 0 )
								//		{
								//			//meshes
								//			ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
								//			//!!!!slowly because no threading? where else
								//			meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
								//		}
								//		else if( renderableItem.X == 1 )
								//		{
								//			//billboards
								//			ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
								//			billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
								//		}
								//	}

								//	RenderOperation( context, outputItem.operation, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
								//}

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
										ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
										var meshData = meshItem.MeshData;

										BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo );

										//!!!!slowly
										//!!!!можно точнее считать? везде так


										//unsafe
										//{
										//	var t = new Vector4F( materialData.currentFrameIndex, 0, 0, 0 );
										//	var temptemp = GpuProgramManager.RegisterUniform( "temptemp", UniformType.Vector4, 1 );
										//	Bgfx.SetUniform( temptemp, &t );
										//}


										//!!!!batching


										//var tr = meshItem.Transform;

										//unsafe
										//{
										//	if( giObjectTransformUniform == null )
										//		giObjectTransformUniform = GpuProgramManager.RegisterUniform( "objectTransform", UniformType.Matrix4x4, 1 );
										//	Bgfx.SetUniform( giObjectTransformUniform.Value, &tr );// meshItem.Transform );
										//}

										//!!!!?
										//if( !meshItem.InstancingEnabled )
										//	fixed( Matrix4F* p = &meshItem.Transform )
										//		Bgfx.SetTransform( (float*)p );

										//var pass = materialData.deferredShadingPass.Get( outputItem.operation.VoxelDataInfo != null/*, outputItem.operation.VirtualizedData != null*/, meshData.BillboardMode != 0 );


										//!!!!slowly batching


										if( meshItem.InstancingEnabled )
										{
											//!!!!или всегда везде выставлять
											var instancingCount = meshItem.InstancingCount;
											if( instancingCount < 0 )
											{
												//!!!! - instancingStart?

												if( meshItem.InstancingVertexBuffer != null )
													instancingCount = meshItem.InstancingVertexBuffer.VertexCount;
												else
													instancingCount = meshItem.InstancingDataBuffer.Size;
											}

											for( int nObject = 0; nObject < instancingCount; nObject++ )
											{
												if( meshItem.InstancingVertexBuffer != null )
												{
													var array = meshItem.InstancingVertexBuffer.Vertices;
													if( array != null )
													{
														fixed( byte* pArray = array )
														{
															var instancingData = (RenderSceneData.ObjectInstanceData*)pArray;
															var instancingData2 = instancingData + nObject + meshItem.InstancingStart;

															instancingData2->Transform.GetTranslation( out var pos );

															var bounds = new Bounds( pos );
															bounds.Expand( meshItem.InstancingMaxLocalBounds );

															//meshData.SpaceBounds.BoundingSphere.Radius
															//meshItem.BoundingSphere.ToBounds( out var bounds );

															if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
															{
																var objectData = new GIObjectsData();
																objectData.Transform = instancingData2->Transform;

																//!!!!
																var c = instancingData2->Color.ToColorValue();// / 255.0f;
																objectData.Color.Red = MathEx.Pow( c.Red, 2 ) * 10.0f;
																objectData.Color.Green = MathEx.Pow( c.Green, 2 ) * 10.0f;
																objectData.Color.Blue = MathEx.Pow( c.Blue, 2 ) * 10.0f;
																objectData.Color.Alpha = MathEx.Pow( c.Alpha, 2 ) * 10.0f;

																objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
																objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

																RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
															}
														}
													}
												}
												else
												{
													var instancingData = (RenderSceneData.ObjectInstanceData*)meshItem.InstancingDataBuffer.Data;
													var instancingData2 = instancingData + nObject + meshItem.InstancingStart;

													instancingData2->Transform.GetTranslation( out var pos );

													var bounds = new Bounds( pos );
													bounds.Expand( meshItem.InstancingMaxLocalBounds );

													if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
													{
														var objectData = new GIObjectsData();
														objectData.Transform = instancingData2->Transform;

														//!!!!
														var c = instancingData2->Color.ToColorValue();// / 255.0f;
														objectData.Color.Red = MathEx.Pow( c.Red, 2 ) * 10.0f;
														objectData.Color.Green = MathEx.Pow( c.Green, 2 ) * 10.0f;
														objectData.Color.Blue = MathEx.Pow( c.Blue, 2 ) * 10.0f;
														objectData.Color.Alpha = MathEx.Pow( c.Alpha, 2 ) * 10.0f;

														objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
														objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

														RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
													}
												}
											}
										}
										else
										{
											meshItem.BoundingSphere.ToBounds( out var bounds );
											if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
											{
												var objectData = new GIObjectsData();
												meshItem.Transform.GetTranspose( out objectData.Transform );
												objectData.Color = meshItem.Color;
												objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
												objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

												RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
											}
										}

										//RenderOperation( context, outputItem.operation, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount );

									}
									else if( renderableItem.X == 1 )
									{
										//billboards
										ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
										var meshData = Billboard.GetBillboardMesh().Result.MeshData;

										//!!!!

										//BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo );

										//billboardItem.GetWorldMatrix( out var worldMatrix );
										//Bgfx.SetTransform( (float*)&worldMatrix );

										//var pass = materialData.deferredShadingPass.Billboard;

										//RenderOperation( context, outputItem.operation, pass, null, billboardItem.CutVolumes );

									}
								}
							}
						}
					}

					//!!!!impl
					////render layers
					//if( DebugDrawLayers )
					//{
					//	for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
					//	{
					//		var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

					//		if( renderableGroup.RenderableGroup.X == 0 )
					//		{
					//			//meshes

					//			ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.RenderableGroup.Y ];
					//			var meshData = meshItem.MeshData;

					//			if( meshItem.Layers != null )
					//			{
					//				for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
					//				{
					//					ref var layer = ref meshItem.Layers[ nLayer ];
					//					foreach( var materialData in GetLayerMaterialData( ref layer, true, true ) )
					//					{
					//						if( materialData.deferredShadingPass.Usual != null )
					//						{
					//							if( !meshItem.InstancingEnabled )
					//								fixed( Matrix4F* p = &meshItem.Transform )
					//									Bgfx.SetTransform( (float*)p );

					//							var color = /*meshItem.Color * */ layer.MaterialColor;

					//							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
					//							{
					//								var oper = meshData.RenderOperations[ nOperation ];
					//								var voxelRendering = oper.VoxelDataInfo != null;

					//								//bind material data
					//								BindMaterialData( context, materialData, false, voxelRendering );
					//								BindSamplersForTextureOnlySlots( context, false, voxelRendering );
					//								materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );

					//								BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, null, oper.VoxelDataInfo/*, oper.VirtualizedDataImage, oper.VirtualizedDataInfo*/ );

					//								var pass = materialData.deferredShadingPass.Get( oper.VoxelDataInfo != null/*, oper.VirtualizedData != null*/, meshData.BillboardMode != 0 );

					//								RenderOperation( context, oper, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount );
					//							}
					//						}
					//					}
					//				}
					//			}
					//		}
					//	}
					//}
				}

				//clear outputInstancingManagers
				foreach( var manager in outputInstancingManagers )
					manager.Clear();
			}
		}

		void InitGIDeferredShadingData()
		{
			giDeferredShadingData = new GIDeferredShadingData();

			for( int nShadingModel = 0; nShadingModel < 2; nShadingModel++ )
			{
				var shadingModelFull = nShadingModel != 0;

				//environment light
				{
					var defines = new List<(string, string)>();
					defines.Add( ("LIGHT_TYPE_" + Light.TypeEnum.Ambient.ToString().ToUpper(), "") );
					if( shadingModelFull )
						defines.Add( ("GI_SHADING_MODEL_FULL", "") );

					string error2;
					var program = GpuProgramManager.GetProgram( "GIRenderAmbientLight_", GpuProgramType.Compute, @"Base\Shaders\GIRenderAmbientLight.sc", defines, true, out error2 );
					if( !string.IsNullOrEmpty( error2 ) )
					{
						Log.Fatal( error2 );
						return;
					}

					var program2 = new Program( program.RealObject );

					if( shadingModelFull )
						giDeferredShadingData.passesPerLightWithoutShadowsFull[ 0 ] = program2;
					else
						giDeferredShadingData.passesPerLightWithoutShadowsSimple[ 0 ] = program2;
				}

				//direct lights
				for( int nShadows = 0; nShadows < 2; nShadows++ )
				{
					var shadows = nShadows != 0;

					if( RenderingSystem.ShadowTechnique == ProjectSettingsPage_Rendering.ShadowTechniqueEnum.None && shadows )
						continue;

					for( int nLight = 1; nLight < 4; nLight++ )
					{
						var lightType = (Light.TypeEnum)nLight;

						var defines = new List<(string, string)>();
						defines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );
						if( shadows )
							defines.Add( ("SHADOW_MAP", "") );
						if( shadingModelFull )
							defines.Add( ("GI_SHADING_MODEL_FULL", "") );

						string error2;
						var program = GpuProgramManager.GetProgram( "GIRenderDirectLight_", GpuProgramType.Compute, @"Base\Shaders\GIRenderDirectLight.sc", defines, true, out error2 );
						if( !string.IsNullOrEmpty( error2 ) )
						{
							Log.Fatal( error2 );
							return;
						}

						var program2 = new Program( program.RealObject );

						if( shadingModelFull )
						{
							if( nShadows == 1 )
								giDeferredShadingData.passesPerLightWithShadowsFull[ nLight ] = program2;
							else
								giDeferredShadingData.passesPerLightWithoutShadowsFull[ nLight ] = program2;
						}
						else
						{
							if( nShadows == 1 )
								giDeferredShadingData.passesPerLightWithShadowsSimple[ nLight ] = program2;
							else
								giDeferredShadingData.passesPerLightWithoutShadowsSimple[ nLight ] = program2;
						}
					}
				}
			}
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void RenderEnvironmentLightDeferredGI( ViewportRenderingContext context, FrameData frameData, ImageComponent giGBufferGridTexture, ImageComponent giLightingGridTexture, int level )
		{
			var indirectData = context.FrameData.IndirectLightingFrameData;

			var ambientLighting = indirectData.Owner.AmbientLighting.Value;
			if( ambientLighting <= 0 )
				return;

			var cellSizeOfLevel = indirectData.CellSizeLevel0 * MathEx.Pow( 2.0f, level );
			var cellSizeOfLevelInv = 1.0f / cellSizeOfLevel;
			var gridSizeHalf = cellSizeOfLevel * indirectData.GridResolution * 0.5f;
			var gridPosition = indirectData.GridCenter - new Vector3( gridSizeHalf, gridSizeHalf, gridSizeHalf );
			var voxelizationFactor = (float)indirectData.Owner.VoxelizationExpandFactor.Value;
			var gridSize = indirectData.GridResolution;

			var gridIndexes = new BoundsI( 0, 0, 0, gridSize - 1, gridSize - 1, gridSize - 1 );


			//!!!!можно без свапа текстур, т.к. полное копирование. где еще так


			//make copy of lighting texture because can't image load and write from rgba16f
			var tempLightingTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize, gridSize, gridSize ), PixelFormat.Float16RGBA, 0, false );
			if( tempLightingTexture == null )
				return;

			{
				if( !GICopyLightingGridProgram.HasValue )
					return;

				context.BindComputeImage( 0, tempLightingTexture, 0, ComputeBufferAccessEnum.Write );
				context.BindTexture( 1, giLightingGridTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
				//!!!!is not work
				//context.BindComputeImage( 1, giLightingGridTexture, 0, ComputeBufferAccessEnum.Read );

				var parameters = stackalloc Vector4F[ 3 ];
				parameters[ 0 ] = new Vector4F( indirectData.GridResolution, level, 0, 0 );
				parameters[ 1 ] = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
				parameters[ 2 ] = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );
				var uniform = GpuProgramManager.RegisterUniform( "giCopyLightingGridParameters", UniformType.Vector4, 3 );
				Bgfx.SetUniform( uniform, parameters, 3 );

				var size = gridIndexes.GetSize() + new Vector3I( 1, 1, 1 );
				var jobSize = new Vector3I( (int)Math.Ceiling( size.X / 8.0 ), (int)Math.Ceiling( size.Y / 8.0 ), (int)Math.Ceiling( size.Z / 8.0 ) );

				var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

				Bgfx.Dispatch( context.CurrentViewNumber, GICopyLightingGridProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, discardFlags );
				context.UpdateStatisticsCurrent.ComputeDispatches++;
			}

			{
				context.BindComputeImage( 0, giLightingGridTexture, 0, ComputeBufferAccessEnum.Write );
				context.BindTexture( 1, giGBufferGridTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
				context.BindTexture( 2, tempLightingTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
				//context.BindComputeImage( 1, giGBufferGridTexture, 0, ComputeBufferAccessEnum.Read );
				//context.BindComputeImage( 2, tempLightingTexture, 0, ComputeBufferAccessEnum.Read );

				var parameters = stackalloc Vector4F[ 4 ];
				parameters[ 0 ] = new Vector4F( cellSizeOfLevel, voxelizationFactor, indirectData.GridResolution, level );
				parameters[ 1 ] = new Vector4F( gridPosition.ToVector3F(), 0 );
				parameters[ 2 ] = new Vector4F( gridIndexes.Minimum.ToVector3F(), (float)ambientLighting );
				parameters[ 3 ] = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );
				if( giRenderLightParametersUniform == null )
					giRenderLightParametersUniform = GpuProgramManager.RegisterUniform( "giRenderLightParameters", UniformType.Vector4, 4 );
				Bgfx.SetUniform( giRenderLightParametersUniform.Value, parameters, 4 );

				var ambientLight = frameData.Lights[ frameData.LightsInFrustumSorted[ 0 ] ];
				ambientLight.Bind( this, context );

				BindBrdfLUT( context );

				if( u_deferredEnvironmentData == null )
					u_deferredEnvironmentData = GpuProgramManager.RegisterUniform( "u_deferredEnvironmentData", UniformType.Vector4, 4 );
				if( u_deferredEnvironmentIrradiance == null )
					u_deferredEnvironmentIrradiance = GpuProgramManager.RegisterUniform( "u_deferredEnvironmentIrradiance", UniformType.Vector4, 9 );

				GetBackgroundEnvironmentData( context, frameData, out var ambientLightTexture, out var ambientLightIrradiance );

				context.BindTexture( 8/*"s_environmentTexture"*/, ambientLightTexture.Value.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, true );

				var data = new DeferredEnvironmentDataUniform();
				data.rotation = ambientLightTexture.Value.Rotation;
				data.multiplierAndAffect = ambientLightTexture.Value.MultiplierAndAffect;
				data.iblRotation = ambientLightIrradiance.Value.Rotation;
				data.iblMultiplierAndAffect = ambientLightIrradiance.Value.MultiplierAndAffect;
				Bgfx.SetUniform( u_deferredEnvironmentData.Value, &data, 4 );

				fixed( Vector4F* harmonics = ambientLightIrradiance.Value.Harmonics ?? EnvironmentIrradianceData.GrayHarmonics )
					Bgfx.SetUniform( u_deferredEnvironmentIrradiance.Value, harmonics, 9 );


				Program program;
				if( indirectData.Owner.ShadingModelFull.Value )
					program = giDeferredShadingData.passesPerLightWithoutShadowsFull[ 0 ];
				else
					program = giDeferredShadingData.passesPerLightWithoutShadowsSimple[ 0 ];

				var size = gridIndexes.GetSize() + new Vector3I( 1, 1, 1 );
				var jobSize = new Vector3I( (int)Math.Ceiling( size.X / 8.0 ), (int)Math.Ceiling( size.Y / 8.0 ), (int)Math.Ceiling( size.Z / 8.0 ) );

				var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

				Bgfx.Dispatch( context.CurrentViewNumber, program, jobSize.X, jobSize.Y, jobSize.Z, discardFlags );
				context.UpdateStatisticsCurrent.ComputeDispatches++;
			}

			context.DynamicTexture_Free( tempLightingTexture );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		unsafe void RenderDirectLightDeferredGI( ViewportRenderingContext context, FrameData frameData, ImageComponent giGBufferGridTexture, ImageComponent giLightingGridTexture, int level, LightItem lightItem )
		{
			var indirectData = context.FrameData.IndirectLightingFrameData;

			var directLighting = indirectData.Owner.DirectLighting.Value;
			if( directLighting <= 0 )
				return;

			var cellSizeOfLevel = indirectData.CellSizeLevel0 * MathEx.Pow( 2.0f, level );
			var cellSizeOfLevelInv = 1.0f / cellSizeOfLevel;
			var gridSizeHalf = cellSizeOfLevel * indirectData.GridResolution * 0.5f;
			var gridPosition = indirectData.GridCenter - new Vector3( gridSizeHalf, gridSizeHalf, gridSizeHalf );
			var voxelizationFactor = (float)indirectData.Owner.VoxelizationExpandFactor.Value;
			var gridSize = indirectData.GridResolution;


			BoundsI gridIndexes;

			if( lightItem.data.Type == Light.TypeEnum.Point )
			{
				var lightBounds = new Bounds( lightItem.data.Position );
				lightBounds.Expand( lightItem.data.AttenuationFar );

				if( !indirectData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref lightBounds, out gridIndexes ) )
					return;
			}
			else if( lightItem.data.Type == Light.TypeEnum.Spotlight )
			{
				//!!!!slowly

				//Spotlight
				var outer = lightItem.data.SpotlightOuterAngle;
				if( outer > 179 )
					outer = 179;
				var radius = lightItem.data.AttenuationFar * Math.Tan( outer.InRadians() / 2 );
				var height = lightItem.data.AttenuationFar;
				SimpleMeshGenerator.GenerateCone( 0, SimpleMeshGenerator.ConeOrigin.Center, radius, height, 16, true, true, out Vector3[] positions, out var indices );

				var mat = new Matrix4(
					lightItem.data.Rotation.ToMatrix3() * Matrix3.FromRotateByY( Math.PI ) * Matrix3.FromScale( new Vector3( 1.1, 1.1, 1.1 ) ),
					lightItem.data.Position + lightItem.data.Rotation.GetForward() * lightItem.data.AttenuationFar / 2 );

				for( int n = 0; n < positions.Length; n++ )
					positions[ n ] = mat * positions[ n ];

				var lightBounds = new Bounds( positions[ 0 ] );
				for( int n = 1; n < positions.Length; n++ )
					lightBounds.Add( ref positions[ n ] );

				if( !indirectData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref lightBounds, out gridIndexes ) )
					return;
			}
			else
				gridIndexes = new BoundsI( 0, 0, 0, gridSize - 1, gridSize - 1, gridSize - 1 );


			//!!!!может использовать другой формат чтобы поменьше конвертировать

			//!!!!? can be parallel if write to different images and combine at the end. depends how made gpu internally

			//make copy of lighting texture because can't image load and write from rgba16f
			var tempLightingTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize, gridSize, gridSize ), PixelFormat.Float16RGBA, 0, false );
			if( tempLightingTexture == null )
				return;

			{
				if( !GICopyLightingGridProgram.HasValue )
					return;

				context.BindComputeImage( 0, tempLightingTexture, 0, ComputeBufferAccessEnum.Write );
				context.BindTexture( 1, giLightingGridTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
				//!!!!is not work
				//context.BindComputeImage( 1, giLightingGridTexture, 0, ComputeBufferAccessEnum.Read );

				var parameters = stackalloc Vector4F[ 3 ];
				parameters[ 0 ] = new Vector4F( indirectData.GridResolution, level, 0, 0 );
				parameters[ 1 ] = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
				parameters[ 2 ] = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );
				var uniform = GpuProgramManager.RegisterUniform( "giCopyLightingGridParameters", UniformType.Vector4, 3 );
				Bgfx.SetUniform( uniform, parameters, 3 );

				var size = gridIndexes.GetSize() + new Vector3I( 1, 1, 1 );
				var jobSize = new Vector3I( (int)Math.Ceiling( size.X / 8.0 ), (int)Math.Ceiling( size.Y / 8.0 ), (int)Math.Ceiling( size.Z / 8.0 ) );

				var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

				Bgfx.Dispatch( context.CurrentViewNumber, GICopyLightingGridProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, discardFlags );
				context.UpdateStatisticsCurrent.ComputeDispatches++;
			}

			{
				context.BindComputeImage( 0, giLightingGridTexture, 0, ComputeBufferAccessEnum.Write );
				context.BindTexture( 1, giGBufferGridTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
				context.BindTexture( 2, tempLightingTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
				//context.BindComputeImage( 1, giGBufferGridTexture, 0, ComputeBufferAccessEnum.Read );
				//context.BindComputeImage( 2, tempLightingTexture, 0, ComputeBufferAccessEnum.Read );

				var parameters = stackalloc Vector4F[ 4 ];
				parameters[ 0 ] = new Vector4F( cellSizeOfLevel, voxelizationFactor, indirectData.GridResolution, level );
				parameters[ 1 ] = new Vector4F( gridPosition.ToVector3F(), 0 );
				parameters[ 2 ] = new Vector4F( gridIndexes.Minimum.ToVector3F(), (float)directLighting );
				parameters[ 3 ] = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );
				if( giRenderLightParametersUniform == null )
					giRenderLightParametersUniform = GpuProgramManager.RegisterUniform( "giRenderLightParameters", UniformType.Vector4, 4 );
				Bgfx.SetUniform( giRenderLightParametersUniform.Value, parameters, 4 );

				lightItem.Bind( this, context );

				BindBrdfLUT( context );

				//light mask
				if( RenderingSystem.LightMask && lightItem.data.Type != Light.TypeEnum.Ambient )
				{
					if( lightItem.data.Type == Light.TypeEnum.Point )
					{
						var texture = lightItem.data.Mask;
						if( texture == null || texture.Result.TextureType != ImageComponent.TypeEnum.Cube )
							texture = ResourceUtility.WhiteTextureCube;

						context.BindTexture( 4, texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Point, 0, true );
					}
					else
					{
						var texture = lightItem.data.Mask;
						if( texture == null || texture.Result.TextureType != ImageComponent.TypeEnum._2D )
							texture = ResourceUtility.WhiteTexture2D;

						var clamp = lightItem.data.Type == Light.TypeEnum.Spotlight;
						context.BindTexture( 4, texture, clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Point, 0, true );
					}
				}

				//shadowMap
				{
					ImageComponent texture;
					if( lightItem.prepareShadows )
						texture = lightItem.shadowTexture;
					else
					{
						if( lightItem.data.Type == Light.TypeEnum.Point )
							texture = nullShadowTextureCube;
						else
							texture = nullShadowTexture2D;
					}

					if( RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4 )
						context.BindTexture( 5, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point, 0, true );
					else
						context.BindTexture( 5, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point, TextureFlags.CompareLessEqual, true );
				}

				Program program;
				if( indirectData.Owner.ShadingModelFull.Value )
				{
					if( lightItem.prepareShadows )
						program = giDeferredShadingData.passesPerLightWithShadowsFull[ (int)lightItem.data.Type ];
					else
						program = giDeferredShadingData.passesPerLightWithoutShadowsFull[ (int)lightItem.data.Type ];
				}
				else
				{
					if( lightItem.prepareShadows )
						program = giDeferredShadingData.passesPerLightWithShadowsSimple[ (int)lightItem.data.Type ];
					else
						program = giDeferredShadingData.passesPerLightWithoutShadowsSimple[ (int)lightItem.data.Type ];
				}

				var size = gridIndexes.GetSize() + new Vector3I( 1, 1, 1 );
				var jobSize = new Vector3I( (int)Math.Ceiling( size.X / 8.0 ), (int)Math.Ceiling( size.Y / 8.0 ), (int)Math.Ceiling( size.Z / 8.0 ) );

				var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

				Bgfx.Dispatch( context.CurrentViewNumber, program, jobSize.X, jobSize.Y, jobSize.Z, discardFlags );
				context.UpdateStatisticsCurrent.ComputeDispatches++;
			}

			context.DynamicTexture_Free( tempLightingTexture );
		}

		[MethodImpl( (MethodImplOptions)512 )]
		protected unsafe virtual void RenderLightsDeferredGI( ViewportRenderingContext context, FrameData frameData, ImageComponent giGBufferGridTexture, ImageComponent giLightingGridTexture, int level )
		{
			if( giDeferredShadingData == null )
				InitGIDeferredShadingData();

			foreach( var lightIndex in frameData.LightsInFrustumSorted )
			{
				var lightItem = frameData.Lights[ lightIndex ];

				if( lightItem.data.Type == Light.TypeEnum.Ambient )
					RenderEnvironmentLightDeferredGI( context, frameData, giGBufferGridTexture, giLightingGridTexture, level );
				else
					RenderDirectLightDeferredGI( context, frameData, giGBufferGridTexture, giLightingGridTexture, level, lightItem );
			}
		}

		protected internal unsafe virtual void RenderIndirectLightingFullTechnique( ViewportRenderingContext context, FrameData frameData, ref ImageComponent deferredLightTexture, ImageComponent normalTexture, ImageComponent gBuffer2Texture, ImageComponent gBuffer3Texture, ImageComponent gBuffer4Texture, ImageComponent gBuffer5Texture, ImageComponent motionAndObjectIdTexture, ImageComponent depthTexture )
		{
			var indirectFrameData = frameData.IndirectLightingFrameData;
			var indirectLighting = indirectFrameData.Owner;
			var gridSize = indirectFrameData.GridResolution;
			var levels = indirectFrameData.Levels;
			//var cellSizeLevel0 = indirectFrameData.CellSizeLevel0;
			//var gridBounds = indirectFrameData.GridBounds;


			if( motionAndObjectIdTexture == null || depthTexture == null )
				return;
			if( indirectLighting.Intensity <= 0 )
				return;
			var multiplier = /*indirectLighting.Multiplier * */RenderingEffect_IndirectLighting.GlobalMultiplier;
			if( multiplier <= 0 )
				return;

			var pipeline = context.RenderingPipeline;
			if( !pipeline.GetUseMultiRenderTargets() )
				return;


			//!!!!temp
			//check shader files exists
			if( !giShaderFilesExists.HasValue )
			{
				giShaderFilesExists = VirtualFile.Exists( @"Base\Shaders\GIClearGBufferGrid.sc" );

				if( !giShaderFilesExists.Value )
					Log.Warning( "Indirect Lighting: Shader files are not exist." );
			}
			if( !giShaderFilesExists.Value )
				return;


			//get current viewport context data

			//var anyDataKey = GetIndirectLightingDataKey();

			//ViewportContextData viewportContextData = null;
			//{
			//	context.AnyDataAutoDispose.TryGetValue( anyDataKey, out var current );
			//	viewportContextData = current as ViewportContextData;
			//}

			//var recreate = false;
			//if( viewportContextData != null )
			//{
			//	//if( viewportContextData.ObjectsTextureCreatedCount != fullModeData.objects.Count ||
			//	//	viewportContextData.ObjectTypesTextureCreatedCount != fullModeData.objectTypes.Count ||
			//	//	viewportContextData.DirectRadianceTextureCreatedCount != fullModeData.directRadiance.Count )
			//	//{
			//	//	recreate = true;
			//	//}
			//}

			////if( !EngineApp._DebugCapsLock )
			////	recreate = true;


			////create new
			//if( viewportContextData == null || recreate )
			//{


			//	//!!!!impl partial dynamic update



			//	//delete old
			//	if( viewportContextData != null )
			//	{
			//		viewportContextData.Dispose();
			//		viewportContextData = null;

			//		context.AnyDataAutoDispose.Remove( anyDataKey );
			//	}


			//	//create context data
			//	viewportContextData = new ViewportContextData();
			//	context.AnyDataAutoDispose[ anyDataKey ] = viewportContextData;

			//	//create and copy textures

			//}


			//alloc gbuffer texture

			//Atomic limitations
			//There are some severe limitations on image atomic operations.First, atomics can only be used on integer images, either signed or unsigned.Second, they can only be used on images with the GL_R32I/ r32i or GL_R32UI/ r32ui formats.

			var giGBufferGridTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize * levels, gridSize, gridSize ), PixelFormat.R32G32_UInt, 0, false );//R32G32B32A32_UInt
			if( giGBufferGridTexture == null )
				return;
			context.ObjectsDuringUpdate.namedTextures[ "giGBufferGridTexture" ] = giGBufferGridTexture;

			//alloc lighting texture

			var giLightingGridTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize * levels, gridSize, gridSize ), PixelFormat.Float16RGBA, 0, false );
			if( giLightingGridTexture == null )
				return;
			context.ObjectsDuringUpdate.namedTextures[ "giLightingGridTexture" ] = giLightingGridTexture;


			//enable view for compute operations
			context.SetComputePass();

			GIClearGBufferGrid( context, frameData, giGBufferGridTexture );
			GIClearLightingGrid( context, frameData, giLightingGridTexture );

			for( int level = 0; level < levels; level++ )
				Render3DSceneGI( context, frameData, giGBufferGridTexture, giLightingGridTexture, level );

			//render lights
			{
				//!!!!может быть лучше перебирать источники, внутри перебирать уровни

				for( int level = 0; level < levels; level++ )
					RenderLightsDeferredGI( context, frameData, giGBufferGridTexture, giLightingGridTexture, level );
			}


			//voxel cone tracing

			var lightingTextureSize = deferredLightTexture.Result.ResultSize / (int)indirectLighting.ResolutionFull.Value;

			var lightingTexture = context.RenderTarget2D_Alloc( lightingTextureSize, deferredLightTexture.Result.ResultFormat );
			{
				context.SetViewport( lightingTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				var shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Full_Lighting_fs.sc";

				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, giGBufferGridTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, giLightingGridTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, normalTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, gBuffer2Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 4, gBuffer3Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 5, gBuffer4Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 6, gBuffer5Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 7, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

				//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 8, BrdfLUT, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

				//NativeMethods.bgfx_set_vertex_buffer( 0, objectsBuffer.NativeObjectHandle, 0, -1 );
				////Bgfx.SetVertexBuffer( 0, objectsBuffer.NativeObjectHandle );

				//!!!!как лучше разложить настройки
				//!!!!!еще есть Intensity эффекта
				shader.Parameters.Set( "diffuseIntensity", (float)indirectLighting.DiffuseOutput.Value );
				shader.Parameters.Set( "specularIntensity", (float)indirectLighting.SpecularOutput.Value );
				shader.Parameters.Set( "multiplier", (float)multiplier );

				var data = indirectFrameData;

				shader.Parameters.Set( "gridParameters", new Vector4F( data.Levels, data.GridResolution, data.CellSizeLevel0, 0 ) );
				//!!!!double
				shader.Parameters.Set( "gridCenter", data.GridCenter.ToVector3F() );

				shader.Parameters.Set( "traceParameters", new Vector4F( (float)indirectLighting.TraceLength.Value, (float)indirectLighting.TraceStartOffset.Value, (float)indirectLighting.TraceStepFactor.Value, 0 ) );

				//!!!!double
				context.Owner.CameraSettings.GetViewProjectionInverseMatrix().ToMatrix4F( out var invViewProjMatrix );

				//!!!!double
				shader.Parameters.Set( "cameraPosition", context.Owner.CameraSettings.Position.ToVector3F() );

				//shader.Parameters.Set( "viewProj", viewProjMatrix );
				shader.Parameters.Set( "invViewProj", invViewProjMatrix );


				context.RenderQuadToCurrentViewport( shader );


				//var traceLength = indirectLighting.TraceLength.Value * indirectLighting.Distance.Value;
				//var distanceLevel0 = data.CellSizeLevel0 * data.GridResolution * 0.5f;
				//var traceLength = (float)indirectLighting.TraceLength.Value * distanceLevel0;

				//shader.Parameters.Set( "gridSize", (float)gridSize );
				////!!!!double
				//shader.Parameters.Set( "gridPosition", indirectFrameData.GridPosition.ToVector3F() );
				//shader.Parameters.Set( "cellSize", (float)indirectFrameData.CellSize );
			}


			//!!!!apply depth comparison. bilateral. где еще
			//blur
			var blurTexture = pipeline.GaussianBlur( context, lightingTexture, indirectLighting.BlurFactor, indirectLighting.BlurDownscalingMode, indirectLighting.BlurDownscalingValue );


			//free lighting texture
			if( lightingTexture != null )
				context.DynamicTexture_Free( lightingTexture );


			//write to deferredLightTexture with Add blending
			{
				context.SetViewport( deferredLightTexture.Result.GetRenderTarget().Viewports[ 0 ] );

				var shader = new CanvasRenderer.ShaderItem();
				shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
				shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Full_Final_fs.sc";

				shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, blurTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

				shader.Parameters.Set( "intensity", (float)indirectLighting.Intensity );

				context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
			}


			//free targets
			context.DynamicTexture_Free( blurTexture );

			//used later for debug visualization
			//context.DynamicTexture_Free( giGBufferGridTexture );
			//context.DynamicTexture_Free( giLightingGridTexture );
		}
	}
}






//	{
//		var gridData = fullModeData.GridData;



//		//var gridData = new byte[ GridSize * GridSize * GridSize * 4 ];
//		//var gridData = new float[ GridSize * GridSize * GridSize ];

//		//var gpuData = new byte[ textureSize * textureSize * 4 ];
//		//Array.Copy( data, 0, gpuData, 0, data.Length );

//		//var image = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, new Vector2I( textureSize, textureSize ), PixelFormat.Float32R, 0, false );


//		var image = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
//		image.CreateType = ImageComponent.TypeEnum._3D;
//		image.CreateSize = new Vector2I( GridSize, GridSize );
//		image.CreateDepth = GridSize;
//		//!!!!
//		image.CreateMipmaps = true;
//		//image.CreateMipmaps = false;
//		image.CreateFormat = PixelFormat.Float32R;
//		//!!!!AutoMipmaps
//		image.CreateUsage = ImageComponent.Usages.WriteOnly;// | ImageComponent.Usages.AutoMipmaps;
//		image.Enabled = true;

//		var gpuTexture = image.Result;
//		if( gpuTexture != null )
//		{
//			var datas = new List<GpuTexture.SurfaceData>();
//			datas.Add( new GpuTexture.SurfaceData( gridData ) );

//			var current = gridData;
//			var currentSize = GridSize;

//			for( int gridSize = GridSize / 2; gridSize > 0; gridSize /= 2 )
//			{
//				var mip = GenerateMip( current, currentSize );

//				datas.Add( new GpuTexture.SurfaceData( mip, mipLevel: datas.Count ) );

//				current = mip;
//				currentSize = gridSize;
//			}

//			gpuTexture.SetData( datas.ToArray() );


//			//var datas = new List<GpuTexture.SurfaceData>();
//			//for( int z = 0; z < GridSize; z++ )
//			//{
//			//	var data = new GpuTexture.SurfaceData( new ArraySegment<byte>( gridData, GridSize * GridSize * 4 * z, GridSize * GridSize * 4 ) );
//			//	datas.Add( data );
//			//}
//			//gpuTexture.SetData( datas.ToArray() );

//			//gpuTexture.SetData( new GpuTexture.SurfaceData[] { new GpuTexture.SurfaceData( gpuData ) } );
//		}

//		viewportContextData.GridTexture = image;


//{

//	//create and copy textures

//	{
//		var gridData = new byte[ gridSize * gridSize * gridSize * 4 ];

//		fixed( byte* pGridData = gridData )
//		{
//			float* p = (float*)pGridData;
//			for( int n = 0; n < gridData.Length / 4; n++ )
//				p[ n ] = 1;
//		}


//		//!!!!
//		var GridSize = gridSize;


//		//var gridData = new byte[ GridSize * GridSize * GridSize * 4 ];
//		//var gridData = new float[ GridSize * GridSize * GridSize ];

//		//var gpuData = new byte[ textureSize * textureSize * 4 ];
//		//Array.Copy( data, 0, gpuData, 0, data.Length );

//		//var image = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.DynamicTexture, ImageComponent.TypeEnum._2D, new Vector2I( textureSize, textureSize ), PixelFormat.Float32R, 0, false );



//		var gpuTexture = gridTexture.Result;
//		if( gpuTexture != null )
//		{
//			var datas = new List<GpuTexture.SurfaceData>();
//			datas.Add( new GpuTexture.SurfaceData( gridData ) );

//			var current = gridData;
//			var currentSize = GridSize;

//			for( int gridSize2 = GridSize / 2; gridSize2 > 0; gridSize2 /= 2 )
//			{
//				var mip = RenderingEffect_IndirectLighting.GenerateMip( current, currentSize );

//				datas.Add( new GpuTexture.SurfaceData( mip, mipLevel: datas.Count ) );

//				current = mip;
//				currentSize = gridSize2;
//			}

//			gpuTexture.SetData( datas.ToArray() );
//		}
//	}
//}



//if( indirectLighting.DebugMode.Value != RenderingEffect_IndirectLighting.DebugModeEnum.None )
//{
//	var level = indirectLighting.DebugModeLevel.Value;

//	var cellSizeOfLevel = indirectFrameData.CellSizeLevel0 * MathEx.Pow( 2.0f, level );
//	var offset = cellSizeOfLevel * indirectFrameData.GridResolution * 0.5f;
//	//var gridPosition = indirectFrameData.GridCenter - new Vector3( offset, offset, offset );

//	var gridBounds = new Bounds( indirectFrameData.GridCenter );
//	gridBounds.Expand( offset );

//	var renderer = context.Owner.Simple3DRenderer;
//	renderer.SetColor( new ColorValue( 0, 0, 1 ) );
//	renderer.AddBounds( gridBounds );
//}

//!!!!
////debug
//if( indirectLighting.DebugShowGridBounds )
//{
//	var renderer = context.Owner.Simple3DRenderer;
//	renderer.SetColor( new ColorValue( 1, 0, 0 ) );
//	renderer.AddBounds( gridBounds );
//}



//[MethodImpl( (MethodImplOptions)512 )]
//protected internal unsafe virtual void GIClearGBufferGrid( ViewportRenderingContext context, FrameData frameData, int gridSize, ImageComponent giGBufferGridTexture )
//{
//	zzzzz;
//	var computeProgram = GIClearGridProgram;
//	if( computeProgram.Value == null )
//		return;


//	//!!!!размер у мипов меньше
//	//!!!!
//	int mipLevels = 10;


//	for( int level = 0; level < mipLevels; level++ )
//	{

//		//!!!!
//		//Log.Info(gridTexture.Result.Mipmaps


//		//var maxBones = Math.Max( MathEx.NextPowerOfTwo( bones.Length ), 64 );

//		////set bone data
//		//var boneData = new Matrix4F[ maxBones ];
//		//{
//		//	//!!!!
//		//}

//		////enumerate render operations of the mesh
//		//for( int nOper = 0; nOper < modifiableMesh.Result.MeshData.RenderOperations.Count; nOper++ )
//		//{

//		//var sourceOper = originalMesh.Result.MeshData.RenderOperations[ nOper ];
//		//var destOper = modifiableMesh.Result.MeshData.RenderOperations[ nOper ];

//		//var sourceVertexBuffer = sourceOper.VertexBuffers[ 0 ];
//		//var destVertexBuffer = destOper.VertexBuffers[ 0 ];

//		//bind buffers
//		//if( sourceVertexBuffer.Flags.HasFlag( GpuBufferFlags.Dynamic ) || sourceVertexBuffer.Flags.HasFlag( GpuBufferFlags.ComputeWrite ) )
//		//	Bgfx.SetComputeBuffer( 0, (DynamicVertexBuffer)sourceVertexBuffer.GetNativeObject(), ComputeBufferAccess.Read );
//		//else
//		//	Bgfx.SetComputeBuffer( 0, (VertexBuffer)sourceVertexBuffer.GetNativeObject(), ComputeBufferAccess.Read );


//		Bgfx.SetComputeImage( 0, gridTexture.Result.GetNativeObject( true ), (byte)level, ComputeBufferAccess.Write, TextureFormat.R32F );
//		//Bgfx.SetComputeImage( 0, gridTexture.Result.GetNativeObject( true ), (byte)level, ComputeBufferAccess.Write, TextureFormat.R32U );

//		//!!!!
//		//Bgfx.SetComputeImage( 0, texture.Result.GetNativeObject( true ), 0, ComputeBufferAccess.Write, TextureFormat.RGBA32F );
//		//Bgfx.SetComputeImage( 0, texture.Result.GetRealObject( true ), 0, ComputeBufferAccess.Write, TextureFormat.R32F );
//		// R16F );
//		//Bgfx.SetComputeImage( 0, texture.Result.GetRealObject( false ), 0, ComputeBufferAccess.Write, TextureFormat.R32F );// R16F );

//		//Bgfx.SetComputeBuffer( 0, (DynamicVertexBuffer)destVertexBuffer.GetNativeObject(), ComputeBufferAccess.Write );


//		//bind parameters
//		{
//			var parameters = new Vector4F( gridSize, 0, 0, 0 );

//			if( giClearGridParametersUniform == null )
//				giClearGridParametersUniform = GpuProgramManager.RegisterUniform( "giClearGridParameters", UniformType.Vector4, 1 );
//			Bgfx.SetUniform( giClearGridParametersUniform.Value, &parameters );



//			//var arraySize = 1;

//			//var parameters = stackalloc Vector4F[ arraySize ];
//			//parameters[ 0 ] = new Vector4F( gridSize, 0, 0, 0 );

//			//if( giClearGridParametersUniform == null )
//			//	giClearGridParametersUniform = GpuProgramManager.RegisterUniform( "giClearGridParameters", UniformType.Vector4, arraySize );
//			//Bgfx.SetUniform( giClearGridParametersUniform, parameters, arraySize );


//			//Vector4F testParameter = TestParameter.Value;
//			////Vector4F testParameter = new Vector4F( TestParameter.Value, 0, 0, 0 );

//			//var arraySize = 1;// sizeof( SkinningParameters ) / sizeof( Vector4F );
//			//if( u_testParameter == null )
//			//	u_testParameter = GpuProgramManager.RegisterUniform( "u_testParameter", UniformType.Vector4, arraySize );
//			//Bgfx.SetUniform( u_testParameter.Value, &testParameter, arraySize );


//			//Vector4F testParameter = TestParameter.Value;
//			////Vector4F testParameter = new Vector4F( TestParameter.Value, 0, 0, 0 );

//			//var arraySize = 1;// sizeof( SkinningParameters ) / sizeof( Vector4F );
//			//if( u_testParameter == null )
//			//	u_testParameter = GpuProgramManager.RegisterUniform( "u_testParameter", UniformType.Vector4, arraySize );
//			//Bgfx.SetUniform( u_testParameter.Value, &testParameter, arraySize );

//			////var arraySize = sizeof( SkinningParameters ) / sizeof( Vector4F );
//			////if( u_skinningParameters == null )
//			////	u_skinningParameters = GpuProgramManager.RegisterUniform( "u_skinningParameters", UniformType.Vector4, arraySize );
//			////Bgfx.SetUniform( u_skinningParameters.Value, &parameters, arraySize );
//		}

//		//shader->setVectori( "u_min", start );
//		//shader->bindImage3D( texID, "u_image", GL_WRITE_ONLY, format );
//		//shader->setInt( "u_resolution", int( resolution ) );
//		//shader->setInt( "u_clipmapLevel", int( clipmapLevel ) );
//		//shader->setVectori( "u_extent", extent );
//		//shader->setInt( "u_borderWidth", borderWidth );

//		//extent = glm::ivec3( glm::ceil( glm::vec3( extent ) / 8.0f ) );

//		var jobSize = (int)Math.Ceiling( gridSize / 8.0 );

//		Bgfx.Dispatch( context.CurrentViewNumber, computeProgram.Value, jobSize, jobSize, jobSize, DiscardFlags.All );
//	}
//}
