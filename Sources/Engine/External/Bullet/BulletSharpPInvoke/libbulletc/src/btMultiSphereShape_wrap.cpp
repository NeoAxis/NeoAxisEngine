#include <BulletCollision/CollisionShapes/btMultiSphereShape.h>

#include "conversion.h"
#include "btMultiSphereShape_wrap.h"

btMultiSphereShape* btMultiSphereShape_new(const btScalar* positions, const btScalar* radi,
	int numSpheres)
{
	btVector3* positionsTemp = new btVector3[numSpheres];
	for (int i = 0; i < numSpheres; i++)
	{
		Vector3TobtVector3(&positions[i*3], &positionsTemp[i]);
	}
	btMultiSphereShape* shape = new btMultiSphereShape(positionsTemp, radi, numSpheres);
	delete[] positionsTemp;
	return shape;
}

btMultiSphereShape* btMultiSphereShape_new2(const btVector3* positions, const btScalar* radi, int numSpheres)
{
	return new btMultiSphereShape(positions, radi, numSpheres);
}

int btMultiSphereShape_getSphereCount(btMultiSphereShape* obj)
{
	return obj->getSphereCount();
}

void btMultiSphereShape_getSpherePosition(btMultiSphereShape* obj, int index, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getSpherePosition(index));
}

btScalar btMultiSphereShape_getSphereRadius(btMultiSphereShape* obj, int index)
{
	return obj->getSphereRadius(index);
}
