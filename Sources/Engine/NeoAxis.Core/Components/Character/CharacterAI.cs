// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NeoAxis
{
	/// <summary>
	/// Task-based artificial intelligence for character.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character AI", -8997 )]
	public class CharacterAI : AI
	{
		static FastRandom staticRandom = new FastRandom( 0 );

		PathController pathController;
		float lookAtControlledCharacterByPlayerRemainingTimeToUpdate;
		Character lookAtControlledCharacterByPlayerLookAtObject;

		//combat mode
		Weapon[] weaponsCache;
		ObjectInSpace combatModeCurrentTarget;
		float combatModeUpdateTargetRemainingTime;
		float combatModeUpdateTasksRemainingTime;

		//looking for food in the ground mode
		enum LookingForFoodTask
		{
			Idle,
			Move,
		}
		LookingForFoodTask lookingForFoodCurrentTask = LookingForFoodTask.Idle;
		float lookingForFoodUpdateTasksRemainingTime;

		//traffic walking mode
		//Crossroad.CrossroadLogicalData trafficWalkingModeLastPassedCrossroad;
		float trafficWalkingModeUpdateTasksRemainingTime;

		/////////////////////////////////////////

		/// <summary>
		/// Whether to use pathfinding functionality.
		/// </summary>
		[DefaultValue( true )]
		public Reference<bool> Pathfinding
		{
			get { if( _pathfinding.BeginGet() ) Pathfinding = _pathfinding.Get( this ); return _pathfinding.value; }
			set { if( _pathfinding.BeginSet( this, ref value ) ) { try { PathfindingChanged?.Invoke( this ); } finally { _pathfinding.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Pathfinding"/> property value changes.</summary>
		public event Action<CharacterAI> PathfindingChanged;
		ReferenceField<bool> _pathfinding = true;

		/// <summary>
		/// Use only the specified pathfinding object. This is used for scenes in which there are several pathfinding objects.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Pathfinding> PathfindingSpecific
		{
			get { if( _pathfindingSpecific.BeginGet() ) PathfindingSpecific = _pathfindingSpecific.Get( this ); return _pathfindingSpecific.value; }
			set { if( _pathfindingSpecific.BeginSet( this, ref value ) ) { try { PathfindingSpecificChanged?.Invoke( this ); } finally { _pathfindingSpecific.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PathfindingSpecific"/> property value changes.</summary>
		public event Action<CharacterAI> PathfindingSpecificChanged;
		ReferenceField<Pathfinding> _pathfindingSpecific = null;

		[DefaultValue( true )]
		public Reference<bool> AllowRun
		{
			get { if( _allowRun.BeginGet() ) AllowRun = _allowRun.Get( this ); return _allowRun.value; }
			set { if( _allowRun.BeginSet( this, ref value ) ) { try { AllowRunChanged?.Invoke( this ); } finally { _allowRun.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AllowRun"/> property value changes.</summary>
		public event Action<CharacterAI> AllowRunChanged;
		ReferenceField<bool> _allowRun = true;

		/// <summary>
		/// Whether to enable the ability to interact with the character.
		/// </summary>
		[DefaultValue( false )]
		public Reference<bool> AllowInteract
		{
			get { if( _allowInteract.BeginGet() ) AllowInteract = _allowInteract.Get( this ); return _allowInteract.value; }
			set { if( _allowInteract.BeginSet( this, ref value ) ) { try { AllowInteractChanged?.Invoke( this ); } finally { _allowInteract.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="AllowInteract"/> property value changes.</summary>
		public event Action<CharacterAI> AllowInteractChanged;
		ReferenceField<bool> _allowInteract = false;

		/// <summary>
		/// The entry point of the dialogue based on the flow graph.
		/// </summary>
		[DefaultValue( null )]
		public Reference<Component> DialogueFlow
		{
			get { if( _dialogueFlow.BeginGet() ) DialogueFlow = _dialogueFlow.Get( this ); return _dialogueFlow.value; }
			set { if( _dialogueFlow.BeginSet( this, ref value ) ) { try { DialogueFlowChanged?.Invoke( this ); } finally { _dialogueFlow.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DialogueFlow"/> property value changes.</summary>
		public event Action<CharacterAI> DialogueFlowChanged;
		ReferenceField<Component> _dialogueFlow = null;

		/// <summary>
		/// The distance at which need to look at the character controlled by the player.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<double> LookAtControlledCharacterByPlayerDistance
		{
			get { if( _lookAtControlledCharacterByPlayerDistance.BeginGet() ) LookAtControlledCharacterByPlayerDistance = _lookAtControlledCharacterByPlayerDistance.Get( this ); return _lookAtControlledCharacterByPlayerDistance.value; }
			set { if( _lookAtControlledCharacterByPlayerDistance.BeginSet( this, ref value ) ) { try { LookAtControlledCharacterByPlayerDistanceChanged?.Invoke( this ); } finally { _lookAtControlledCharacterByPlayerDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LookAtControlledCharacterByPlayerDistance"/> property value changes.</summary>
		public event Action<CharacterAI> LookAtControlledCharacterByPlayerDistanceChanged;
		ReferenceField<double> _lookAtControlledCharacterByPlayerDistance = 0.0;

		/// <summary>
		/// The speed of turning. Set zero to use default value specified in the character type.
		/// </summary>
		[DefaultValue( 0.0 )]
		public Reference<Degree> TurningSpeedOverride
		{
			get { if( _turningSpeedOverride.BeginGet() ) TurningSpeedOverride = _turningSpeedOverride.Get( this ); return _turningSpeedOverride.value; }
			set { if( _turningSpeedOverride.BeginSet( this, ref value ) ) { try { TurningSpeedOverrideChanged?.Invoke( this ); } finally { _turningSpeedOverride.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TurningSpeedOverride"/> property value changes.</summary>
		public event Action<CharacterAI> TurningSpeedOverrideChanged;
		ReferenceField<Degree> _turningSpeedOverride = new Degree( 0.0 );

		///////////////////////////////////////////////

		/// <summary>
		/// Whether to enabled a combat mode. In the combat mode the AI manages weapons and can drive the vehicle.
		/// </summary>
		[Category( "Combat Mode" )]
		[DefaultValue( false )]
		public Reference<bool> CombatMode
		{
			get { if( _combatMode.BeginGet() ) CombatMode = _combatMode.Get( this ); return _combatMode.value; }
			set { if( _combatMode.BeginSet( this, ref value ) ) { try { CombatModeChanged?.Invoke( this ); } finally { _combatMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CombatMode"/> property value changes.</summary>
		public event Action<CharacterAI> CombatModeChanged;
		ReferenceField<bool> _combatMode = false;

		/// <summary>
		/// Whether to allow control the vehicle in the combat mode.
		/// </summary>
		[Category( "Combat Mode" )]
		[DefaultValue( true )]
		public Reference<bool> CombatModeCanMove
		{
			get { if( _combatModeCanMove.BeginGet() ) CombatModeCanMove = _combatModeCanMove.Get( this ); return _combatModeCanMove.value; }
			set { if( _combatModeCanMove.BeginSet( this, ref value ) ) { try { CombatModeCanMoveChanged?.Invoke( this ); } finally { _combatModeCanMove.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CombatModeCanMove"/> property value changes.</summary>
		public event Action<CharacterAI> CombatModeCanMoveChanged;
		ReferenceField<bool> _combatModeCanMove = true;

		[Category( "Combat Mode" )]
		[DefaultValue( 300 )]
		public Reference<double> CombatModeFindEnemyDistance
		{
			get { if( _combatModeFindEnemyDistance.BeginGet() ) CombatModeFindEnemyDistance = _combatModeFindEnemyDistance.Get( this ); return _combatModeFindEnemyDistance.value; }
			set { if( _combatModeFindEnemyDistance.BeginSet( this, ref value ) ) { try { CombatModeFindEnemyDistanceChanged?.Invoke( this ); } finally { _combatModeFindEnemyDistance.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="CombatModeFindEnemyDistance"/> property value changes.</summary>
		public event Action<CharacterAI> CombatModeFindEnemyDistanceChanged;
		ReferenceField<double> _combatModeFindEnemyDistance = 300;

		///////////////////////////////////////////////

		[Category( "Looking For Food Mode" )]
		[DefaultValue( false )]
		public Reference<bool> LookingForFoodMode
		{
			get { if( _lookingForFoodMode.BeginGet() ) LookingForFoodMode = _lookingForFoodMode.Get( this ); return _lookingForFoodMode.value; }
			set { if( _lookingForFoodMode.BeginSet( this, ref value ) ) { try { LookingForFoodModeChanged?.Invoke( this ); } finally { _lookingForFoodMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LookingForFoodMode"/> property value changes.</summary>
		public event Action<CharacterAI> LookingForFoodModeChanged;
		ReferenceField<bool> _lookingForFoodMode = false;

		[Category( "Looking For Food Mode" )]
		[DefaultValue( null )]
		public Reference<Area> LookingForFoodModeArea
		{
			get { if( _lookingForFoodModeArea.BeginGet() ) LookingForFoodModeArea = _lookingForFoodModeArea.Get( this ); return _lookingForFoodModeArea.value; }
			set { if( _lookingForFoodModeArea.BeginSet( this, ref value ) ) { try { LookingForFoodModeAreaChanged?.Invoke( this ); } finally { _lookingForFoodModeArea.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LookingForFoodModeArea"/> property value changes.</summary>
		public event Action<CharacterAI> LookingForFoodModeAreaChanged;
		ReferenceField<Area> _lookingForFoodModeArea = null;

		[Category( "Looking For Food Mode" )]
		[DefaultValue( 5.0 )]
		public Reference<double> LookingForFoodModeIdleTime
		{
			get { if( _lookingForFoodModeIdleTime.BeginGet() ) LookingForFoodModeIdleTime = _lookingForFoodModeIdleTime.Get( this ); return _lookingForFoodModeIdleTime.value; }
			set { if( _lookingForFoodModeIdleTime.BeginSet( this, ref value ) ) { try { LookingForFoodModeIdleTimeChanged?.Invoke( this ); } finally { _lookingForFoodModeIdleTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LookingForFoodModeIdleTime"/> property value changes.</summary>
		public event Action<CharacterAI> LookingForFoodModeIdleTimeChanged;
		ReferenceField<double> _lookingForFoodModeIdleTime = 10.0;

		[Category( "Looking For Food Mode" )]
		[DefaultValue( 15.0 )]
		public Reference<double> LookingForFoodInGroundModeMoveMaxTime
		{
			get { if( _lookingForFoodInGroundModeMoveMaxTime.BeginGet() ) LookingForFoodInGroundModeMoveMaxTime = _lookingForFoodInGroundModeMoveMaxTime.Get( this ); return _lookingForFoodInGroundModeMoveMaxTime.value; }
			set { if( _lookingForFoodInGroundModeMoveMaxTime.BeginSet( this, ref value ) ) { try { LookingForFoodInGroundModeMoveMaxTimeChanged?.Invoke( this ); } finally { _lookingForFoodInGroundModeMoveMaxTime.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="LookingForFoodInGroundModeMoveMaxTime"/> property value changes.</summary>
		public event Action<CharacterAI> LookingForFoodInGroundModeMoveMaxTimeChanged;
		ReferenceField<double> _lookingForFoodInGroundModeMoveMaxTime = 15.0;

		///////////////////////////////////////////////

		/// <summary>
		/// Whether to enabled a walking mode. In the walking mode the AI manages walking of character by using the traffic system.
		/// </summary>
		[Category( "Traffic Walking Mode" )]
		[DefaultValue( false )]
		public Reference<bool> TrafficWalkingMode
		{
			get { if( _trafficWalkingMode.BeginGet() ) TrafficWalkingMode = _trafficWalkingMode.Get( this ); return _trafficWalkingMode.value; }
			set { if( _trafficWalkingMode.BeginSet( this, ref value ) ) { try { TrafficWalkingModeChanged?.Invoke( this ); } finally { _trafficWalkingMode.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrafficWalkingMode"/> property value changes.</summary>
		public event Action<CharacterAI> TrafficWalkingModeChanged;
		ReferenceField<bool> _trafficWalkingMode = false;

		[Category( "Traffic Walking Mode" )]
		[DefaultValue( null )]
		public Reference<Road> TrafficWalkingModeCurrentRoad
		{
			get { if( _trafficWalkingModeCurrentRoad.BeginGet() ) TrafficWalkingModeCurrentRoad = _trafficWalkingModeCurrentRoad.Get( this ); return _trafficWalkingModeCurrentRoad.value; }
			set { if( _trafficWalkingModeCurrentRoad.BeginSet( this, ref value ) ) { try { TrafficWalkingModeCurrentRoadChanged?.Invoke( this ); } finally { _trafficWalkingModeCurrentRoad.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrafficWalkingModeCurrentRoad"/> property value changes.</summary>
		public event Action<CharacterAI> TrafficWalkingModeCurrentRoadChanged;
		ReferenceField<Road> _trafficWalkingModeCurrentRoad = null;

		[Category( "Traffic Walking Mode" )]
		[DefaultValue( 0 )]
		public Reference<int> TrafficWalkingModeCurrentRoadLane
		{
			get { if( _trafficWalkingModeCurrentRoadLane.BeginGet() ) TrafficWalkingModeCurrentRoadLane = _trafficWalkingModeCurrentRoadLane.Get( this ); return _trafficWalkingModeCurrentRoadLane.value; }
			set { if( _trafficWalkingModeCurrentRoadLane.BeginSet( this, ref value ) ) { try { TrafficWalkingModeCurrentRoadLaneChanged?.Invoke( this ); } finally { _trafficWalkingModeCurrentRoadLane.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="TrafficWalkingModeCurrentRoadLane"/> property value changes.</summary>
		public event Action<CharacterAI> TrafficWalkingModeCurrentRoadLaneChanged;
		ReferenceField<int> _trafficWalkingModeCurrentRoadLane = 0;

		//!!!!impl? значит и создавать можно
		//[Category( "Traffic Walking Mode" )]
		//[DefaultValue( false )]
		//public Reference<bool> TrafficWalkingModeDeleteObjectAtEndOfRoad
		//{
		//	get { if( _trafficWalkingModeDeleteObjectAtEndOfRoad.BeginGet() ) TrafficWalkingModeDeleteObjectAtEndOfRoad = _trafficWalkingModeDeleteObjectAtEndOfRoad.Get( this ); return _trafficWalkingModeDeleteObjectAtEndOfRoad.value; }
		//	set { if( _trafficWalkingModeDeleteObjectAtEndOfRoad.BeginSet( this, ref value ) ) { try { TrafficWalkingModeDeleteObjectAtEndOfRoadChanged?.Invoke( this ); } finally { _trafficWalkingModeDeleteObjectAtEndOfRoad.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="TrafficWalkingModeDeleteObjectAtEndOfRoad"/> property value changes.</summary>
		//public event Action<CharacterAI> TrafficWalkingModeDeleteObjectAtEndOfRoadChanged;
		//ReferenceField<bool> _trafficWalkingModeDeleteObjectAtEndOfRoad = false;

		///////////////////////////////////////////////

		/// <summary>
		/// Whether to show a calculated path.
		/// </summary>
		[Category( "Debug" )]
		[DefaultValue( false )]
		public Reference<bool> DebugVisualization
		{
			get { if( _debugVisualization.BeginGet() ) DebugVisualization = _debugVisualization.Get( this ); return _debugVisualization.value; }
			set { if( _debugVisualization.BeginSet( this, ref value ) ) { try { DebugVisualizationChanged?.Invoke( this ); } finally { _debugVisualization.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DebugVisualization"/> property value changes.</summary>
		public event Action<CharacterAI> DebugVisualizationChanged;
		ReferenceField<bool> _debugVisualization = false;


		/////////////////////////////////////////

		class PathController
		{
			//!!!!
			readonly float reachDestinationPointDistance = 0.5f;
			readonly float reachDestinationPointZDifference = 1.5f;
			readonly float maxAllowableDeviationFromPath = 0.5f;
			readonly float updatePathWhenTargetPositionHasChangedMoreThanDistance = 1;
			//readonly float stepSize = 1;
			//readonly Vec3 polygonPickExtents = new Vec3( 2, 2, 2 );
			//readonly int maxPolygonPath = 512;
			//readonly int maxSmoothPath = 4096;
			//readonly int maxSteerPoints = 16;

			Pathfinding.FindPathContext findPathContext;

			Vector3? foundPathForTargetPosition;
			Vector3[] path;
			int currentIndex;
			float updateRemainingTime;

			//

			public void Clear()
			{
				foundPathForTargetPosition = null;
				path = null;
				currentIndex = 0;
			}

			public void Update( Pathfinding pathfinding, float delta, double distanceToReach, Vector3 from, Vector3 target )
			{
				//wait before last path find
				if( updateRemainingTime > 0 )
				{
					updateRemainingTime -= delta;
					if( updateRemainingTime < 0 )
						updateRemainingTime = 0;
				}

				////already on target position?
				//if( ( from.ToVector2() - target.ToVector2() ).LengthSquared() <
				//	reachDestinationPointDistance * reachDestinationPointDistance &&
				//	Math.Abs( from.Z - target.Z ) < reachDestinationPointZDifference )
				//{
				//	Clear();
				//	return;
				//}

				//clear path when target position was updated
				if( path != null && ( foundPathForTargetPosition.Value - target ).Length() > updatePathWhenTargetPositionHasChangedMoreThanDistance )
					Clear();

				//clear path when unit goaway from path
				if( path != null && currentIndex > 0 )
				{
					var previous = path[ currentIndex - 1 ];
					var next = path[ currentIndex ];

					var min = Math.Min( previous.Z, next.Z );
					var max = Math.Max( previous.Z, next.Z );

					var projectedPoint = MathAlgorithms.ProjectPointToLine( previous.ToVector2(), next.ToVector2(), from.ToVector2() );
					var distanceXY = ( from.ToVector2() - projectedPoint ).Length();

					if( distanceXY > maxAllowableDeviationFromPath ||
						from.Z + reachDestinationPointZDifference < min ||
						from.Z - reachDestinationPointZDifference > max )
					{
						Clear();
					}
				}

				//check if need update path
				if( path == null && updateRemainingTime == 0 )
				{
					if( findPathContext == null )
					{
						findPathContext = new Pathfinding.FindPathContext();

						////public double StepSize = 0.5;
						////public double Slop = 0.01;
						////public Vector3 PolygonPickExtents = new Vector3( 2, 2, 2 );
						//////public int MaxPolygonPath = 512;
						////public int MaxSmoothPath = 2048;
						//////public int MaxSteerPoints = 16;

						findPathContext.Start = from;
						findPathContext.End = target;

						//set wait = true to disable multithreading
						pathfinding.FindPath( findPathContext, false );

						if( !string.IsNullOrEmpty( findPathContext.Error ) )
							Log.Warning( findPathContext.Error );
					}

					if( findPathContext != null && findPathContext.Finished )
					{
						if( findPathContext.Path != null )
						{
							path = findPathContext.Path;
							foundPathForTargetPosition = target;
							currentIndex = 0;

							//disable finding a new path during specified time.
							updateRemainingTime = 0.3f;
						}
						else
						{
							path = null;
							foundPathForTargetPosition = null;
							currentIndex = 0;

							//disable finding a new path during specified time.
							updateRemainingTime = 1.0f;
						}

						findPathContext = null;
					}
				}

				//progress
				if( path != null )
				{
					Vector3 point;
					while( true )
					{
						point = path[ currentIndex ];

						if( ( from.ToVector2() - point.ToVector2() ).LengthSquared() <
							reachDestinationPointDistance * reachDestinationPointDistance &&
							Math.Abs( from.Z - point.Z ) < reachDestinationPointZDifference )
						{
							//reach point
							currentIndex++;
							if( currentIndex == path.Length )
							{
								//path is ended
								Clear();
								break;
							}
						}
						else
							break;
					}
				}
			}

			public bool FindingInProcess
			{
				get { return findPathContext != null && !findPathContext.Finished; }
			}

			public bool GetNextPointPosition( out Vector3 position )
			{
				if( path != null )
				{
					position = path[ currentIndex ];
					return true;
				}
				position = Vector3.Zero;
				return false;
			}

			public void DrawPath( ViewportRenderingContext context )
			{
				if( path != null )
				{
					var renderer = context.Owner.Simple3DRenderer;
					var offset = new Vector3( 0, 0, 0.1 );
					var color = new ColorValue( 0, 0.5, 1 );

					renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );

					for( int n = 1; n < path.Length; n++ )
					{
						var from = path[ n - 1 ] + offset;
						var to = path[ n ] + offset;
						renderer.AddLine( from, to );
					}
				}
			}
		}

		/////////////////////////////////////////

		protected override void OnMetadataGetMembersFilter( Metadata.GetMembersContext context, Metadata.Member member, ref bool skip )
		{
			base.OnMetadataGetMembersFilter( context, member, ref skip );

			if( member is Metadata.Property )
			{
				switch( member.Name )
				{
				case nameof( PathfindingSpecific ):
					if( !Pathfinding )
						skip = true;
					break;

				case nameof( CombatModeCanMove ):
				case nameof( CombatModeFindEnemyDistance ):
					if( !CombatMode )
						skip = true;
					break;

				case nameof( TrafficWalkingModeCurrentRoad ):
				case nameof( TrafficWalkingModeCurrentRoadLane ):
					//case nameof( TrafficWalkingModeDeleteObjectAtEndOfRoad ):
					if( !TrafficWalkingMode )
						skip = true;
					break;

				case nameof( LookingForFoodModeArea ):
				case nameof( LookingForFoodModeIdleTime ):
				case nameof( LookingForFoodInGroundModeMoveMaxTime ):
					if( !LookingForFoodMode )
						skip = true;
					break;
				}
			}
		}

		[Browsable( false )]
		public Character Character
		{
			get { return Parent as Character; }
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void SimulationStepTasks( Character character )
		{
			var task = CurrentTask;
			if( task != null && character != null )
			{
				//MoveToPosition, MoveToObject
				var moveTo = task as CharacterAITask_MoveTo;
				if( moveTo != null )
				{
					var moveToObject = moveTo as CharacterAITask_MoveToObject;
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
						else if( moveTo is CharacterAITask_MoveToPosition moveToPosition )
							target = moveToPosition.Target;

						var diff = target - character.TransformV.Position;
						if( diff.X == 0 && diff.Y == 0 )
							diff.X += 0.001;
						var distanceXY = diff.ToVector2().Length();
						var distanceZ = Math.Abs( diff.Z );

						if( distanceXY <= moveTo.DistanceToReach && distanceZ < 2.0 ) //!!!! character.Height )
						{
							//reach
							PathfindingClearData();
							if( task.DeleteTaskWhenReach )
								task.Dispose();
						}
						else
						{
							//move character

							if( Pathfinding )
							{
								PathfindingUpdateAndGetMoveVector( Time.SimulationDelta, moveTo.DistanceToReach, character.TransformV.Position, target, out var vector, out var findingInProcess );
								if( vector != Vector3.Zero )
									diff = vector;
								else if( findingInProcess )
									diff = Vector3.Zero;
							}

							if( diff != Vector3.Zero )
							{
								var speed = TurningSpeedOverride.Value;
								if( speed != 0 )
									character.TurningSpeedOverride = speed;
								else
									character.TurningSpeedOverride = null;
								character.TurnToDirection( diff.ToVector2F(), speed >= 10000 );
							}

							//!!!!it is better to call it less often, here it is less likely to update the task. inside in SetMoveVector the timer will be different
							character.SetMoveVector( diff.ToVector2().GetNormalize().ToVector2F(), moveTo.Run );
						}
					}
				}

				//TurnToPosition, TurnToObject
				var turnTo = task as CharacterAITask_TurnTo;
				if( turnTo != null )
				{
					var turnToObject = turnTo as CharacterAITask_TurnToObject;
					if( turnToObject != null && ( turnToObject.Target.Value == null || !turnToObject.Target.Value.EnabledInHierarchy ) )
					{
						//no target
						if( task.DeleteTaskWhenReach )
							task.Dispose();
					}
					else
					{
						Vector3 target = Vector3.Zero;
						if( turnToObject != null )
							target = turnToObject.Target.Value.TransformV.Position;
						else if( turnTo is CharacterAITask_TurnToPosition turnToPosition )
							target = turnToPosition.Target;

						var diff = ( target - character.TransformV.Position ).ToVector2F();
						if( diff.X == 0 && diff.Y == 0 )
							diff.X += 0.001f;
						diff.Normalize();

						var current = character.CurrentTurnToDirection.GetVector().ToVector2().GetNormalize();
						//var current = character.TransformV.Rotation.GetForward().ToVector2().GetNormalize();

						if( current.Equals( diff, 0.001f ) )
						{
							//reach
							if( task.DeleteTaskWhenReach )
								task.Dispose();
						}
						else
						{
							//turn character
							var speed = TurningSpeedOverride.Value;
							if( speed != 0 )
								character.TurningSpeedOverride = speed;
							else
								character.TurningSpeedOverride = null;
							character.TurnToDirection( diff, speed >= 10000 );
						}
					}
				}

				//add new tasks here or use OnPerformTaskSimulationStep. see CharacterAITask_PressButton as example

			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//!!!!call less often

			var character = Character;
			if( character != null )
			{
				var alive = character.LifeStatus.Value == Character.LifeStatusEnum.Normal;

				if( alive )
					SimulationStepTasks( character );
				else
				{
					ClearTaskQueue();
					PathfindingClearData();
				}

				if( CombatMode )
				{
					if( alive )
						CombatModeSimulationStep( character );
					else
						combatModeCurrentTarget = null;
				}

				if( LookingForFoodMode )
				{
					if( alive )
						LookingForFoodModeSimulationStep( character );
				}

				if( TrafficWalkingMode )
				{
					if( alive )
						TrafficWalkingModeSimulationStep( character );
				}

				if( alive )
					LookAtControlledCharacterByPlayerSimulationStep();
			}
		}

		public void Stop()
		{
			PathfindingClearData();
			ClearTaskQueue();
		}

		public CharacterAITask_MoveToPosition MoveTo( Vector3 target, bool run, bool clearTaskQueue = true )
		{
			if( clearTaskQueue )
			{
				PathfindingClearData();
				ClearTaskQueue();
			}

			var task = CreateComponent<CharacterAITask_MoveToPosition>( enabled: false );
			task.Target = target;
			task.Run = run;
			task.Enabled = true;

			return task;
		}

		public CharacterAITask_MoveToObject MoveTo( ObjectInSpace target, bool run, bool clearTaskQueue = true )
		{
			if( clearTaskQueue )
			{
				PathfindingClearData();
				ClearTaskQueue();
			}

			var task = CreateComponent<CharacterAITask_MoveToObject>( enabled: false );
			task.Target = target;
			task.Run = run;
			task.Enabled = true;

			return task;
		}

		public Pathfinding GetPathfinding()
		{
			var result = PathfindingSpecific.Value;
			if( result == null )
			{
				var scene = FindParent<Scene>();
				if( scene != null )
					result = NeoAxis.Pathfinding.Instances.FirstOrDefault( p => p.ParentRoot == scene );
			}
			return result;
		}

		void PathfindingUpdateAndGetMoveVector( float delta, double distanceToReach, Vector3 from, Vector3 target, out Vector3 moveVector, out bool findingInProcess )
		{
			moveVector = Vector3.Zero;
			findingInProcess = false;

			var pathfinding = GetPathfinding();
			if( pathfinding != null )
			{
				//update
				if( pathController == null )
					pathController = new PathController();
				pathController.Update( pathfinding, delta, distanceToReach, from, target );

				//get move vector
				if( pathController.GetNextPointPosition( out var nextPointPosition ) )
				{
					var vector = nextPointPosition - from;
					if( vector.X != 0 || vector.Y != 0 )
						moveVector = new Vector3( vector.ToVector2().GetNormalize(), vector.Z );
				}

				//get finding state
				findingInProcess = pathController.FindingInProcess;
			}
		}

		public void PathfindingClearData()
		{
			pathController?.Clear();
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

			combatModeUpdateTargetRemainingTime = staticRandom.Next( 1.0f );
			combatModeUpdateTasksRemainingTime = combatModeUpdateTargetRemainingTime;
			lookingForFoodUpdateTasksRemainingTime = combatModeUpdateTargetRemainingTime;
			trafficWalkingModeUpdateTasksRemainingTime = combatModeUpdateTargetRemainingTime;
		}

		private void Scene_GetRenderSceneData( Scene scene, ViewportRenderingContext context )
		{
			if( DebugVisualization )
			{
				pathController?.DrawPath( context );
				if( CombatMode )
					DebugRenderCombatMode( context );

				var character = Character;
				if( character != null )
				{
					//Move tasks

					Vector3 lastTarget = character.TransformV.Position;

					foreach( var task in GetComponents<AITask>( onlyEnabledInHierarchy: true ) )
					{
						Vector3? target = null;
						if( task is CharacterAITask_MoveToPosition moveToPosition )
							target = moveToPosition.Target;
						else if( task is CharacterAITask_MoveToObject moveToObject )
						{
							if( moveToObject.Target.Value != null )
								target = moveToObject.Target.Value.TransformV.Position;
						}

						if( target.HasValue )
						{
							var renderer = context.Owner.Simple3DRenderer;
							var offset = new Vector3( 0, 0, 0.1 );
							var color = new ColorValue( 0.3, 1, 0.3 );
							renderer.SetColor( color, color * ProjectSettings.Get.Colors.HiddenByOtherObjectsColorMultiplier );
							renderer.AddArrow( lastTarget, target.Value );

							lastTarget = target.Value;
						}
					}
				}
			}
		}

		public override void InteractionGetInfo( GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info )
		{
			if( AllowInteract )
			{
				//interact with another character
				var character = gameMode.ObjectControlledByPlayer.Value as Character;
				if( character != null )
				{
					info = new InteractiveObjectObjectInfo();
					info.AllowInteract = true;
					//info.SelectionTextInfo.Add( Name );
					//info.Text.Add( "Click to interact." );
					return;
				}
			}

			base.InteractionGetInfo( gameMode, initiator, ref info );
		}

		bool StartDialogueFlow( GameMode gameMode, Component secondParticipant, ServerNetworkService_Components.ClientItem sendToSpecifiedClient )
		{
			var dialogueFlow = DialogueFlow.Value;
			if( dialogueFlow != null && secondParticipant != null )
			{
				////check already in interaction
				//foreach( var i in gameMode.GetComponents<ContinuousInteraction>() )
				//{
				//	if( i.Creator.Value == this )
				//		return false;
				//}

				//flow graph node as entry is ok too
				var flowNode = dialogueFlow as FlowGraphNode;
				if( flowNode != null )
					dialogueFlow = flowNode.ControlledObject.Value;

				//get entry of the flow component
				var entry = ObjectEx.PropertyGet<FlowInput>( dialogueFlow, "Entry" );
				if( entry == null )
				{
					Log.Warning( "Character AI: No entry to the dialogue flow." );
					return true;
				}

				//create interaction
				var interaction = gameMode.CreateComponent<ContinuousInteraction>( enabled: false, networkMode: NetworkModeEnum.SelectedUsers );
				if( sendToSpecifiedClient != null )
					interaction.NetworkModeAddUser( sendToSpecifiedClient );
				interaction.Creator = this;
				//make reference for SecondParticipant to synchronize via network
				interaction.SecondParticipant = new Reference<Component>( null, "root:" + secondParticipant.GetPathFromRoot() );
				interaction.Enabled = true;

				//start a new flow
				ContinuousInteraction.Latest = interaction;
				var initVariables = new List<Tuple<string, object>>();
				initVariables.Add( new Tuple<string, object>( "_Interaction", interaction ) );
				var flow = Flow.Execute( ParentRoot.HierarchyController, entry, initVariables );
				interaction.DeleteWhenFlowEnded = flow;

				return true;
			}

			return false;
		}

		public override bool InteractionInputMessage( GameMode gameMode, Component initiator, InputMessage message )
		{
			//entry to dialogue flow
			var buttonDown = message as InputMessageMouseButtonDown;
			if( buttonDown != null && AllowInteract )
			{
				if( NetworkIsClient )
				{
					//var writer = 
					var m = BeginNetworkMessageToServer( "ObjectInteractionInputMessage_InteractByClick" );
					if( m != null )
					{
						//writer.Write( (byte)buttonDown.Button );
						m.End();
					}
				}
				else
				{
					if( StartDialogueFlow( gameMode, gameMode.ObjectControlledByPlayer, null ) )
						return true;
				}
			}

			return base.InteractionInputMessage( gameMode, initiator, message );
		}

		public override void InteractionEnter( ObjectInteractionContext context )
		{
			base.InteractionEnter( context );
		}

		public override void InteractionExit( ObjectInteractionContext context )
		{
			base.InteractionExit( context );
		}

		public override void InteractionUpdate( ObjectInteractionContext context )
		{
			base.InteractionUpdate( context );
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;

			if( message == "ObjectInteractionInputMessage_InteractByClick" )
			{

				//!!!!server security verifications. characters must be close. what else


				//var button = (EMouseButtons)reader.ReadByte();
				if( !reader.Complete() )
					return false;

				var scene = ParentRoot as Scene;
				if( scene != null )
				{
					var gameMode = (GameMode)scene.GetGameMode();
					if( gameMode != null )
					{
						var networkLogic = NetworkLogicUtility.GetNetworkLogic( this );
						if( networkLogic != null )
						{
							var secondParticipant = networkLogic.ServerGetObjectControlledByUser( client.User, true );
							if( secondParticipant != null )
								StartDialogueFlow( gameMode, secondParticipant, client );
						}
					}
				}
			}

			return true;
		}

		void LookAtControlledCharacterByPlayerSimulationStep()
		{
			//!!!!also look at when in dialogue (useful for multiplayer mode)

			//update look at object
			var distance = LookAtControlledCharacterByPlayerDistance.Value;
			if( distance > 0 )
			{
				//update not each simulation step
				lookAtControlledCharacterByPlayerRemainingTimeToUpdate -= Time.SimulationDelta;
				if( lookAtControlledCharacterByPlayerRemainingTimeToUpdate <= 0 )
				{
					lookAtControlledCharacterByPlayerRemainingTimeToUpdate = 1 + staticRandom.NextFloat();

					//do update

					lookAtControlledCharacterByPlayerLookAtObject = null;

					var thisCharacter = Character;
					if( thisCharacter != null )
					{
						var sphere = new Sphere( thisCharacter.GetCenteredPosition(), distance );

						var characterType = MetadataManager.GetTypeOfNetType( typeof( Character ) );
						var getItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, characterType, true, sphere );
						thisCharacter.ParentScene.GetObjectsInSpace( getItem );

						foreach( var resultItem in getItem.Result )
						{
							var character = resultItem.Object as Character;
							if( character != null && sphere.Contains( character.GetCenteredPosition() ) )
							{
								//!!!!maybe also check InputEnabled

								//detect controlled by player
								if( character.GetComponent<CharacterInputProcessing>() != null )
								{
									lookAtControlledCharacterByPlayerLookAtObject = character;
									break;
								}
							}
						}
					}
				}

				if( lookAtControlledCharacterByPlayerLookAtObject != null && !lookAtControlledCharacterByPlayerLookAtObject.EnabledInHierarchy )
					lookAtControlledCharacterByPlayerLookAtObject = null;

				//update character's task
				if( lookAtControlledCharacterByPlayerLookAtObject != null && CurrentTask == null )
				{
					var character = Character;
					if( character != null )
					{
						var direction = ( lookAtControlledCharacterByPlayerLookAtObject.GetCenteredPosition() - character.GetCenteredPosition() ).GetNormalize();

						var speed = TurningSpeedOverride.Value;
						if( speed != 0 )
							character.TurningSpeedOverride = speed;
						else
							character.TurningSpeedOverride = null;
						character.TurnToDirection( direction.ToVector2F(), speed >= 10000 );
					}
				}
			}
		}

		///////////////////////////////////////////////
		//combat mode

		struct UpdateCurrentTargetObject
		{
			public ObjectInSpace Object;
			public Vector3 TargetCenter;
			public double DistanceSquared;
		}

		[MethodImpl( (MethodImplOptions)512 )]
		void UpdateCurrentTarget( Character thisObject )
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
				combatModeCurrentTarget = objects[ 0 ].Object;
			else
				combatModeCurrentTarget = null;
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
		void CombatModeSimulationStep( Character thisObject )
		{
			if( weaponsCache == null )
				weaponsCache = thisObject.GetComponents<Weapon>( checkChildren: true, onlyEnabledInHierarchy: true );

			if( weaponsCache.Length != 0 )
			{
				//update current target
				{
					combatModeUpdateTargetRemainingTime -= Time.SimulationDelta;
					if( combatModeUpdateTargetRemainingTime <= 0 )
					{
						UpdateCurrentTarget( thisObject );
						combatModeUpdateTargetRemainingTime += 1.0f + staticRandom.Next( 0.1f );
					}

					//reset when target not exists
					if( combatModeCurrentTarget != null && combatModeCurrentTarget.Parent == null )
						combatModeCurrentTarget = null;
				}

				//update moving tasks
				{
					combatModeUpdateTasksRemainingTime -= Time.SimulationDelta;
					if( combatModeUpdateTasksRemainingTime <= 0 )
					{
						if( combatModeCurrentTarget != null )
						{
							var range = GetOptimalAttackDistance();
							var distanceSquared = ( combatModeCurrentTarget.TransformV.Position - thisObject.TransformV.Position ).LengthSquared();

							//check by distance
							if( distanceSquared > range.Maximum * range.Maximum )
							{
								var alreadyMoving = false;

								var currentTasks = GetComponents<AITask>();
								if( currentTasks.Length == 1 )
								{
									var moveToObject = currentTasks[ 0 ] as VehicleAITask_MoveToObject;
									if( moveToObject != null && moveToObject.Target.Value == combatModeCurrentTarget )
										alreadyMoving = true;
								}

								if( !alreadyMoving )
									MoveTo( combatModeCurrentTarget, AllowRun, true );
							}
							else
								ClearTaskQueue();
						}
						else
							ClearTaskQueue();
					}
				}

				//update weapons
				{

					//!!!!call less often?


					for( int n = 0; n < weaponsCache.Length; n++ )
					{
						var weapon = weaponsCache[ n ];
						var weaponType = weapon.WeaponType.Value;

						//look to
						{
							Vector3? lookTo = null;

							if( combatModeCurrentTarget != null )
							{
								var vehicle = combatModeCurrentTarget as Vehicle;
								if( vehicle != null )
								{
									vehicle.GetBox( out var box );
									lookTo = box.ToBounds().GetCenter();
								}

								var character = combatModeCurrentTarget as Character;
								if( character != null )
									lookTo = character.GetCenteredPosition();
							}

							thisObject.LookToPosition( lookTo, false );
						}

						//fire
						if( combatModeCurrentTarget != null )
						{
							var distance = ( combatModeCurrentTarget.TransformV.Position - thisObject.TransformV.Position ).Length();

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

		///////////////////////////////////////////////
		//looking for food mode

		void LookingForFoodModeSimulationStep( Character thisObject )
		{
			lookingForFoodUpdateTasksRemainingTime -= Time.SimulationDelta;
			if( lookingForFoodUpdateTasksRemainingTime <= 0 )
			{
				lookingForFoodUpdateTasksRemainingTime = 1.0f + staticRandom.Next( 1.0f );

				if( lookingForFoodCurrentTask == LookingForFoodTask.Idle )
				{
					var area = LookingForFoodModeArea.Value;
					if( area != null )
					{
						var points = area.GetPointPositions();

						var points2 = new Vector2[ points.Length ];
						for( int n = 0; n < points.Length; n++ )
							points2[ n ] = points[ n ].ToVector2();

						var bounds2 = Rectangle.Cleared;
						foreach( var point in points2 )
							bounds2.Add( point );

						Vector2? foundPosition = null;
						for( int n = 0; n < 20; n++ )
						{
							var position2 = new Vector2( staticRandom.Next( bounds2.Minimum.X, bounds2.Maximum.X ), staticRandom.Next( bounds2.Minimum.Y, bounds2.Maximum.Y ) );
							if( MathAlgorithms.IsPointInPolygon( points2, position2 ) )
							{
								foundPosition = position2;
								break;
							}
						}

						if( foundPosition.HasValue )
						{
							var targetPosition = new Vector3( foundPosition.Value, area.TransformV.Position.Z );

							MoveTo( targetPosition, false );
							lookingForFoodCurrentTask = LookingForFoodTask.Move;
							lookingForFoodUpdateTasksRemainingTime = 1.0f + staticRandom.Next( 1.0f );
						}
					}
				}
				else
				{
					var currentTask = CurrentTask;
					if( currentTask == null || currentTask.TimeCreated > LookingForFoodInGroundModeMoveMaxTime.Value )
					{
						lookingForFoodCurrentTask = LookingForFoodTask.Idle;
						lookingForFoodUpdateTasksRemainingTime = (float)LookingForFoodModeIdleTime.Value;
					}
				}
			}
		}

		///////////////////////////////////////////////
		//traffic walking mode

		[MethodImpl( (MethodImplOptions)512 )]
		void TrafficWalkingModeSimulationStep( Character thisObject )
		{
			trafficWalkingModeUpdateTasksRemainingTime -= Time.SimulationDelta;
			if( trafficWalkingModeUpdateTasksRemainingTime <= 0 )
			{
				trafficWalkingModeUpdateTasksRemainingTime = 1.0f + staticRandom.Next( 1.0f );

				//!!!!
				var maxDistanceWhenOutsideRoad = 30.0;


				//!!!!чтобы расходились если застряли

				//!!!!сейчас ходят без цели. поворачивают на перекрестке только если кончается текущая дорога



				var road = TrafficWalkingModeCurrentRoad.Value;
				if( road != null )
				{
					var roadData = road.GetRoadData();
					var roadLogicalData = road.GetLogicalData();
					if( roadData != null && roadLogicalData != null )
					{
						var roadEndTime = roadData.LastPoint.TimeOnCurve;

						//get current state

						var lane = TrafficWalkingModeCurrentRoadLane.Value;
						var laneOffset = roadData.GetLaneOffset( lane );
						var directionToBack = lane < roadData.Lanes / 2;

						var objPosition = thisObject.TransformV.Position;
						roadData.GetClosestCurveTimeToPosition( objPosition, maxDistanceWhenOutsideRoad, 0.5, out var timeOnCurve );


						//detect on end of road
						if( !directionToBack && timeOnCurve >= roadEndTime - 0.001 )
						{
							TrafficWalkingModeCurrentRoadLane = staticRandom.Next( roadData.Lanes / 2 );
							return;
						}
						if( directionToBack && timeOnCurve <= 0.001 )
						{
							TrafficWalkingModeCurrentRoadLane = roadData.Lanes - 1 - staticRandom.Next( roadData.Lanes / 2 );
							return;
						}
						////delete object
						//if( TrafficWalkingModeDeleteObjectAtEndOfRoad )
						//{
						//	thisObject.RemoveFromParent( true );
						//	return;
						//}


						//precalculate 3 seconds
						var precalculateSeconds = 3.0f;
						var precalculationDistance = thisObject.TypeCached.WalkForwardMaxSpeed.Value * precalculateSeconds;

						//calculate how much time to add depending current position
						double precalculationAddTime;
						{
							var timeOnCurve2 = timeOnCurve + ( directionToBack ? -0.01 : 0.01 );

							roadData.GetPositionByTime( timeOnCurve, out var pos1 );
							roadData.GetPositionByTime( timeOnCurve2, out var pos2 );

							var distance = ( pos2 - pos1 ).Length();
							if( distance == 0 )
								distance = 0.001;

							precalculationAddTime = precalculationDistance / distance * 0.01;
						}

						var stepCount = 2;
						for( int nStep = 0; nStep < stepCount; nStep++ )
						{
							var addTime = ( nStep + 1.0 ) / stepCount * precalculationAddTime;

							var time = timeOnCurve;
							var canCheckCrossroad = false;
							if( !directionToBack )
							{
								time += addTime;
								if( time > roadEndTime )
									canCheckCrossroad = true;
							}
							else
							{
								time -= addTime;
								if( time < 0 )
									canCheckCrossroad = true;
							}

							//get connected nodes in interval
							if( canCheckCrossroad )//can turn on crossroad only when current road is ended
							{
								var minTime = Math.Min( timeOnCurve, time );
								var maxTime = Math.Max( timeOnCurve, time );

								var connectedRoads = roadLogicalData.GetConnectedRoadsInInterval( new Range( minTime, maxTime ), null ).ToArray();
								if( connectedRoads.Length != 0 )
								{
									var selectIndex = staticRandom.Next( connectedRoads.Length - 1 );
									var connectedCrossroadRoadItem = connectedRoads[ selectIndex ];
									var connectedRoadData = connectedCrossroadRoadItem.ConnectedRoad;

									//don't select same road
									if( roadData != connectedRoadData )
									{
										//!!!!можно по идее указывать lane
										connectedRoadData.GetClosestCurveTimeToPosition( objPosition, maxDistanceWhenOutsideRoad, 0.5, out var connectedRoadTime );

										//calculate lane on new road. don't change lane?
										int newLane;
										if( !connectedCrossroadRoadItem.ForwardDirection )
											newLane = connectedRoadData.Lanes - 1 - staticRandom.Next( connectedRoadData.Lanes / 2 );
										else
											newLane = staticRandom.Next( connectedRoadData.Lanes / 2 );

										//trafficWalkingModeLastPassedCrossroad = connectedCrossroadRoadItem.Owner;
										TrafficWalkingModeCurrentRoad = connectedRoadData.Owner;
										TrafficWalkingModeCurrentRoadLane = newLane;

										connectedRoadData.GetPositionByTime( connectedRoadTime, out var position2 );
										connectedRoadData.GetDirectionByTime( connectedRoadTime, out var direction2 );
										var laneOffset2 = connectedRoadData.GetLaneOffset( lane );
										var positionOnLane2 = position2 + QuaternionF.FromDirectionZAxisUp( direction2.ToVector3F() ) * new Vector3F( 0, (float)laneOffset2, 0 );
										MoveTo( positionOnLane2, false, nStep == 0 );

										break;
									}
								}
							}

							roadData.GetPositionByTime( time, out var position );
							roadData.GetDirectionByTime( time, out var direction );
							var positionOnLane = position + QuaternionF.FromDirectionZAxisUp( direction.ToVector3F() ) * new Vector3F( 0, (float)laneOffset, 0 );
							MoveTo( positionOnLane, false, nStep == 0 );
						}

						trafficWalkingModeUpdateTasksRemainingTime = precalculateSeconds * 0.7f - 1.0f + staticRandom.Next( 1.0f );
					}
				}
			}
		}
	}
}
