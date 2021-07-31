// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;
using BulletSharp;
using BulletSharp.Math;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary methods for working with the Bullet physics engine.
	/// </summary>
	public static class BulletPhysicsUtility
	{
		//volatile static Dictionary<Type, Func<CollisionShape, Vector3>> centerOfMassDictionary;
		//!!!!delete
		//volatile static Dictionary<Type, Func<CollisionShape, double>> volumeDictionary;

		//

		internal static void InitLibrary()
		{
			NativeLibraryManager.PreLoadLibrary( "libbulletc" );
		}

		internal static void ShutdownLibrary()
		{
		}

		public static Vector3 ToVector3( BulletSharp.Math.Vector3 v )
		{
			return new Vector3( v.X, v.Y, v.Z );
		}

		public static BulletSharp.Math.Vector3 ToVector3( Vector3 v )
		{
			return new BulletSharp.Math.Vector3( v.X, v.Y, v.Z );
		}

		public static BulletSharp.Math.Vector3[] ToVectorArray( Vector3[] arr )
		{
			var result = new BulletSharp.Math.Vector3[ arr.Length ];
			for( int n = 0; n < result.Length; n++ )
				result[ n ] = ToVector3( arr[ n ] );
			return result;
		}

		public static bool IntersectsConvexHull( Plane[] convexPlanes, BulletSharp.SoftBody.SoftBody body )
		{
			var nodes = body.Nodes;
			var nnodes = nodes.Count;

			for( int i = 0; i < nnodes; i++ )
				if( MathAlgorithms.IsVertexInsideConvexHull( convexPlanes, ToVector3( nodes[ i ].Position ) ) )
					return true;

			return false;
		}

		//public static void GetHullVertices( Vec3[] vertices, int[] indices, out Vec3[] hullVertices, out int[] hullIndices )
		//{
		//	using( var triMesh = CreateTriangleMesh( vertices, indices ) )
		//	{
		//		GetHullVertices( triMesh, out hullVertices, out hullIndices );
		//	}
		//}

		//public static void GetHullVertices( TriangleMesh triangleMesh, out Vec3[] vertices, out int[] indices )
		//{
		//	using( var tmpConvexShape = new ConvexTriangleMeshShape( triangleMesh ) )
		//	{
		//		GetHullVertices( tmpConvexShape, out vertices, out indices );
		//	}
		//}

		//public static void GetHullVertices( Vec3[] vertices, out Vec3[] hullVertices, out int[] hullIndices )
		//{
		//	using( var tmpShape = new ConvexHullShape( ToVectorArray( vertices ) ) )
		//	{
		//		GetHullVertices( tmpShape, out hullVertices, out hullIndices );
		//	}
		//}

		//public static void GetHullVertices( ConvexShape convex, out Vec3[] vertices, out int[] indices )
		//{
		//	using( var hull = new ShapeHull( convex ) )
		//	{
		//		hull.BuildHull( convex.Margin );

		//		var hIndices = hull.Indices;
		//		var points = hull.Vertices;

		//		vertices = new Vec3[ hull.NumVertices ];
		//		indices = new int[ hull.NumIndices ];

		//		for( int i = 0; i < indices.Length; i++ )
		//			indices[ i ] = (int)hIndices[ i ];

		//		for( int v = 0; v < vertices.Length; v++ )
		//			vertices[ v ] = Convert( points[ v ] );
		//	}
		//}

		//public static TriangleMesh CreateTriangleMesh( Vec3[] vertices, int[] indices )
		//{
		//	var triangleMesh = new TriangleMesh();

		//	var triangleCount = indices.Length / 3;
		//	for( int i = 0; i < triangleCount; i++ )
		//	{
		//		var index0 = indices[ i * 3 ];
		//		var index1 = indices[ i * 3 + 1 ];
		//		var index2 = indices[ i * 3 + 2 ];

		//		Convert( ref vertices[ index0 ], out var vertex0 );
		//		Convert( ref vertices[ index1 ], out var vertex1 );
		//		Convert( ref vertices[ index2 ], out var vertex2 );

		//		triangleMesh.AddTriangleRef( ref vertex0, ref vertex1, ref vertex2, true );
		//	}

		//	return triangleMesh;
		//}

		//!!!!slowly? ref?

		//convert to Bullet

		public static Matrix Convert( Matrix4 v )
		{
			return new Matrix(
				v.Item0.X, v.Item0.Y, v.Item0.Z, v.Item0.W,
				v.Item1.X, v.Item1.Y, v.Item1.Z, v.Item1.W,
				v.Item2.X, v.Item2.Y, v.Item2.Z, v.Item2.W,
				v.Item3.X, v.Item3.Y, v.Item3.Z, v.Item3.W );
		}

		public static void Convert( ref Matrix4 v, out Matrix result )
		{
			result = new Matrix(
				v.Item0.X, v.Item0.Y, v.Item0.Z, v.Item0.W,
				v.Item1.X, v.Item1.Y, v.Item1.Z, v.Item1.W,
				v.Item2.X, v.Item2.Y, v.Item2.Z, v.Item2.W,
				v.Item3.X, v.Item3.Y, v.Item3.Z, v.Item3.W );
		}

		public static BulletSharp.Math.Quaternion Convert( Quaternion v )
		{
			return new BulletSharp.Math.Quaternion( v.X, v.Y, v.Z, v.W );
		}

		public static BulletSharp.Math.Vector3 Convert( Vector3 v )
		{
			return new BulletSharp.Math.Vector3( v.X, v.Y, v.Z );
		}

		public static BulletSharp.Math.Vector3 Convert( Vector3F v )
		{
			return new BulletSharp.Math.Vector3( v.X, v.Y, v.Z );
		}

		public static void Convert( ref Vector3 v, out BulletSharp.Math.Vector3 result )
		{
			result = new BulletSharp.Math.Vector3( v.X, v.Y, v.Z );
		}

		public static void Convert( ref Vector3F v, out BulletSharp.Math.Vector3 result )
		{
			result = new BulletSharp.Math.Vector3( v.X, v.Y, v.Z );
		}

		public static BulletSharp.Math.Vector3[] Convert( Vector3F[] vertices )
		{
			var result = new BulletSharp.Math.Vector3[ vertices.Length ];
			for( int n = 0; n < vertices.Length; n++ )
				result[ n ] = Convert( vertices[ n ] );
			return result;
		}

		public static BulletSharp.Math.Vector3[] Convert( Vector3[] vertices )
		{
			var result = new BulletSharp.Math.Vector3[ vertices.Length ];
			for( int n = 0; n < vertices.Length; n++ )
				result[ n ] = Convert( vertices[ n ] );
			//unsafe
			//{
			//	fixed ( void* src = vertices )
			//	fixed ( void* dst = result )
			//		Buffer.MemoryCopy( src, dst, vertices.Length * 24, vertices.Length * 24 );
			//}
			return result;
		}

		internal static double Convert( double v )
		{
			return v;
		}


		//convert from Bullet

		public static Quaternion Convert( BulletSharp.Math.Quaternion v )
		{
			return new Quaternion( v.X, v.Y, v.Z, v.W );
		}

		public static Vector3 Convert( BulletSharp.Math.Vector3 v )
		{
			return new Vector3( v.X, v.Y, v.Z );
		}

		public static Matrix4 Convert( Matrix v )
		{
			return new Matrix4(
				v.M11, v.M12, v.M13, v.M14,
				v.M21, v.M22, v.M23, v.M24,
				v.M31, v.M32, v.M33, v.M34,
				v.M41, v.M42, v.M43, v.M44 );
		}

		public static void Convert( ref Matrix v, out Matrix4 result )
		{
			result = new Matrix4(
				v.M11, v.M12, v.M13, v.M14,
				v.M21, v.M22, v.M23, v.M24,
				v.M31, v.M32, v.M33, v.M34,
				v.M41, v.M42, v.M43, v.M44 );
		}



		//public static Vector3 GetCenterOfMass( CollisionShape shape )
		//{
		//	if( centerOfMassDictionary == null )
		//	{
		//		centerOfMassDictionary = new Dictionary<Type, Func<CollisionShape, Vector3>>
		//		{
		//			{ typeof(EmptyShape),                (s) => { return CalcShapeCOM((EmptyShape)s); } },

		//			{ typeof(BulletSharp.SphereShape),   (s) => { return CalcShapeCOM((BulletSharp.SphereShape)s); } },
		//			{ typeof(BulletSharp.BoxShape),      (s) => { return CalcShapeCOM((BulletSharp.BoxShape)s); } },
		//			{ typeof(BulletSharp.CylinderShape), (s) => { return CalcShapeCOM((BulletSharp.CylinderShape)s); } },
		//			{ typeof(BulletSharp.CapsuleShape),  (s) => { return CalcShapeCOM((BulletSharp.CapsuleShape)s); } },
		//			{ typeof(ConeShape),                 (s) => { return CalcShapeCOM((ConeShape)s); } },
		//			{ typeof(ConeShapeX),                (s) => { return CalcShapeCOM((ConeShapeX)s); } },
		//			{ typeof(ConeShapeZ),                (s) => { return CalcShapeCOM((ConeShapeZ)s); } },
		//			{ typeof(MultiSphereShape),          (s) => { return CalcShapeCOM((MultiSphereShape)s); } },
		//			{ typeof(ConvexHullShape),           (s) => { return CalcShapeCOM((ConvexHullShape)s); } },
		//			{ typeof(ConvexTriangleMeshShape),   (s) => { return CalcShapeCOM((ConvexTriangleMeshShape)s); } },

		//			{ typeof(BvhTriangleMeshShape),      (s) => { return CalcShapeCOM((BvhTriangleMeshShape)s); } },
		//			{ typeof(HeightfieldTerrainShape),   (s) => { return CalcShapeCOM((HeightfieldTerrainShape)s); } },
		//			{ typeof(StaticPlaneShape),          (s) => { return CalcShapeCOM((StaticPlaneShape)s); } },

		//			{ typeof(CompoundShape),             (s) => { return CalcShapeCOM((CompoundShape)s); } },
		//		};
		//	}

		//	var shapeType = shape.GetType();

		//	var centerOfMass = Vector3.Zero;
		//	if( centerOfMassDictionary.ContainsKey( shapeType ) )
		//		centerOfMass = centerOfMassDictionary[ shapeType ]( shape );

		//	return centerOfMass;
		//}

		//static Vector3 CalcShapeCOM( EmptyShape shape )
		//{
		//	return Vector3.Zero;
		//}

		//static Vector3 CalcShapeCOM( BulletSharp.SphereShape shape )
		//{
		//	return Vector3.Zero;
		//}
		//static Vector3 CalcShapeCOM( BulletSharp.BoxShape shape )
		//{
		//	return Vector3.Zero;
		//}
		//static Vector3 CalcShapeCOM( BulletSharp.CylinderShape shape )
		//{
		//	return Vector3.Zero;
		//}
		//static Vector3 CalcShapeCOM( BulletSharp.CapsuleShape shape )
		//{
		//	return Vector3.Zero;
		//}

		//static Vector3 CalcShapeCOM( ConeShape shape )
		//{
		//	Vector3 centerOfMass = Vector3.Zero;
		//	centerOfMass.Y = -shape.Height / 4.0;
		//	return centerOfMass;
		//}
		//static Vector3 CalcShapeCOM( ConeShapeX shape )
		//{
		//	Vector3 centerOfMass = Vector3.Zero;
		//	centerOfMass.X = -shape.Height / 4.0;
		//	return centerOfMass;
		//}
		//static Vector3 CalcShapeCOM( ConeShapeZ shape )
		//{
		//	Vector3 centerOfMass = Vector3.Zero;
		//	centerOfMass.Z = -shape.Height / 4.0;
		//	return centerOfMass;
		//}

		//static Vector3 CalcShapeCOM( MultiSphereShape shape )
		//{
		//	double totalVolume = 0.0;
		//	Vector3 weighetdPosition = Vector3.Zero;
		//	for( int cI = 0; cI < shape.SphereCount; cI++ )
		//	{
		//		double R = shape.GetSphereRadius( cI );
		//		double V = ( 4.0 / 3.0 ) * Math.PI * R * R * R;

		//		weighetdPosition += V * shape.GetSpherePosition( cI );
		//		totalVolume += V;
		//	}
		//	Vector3 centerOfMass = weighetdPosition / totalVolume;

		//	return centerOfMass;
		//}

		//static Vector3 CalcShapeCOM( ConvexHullShape shape )
		//{
		//	Vector3Array array = shape.UnscaledPoints;

		//	Vector3 centerOfMass = Vector3.Zero;
		//	for( int cI = 0; cI < array.Count; cI++ )
		//	{
		//		centerOfMass += array[ cI ];
		//	}
		//	centerOfMass /= array.Count;

		//	return centerOfMass;
		//}

		//static Vector3 CalcShapeCOM( ConvexTriangleMeshShape shape )
		//{
		//	Vector3 centerOfMass = Vector3.Zero;
		//	for( int cI = 0; cI < shape.NumVertices; cI++ )
		//	{
		//		Vector3 vertex;
		//		shape.GetVertex( cI, out vertex );
		//		centerOfMass += vertex;
		//	}
		//	centerOfMass /= shape.NumVertices;

		//	return centerOfMass;
		//}

		//static Vector3 CalcShapeCOM( BvhTriangleMeshShape shape )
		//{
		//	Vector3 aabbMin;
		//	Vector3 aabbMax;
		//	Matrix m = Matrix.Identity;
		//	shape.GetAabb( m, out aabbMin, out aabbMax );

		//	TriangleBuffer buffer = new TriangleBuffer();
		//	shape.ProcessAllTriangles( buffer, aabbMin, aabbMax );

		//	Vector3 centerOfMass = Vector3.Zero;
		//	for( int cI = 0; cI < buffer.NumTriangles; cI++ )
		//	{
		//		var tri = buffer.GetTriangle( cI );
		//		centerOfMass += ( tri.Vertex0 + tri.Vertex1 + tri.Vertex2 ) / 3.0;
		//	}
		//	centerOfMass /= buffer.NumTriangles;

		//	return centerOfMass;
		//}

		//static Vector3 CalcShapeCOM( HeightfieldTerrainShape shape )
		//{
		//	Vector3 aabbMin;
		//	Vector3 aabbMax;
		//	Matrix m = Matrix.Identity;
		//	shape.GetAabb( m, out aabbMin, out aabbMax );

		//	TriangleBuffer buffer = new TriangleBuffer();
		//	shape.ProcessAllTriangles( buffer, aabbMin, aabbMax );

		//	Vector3 centerOfMass = Vector3.Zero;
		//	for( int cI = 0; cI < buffer.NumTriangles; cI++ )
		//	{
		//		var tri = buffer.GetTriangle( cI );
		//		centerOfMass += ( tri.Vertex0 + tri.Vertex1 + tri.Vertex2 ) / 3.0;
		//	}
		//	centerOfMass /= buffer.NumTriangles;

		//	return centerOfMass;
		//}

		//static Vector3 CalcShapeCOM( StaticPlaneShape shape )
		//{
		//	Vector3 aabbMin;
		//	Vector3 aabbMax;
		//	Matrix m = Matrix.Identity;
		//	shape.GetAabb( m, out aabbMin, out aabbMax );

		//	TriangleBuffer buffer = new TriangleBuffer();
		//	shape.ProcessAllTriangles( buffer, aabbMin, aabbMax );

		//	Vector3 centerOfMass = Vector3.Zero;
		//	for( int cI = 0; cI < buffer.NumTriangles; cI++ )
		//	{
		//		var tri = buffer.GetTriangle( cI );
		//		centerOfMass += ( tri.Vertex0 + tri.Vertex1 + tri.Vertex2 ) / 3.0;
		//	}
		//	centerOfMass /= buffer.NumTriangles;

		//	return centerOfMass;
		//}

		//static Vector3 CalcShapeCOM( CompoundShape shape )
		//{
		//	Vector3 weightedCOM = Vector3.Zero;
		//	double totalVolume = 0.0;

		//	for( int cI = 0; cI < shape.NumChildShapes; cI++ )
		//	{
		//		var childShape = shape.GetChildShape( cI );
		//		var childTransform = shape.GetChildTransform( cI );

		//		Vector3 shapeCOM = Vector3.Zero;
		//		var shapeVolume = 0.0;

		//		var childShapeType = childShape.GetType();
		//		if( childShapeType == typeof( CompoundShape ) )
		//		{
		//			shapeCOM = CalcShapeCOM( (CompoundShape)childShape, out shapeVolume );
		//		}
		//		else
		//		{
		//			shapeCOM = GetCenterOfMass( childShape );

		//			if( volumeDictionary == null )
		//			{
		//				volumeDictionary = new Dictionary<Type, Func<CollisionShape, double>>
		//				{
		//					{ typeof(EmptyShape),                (s) => { return CalcShapeVolume((EmptyShape)s); } },

		//					{ typeof(BulletSharp.SphereShape),   (s) => { return CalcShapeVolume((BulletSharp.SphereShape)s); } },
		//					{ typeof(BulletSharp.BoxShape),      (s) => { return CalcShapeVolume((BulletSharp.BoxShape)s); } },
		//					{ typeof(BulletSharp.CylinderShape), (s) => { return CalcShapeVolume((BulletSharp.CylinderShape)s); } },
		//					{ typeof(BulletSharp.CapsuleShape),  (s) => { return CalcShapeVolume((BulletSharp.CapsuleShape)s); } },
		//					{ typeof(ConeShape),                 (s) => { return CalcShapeVolume((ConeShape)s); } },
		//					{ typeof(ConeShapeX),                (s) => { return CalcShapeVolume((ConeShapeX)s); } },
		//					{ typeof(ConeShapeZ),                (s) => { return CalcShapeVolume((ConeShapeZ)s); } },
		//					{ typeof(MultiSphereShape),          (s) => { return CalcShapeVolume((MultiSphereShape)s); } },
		//					{ typeof(ConvexHullShape),           (s) => { return CalcShapeVolume((ConvexHullShape)s); } },
		//					{ typeof(ConvexTriangleMeshShape),   (s) => { return CalcShapeVolume((ConvexTriangleMeshShape)s); } },

		//					{ typeof(BvhTriangleMeshShape),      (s) => { return CalcShapeVolume((BvhTriangleMeshShape)s); } },
		//					{ typeof(HeightfieldTerrainShape),   (s) => { return CalcShapeVolume((HeightfieldTerrainShape)s); } },
		//					{ typeof(StaticPlaneShape),          (s) => { return CalcShapeVolume((StaticPlaneShape)s); } },
		//				};
		//			}

		//			if( volumeDictionary.ContainsKey( childShapeType ) )
		//				shapeVolume = volumeDictionary[ childShapeType ]( childShape );
		//		}
		//		shapeCOM += childTransform.Origin;

		//		weightedCOM += shapeCOM * shapeVolume;
		//		totalVolume += shapeVolume;
		//	}

		//	Vector3 centerOfMass = weightedCOM / totalVolume;
		//	return ( centerOfMass );
		//}

		//static Vector3 CalcShapeCOM( CompoundShape shape, out double totalShapeVolume )
		//{
		//	Vector3 weightedCOM = Vector3.Zero;
		//	double totalVolume = 0.0;

		//	for( int cI = 0; cI < shape.NumChildShapes; cI++ )
		//	{
		//		var childShape = shape.GetChildShape( cI );
		//		var childTransform = shape.GetChildTransform( cI );
		//		var childShapeType = childShape.GetType();

		//		Vector3 shapeCOM = Vector3.Zero;
		//		var shapeVolume = 0.0;

		//		if( childShapeType == typeof( CompoundShape ) )
		//		{
		//			shapeCOM = CalcShapeCOM( (CompoundShape)childShape, out shapeVolume );
		//		}
		//		else
		//		{
		//			shapeCOM = GetCenterOfMass( childShape );

		//			if( volumeDictionary == null )
		//			{
		//				volumeDictionary = new Dictionary<Type, Func<CollisionShape, double>>
		//				{
		//					{ typeof(EmptyShape),                (s) => { return CalcShapeVolume((EmptyShape)s); } },

		//					{ typeof(BulletSharp.SphereShape),   (s) => { return CalcShapeVolume((BulletSharp.SphereShape)s); } },
		//					{ typeof(BulletSharp.BoxShape),      (s) => { return CalcShapeVolume((BulletSharp.BoxShape)s); } },
		//					{ typeof(BulletSharp.CylinderShape), (s) => { return CalcShapeVolume((BulletSharp.CylinderShape)s); } },
		//					{ typeof(BulletSharp.CapsuleShape),  (s) => { return CalcShapeVolume((BulletSharp.CapsuleShape)s); } },
		//					{ typeof(ConeShape),                 (s) => { return CalcShapeVolume((ConeShape)s); } },
		//					{ typeof(ConeShapeX),                (s) => { return CalcShapeVolume((ConeShapeX)s); } },
		//					{ typeof(ConeShapeZ),                (s) => { return CalcShapeVolume((ConeShapeZ)s); } },
		//					{ typeof(MultiSphereShape),          (s) => { return CalcShapeVolume((MultiSphereShape)s); } },
		//					{ typeof(ConvexHullShape),           (s) => { return CalcShapeVolume((ConvexHullShape)s); } },
		//					{ typeof(ConvexTriangleMeshShape),   (s) => { return CalcShapeVolume((ConvexTriangleMeshShape)s); } },

		//					{ typeof(BvhTriangleMeshShape),      (s) => { return CalcShapeVolume((BvhTriangleMeshShape)s); } },
		//					{ typeof(HeightfieldTerrainShape),   (s) => { return CalcShapeVolume((HeightfieldTerrainShape)s); } },
		//					{ typeof(StaticPlaneShape),          (s) => { return CalcShapeVolume((StaticPlaneShape)s); } },
		//				};
		//			}

		//			if( volumeDictionary.ContainsKey( childShapeType ) )
		//				shapeVolume = volumeDictionary[ childShapeType ]( shape );
		//		}

		//		Vector4 temp = Vector3.Transform( shapeCOM, childTransform );
		//		shapeCOM.X = temp.X;
		//		shapeCOM.Y = temp.Y;
		//		shapeCOM.Z = temp.Z;

		//		weightedCOM += shapeCOM * shapeVolume;
		//		totalVolume += shapeVolume;
		//	}
		//	totalShapeVolume = totalVolume;

		//	Vector3 centerOfMass = weightedCOM / totalVolume;
		//	return ( centerOfMass );
		//}

		//static double CalcShapeVolume( EmptyShape shape )
		//{
		//	return 0.0;
		//}

		//static double CalcShapeVolume( BulletSharp.SphereShape shape )
		//{
		//	double R = shape.Radius;
		//	double V = ( 4.0 / 3.0 ) * Math.PI * R * R * R;
		//	return V;
		//}
		//static double CalcShapeVolume( BulletSharp.BoxShape shape )
		//{
		//	Vector3 min;
		//	shape.GetVertex( 0, out min );
		//	Vector3 max;
		//	shape.GetVertex( 0, out max );
		//	for( int cI = 1; cI < shape.NumVertices; cI++ )
		//	{
		//		Vector3 tmp;
		//		shape.GetVertex( cI, out tmp );
		//		min = Vector3.Min( min, tmp );
		//		max = Vector3.Max( max, tmp );
		//	}
		//	Vector3 size = max - min;

		//	double V = size.X * size.Y * size.Z;
		//	return V;
		//}
		//static double CalcShapeVolume( BulletSharp.CylinderShape shape )
		//{
		//	var extents = shape.HalfExtentsWithMargin;
		//	double r = extents.X;
		//	double h = 2.0 * extents.Y;
		//	double V = Math.PI * r * r * h;
		//	return V;
		//}
		//static double CalcShapeVolume( BulletSharp.CapsuleShape shape )
		//{
		//	double r = shape.Radius;
		//	double h = 2.0 * shape.HalfHeight;

		//	double V = Math.PI * r * r * h + ( 4.0 / 3.0 ) * Math.PI * r * r * r;
		//	return V;
		//}

		//static double CalcShapeVolume( ConeShape shape )
		//{
		//	double r = shape.Radius;
		//	double h = shape.Height;
		//	double V = ( 1.0 / 3.0 ) * Math.PI * r * r * h;
		//	return V;
		//}
		//static double CalcShapeVolume( ConeShapeX shape )
		//{
		//	double r = shape.Radius;
		//	double h = shape.Height;
		//	double V = ( 1.0 / 3.0 ) * Math.PI * r * r * h;
		//	return V;
		//}
		//static double CalcShapeVolume( ConeShapeZ shape )
		//{
		//	double r = shape.Radius;
		//	double h = shape.Height;
		//	double V = ( 1.0 / 3.0 ) * Math.PI * r * r * h;
		//	return V;
		//}

		//static double CalcShapeVolume( MultiSphereShape shape )
		//{
		//	Vector3 center = Vector3.Zero;
		//	double R = 0.0;
		//	shape.GetBoundingSphere( out center, out R );

		//	double V = ( 4.0 / 3.0 ) * Math.PI * R * R * R; // approximately
		//	return V;
		//}

		//static double CalcShapeVolume( ConvexHullShape shape )
		//{
		//	ShapeHull sh = new ShapeHull( shape );
		//	sh.BuildHull( shape.Margin );

		//	double vol = 0.0;
		//	for( int nTriangle = 0; nTriangle < sh.NumTriangles; nTriangle++ )
		//	{
		//		uint index0 = sh.Indices[ nTriangle * 3 + 0 ];
		//		uint index1 = sh.Indices[ nTriangle * 3 + 1 ];
		//		uint index2 = sh.Indices[ nTriangle * 3 + 2 ];

		//		Vector3 point0 = sh.Vertices[ (int)index0 ];
		//		Vector3 point1 = sh.Vertices[ (int)index1 ];
		//		Vector3 point2 = sh.Vertices[ (int)index2 ];

		//		vol += Determinant( point0, point1, point2 );
		//	}

		//	return vol / 6.0f; // since the determinant give 6 times tetra volume
		//}

		//static double CalcShapeVolume( ConvexTriangleMeshShape shape )
		//{
		//	ShapeHull sh = new ShapeHull( shape );
		//	sh.BuildHull( shape.Margin );

		//	double vol = 0.0;
		//	for( int nTriangle = 0; nTriangle < sh.NumTriangles; nTriangle++ )
		//	{
		//		uint index0 = sh.Indices[ nTriangle * 3 + 0 ];
		//		uint index1 = sh.Indices[ nTriangle * 3 + 1 ];
		//		uint index2 = sh.Indices[ nTriangle * 3 + 2 ];

		//		Vector3 point0 = sh.Vertices[ (int)index0 ];
		//		Vector3 point1 = sh.Vertices[ (int)index1 ];
		//		Vector3 point2 = sh.Vertices[ (int)index2 ];

		//		vol += Determinant( point0, point1, point2 );
		//	}

		//	return vol / 6.0f; // since the determinant give 6 times tetra volume
		//}

		//static double CalcShapeVolume( BvhTriangleMeshShape shape )
		//{
		//	// can be concave!
		//	Vector3 aabbMin;
		//	Vector3 aabbMax;
		//	Matrix m = Matrix.Identity;
		//	shape.GetAabb( m, out aabbMin, out aabbMax );

		//	Vector3 size = aabbMax - aabbMin;
		//	double V = size.X * size.Y * size.Z;
		//	return V;
		//}
		//static double CalcShapeVolume( HeightfieldTerrainShape shape )
		//{
		//	Vector3 aabbMin;
		//	Vector3 aabbMax;
		//	Matrix m = Matrix.Identity;
		//	shape.GetAabb( m, out aabbMin, out aabbMax );

		//	Vector3 size = aabbMax - aabbMin;
		//	double V = size.X * size.Y * size.Z;
		//	return V;
		//}
		//static double CalcShapeVolume( StaticPlaneShape shape )
		//{
		//	Vector3 aabbMin;
		//	Vector3 aabbMax;
		//	Matrix m = Matrix.Identity;
		//	shape.GetAabb( m, out aabbMin, out aabbMax );

		//	Vector3 size = aabbMax - aabbMin;
		//	double V = size.X * size.Y * size.Z;
		//	return V;
		//}

		//static double Determinant( Vector3 row0, Vector3 row1, Vector3 row2 )
		//{
		//	// row0 (a, b, c)
		//	// row1 (d, e, f)
		//	// row2 (g, h, i)

		//	double v0 = row0.X * ( row1.Y * row2.Z - row1.Z * row2.Y );
		//	double v1 = row0.Y * ( row1.X * row2.Z - row1.Z * row2.X );
		//	double v2 = row0.Z * ( row1.X * row2.Y - row1.Y * row2.X );

		//	double A = v0 - v1 + v2;
		//	return A;
		//}
	}
}
