#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btUniversalConstraint* btUniversalConstraint_new(btRigidBody* rbA, btRigidBody* rbB, const btVector3* anchor, const btVector3* axis1, const btVector3* axis2);
	EXPORT void btUniversalConstraint_getAnchor(btUniversalConstraint* obj, btVector3* value);
	EXPORT void btUniversalConstraint_getAnchor2(btUniversalConstraint* obj, btVector3* value);
	EXPORT btScalar btUniversalConstraint_getAngle1(btUniversalConstraint* obj);
	EXPORT btScalar btUniversalConstraint_getAngle2(btUniversalConstraint* obj);
	EXPORT void btUniversalConstraint_getAxis1(btUniversalConstraint* obj, btVector3* value);
	EXPORT void btUniversalConstraint_getAxis2(btUniversalConstraint* obj, btVector3* value);
	EXPORT void btUniversalConstraint_setLowerLimit(btUniversalConstraint* obj, btScalar ang1min, btScalar ang2min);
	EXPORT void btUniversalConstraint_setUpperLimit(btUniversalConstraint* obj, btScalar ang1max, btScalar ang2max);
#ifdef __cplusplus
}
#endif
