// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !ANDROID && !IOS && !WEB && !UWP
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
//using DirectInput;
//using XInputNativeWrapper;


//!!!!use from from 3.5 below


namespace NeoAxis
{
	/// <summary>
	/// Describes singleton class for managing input devices on macOS.
	/// </summary>
	internal class MacOSInputDeviceManager : InputDeviceManager
	{
		IntPtr windowHandle;

		//!!!!
		//unsafe IDirectInput* directInput;
		//bool haveXInput;

		//

		internal MacOSInputDeviceManager( IntPtr windowHandle )
		{
			this.windowHandle = windowHandle;
		}

		unsafe internal override bool OnInit()
		{
			NativeUtility.PreloadLibrary( "NeoAxisCoreNative" );

			//!!!!
			//try
			//{
			//	void* directInputTemp;
			//	GUID iidDirectInput = DInput.IID_IDirectInput8W;
			//	int hr = DInput.DirectInput8Create( ref iidDirectInput, out directInputTemp );
			//	if( global::DirectInput.Wrapper.FAILED( hr ) )
			//	{
			//		Log.Warning( "WindowsInputDeviceManager: DirectInput8Create failed." );
			//		return false;
			//	}
			//	directInput = (IDirectInput*)directInputTemp;

			//	if( !CreateDevices() )
			//		return false;
			//}
			//catch
			//{
			//	return false;
			//}

			return true;
		}

		internal unsafe override void OnShutdown()
		{
			//!!!!
			//if( directInput != null )
			//{
			//	IDirectInput.Release( directInput );
			//	directInput = null;
			//}
		}

		public new static WindowsInputDeviceManager Instance
		{
			get { return (WindowsInputDeviceManager)InputDeviceManager.Instance; }
		}

		public IntPtr WindowHandle
		{
			get { return windowHandle; }
		}

		//!!!!

		//public unsafe IDirectInput* DirectInput
		//{
		//	get { return directInput; }
		//}

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

		//	// check for DirectInput devices

		//	int hr = IDirectInput.EnumDevices( directInput, DInput.DI8DEVCLASS_GAMECTRL,
		//			EnumDevicesHandler, null, DInput.DIEDFL_ATTACHEDONLY );

		//	if( global::DirectInput.Wrapper.FAILED( hr ) )
		//	{
		//		Log.Warning( "WindowsInputDeviceManager: IDirectInput.EnumDevices failed ({0}).",
		//			DInput.GetOutString( DInput.DXGetErrorStringW( hr ) ) );
		//		return false;
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
#endif




//!!!!from 3.5:


//// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Diagnostics;
//using System.Runtime.InteropServices;

//namespace Engine
//{
//	/// <summary>
//	/// Describes singleton class for managing input devices
//	/// </summary>
//	internal class MacOSXInputDeviceManager : InputDeviceManager
//	{
//		///////////////////////////////////////////

//		struct Wrapper
//		{
//			public const string library = "MacAppNativeWrapper";
//			public const CallingConvention convention = CallingConvention.Cdecl;
//		}

//		struct MacAppNativeWrapper
//		{
//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_InitDeviceManager", CallingConvention = Wrapper.convention )]
//			[return: MarshalAs( UnmanagedType.U1 )]
//			public unsafe static extern bool InitDeviceManager( out int deviceCount, IntPtr* nativeObjects );

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_ShutdownDeviceManager", CallingConvention = Wrapper.convention )]
//			public unsafe static extern void ShutdownDeviceManager();

//			[DllImport( Wrapper.library, EntryPoint = "MacAppNativeWrapper_GetDeviceName", CallingConvention = Wrapper.convention )]
//			public unsafe static extern IntPtr GetDeviceName( IntPtr nativeObject );
//		}

//		///////////////////////////////////////////

//		public new static MacOSXInputDeviceManager Instance
//		{
//			get { return (MacOSXInputDeviceManager)InputDeviceManager.Instance; }
//		}

//		unsafe internal override bool OnInit()
//		{
//			int deviceCount;
//			IntPtr* nativeObjects = stackalloc IntPtr[ 1024 ];
//			if( !MacAppNativeWrapper.InitDeviceManager( out deviceCount, nativeObjects ) )
//				return false;

//			for( int n = 0; n < deviceCount; n++ )
//			{
//				IntPtr nativeObject = nativeObjects[ n ];

//				string deviceName = MacOSXPlatformFunctionality.MacAppNativeWrapper.GetOutString(
//					MacAppNativeWrapper.GetDeviceName( nativeObject ) );

//				MacOSXJoystickInputDevice joystick = new MacOSXJoystickInputDevice( deviceName, nativeObject );

//				if( !joystick.Init() )
//				{
//					joystick.CallOnShutdown();
//					return true;
//				}

//				RegisterDevice( joystick );
//			}

//			return true;
//		}

//		internal unsafe override void OnShutdown()
//		{
//			MacAppNativeWrapper.ShutdownDeviceManager();
//		}

//	}
//}

