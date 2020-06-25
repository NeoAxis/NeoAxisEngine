// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once
using namespace Ogre;

typedef void MyOgreLogListener_messageLoggedDelegate( const wchar16* message, LogMessageLevel lml, 
	bool maskDebug);

///////////////////////////////////////////////////////////////////////////////////////////////////

class MyOgreLogListener : public LogListener
{
public:
	MyOgreLogListener_messageLoggedDelegate* messageLoggedDelegate;

	virtual void messageLogged( const String& message, LogMessageLevel lml, bool maskDebug, 
		const String &logName )
	{
		messageLoggedDelegate(TO_WCHAR16(StringUtil::toUTFWide(message).c_str()), lml, maskDebug);
	}
};
