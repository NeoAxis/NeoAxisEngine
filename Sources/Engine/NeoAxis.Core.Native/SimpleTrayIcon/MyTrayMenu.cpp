#if (defined( __WIN32__ ) || defined( _WIN32 ))

#include "pch.h"
#include "TrayMenuItem.h"
#include "TrayIcon.h"
#include "MyTrayMenu.h"
#include <functional>

using namespace std;

MyTrayMenu::MyTrayMenu(const HICON hIcon, const _In_ WCHAR tip[128], const TrayMenuClickHandler onClick, const TrayMenuClickHandler onDoubleClick) noexcept
{
	m_hIcon = hIcon;
	wcscpy_s(m_tip, tip);
	m_onClick = onClick;
	m_onDoubleClick = onDoubleClick;
}

void MyTrayMenu::Show() noexcept
{
	m_trayIcon.emplace(std::make_unique<TrayIcon>(m_hIcon, m_tip, std::bind(m_onClick, this), std::bind(m_onDoubleClick, this)));

	while (!m_items.empty())
	{
		const auto& item = m_items.front();
		m_items.pop();
		AddItem(item);
	}
}

void MyTrayMenu::AddItem(TrayMenuItemBase& pTrayMenuItem) noexcept
{
	if (m_trayIcon)
	{
		m_trayIcon->get()->AddItem(pTrayMenuItem);
		m_addedItems.push(pTrayMenuItem);
	}
	else
	{
		m_items.push(pTrayMenuItem);
	}
}

void MyTrayMenu::RemoveItem(TrayMenuItemBase& pTrayMenuItem) noexcept
{
	if (m_trayIcon)
	{
		m_trayIcon->get()->RemoveItem(pTrayMenuItem);
		RemoveItem(m_addedItems, pTrayMenuItem);
	}
	else
	{
		RemoveItem(m_items, pTrayMenuItem);
	}
}

void MyTrayMenu::Close()
{
	m_trayIcon.reset();
}

void MyTrayMenu::SetIcon(const HICON hIcon) noexcept
{
	m_hIcon = hIcon;
	if (m_trayIcon)
	{
		m_trayIcon->get()->SetIcon(hIcon);
	}
}

BOOL MyTrayMenu::RemoveItem(std::queue<std::reference_wrapper<TrayMenuItemBase>>& pQueue, const TrayMenuItemBase& pItem)
{
	const auto max = pQueue.size();
	int i = 0;
	while (!pQueue.empty())
	{
		const auto& current = pQueue.front();
		if (&current.get() == &pItem)
		{
			pQueue.pop();
			break;
		}

		pQueue.pop();
		pQueue.push(current);

		if (i == max)
		{
			return FALSE;
		}
	}

	return TRUE;
}

#endif