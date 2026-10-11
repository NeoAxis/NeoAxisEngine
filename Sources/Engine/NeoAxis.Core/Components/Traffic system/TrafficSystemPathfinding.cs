// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NeoAxis
{
	class TrafficSystemPathfinding
	{
		internal TrafficSystem owner;
		BackgroundThreadData backgroundThreadData;

		///////////////////////////////////////////////

		class Graph
		{
			public List<Island> islands = new List<Island>();
			public Dictionary<Road.LogicalData, Island> islandByRoad = new Dictionary<Road.LogicalData, Island>();

			/////////////////////

			public class Island
			{
				public ESet<Road.LogicalData> roads = new ESet<Road.LogicalData>();
				public ESet<RoadJunction.RoadJunctionLogicalData> junctions = new ESet<RoadJunction.RoadJunctionLogicalData>();

				//maybe place this data inside Roadjunction logical data class
				public Dictionary<RoadJunction.RoadJunctionLogicalData.ConnectedRoadItem, RoadJunction.RoadJunctionLogicalData.ConnectedRoadItem> junctionConnectionsAllowedToMove = new Dictionary<RoadJunction.RoadJunctionLogicalData.ConnectedRoadItem, RoadJunction.RoadJunctionLogicalData.ConnectedRoadItem>();
			}
		}

		///////////////////////////////////////////////

		class BackgroundThreadData
		{
			public TrafficSystemPathfinding owner;
			public TrafficSystem trafficSystem;
			public TrafficSystem.RoadsPrecompiledData precompiledData;

			public Queue<Command> commandQueue = new Queue<Command>();
			Thread commandThread;

			public Graph graph;

			//graph visualization
			public volatile GraphVisualizationArrow[] navigationGraphVertices;
			public volatile GraphVisualizationArrow[] navigationGraphVerticesPrevious;
			//public volatile Vector3[] navigationGraphVertices;
			//public volatile Vector3[] navigationGraphVerticesPrevious;

			////dynamic obstacles
			//public Dictionary<Component, long> dynamicObstacles = new Dictionary<Component, long>();
			//public volatile int dynamicObstaclesCountLockFree;

			////////////

			public struct GraphVisualizationArrow
			{
				public Vector3 From;
				public Vector3 To;
				public ColorValue Color;
			}

			////////////

			public BackgroundThreadData( TrafficSystemPathfinding owner, TrafficSystem.RoadsPrecompiledData precompiledData )
			{
				this.owner = owner;
				this.trafficSystem = owner.owner;
				this.precompiledData = precompiledData;
			}

			void WaitCommandToProcessFromMainThread( Command command )
			{
				while( !command.processed )
				{
					UpdateFromMainThread();
					Thread.Sleep( 0 );
				}
			}

			public bool CommandTypeIsInQueue( Type type )
			{
				lock( commandQueue )
				{
					foreach( var c in commandQueue )
					{
						if( c.GetType() == type )
							return true;
					}
				}
				return false;
			}

			public void AddCommandFromMainThread( Command command, bool wait, bool skipIfSameTypeCommandInQueue )
			{
				lock( commandQueue )
				{
					if( skipIfSameTypeCommandInQueue )
					{
						if( CommandTypeIsInQueue( command.GetType() ) )
							return;
					}

					commandQueue.Enqueue( command );
				}

				if( wait )
					WaitCommandToProcessFromMainThread( command );
			}

			void ThreadFunction( object param )
			{
				try
				{
					next:
					Command command = null;
					lock( commandQueue )
					{
						if( commandQueue.Count != 0 )
							command = commandQueue.Dequeue();
					}

					if( command != null )
					{
						command.PerformProcess();
						goto next;
					}
				}
				catch( Exception e )
				{
					Log.Warning( "TrafficSystem: Pathfinding: Exception in pathfinding background thread: " + e.ToString() );
				}
			}

			public bool CommandQueueIsEmpty
			{
				get
				{
					lock( commandQueue )
						return commandQueue.Count == 0;
				}
			}

			public void UpdateFromMainThread()
			{
				//process command queue
				lock( commandQueue )
				{
					if( commandThread != null && !commandThread.IsAlive )
						commandThread = null;

					if( commandQueue.Count != 0 && commandThread == null )
					{
						commandThread = new Thread( ThreadFunction );
						commandThread.IsBackground = true;
						commandThread.Start();
					}
				}
			}

			public void UpdateGraph()
			{
				if( precompiledData == null )
					return;

				if( graph == null )
				{
					try
					{
						graph = new Graph();

						//clear caches
						{
							foreach( var junction in precompiledData.Junctions )
							{
								foreach( var connectedRoadItem in junction.ConnectedRoads )
									connectedRoadItem.EnterToExitCurves = null;
							}
						}

						var waysToUse = new[] { RoadType.WayToUseEnum.Walking, RoadType.WayToUseEnum.Driving };
						foreach( var wayToUse in waysToUse )
						{
							var roadsList = wayToUse == RoadType.WayToUseEnum.Walking ? precompiledData.WalkingRoads : precompiledData.DrivingRoads;
							foreach( var startRoad in roadsList )
							{
								//check for already processed
								if( graph.islandByRoad.ContainsKey( startRoad ) )
									continue;

								//create new island
								var island = new Graph.Island();
								graph.islands.Add( island );

								//the list of roads to process
								var connectedRoadsToProcess = new ESet<Road.LogicalData>();
								connectedRoadsToProcess.Add( startRoad );

								while( connectedRoadsToProcess.Count > 0 )
								{
									//get road from the list to process and remove it from the list
									var road = connectedRoadsToProcess.First();
									connectedRoadsToProcess.Remove( road );

									//add the road to the island
									island.roads.AddWithCheckAlreadyContained( road );
									graph.islandByRoad[ road ] = island;

									//process connected roads
									foreach( var connectedNode in road.ConnectedNodes )
									{
										var junction = connectedNode as RoadJunction.RoadJunctionLogicalData;
										if( junction != null )
										{
											foreach( var connectedRoad in junction.ConnectedRoads )
											{
												//get same type of wayToUse roads
												if( connectedRoad.ConnectedRoad.RoadType.WayToUse == wayToUse )
												{
													//in the thread can't create logical data
													var connectedRoadLogicalData = connectedRoad.ConnectedRoad.Owner.GetLogicalData( false );
													if( connectedRoadLogicalData != null )
													{
														//add junction to the island when it connected with another road
														island.junctions.AddWithCheckAlreadyContained( junction );

														if( !island.roads.Contains( connectedRoadLogicalData ) )
															connectedRoadsToProcess.AddWithCheckAlreadyContained( connectedRoadLogicalData );
													}
												}
											}
										}
									}
								}
							}
						}

						//calculate junction connections allowed to move
						foreach( var island in graph.islands )
						{
							foreach( var junction in island.junctions )
							{
								foreach( var connectedRoadFrom in junction.ConnectedRoads )
								{
									var existLanes =
										!connectedRoadFrom.ForwardDirection && connectedRoadFrom.ConnectedRoad.LanesForward > 0 ||
										connectedRoadFrom.ForwardDirection && connectedRoadFrom.ConnectedRoad.LanesBackward > 0;
									if( existLanes )
									{
										var roadLogicalData = connectedRoadFrom.ConnectedRoad.Owner.GetLogicalData( false );
										if( roadLogicalData != null )
										{
											var firstJunctionItem = roadLogicalData.FindFirstJunctionFrom( connectedRoadFrom.TimeOnCurve, !connectedRoadFrom.ForwardDirection, connectedRoadFrom );

											if( firstJunctionItem != null )
												island.junctionConnectionsAllowedToMove[ connectedRoadFrom ] = firstJunctionItem;
										}
									}
								}
							}
						}
					}
					catch( Exception e )
					{
						Log.Warning( "TrafficSystem: Pathfinding: Exception in UpdateGraph: " + e.ToString() );
						return;
					}
					finally { }
				}
			}
		}

		/////////////////////////////////////////

		abstract class Command
		{
			public BackgroundThreadData owner;
			public volatile bool processed;

			protected abstract void Process();

			public void PerformProcess()
			{
				Process();
				processed = true;
			}
		}

		/////////////////////////////////////////

		//internal class DynamicGeometriesToUpdateItem
		//{
		//	public bool Add;

		//	public Box? Box;
		//	public Cylinder? Cylinder;
		//	public bool Walkable;
		//}

		/////////////////////////////////////////

		//class CommandForceUpdate : Command
		//{
		//	protected override void Process()
		//	{
		//		owner.UpdateGraph();
		//	}
		//}

		/////////////////////////////////////////

		//class CommandUpdateDynamicObstacles : Command
		//{
		//	public KeyValuePair<Component, DynamicGeometriesToUpdateItem>[] geometries;

		//	//

		//	protected override void Process()
		//	{
		//		var dynamicNavMesh = owner.dynamicNavMesh;
		//		if( dynamicNavMesh != null )
		//		{
		//			var flagMergeThreshold = dynamicNavMesh.config.walkableClimb;

		//			foreach( var pair in geometries )
		//			{
		//				var geometry = pair.Key;
		//				var item = pair.Value;

		//				//remove
		//				{
		//					if( owner.dynamicObstacles.TryGetValue( geometry, out var id ) )
		//					{
		//						dynamicNavMesh.RemoveCollider( id );
		//						owner.dynamicObstacles.Remove( geometry );
		//					}
		//				}

		//				//add
		//				if( item.Add )
		//				{
		//					var area = item.Walkable ? areaWalkable : areaNotWalkable;

		//					IDtCollider collider = null;

		//					if( item.Box != null )
		//					{
		//						var box = item.Box.Value;

		//						var center = ConvertToDotRecastCoordinates( box.Center );
		//						var up = ConvertToDotRecastCoordinates( box.Axis.Item2 );
		//						var forward = ConvertToDotRecastCoordinates( box.Axis.Item1 );
		//						var extent = ConvertToDotRecastCoordinates( box.Extents, true );

		//						collider = new DtBoxCollider( center, DtBoxCollider.GetHalfEdges( up, forward, extent ), area, flagMergeThreshold );
		//					}
		//					else if( item.Cylinder != null )
		//					{
		//						var cylinder = item.Cylinder.Value;

		//						var start = ConvertToDotRecastCoordinates( cylinder.Point1 );
		//						var end = ConvertToDotRecastCoordinates( cylinder.Point2 );

		//						collider = new DtCylinderCollider( start, end, (float)cylinder.Radius, area, flagMergeThreshold );
		//					}

		//					if( collider != null )
		//						owner.dynamicObstacles[ geometry ] = dynamicNavMesh.AddCollider( collider );
		//				}
		//			}

		//			owner.dynamicObstaclesCountLockFree = owner.dynamicObstacles.Count;

		//			if( dynamicNavMesh.Update() )
		//			{
		//				owner.navMesh = dynamicNavMesh.NavMesh();
		//				owner.freeNavMeshQueries.Clear();
		//			}

		//			owner.navigationMeshVertices = null;
		//		}
		//	}
		//}

		/////////////////////////////////////////

		class CommandFindPath : Command
		{
			public TrafficSystem.FindPathContext[] contexts;

			/////////////////////

			protected override void Process()
			{
				//update graph
				if( owner.graph == null )
					owner.UpdateGraph();

				//find paths
				var graph = owner.graph;
				if( graph != null )
				{
					ParallelOptions parallelOptions;
					if( owner.trafficSystem.PathfindingParallelCount > 0 )
						parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = owner.trafficSystem.PathfindingParallelCount };
					else
						parallelOptions = new ParallelOptions();

					Parallel.ForEach( contexts, parallelOptions, delegate ( TrafficSystem.FindPathContext context )
					{
						//!!!!maybe save list in the memory:
						//DtNavMeshQuery query = null;
						//lock( owner.freeNavMeshQueries )
						//{
						//	if( owner.freeNavMeshQueries.Count != 0 )
						//		query = owner.freeNavMeshQueries.Dequeue();
						//	else
						//		query = new DtNavMeshQuery( owner.navMesh );
						//}

						try
						{
							//get start road logical data
							var startRoad = context.StartRoad.GetLogicalData( false );
							if( startRoad == null )
							{
								context.Error = "Start road logical data is null.";
								return;
							}

							//get end road logical data
							var endRoad = context.EndRoad.GetLogicalData( false );
							if( endRoad == null )
							{
								context.Error = "End road logical data is null.";
								return;
							}

							//get island
							if( !graph.islandByRoad.TryGetValue( startRoad, out var island ) )
							{
								context.Error = "Start road is not in any island.";
								return;
							}

							//check that start and end roads are in the same island
							if( !graph.islandByRoad.TryGetValue( endRoad, out var island2 ) )
							{
								context.Error = "End road is not in any island.";
								return;
							}
							if( island != island2 )
							{
								context.Error = "Start road and end road are in different islands.";
								return;
							}

							var startLaneForwardDirection = startRoad.RoadData.IsForwardLane( context.StartLane );
							var endLaneForwardDirection = endRoad.RoadData.IsForwardLane( context.EndLane );

							//find first junction
							var firstJunctionItem = startRoad.FindFirstJunctionFrom( context.StartTimeOnCurve, startLaneForwardDirection, null );

							//check for same start and end road and no junctions between them
							if( startRoad == endRoad && startLaneForwardDirection == endLaneForwardDirection )
							{
								if( startLaneForwardDirection )
								{
									if( context.EndTimeOnCurve > context.StartTimeOnCurve && ( firstJunctionItem == null || context.EndTimeOnCurve < firstJunctionItem.TimeOnCurve ) )
									{
										context.Path = new TrafficSystem.FindPathContext.PathItem[]
										{
											new TrafficSystem.FindPathContext.PathItem() { StartPoint = true },
											new TrafficSystem.FindPathContext.PathItem() { EndPoint = true }
										};
										//context.Path = Array.Empty<TrafficSystem.FindPathContext.PathItem>();
										return;
									}
								}
								else
								{
									if( context.EndTimeOnCurve < context.StartTimeOnCurve && ( firstJunctionItem == null || context.EndTimeOnCurve > firstJunctionItem.TimeOnCurve ) )
									{
										context.Path = new TrafficSystem.FindPathContext.PathItem[]
										{
											new TrafficSystem.FindPathContext.PathItem() { StartPoint = true },
											new TrafficSystem.FindPathContext.PathItem() { EndPoint = true }
										};
										//context.Path = Array.Empty<TrafficSystem.FindPathContext.PathItem>();
										return;
									}
								}
							}

							if( firstJunctionItem == null )
							{
								context.Error = "No junction ahead.";
								return;
							}

							//!!!!не учитывается если хотим заехать на перекресток

							//find end junction which need to reach
							var endJunctionItem = endRoad.FindEndJunctionItem( context.EndTimeOnCurve, endLaneForwardDirection );
							if( endJunctionItem == null )
							{
								context.Error = "There is no road to get there.";
								return;
							}

							//later:
							//add road item to path when need change lane. right now no code for change lanes


							//find path from start junction to end junction

							var bestPath = new List<TrafficSystem.FindPathContext.PathItem>( 32 );
							var bestPathLength = double.MaxValue;

							var currentPath = new List<TrafficSystem.FindPathContext.PathItem>( 32 );
							var currentPathLength = 0.0;

							var visitedEnterJunctions = new HashSet<RoadJunction.RoadJunctionLogicalData.ConnectedRoadItem>( 32 );

							var levelLimitReached = false;
							var stepCount = 0;

							void FindRecursiveEnterJunction( RoadJunction.RoadJunctionLogicalData.ConnectedRoadItem enter, int level )
							{
								//check limits
								if( level >= context.LimitLevel )
								{
									levelLimitReached = true;
									return;
								}
								stepCount++;
								if( stepCount >= context.LimitSteps )
									return;

								//skip already visited enters
								if( visitedEnterJunctions.Contains( enter ) )
									return;
								//add to visited
								visitedEnterJunctions.Add( enter );

								//get world position of the enter junction
								enter.ConnectedRoad.GetPositionByTime( enter.TimeOnCurve, out var enterPosition );

								//add enter to current path
								currentPath.Add( new TrafficSystem.FindPathContext.PathItem() { EnterToJunction = enter } );

								//get exits from current junction
								var exits = enter.Owner.GetExits( enter );
								for( int nExit = 0; nExit < exits.Count; nExit++ )
								{
									var exit = exits[ nExit ];

									//get world position of the exit junction
									exit.ConnectedRoad.GetPositionByTime( enter.TimeOnCurve, out var exitPosition );
									//!!!!not best way to calculate length. need to use road data
									//calculate length from enter to exit
									var length = ( exitPosition - enterPosition ).Length();

									//check for already bigger path length than best path length
									if( currentPathLength + length < bestPathLength )
									{
										//add exit to current path
										currentPath.Add( new TrafficSystem.FindPathContext.PathItem() { ExitFromJunction = exit } );
										currentPathLength += length;

										//check found end junction
										var found = endJunctionItem == exit && endLaneForwardDirection != exit.ForwardDirection;
										if( found )
										{
											var currentPathLength2 = currentPathLength;

											//add distance from exit to end point on the road
											currentPathLength2 += ( context.EndPosition - exitPosition ).Length();

											//replace best path if current path is shorter
											if( currentPathLength2 < bestPathLength )
											{
												bestPath.Clear();
												bestPath.AddRange( currentPath );
												bestPathLength = currentPathLength2;
											}
										}

										//find enter junction item on the road
										if( island.junctionConnectionsAllowedToMove.TryGetValue( exit, out var moveTo ) )
										{
											FindRecursiveEnterJunction( moveTo, level + 1 );
											if( stepCount >= context.LimitSteps )
												return;
										}

										//remove last item from current path
										currentPath.RemoveAt( currentPath.Count - 1 );
										currentPathLength -= length;
									}
								}

								//remove enter from current path
								currentPath.RemoveAt( currentPath.Count - 1 );

								//remove from visited
								visitedEnterJunctions.Remove( enter );
							}

							//start recursive search
							currentPath.Add( new TrafficSystem.FindPathContext.PathItem() { StartPoint = true } );
							FindRecursiveEnterJunction( firstJunctionItem, 0 );

							if( bestPath.Count > 1 )
							{
								bestPath.Add( new TrafficSystem.FindPathContext.PathItem() { EndPoint = true } );

								context.Path = bestPath.ToArray();
								return;
							}

							if( stepCount >= context.LimitSteps )
							{
								context.Error = "Limit steps exceeded.";
								return;
							}
							if( levelLimitReached )
							{
								context.Error = "Limit level exceeded.";
								return;
							}

							context.Error = "Unknown reason.";
							return;

						}
						catch( Exception e )
						{
							context.Error = "Exception: " + e.Message;
						}
						finally
						{
							//lock( owner.freeNavMeshQueries )
							//	owner.freeNavMeshQueries.Enqueue( query );
						}
					} );
				}
				else
				{
					foreach( var context in contexts )
						context.Error = "No navigation graph";
				}

				foreach( var context in contexts )
					context.finished = true;
			}
		}

		/////////////////////////////////////////

		class CommandGetNavigationGraphVertices : Command
		{
			protected override void Process()
			{
				//update graph
				if( owner.graph == null )
					owner.UpdateGraph();

				//update only when command queue is empty
				if( owner.navigationGraphVertices == null && owner.CommandQueueIsEmpty )
				{
					var graph = owner.graph;
					if( graph != null )
					{
						var result = new List<BackgroundThreadData.GraphVisualizationArrow>();
						//var result = new List<Vector3>( 1024 );

						foreach( var island in graph.islands )
						{
							//junctions
							foreach( var junction in island.junctions )
							{
								var position = junction.Owner.TransformV.Position;

								result.Add( new BackgroundThreadData.GraphVisualizationArrow()
								{
									From = position,
									To = position + new Vector3( 0, 0, 3 ),
									Color = new ColorValue( 0, 0, 1 )
								} );

								foreach( var connectedRoad in junction.ConnectedRoads )
								{
									connectedRoad.ConnectedRoad.GetPositionByTime( connectedRoad.TimeOnCurve, out var connectionPosition );

									result.Add( new BackgroundThreadData.GraphVisualizationArrow()
									{
										From = position,
										To = connectionPosition,
										Color = new ColorValue( 0, 1, 0 )
									} );
								}
							}

							//junction connections
							foreach( var pair in island.junctionConnectionsAllowedToMove )
							{
								var from = pair.Key;
								var to = pair.Value;

								from.ConnectedRoad.GetPositionByTime( from.TimeOnCurve, out var positionFrom );
								to.ConnectedRoad.GetPositionByTime( to.TimeOnCurve, out var positionTo );

								var left = Vector3.Cross( ( positionTo - positionFrom ).GetNormalize(), new Vector3( 0, 0, 1 ) ).GetNormalize();
								var positionFrom2 = positionFrom + left * 0.2;
								var positionTo2 = positionTo + left * 0.2;

								result.Add( new BackgroundThreadData.GraphVisualizationArrow()
								{
									From = positionFrom2,
									To = positionTo2,
									Color = new ColorValue( 1, 1, 0 )
								} );
							}
						}

						var resultArray = result.ToArray();
						owner.navigationGraphVerticesPrevious = resultArray;
						owner.navigationGraphVertices = resultArray;
					}
				}
			}
		}

		///////////////////////////////////////////

		//OnGetRenderSceneData?
		internal void Scene_RenderEvent( Scene sender, Viewport viewport )
		{
			var context2 = viewport.RenderingContext.ObjectInSpaceRenderingContext;

			if( owner.PathfindingShowGraph )
				DrawGraph( viewport );
		}

		public void DrawGraph( Viewport viewport )
		{
			var backgroundThreadData = GetBackgroundThreadData();
			if( backgroundThreadData != null )
			{
				//no data, need to get it
				if( backgroundThreadData.navigationGraphVertices == null )
					NeedGetNavigationGraphVertices( false );

				var vertices = backgroundThreadData.navigationGraphVertices ?? backgroundThreadData.navigationGraphVerticesPrevious;
				if( vertices != null && vertices.Length > 0 )
				{
					const double thickness = 0.1;
					var alpha = 0.5f;

					var renderer = viewport.Simple3DRenderer;

					for( int n = 0; n < vertices.Length; n++ )
					{
						var arrow = vertices[ n ];

						var color = arrow.Color;
						color.Alpha = alpha;

						renderer.SetColor( color, false );
						renderer.AddArrow( arrow.From, arrow.To, thickness * 6, thickness * 2, true, thickness );
					}
				}
			}
		}

		BackgroundThreadData GetBackgroundThreadData()
		{
			var precompiledData = owner.GetRoadsPrecompiledData( false );

			if( precompiledData != null )
			{
				if( backgroundThreadData == null || backgroundThreadData.precompiledData != precompiledData )
					backgroundThreadData = new BackgroundThreadData( this, precompiledData );
			}
			else
				backgroundThreadData = null;

			return backgroundThreadData;
		}

		internal void OnUpdate( float delta )
		{

			////update dynamic obstacles
			//dynamicGeometriesUpdateRemainingTime -= delta;
			//if( dynamicGeometriesUpdateRemainingTime < 0 )
			//{
			//	dynamicGeometriesUpdateRemainingTime = DynamicObstaclesUpdatePeriod.Value;

			//	lock( dynamicGeometriesToUpdate )
			//	{
			//		if( dynamicGeometriesToUpdate.Count > 0 )
			//		{
			//			if( !CommandTypeIsInQueue( typeof( CommandUpdateDynamicObstacles ) ) )
			//			{
			//				var command = new CommandUpdateDynamicObstacles();
			//				command.geometries = dynamicGeometriesToUpdate.ToArray();
			//				AddCommand( command, false, false );

			//				dynamicGeometriesToUpdate.Clear();
			//			}
			//		}
			//	}
			//}
			//}

			var backgroundThreadData = GetBackgroundThreadData();
			backgroundThreadData?.UpdateFromMainThread();
		}

		//bool CommandTypeIsInQueue( Type type )
		//{
		//	var backgroundThreadData = GetBackgroundThreadData();
		//	if( backgroundThreadData != null )
		//		return backgroundThreadData.CommandTypeIsInQueue( type );
		//	return false;
		//}

		void AddCommand( Command command, bool wait, bool skipIfSameTypeCommandInQueue )
		{
			var backgroundThreadData = GetBackgroundThreadData();
			if( backgroundThreadData == null )
				return;

			command.owner = backgroundThreadData;

			backgroundThreadData.AddCommandFromMainThread( command, wait, skipIfSameTypeCommandInQueue );
		}

		public void FindPath( TrafficSystem.FindPathContext[] contexts, bool wait )
		{
			//need roads precompiled data to find paths. it building not immediately
			owner.NeedHaveRoadsPrecompiledData();

			var command = new CommandFindPath();
			command.contexts = contexts;
			AddCommand( command, wait, false );
		}

		void NeedGetNavigationGraphVertices( bool wait )
		{
			//don't add when the command is already in the queue
			var command = new CommandGetNavigationGraphVertices();
			AddCommand( command, wait, true );
		}

		//public void StartFullUpdate()
		//{
		//	owner.GetRoadsPrecompiledData();

		//	//if( precompiledData == null )
		//	if( !BuildPrecompiledData( out var error ) )
		//		return;

		//	var command = new CommandForceUpdate();
		//	AddCommand( command, false, true );

		//	////add dynamic obstacles update commands for all dynamic geometries
		//	//var scene = ParentScene;
		//	//if( scene != null )
		//	//{
		//	//	foreach( var geometry in scene.GetComponents<PathfindingGeometry>( false, true, true ) )
		//	//	{
		//	//		if( geometry.Dynamic )
		//	//		{
		//	//			var add = true;
		//	//			FilterGeometry?.Invoke( this, geometry, ref add );
		//	//			if( add )
		//	//				geometry.DynamicMode_UpdatePathfindingComponents( this );
		//	//		}
		//	//	}

		//	//	foreach( var geometryTag in scene.GetComponents<PathfindingGeometryTag>( false, true, true ) )
		//	//	{
		//	//		if( geometryTag.Dynamic )
		//	//		{
		//	//			var add = true;
		//	//			FilterGeometry?.Invoke( this, geometryTag, ref add );
		//	//			if( add )
		//	//				geometryTag.DynamicMode_UpdatePathfindingComponents( this );
		//	//		}
		//	//	}
		//	//}
		//}

		//internal static void UpdateDynamicGeometry( Component geometry, Scene scene, DynamicGeometriesToUpdateItem data, Pathfinding specifiedPathfinding )
		//{
		//	if( scene == null )
		//		return;

		//	var instances = Instances;
		//	for( int n = 0; n < instances.Count; n++ )
		//	{
		//		var pathfinding = instances[ n ];

		//		if( scene == pathfinding.ParentScene && ( specifiedPathfinding == null || pathfinding == specifiedPathfinding ) )
		//			pathfinding.OnUpdatePathfindingGeometry( geometry, data );
		//	}
		//}

		//internal void OnUpdatePathfindingGeometry( Component geometry, DynamicGeometriesToUpdateItem data )
		//{
		//	var add = true;
		//	FilterGeometry?.Invoke( this, geometry, ref add );
		//	if( add )
		//	{
		//		lock( dynamicGeometriesToUpdate )
		//			dynamicGeometriesToUpdate[ geometry ] = data;
		//	}
		//}


		enum FindPathIntervalType
		{
			Road,
			Junction,
		}

		public IList<Vector3> FindPathCalculatePositions( TrafficSystem.FindPathContext context, double roadMinimalDistanceBetweenPoints, double junctionMinimalDistanceBetweenPoints )
		{
			var roadMinimalDistanceBetweenPointsSquared = roadMinimalDistanceBetweenPoints * roadMinimalDistanceBetweenPoints;
			var junctionMinimalDistanceBetweenPointsSquared = junctionMinimalDistanceBetweenPoints * junctionMinimalDistanceBetweenPoints;

			//!!!!перестроение когда нет junction. так же когда будут более сложные junctions
			var startLane = context.StartLane;

			//calculate start lane offset from center of the road
			int startLaneOffset = 0;
			var startLaneForwardDirection = false;
			{
				var roadData = context.StartRoad.GetRoadData();
				if( roadData != null )
				{
					startLaneOffset = roadData.GetLaneOffsetFromCenterOfRoad( startLane );
					startLaneForwardDirection = roadData.IsForwardLane( startLane );
				}
			}

			//!!!!GC
			var result = new List<Vector3>( 64 );
			var lastPosition = Vector3.Zero;


			for( int nPathItem = 0; nPathItem < context.Path.Length - 1; nPathItem++ )
			{
				var pathItemFrom = context.Path[ nPathItem ];
				var pathItemTo = context.Path[ nPathItem + 1 ];


				FindPathIntervalType intervalType;

				//for road interval type
				Road.RoadData roadData = null;
				double startTimeOnCurve = 0;
				double endTimeOnCurve = 0;
				bool forwardDirection = false;

				//for junction interval type
				RoadJunction.RoadJunctionLogicalData.ConnectedRoadItem enter = null;
				RoadJunction.RoadJunctionLogicalData.ConnectedRoadItem exit = null;

				if( pathItemFrom.StartPoint )
				{
					intervalType = FindPathIntervalType.Road;
					roadData = context.StartRoad.GetRoadData();
					startTimeOnCurve = context.StartTimeOnCurve;
					forwardDirection = roadData.IsForwardLane( startLane );
				}
				else if( pathItemFrom.ExitFromJunction != null )
				{
					intervalType = FindPathIntervalType.Road;
					roadData = pathItemFrom.ExitFromJunction.ConnectedRoad;
					startTimeOnCurve = pathItemFrom.ExitFromJunction.TimeOnCurve;
					forwardDirection = !pathItemFrom.ExitFromJunction.ForwardDirection;
				}
				else if( pathItemFrom.EnterToJunction != null )
				{
					intervalType = FindPathIntervalType.Junction;
					enter = pathItemFrom.EnterToJunction;
				}
				else
					continue;

				if( pathItemTo.EndPoint )
				{
					roadData = context.EndRoad.GetRoadData();
					endTimeOnCurve = context.EndTimeOnCurve;
				}
				else if( pathItemTo.EnterToJunction != null )
					endTimeOnCurve = pathItemTo.EnterToJunction.TimeOnCurve;
				else if( pathItemTo.ExitFromJunction != null )
					exit = pathItemTo.ExitFromJunction;
				else
					continue;

				if( intervalType == FindPathIntervalType.Road )
				{
					//road interval type

					//check for invalid data
					if( roadData == null )
						continue;

					//select lane. using basic lane or closest possible
					var lane = roadData.GetLaneFromLaneOffsetWithClamp( startLaneOffset, forwardDirection );
					if( pathItemTo.EndPoint )
						lane = context.EndLane;

					var laneCurves = roadData.GetLaneCurves();
					if( lane >= 0 && lane < laneCurves.Length )
					{
						var laneCurve = laneCurves[ lane ];

						if( startTimeOnCurve < endTimeOnCurve )
						{
							var timeRange = new Range( startTimeOnCurve, endTimeOnCurve );
							if( laneCurve.GetRangeByTimeRange( timeRange, out var range ) )
							{
								var curve = laneCurve.Curve;
								for( int n = range.Minimum; n <= range.Maximum; n++ )
								{
									var item = curve[ n ];

									lastPosition = item.Position;

									if( result.Count != 0 && n != 0 && n != range.Maximum )
									{
										var distanceSquared = ( result[ result.Count - 1 ] - item.Position ).LengthSquared();
										if( distanceSquared < roadMinimalDistanceBetweenPointsSquared )
											continue;
									}
									result.Add( item.Position );
								}
							}
						}
						else
						{
							var timeRange = new Range( endTimeOnCurve, startTimeOnCurve );
							if( laneCurve.GetRangeByTimeRange( timeRange, out var range ) )
							{
								var curve = laneCurve.Curve;
								for( int n = range.Maximum; n >= range.Minimum; n-- )
								{
									var item = curve[ n ];

									lastPosition = item.Position;

									if( result.Count != 0 && n != 0 && n != range.Minimum )
									{
										var distanceSquared = ( result[ result.Count - 1 ] - item.Position ).LengthSquared();
										if( distanceSquared < roadMinimalDistanceBetweenPointsSquared )
											continue;
									}
									result.Add( item.Position );
								}
							}
						}
					}
				}
				else if( intervalType == FindPathIntervalType.Junction )
				{
					//junction interval type

					if( enter == null || exit == null )
						continue;
					if( result.Count < 2 )
						continue;

					//!!!!right?
					var enterLine = enter.ConnectedRoad.GetLaneFromLaneOffsetWithClamp( startLaneOffset, enter.ForwardDirection );

					var exitLane = exit.ConnectedRoad.GetLaneFromLaneOffsetWithClamp( startLaneOffset, !exit.ForwardDirection );
					if( nPathItem >= context.Path.Length - 3 )
						exitLane = context.EndLane;

					//get item from the cache
					var cacheCurveItem = enter.GetEnterToExitCurveItem( exit, enterLine, exitLane );

					//calculate curve when it not in the cache
					if( cacheCurveItem.Curve == null )
					{
						var enterPosition = result[ result.Count - 1 ];
						var enterPosition2 = result[ result.Count - 2 ];
						var enterDirection = ( enterPosition2 - enterPosition ).GetNormalize();

						Vector3 exitPosition = Vector3.Zero;
						Vector3 exitPosition2 = Vector3.Zero;
						var exitPositionFound = false;

						var laneCurves = exit.ConnectedRoad.GetLaneCurves();
						if( exitLane >= 0 && exitLane < laneCurves.Length )
						{
							var laneCurve = laneCurves[ exitLane ];

							if( laneCurve.GetIndexByTime( exit.TimeOnCurve, out var index ) )
							{
								exitPosition = laneCurve.Curve[ index ].Position;

								for( int n = 0; n < 10; n++ )
								{
									if( index + n < laneCurve.Curve.Length )
										exitPosition2 = laneCurve.Curve[ index + n ].Position;
									else if( index - n >= 0 )
										exitPosition2 = laneCurve.Curve[ index - n ].Position;

									if( ( exitPosition - exitPosition2 ).LengthSquared() > 1 )
									{
										exitPositionFound = true;
										break;
									}
								}
							}
						}

						if( exitPositionFound )
						{
							var exitDirection = ( exitPosition2 - exitPosition ).GetNormalize();

							var enterPosition2D = enterPosition.ToVector2();
							var enterDirection2D = enterDirection.ToVector2();
							var exitPosition2D = exitPosition.ToVector2();
							var exitDirection2D = exitDirection.ToVector2();

							if( MathAlgorithms.IntersectRayRay( enterPosition2D, enterPosition2D + enterDirection2D, exitPosition2D, exitPosition2D + exitDirection2D, out Vector2 intersectionPoint2D ) )
							{
								var intersectionPoint = new Vector3( intersectionPoint2D, ( enterPosition.Z + exitPosition.Z ) * 0.5 );

								var enterDistance2D = ( intersectionPoint2D - enterPosition2D ).Length();
								var exitDistance2D = ( intersectionPoint2D - exitPosition2D ).Length();


								//!!!!not best solution


								//!!!!calculate
								//var requiredMediumPointsDistance = 10000.0;

								//!!!!
								//var enterMediumPointDistance = Math.Min( requiredMediumPointsDistance, enterDistance2D - 0.01 );
								//var exitMediumPointDistance = Math.Min( requiredMediumPointsDistance, exitDistance2D - 0.01 );

								var curve = new CurveBezier();
								curve.AddPoint( new Curve.Point( 0, enterPosition ) );
								//curve.AddPoint( new Curve.Point( 0.25, intersectionPoint + enterDirection * enterMediumPointDistance ) );
								curve.AddPoint( new Curve.Point( 0.5, intersectionPoint ) );
								//curve.AddPoint( new Curve.Point( 0.75, intersectionPoint + exitDirection * exitMediumPointDistance ) );
								curve.AddPoint( new Curve.Point( 1, exitPosition ) );

								var length = enterDistance2D + exitDistance2D;
								var step = 1.0 / length;

								var curvePoints = new List<(double Time, Vector3 Position)>();
								var timeEnd = curve.Points[ curve.Points.Count - 1 ].Time + step * 0.5;
								for( var time = 0.0; time <= timeEnd; time += step )
									curvePoints.Add( (time, curve.CalculateValueByTime( time )) );

								cacheCurveItem.Curve = curvePoints.ToArray();
							}
							else
								cacheCurveItem.Curve = Array.Empty<(double Time, Vector3 Position)>();
						}
						else
							cacheCurveItem.Curve = Array.Empty<(double Time, Vector3 Position)>();
					}

					//add to result
					for( int n = 0; n < cacheCurveItem.Curve.Length; n++ )
					{
						var curvePoint = cacheCurveItem.Curve[ n ];

						if( result.Count != 0 && n != 0 && n != cacheCurveItem.Curve.Length - 1 )
						{
							var distanceSquared = ( result[ result.Count - 1 ] - curvePoint.Position ).LengthSquared();
							if( distanceSquared < junctionMinimalDistanceBetweenPointsSquared )
								continue;
						}

						result.Add( curvePoint.Position );
					}
				}
			}

			if( result.Count > 0 && result[ result.Count - 1 ] != lastPosition )
				result.Add( lastPosition );

			return result;
		}

	}
}
