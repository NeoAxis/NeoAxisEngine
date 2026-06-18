// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#pragma once
using namespace Native;

typedef void MyOgreLogListener_messageLoggedDelegate(const wchar16* message, LogMessageLevel lml);
//typedef void MyOgreLogListener_messageLoggedDelegate( const wchar16* message, LogMessageLevel lml, bool maskDebug);

///////////////////////////////////////////////////////////////////////////////////////////////////

class MyOgreLogListener : public LogListener
{
public:
	MyOgreLogListener_messageLoggedDelegate* messageLoggedDelegate;

	virtual void messageLogged( const String& message, LogMessageLevel lml, bool maskDebug, const String &logName )
	{
		messageLoggedDelegate(TO_WCHAR16(StringUtil::toUTFWide(message).c_str()), lml);
		//messageLoggedDelegate(TO_WCHAR16(StringUtil::toUTFWide(message).c_str()), lml, maskDebug);
	}
};
