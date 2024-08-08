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
#elif defined( __APPLE__)
	#include <TargetConditionals.h>
	#if TARGET_OS_IPHONE
		#define PLATFORM_IOS
	#elif TARGET_OS_MAC
		#define PLATFORM_OSX
	#else
		#error "Unknown Apple platform"
	#endif
#elif defined(__ANDROID__)
	#define PLATFORM_ANDROID
#elif defined(__linux__)
	#define PLATFORM_LINUX
#else
	#error Platform is not supported.
#endif

#if defined(PLATFORM_WINDOWS) || defined(PLATFORM_WINRT)
	#ifndef _CRT_SECURE_NO_WARNINGS
		#define _CRT_SECURE_NO_WARNINGS
	#endif
	#ifndef NOMINMAX
		#define NOMINMAX
	#endif
	#include <windows.h>
#endif

#ifdef PLATFORM_OSX
	#include <Carbon/Carbon.h>
#endif

#ifdef PLATFORM_ANDROID
	#include <stdlib.h>
	#include <pthread.h>
	#include <string.h>
	#include <stdint.h>
	#include <android/log.h>
#endif

#ifdef PLATFORM_LINUX
	#include <stdlib.h>
	#include <pthread.h>
	#include <string.h>
	#include <stdint.h>
#endif

#include <stdio.h>

//#ifdef _DEBUG
//#error Debug version are not supported.
//#endif
