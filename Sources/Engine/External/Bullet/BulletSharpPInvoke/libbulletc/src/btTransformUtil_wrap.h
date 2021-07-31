#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT void btTransformUtil_calculateDiffAxisAngle(const btTransform* transform0, const btTransform* transform1, btVector3* axis, btScalar* angle);
	EXPORT void btTransformUtil_calculateDiffAxisAngleQuaternion(const btQuaternion* orn0, const btQuaternion* orn1a, btVector3* axis, btScalar* angle);
	EXPORT void btTransformUtil_calculateVelocity(const btTransform* transform0, const btTransform* transform1, btScalar timeStep, btVector3* linVel, btVector3* angVel);
	EXPORT void btTransformUtil_calculateVelocityQuaternion(const btVector3* pos0, const btVector3* pos1, const btQuaternion* orn0, const btQuaternion* orn1, btScalar timeStep, btVector3* linVel, btVector3* angVel);
	EXPORT void btTransformUtil_integrateTransform(const btTransform* curTrans, const btVector3* linvel, const btVector3* angvel, btScalar timeStep, btTransform* predictedTransform);

	EXPORT btConvexSeparatingDistanceUtil* btConvexSeparatingDistanceUtil_new(btScalar boundingRadiusA, btScalar boundingRadiusB);
	EXPORT btScalar btConvexSeparatingDistanceUtil_getConservativeSeparatingDistance(btConvexSeparatingDistanceUtil* obj);
	EXPORT void btConvexSeparatingDistanceUtil_initSeparatingDistance(btConvexSeparatingDistanceUtil* obj, const btVector3* separatingVector, btScalar separatingDistance, const btTransform* transA, const btTransform* transB);
	EXPORT void btConvexSeparatingDistanceUtil_updateSeparatingDistance(btConvexSeparatingDistanceUtil* obj, const btTransform* transA, const btTransform* transB);
	EXPORT void btConvexSeparatingDistanceUtil_delete(btConvexSeparatingDistanceUtil* obj);
#ifdef __cplusplus
}
#endif
