#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btMultiBodySliderConstraint* btMultiBodySliderConstraint_new(btMultiBody* body, int link, btRigidBody* bodyB, const btVector3* pivotInA, const btVector3* pivotInB, const btMatrix3x3* frameInA, const btMatrix3x3* frameInB, const btVector3* jointAxis);
	EXPORT btMultiBodySliderConstraint* btMultiBodySliderConstraint_new2(btMultiBody* bodyA, int linkA, btMultiBody* bodyB, int linkB, const btVector3* pivotInA, const btVector3* pivotInB, const btMatrix3x3* frameInA, const btMatrix3x3* frameInB, const btVector3* jointAxis);
	EXPORT void btMultiBodySliderConstraint_getFrameInA(btMultiBodySliderConstraint* obj, btMatrix3x3* value);
	EXPORT void btMultiBodySliderConstraint_getFrameInB(btMultiBodySliderConstraint* obj, btMatrix3x3* value);
	EXPORT void btMultiBodySliderConstraint_getJointAxis(btMultiBodySliderConstraint* obj, btVector3* value);
	EXPORT void btMultiBodySliderConstraint_getPivotInA(btMultiBodySliderConstraint* obj, btVector3* value);
	EXPORT void btMultiBodySliderConstraint_getPivotInB(btMultiBodySliderConstraint* obj, btVector3* value);
	EXPORT void btMultiBodySliderConstraint_setFrameInA(btMultiBodySliderConstraint* obj, const btMatrix3x3* frameInA);
	EXPORT void btMultiBodySliderConstraint_setFrameInB(btMultiBodySliderConstraint* obj, const btMatrix3x3* frameInB);
	EXPORT void btMultiBodySliderConstraint_setJointAxis(btMultiBodySliderConstraint* obj, const btVector3* jointAxis);
	EXPORT void btMultiBodySliderConstraint_setPivotInA(btMultiBodySliderConstraint* obj, const btVector3* pivotInA);
	EXPORT void btMultiBodySliderConstraint_setPivotInB(btMultiBodySliderConstraint* obj, const btVector3* pivotInB);
#ifdef __cplusplus
}
#endif
