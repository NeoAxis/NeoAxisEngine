// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace NeoAxis
{
	/// <summary>
	/// Task-based artificial intelligence for 2D character.
	/// </summary>
	[AddToResourcesWindow( @"Base\2D\Character 2D AI", -7897 )]
	public class Character2DAI : AI
	{
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
		public event Action<Character2DAI> AllowInteractChanged;
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
		public event Action<Character2DAI> DialogueFlowChanged;
		ReferenceField<Component> _dialogueFlow = null;

		///////////////////////////////////////////////

		[Browsable( false )]
		public Character2D Character
		{
			get { return Parent as Character2D; }
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			//task management
			var character = Character;
			if( character != null )
			{
				var task = CurrentTask;
				if( task != null )
				{
					//MoveToPosition, MoveToObject
					var moveTo = task as Character2DAITask_MoveTo;
					if( moveTo != null )
					{
						var moveToObject = moveTo as Character2DAITask_MoveToObject;
						if( moveToObject != null && ( moveToObject.Target.Value == null || !moveToObject.Target.Value.EnabledInHierarchy ) )
						{
							//no target
							if( task.DeleteTaskWhenReach )
								task.Dispose();
						}
						else
						{
							Vector2 target = Vector2.Zero;
							if( moveToObject != null )
								target = moveToObject.Target.Value.TransformV.Position.ToVector2();
							else if( moveTo is Character2DAITask_MoveToPosition moveToPosition )
								target = moveToPosition.Target;

							var diff = target - character.TransformV.Position.ToVector2();
							var distanceX = Math.Abs( diff.X );
							var distanceZ = Math.Abs( diff.Y );

							if( distanceX <= moveTo.DistanceToReach && distanceZ < character.TypeCached.Height )
							{
								//reach
								if( task.DeleteTaskWhenReach )
									task.Dispose();
							}
							else
							{
								//move character
								if( diff.X != 0 || diff.Y != 0 )
								{
									character.SetLookToDirection( diff );
									character.SetMoveVector( diff.X > 0 ? 1 : -1, moveTo.Run );
								}
							}
						}
					}
				}
			}
		}

		public void Stop()
		{
			ClearTaskQueue();
		}

		public Character2DAITask_MoveToPosition MoveTo( Vector2 target, bool run, bool clearTaskQueue = true )
		{
			if( clearTaskQueue )
				ClearTaskQueue();

			var task = CreateComponent<Character2DAITask_MoveToPosition>( enabled: false );
			task.Target = target;
			task.Run = run;
			task.Enabled = true;

			return task;
		}

		public Character2DAITask_MoveToObject MoveTo( ObjectInSpace target, bool run, bool clearTaskQueue = true )
		{
			if( clearTaskQueue )
				ClearTaskQueue();

			var task = CreateComponent<Character2DAITask_MoveToObject>( enabled: false );
			task.Target = target;
			task.Run = run;
			task.Enabled = true;

			return task;
		}

		public override void InteractionGetInfo( GameMode gameMode, Component initiator, ref InteractiveObjectObjectInfo info )
		{
			if( AllowInteract )
			{
				//interact with another character
				var character = gameMode.ObjectControlledByPlayer.Value as Character2D;
				if( character != null )
				{
					info = new InteractiveObjectObjectInfo();
					info.AllowInteract = true;
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
	}
}
