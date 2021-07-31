#include <BulletCollision/CollisionShapes/btMinkowskiSumShape.h>

#include "conversion.h"
#include "btMinkowskiSumShape_wrap.h"

btMinkowskiSumShape* btMinkowskiSumShape_new(const btConvexShape* shapeA, const btConvexShape* shapeB)
{
	return new btMinkowskiSumShape(shapeA, shapeB);
}

const btConvexShape* btMinkowskiSumShape_getShapeA(btMinkowskiSumShape* obj)
{
	return obj->getShapeA();
}

const btConvexShape* btMinkowskiSumShape_getShapeB(btMinkowskiSumShape* obj)
{
	return obj->getShapeB();
}

void btMinkowskiSumShape_getTransformA(btMinkowskiSumShape* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->getTransformA());
}

void btMinkowskiSumShape_GetTransformB(btMinkowskiSumShape* obj, btTransform* value)
{
	BTTRANSFORM_COPY(value, &obj->GetTransformB());
}

void btMinkowskiSumShape_setTransformA(btMinkowskiSumShape* obj, const btTransform* transA)
{
	BTTRANSFORM_IN(transA);
	obj->setTransformA(BTTRANSFORM_USE(transA));
}

void btMinkowskiSumShape_setTransformB(btMinkowskiSumShape* obj, const btTransform* transB)
{
	BTTRANSFORM_IN(transB);
	obj->setTransformB(BTTRANSFORM_USE(transB));
}
