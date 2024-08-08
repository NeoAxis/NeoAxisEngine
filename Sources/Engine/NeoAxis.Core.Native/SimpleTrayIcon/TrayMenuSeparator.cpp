#include "OgreStableHeaders.h"
#ifdef PLATFORM_WINDOWS //#if (defined( __WIN32__ ) || defined( _WIN32 ))

#include "pch.h"
#include "TrayMenuSeparator.h"

UINT TrayMenuSeparator::GetFlags() const noexcept
{
    return MF_SEPARATOR;
}

#endif