#include <BulletDynamics/Vehicle/btVehicleRaycaster.h>

#include "conversion.h"
#include "btVehicleRaycaster_wrap.h"

#ifndef BULLETC_DISABLE_IACTION_CLASSES

btVehicleRaycaster_btVehicleRaycasterResult* btVehicleRaycaster_btVehicleRaycasterResult_new()
{
	return new btVehicleRaycaster::btVehicleRaycasterResult();
}

btScalar btVehicleRaycaster_btVehicleRaycasterResult_getDistFraction(btVehicleRaycaster_btVehicleRaycasterResult* obj)
{
	return obj->m_distFraction;
}

void btVehicleRaycaster_btVehicleRaycasterResult_getHitNormalInWorld(btVehicleRaycaster_btVehicleRaycasterResult* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_hitNormalInWorld);
}

void btVehicleRaycaster_btVehicleRaycasterResult_getHitPointInWorld(btVehicleRaycaster_btVehicleRaycasterResult* obj,
	btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_hitPointInWorld);
}

void btVehicleRaycaster_btVehicleRaycasterResult_setDistFraction(btVehicleRaycaster_btVehicleRaycasterResult* obj,
	btScalar value)
{
	obj->m_distFraction = value;
}

void btVehicleRaycaster_btVehicleRaycasterResult_setHitNormalInWorld(btVehicleRaycaster_btVehicleRaycasterResult* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_hitNormalInWorld, value);
}

void btVehicleRaycaster_btVehicleRaycasterResult_setHitPointInWorld(btVehicleRaycaster_btVehicleRaycasterResult* obj,
	const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_hitPointInWorld, value);
}

void btVehicleRaycaster_btVehicleRaycasterResult_delete(btVehicleRaycaster_btVehicleRaycasterResult* obj)
{
	delete obj;
}


void* btVehicleRaycaster_castRay(btVehicleRaycaster* obj, const btVector3* from,
	const btVector3* to, btVehicleRaycaster_btVehicleRaycasterResult* result)
{
	BTVECTOR3_IN(from);
	BTVECTOR3_IN(to);
	return obj->castRay(BTVECTOR3_USE(from), BTVECTOR3_USE(to), *result);
}

void btVehicleRaycaster_delete(btVehicleRaycaster* obj)
{
	delete obj;
}

#endif
