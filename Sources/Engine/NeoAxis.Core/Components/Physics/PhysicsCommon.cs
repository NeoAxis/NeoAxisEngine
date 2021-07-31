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
	/// The list of modes of physical object's axis.
	/// </summary>
	public enum PhysicsAxisMode
	{
		Locked,
		Limited,
		Free,
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The data to perform a search query of physical objects by ray.
	/// </summary>
	public class PhysicsRayTestItem
	{
		//input
		public Ray Ray { get; set; }
		public int CollisionFilterGroup { get; set; } = 1;
		public int CollisionFilterMask { get; set; } = -1;
		public ModeEnum Mode { get; set; }
		public CollisionObject SingleCastCollisionObject { get; set; }

		//output
		public ResultItem[] Result { get; set; }

		//addition
		public object UserData { get; set; }

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

		/// <summary>
		/// Represents a result item for <see cref="PhysicsRayTestItem"/>.
		/// </summary>
		public class ResultItem
		{
			public Component_ObjectInSpace Body { get; set; }
			public Component_CollisionShape Shape { get; set; }
			public Vector3 Position { get; set; }
			public Vector3 Normal { get; set; }
			public double DistanceScale { get; set; }
			public int TriangleIndexSource { get; set; }
			public int TriangleIndexProcessed { get; set; }

		}

		////////////////

		public PhysicsRayTestItem()
		{
		}

		public PhysicsRayTestItem( Ray ray, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode )
		{
			this.Ray = ray;
			this.CollisionFilterGroup = collisionFilterGroup;
			this.CollisionFilterMask = collisionFilterMask;
			this.Mode = mode;
		}

		public PhysicsRayTestItem( Ray ray, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, CollisionObject singleCastCollisionObject )
		{
			this.Ray = ray;
			this.CollisionFilterGroup = collisionFilterGroup;
			this.CollisionFilterMask = collisionFilterMask;
			this.Mode = mode;
			this.SingleCastCollisionObject = singleCastCollisionObject;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// The data to perform a search query of physical objects at a specified volume.
	/// </summary>
	public class PhysicsContactTestItem
	{
		//input
		public int CollisionFilterGroup { get; set; } = 1;
		public int CollisionFilterMask { get; set; } = -1;
		public ModeEnum Mode { get; set; }
		public CollisionObject CollisionObject { get; set; }
		public bool CollisionObjectAutoDispose { get; set; }
		public Predicate<Vector3> CheckPositionWorldOnB;

		public double ClosestDistanceThreshold { get; set; }

		//output
		volatile ResultItem[] result;
		public ResultItem[] Result { get { return result; } set { result = value; } }

		//addition
		public object UserData { get; set; }

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

		/// <summary>
		/// Represents a result item for <see cref="PhysicsContactTestItem"/>.
		/// </summary>
		public class ResultItem
		{
			public Component_PhysicalBody Body { get; set; }
			public Component_CollisionShape Shape { get; set; }

			public Vector3 LocalPointA { get; set; }
			public Vector3 PositionWorldOnA { get; set; }

			public Vector3 LocalPointB { get; set; }
			public Vector3 PositionWorldOnB { get; set; }

			//public Vec3 Normal { get; set; }
			public double Distance { get; set; }

			public int TriangleIndexSource { get; set; }
			public int TriangleIndexProcessed { get; set; }
		}

		////////////////

		public PhysicsContactTestItem()
		{
			//this.ContactGroup = CollisionFilterGroups.DefaultFilter;
		}

		void Construct( int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, CollisionObject collisionObject, bool collisionObjectAutoDispose, Predicate<Vector3> checkPositionWorldOnB )
		{
			CollisionFilterGroup = collisionFilterGroup;
			CollisionFilterMask = collisionFilterMask;
			Mode = mode;
			CollisionObject = collisionObject;
			CollisionObjectAutoDispose = collisionObjectAutoDispose;
			CheckPositionWorldOnB = checkPositionWorldOnB;
		}

		public PhysicsContactTestItem( int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, CollisionObject collisionObject, bool collisionObjectAutoDispose )
		{
			Construct( collisionFilterGroup, collisionFilterMask, mode, collisionObject, collisionObjectAutoDispose, null );
		}

		public PhysicsContactTestItem( int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, Sphere sphere )
		{
			var geometry = sphere;
			geometry.Radius += 0.001;

			var collisionObject = new CollisionObject();
			collisionObject.CollisionShape = new BulletSharp.SphereShape( sphere.Radius );
			collisionObject.WorldTransform = Matrix.Translation( BulletPhysicsUtility.Convert( sphere.Origin ) );
			Construct( collisionFilterGroup, collisionFilterMask, mode, collisionObject, true, point => geometry.Contains( point ) );
		}

		public PhysicsContactTestItem( int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, Box box )
		{
			var geometry = box;
			geometry.Extents += new Vector3( 0.001, 0.001, 0.001 );

			var collisionObject = new CollisionObject();
			collisionObject.CollisionShape = new BulletSharp.BoxShape( BulletPhysicsUtility.Convert( box.Extents ) );
			collisionObject.WorldTransform = BulletPhysicsUtility.Convert( new Matrix4( box.Axis, box.Center ) );
			Construct( collisionFilterGroup, collisionFilterMask, mode, collisionObject, true, point => geometry.Contains( point ) );
		}

		public PhysicsContactTestItem( int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, Bounds bounds )
		{
			var geometry = bounds;
			geometry.Expand( 0.001 );

			var collisionObject = new CollisionObject();
			collisionObject.CollisionShape = new BulletSharp.BoxShape( BulletPhysicsUtility.Convert( bounds.GetSize() * 0.5 ) );
			collisionObject.WorldTransform = BulletPhysicsUtility.Convert( Matrix4.FromTranslate( bounds.GetCenter() ) );
			Construct( collisionFilterGroup, collisionFilterMask, mode, collisionObject, true, point => geometry.Contains( point ) );
		}

		public PhysicsContactTestItem( int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, Capsule capsule )
		{
			var geometry = capsule;
			geometry.Radius += 0.001;

			var collisionObject = new CollisionObject();

			if( capsule.Point1.ToVector2().Equals( capsule.Point2.ToVector2(), 0.0001 ) )
			{
				collisionObject.CollisionShape = new BulletSharp.CapsuleShapeZ( capsule.Radius, capsule.GetLength() );
				collisionObject.WorldTransform = BulletPhysicsUtility.Convert( Matrix4.FromTranslate( capsule.GetCenter() ) );
			}
			else
			{
				collisionObject.CollisionShape = new BulletSharp.CapsuleShapeX( capsule.Radius, capsule.GetLength() );
				collisionObject.WorldTransform = BulletPhysicsUtility.Convert( new Matrix4( Quaternion.FromDirectionZAxisUp( capsule.GetDirection() ).ToMatrix3(), capsule.GetCenter() ) );
			}

			Construct( collisionFilterGroup, collisionFilterMask, mode, collisionObject, true, point => geometry.Contains( point ) );
		}

		public PhysicsContactTestItem( int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, Cylinder cylinder )
		{
			//cylinder works good without additional check for position of point on B.

			//var geometry = cylinder;
			//var offset = geometry.GetDirection() * 0.001;
			//geometry.Point1 -= offset;
			//geometry.Point2 += offset;
			//geometry.Radius += 0.001;

			var collisionObject = new CollisionObject();

			if( cylinder.Point1.ToVector2().Equals( cylinder.Point2.ToVector2(), 0.0001 ) )
			{
				collisionObject.CollisionShape = new BulletSharp.CylinderShapeZ( cylinder.Radius, cylinder.Radius, cylinder.GetLength() * 0.5 );
				collisionObject.WorldTransform = BulletPhysicsUtility.Convert( Matrix4.FromTranslate( cylinder.GetCenter() ) );
			}
			else
			{
				collisionObject.CollisionShape = new BulletSharp.CylinderShapeX( cylinder.GetLength() * 0.5, cylinder.Radius, cylinder.Radius );
				collisionObject.WorldTransform = BulletPhysicsUtility.Convert( new Matrix4( Quaternion.FromDirectionZAxisUp( cylinder.GetDirection() ).ToMatrix3(), cylinder.GetCenter() ) );
			}

			Construct( collisionFilterGroup, collisionFilterMask, mode, collisionObject, true, null );// point => geometry.Contains( point ) );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	//!!!!в Component_Scene?
	/// <summary>
	/// The data to perform a search query of physical objects at a specified volume which is defined as transfer an object from one point to another.
	/// </summary>
	public class PhysicsConvexSweepTestItem
	{
		//input
		internal Matrix4 originalFrom;
		public Matrix4 OriginalFrom { get { return originalFrom; } set { originalFrom = value; } }
		internal Matrix4 originalTo;
		public Matrix4 OriginalTo { get { return originalTo; } set { originalTo = value; } }

		internal Matrix4 transformedFrom;
		public Matrix4 TransformedFrom { get { return transformedFrom; } set { transformedFrom = value; } }
		internal Matrix4 transformedTo;
		public Matrix4 TransformedTo { get { return transformedTo; } set { transformedTo = value; } }

		public int CollisionFilterGroup { get; set; } = 1;
		public int CollisionFilterMask { get; set; } = -1;
		public ModeEnum Mode { get; set; }
		public ConvexShape Shape { get; set; }
		public bool ShapeAutoDispose { get; set; }

		public double AllowedCcdPenetration { get; set; }

		//output
		volatile ResultItem[] result;
		public ResultItem[] Result { get { return result; } set { result = value; } }

		//addition
		public object UserData { get; set; }

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

		/// <summary>
		/// Represents a result item for <see cref="PhysicsConvexSweepTestItem"/>.
		/// </summary>
		public class ResultItem
		{
			public Component_ObjectInSpace Body { get; set; }
			public Component_CollisionShape Shape { get; set; }
			public Vector3 Position { get; set; }
			public Vector3 Normal { get; set; }
			public double DistanceScale { get; set; }
			//public Vec3 HitPoint { get; set; }

			public int TriangleIndexSource { get; set; }
			public int TriangleIndexProcessed { get; set; }
		}

		////////////////

		public PhysicsConvexSweepTestItem()
		{
		}

		//!!!!что-то еще?
		public PhysicsConvexSweepTestItem( Matrix4 from, Matrix4 to, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, ConvexShape shape, bool shapeAutoDispose )
		{
			this.OriginalFrom = from;
			this.OriginalTo = to;
			this.CollisionFilterGroup = collisionFilterGroup;
			this.CollisionFilterMask = collisionFilterMask;
			this.Mode = mode;
			this.Shape = shape;
			this.ShapeAutoDispose = shapeAutoDispose;
		}

		public PhysicsConvexSweepTestItem( Matrix4 from, Matrix4 to, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, Box box )
		{
			this.originalFrom = from;
			this.originalTo = to;
			this.CollisionFilterGroup = collisionFilterGroup;
			this.CollisionFilterMask = collisionFilterMask;
			this.Mode = mode;

			Matrix4 offset = new Matrix4( box.Axis, box.Center );
			transformedFrom = originalFrom * offset;
			transformedTo = originalTo * offset;

			Shape = new BulletSharp.BoxShape( BulletPhysicsUtility.Convert( box.Extents ) );
			ShapeAutoDispose = true;
		}

		public PhysicsConvexSweepTestItem( Matrix4 from, Matrix4 to, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, Bounds bounds )
		{
			this.originalFrom = from;
			this.originalTo = to;
			this.CollisionFilterGroup = collisionFilterGroup;
			this.CollisionFilterMask = collisionFilterMask;
			this.Mode = mode;

			transformedFrom = originalFrom;
			transformedTo = originalTo;

			Vector3 offset = bounds.GetCenter();
			if( !offset.Equals( Vector3.Zero, MathEx.Epsilon ) )
			{
				transformedFrom.SetTranslation( transformedFrom.GetTranslation() + offset );
				transformedTo.SetTranslation( transformedTo.GetTranslation() + offset );
			}

			var halfSize = bounds.GetSize() / 2;
			Shape = new BulletSharp.BoxShape( BulletPhysicsUtility.Convert( halfSize ) );
			ShapeAutoDispose = true;
		}

		public PhysicsConvexSweepTestItem( Matrix4 from, Matrix4 to, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, Sphere sphere )
		{
			this.originalFrom = from;
			this.originalTo = to;
			this.CollisionFilterGroup = collisionFilterGroup;
			this.CollisionFilterMask = collisionFilterMask;
			this.Mode = mode;

			transformedFrom = originalFrom;
			transformedTo = originalTo;

			Vector3 offset = sphere.Origin;
			if( offset != Vector3.Zero )
			{
				transformedFrom.SetTranslation( transformedFrom.GetTranslation() + offset );
				transformedTo.SetTranslation( transformedTo.GetTranslation() + offset );
			}

			Shape = new BulletSharp.SphereShape( sphere.Radius );
			ShapeAutoDispose = true;
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// An interface for accessing a component of a physical object.
	/// </summary>
	public interface Component_IPhysicalObject
	{
		void Render( ViewportRenderingContext context, out int verticesRendered );
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public interface IComponent_SoftBody
	{
		[Browsable( false )]
		CollisionObject BulletBody { get; }

		Vector3 GetNodePosition( int nodeIndex );
		int FindClosestNodeIndex( Vector3 worldPosition );

		bool GetProcessedData( out Vector3F[] processedVertices, /*out Vec3F[] processedVerticesNormals, */out int[] processedIndices, out int[] processedTrianglesToSourceIndex );
	}

}
