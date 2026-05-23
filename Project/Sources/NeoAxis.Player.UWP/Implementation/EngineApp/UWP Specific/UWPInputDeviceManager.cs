// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
//using DirectInput;
//using XInputNativeWrapper;
using Windows.Gaming.Input;

namespace NeoAxis
{
	/// <summary>
	/// Describes singleton class for managing input devices
	/// </summary>
	internal class UWPInputDeviceManager : InputDeviceManager
	{
		IntPtr windowHandle;

		//bool haveXInput;

		//

		internal UWPInputDeviceManager( IntPtr windowHandle )
		{
			this.windowHandle = windowHandle;
		}

		unsafe protected override bool OnInit()
		{
			//NativeUtility.PreloadLibrary( "NeoAxisCoreNative" );

			try
			{
				//!!!!impl
				// https://docs.microsoft.com/en-us/uwp/api/windows.gaming.input

				//!!!!impl
				Gamepad.GamepadAdded += Gamepad_GamepadAdded;
				Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;

				//if( !CreateDevices() )
				//	return false;
			}
			catch
			{
				return false;
			}

			return true;
		}

		private void Gamepad_GamepadAdded( object sender, Gamepad e )
		{
		}

		private void Gamepad_GamepadRemoved( object sender, Gamepad e )
		{
		}

		protected unsafe override void OnShutdown()
		{
			//if( directInput != null )
			//{
			//	IDirectInput.Release( directInput );
			//	directInput = null;
			//}
		}

		public new static UWPInputDeviceManager Instance
		{
			get { return (UWPInputDeviceManager)InputDeviceManager.Instance; }
		}


		//unsafe bool CreateDevices()
		//{
		//	// check for XInput presense
		//	haveXInput = XInput.IsXInputPresent();

		//	if( haveXInput )
		//	{
		//		XINPUT_STATE state = new XINPUT_STATE();

		//		for( int n = 0; n < XInput.MaxControllers; n++ )
		//		{
		//			int result = XInput.GetState( n, ref state );
		//			if( !XInputNativeWrapper.Wrapper.FAILED( result ) )
		//			{
		//				string name = string.Format( "XBox Controller {0}", n );

		//				WindowsXBoxGamepad device = new WindowsXBoxGamepad( name, n );
		//				if( !device.Init() )
		//				{
		//					device.CallOnShutdown();
		//					continue;
		//				}

		//				RegisterDevice( device );
		//			}
		//		}
		//	}

		//	return true;
		//}

		//static unsafe bool EnumDevicesHandler( IntPtr /*DIDEVICEINSTANCE*/ lpddi, void* pvRef )
		//{
		//	DIDEVICEINSTANCE* deviceInstance = (DIDEVICEINSTANCE*)lpddi.ToPointer();

		//	//ignore XInput devices
		//	if( Instance.haveXInput && DInput.IsXInputDevice( ref deviceInstance->guidProduct ) )
		//		return true; //continue

		//	if( ( deviceInstance->dwDevType & DInput.DI8DEVTYPE_JOYSTICK ) != 0 )
		//	{
		//		string deviceName = new string( deviceInstance->tszInstanceName );

		//		DirectInputJoystickInputDevice joystick = new DirectInputJoystickInputDevice(
		//			deviceName, deviceInstance->guidInstance );

		//		if( !joystick.Init() )
		//		{
		//			joystick.CallOnShutdown();
		//			return true;
		//		}

		//		Instance.RegisterDevice( joystick );
		//	}

		//	return true; //continue
		//}
	}
}