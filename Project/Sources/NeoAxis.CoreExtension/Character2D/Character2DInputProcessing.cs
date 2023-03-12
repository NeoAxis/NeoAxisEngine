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

			if( !gameMode.FreeCamera && InputEnabled )
			{
				//key down
				var keyDown = message as InputMessageKeyDown;
				if( keyDown != null )
				{
					if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.None || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson )
					{
						//jump
						if( Character.JumpSupport && keyDown.Key == EKeys.Space )
						{
							if( NetworkIsClient )
							{
								BeginNetworkMessageToServer( "Jump" );
								EndNetworkMessage();
							}
							else
								Character.TryJump();
						}

						//drop item
						if( keyDown.Key == EKeys.T )
						{
							var item = Character.ItemGetEnabledFirst();
							if( item != null )
							{
								if( NetworkIsClient )
								{
									var component = item as Component;
									if( component != null )
									{
										var writer = Character.BeginNetworkMessageToServer( "ItemDrop" );
										if( writer != null )
										{
											writer.WriteVariableUInt64( (ulong)component.NetworkID );
											Character.EndNetworkMessage();
										}
									}
								}
								else
									Character.ItemDrop( item, true );
							}
						}
					}
				}

				//mouse down
				var mouseDown = message as InputMessageMouseButtonDown;
				if( mouseDown != null && mouseDown.Button == EMouseButtons.Left )
				{
					if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.None || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson )
					{
						var item = Character.ItemGetEnabledFirst();
						if( item != null )
						{
							var weapon = item as Weapon2D;
							if( weapon != null )
							{
								firing = true;

								if( NetworkIsClient )
								{
									weapon.BeginNetworkMessageToServer( "FiringBegin" );
									weapon.EndNetworkMessage();
								}
								else
									weapon.FiringBegin();
							}
						}
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

		void UpdateObjectControl()
		{
			var character = Character;
			if( character != null && InputEnabled )
			{
				double vector = 0;

				if( IsKeyPressed( EKeys.A ) || IsKeyPressed( EKeys.Left ) || IsKeyPressed( EKeys.NumPad4 ) )
					vector -= 1.0;
				if( IsKeyPressed( EKeys.D ) || IsKeyPressed( EKeys.Right ) || IsKeyPressed( EKeys.NumPad6 ) )
					vector += 1.0;

				bool run = false;
				if( character.RunSupport )
					run = IsKeyPressed( EKeys.Shift );

				//update lookDirection
				if( vector != 0 )
					lookDirection = new Vector2( vector > 0 ? 1 : -1, 0 );

				character.SetMoveVector( vector, run );
				character.SetLookToDirection( lookDirection );

				//send data to the server
				if( NetworkIsClient )
				{
					//!!!!no sense to send same values. firing too

					var writer = BeginNetworkMessageToServer( "UpdateObjectControl" );
					if( writer != null )
					{
						writer.Write( (float)vector );
						writer.Write( run );
						writer.Write( lookDirection.ToVector2F() );
						EndNetworkMessage();
					}
				}


				//firing
				if( IsMouseButtonPressed( EMouseButtons.Left ) && firing )
				{
					var item = Character.ItemGetEnabledFirst();
					if( item != null )
					{
						var weapon = item as Weapon2D;
						if( weapon != null )
						{
							if( NetworkIsClient )
							{
								weapon.BeginNetworkMessageToServer( "FiringBegin" );
								weapon.EndNetworkMessage();
							}
							else
								weapon.FiringBegin();
						}
					}
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

			var character = Character;
			if( character != null )
			{
				//security check the object is controlled by the player
				var networkLogic = NetworkLogicUtility.GetNetworkLogic( character );
				if( networkLogic != null && networkLogic.ServerGetObjectControlledByUser( client.User, true ) == character )
				{
					if( message == "UpdateObjectControl" )
					{
						var vector = reader.ReadSingle();
						var run = reader.ReadBoolean();
						var lookDirection = reader.ReadVector2F();
						if( !reader.Complete() )
							return false;

						MathEx.Clamp( ref vector, -1, 1 );
						if( lookDirection != Vector2.Zero )
							lookDirection.Normalize();

						character.SetMoveVector( vector, run );
						character.SetLookToDirection( lookDirection );
					}
					else if( message == "Jump" )
						character.TryJump();
				}
			}

			return true;
		}

	}
}
