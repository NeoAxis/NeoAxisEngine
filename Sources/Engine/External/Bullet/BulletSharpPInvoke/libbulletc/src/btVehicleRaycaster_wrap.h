#include "main.h"

#ifdef __cplusplus
extern "C" {
#endif
	EXPORT btVehicleRaycaster_btVehicleRaycasterResult* btVehicleRaycaster_btVehicleRaycasterResult_new();
	EXPORT btScalar btVehicleRaycaster_btVehicleRaycasterResult_getDistFraction(btVehicleRaycaster_btVehicleRaycasterResult* obj);
	EXPORT void btVehicleRaycaster_btVehicleRaycasterResult_getHitNormalInWorld(btVehicleRaycaster_btVehicleRaycasterResult* obj, btVector3* value);
	EXPORT void btVehicleRaycaster_btVehicleRaycasterResult_getHitPointInWorld(btVehicleRaycaster_btVehicleRaycasterResult* obj, btVector3* value);
	EXPORT void btVehicleRaycaster_btVehicleRaycasterResult_setDistFraction(btVehicleRaycaster_btVehicleRaycasterResult* obj, btScalar value);
	EXPORT void btVehicleRaycaster_btVehicleRaycasterResult_setHitNormalInWorld(btVehicleRaycaster_btVehicleRaycasterResult* obj, const btVector3* value);
	EXPORT void btVehicleRaycaster_btVehicleRaycasterResult_setHitPointInWorld(btVehicleRaycaster_btVehicleRaycasterResult* obj, const btVector3* value);
	EXPORT void btVehicleRaycaster_btVehicleRaycasterResult_delete(btVehicleRaycaster_btVehicleRaycasterResult* obj);

	EXPORT void* btVehicleRaycaster_castRay(btVehicleRaycaster* obj, const btVector3* from, const btVector3* to, btVehicleRaycaster_btVehicleRaycasterResult* result);
	EXPORT void btVehicleRaycaster_delete(btVehicleRaycaster* obj);
#ifdef __cplusplus
}
#endif
