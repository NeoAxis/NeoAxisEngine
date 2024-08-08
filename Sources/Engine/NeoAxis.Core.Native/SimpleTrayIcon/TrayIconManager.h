#include "OgreStableHeaders.h"
#ifdef PLATFORM_WINDOWS //#if (defined( __WIN32__ ) || defined( _WIN32 ))

#pragma once
#include <memory>
#include <optional>

//class TrayIconDataWrapper
//{
//private:
//	std::optional<NOTIFYICONDATAW> m_item;
//public:
//	TrayIconDataWrapper() = default;
//
//	~TrayIconDataWrapper()
//	{
//		reset();
//	}
//
//	/*constexpr*/ NOTIFYICONDATAW& emplace(NOTIFYICONDATAW& value)
//	{
//		return m_item.emplace(value);
//	}
//
//	/*constexpr*/ void reset()
//	{
//		if (m_item) 
//		{
//			try
//			{
//				Shell_NotifyIconW(NIM_DELETE, &m_item.value());
//				m_item.reset();
//			}
//			catch (const std::bad_optional_access&)
//			{
//				// Not thread safe, but don't propagate error if we fail.
//			}
//		}
//	}
//
//	constexpr bool has_value()
//	{
//		return m_item.has_value();
//	}
//
//	_NODISCARD constexpr NOTIFYICONDATAW& value() noexcept
//	{
//		return m_item.value();
//	}
//};


//class TrayIconManager
//{
//public:
//	// Returns a new empty struct.
//	static std::shared_ptr<TrayIconDataWrapper> Create() noexcept;
//	// Notifies the shell that we're finished; prevents lingering icons.
//	static void Cleanup() noexcept;
//};

#endif