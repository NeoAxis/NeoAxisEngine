// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

#ifdef _WIN32
	#define EXPORT extern "C" __declspec(dllexport)
#else
	#define EXPORT extern "C" __attribute__ ((visibility("default")))
#endif

//typedef unsigned int uint;
//typedef unsigned char uint8;

//#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32 || OGRE_PLATFORM == OGRE_PLATFORM_WINRT
//	typedef wchar_t wchar16;
//#else
//	typedef unsigned short wchar16;
//#endif

//#ifdef _DEBUG
//#error Debug version are not supported.
//#endif

#if (defined( __WIN32__ ) || defined( _WIN32 )) && !defined(__ANDROID__)
#   include <sdkddkver.h>
#   if defined(WINAPI_FAMILY)
#       include <winapifamily.h>
#       if WINAPI_FAMILY == WINAPI_FAMILY_APP|| WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
#           define PLATFORM_WINRT
#       else
#           define PLATFORM_WINDOWS
#       endif
#   else
#       define PLATFORM_WINDOWS
#   endif
#elif defined( __APPLE_CC__)
#	define PLATFORM_MACOS
#elif defined(__ANDROID__)
#   define PLATFORM_ANDROID
#else
	#error Platform is not supported.
#endif




////From UtilsNativeWrapper
//#ifdef _WIN32
//	#include <windows.h>
//	#include <stdio.h>
//	#include <float.h>
//#endif
//
//#ifdef __APPLE_CC__
//	#include <pthread.h>
//	#include <stdio.h>
//	#include <string.h>
//#endif
//
//#ifdef ANDROID
//	#include <pthread.h>
//	#include <stdio.h>
//	#include <string.h>
//#endif
//
//#include "MemoryManager.h"
//
//
////From UtilsNativeWrapper END





////#ifdef _WIN32
//#include <windows.h>
//#include <stdio.h>
//#include <float.h>
//#include <memory.h>
//#include <atlbase.h>
//#include <atlstr.h>
//#include <time.h>
////#include <stdint.h>
//#include <stdlib.h>
//#include <algorithm>
//#include <string>
//#include <vector>
//#include <sstream>
////#endif

//#ifdef __APPLE_CC__
//	#include <pthread.h>
//	#include <stdio.h>
//	#include <string.h>
//#endif

//#ifdef ANDROID
//	#include <pthread.h>
//	#include <stdio.h>
//	#include <string.h>
//#endif




//
//
//
//
//
//#include <memory.h>
//#include <stdlib.h>
//#ifdef PLATFORM_WINDOWS
//	#include <malloc.h>
//#endif
//#include <stdio.h>
//#include <stdlib.h>
//#include <assert.h>
//#include <string.h>
//#include <float.h>
//#include <math.h>
//#include <wchar.h>
//#include <vector>
//#include <set>
//#include <map>
//#include <cmath>
//#include <algorithm>
//#include <sstream>
//#ifdef PLATFORM_MACOS
//	#include <new>
//#endif
//
//#ifdef PLATFORM_MACOS
//	#define isfinite std::isfinite
//#endif
//
//#include "MemoryManager.h"
//#undef malloc
//#undef calloc
//#undef realloc
//#undef free
//#undef _aligned_malloc
//#undef _aligned_free
//
//#include <string>

//#define _DefinedMemoryAllocationType MemoryAllocationType_Renderer
//#include "MemoryManager.h"
//
//#include "MemoryManager_SimpleNew.h"

//From DirectInputNativeWrapper
#ifdef PLATFORM_WINDOWS
#define _CRT_SECURE_NO_DEPRECATE 
#ifndef _WIN32_DCOM
#define _WIN32_DCOM 
#endif	
#include <windows.h>
#include <wbemidl.h>
#include <strsafe.h>
#define DIRECTINPUT_VERSION 0x0800	
#include <dinput.h>
#endif //PLATFORM_WINDOWS
//From DirectInputNativeWrapper END

#ifdef _WIN32
#include <windows.h>
#endif

#ifdef __APPLE_CC__
#include <stdio.h>
#include <string.h>

typedef unsigned char byte;
#endif


//#include <memory.h>
//#include <stdlib.h>
//#if OGRE_PLATFORM != OGRE_PLATFORM_APPLE
//	#include <malloc.h>
//#endif
//#include <stdio.h>
//#include <stdlib.h>
//#include <assert.h>
//#include <string.h>
//#include <float.h>
//#include <math.h>
//#include <wchar.h>

//#define _DefinedMemoryAllocationType MemoryAllocationType_Renderer
//#include "MemoryManager.h"
//#include "MemoryManager_SimpleNew.h"
//
//#include <new>

#include "OgrePrerequisites.h"
#include "OgreStableHeaders.h"
#include "Vector3I.h"
#include "AxisAlignedBoxI.h"
#include "Bounds.h"
#include "BoundsD.h"
#include "BoundsI.h"
#include "OBB.h"

//#ifdef _WIN32
//#include <windows.h>
//#include <stdio.h>
//#include <float.h>
//#include <memory.h>
//#include <atlbase.h>
//#include <atlstr.h>
//#include <time.h>
////#include <stdint.h>
//#include <stdlib.h>
//#include <algorithm>
//#include <string>
//#include <vector>
//#include <sstream>
//#endif

////#define _DefinedMemoryAllocationType MemoryAllocationType_Physics
//#include "MemoryManager_SimpleNew.h"
//
//#ifdef PLATFORM_WINDOWS
//	typedef wchar_t wchar16;
//#else
//	typedef unsigned short wchar16;
//#endif
//
//typedef unsigned char byte;
//typedef unsigned int uint;
//
//typedef std::string String;
//typedef std::wstring WString;

//extern void Fatal(const char* text);


#include "Bounds.h"
#include "BoundsI.h"
#include "OBB.h"
#include "OBBD.h"
#include "OgrePlaneD.h"
#include "OgreRayD.h"
#include "OgreAxisAlignedBoxD.h"
