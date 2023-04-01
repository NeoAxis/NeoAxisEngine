// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "OgreNativeWrapperGeneral.h"
#pragma hdrstop

#include "MiniDump.h"

#ifdef MINIDUMP


#include "MiniDump.h"
#include <windows.h>
#include <dbghelp.h>
#include <tchar.h>

typedef BOOL (WINAPI *MINIDUMPWRITEDUMP)(	HANDLE hProcess,
											DWORD dwPid,
											HANDLE hFile,
											MINIDUMP_TYPE DumpType,
											CONST PMINIDUMP_EXCEPTION_INFORMATION ExceptionParam,
											CONST PMINIDUMP_USER_STREAM_INFORMATION UserStreamParam,
											CONST PMINIDUMP_CALLBACK_INFORMATION CallbackParam);

BOOL MiniDump::Create(HMODULE hModule, PEXCEPTION_POINTERS pExceptionInfo)
{
	BOOL bRet = FALSE;
	DWORD dwLastError = 0;

	HANDLE hImpersonationToken = NULL;
	if(!GetImpersonationToken(&hImpersonationToken))
		return FALSE;

	HMODULE hDbgDll = LocalLoadLibrary(hModule, _T("dbghelp.dll"));
	if(!hDbgDll)
		return FALSE;

	MINIDUMPWRITEDUMP pDumpFunction = (MINIDUMPWRITEDUMP)::GetProcAddress(hDbgDll, "MiniDumpWriteDump");
	if(!pDumpFunction)
	{
		FreeLibrary(hDbgDll);
		return FALSE;
	}

//	::CreateDirectory("Errors",NULL);

	//const char* dumpfilename = "UserSettings//NativeError.dmp";
	const char* dumpfilename = "C:\\_Temp\\NativeError.dmp";
	//const char* dumpfilename = "C://NativeError.dmp";

	HANDLE hDumpFile = ::CreateFile(dumpfilename, 
									GENERIC_READ | GENERIC_WRITE, 
									FILE_SHARE_WRITE | FILE_SHARE_READ, 
									0, CREATE_ALWAYS, 0, 0);
	if(hDumpFile == INVALID_HANDLE_VALUE)
	{
		MessageBox(0, "Cannot create dump file.", "Error", MB_OK);
		return FALSE;
	}

	MINIDUMP_EXCEPTION_INFORMATION stInfo = {0};

	stInfo.ThreadId = GetCurrentThreadId();
	stInfo.ExceptionPointers = pExceptionInfo;
	stInfo.ClientPointers = TRUE;

	// We need the SeDebugPrivilege to be able to run MiniDumpWriteDump
	TOKEN_PRIVILEGES tp;
	BOOL bPrivilegeEnabled = EnablePriv(SE_DEBUG_NAME, hImpersonationToken, &tp);

	// DBGHELP.DLL is not thread safe
	//EnterCriticalSection(pCS);
	bRet = pDumpFunction(GetCurrentProcess(), GetCurrentProcessId(), hDumpFile, 
		MiniDumpWithDataSegs, &stInfo, NULL, NULL);
	//LeaveCriticalSection(pCS);

	if(bPrivilegeEnabled)
		RestorePriv(hImpersonationToken, &tp);

	CloseHandle(hDumpFile);

	FreeLibrary(hDbgDll);

	return bRet;
}

BOOL MiniDump::GetImpersonationToken(HANDLE* phToken)
{
	*phToken = NULL;

	if(!OpenThreadToken(GetCurrentThread(), TOKEN_QUERY | TOKEN_ADJUST_PRIVILEGES, TRUE, phToken))
	{
		if(GetLastError() == ERROR_NO_TOKEN)
		{
			// No impersonation token for the curren thread available - go for the process token
			if(!OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY | TOKEN_ADJUST_PRIVILEGES, phToken))
				return FALSE;
		}
		else
			return FALSE;
	}

	return TRUE;
}

BOOL MiniDump::EnablePriv(LPCTSTR pszPriv, HANDLE hToken, TOKEN_PRIVILEGES* ptpOld)
{
	BOOL bOk = FALSE;

	TOKEN_PRIVILEGES tp;
	tp.PrivilegeCount = 1;
	tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
	bOk = LookupPrivilegeValue( 0, pszPriv, &tp.Privileges[0].Luid);
	if(bOk)
	{
		DWORD cbOld = sizeof(*ptpOld);
		bOk = AdjustTokenPrivileges(hToken, FALSE, &tp, cbOld, ptpOld, &cbOld);
	}

	return (bOk && (ERROR_NOT_ALL_ASSIGNED != GetLastError()));
}

BOOL MiniDump::RestorePriv(HANDLE hToken, TOKEN_PRIVILEGES* ptpOld)
{
	BOOL bOk = AdjustTokenPrivileges(hToken, FALSE, ptpOld, 0, 0, 0);	
	return (bOk && (ERROR_NOT_ALL_ASSIGNED != GetLastError()));
}

HMODULE MiniDump::LocalLoadLibrary(HMODULE hModule, LPCTSTR pszModule)
{
	HMODULE hDll = NULL;

	// Attempt to load local module first
	TCHAR pszModulePath[MAX_PATH];
	if(GetModuleFileName(hModule, pszModulePath, sizeof(pszModulePath) / sizeof(pszModulePath[0])))
	{
		TCHAR* pSlash = _tcsrchr(pszModulePath, _T('\\'));
		if(0 != pSlash)
		{
			_tcscpy(pSlash + 1, pszModule);
			hDll = ::LoadLibrary(pszModulePath);
		}
	}

	if(!hDll)
	{
		// If not found, load any available copy
		hDll = ::LoadLibrary(pszModule);
	}

	return hDll;
}

unsigned long ExceptionHandler(LPEXCEPTION_POINTERS pExceptionPointers)
{
	if(MiniDump::Create(NULL, pExceptionPointers))
	{
	}
	else
	{
	}

	return EXCEPTION_EXECUTE_HANDLER;
}

#endif
