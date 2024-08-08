// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;
using Internal.SharpBgfx;

namespace NeoAxis
{
	public partial class RenderingPipeline_Basic
	{
		// Don't add many non static fields. Rendering pipeline is created for each temporary render target during frame rendering.

		static bool tessellationProgramUniformsInitialized;
		static Program tessellationPrepareProgram;
		static Uniform u_tessellationParametersUniform;
		static Uniform u_tessellationSourceVertexLayout;
		static Uniform u_tessellationDestVertexLayout;

		///////////////////////////////////////////////

		void TessellationClearOldNotUsedItems( ViewportRenderingContext context, bool deleteAll )
		{
			if( context.TessellationCacheItems != null )
			{
				var toRemove = new List<(RenderSceneData.MeshDataRenderOperation operation, Material.CompiledMaterialData materialData)>();

				foreach( var pair in context.TessellationCacheItems )
				{
					var key = pair.Key;
					var item = pair.Value;

					if( !item.LastTimeUsed || deleteAll )
					{
						toRemove.Add( key );
						item.Dispose();
					}

					item.LastTimeUsed = false;
				}

				if( toRemove.Count != 0 )
				{
					foreach( var key in toRemove )
						context.TessellationCacheItems.Remove( key );
				}
			}
		}

		void TessellationPrepare( ViewportRenderingContext context )
		{
			var frameData = context.FrameData;
			var tessellationQuality = (float)TessellationQuality;

			//check scene settings are changed
			if( context.TessellationCacheItemsCalculatedForQuality != tessellationQuality )
			{
				TessellationClearOldNotUsedItems( context, true );
				context.TessellationCacheItemsCalculatedForQuality = tessellationQuality;
			}

			if( RenderingSystem.Tessellation && tessellationQuality > 0 )
			{
				if( context.TessellationCacheItems == null )
					context.TessellationCacheItems = new Dictionary<(RenderSceneData.MeshDataRenderOperation operation, Material.CompiledMaterialData materialData), ViewportRenderingContext.TessellationCacheItem>();

				//select items to calculate
				var itemsToCalculate = new ESet<(RenderSceneData.MeshDataRenderOperation operation, Material.CompiledMaterialData materialData)>();
				{
					//var viewportOwner = context.Owner;
					var frameDataRenderSceneDataMeshes = frameData.RenderSceneData.Meshes;
					//var frameDataMeshes = frameData.Meshes;

					using( var renderableGroupsToDraw = new OpenListNative<RenderableGroupWithDistance>( frameData.RenderableGroupsInFrustum.Count ) )
					{
						foreach( var renderableGroup in frameData.RenderableGroupsInFrustum )
						{
							if( renderableGroup.X == 0 )
							{
								ref var meshItem = ref frameDataRenderSceneDataMeshes.Data[ renderableGroup.Y ];

								if( meshItem.Tessellation )
								{
									var meshData = meshItem.MeshData;

									for( int nOperation = 0; nOperation < meshData.RenderOperations.Count; nOperation++ )
									{
										var oper = meshData.RenderOperations[ nOperation ];
										if( oper.ContainsDisposedBuffers() )
											continue;

										//!!!!multi materials
										//foreach( var materialData in GetMeshMaterialData( ref meshItem, oper, nOperation, false, context.DeferredShading ) )

										Material material;
										if( meshItem.ReplaceMaterial != null )
											material = meshItem.ReplaceMaterial;
										else
											material = oper.Material;

										//!!!!impl
										//if( meshItem.ReplaceMaterialSelectively != null )
										//{
										//	for( int n = 0; n < meshItem.ReplaceMaterialSelectively.Length; n++ )
										//	{
										//		var result = meshItem.ReplaceMaterialSelectively[ n ].Result;
										//		if( result != null && result.tessellation )
										//			enable = true;
										//	}
										//}

										var materialData = material?.Result;
										if( materialData != null && materialData.tessellationQuality != 0 )
											itemsToCalculate.AddWithCheckAlreadyContained( (oper, materialData) );
									}
								}
							}
						}


						//!!!!need when use limit
						////sort by distance
						//CollectionUtility.MergeSortUnmanaged( renderableGroupsToDraw.Data, renderableGroupsToDraw.Count, delegate ( RenderableGroupWithDistance* a, RenderableGroupWithDistance* b )
						//{
						//	if( a->DistanceSquared < b->DistanceSquared )
						//		return -1;
						//	if( a->DistanceSquared > b->DistanceSquared )
						//		return 1;
						//	return 0;
						//}, true );

					}
				}

				//calculate

				if( itemsToCalculate.Count != 0 && !tessellationProgramUniformsInitialized )
				{
					{
						var program = GpuProgramManager.GetProgram( "Tessellation", GpuProgramType.Compute, @"Base\Shaders\TessellationPrepare.sc", null, true, out var error2 );
						if( !string.IsNullOrEmpty( error2 ) )
							Log.Fatal( error2 );
						else
							tessellationPrepareProgram = new Program( program.RealObject );

					}

					u_tessellationParametersUniform = GpuProgramManager.RegisterUniform( "u_tessellationParameters", UniformType.Vector4, 1 );
					u_tessellationSourceVertexLayout = GpuProgramManager.RegisterUniform( "u_tessellationSourceVertexLayout", UniformType.Vector4, 3 );
					u_tessellationDestVertexLayout = GpuProgramManager.RegisterUniform( "u_tessellationDestVertexLayout", UniformType.Vector4, 3 );

					tessellationProgramUniformsInitialized = true;
				}

				var computePassEnabled = false;

				foreach( var key in itemsToCalculate )
				{
					if( !context.TessellationCacheItems.TryGetValue( key, out var item ) )
					{
						//calculate new item
						item = new ViewportRenderingContext.TessellationCacheItem();
						if( TessellationCalculateItem( context, key, item, ref computePassEnabled ) )
							context.TessellationCacheItems[ key ] = item;
					}

					item.LastTimeUsed = true;
				}
			}

			//clear old not used items
			TessellationClearOldNotUsedItems( context, false );
		}

		ViewportRenderingContext.TessellationCacheItem TessellationGetItem( ViewportRenderingContext context, RenderSceneData.MeshDataRenderOperation operation, Material.CompiledMaterialData materialData )
		{
			var key = (operation, materialData);

			var items = context.TessellationCacheItems;
			if( items != null )
			{
				items.TryGetValue( key, out var item );
				return item;
			}

			return null;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		static float GetVertexLayoutElement( VertexElementType type, int offsetInBytes )
		{
			var offsetInFloats = offsetInBytes / 4;
			return offsetInFloats * 256 | (int)type;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		static unsafe void WriteVertexLayout( float* data, VertexElement[] structure )
		{
			for( int nElement = 0; nElement < structure.Length; nElement++ )
			{
				ref var element = ref structure[ nElement ];

				//!!!!can be faster by mean indexing

				switch( element.Semantic )
				{
				case VertexElementSemantic.Position: data[ 0 ] = GetVertexLayoutElement( element.Type, element.Offset ); break;
				case VertexElementSemantic.Normal: data[ 1 ] = GetVertexLayoutElement( element.Type, element.Offset ); break;
				case VertexElementSemantic.Tangent: data[ 2 ] = GetVertexLayoutElement( element.Type, element.Offset ); break;
				case VertexElementSemantic.Color0: data[ 3 ] = GetVertexLayoutElement( element.Type, element.Offset ); break;
				case VertexElementSemantic.TextureCoordinate0: data[ 4 ] = GetVertexLayoutElement( element.Type, element.Offset ); break;
				case VertexElementSemantic.TextureCoordinate1: data[ 5 ] = GetVertexLayoutElement( element.Type, element.Offset ); break;
				case VertexElementSemantic.TextureCoordinate2: data[ 6 ] = GetVertexLayoutElement( element.Type, element.Offset ); break;
				case VertexElementSemantic.BlendIndices: data[ 7 ] = GetVertexLayoutElement( element.Type, element.Offset ); break;
				case VertexElementSemantic.BlendWeights: data[ 8 ] = GetVertexLayoutElement( element.Type, element.Offset ); break;
				case VertexElementSemantic.Color3: data[ 9 ] = GetVertexLayoutElement( element.Type, element.Offset ); break;
					//case VertexElementSemantic.Color2: data[ 10 ] = GetVertexLayoutElement( element.Type, element.Offset ); break;
				}
			}
		}

		unsafe bool TessellationCalculateItem( ViewportRenderingContext context, (RenderSceneData.MeshDataRenderOperation operation, Material.CompiledMaterialData materialData) key, ViewportRenderingContext.TessellationCacheItem item, ref bool computePassEnabled )
		{
			var operation = key.operation;
			var materialData = key.materialData;

			if( operation.VertexBuffers.Length != 1 || operation.IndexBuffer == null || operation.IndexBuffer.IndexCount == 0 )
				return false;

			//!!!!start offset support
			if( operation.IndexStartOffset != 0 || operation.VertexStartOffset != 0 )
				return false;


			//!!!!heightmap
			//height map or compiled shader, height map scale. make simple implementation as beginning without full shader graph support. support only one binded height map as beginning


			//!!!!apply quality. 0.25 - 4.
			var quality = materialData.tessellationQuality * (float)TessellationQuality;


			var sourceVertexBuffer = operation.VertexBuffers[ 0 ];
			var sourceIndexBuffer = operation.IndexBuffer;
			var sourceTriangleCount = operation.IndexBuffer.IndexCount / 3;
			var sourceVertexDeclaration = sourceVertexBuffer.VertexDeclaration;


			VertexElement[] destVertexStructure;
			VertexLayout destVertexDeclaration;

			if( sourceVertexDeclaration.Stride % 16 != 0 )
			{
				//make vertex structure with stride 16 bytes. to write from compute shader

				var destStructure2 = new List<VertexElement>( operation.VertexStructure );

				var gap = 16 - sourceVertexDeclaration.Stride % 16;

				VertexElementType type;
				if( gap == 4 )
					type = VertexElementType.Float1;
				else if( gap == 8 )
					type = VertexElementType.Float2;
				else
					type = VertexElementType.Float3;

				//!!!!если занят Color2

				destStructure2.Add( new VertexElement( 0, sourceVertexDeclaration.Stride, type, VertexElementSemantic.Color2 ) );

				destVertexStructure = destStructure2.ToArray();
				destVertexDeclaration = VertexElements.CreateVertexDeclaration( destVertexStructure, 0 );
			}
			else
			{
				//already with 16 bytes stride
				destVertexStructure = operation.VertexStructure;
				destVertexDeclaration = sourceVertexDeclaration;
			}



			//!!!!
			//var tessellationSteps = 2;

			var destVertexCount = sourceTriangleCount * 4;
			var destIndexCount = sourceTriangleCount * 9;


			//for( var step = 0; step < tessellationSteps; step++ )
			//{
			//}

			//var destVertexCount = sourceTriangleCount * 3;
			//var destIndexCount = sourceTriangleCount * 3;


			//create buffers
			var destVertexBuffer = GpuBufferManager.CreateVertexBuffer( destVertexCount, destVertexDeclaration, GpuBufferFlags.ComputeWrite );
			var destIndexBuffer = GpuBufferManager.CreateIndexBuffer( destIndexCount, GpuBufferFlags.ComputeWrite );


			//calculate

			if( !computePassEnabled )
			{
				computePassEnabled = true;

				//enable view for compute operations
				context.SetComputePass();
			}

			context.BindComputeBuffer( 0, sourceVertexBuffer, ComputeBufferAccessEnum.Read );
			context.BindComputeBuffer( 1, sourceIndexBuffer, ComputeBufferAccessEnum.Read );
			context.BindComputeBuffer( 2, destVertexBuffer, ComputeBufferAccessEnum.Write );
			context.BindComputeBuffer( 3, destIndexBuffer, ComputeBufferAccessEnum.Write );

			int sourceVertexSizeInFloat = sourceVertexDeclaration.Stride / 4;
			int destVertexSizeInFloat = destVertexDeclaration.Stride / 4;

			//int sourceVertexSizeInVec4 = sourceVertexDeclaration.Stride / 16;
			//int destVertexSizeInVec4 = sourceVertexSizeInVec4;//destVertexDeclaration.Stride / 16;

			{
				var parameters = new Vector4F( sourceTriangleCount, sourceVertexSizeInFloat, destVertexSizeInFloat, 0 );
				//var parameters = new Vector4F( sourceTriangleCount, sourceVertexSizeInVec4, destVertexSizeInVec4, 0 );
				Bgfx.SetUniform( u_tessellationParametersUniform, &parameters );
			}

			{
				var data = stackalloc float[ 4 * 3 ];
				NativeUtility.ZeroMemory( data, 4 * 4 * 3 );
				WriteVertexLayout( data, operation.VertexStructure );
				Bgfx.SetUniform( u_tessellationSourceVertexLayout, data, 3 );
			}

			{
				var data = stackalloc float[ 4 * 3 ];
				NativeUtility.ZeroMemory( data, 4 * 4 * 3 );
				WriteVertexLayout( data, destVertexStructure );
				Bgfx.SetUniform( u_tessellationDestVertexLayout, data, 3 );
			}

			var jobSize = new Vector3I( (int)Math.Ceiling( sourceTriangleCount / 512.0 ), 1, 1 );

			//!!!!use context.Dispatch
			Bgfx.Dispatch( (ushort)RenderingSystem.CurrentViewNumber, tessellationPrepareProgram, jobSize.X, jobSize.Y, jobSize.Z, DiscardFlags.All );
			context.UpdateStatisticsCurrent.ComputeDispatches++;


			item.VertexBuffer = destVertexBuffer;
			item.IndexBuffer = destIndexBuffer;

			return true;
		}
	}
}
