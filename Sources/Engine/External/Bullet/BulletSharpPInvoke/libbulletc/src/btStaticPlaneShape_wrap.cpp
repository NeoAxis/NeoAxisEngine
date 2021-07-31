#include <BulletCollision/CollisionShapes/btStaticPlaneShape.h>

#include "conversion.h"
#include "btStaticPlaneShape_wrap.h"

btStaticPlaneShape* btStaticPlaneShape_new(const btVector3* planeNormal, btScalar planeConstant)
{
	BTVECTOR3_IN(planeNormal);
	return new btStaticPlaneShape(BTVECTOR3_USE(planeNormal), planeConstant);
}

btScalar btStaticPlaneShape_getPlaneConstant(btStaticPlaneShape* obj)
{
	return obj->getPlaneConstant();
}

void btStaticPlaneShape_getPlaneNormal(btStaticPlaneShape* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getPlaneNormal());
}
