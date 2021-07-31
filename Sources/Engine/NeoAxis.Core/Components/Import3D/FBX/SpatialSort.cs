//// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System.Collections.Generic;

//namespace NeoAxis.Import.FBX
//{
//	//ToDo : Можно немного ускорить сравнивая ControlPointIndex перед сравнением: if( ( positions[i].Position - position ).LengthSquared() < squaredRadius ) При условии что части Mesh не будут трансформироваться по отдельности, и равенство ControolPointIndex означает равенство координат.
//	//From Assimp

//	/// <summary>
//	/// A little helper class to quickly find all vertices in the epsilon environment of a given
//	/// position. Construct an instance with an array of positions. The class stores the given positions
//	/// by their indices and sorts them by their distance to an arbitrary chosen plane.
//	/// You can then query the instance for all vertices close to a given position in an average O(log n)
//	/// time, with O(n) worst case complexity when all vertices lay on the plane. The plane is chosen
//	/// so that it avoids common planes in usual data sets. */
//	/// </summary>
//	class SpatialSort
//	{
//		//Normal of the sorting plane, normalized. The center is always at (0, 0, 0) 
//		readonly Vector3F planeNormal;

//		struct Entry
//		{
//			public readonly int Index; // The vertex referred by this entry
//			public readonly Vector3F Position; // Position
//			public readonly float Distance; // Distance of this vertex to the sorting plane


//			public Entry( int index, Vector3F position, float distance )
//			{
//				Index = index;
//				Position = position;
//				Distance = distance;
//			}

//			//public static Entry Default { get; } = new Entry( 999999999, new Vec3F(), 99999); //Default constructor in Assimp assigned such values
//			//public static bool operator <( Entry e1, Entry e2 ) { return e1.Distance < e2.Distance; }
//			//public static bool operator >( Entry e1, Entry e2 ) { return e1.Distance > e2.Distance; }
//		}

//		class EntryComparer : IComparer<Entry>
//		{
//			public int Compare( Entry x, Entry y )
//			{
//				if( x.Distance < y.Distance )
//					return -1;
//				if( x.Distance > y.Distance )
//					return 1;
//				return 0;
//			}
//		}

//		// all positions, sorted by distance to the sorting plane
//		readonly List<Entry> positions = new List<Entry>();

//		public SpatialSort( VertexInfo[] vertices )
//		{
//			// define the reference plane. We choose some arbitrary vector away from all basic axises
//			// in the hope that no model spreads all its vertices along this plane.
//			planeNormal = new Vector3F( 0.8523f, 0.34321f, 0.5736f );
//			Append( vertices );
//		}

//		void Append( VertexInfo[] vertices )
//		{
//			// store references to all given positions along with their distance to the reference plane
//			for( int i = 0; i < vertices.Length; i++ )
//			{
//				float distance = Vector3F.Dot( vertices[ i ].Vertex.Position, planeNormal );
//				positions.Add( new Entry( i, vertices[ i ].Vertex.Position, distance ) );
//			}
//			// now sort the array ascending by distance.
//			positions.Sort( new EntryComparer() );
//		}

//		/// <summary>
//		/// Finds all positions close to the given position
//		/// </summary>
//		/// <param name="position">The position to look for vertices.</param>
//		/// <param name="radius">Maximal distance from the position a vertex may have to be counted in.</param>
//		/// <returns></returns>
//		public List<int> FindPositions( Vector3F position, float radius )
//		{
//			float dist = Vector3F.Dot( position, planeNormal );
//			float minDist = dist - radius;
//			float maxDist = dist + radius;

//			var results = new List<int>();
//			// quick check for positions outside the range
//			if( positions.Count == 0 )
//				return results;
//			if( maxDist < positions[ 0 ].Distance )
//				return results;
//			if( minDist > positions[ positions.Count - 1 ].Distance )
//				return results;

//			//int index = BinarySerach( positions, minDist ); //Warning : В Assimp был такой BinarySearch

//			// do a binary search for the minimal distance to start the iteration there
//			int index = positions.BinarySearch( new Entry( -1, new Vector3F(), minDist ), new EntryComparer() );
//			if( index < 0 )
//				index = ~index;
//			else
//				index++;

//			// Now start iterating from there until the first position lays outside of the distance range.
//			// Add all positions inside the distance range within the given radius to the result aray
//			float squaredRadius = radius * radius;
//			for( int i = index; i < positions.Count && positions[ i ].Distance < maxDist; i++ )
//			{
//				if( ( positions[ i ].Position - position ).LengthSquared() < squaredRadius )
//					results.Add( positions[ i ].Index );
//			}
//			return results;
//		}

//		//Warning : В Assimp был такой BinarySearch
//		//static int BinarySerach( List<Entry> positions, float minDist )
//		//{
//		//	int index = positions.Count / 2;
//		//	int binaryStepSize = positions.Count / 4;
//		//	while( binaryStepSize > 1 )
//		//	{
//		//		if( positions[index].Distance < minDist )
//		//			index += binaryStepSize;
//		//		else
//		//			index -= binaryStepSize;

//		//		binaryStepSize /= 2;
//		//	}
//		//	// depending on the direction of the last step we need to single step a bit back or forth
//		//	// to find the actual beginning element of the range
//		//	while( index > 0 && positions[index].Distance > minDist )
//		//		index--;
//		//	while( index < ( positions.Count - 1 ) && positions[index].Distance < minDist )
//		//		index++;
//		//	return index;
//		//}
//	}
//}
