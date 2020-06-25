// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "OBBD.h"

namespace Ogre
{
	const OBBD OBBD::OBBD_ZERO( Vector3D::ZERO, Vector3D::ZERO, Matrix3D::IDENTITY );
	const OBBD OBBD::OBBD_CLEARED( Vector3D::ZERO, Vector3D( DBL_MIN, DBL_MIN, DBL_MIN), Matrix3D::IDENTITY );
}
