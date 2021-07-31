#include <BulletCollision/NarrowPhaseCollision/btVoronoiSimplexSolver.h>

#include "conversion.h"
#include "btVoronoiSimplexSolver_wrap.h"

bool btUsageBitfield_getUnused1(btUsageBitfield* obj)
{
	return obj->unused1;
}

bool btUsageBitfield_getUnused2(btUsageBitfield* obj)
{
	return obj->unused2;
}

bool btUsageBitfield_getUnused3(btUsageBitfield* obj)
{
	return obj->unused3;
}

bool btUsageBitfield_getUnused4(btUsageBitfield* obj)
{
	return obj->unused4;
}

bool btUsageBitfield_getUsedVertexA(btUsageBitfield* obj)
{
	return obj->usedVertexA;
}

bool btUsageBitfield_getUsedVertexB(btUsageBitfield* obj)
{
	return obj->usedVertexB;
}

bool btUsageBitfield_getUsedVertexC(btUsageBitfield* obj)
{
	return obj->usedVertexC;
}

bool btUsageBitfield_getUsedVertexD(btUsageBitfield* obj)
{
	return obj->usedVertexD;
}

void btUsageBitfield_reset(btUsageBitfield* obj)
{
	obj->reset();
}

void btUsageBitfield_setUnused1(btUsageBitfield* obj, bool value)
{
	obj->unused1 = value;
}

void btUsageBitfield_setUnused2(btUsageBitfield* obj, bool value)
{
	obj->unused2 = value;
}

void btUsageBitfield_setUnused3(btUsageBitfield* obj, bool value)
{
	obj->unused3 = value;
}

void btUsageBitfield_setUnused4(btUsageBitfield* obj, bool value)
{
	obj->unused4 = value;
}

void btUsageBitfield_setUsedVertexA(btUsageBitfield* obj, bool value)
{
	obj->usedVertexA = value;
}

void btUsageBitfield_setUsedVertexB(btUsageBitfield* obj, bool value)
{
	obj->usedVertexB = value;
}

void btUsageBitfield_setUsedVertexC(btUsageBitfield* obj, bool value)
{
	obj->usedVertexC = value;
}

void btUsageBitfield_setUsedVertexD(btUsageBitfield* obj, bool value)
{
	obj->usedVertexD = value;
}


btSubSimplexClosestResult* btSubSimplexClosestResult_new()
{
	return new btSubSimplexClosestResult();
}

btScalar* btSubSimplexClosestResult_getBarycentricCoords(btSubSimplexClosestResult* obj)
{
	return obj->m_barycentricCoords;
}

void btSubSimplexClosestResult_getClosestPointOnSimplex(btSubSimplexClosestResult* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_closestPointOnSimplex);
}

bool btSubSimplexClosestResult_getDegenerate(btSubSimplexClosestResult* obj)
{
	return obj->m_degenerate;
}

btUsageBitfield* btSubSimplexClosestResult_getUsedVertices(btSubSimplexClosestResult* obj)
{
	return &obj->m_usedVertices;
}

bool btSubSimplexClosestResult_isValid(btSubSimplexClosestResult* obj)
{
	return obj->isValid();
}

void btSubSimplexClosestResult_reset(btSubSimplexClosestResult* obj)
{
	obj->reset();
}

void btSubSimplexClosestResult_setBarycentricCoordinates(btSubSimplexClosestResult* obj)
{
	obj->setBarycentricCoordinates();
}

void btSubSimplexClosestResult_setBarycentricCoordinates2(btSubSimplexClosestResult* obj,
	btScalar a)
{
	obj->setBarycentricCoordinates(a);
}

void btSubSimplexClosestResult_setBarycentricCoordinates3(btSubSimplexClosestResult* obj,
	btScalar a, btScalar b)
{
	obj->setBarycentricCoordinates(a, b);
}

void btSubSimplexClosestResult_setBarycentricCoordinates4(btSubSimplexClosestResult* obj,
	btScalar a, btScalar b, btScalar c)
{
	obj->setBarycentricCoordinates(a, b, c);
}

void btSubSimplexClosestResult_setBarycentricCoordinates5(btSubSimplexClosestResult* obj,
	btScalar a, btScalar b, btScalar c, btScalar d)
{
	obj->setBarycentricCoordinates(a, b, c, d);
}

void btSubSimplexClosestResult_setClosestPointOnSimplex(btSubSimplexClosestResult* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_closestPointOnSimplex, value);
}

void btSubSimplexClosestResult_setDegenerate(btSubSimplexClosestResult* obj, bool value)
{
	obj->m_degenerate = value;
}

void btSubSimplexClosestResult_setUsedVertices(btSubSimplexClosestResult* obj, const btUsageBitfield* value)
{
	obj->m_usedVertices = *value;
}

void btSubSimplexClosestResult_delete(btSubSimplexClosestResult* obj)
{
	delete obj;
}


btVoronoiSimplexSolver* btVoronoiSimplexSolver_new()
{
	return new btVoronoiSimplexSolver();
}

void btVoronoiSimplexSolver_addVertex(btVoronoiSimplexSolver* obj, const btVector3* w,
	const btVector3* p, const btVector3* q)
{
	BTVECTOR3_IN(w);
	BTVECTOR3_IN(p);
	BTVECTOR3_IN(q);
	obj->addVertex(BTVECTOR3_USE(w), BTVECTOR3_USE(p), BTVECTOR3_USE(q));
}

void btVoronoiSimplexSolver_backup_closest(btVoronoiSimplexSolver* obj, btVector3* v)
{
	BTVECTOR3_DEF(v);
	obj->backup_closest(BTVECTOR3_USE(v));
	BTVECTOR3_DEF_OUT(v);
}

bool btVoronoiSimplexSolver_closest(btVoronoiSimplexSolver* obj, btVector3* v)
{
	BTVECTOR3_DEF(v);
	bool ret = obj->closest(BTVECTOR3_USE(v));
	BTVECTOR3_DEF_OUT(v);
	return ret;
}

bool btVoronoiSimplexSolver_closestPtPointTetrahedron(btVoronoiSimplexSolver* obj,
	const btVector3* p, const btVector3* a, const btVector3* b, const btVector3* c,
	const btVector3* d, btSubSimplexClosestResult* finalResult)
{
	BTVECTOR3_IN(p);
	BTVECTOR3_IN(a);
	BTVECTOR3_IN(b);
	BTVECTOR3_IN(c);
	BTVECTOR3_IN(d);
	return obj->closestPtPointTetrahedron(BTVECTOR3_USE(p), BTVECTOR3_USE(a), BTVECTOR3_USE(b),
		BTVECTOR3_USE(c), BTVECTOR3_USE(d), *finalResult);
}

bool btVoronoiSimplexSolver_closestPtPointTriangle(btVoronoiSimplexSolver* obj, const btVector3* p,
	const btVector3* a, const btVector3* b, const btVector3* c, btSubSimplexClosestResult* result)
{
	BTVECTOR3_IN(p);
	BTVECTOR3_IN(a);
	BTVECTOR3_IN(b);
	BTVECTOR3_IN(c);
	return obj->closestPtPointTriangle(BTVECTOR3_USE(p), BTVECTOR3_USE(a), BTVECTOR3_USE(b),
		BTVECTOR3_USE(c), *result);
}

void btVoronoiSimplexSolver_compute_points(btVoronoiSimplexSolver* obj, btVector3* p1,
	btVector3* p2)
{
	BTVECTOR3_DEF(p1);
	BTVECTOR3_DEF(p2);
	obj->compute_points(BTVECTOR3_USE(p1), BTVECTOR3_USE(p2));
	BTVECTOR3_DEF_OUT(p1);
	BTVECTOR3_DEF_OUT(p2);
}

bool btVoronoiSimplexSolver_emptySimplex(btVoronoiSimplexSolver* obj)
{
	return obj->emptySimplex();
}

bool btVoronoiSimplexSolver_fullSimplex(btVoronoiSimplexSolver* obj)
{
	return obj->fullSimplex();
}

btSubSimplexClosestResult* btVoronoiSimplexSolver_getCachedBC(btVoronoiSimplexSolver* obj)
{
	return &obj->m_cachedBC;
}

void btVoronoiSimplexSolver_getCachedP1(btVoronoiSimplexSolver* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_cachedP1);
}

void btVoronoiSimplexSolver_getCachedP2(btVoronoiSimplexSolver* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_cachedP2);
}

void btVoronoiSimplexSolver_getCachedV(btVoronoiSimplexSolver* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_cachedV);
}

bool btVoronoiSimplexSolver_getCachedValidClosest(btVoronoiSimplexSolver* obj)
{
	return obj->m_cachedValidClosest;
}

btScalar btVoronoiSimplexSolver_getEqualVertexThreshold(btVoronoiSimplexSolver* obj)
{
	return obj->getEqualVertexThreshold();
}

void btVoronoiSimplexSolver_getLastW(btVoronoiSimplexSolver* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_lastW);
}

bool btVoronoiSimplexSolver_getNeedsUpdate(btVoronoiSimplexSolver* obj)
{
	return obj->m_needsUpdate;
}

int btVoronoiSimplexSolver_getNumVertices(btVoronoiSimplexSolver* obj)
{
	return obj->numVertices();
}

int btVoronoiSimplexSolver_getSimplex(btVoronoiSimplexSolver* obj, btVector3* pBuf,
	btVector3* qBuf, btVector3* yBuf)
{
	return obj->getSimplex(pBuf, qBuf, yBuf);
}

btVector3* btVoronoiSimplexSolver_getSimplexPointsP(btVoronoiSimplexSolver* obj)
{
	return obj->m_simplexPointsP;
}

btVector3* btVoronoiSimplexSolver_getSimplexPointsQ(btVoronoiSimplexSolver* obj)
{
	return obj->m_simplexPointsQ;
}

btVector3* btVoronoiSimplexSolver_getSimplexVectorW(btVoronoiSimplexSolver* obj)
{
	return obj->m_simplexVectorW;
}

bool btVoronoiSimplexSolver_inSimplex(btVoronoiSimplexSolver* obj, const btVector3* w)
{
	BTVECTOR3_IN(w);
	return obj->inSimplex(BTVECTOR3_USE(w));
}

btScalar btVoronoiSimplexSolver_maxVertex(btVoronoiSimplexSolver* obj)
{
	return obj->maxVertex();
}

int btVoronoiSimplexSolver_numVertices(btVoronoiSimplexSolver* obj)
{
	return obj->numVertices();
}

int btVoronoiSimplexSolver_pointOutsideOfPlane(btVoronoiSimplexSolver* obj, const btVector3* p,
	const btVector3* a, const btVector3* b, const btVector3* c, const btVector3* d)
{
	BTVECTOR3_IN(p);
	BTVECTOR3_IN(a);
	BTVECTOR3_IN(b);
	BTVECTOR3_IN(c);
	BTVECTOR3_IN(d);
	return obj->pointOutsideOfPlane(BTVECTOR3_USE(p), BTVECTOR3_USE(a), BTVECTOR3_USE(b),
		BTVECTOR3_USE(c), BTVECTOR3_USE(d));
}

void btVoronoiSimplexSolver_reduceVertices(btVoronoiSimplexSolver* obj, const btUsageBitfield* usedVerts)
{
	obj->reduceVertices(*usedVerts);
}

void btVoronoiSimplexSolver_removeVertex(btVoronoiSimplexSolver* obj, int index)
{
	obj->removeVertex(index);
}

void btVoronoiSimplexSolver_reset(btVoronoiSimplexSolver* obj)
{
	obj->reset();
}

void btVoronoiSimplexSolver_setCachedBC(btVoronoiSimplexSolver* obj, const btSubSimplexClosestResult* value)
{
	obj->m_cachedBC = *value;
}

void btVoronoiSimplexSolver_setCachedP1(btVoronoiSimplexSolver* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_cachedP1, value);
}

void btVoronoiSimplexSolver_setCachedP2(btVoronoiSimplexSolver* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_cachedP2, value);
}

void btVoronoiSimplexSolver_setCachedV(btVoronoiSimplexSolver* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_cachedV, value);
}

void btVoronoiSimplexSolver_setCachedValidClosest(btVoronoiSimplexSolver* obj, bool value)
{
	obj->m_cachedValidClosest = value;
}

void btVoronoiSimplexSolver_setEqualVertexThreshold(btVoronoiSimplexSolver* obj,
	btScalar threshold)
{
	obj->setEqualVertexThreshold(threshold);
}

void btVoronoiSimplexSolver_setLastW(btVoronoiSimplexSolver* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_lastW, value);
}

void btVoronoiSimplexSolver_setNeedsUpdate(btVoronoiSimplexSolver* obj, bool value)
{
	obj->m_needsUpdate = value;
}

void btVoronoiSimplexSolver_setNumVertices(btVoronoiSimplexSolver* obj, int value)
{
	obj->m_numVertices = value;
}

bool btVoronoiSimplexSolver_updateClosestVectorAndPoints(btVoronoiSimplexSolver* obj)
{
	return obj->updateClosestVectorAndPoints();
}

void btVoronoiSimplexSolver_delete(btVoronoiSimplexSolver* obj)
{
	delete obj;
}
