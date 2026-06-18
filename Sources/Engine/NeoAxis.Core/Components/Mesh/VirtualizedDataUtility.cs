//// Copyright 2006–2026 Ivan Efimov. All rights reserved.
//using System;
//using System.Linq;
//using System.Collections.Generic;

//namespace NeoAxis
//{
//	static class VirtualizedDataUtility
//	{
//		//!!!!
//		//!!!!ili yuzat rayAABBIntersect
//		//!!!!https://alain.xyz/blog/ray-tracing-acceleration-structures
//		static bool LineAABBIntersection( ref Vector3F origin, ref Vector3F rayStep, ref Vector3F boundsMin, ref Vector3F boundsMax )
//		{
//			float tNear = float.MinValue;
//			float tFar = float.MaxValue;

//			for( int axis = 0; axis < 3; ++axis )
//			{
//				float rayOnAxis = rayStep[ axis ];
//				float originOnAxis = origin[ axis ];
//				float minOnAxis = boundsMin[ axis ];
//				float maxOnAxis = boundsMax[ axis ];
//				if( rayOnAxis == 0 )
//				{
//					if( originOnAxis < minOnAxis || maxOnAxis < originOnAxis )
//						return false;
//				}
//				else
//				{
//					float rayOnAxisInv = 1.0f / rayOnAxis;
//					float t0 = ( minOnAxis - originOnAxis ) * rayOnAxisInv;
//					float t1 = ( maxOnAxis - originOnAxis ) * rayOnAxisInv;

//					float tMin = Math.Min( t0, t1 );
//					float tMax = Math.Max( t0, t1 );

//					tNear = Math.Max( tNear, tMin );
//					tFar = Math.Min( tFar, tMax );

//					if( tFar < 0.0 || tFar < tNear || 1.0 < tNear )
//						return false;
//				}
//			}

//			return true;
//		}

//		public static unsafe Mesh.CompiledData.RayCastResult VirtualizedRayCast( byte[] virtualizedData, Ray ray, Mesh.CompiledData.RayCastModes mode, bool twoSided )
//		{
//			//!!!!twoSided

//			if( virtualizedData == null )
//				return null;
//			if( virtualizedData.Length < sizeof( MeshGeometry.VirtualizedDataHeader ) )
//				return null;

//			var rayF = ray.ToRayF();

//			fixed( byte* pVirtualizedData = virtualizedData )
//			{
//				var header = (MeshGeometry.VirtualizedDataHeader*)pVirtualizedData;

//				if( header->Version == 1 )
//				{
//					var fullFormat = ( header->Flags & MeshGeometry.VirtualizedDataHeader.FlagsEnum.FullFormat ) != 0;
//					var vertexCount = header->VertexCount;
//					var triangleCount = header->TriangleCount;

//					var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );
//					var verticesSizeInBytes = header->VertexCount * vertexSizeInBytes;
//					var trianglesSizeInBytes = header->TriangleCount * 16;
//					var nodesSizeInBytes = header->NodeCount * 32;

//					var pVertices = pVirtualizedData + sizeof( MeshGeometry.VirtualizedDataHeader );
//					var pTriangles = pVertices + verticesSizeInBytes;
//					var pNodes = pTriangles + trianglesSizeInBytes;

//					//if( sizeof( VirtualizedDataHeader ) + verticesSizeInBytes + trianglesSizeInBytes + nodesSizeInBytes != virtualizedData.Length )
//					//{
//					//	error = "Invalid structure.";
//					//	return false;
//					//}


//					var bestResultScale = 0.0f;
//					byte* bestResultVertex0 = null;
//					byte* bestResultVertex1 = null;
//					byte* bestResultVertex2 = null;
//					var bestResultFullFormat = false;


//					const int STACK_SIZE = 1024;
//					//#define BVH_STACK_SIZE 32
//					//#define BVH_FLT_MAX 3.402823466e+38f

//					var stack = stackalloc int[ STACK_SIZE ];

//					int stackIndex = 0;
//					stack[ stackIndex++ ] = 0;

//					//!!!!
//					int maxStackIndex = 0;
//					int steps = 0;

//					while( stackIndex != 0 )
//					{
//						stackIndex--;

//						steps++;

//						//!!!! * 2 premultiple
//						int bvhIndexOffset = stack[ stackIndex ] * 2;

//						//!!!!bez pakovaniya v 32 bytes vlazit. sravnit

//						var data0 = (Vector4F*)( pNodes + ( bvhIndexOffset + 0 ) * 16 );
//						//vec4 data0 = getVirtualizedData( clusterData, clusterTextureSize, nodesOffset + bvhIndexOffset + 0 );

//						var data0Half = (HalfType*)data0;
//						var boundsMin = new Vector3F( *( data0Half + 0 ), *( data0Half + 1 ), data0->Y );
//						var boundsMax = new Vector3F( *( data0Half + 4 ), *( data0Half + 5 ), data0->W );
//						//vec3 boundsMin = vec3( unpackHalf2x16( asuint( data0.x ) ), data0.y );
//						//vec3 boundsMax = vec3( unpackHalf2x16( asuint( data0.z ) ), data0.w );


//						//!!!!yuzat rayAABBIntersect? ili https://alain.xyz/blog/ray-tracing-acceleration-structures
//						//bool intersects2;
//						//float intersectScale2;
//						//rayAABBIntersect( localRayOrigin, localRayDirection, boundsMin, boundsMax, intersects2, intersectScale2 );

//						if( LineAABBIntersection( ref rayF.Origin, ref rayF.Direction, ref boundsMin, ref boundsMax ) ) //if( intersects2 )
//						{
//							var data1 = (Vector4F*)( pNodes + ( bvhIndexOffset + 1 ) * 16 );
//							//vec4 data1 = getVirtualizedData( clusterData, clusterTextureSize, nodesOffset + bvhIndexOffset + 1 );

//							//!!!!can merge to 2 ints

//							int triangleId = (int)data1->Z; // -1 if data is not leaf
//							if( triangleId < 0 )
//							{
//								//branch node					
//								if( stackIndex + 1 >= STACK_SIZE )
//									break;

//								stack[ stackIndex++ ] = (int)data1->X;//leftIdx;
//								stack[ stackIndex++ ] = (int)data1->Y;//rightIdx;

//								if( stackIndex > maxStackIndex )
//									maxStackIndex = stackIndex;
//							}
//							else
//							{
//								//leaf node

//								int triangleCount2 = (int)data1->W;
//								for( int n = 0; n < triangleCount2; n++ )
//								{
//									var triangleId2 = triangleId + n;

//									var pTriangle = pTriangles + triangleId2 * 16;

//									var index0 = (int)*(float*)( pTriangle + 0 );
//									var index1 = (int)*(float*)( pTriangle + 4 );
//									var index2 = (int)*(float*)( pTriangle + 8 );
//									//!!!!use
//									var materialIndex = (int)*(float*)( pTriangle + 12 );

//									var pVertex0 = pVertices + index0 * vertexSizeInBytes;
//									var pVertex1 = pVertices + index1 * vertexSizeInBytes;
//									var pVertex2 = pVertices + index2 * vertexSizeInBytes;

//									//!!!!slowly
//									var v0 = *(Vector3F*)( pVertex0 + 0 );
//									var v1 = *(Vector3F*)( pVertex1 + 0 );
//									var v2 = *(Vector3F*)( pVertex2 + 0 );

//									if( MathAlgorithms.IntersectTriangleRay( ref v0, ref v1, ref v2, ref rayF, out var scale ) )
//									{
//										if( bestResultVertex0 == null || scale < bestResultScale )
//										{
//											bestResultScale = scale;
//											bestResultVertex0 = pVertex0;
//											bestResultVertex1 = pVertex1;
//											bestResultVertex2 = pVertex2;
//											bestResultFullFormat = fullFormat;
//										}
//									}
//								}
//							}
//						}
//					}

//					//!!!!был максимум 14 на Bag.obj
//					//Log.Info( maxStackIndex.ToString() + " " + steps.ToString() );

//					if( bestResultVertex0 != null )
//					{
//						var pVertex0 = bestResultVertex0;
//						var pVertex1 = bestResultVertex1;
//						var pVertex2 = bestResultVertex2;
//						var pVertices2 = stackalloc byte*[ 3 ] { pVertex0, pVertex1, pVertex2 };

//						var v0 = *(Vector3F*)( pVertex0 + 0 );
//						var v1 = *(Vector3F*)( pVertex1 + 0 );
//						var v2 = *(Vector3F*)( pVertex2 + 0 );

//						var r = new Mesh.CompiledData.RayCastResult();
//						r.Scale = bestResultScale;
//						MathAlgorithms.CalculateTriangleNormal( ref v0, ref v1, ref v2, out r.Normal );
//						r.ContainsVertexInfo = true;

//						for( int n = 0; n < 3; n++ )
//						{
//							var pVertex = pVertices2[ n ];

//							var v = new StandardVertex();
//							v.TexCoord0.X = *(HalfType*)( pVertex + 12 );
//							v.TexCoord0.Y = *(HalfType*)( pVertex + 14 );
//							v.Normal.X = *(HalfType*)( pVertex + 16 );
//							v.Normal.Y = *(HalfType*)( pVertex + 18 );
//							v.Normal.Z = *(HalfType*)( pVertex + 20 );
//							v.Tangent.X = *(HalfType*)( pVertex + 24 );
//							v.Tangent.Y = *(HalfType*)( pVertex + 26 );
//							v.Tangent.Z = *(HalfType*)( pVertex + 28 );
//							v.Tangent.W = *(HalfType*)( pVertex + 30 );

//							if( bestResultFullFormat )
//							{
//								v.TexCoord1.X = *(HalfType*)( pVertex + 32 );
//								v.TexCoord1.Y = *(HalfType*)( pVertex + 34 );
//								v.TexCoord2.X = *(HalfType*)( pVertex + 36 );
//								v.TexCoord2.Y = *(HalfType*)( pVertex + 38 );
//								v.Color.Red = *(HalfType*)( pVertex + 40 );
//								v.Color.Green = *(HalfType*)( pVertex + 42 );
//								v.Color.Blue = *(HalfType*)( pVertex + 44 );
//								v.Color.Alpha = *(HalfType*)( pVertex + 46 );
//							}

//							switch( n )
//							{
//							case 0: v.Position = v0; r.Vertex0 = v; break;
//							case 1: v.Position = v1; r.Vertex1 = v; break;
//							case 2: v.Position = v2; r.Vertex2 = v; break;
//							}
//						}

//						return r;
//					}
//				}
//			}

//			return null;
//		}
//	}
//}













//var clusterCount = header->ClusterCount;

//for( int nCluster = 0; nCluster < clusterCount; nCluster++ )
//{
//	var clusterHeader = (MeshGeometry.ClusterDataHeaderClusterInfo*)( pVirtualizedData + sizeof( MeshGeometry.VirtualizedDataHeader ) + sizeof( MeshGeometry.ClusterDataHeaderClusterInfo ) * nCluster );

//	var trianglesMode = ( clusterHeader->Flags & MeshGeometry.ClusterDataHeaderClusterInfo.FlagsEnum.TrianglesMode ) != 0;
//	var fullFormat = ( clusterHeader->Flags & MeshGeometry.ClusterDataHeaderClusterInfo.FlagsEnum.FullFormat ) != 0;

//	if( !trianglesMode )
//	{
//		//clustered

//		var clusterRotationInv = clusterHeader->Rotation.GetInverse();
//		var clusterSpaceRayOrigin = clusterRotationInv * ( rayF.Origin - clusterHeader->Position );
//		var clusterSpaceRayDirection = clusterRotationInv * rayF.Direction;
//		var clusterSpaceRay = new RayF( clusterSpaceRayOrigin, clusterSpaceRayDirection );

//		var size = new Vector3F( clusterHeader->GridSize.ToVector2F() * clusterHeader->CellSize, clusterHeader->Height );
//		var localBounds = new BoundsF( Vector3F.Zero, size );

//		if( localBounds.Intersects( ref clusterSpaceRay, out var intersectScale ) )
//		{
//			var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );

//			var vertexCount = clusterHeader->ActualVertexCount;
//			var triangleCount = clusterHeader->ActualTriangleCount;

//			var pGrid = pVirtualizedData + clusterHeader->DataPositionInBytes;
//			var pCellTriangles = pGrid + clusterHeader->GridSize.X * clusterHeader->GridSize.Y * 16;
//			var pTriangles = pCellTriangles + clusterHeader->CellTriangleBatches * 16;
//			var pVertices = pTriangles + ( clusterHeader->ActualTriangleCount + 1 ) / 2 * 16;


//			//!!!!
//			var useGrid = true;
//			var useGrid2 = true;
//			//var useGrid = false;


//			//!!!!
//			if( useGrid2 )
//			{
//				var cellSize = clusterHeader->CellSize;
//				var gridSize = clusterHeader->GridSize;
//				var clusterHeight = clusterHeader->Height;


//				var trianglesToCheck = new List<int>( 16 );

//				for( int y = 0; y < gridSize.Y; y++ )
//				{
//					for( int x = 0; x < gridSize.X; x++ )
//					{
//						var indexOfVoxel = getClusterBufferIndexOfVoxel( gridSize, new Vector2I( x, y ) );
//						var voxelValue4 = *(Vector4F*)( pGrid + indexOfVoxel * 16 );
//						//vec4 voxelValue4 = getClusterValue( clusterData, clusterTextureSize, gridOffset + indexOfVoxel );
//						float cellHeight = voxelValue4.X;


//						{
//							//got intersected with the cell. add may collide triangles to trianglesToCheck

//							int cellTrianglesCode = (int)voxelValue4.Y;

//							//process triangles of the cell
//							if( cellTrianglesCode >= 0 )
//							{
//								int cellTrianglesIndex = cellTrianglesCode / 2;
//								bool cellTrianglesTwoBatches = ( cellTrianglesCode % 2 ) != 0;

//								//get height ranges and triangle indexes

//								//!!!!
//								var maxHeightOfCurrentCell = -0.001f;

//								//batch 1
//								{
//									var cellTriangles = *(Vector4F*)( pCellTriangles + ( cellTrianglesIndex + 0 ) * 16 );
//									//uvec4 cellTriangles = asuint( getClusterValue( clusterData, clusterTextureSize, cellTrianglesOffset + cellTrianglesIndex + 0 ) );

//									for( int n = 0; n < 4; n++ )
//									{
//										var v = cellTriangles[ n ];
//										var v2 = (HalfType*)&v;

//										var triangleMaxHeight = (float)*( v2 + 0 );
//										//uint v = cellTriangles[ n ];
//										//float triangleMaxHeight = f16tof32( v & 0xffff );

//										//no sense
//										//if( triangleMaxHeight < 0.0 )
//										//	break;

//										if( triangleMaxHeight > maxHeightOfCurrentCell )
//										{
//											var triangleId = (int)*( v2 + 1 );
//											//int triangleId = (int)f16tof32( v >> 16 );

//											//!!!!maybe check triangle intersect here?
//											trianglesToCheck.Add( triangleId );
//										}
//									}
//								}

//								//batch 2
//								if( cellTrianglesTwoBatches )
//								{
//									var cellTriangles = *(Vector4F*)( pCellTriangles + ( cellTrianglesIndex + 1 ) * 16 );
//									//uvec4 cellTriangles = asuint( getClusterValue( clusterData, clusterTextureSize, cellTrianglesOffset + cellTrianglesIndex + 1 ) );

//									for( int n = 0; n < 4; n++ )
//									{
//										var v = cellTriangles[ n ];
//										var v2 = (HalfType*)&v;

//										var triangleMaxHeight = (float)*( v2 + 0 );
//										//uint v = cellTriangles[ n ];
//										//float triangleMaxHeight = f16tof32( v & 0xffff );

//										//no sense
//										//if( triangleMaxHeight < 0.0 )
//										//	break;

//										if( triangleMaxHeight > maxHeightOfCurrentCell )
//										{
//											var triangleId = (int)*( v2 + 1 );
//											//int triangleId = (int)f16tof32( v >> 16 );

//											//!!!!maybe check triangle intersect here?
//											trianglesToCheck.Add( triangleId );
//										}
//									}
//								}
//							}
//						}

//					}
//				}


//				for( int n = 0; n < trianglesToCheck.Count; n++ )
//				{
//					var triangleId = trianglesToCheck[ n ];

//					var pTriangle = pTriangles + triangleId * 8;

//					var index0 = (int)*(HalfType*)( pTriangle + 0 );
//					var index1 = (int)*(HalfType*)( pTriangle + 2 );
//					var index2 = (int)*(HalfType*)( pTriangle + 4 );

//					var pVertex0 = pVertices + index0 * vertexSizeInBytes;
//					var pVertex1 = pVertices + index1 * vertexSizeInBytes;
//					var pVertex2 = pVertices + index2 * vertexSizeInBytes;

//					//!!!!slowly
//					var v0 = *(Vector3F*)( pVertex0 + 0 );
//					var v1 = *(Vector3F*)( pVertex1 + 0 );
//					var v2 = *(Vector3F*)( pVertex2 + 0 );

//					if( MathAlgorithms.IntersectTriangleRay( ref v0, ref v1, ref v2, ref rayF, out var scale ) )
//					{
//						if( bestResultVertex0 == null || scale < bestResultScale )
//						{
//							bestResultScale = scale;
//							bestResultVertex0 = pVertex0;
//							bestResultVertex1 = pVertex1;
//							bestResultVertex2 = pVertex2;
//							bestResultFullFormat = fullFormat;
//						}
//					}
//				}

//			}
//			else if( useGrid )
//			{
//				var cellSize = clusterHeader->CellSize;
//				var gridSize = clusterHeader->GridSize;
//				var clusterHeight = clusterHeader->Height;

//				var currentPosition = clusterSpaceRayOrigin + clusterSpaceRayDirection * intersectScale;
//				var boundsIntersect = currentPosition;

//				var currentIndex = ( currentPosition.ToVector2() / cellSize ).ToVector2I();
//				MathEx.Clamp( ref currentIndex.X, 0, gridSize.X - 1 );
//				MathEx.Clamp( ref currentIndex.Y, 0, gridSize.Y - 1 );

//				//!!!!
//				var rayOutsideBounds = false;

//				var trianglesToCheck = new List<int>( 16 );


//				//!!!!
//				const int maxSteps = 1000;
//				for( int nIteration = 0; nIteration < maxSteps; nIteration++ )
//				{

//					Vector3F outPositionOfCurrentCell;
//					{
//						var origin = currentPosition;
//						var farDirection = clusterSpaceRayDirection * 300.0f * cellSize;

//						var backFrom = origin + farDirection;
//						var backDirection = -farDirection;

//						//!!!!sense z margins?
//						var cellMin = new Vector3F( currentIndex.X * cellSize, currentIndex.Y * cellSize, -300.0f * cellSize );
//						var cellMax = new Vector3F( cellMin.X + cellSize, cellMin.Y + cellSize, clusterHeight + 300.0f * cellSize );

//						//!!!!may work different than shader function
//						var b = new BoundsF( cellMin, cellMax );
//						var r = new RayF( backFrom, backDirection );
//						b.Intersects( ref r, out var intersectScale2 );
//						//bool intersects2;
//						//float intersectScale2;
//						//rayAABBIntersect( backFrom, backDirection, cellMin, cellMax, intersects2, intersectScale2 );

//						outPositionOfCurrentCell = backFrom + backDirection * intersectScale2;
//					}

//					//!!!!
//					float maxHeightOfCurrentCell = -0.001f;
//					//float maxHeightOfCurrentCell = Math.Min( outPositionOfCurrentCell.Z, currentPosition.Z );

//					//get cell info
//					var indexOfVoxel = getClusterBufferIndexOfVoxel( gridSize, currentIndex );
//					var voxelValue4 = *(Vector4F*)( pGrid + indexOfVoxel * 16 );
//					//vec4 voxelValue4 = getClusterValue( clusterData, clusterTextureSize, gridOffset + indexOfVoxel );
//					float cellHeight = voxelValue4.X;

//					//!!!!
//					//bool underSurface = maxHeightOfCurrentCell <= cellHeight;


//					//!!!!if( underSurface )
//					{
//						//got intersected with the cell. add may collide triangles to trianglesToCheck

//						int cellTrianglesCode = (int)voxelValue4.Y;

//						//process triangles of the cell
//						if( cellTrianglesCode >= 0 )
//						{
//							int cellTrianglesIndex = cellTrianglesCode / 2;
//							bool cellTrianglesTwoBatches = ( cellTrianglesCode % 2 ) != 0;

//							//get height ranges and triangle indexes

//							//batch 1
//							{
//								var cellTriangles = *(Vector4F*)( pCellTriangles + ( cellTrianglesIndex + 0 ) * 16 );
//								//uvec4 cellTriangles = asuint( getClusterValue( clusterData, clusterTextureSize, cellTrianglesOffset + cellTrianglesIndex + 0 ) );

//								for( int n = 0; n < 4; n++ )
//								{
//									var v = cellTriangles[ n ];
//									var v2 = (HalfType*)&v;

//									var triangleMaxHeight = (float)*( v2 + 0 );
//									//uint v = cellTriangles[ n ];
//									//float triangleMaxHeight = f16tof32( v & 0xffff );

//									//no sense
//									//if( triangleMaxHeight < 0.0 )
//									//	break;

//									if( triangleMaxHeight > maxHeightOfCurrentCell )
//									{
//										var triangleId = (int)*( v2 + 1 );
//										//int triangleId = (int)f16tof32( v >> 16 );

//										//!!!!maybe check triangle intersect here?
//										trianglesToCheck.Add( triangleId );
//									}
//								}
//							}

//							//batch 2
//							if( cellTrianglesTwoBatches )
//							{
//								var cellTriangles = *(Vector4F*)( pCellTriangles + ( cellTrianglesIndex + 1 ) * 16 );
//								//uvec4 cellTriangles = asuint( getClusterValue( clusterData, clusterTextureSize, cellTrianglesOffset + cellTrianglesIndex + 1 ) );

//								for( int n = 0; n < 4; n++ )
//								{
//									var v = cellTriangles[ n ];
//									var v2 = (HalfType*)&v;

//									var triangleMaxHeight = (float)*( v2 + 0 );
//									//uint v = cellTriangles[ n ];
//									//float triangleMaxHeight = f16tof32( v & 0xffff );

//									//no sense
//									//if( triangleMaxHeight < 0.0 )
//									//	break;

//									if( triangleMaxHeight > maxHeightOfCurrentCell )
//									{
//										var triangleId = (int)*( v2 + 1 );
//										//int triangleId = (int)f16tof32( v >> 16 );

//										//!!!!maybe check triangle intersect here?
//										trianglesToCheck.Add( triangleId );
//									}
//								}
//							}
//						}
//					}


//					//!!!!
//					var newPosition = outPositionOfCurrentCell + clusterSpaceRayDirection * cellSize * 0.001f;

//					currentPosition = newPosition;
//					currentIndex = ( currentPosition.ToVector2() / cellSize ).ToVector2I();

//					if( currentPosition.X < 0.0 || currentIndex.X >= gridSize.X ||
//						currentPosition.Y < 0.0 || currentIndex.Y >= gridSize.Y ||
//						currentPosition.Z < 0.0 || currentPosition.Z >= clusterHeight )
//					{
//						//the ray is outside the cluster
//						rayOutsideBounds = true;
//						break;
//					}
//				}

//				for( int n = 0; n < trianglesToCheck.Count; n++ )
//				{
//					var triangleId = trianglesToCheck[ n ];

//					var pTriangle = pTriangles + triangleId * 8;

//					var index0 = (int)*(HalfType*)( pTriangle + 0 );
//					var index1 = (int)*(HalfType*)( pTriangle + 2 );
//					var index2 = (int)*(HalfType*)( pTriangle + 4 );

//					var pVertex0 = pVertices + index0 * vertexSizeInBytes;
//					var pVertex1 = pVertices + index1 * vertexSizeInBytes;
//					var pVertex2 = pVertices + index2 * vertexSizeInBytes;

//					//!!!!slowly
//					var v0 = *(Vector3F*)( pVertex0 + 0 );
//					var v1 = *(Vector3F*)( pVertex1 + 0 );
//					var v2 = *(Vector3F*)( pVertex2 + 0 );

//					if( MathAlgorithms.IntersectTriangleRay( ref v0, ref v1, ref v2, ref rayF, out var scale ) )
//					{
//						if( bestResultVertex0 == null || scale < bestResultScale )
//						{
//							bestResultScale = scale;
//							bestResultVertex0 = pVertex0;
//							bestResultVertex1 = pVertex1;
//							bestResultVertex2 = pVertex2;
//							bestResultFullFormat = fullFormat;
//						}
//					}
//				}
//			}
//			else
//			{
//				//no grid

//				for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
//				{
//					var pTriangle = pTriangles + nTriangle * 8;

//					var index0 = (int)*(HalfType*)( pTriangle + 0 );
//					var index1 = (int)*(HalfType*)( pTriangle + 2 );
//					var index2 = (int)*(HalfType*)( pTriangle + 4 );

//					var pVertex0 = pVertices + index0 * vertexSizeInBytes;
//					var pVertex1 = pVertices + index1 * vertexSizeInBytes;
//					var pVertex2 = pVertices + index2 * vertexSizeInBytes;

//					//!!!!slowly
//					var v0 = *(Vector3F*)( pVertex0 + 0 );
//					var v1 = *(Vector3F*)( pVertex1 + 0 );
//					var v2 = *(Vector3F*)( pVertex2 + 0 );

//					if( MathAlgorithms.IntersectTriangleRay( ref v0, ref v1, ref v2, ref rayF, out var scale ) )
//					{
//						if( bestResultVertex0 == null || scale < bestResultScale )
//						{
//							bestResultScale = scale;
//							bestResultVertex0 = pVertex0;
//							bestResultVertex1 = pVertex1;
//							bestResultVertex2 = pVertex2;
//							bestResultFullFormat = fullFormat;
//						}
//					}
//				}
//			}
//		}
//	}
//	else
//	{
//		//separate

//		//!!!!octree

//		var vertexCount = clusterHeader->ActualVertexCount;
//		var triangleCount = clusterHeader->ActualTriangleCount;

//		var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );
//		var pVertices = pVirtualizedData + clusterHeader->DataPositionInBytes;
//		var pTriangles = pVertices + clusterHeader->ActualVertexCount * vertexSizeInBytes;

//		for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
//		{
//			var sourceTrianglePointer = pTriangles + nTriangle * 12;
//			var index0 = *(int*)( sourceTrianglePointer + 0 );
//			var index1 = *(int*)( sourceTrianglePointer + 4 );
//			var index2 = *(int*)( sourceTrianglePointer + 8 );

//			var pVertex0 = pVertices + index0 * vertexSizeInBytes;
//			var pVertex1 = pVertices + index1 * vertexSizeInBytes;
//			var pVertex2 = pVertices + index2 * vertexSizeInBytes;

//			//!!!!slowly
//			var v0 = *(Vector3F*)( pVertex0 + 0 );
//			var v1 = *(Vector3F*)( pVertex1 + 0 );
//			var v2 = *(Vector3F*)( pVertex2 + 0 );

//			if( MathAlgorithms.IntersectTriangleRay( ref v0, ref v1, ref v2, ref rayF, out var scale ) )
//			{
//				if( bestResultVertex0 == null || scale < bestResultScale )
//				{
//					bestResultScale = scale;
//					bestResultVertex0 = pVertex0;
//					bestResultVertex1 = pVertex1;
//					bestResultVertex2 = pVertex2;
//					bestResultFullFormat = fullFormat;
//				}
//			}
//		}
//	}
//}

