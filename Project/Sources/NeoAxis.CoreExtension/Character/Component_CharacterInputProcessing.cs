// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using NeoAxis.Editor;

namespace NeoAxis
{
	/// <summary>
	/// An object to process input from a player to a character.
	/// </summary>
	[AddToResourcesWindow( @"Base\3D\Character Input Processing", -8998 )]
	public class Component_CharacterInputProcessing : Component_InputProcessing
	{
		SphericalDirection lookDirection;
		bool firing;

		//

		[Browsable( false )]
		public Component_Character Character
		{
			get { return Parent as Component_Character; }
		}

		protected override void OnEnabledInHierarchyChanged()
		{
			base.OnEnabledInHierarchyChanged();

			if( EnabledInHierarchy )
			{
				var character = Character;
				if( character != null )
				{
					//get initial look direction
					lookDirection = SphericalDirection.FromVector( character.TransformV.Rotation.GetForward() );
				}
			}
		}

		protected override void OnInputMessage( UIControl playScreen, Component_GameMode gameMode, InputMessage message )
		{
			base.OnInputMessage( playScreen, gameMode, message );

			if( !gameMode.FreeCamera && InputEnabled )
			{
				//key down
				var keyDown = message as InputMessageKeyDown;
				if( keyDown != null )
				{
					if( gameMode.UseBuiltInCamera.Value == Component_GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == Component_GameMode.BuiltInCameraEnum.ThirdPerson )
					{
						//jump
						if( Character.JumpSupport && keyDown.Key == EKeys.Space )
							Character.TryJump();

						//drop item
						if( keyDown.Key == EKeys.T )
						{
							var item = Character.ItemGetEnabledFirst();
							if( item != null )
							{
								Transform newTransform = null;

								var obj = item as Component_ObjectInSpace;
								if( obj != null )
								{
									//it is simple implementation
									var scaleFactor = Character.GetScaleFactor();
									newTransform = new Transform( Character.TransformV.Position + new Vector3( 0, 0, 0.2 * scaleFactor - Character.PositionToGroundHeight * scaleFactor ), Character.TransformV.Rotation, obj.TransformV.Scale );
								}

								Character.ItemDrop( item, newTransform );
							}
						}
					}
				}

				//mouse down
				var mouseDown = message as InputMessageMouseButtonDown;
				if( mouseDown != null && mouseDown.Button == EMouseButtons.Left )
				{
					if( gameMode.UseBuiltInCamera.Value == Component_GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == Component_GameMode.BuiltInCameraEnum.ThirdPerson )
					{
						var item = Character.ItemGetEnabledFirst();
						if( item != null )
						{
							var weapon = item as Component_Weapon;
							if( weapon != null )
							{
								firing = true;
								weapon.FiringBegin();
							}
						}
					}
				}

				//mouse up
				var mouseUp = message as InputMessageMouseButtonUp;
				if( mouseUp != null && mouseUp.Button == EMouseButtons.Left )
					firing = false;

				//mouse move
				var mouseMove = message as InputMessageMouseMove;
				if( mouseMove != null && MouseRelativeMode )
				{
					if( gameMode.UseBuiltInCamera.Value == Component_GameMode.BuiltInCameraEnum.FirstPerson || gameMode.UseBuiltInCamera.Value == Component_GameMode.BuiltInCameraEnum.ThirdPerson )
					{
						var mouseOffset = mouseMove.Position;

						var fpsCamera = gameMode.UseBuiltInCamera.Value == Component_GameMode.BuiltInCameraEnum.FirstPerson;

						//!!!!
						Vector2 sens = new Vector2( 1, 1 ) * 2;
						//Vector2 sens = GameControlsManager.Instance.MouseSensitivity * 2;

						lookDirection.Horizontal -= mouseOffset.X * sens.X;
						lookDirection.Vertical -= mouseOffset.Y * sens.Y;

						double limit = fpsCamera ? 0.1 : Math.PI / 8;
						if( lookDirection.Vertical < -( Math.PI / 2 - limit ) )
							lookDirection.Vertical = -( Math.PI / 2 - limit );
						if( lookDirection.Vertical > ( Math.PI / 2 - limit ) )
							lookDirection.Vertical = ( Math.PI / 2 - limit );
					}
				}
			}
		}

		protected override void OnSimulationStep()
		{
			base.OnSimulationStep();

			var character = Character;
			if( character != null && InputEnabled )
			{
				//movement

				Vector2 localVector = Vector2.Zero;
				if( IsKeyPressed( EKeys.W ) || IsKeyPressed( EKeys.Up ) || IsKeyPressed( EKeys.NumPad8 ) )
					localVector.X += 1.0;
				if( IsKeyPressed( EKeys.S ) || IsKeyPressed( EKeys.Down ) || IsKeyPressed( EKeys.NumPad2 ) )
					localVector.X -= 1.0;
				if( IsKeyPressed( EKeys.A ) || IsKeyPressed( EKeys.Left ) || IsKeyPressed( EKeys.NumPad4 ) )
					localVector.Y += 1.0;
				if( IsKeyPressed( EKeys.D ) || IsKeyPressed( EKeys.Right ) || IsKeyPressed( EKeys.NumPad6 ) )
					localVector.Y -= 1.0;
				//localVector.X += Intellect.GetControlKeyStrength( GameControlKeys.Forward );
				//localVector.X -= Intellect.GetControlKeyStrength( GameControlKeys.Backward );
				//localVector.Y += Intellect.GetControlKeyStrength( GameControlKeys.Left );
				//localVector.Y -= Intellect.GetControlKeyStrength( GameControlKeys.Right );

				Vector2 vector = ( new Vector3( localVector.X, localVector.Y, 0 ) * character.GetTransform().Rotation ).ToVector2();
				if( vector != Vector2.Zero )
				{
					var length = vector.Length();
					if( length > 1 )
						vector /= length;
				}

				bool run = false;
				if( character.RunSupport )
					run = IsKeyPressed( EKeys.Shift );

				character.SetMoveVector( vector, run );
				character.SetLookToDirection( lookDirection );

				//firing
				if( IsMouseButtonPressed( EMouseButtons.Left ) && firing )
				{
					var item = Character.ItemGetEnabledFirst();
					if( item != null )
					{
						var weapon = item as Component_Weapon;
						if( weapon != null )
							weapon.FiringBegin();
					}
				}
			}
		}

	}
}
