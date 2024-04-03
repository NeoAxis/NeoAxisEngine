#if (defined( __WIN32__ ) || defined( _WIN32 ))

#include "pch.h"
#include "TrayIconManager.h"
#include <memory>

//static std::vector<std::weak_ptr<TrayIconDataWrapper>> items;
//
//std::shared_ptr<TrayIconDataWrapper> TrayIconManager::Create() noexcept
//{
//	auto shared = std::make_shared<TrayIconDataWrapper>();
//	items.push_back(shared);
//	return shared;
//}
//
//void TrayIconManager::Cleanup() noexcept
//{
//	for (auto& item : items) {
//		if (std::shared_ptr<TrayIconDataWrapper> spt = item.lock()) {
//			spt->reset();
//		}
//	}
//}

#endif