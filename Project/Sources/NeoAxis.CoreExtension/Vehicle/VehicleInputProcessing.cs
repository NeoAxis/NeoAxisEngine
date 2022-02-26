// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// An object to process input from a player to a vehicle.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Vehicle Input Processing", -7997 )]
	public class VehicleInputProcessing : InputProcessing
	{
		[Browsable( false )]
		public Vehicle Vehicle
		{
			get { return Parent as Vehicle; }
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();
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
						//exit from the vehicle
						if( keyDown.Key == EKeys.E )
						{
							var seat = Vehicle.GetComponent<VehicleCharacterSeat>();
							if( seat != null )
							{
								var character = seat.Character.Value;
								if( character != null )
								{
									seat.RemoveCharacterFromSeat();
									gameMode.ObjectControlledByPlayer = ReferenceUtility.MakeRootReference( character );

									GameMode.PlayScreen?.ParentContainer?.Viewport?.NotifyInstantCameraMovement();
								}
							}
						}
					}
				}
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

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

				//update the vehicle
				vehicle.Throttle = throttle;
				vehicle.Brake = brake ? 1 : 0;
				vehicle.Steering = steering;
			}
		}
	}
}
