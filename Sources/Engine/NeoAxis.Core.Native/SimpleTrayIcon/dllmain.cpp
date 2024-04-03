#if (defined( __WIN32__ ) || defined( _WIN32 ))

// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include "TrayIconManager.h"

extern void OnProcessDetach();

BOOL APIENTRY DllMain([[maybe_unused]] HMODULE hModule,
	[[maybe_unused]] DWORD  ul_reason_for_call,
	[[maybe_unused]] LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	//case DLL_PROCESS_ATTACH:
	//	DisableThreadLibraryCalls(hModule);
	//	break;

	case DLL_PROCESS_DETACH:
		// Finalizers are flaky; programmers are lazy. For redundancy, we ensure that icons are deleted when unloading.
		// https://github.com/dotnet/docs/issues/17463

		OnProcessDetach();
		//!!!!
		//TrayIconManager::Cleanup();

		break;
	}
	return TRUE;
}

#endif