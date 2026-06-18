// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#include "NativeStableHeaders.h"
#include "NeoAxisCoreNative.h"
#include "DirectInputNativeWrapper.h"

#pragma region Desktop Family
#ifdef PLATFORM_WINDOWS

#pragma comment (lib, "dinput8.lib")
#pragma comment (lib, "dxguid.lib")

#endif /* PLATFORM_WINDOWS */
#pragma endregion

//#ifndef _UNICODE
//	#error need unicode
//#endif
