// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Internal.SharpBgfx;

namespace NeoAxis
{
	public partial class RenderingPipeline_Basic
	{
		// Don't add many non static fields. Rendering pipeline is created for each temporary render target during frame rendering.

		const PixelFormat giColorGridFormat = PixelFormat.R11G11B10_Float;//Float16RGBA;

		static Program? giClearGridsProgram;
		static Uniform giObjectDataUniform;
		static Uniform giRenderToGridOperationDataUniform;
		static Program? giStandardGIVoxelPrepareProgram;
		static Uniform giMeshBoundsUniform;

		static Dictionary<int, GIInstanceBuffersItem> giInstanceBuffers = new Dictionary<int, GIInstanceBuffersItem>();

		static Program? giCalculateSmallGridProgram;
		static Program? giCalculateVerySmallGridProgram;
		static Program? giCalculateSDF4Program;
		static Program? giCalculateSDFProgram;
		//static Program? giCopyGrid1Program;
		static Program? giCalculateIndirectLightingProgram;
		static Program? giCopyToPreviousGridProgram;

		//!!!!temp? не так. каждому свой счетчик
		static int giFrameCounter;

		///////////////////////////////////////////////

		public class GIData
		{
			public float VoxelizationConservative;

			public int GridSize;
			public CascadeItem[] Cascades;

			public ImageComponent PreviousGrid2Texture;
			public bool PreviousGrid2TextureJustCreated;

			public ImageComponent Grid1Texture;
			public ImageComponent Grid2Texture;
			public ImageComponent Grid2TextureIndirected;

			/////////////////////

			public class CascadeItem
			{
				public BoundsF BoundsRelative;
				public Bounds BoundsWorld;
				public float CellSize;
				public float CellSizeInv;
			}
		}

		///////////////////////////////////////////////

		class GIInstanceBuffersItem
		{
			//public IndirectBuffer IndirectBuffer;
			public GpuVertexBuffer InstanceDataBuffer;
		}

		///////////////////////////////////////////////

		static int GetGIGridSizeEnumAsInteger( GIGridSizeEnum value )
		{
			return 64 << (int)value;
		}

		double[] GIGetShadowCascadeSplitDistances( ViewportRenderingContext context )
		{
			var result = new double[ GICascades + 1 ];

			int index = 0;
			//!!!!
			result[ index++ ] = 1;
			//result[ index++ ] = context.Owner.CameraSettings.NearClipDistance;

			if( GICascades >= 2 )
			{
				double[] values = new double[ GICascades ];
				values[ 0 ] = 1;
				for( int n = 1; n < values.Length; n++ )
					values[ n ] = values[ n - 1 ] * GICascadeDistribution;

				double total = 0;
				foreach( var v in values )
					total += v;
				if( total <= 0.01 )
					total = 0.01;

				double current = 0;
				for( int n = 0; n < values.Length - 1; n++ )
				{
					current += values[ n ];
					result[ index++ ] = GIDistance * current / total;
				}
			}

			result[ index++ ] = GIDistance;

			return result;
		}

		protected internal unsafe virtual void GIInit( ViewportRenderingContext context )
		{
			if( RenderingSystem.GlobalIllumination && ( IndirectLighting || Reflection ) && GIDistance.Value >= 0 && GICascades.Value > 0 && GICascades.Value <= 6 )
			{
				var cameraPosition = context.owner.CameraSettings.Position;
				var cascadeDistances = GIGetShadowCascadeSplitDistances( context );

				var giData = new GIData();
				giData.VoxelizationConservative = (float)GIVoxelizationConservative.Value;
				giData.GridSize = GetGIGridSizeEnumAsInteger( GIGridSize );

				giData.Cascades = new GIData.CascadeItem[ GICascades.Value ];

				for( int nCascade = 0; nCascade < giData.Cascades.Length; nCascade++ )
				{
					var cascadeItem = new GIData.CascadeItem();

					var distance = GIDistance.Value;

					var index = nCascade + 1;
					if( index < cascadeDistances.Length )
						distance = cascadeDistances[ index ];

					cascadeItem.CellSize = (float)( distance * 2 / giData.GridSize );
					cascadeItem.CellSizeInv = 1.0f / cascadeItem.CellSize;

					var centerRelative = new Vector3( cameraPosition.X % cascadeItem.CellSize, cameraPosition.Y % cascadeItem.CellSize, cameraPosition.Z % cascadeItem.CellSize );
					cascadeItem.BoundsRelative = ( -centerRelative + new Bounds( -distance, -distance, -distance, distance, distance, distance ) ).ToBoundsF();

					cascadeItem.BoundsWorld = cameraPosition + cascadeItem.BoundsRelative.ToBounds();

					giData.Cascades[ nCascade ] = cascadeItem;
				}

				context.FrameData.GIData = giData;
			}
			else
			{
				//delete resources
				GIDestroyPreviousGridTexture( context );
			}
		}

		static ImageComponent GICreatePreviousGridTexture( Vector3I size )
		{
			ImageComponent texture = ComponentUtility.CreateComponent<ImageComponent>( null, true, false );
			texture.CreateType = ImageComponent.TypeEnum._3D;
			texture.CreateSize = size.ToVector2I();
			texture.CreateDepth = size.Z;
			texture.CreateFormat = giColorGridFormat;
			texture.CreateUsage = ImageComponent.Usages.ComputeWrite | ImageComponent.Usages.WriteOnly;
			texture.Enabled = true;

			return texture;
		}

		static void GIDestroyPreviousGridTexture( ViewportRenderingContext context )
		{
			var anyDataKey = "GIPreviousGrid";

			context.AnyDataAutoDispose.TryGetValue( anyDataKey, out var current );
			if( current != null )
			{
				context.AnyDataAutoDispose.Remove( anyDataKey );
				current.Dispose();
			}
		}

		static void GIGetOrCreatePreviousGridTexture( ViewportRenderingContext context )
		{
			var giData = context.FrameData.GIData;
			var gridSize = giData.GridSize;
			var cascades = giData.Cascades;

			var anyDataKey = "GIPreviousGrid";
			var demandedSize = new Vector3I( gridSize * cascades.Length, gridSize, gridSize );

			//check to destroy
			{
				context.AnyDataAutoDispose.TryGetValue( anyDataKey, out var current );
				var current2 = current as ImageComponent;

				if( current2 != null && new Vector3I( current2.Result.ResultSize, current2.Result.ResultDepth ) != demandedSize )
					GIDestroyPreviousGridTexture( context );
			}

			//create and get
			{
				context.AnyDataAutoDispose.TryGetValue( anyDataKey, out var current );
				var current2 = current as ImageComponent;

				if( current2 == null )
				{
					//create
					current2 = GICreatePreviousGridTexture( demandedSize );
					context.AnyDataAutoDispose[ anyDataKey ] = current2;
					giData.PreviousGrid2TextureJustCreated = true;
				}

				giData.PreviousGrid2Texture = current2;
			}
		}

		protected internal unsafe virtual void GIPrepare( ViewportRenderingContext context )
		{
			//already enabled
			////enable view for compute operations
			//context.SetComputePass();

			var giData = context.FrameData.GIData;
			var gridSize = giData.GridSize;
			var cascades = giData.Cascades;

			GIGetOrCreatePreviousGridTexture( context );

			giData.Grid1Texture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize * cascades.Length, gridSize, gridSize ), PixelFormat.R32_UInt, 0, false );
			if( giData.Grid1Texture == null )
				return;

			giData.Grid2Texture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize * cascades.Length, gridSize, gridSize ), giColorGridFormat, 0, false );
			if( giData.Grid2Texture == null )
				return;

			giData.Grid2TextureIndirected = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize * cascades.Length, gridSize, gridSize ), giColorGridFormat, 0, false );
			if( giData.Grid2TextureIndirected == null )
				return;

			giObjectDataUniform = GpuProgramManager.RegisterUniform( "giObjectData", UniformType.Vector4, 5 );
			giRenderToGridOperationDataUniform = GpuProgramManager.RegisterUniform( "giRenderToGridOperationData", UniformType.Vector4, 1 );
			giMeshBoundsUniform = GpuProgramManager.RegisterUniform( "giMeshBounds", UniformType.Vector4, 2 );

			if( !giStandardGIVoxelPrepareProgram.HasValue )
			{
				var program = GpuProgramManager.GetProgram( "Standard_GI_VoxelPrepare", GpuProgramType.Compute, @"Base\Shaders\MaterialStandard_GI_VoxelPrepare.sc", null, true, out var error2 );
				if( !string.IsNullOrEmpty( error2 ) )
					Log.Fatal( error2 );
				else
					giStandardGIVoxelPrepareProgram = new Program( program.RealObject );
			}

			GIClearGrids( context );

			for( var cascade = 0; cascade < cascades.Length; cascade++ )
			{
				GIRenderScene( context, cascade );
				GICalculateGridAcceleration( context, cascade );
			}

			for( var cascade = 0; cascade < cascades.Length; cascade++ )
				GICalculateIndirectLighting( context, cascade );

			GICopyToPreviousGrid( context );
		}

		protected internal unsafe virtual void GIClearGrids( ViewportRenderingContext context )
		{
			var giData = context.FrameData.GIData;
			var gridSize = giData.GridSize;
			var cascades = giData.Cascades;

			if( !giClearGridsProgram.HasValue )
			{
				var program = GpuProgramManager.GetProgram( "GIClearGrids", GpuProgramType.Compute, @"Base\Shaders\GI\GI1ClearGrids.sc", null, true, out var error2 );
				if( !string.IsNullOrEmpty( error2 ) )
					Log.Fatal( error2 );
				else
					giClearGridsProgram = new Program( program.RealObject );
			}

			{
				context.BindComputeImage( 0, giData.Grid1Texture, 0, ComputeBufferAccessEnum.Write );
				context.BindComputeImage( 1, giData.Grid2Texture, 0, ComputeBufferAccessEnum.Write );

				var jobSize = new Vector3I( gridSize * cascades.Length / 8, gridSize / 8, gridSize / 8 );

				Bgfx.Dispatch( (ushort)context.CurrentViewNumber, giClearGridsProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
				context.UpdateStatisticsCurrent.ComputeDispatches++;
			}

			if( giData.PreviousGrid2TextureJustCreated )
			{
				context.BindComputeImage( 0, giData.Grid1Texture, 0, ComputeBufferAccessEnum.Write );
				context.BindComputeImage( 1, giData.PreviousGrid2Texture, 0, ComputeBufferAccessEnum.Write );

				var jobSize = new Vector3I( gridSize * cascades.Length / 8, gridSize / 8, gridSize / 8 );

				Bgfx.Dispatch( (ushort)context.CurrentViewNumber, giClearGridsProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
				context.UpdateStatisticsCurrent.ComputeDispatches++;
			}
		}

		static GIInstanceBuffersItem GIGetInstanceBuffers( int instanceCount )
		{
			var allocateCount = MathEx.NextPowerOfTwo( instanceCount );

			if( !giInstanceBuffers.TryGetValue( allocateCount, out var item ) )
			{
				item = new GIInstanceBuffersItem();

				//item.IndirectBuffer = new IndirectBuffer( allocateCount );

				item.InstanceDataBuffer = GpuBufferManager.CreateVertexBuffer( allocateCount, GpuBufferManager.GIInstanceDataBufferDeclaration, GpuBufferFlags.Dynamic | GpuBufferFlags.ComputeRead | GpuBufferFlags.ComputeWrite );

				giInstanceBuffers.Add( allocateCount, item );
			}

			return item;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		public unsafe void GIRenderOperation( ViewportRenderingContext context, GIData giData, Program giVoxelProgram, RenderSceneData.ObjectInstanceData* objectData, RenderSceneData.CutVolumeItem[] cutVolumes, GpuVertexBuffer instancingVertexBuffer, InstanceDataBuffer* instancingDataBuffer, int instancingDataBufferStart, int instancingCount, int cascade, float cellSizeInv, float maxRadius )
		{
			SetCutVolumeSettingsUniforms( context, cutVolumes, false );

			bool useInstancingBuffer;
			int instanceCount;
			int instancingBufferStart;

			if( instancingVertexBuffer != null )
			{
				context.BindComputeBuffer( 5, instancingVertexBuffer, ComputeBufferAccessEnum.Read );

				useInstancingBuffer = true;
				instanceCount = instancingCount;
				instancingBufferStart = 0;
			}
			else if( instancingDataBuffer != null )
			{
				NativeMethods.bgfx_set_compute_vertex_buffer( 5, instancingDataBuffer->data.handle, ComputeBufferAccess.Read );
				//NativeMethods.bgfx_set_compute_dynamic_vertex_buffer( 5, instancingDataBuffer->data.handle, ComputeBufferAccess.Read );

				useInstancingBuffer = true;
				instanceCount = instancingCount;//instancingDataBuffer.data.num;
				instancingBufferStart = instancingDataBufferStart;//instancingDataBuffer->data.offset / 16;
			}
			else
			{
				//!!!!пустоту в буфер 5?

				Bgfx.SetUniform( giObjectDataUniform, objectData, 5 );

				useInstancingBuffer = false;
				instanceCount = 1;
				instancingBufferStart = 0;
			}

			var renderToGridInstancing = new Vector4F( useInstancingBuffer ? 1 : 0, instanceCount, instancingBufferStart, maxRadius );
			Bgfx.SetUniform( giRenderToGridOperationDataUniform, &renderToGridInstancing );

			var instanceBuffers = GIGetInstanceBuffers( instanceCount );
			//var indirectBuffer = instanceBuffers.IndirectBuffer;
			var instanceDataBuffer = instanceBuffers.InstanceDataBuffer;

			//prepare indirect buffer and data for instances
			{
				//Bgfx.SetComputeBuffer( 0, indirectBuffer, ComputeBufferAccess.Write );
				context.BindComputeBuffer( 1, instanceDataBuffer, ComputeBufferAccessEnum.Write );

				//!!!!может другие тоже выключить. где еще
				var discardFlags = DiscardFlags.None;// /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

				var jobSizeX = (int)Math.Ceiling( instanceCount / 32.0 );
				Bgfx.Dispatch( (ushort)context.CurrentViewNumber, giStandardGIVoxelPrepareProgram.Value, jobSizeX, 1, 1, discardFlags );
				context.UpdateStatisticsCurrent.ComputeDispatches++;

				//restore bindings
				//BindBonesTexture( context, context.FrameData );
				BindMaterialsTexture( context, context.FrameData );
			}

			//render
			{
				context.BindComputeBuffer( 7, instanceDataBuffer, ComputeBufferAccessEnum.Read );

				var size = maxRadius * 2 * cellSizeInv;
				if( size > giData.GridSize )
					size = giData.GridSize;

				var v = (int)Math.Ceiling( size / 8.0 );
				var jobSize = new Vector3I( v, v, v );

				////var size = gridIndexes.GetSize() + new Vector3I( 1, 1, 1 );
				////var jobSize = new Vector3I( (int)Math.Ceiling( size.X / 8.0 ), (int)Math.Ceiling( size.Y / 8.0 ), (int)Math.Ceiling( size.Z / 8.0 ) );

				var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

				Bgfx.Dispatch( (ushort)context.CurrentViewNumber, giVoxelProgram, jobSize.X, jobSize.Y, jobSize.Z, discardFlags );
				context.UpdateStatisticsCurrent.ComputeDispatches++;

				//Bgfx.Dispatch( context.CurrentViewNumber, giVoxelProgram.Value, indirectBuffer, 0, instanceCount, discardFlags );
				//context.UpdateStatisticsCurrent.ComputeDispatches++;
			}



			////var renderToGridInstancing = new Vector4F( useInstancingBuffer ? 1 : 0, instanceCount, instancingBufferStart, maxRadius );
			////Bgfx.SetUniform( giRenderToGridOperationDataUniform, &renderToGridInstancing );

			//////!!!!cull by cell indexes in better in some cases

			////var size = maxRadius * 2 * cellSizeInv;
			//////!!!!?
			////if( size > giData.GridSize )
			////	size = giData.GridSize;

			////var v = (int)Math.Ceiling( size / 8.0 );
			////var jobSize = new Vector3I( v, v, v );

			//////var size = gridIndexes.GetSize() + new Vector3I( 1, 1, 1 );
			//////var jobSize = new Vector3I( (int)Math.Ceiling( size.X / 8.0 ), (int)Math.Ceiling( size.Y / 8.0 ), (int)Math.Ceiling( size.Z / 8.0 ) );

			////var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

			////Bgfx.Dispatch( context.CurrentViewNumber, giVoxelProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, discardFlags );
			////context.UpdateStatisticsCurrent.ComputeDispatches++;



			////var data = context.FrameData.IndirectLightingFrameData;

			////var cellSizeOfLevel = data.CellSizeLevel0 * MathEx.Pow( 2.0f, level );
			////var gridSizeHalf = cellSizeOfLevel * data.GridResolution * 0.5f;
			////var gridPosition = data.GridCenter - new Vector3( gridSizeHalf, gridSizeHalf, gridSizeHalf );
			////var voxelizationFactor = (float)data.Owner.VoxelizationFactor.Value;

			////{
			////	var parameters = new Vector4F( cellSizeOfLevel, voxelizationFactor, data.GridResolution, level );
			////	if( giRenderToGridParametersUniform == null )
			////		giRenderToGridParametersUniform = GpuProgramManager.RegisterUniform( "giRenderToGridParameters", UniformType.Vector4, 1 );
			////	Bgfx.SetUniform( giRenderToGridParametersUniform.Value, &parameters );
			////}

			////{
			////	//!!!!double
			////	var v = new Vector4F( gridPosition.ToVector3F(), 0 );
			////	var uniform = GpuProgramManager.RegisterUniform( "gridPosition", UniformType.Vector4, 1 );
			////	Bgfx.SetUniform( uniform, &v );
			////}

			////{
			////	var v = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
			////	var uniform = GpuProgramManager.RegisterUniform( "giGridIndexesMin", UniformType.Vector4, 1 );
			////	Bgfx.SetUniform( uniform, &v );
			////}
			////{
			////	var v = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );
			////	var uniform = GpuProgramManager.RegisterUniform( "giGridIndexesMax", UniformType.Vector4, 1 );
			////	Bgfx.SetUniform( uniform, &v );
			////}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		unsafe void GISetMeshBoundsUniform( ref Bounds meshBounds )
		{
			Vector4F* points = stackalloc Vector4F[ 2 ];
			points[ 0 ] = new Vector4F( meshBounds.Minimum.ToVector3F(), 0 );
			points[ 1 ] = new Vector4F( meshBounds.Maximum.ToVector3F(), 0 );
			Bgfx.SetUniform( giMeshBoundsUniform, points, 2 );
		}

		protected internal unsafe virtual void GIRenderScene( ViewportRenderingContext context, int cascade )
		{
			//!!!!slowly? выборка из всех это долго?

			var frameData = context.FrameData;
			var giData = frameData.GIData;

			var viewportOwner = context.Owner;

			var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
			var frameDataRenderSceneDataBillboards = frameData.RenderSceneData.Billboards;
			var frameDataMeshes = frameData.Meshes;
			var frameDataBillboards = frameData.Billboards;

			var cascadeItem = giData.Cascades[ cascade ];
			var cascadeBoundsRelative = cascadeItem.BoundsRelative;
			var cascadeBoundsWorld = cascadeItem.BoundsWorld;

			using( var renderableGroupsToDraw = new OpenListNative<RenderableGroupWithDistance>( frameData.RenderableGroupsInFrustum.Count + frameData.RenderableGroupsForGI.Count ) )
			{
				//RenderableGroupsInFrustum
				foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
				{
					bool add = false;

					if( renderableGroup.X == 0 )
					{
						ref var data = ref frameDataMeshes.Data[ renderableGroup.Y ];

						if( ( data.Flags & FrameData.MeshItem.FlagsEnum.UseGI ) != 0 )
						{
							ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];

							//!!!!отсекать внутрений бокс

							//!!!!может BoundingBox быстрее проверять
							if( cascadeBoundsWorld.Intersects( ref meshItem.BoundingSphere ) )
								add = true;
						}
					}
					else
					{
						ref var data = ref frameDataBillboards.Data[ renderableGroup.Y ];

						if( ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseGI ) != 0 )
						{
							ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup.Y ];
							if( cascadeBoundsWorld.Intersects( ref billboardItem.BoundingSphere ) )
								add = true;
						}
					}

					if( add )
					{
						//renderableGroupsToDraw.Add( renderableGroup );

						var item = new RenderableGroupWithDistance();
						item.RenderableGroup = renderableGroup;
						item.DistanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.RenderableGroup );
						renderableGroupsToDraw.Add( ref item );
					}
				}

				//RenderableGroupsForGI
				foreach( var renderableGroup in frameData.RenderableGroupsForGI )
				{
					bool add = false;

					if( renderableGroup.X == 0 )
					{
						ref var data = ref frameDataMeshes.Data[ renderableGroup.Y ];

						if( ( data.Flags & FrameData.MeshItem.FlagsEnum.UseGI ) != 0 )
						{
							ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];
							if( cascadeBoundsWorld.Intersects( ref meshItem.BoundingSphere ) )
								add = true;
						}
					}
					else
					{
						ref var data = ref frameDataBillboards.Data[ renderableGroup.Y ];

						if( ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseGI ) != 0 )
						{
							ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableGroup.Y ];
							if( cascadeBoundsWorld.Intersects( ref billboardItem.BoundingSphere ) )
								add = true;
						}
					}

					if( add )
					{
						//renderableGroupsToDraw.Add( renderableGroup );

						var item = new RenderableGroupWithDistance();
						item.RenderableGroup = renderableGroup;
						item.DistanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.RenderableGroup );
						renderableGroupsToDraw.Add( ref item );
					}
				}

				if( renderableGroupsToDraw.Count == 0 )
					return;


				//!!!!distance can be behind camera

				//sort by distance to prevent flickering
				CollectionUtility.MergeSortUnmanaged( renderableGroupsToDraw.Data, renderableGroupsToDraw.Count, delegate ( RenderableGroupWithDistance* a, RenderableGroupWithDistance* b )
				{
					if( a->DistanceSquared < b->DistanceSquared )
						return -1;
					if( a->DistanceSquared > b->DistanceSquared )
						return 1;
					return 0;
				}, true );

				var gridPositionRelative = cascadeBoundsRelative.Minimum;
				var gridSize = giData.GridSize;
				var gridPositionWorld = context.Owner.CameraSettings.Position + gridPositionRelative;

				//giRenderToGridParameters
				{
					var clipDistance = 0.0;
					if( cascade > 0 )
					{
						var previousCascade = giData.Cascades[ cascade - 1 ];

						//!!!!calculate better

						clipDistance = previousCascade.CellSize * giData.GridSize / 2.0f - previousCascade.CellSize * 0.5;
						if( clipDistance < 0 )
							clipDistance = 0;
					}

					//var emissiveLighting = 1.0f;
					//var emissiveLighting = indirectData.Owner.EmissiveLighting.Value;

					var parameters = stackalloc Vector4F[ 2 ];
					parameters[ 0 ] = new Vector4F( cascadeItem.CellSize, giData.VoxelizationConservative, gridSize, cascade );
					parameters[ 1 ] = new Vector4F( gridPositionRelative, (float)clipDistance );
					//parameters[ 1 ] = new Vector4F( gridPositionRelative, (float)emissiveLighting );
					var uniform = GpuProgramManager.RegisterUniform( "giRenderToGridParameters", UniformType.Vector4, 2 );
					Bgfx.SetUniform( uniform, parameters, 2 );
				}

				//bind textures for all render operations
				context.BindComputeImage( 0, giData.Grid1Texture, 0, ComputeBufferAccessEnum.ReadWrite );
				context.BindComputeImage( 2, giData.Grid2Texture, 0, ComputeBufferAccessEnum.Write );
				SetCutVolumeSettingsUniforms( context, null, true );
				BindBrdfLUT( context );//, true );
				BindMaterialsTexture( context, frameData );//, true );

				//!!!!impl
				//BindBonesTexture( context, frameData, true );

				BindSamplersForTextureOnlySlots( context, true, true );
				BindMaterialData( context, null, false, true );
				BindForwardLightAndShadows( context, frameData, true );

				//!!!!parallel is possible?
				var sectorsByDistance = 1;

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

							ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.RenderableGroup.Y ];

							var meshData = meshItem.MeshDataLOD0 ?? meshItem.MeshData;
							var lods = meshData.LODs;
							if( lods != null )
								meshData = lods[ lods.Length - 1 ].Mesh?.Result?.MeshData ?? meshData;

							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
							{
								var oper = meshData.RenderOperations[ nOperation ];

								if( oper.VoxelDataInfo != null )
								{
									foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, false, true ) )
									{
										if( materialData.giSupport && materialData.giVoxelProgram != Program.Invalid )
										{
											//!!!!так?
											bool instancing = Instancing && meshItem.AnimationData == null && !meshItem.InstancingEnabled && meshItem.CutVolumes == null;
											manager.Add( renderableGroup.RenderableGroup, nOperation, oper, materialData, instancing );
										}
									}
								}
							}
						}
						else if( renderableGroup.RenderableGroup.X == 1 )
						{
							//billboards

							ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.RenderableGroup.Y ];
							var meshData = Billboard.GetBillboardMesh().Result.MeshData;

							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
							{
								var oper = meshData.RenderOperations[ nOperation ];

								if( oper.VoxelDataInfo != null )
								{
									foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, false, true ) )
									{
										if( materialData.giSupport && materialData.giVoxelProgram != Program.Invalid )
										{
											//!!!!так?
											bool instancing = Instancing && billboardItem.CutVolumes == null;
											manager.Add( renderableGroup.RenderableGroup, nOperation, oper, materialData, instancing );
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

							if( Instancing && outputItem.renderableItemsCount >= 2 )
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

									ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, true/*!!!! nRenderableItem == 0*/ );

									GISetMeshBoundsUniform( ref meshData.SpaceBounds.boundingBox );

									BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo, null, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative/*!!!!, true*/ );

									//pass = materialData.deferredShadingPass.Get( outputItem.operation.VoxelDataInfo != null, meshData.BillboardMode != 0 );

									int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
									int instanceCount = outputItem.renderableItemsCount;

									ref var meshSphere = ref meshData.SpaceBounds.boundingSphere;
									var meshRadius = (float)( meshSphere.Center.Length() + meshSphere.Radius );

									//!!!!
									//if( EngineApp._DebugCapsLock )
									//	Log.Info( "dynamic: " + instanceCount.ToString() );


									var instanceDataBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
									if( instanceDataBuffer.Valid )
									{
										var maxRadius = 0.0f;

										//get instancing matrices
										RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceDataBuffer.Data;
										int currentMatrix = 0;
										for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
										{
											var renderableItem = outputItem.renderableItems[ nRenderableItem ];

											//if( renderableItem.X == 0 )
											//{

											//meshes
											ref var meshItem2 = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
											meshItem2.GetInstancingData( out instancingData[ currentMatrix ] );

											var maxRadius2 = meshRadius * instancingData[ currentMatrix ].TransformRelative.DecomposeScaleMaxComponent();
											if( maxRadius2 > maxRadius )
												maxRadius = maxRadius2;

											currentMatrix++;

											//}
											//else if( renderableItem.X == 1 )
											//{
											//	//billboards
											//	ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
											//	billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
											//}
										}

										var instancingDataBufferStart = instanceDataBuffer.data.offset / 16;

										GIRenderOperation( context, giData, materialData.giVoxelProgram, null, meshItem.CutVolumes, null, &instanceDataBuffer, instancingDataBufferStart, instanceCount, cascade, cascadeItem.CellSizeInv, maxRadius );
									}


									//////last working not optimized
									////for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
									////{
									////	var renderableItem = outputItem.renderableItems[ nRenderableItem ];
									////	ref var meshItem2 = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];

									////	meshItem2.GetInstancingData( out var objectData );

									////	var maxRadius = meshRadius * meshItem2.TransformRelative.DecomposeScaleMaxComponent();

									////	//var gridIndexes = new BoundsI( 0, 0, 0, giData.GridSize, giData.GridSize, giData.GridSize );

									////	//meshItem2.BoundingSphere.ToBounds( out var bounds );
									////	//if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
									////	{
									////		GIRenderOperation( context, giData, materialData.giVoxelProgram, &objectData/*, ref gridIndexes*/, meshItem.CutVolumes, null, 1, cascade, cascadeItem.CellSizeInv, maxRadius );
									////	}
									////}




									////var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );
									////if( instanceBuffer.Valid )
									////{
									////	//get instancing matrices
									////	RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
									////	int currentMatrix = 0;
									////	for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
									////	{
									////		var renderableItem = outputItem.renderableItems[ nRenderableItem ];

									////		if( renderableItem.X == 0 )
									////		{
									////			//meshes
									////			ref var meshItem2 = ref frameDataRenderSceneDataMeshes.Data[ renderableItem.Y ];
									////			meshItem2.GetInstancingData( out instancingData[ currentMatrix++ ] );
									////		}
									////		else if( renderableItem.X == 1 )
									////		{
									////			//billboards
									////			ref var billboardItem = ref frameDataRenderSceneDataBillboards.Data[ renderableItem.Y ];
									////			billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
									////		}
									////	}

									////	//!!!!temp
									////	var gridIndexes = new BoundsI( 0, 0, 0, giData.GridSize, giData.GridSize, giData.GridSize );

									////	GIRenderOperation( context, materialData.giVoxelProgram, null, ref gridIndexes, meshItem.CutVolumes, null, ref instanceBuffer );
									////}



									////int instanceCount = outputItem.renderableItemsCount;
									////for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
									////{
									////	var renderableItem = outputItem.renderableItems[ nRenderableItem ];
									////	ref var meshItem2 = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];

									////	meshItem2.BoundingSphere.ToBounds( out var bounds );
									////	if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
									////	{
									////		var objectData = new GIObjectsData();
									////		meshItem2.TransformRelative.GetTranspose( out objectData.Transform );
									////		objectData.Color = meshItem2.Color;
									////		objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
									////		objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

									////		RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
									////	}
									////}

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

										//!!!!один раз вызвать. везде так
										ForwardBindGeneralTexturesUniforms( context, frameData/*, ref meshItem.BoundingSphere, lightItem, receiveShadows*/, true/*!!!! nRenderableItem == 0*/ );

										GISetMeshBoundsUniform( ref meshData.SpaceBounds.boundingBox );

										BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo, meshItem.ObjectInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative/*!!!!, true*/ );

										//!!!!slowly
										//!!!!можно точнее считать? везде так


										//!!!!batching

										//var pass = materialData.deferredShadingPass.Get( outputItem.operation.VoxelDataInfo != null/*, outputItem.operation.VirtualizedData != null*/, meshData.BillboardMode != 0 );


										//!!!!slowly batching

										if( meshItem.InstancingEnabled )
										{
											if( meshItem.InstancingVertexBuffer != null )
											{
												var instancingCount = meshItem.InstancingCount;
												if( instancingCount < 0 )
													instancingCount = meshItem.InstancingVertexBuffer.VertexCount;

												ref var meshSphere = ref meshData.SpaceBounds.boundingSphere;
												var meshRadius = (float)( meshSphere.Center.Length() + meshSphere.Radius );

												//!!!!slowly? precalculate?
												float maxRadius = 0;

												var array = meshItem.InstancingVertexBuffer.Vertices;
												if( array != null )
												{
													fixed( byte* pArray = array )
													{
														var instancingData = (RenderSceneData.ObjectInstanceData*)pArray;

														for( int nObject = 0; nObject < instancingCount; nObject++ )
														{
															var instancingData2 = instancingData + nObject + meshItem.InstancingStart;

															var maxRadius2 = meshRadius * instancingData2->TransformRelative.DecomposeScaleMaxComponent();
															if( maxRadius2 > maxRadius )
																maxRadius = maxRadius2;
														}
													}
												}

												////var gridIndexes = new BoundsI( 0, 0, 0, giData.GridSize, giData.GridSize, giData.GridSize );
												////objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
												////objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

												GIRenderOperation( context, giData, materialData.giVoxelProgram, null, /*, ref gridIndexes*/ meshItem.CutVolumes, meshItem.InstancingVertexBuffer, null, 0, instancingCount, cascade, cascadeItem.CellSizeInv, maxRadius );

												//!!!!
												//if( EngineApp._DebugCapsLock )
												//	Log.Info( "vertex buffer: " + instancingCount.ToString() );
											}
											else
											{

												//if( !EngineApp._DebugCapsLock )
												//{

												var instancingCount = meshItem.InstancingCount;
												if( instancingCount < 0 )
													instancingCount = meshItem.InstancingDataBuffer.Size;

												var maxRadius = 0.0f;
												{
													var instancingData = (RenderSceneData.ObjectInstanceData*)meshItem.InstancingDataBuffer.Data + meshItem.InstancingStart;

													ref var meshSphere = ref meshData.SpaceBounds.boundingSphere;
													var meshRadius = (float)( meshSphere.Center.Length() + meshSphere.Radius );

													for( int nObject = 0; nObject < instancingCount; nObject++ )
													{
														var maxRadius2 = meshRadius * instancingData[ nObject ].TransformRelative.DecomposeScaleMaxComponent();
														if( maxRadius2 > maxRadius )
															maxRadius = maxRadius2;
													}
												}

												var buffer = meshItem.InstancingDataBuffer;
												var instancingDataBufferStart = ( buffer.data.offset + meshItem.InstancingStart * sizeof( RenderSceneData.ObjectInstanceData ) ) / 16;

												GIRenderOperation( context, giData, materialData.giVoxelProgram, null, meshItem.CutVolumes, null, &buffer, instancingDataBufferStart, instancingCount, cascade, cascadeItem.CellSizeInv, maxRadius );

												//}
												//else
												//{

												//	var instancingCount = meshItem.InstancingCount;
												//	if( instancingCount < 0 )
												//		instancingCount = meshItem.InstancingDataBuffer.Size;

												//	var instancingData = (RenderSceneData.ObjectInstanceData*)meshItem.InstancingDataBuffer.Data + meshItem.InstancingStart;

												//	ref var meshSphere = ref meshData.SpaceBounds.boundingSphere;
												//	var meshRadius = (float)( meshSphere.Center.Length() + meshSphere.Radius );

												//	for( int nObject = 0; nObject < instancingCount; nObject++ )
												//	{
												//		var instancingData2 = instancingData + nObject;// + meshItem.InstancingStart;

												//		var maxRadius = meshRadius * instancingData2->TransformRelative.DecomposeScaleMaxComponent();
												//		////instancingData2->TransformRelative.DecomposeScale( out var scale );
												//		////var maxRadius = meshRadius * scale.MaxComponent();

												//		//!!!!
												//		//instancingData2->TransformRelative.GetTranslation( out var pos );

												//		//var bounds = new Bounds( pos );
												//		//bounds.Expand( meshItem.InstancingMaxLocalBounds );

												//		//!!!!GIGetGridIndexes

												//		//!!!!
												//		//if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
												//		{
												//			//var gridIndexes = new BoundsI( 0, 0, 0, giData.GridSize, giData.GridSize, giData.GridSize );

												//			GIRenderOperation( context, giData, materialData.giVoxelProgram, instancingData2/*, ref gridIndexes*/, meshItem.CutVolumes, null, null, 0, 1, cascade, cascadeItem.CellSizeInv, maxRadius );
												//		}

												//		//if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
												//		//{
												//		//	var objectData = new GIObjectsData();
												//		//	objectData.TransformRelative = instancingData2->TransformRelative;

												//		//	//!!!!
												//		//	var c = instancingData2->Color.ToColorValue();// / 255.0f;
												//		//	objectData.Color.Red = MathEx.Pow( c.Red, 2 ) * 10.0f;
												//		//	objectData.Color.Green = MathEx.Pow( c.Green, 2 ) * 10.0f;
												//		//	objectData.Color.Blue = MathEx.Pow( c.Blue, 2 ) * 10.0f;
												//		//	objectData.Color.Alpha = MathEx.Pow( c.Alpha, 2 ) * 10.0f;

												//		//	objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
												//		//	objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

												//		//	GIRenderOperation( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
												//		//}

												//		//#endif

												//	}

												//}


												//!!!!
												//if( EngineApp._DebugCapsLock )
												//	Log.Info( "transient buffer: " + instancingCount.ToString() );

											}
										}
										else
										{

											//meshItem.BoundingSphere.ToBounds( out var bounds );

											//meshItem.TransformRelative.get

											//!!!!slowly. maybe save source scale

											ref var meshSphere = ref meshData.SpaceBounds.boundingSphere;
											var meshRadius = (float)( meshSphere.Center.Length() + meshSphere.Radius );

											var maxRadius = meshRadius * meshItem.TransformRelative.DecomposeScaleMaxComponent();

											//!!!!
											//if( giData.GIGetGridIndexes( ref gridPositionWorld, cellSizeInv/*cellSizeOfLevelInv*/, ref bounds, out var gridIndexes ) )
											//if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
											{

												//var gridIndexes = new BoundsI( 0, 0, 0, giData.GridSize, giData.GridSize, giData.GridSize );

												//!!!!для gi необязательно внутри всё копировать
												meshItem.GetInstancingData( out var objectData );

												//objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
												//objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

												GIRenderOperation( context, giData, materialData.giVoxelProgram, &objectData/*, ref gridIndexes*/, meshItem.CutVolumes, null, null, 0, 1, cascade, cascadeItem.CellSizeInv, maxRadius );
											}

											//!!!!
											//if( EngineApp._DebugCapsLock )
											//	Log.Info( "separate" );

											//!!!!
											//if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
											//{
											//	var objectData = new GIObjectsData();
											//	meshItem.TransformRelative.GetTranspose( out objectData.TransformRelative );
											//	objectData.Color = meshItem.Color;
											//	objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
											//	objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

											//	GIRenderOperation( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
											//}

										}

										//RenderOperation( context, outputItem.operation, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount );

									}
									else if( renderableItem.X == 1 )
									{
										//billboards
										ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
										var meshData = Billboard.GetBillboardMesh().Result.MeshData;

										//!!!!

										//ForwardBindGeneralTexturesUniforms( context, frameData/*, ref billboardItem.BoundingSphere, lightItem, receiveShadows*/, nRenderableItem == 0 );

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
					//					foreach( var materialData in GetLayerMaterialData( ref layer, true, true, qqtrue ) )
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

		protected internal unsafe virtual void GICalculateGridAcceleration( ViewportRenderingContext context, int cascade )
		{
			var giData = context.FrameData.GIData;
			var gridSize = giData.GridSize;
			var cascades = giData.Cascades;

			var smallGridSize = gridSize / 4;
			var smallGridTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( smallGridSize, smallGridSize, smallGridSize ), PixelFormat.R32G32_UInt, 0, false );
			if( smallGridTexture == null )
				return;
			//context.ObjectsDuringUpdate.namedTextures[ "giSmallGrid" ] = smallGridTexture;

			var verySmallGridSize = gridSize / 16;
			var verySmallGridTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( verySmallGridSize, verySmallGridSize, verySmallGridSize ), PixelFormat.R32G32_UInt, 0, false );
			if( verySmallGridTexture == null )
				return;

			var sdf4Size = gridSize / 4;
			var sdf4Texture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( sdf4Size, sdf4Size, sdf4Size ), PixelFormat.Float32R, 0, false );
			if( sdf4Texture == null )
				return;

			//var grid1CopyTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize, gridSize, gridSize ), PixelFormat.R32_UInt, 0, false );
			//if( grid1CopyTexture == null )
			//	return;


			//for( var cascade = 0; cascade < cascades.Length; cascade++ )
			//{
			var cascadeItem = cascades[ cascade ];

			//calculate small grid
			{
				if( !giCalculateSmallGridProgram.HasValue )
				{
					var program = GpuProgramManager.GetProgram( "GICalculateSmallGrid", GpuProgramType.Compute, @"Base\Shaders\GI\GI2CalculateSmallGrid.sc", null, true, out var error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						Log.Fatal( error2 );
					else
						giCalculateSmallGridProgram = new Program( program.RealObject );
				}

				context.BindTexture( 0, giData.Grid1Texture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point );
				context.BindComputeImage( 1, smallGridTexture, 0, ComputeBufferAccessEnum.Write );

				var parameters = new Vector4F( gridSize * cascade, 0, 0, 0 );
				context.SetUniform( "giCalculateSmallGridParameters", ParameterType.Vector4, 1, &parameters );

				var jobSize = new Vector3I( smallGridSize / 8, smallGridSize / 8, smallGridSize / 8 );

				Bgfx.Dispatch( (ushort)context.CurrentViewNumber, giCalculateSmallGridProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
				context.UpdateStatisticsCurrent.ComputeDispatches++;
			}

			//calculate very small grid
			{
				if( !giCalculateVerySmallGridProgram.HasValue )
				{
					var program = GpuProgramManager.GetProgram( "GICalculateVerySmallGrid", GpuProgramType.Compute, @"Base\Shaders\GI\GI3CalculateVerySmallGrid.sc", null, true, out var error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						Log.Fatal( error2 );
					else
						giCalculateVerySmallGridProgram = new Program( program.RealObject );
				}

				context.BindTexture( 0, smallGridTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point );
				context.BindComputeImage( 1, verySmallGridTexture, 0, ComputeBufferAccessEnum.Write );

				var jobSize = new Vector3I( verySmallGridSize / 8, verySmallGridSize / 8, verySmallGridSize / 8 );

				Bgfx.Dispatch( (ushort)context.CurrentViewNumber, giCalculateVerySmallGridProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
				context.UpdateStatisticsCurrent.ComputeDispatches++;
			}

			//calculate sdf4
			{
				if( !giCalculateSDF4Program.HasValue )
				{
					var program = GpuProgramManager.GetProgram( "GICalculateSDF4", GpuProgramType.Compute, @"Base\Shaders\GI\GI4CalculateSDF4.sc", null, true, out var error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						Log.Fatal( error2 );
					else
						giCalculateSDF4Program = new Program( program.RealObject );
				}

				context.BindTexture( 0, verySmallGridTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );
				context.BindComputeImage( 1, sdf4Texture, 0, ComputeBufferAccessEnum.Write );

				var parameters = new Vector4F( sdf4Size, 0, 0, 0 );
				context.SetUniform( "giCalculateSDF4Parameters", ParameterType.Vector4, 1, &parameters );

				var jobSize = new Vector3I( sdf4Size / 8, sdf4Size / 8, sdf4Size / 8 );

				Bgfx.Dispatch( (ushort)context.CurrentViewNumber, giCalculateSDF4Program.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
				context.UpdateStatisticsCurrent.ComputeDispatches++;
			}

			////copy grid1 to temp copy
			//{
			//	if( !giCopyGrid1Program.HasValue )
			//	{
			//		var program = GpuProgramManager.GetProgram( "GICopyGrid1", GpuProgramType.Compute, @"Base\Shaders\GI\GICopyGrid1.sc", null, true, out var error2 );
			//		if( !string.IsNullOrEmpty( error2 ) )
			//			Log.Fatal( error2 );
			//		else
			//			giCopyGrid1Program = new Program( program.RealObject );
			//	}

			//	context.BindTexture( 0, giData.Grid1Texture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point );
			//	context.BindComputeImage( 1, grid1CopyTexture, 0, ComputeBufferAccessEnum.Write );

			//	var parameters = new Vector4F( gridSize * cascade, 0, 0, 0 );
			//	context.SetUniform( "giCopyGridParameters", ParameterType.Vector4, 1, &parameters );

			//	var jobSize = new Vector3I( gridSize / 8, gridSize / 8, gridSize / 8 );

			//	Bgfx.Dispatch( context.CurrentViewNumber, giCopyGrid1Program.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
			//	context.UpdateStatisticsCurrent.ComputeDispatches++;
			//}

			//calculate sdf
			{
				if( !giCalculateSDFProgram.HasValue )
				{
					var program = GpuProgramManager.GetProgram( "GICalculateSDF", GpuProgramType.Compute, @"Base\Shaders\GI\GI5CalculateSDF.sc", null, true, out var error2 );
					if( !string.IsNullOrEmpty( error2 ) )
						Log.Fatal( error2 );
					else
						giCalculateSDFProgram = new Program( program.RealObject );
				}

				context.BindTexture( 0, sdf4Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );
				//RO is not works
				//context.BindComputeImage( 0, sdf4Texture, 0, ComputeBufferAccessEnum.ReadWrite );
				//context.BindComputeImage( 0, sdf4Texture, 0, ComputeBufferAccessEnum.Read );

				context.BindComputeImage( 1, giData.Grid1Texture, 0, ComputeBufferAccessEnum.ReadWrite );
				//context.BindTexture( 2, grid1CopyTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point );
				context.BindTexture( 2, smallGridTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );

				var parameters = new Vector4F( gridSize, gridSize * cascade, 0, 0 );
				context.SetUniform( "giCalculateSDFParameters", ParameterType.Vector4, 1, &parameters );

				var jobSize = new Vector3I( gridSize / 8, gridSize / 8, gridSize / 8 );

				Bgfx.Dispatch( (ushort)context.CurrentViewNumber, giCalculateSDFProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
				context.UpdateStatisticsCurrent.ComputeDispatches++;
			}

			//}

			context.DynamicTexture_Free( smallGridTexture );
			context.DynamicTexture_Free( verySmallGridTexture );
			context.DynamicTexture_Free( sdf4Texture );
			//context.DynamicTexture_Free( grid1CopyTexture );
		}

		public unsafe static void GISetRayCastInfoUniform( ViewportRenderingContext context, bool accelerated )
		{
			var giData = context.FrameData.GIData;

			var array = stackalloc Vector4F[ 7 ];
			array[ 0 ] = new Vector4F( giData.Cascades.Length, giData.GridSize, accelerated ? 1 : 0, 0 );

			for( int cascade = 0; cascade < giData.Cascades.Length; cascade++ )
			{
				var cascadeItem = giData.Cascades[ cascade ];
				var gridPosition = cascadeItem.BoundsRelative.Minimum;
				var cellSize = cascadeItem.CellSize;

				array[ 1 + cascade ] = new Vector4F( gridPosition, cellSize );
			}

			context.SetUniform( "giRayCastInfo", ParameterType.Vector4, 5, array );
		}

		unsafe void GICalculateIndirectLighting( ViewportRenderingContext context, int cascade )
		{
			var giData = context.FrameData.GIData;
			var gridSize = giData.GridSize;
			//var cascades = giData.Cascades;

			if( !giCalculateIndirectLightingProgram.HasValue )
			{
				var program = GpuProgramManager.GetProgram( "GICalculateIndirectLighting", GpuProgramType.Compute, @"Base\Shaders\GI\GI6CalculateIndirectLighting.sc", null, true, out var error2 );
				if( !string.IsNullOrEmpty( error2 ) )
					Log.Fatal( error2 );
				else
					giCalculateIndirectLightingProgram = new Program( program.RealObject );
			}

			GISetRayCastInfoUniform( context, true );

			context.BindComputeImage( 0, giData.Grid2TextureIndirected, 0, ComputeBufferAccessEnum.Write );
			context.BindTexture( 1, giData.Grid1Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );
			context.BindTexture( 2, giData.Grid2Texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point );
			context.BindTexture( 3, giData.PreviousGrid2Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point );

			unchecked
			{
				giFrameCounter++;
			}

			var parameters = new Vector4F( gridSize, cascade, giFrameCounter, 0 );
			//var parameters = new Vector4F( gridSize, gridSize * cascade, giFrameCounter, 0 );
			context.SetUniform( "giCalculateIndirectLightingParameters", ParameterType.Vector4, 1, &parameters );

			var jobSize = new Vector3I( gridSize / 8, gridSize / 8, gridSize / 8 );

			Bgfx.Dispatch( (ushort)context.CurrentViewNumber, giCalculateIndirectLightingProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
			context.UpdateStatisticsCurrent.ComputeDispatches++;
		}

		void GICopyToPreviousGrid( ViewportRenderingContext context )
		{
			var giData = context.FrameData.GIData;
			var gridSize = giData.GridSize;
			var cascades = giData.Cascades;

			if( !giCopyToPreviousGridProgram.HasValue )
			{
				var program = GpuProgramManager.GetProgram( "GICopyToPreviousGrid", GpuProgramType.Compute, @"Base\Shaders\GI\GI7CopyToPreviousGrid.sc", null, true, out var error2 );
				if( !string.IsNullOrEmpty( error2 ) )
					Log.Fatal( error2 );
				else
					giCopyToPreviousGridProgram = new Program( program.RealObject );
			}

			context.BindTexture( 0, giData.Grid2TextureIndirected ?? giData.Grid2Texture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point );
			context.BindComputeImage( 1, giData.PreviousGrid2Texture, 0, ComputeBufferAccessEnum.Write );

			var jobSize = new Vector3I( gridSize * cascades.Length / 8, gridSize / 8, gridSize / 8 );

			Bgfx.Dispatch( (ushort)context.CurrentViewNumber, giCopyToPreviousGridProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
			context.UpdateStatisticsCurrent.ComputeDispatches++;
		}
	}
}









///////////////////////////////////////////////

////3 + 1 + 1 + 2 = 7 * 16 = 112
////7 vec4
//////3 + 1 + 2 = 6 * 16 = 96 bytes
//////6 vec4
//[StructLayout( LayoutKind.Sequential, Pack = 1 )]
//public struct GIObjectsData
//{
//	//48 bytes
//	public Matrix3x4F TransformRelative;

//	//16 bytes
//	public ColorValue Color;

//	//16 bytes
//	public float LodValue;
//	public float VisibilityDistance;
//	public float/*byte*/ ReceiveDecals;
//	public float unused;

//	////16 bytes
//	//public ColorByte Color;
//	//public float LodValue;
//	//public float VisibilityDistance;
//	//public float/*byte*/ ReceiveDecals;

//	////16 bytes
//	//	public Vector3F PreviousFramePositionChange;//public Vector3F PositionPreviousFrameRelative;
//	//public ColorByte Color;

//	////16 bytes
//	//public float LodValue;
//	//public float VisibilityDistance;
//	//public byte ReceiveDecals;
//	//public byte MotionBlurFactor;
//	//public byte Unused1;
//	//public byte Unused2;
//	//public uint CullingByCameraDirectionData;

//	//32 bytes
//	public Vector4F GridIndexesMin;
//	public Vector4F GridIndexesMax;
//}



//public bool GIGetGridIndexes( ref Vector3 gridPosition, double cellSizeOfLevelInv, ref Bounds bounds, out BoundsI gridIndexes )
////public bool GetGridIndexes( int level, ref Bounds bounds, out BoundsI gridIndexes )
//{
//	//var cellSizeOfLevel = CellSizeLevel0 * MathEx.Pow( 2.0f, level );
//	//var gridSizeHalf = cellSizeOfLevel * GridResolution * 0.5f;
//	//var gridPosition = GridCenter - new Vector3( gridSizeHalf, gridSizeHalf, gridSizeHalf );
//	//var cellSizeOfLevelInv = 1.0f / cellSizeOfLevel;

//	//!!!!temp так
//	var cellSize = 1.0 / cellSizeOfLevelInv;

//	//!!!!good?
//	var border = VoxelizationConservative * (float)cellSize;
//	var border3 = new Vector3F( border, border, border );

//	var indexMinF = ( bounds.Minimum - gridPosition ) * cellSizeOfLevelInv - border3;
//	var indexMin = indexMinF.ToVector3I();
//	var indexMaxF = ( bounds.Maximum - gridPosition ) * cellSizeOfLevelInv + border3;
//	var indexMax = indexMaxF.ToVector3I();

//	gridIndexes = new BoundsI( indexMin, indexMax );

//	if( indexMax.X < 0 || indexMax.Y < 0 || indexMax.Z < 0 )
//		return false;
//	if( indexMin.X >= GridSize || indexMin.Y >= GridSize || indexMin.Z >= GridSize )
//		return false;

//	if( gridIndexes.Minimum.X < 0 )
//		gridIndexes.Minimum.X = 0;
//	if( gridIndexes.Minimum.Y < 0 )
//		gridIndexes.Minimum.Y = 0;
//	if( gridIndexes.Minimum.Z < 0 )
//		gridIndexes.Minimum.Z = 0;
//	if( gridIndexes.Maximum.X >= GridSize )
//		gridIndexes.Maximum.X = GridSize - 1;
//	if( gridIndexes.Maximum.Y >= GridSize )
//		gridIndexes.Maximum.Y = GridSize - 1;
//	if( gridIndexes.Maximum.Z >= GridSize )
//		gridIndexes.Maximum.Z = GridSize - 1;

//	return true;
//}




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



//////////////////////////////////////////////////////



//var indirectData = context.FrameData.IndirectLightingFrameData;
//var cellSizeOfLevel = indirectData.CellSizeLevel0 * MathEx.Pow( 2.0f, level );
//var cellSizeOfLevelInv = 1.0f / cellSizeOfLevel;
//var gridSizeHalf = cellSizeOfLevel * indirectData.GridResolution * 0.5f;
//var gridPosition = indirectData.GridCenter - new Vector3( gridSizeHalf, gridSizeHalf, gridSizeHalf );
//var voxelizationFactor = (float)indirectData.Owner.VoxelizationExpandFactor.Value;
//var emissiveLighting = indirectData.Owner.EmissiveLighting.Value;

//var parameters = stackalloc Vector4F[ 2 ];
//parameters[ 0 ] = new Vector4F( cellSizeOfLevel, voxelizationFactor, indirectData.GridResolution, level );
////!!!!double
//parameters[ 1 ] = new Vector4F( gridPosition.ToVector3F(), (float)emissiveLighting );
//var uniform = GpuProgramManager.RegisterUniform( "giRenderToGridParameters", UniformType.Vector4, 2 );
//Bgfx.SetUniform( uniform, parameters, 2 );


//context.BindComputeImage( 0, giGBufferGridTexture, 0, ComputeBufferAccessEnum.Write );
//context.BindComputeImage( 1, giLightingGridTexture, 0, ComputeBufferAccessEnum.Write );
////context.BindComputeImage( 3, giGBufferGridTexture, 0, ComputeBufferAccessEnum.Write );
////context.BindComputeImage( 4, giLightingGridTexture, 0, ComputeBufferAccessEnum.Write );


////bind textures for all render operations

//SetCutVolumeSettingsUniforms( context, null, true );

////!!!!temp?
//BindMaterialsTexture( context, frameData, true );
////BindMaterialsTexture( context, frameData );

//BindBonesTexture( context, frameData, true );

//BindSamplersForTextureOnlySlots( context, true, true );
//BindMaterialData( context, null, false, true );


////prepare outputInstancingManagers
//Parallel.For( 0, sectorsByDistance, delegate ( int nSector )
//{
//	var manager = outputInstancingManagers[ nSector ];

//	int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorsByDistance );
//	int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorsByDistance );
//	if( nSector == sectorsByDistance - 1 )
//		indexTo = renderableGroupsToDraw.Count;

//	//fill output manager
//	for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
//	{
//		var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

//		if( renderableGroup/*.RenderableGroup*/.X == 0 )
//		{
//			//meshes

//			ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup/*.RenderableGroup*/.Y ];

//			var meshData = meshItem.MeshDataLOD0 ?? meshItem.MeshData;
//			var lods = meshData.LODs;
//			if( lods != null )
//				meshData = lods[ lods.Length - 1 ].Mesh?.Result?.MeshData ?? meshData;

//			for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
//			{
//				var oper = meshData.RenderOperations[ nOperation ];

//				if( oper.VoxelDataInfo != null )
//				{
//					foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, true ) )
//					{
//						if( materialData.deferredShadingSupport )
//						{
//							bool instancing = Instancing && meshItem.AnimationData == null && !meshItem.InstancingEnabled && meshItem.CutVolumes == null;
//							manager.Add( renderableGroup/*.RenderableGroup*/, nOperation, oper, materialData, instancing );
//						}
//					}
//				}
//			}
//		}
//		else if( renderableGroup/*.RenderableGroup*/.X == 1 )
//		{
//			//billboards

//			ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup/*.RenderableGroup*/.Y ];
//			var meshData = Billboard.GetBillboardMesh().Result.MeshData;

//			for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
//			{
//				var oper = meshData.RenderOperations[ nOperation ];

//				if( oper.VoxelDataInfo != null )
//				{
//					foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, true ) )
//					{
//						if( materialData.deferredShadingSupport )
//						{
//							bool instancing = Instancing && billboardItem.CutVolumes == null;
//							manager.Add( renderableGroup/*.RenderableGroup*/, nOperation, oper, materialData, instancing );
//						}
//					}
//				}
//			}
//		}
//	}

//	manager.Prepare();
//} );

////!!!!can render parallel for each instance. deferred lighting and deferred lighting gi are also can be optimized

////push to GPU
//for( int nSector = 0; nSector < sectorsByDistance; nSector++ )
//{
//	var manager = outputInstancingManagers[ nSector ];

//	int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorsByDistance );
//	int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorsByDistance );
//	if( nSector == sectorsByDistance - 1 )
//		indexTo = renderableGroupsToDraw.Count;

//	//render output items
//	{
//		var outputItems = manager.outputItems;
//		for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
//		{
//			ref var outputItem = ref outputItems.Data[ nOutputItem ];
//			var materialData = outputItem.materialData;
//			var voxelRendering = outputItem.operation.VoxelDataInfo != null;

//			if( Instancing && outputItem.renderableItemsCount >= 2 )//InstancingMinCount )
//			{
//				//with instancing

//				//bind material data
//				BindMaterialData( context, materialData, false, voxelRendering );
//				BindSamplersForTextureOnlySlots( context, false, voxelRendering );

//				//GpuMaterialPass pass = null;

//				//bind operation data
//				var firstRenderableItem = outputItem.renderableItemFirst;
//				if( firstRenderableItem.X == 0 )
//				{
//					//meshes
//					ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ firstRenderableItem.Y ];
//					var meshData = meshItem.MeshData;

//					BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo, null, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

//					//pass = materialData.deferredShadingPass.Get( outputItem.operation.VoxelDataInfo != null, meshData.BillboardMode != 0 );


//					//!!!!slowly instancing


//					int instanceCount = outputItem.renderableItemsCount;
//					for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
//					{
//						var renderableItem = outputItem.renderableItems[ nRenderableItem ];
//						ref var meshItem2 = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];

//						meshItem2.BoundingSphere.ToBounds( out var bounds );
//						if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
//						{
//							var objectData = new GIObjectsData();
//							meshItem2.TransformRelative.GetTranspose( out objectData.Transform );
//							objectData.Color = meshItem2.Color;
//							objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
//							objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

//							RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
//						}
//					}

//				}
//				else if( firstRenderableItem.X == 1 )
//				{
//					//!!!!

//					////billboards
//					//ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ firstRenderableItem.Y ];
//					//var meshData = Billboard.GetBillboardMesh().Result.MeshData;

//					//BindRenderOperationData( context, frameData, materialData, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo );

//					//pass = materialData.deferredShadingPass.Billboard;
//				}


//				//int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
//				//int instanceCount = outputItem.renderableItemsCount;

//				//if( InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) == instanceCount )
//				//{
//				//	var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );

//				//	//get instancing matrices
//				//	RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
//				//	int currentMatrix = 0;
//				//	for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
//				//	{
//				//		var renderableItem = outputItem.renderableItems[ nRenderableItem ];

//				//		if( renderableItem.X == 0 )
//				//		{
//				//			//meshes
//				//			ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
//				//			//!!!!slowly because no threading? where else
//				//			meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
//				//		}
//				//		else if( renderableItem.X == 1 )
//				//		{
//				//			//billboards
//				//			ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
//				//			billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
//				//		}
//				//	}

//				//	RenderOperation( context, outputItem.operation, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
//				//}

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
//						ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
//						var meshData = meshItem.MeshData;

//						BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo, meshItem.MaterialInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

//						//!!!!slowly
//						//!!!!можно точнее считать? везде так


//						//unsafe
//						//{
//						//	var t = new Vector4F( materialData.currentFrameIndex, 0, 0, 0 );
//						//	var temptemp = GpuProgramManager.RegisterUniform( "temptemp", UniformType.Vector4, 1 );
//						//	Bgfx.SetUniform( temptemp, &t );
//						//}


//						//!!!!batching


//						//var tr = meshItem.Transform;

//						//unsafe
//						//{
//						//	if( giObjectTransformUniform == null )
//						//		giObjectTransformUniform = GpuProgramManager.RegisterUniform( "objectTransform", UniformType.Matrix4x4, 1 );
//						//	Bgfx.SetUniform( giObjectTransformUniform.Value, &tr );// meshItem.Transform );
//						//}

//						//!!!!?
//						//if( !meshItem.InstancingEnabled )
//						//	fixed( Matrix4F* p = &meshItem.Transform )
//						//		Bgfx.SetTransform( (float*)p );

//						//var pass = materialData.deferredShadingPass.Get( outputItem.operation.VoxelDataInfo != null/*, outputItem.operation.VirtualizedData != null*/, meshData.BillboardMode != 0 );


//						//!!!!slowly batching


//						if( meshItem.InstancingEnabled )
//						{
//							//!!!!или всегда везде выставлять
//							var instancingCount = meshItem.InstancingCount;
//							if( instancingCount < 0 )
//							{
//								//!!!! - instancingStart?

//								if( meshItem.InstancingVertexBuffer != null )
//									instancingCount = meshItem.InstancingVertexBuffer.VertexCount;
//								else
//									instancingCount = meshItem.InstancingDataBuffer.Size;
//							}

//							for( int nObject = 0; nObject < instancingCount; nObject++ )
//							{
//								if( meshItem.InstancingVertexBuffer != null )
//								{
//									var array = meshItem.InstancingVertexBuffer.Vertices;
//									if( array != null )
//									{
//										fixed( byte* pArray = array )
//										{
//											var instancingData = (RenderSceneData.ObjectInstanceData*)pArray;
//											var instancingData2 = instancingData + nObject + meshItem.InstancingStart;

//											instancingData2->TransformRelative.GetTranslation( out var pos );

//											var bounds = new Bounds( pos );
//											bounds.Expand( meshItem.InstancingMaxLocalBounds );

//											//meshData.SpaceBounds.BoundingSphere.Radius
//											//meshItem.BoundingSphere.ToBounds( out var bounds );

//											if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
//											{
//												var objectData = new GIObjectsData();
//												objectData.Transform = instancingData2->TransformRelative;

//												//!!!!
//												var c = instancingData2->Color.ToColorValue();// / 255.0f;
//												objectData.Color.Red = MathEx.Pow( c.Red, 2 ) * 10.0f;
//												objectData.Color.Green = MathEx.Pow( c.Green, 2 ) * 10.0f;
//												objectData.Color.Blue = MathEx.Pow( c.Blue, 2 ) * 10.0f;
//												objectData.Color.Alpha = MathEx.Pow( c.Alpha, 2 ) * 10.0f;

//												objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
//												objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

//												RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
//											}
//										}
//									}
//								}
//								else
//								{
//									var instancingData = (RenderSceneData.ObjectInstanceData*)meshItem.InstancingDataBuffer.Data;
//									var instancingData2 = instancingData + nObject + meshItem.InstancingStart;

//									instancingData2->TransformRelative.GetTranslation( out var pos );

//									var bounds = new Bounds( pos );
//									bounds.Expand( meshItem.InstancingMaxLocalBounds );

//									if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
//									{
//										var objectData = new GIObjectsData();
//										objectData.Transform = instancingData2->TransformRelative;

//										//!!!!
//										var c = instancingData2->Color.ToColorValue();// / 255.0f;
//										objectData.Color.Red = MathEx.Pow( c.Red, 2 ) * 10.0f;
//										objectData.Color.Green = MathEx.Pow( c.Green, 2 ) * 10.0f;
//										objectData.Color.Blue = MathEx.Pow( c.Blue, 2 ) * 10.0f;
//										objectData.Color.Alpha = MathEx.Pow( c.Alpha, 2 ) * 10.0f;

//										objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
//										objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

//										RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
//									}
//								}
//							}
//						}
//						else
//						{
//							meshItem.BoundingSphere.ToBounds( out var bounds );
//							if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
//							{
//								var objectData = new GIObjectsData();
//								meshItem.TransformRelative.GetTranspose( out objectData.Transform );
//								objectData.Color = meshItem.Color;
//								objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
//								objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

//								RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
//							}
//						}

//						//RenderOperation( context, outputItem.operation, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount );

//					}
//					else if( renderableItem.X == 1 )
//					{
//						//billboards
//						ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
//						var meshData = Billboard.GetBillboardMesh().Result.MeshData;

//						//!!!!

//						//BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo );

//						//billboardItem.GetWorldMatrix( out var worldMatrix );
//						//Bgfx.SetTransform( (float*)&worldMatrix );

//						//var pass = materialData.deferredShadingPass.Billboard;

//						//RenderOperation( context, outputItem.operation, pass, null, billboardItem.CutVolumes );

//					}
//				}
//			}
//		}
//	}

//	//!!!!impl
//	////render layers
//	//if( DebugDrawLayers )
//	//{
//	//	for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
//	//	{
//	//		var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

//	//		if( renderableGroup.RenderableGroup.X == 0 )
//	//		{
//	//			//meshes

//	//			ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.RenderableGroup.Y ];
//	//			var meshData = meshItem.MeshData;

//	//			if( meshItem.Layers != null )
//	//			{
//	//				for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
//	//				{
//	//					ref var layer = ref meshItem.Layers[ nLayer ];
//	//					foreach( var materialData in GetLayerMaterialData( ref layer, true, true, qqtrue ) )
//	//					{
//	//						if( materialData.deferredShadingPass.Usual != null )
//	//						{
//	//							if( !meshItem.InstancingEnabled )
//	//								fixed( Matrix4F* p = &meshItem.Transform )
//	//									Bgfx.SetTransform( (float*)p );

//	//							var color = /*meshItem.Color * */ layer.MaterialColor;

//	//							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
//	//							{
//	//								var oper = meshData.RenderOperations[ nOperation ];
//	//								var voxelRendering = oper.VoxelDataInfo != null;

//	//								//bind material data
//	//								BindMaterialData( context, materialData, false, voxelRendering );
//	//								BindSamplersForTextureOnlySlots( context, false, voxelRendering );
//	//								materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );

//	//								BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, null, oper.VoxelDataInfo/*, oper.VirtualizedDataImage, oper.VirtualizedDataInfo*/ );

//	//								var pass = materialData.deferredShadingPass.Get( oper.VoxelDataInfo != null/*, oper.VirtualizedData != null*/, meshData.BillboardMode != 0 );

//	//								RenderOperation( context, oper, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount );
//	//							}
//	//						}
//	//					}
//	//				}
//	//			}
//	//		}
//	//	}
//	//}
//}

////clear outputInstancingManagers
//foreach( var manager in outputInstancingManagers )
//	manager.Clear();




//////////////////////////////////////////////////////




//static Program? giClearGBufferGridProgram;
//static Program? giClearLightingGridProgram;
//static Uniform? giObjectDataUniform;
//static Uniform? giRenderLightParametersUniform;
//static Program? giCopyLightingGridProgram;
//static GIDeferredShadingData giDeferredShadingData;

//static bool? giShaderFilesExists;

/////////////////////////////////////////////////

//[StructLayout( LayoutKind.Sequential )]
//public struct GIObjectsData
//{
//	public Matrix3x4F Transform;
//	public ColorValue Color;
//	public Vector4F GridIndexesMin;
//	public Vector4F GridIndexesMax;
//}

//[MethodImpl( (MethodImplOptions)512 )]
//public unsafe void RenderOperationGI( ViewportRenderingContext context, Program? giVoxelProgram, GIObjectsData* objectData, ref BoundsI gridIndexes, RenderSceneData.CutVolumeItem[] cutVolumes )//, int level ) // RenderSceneData.MeshDataRenderOperation op, GpuMaterialPass pass, List<ParameterContainer> parameterContainers, RenderSceneData.CutVolumeItem[] cutVolumes, bool instancingEnabled, GpuVertexBuffer instancingBuffer, ref InstanceDataBuffer instancingDataBuffer, int instancingStart, int instancingCount )
//{
//	if( !giVoxelProgram.HasValue )
//		return;

//	SetCutVolumeSettingsUniforms( context, cutVolumes, false );

//	if( giObjectDataUniform == null )
//		giObjectDataUniform = GpuProgramManager.RegisterUniform( "giObjectData", UniformType.Vector4, 6 );
//	Bgfx.SetUniform( giObjectDataUniform.Value, objectData, 6 );

//	var size = gridIndexes.GetSize() + new Vector3I( 1, 1, 1 );
//	var jobSize = new Vector3I( (int)Math.Ceiling( size.X / 8.0 ), (int)Math.Ceiling( size.Y / 8.0 ), (int)Math.Ceiling( size.Z / 8.0 ) );

//	var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

//	Bgfx.Dispatch( context.CurrentViewNumber, giVoxelProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, discardFlags );
//	context.UpdateStatisticsCurrent.ComputeDispatches++;


//	//var data = context.FrameData.IndirectLightingFrameData;

//	//var cellSizeOfLevel = data.CellSizeLevel0 * MathEx.Pow( 2.0f, level );
//	//var gridSizeHalf = cellSizeOfLevel * data.GridResolution * 0.5f;
//	//var gridPosition = data.GridCenter - new Vector3( gridSizeHalf, gridSizeHalf, gridSizeHalf );
//	//var voxelizationFactor = (float)data.Owner.VoxelizationFactor.Value;

//	//{
//	//	var parameters = new Vector4F( cellSizeOfLevel, voxelizationFactor, data.GridResolution, level );
//	//	if( giRenderToGridParametersUniform == null )
//	//		giRenderToGridParametersUniform = GpuProgramManager.RegisterUniform( "giRenderToGridParameters", UniformType.Vector4, 1 );
//	//	Bgfx.SetUniform( giRenderToGridParametersUniform.Value, &parameters );
//	//}

//	//{
//	//	//!!!!double
//	//	var v = new Vector4F( gridPosition.ToVector3F(), 0 );
//	//	var uniform = GpuProgramManager.RegisterUniform( "gridPosition", UniformType.Vector4, 1 );
//	//	Bgfx.SetUniform( uniform, &v );
//	//}

//	//{
//	//	var v = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
//	//	var uniform = GpuProgramManager.RegisterUniform( "giGridIndexesMin", UniformType.Vector4, 1 );
//	//	Bgfx.SetUniform( uniform, &v );
//	//}
//	//{
//	//	var v = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );
//	//	var uniform = GpuProgramManager.RegisterUniform( "giGridIndexesMax", UniformType.Vector4, 1 );
//	//	Bgfx.SetUniform( uniform, &v );
//	//}
//}

//[MethodImpl( (MethodImplOptions)512 )]
//protected internal unsafe virtual void Render3DSceneGI( ViewportRenderingContext context, FrameData frameData, ImageComponent giGBufferGridTexture, ImageComponent giLightingGridTexture, int level )
//{
//	var indirectFrameData = frameData.IndirectLightingFrameData;

//	ref var levelBounds = ref indirectFrameData.BoundsByLevel[ level ];

//	var viewportOwner = context.Owner;
//	var sectorsByDistance = SectorsByDistance.Value;

//	using( var renderableGroupsToDraw = new OpenListNative<Vector2I>( frameData.RenderableGroupsInFrustum.Count + frameData.RenderableGroupsForGI.Count ) )
//	{
//		//RenderableGroupsInFrustum
//		foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
//		{
//			bool add = false;

//			if( renderableGroup.X == 0 )
//			{
//				ref var data = ref frameData.Meshes.Data[ renderableGroup.Y ];
//				if( ( data.Flags & FrameData.MeshItem.FlagsEnum.UseDeferred ) != 0 )
//				{
//					ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
//					if( levelBounds.Intersects( ref meshItem.BoundingSphere ) )
//						add = true;
//				}
//			}
//			else
//			{
//				ref var data = ref frameData.Billboards.Data[ renderableGroup.Y ];
//				if( ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseDeferred ) != 0 )
//				{
//					ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];
//					if( levelBounds.Intersects( ref billboardItem.BoundingSphere ) )
//						add = true;
//				}
//			}

//			if( add )
//			{
//				renderableGroupsToDraw.Add( renderableGroup );
//				//var item = new RenderableGroupWithDistance();
//				//item.RenderableGroup = renderableGroup;
//				//item.DistanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.RenderableGroup );
//				//renderableGroupsToDraw.Add( ref item );
//			}
//		}

//		//RenderableGroupsForGI
//		foreach( var renderableGroup in frameData.RenderableGroupsForGI )
//		{
//			bool add = false;

//			if( renderableGroup.X == 0 )
//			{
//				ref var data = ref frameData.Meshes.Data[ renderableGroup.Y ];
//				if( ( data.Flags & FrameData.MeshItem.FlagsEnum.UseDeferred ) != 0 )
//				{
//					ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.Y ];
//					if( levelBounds.Intersects( ref meshItem.BoundingSphere ) )
//						add = true;
//				}
//			}
//			else
//			{
//				ref var data = ref frameData.Billboards.Data[ renderableGroup.Y ];
//				if( ( data.Flags & FrameData.BillboardItem.FlagsEnum.UseDeferred ) != 0 )
//				{
//					ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup.Y ];
//					if( levelBounds.Intersects( ref billboardItem.BoundingSphere ) )
//						add = true;
//				}
//			}

//			if( add )
//			{
//				renderableGroupsToDraw.Add( renderableGroup );
//				//var item = new RenderableGroupWithDistance();
//				//item.RenderableGroup = renderableGroup;
//				//item.DistanceSquared = frameData.GetObjectGroupDistanceToCameraSquared( ref item.RenderableGroup );
//				//renderableGroupsToDraw.Add( ref item );
//			}
//		}

//		if( renderableGroupsToDraw.Count == 0 )
//			return;


//		////sort by distance
//		//CollectionUtility.MergeSortUnmanaged( renderableGroupsToDraw.Data, renderableGroupsToDraw.Count, delegate ( RenderableGroupWithDistance* a, RenderableGroupWithDistance* b )
//		//{
//		//	if( a->DistanceSquared < b->DistanceSquared )
//		//		return -1;
//		//	if( a->DistanceSquared > b->DistanceSquared )
//		//		return 1;
//		//	return 0;
//		//}, true );


//		var indirectData = context.FrameData.IndirectLightingFrameData;
//		var cellSizeOfLevel = indirectData.CellSizeLevel0 * MathEx.Pow( 2.0f, level );
//		var cellSizeOfLevelInv = 1.0f / cellSizeOfLevel;
//		var gridSizeHalf = cellSizeOfLevel * indirectData.GridResolution * 0.5f;
//		var gridPosition = indirectData.GridCenter - new Vector3( gridSizeHalf, gridSizeHalf, gridSizeHalf );
//		var voxelizationFactor = (float)indirectData.Owner.VoxelizationExpandFactor.Value;
//		var emissiveLighting = indirectData.Owner.EmissiveLighting.Value;

//		var parameters = stackalloc Vector4F[ 2 ];
//		parameters[ 0 ] = new Vector4F( cellSizeOfLevel, voxelizationFactor, indirectData.GridResolution, level );
//		//!!!!double
//		parameters[ 1 ] = new Vector4F( gridPosition.ToVector3F(), (float)emissiveLighting );
//		var uniform = GpuProgramManager.RegisterUniform( "giRenderToGridParameters", UniformType.Vector4, 2 );
//		Bgfx.SetUniform( uniform, parameters, 2 );


//		context.BindComputeImage( 0, giGBufferGridTexture, 0, ComputeBufferAccessEnum.Write );
//		context.BindComputeImage( 1, giLightingGridTexture, 0, ComputeBufferAccessEnum.Write );
//		//context.BindComputeImage( 3, giGBufferGridTexture, 0, ComputeBufferAccessEnum.Write );
//		//context.BindComputeImage( 4, giLightingGridTexture, 0, ComputeBufferAccessEnum.Write );


//		//bind textures for all render operations

//		SetCutVolumeSettingsUniforms( context, null, true );

//		//!!!!temp?
//		BindMaterialsTexture( context, frameData, true );
//		//BindMaterialsTexture( context, frameData );

//		BindBonesTexture( context, frameData, true );

//		BindSamplersForTextureOnlySlots( context, true, true );
//		BindMaterialData( context, null, false, true );


//		//prepare outputInstancingManagers
//		Parallel.For( 0, sectorsByDistance, delegate ( int nSector )
//		{
//			var manager = outputInstancingManagers[ nSector ];

//			int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorsByDistance );
//			int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorsByDistance );
//			if( nSector == sectorsByDistance - 1 )
//				indexTo = renderableGroupsToDraw.Count;

//			//fill output manager
//			for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
//			{
//				var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

//				if( renderableGroup/*.RenderableGroup*/.X == 0 )
//				{
//					//meshes

//					ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup/*.RenderableGroup*/.Y ];

//					var meshData = meshItem.MeshDataLOD0 ?? meshItem.MeshData;
//					var lods = meshData.LODs;
//					if( lods != null )
//						meshData = lods[ lods.Length - 1 ].Mesh?.Result?.MeshData ?? meshData;

//					for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
//					{
//						var oper = meshData.RenderOperations[ nOperation ];

//						if( oper.VoxelDataInfo != null )
//						{
//							foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, true, true ) )
//							{
//								if( materialData.deferredShadingSupport )
//								{
//									bool instancing = Instancing && meshItem.AnimationData == null && !meshItem.InstancingEnabled && meshItem.CutVolumes == null;
//									manager.Add( renderableGroup/*.RenderableGroup*/, nOperation, oper, materialData, instancing );
//								}
//							}
//						}
//					}
//				}
//				else if( renderableGroup/*.RenderableGroup*/.X == 1 )
//				{
//					//billboards

//					ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableGroup/*.RenderableGroup*/.Y ];
//					var meshData = Billboard.GetBillboardMesh().Result.MeshData;

//					for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
//					{
//						var oper = meshData.RenderOperations[ nOperation ];

//						if( oper.VoxelDataInfo != null )
//						{
//							foreach( var materialData in GetBillboardMaterialData( ref billboardItem, true, true ) )
//							{
//								if( materialData.deferredShadingSupport )
//								{
//									bool instancing = Instancing && billboardItem.CutVolumes == null;
//									manager.Add( renderableGroup/*.RenderableGroup*/, nOperation, oper, materialData, instancing );
//								}
//							}
//						}
//					}
//				}
//			}

//			manager.Prepare();
//		} );

//		//!!!!can render parallel for each instance. deferred lighting and deferred lighting gi are also can be optimized

//		//push to GPU
//		for( int nSector = 0; nSector < sectorsByDistance; nSector++ )
//		{
//			var manager = outputInstancingManagers[ nSector ];

//			int indexFrom = (int)( (float)renderableGroupsToDraw.Count * nSector / sectorsByDistance );
//			int indexTo = (int)( (float)renderableGroupsToDraw.Count * ( nSector + 1 ) / sectorsByDistance );
//			if( nSector == sectorsByDistance - 1 )
//				indexTo = renderableGroupsToDraw.Count;

//			//render output items
//			{
//				var outputItems = manager.outputItems;
//				for( int nOutputItem = 0; nOutputItem < outputItems.Count; nOutputItem++ )
//				{
//					ref var outputItem = ref outputItems.Data[ nOutputItem ];
//					var materialData = outputItem.materialData;
//					var voxelRendering = outputItem.operation.VoxelDataInfo != null;

//					if( Instancing && outputItem.renderableItemsCount >= 2 )//InstancingMinCount )
//					{
//						//with instancing

//						//bind material data
//						BindMaterialData( context, materialData, false, voxelRendering );
//						BindSamplersForTextureOnlySlots( context, false, voxelRendering );

//						//GpuMaterialPass pass = null;

//						//bind operation data
//						var firstRenderableItem = outputItem.renderableItemFirst;
//						if( firstRenderableItem.X == 0 )
//						{
//							//meshes
//							ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ firstRenderableItem.Y ];
//							var meshData = meshItem.MeshData;

//							BindRenderOperationData( context, frameData, materialData, true, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo, null, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

//							//pass = materialData.deferredShadingPass.Get( outputItem.operation.VoxelDataInfo != null, meshData.BillboardMode != 0 );


//							//!!!!slowly instancing


//							int instanceCount = outputItem.renderableItemsCount;
//							for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
//							{
//								var renderableItem = outputItem.renderableItems[ nRenderableItem ];
//								ref var meshItem2 = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];

//								meshItem2.BoundingSphere.ToBounds( out var bounds );
//								if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
//								{
//									var objectData = new GIObjectsData();
//									meshItem2.TransformRelative.GetTranspose( out objectData.Transform );
//									objectData.Color = meshItem2.Color;
//									objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
//									objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

//									RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
//								}
//							}

//						}
//						else if( firstRenderableItem.X == 1 )
//						{
//							//!!!!

//							////billboards
//							//ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ firstRenderableItem.Y ];
//							//var meshData = Billboard.GetBillboardMesh().Result.MeshData;

//							//BindRenderOperationData( context, frameData, materialData, true, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo );

//							//pass = materialData.deferredShadingPass.Billboard;
//						}


//						//int instanceStride = sizeof( RenderSceneData.ObjectInstanceData );
//						//int instanceCount = outputItem.renderableItemsCount;

//						//if( InstanceDataBuffer.GetAvailableSpace( instanceCount, instanceStride ) == instanceCount )
//						//{
//						//	var instanceBuffer = new InstanceDataBuffer( instanceCount, instanceStride );

//						//	//get instancing matrices
//						//	RenderSceneData.ObjectInstanceData* instancingData = (RenderSceneData.ObjectInstanceData*)instanceBuffer.Data;
//						//	int currentMatrix = 0;
//						//	for( int nRenderableItem = 0; nRenderableItem < instanceCount; nRenderableItem++ )
//						//	{
//						//		var renderableItem = outputItem.renderableItems[ nRenderableItem ];

//						//		if( renderableItem.X == 0 )
//						//		{
//						//			//meshes
//						//			ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
//						//			//!!!!slowly because no threading? where else
//						//			meshItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
//						//		}
//						//		else if( renderableItem.X == 1 )
//						//		{
//						//			//billboards
//						//			ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
//						//			billboardItem.GetInstancingData( out instancingData[ currentMatrix++ ] );
//						//		}
//						//	}

//						//	RenderOperation( context, outputItem.operation, pass, null, null, true, null, ref instanceBuffer, 0, instanceCount );
//						//}

//					}
//					else
//					{
//						//without instancing

//						for( int nRenderableItem = 0; nRenderableItem < outputItem.renderableItemsCount; nRenderableItem++ )
//						{
//							Vector3I renderableItem;
//							if( nRenderableItem != 0 )
//								renderableItem = outputItem.renderableItems[ nRenderableItem ];
//							else
//								renderableItem = outputItem.renderableItemFirst;

//							//bind material data
//							BindMaterialData( context, materialData, false, voxelRendering );
//							BindSamplersForTextureOnlySlots( context, false, voxelRendering );

//							//bind render operation data, set matrix
//							if( renderableItem.X == 0 )
//							{
//								//meshes
//								ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableItem.Y ];
//								var meshData = meshItem.MeshData;

//								BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.boundingSphere.Radius, meshItem.ReceiveDecals, ref meshItem.PreviousFramePositionChange, meshItem.LODValue, outputItem.operation.UnwrappedUV, ref meshItem.Color, outputItem.operation.VertexStructureContainsColor, false, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo, meshItem.MaterialInstanceParameters, meshItem.CullingByCameraDirectionData, ref meshItem.InstancingPositionOffsetRelative );

//								//!!!!slowly
//								//!!!!можно точнее считать? везде так


//								//unsafe
//								//{
//								//	var t = new Vector4F( materialData.currentFrameIndex, 0, 0, 0 );
//								//	var temptemp = GpuProgramManager.RegisterUniform( "temptemp", UniformType.Vector4, 1 );
//								//	Bgfx.SetUniform( temptemp, &t );
//								//}


//								//!!!!batching


//								//var tr = meshItem.Transform;

//								//unsafe
//								//{
//								//	if( giObjectTransformUniform == null )
//								//		giObjectTransformUniform = GpuProgramManager.RegisterUniform( "objectTransform", UniformType.Matrix4x4, 1 );
//								//	Bgfx.SetUniform( giObjectTransformUniform.Value, &tr );// meshItem.Transform );
//								//}

//								//!!!!?
//								//if( !meshItem.InstancingEnabled )
//								//	fixed( Matrix4F* p = &meshItem.Transform )
//								//		Bgfx.SetTransform( (float*)p );

//								//var pass = materialData.deferredShadingPass.Get( outputItem.operation.VoxelDataInfo != null/*, outputItem.operation.VirtualizedData != null*/, meshData.BillboardMode != 0 );


//								//!!!!slowly batching


//								if( meshItem.InstancingEnabled )
//								{
//									//!!!!или всегда везде выставлять
//									var instancingCount = meshItem.InstancingCount;
//									if( instancingCount < 0 )
//									{
//										//!!!! - instancingStart?

//										if( meshItem.InstancingVertexBuffer != null )
//											instancingCount = meshItem.InstancingVertexBuffer.VertexCount;
//										else
//											instancingCount = meshItem.InstancingDataBuffer.Size;
//									}

//									for( int nObject = 0; nObject < instancingCount; nObject++ )
//									{
//										if( meshItem.InstancingVertexBuffer != null )
//										{
//											var array = meshItem.InstancingVertexBuffer.Vertices;
//											if( array != null )
//											{
//												fixed( byte* pArray = array )
//												{
//													var instancingData = (RenderSceneData.ObjectInstanceData*)pArray;
//													var instancingData2 = instancingData + nObject + meshItem.InstancingStart;

//													instancingData2->TransformRelative.GetTranslation( out var pos );

//													var bounds = new Bounds( pos );
//													bounds.Expand( meshItem.InstancingMaxLocalBounds );

//													//meshData.SpaceBounds.BoundingSphere.Radius
//													//meshItem.BoundingSphere.ToBounds( out var bounds );

//													if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
//													{
//														var objectData = new GIObjectsData();
//														objectData.Transform = instancingData2->TransformRelative;

//														//!!!!
//														var c = instancingData2->Color.ToColorValue();// / 255.0f;
//														objectData.Color.Red = MathEx.Pow( c.Red, 2 ) * 10.0f;
//														objectData.Color.Green = MathEx.Pow( c.Green, 2 ) * 10.0f;
//														objectData.Color.Blue = MathEx.Pow( c.Blue, 2 ) * 10.0f;
//														objectData.Color.Alpha = MathEx.Pow( c.Alpha, 2 ) * 10.0f;

//														objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
//														objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

//														RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
//													}
//												}
//											}
//										}
//										else
//										{
//											var instancingData = (RenderSceneData.ObjectInstanceData*)meshItem.InstancingDataBuffer.Data;
//											var instancingData2 = instancingData + nObject + meshItem.InstancingStart;

//											instancingData2->TransformRelative.GetTranslation( out var pos );

//											var bounds = new Bounds( pos );
//											bounds.Expand( meshItem.InstancingMaxLocalBounds );

//											if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
//											{
//												var objectData = new GIObjectsData();
//												objectData.Transform = instancingData2->TransformRelative;

//												//!!!!
//												var c = instancingData2->Color.ToColorValue();// / 255.0f;
//												objectData.Color.Red = MathEx.Pow( c.Red, 2 ) * 10.0f;
//												objectData.Color.Green = MathEx.Pow( c.Green, 2 ) * 10.0f;
//												objectData.Color.Blue = MathEx.Pow( c.Blue, 2 ) * 10.0f;
//												objectData.Color.Alpha = MathEx.Pow( c.Alpha, 2 ) * 10.0f;

//												objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
//												objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

//												RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
//											}
//										}
//									}
//								}
//								else
//								{
//									meshItem.BoundingSphere.ToBounds( out var bounds );
//									if( indirectFrameData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref bounds, out var gridIndexes ) )
//									{
//										var objectData = new GIObjectsData();
//										meshItem.TransformRelative.GetTranspose( out objectData.Transform );
//										objectData.Color = meshItem.Color;
//										objectData.GridIndexesMin = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
//										objectData.GridIndexesMax = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );

//										RenderOperationGI( context, materialData.giVoxelProgram, &objectData, ref gridIndexes, meshItem.CutVolumes );
//									}
//								}

//								//RenderOperation( context, outputItem.operation, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount );

//							}
//							else if( renderableItem.X == 1 )
//							{
//								//billboards
//								ref var billboardItem = ref frameData.RenderSceneData.Billboards.Data[ renderableItem.Y ];
//								var meshData = Billboard.GetBillboardMesh().Result.MeshData;

//								//!!!!

//								//BindRenderOperationData( context, frameData, materialData, false, null, meshData.BillboardMode, billboardItem.ShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, billboardItem.ReceiveDecals, ref billboardItem.PositionPreviousFrame, 0, outputItem.operation.UnwrappedUV, ref billboardItem.Color, outputItem.operation.VertexStructureContainsColor, false, billboardItem.VisibilityDistance, billboardItem.MotionBlurFactor, false, outputItem.operation.VoxelDataImage, outputItem.operation.VoxelDataInfo );

//								//billboardItem.GetWorldMatrix( out var worldMatrix );
//								//Bgfx.SetTransform( (float*)&worldMatrix );

//								//var pass = materialData.deferredShadingPass.Billboard;

//								//RenderOperation( context, outputItem.operation, pass, null, billboardItem.CutVolumes );

//							}
//						}
//					}
//				}
//			}

//			//!!!!impl
//			////render layers
//			//if( DebugDrawLayers )
//			//{
//			//	for( int nRenderableGroup = indexFrom; nRenderableGroup < indexTo; nRenderableGroup++ )
//			//	{
//			//		var renderableGroup = renderableGroupsToDraw[ nRenderableGroup ];

//			//		if( renderableGroup.RenderableGroup.X == 0 )
//			//		{
//			//			//meshes

//			//			ref var meshItem = ref frameData.RenderSceneData.Meshes.Data[ renderableGroup.RenderableGroup.Y ];
//			//			var meshData = meshItem.MeshData;

//			//			if( meshItem.Layers != null )
//			//			{
//			//				for( int nLayer = 0; nLayer < meshItem.Layers.Length; nLayer++ )
//			//				{
//			//					ref var layer = ref meshItem.Layers[ nLayer ];
//			//					foreach( var materialData in GetLayerMaterialData( ref layer, true, true, qqtrue ) )
//			//					{
//			//						if( materialData.deferredShadingPass.Usual != null )
//			//						{
//			//							if( !meshItem.InstancingEnabled )
//			//								fixed( Matrix4F* p = &meshItem.Transform )
//			//									Bgfx.SetTransform( (float*)p );

//			//							var color = /*meshItem.Color * */ layer.MaterialColor;

//			//							for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
//			//							{
//			//								var oper = meshData.RenderOperations[ nOperation ];
//			//								var voxelRendering = oper.VoxelDataInfo != null;

//			//								//bind material data
//			//								BindMaterialData( context, materialData, false, voxelRendering );
//			//								BindSamplersForTextureOnlySlots( context, false, voxelRendering );
//			//								materialData.BindCurrentFrameDataMaskTextures( context, layer.Mask );

//			//								BindRenderOperationData( context, frameData, materialData, meshItem.InstancingEnabled, meshItem.AnimationData, meshData.BillboardMode, meshData.BillboardShadowOffset * meshData.SpaceBounds.BoundingSphere.Value.Radius, meshItem.ReceiveDecals, ref meshItem.PositionPreviousFrame, meshItem.LODValue, oper.UnwrappedUV, ref color, oper.VertexStructureContainsColor, true, meshItem.VisibilityDistance, meshItem.MotionBlurFactor, layer.MaskFormat == PaintLayer.MaskFormatEnum.Triangles, null, oper.VoxelDataInfo/*, oper.VirtualizedDataImage, oper.VirtualizedDataInfo*/ );

//			//								var pass = materialData.deferredShadingPass.Get( oper.VoxelDataInfo != null/*, oper.VirtualizedData != null*/, meshData.BillboardMode != 0 );

//			//								RenderOperation( context, oper, pass, null, meshItem.CutVolumes, meshItem.InstancingEnabled, meshItem.InstancingVertexBuffer, ref meshItem.InstancingDataBuffer, meshItem.InstancingStart, meshItem.InstancingCount );
//			//							}
//			//						}
//			//					}
//			//				}
//			//			}
//			//		}
//			//	}
//			//}
//		}

//		//clear outputInstancingManagers
//		foreach( var manager in outputInstancingManagers )
//			manager.Clear();
//	}
//}

//void InitGIDeferredShadingData()
//{
//	giDeferredShadingData = new GIDeferredShadingData();

//	for( int nShadingModel = 0; nShadingModel < 2; nShadingModel++ )
//	{
//		var shadingModelFull = nShadingModel != 0;

//		//environment light
//		{
//			var defines = new List<(string, string)>();
//			defines.Add( ("LIGHT_TYPE_" + Light.TypeEnum.Ambient.ToString().ToUpper(), "") );
//			if( shadingModelFull )
//				defines.Add( ("GI_SHADING_MODEL_FULL", "") );

//			string error2;
//			var program = GpuProgramManager.GetProgram( "GIRenderAmbientLight_", GpuProgramType.Compute, @"Base\Shaders\GIRenderAmbientLight.sc", defines, true, out error2 );
//			if( !string.IsNullOrEmpty( error2 ) )
//			{
//				Log.Fatal( error2 );
//				return;
//			}

//			var program2 = new Program( program.RealObject );

//			if( shadingModelFull )
//				giDeferredShadingData.passesPerLightWithoutShadowsFull[ 0 ] = program2;
//			else
//				giDeferredShadingData.passesPerLightWithoutShadowsSimple[ 0 ] = program2;
//		}

//		//direct lights
//		for( int nShadows = 0; nShadows < 2; nShadows++ )
//		{
//			var shadows = nShadows != 0;

//			if( RenderingSystem.ShadowTechnique == ProjectSettingsPage_Rendering.ShadowTechniqueEnum.None && shadows )
//				continue;

//			for( int nLight = 1; nLight < 4; nLight++ )
//			{
//				var lightType = (Light.TypeEnum)nLight;

//				var defines = new List<(string, string)>();
//				defines.Add( ("LIGHT_TYPE_" + lightType.ToString().ToUpper(), "") );
//				if( shadows )
//					defines.Add( ("SHADOW_MAP", "") );
//				if( shadingModelFull )
//					defines.Add( ("GI_SHADING_MODEL_FULL", "") );

//				string error2;
//				var program = GpuProgramManager.GetProgram( "GIRenderDirectLight_", GpuProgramType.Compute, @"Base\Shaders\GIRenderDirectLight.sc", defines, true, out error2 );
//				if( !string.IsNullOrEmpty( error2 ) )
//				{
//					Log.Fatal( error2 );
//					return;
//				}

//				var program2 = new Program( program.RealObject );

//				if( shadingModelFull )
//				{
//					if( nShadows == 1 )
//						giDeferredShadingData.passesPerLightWithShadowsFull[ nLight ] = program2;
//					else
//						giDeferredShadingData.passesPerLightWithoutShadowsFull[ nLight ] = program2;
//				}
//				else
//				{
//					if( nShadows == 1 )
//						giDeferredShadingData.passesPerLightWithShadowsSimple[ nLight ] = program2;
//					else
//						giDeferredShadingData.passesPerLightWithoutShadowsSimple[ nLight ] = program2;
//				}
//			}
//		}
//	}
//}

//[MethodImpl( (MethodImplOptions)512 )]
//unsafe void RenderEnvironmentLightDeferredGI( ViewportRenderingContext context, FrameData frameData, ImageComponent giGBufferGridTexture, ImageComponent giLightingGridTexture, int level )
//{
//	var indirectData = context.FrameData.IndirectLightingFrameData;

//	var ambientLighting = indirectData.Owner.AmbientLighting.Value;
//	if( ambientLighting <= 0 )
//		return;

//	var cellSizeOfLevel = indirectData.CellSizeLevel0 * MathEx.Pow( 2.0f, level );
//	var cellSizeOfLevelInv = 1.0f / cellSizeOfLevel;
//	var gridSizeHalf = cellSizeOfLevel * indirectData.GridResolution * 0.5f;
//	var gridPosition = indirectData.GridCenter - new Vector3( gridSizeHalf, gridSizeHalf, gridSizeHalf );
//	var voxelizationFactor = (float)indirectData.Owner.VoxelizationExpandFactor.Value;
//	var gridSize = indirectData.GridResolution;

//	var gridIndexes = new BoundsI( 0, 0, 0, gridSize - 1, gridSize - 1, gridSize - 1 );


//	//!!!!можно без свапа текстур, т.к. полное копирование. где еще так


//	//make copy of lighting texture because can't image load and write from rgba16f
//	var tempLightingTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize, gridSize, gridSize ), PixelFormat.Float16RGBA, 0, false );
//	if( tempLightingTexture == null )
//		return;

//	{
//		if( !GICopyLightingGridProgram.HasValue )
//			return;

//		context.BindComputeImage( 0, tempLightingTexture, 0, ComputeBufferAccessEnum.Write );
//		context.BindTexture( 1, giLightingGridTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
//		//!!!!is not work
//		//context.BindComputeImage( 1, giLightingGridTexture, 0, ComputeBufferAccessEnum.Read );

//		var parameters = stackalloc Vector4F[ 3 ];
//		parameters[ 0 ] = new Vector4F( indirectData.GridResolution, level, 0, 0 );
//		parameters[ 1 ] = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
//		parameters[ 2 ] = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );
//		var uniform = GpuProgramManager.RegisterUniform( "giCopyLightingGridParameters", UniformType.Vector4, 3 );
//		Bgfx.SetUniform( uniform, parameters, 3 );

//		var size = gridIndexes.GetSize() + new Vector3I( 1, 1, 1 );
//		var jobSize = new Vector3I( (int)Math.Ceiling( size.X / 8.0 ), (int)Math.Ceiling( size.Y / 8.0 ), (int)Math.Ceiling( size.Z / 8.0 ) );

//		var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

//		Bgfx.Dispatch( context.CurrentViewNumber, GICopyLightingGridProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, discardFlags );
//		context.UpdateStatisticsCurrent.ComputeDispatches++;
//	}

//	{
//		context.BindComputeImage( 0, giLightingGridTexture, 0, ComputeBufferAccessEnum.Write );
//		context.BindTexture( 1, giGBufferGridTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
//		context.BindTexture( 2, tempLightingTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
//		//context.BindComputeImage( 1, giGBufferGridTexture, 0, ComputeBufferAccessEnum.Read );
//		//context.BindComputeImage( 2, tempLightingTexture, 0, ComputeBufferAccessEnum.Read );

//		var parameters = stackalloc Vector4F[ 4 ];
//		parameters[ 0 ] = new Vector4F( cellSizeOfLevel, voxelizationFactor, indirectData.GridResolution, level );
//		parameters[ 1 ] = new Vector4F( gridPosition.ToVector3F(), 0 );
//		parameters[ 2 ] = new Vector4F( gridIndexes.Minimum.ToVector3F(), (float)ambientLighting );
//		parameters[ 3 ] = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );
//		if( giRenderLightParametersUniform == null )
//			giRenderLightParametersUniform = GpuProgramManager.RegisterUniform( "giRenderLightParameters", UniformType.Vector4, 4 );
//		Bgfx.SetUniform( giRenderLightParametersUniform.Value, parameters, 4 );

//		var ambientLight = frameData.Lights[ frameData.LightsInFrustumSorted[ 0 ] ];
//		//!!!!was
//		//ambientLight.Bind( this, context );

//		BindBrdfLUT( context );

//		if( u_deferredEnvironmentData == null )
//			u_deferredEnvironmentData = GpuProgramManager.RegisterUniform( "u_deferredEnvironmentData", UniformType.Vector4, 4 );
//		if( u_deferredEnvironmentIrradiance == null )
//			u_deferredEnvironmentIrradiance = GpuProgramManager.RegisterUniform( "u_deferredEnvironmentIrradiance", UniformType.Vector4, 9 );

//		GetBackgroundEnvironmentData( context, frameData, out var ambientLightTexture, out var ambientLightIrradiance );

//		context.BindTexture( 8/*"s_environmentTexture"*/, ambientLightTexture.Texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear, 0, true );

//		var data = new DeferredEnvironmentDataUniform();
//		data.rotation = ambientLightTexture.Rotation;
//		data.multiplierAndAffect = ambientLightTexture.MultiplierAndAffect;
//		data.iblRotation = ambientLightIrradiance.Rotation;
//		data.iblMultiplierAndAffect = ambientLightIrradiance.MultiplierAndAffect;
//		Bgfx.SetUniform( u_deferredEnvironmentData.Value, &data, 4 );

//		fixed( Vector4F* harmonics = ambientLightIrradiance.Harmonics ?? EnvironmentIrradianceData.GrayHarmonics )
//			Bgfx.SetUniform( u_deferredEnvironmentIrradiance.Value, harmonics, 9 );


//		Program program;
//		if( indirectData.Owner.ShadingModelFull.Value )
//			program = giDeferredShadingData.passesPerLightWithoutShadowsFull[ 0 ];
//		else
//			program = giDeferredShadingData.passesPerLightWithoutShadowsSimple[ 0 ];

//		var size = gridIndexes.GetSize() + new Vector3I( 1, 1, 1 );
//		var jobSize = new Vector3I( (int)Math.Ceiling( size.X / 8.0 ), (int)Math.Ceiling( size.Y / 8.0 ), (int)Math.Ceiling( size.Z / 8.0 ) );

//		var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

//		Bgfx.Dispatch( context.CurrentViewNumber, program, jobSize.X, jobSize.Y, jobSize.Z, discardFlags );
//		context.UpdateStatisticsCurrent.ComputeDispatches++;
//	}

//	context.DynamicTexture_Free( tempLightingTexture );
//}

//[MethodImpl( (MethodImplOptions)512 )]
//unsafe void RenderDirectLightDeferredGI( ViewportRenderingContext context, FrameData frameData, ImageComponent giGBufferGridTexture, ImageComponent giLightingGridTexture, int level, LightItem lightItem )
//{
//	var indirectData = context.FrameData.IndirectLightingFrameData;

//	var directLighting = indirectData.Owner.DirectLighting.Value;
//	if( directLighting <= 0 )
//		return;

//	var cellSizeOfLevel = indirectData.CellSizeLevel0 * MathEx.Pow( 2.0f, level );
//	var cellSizeOfLevelInv = 1.0f / cellSizeOfLevel;
//	var gridSizeHalf = cellSizeOfLevel * indirectData.GridResolution * 0.5f;
//	var gridPosition = indirectData.GridCenter - new Vector3( gridSizeHalf, gridSizeHalf, gridSizeHalf );
//	var voxelizationFactor = (float)indirectData.Owner.VoxelizationExpandFactor.Value;
//	var gridSize = indirectData.GridResolution;


//	BoundsI gridIndexes;

//	if( lightItem.data.Type == Light.TypeEnum.Point )
//	{
//		var lightBounds = new Bounds( lightItem.data.Position );
//		lightBounds.Expand( lightItem.data.AttenuationFar );

//		if( !indirectData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref lightBounds, out gridIndexes ) )
//			return;
//	}
//	else if( lightItem.data.Type == Light.TypeEnum.Spotlight )
//	{
//		//!!!!slowly

//		//Spotlight
//		var outer = lightItem.data.SpotlightOuterAngle;
//		if( outer > 179 )
//			outer = 179;
//		var radius = lightItem.data.AttenuationFar * Math.Tan( outer.InRadians() / 2 );
//		var height = lightItem.data.AttenuationFar;
//		SimpleMeshGenerator.GenerateCone( 0, SimpleMeshGenerator.ConeOrigin.Center, radius, height, 16, true, true, out Vector3[] positions, out var indices );

//		var mat = new Matrix4(
//			lightItem.data.Rotation.ToMatrix3() * Matrix3.FromRotateByY( Math.PI ) * Matrix3.FromScale( new Vector3( 1.1, 1.1, 1.1 ) ),
//			lightItem.data.Position + lightItem.data.Rotation.GetForward() * lightItem.data.AttenuationFar / 2 );

//		for( int n = 0; n < positions.Length; n++ )
//			positions[ n ] = mat * positions[ n ];

//		var lightBounds = new Bounds( positions[ 0 ] );
//		for( int n = 1; n < positions.Length; n++ )
//			lightBounds.Add( ref positions[ n ] );

//		if( !indirectData.GetGridIndexes( ref gridPosition, ref cellSizeOfLevelInv, ref lightBounds, out gridIndexes ) )
//			return;
//	}
//	else
//		gridIndexes = new BoundsI( 0, 0, 0, gridSize - 1, gridSize - 1, gridSize - 1 );


//	//!!!!может использовать другой формат чтобы поменьше конвертировать

//	//!!!!? can be parallel if write to different images and combine at the end. depends how made gpu internally

//	//make copy of lighting texture because can't image load and write from rgba16f
//	var tempLightingTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize, gridSize, gridSize ), PixelFormat.Float16RGBA, 0, false );
//	if( tempLightingTexture == null )
//		return;

//	{
//		if( !GICopyLightingGridProgram.HasValue )
//			return;

//		context.BindComputeImage( 0, tempLightingTexture, 0, ComputeBufferAccessEnum.Write );
//		context.BindTexture( 1, giLightingGridTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
//		//!!!!is not work
//		//context.BindComputeImage( 1, giLightingGridTexture, 0, ComputeBufferAccessEnum.Read );

//		var parameters = stackalloc Vector4F[ 3 ];
//		parameters[ 0 ] = new Vector4F( indirectData.GridResolution, level, 0, 0 );
//		parameters[ 1 ] = new Vector4F( gridIndexes.Minimum.ToVector3F(), 0 );
//		parameters[ 2 ] = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );
//		var uniform = GpuProgramManager.RegisterUniform( "giCopyLightingGridParameters", UniformType.Vector4, 3 );
//		Bgfx.SetUniform( uniform, parameters, 3 );

//		var size = gridIndexes.GetSize() + new Vector3I( 1, 1, 1 );
//		var jobSize = new Vector3I( (int)Math.Ceiling( size.X / 8.0 ), (int)Math.Ceiling( size.Y / 8.0 ), (int)Math.Ceiling( size.Z / 8.0 ) );

//		var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

//		Bgfx.Dispatch( context.CurrentViewNumber, GICopyLightingGridProgram.Value, jobSize.X, jobSize.Y, jobSize.Z, discardFlags );
//		context.UpdateStatisticsCurrent.ComputeDispatches++;
//	}

//	{
//		context.BindComputeImage( 0, giLightingGridTexture, 0, ComputeBufferAccessEnum.Write );
//		context.BindTexture( 1, giGBufferGridTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
//		context.BindTexture( 2, tempLightingTexture, TextureAddressingMode.Wrap, FilterOption.Point, FilterOption.Point, FilterOption.Point, 0, false );
//		//context.BindComputeImage( 1, giGBufferGridTexture, 0, ComputeBufferAccessEnum.Read );
//		//context.BindComputeImage( 2, tempLightingTexture, 0, ComputeBufferAccessEnum.Read );

//		var parameters = stackalloc Vector4F[ 4 ];
//		parameters[ 0 ] = new Vector4F( cellSizeOfLevel, voxelizationFactor, indirectData.GridResolution, level );
//		parameters[ 1 ] = new Vector4F( gridPosition.ToVector3F(), 0 );
//		parameters[ 2 ] = new Vector4F( gridIndexes.Minimum.ToVector3F(), (float)directLighting );
//		parameters[ 3 ] = new Vector4F( gridIndexes.Maximum.ToVector3F(), 0 );
//		if( giRenderLightParametersUniform == null )
//			giRenderLightParametersUniform = GpuProgramManager.RegisterUniform( "giRenderLightParameters", UniformType.Vector4, 4 );
//		Bgfx.SetUniform( giRenderLightParametersUniform.Value, parameters, 4 );

//		//!!!!was
//		//lightItem.Bind( this, context );

//		BindBrdfLUT( context );

//		//light mask
//		if( RenderingSystem.LightMask && lightItem.data.Type != Light.TypeEnum.Ambient )
//		{
//			if( lightItem.data.Type == Light.TypeEnum.Point )
//			{
//				var texture = lightItem.data.Mask;
//				if( texture == null || texture.Result.TextureType != ImageComponent.TypeEnum.Cube )
//					texture = ResourceUtility.WhiteTextureCube;

//				context.BindTexture( 4, texture, TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Point, 0, true );
//			}
//			else
//			{
//				var texture = lightItem.data.Mask;
//				if( texture == null || texture.Result.TextureType != ImageComponent.TypeEnum._2D )
//					texture = ResourceUtility.WhiteTexture2D;

//				var clamp = lightItem.data.Type == Light.TypeEnum.Spotlight;
//				context.BindTexture( 4, texture, clamp ? TextureAddressingMode.Clamp : TextureAddressingMode.Wrap, FilterOption.Linear, FilterOption.Linear, FilterOption.Point, 0, true );
//			}
//		}

//		//shadowMap
//		{
//			ImageComponent texture;
//			if( lightItem.prepareShadows )
//			{
//				//!!!!
//				texture = null;//lightItem.shadowTexture;
//			}
//			else
//			{
//				if( lightItem.data.Type == Light.TypeEnum.Point )
//					texture = nullShadowTextureCube;
//				else
//					texture = nullShadowTexture2D;
//			}

//			if( RenderingSystem.ShadowTextureFormat == ProjectSettingsPage_Rendering.ShadowTextureFormatEnum.Byte4 )
//				context.BindTexture( 5, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point, 0, true );
//			else
//				context.BindTexture( 5, texture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point, TextureFlags.CompareLessEqual, true );
//		}

//		Program program;
//		if( indirectData.Owner.ShadingModelFull.Value )
//		{
//			if( lightItem.prepareShadows )
//				program = giDeferredShadingData.passesPerLightWithShadowsFull[ (int)lightItem.data.Type ];
//			else
//				program = giDeferredShadingData.passesPerLightWithoutShadowsFull[ (int)lightItem.data.Type ];
//		}
//		else
//		{
//			if( lightItem.prepareShadows )
//				program = giDeferredShadingData.passesPerLightWithShadowsSimple[ (int)lightItem.data.Type ];
//			else
//				program = giDeferredShadingData.passesPerLightWithoutShadowsSimple[ (int)lightItem.data.Type ];
//		}

//		var size = gridIndexes.GetSize() + new Vector3I( 1, 1, 1 );
//		var jobSize = new Vector3I( (int)Math.Ceiling( size.X / 8.0 ), (int)Math.Ceiling( size.Y / 8.0 ), (int)Math.Ceiling( size.Z / 8.0 ) );

//		var discardFlags = /*DiscardFlags.Bindings | */DiscardFlags.IndexBuffer | DiscardFlags.InstanceData | DiscardFlags.State | DiscardFlags.Transform | DiscardFlags.VertexStreams;

//		Bgfx.Dispatch( context.CurrentViewNumber, program, jobSize.X, jobSize.Y, jobSize.Z, discardFlags );
//		context.UpdateStatisticsCurrent.ComputeDispatches++;
//	}

//	context.DynamicTexture_Free( tempLightingTexture );
//}

//[MethodImpl( (MethodImplOptions)512 )]
//protected unsafe virtual void RenderLightsDeferredGI( ViewportRenderingContext context, FrameData frameData, ImageComponent giGBufferGridTexture, ImageComponent giLightingGridTexture, int level )
//{
//	if( giDeferredShadingData == null )
//		InitGIDeferredShadingData();

//	foreach( var lightIndex in frameData.LightsInFrustumSorted )
//	{
//		var lightItem = frameData.Lights[ lightIndex ];

//		if( lightItem.data.Type == Light.TypeEnum.Ambient )
//			RenderEnvironmentLightDeferredGI( context, frameData, giGBufferGridTexture, giLightingGridTexture, level );
//		else
//			RenderDirectLightDeferredGI( context, frameData, giGBufferGridTexture, giLightingGridTexture, level, lightItem );
//	}
//}

//protected internal unsafe virtual void RenderIndirectLightingFullTechnique( ViewportRenderingContext context, FrameData frameData, ref ImageComponent deferredLightTexture, ImageComponent normalTexture, ImageComponent gBuffer2Texture, ImageComponent gBuffer3Texture, ImageComponent gBuffer4Texture, ImageComponent gBuffer5Texture, ImageComponent motionAndObjectIdTexture, ImageComponent depthTexture )
//{
//	var indirectFrameData = frameData.IndirectLightingFrameData;
//	var indirectLighting = indirectFrameData.Owner;
//	var gridSize = indirectFrameData.GridResolution;
//	var levels = indirectFrameData.Levels;
//	//var cellSizeLevel0 = indirectFrameData.CellSizeLevel0;
//	//var gridBounds = indirectFrameData.GridBounds;


//	if( motionAndObjectIdTexture == null || depthTexture == null )
//		return;
//	if( indirectLighting.Intensity <= 0 )
//		return;
//	var multiplier = /*indirectLighting.Multiplier * */RenderingEffect_IndirectLighting.GlobalMultiplier;
//	if( multiplier <= 0 )
//		return;

//	var pipeline = context.RenderingPipeline;
//	if( !pipeline.GetUseMultiRenderTargets() )
//		return;


//	//!!!!temp
//	//check shader files exists
//	if( !giShaderFilesExists.HasValue )
//	{
//		giShaderFilesExists = VirtualFile.Exists( @"Base\Shaders\GIClearGBufferGrid.sc" );

//		if( !giShaderFilesExists.Value )
//			Log.Warning( "Indirect Lighting: Shader files are not exist." );
//	}
//	if( !giShaderFilesExists.Value )
//		return;


//	//get current viewport context data

//	//var anyDataKey = GetIndirectLightingDataKey();

//	//ViewportContextData viewportContextData = null;
//	//{
//	//	context.AnyDataAutoDispose.TryGetValue( anyDataKey, out var current );
//	//	viewportContextData = current as ViewportContextData;
//	//}

//	//var recreate = false;
//	//if( viewportContextData != null )
//	//{
//	//	//if( viewportContextData.ObjectsTextureCreatedCount != fullModeData.objects.Count ||
//	//	//	viewportContextData.ObjectTypesTextureCreatedCount != fullModeData.objectTypes.Count ||
//	//	//	viewportContextData.DirectRadianceTextureCreatedCount != fullModeData.directRadiance.Count )
//	//	//{
//	//	//	recreate = true;
//	//	//}
//	//}

//	////if( !EngineApp._DebugCapsLock )
//	////	recreate = true;


//	////create new
//	//if( viewportContextData == null || recreate )
//	//{


//	//	//!!!!impl partial dynamic update



//	//	//delete old
//	//	if( viewportContextData != null )
//	//	{
//	//		viewportContextData.Dispose();
//	//		viewportContextData = null;

//	//		context.AnyDataAutoDispose.Remove( anyDataKey );
//	//	}


//	//	//create context data
//	//	viewportContextData = new ViewportContextData();
//	//	context.AnyDataAutoDispose[ anyDataKey ] = viewportContextData;

//	//	//create and copy textures

//	//}


//	//alloc gbuffer texture

//	//Atomic limitations
//	//There are some severe limitations on image atomic operations. First, atomics can only be used on integer images, either signed or unsigned. Second, they can only be used on images with the GL_R32I/r32i or GL_R32UI/r32ui formats.

//	var giGBufferGridTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize * levels, gridSize, gridSize ), PixelFormat.R32G32_UInt, 0, false );//R32G32B32A32_UInt
//	if( giGBufferGridTexture == null )
//		return;
//	context.ObjectsDuringUpdate.namedTextures[ "giGBufferGridTexture" ] = giGBufferGridTexture;

//	//alloc lighting texture

//	var giLightingGridTexture = context.DynamicTexture_Alloc( ViewportRenderingContext.DynamicTextureType.ComputeWrite, new Vector3I( gridSize * levels, gridSize, gridSize ), PixelFormat.Float16RGBA, 0, false );
//	if( giLightingGridTexture == null )
//		return;
//	context.ObjectsDuringUpdate.namedTextures[ "giLightingGridTexture" ] = giLightingGridTexture;


//	//enable view for compute operations
//	context.SetComputePass();

//	GIClearGBufferGrid( context, frameData, giGBufferGridTexture );
//	GIClearLightingGrid( context, frameData, giLightingGridTexture );

//	for( int level = 0; level < levels; level++ )
//		Render3DSceneGI( context, frameData, giGBufferGridTexture, giLightingGridTexture, level );

//	//render lights
//	{
//		//!!!!может быть лучше перебирать источники, внутри перебирать уровни

//		for( int level = 0; level < levels; level++ )
//			RenderLightsDeferredGI( context, frameData, giGBufferGridTexture, giLightingGridTexture, level );
//	}


//	//voxel cone tracing

//	var lightingTextureSize = deferredLightTexture.Result.ResultSize / (int)indirectLighting.ResolutionFull.Value;

//	var lightingTexture = context.RenderTarget2D_Alloc( lightingTextureSize, deferredLightTexture.Result.ResultFormat );
//	{
//		context.SetViewport( lightingTexture.Result.GetRenderTarget().Viewports[ 0 ] );

//		var shader = new CanvasRenderer.ShaderItem();
//		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
//		shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Full_Lighting_fs.sc";

//		//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, giGBufferGridTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 1, giLightingGridTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.None ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 2, normalTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 3, gBuffer2Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 4, gBuffer3Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 5, gBuffer4Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 6, gBuffer5Texture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.Point ) );

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 7, depthTexture, TextureAddressingMode.Clamp, FilterOption.Point, FilterOption.Point, FilterOption.None ) );

//		//shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 8, BrdfLUT, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Linear ) );

//		//NativeMethods.bgfx_set_vertex_buffer( 0, objectsBuffer.NativeObjectHandle, 0, -1 );
//		////Bgfx.SetVertexBuffer( 0, objectsBuffer.NativeObjectHandle );

//		//!!!!как лучше разложить настройки
//		//!!!!!еще есть Intensity эффекта
//		shader.Parameters.Set( "diffuseIntensity", (float)indirectLighting.DiffuseOutput.Value );
//		shader.Parameters.Set( "specularIntensity", (float)indirectLighting.SpecularOutput.Value );
//		shader.Parameters.Set( "multiplier", (float)multiplier );

//		var data = indirectFrameData;

//		shader.Parameters.Set( "gridParameters", new Vector4F( data.Levels, data.GridResolution, data.CellSizeLevel0, 0 ) );
//		//!!!!double
//		shader.Parameters.Set( "gridCenter", data.GridCenter.ToVector3F() );

//		shader.Parameters.Set( "traceParameters", new Vector4F( (float)indirectLighting.TraceLength.Value, (float)indirectLighting.TraceStartOffset.Value, (float)indirectLighting.TraceStepFactor.Value, 0 ) );

//		//!!!!double
//		context.Owner.CameraSettings.GetViewProjectionInverseMatrixAbsolute().ToMatrix4F( out var invViewProjMatrix );

//		//!!!!double
//		shader.Parameters.Set( "cameraPosition", context.Owner.CameraSettings.Position.ToVector3F() );

//		//shader.Parameters.Set( "viewProj", viewProjMatrix );
//		shader.Parameters.Set( "invViewProj", invViewProjMatrix );


//		context.RenderQuadToCurrentViewport( shader );


//		//var traceLength = indirectLighting.TraceLength.Value * indirectLighting.Distance.Value;
//		//var distanceLevel0 = data.CellSizeLevel0 * data.GridResolution * 0.5f;
//		//var traceLength = (float)indirectLighting.TraceLength.Value * distanceLevel0;

//		//shader.Parameters.Set( "gridSize", (float)gridSize );
//		////!!!!double
//		//shader.Parameters.Set( "gridPosition", indirectFrameData.GridPosition.ToVector3F() );
//		//shader.Parameters.Set( "cellSize", (float)indirectFrameData.CellSize );
//	}


//	//!!!!apply depth comparison. bilateral. где еще
//	//blur
//	var blurTexture = pipeline.GaussianBlur( context, lightingTexture, indirectLighting.BlurFactor, indirectLighting.BlurDownscalingMode, indirectLighting.BlurDownscalingValue );


//	//free lighting texture
//	if( lightingTexture != null )
//		context.DynamicTexture_Free( lightingTexture );


//	//write to deferredLightTexture with Add blending
//	{
//		context.SetViewport( deferredLightTexture.Result.GetRenderTarget().Viewports[ 0 ] );

//		var shader = new CanvasRenderer.ShaderItem();
//		shader.VertexProgramFileName = @"Base\Shaders\EffectsCommon_vs.sc";
//		shader.FragmentProgramFileName = @"Base\Shaders\Effects\IndirectLighting\Full_Final_fs.sc";

//		shader.Parameters.Set( new ViewportRenderingContext.BindTextureData( 0, blurTexture, TextureAddressingMode.Clamp, FilterOption.Linear, FilterOption.Linear, FilterOption.Point ) );

//		shader.Parameters.Set( "intensity", (float)indirectLighting.Intensity );

//		context.RenderQuadToCurrentViewport( shader, CanvasRenderer.BlendingType.Add );
//	}


//	//free targets
//	context.DynamicTexture_Free( blurTexture );

//	//used later for debug visualization
//	//context.DynamicTexture_Free( giGBufferGridTexture );
//	//context.DynamicTexture_Free( giLightingGridTexture );
//}

