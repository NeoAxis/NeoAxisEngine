#include <BulletSoftBody/btSoftBodySolvers.h>

#include "btSoftBodySolvers_wrap.h"

bool btSoftBodySolver_checkInitialized(btSoftBodySolver* obj)
{
	return obj->checkInitialized();
}

void btSoftBodySolver_copyBackToSoftBodies(btSoftBodySolver* obj, bool bMove)
{
	obj->copyBackToSoftBodies(bMove);
}

int btSoftBodySolver_getNumberOfPositionIterations(btSoftBodySolver* obj)
{
	return obj->getNumberOfPositionIterations();
}

int btSoftBodySolver_getNumberOfVelocityIterations(btSoftBodySolver* obj)
{
	return obj->getNumberOfVelocityIterations();
}
/*
SolverTypes btSoftBodySolver_getSolverType(btSoftBodySolver* obj)
{
	return obj->getSolverType();
}
*/
float btSoftBodySolver_getTimeScale(btSoftBodySolver* obj)
{
	return obj->getTimeScale();
}
/*
void btSoftBodySolver_optimize(btSoftBodySolver* obj, btAlignedObjectArray_btSoftBodyPtr* softBodies,
	bool forceUpdate)
{
	obj->optimize(*softBodies, forceUpdate);
}
*/
void btSoftBodySolver_predictMotion(btSoftBodySolver* obj, float solverdt)
{
	obj->predictMotion(solverdt);
}
/*
void btSoftBodySolver_processCollision(btSoftBodySolver* obj, btSoftBody* __unnamed0,
	const btCollisionObjectWrapper* __unnamed1)
{
	obj->processCollision(__unnamed0, __unnamed1);
}

void btSoftBodySolver_processCollision2(btSoftBodySolver* obj, btSoftBody* __unnamed0,
	btSoftBody* __unnamed1)
{
	obj->processCollision(__unnamed0, __unnamed1);
}
*/
void btSoftBodySolver_setNumberOfPositionIterations(btSoftBodySolver* obj, int iterations)
{
	obj->setNumberOfPositionIterations(iterations);
}

void btSoftBodySolver_setNumberOfVelocityIterations(btSoftBodySolver* obj, int iterations)
{
	obj->setNumberOfVelocityIterations(iterations);
}

void btSoftBodySolver_solveConstraints(btSoftBodySolver* obj, float solverdt)
{
	obj->solveConstraints(solverdt);
}

void btSoftBodySolver_updateSoftBodies(btSoftBodySolver* obj)
{
	obj->updateSoftBodies();
}

void btSoftBodySolver_delete(btSoftBodySolver* obj)
{
	delete obj;
}

/*
void btSoftBodySolverOutput_copySoftBodyToVertexBuffer(btSoftBodySolverOutput* obj,
	const btSoftBody* softBody, btVertexBufferDescriptor* vertexBuffer)
{
	obj->copySoftBodyToVertexBuffer(softBody, vertexBuffer);
}
*/
void btSoftBodySolverOutput_delete(btSoftBodySolverOutput* obj)
{
	delete obj;
}
