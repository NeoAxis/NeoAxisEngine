// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once
using namespace Ogre;

#include "MyOgreLogListener.h"

EXPORT void OgreLogManager_getDefaultLog_addListener( Root* root, MyOgreLogListener* listener )
{
	root->mLogManager->getDefaultLog()->addListener( listener );
}

EXPORT void OgreLogManager_getDefaultLog_removeListener( Root* root, MyOgreLogListener* listener )
{
	root->mLogManager->getDefaultLog()->removeListener( listener );
}
