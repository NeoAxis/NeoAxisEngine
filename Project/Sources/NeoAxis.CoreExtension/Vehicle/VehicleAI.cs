// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Task-based artificial intelligence for vehicle.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Vehicle AI", -7996 )]
	public class VehicleAI : AI
	{
		/// <summary>
		/// Whether to visualize a task info.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> DebugVisualization
		{
			get { if( _debugVisualization.BeginGet() ) DebugVisualization = _debugVisualization.Get( this ); return _debugVisualization.value; }
			set { if( _debugVisualization.BeginSet( ref value ) ) { try { DebugVisualizationChanged?.Invoke( this ); } finally { _debugVisualization.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugVisualization"/> property value changes.</summary>
		public event Action<VehicleAI> DebugVisualizationChanged;
		ReferenceField<bool> _debugVisualization = false;

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				//switch( member.Name )
				//{
				//case nameof( PathfindingSpecific ):
				//	if( !Pathfinding )
				//		skip = true;
				//	break;
				//}
			}
		}

		[Browsable( false )]
		public Vehicle Vehicle
		{
			get { return Parent as Vehicle; }
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//!!!!call less often

			var vehicle = Vehicle;
			if( vehicle != null )
			{
				var moving = false;
				var skipUpdate = false;

				var task = CurrentTask;
				if( task != null )
				{
					//MoveToPosition, MoveToObject
					var moveTo = task as VehicleAITask_MoveTo;
					if( moveTo != null )
					{
						var moveToObject = moveTo as VehicleAITask_MoveToObject;
						if( moveToObject != null && ( moveToObject.Target.Value == null || !moveToObject.Target.Value.EnabledInHierarchy ) )
						{
							//no target
							PathfindingClearData();
							if( task.DeleteTaskWhenReach )
								task.Dispose();
						}
						else
						{
							Vector3 target = Vector3.Zero;
							if( moveToObject != null )
								target = moveToObject.Target.Value.TransformV.Position;
							else if( moveTo is VehicleAITask_MoveToPosition moveToPosition )
								target = moveToPosition.Target;

							var diff = target - vehicle.TransformV.Position;
							var distanceXY = diff.ToVector2().Length();
							var distanceZ = Math.Abs( diff.Z );

							if( distanceXY <= moveTo.DistanceToReach )//!!!! && distanceZ < vehicle.Height )
							{
								//reach
								PathfindingClearData();
								if( task.DeleteTaskWhenReach )
								{
									task.Dispose();

									skipUpdate = true;
								}
							}
							else
							{
								//drive vehicle

								if( diff.X != 0 || diff.Y != 0 )
								{
									var taskDir = diff.ToVector2().GetNormalize();
									var vehicleDir = vehicle.TransformV.Rotation.GetForward().ToVector2();

									var taskAngle = Math.Atan2( taskDir.Y, taskDir.X );
									var vehicleAngle = Math.Atan2( vehicleDir.Y, vehicleDir.X );

									var angle = taskAngle - vehicleAngle;
									var d = new Vector2( Math.Cos( angle ), Math.Sin( angle ) );

									moving = true;

									//!!!!average throttle
									if( vehicle.IsOnGround() && vehicle.GroundRelativeVelocitySmooth.ToVector2().Length() < moveToObject.Speed )
										vehicle.Throttle = 1;
									else
										vehicle.Throttle = 0;
									//vehicle.Throttle = 1;

									vehicle.Steering = -d.Y;
									vehicle.Brake = 0;
								}

								//!!!!
								//if( Pathfinding )
								//{
								//	PathfindingUpdateAndGetMoveVector( Time.SimulationDelta, moveTo.DistanceToReach, vehicle.TransformV.Position, target, out var vector );
								//	if( vector != Vector3.Zero )
								//		diff = vector;
								//}

								//if( diff.X != 0 || diff.Y != 0 )
								//{
								//	vehicle.SetLookToDirection( diff );
								//	vehicle.SetMoveVector( diff.ToVector2().GetNormalize(), moveTo.Run );
								//}
							}
						}
					}
				}

				if( !moving && !skipUpdate )
				{
					var input = vehicle.GetComponent<VehicleInputProcessing>();
					if( input == null || !input.InputEnabled )
					{
						vehicle.Throttle = 0;
						vehicle.Steering = 0;
						vehicle.Brake = 1;
					}
				}
			}
		}

		public void Stop()
		{
			PathfindingClearData();
			ClearTaskQueue();
		}

		public VehicleAITask_MoveToPosition MoveTo( Vector3 target, bool run, bool clearTaskQueue = true )
		{
			if( clearTaskQueue )
			{
				PathfindingClearData();
				ClearTaskQueue();
			}

			var task = CreateComponent<VehicleAITask_MoveToPosition>( enabled: false );
			task.Target = target;
			//task.Run = run;
			task.Enabled = true;

			return task;
		}

		public VehicleAITask_MoveToObject MoveTo( ObjectInSpace target, bool run, bool clearTaskQueue = true )
		{
			if( clearTaskQueue )
			{
				PathfindingClearData();
				ClearTaskQueue();
			}

			var task = CreateComponent<VehicleAITask_MoveToObject>( enabled: false );
			task.Target = target;
			//task.Run = run;
			task.Enabled = true;

			return task;
		}

		public void PathfindingClearData()
		{
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = FindParent<Scene>();
			if( scene != null )
			{
				if( EnabledInHierarchy )
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
				else
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
			}
		}

		private void Scene_GetRenderSceneData( Scene scene, ViewportRenderingContext context )
		{
			if( DebugVisualization )
				DebugRenderTasks( context );
		}

		List<Vector3> GetTasksPoints()
		{
			var result = new List<Vector3>();

			foreach( var task in GetComponents<AITask>( onlyEnabledInHierarchy: true ) )
			{
				var moveToPosition = task as VehicleAITask_MoveToPosition;
				if( moveToPosition != null )
					result.Add( moveToPosition.Target.Value );

				var moveToObject = task as VehicleAITask_MoveToObject;
				if( moveToObject != null )
				{
					var target = moveToObject.Target.Value;
					if( target != null )
						result.Add( target.TransformV.Position );
				}
			}

			return result;
		}

		void DebugRenderTasks( ViewportRenderingContext context )
		{
			var renderer = context.Owner.Simple3DRenderer;

			var vehicle = Vehicle;
			if( vehicle == null )
				return;

			{
				var previousPoint = vehicle.TransformV.Position;

				renderer.SetColor( new ColorValue( 1, 0, 0, 0.5 ) );

				foreach( var p in GetTasksPoints() )
				{
					renderer.AddArrow( previousPoint, p );
					previousPoint = p;
				}
			}

			//var vehicle = Vehicle;
			//var currentTask = CurrentTask;
			//if( vehicle != null && currentTask != null )
			//{
			//	var moveToPosition = currentTask as VehicleAITask_MoveToPosition;
			//	if( moveToPosition != null )
			//	{
			//		renderer.SetColor( new ColorValue( 1, 1, 0, 0.5 ) );
			//		renderer.AddArrow( vehicle.TransformV.Position, moveToPosition.Target.Value );
			//	}

			//	var moveToObject = currentTask as VehicleAITask_MoveToObject;
			//	if( moveToObject != null )
			//	{
			//		var target = moveToObject.Target.Value;
			//		if( target != null )
			//		{
			//			renderer.SetColor( new ColorValue( 1, 1, 0, 0.5 ) );
			//			renderer.AddArrow( vehicle.TransformV.Position, target.TransformV.Position );
			//		}
			//	}

			//}

			//!!!!
			//if( EngineApp.ApplicationType == EngineApp.ApplicationTypeEnum.Simulation )
			//{
			//	var system = RoadSystem.GetOrCreateRoadSystemForScene( vehicle.ParentScene );

			//	var tasksPoints = GetTasksPoints();
			//	if( tasksPoints.Count != 0 )
			//	{
			//		var taskPoint = tasksPoints[ 0 ];

			//		var info = new RoadSystem.PathfindingInfo();
			//		info.From = vehicle.TransformV.Position;
			//		info.To = taskPoint;

			//		var result = system.StartPathfinding( info );

			//		if( result != null )
			//		{
			//			var points = result.Points;
			//			if( points.Length != 0 )
			//			{
			//				renderer.SetColor( new ColorValue( 0, 1, 0 ) );
			//				renderer.AddArrow( vehicle.TransformV.Position, points[ 0 ] );
			//				for( int n = 1; n < points.Length; n++ )
			//					renderer.AddArrow( points[ n - 1 ], points[ n ] );
			//			}
			//		}



			//		//var nearestRoadFrom = system.FindNearestRoad( vehicle.TransformV.Position );
			//		//var nearestRoadTo = system.FindNearestRoad( taskPoint );

			//		//if( nearestRoadFrom != null && nearestRoadTo != null )
			//		//{
			//		//	var pathOnRoads = system.FindPath( nearestRoadFrom, nearestRoadTo );


			//		//	renderer.SetColor( new ColorValue( 0, 1, 0 ) );
			//		//	renderer.AddArrow( vehicle.TransformV.Position, nearestRoadFrom.Position );

			//		//	renderer.SetColor( new ColorValue( 0, 1, 0 ) );
			//		//	renderer.AddArrow( nearestRoadTo.Position, vehicle.TransformV.Position );

			//		//	if( pathOnRoads != null )
			//		//	{
			//		//		for( int n = 1; n < pathOnRoads.Length; n++ )
			//		//			renderer.AddArrow( pathOnRoads[ n - 1 ], pathOnRoads[ n ] );
			//		//	}
			//		//}

			//	}
			//}

		}
	}
}