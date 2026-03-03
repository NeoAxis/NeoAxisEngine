// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// An object to process input from a player to a 2D character.
	/// </summary>
	[AddToResourcesWindow( @"Base\2D\Character 2D Input Processing", -7898 )]
	public class Character2DInputProcessing : InputProcessing
	{
		Vector2 lookDirection;
		bool firing;

		//!!!!
		//double networkSentVector = double.MaxValue;
		//bool networkSentRun;
		//Vector2 networkSentLookDirection;

		//

		[Browsable( false )]
		public Character2D Character
		{
			get { return Parent as Character2D; }
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
			{
				if( Character != null )
				{
					//get initial look direction
					lookDirection = Character.TransformV.Rotation.GetForward().ToVector2();
				}
			}
		}

		protected override void OnInputMessage( GameMode gameMode, InputMessage message )
		{
			base.OnInputMessage( gameMode, message );

			var character = Character;
			if( character != null && InputEnabled && !gameMode.FreeCamera ) //&& ( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson || gameMode.GetCameraManagementOfCurrentObject() != null ) )
			{
				//key down
				var keyDown = message as InputMessageKeyDown;
				if( keyDown != null )
				{
					//jump
					if( Character.TypeCached.JumpSupport && ( keyDown.Key == gameMode.KeyJump1 || keyDown.Key == gameMode.KeyJump2 ) )
					{
						if( NetworkIsClient )
							Character.JumpClient();
						else
							Character.Jump();
					}

					//!!!!how to configure? keys to activate items too
					//drop item
					if( keyDown.Key >= EKeys.D1 && keyDown.Key <= EKeys.D8 )
					{
						if( IsKeyPressed( gameMode.KeyDrop1 ) || IsKeyPressed( gameMode.KeyDrop2 ) )
						{
							var index = keyDown.Key - EKeys.D1;

							var items = character.GetAllItems();
							var item = index < items.Length ? items[ index ] : null;
							if( item != null )
							{
								var amount = 1;
								if( NetworkIsClient )
									character.ItemDropClient( item, amount );
								else
									character.ItemDrop( gameMode, item, amount );
							}
						}
					}

					////drop item
					//if( keyDown.Key == gameMode.KeyDrop1 || keyDown.Key == gameMode.KeyDrop2 )
					//{
					//	var item = Character.GetActiveItem();//var item = Character.ItemGetFirst();
					//	if( item != null )
					//	{
					//		var amount = 1;
					//		if( NetworkIsClient )
					//			character.ItemDropClient( item, amount );
					//		else
					//			character.ItemDrop( gameMode, item, amount );
					//	}
					//}
				}

				//mouse down
				var mouseDown = message as InputMessageMouseButtonDown;
				if( mouseDown != null && mouseDown.Button == EMouseButtons.Left )
				{
					var weapon = Character.GetActiveWeapon();
					if( weapon != null )
					{
						firing = true;

						if( NetworkIsClient )
							weapon.FiringBeginClient();
						else
							weapon.FiringBegin();
					}
				}

				//mouse up
				var mouseUp = message as InputMessageMouseButtonUp;
				if( mouseUp != null && mouseUp.Button == EMouseButtons.Left )
					firing = false;

				////mouse move
				//var mouseMove = message as InputMessageMouseMove;
				//if( mouseMove != null && MouseRelativeMode )
				//{
				//	if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.None || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson )
				//	{
				//		var mouseOffset = mouseMove.Position;

				//		var fpsCamera = gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson;

				//		//!!!!
				//		Vector2 sens = new Vector2( 1, 1 ) * 2;
				//		//Vector2 sens = GameControlsManager.Instance.MouseSensitivity * 2;

				//		lookDirection.Horizontal -= mouseOffset.X * sens.X;
				//		lookDirection.Vertical -= mouseOffset.Y * sens.Y;

				//		double limit = fpsCamera ? 0.1 : Math.PI / 8;
				//		if( lookDirection.Vertical < -( Math.PI / 2 - limit ) )
				//			lookDirection.Vertical = -( Math.PI / 2 - limit );
				//		if( lookDirection.Vertical > ( Math.PI / 2 - limit ) )
				//			lookDirection.Vertical = ( Math.PI / 2 - limit );
				//	}
				//}

			}
		}

		void ObjectControlSimulationStep()
		{
			var character = Character;
			if( character != null && InputEnabled )
			{
				double vector = 0;
				bool run = false;

				//move
				var scene = ParentRoot as Scene;
				if( scene != null )
				{
					var gameMode = (GameMode)scene.GetGameMode();
					if( gameMode != null )
					{
						if( IsKeyPressed( gameMode.KeyLeft1 ) || IsKeyPressed( gameMode.KeyLeft2 ) /*|| IsKeyPressed( EKeys.NumPad4 )*/ )
							vector -= 1.0;
						if( IsKeyPressed( gameMode.KeyRight1 ) || IsKeyPressed( gameMode.KeyRight2 ) /*|| IsKeyPressed( EKeys.NumPad6 )*/ )
							vector += 1.0;

						if( character.TypeCached.RunSupport )
							run = IsKeyPressed( gameMode.KeyRun1 ) || IsKeyPressed( gameMode.KeyRun2 );
					}
				}

				//update lookDirection
				if( vector != 0 )
					lookDirection = new Vector2( vector > 0 ? 1 : -1, 0 );

				character.SetMoveVector( vector, run );
				character.SetLookToDirection( lookDirection );

				//send data to the server
				if( NetworkIsClient )
				{
					//!!!!почему не останавливается? ведь должно быть постоянно SetMoveVector
					//if( networkSentVector != vector || networkSentRun != run || networkSentLookDirection != lookDirection )
					//{
					//	networkSentVector = vector;
					//	networkSentRun = run;
					//	networkSentLookDirection = lookDirection;

					var m = BeginNetworkMessageToServer( "UpdateObjectControlCharacter2D" );
					if( m != null )
					{
						var writer = m.Writer;
						writer.Write( new HalfType( vector ) );
						writer.Write( run );
						writer.Write( lookDirection.ToVector2H() );
						m.End();
					}
					//}
				}


				//!!!!don't send same for weapon

				//update firing
				if( IsMouseButtonPressed( EMouseButtons.Left ) && firing )
				{
					var weapon = Character.GetActiveWeapon();
					if( weapon != null )
					{
						if( NetworkIsClient )
							weapon.FiringBeginClient();
						else
							weapon.FiringBegin();
					}
				}
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( NetworkIsSingle )
				ObjectControlSimulationStep();
		}

		protected override void OnSimulationStepClient()
		{
			base.OnSimulationStepClient();

			ObjectControlSimulationStep();
		}

		protected override bool OnReceiveNetworkMessageFromClient( ServerNetworkService_Components.ClientItem client, string message, ArrayDataReader reader )
		{
			if( !base.OnReceiveNetworkMessageFromClient( client, message, reader ) )
				return false;

			var character = Character;
			if( character != null )
			{
				//security check the object is controlled by the player
				var networkLogic = NetworkLogicUtility.GetNetworkLogic( character );
				if( networkLogic != null && networkLogic.ServerGetObjectControlledByUser( client.User, true ) == character )
				{
					if( message == "UpdateObjectControlCharacter2D" )
					{
						var vector = (float)reader.ReadHalf();
						var run = reader.ReadBoolean();
						var lookDirection = reader.ReadVector2H().ToVector2();
						if( !reader.Complete() )
							return false;

						MathEx.Clamp( ref vector, -1, 1 );
						if( lookDirection != Vector2.Zero )
							lookDirection.Normalize();

						character.SetMoveVector( vector, run );
						character.SetLookToDirection( lookDirection );
					}
				}
			}

			return true;
		}
	}
}
