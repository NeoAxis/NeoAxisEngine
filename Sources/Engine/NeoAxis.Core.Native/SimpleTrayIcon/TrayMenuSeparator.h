#include "OgreStableHeaders.h"
#ifdef PLATFORM_WINDOWS //#if (defined( __WIN32__ ) || defined( _WIN32 ))

#pragma once

#include "TrayMenuItemBase.h"

class TrayMenuSeparator : public TrayMenuItemBase
{
protected:
	UINT GetFlags() const noexcept;
};


#endif