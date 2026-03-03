// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "NativeStableHeaders.h"
#include "NativeDebugKeys.h"
#include "NativeException.h"

#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32 || OGRE_PLATFORM == OGRE_PLATFORM_WINRT
#define byte win_byte_override
#include <windows.h>
#endif

#if OGRE_PLATFORM == OGRE_PLATFORM_APPLE
	#include <Carbon/Carbon.h>
#endif

#ifdef ANDROID 
	#include <stdio.h>
	#include <android/log.h>
#endif


namespace Native
{
	bool DebugKeys::isKeyPressed(char keyCode)
	{
#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32
		return GetKeyState(keyCode) < 0;
#else
		return false;
#endif
	}

}

//void Fatal(const char* text)
//{
//#if OGRE_PLATFORM == OGRE_PLATFORM_APPLE
//	CFStringRef textRef = CFStringCreateWithCString(NULL, text, kCFStringEncodingUTF8);
//	CFUserNotificationDisplayAlert(0, kCFUserNotificationStopAlertLevel, NULL, NULL, NULL, 
//		CFSTR("Fatal"), textRef, CFSTR("OK"), NULL, NULL, NULL);
//	CFRelease(textRef);
//#elif defined ANDROID
////TO DO: vladimir need visual box
//	char tempBuffer[4096];
//	sprintf(tempBuffer, "OgreMain fatal error: %s\n", text);
//	__android_log_write(ANDROID_LOG_ERROR,"NeoAxis Engine", tempBuffer);
//#elif OGRE_PLATFORM == OGRE_PLATFORM_WIN32
//	MessageBox(NULL, text, "Fatal", MB_OK | MB_ICONEXCLAMATION);
//#elif OGRE_PLATFORM == OGRE_PLATFORM_WINRT
//	OutputDebugStringA("Fatal: ");
//	OutputDebugStringA(text);
//	OutputDebugStringA("\n");
//	std::cerr << "Fatal: " << text << std::endl;
//	OGRE_EXCEPT(Native::Exception::ERR_INTERNAL_ERROR, text, "Fatal");
//#endif
//
//#ifndef ANDROID
//	exit(0);
//#else
//	int* x = 0;
//	*x = 42;
//#endif
//}

void Fatal(const Native::String& text)
{
	Fatal(text.c_str());
}

void Fatal(const Native::WString& text)
{
	Fatal(Native::StringUtil::toUTF8(text));
}

void DebugMessage(const char* text)
{
#if OGRE_PLATFORM == OGRE_PLATFORM_APPLE_IOS
	printf("NeoAxisCoreNative fatal error: %s\n", text);
	//char tempBuffer[4096];
	//sprintf(tempBuffer, "OgreMain fatal error: %s\n", text);
	//printf(ANDROID_LOG_ERROR, "NeoAxis Engine", tempBuffer);
#elif OGRE_PLATFORM == OGRE_PLATFORM_APPLE
	CFStringRef textRef = CFStringCreateWithCString(NULL, text, kCFStringEncodingUTF8);
	CFUserNotificationDisplayAlert(0, kCFUserNotificationStopAlertLevel, NULL, NULL, NULL,
		CFSTR("Fatal"), textRef, CFSTR("OK"), NULL, NULL, NULL);
	CFRelease(textRef);
#elif defined ANDROID
	char tempBuffer[4096];
	sprintf(tempBuffer, "NeoAxisCoreNative fatal error: %s\n", text);
	__android_log_write(ANDROID_LOG_ERROR, "NeoAxis Engine", tempBuffer);
#elif OGRE_PLATFORM == OGRE_PLATFORM_WIN32
	MessageBox(NULL, text, "Fatal", MB_OK | MB_ICONEXCLAMATION);
#elif OGRE_PLATFORM == OGRE_PLATFORM_WINRT
	OutputDebugStringA("Fatal: ");
	OutputDebugStringA(text);
	OutputDebugStringA("\n");
	std::cerr << "Fatal: " << text << std::endl;
	OGRE_EXCEPT(Native::Exception::ERR_INTERNAL_ERROR, text, "Fatal");
#endif
}

void DebugMessage(const Native::String& text)
{
	DebugMessage(text.c_str());
}

void DebugMessage(const Native::WString& text)
{
	DebugMessage(Native::StringUtil::toUTF8(text));
}
