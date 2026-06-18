// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#include "NativeStableHeaders.h"

//void Fatal(const char* text)
//{
//#ifdef PLATFORM_OSX
//	CFStringRef textRef = CFStringCreateWithCString(NULL, text, kCFStringEncodingUTF8);
//	CFUserNotificationDisplayAlert(0, kCFUserNotificationStopAlertLevel, NULL, NULL, NULL, 
//		CFSTR("Fatal"), textRef, CFSTR("OK"), NULL, NULL, NULL);
//	CFRelease(textRef);
//#else
//	MessageBoxA(NULL, text, "Fatal", MB_OK | MB_ICONEXCLAMATION);
//#endif
//	exit(0);
//}
//