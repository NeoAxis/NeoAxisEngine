#include "main.h"

#ifndef BT_COLLISION_WORLD_H
#define p_btCollisionWorld_ContactResultCallback_addSingleResult void*
#define p_btCollisionWorld_ContactResultCallback_needsCollision void*
#define p_btCollisionWorld_ConvexResultCallback_addSingleResult void*
#define p_btCollisionWorld_ConvexResultCallback_needsCollision void*
#define p_btCollisionWorld_RayResultCallback_addSingleResult void*
#define p_btCollisionWorld_RayResultCallback_needsCollision void*
#define btCollisionWorld_ContactResultCallbackWrapper void
#define btCollisionWorld_ConvexResultCallbackWrapper void
#define btCollisionWorld_RayResultCallbackWrapper void
#else
typedef btScalar (*p_btCollisionWorld_ContactResultCallback_addSingleResult)(btManifoldPoint& cp,
	const btCollisionObjectWrapper* colObj0Wrap, int partId0, int index0, const btCollisionObjectWrapper* colObj1Wrap,
	int partId1, int index1);
typedef bool (*p_btCollisionWorld_ContactResultCallback_needsCollision)(btBroadphaseProxy* proxy0);

class btCollisionWorld_ContactResultCallbackWrapper : public btCollisionWorld_ContactResultCallback
{
private:
	p_btCollisionWorld_ContactResultCallback_addSingleResult _addSingleResultCallback;
	p_btCollisionWorld_ContactResultCallback_needsCollision _needsCollisionCallback;

public:
	btCollisionWorld_ContactResultCallbackWrapper(p_btCollisionWorld_ContactResultCallback_addSingleResult addSingleResultCallback,
		p_btCollisionWorld_ContactResultCallback_needsCollision needsCollisionCallback);

	virtual btScalar addSingleResult(btManifoldPoint& cp, const btCollisionObjectWrapper* colObj0Wrap,
		int partId0, int index0, const btCollisionObjectWrapper* colObj1Wrap, int partId1,
		int index1);
	virtual bool needsCollision(btBroadphaseProxy* proxy0) const;

	virtual bool baseNeedsCollision(btBroadphaseProxy* proxy0) const;
};

typedef btScalar (*p_btCollisionWorld_ConvexResultCallback_addSingleResult)(btCollisionWorld_LocalConvexResult& convexResult,
	bool normalInWorldSpace);
typedef bool (*p_btCollisionWorld_ConvexResultCallback_needsCollision)(btBroadphaseProxy* proxy0);

class btCollisionWorld_ConvexResultCallbackWrapper : public btCollisionWorld_ConvexResultCallback
{
private:
	p_btCollisionWorld_ConvexResultCallback_addSingleResult _addSingleResultCallback;
	p_btCollisionWorld_ConvexResultCallback_needsCollision _needsCollisionCallback;

public:
	btCollisionWorld_ConvexResultCallbackWrapper(p_btCollisionWorld_ConvexResultCallback_addSingleResult addSingleResultCallback,
		p_btCollisionWorld_ConvexResultCallback_needsCollision needsCollisionCallback);

	virtual btScalar addSingleResult(btCollisionWorld_LocalConvexResult& convexResult,
		bool normalInWorldSpace);
	virtual bool needsCollision(btBroadphaseProxy* proxy0) const;

	virtual bool baseNeedsCollision(btBroadphaseProxy* proxy0) const;
};

typedef btScalar (*p_btCollisionWorld_RayResultCallback_addSingleResult)(btCollisionWorld_LocalRayResult& rayResult,
	bool normalInWorldSpace);
typedef bool (*p_btCollisionWorld_RayResultCallback_needsCollision)(btBroadphaseProxy* proxy0);

class btCollisionWorld_RayResultCallbackWrapper : public btCollisionWorld_RayResultCallback
{
private:
	p_btCollisionWorld_RayResultCallback_addSingleResult _addSingleResultCallback;
	p_btCollisionWorld_RayResultCallback_needsCollision _needsCollisionCallback;

public:
	btCollisionWorld_RayResultCallbackWrapper(p_btCollisionWorld_RayResultCallback_addSingleResult addSingleResultCallback,
		p_btCollisionWorld_RayResultCallback_needsCollision needsCollisionCallback);

	virtual btScalar addSingleResult(btCollisionWorld_LocalRayResult& rayResult,
		bool normalInWorldSpace);
	virtual bool needsCollision(btBroadphaseProxy* proxy0) const;

	virtual bool baseNeedsCollision(btBroadphaseProxy* proxy0) const;
};
#endif

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btCollisionWorld_AllHitsRayResultCallback* btCollisionWorld_AllHitsRayResultCallback_new(const btVector3* rayFromWorld, const btVector3* rayToWorld);
	EXPORT btAlignedObjectArray_const_btCollisionObjectPtr* btCollisionWorld_AllHitsRayResultCallback_getCollisionObjects(btCollisionWorld_AllHitsRayResultCallback* obj);
	EXPORT btAlignedObjectArray_btScalar* btCollisionWorld_AllHitsRayResultCallback_getHitFractions(btCollisionWorld_AllHitsRayResultCallback* obj);
	EXPORT btAlignedObjectArray_btVector3* btCollisionWorld_AllHitsRayResultCallback_getHitNormalWorld(btCollisionWorld_AllHitsRayResultCallback* obj);
	EXPORT btAlignedObjectArray_btVector3* btCollisionWorld_AllHitsRayResultCallback_getHitPointWorld(btCollisionWorld_AllHitsRayResultCallback* obj);
	EXPORT void btCollisionWorld_AllHitsRayResultCallback_getRayFromWorld(btCollisionWorld_AllHitsRayResultCallback* obj, btVector3* value);
	EXPORT void btCollisionWorld_AllHitsRayResultCallback_getRayToWorld(btCollisionWorld_AllHitsRayResultCallback* obj, btVector3* value);
	EXPORT void btCollisionWorld_AllHitsRayResultCallback_setRayFromWorld(btCollisionWorld_AllHitsRayResultCallback* obj, const btVector3* value);
	EXPORT void btCollisionWorld_AllHitsRayResultCallback_setRayToWorld(btCollisionWorld_AllHitsRayResultCallback* obj, const btVector3* value);

	EXPORT btCollisionWorld_ClosestConvexResultCallback* btCollisionWorld_ClosestConvexResultCallback_new(const btVector3* convexFromWorld, const btVector3* convexToWorld);
	EXPORT void btCollisionWorld_ClosestConvexResultCallback_getConvexFromWorld(btCollisionWorld_ClosestConvexResultCallback* obj, btVector3* value);
	EXPORT void btCollisionWorld_ClosestConvexResultCallback_getConvexToWorld(btCollisionWorld_ClosestConvexResultCallback* obj, btVector3* value);
	EXPORT const btCollisionObject* btCollisionWorld_ClosestConvexResultCallback_getHitCollisionObject(btCollisionWorld_ClosestConvexResultCallback* obj);
	EXPORT void btCollisionWorld_ClosestConvexResultCallback_getHitNormalWorld(btCollisionWorld_ClosestConvexResultCallback* obj, btVector3* value);
	EXPORT void btCollisionWorld_ClosestConvexResultCallback_getHitPointWorld(btCollisionWorld_ClosestConvexResultCallback* obj, btVector3* value);
	EXPORT void btCollisionWorld_ClosestConvexResultCallback_setConvexFromWorld(btCollisionWorld_ClosestConvexResultCallback* obj, const btVector3* value);
	EXPORT void btCollisionWorld_ClosestConvexResultCallback_setConvexToWorld(btCollisionWorld_ClosestConvexResultCallback* obj, const btVector3* value);
	EXPORT void btCollisionWorld_ClosestConvexResultCallback_setHitCollisionObject(btCollisionWorld_ClosestConvexResultCallback* obj, const btCollisionObject* value);
	EXPORT void btCollisionWorld_ClosestConvexResultCallback_setHitNormalWorld(btCollisionWorld_ClosestConvexResultCallback* obj, const btVector3* value);
	EXPORT void btCollisionWorld_ClosestConvexResultCallback_setHitPointWorld(btCollisionWorld_ClosestConvexResultCallback* obj, const btVector3* value);

	EXPORT btCollisionWorld_ClosestRayResultCallback* btCollisionWorld_ClosestRayResultCallback_new(const btVector3* rayFromWorld, const btVector3* rayToWorld);
	EXPORT void btCollisionWorld_ClosestRayResultCallback_getHitNormalWorld(btCollisionWorld_ClosestRayResultCallback* obj, btVector3* value);
	EXPORT void btCollisionWorld_ClosestRayResultCallback_getHitPointWorld(btCollisionWorld_ClosestRayResultCallback* obj, btVector3* value);
	EXPORT void btCollisionWorld_ClosestRayResultCallback_getRayFromWorld(btCollisionWorld_ClosestRayResultCallback* obj, btVector3* value);
	EXPORT void btCollisionWorld_ClosestRayResultCallback_getRayToWorld(btCollisionWorld_ClosestRayResultCallback* obj, btVector3* value);
	EXPORT void btCollisionWorld_ClosestRayResultCallback_setHitNormalWorld(btCollisionWorld_ClosestRayResultCallback* obj, const btVector3* value);
	EXPORT void btCollisionWorld_ClosestRayResultCallback_setHitPointWorld(btCollisionWorld_ClosestRayResultCallback* obj, const btVector3* value);
	EXPORT void btCollisionWorld_ClosestRayResultCallback_setRayFromWorld(btCollisionWorld_ClosestRayResultCallback* obj, const btVector3* value);
	EXPORT void btCollisionWorld_ClosestRayResultCallback_setRayToWorld(btCollisionWorld_ClosestRayResultCallback* obj, const btVector3* value);

	EXPORT btCollisionWorld_ContactResultCallbackWrapper* btCollisionWorld_ContactResultCallbackWrapper_new(
		p_btCollisionWorld_ContactResultCallback_addSingleResult addSingleResultCallback,
		p_btCollisionWorld_ContactResultCallback_needsCollision needsCollisionCallback);
	EXPORT bool btCollisionWorld_ContactResultCallbackWrapper_needsCollision(btCollisionWorld_ContactResultCallbackWrapper* obj, btBroadphaseProxy* proxy0);

	EXPORT btScalar btCollisionWorld_ContactResultCallback_addSingleResult(btCollisionWorld_ContactResultCallback* obj, btManifoldPoint* cp, const btCollisionObjectWrapper* colObj0Wrap, int partId0, int index0, const btCollisionObjectWrapper* colObj1Wrap, int partId1, int index1);
	EXPORT btScalar btCollisionWorld_ContactResultCallback_getClosestDistanceThreshold(btCollisionWorld_ContactResultCallback* obj);
	EXPORT int btCollisionWorld_ContactResultCallback_getCollisionFilterGroup(btCollisionWorld_ContactResultCallback* obj);
	EXPORT int btCollisionWorld_ContactResultCallback_getCollisionFilterMask(btCollisionWorld_ContactResultCallback* obj);
	EXPORT bool btCollisionWorld_ContactResultCallback_needsCollision(btCollisionWorld_ContactResultCallback* obj, btBroadphaseProxy* proxy0);
	EXPORT void btCollisionWorld_ContactResultCallback_setClosestDistanceThreshold(btCollisionWorld_ContactResultCallback* obj, btScalar value);
	EXPORT void btCollisionWorld_ContactResultCallback_setCollisionFilterGroup(btCollisionWorld_ContactResultCallback* obj, int value);
	EXPORT void btCollisionWorld_ContactResultCallback_setCollisionFilterMask(btCollisionWorld_ContactResultCallback* obj, int value);
	EXPORT void btCollisionWorld_ContactResultCallback_delete(btCollisionWorld_ContactResultCallback* obj);

	EXPORT btCollisionWorld_ConvexResultCallbackWrapper* btCollisionWorld_ConvexResultCallbackWrapper_new(
		p_btCollisionWorld_ConvexResultCallback_addSingleResult addSingleResultCallback,
		p_btCollisionWorld_ConvexResultCallback_needsCollision needsCollisionCallback);
	EXPORT bool btCollisionWorld_ConvexResultCallbackWrapper_needsCollision(btCollisionWorld_ConvexResultCallbackWrapper* obj, btBroadphaseProxy* proxy0);

	EXPORT btScalar btCollisionWorld_ConvexResultCallback_addSingleResult(btCollisionWorld_ConvexResultCallback* obj, btCollisionWorld_LocalConvexResult* convexResult, bool normalInWorldSpace);
	EXPORT btScalar btCollisionWorld_ConvexResultCallback_getClosestHitFraction(btCollisionWorld_ConvexResultCallback* obj);
	EXPORT int btCollisionWorld_ConvexResultCallback_getCollisionFilterGroup(btCollisionWorld_ConvexResultCallback* obj);
	EXPORT int btCollisionWorld_ConvexResultCallback_getCollisionFilterMask(btCollisionWorld_ConvexResultCallback* obj);
	EXPORT bool btCollisionWorld_ConvexResultCallback_hasHit(btCollisionWorld_ConvexResultCallback* obj);
	EXPORT bool btCollisionWorld_ConvexResultCallback_needsCollision(btCollisionWorld_ConvexResultCallback* obj, btBroadphaseProxy* proxy0);
	EXPORT void btCollisionWorld_ConvexResultCallback_setClosestHitFraction(btCollisionWorld_ConvexResultCallback* obj, btScalar value);
	EXPORT void btCollisionWorld_ConvexResultCallback_setCollisionFilterGroup(btCollisionWorld_ConvexResultCallback* obj, int value);
	EXPORT void btCollisionWorld_ConvexResultCallback_setCollisionFilterMask(btCollisionWorld_ConvexResultCallback* obj, int value);
	EXPORT void btCollisionWorld_ConvexResultCallback_delete(btCollisionWorld_ConvexResultCallback* obj);

	EXPORT btCollisionWorld_LocalConvexResult* btCollisionWorld_LocalConvexResult_new(const btCollisionObject* hitCollisionObject, btCollisionWorld_LocalShapeInfo* localShapeInfo, const btVector3* hitNormalLocal, const btVector3* hitPointLocal, btScalar hitFraction);
	EXPORT const btCollisionObject* btCollisionWorld_LocalConvexResult_getHitCollisionObject(btCollisionWorld_LocalConvexResult* obj);
	EXPORT btScalar btCollisionWorld_LocalConvexResult_getHitFraction(btCollisionWorld_LocalConvexResult* obj);
	EXPORT void btCollisionWorld_LocalConvexResult_getHitNormalLocal(btCollisionWorld_LocalConvexResult* obj, btVector3* value);
	EXPORT void btCollisionWorld_LocalConvexResult_getHitPointLocal(btCollisionWorld_LocalConvexResult* obj, btVector3* value);
	EXPORT btCollisionWorld_LocalShapeInfo* btCollisionWorld_LocalConvexResult_getLocalShapeInfo(btCollisionWorld_LocalConvexResult* obj);
	EXPORT void btCollisionWorld_LocalConvexResult_setHitCollisionObject(btCollisionWorld_LocalConvexResult* obj, const btCollisionObject* value);
	EXPORT void btCollisionWorld_LocalConvexResult_setHitFraction(btCollisionWorld_LocalConvexResult* obj, btScalar value);
	EXPORT void btCollisionWorld_LocalConvexResult_setHitNormalLocal(btCollisionWorld_LocalConvexResult* obj, const btVector3* value);
	EXPORT void btCollisionWorld_LocalConvexResult_setHitPointLocal(btCollisionWorld_LocalConvexResult* obj, const btVector3* value);
	EXPORT void btCollisionWorld_LocalConvexResult_setLocalShapeInfo(btCollisionWorld_LocalConvexResult* obj, btCollisionWorld_LocalShapeInfo* value);
	EXPORT void btCollisionWorld_LocalConvexResult_delete(btCollisionWorld_LocalConvexResult* obj);

	EXPORT btCollisionWorld_LocalRayResult* btCollisionWorld_LocalRayResult_new(const btCollisionObject* collisionObject, btCollisionWorld_LocalShapeInfo* localShapeInfo, const btVector3* hitNormalLocal, btScalar hitFraction);
	EXPORT const btCollisionObject* btCollisionWorld_LocalRayResult_getCollisionObject(btCollisionWorld_LocalRayResult* obj);
	EXPORT btScalar btCollisionWorld_LocalRayResult_getHitFraction(btCollisionWorld_LocalRayResult* obj);
	EXPORT void btCollisionWorld_LocalRayResult_getHitNormalLocal(btCollisionWorld_LocalRayResult* obj, btVector3* value);
	EXPORT btCollisionWorld_LocalShapeInfo* btCollisionWorld_LocalRayResult_getLocalShapeInfo(btCollisionWorld_LocalRayResult* obj);
	EXPORT void btCollisionWorld_LocalRayResult_setCollisionObject(btCollisionWorld_LocalRayResult* obj, const btCollisionObject* value);
	EXPORT void btCollisionWorld_LocalRayResult_setHitFraction(btCollisionWorld_LocalRayResult* obj, btScalar value);
	EXPORT void btCollisionWorld_LocalRayResult_setHitNormalLocal(btCollisionWorld_LocalRayResult* obj, const btVector3* value);
	EXPORT void btCollisionWorld_LocalRayResult_setLocalShapeInfo(btCollisionWorld_LocalRayResult* obj, btCollisionWorld_LocalShapeInfo* value);
	EXPORT void btCollisionWorld_LocalRayResult_delete(btCollisionWorld_LocalRayResult* obj);

	EXPORT btCollisionWorld_LocalShapeInfo* btCollisionWorld_LocalShapeInfo_new();
	EXPORT int btCollisionWorld_LocalShapeInfo_getShapePart(btCollisionWorld_LocalShapeInfo* obj);
	EXPORT int btCollisionWorld_LocalShapeInfo_getTriangleIndex(btCollisionWorld_LocalShapeInfo* obj);
	EXPORT void btCollisionWorld_LocalShapeInfo_setShapePart(btCollisionWorld_LocalShapeInfo* obj, int value);
	EXPORT void btCollisionWorld_LocalShapeInfo_setTriangleIndex(btCollisionWorld_LocalShapeInfo* obj, int value);
	EXPORT void btCollisionWorld_LocalShapeInfo_delete(btCollisionWorld_LocalShapeInfo* obj);

	EXPORT btCollisionWorld_RayResultCallbackWrapper* btCollisionWorld_RayResultCallbackWrapper_new(
		p_btCollisionWorld_RayResultCallback_addSingleResult addSingleResultCallback,
		p_btCollisionWorld_RayResultCallback_needsCollision needsCollisionCallback);
	EXPORT bool btCollisionWorld_RayResultCallbackWrapper_needsCollision(btCollisionWorld_RayResultCallbackWrapper* obj, btBroadphaseProxy* proxy0);

	EXPORT btScalar btCollisionWorld_RayResultCallback_addSingleResult(btCollisionWorld_RayResultCallback* obj, btCollisionWorld_LocalRayResult* rayResult, bool normalInWorldSpace);
	EXPORT btScalar btCollisionWorld_RayResultCallback_getClosestHitFraction(btCollisionWorld_RayResultCallback* obj);
	EXPORT int btCollisionWorld_RayResultCallback_getCollisionFilterGroup(btCollisionWorld_RayResultCallback* obj);
	EXPORT int btCollisionWorld_RayResultCallback_getCollisionFilterMask(btCollisionWorld_RayResultCallback* obj);
	EXPORT const btCollisionObject* btCollisionWorld_RayResultCallback_getCollisionObject(btCollisionWorld_RayResultCallback* obj);
	EXPORT unsigned int btCollisionWorld_RayResultCallback_getFlags(btCollisionWorld_RayResultCallback* obj);
	EXPORT bool btCollisionWorld_RayResultCallback_hasHit(btCollisionWorld_RayResultCallback* obj);
	EXPORT bool btCollisionWorld_RayResultCallback_needsCollision(btCollisionWorld_RayResultCallback* obj, btBroadphaseProxy* proxy0);
	EXPORT void btCollisionWorld_RayResultCallback_setClosestHitFraction(btCollisionWorld_RayResultCallback* obj, btScalar value);
	EXPORT void btCollisionWorld_RayResultCallback_setCollisionFilterGroup(btCollisionWorld_RayResultCallback* obj, int value);
	EXPORT void btCollisionWorld_RayResultCallback_setCollisionFilterMask(btCollisionWorld_RayResultCallback* obj, int value);
	EXPORT void btCollisionWorld_RayResultCallback_setCollisionObject(btCollisionWorld_RayResultCallback* obj, const btCollisionObject* value);
	EXPORT void btCollisionWorld_RayResultCallback_setFlags(btCollisionWorld_RayResultCallback* obj, unsigned int value);
	EXPORT void btCollisionWorld_RayResultCallback_delete(btCollisionWorld_RayResultCallback* obj);

	EXPORT btCollisionWorld* btCollisionWorld_new(btDispatcher* dispatcher, btBroadphaseInterface* broadphasePairCache, btCollisionConfiguration* collisionConfiguration);
	EXPORT void btCollisionWorld_addCollisionObject(btCollisionWorld* obj, btCollisionObject* collisionObject);
	EXPORT void btCollisionWorld_addCollisionObject2(btCollisionWorld* obj, btCollisionObject* collisionObject, int collisionFilterGroup);
	EXPORT void btCollisionWorld_addCollisionObject3(btCollisionWorld* obj, btCollisionObject* collisionObject, int collisionFilterGroup, int collisionFilterMask);
	EXPORT void btCollisionWorld_computeOverlappingPairs(btCollisionWorld* obj);
	EXPORT void btCollisionWorld_contactPairTest(btCollisionWorld* obj, btCollisionObject* colObjA, btCollisionObject* colObjB, btCollisionWorld_ContactResultCallback* resultCallback);
	EXPORT void btCollisionWorld_contactTest(btCollisionWorld* obj, btCollisionObject* colObj, btCollisionWorld_ContactResultCallback* resultCallback);
	EXPORT void btCollisionWorld_convexSweepTest(btCollisionWorld* obj, const btConvexShape* castShape, const btTransform* from, const btTransform* to, btCollisionWorld_ConvexResultCallback* resultCallback, btScalar allowedCcdPenetration);
	EXPORT void btCollisionWorld_debugDrawObject(btCollisionWorld* obj, const btTransform* worldTransform, const btCollisionShape* shape, const btVector3* color);
	EXPORT void btCollisionWorld_debugDrawWorld(btCollisionWorld* obj);
	EXPORT btBroadphaseInterface* btCollisionWorld_getBroadphase(btCollisionWorld* obj);
	EXPORT btAlignedObjectArray_btCollisionObjectPtr* btCollisionWorld_getCollisionObjectArray(btCollisionWorld* obj);
	EXPORT btIDebugDraw* btCollisionWorld_getDebugDrawer(btCollisionWorld* obj);
	EXPORT btDispatcher* btCollisionWorld_getDispatcher(btCollisionWorld* obj);
	EXPORT btDispatcherInfo* btCollisionWorld_getDispatchInfo(btCollisionWorld* obj);
	EXPORT bool btCollisionWorld_getForceUpdateAllAabbs(btCollisionWorld* obj);
	EXPORT int btCollisionWorld_getNumCollisionObjects(btCollisionWorld* obj);
	EXPORT btOverlappingPairCache* btCollisionWorld_getPairCache(btCollisionWorld* obj);
	EXPORT void btCollisionWorld_objectQuerySingle(const btConvexShape* castShape, const btTransform* rayFromTrans, const btTransform* rayToTrans, btCollisionObject* collisionObject, const btCollisionShape* collisionShape, const btTransform* colObjWorldTransform, btCollisionWorld_ConvexResultCallback* resultCallback, btScalar allowedPenetration);
	EXPORT void btCollisionWorld_objectQuerySingleInternal(const btConvexShape* castShape, const btTransform* convexFromTrans, const btTransform* convexToTrans, const btCollisionObjectWrapper* colObjWrap, btCollisionWorld_ConvexResultCallback* resultCallback, btScalar allowedPenetration);
	EXPORT void btCollisionWorld_performDiscreteCollisionDetection(btCollisionWorld* obj);
	EXPORT void btCollisionWorld_rayTest(btCollisionWorld* obj, const btVector3* rayFromWorld, const btVector3* rayToWorld, btCollisionWorld_RayResultCallback* resultCallback);
	EXPORT void btCollisionWorld_rayTestSingle(const btTransform* rayFromTrans, const btTransform* rayToTrans, btCollisionObject* collisionObject, const btCollisionShape* collisionShape, const btTransform* colObjWorldTransform, btCollisionWorld_RayResultCallback* resultCallback);
	EXPORT void btCollisionWorld_rayTestSingleInternal(const btTransform* rayFromTrans, const btTransform* rayToTrans, const btCollisionObjectWrapper* collisionObjectWrap, btCollisionWorld_RayResultCallback* resultCallback);
	EXPORT void btCollisionWorld_removeCollisionObject(btCollisionWorld* obj, btCollisionObject* collisionObject);
	EXPORT void btCollisionWorld_serialize(btCollisionWorld* obj, btSerializer* serializer);
	EXPORT void btCollisionWorld_setBroadphase(btCollisionWorld* obj, btBroadphaseInterface* pairCache);
	EXPORT void btCollisionWorld_setDebugDrawer(btCollisionWorld* obj, btIDebugDraw* debugDrawer);
	EXPORT void btCollisionWorld_setForceUpdateAllAabbs(btCollisionWorld* obj, bool forceUpdateAllAabbs);
	EXPORT void btCollisionWorld_updateAabbs(btCollisionWorld* obj);
	EXPORT void btCollisionWorld_updateSingleAabb(btCollisionWorld* obj, btCollisionObject* colObj);
	EXPORT void btCollisionWorld_delete(btCollisionWorld* obj);
	//!!!!betauser
	EXPORT void* btCollisionWorld_callCustomMethod(btCollisionWorld* obj, int message, void* param1, void* param2);
#ifdef __cplusplus
}
#endif
