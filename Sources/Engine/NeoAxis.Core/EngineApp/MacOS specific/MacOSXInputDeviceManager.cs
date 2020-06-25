//// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Diagnostics;
//using System.Runtime.InteropServices;

//namespace NeoAxis.Input
//{
//	/// <summary>
//	/// Describes singleton class for managing input devices
//	/// </summary>
//	internal class MacOSXInputDeviceManager : InputDeviceManager
//	{
//		///////////////////////////////////////////

//		struct Wrapper
//		{
//			public const string library = "NeoAxisCoreNative";
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

