#include <BulletCollision/CollisionDispatch/btCollisionConfiguration.h>
#include <BulletCollision/CollisionDispatch/btSimulationIslandManager.h>
#include <BulletDynamics/ConstraintSolver/btConstraintSolver.h>
#include <BulletDynamics/ConstraintSolver/btTypedConstraint.h>
#include <BulletDynamics/Dynamics/btActionInterface.h>
#include <BulletDynamics/Dynamics/btDiscreteDynamicsWorld.h>
#include <BulletDynamics/Dynamics/btRigidBody.h>

#include "btDiscreteDynamicsWorld_wrap.h"

btDiscreteDynamicsWorld* btDiscreteDynamicsWorld_new(btDispatcher* dispatcher, btBroadphaseInterface* pairCache,
	btConstraintSolver* constraintSolver, btCollisionConfiguration* collisionConfiguration)
{
	return new btDiscreteDynamicsWorld(dispatcher, pairCache, constraintSolver, collisionConfiguration);
}

void btDiscreteDynamicsWorld_applyGravity(btDiscreteDynamicsWorld* obj)
{
	obj->applyGravity();
}

void btDiscreteDynamicsWorld_debugDrawConstraint(btDiscreteDynamicsWorld* obj, btTypedConstraint* constraint)
{
	obj->debugDrawConstraint(constraint);
}

bool btDiscreteDynamicsWorld_getApplySpeculativeContactRestitution(btDiscreteDynamicsWorld* obj)
{
	return obj->getApplySpeculativeContactRestitution();
}

btCollisionWorld* btDiscreteDynamicsWorld_getCollisionWorld(btDiscreteDynamicsWorld* obj)
{
	return obj->getCollisionWorld();
}

bool btDiscreteDynamicsWorld_getLatencyMotionStateInterpolation(btDiscreteDynamicsWorld* obj)
{
	return obj->getLatencyMotionStateInterpolation();
}

btSimulationIslandManager* btDiscreteDynamicsWorld_getSimulationIslandManager(btDiscreteDynamicsWorld* obj)
{
	return obj->getSimulationIslandManager();
}

bool btDiscreteDynamicsWorld_getSynchronizeAllMotionStates(btDiscreteDynamicsWorld* obj)
{
	return obj->getSynchronizeAllMotionStates();
}

void btDiscreteDynamicsWorld_setApplySpeculativeContactRestitution(btDiscreteDynamicsWorld* obj,
	bool enable)
{
	obj->setApplySpeculativeContactRestitution(enable);
}

void btDiscreteDynamicsWorld_setLatencyMotionStateInterpolation(btDiscreteDynamicsWorld* obj,
	bool latencyInterpolation)
{
	obj->setLatencyMotionStateInterpolation(latencyInterpolation);
}

void btDiscreteDynamicsWorld_setNumTasks(btDiscreteDynamicsWorld* obj, int numTasks)
{
	obj->setNumTasks(numTasks);
}

void btDiscreteDynamicsWorld_setSynchronizeAllMotionStates(btDiscreteDynamicsWorld* obj,
	bool synchronizeAll)
{
	obj->setSynchronizeAllMotionStates(synchronizeAll);
}

void btDiscreteDynamicsWorld_synchronizeSingleMotionState(btDiscreteDynamicsWorld* obj,
	btRigidBody* body)
{
	obj->synchronizeSingleMotionState(body);
}

void btDiscreteDynamicsWorld_updateVehicles(btDiscreteDynamicsWorld* obj, btScalar timeStep)
{
	obj->updateVehicles(timeStep);
}
