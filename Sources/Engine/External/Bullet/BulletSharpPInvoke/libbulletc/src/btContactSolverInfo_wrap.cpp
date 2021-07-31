#include <BulletDynamics/ConstraintSolver/btContactSolverInfo.h>

#include "btContactSolverInfo_wrap.h"

btContactSolverInfoData* btContactSolverInfoData_new()
{
	return new btContactSolverInfoData();
}

btScalar btContactSolverInfoData_getDamping(btContactSolverInfoData* obj)
{
	return obj->m_damping;
}

btScalar btContactSolverInfoData_getErp(btContactSolverInfoData* obj)
{
	return obj->m_erp;
}

btScalar btContactSolverInfoData_getErp2(btContactSolverInfoData* obj)
{
	return obj->m_erp2;
}

btScalar btContactSolverInfoData_getFriction(btContactSolverInfoData* obj)
{
	return obj->m_friction;
}

btScalar btContactSolverInfoData_getFrictionCfm(btContactSolverInfoData* obj)
{
	return obj->m_frictionCFM;
}

btScalar btContactSolverInfoData_getFrictionErp(btContactSolverInfoData* obj)
{
	return obj->m_frictionERP;
}

btScalar btContactSolverInfoData_getGlobalCfm(btContactSolverInfoData* obj)
{
	return obj->m_globalCfm;
}

btScalar btContactSolverInfoData_getLinearSlop(btContactSolverInfoData* obj)
{
	return obj->m_linearSlop;
}

btScalar btContactSolverInfoData_getMaxErrorReduction(btContactSolverInfoData* obj)
{
	return obj->m_maxErrorReduction;
}

btScalar btContactSolverInfoData_getMaxGyroscopicForce(btContactSolverInfoData* obj)
{
	return obj->m_maxGyroscopicForce;
}

int btContactSolverInfoData_getMinimumSolverBatchSize(btContactSolverInfoData* obj)
{
	return obj->m_minimumSolverBatchSize;
}

int btContactSolverInfoData_getNumIterations(btContactSolverInfoData* obj)
{
	return obj->m_numIterations;
}

int btContactSolverInfoData_getRestingContactRestitutionThreshold(btContactSolverInfoData* obj)
{
	return obj->m_restingContactRestitutionThreshold;
}

btScalar btContactSolverInfoData_getRestitution(btContactSolverInfoData* obj)
{
	return obj->m_restitution;
}

btScalar btContactSolverInfoData_getSingleAxisRollingFrictionThreshold(btContactSolverInfoData* obj)
{
	return obj->m_singleAxisRollingFrictionThreshold;
}

int btContactSolverInfoData_getSolverMode(btContactSolverInfoData* obj)
{
	return obj->m_solverMode;
}

btScalar btContactSolverInfoData_getSor(btContactSolverInfoData* obj)
{
	return obj->m_sor;
}

int btContactSolverInfoData_getSplitImpulse(btContactSolverInfoData* obj)
{
	return obj->m_splitImpulse;
}

btScalar btContactSolverInfoData_getSplitImpulsePenetrationThreshold(btContactSolverInfoData* obj)
{
	return obj->m_splitImpulsePenetrationThreshold;
}

btScalar btContactSolverInfoData_getSplitImpulseTurnErp(btContactSolverInfoData* obj)
{
	return obj->m_splitImpulseTurnErp;
}

btScalar btContactSolverInfoData_getTau(btContactSolverInfoData* obj)
{
	return obj->m_tau;
}

btScalar btContactSolverInfoData_getTimeStep(btContactSolverInfoData* obj)
{
	return obj->m_timeStep;
}

btScalar btContactSolverInfoData_getWarmstartingFactor(btContactSolverInfoData* obj)
{
	return obj->m_warmstartingFactor;
}

void btContactSolverInfoData_setDamping(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_damping = value;
}

void btContactSolverInfoData_setErp(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_erp = value;
}

void btContactSolverInfoData_setErp2(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_erp2 = value;
}

void btContactSolverInfoData_setFriction(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_friction = value;
}

void btContactSolverInfoData_setFrictionCfm(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_frictionCFM = value;
}

void btContactSolverInfoData_setFrictionErp(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_frictionERP = value;
}

void btContactSolverInfoData_setGlobalCfm(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_globalCfm = value;
}

void btContactSolverInfoData_setLinearSlop(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_linearSlop = value;
}

void btContactSolverInfoData_setMaxErrorReduction(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_maxErrorReduction = value;
}

void btContactSolverInfoData_setMaxGyroscopicForce(btContactSolverInfoData* obj,
	btScalar value)
{
	obj->m_maxGyroscopicForce = value;
}

void btContactSolverInfoData_setMinimumSolverBatchSize(btContactSolverInfoData* obj,
	int value)
{
	obj->m_minimumSolverBatchSize = value;
}

void btContactSolverInfoData_setNumIterations(btContactSolverInfoData* obj, int value)
{
	obj->m_numIterations = value;
}

void btContactSolverInfoData_setRestingContactRestitutionThreshold(btContactSolverInfoData* obj,
	int value)
{
	obj->m_restingContactRestitutionThreshold = value;
}

void btContactSolverInfoData_setRestitution(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_restitution = value;
}

void btContactSolverInfoData_setSingleAxisRollingFrictionThreshold(btContactSolverInfoData* obj,
	btScalar value)
{
	obj->m_singleAxisRollingFrictionThreshold = value;
}

void btContactSolverInfoData_setSolverMode(btContactSolverInfoData* obj, int value)
{
	obj->m_solverMode = value;
}

void btContactSolverInfoData_setSor(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_sor = value;
}

void btContactSolverInfoData_setSplitImpulse(btContactSolverInfoData* obj, int value)
{
	obj->m_splitImpulse = value;
}

void btContactSolverInfoData_setSplitImpulsePenetrationThreshold(btContactSolverInfoData* obj,
	btScalar value)
{
	obj->m_splitImpulsePenetrationThreshold = value;
}

void btContactSolverInfoData_setSplitImpulseTurnErp(btContactSolverInfoData* obj,
	btScalar value)
{
	obj->m_splitImpulseTurnErp = value;
}

void btContactSolverInfoData_setTau(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_tau = value;
}

void btContactSolverInfoData_setTimeStep(btContactSolverInfoData* obj, btScalar value)
{
	obj->m_timeStep = value;
}

void btContactSolverInfoData_setWarmstartingFactor(btContactSolverInfoData* obj,
	btScalar value)
{
	obj->m_warmstartingFactor = value;
}

void btContactSolverInfoData_delete(btContactSolverInfoData* obj)
{
	delete obj;
}


btContactSolverInfo* btContactSolverInfo_new()
{
	return new btContactSolverInfo();
}
