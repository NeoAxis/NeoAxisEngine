#include <BulletDynamics/ConstraintSolver/btPoint2PointConstraint.h>

#include "conversion.h"
#include "btPoint2PointConstraint_wrap.h"

btConstraintSetting* btConstraintSetting_new()
{
	return new btConstraintSetting();
}

btScalar btConstraintSetting_getDamping(btConstraintSetting* obj)
{
	return obj->m_damping;
}

btScalar btConstraintSetting_getImpulseClamp(btConstraintSetting* obj)
{
	return obj->m_impulseClamp;
}

btScalar btConstraintSetting_getTau(btConstraintSetting* obj)
{
	return obj->m_tau;
}

void btConstraintSetting_setDamping(btConstraintSetting* obj, btScalar value)
{
	obj->m_damping = value;
}

void btConstraintSetting_setImpulseClamp(btConstraintSetting* obj, btScalar value)
{
	obj->m_impulseClamp = value;
}

void btConstraintSetting_setTau(btConstraintSetting* obj, btScalar value)
{
	obj->m_tau = value;
}

void btConstraintSetting_delete(btConstraintSetting* obj)
{
	delete obj;
}


btPoint2PointConstraint* btPoint2PointConstraint_new(btRigidBody* rbA, btRigidBody* rbB,
	const btVector3* pivotInA, const btVector3* pivotInB)
{
	BTVECTOR3_IN(pivotInA);
	BTVECTOR3_IN(pivotInB);
	return new btPoint2PointConstraint(*rbA, *rbB, BTVECTOR3_USE(pivotInA), BTVECTOR3_USE(pivotInB));
}

btPoint2PointConstraint* btPoint2PointConstraint_new2(btRigidBody* rbA, const btVector3* pivotInA)
{
	BTVECTOR3_IN(pivotInA);
	return new btPoint2PointConstraint(*rbA, BTVECTOR3_USE(pivotInA));
}

int btPoint2PointConstraint_getFlags(btPoint2PointConstraint* obj)
{
	return obj->getFlags();
}

void btPoint2PointConstraint_getInfo1NonVirtual(btPoint2PointConstraint* obj, btTypedConstraint_btConstraintInfo1* info)
{
	obj->getInfo1NonVirtual(info);
}

void btPoint2PointConstraint_getInfo2NonVirtual(btPoint2PointConstraint* obj, btTypedConstraint_btConstraintInfo2* info,
	const btTransform* body0_trans, const btTransform* body1_trans)
{
	BTTRANSFORM_IN(body0_trans);
	BTTRANSFORM_IN(body1_trans);
	obj->getInfo2NonVirtual(info, BTTRANSFORM_USE(body0_trans), BTTRANSFORM_USE(body1_trans));
}

void btPoint2PointConstraint_getPivotInA(btPoint2PointConstraint* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getPivotInA());
}

void btPoint2PointConstraint_getPivotInB(btPoint2PointConstraint* obj, btVector3* value)
{
	BTVECTOR3_COPY(value, &obj->getPivotInB());
}

btConstraintSetting* btPoint2PointConstraint_getSetting(btPoint2PointConstraint* obj)
{
	return &obj->m_setting;
}

bool btPoint2PointConstraint_getUseSolveConstraintObsolete(btPoint2PointConstraint* obj)
{
	return obj->m_useSolveConstraintObsolete;
}

void btPoint2PointConstraint_setPivotA(btPoint2PointConstraint* obj, const btVector3* pivotA)
{
	BTVECTOR3_IN(pivotA);
	obj->setPivotA(BTVECTOR3_USE(pivotA));
}

void btPoint2PointConstraint_setPivotB(btPoint2PointConstraint* obj, const btVector3* pivotB)
{
	BTVECTOR3_IN(pivotB);
	obj->setPivotB(BTVECTOR3_USE(pivotB));
}

void btPoint2PointConstraint_setUseSolveConstraintObsolete(btPoint2PointConstraint* obj,
	bool value)
{
	obj->m_useSolveConstraintObsolete = value;
}

void btPoint2PointConstraint_updateRHS(btPoint2PointConstraint* obj, btScalar timeStep)
{
	obj->updateRHS(timeStep);
}
