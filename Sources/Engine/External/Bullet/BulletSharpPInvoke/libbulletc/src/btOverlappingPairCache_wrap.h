#include "main.h"

#ifndef BT_OVERLAPPING_PAIR_CACHE_H
#define p_btOverlapFilterCallback_needBroadphaseCollision void*
#define btOverlapFilterCallbackWrapper void
#else
typedef bool(*p_btOverlapFilterCallback_needBroadphaseCollision)(btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1);

class btOverlapFilterCallbackWrapper : public btOverlapFilterCallback
{
private:
	p_btOverlapFilterCallback_needBroadphaseCollision _needBroadphaseCollision;

public:
	btOverlapFilterCallbackWrapper(p_btOverlapFilterCallback_needBroadphaseCollision needBroadphaseCollision);

	virtual bool needBroadphaseCollision(btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1) const;
};
#endif

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT bool btOverlapCallback_processOverlap(btOverlapCallback* obj, btBroadphasePair* pair);
	EXPORT void btOverlapCallback_delete(btOverlapCallback* obj);

	EXPORT bool btOverlapFilterCallback_needBroadphaseCollision(btOverlapFilterCallback* obj, btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1);
	EXPORT void btOverlapFilterCallback_delete(btOverlapFilterCallback* obj);

	EXPORT void btOverlappingPairCache_cleanOverlappingPair(btOverlappingPairCache* obj, btBroadphasePair* pair, btDispatcher* dispatcher);
	EXPORT void btOverlappingPairCache_cleanProxyFromPairs(btOverlappingPairCache* obj, btBroadphaseProxy* proxy, btDispatcher* dispatcher);
	EXPORT btBroadphasePair* btOverlappingPairCache_findPair(btOverlappingPairCache* obj, btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1);
	EXPORT int btOverlappingPairCache_getNumOverlappingPairs(btOverlappingPairCache* obj);
	EXPORT btAlignedObjectArray_btBroadphasePair* btOverlappingPairCache_getOverlappingPairArray(btOverlappingPairCache* obj);
	EXPORT btBroadphasePair* btOverlappingPairCache_getOverlappingPairArrayPtr(btOverlappingPairCache* obj);
	EXPORT bool btOverlappingPairCache_hasDeferredRemoval(btOverlappingPairCache* obj);
	EXPORT void btOverlappingPairCache_processAllOverlappingPairs(btOverlappingPairCache* obj, btOverlapCallback* __unnamed0, btDispatcher* dispatcher);
	EXPORT void btOverlappingPairCache_setInternalGhostPairCallback(btOverlappingPairCache* obj, btOverlappingPairCallback* ghostPairCallback);
	EXPORT void btOverlappingPairCache_setOverlapFilterCallback(btOverlappingPairCache* obj, btOverlapFilterCallback* callback);
	EXPORT void btOverlappingPairCache_sortOverlappingPairs(btOverlappingPairCache* obj, btDispatcher* dispatcher);

	EXPORT btOverlapFilterCallbackWrapper* btOverlapFilterCallbackWrapper_new(p_btOverlapFilterCallback_needBroadphaseCollision needBroadphaseCollision);

	EXPORT btHashedOverlappingPairCache* btHashedOverlappingPairCache_new();
	EXPORT int btHashedOverlappingPairCache_GetCount(btHashedOverlappingPairCache* obj);
	EXPORT btOverlapFilterCallback* btHashedOverlappingPairCache_getOverlapFilterCallback(btHashedOverlappingPairCache* obj);
	EXPORT bool btHashedOverlappingPairCache_needsBroadphaseCollision(btHashedOverlappingPairCache* obj, btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1);

	EXPORT btSortedOverlappingPairCache* btSortedOverlappingPairCache_new();
	EXPORT btOverlapFilterCallback* btSortedOverlappingPairCache_getOverlapFilterCallback(btSortedOverlappingPairCache* obj);
	EXPORT bool btSortedOverlappingPairCache_needsBroadphaseCollision(btSortedOverlappingPairCache* obj, btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1);

	EXPORT btNullPairCache* btNullPairCache_new();
#ifdef __cplusplus
}
#endif
