#include "OgreStableHeaders.h"
#include "Vector3I.h"
#include "OgreVector3D.h"

namespace Ogre
{
    const Vector3I Vector3I::ZERO( 0, 0, 0 );

    const Vector3I Vector3I::UNIT_X( 1, 0, 0 );
    const Vector3I Vector3I::UNIT_Y( 0, 1, 0 );
    const Vector3I Vector3I::UNIT_Z( 0, 0, 1 );
    const Vector3I Vector3I::NEGATIVE_UNIT_X( -1,  0,  0 );
    const Vector3I Vector3I::NEGATIVE_UNIT_Y(  0, -1,  0 );
    const Vector3I Vector3I::NEGATIVE_UNIT_Z(  0,  0, -1 );
    const Vector3I Vector3I::UNIT_SCALE(1, 1, 1);

	Vector3D Vector3I::toVector3D() const
	{
		return Vector3D(x, y, z);
	}
}
