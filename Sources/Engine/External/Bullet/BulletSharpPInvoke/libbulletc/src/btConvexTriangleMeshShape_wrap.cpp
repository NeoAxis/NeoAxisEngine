#include <BulletCollision/CollisionShapes/btConvexTriangleMeshShape.h>
#include <BulletCollision/CollisionShapes/btStridingMeshInterface.h>

#include "conversion.h"
#include "btConvexTriangleMeshShape_wrap.h"

btConvexTriangleMeshShape* btConvexTriangleMeshShape_new(btStridingMeshInterface* meshInterface,
	bool calcAabb)
{
	return new btConvexTriangleMeshShape(meshInterface, calcAabb);
}

void btConvexTriangleMeshShape_calculatePrincipalAxisTransform(btConvexTriangleMeshShape* obj,
	btTransform* principal, btVector3* inertia, btScalar* volume)
{
	BTTRANSFORM_IN(principal);
	BTVECTOR3_DEF(inertia);
	obj->calculatePrincipalAxisTransform(BTTRANSFORM_USE(principal), BTVECTOR3_USE(inertia),
		*volume);
	BTTRANSFORM_DEF_OUT(principal);
	BTVECTOR3_DEF_OUT(inertia);
}

btStridingMeshInterface* btConvexTriangleMeshShape_getMeshInterface(btConvexTriangleMeshShape* obj)
{
	return obj->getMeshInterface();
}
