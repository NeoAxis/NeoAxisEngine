#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btFixedConstraint* btFixedConstraint_new(btRigidBody* rbA, btRigidBody* rbB, const btTransform* frameInA, const btTransform* frameInB);
#ifdef __cplusplus
}
#endif
