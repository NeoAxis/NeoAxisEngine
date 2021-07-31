// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once
using namespace Ogre;

EXPORT NameValuePairList* OgreNameValuePairList_New()
{
	return new NameValuePairList();
}

EXPORT void OgreNameValuePairList_Delete( NameValuePairList* _this )
{
	delete _this;
}

EXPORT void OgreNameValuePairList_insert( NameValuePairList* _this, char* key, char* value )
{
	_this->insert( NameValuePairList::value_type( key, value ) );
}
