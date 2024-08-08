// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

#include "NeoAxisCoreNative.h"
#ifdef PLATFORM_OSX

qqqq;

#import <Cocoa/Cocoa.h>
#import <AppKit/AppKit.h>
#import <Carbon/Carbon.h>
#include <dlfcn.h>
#include <sys/time.h>
#include <sys/sysctl.h> //from utils
#include <wchar.h>
#include <vector>
#include <queue>
#include <string>

//#include <AGL/agl.h>
//#include <OpenGL/OpenGL.h>


#include "KeyCodes.h"
//!!!!было в 3.5
//#import "Joystick.h"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

#define EXPORT extern "C" __attribute__ ((visibility("default")))

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

@interface AppDelegate : NSObject
{
}

- (id)initDelegate;

@end

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

qqq;

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

qqqq;

uint16* CreateOutString(const char* str);
uint16* CreateOutString(const std::wstring& str);
uint16* CreateOutString(const NSString* str);

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

qqqqq;

void LogInfo(const NSString* text);
void LogWarning(const NSString* text);
void LogFatal(const NSString* text);

//ansi string
void LogInfo(const char* text);
void LogWarning(const char* text);
void LogFatal(const char* text);

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

qqqqq;


typedef void* CallbackMessageEventFunction(MessageTypes messageType, int parameterA, int parameterB, int parameterC);
typedef void CallbackLogInfoFunction(const uint16* text);
typedef void CallbackLogWarningFunction(const uint16* text);
typedef void CallbackLogFatalFunction(const uint16* text);

CallbackMessageEventFunction* callbackMessageEvent = NULL;
CallbackLogInfoFunction* callbackLogInfo = NULL;
CallbackLogInfoFunction* callbackLogWarning = NULL;
CallbackLogFatalFunction* callbackLogFatal = NULL;

NSWindow* mainWindow = NULL;

enum KeyModifierFlags
{
	KeyModifierFlags_Unknown = -1,

	KeyModifierFlags_LeftShift = 0,
	KeyModifierFlags_RightShift,
	KeyModifierFlags_LeftControl,
	KeyModifierFlags_RightControl,
	KeyModifierFlags_LeftAlt,
	KeyModifierFlags_RightAlt,
	KeyModifierFlags_LeftCommand,
	KeyModifierFlags_RightCommand,
	KeyModifierFlags_Function,

	KeyModifierFlags_Count,
};

struct Vec3i
{
	int x;
	int y;
	int z;
};

bool keysPressed[EKeys_Count];
bool capsLockPressed = false;

int ignoreMouseMoveDeltaCounter = 3;
int mouseMoveDeltaX = 0;
int mouseMoveDeltaY = 0;

int eKeyToKeyCode[EKeys_Count];

bool needQuitMessageLoop = false;

bool needRestoreVideoMode = false;
int initialScreenSizeX = 0;
int initialScreenSizeY = 0;
int initialScreenBPP = 0;

bool initialGammaInitialized = false;
CGGammaValue initialGammaRedTable[256];
CGGammaValue initialGammaGreenTable[256];
CGGammaValue initialGammaBlueTable[256];

CGLContextObj fullscreenMinimizedMode_CGLContextObj = NULL;

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

qqq;
void InitGamma();
EXPORT bool ChangeVideoMode(int width, int height, int bpp);

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

uint16* CreateOutString(const char* str)
{
	int length = strlen(str);

	uint16* result = new uint16[length + 1];
	for (int n = 0; n < length; n++)
		result[n] = (uint16)str[n];
	result[length] = 0;

	return result;
}

uint16* CreateOutString(const std::wstring& str)
{
	int length = str.length();

	uint16* result = new uint16[length + 1];
	for (int n = 0; n < length; n++)
		result[n] = (uint16)str[n];
	result[length] = 0;

	return result;
}

uint16* CreateOutString(const NSString* str)
{
	int length = [str length];

	uint16* result = new uint16[length + 1];
	for (int n = 0; n < length; n++)
		result[n] = (uint16)[str characterAtIndex : n];
	result[length] = 0;

	return result;
}

EXPORT void MacAppNativeWrapper_FreeOutString(uint16* buffer)
{
	delete[] buffer;
}

qqqq;

void MessageEvent(MessageTypes messageType, int parameterA = 0, int parameterB = 0, int parameterC = 0)
{
	callbackMessageEvent(messageType, parameterA, parameterB, parameterC);
}

void LogInfo(const char* text)
{
	callbackLogInfo(CreateOutString(text));
}

void LogInfo(const NSString* text)
{
	callbackLogInfo(CreateOutString(text));
}

void LogWarning(const char* text)
{
	callbackLogWarning(CreateOutString(text));
}

void LogWarning(const NSString* text)
{
	callbackLogWarning(CreateOutString(text));
}

void LogFatal(const char* text)
{
	callbackLogFatal(CreateOutString(text));
}

void LogFatal(const NSString* text)
{
	callbackLogFatal(CreateOutString(text));
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct Recti
{
	int left;
	int top;
	int right;
	int bottom;
};

Recti ConvertToRecti(NSRect nsRect)
{
	Recti result;
	result.left = (int)nsRect.origin.x;
	result.top = (int)nsRect.origin.y;
	result.right = result.left + (int)nsRect.size.width;
	result.bottom = result.top + (int)nsRect.size.height;
	return result;
}

Recti ConvertToRecti(CGRect cgRect)
{
	Recti result;
	result.left = (int)cgRect.origin.x;
	result.top = (int)cgRect.origin.y;
	result.right = result.left + (int)cgRect.size.width;
	result.bottom = result.top + (int)cgRect.size.height;
	return result;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

enum WindowState
{
	WindowState_Maximized,
	WindowState_Minimized,
	WindowState_Normal
};

enum WindowBorderStyles
{
	WindowBorderStyles_None,
	WindowBorderStyles_Sizeable
};

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

void GetOSVersion(int* major, int* minor, int* bugFix)
{
	SInt32 nmajor = 0;
	SInt32 nminor = 0;
	SInt32 nbugFix = 0;
	Gestalt(gestaltSystemVersionMajor, &nmajor);
	Gestalt(gestaltSystemVersionMinor, &nminor);
	Gestalt(gestaltSystemVersionBugFix, &nbugFix);
	*major = nmajor;
	*minor = nminor;
	*bugFix = nbugFix;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

void ResetKeyPressedFlags()
{
	for (int n = 0; n < EKeys_Count; n++)
		keysPressed[n] = false;
}

int GetKeyCodeByEKey(EKeys eKey)
{
	return eKeyToKeyCode[eKey];
}

NSString* GetNSStringFromUTF16(const uint16* text)
{
	int length = 0;
	while (true)
	{
		if (text[length] == 0)
			break;
		length++;
	}

	NSString* nsString = [[NSString alloc]initWithBytes:text length : (length * 2)
		encoding : NSUTF16LittleEndianStringEncoding];

	return nsString;
}

EXPORT void MacAppNativeWrapper_MessageBox(const uint16* text, const uint16* caption)
{
	//NSAutoreleasePool* pool = [[NSAutoreleasePool alloc] init];

	if (needRestoreVideoMode)
		ChangeVideoMode(initialScreenSizeX, initialScreenSizeY, initialScreenBPP);

	CGLContextObj contextObj = CGLGetCurrentContext();
	CGLSetCurrentContext(NULL);
	CGLClearDrawable(contextObj);

	CGReleaseAllDisplays();

	[mainWindow miniaturize : nil] ;

	//show message
	{
		NSString* nsText = GetNSStringFromUTF16(text);
		NSString* nsCaption = GetNSStringFromUTF16(caption);

		bool cursorVisible = CGCursorIsVisible();
		if (!cursorVisible)
			CGDisplayShowCursor(kCGDirectMainDisplay);

		NSAlert* alert = [[NSAlert alloc]init];
		[alert setMessageText : nsCaption] ;
		[alert setAlertStyle : NSCriticalAlertStyle] ;
		[alert setInformativeText : nsText] ;
		[alert runModal] ;
		//[alert release];

		if (!cursorVisible)
			CGDisplayHideCursor(kCGDirectMainDisplay);

		//[nsText release];
		//[nsCaption release];
	}

	//- restore?
	//- no sense.

	//[pool release];
}

void InitEKeyToKeyCodeArray()
{
	for (int n = 0; n < EKeys_Count; n++)
		eKeyToKeyCode[n] = 0;

	for (int keyCode = 0; keyCode < 1000; keyCode++)
	{
		EKeys eKey = GetEKeyByKeyCode(keyCode);
		if (eKey != (EKeys)0)
			eKeyToKeyCode[eKey] = keyCode;
	}
}

//!!!!было в 3.5
//EXPORT void MacAppNativeWrapper_FullscreenFadeOut(bool exitApplication)
//{
//	CGError cgError;
//
//	// Do the fancy display fading
//	CGDisplayFadeReservationToken fadeReservationToken = 0;
//	cgError = CGAcquireDisplayFadeReservation(kCGMaxDisplayReservationInterval, &fadeReservationToken);
//	if (fadeReservationToken)
//	{
//		cgError = CGDisplayFade(fadeReservationToken, exitApplication ? .5f : 2.0f, kCGDisplayBlendNormal,
//			kCGDisplayBlendSolidColor, 0.0f, 0.0f, 0.0f, /*exitApplication ? false : */true);
//
//		CGReleaseDisplayFadeReservation(fadeReservationToken);
//		fadeReservationToken = 0;
//	}
//
//	// Grab the main display and save it for later.
//	// You could render to any display, but picking what display
//	// to render to could be interesting.
//	if (!exitApplication)
//	{
//		CGCaptureAllDisplays();
//	}
//}

//!!!!было в 3.5
//EXPORT void MacAppNativeWrapper_FullscreenFadeIn(bool exitApplication)
//{
//	//if(exitApplication)
//	//	CGDisplayRelease( kCGDirectMainDisplay );
//
//	CGError cgError;
//
//	CGDisplayFadeReservationToken fadeReservationToken = 0;
//	cgError = CGAcquireDisplayFadeReservation(kCGMaxDisplayReservationInterval, &fadeReservationToken);
//	if (fadeReservationToken)
//	{
//		cgError = CGDisplayFade(fadeReservationToken, 2.0f, kCGDisplayBlendSolidColor,
//			kCGDisplayBlendNormal, 0.0f, 0.0f, 0.0f, exitApplication ? true : false);
//
//		CGReleaseDisplayFadeReservation(fadeReservationToken);
//
//		fadeReservationToken = 0;
//	}
//}

EXPORT void* MacAppNativeWrapper_InitApplicationWindow(bool fullscreen, int windowSizeX, int windowSizeY, const uint16* title)
{
	//NSAutoreleasePool* pool = [[NSAutoreleasePool alloc] init];

	InitEKeyToKeyCodeArray();
	ResetKeyPressedFlags();
	//ResetKeyModifierFlags();

	//[NSApplication sharedApplication];

	mainWindow = [[NSApp delegate]window];

	[NSApp setDelegate : [[[AppDelegate alloc]initDelegate] autorelease] ] ;
	[mainWindow setDelegate : [NSApp delegate] ] ;

	NSString* nsTitle = GetNSStringFromUTF16(title);
	[mainWindow setTitle : nsTitle] ;
	[nsTitle release] ;

	//process events
	{
		NSAutoreleasePool* pool = [[NSAutoreleasePool alloc]init];
		NSEvent* event;
		do
		{
			event = [NSApp nextEventMatchingMask : NSAnyEventMask untilDate : nil inMode : NSDefaultRunLoopMode dequeue : YES];
		} while (event != nil);
		[pool release] ;
	}

	NSRect screenRect = [[NSScreen mainScreen]frame];
	NSSize screenSize = screenRect.size;

	if (!fullscreen)
	{
		if (windowSizeX == screenSize.width && windowSizeY == screenSize.height)
		{
			if ([mainWindow isZoomed] == NO)
				[mainWindow zoom : nil];
		}
		else
		{
			int windowPositionX = (screenSize.width - windowSizeX) / 2;
			int windowPositionY = (screenSize.height - windowSizeY) / 2;

			NSRect frameRect = NSMakeRect(windowPositionX, windowPositionY, windowSizeX, windowSizeY);
			[mainWindow setFrame : frameRect display : YES] ;
		}
	}
	else
	{
		[mainWindow setLevel : NSMainMenuWindowLevel + 1] ;

		int major;
		int minor;
		int bugFix;
		GetOSVersion(&major, &minor, &bugFix);
		if (major == 10 && minor <= 5)
			SetSystemUIMode(kUIModeAllHidden, 0);
		else
			[mainWindow setStyleMask : NSBorderlessWindowMask];

		if (major == 10 && minor >= 7 || (major > 10))
		{
			const int _NSApplicationPresentationAutoHideDock = 1 << 0;
			const int _NSApplicationPresentationAutoHideMenuBar = 1 << 2;
			const int _NSWindowCollectionBehaviorFullScreenPrimary = 1 << 7;

			[[NSApplication sharedApplication]setPresentationOptions:
			_NSApplicationPresentationAutoHideMenuBar | _NSApplicationPresentationAutoHideDock];
			NSWindowCollectionBehavior collection = [mainWindow collectionBehavior];
			collection |= _NSWindowCollectionBehaviorFullScreenPrimary;
			[mainWindow setCollectionBehavior : collection] ;
			//[mainWindow toggleFullScreen : self];
		}

		[mainWindow setBackingType : NSBackingStoreBuffered];
		[mainWindow setOpaque : YES] ;
		[mainWindow setFrame : screenRect display : YES] ;
	}

	[mainWindow setIsVisible : TRUE] ;

	[mainWindow setAcceptsMouseMovedEvents : YES] ;

	[NSEvent startPeriodicEventsAfterDelay : 0.0f withPeriod : 0.01f] ;

	InitGamma();

	//[pool release];

	return mainWindow;
}

EXPORT bool MacAppNativeWrapper_IsWindowVisible()
{
	return[mainWindow isVisible];
}

EXPORT bool MacAppNativeWrapper_IsWindowActive()
{
	return ([mainWindow isKeyWindow] || ([mainWindow level] == NSMainMenuWindowLevel + 1)) &&
		![mainWindow isMiniaturized];
}

EXPORT bool MacAppNativeWrapper_IsWindowFocused()
{
	return ([mainWindow isKeyWindow] || ([mainWindow level] == NSMainMenuWindowLevel + 1)) &&
		![mainWindow isMiniaturized];

	//return [mainWindow isOnActiveSpace] && ![mainWindow isMiniaturized];
}

EXPORT void MacAppNativeWrapper_GetWindowRectangle(Recti* rect)
{
	NSSize screenSize = [[NSScreen mainScreen]frame] .size;

	NSRect frameRect = [mainWindow frame];
	rect->left = frameRect.origin.x;
	rect->top = screenSize.height - (frameRect.origin.y + frameRect.size.height);
	rect->right = rect->left + frameRect.size.width;
	rect->bottom = rect->top + frameRect.size.height;
}

NSRect GetClientRect(bool fullScreen)
{
	if (fullScreen)
	{
		return [[NSScreen mainScreen]frame];
	}
	else
	{
		NSRect frameRect = [mainWindow frame];
		return[mainWindow contentRectForFrameRect : frameRect];
	}
}

EXPORT void MacAppNativeWrapper_GetWindowClientRect(bool fullScreen, Recti* rect)
{
	NSRect clientRect = GetClientRect(fullScreen);

	rect->left = 0;
	rect->top = 0;
	rect->right = clientRect.size.width;
	rect->bottom = clientRect.size.height;
}

EXPORT void MacAppNativeWrapper_GetClientRectangleCursorPosition(bool fullScreen, int* x, int* y)
{
	NSPoint mousePoint = [NSEvent mouseLocation];

	NSRect clientRect = GetClientRect(fullScreen);

	*x = mousePoint.x - clientRect.origin.x;
	*y = clientRect.size.height - (mousePoint.y - clientRect.origin.y);
}

EXPORT void MacAppNativeWrapper_SetClientRectangleCursorPosition(bool fullScreen, int x, int y)
{
	NSSize screenSize = [[NSScreen mainScreen]frame] .size;

	NSRect clientRect = GetClientRect(fullScreen);

	CGPoint cgMousePoint = CGPointMake(
		clientRect.origin.x + x,
		screenSize.height - (clientRect.origin.y + clientRect.size.height) + y);

	CGDisplayMoveCursorToPoint(kCGDirectMainDisplay, cgMousePoint);
}

EXPORT void MacAppNativeWrapper_ShowSystemCursor(bool show)
{
	if (show)
	{
		if (!CGCursorIsVisible())
			CGDisplayShowCursor(kCGDirectMainDisplay);
	}
	else
	{
		if (CGCursorIsVisible())
			CGDisplayHideCursor(kCGDirectMainDisplay);
	}

	//if(visible)
	//	[NSCursor unhide];
	//else
	//	[NSCursor hide];
}

//было в 3.5
//EXPORT double MacAppNativeWrapper_GetSystemTime()
//{
//	struct timeval now;
//	gettimeofday(&now, NULL);
//	return ((double)now.tv_sec) + ((double)now.tv_usec) * .000001;
//
//	//return [NSDate timeIntervalSinceReferenceDate];
//}

//EXPORT void MacAppNativeWrapper_SetWindowTopMost(bool value)
//{
//	if(value)
//	{
//		[mainWindow setLevel:NSMainMenuWindowLevel + 1];
//	}
//	else 
//	{
//		[mainWindow makeKeyAndOrderFront:nil];
//	}
//}

bool GetKeyDataByEvent(NSEvent* event, int* keyCode, int* character, EKeys* eKey)
{
	NSString* str = [(NSEvent*)event characters];
	if (str != nil)
	{
		if ([str length] > 0)
		{
			*keyCode = [event keyCode];
			*character = [str characterAtIndex : 0];
			*eKey = GetEKeyByKeyCode(*keyCode);
			return true;
		}

		//true?
		//[str release];
	}
	return false;
}

EMouseButtons GetEMouseButtonByEvent(NSEvent* event)
{
	switch ([event type])
	{
	case NSLeftMouseDown:
	case NSLeftMouseUp:
		return EMouseButtons_Left;

	case NSRightMouseDown:
	case NSRightMouseUp:
		return EMouseButtons_Right;

	case NSOtherMouseDown:
	case NSOtherMouseUp:
	{
		int number = [event buttonNumber];
		if (number == 2)
			return EMouseButtons_Middle;
		if (number == 3)
			return EMouseButtons_XButton1;
		if (number == 4)
			return EMouseButtons_XButton2;
	}
	break;
	}

	return EMouseButtons_Unknown;
}

//KeyModifierFlags GetKeyModifierFlagByEkey(EKeys eKey)
//{
//	switch(eKey)
//	{
//	case EKeys_LShift:
//		return KeyModifierFlags_LeftShift;
//	case EKeys_RShift:
//		return KeyModifierFlags_RightShift;
//	case EKeys_LControl:
//		return KeyModifierFlags_LeftControl;
//	case EKeys_RControl:
//		return KeyModifierFlags_RightControl;
//	case EKeys_LAlt:
//		return KeyModifierFlags_LeftAlt;
//	case EKeys_RAlt:
//		return KeyModifierFlags_RightAlt;
//	case EKeys_LCommand:
//		return KeyModifierFlags_LeftCommand;
//	case EKeys_RCommand:
//		return KeyModifierFlags_RightCommand;
//	case EKeys_Function:
//		return KeyModifierFlags_Function;
//	}
//
//	return KeyModifierFlags_Unknown;
//}

EKeys GetEKeyByKeyModifierFlag(KeyModifierFlags flag)
{
	switch (flag)
	{
	case KeyModifierFlags_LeftShift:
		return EKeys_LShift;
	case KeyModifierFlags_RightShift:
		return EKeys_RShift;
	case KeyModifierFlags_LeftControl:
		return EKeys_LControl;
	case KeyModifierFlags_RightControl:
		return EKeys_RControl;
	case KeyModifierFlags_LeftAlt:
		return EKeys_LAlt;
	case KeyModifierFlags_RightAlt:
		return EKeys_RAlt;
	case KeyModifierFlags_LeftCommand:
		return EKeys_LCommand;
	case KeyModifierFlags_RightCommand:
		return EKeys_RCommand;
	case KeyModifierFlags_Function:
		return EKeys_Function;
	}

	return (EKeys)0;
}

bool IsModifierFlagEnabled(uint modifierFlags, KeyModifierFlags flag)
{
	switch (flag)
	{
	case KeyModifierFlags_LeftShift:
		if (modifierFlags & NSShiftKeyMask)
			return (modifierFlags & 2) != 0;
		break;

	case KeyModifierFlags_RightShift:
		if (modifierFlags & NSShiftKeyMask)
			return (modifierFlags & 4) != 0;
		break;

	case KeyModifierFlags_LeftControl:
		if (modifierFlags & NSControlKeyMask)
			return true;
		break;

		//no right control
	//case KeyModifierFlags_RightControl:
	//	break;

	case KeyModifierFlags_LeftAlt:
		if (modifierFlags & NSAlternateKeyMask)
			return (modifierFlags & 32) != 0;
		break;

	case KeyModifierFlags_RightAlt:
		if (modifierFlags & NSAlternateKeyMask)
			return (modifierFlags & 64) != 0;
		break;

	case KeyModifierFlags_LeftCommand:
		if (modifierFlags & NSCommandKeyMask)
			return (modifierFlags & 8) != 0;
		break;

	case KeyModifierFlags_RightCommand:
		if (modifierFlags & NSCommandKeyMask)
			return (modifierFlags & 16) != 0;
		break;

	case KeyModifierFlags_Function:
		if (modifierFlags & NSFunctionKeyMask)
			return true;
		break;
	}

	return false;
}

void UpdateKeyModifierKeysAndSendCallbackMessages(uint modifierFlags)
{
	for (int n = 0; n < KeyModifierFlags_Count; n++)
	{
		KeyModifierFlags flag = (KeyModifierFlags)n;

		bool pressed = IsModifierFlagEnabled(modifierFlags, flag);

		EKeys eKey = GetEKeyByKeyModifierFlag(flag);

		//if(keyModifierFlags[n] != pressed)
		if (keysPressed[eKey] != pressed)
		{
			//keyModifierFlags[n] = pressed;
			keysPressed[eKey] = pressed;

			//EKeys eKey = GetEKeyByKeyModifierFlag(flag);

			if (pressed)
				MessageEvent(MessageTypes_KeyDown, eKey, 0);
			else
				MessageEvent(MessageTypes_KeyUp, eKey, 0);
		}
	}

	capsLockPressed = (modifierFlags & NSAlphaShiftKeyMask) != 0;
}

EXPORT bool MacAppNativeWrapper_ProcessEvents()
{
	NSAutoreleasePool* pool = [[NSAutoreleasePool alloc]init];

	NSEvent* event;

	do
	{
		event = [NSApp nextEventMatchingMask : NSAnyEventMask untilDate : nil inMode : NSDefaultRunLoopMode dequeue : YES];

		if (event != nil)
		{
			switch ([event type])
			{
			case NSLeftMouseDown:
			case NSRightMouseDown:
			case NSOtherMouseDown:
			{
				EMouseButtons button = GetEMouseButtonByEvent(event);
				if (button != EMouseButtons_Unknown)
				{
					MessageEvent(MessageTypes_MouseDown, button);
					if ([event clickCount] == 2)
						MessageEvent(MessageTypes_MouseDoubleClick, button);
				}
			}
			break;

			case NSLeftMouseUp:
			case NSRightMouseUp:
			case NSOtherMouseUp:
			{
				EMouseButtons button = GetEMouseButtonByEvent(event);
				if (button != EMouseButtons_Unknown)
					MessageEvent(MessageTypes_MouseUp, button);
			}
			break;

			case NSScrollWheel:
			{
				float delta = [event deltaY] * 10.0f;
				MessageEvent(MessageTypes_MouseWheel, (int)delta);
			}
			break;

			case NSMouseMoved:
			case NSLeftMouseDragged:
			case NSRightMouseDragged:
			case NSOtherMouseDragged:
			{
				if (ignoreMouseMoveDeltaCounter > 0)
				{
					ignoreMouseMoveDeltaCounter--;
					mouseMoveDeltaX = 0;
					mouseMoveDeltaY = 0;
				}

				if (ignoreMouseMoveDeltaCounter == 0)
				{
					mouseMoveDeltaX += (int)[event deltaX];
					mouseMoveDeltaY += (int)[event deltaY];
					MessageEvent(MessageTypes_MouseMove);
				}
			}
			break;

			case NSKeyDown:
			{
				int keyCode;
				int character;
				EKeys eKey;
				if (GetKeyDataByEvent(event, &keyCode, &character, &eKey))
				{
					keysPressed[eKey] = true;
					MessageEvent(MessageTypes_KeyDown, eKey, character);
				}

				goto skipSendEvent;
			}
			break;

			case NSKeyUp:
			{
				int keyCode;
				int character;
				EKeys eKey;
				if (GetKeyDataByEvent(event, &keyCode, &character, &eKey))
				{
					keysPressed[eKey] = false;
					MessageEvent(MessageTypes_KeyUp, eKey, character);
				}

				goto skipSendEvent;
			}
			break;

			case NSFlagsChanged:
			{
				NSUInteger modifierFlags = [event modifierFlags];
				UpdateKeyModifierKeysAndSendCallbackMessages(modifierFlags);
			}
			break;

			case NSPeriodic:
				MessageEvent(MessageTypes_Periodic);
				break;
			}

			[NSApp sendEvent : event];

		skipSendEvent:;

			//[event release];
		}

	} while (event != nil);

	[pool release] ;

	return !needQuitMessageLoop;
}

EXPORT void MacAppNativeWrapper_ShutdownApplicationWindow()
{
}

EXPORT int MacAppNativeWrapper_GetWindowState()
{
	if ([mainWindow isMiniaturized] == YES)
		return WindowState_Minimized;

	if ([mainWindow isZoomed] == YES)
		return WindowState_Maximized;

	return WindowState_Normal;
}

EXPORT void MacAppNativeWrapper_SetWindowState(int state)
{
	switch (state)
	{
	case WindowState_Maximized:
		if ([mainWindow isMiniaturized] == YES)
			[mainWindow deminiaturize : nil];
		if ([mainWindow isZoomed] == NO)
			[mainWindow zoom : nil];
		[mainWindow makeKeyAndOrderFront : nil] ;
		break;

	case WindowState_Minimized:
		if ([mainWindow isMiniaturized] == NO)
			[mainWindow miniaturize : nil];
		break;

	case WindowState_Normal:
		if ([mainWindow isMiniaturized] == YES)
			[mainWindow deminiaturize : nil];
		if ([mainWindow isZoomed] == YES)
			[mainWindow zoom : nil];
		[mainWindow makeKeyAndOrderFront : nil] ;
		break;
	}
}

EXPORT void MacAppNativeWrapper_Initialize(CallbackMessageEventFunction* messageEvent,
	CallbackLogInfoFunction* logInfo, CallbackLogWarningFunction* logWarning,
	CallbackLogFatalFunction* logFatal)
{
	callbackMessageEvent = messageEvent;
	callbackLogInfo = logInfo;
	callbackLogWarning = logWarning;
	callbackLogFatal = logFatal;
}

//int GetBitDepthFromDisplayMode(CGDisplayModeRef displayMode)
//{
//	CFStringRef pixEnc = CGDisplayModeCopyPixelEncoding(displayMode);
//	
//	if(CFStringCompare(pixEnc, CFSTR(IO32BitDirectPixels), kCFCompareCaseInsensitive) == kCFCompareEqualTo)
//		return 32;	
//	if(CFStringCompare(pixEnc, CFSTR(IO16BitDirectPixels), kCFCompareCaseInsensitive) == kCFCompareEqualTo)
//		return 16;	
//	if(CFStringCompare(pixEnc, CFSTR(IO8BitIndexedPixels), kCFCompareCaseInsensitive) == kCFCompareEqualTo)
//		return 8;
//	return 0;
//}

//bool IsDisplayModesEqual(CGDisplayModeRef displayMode1, CGDisplayModeRef displayMode2)
//{
//	if(CGDisplayModeGetWidth(displayMode1) != CGDisplayModeGetWidth(displayMode2))
//		return false;	
//	if(CGDisplayModeGetHeight(displayMode1) != CGDisplayModeGetHeight(displayMode2))
//		return false;	
//	if(GetBitDepthFromDisplayMode(displayMode1) != GetBitDepthFromDisplayMode(displayMode2))
//		return false;	
//	return true;
//}

EXPORT void MacAppNativeWrapper_GetScreenSize(int* width, int* height)
{
	NSRect rect = [[NSScreen mainScreen]frame];
	*width = rect.size.width;
	*height = rect.size.height;
}

EXPORT int MacAppNativeWrapper_GetScreenBitsPerPixel()
{
	int depth = NSBitsPerPixelFromDepth([[NSScreen mainScreen]depth] );
	if (depth == 24)
		depth = 32;
	if (depth == 15)
		depth = 16;
	return depth;

	//CGDisplayModeRef mode = CGDisplayCopyDisplayMode(kCGDirectMainDisplay);
	//return GetBitDepthFromDisplayMode(mode);
}

int GetNumberValueForKey(CFDictionaryRef desc, CFStringRef key)
{
	CFNumberRef value = (CFNumberRef)CFDictionaryGetValue(desc, key);
	if (value == NULL)
		return 0;
	int num = 0;
	CFNumberGetValue(value, kCFNumberIntType, &num);
	return num;
}

EXPORT void MacAppNativeWrapper_GetVideoModes(int* outCount, Vec3i** outArray)
{
	*outCount = 0;
	*outArray = NULL;

	CFArrayRef modes = CGDisplayAvailableModes(kCGDirectMainDisplay);
	//CFArrayRef modes = CGDisplayCopyAllDisplayModes(kCGDirectMainDisplay, NULL);

	if (modes != NULL)
	{
		int arrayCount = CFArrayGetCount(modes);

		int count = 0;
		Vec3i* array = (Vec3i*)malloc(arrayCount * sizeof(Vec3i));

		for (int n = 0; n < arrayCount; n++)
		{
			CFDictionaryRef mode = (CFDictionaryRef)CFArrayGetValueAtIndex(modes, n);
			//CGDisplayModeRef mode = (CGDisplayModeRef)CFArrayGetValueAtIndex(modes, n);

			Vec3i item;
			item.x = GetNumberValueForKey(mode, kCGDisplayWidth);
			item.y = GetNumberValueForKey(mode, kCGDisplayHeight);
			item.z = GetNumberValueForKey(mode, kCGDisplayBitsPerPixel);
			//item.x = CGDisplayModeGetWidth(mode);
			//item.y = CGDisplayModeGetHeight(mode);
			//item.z = GetBitDepthFromDisplayMode(mode);

			array[count] = item;
			count++;
		}

		*outCount = count;
		*outArray = array;
	}
}

CFDictionaryRef GetDisplayMode(int width, int height, int colorDepth)
{
	CFArrayRef modes = CGDisplayAvailableModes(kCGDirectMainDisplay);
	//CFArrayRef modes = CGDisplayCopyAllDisplayModes(kCGDirectMainDisplay, NULL);

	if (modes != NULL)
	{
		int count = CFArrayGetCount(modes);
		for (int n = 0; n < count; n++)
		{
			CFDictionaryRef mode = (CFDictionaryRef)CFArrayGetValueAtIndex(modes, n);
			//CGDisplayModeRef mode = (CGDisplayModeRef)CFArrayGetValueAtIndex(modes, n);

			int x = GetNumberValueForKey(mode, kCGDisplayWidth);
			int y = GetNumberValueForKey(mode, kCGDisplayHeight);
			int bpp = GetNumberValueForKey(mode, kCGDisplayBitsPerPixel);
			//int x = CGDisplayModeGetWidth(mode);
			//int y = CGDisplayModeGetHeight(mode);
			//int bpp = GetBitDepthFromDisplayMode(mode);

			if (width == x && height == y && bpp == colorDepth)
				return mode;
		}
	}

	return NULL;
}

bool ChangeVideoMode(int width, int height, int bpp)
{
	int currentWidth;
	int currentHeight;
	int currentBPP;
	MacAppNativeWrapper_GetScreenSize(&currentWidth, &currentHeight);
	currentBPP = MacAppNativeWrapper_GetScreenBitsPerPixel();

	if (currentWidth != width || currentHeight != height || currentBPP != bpp)
	{
		CFDictionaryRef mode = GetDisplayMode(width, height, bpp);
		if (mode == NULL)
			return false;

		CGError error = CGDisplaySwitchToMode(kCGDirectMainDisplay, mode);
		if (error != kCGErrorSuccess)
			return false;
	}

	return true;
}

EXPORT bool MacAppNativeWrapper_ChangeVideoMode(int width, int height, int bpp)
{
	if (!needRestoreVideoMode)
	{
		needRestoreVideoMode = true;
		MacAppNativeWrapper_GetScreenSize(&initialScreenSizeX, &initialScreenSizeY);
		initialScreenBPP = MacAppNativeWrapper_GetScreenBitsPerPixel();
	}

	if (!ChangeVideoMode(width, height, bpp))
		return false;

	return true;
}

EXPORT bool MacAppNativeWrapper_RestoreVideoMode()
{
	if (needRestoreVideoMode)
	{
		ChangeVideoMode(initialScreenSizeX, initialScreenSizeY, initialScreenBPP);
		needRestoreVideoMode = false;
	}
}

//EXPORT int MacAppNativeWrapper_GetDisplayCount()
//{
//	uint32_t maxScreens = 32;
//	CGDirectDisplayID activeDisplays[maxScreens];
//	uint32_t displayCount;
//	
//	CGGetActiveDisplayList(maxScreens, activeDisplays, &displayCount);
//	
//	return (int)displayCount;
//}

EXPORT bool MacAppNativeWrapper_IsKeyPressed(EKeys eKey)
{
	return keysPressed[eKey];
}

EXPORT bool MacAppNativeWrapper_IsKeyLocked(EKeys eKey)
{
	if (eKey == EKeys_CapsLock)
		return capsLockPressed;
	return false;
}

EXPORT bool MacAppNativeWrapper_IsSystemKey(EKeys eKey)
{
	int keyCode = GetKeyCodeByEKey(eKey);
	if (keyCode == 0)
		return false;
	return IsSystemKey(keyCode);
}

EXPORT bool MacAppNativeWrapper_IsMouseButtonPressed(int buttonCode)
{
	return CGEventSourceButtonState(kCGEventSourceStateCombinedSessionState, buttonCode);
}

EXPORT void MacAppNativeWrapper_FreeMemory(void* buffer)
{
	free(buffer);
}

//EXPORT void MacAppNativeWrapper_SetWindowPosition(int x, int y)
//{
//!
//	int screenHeight = [[NSScreen mainScreen] frame].size.height;
//	int windowWidth = [mainWindow frame].size.width;
//	int windowHeight = [mainWindow frame].size.height;
//	
//	[mainWindow setFrameOrigin:NSMakePoint(x, screenHeight - y - windowHeight)];
//}

EXPORT void MacAppNativeWrapper_SetWindowSize(int width, int height)
{
	//only for windowed mode

	NSRect oldFrame = [mainWindow frame];

	int diffX = width - oldFrame.size.width;
	int diffY = height - oldFrame.size.height;

	int newPositionX = oldFrame.origin.x - diffX / 2;
	if (newPositionX < 0)
		newPositionX = 0;
	int newPositionY = oldFrame.origin.y - diffY / 2;
	if (newPositionY < 0)
		newPositionY = 0;

	NSRect newFrame = NSMakeRect(newPositionX, newPositionY, width, height);
	[mainWindow setFrame : newFrame display : YES animate : YES] ;
}

//EXPORT void MacAppNativeWrapper_SetWindowBorderStyle( WindowBorderStyles style )
//{
//}

EXPORT void MacAppNativeWrapper_SetWindowRectangle(int left, int top, int right, int bottom)
{
	int width = right - left;
	int height = bottom - top;

	NSRect newFrame = NSMakeRect(left, top, width, height);
	[mainWindow setFrame : newFrame display : YES animate : YES] ;
}

EXPORT void MacAppNativeWrapper_SetWindowTitle(const uint16* title)
{
	NSAutoreleasePool* pool = [[NSAutoreleasePool alloc]init];

	NSString* nsTitle = GetNSStringFromUTF16(title);
	[mainWindow setTitle : nsTitle] ;
	//[nsTitle release];

	[pool release] ;
}

EXPORT void MacAppNativeWrapper_GetMouseMoveDelta(int* x, int* y)
{
	*x = mouseMoveDeltaX;
	*y = mouseMoveDeltaY;
}

EXPORT void MacAppNativeWrapper_ResetMouseMoveDelta(bool resetIgnoreCounter)
{
	mouseMoveDeltaX = 0;
	mouseMoveDeltaY = 0;

	if (resetIgnoreCounter)
		ignoreMouseMoveDeltaCounter = 3;
}

EXPORT void* MacAppNativeWrapper_LoadLibrary(const uint16* path)
{
	NSAutoreleasePool* pool = [[NSAutoreleasePool alloc]init];

	NSString* nsPath = GetNSStringFromUTF16(path);
	void* handle = dlopen((char*)[nsPath cStringUsingEncoding : NSUTF8StringEncoding], RTLD_LAZY | RTLD_GLOBAL);

	[pool release] ;

	return handle;
}
//EXPORT void* MacAppNativeWrapper_LoadLibrary(char* path)
//{
//	return dlopen( path, RTLD_LAZY | RTLD_GLOBAL);
//}

EXPORT void* MacAppNativeWrapper_CallCustomPlatformSpecificMethod(const uint16* message, void* param)
{
	NSString* nsMessage = GetNSStringFromUTF16(message);

	if ([nsMessage compare : @"GetSystemLanguageName"] == NSOrderedSame)
	{
		NSAutoreleasePool* pool = [[NSAutoreleasePool alloc]init];

		NSArray* languages = [NSLocale preferredLanguages];
		NSString* languageName = [languages objectAtIndex : 0];

		uint16* result = CreateOutString(languageName);

		[pool release] ;

		return result;
	}

	if ([nsMessage compare : @"GetSystemLanguageEnglishName"] == NSOrderedSame)
	{
		NSAutoreleasePool* pool = [[NSAutoreleasePool alloc]init];

		NSArray* languages = [NSLocale preferredLanguages];
		NSString* languageName = [languages objectAtIndex : 0];
		NSLocale* usLocale = [[[NSLocale alloc]initWithLocaleIdentifier:@"en_US"] autorelease];
		NSString* displayName = [usLocale displayNameForKey : NSLocaleIdentifier value : languageName];

		uint16* result = CreateOutString(displayName);

		[pool release] ;

		return result;
	}

	if ([nsMessage compare : @"MyMessage"] == NSOrderedSame)
	{
		//do something

		return NULL;
	}

	return NULL;
}

EXPORT void MacAppNativeWrapper_GetLoadedBundleNames(void*** outList, int* outCount)
{
	NSAutoreleasePool* pool = [[NSAutoreleasePool alloc]init];

	CFArrayRef allBundles = CFBundleGetAllBundles();

	int count = CFArrayGetCount(allBundles);
	void** list = (void**)malloc(sizeof(void*) * count);

	for (int n = 0; n < count; n++)
	{
		CFBundleRef bundle = (CFBundleRef)CFArrayGetValueAtIndex(allBundles, n);

		CFURLRef bundleURL = CFBundleCopyBundleURL(bundle);
		NSString* bundlePath = (NSString*)CFURLCopyFileSystemPath(bundleURL, kCFURLPOSIXPathStyle);

		list[n] = CreateOutString(bundlePath);
	}

	*outList = list;
	*outCount = count;

	[pool release] ;
}

EXPORT void MacAppNativeWrapper_UpdateAcceptsMouseMovedEventsFlag()
{
	[mainWindow setAcceptsMouseMovedEvents : YES] ;
}

EXPORT void MacAppNativeWrapper_UpdateWindowForProcessChangingVideoMode()
{
	NSRect screenRect = [[NSScreen mainScreen]frame];
	NSSize screenSize = screenRect.size;

	CGCaptureAllDisplays();

	[mainWindow setLevel : NSMainMenuWindowLevel + 1] ;
	[mainWindow setFrame : screenRect display : YES] ;
}

EXPORT void MacAppNativeWrapper_MinimizeWindow()
{
	[mainWindow miniaturize : nil] ;
}

EXPORT void MacAppNativeWrapper_ActivateFullscreenMinimizedMode()
{
	int major;
	int minor;
	int bugFix;
	GetOSVersion(&major, &minor, &bugFix);
	if (major == 10 && minor <= 5)
		SetSystemUIMode(kUIModeNormal, 0);

	if (needRestoreVideoMode)
		ChangeVideoMode(initialScreenSizeX, initialScreenSizeY, initialScreenBPP);

	fullscreenMinimizedMode_CGLContextObj = CGLGetCurrentContext();
	CGLSetCurrentContext(NULL);
	CGLClearDrawable(fullscreenMinimizedMode_CGLContextObj);

	CGReleaseAllDisplays();

	if (!CGCursorIsVisible())
		CGDisplayShowCursor(kCGDirectMainDisplay);
}

EXPORT void MacAppNativeWrapper_RestoreFromFullscreenMinimizedMode(int width, int height)
{
	int major;
	int minor;
	int bugFix;
	GetOSVersion(&major, &minor, &bugFix);
	if (major == 10 && minor <= 5)
		SetSystemUIMode(kUIModeAllHidden, 0);

	CGCaptureAllDisplays();

	int bpp;
	if (needRestoreVideoMode)
		bpp = initialScreenBPP;
	else
		bpp = MacAppNativeWrapper_GetScreenBitsPerPixel();
	ChangeVideoMode(width, height, bpp);

	if (fullscreenMinimizedMode_CGLContextObj != NULL)
	{
		CGLSetCurrentContext(fullscreenMinimizedMode_CGLContextObj);
		CGLSetFullScreen(fullscreenMinimizedMode_CGLContextObj);
		fullscreenMinimizedMode_CGLContextObj = NULL;
	}

	NSRect screenRect = [[NSScreen mainScreen]frame];
	[mainWindow setLevel : NSMainMenuWindowLevel + 1] ;
	[mainWindow setFrame : screenRect display : YES] ;
}

void InitGamma()
{
	CGDisplayRestoreColorSyncSettings();

	uint32_t count;
	CGError cgError = CGGetDisplayTransferByTable(kCGDirectMainDisplay, 256,
		initialGammaRedTable, initialGammaGreenTable, initialGammaBlueTable, &count);
	if (cgError == kCGErrorSuccess)
		initialGammaInitialized = true;
}

EXPORT void MacAppNativeWrapper_SetGamma(float value)
{
	CGError cgError;

	if (value != 1.0f)
	{
		if (initialGammaInitialized)
		{
			CGGammaValue redTable[256];
			CGGammaValue greenTable[256];
			CGGammaValue blueTable[256];
			for (int n = 0; n < 256; n++)
			{
				redTable[n] = initialGammaRedTable[n] * value;
				greenTable[n] = initialGammaGreenTable[n] * value;
				blueTable[n] = initialGammaBlueTable[n] * value;
			}

			CGSetDisplayTransferByTable(kCGDirectMainDisplay, 256, redTable, greenTable, blueTable);
		}
	}
	else
	{
		CGDisplayRestoreColorSyncSettings();
	}
}

EXPORT int MacAppNativeWrapper_GetActiveDisplayList(int bufferLength, uint* buffer)
{
	uint count;
	if (CGGetActiveDisplayList(bufferLength, buffer, &count) != CGDisplayNoErr)
		return 0;
	return count;
}

EXPORT void MacAppNativeWrapper_GetDisplayInfo(uint display, uint16** deviceName, Recti* bounds, Recti* workingArea, bool* primary)
{
	Recti rect = ConvertToRecti(CGDisplayBounds(display));

	char name[256];
	sprintf(name, "%d", (int)CGDisplayUnitNumber(display));
	*deviceName = CreateOutString(name);
	*bounds = rect;
	*workingArea = rect;
	*primary = CGDisplayIsMain(display);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

EXPORT FILE* MacAppNativeWrapper_VirtualFileStream_Open(const uint16* path)
{
	NSAutoreleasePool* pool = [[NSAutoreleasePool alloc]init];

	NSString* nsPath = GetNSStringFromUTF16(path);
	FILE* file = fopen((char*)[nsPath cStringUsingEncoding : NSUTF8StringEncoding], "rb");

	[pool release] ;

	return file;
}

EXPORT void MacAppNativeWrapper_VirtualFileStream_Close(FILE* handle)
{
	fclose(handle);
}

EXPORT int MacAppNativeWrapper_VirtualFileStream_Length(FILE* handle)
{
	int pos = ftell(handle);
	fseek(handle, 0, SEEK_END);
	int size = ftell(handle);
	fseek(handle, pos, SEEK_SET);
	return size;
}

EXPORT int MacAppNativeWrapper_VirtualFileStream_Read(FILE* handle, void* buffer, int count)
{
	return fread(buffer, 1, count, handle);
}

EXPORT int MacAppNativeWrapper_VirtualFileStream_Seek(FILE* handle, int offset, int origin)
{
	return fseek(handle, offset, origin);
}

//EXPORT void UtilsNativeWrapper_GetOSVersion(int* major, int* minor, int* bugFix)
//{
//	SInt32 nmajor = 0;
//	SInt32 nminor = 0;
//	SInt32 nbugFix = 0;
//	Gestalt(gestaltSystemVersionMajor, &nmajor);
//	Gestalt(gestaltSystemVersionMinor, &nminor);
//	Gestalt(gestaltSystemVersionBugFix, &nbugFix);
//	*major = nmajor;
//	*minor = nminor;
//	*bugFix = nbugFix;
//}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

@implementation AppDelegate

- (id)initDelegate
{
	self = [super init];
	return (self);
}

- (NSApplicationTerminateReply)applicationShouldTerminate:(NSApplication*)app
{
	needQuitMessageLoop = true;

	return NSTerminateNow;
}

- (void)windowWillClose:(id)sender
{
	needQuitMessageLoop = true;
}

- (void)windowDidResize : (NSNotification*)aNotification
{
	MessageEvent(MessageTypes_WindowDidResize);
}

- (void)windowDidBecomeKey : (NSNotification*)aNotification
{
	MessageEvent(MessageTypes_WindowDidBecomeKey);

	mouseMoveDeltaX = 0;
	mouseMoveDeltaY = 0;
	ignoreMouseMoveDeltaCounter = 3;

}

- (void)windowDidResignKey:(NSNotification*)aNotification
{
	ResetKeyPressedFlags();
	MessageEvent(MessageTypes_WindowDidResignKey);

	mouseMoveDeltaX = 0;
	mouseMoveDeltaY = 0;
	ignoreMouseMoveDeltaCounter = 3;
}

- (void)windowWillMiniaturize:(NSNotification*)aNotification
{
	MessageEvent(MessageTypes_WindowWillMiniaturize);
}

- (void)windowDidMiniaturize : (NSNotification*)aNotification
{
	MessageEvent(MessageTypes_WindowDidMiniaturize);
}

- (void)windowDidDeminiaturize : (NSNotification*)aNotification
{
	MessageEvent(MessageTypes_WindowDidDeminiaturize);

	mouseMoveDeltaX = 0;
	mouseMoveDeltaY = 0;
	ignoreMouseMoveDeltaCounter = 3;
}

#endif