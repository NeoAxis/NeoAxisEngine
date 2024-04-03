#if (defined( __WIN32__ ) || defined( _WIN32 ))

#include "pch.h"
#include "dllapi.h"

#define GUARD_NOT_NULL(x) if (!x) return E_POINTER

#define API_TRY try {
#define API_CATCH(x) } \
	catch (const Win32Exception& e) { \
			x \
			return e.GetHR(); \
	} \
	return S_OK;



TRAYAPI TrayMenuCreate(const HICON hIcon, const _In_ WCHAR tip[128], const _In_ TrayMenuClickHandler onClick, const _In_ TrayMenuClickHandler onDoubleClick, _Outptr_result_nullonfailure_ MyTrayMenu** pInstance) noexcept
{
	*pInstance = NULL;
	GUARD_NOT_NULL(hIcon);
	//GUARD_NOT_NULL(onDoubleClick);

	API_TRY
		* pInstance = new MyTrayMenu(hIcon, tip, onClick, onDoubleClick);
	API_CATCH(
		if (pInstance) {
			if (*pInstance) {
				delete* pInstance;
			}

			*pInstance = NULL;
		}
	)
}

TRAYAPI TrayMenuRelease(_In_ MyTrayMenu* pInstance) noexcept
{
	delete pInstance;
	return S_OK;
}

//TRAYAPI TrayMenuRelease(const _Inout_ MyTrayMenu** pInstance) noexcept
//{
//	if (pInstance) {
//		if (*pInstance) {
//			delete* pInstance;
//		}
//
//		*pInstance = NULL;
//	}
//
//	return S_OK;
//}

TRAYAPI TrayMenuShow(_In_ MyTrayMenu* pInstance) noexcept
{
	GUARD_NOT_NULL(pInstance);
	pInstance->Show();
	return S_OK;
}

TRAYAPI TrayMenuClose(_In_ MyTrayMenu* pInstance) noexcept
{
	GUARD_NOT_NULL(pInstance);
	pInstance->Close();
	return S_OK;
}

TRAYAPI TrayMenuAdd(_In_ MyTrayMenu* pInstance, _In_ TrayMenuItemBase* pTrayMenuItem) noexcept
{
	GUARD_NOT_NULL(pInstance);
	GUARD_NOT_NULL(pTrayMenuItem);
	pInstance->AddItem(*pTrayMenuItem);
	return S_OK;
}

TRAYAPI TrayMenuRemove(_In_ MyTrayMenu* pInstance, _In_ TrayMenuItemBase* pTrayMenuItem) noexcept
{
	GUARD_NOT_NULL(pInstance);
	GUARD_NOT_NULL(pTrayMenuItem);
	pInstance->RemoveItem(*pTrayMenuItem);
	return S_OK;
}

TRAYAPI TrayMenuSetIcon(_In_ MyTrayMenu* pInstance, const HICON hIcon) noexcept
{
	GUARD_NOT_NULL(pInstance);
	GUARD_NOT_NULL(hIcon);
	pInstance->SetIcon(hIcon);
	return S_OK;
}

TRAYAPI TrayMenuItemCreate(const _In_ TrayMenuItemClickHandler onClicked, _Outptr_result_nullonfailure_ TrayMenuItem** pInstance) noexcept
{
	GUARD_NOT_NULL(onClicked);
	GUARD_NOT_NULL(pInstance);
	*pInstance = NULL;

	API_TRY
		* pInstance = new TrayMenuItem(onClicked);
	API_CATCH(
		if (pInstance) {
			if (*pInstance) {
				delete* pInstance;
			}

			*pInstance = NULL;
		}
	)
}

TRAYAPI TrayMenuItemRelease(const _Inout_ TrayMenuItem** pInstance) noexcept
{
	if (pInstance) {
		if (*pInstance) {
			delete* pInstance;
		}

		*pInstance = NULL;
	}

	return S_OK;
}

TRAYAPI TrayMenuItemContent(_In_ TrayMenuItem* pInstance, _In_ LPCWSTR value) noexcept
{
	GUARD_NOT_NULL(pInstance);
	pInstance->Content(value);
	return S_OK;
}

TRAYAPI TrayMenuItemIsChecked(_In_ TrayMenuItem* pInstance, const BOOL value) noexcept
{
	GUARD_NOT_NULL(pInstance);
	pInstance->IsChecked(value);
	return S_OK;
}

TRAYAPI TrayMenuSeparatorCreate(_Outptr_result_nullonfailure_ TrayMenuSeparator** pInstance) noexcept
{
	GUARD_NOT_NULL(pInstance);
	*pInstance = NULL;

	API_TRY
		* pInstance = new TrayMenuSeparator();
	API_CATCH(
		if (pInstance) {
			if (*pInstance) {
				delete* pInstance;
			}

			*pInstance = NULL;
		}
	)
}

TRAYAPI TrayMenuSeparatorRelease(const _Inout_ TrayMenuSeparator** pInstance) noexcept
{
	if (pInstance) {
		if (*pInstance) {
			delete* pInstance;
		}

		*pInstance = NULL;
	}

	return S_OK;
}

#endif