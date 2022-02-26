// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Internal.tainicom.Aether.Physics2D;
using Internal.tainicom.Aether.Physics2D.Dynamics;
using Internal.tainicom.Aether.Physics2D.Collision;
using Internal.tainicom.Aether.Physics2D.Collision.Shapes;
using Internal.tainicom.Aether.Physics2D.Common;

namespace NeoAxis
{
	public partial class Scene
	{
		World physics2DWorld;

		/////////////////////////////////////////

		public World Physics2DGetWorld( bool canInit )
		{
			if( physics2DWorld == null && canInit )
			{
				physics2DWorld = new World();

				//!!!!
				//Settings.UseConvexHullPolygons = false;

				Settings.MaxPolygonVertices = 64;

				Physics2DUpdateGravity();
			}

			return physics2DWorld;
		}

		void Physics2DWorldDestroy()
		{
			physics2DWorld = null;
		}

		[DisplayName( "Physics 2D Simulation Step After" )]
		public event Action<Scene> Physics2DSimulationStepAfter;

		internal void Physics2DSimulate( bool editorSimulateSelectedMode, ESet<IPhysicalObject> updateOnlySelected )
		{
			if( physics2DWorld != null )
			{
				////update settings
				//if( physicsWorldData.world.SolverInfo.NumIterations != PhysicsNumberIterations )
				//	physicsWorldData.world.SolverInfo.NumIterations = PhysicsNumberIterations;

				//simulate

				float invStep = ProjectSettings.Get.General.SimulationStepsPerSecondInv;
				physics2DWorld.Step( invStep );

				//int steps = PhysicsSimulationSteps;
				//if( steps == 0 )
				//	steps = 1;
				//for( int n = 0; n < steps; n++ )
				//{
				//	int substepsPassed = physicsWorldData.world.StepSimulation( invStep / steps, 1, invStep / steps );
				//}
				////int substepsPassed = physicsWorldData.world.StepSimulation( invStep, 1, invStep );
				////int substepsPassed = physicsWorld.StepSimulation( (float)ProjectSettings.Get.SimulationStepsPerSecondInv, 1, 0.0166666675F );

				//get updated data
				{
					//!!!!slowly. update only updated

					foreach( var body in GetComponents<PhysicalBody2D>( false, true, true ) )
					{
						if( body != null && ( updateOnlySelected == null || updateOnlySelected.Contains( body ) ) )
							body.UpdateDataFromPhysicsEngine();
					}

					foreach( var constraint in GetComponents<Constraint2D>( false, true, true ) )
					{
						if( constraint != null && ( updateOnlySelected == null || updateOnlySelected.Contains( constraint ) ) )
							constraint.UpdateDataFromPhysicsEngine();
					}
				}

				Physics2DSimulationStepAfter?.Invoke( this );
			}
		}

		void Physics2DUpdateGravity()
		{
			if( physics2DWorld != null )
				physics2DWorld.Gravity = Physics2DUtility.Convert( Gravity2D );
		}

		void Physics2DViewportUpdateBegin( Viewport viewport )
		{
		}

		static bool ShouldCollide( Fixture fixture, Physics2DRayTestItem item )
		{
			if( fixture.CollisionGroup != 0 && fixture.CollisionGroup == item.CollisionGroup )
				return fixture.CollisionGroup > 0;

			bool collide =
				( ( fixture.CollidesWith & (Category)item.CollisionCategories ) != 0 ) &&
				( ( (Category)item.CollidesWith & fixture.CollisionCategories ) != 0 );

			return collide;
		}

		static bool ShouldCollide( Fixture fixture, Physics2DContactTestItem item )
		{
			if( fixture.CollisionGroup != 0 && fixture.CollisionGroup == item.CollisionGroup )
				return fixture.CollisionGroup > 0;

			bool collide =
				( ( fixture.CollidesWith & (Category)item.CollisionCategories ) != 0 ) &&
				( ( (Category)item.CollidesWith & fixture.CollisionCategories ) != 0 );

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

		public void Physics2DRayTest( Physics2DRayTestItem[] items )
		{
			if( physics2DWorld != null )
			{
				//!!!!threading

				foreach( var item in items )
				{
					var result = new List<Physics2DRayTestItem.ResultItem>();

					var point1 = Physics2DUtility.Convert( item.Ray.Origin );
					var point2 = Physics2DUtility.Convert( item.Ray.GetEndPoint() );

					physics2DWorld.RayCast( ( f, p, n, fr ) =>
					{
						if( ShouldCollide( f, item ) )
						{
							var shape = f.Tag as CollisionShape2D;
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

		public void Physics2DRayTest( Physics2DRayTestItem item )
		{
			Physics2DRayTest( new Physics2DRayTestItem[] { item } );
		}

		/////////////////////////////////////////

		public void Physics2DContactTest( Physics2DContactTestItem[] items )
		{
			if( physics2DWorld != null )
			{
				foreach( var item in items )
				{
					var aabb = new AABB( Physics2DUtility.Convert( item.Bounds.Minimum ), Physics2DUtility.Convert( item.Bounds.Maximum ) );

					var fixtures = new List<Fixture>();
					physics2DWorld.QueryAABB( delegate ( Fixture fixture )
					{
						fixtures.Add( fixture );
						return true;
					}, ref aabb );
					//var fixtures = worldData.world.QueryAABB( ref aabb );

					var result = new List<Physics2DContactTestItem.ResultItem>( fixtures.Count );

					bool Contains( PhysicalBody2D body, CollisionShape2D shape )
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

								var identity = Internal.tainicom.Aether.Physics2D.Common.Transform.Identity;
								if( !Collision.TestOverlap( polygonShape, 0, fixture.Shape, 0, ref identity, ref bodyTransform ) )
									skip = true;
							}

							if( !skip )
							{
								var shape = fixture.Tag as CollisionShape2D;
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

		public void Physics2DContactTest( Physics2DContactTestItem item )
		{
			Physics2DContactTest( new Physics2DContactTestItem[] { item } );
		}

		//public void Physics2DConvexSweepTest( Physics2DConvexSweepTestItem[] items )
		//public void Physics2DConvexSweepTest( Physics2DConvexSweepTestItem item )

	}
}
