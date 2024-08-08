/*
-----------------------------------------------------------------------------
This source file is part of OGRE
    (Object-oriented Graphics Rendering Engine)
For the latest info, see http://www.ogre3d.org/

Copyright (c) 2000-2006 Torus Knot Software Ltd
Also see acknowledgements in Readme.html

This program is free software; you can redistribute it and/or modify it under
the terms of the GNU Lesser General Public License as published by the Free Software
Foundation; either version 2 of the License, or (at your option) any later
version.

This program is distributed in the hope that it will be useful, but WITHOUT
ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with
this program; if not, write to the Free Software Foundation, Inc., 59 Temple
Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/lesser.txt.

You may alternatively use this source under the terms of a specific version of
the OGRE Unrestricted License provided you have obtained such a license from
Torus Knot Software Ltd.
-----------------------------------------------------------------------------
*/
#ifndef __NULLPREREQUISITES_H__
#define __NULLPREREQUISITES_H__

#include "OgrePrerequisites.h"

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif
#ifndef NOMINMAX
#define NOMINMAX // required to stop windows.h messing up std::min
#endif


namespace Ogre
{
	// Predefine classes
	class NULLRenderSystem;
	class NULLRenderWindow;
	class NULLTexture;
	class NULLTextureManager;
	class NULLGpuProgram;
	class NULLGpuProgramManager;
    class NULLHardwareBufferManager;
    class NULLHardwareIndexBuffer;
    class NULLHLSLProgramFactory;

// Should we ask to manage vertex/index buffers automatically?
// Doing so avoids lost devices, but also has a performance impact
// which is unacceptably bad when using very large buffers
#define OGRE_NULL_MANAGE_BUFFERS 1

    //-------------------------------------------
	// Windows setttings
	//-------------------------------------------
#if (OGRE_PLATFORM == OGRE_PLATFORM_WIN32) && !defined(OGRE_STATIC_LIB)
#	ifdef OGRENULLENGINEDLL_EXPORTS
#		define _OgreNULLExport __declspec(dllexport)
#	else
#       if defined( __MINGW32__ )
#           define _OgreNULLExport
#       else
#    		define _OgreNULLExport __declspec(dllimport)
#       endif
#	endif
#else
#	define _OgreNULLExport
#endif	// OGRE_WIN32
}
#endif
