// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

/* Finds the current platform */
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
	#define PLATFORM_MACOS
#elif defined(__ANDROID__)
	#define PLATFORM_ANDROID
#else
	#error Platform is not supported.
#endif

#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINRT)
	#define _CRT_SECURE_NO_WARNINGS
	#define NOMINMAX
	#include <windows.h>
#endif

#ifdef PLATFORM_MACOS
	#include <Carbon/Carbon.h>
#endif

#ifdef PLATFORM_ANDROID
	#include <stdlib.h>
	#include <pthread.h>
	#include <string.h>
	#include <stdint.h>
	#include <android/log.h>
#endif

#include <stdio.h>

//#ifdef _DEBUG
//#error Debug version are not supported.
//#endif
