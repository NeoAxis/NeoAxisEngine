// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once
using namespace Ogre;

///////////////////////////////////////////////////////////////////////////////////////////////////

EXPORT int OgreStringInterface_getParameterCount( StringInterface* _this )
{
	return _this->getParameters().size();
}

EXPORT wchar16* OgreStringInterface_getParameterName( StringInterface* _this, int index )
{
	return CreateOutString(_this->getParameters()[index].name);
}

EXPORT wchar16* OgreStringInterface_getParameter( StringInterface* _this, char* name )
{
	return CreateOutString(_this->getParameter(name));
}

EXPORT bool OgreStringInterface_setParameter( StringInterface* _this, char* name, wchar16* value )
{
	return _this->setParameter(name, TO_WCHAR_T(value));
}

///////////////////////////////////////////////////////////////////////////////////////////////////
