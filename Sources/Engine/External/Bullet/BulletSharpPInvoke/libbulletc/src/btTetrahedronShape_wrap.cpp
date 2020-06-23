#include <BulletCollision/CollisionShapes/btTetrahedronShape.h>

#include "conversion.h"
#include "btTetrahedronShape_wrap.h"

btBU_Simplex1to4* btBU_Simplex1to4_new()
{
	return new btBU_Simplex1to4();
}

btBU_Simplex1to4* btBU_Simplex1to4_new2(const btVector3* pt0)
{
	BTVECTOR3_IN(pt0);
	return new btBU_Simplex1to4(BTVECTOR3_USE(pt0));
}

btBU_Simplex1to4* btBU_Simplex1to4_new3(const btVector3* pt0, const btVector3* pt1)
{
	BTVECTOR3_IN(pt0);
	BTVECTOR3_IN(pt1);
	return new btBU_Simplex1to4(BTVECTOR3_USE(pt0), BTVECTOR3_USE(pt1));
}

btBU_Simplex1to4* btBU_Simplex1to4_new4(const btVector3* pt0, const btVector3* pt1,
	const btVector3* pt2)
{
	BTVECTOR3_IN(pt0);
	BTVECTOR3_IN(pt1);
	BTVECTOR3_IN(pt2);
	return new btBU_Simplex1to4(BTVECTOR3_USE(pt0), BTVECTOR3_USE(pt1), BTVECTOR3_USE(pt2));
}

btBU_Simplex1to4* btBU_Simplex1to4_new5(const btVector3* pt0, const btVector3* pt1,
	const btVector3* pt2, const btVector3* pt3)
{
	BTVECTOR3_IN(pt0);
	BTVECTOR3_IN(pt1);
	BTVECTOR3_IN(pt2);
	BTVECTOR3_IN(pt3);
	return new btBU_Simplex1to4(BTVECTOR3_USE(pt0), BTVECTOR3_USE(pt1), BTVECTOR3_USE(pt2),
		BTVECTOR3_USE(pt3));
}

void btBU_Simplex1to4_addVertex(btBU_Simplex1to4* obj, const btVector3* pt)
{
	BTVECTOR3_IN(pt);
	obj->addVertex(BTVECTOR3_USE(pt));
}

int btBU_Simplex1to4_getIndex(btBU_Simplex1to4* obj, int i)
{
	return obj->getIndex(i);
}

void btBU_Simplex1to4_reset(btBU_Simplex1to4* obj)
{
	obj->reset();
}
