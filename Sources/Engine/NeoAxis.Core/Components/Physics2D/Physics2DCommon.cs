// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	[Flags]
	public enum Physics2DCategories
	{
		None = 0x00000000,
		Category1 = 0x00000001,
		Category2 = 0x00000002,
		Category3 = 0x00000004,
		Category4 = 0x00000008,
		Category5 = 0x00000010,
		Category6 = 0x00000020,
		Category7 = 0x00000040,
		Category8 = 0x00000080,
		Category9 = 0x00000100,
		Category10 = 0x00000200,
		Category11 = 0x00000400,
		Category12 = 0x00000800,
		Category13 = 0x00001000,
		Category14 = 0x00002000,
		Category15 = 0x00004000,
		Category16 = 0x00008000,
		Category17 = 0x00010000,
		Category18 = 0x00020000,
		Category19 = 0x00040000,
		Category20 = 0x00080000,
		Category21 = 0x00100000,
		Category22 = 0x00200000,
		Category23 = 0x00400000,
		Category24 = 0x00800000,
		Category25 = 0x01000000,
		Category26 = 0x02000000,
		Category27 = 0x04000000,
		Category28 = 0x08000000,
		Category29 = 0x10000000,
		Category30 = 0x20000000,
		Category31 = 0x40000000,
		All = int.MaxValue,
	}

	/// <summary>
	/// The data to perform a search query of 2D physical objects by ray.
	/// </summary>
	public class Physics2DRayTestItem
	{
		//input
		public Ray2 Ray { get; set; }
		public Physics2DCategories CollisionCategories { get; set; } = Physics2DCategories.Category1;
		public Physics2DCategories CollidesWith { get; set; } = Physics2DCategories.All;
		public int CollisionGroup { get; set; } = 0;
		public ModeEnum Mode { get; set; }
		//public CollisionObject SingleCastCollisionObject { get; set; }

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
		/// Represents a result item for <see cref="Physics2DRayTestItem"/>.
		/// </summary>
		public class ResultItem
		{
			public PhysicalBody2D Body { get; set; }
			public CollisionShape2D Shape { get; set; }
			public Vector2 Position { get; set; }
			public Vector2 Normal { get; set; }
			public double DistanceScale { get; set; }
			//public int TriangleIndexSource { get; set; }
			//public int TriangleIndexProcessed { get; set; }
		}

		////////////////

		public Physics2DRayTestItem()
		{
		}

		public Physics2DRayTestItem( Ray2 ray, Physics2DCategories collisionCategories = Physics2DCategories.Category1, Physics2DCategories collidesWith = Physics2DCategories.All, int collisionGroup = 0, ModeEnum mode = ModeEnum.All )
		{
			this.Ray = ray;
			this.CollisionCategories = collisionCategories;
			this.CollidesWith = collidesWith;
			this.CollisionGroup = collisionGroup;
			this.Mode = mode;
		}

		//public Physics2DRayCastItem( Ray ray, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, CollisionObject singleCastCollisionObject )
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
	/// The data to perform a search query of physical objects at a specified area.
	/// </summary>
	public class Physics2DContactTestItem
	{
		//input
		public Physics2DCategories CollisionCategories { get; set; } = Physics2DCategories.Category1;
		public Physics2DCategories CollidesWith { get; set; } = Physics2DCategories.All;
		public int CollisionGroup { get; set; } = 0;
		public ModeEnum Mode { get; set; }
		public Rectangle Bounds { get; set; }

		public Vector2[] Convex { get; set; }
		//public CollisionObject CollisionObject { get; set; }
		//public bool CollisionObjectAutoDispose { get; set; }
		//public Predicate<Vector3> CheckPositionWorldOnB;

		//public double ClosestDistanceThreshold { get; set; }

		//output
		volatile ResultItem[] result;
		public ResultItem[] Result { get { return result; } set { result = value; } }

		//addition
		public object UserData { get; set; }

		////////////////

		public enum ModeEnum
		{
			One,
			//OneClosest,
			//OneForEach,
			//OneClosestForEach,
			All
		}

		////////////////

		/// <summary>
		/// Represents a result item for <see cref="Physics2DContactTestItem"/>.
		/// </summary>
		public class ResultItem
		{
			public PhysicalBody2D Body { get; set; }
			public CollisionShape2D Shape { get; set; }

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

		public Physics2DContactTestItem()
		{
		}

		public Physics2DContactTestItem( Rectangle bounds, Physics2DCategories collisionCategories = Physics2DCategories.Category1, Physics2DCategories collidesWith = Physics2DCategories.All, int collisionGroup = 0, ModeEnum mode = ModeEnum.All )
		{
			CollisionCategories = collisionCategories;
			CollidesWith = collidesWith;
			CollisionGroup = collisionGroup;
			Mode = mode;
			Bounds = bounds;
		}

		public Physics2DContactTestItem( Vector2[] convex, Physics2DCategories collisionCategories = Physics2DCategories.Category1, Physics2DCategories collidesWith = Physics2DCategories.All, int collisionGroup = 0, ModeEnum mode = ModeEnum.All )
		{
			var bounds = new Rectangle( convex[ 0 ] );
			foreach( var p in convex )
				bounds.Add( p );

			CollisionCategories = collisionCategories;
			CollidesWith = collidesWith;
			CollisionGroup = collisionGroup;
			Mode = mode;
			Bounds = bounds;
			Convex = convex;
		}

		public Physics2DContactTestItem( Capsule2 capsule, int edges = 32, Physics2DCategories collisionCategories = Physics2DCategories.Category1, Physics2DCategories collidesWith = Physics2DCategories.All, int collisionGroup = 0, ModeEnum mode = ModeEnum.All )
		{
			CollisionCategories = collisionCategories;
			CollidesWith = collidesWith;
			CollisionGroup = collisionGroup;
			Mode = mode;
			Bounds = capsule.ToBounds();
			Convex = MathAlgorithms.GenerateCapsuleConvex( capsule, edges );
		}
	}
}
