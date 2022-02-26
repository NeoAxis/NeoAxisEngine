// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once
using namespace Ogre;

#include "MyOgreVirtualFileSystem.h"
#include "MyOgreSceneManager.h"
#include "MiniDump.h"

EXPORT Root* OgreRoot_New(const wchar16* nativeLibrariesDirectory)
{
	return new Root(TO_WCHAR_T(nativeLibrariesDirectory));
}

EXPORT void OgreRoot_SetGeneralSetting( Root* root, const wchar16* key, const wchar16* value )
{
	root->SetGeneralSetting( TO_WCHAR_T(key), TO_WCHAR_T(value) );
}

EXPORT void OgreRoot_Delete(Root* root)
{
	delete root;
}

EXPORT void OgreRoot_initialise( Root* root )
{
	root->initialise();
}

EXPORT MyOgreSceneManager* OgreRoot_createSceneManager( Root* root, char* typeName )
{
	return new MyOgreSceneManager(typeName);
}

EXPORT void OgreRoot_destroySceneManager(MyOgreSceneManager* sceneManager)
{
	root->sceneManager = NULL;
	delete sceneManager;
}
