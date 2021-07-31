#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btMultiSphereShape* btMultiSphereShape_new(const btScalar* positions, const btScalar* radi, int numSpheres);
	EXPORT btMultiSphereShape* btMultiSphereShape_new2(const btVector3* positions, const btScalar* radi, int numSpheres);
	EXPORT int btMultiSphereShape_getSphereCount(btMultiSphereShape* obj);
	EXPORT void btMultiSphereShape_getSpherePosition(btMultiSphereShape* obj, int index, btVector3* value);
	EXPORT btScalar btMultiSphereShape_getSphereRadius(btMultiSphereShape* obj, int index);
#ifdef __cplusplus
}
#endif
