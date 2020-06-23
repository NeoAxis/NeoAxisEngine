#include <BulletCollision/CollisionDispatch/btCollisionWorld.h>
#include <BulletDynamics/Character/btCharacterControllerInterface.h>

#include "btCharacterControllerInterface_wrap.h"

#ifndef BULLETC_DISABLE_IACTION_CLASSES

#include "conversion.h"

bool btCharacterControllerInterface_canJump(btCharacterControllerInterface* obj)
{
	return obj->canJump();
}

void btCharacterControllerInterface_jump(btCharacterControllerInterface* obj)
{
	obj->jump();
}

void btCharacterControllerInterface_jump2(btCharacterControllerInterface* obj, const btVector3* dir)
{
	BTVECTOR3_IN(dir);
	obj->jump(BTVECTOR3_USE(dir));
}

bool btCharacterControllerInterface_onGround(btCharacterControllerInterface* obj)
{
	return obj->onGround();
}

void btCharacterControllerInterface_playerStep(btCharacterControllerInterface* obj,
	btCollisionWorld* collisionWorld, btScalar dt)
{
	obj->playerStep(collisionWorld, dt);
}

void btCharacterControllerInterface_preStep(btCharacterControllerInterface* obj,
	btCollisionWorld* collisionWorld)
{
	obj->preStep(collisionWorld);
}

void btCharacterControllerInterface_reset(btCharacterControllerInterface* obj, btCollisionWorld* collisionWorld)
{
	obj->reset(collisionWorld);
}

void btCharacterControllerInterface_setUpInterpolate(btCharacterControllerInterface* obj,
	bool value)
{
	obj->setUpInterpolate(value);
}

void btCharacterControllerInterface_setWalkDirection(btCharacterControllerInterface* obj,
	const btVector3* walkDirection)
{
	BTVECTOR3_IN(walkDirection);
	obj->setWalkDirection(BTVECTOR3_USE(walkDirection));
}

void btCharacterControllerInterface_setVelocityForTimeInterval(btCharacterControllerInterface* obj,
	const btVector3* velocity, btScalar timeInterval)
{
	BTVECTOR3_IN(velocity);
	obj->setVelocityForTimeInterval(BTVECTOR3_USE(velocity), timeInterval);
}

void btCharacterControllerInterface_warp(btCharacterControllerInterface* obj, const btVector3* origin)
{
	BTVECTOR3_IN(origin);
	obj->warp(BTVECTOR3_USE(origin));
}

#endif
