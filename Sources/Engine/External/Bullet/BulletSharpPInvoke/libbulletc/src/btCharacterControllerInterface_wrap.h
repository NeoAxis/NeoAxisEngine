#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT bool btCharacterControllerInterface_canJump(btCharacterControllerInterface* obj);
	EXPORT void btCharacterControllerInterface_jump(btCharacterControllerInterface* obj);
	EXPORT void btCharacterControllerInterface_jump2(btCharacterControllerInterface* obj, const btVector3* dir);
	EXPORT bool btCharacterControllerInterface_onGround(btCharacterControllerInterface* obj);
	EXPORT void btCharacterControllerInterface_playerStep(btCharacterControllerInterface* obj, btCollisionWorld* collisionWorld, btScalar dt);
	EXPORT void btCharacterControllerInterface_preStep(btCharacterControllerInterface* obj, btCollisionWorld* collisionWorld);
	EXPORT void btCharacterControllerInterface_reset(btCharacterControllerInterface* obj, btCollisionWorld* collisionWorld);
	EXPORT void btCharacterControllerInterface_setUpInterpolate(btCharacterControllerInterface* obj, bool value);
	EXPORT void btCharacterControllerInterface_setWalkDirection(btCharacterControllerInterface* obj, const btVector3* walkDirection);
	EXPORT void btCharacterControllerInterface_setVelocityForTimeInterval(btCharacterControllerInterface* obj, const btVector3* velocity, btScalar timeInterval);
	EXPORT void btCharacterControllerInterface_warp(btCharacterControllerInterface* obj, const btVector3* origin);
#ifdef __cplusplus
}
#endif
