// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

#pragma region Desktop Family
#ifdef PLATFORM_WINDOWS

using namespace Ogre;

#define EXPORT extern "C" __declspec(dllexport)

//typedef unsigned int uint;
//typedef unsigned char uint8;
#define SAFE_DELETE(q){if(q){delete q;q=NULL;}else 0;}
#define SAFE_RELEASE(p) { if(p) { (p)->Release(); (p)=NULL; } }

///////////////////////////////////////////////////////////////////////////////////////////////////

wchar_t* CreateOutString(const wchar_t* str)
{
//#ifndef _UNICODE
//	#error need unicode
//#endif

	wchar_t* result = new wchar_t[wcslen(str) + 1];
	wcscpy_s(result, wcslen(str) + 1, str);
	return result;
}

EXPORT void DInput_FreeOutString(wchar_t* pointer)
{
	delete[] pointer;
}

///////////////////////////////////////////////////////////////////////////////////////////////////

//struct DInputStructureSizes
//{
//public:
//
//	int sizeGUID;
//	int sizeDIMOUSESTATE;
//	int sizeDIDEVICEINSTANCE;
//
//	void Init()
//	{
//		sizeGUID = sizeof( GUID );
//		sizeDIMOUSESTATE = sizeof( DIMOUSESTATE );
//		sizeDIDEVICEINSTANCE = sizeof( DIDEVICEINSTANCE );
//	}
//};

///////////////////////////////////////////////////////////////////////////////////////////////////

//EXPORT void DInput_GetStructureSizes( DInputStructureSizes* sizes )
//{
//	DInputStructureSizes originalSizes;
//	originalSizes.Init();
//	*sizes = originalSizes;
//}

EXPORT int DInput_DirectInput8Create( const IID & riidltf, void** ppvOut )
{
	return DirectInput8Create(GetModuleHandle( NULL ), DIRECTINPUT_VERSION,
		riidltf, ppvOut, NULL);
}

EXPORT const DIDATAFORMAT* DInput_Get_c_dfDIJoystick()
{
	return &c_dfDIJoystick;
}

EXPORT const DIDATAFORMAT* DInput_Get_c_dfDIJoystick2()
{
	return &c_dfDIJoystick2;
}

EXPORT const wchar_t* DInput_DXGetErrorStringW(int hr)
//EXPORT const WCHAR* DInput_DXGetErrorStringW( int hr )
{
#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32
	LPVOID lpMsgBuf;
	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER |
		FORMAT_MESSAGE_FROM_SYSTEM |
		FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL,
		hr,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf,
		0,
		NULL
	);
	const wchar_t* str = (wchar_t*)lpMsgBuf;
	wchar_t* result = CreateOutString(str ? str : L"");
	LocalFree(lpMsgBuf);
	return result;
#endif

	////#ifndef _UNICODE
	////	#error need unicode
	////#endif

	//const wchar_t* str = DXGetErrorStringW(hr);
	////const WCHAR* str = DXGetErrorString(hr);
	//if(!str)
	//	return NULL;
	//return CreateOutString(str);
}

EXPORT const DIDATAFORMAT* DInput_Get_c_dfDIMouse()
{
	return &c_dfDIMouse;
}

EXPORT int DInput_GetHRESULT_DIERR_INPUTLOST()
{
	return DIERR_INPUTLOST;
}

EXPORT int DInput_GetHRESULT_DIERR_NOTACQUIRED()
{
	return DIERR_NOTACQUIRED;
}

EXPORT int DInput_GetHRESULT_DIERR_NOTINITIALIZED()
{
	return DIERR_NOTINITIALIZED;
}

EXPORT const GUID * DInput_getDIPROP_APPDATA()
{
	return &DIPROP_APPDATA;
}

EXPORT const GUID * DInput_getDIPROP_AUTOCENTER()
{
	return &DIPROP_AUTOCENTER;
}

EXPORT const GUID * DInput_getDIPROP_AXISMODE()
{
	return &DIPROP_AXISMODE;
}

EXPORT const GUID * DInput_getDIPROP_BUFFERSIZE()
{
	return &DIPROP_BUFFERSIZE;
}

EXPORT const GUID * DInput_getDIPROP_CALIBRATION()
{
	return &DIPROP_CALIBRATION;
}

EXPORT const GUID * DInput_getDIPROP_CALIBRATIONMODE()
{
	return &DIPROP_CALIBRATIONMODE;
}

EXPORT const GUID * DInput_getDIPROP_CPOINTS()
{
	return &DIPROP_CPOINTS;
}

EXPORT const GUID * DInput_getDIPROP_DEADZONE()
{
	return &DIPROP_DEADZONE;
}

EXPORT const GUID * DInput_getDIPROP_FFGAIN()
{
	return &DIPROP_FFGAIN;
}

EXPORT const GUID * DInput_getDIPROP_INSTANCENAME()
{
	return &DIPROP_INSTANCENAME;
}

EXPORT const GUID * DInput_getDIPROP_PRODUCTNAME()
{
	return &DIPROP_PRODUCTNAME;
}

EXPORT const GUID * DInput_getDIPROP_RANGE()
{
	return &DIPROP_RANGE;
}

EXPORT const GUID * DInput_getDIPROP_SATURATION()
{
	return &DIPROP_SATURATION;
}

///////////////////////////////////////////////////////////////////////////////////////////////////

//-----------------------------------------------------------------------------
//  Enum each PNP device using WMI and check each device ID to see if it  contains 
// "IG_" (ex. "VID_045E&PID_028E&IG_00").  If it does, then it's an XInput device
// Unfortunately this information can not be  found by just using DirectInput    
//-----------------------------------------------------------------------------
EXPORT bool DInput_IsXInputDevice( const GUID * pGuidProductFromDirectInput )
{
	IWbemLocator*           pIWbemLocator  = NULL;
	IEnumWbemClassObject*  	pEnumDevices   = NULL;
	IWbemClassObject*       pDevices[20]   = {0};
	IWbemServices*          pIWbemServices = NULL;
	BSTR                 	bstrNamespace  = NULL;
	BSTR                    bstrDeviceID   = NULL;
	BSTR                    bstrClassName  = NULL;
	DWORD           			uReturned      = 0;
	bool                    bIsXinputDevice= false;
	UINT                    iDevice        = 0;
	VARIANT         			var;
	HRESULT                 hr;

	// CoInit if needed

	hr = CoInitialize(NULL);
	bool bCleanupCOM = SUCCEEDED(hr);

	// Create WMI
	hr = CoCreateInstance( __uuidof(WbemLocator), NULL, CLSCTX_INPROC_SERVER, 
		__uuidof(IWbemLocator), (LPVOID*) &pIWbemLocator);

	if(FAILED(hr) || pIWbemLocator == NULL)
		goto LCleanup;

	bstrNamespace = SysAllocString(L"\\\\.\\root\\cimv2" );
	if( bstrNamespace == NULL )
		goto LCleanup;        
	bstrClassName = SysAllocString( L"Win32_PNPEntity" );   
	if(bstrClassName == NULL )
		goto LCleanup;        
	bstrDeviceID  =	SysAllocString( L"DeviceID" );
	if( bstrDeviceID == NULL )
		goto LCleanup;        

	// Connect to WMI 
	hr = pIWbemLocator->ConnectServer( bstrNamespace, NULL, NULL, 0L, 0L, NULL, NULL, &pIWbemServices );
	if(	FAILED(hr) || pIWbemServices == NULL ) goto LCleanup;

	//	Switch security level to IMPERSONATE. 
	//CoSetProxyBlanket(pIWbemServices, RPC_C_AUTHN_WINNT, RPC_C_AUTHZ_NONE, NULL, 
	//	RPC_C_AUTHN_LEVEL_CALL, RPC_C_IMP_LEVEL_IMPERSONATE, NULL, EOAC_NONE); 
	CoSetProxyBlanket( pIWbemServices, RPC_C_AUTHN_WINNT, RPC_C_AUTHZ_NONE, NULL, 
		RPC_C_AUTHN_LEVEL_CALL, RPC_C_IMP_LEVEL_IMPERSONATE, NULL, 0 );

	hr = pIWbemServices->CreateInstanceEnum(bstrClassName, 0, NULL, &pEnumDevices ); 
	if( FAILED(hr) || pEnumDevices == NULL )
		goto LCleanup;

	// Loop over all devices
	for( ;; )
	{
		// Get 20 at a time
		hr = pEnumDevices->Next( 10000, 20, pDevices, &uReturned );
		if(FAILED(hr)) goto LCleanup;
		if( uReturned == 0 ) break;

		for(iDevice = 0; iDevice < uReturned; iDevice++)
		{
			// For each device, get its device ID

			hr = pDevices[iDevice]->Get( bstrDeviceID, 0L, &var, NULL, NULL );

			if( SUCCEEDED( hr ) && 
				var.vt == VT_BSTR &&
				var.bstrVal != NULL )
			{
				// Check if the device ID contains "IG_".  If it does, then it's an XInput device
				// This information can not be found from DirectInput 
					if( wcsstr(var.bstrVal, L"IG_" ) )
					{
						// If it does, then get the VID/PID from var.bstrVal
						DWORD dwPid = 0, dwVid = 0;
						WCHAR* strVid = wcsstr( var.bstrVal, L"VID_" );
						if( strVid && swscanf( strVid, L"VID_%4X", &dwVid ) != 1 )
							dwVid = 0;

						WCHAR* strPid = wcsstr( var.bstrVal, L"PID_" );
						if(strPid && swscanf( strPid, L"PID_%4X", &dwPid ) != 1)
							dwPid = 0;

						// Compare the VID/PID to the DInput device
						DWORD dwVidPid = MAKELONG(dwVid, dwPid );
						if( dwVidPid ==	pGuidProductFromDirectInput->Data1 )
						{
							bIsXinputDevice = true;
							goto LCleanup;
						}
					}
			}   

			SAFE_RELEASE( pDevices[iDevice] );
		}
	}

LCleanup:

	if(bstrNamespace)
		SysFreeString(bstrNamespace);
	if(bstrDeviceID)
		SysFreeString(bstrDeviceID);
	if(bstrClassName)
		SysFreeString(bstrClassName);

	for(iDevice = 0; iDevice < 20; iDevice++ )
		SAFE_RELEASE(pDevices[iDevice] );

	SAFE_RELEASE( pEnumDevices );
	SAFE_RELEASE( pIWbemLocator );
	SAFE_RELEASE( pIWbemServices );

	if(bCleanupCOM)
		CoUninitialize();

	return bIsXinputDevice;
}

///////////////////////////////////////////////////////////////////////////////////////////////////

EXPORT uint NIDirectInput_Release( IDirectInput* _this )
{
	return _this->Release();
}

EXPORT int NIDirectInput_CreateDevice( IDirectInput* _this, 
	const GUID & rguid, IDirectInputDevice** lplpDirectInputDevice, LPUNKNOWN pUnkOuter )
{
	return _this->CreateDevice(rguid, lplpDirectInputDevice, pUnkOuter);
}

//
// callback prototype:
// BOOL CALLBACK DIEnumDevicesCallback( LPCDIDEVICEINSTANCE lpddi, LPVOID pvRef  );
//

EXPORT int NIDirectInput_EnumDevices( IDirectInput* _this, 
	DWORD dwDevType,              
	LPDIENUMDEVICESCALLBACK lpCallback,  
	LPVOID pvRef,                 
	DWORD dwFlags )
{
	return _this->EnumDevices(dwDevType, lpCallback, pvRef, dwFlags);
}

//
// callback prototype:
// BOOL DIEnumDeviceObjectsCallback(LPCDIDEVICEOBJECTINSTANCE lpddoi, LPVOID pvRef);
//

EXPORT int NIDirectInput_EnumObjects(IDirectInputDevice8* _this,
  LPDIENUMDEVICEOBJECTSCALLBACK lpCallback,
  LPVOID pvRef,
  DWORD dwFlags)
{
	return _this->EnumObjects(lpCallback, pvRef, dwFlags);
}

///////////////////////////////////////////////////////////////////////////////////////////////////

EXPORT uint NIDirectInputDevice8_Release( IDirectInputDevice8* _this )
{
	return _this->Release();
}

EXPORT int NIDirectInputDevice8_Acquire( IDirectInputDevice8* _this )
{
	return _this->Acquire();
}

EXPORT int NIDirectInputDevice8_Unacquire( IDirectInputDevice8* _this )
{
	return _this->Unacquire();
}

EXPORT int NIDirectInputDevice8_GetDeviceState( IDirectInputDevice8* _this,
	DWORD cbData, void* lpvData )
{
	return _this->GetDeviceState(cbData, lpvData);
}

EXPORT int NIDirectInputDevice8_GetDeviceData( IDirectInputDevice8* _this,
	  DWORD cbObjectData,
	  LPDIDEVICEOBJECTDATA rgdod,
	  LPDWORD pdwInOut,
	  DWORD dwFlags)
{
	return _this->GetDeviceData(cbObjectData, rgdod, pdwInOut, dwFlags);
}

EXPORT int NIDirectInputDevice8_SetDataFormat( IDirectInputDevice8* _this, 
	DIDATAFORMAT* lpdf )
{
	return _this->SetDataFormat(lpdf);
}

EXPORT int NIDirectInputDevice8_SetCooperativeLevel( IDirectInputDevice8* _this,
	HWND hwnd, uint dwFlags )
{
	return _this->SetCooperativeLevel(hwnd, dwFlags);
}

EXPORT int NIDirectInputDevice8_GetCapabilities( IDirectInputDevice8* _this,
	LPDIDEVCAPS lpDIDevCaps )
{
	return _this->GetCapabilities(lpDIDevCaps);
}

EXPORT int NIDirectInputDevice8_Poll( IDirectInputDevice8* _this)
{
	return _this->Poll();
}

EXPORT int NIDirectInputDevice8_SetProperty(IDirectInputDevice8* _this,
	REFGUID rguidProp, LPCDIPROPHEADER pdiph)
{
	return _this->SetProperty(rguidProp, pdiph);
}

EXPORT int NIDirectInputDevice8_SetProperty_DIPROPRANGE(IDirectInputDevice8* _this,
	REFGUID rguidProp, LPDIPROPRANGE pdiph)
{
	return _this->SetProperty(rguidProp, (LPCDIPROPHEADER)pdiph);
}

EXPORT int NIDirectInputDevice8_CreateEffect(IDirectInputDevice8* _this,
	REFGUID rguid, LPCDIEFFECT lpeff, LPDIRECTINPUTEFFECT * ppdeff, LPUNKNOWN punkOuter)
{
	return _this->CreateEffect(rguid, lpeff, ppdeff, punkOuter);
}

EXPORT int NIDirectInputDevice8_EnumCreatedEffectObjects(IDirectInputDevice8* _this,
  LPDIENUMCREATEDEFFECTOBJECTSCALLBACK lpCallback, LPVOID pvRef, DWORD fl)
{
	return _this->EnumCreatedEffectObjects(lpCallback, pvRef, fl);
}

EXPORT int NIDirectInputDevice8_EnumEffects(IDirectInputDevice8* _this,
  LPDIENUMEFFECTSCALLBACK lpCallback, LPVOID pvRef, DWORD dwEffType)
{
	return _this->EnumEffects(lpCallback, pvRef, dwEffType);
}

//EXPORT int NIDirectInputDevice8_Escape(IDirectInputDevice8* _this, LPDIEFFESCAPE pesc)
//{
//	return _this->Escape(pesc);
//}

//EXPORT int NIDirectInputDevice8_GetEffectInfo(IDirectInputDevice8* _this, 
//	LPDIEFFECTINFOW pdei, const GUID & rguid)
//{
//	return _this->GetEffectInfo(pdei, rguid);
//}

EXPORT int NIDirectInputDevice8_GetForceFeedbackState(IDirectInputDevice8* _this, LPDWORD pdwOut)
{
	return _this->GetForceFeedbackState(pdwOut);
}

EXPORT int NIDirectInputDevice8_SendForceFeedbackCommand(IDirectInputDevice8* _this, DWORD dwFlags)
{
	return _this->SendForceFeedbackCommand(dwFlags);
}	   

///////////////////////////////////////////////////////////////////////////////////////////////////

EXPORT int NIDirectInputEffect_Download(IDirectInputEffect *_this)
{
	return _this->Download();
}

//EXPORT int NIDirectInputEffect_Escape(IDirectInputEffect *_this, LPDIEFFESCAPE pesc)
//{
//	return _this->Escape(pesc);
//}

EXPORT int NIDirectInputEffect_GetEffectGuid(IDirectInputEffect *_this, LPGUID pguid)
{
	return _this->GetEffectGuid(pguid);
}

EXPORT int NIDirectInputEffect_GetEffectStatus(IDirectInputEffect *_this, LPDWORD pdwFlags)
{
	return _this->GetEffectStatus(pdwFlags);
}

EXPORT int NIDirectInputEffect_GetParameters(IDirectInputEffect *_this, LPDIEFFECT peff, DWORD dwFlags)
{
	return _this->GetParameters(peff, dwFlags);
}

EXPORT int NIDirectInputEffect_SetParameters(IDirectInputEffect *_this, LPCDIEFFECT peff, DWORD dwFlags)
{
	return _this->SetParameters(peff, dwFlags);
}

EXPORT int NIDirectInputEffect_Start(IDirectInputEffect *_this, DWORD dwIterations, DWORD dwFlags)
{
	return _this->Start(dwIterations, dwFlags);
}

EXPORT int NIDirectInputEffect_Stop(IDirectInputEffect *_this)
{
	return _this->Stop();
}

EXPORT int NIDirectInputEffect_Unload(IDirectInputEffect *_this)
{
	return _this->Unload();
}

EXPORT uint NIDirectInputEffect_Release(IDirectInputEffect *_this )
{
	return _this->Release();
}


#endif /* PLATFORM_WINDOWS */
#pragma endregion


///////////////////////////////////////////////////////////////////////////////////////////////////
