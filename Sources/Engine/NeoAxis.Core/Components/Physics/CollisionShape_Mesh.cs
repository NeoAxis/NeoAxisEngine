// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace NeoAxis
{
	/// <summary>
	/// Mesh-based collision shape.
	/// </summary>
	public class CollisionShape_Mesh : CollisionShape
	{
		//Vector3F[] processedVertices;
		//int[] processedIndices;
		//int[] processedTrianglesToSourceIndex;

		//!!!!need delete?
		//TriangleIndexVertexArray indexVertexArrays;

		//!!!!все настройки
		//PerTriangleMaterial[] perTriangleMaterials;
		//short[] perTriangleMaterialIndices;

		[Browsable( false )]
		public bool CheckValidData { get; set; } = true;
		[Browsable( false )]
		public bool MergeEqualVerticesRemoveInvalidTriangles { get; set; } = true;

		/////////////////////////////////////////

		/// <summary>
		/// The mesh used by the collision shape.
		/// </summary>
		[Serialize]
		public Reference<Mesh> Mesh
		{
			get { if( _mesh.BeginGet() ) Mesh = _mesh.Get( this ); return _mesh.value; }
			set
			{
				if( _mesh.BeginSet( ref value ) )
				{
					try
					{
						MeshChanged?.Invoke( this );
						NeedUpdateCachedVolume();
						if( EnabledInHierarchy )
							RecreateBody();
						//!!!!bounds update?
						//!!!!what else
					}
					finally { _mesh.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Mesh"/> property value changes.</summary>
		public event Action<CollisionShape_Mesh> MeshChanged;
		ReferenceField<Mesh> _mesh;

		/// <summary>
		/// The reference to the vertex array of the mesh.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Shallow )]//!!!!или только для типов Shallow? а как обычно - полностью?
		public Reference<Vector3F[]> Vertices
		{
			get { if( _vertices.BeginGet() ) Vertices = _vertices.Get( this ); return _vertices.value; }
			set
			{
				if( _vertices.BeginSet( ref value ) )
				{
					try
					{
						VerticesChanged?.Invoke( this );
						NeedUpdateCachedVolume();
						if( EnabledInHierarchy )
							RecreateBody();
						//!!!!bounds update?
						//!!!!what else
					}
					finally { _vertices.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Vertices"/> property value changes.</summary>
		public event Action<CollisionShape_Mesh> VerticesChanged;
		ReferenceField<Vector3F[]> _vertices;

		/// <summary>
		/// The reference to the index array of the mesh.
		/// </summary>
		[Serialize]
		[Cloneable( CloneType.Shallow )]//!!!!или только для типов Shallow? а как обычно - полностью?
		public Reference<int[]> Indices
		{
			get { if( _indices.BeginGet() ) Indices = _indices.Get( this ); return _indices.value; }
			set
			{
				if( _indices.BeginSet( ref value ) )
				{
					try
					{
						IndicesChanged?.Invoke( this );
						NeedUpdateCachedVolume();
						if( EnabledInHierarchy )
							RecreateBody();
						//!!!!bounds update?
						//!!!!what else
					}
					finally { _indices.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="Indices"/> property value changes.</summary>
		public event Action<CollisionShape_Mesh> IndicesChanged;
		ReferenceField<int[]> _indices;

		/// <summary>
		/// Enumerates the types of collision shape being created internally.
		/// </summary>
		public enum ShapeTypeEnum
		{
			Auto,
			TriangleMesh,
			Convex,
			//раньше была декомпозиция на лету. теперь в редакторе
			//ConvexDecomposition,
		}

		/// <summary>
		/// Defines the type of collision shape being created internally.
		/// </summary>
		[DefaultValue( ShapeTypeEnum.Auto )]
		[Serialize]
		public Reference<ShapeTypeEnum> ShapeType
		{
			get { if( _shapeType.BeginGet() ) ShapeType = _shapeType.Get( this ); return _shapeType.value; }
			set
			{
				if( _shapeType.BeginSet( ref value ) )
				{
					try
					{
						ShapeTypeChanged?.Invoke( this );
						NeedUpdateCachedVolume();
						if( EnabledInHierarchy )
							RecreateBody();
					}
					finally { _shapeType.EndSet(); }
				}
			}
		}
		/// <summary>Occurs when the <see cref="ShapeType"/> property value changes.</summary>
		public event Action<CollisionShape_Mesh> ShapeTypeChanged;
		ReferenceField<ShapeTypeEnum> _shapeType = ShapeTypeEnum.Auto;

		/////////////////////////////////////////

		public bool GetSourceData( out Vector3F[] vertices, out int[] indices )
		{
			var mesh = Mesh.Value;
			if( mesh != null )
			{
				if( mesh.Result != null )
				{
					vertices = mesh.Result.ExtractedVerticesPositions;
					indices = mesh.Result.ExtractedIndices;
				}
				else
				{
					vertices = null;
					indices = null;
				}
			}
			else
			{
				vertices = Vertices;
				indices = Indices;
			}

			return vertices != null && vertices.Length != 0 && indices != null && indices.Length != 0;
		}

		protected internal override void GetShapeKey( StringBuilder key )
		{
			base.GetShapeKey( key );

			//get source geometry
			if( !GetSourceData( out var sourceVertices, out var sourceIndices ) )
				return;

			key.Append( " mes " );
			key.Append( sourceVertices.Length );
			key.Append( ' ' );
			key.Append( sourceIndices.Length );

			{
				var hash = 0;
				for( int n = 0; n < sourceVertices.Length; n++ )
					hash ^= sourceVertices[ n ].GetHashCode();
				key.Append( ' ' );
				key.Append( hash );
			}
			{
				var hash = 0;
				for( int n = 0; n < sourceIndices.Length; n++ )
					hash ^= sourceIndices[ n ].GetHashCode();
				key.Append( ' ' );
				key.Append( hash );
			}

			if( sourceVertices.Length > 0 )
			{
				key.Append( ' ' );
				key.Append( sourceVertices[ 0 ].ToString() );
			}
			if( sourceIndices.Length > 0 )
			{
				key.Append( ' ' );
				key.Append( sourceIndices[ 0 ] );
			}
		}

		protected unsafe internal override void CreateShape( Scene scene, IntPtr nativeShape, ref Vector3F position, ref QuaternionF rotation, ref Vector3F localScaling, ref Scene.PhysicsWorldClass.Shape.CollisionShapeData collisionShapeData )
		{
			var epsilon = 0.0001f;

			//clear data
			Vector3F[] processedVertices = null;
			int[] processedIndices = null;
			int[] processedTrianglesToSourceIndex = null;

			//get source geometry
			if( !GetSourceData( out var sourceVertices, out var sourceIndices ) )
				return;// (null, null);


			//!!!!multi materials


			//check valid data
			if( CheckValidData && !MathAlgorithms.CheckValidVertexIndexBuffer( sourceVertices.Length, sourceIndices, false ) )
			{
				Log.Info( "CollisionShape_Mesh: CreateShape: Invalid source data." );
				return;
			}

			//process geometry
			if( MergeEqualVerticesRemoveInvalidTriangles )
			{
				//!!!!slowly. later use cached precalculated bullet shape.
				MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( sourceVertices, sourceIndices, epsilon, epsilon, true, true, out processedVertices, out processedIndices, out processedTrianglesToSourceIndex );
			}
			else
			{
				processedVertices = sourceVertices;
				processedIndices = sourceIndices;
				processedTrianglesToSourceIndex = null;
			}

			//!!!!slowly? IsMeshConvex for many objects. can pass parameters to the cache
			var makeConvex = ShapeType.Value == ShapeTypeEnum.Auto && ParentRigidBody.MotionType.Value == PhysicsMotionType.Dynamic && MathAlgorithms.IsMeshConvex( processedVertices, processedIndices, epsilon ) || ShapeType.Value == ShapeTypeEnum.Convex;

			fixed( Vector3F* pProcessedVertices = processedVertices )
			{
				fixed( int* pProcessedIndices = processedIndices )
				{
					if( makeConvex )
					{
						var convexRadius = scene.PhysicsAdvancedSettings ? scene.PhysicsDefaultConvexRadius.Value : Scene.physicsDefaultConvexRadiusDefault;

						PhysicsNative.JShape_AddConvexHull( nativeShape, ref position, ref rotation, ref localScaling, pProcessedVertices, processedVertices.Length, (float)convexRadius );
					}
					else
					{
						PhysicsNative.JShape_AddMesh( nativeShape, ref position, ref rotation, ref localScaling, pProcessedVertices, processedVertices.Length, pProcessedIndices, processedIndices.Length );
					}
				}
			}

			collisionShapeData.meshShapeProcessedVertices = processedVertices;
			collisionShapeData.meshShapeProcessedIndices = processedIndices;
			collisionShapeData.meshShapeProcessedTrianglesToSourceIndex = processedTrianglesToSourceIndex;



			//var epsilon = 0.0001f;

			////clear data
			//processedVertices = null;
			//processedIndices = null;
			//processedTrianglesToSourceIndex = null;

			////get source geometry
			//if( !GetSourceData( out var sourceVertices, out var sourceIndices ) )
			//	return;// (null, null);


			////!!!!multi materials


			////check valid data
			//if( CheckValidData && !MathAlgorithms.CheckValidVertexIndexBuffer( sourceVertices.Length, sourceIndices, false ) )
			//{
			//	Log.Info( "CollisionShape_Mesh: CreateShape: Invalid source data." );
			//	return;
			//}

			////process geometry
			//if( MergeEqualVerticesRemoveInvalidTriangles )
			//{
			//	//!!!!slowly. later use cached precalculated bullet shape.
			//	MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( sourceVertices, sourceIndices, epsilon, epsilon, true, true, out processedVertices, out processedIndices, out processedTrianglesToSourceIndex );
			//}
			//else
			//{
			//	processedVertices = sourceVertices;
			//	processedIndices = sourceIndices;
			//	processedTrianglesToSourceIndex = null;
			//}

			////!!!!slowly? IsMeshConvex for many objects. can pass parameters to the cache
			//var makeConvex = ShapeType.Value == ShapeTypeEnum.Auto && ParentRigidBody.MotionType.Value == RigidBody.MotionTypeEnum.Dynamic && MathAlgorithms.IsMeshConvex( processedVertices, processedIndices, epsilon ) || ShapeType.Value == ShapeTypeEnum.Convex;

			//fixed( Vector3F* pProcessedVertices = processedVertices )
			//{
			//	fixed( int* pProcessedIndices = processedIndices )
			//	{
			//		if( makeConvex )
			//		{
			//			PhysicsNative.JShape_AddConvexHull( nativeShape, ref position, ref rotation, ref localScaling, pProcessedVertices, processedVertices.Length );
			//		}
			//		else
			//		{
			//			PhysicsNative.JShape_AddMesh( nativeShape, ref position, ref rotation, ref localScaling, pProcessedVertices, processedVertices.Length, pProcessedIndices, processedIndices.Length );
			//		}
			//	}
			//}

			////var meshCacheItem = physicsWorldData.AllocateShapeInCache( ref localScaling, sourceVertices, sourceIndices, CheckValidData, MergeEqualVerticesRemoveInvalidTriangles, makeConvex );

			////if( meshCacheItem != null )
			////{
			////	processedVertices = meshCacheItem.ProcessedVertices;
			////	processedIndices = meshCacheItem.ProcessedIndices;
			////	processedTrianglesToSourceIndex = meshCacheItem.ProcessedTrianglesToSourceIndex;
			////}

			////if( meshCacheItem != null )
			////	return (meshCacheItem.Shape, meshCacheItem);
			////else
			////	return (null, null);
		}

		//!!!!
		//protected internal override (Internal.BulletSharp.CollisionShape shape, Scene.PhysicsWorldDataClass.MeshShapeCacheItem meshShapeCacheItem) CreateShape( Scene.PhysicsWorldDataClass physicsWorldData, ref Vector3 localScaling )
		//{
		//	var epsilon = 0.0001f;

		//	//clear data
		//	processedVertices = null;
		//	processedIndices = null;
		//	processedTrianglesToSourceIndex = null;

		//	//get source geometry
		//	if( !GetSourceData( out var sourceVertices, out var sourceIndices ) )
		//		return (null, null);

		//	//!!!!slowly? IsMeshConvex for many objects. can pass parameters to the cache
		//	var makeConvex = ShapeType.Value == ShapeTypeEnum.Auto && ParentRigidBody.MotionType.Value == RigidBody.MotionTypeEnum.Dynamic && MathAlgorithms.IsMeshConvex( processedVertices, processedIndices, epsilon ) || ShapeType.Value == ShapeTypeEnum.Convex;

		//	var meshCacheItem = physicsWorldData.AllocateShapeInCache( ref localScaling, sourceVertices, sourceIndices, CheckValidData, MergeEqualVerticesRemoveInvalidTriangles, makeConvex );

		//	if( meshCacheItem != null )
		//	{
		//		processedVertices = meshCacheItem.ProcessedVertices;
		//		processedIndices = meshCacheItem.ProcessedIndices;
		//		processedTrianglesToSourceIndex = meshCacheItem.ProcessedTrianglesToSourceIndex;
		//	}

		//	if( meshCacheItem != null )
		//		return (meshCacheItem.Shape, meshCacheItem);
		//	else
		//		return (null, null);

		//	//if( !string.IsNullOrEmpty( error ) )
		//	//{
		//	//	Log.Info( "CollisionShape_Mesh: CreateShape: " + error );
		//	//	return null;
		//	//}

		//	//if( meshCacheItem != null )
		//	//	meshShapeCacheItems.Add( meshCacheItem );

		//	//if( meshCacheItem != null )
		//	//	return meshCacheItem.Shape;
		//	//return null;


		//	////check valid data
		//	//if( CheckValidData )
		//	//{
		//	//	if( !MathAlgorithms.CheckValidVertexIndexBuffer( sourceVertices.Length, sourceIndices, false ) )
		//	//	{
		//	//		Log.Info( "CollisionShape_Mesh: CreateShape: Invalid source data." );
		//	//		return null;
		//	//	}
		//	//}

		//	////process geometry
		//	//if( MergeEqualVerticesRemoveInvalidTriangles )
		//	//{
		//	//	//!!!!slowly. later use cached precalculated bullet shape.
		//	//	MathAlgorithms.MergeEqualVerticesRemoveInvalidTriangles( sourceVertices, sourceIndices, epsilon, epsilon, out processedVertices, out processedIndices, out processedTrianglesToSourceIndex );
		//	//}
		//	//else
		//	//{
		//	//	processedVertices = sourceVertices;
		//	//	processedIndices = sourceIndices;
		//	//}

		//	////create bullet shape

		//	//if( ShapeType.Value == ShapeTypeEnum.Auto && ParentRigidBody.MotionType.Value == RigidBody.MotionTypeEnum.Dynamic && MathAlgorithms.IsMeshConvex( processedVertices, processedIndices, epsilon ) || ShapeType.Value == ShapeTypeEnum.Convex )
		//	//{
		//	//	if( MathAlgorithms.IsPlaneMesh( processedVertices, processedIndices, epsilon ) )
		//	//	{
		//	//		Log.Info( "CollisionShape_Mesh: CreateShape: Unable to create shape as convex hull. All vertices on the one plane." );
		//	//		return null;
		//	//	}

		//	//	//!!!!тут иначе? возможно лучше получить результирующие processed данные из буллета. как получить processedTrianglesToSourceIndex - это вопрос. возможно ли?
		//	//	//если нельзя то processedTrianglesToSourceIndex = new int[ 0 ]; - что означает нельзя сконвертировать.
		//	//	//если processedTrianglesToSourceIndex == null, то конвертация 1:1.

		//	//	try
		//	//	{
		//	//		MathAlgorithms.ConvexHullFromMesh( MathUtility.ToVector3Array( processedVertices ), processedIndices, out var processedVertices2, out processedIndices );
		//	//		processedVertices = MathUtility.ToVector3FArray( processedVertices2 );

		//	//		//var convex = ConvexHullAlgorithm.Create( processedVertices, processedIndices );

		//	//		//var vlist = new List<Vec3F>( convex.Faces.Length * 3 );
		//	//		//foreach( var f in convex.Faces )
		//	//		//	for( int v = 0; v < f.Vertices.Length; v++ )
		//	//		//		vlist.Add( f.Vertices[ v ].ToVec3F() );

		//	//		//processedVertices = vlist.ToArray();
		//	//		//processedIndices = null;

		//	//		//BulletUtils.GetHullVertices( processedVertices.ToVec3Array(), processedIndices, out var processedVertices2, out processedIndices );
		//	//		//processedVertices = processedVertices2.ToVec3FArray();
		//	//		//BulletUtils.GetHullVertices( processedVertices, processedIndices, out processedVertices, out processedIndices );

		//	//		//если нельзя то processedTrianglesToSourceIndex = new int[ 0 ]; - что означает нельзя сконвертировать.
		//	//		processedTrianglesToSourceIndex = Array.Empty<int>();
		//	//	}
		//	//	catch( Exception e )
		//	//	{
		//	//		Log.Info( "CollisionShape_Mesh: CreateShape: Unable to create shape as convex hull. " + e.Message );
		//	//		return null;
		//	//	}

		//	//	//!!!!
		//	//	var processedVerticesBullet = BulletPhysicsUtility.Convert( processedVertices );

		//	//	return new ConvexHullShape( processedVerticesBullet );
		//	//}
		//	//else
		//	//{
		//	//	//!!!проверки на ошибки данных

		//	//	//!!!!need call dispose?
		//	//	//!!!!can create without making of Vector3[] array. IntPtr constructor? internally the memory will copied?
		//	//	indexVertexArrays = new TriangleIndexVertexArray( processedIndices, BulletPhysicsUtility.Convert( processedVertices ) );

		//	//	//indexVertexArrays = new TriangleIndexVertexArray();
		//	//	//var indexedMesh = new IndexedMesh();
		//	//	//indexedMesh.Allocate( totalTriangles, totalVerts, triangleIndexStride, vertexStride );
		//	//	//indexedMesh SetData( ICollection<int> triangles, ICollection<Vector3> vertices );
		//	//	//indexVertexArrays.AddIndexedMesh( indexedMesh );


		//	//	//!!!!расшаривать данные которые тут. одинаковые в разных объектах

		//	//	//!!!!определять когда не считать кеш

		//	//	//It is better to use "useQuantizedAabbCompression=true", because it makes the tree data structure 4 times smaller: sizeof( btOptimizedBvhNode ) = 64 and sizeof( btQuantizedBvhNode ) = 16 bytes.Note that the number of AABB tree nodes is twice the number of triangles.

		//	//	//Instead of creating the tree on the XBox 360 console, it is better to deserialize it directly from disk to memory. See btOptimizedBvh::deSerializeInPlace in Demos/ConcaveDemo/ConcavePhysicsDemo.cpp 

		//	//	//без useQuantizedAabbCompression в три раза быстрее создается

		//	//	//!!!!в ключ итема в кеше давать геометрию до обработки (MergeEqualVerticesRemoveInvalidTriangles)

		//	//	//!!!!enable when cache support
		//	//	bool useQuantizedAabbCompression = false;
		//	//	//bool useQuantizedAabbCompression = true;
		//	//	bool buildBvh = true;

		//	//	//!!!!в другом конструкторе можно еще указать какие-то bound min max
		//	//	//public BvhTriangleMeshShape( StridingMeshInterface meshInterface, bool useQuantizedAabbCompression, Vector3 bvhAabbMin, Vector3 bvhAabbMax, bool buildBvh = true );

		//	//	return new BvhTriangleMeshShape( indexVertexArrays, useQuantizedAabbCompression, buildBvh );
		//	//}
		//}

		//!!!!
		//public bool GetProcessedData( out Vector3F[] processedVertices, out int[] processedIndices, out int[] processedTrianglesToSourceIndex )
		//{
		//	processedVertices = this.processedVertices;
		//	processedIndices = this.processedIndices;
		//	processedTrianglesToSourceIndex = this.processedTrianglesToSourceIndex;
		//	return processedVertices != null;
		//}

		//!!!!
		//public bool GetData( out Vector3F[] vertices, out int[] indices )
		//{
		//	if( GetProcessedData( out vertices, out indices, out _ ) )
		//		return true;
		//	return GetSourceData( out vertices, out indices );
		//}

		public bool GetTriangleSourceData( int triangleID, bool applyWorldTransform, out Triangle triangle )
		{
			if( !GetSourceData( out var vertices, out var indices ) )
			{
				triangle = new Triangle();
				return false;
			}
			return GetTriangleData( triangleID, applyWorldTransform, vertices, indices, out triangle );
		}

		//!!!!
		//public bool GetTriangleProcessedData( int triangleID, bool applyWorldTransform, out Triangle triangle )
		//{
		//	if( !GetProcessedData( out var vertices, out var indices, out _ ) )
		//	{
		//		triangle = new Triangle();
		//		return false;
		//	}
		//	return GetTriangleData( triangleID, applyWorldTransform, vertices, indices, out triangle );
		//}

		bool GetTriangleData( int triangleID, bool applyWorldTransform, Vector3F[] vertices, int[] indices, out Triangle triangle )
		{
			if( applyWorldTransform )
			{
				var t = ParentRigidBody.Transform.Value.ToMatrix4();
				var local = LocalTransform.Value;
				if( local != null && !local.IsIdentity )
					t *= local.ToMatrix4();
				return MathAlgorithms.GetTriangleData( triangleID, ref t, vertices, indices, out triangle );
			}
			else
				return MathAlgorithms.GetTriangleData( triangleID, null, vertices, indices, out triangle );
		}

		protected internal override void Render( Viewport viewport, Transform bodyTransform, bool solid, ref int verticesRendered )
		{
			Matrix4 t = bodyTransform.ToMatrix4();
			var local = LocalTransform.Value;
			if( !local.IsIdentity )
				t *= local.ToMatrix4();

			//!!!!
			//if( MeshType == MeshTypes.ConvexDecomposition )
			//{
			//	PhysicsWorld.ConvexDecompositionDataItem[] items = GetConvexDecompositionData();
			//	if( items != null )
			//	{
			//		foreach( PhysicsWorld.ConvexDecompositionDataItem item in items )
			//			debugGeometry.AddVertexIndexBuffer( item.Vertices, item.Indices, t, true, false );
			//		return;
			//	}
			//}

			//!!!!
			if( GetSourceData( out Vector3F[] vertices, out int[] indices ) )
			//if( GetData( out Vector3F[] vertices, out int[] indices ) )
			{
				if( indices != null )
					viewport.Simple3DRenderer.AddTriangles( vertices, indices, t, !solid, false );
				else
					viewport.Simple3DRenderer.AddTriangles( vertices, t, !solid, false );
				verticesRendered += vertices.Length;
			}
		}

		protected override double OnCalculateVolume()
		{
			//!!!!
			return 1;

			//if( MeshType == MeshTypes.ConvexDecomposition )
			//{
			//	//convex hull decompisition
			//	PhysicsWorld.ConvexDecompositionDataItem[] items = GetConvexDecompositionData();
			//	if( items == null )
			//		return 0;
			//	float volume = 0;
			//	foreach( PhysicsWorld.ConvexDecompositionDataItem item in items )
			//		volume += GetConvexHullVolume( item.Vertices, item.Indices );
			//	return volume;
			//}
			//else if( MeshType == MeshTypes.ConvexHull )
			//{
			//	//convex mesh
			//	Vec3F[] vertices;
			//	int[] indices;
			//	if( !GetData( out vertices, out indices ) )
			//		return 0;
			//	return GetConvexHullVolume( vertices, indices );
			//}
			//else
			//{
			//	//triangle mesh
			//	//not true method. calculated by bounding box.
			//	Bounds b;
			//	GetDataBounds( out b );
			//	Vec3 dimensions = b.GetSize();
			//	return dimensions.X * dimensions.Y * dimensions.Z;
			//}

		}

		internal override Vector3 GetCenterOfMassPositionNotScaledByParent()
		{
			//!!!!

			return base.GetCenterOfMassPositionNotScaledByParent();
		}

		//!!!!все методы из MeshShape
	}
}
