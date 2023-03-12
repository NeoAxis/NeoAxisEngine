// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if WINDOWS || UWP
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using DirectInput;
using XInputNativeWrapper;

namespace NeoAxis
{
	/// <summary>
	/// Describes singleton class for managing input devices
	/// </summary>
	internal class WindowsInputDeviceManager : InputDeviceManager
	{
		IntPtr windowHandle;
		unsafe IDirectInput* directInput;
		bool haveXInput;

		//

		internal WindowsInputDeviceManager( IntPtr windowHandle )
		{
			this.windowHandle = windowHandle;
		}

		unsafe internal override bool OnInit()
		{
			NativeUtility.PreloadLibrary( "NeoAxisCoreNative" );

			try
			{
				void* directInputTemp;
				GUID iidDirectInput = DInput.IID_IDirectInput8W;
				int hr = DInput.DirectInput8Create( ref iidDirectInput, out directInputTemp );
				if( global::DirectInput.Wrapper.FAILED( hr ) )
				{
					Log.Warning( "WindowsInputDeviceManager: DirectInput8Create failed." );
					return false;
				}
				directInput = (IDirectInput*)directInputTemp;

				if( !CreateDevices() )
					return false;
			}
			catch
			{
				return false;
			}

			return true;
		}

		internal unsafe override void OnShutdown()
		{
			if( directInput != null )
			{
				IDirectInput.Release( directInput );
				directInput = null;
			}
		}

		public new static WindowsInputDeviceManager Instance
		{
			get { return (WindowsInputDeviceManager)InputDeviceManager.Instance; }
		}

		public IntPtr WindowHandle
		{
			get { return windowHandle; }
		}

		public unsafe IDirectInput* DirectInput
		{
			get { return directInput; }
		}

		unsafe bool CreateDevices()
		{
			// check for XInput presense
			haveXInput = XInput.IsXInputPresent();

			if( haveXInput )
			{
				XINPUT_STATE state = new XINPUT_STATE();

				for( int n = 0; n < XInput.MaxControllers; n++ )
				{
					int result = XInput.GetState( n, ref state );
					if( !XInputNativeWrapper.Wrapper.FAILED( result ) )
					{
						string name = string.Format( "XBox Controller {0}", n );

						WindowsXBoxGamepad device = new WindowsXBoxGamepad( name, n );
						if( !device.Init() )
						{
							device.CallOnShutdown();
							continue;
						}

						RegisterDevice( device );
					}
				}
			}

			// check for DirectInput devices

			int hr = IDirectInput.EnumDevices( directInput, DInput.DI8DEVCLASS_GAMECTRL,
					EnumDevicesHandler, null, DInput.DIEDFL_ATTACHEDONLY );

			if( global::DirectInput.Wrapper.FAILED( hr ) )
			{
				Log.Warning( "WindowsInputDeviceManager: IDirectInput.EnumDevices failed ({0}).",
					DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
				return false;
			}

			return true;
		}

		static unsafe bool EnumDevicesHandler( IntPtr /*DIDEVICEINSTANCE*/ lpddi, void* pvRef )
		{
			DIDEVICEINSTANCE* deviceInstance = (DIDEVICEINSTANCE*)lpddi.ToPointer();

			//ignore XInput devices
			if( Instance.haveXInput && DInput.IsXInputDevice( ref deviceInstance->guidProduct ) )
				return true; //continue

			if( ( deviceInstance->dwDevType & DInput.DI8DEVTYPE_JOYSTICK ) != 0 )
			{
				string deviceName = new string( deviceInstance->tszInstanceName );

				DirectInputJoystickInputDevice joystick = new DirectInputJoystickInputDevice(
					deviceName, deviceInstance->guidInstance );

				if( !joystick.Init() )
				{
					joystick.CallOnShutdown();
					return true;
				}

				Instance.RegisterDevice( joystick );
			}

			return true; //continue
		}
	}
}
#endif