// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if WINDOWS || UWP
using System;
using System.Collections.Generic;
using System.Text;
using XInputNativeWrapper;

namespace NeoAxis
{
	internal class WindowsXBoxForceFeedbackController : ForceFeedbackController
	{
		internal WindowsXBoxForceFeedbackController( JoystickInputDevice joystick )
			: base( joystick )
		{
		}

		public override bool HaveRumble
		{
			get { return true; }
		}

		public override bool SetRumbleSpeed( float leftMotor, float rightMotor )
		{
			int controllerIndex = ( (WindowsXBoxGamepad)Device ).controllerIndex;

			XINPUT_VIBRATION vibration = new XINPUT_VIBRATION();
			vibration.wLeftMotorSpeed = (ushort)( leftMotor * 65535.0f );
			vibration.wRightMotorSpeed = (ushort)( rightMotor * 65535.0f );

			int hr = XInput.SetState( controllerIndex, ref vibration );
			if( XInputNativeWrapper.Wrapper.FAILED( hr ) )
			{
				Log.Warning( "WindowsXBox360ForceFeedbackController: Cannot " +
					"set vibration params for \"{0}\".", Device.Name );
				return false;
			}
			return true;
		}
	}
}
#endif