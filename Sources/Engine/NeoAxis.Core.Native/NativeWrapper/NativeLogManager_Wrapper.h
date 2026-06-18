// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#pragma once
using namespace Native;

#include "MyNativeLogListener.h"

EXPORT void OgreLogManager_getDefaultLog_addListener( Root* root, MyOgreLogListener* listener )
{
	root->mLogManager->getDefaultLog()->addListener( listener );
}

EXPORT void OgreLogManager_getDefaultLog_removeListener( Root* root, MyOgreLogListener* listener )
{
	root->mLogManager->getDefaultLog()->removeListener( listener );
}
