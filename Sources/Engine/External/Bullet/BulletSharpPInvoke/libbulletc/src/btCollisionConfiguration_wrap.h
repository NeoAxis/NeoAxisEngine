#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btCollisionAlgorithmCreateFunc* btCollisionConfiguration_getCollisionAlgorithmCreateFunc(btCollisionConfiguration* obj, int proxyType0, int proxyType1);
	EXPORT btPoolAllocator* btCollisionConfiguration_getCollisionAlgorithmPool(btCollisionConfiguration* obj);
	EXPORT btPoolAllocator* btCollisionConfiguration_getPersistentManifoldPool(btCollisionConfiguration* obj);
	EXPORT void btCollisionConfiguration_delete(btCollisionConfiguration* obj);
#ifdef __cplusplus
}
#endif
