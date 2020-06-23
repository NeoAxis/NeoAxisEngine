#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btStaticPlaneShape* btStaticPlaneShape_new(const btVector3* planeNormal, btScalar planeConstant);
	EXPORT btScalar btStaticPlaneShape_getPlaneConstant(btStaticPlaneShape* obj);
	EXPORT void btStaticPlaneShape_getPlaneNormal(btStaticPlaneShape* obj, btVector3* value);
#ifdef __cplusplus
}
#endif
