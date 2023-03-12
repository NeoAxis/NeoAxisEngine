// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if WINDOWS || UWP
using System;
using System.Runtime.InteropServices;
using System.Security;
using NeoAxis;

namespace DirectInput
{
	struct Wrapper
	{
		public const string library = "NeoAxisCoreNative";
		public const CallingConvention convention = CallingConvention.Cdecl;

		public const int MAX_PATH = 260;   // windef.h MAX_PATH = 260
		//public const int MAXCPOINTSNUM = 8;	// dinput.h

		public static bool FAILED( int hr )
		{
			return hr < 0;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	//[StructLayout( LayoutKind.Sequential )]
	//struct DInputStructureSizes
	//{
	//   int sizeGUID;
	//   int sizeDIMOUSESTATE;

	//   //

	//   unsafe public void Init()
	//   {
	//      sizeGUID = sizeof( GUID );
	//      sizeDIMOUSESTATE = sizeof( DIMOUSESTATE );
	//   }
	//}

	////////////////////////////////////////////////////////////////////////////////////////////////

	[StructLayout( LayoutKind.Sequential )]
	struct DIDEVICEINSTANCE
	{
		public uint dwSize;
		public GUID guidInstance;
		public GUID guidProduct;
		public uint dwDevType;
		public unsafe fixed char tszInstanceName[ Wrapper.MAX_PATH ];
		public unsafe fixed char tszProductName[ Wrapper.MAX_PATH ];
		public GUID guidFFDriver;
		public ushort wUsagePage;
		public ushort wUsage;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	[StructLayout( LayoutKind.Sequential )]
	struct DIDEVICEOBJECTINSTANCE
	{
		public uint dwSize;
		public GUID guidType;
		public uint dwOfs;
		public uint dwType;
		public uint dwFlags;
		public unsafe fixed char tszName[ Wrapper.MAX_PATH ];
		public uint dwFFMaxForce;
		public uint dwFFForceResolution;
		public ushort wCollectionNumber;
		public ushort wDesignatorIndex;
		public ushort wUsagePage;
		public ushort wUsage;
		public uint dwDimension;
		public ushort wExponent;
		public ushort wReportId;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	[StructLayout( LayoutKind.Sequential )]
	struct DIDEVICEOBJECTDATA
	{
		public uint dwOfs;
		public uint dwData;
		public uint dwTimeStamp;
		public uint dwSequence;
		public IntPtr uAppData;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
	struct DIDEVCAPS
	{
		public uint dwSize;
		public uint dwFlags;
		public uint dwDevType;
		public uint dwAxes;
		public uint dwButtons;
		public uint dwPOVs;
		public uint dwFFSamplePeriod;
		public uint dwFFMinTimeResolution;
		public uint dwFirmwareRevision;
		public uint dwHardwareRevision;
		public uint dwFFDriverVersion;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	[StructLayout( LayoutKind.Sequential )]
	struct DIMOUSESTATE
	{
		public int lX;
		public int lY;
		public int lZ;
		public unsafe fixed byte rgbButtons[ 4 ];
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	//[StructLayout( LayoutKind.Sequential )]
	//struct DIJOYSTATE
	//{
	//   public int lX;
	//   public int lY;
	//   public int lZ;
	//   public int lRx;
	//   public int lRy;
	//   public int lRz;
	//   public unsafe fixed int rglSlider[ 2 ];
	//   public unsafe fixed uint rgdwPOV[ 4 ];
	//   public unsafe fixed byte rgbButtons[ 32 ];
	//}

	//[StructLayout( LayoutKind.Sequential )]
	//struct CPOINT
	//{
	//   public int lP;     // raw value
	//   public uint dwLog;  // logical_value / max_logical_value * 10000
	//}

	//[StructLayout( LayoutKind.Sequential )]
	//struct DIPROPCPOINTS
	//{
	//   public DIPROPHEADER diph;
	//   public uint dwCPointsNum;
	//   x;
	//   [MarshalAs( UnmanagedType.ByValArray, SizeConst = Wrapper.MAXCPOINTSNUM )]
	//   public CPOINT[] cp;
	//}

	[StructLayout( LayoutKind.Sequential )]
	struct DIPROPDWORD
	{
		public DIPROPHEADER diph;
		public uint dwData;
	}

	//[StructLayout( LayoutKind.Sequential )]
	//struct DIPROPGUIDANDPATH
	//{
	//   public DIPROPHEADER diph;
	//   public GUID guidClass;
	//   x;
	//   [MarshalAs( UnmanagedType.LPWStr, SizeConst = Wrapper.MAX_PATH )]
	//   public string wszPath;
	//}

	[StructLayout( LayoutKind.Sequential )]
	struct DIPROPHEADER
	{
		public uint dwSize;
		public uint dwHeaderSize;
		public uint dwObj;
		public uint dwHow;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct DIPROPPOINTER
	{
		public DIPROPHEADER diph;
		public IntPtr uData;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct DIPROPRANGE
	{
		public DIPROPHEADER diph;
		public int lMin;
		public int lMax;
	}

	//[StructLayout( LayoutKind.Sequential )]
	//struct DIPROPSTRING
	//{
	//   public DIPROPHEADER diph;
	//   x;
	//   [MarshalAs( UnmanagedType.LPWStr, SizeConst = Wrapper.MAX_PATH )]
	//   public string wsz;
	//}

	////////////////////////////////////////////////////////////////////////////////////////////////

	//[StructLayout( LayoutKind.Sequential )]
	//struct DIJOYSTATE2
	//{
	//   public int lX;
	//   public int lY;
	//   public int lZ;
	//   public int lRx;
	//   public int lRy;
	//   public int lRz;
	//   public unsafe fixed int rglSlider[ 2 ];
	//   public unsafe fixed uint rgdwPOV[ 4 ];
	//   public unsafe fixed byte rgbButtons[ 128 ];
	//   public int lVX;
	//   public int lVY;
	//   public int lVZ;
	//   public int lVRx;
	//   public int lVRy;
	//   public int lVRz;
	//   public unsafe fixed int rglVSlider[ 2 ];
	//   public int lAX;
	//   public int lAY;
	//   public int lAZ;
	//   public int lARx;
	//   public int lARy;
	//   public int lARz;
	//   public unsafe fixed int rglASlider[ 2 ];
	//   public int lFX;
	//   public int lFY;
	//   public int lFZ;
	//   public int lFRx;
	//   public int lFRy;
	//   public int lFRz;
	//   public unsafe fixed int rglFSlider[ 2 ];
	//}

	////////////////////////////////////////////////////////////////////////////////////////////////

	[StructLayout( LayoutKind.Sequential )]
	struct DIEFFECT
	{
		public uint dwSize;
		public uint dwFlags;
		public uint dwDuration;
		public uint dwSamplePeriod;
		public uint dwGain;
		public uint dwTriggerButton;
		public uint dwTriggerRepeatInterval;
		public uint cAxes;
		public unsafe uint* rgdwAxes;
		public unsafe int* rglDirection;
		public unsafe DIENVELOPE* lpEnvelope;
		public uint cbTypeSpecificParams;
		public unsafe void* lpvTypeSpecificParams;
		public uint dwStartDelay;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct DIEFFECTINFO		//  DIEFFECTINFOW
	{
		public uint dwSize;
		public GUID guid;
		public uint dwEffType;
		public uint dwStaticParams;
		public uint dwDynamicParams;
		public unsafe fixed char tszName[ Wrapper.MAX_PATH ];
	}

	//[StructLayout( LayoutKind.Sequential )]
	//struct DIEFFESCAPE
	//{
	//   public uint dwSize;
	//   public uint dwCommand;
	//   public unsafe void* /*LPVOID*/ lpvInBuffer;
	//   public uint cbInBuffer;
	//   public unsafe void* /*LPVOID*/ lpvOutBuffer;
	//   public uint cbOutBuffer;
	//}

	[StructLayout( LayoutKind.Sequential )]
	struct DIENVELOPE
	{
		public uint dwSize;
		public uint dwAttackLevel;
		public uint dwAttackTime;
		public uint dwFadeLevel;
		public uint dwFadeTime;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct DICONDITION
	{
		public int lOffset;
		public int lPositiveCoefficient;
		public int lNegativeCoefficient;
		public uint dwPositiveSaturation;
		public uint dwNegativeSaturation;
		public int lDeadBand;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct DICONSTANTFORCE
	{
		public int lMagnitude;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct DICUSTOMFORCE
	{
		public uint cChannels;
		public uint dwSamplePeriod;
		public uint cSamples;
		public unsafe int* rglForceData;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct DIPERIODIC
	{
		public uint dwMagnitude;
		public int lOffset;
		public uint dwPhase;
		public uint dwPeriod;
	}

	[StructLayout( LayoutKind.Sequential )]
	struct DIRAMPFORCE
	{
		public int lStart;
		public int lEnd;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	struct DInput
	{
		public const uint DIERR_UNSUPPORTED = 0x80004001;
		public const uint DIERR_INCOMPLETEEFFECT = 0x80040206;
		public const uint DIERR_INVALIDPARAM = 0x80070057;
		public const uint DIERR_NOTEXCLUSIVEACQUIRED = 0x80040205;
		public const uint DIERR_NOTINITIALIZED = 0x80070015;
		public const uint DIERR_INPUTLOST = 0x8007001E;

		public const uint DIEFT_ALL = 0x00000000;

		public const uint DISCL_EXCLUSIVE = 0x00000001;
		public const uint DISCL_NONEXCLUSIVE = 0x00000002;
		public const uint DISCL_FOREGROUND = 0x00000004;
		public const uint DISCL_BACKGROUND = 0x00000008;
		public const uint DISCL_NOWINKEY = 0x00000010;

		public const uint S_OK = 0x00000000;
		public const uint S_FALSE = 0x00000001;

		public const uint DI_OK = S_OK;
		public const uint DI_NOEFFECT = S_FALSE;
		public const uint DI_POLLEDDEVICE = 0x00000002;

		public const uint DI8DEVCLASS_ALL = 0;
		public const uint DI8DEVCLASS_DEVICE = 1;
		public const uint DI8DEVCLASS_KEYBOARD = 3;
		public const uint DI8DEVCLASS_POINTER = 2;
		public const uint DI8DEVCLASS_GAMECTRL = 4;

		public const uint DI8DEVTYPE_DEVICE = 0x11;
		public const uint DI8DEVTYPE_MOUSE = 0x12;
		public const uint DI8DEVTYPE_KEYBOARD = 0x13;
		public const uint DI8DEVTYPE_JOYSTICK = 0x14;
		public const uint DI8DEVTYPE_GAMEPAD = 0x15;
		public const uint DI8DEVTYPE_DRIVING = 0x16;
		public const uint DI8DEVTYPE_FLIGHT = 0x17;
		public const uint DI8DEVTYPE_1STPERSON = 0x18;
		public const uint DI8DEVTYPE_DEVICECTRL = 0x19;
		public const uint DI8DEVTYPE_SCREENPOINTER = 0x1A;
		public const uint DI8DEVTYPE_REMOTE = 0x1B;
		public const uint DI8DEVTYPE_SUPPLEMENTAL = 0x1C;

		public const uint DIEDFL_ALLDEVICES = 0x00000000;
		public const uint DIEDFL_ATTACHEDONLY = 0x00000001;
		public const uint DIEDFL_FORCEFEEDBACK = 0x00000100;
		public const uint DIEDFL_INCLUDEALIASES = 0x00010000;
		public const uint DIEDFL_INCLUDEPHANTOMS = 0x00020000;
		public const uint DIEDFL_INCLUDEHIDDEN = 0x00040000;

		public const uint DIDC_ATTACHED = 0x00000001;
		public const uint DIDC_POLLEDDEVICE = 0x00000002;
		public const uint DIDC_EMULATED = 0x00000004;
		public const uint DIDC_POLLEDDATAFORMAT = 0x00000008;
		public const uint DIDC_FORCEFEEDBACK = 0x00000100;
		public const uint DIDC_FFATTACK = 0x00000200;
		public const uint DIDC_FFFADE = 0x00000400;
		public const uint DIDC_SATURATION = 0x00000800;
		public const uint DIDC_POSNEGCOEFFICIENTS = 0x00001000;
		public const uint DIDC_POSNEGSATURATION = 0x00002000;
		public const uint DIDC_DEADBAND = 0x00004000;
		public const uint DIDC_STARTDELAY = 0x00008000;
		public const uint DIDC_ALIAS = 0x00010000;
		public const uint DIDC_PHANTOM = 0x00020000;
		public const uint DIDC_HIDDEN = 0x00040000;

		public const uint DIDFT_ALL = 0x00000000;
		public const uint DIDFT_RELAXIS = 0x00000001;
		public const uint DIDFT_ABSAXIS = 0x00000002;
		public const uint DIDFT_AXIS = 0x00000003;

		public const uint DIDFT_FFACTUATOR = 0x01000000;
		public const uint DIDFT_FFEFFECTTRIGGER = 0x02000000;

		public const uint DIDOI_FFACTUATOR = 0x00000001;
		public const uint DIDOI_FFEFFECTTRIGGER = 0x00000002;
		public const uint DIDOI_POLLED = 0x00008000;
		public const uint DIDOI_ASPECTPOSITION = 0x00000100;
		public const uint DIDOI_ASPECTVELOCITY = 0x00000200;
		public const uint DIDOI_ASPECTACCEL = 0x00000300;
		public const uint DIDOI_ASPECTFORCE = 0x00000400;
		public const uint DIDOI_ASPECTMASK = 0x00000F00;
		public const uint DIDOI_GUIDISUSAGE = 0x00010000;

		public const uint DIPH_DEVICE = 0;
		public const uint DIPH_BYOFFSET = 1;
		public const uint DIPH_BYID = 2;
		public const uint DIPH_BYUSAGE = 3;

		public const uint DIEFF_OBJECTIDS = 0x00000001;
		public const uint DIEFF_OBJECTOFFSETS = 0x00000002;
		public const uint DIEFF_CARTESIAN = 0x00000010;
		public const uint DIEFF_POLAR = 0x00000020;
		public const uint DIEFF_SPHERICAL = 0x00000040;

		public const uint DI_DEGREES = 100;
		public const uint DI_FFNOMINALMAX = 10000;
		public const uint DI_SECONDS = 1000000;

		public const uint DIEP_DURATION = 0x00000001;
		public const uint DIEP_SAMPLEPERIOD = 0x00000002;
		public const uint DIEP_GAIN = 0x00000004;
		public const uint DIEP_TRIGGERBUTTON = 0x00000008;
		public const uint DIEP_TRIGGERREPEATINTERVAL = 0x00000010;
		public const uint DIEP_AXES = 0x00000020;
		public const uint DIEP_DIRECTION = 0x00000040;
		public const uint DIEP_ENVELOPE = 0x00000080;
		public const uint DIEP_TYPESPECIFICPARAMS = 0x00000100;
		public const uint DIEP_STARTDELAY = 0x00000200;
		public const uint DIEP_ALLPARAMS_DX5 = 0x000001FF;
		public const uint DIEP_ALLPARAMS = 0x000003FF;
		public const uint DIEP_START = 0x20000000;
		public const uint DIEP_NORESTART = 0x40000000;
		public const uint DIEP_NODOWNLOAD = 0x80000000;
		public const uint DIEB_NOTRIGGER = 0xFFFFFFFF;

		public const uint INFINITE = 0xFFFFFFFF;

		public const int DIJOFS_X = 0;
		public const int DIJOFS_Y = 4;
		public const int DIJOFS_Z = 8;
		public const int DIJOFS_RX = 12;
		public const int DIJOFS_RY = 16;
		public const int DIJOFS_RZ = 20;
		public const int DIJOFS_SLIDER00 = 24;
		public const int DIJOFS_SLIDER01 = 28;
		public const int DIJOFS_SLIDER10 = 200;
		public const int DIJOFS_SLIDER11 = 204;
		public const int DIJOFS_SLIDER20 = 232;
		public const int DIJOFS_SLIDER21 = 236;
		public const int DIJOFS_SLIDER30 = 264;
		public const int DIJOFS_SLIDER31 = 268;
		public const int DIJOFS_POV0 = 32;
		public const int DIJOFS_POV1 = 36;
		public const int DIJOFS_POV2 = 40;
		public const int DIJOFS_POV3 = 44;
		public const int DIJOFS_BUTTON0 = 48;
		public const int DIJOFS_BUTTON128 = 176;

		//

		public static GUID IID_IDirectInput8W = new GUID(
			0xBF798031, 0x483A, 0x4DA2, 0xAA, 0x99, 0x5D, 0x64, 0xED, 0x36, 0x97, 0x00 );
		public static GUID GUID_SysMouse = new GUID(
			0x6F1D2B60, 0xD5A0, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00 );

		public static GUID GUID_XAxis = new GUID( 0xA36D02E0, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00 );
		public static GUID GUID_YAxis = new GUID( 0xA36D02E1, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00 );
		public static GUID GUID_ZAxis = new GUID( 0xA36D02E2, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00 );
		public static GUID GUID_RxAxis = new GUID( 0xA36D02F4, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00 );
		public static GUID GUID_RyAxis = new GUID( 0xA36D02F5, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00 );
		public static GUID GUID_RzAxis = new GUID( 0xA36D02E3, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00 );
		public static GUID GUID_Slider = new GUID( 0xA36D02E4, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00 );
		public static GUID GUID_Button = new GUID( 0xA36D02F0, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00 );
		public static GUID GUID_Key = new GUID( 0x55728220, 0xD33C, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00 );
		public static GUID GUID_POV = new GUID( 0xA36D02F2, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00 );
		public static GUID GUID_Unknown = new GUID( 0xA36D02F3, 0xC9F3, 0x11CF, 0xBF, 0xC7, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00 );

		public static GUID GUID_ConstantForce = new GUID( 0x13541C20, 0x8E33, 0x11D0, 0x9A, 0xD0, 0x00, 0xA0, 0xC9, 0xA0, 0x6E, 0x35 );
		public static GUID GUID_RampForce = new GUID( 0x13541C21, 0x8E33, 0x11D0, 0x9A, 0xD0, 0x00, 0xA0, 0xC9, 0xA0, 0x6E, 0x35 );
		public static GUID GUID_Square = new GUID( 0x13541C22, 0x8E33, 0x11D0, 0x9A, 0xD0, 0x00, 0xA0, 0xC9, 0xA0, 0x6E, 0x35 );
		public static GUID GUID_Sine = new GUID( 0x13541C23, 0x8E33, 0x11D0, 0x9A, 0xD0, 0x00, 0xA0, 0xC9, 0xA0, 0x6E, 0x35 );
		public static GUID GUID_Triangle = new GUID( 0x13541C24, 0x8E33, 0x11D0, 0x9A, 0xD0, 0x00, 0xA0, 0xC9, 0xA0, 0x6E, 0x35 );
		public static GUID GUID_SawtoothUp = new GUID( 0x13541C25, 0x8E33, 0x11D0, 0x9A, 0xD0, 0x00, 0xA0, 0xC9, 0xA0, 0x6E, 0x35 );
		public static GUID GUID_SawtoothDown = new GUID( 0x13541C26, 0x8E33, 0x11D0, 0x9A, 0xD0, 0x00, 0xA0, 0xC9, 0xA0, 0x6E, 0x35 );
		public static GUID GUID_Spring = new GUID( 0x13541C27, 0x8E33, 0x11D0, 0x9A, 0xD0, 0x00, 0xA0, 0xC9, 0xA0, 0x6E, 0x35 );
		public static GUID GUID_Damper = new GUID( 0x13541C28, 0x8E33, 0x11D0, 0x9A, 0xD0, 0x00, 0xA0, 0xC9, 0xA0, 0x6E, 0x35 );
		public static GUID GUID_Inertia = new GUID( 0x13541C29, 0x8E33, 0x11D0, 0x9A, 0xD0, 0x00, 0xA0, 0xC9, 0xA0, 0x6E, 0x35 );
		public static GUID GUID_Friction = new GUID( 0x13541C2A, 0x8E33, 0x11D0, 0x9A, 0xD0, 0x00, 0xA0, 0xC9, 0xA0, 0x6E, 0x35 );
		public static GUID GUID_CustomForce = new GUID( 0x13541C2B, 0x8E33, 0x11D0, 0x9A, 0xD0, 0x00, 0xA0, 0xC9, 0xA0, 0x6E, 0x35 );

		//

		[DllImport( Wrapper.library, EntryPoint = "DInput_FreeOutString", CallingConvention = Wrapper.convention )]
		public unsafe static extern void FreeOutString( IntPtr pointer );

		public static string GetOutString( IntPtr pointer )
		{
			if( pointer != IntPtr.Zero )
			{
				string result = Marshal.PtrToStringUni( pointer );
				FreeOutString( pointer );
				return result;
			}
			else
				return null;
		}

		//[DllImport( Wrapper.library, EntryPoint = "DInput_GetStructureSizes", CallingConvention = Wrapper.convention )]
		//public static extern void GetStructureSizes( out DInputStructureSizes sizes );

		[DllImport( Wrapper.library, EntryPoint = "DInput_DirectInput8Create", CallingConvention = Wrapper.convention )]
		public unsafe static extern int DirectInput8Create( ref GUID riidltf, out void* ppvOut );

		[DllImport( Wrapper.library, EntryPoint = "DInput_Get_c_dfDIMouse", CallingConvention = Wrapper.convention )]
		public unsafe static extern void* /*DIDATAFORMAT*/ Get_c_dfDIMouse();

		[DllImport( Wrapper.library, EntryPoint = "DInput_Get_c_dfDIJoystick", CallingConvention = Wrapper.convention )]
		public unsafe static extern void* /*DIDATAFORMAT*/ Get_c_dfDIJoystick();

		[DllImport( Wrapper.library, EntryPoint = "DInput_Get_c_dfDIJoystick2", CallingConvention = Wrapper.convention )]
		public unsafe static extern void* /*DIDATAFORMAT*/ Get_c_dfDIJoystick2();

		[DllImport( Wrapper.library, EntryPoint = "DInput_GetHRESULT_DIERR_NOTACQUIRED", CallingConvention = Wrapper.convention )]
		public static extern int GetHRESULT_DIERR_NOTACQUIRED();

		[DllImport( Wrapper.library, EntryPoint = "DInput_GetHRESULT_DIERR_NOTINITIALIZED", CallingConvention = Wrapper.convention )]
		public static extern int GetHRESULT_DIERR_NOTINITIALIZED();

		[DllImport( Wrapper.library, EntryPoint = "DInput_DXGetErrorStringW" )]
		public unsafe static extern IntPtr/*string*/ DXGetErrorStringW( int/*HRESULT*/ hr );

		[DllImport( Wrapper.library, EntryPoint = "DInput_IsXInputDevice", CallingConvention = Wrapper.convention )]
		[return: MarshalAs( UnmanagedType.U1 )]
		public static extern bool IsXInputDevice( ref GUID pGuidProductFromDirectInput );

		[DllImport( Wrapper.library, EntryPoint = "DInput_GetHRESULT_DIERR_INPUTLOST", CallingConvention = Wrapper.convention )]
		public static extern int GetHRESULT_DIERR_INPUTLOST();

		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_APPDATA", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_APPDATA();
		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_AUTOCENTER", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_AUTOCENTER();
		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_AXISMODE", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_AXISMODE();
		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_BUFFERSIZE", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_BUFFERSIZE();
		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_CALIBRATION", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_CALIBRATION();
		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_CALIBRATIONMODE", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_CALIBRATIONMODE();
		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_CPOINTS", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_CPOINTS();
		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_DEADZONE", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_DEADZONE();
		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_FFGAIN", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_FFGAIN();
		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_INSTANCENAME", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_INSTANCENAME();
		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_PRODUCTNAME", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_PRODUCTNAME();
		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_RANGE", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_RANGE();
		[DllImport( Wrapper.library, EntryPoint = "DInput_getDIPROP_SATURATION", CallingConvention = Wrapper.convention )]
		public unsafe static extern void*/*GUID*/ getDIPROP_SATURATION();


		//static public string GetErrorText(int hr)
		//{
		//    switch ((uint)hr)
		//    {
		//        case DIERR_INCOMPLETEEFFECT:
		//            return "The effect could not be downloaded because essential information " +
		//                "is missing.  For example, no axes have been associated with the: " +
		//                "effect, or no type-specific information has been created.";
		//        case DIERR_INVALIDPARAM:
		//            return "An invalid parameter was passed to the returning function, " +
		//                "or the object was not in a state that admitted the function" +
		//                "to be called.";
		//        case DIERR_NOTEXCLUSIVEACQUIRED:
		//            return "The operation cannot be performed unless the device is acquired " +
		//                "in DISCL_EXCLUSIVE mode.";
		//        case DIERR_NOTINITIALIZED:
		//            return "An object has not been initialized.";
		//        case DIERR_UNSUPPORTED:
		//            return "The function called is not supported at this time.";
		//        case DIERR_INPUTLOST:
		//            return "Access to the device has been lost. It must be re-acquired.";
		//        default:
		//            return "";
		//    }
		//}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	struct IDirectInput
	{
		[DllImport( Wrapper.library, EntryPoint = "NIDirectInput_Release", CallingConvention = Wrapper.convention )]
		public unsafe static extern uint Release( void* /*IDirectInput*/ _this );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInput_CreateDevice", CallingConvention = Wrapper.convention )]
		public unsafe static extern int CreateDevice( void* /*IDirectInput*/ _this,
			ref GUID rguid, out void* /*IDirectInputDevice*/ lplpDirectInputDevice, void* pUnkOuter );

		[UnmanagedFunctionPointer( CallingConvention.Winapi )]
		public unsafe delegate bool EnumDevicesCallback(
			IntPtr /*DIDEVICEINSTANCE*/ lpddi, void* pvRef );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInput_EnumDevices", CallingConvention = Wrapper.convention )]
		public unsafe static extern int EnumDevices( void* /*IDirectInput*/ _this,
			uint/*DeviceClassFilter*/ dwDevType, EnumDevicesCallback lpCallback, void* pvRef,
			uint/*DeviceEnumFlags*/ dwFlags );
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	struct IDirectInputDevice8
	{
		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_Release", CallingConvention = Wrapper.convention )]
		public unsafe static extern int Release( void* /*IDirectInputDevice8*/ _this );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_Acquire", CallingConvention = Wrapper.convention )]
		public unsafe static extern int Acquire( void* /*IDirectInputDevice8*/ _this );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_Unacquire", CallingConvention = Wrapper.convention )]
		public unsafe static extern int Unacquire( void* /*IDirectInputDevice8*/ _this );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_GetDeviceState", CallingConvention = Wrapper.convention )]
		public unsafe static extern int GetDeviceState( void* /*IDirectInputDevice8*/ _this,
			uint cbData, void* lpvData );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_SetDataFormat", CallingConvention = Wrapper.convention )]
		public unsafe static extern int SetDataFormat( void* /*IDirectInputDevice8*/ _this,
			void* /*DIDATAFORMAT*/ lpdf );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_GetDeviceData", CallingConvention = Wrapper.convention )]
		public unsafe static extern int GetDeviceData( void* /*IDirectInputDevice8*/ _this,
			uint cbObjectData, void*/*DIDEVICEOBJECTDATA*/ diDeviceObjectData,
			ref uint /*LPDWORD*/ pdwInOut, uint dwFlags );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_SetCooperativeLevel", CallingConvention = Wrapper.convention )]
		public unsafe static extern int SetCooperativeLevel( void*/*IDirectInputDevice8*/ _this,
			IntPtr hwnd, uint/*CooperativeLevelFlags*/ dwFlags );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_GetCapabilities", CallingConvention = Wrapper.convention )]
		public unsafe static extern int GetCapabilities( void* /*IDirectInputDevice8*/ _this,
			ref DIDEVCAPS lpDIDevCaps );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_Poll", CallingConvention = Wrapper.convention )]
		public unsafe static extern int Poll( void* /*IDirectInputDevice8*/ _this );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_SetProperty", CallingConvention = Wrapper.convention )]
		public unsafe static extern int SetProperty( void* /*IDirectInputDevice8*/ _this,
			void*/*GUID*/ rguidProp, ref DIPROPHEADER pdiph );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_SetProperty_DIPROPRANGE", CallingConvention = Wrapper.convention )]
		public unsafe static extern int SetProperty_DIPROPRANGE( void* /*IDirectInputDevice8*/ _this,
			void*/*GUID*/ rguidProp, ref DIPROPRANGE pdiph );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_CreateEffect", CallingConvention = Wrapper.convention )]
		public unsafe static extern int CreateEffect( void* /*IDirectInputDevice8*/ _this,
			ref GUID rguid, ref DIEFFECT /*LPCDIEFFECT*/ lpeff,
			out void* /*LPDIRECTINPUTEFFECT*/ ppdeff, void* /*LPUNKNOWN*/ punkOuter );

		[UnmanagedFunctionPointer( CallingConvention.Winapi )]
		public unsafe delegate bool EnumDeviceObjectsCallback(
			IntPtr /*DIDEVICEOBJECTINSTANCE*/ lpddoi, void* pvRef );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInput_EnumObjects", CallingConvention = Wrapper.convention )]
		public unsafe static extern int EnumObjects( void* /*IDirectInputDevice8*/ _this,
			EnumDeviceObjectsCallback lpCallback, void* pvRef, uint dwFlags );

		[UnmanagedFunctionPointer( CallingConvention.Winapi )]
		public unsafe delegate bool DIEnumCreatedEffectObjectsCallback( void* /*LPDIRECTINPUTEFFECT*/ peff, void* pvRef );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_EnumCreatedEffectObjects", CallingConvention = Wrapper.convention )]
		public unsafe static extern int EnumCreatedEffectObjects( void* /*IDirectInputDevice8*/ _this,
			DIEnumCreatedEffectObjectsCallback lpCallback, void* /*LPVOID*/ pvRef, uint fl );

		[UnmanagedFunctionPointer( CallingConvention.Winapi )]
		public unsafe delegate bool DIEnumEffectsCallback( IntPtr /*DIEFFECTINFO*/ pdei, void* pvRef );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_EnumEffects", CallingConvention = Wrapper.convention )]
		public unsafe static extern int EnumEffects( void* /*IDirectInputDevice8*/ _this,
			DIEnumEffectsCallback lpCallback, void* pvRef, uint dwEffType );

		//[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_Escape", CallingConvention = Wrapper.convention )]
		//public unsafe static extern int Escape( void* /*IDirectInputDevice8*/ _this, ref DIEFFESCAPE /*LPDIEFFESCAPE*/ pesc );

		//[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_GetEffectInfo", CallingConvention = Wrapper.convention )]
		//public unsafe static extern int GetEffectInfo( void* /*IDirectInputDevice8*/ _this, ref DIEFFECTINFO /*LPDIEFFECTINFOW*/ pdei, ref GUID rguid );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_GetForceFeedbackState", CallingConvention = Wrapper.convention )]
		public unsafe static extern int GetForceFeedbackState( void* /*IDirectInputDevice8*/ _this,
			ref uint /*LPDWORD*/ pdwOut );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputDevice8_SendForceFeedbackCommand", CallingConvention = Wrapper.convention )]
		public unsafe static extern int SendForceFeedbackCommand(
			void* /*IDirectInputDevice8*/ _this, uint dwFlags );
	}

	////////////////////////////////////////////////////////////////////////////////////////////////

	struct IDirectInputEffect
	{
		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputEffect_Download", CallingConvention = Wrapper.convention )]
		public unsafe static extern int Download( void* /*IDirectInputEffect*/ _this );

		//[DllImport( Wrapper.library, EntryPoint = "NIDirectInputEffect_Escape", CallingConvention = Wrapper.convention )]
		//public unsafe static extern int Escape( void* /*IDirectInputEffect*/ _this, ref DIEFFESCAPE /*LPDIEFFESCAPE*/ pesc );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputEffect_GetEffectGuid", CallingConvention = Wrapper.convention )]
		public unsafe static extern int GetEffectGuid( void* /*IDirectInputEffect*/ _this,
			ref GUID /*LPGUID*/ pguid );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputEffect_GetEffectStatus", CallingConvention = Wrapper.convention )]
		public unsafe static extern int GetEffectStatus( void* /*IDirectInputEffect*/ _this,
			ref uint /*LPDWORD*/ pdwFlags );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputEffect_GetParameters", CallingConvention = Wrapper.convention )]
		public unsafe static extern int GetParameters( void* /*IDirectInputEffect*/ _this,
			ref DIEFFECT /*LPDIEFFECT*/ peff, uint dwFlags );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputEffect_SetParameters", CallingConvention = Wrapper.convention )]
		public unsafe static extern int SetParameters( void* /*IDirectInputEffect*/ _this,
			ref DIEFFECT /*LPCDIEFFECT*/ peff, uint dwFlags );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputEffect_Start", CallingConvention = Wrapper.convention )]
		public unsafe static extern int Start( void* /*IDirectInputEffect*/ _this, uint dwIterations, uint dwFlags );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputEffect_Stop", CallingConvention = Wrapper.convention )]
		public unsafe static extern int Stop( void* /*IDirectInputEffect*/ _this );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputEffect_Unload", CallingConvention = Wrapper.convention )]
		public unsafe static extern int Unload( void* /*IDirectInputEffect*/ _this );

		[DllImport( Wrapper.library, EntryPoint = "NIDirectInputEffect_Release", CallingConvention = Wrapper.convention )]
		public unsafe static extern int Release( void* /*IDirectInputEffect*/ _this );
	}
}
#endif