// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// An object to process input from a player to a vehicle.
	/// </summary>
	[AddToResourcesWindow( @"Addons\Vehicle\Vehicle Input Processing", 22003 )]
	public class VehicleInputProcessing : InputProcessing
	{
		[Browsable( false )]
		public Vehicle Vehicle
		{
			get { return Parent as Vehicle; }
		}

		protected override void OnInputMessage( GameMode gameMode, InputMessage message )
		{
			base.OnInputMessage( gameMode, message );

			if( !gameMode.FreeCamera && InputEnabled )
			{
				//key down
				var keyDown = message as InputMessageKeyDown;
				if( keyDown != null )
				{
					if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson )
					{

						//!!!!hardcoded EKeys


						//exit from the vehicle
						if( keyDown.Key == EKeys.E )
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
							{
								var seatIndex = Vehicle.GetSeatIndexByObject( obj );
								if( seatIndex != -1 )
								{
									if( NetworkIsClient )
									{
										var writer = BeginNetworkMessageToServer( "RemoveObjectFromSeat" );
										if( writer != null )
										{
											writer.WriteVariableInt32( seatIndex );
											writer.WriteVariableUInt64( (ulong)obj.NetworkID );
											EndNetworkMessage();
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


							//var seat = Vehicle.GetComponent<VehicleSeat>();
							//if( seat != null )
							//{
							//	var character = seat.Character.Value;
							//	if( character != null )
							//	{
							//		if( NetworkIsClient )
							//		{
							//			var writer = BeginNetworkMessageToServer( "RemoveCharacterFromSeat" );
							//			if( writer != null )
							//			{
							//				writer.WriteVariableUInt64( (ulong)character.NetworkID );
							//				writer.WriteVariableUInt64( (ulong)seat.NetworkID );
							//				EndNetworkMessage();
							//			}
							//		}
							//		else
							//		{
							//			seat.RemoveCharacterFromSeat();
							//			gameMode.ObjectControlledByPlayer = ReferenceUtility.MakeRootReference( character );

							//			GameMode.PlayScreen?.ParentContainer?.Viewport?.NotifyInstantCameraMovement();
							//		}
							//	}
							//}
						}
					}
				}
			}
		}

		void UpdateObjectControl()
		{
			var vehicle = Vehicle;

			if( vehicle != null && InputEnabled )
			{
				//get control data

				double throttle = 0;
				if( IsKeyPressed( EKeys.W ) || IsKeyPressed( EKeys.Up ) || IsKeyPressed( EKeys.NumPad8 ) )
					throttle += 1;
				if( IsKeyPressed( EKeys.S ) || IsKeyPressed( EKeys.Down ) || IsKeyPressed( EKeys.NumPad2 ) )
					throttle -= 1;

				double steering = 0;
				if( IsKeyPressed( EKeys.A ) || IsKeyPressed( EKeys.Left ) || IsKeyPressed( EKeys.NumPad4 ) )
					steering -= 1;
				if( IsKeyPressed( EKeys.D ) || IsKeyPressed( EKeys.Right ) || IsKeyPressed( EKeys.NumPad6 ) )
					steering += 1;

				var brake = IsKeyPressed( EKeys.Space );


				if( NetworkIsClient )
				{
					//send data to the server

					//!!!!не нужно слать если не менялось

					var writer = BeginNetworkMessageToServer( "UpdateObjectControl" );
					if( writer != null )
					{
						writer.Write( (float)throttle );
						writer.Write( (float)( brake ? 1 : 0 ) );
						writer.Write( (float)steering );
						EndNetworkMessage();
					}
				}
				else
				{
					//update the vehicle
					vehicle.Throttle = throttle;
					vehicle.Brake = brake ? 1 : 0;
					vehicle.HandBrake = 0;
					vehicle.Steering = steering;
				}
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			if( NetworkIsSingle )
				UpdateObjectControl();
		}

		protected override void OnSimulationStepClient()
		{
			base.OnSimulationStepClient();

			UpdateObjectControl();
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
					if( message == "UpdateObjectControl" )
					{
						var throttle = reader.ReadSingle();
						var brake = reader.ReadSingle();
						var steering = reader.ReadSingle();
						if( !reader.Complete() )
							return false;

						MathEx.Clamp( ref throttle, -1, 1 );
						MathEx.Clamp( ref brake, 0, 1 );
						MathEx.Clamp( ref steering, -1, 1 );

						vehicle.Throttle = throttle;
						vehicle.Brake = brake;
						vehicle.HandBrake = 0;
						vehicle.Steering = steering;
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
				}
			}

			return true;
		}
	}
}
