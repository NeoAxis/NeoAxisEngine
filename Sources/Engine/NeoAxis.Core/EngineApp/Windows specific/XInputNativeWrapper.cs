// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if WINDOWS || UWP
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using NeoAxis;
using NeoAxis.Input;

namespace XInputNativeWrapper
{
	struct Wrapper
	{
		public const string library = "XInput1_3.dll";
		public const CallingConvention convention = CallingConvention.Winapi;

		public static bool FAILED( int result )
		{
			return result != 0;
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////

	[StructLayout( LayoutKind.Sequential )]
	struct XINPUT_GAMEPAD
	{
		public ushort wButtons;
		public byte bLeftTrigger;
		public byte bRightTrigger;
		public short sThumbLX;
		public short sThumbLY;
		public short sThumbRX;
		public short sThumbRY;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct XINPUT_STATE
	{
		public uint dwPacketNumber;
		public XINPUT_GAMEPAD Gamepad;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct XINPUT_VIBRATION
	{
		public ushort wLeftMotorSpeed;
		public ushort wRightMotorSpeed;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct XINPUT_CAPABILITIES
	{
		public byte Type;
		public byte SubType;
		public ushort Flags;
		public XINPUT_GAMEPAD Gamepad;
		public XINPUT_VIBRATION Vibration;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct XINPUT_BATTERY_INFORMATION
	{
		public byte BatteryType;
		public byte BatteryLevel;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct XINPUT_KEYSTROKE
	{
		public ushort VirtualKey;
		public char Unicode;
		public ushort Flags;
		public byte UserIndex;
		public byte HidCode;
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////

	struct XInput
	{
		public const int MaxControllers = 4;

		// Device types available in XINPUT_CAPABILITIES
		public const byte XINPUT_DEVTYPE_GAMEPAD = 0x01;

		// Device subtypes available in XINPUT_CAPABILITIES
		public const byte XINPUT_DEVSUBTYPE_GAMEPAD = 0x01;
		public const byte XINPUT_DEVSUBTYPE_WHEEL = 0x02;
		public const byte XINPUT_DEVSUBTYPE_ARCADE_STICK = 0x03;
		public const byte XINPUT_DEVSUBTYPE_FLIGHT_SICK = 0x04;
		public const byte XINPUT_DEVSUBTYPE_DANCE_PAD = 0x05;

		// Flags for XINPUT_CAPABILITIES
		public const ushort XINPUT_CAPS_VOICE_SUPPORTED = 0x0004;

		// Constants for gamepad buttons
		public const ushort XINPUT_GAMEPAD_DPAD_UP = 0x0001;
		public const ushort XINPUT_GAMEPAD_DPAD_DOWN = 0x0002;
		public const ushort XINPUT_GAMEPAD_DPAD_LEFT = 0x0004;
		public const ushort XINPUT_GAMEPAD_DPAD_RIGHT = 0x0008;
		public const ushort XINPUT_GAMEPAD_START = 0x0010;
		public const ushort XINPUT_GAMEPAD_BACK = 0x0020;
		public const ushort XINPUT_GAMEPAD_LEFT_THUMB = 0x0040;
		public const ushort XINPUT_GAMEPAD_RIGHT_THUMB = 0x0080;
		public const ushort XINPUT_GAMEPAD_LEFT_SHOULDER = 0x0100;
		public const ushort XINPUT_GAMEPAD_RIGHT_SHOULDER = 0x0200;
		public const ushort XINPUT_GAMEPAD_A = 0x1000;
		public const ushort XINPUT_GAMEPAD_B = 0x2000;
		public const ushort XINPUT_GAMEPAD_X = 0x4000;
		public const ushort XINPUT_GAMEPAD_Y = 0x8000;

		// Gamepad thresholds
		public const ushort XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE = 7849;
		public const ushort XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE = 8689;
		public const ushort XINPUT_GAMEPAD_TRIGGER_THRESHOLD = 30;

		// Flags to pass to XInputGetCapabilities
		public const uint XINPUT_FLAG_GAMEPAD = 0x00000001;

		// Devices that support batteries
		public const byte BATTERY_DEVTYPE_GAMEPAD = 0x00;
		public const byte BATTERY_DEVTYPE_HEADSET = 0x01;

		// Flags for battery status level
		public const byte BATTERY_TYPE_DISCONNECTED = 0x00;    // This device is not connected
		public const byte BATTERY_TYPE_WIRED = 0x01;    // Wired device, no battery
		public const byte BATTERY_TYPE_ALKALINE = 0x02;    // Alkaline battery source
		public const byte BATTERY_TYPE_NIMH = 0x03;    // Nickel Metal Hydride battery source
		public const byte BATTERY_TYPE_UNKNOWN = 0xFF;    // Cannot determine the battery type

		// These are only valid for wireless, connected devices, with known battery types
		// The amount of use time remaining depends on the type of device.
		public const byte BATTERY_LEVEL_EMPTY = 0x00;
		public const byte BATTERY_LEVEL_LOW = 0x01;
		public const byte BATTERY_LEVEL_MEDIUM = 0x02;
		public const byte BATTERY_LEVEL_FULL = 0x03;

		// User index definitions
		public const uint XUSER_MAX_COUNT = 4;
		public const uint XUSER_INDEX_ANY = 0x000000FF;

		// Codes returned for the gamepad keystroke

		public const ushort VK_PAD_A = 0x5800;
		public const ushort VK_PAD_B = 0x5801;
		public const ushort VK_PAD_X = 0x5802;
		public const ushort VK_PAD_Y = 0x5803;
		public const ushort VK_PAD_RSHOULDER = 0x5804;
		public const ushort VK_PAD_LSHOULDER = 0x5805;
		public const ushort VK_PAD_LTRIGGER = 0x5806;
		public const ushort VK_PAD_RTRIGGER = 0x5807;

		public const ushort VK_PAD_DPAD_UP = 0x5810;
		public const ushort VK_PAD_DPAD_DOWN = 0x5811;
		public const ushort VK_PAD_DPAD_LEFT = 0x5812;
		public const ushort VK_PAD_DPAD_RIGHT = 0x5813;
		public const ushort VK_PAD_START = 0x5814;
		public const ushort VK_PAD_BACK = 0x5815;
		public const ushort VK_PAD_LTHUMB_PRESS = 0x5816;
		public const ushort VK_PAD_RTHUMB_PRESS = 0x5817;

		public const ushort VK_PAD_LTHUMB_UP = 0x5820;
		public const ushort VK_PAD_LTHUMB_DOWN = 0x5821;
		public const ushort VK_PAD_LTHUMB_RIGHT = 0x5822;
		public const ushort VK_PAD_LTHUMB_LEFT = 0x5823;
		public const ushort VK_PAD_LTHUMB_UPLEFT = 0x5824;
		public const ushort VK_PAD_LTHUMB_UPRIGHT = 0x5825;
		public const ushort VK_PAD_LTHUMB_DOWNRIGHT = 0x5826;
		public const ushort VK_PAD_LTHUMB_DOWNLEFT = 0x5827;

		public const ushort VK_PAD_RTHUMB_UP = 0x5830;
		public const ushort VK_PAD_RTHUMB_DOWN = 0x5831;
		public const ushort VK_PAD_RTHUMB_RIGHT = 0x5832;
		public const ushort VK_PAD_RTHUMB_LEFT = 0x5833;
		public const ushort VK_PAD_RTHUMB_UPLEFT = 0x5834;
		public const ushort VK_PAD_RTHUMB_UPRIGHT = 0x5835;
		public const ushort VK_PAD_RTHUMB_DOWNRIGHT = 0x5836;
		public const ushort VK_PAD_RTHUMB_DOWNLEFT = 0x5837;

		// Flags used in XINPUT_KEYSTROKE
		public const ushort XINPUT_KEYSTROKE_KEYDOWN = 0x0001;
		public const ushort XINPUT_KEYSTROKE_KEYUP = 0x0002;
		public const ushort XINPUT_KEYSTROKE_REPEAT = 0x0004;

		//

		[DllImport( Wrapper.library, EntryPoint = "XInputGetState", CallingConvention = Wrapper.convention )]
		public static extern int GetState(
			int dwUserIndex,  // [in] Index of the gamer associated with the device
			ref XINPUT_STATE pState    // [out] Receives the current state
		);

		[DllImport( Wrapper.library, EntryPoint = "XInputSetState", CallingConvention = Wrapper.convention )]
		public static extern int SetState
		(
			int dwUserIndex,   // [in] Index of the gamer associated with the device
			ref XINPUT_VIBRATION pVibration // [in, out] The vibration information to send to the controller
		);

		[DllImport( Wrapper.library, EntryPoint = "XInputGetCapabilities", CallingConvention = Wrapper.convention )]
		public static extern int GetCapabilities
		(
			int dwUserIndex,   // [in] Index of the gamer associated with the device
			uint dwFlags,       // [in] Input flags that identify the device type
			ref XINPUT_CAPABILITIES pCapabilities  // [out] Receives the capabilities
		);

		[DllImport( Wrapper.library, EntryPoint = "XInputEnable", CallingConvention = Wrapper.convention )]
		public static extern void Enable
		(
			bool enable     // [in] Indicates whether xinput is enabled or disabled. 
		);

		[DllImport( Wrapper.library, EntryPoint = "XInputGetDSoundAudioDeviceGuids", CallingConvention = Wrapper.convention )]
		public static extern int GetDSoundAudioDeviceGuids
		(
			int dwUserIndex,          // [in] Index of the gamer associated with the device
			ref GUID pDSoundRenderGuid,    // [out] DSound device ID for render
			ref GUID pDSoundCaptureGuid    // [out] DSound device ID for capture
		);

		[DllImport( Wrapper.library, EntryPoint = "XInputGetBatteryInformation", CallingConvention = Wrapper.convention )]
		public static extern int GetBatteryInformation
		(
			int dwUserIndex,        // [in]  Index of the gamer associated with the device
			byte devType,            // [in]  Which device on this user index
			ref XINPUT_BATTERY_INFORMATION pBatteryInformation // [out] Contains the level and types of batteries
		);

		[DllImport( Wrapper.library, EntryPoint = "XInputGetKeystroke", CallingConvention = Wrapper.convention )]
		public static extern int GetKeystroke
		(
			int dwUserIndex,              // [in]  Index of the gamer associated with the device
			uint dwReserved,               // [in]  Reserved for future use
			ref XINPUT_KEYSTROKE pKeystroke    // [out] Pointer to an XINPUT_KEYSTROKE structure that receives an input event.
		);

		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode )]
		private static extern IntPtr LoadLibrary( string lpLibFileName );

		[DllImport( "kernel32.dll" )]
		private static extern bool FreeLibrary( IntPtr hModule );

		public static bool IsXInputPresent()
		{
			IntPtr pointer = XInput.LoadLibrary( Wrapper.library );
			if( pointer != IntPtr.Zero )
			{
				XInput.FreeLibrary( pointer );
				return true;
			}
			else
				return false;
		}
	}
}
#endif