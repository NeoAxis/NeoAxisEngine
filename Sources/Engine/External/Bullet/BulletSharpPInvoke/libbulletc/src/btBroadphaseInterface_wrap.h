#include "main.h"

#ifndef BT_BROADPHASE_INTERFACE_H
#define p_btBroadphaseAabbCallback_process void*
#define btBroadphaseAabbCallbackWrapper void
#define btBroadphaseRayCallbackWrapper void
#else
typedef bool (*p_btBroadphaseAabbCallback_process)(const btBroadphaseProxy* proxy);

class btBroadphaseAabbCallbackWrapper : public btBroadphaseAabbCallback
{
private:
	p_btBroadphaseAabbCallback_process _processCallback;

public:
	btBroadphaseAabbCallbackWrapper(p_btBroadphaseAabbCallback_process processCallback);

	virtual bool process(const btBroadphaseProxy* proxy);
};

class btBroadphaseRayCallbackWrapper : public btBroadphaseRayCallback
{
private:
	p_btBroadphaseAabbCallback_process _processCallback;

public:
	btBroadphaseRayCallbackWrapper(p_btBroadphaseAabbCallback_process processCallback);

	virtual bool process(const btBroadphaseProxy* proxy);
};
#endif

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btBroadphaseAabbCallbackWrapper* btBroadphaseAabbCallbackWrapper_new(p_btBroadphaseAabbCallback_process processCallback);

	EXPORT bool btBroadphaseAabbCallback_process(btBroadphaseAabbCallback* obj, const btBroadphaseProxy* proxy);
	EXPORT void btBroadphaseAabbCallback_delete(btBroadphaseAabbCallback* obj);

	EXPORT btBroadphaseRayCallbackWrapper* btBroadphaseRayCallbackWrapper_new(p_btBroadphaseAabbCallback_process processCallback);

	EXPORT btScalar btBroadphaseRayCallback_getLambda_max(btBroadphaseRayCallback* obj);
	EXPORT void btBroadphaseRayCallback_getRayDirectionInverse(btBroadphaseRayCallback* obj, btVector3* value);
	EXPORT unsigned int* btBroadphaseRayCallback_getSigns(btBroadphaseRayCallback* obj);
	EXPORT void btBroadphaseRayCallback_setLambda_max(btBroadphaseRayCallback* obj, btScalar value);
	EXPORT void btBroadphaseRayCallback_setRayDirectionInverse(btBroadphaseRayCallback* obj, const btVector3* value);

	EXPORT void btBroadphaseInterface_aabbTest(btBroadphaseInterface* obj, const btVector3* aabbMin, const btVector3* aabbMax, btBroadphaseAabbCallback* callback);
	EXPORT void btBroadphaseInterface_calculateOverlappingPairs(btBroadphaseInterface* obj, btDispatcher* dispatcher);
	EXPORT btBroadphaseProxy* btBroadphaseInterface_createProxy(btBroadphaseInterface* obj, const btVector3* aabbMin, const btVector3* aabbMax, int shapeType, void* userPtr, int collisionFilterGroup, int collisionFilterMask, btDispatcher* dispatcher);
	EXPORT void btBroadphaseInterface_destroyProxy(btBroadphaseInterface* obj, btBroadphaseProxy* proxy, btDispatcher* dispatcher);
	EXPORT void btBroadphaseInterface_getAabb(btBroadphaseInterface* obj, btBroadphaseProxy* proxy, btVector3* aabbMin, btVector3* aabbMax);
	EXPORT void btBroadphaseInterface_getBroadphaseAabb(btBroadphaseInterface* obj, btVector3* aabbMin, btVector3* aabbMax);
	EXPORT btOverlappingPairCache* btBroadphaseInterface_getOverlappingPairCache(btBroadphaseInterface* obj);
	EXPORT void btBroadphaseInterface_printStats(btBroadphaseInterface* obj);
	EXPORT void btBroadphaseInterface_rayTest(btBroadphaseInterface* obj, const btVector3* rayFrom, const btVector3* rayTo, btBroadphaseRayCallback* rayCallback);
	EXPORT void btBroadphaseInterface_rayTest2(btBroadphaseInterface* obj, const btVector3* rayFrom, const btVector3* rayTo, btBroadphaseRayCallback* rayCallback, const btVector3* aabbMin);
	EXPORT void btBroadphaseInterface_rayTest3(btBroadphaseInterface* obj, const btVector3* rayFrom, const btVector3* rayTo, btBroadphaseRayCallback* rayCallback, const btVector3* aabbMin, const btVector3* aabbMax);
	EXPORT void btBroadphaseInterface_resetPool(btBroadphaseInterface* obj, btDispatcher* dispatcher);
	EXPORT void btBroadphaseInterface_setAabb(btBroadphaseInterface* obj, btBroadphaseProxy* proxy, const btVector3* aabbMin, const btVector3* aabbMax, btDispatcher* dispatcher);
	EXPORT void btBroadphaseInterface_delete(btBroadphaseInterface* obj);
#ifdef __cplusplus
}
#endif
