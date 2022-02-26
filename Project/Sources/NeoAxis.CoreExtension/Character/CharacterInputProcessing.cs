// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
		SphericalDirection lookDirection;
		bool[] firing = new bool[ 3 ];

		//

		[Browsable( false )]
		public SphericalDirection LookDirection
		{
			get { return lookDirection; }
			set { lookDirection = value; }
		}

		[Browsable( false )]
		public Character Character
		{
			get { return Parent as Character; }
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
						if( Character.JumpSupport && keyDown.Key == EKeys.Space )
							Character.TryJump();

						//drop item
						if( keyDown.Key == EKeys.T )
						{
							var item = Character.ItemGetEnabledFirst();
							if( item != null )
							{
								Transform newTransform = null;

								var obj = item as ObjectInSpace;
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
								weapon.FiringBegin( mode );
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
						var mouseOffset = mouseMove.Position;

						var fpsCamera = gameMode.UseBuiltInCamera.Value == GameMode.BuiltInCameraEnum.FirstPerson;

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
				character.SetTurnToDirection( lookDirection, true );

				//firing
				if( IsMouseButtonPressed( EMouseButtons.Left ) && firing[ 1 ] )
				{
					var item = Character.ItemGetEnabledFirst();
					if( item != null )
					{
						var weapon = item as Weapon;
						if( weapon != null )
							weapon.FiringBegin( 1 );
					}
				}
				if( IsMouseButtonPressed( EMouseButtons.Right ) && firing[ 2 ] )
				{
					var item = Character.ItemGetEnabledFirst();
					if( item != null )
					{
						var weapon = item as Weapon;
						if( weapon != null )
							weapon.FiringBegin( 2 );
					}
				}

			}
		}

	}
}
