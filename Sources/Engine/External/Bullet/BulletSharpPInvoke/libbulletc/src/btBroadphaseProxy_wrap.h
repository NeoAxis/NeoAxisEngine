#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT void btBroadphaseProxy_getAabbMax(btBroadphaseProxy* obj, btVector3* value);
	EXPORT void btBroadphaseProxy_getAabbMin(btBroadphaseProxy* obj, btVector3* value);
	EXPORT void* btBroadphaseProxy_getClientObject(btBroadphaseProxy* obj);
	EXPORT int btBroadphaseProxy_getCollisionFilterGroup(btBroadphaseProxy* obj);
	EXPORT int btBroadphaseProxy_getCollisionFilterMask(btBroadphaseProxy* obj);
	EXPORT int btBroadphaseProxy_getUid(btBroadphaseProxy* obj);
	EXPORT int btBroadphaseProxy_getUniqueId(btBroadphaseProxy* obj);
	EXPORT bool btBroadphaseProxy_isCompound(int proxyType);
	EXPORT bool btBroadphaseProxy_isConcave(int proxyType);
	EXPORT bool btBroadphaseProxy_isConvex(int proxyType);
	EXPORT bool btBroadphaseProxy_isConvex2d(int proxyType);
	EXPORT bool btBroadphaseProxy_isInfinite(int proxyType);
	EXPORT bool btBroadphaseProxy_isNonMoving(int proxyType);
	EXPORT bool btBroadphaseProxy_isPolyhedral(int proxyType);
	EXPORT bool btBroadphaseProxy_isSoftBody(int proxyType);
	EXPORT void btBroadphaseProxy_setAabbMax(btBroadphaseProxy* obj, const btVector3* value);
	EXPORT void btBroadphaseProxy_setAabbMin(btBroadphaseProxy* obj, const btVector3* value);
	EXPORT void btBroadphaseProxy_setClientObject(btBroadphaseProxy* obj, void* value);
	EXPORT void btBroadphaseProxy_setCollisionFilterGroup(btBroadphaseProxy* obj, int value);
	EXPORT void btBroadphaseProxy_setCollisionFilterMask(btBroadphaseProxy* obj, int value);
	EXPORT void btBroadphaseProxy_setUniqueId(btBroadphaseProxy* obj, int value);
	EXPORT void btBroadphaseProxy_delete(btBroadphaseProxy* obj);

	EXPORT btBroadphasePair* btBroadphasePair_new();
	EXPORT btBroadphasePair* btBroadphasePair_new2(const btBroadphasePair* other);
	EXPORT btBroadphasePair* btBroadphasePair_new3(btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1);
	EXPORT btCollisionAlgorithm* btBroadphasePair_getAlgorithm(btBroadphasePair* obj);
	EXPORT btBroadphaseProxy* btBroadphasePair_getPProxy0(btBroadphasePair* obj);
	EXPORT btBroadphaseProxy* btBroadphasePair_getPProxy1(btBroadphasePair* obj);
	EXPORT void btBroadphasePair_setAlgorithm(btBroadphasePair* obj, btCollisionAlgorithm* value);
	EXPORT void btBroadphasePair_setPProxy0(btBroadphasePair* obj, btBroadphaseProxy* value);
	EXPORT void btBroadphasePair_setPProxy1(btBroadphasePair* obj, btBroadphaseProxy* value);
	EXPORT void btBroadphasePair_delete(btBroadphasePair* obj);
#ifdef __cplusplus
}
#endif
