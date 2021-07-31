#include <BulletCollision/BroadphaseCollision/btBroadphaseProxy.h>
#include <BulletCollision/BroadphaseCollision/btCollisionAlgorithm.h>

#include "conversion.h"
#include "btBroadphaseProxy_wrap.h"

void btBroadphaseProxy_getAabbMax(btBroadphaseProxy* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_aabbMax);
}

void btBroadphaseProxy_getAabbMin(btBroadphaseProxy* obj, btVector3* value)
{
	BTVECTOR3_SET(value, obj->m_aabbMin);
}

void* btBroadphaseProxy_getClientObject(btBroadphaseProxy* obj)
{
	return obj->m_clientObject;
}

int btBroadphaseProxy_getCollisionFilterGroup(btBroadphaseProxy* obj)
{
	return obj->m_collisionFilterGroup;
}

int btBroadphaseProxy_getCollisionFilterMask(btBroadphaseProxy* obj)
{
	return obj->m_collisionFilterMask;
}

int btBroadphaseProxy_getUid(btBroadphaseProxy* obj)
{
	return obj->getUid();
}

int btBroadphaseProxy_getUniqueId(btBroadphaseProxy* obj)
{
	return obj->m_uniqueId;
}

bool btBroadphaseProxy_isCompound(int proxyType)
{
	return btBroadphaseProxy::isCompound(proxyType);
}

bool btBroadphaseProxy_isConcave(int proxyType)
{
	return btBroadphaseProxy::isConcave(proxyType);
}

bool btBroadphaseProxy_isConvex(int proxyType)
{
	return btBroadphaseProxy::isConvex(proxyType);
}

bool btBroadphaseProxy_isConvex2d(int proxyType)
{
	return btBroadphaseProxy::isConvex2d(proxyType);
}

bool btBroadphaseProxy_isInfinite(int proxyType)
{
	return btBroadphaseProxy::isInfinite(proxyType);
}

bool btBroadphaseProxy_isNonMoving(int proxyType)
{
	return btBroadphaseProxy::isNonMoving(proxyType);
}

bool btBroadphaseProxy_isPolyhedral(int proxyType)
{
	return btBroadphaseProxy::isPolyhedral(proxyType);
}

bool btBroadphaseProxy_isSoftBody(int proxyType)
{
	return btBroadphaseProxy::isSoftBody(proxyType);
}

void btBroadphaseProxy_setAabbMax(btBroadphaseProxy* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_aabbMax, value);
}

void btBroadphaseProxy_setAabbMin(btBroadphaseProxy* obj, const btVector3* value)
{
	BTVECTOR3_COPY(&obj->m_aabbMin, value);
}

void btBroadphaseProxy_setClientObject(btBroadphaseProxy* obj, void* value)
{
	obj->m_clientObject = value;
}

void btBroadphaseProxy_setCollisionFilterGroup(btBroadphaseProxy* obj, int value)
{
	obj->m_collisionFilterGroup = value;
}

void btBroadphaseProxy_setCollisionFilterMask(btBroadphaseProxy* obj, int value)
{
	obj->m_collisionFilterMask = value;
}

void btBroadphaseProxy_setUniqueId(btBroadphaseProxy* obj, int value)
{
	obj->m_uniqueId = value;
}

void btBroadphaseProxy_delete(btBroadphaseProxy* obj)
{
	delete obj;
}


btBroadphasePair* btBroadphasePair_new()
{
	return new btBroadphasePair();
}

btBroadphasePair* btBroadphasePair_new2(const btBroadphasePair* other)
{
	return new btBroadphasePair(*other);
}

btBroadphasePair* btBroadphasePair_new3(btBroadphaseProxy* proxy0, btBroadphaseProxy* proxy1)
{
	return new btBroadphasePair(*proxy0, *proxy1);
}

btCollisionAlgorithm* btBroadphasePair_getAlgorithm(btBroadphasePair* obj)
{
	return obj->m_algorithm;
}

btBroadphaseProxy* btBroadphasePair_getPProxy0(btBroadphasePair* obj)
{
	return obj->m_pProxy0;
}

btBroadphaseProxy* btBroadphasePair_getPProxy1(btBroadphasePair* obj)
{
	return obj->m_pProxy1;
}

void btBroadphasePair_setAlgorithm(btBroadphasePair* obj, btCollisionAlgorithm* value)
{
	obj->m_algorithm = value;
}

void btBroadphasePair_setPProxy0(btBroadphasePair* obj, btBroadphaseProxy* value)
{
	obj->m_pProxy0 = value;
}

void btBroadphasePair_setPProxy1(btBroadphasePair* obj, btBroadphaseProxy* value)
{
	obj->m_pProxy1 = value;
}

void btBroadphasePair_delete(btBroadphasePair* obj)
{
	delete obj;
}
