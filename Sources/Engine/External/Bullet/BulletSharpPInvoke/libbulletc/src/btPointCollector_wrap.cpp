#include <BulletCollision/NarrowPhaseCollision/btPointCollector.h>

#include "conversion.h"
#include "btPointCollector_wrap.h"

btPointCollector* btPointCollector_new()
{
	return new btPointCollector();
}

btScalar btPointCollector_getDistance(btPointCollector* obj)
{
	return obj->m_distance;
}

bool btPointCollector_getHasResult(btPointCollector* obj)
{
	return obj->m_hasResult;
}

void btPointCollector_getNormalOnBInWorld(btPointCollector* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_normalOnBInWorld);
}

void btPointCollector_getPointInWorld(btPointCollector* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_pointInWorld);
}

void btPointCollector_setDistance(btPointCollector* obj, btScalar value)
{
	obj->m_distance = value;
}

void btPointCollector_setHasResult(btPointCollector* obj, bool value)
{
	obj->m_hasResult = value;
}

void btPointCollector_setNormalOnBInWorld(btPointCollector* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_normalOnBInWorld, value);
}

void btPointCollector_setPointInWorld(btPointCollector* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_pointInWorld, value);
}
