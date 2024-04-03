// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The list of modes of physical object's axis.
	/// </summary>
	public enum PhysicsAxisMode
	{
		Locked,
		Limited,
		Free,
	}

	public enum PhysicsMotionType
	{
		Static,
		Kinematic,
		Dynamic,
	}

	public enum PhysicsMotionQuality
	{
		Discrete,
		LinearCast,
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The data to perform a search query of physical objects by ray.
	/// </summary>
	public class PhysicsRayTestItem
	{
		//input
		public Ray Ray;
		public ModeEnum Mode;
		public FlagsEnum Flags;
		//!!!!impl
		public int CollisionFilterGroup = 1;
		public int CollisionFilterMask = -1;

		//output
		public ResultItem[] Result;

		//addition
		public object UserData;

		////////////////

		public enum ModeEnum
		{
			One,
			OneClosest,
			OneForEach,
			OneClosestForEach,
			All
		}

		////////////////

		[Flags]
		public enum FlagsEnum
		{
			None = 0,
			CalculateNormal = 1,

			//!!!!triangle id?
		}

		////////////////

		//!!!!struct?
		/// <summary>
		/// Represents a result item for <see cref="PhysicsRayTestItem"/>.
		/// </summary>
		public struct ResultItem
		{
			public Scene.PhysicsWorldClass.Body Body;
			//!!!!impl
			//public int ShapeIndex;//public CollisionShape Shape;
			public Vector3 Position;
			public float DistanceScale;
			public Vector3F Normal;
			//!!!!impl
			//public int TriangleIndexSource;
			//public int TriangleIndexProcessed;
		}

		////////////////

		public PhysicsRayTestItem()
		{
		}

		public PhysicsRayTestItem( Ray ray, ModeEnum mode, FlagsEnum flags, int collisionFilterGroup = 1, int collisionFilterMask = -1 )
		{
			Ray = ray;
			Mode = mode;
			Flags = flags;
			CollisionFilterGroup = collisionFilterGroup;
			CollisionFilterMask = collisionFilterMask;
		}
		//public PhysicsRayTestItem( Ray ray, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode )
		//{
		//	this.Ray = ray;
		//	this.CollisionFilterGroup = collisionFilterGroup;
		//	this.CollisionFilterMask = collisionFilterMask;
		//	this.Mode = mode;
		//}

		//public PhysicsRayTestItem( Ray ray, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, IntPtr/*CollisionObject*/ singleCastCollisionObject )
		//{
		//	this.Ray = ray;
		//	this.CollisionFilterGroup = collisionFilterGroup;
		//	this.CollisionFilterMask = collisionFilterMask;
		//	this.Mode = mode;
		//	this.SingleCastCollisionObject = singleCastCollisionObject;
		//}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The data to perform a search query of physical objects at a specified volume.
	/// </summary>
	public class PhysicsVolumeTestItem
	{
		//input

		public Sphere? ShapeSphere;
		public Box? ShapeBox;
		public Capsule? ShapeCapsule;
		public Cylinder? ShapeCylinder;

		public Vector3 Direction;
		public ModeEnum Mode;

		//!!!!impl
		public int CollisionFilterGroup = 1;
		public int CollisionFilterMask = -1;

		//!!!!
		//public CollisionObject CollisionObject { get; set; }
		//public bool CollisionObjectAutoDispose { get; set; }
		//public Predicate<Vector3> CheckPositionWorldOnB;
		//public double ClosestDistanceThreshold { get; set; }

		//output
		public ResultItem[] Result;

		//addition
		public object UserData;

		//how make it better?
		//public ShapeTypeEnum ShapeType;
		//public Sphere ShapeSphere;

		////////////////

		//public enum ShapeTypeEnum
		//{
		//	Sphere,
		//}

		////////////////

		public enum ModeEnum
		{
			One,
			OneClosest,
			OneForEach,
			OneClosestForEach,
			All
		}

		////////////////

		//[Flags]
		//public enum FlagsEnum
		//{
		//	None = 0,
		//	CollideWithBackFaces = 1,
		//}

		////////////////

		/// <summary>
		/// Represents a result item for <see cref="PhysicsVolumeTestItem"/>.
		/// </summary>
		public struct ResultItem
		{
			public Scene.PhysicsWorldClass.Body Body;
			public float DistanceScale;
			//public bool BackFaceHit;

			//!!!!если иметь shape index, тогда может быть несколько результатов на одно тело
			//public int ShapeIndex;
			//public PhysicalBody Body { get; set; }
			//public CollisionShape Shape { get; set; }

			//public Vector3 LocalPointA { get; set; }
			//public Vector3 PositionWorldOnA { get; set; }

			//public Vector3 LocalPointB { get; set; }
			//public Vector3 PositionWorldOnB { get; set; }

			////public Vec3 Normal { get; set; }
			//public double Distance { get; set; }

			//public int TriangleIndexSource { get; set; }
			//public int TriangleIndexProcessed { get; set; }
		}

		////////////////

		public PhysicsVolumeTestItem()
		{
			//this.ContactGroup = CollisionFilterGroups.DefaultFilter;
		}

		void Construct( Vector3 direction, ModeEnum mode, int collisionFilterGroup, int collisionFilterMask )
		{
			Direction = direction;
			Mode = mode;
			CollisionFilterGroup = collisionFilterGroup;
			CollisionFilterMask = collisionFilterMask;

			//CollisionObject = collisionObject;
			//CollisionObjectAutoDispose = collisionObjectAutoDispose;
			//CheckPositionWorldOnB = checkPositionWorldOnB;
		}

		//void Construct( int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, object/*!!!!CollisionObject*/ collisionObject, bool collisionObjectAutoDispose, Predicate<Vector3> checkPositionWorldOnB )
		//{
		//	CollisionFilterGroup = collisionFilterGroup;
		//	CollisionFilterMask = collisionFilterMask;
		//	Mode = mode;
		//	//!!!!
		//	//CollisionObject = collisionObject;
		//	//CollisionObjectAutoDispose = collisionObjectAutoDispose;
		//	//CheckPositionWorldOnB = checkPositionWorldOnB;
		//}

		//public PhysicsContactTestItem( int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, object/*!!!!CollisionObject*/  collisionObject, bool collisionObjectAutoDispose )
		//{
		//	Construct( collisionFilterGroup, collisionFilterMask, mode, collisionObject, collisionObjectAutoDispose, null );
		//}

		public PhysicsVolumeTestItem( Sphere sphere, Vector3 direction, ModeEnum mode, int collisionFilterGroup = 1, int collisionFilterMask = -1 )
		{
			Construct( direction, mode, collisionFilterGroup, collisionFilterMask );
			ShapeSphere = sphere;

			//var geometry = sphere;
			//geometry.Radius += 0.001;
			//var collisionObject = new CollisionObject();
			//collisionObject.CollisionShape = new Internal.BulletSharp.SphereShape( sphere.Radius );
			//collisionObject.WorldTransform = BMatrix.Translation( BulletPhysicsUtility.Convert( sphere.Center ) );
			//Construct( collisionFilterGroup, collisionFilterMask, mode, collisionObject, true, point => geometry.Contains( point ) );
		}

		public PhysicsVolumeTestItem( Box box, Vector3 direction, ModeEnum mode, int collisionFilterGroup = 1, int collisionFilterMask = -1 )
		{
			Construct( direction, mode, collisionFilterGroup, collisionFilterMask );
			ShapeBox = box;

			//var geometry = box;
			//geometry.Extents += new Vector3( 0.001, 0.001, 0.001 );

			//var collisionObject = new CollisionObject();
			//collisionObject.CollisionShape = new Internal.BulletSharp.BoxShape( BulletPhysicsUtility.Convert( box.Extents ) );
			//collisionObject.WorldTransform = BulletPhysicsUtility.Convert( new Matrix4( box.Axis, box.Center ) );
			//Construct( collisionFilterGroup, collisionFilterMask, mode, collisionObject, true, point => geometry.Contains( point ) );
		}

		public PhysicsVolumeTestItem( Bounds bounds, Vector3 direction, ModeEnum mode, int collisionFilterGroup = 1, int collisionFilterMask = -1 )
		{
			Construct( direction, mode, collisionFilterGroup, collisionFilterMask );
			ShapeBox = new Box( bounds );

			//var geometry = bounds;
			//geometry.Expand( 0.001 );

			//var collisionObject = new CollisionObject();
			//collisionObject.CollisionShape = new Internal.BulletSharp.BoxShape( BulletPhysicsUtility.Convert( bounds.GetSize() * 0.5 ) );
			//collisionObject.WorldTransform = BulletPhysicsUtility.Convert( Matrix4.FromTranslate( bounds.GetCenter() ) );
			//Construct( collisionFilterGroup, collisionFilterMask, mode, collisionObject, true, point => geometry.Contains( point ) );
		}

		public PhysicsVolumeTestItem( Capsule capsule, Vector3 direction, ModeEnum mode, int collisionFilterGroup = 1, int collisionFilterMask = -1 )
		{
			Construct( direction, mode, collisionFilterGroup, collisionFilterMask );
			ShapeCapsule = capsule;

			//var geometry = capsule;
			//geometry.Radius += 0.001;

			//var collisionObject = new CollisionObject();

			//if( capsule.Point1.ToVector2().Equals( capsule.Point2.ToVector2(), 0.0001 ) )
			//{
			//	collisionObject.CollisionShape = new Internal.BulletSharp.CapsuleShapeZ( capsule.Radius, capsule.GetLength() );
			//	collisionObject.WorldTransform = BulletPhysicsUtility.Convert( Matrix4.FromTranslate( capsule.GetCenter() ) );
			//}
			//else
			//{
			//	collisionObject.CollisionShape = new Internal.BulletSharp.CapsuleShapeX( capsule.Radius, capsule.GetLength() );
			//	collisionObject.WorldTransform = BulletPhysicsUtility.Convert( new Matrix4( Quaternion.FromDirectionZAxisUp( capsule.GetDirection() ).ToMatrix3(), capsule.GetCenter() ) );
			//}

			//Construct( collisionFilterGroup, collisionFilterMask, mode, collisionObject, true, point => geometry.Contains( point ) );
		}

		public PhysicsVolumeTestItem( Cylinder cylinder, Vector3 direction, ModeEnum mode, int collisionFilterGroup = 1, int collisionFilterMask = -1 )
		{
			ShapeCylinder = cylinder;

			//cylinder works good without additional check for position of point on B.

			//var geometry = cylinder;
			//var offset = geometry.GetDirection() * 0.001;
			//geometry.Point1 -= offset;
			//geometry.Point2 += offset;
			//geometry.Radius += 0.001;

			//!!!!

			//var collisionObject = new CollisionObject();

			//if( cylinder.Point1.ToVector2().Equals( cylinder.Point2.ToVector2(), 0.0001 ) )
			//{
			//	collisionObject.CollisionShape = new Internal.BulletSharp.CylinderShapeZ( cylinder.Radius, cylinder.Radius, cylinder.GetLength() * 0.5 );
			//	collisionObject.WorldTransform = BulletPhysicsUtility.Convert( Matrix4.FromTranslate( cylinder.GetCenter() ) );
			//}
			//else
			//{
			//	collisionObject.CollisionShape = new Internal.BulletSharp.CylinderShapeX( cylinder.GetLength() * 0.5, cylinder.Radius, cylinder.Radius );
			//	collisionObject.WorldTransform = BulletPhysicsUtility.Convert( new Matrix4( Quaternion.FromDirectionZAxisUp( cylinder.GetDirection() ).ToMatrix3(), cylinder.GetCenter() ) );
			//}

			//Construct( collisionFilterGroup, collisionFilterMask, mode, collisionObject, true, null );// point => geometry.Contains( point ) );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//!!!!impl

	////!!!!в Scene?
	///// <summary>
	///// The data to perform a search query of physical objects at a specified volume which is defined as transfer an object from one point to another.
	///// </summary>
	//public class PhysicsConvexSweepTestItem
	//{
	//	//input
	//	internal Matrix4 originalFrom;
	//	public Matrix4 OriginalFrom { get { return originalFrom; } set { originalFrom = value; } }
	//	internal Matrix4 originalTo;
	//	public Matrix4 OriginalTo { get { return originalTo; } set { originalTo = value; } }

	//	internal Matrix4 transformedFrom;
	//	public Matrix4 TransformedFrom { get { return transformedFrom; } set { transformedFrom = value; } }
	//	internal Matrix4 transformedTo;
	//	public Matrix4 TransformedTo { get { return transformedTo; } set { transformedTo = value; } }

	//	public int CollisionFilterGroup { get; set; } = 1;
	//	public int CollisionFilterMask { get; set; } = -1;
	//	public ModeEnum Mode { get; set; }
	//	//!!!!
	//	public object Shape { get; set; }
	//	//public ConvexShape Shape { get; set; }
	//	public bool ShapeAutoDispose { get; set; }

	//	public double AllowedCcdPenetration { get; set; }

	//	//output
	//	volatile ResultItem[] result;
	//	public ResultItem[] Result { get { return result; } set { result = value; } }

	//	//addition
	//	public object UserData { get; set; }

	//	////////////////

	//	public enum ModeEnum
	//	{
	//		One,
	//		OneClosest,
	//		OneForEach,
	//		OneClosestForEach,
	//		All
	//	}

	//	////////////////

	//	/// <summary>
	//	/// Represents a result item for <see cref="PhysicsConvexSweepTestItem"/>.
	//	/// </summary>
	//	public class ResultItem
	//	{
	//		//!!!!все параметры

	//		//!!!!!
	//		public Scene.PhysicsWorldClass.Body Body { get; set; }
	//		//public ObjectInSpace Body { get; set; }
	//		//public CollisionShape Shape { get; set; }

	//		public Vector3 Position { get; set; }
	//		public Vector3 Normal { get; set; }
	//		public double DistanceScale { get; set; }
	//		//public Vec3 HitPoint { get; set; }

	//		public int TriangleIndexSource { get; set; }
	//		public int TriangleIndexProcessed { get; set; }
	//	}

	//	////////////////

	//	public PhysicsConvexSweepTestItem()
	//	{
	//	}

	//	//!!!!что-то еще?
	//	public PhysicsConvexSweepTestItem( Matrix4 from, Matrix4 to, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, object/*!!!!ConvexShape*/ shape, bool shapeAutoDispose )
	//	{
	//		this.OriginalFrom = from;
	//		this.OriginalTo = to;
	//		this.CollisionFilterGroup = collisionFilterGroup;
	//		this.CollisionFilterMask = collisionFilterMask;
	//		this.Mode = mode;
	//		this.Shape = shape;
	//		this.ShapeAutoDispose = shapeAutoDispose;
	//	}

	//	public PhysicsConvexSweepTestItem( Matrix4 from, Matrix4 to, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, Box box )
	//	{
	//		this.originalFrom = from;
	//		this.originalTo = to;
	//		this.CollisionFilterGroup = collisionFilterGroup;
	//		this.CollisionFilterMask = collisionFilterMask;
	//		this.Mode = mode;

	//		Matrix4 offset = new Matrix4( box.Axis, box.Center );
	//		transformedFrom = originalFrom * offset;
	//		transformedTo = originalTo * offset;

	//		//!!!!
	//		//Shape = new Internal.BulletSharp.BoxShape( BulletPhysicsUtility.Convert( box.Extents ) );
	//		ShapeAutoDispose = true;
	//	}

	//	public PhysicsConvexSweepTestItem( Matrix4 from, Matrix4 to, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, Bounds bounds )
	//	{
	//		this.originalFrom = from;
	//		this.originalTo = to;
	//		this.CollisionFilterGroup = collisionFilterGroup;
	//		this.CollisionFilterMask = collisionFilterMask;
	//		this.Mode = mode;

	//		transformedFrom = originalFrom;
	//		transformedTo = originalTo;

	//		Vector3 offset = bounds.GetCenter();
	//		if( !offset.Equals( Vector3.Zero, MathEx.Epsilon ) )
	//		{
	//			transformedFrom.SetTranslation( transformedFrom.GetTranslation() + offset );
	//			transformedTo.SetTranslation( transformedTo.GetTranslation() + offset );
	//		}

	//		//!!!!
	//		//var halfSize = bounds.GetSize() / 2;
	//		//Shape = new Internal.BulletSharp.BoxShape( BulletPhysicsUtility.Convert( halfSize ) );
	//		ShapeAutoDispose = true;
	//	}

	//	public PhysicsConvexSweepTestItem( Matrix4 from, Matrix4 to, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, Sphere sphere )
	//	{
	//		this.originalFrom = from;
	//		this.originalTo = to;
	//		this.CollisionFilterGroup = collisionFilterGroup;
	//		this.CollisionFilterMask = collisionFilterMask;
	//		this.Mode = mode;

	//		transformedFrom = originalFrom;
	//		transformedTo = originalTo;

	//		Vector3 offset = sphere.Center;
	//		if( offset != Vector3.Zero )
	//		{
	//			transformedFrom.SetTranslation( transformedFrom.GetTranslation() + offset );
	//			transformedTo.SetTranslation( transformedTo.GetTranslation() + offset );
	//		}

	//		//!!!!
	//		//Shape = new Internal.BulletSharp.SphereShape( sphere.Radius );
	//		ShapeAutoDispose = true;
	//	}
	//}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// An interface for accessing a component of a physical object.
	/// </summary>
	public interface IPhysicalObject
	{
		//!!!!need?
		//void RenderPhysicalObject( ViewportRenderingContext context, out int verticesRendered );
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//public interface ISoftBody
	//{
	//	[Browsable( false )]
	//	CollisionObject BulletBody { get; }

	//	Vector3 GetNodePosition( int nodeIndex );
	//	int FindClosestNodeIndex( Vector3 worldPosition );

	//	bool GetProcessedData( out Vector3F[] processedVertices, /*out Vec3F[] processedVerticesNormals, */out int[] processedIndices, out int[] processedTrianglesToSourceIndex );
	//}

}
