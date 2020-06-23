#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btAxisSweep3* btAxisSweep3_new(const btVector3* worldAabbMin, const btVector3* worldAabbMax, unsigned short maxHandles, btOverlappingPairCache* pairCache, bool disableRaycastAccelerator);
	EXPORT unsigned short btAxisSweep3_addHandle(btAxisSweep3* obj, const btVector3* aabbMin, const btVector3* aabbMax, void* pOwner, int collisionFilterGroup, int collisionFilterMask, btDispatcher* dispatcher);
	EXPORT btAxisSweep3Internal_unsigned_short_Handle* btAxisSweep3_getHandle(btAxisSweep3* obj, unsigned short index);
	EXPORT unsigned short btAxisSweep3_getNumHandles(btAxisSweep3* obj);
	EXPORT const btOverlappingPairCallback* btAxisSweep3_getOverlappingPairUserCallback(btAxisSweep3* obj);
	EXPORT void btAxisSweep3_quantize(btAxisSweep3* obj, unsigned short* out, const btVector3* point, int isMax);
	EXPORT void btAxisSweep3_removeHandle(btAxisSweep3* obj, unsigned short handle, btDispatcher* dispatcher);
	EXPORT void btAxisSweep3_setOverlappingPairUserCallback(btAxisSweep3* obj, btOverlappingPairCallback* pairCallback);
	EXPORT bool btAxisSweep3_testAabbOverlap(btAxisSweep3* obj, btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1);
	EXPORT void btAxisSweep3_unQuantize(btAxisSweep3* obj, btBroadphaseProxy* proxy, btVector3* aabbMin, btVector3* aabbMax);
	EXPORT void btAxisSweep3_updateHandle(btAxisSweep3* obj, unsigned short handle, const btVector3* aabbMin, const btVector3* aabbMax, btDispatcher* dispatcher);

	EXPORT bt32BitAxisSweep3* bt32BitAxisSweep3_new(const btVector3* worldAabbMin, const btVector3* worldAabbMax, unsigned int maxHandles, btOverlappingPairCache* pairCache, bool disableRaycastAccelerator);
	EXPORT unsigned int bt32BitAxisSweep3_addHandle(bt32BitAxisSweep3* obj, const btVector3* aabbMin, const btVector3* aabbMax, void* pOwner, int collisionFilterGroup, int collisionFilterMask, btDispatcher* dispatchery);
	EXPORT btAxisSweep3Internal_unsigned_int_Handle* bt32BitAxisSweep3_getHandle(bt32BitAxisSweep3* obj, unsigned int index);
	EXPORT unsigned int bt32BitAxisSweep3_getNumHandles(bt32BitAxisSweep3* obj);
	EXPORT const btOverlappingPairCallback* bt32BitAxisSweep3_getOverlappingPairUserCallback(bt32BitAxisSweep3* obj);
	EXPORT void bt32BitAxisSweep3_quantize(bt32BitAxisSweep3* obj, unsigned int* out, const btVector3* point, int isMax);
	EXPORT void bt32BitAxisSweep3_removeHandle(bt32BitAxisSweep3* obj, unsigned int handle, btDispatcher* dispatcher);
	EXPORT void bt32BitAxisSweep3_setOverlappingPairUserCallback(bt32BitAxisSweep3* obj, btOverlappingPairCallback* pairCallback);
	EXPORT bool bt32BitAxisSweep3_testAabbOverlap(bt32BitAxisSweep3* obj, btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1);
	EXPORT void bt32BitAxisSweep3_unQuantize(bt32BitAxisSweep3* obj, btBroadphaseProxy* proxy, btVector3* aabbMin, btVector3* aabbMax);
	EXPORT void bt32BitAxisSweep3_updateHandle(bt32BitAxisSweep3* obj, unsigned int handle, const btVector3* aabbMin, const btVector3* aabbMax, btDispatcher* dispatcher);
#ifdef __cplusplus
}
#endif
