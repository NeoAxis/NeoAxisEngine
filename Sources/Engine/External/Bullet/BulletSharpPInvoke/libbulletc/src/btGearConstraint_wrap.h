#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btGearConstraint* btGearConstraint_new(btRigidBody* rbA, btRigidBody* rbB, const btVector3* axisInA, const btVector3* axisInB, btScalar ratio);
	EXPORT void btGearConstraint_getAxisA(btGearConstraint* obj, btVector3* value);
	EXPORT void btGearConstraint_getAxisB(btGearConstraint* obj, btVector3* value);
	EXPORT btScalar btGearConstraint_getRatio(btGearConstraint* obj);
	EXPORT void btGearConstraint_setAxisA(btGearConstraint* obj, btVector3* axisA);
	EXPORT void btGearConstraint_setAxisB(btGearConstraint* obj, btVector3* axisB);
	EXPORT void btGearConstraint_setRatio(btGearConstraint* obj, btScalar ratio);
#ifdef __cplusplus
}
#endif
