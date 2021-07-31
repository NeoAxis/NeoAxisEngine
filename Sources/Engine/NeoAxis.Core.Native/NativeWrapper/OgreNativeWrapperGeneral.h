// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

#ifdef _WIN32
	#include <windows.h>
	#include <objbase.h>
#endif

#include "Ogre.h"

#include "OgreArchive.h"
#include "OgreArchiveFactory.h"

#define super __super

#ifdef _WIN32
	#define INLINE __forceinline
	#define EXPORT extern "C" __declspec(dllexport)
#else
	#define INLINE __inline__ __attribute__((__always_inline__))
	#define EXPORT extern "C" __attribute__ ((visibility("default")))
#endif

#define SAFE_DELETE(q){if(q){delete q;q=NULL;}else 0;}

extern Ogre::wchar16* CreateOutString(const Ogre::WString& str);
extern Ogre::wchar16* CreateOutString(const Ogre::String& str);

//#ifdef _DEBUG
//#error Debug version are not supported.
//#endif

using Ogre::uint;
