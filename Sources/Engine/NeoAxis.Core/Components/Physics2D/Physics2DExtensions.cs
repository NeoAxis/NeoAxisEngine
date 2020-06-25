// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using tainicom.Aether.Physics2D;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;

namespace NeoAxis
{
	/// <summary>
	/// The data to perform a search query of 2D physical objects by ray.
	/// </summary>
	public class Physics2DRayTestItem
	{
		//input
		public Ray2 Ray { get; set; }
		public Category CollisionCategories { get; set; } = Category.Category1;
		public Category CollidesWith { get; set; } = Category.All;
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
			public Component_PhysicalBody2D Body { get; set; }
			public Component_CollisionShape2D Shape { get; set; }
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

		public Physics2DRayTestItem( Ray2 ray, Category collisionCategories = Category.Category1, Category collidesWith = Category.All, int collisionGroup = 0, ModeEnum mode = ModeEnum.All )
		{
			this.Ray = ray;
			this.CollisionCategories = collisionCategories;
			this.CollidesWith = collidesWith;
			this.CollisionGroup = collisionGroup;
			this.Mode = mode;
		}

		//public Physics2DRayTestItem( Ray ray, int collisionFilterGroup, int collisionFilterMask, ModeEnum mode, CollisionObject singleCastCollisionObject )
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
		public Category CollisionCategories { get; set; } = Category.Category1;
		public Category CollidesWith { get; set; } = Category.All;
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
			public Component_PhysicalBody2D Body { get; set; }
			public Component_CollisionShape2D Shape { get; set; }

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

		public Physics2DContactTestItem( Rectangle bounds, Category collisionCategories = Category.Category1, Category collidesWith = Category.All, int collisionGroup = 0, ModeEnum mode = ModeEnum.All )
		{
			CollisionCategories = collisionCategories;
			CollidesWith = collidesWith;
			CollisionGroup = collisionGroup;
			Mode = mode;
			Bounds = bounds;
		}

		public Physics2DContactTestItem( Vector2[] convex, Category collisionCategories = Category.Category1, Category collidesWith = Category.All, int collisionGroup = 0, ModeEnum mode = ModeEnum.All )
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

		public Physics2DContactTestItem( Capsule2 capsule, int edges = 32, Category collisionCategories = Category.Category1, Category collidesWith = Category.All, int collisionGroup = 0, ModeEnum mode = ModeEnum.All )
		{
			CollisionCategories = collisionCategories;
			CollidesWith = collidesWith;
			CollisionGroup = collisionGroup;
			Mode = mode;
			Bounds = capsule.ToBounds();
			Convex = MathAlgorithms.GenerateCapsuleConvex( capsule, edges );
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Auxiliary extension methods for working with the 2D physics engine.
	/// </summary>
	public static class Physics2DExtensions
	{
		static bool ShouldCollide( Fixture fixture, Physics2DRayTestItem item )
		{
			if( fixture.CollisionGroup != 0 && fixture.CollisionGroup == item.CollisionGroup )
				return fixture.CollisionGroup > 0;

			bool collide =
				( ( fixture.CollidesWith & item.CollisionCategories ) != 0 ) &&
				( ( item.CollidesWith & fixture.CollisionCategories ) != 0 );

			return collide;
		}

		static bool ShouldCollide( Fixture fixture, Physics2DContactTestItem item )
		{
			if( fixture.CollisionGroup != 0 && fixture.CollisionGroup == item.CollisionGroup )
				return fixture.CollisionGroup > 0;

			bool collide =
				( ( fixture.CollidesWith & item.CollisionCategories ) != 0 ) &&
				( ( item.CollidesWith & fixture.CollisionCategories ) != 0 );

			return collide;
		}

		static void RayTest_PostProcess( Physics2DRayTestItem item, List<Physics2DRayTestItem.ResultItem> resultList )
		{
			if( resultList.Count == 0 )
			{
				item.Result = Array.Empty<Physics2DRayTestItem.ResultItem>();
				return;
			}

			if( item.Mode == Physics2DRayTestItem.ModeEnum.OneClosest || item.Mode == Physics2DRayTestItem.ModeEnum.One )
			{
				item.Result = new Physics2DRayTestItem.ResultItem[] { resultList[ 0 ] };
				return;
			}

			if( item.Mode == Physics2DRayTestItem.ModeEnum.OneForEach )
			{
				item.Result = GetResultsOnePerShape( resultList );
				return;
			}

			//sort by distance in any other case
			CollectionUtility.SelectionSort( resultList, Compare );

			if( item.Mode == Physics2DRayTestItem.ModeEnum.All )
			{
				item.Result = resultList.ToArray();
				return;
			}

			//item.Mode == PhysicsRayTestItem.ModeEnum.OneClosestForEach
			item.Result = GetResultsOnePerShape( resultList );
		}

		static Physics2DRayTestItem.ResultItem[] GetResultsOnePerShape( List<Physics2DRayTestItem.ResultItem> list )
		{
			var resultCount = 0;
			var result = new Physics2DRayTestItem.ResultItem[ list.Count ];

			for( int i = 0; i < list.Count; i++ )
			{
				object shapeBody = list[ i ].Shape;
				if( shapeBody == null )
					shapeBody = list[ i ].Body;

				var added = false;

				for( int idx = 0; idx < resultCount; idx++ )
				{
					object addedShapeBody = result[ idx ].Shape;
					if( addedShapeBody == null )
						addedShapeBody = result[ idx ].Body;

					if( ReferenceEquals( addedShapeBody, shapeBody ) )
					{
						added = true;
						break;
					}
				}

				if( !added )
					result[ resultCount++ ] = list[ i ];
			}

			if( resultCount != result.Length )
				Array.Resize( ref result, resultCount );

			return result;
		}

		static int Compare( Physics2DRayTestItem.ResultItem x, Physics2DRayTestItem.ResultItem y )
		{
			if( x.DistanceScale < y.DistanceScale )
				return -1;
			if( x.DistanceScale > y.DistanceScale )
				return 1;
			return 0;
		}

		public static void Physics2DRayTest( this Component_Scene scene, Physics2DRayTestItem[] items )
		{
			var worldData = Physics2DUtility.GetWorldData( scene, false );
			if( worldData != null )
			{
				//!!!!threading

				foreach( var item in items )
				{
					var result = new List<Physics2DRayTestItem.ResultItem>();

					var point1 = Physics2DUtility.Convert( item.Ray.Origin );
					var point2 = Physics2DUtility.Convert( item.Ray.GetEndPoint() );

					worldData.world.RayCast( ( f, p, n, fr ) =>
					{
						if( ShouldCollide( f, item ) )
						{
							var shape = f.Tag as Component_CollisionShape2D;
							if( shape != null )
							{
								var resultItem = new Physics2DRayTestItem.ResultItem();
								resultItem.Body = shape.ParentRigidBody;
								resultItem.Shape = shape;
								resultItem.Position = Physics2DUtility.Convert( p );
								resultItem.Normal = Physics2DUtility.Convert( n );
								resultItem.DistanceScale = fr;

								result.Add( resultItem );

								if( item.Mode == Physics2DRayTestItem.ModeEnum.One )
									return 0;
							}
						}

						return 1;
					}, point1, point2 );

					RayTest_PostProcess( item, result );
				}
			}
			else
			{
				foreach( var item in items )
					item.Result = Array.Empty<Physics2DRayTestItem.ResultItem>();
			}
		}

		public static void Physics2DRayTest( this Component_Scene scene, Physics2DRayTestItem item )
		{
			Physics2DRayTest( scene, new Physics2DRayTestItem[] { item } );
		}

		/////////////////////////////////////////

		public static void Physics2DContactTest( this Component_Scene scene, Physics2DContactTestItem[] items )
		{
			var worldData = Physics2DUtility.GetWorldData( scene, false );
			if( worldData != null )
			{
				foreach( var item in items )
				{
					var aabb = new AABB( Physics2DUtility.Convert( item.Bounds.Minimum ), Physics2DUtility.Convert( item.Bounds.Maximum ) );
					var fixtures = worldData.world.QueryAABB( ref aabb );

					var result = new List<Physics2DContactTestItem.ResultItem>( fixtures.Count );

					bool Contains( Component_PhysicalBody2D body, Component_CollisionShape2D shape )
					{
						for( int n = 0; n < result.Count; n++ )
							if( result[ n ].Body == body && result[ n ].Shape == shape )
								return true;
						return false;
					}

					PolygonShape polygonShape = null;
					if( item.Convex != null )
					{
						var vertices = new Vertices( item.Convex.Length );
						foreach( var p in item.Convex )
							vertices.Add( Physics2DUtility.Convert( p ) );
						polygonShape = new PolygonShape( vertices, 0 );
					}

					foreach( var fixture in fixtures )
					{
						if( ShouldCollide( fixture, item ) )
						{
							bool skip = false;

							if( polygonShape != null )
							{
								fixture.Body.GetTransform( out var bodyTransform );

								var identity = tainicom.Aether.Physics2D.Common.Transform.Identity;
								if( !Collision.TestOverlap( polygonShape, 0, fixture.Shape, 0, ref identity, ref bodyTransform ) )
									skip = true;
							}

							if( !skip )
							{
								var shape = fixture.Tag as Component_CollisionShape2D;
								if( shape != null )
								{
									var body = shape.ParentRigidBody;
									if( !Contains( body, shape ) )
									{
										var resultItem = new Physics2DContactTestItem.ResultItem();
										resultItem.Body = body;
										resultItem.Shape = shape;
										result.Add( resultItem );

										if( item.Mode == Physics2DContactTestItem.ModeEnum.One )
											break;
									}
								}
							}
						}
					}

					item.Result = result.ToArray();
				}
			}
			else
			{
				foreach( var item in items )
					item.Result = Array.Empty<Physics2DContactTestItem.ResultItem>();
			}
		}

		public static void Physics2DContactTest( this Component_Scene scene, Physics2DContactTestItem item )
		{
			Physics2DContactTest( scene, new Physics2DContactTestItem[] { item } );
		}

		//public void Physics2DConvexSweepTest( Physics2DConvexSweepTestItem[] items )
		//public void Physics2DConvexSweepTest( Physics2DConvexSweepTestItem item )
	}
}
