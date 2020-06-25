#include "OgreStableHeaders.h"
#include "BoundsD.h"

namespace Ogre
{
	const BoundsD BoundsD::BOUNDSD_ZERO( 0, 0, 0, 0, 0, 0 );
	const BoundsD BoundsD::BOUNDSD_CLEARED(DBL_MAX, DBL_MAX, DBL_MAX, DBL_MIN, DBL_MIN, DBL_MIN );
}
