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
		//!!!!было
		//SphericalDirection lookDirection;
		bool[] firing = new bool[ 3 ];

		//

		//!!!!было
		//[Browsable( false )]
		//public SphericalDirection LookDirection
		//{
		//	get { return lookDirection; }
		//	set { lookDirection = value; }
		//}

		[Browsable( false )]
		public Character Character
		{
			get { return Parent as Character; }
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			//!!!!было
			//if( EnabledInHierarchy )
			//{
			//	var character = Character;
			//	if( character != null )
			//	{
			//		//get initial look direction
			//		lookDirection = SphericalDirection.FromVector( character.TransformV.Rotation.GetForward() );
			//	}
			//}
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
						//jump
						var characterType = Character.CharacterType.Value;
						if( characterType != null && characterType.Jump && keyDown.Key == EKeys.Space )
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
				if( mouseDown != null && ( mouseDown.Button == EMouseButtons.Left || mouseDown.Button == EMouseButtons.Right ) )
				{
					if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson )
					{
						var item = Character.ItemGetEnabledFirst();
						if( item != null )
						{
							var weapon = item as Weapon;
							if( weapon != null )
							{
								var mode = mouseDown.Button == EMouseButtons.Left ? 1 : 2;

								firing[ mode ] = true;

								if( NetworkIsClient )
								{
									var writer = weapon.BeginNetworkMessageToServer( "FiringBegin" );
									if( writer != null )
									{
										writer.WriteVariableInt32( mode );
										weapon.EndNetworkMessage();
									}
								}
								else
									weapon.FiringBegin( mode, 0 );
							}
						}
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
					if( gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.ThirdPerson )
					{
						var mouseOffset = mouseMove.Position.ToVector2F();

						var fpsCamera = gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson;

						//!!!!
						Vector2F sens = new Vector2F( 1, 1 ) * 2;
						//Vector2 sens = GameControlsManager.Instance.MouseSensitivity * 2;

						var lookDirection = Character.CurrentTurnToDirection;

						lookDirection.Horizontal -= mouseOffset.X * sens.X;
						lookDirection.Vertical -= mouseOffset.Y * sens.Y;

						float limit = fpsCamera ? 0.1f : MathEx.PI / 8;
						if( lookDirection.Vertical < -( MathEx.PI / 2 - limit ) )
							lookDirection.Vertical = -( MathEx.PI / 2 - limit );
						if( lookDirection.Vertical > MathEx.PI / 2 - limit )
							lookDirection.Vertical = MathEx.PI / 2 - limit;


						//!!!!turn instantly optionally? property in this?

						Character.TurnToDirection( lookDirection, true );
					}
				}
			}
		}

		void UpdateObjectControl()
		{
			var character = Character;
			if( character != null && InputEnabled )
			{
				//movement

				Vector2F localVector = Vector2F.Zero;
				if( IsKeyPressed( EKeys.W ) || IsKeyPressed( EKeys.Up ) || IsKeyPressed( EKeys.NumPad8 ) )
					localVector.X += 1.0f;
				if( IsKeyPressed( EKeys.S ) || IsKeyPressed( EKeys.Down ) || IsKeyPressed( EKeys.NumPad2 ) )
					localVector.X -= 1.0f;
				if( IsKeyPressed( EKeys.A ) || IsKeyPressed( EKeys.Left ) || IsKeyPressed( EKeys.NumPad4 ) )
					localVector.Y += 1.0f;
				if( IsKeyPressed( EKeys.D ) || IsKeyPressed( EKeys.Right ) || IsKeyPressed( EKeys.NumPad6 ) )
					localVector.Y -= 1.0f;
				//localVector.X += Intellect.GetControlKeyStrength( GameControlKeys.Forward );
				//localVector.X -= Intellect.GetControlKeyStrength( GameControlKeys.Backward );
				//localVector.Y += Intellect.GetControlKeyStrength( GameControlKeys.Left );
				//localVector.Y -= Intellect.GetControlKeyStrength( GameControlKeys.Right );

				Vector2F vector = ( new Vector3F( localVector.X, localVector.Y, 0 ) * character.TransformV.Rotation ).ToVector2F();
				if( vector != Vector2.Zero )
				{
					var length = vector.Length();
					if( length > 1 )
						vector /= length;
				}

				bool run = false;
				var characterType = Character.CharacterType.Value;
				if( characterType != null && characterType.Run )
					run = IsKeyPressed( EKeys.Shift );

				character.SetMoveVector( vector, run );
				//!!!!было
				//character.SetTurnToDirection( lookDirection, true );

				//send data to the server
				if( NetworkIsClient )
				{
					//!!!!no sense to send same values. firing too

					var writer = BeginNetworkMessageToServer( "UpdateObjectControl" );
					if( writer != null )
					{
						writer.Write( vector );
						writer.Write( run );

						//!!!!impl check

						writer.Write( character.CurrentTurnToDirection );
						//.ToSphericalDirectionF() );
						//writer.Write( lookDirection.ToSphericalDirectionF() );

						EndNetworkMessage();
					}
				}

				//firing
				if( IsMouseButtonPressed( EMouseButtons.Left ) && firing[ 1 ] )
				{
					var item = Character.ItemGetEnabledFirst();
					if( item != null )
					{
						var weapon = item as Weapon;
						if( weapon != null )
						{
							if( NetworkIsClient )
							{
								var writer = weapon.BeginNetworkMessageToServer( "FiringBegin" );
								if( writer != null )
								{
									writer.WriteVariableInt32( 1 );
									weapon.EndNetworkMessage();
								}
							}
							else
								weapon.FiringBegin( 1, 0 );
						}
					}
				}
				if( IsMouseButtonPressed( EMouseButtons.Right ) && firing[ 2 ] )
				{
					var item = Character.ItemGetEnabledFirst();
					if( item != null )
					{
						var weapon = item as Weapon;
						if( weapon != null )
						{
							if( NetworkIsClient )
							{
								var writer = weapon.BeginNetworkMessageToServer( "FiringBegin" );
								if( writer != null )
								{
									writer.WriteVariableInt32( 2 );
									weapon.EndNetworkMessage();
								}
							}
							else
								weapon.FiringBegin( 2, 0 );
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
						var vector = reader.ReadVector2F();
						var run = reader.ReadBoolean();
						var lookDirection = reader.ReadSphericalDirectionF();
						if( !reader.Complete() )
							return false;

						if( vector != Vector2F.Zero )
							vector.Normalize();

						character.SetMoveVector( vector, run );
						character.TurnToDirection( lookDirection, true );
					}
					else if( message == "Jump" )
						character.TryJump();
				}
			}

			return true;
		}
	}
}
