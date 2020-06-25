// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

#ifdef PLATFORM_WINDOWS
	//!!!!!temp
	#define MINIDUMP
#endif

//#ifdef _WIN32
//#define MINIDUMP
//#endif

#ifdef MINIDUMP

class MiniDump
{
	static BOOL GetImpersonationToken(HANDLE* phToken);
	static BOOL EnablePriv(LPCTSTR pszPriv, HANDLE hToken, TOKEN_PRIVILEGES* ptpOld);
	static BOOL RestorePriv(HANDLE hToken, TOKEN_PRIVILEGES* ptpOld);
	static HMODULE LocalLoadLibrary(HMODULE hModule, LPCTSTR pszModule);

public:
	static BOOL Create(HMODULE hModule, PEXCEPTION_POINTERS pExceptionInfo);
};

extern unsigned long ExceptionHandler(LPEXCEPTION_POINTERS pExceptionPointers);

#endif