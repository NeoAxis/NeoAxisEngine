#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btBroadphasePair* btOverlappingPairCallback_addOverlappingPair(btOverlappingPairCallback* obj, btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1);
	EXPORT void* btOverlappingPairCallback_removeOverlappingPair(btOverlappingPairCallback* obj, btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1, btDispatcher* dispatcher);
	EXPORT void btOverlappingPairCallback_removeOverlappingPairsContainingProxy(btOverlappingPairCallback* obj, btBroadphaseProxy* proxy0, btDispatcher* dispatcher);
	EXPORT void btOverlappingPairCallback_delete(btOverlappingPairCallback* obj);
#ifdef __cplusplus
}
#endif
