#include <BulletCollision/CollisionShapes/btScaledBvhTriangleMeshShape.h>

#include "conversion.h"
#include "btScaledBvhTriangleMeshShape_wrap.h"

btScaledBvhTriangleMeshShape* btScaledBvhTriangleMeshShape_new(btBvhTriangleMeshShape* childShape,
	const btVector3* localScaling)
{
	BTVECTOR3_IN(localScaling);
	return new btScaledBvhTriangleMeshShape(childShape, BTVECTOR3_USE(localScaling));
}

btBvhTriangleMeshShape* btScaledBvhTriangleMeshShape_getChildShape(btScaledBvhTriangleMeshShape* obj)
{
	return obj->getChildShape();
}
