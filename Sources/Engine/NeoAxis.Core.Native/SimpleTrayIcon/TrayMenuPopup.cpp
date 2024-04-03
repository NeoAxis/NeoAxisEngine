#if (defined( __WIN32__ ) || defined( _WIN32 ))

#include "pch.h"
#include "TrayMenuPopup.h"



TrayMenuPopup::TrayMenuPopup(const HWND hWnd, const std::vector<std::reference_wrapper<TrayMenuItemBase>>& items)
{
	m_hWnd = hWnd;
	auto menu = m_hMenu = CreateMenu();
	auto subMenu = m_hSubMenu = CreatePopupMenu();

	if (!AppendMenu(menu, MF_POPUP, reinterpret_cast<UINT_PTR>(subMenu), NULL)) {
		throw Win32Exception("Failed to append menu.");
	}

	for (size_t i = 0; i < items.size(); i++)
	{
		auto& item = items.at(i);
		item.get().Attach(hWnd, m_hSubMenu);
	}

	m_items = items;
	DrawMenuBar(hWnd);
}

TrayMenuPopup::~TrayMenuPopup()
{
	for (const auto x : m_items) {
		x.get().Detach();
	}

	if (m_hSubMenu) {
		DestroyMenu(m_hSubMenu);
	}

	if (m_hMenu) {
		DestroyMenu(m_hMenu);
	}
}

void TrayMenuPopup::Attach(TrayMenuItemBase& item) noexcept
{
	item.Attach(m_hWnd, m_hSubMenu);
	m_items.push_back(item);
}

void TrayMenuPopup::Track() const noexcept
{
    POINT mouse_pointer;
    GetCursorPos(&mouse_pointer);
    SetForegroundWindow(m_hWnd); // Needed for the context menu to disappear.
    TrackPopupMenu(m_hSubMenu, TPM_CENTERALIGN | TPM_BOTTOMALIGN, mouse_pointer.x, mouse_pointer.y, 0, m_hWnd, nullptr);
}

UINT TrayMenuPopup::GetFlags() const noexcept
{
	return MF_SEPARATOR;
}

#endif