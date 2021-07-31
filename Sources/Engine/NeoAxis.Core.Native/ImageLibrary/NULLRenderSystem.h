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
#ifndef __NULLRENDERSYSTEM_H__
#define __NULLRENDERSYSTEM_H__

#include "NULLPrerequisites.h"
#include "OgreString.h"
#include "OgreStringConverter.h"
#include "OgreRenderSystem.h"
#include "OgreRenderSystemCapabilities.h"

namespace Ogre 
{
#define MAX_LIGHTS 8

	/**
	Implementation of NULL as a rendering system.
	*/
	class NULLRenderSystem : public RenderSystem
	{

	public:
		
		// Stored options
		ConfigOptionMap mOptions;
		
	public:

		// constructor
		NULLRenderSystem(Root* root);
		// destructor
		~NULLRenderSystem();

		virtual void initConfigOptions(void);

		// Overridden RenderSystem functions
		ConfigOptionMap& getConfigOptions(void);
		String validateConfigOptions(void);

		String getErrorDescription( long errorNumber ) const;
		const String& getName(void) const;
		// Low-level overridden members
		void setConfigOption( const String &name, const String &value );
		//void reinitialise();
		void shutdown();

};
}
#endif
