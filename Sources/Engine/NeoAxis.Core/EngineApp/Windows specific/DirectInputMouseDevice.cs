#if !ANDROID && !IOS && !WEB
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using NeoAxis;

namespace DirectInput
{
	class DirectInputMouseDevice
	{
		static DirectInputMouseDevice instance;

		unsafe IDirectInput* directInput;
		bool needReleaseDirectInput;

		unsafe IDirectInputDevice8* mouseDevice;

		//!!!!
		IntPtr acquiredWindowHandle;

		///////////////////////////////////////////

		public struct State
		{
			Vector3I position;

			internal State( Vector3I position )
			{
				this.position = position;
			}

			public Vector3I Position
			{
				get { return position; }
			}
		};

		///////////////////////////////////////////

		unsafe public static bool Init( IntPtr windowHandle, IDirectInput* alreadyCreatedDirectInput )
		{
			if( instance != null )
			{
				Log.Fatal( "SystemMouseDevice.Init: instance != null." );
				return false;
			}

			NativeUtility.PreloadLibrary( "NeoAxisCoreNative" );

			instance = new DirectInputMouseDevice();
			if( !instance.InitInternal( windowHandle, alreadyCreatedDirectInput ) )
			{
				Shutdown();
				return false;
			}
			return true;
		}

		public static void Shutdown()
		{
			if( instance != null )
			{
				instance.ShutdownInternal();
				instance = null;
			}
		}

		public static DirectInputMouseDevice Instance
		{
			get { return instance; }
		}

		unsafe bool InitInternal( IntPtr windowHandle, IDirectInput* alreadyCreatedDirectInput )
		{
			int hr;

			if( alreadyCreatedDirectInput == null )
			{
				void*/*IDirectInput*/ directInputTemp;

				GUID iidDIrectInput = DInput.IID_IDirectInput8W;
				hr = DInput.DirectInput8Create( ref iidDIrectInput, out directInputTemp );
				if( DirectInput.Wrapper.FAILED( hr ) )
				{
					Log.Info( "SystemMouseDevice: DirectInput: DirectInput8Create failed." );
					return false;
				}

				directInput = (IDirectInput*)directInputTemp;
				needReleaseDirectInput = true;
			}
			else
			{
				directInput = alreadyCreatedDirectInput;
			}

			void*/*IDirectInputDevice8*/ mouseDeviceTemp;
			GUID guidSysMouse = DInput.GUID_SysMouse;

			hr = IDirectInput.CreateDevice( directInput, ref guidSysMouse,
				out mouseDeviceTemp, null );

			if( Wrapper.FAILED( hr ) )
			{
				Log.Info( "SystemMouseDevice: DirectInput: CreateDevice failed." );
				return false;
			}
			mouseDevice = (IDirectInputDevice8*)mouseDeviceTemp;

			hr = IDirectInputDevice8.SetDataFormat( mouseDevice, DInput.Get_c_dfDIMouse() );
			if( Wrapper.FAILED( hr ) )
			{
				Log.Info( "SystemMouseDevice: DirectInput: SetDataFormat failed." );
				return false;
			}

			//!!!!для Game.exe включать
			//if( windowHandle != IntPtr.Zero )
			//	Acquire( windowHandle );

			return true;
		}

		unsafe void ShutdownInternal()
		{
			if( mouseDevice != null )
			{
				Unacquire();
				IDirectInputDevice8.Release( mouseDevice );
				mouseDevice = null;
			}

			if( directInput != null )
			{
				if( needReleaseDirectInput )
					IDirectInput.Release( directInput );
				directInput = null;
			}
		}

		public State GetState()
		{
			unsafe
			{
				if( mouseDevice == null )
					return new State( Vector3I.Zero );

				DIMOUSESTATE diState = new DIMOUSESTATE();

				int hr;

				hr = IDirectInputDevice8.GetDeviceState( mouseDevice,
					(uint)sizeof( DIMOUSESTATE ), &diState );

				if( Wrapper.FAILED( hr ) )
				{
					hr = IDirectInputDevice8.Acquire( mouseDevice );
					while( hr == DInput.GetHRESULT_DIERR_INPUTLOST() )
						hr = IDirectInputDevice8.Acquire( mouseDevice );

					return new State( Vector3I.Zero );
				}

				return new State( new Vector3I( diState.lX, diState.lY, diState.lZ ) );
			}
		}

		public unsafe bool Acquire( IntPtr windowHandle )
		{
			if( mouseDevice == null )
				return false;

			acquiredWindowHandle = windowHandle;

			int hr = IDirectInputDevice8.SetCooperativeLevel( mouseDevice, windowHandle, DInput.DISCL_NONEXCLUSIVE | DInput.DISCL_FOREGROUND );
			if( (uint)hr == DInput.DIERR_UNSUPPORTED )
			{
				Log.Info( "SystemMouseDevice: DirectInput: SetCooperativeLevel returned " +
					"DIERR_UNSUPPORTED. For security reasons, background exclusive " +
					"mouse access is not allowed." );
				return false;
			}

			//if( Wrapper.FAILED( hr ) )
			//{
			//	Log.Info( "SystemMouseDevice: DirectInput: SetCooperativeLevel failed." );
			//	return false;
			//}

			//Buffer mode

			//DIPROPDWORD dipdw;
			//dipdw.diph.dwSize       = sizeof(DIPROPDWORD);
			//dipdw.diph.dwHeaderSize = sizeof(DIPROPHEADER);
			//dipdw.diph.dwObj        = 0;
			//dipdw.diph.dwHow        = DIPH_DEVICE;
			//dipdw.dwData            = DINPUT_BUFFER_SIZE; // Arbitary buffer size

			//if( FAILED( hr = pMouse->SetProperty( DIPROP_BUFFERSIZE, &dipdw.diph ) ) )
			//   return false;

			//Buffer mode END

			//check error maybe
			IDirectInputDevice8.Acquire( mouseDevice );

			return true;
		}

		public unsafe void Unacquire()
		{
			if( acquiredWindowHandle != IntPtr.Zero )
			{
				if( mouseDevice != null )
					IDirectInputDevice8.Unacquire( mouseDevice );
				acquiredWindowHandle = IntPtr.Zero;
			}
		}

		public bool IsWindowAcquired
		{
			get { return acquiredWindowHandle != IntPtr.Zero; }
		}

	}
}
#endif