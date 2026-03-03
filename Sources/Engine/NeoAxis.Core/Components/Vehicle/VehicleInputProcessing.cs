// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// An object to process input from a player to a vehicle.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Vehicle\Vehicle Input Processing", 22004 )]
	public class VehicleInputProcessing : InputProcessing
	{
		bool[] firing = new bool[ 3 ];

		//

		[Browsable( false )]
		public Vehicle Vehicle
		{
			get { return Parent as Vehicle; }
		}

		public void UpdateTurnToDirectionAndLookToToPosition( GameMode gameMode )//, Vector2F? firstPersonMouseOffset )
		{
			var scene = ParentRoot as Scene;
			if( scene == null )
				return;

			var vehicle = Vehicle;

			//first person camera
			if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson )
			{
				//just forward. maybe implement targeting

				var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
				var cameraSettings = viewport.CameraSettings;

				var ray = new Ray( cameraSettings.Position, cameraSettings.Direction * 1000 );
				var lookAt = ray.GetPointOnRay( 1 );

				vehicle.LookToPosition( lookAt );
			}

			//if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson )
			//{
			//	if( firstPersonMouseOffset.HasValue )
			//	{
			//		//update turn to direction

			//		var turnToDirection = character.CurrentTurnToDirection;

			//		turnToDirection.Horizontal -= firstPersonMouseOffset.Value.X;
			//		turnToDirection.Vertical -= firstPersonMouseOffset.Value.Y;

			//		var limit = new DegreeF( 20 ).InRadians();
			//		if( turnToDirection.Vertical < -( MathEx.PI / 2 - limit ) )
			//			turnToDirection.Vertical = -( MathEx.PI / 2 - limit );
			//		if( turnToDirection.Vertical > MathEx.PI / 2 - limit )
			//			turnToDirection.Vertical = MathEx.PI / 2 - limit;

			//		character.TurnToDirection( turnToDirection, true );

			//		//update third person direction to have same camera direction after camera type change
			//		gameMode.ThirdPersonCameraHorizontalAngle = new Radian( turnToDirection.Horizontal ).InDegrees();
			//		gameMode.ThirdPersonCameraVerticalAngle = new Radian( turnToDirection.Vertical ).InDegrees();
			//	}

			//	//calculate LookToPosition
			//	{
			//		var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
			//		var cameraSettings = viewport.CameraSettings;

			//		var characterCenter = character.TransformV.Position + new Vector3( 0, 0, character.TypeCached.Height * 0.5 );
			//		var cameraDistanceToCharacter = ( characterCenter - cameraSettings.Position ).Length();

			//		var ray = new Ray( cameraSettings.Position, cameraSettings.Direction * 1000 );
			//		var item = new PhysicsRayTestItem( ray, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.None );
			//		scene.PhysicsRayTest( item );

			//		var lookAt = ray.GetPointOnRay( 1 );

			//		foreach( var resultItem in item.Result )
			//		{
			//			//skip character body
			//			if( resultItem.Body == character.PhysicalBody )
			//				continue;

			//			//check the object is too close to the camera. before the character
			//			var pos = ray.GetPointOnRay( resultItem.DistanceScale );
			//			var distanceToCamera = ( pos - cameraSettings.Position ).Length();
			//			if( distanceToCamera < cameraDistanceToCharacter )
			//				continue;

			//			//found
			//			lookAt = pos;
			//			break;
			//		}

			//		character.LookToPosition( lookAt, true );
			//	}
			//}

			//third person camera
			if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson )
			{
				var viewport = RenderingSystem.ApplicationRenderTarget.Viewports[ 0 ];
				var cameraSettings = viewport.CameraSettings;

				//!!!!?
				var vehicleCenter = vehicle.TransformV.Position;// + new Vector3( 0, 0, vehicle.TypeCached.Height * 0.5 );
				var cameraDistanceToVehicle = ( vehicleCenter - cameraSettings.Position ).Length();

				//change it to calibrate end point when no collision with objects by the ray
				var rayDistance = 100;

				var ray = new Ray( cameraSettings.Position, cameraSettings.Direction * rayDistance );
				var item = new PhysicsRayTestItem( ray, PhysicsRayTestItem.ModeEnum.OneClosestForEach, PhysicsRayTestItem.FlagsEnum.None );
				scene.PhysicsRayTest( item );

				//find weapon look at target

				var lookAt = ray.GetPointOnRay( 1 );

				foreach( var resultItem in item.Result )
				{

					//!!!!все другие вложенные тоже пропустить


					//skip vehicle body
					if( resultItem.Body == vehicle.PhysicalBody )
						continue;

					//check the object is too close to the camera. before the vehicle
					var pos = ray.GetPointOnRay( resultItem.DistanceScale );
					var distanceToCamera = ( pos - cameraSettings.Position ).Length();
					if( distanceToCamera < cameraDistanceToVehicle )
						continue;

					//found
					lookAt = pos;
					break;
				}

				vehicle.LookToPosition( lookAt );
			}
		}

		public void ExitObjectFromVehicle( GameMode gameMode, ObjectInSpace obj )
		{
			var seatIndex = Vehicle.GetSeatIndexByObject( obj );
			if( seatIndex != -1 )
			{
				if( NetworkIsClient )
				{
					var m = BeginNetworkMessageToServer( "RemoveObjectFromSeat" );
					if( m != null )
					{
						m.Writer.WriteVariableInt32( seatIndex );
						m.Writer.WriteVariableUInt64( (ulong)obj.NetworkID );
						m.End();
					}
				}
				else
				{
					Vehicle.RemoveObjectFromSeat( seatIndex, true );
					gameMode.ObjectControlledByPlayer = ReferenceUtility.MakeRootReference( obj );
					GameMode.PlayScreen?.ParentContainer?.Viewport?.NotifyInstantCameraMovement();
				}
			}
		}

		public void ExitAllObjectsFromVehicle( GameMode gameMode )
		{
			var objectsOnSeats = new List<ObjectInSpace>();
			for( int n = 0; n < Vehicle.ObjectsOnSeats.Count; n++ )
			{
				var obj2 = Vehicle.ObjectsOnSeats[ n ].Value;
				if( obj2 != null )
					objectsOnSeats.Add( obj2 );
			}

			foreach( var obj in objectsOnSeats )
				ExitObjectFromVehicle( gameMode, obj );
		}

		protected override void OnInputMessage( GameMode gameMode, InputMessage message )
		{
			base.OnInputMessage( gameMode, message );

			var vehicle = Vehicle;

			if( vehicle != null )
			{
				//input enabled changed
				{
					var m = message as InputMessageInputEnabledChanged;
					if( m != null && !m.Value )
					{
						vehicle.RequiredLookToPosition = null;
					}
				}

				if( InputEnabled && !gameMode.FreeCamera && ( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson || gameMode.GetCameraManagementOfCurrentObject() != null ) )
				{
					//key down
					var keyDown = message as InputMessageKeyDown;
					if( keyDown != null )
					{
						//exit from the vehicle
						if( keyDown.Key == gameMode.KeyInteract1 || keyDown.Key == gameMode.KeyInteract2 )
						{
							//!!!!select seat/obj

							ObjectInSpace firstObjectOnSeat = null;
							for( int n = 0; n < Vehicle.ObjectsOnSeats.Count; n++ )
							{
								var obj2 = Vehicle.ObjectsOnSeats[ n ].Value;
								if( obj2 != null )
								{
									firstObjectOnSeat = obj2;
									break;
								}
							}

							var obj = firstObjectOnSeat;//var obj = gameMode.ObjectControlledByPlayer.Value as ObjectInSpace;
							if( obj != null )
								ExitObjectFromVehicle( gameMode, obj );


							////var seat = Vehicle.GetComponent<VehicleSeat>();
							////if( seat != null )
							////{
							////	var character = seat.Character.Value;
							////	if( character != null )
							////	{
							////		if( NetworkIsClient )
							////		{
							////			var writer = BeginNetworkMessageToServer( "RemoveCharacterFromSeat" );
							////			if( writer != null )
							////			{
							////				writer.WriteVariableUInt64( (ulong)character.NetworkID );
							////				writer.WriteVariableUInt64( (ulong)seat.NetworkID );
							////				EndNetworkMessage();
							////			}
							////		}
							////		else
							////		{
							////			seat.RemoveCharacterFromSeat();
							////			gameMode.ObjectControlledByPlayer = ReferenceUtility.MakeRootReference( character );

							////			GameMode.PlayScreen?.ParentContainer?.Viewport?.NotifyInstantCameraMovement();
							////		}
							////	}
							////}
						}

						if( keyDown.Key == gameMode.KeyHeadlightsLow1 || keyDown.Key == gameMode.KeyHeadlightsLow2 )
						{
							var value = vehicle.HeadlightsLow.Value > 0.5f ? 0.0f : 1.0f;

							if( NetworkIsClient )
							{
								var m = BeginNetworkMessageToServer( "HeadlightsLow" );
								if( m != null )
								{
									m.Writer.Write( value );
									m.End();
								}
							}
							else
							{
								vehicle.HeadlightsLow = value;
								//reset high
								if( value != 0 )
									vehicle.HeadlightsHigh = 0;
							}
						}

						if( keyDown.Key == gameMode.KeyHeadlightsHigh1 || keyDown.Key == gameMode.KeyHeadlightsHigh2 )
						{
							var value = vehicle.HeadlightsHigh.Value > 0.5f ? 0.0f : 1.0f;

							if( NetworkIsClient )
							{
								var m = BeginNetworkMessageToServer( "HeadlightsHigh" );
								if( m != null )
								{
									m.Writer.Write( value );
									m.End();
								}
							}
							else
							{
								vehicle.HeadlightsHigh = value;
								//reset low
								if( value != 0 )
									vehicle.HeadlightsLow = 0;
							}
						}

						if( keyDown.Key == gameMode.KeyLeftTurnSignal1 || keyDown.Key == gameMode.KeyLeftTurnSignal2 )
						{
							var value = vehicle.LeftTurnSignal.Value > 0.5f ? 0.0f : 1.0f;

							if( NetworkIsClient )
							{
								var m = BeginNetworkMessageToServer( "LeftTurnSignal" );
								if( m != null )
								{
									m.Writer.Write( value );
									m.End();
								}
							}
							else
								vehicle.LeftTurnSignal = value;
						}

						if( keyDown.Key == gameMode.KeyRightTurnSignal1 || keyDown.Key == gameMode.KeyRightTurnSignal2 )
						{
							var value = vehicle.RightTurnSignal.Value > 0.5f ? 0.0f : 1.0f;

							if( NetworkIsClient )
							{
								var m = BeginNetworkMessageToServer( "RightTurnSignal" );
								if( m != null )
								{
									m.Writer.Write( value );
									m.End();
								}
							}
							else
								vehicle.RightTurnSignal = value;
						}
					}

					//mouse down
					var mouseDown = message as InputMessageMouseButtonDown;
					if( mouseDown != null && ( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right ) )
					{
						var weapons = vehicle.GetComponents<Weapon>( checkChildren: true );
						if( weapons.Length != 0 )
						{
							var mode = mouseDown.Button == EMouseButtons.Left ? 1 : 2;
							firing[ mode ] = true;

							foreach( var weapon in weapons )
							{
								if( NetworkIsClient )
									weapon.FiringBeginClient( mode );
								else
									weapon.FiringBegin( mode, 0 );
							}
						}

						//will not work in network
						//if( vehicle.DynamicData?.Turrets != null )
						//{
						//	foreach( var turret in vehicle.DynamicData.Turrets )
						//	{
						//		var weapon = turret.Weapon;
						//		if( weapon != null )
						//		{
						//			var mode = mouseDown.Button == EMouseButtons.Left ? 1 : 2;
						//			firing[ mode ] = true;

						//			if( NetworkIsClient )
						//				weapon.FiringBeginClient( mode );
						//			else
						//				weapon.FiringBegin( mode, 0 );
						//		}
						//	}
						//}
					}

					//mouse up
					var mouseUp = message as InputMessageMouseButtonUp;
					if( mouseUp != null && mouseUp.Button == EMouseButtons.Left )
						firing[ 1 ] = false;
					if( mouseUp != null && mouseUp.Button == EMouseButtons.Right )
						firing[ 2 ] = false;


					////mouse move
					//var mouseMove = message as InputMessageMouseMove;
					//if( mouseMove != null && MouseRelativeMode )
					//{
					//	if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson )
					//	{
					//		if( !character.Sitting )
					//		{
					//			var sensitivity = new Vector2F( (float)GameMode.MouseSensitivity, (float)GameMode.MouseSensitivity ) * 2;
					//			var mouseOffset = mouseMove.Position.ToVector2F() * sensitivity;

					//			UpdateTurnToDirectionAndLookToToPosition( gameMode, mouseOffset );
					//		}
					//	}
					//}
				}
			}
		}

		void ObjectControlSimulationStep()
		{
			var vehicle = Vehicle;
			if( vehicle != null && InputEnabled )
			{
				//get control data

				float throttle = 0;
				float steering = 0;
				bool brake = false;

				var scene = ParentRoot as Scene;
				var gameMode = (GameMode)scene?.GetGameMode();

				if( gameMode != null && !gameMode.FreeCamera && ( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson || gameMode.GetCameraManagementOfCurrentObject() != null ) )
				{
					if( IsKeyPressed( gameMode.KeyForward1 ) || IsKeyPressed( gameMode.KeyForward2 ) )
						throttle += 1;
					if( IsKeyPressed( gameMode.KeyBackward1 ) || IsKeyPressed( gameMode.KeyBackward2 ) )
						throttle -= 1;

					if( IsKeyPressed( gameMode.KeyLeft1 ) || IsKeyPressed( gameMode.KeyLeft2 ) )
						steering -= 1;
					if( IsKeyPressed( gameMode.KeyRight1 ) || IsKeyPressed( gameMode.KeyRight2 ) )
						steering += 1;

					brake = IsKeyPressed( gameMode.KeyBrake1 ) || IsKeyPressed( gameMode.KeyBrake2 );

					var touchSlider = TouchSliders[ 0 ].ToVector2F();
					throttle -= touchSlider.Y;
					steering += touchSlider.X;

					MathEx.Clamp( ref throttle, -1, 1 );
					MathEx.Clamp( ref steering, -1, 1 );
				}

				if( NetworkIsClient )
				{
					//send data to the server

					//!!!!не нужно слать если не менялось

					var m = BeginNetworkMessageToServer( "UpdateObjectControlVehicle" );
					if( m != null )
					{
						var writer = m.Writer;

						writer.Write( new HalfType( throttle ) );
						writer.Write( new HalfType( brake ? 1.0 : 0.0 ) );
						writer.Write( new HalfType( steering ) );

						writer.Write( vehicle.RequiredLookToPosition.HasValue );
						if( vehicle.RequiredLookToPosition.HasValue )
							writer.Write( vehicle.RequiredLookToPosition.Value );

						m.End();
					}
				}
				else
				{
					//update the vehicle
					vehicle.Throttle = throttle;
					vehicle.Brake = brake ? 1 : 0;
					vehicle.HandBrake = 0;
					vehicle.Steering = steering;
					vehicle.SetMotorOn();
				}

				if( gameMode != null && !gameMode.FreeCamera && ( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson || gameMode.GetCameraManagementOfCurrentObject() != null ) )
				{
					//update turn to direction and weapon target
					UpdateTurnToDirectionAndLookToToPosition( gameMode );//, null );


					//!!!!network: don't send same for weapon if possible


					//update firing
					if( IsMouseButtonPressed( EMouseButtons.Left ) && firing[ 1 ] )
					{
						var weapons = vehicle.GetComponents<Weapon>( checkChildren: true );
						if( weapons.Length != 0 )
						{
							foreach( var weapon in weapons )
							{
								if( NetworkIsClient )
									weapon.FiringBeginClient( 1 );
								else
									weapon.FiringBegin( 1, 0 );
							}
						}

						//will not work in network
						//if( vehicle.DynamicData?.Turrets != null )
						//{
						//	foreach( var turret in vehicle.DynamicData.Turrets )
						//	{
						//		var weapon = turret.Weapon;
						//		if( weapon != null )
						//		{
						//			if( NetworkIsClient )
						//				weapon.FiringBeginClient( 1 );
						//			else
						//				weapon.FiringBegin( 1, 0 );
						//		}
						//	}
						//}
					}
					if( IsMouseButtonPressed( EMouseButtons.Right ) && firing[ 2 ] )
					{
						var weapons = vehicle.GetComponents<Weapon>( checkChildren: true );
						if( weapons.Length != 0 )
						{
							foreach( var weapon in weapons )
							{
								if( NetworkIsClient )
									weapon.FiringBeginClient( 2 );
								else
									weapon.FiringBegin( 2, 0 );
							}
						}

						//will not work in network
						//if( vehicle.DynamicData?.Turrets != null )
						//{
						//	foreach( var turret in vehicle.DynamicData.Turrets )
						//	{
						//		var weapon = turret.Weapon;
						//		if( weapon != null )
						//		{
						//			if( NetworkIsClient )
						//				weapon.FiringBeginClient( 2 );
						//			else
						//				weapon.FiringBegin( 2, 0 );
						//		}
						//	}
						//}
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

			var vehicle = Vehicle;
			if( vehicle != null )
			{
				//security check the object is controlled by the player
				var networkLogic = NetworkLogicUtility.GetNetworkLogic( vehicle );
				if( networkLogic != null && networkLogic.ServerGetObjectControlledByUser( client.User, true ) == vehicle )
				{
					if( message == "UpdateObjectControlVehicle" )
					{
						var throttle = (float)reader.ReadHalf();
						var brake = (float)reader.ReadHalf();
						var steering = (float)reader.ReadHalf();
						Vector3? lookToPosition = null;

						if( reader.ReadBoolean() )
							lookToPosition = reader.ReadVector3();

						if( !reader.Complete() )
							return false;

						MathEx.Clamp( ref throttle, -1, 1 );
						MathEx.Clamp( ref brake, 0, 1 );
						MathEx.Clamp( ref steering, -1, 1 );

						vehicle.Throttle = throttle;
						vehicle.Brake = brake;
						vehicle.HandBrake = 0;
						vehicle.Steering = steering;
						vehicle.LookToPosition( lookToPosition );
						vehicle.SetMotorOn();
					}
					else if( message == "RemoveObjectFromSeat" )
					{
						var seatIndex = reader.ReadVariableInt32();
						var objectNetworkID = (long)reader.ReadVariableUInt64();
						if( !reader.Complete() )
							return false;

						var obj = ParentRoot.HierarchyController.GetComponentByNetworkID( objectNetworkID ) as ObjectInSpace;
						if( obj != null )
						{
							vehicle.RemoveObjectFromSeat( seatIndex, true );

							networkLogic.ServerChangeObjectControlled( client.User, obj );

							//!!!!
							//if( !string.IsNullOrEmpty( reason ) )
							//{
							//}
						}
					}
					else if( message == "HeadlightsLow" )
					{
						var value = reader.ReadSingle();
						if( !reader.Complete() )
							return false;
						vehicle.HeadlightsLow = value;
						//reset high
						if( value != 0 )
							vehicle.HeadlightsHigh = 0;
					}
					else if( message == "HeadlightsHigh" )
					{
						var value = reader.ReadSingle();
						if( !reader.Complete() )
							return false;
						vehicle.HeadlightsHigh = value;
						//reset low
						if( value != 0 )
							vehicle.HeadlightsLow = 0;
					}
					else if( message == "LeftTurnSignal" )
					{
						var value = reader.ReadSingle();
						if( !reader.Complete() )
							return false;
						vehicle.LeftTurnSignal = value;
					}
					else if( message == "RightTurnSignal" )
					{
						var value = reader.ReadSingle();
						if( !reader.Complete() )
							return false;
						vehicle.RightTurnSignal = value;
					}
				}
			}

			return true;
		}
	}
}
