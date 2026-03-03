//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//#if !DEPLOY
//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Threading.Tasks;

////!!!!
////-мержить вершины на периметрах.			
////- максимальный размер кластера.			
////- центрирование.			
////- может плохо найденные оставить в куче.
////- учитывать нормаль вершин при CanAddTriangle. или еще как-то.

////- баг на красной тачке.


//namespace NeoAxis
//{
//	public class ClusterMeshSimplifier
//	{
//		//source data
//		public double SimplificationFactor;
//		public byte[] SourceVertices;
//		public VertexElement[] SourceVertexStructure;
//		public StandardVertex[] Vertices;
//		public StandardVertex.Components VertexComponents;
//		public int[] Indices;
//		float MaxClusterSize;

//		//processing data

//		Vector3F[] TriangleNormals;
//		//Dictionary<Vector3F, List<int>> VerticesByPosition;
//		Dictionary<int, List<int>> VerticesByTriangle;
//		Dictionary<int, List<int>> TrianglesByVertex;
//		AdjoiningTrianglesItem[] AdjoiningTriangles;

//		/////////////////////////////////////////

//		struct AdjoiningTrianglesItem
//		{
//			public int[] Triangles;
//		}

//		/////////////////////////////////////////

//		class Cluster
//		{
//			public ClusterMeshSimplifier Owner;

//			public Vector3F Normal;
//			public Vector3F Position;
//			public QuaternionF Rotation;
//			public Matrix4F ClusterSpaceToObjectSpace;
//			public Matrix4F ObjectSpaceToClusterSpace;

//			public ESet<int> Triangles = new ESet<int>( 32 );
//			public BoundsF LocalBounds = BoundsF.Cleared;
//			//!!!!slowly?
//			public List<int> Perimeter = new List<int>( 32 );
//			//public LinkedList<int> Perimeter = new LinkedList<int>();

//			//public bool TrianglesMode;

//			//public float CellSize;
//			//public Vector2I GridSize;
//			//public float Height;

//			//result data
//			public StandardVertex[] ClusterVertices;
//			public int[] ClusterIndices;

//			//

//			public enum CanAddTriangleResult
//			{
//				No,
//				MaybeLater,
//				Yes,
//			}

//			//

//			Line3F GetProjectedToClusterSpace( Line3F line )
//			{
//				return new Line3F( ObjectSpaceToClusterSpace * line.Start, ObjectSpaceToClusterSpace * line.End );
//			}

//			public CanAddTriangleResult CanAddTriangle( int nTriangle, out int perimeterInsertIndex, out int perimiterInsertValue, out int perimeterRemoveIndex )
//			{
//				perimeterInsertIndex = 0;
//				perimiterInsertValue = 0;
//				perimeterRemoveIndex = -1;

//				ref var triNormal = ref Owner.TriangleNormals[ nTriangle ];

//				var degree = MathAlgorithms.GetVectorsAngle( ref triNormal, ref Normal ).InDegrees();


//				//!!!!может угол между нормалями тругольников небольшой, а угол к нормали кластера большой

//				//!!!!
//				//!!!!разное для разного уровня?
//				if( degree > 35 )//70 )
//					return CanAddTriangleResult.No;

//				//if( Triangles.Count >= ClusteredMaxTriangleCount )
//				//	return CanAddTriangleResult.No;


//				GetLocalBoundsWithNewTriangle( nTriangle, out var newBounds );
//				var size = newBounds.GetSize();
//				//check for shape
//				if( size.MinComponent() > 0 )
//				{
//					var h = size.ToVector2().Length();
//					var v = size.Z;

//					//!!!!maybe 5 - 10
//					if( v > h / 8 )
//						return CanAddTriangleResult.MaybeLater;
//				}
//				//!!!!good?
//				//check for max size
//				if( size.X > Owner.MaxClusterSize || size.Y > Owner.MaxClusterSize )
//					return CanAddTriangleResult.No;


//				//если у него две точки на периметре. и рядом (включая зацикленный случай)
//				//линию от новой вершины для центра пересекаем с ребром между двумя другими точками.
//				//если пересекаются, то значит можно добавлять


//				//!!!!Line3 переименовать в Line

//				//!!!!двигать центр кластера, а может и Rotation


//				var index0 = Owner.Indices[ nTriangle * 3 + 0 ];
//				var index1 = Owner.Indices[ nTriangle * 3 + 1 ];
//				var index2 = Owner.Indices[ nTriangle * 3 + 2 ];

//				//!!!!slowly
//				var vertex0PerimeterIndex = Perimeter.IndexOf( index0 );
//				var vertex1PerimeterIndex = Perimeter.IndexOf( index1 );
//				var vertex2PerimeterIndex = Perimeter.IndexOf( index2 );

//				var onPerimeterCount = 0;
//				if( vertex0PerimeterIndex != -1 )
//					onPerimeterCount++;
//				if( vertex1PerimeterIndex != -1 )
//					onPerimeterCount++;
//				if( vertex2PerimeterIndex != -1 )
//					onPerimeterCount++;

//				if( onPerimeterCount <= 1 )
//					return CanAddTriangleResult.MaybeLater;

//				var add = false;

//				if( onPerimeterCount == 2 )
//				{
//					//two points on perimeter. add triangle and add one point from perimeter

//					int perimeterIndex1;
//					int perimeterIndex2;
//					int newVertexIndex;
//					if( vertex0PerimeterIndex == -1 )
//					{
//						perimeterIndex1 = vertex1PerimeterIndex;
//						perimeterIndex2 = vertex2PerimeterIndex;
//						newVertexIndex = index0;
//					}
//					else if( vertex1PerimeterIndex == -1 )
//					{
//						perimeterIndex1 = vertex0PerimeterIndex;
//						perimeterIndex2 = vertex2PerimeterIndex;
//						newVertexIndex = index1;
//					}
//					else
//					{
//						perimeterIndex1 = vertex0PerimeterIndex;
//						perimeterIndex2 = vertex1PerimeterIndex;
//						newVertexIndex = index2;
//					}


//					var lineFromNewVertexToPosition = new Line3F( Owner.Vertices[ newVertexIndex ].Position, Position );
//					var lineFromNewVertexToPositionProjected = GetProjectedToClusterSpace( lineFromNewVertexToPosition );

//					var lineBetweenTwoOnPerimeter = new Line3F(
//						Owner.Vertices[ Perimeter[ perimeterIndex1 ] ].Position,
//						Owner.Vertices[ Perimeter[ perimeterIndex2 ] ].Position );
//					var lineBetweenTwoOnPerimeterProjected = GetProjectedToClusterSpace( lineBetweenTwoOnPerimeter );

//					var intersects = MathAlgorithms.IntersectLineLine( lineFromNewVertexToPositionProjected.ToLine2F(), lineBetweenTwoOnPerimeterProjected.ToLine2F(), out _ );
//					if( !intersects )
//						return CanAddTriangleResult.MaybeLater;

//					if( perimeterIndex1 == perimeterIndex2 - 1 )
//					{
//						add = true;
//						perimeterInsertIndex = perimeterIndex2;
//					}
//					else if( perimeterIndex2 == perimeterIndex1 - 1 )
//					{
//						add = true;
//						perimeterInsertIndex = perimeterIndex1;
//					}
//					else if( perimeterIndex1 == 0 && perimeterIndex2 == Perimeter.Count - 1 )
//					{
//						add = true;
//						perimeterInsertIndex = Perimeter.Count;
//					}
//					else if( perimeterIndex2 == 0 && perimeterIndex1 == Perimeter.Count - 1 )
//					{
//						add = true;
//						perimeterInsertIndex = Perimeter.Count;
//					}

//					perimiterInsertValue = newVertexIndex;
//				}
//				else
//				{
//					//three points on perimeter. add triange and remove one point from perimeter
//					//check it on one line in the perimeter

//					if( ( vertex0PerimeterIndex + 1 ) % Perimeter.Count == vertex1PerimeterIndex &&
//						( vertex0PerimeterIndex + 2 ) % Perimeter.Count == vertex2PerimeterIndex )
//					{
//						add = true;
//						perimeterRemoveIndex = vertex1PerimeterIndex;
//					}
//					else if( ( vertex0PerimeterIndex + 1 ) % Perimeter.Count == vertex2PerimeterIndex &&
//					   ( vertex0PerimeterIndex + 2 ) % Perimeter.Count == vertex1PerimeterIndex )
//					{
//						add = true;
//						perimeterRemoveIndex = vertex2PerimeterIndex;
//					}
//					else if( ( vertex1PerimeterIndex + 1 ) % Perimeter.Count == vertex0PerimeterIndex &&
//						( vertex1PerimeterIndex + 2 ) % Perimeter.Count == vertex2PerimeterIndex )
//					{
//						add = true;
//						perimeterRemoveIndex = vertex0PerimeterIndex;
//					}
//					else if( ( vertex1PerimeterIndex + 1 ) % Perimeter.Count == vertex2PerimeterIndex &&
//					   ( vertex1PerimeterIndex + 2 ) % Perimeter.Count == vertex0PerimeterIndex )
//					{
//						add = true;
//						perimeterRemoveIndex = vertex2PerimeterIndex;
//					}
//					else if( ( vertex2PerimeterIndex + 1 ) % Perimeter.Count == vertex0PerimeterIndex &&
//					   ( vertex2PerimeterIndex + 2 ) % Perimeter.Count == vertex1PerimeterIndex )
//					{
//						add = true;
//						perimeterRemoveIndex = vertex0PerimeterIndex;
//					}
//					else if( ( vertex2PerimeterIndex + 1 ) % Perimeter.Count == vertex1PerimeterIndex &&
//					  ( vertex2PerimeterIndex + 2 ) % Perimeter.Count == vertex0PerimeterIndex )
//					{
//						add = true;
//						perimeterRemoveIndex = vertex1PerimeterIndex;
//					}
//				}

//				if( !add )
//					return CanAddTriangleResult.MaybeLater;

//				return CanAddTriangleResult.Yes;
//			}

//			void GetLocalBoundsWithNewTriangle( int nTriangle, out BoundsF result )
//			{
//				result = LocalBounds;

//				var index0 = Owner.Indices[ nTriangle * 3 + 0 ];
//				var index1 = Owner.Indices[ nTriangle * 3 + 1 ];
//				var index2 = Owner.Indices[ nTriangle * 3 + 2 ];

//				ref var v0 = ref Owner.Vertices[ index0 ].Position;
//				ref var v1 = ref Owner.Vertices[ index1 ].Position;
//				ref var v2 = ref Owner.Vertices[ index2 ].Position;

//				//!!!!maybe precalculate
//				Matrix4F.Multiply( ref ObjectSpaceToClusterSpace, ref v0, out var tv0 );
//				Matrix4F.Multiply( ref ObjectSpaceToClusterSpace, ref v1, out var tv1 );
//				Matrix4F.Multiply( ref ObjectSpaceToClusterSpace, ref v2, out var tv2 );

//				result.Add( ref tv0 );
//				result.Add( ref tv1 );
//				result.Add( ref tv2 );
//			}

//			public void AddTriangle( int nTriangle, int perimeterInsertIndex, int perimiterInsertValue, int perimeterRemoveIndex )
//			{
//				Triangles.Add( nTriangle );

//				GetLocalBoundsWithNewTriangle( nTriangle, out LocalBounds );

//				//add to Perimeter

//				if( Triangles.Count == 1 )
//				{
//					var index0 = Owner.Indices[ nTriangle * 3 + 0 ];
//					var index1 = Owner.Indices[ nTriangle * 3 + 1 ];
//					var index2 = Owner.Indices[ nTriangle * 3 + 2 ];

//					Perimeter.Add( index0 );
//					Perimeter.Add( index1 );
//					Perimeter.Add( index2 );
//				}
//				else
//				{
//					//!!!!slowly
//					if( perimeterRemoveIndex != -1 )
//						Perimeter.RemoveAt( perimeterRemoveIndex );
//					else
//						Perimeter.Insert( perimeterInsertIndex, perimiterInsertValue );
//				}

//				//!!!!нормаль обновлять?

//				//try to update cluster Position
//				{
//					var newCenter = Vector3F.Zero;
//					foreach( var index in Perimeter )
//						newCenter += Owner.Vertices[ index ].Position;
//					newCenter /= Perimeter.Count;

//					var allow = true;

//					//!!!!slowly

//					foreach( var vertexIndex in Perimeter )
//					{
//						var lineFromVertexToPosition = new Line3F( Owner.Vertices[ vertexIndex ].Position, Position );
//						var lineFromVertexToPositionProjected = GetProjectedToClusterSpace( lineFromVertexToPosition );

//						for( var nEdge = 0; nEdge < Perimeter.Count; nEdge++ )
//						{
//							var vertexIndex1 = nEdge;
//							var vertexIndex2 = ( nEdge + 1 ) % Perimeter.Count;

//							if( vertexIndex != vertexIndex1 && vertexIndex != vertexIndex2 )
//							{
//								var edgeLine = new Line3F(
//									Owner.Vertices[ Perimeter[ vertexIndex1 ] ].Position,
//									Owner.Vertices[ Perimeter[ vertexIndex2 ] ].Position );

//								var edgeLineProjected = GetProjectedToClusterSpace( edgeLine );

//								var intersects = MathAlgorithms.IntersectLineLine( lineFromVertexToPositionProjected.ToLine2F(), edgeLineProjected.ToLine2F(), out _ );
//								if( intersects )
//								{
//									allow = false;
//									goto end;
//								}
//							}
//						}
//					}
//					end:;

//					if( allow )
//						Position = newCenter;
//				}
//			}

//			//public bool CalculateActualTrianglesAndRasterizeTriangles()
//			//{
//			//	//!!!!precalculate
//			//	var clusterSpaceToObjectSpace = new Matrix4F( Rotation.ToMatrix3(), Position );
//			//	var objectSpaceToClusterSpace = clusterSpaceToObjectSpace.GetInverse();


//			//	//!!!!slowly?
//			//	//calculate actual triangles. ActualClusterVertices, ActualClusterIndices
//			//	{
//			//		var actualVertices = new List<StandardVertex>( Triangles.Count * 3 );
//			//		var actualIndices = new List<int>( Triangles.Count * 3 );

//			//		foreach( var nTriangle in Triangles )
//			//		{
//			//			var index0 = Group.Indices[ nTriangle * 3 + 0 ];
//			//			var index1 = Group.Indices[ nTriangle * 3 + 1 ];
//			//			var index2 = Group.Indices[ nTriangle * 3 + 2 ];

//			//			ref var v0 = ref Group.Vertices[ index0 ];
//			//			ref var v1 = ref Group.Vertices[ index1 ];
//			//			ref var v2 = ref Group.Vertices[ index2 ];

//			//			actualIndices.Add( actualVertices.Count );
//			//			actualVertices.Add( v0 );
//			//			actualIndices.Add( actualVertices.Count );
//			//			actualVertices.Add( v1 );
//			//			actualIndices.Add( actualVertices.Count );
//			//			actualVertices.Add( v2 );
//			//		}

//			//		ActualClusterVertices = actualVertices.ToArray();
//			//		ActualClusterIndices = actualIndices.ToArray();

//			//		//!!!!slowly? but less amount of matrix multiply
//			//		MathAlgorithms.MergeEqualVertices( ref ActualClusterVertices, ref ActualClusterIndices, 0, 0, false );

//			//		//MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( actualVertices.ToArray(), actualIndices.ToArray(), 0, 0, 0, true, out ActualClusterVertices, out ActualClusterIndices, out _ );
//			//	}

//			//	Grid = new Cell[ GridSize.X, GridSize.Y ];

//			//	var verticesProjected = new Vector3F[ ActualClusterVertices.Length ];
//			//	for( int n = 0; n < verticesProjected.Length; n++ )
//			//	{
//			//		var p = ActualClusterVertices[ n ].Position;
//			//		Matrix4F.Multiply( ref objectSpaceToClusterSpace, ref p, out verticesProjected[ n ] );
//			//		//verticesProjected[ n ] = p;
//			//	}

//			//	for( int nTriangle = 0; nTriangle < ActualClusterIndices.Length / 3; nTriangle++ )
//			//	{
//			//		var index0 = ActualClusterIndices[ nTriangle * 3 + 0 ];
//			//		var index1 = ActualClusterIndices[ nTriangle * 3 + 1 ];
//			//		var index2 = ActualClusterIndices[ nTriangle * 3 + 2 ];

//			//		ref var v0Projected = ref verticesProjected[ index0 ];
//			//		ref var v1Projected = ref verticesProjected[ index1 ];
//			//		ref var v2Projected = ref verticesProjected[ index2 ];

//			//		//ref var v0 = ref ActualClusterVertices[ index0 ].Position;
//			//		//ref var v1 = ref ActualClusterVertices[ index1 ].Position;
//			//		//ref var v2 = ref ActualClusterVertices[ index2 ].Position;

//			//		//var v0Projected = objectSpaceToClusterSpace * v0;
//			//		//var v1Projected = objectSpaceToClusterSpace * v1;
//			//		//var v2Projected = objectSpaceToClusterSpace * v2;


//			//		//!!!!slowly?

//			//		//var cells = new List<Vector2I>( 256 );

//			//		//!!!!

//			//		var triangle = new Triangle2F( v0Projected.ToVector2(), v1Projected.ToVector2(), v2Projected.ToVector2() );


//			//		//!!!!интервал

//			//		for( int y = 0; y < GridSize.Y; y++ )
//			//		{
//			//			for( int x = 0; x < GridSize.X; x++ )
//			//			{
//			//				//var clusterSize = new Vector3( GridSize.ToVector2() * CellSize, Height );

//			//				var b = new RectangleF( CellSize * x, CellSize * y, CellSize * x + CellSize, CellSize * y + CellSize );


//			//				//!!!!temp


//			//				var trb = new RectangleF( triangle.A );
//			//				trb.Add( triangle.B );
//			//				trb.Add( triangle.C );
//			//				if( b.Intersects( trb ) )
//			//				//if( b.Intersects( ref triangle ) )
//			//				{
//			//					ref var cell = ref Grid[ x, y ];
//			//					//ref var cell = ref Grid[ y * GridSize.X + x ];

//			//					if( cell.ActualTriangles == null )
//			//						cell.ActualTriangles = new List<int>( 8 );// new List<(int, float)>( 8 );

//			//					if( cell.ActualTriangles.Count == 8 )
//			//						return false;

//			//					cell.ActualTriangles.Add( nTriangle );

//			//					//float maxHeightInCell = 0.0f;
//			//					//{
//			//					//	var clipBounds = b;

//			//					//	var polygon = new Vector3[ 3 ];
//			//					//	polygon[ 0 ] = v0Projected;
//			//					//	polygon[ 1 ] = v1Projected;
//			//					//	polygon[ 2 ] = v2Projected;

//			//					//	//+X
//			//					//	{
//			//					//		var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Maximum, 0 ), new Vector3( -1, 0, 0 ) );
//			//					//		polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
//			//					//	}

//			//					//	//+Y
//			//					//	{
//			//					//		var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Maximum, 0 ), new Vector3( 0, -1, 0 ) );
//			//					//		polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
//			//					//	}

//			//					//	//-X
//			//					//	{
//			//					//		var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Minimum, 0 ), new Vector3( 1, 0, 0 ) );
//			//					//		polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
//			//					//	}

//			//					//	//-Y
//			//					//	{
//			//					//		var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Minimum, 0 ), new Vector3( 0, 1, 0 ) );
//			//					//		polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
//			//					//	}

//			//					//	foreach( var p in polygon )
//			//					//		maxHeightInCell = Math.Max( maxHeightInCell, (float)p.Z );
//			//					//}

//			//					//cell.ActualTriangles.Add( (nTriangle, maxHeightInCell) );


//			//					//cells.Add( new Vector2I( x, y ) );
//			//					//cells.AddWithCheckAlreadyContained( new Vector2I( x, y ) );
//			//				}


//			//				//var b = new Bounds( CellSize * x, CellSize * y, 0, ( CellSize + 1 ) * x, ( CellSize + 1 ) * y, Height );

//			//				//if( b.Intersects( new Triangle( v0Projected, v1Projected, v2Projected ) ) )
//			//				//{
//			//				//	cells.AddWithCheckAlreadyContained( new Vector2I( x, y ) );
//			//				//}
//			//			}
//			//		}

//			//		//!!!!это неточно, т.к. position округляется
//			//		//MathAlgorithms.Fill2DTriangle(
//			//		//	( v0Projected.ToVector2() / CellSize ).ToVector2I(),
//			//		//	( v1Projected.ToVector2() / CellSize ).ToVector2I(),
//			//		//	( v2Projected.ToVector2() / CellSize ).ToVector2I(),
//			//		//	new RectangleI( 0, 0, GridSize.X + 1, GridSize.Y + 1 ), delegate ( Vector2I point )
//			//		//{
//			//		//	var cellIndex = point;
//			//		//	if( cellIndex.X < GridSize.X && cellIndex.Y < GridSize.Y )
//			//		//		cells.AddWithCheckAlreadyContained( cellIndex );
//			//		//} );

//			//		//MathAlgorithms.Fill2DTriangle(
//			//		//	( v0Projected.ToVector2() / CellSize * 10 ).ToVector2I(),
//			//		//	( v1Projected.ToVector2() / CellSize * 10 ).ToVector2I(),
//			//		//	( v2Projected.ToVector2() / CellSize * 10 ).ToVector2I(),
//			//		//	new RectangleI( 0, 0, ( GridSize.X + 1 ) * 10, ( GridSize.Y + 1 ) * 10 ), delegate ( Vector2I point )
//			//		//	{
//			//		//		var cellIndex = point / 10;
//			//		//		if( cellIndex.X < GridSize.X && cellIndex.Y < GridSize.Y )
//			//		//			cells.AddWithCheckAlreadyContained( cellIndex );
//			//		//	} );

//			//		//foreach( var cellIndex in cells )
//			//		//{
//			//		//	ref var cell = ref Grid[ cellIndex.Y * GridSize.X + cellIndex.X ];

//			//		//	if( cell.Triangles == null )
//			//		//		cell.Triangles = new List<int>( 8 );

//			//		//	if( cell.Triangles.Count == 8 )
//			//		//		return false;

//			//		//	cell.Triangles.Add( nTriangle );
//			//		//}
//			//	}

//			//	return true;
//			//}

//			public double GetScore()
//			{
//				return Triangles.Count;

//				//if( !TrianglesMode )
//				//{
//				//	//!!!!

//				//	var cellsWithTriangles = 0;
//				//	var trianglesDensity = 0;

//				//	for( int y = 0; y < GridSize.Y; y++ )
//				//	{
//				//		for( int x = 0; x < GridSize.X; x++ )
//				//		{
//				//			ref var cell = ref Grid[ x, y ];

//				//			if( cell.ActualTriangles != null && cell.ActualTriangles.Count != 0 )
//				//			{
//				//				cellsWithTriangles++;
//				//				trianglesDensity += cell.ActualTriangles.Count;
//				//			}
//				//		}
//				//	}

//				//	var cellCount = (double)GridSize.X * GridSize.Y;

//				//	var emptySpaceScore = (double)cellsWithTriangles / cellCount;
//				//	var triangleCountScore = (double)Triangles.Count / (double)ClusteredMaxTriangleCount;
//				//	var trianglesDensityScore = (double)trianglesDensity / ( cellCount * 8 );

//				//	//!!!!распределенность в ячейках. т.е. что примерно одинаковое количество

//				//	//!!!!multipliers
//				//	return emptySpaceScore * 1 + triangleCountScore * 1 + trianglesDensityScore * 0;


//				//	//!!!!качество кластера определяется. параметр качества
//				//	//1. полезная площадь
//				//	//2. заполненность треугольников в списках
//				//	// может заполненность и равномерность это разное. например малозаполненно 1-2 треугольника, но равномерно
//				//	////ПУСтота получается тут же учитывается? не, геометрия ведь есть
//				//	//3. размер грида. чем больше, тем меньше кластеров

//				//	//return Triangles.Count;

//				//	////var size2 = GridSize.ToVector2() * CellSize;
//				//	////return size2.X * size2.Y;

//				//}

//				//return 0;
//			}
//		}

//		/////////////////////////////////////////

//		int[] GetAdjoiningTriangles( int nTriangle )
//		{
//			return AdjoiningTriangles[ nTriangle ].Triangles;
//		}

//		Cluster CalculateCluster( ESet<int> trianglesToConsider, int startTriangle )//, int edge )
//		{
//			//нормалью будет нормаль треугольника. и ищем суммарно с соседними, если нормаль удовлетворяет начальному треугольнику
//			Vector3F clusterNormal;
//			{
//				clusterNormal = TriangleNormals[ startTriangle ];

//				//!!!!

//				//!!!!trianglesToConsider

//			}

//			//!!!!не так если нормаль не от одного

//			Vector3F startTriangleCenter;
//			var clusterForward = Vector3F.Zero;
//			{
//				var index0 = Indices[ startTriangle * 3 + 0 ];
//				var index1 = Indices[ startTriangle * 3 + 1 ];
//				var index2 = Indices[ startTriangle * 3 + 2 ];

//				ref var v0 = ref Vertices[ index0 ].Position;
//				ref var v1 = ref Vertices[ index1 ].Position;
//				ref var v2 = ref Vertices[ index2 ].Position;

//				startTriangleCenter = ( v0 + v1 + v2 ) / 3.0f;

//				if( v0 != v1 )
//					clusterForward = ( v1 - v0 ).GetNormalize();
//				else if( v0 != v2 )
//					clusterForward = ( v2 - v0 ).GetNormalize();
//				else if( v1 != v2 )
//					clusterForward = ( v2 - v1 ).GetNormalize();
//			}


//			////!!!!temp
//			//clusterForward = Vector3F.XAxis;
//			//clusterNormal = Vector3F.ZAxis;


//			var cluster = new Cluster();
//			cluster.Owner = this;
//			cluster.AddTriangle( startTriangle, 0, 0, 0 );

//			var degenerate = clusterForward == Vector3F.Zero;
//			if( !degenerate )
//			{
//				cluster.Normal = clusterNormal;
//				cluster.Position = startTriangleCenter;
//				cluster.Rotation = QuaternionF.LookAt( clusterForward, cluster.Normal );
//				cluster.ClusterSpaceToObjectSpace = new Matrix4F( cluster.Rotation.ToMatrix3(), cluster.Position );
//				cluster.ObjectSpaceToClusterSpace = cluster.ClusterSpaceToObjectSpace.GetInverse();

//				var neverCheckAnymore = new ESet<int>( 128 );

//				var toCheck = new ESet<int>( 128 );
//				toCheck.AddRange( GetAdjoiningTriangles( startTriangle ).Where( t => trianglesToConsider.Contains( t ) ) );

//				while( true )
//				{
//					var updated = false;

//					var toCheckCopy = toCheck.ToArray();
//					foreach( var tri in toCheckCopy )
//					{
//						if( !cluster.Triangles.Contains( tri ) )
//						{
//							var canAdd = cluster.CanAddTriangle( tri, out var perimeterInsertIndex, out var perimiterInsertValue, out var perimeterRemoveIndex );

//							if( canAdd == Cluster.CanAddTriangleResult.Yes )
//							{
//								cluster.AddTriangle( tri, perimeterInsertIndex, perimiterInsertValue, perimeterRemoveIndex );

//								toCheck.Remove( tri );

//								//!!!!сначала лучших добавлять?

//								foreach( var t in GetAdjoiningTriangles( tri ) )
//								{
//									if( !cluster.Triangles.Contains( t ) && !neverCheckAnymore.Contains( t ) && trianglesToConsider.Contains( t ) )
//										toCheck.AddWithCheckAlreadyContained( t );
//								}

//								updated = true;
//							}
//							else if( canAdd == Cluster.CanAddTriangleResult.No )
//							{
//								toCheck.Remove( tri );
//								neverCheckAnymore.AddWithCheckAlreadyContained( tri );

//								updated = true;
//							}
//						}
//					}

//					if( !updated )
//						break;
//				}


//				////!!!!какие еще критерии
//				////!!!!может > 20
//				////!!!!
//				//if( cluster.Triangles.Count > 10 )// && false )
//				//{

//				//	//!!!!temp

//				//	////cluster.ActualClusterIndices

//				//	//var localBoundsTemp = BoundsF.Cleared;
//				//	//{
//				//	//	foreach( var nTriangle in cluster.Triangles )
//				//	//	{
//				//	//		var index0 = group.Indices[ nTriangle * 3 + 0 ];
//				//	//		var index1 = group.Indices[ nTriangle * 3 + 1 ];
//				//	//		var index2 = group.Indices[ nTriangle * 3 + 2 ];

//				//	//		ref var v0 = ref group.Vertices[ index0 ].Position;
//				//	//		ref var v1 = ref group.Vertices[ index1 ].Position;
//				//	//		ref var v2 = ref group.Vertices[ index2 ].Position;

//				//	//		localBoundsTemp.Add( v0 );
//				//	//		localBoundsTemp.Add( v1 );
//				//	//		localBoundsTemp.Add( v2 );
//				//	//	}
//				//	//}
//				//	//cluster.Position = localBoundsTemp.Minimum;
//				//	//cluster.Height = localBoundsTemp.GetSize().Z;


//				//	//Log.Info( "--" );
//				//	//{
//				//	//	string q = "";
//				//	//	foreach( var a in cluster.Triangles )
//				//	//		q += " " + a.ToString();
//				//	//	Log.Info( q );
//				//	//}
//				//	//Log.Info( cluster.Position.ToString() );


//				//	var localClusterPosition = cluster.CenteredLocalBounds.Minimum;
//				//	//var offsetFromCenter = cluster.CenteredLocalBounds.Minimum - cluster.CenteredLocalBounds.GetCenter();
//				//	cluster.Position = clusterCenter + cluster.Rotation * localClusterPosition;
//				//	cluster.Height = cluster.CenteredLocalBounds.GetSize().Z;

//				//	////var offsetFromCenter = cluster.CenteredLocalBounds.Minimum - cluster.CenteredLocalBounds.GetCenter();
//				//	////cluster.Position = clusterCenter + cluster.Rotation * offsetFromCenter;
//				//	////cluster.Height = cluster.CenteredLocalBounds.GetSize().Z;

//				//	//if( cluster.Height < 0.000001f )
//				//	//	cluster.Height = 0.000001f;

//				//	//var done = false;


//				//	//!!!!Grid size range

//				//	//!!!!чаще шаг


//				//	////!!!!temp
//				//	////var boundsSize = localBoundsTemp.GetSize();
//				//	//var boundsSize = cluster.CenteredLocalBounds.GetSize();

//				//	////!!!!
//				//	////for( var gridSize = 8; gridSize <= 64; gridSize *= 2 )
//				//	//for( var gridSize = 2; gridSize <= 64; gridSize *= 2 )
//				//	//{
//				//	//	if( cluster.CalculateActualTrianglesAndRasterizeTriangles() )
//				//	//	{
//				//	//		done = true;
//				//	//		break;
//				//	//	}
//				//	//}

//				//	////need try smaller area because reach limit of grid size
//				//	//if( !done )
//				//	//{
//				//	//	//make a new cluster with removed 10% of far triangles


//				//	//	//!!!!часто сюда попадает?


//				//	//	//!!!!
//				//	//	//Log.Info( "!done" );


//				//	//	//!!!!расстояние по прямоугольнику, чтобы было прямоугольнее

//				//	//	float GetTriangleDistanceSquared( int nTriangle )
//				//	//	{
//				//	//		var index0 = group.Indices[ nTriangle * 3 + 0 ];
//				//	//		var index1 = group.Indices[ nTriangle * 3 + 1 ];
//				//	//		var index2 = group.Indices[ nTriangle * 3 + 2 ];

//				//	//		ref var v0 = ref group.Vertices[ index0 ].Position;
//				//	//		ref var v1 = ref group.Vertices[ index1 ].Position;
//				//	//		ref var v2 = ref group.Vertices[ index2 ].Position;

//				//	//		return Math.Max( ( v0 - clusterCenter ).LengthSquared(), Math.Max( ( v1 - clusterCenter ).LengthSquared(), ( v2 - clusterCenter ).LengthSquared() ) );
//				//	//	}

//				//	//	var triangles = cluster.Triangles.ToArray();
//				//	//	CollectionUtility.MergeSort( triangles, delegate ( int tri1, int tri2 )
//				//	//	{
//				//	//		var d1 = GetTriangleDistanceSquared( tri1 );
//				//	//		var d2 = GetTriangleDistanceSquared( tri2 );

//				//	//		if( d1 < d2 )
//				//	//			return -1;
//				//	//		if( d1 > d2 )
//				//	//			return 1;
//				//	//		return 0;
//				//	//	}, false );// true );

//				//	//	//!!!!0.75
//				//	//	var newTriangleCount = (int)( triangles.Length * 0.75f );
//				//	//	if( newTriangleCount < 1 )
//				//	//		newTriangleCount = 1;

//				//	//	var trianglesToConsider2 = new ESet<int>( newTriangleCount + 1 );
//				//	//	trianglesToConsider2.Add( startTriangle );
//				//	//	for( int n = 0; n < newTriangleCount; n++ )
//				//	//		trianglesToConsider2.AddWithCheckAlreadyContained( triangles[ n ] );

//				//	//	//var trianglesToConsider2 = new ESet<int>( trianglesToConsider );
//				//	//	//for( int tri = newTriangleCount + 1; tri < triangles.Length; tri++ )
//				//	//	//{
//				//	//	//	if( tri != startTriangle )
//				//	//	//		trianglesToConsider2.Remove( tri );
//				//	//	//}

//				//	//	//recursive
//				//	//	var cluster2 = CalculateCluster( group, trianglesToConsider2, startTriangle, edge );

//				//	//	return cluster2;
//				//	//}

//				//}
//			}

//			return cluster;
//		}

//		public bool Calculate( out byte[] newVertices, out VertexElement[] newVertexStructure, out int[] newIndices, out string error )
//		{
//			error = "";
//			newVertices = null;
//			newVertexStructure = null;
//			newIndices = null;

//			VertexElements.GetInfo( SourceVertexStructure, out var vertexSize, out _ );

//			if( !MeshGeometry.VerticesExtractStandardVertex( SourceVertices, SourceVertexStructure, out Vertices, out VertexComponents ) )
//			{
//				error = "MeshGeometry.VerticesExtractStandardVertex failed.";
//				return false;
//			}

//			////!!!!
//			////var time = DateTime.Now;

//			//precalculate some data

//			var triangleCount = Indices.Length / 3;

//			var totalBounds = BoundsF.Cleared;
//			{
//				for( int n = 0; n < Vertices.Length; n++ )
//					totalBounds.Add( ref Vertices[ n ].Position );
//				//for( int tri = 0; tri < triangleCount; tri++ )
//				//{
//				//	var index0 = Indices[ tri * 3 + 0 ];
//				//	var index1 = Indices[ tri * 3 + 1 ];
//				//	var index2 = Indices[ tri * 3 + 2 ];

//				//	var bounds = new BoundsF( Vertices[ index0 ].Position );
//				//	bounds.Add( ref Vertices[ index1 ].Position );
//				//	bounds.Add( ref Vertices[ index2 ].Position );

//				//	trianglesBounds[ tri ] = new TriangleBounds() { Bounds = bounds, TriangleIndex = tri };
//				//}

//			}


//			//!!!!
//			MaxClusterSize = totalBounds.GetSize().MaxComponent() / 8;// 10;


//			TriangleNormals = new Vector3F[ triangleCount ];
//			for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
//			{
//				var index0 = Indices[ nTriangle * 3 + 0 ];
//				var index1 = Indices[ nTriangle * 3 + 1 ];
//				var index2 = Indices[ nTriangle * 3 + 2 ];

//				ref var v0 = ref Vertices[ index0 ].Position;
//				ref var v1 = ref Vertices[ index1 ].Position;
//				ref var v2 = ref Vertices[ index2 ].Position;

//				MathAlgorithms.CalculateTriangleNormal( ref v0, ref v1, ref v2, out TriangleNormals[ nTriangle ] );
//			}

//			VerticesByTriangle = new Dictionary<int, List<int>>( 512 );
//			for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
//			{
//				var index0 = Indices[ nTriangle * 3 + 0 ];
//				var index1 = Indices[ nTriangle * 3 + 1 ];
//				var index2 = Indices[ nTriangle * 3 + 2 ];

//				if( !VerticesByTriangle.TryGetValue( nTriangle, out var list ) )
//				{
//					list = new List<int>();
//					VerticesByTriangle[ nTriangle ] = list;
//				}

//				if( !list.Contains( index0 ) )
//					list.Add( index0 );
//				if( !list.Contains( index1 ) )
//					list.Add( index1 );
//				if( !list.Contains( index2 ) )
//					list.Add( index2 );
//			}

//			TrianglesByVertex = new Dictionary<int, List<int>>( 512 );
//			for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
//			{
//				var index0 = Indices[ nTriangle * 3 + 0 ];
//				var index1 = Indices[ nTriangle * 3 + 1 ];
//				var index2 = Indices[ nTriangle * 3 + 2 ];

//				if( !TrianglesByVertex.TryGetValue( index0, out var list ) )
//				{
//					list = new List<int>();
//					TrianglesByVertex[ index0 ] = list;
//				}
//				if( !list.Contains( nTriangle ) )
//					list.Add( nTriangle );

//				if( !TrianglesByVertex.TryGetValue( index1, out list ) )
//				{
//					list = new List<int>();
//					TrianglesByVertex[ index1 ] = list;
//				}
//				if( !list.Contains( nTriangle ) )
//					list.Add( nTriangle );

//				if( !TrianglesByVertex.TryGetValue( index2, out list ) )
//				{
//					list = new List<int>();
//					TrianglesByVertex[ index2 ] = list;
//				}
//				if( !list.Contains( nTriangle ) )
//					list.Add( nTriangle );
//			}

//			//VerticesByPosition = new Dictionary<Vector3F, List<int>>( vertices.Length );
//			//for( int nVertex = 0; nVertex < vertices.Length; nVertex++ )
//			//{
//			//	ref var v = ref vertices[ nVertex ];

//			//	if( !group.VerticesByPosition.TryGetValue( v.Position, out var list ) )
//			//	{
//			//		list = new List<int>();
//			//		group.VerticesByPosition[ v.Position ] = list;
//			//	}

//			//	list.Add( nVertex );
//			//}

//			AdjoiningTriangles = new AdjoiningTrianglesItem[ triangleCount ];
//			for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
//			{
//				var resultList = new ESet<int>( 12 );

//				VerticesByTriangle.TryGetValue( nTriangle, out var list );
//				foreach( var vertexIndex in list )
//				{
//					if( TrianglesByVertex.TryGetValue( vertexIndex, out var list2 ) )
//					{
//						foreach( var tri in list2 )
//						{
//							if( tri != nTriangle )
//								resultList.AddWithCheckAlreadyContained( tri );
//						}
//					}

//					////also add triangle which have vertices with same position but not same
//					//if( group.VerticesByPosition.TryGetValue( vertices[ vertexIndex ].Position, out var list3 ) )
//					//{
//					//	foreach( var vertexIndex2 in list3 )
//					//	{
//					//		if( group.TrianglesByVertex.TryGetValue( vertexIndex2, out var list4 ) )
//					//		{
//					//			foreach( var tri in list4 )
//					//			{
//					//				if( tri != nTriangle )
//					//					resultList.AddWithCheckAlreadyContained( tri );
//					//			}
//					//		}
//					//	}
//					//}
//				}

//				var item = new AdjoiningTrianglesItem() { Triangles = resultList.ToArray() };
//				AdjoiningTriangles[ nTriangle ] = item;
//			}

//			////select format
//			//var fullFormat = false;
//			//if( vertexStructure.GetElementBySemantic( VertexElementSemantic.Color0, out _ ) )
//			//	fullFormat = true;
//			//else if( vertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate1, out _ ) )
//			//	fullFormat = true;
//			//else if( vertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate2, out _ ) )
//			//	fullFormat = true;

//			//var triangleCount = indices.Length / 3;


//			//calculate


//			//var trianglesBounds = new TriangleBounds[ triangleCount ];
//			//for( int tri = 0; tri < triangleCount; tri++ )
//			//{
//			//	var index0 = indices[ tri * 3 + 0 ];
//			//	var index1 = indices[ tri * 3 + 1 ];
//			//	var index2 = indices[ tri * 3 + 2 ];

//			//	var bounds = new BoundsF( vertices[ index0 ].Position );
//			//	bounds.Add( ref vertices[ index1 ].Position );
//			//	bounds.Add( ref vertices[ index2 ].Position );

//			//	trianglesBounds[ tri ] = new TriangleBounds() { Bounds = bounds, TriangleIndex = tri };
//			//}


//			//var newTriangles = new VirtualizedTriangle[ triangleCount ];
//			//for( int tri = 0; tri < newTriangles.Length; tri++ )
//			//{
//			//	//!!!!right?
//			//	var tri2 = triangleIndexes[ tri ];

//			//	var triangle = new VirtualizedTriangle();
//			//	triangle.Index0 = indices[ tri2 * 3 + 0 ];
//			//	triangle.Index1 = indices[ tri2 * 3 + 1 ];
//			//	triangle.Index2 = indices[ tri2 * 3 + 2 ];
//			//	//!!!!right?
//			//	if( vertexMaterialIndexes != null )
//			//		triangle.MaterialIndex = vertexMaterialIndexes[ (int)triangle.Index0 ];

//			//	newTriangles[ tri ] = triangle;
//			//}


//			//process
//			var resultClusters = new List<Cluster>( 512 );

//			//find clusters
//			{
//				//var random = new FastRandom( 0 );

//				var trianglesToConsider = new ESet<int>( triangleCount );
//				for( int n = 0; n < triangleCount; n++ )
//					trianglesToConsider.Add( n );

//				//var processedTrianglesWithoutCluster = new List<int>( 1024 );


//				while( trianglesToConsider.Count != 0 )
//				{
//					//!!!!
//					int randomSelectionCount = 30;// 50;// 20;//10

//					if( trianglesToConsider.Count < randomSelectionCount )
//						randomSelectionCount = trianglesToConsider.Count;


//					//!!!!
//					//randomSelectionCount = 1;


//					var toCheck = new List<int>( randomSelectionCount );
//					{
//						var array = trianglesToConsider.ToArray();

//						for( int n = 0; n < randomSelectionCount; n++ )
//						{
//							var index = ( array.Length * n ) / randomSelectionCount;
//							if( index >= array.Length )
//								index = array.Length - 1;
//							var tri = array[ index ];

//							toCheck.Add( tri );
//						}
//					}

//					//sort because can be different order because threading
//					var clustersToCompare = new List<(Cluster, int)>( toCheck.Count * 3 );
//					Parallel.ForEach( toCheck, delegate ( int startTriangle )
//					{
//						//int edge = 0;
//						//for( int edge = 0; edge < 3; edge++ )
//						//{
//						var cluster = CalculateCluster( trianglesToConsider, startTriangle );//, edge );
//						if( cluster != null )
//						{
//							lock( clustersToCompare )
//								clustersToCompare.Add( (cluster, startTriangle) );
//						}
//						//}
//					} );
//					CollectionUtility.MergeSort( clustersToCompare, delegate ( (Cluster, int) v1, (Cluster, int) v2 )
//					{
//						return v2.Item2 - v1.Item2;
//					} );

//					//get best cluster by score
//					Cluster bestCluster = null;
//					var bestScore = 0.0;
//					foreach( var cluster in clustersToCompare )
//					{
//						var score = cluster.Item1.GetScore();
//						if( bestCluster == null || score > bestScore )
//						{
//							bestCluster = cluster.Item1;
//							bestScore = score;
//						}
//					}

//					//add selected cluster
//					//if( !bestCluster.TrianglesMode )
//					resultClusters.Add( bestCluster );
//					//else
//					//	processedTrianglesWithoutCluster.AddRange( bestCluster.Triangles );

//					//remove triangles from processing
//					foreach( var tri in bestCluster.Triangles )
//						trianglesToConsider.Remove( tri );
//				}

//				////processedTrianglesWithoutCluster
//				//if( processedTrianglesWithoutCluster.Count != 0 )
//				//{
//				//	var cluster = new Cluster();
//				//	cluster.Group = group;
//				//	cluster.TrianglesMode = true;
//				//	foreach( var tri in processedTrianglesWithoutCluster )
//				//		cluster.Triangles.Add( tri );

//				//	resultClusters.Add( cluster );
//				//}
//			}

//			//check created clusters
//			{
//				//check for shared triangles
//				var triangles = new bool[ Indices.Length / 3 ];
//				foreach( var cluster in resultClusters )
//				{
//					foreach( var tri in cluster.Triangles )
//					{
//						if( triangles[ tri ] )
//						{
//							error = "Internal error. Triangles overlapping.";
//							return false;
//						}
//						triangles[ tri ] = true;
//					}
//				}
//			}

//			//calculate cluster triangles
//			foreach( var cluster in resultClusters )
//			{
//				var perimeterVertices = cluster.Perimeter.ToArray();

//				var indexCountWhenMerged = perimeterVertices.Length * 3;
//				if( indexCountWhenMerged / 3 < cluster.Triangles.Count )
//				{

//					//!!!!обновлять Position


//					var center = cluster.Position;


//					var closestVertex = 0;
//					var closestDistanceSquared = float.MaxValue;
//					foreach( var index in cluster.Perimeter )
//					{
//						var p = Vertices[ index ].Position;

//						var distanceSquared = ( p - center ).LengthSquared();
//						if( distanceSquared < closestDistanceSquared )
//						{
//							closestVertex = index;
//							closestDistanceSquared = distanceSquared;
//						}
//					}

//					var centerVertex = Vertices[ closestVertex ];
//					if( cluster.Perimeter.Contains( closestVertex ) )
//						centerVertex.Position = center;

//					//centerVertex = Vertices[ perimeterVertices[ 0 ] ];


//					cluster.ClusterVertices = new StandardVertex[ perimeterVertices.Length + 1 ];
//					for( int n = 0; n < perimeterVertices.Length; n++ )
//						cluster.ClusterVertices[ n ] = Vertices[ perimeterVertices[ n ] ];
//					cluster.ClusterVertices[ cluster.ClusterVertices.Length - 1 ] = centerVertex;

//					cluster.ClusterIndices = new int[ perimeterVertices.Length * 3 ];
//					for( int tri = 0; tri < cluster.ClusterIndices.Length / 3; tri++ )
//					{
//						cluster.ClusterIndices[ tri * 3 + 0 ] = tri;
//						cluster.ClusterIndices[ tri * 3 + 1 ] = ( tri + 1 ) % perimeterVertices.Length;
//						cluster.ClusterIndices[ tri * 3 + 2 ] = cluster.ClusterVertices.Length - 1;
//					}
//				}
//				else
//				{
//					var triangles = cluster.Triangles.ToArray();

//					cluster.ClusterVertices = new StandardVertex[ triangles.Length * 3 ];
//					for( int n = 0; n < triangles.Length; n++ )
//					{
//						var tri = triangles[ n ];
//						cluster.ClusterVertices[ n * 3 + 0 ] = Vertices[ Indices[ tri * 3 + 0 ] ];
//						cluster.ClusterVertices[ n * 3 + 1 ] = Vertices[ Indices[ tri * 3 + 1 ] ];
//						cluster.ClusterVertices[ n * 3 + 2 ] = Vertices[ Indices[ tri * 3 + 2 ] ];
//					}

//					cluster.ClusterIndices = new int[ triangles.Length * 3 ];
//					for( int n = 0; n < cluster.ClusterIndices.Length; n++ )
//						cluster.ClusterIndices[ n ] = n;
//				}
//			}

//			//make result arrays
//			{
//				var totalVertexCount = 0;
//				var totalIndexCount = 0;
//				foreach( var cluster in resultClusters )
//				{
//					if( cluster.ClusterVertices != null )
//					{
//						totalVertexCount += cluster.ClusterVertices.Length;
//						totalIndexCount += cluster.ClusterIndices.Length;
//					}
//				}

//				var newVertices2 = new StandardVertex[ totalVertexCount ];
//				newIndices = new int[ totalIndexCount ];

//				var currentVertex = 0;
//				var currentIndex = 0;

//				foreach( var cluster in resultClusters )
//				{
//					if( cluster.ClusterVertices != null )
//					{
//						var startVertexOffset = currentVertex;

//						for( int n = 0; n < cluster.ClusterVertices.Length; n++ )
//							newVertices2[ currentVertex++ ] = cluster.ClusterVertices[ n ];

//						foreach( var index in cluster.ClusterIndices )
//							newIndices[ currentIndex++ ] = startVertexOffset + index;
//					}
//				}

//				//!!!!
//				MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( newVertices2, newIndices, float.Epsilon, MaxClusterSize, 0.2f, true, true, out newVertices2, out newIndices, out _ );

//				//MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( newVertices2, newIndices, float.Epsilon, float.Epsilon, float.Epsilon, true, true, out newVertices2, out newIndices, out _ );

//				MeshGeometry.MakeVertices( newVertices2, VertexComponents, out newVertexStructure, out newVertices );
//			}



//			//	////ActualClusterVertices, ActualClusterIndices
//			//	//{
//			//	//	var actualVertices = new List<StandardVertex>( cluster.Triangles.Count * 3 );
//			//	//	var actualIndices = new List<int>( cluster.Triangles.Count * 3 );

//			//	//	foreach( var nTriangle in cluster.Triangles )
//			//	//	{
//			//	//		var index0 = group.Indices[ nTriangle * 3 + 0 ];
//			//	//		var index1 = group.Indices[ nTriangle * 3 + 1 ];
//			//	//		var index2 = group.Indices[ nTriangle * 3 + 2 ];

//			//	//		ref var v0 = ref group.Vertices[ index0 ];
//			//	//		ref var v1 = ref group.Vertices[ index1 ];
//			//	//		ref var v2 = ref group.Vertices[ index2 ];

//			//	//		actualIndices.Add( actualVertices.Count );
//			//	//		actualVertices.Add( v0 );
//			//	//		actualIndices.Add( actualVertices.Count );
//			//	//		actualVertices.Add( v1 );
//			//	//		actualIndices.Add( actualVertices.Count );
//			//	//		actualVertices.Add( v2 );
//			//	//	}

//			//	//	//merge vertices
//			//	//	MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( actualVertices.ToArray(), actualIndices.ToArray(), 0, 0, 0, true, out cluster.ActualClusterVertices, out cluster.ActualClusterIndices, out _ );
//			//	//	//MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( actualVertices.ToArray(), actualIndices.ToArray(), 0, float.Epsilon, float.Epsilon, true, out cluster.ActualClusterVertices, out cluster.ActualClusterIndices, out _ );
//			//	//}


//			//	//simplify geometry
//			//	if( !cluster.TrianglesMode )
//			//	{
//			//		//clustered


//			//		//merge actual vertices
//			//		//MathAlgorithms.MergeEqualVertices( ref cluster.ActualClusterVertices, ref cluster.ActualClusterIndices, 0, 0, false );
//			//		////MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( cluster.ActualClusterVertices, cluster.ActualClusterIndices, 0, 0, 0, true, out cluster.ActualClusterVertices, out cluster.ActualClusterIndices, out _ );



//			//		//var size = new Vector3( cluster.GridSize.ToVector2() * cluster.CellSize, cluster.Height );

//			//		//SimpleMeshGenerator.GeneratePlane( size.ToVector2(), Vector2.Zero, Vector2.Zero, Vector2.Zero, out Vector3F[] p, out _, out _, out _, out var i, out _ );

//			//		//var bounds = new Bounds( Vector3.Zero, size );
//			//		//SimpleMeshGenerator.GenerateBox( bounds, out var vertices2, out var indices2 );

//			//		//for( int n = 0; n < vertices2.Length; n++ )
//			//		//	vertices2[ n ] = cluster.Rotation.ToQuaternion() * vertices2[ n ] + cluster.Position;


//			//		//!!!!temp

//			//		var size = new Vector3( cluster.GridSize.ToVector2() * cluster.CellSize, cluster.Height );

//			//		var bounds = new Bounds( Vector3.Zero, size );
//			//		SimpleMeshGenerator.GenerateBox( bounds, out var vertices2, out var indices2 );

//			//		for( int n = 0; n < vertices2.Length; n++ )
//			//			vertices2[ n ] = cluster.Rotation.ToQuaternion() * vertices2[ n ] + cluster.Position;

//			//		var newIndices = new List<int>();
//			//		for( int tri = 0; tri < indices2.Length / 3; tri++ )
//			//		{
//			//			var index0 = indices2[ tri * 3 + 0 ];
//			//			var index1 = indices2[ tri * 3 + 1 ];
//			//			var index2 = indices2[ tri * 3 + 2 ];

//			//			var v0 = vertices2[ index0 ];
//			//			var v1 = vertices2[ index1 ];
//			//			var v2 = vertices2[ index2 ];

//			//			var n = MathAlgorithms.CalculateTriangleNormal( v0, v1, v2 );

//			//			if( n.Z > -0.5f )
//			//			{
//			//				newIndices.Add( index0 );
//			//				newIndices.Add( index1 );
//			//				newIndices.Add( index2 );
//			//			}
//			//		}

//			//		indices2 = newIndices.ToArray();



//			//		//!!!!actual:

//			//		//var size = new Vector3( cluster.GridSize.ToVector2() * cluster.CellSize, cluster.Height );

//			//		//var bounds = new Bounds( Vector3.Zero, size );
//			//		//SimpleMeshGenerator.GenerateBox( bounds, out var vertices2, out var indices2 );

//			//		//for( int n = 0; n < vertices2.Length; n++ )
//			//		//	vertices2[ n ] = cluster.Rotation.ToQuaternion() * vertices2[ n ] + cluster.Position;




//			//		//!!!!куллить в вершинном шейдере весь кластер
//			//		//!!!!задний фейс не нужен? если куллить в вершинном шейдере то в этом нет смысла

//			//		//!!!!обрезать 4 угла. иначе считать depthOffset
//			//		//!!!!упрощать до 4 треугольников когда можно. иначе считать depthOffset


//			//		//merge vertices
//			//		{
//			//			MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( CollectionUtility.ToVector3F( vertices2 ), indices2, 0, float.Epsilon, true, out var clusterVertices2, out var clusterIndices2, out _ );
//			//			//MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( CollectionUtility.ToVector3F( vertices2 ), indices2, 0, float.Epsilon, float.Epsilon, true, out var clusterVertices2, out var clusterIndices2, out var processedTrianglesToSourceIndex );

//			//			cluster.ClusterVertices = clusterVertices2.Select( v => new StandardVertex( v ) ).ToArray();
//			//			cluster.ClusterIndices = clusterIndices2;
//			//		}

//			//		////remove back triangles
//			//		//var indices3 = new List<int>( indices2.Length );
//			//		//for( int nTriangle = 0; nTriangle < indices2.Length / 3; nTriangle++ )
//			//		//{
//			//		//	var index0 = indices2[ nTriangle * 3 + 0 ];
//			//		//	var index1 = indices2[ nTriangle * 3 + 1 ];
//			//		//	var index2 = indices2[ nTriangle * 3 + 2 ];

//			//		//	ref var v0 = ref vertices2[ index0 ];
//			//		//	ref var v1 = ref vertices2[ index1 ];
//			//		//	ref var v2 = ref vertices2[ index2 ];

//			//		//	MathAlgorithms.CalculateTriangleNormal( ref v0, ref v1, ref v2, out var triNormal );

//			//		//	var visible = false;

//			//		//	foreach( var tri in cluster.Triangles )
//			//		//	{
//			//		//		ref var normal = ref triangleNormals[ tri ];

//			//		//		var degree = MathAlgorithms.GetVectorsAngle( ref triNormal, ref normal ).InDegrees();
//			//		//		if( degree <= 90 )
//			//		//		{
//			//		//			visible = true;
//			//		//			break;
//			//		//		}
//			//		//	}

//			//		//	if( visible )
//			//		//	{
//			//		//		indices3.Add( index0 );
//			//		//		indices3.Add( index1 );
//			//		//		indices3.Add( index2 );
//			//		//	}
//			//		//}


//			//		//var epsilon = 0.000001f;
//			//		//MathAlgorithms.ConvexHullFromMesh( CollectionUtility.ToVector3( clusterVertices.ToArray() ), out var vertices2, out var indices2, epsilon );
//			//		//var vertices2 = CollectionUtility.ToVector3F( vertices2D );
//			//		//MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( vertices3.ToArray(), indices2.ToArray(), epsilon, epsilon, true, out clusterVerticesOutput, out clusterIndicesOutput, out _ );
//			//	}
//			//	else
//			//	{
//			//		//separate

//			//		//ActualClusterVertices, ActualClusterIndices
//			//		{
//			//			var actualVertices = new List<StandardVertex>( cluster.Triangles.Count * 3 );
//			//			var actualIndices = new List<int>( cluster.Triangles.Count * 3 );

//			//			foreach( var nTriangle in cluster.Triangles )
//			//			{
//			//				var index0 = group.Indices[ nTriangle * 3 + 0 ];
//			//				var index1 = group.Indices[ nTriangle * 3 + 1 ];
//			//				var index2 = group.Indices[ nTriangle * 3 + 2 ];

//			//				ref var v0 = ref group.Vertices[ index0 ];
//			//				ref var v1 = ref group.Vertices[ index1 ];
//			//				ref var v2 = ref group.Vertices[ index2 ];

//			//				actualIndices.Add( actualVertices.Count );
//			//				actualVertices.Add( v0 );
//			//				actualIndices.Add( actualVertices.Count );
//			//				actualVertices.Add( v1 );
//			//				actualIndices.Add( actualVertices.Count );
//			//				actualVertices.Add( v2 );
//			//			}

//			//			//merge vertices
//			//			MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( actualVertices.ToArray(), actualIndices.ToArray(), 0, 0, 0, true, false, out cluster.ActualClusterVertices, out cluster.ActualClusterIndices, out _ );
//			//		}

//			//		cluster.ClusterVertices = cluster.ActualClusterVertices;
//			//		cluster.ClusterIndices = cluster.ActualClusterIndices;
//			//	}


//			//}


//			//var time2 = DateTime.Now;
//			//Log.Info( ( time2 - time ).TotalSeconds.ToString() );

//			return true;
//		}
//	}
//}

//#endif