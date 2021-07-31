// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once
using namespace Ogre;

#include "MyOgreVirtualFileSystem.h"

EXPORT void OgreArchiveManager_addArchiveFactory( MyOgreVirtualArchiveFactory* factory )
{
	root->mArchiveManager->addArchiveFactory( factory );
}
