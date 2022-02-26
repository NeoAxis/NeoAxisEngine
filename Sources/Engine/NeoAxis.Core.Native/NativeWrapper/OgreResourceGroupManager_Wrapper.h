// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once
using namespace Ogre;

#include "MiniDump.h"

EXPORT void OgreResourceGroupManager_initialiseAllResourceGroups( Root* root, wchar16** error )
{
	//*error = NULL;
	//__try
	//{
	//	root->mResourceGroupManager->initialiseAllResourceGroups();
	//}
	//__except(ExceptionHandler(GetExceptionInformation()))
	//{
	//	Fatal("OgreResourceGroupManager_initialiseAllResourceGroups: fatal");
	//}

	*error = NULL;
	try
	{
		root->mResourceGroupManager->initialiseAllResourceGroups();
	}
	catch(Exception& ex)
	{
		*error = CreateOutString(ex.getDescription());
		return;
	}
}

EXPORT void OgreResourceGroupManager_addResourceLocation( Root* root, char* name, char* locType, bool recursive )
{
	root->mResourceGroupManager->addResourceLocation( name, locType, 
		ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME, recursive );
}
