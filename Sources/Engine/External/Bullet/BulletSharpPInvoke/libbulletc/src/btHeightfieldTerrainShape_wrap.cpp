#include <BulletCollision/CollisionShapes/btHeightfieldTerrainShape.h>

#include "btHeightfieldTerrainShape_wrap.h"

btHeightfieldTerrainShape* btHeightfieldTerrainShape_new(int heightStickWidth, int heightStickLength,
	const void* heightfieldData, btScalar heightScale, btScalar minHeight, btScalar maxHeight,
	int upAxis, PHY_ScalarType heightDataType, bool flipQuadEdges)
{
	return new btHeightfieldTerrainShape(heightStickWidth, heightStickLength, heightfieldData,
		heightScale, minHeight, maxHeight, upAxis, heightDataType, flipQuadEdges);
}

btHeightfieldTerrainShape* btHeightfieldTerrainShape_new2(int heightStickWidth, int heightStickLength,
	const void* heightfieldData, btScalar maxHeight, int upAxis, bool useFloatData,
	bool flipQuadEdges)
{
	return new btHeightfieldTerrainShape(heightStickWidth, heightStickLength, heightfieldData,
		maxHeight, upAxis, useFloatData, flipQuadEdges);
}

void btHeightfieldTerrainShape_setUseDiamondSubdivision(btHeightfieldTerrainShape* obj)
{
	obj->setUseDiamondSubdivision();
}

void btHeightfieldTerrainShape_setUseDiamondSubdivision2(btHeightfieldTerrainShape* obj,
	bool useDiamondSubdivision)
{
	obj->setUseDiamondSubdivision(useDiamondSubdivision);
}

void btHeightfieldTerrainShape_setUseZigzagSubdivision(btHeightfieldTerrainShape* obj)
{
	obj->setUseZigzagSubdivision();
}

void btHeightfieldTerrainShape_setUseZigzagSubdivision2(btHeightfieldTerrainShape* obj,
	bool useZigzagSubdivision)
{
	obj->setUseZigzagSubdivision(useZigzagSubdivision);
}
