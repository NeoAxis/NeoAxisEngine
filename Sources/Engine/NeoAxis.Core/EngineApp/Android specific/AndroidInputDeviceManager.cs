//// Copyright (C) 2006-2011 NeoAxis Group Ltd.
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Diagnostics;
//using System.Runtime.InteropServices;

//namespace Engine
//{
//   /// <summary>
//   /// Describes singleton class for managing input devices
//   /// </summary>
//   internal class AndroidInputDeviceManager : InputDeviceManager
//   {
//      struct Wrapper
//      {
//         public const string library = "AndroidAppNativeWrapper";
//         public const CallingConvention convention = CallingConvention.Cdecl;
//      }

//      //struct AndroidAppNativeWrapper
//      //{
//      //   [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_FreeArray", CallingConvention = Wrapper.convention )]
//      //   public unsafe static extern void FreeArray( void* pArray );
//      //   [DllImport( Wrapper.library, EntryPoint = "AndroidAppNativeWrapper_DoShutDown", CallingConvention = Wrapper.convention )]
//      //   public unsafe static extern void DoShutDown();
//      //}

//      //

//      internal AndroidInputDeviceManager()
//      {
//      }

//      unsafe internal override bool OnInit()
//      {
//         if( !CreateDevices() )
//            return false;

//         return true;
//      }

//      internal unsafe override void OnShutdown()
//      {
//         //AndroidAppNativeWrapper.DoShutDown();
//      }

//      public new static AndroidInputDeviceManager Instance
//      {
//         get { return (AndroidInputDeviceManager)InputDeviceManager.Instance; }
//      }

//      unsafe bool CreateDevices()
//      {
//         //int joystickCount;
//         //if(!MacAppNativeWrapper.ActivateJoysticks(out joystickCount))
//         //    return false;

//         //for(int i=0; i<joystickCount; i++)
//         //{
//         //    char* name;

//         //    MacAppNativeWrapper.GetJoystickName(i, out name);
//         //    string deviceName = Marshal.PtrToStringAnsi((IntPtr) name);

//         //    MacAppNativeWrapper.FreeArray((void*)name);

//         //    MacOSXJoystickInputDevice joystick = new MacOSXJoystickInputDevice(deviceName, i);

//         //    if(!joystick.Init())
//         //    {
//         //        joystick.CallOnShutdown();
//         //        return true;
//         //    }

//         //    Instance.RegisterDevice(joystick);
//         //}

//         return true;
//      }

//      unsafe static void JoystickMessageEvent( int messageType, int parameterA, int parameterB )
//      {
//      }
//   }
//}

