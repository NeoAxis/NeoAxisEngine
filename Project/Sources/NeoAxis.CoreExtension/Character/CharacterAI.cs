// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Task-based artificial intelligence for character.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character AI", -8997 )]
	public class CharacterAI : AI
	{
		PathController pathController;

		/////////////////////////////////////////

		/// <summary>
		/// Whether to use pathfinding functionality.
		/// </summary>
		[DefaultValue( true )]
		[Category( "Character AI" )]
		public Reference<bool> Pathfinding
		{
			get { if( _pathfinding.BeginGet() ) Pathfinding = _pathfinding.Get( this ); return _pathfinding.value; }
			set { if( _pathfinding.BeginSet( ref value ) ) { try { PathfindingChanged?.Invoke( this ); } finally { _pathfinding.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="Pathfinding"/> property value changes.</summary>
		public event Action<CharacterAI> PathfindingChanged;
		ReferenceField<bool> _pathfinding = true;

		/// <summary>
		/// Use only the specified pathfinding object. This is used for scenes in which there are several pathfinding objects.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Character AI" )]
		public Reference<Pathfinding> PathfindingSpecific
		{
			get { if( _pathfindingSpecific.BeginGet() ) PathfindingSpecific = _pathfindingSpecific.Get( this ); return _pathfindingSpecific.value; }
			set { if( _pathfindingSpecific.BeginSet( ref value ) ) { try { PathfindingSpecificChanged?.Invoke( this ); } finally { _pathfindingSpecific.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PathfindingSpecific"/> property value changes.</summary>
		public event Action<CharacterAI> PathfindingSpecificChanged;
		ReferenceField<Pathfinding> _pathfindingSpecific = null;

		/// <summary>
		/// Whether to show a calculated path.
		/// </summary>
		[DefaultValue( false )]
		[Category( "Character AI" )]
		public Reference<bool> DisplayPath
		{
			get { if( _displayPath.BeginGet() ) DisplayPath = _displayPath.Get( this ); return _displayPath.value; }
			set { if( _displayPath.BeginSet( ref value ) ) { try { DisplayPathChanged?.Invoke( this ); } finally { _displayPath.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayPath"/> property value changes.</summary>
		public event Action<CharacterAI> DisplayPathChanged;
		ReferenceField<bool> _displayPath = false;

		///// <summary>
		///// Defines a dialogue tree of the NPC.
		///// </summary>
		//[DefaultValue( null )]
		//public Reference<DialogueTree> SourceDialogueTree
		//{
		//	get { if( _sourceDialogueTree.BeginGet() ) SourceDialogueTree = _sourceDialogueTree.Get( this ); return _sourceDialogueTree.value; }
		//	set { if( _sourceDialogueTree.BeginSet( ref value ) ) { try { SourceDialogueTreeChanged?.Invoke( this ); } finally { _sourceDialogueTree.EndSet(); } } }
		//}
		///// <summary>Occurs when the <see cref="SourceDialogueTree"/> property value changes.</summary>
		//public event Action<CharacterAI> SourceDialogueTreeChanged;
		//ReferenceField<DialogueTree> _sourceDialogueTree = null;

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
				}
			}
		}

		[Browsable( false )]
		public Character Character
		{
			get { return Parent as Character; }
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//task management
			var task = CurrentTask;
			var character = Character;
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
								character.TurnToDirection( diff.ToVector3F(), false );

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

						if( current.Equals( diff, 0.001f ) )
						{
							//reach
							if( task.DeleteTaskWhenReach )
								task.Dispose();
						}
						else
						{
							//turn character
							character.TurnToDirection( new Vector3F( diff, 0 ), turnTo.TurnInstantly );
						}
					}
				}

				//add new tasks here or use OnPerformTaskSimulationStep. see CharacterAITask_PressButton as example

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
		}

		private void Scene_GetRenderSceneData( Scene scene, ViewportRenderingContext context )
		{
			if( DisplayPath )
				pathController?.DrawPath( context );
		}

		public override void ObjectInteractionGetInfo( GameMode gameMode, ref InteractiveObjectObjectInfo info )
		{
			//!!!!если есть дерево

			////talk with another character
			//var character = gameMode.ObjectControlledByPlayer.Value as Character;
			//if( character != null && SourceDialogueTree.Value != null )
			//{
			//	info = new InteractiveObjectObjectInfo();
			//	info.AllowInteract = true;
			//	info.SelectionTextInfo.Add( Name );
			//	info.SelectionTextInfo.Add( "Press E to start a dialogue." );
			//	return;
			//}

			base.ObjectInteractionGetInfo( gameMode, ref info );
		}

		public override bool ObjectInteractionInputMessage( GameMode gameMode, InputMessage message )
		{
			var keyDown = message as InputMessageKeyDown;
			if( keyDown != null )
			{
				if( keyDown.Key == EKeys.E )
				{
					//!!!!
					Log.Info( "ee" );

					////start control by a character
					//var character = gameMode.ObjectControlledByPlayer.Value as Character;
					//if( character != null )
					//{
					//	var seat = GetComponent<VehicleCharacterSeat>();
					//	if( seat != null && !seat.Character.ReferenceOrValueSpecified )
					//	{
					//		if( NetworkIsClient )
					//		{
					//			var writer = BeginNetworkMessageToServer( "PutObjectToSeat" );
					//			if( writer != null )
					//			{
					//				writer.WriteVariableUInt64( (ulong)character.NetworkID );
					//				writer.WriteVariableUInt64( (ulong)seat.NetworkID );
					//				EndNetworkMessage();
					//			}
					//		}
					//		else
					//		{
					//			seat.PutCharacterToSeat( character );
					//			gameMode.ObjectControlledByPlayer = ReferenceUtility.MakeRootReference( this );

					//			GameMode.PlayScreen?.ParentContainer?.Viewport?.NotifyInstantCameraMovement();
					//		}

					//		return true;
					//	}
					//}
				}
			}

			//!!!!
			//var mouseDown = message as InputMessageMouseButtonDown;
			//if( mouseDown != null )
			//{
			//	if( mouseDown.Button == EMouseButtons.Left )
			//	{
			//		var character = gameMode.ObjectControlledByPlayer.Value as Character;
			//		if( character != null )
			//		{
			//			if( NetworkIsClient )
			//			{
			//				var writer = BeginNetworkMessageToServer( "PutObjectToSeat" );
			//				if( writer != null )
			//				{
			//					writer.WriteVariableUInt64( (ulong)character.NetworkID );
			//					EndNetworkMessage();
			//				}
			//			}
			//			else
			//			{
			//				PutCharacterToSeat( character, out var reason );

			//				//!!!!reason
			//			}
			//		}

			//		return true;
			//	}
			//}

			return base.ObjectInteractionInputMessage( gameMode, message );
		}

		public override void ObjectInteractionEnter( ObjectInteractionContext context )
		{
			base.ObjectInteractionEnter( context );
		}

		public override void ObjectInteractionExit( ObjectInteractionContext context )
		{
			base.ObjectInteractionExit( context );
		}

		public override void ObjectInteractionUpdate( ObjectInteractionContext context )
		{
			base.ObjectInteractionUpdate( context );
		}

	}
}
