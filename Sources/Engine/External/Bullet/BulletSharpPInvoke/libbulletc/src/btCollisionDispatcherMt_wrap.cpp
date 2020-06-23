#include <BulletCollision/CollisionDispatch/btCollisionConfiguration.h>
#include <BulletCollision/CollisionDispatch/btCollisionDispatcherMt.h>

#include "btCollisionDispatcherMt_wrap.h"

btCollisionDispatcher* btCollisionDispatcherMt_new(btCollisionConfiguration* collisionConfiguration, int grainSize)
{
	return new btCollisionDispatcherMt(collisionConfiguration, grainSize);
}
