#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btMultiBodyDynamicsWorld* btMultiBodyDynamicsWorld_new(btDispatcher* dispatcher, btBroadphaseInterface* pairCache, btMultiBodyConstraintSolver* constraintSolver, btCollisionConfiguration* collisionConfiguration);
	EXPORT void btMultiBodyDynamicsWorld_addMultiBody(btMultiBodyDynamicsWorld* obj, btMultiBody* body, int group, int mask);
	EXPORT void btMultiBodyDynamicsWorld_addMultiBodyConstraint(btMultiBodyDynamicsWorld* obj, btMultiBodyConstraint* constraint);
	EXPORT void btMultiBodyDynamicsWorld_clearMultiBodyConstraintForces(btMultiBodyDynamicsWorld* obj);
	EXPORT void btMultiBodyDynamicsWorld_clearMultiBodyForces(btMultiBodyDynamicsWorld* obj);
	EXPORT void btMultiBodyDynamicsWorld_debugDrawMultiBodyConstraint(btMultiBodyDynamicsWorld* obj, btMultiBodyConstraint* constraint);
	EXPORT void btMultiBodyDynamicsWorld_forwardKinematics(btMultiBodyDynamicsWorld* obj);
	EXPORT btMultiBody* btMultiBodyDynamicsWorld_getMultiBody(btMultiBodyDynamicsWorld* obj, int mbIndex);
	EXPORT btMultiBodyConstraint* btMultiBodyDynamicsWorld_getMultiBodyConstraint(btMultiBodyDynamicsWorld* obj, int constraintIndex);
	EXPORT int btMultiBodyDynamicsWorld_getNumMultibodies(btMultiBodyDynamicsWorld* obj);
	EXPORT int btMultiBodyDynamicsWorld_getNumMultiBodyConstraints(btMultiBodyDynamicsWorld* obj);
	EXPORT void btMultiBodyDynamicsWorld_integrateTransforms(btMultiBodyDynamicsWorld* obj, btScalar timeStep);
	EXPORT void btMultiBodyDynamicsWorld_removeMultiBody(btMultiBodyDynamicsWorld* obj, btMultiBody* body);
	EXPORT void btMultiBodyDynamicsWorld_removeMultiBodyConstraint(btMultiBodyDynamicsWorld* obj, btMultiBodyConstraint* constraint);
#ifdef __cplusplus
}
#endif
