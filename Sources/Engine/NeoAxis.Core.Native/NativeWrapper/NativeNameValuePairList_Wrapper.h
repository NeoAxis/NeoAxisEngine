// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#pragma once
using namespace Native;

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
