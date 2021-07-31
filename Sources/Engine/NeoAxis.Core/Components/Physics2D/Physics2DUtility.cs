// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using tainicom.Aether.Physics2D;
using tainicom.Aether.Physics2D.Dynamics;

namespace NeoAxis
{
	/// <summary>
	/// Auxiliary methods for working with the 2D physics engine.
	/// </summary>
	public static class Physics2DUtility
	{
		/////////////////////////////////////////

		public class Physics2DWorldDataImpl : Component_Scene.IPhysics2DWorldData
		{
			public Component_Scene scene;
			public World world;

			//

			public Physics2DWorldDataImpl( Component_Scene scene )
			{
				this.scene = scene;

				world = new World();

				//!!!!
				//Settings.UseConvexHullPolygons = false;

				Settings.MaxPolygonVertices = 64;

				UpdateGravity();
			}

			public void Destroy()
			{
				world = null;
			}

			public void UpdateGravity()
			{
				world.Gravity = Convert( scene.Gravity2D );
			}

			public void Simulate()
			{
				////update settings
				//if( physicsWorldData.world.SolverInfo.NumIterations != PhysicsNumberIterations )
				//	physicsWorldData.world.SolverInfo.NumIterations = PhysicsNumberIterations;

				//simulate

				float invStep = ProjectSettings.Get.SimulationStepsPerSecondInv;
				world.Step( invStep );

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

					foreach( var body in scene.GetComponents<Component_PhysicalBody2D>( false, true, true ) )
						body.UpdateDataFromPhysicsEngine();

					foreach( var constraint in scene.GetComponents<Component_Constraint2D>( false, true, true ) )
						constraint.UpdateDataFromPhysicsEngine();
				}
			}

			public void ViewportUpdateBegin( Viewport viewport )
			{
			}
		}

		/////////////////////////////////////////

		static Component_Scene.IPhysics2DWorldData GetPhysics2DWorldDataInit( Component_Scene scene )
		{
			return new Physics2DWorldDataImpl( scene );
		}

		public static Physics2DWorldDataImpl GetWorldData( Component_Scene scene, bool canInit )
		{
			return (Physics2DWorldDataImpl)scene.Physics2DGetWorldData( canInit, GetPhysics2DWorldDataInit );
		}

		public static Vector2 Convert( Microsoft.Xna.Framework.Vector2 v )
		{
			return new Vector2( v.X, v.Y );
		}

		public static Microsoft.Xna.Framework.Vector2 Convert( Vector2 v )
		{
			return new Microsoft.Xna.Framework.Vector2( (float)v.X, (float)v.Y );
		}
	}
}
