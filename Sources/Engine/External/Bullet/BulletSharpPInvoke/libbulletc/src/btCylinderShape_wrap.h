#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btCylinderShape* btCylinderShape_new(const btVector3* halfExtents);
	EXPORT btCylinderShape* btCylinderShape_new2(btScalar halfExtentX, btScalar halfExtentY, btScalar halfExtentZ);
	EXPORT void btCylinderShape_getHalfExtentsWithMargin(btCylinderShape* obj, btVector3* value);
	EXPORT void btCylinderShape_getHalfExtentsWithoutMargin(btCylinderShape* obj, btVector3* value);
	EXPORT btScalar btCylinderShape_getRadius(btCylinderShape* obj);
	EXPORT int btCylinderShape_getUpAxis(btCylinderShape* obj);

	EXPORT btCylinderShapeX* btCylinderShapeX_new(const btVector3* halfExtents);
	EXPORT btCylinderShapeX* btCylinderShapeX_new2(btScalar halfExtentX, btScalar halfExtentY, btScalar halfExtentZ);

	EXPORT btCylinderShapeZ* btCylinderShapeZ_new(const btVector3* halfExtents);
	EXPORT btCylinderShapeZ* btCylinderShapeZ_new2(btScalar halfExtentX, btScalar halfExtentY, btScalar halfExtentZ);
#ifdef __cplusplus
}
#endif
