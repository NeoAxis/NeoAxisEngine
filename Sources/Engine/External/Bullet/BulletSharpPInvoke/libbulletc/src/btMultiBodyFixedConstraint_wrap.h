#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btMultiBodyFixedConstraint* btMultiBodyFixedConstraint_new(btMultiBody* body, int link, btRigidBody* bodyB, const btVector3* pivotInA, const btVector3* pivotInB, const btMatrix3x3* frameInA, const btMatrix3x3* frameInB);
	EXPORT btMultiBodyFixedConstraint* btMultiBodyFixedConstraint_new2(btMultiBody* bodyA, int linkA, btMultiBody* bodyB, int linkB, const btVector3* pivotInA, const btVector3* pivotInB, const btMatrix3x3* frameInA, const btMatrix3x3* frameInB);
	EXPORT void btMultiBodyFixedConstraint_getFrameInA(btMultiBodyFixedConstraint* obj, btMatrix3x3* value);
	EXPORT void btMultiBodyFixedConstraint_getFrameInB(btMultiBodyFixedConstraint* obj, btMatrix3x3* value);
	EXPORT void btMultiBodyFixedConstraint_getPivotInA(btMultiBodyFixedConstraint* obj, btVector3* value);
	EXPORT void btMultiBodyFixedConstraint_getPivotInB(btMultiBodyFixedConstraint* obj, btVector3* value);
	EXPORT void btMultiBodyFixedConstraint_setFrameInA(btMultiBodyFixedConstraint* obj, const btMatrix3x3* frameInA);
	EXPORT void btMultiBodyFixedConstraint_setFrameInB(btMultiBodyFixedConstraint* obj, const btMatrix3x3* frameInB);
	EXPORT void btMultiBodyFixedConstraint_setPivotInA(btMultiBodyFixedConstraint* obj, const btVector3* pivotInA);
	EXPORT void btMultiBodyFixedConstraint_setPivotInB(btMultiBodyFixedConstraint* obj, const btVector3* pivotInB);
#ifdef __cplusplus
}
#endif
