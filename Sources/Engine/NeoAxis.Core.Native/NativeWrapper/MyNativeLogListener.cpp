// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#include "NativeStableHeaders.h"
#include "NativeNativeWrapperGeneral.h"
#include "MyNativeLogListener.h"

EXPORT MyOgreLogListener* MyOgreLogListener_New(
	MyOgreLogListener_messageLoggedDelegate messageLogged )
{
	MyOgreLogListener* _this = new MyOgreLogListener();
	_this->messageLoggedDelegate = messageLogged;
	return _this;
}

EXPORT void MyOgreLogListener_Delete( MyOgreLogListener* _this )
{
	delete _this;
}
