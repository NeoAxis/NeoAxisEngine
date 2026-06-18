//// Copyright 2006–2026 Ivan Efimov. All rights reserved.
//// https://github.com/fuqunaga/ComputeShaderBVHMeshHit MIT
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Runtime.InteropServices;

//namespace NeoAxis
//{
//	class MeshGeometry_CalculateVirtualized
//	{
//		public MeshGeometry Geometry;

//		public double ProxyMeshFactor;
//		public bool ProxyMeshCompress;
//		public bool ProxyMeshOptimize;

//		//BoundsF[] trianglesBounds;

//		/////////////////////////////////////////////////

//		//[StructLayout( LayoutKind.Sequential, Pack = 1 )]
//		//struct Vertex
//		//{
//		//	public Vector3F Position;
//		//	//!!!!
//		//	//cluster index
//		//	//public float Color2;
//		//}

//		/////////////////////////////////////////////////

//		//class MultiMaterialGroup
//		//{
//		//	public StandardVertex[] Vertices;
//		//	public int[] Indices;

//		//	public int MaterialIndex;

//		//	public Vector3F[] TriangleNormals;
//		//	//!!!!
//		//	public Dictionary<Vector3F, List<int>> VerticesByPosition;
//		//	public Dictionary<int, List<int>> VerticesByTriangle;
//		//	public Dictionary<int, List<int>> TrianglesByVertex;

//		//	public struct AdjoiningTrianglesItem
//		//	{
//		//		public int[] Triangles;
//		//	}
//		//	public AdjoiningTrianglesItem[] AdjoiningTriangles;
//		//}

//		/////////////////////////////////////////////////

//		//class Cluster
//		//{
//		//	//!!!!
//		//	public const int ClusteredMaxTriangleCount = 4096;// 256;

//		//	public MultiMaterialGroup Group;
//		//	public Vector3F Normal;

//		//	public ESet<int> Triangles = new ESet<int>( 32 );

//		//	public bool TrianglesMode;

//		//	public Vector3F Position;
//		//	public QuaternionF Rotation;
//		//	public float CellSize;
//		//	public Vector2I GridSize;
//		//	public float Height;

//		//	public BoundsF CenteredLocalBounds = BoundsF.Cleared;
//		//	public Matrix4F CenteredClusterSpaceToObjectSpace;
//		//	public Matrix4F CenteredObjectSpaceToClusterSpace;

//		//	public Cell[,] Grid;

//		//	//result data
//		//	public MeshGeometry.ClusterDataHeaderClusterInfo ClusterInfo;
//		//	public StandardVertex[] ActualClusterVertices;
//		//	public int[] ActualClusterIndices;
//		//	public StandardVertex[] ClusterVertices;
//		//	public int[] ClusterIndices;
//		//	public byte[] ClusterBody;

//		//	//

//		//	public enum CanAddTriangleResult
//		//	{
//		//		No,
//		//		MaybeLater,
//		//		Yes,
//		//	}

//		//	public struct Cell
//		//	{
//		//		public List<int> ActualTriangles;
//		//		//public List<(int, float)> ActualTriangles;
//		//	}

//		//	//public struct Cell
//		//	//{
//		//	//	public float Height;
//		//	//	public int TriangleSetIndex;
//		//	//	public Vector2I NearestCellIndex;
//		//	//	public float NearestCellHeight;
//		//	//}

//		//	//public struct Cell
//		//	//{
//		//	//	public float Height;
//		//	//	public int TriangleSetIndex;
//		//	//	public Vector2I NearestCellIndex;
//		//	//	public float NearestCellHeight;
//		//	//}

//		//	//

//		//	public CanAddTriangleResult CanAddTriangle( int nTriangle )
//		//	{
//		//		ref var triNormal = ref Group.TriangleNormals[ nTriangle ];

//		//		var degree = MathAlgorithms.GetVectorsAngle( ref triNormal, ref Normal ).InDegrees();

//		//		//!!!!
//		//		if( degree > 70 )
//		//			return CanAddTriangleResult.No;

//		//		if( Triangles.Count >= ClusteredMaxTriangleCount )
//		//			return CanAddTriangleResult.No;


//		//		GetCenteredLocalBoundsWithNewTriangle( nTriangle, out var newBounds );
//		//		var size = newBounds.GetSize();
//		//		if( size.MinComponent() > 0 )
//		//		{
//		//			var h = size.ToVector2().Length();
//		//			var v = size.Z;

//		//			//!!!!maybe 5 - 10
//		//			if( v > h / 8 )
//		//				return CanAddTriangleResult.MaybeLater;
//		//		}


//		//		//!!!!больше 8 вершин связано


//		//		//!!!!перекрывание. наверное всё же лучше тоже убрать, т.к. у нас ведь цельная поверхность


//		//		return CanAddTriangleResult.Yes;
//		//	}

//		//	void GetCenteredLocalBoundsWithNewTriangle( int nTriangle, out BoundsF result )
//		//	{
//		//		result = CenteredLocalBounds;

//		//		var index0 = Group.Indices[ nTriangle * 3 + 0 ];
//		//		var index1 = Group.Indices[ nTriangle * 3 + 1 ];
//		//		var index2 = Group.Indices[ nTriangle * 3 + 2 ];

//		//		ref var v0 = ref Group.Vertices[ index0 ].Position;
//		//		ref var v1 = ref Group.Vertices[ index1 ].Position;
//		//		ref var v2 = ref Group.Vertices[ index2 ].Position;

//		//		//!!!!maybe precalculate
//		//		Matrix4F.Multiply( ref CenteredObjectSpaceToClusterSpace, ref v0, out var tv0 );
//		//		Matrix4F.Multiply( ref CenteredObjectSpaceToClusterSpace, ref v1, out var tv1 );
//		//		Matrix4F.Multiply( ref CenteredObjectSpaceToClusterSpace, ref v2, out var tv2 );
//		//		//var tv0 = CenteredObjectSpaceToClusterSpace * v0;
//		//		//var tv1 = CenteredObjectSpaceToClusterSpace * v1;
//		//		//var tv2 = CenteredObjectSpaceToClusterSpace * v2;

//		//		result.Add( ref tv0 );
//		//		result.Add( ref tv1 );
//		//		result.Add( ref tv2 );
//		//	}

//		//	public void AddTriangle( int nTriangle )
//		//	{
//		//		Triangles.Add( nTriangle );

//		//		GetCenteredLocalBoundsWithNewTriangle( nTriangle, out CenteredLocalBounds );
//		//	}

//		//	public bool CalculateActualTrianglesAndRasterizeTriangles()
//		//	{
//		//		//!!!!precalculate
//		//		var clusterSpaceToObjectSpace = new Matrix4F( Rotation.ToMatrix3(), Position );
//		//		var objectSpaceToClusterSpace = clusterSpaceToObjectSpace.GetInverse();


//		//		//!!!!slowly?
//		//		//calculate actual triangles. ActualClusterVertices, ActualClusterIndices
//		//		{
//		//			var actualVertices = new List<StandardVertex>( Triangles.Count * 3 );
//		//			var actualIndices = new List<int>( Triangles.Count * 3 );

//		//			foreach( var nTriangle in Triangles )
//		//			{
//		//				var index0 = Group.Indices[ nTriangle * 3 + 0 ];
//		//				var index1 = Group.Indices[ nTriangle * 3 + 1 ];
//		//				var index2 = Group.Indices[ nTriangle * 3 + 2 ];

//		//				ref var v0 = ref Group.Vertices[ index0 ];
//		//				ref var v1 = ref Group.Vertices[ index1 ];
//		//				ref var v2 = ref Group.Vertices[ index2 ];

//		//				actualIndices.Add( actualVertices.Count );
//		//				actualVertices.Add( v0 );
//		//				actualIndices.Add( actualVertices.Count );
//		//				actualVertices.Add( v1 );
//		//				actualIndices.Add( actualVertices.Count );
//		//				actualVertices.Add( v2 );
//		//			}

//		//			ActualClusterVertices = actualVertices.ToArray();
//		//			ActualClusterIndices = actualIndices.ToArray();

//		//			//!!!!slowly? but less amount of matrix multiply
//		//			MathAlgorithms.MergeEqualVertices( ref ActualClusterVertices, ref ActualClusterIndices, 0, 0, false );

//		//			//MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( actualVertices.ToArray(), actualIndices.ToArray(), 0, 0, 0, true, out ActualClusterVertices, out ActualClusterIndices, out _ );
//		//		}

//		//		Grid = new Cell[ GridSize.X, GridSize.Y ];

//		//		var verticesProjected = new Vector3F[ ActualClusterVertices.Length ];
//		//		for( int n = 0; n < verticesProjected.Length; n++ )
//		//		{
//		//			var p = ActualClusterVertices[ n ].Position;
//		//			Matrix4F.Multiply( ref objectSpaceToClusterSpace, ref p, out verticesProjected[ n ] );
//		//			//verticesProjected[ n ] = p;
//		//		}

//		//		for( int nTriangle = 0; nTriangle < ActualClusterIndices.Length / 3; nTriangle++ )
//		//		{
//		//			var index0 = ActualClusterIndices[ nTriangle * 3 + 0 ];
//		//			var index1 = ActualClusterIndices[ nTriangle * 3 + 1 ];
//		//			var index2 = ActualClusterIndices[ nTriangle * 3 + 2 ];

//		//			ref var v0Projected = ref verticesProjected[ index0 ];
//		//			ref var v1Projected = ref verticesProjected[ index1 ];
//		//			ref var v2Projected = ref verticesProjected[ index2 ];

//		//			//ref var v0 = ref ActualClusterVertices[ index0 ].Position;
//		//			//ref var v1 = ref ActualClusterVertices[ index1 ].Position;
//		//			//ref var v2 = ref ActualClusterVertices[ index2 ].Position;

//		//			//var v0Projected = objectSpaceToClusterSpace * v0;
//		//			//var v1Projected = objectSpaceToClusterSpace * v1;
//		//			//var v2Projected = objectSpaceToClusterSpace * v2;


//		//			//!!!!slowly?

//		//			//var cells = new List<Vector2I>( 256 );

//		//			//!!!!

//		//			var triangle = new Triangle2F( v0Projected.ToVector2(), v1Projected.ToVector2(), v2Projected.ToVector2() );


//		//			//!!!!интервал

//		//			for( int y = 0; y < GridSize.Y; y++ )
//		//			{
//		//				for( int x = 0; x < GridSize.X; x++ )
//		//				{
//		//					//var clusterSize = new Vector3( GridSize.ToVector2() * CellSize, Height );

//		//					var b = new RectangleF( CellSize * x, CellSize * y, CellSize * x + CellSize, CellSize * y + CellSize );


//		//					//!!!!temp


//		//					var trb = new RectangleF( triangle.A );
//		//					trb.Add( triangle.B );
//		//					trb.Add( triangle.C );
//		//					if( b.Intersects( trb ) )
//		//					//if( b.Intersects( ref triangle ) )
//		//					{
//		//						ref var cell = ref Grid[ x, y ];
//		//						//ref var cell = ref Grid[ y * GridSize.X + x ];

//		//						if( cell.ActualTriangles == null )
//		//							cell.ActualTriangles = new List<int>( 8 );// new List<(int, float)>( 8 );

//		//						if( cell.ActualTriangles.Count == 8 )
//		//							return false;

//		//						cell.ActualTriangles.Add( nTriangle );

//		//						//float maxHeightInCell = 0.0f;
//		//						//{
//		//						//	var clipBounds = b;

//		//						//	var polygon = new Vector3[ 3 ];
//		//						//	polygon[ 0 ] = v0Projected;
//		//						//	polygon[ 1 ] = v1Projected;
//		//						//	polygon[ 2 ] = v2Projected;

//		//						//	//+X
//		//						//	{
//		//						//		var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Maximum, 0 ), new Vector3( -1, 0, 0 ) );
//		//						//		polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
//		//						//	}

//		//						//	//+Y
//		//						//	{
//		//						//		var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Maximum, 0 ), new Vector3( 0, -1, 0 ) );
//		//						//		polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
//		//						//	}

//		//						//	//-X
//		//						//	{
//		//						//		var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Minimum, 0 ), new Vector3( 1, 0, 0 ) );
//		//						//		polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
//		//						//	}

//		//						//	//-Y
//		//						//	{
//		//						//		var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Minimum, 0 ), new Vector3( 0, 1, 0 ) );
//		//						//		polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
//		//						//	}

//		//						//	foreach( var p in polygon )
//		//						//		maxHeightInCell = Math.Max( maxHeightInCell, (float)p.Z );
//		//						//}

//		//						//cell.ActualTriangles.Add( (nTriangle, maxHeightInCell) );


//		//						//cells.Add( new Vector2I( x, y ) );
//		//						//cells.AddWithCheckAlreadyContained( new Vector2I( x, y ) );
//		//					}


//		//					//var b = new Bounds( CellSize * x, CellSize * y, 0, ( CellSize + 1 ) * x, ( CellSize + 1 ) * y, Height );

//		//					//if( b.Intersects( new Triangle( v0Projected, v1Projected, v2Projected ) ) )
//		//					//{
//		//					//	cells.AddWithCheckAlreadyContained( new Vector2I( x, y ) );
//		//					//}
//		//				}
//		//			}

//		//			//!!!!это неточно, т.к. position округляется
//		//			//MathAlgorithms.Fill2DTriangle(
//		//			//	( v0Projected.ToVector2() / CellSize ).ToVector2I(),
//		//			//	( v1Projected.ToVector2() / CellSize ).ToVector2I(),
//		//			//	( v2Projected.ToVector2() / CellSize ).ToVector2I(),
//		//			//	new RectangleI( 0, 0, GridSize.X + 1, GridSize.Y + 1 ), delegate ( Vector2I point )
//		//			//{
//		//			//	var cellIndex = point;
//		//			//	if( cellIndex.X < GridSize.X && cellIndex.Y < GridSize.Y )
//		//			//		cells.AddWithCheckAlreadyContained( cellIndex );
//		//			//} );

//		//			//MathAlgorithms.Fill2DTriangle(
//		//			//	( v0Projected.ToVector2() / CellSize * 10 ).ToVector2I(),
//		//			//	( v1Projected.ToVector2() / CellSize * 10 ).ToVector2I(),
//		//			//	( v2Projected.ToVector2() / CellSize * 10 ).ToVector2I(),
//		//			//	new RectangleI( 0, 0, ( GridSize.X + 1 ) * 10, ( GridSize.Y + 1 ) * 10 ), delegate ( Vector2I point )
//		//			//	{
//		//			//		var cellIndex = point / 10;
//		//			//		if( cellIndex.X < GridSize.X && cellIndex.Y < GridSize.Y )
//		//			//			cells.AddWithCheckAlreadyContained( cellIndex );
//		//			//	} );

//		//			//foreach( var cellIndex in cells )
//		//			//{
//		//			//	ref var cell = ref Grid[ cellIndex.Y * GridSize.X + cellIndex.X ];

//		//			//	if( cell.Triangles == null )
//		//			//		cell.Triangles = new List<int>( 8 );

//		//			//	if( cell.Triangles.Count == 8 )
//		//			//		return false;

//		//			//	cell.Triangles.Add( nTriangle );
//		//			//}
//		//		}

//		//		return true;
//		//	}

//		//	public double GetScore()
//		//	{
//		//		if( !TrianglesMode )
//		//		{
//		//			//!!!!

//		//			var cellsWithTriangles = 0;
//		//			var trianglesDensity = 0;

//		//			for( int y = 0; y < GridSize.Y; y++ )
//		//			{
//		//				for( int x = 0; x < GridSize.X; x++ )
//		//				{
//		//					ref var cell = ref Grid[ x, y ];

//		//					if( cell.ActualTriangles != null && cell.ActualTriangles.Count != 0 )
//		//					{
//		//						cellsWithTriangles++;
//		//						trianglesDensity += cell.ActualTriangles.Count;
//		//					}
//		//				}
//		//			}

//		//			var cellCount = (double)GridSize.X * GridSize.Y;

//		//			var emptySpaceScore = (double)cellsWithTriangles / cellCount;
//		//			var triangleCountScore = (double)Triangles.Count / (double)ClusteredMaxTriangleCount;
//		//			var trianglesDensityScore = (double)trianglesDensity / ( cellCount * 8 );

//		//			//!!!!распределенность в ячейках. т.е. что примерно одинаковое количество

//		//			//!!!!multipliers
//		//			return emptySpaceScore * 1 + triangleCountScore * 1 + trianglesDensityScore * 0;


//		//			//!!!!качество кластера определяется. параметр качества
//		//			//1. полезная площадь
//		//			//2. заполненность треугольников в списках
//		//			// может заполненность и равномерность это разное. например малозаполненно 1-2 треугольника, но равномерно
//		//			////ПУСтота получается тут же учитывается? не, геометрия ведь есть
//		//			//3. размер грида. чем больше, тем меньше кластеров

//		//			//return Triangles.Count;

//		//			////var size2 = GridSize.ToVector2() * CellSize;
//		//			////return size2.X * size2.Y;

//		//		}

//		//		return 0;
//		//	}
//		//}

//		/////////////////////////////////////////////////

//		////!!!!не добавлять сепаров пока добавляются кластерные?

//		////!!!!не добавлять если соединение даст больше 8 труегольников. очевидно что такой не подходит. а может и больше количество дропать 20-30

//		////!!!!какой максимальный размер сетки. с точки зрения distance field кластерам лучше сетку делать меньше.

//		////!!!!кластеру лучше иметь более ровное распределение вершин на нём? чтобы 2д грид меньше был

//		////!!!!кластер может не добавлять в себя сложные соединения трегольников

//		////!!!!чтобы при проецировании не могло былть накладывания друг на друга

//		//////!!!!ajoining проверять если вершина в том же месте
//		//////!!!!может не только по связанным вершинам, но и по тем которые в одной точке. хотя это может быть прерыванием сурфейса


//		//static int[] GetAdjoiningTriangles( MultiMaterialGroup group, int nTriangle )
//		//{
//		//	return group.AdjoiningTriangles[ nTriangle ].Triangles;
//		//}

//		//static Cluster CalculateCluster( MultiMaterialGroup group, ESet<int> trianglesToConsider, int startTriangle, int edge )
//		//{
//		//	//нормалью будет нормаль треугольника. и ищем суммарно с соседними, если нормаль удовлетворяет начальному треугольнику
//		//	Vector3F clusterNormal;
//		//	{
//		//		clusterNormal = group.TriangleNormals[ startTriangle ];

//		//		//!!!!

//		//		//!!!!trianglesToConsider

//		//	}

//		//	//!!!!не так если нормаль не от одного

//		//	Vector3F clusterCenter;
//		//	var clusterForward = Vector3F.Zero;
//		//	{
//		//		var index0 = group.Indices[ startTriangle * 3 + 0 ];
//		//		var index1 = group.Indices[ startTriangle * 3 + 1 ];
//		//		var index2 = group.Indices[ startTriangle * 3 + 2 ];

//		//		ref var v0 = ref group.Vertices[ index0 ].Position;
//		//		ref var v1 = ref group.Vertices[ index1 ].Position;
//		//		ref var v2 = ref group.Vertices[ index2 ].Position;

//		//		clusterCenter = ( v0 + v1 + v2 ) / 3.0f;

//		//		//use edge to select direction
//		//		switch( edge )
//		//		{
//		//		case 0: if( v0 != v1 ) clusterForward = ( v1 - v0 ).GetNormalize(); break;
//		//		case 1: if( v0 != v2 ) clusterForward = ( v2 - v0 ).GetNormalize(); break;
//		//		case 2: if( v1 != v2 ) clusterForward = ( v2 - v1 ).GetNormalize(); break;
//		//		}
//		//	}


//		//	////!!!!temp
//		//	//clusterForward = Vector3F.XAxis;
//		//	//clusterNormal = Vector3F.ZAxis;


//		//	var degenerate = clusterForward == Vector3F.Zero;


//		//	//разрастаемся

//		//	var cluster = new Cluster();
//		//	cluster.Group = group;
//		//	cluster.AddTriangle( startTriangle );

//		//	if( !degenerate )
//		//	{
//		//		cluster.Normal = clusterNormal;
//		//		cluster.Rotation = QuaternionF.LookAt( clusterForward, cluster.Normal );

//		//		cluster.CenteredClusterSpaceToObjectSpace = new Matrix4F( cluster.Rotation.ToMatrix3(), clusterCenter );
//		//		cluster.CenteredObjectSpaceToClusterSpace = cluster.CenteredClusterSpaceToObjectSpace.GetInverse();

//		//		var neverCheckAnymore = new ESet<int>( 128 );

//		//		var toCheck = new ESet<int>( 128 );
//		//		toCheck.AddRange( GetAdjoiningTriangles( group, startTriangle ).Where( t => trianglesToConsider.Contains( t ) ) );

//		//		while( true )
//		//		{
//		//			var updated = false;

//		//			var toCheckCopy = toCheck.ToArray();


//		//			//!!!!сортировать брать сначала которые ближе к плоскости?


//		//			foreach( var tri in toCheckCopy )
//		//			{
//		//				if( !cluster.Triangles.Contains( tri ) )
//		//				{
//		//					var canAdd = cluster.CanAddTriangle( tri );

//		//					if( canAdd == Cluster.CanAddTriangleResult.Yes )
//		//					{
//		//						cluster.AddTriangle( tri );

//		//						toCheck.Remove( tri );

//		//						//!!!!сначала лучших добавлять?

//		//						foreach( var t in GetAdjoiningTriangles( group, tri ) )
//		//						{
//		//							if( !cluster.Triangles.Contains( t ) && !neverCheckAnymore.Contains( t ) && trianglesToConsider.Contains( t ) )
//		//								toCheck.AddWithCheckAlreadyContained( t );
//		//						}

//		//						updated = true;

//		//						//fast exit. max amount of triangles reached
//		//						if( cluster.Triangles.Count >= Cluster.ClusteredMaxTriangleCount )
//		//							goto end;
//		//					}
//		//					else if( canAdd == Cluster.CanAddTriangleResult.No )
//		//					{
//		//						toCheck.Remove( tri );
//		//						neverCheckAnymore.AddWithCheckAlreadyContained( tri );

//		//						updated = true;
//		//					}
//		//				}
//		//			}

//		//			if( !updated )
//		//				break;
//		//		}
//		//		end:;

//		//		//!!!!какие еще критерии
//		//		//!!!!может > 20
//		//		//!!!!
//		//		if( cluster.Triangles.Count > 10 )// && false )
//		//		{

//		//			//!!!!temp

//		//			////cluster.ActualClusterIndices

//		//			//var localBoundsTemp = BoundsF.Cleared;
//		//			//{
//		//			//	foreach( var nTriangle in cluster.Triangles )
//		//			//	{
//		//			//		var index0 = group.Indices[ nTriangle * 3 + 0 ];
//		//			//		var index1 = group.Indices[ nTriangle * 3 + 1 ];
//		//			//		var index2 = group.Indices[ nTriangle * 3 + 2 ];

//		//			//		ref var v0 = ref group.Vertices[ index0 ].Position;
//		//			//		ref var v1 = ref group.Vertices[ index1 ].Position;
//		//			//		ref var v2 = ref group.Vertices[ index2 ].Position;

//		//			//		localBoundsTemp.Add( v0 );
//		//			//		localBoundsTemp.Add( v1 );
//		//			//		localBoundsTemp.Add( v2 );
//		//			//	}
//		//			//}
//		//			//cluster.Position = localBoundsTemp.Minimum;
//		//			//cluster.Height = localBoundsTemp.GetSize().Z;


//		//			//Log.Info( "--" );
//		//			//{
//		//			//	string q = "";
//		//			//	foreach( var a in cluster.Triangles )
//		//			//		q += " " + a.ToString();
//		//			//	Log.Info( q );
//		//			//}
//		//			//Log.Info( cluster.Position.ToString() );


//		//			var localClusterPosition = cluster.CenteredLocalBounds.Minimum;
//		//			//var offsetFromCenter = cluster.CenteredLocalBounds.Minimum - cluster.CenteredLocalBounds.GetCenter();
//		//			cluster.Position = clusterCenter + cluster.Rotation * localClusterPosition;
//		//			cluster.Height = cluster.CenteredLocalBounds.GetSize().Z;

//		//			//var offsetFromCenter = cluster.CenteredLocalBounds.Minimum - cluster.CenteredLocalBounds.GetCenter();
//		//			//cluster.Position = clusterCenter + cluster.Rotation * offsetFromCenter;
//		//			//cluster.Height = cluster.CenteredLocalBounds.GetSize().Z;

//		//			if( cluster.Height < 0.000001f )
//		//				cluster.Height = 0.000001f;

//		//			var done = false;


//		//			//!!!!Grid size range

//		//			//!!!!чаще шаг


//		//			//!!!!temp
//		//			//var boundsSize = localBoundsTemp.GetSize();
//		//			var boundsSize = cluster.CenteredLocalBounds.GetSize();

//		//			//!!!!
//		//			//for( var gridSize = 8; gridSize <= 64; gridSize *= 2 )
//		//			for( var gridSize = 2; gridSize <= 64; gridSize *= 2 )
//		//			{
//		//				//var size = new Vector3( cluster.GridSize.ToVector2() * cluster.CellSize, cluster.Height );

//		//				//var size2 = cluster.CenteredLocalBounds.GetSize().ToVector2();

//		//				//!!!! + 1

//		//				var size2 = boundsSize.ToVector2();
//		//				//var size2 = cluster.GridSize.ToVector2F() * cluster.CellSize;
//		//				if( size2.X <= 0 )
//		//					size2.X = 0.0001f;
//		//				if( size2.Y <= 0 )
//		//					size2.Y = 0.0001f;

//		//				if( size2.X > size2.Y )
//		//				{
//		//					cluster.CellSize = size2.X / gridSize;
//		//					cluster.GridSize = new Vector2I( gridSize, MathEx.Clamp( (int)( gridSize * size2.Y / size2.X + 1 ), 1, gridSize ) );
//		//				}
//		//				else
//		//				{
//		//					cluster.CellSize = size2.Y / gridSize;
//		//					cluster.GridSize = new Vector2I( MathEx.Clamp( (int)( gridSize * size2.X / size2.Y + 1 ), 1, gridSize ), gridSize );
//		//				}

//		//				if( cluster.CalculateActualTrianglesAndRasterizeTriangles() )
//		//				{
//		//					done = true;
//		//					break;
//		//				}
//		//			}

//		//			//need try smaller area because reach limit of grid size
//		//			if( !done )
//		//			{
//		//				//make a new cluster with removed 10% of far triangles


//		//				//!!!!часто сюда попадает?


//		//				//!!!!
//		//				//Log.Info( "!done" );


//		//				//!!!!расстояние по прямоугольнику, чтобы было прямоугольнее

//		//				float GetTriangleDistanceSquared( int nTriangle )
//		//				{
//		//					var index0 = group.Indices[ nTriangle * 3 + 0 ];
//		//					var index1 = group.Indices[ nTriangle * 3 + 1 ];
//		//					var index2 = group.Indices[ nTriangle * 3 + 2 ];

//		//					ref var v0 = ref group.Vertices[ index0 ].Position;
//		//					ref var v1 = ref group.Vertices[ index1 ].Position;
//		//					ref var v2 = ref group.Vertices[ index2 ].Position;

//		//					return Math.Max( ( v0 - clusterCenter ).LengthSquared(), Math.Max( ( v1 - clusterCenter ).LengthSquared(), ( v2 - clusterCenter ).LengthSquared() ) );
//		//				}

//		//				var triangles = cluster.Triangles.ToArray();
//		//				CollectionUtility.MergeSort( triangles, delegate ( int tri1, int tri2 )
//		//				{
//		//					var d1 = GetTriangleDistanceSquared( tri1 );
//		//					var d2 = GetTriangleDistanceSquared( tri2 );

//		//					if( d1 < d2 )
//		//						return -1;
//		//					if( d1 > d2 )
//		//						return 1;
//		//					return 0;
//		//				}, false );// true );

//		//				//!!!!0.75
//		//				var newTriangleCount = (int)( triangles.Length * 0.75f );
//		//				if( newTriangleCount < 1 )
//		//					newTriangleCount = 1;

//		//				var trianglesToConsider2 = new ESet<int>( newTriangleCount + 1 );
//		//				trianglesToConsider2.Add( startTriangle );
//		//				for( int n = 0; n < newTriangleCount; n++ )
//		//					trianglesToConsider2.AddWithCheckAlreadyContained( triangles[ n ] );

//		//				//var trianglesToConsider2 = new ESet<int>( trianglesToConsider );
//		//				//for( int tri = newTriangleCount + 1; tri < triangles.Length; tri++ )
//		//				//{
//		//				//	if( tri != startTriangle )
//		//				//		trianglesToConsider2.Remove( tri );
//		//				//}

//		//				//recursive
//		//				var cluster2 = CalculateCluster( group, trianglesToConsider2, startTriangle, edge );

//		//				return cluster2;
//		//			}



//		//			//var size2 = cluster.CenteredLocalBounds.GetSize().ToVector2();
//		//			//if( size2.X <= 0 )
//		//			//	size2.X = 0.0001f;
//		//			//if( size2.Y <= 0 )
//		//			//	size2.Y = 0.0001f;

//		//			//if( size2.X > size2.Y )
//		//			//{
//		//			//	cluster.CellSize = size2.X / 64;
//		//			//	cluster.GridSize = new Vector2I( 64, MathEx.Clamp( (int)( 64 * size2.Y / size2.X + 1 ), 1, 64 ) );
//		//			//}
//		//			//else
//		//			//{
//		//			//	cluster.CellSize = size2.Y / 64;
//		//			//	cluster.GridSize = new Vector2I( MathEx.Clamp( (int)( 64 * size2.X / size2.Y + 1 ), 1, 64 ), 64 );
//		//			//}


//		//			//var clusterSpaceToObjectSpace = new Matrix4F( cluster.Rotation.ToMatrix3(), cluster.Position );

//		//		}
//		//		else
//		//			cluster.TrianglesMode = true;
//		//	}
//		//	else
//		//		cluster.TrianglesMode = true;


//		//	return cluster;



//		//	//var nTriangleStart = trianglesToConsider.First();
//		//	//trianglesToConsider.Remove( nTriangleStart );

//		//	//var cluster = new Cluster();
//		//	//cluster.Group = group;
//		//	//clusters.Add( cluster );
//		//	//cluster.Triangles.Add( nTriangleStart );

//		//	//var toCheck = new Queue<int>();
//		//	//toCheck.Enqueue( nTriangleStart );

//		//	//while( toCheck.Count != 0 )
//		//	//{
//		//	//	var nTriangle = toCheck.Dequeue();

//		//	//	//get adjoining triangles
//		//	//	var adjoiningTriangles = new ESet<int>();
//		//	//	{
//		//	//		verticesByTriangle.TryGetValue( nTriangle, out var list );

//		//	//		foreach( var vertexIndex in list )
//		//	//		{
//		//	//			trianglesByVertex.TryGetValue( vertexIndex, out var list2 );
//		//	//			foreach( var tri in list2 )
//		//	//			{
//		//	//				if( tri != nTriangle )
//		//	//					adjoiningTriangles.AddWithCheckAlreadyContained( tri );
//		//	//			}
//		//	//		}
//		//	//	}

//		//	//	foreach( var tri in adjoiningTriangles )
//		//	//	{
//		//	//		if( !cluster.Triangles.Contains( tri ) )
//		//	//		{
//		//	//			cluster.Triangles.Add( tri );
//		//	//			trianglesToConsider.Remove( tri );

//		//	//			toCheck.Enqueue( tri );
//		//	//		}
//		//	//	}
//		//	//}

//		//}

//		////static T GetElementIndex<T>( ESet<T> set, int value )
//		////{
//		////	var current = 0;

//		////	var enumerator = set.GetEnumerator();
//		////	while( enumerator.MoveNext() )
//		////	{
//		////		var item = enumerator.Current;
//		////		if( current == value )
//		////			return item;
//		////		current++;
//		////	}

//		////	return default;
//		////}

//		//struct TriangleMaxHeightItem
//		//{
//		//	public int TriangleId;
//		//	public float MaxHeight;

//		//	public TriangleMaxHeightItem( int triangleId, float maxHeight )
//		//	{
//		//		TriangleId = triangleId;
//		//		MaxHeight = maxHeight;
//		//	}
//		//}

//		public struct TriangleBounds
//		{
//			public BoundsF Bounds;
//			public int TriangleIndex;
//		}

//		class Node
//		{
//			public BoundsF Bounds;
//			public Node Left;
//			public Node Right;
//			public List<int> TriangleIndexes;

//			public bool IsLeaf => TriangleIndexes != null;
//		}

//		static BoundsF CalcBounds( ArraySegment<TriangleBounds> triangleBoundsArray )
//		{
//			var result = BoundsF.Cleared;
//			for( var i = 0; i < triangleBoundsArray.Count; ++i )
//				result.Add( triangleBoundsArray[ i ].Bounds );
//			return result;

//			//var min = Vector3.one * float.MaxValue;
//			//var max = Vector3.one * float.MinValue;

//			//for( var i = 0; i < triangleBoundsArray.Length; ++i )
//			//{
//			//	var bounds = triangleBoundsArray[ i ].bounds;
//			//	min = Vector3.Min( min, bounds.min );
//			//	max = Vector3.Max( max, bounds.max );
//			//}

//			//return new Bounds() { min = min, max = max };
//		}

//		// SAH(Surface Area Heuristics)
//		// the current bbox has a cost of (number of triangles) * surfaceArea of C = N * SA
//		static (BoundsF, float) CalcBoundsAndSAH( ArraySegment<TriangleBounds> triangleBoundsArray )
//		{
//			var bounds = CalcBounds( triangleBoundsArray );

//			var size = bounds.GetSize();
//			//!!!!
//			var sah = triangleBoundsArray.Count * ( size.X * size.Y + size.X * size.Z + size.Y * size.Z );
//			//var sah = triangleBoundsArray.Length * ( size.x * size.y + size.x * size.y + size.y * size.z );

//			return (bounds, sah);
//		}

//		static (ArraySegment<TriangleBounds> left, ArraySegment<TriangleBounds> right) SplitLR( ArraySegment<TriangleBounds> triBoundsArray, int axis, float split, ref TriangleBounds[] leftBuf, ref TriangleBounds[] rightBuf )
//		{
//			var leftCount = 0;
//			var rightCount = 0;

//			for( var i = 0; i < triBoundsArray.Count; ++i )
//			{
//				var tb = triBoundsArray[ i ];

//				if( tb.Bounds.GetCenter()[ axis ] < split )
//					leftBuf[ leftCount++ ] = tb;
//				else
//					rightBuf[ rightCount++ ] = tb;
//			}

//			return (new ArraySegment<TriangleBounds>( leftBuf, 0, leftCount ), new ArraySegment<TriangleBounds>( rightBuf, 0, rightCount ));
//			//return (leftBuf.Slice( 0, leftCount ), rightBuf.Slice( 0, rightCount ));
//		}

//		static Node CreateBvhRecursive( ArraySegment<TriangleBounds> triangleBoundsArray, int splitCount, int recursiveCount = 0 )
//		{
//			static Node CreateBvhNodeLeaf( ArraySegment<TriangleBounds> triangleBoundsArray )
//			{
//				return new Node()
//				{
//					Bounds = CalcBounds( triangleBoundsArray ),
//					TriangleIndexes = triangleBoundsArray.Select( n => n.TriangleIndex ).ToList()
//				};
//			}

//			// Find smallest cost split
//			// Select Axis  0 = X, 1 = Y, 2 = Z
//			var bestSplit = 0f;
//			var bestAxis = -1;

//			if( triangleBoundsArray.Count >= 4 )
//			{
//				var (totalBounds, minCost) = CalcBoundsAndSAH( triangleBoundsArray );
//				var size = totalBounds.GetSize();

//				var leftBuf = new TriangleBounds[ triangleBoundsArray.Count ];
//				var rightBuf = new TriangleBounds[ triangleBoundsArray.Count ];

//				for( var axis = 0; axis < 3; ++axis )
//				{
//					if( size[ axis ] < 0.001 ) continue;

//					var step = size[ axis ] / ( splitCount / ( recursiveCount + 1 ) );

//					var stepStart = totalBounds.Minimum[ axis ] + step;
//					var stepEnd = totalBounds.Maximum[ axis ] - step;


//					for( var testSplit = stepStart; testSplit < stepEnd; testSplit += step )
//					{
//						var (left, right) = SplitLR( triangleBoundsArray, axis, testSplit, ref leftBuf, ref rightBuf );

//						if( left.Count <= 1 || right.Count <= 1 )
//							continue;

//						var (_, costLeft) = CalcBoundsAndSAH( left );
//						var (_, costRight) = CalcBoundsAndSAH( right );

//						var cost = costLeft + costRight;

//						if( cost < minCost )
//						{
//							minCost = cost;
//							bestAxis = axis;
//							bestSplit = testSplit;
//						}
//					}
//				}
//			}


//			Node ret;

//			// Not Split
//			if( bestAxis < 0 )
//			{
//				ret = CreateBvhNodeLeaf( triangleBoundsArray );
//			}
//			// Calc child
//			else
//			{
//				var leftBuf = new TriangleBounds[ triangleBoundsArray.Count ];
//				var rightBuf = new TriangleBounds[ triangleBoundsArray.Count ];

//				var (left, right) = SplitLR( triangleBoundsArray, bestAxis, bestSplit, ref leftBuf, ref rightBuf );

//				var leftNode = CreateBvhRecursive( left, splitCount, recursiveCount + 1 );
//				var rightNode = CreateBvhRecursive( right, splitCount, recursiveCount + 1 );

//				var bounds = leftNode.Bounds;
//				bounds.Add( ref rightNode.Bounds );
//				//bounds.Encapsulate( rightNode.Bounds );

//				ret = new Node()
//				{
//					Bounds = bounds,
//					Left = leftNode,
//					Right = rightNode
//				};
//			}

//			return ret;
//		}

//		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
//		struct BvhData
//		{
//			//16 bytes
//			public HalfType MinimumX;
//			public HalfType MinimumY;
//			public float MinimumZ;
//			public HalfType MaximumX;
//			public HalfType MaximumY;
//			public float MaximumZ;

//			//!!!!bez pakovaniya v 32 bytes vlazit. sravnit
//			//!!!!can merge to 2 ints
//			//16 bytes
//			public float leftIdx;
//			public float rightIdx;
//			public float triangleIdx; // -1 if data is not leaf
//			public float triangleCount;

//			public BoundsF Bounds
//			{
//				//get
//				//{
//				//	return new Bounds( MinimumX, MinimumY, MinimumZ
//				//}
//				set
//				{
//					MinimumX = new HalfType( value.Minimum.X );
//					MinimumY = new HalfType( value.Minimum.Y );
//					MinimumZ = value.Minimum.Z;
//					MaximumX = new HalfType( value.Maximum.X );
//					MaximumY = new HalfType( value.Maximum.Y );
//					MaximumZ = value.Maximum.Z;
//				}
//			}

//			//public bool IsLeaf => triangleIdx >= 0;
//		}

//		[StructLayout( LayoutKind.Sequential, Pack = 1 )]
//		struct VirtualizedTriangle
//		{
//			//16 bytes
//			public float Index0;
//			public float Index1;
//			public float Index2;
//			public float MaterialIndex;
//		}

//		static void CreatteBvhDatasRecursive( Node node, List<BvhData> datas, List<int> triangleIndexes )
//		{
//			var data = new BvhData()
//			{
//				Bounds = node.Bounds,
//				//min = node.Bounds.Minimum,
//				//max = node.Bounds.Maximum,
//				leftIdx = -1,
//				rightIdx = -1,
//				triangleIdx = -1,
//				triangleCount = 0
//			};

//			if( node.IsLeaf )
//			{
//				var idx = triangleIndexes.Count;
//				triangleIndexes.AddRange( node.TriangleIndexes );

//				data.triangleIdx = idx;
//				data.triangleCount = node.TriangleIndexes.Count;

//				datas.Add( data );
//			}
//			else
//			{
//				data.triangleIdx = -1;

//				var dataIdx = datas.Count;
//				datas.Add( default ); // reserve my data idx

//				data.leftIdx = datas.Count;
//				CreatteBvhDatasRecursive( node.Left, datas, triangleIndexes );

//				data.rightIdx = datas.Count;
//				CreatteBvhDatasRecursive( node.Right, datas, triangleIndexes );

//				datas[ dataIdx ] = data;
//			}
//		}

//		static (List<BvhData>, List<int>) CreatteBvhDatas( Node node )
//		{
//			var datas = new List<BvhData>();
//			var triangleIndexes = new List<int>();

//			CreatteBvhDatasRecursive( node, datas, triangleIndexes );

//			return (datas, triangleIndexes);
//		}

//		public unsafe bool Calculate()
//		{
//			//!!!!support procedural geometries

//			if( sizeof( BvhData ) != 32 )
//				Log.Fatal( "MeshGeometry: CalculateVirtualized: sizeof( BvhData ) != 32." );

//			var vertexStructure = Geometry.VertexStructure.Value;
//			Geometry.VerticesExtractStandardVertex( out var vertices, out var vertexComponents );
//			var indices = Geometry.Indices.Value;

//			if( vertexStructure == null || vertices == null || vertices.Length == 0 || indices == null || indices.Length == 0 )
//				return false;

//			var vertexMaterialIndexes = Geometry.VerticesExtractChannel<float>( VertexElementSemantic.Color3 );

//			//!!!!
//			//var time = DateTime.Now;


//			//select format
//			var fullFormat = false;
//			if( vertexStructure.GetElementBySemantic( VertexElementSemantic.Color0, out _ ) )
//				fullFormat = true;
//			else if( vertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate1, out _ ) )
//				fullFormat = true;
//			else if( vertexStructure.GetElementBySemantic( VertexElementSemantic.TextureCoordinate2, out _ ) )
//				fullFormat = true;

//			var triangleCount = indices.Length / 3;


//			//calculate


//			//!!!!
//			int splitCount = 64;


//			var trianglesBounds = new TriangleBounds[ triangleCount ];
//			for( int tri = 0; tri < triangleCount; tri++ )
//			{
//				var index0 = indices[ tri * 3 + 0 ];
//				var index1 = indices[ tri * 3 + 1 ];
//				var index2 = indices[ tri * 3 + 2 ];

//				var bounds = new BoundsF( vertices[ index0 ].Position );
//				bounds.Add( ref vertices[ index1 ].Position );
//				bounds.Add( ref vertices[ index2 ].Position );

//				trianglesBounds[ tri ] = new TriangleBounds() { Bounds = bounds, TriangleIndex = tri };
//			}


//			var rootNode = CreateBvhRecursive( new ArraySegment<TriangleBounds>( trianglesBounds ), splitCount );

//			var (bvhDatas, triangleIndexes) = CreatteBvhDatas( rootNode );

//			var newTriangles = new VirtualizedTriangle[ triangleCount ];
//			for( int tri = 0; tri < newTriangles.Length; tri++ )
//			{
//				//!!!!right?
//				var tri2 = triangleIndexes[ tri ];

//				var triangle = new VirtualizedTriangle();
//				triangle.Index0 = indices[ tri2 * 3 + 0 ];
//				triangle.Index1 = indices[ tri2 * 3 + 1 ];
//				triangle.Index2 = indices[ tri2 * 3 + 2 ];
//				//!!!!right?
//				if( vertexMaterialIndexes != null )
//					triangle.MaterialIndex = vertexMaterialIndexes[ (int)triangle.Index0 ];

//				newTriangles[ tri ] = triangle;
//			}

//			//var newIndices = new int[ indices.Length ];
//			//for( int tri = 0; tri < newIndices.Length / 3; tri++ )
//			//{
//			//	//!!!!good?
//			//	var tri2 = triangleIndexes[ tri ];
//			//	newIndices[ tri * 3 + 0 ] = indices[ tri2 * 3 + 0 ];
//			//	newIndices[ tri * 3 + 1 ] = indices[ tri2 * 3 + 1 ];
//			//	newIndices[ tri * 3 + 2 ] = indices[ tri2 * 3 + 2 ];
//			//}
//			//var sortedTriangles = triangleIndexes.Select( idx => triangles[ idx ] ).ToList();






//			////calculate groups
//			//var multiMaterialGroups = new List<MultiMaterialGroup>();
//			//{
//			//	var vertexMaterialIndexes = Geometry.VerticesExtractChannel<float>( VertexElementSemantic.Color3 );
//			//	if( vertexMaterialIndexes != null )
//			//	{
//			//		var ranges = MeshGeometry.GetMaterialIndexRangesFromVertexMaterialIndexes( vertexMaterialIndexes, allIndices );
//			//		if( ranges != null )
//			//		{
//			//			for( int materialIndex = 0; materialIndex < ranges.Length; materialIndex++ )
//			//			{
//			//				var range = ranges[ materialIndex ];

//			//				var newVertices2 = new OpenList<StandardVertex>( allVertices.Length );
//			//				var firstVertexIndex = -1;

//			//				for( int n = 0; n < vertexMaterialIndexes.Length; n++ )
//			//				{
//			//					if( vertexMaterialIndexes[ n ] == materialIndex )
//			//					{
//			//						newVertices2.Add( ref allVertices[ n ] );

//			//						if( firstVertexIndex == -1 )
//			//							firstVertexIndex = n;
//			//					}
//			//				}

//			//				var newIndices2 = new List<int>( range.Size + 1 );

//			//				//!!!!check. в SetVertexIndexBuffers тоже проверить
//			//				for( int n = range.Minimum; n < range.Maximum; n++ )
//			//				{
//			//					var index = allIndices[ n ];
//			//					newIndices2.Add( index - firstVertexIndex );
//			//				}

//			//				var group = new MultiMaterialGroup();
//			//				group.Vertices = newVertices2.ToArray();
//			//				group.Indices = newIndices2.ToArray();
//			//				group.MaterialIndex = materialIndex;
//			//				multiMaterialGroups.Add( group );
//			//			}
//			//		}
//			//	}
//			//	else
//			//	{
//			//		var group = new MultiMaterialGroup();
//			//		group.Vertices = allVertices;
//			//		group.Indices = allIndices;
//			//		multiMaterialGroups.Add( group );
//			//	}
//			//}

//			////precalculate some data
//			//foreach( var group in multiMaterialGroups )
//			//{
//			//	var vertices = group.Vertices;
//			//	var indices = group.Indices;
//			//	var triangleCount = indices.Length / 3;

//			//	group.TriangleNormals = new Vector3F[ triangleCount ];
//			//	for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
//			//	{
//			//		var index0 = indices[ nTriangle * 3 + 0 ];
//			//		var index1 = indices[ nTriangle * 3 + 1 ];
//			//		var index2 = indices[ nTriangle * 3 + 2 ];

//			//		ref var v0 = ref vertices[ index0 ].Position;
//			//		ref var v1 = ref vertices[ index1 ].Position;
//			//		ref var v2 = ref vertices[ index2 ].Position;

//			//		MathAlgorithms.CalculateTriangleNormal( ref v0, ref v1, ref v2, out group.TriangleNormals[ nTriangle ] );
//			//	}

//			//	group.VerticesByPosition = new Dictionary<Vector3F, List<int>>( vertices.Length );
//			//	for( int nVertex = 0; nVertex < vertices.Length; nVertex++ )
//			//	{
//			//		ref var v = ref vertices[ nVertex ];

//			//		if( !group.VerticesByPosition.TryGetValue( v.Position, out var list ) )
//			//		{
//			//			list = new List<int>();
//			//			group.VerticesByPosition[ v.Position ] = list;
//			//		}

//			//		list.Add( nVertex );
//			//	}

//			//	group.VerticesByTriangle = new Dictionary<int, List<int>>( 512 );
//			//	for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
//			//	{
//			//		var index0 = indices[ nTriangle * 3 + 0 ];
//			//		var index1 = indices[ nTriangle * 3 + 1 ];
//			//		var index2 = indices[ nTriangle * 3 + 2 ];

//			//		if( !group.VerticesByTriangle.TryGetValue( nTriangle, out var list ) )
//			//		{
//			//			list = new List<int>();
//			//			group.VerticesByTriangle[ nTriangle ] = list;
//			//		}

//			//		if( !list.Contains( index0 ) )
//			//			list.Add( index0 );
//			//		if( !list.Contains( index1 ) )
//			//			list.Add( index1 );
//			//		if( !list.Contains( index2 ) )
//			//			list.Add( index2 );
//			//	}

//			//	group.TrianglesByVertex = new Dictionary<int, List<int>>( 512 );
//			//	for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
//			//	{
//			//		var index0 = indices[ nTriangle * 3 + 0 ];
//			//		var index1 = indices[ nTriangle * 3 + 1 ];
//			//		var index2 = indices[ nTriangle * 3 + 2 ];

//			//		if( !group.TrianglesByVertex.TryGetValue( index0, out var list ) )
//			//		{
//			//			list = new List<int>();
//			//			group.TrianglesByVertex[ index0 ] = list;
//			//		}
//			//		if( !list.Contains( nTriangle ) )
//			//			list.Add( nTriangle );

//			//		if( !group.TrianglesByVertex.TryGetValue( index1, out list ) )
//			//		{
//			//			list = new List<int>();
//			//			group.TrianglesByVertex[ index1 ] = list;
//			//		}
//			//		if( !list.Contains( nTriangle ) )
//			//			list.Add( nTriangle );

//			//		if( !group.TrianglesByVertex.TryGetValue( index2, out list ) )
//			//		{
//			//			list = new List<int>();
//			//			group.TrianglesByVertex[ index2 ] = list;
//			//		}
//			//		if( !list.Contains( nTriangle ) )
//			//			list.Add( nTriangle );
//			//	}

//			//	group.AdjoiningTriangles = new MultiMaterialGroup.AdjoiningTrianglesItem[ triangleCount ];
//			//	for( int nTriangle = 0; nTriangle < triangleCount; nTriangle++ )
//			//	{
//			//		var resultList = new ESet<int>( 12 );

//			//		group.VerticesByTriangle.TryGetValue( nTriangle, out var list );
//			//		foreach( var vertexIndex in list )
//			//		{
//			//			if( group.TrianglesByVertex.TryGetValue( vertexIndex, out var list2 ) )
//			//			{
//			//				foreach( var tri in list2 )
//			//				{
//			//					if( tri != nTriangle )
//			//						resultList.AddWithCheckAlreadyContained( tri );
//			//				}
//			//			}

//			//			//also add triangle which have vertices with same position but not same
//			//			if( group.VerticesByPosition.TryGetValue( vertices[ vertexIndex ].Position, out var list3 ) )
//			//			{
//			//				foreach( var vertexIndex2 in list3 )
//			//				{
//			//					if( group.TrianglesByVertex.TryGetValue( vertexIndex2, out var list4 ) )
//			//					{
//			//						foreach( var tri in list4 )
//			//						{
//			//							if( tri != nTriangle )
//			//								resultList.AddWithCheckAlreadyContained( tri );
//			//						}
//			//					}
//			//				}
//			//			}
//			//		}

//			//		var item = new MultiMaterialGroup.AdjoiningTrianglesItem() { Triangles = resultList.ToArray() };
//			//		group.AdjoiningTriangles[ nTriangle ] = item;
//			//	}
//			//}

//			////process
//			//var resultClusters = new List<Cluster>( 512 );

//			//foreach( var group in multiMaterialGroups )
//			//{
//			//	var vertices = group.Vertices;
//			//	var indices = group.Indices;
//			//	var triangleCount = indices.Length / 3;


//			//	//find clusters
//			//	{
//			//		//var random = new FastRandom( 0 );

//			//		var trianglesToConsider = new ESet<int>( triangleCount );
//			//		for( int n = 0; n < triangleCount; n++ )
//			//			trianglesToConsider.Add( n );

//			//		var processedTrianglesWithoutCluster = new List<int>( 1024 );


//			//		while( trianglesToConsider.Count != 0 )
//			//		{
//			//			//!!!!
//			//			int randomSelectionCount = 10;// 50;// 20;//10

//			//			if( trianglesToConsider.Count < randomSelectionCount )
//			//				randomSelectionCount = trianglesToConsider.Count;


//			//			//!!!!
//			//			randomSelectionCount = 1;


//			//			var toCheck = new List<int>( randomSelectionCount );
//			//			{
//			//				var array = trianglesToConsider.ToArray();

//			//				for( int n = 0; n < randomSelectionCount; n++ )
//			//				{
//			//					var index = ( array.Length * n ) / randomSelectionCount;
//			//					if( index >= array.Length )
//			//						index = array.Length - 1;
//			//					var tri = array[ index ];

//			//					toCheck.Add( tri );
//			//				}

//			//				//slowly
//			//				//var remaining = (ESet<int>)trianglesToConsider.Clone();
//			//				//for( int n = 0; n < randomSelectionCount; n++ )
//			//				//{
//			//				//	var index = random.Next( remaining.Count - 1 );
//			//				//	var tri = GetElementIndex( remaining, index );
//			//				//	remaining.Remove( tri );
//			//				//	toCheck.Add( tri );
//			//				//}
//			//			}

//			//			//sort because can be different order because threading
//			//			var clustersToCompare = new List<(Cluster, int)>( toCheck.Count * 3 );
//			//			Parallel.ForEach( toCheck, delegate ( int startTriangle )
//			//			{
//			//				//!!!!
//			//				//!!!!need
//			//				int edge = 0;
//			//				//for( int edge = 0; edge < 3; edge++ )
//			//				{
//			//					var cluster = CalculateCluster( group, trianglesToConsider, startTriangle, edge );
//			//					if( cluster != null )
//			//					{
//			//						lock( clustersToCompare )
//			//							clustersToCompare.Add( (cluster, startTriangle) );
//			//					}
//			//				}
//			//			} );
//			//			CollectionUtility.MergeSort( clustersToCompare, delegate ( (Cluster, int) v1, (Cluster, int) v2 )
//			//			{
//			//				return v2.Item2 - v1.Item2;
//			//			} );

//			//			//get best cluster by score
//			//			Cluster bestCluster = null;
//			//			var bestScore = 0.0;
//			//			foreach( var cluster in clustersToCompare )
//			//			{
//			//				var score = cluster.Item1.GetScore();
//			//				if( bestCluster == null || score > bestScore )
//			//				{
//			//					bestCluster = cluster.Item1;
//			//					bestScore = score;
//			//				}
//			//			}

//			//			//add selected cluster
//			//			if( !bestCluster.TrianglesMode )
//			//				resultClusters.Add( bestCluster );
//			//			else
//			//				processedTrianglesWithoutCluster.AddRange( bestCluster.Triangles );

//			//			//remove triangles from processing
//			//			foreach( var tri in bestCluster.Triangles )
//			//				trianglesToConsider.Remove( tri );
//			//		}

//			//		//processedTrianglesWithoutCluster
//			//		if( processedTrianglesWithoutCluster.Count != 0 )
//			//		{
//			//			var cluster = new Cluster();
//			//			cluster.Group = group;
//			//			cluster.TrianglesMode = true;
//			//			foreach( var tri in processedTrianglesWithoutCluster )
//			//				cluster.Triangles.Add( tri );

//			//			resultClusters.Add( cluster );
//			//		}
//			//	}
//			//}


//			////check created clusters
//			//{
//			//	//check for shared triangles

//			//	var triangles = new ESet<int>( allIndices.Length / 3 );
//			//	foreach( var cluster in resultClusters )
//			//	{
//			//		foreach( var tri in cluster.Triangles )
//			//		{
//			//			if( triangles.Contains( tri ) )
//			//			{
//			//				Log.Warning( "MeshGeometry: CalculateClusters: Internal error. Triangles overlapping." );
//			//				return false;
//			//			}

//			//			triangles.Add( tri );
//			//		}
//			//	}
//			//}


//			////calculate cluster info

//			//var totalVertexCount = 0;
//			//var totalTriangleCount = 0;

//			//for( int nCluster = 0; nCluster < resultClusters.Count; nCluster++ )
//			//{
//			//	var cluster = resultClusters[ nCluster ];
//			//	var group = cluster.Group;


//			//	ref var clusterInfo = ref cluster.ClusterInfo;

//			//	if( cluster.TrianglesMode )
//			//		clusterInfo.Flags |= MeshGeometry.ClusterDataHeaderClusterInfo.FlagsEnum.TrianglesMode;
//			//	if( fullFormat )
//			//		clusterInfo.Flags |= MeshGeometry.ClusterDataHeaderClusterInfo.FlagsEnum.FullFormat;
//			//	clusterInfo.MaterialIndex = group.MaterialIndex;
//			//	clusterInfo.Position = cluster.Position;
//			//	clusterInfo.Rotation = cluster.Rotation;
//			//	clusterInfo.CellSize = cluster.CellSize;
//			//	clusterInfo.GridSize = cluster.GridSize;
//			//	clusterInfo.Height = cluster.Height;


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


//			//	if( !cluster.TrianglesMode )
//			//	{
//			//		var clusterSpaceToObjectSpace = new Matrix4F( cluster.Rotation.ToMatrix3(), cluster.Position );
//			//		var objectSpaceToClusterSpace = clusterSpaceToObjectSpace.GetInverse();

//			//		//calculate grid and cell triangles
//			//		var grid = new byte[ cluster.GridSize.X * cluster.GridSize.Y * 16 ];
//			//		var cellTriangleBatches = new List<Vector4F>( 1024 );

//			//		var cellSize = cluster.CellSize;

//			//		fixed( byte* pGrid = grid )
//			//		{
//			//			for( int y = 0; y < cluster.GridSize.Y; y++ )
//			//			{
//			//				for( int x = 0; x < cluster.GridSize.X; x++ )
//			//				{

//			//					//!!!!pack 8 bytes. везде 16 поменять

//			//					//!!!!отсортировать треугольники


//			//					ref var cell = ref cluster.Grid[ x, y ];

//			//					var cellHeight = 0.0f;

//			//					int cellTrianglesCode;
//			//					if( cell.ActualTriangles != null && cell.ActualTriangles.Count > 0 )
//			//					{
//			//						//add cell triangles

//			//						if( cell.ActualTriangles.Count > 8 )
//			//							Log.Fatal( "MeshGeometry: CalculateClusters: cell.ActualTriangles.Count > 8." );

//			//						var clipBounds = new RectangleF( cellSize * x, cellSize * y, cellSize * x + cellSize, cellSize * y + cellSize );


//			//						var actualTriangles = new List<TriangleMaxHeightItem>( 8 );

//			//						for( int n = 0; n < cell.ActualTriangles.Count; n++ )
//			//						{
//			//							var triangleId = cell.ActualTriangles[ n ];

//			//							//calculate max height of the triangle in the cell
//			//							var maxHeight = 0.0f;

//			//							var index0 = cluster.ActualClusterIndices[ triangleId * 3 + 0 ];
//			//							var index1 = cluster.ActualClusterIndices[ triangleId * 3 + 1 ];
//			//							var index2 = cluster.ActualClusterIndices[ triangleId * 3 + 2 ];

//			//							var v0Projected = objectSpaceToClusterSpace * cluster.ActualClusterVertices[ index0 ].Position;
//			//							var v1Projected = objectSpaceToClusterSpace * cluster.ActualClusterVertices[ index1 ].Position;
//			//							var v2Projected = objectSpaceToClusterSpace * cluster.ActualClusterVertices[ index2 ].Position;

//			//							var polygon = new Vector3[] { v0Projected, v1Projected, v2Projected };

//			//							//+X
//			//							{
//			//								var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Maximum, 0 ), -Vector3.XAxis );
//			//								polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
//			//							}

//			//							//+Y
//			//							{
//			//								var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Maximum, 0 ), -Vector3.YAxis );
//			//								polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
//			//							}

//			//							//-X
//			//							{
//			//								var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Minimum, 0 ), Vector3.XAxis );
//			//								polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
//			//							}

//			//							//-Y
//			//							{
//			//								var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Minimum, 0 ), Vector3.YAxis );
//			//								polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
//			//							}

//			//							foreach( var p in polygon )
//			//								maxHeight = Math.Max( maxHeight, (float)p.Z );

//			//							actualTriangles.Add( new TriangleMaxHeightItem( triangleId, maxHeight ) );
//			//						}

//			//						//sort by max height
//			//						CollectionUtility.InsertionSort( actualTriangles, delegate ( TriangleMaxHeightItem i1, TriangleMaxHeightItem i2 )
//			//						{
//			//							if( i1.MaxHeight > i2.MaxHeight )
//			//								return -1;
//			//							if( i1.MaxHeight < i2.MaxHeight )
//			//								return 1;
//			//							return 0;
//			//						} );

//			//						var data = new Vector4F[ 2 ];
//			//						fixed( Vector4F* pData2 = data )
//			//						{
//			//							//clear
//			//							{
//			//								var pData = (HalfType*)pData2;
//			//								for( int n = 0; n < 16; n++ )
//			//								{
//			//									*pData = new HalfType( -100.0f );
//			//									pData++;
//			//								}
//			//							}

//			//							{
//			//								var pData = (HalfType*)pData2;

//			//								foreach( var item in actualTriangles )
//			//								{
//			//									var triangleId = item.TriangleId;
//			//									var maxHeight = item.MaxHeight;

//			//									if( triangleId < 0 )
//			//										Log.Fatal( "MeshGeometry: CalculateClusters: triangleId < 0." );
//			//									if( triangleId >= cluster.ActualClusterIndices.Length / 3 )
//			//										Log.Fatal( "MeshGeometry: CalculateClusters: triangleId >= cluster.ActualClusterIndices.Length / 3." );

//			//									*pData = new HalfType( maxHeight );
//			//									pData++;
//			//									*pData = new HalfType( triangleId );
//			//									pData++;

//			//									if( maxHeight > cellHeight )
//			//										cellHeight = maxHeight;
//			//								}
//			//							}
//			//						}

//			//						var twoBatches = cell.ActualTriangles.Count > 4;

//			//						var cellTrianglesIndex = cellTriangleBatches.Count;

//			//						cellTriangleBatches.Add( data[ 0 ] );
//			//						if( twoBatches )
//			//							cellTriangleBatches.Add( data[ 1 ] );

//			//						cellTrianglesCode = cellTrianglesIndex * 2 + ( twoBatches ? 1 : 0 );
//			//					}
//			//					else
//			//						cellTrianglesCode = -100;


//			//					//!!!!
//			//					var distanceFields = new float[] { 1, 1, 1, 1 };

//			//					byte* pCell = pGrid + ( cluster.GridSize.X * y + x ) * 16;

//			//					//16 bytes
//			//					*(float*)pCell = cellHeight;
//			//					pCell += 4;
//			//					*(float*)pCell = cellTrianglesCode;
//			//					pCell += 4;
//			//					*(HalfType*)pCell = new HalfType( distanceFields[ 0 ] );
//			//					pCell += 2;
//			//					*(HalfType*)pCell = new HalfType( distanceFields[ 1 ] );
//			//					pCell += 2;
//			//					*(HalfType*)pCell = new HalfType( distanceFields[ 2 ] );
//			//					pCell += 2;
//			//					*(HalfType*)pCell = new HalfType( distanceFields[ 3 ] );
//			//					pCell += 2;
//			//				}
//			//			}
//			//		}

//			//		clusterInfo.CellTriangleBatches = cellTriangleBatches.Count;


//			//		var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );
//			//		var triangleCount = cluster.ActualClusterIndices.Length / 3;

//			//		var gridSizeInBytes = cluster.GridSize.X * cluster.GridSize.Y * 16;
//			//		var cellTrianglesSizeInBytes = cellTriangleBatches.Count * 16;
//			//		var trianglesSizeInBytes = ( triangleCount + 1 ) / 2 * 16;
//			//		var verticesSizeInBytes = cluster.ActualClusterVertices.Length * vertexSizeInBytes;
//			//		var totalSizeInBytes = gridSizeInBytes + cellTrianglesSizeInBytes + trianglesSizeInBytes + verticesSizeInBytes;

//			//		if( totalSizeInBytes % 16 != 0 )
//			//			Log.Fatal( "MeshGeometry: CalculateClusters: totalSizeInBytes % 16 != 0." );

//			//		cluster.ClusterBody = new byte[ totalSizeInBytes ];
//			//		fixed( byte* pClusterBody = cluster.ClusterBody )
//			//		{
//			//			byte* pGrid = pClusterBody;
//			//			byte* pCellTriangles = pGrid + gridSizeInBytes;
//			//			byte* pTriangles = pCellTriangles + cellTrianglesSizeInBytes;
//			//			byte* pVertices = pTriangles + trianglesSizeInBytes;

//			//			//write grid
//			//			{
//			//				fixed( byte* p = grid )
//			//					NativeUtility.CopyMemory( pGrid, p, grid.Length );

//			//				//for( int y = 0; y < cluster.GridSize.Y; y++ )
//			//				//{
//			//				//	for( int x = 0; x < cluster.GridSize.X; x++ )
//			//				//	{
//			//				//		byte* pCell = pGrid + ( cluster.GridSize.X * y + x ) * 16;

//			//				//		//!!!!pack 8 bytes

//			//				//		var cellHeight = zzz;

//			//				//		var cellTrianglesCount = zzz;

//			//				//		int cellTrianglesCode;
//			//				//		if( cellTrianglesCount > 0 )
//			//				//			cellTrianglesCode = cellTrianglesIndex * 2 + ( cellTrianglesCount > 4 ? 1 : 0 );
//			//				//		else
//			//				//			cellTrianglesCode = -100;

//			//				//		var distanceFields = zzzzz;

//			//				//		//16 bytes
//			//				//		*(float*)pCell = cellHeight;
//			//				//		pCell += 4;
//			//				//		*(float*)pCell = cellTrianglesCode;
//			//				//		pCell += 4;
//			//				//		*(HalfType*)pCell = new HalfType( distanceFields[ 0 ] );
//			//				//		pCell += 2;
//			//				//		*(HalfType*)pCell = new HalfType( distanceFields[ 1 ] );
//			//				//		pCell += 2;
//			//				//		*(HalfType*)pCell = new HalfType( distanceFields[ 2 ] );
//			//				//		pCell += 2;
//			//				//		*(HalfType*)pCell = new HalfType( distanceFields[ 3 ] );
//			//				//		pCell += 2;
//			//				//	}
//			//				//}
//			//			}

//			//			//write cell triangles
//			//			{
//			//				var ar = cellTriangleBatches.ToArray();
//			//				fixed( Vector4F* p = ar )
//			//					NativeUtility.CopyMemory( pCellTriangles, p, cellTriangleBatches.Count * 16 );

//			//				//int currentCellTrianglesOffset = 0;

//			//				//byte* pCurrent = pCellTriangles;

//			//				//for( int nTriangleSet = 0; nTriangleSet < zzzcluster.CellTriangles.Length; nTriangleSet++ )
//			//				//{
//			//				//	var triangleCount = zz;

//			//				//	zzzzzz;
//			//				//	var values = new Vector2H[ 8 ];
//			//				//	for( int n = 0; n < values.Length; n++ )
//			//				//		values[ n ] = new Vector2H( -1.0f, 0 );

//			//				//	for( int n = 0; n < triangleCount; n++ )
//			//				//	{
//			//				//		values[ n ] = zzzzz;
//			//				//	}

//			//				//	zzz;

//			//				//	var parts = triangleCount >= 5 ? 2 : 1;
//			//				//	//var parts = triangleCount >= 4 ? 2 : 1;


//			//				//	//*(HalfType*)pTriangleSets = new HalfType( height );
//			//				//	//pCurrent += 2;
//			//				//	//*(HalfType*)pTriangleSets = new HalfType( triangleDataOffset );
//			//				//	//pCurrent += 2;

//			//				//	zzzz;

//			//				//	currentCellTrianglesOffset += zzz;
//			//				//}
//			//			}

//			//			//write triangles
//			//			for( int nTriangle = 0; nTriangle < cluster.ActualClusterIndices.Length / 3; nTriangle++ )
//			//			{
//			//				byte* pTriangle = pTriangles + nTriangle * 8;

//			//				//8 bytes
//			//				*(HalfType*)pTriangle = new HalfType( cluster.ActualClusterIndices[ nTriangle * 3 + 0 ] );
//			//				pTriangle += 2;
//			//				*(HalfType*)pTriangle = new HalfType( cluster.ActualClusterIndices[ nTriangle * 3 + 1 ] );
//			//				pTriangle += 2;
//			//				*(HalfType*)pTriangle = new HalfType( cluster.ActualClusterIndices[ nTriangle * 3 + 2 ] );
//			//				pTriangle += 2;
//			//				*(HalfType*)pTriangle = new HalfType( 0 );
//			//				pTriangle += 2;
//			//			}
//			//			//it is not aligned 16 bytes


//			//			//write vertices
//			//			for( int nVertex = 0; nVertex < cluster.ActualClusterVertices.Length; nVertex++ )
//			//			{
//			//				ref var v = ref cluster.ActualClusterVertices[ nVertex ];

//			//				byte* pVertex = pVertices + nVertex * vertexSizeInBytes;

//			//				//position in cluster space
//			//				//var clusterSpacePosition = objectSpaceToClusterSpace * v.Position;
//			//				//var clusterSpacePosition = cluster.Rotation.GetInverse() * ( v.Position - cluster.Position );

//			//				//var packedNormalAndTangent = new Vector3F( 1, 0, 0 );

//			//				//16 bytes
//			//				*(float*)pVertex = v.Position.X;
//			//				pVertex += 4;
//			//				*(float*)pVertex = v.Position.Y;
//			//				pVertex += 4;
//			//				*(float*)pVertex = v.Position.Z;
//			//				pVertex += 4;
//			//				*(HalfType*)pVertex = new HalfType( v.TexCoord0.X );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.TexCoord0.Y );
//			//				pVertex += 2;

//			//				//16 bytes
//			//				*(HalfType*)pVertex = new HalfType( v.Normal.X );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.Normal.Y );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.Normal.Z );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( 0.0f );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.Tangent.X );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.Tangent.Y );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.Tangent.Z );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.Tangent.W );
//			//				pVertex += 2;

//			//				if( fullFormat )
//			//				{
//			//					//16 bytes
//			//					*(HalfType*)pVertex = new HalfType( v.TexCoord1.X );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.TexCoord1.Y );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.TexCoord2.X );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.TexCoord2.Y );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.Color.Red );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.Color.Green );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.Color.Blue );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.Color.Alpha );
//			//					pVertex += 2;
//			//				}


//			//				////16 bytes
//			//				//*(HalfType*)pVertex = new HalfType( clusterSpacePosition.X );
//			//				//pVertex += 2;
//			//				//*(HalfType*)pVertex = new HalfType( clusterSpacePosition.Y );
//			//				//pVertex += 2;
//			//				//*(HalfType*)pVertex = new HalfType( clusterSpacePosition.Z );
//			//				//pVertex += 2;
//			//				//*(HalfType*)pVertex = new HalfType( packedNormalAndTangent.X );
//			//				//pVertex += 2;
//			//				//*(HalfType*)pVertex = new HalfType( packedNormalAndTangent.Y );
//			//				//pVertex += 2;
//			//				//*(HalfType*)pVertex = new HalfType( packedNormalAndTangent.Z );
//			//				//pVertex += 2;
//			//				//*(HalfType*)pVertex = new HalfType( v.TexCoord0.X );
//			//				//pVertex += 2;
//			//				//*(HalfType*)pVertex = new HalfType( v.TexCoord0.Y );
//			//				//pVertex += 2;

//			//				//if( fullFormat )
//			//				//{
//			//				//	//16 bytes
//			//				//	*(HalfType*)pVertex = new HalfType( v.TexCoord1.X );
//			//				//	pVertex += 2;
//			//				//	*(HalfType*)pVertex = new HalfType( v.TexCoord1.Y );
//			//				//	pVertex += 2;
//			//				//	*(HalfType*)pVertex = new HalfType( v.TexCoord2.X );
//			//				//	pVertex += 2;
//			//				//	*(HalfType*)pVertex = new HalfType( v.TexCoord2.Y );
//			//				//	pVertex += 2;
//			//				//	*(HalfType*)pVertex = new HalfType( v.Color.Red );
//			//				//	pVertex += 2;
//			//				//	*(HalfType*)pVertex = new HalfType( v.Color.Green );
//			//				//	pVertex += 2;
//			//				//	*(HalfType*)pVertex = new HalfType( v.Color.Blue );
//			//				//	pVertex += 2;
//			//				//	*(HalfType*)pVertex = new HalfType( v.Color.Alpha );
//			//				//	pVertex += 2;
//			//				//}
//			//			}
//			//		}

//			//		clusterInfo.ActualVertexCount = cluster.ActualClusterVertices.Length;
//			//		clusterInfo.ActualTriangleCount = cluster.ActualClusterIndices.Length / 3;
//			//	}
//			//	else
//			//	{
//			//		var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );
//			//		var verticesSizeInBytes = cluster.ClusterVertices.Length * vertexSizeInBytes;
//			//		var trianglesSizeInBytes = cluster.ClusterIndices.Length / 3 * 12;

//			//		cluster.ClusterBody = new byte[ verticesSizeInBytes + trianglesSizeInBytes ];
//			//		fixed( byte* pClusterBody = cluster.ClusterBody )
//			//		{
//			//			//write vertices
//			//			byte* pVertices = pClusterBody;
//			//			for( int n = 0; n < cluster.ClusterVertices.Length; n++ )
//			//			{
//			//				var v = cluster.ClusterVertices[ n ];

//			//				byte* pVertex = pVertices + n * vertexSizeInBytes;

//			//				//16 bytes
//			//				*(float*)pVertex = v.Position.X;
//			//				pVertex += 4;
//			//				*(float*)pVertex = v.Position.Y;
//			//				pVertex += 4;
//			//				*(float*)pVertex = v.Position.Z;
//			//				pVertex += 4;
//			//				*(HalfType*)pVertex = new HalfType( v.TexCoord0.X );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.TexCoord0.Y );
//			//				pVertex += 2;

//			//				//16 bytes
//			//				*(HalfType*)pVertex = new HalfType( v.Normal.X );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.Normal.Y );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.Normal.Z );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( 0 );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.Tangent.X );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.Tangent.Y );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.Tangent.Z );
//			//				pVertex += 2;
//			//				*(HalfType*)pVertex = new HalfType( v.Tangent.W );
//			//				pVertex += 2;

//			//				if( fullFormat )
//			//				{
//			//					//16 bytes
//			//					*(HalfType*)pVertex = new HalfType( v.TexCoord1.X );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.TexCoord1.Y );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.TexCoord2.X );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.TexCoord2.Y );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.Color.Red );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.Color.Green );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.Color.Blue );
//			//					pVertex += 2;
//			//					*(HalfType*)pVertex = new HalfType( v.Color.Alpha );
//			//					pVertex += 2;
//			//				}
//			//			}

//			//			//write indices
//			//			int* pTriangles = (int*)( pClusterBody + verticesSizeInBytes );
//			//			for( int n = 0; n < cluster.ClusterIndices.Length; n++ )
//			//				pTriangles[ n ] = cluster.ClusterIndices[ n ];
//			//		}

//			//		clusterInfo.ActualVertexCount = cluster.ClusterVertices.Length;
//			//		clusterInfo.ActualTriangleCount = cluster.ClusterIndices.Length / 3;
//			//	}

//			//	clusterInfo.TriangleStartOffset = totalTriangleCount;
//			//	clusterInfo.TriangleCount = cluster.ClusterIndices.Length / 3;

//			//	totalVertexCount += cluster.ClusterVertices.Length;
//			//	totalTriangleCount += cluster.ClusterIndices.Length / 3;
//			//}


//			//generate proxy mesh. write Vertices, Indices
//			{
//				Geometry.CalculateSimplification( ProxyMeshFactor, out var proxyMeshVertices, out var proxyMeshIndices, out var error );

//				if( !string.IsNullOrEmpty( error ) )
//				{
//					//!!!!error

//					//!!!!temp
//					Log.Warning( error );

//					return false;
//				}

//				//Geometry.VertexStructure = newVertexStructure;
//				Geometry.Vertices = proxyMeshVertices;
//				Geometry.Indices = proxyMeshIndices;

//				//var resultVertices = new OpenList<Vertex>( totalVertexCount );
//				//var resultIndices = new OpenList<int>( totalTriangleCount * 3 );

//				//for( int nCluster = 0; nCluster < resultClusters.Count; nCluster++ )
//				//{
//				//	var cluster = resultClusters[ nCluster ];
//				//	var group = cluster.Group;

//				//	var startVertexIndex = resultVertices.Count;

//				//	foreach( var p in cluster.ClusterVertices )
//				//	{
//				//		var v = new Vertex();
//				//		v.Position = p.Position;
//				//		//!!!!vertex shader visibility optimization
//				//		//v.Color2 = nCluster;

//				//		resultVertices.Add( ref v );
//				//	}

//				//	foreach( var index in cluster.ClusterIndices )
//				//		resultIndices.Add( index + startVertexIndex );
//				//}

//				//var newVertexStructure = StandardVertex.MakeStructure( StandardVertex.Components.Position, true, out int vertexSize );
//				//unsafe
//				//{
//				//	if( vertexSize != sizeof( Vertex ) )
//				//		Log.Fatal( "MeshGeometry: CalculateClusters: vertexSize != sizeof( Vertex )." );
//				//}

//				//Geometry.VertexStructure = newVertexStructure;
//				//Geometry.Vertices = CollectionUtility.ToByteArray( resultVertices.ToArray() );
//				//Geometry.Indices = resultIndices.ToArray();


//				if( ProxyMeshCompress )
//					Geometry.CompressVertices();

//				if( ProxyMeshOptimize )
//				{
//					try
//					{
//						Geometry.OptimizeVertexCache();
//						Geometry.OptimizeOverdraw();
//						Geometry.OptimizeVertexFetch();
//					}
//					catch { }
//				}
//			}


//			//write VirtualizedData
//			{
//				var header = new MeshGeometry.VirtualizedDataHeader();
//				header.Version = 1;
//				if( fullFormat )
//					header.Flags |= MeshGeometry.VirtualizedDataHeader.FlagsEnum.FullFormat;
//				header.VertexCount = vertices.Length;
//				header.TriangleCount = newTriangles.Length;
//				header.NodeCount = bvhDatas.Count;

//				var vertexSizeInBytes = 32 + ( fullFormat ? 16 : 0 );
//				var verticesSizeInBytes = header.VertexCount * vertexSizeInBytes;
//				var trianglesSizeInBytes = header.TriangleCount * 16;
//				var nodesSizeInBytes = header.NodeCount * 32;
//				var sizeInBytes = sizeof( MeshGeometry.VirtualizedDataHeader ) + verticesSizeInBytes + trianglesSizeInBytes + nodesSizeInBytes;

//				//write result array
//				{
//					var writer = new ArrayDataWriter( sizeInBytes );
//					writer.Write( &header, sizeof( MeshGeometry.VirtualizedDataHeader ) );

//					foreach( var v in vertices )
//					{
//						//16 bytes
//						writer.Write( v.Position );
//						writer.Write( v.TexCoord0.ToVector2H() );

//						//16 bytes
//						writer.Write( v.Normal.ToVector3H() );
//						writer.Write( new HalfType( 0 ) );
//						writer.Write( v.Tangent.ToVector4H() );

//						if( fullFormat )
//						{
//							//16 bytes
//							writer.Write( v.TexCoord1.ToVector2H() );
//							writer.Write( v.TexCoord2.ToVector2H() );
//							writer.Write( v.Color.ToVector4H() );
//						}
//					}

//					writer.Write( newTriangles );
//					writer.Write( bvhDatas.ToArray() );

//					if( writer.Length != sizeInBytes )
//						Log.Fatal( "MeshGeometry: CalculateVirtualized: writer.Length != sizeInBytes." );

//					Geometry.VirtualizedData = writer.ToArray();
//				}
//			}


//			//var time2 = DateTime.Now;
//			//Log.Info( ( time2 - time ).TotalSeconds.ToString() );

//			return true;
//		}
//	}
//}
