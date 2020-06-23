#include <BulletCollision/BroadphaseCollision/btDbvtBroadphase.h>
#include <BulletCollision/BroadphaseCollision/btDispatcher.h>

#include "conversion.h"
#include "btDbvtBroadphase_wrap.h"

btDbvtNode* btDbvtProxy_getLeaf(btDbvtProxy* obj)
{
	return obj->leaf;
}

btDbvtProxy** btDbvtProxy_getLinks(btDbvtProxy* obj)
{
	return obj->links;
}

int btDbvtProxy_getStage(btDbvtProxy* obj)
{
	return obj->stage;
}

void btDbvtProxy_setLeaf(btDbvtProxy* obj, btDbvtNode* value)
{
	obj->leaf = value;
}

void btDbvtProxy_setStage(btDbvtProxy* obj, int value)
{
	obj->stage = value;
}


btDbvtBroadphase* btDbvtBroadphase_new(btOverlappingPairCache* paircache)
{
	return new btDbvtBroadphase(paircache);
}

void btDbvtBroadphase_benchmark(btBroadphaseInterface* __unnamed0)
{
	btDbvtBroadphase::benchmark(__unnamed0);
}

void btDbvtBroadphase_collide(btDbvtBroadphase* obj, btDispatcher* dispatcher)
{
	obj->collide(dispatcher);
}

int btDbvtBroadphase_getCid(btDbvtBroadphase* obj)
{
	return obj->m_cid;
}

int btDbvtBroadphase_getCupdates(btDbvtBroadphase* obj)
{
	return obj->m_cupdates;
}

bool btDbvtBroadphase_getDeferedcollide(btDbvtBroadphase* obj)
{
	return obj->m_deferedcollide;
}

int btDbvtBroadphase_getDupdates(btDbvtBroadphase* obj)
{
	return obj->m_dupdates;
}

int btDbvtBroadphase_getFixedleft(btDbvtBroadphase* obj)
{
	return obj->m_fixedleft;
}

int btDbvtBroadphase_getFupdates(btDbvtBroadphase* obj)
{
	return obj->m_fupdates;
}

int btDbvtBroadphase_getGid(btDbvtBroadphase* obj)
{
	return obj->m_gid;
}

bool btDbvtBroadphase_getNeedcleanup(btDbvtBroadphase* obj)
{
	return obj->m_needcleanup;
}

int btDbvtBroadphase_getNewpairs(btDbvtBroadphase* obj)
{
	return obj->m_newpairs;
}

btOverlappingPairCache* btDbvtBroadphase_getPaircache(btDbvtBroadphase* obj)
{
	return obj->m_paircache;
}

int btDbvtBroadphase_getPid(btDbvtBroadphase* obj)
{
	return obj->m_pid;
}

btScalar btDbvtBroadphase_getPrediction(btDbvtBroadphase* obj)
{
	return obj->m_prediction;
}

bool btDbvtBroadphase_getReleasepaircache(btDbvtBroadphase* obj)
{
	return obj->m_releasepaircache;
}

btDbvt* btDbvtBroadphase_getSets(btDbvtBroadphase* obj)
{
	return obj->m_sets;
}

int btDbvtBroadphase_getStageCurrent(btDbvtBroadphase* obj)
{
	return obj->m_stageCurrent;
}

btDbvtProxy** btDbvtBroadphase_getStageRoots(btDbvtBroadphase* obj)
{
	return obj->m_stageRoots;
}

unsigned int btDbvtBroadphase_getUpdates_call(btDbvtBroadphase* obj)
{
	return obj->m_updates_call;
}

unsigned int btDbvtBroadphase_getUpdates_done(btDbvtBroadphase* obj)
{
	return obj->m_updates_done;
}

btScalar btDbvtBroadphase_getUpdates_ratio(btDbvtBroadphase* obj)
{
	return obj->m_updates_ratio;
}

btScalar btDbvtBroadphase_getVelocityPrediction(btDbvtBroadphase* obj)
{
	return obj->getVelocityPrediction();
}

void btDbvtBroadphase_optimize(btDbvtBroadphase* obj)
{
	obj->optimize();
}

void btDbvtBroadphase_performDeferredRemoval(btDbvtBroadphase* obj, btDispatcher* dispatcher)
{
	obj->performDeferredRemoval(dispatcher);
}

void btDbvtBroadphase_setAabbForceUpdate(btDbvtBroadphase* obj, btBroadphaseProxy* absproxy,
	const btVector3* aabbMin, const btVector3* aabbMax, btDispatcher* __unnamed3)
{
	BTVECTOR3_IN(aabbMin);
	BTVECTOR3_IN(aabbMax);
	obj->setAabbForceUpdate(absproxy, BTVECTOR3_USE(aabbMin), BTVECTOR3_USE(aabbMax),
		__unnamed3);
}

void btDbvtBroadphase_setCid(btDbvtBroadphase* obj, int value)
{
	obj->m_cid = value;
}

void btDbvtBroadphase_setCupdates(btDbvtBroadphase* obj, int value)
{
	obj->m_cupdates = value;
}

void btDbvtBroadphase_setDeferedcollide(btDbvtBroadphase* obj, bool value)
{
	obj->m_deferedcollide = value;
}

void btDbvtBroadphase_setDupdates(btDbvtBroadphase* obj, int value)
{
	obj->m_dupdates = value;
}

void btDbvtBroadphase_setFixedleft(btDbvtBroadphase* obj, int value)
{
	obj->m_fixedleft = value;
}

void btDbvtBroadphase_setFupdates(btDbvtBroadphase* obj, int value)
{
	obj->m_fupdates = value;
}

void btDbvtBroadphase_setGid(btDbvtBroadphase* obj, int value)
{
	obj->m_gid = value;
}

void btDbvtBroadphase_setNeedcleanup(btDbvtBroadphase* obj, bool value)
{
	obj->m_needcleanup = value;
}

void btDbvtBroadphase_setNewpairs(btDbvtBroadphase* obj, int value)
{
	obj->m_newpairs = value;
}

void btDbvtBroadphase_setPaircache(btDbvtBroadphase* obj, btOverlappingPairCache* value)
{
	obj->m_paircache = value;
}

void btDbvtBroadphase_setPid(btDbvtBroadphase* obj, int value)
{
	obj->m_pid = value;
}

void btDbvtBroadphase_setPrediction(btDbvtBroadphase* obj, btScalar value)
{
	obj->m_prediction = value;
}

void btDbvtBroadphase_setReleasepaircache(btDbvtBroadphase* obj, bool value)
{
	obj->m_releasepaircache = value;
}

void btDbvtBroadphase_setStageCurrent(btDbvtBroadphase* obj, int value)
{
	obj->m_stageCurrent = value;
}

void btDbvtBroadphase_setUpdates_call(btDbvtBroadphase* obj, unsigned int value)
{
	obj->m_updates_call = value;
}

void btDbvtBroadphase_setUpdates_done(btDbvtBroadphase* obj, unsigned int value)
{
	obj->m_updates_done = value;
}

void btDbvtBroadphase_setUpdates_ratio(btDbvtBroadphase* obj, btScalar value)
{
	obj->m_updates_ratio = value;
}

void btDbvtBroadphase_setVelocityPrediction(btDbvtBroadphase* obj, btScalar prediction)
{
	obj->setVelocityPrediction(prediction);
}
