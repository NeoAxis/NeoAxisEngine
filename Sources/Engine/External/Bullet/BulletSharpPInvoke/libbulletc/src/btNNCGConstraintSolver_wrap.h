#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btNNCGConstraintSolver* btNNCGConstraintSolver_new();
	EXPORT bool btNNCGConstraintSolver_getOnlyForNoneContact(btNNCGConstraintSolver* obj);
	EXPORT void btNNCGConstraintSolver_setOnlyForNoneContact(btNNCGConstraintSolver* obj, bool value);
#ifdef __cplusplus
}
#endif
