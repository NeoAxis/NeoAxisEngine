#include "OgreStableHeaders.h"
#include "Bounds.h"

namespace Ogre
{
	const Bounds Bounds::BOUNDS_ZERO( 0, 0, 0, 0, 0, 0 );
	const Bounds Bounds::BOUNDS_CLEARED( FLT_MAX, FLT_MAX, FLT_MAX, FLT_MIN, FLT_MIN, FLT_MIN );
}
