// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "OgreDebugKeys.h"
#include "OgreException.h"

#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32 || OGRE_PLATFORM == OGRE_PLATFORM_WINRT
#include <windows.h>
#endif

#if OGRE_PLATFORM == OGRE_PLATFORM_APPLE
	#include <Carbon/Carbon.h>
#endif

#ifdef ANDROID 
	#include <stdio.h>
	#include <android/log.h>
#endif


namespace Ogre
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
//	OGRE_EXCEPT(Ogre::Exception::ERR_INTERNAL_ERROR, text, "Fatal");
//#endif
//
//#ifndef ANDROID
//	exit(0);
//#else
//	int* x = 0;
//	*x = 42;
//#endif
//}

void Fatal(const Ogre::String& text)
{
	Fatal(text.c_str());
}

void Fatal(const Ogre::WString& text)
{
	Fatal(Ogre::StringUtil::toUTF8(text));
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
	OGRE_EXCEPT(Ogre::Exception::ERR_INTERNAL_ERROR, text, "Fatal");
#endif
}

void DebugMessage(const Ogre::String& text)
{
	DebugMessage(text.c_str());
}

void DebugMessage(const Ogre::WString& text)
{
	DebugMessage(Ogre::StringUtil::toUTF8(text));
}
