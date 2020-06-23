#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btHinge2Constraint* btHinge2Constraint_new(btRigidBody* rbA, btRigidBody* rbB, btVector3* anchor, btVector3* axis1, btVector3* axis2);
	EXPORT void btHinge2Constraint_getAnchor(btHinge2Constraint* obj, btVector3* value);
	EXPORT void btHinge2Constraint_getAnchor2(btHinge2Constraint* obj, btVector3* value);
	EXPORT btScalar btHinge2Constraint_getAngle1(btHinge2Constraint* obj);
	EXPORT btScalar btHinge2Constraint_getAngle2(btHinge2Constraint* obj);
	EXPORT void btHinge2Constraint_getAxis1(btHinge2Constraint* obj, btVector3* value);
	EXPORT void btHinge2Constraint_getAxis2(btHinge2Constraint* obj, btVector3* value);
	EXPORT void btHinge2Constraint_setLowerLimit(btHinge2Constraint* obj, btScalar ang1min);
	EXPORT void btHinge2Constraint_setUpperLimit(btHinge2Constraint* obj, btScalar ang1max);
#ifdef __cplusplus
}
#endif
