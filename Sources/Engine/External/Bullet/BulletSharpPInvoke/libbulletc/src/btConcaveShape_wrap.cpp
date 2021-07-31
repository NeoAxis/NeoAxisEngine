#include <BulletCollision/CollisionShapes/btConcaveShape.h>

#include "conversion.h"
#include "btConcaveShape_wrap.h"

void btConcaveShape_processAllTriangles(btConcaveShape* obj, btTriangleCallback* callback,
	const btVector3* aabbMin, const btVector3* aabbMax)
{
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	obj->processAllTriangles(callback, BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax));
}
