// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
#pragma once

#import <Cocoa/Cocoa.h>
#import <AppKit/AppKit.h>
#import <Carbon/Carbon.h>
#include <dlfcn.h>
#include <sys/time.h>
#include <wchar.h>
#include <vector>
#include <queue>
#include <string>
#include <AGL/agl.h>
#include <OpenGL/OpenGL.h>

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

#define EXPORT extern "C" __attribute__ ((visibility("default")))

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

@interface AppDelegate : NSObject
{
}

- (id)initDelegate;

@end

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

enum EMouseButtons
{
	EMouseButtons_Unknown = -1,

	EMouseButtons_Left = 0,
	EMouseButtons_Right,
	EMouseButtons_Middle,
	EMouseButtons_XButton1,
	EMouseButtons_XButton2,
};

enum MessageTypes
{
	MessageTypes_MouseDown,
	MessageTypes_MouseUp,
	MessageTypes_MouseDoubleClick,
	MessageTypes_MouseWheel,
	MessageTypes_MouseMove,
	MessageTypes_KeyDown,
	MessageTypes_KeyUp,
	MessageTypes_WindowDidResize,
	MessageTypes_WindowDidBecomeKey,
	MessageTypes_WindowDidResignKey,
	MessageTypes_WindowWillMiniaturize,
	MessageTypes_WindowDidMiniaturize,
	MessageTypes_WindowDidDeminiaturize,
	MessageTypes_Periodic,
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

uint16* CreateOutString(const char* str);
uint16* CreateOutString(const std::wstring& str);
uint16* CreateOutString(const NSString* str);

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

void LogInfo(const NSString* text);
void LogWarning(const NSString* text);
void LogFatal(const NSString* text);

//ansi string
void LogInfo(const char* text);
void LogWarning(const char* text);
void LogFatal(const char* text);
