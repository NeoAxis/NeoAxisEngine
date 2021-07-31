#include <BulletCollision/CollisionShapes/btConvexInternalShape.h>

#include "conversion.h"
#include "btConvexInternalShape_wrap.h"

void btConvexInternalShape_getImplicitShapeDimensions(btConvexInternalShape* obj,
	btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getImplicitShapeDimensions());
}

void btConvexInternalShape_getLocalScalingNV(btConvexInternalShape* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getLocalScalingNV());
}

btScalar btConvexInternalShape_getMarginNV(btConvexInternalShape* obj)
{
	return obj->getMarginNV();
}

void btConvexInternalShape_setImplicitShapeDimensions(btConvexInternalShape* obj,
	const btVector3* dimensions)
{
	BTVECTOR3_IN(dimensions);
	obj->setImplicitShapeDimensions(BTVECTOR3_USE(dimensions));
}

void btConvexInternalShape_setSafeMargin(btConvexInternalShape* obj, btScalar minDimension,
	btScalar defaultMarginMultiplier)
{
	obj->setSafeMargin(minDimension, defaultMarginMultiplier);
}

void btConvexInternalShape_setSafeMargin2(btConvexInternalShape* obj, const btVector3* halfExtents,
	btScalar defaultMarginMultiplier)
{
	BTVECTOR3_IN(halfExtents);
	obj->setSafeMargin(BTVECTOR3_USE(halfExtents), defaultMarginMultiplier);
}


void btConvexInternalAabbCachingShape_recalcLocalAabb(btConvexInternalAabbCachingShape* obj)
{
	obj->recalcLocalAabb();
}
