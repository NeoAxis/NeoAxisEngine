// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "BoundsI.h"

namespace Ogre
{
	const BoundsI BoundsI::BOUNDS_ZERO( 0, 0, 0, 0, 0, 0 );
	const BoundsI BoundsI::BOUNDS_CLEARED( INT_MAX, INT_MAX, INT_MAX, INT_MIN, INT_MIN, INT_MIN );
}
