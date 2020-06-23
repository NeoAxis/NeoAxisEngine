#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btPersistentManifold* btContactConstraint_getContactManifold(btContactConstraint* obj);
	EXPORT void btContactConstraint_setContactManifold(btContactConstraint* obj, btPersistentManifold* contactManifold);
#ifdef __cplusplus
}
#endif
