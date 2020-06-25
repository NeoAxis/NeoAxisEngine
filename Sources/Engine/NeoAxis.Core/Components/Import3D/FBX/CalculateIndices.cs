// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Import.FBX
{
	//ToDo : Сравнивать BoneAssignment
	static class CalcIndices
	{
		public static void CalculateIndicesBySpatialSort( MeshData data, out StandardVertex[] vertices, out int[] indices )
		{
			VertexInfo[] oldVertices = data.Vertices;

			SpatialSort vertexFinder = data.CalcCache.VertexFinder;
			float positionEpsilon = data.CalcCache.PositionEpsilon;

			indices = new int[ oldVertices.Length ];
			//for each old vertex contains a new index to newVerticesList. While processing if not -1 then this vertex is already processed
			for( int i = 0; i < indices.Length; i++ )
				indices[ i ] = -1;
			var newVerticesList = new List<StandardVertex>( data.Vertices.Length );

			for( int i = 0; i < oldVertices.Length; i++ )
			{
				if( indices[ i ] != -1 ) //already processed
					continue;

				List<int> closeVerticesOldIndices = vertexFinder.FindPositions( oldVertices[ i ].Vertex.Position, positionEpsilon );
				System.Diagnostics.Debug.Assert( closeVerticesOldIndices.Count != 0 ); //at least must find itself
				List<int> closeAndEqualVerticesOldIndices = new List<int>();

				//select only equal vertices in the group and exclude i
				for( int j = 0; j < closeVerticesOldIndices.Count; j++ )
				{
					if( closeVerticesOldIndices[ j ] != i && oldVertices[ i ].Vertex.Equals( oldVertices[ closeVerticesOldIndices[ j ] ].Vertex, positionEpsilon ) )
						closeAndEqualVerticesOldIndices.Add( closeVerticesOldIndices[ j ] );
				}

				int assignedIndex = -1;
				//find any already assigned index in the group 
				for( int j = 0; j < closeAndEqualVerticesOldIndices.Count; j++ )
				{
					assignedIndex = indices[ closeAndEqualVerticesOldIndices[ j ] ];
					if( assignedIndex != -1 )
						break;
				}

				if( assignedIndex == -1 ) //all unassigned
				{
					assignedIndex = newVerticesList.Count;
					newVerticesList.Add( data.Vertices[ i ].Vertex );
				}

				//now fill all anassigned verex indices in the close group
				indices[ i ] = assignedIndex;
				for( int j = 0; j < closeAndEqualVerticesOldIndices.Count; j++ )
				{
					ref int curIndexRef = ref indices[ closeAndEqualVerticesOldIndices[ j ] ];
					if( curIndexRef == -1 )
						curIndexRef = assignedIndex;
				}
			}

			vertices = newVerticesList.ToArray();
		}

		public static void CalculateIndicesByOctree( MeshData data, out StandardVertex[] vertices, out int[] indices )
		{
			float epsilon = data.CalcCache.PositionEpsilon;
			List<StandardVertex> verticesList = new List<StandardVertex>( data.Vertices.Length );
			indices = new int[ data.Vertices.Length ];

			var bounds = Bounds.Cleared;
			for( int i = 0; i < data.Vertices.Length; i++ )
				bounds.Add( data.Vertices[ i ].Vertex.Position );
			bounds.Expand( epsilon * 2 );

			var initSettings = new OctreeContainer.InitSettings();
			initSettings.InitialOctreeBounds = bounds;
			initSettings.OctreeBoundsRebuildExpand = Vector3.Zero;
			initSettings.MinNodeSize = bounds.GetSize() / 100;// 50;
			var octreeContainer = new OctreeContainer( initSettings );

			for( int i = 0; i < data.Vertices.Length; i++ )
			{
				var p = data.Vertices[ i ].Vertex.Position;
				var b = new Bounds( p - new Vector3F( epsilon, epsilon, epsilon ), p + new Vector3F( epsilon, epsilon, epsilon ) );

				int newIndex = -1;
				//!!!!check by Vec3 position
				int[] result = octreeContainer.GetObjects( b, 0xFFFFFFFF, OctreeContainer.ModeEnum.All );
				bool found = false;

				for( int j = 0; j < result.Length; j++ )
				{
					if( verticesList[ result[ j ] ].Equals( ref data.Vertices[ i ].Vertex, epsilon ) )
					{
						found = true;
						newIndex = result[ j ];
						break;
					}
				}

				if( !found )
				{
					newIndex = verticesList.Count;
					int indexC = octreeContainer.AddObject( b, 1 ); //assigned index is the number of element in the container
					System.Diagnostics.Debug.Assert( newIndex == indexC );
					verticesList.Add( data.Vertices[ i ].Vertex );
				}

				indices[ i ] = newIndex;
			}
			vertices = verticesList.ToArray();
		}

		/*
		  //2 times slower than CalculateIndicesBySpatialSort on the model with vertices.Count == 61836 
		public static void CalculateIndicesBySpatialSort2( MeshData data, out StandardVertexF[] vertices, out int[] indices )
		{
			VertexInfo[] oldVertices = data.Vertices;
			var newVerticesList = new List<StandardVertexF>( data.Vertices.Length );
			indices = new int[data.Vertices.Length];
			
			SpatialSort vertexFinder = data.CalcCache.VertexFinder;
			float positionEpsilon = data.CalcCache.PositionEpsilon;

			Dictionary<int,int> oldToNewIndexes = new Dictionary<int, int>();
			
			for( int i = 0; i < oldVertices.Length; i++ )
			{
				var p = oldVertices[i].Vertex.position;
				//var b = new Bounds( p - new Vec3F( epsilon, epsilon, epsilon ), p + new Vec3F( epsilon, epsilon, epsilon ) );

				int foundNewIndex = -1; //index to newVerticesList
				List<int> closeVerticesOldIndexes = vertexFinder.FindPositions( p, positionEpsilon );
				for( int j = 0; j < closeVerticesOldIndexes.Count; j++ )
				{
					int closeVerticesOldIndex = closeVerticesOldIndexes[j];
					if( closeVerticesOldIndex == i) //found itself
						continue;

					if( oldVertices[closeVerticesOldIndex].Vertex.Equals( oldVertices[i].Vertex, positionEpsilon ) )
					{
						// if found then this vertex is already added to newVerticesList
						// if not found continue the search among the close vertices, which one is added
						if( oldToNewIndexes.TryGetValue( closeVerticesOldIndex, out int ind ) )
						{
							foundNewIndex = ind;
							break;
						}
					}
				}

				if( foundNewIndex != -1 )
				{
					indices[i] = foundNewIndex;
				}
				else
				{
					//if( foundOldIndex != -1 ) //есть близкий но не добавленный.
					newVerticesList.Add( data.Vertices[i].Vertex );
					indices[i] = newVerticesList.Count - 1;
					oldToNewIndexes[i] = newVerticesList.Count - 1;
				}
			}
			vertices = newVerticesList.ToArray();
		}
		*/



		//static bool BoneAssignmentEqual( BoneAssignment a1, BoneAssignment a2 )
		//{
		//	// ReSharper disable CompareOfFloatsByEqualityOperator
		//	return a1.count == a2.count &&
		//		   a1.boneIndex0 == a2.boneIndex0 &&
		//		   a1.boneIndex1 == a2.boneIndex1 &&
		//		   a1.boneIndex2 == a2.boneIndex2 &&
		//		   a1.boneIndex3 == a2.boneIndex3 &&
		//		   a1.weight0 == a2.weight0 &&
		//		   a1.weight1 == a2.weight1 &&
		//		   a1.weight2 == a2.weight2 &&
		//		   a1.weight3 == a2.weight3;
		//	// ReSharper restore CompareOfFloatsByEqualityOperator
		//	//return a1.Equals( a2 );
		//}

	}
}
