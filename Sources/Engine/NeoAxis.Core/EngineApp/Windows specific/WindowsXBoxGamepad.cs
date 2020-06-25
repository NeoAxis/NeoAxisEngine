// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if WINDOWS || UWP
using System;
using System.Collections.Generic;
using System.Text;
using XInputNativeWrapper;

namespace NeoAxis.Input
{
	internal class WindowsXBoxGamepad : JoystickInputDevice
	{
		internal int controllerIndex;

		//

		internal WindowsXBoxGamepad( string name, int controllerIndex )
			: base( name )
		{
			this.controllerIndex = controllerIndex;
		}

		internal bool Init()
		{
			//buttons
			Button[] buttons = new Button[ 10 ];
			buttons[ 0 ] = new Button( JoystickButtons.XBox360_A, 0 );
			buttons[ 1 ] = new Button( JoystickButtons.XBox360_B, 1 );
			buttons[ 2 ] = new Button( JoystickButtons.XBox360_X, 2 );
			buttons[ 3 ] = new Button( JoystickButtons.XBox360_Y, 3 );
			buttons[ 4 ] = new Button( JoystickButtons.XBox360_LeftShoulder, 4 );
			buttons[ 5 ] = new Button( JoystickButtons.XBox360_RightShoulder, 5 );
			buttons[ 6 ] = new Button( JoystickButtons.XBox360_Back, 6 );
			buttons[ 7 ] = new Button( JoystickButtons.XBox360_Start, 7 );
			buttons[ 8 ] = new Button( JoystickButtons.XBox360_LeftThumbstick, 8 );
			buttons[ 9 ] = new Button( JoystickButtons.XBox360_RightThumbstick, 9 );

			//axes
			Axis[] axes = new Axis[ 6 ];

			for( int n = 0; n < 6; n++ )
			{
				JoystickInputDevice.Axis axis = null;

				switch( n )
				{
				case 0: // left thumb x
					axis = new JoystickInputDevice.Axis(
						JoystickAxes.XBox360_LeftThumbstickX, new RangeF( -1, 1 ), false );
					break;

				case 1: // left thumb y
					axis = new JoystickInputDevice.Axis(
						JoystickAxes.XBox360_LeftThumbstickY, new RangeF( -1, 1 ), false );
					break;

				case 2:	// right thumb x
					axis = new JoystickInputDevice.Axis(
						JoystickAxes.XBox360_RightThumbstickX, new RangeF( -1, 1 ), false );
					break;

				case 3: // right thumb y
					axis = new JoystickInputDevice.Axis(
						JoystickAxes.XBox360_RightThumbstickY, new RangeF( -1, 1 ), false );
					break;

				case 4: // left trigger
					axis = new JoystickInputDevice.Axis(
						JoystickAxes.XBox360_LeftTrigger, new RangeF( 0, 1 ), false );
					break;

				case 5: // right trigger
					axis = new JoystickInputDevice.Axis(
						JoystickAxes.XBox360_RightTrigger, new RangeF( 0, 1 ), false );
					break;
				}

				axes[ n ] = axis;
			}

			//povs
			POV[] povs = new POV[ 1 ];
			povs[ 0 ] = new JoystickInputDevice.POV( JoystickPOVs.POV1 );

			//sliders
			Slider[] sliders = new Slider[ 0 ];

			//forceFeedbackController
			WindowsXBoxForceFeedbackController forceFeedbackController =
				new WindowsXBoxForceFeedbackController( this );

			//initialize data
			InitDeviceData( buttons, axes, povs, sliders, forceFeedbackController );

			return true;
		}

		protected override void OnShutdown()
		{
		}

		protected override void OnUpdateState()
		{
			XINPUT_STATE state = new XINPUT_STATE();

			int result = XInput.GetState( controllerIndex, ref state );
			if( XInputNativeWrapper.Wrapper.FAILED( result ) )
				return;

			////////////////////////////////////////

			if( state.Gamepad.sThumbLX < XInput.XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE &&
				state.Gamepad.sThumbLX > -XInput.XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE )
			{
				state.Gamepad.sThumbLX = 0;
			}

			if( state.Gamepad.sThumbLY < XInput.XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE &&
				state.Gamepad.sThumbLY > -XInput.XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE )
			{
				state.Gamepad.sThumbLY = 0;
			}

			if( state.Gamepad.sThumbRX < XInput.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE &&
				 state.Gamepad.sThumbRX > -XInput.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE )
			{
				state.Gamepad.sThumbRX = 0;
			}

			if( state.Gamepad.sThumbRY < XInput.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE &&
				state.Gamepad.sThumbRY > -XInput.XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE )
			{
				state.Gamepad.sThumbRY = 0;
			}

			if( state.Gamepad.bLeftTrigger < XInput.XINPUT_GAMEPAD_TRIGGER_THRESHOLD )
			{
				state.Gamepad.bLeftTrigger = 0;
			}

			if( state.Gamepad.bRightTrigger < XInput.XINPUT_GAMEPAD_TRIGGER_THRESHOLD )
			{
				state.Gamepad.bRightTrigger = 0;
			}

			////////////////////////////////////////

			JoystickPOVDirections direction = JoystickPOVDirections.Centered;

			if( ( state.Gamepad.wButtons & XInput.XINPUT_GAMEPAD_DPAD_DOWN ) != 0 )
				direction |= JoystickPOVDirections.South;
			if( ( state.Gamepad.wButtons & XInput.XINPUT_GAMEPAD_DPAD_UP ) != 0 )
				direction |= JoystickPOVDirections.North;
			if( ( state.Gamepad.wButtons & XInput.XINPUT_GAMEPAD_DPAD_RIGHT ) != 0 )
				direction |= JoystickPOVDirections.East;
			if( ( state.Gamepad.wButtons & XInput.XINPUT_GAMEPAD_DPAD_LEFT ) != 0 )
				direction |= JoystickPOVDirections.West;

			if( POVs[ 0 ].Value != direction )
			{
				POVs[ 0 ].Value = direction;

				InputDeviceManager.Instance.SendEvent(
					new JoystickPOVChangedEvent( this, POVs[ 0 ] ) );
			}

			////////////////////////////////////////

			for( int n = 0; n < 10; n++ )
			{
				uint bits = 0;

				switch( n )
				{
				case 0: bits = XInput.XINPUT_GAMEPAD_A; break;
				case 1: bits = XInput.XINPUT_GAMEPAD_B; break;
				case 2: bits = XInput.XINPUT_GAMEPAD_X; break;
				case 3: bits = XInput.XINPUT_GAMEPAD_Y; break;
				case 4: bits = XInput.XINPUT_GAMEPAD_LEFT_SHOULDER; break;
				case 5: bits = XInput.XINPUT_GAMEPAD_RIGHT_SHOULDER; break;
				case 6: bits = XInput.XINPUT_GAMEPAD_BACK; break;
				case 7: bits = XInput.XINPUT_GAMEPAD_START; break;
				case 8: bits = XInput.XINPUT_GAMEPAD_LEFT_THUMB; break;
				case 9: bits = XInput.XINPUT_GAMEPAD_RIGHT_THUMB; break;
				}

				bool pressed = ( state.Gamepad.wButtons & bits ) != 0;

				if( Buttons[ n ].Pressed != pressed )
				{
					Buttons[ n ].Pressed = pressed;

					if( pressed )
					{
						InputDeviceManager.Instance.SendEvent(
							new JoystickButtonDownEvent( this, Buttons[ n ] ) );
					}
					else
					{
						InputDeviceManager.Instance.SendEvent(
							new JoystickButtonUpEvent( this, Buttons[ n ] ) );
					}
				}
			}

			////////////////////////////////////////

			{
				float value = (float)state.Gamepad.sThumbLX;
				value /= ( value > 0 ) ? 32767.0f : 32768.0f;

				if( Axes[ 0 ].Value != value )
				{
					Axes[ 0 ].Value = value;
					InputDeviceManager.Instance.SendEvent(
						new JoystickAxisChangedEvent( this, Axes[ 0 ] ) );
				}
			}

			{
				float value = (float)state.Gamepad.sThumbLY;
				value /= ( value > 0 ) ? 32767.0f : 32768.0f;

				if( Axes[ 1 ].Value != value )
				{
					Axes[ 1 ].Value = value;
					InputDeviceManager.Instance.SendEvent(
						new JoystickAxisChangedEvent( this, Axes[ 1 ] ) );
				}
			}

			{
				float value = (float)state.Gamepad.sThumbRX;
				value /= ( value > 0 ) ? 32767.0f : 32768.0f;

				if( Axes[ 2 ].Value != value )
				{
					Axes[ 2 ].Value = value;
					InputDeviceManager.Instance.SendEvent(
						new JoystickAxisChangedEvent( this, Axes[ 2 ] ) );
				}
			}

			{
				float value = (float)state.Gamepad.sThumbRY;
				value /= ( value > 0 ) ? 32767.0f : 32768.0f;

				if( Axes[ 3 ].Value != value )
				{
					Axes[ 3 ].Value = value;
					InputDeviceManager.Instance.SendEvent(
						new JoystickAxisChangedEvent( this, Axes[ 3 ] ) );
				}
			}

			{
				float value = (float)state.Gamepad.bLeftTrigger / 255.0f;

				if( Axes[ 4 ].Value != value )
				{
					Axes[ 4 ].Value = value;
					InputDeviceManager.Instance.SendEvent(
						new JoystickAxisChangedEvent( this, Axes[ 4 ] ) );
				}
			}

			{
				float value = (float)state.Gamepad.bRightTrigger / 255.0f;

				if( Axes[ 5 ].Value != value )
				{
					Axes[ 5 ].Value = value;
					InputDeviceManager.Instance.SendEvent(
						new JoystickAxisChangedEvent( this, Axes[ 5 ] ) );
				}
			}

		}
	}
}
#endif