#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btGeneric6DofSpringConstraint* btGeneric6DofSpringConstraint_new(btRigidBody* rbA, btRigidBody* rbB, const btTransform* frameInA, const btTransform* frameInB, bool useLinearReferenceFrameA);
	EXPORT btGeneric6DofSpringConstraint* btGeneric6DofSpringConstraint_new2(btRigidBody* rbB, const btTransform* frameInB, bool useLinearReferenceFrameB);
	EXPORT void btGeneric6DofSpringConstraint_enableSpring(btGeneric6DofSpringConstraint* obj, int index, bool onOff);
	EXPORT btScalar btGeneric6DofSpringConstraint_getDamping(btGeneric6DofSpringConstraint* obj, int index);
	EXPORT btScalar btGeneric6DofSpringConstraint_getEquilibriumPoint(btGeneric6DofSpringConstraint* obj, int index);
	EXPORT btScalar btGeneric6DofSpringConstraint_getStiffness(btGeneric6DofSpringConstraint* obj, int index);
	EXPORT bool btGeneric6DofSpringConstraint_isSpringEnabled(btGeneric6DofSpringConstraint* obj, int index);
	EXPORT void btGeneric6DofSpringConstraint_setDamping(btGeneric6DofSpringConstraint* obj, int index, btScalar damping);
	EXPORT void btGeneric6DofSpringConstraint_setEquilibriumPoint(btGeneric6DofSpringConstraint* obj);
	EXPORT void btGeneric6DofSpringConstraint_setEquilibriumPoint2(btGeneric6DofSpringConstraint* obj, int index);
	EXPORT void btGeneric6DofSpringConstraint_setEquilibriumPoint3(btGeneric6DofSpringConstraint* obj, int index, btScalar val);
	EXPORT void btGeneric6DofSpringConstraint_setStiffness(btGeneric6DofSpringConstraint* obj, int index, btScalar stiffness);
#ifdef __cplusplus
}
#endif
