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
using SharpNav.IO.Binary;
using System.IO;

namespace NeoAxis.Addon.Pathfinding
{
	/// <summary>
	/// Navigation mesh pathfinding.
	/// </summary>
	[AddToResourcesWindow( @"Base\Game framework\Pathfinding", -9996 )]
	[EditorSettingsCell( typeof( Component_Pathfinding_SettingsCell ) )]
	public class Component_Pathfinding : Component_ObjectInSpace
	{
		static List<Component_Pathfinding> instances = new List<Component_Pathfinding>();
		static ReadOnlyCollection<Component_Pathfinding> instancesReadOnly;

		TiledNavMesh tiledNavMesh;
		NavMeshQuery navMeshQuery;

		//debug draw
		Vector3F[] debugNavigationMeshVertices;
		//volatile bool debugNavigationMeshDirty;
		////Vec3[] tileGridMeshVertices;
		////int[] tileGridMeshIndices;
		////Vec3[] cellGridMeshVertices;
		////int[] cellGridMeshIndices;
		////bool drawTileGrid;

		/////////////////////////////////////////

		//!!!!
		///// <summary>
		///// The size of a tile.
		///// </summary>
		//[DefaultValue( 32 )]
		//[Serialize]
		//[Category( "Grid" )]
		//public Reference<int> TileSize
		//{
		//	get { if( _tileSize.BeginGet() ) TileSize = _tileSize.Get( this ); return _tileSize.value; }
		//	set
		//	{
		//		if( _tileSize.BeginSet( ref value ) )
		//		{
		//			if( value < 16 )
		//				value = new Reference<int>( 16, value.GetByReference );
		//			try { TileSizeChanged?.Invoke( this ); } finally { _tileSize.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Pathfinding> TileSizeChanged;
		//ReferenceField<int> _tileSize = 32;

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
				if( _cellSize.BeginSet( ref value ) )
				{
					if( value < 0.01 )
						value = new Reference<double>( 0.01, value.GetByReference );
					try { CellSizeChanged?.Invoke( this ); } finally { _cellSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CellSize"/> property value changes.</summary>
		public event Action<Component_Pathfinding> CellSizeChanged;
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
				if( _cellHeight.BeginSet( ref value ) )
				{
					if( value < 0.01 )
						value = new Reference<double>( 0.01, value.GetByReference );
					try { CellHeightChanged?.Invoke( this ); } finally { _cellHeight.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="CellHeight"/> property value changes.</summary>
		public event Action<Component_Pathfinding> CellHeightChanged;
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
		//		if( _trianglesPerChunk.BeginSet( ref value ) )
		//		{
		//			if( value < 128 )
		//				value = new Reference<int>( 128, value.GetByReference );
		//			try { TrianglesPerChunkChanged?.Invoke( this ); } finally { _trianglesPerChunk.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Pathfinding> TrianglesPerChunkChanged;
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
				if( _minRegionSize.BeginSet( ref value ) )
				{
					if( value < 1 )
						value = new Reference<int>( 1, value.GetByReference );
					try { MinRegionSizeChanged?.Invoke( this ); } finally { _minRegionSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MinRegionSize"/> property value changes.</summary>
		public event Action<Component_Pathfinding> MinRegionSizeChanged;
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
				if( _mergedRegionSize.BeginSet( ref value ) )
				{
					if( value < 0 )
						value = new Reference<int>( 0, value.GetByReference );
					try { MergedRegionSizeChanged?.Invoke( this ); } finally { _mergedRegionSize.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MergedRegionSize"/> property value changes.</summary>
		public event Action<Component_Pathfinding> MergedRegionSizeChanged;
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
				if( _maxEdgeLength.BeginSet( ref value ) )
				{
					if( value < 0 )
						value = new Reference<int>( 0, value.GetByReference );
					try { MaxEdgeLengthChanged?.Invoke( this ); } finally { _maxEdgeLength.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaxEdgeLength"/> property value changes.</summary>
		public event Action<Component_Pathfinding> MaxEdgeLengthChanged;
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
				if( _maxEdgeError.BeginSet( ref value ) )
				{
					if( value < 0.1 )
						value = new Reference<double>( 0.1, value.GetByReference );
					try { MaxEdgeErrorChanged?.Invoke( this ); } finally { _maxEdgeError.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaxEdgeError"/> property value changes.</summary>
		public event Action<Component_Pathfinding> MaxEdgeErrorChanged;
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
				if( _maxVerticesPerPolygon.BeginSet( ref value ) )
				{
					if( value < 3 )
						value = new Reference<int>( 3, value.GetByReference );
					try { MaxVerticesPerPolygonChanged?.Invoke( this ); } finally { _maxVerticesPerPolygon.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="MaxVerticesPerPolygon"/> property value changes.</summary>
		public event Action<Component_Pathfinding> MaxVerticesPerPolygonChanged;
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
				if( _detailSampleDistance.BeginSet( ref value ) )
				{
					if( value < 0.0 )
						value = new Reference<int>( 0, value.GetByReference );
					try { DetailSampleDistanceChanged?.Invoke( this ); } finally { _detailSampleDistance.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DetailSampleDistance"/> property value changes.</summary>
		public event Action<Component_Pathfinding> DetailSampleDistanceChanged;
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
				if( _detailMaxSampleError.BeginSet( ref value ) )
				{
					if( value < 0.0 )
						value = new Reference<int>( 0, value.GetByReference );
					try { DetailMaxSampleErrorChanged?.Invoke( this ); } finally { _detailMaxSampleError.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="DetailMaxSampleError"/> property value changes.</summary>
		public event Action<Component_Pathfinding> DetailMaxSampleErrorChanged;
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
				if( _agentHeight.BeginSet( ref value ) )
				{
					if( value < 0.1 )
						value = new Reference<double>( 0.1, value.GetByReference );
					try { AgentHeightChanged?.Invoke( this ); } finally { _agentHeight.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AgentHeight"/> property value changes.</summary>
		public event Action<Component_Pathfinding> AgentHeightChanged;
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
				if( _agentRadius.BeginSet( ref value ) )
				{
					if( value < 0.0 )
						value = new Reference<double>( 0.0, value.GetByReference );
					try { AgentRadiusChanged?.Invoke( this ); } finally { _agentRadius.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AgentRadius"/> property value changes.</summary>
		public event Action<Component_Pathfinding> AgentRadiusChanged;
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
				if( _agentMaxClimb.BeginSet( ref value ) )
				{
					if( value < 0.001 )
						value = new Reference<double>( 0.001, value.GetByReference );
					try { AgentMaxClimbChanged?.Invoke( this ); } finally { _agentMaxClimb.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="AgentMaxClimb"/> property value changes.</summary>
		public event Action<Component_Pathfinding> AgentMaxClimbChanged;
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
		//		if( _agentMaxSlope.BeginSet( ref value ) )
		//		{
		//			if( value.Value < 1 )
		//				value = new Reference<Degree>( 1, value.GetByReference );
		//			if( value.Value > 89 )
		//				value = new Reference<Degree>( 89, value.GetByReference );
		//			try { AgentMaxSlopeChanged?.Invoke( this ); } finally { _agentMaxSlope.EndSet(); }
		//		}
		//	}
		//}
		//public event Action<Component_Pathfinding> AgentMaxSlopeChanged;
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
				if( _pathfindingMaxNodes.BeginSet( ref value ) )
				{
					if( value < 4 )
						value = new Reference<int>( 4, value.GetByReference );
					if( value > 65536 )
						value = new Reference<int>( 65536, value.GetByReference );
					try
					{
						PathfindingMaxNodesChanged?.Invoke( this );
						navMeshQuery = null;
					}
					finally { _pathfindingMaxNodes.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="PathfindingMaxNodes"/> property value changes.</summary>
		public event Action<Component_Pathfinding> PathfindingMaxNodesChanged;
		ReferenceField<int> _pathfindingMaxNodes = 4096;

		[Category( "Debug" )]
		[DefaultValue( false )]
		[Serialize]
		public Reference<bool> AlwaysDisplayNavMesh
		{
			get { if( _alwaysDisplayNavMesh.BeginGet() ) AlwaysDisplayNavMesh = _alwaysDisplayNavMesh.Get( this ); return _alwaysDisplayNavMesh.value; }
			set { if( _alwaysDisplayNavMesh.BeginSet( ref value ) ) { try { AlwaysDisplayNavMeshChanged?.Invoke( this ); } finally { _alwaysDisplayNavMesh.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AlwaysDisplayNavMesh"/> property value changes.</summary>
		public event Action<Component_Pathfinding> AlwaysDisplayNavMeshChanged;
		ReferenceField<bool> _alwaysDisplayNavMesh = false;

		const int navMeshDataVersion = 1;
		byte[] navMeshData;
		[Browsable( false )]
		[Serialize]
		public byte[] NavMeshData
		{
			get { return navMeshData; }
			set
			{
				navMeshData = value;

				if( EnabledInHierarchy )
				{
					if( NavMeshData != null )
					{
						if( !InitNavMesh( out var error ) )
							Log.Error( "Component_Pathfinding: Error: " + error );
					}
					else
						DestroyNavMesh();
				}
			}
		}

		/////////////////////////////////////////

		class IndexVertexBufferCollector
		{
			//!!!!memory. mesh instancing
			public Vector3[] resultVertices = new Vector3[ 4096 ];
			public int[] resultIndices = new int[ 4096 ];
			public int resultVertexCount;
			public int resultIndexCount;

			public void Add( Vector3[] vertices, /*int vertexCount, */int[] indices/*, int indexCount*/ )
			{
				var vertexCount = vertices.Length;
				var indexCount = indices.Length;

				int newVertexCount = resultVertexCount + vertexCount;
				int newIndexCount = resultIndexCount + indexCount;

				if( newVertexCount > resultVertices.Length )
				{
					int s = resultVertices.Length;
					while( newVertexCount > s )
						s *= 2;
					Vector3[] old = resultVertices;
					resultVertices = new Vector3[ s ];
					Array.Copy( old, resultVertices, old.Length );
				}

				if( newIndexCount > resultIndices.Length )
				{
					int s = resultIndices.Length;
					while( newIndexCount > s )
						s *= 2;
					int[] old = resultIndices;
					resultIndices = new int[ s ];
					Array.Copy( old, resultIndices, old.Length );
				}

				Array.Copy( vertices, 0, resultVertices, resultVertexCount, vertexCount );
				for( int n = 0; n < indexCount; n++ )
					resultIndices[ resultIndexCount + n ] = resultVertexCount + indices[ n ];
				resultVertexCount = newVertexCount;
				resultIndexCount = newIndexCount;
			}
		}

		///////////////////////////////////////////

		static Component_Pathfinding()
		{
			instancesReadOnly = new ReadOnlyCollection<Component_Pathfinding>( instances );
		}

		public Component_Pathfinding()
		{
		}

		public static IList<Component_Pathfinding> Instances
		{
			get { return instancesReadOnly; }
		}

		//!!!!double

		static SharpNav.Geometry.Vector3 ToSharpNav( Vector3 v, bool abs = false )
		{
			if( abs )
				return new SharpNav.Geometry.Vector3( (float)Math.Abs( v.X ), (float)Math.Abs( v.Z ), (float)Math.Abs( v.Y ) );
			else
				return new SharpNav.Geometry.Vector3( (float)v.X, (float)v.Z, (float)-v.Y );
		}

		static BBox3 ToSharpNav( Bounds v )
		{
			var min = new Vector3( v.Minimum.X, v.Minimum.Z, -v.Minimum.Y );
			var max = new Vector3( v.Maximum.X, v.Maximum.Z, -v.Maximum.Y );

			var b = new Bounds( min );
			b.Add( max );
			return new BBox3( (float)b.Minimum.X, (float)b.Minimum.Y, (float)b.Minimum.Z, (float)b.Maximum.X, (float)b.Maximum.Y, (float)b.Maximum.Z );
		}

		static Vector3F ToEngine( SharpNav.Geometry.Vector3 v )
		{
			return new Vector3F( v.X, -v.Z, v.Y );
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			if( EnabledInHierarchy )
				instances.Add( this );

			base.OnEnabledInHierarchyChanged();

			var scene = ParentScene;
			if( scene != null )
			{
				if( EnabledInHierarchy )
					scene.RenderEvent += Scene_RenderEvent;
				else
					scene.RenderEvent -= Scene_RenderEvent;
			}

			if( EnabledInHierarchy )
			{
				if( NavMeshData != null )
				{
					if( !InitNavMesh( out var error ) )
						Log.Error( "Component_Pathfinding: Error: " + error );
				}
			}
			else
				DestroyNavMesh();

			if( !EnabledInHierarchy )
				instances.Remove( this );
		}

		void AddGeometryToCollector( IndexVertexBufferCollector collector, Component_Pathfinding_Geometry geometry )
		{
			geometry.GetGeometry( out var vertices, out var indices );
			if( vertices != null )
				collector.Add( vertices, indices );
		}

		void AddGeometryTagToCollector( IndexVertexBufferCollector collector, Component_Pathfinding_GeometryTag geometryTag )
		{
			//!!!!

			//_MeshInSpace
			var meshInSpace = geometryTag.Parent as Component_MeshInSpace;
			if( meshInSpace != null )
			{
				var mesh = meshInSpace.Mesh.Value;
				if( mesh != null && mesh.Result != null )
				{
					var transform = meshInSpace.Transform.Value.ToMatrix4();

					var vertices = new Vector3[ mesh.Result.ExtractedVerticesPositions.Length ];
					for( int n = 0; n < vertices.Length; n++ )
						vertices[ n ] = transform * mesh.Result.ExtractedVerticesPositions[ n ].ToVector3();

					collector.Add( vertices, mesh.Result.ExtractedIndices );
				}
			}

			////HeightmapTerrain
			//HeightmapTerrain terrain = entity as HeightmapTerrain;
			//if( terrain != null && terrain.Enabled )
			//{
			//	int size = terrain.GetHeightmapSizeAsInteger();

			//	Vec3[] vertices = new Vec3[ ( size + 1 ) * ( size + 1 ) ];
			//	int[] indices = new int[ size * size * 6 ];

			//	int vertexPosition = 0;
			//	int indexPosition = 0;

			//	for( int y = 0; y < size + 1; y++ )
			//	{
			//		for( int x = 0; x < size + 1; x++ )
			//		{
			//			//if( !terrain.GetHoleFlag( new Vec2i( x, y ) ) )
			//			//{
			//			Vec2 pos2 = terrain.GetPositionXY( new Vec2I( x, y ) );
			//			Vec3 pos = new Vec3( pos2.X, pos2.Y, terrain.GetHeight( new Vec2I( x, y ) ) );
			//			vertices[ vertexPosition ] = pos;
			//			vertexPosition++;
			//			//}
			//		}
			//	}

			//	for( int y = 0; y < size; y++ )
			//	{
			//		for( int x = 0; x < size; x++ )
			//		{
			//			if( !terrain.GetHoleFlag( new Vec2I( x, y ) ) )
			//			{
			//				indices[ indexPosition + 0 ] = ( size + 1 ) * y + x;
			//				indices[ indexPosition + 1 ] = ( size + 1 ) * y + x + 1;
			//				indices[ indexPosition + 2 ] = ( size + 1 ) * ( y + 1 ) + x + 1;
			//				indices[ indexPosition + 3 ] = ( size + 1 ) * ( y + 1 ) + x + 1;
			//				indices[ indexPosition + 4 ] = ( size + 1 ) * ( y + 1 ) + x;
			//				indices[ indexPosition + 5 ] = ( size + 1 ) * y + x;
			//				indexPosition += 6;
			//			}
			//		}
			//	}

			//	collector.Add( vertices, vertexPosition, indices, indexPosition );
			//}
		}

		IndexVertexBufferCollector GetAllGeometriesForNavigationMesh()
		{
			var collector = new IndexVertexBufferCollector();

			var scene = FindParent<Component_Scene>();
			if( scene != null )
			{
				foreach( var geometry in scene.GetComponents<Component_Pathfinding_Geometry>( false, true, true ) )
				{
					var type = geometry.Type.Value;
					if( type == Component_Pathfinding_Geometry.TypeEnum.WalkableArea || type == Component_Pathfinding_Geometry.TypeEnum.BakedObstacle )
						AddGeometryToCollector( collector, geometry );
				}

				foreach( var geometryTag in scene.GetComponents<Component_Pathfinding_GeometryTag>( false, true, true ) )
				{
					var type = geometryTag.Type.Value;
					if( type == Component_Pathfinding_GeometryTag.TypeEnum.WalkableArea || type == Component_Pathfinding_GeometryTag.TypeEnum.BakedObstacle )
						AddGeometryTagToCollector( collector, geometryTag );
				}
			}

			return collector;
		}

		//!!!!
		//OnGetRenderSceneData		
		private void Scene_RenderEvent( Component_Scene sender, Viewport viewport )
		{
			//!!!!было
			////!!!!пока так
			//TempObstaclesUpdate( false );

			var context2 = viewport.RenderingContext.objectInSpaceRenderingContext;

			bool display = AlwaysDisplayNavMesh || context2.selectedObjects.Contains( this );// || context.canSelectObjects.Contains( this ) ;
			if( display )
			{
				DebugDrawNavMesh( viewport );

				//var renderer = context.viewport.Simple3DRenderer;
				//renderer.SetColor( new ColorValue( 0, 0, 1 ) );
				//Bounds bounds = new Bounds( boundsMin, boundsMax );
				//camera.DebugGeometry.AddBounds( bounds );
			}
		}

		public void DebugDrawNavMesh( Viewport viewport )
		{
			if( tiledNavMesh == null )
				return;

			//if( debugNavigationMeshDirty )
			//{
			//	debugNavigationMeshVertices = null;
			//	debugNavigationMeshIndices = null;
			//	debugNavigationMeshDirty = false;
			//}

			//get triangles for visualization
			if( debugNavigationMeshVertices == null )
				GetDebugNavigationMeshGeometry( out debugNavigationMeshVertices );

			//Render NavMesh
			if( debugNavigationMeshVertices != null )
			{
				var transform = Matrix4.FromTranslate( new Vector3( 0, 0, 0.05 ) );

				var renderer = viewport.Simple3DRenderer;

				renderer.SetColor( new ColorValue( 0, 1, 0, 0.3 ), new ColorValue( 0, 1, 0, 0.1 ) );
				renderer.AddTriangles( debugNavigationMeshVertices, transform, false, true );

				renderer.SetColor( new ColorValue( 1, 1, 0, 0.3 ), new ColorValue( 1, 1, 0, 0.1 ) );
				renderer.AddTriangles( debugNavigationMeshVertices, transform, true, true );
			}
		}

		//void DebugRenderTileGrid( Camera camera )
		//{
		//   //make a tile grid
		//   {
		//      if( tileGridMeshVertices == null )
		//         CreateTileGridMesh( out tileGridMeshVertices, out tileGridMeshIndices, false );

		//      camera.DebugGeometry.Color = new ColorValue( 0f, 1f, 0f, .3f );
		//      camera.DebugGeometry.AddVertexIndexBuffer( tileGridMeshVertices, tileGridMeshIndices,
		//         Mat4.Identity, true, false );
		//   }

		//   //make a cell grid
		//   {
		//      if( cellGridMeshVertices == null )
		//         CreateTileGridMesh( out cellGridMeshVertices, out cellGridMeshIndices, true );

		//      camera.DebugGeometry.Color = new ColorValue( 1f, 0f, 0f, .3f );
		//      camera.DebugGeometry.AddVertexIndexBuffer( cellGridMeshVertices, cellGridMeshIndices,
		//         Mat4.Identity, true, false );
		//   }

		//   //add the bounds
		//   {
		//      camera.DebugGeometry.Color = new ColorValue( 0f, 1f, 1f, .6f );
		//      camera.DebugGeometry.AddBounds( new Bounds( boundsMin, boundsMax ) );
		//   }
		//}

		//void CreateTileGridMesh( out Vec3[] vertices, out int[] indices, bool cellSplit )
		//{
		//   int size;
		//   if( cellSplit )
		//      size = 128;
		//   else
		//      size = 64;

		//   vertices = new Vec3[ ( size + 1 ) * ( size + 1 ) ];
		//   {
		//      int vertexPosition = 0;
		//      for( int y = 0; y < size + 1; y++ )
		//      {
		//         for( int x = 0; x < size + 1; x++ )
		//         {
		//            xx;
		//            if( cellSplit )
		//            {
		//               vertices[ vertexPosition ] = new Vec3(
		//                  boundsMin.X + x * tileSize * cellSize,
		//                  boundsMax.Y - y * tileSize * cellSize,
		//                  boundsMin.Z + gridHeight * ( boundsMax.Z - boundsMin.Z ) );
		//            }
		//            else
		//            {
		//               vertices[ vertexPosition ] = new Vec3(
		//                  boundsMin.X + x * tileSize,
		//                  boundsMax.Y - y * tileSize,
		//                  boundsMin.Z + gridHeight * ( boundsMax.Z - boundsMin.Z ) );
		//            }
		//            vertexPosition++;
		//         }
		//      }
		//   }

		//   indices = new int[ size * size * 6 ];
		//   {
		//      int indexPosition = 0;
		//      for( int y = 0; y < size; y++ )
		//      {
		//         for( int x = 0; x < size; x++ )
		//         {
		//            indices[ indexPosition + 0 ] = ( size + 1 ) * y + x;
		//            indices[ indexPosition + 1 ] = ( size + 1 ) * y + x + 1;
		//            indices[ indexPosition + 2 ] = ( size + 1 ) * ( y + 1 ) + x + 1;
		//            indices[ indexPosition + 3 ] = ( size + 1 ) * ( y + 1 ) + x + 1;
		//            indices[ indexPosition + 4 ] = ( size + 1 ) * ( y + 1 ) + x;
		//            indices[ indexPosition + 5 ] = ( size + 1 ) * y + x;
		//            indexPosition += 6;
		//         }
		//      }
		//   }
		//}

		void GetDebugNavigationMeshGeometry( out Vector3F[] vertices )
		{
			//WaitForExecutionAllQueuedOperations();

			//lock( nativeLock )
			//{

			//!!!!one tile
			var tile = tiledNavMesh.GetTileAt( 0, 0, 0 );

			int capacity = 0;
			for( int nPoly = 0; nPoly < tile.PolyCount; nPoly++ )
			{
				var poly = tile.Polys[ nPoly ];
				if( poly.Area.IsWalkable )
				{
					for( int n = 2; n < poly.VertCount; n++ )
						capacity += 3;
				}
			}

			var result = new List<Vector3F>( capacity );

			for( int nPoly = 0; nPoly < tile.PolyCount; nPoly++ )
			{
				var poly = tile.Polys[ nPoly ];
				if( poly.Area.IsWalkable )
				{
					for( int n = 2; n < poly.VertCount; n++ )
					{
						int index0 = poly.Verts[ 0 ];
						int index1 = poly.Verts[ n - 1 ];
						int index2 = poly.Verts[ n ];

						result.Add( ToEngine( tile.Verts[ index0 ] ) );
						result.Add( ToEngine( tile.Verts[ index1 ] ) );
						result.Add( ToEngine( tile.Verts[ index2 ] ) );
					}
				}
			}

			vertices = result.ToArray();
		}

		public bool BuildNavMesh( out string error )
		{
			DestroyNavMesh();

			if( !EnabledInHierarchy )
			{
				error = "Is not enabled.";
				return false;
			}

			//get geometry data
			var collector = GetAllGeometriesForNavigationMesh();
			Vector3[] vertices = collector.resultVertices;
			int[] indices = collector.resultIndices;
			int vertexCount = collector.resultVertexCount;
			int indexCount = collector.resultIndexCount;

			if( vertexCount == 0 )
			{
				error = "No vertices were gathered from collision objects.";
				return false;
			}

			//get settings
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

			TiledNavMesh newTiledNavMesh;

			try
			{
				//level.SetBoundingBoxOffset(new SVector3(settings.CellSize * 0.5f, settings.CellHeight * 0.5f, settings.CellSize * 0.5f));

				var bounds = Bounds.Cleared;
				bounds.Add( vertices );
				var heightfield = new Heightfield( ToSharpNav( bounds ), settings );

				var vertices2 = new SharpNav.Geometry.Vector3[ indexCount ];
				for( int index = 0; index < indexCount; index++ )
					vertices2[ index ] = ToSharpNav( vertices[ indices[ index ] ] );

				//Area[] areas = AreaGenerator.From( vertices2, Area.Default )
				//	.MarkBelowSlope( (float)AgentMaxSlope.Value.InRadians(), Area.Null )
				//	.ToArray();
				//Area[] areas = AreaGenerator.From(triEnumerable, Area.Default)
				//	.MarkAboveHeight(areaSettings.MaxLevelHeight, Area.Null)
				//	.MarkBelowHeight(areaSettings.MinLevelHeight, Area.Null)
				//	.MarkBelowSlope(areaSettings.MaxTriSlope, Area.Null)
				//	.ToArray();
				//heightfield.RasterizeTrianglesWithAreas( vertices2, areas );
				heightfield.RasterizeTriangles( vertices2, Area.Default );

				heightfield.FilterLedgeSpans( settings.VoxelAgentHeight, settings.VoxelMaxClimb );
				heightfield.FilterLowHangingWalkableObstacles( settings.VoxelMaxClimb );
				heightfield.FilterWalkableLowHeightSpans( settings.VoxelAgentHeight );

				var compactHeightfield = new CompactHeightfield( heightfield, settings );
				compactHeightfield.Erode( settings.VoxelAgentRadius );
				compactHeightfield.BuildDistanceField();
				compactHeightfield.BuildRegions( 0, settings.MinRegionSize, settings.MergedRegionSize );

				//!!!!
				System.Random r = new System.Random();
				var regionColors = new ColorByte[ compactHeightfield.MaxRegions ];
				regionColors[ 0 ] = new ColorByte( 0, 0, 0 );
				for( int i = 1; i < regionColors.Length; i++ )
					regionColors[ i ] = new ColorByte( (byte)r.Next( 0, 255 ), (byte)r.Next( 0, 255 ), (byte)r.Next( 0, 255 ), (byte)255 );

				var contourSet = compactHeightfield.BuildContourSet( settings );
				var polyMesh = new PolyMesh( contourSet, settings );
				var polyMeshDetail = new PolyMeshDetail( polyMesh, compactHeightfield, settings );

				var buildData = new NavMeshBuilder( polyMesh, polyMeshDetail, new OffMeshConnection[ 0 ], settings );
				newTiledNavMesh = new TiledNavMesh( buildData );

				//!!!!
				////Pathfinding with multiple units
				//GenerateCrowd();
			}
			catch( Exception e )
			{
				DestroyNavMesh();
				error = e.Message;
				return false;
			}

			int dataLength;
			byte[] data;

			using( var memoryStream = new MemoryStream() )
			{
				var serializer = new NavMeshBinarySerializer();
				serializer.Serialize( memoryStream, newTiledNavMesh );

				dataLength = (int)memoryStream.Length;
				data = memoryStream.GetBuffer();
			}

			//generate nav mesh data
			var writer = new ArrayDataWriter();
			writer.Write( navMeshDataVersion );
			writer.Write( dataLength );
			writer.Write( data, 0, dataLength );

			//set NavMeshData and init
			var newNavMeshData = new byte[ writer.BitLength / 8 ];
			Buffer.BlockCopy( writer.Data, 0, newNavMeshData, 0, newNavMeshData.Length );
			NavMeshData = newNavMeshData;

			error = "";
			return true;
		}

		public bool InitNavMesh( out string error )
		{
			DestroyNavMesh();

			if( !EnabledInHierarchy )
			{
				error = "Is not enabled.";
				return false;
			}

			if( NavMeshData == null )
			{
				error = "No data.";
				return false;
			}

			//check version
			var reader = new ArrayDataReader( NavMeshData );
			if( reader.ReadInt32() != navMeshDataVersion )
			{
				error = "Invalid version.";
				return false;
			}

			//read data
			var dataLength = reader.ReadInt32();
			var data = new byte[ dataLength ];
			reader.ReadBuffer( data );

			if( reader.BitPosition != reader.EndBitPosition || reader.Overflow )
			{
				error = "Invalid data.";
				return false;
			}

			//init
			try
			{
				using( var memoryStream = new MemoryStream( data ) )
				{
					var serializer = new NavMeshBinarySerializer();
					tiledNavMesh = serializer.Deserialize( memoryStream );
				}
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			//!!!!
			//TempObstaclesUpdate( true );

			error = "";
			return true;
		}

		public void DestroyNavMesh()
		{
			//WaitForExecutionAllQueuedOperations();
			//lock( nativeLock )
			//{
			//	//!!!!!нужно реально удалить обстаклы?
			//	foreach( TempObstacle obstacle in tempObstacles )
			//	{
			//		obstacle.nativeID = 0;
			//		obstacle.nativeCreated = false;
			//	}

			//!!!!что-то еще чистить?

			//	//!!!!надо ли удалять?
			//	TempObstaclesDestroy();

			tiledNavMesh = null;
			navMeshQuery = null;
			debugNavigationMeshVertices = null;
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

		static void VMad( ref SharpNav.Geometry.Vector3 dest, SharpNav.Geometry.Vector3 v1, SharpNav.Geometry.Vector3 v2, float s )
		{
			dest.X = v1.X + v2.X * s;
			dest.Y = v1.Y + v2.Y * s;
			dest.Z = v1.Z + v2.Z * s;
		}

		static bool GetSteerTarget( NavMeshQuery navMeshQuery, SharpNav.Geometry.Vector3 startPos, SharpNav.Geometry.Vector3 endPos, float minTargetDist, SharpNav.Pathfinding.Path path, ref SharpNav.Geometry.Vector3 steerPos, ref StraightPathFlags steerPosFlag, ref NavPolyId steerPosRef )
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

		static bool InRange( SharpNav.Geometry.Vector3 v1, SharpNav.Geometry.Vector3 v2, float r, float h )
		{
			float dx = v2.X - v1.X;
			float dy = v2.Y - v1.Y;
			float dz = v2.Z - v1.Z;
			return ( dx * dx + dz * dz ) < ( r * r ) && Math.Abs( dy ) < h;
		}

		public void FindPath( FindPathContext context )
		{
			if( tiledNavMesh != null )
			{
				if( navMeshQuery == null )
					navMeshQuery = new NavMeshQuery( tiledNavMesh, PathfindingMaxNodes );

				try
				{
					var extents = ToSharpNav( context.PolygonPickExtents, true );
					var startPt = navMeshQuery.FindNearestPoly( ToSharpNav( context.Start ), extents );
					var endPt = navMeshQuery.FindNearestPoly( ToSharpNav( context.End ), extents );

					var filter = new NavQueryFilter();

					var path = new SharpNav.Pathfinding.Path();
					var found = navMeshQuery.FindPath( ref startPt, ref endPt, filter, path );

					Vector3[] pathResult = null;
					if( found )
					{
						//find a smooth path over the mesh surface
						int npolys = path.Count;
						SharpNav.Geometry.Vector3 iterPos = new SharpNav.Geometry.Vector3();
						SharpNav.Geometry.Vector3 targetPos = new SharpNav.Geometry.Vector3();
						navMeshQuery.ClosestPointOnPoly( startPt.Polygon, startPt.Position, ref iterPos );
						navMeshQuery.ClosestPointOnPoly( path[ npolys - 1 ], endPt.Position, ref targetPos );

						var smoothPath = new List<SharpNav.Geometry.Vector3>( context.MaxSmoothPath );
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
							SharpNav.Geometry.Vector3 steerPos = new SharpNav.Geometry.Vector3();
							StraightPathFlags steerPosFlag = 0;
							NavPolyId steerPosRef = NavPolyId.Null;

							if( !GetSteerTarget( navMeshQuery, iterPos, targetPos, slop, path, ref steerPos, ref steerPosFlag, ref steerPosRef ) )
								break;

							bool endOfPath = ( steerPosFlag & StraightPathFlags.End ) != 0 ? true : false;
							bool offMeshConnection = ( steerPosFlag & StraightPathFlags.OffMeshConnection ) != 0 ? true : false;

							//find movement delta
							SharpNav.Geometry.Vector3 delta = steerPos - iterPos;
							float len = (float)Math.Sqrt( SharpNav.Geometry.Vector3.Dot( delta, delta ) );

							//if steer target is at end of path or off-mesh link
							//don't move past location
							if( ( endOfPath || offMeshConnection ) && len < stepSize )
								len = 1;
							else
								len = stepSize / len;

							SharpNav.Geometry.Vector3 moveTgt = new SharpNav.Geometry.Vector3();
							VMad( ref moveTgt, iterPos, delta, len );

							//move
							SharpNav.Geometry.Vector3 result = new SharpNav.Geometry.Vector3();
							List<NavPolyId> visited = new List<NavPolyId>( 16 );
							NavPoint startPoint = new NavPoint( path[ 0 ], iterPos );
							navMeshQuery.MoveAlongSurface( ref startPoint, ref moveTgt, out result, visited );
							path.FixupCorridor( visited );
							npolys = path.Count;
							float h = 0;
							navMeshQuery.GetPolyHeight( path[ 0 ], result, ref h );
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
			}
			else
				context.Error = "No NavMesh";

			context.Finished = true;
		}
	}
}
