// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Task-based artificial intelligence for vehicle.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Vehicle\Vehicle AI", 22004 )]
	public class VehicleAI : AI
	{
		static FastRandom staticRandom = new FastRandom( 0 );

		Weapon[] weaponsCache;
		ObjectInSpace currentTarget;
		float updateTargetRemainingTime;
		float updateTasksRemainingTime;

		///////////////////////////////////////////////

		/// <summary>
		/// Whether to enabled a combat mode. In the combat mode the AI manages weapons and can drive the vehicle.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> CombatMode
		{
			get { if( _combatMode.BeginGet() ) CombatMode = _combatMode.Get( this ); return _combatMode.value; }
			set { if( _combatMode.BeginSet( this, ref value ) ) { try { CombatModeChanged?.Invoke( this ); } finally { _combatMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CombatMode"/> property value changes.</summary>
		public event Action<VehicleAI> CombatModeChanged;
		ReferenceField<bool> _combatMode = false;

		/// <summary>
		/// Whether to allow control the vehicle in the combat mode.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> CombatModeCanMove
		{
			get { if( _combatModeCanMove.BeginGet() ) CombatModeCanMove = _combatModeCanMove.Get( this ); return _combatModeCanMove.value; }
			set { if( _combatModeCanMove.BeginSet( this, ref value ) ) { try { CombatModeCanMoveChanged?.Invoke( this ); } finally { _combatModeCanMove.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CombatModeCanMove"/> property value changes.</summary>
		public event Action<VehicleAI> CombatModeCanMoveChanged;
		ReferenceField<bool> _combatModeCanMove = true;

		[DefaultValue( 300 )]
		public Reference<double> CombatModeFindEnemyDistance
		{
			get { if( _combatModeFindEnemyDistance.BeginGet() ) CombatModeFindEnemyDistance = _combatModeFindEnemyDistance.Get( this ); return _combatModeFindEnemyDistance.value; }
			set { if( _combatModeFindEnemyDistance.BeginSet( this, ref value ) ) { try { CombatModeFindEnemyDistanceChanged?.Invoke( this ); } finally { _combatModeFindEnemyDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CombatModeFindEnemyDistance"/> property value changes.</summary>
		public event Action<VehicleAI> CombatModeFindEnemyDistanceChanged;
		ReferenceField<double> _combatModeFindEnemyDistance = 300;


		/// <summary>
		/// Whether to visualize a task info.
		/// </summary>
		[Category( "Debug" )]
		[DefaultValue( false )]
		public Reference<bool> DebugVisualization
		{
			get { if( _debugVisualization.BeginGet() ) DebugVisualization = _debugVisualization.Get( this ); return _debugVisualization.value; }
			set { if( _debugVisualization.BeginSet( this, ref value ) ) { try { DebugVisualizationChanged?.Invoke( this ); } finally { _debugVisualization.EndSet(); } } }
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
				switch( member.Name )
				{
				case nameof( CombatModeCanMove ):
				case nameof( CombatModeFindEnemyDistance ):
					if( !CombatMode )
						skip = true;
					break;
				}
			}
		}

		[Browsable( false )]
		public Vehicle Vehicle
		{
			get { return Parent as Vehicle; }
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void SimulationStepTasks( Vehicle vehicle )
		{
			//!!!!call less often

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
						//var distanceZ = Math.Abs( diff.Z );

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

								vehicle.Steering = (float)-d.Y;
								vehicle.Brake = 0;
								vehicle.HandBrake = 0;

								vehicle.SetMotorOn();
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

				var underControl = input != null && input.InputEnabled;
				//if( NetworkIsServer && NetworkSceneManagerUtility.IsObjectControlledByPlayer( vehicle ) )
				if( NetworkIsServer )//&& NetworkSceneManagerUtility.GetUserByObjectControlled( vehicle, true ) != null )
				{
					var networkLogic = NetworkLogicUtility.GetNetworkLogic( vehicle );
					if( networkLogic != null && networkLogic.ServerGetUserByObjectControlled( vehicle, true ) != null )
						underControl = true;
				}

				if( !underControl )//if( input == null || !input.InputEnabled )
				{
					vehicle.Throttle = 0;
					vehicle.Steering = 0;
					vehicle.Brake = 0;
					vehicle.HandBrake = 1;
					//vehicle.Brake = 1;
					//vehicle.HandBrake = 0;
				}
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//!!!!call less often

			var vehicle = Vehicle;
			if( vehicle != null )
			{
				SimulationStepTasks( vehicle );
				if( CombatMode )
					SimulationStepCombatMode( vehicle );
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
				if( EnabledInHierarchyAndIsInstance )
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
				else
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
			}

			updateTargetRemainingTime = staticRandom.Next( 1.0f );
			updateTasksRemainingTime = staticRandom.Next( 1.0f );
		}

		private void Scene_GetRenderSceneData( Scene scene, ViewportRenderingContext context )
		{
			if( DebugVisualization )
			{
				DebugRenderTasks( context );
				if( CombatMode )
					DebugRenderCombatMode( context );
			}
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
			//if( EngineApp.IsSimulation )
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


		//combat mode

		struct UpdateCurrentTargetObject
		{
			public ObjectInSpace Object;
			public Vector3 TargetCenter;
			public double DistanceSquared;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void UpdateCurrentTarget( Vehicle thisObject )
		{
			var findDistance = CombatModeFindEnemyDistance.Value;
			if( findDistance == 0 )
				return;

			var tr = thisObject.TransformV;

			var bounds = new Bounds( tr.Position );
			bounds.Expand( findDistance );

			var item = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, bounds );
			thisObject.ParentScene.GetObjectsInSpace( item );

			//!!!!GC
			var objects = new List<UpdateCurrentTargetObject>( 64 );

			for( int n = 0; n < item.Result.Length; n++ )
			{
				ref var itemResult = ref item.Result[ n ];

				var obj = itemResult.Object;

				//vehicles
				var vehicle = obj as Vehicle;
				if( vehicle != null && vehicle.Team != 0 && vehicle.Team != thisObject.Team )
				{
					if( ( obj.TransformV.Position - tr.Position ).LengthSquared() < findDistance * findDistance )
					{
						vehicle.GetBox( out var box );
						var targetCenter = box.ToBounds().GetCenter();

						var objectItem = new UpdateCurrentTargetObject();
						objectItem.Object = obj;
						objectItem.TargetCenter = targetCenter;
						objectItem.DistanceSquared = ( objectItem.TargetCenter - tr.Position ).LengthSquared();
						objects.Add( objectItem );
					}
				}

				//characters
				var character = obj as Character;
				if( character != null && character.Team != 0 && character.Team != thisObject.Team )
				{
					if( character.LifeStatus.Value == Character.LifeStatusEnum.Normal )
					{
						if( ( obj.TransformV.Position - tr.Position ).LengthSquared() < findDistance * findDistance )
						{
							var targetCenter = character.GetCenteredPosition();

							var objectItem = new UpdateCurrentTargetObject();
							objectItem.Object = obj;
							objectItem.TargetCenter = targetCenter;
							objectItem.DistanceSquared = ( objectItem.TargetCenter - tr.Position ).LengthSquared();
							objects.Add( objectItem );
						}
					}
				}

				//other components
				//..

			}

			CollectionUtility.MergeSort( objects, delegate ( UpdateCurrentTargetObject item1, UpdateCurrentTargetObject item2 )
			{
				if( item1.DistanceSquared < item2.DistanceSquared )
					return -1;
				if( item1.DistanceSquared > item2.DistanceSquared )
					return 1;
				return 0;
			} );

			if( objects.Count > 0 )
				currentTarget = objects[ 0 ].Object;
			else
				currentTarget = null;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | (MethodImplOptions)512 )]
		Range GetOptimalAttackDistance()
		{
			var result = Range.Zero;

			if( weaponsCache != null )
			{
				for( int n = 0; n < weaponsCache.Length; n++ )
				{
					var weapon = weaponsCache[ n ];
					var weaponType = weapon.WeaponType.Value;

					if( weaponType.Mode1Enabled )
					{
						var range = weaponType.Mode1FiringDistance.Value;
						result = new Range( Math.Min( result.Minimum, range.Minimum ), Math.Max( result.Maximum, range.Maximum ) );
					}

					if( weaponType.Mode2Enabled )
					{
						var range = weaponType.Mode2FiringDistance.Value;
						result = new Range( Math.Min( result.Minimum, range.Minimum ), Math.Max( result.Maximum, range.Maximum ) );
					}
				}
			}

			return result;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void SimulationStepCombatMode( Vehicle thisObject )
		{
			if( weaponsCache == null )
				weaponsCache = thisObject.GetComponents<Weapon>( checkChildren: true, onlyEnabledInHierarchy: true );

			if( weaponsCache.Length != 0 )
			{
				//update current target
				{
					updateTargetRemainingTime -= Time.SimulationDelta;
					if( updateTargetRemainingTime <= 0 )
					{
						UpdateCurrentTarget( thisObject );
						updateTargetRemainingTime += 1.0f + staticRandom.Next( 0.1f );
					}

					//reset when target not exists
					if( currentTarget != null && currentTarget.Parent == null )
						currentTarget = null;
				}

				//update moving tasks
				{
					updateTasksRemainingTime -= Time.SimulationDelta;
					if( updateTasksRemainingTime <= 0 )
					{
						if( currentTarget != null )
						{
							var range = GetOptimalAttackDistance();
							var distanceSquared = ( currentTarget.TransformV.Position - thisObject.TransformV.Position ).LengthSquared();

							//check by distance
							if( distanceSquared > range.Maximum * range.Maximum )
							{
								var alreadyMoving = false;

								var currentTasks = GetComponents<AITask>();
								if( currentTasks.Length == 1 )
								{
									var moveToObject = currentTasks[ 0 ] as VehicleAITask_MoveToObject;
									if( moveToObject != null && moveToObject.Target.Value == currentTarget )
										alreadyMoving = true;
								}

								if( !alreadyMoving )
									MoveTo( currentTarget, true, true );
							}
							else
								ClearTaskQueue();
						}
						else
							ClearTaskQueue();

						updateTasksRemainingTime += 1.0f + staticRandom.Next( 0.1f );
					}
				}

				//update weapons
				{
					//!!!!call less often?

					if( currentTarget != null )
						Vehicle.LookToPosition( currentTarget.TransformV.Position );
					else
						Vehicle.LookToPosition( null );

					for( int n = 0; n < weaponsCache.Length; n++ )
					{
						var weapon = weaponsCache[ n ];
						var weaponType = weapon.WeaponType.Value;

						//fire
						if( currentTarget != null )
						{
							var distance = ( currentTarget.TransformV.Position - thisObject.TransformV.Position ).Length();

							if( weaponType.Mode1Enabled )
							{
								var range = weaponType.Mode1FiringDistance.Value;
								if( distance > range.Minimum && distance < range.Maximum )
									weapon.FiringBegin( 1, 0 );
							}

							if( weaponType.Mode2Enabled )
							{
								var range = weaponType.Mode2FiringDistance.Value;
								if( distance > range.Minimum && distance < range.Maximum )
									weapon.FiringBegin( 2, 0 );
							}
						}
					}
				}
			}
		}

		void DebugRenderCombatMode( ViewportRenderingContext context )
		{
			//!!!!
		}
	}
}