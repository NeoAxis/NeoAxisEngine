// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using NeoAxis.Editor;
using NeoAxis.Addon.Pathfinding;

namespace NeoAxis
{
	/// <summary>
	/// Task-based artificial intelligence for character.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character AI", -8997 )]
	public class Component_CharacterAI : Component_AI
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
		public event Action<Component_CharacterAI> PathfindingChanged;
		ReferenceField<bool> _pathfinding = true;

		/// <summary>
		/// Use only the specified pathfinding object. This is used for scenes in which there are several pathfinding objects.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Character AI" )]
		public Reference<Component_Pathfinding> PathfindingSpecific
		{
			get { if( _pathfindingSpecific.BeginGet() ) PathfindingSpecific = _pathfindingSpecific.Get( this ); return _pathfindingSpecific.value; }
			set { if( _pathfindingSpecific.BeginSet( ref value ) ) { try { PathfindingSpecificChanged?.Invoke( this ); } finally { _pathfindingSpecific.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="PathfindingSpecific"/> property value changes.</summary>
		public event Action<Component_CharacterAI> PathfindingSpecificChanged;
		ReferenceField<Component_Pathfinding> _pathfindingSpecific = null;

		[DefaultValue( false )]
		[Category( "Character AI" )]
		public Reference<bool> DisplayPath
		{
			get { if( _displayPath.BeginGet() ) DisplayPath = _displayPath.Get( this ); return _displayPath.value; }
			set { if( _displayPath.BeginSet( ref value ) ) { try { DisplayPathChanged?.Invoke( this ); } finally { _displayPath.EndSet(); } } }
		}
		/// <summary>Occurs when the <see cref="DisplayPath"/> property value changes.</summary>
		public event Action<Component_CharacterAI> DisplayPathChanged;
		ReferenceField<bool> _displayPath = false;

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

			public void Update( Component_Pathfinding pathfinding, float delta, double distanceToReach, Vector3 from, Vector3 target )
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
					var context = new Component_Pathfinding.FindPathContext();
					context.Start = from;
					context.End = target;

					//!!!!
					////public double StepSize = 0.5;
					////public double Slop = 0.01;
					////public Vector3 PolygonPickExtents = new Vector3( 2, 2, 2 );
					//////public int MaxPolygonPath = 512;
					////public int MaxSmoothPath = 2048;
					//////public int MaxSteerPoints = 16;

					//var found = pathfinding.FindPath( from, target, stepSize, polygonPickExtents, maxPolygonPath, maxSmoothPath, maxSteerPoints, out path );

					pathfinding.FindPath( context );

					if( !string.IsNullOrEmpty( context.Error ) )
						Log.Warning( context.Error );

					if( context.Path != null )
					{
						path = context.Path;
						foundPathForTargetPosition = target;
						currentIndex = 0;

						//can't find new path during specified time.
						updateRemainingTime = 0.3f;
					}
					else
					{
						path = null;
						foundPathForTargetPosition = null;
						currentIndex = 0;

						//can't find new path during specified time.
						updateRemainingTime = 1.0f;
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

					renderer.SetColor( color, color * ProjectSettings.Get.HiddenByOtherObjectsColorMultiplier );

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
		public Component_Character Character
		{
			get { return Parent as Component_Character; }
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
				var moveTo = task as Component_CharacterAITask_MoveTo;
				if( moveTo != null )
				{
					var moveToObject = moveTo as Component_CharacterAITask_MoveToObject;
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
						else if( moveTo is Component_CharacterAITask_MoveToPosition moveToPosition )
							target = moveToPosition.Target;

						var diff = target - character.TransformV.Position;
						var distanceXY = diff.ToVector2().Length();
						var distanceZ = Math.Abs( diff.Z );

						if( distanceXY <= moveTo.DistanceToReach && distanceZ < character.Height )
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
								PathfindingUpdateAndGetMoveVector( Time.SimulationDelta, moveTo.DistanceToReach, character.TransformV.Position, target, out var vector );
								if( vector != Vector3.Zero )
									diff = vector;
							}

							if( diff.X != 0 || diff.Y != 0 )
							{
								character.SetLookToDirection( diff );
								character.SetMoveVector( diff.ToVector2().GetNormalize(), moveTo.Run );
							}
						}
					}
				}

			}
		}

		public void Stop()
		{
			PathfindingClearData();
			ClearTaskQueue();
		}

		public Component_CharacterAITask_MoveToPosition MoveTo( Vector3 target, bool run, bool clearTaskQueue = true )
		{
			if( clearTaskQueue )
			{
				PathfindingClearData();
				ClearTaskQueue();
			}

			var task = CreateComponent<Component_CharacterAITask_MoveToPosition>( enabled: false );
			task.Target = target;
			task.Run = run;
			task.Enabled = true;

			return task;
		}

		public Component_CharacterAITask_MoveToObject MoveTo( Component_ObjectInSpace target, bool run, bool clearTaskQueue = true )
		{
			if( clearTaskQueue )
			{
				PathfindingClearData();
				ClearTaskQueue();
			}

			var task = CreateComponent<Component_CharacterAITask_MoveToObject>( enabled: false );
			task.Target = target;
			task.Run = run;
			task.Enabled = true;

			return task;
		}

		public Component_Pathfinding GetPathfinding()
		{
			var result = PathfindingSpecific.Value;
			if( result == null )
			{
				var scene = FindParent<Component_Scene>();
				if( scene != null )
					result = Component_Pathfinding.Instances.FirstOrDefault( p => p.ParentRoot == scene );
			}
			return result;
		}

		void PathfindingUpdateAndGetMoveVector( float delta, double distanceToReach, Vector3 from, Vector3 target, out Vector3 moveVector )
		{
			moveVector = Vector3.Zero;

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
			}
		}

		public void PathfindingClearData()
		{
			pathController?.Clear();
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			var scene = FindParent<Component_Scene>();
			if( scene != null )
			{
				if( EnabledInHierarchy )
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
				else
					scene.GetRenderSceneData += Scene_GetRenderSceneData;
			}
		}

		private void Scene_GetRenderSceneData( Component_Scene scene, ViewportRenderingContext context )
		{
			if( DisplayPath )
				pathController?.DrawPath( context );
		}
	}
}
