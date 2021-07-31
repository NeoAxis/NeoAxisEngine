/*
-----------------------------------------------------------------------------
This source file is part of OGRE
    (Object-oriented Graphics Rendering Engine)
For the latest info, see http://www.ogre3d.org/

Copyright (c) 2000-2009 Torus Knot Software Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
-----------------------------------------------------------------------------
*/
#include "OgreStableHeaders.h"
#include "OgreDynLib.h"
#include "OgreException.h"
#include "OgreLogManager.h"
#include "OgreRoot.h"

#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32 || OGRE_PLATFORM == OGRE_PLATFORM_WINRT
#   define WIN32_LEAN_AND_MEAN
#  if !defined(NOMINMAX) && defined(_MSC_VER)
#  define NOMINMAX // required to stop windows.h messing up std::min
#  endif
#   include <windows.h>
#endif

//#if OGRE_PLATFORM == OGRE_PLATFORM_WINRT
//#include "OgreUTFString.h"
//#endif

#if OGRE_PLATFORM == OGRE_PLATFORM_APPLE || OGRE_PLATFORM == OGRE_PLATFORM_APPLE_IOS
//!!!!was: #   include "macPlugins.h"
#   include "macUtils.h"
#endif
#if OGRE_PLATFORM == OGRE_PLATFORM_APPLE || OGRE_PLATFORM == OGRE_PLATFORM_APPLE_IOS || OGRE_PLATFORM == OGRE_PLATFORM_NACL 
#   include <dlfcn.h>
#endif

namespace Ogre {

    //-----------------------------------------------------------------------
    DynLib::DynLib( const String& name )
    {
        mName = name;
        m_hInst = NULL;
    }

    //-----------------------------------------------------------------------
    DynLib::~DynLib()
    {
    }

    //-----------------------------------------------------------------------
    void DynLib::load()
    {
        // Log library load
        root->mLogManager->logMessage("Loading library " + mName);

		String name = mName;
//#if OGRE_PLATFORM == OGRE_PLATFORM_LINUX
//        // dlopen() does not add .so to the filename, like windows does for .dll
//        if (name.substr(name.length() - 3, 3) != ".so")
//           name += ".so";
//#endif

		WString path = root->nativeLibrariesDirectory;

		if(path[path.length() - 1] != L'/' && path[path.length() - 1] != L'\\')
		{
#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32 || OGRE_PLATFORM == OGRE_PLATFORM_WINRT
			path += L"\\";
#else
			path += L"/";
#endif
		}
		path += StringUtil::toUTFWide(name);

#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32 || OGRE_PLATFORM == OGRE_PLATFORM_WINRT
		m_hInst = (DYNLIB_HANDLE)DYNLIB_LOAD( path.c_str() );
#else
		m_hInst = (DYNLIB_HANDLE)DYNLIB_LOAD( StringUtil::toUTF8(path).c_str() );
#endif
        //m_hInst = (DYNLIB_HANDLE)DYNLIB_LOAD( name.c_str() );

        if( !m_hInst )
            OGRE_EXCEPT(
                Exception::ERR_INTERNAL_ERROR, 
                "Could not load dynamic library " + mName + 
                ".  System Error: " + dynlibError(),
                "DynLib::load" );
    }

    //-----------------------------------------------------------------------
    void DynLib::unload()
    {
        // Log library unload
        root->mLogManager->logMessage("Unloading library " + mName);

        if( DYNLIB_UNLOAD( m_hInst ) )
		{
            OGRE_EXCEPT(
                Exception::ERR_INTERNAL_ERROR, 
                "Could not unload dynamic library " + mName +
                ".  System Error: " + dynlibError(),
                "DynLib::unload");
		}

    }

    //-----------------------------------------------------------------------
    void* DynLib::getSymbol( const String& strName ) const throw()
    {
        return (void*)DYNLIB_GETSYM( m_hInst, strName.c_str() );
    }
    //-----------------------------------------------------------------------
    String DynLib::dynlibError( void ) 
    {
#if OGRE_PLATFORM == OGRE_PLATFORM_WIN32
        LPVOID lpMsgBuf; 
        FormatMessage( 
            FORMAT_MESSAGE_ALLOCATE_BUFFER | 
            FORMAT_MESSAGE_FROM_SYSTEM | 
            FORMAT_MESSAGE_IGNORE_INSERTS, 
            NULL, 
            GetLastError(), 
            MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), 
            (LPTSTR) &lpMsgBuf, 
            0, 
            NULL 
            ); 
        String ret = (char*)lpMsgBuf;
        // Free the buffer.
        LocalFree( lpMsgBuf );
        return ret;
#elif OGRE_PLATFORM == OGRE_PLATFORM_WINRT
		WCHAR wideMsgBuf[1024];
		if (0 == FormatMessageW(
			FORMAT_MESSAGE_FROM_SYSTEM |
			FORMAT_MESSAGE_IGNORE_INSERTS,
			NULL,
			GetLastError(),
			MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
			wideMsgBuf,
			sizeof(wideMsgBuf) / sizeof(wideMsgBuf[0]),
			NULL
		))
		{
			wideMsgBuf[0] = 0;
		}
		String ret = StringUtil::toUTF8(wideMsgBuf);
		return ret;
#elif OGRE_PLATFORM == OGRE_PLATFORM_LINUX || OGRE_PLATFORM == OGRE_PLATFORM_TEGRA2
        return String(dlerror());
#elif OGRE_PLATFORM == OGRE_PLATFORM_APPLE
        return String(MacDybLibGetError());
#else
        return String("");
#endif
    }

}
