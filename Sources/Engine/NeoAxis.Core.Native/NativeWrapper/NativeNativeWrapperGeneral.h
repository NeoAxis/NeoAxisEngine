// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#pragma once

#ifdef _WIN32
	#define byte win_byte_override
	#include <windows.h>
	#include <objbase.h>
#endif

#include "Native.h"

#include "NativeArchive.h"
#include "NativeArchiveFactory.h"

#define super __super

#ifdef _WIN32
	#define INLINE __forceinline
	#define EXPORT extern "C" __declspec(dllexport)
#else
	#define INLINE __inline__ __attribute__((__always_inline__))
	#define EXPORT extern "C" __attribute__ ((visibility("default")))
#endif

#define SAFE_DELETE(q){if(q){delete q;q=NULL;}else 0;}

extern Native::wchar16* CreateOutString(const Native::WString& str);
extern Native::wchar16* CreateOutString(const Native::String& str);

//#ifdef _DEBUG
//#error Debug version are not supported.
//#endif

using Native::uint;
