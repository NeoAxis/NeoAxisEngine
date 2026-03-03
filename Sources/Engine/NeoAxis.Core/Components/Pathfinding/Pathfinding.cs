// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Internal.SharpNav;
using Internal.SharpNav.Pathfinding;
using Internal.SharpNav.Geometry;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// Navigation mesh pathfinding.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Pathfinding\Pathfinding", 540 )]
#if !DEPLOY
	[Editor.SettingsCell( typeof( Editor.PathfindingSettingsCell ) )]
#endif
	public class Pathfinding : ObjectInSpace
	{
		static List<Pathfinding> instances = new List<Pathfinding>();
		static ReadOnlyCollection<Pathfinding> instancesReadOnly;

		const int maxPathFindInParallel = 8;

		PrecompiledDataClass precompiledData;
		BackgroundThreadData backgroundThreadData;

		bool firstOnUpdateAfterEnabledInHierarchy;

		//!!!!
		//Vector3[] tempDebugVertices;
		//int[] tempDebugIndices;

		/////////////////////////////////////////

		//!!!!
		//!!!!THE SUPPORT OF THE FEATURE IS NOT COMPLETED.
		/// <summary>
		/// Whether to use tiles to partition the navigation mesh. THE SUPPORT OF THE FEATURE IS NOT COMPLETED.
		/// </summary>
		[Category( "Tiles" )]
		[DefaultValue( false )]//!!!![DefaultValue( true )]
		public Reference<bool> Tiles
		{
			get { if( _tiles.BeginGet() ) Tiles = _tiles.Get( this ); return _tiles.value; }
			set { if( _tiles.BeginSet( this, ref value ) ) { try { TilesChanged?.Invoke( this ); } finally { _tiles.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Tiles"/> property value changes.</summary>
		public event Action<Pathfinding> TilesChanged;
		ReferenceField<bool> _tiles = false;//!!!! true;

		//!!!!default
		/// <summary>
		/// The amount of cells in the tile by one axis.
		/// </summary>
		[Category( "Tiles" )]
		[DefaultValue( 30 )]
		public Reference<int> TileSizeInCells
		{
			get { if( _tileSizeInCells.BeginGet() ) TileSizeInCells = _tileSizeInCells.Get( this ); return _tileSizeInCells.value; }
			set
			{
				if( _tileSizeInCells.BeginSet( this, ref value ) )
				{
					if( value < 1 )
						value = new Reference<int>( 1, value.GetByReference );
					try { TileSizeInCellsChanged?.Invoke( this ); } finally { _tileSizeInCells.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="TileSizeInCells"/> property value changes.</summary>
		public event Action<Pathfinding> TileSizeInCellsChanged;
		ReferenceField<int> _tileSizeInCells = 30;

		/// <summary>
		/// The width and depth resolution used when sampling the source geometry. The width and depth of the voxels in voxel fields. The width and depth of the cell columns that make up voxel fields. A lower value allows for the generated meshes to more closely match the source geometry, but at a higher processing and memory cost.
		/// </summary>
		[DefaultValue( 0.3 )]
		[Serialize]
		[Category( "Grid" )]
		public Reference<double> CellSize
		{
			get { if( _cellSize.BeginGet() ) CellSize = _cellSize.Get( this ); return _cellSize.value; }
			set
			{
				if( _cellSize.BeginSet( this, ref value ) )
				{
					if( value < 0.01 )
						value = new Reference<double>( 0.01, value.GetByReference );
					try { CellSizeChanged?.Invoke( this ); } finally { _cellSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CellSize"/> property value changes.</summary>
		public event Action<Pathfinding> CellSizeChanged;
		ReferenceField<double> _cellSize = 0.3;

		/// <summary>
		/// The height resolution used when sampling the source geometry. The height of the voxels in voxel fields.
		/// </summary>
		[DefaultValue( 0.2 )]
		[Serialize]
		[Category( "Grid" )]
		public Reference<double> CellHeight
		{
			get { if( _cellHeight.BeginGet() ) CellHeight = _cellHeight.Get( this ); return _cellHeight.value; }
			set
			{
				if( _cellHeight.BeginSet( this, ref value ) )
				{
					if( value < 0.01 )
						value = new Reference<double>( 0.01, value.GetByReference );
					try { CellHeightChanged?.Invoke( this ); } finally { _cellHeight.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CellHeight"/> property value changes.</summary>
		public event Action<Pathfinding> CellHeightChanged;
		ReferenceField<double> _cellHeight = 0.2;

		//!!!!?
		///// <summary>
		///// Max amount of triangles for each chunk in the internal AABB tree.
		///// </summary>
		//[DefaultValue( 512 )]
		//[Serialize]
		//[Category( "Grid" )]
		//public Reference<int> TrianglesPerChunk
		//{
		//	get { if( _trianglesPerChunk.BeginGet() ) TrianglesPerChunk = _trianglesPerChunk.Get( this ); return _trianglesPerChunk.value; }
		//	set
		//	{
		//		if( _trianglesPerChunk.BeginSet( this, ref value ) )
		//		{
		//			if( value < 128 )
		//				value = new Reference<int>( 128, value.GetByReference );
		//			try { TrianglesPerChunkChanged?.Invoke( this ); } finally { _trianglesPerChunk.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Pathfinding> TrianglesPerChunkChanged;
		//ReferenceField<int> _trianglesPerChunk = 512;

		/// <summary>
		/// The minimum region size for unconnected (island) regions. The value is in voxels. Regions that are not connected to any other region and are smaller than this size will be culled before mesh generation. I.e. They will no longer be considered traversable.
		/// </summary>
		[DefaultValue( 8 )]
		[Serialize]
		[Category( "Regions" )]
		public Reference<int> MinRegionSize
		{
			get { if( _minRegionSize.BeginGet() ) MinRegionSize = _minRegionSize.Get( this ); return _minRegionSize.value; }
			set
			{
				if( _minRegionSize.BeginSet( this, ref value ) )
				{
					if( value < 1 )
						value = new Reference<int>( 1, value.GetByReference );
					try { MinRegionSizeChanged?.Invoke( this ); } finally { _minRegionSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MinRegionSize"/> property value changes.</summary>
		public event Action<Pathfinding> MinRegionSizeChanged;
		ReferenceField<int> _minRegionSize = 8;

		/// <summary>
		/// Any regions smaller than this size will, if possible, be merged with larger regions. Value is in voxels. Helps reduce the number of small regions. This is especially an issue in diagonal path regions where inherent faults in the region generation algorithm can result in unnecessarily small regions.
		/// </summary>
		[DefaultValue( 20 )]
		[Serialize]
		[Category( "Regions" )]
		public Reference<int> MergedRegionSize
		{
			get { if( _mergedRegionSize.BeginGet() ) MergedRegionSize = _mergedRegionSize.Get( this ); return _mergedRegionSize.value; }
			set
			{
				if( _mergedRegionSize.BeginSet( this, ref value ) )
				{
					if( value < 0 )
						value = new Reference<int>( 0, value.GetByReference );
					try { MergedRegionSizeChanged?.Invoke( this ); } finally { _mergedRegionSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MergedRegionSize"/> property value changes.</summary>
		public event Action<Pathfinding> MergedRegionSizeChanged;
		ReferenceField<int> _mergedRegionSize = 20;

		/// <summary>
		/// The maximum length of polygon edges that represent the border of meshes. More vertices will be added to border edges if this value is exceeded for a particular edge. A value of zero will disable this feature.
		/// </summary>
		[DefaultValue( 12 )]
		[Serialize]
		[Category( "Polygonization" )]
		public Reference<int> MaxEdgeLength
		{
			get { if( _maxEdgeLength.BeginGet() ) MaxEdgeLength = _maxEdgeLength.Get( this ); return _maxEdgeLength.value; }
			set
			{
				if( _maxEdgeLength.BeginSet( this, ref value ) )
				{
					if( value < 0 )
						value = new Reference<int>( 0, value.GetByReference );
					try { MaxEdgeLengthChanged?.Invoke( this ); } finally { _maxEdgeLength.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaxEdgeLength"/> property value changes.</summary>
		public event Action<Pathfinding> MaxEdgeLengthChanged;
		ReferenceField<int> _maxEdgeLength = 12;

		/// <summary>
		/// The maximum distance the edges of meshes may deviate from the source geometry. A lower value will result in mesh edges following the xy-plane geometry contour more accurately at the expense of an increased triangle count.
		/// </summary>
		[DefaultValue( 1.8 )]
		[Serialize]
		[Category( "Polygonization" )]
		public Reference<double> MaxEdgeError
		{
			get { if( _maxEdgeError.BeginGet() ) MaxEdgeError = _maxEdgeError.Get( this ); return _maxEdgeError.value; }
			set
			{
				if( _maxEdgeError.BeginSet( this, ref value ) )
				{
					if( value < 0.1 )
						value = new Reference<double>( 0.1, value.GetByReference );
					try { MaxEdgeErrorChanged?.Invoke( this ); } finally { _maxEdgeError.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaxEdgeError"/> property value changes.</summary>
		public event Action<Pathfinding> MaxEdgeErrorChanged;
		ReferenceField<double> _maxEdgeError = 1.8;

		[DefaultValue( 6 )]
		[Serialize]
		[Category( "Polygonization" )]
		[Range( 3, 6 )]
		public Reference<int> MaxVerticesPerPolygon
		{
			get { if( _maxVerticesPerPolygon.BeginGet() ) MaxVerticesPerPolygon = _maxVerticesPerPolygon.Get( this ); return _maxVerticesPerPolygon.value; }
			set
			{
				if( _maxVerticesPerPolygon.BeginSet( this, ref value ) )
				{
					if( value < 3 )
						value = new Reference<int>( 3, value.GetByReference );
					try { MaxVerticesPerPolygonChanged?.Invoke( this ); } finally { _maxVerticesPerPolygon.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaxVerticesPerPolygon"/> property value changes.</summary>
		public event Action<Pathfinding> MaxVerticesPerPolygonChanged;
		ReferenceField<int> _maxVerticesPerPolygon = 6;

		/// <summary>
		/// Sets the sampling distance to use when matching the detail mesh to the surface of the original geometry. Impacts how well the final detail mesh conforms to the surface contour of the original geometry. Higher values result in a detail mesh which conforms more closely to the original geometry's surface at the cost of a higher final triangle count and higher processing cost.
		/// </summary>
		[DefaultValue( 6 )]
		[Serialize]
		[Category( "Detail Mesh" )]
		public Reference<int> DetailSampleDistance
		{
			get { if( _detailSampleDistance.BeginGet() ) DetailSampleDistance = _detailSampleDistance.Get( this ); return _detailSampleDistance.value; }
			set
			{
				if( _detailSampleDistance.BeginSet( this, ref value ) )
				{
					if( value < 0.0 )
						value = new Reference<int>( 0, value.GetByReference );
					try { DetailSampleDistanceChanged?.Invoke( this ); } finally { _detailSampleDistance.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DetailSampleDistance"/> property value changes.</summary>
		public event Action<Pathfinding> DetailSampleDistanceChanged;
		ReferenceField<int> _detailSampleDistance = 6;

		/// <summary>
		/// The maximum distance the surface of the detail mesh may deviate from the surface of the original geometry.
		/// </summary>
		[DefaultValue( 1 )]
		[Serialize]
		[Category( "Detail Mesh" )]
		public Reference<int> DetailMaxSampleError
		{
			get { if( _detailMaxSampleError.BeginGet() ) DetailMaxSampleError = _detailMaxSampleError.Get( this ); return _detailMaxSampleError.value; }
			set
			{
				if( _detailMaxSampleError.BeginSet( this, ref value ) )
				{
					if( value < 0.0 )
						value = new Reference<int>( 0, value.GetByReference );
					try { DetailMaxSampleErrorChanged?.Invoke( this ); } finally { _detailMaxSampleError.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DetailMaxSampleError"/> property value changes.</summary>
		public event Action<Pathfinding> DetailMaxSampleErrorChanged;
		ReferenceField<int> _detailMaxSampleError = 1;

		/// <summary>
		/// Minimum height where the agent can still walk.
		/// </summary>
		[DefaultValue( 2.0 )]
		[Serialize]
		[Category( "Agent" )]
		public Reference<double> AgentHeight
		{
			get { if( _agentHeight.BeginGet() ) AgentHeight = _agentHeight.Get( this ); return _agentHeight.value; }
			set
			{
				if( _agentHeight.BeginSet( this, ref value ) )
				{
					if( value < 0.1 )
						value = new Reference<double>( 0.1, value.GetByReference );
					try { AgentHeightChanged?.Invoke( this ); } finally { _agentHeight.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AgentHeight"/> property value changes.</summary>
		public event Action<Pathfinding> AgentHeightChanged;
		ReferenceField<double> _agentHeight = 2.0;

		/// <summary>
		/// Radius of the agent.
		/// </summary>
		[DefaultValue( 0.6 )]
		[Serialize]
		[Category( "Agent" )]
		public Reference<double> AgentRadius
		{
			get { if( _agentRadius.BeginGet() ) AgentRadius = _agentRadius.Get( this ); return _agentRadius.value; }
			set
			{
				if( _agentRadius.BeginSet( this, ref value ) )
				{
					if( value < 0.0 )
						value = new Reference<double>( 0.0, value.GetByReference );
					try { AgentRadiusChanged?.Invoke( this ); } finally { _agentRadius.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AgentRadius"/> property value changes.</summary>
		public event Action<Pathfinding> AgentRadiusChanged;
		ReferenceField<double> _agentRadius = 0.6;

		/// <summary>
		/// Maximum height between grid cells the agent can climb.
		/// </summary>
		[DefaultValue( 0.9 )]
		[Serialize]
		[Category( "Agent" )]
		public Reference<double> AgentMaxClimb
		{
			get { if( _agentMaxClimb.BeginGet() ) AgentMaxClimb = _agentMaxClimb.Get( this ); return _agentMaxClimb.value; }
			set
			{
				if( _agentMaxClimb.BeginSet( this, ref value ) )
				{
					if( value < 0.001 )
						value = new Reference<double>( 0.001, value.GetByReference );
					try { AgentMaxClimbChanged?.Invoke( this ); } finally { _agentMaxClimb.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AgentMaxClimb"/> property value changes.</summary>
		public event Action<Pathfinding> AgentMaxClimbChanged;
		ReferenceField<double> _agentMaxClimb = 0.9;

		//!!!!
		///// <summary>
		///// Maximum walkable slope angle in degrees.
		///// </summary>
		//[DefaultValue( "45" )]
		//[Serialize]
		//[Category( "Agent" )]
		//[ApplicableRange( 1, 89 )]
		//public Reference<Degree> AgentMaxSlope
		//{
		//	get { if( _agentMaxSlope.BeginGet() ) AgentMaxSlope = _agentMaxSlope.Get( this ); return _agentMaxSlope.value; }
		//	set
		//	{
		//		if( _agentMaxSlope.BeginSet( this, ref value ) )
		//		{
		//			if( value.Value < 1 )
		//				value = new Reference<Degree>( 1, value.GetByReference );
		//			if( value.Value > 89 )
		//				value = new Reference<Degree>( 89, value.GetByReference );
		//			try { AgentMaxSlopeChanged?.Invoke( this ); } finally { _agentMaxSlope.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Pathfinding> AgentMaxSlopeChanged;
		//ReferenceField<Degree> _agentMaxSlope = new Degree( 45 );

		/// <summary>
		/// Maximum number of search nodes to use (max 65536).
		/// </summary>
		[DefaultValue( 4096 )]
		[Serialize]
		[Category( "Pathfinding" )]
		public Reference<int> PathfindingMaxNodes
		{
			get { if( _pathfindingMaxNodes.BeginGet() ) PathfindingMaxNodes = _pathfindingMaxNodes.Get( this ); return _pathfindingMaxNodes.value; }
			set
			{
				if( _pathfindingMaxNodes.BeginSet( this, ref value ) )
				{
					if( value < 4 )
						value = new Reference<int>( 4, value.GetByReference );
					if( value > 65536 )
						value = new Reference<int>( 65536, value.GetByReference );
					try
					{
						PathfindingMaxNodesChanged?.Invoke( this );
						//navMeshQuery = null;
					}
					finally { _pathfindingMaxNodes.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="PathfindingMaxNodes"/> property value changes.</summary>
		public event Action<Pathfinding> PathfindingMaxNodesChanged;
		ReferenceField<int> _pathfindingMaxNodes = 4096;

		[Category( "Debug" )]
		[DefaultValue( false )]
		[Serialize]
		public Reference<bool> AlwaysDisplayNavMesh
		{
			get { if( _alwaysDisplayNavMesh.BeginGet() ) AlwaysDisplayNavMesh = _alwaysDisplayNavMesh.Get( this ); return _alwaysDisplayNavMesh.value; }
			set { if( _alwaysDisplayNavMesh.BeginSet( this, ref value ) ) { try { AlwaysDisplayNavMeshChanged?.Invoke( this ); } finally { _alwaysDisplayNavMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AlwaysDisplayNavMesh"/> property value changes.</summary>
		public event Action<Pathfinding> AlwaysDisplayNavMeshChanged;
		ReferenceField<bool> _alwaysDisplayNavMesh = false;

		[Browsable( false )]
		public PrecompiledDataClass PrecompiledData
		{
			get { return precompiledData; }
			set
			{
				if( precompiledData == value )
					return;
				precompiledData = value;

				if( precompiledData != null )
				{
					var settings = new NavMeshGenerationSettings();
					settings.CellSize = (float)CellSize;
					settings.CellHeight = (float)CellHeight;
					settings.MaxClimb = (float)AgentMaxClimb;
					settings.AgentHeight = (float)AgentHeight;
					settings.AgentRadius = (float)AgentRadius;
					settings.MinRegionSize = MinRegionSize;
					settings.MergedRegionSize = MergedRegionSize;
					settings.MaxEdgeLength = MaxEdgeLength;
					settings.MaxEdgeError = (float)MaxEdgeError;
					settings.VertsPerPoly = MaxVerticesPerPolygon;
					settings.SampleDistance = DetailSampleDistance;
					settings.MaxSampleError = DetailMaxSampleError;
					settings.BuildBoundingVolumeTree = true;
					precompiledData.generationSettings = settings;
				}
			}
		}

		/////////////////////////////////////////

		public class PrecompiledDataClass
		{
			internal bool tiles;
			internal int tileSizeInCells;
			internal double cellSize;
			internal double cellHeight;

			internal class TileData
			{
				public Vector2I index;
				//public Rectangle bounds;
				public TileGeometry staticGeometry;
			}

			internal Dictionary<Vector2I, TileData> precompiledTiles = new Dictionary<Vector2I, TileData>();

			//!!!!
			//!!!!struct?
			internal class MeshOffLinkConnection
			{
				public Vector3 Position1;
				public Vector3 Position2;
				//!!!!
			}
			internal List<MeshOffLinkConnection> meshOffLinkConnections = new List<MeshOffLinkConnection>();

			internal NavMeshGenerationSettings generationSettings = new NavMeshGenerationSettings();

			//

			internal bool Load( TextBlock block )
			{
				try
				{
					tiles = bool.Parse( block.GetAttribute( "Tiles" ) );
					tileSizeInCells = int.Parse( block.GetAttribute( "TileSizeInCells" ) );
					cellSize = double.Parse( block.GetAttribute( "CellSize" ) );
					cellHeight = double.Parse( block.GetAttribute( "CellHeight" ) );

					foreach( var tileBlock in block.Children )
					{
						if( tileBlock.Name == "Tile" )
						{
							var tile = new TileData();
							tile.index = Vector2I.Parse( tileBlock.GetAttribute( "Index" ) );
							//tile.bounds = Bounds.Parse( tileBlock.GetAttribute( "Bounds" ) );

							var staticGeometryBlock = tileBlock.FindChild( "StaticGeometry" );
							if( staticGeometryBlock != null )
							{
								var staticGeometry = new TileGeometry();
								if( !staticGeometry.Load( staticGeometryBlock ) )
									return false;
								tile.staticGeometry = staticGeometry;
							}

							precompiledTiles[ tile.index ] = tile;
						}
					}

					//!!!!!mesh off links
				}
				catch
				{
					return false;
				}
				return true;
			}

			internal void Save( TextBlock block )
			{
				block.SetAttribute( "Tiles", tiles.ToString() );
				block.SetAttribute( "TileSizeInCells", tileSizeInCells.ToString() );
				block.SetAttribute( "CellSize", cellSize.ToString() );
				block.SetAttribute( "CellHeight", cellHeight.ToString() );

				foreach( var tile in precompiledTiles.Values )
				{
					var tileBlock = block.AddChild( "Tile" );
					tileBlock.SetAttribute( "Index", tile.index.ToString() );
					//tileBlock.SetAttribute( "Bounds", tile.bounds.ToString() );

					if( tile.staticGeometry != null )
					{
						var staticGeometryBlock = tileBlock.AddChild( "StaticGeometry" );
						tile.staticGeometry.Save( staticGeometryBlock );
					}
				}
			}

			internal RectangleI GetTileIndexRangeByBounds( Rectangle bounds )
			{
				if( tiles )
				{
					var tileSize = cellSize * tileSizeInCells;

					var min = bounds.Minimum / tileSize;
					if( min.X < 0 ) min.X--;
					if( min.Y < 0 ) min.Y--;

					var max = bounds.Maximum / tileSize;
					if( max.X < 0 ) max.X--;
					if( max.Y < 0 ) max.Y--;

					return new RectangleI( (int)min.X, (int)min.Y, (int)max.X + 1, (int)max.Y + 1 );
				}
				else
					return new RectangleI( 0, 0, 1, 1 );
			}

			internal Rectangle GetTileBounds( Vector2I index )
			{
				if( tiles )
				{
					var tileSize = cellSize * tileSizeInCells;
					var x = tileSize * index.X;
					var y = tileSize * index.Y;
					return new Rectangle( x, y, x + tileSize, y + tileSize );
				}
				else
					return new Rectangle( double.MinValue, double.MinValue, double.MaxValue, double.MaxValue );
			}
		}

		/////////////////////////////////////////

		class BackgroundThreadData
		{
			public PrecompiledDataClass precompiledData;
			public Dictionary<Vector2I, TileData> backgroundTiles = new Dictionary<Vector2I, TileData>();

			//!!!!проверить не всё ли время обновляется
			//!!!!обновлять а то накапливаться будут
			public Queue<Command> commandQueue = new Queue<Command>();
			Thread commandThread;

			public TiledNavMesh tiledNavMesh;
			public Queue<NavMeshQuery> freeNavMeshQueries = new Queue<NavMeshQuery>();
			public int navMeshQueriesCreatedForMaxNodes = -1;

			//separate by tiles?
			public Vector3[] navigationMeshVertices;

			////////////

			public class TileData
			{
				public Vector2I index;
				public bool needUpdate;
				//!!!!проверить чтобы не копились
				public ESet<DynamicObstacleData> dynamicObstacles = new ESet<DynamicObstacleData>();
				////debug draw
				//public Vector3F[] navigationMeshVertices;
			}

			////////////

			public BackgroundThreadData( PrecompiledDataClass precompiledData )
			{
				this.precompiledData = precompiledData;

				//create tiles
				foreach( var precompiledTile in precompiledData.precompiledTiles.Values )
				{
					var backgroundTile = new TileData();
					backgroundTile.index = precompiledTile.index;
					backgroundTiles.Add( backgroundTile.index, backgroundTile );
				}
			}

			void WaitCommandToProcessFromMainThread( Command command )
			{
				while( !command.processed )
				{
					UpdateFromMainThread();
					//!!!!
					Thread.Sleep( 0 );
				}
			}

			public void AddCommandFromMainThread( Command command, bool wait, bool skipIfSameTypeCommandInQueue )
			{
				lock( commandQueue )
				{
					if( skipIfSameTypeCommandInQueue )
					{
						var type = command.GetType();
						foreach( var c in commandQueue )
						{
							if( c.GetType() == type )
								return;
						}
					}

					commandQueue.Enqueue( command );
				}

				if( wait )
					WaitCommandToProcessFromMainThread( command );
			}

			void ThreadFunction( object param )
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
				if( backgroundTiles.Count == 0 )
					return;

				//!!!!tiles
				//!!!!пока полностью обновляется
				foreach( var tile in backgroundTiles.Values )
				{
					if( tile.needUpdate )
					{
						tile.needUpdate = false;

						tiledNavMesh = null;
						freeNavMeshQueries.Clear();
						navMeshQueriesCreatedForMaxNodes = -1;

						//!!!!
						navigationMeshVertices = null;
					}
				}

				if( tiledNavMesh == null )
				{
					try
					{
						var settings = precompiledData.generationSettings;

						//!!!!
						//level.SetBoundingBoxOffset(new SVector3(settings.CellSize * 0.5f, settings.CellHeight * 0.5f, settings.CellSize * 0.5f));

						if( precompiledData.tiles )
						{
							var tileSize = precompiledData.cellSize * precompiledData.tileSizeInCells;

							var maxTiles = precompiledData.precompiledTiles.Count;

							//!!!!
							var maxPolys = 10000;// 0;

							var buildDatas = new List<NavMeshBuilder>();

							foreach( var tile in precompiledData.precompiledTiles.Values )
							{
								var tileBounds = precompiledData.GetTileBounds( tile.index );

								Bounds boundsObjects = Bounds.Cleared;
								{
									if( tile.staticGeometry != null )
										boundsObjects.Add( tile.staticGeometry.bounds );

									var backgroundTile = backgroundTiles[ tile.index ];
									foreach( var dynamicObstacle in backgroundTile.dynamicObstacles )
										boundsObjects.Add( dynamicObstacle.Vertices );
								}

								if( !boundsObjects.IsCleared() )
								{
									var boundsMinZ = boundsObjects.Minimum.Z;
									boundsMinZ /= precompiledData.cellHeight;
									boundsMinZ = (int)boundsMinZ - 2;
									boundsMinZ *= precompiledData.cellHeight;

									var boundsMaxZ = boundsObjects.Maximum.Z;
									boundsMaxZ /= precompiledData.cellHeight;
									boundsMaxZ = (int)boundsMaxZ + 2;
									boundsMaxZ *= precompiledData.cellHeight;

									var tileBounds3 = new Bounds(
										new Vector3( tileBounds.Minimum, boundsMinZ ),
										new Vector3( tileBounds.Maximum, boundsMaxZ ) );

									var heightfield = new Heightfield( ToSharpNav( tileBounds3 ), settings );

									//!!!!по идее это можно не обновлять
									//rasterize static geometry
									if( tile.staticGeometry != null )
									{
										for( int n = 0; n < 256; n++ )
										{
											var areaData = tile.staticGeometry.data[ n ];
											if( areaData != null && areaData.vertices.Count != 0 )
											{
												var vertices2 = new Internal.SharpNav.Geometry.Vector3[ areaData.vertices.Count ];
												for( int nVertex = 0; nVertex < vertices2.Length; nVertex++ )
													vertices2[ nVertex ] = ToSharpNav( areaData.vertices.Data[ nVertex ] );
												////!!!!с индексами передавать
												//var vertices2 = new SharpNav.Geometry.Vector3[ areaData.indices.Count ];
												//for( int index = 0; index < areaData.indices.Count; index++ )
												//	vertices2[ index ] = ToSharpNav( areaData.vertices.Data[ areaData.indices.Data[ index ] ] );

												heightfield.RasterizeTriangles( vertices2, (byte)n );
											}
										}

										//Area[] areas = AreaGenerator.From( vertices2, Area.Default )
										//	.MarkBelowSlope( (float)AgentMaxSlope.Value.InRadians(), Area.Null )
										//	.ToArray();
										//Area[] areas = AreaGenerator.From(triEnumerable, Area.Default)
										//	.MarkAboveHeight(areaSettings.MaxLevelHeight, Area.Null)
										//	.MarkBelowHeight(areaSettings.MinLevelHeight, Area.Null)
										//	.MarkBelowSlope(areaSettings.MaxTriSlope, Area.Null)
										//	.ToArray();
										//heightfield.RasterizeTrianglesWithAreas( vertices2, areas );
									}

									//rasterize dynamic obstacles
									{
										var backgroundTile = backgroundTiles[ tile.index ];

										foreach( var dynamicObstacle in backgroundTile.dynamicObstacles )
										{
											//!!!!с индексами передавать
											var vertices2 = new Internal.SharpNav.Geometry.Vector3[ dynamicObstacle.Indices.Length ];
											for( int index = 0; index < dynamicObstacle.Indices.Length; index++ )
												vertices2[ index ] = ToSharpNav( dynamicObstacle.Vertices[ dynamicObstacle.Indices[ index ] ] );

											heightfield.RasterizeTriangles( vertices2, dynamicObstacle.Area );
										}
									}

									//heightfield.RasterizeTriangles( vertices2, Area.Default );
									////Area[] areas = AreaGenerator.From( vertices2, Area.Default )
									////	.MarkBelowSlope( (float)AgentMaxSlope.Value.InRadians(), Area.Null )
									////	.ToArray();
									////Area[] areas = AreaGenerator.From(triEnumerable, Area.Default)
									////	.MarkAboveHeight(areaSettings.MaxLevelHeight, Area.Null)
									////	.MarkBelowHeight(areaSettings.MinLevelHeight, Area.Null)
									////	.MarkBelowSlope(areaSettings.MaxTriSlope, Area.Null)
									////	.ToArray();
									////heightfield.RasterizeTrianglesWithAreas( vertices2, areas );

									heightfield.FilterLedgeSpans( settings.VoxelAgentHeight, settings.VoxelMaxClimb );
									heightfield.FilterLowHangingWalkableObstacles( settings.VoxelMaxClimb );
									heightfield.FilterWalkableLowHeightSpans( settings.VoxelAgentHeight );

									var compactHeightfield = new CompactHeightfield( heightfield, settings );
									compactHeightfield.Erode( settings.VoxelAgentRadius );
									compactHeightfield.BuildDistanceField();
									//!!!!boiderSize?
									compactHeightfield.BuildRegions( 0, settings.MinRegionSize, settings.MergedRegionSize );

									//System.Random r = new System.Random();
									//var regionColors = new ColorByte[ compactHeightfield.MaxRegions ];
									//regionColors[ 0 ] = new ColorByte( 0, 0, 0 );
									//for( int i = 1; i < regionColors.Length; i++ )
									//	regionColors[ i ] = new ColorByte( (byte)r.Next( 0, 255 ), (byte)r.Next( 0, 255 ), (byte)r.Next( 0, 255 ), (byte)255 );

									var contourSet = compactHeightfield.BuildContourSet( settings );
									var polyMesh = new PolyMesh( contourSet, settings );
									var polyMeshDetail = new PolyMeshDetail( polyMesh, compactHeightfield, settings );

									var buildData = new NavMeshBuilder( polyMesh, polyMeshDetail, new OffMeshConnection[ 0 ], settings );
									buildData.Header.X = tile.index.X;
									buildData.Header.Y = tile.index.Y;

									buildDatas.Add( buildData );
								}
							}

							tiledNavMesh = new TiledNavMesh( new Internal.SharpNav.Geometry.Vector3( 0, 0, 0 ), (float)tileSize, (float)tileSize, maxTiles, maxPolys );
							foreach( var buildData in buildDatas )
								tiledNavMesh.AddTile( buildData );


							//var boundsMin = new SharpNav.Geometry.Vector3( float.MaxValue, float.MaxValue, float.MaxValue );
							//foreach( var buildData in buildDatas )
							//{
							//	var m = buildData.Header.Bounds.Min;

							//	//!!!!!y -> z, z -> y
							//	if( m.X < boundsMin.X )
							//		boundsMin.X = m.X;
							//	if( m.Y < boundsMin.Y )
							//		boundsMin.Y = m.Y;
							//	if( m.Z < boundsMin.Z )
							//		boundsMin.Z = m.Z;
							//}
							//tiledNavMesh = new TiledNavMesh( boundsMin, (float)tileSize, (float)tileSize, maxTiles, maxPolys );

						}
						else
						{
							var tile = precompiledData.precompiledTiles[ Vector2I.Zero ];

							Bounds bounds = Bounds.Cleared;
							{
								if( tile.staticGeometry != null )
									bounds.Add( tile.staticGeometry.bounds );

								var backgroundTile = backgroundTiles[ tile.index ];
								foreach( var dynamicObstacle in backgroundTile.dynamicObstacles )
									bounds.Add( dynamicObstacle.Vertices );
							}

							if( !bounds.IsCleared() )
							{
								var heightfield = new Heightfield( ToSharpNav( bounds ), settings );

								//!!!!по идее это можно не обновлять
								//rasterize static geometry
								if( tile.staticGeometry != null )
								{
									for( int n = 0; n < 256; n++ )
									{
										var areaData = tile.staticGeometry.data[ n ];
										if( areaData != null && areaData.vertices.Count != 0 )
										{
											var vertices2 = new Internal.SharpNav.Geometry.Vector3[ areaData.vertices.Count ];
											for( int nVertex = 0; nVertex < vertices2.Length; nVertex++ )
												vertices2[ nVertex ] = ToSharpNav( areaData.vertices.Data[ nVertex ] );
											////!!!!с индексами передавать
											//var vertices2 = new SharpNav.Geometry.Vector3[ areaData.indices.Count ];
											//for( int index = 0; index < areaData.indices.Count; index++ )
											//	vertices2[ index ] = ToSharpNav( areaData.vertices.Data[ areaData.indices.Data[ index ] ] );

											heightfield.RasterizeTriangles( vertices2, (byte)n );
										}
									}

									//Area[] areas = AreaGenerator.From( vertices2, Area.Default )
									//	.MarkBelowSlope( (float)AgentMaxSlope.Value.InRadians(), Area.Null )
									//	.ToArray();
									//Area[] areas = AreaGenerator.From(triEnumerable, Area.Default)
									//	.MarkAboveHeight(areaSettings.MaxLevelHeight, Area.Null)
									//	.MarkBelowHeight(areaSettings.MinLevelHeight, Area.Null)
									//	.MarkBelowSlope(areaSettings.MaxTriSlope, Area.Null)
									//	.ToArray();
									//heightfield.RasterizeTrianglesWithAreas( vertices2, areas );
								}

								//rasterize dynamic obstacles
								{
									var backgroundTile = backgroundTiles[ tile.index ];

									foreach( var dynamicObstacle in backgroundTile.dynamicObstacles )
									{
										//!!!!с индексами передавать
										var vertices2 = new Internal.SharpNav.Geometry.Vector3[ dynamicObstacle.Indices.Length ];
										for( int index = 0; index < dynamicObstacle.Indices.Length; index++ )
											vertices2[ index ] = ToSharpNav( dynamicObstacle.Vertices[ dynamicObstacle.Indices[ index ] ] );

										heightfield.RasterizeTriangles( vertices2, dynamicObstacle.Area );
									}
								}

								heightfield.FilterLedgeSpans( settings.VoxelAgentHeight, settings.VoxelMaxClimb );
								heightfield.FilterLowHangingWalkableObstacles( settings.VoxelMaxClimb );
								heightfield.FilterWalkableLowHeightSpans( settings.VoxelAgentHeight );

								var compactHeightfield = new CompactHeightfield( heightfield, settings );
								compactHeightfield.Erode( settings.VoxelAgentRadius );
								compactHeightfield.BuildDistanceField();
								//!!!!boiderSize?
								compactHeightfield.BuildRegions( 0, settings.MinRegionSize, settings.MergedRegionSize );

								//System.Random r = new System.Random();
								//var regionColors = new ColorByte[ compactHeightfield.MaxRegions ];
								//regionColors[ 0 ] = new ColorByte( 0, 0, 0 );
								//for( int i = 1; i < regionColors.Length; i++ )
								//	regionColors[ i ] = new ColorByte( (byte)r.Next( 0, 255 ), (byte)r.Next( 0, 255 ), (byte)r.Next( 0, 255 ), (byte)255 );

								var contourSet = compactHeightfield.BuildContourSet( settings );
								var polyMesh = new PolyMesh( contourSet, settings );
								var polyMeshDetail = new PolyMeshDetail( polyMesh, compactHeightfield, settings );

								var offMeshConnections = new List<OffMeshConnection>();

								//!!!!temp
								//foreach( var precompiledConnection in precompiledData.meshOffLinkConnections )
								//{
								//	var connection = new OffMeshConnection();

								//	connection.Pos0 = ToSharpNav( precompiledConnection.Position1 );
								//	connection.Pos1 = ToSharpNav( precompiledConnection.Position2 );

								//	//!!!!

								//	connection.Radius = 1;

								//	//!!!!
								//	connection.Flags = OffMeshConnectionFlags.Bidirectional;

								//	//!!!!
								//	//connection.Poly = zzzz;

								//	//!!!!
								//	connection.Side = BoundarySide.PlusX;

								//	offMeshConnections.Add( connection );
								//}

								var buildData = new NavMeshBuilder( polyMesh, polyMeshDetail, offMeshConnections.ToArray(), settings );
								tiledNavMesh = new TiledNavMesh( buildData );
							}
						}

						//!!!!
						////Pathfinding with multiple units
						//GenerateCrowd();
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
			//public int MaxPolygonPath = 512;
			public int MaxSmoothPath = 2048;
			//public int MaxSteerPoints = 16;

			public bool Finished;
			public Vector3[] Path;
			public string Error = string.Empty;
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

		class CommandDynamicObstacleAdd : Command
		{
			public DynamicObstacleData dynamicObstacle;

			protected override void Process()
			{
				if( owner.backgroundTiles.Count == 0 )
					return;

				var obstanceBounds = dynamicObstacle.Bounds.ToRectangle();

				var tileIndexRange = owner.precompiledData.GetTileIndexRangeByBounds( dynamicObstacle.Bounds.ToRectangle() );
				for( int y = tileIndexRange.Minimum.Y; y < tileIndexRange.Maximum.Y; y++ )
				{
					for( int x = tileIndexRange.Minimum.X; x < tileIndexRange.Maximum.X; x++ )
					{
						var index = new Vector2I( x, y );
						var tileBounds = owner.precompiledData.GetTileBounds( index );

						if( obstanceBounds.Intersects( tileBounds ) )
						{
							if( owner.backgroundTiles.TryGetValue( index, out var tile ) )
							{
								if( tile.dynamicObstacles.AddWithCheckAlreadyContained( dynamicObstacle ) )
								{
									tile.needUpdate = true;

									//!!!!
									owner.navigationMeshVertices = null;
								}
							}
						}
					}
				}


				//	var tile = owner.backgroundTiles[ Vector2I.Zero ];

				//	if( tile.dynamicObstacles.AddWithCheckAlreadyContained( dynamicObstacle ) )
				//	{
				//		tile.needUpdate = true;

				//		//!!!!
				//		owner.navigationMeshVertices = null;
				//	}
			}
		}

		/////////////////////////////////////////

		class CommandDynamicObstacleDelete : Command
		{
			public DynamicObstacleData dynamicObstacle;

			protected override void Process()
			{
				if( owner.backgroundTiles.Count == 0 )
					return;

				var obstanceBounds = dynamicObstacle.Bounds.ToRectangle();

				var tileIndexRange = owner.precompiledData.GetTileIndexRangeByBounds( dynamicObstacle.Bounds.ToRectangle() );
				for( int y = tileIndexRange.Minimum.Y; y < tileIndexRange.Maximum.Y; y++ )
				{
					for( int x = tileIndexRange.Minimum.X; x < tileIndexRange.Maximum.X; x++ )
					{
						var index = new Vector2I( x, y );
						var tileBounds = owner.precompiledData.GetTileBounds( index );

						if( obstanceBounds.Intersects( tileBounds ) )
						{
							if( owner.backgroundTiles.TryGetValue( index, out var tile ) )
							{
								if( tile.dynamicObstacles.Remove( dynamicObstacle ) )
								{
									tile.needUpdate = true;

									//!!!!
									owner.navigationMeshVertices = null;
								}
							}
						}
					}
				}


				//if( owner.backgroundTiles.Count != 0 )
				//{
				//	var tile = owner.backgroundTiles[ Vector2I.Zero ];

				//	if( tile.dynamicObstacles.Remove( dynamicObstacle ) )
				//	{
				//		tile.needUpdate = true;

				//		//!!!!
				//		owner.navigationMeshVertices = null;
				//	}
				//}
			}
		}

		/////////////////////////////////////////

		class CommandFindPath : Command
		{
			public FindPathContext[] contexts;
			public int pathfindingMaxNodes;

			//

			static void VMad( ref Internal.SharpNav.Geometry.Vector3 dest, Internal.SharpNav.Geometry.Vector3 v1, Internal.SharpNav.Geometry.Vector3 v2, float s )
			{
				dest.X = v1.X + v2.X * s;
				dest.Y = v1.Y + v2.Y * s;
				dest.Z = v1.Z + v2.Z * s;
			}

			static bool GetSteerTarget( NavMeshQuery navMeshQuery, Internal.SharpNav.Geometry.Vector3 startPos, Internal.SharpNav.Geometry.Vector3 endPos, float minTargetDist, Internal.SharpNav.Pathfinding.Path path, ref Internal.SharpNav.Geometry.Vector3 steerPos, ref StraightPathFlags steerPosFlag, ref NavPolyId steerPosRef )
			{
				StraightPath steerPath = new StraightPath();
				navMeshQuery.FindStraightPath( startPos, endPos, path, steerPath, 0 );
				int nsteerPath = steerPath.Count;
				if( nsteerPath == 0 )
					return false;

				//find vertex far enough to steer to
				int ns = 0;
				while( ns < nsteerPath )
				{
					if( ( steerPath[ ns ].Flags & StraightPathFlags.OffMeshConnection ) != 0 ||
						!InRange( steerPath[ ns ].Point.Position, startPos, minTargetDist, 1000.0f ) )
						break;

					ns++;
				}

				//failed to find good point to steer to
				if( ns >= nsteerPath )
					return false;

				steerPos = steerPath[ ns ].Point.Position;
				steerPos.Y = startPos.Y;
				steerPosFlag = steerPath[ ns ].Flags;
				if( steerPosFlag == StraightPathFlags.None && ns == ( nsteerPath - 1 ) )
					steerPosFlag = StraightPathFlags.End; // otherwise seeks path infinitely!!!
				steerPosRef = steerPath[ ns ].Point.Polygon;

				return true;
			}

			static bool InRange( Internal.SharpNav.Geometry.Vector3 v1, Internal.SharpNav.Geometry.Vector3 v2, float r, float h )
			{
				float dx = v2.X - v1.X;
				float dy = v2.Y - v1.Y;
				float dz = v2.Z - v1.Z;
				return ( dx * dx + dz * dz ) < ( r * r ) && Math.Abs( dy ) < h;
			}

			protected override void Process()
			{
				owner.UpdateNavMesh();

				if( owner.tiledNavMesh != null )
				{
					if( owner.navMeshQueriesCreatedForMaxNodes != pathfindingMaxNodes )
						owner.freeNavMeshQueries.Clear();
					owner.navMeshQueriesCreatedForMaxNodes = pathfindingMaxNodes;

					var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = maxPathFindInParallel };
					Parallel.ForEach( contexts, parallelOptions, delegate ( FindPathContext context )
					{
						NavMeshQuery query = null;
						lock( owner.freeNavMeshQueries )
						{
							if( owner.freeNavMeshQueries.Count != 0 )
								query = owner.freeNavMeshQueries.Dequeue();
							else
								query = new NavMeshQuery( owner.tiledNavMesh, pathfindingMaxNodes );
						}

						try
						{
							var extents = ToSharpNav( context.PolygonPickExtents, true );
							var startPt = query.FindNearestPoly( ToSharpNav( context.Start ), extents );
							var endPt = query.FindNearestPoly( ToSharpNav( context.End ), extents );

							var filter = new NavQueryFilter();

							var path = new Internal.SharpNav.Pathfinding.Path();
							var found = query.FindPath( ref startPt, ref endPt, filter, path );

							Vector3[] pathResult = null;
							if( found )
							{
								//find a smooth path over the mesh surface
								int npolys = path.Count;
								Internal.SharpNav.Geometry.Vector3 iterPos = new Internal.SharpNav.Geometry.Vector3();
								Internal.SharpNav.Geometry.Vector3 targetPos = new Internal.SharpNav.Geometry.Vector3();
								query.ClosestPointOnPoly( startPt.Polygon, startPt.Position, ref iterPos );
								query.ClosestPointOnPoly( path[ npolys - 1 ], endPt.Position, ref targetPos );

								var smoothPath = new List<Internal.SharpNav.Geometry.Vector3>( context.MaxSmoothPath );
								smoothPath.Add( iterPos );

								//for( int n = 0; n < path.Count; n++ )
								//{
								//	Vector3 closest = Vector3.Zero;
								//	if( navMeshQuery.ClosestPointOnPoly( path[ n ], startPt.Position, ref closest ) )
								//		smoothPath.Add( closest );
								//}

								float stepSize = (float)context.StepSize;
								float slop = (float)context.Slop;

								while( npolys > 0 && smoothPath.Count < smoothPath.Capacity )
								{
									//find location to steer towards
									Internal.SharpNav.Geometry.Vector3 steerPos = new Internal.SharpNav.Geometry.Vector3();
									StraightPathFlags steerPosFlag = 0;
									NavPolyId steerPosRef = NavPolyId.Null;

									if( !GetSteerTarget( query, iterPos, targetPos, slop, path, ref steerPos, ref steerPosFlag, ref steerPosRef ) )
										break;

									bool endOfPath = ( steerPosFlag & StraightPathFlags.End ) != 0 ? true : false;
									bool offMeshConnection = ( steerPosFlag & StraightPathFlags.OffMeshConnection ) != 0 ? true : false;

									//find movement delta
									Internal.SharpNav.Geometry.Vector3 delta = steerPos - iterPos;
									float len = (float)Math.Sqrt( Internal.SharpNav.Geometry.Vector3.Dot( delta, delta ) );

									//if steer target is at end of path or off-mesh link
									//don't move past location
									if( ( endOfPath || offMeshConnection ) && len < stepSize )
										len = 1;
									else
										len = stepSize / len;

									Internal.SharpNav.Geometry.Vector3 moveTgt = new Internal.SharpNav.Geometry.Vector3();
									VMad( ref moveTgt, iterPos, delta, len );

									//move
									Internal.SharpNav.Geometry.Vector3 result = new Internal.SharpNav.Geometry.Vector3();
									List<NavPolyId> visited = new List<NavPolyId>( 16 );
									NavPoint startPoint = new NavPoint( path[ 0 ], iterPos );
									query.MoveAlongSurface( ref startPoint, ref moveTgt, out result, visited );
									path.FixupCorridor( visited );
									npolys = path.Count;
									float h = 0;
									query.GetPolyHeight( path[ 0 ], result, ref h );
									result.Y = h;
									iterPos = result;

									//handle end of path when close enough
									if( endOfPath && InRange( iterPos, steerPos, slop, 1.0f ) )
									{
										//reached end of path
										iterPos = targetPos;
										if( smoothPath.Count < smoothPath.Capacity )
											smoothPath.Add( iterPos );
										break;
									}

									//store results
									if( smoothPath.Count < smoothPath.Capacity )
										smoothPath.Add( iterPos );
								}

								pathResult = new Vector3[ smoothPath.Count ];
								for( int n = 0; n < pathResult.Length; n++ )
									pathResult[ n ] = ToEngine( smoothPath[ n ] );

								//if( pathCount > 0 )
								//{
								//	path = new Vec3[ pathCount ];
								//	//!!!!double support
								//	for( int n = 0; n < pathCount; n++ )
								//		path[ n ] = ToEngineVec3( pathPointer[ n ] );
								//}
								//else
								//	path = new Vec3[] { context.Start };
							}

							context.Path = pathResult;
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
					owner.UpdateNavMesh();

					if( owner.tiledNavMesh != null )
					{
						//!!!!separate by tiles?

						//!!!!capacity
						var result = new List<Vector3>( 1024 );

						foreach( var tile in owner.tiledNavMesh.Tiles )
						{
							//int capacity = 0;
							//for( int nPoly = 0; nPoly < tile.PolyCount; nPoly++ )
							//{
							//	var poly = tile.Polys[ nPoly ];
							//	if( poly.Area.IsWalkable )
							//	{
							//		for( int n = 2; n < poly.VertCount; n++ )
							//			capacity += 3;
							//	}
							//}

							for( int nPoly = 0; nPoly < tile.PolyCount; nPoly++ )
							{
								var poly = tile.Polys[ nPoly ];
								if( poly.Area.IsWalkable )
								{
									for( int n = 2; n < poly.VertCount; n++ )
									{
										int index0 = poly.Verts[ 0 ];
										int index1 = poly.Verts[ n ];
										int index2 = poly.Verts[ n - 1 ];
										//int index0 = poly.Verts[ 0 ];
										//int index1 = poly.Verts[ n - 1 ];
										//int index2 = poly.Verts[ n ];

										result.Add( ToEngine( tile.Verts[ index0 ] ) );
										result.Add( ToEngine( tile.Verts[ index1 ] ) );
										result.Add( ToEngine( tile.Verts[ index2 ] ) );
									}
								}
							}
						}

						//with indexes?
						owner.navigationMeshVertices = result.ToArray();
					}
				}
			}
		}

		/////////////////////////////////////////

		internal class TileGeometry
		{
			public Bounds bounds = Bounds.Cleared;
			public AreaData[] data = new AreaData[ 256 ];

			//

			public class AreaData
			{
				public OpenList<Vector3> vertices;
				//public OpenList<int> indices;

				public AreaData( bool createLists )
				{
					if( createLists )
					{
						vertices = new OpenList<Vector3>( 2048 );
						//indices = new OpenList<int>( 2048 );
					}
				}
			}

			public void Add( Vector3[] vertices, int[] indices, byte area, bool clipByBounds, Rectangle clipBounds )
			{
				if( indices.Length == 0 || vertices.Length == 0 )
					return;

				var newVertices = new List<Vector3>( vertices.Length );

				if( clipByBounds )
				{
					for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
					{
						var polygon = new Vector3[ 3 ];
						polygon[ 0 ] = vertices[ indices[ nTriangle * 3 + 0 ] ];
						polygon[ 1 ] = vertices[ indices[ nTriangle * 3 + 1 ] ];
						polygon[ 2 ] = vertices[ indices[ nTriangle * 3 + 2 ] ];

						//+X
						{
							var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Maximum, 0 ), new Vector3( -1, 0, 0 ) );
							polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
						}

						//+Y
						{
							var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Maximum, 0 ), new Vector3( 0, -1, 0 ) );
							polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
						}

						//-X
						{
							var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Minimum, 0 ), new Vector3( 1, 0, 0 ) );
							polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
						}

						//-Y
						{
							var plane = Plane.FromPointAndNormal( new Vector3( clipBounds.Minimum, 0 ), new Vector3( 0, 1, 0 ) );
							polygon = MathAlgorithms.ClipPolygonByPlane( polygon, plane );
						}

						//triangulate
						for( int n = 1; n < polygon.Length - 1; n++ )
						{
							var v1 = polygon[ 0 ];
							var v2 = polygon[ n ];
							var v3 = polygon[ n + 1 ];

							if( v1 != v2 && v1 != v3 && v2 != v3 )
							{
								newVertices.Add( v1 );
								newVertices.Add( v2 );
								newVertices.Add( v3 );
							}
						}
					}
				}
				else
				{
					for( int nTriangle = 0; nTriangle < indices.Length / 3; nTriangle++ )
					{
						var v1 = vertices[ indices[ nTriangle * 3 + 0 ] ];
						var v2 = vertices[ indices[ nTriangle * 3 + 1 ] ];
						var v3 = vertices[ indices[ nTriangle * 3 + 2 ] ];

						if( v1 != v2 && v1 != v3 && v2 != v3 )
						{
							newVertices.Add( v1 );
							newVertices.Add( v2 );
							newVertices.Add( v3 );
						}
					}
				}

				if( newVertices.Count == 0 )
					return;

				var areaData = data[ area ];
				if( areaData == null )
				{
					areaData = new AreaData( true );
					data[ area ] = areaData;
				}

				areaData.vertices.AddRange( newVertices );
				//{
				//	var startVertexIndex = areaData.vertices.Count;

				//	areaData.vertices.AddRange( vertices );
				//	foreach( var index in indices )
				//		areaData.indices.Add( startVertexIndex + index );
				//}

				bounds.Add( newVertices );
			}

			public bool IsEmpty
			{
				get
				{
					for( int n = 0; n < 256; n++ )
					{
						if( data[ n ] != null )
							return false;
					}
					return true;
				}
			}

			public bool Load( TextBlock block )
			{
				try
				{
					foreach( var areaBlock in block.Children )
					{
						if( areaBlock.Name == "Area" )
						{
							var areaData = new AreaData( false );

							var index = int.Parse( areaBlock.GetAttribute( "Index" ) );

							var vertices = CollectionUtility.FromByteArray<Vector3>( Convert.FromBase64String( areaBlock.GetAttribute( "Vertices" ) ) );
							//var indices = CollectionUtility.FromByteArray<int>( Convert.FromBase64String( areaBlock.GetAttribute( "Indices" ) ) );

							areaData.vertices = new OpenList<Vector3>( vertices );
							//areaData.indices = new OpenList<int>( indices );

							data[ index ] = areaData;

							bounds.Add( vertices );
						}
					}
				}
				catch
				{
					return false;
				}
				return true;
			}

			public void Save( TextBlock block )
			{
				for( int n = 0; n < 256; n++ )
				{
					var areaData = data[ n ];
					if( areaData != null && areaData.vertices.Count != 0 )
					{
						var areaBlock = block.AddChild( "Area" );

						areaBlock.SetAttribute( "Index", n.ToString() );
						areaBlock.SetAttribute( "Vertices", Convert.ToBase64String( CollectionUtility.ToByteArray( areaData.vertices.ToArray() ), Base64FormattingOptions.None ) );
						//areaBlock.SetAttribute( "Indices", Convert.ToBase64String( CollectionUtility.ToByteArray( areaData.indices.ToArray() ), Base64FormattingOptions.None ) );
					}
				}
			}
		}

		/////////////////////////////////////////

		public class DynamicObstacleData
		{
			public Bounds Bounds;
			public byte Area;
			public Vector3[] Vertices;
			public int[] Indices;
		}

		///////////////////////////////////////////

		static Pathfinding()
		{
			instancesReadOnly = new ReadOnlyCollection<Pathfinding>( instances );
		}

		public Pathfinding()
		{
		}

		public static IList<Pathfinding> Instances
		{
			get { return instancesReadOnly; }
		}

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			var p = member as Metadata.Property;
			if( p != null )
			{
				switch( p.Name )
				{
				case nameof( TileSizeInCells ):
					if( !Tiles )
						skip = true;
					break;
				}
			}
		}

		//!!!!double

		static Internal.SharpNav.Geometry.Vector3 ToSharpNav( Vector3 v, bool abs = false )
		{
			if( abs )
				return new Internal.SharpNav.Geometry.Vector3( (float)Math.Abs( v.X ), (float)Math.Abs( v.Z ), (float)Math.Abs( v.Y ) );
			else
				return new Internal.SharpNav.Geometry.Vector3( (float)v.X, (float)v.Z, (float)v.Y );
		}

		static BBox3 ToSharpNav( Bounds v )
		{
			var min = new Vector3( v.Minimum.X, v.Minimum.Z, v.Minimum.Y );
			var max = new Vector3( v.Maximum.X, v.Maximum.Z, v.Maximum.Y );

			var b = new Bounds( min );
			b.Add( max );
			return new BBox3( (float)b.Minimum.X, (float)b.Minimum.Y, (float)b.Minimum.Z, (float)b.Maximum.X, (float)b.Maximum.Y, (float)b.Maximum.Z );
		}

		static Vector3F ToEngine( Internal.SharpNav.Geometry.Vector3 v )
		{
			return new Vector3F( v.X, v.Z, v.Y );
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			if( EnabledInHierarchyAndIsInstance )
				instances.Add( this );

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
			{
				firstOnUpdateAfterEnabledInHierarchy = true;
				//AddCommandsToUpdateDynamicObstacles();
				//DoForceUpdate( false );
			}

			if( !EnabledInHierarchyAndIsInstance )
				instances.Remove( this );
		}

		Bounds GetGeometriesBoundsForNavMesh()
		{
			var result = Bounds.Cleared;

			var scene = FindParent<Scene>();
			if( scene != null )
			{
				//Geometry
				foreach( var geometry in scene.GetComponents<PathfindingGeometry>( false, true, true ) )
				{
					var type = geometry.Type.Value;
					if( type == PathfindingGeometry.TypeEnum.BakedObstacle )
						result.Add( geometry.SpaceBounds.BoundingBox );
				}

				foreach( var geometryTag in scene.GetComponents<PathfindingGeometryTag>( false, true, true ) )
				{
					var type = geometryTag.Type.Value;
					if( type == PathfindingGeometryTag.TypeEnum.BakedObstacle )
					{
						//MeshInSpace
						var meshInSpace = geometryTag.Parent as MeshInSpace;
						if( meshInSpace != null )
						{
							var mesh = meshInSpace.Mesh.Value;
							if( mesh != null && mesh.Result != null )
							{
								var b = meshInSpace.SpaceBounds.BoundingBox;
								if( !b.IsCleared() )
									result.Add( b );
							}
						}

						//Terrain
						var terrain = geometryTag.Parent as Terrain;
						if( terrain != null )
						{
							var b = terrain.GetBoundsFromTiles();
							if( !b.IsCleared() )
								result.Add( b );
						}
					}
				}
			}

			return result;
		}

		List<Component> GetAllBakedGeometriesAndGeometryTags()
		{
			var result = new List<Component>( 128 );

			var scene = FindParent<Scene>();
			if( scene != null )
			{
				foreach( var geometry in scene.GetComponents<PathfindingGeometry>( false, true, true ) )
				{
					var type = geometry.Type.Value;
					if( type == PathfindingGeometry.TypeEnum.BakedObstacle )
						result.Add( geometry );
				}

				foreach( var geometryTag in scene.GetComponents<PathfindingGeometryTag>( false, true, true ) )
				{
					var type = geometryTag.Type.Value;
					if( type == PathfindingGeometryTag.TypeEnum.BakedObstacle )
						result.Add( geometryTag );
				}
			}

			return result;
		}

		TileGeometry GetGeometriesForNavMesh( List<Component> allBakedGeometriesAndGeometryTags, bool clipByBounds, Rectangle clipBounds )
		{
			var result = new TileGeometry();

			foreach( var obj in allBakedGeometriesAndGeometryTags )
			{
				//Geometry
				var geometry = obj as PathfindingGeometry;
				if( geometry != null && ( !clipByBounds || geometry.SpaceBounds.BoundingBox.ToRectangle().Intersects( clipBounds ) ) )
				{
					var type = geometry.Type.Value;
					if( type == PathfindingGeometry.TypeEnum.BakedObstacle )
					{
						geometry.GetGeometry( out var vertices, out var indices );
						if( vertices != null )
							result.Add( vertices, indices, (byte)geometry.Area, clipByBounds, clipBounds );
					}
				}

				var geometryTag = obj as PathfindingGeometryTag;
				if( geometryTag != null )
				{
					var type = geometryTag.Type.Value;
					if( type == PathfindingGeometryTag.TypeEnum.BakedObstacle )
					{
						//MeshInSpace
						var meshInSpace = geometryTag.Parent as MeshInSpace;
						if( meshInSpace != null && ( !clipByBounds || meshInSpace.SpaceBounds.BoundingBox.ToRectangle().Intersects( clipBounds ) ) )
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

								result.Add( vertices, mesh.Result.ExtractedIndices, (byte)geometryTag.Area, clipByBounds, clipBounds );
							}
						}

						//Terrain
						var terrain = geometryTag.Parent as Terrain;
						if( terrain != null && ( !clipByBounds || terrain.GetBounds2().Intersects( clipBounds ) ) )
						{
							terrain.GetGeometryFromTiles( delegate ( SpaceBounds tileBounds, Vector3[] tileVertices, int[] tileIndices )
							{
								if( !clipByBounds || tileBounds.BoundingBox.ToRectangle().Intersects( clipBounds ) )
									result.Add( tileVertices, tileIndices, (byte)geometryTag.Area, clipByBounds, clipBounds );
							} );
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

			bool display = AlwaysDisplayNavMesh || context2.selectedObjects.Contains( this );// || context.canSelectObjects.Contains( this ) ;
			if( display )
				DebugDrawNavMesh( viewport );

			////!!!!
			//if( tempDebugIndices != null )
			//{
			//	var transform = Matrix4.FromTranslate( new Vector3( 0, 0, 0.05 ) );

			//	var renderer = viewport.Simple3DRenderer;

			//	renderer.SetColor( new ColorValue( 0, 0, 1, 1 ), new ColorValue( 0, 0, 1, 0.3 ) );
			//	renderer.AddTriangles( tempDebugVertices, tempDebugIndices, transform, false, true );

			//	renderer.SetColor( new ColorValue( 1, 0, 0, 1 ), new ColorValue( 1, 0, 0, 0.3 ) );
			//	renderer.AddTriangles( tempDebugVertices, tempDebugIndices, transform, true, true );
			//}
		}

		public void DebugDrawNavMesh( Viewport viewport )
		{
			var backgroundThreadData = GetBackgroundThreadData();
			if( backgroundThreadData != null )
			{
				var vertices = backgroundThreadData.navigationMeshVertices;

				if( vertices == null )
					NeedGetNavigationMeshVertices( false );

				if( vertices != null )
				{
					//render
					var transform = Matrix4.FromTranslate( new Vector3( 0, 0, 0.05 ) );
					var renderer = viewport.Simple3DRenderer;
					renderer.SetColor( new ColorValue( 0, 1, 0, 0.3 ), new ColorValue( 0, 1, 0, 0.1 ) );
					renderer.AddTriangles( vertices, ref transform, false, true );
					renderer.SetColor( new ColorValue( 1, 1, 0, 0.3 ), new ColorValue( 1, 1, 0, 0.1 ) );
					renderer.AddTriangles( vertices, ref transform, true, true );
				}
			}
		}

		void AddCommandsToUpdateDynamicObstacles()
		{
			var scene = FindParent<Scene>();
			if( scene != null )
			{
				foreach( var geometry in scene.GetComponents<PathfindingGeometry>( false, true, true ) )
				{
					var type = geometry.Type.Value;
					if( type == PathfindingGeometry.TypeEnum.DynamicObstacle )
						geometry.UpdateDynamicObstacle( true );
				}

				//foreach( var geometryTag in scene.GetComponents<Pathfinding_GeometryTag>( false, true, true ) )
				//{
				//	var type = geometryTag.Type.Value;
				//	if( type == Pathfinding_GeometryTag.TypeEnum.WalkableArea || type == Pathfinding_GeometryTag.TypeEnum.BakedObstacle )
				//		AddGeometryTagToCollector( collector, geometryTag );
				//}
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
			newData.tiles = Tiles;
			newData.tileSizeInCells = TileSizeInCells;
			newData.cellSize = CellSize;
			newData.cellHeight = CellHeight;

			var allBakedGeometriesAndGeometryTags = GetAllBakedGeometriesAndGeometryTags();

			if( Tiles )
			{
				var totalBounds = GetGeometriesBoundsForNavMesh();

				var tileIndexRange = newData.GetTileIndexRangeByBounds( totalBounds.ToRectangle() );
				for( int y = tileIndexRange.Minimum.Y; y < tileIndexRange.Maximum.Y; y++ )
				{
					for( int x = tileIndexRange.Minimum.X; x < tileIndexRange.Maximum.X; x++ )
					{
						var index = new Vector2I( x, y );
						var tileBounds = newData.GetTileBounds( index );

						var tileGeometry = GetGeometriesForNavMesh( allBakedGeometriesAndGeometryTags, true, tileBounds );

						var tile = new PrecompiledDataClass.TileData();
						tile.index = index;
						if( !tileGeometry.IsEmpty )
							tile.staticGeometry = tileGeometry;
						newData.precompiledTiles.Add( tile.index, tile );
					}
				}

				//if( newData.precompiledTiles.Count == 0 )
				//{
				//	error = "No vertices were gathered from collision objects.";
				//	return false;
				//}
			}
			else
			{
				var tileGeometry = GetGeometriesForNavMesh( allBakedGeometriesAndGeometryTags, false, new Rectangle( double.MinValue, double.MinValue, double.MaxValue, double.MaxValue ) );
				//if( tileGeometry.IsEmpty )
				//{
				//	error = "No vertices were gathered from collision objects.";
				//	return false;
				//}

				var tile = new PrecompiledDataClass.TileData();
				tile.index = Vector2I.Zero;
				tile.staticGeometry = tileGeometry;
				newData.precompiledTiles.Add( tile.index, tile );
			}

			//!!!!
			//{
			//	var scene = FindParent<Scene>();

			//	var object1 = scene.GetComponent( "Object In Space 3" ) as ObjectInSpace;
			//	var object2 = scene.GetComponent( "Object In Space 4" ) as ObjectInSpace;

			//	if( object1 != null && object2 != null )
			//	{
			//		var connection = new PrecompiledDataClass.MeshOffLinkConnection();
			//		connection.Position1 = object1.TransformV.Position;
			//		connection.Position2 = object2.TransformV.Position;

			//		//!!!!

			//		newData.meshOffLinkConnections.Add( connection );
			//	}
			//}

			PrecompiledData = newData;

			//update dynamic obstacles
			AddCommandsToUpdateDynamicObstacles();

			return true;
		}

		BackgroundThreadData GetBackgroundThreadData()
		{
			if( precompiledData != null && EnabledInHierarchy ) //!!!!new EnabledInHierarchy
			{
				if( backgroundThreadData == null || backgroundThreadData.precompiledData != precompiledData )
					backgroundThreadData = new BackgroundThreadData( precompiledData );
			}
			else
				backgroundThreadData = null;

			return backgroundThreadData;
		}

		protected override void OnUpdate( float delta )
		{
			base.OnUpdate( delta );

			if( EnabledInHierarchy ) //!!!!new EnabledInHierarchy
			{
				if( firstOnUpdateAfterEnabledInHierarchy )
				{
					firstOnUpdateAfterEnabledInHierarchy = false;
					//AddCommandsToUpdateDynamicObstacles();
					DoForceUpdate( false );
				}
			}

			var backgroundThreadData = GetBackgroundThreadData();
			backgroundThreadData?.UpdateFromMainThread();
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
			command.pathfindingMaxNodes = PathfindingMaxNodes;
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

		public void DynamicObstacleAdd( DynamicObstacleData dynamicObstacle, bool wait )
		{
			var command = new CommandDynamicObstacleAdd();
			command.dynamicObstacle = dynamicObstacle;
			AddCommand( command, wait, false );
		}

		public void DynamicObstacleDelete( DynamicObstacleData dynamicObstacle, bool wait )
		{
			var command = new CommandDynamicObstacleDelete();
			command.dynamicObstacle = dynamicObstacle;
			AddCommand( command, wait, false );
		}

		protected override bool OnLoad( Metadata.LoadContext context, TextBlock block, out string error )
		{
			if( !base.OnLoad( context, block, out error ) )
				return false;

			var blockData = block.FindChild( "PrecompiledData" );
			if( blockData != null )
			{
				var precompiledData = new PrecompiledDataClass();
				if( !precompiledData.Load( blockData ) )
					return false;
				PrecompiledData = precompiledData;
			}

			return true;
		}

		protected override bool OnSave( Metadata.SaveContext context, TextBlock block, ref bool skipSave, out string error )
		{
			if( !base.OnSave( context, block, ref skipSave, out error ) )
				return false;

			if( PrecompiledData != null )
			{
				var blockData = block.AddChild( "PrecompiledData" );
				PrecompiledData.Save( blockData );
			}

			return true;
		}

		public void DoForceUpdate( bool wait )
		{
			var command = new CommandForceUpdate();
			AddCommand( command, wait, true );
		}

	}
}
