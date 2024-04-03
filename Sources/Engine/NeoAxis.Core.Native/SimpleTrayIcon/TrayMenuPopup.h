#if (defined( __WIN32__ ) || defined( _WIN32 ))

#pragma once
#include "TrayMenuItem.h"

class TrayMenuPopup
{
private:
	HWND m_hWnd;
	std::vector<std::reference_wrapper<TrayMenuItemBase>> m_items;
	HMENU m_hMenu;
	HMENU m_hSubMenu;

public:
	TrayMenuPopup(const HWND hWnd, const std::vector<std::reference_wrapper<TrayMenuItemBase>>& items);
	~TrayMenuPopup();
	void Attach(TrayMenuItemBase& item) noexcept;
	void Track() const noexcept;

protected:
	UINT GetFlags() const noexcept;
};

#endif