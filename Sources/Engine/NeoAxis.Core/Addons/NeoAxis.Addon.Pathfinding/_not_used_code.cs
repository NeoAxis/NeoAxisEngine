// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoAxis.Editor;
using SharpNav;
using SharpNav.Pathfinding;
using SharpNav.Geometry;

//namespace NeoAxis.Addon.Pathfinding
//{
//	class Component_Pathfinding_NotUsed
//	{



//!!!!было
////!!!!пока так
//public bool tempObstaclesNeedUpdate;
//List<uint> tempObstacles = new List<uint>();

//[FieldSerialize( "gridHeight" )]
//float gridHeight = .5f;

////bool needUpdate;

////the list of created dynamic obstacles
//Set<TempObstacle> tempObstacles = new Set<TempObstacle>();

////lock object for all Wrapper calls.
//object nativeLock = new object();

////operations
//object operationsLock = new object();
//Queue<Task> operations = new Queue<Task>();
//QueuedOperation_TempObstacle tempObstacleOperationDuringConstruction;




///// <summary>
///// Partition the walkable surface into simple regions without holes.
///// </summary>
//[DefaultValue( false )]
//[Serialize]
//[Category( "Regions" )]
//public Reference<bool> MonotonePartitioning
//{
//	get { if( _monotonePartitioning.BeginGet() ) MonotonePartitioning = _monotonePartitioning.Get( this ); return _monotonePartitioning.value; }
//	set { if( _monotonePartitioning.BeginSet( ref value ) ) { try { MonotonePartitioningChanged?.Invoke( this ); } finally { _monotonePartitioning.EndSet(); } } }
//}
//public event Action<Component_Pathfinding> MonotonePartitioningChanged;
//ReferenceField<bool> _monotonePartitioning = false;




/////////////////////////////////////////

//public sealed class TempObstacle
//{
//	RecastNavigationSystem owner;

//	internal Vec3 position;
//	internal float radius;
//	internal float height;

//	//!!!!!IntPtr nativeID;
//	internal volatile bool nativeCreated;
//	internal volatile uint nativeID;
//	internal bool deleted;

//	//

//	internal TempObstacle( RecastNavigationSystem owner, Vec3 position, float radius, float height )
//	{
//		this.owner = owner;
//		this.position = position;
//		this.radius = radius;
//		this.height = height;
//	}

//	public RecastNavigationSystem Owner
//	{
//		get { return owner; }
//	}

//	public Vec3 Position
//	{
//		get { return position; }
//	}

//	public float Radius
//	{
//		get { return radius; }
//	}

//	public float Height
//	{
//		get { return height; }
//	}
//}


///////////////////////////////////////////

//public class FindPath_InBackground_State
//{
//	internal volatile bool finished;
//	internal volatile bool pathFound;
//	internal volatile Vec3[] path;

//	object tag;

//	public bool Finished
//	{
//		get { return finished; }
//	}

//	public bool PathFound
//	{
//		get { return pathFound; }
//	}

//	public Vec3[] Path
//	{
//		get { return path; }
//	}

//	public object Tag
//	{
//		get { return tag; }
//		set { tag = value; }
//	}
//}

///////////////////////////////////////////

//class QueuedOperation_TempObstacle
//{
//	public class AddData
//	{
//		public Vec3 position;
//		public float radius;
//		public float height;
//	}
//	public Dictionary<TempObstacle, AddData> add = new Dictionary<TempObstacle, AddData>();
//	public List<uint> remove = new List<uint>();
//}

///////////////////////////////////////////

//class QueuedOperationFindPath
//{
//	public Vec3F start;
//	public Vec3F end;
//	public float stepSize;
//	public Vec3F polygonPickExtents;
//	public int maxPolygonPath;
//	public int maxSmoothPath;
//	public int maxSteerPoints;

//	public FindPath_InBackground_State state;
//}


//[Browsable( false )] //SodanKerjuu: controlled by the initialize toolbox form
//public bool DrawTileGrid
//{
//   get { return drawTileGrid; }
//   set { drawTileGrid = value; }
//}

//[DefaultValue( .5f )]
//public float GridHeight
//{
//   get { return gridHeight; }
//   set
//   {
//      gridHeight = value;
//      MathFunctions.Saturate( ref gridHeight );
//      ClearDebugGrids();
//   }
//}

//[Browsable( false )]
//public ICollection<TempObstacle> TempObstacles
//{
//	get { return tempObstacles.AsReadOnly(); }
//}


//[Browsable( false )]
//public bool IsInitialized
//{
//	get { return recastWorld != IntPtr.Zero; }
//}

////public void DestroyAllTiles()
////{
////   WaitForExecutionAllQueuedOperations();
////   lock( nativeLock )
////   {
////      if( recastWorld != IntPtr.Zero )
////      {
////         Wrapper.DestroyAllTiles( recastWorld );
////      }
////   }

////   //refresh debug mesh
////   debugNavigationMeshVertices = null;
////   debugNavigationMeshIndices = null;
////}

//public unsafe bool BuildNavMesh( out string error )
//{
//	//WaitForExecutionAllQueuedOperations();
//	//lock( nativeLock )
//	//{

//	if( recastWorld == IntPtr.Zero )
//	{
//		error = "Need to initialize the Recast world.";
//		return false;
//	}

//	//if( geometries.Count == 0 )
//	//{
//	//	error = "No collision objects are selected.";
//	//	return false;
//	//}

//	var collector = GetAllGeometriesForNavigationMesh();
//	Vec3[] vertices = collector.resultVertices;
//	int[] indices = collector.resultIndices;
//	int vertexCount = collector.resultVertexCount;
//	int indexCount = collector.resultIndexCount;

//	if( vertexCount == 0 )
//	{
//		error = "No vertices were gathered from collision objects.";
//		return false;
//	}

//	//convert to Recast space
//	var recastVertices = new Vec3F[ vertexCount ];
//	for( int n = 0; n < vertexCount; n++ )
//		recastVertices[ n ] = ToRecastVec3( vertices[ n ].ToVec3F() );

//	fixed ( Vec3F* pVertices = recastVertices )
//	fixed ( int* pIndices = indices )
//	{
//		Wrapper.SetGeometry( recastWorld, (IntPtr)pVertices, vertexCount, (IntPtr)pIndices, indexCount, TrianglesPerChunk );

//		//if( buildAllTiles )
//		Wrapper.BuildAllTiles( recastWorld );
//	}

//	////add temp obstacles
//	//{
//	//	const int maxBufferSizeToUpdate = 63;
//	//	int bufferLength = 0;

//	//	foreach( TempObstacle obstacle in tempObstacles )
//	//	{
//	//		Vec3 recastPosition = ToRecastVec3( obstacle.position );
//	//		uint id = Wrapper.AddTempObstacle( recastWorld, ref recastPosition, obstacle.radius, obstacle.height );

//	//		if( id != 0 )
//	//		{
//	//			//update TempObstacle
//	//			obstacle.nativeID = id;
//	//			obstacle.nativeCreated = true;

//	//			bufferLength++;
//	//			if( bufferLength >= maxBufferSizeToUpdate )
//	//			{
//	//				UpdateNavMesh();
//	//				bufferLength = 0;
//	//			}
//	//		}
//	//	}

//	//	if( bufferLength != 0 )
//	//	{
//	//		UpdateNavMesh();
//	//		bufferLength = 0;
//	//	}
//	//}
//	//}

//	//refresh debug mesh
//	debugNavigationMeshVertices = null;
//	//debugNavigationMeshIndices = null;

//	error = "";
//	return true;
//}

//public FindPath_InBackground_State FindPathInBackground( Vec3 start, Vec3 end, float stepSize, Vec3 polygonPickExtents,
//	int maxPolygonPath, int maxSmoothPath, int maxSteerPoints )
//{
//	if( recastWorld == IntPtr.Zero )
//	{
//		FindPath_InBackground_State state2 = new FindPath_InBackground_State();
//		state2.finished = true;
//		state2.pathFound = false;
//		state2.path = null;
//		return state2;
//	}

//	QueuedOperationFindPath oper = new QueuedOperationFindPath();
//	oper.start = start;
//	oper.end = end;
//	oper.stepSize = stepSize;
//	oper.polygonPickExtents = polygonPickExtents;
//	oper.maxPolygonPath = maxPolygonPath;
//	oper.maxSmoothPath = maxSmoothPath;
//	oper.maxSteerPoints = maxSteerPoints;

//	oper.state = new FindPath_InBackground_State();

//	lock( operationsLock )
//	{
//		FlushTempObstacleOperationDuringConstruction();

//		Task task = new Task( FindPathOperation_Function, oper );
//		AddOperation( task );
//	}

//	return oper.state;
//}

//public bool FindPathImmediately( Vec3 start, Vec3 end, float stepSize, Vec3 polygonPickExtents, int maxPolygonPath,
//	int maxSmoothPath, int maxSteerPoints, out Vec3[] outPath )
//{
//	if( recastWorld == IntPtr.Zero )
//	{
//		outPath = null;
//		return false;
//	}

//	FindPath_InBackground_State state = FindPathInBackground( start, end, stepSize, polygonPickExtents, maxPolygonPath,
//		maxSmoothPath, maxSteerPoints );
//	WaitForExecutionAllQueuedOperations();

//	if( !state.Finished )
//		Log.Fatal( "RecastNavigationSystem: FindPath_Immediately: !state.Finished." );

//	outPath = state.Path;
//	return state.PathFound;
//}


//public TempObstacle TempObstacle_Create( Vec3 position, float radius, float height )
//{
//	lock( operationsLock )
//	{
//		TempObstacle obstacle = new TempObstacle( this, position, radius, height );
//		tempObstacles.Add( obstacle );

//		if( recastWorld != IntPtr.Zero )
//		{
//			if( tempObstacleOperationDuringConstruction == null )
//				tempObstacleOperationDuringConstruction = new QueuedOperation_TempObstacle();

//			//add to the list
//			QueuedOperation_TempObstacle.AddData addData = new QueuedOperation_TempObstacle.AddData();
//			addData.position = position;
//			addData.radius = radius;
//			addData.height = height;
//			tempObstacleOperationDuringConstruction.add.Add( obstacle, addData );
//		}

//		debugNavigationMeshDirty = true;

//		return obstacle;
//	}
//}

//public void TempObstacle_Update( TempObstacle obstacle, Vec3 position, float radius, float height )
//{
//	if( obstacle.deleted )
//		Log.Fatal( "RecastNavigationSystem: TempObstacle_Update: The obstacle is already deleted." );

//	if( obstacle.position != position || obstacle.radius != radius || obstacle.height != height )
//	{
//		lock( operationsLock )
//		{
//			obstacle.position = position;
//			obstacle.radius = radius;
//			obstacle.height = height;

//			if( recastWorld != IntPtr.Zero )
//			{
//				if( tempObstacleOperationDuringConstruction == null )
//					tempObstacleOperationDuringConstruction = new QueuedOperation_TempObstacle();

//				//delete native object
//				if( obstacle.nativeCreated )
//				{
//					tempObstacleOperationDuringConstruction.remove.Add( obstacle.nativeID );
//					obstacle.nativeID = 0;
//					obstacle.nativeCreated = false;
//				}

//				//add to the list
//				QueuedOperation_TempObstacle.AddData addData;
//				if( tempObstacleOperationDuringConstruction.add.TryGetValue( obstacle, out addData ) )
//				{
//					addData.position = position;
//					addData.radius = radius;
//					addData.height = height;
//				}
//				else
//				{
//					addData = new QueuedOperation_TempObstacle.AddData();
//					addData.position = position;
//					addData.radius = radius;
//					addData.height = height;
//					tempObstacleOperationDuringConstruction.add.Add( obstacle, addData );
//				}
//			}

//			debugNavigationMeshDirty = true;
//		}
//	}
//}

//public void TempObstacle_Delete( TempObstacle obstacle )
//{
//	lock( operationsLock )
//	{
//		if( obstacle.deleted )
//			Log.Fatal( "RecastNavigationSystem: TempObstacle_Delete: The obstacle is already deleted." );

//		if( recastWorld != IntPtr.Zero )
//		{
//			if( tempObstacleOperationDuringConstruction == null )
//				tempObstacleOperationDuringConstruction = new QueuedOperation_TempObstacle();

//			//delete native object
//			if( obstacle.nativeCreated )
//			{
//				tempObstacleOperationDuringConstruction.remove.Add( obstacle.nativeID );
//				obstacle.nativeID = 0;
//				obstacle.nativeCreated = false;
//			}

//			//remove from the list of added
//			tempObstacleOperationDuringConstruction.add.Remove( obstacle );
//		}

//		obstacle.deleted = true;

//		debugNavigationMeshDirty = true;
//	}
//}

//void AddOperation( Task operation )
//{
//	operation.ContinueWith( ( a ) => OnOperationFinished() );

//	lock( operationsLock )
//	{
//		operations.Enqueue( operation );

//		//Log.Info( "operations on add: " + operations.Count.ToString() );

//		if( operations.Count == 1 )
//			operation.Start();
//	}
//}

//void OnOperationFinished()
//{
//	lock( operationsLock )
//	{
//		if( operations.Count != 0 )
//		{
//			operations.Dequeue();

//			//Log.Info( "operations on finish: " + operations.Count.ToString() );

//			if( operations.Count != 0 )
//			{
//				Task task = operations.Peek();
//				task.Start();
//			}
//		}
//	}
//}

//unsafe void FindPathOperation_Function( object data )
//{
//	//Log.Info( "find path function" );

//	QueuedOperationFindPath oper = (QueuedOperationFindPath)data;

//	//convert to Recast space
//	Vec3F recastStart = ToRecastVec3( oper.start );
//	Vec3F recastEnd = ToRecastVec3( oper.end );
//	Vec3F recastPolygonPickExtents = new Vec3F( oper.polygonPickExtents.X, oper.polygonPickExtents.Z, oper.polygonPickExtents.Y );

//	Vec3F* pathPointer;
//	int pathCount;
//	bool result;

//	lock( nativeLock )
//	{
//		result = Wrapper.FindPath( recastWorld, ref recastStart, ref recastEnd, oper.stepSize, ref recastPolygonPickExtents,
//			oper.maxPolygonPath, oper.maxSmoothPath, oper.maxSteerPoints, out pathPointer, out pathCount );
//	}

//	Vec3[] path = null;
//	if( result )
//	{
//		if( pathCount > 0 )
//		{
//			path = new Vec3[ pathCount ];
//			for( int n = 0; n < pathCount; n++ )
//				path[ n ] = ToEngineVec3( pathPointer[ n ] );
//		}
//		else
//		{
//			path = new Vec3[ 1 ];
//			path[ 0 ] = oper.start;
//		}

//		Wrapper.FreeMemory( (IntPtr)pathPointer );
//	}

//	//update state
//	oper.state.path = path;
//	oper.state.pathFound = result;
//	oper.state.finished = true;

//	//Log.Info( "find path function end: " + ( ( path != null ) ? path.Length.ToString() : "null" ) );
//}

//!!!!было
//void UpdateNavMesh()
//{
//	Wrapper.UpdateNavMesh( recastWorld );
//	debugNavigationMeshVertices = null;
//	//debugNavigationMeshDirty = true;
//}

//void TempObstacleOperation_Function( object data )
//{
//	QueuedOperation_TempObstacle oper = (QueuedOperation_TempObstacle)data;

//	const int maxBufferSizeToUpdate = 63;
//	int bufferLength = 0;

//	lock( nativeLock )
//	{
//		//remove
//		for( int n = 0; n < oper.remove.Count; n++ )
//		{
//			Status removingResult = (Status)Wrapper.RemoveTempObstacle( recastWorld, oper.remove[ n ] );
//			//!!!!!!!проверить
//			if( removingResult != Status.SUCCESS )
//				Log.Fatal( "RecastNavigationSystem: TempObstacleOperation_Function: removingResult != Status.SUCCESS" );

//			//Log.Info( "remove: " + oper.remove[ n ].ToString() );

//			bufferLength++;
//			if( bufferLength >= maxBufferSizeToUpdate )
//			{
//				UpdateNavMesh();
//				bufferLength = 0;
//			}
//		}

//		//add
//		foreach( KeyValuePair<TempObstacle, QueuedOperation_TempObstacle.AddData> pair in oper.add )
//		{
//			TempObstacle obstacle = pair.Key;
//			QueuedOperation_TempObstacle.AddData addData = pair.Value;

//			Vec3 recastPosition = ToRecastVec3( addData.position );
//			uint id = Wrapper.AddTempObstacle( recastWorld, ref recastPosition, addData.radius, addData.height );

//			//Log.Info( "add: " + id.ToString() );

//			if( id != 0 )
//			{
//				//update TempObstacle
//				obstacle.nativeID = id;
//				obstacle.nativeCreated = true;

//				bufferLength++;
//				if( bufferLength >= maxBufferSizeToUpdate )
//				{
//					UpdateNavMesh();
//					bufferLength = 0;
//				}
//			}
//			else
//			{
//				//!!!!!temp
//				Log.Info( "NOT created" );
//			}
//		}

//		if( bufferLength != 0 )
//		{
//			UpdateNavMesh();
//			bufferLength = 0;
//		}
//	}
//}

//void FlushTempObstacleOperationDuringConstruction()
//{
//	lock( operationsLock )
//	{
//		if( tempObstacleOperationDuringConstruction != null )
//		{
//			Task task = new Task( TempObstacleOperation_Function, tempObstacleOperationDuringConstruction );
//			AddOperation( task );

//			tempObstacleOperationDuringConstruction = null;
//		}
//	}
//}

//void WaitForExecutionAllQueuedOperations()
//{
//	//Log.Info( "wait for" );

//	FlushTempObstacleOperationDuringConstruction();

//	while( true )
//	{
//		bool wait = false;
//		lock( operationsLock )
//		{
//			if( operations.Count != 0 )
//				wait = true;
//		}
//		if( wait )
//			1 ?
//		  Thread.Sleep( 0 );//!!!!1?
//		else
//			break;
//	}
//}

//public int GetOperationsQueueLength()
//{
//	lock( operationsLock )
//	{
//		return operations.Count;
//	}
//}


//!!!!было
//void AddTempObstacleGeometry( Component_Pathfinding_Geometry geometry )
//{
//	Vec3F pos = geometry.Transform.Value.Position.ToVec3F();

//	//!!!!
//	Log.Info( "add temp: " + pos.ToString() );

//	//!!!!settings
//	var id = Wrapper.AddTempObstacle( recastWorld, ref pos, 3, 3 );
//	tempObstacles.Add( id );
//}

//!!!!было
//void AddTempObstacleGeometryTag( Component_Pathfinding_GeometryTag geometryTag )
//{
//	//_MeshInSpace
//	var meshInSpace = geometryTag.Parent as Component_MeshInSpace;
//	if( meshInSpace != null )
//	{
//		var mesh = meshInSpace.Mesh.Value;
//		if( mesh != null && mesh.Result != null )
//		{
//			//!!!!

//			//var transform = meshInSpace.Transform.Value.ToMat4();

//			//var vertices = new Vec3[ mesh.Result.ExtractedVerticesPositions.Length ];
//			//for( int n = 0; n < vertices.Length; n++ )
//			//	vertices[ n ] = transform * mesh.Result.ExtractedVerticesPositions[ n ].ToVec3();

//			Vec3F pos = meshInSpace.Transform.Value.Position.ToVec3F();

//			//!!!!
//			Log.Info( "add temp: " + pos.ToString() );

//			//!!!!settings
//			var id = Wrapper.AddTempObstacle( recastWorld, ref pos, 3, 3 );
//			tempObstacles.Add( id );
//		}
//	}

//	//!!!!
//}

//!!!!было
////!!!!пока так
//void TempObstaclesUpdate( bool forceUpdate )
//{
//	if( tempObstaclesNeedUpdate || forceUpdate )
//	{
//		TempObstaclesDestroy();

//		//!!!!
//		//Log.Info( "update" );

//		//!!!!пока так. slowly

//		var scene = FindParent<Component_Scene>();
//		if( scene != null )
//		{
//			foreach( var geometry in scene.GetComponents<Component_Pathfinding_Geometry>( false, true, true ) )
//			{
//				var type = geometry.Type.Value;
//				if( type == Component_Pathfinding_Geometry.TypeEnum.TemporaryObstacle )
//					AddTempObstacleGeometry( geometry );
//			}

//			//!!!!
//			//foreach( var geometryTag in scene.GetComponents<Component_Pathfinding_GeometryTag>( false, true, true ) )
//			//{
//			//	var type = geometryTag.Type.Value;
//			//	if( type == Component_Pathfinding_GeometryTag.TypeEnum.TemporaryObstacle )
//			//		AddTempObstacleGeometryTag( geometryTag );
//			//}
//		}

//		////!!!!
//		//Wrapper.BuildAllTiles( recastWorld );

//		UpdateNavMesh();

//		tempObstaclesNeedUpdate = false;
//	}
//}

//!!!!было
//void TempObstaclesDestroy()
//{
//	foreach( var id in tempObstacles )
//		Wrapper.RemoveTempObstacle( recastWorld, id );
//	tempObstacles.Clear();
//}


//	}
//}
