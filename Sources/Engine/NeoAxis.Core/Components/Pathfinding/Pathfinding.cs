// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using Internal.DotRecast.Core;
using Internal.DotRecast.Core.Numerics;
using Internal.DotRecast.Detour;
using Internal.DotRecast.Detour.Dynamic;
using Internal.DotRecast.Detour.Dynamic.Colliders;
using Internal.DotRecast.Detour.Dynamic.Io;
using Internal.DotRecast.Recast;
using Internal.DotRecast.Recast.Geom;
using Internal.DotRecast.Recast.Toolset;
using Internal.DotRecast.Recast.Toolset.Builder;
using Internal.DotRecast.Recast.Toolset.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NeoAxis
{
	/// <summary>
	/// Navigation mesh pathfinding.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Pathfinding\Pathfinding", 540 )]
	[Editor.SettingsCell( typeof( Editor.PathfindingSettingsCell ) )]
	public class Pathfinding : ObjectInSpace
	{
		static List<Pathfinding> instances = new List<Pathfinding>();
		static Pathfinding[] instancesAsArray = Array.Empty<Pathfinding>();

		PrecompiledDataClass precompiledData;
		BackgroundThreadData backgroundThreadData;

		bool firstOnUpdateAfterEnabledInHierarchy;

		const int areaWalkable = SampleAreaModifications.SAMPLE_POLYAREA_TYPE_WALKABLE;
		const int areaNotWalkable = 0;
		Dictionary<Component, DynamicGeometriesToUpdateItem> dynamicGeometriesToUpdate = new Dictionary<Component, DynamicGeometriesToUpdateItem>();

		double dynamicGeometriesUpdateRemainingTime;
		//double autoUpdateEditorRemainingTime;

		/////////////////////////////////////////

		/// <summary>
		/// The width and depth resolution used when sampling the source geometry. The width and depth of the voxels in voxel fields. The width and depth of the cell columns that make up voxel fields. A lower value allows for the generated meshes to more closely match the source geometry, but at a higher processing and memory cost.
		/// </summary>
		[DefaultValue( 0.3 )]
		[Category( "Grid" )]
		public Reference<double> CellSize
		{
			get { if( _cellSize.BeginGet() ) CellSize = _cellSize.Get( this ); return _cellSize.value; }
			set
			{
				if( value < 0.01 )
					value = new Reference<double>( 0.01, value.GetByReference );
				if( _cellSize.BeginSet( this, ref value ) ) { try { CellSizeChanged?.Invoke( this ); } finally { _cellSize.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="CellSize"/> property value changes.</summary>
		public event Action<Pathfinding> CellSizeChanged;
		ReferenceField<double> _cellSize = 0.3;

		/// <summary>
		/// The height resolution used when sampling the source geometry. The height of the voxels in voxel fields.
		/// </summary>
		[DefaultValue( 0.2 )]
		[Category( "Grid" )]
		public Reference<double> CellHeight
		{
			get { if( _cellHeight.BeginGet() ) CellHeight = _cellHeight.Get( this ); return _cellHeight.value; }
			set
			{
				if( value < 0.01 )
					value = new Reference<double>( 0.01, value.GetByReference );
				if( _cellHeight.BeginSet( this, ref value ) ) { try { CellHeightChanged?.Invoke( this ); } finally { _cellHeight.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="CellHeight"/> property value changes.</summary>
		public event Action<Pathfinding> CellHeightChanged;
		ReferenceField<double> _cellHeight = 0.2;

		/// <summary>
		/// The amount of cells in the tile by one axis.
		/// </summary>
		[Category( "Grid" )]
		[DefaultValue( 32 )]
		public Reference<int> TileSize
		{
			get { if( _tileSize.BeginGet() ) TileSize = _tileSize.Get( this ); return _tileSize.value; }
			set
			{
				if( value < 1 )
					value = new Reference<int>( 1, value.GetByReference );
				if( _tileSize.BeginSet( this, ref value ) ) { try { TileSizeChanged?.Invoke( this ); } finally { _tileSize.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="TileSize"/> property value changes.</summary>
		public event Action<Pathfinding> TileSizeChanged;
		ReferenceField<int> _tileSize = 32;

		/// <summary>
		/// The minimum region size for unconnected (island) regions. The value is in voxels. Regions that are not connected to any other region and are smaller than this size will be culled before mesh generation. I.e. They will no longer be considered traversable.
		/// </summary>
		[DefaultValue( 8 )]
		[Category( "Regions" )]
		public Reference<int> MinRegionSize
		{
			get { if( _minRegionSize.BeginGet() ) MinRegionSize = _minRegionSize.Get( this ); return _minRegionSize.value; }
			set
			{
				if( value < 1 )
					value = new Reference<int>( 1, value.GetByReference );
				if( _minRegionSize.BeginSet( this, ref value ) ) { try { MinRegionSizeChanged?.Invoke( this ); } finally { _minRegionSize.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="MinRegionSize"/> property value changes.</summary>
		public event Action<Pathfinding> MinRegionSizeChanged;
		ReferenceField<int> _minRegionSize = 8;

		/// <summary>
		/// Any regions smaller than this size will, if possible, be merged with larger regions. Value is in voxels. Helps reduce the number of small regions. This is especially an issue in diagonal path regions where inherent faults in the region generation algorithm can result in unnecessarily small regions.
		/// </summary>
		[DefaultValue( 20 )]
		[Category( "Regions" )]
		public Reference<int> MergedRegionSize
		{
			get { if( _mergedRegionSize.BeginGet() ) MergedRegionSize = _mergedRegionSize.Get( this ); return _mergedRegionSize.value; }
			set
			{
				if( value < 0 )
					value = new Reference<int>( 0, value.GetByReference );
				if( _mergedRegionSize.BeginSet( this, ref value ) ) { try { MergedRegionSizeChanged?.Invoke( this ); } finally { _mergedRegionSize.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="MergedRegionSize"/> property value changes.</summary>
		public event Action<Pathfinding> MergedRegionSizeChanged;
		ReferenceField<int> _mergedRegionSize = 20;

		/// <summary>
		/// The maximum length of polygon edges that represent the border of meshes. More vertices will be added to border edges if this value is exceeded for a particular edge. A value of zero will disable this feature.
		/// </summary>
		[DefaultValue( 12 )]
		[Category( "Polygonization" )]
		public Reference<int> EdgeMaxLength
		{
			get { if( _edgeMaxLength.BeginGet() ) EdgeMaxLength = _edgeMaxLength.Get( this ); return _edgeMaxLength.value; }
			set
			{
				if( value < 0 )
					value = new Reference<int>( 0, value.GetByReference );
				if( _edgeMaxLength.BeginSet( this, ref value ) ) { try { EdgeMaxLengthChanged?.Invoke( this ); } finally { _edgeMaxLength.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="EdgeMaxLength"/> property value changes.</summary>
		public event Action<Pathfinding> EdgeMaxLengthChanged;
		ReferenceField<int> _edgeMaxLength = 12;

		/// <summary>
		/// The maximum distance the edges of meshes may deviate from the source geometry. A lower value will result in mesh edges following the xy-plane geometry contour more accurately at the expense of an increased triangle count.
		/// </summary>
		[DefaultValue( 1.3 )]
		[Category( "Polygonization" )]
		public Reference<double> EdgeMaxError
		{
			get { if( _edgeMaxError.BeginGet() ) EdgeMaxError = _edgeMaxError.Get( this ); return _edgeMaxError.value; }
			set
			{
				if( value < 0.1 )
					value = new Reference<double>( 0.1, value.GetByReference );
				if( _edgeMaxError.BeginSet( this, ref value ) ) { try { EdgeMaxErrorChanged?.Invoke( this ); } finally { _edgeMaxError.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="EdgeMaxError"/> property value changes.</summary>
		public event Action<Pathfinding> EdgeMaxErrorChanged;
		ReferenceField<double> _edgeMaxError = 1.3;

		//[DefaultValue( 6 )]
		//[Category( "Polygonization" )]
		//[Range( 3, 6 )]
		//public Reference<int> MaxVerticesPerPolygon
		//{
		//	get { if( _maxVerticesPerPolygon.BeginGet() ) MaxVerticesPerPolygon = _maxVerticesPerPolygon.Get( this ); return _maxVerticesPerPolygon.value; }
		//	set
		//	{
		//		if( value < 3 )
		//			value = new Reference<int>( 3, value.GetByReference );
		//		if( _maxVerticesPerPolygon.BeginSet( this, ref value ) ) { try { MaxVerticesPerPolygonChanged?.Invoke( this ); } finally { _maxVerticesPerPolygon.EndSet(); } }
		//	}
		//}
		///// <summary>Occurs when the <see cref="MaxVerticesPerPolygon"/> property value changes.</summary>
		//public event Action<Pathfinding> MaxVerticesPerPolygonChanged;
		//ReferenceField<int> _maxVerticesPerPolygon = 6;

		/// <summary>
		/// Sets the sampling distance to use when matching the detail mesh to the surface of the original geometry. Impacts how well the final detail mesh conforms to the surface contour of the original geometry. Higher values result in a detail mesh which conforms more closely to the original geometry's surface at the cost of a higher final triangle count and higher processing cost.
		/// </summary>
		[DefaultValue( 6 )]
		[Category( "Detail Mesh" )]
		public Reference<double> DetailSampleDistance
		{
			get { if( _detailSampleDistance.BeginGet() ) DetailSampleDistance = _detailSampleDistance.Get( this ); return _detailSampleDistance.value; }
			set
			{
				if( value < 0.0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _detailSampleDistance.BeginSet( this, ref value ) ) { try { DetailSampleDistanceChanged?.Invoke( this ); } finally { _detailSampleDistance.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="DetailSampleDistance"/> property value changes.</summary>
		public event Action<Pathfinding> DetailSampleDistanceChanged;
		ReferenceField<double> _detailSampleDistance = 6;

		/// <summary>
		/// The maximum distance the surface of the detail mesh may deviate from the surface of the original geometry.
		/// </summary>
		[DefaultValue( 1 )]
		[Category( "Detail Mesh" )]
		public Reference<double> DetailMaxSampleError
		{
			get { if( _detailMaxSampleError.BeginGet() ) DetailMaxSampleError = _detailMaxSampleError.Get( this ); return _detailMaxSampleError.value; }
			set
			{
				if( value < 0.0 )
					value = new Reference<double>( 0, value.GetByReference );
				if( _detailMaxSampleError.BeginSet( this, ref value ) ) { try { DetailMaxSampleErrorChanged?.Invoke( this ); } finally { _detailMaxSampleError.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="DetailMaxSampleError"/> property value changes.</summary>
		public event Action<Pathfinding> DetailMaxSampleErrorChanged;
		ReferenceField<double> _detailMaxSampleError = 1;

		/// <summary>
		/// Minimum height where the agent can still walk.
		/// </summary>
		[DefaultValue( 2.0 )]
		[Category( "Agent" )]
		public Reference<double> AgentHeight
		{
			get { if( _agentHeight.BeginGet() ) AgentHeight = _agentHeight.Get( this ); return _agentHeight.value; }
			set
			{
				if( value < 0.1 )
					value = new Reference<double>( 0.1, value.GetByReference );
				if( _agentHeight.BeginSet( this, ref value ) ) { try { AgentHeightChanged?.Invoke( this ); } finally { _agentHeight.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="AgentHeight"/> property value changes.</summary>
		public event Action<Pathfinding> AgentHeightChanged;
		ReferenceField<double> _agentHeight = 2.0;

		/// <summary>
		/// Radius of the agent.
		/// </summary>
		[DefaultValue( 0.6 )]
		[Category( "Agent" )]
		public Reference<double> AgentRadius
		{
			get { if( _agentRadius.BeginGet() ) AgentRadius = _agentRadius.Get( this ); return _agentRadius.value; }
			set
			{
				if( value < 0.0 )
					value = new Reference<double>( 0.0, value.GetByReference );
				if( _agentRadius.BeginSet( this, ref value ) ) { try { AgentRadiusChanged?.Invoke( this ); } finally { _agentRadius.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="AgentRadius"/> property value changes.</summary>
		public event Action<Pathfinding> AgentRadiusChanged;
		ReferenceField<double> _agentRadius = 0.6;

		/// <summary>
		/// Maximum height between grid cells the agent can climb.
		/// </summary>
		[DefaultValue( 0.9 )]
		[Category( "Agent" )]
		public Reference<double> AgentMaxClimb
		{
			get { if( _agentMaxClimb.BeginGet() ) AgentMaxClimb = _agentMaxClimb.Get( this ); return _agentMaxClimb.value; }
			set
			{
				if( value < 0.001 )
					value = new Reference<double>( 0.001, value.GetByReference );
				if( _agentMaxClimb.BeginSet( this, ref value ) ) { try { AgentMaxClimbChanged?.Invoke( this ); } finally { _agentMaxClimb.EndSet(); } }
			}
		}
		/// <summary>Occurs when the <see cref="AgentMaxClimb"/> property value changes.</summary>
		public event Action<Pathfinding> AgentMaxClimbChanged;
		ReferenceField<double> _agentMaxClimb = 0.9;

		/// <summary>
		/// Maximum walkable slope angle in degrees.
		/// </summary>
		[DefaultValue( "45" )]
		[Category( "Agent" )]
		[Range( 1, 89 )]
		public Reference<Degree> AgentMaxSlope
		{
			get { if( _agentMaxSlope.BeginGet() ) AgentMaxSlope = _agentMaxSlope.Get( this ); return _agentMaxSlope.value; }
			set
			{
				if( value.Value < 1 )
					value = new Reference<Degree>( 1, value.GetByReference );
				if( value.Value > 89 )
					value = new Reference<Degree>( 89, value.GetByReference );
				if( _agentMaxSlope.BeginSet( this, ref value ) ) { try { AgentMaxSlopeChanged?.Invoke( this ); } finally { _agentMaxSlope.EndSet(); } }
			}
		}
		public event Action<Pathfinding> AgentMaxSlopeChanged;
		ReferenceField<Degree> _agentMaxSlope = new Degree( 45 );

		/// <summary>
		/// The period in seconds between updates of dynamic obstacles.
		/// </summary>
		[DefaultValue( 1.0 )]
		[Category( "Dynamic Obstacles" )]
		public Reference<double> DynamicObstaclesUpdatePeriod
		{
			get { if( _dynamicObstaclesUpdatePeriod.BeginGet() ) DynamicObstaclesUpdatePeriod = _dynamicObstaclesUpdatePeriod.Get( this ); return _dynamicObstaclesUpdatePeriod.value; }
			set { if( _dynamicObstaclesUpdatePeriod.BeginSet( this, ref value ) ) { try { DynamicObstaclesUpdatePeriodChanged?.Invoke( this ); } finally { _dynamicObstaclesUpdatePeriod.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DynamicObstaclesUpdatePeriod"/> property value changes.</summary>
		public event Action<Pathfinding> DynamicObstaclesUpdatePeriodChanged;
		ReferenceField<double> _dynamicObstaclesUpdatePeriod = 1.0;

		/// <summary>
		/// The maximum number of pathfinding processes to run in parallel. Set zero for automatically use the number of processor cores.
		/// </summary>
		[DefaultValue( 4 )]
		[Category( "Pathfinding" )]
		public Reference<int> PathfindingParallelCount
		{
			get { if( _pathfindingParallelCount.BeginGet() ) PathfindingParallelCount = _pathfindingParallelCount.Get( this ); return _pathfindingParallelCount.value; }
			set { if( _pathfindingParallelCount.BeginSet( this, ref value ) ) { try { PathfindingParallelCountChanged?.Invoke( this ); } finally { _pathfindingParallelCount.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PathfindingParallelCount"/> property value changes.</summary>
		public event Action<Pathfinding> PathfindingParallelCountChanged;
		ReferenceField<int> _pathfindingParallelCount = 4;

		/// <summary>
		/// Whether to visualize the input geometry used for navigation mesh generation.
		/// </summary>
		[Category( "Visualization" )]
		[DefaultValue( false )]
		public Reference<bool> ShowInputMesh
		{
			get { if( _showInputMesh.BeginGet() ) ShowInputMesh = _showInputMesh.Get( this ); return _showInputMesh.value; }
			set { if( _showInputMesh.BeginSet( this, ref value ) ) { try { ShowInputMeshChanged?.Invoke( this ); } finally { _showInputMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShowInputMesh"/> property value changes.</summary>
		public event Action<Pathfinding> ShowInputMeshChanged;
		ReferenceField<bool> _showInputMesh = false;

		/// <summary>
		/// Whether to visualize the navigation mesh.
		/// </summary>
		[Category( "Visualization" )]
		[DefaultValue( false )]
		public Reference<bool> ShowNavMesh
		{
			get { if( _showNavMesh.BeginGet() ) ShowNavMesh = _showNavMesh.Get( this ); return _showNavMesh.value; }
			set { if( _showNavMesh.BeginSet( this, ref value ) ) { try { ShowNavMeshChanged?.Invoke( this ); } finally { _showNavMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShowNavMesh"/> property value changes.</summary>
		public event Action<Pathfinding> ShowNavMeshChanged;
		ReferenceField<bool> _showNavMesh = false;

		/// <summary>
		/// Whether to visualize the navigation mesh when the object is selected in the editor.
		/// </summary>
		[Category( "Visualization" )]
		[DefaultValue( true )]
		public Reference<bool> ShowNavMeshWhenSelected
		{
			get { if( _showNavMeshWhenSelected.BeginGet() ) ShowNavMeshWhenSelected = _showNavMeshWhenSelected.Get( this ); return _showNavMeshWhenSelected.value; }
			set { if( _showNavMeshWhenSelected.BeginSet( this, ref value ) ) { try { ShowNavMeshWhenSelectedChanged?.Invoke( this ); } finally { _showNavMeshWhenSelected.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="ShowNavMeshWhenSelected"/> property value changes.</summary>
		public event Action<Pathfinding> ShowNavMeshWhenSelectedChanged;
		ReferenceField<bool> _showNavMeshWhenSelected = true;

		//work with frezees
		///// <summary>
		///// The period in seconds between updates of the navigation mesh in the editor. Set zero to disable automatic updates.
		///// </summary>
		//[DefaultValue( 1.0 )]
		//public Reference<double> AutoUpdatePeriodEditor
		//{
		//	get { if( _autoUpdatePeriodEditor.BeginGet() ) AutoUpdatePeriodEditor = _autoUpdatePeriodEditor.Get( this ); return _autoUpdatePeriodEditor.value; }
		//	set { if( _autoUpdatePeriodEditor.BeginSet( this, ref value ) ) { try { AutoUpdatePeriodEditorChanged?.Invoke( this ); } finally { _autoUpdatePeriodEditor.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="AutoUpdatePeriodEditor"/> property value changes.</summary>
		//public event Action<Pathfinding> AutoUpdatePeriodEditorChanged;
		//ReferenceField<double> _autoUpdatePeriodEditor = 1.0;

		/////////////////////////////////////////

		public delegate void FilterGeometryDelegate( Pathfinding sender, Component geometryOrGeometryTag, ref bool add );
		public event FilterGeometryDelegate FilterGeometry;

		/////////////////////////////////////////

		[Browsable( false )]
		internal PrecompiledDataClass PrecompiledData
		{
			get { return precompiledData; }
			set
			{
				if( precompiledData == value )
					return;
				precompiledData = value;

				if( precompiledData != null )
				{
					var settings = new RcNavMeshBuildSettings();

					settings.cellSize = (float)CellSize;
					settings.cellHeight = (float)CellHeight;

					settings.agentHeight = (float)AgentHeight;
					settings.agentRadius = (float)AgentRadius;
					settings.agentMaxClimb = (float)AgentMaxClimb;
					settings.agentMaxSlope = (float)AgentMaxSlope.Value;

					//!!!!
					//public float agentMaxAcceleration = 8.0f;
					//public float agentMaxSpeed = 3.5f;

					settings.minRegionSize = MinRegionSize;
					settings.mergedRegionSize = MergedRegionSize;

					//!!!!
					//public int partitioning = RcPartitionType.WATERSHED.Value;

					//!!!!
					//public bool filterLowHangingObstacles = true;
					//public bool filterLedgeSpans = true;
					//public bool filterWalkableLowHeightSpans = true;

					settings.edgeMaxLen = EdgeMaxLength;
					settings.edgeMaxError = (float)EdgeMaxError;
					//settings.vertsPerPoly = MaxVerticesPerPolygon;

					settings.detailSampleDist = (float)DetailSampleDistance;
					settings.detailSampleMaxError = (float)DetailMaxSampleError;

					settings.tiled = true;
					settings.tileSize = TileSize;

					settings.keepInterResults = false;
					//public bool buildAll = true;

					precompiledData.buildSettings = settings;
				}
			}
		}

		/////////////////////////////////////////

		internal class PrecompiledDataClass
		{
			internal StaticGeometry staticGeometry;

			//!!!!
			//!!!!struct?
			//internal class MeshOffLinkConnection
			//{
			//	public Vector3 Position1;
			//	public Vector3 Position2;
			//	//!!!!
			//}
			//internal List<MeshOffLinkConnection> meshOffLinkConnections = new List<MeshOffLinkConnection>();

			internal RcNavMeshBuildSettings buildSettings = new RcNavMeshBuildSettings();
		}

		/////////////////////////////////////////

		//not work
		public class ConvexVolume
		{
			public List<Vector3> Vertices = new List<Vector3>(); //public Vector3[] Vertices;
			public double HeightMin;
			public double HeightMax;

			//!!!!walkable on the volume
		}

		/////////////////////////////////////////

		internal class BackgroundThreadData
		{
			public Pathfinding pathfinding;
			public PrecompiledDataClass precompiledData;

			public Queue<Command> commandQueue = new Queue<Command>();
			Thread commandThread;

			public DtDynamicNavMesh dynamicNavMesh;
			public DtNavMesh navMesh;
			public Queue<DtNavMeshQuery> freeNavMeshQueries = new Queue<DtNavMeshQuery>();

			//separate by tiles?
			//!!!!indices?
			public volatile Vector3[] navigationMeshVertices;
			public volatile Vector3[] navigationMeshVerticesPrevious;

			//dynamic obstacles
			public Dictionary<Component, long> dynamicObstacles = new Dictionary<Component, long>();
			public volatile int dynamicObstaclesCountLockFree;

			////////////

			public BackgroundThreadData( Pathfinding pathfinding, PrecompiledDataClass precompiledData )
			{
				this.pathfinding = pathfinding;
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
						//var type = command.GetType();
						//foreach( var c in commandQueue )
						//{
						//	if( c.GetType() == type )
						//		return;
						//}
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
					Log.Warning( "Exception in pathfinding background thread: " + e.ToString() );
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

			public void UpdateNavMesh()
			{
				if( precompiledData == null )
					return;

				if( navMesh == null )
				{
					try
					{
						var settings = precompiledData.buildSettings;
						var staticGeometry = precompiledData.staticGeometry;

						Bounds bounds = Bounds.Cleared;
						{
							if( staticGeometry != null )
								bounds.Add( staticGeometry.bounds );
						}

						if( !bounds.IsCleared() )
						{
							var vertices = new List<Vector3>( 2048 );
							var indices = new List<int>( 2048 );

							//rasterize static geometry
							if( staticGeometry != null && !staticGeometry.IsEmpty )
							{
								var areaData = staticGeometry.data[ areaWalkable ];
								if( areaData != null )
								{
									var startVertexIndex = vertices.Count;
									vertices.AddRange( areaData.vertices.Data );
									for( int n = 0; n < areaData.indices.Count; n++ )
										indices.Add( startVertexIndex + areaData.indices[ n ] );
								}
							}

							var floatVertices = new float[ vertices.Count * 3 ];
							for( int n = 0; n < vertices.Count; n++ )
							{
								var v = ConvertToDotRecastCoordinates( vertices[ n ] );
								floatVertices[ n * 3 + 0 ] = v.X;
								floatVertices[ n * 3 + 1 ] = v.Y;
								floatVertices[ n * 3 + 2 ] = v.Z;
							}

							//add triangles
							var geometryProvider = new RcSampleInputGeomProvider( floatVertices, indices.ToArray() );

							//!!!!not work
							////add convex volumes
							//foreach( var sourceVolume in staticGeometry.convexVolumes )
							//{
							//	var volume = new RcConvexVolume();

							//	//var vv = sourceVolume.Vertices.GetReverse().ToArray();
							//	//volume.verts = new float[ sourceVolume.Vertices.Count * 3 ];
							//	//for( int n = 0; n < vv.Length; n++ )
							//	//{
							//	//	var v = ConvertToDotRecastCoordinates( vv[ n ] );
							//	//	volume.verts[ n * 3 + 0 ] = v.X;
							//	//	volume.verts[ n * 3 + 1 ] = v.Y;
							//	//	volume.verts[ n * 3 + 2 ] = v.Z;
							//	//}

							//	volume.verts = new float[ sourceVolume.Vertices.Count * 3 ];
							//	for( int n = 0; n < sourceVolume.Vertices.Count; n++ )
							//	{
							//		var v = ConvertToDotRecastCoordinates( sourceVolume.Vertices[ n ] );
							//		volume.verts[ n * 3 + 0 ] = v.X;
							//		volume.verts[ n * 3 + 1 ] = v.Y;
							//		volume.verts[ n * 3 + 2 ] = v.Z;
							//	}

							//	volume.hmin = (float)sourceVolume.HeightMin;
							//	volume.hmax = (float)sourceVolume.HeightMax;

							//	//!!!!
							//	//volume.areaMod = new RcAreaModification( 42 );// RcAreaModification.RC_AREA_FLAGS_MASK );

							//	geometryProvider.AddConvexVolume( volume );
							//}

							var walkableRadius = (int)MathF.Ceiling( settings.agentRadius / settings.cellSize );
							var cfg = new RcConfig(
								true, settings.tileSize, settings.tileSize,
								walkableRadius + 3,
								RcPartitionType.OfValue( settings.partitioning ),
								settings.cellSize, settings.cellHeight,
								settings.agentMaxSlope, settings.agentHeight, settings.agentRadius, settings.agentMaxClimb,
								(int)RcMath.Sqr( settings.minRegionSize ), (int)RcMath.Sqr( settings.mergedRegionSize ),
								(int)( settings.edgeMaxLen / settings.cellSize ), settings.edgeMaxError,
								settings.vertsPerPoly,
								settings.detailSampleDist, settings.detailSampleMaxError,
								true, true, true,
								SampleAreaModifications.SAMPLE_AREAMOD_WALKABLE, true );

							var rcBuilder = new RcBuilder();
							var results = rcBuilder.BuildTiles( geometryProvider, cfg, true, true );

							var voxelFile = DtVoxelFile.From( cfg, results );
							var dynaMesh = new DtDynamicNavMesh( voxelFile );
							dynaMesh.config.minRegionArea = 0;

							if( staticGeometry != null )
							{
								for( int area = 0; area < staticGeometry.data.Length; area++ )
								{
									if( area == areaWalkable )
										continue;

									var areaData = staticGeometry.data[ area ];
									if( areaData == null || areaData.indices.Count == 0 )
										continue;

									var areaVertices = new float[ areaData.vertices.Count * 3 ];
									for( int n = 0; n < areaData.vertices.Count; n++ )
									{
										var v = ConvertToDotRecastCoordinates( areaData.vertices[ n ] );
										areaVertices[ n * 3 + 0 ] = v.X;
										areaVertices[ n * 3 + 1 ] = v.Y;
										areaVertices[ n * 3 + 2 ] = v.Z;
									}

									var areaIndices = new int[ areaData.indices.Count ];
									for( int n = 0; n < areaData.indices.Count; n++ )
										areaIndices[ n ] = areaData.indices[ n ];

									dynaMesh.AddCollider( new DtTrimeshCollider( areaVertices, areaIndices, area, dynaMesh.config.walkableClimb ) );
								}
							}

							dynaMesh.Build();

							dynamicNavMesh = dynaMesh;
							navMesh = dynaMesh.NavMesh();
						}
					}
					catch//( Exception e )
					{
						//!!!!
						//error = e.Message;
						return;// false;
					}
					finally { }
				}
			}
		}

		/////////////////////////////////////////

		internal abstract class Command
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

		/// <summary>
		/// The data to execution finding path.
		/// </summary>
		public class FindPathContext
		{
			public Vector3 Start;
			public Vector3 End;

			public double StepSize = 0.5;
			public double Slop = 0.01;
			public Vector3 PolygonPickExtents = new Vector3( 2, 2, 2 );

			public int MaxPolygonPath = 512;
			public int MaxSmoothPath = 2048;
			//public int MaxSteerPoints = 16;

			public bool Finished;
			public bool Partial;
			public PathPoint[] Path;
			public string Error = string.Empty;

			//

			public struct PathPoint
			{
				public Vector3 Position;
				public bool Turn;
			}
		}

		/////////////////////////////////////////

		internal class DynamicGeometriesToUpdateItem
		{
			public bool Add;

			public Box? Box;
			public Cylinder? Cylinder;
			public bool Walkable;
		}

		/////////////////////////////////////////

		class CommandForceUpdate : Command
		{
			protected override void Process()
			{
				owner.UpdateNavMesh();
			}
		}

		/////////////////////////////////////////

		class CommandUpdateDynamicObstacles : Command
		{
			public KeyValuePair<Component, DynamicGeometriesToUpdateItem>[] geometries;

			//

			protected override void Process()
			{
				var dynamicNavMesh = owner.dynamicNavMesh;
				if( dynamicNavMesh != null )
				{
					var flagMergeThreshold = dynamicNavMesh.config.walkableClimb;

					foreach( var pair in geometries )
					{
						var geometry = pair.Key;
						var item = pair.Value;

						//remove
						{
							if( owner.dynamicObstacles.TryGetValue( geometry, out var id ) )
							{
								dynamicNavMesh.RemoveCollider( id );
								owner.dynamicObstacles.Remove( geometry );
							}
						}

						//add
						if( item.Add )
						{
							var area = item.Walkable ? areaWalkable : areaNotWalkable;

							IDtCollider collider = null;

							if( item.Box != null )
							{
								var box = item.Box.Value;

								var center = ConvertToDotRecastCoordinates( box.Center );
								var up = ConvertToDotRecastCoordinates( box.Axis.Item2 );
								var forward = ConvertToDotRecastCoordinates( box.Axis.Item1 );
								var extent = ConvertToDotRecastCoordinates( box.Extents, true );

								collider = new DtBoxCollider( center, DtBoxCollider.GetHalfEdges( up, forward, extent ), area, flagMergeThreshold );
							}
							else if( item.Cylinder != null )
							{
								var cylinder = item.Cylinder.Value;

								var start = ConvertToDotRecastCoordinates( cylinder.Point1 );
								var end = ConvertToDotRecastCoordinates( cylinder.Point2 );

								collider = new DtCylinderCollider( start, end, (float)cylinder.Radius, area, flagMergeThreshold );
							}

							if( collider != null )
								owner.dynamicObstacles[ geometry ] = dynamicNavMesh.AddCollider( collider );
						}
					}

					owner.dynamicObstaclesCountLockFree = owner.dynamicObstacles.Count;

					if( dynamicNavMesh.Update() )
					{
						owner.navMesh = dynamicNavMesh.NavMesh();
						owner.freeNavMeshQueries.Clear();
					}

					owner.navigationMeshVertices = null;
				}
			}
		}

		/////////////////////////////////////////

		class CommandFindPath : Command
		{
			public FindPathContext[] contexts;

			//

			protected override void Process()
			{
				//owner.UpdateNavMesh();

				if( owner.navMesh != null )
				{
					ParallelOptions parallelOptions;
					if( owner.pathfinding.PathfindingParallelCount > 0 )
						parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = owner.pathfinding.PathfindingParallelCount };
					else
						parallelOptions = new ParallelOptions();

					Parallel.ForEach( contexts, parallelOptions, delegate ( FindPathContext context )
					{
						DtNavMeshQuery query = null;
						lock( owner.freeNavMeshQueries )
						{
							if( owner.freeNavMeshQueries.Count != 0 )
								query = owner.freeNavMeshQueries.Dequeue();
							else
								query = new DtNavMeshQuery( owner.navMesh );
						}

						try
						{
							var halfExtents = ConvertToDotRecastCoordinates( context.PolygonPickExtents * 0.5, true );
							var startPos = ConvertToDotRecastCoordinates( context.Start );
							var endPos = ConvertToDotRecastCoordinates( context.End );

							var filter = new DtQueryDefaultFilter();
							//filter.SetIncludeFlags( 0xffff );
							//filter.SetExcludeFlags( 0 );

							query.FindNearestPoly( startPos, halfExtents, filter, out var startRef, out var startNearestPt, out _ );
							query.FindNearestPoly( endPos, halfExtents, filter, out var endRef, out var endNearestPt, out _ );

							var polys = new long[ context.MaxPolygonPath ];
							var smoothPath = new RcVec3f[ context.MaxSmoothPath ];

							var enableRaycast = true;

							var tool = new RcTestNavMeshTool();
							var status = tool.FindFollowPath( owner.navMesh, query, startRef, endRef, startPos, endPos, filter, enableRaycast, polys, out var polysCount, smoothPath, out var smoothPathSize, (float)context.StepSize, (float)context.Slop );

							if( status.Succeeded() )
							{
								context.Path = new FindPathContext.PathPoint[ smoothPathSize ];
								for( int n = 0; n < smoothPathSize; n++ )
								{
									ref var p = ref context.Path[ n ];
									p.Position = ConvertToEngineCoordinates( smoothPath[ n ] );
								}

								var stepSize = context.StepSize;

								for( int n = 1; n < smoothPathSize - 1; n++ )
								{
									ref var p = ref context.Path[ n ];

									var previous = context.Path[ n - 1 ].Position.ToVector2();
									var next = context.Path[ n + 1 ].Position.ToVector2();
									var p2 = p.Position.ToVector2();

									var projected = MathAlgorithms.ProjectPointToLine( previous, next, p2 );

									var lengthSquared = ( projected - p2 ).LengthSquared();
									p.Turn = lengthSquared > ( stepSize * 0.01 ) * ( stepSize * 0.01 );
								}
							}



							////var pathResult = new Vector3[ smoothPathSize ];
							////for( int n = 0; n < smoothPathSize; n++ )
							////	pathResult[ n ] = ConvertToEngineCoordinates( smoothPath[ n ] );


							////var path = new long[ context.MaxPolygonPath ];
							////query.FindPath( startRef, endRef, startPos, endPos, filter, path, out var pathCount, 256 );

							////var options = 0;

							////var straightPath = new DtStraightPath[ 256 ];

							////if( true )
							////{

							////	//var status = query.FindStraightPath( startPos, endPos, path, 256, straightPath, out var straightPathCount, 256, options );

							////	//pathResult = new Vector3[ straightPathCount ];
							////	//for( int n = 0; n < straightPathCount; n++ )
							////	//	pathResult[ n ] = ConvertToEngineCoordinates( straightPath[ n ].pos );


							////	//var polys = new long[ context.MaxPolygonPath ];

							////	//query.FindPath( startRef, endRef, startPos, endPos, filter, polys, out var polysCount, polys.Length );

							////	//if( polysCount > 0 )
							////	//{

							////	//	// Iterate over the path to find smooth path on the detail mesh surface.
							////	//	query.ClosestPointOnPoly( startRef, startPos, out var iterPos, out var _ );
							////	//	query.ClosestPointOnPoly( polys[ polysCount - 1 ], endPos, out var targetPos, out var _ );

							////	//	var pathResult = new Vector3[ polysCount ];
							////	//	for( int n = 0; n < polysCount; n++ )
							////	//		pathResult[ n ] = ConvertToEngineCoordinates( polys[ n ].pos );

							////	//	context.Path = pathResult;
							////	//}
							////}
							////else

						}
						catch( Exception e )
						{
							context.Error = e.Message;
						}
						finally
						{
							lock( owner.freeNavMeshQueries )
								owner.freeNavMeshQueries.Enqueue( query );
						}
					} );
				}
				else
				{
					foreach( var context in contexts )
						context.Error = "No navigation mesh";
				}

				foreach( var context in contexts )
					context.Finished = true;
			}
		}

		/////////////////////////////////////////

		class CommandGetNavigationMeshVertices : Command
		{
			protected override void Process()
			{
				//update only when command queue is empty
				if( owner.navigationMeshVertices == null && owner.CommandQueueIsEmpty )
				{
					//owner.UpdateNavMesh();

					if( owner.navMesh != null )
					{
						//!!!!separate by tiles?
						//!!!!indices?

						var result = new List<Vector3>( 1024 );

						var navMesh = owner.navMesh;

						for( int nTile = 0; nTile < navMesh.GetMaxTiles(); nTile++ )
						{
							var tile = navMesh.GetTile( nTile );
							if( tile == null )
								continue;

							var data = tile.data;
							if( data == null )
								continue;
							var vertices = data.verts;
							var polys = data.polys;
							if( vertices == null || polys == null )
								continue;

							foreach( var poly in polys )
							{
								//if( poly.Area.IsWalkable )
								//{

								for( int n = 2; n < poly.vertCount; n++ )
								{
									int index0 = poly.verts[ 0 ];
									int index1 = poly.verts[ n ];
									int index2 = poly.verts[ n - 1 ];

									var v0 = new RcVec3f( vertices[ index0 * 3 ], vertices[ index0 * 3 + 1 ], vertices[ index0 * 3 + 2 ] );
									var v1 = new RcVec3f( vertices[ index1 * 3 ], vertices[ index1 * 3 + 1 ], vertices[ index1 * 3 + 2 ] );
									var v2 = new RcVec3f( vertices[ index2 * 3 ], vertices[ index2 * 3 + 1 ], vertices[ index2 * 3 + 2 ] );

									var vv0 = ConvertToEngineCoordinates( v0 );
									var vv1 = ConvertToEngineCoordinates( v1 );
									var vv2 = ConvertToEngineCoordinates( v2 );

									result.Add( vv0 );
									//result.Add( v1 );
									result.Add( vv2 );
									result.Add( vv1 );
								}

								//}
							}
						}

						//with indexes?
						var resultArray = result.ToArray();
						owner.navigationMeshVerticesPrevious = resultArray;
						owner.navigationMeshVertices = resultArray;
					}
				}
			}
		}

		/////////////////////////////////////////

		internal class StaticGeometry
		{
			public double AgentHeight;
			public double AgentMaxClimb;

			public Bounds bounds = Bounds.Cleared;
			public AreaData[] data = new AreaData[ 256 ];
			public AreaData areaDataNotWalkable = new AreaData();

			//public List<ConvexVolume> convexVolumes = new List<ConvexVolume>();

			//

			public class AreaData
			{

				//!!!!maybe not save in simulation. used to visualize input mesh

				public OpenList<Vector3> vertices;
				public OpenList<int> indices;

				public AreaData()
				{
					vertices = new OpenList<Vector3>( 2048 );
					indices = new OpenList<int>( 2048 );
				}
			}

			public void Add( Vector3[] vertices, int[] indices, int area ) //byte area
			{
				if( indices.Length == 0 || vertices.Length == 0 )
					return;

				//var newVertices = new List<Vector3>( vertices.Length );
				//var newIndices = new List<int>( indices.Length );

				//if( clipByBounds )
				//{
				//	for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
				//	{
				//		var polygon = new Vector3[ 3 ];
				//		polygon[ 0 ] = vertices[ indices[ nTriangle * 3 + 0 ] ];
				//		polygon[ 1 ] = vertices[ indices[ nTriangle * 3 + 1 ] ];
				//		polygon[ 2 ] = vertices[ indices[ nTriangle * 3 + 2 ] ];

				//		//+X
				//		{
				//			var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Maximum, 0 ), new Vector3( -1, 0, 0 ) );
				//			polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
				//		}

				//		//+Y
				//		{
				//			var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Maximum, 0 ), new Vector3( 0, -1, 0 ) );
				//			polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
				//		}

				//		//-X
				//		{
				//			var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Minimum, 0 ), new Vector3( 1, 0, 0 ) );
				//			polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
				//		}

				//		//-Y
				//		{
				//			var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Minimum, 0 ), new Vector3( 0, 1, 0 ) );
				//			polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
				//		}

				//		//!!!!impl indices

				//		////triangulate
				//		//for( int n = 1; n < polygon.Length - 1; n++ )
				//		//{
				//		//	var v1 = polygon[ 0 ];
				//		//	var v2 = polygon[ n ];
				//		//	var v3 = polygon[ n + 1 ];

				//		//	if( v1 != v2 && v1 != v3 && v2 != v3 )
				//		//	{
				//		//		newVertices.Add( v1 );
				//		//		newVertices.Add( v2 );
				//		//		newVertices.Add( v3 );
				//		//	}
				//		//}
				//	}
				//}
				//else
				{
					//for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
					//{
					//	var v1 = vertices[ indices[ nTriangle * 3 + 0 ] ];
					//	var v2 = vertices[ indices[ nTriangle * 3 + 1 ] ];
					//	var v3 = vertices[ indices[ nTriangle * 3 + 2 ] ];

					//	if( v1 != v2 && v1 != v3 && v2 != v3 )
					//	{
					//		newVertices.Add( v1 );
					//		newVertices.Add( v2 );
					//		newVertices.Add( v3 );
					//	}
					//}
				}

				//if( newVertices.Count == 0 )
				//	return;

				var areaData = data[ area ];
				if( areaData == null )
				{
					areaData = new AreaData();
					data[ area ] = areaData;
				}

				{
					var startVertexIndex = areaData.vertices.Count;
					areaData.vertices.AddRange( vertices );
					foreach( var index in indices )
						areaData.indices.Add( startVertexIndex + index );

					bounds.Add( vertices );
				}

				//fill walkable gap inside
				if( area == areaNotWalkable )
				{
					var startVertexIndex = areaData.vertices.Count;
					for( int n = 0; n < vertices.Length; n++ )
						areaData.vertices.Add( vertices[ n ] + new Vector3( 0, 0, AgentHeight * 0.9 ) );
					foreach( var index in indices )
						areaData.indices.Add( startVertexIndex + index );

					bounds.Add( vertices );
				}
			}

			public bool IsEmpty
			{
				get
				{
					for( int n = 0; n < data.Length; n++ )
					{
						if( data[ n ] != null && data[ n ].indices.Count != 0 )
							return false;
					}
					return true;
				}
			}
		}

		///////////////////////////////////////////

		static Pathfinding()
		{
		}

		public Pathfinding()
		{
		}

		public static IList<Pathfinding> Instances
		{
			get { return instancesAsArray; }
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			//!!!!impl

			//var p = member as Metadata.Property;
			//if( p != null )
			//{
			//	switch( p.Name )
			//	{
			//	case nameof( TileSizeInCells ):
			//		if( !Tiles )
			//			skip = true;
			//		break;
			//	}
			//}
		}

		//!!!!double

		public static RcVec3f ConvertToDotRecastCoordinates( Vector3 v, bool abs = false )
		{
			if( abs )
				return new RcVec3f( (float)Math.Abs( v.X ), (float)Math.Abs( v.Z ), (float)Math.Abs( v.Y ) );
			else
				return new RcVec3f( (float)v.X, (float)v.Z, -(float)v.Y );
		}

		public static Vector3F ConvertToEngineCoordinates( RcVec3f v, bool abs = false )
		{
			if( abs )
				return new Vector3F( Math.Abs( v.X ), Math.Abs( v.Z ), Math.Abs( v.Y ) );
			else
				return new Vector3F( v.X, -v.Z, v.Y );
		}

		////static BBox3 ToSharpNav( Bounds v )
		////{
		////	var min = new Vector3( v.Minimum.X, v.Minimum.Z, v.Minimum.Y );
		////	var max = new Vector3( v.Maximum.X, v.Maximum.Z, v.Maximum.Y );

		////	var b = new Bounds( min );
		////	b.Add( max );
		////	return new BBox3( (float)b.Minimum.X, (float)b.Minimum.Y, (float)b.Minimum.Z, (float)b.Maximum.X, (float)b.Maximum.Y, (float)b.Maximum.Z );
		////}

		protected override void OnEnabledInHierarchyChanged()
		{
			if( EnabledInHierarchyAndIsInstance )
			{
				lock( instances )
				{
					instances.Add( this );
					instancesAsArray = instances.ToArray();
				}
			}

			base.OnEnabledInHierarchyChanged();

			var scene = ParentScene;
			if( scene != null )
			{
				if( EnabledInHierarchyAndIsInstance )
					scene.RenderEvent += Scene_RenderEvent;
				else
					scene.RenderEvent -= Scene_RenderEvent;
			}

			if( EnabledInHierarchyAndIsInstance )
				firstOnUpdateAfterEnabledInHierarchy = true;

			if( !EnabledInHierarchyAndIsInstance )
			{
				lock( instances )
				{
					instances.Remove( this );
					instancesAsArray = instances.ToArray();
				}
			}
		}

		////Bounds GetGeometriesBoundsForNavMesh()
		////{
		////	var result = Bounds.Cleared;

		////	var scene = FindParent<Scene>();
		////	if( scene != null )
		////	{
		////		//Geometry
		////		foreach( var geometry in scene.GetComponents<PathfindingGeometry>( false, true, true ) )
		////		{
		////			var type = geometry.Type.Value;
		////			if( type == PathfindingGeometry.TypeEnum.BakedObstacle )
		////				result.Add( geometry.SpaceBounds.BoundingBox );
		////		}

		////		foreach( var geometryTag in scene.GetComponents<PathfindingGeometryTag>( false, true, true ) )
		////		{
		////			var type = geometryTag.Type.Value;
		////			if( type == PathfindingGeometryTag.TypeEnum.BakedObstacle )
		////			{
		////				//MeshInSpace
		////				var meshInSpace = geometryTag.Parent as MeshInSpace;
		////				if( meshInSpace != null )
		////				{
		////					var mesh = meshInSpace.Mesh.Value;
		////					if( mesh != null && mesh.Result != null )
		////					{
		////						var b = meshInSpace.SpaceBounds.BoundingBox;
		////						if( !b.IsCleared() )
		////							result.Add( b );
		////					}
		////				}

		////				//Terrain
		////				var terrain = geometryTag.Parent as Terrain;
		////				if( terrain != null )
		////				{
		////					var b = terrain.GetBoundsFromTiles();
		////					if( !b.IsCleared() )
		////						result.Add( b );
		////				}
		////			}
		////		}
		////	}

		////	return result;
		////}

		List<Component> GetAllBakedGeometriesAndGeometryTags()
		{
			var result = new List<Component>( 128 );

			var scene = ParentScene;
			if( scene != null )
			{
				foreach( var geometry in scene.GetComponents<PathfindingGeometry>( false, true, true ) )
				{
					if( !geometry.Dynamic )
					{
						var add = true;
						FilterGeometry?.Invoke( this, geometry, ref add );
						if( add )
							result.Add( geometry );
					}
				}

				foreach( var geometryTag in scene.GetComponents<PathfindingGeometryTag>( false, true, true ) )
				{
					var add = true;
					FilterGeometry?.Invoke( this, geometryTag, ref add );
					if( add )
						result.Add( geometryTag );
				}
			}

			return result;
		}

		StaticGeometry GetGeometriesForNavMesh( List<Component> allBakedGeometriesAndGeometryTags )//, bool clipByBounds, Rectangle clipBounds )
		{
			var result = new StaticGeometry();
			result.AgentHeight = AgentHeight;
			result.AgentMaxClimb = AgentMaxClimb;

			foreach( var obj in allBakedGeometriesAndGeometryTags )
			{
				//Geometry
				var geometry = obj as PathfindingGeometry;
				if( geometry != null ) //&& ( !clipByBounds || geometry.SpaceBounds.BoundingBox.ToRectangle().Intersects( clipBounds ) ) )
				{
					if( !geometry.Dynamic )
					{
						//var convexVolume = geometry.GetConvexVolume();
						//if( convexVolume != null )
						//{
						//	result.convexVolumes.Add( convexVolume );
						//}
						//else
						{
							geometry.GetGeometry( out var vertices, out var indices );
							if( vertices != null )
								result.Add( vertices, indices, geometry.Walkable ? areaWalkable : areaNotWalkable );
						}
					}
				}

				var geometryTag = obj as PathfindingGeometryTag;
				if( geometryTag != null )
				{
					//var type = geometryTag.Type.Value;
					//if( type == PathfindingGeometryTag.TypeEnum.BakedObstacle )
					{
						if( !geometryTag.Dynamic )
						{
							//MeshInSpace
							var meshInSpace = geometryTag.Parent as MeshInSpace;
							if( meshInSpace != null ) //&& ( !clipByBounds || meshInSpace.SpaceBounds.BoundingBox.ToRectangle().Intersects( clipBounds ) ) )
							{
								var mesh = meshInSpace.Mesh.Value;
								if( mesh != null && mesh.Result != null )
								{
									var transform = meshInSpace.Transform.Value.ToMatrix4();
									var extractedVertices = mesh.Result.ExtractedVerticesPositions;

									//!!!!slowly

									var vertices = new Vector3[ extractedVertices.Length ];
									for( int n = 0; n < vertices.Length; n++ )
										vertices[ n ] = transform * extractedVertices[ n ].ToVector3();

									result.Add( vertices, mesh.Result.ExtractedIndices, geometryTag.Walkable ? areaWalkable : areaNotWalkable );
								}
							}

							//Terrain
							var terrain = geometryTag.Parent as Terrain;
							if( terrain != null ) //&& ( !clipByBounds || terrain.GetBounds2().Intersects( clipBounds ) ) )
							{
								terrain.GetGeometryFromTiles( delegate ( SpaceBounds tileBounds, Vector3[] tileVertices, int[] tileIndices )
								{
									//if( !clipByBounds || tileBounds.BoundingBox.ToRectangle().Intersects( clipBounds ) )
									result.Add( tileVertices, tileIndices, geometryTag.Walkable ? areaWalkable : areaNotWalkable );
								} );
							}

							//Fence
							var fence = geometryTag.Parent as Fence;
							if( fence != null )
							{
								var logicalData = fence.GetLogicalData();
								if( logicalData != null )
								{
									foreach( var item in logicalData.GetMeshesToCreate() )
									{
										var mesh = item.mesh;
										if( mesh == null || mesh.Result == null )
											continue;

										//the same correction as for collision bodies
										Transform itemTransform;
										if( item.clipDistanceFactor.HasValue )
											itemTransform = item.transform.UpdateScale( item.transform.Scale * new Vector3( item.clipDistanceFactor.Value, 1, 1 ) );
										else
											itemTransform = item.transform;

										var transform = itemTransform.ToMatrix4();
										var extractedVertices = mesh.Result.ExtractedVerticesPositions;

										var vertices = new Vector3[ extractedVertices.Length ];
										for( int n = 0; n < vertices.Length; n++ )
											vertices[ n ] = transform * extractedVertices[ n ].ToVector3();

										result.Add( vertices, mesh.Result.ExtractedIndices, geometryTag.Walkable ? areaWalkable : areaNotWalkable );
									}
								}
							}

							//Building
							var building = geometryTag.Parent as Building;
							if( building != null )
							{
								var logicalData = building.GetLogicalData();
								if( logicalData != null && logicalData.TotalHeight > 0 )
								{
									logicalData.GetCellsBox( out var box, out _ );

									SimpleMeshGenerator.GenerateBox( box.Extents * 2, out Vector3[] verticesLocal, out int[] indices );

									var vertices = new Vector3[ verticesLocal.Length ];
									for( int n = 0; n < vertices.Length; n++ )
										vertices[ n ] = box.Center + box.Axis * verticesLocal[ n ];

									result.Add( vertices, indices, geometryTag.Walkable ? areaWalkable : areaNotWalkable );
								}
							}
						}
					}
				}
			}

			return result;
		}

		//OnGetRenderSceneData?
		private void Scene_RenderEvent( Scene sender, Viewport viewport )
		{
			var context2 = viewport.RenderingContext.ObjectInSpaceRenderingContext;
			if( ShowInputMesh )
				DrawInputMesh( viewport );
			if( ShowNavMesh || ShowNavMeshWhenSelected && context2.selectedObjects.Contains( this ) )
				DrawNavMesh( viewport );
		}

		public void DrawInputMesh( Viewport viewport )
		{
			var staticGeometry = GetBackgroundThreadData()?.precompiledData?.staticGeometry;
			if( staticGeometry != null )
			{
				foreach( var areaData in staticGeometry.data )
				{
					if( areaData == null )
						continue;

					var vertices = areaData.vertices?.ToArray();
					var indices = areaData.indices?.ToArray();
					if( vertices != null && indices != null )
					{
						var transform = Matrix4.FromTranslate( new Vector3( 0, 0, 0.05 ) );
						var renderer = viewport.Simple3DRenderer;
						renderer.SetColor( new ColorValue( 0, 0, 1, 0.4 ), new ColorValue( 0, 0, 1, 0.2 ) );
						renderer.AddTriangles( vertices, indices, ref transform, false, true );
						renderer.SetColor( new ColorValue( 0, 0, 1, 1 ), new ColorValue( 0, 0, 1, 0.5 ) );
						renderer.AddTriangles( vertices, indices, ref transform, true, true );
					}
				}
			}
		}

		public void DrawNavMesh( Viewport viewport )
		{
			var backgroundThreadData = GetBackgroundThreadData();
			if( backgroundThreadData != null )
			{
				//no data, need to get it
				if( backgroundThreadData.navigationMeshVertices == null )
					NeedGetNavigationMeshVertices( false );

				var vertices = backgroundThreadData.navigationMeshVertices ?? backgroundThreadData.navigationMeshVerticesPrevious;
				if( vertices != null )
				{
					var transform = Matrix4.FromTranslate( new Vector3( 0, 0, 0.05 ) );
					var renderer = viewport.Simple3DRenderer;
					renderer.SetColor( new ColorValue( 0, 1, 0, 0.4 ), new ColorValue( 0, 1, 0, 0.2 ) );
					renderer.AddTriangles( vertices, ref transform, false, true );
					renderer.SetColor( new ColorValue( 1, 1, 0, 1 ), new ColorValue( 1, 1, 0, 0.5 ) );
					renderer.AddTriangles( vertices, ref transform, true, true );
				}
			}
		}

		public bool BuildPrecompiledData( out string error )
		{
			error = "";

			PrecompiledData = null;

			if( !EnabledInHierarchyAndIsInstance )
			{
				error = "The component is not enabled.";
				return false;
			}

			var newData = new PrecompiledDataClass();

			var allBakedGeometriesAndGeometryTags = GetAllBakedGeometriesAndGeometryTags();

			var staticGeometry = GetGeometriesForNavMesh( allBakedGeometriesAndGeometryTags );//, false, new Rectangle( double.MinValue, double.MinValue, double.MaxValue, double.MaxValue ) );

			if( staticGeometry.IsEmpty )
			{
				error = "No geometry were gathered from collision objects.";
				return false;
			}

			newData.staticGeometry = staticGeometry;

			PrecompiledData = newData;

			return true;
		}

		internal BackgroundThreadData GetBackgroundThreadData()
		{
			if( precompiledData != null && EnabledInHierarchy )
			{
				if( backgroundThreadData == null || backgroundThreadData.precompiledData != precompiledData )
					backgroundThreadData = new BackgroundThreadData( this, precompiledData );
			}
			else
				backgroundThreadData = null;

			return backgroundThreadData;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EnabledInHierarchy )
			{
				if( firstOnUpdateAfterEnabledInHierarchy )
				{
					firstOnUpdateAfterEnabledInHierarchy = false;
					StartFullUpdate();// false );
				}

				//update dynamic obstacles
				dynamicGeometriesUpdateRemainingTime -= delta;
				if( dynamicGeometriesUpdateRemainingTime < 0 )
				{
					dynamicGeometriesUpdateRemainingTime = DynamicObstaclesUpdatePeriod.Value;

					lock( dynamicGeometriesToUpdate )
					{
						if( dynamicGeometriesToUpdate.Count > 0 )
						{
							if( !CommandTypeIsInQueue( typeof( CommandUpdateDynamicObstacles ) ) )
							{
								var command = new CommandUpdateDynamicObstacles();
								command.geometries = dynamicGeometriesToUpdate.ToArray();
								AddCommand( command, false, false );

								dynamicGeometriesToUpdate.Clear();
							}
						}
					}
				}

				//work with frezees
				////auto update in the editor
				//if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Editor && AutoUpdatePeriodEditor.Value != 0 )
				//{
				//	autoUpdateEditorRemainingTime -= delta;
				//	if( autoUpdateEditorRemainingTime < 0 )
				//	{
				//		autoUpdateEditorRemainingTime = AutoUpdatePeriodEditor.Value;

				//		if( !CommandTypeIsInQueue( typeof( CommandForceUpdate ) ) )
				//			StartFullUpdate();
				//	}
				//}
			}

			var backgroundThreadData = GetBackgroundThreadData();
			backgroundThreadData?.UpdateFromMainThread();
		}

		bool CommandTypeIsInQueue( Type type )
		{
			var backgroundThreadData = GetBackgroundThreadData();
			if( backgroundThreadData != null )
				return backgroundThreadData.CommandTypeIsInQueue( type );
			return false;
		}

		void AddCommand( Command command, bool wait, bool skipIfSameTypeCommandInQueue )
		{
			var backgroundThreadData = GetBackgroundThreadData();
			if( backgroundThreadData == null )
				return;

			command.owner = backgroundThreadData;

			backgroundThreadData.AddCommandFromMainThread( command, wait, skipIfSameTypeCommandInQueue );
		}

		public void FindPath( FindPathContext[] contexts, bool wait )
		{
			var command = new CommandFindPath();
			command.contexts = contexts;
			AddCommand( command, wait, false );
		}

		public void FindPath( FindPathContext context, bool wait )
		{
			FindPath( new FindPathContext[] { context }, wait );
		}

		void NeedGetNavigationMeshVertices( bool wait )
		{
			//don't add when the command is already in the queue
			var command = new CommandGetNavigationMeshVertices();
			AddCommand( command, wait, true );
		}

		public void StartFullUpdate()// bool wait )
		{
			//if( precompiledData == null )
			if( !BuildPrecompiledData( out var error ) )
			{
				//Log.Warning( error );
				return;
			}

			var command = new CommandForceUpdate();
			AddCommand( command, false, true );

			//add dynamic obstacles update commands for all dynamic geometries
			var scene = ParentScene;
			if( scene != null )
			{
				foreach( var geometry in scene.GetComponents<PathfindingGeometry>( false, true, true ) )
				{
					if( geometry.Dynamic )
					{
						var add = true;
						FilterGeometry?.Invoke( this, geometry, ref add );
						if( add )
							geometry.DynamicMode_UpdatePathfindingComponents( this );
					}
				}

				foreach( var geometryTag in scene.GetComponents<PathfindingGeometryTag>( false, true, true ) )
				{
					if( geometryTag.Dynamic )
					{
						var add = true;
						FilterGeometry?.Invoke( this, geometryTag, ref add );
						if( add )
							geometryTag.DynamicMode_UpdatePathfindingComponents( this );
					}
				}
			}
		}

		internal static void UpdateDynamicGeometry( Component geometry, Scene scene, DynamicGeometriesToUpdateItem data, Pathfinding specifiedPathfinding )
		{
			if( scene == null )
				return;

			var instances = Instances;
			for( int n = 0; n < instances.Count; n++ )
			{
				var pathfinding = instances[ n ];

				if( scene == pathfinding.ParentScene && ( specifiedPathfinding == null || pathfinding == specifiedPathfinding ) )
					pathfinding.OnUpdatePathfindingGeometry( geometry, data );
			}
		}

		internal void OnUpdatePathfindingGeometry( Component geometry, DynamicGeometriesToUpdateItem data )
		{
			var add = true;
			FilterGeometry?.Invoke( this, geometry, ref add );
			if( add )
			{
				lock( dynamicGeometriesToUpdate )
					dynamicGeometriesToUpdate[ geometry ] = data;
			}
		}
	}
}
