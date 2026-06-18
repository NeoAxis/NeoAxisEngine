// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#pragma once
using namespace Native;

#include "MyNativeVirtualFileSystem.h"

EXPORT void OgreArchiveManager_addArchiveFactory( MyOgreVirtualArchiveFactory* factory )
{
	root->mArchiveManager->addArchiveFactory( factory );
}
