/*
-----------------------------------------------------------------------------
This source file is part of OGRE
    (Object-oriented Graphics Rendering Engine)
For the latest info, see http://www.ogre3d.org

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
#include "NULLRenderSystem.h"
#include "NULLPrerequisites.h"
#include "OgreLogManager.h"
#include "OgreMath.h"
//#include "NULLHLSLProgramFactory.h"
//#include "OgreFrustum.h"
#include "OgreRoot.h"

#ifdef _WIN32
#include <windows.h>
#endif
#ifdef ANDROID
#include <android/log.h>
#endif

//void Fatal(const char* text)
//{
//#ifdef _WIN32
//	MessageBox(NULL, text, "Fatal", MB_OK | MB_ICONEXCLAMATION);
//	exit(0);
//#endif//_WIN32
//
////!!!!!dr
//#ifdef ANDROID
//	char tempString[4096];
//	sprintf(tempString, "RenderSystemNULL fatal: %s", text);
//	__android_log_write(ANDROID_LOG_ERROR,"NeoAxis Engine", tempString);
//	int* x = 0;
//	*x = 42;
//#endif//ANDROID
//}

namespace Ogre 
{

	//---------------------------------------------------------------------
	NULLRenderSystem::NULLRenderSystem(Root* root)
		: RenderSystem()
	{
		//root->mLogManager->logMessage( "NULL : " + getName() + " created." );

        //mHLSLProgramFactory = NULL;

		// set config options defaults
		initConfigOptions();
	}

	//---------------------------------------------------------------------

	NULLRenderSystem::~NULLRenderSystem()
	{
        shutdown();

        //// Deleting the HLSL program factory
        //if (mHLSLProgramFactory)
        //{
        //    // Remove from manager safely
        //    if (HighLevelGpuProgramManager::getSingletonPtr())
        //        HighLevelGpuProgramManager::getSingleton().removeFactory(mHLSLProgramFactory);
        //    delete mHLSLProgramFactory;
        //    mHLSLProgramFactory = 0;
        //}

		//root->mLogManager->logMessage( "NULL : " + getName() + " destroyed." );
	}

	//---------------------------------------------------------------------

	const String& NULLRenderSystem::getName() const
	{
		static String strName( "NULL Rendering Subsystem");
		return strName;
	}

	//---------------------------------------------------------------------

	void NULLRenderSystem::initConfigOptions()
	{	}

	//---------------------------------------------------------------------

	void NULLRenderSystem::setConfigOption( const String &name, const String &value )
	{ }

	//---------------------------------------------------------------------

	String NULLRenderSystem::validateConfigOptions()
	{
        return StringUtil::BLANK;
	}

	//---------------------------------------------------------------------

	ConfigOptionMap& NULLRenderSystem::getConfigOptions()
	{
		// return a COPY of the current config options
		return mOptions;
	}

	//---------------------------------------------------------------------

	//void NULLRenderSystem::reinitialise()
	//{
	//	root->mLogManager->logMessage( "NULL : Reinitialising" );
	//	this->shutdown();
	//	this->_initialise( true );
	//}

	//---------------------------------------------------------------------

	void NULLRenderSystem::shutdown()
	{
		RenderSystem::shutdown();
		root->mLogManager->logMessage("NULL : Shutting down cleanly.");
	}

	//---------------------------------------------------------------------
	String NULLRenderSystem::getErrorDescription( long errorNumber ) const
	{
		return StringUtil::BLANK;
	}
}
