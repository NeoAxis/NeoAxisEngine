// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// An object to process input from a player to a character.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character Input Processing", -8998 )]
	public class CharacterInputProcessing : InputProcessing
	{
		bool[] firing = new bool[ 3 ];

		//

		[Browsable( false )]
		public Character Character
		{
			get { return Parent as Character; }
		}

		public void UpdateTurnToDirectionAndLookToToPosition( GameMode gameMode, Vector2F? firstPersonMouseOffset )
		{
			var scene = ParentRoot as Scene;
			if( scene == null )
				return;

			var character = Character;

			//first person camera
			if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson )
			{
				if( firstPersonMouseOffset.HasValue )
				{
					//update turn to direction

					var turnToDirection = character.CurrentTurnToDirection;

					turnToDirection.Horizontal -= firstPersonMouseOffset.Value.X;
					turnToDirection.Vertical -= firstPersonMouseOffset.Value.Y;

					var limit = new DegreeF( 20 ).InRadians();
					if( turnToDirection.Vertical < -( MathEx.PI / 2 - limit ) )
						turnToDirection.Vertical = -( MathEx.PI / 2 - limit );
					if( turnToDirection.Vertical > MathEx.PI / 2 - limit )
						turnToDirection.Vertical = MathEx.PI / 2 - limit;

					character.TurnToDirection( turnToDirection, true );

					//update third person direction to have same camera direction after camera type change
					gameMode.ThirdPersonCameraHorizontalAngle = new Radian( turnToDirection.Horizontal ).InDegrees();
					gameMode.ThirdPersonCameraVerticalAngle = new Radian( turnToDirection.Vertical ).InDegrees();
				}

				//calculate LookToPosition
				{
					var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
					var cameraSettings = viewport.CameraSettings;

					var characterCenter = character.TransformV.Position + new Vector3( 0, 0, character.TypeCached.Height * 0.5 );
					var cameraDistanceToCharacter = ( characterCenter - cameraSettings.Position ).Length();

					var ray = new Ray( cameraSettings.Position, cameraSettings.Direction * 1000 );
					var item = new PhysicsRayTestItem( ray, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.None );
					scene.PhysicsRayTest( item );

					var lookAt = ray.GetPointOnRay( 1 );

					foreach( var resultItem in item.Result )
					{
						//skip character body
						if( resultItem.Body == character.PhysicalBody )
							continue;

						//check the object is too close to the camera. before the character
						var pos = ray.GetPointOnRay( resultItem.DistanceScale );
						var distanceToCamera = ( pos - cameraSettings.Position ).Length();
						if( distanceToCamera < cameraDistanceToCharacter )
							continue;

						//found
						lookAt = pos;
						break;
					}

					character.LookToPosition( lookAt, true );
				}
			}

			//third person camera
			if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson )
			{
				var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
				var cameraSettings = viewport.CameraSettings;

				var characterCenter = character.TransformV.Position + new Vector3( 0, 0, character.TypeCached.Height * 0.5 );
				var cameraDistanceToCharacter = ( characterCenter - cameraSettings.Position ).Length();

				//change it to calibrate end point when no collision with objects by the ray
				var rayDistance = 100;

				var ray = new Ray( cameraSettings.Position, cameraSettings.Direction * rayDistance );
				var item = new PhysicsRayTestItem( ray, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.None );
				scene.PhysicsRayTest( item );

				//find weapon look at target

				var lookAt = ray.GetPointOnRay( 1 );

				foreach( var resultItem in item.Result )
				{
					//skip character body
					if( resultItem.Body == character.PhysicalBody )
						continue;

					//check the object is too close to the camera. before the character
					var pos = ray.GetPointOnRay( resultItem.DistanceScale );
					var distanceToCamera = ( pos - cameraSettings.Position ).Length();
					if( distanceToCamera < cameraDistanceToCharacter )
						continue;

					//found
					lookAt = pos;
					break;
				}

				Character.LookToPosition( lookAt, false );


				////var lookDirection = ( lookAt - characterCenter ).GetNormalize();

				////need?
				////float limit = fpsCamera ? 0.1f : MathEx.PI / 8;
				////if( lookDirection.Vertical < -( MathEx.PI / 2 - limit ) )
				////	lookDirection.Vertical = -( MathEx.PI / 2 - limit );
				////if( lookDirection.Vertical > MathEx.PI / 2 - limit )
				////	lookDirection.Vertical = MathEx.PI / 2 - limit;

				////if( !gameMode.ThirdPersonCameraFollowDirection )
				////{

				////Character.TurnToDirection( (float)MathEx.DegreeToRadian( gameMode.ThirdPersonCameraHorizontalAngle.Value ), false );

				////var direction = new SphericalDirection( MathEx.DegreeToRadian( gameMode.ThirdPersonCameraHorizontalAngle.Value ), MathEx.DegreeToRadian( gameMode.ThirdPersonCameraVerticalAngle.Value ) );
				////Character.TurnToDirection( direction.ToSphericalDirectionF(), true );

				////	//Character.TurnToDirection( SphericalDirectionF.FromVector( lookDirection.ToVector3F() ), true );
				////}

				////Character.LookToPosition( lookAt, false );
			}
		}

		protected override void OnInputMessage( GameMode gameMode, InputMessage message )
		{
			base.OnInputMessage( gameMode, message );

			var character = Character;

			if( character != null && InputEnabled && !gameMode.FreeCamera && ( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson || gameMode.GetCameraManagementOfCurrentObject() != null ) )
			{
				//key down
				var keyDown = message as InputMessageKeyDown;
				if( keyDown != null )
				{
					//jump
					var characterType = character.TypeCached;//CharacterType.Value;
					if( characterType != null && characterType.Jump && ( keyDown.Key == gameMode.KeyJump1 || keyDown.Key == gameMode.KeyJump2 ) )
					{
						if( NetworkIsClient )
							character.JumpClient();
						else
							character.Jump();
					}

					//!!!!how to configure? keys to activate items too
					//drop item
					if( keyDown.Key >= EKeys.D1 && keyDown.Key <= EKeys.D8 && character.TypeCached.AllowManageInventory )
					{
						if( IsKeyPressed( gameMode.KeyDrop1 ) || IsKeyPressed( gameMode.KeyDrop2 ) )
						{
							var index = keyDown.Key - EKeys.D1;

							var items = character.GetAllItems();
							var item = index < items.Length ? items[ index ] : null;
							if( item != null )
							{
								//disable drop when weapon firing
								var weapon = item as Weapon;
								if( weapon == null || !weapon.IsFiringAnyMode() )
								{
									var amount = 1;
									if( NetworkIsClient )
										character.ItemDropClient( item, amount );
									else
										character.ItemDrop( gameMode, item, amount );
								}
							}
						}
					}

					////drop item
					//if( keyDown.Key == gameMode.KeyDrop1 || keyDown.Key == gameMode.KeyDrop2 )
					//{
					//	var item = character.GetActiveItem();
					//	//var item = character.ItemGetFirst();
					//	if( item != null )
					//	{
					//		var amount = 1;
					//		if( NetworkIsClient )
					//			character.ItemDropClient( item, amount );
					//		else
					//			character.ItemDrop( gameMode, item, amount );
					//	}
					//}

					//exit from seat
					if( keyDown.Key == gameMode.KeyInteract1 || keyDown.Key == gameMode.KeyInteract2 )
					{
						//get seat where the character sitting
						Seat seat = null;
						{
							var scene = character.ParentScene;
							if( scene != null )
							{
								var getObjectsItem = new Scene.GetObjectsInSpaceItem( Scene.GetObjectsInSpaceItem.CastTypeEnum.All, null, true, character.SpaceBounds.BoundingBox );
								scene.GetObjectsInSpace( getObjectsItem );
								foreach( var resultItem in getObjectsItem.Result )
								{
									var seat2 = resultItem.Object as Seat;
									if( seat2 != null && seat2.GetSeatIndexByObject( character ) != -1 )
									{
										seat = seat2;
										break;
									}
								}
							}
						}

						if( seat != null )
						{
							var seatIndex = seat.GetSeatIndexByObject( character );
							if( seatIndex != -1 )
							{
								if( NetworkIsClient )
								{
									var m = BeginNetworkMessageToServer( "RemoveObjectFromSeat" );
									if( m != null )
									{
										m.Writer.WriteVariableUInt64( (ulong)seat.NetworkID );
										m.Writer.WriteVariableInt32( seatIndex );
										m.End();
									}
								}
								else
								{
									seat.RemoveObjectFromSeat( seatIndex );
									GameMode.PlayScreen?.ParentContainer?.Viewport?.NotifyInstantCameraMovement();
								}
							}
						}
					}
				}

				//mouse down
				var mouseDown = message as InputMessageMouseButtonDown;
				if( mouseDown != null && ( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right ) )
				{
					var weapon = character.GetActiveWeapon();
					if( weapon != null )
					{
						var mode = mouseDown.Button == EMouseButtons.Left ? 1 : 2;

						firing[ mode ] = true;

						if( NetworkIsClient )
							weapon.FiringBeginClient( mode );
						else
							weapon.FiringBegin( mode, 0 );
					}
				}

				//mouse up
				var mouseUp = message as InputMessageMouseButtonUp;
				if( mouseUp != null && mouseUp.Button == EMouseButtons.Left )
					firing[ 1 ] = false;
				if( mouseUp != null && mouseUp.Button == EMouseButtons.Right )
					firing[ 2 ] = false;

				//mouse move
				var mouseMove = message as InputMessageMouseMove;
				if( mouseMove != null && MouseRelativeMode )
				{
					if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson )
					{
						if( !character.Sitting )
						{
							var sensitivity = new Vector2F( (float)GameMode.MouseSensitivity, (float)GameMode.MouseSensitivity ) * 2;
							var mouseOffset = mouseMove.Position.ToVector2F() * sensitivity;

							UpdateTurnToDirectionAndLookToToPosition( gameMode, mouseOffset );
						}
					}
				}
			}
		}

		void ObjectControlSimulationStep()
		{
			var character = Character;
			if( character != null && InputEnabled )
			{
				var scene = ParentRoot as Scene;
				var gameMode = (GameMode)scene?.GetGameMode();//var gameMode = scene?.GetComponent<GameMode>();

				if( gameMode != null && !gameMode.FreeCamera && ( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson || gameMode.GetCameraManagementOfCurrentObject() != null ) )
				{

					character.AllowLookToBackWhenNoActiveItem = gameMode.ThirdPersonCameraAllowLookToBackWhenNoActiveItem;

					//move
					{
						Vector2F localVector = Vector2F.Zero;
						bool run = false;

						if( gameMode != null )
						{
							if( IsKeyPressed( gameMode.KeyForward1 ) || IsKeyPressed( gameMode.KeyForward2 ) )
								localVector.X += 1.0f;
							if( IsKeyPressed( gameMode.KeyBackward1 ) || IsKeyPressed( gameMode.KeyBackward2 ) )
								localVector.X -= 1.0f;
							if( IsKeyPressed( gameMode.KeyLeft1 ) || IsKeyPressed( gameMode.KeyLeft2 ) )
								localVector.Y += 1.0f;
							if( IsKeyPressed( gameMode.KeyRight1 ) || IsKeyPressed( gameMode.KeyRight2 ) )
								localVector.Y -= 1.0f;

							var characterType = Character.TypeCached;//CharacterType.Value;
							if( characterType != null && characterType.Run )
								run = IsKeyPressed( gameMode.KeyRun1 ) || IsKeyPressed( gameMode.KeyRun2 );

							var touchSlider = TouchSliders[ 0 ].ToVector2F();
							if( touchSlider != Vector2F.Zero )
							{
								var length = touchSlider.Length();
								touchSlider.Normalize();
								localVector += new Vector2F( -touchSlider.Y, -touchSlider.X );

								if( length > 1 && characterType != null && characterType.Run )
									run = true;
							}
						}

						//movement works depending on camera direction

						var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
						var cameraSettings = viewport.CameraSettings;

						var allowLookToBack = character.AllowLookToBackWhenNoActiveItem && character.GetActiveItem() == null;

						var vector = Vector2F.Zero;

						var direction = cameraSettings.Direction.ToVector2F();
						if( direction != Vector2.Zero )
						{
							direction.Normalize();
							var rotation = QuaternionF.FromDirectionZAxisUp( new Vector3F( direction, 0 ) );
							vector = ( new Vector3F( localVector.X, localVector.Y, 0 ) * rotation ).ToVector2();
							if( vector != Vector2F.Zero )
								vector.Normalize();
						}
						//var vector = ( new Vector3F( localVector.X, localVector.Y, 0 ) * cameraSettings.Rotation ).ToVector2F();
						//if( vector != Vector2F.Zero )
						//	vector.Normalize();

						character.SetMoveVector( vector, run );


						//Vector2F vector = ( new Vector3F( localVector.X, localVector.Y, 0 ) * character.TransformV.Rotation ).ToVector2F();
						//if( vector != Vector2.Zero )
						//{
						//	var length = vector.Length();
						//	if( length > 1 )
						//		vector /= length;
						//}

						//character.SetMoveVector( vector, run );


						if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson )
						{
							var turnToVector = vector;

							if( turnToVector != Vector2F.Zero )
							{
								//switch to backward rotation and backward animation
								if( localVector.X < -0.1f && !allowLookToBack )
									turnToVector *= -1;

								character.TurnToDirection( turnToVector, false );
							}
							else
							{
								//stop turning
								character.TurnToDirection( null, false );

								////turn forward when idle
								//character.TurnToDirection( cameraSettings.Rotation.GetForward().ToVector2F(), false );
							}
						}
					}

					//update turn to direction and weapon target
					UpdateTurnToDirectionAndLookToToPosition( gameMode, null );


					//!!!!don't send same for weapon if possible. jump too


					//update firing
					if( IsMouseButtonPressed( EMouseButtons.Left ) && firing[ 1 ] )
					{
						var weapon = Character.GetActiveWeapon();
						if( weapon != null )
						{
							if( NetworkIsClient )
								weapon.FiringBeginClient( 1 );
							else
								weapon.FiringBegin( 1, 0 );
						}
					}
					if( IsMouseButtonPressed( EMouseButtons.Right ) && firing[ 2 ] )
					{
						var weapon = Character.GetActiveWeapon();
						if( weapon != null )
						{
							if( NetworkIsClient )
								weapon.FiringBeginClient( 2 );
							else
								weapon.FiringBegin( 2, 0 );
						}
					}

					//send data to the server
					if( NetworkIsClient )
					{

						//!!!!maybe send some data not each step

						//!!!!no sense to send same values. firing too

						//!!!!pack lookTo


						var m = BeginNetworkMessageToServer( "UpdateObjectControlCharacter" );
						if( m != null )
						{
							var writer = m.Writer;
							writer.Write( character.AllowLookToBackWhenNoActiveItem );
							writer.Write( character.MoveVector.ToVector2H() );
							writer.Write( character.MoveVectorRun );

							var turnInstantly = gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson;
							writer.Write( turnInstantly );

							if( turnInstantly )
							{
								writer.Write( new HalfType( character.CurrentTurnToDirection.Horizontal ) );
								writer.Write( new HalfType( character.CurrentTurnToDirection.Vertical ) );

								writer.Write( character.CurrentLookToPosition.HasValue );
								if( character.CurrentLookToPosition.HasValue )
									writer.Write( character.CurrentLookToPosition.Value );
							}
							else
							{
								writer.Write( character.RequiredTurnToDirection.HasValue );
								if( character.RequiredTurnToDirection.HasValue )
								{
									writer.Write( new HalfType( character.RequiredTurnToDirection.Value.Horizontal ) );
									writer.Write( new HalfType( character.RequiredTurnToDirection.Value.Vertical ) );
								}

								writer.Write( character.RequiredLookToPosition.HasValue );
								if( character.RequiredLookToPosition.HasValue )
									writer.Write( character.RequiredLookToPosition.Value );
							}

							m.End();
						}
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
					if( message == "UpdateObjectControlCharacter" )
					{
						var allowLookToBackWhenNoActiveItem = reader.ReadBoolean();
						var vector = reader.ReadVector2H().ToVector2F();
						if( vector != Vector2F.Zero )
							vector.Normalize();
						var run = reader.ReadBoolean();

						var turnInstantly = reader.ReadBoolean();

						SphericalDirectionF? turnToDirection = null;
						Vector3? lookToPosition = null;

						if( turnInstantly )
						{
							turnToDirection = new SphericalDirectionF( reader.ReadHalf(), reader.ReadHalf() );
							if( reader.ReadBoolean() )
								lookToPosition = reader.ReadVector3();
						}
						else
						{
							if( reader.ReadBoolean() )
								turnToDirection = new SphericalDirectionF( reader.ReadHalf(), reader.ReadHalf() );
							if( reader.ReadBoolean() )
								lookToPosition = reader.ReadVector3();
						}

						if( !reader.Complete() )
							return false;

						character.AllowLookToBackWhenNoActiveItem = allowLookToBackWhenNoActiveItem;
						character.SetMoveVector( vector, run );
						character.TurnToDirection( turnToDirection, turnInstantly );
						character.LookToPosition( lookToPosition, turnInstantly );
					}
					else if( message == "RemoveObjectFromSeat" )
					{
						var seatNetworkID = (long)reader.ReadVariableUInt64();
						var seatIndex = reader.ReadVariableInt32();
						if( !reader.Complete() )
							return false;


						//!!!!verify security


						var seat = ParentRoot.HierarchyController.GetComponentByNetworkID( seatNetworkID ) as Seat;
						if( seat != null )
						{
							seat.RemoveObjectFromSeat( seatIndex );
							//networkLogic.ServerChangeObjectControlled( client.User, obj );
						}
					}
				}
			}

			return true;
		}
	}
}
