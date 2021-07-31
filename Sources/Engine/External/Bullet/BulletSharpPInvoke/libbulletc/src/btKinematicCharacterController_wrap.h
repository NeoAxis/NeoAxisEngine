#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btKinematicCharacterController* btKinematicCharacterController_new(btPairCachingGhostObject* ghostObject, btConvexShape* convexShape, btScalar stepHeight);
	EXPORT btKinematicCharacterController* btKinematicCharacterController_new2(btPairCachingGhostObject* ghostObject, btConvexShape* convexShape, btScalar stepHeight, const btVector3* up);
	EXPORT void btKinematicCharacterController_applyImpulse(btKinematicCharacterController* obj, const btVector3* v);
	EXPORT btScalar btKinematicCharacterController_getAngularDamping(btKinematicCharacterController* obj);
	EXPORT void btKinematicCharacterController_getAngularVelocity(btKinematicCharacterController* obj, btVector3* value);
	EXPORT btScalar btKinematicCharacterController_getFallSpeed(btKinematicCharacterController* obj);
	EXPORT btPairCachingGhostObject* btKinematicCharacterController_getGhostObject(btKinematicCharacterController* obj);
	EXPORT void btKinematicCharacterController_getGravity(btKinematicCharacterController* obj, btVector3* value);
	EXPORT btScalar btKinematicCharacterController_getJumpSpeed(btKinematicCharacterController* obj);
	EXPORT btScalar btKinematicCharacterController_getLinearDamping(btKinematicCharacterController* obj);
	EXPORT void btKinematicCharacterController_getLinearVelocity(btKinematicCharacterController* obj, btVector3* value);
	EXPORT btScalar btKinematicCharacterController_getMaxPenetrationDepth(btKinematicCharacterController* obj);
	EXPORT btScalar btKinematicCharacterController_getMaxSlope(btKinematicCharacterController* obj);
	EXPORT btScalar btKinematicCharacterController_getStepHeight(btKinematicCharacterController* obj);
	EXPORT void btKinematicCharacterController_getUp(btKinematicCharacterController* obj, btVector3* value);
	EXPORT void btKinematicCharacterController_setAngularDamping(btKinematicCharacterController* obj, btScalar d);
	EXPORT void btKinematicCharacterController_setAngularVelocity(btKinematicCharacterController* obj, const btVector3* velocity);
	EXPORT void btKinematicCharacterController_setFallSpeed(btKinematicCharacterController* obj, btScalar fallSpeed);
	EXPORT void btKinematicCharacterController_setGravity(btKinematicCharacterController* obj, const btVector3* gravity);
	EXPORT void btKinematicCharacterController_setJumpSpeed(btKinematicCharacterController* obj, btScalar jumpSpeed);
	EXPORT void btKinematicCharacterController_setLinearDamping(btKinematicCharacterController* obj, btScalar d);
	EXPORT void btKinematicCharacterController_setLinearVelocity(btKinematicCharacterController* obj, const btVector3* velocity);
	EXPORT void btKinematicCharacterController_setMaxJumpHeight(btKinematicCharacterController* obj, btScalar maxJumpHeight);
	EXPORT void btKinematicCharacterController_setMaxPenetrationDepth(btKinematicCharacterController* obj, btScalar d);
	EXPORT void btKinematicCharacterController_setMaxSlope(btKinematicCharacterController* obj, btScalar slopeRadians);
	EXPORT void btKinematicCharacterController_setStepHeight(btKinematicCharacterController* obj, btScalar h);
	EXPORT void btKinematicCharacterController_setUp(btKinematicCharacterController* obj, const btVector3* up);
	EXPORT void btKinematicCharacterController_setUseGhostSweepTest(btKinematicCharacterController* obj, bool useGhostObjectSweepTest);
#ifdef __cplusplus
}
#endif
