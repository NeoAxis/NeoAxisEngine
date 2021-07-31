#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btSoftRigidDynamicsWorld* btSoftRigidDynamicsWorld_new(btDispatcher* dispatcher, btBroadphaseInterface* pairCache, btConstraintSolver* constraintSolver, btCollisionConfiguration* collisionConfiguration, btSoftBodySolver* softBodySolver);
	EXPORT void btSoftRigidDynamicsWorld_addSoftBody(btSoftRigidDynamicsWorld* obj, btSoftBody* body);
	EXPORT void btSoftRigidDynamicsWorld_addSoftBody2(btSoftRigidDynamicsWorld* obj, btSoftBody* body, int collisionFilterGroup);
	EXPORT void btSoftRigidDynamicsWorld_addSoftBody3(btSoftRigidDynamicsWorld* obj, btSoftBody* body, int collisionFilterGroup, int collisionFilterMask);
	EXPORT int btSoftRigidDynamicsWorld_getDrawFlags(btSoftRigidDynamicsWorld* obj);
	EXPORT btAlignedObjectArray_btSoftBodyPtr* btSoftRigidDynamicsWorld_getSoftBodyArray(btSoftRigidDynamicsWorld* obj);
	EXPORT btSoftBodyWorldInfo* btSoftRigidDynamicsWorld_getWorldInfo(btSoftRigidDynamicsWorld* obj);
	EXPORT void btSoftRigidDynamicsWorld_removeSoftBody(btSoftRigidDynamicsWorld* obj, btSoftBody* body);
	EXPORT void btSoftRigidDynamicsWorld_setDrawFlags(btSoftRigidDynamicsWorld* obj, int f);
#ifdef __cplusplus
}
#endif
