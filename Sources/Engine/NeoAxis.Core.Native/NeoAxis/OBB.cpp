// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "OgreStableHeaders.h"
#include "OBB.h"

namespace Ogre
{
	const OBB OBB::OBB_ZERO( Vector3::ZERO, Vector3::ZERO, Matrix3::IDENTITY );
	const OBB OBB::OBB_CLEARED( Vector3::ZERO, Vector3( FLT_MIN, FLT_MIN, FLT_MIN ), Matrix3::IDENTITY );
}
