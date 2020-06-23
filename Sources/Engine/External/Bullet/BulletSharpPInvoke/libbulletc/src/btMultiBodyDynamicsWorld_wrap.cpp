#include <BulletCollision/CollisionDispatch/btCollisionConfiguration.h>
#include <BulletDynamics/Featherstone/btMultiBody.h>
#include <BulletDynamics/Featherstone/btMultiBodyConstraint.h>
#include <BulletDynamics/Featherstone/btMultiBodyConstraintSolver.h>
#include <BulletDynamics/Featherstone/btMultiBodyDynamicsWorld.h>

#include "btMultiBodyDynamicsWorld_wrap.h"

btMultiBodyDynamicsWorld* btMultiBodyDynamicsWorld_new(btDispatcher* dispatcher,
	btBroadphaseInterface* pairCache, btMultiBodyConstraintSolver* constraintSolver,
	btCollisionConfiguration* collisionConfiguration)
{
	return new btMultiBodyDynamicsWorld(dispatcher, pairCache, constraintSolver,
		collisionConfiguration);
}

void btMultiBodyDynamicsWorld_addMultiBody(btMultiBodyDynamicsWorld* obj, btMultiBody* body,
	int group, int mask)
{
	obj->addMultiBody(body, group, mask);
}

void btMultiBodyDynamicsWorld_addMultiBodyConstraint(btMultiBodyDynamicsWorld* obj,
	btMultiBodyConstraint* constraint)
{
	obj->addMultiBodyConstraint(constraint);
}

void btMultiBodyDynamicsWorld_clearMultiBodyConstraintForces(btMultiBodyDynamicsWorld* obj)
{
	obj->clearMultiBodyConstraintForces();
}

void btMultiBodyDynamicsWorld_clearMultiBodyForces(btMultiBodyDynamicsWorld* obj)
{
	obj->clearMultiBodyForces();
}

void btMultiBodyDynamicsWorld_debugDrawMultiBodyConstraint(btMultiBodyDynamicsWorld* obj,
	btMultiBodyConstraint* constraint)
{
	obj->debugDrawMultiBodyConstraint(constraint);
}

void btMultiBodyDynamicsWorld_forwardKinematics(btMultiBodyDynamicsWorld* obj)
{
	obj->forwardKinematics();
}

btMultiBody* btMultiBodyDynamicsWorld_getMultiBody(btMultiBodyDynamicsWorld* obj,
	int mbIndex)
{
	return obj->getMultiBody(mbIndex);
}

btMultiBodyConstraint* btMultiBodyDynamicsWorld_getMultiBodyConstraint(btMultiBodyDynamicsWorld* obj,
	int constraintIndex)
{
	return obj->getMultiBodyConstraint(constraintIndex);
}

int btMultiBodyDynamicsWorld_getNumMultibodies(btMultiBodyDynamicsWorld* obj)
{
	return obj->getNumMultibodies();
}

int btMultiBodyDynamicsWorld_getNumMultiBodyConstraints(btMultiBodyDynamicsWorld* obj)
{
	return obj->getNumMultiBodyConstraints();
}

void btMultiBodyDynamicsWorld_integrateTransforms(btMultiBodyDynamicsWorld* obj,
	btScalar timeStep)
{
	obj->integrateTransforms(timeStep);
}

void btMultiBodyDynamicsWorld_removeMultiBody(btMultiBodyDynamicsWorld* obj, btMultiBody* body)
{
	obj->removeMultiBody(body);
}

void btMultiBodyDynamicsWorld_removeMultiBodyConstraint(btMultiBodyDynamicsWorld* obj,
	btMultiBodyConstraint* constraint)
{
	obj->removeMultiBodyConstraint(constraint);
}
