#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btMinkowskiSumShape* btMinkowskiSumShape_new(const btConvexShape* shapeA, const btConvexShape* shapeB);
	EXPORT const btConvexShape* btMinkowskiSumShape_getShapeA(btMinkowskiSumShape* obj);
	EXPORT const btConvexShape* btMinkowskiSumShape_getShapeB(btMinkowskiSumShape* obj);
	EXPORT void btMinkowskiSumShape_getTransformA(btMinkowskiSumShape* obj, btTransform* value);
	EXPORT void btMinkowskiSumShape_GetTransformB(btMinkowskiSumShape* obj, btTransform* value);
	EXPORT void btMinkowskiSumShape_setTransformA(btMinkowskiSumShape* obj, const btTransform* transA);
	EXPORT void btMinkowskiSumShape_setTransformB(btMinkowskiSumShape* obj, const btTransform* transB);
#ifdef __cplusplus
}
#endif
