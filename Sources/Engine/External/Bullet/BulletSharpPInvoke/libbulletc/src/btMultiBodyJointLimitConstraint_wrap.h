#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btMultiBodyJointLimitConstraint* btMultiBodyJointLimitConstraint_new(btMultiBody* body, int link, btScalar lower, btScalar upper);
#ifdef __cplusplus
}
#endif
