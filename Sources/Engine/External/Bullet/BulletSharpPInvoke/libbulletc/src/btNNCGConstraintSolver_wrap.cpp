#include <BulletDynamics/ConstraintSolver/btNNCGConstraintSolver.h>

#include "btNNCGConstraintSolver_wrap.h"

btNNCGConstraintSolver* btNNCGConstraintSolver_new()
{
	return new btNNCGConstraintSolver();
}

bool btNNCGConstraintSolver_getOnlyForNoneContact(btNNCGConstraintSolver* obj)
{
	return obj->m_onlyForNoneContact;
}

void btNNCGConstraintSolver_setOnlyForNoneContact(btNNCGConstraintSolver* obj, bool value)
{
	obj->m_onlyForNoneContact = value;
}
