// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
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
