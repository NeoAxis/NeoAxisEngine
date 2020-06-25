/*
-----------------------------------------------------------------------------
This source file is part of OGRE
    (Object-oriented Graphics Rendering Engine)
For the latest info, see http://www.ogre3d.org

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
#ifndef __RenderSystem_H_
#define __RenderSystem_H_

// Precompiler options
#include "OgrePrerequisites.h"

#include "OgreString.h"

//#include "OgreTextureUnitState.h"
#include "OgreCommon.h"

#include "OgreRenderSystemCapabilities.h"
//#include "OgreFrameListener.h"
#include "OgreConfigOptionMap.h"
#include "OgrePlane.h"
#include "OgreIteratorWrappers.h"
#include "OgreBlendMode.h"

namespace Ogre
{
	/** \addtogroup Core
	*  @{
	*/
	/** \addtogroup RenderSystem
	*  @{
	*/

	class _OgreExport RenderSystem : public RenderSysAlloc
    {
    public:
        /** Default Constructor.
        */
        RenderSystem();

        /** Destructor.
        */
        virtual ~RenderSystem();

        /** Returns the name of the rendering system.
        */
        virtual const String& getName(void) const = 0;

        /** Returns the details of this API's configuration options
            @remarks
                Each render system must be able to inform the world
                of what options must/can be specified for it's
                operation.
            @par
                These are passed as strings for portability, but
                grouped into a structure (_ConfigOption) which includes
                both options and current value.
            @par
                Note that the settings returned from this call are
                affected by the options that have been set so far,
                since some options are interdependent.
            @par
                This routine is called automatically by the default
                configuration dialogue produced by Root::showConfigDialog
                or may be used by the caller for custom settings dialogs
            @returns
                A 'map' of options, i.e. a list of options which is also
                indexed by option name.
         */
        virtual ConfigOptionMap& getConfigOptions(void) = 0;

        /** Start up the renderer using the settings selected (Or the defaults if none have been selected).
            @remarks
                Called by Root::setRenderSystem. Shouldn't really be called
                directly, although  this can be done if the app wants to.
            @param
                autoCreateWindow If true, creates a render window
                automatically, based on settings chosen so far. This saves
		an extra call to _createRenderWindow
                for the main render window.
            @par
                If an application has more specific window requirements,
                however (e.g. a level design app), it should specify false
                for this parameter and do it manually.
            @returns
                A pointer to the automatically created window, if requested, otherwise null.
        */
		//virtual RenderWindow* _initialise(bool autoCreateWindow, const String& windowTitle = "OGRE Render Window");

		//betauser
		///** Force the render system to use the special capabilities. Can only be called
		//*    before the render system has been fully initializer (before createWindow is called) 
		//*	@param
		//*		 capabilities has to be a subset of the real capabilities and the caller is 
		//*		 responsible for deallocating capabilities.
		//*/
		//virtual void useCustomRenderSystemCapabilities(RenderSystemCapabilities* capabilities);

        ///** Restart the renderer (normally following a change in settings).
        //*/
        //virtual void reinitialise(void) = 0;

        /** Shutdown the renderer and cleanup resources.
        */
        virtual void shutdown(void);

        virtual String getErrorDescription(long errorNumber) const = 0;

		/** Returns the driver version.
		*/
		virtual const DriverVersion& getDriverVersion(void) const { return mDriverVersion; }

    protected:
		 //betauser
		 public:

		DriverVersion mDriverVersion;
    };
	/** @} */
	/** @} */
}

#endif
